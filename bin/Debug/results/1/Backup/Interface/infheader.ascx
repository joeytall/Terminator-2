<%@ Control Language="C#" AutoEventWireup="true" CodeFile="infheader.ascx.cs"
    Inherits="interface_infheader" %>
<link type="text/css" href="../styles/mainmenu.css" rel="stylesheet" />
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<script type="text/javascript" src="../Jscript/Header.js"></script>
<script type="text/javascript" src="../Jscript/Codes.js"></script>
<script type="text/javascript" src="../Jscript/Validation.js"></script>
<script type="text/javascript" src="../Jscript/StopDoubleClick.js"></script>
<script type="text/javascript">

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

    function toolbarclicked(sender, e) {
        var button = e.get_item();
        var commandname = button.get_commandName();
        switch (commandname) {
            case "search":
                if (WarnUser())
                    settoQueryMode("infmain.aspx");
                break;
            case "new":
                if (WarnUser())
                    window.location.href = 'infmain.aspx?mode=new';
                break;
            case "lookup":
                if (WarnUser())
                {
                    LookupClicked();
                }
                break;
            case "savequery":
                SaveQuery();
                break;
              case "save":
                noPrompt();
                if (Page_ClientValidate('')) {
                  var mode = $('#hidMode', parent.mainmodule.document).val();
                  if (mode == "edit")
                    updateinterface();
                  else if (mode == "new")
                    createinterface();
                  else if (mode == "duplicate")
                    duplicateinterface();
                }
                break;
              case "delete":
                var resp = window.confirm('Are you sure want to delete this interface?');
                if (resp) {
                  noPrompt();
                  deleteinterface();
                }
                break;
            case "email":
                sendEmail("Interface", "<%=m_interfacename%>");
                break;
            case "duplicate":
                if (WarnUser())
                    window.location.href = 'infmain.aspx?mode=duplicate&interfacename=<%=m_interfacename %>';
                break;
            case "print":
                printReport('txtinterfacename', 'infrpt')
                break;
            case "linkeddoc":
                LinkDoc('interface', '<%=m_interfacename %>');
                break;
            case "exporttemplate":
                __doPostBack("exporttemplate", "");
                break;
        }
          }

</script>
<asp:Literal ID="litFrameScript" runat="server" />
<asp:Literal ID="litDateLCIDScript" runat="server" />
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
        </ul>
    </div>
    <br />
    <br />
    <div style="background-image: url(../IMAGES/toolbarback.png); background-repeat: repeat-x;
        position: relative; top: -28px; display: block; width: 100%; height: 64px; padding: 5px 0 0 20px;
        vertical-align: top; z-index: 5;">
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
                    <span id="spnoperation"><asp:Literal ID="litOperation" runat="server">Query</asp:Literal></span>
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
    </div>
    <asp:HiddenField ID="hidCurrentRecord" runat="server" />
</div>

<script type="text/javascript">
  function updateinterface() {
    $(document).ready(function () {
      var dbfield = "", dbval = "";
      var userid = '<%=Session["Login"] %>';
      var dirtylog = $('#txtdirtylog').val();

      var xml = "<?xml version='1.0' encoding='UTF-8'?><interfaces><interface>";
      xml = xml + collectform("InfMaster");
      xml = xml + "</interface></interfaces>";
      if (!allowsubmit())
        return;
      $.ajax({
        type: "GET",
        url: "../InternalServices/ServiceInterface.svc/UpdateInterface",
        //url: "../InternalServices/ServicePM.svc/DoWork",
        data: { "xml": xml },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function (xhr, settings) {
          xhr.setRequestHeader("userid", userid);
        },

        success: function (data, status) {
          FormSubmitted = false;
          var result = data.d;
          var d_array = result.split("^");
          if (d_array[0] == "TRUE") {
            var newdirtylog;
            newdirtylog = parseInt(dirtylog) + 1;
            $('#txtdirtylog').val(newdirtylog);
            savesucceeded(d_array[1]);
          }
          else if (d_array[0] == "FALSE") {
            alert(d_array[1]);
          }
        },
        error: OnGetMemberError
      });

    });

  }

  function createinterface() {
    $(document).ready(function () {
      var dbfield = "", dbval = "";
      var userid = '<%=Session["Login"] %>';
      var dirtylog = $('#txtdirtylog').val();

      var xml = "<?xml version='1.0' encoding='UTF-8'?><interfaces><interface>";
      xml = xml + collectform("InfMaster");
      xml = xml + "</interface></interfaces>";
      if (!allowsubmit())
        return;
      $.ajax({
        type: "GET",
        url: "../InternalServices/ServiceInterface.svc/CreateInterface",
        data: { "xml": xml },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function (xhr, settings) {
          xhr.setRequestHeader("userid", userid);
        },

        success: function (data, status) {
          FormSubmitted = false;
          var result = data.d;
          var d_array = result.split("^");
          if (d_array[0] == "TRUE") {
            var newdirtylog;
            newdirtylog = parseInt(dirtylog) + 1;
            $('#txtdirtylog').val(newdirtylog);
            savesucceeded(d_array[1]);
          }
          else if (d_array[0] == "FALSE") {
            alert(d_array[1]);
          }
        },
        error: OnGetMemberError
      });

    });
  }

    function deleteinterface() {
        var userid = '<%=Session["Login"] %>';
    if (!allowsubmit())
        return;
    $(document).ready(function () {
      $.ajax({
        type: "GET",
        url: "../InternalServices/ServiceInterface.svc/DeleteInterface",
        data: { "userid": userid, "interfacename": "<%=m_interfacename %>" },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, status) {
          FormSubmitted = false;
          
          if (data.d != "OK") {
              alert(data.d);
          }
          else {
              alert("Successed");
              setTimeout(parent.controlpanel.RefreshPanelGridsOnDelete(), 0);
              window.location.href = 'infmain.aspx?mode=query';
          }

        },
        error: OnGetMemberError
      });

    });

  }

    function importinterface()
    {
        <%-- var interfacename = "<%= m_interfacename%>";
        var xmlstring = "<%= m_xmlstr%>";
        if (!allowsubmit())
            return;
        $.ajax({
            type: "GET",
            url: "../InternalServices/ServiceInterface.svc/InterfaceImport",
            data: { "xmlstring": xmlstring, "interfacename": interfacename  },
            contentType: "application/json; charset=utf-8",
            dataType: "json",

            success: function (data, status) {
                FormSubmitted = false;
            },
            error: OnGetMemberError
        });--%>

        $.ajax({
            type: "GET",
            url: "../AzzierServices/ServiceInterfaceExt.svc/sertest",
            data: {},
            contentType: "application/json; charset=utf-8",
            dataType: "json",

            success: function (data, status) {
                alert("finished");
            },
            error: function () {
                alert("error");
            }
        });

 
    }

    function duplicateinterface()
    {
        $(document).ready(function () {
            var dbfield = "", dbval = "";
            var userid = '<%=Session["Login"] %>';
            var dirtylog = $('#txtdirtylog').val();

            var xml = "<?xml version='1.0' encoding='UTF-8'?><interfaces><interface>";
            xml = xml + collectform("InfMaster");
            xml = xml + "</interface><oldinterfacename>"+ "<%=m_oldinterfacename%>" +"</oldinterfacename></interfaces>";
            if (!allowsubmit())
                return;
            $.ajax({
                type: "GET",
                url: "../InternalServices/ServiceInterface.svc/DuplicateInterface",
                data: { "xml": xml },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function (xhr, settings) {
                    xhr.setRequestHeader("userid", userid);
                },

                success: function (data, status) {
                    FormSubmitted = false;
                    var result = data.d;
                    var d_array = result.split("^");
                    if (d_array[0] == "TRUE") {
                        var newdirtylog;
                        newdirtylog = parseInt(dirtylog) + 1;
                        $('#txtdirtylog').val(newdirtylog);
                        savesucceeded(d_array[1]);
                    }
                    else if (d_array[0] == "FALSE") {
                        alert(d_array[1]);
                    }
                },
                error: OnGetMemberError
            });

        });
    }

  function savesucceeded(interfacename) {
    if (interfacename == "")
      window.location.href = 'infmain.aspx';
    else
      window.location.href = 'infmain.aspx?mode=edit&interfacename=' + interfacename;
  }

  function OnGetMemberError(request, status, error) {
    FormSubmitted = false;
    alert(status + "--" + error + "--" + request.responseText);
  }

</script>
