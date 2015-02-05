<%@ Page Language="C#" AutoEventWireup="true" CodeFile="poinvoices.aspx.cs" Inherits="purchase_poinvoices" %>

<%@ Register TagPrefix="uc1" TagName="Header" Src="poheader.ascx" %>
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
    <uc1:Header ID="ucHeader1" runat="server" />
    <asp:Literal ID="litMessage" runat="server" />
    <asp:Panel ID="MainControlsPanel" runat="server" CssClass="MainControlsPanelClass">
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        </telerik:RadScriptManager>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false">
        </telerik:RadWindowManager>
        <telerik:RadAjaxManager ID="ajaxmanager" runat="server">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="grdinvoiceheader">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdinvoiceheader" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
            <ClientEvents OnRequestStart="RequestStart" OnResponseEnd="ResponseEnd" />
        </telerik:RadAjaxManager>
        <asp:SqlDataSource ID="PurchaseInvoices_SqlDataSource" runat="server" ProviderName="System.Data.OleDb">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="InvoiceDetail_SqlDataSource" runat="server" ProviderName="System.Data.OleDb">
        <SelectParameters>
          <asp:SessionParameter Name="InvoiceNo" SessionField="InvoiceNo" Type="string" />
        </SelectParameters>
        </asp:SqlDataSource>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </asp:Panel>
    <asp:Literal ID="litFrameScript" runat="server" />
    <asp:HiddenField ID="hidMode" Value="edit" runat="server" />
    <iframe id="ifValidate" src="" frameborder="0" scrolling="no" style="visibility: hidden;
        display: none;"></iframe>
    </form>
</body>
<style>
        .InnerHeaderStyle
    {
      background:  #98f5ff !important;
      font-size: 13px !important;
      color: white !important; /*add more style definitions here*/
    }
    .InnerItemStyle
    {
      background: white !important;
      color: brown !important; /*add more style definitions here*/
    }
    </style>
<script type="text/javascript" src="../js/jquery-ui-1.8.21.custom.min.js"></script>
<script type="text/javascript" src="../jscript/codes.js"></script>
<script type="text/javascript" src="../jscript/codevalidate.js"></script>
<script type="text/javascript" src="../jscript/setvalue.js"></script>
<script type="text/javascript" src="../jscript/callbackfunction.js"></script>
<script type="text/javascript" src="../jscript/radwindow.js"></script>
<script type="text/javascript" src="../jscript/autocomplete.js"></script>
<script type="text/javascript" src="../jscript/stopdoubleclick.js"></script>
<script type="text/javascript" src="../jscript/RadControls.js"></script>
<script type="text/javascript">
    $(window).load(function () {
        setpanelheight(1);
        initToolbar();
    });

    function EditVendorContacts(counter, tfield) {
        var URL = 'vcontacts.aspx?counter=' + counter+"&referer="+tfield;
        if (GetRadWindow() == null) {
            oWnd = radopen(URL, null);
            window.setTimeout(function () { oWnd.setActive(true); oWnd.set_modal(true); });
        }
        else {
            var radwin = GetRadWindow();
            var oManager = GetRadWindow().get_windowManager();
            oWnd = oManager.open(URL, null);
            window.setTimeout(function () { oWnd.setActive(true); oWnd.set_modal(true); });
        } return false;
    }

    function editInvoice(counter, POnum) {
        var url = "invoicemain.aspx?invoiceCounter=" + counter + "&ponum=" + POnum;
        OpenRadWindow(url);
        return false;
    }

    function refreshGrid() {
        var masterTable = $find('grdinvoiceheader').get_masterTableView();
            masterTable.rebind();
    }

    function addNewInvoice(POnum) {
      OpenRadWindow('invoicemain.aspx?ponum='+POnum);
    }

</script>
</html>
