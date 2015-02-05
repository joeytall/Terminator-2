<!DOCTYPE HTML>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="wrmain.aspx.cs" Inherits="workrequest_Wrmain"
  MaintainScrollPositionOnPostback="true" %>

<%@ Register TagPrefix="uc1" TagName="Header" Src="Wrheader.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<html>
<head id="Head1" runat="server">
  <title></title>
  <meta http-equiv="content-type" content="text/html; charset=utf-8" />
  <link type="text/css" href="../Styles/Header.css" rel="stylesheet" />
  <link type="text/css" href="../Styles/Azzier.css" rel="stylesheet" />
  <link type="text/css" href="~/Styles/Customer.css" rel="stylesheet" />
  <link type="text/css" href="../css/custom-theme/jquery-ui-1.8.21.custom.css" rel="stylesheet" />
  <link href="../Styles/jquery.mCustomScrollbar.css" rel="stylesheet" type="text/css" />
  <script type="text/javascript">
  </script>
</head>
<body style="background-image: url(../IMAGES/back.png); background-repeat: repeat-x;
  margin: 0px; padding: 0px;" onkeydown="keyPressed(event.keyCode)" >
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
        var labourtax1 = 0;
        var labourtax2 = 0;
        var sevicetax1 = 0.05;
        var sevicetax2 = 0.07;

        function editLabour(counter, estimate) {
            URL = "../util/labourform.aspx?counter=" + counter + "&ordertype=workrequest&estimate=" + estimate + "&ordernum=<%=m_wrnum %>";
            var labWnd = radopen(URL, null);
            labWnd.set_modal(true);
            return false;
        }

        function editTask(counter, estimate) {
            URL = "../util/taskform.aspx?counter=" + counter + "&ordertype=workrequest&estimate=" + estimate + "&ordernum=<%=m_wrnum %>";
            var labWnd = radopen(URL, null);
            labWnd.set_modal(true);
            return false;
        }

        function editMaterial(counter, estimate) {
            URL = "../util/Materialform.aspx?counter=" + counter + "&ordertype=workrequest&estimate=" + estimate + "&ordernum=<%=m_wrnum %>";
            var labWnd = radopen(URL, null);
            labWnd.set_modal(true);
            return false;
          }

        function editService(counter, estimate) {
            URL = "../util/Serviceform.aspx?counter=" + counter + "&ordertype=workrequest&estimate=" + estimate + "&ordernum=<%=m_wrnum %>";
            var labWnd = radopen(URL, null);
            labWnd.set_modal(true);
            return false;
        }

        function editTools(counter, estimate) {
            URL = "../util/Toolsform.aspx?counter=" + counter + "&ordertype=workrequest&estimate=" + estimate + "&ordernum=<%=m_wrnum %>";
            var labWnd = radopen(URL, null);
            labWnd.set_modal(true);
            return false;
        }

        function saveStatus() {
            var obj = document.getElementById("txtwrnum");
            var changed = document.getElementById("hidDataChanged");
            if (changed == null)
                changed = 0;
            else
                changed = document.getElementById("hidDataChanged").value * 1;
            var resp = 1;
            if (changed == 1) {
                //resp = window.confirm("Workorder has been changed. Do you want to save the changes? Click Yes to save workorder and change status\nClick No to abort. ");
                resp = window.confirm('<%=m_msg["T33"] %>');
                //if (WarnUser())
            }
            if (resp) {
                if (Page_ClientValidate(''))
                    __doPostBack("saveStatus", "");
                else
                {
                  var targetStatusCodeField = document.getElementById("hidTargetStatusCode");
                  if (targetStatusCodeField != null) {
                    var statuscodefield = document.getElementById("txtstatuscode");
                    var oldstatuscode = 1;
                    if (statuscodefield != null)
                      oldstatuscode = statuscodefield.value;

                    targetStatusCodeField.value = oldstatuscode;
                  }
                }

            }
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
              updatetotals();

            }

        function updatetotals() {
            $(document).ready(function () {
            $.ajax({
              type: "GET",
              url: "../InternalServices/ServiceWR.svc/GetTotals",
              data: { "wrnum": "<%=m_wrnum %>" },
              contentType: "application/json; charset=utf-8",
              dataType: "json",
        
              beforeSend: function (xhr, settings) {
                xhr.setRequestHeader("userid", "<%=Session["Login"].ToString() %>");
              },

              success: function (data, status) {
        
                var result = data.d;
          
                var totals = result.split("^");
                if (totals.length == 5)
                {
                  setvalue("txtesthours",totals[0]);
                  setvalue("txtestlabor",totals[1]);
                  setvalue("txtestmaterial",totals[2]);
                  setvalue("txtesttools",totals[3]);
                  setvalue("txtestservice",totals[4]);
                }
                else {
                  alert(result);
                }
              },
              error: OnGetMemberError
            });
          });
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
                //alert("Cannot change location for fixed position equipment.");
                alert('<%=m_msg["T38"] %>');
            }
            else {
                generalLookup("txtlocation");
            }
        }

    function chgframesize() {
    var temp = top.document.getElementById("mainmodule").scrollWidth;
    var scrwidth = screen.width;
    var newframewidth = Math.round((temp * 1) / (scrwidth * 1) * 100);
    var newframewidth1 = 100 - newframewidth;

    $.ajax({
      type: "get",
      data: { "key": "WOFrameMain", "value": newframewidth, "key1": "WOFrameControlPanel", "value1": newframewidth1, "UserModule": "WO" },
      url: "../InternalServices/ServiceGeneral.svc/SetFrameSession",
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (data) {
      },
      error: function (request, status, err) {
        alert(err);
      }
    });
  }

  </script>
  <uc1:Header ID="ucHeader1" runat="server" />
  <asp:Panel ID="MainControlsPanel" CssClass="MainControlsPanelClass" runat="server">
    <telerik:RadScriptManager ID="RadScriptManager" runat="server">
    </telerik:RadScriptManager>
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false">
    </telerik:RadWindowManager>
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
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
      <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="grdmaterial">
          <UpdatedControls>
            <telerik:AjaxUpdatedControl ControlID="grdmaterial" />
            <telerik:AjaxUpdatedControl ControlID="txtestmaterial" />
          </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="grdlabour">
          <UpdatedControls>
            <telerik:AjaxUpdatedControl ControlID="grdlabour" />
            <telerik:AjaxUpdatedControl ControlID="txtestlabor" />
            <telerik:AjaxUpdatedControl ControlID="txtesthours" />
          </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="grdservice">
          <UpdatedControls>
            <telerik:AjaxUpdatedControl ControlID="grdservice" />
            <telerik:AjaxUpdatedControl ControlID="txtestservice" />
          </UpdatedControls>
        </telerik:AjaxSetting>
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
      </AjaxSettings>
      <ClientEvents OnRequestStart="RequestStart" OnResponseEnd="ResponseEnd" />
    </telerik:RadAjaxManager>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ValidationGroup=""
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
  <asp:HiddenField ID="HidLastPrice" runat="server" />
  <asp:HiddenField ID="HidQuotedPrice" runat="server" />
  <asp:HiddenField ID="HidFilename" runat="server" Value="workorder/womain.aspx" />
  <input type="hidden" id="hidProcessing" value="0" />
  <input type="hidden" id="hidDataChanged" value="0" />
  <input type="hidden" id="hidNoPromt" value="0" />
  <input type="hidden" id="hidempid" value="<%=Session["Login"].ToString() %>" />
  <input type="hidden" id="hidrequest" />
  <asp:HiddenField ID="hidKeyName" value="txtwrnum" runat="server" />
  <br />
  <br />
  </form>
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

  function getlocationinfo() {
    
    generalValidateField("txtlocation");
  }


</script>
<script type="text/javascript">
    <asp:Literal id="litScript1" runat="server" />
</script>
</html>
