<%@ Page Language="C#" AutoEventWireup="true" CodeFile="equipmentmeasurement.aspx.cs" Inherits="Equipment_equipmentmeasurement" %>
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
    <script type="text/javascript" src="../jscript/stopdoubleclick.js"></script>
    <form id="MainForm" runat="server">
        

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
        var maximumRows = <%= (grdreading.AllowPaging)? grdreading.PageSize : grdreading.Items.Count %>;

        rebinddatatogrid(currentPageIndex * pageSize,  maximumRows, sortExpressionsAsSQL, filterExpressionsAsSQL);
       
    }

    function grdmeasurementlist_OnCommand(sender, args) {
        if (args.get_commandName() == "Page" || args.get_commandName() == "PageSize" || args.get_commandName() == "Filter" || args.get_commandName() == "Sort") {
            document.getElementById("hidIsInitial").value = "1";
            rebinddatatogrid(0, 100, '', '', true);
        }
    }

    function rebinddatatogrid(startRowIndex, maximumRows, sortExpression, filterExpression) {
        var grdmeasurementlist = $find("<%=grdmeasurementlist.ClientID %>");
        var MasterTable = grdmeasurementlist.get_masterTableView();
        var selectedRows = MasterTable.get_selectedItems();
        var equipment = "<%= m_equipment%>";
        var xmlparam = "<?xml version='1.0' encoding='UTF-8'?><roots><Linkid>";
        for (var i = 0; i < selectedRows.length; i++) {
              var row = selectedRows[i];
              var measurementname = row.getDataKeyValue("MeasurementName");
              
              xmlparam = xmlparam + "<IDS>" + measurementname + "</IDS>";
          }
        if (selectedRows.length<1)
        {
            "<IDS></IDS>"
        }
          xmlparam = xmlparam + "</Linkid>";
          xmlparam = xmlparam +"<Equipment>"+equipment+"</Equipment>";
          xmlparam = xmlparam + "</roots>";
          $.ajax({
              type: "GET",
              url: "../InternalServices/ServiceMeter.svc/GetMeasurementReadingList",
              data: { "paramXML": xmlparam, "startRowIndex":startRowIndex , "maximumRows": maximumRows, "sortExpression": sortExpression, "filterExpression": filterExpression },
              dataType: "json",
              contentType: "application/json; charset=utf-8",
              success: function (data, status) {
                  FormSubmitted = false;
                  OnGetMeasurementReadingSuccess(data);
              },
              error: OnGetMeasurementReadingError
          });
    }

    function OnGetMeasurementReadingError(request, status, error) {
        FormSubmitted = false;
        alert(status + "--" + error + "--" + request.responseText);
    }
    function OnGetMeasurementReadingSuccess(result)
    {
        var grdreading = $find("<%= grdreading.ClientID %>");
        var tableview = grdreading.get_masterTableView();
        
        tableview.set_dataSource(result.d.Data)
        //result.count should hold the count of the items.
        tableview.set_virtualItemCount(result.d.Count);
        tableview.dataBind();
    }

    function grdmeasurementlist_OnSelectionChanged()
    {
        if (document.getElementById("hidIsInitial").value !="1"){
            rebinddatatogrid(0,100,'','');
        }
    }
    
    function GrdmeasurementOnDataBound(sender,eventargs)
    {
        var grid = $find("<%= grdmeasurementlist.ClientID %>");
        var mastertable = grid.get_masterTableView();
        var grdrows = mastertable.get_dataItems();      
        for (var i = 0; i < grdrows.length; i++)
        {
            grdrows[i].set_selected(true);
        }
        rebinddatatogrid(0,100,'','');
        document.getElementById("hidIsInitial").value= "0";      
      
    }

    function editmeasurement(index)
    {     
        var grid = $find("grdmeasurementlist");
        var counter;
        var measurementname;
        var URL = "addmeasurement.aspx?linktype=equipment&linkid=<%=m_equipment%>";
        if (index != "")
        {
            counter = grid.get_masterTableView().get_dataItems()[index].getDataKeyValue("Counter");
            measurementname = grid.get_masterTableView().get_dataItems()[index].getDataKeyValue("MeasurementName");
            URL = URL + "&measurementname="+measurementname; 
            URL = URL + "&counter="+counter;
       }
        dWnd = radopen(URL, null);
        dWnd.set_modal(true);
    }
    function editreading (index)
    {
        var grid = $find("grdreading");
        var counter;
        var measurementname;
        var URL = "measuremententry.aspx?linktype=equipment&linkid=<%=m_equipment%>";
        if (index != "")
        {
            counter = grid.get_masterTableView().get_dataItems()[index].getDataKeyValue("Counter");
            measurementname = grid.get_masterTableView().get_dataItems()[index].getDataKeyValue("MeasurementName");
            URL = URL + "&measurementname="+measurementname;
            URL = URL + "&counter="+counter;
        }
        dWnd = radopen(URL, null);
        dWnd.set_modal(true);
    }

    function refreshGrid()
    {
        FormSubmitted = false;
        var grdmeasurementlist = $find("grdmeasurementlist");
        grdmeasurementlist.get_masterTableView().rebind();
    }
    function checkall(chkboxclientid)
    {
        document.getElementById("hidIsInitial").value = "1";
        var checked = document.getElementById(chkboxclientid).checked;
        var grdmeasuremrntlist = $find("<%=grdmeasurementlist.ClientID %>");
        var MasterTable = grdmeasuremrntlist.get_masterTableView();
        var selectedRows = MasterTable.get_dataItems();
        for (var i = 0; i < selectedRows.length; i++) {
            var row = selectedRows[i];
            row.set_selected(checked);
        }
        document.getElementById("hidIsInitial").value = "0";
        rebinddatatogrid(0, 100, '', '');
    }
</script>