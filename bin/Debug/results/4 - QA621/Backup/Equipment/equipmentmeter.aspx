<%@ Page Language="C#" AutoEventWireup="true" CodeFile="equipmentmeter.aspx.cs" Inherits="Equipment_equipmentmeter" %>
<%@ Register TagPrefix="uc1" TagName="EqpHeader" Src="equipmentheader.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Reference Control="EQPChartControl.ascx" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<title></title>
<link type="text/css" href="~/Styles/Azzier.css" rel="stylesheet" />
<link type="text/css" href="~/Styles/Customer.css" rel="stylesheet" />
<link type="text/css" href="~/Styles/Header.css" rel="stylesheet" />
<link type="text/css" href="~/Styles/Gauges.css" rel="stylesheet" />
</head>

<body style="background-image:url(../IMAGES/back.png); background-repeat:repeat-x; margin:0px; padding:0px;" >
    <asp:Literal ID="litMessage" runat="server" />
    <form id="MainForm" runat="server">
        <script type="text/javascript" src="../js/jquery-1.7.2.min.js"></script>
        <script type="text/javascript" src="../js/jquery-ui-1.8.21.custom.min.js"></script>
        <script type="text/javascript" src="../Jscript/AutoComplete.js"></script>
        <script type="text/javascript" src="../Jscript/CodeValidate.js"></script>
        <script type="text/javascript" src="../Jscript/SetValue.js"></script>
        <script type="text/javascript" src="../Jscript/Validation.js"></script>
        <script type="text/javascript" src="../charts/FusionCharts.js"></script>
        <script type="text/javascript" src="../jscript/codes.js"></script>
        <script type="text/javascript" src="../Jscript/CodeValidate.js"></script>
        <script type="text/javascript" src="../Jscript/radwindow.js"></script>
        <script type="text/javascript" src="../Jscript/callbackfunction.js"></script>
        <uc1:EqpHeader id="ucHeader1" runat="server" />
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server"></telerik:RadScriptManager>
        <asp:Panel ID="MainControlsPanel" runat="server" CssClass="MainControlsPanelClass">
            <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false">
            </telerik:RadWindowManager>
            <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
                <AjaxSettings>
                </AjaxSettings>
            </telerik:RadAjaxManager>
        </asp:Panel>
        <asp:HiddenField id="hidMode" value="edit" runat="server" />
        <asp:HiddenField ID="hidIsInitial" Value = "1" runat = "server"/>
    </form>
</body>
</html>

<script type="text/javascript">
    $(window).load(function () {
        setpanelheight(1);
        initToolbar();
    });

    function Grid_OnCommand(sender,args) {
        args.set_cancel(true);
        var currentPageIndex = sender.get_masterTableView().get_currentPageIndex();
        var pageSize = sender.get_masterTableView().get_pageSize();
        var sortExpressions = sender.get_masterTableView().get_sortExpressions();
        var filterExpressions = sender.get_masterTableView().get_filterExpressions();
        var sortExpressionsAsSQL = sortExpressions.toString();
        var filterExpressionsAsSQL = filterExpressions.toDynamicLinq();
            var maximumRows = <%= (grdmeterlist.AllowPaging)? grdmeterlist.PageSize : grdmeterlist.Items.Count %>;

        rebinddatatogrid(currentPageIndex * pageSize,  maximumRows, sortExpressionsAsSQL, filterExpressionsAsSQL);
    }


    function grdmeterlist_OnCommand(sender, args) {
        if (args.get_commandName() == "Page" || args.get_commandName() == "PageSize" || args.get_commandName() == "Filter" || args.get_commandName() == "Sort")
        {
            document.getElementById("hidIsInitial").value = "1";
            rebinddatatogrid(0, 100, '', '', true);
        }
    }

    function rebinddatatogrid(startRowIndex, maximumRows, sortExpression, filterExpression) {
        var grdmeasurementlist = $find("<%=grdmeterlist.ClientID %>");
        var MasterTable = grdmeasurementlist.get_masterTableView();
        var selectedRows = MasterTable.get_selectedItems();
        var equipment = "<%= m_equipment%>";
        var xmlparam = "<?xml version='1.0' encoding='UTF-8'?><roots><Linkid>";

        for (var i = 0; i < selectedRows.length; i++) {
            var row = selectedRows[i];
            var metername = row.getDataKeyValue("MeterName");

            xmlparam = xmlparam + "<IDS>" + metername + "</IDS>";
        }
        if (selectedRows.length<1)
        {
            "<IDS></IDS>"
        }
        xmlparam = xmlparam + "</Linkid>";
        xmlparam = xmlparam + "<Equipment>" + equipment + "</Equipment>";
        xmlparam = xmlparam + "</roots>";
        $.ajax({
            type: "GET",
            url: "../InternalServices/ServiceMeter.svc/GetMeterReadingList",
            data: { "paramXML": xmlparam, "startRowIndex": startRowIndex, "maximumRows": maximumRows, "sortExpression": sortExpression, "filterExpression": filterExpression },
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data, status) {
                FormSubmitted = false;
                OnGetMeterReadingSuccess(data);
            },
            error: OnGetMeterReadingError
        });
    }


    function grdmeterlist_OnSelectionChanged() {
        if (document.getElementById("hidIsInitial").value != "1") {
            rebinddatatogrid(0, 100, '', '');
        }
    }

    function Grdmeterlist_OnDataBound(sender, eventargs) {
        var grid = $find("<%= grdmeterlist.ClientID %>");
        var mastertable = grid.get_masterTableView();
        var grdrows = mastertable.get_dataItems();
        for (var i = 0; i < grdrows.length; i++) {
            grdrows[i].set_selected(true);
        }
        rebinddatatogrid(0, 100, '', '');
        document.getElementById("hidIsInitial").value = "0";

    }

    function OnGetMeterReadingError(request, status, error) {
        FormSubmitted = false;
        alert(status + "--" + error + "--" + request.responseText);
    }
    function OnGetMeterReadingSuccess(result)
    {
        var grdreading = $find("<%= grdreading.ClientID %>");
        var tableview = grdreading.get_masterTableView();
        tableview.set_dataSource(result.d.Data)
        //result.count should hold the count of the items.
        tableview.set_virtualItemCount(result.d.Count);
        tableview.dataBind();
    }

    function editmeter(index)
    {
        var grid = $find("grdmeterlist");
        var counter;
        var metername;
        var URL = "addmeter.aspx?equipment=<%=m_equipment%>";
        if (index != "")
        {
            counter = grid.get_masterTableView().get_dataItems()[index].getDataKeyValue("Counter");
            metername = grid.get_masterTableView().get_dataItems()[index].getDataKeyValue("MeterName");
            URL = URL + "&metername="+metername; 
            URL = URL + "&counter="+counter;
        }
        dWnd = radopen(URL, null);
        dWnd.set_modal(true);
    }

    function editreading(index)
    {

        var grid = $find("grdreading");
        var counter;
        var metername;
        var URL = "meterentry.aspx?equipment=<%=m_equipment%>";
        if (index != "")
        {
            counter = grid.get_masterTableView().get_dataItems()[index].getDataKeyValue("Counter");
            metername = grid.get_masterTableView().get_dataItems()[index].getDataKeyValue("MeterName");
            URL = URL + "&metername="+metername;
            URL = URL + "&counter="+counter;
        }
        dWnd = radopen(URL, null);
        dWnd.set_modal(true);
    }

    function refreshGrid()
    {
        FormSubmitted = false;
        var grdmeasurementlist = $find("grdmeterlist");
        grdmeasurementlist.get_masterTableView().rebind();
    }

    function checkall(chkboxclientid)
    {
        document.getElementById("hidIsInitial").value = "1";
        var checked = document.getElementById(chkboxclientid).checked;
        var grdmeasuremrntlist = $find("<%=grdmeterlist.ClientID %>");
        var MasterTable = grdmeasuremrntlist.get_masterTableView();
        var selectedRows = MasterTable.get_dataItems();
        for (var i = 0; i < selectedRows.length; i++) {
            var row = selectedRows[i];
            row.set_selected(checked);
        }
        document.getElementById("hidIsInitial").value = "0";
        rebinddatatogrid(0, 100, '', '');
    }


    function changeout(index)
    {
        var grid = $find("grdmeterlist");
        var counter;
        var metername;
        var URL = "meterchangeout.aspx?equipment=<%=m_equipment%>";
        if (index != "")
        {
            counter = grid.get_masterTableView().get_dataItems()[index].getDataKeyValue("Counter");
            metername = grid.get_masterTableView().get_dataItems()[index].getDataKeyValue("MeterName");
            URL = URL + "&metername="+metername; 
        }
        dWnd = radopen(URL, null);
        dWnd.set_modal(true);
    }
</script>