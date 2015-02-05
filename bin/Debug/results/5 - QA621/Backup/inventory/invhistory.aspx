<%@ Page Language="C#" AutoEventWireup="true" CodeFile="invhistory.aspx.cs" Inherits="inventory_invhistory"
    MaintainScrollPositionOnPostback="true" %>

<%@ Register TagPrefix="uc1" TagName="InvHeader" Src="invheader.ascx" %>
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
    <style>
    </style>

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
    <script type="text/javascript" src="../Jscript/CallBackFunction.js"></script>
    <script type="text/javascript" src="../Jscript/RadWindow.js"></script>
    <script type="text/javascript" src="../Jscript/Stopdoubleclick.js"></script>
    <script type="text/javascript">

        function duplicate() {
          $(document).ready(function () {
            var itemnum = $('#txtitemnum').val();
            window.location.href = 'invmain.aspx?mode=duplicate&itemnum=' + itemnum;
          });
        }

        function edititemvendor(counter) {

          var URL;
          if (counter == "") {
             URL = "itemvendor.aspx?itemnum=<%=m_itemnum %>&refreshgrid=grditemvendor";
          }
          else {
            URL = "itemvendor.aspx?counter=" + counter + "&refreshgrid=grditemvendor";
          }
          var dWnd;
          dWnd = radopen(URL, null);
          dWnd.set_modal(true);
          return false;
        }

        function refreshgrid(gridid) {
          var grid = $find(gridid);
          if (grid != null) {
            var masterTable = $find(gridid).get_masterTableView();
            masterTable.rebind();
          }
        }

    </script>
    <uc1:InvHeader ID="ucHeader1" runat="server" />
    <asp:Panel ID="MainControlsPanel" runat="server" CssClass="MainControlsPanelClass">
        <telerik:RadScriptManager ID="RadScriptManager" runat="server">
        </telerik:RadScriptManager>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false">
        </telerik:RadWindowManager>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="grdinvmain">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdinvmain" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
            <ClientEvents OnRequestStart="RequestStart" OnResponseEnd="ResponseEnd" />
        </telerik:RadAjaxManager>
        <asp:SqlDataSource ID="ItemHistorySqlDataSource" runat="server" ProviderName="System.Data.OleDb">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="TransactionSqlDataSource" runat="server" ProviderName="System.Data.OleDb">
        </asp:SqlDataSource>
        
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </asp:Panel>
    <asp:HiddenField ID="hidMode" runat="server" />
    <asp:HiddenField ID="HidOldItemNum" runat="server" />    
    <input type="hidden" id="hidDataChanged" value="0" />
    <input type="hidden" id="hidNoPromt" value="0" />
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
            __doPostBack("Query", "");
        }
    }

</script>
</html>
