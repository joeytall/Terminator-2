<%@ Page Language="C#" AutoEventWireup="true" CodeFile="pdmwolist.aspx.cs" Inherits="equipment_pdmwolist" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link type="text/css" href="../styles/azzier.css" rel="stylesheet" />
    <link type="text/css" href="~/Styles/Customer.css" rel="stylesheet" />
    <link type="text/css" href="../css/custom-theme/jquery-ui-1.8.21.custom.css" rel="stylesheet" />
</head>
<body class = "popupbody">
    <script type="text/javascript" src="../js/jquery-1.7.2.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.21.custom.min.js"></script>
    <script type="text/javascript" src="../Jscript/CodeValidate.js"></script>
    <script type="text/javascript" src="../Jscript/RadWindow.js"></script>
    <script type="text/javascript" src="../Jscript/Codes.js"></script>
    <form id="MainForm" runat="server">
        <asp:Panel ID="MainControlsPanel" runat="server" CssClass="popupwinpanel">
        <telerik:RadScriptManager ID="RadScriptManager" runat="server">
        </telerik:RadScriptManager>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" >
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdwolist" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>

        <asp:LinkButton ID="btnclose" runat="server" CausesValidation="False" OnClientClick="radwindowClose();return false;" >
    <p align="center" >
       <asp:Image ID="Image4" runat="server" ImageUrl="../images/can.gif" Height="24px" Width="24px" /><br />
       <label>Close</label>
       </p>
    </asp:LinkButton>
    </asp:Panel>
    </form>
</body>

<script type="text/javascript">
 $(document).ready(function () {
        radwinsetsize(<%=screen.Width + 180 %>, <%= screen.Height + 180 %>);
        maincontrolsetsize($('#MainControlsPanel'), <%= screen.Height +50  %>, <%= screen.Width + 50  %>);
    })
</script>
</html>
