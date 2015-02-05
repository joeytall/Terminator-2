<%@ Page Language="C#" AutoEventWireup="true" CodeFile="equipmentspecs.aspx.cs" Inherits="Equipment_Equipmentspecs" %>

<%@ Register TagPrefix="uc1" TagName="EqpHeader" Src="equipmentheader.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link type="text/css" href="~/Styles/Azzier.css" rel="stylesheet" />
    <link type="text/css" href="~/Styles/Customer.css" rel="stylesheet" />
    <link type="text/css" href="~/Styles/Header.css" rel="stylesheet" />
    <link type="text/css" href="../css/custom-theme/jquery-ui-1.8.21.custom.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.7.2.min.js"></script>
</head>
<body style="background-image: url(../IMAGES/back.png); background-repeat: repeat-x;
    margin: 0px; padding: 0px;">
    <form id="MainForm" runat="server">
    <uc1:EqpHeader ID="ucHeader1" runat="server" />
    <asp:Literal ID="litMessage" runat="server" />
    <asp:Panel ID="MainControlsPanel" runat="server" Style="position: absolute; left: 10px;
        top: 130px; border: thin solid #003300; background-color: transparent; border-radius: 1em;">
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        </telerik:RadScriptManager>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false">
        </telerik:RadWindowManager>
        <telerik:RadAjaxManager ID="ajaxmanager" runat="server">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="grdequipmentspecs">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdequipmentspecs" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
            <ClientEvents OnRequestStart="RequestStart" OnResponseEnd="ResponseEnd" />
        </telerik:RadAjaxManager>
        <asp:SqlDataSource ID="EquipmentSpecs_SqlDataSource" runat="server" ProviderName="System.Data.OleDb">
        </asp:SqlDataSource>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ValidationGroup="Equipmentspecs"
            ShowSummary="False" />
    </asp:Panel>
    <asp:Literal ID="litFrameScript" runat="server" />
    <asp:HiddenField ID="hidMode" Value="edit" runat="server" />
    <iframe id="ifValidate" src="" frameborder="0" scrolling="no" style="visibility: hidden;
        display: none;"></iframe>
    </form>
</body>
<script type="text/javascript" src="../js/jquery-1.7.2.min.js"></script>
<script type="text/javascript" src="../js/jquery-ui-1.8.21.custom.min.js"></script>
<script type="text/javascript" src="../jscript/codes.js"></script>
<script type="text/javascript" src="../jscript/codevalidate.js"></script>
<script type="text/javascript" src="../jscript/setvalue.js"></script>
<script type="text/javascript" src="../jscript/callbackfunction.js"></script>
<script type="text/javascript" src="../jscript/radwindow.js"></script>
<script type="text/javascript" src="../jscript/autocomplete.js"></script>
<script type="text/javascript" src="../jscript/stopdoubleclick.js"></script>
<script type="text/javascript">

  $(window).load(function () {
      setpanelheight(1);
      initToolbar();
  });

  function refreshGrid() {
    var grid = $find("<%=grdEquipmentspecs.ClientID%>");
    //if (grid!=null)
    grid.get_masterTableView().rebind();
    
  }
</script>
</html>
