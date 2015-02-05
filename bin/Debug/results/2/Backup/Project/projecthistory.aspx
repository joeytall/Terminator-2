<%@ Page Language="C#" AutoEventWireup="true" CodeFile="projecthistory.aspx.cs" Inherits="Project_projecthistory" %>

<%@ Register TagPrefix="uc1" TagName="ProjectHeader" Src="projectheader.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta http-equiv="content-type" content="text/html; charset=utf-8" />
    <link type="text/css" href="../Styles/Header.css" rel="stylesheet" />
    <link type="text/css" href="../Styles/Azzier.css" rel="stylesheet" />
    <link type="text/css" href="~/Styles/Customer.css" rel="stylesheet" />
    <link type="text/css" href="../css/custom-theme/jquery-ui-1.8.21.custom.css" rel="stylesheet" />
</head>
<body style="background-image: url(../IMAGES/back.png); background-repeat: repeat-x;
    margin: 0px; padding: 0px;" onload = "pageinit()">
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
        <script type="text/javascript" src="../Jscript/RecalculatePosition.js"></script>
        <script type="text/javascript" src="../Jscript/RadWindow.js"></script>
        <script type="text/javascript" src="../Jscript/Stopdoubleclick.js"></script>
        <script type="text/javascript">


        </script>
        <uc1:ProjectHeader ID="ucHeader1" runat="server" />
        <asp:Panel ID="MainControlsPanel" runat="server" CssClass="MainControlsPanelClass">
            <telerik:RadScriptManager ID="RadScriptManager" runat="server">
            </telerik:RadScriptManager>
            <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false">
            </telerik:RadWindowManager>
        </asp:Panel>    
        <asp:HiddenField ID="hidMode" runat="server" />
    </form>
    

</body>
</html>

<script type="text/javascript">
    $(window).load(function () {
        setpanelheight(1);
        initToolbar();
    });

    function pageinit() {
        $(document).ready(function () {
            try {
                setTimeout(parent.controlpanel.RefreshPanelGrids(), 0);
                setTimeout(parent.controlpanel._updatecounts(), 200);
            }
            catch (err) {
                alert(err);
            }
        });
    }
</script>
