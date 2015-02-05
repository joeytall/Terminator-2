<%@ Page Language="C#" AutoEventWireup="true" CodeFile="pdmhistory.aspx.cs" Inherits="pdm_pdmhistory" %>
<%@ Register TagPrefix="uc1" TagName="PDMHeader" Src="pdmheader.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<title></title>
<link type="text/css" href="~/Styles/Azzier.css" rel="stylesheet" />
<link type="text/css" href="~/Styles/Customer.css" rel="stylesheet" />
<link type="text/css" href="~/Styles/Header.css" rel="stylesheet" />
<link type="text/css" href="../css/custom-theme/jquery-ui-1.8.21.custom.css" rel="stylesheet" />

<style type="text/css">
    .rgAltRow, .rgRow
    {
        cursor: pointer !important;
    }
</style>

</head>

<body style="background-image:url(../IMAGES/back.png); background-repeat:repeat-x; margin:0px; padding:0px;">
<asp:Literal ID="litMessage" runat="server" />
<form id="MainForm" runat="server">
<uc1:PDMHeader id="ucHeader1" runat="server" />

<asp:Panel ID="MainControlsPanel" runat="server" CssClass="MainControlsPanelClass">
  <telerik:RadScriptManager ID="RadScriptManager1" runat="server"></telerik:RadScriptManager>
  <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false">
    </telerik:RadWindowManager>
  <telerik:RadAjaxManager ID="ajaxmanager" runat="server">
  </telerik:RadAjaxManager>

</asp:Panel>
<asp:Literal ID="litFrameScript" runat="server" />
<asp:HiddenField id="hidMode" value="edit" runat="server" />

</form>
</body>
<script type="text/javascript" src="../js/jquery-1.7.2.min.js"></script>
<script type="text/javascript" src="../js/jquery-ui-1.8.21.custom.min.js"></script>
<script type="text/javascript" src="../jscript/codes.js"></script>
<script type="text/javascript" src="../Jscript/CodeValidate.js"></script>
<script type="text/javascript" src="../Jscript/radwindow.js"></script>
<script type="text/javascript">
  $(window).load(function () {
      setpanelheight(1);
      initToolbar();
  });

</script>
</html>

