<%@ Page Language="C#" AutoEventWireup="true" CodeFile="srmain.aspx.cs" Inherits="StoreRoom_srmain" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc1" TagName="SRHeader" Src="srheader.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta http-equiv="content-type" content="text/html; charset=utf-8" />
    <link type="text/css" href="../Styles/Header.css" rel="stylesheet" />
    <link type="text/css" href="../Styles/Azzier.css" rel="stylesheet" />
    <link type="text/css" href="~/Styles/Customer.css" rel="stylesheet" />
    <link type="text/css" href="../css/custom-theme/jquery-ui-1.8.21.custom.css" rel="stylesheet" />
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
    .InnerAlernatingItemStyle
    {
      background: white !important;
      color: brown !important; /*add more style definitions here*/
    }
    .MostInnerHeaderStyle
    {
      background: #FFEBCD !important;
      /*font-size: 15px !important;*/
      color: white !important; /*add more style definitions here*/
    }
    .MostInnerItemStyle
    {
      background: white !important;
      color: olive !important; /*add more style definitions here*/
    }
    .MostInnerAlernatingItemStyle
    {
      background: white !important;
      color: olive !important; /*add more style definitions here*/
    }
    </style>
</head>

<script type="text/javascript" src="../js/jquery-1.7.2.min.js"></script>
<script type="text/javascript" src="../js/jquery-ui-1.8.21.custom.min.js"></script>
<script type="text/javascript" src="../Jscript/AutoComplete.js"></script>
<script type="text/javascript" src="../Jscript/CodeValidate.js"></script>
<script type="text/javascript" src="../Jscript/SetValue.js"></script>
<script type="text/javascript" src="../Jscript/Codes.js"></script>
<script type="text/javascript" src="../Jscript/MaterialCalculations.js"></script>
<script type="text/javascript" src="../Jscript/CallBackFunction.js"></script>
<script type="text/javascript" src="../Jscript/RadWindow.js"></script>
<script type="text/javascript" src="../Jscript/Stopdoubleclick.js"></script>
<script type="text/javascript" src="../Jscript/RadControls.js"></script>


<script type="text/javascript">
    $(window).load(function () {
        setpanelheight(1);
        initToolbar();
    });

</script>

<body style="background-image: url(../IMAGES/back.png); background-repeat: repeat-x; margin: 0px; padding: 0px;" onkeydown="keyPressed(event.keyCode)" >
    <asp:Literal ID="litMessage" runat="server" />
    <asp:SqlDataSource ID="StoreRoomItemListSqlDataSource" runat="server" ProviderName="System.Data.OleDb">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="StoreRoomPositionListSqlDataSource" runat="server" ProviderName="System.Data.OleDb">
    <SelectParameters>
        <asp:SessionParameter Name="ItemNum" SessionField="ItemNum" Type="string" />
        <asp:SessionParameter Name="StoreRoom" SessionField="Storeroom" Type="string" />
    </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="StoreRoomLotListSqlDataSource" runat="server" ProviderName="System.Data.OleDb">
    <SelectParameters>
        <asp:SessionParameter Name="ItemNum" SessionField="ItemNum" Type="string" />
        <asp:SessionParameter Name="Storeroom" SessionField="Storeroom" Type="string" />
        <asp:SessionParameter Name="Position" SessionField="Position" Type="string" />
        </SelectParameters>
    </asp:SqlDataSource>
    <form id="mainform" runat="server">
        <uc1:SRHeader ID="ucHeader1" runat="server" />
        <telerik:RadScriptManager ID="RadScriptManager" runat="server">
        </telerik:RadScriptManager>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false">
        </telerik:RadWindowManager>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            <AjaxSettings>
            
            <telerik:AjaxSetting AjaxControlID="grditemlist">
                    <UpdatedControls>
                    
                        <telerik:AjaxUpdatedControl ControlID="grditemlist" />
                    
                    </UpdatedControls>
                </telerik:AjaxSetting>
            
            </AjaxSettings>
            <ClientEvents OnRequestStart="RequestStart" OnResponseEnd="ResponseEnd" />
        </telerik:RadAjaxManager>
        <asp:Panel ID="MainControlsPanel" runat="server" CssClass="MainControlsPanelClass">
        </asp:Panel>
        <asp:HiddenField ID="hidMode" runat="server" />
        <asp:HiddenField ID="hidKeyName" Value="txtstoreroom" runat="server" />
        <input type="hidden" id="hidempid" value="<%=Session["Login"].ToString() %>" />
    </form>
    <asp:Literal ID="litScript1" runat="server"></asp:Literal>
</body>
</html>


<script type="text/javascript">
    function issue(itemnum, storeroom) {
        var URL = "../inventory/invissue.aspx?itemnum=" + itemnum + "&storeroom=" + storeroom;
        var dWnd;
        dWnd = radopen(URL, null);
        dWnd.set_modal(true);
    }

    function editstoremain(counter, itemnum) {
        var URL;
        if (counter == "") {
            URL = "../inventory/storemain.aspx?itemnum=" + itemnum;
        }
        else {
            URL = "../inventory/storemain.aspx?counter=" + counter;
        }
        var dWnd;
        dWnd = radopen(URL, null);
        dWnd.set_modal(true);
        return false;
    }

    function refreshGrid() {
        var masterTable = $find("grditemlist").get_masterTableView();
        masterTable.rebind();
    }


    function addnewitem() {
        var URL = "../inventory/storemain.aspx?storeroom=<%=m_storeroom %>";
        var oWnd;

        if (GetRadWindow() == null) {
            oWnd = radopen(URL, null);
            window.setTimeout(function () { oWnd.setActive(true); oWnd.set_modal(true); });
        }
        else {
            var radwin = GetRadWindow();
            var oManager = GetRadWindow().get_windowManager();
            oWnd = oManager.open(URL, null);
            window.setTimeout(function () { oWnd.setActive(true); oWnd.set_modal(true); });
        } 
        return false;
    }
    function keyPressed(code) {
        if (code == 13 && "<%=querymode %>" == "query") {
            LookupClicked();
        }
    }
</script>