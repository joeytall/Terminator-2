<%@ Page Language="C#" AutoEventWireup="true" CodeFile="procmain.aspx.cs" Inherits="proc_procmain"
    MaintainScrollPositionOnPostback="true" %>

<%@ Register TagPrefix="uc1" TagName="Header" Src="Procheader.ascx" %>
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
    <script type="text/javascript" src="../Jscript/RecalculatePosition.js"></script>
    <script type="text/javascript" src="../Jscript/RadWindow.js"></script>
    <script type="text/javascript" src="../Jscript/Stopdoubleclick.js"></script>
    <script type="text/javascript" src="../Jscript/RadControls.js"></script>
    <script type="text/javascript">

//    var FormSubmitted = false;

    var labourtax1 = 0;
    var labourtax2 = 0;
    var sevicetax1 = 0.05;
    var sevicetax2 = 0.07;
    var URL = "";

    function editLabour(counter, estimate) {
      URL = "../util/labourform.aspx?counter=" + counter + "&ordertype=procedure&estimate=" + estimate + "&ordernum=<%=m_procnum %>";
      var labWnd = radopen(URL, null);
      labWnd.set_modal(true);
      return false;
    }

    function editTask(counter, estimate) {
      URL = "../util/taskform.aspx?counter=" + counter + "&ordertype=procedure&estimate=" + estimate + "&ordernum=<%=m_procnum %>";
      var labWnd = radopen(URL, null);
      labWnd.set_modal(true);
      return false;
    }

    function editMaterial(counter, estimate) {
      URL = "../util/Materialform.aspx?counter=" + counter + "&ordertype=procedure&estimate=" + estimate + "&ordernum=<%=m_procnum %>";
      var labWnd = radopen(URL, null);
      labWnd.set_modal(true);
      return false;
    }

    function editService(counter, estimate) {
      URL = "../util/Serviceform.aspx?counter=" + counter + "&ordertype=procedure&estimate=" + estimate + "&ordernum=<%=m_procnum %>";
      var labWnd = radopen(URL, null);
      labWnd.set_modal(true);
      return false;
    }

    function editTools(counter, estimate) {
      URL = "../util/Toolsform.aspx?counter=" + counter + "&ordertype=procedure&estimate=" + estimate + "&ordernum=<%=m_procnum %>";
      var labWnd = radopen(URL, null);
      labWnd.set_modal(true);
      return false;
    }

    function saveStatus() {
      var obj = document.getElementById("txtprocnum");
      var changed = document.getElementById("hidChanged");
      if (changed == null)
        changed = 1;
      else
        changed = 0;
        var resp;
        if (changed == 1) {
          resp = window.confirm('<%=m_msg["T33"] %>');
        }
        if (resp) {
          if (Page_ClientValidate(''))
            __doPostBack("saveStatus", "");
        }
    }

    function refreshGrid(grid, estimate) {
      var gridid;
      if (grid == "labour")
        gridid = "grdproclabour";
      if (grid == "material")
        gridid = "grdprocmatl";
      if (grid == "service")
        gridid = "grdprocservice";
      if (grid == "tool")
        gridid = "grdproctools";
      if (grid == "tasks")
        gridid = "grdproctasks";
      if (true) {
        var masterTable = $find(gridid).get_masterTableView();
        masterTable.rebind();
      }
    }

    function locFocus() {
      var obj = document.getElementById("txtequipment");
      var obj1 = document.getElementById("HidMobile");
      var x = trim(obj.value);
      if (x.length && obj1.value == '0')
        obj.focus();
    }

    function loclookup() {
      var x = trim($("#txtequipment").val());
      var mob = $("#HidMobile").val();
      if (x.length && mob == '0') {
          alert('<%=m_msg["T38"] %>');
      }
      else {
        generalLookup("txtlocation");
      }
    }

    function addtopm(resname) {
      switch(resname)
      {
          case 'labour':
              dismsg = '<%=m_msg["T39"] %>';
              break;
          case 'material':
              dismsg = '<%=m_msg["T40"] %>';
              break;
          case 'service':
              dismsg = '<%=m_msg["T41"] %>';
              break;
          case 'tool':
              dismsg = '<%=m_msg["T42"] %>';
              break;
          case 'task':
              dismsg = '<%=m_msg["T43"] %>';
              break;
      }
      var resp = window.confirm(dismsg)
      if (resp) {
        return true;
      }
      else {
        return false;
      }
    }

    </script>

    <uc1:Header ID="ucHeader1" runat="server" />
    <asp:Panel ID="MainControlsPanel" runat="server"  CssClass="MainControlsPanelClass">
        <telerik:RadScriptManager ID="RadScriptManager" runat="server">
        </telerik:RadScriptManager>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false">
        </telerik:RadWindowManager>
        <asp:SqlDataSource ID="ProcLabourSqlDataSource" runat="server" ProviderName="System.Data.OleDb">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="ProcServiceSqlDataSource" runat="server" ProviderName="System.Data.OleDb">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="ProcMatlSqlDataSource" runat="server" ProviderName="System.Data.OleDb">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="ProcTasksSqlDataSource" runat="server" ProviderName="System.Data.OleDb">
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="ProcToolsSqlDataSource" runat="server" ProviderName="System.Data.OleDb">
        </asp:SqlDataSource>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="grdprocmatl">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdprocmatl" />
                        <telerik:AjaxUpdatedControl ControlID="txtestmaterial" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="grdproclabour">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdproclabour" />
                        <telerik:AjaxUpdatedControl ControlID="txtesthours" />
                        <telerik:AjaxUpdatedControl ControlID="txtestlabor" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="grdprocservice">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdprocservice" />
                        <telerik:AjaxUpdatedControl ControlID="txtestservice" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="grdproctasks">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdproctasks" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="grdproctools">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdproctools" />
                        <telerik:AjaxUpdatedControl ControlID="txtesttools" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
            <ClientEvents OnRequestStart="RequestStart" OnResponseEnd="ResponseEnd" />
        </telerik:RadAjaxManager>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
        <asp:ValidationSummary ID="ValidationSummary3" runat="server" ShowMessageBox="True"
      ValidationGroup="proclabour" ShowSummary="False" />
    <asp:ValidationSummary ID="ValidationSummary2" runat="server" ShowMessageBox="True"
      ValidationGroup="procservice" ShowSummary="False" />
    <asp:ValidationSummary ID="ValidationSummary6" runat="server" ShowMessageBox="True"
      ValidationGroup="procmatl" ShowSummary="False" />
    <asp:ValidationSummary ID="ValidationSummary8" runat="server" ShowMessageBox="True"
      ValidationGroup="proctasks" ShowSummary="False" />
    <asp:ValidationSummary ID="ValidationSummary10" runat="server" ShowMessageBox="True"
      ValidationGroup="proctools" ShowSummary="False" />
        <asp:HiddenField ID="hidwolabouract" runat="server" />
        <asp:HiddenField ID="hidwolabourest" runat="server" />
        <asp:HiddenField ID="hidwomatlact" runat="server" />
        <asp:HiddenField ID="hidwomatlest" runat="server" />
        <asp:HiddenField ID="hidwoserviceact" runat="server" />
        <asp:HiddenField ID="hidwoserviceest" runat="server" />
        <asp:HiddenField ID="hidwotasksest" runat="server" />
        <asp:HiddenField ID="hidwotasksact" runat="server" />
        <asp:HiddenField ID="hidwotoolsest" runat="server" />
        <asp:HiddenField ID="hidwotoolsact" runat="server" />
        <input id="SelEstLabour" type="hidden" runat="server" />
        <input id="SelEstTasks" type="hidden" runat="server" />
        <input id="SelEstMaterial" type="hidden" runat="server" />
        <input id="SelEstTools" type="hidden" runat="server" />
        <input id="SelEstService" type="hidden" runat="server" />
    </asp:Panel>
    <asp:HiddenField ID="hidMode" runat="server" />
    <asp:HiddenField ID="hidRoot" Value="<%=Request.ApplicationPath %>" runat="server" />
    <asp:HiddenField ID="hidWOCurrentRecord" runat="server" />
    <asp:HiddenField ID="hidTargetStatusCode" Value="<%=statuscode %>" runat="server" />
    <asp:HiddenField ID="hidStatusComments" Value="" runat="server" />
    <asp:HiddenField ID="hidTargetStatus" Value="" runat="server" />
    <asp:HiddenField ID="hidStatusApplyTo" Value="" runat="server" />
    <asp:HiddenField ID="hidToDo" Value="" runat="server" />
    <asp:HiddenField ID="HidSysMatPrice" runat="server" />
    <asp:HiddenField ID="HidSysServicePrice" runat="server" />
    <asp:HiddenField ID="HidMatMarkup" runat="server" />
    <asp:HiddenField ID="hidprocnum" runat="server" />
    <asp:HiddenField ID="HidMatCost" runat="server" />
    <asp:HiddenField ID="HidMatTax1" runat="server" />
    <asp:HiddenField ID="HidMatTax2" runat="server" />
    <asp:HiddenField ID="HidLabTax1" runat="server" />
    <asp:HiddenField ID="HidLabTax2" runat="server" />
    <asp:HiddenField ID="HidLabSelType" runat="server" />
    <asp:HiddenField ID="HidLabCost" runat="server" />
    <asp:HiddenField ID="HidServTax1" runat="server" />
    <asp:HiddenField ID="HidServTax2" runat="server" />
    <asp:HiddenField ID="HidToolTax1" runat="server" />
    <asp:HiddenField ID="HidToolTax2" runat="server" />
    <asp:HiddenField ID="HidMobile" runat="server" />
        <asp:HiddenField ID="hidKeyName" Value="txtprocnum" runat="server" />
        <input type="hidden" id="hidProcessing" value="0" />
    <input type="hidden" id="hidDataChanged" value="0" />
    <input type="hidden" id="hidNoPromt" value="0" />
    <input type="hidden" id="hidempid" value="<%=Session["Login"].ToString() %>" />
    </form>
</body>
<script type="text/javascript">
    $(window).load(function () {
        setpanelheight(1);
        initToolbar();
    });

  function keyPressed(code)
  {
      if (code==13 && "<%=querymode %>" == "query")
          LookupClicked();
  }


</script>
<script type="text/javascript">
    <asp:Literal id="litScript1" runat="server" />
</script>
</html>
