<%@ Control Language="C#" AutoEventWireup="true" CodeFile="srheader.ascx.cs" Inherits="StoreRoom_srheader" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<link type="text/css" href="../styles/mainmenu.css" rel="stylesheet" />
<asp:Literal ID="litFrameScript" runat="server" />

<script type="text/javascript" src="../Jscript/Header.js"></script>
<script type="text/javascript" src="../Jscript/Codes.js"></script>
<script type="text/javascript" src="../Jscript/Validation.js"></script>
<script type="text/javascript" src="../Jscript/StopDoubleClick.js"></script>
<script type = "text/javascript">

    function toolbarclicked(sender, e) {
        var button = e.get_item();
        var commandname = button.get_commandName();
        var txtstoreroom = document.getElementById("txtstoreroom");
        switch (commandname) {
            case "search":
                if (WarnUser())
                    settoQueryMode("srmain.aspx?mode=query");
                break;
            case "new":
                if (WarnUser())
                    window.location.href = 'srmain.aspx?mode=new';
                break;
            case "save":
                if (Page_ClientValidate('')) {
                    saveStoreRoom();
                }

                break;
            case "lookup":
                LookupClicked();
                break;
            case "savequery":
                SaveQuery();
                break;
            case "delete":
                deleteStoreRoom();
                break;
            case "multipleissue":
                opennewmodalwindow("../StoreRoom/multipleissue.aspx?storeroom=" + txtstoreroom.value, "MultipleIssue");
                break;
            case "linkeddoc":
                LinkDoc('storeroom', '<%=m_storeroom %>');
                break;
            case "email":
                sendEmail("storeroom", "<%=m_storeroom%>");
                break;
        }
    }

    function managelink() {
        return false;
    }

    function opennewmodalwindow(url, name) {
        /* This function is used to open a new modal window for admin panel*/
        var oWnd;
        if (GetRadWindow() == null) {
            oWnd = radopen(url, null);
            oWnd.set_modal(true);
        }
        else {
            /*
            var radwin = GetRadWindow();
            var oManager = GetRadWindow().get_windowManager();
            oWnd = oManager.open(url, null);
            window.setTimeout(function () { oWnd.setActive(true); oWnd.set_modal(true); });
            */
            //window.setTimeout(function () {oWnd.set_modal(true); });
            var radwin = GetRadWindow();
            var oManager = GetRadWindow().get_windowManager();
            oWnd = oManager.open(url, null);
            oWnd.set_modal(true);

        }
    }
</script>

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
    <br/><br/>
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
    </div>
</div>
<script type= "text/javascript">
    function saveStoreRoom() {
        
          var dbfield = "", dbval = "";
            var userid = '<%=Session["Login"] %>';
            var statuscode = $('input:hidden[name=hidTargetStatusCode]').val();
            var hidprocnum = $('input:hidden[name=hidprocnum]').val();
            var dirtylog = $('#txtdirtylog').val();
            var woxml = "<?xml version='1.0' encoding='UTF-8'?><workorders><workorder>";
            woxml = woxml + collectform("StoreRoom");
           
            $('input:radio[name=rblchargeback]:checked').each(function () {
                dbval = $(this).val();
                woxml = woxml + "<chargeback>" + $(this).val() + "</chargeback>";
            });

            if (dirtylog != '') {
                woxml = woxml + "<dirtylog>" + dirtylog + "</dirtylog>";
            }

            woxml = woxml + "</workorder></workorders>";
            if (!allowsubmit()) return;
            $.ajax({
                type: "GET",
                url: "../InternalServices/ServiceInventory.svc/SaveStoreRoom",
                data: { "paramXML": woxml },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnSaveSuccess,
                error: OnGetMemberError
            });

        
    }

    

    function deleteStoreRoom() {
      var storeroom = getvalue("txtstoreroom");

        if (!confirm("Are you sure to delete the storeroom record?")) return;
        if (!allowsubmit()) return;
        $.ajax({
          type: "GET",
          url: "../InternalServices/ServiceInventory.svc/DeleteStoreRoom",
          data: { "storeroom": storeroom,"check":"1" },
          dataType: "json",
          contentType: "application/json; charset=utf-8",
          success: function (data, status) {

            var result = data.d;
            var values = result.split("::");
            if (values[0] == "0") {
              OnDeleteSuccess();
            }
            else {
              FormSubmitted = false;
              if (values[0] == "-1")
                alert(values[1]);
              else {
                var URL = "inventoryusage.aspx?storeroom=" + storeroom;
                var dWnd;
                dWnd = radopen(URL, null);
                dWnd.set_modal(true);
              }
              
            }
          },
          error: OnGetMemberError
        });
    }

    function OnGetMemberError(request, status, error) {
        FormSubmitted = false;
        alert(status + "--" + error + "--" + request.responseText);
    }

    function OnSaveSuccess() {
        var txtstoreroom = document.getElementById("txtstoreroom");
        window.location.href = 'srmain.aspx?mode=edit&storeroom=' + txtstoreroom.value;
        FormSubmitted = false;
    }

    function OnDeleteSuccess() {
        setTimeout(parent.controlpanel.RefreshPanelGridsOnDelete(), 0);
        window.location.href = 'srmain.aspx?mode=query';
        FormSubmitted = false;
    }
</script>