<%@ Control Language="C#" AutoEventWireup="true" CodeFile="tcheader.ascx.cs" Inherits="Timecards_tcheader" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<link type="text/css" href="../styles/mainmenu.css" rel="stylesheet" />
<asp:Literal ID="litFrameScript" runat="server" />
<asp:Literal ID="litDateLCIDScript" runat="server" />

<script type="text/javascript" src="../Jscript/Header.js"></script>
<script type="text/javascript" src="../Jscript/Codes.js"></script>
<script type="text/javascript" src="../Jscript/Validation.js"></script>
<script type="text/javascript" src="../Jscript/StopDoubleClick.js"></script>
<script type="text/javascript">
    function toolbarclicked(sender, e) {
        var button = e.get_item();
        var commandname = button.get_commandName();
        switch (commandname) {
            case "search":
                if (WarnUser())
                    settoQueryMode("../timecards/tcmain.aspx?mode=query");
                break;
            case "new":
                if (WarnUser())
                window.location.href = 'tcmain.aspx?mode=new';
                break;
            case "save":
                var txtwonum = document.getElementById('txtwonum');
                var txtdraccount = document.getElementById('txtdraccount');
                var wonum = "";
                var draccount = "";
                var mode = $('#hidMode', parent.mainmodule.document).val();
                if (txtwonum != null) {
                    wonum = txtwonum.value;
                }
                if (txtdraccount != null) {
                    draccount = txtdraccount.value;
                }
                noPrompt();
                if (Page_ClientValidate('')) {
                    if ((wonum == "") && (draccount == "")) {
                        alert("Work Order or Draccount is required.");
                        return;
                    }
                    else {
                        savetimecard();
                    }
                }
                break;
            case "lookup":
                LookupClicked();
                break;
            case "savequery":
                SaveQuery();
                break;
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

<script type="text/javascript">
    function savetimecard() {
        var mode = "<%=m_mode %>";
        var xml = "<?xml version='1.0' encoding='UTF-8'?><TimeCard><WOLabour>";
        xml = xml + collectformall("v_WOLabourDetail");
        xml = xml + "</WOLabour></TimeCard>";
        if (!allowsubmit()) return;

        $.ajax({
            type: "GET",
            url: "../InternalServices/ServiceLabour.svc/SaveTimeCard",
            data: { "paramXML": xml},
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr, settings) {
                xhr.setRequestHeader("userid","<%=Session["Login"] %>");
                xhr.setRequestHeader("mode", mode);
            },
            success: OnSaveTimeCardSucess,
            error: OnGetMemberError
        });
    }
    function OnSaveTimeCardSucess (data, status) {
        var mode = "<%=m_mode %>";
        FormSubmitted=false;
        var result = data.d;
        if (result != "OK")
            { alert(result);}
            else {
            //alert("Save sucess!");
            switch (mode)
            {
                case "new":
                    var empid = document.getElementById("txtempid").value
                    var transdate = document.getElementById("txttransdate").value
                    window.location.href = "tcmain.aspx?mode=new&empid="+empid+"&transdate="+transdate;
                    break;
                case "edit":
                    var counter = document.getElementById("txtcounter").value;
                    window.location.href = "tcmain.aspx?mode=edit&counter="+counter;
                    break;
            }
        }
    }
    function OnGetMemberError(request, status, error) {
      FormSubmitted=false;
      alert(status + "--" + error + "--" + request.responseText);
    }
</script>