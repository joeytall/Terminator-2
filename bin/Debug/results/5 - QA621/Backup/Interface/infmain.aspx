<%@ Page Language="C#" AutoEventWireup="true" CodeFile="infmain.aspx.cs" Inherits="interface_infmain"
    MaintainScrollPositionOnPostback="true" %>

<%@ Register TagPrefix="uc1" TagName="InfHeader" Src="infheader.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <meta http-equiv="content-type" content="text/html; charset=utf-8" />
    <link type="text/css" href="../Styles/Header.css" rel="stylesheet" />
    <link type="text/css" href="../Styles/Azzier.css" rel="stylesheet" />
    <link type="text/css" href="~/Styles/Customer.css" rel="stylesheet" />
    <link type="text/css" href="../css/custom-theme/jquery-ui-1.8.21.custom.css" rel="stylesheet" />
</head>
<body style="background-image: url(../IMAGES/back.png); background-repeat: repeat-x;
    margin: 0px; padding: 0px;" onkeydown="keyPressed(event.keyCode)">
    <asp:Literal ID="litMessage" runat="server" />
    <form id="MainForm" runat="server">
    <script type="text/javascript" src="../js/jquery-1.7.2.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.21.custom.min.js"></script>
    <script type="text/javascript" src="../Jscript/AutoComplete.js"></script>
    <script type="text/javascript" src="../Jscript/CodeValidate.js"></script>
    <script type="text/javascript" src="../Jscript/SetValue.js"></script>
    <script type="text/javascript" src="../Jscript/Codes.js"></script>
    <script type="text/javascript" src="../Jscript/LabourCalculations.js"></script>
    <script type="text/javascript" src="../Jscript/MaterialCalculations.js"></script>
    <script type="text/javascript" src="../Jscript/ServiceCalculations.js"></script>
    <script type="text/javascript" src="../Jscript/ToolsCalculations.js"></script>
    <script type="text/javascript" src="../Jscript/CallBackFunction.js"></script>
    <script type="text/javascript" src="../Jscript/RadWindow.js"></script>
    <script type="text/javascript" src="../Jscript/Stopdoubleclick.js"></script>
    <script type="text/javascript" src="../Jscript/RadControls.js"></script>
    <script type="text/javascript">

        function duplicate() {
            $(document).ready(function () {
                var infname = $('#txtinterfacename').val();
                window.location.href = 'infmain.aspx?mode=duplicate&interfacename=' + infname;
            });
        }

        function clearoldempval() {
            $(document).ready(function () {
            });
        }

    </script>
    <uc1:InfHeader ID="ucHeader1" runat="server" />
    <asp:Panel ID="MainControlsPanel" runat="server" CssClass="MainControlsPanelClass">
        <telerik:RadScriptManager ID="RadScriptManager" runat="server">
        </telerik:RadScriptManager>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false">
        </telerik:RadWindowManager>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            <AjaxSettings>
            </AjaxSettings>
            <ClientEvents OnRequestStart="RequestStart" OnResponseEnd="ResponseEnd" />
        </telerik:RadAjaxManager>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
        <asp:SqlDataSource ID="InfTablesSqlDataSource" runat="server" ProviderName="System.Data.OleDb"></asp:SqlDataSource>
    </asp:Panel>
    <asp:SqlDataSource ID="UserModuleListSqlDataSource" runat="server" ProviderName="System.Data.OleDb">
        </asp:SqlDataSource>
    <asp:SqlDataSource ID="InterfaceTypeListSqlDataSource" runat="server" ProviderName="System.Data.OleDb">
        </asp:SqlDataSource>
    <asp:HiddenField ID="hidMode" runat="server" />
    <asp:HiddenField ID="HidOldInterface" runat="server" />    
    <input type="hidden" id="hidDataChanged" value="0" />
    <input type="hidden" id="hidNoPromt" value="0" />
  <asp:HiddenField ID="hidKeyName" value="txtinterfacename" runat="server" />
    <iframe id="ifValidate" src="" frameborder="0" scrolling="no" style="visibility: hidden;
        display: none;"></iframe>
    </form>
</body>
<script type="text/javascript" language="javascript">
    <asp:Literal ID="litScript1" runat="server"></asp:Literal>
</script>
<script type="text/javascript">
    $(window).load(function () {
        setpanelheight(1);
        initToolbar();
    });

    function keyPressed(code) {
        if (code == 13 && "<%=querymode %>" == "query") {
            parent.window.frames[1].LookupGrid("Infmaster", "grdresults");
        }
   }

    function editMap(counter) {
      var dWnd;
      dWnd = radopen('infmap.aspx?counter=' + counter, null);
      dWnd.set_modal(true);
      return false;
    }
</script>
</html>
