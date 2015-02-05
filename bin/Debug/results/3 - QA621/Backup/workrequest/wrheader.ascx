<%@ Control Language="C#" AutoEventWireup="true" CodeFile="wrheader.ascx.cs" Inherits="workrequest_wrheader" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<link type="text/css" href="../styles/mainmenu.css" rel="stylesheet" />
<script type="text/javascript" src="../Jscript/Header.js"></script>
<script type="text/javascript" src="../Jscript/Codes.js"></script>
<script type="text/javascript" src="../Jscript/Validation.js"></script>
<script type="text/javascript" src="../Jscript/StopDoubleClick.js"></script>
<script type="text/javascript">

    function changeStatus() {
        var wrnum = "<%=m_wrnum %>";
        //statuswindow = dhtmlmodal.open('StatusBox', 'iframe', 'Wostatus.aspx?wonum=' + wonum, 'Change Status', 'width=700px,height=600px,center=1,resize=1,scrolling=1')
        var oWnd;
        if (GetRadWindow() == null) {
            oWnd = radopen('Wrstatus.aspx?wrnum=' + wrnum, null);
            oWnd.set_modal(true);
        }
    }

    function addToWorklist() {
        var obj = document.getElementById("txtwrnum");
        var wrnum = obj.value;
        obj = document.getElementById("ifValidate");
        obj.src = "../Codes/Addworklist.aspx?module=wr&linkid=" + wrnum + "&type=worklist";
    }

    function opencalendar() {
        var oWnd = radopen('Wrcalendar.aspx?wrnum=<%=m_wrnum %>', null);
        oWnd.set_modal(true);
    }

    function openlink(URL, subfolder) {
        var urltype = URL.substr(0, 4);
        var urladdr = URL.substr(5);

        if (urltype == "file")
            URL = "<%=linksrvr%>" + subfolder + "/" + urladdr;
        else
            URL = urladdr;
        newwin = window.open(URL);
        setTimeout('newwin.focus();', 100);
        //  window.close();
      }

    function showmap() {
      var URL = "../codes/map.aspx?location=<%=m_location %>";
      var dWnd;
      if (GetRadWindow() == null) {
        dWnd = radopen(URL, null);
        dWnd.set_modal(true);
      }
    }

    function toolbarclicked(sender, eventArgs) {
        var button = eventArgs.get_item();
        var commandname = button.get_commandName();
        switch (commandname) {
            case "search":
                if (WarnUser())
                    settoQueryMode("wrmain.aspx");
                break;
            case "newwo":
                if (WarnUser())
                    window.location.href = 'Wrmain.aspx?mode=new&numbering=manual';
                break;
            case "autowo":
                if (WarnUser())
                    window.location.href = 'Wrmain.aspx?mode=new&numbering=auto';
                break;
            case "lookup":
                LookupClicked();
                break;
            case "save":
              if (checksave()) {
                  if (Page_ClientValidate('')) {
                    var mode = $('#hidMode', parent.mainmodule.document).val();
                    if (mode == "new") {
                      createwr();
                    }
                    else if (mode == "edit") {
                      savewr();
                    }

                    else if (mode == "duplicate") {
                      var estimate = 1;
                      duplicatewr(estimate);
                    }
                  }
                }
                break;
            case "complete":
                noPrompt();
                if (Page_ClientValidate('')) {
                    __doPostBack("complete", "");
                }
                break;
            case "status":
                changeStatus();
                break;
            case "map":
                showmap();
                break;
            case "duplicate":
                if (WarnUser()) {
                    var obj = document.getElementById("txtwrnum");
                    var wrnum = obj.value;
                    window.location.href = 'Wrmain.aspx?mode=duplicate&wrnum=' + wrnum;
                }
                //duplicateWorkOrder();
                break;
            case "print":
                PrintForm("<%=defaultprintcounter %>", "WRPrintForm", "<%=m_wrnum %>");
                break;
            case "batchprint":
                PrintBatchForms("<%=defaultprintcounter %>", "WRPrintForm", "WRNum");
                break;
            case "email":
                sendEmail("WorkRequest", "<%=m_wrnum%>");
                break;
            case "picture":
                break;
            case "linkeddoc":
                LinkDoc('workrequest', '<%=m_wrnum %>');
                break;
            case "calendar":
                opencalendar();
                break;
            case "savequery":
                SaveQuery();
                break;
        }
    }

    function setSelectedIndex() {
        var combo = $find("<%= toolbarcombobox.ClientID %>");
        combo.trackChanges();
        combo.get_items().getItem(0).select();
        combo.updateClientState();
        combo.commitChanges();
        if (combo.get_dropDownVisible())
            combo.hideDropDown();
        combo.get_inputDomElement().blur();
        combo._raiseClientBlur(e);
        combo._focused = false;
    }

</script>
<script type="text/javascript">

    function OnClientSelectedIndexChanged(sender, eventArgs) {
        var item = eventArgs.get_item();
    }

    function checksave() {

        if ("<%=m_mode %>" == "edit") {

            var proc = document.getElementById("txtprocnum").value;
            var oldproc = document.getElementById("hidprocnum").value;

            if ((proc != "") && (proc != oldproc)) {
                resp = window.confirm('<%= m_msg["T4"] %>')
                if (resp) {
                    noPrompt();
                    return true;
                }
                else
                    return false;
            }
            else {
                return true;
            }
        }
        else {
            noPrompt();
            return true;
        }
    }    
    
</script>
<script type="text/javascript">
    function savewr() {

      $(document).ready(function () {
        var dbfield = "", dbval = "", procnum = "";
        var userid = '<%=Session["Login"] %>';
        var statuscode = $('input:hidden[name=hidTargetStatusCode]').val();
        var hidprocnum = $('input:hidden[name=hidprocnum]').val();
        var dirtylog = $('#txtdirtylog').val();
        var woxml = "<?xml version='1.0' encoding='UTF-8'?><workorders><workorder>";
        woxml = woxml + collectformall("WorkRequest");
        procnum = $("#txtprocnum").val();

        if (dirtylog != '') {
          woxml = woxml + "<dirtylog>" + dirtylog + "</dirtylog>";
        }

        woxml = woxml + "</workorder></workorders>";
        //            $.support.cors = true;

        if (!allowsubmit()) return;
        $.ajax({
          type: "GET",
          // type: "POST",
          url: "../InternalServices/ServiceWR.svc/SaveWorkRequest",
          data: { "xmlnvc": woxml },
          contentType: "application/json; charset=utf-8",
          dataType: "json",
          beforeSend: function (xhr, settings) {
            xhr.setRequestHeader("userid", userid);
            xhr.setRequestHeader("hidprocnum", hidprocnum);
          },
          success: function (data, status) {
            var result = data.d;
            var d_array = result.split("^");
            if (d_array[0] == "TRUE") {
              var newdirtylog;
              newdirtylog = parseInt(dirtylog) + 1;
              $('#txtdirtylog').val(newdirtylog);
              if (hidprocnum != procnum) {
                $('input:hidden[name=hidprocnum]').val(procnum);
              }
              savesucceeded(d_array[1]);
            }
            else if (d_array[0] == "FALSE") {
              alert(d_array[1]);
            }
            FormSubmitted = false;
          },
          error: OnGetMemberError
        });

      });

    }

    function createwr() {
      //$(document).ready(function () {
        var userid = '<%=Session["Login"] %>';
        var dbfield, dbval;
        var statuscode = $('input:hidden[name=hidTargetStatusCode]').val();
        var woxml = "<?xml version='1.0' encoding='UTF-8'?><workorders><workorder>";
        woxml = woxml + collectformall("WorkRequest");

        //if (statuscode != '') {
          //woxml = woxml + "<statuscode>" + statuscode + "</statuscode>";
        //}
        woxml = woxml + "</workorder></workorders>";
        if (!allowsubmit()) return;
        $.ajax({
          type: "GET",
          url: "../InternalServices/ServiceWR.svc/CreateWorkRequest",
          data: { "userid": userid, "xmlnvc": woxml },
          contentType: "application/json; charset=utf-8",
          dataType: "json",
          success: function (data, status) {
            createsucceeded(data);

            FormSubmitted = false;
          },
          error: OnGetMemberError
        });
      //});
          }

          function duplicatewr(copyfrom) {

            $(document).ready(function () {
              var userid = '<%=Session["Login"] %>';
              var dbfield, dbval;
              var statuscode = $('input:hidden[name=hidTargetStatusCode]').val();
              var woxml = "<?xml version='1.0' encoding='UTF-8'?><workorders><workorder>";
              woxml = woxml + collectform("WorkRequest");

              $('input:radio[name=rblchargeback]:checked').each(function () {
                dbval = $(this).val();
                //if (dbval != '') {
                woxml = woxml + "<chargeback>" + $(this).val() + "</chargeback>";
                //}
              });

              //if (statuscode != '') {
              //  woxml = woxml + "<statuscode>" + statuscode + "</statuscode>";
              //}

              woxml = woxml + "</workorder></workorders>";

              if (!allowsubmit()) return;
              $.ajax({
                type: "GET",
                url: "../InternalServices/ServiceWR.svc/CreateWorkRequest",
                data: { "userid": userid, "xmlnvc": woxml, "oldwrnum": "<%=m_wrnum %>" },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, status) {
                  createsucceeded(data);

                  FormSubmitted = false;
                },
                error: OnGetMemberError
              });
            });
          }

    function createsucceeded(result) {
        var wrnum = result.d;
        window.location.href = 'Wrmain.aspx?mode=edit&wrnum=' + wrnum;
    }

    function savesucceeded(wrnum) {
        window.location.href = 'Wrmain.aspx?mode=edit&wrnum=' + wrnum;
        // window.location.href =
        //alert(result.d);
        // alert(result.d + "--" + stat);
    }

    function succeeded(wrnum) {
        window.location.href = 'Wrmain.aspx?mode=edit&wrnum=' + wrnum;
    }

    function OnGetMemberError(request, status, error) {
        alert(status + "--" + error + "--" + request.responseText);
        // alert(status);
        FormSubmitted = false;
    }

    /*
    function gridrefresh() {
        masterTable = $find("grdlabourest").get_masterTableView();
        masterTable.rebind();
    }
    */


    
</script>
<asp:Literal ID="litFrameScript" runat="server" />
<asp:Literal ID="litDateLCIDScript" runat="server" />
<asp:Literal ID="litScript" runat="server" />
<div id="header-wrap">
    <div style="position: fixed; width: 50px; height: 32px; top: 0px; left: 10px; z-index: 500;">
        <a href="javascript:ModuleMenu();" onmouseover="imageChangeover('imgMainMenu','../IMAGES/tablogo_over.png')"
            onmouseout="imageChangeover('imgMainMenu','../IMAGES/tablogo.png')">
            <img id="imgMainMenu" src="../IMAGES/tablogo.png" width="46" height="35" style="position: relative;
                top: -3px; padding: 0 20px 0 0;" border="0" alt="" title="" /></a>
    </div>
    <div>
        <ul id="topmainmenu_table" class="topmainmenu_menulist" style="display: block; width: 100%;
            height: 30px; position: relative; padding: 0 0 0 10px; margin: 0px; background-image: url(../IMAGES/tabback.png);
            background-repeat: repeat-x; z-index: 3;">
            <li>
                <img id="topmainmenu_1" src="../IMAGES/tablogo.png" width="46" height="35" style="position: relative;
                    top: -40px; padding: 0 20px 0 0;" border="0" alt="" title="" />
            </li>
            <li>
                <asp:HyperLink ID="tabhlkMain" runat="server">
                    <asp:Image ID="tabimgMain" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
                        Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
                    <asp:Label ID="tablblMain" runat="server" CssClass="toptabout" Text="Main" /></asp:HyperLink>
            </li>
            <!--
            <li>
                <asp:HyperLink ID="tabhlkAccounts" runat="server">
                    <asp:Image ID="tabimgAccounts" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
                        Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
                    <asp:Label ID="tablblAccounts" runat="server" CssClass="toptabout" Text="Accounts" /></asp:HyperLink>
            </li>
            -->
            <li>
                <asp:HyperLink ID="tabhlkHistory" runat="server">
                    <asp:Image ID="tabimgHistory" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
                        Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
                    <asp:Label ID="tablblHistory" runat="server" CssClass="toptabout" Text="History" /></asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="tabhlkWorkOrder" runat="server">
                    <asp:Image ID="tabimgWorkOrder" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
                        Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
                    <asp:Label ID="tablblWorkOrder" runat="server" CssClass="toptabout" Text="Work Order" /></asp:HyperLink>
            </li>
        </ul>
    </div>
    <br />
    <br />
    <div style="background-image: url(../IMAGES/toolbarback.png); background-repeat: repeat-x;
        position: relative; top: -28px; display: block; width: 100%; height: 64px; padding: 5px 0 0 20px;
        vertical-align: top; z-index: 5;; overflow:hidden">
        <telerik:RadToolBar ID="toolbar" runat="server">
        </telerik:RadToolBar>
    </div>
    <div style="position: relative; top: -34px; display: block; padding: 0px; margin: 0px;
        width: 100%; height: 30px; background-image: url(../IMAGES/statbar_back.png); background-repeat: repeat-x;
        z-index: 5;">
        <table width="100%">
            <tr valign="middle">
                <td width="20%" style="font-family: Arial, Helvetica, sans-serif; font-size: 8pt;
                    color: #FFFFFF;" valign="middle">
                    Current Operation:
                    <asp:Literal ID="litOperation" runat="server">Query</asp:Literal>
                </td>
                <td width="30%" style="font-family: Arial, Helvetica, sans-serif; font-size: 8pt;
                    color: #FFFFFF;" valign="middle">
                    <asp:PlaceHolder ID="plhLinkDoc" runat="server" />
                </td>
                <td width="30%" style="font-family: Arial, Helvetica, sans-serif; font-size: 8pt;
                    color: #FFFFFF;" valign="middle">
                    <asp:PlaceHolder ID="plhReports" runat="server" />
                </td>
                <td width="20%" style="font-family: Arial, Helvetica, sans-serif; font-size: 8pt;
                    color: #FFFFFF;" valign="middle">
                    <asp:PlaceHolder ID="plhQuery" runat="server" />
                </td>
            </tr>
        </table>
        <!--  <div style="  position:relative; top:8px; left:0px; font-family: Arial, Helvetica, sans-serif; font-size:8pt; color: #FFFFFF;">
        Current Operation: <asp:Literal ID="litOperation1" runat="server">Query</asp:Literal>
    </div>-->
    </div>
    <asp:HiddenField ID="hidCurrentRecord" runat="server" />
</div>
