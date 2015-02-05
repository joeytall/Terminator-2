<%@ Page Language="C#" AutoEventWireup="true" CodeFile="pmmain.aspx.cs" Inherits="pm_pmmain"
    MaintainScrollPositionOnPostback="true" %>

<%@ Register TagPrefix="uc1" TagName="PMHeader" Src="pmheader.ascx" %>
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

      function showroute()
      {
        var routename = "<%=nvcPM["RouteName"] %>";
        if (routename != "")
        {
          url = "../route/routeframe.aspx?URL=routemain.aspx&mode=edit&routename=" + routename;
          //if (window.event.shiftKey) {
            handler = window.open(url, '_blank', 'height=' + screen.height + ',width=' + screen.width + ',left=10,top=10,resizable=yes,scrollbars=yes,toolbar=yes,location=yes,status=yes')
          //}
          //else if (window.event.ctrlKey) {
//            handler = window.open('', '_new').location.href = url;
  //        }
    //      else
      //      top.document.location = url;
        }
        else
        {
          alert("No route is attached to this PM.");
        }
      }

      function clearnonduplicatefields() {
        $(document).ready(function () {
          $('#txtpmgendate').val('');
          $('#txtlastpmreading').val('');
          $('#txtlastpmdate').val('');
          $('#txtlastissuedate').val('');
          $('#txtavgusage').val('');
          $('#txtlastwonum').val('');
          $('#txtlastpmreading2').val('');
          $('input:radio[name=rdbinactive][value=0]').click();
        });
      }

      function editLabour(counter, estimate) {
        URL = "../util/labourform.aspx?counter=" + counter + "&ordertype=pm&estimate=" + estimate + "&ordernum=<%=m_pmnum %>";
        var labWnd = radopen(URL, null);
        labWnd.set_modal(true);
        return false;
      }

      function editTask(counter, estimate) {
        URL = "../util/taskform.aspx?counter=" + counter + "&ordertype=pm&estimate=" + estimate + "&ordernum=<%=m_pmnum %>";
        var labWnd = radopen(URL, null);
        labWnd.set_modal(true);
        return false;
      }

      function editMaterial(counter, estimate) {
        URL = "../util/Materialform.aspx?counter=" + counter + "&ordertype=pm&estimate=" + estimate + "&ordernum=<%=m_pmnum %>";
        var labWnd = radopen(URL, null);
        labWnd.set_modal(true);
        return false;
      }

      function editService(counter, estimate) {
        URL = "../util/Serviceform.aspx?counter=" + counter + "&ordertype=pm&estimate=" + estimate + "&ordernum=<%=m_pmnum %>";
        var labWnd = radopen(URL, null);
        labWnd.set_modal(true);
        return false;
      }

      function editTools(counter, estimate) {
        URL = "../util/Toolsform.aspx?counter=" + counter + "&ordertype=pm&estimate=" + estimate + "&ordernum=<%=m_pmnum %>";
        var labWnd = radopen(URL, null);
        labWnd.set_modal(true);
        return false;
      }

      function refreshGrid(grid, estimate) {
        var gridid;
        if (grid == "labour")
          gridid = "grdlabour";
        if (grid == "material")
          gridid = "grdmaterial";
        if (grid == "service")
          gridid = "grdservice";
        if (grid == "tool")
          gridid = "grdtools";
        if (grid == "tasks")
          gridid = "grdtasks";
        if (true) {
          var masterTable = $find(gridid).get_masterTableView();
          masterTable.rebind();
        }
      }

    </script>
    <uc1:PMHeader ID="ucHeader1" runat="server" />
    <asp:Panel ID="MainControlsPanel" runat="server"  CssClass="MainControlsPanelClass">
        <telerik:RadScriptManager ID="RadScriptManager" runat="server">
        </telerik:RadScriptManager>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false">
        </telerik:RadWindowManager>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="grdtasks">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdtasks" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="grdtools">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdtools" />
                        <telerik:AjaxUpdatedControl ControlID="txtesttools" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="grdlabour">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdlabour" />
                        <telerik:AjaxUpdatedControl ControlID="txtesthours" />
                        <telerik:AjaxUpdatedControl ControlID="txtestlabor" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="grdservice">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdservice" />
                        <telerik:AjaxUpdatedControl ControlID="txtestservice" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="grdmaterial">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdmaterial" />
                        <telerik:AjaxUpdatedControl ControlID="txtestmaterial" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
            <ClientEvents OnRequestStart="RequestStart" OnResponseEnd="ResponseEnd" />
        </telerik:RadAjaxManager>
        <asp:LinkButton ID="btnshowroute" runat="server"  OnClientClick="showroute();return false;"  >
          <asp:Image ID="Image1" runat="server" ImageUrl="../images/route/route_16.png" Height="16px" Width="16px" ToolTip="Show Route" />
        </asp:LinkButton>
        <asp:SqlDataSource ID="LabourSqlDataSource" runat="server" ProviderName="System.Data.OleDb">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="ServiceSqlDataSource" runat="server" ProviderName="System.Data.OleDb">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="MaterialSqlDataSource" runat="server" ProviderName="System.Data.OleDb">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="TasksSqlDataSource" runat="server" ProviderName="System.Data.OleDb">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="ToolsSqlDataSource" runat="server" ProviderName="System.Data.OleDb">
        </asp:SqlDataSource>


        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
        <asp:ValidationSummary ID="ValidationSummary3" runat="server" ShowMessageBox="True"
      ValidationGroup="labour" ShowSummary="False" />
    <asp:ValidationSummary ID="ValidationSummary2" runat="server" ShowMessageBox="True"
      ValidationGroup="service" ShowSummary="False" />
    <asp:ValidationSummary ID="ValidationSummary6" runat="server" ShowMessageBox="True"
      ValidationGroup="material" ShowSummary="False" />
    <asp:ValidationSummary ID="ValidationSummary8" runat="server" ShowMessageBox="True"
      ValidationGroup="tasks" ShowSummary="False" />
    <asp:ValidationSummary ID="ValidationSummary10" runat="server" ShowMessageBox="True"
      ValidationGroup="tools" ShowSummary="False" />            

    </asp:Panel>
    <asp:HiddenField ID="hidMode" runat="server" />
    <asp:HiddenField ID="HidOldPMNum" runat="server" /> 
    <asp:HiddenField ID="hidprocnum" runat="server" />   
    <asp:HiddenField ID="HidDivision" runat="server" />
    <input type="hidden" id="hidDataChanged" value="0" />
    <input type="hidden" id="hidNoPromt" value="0" />
    <input type="hidden" id="hidresourcesource" value="PM" />
    <input type="hidden" id="hidempid" value="<%=Session["Login"].ToString() %>" />
    <input type="hidden" id="HidLabSelType" value="E" />
    <asp:HiddenField ID="HidMobile" runat="server" />

    <asp:HiddenField ID="HidSysMatPrice" runat="server" />
  <asp:HiddenField ID="HidSysServicePrice" runat="server" />
  <asp:HiddenField ID="HidMatMarkup" runat="server" />
  <asp:HiddenField ID="HiddenField1" runat="server" />
  <asp:HiddenField ID="HidMatCost" runat="server" />
  <asp:HiddenField ID="HidMatTax1" runat="server" />
  <asp:HiddenField ID="HidMatTax2" runat="server" />
  <asp:HiddenField ID="HidLabTax1" runat="server" />
  <asp:HiddenField ID="HidLabTax2" runat="server" />
  <asp:HiddenField ID="HiddenField2" runat="server" />
  <asp:HiddenField ID="HidLabCost" runat="server" />
  <asp:HiddenField ID="HidServTax1" runat="server" />
  <asp:HiddenField ID="HidServTax2" runat="server" />
  <asp:HiddenField ID="HidToolTax1" runat="server" />
  <asp:HiddenField ID="HidToolTax2" runat="server" />
  <asp:HiddenField ID="hidKeyName" value="txtpmnum" runat="server" />
  <input type="hidden" id="hidrequest" />
    </form>
    <asp:Literal ID="litScript1" runat="server"></asp:Literal>
</body>

<script type="text/javascript">
  $(window).load(function () {
      setpanelheight(1);
      initToolbar();
  });

    function keyPressed(code) {
      if (code == 13 && "<%=querymode %>" == "query") {
          LookupClicked();
      }
    }

    function locFocus() {
      var obj = document.getElementById("txtequipment");
      var obj1 = document.getElementById("HidMobile");
      var x = trim(obj.value);
      if (x.length && obj1.value == '0')
        obj.focus();
    }

    function loclookup(lookuptype) {
      var x = trim($("#txtequipment").val());
      var mob = $("#HidMobile").val();
      if (x.length && mob == '0') {
        //alert("Cannot change location for fixed position equipment.");
        alert('Cannot change location for fixed position equipment.');
      }
      else {
        if (lookuptype == "1")
          generalLookup("txtlocation");
        else
          generalTreeview("txtlocation");
      }
    }

    function meterfocus() {
      var obj = document.getElementById("txtequipment");
      var x = trim(obj.value);
      if (!x.length) {
        alert("Please enter equipment first.");
        obj.focus();
      }
    }

    function meterlookup(index) {
      var obj = document.getElementById("txtequipment");
      var x = trim(obj.value);
      if (!x.length) {
        alert("Please enter equipment first.");        //alert("Cannot change location for fixed position equipment.");
        obj.focus();
      }
      else {
        if (index == 1)
          generalLookup("txtmetername");
        else
          generalLookup("txtmetername2");
      }
    }

    function resetmeters() {
      $('#txtmetername').val('');
      $('#txtmeterinterval').val('');
      $('#txtmeterrange').val('');
      $('#txtcurrentreading').val('');
      $('#txtnextpmreading').val('');
    }

    function updatenested(index) {
      $($("input[name='rblnestedpm']").get(index)).prop('checked', true);
      $("#rblnestedpm").attr("onclick", "roradiobutton('rblnestedpm'," + index + ")");
    }
 
</script>
</html>

