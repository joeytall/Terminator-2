<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="womain.aspx.cs" Inherits="workorder_Womain"
  MaintainScrollPositionOnPostback="true" %>

<%@ Register TagPrefix="uc1" TagName="Header" Src="Woheader.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<html>
<head id="Head1" runat="server">
  <title></title>
  <meta http-equiv="content-type" content="text/html; charset=utf-8" />
  <link type="text/css" href="../Styles/Header.css" rel="stylesheet" />
  <link type="text/css" href="../Styles/Azzier.css" rel="stylesheet" />
  <link type="text/css" href="../Styles/Customer.css" rel="stylesheet" />
  <link type="text/css" href="../css/custom-theme/jquery-ui-1.8.21.custom.css" rel="stylesheet" />
  <link href="../Styles/jquery.mCustomScrollbar.css" rel="stylesheet" type="text/css" />
  <script type="text/javascript">
  </script>
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
  <script type="text/javascript" src="../Jscript/RadControls.js"></script>
  <script type="text/javascript" src="../Jscript/Stopdoubleclick.js"></script>
  <%--<script type="text/javascript" src="../Jscript/header.js"></script>--%>
  <script type="text/javascript">

        function editLabour(counter, estimate) {
            URL = "../util/labourform.aspx?counter=" + counter + "&ordertype=workorder&estimate=" + estimate + "&ordernum=" + getvalue("hidkeyvalue");
            var labWnd = radopen(URL, null);
            labWnd.set_modal(true);
            return false;
        }

        function editTask(counter, estimate) {
            URL = "../util/taskform.aspx?counter=" + counter + "&ordertype=workorder&estimate=" + estimate + "&ordernum=" + getvalue("hidkeyvalue");
            var labWnd = radopen(URL, null);
            labWnd.set_modal(true);
            return false;
        }

        function editMaterial(counter, estimate) {
            URL = "../util/Materialform.aspx?counter=" + counter + "&ordertype=workorder&estimate=" + estimate + "&ordernum=" + getvalue("hidkeyvalue");
            var labWnd = radopen(URL, null);
            labWnd.set_modal(true);
            return false;
          }

        function editActMaterial(batchnum) {
            URL = "actmaterial.aspx?batchnum=" + batchnum + "&wonum=" + getvalue("hidkeyvalue");
          var labWnd = radopen(URL, null);
          labWnd.set_modal(true);
          return false;
        }

        function editActService(batchnum) {
            URL = "actservice.aspx?batchnum=" + batchnum + "&wonum=" + getvalue("hidkeyvalue");
          var labWnd = radopen(URL, null);
          labWnd.set_modal(true);
          return false;
        }

        function editService(counter, estimate) {
            URL = "../util/Serviceform.aspx?counter=" + counter + "&ordertype=workorder&estimate=" + estimate + "&ordernum=" + getvalue("hidkeyvalue");
            var labWnd = radopen(URL, null);
            labWnd.set_modal(true);
            return false;
        }

        function editTools(counter, estimate) {
            URL = "../util/Toolsform.aspx?counter=" + counter + "&ordertype=workorder&estimate=" + estimate + "&ordernum=" + getvalue("hidkeyvalue");
            var labWnd = radopen(URL, null);
            labWnd.set_modal(true);
            return false;
        }

        function saveStatus() {
            var obj = document.getElementById("txtwonum");
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
                {
                    SubmitStatusChange();
                }
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

      function SubmitStatusChange()
      {
          saveworkorder(true);
      }

      function saveworkorder(updatestatus) {
          var dbfield = "", dbval = "", procnum = "";
          var userid = '<%=Session["Login"] %>';
          //var statuscode = $('input:hidden[name=hidTargetStatusCode]').val();
          //var hasstatuscode = false;
          var hidprocnum = getdatafield("ProcNum");

          var dirtylog = $('#txtdirtylog').val();
          var woxml = "<?xml version='1.0' encoding='UTF-8'?><workorders><workorder>";

          var worate = getvalue("hidworate");
          var oldworate = getvalue("hidoldworate");

          if (!isDecimal(worate))
              worate = 0;
          if (!isDecimal(oldworate))
              oldworate = 0;

          if (worate * 1 != oldworate * 1) {
              var resp = window.confirm("Labour cost will be updated as Work Type is changed. Click OK to continue, click Cancel to abort.");
              if (!resp)
                  return;
          }

          woxml = woxml + collectformall("WorkOrder");
          procnum = getvalue("txtprocnum");

          if (dirtylog != '') {
              woxml = woxml + "<dirtylog>" + dirtylog + "</dirtylog>";
          }
          woxml = woxml + "</workorder></workorders>";
          //            $.support.cors = true;
          if (!allowsubmit()) return;

          if (!updatestatus)
          {
              $.ajax({
                  type: "GET",
                  url: "../InternalServices/ServiceWO.svc/SaveWorkorder",
                  data: { "xmlnvc": woxml },
                  contentType: "application/json; charset=utf-8",
                  dataType: "json",
                  beforeSend: function (xhr, settings) {
                      xhr.setRequestHeader("userid", userid);
                      xhr.setRequestHeader("hidprocnum", hidprocnum);
                  },
                  success: function (data, status) {
                      var result = data.d;
                      var d_array = result.split("^");
                      if (d_array[0] == "TRUE") {
                          var newdirtylog
                          newdirtylog = parseInt(dirtylog) + 1;

                          $('#txtdirtylog').val(newdirtylog);

                          if (hidprocnum != procnum) {
                              $('input:hidden[name=hidprocnum]').val(procnum);
                              //alert("procnum: " + $('#hidprocnum').val());
                              //gridrefresh(); //???
                          }
                          savesucceeded(d_array[1]);
                          //savesucceeded(d_array[1], status);
                      }
                      else if (d_array[0] == "FALSE") {
                          alert(d_array[1]);
                      }
                      FormSubmitted = false;
                  },
                  error: OnGetMemberError
              });
          }
          else
          {
              var statuscode = getvalue("hidTargetStatusCode");
              var status = getvalue("hidTargetStatus");
              var remark = getvalue("hidStatusComments");
              $.ajax({
                  type: "GET",
                  url: "../InternalServices/ServiceWO.svc/SaveWorkorderStatus",
                  data: { "xmlnvc": woxml, "statuscode":statuscode,"status":status,"remark":remark },
                  contentType: "application/json; charset=utf-8",
                  dataType: "json",
                  beforeSend: function (xhr, settings) {
                      xhr.setRequestHeader("userid", userid);
                      xhr.setRequestHeader("hidprocnum", hidprocnum);
                  },
                  success: function (data, status) {
                      var result = data.d;
                      var d_array = result.split("^");
                      if (d_array[0] == "TRUE") {
                          var newdirtylog
                          newdirtylog = parseInt(dirtylog) + 1;

                          $('#txtdirtylog').val(newdirtylog);

                          if (hidprocnum != procnum) {
                              $('input:hidden[name=hidprocnum]').val(procnum);
                              //alert("procnum: " + $('#hidprocnum').val());
                              //gridrefresh(); //???
                          }
                          savesucceeded(d_array[1]);
                          //savesucceeded(d_array[1], status);
                      }
                      else if (d_array[0] == "FALSE") {
                          alert(d_array[1]);
                      }
                      FormSubmitted = false;
                  },
                  error: OnGetMemberError
              });
          }


      }

        function refreshGrid(grid, estimate) {
            var gridid;
            if (grid == "labour")
                gridid = "grdwolabour";
            if (grid == "material")
                gridid = "grdwomatl";
            if (grid == "service")
                gridid = "grdwoservice";
            if (grid == "tool")
                gridid = "grdwotools";
            if (grid == "tasks")
                gridid = "grdwotasks";
            if (estimate == 1)
                gridid = gridid + "est";
            else
                gridid = gridid + "act";
            if (true) {
                var jsondata = $("#" +  gridid).attr("searchQuery");
                var data = CreateData(jsondata);
                RebindDataToGrid(data, gridid);
              }
              updatetotals();

            }

        function updatetotals() {
            $(document).ready(function () {
            $.ajax({
              type: "GET",
              url: "../InternalServices/ServiceWO.svc/GetTotals",
              data: { "wonum": getvalue("hidkeyvalue") },
              contentType: "application/json; charset=utf-8",
              dataType: "json",

              beforeSend: function (xhr, settings) {
                xhr.setRequestHeader("userid", "<%=Session["Login"].ToString() %>");
              },

              success: function (data, status) {

                var result = data.d;

                var totals = result.split("^");
                if (totals.length == 10)
                {

                  setvalue("txtesthours",totals[0]);
                  setvalue("txtestlabor",totals[1]);
                  setvalue("txtestmaterial",totals[2]);
                  setvalue("txtesttools",totals[3]);
                  setvalue("txtestservice",totals[4]);
                  setvalue("txtacthours",totals[5]);
                  setvalue("txtactlabor",totals[6]);
                  setvalue("txtactmaterial",totals[7]);
                  setvalue("txtacttools",totals[8]);
                  setvalue("txtactservice",totals[9]);
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
          if (getvalue("hidMode") != "query")
          {
              var obj = document.getElementById("txtequipment");
              var obj1 = document.getElementById("hidMobile");
              var x = trim(obj.value);
              if (x.length && obj1.value == '0')
                  obj.focus();
          }
        }

      function loclookup(lookuptype) {
          if (getvalue("hidMode") != "query")
          {
              var x = trim($("#txtequipment").val());
              var mob = $("#hidMobile").val();
              if (x.length && mob == '0') {
                  //alert("Cannot change location for fixed position equipment.");
                  alert('<%=m_msg["T38"] %>');
              }
              else {
                  if (lookuptype == "1")
                      generalLookup("txtlocation");
                  else
                      generalTreeview("txtlocation");
              }
          }
          else
          {
              if (lookuptype == "1")
                  generalLookup("txtlocation");
              else
                  generalTreeview("txtlocation");
          }
        }

      function phaseFocus() {
          var obj = document.getElementById("txtprojectid");
          var x = trim(obj.value);
          if (!x.length )
          {
            alert("Please enter project first.");
            obj.focus();
          };
      }

      function phaseblur()
      {
          if (getvalue("hidMode") != "query")
          {
              if (getvalue("txtprojectid") == "" && trim(getvalue("txtphase")) != "")
              {
                  updatevalidationcount(-1);
                  alert("Please enter project first");
                  setTimeout('setvalue("txtphase","")',0);
              }
          }
      }

      function phaselookup() {
          if (getvalue("hidMode") != "query")
          {
              var x = trim($("#txtprojectid").val());
              if (!x.length) {
                  alert("Please enter project first.");
                  $("#txtprojectid").focus();
              }
              else {
                  generalLookup("txtphase");
              }
          }
          else
          {
              generalLookup("txtphase");
          }
        }

        function clearphase()
        {
          setvalue("txtphase","");
        }

  </script>
  <uc1:Header ID="ucHeader1" runat="server" />
  <asp:Panel ID="MainControlsPanel" CssClass="MainControlsPanelClass" runat="server">
    <telerik:RadScriptManager ID="RadScriptManager" runat="server">
    </telerik:RadScriptManager>
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false" ShowContentDuringLoad="true">
    </telerik:RadWindowManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
      <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="grdwomatlact">
          <UpdatedControls>
            <telerik:AjaxUpdatedControl ControlID="grdwomatlact" />
            <telerik:AjaxUpdatedControl ControlID="txtactmaterial" />
          </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="grdwomatlest">
          <UpdatedControls>
            <telerik:AjaxUpdatedControl ControlID="grdwomatlest" />
            <telerik:AjaxUpdatedControl ControlID="txtestmaterial" />
          </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="grdwolabouract">
          <UpdatedControls>
            <telerik:AjaxUpdatedControl ControlID="grdwolabouract" />
            <telerik:AjaxUpdatedControl ControlID="txtactlabor" />
            <telerik:AjaxUpdatedControl ControlID="txtacthours" />
          </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="grdwolabourest">
          <UpdatedControls>
            <telerik:AjaxUpdatedControl ControlID="grdwolabourest" />
            <telerik:AjaxUpdatedControl ControlID="txtestlabor" />
            <telerik:AjaxUpdatedControl ControlID="txtesthours" />
          </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="grdwoserviceact">
          <UpdatedControls>
            <telerik:AjaxUpdatedControl ControlID="grdwoserviceact" />
            <telerik:AjaxUpdatedControl ControlID="txtactservice" />
          </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="grdwoserviceest">
          <UpdatedControls>
            <telerik:AjaxUpdatedControl ControlID="grdwoserviceest" />
            <telerik:AjaxUpdatedControl ControlID="txtestservice" />
          </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="grdwotasksest">
          <UpdatedControls>
            <telerik:AjaxUpdatedControl ControlID="grdwotasksest" />
          </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="grdwotasksact">
          <UpdatedControls>
            <telerik:AjaxUpdatedControl ControlID="grdwotasksact" />
          </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="grdwotoolsest">
          <UpdatedControls>
            <telerik:AjaxUpdatedControl ControlID="grdwotoolsest" />
            <telerik:AjaxUpdatedControl ControlID="txtesttools" />
          </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="grdwotoolsact">
          <UpdatedControls>
            <telerik:AjaxUpdatedControl ControlID="grdwotoolsact" />
            <telerik:AjaxUpdatedControl ControlID="txtacttools" />

          </UpdatedControls>
        </telerik:AjaxSetting>
      </AjaxSettings>
      <ClientEvents OnRequestStart="RequestStart" OnResponseEnd="ResponseEnd" />
    </telerik:RadAjaxManager>
    <asp:LinkButton ID="btnenterroute" runat="server" OnClientClick="enterroute();return false;">
      <asp:Image ID="Image1" runat="server" ImageUrl="../images/route/add_reading_32.png"
        Height="16px" Width="16px" ToolTip="Enter Reading" />
    </asp:LinkButton>

    <asp:LinkButton ID="btngotoproj" runat="server" OnClientClick="gotoproj();return false;">
      <asp:Image ID="Image2" runat="server" ImageUrl="../images/project/go_to_projects_16.png" Height="16px" Width="16px" ToolTip="Goto Project" />
    </asp:LinkButton>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
      ShowSummary="False" />



  </asp:Panel>
  <asp:HiddenField ID="hidMode" runat="server" />
  <asp:HiddenField ID="hidRoot" Value="<%=Request.ApplicationPath %>" runat="server" />
  <asp:HiddenField ID="hidWOCurrentRecord" runat="server" />
  <asp:HiddenField ID="hidTargetStatusCode" Value="<%=statuscode %>" runat="server" />
  <asp:HiddenField ID="hidStatusComments" Value="" runat="server" />
  <asp:HiddenField ID="hidTargetStatus" Value="" runat="server" />
  <asp:HiddenField ID="hidStatusApplyTo" Value="" runat="server" />
  <asp:HiddenField ID="hidToDo" Value="" runat="server" />
  <input type="hidden" id="hidMobile" value="0" />
  <asp:HiddenField ID="hidprocnum" runat="server" />
  <asp:HiddenField ID="HidFilename" runat="server" Value="workorder/womain.aspx" />
  <input type="hidden" id="hidProcessing" value="0" />
  <input type="hidden" id="hidDataChanged" value="0" />
  <input type="hidden" id="hidNoPromt" value="0" />
  <input type="hidden" id="hidempid" value="<%=Session["Login"].ToString() %>" />
  <input type="hidden" id="hidscreencounter" value="<%=screen.ScreenCounter %>" />
  <input type="hidden" id="hidwarrantydate" value="" />
  <asp:HiddenField ID="hiddirtylog" runat="server" />
  <asp:HiddenField ID="hidworate" runat="server" />
  <asp:HiddenField ID="hidoldworate" runat="server" />
  <asp:HiddenField ID="hidKeyName" value="txtwonum" runat="server" />
  <input type="hidden" id="hidrequest" />
  <input type="hidden" id="hiddata" />
  <input type="hidden" id="hidfirst" value="1" />
  <input type="hidden" id="hidkeyvalue" runat="server" />
  <input type="hidden" id="hiduserrights" runat="server" />
  <input type="hidden" id="hidolddiv" />
  <input type="hidden" id="hidwrcount" />
  <input type="hidden" id="hidtimerstatus" />


  <br />
  <br />
  </form>
</body>
<script type="text/javascript">
  var g_LCID = <%=Session["LCID"].ToString() %>;
  var disabledestlabour = new Array;
  var disabledestmaterial = new Array;
  var disabledestservice = new Array;
  var disabledesttasks = new Array;
  var disabledesttools = new Array;

  function getTimerStatus(){
      return getvalue("ucHeader1_hidtimerstatus");
  }

    function checkwarranty() {
      var warrantydate = getvalue("hidwarrantydate");
      var warrantyyear = retYear(warrantydate);
      var warrantymonth = retMonth(warrantydate);
      var warrantyday = retDay(warrantydate);
      var wardate = new Date(warrantyyear,warrantymonth,warrantyday);
      var today = new Date();
      if (wardate >= today)
        alert("Equipment is under warranty.");
    }

  function enterroute()
  {
      var routename = getdatafield("routename");
    if (routename != "")
    {
      URL = "../route/readings.aspx?routename=" + routename;
      var Wnd = radopen(URL, null);
      Wnd.set_modal(true);
      return false;
    }
    else
    {
      alert("No route is attached to this Work Order.");
    }
  }

  function gotoproj()
  {
    var projectid = getdatafield("projectid");
    if (projectid != "")
    {
      URL = "../project/projectframe.aspx?projectid=" + projectid + "&mode=edit";
      newwin = window.open(URL, "", "Left = 10, Top = 10, width=<%=Session["ScreenWidth"]%>,height=<%=Session["ScreenHeight"]%>,resizable = yes, menubar = yes, statusbar= yes");
    }
    else
    {
      alert("This Work Order has no project.");
    }
  }

  function CopyMaterialToActual() {
    var grid = $find("<%=grdwomatlest.ClientID %>");
    if (grid != null) {
      var MasterTable = grid.get_masterTableView();
      var selectedRows = MasterTable.get_selectedItems();
      var counterlist = "";
      var i = 0;
      for (i = 0;i< selectedRows.length;i++)
      {
        if (counterlist == "")
          counterlist = selectedRows[i].getDataKeyValue("Counter");
        else
          counterlist = counterlist + "," + selectedRows[i].getDataKeyValue("Counter");
      }
      if (selectedRows.length > 0) {
        var dtp = $telerik.findControl(grid.get_element(), "rdpDate");
        if (dtp.get_selectedDate() != null) {
          var mydate = dtp.get_selectedDate();
          var month = mydate.getMonth() + 1;
          var datestr = mydate.getFullYear() + "/" + month + "/" + mydate.getDate();


          if (!allowsubmit())
              return false;

            $.ajax({
                type: "GET",
                url: "../InternalServices/ServiceWO.svc/CopyMaterial",
                data: { "wonum": getvalue("hidkeyvalue"),"counterlist":counterlist,"transdate": datestr},
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function (xhr, settings) {
                    xhr.setRequestHeader("userid", "<%=Session["Login"] %>");
                },
                success: function (data, status) {
                    var result = data.d;
                    var counter;
                    FormSubmitted = false;
                    if (result == "OK") {
                        var jsondata = $("#grdwomatlest").attr("searchQuery");
                        var data = CreateData(jsondata);
                        RebindDataToGrid(data, "grdwomatlest");

                        jsondata = $("#grdwomatlact").attr("searchQuery");
                        var data = CreateData(jsondata);
                        RebindDataToGrid(data, "grdwomatlact");
                    }
                    else
                    {
                      alert(result);
                    }
                },
                error: OnGetMemberError
            });
            return false;
        }
        else {
          alert('<%=m_msg["T36"] %>');
          var obj = $telerik.findControl(grid.get_element(), "rdpDate");
          obj.get_dateInput().focus();
          return false;
        }
      }
      else {
        alert('<%=m_msg["T37"] %>');
        return false;
      }
    }
  }

  function CopyTasksToActual() {
    var grid = $find("<%=grdwotasksest.ClientID %>");
    if (grid != null) {
      var MasterTable = grid.get_masterTableView();
      var selectedRows = MasterTable.get_selectedItems();
      var counterlist = "";
      var i = 0;
      for (i = 0;i< selectedRows.length;i++)
      {
        if (counterlist == "")
          counterlist = selectedRows[i].getDataKeyValue("Counter");
        else
          counterlist = counterlist + "," + selectedRows[i].getDataKeyValue("Counter");
      }
      if (selectedRows.length > 0) {
        var dtp = $telerik.findControl(grid.get_element(), "rdpDate");
        if (dtp.get_selectedDate() != null) {
          var mydate = dtp.get_selectedDate();
          var month = mydate.getMonth() + 1;
          var datestr = mydate.getFullYear() + "/" + month + "/" + mydate.getDate();

          if (!allowsubmit())
              return false;
          //var myyear =
          $.ajax({
              type: "GET",
              url: "../InternalServices/ServiceWO.svc/CopyTasks",
              data: { "wonum": getvalue("hidkeyvalue"),"counterlist":counterlist,"transdate": datestr},
              contentType: "application/json; charset=utf-8",
              dataType: "json",
              beforeSend: function (xhr, settings) {
                  xhr.setRequestHeader("userid", "<%=Session["Login"] %>");
              },
              success: function (data, status) {
                  var result = data.d;
                  var counter;
                  FormSubmitted = false;
                  if (result == "OK") {
                      //refreshgrid(getvalue("hidkeyvalue"));
                      var jsondata = $("#grdwotasksest").attr("searchQuery");
                      var data = CreateData(jsondata);
                      RebindDataToGrid(data, "grdwotasksest");

                      jsondata = $("#grdwotasksact").attr("searchQuery");
                      var data = CreateData(jsondata);
                      RebindDataToGrid(data, "grdwotasksact");
                  }
                  else
                  {
                    alert(result);
                    //document.location.href = "../workorder/womain.aspx?mode=edit&wonum=" + getvalue("hidkeyvalue");
                  }
              },
              error: OnGetMemberError
          });
            return false;
        }
        else {
          alert('<%=m_msg["T36"] %>');
          var obj = $telerik.findControl(grid.get_element(), "rdpDate");
          obj.get_dateInput().focus();
          return false;
        }
      }
      else {
        alert('<%=m_msg["T37"] %>');
        return false;
      }
    }
  }

  function CopyLabourToActual() {
    var grid = $find("<%=grdwolabourest.ClientID %>");
    if (grid != null) {
      var MasterTable = grid.get_masterTableView();
      var selectedRows = MasterTable.get_selectedItems();
      var counterlist = "";
      var i = 0;
      for (i = 0;i< selectedRows.length;i++)
      {
        if (counterlist == "")
          counterlist = selectedRows[i].getDataKeyValue("Counter");
        else
          counterlist = counterlist + "," + selectedRows[i].getDataKeyValue("Counter");
      }
      if (selectedRows.length > 0) {
          //var dtp = $find(document.MainForm.hidTasksCopyDate.value);
          var dtp = $telerik.findControl(grid.get_element(), "rdpDate");
        if (dtp.get_selectedDate() != null) {
          var mydate = dtp.get_selectedDate();
          var month = mydate.getMonth() + 1;
          var datestr = mydate.getFullYear() + "/" + month + "/" + mydate.getDate();


          if (!allowsubmit())
              return false;
          $.ajax({
              type: "GET",
              url: "../InternalServices/ServiceWO.svc/CopyLabour",
              data: { "wonum": getvalue("hidkeyvalue"),"counterlist":counterlist,"transdate": datestr},
              contentType: "application/json; charset=utf-8",
              dataType: "json",
              beforeSend: function (xhr, settings) {
                  xhr.setRequestHeader("userid", "<%=Session["Login"] %>");
              },
              success: function (data, status) {
                  var result = data.d;
                  var counter;
                  FormSubmitted = false;
                  if (result == "OK") {
                      var jsondata = $("#grdwolabourest").attr("searchQuery");
                      var data = CreateData(jsondata);
                      RebindDataToGrid(data, "grdwolabourest");

                      jsondata = $("#grdwolabouract").attr("searchQuery");
                      var data = CreateData(jsondata);
                      RebindDataToGrid(data, "grdwolabouract");
                  }
                 else
                 {
                    alert(result);
                    //document.location.href = "../workorder/womain.aspx?mode=edit&wonum=" + getvalue("hidkeyvalue");
                 }
                },
                error: OnGetMemberError
            });
            return false;
        }
        else {
          alert('<%=m_msg["T36"] %>');
          var obj = $telerik.findControl(grid.get_element(), "rdpDate");
          obj.get_dateInput().focus();
          return false;
        }
      }
      else {
        alert('<%=m_msg["T37"] %>');
        return false;
      }
    }
  }

  function CopyServicesToActual() {
    var grid = $find("<%=grdwoserviceest.ClientID %>");
    if (grid != null) {
      var MasterTable = grid.get_masterTableView();
      var selectedRows = MasterTable.get_selectedItems();
      var counterlist = "";
      var i = 0;
      for (i = 0;i< selectedRows.length;i++)
      {
        if (counterlist == "")
          counterlist = selectedRows[i].getDataKeyValue("Counter");
        else
          counterlist = counterlist + "," + selectedRows[i].getDataKeyValue("Counter");
      }
      if (selectedRows.length > 0) {
          var dtp = $telerik.findControl(grid.get_element(), "rdpDate");
        if (dtp.get_selectedDate() != null) {
          var mydate = dtp.get_selectedDate();
          var month = mydate.getMonth() + 1;
          var datestr = mydate.getFullYear() + "/" + month + "/" + mydate.getDate();


          if (!allowsubmit())
              return false;

            //var myyear =
            $.ajax({
                type: "GET",
                url: "../InternalServices/ServiceWO.svc/CopyService",
                data: { "wonum": getvalue("hidkeyvalue"),"counterlist":counterlist,"transdate": datestr},
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function (xhr, settings) {
                    xhr.setRequestHeader("userid", "<%=Session["Login"] %>");
                },
                success: function (data, status) {
                    var result = data.d;
                    var counter;
                    FormSubmitted = false;
                    if (result == "OK") {
                        var jsondata = $("#grdwoserviceest").attr("searchQuery");
                        var data = CreateData(jsondata);
                        RebindDataToGrid(data, "grdwoserviceest");

                        jsondata = $("#grdwoserviceact").attr("searchQuery");
                        var data = CreateData(jsondata);
                        RebindDataToGrid(data, "grdwoserviceact");
                    }
                    else
                    {
                      alert(result);
                      //document.location.href = "../workorder/womain.aspx?mode=edit&wonum=" + getvalue("hidkeyvalue");
                    }
                },
                error: OnGetMemberError
            });
            return false;
        }
        else {
          alert('<%=m_msg["T36"] %>');
          var obj = $telerik.findControl(grid.get_element(), "rdpDate");
          obj.get_dateInput().focus();
          return false;
        }
      }
      else {
        alert('<%=m_msg["T37"] %>');
        return false;
      }
    }
  }

  function CopyToolsToActual() {
    var grid = $find("<%=grdwotoolsest.ClientID %>");
    if (grid != null) {
      var MasterTable = grid.get_masterTableView();
      var selectedRows = MasterTable.get_selectedItems();
      var counterlist = "";
      var i = 0;
      for (i = 0;i< selectedRows.length;i++)
      {
        if (counterlist == "")
          counterlist = selectedRows[i].getDataKeyValue("Counter");
        else
          counterlist = counterlist + "," + selectedRows[i].getDataKeyValue("Counter");
      }
      if (selectedRows.length > 0) {
        var dtp = $telerik.findControl(grid.get_element(), "rdpDate");
        if (dtp.get_selectedDate() != null) {
          var mydate = dtp.get_selectedDate();
          var month = mydate.getMonth() + 1;
          var datestr = mydate.getFullYear() + "/" + month + "/" + mydate.getDate();


          if (!allowsubmit())
              return false;
            $.ajax({
                type: "GET",
                url: "../InternalServices/ServiceWO.svc/CopyTools",
                data: { "wonum": getvalue("hidkeyvalue"),"counterlist":counterlist,"transdate": datestr},
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function (xhr, settings) {
                    xhr.setRequestHeader("userid", "<%=Session["Login"] %>");
                },
                success: function (data, status) {
                    var result = data.d;
                    var counter;
                    FormSubmitted = false;
                    if (result == "OK") {
                        var jsondata = $("#grdwotoolsest").attr("searchQuery");
                        var data = CreateData(jsondata);
                        RebindDataToGrid(data, "grdwotoolsest");

                        jsondata = $("#grdwotoolsact").attr("searchQuery");
                        var data = CreateData(jsondata);
                        RebindDataToGrid(data, "grdwotoolsact");
                    }
                    else
                    {
                      alert(result);
                      //document.location.href = "../workorder/womain.aspx?mode=edit&wonum=" + getvalue("hidkeyvalue");
                    }
                },
                error: OnGetMemberError
            });
            return false;
        }
        else {
          alert('<%=m_msg["T36"] %>');
          var obj = $telerik.findControl(grid.get_element(), "rdpDate");
          obj.get_dateInput().focus();
          return false;
        }
      }
      else {
        alert('<%=m_msg["T37"] %>');
        return false;
      }
    }
  }

  function keyPressed(code) {
    if (code == 13 && getvalue("hidMode") == "query") {
        LookupClicked();
    }
  }

  function showwrlist(wonum)
  {
    URL = "cancelworelatewr.aspx?wonum=" + wonum;
    setTimeout("var Wnd = radopen(URL, null);Wnd.set_modal(true);",100);
    //var Wnd = radopen(URL, null);Wnd.set_modal(true);

  }
</script>

<script type="text/javascript">
    function loadrecord(tablename,keyfield, keyvalue, module)
    {
        var oldkey = getdatafield(keyfield.toLowerCase());
        var mode = getvalue("hidMode");
        $.ajax({
            type: "GET",
            url: "../InternalServices/ServiceGeneral.svc/GetDataInJson",
            data: { "queryname":tablename,"keyvalue": keyvalue,"key": keyfield },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, status) {
                var result = data.d;
                var valid = true;
                try
                {
                    var json = JSON.parse(result);
                }
                catch( err)
                {
                    valid= false;
                    alert(result);
                }
                if (valid)
                {
                    setvalue("hiddata",data.d);
                    setvalue("hidkeyvalue",keyvalue);
                    populatescreen(data.d, module);
                    resetscreen(module,"edit");
                    refreshgrid(keyvalue);
                    //parent.window.frames[1].HighlightSelection(keyvalue);
                    if (oldkey != keyvalue || mode == "new" || mode == "duplicate")
                        addtorecentlist(keyvalue,module);
                    else
                        RefreshPanel();


                }

            },
            error: OnGetMemberError
        });
    }

        function setmode(mode)
        {
            setvalue("hidMode",mode);
            $("#spnoperation").html(mode);
            window.status = "";
        }

        function populatescreen(data, module)
        {
            var jsondata = JSON.parse(data);
            for (var key in jsondata) {
                if (jsondata.hasOwnProperty(key)) {
                    var obj = $("DBField='" + key + "'")
                    setscreenvalue(key,jsondata[key]);
                    if (key.toLowerCase() == "dirtylog")
                        setvalue("hiddirtylog",jsondata[key]);
                }
            }
        }

        function setscreenvalue(key,value)
        {
            var object = document.getElementById("txt" + key.toLowerCase());
            if (object != null)  // is textbox or textarea
                setvalue("txt" + key.toLowerCase(),value);
            else
            {
                object = document.getElementById("chk" + key.toLowerCase());
                if (object != null)
                    setvalue("chk" + key.toLowerCase(),value);
                else
                {
                    object = document.getElementById("rbl" + key.toLowerCase());
                    if (object != null)
                    {
                        $('input[name="' + "rbl" + key.toLowerCase() + '"][value="' + value + '"]').prop('checked', true);
                    }
                    else
                    {
                        object = $find("cbb" + key.toLowerCase());
                        if (object!=null)
                        {
                            var item = cbb.findItemByValue(recipienttype);
                            item.select();
                        }
                    }
                }
            }
        }



        function refreshgrid(wonum)
        {
            var jsonData = "{\"WoNum\":\"" + wonum + "\",\"Estimate\":\"0\",\"Inactive\":\"0\",\"OrderType\":\"" + "WorkOrder" + "\"}";
            //var jsonData = "{\"EmailNotifyName\":\"" + name + "\"}",
            var data = CreateData(jsonData);
            $("#grdwolabouract").attr("searchQuery",jsonData);
            RebindDataToGrid(data, "grdwolabouract");
            $("#grdwotasksact").attr("searchQuery",jsonData);
            RebindDataToGrid(data, "grdwotasksact");
            $("#grdwotoolsact").attr("searchQuery",jsonData);
            RebindDataToGrid(data, "grdwotoolsact");


            jsonData = "{\"WoNum\":\"" + wonum + "\",\"Estimate\":\"1\",\"Inactive\":\"0\",\"OrderType\":\"" + "WorkOrder" + "\"}";
            //var jsonData = "{\"EmailNotifyName\":\"" + name + "\"}",
            data = CreateData(jsonData);
            $("#grdwolabourest").attr("searchQuery",jsonData);
            RebindDataToGrid(data, "grdwolabourest");

            $("#grdwomatlest").attr("searchQuery",jsonData);
            RebindDataToGrid(data, "grdwomatlest");

            $("#grdwotasksest").attr("searchQuery",jsonData);
            RebindDataToGrid(data, "grdwotasksest");

            $("#grdwotoolsest").attr("searchQuery",jsonData);
            RebindDataToGrid(data, "grdwotoolsest");

            $("#grdwoserviceest").attr("searchQuery",jsonData);
            RebindDataToGrid(data, "grdwoserviceest");

            jsonData = "{\"WONum\":\"" + wonum + "\",\"Quantity\":\"<>0\",\"TransType\":\"" + "Issue" + "\"}";
            data = CreateData(jsonData);
            $("#grdwoserviceact").attr("searchQuery",jsonData);
            RebindDataToGrid(data, "grdwoserviceact");

            jsonData = "{\"WONum\":\"" + wonum + "\",\"Quantity\":\"<>0\",\"TransType\":\"" + "Issue" + "\"}";
            data = CreateData(jsonData);
            $("#grdwomatlact").attr("searchQuery",jsonData);
            RebindDataToGrid(data, "grdwomatlact");
        }

        function resetfieldstatus()
        {
            $("input").each(function()
            {
                setfieldstatus(this);
            });
        }

        function setfieldstatus(obj)
        {
            var statuscode = 1;
            var code = getvalue("txtstatuscode");
            if (!isDecimal(code))
                statuscode = 1;
            else
                statuscode = code*1;
            var mode = getvalue("hidMode");

            var mandatory = $(obj).attr("Mandatory");
            if (mandatory == null)
                mandatory = 0 ;
            var ro = $(obj).attr("IsReadOnly");
            if (ro == null)
                ro = 0;

            if (mandatory!=0 || ro !=0)
            {

                var ismandatory = false;
                var isreadonly = false;
                var id = obj.id;
                var controlid = id.substring(3);

                //alert(id + ":" + mandatory + ":" +  statuscode);
                if (mandatory*1 <= statuscode && mode != "query" && mandatory*1>=1)
                {
                    ismandatory = true;
                }

                if ((ro*1 >= 1 && mode == "edit") || (ro*1 == 2 && (mode == "new" || mode == "duplicate")))
                {
                    isreadonly = true;
                }

                var needchange =  $(obj).attr("CustomCss") == null || $(obj).attr("CustomCss") == "textline" ;

                if (isreadonly && ismandatory)  // both readonly and mandatory then set to mandatory
                {
                    if (needchange)
                    {
                        $(obj).attr("ClientCss","mdtfield") ;
                        $(obj).addClass("mdtfield");
                        $(obj).removeClass("rofield");
                    }
                }

                if (!isreadonly && ismandatory)
                {
                    if (needchange)
                    {
                        $(obj).attr("ClientCss","mdtfield") ;
                        $(obj).addClass("mdtfield");
                        $(obj).removeClass("rofield");
                    }
                }

                if (isreadonly && !ismandatory)  // both readonly and mandatory then set to mandatory
                {
                    if (needchange)
                    {
                        $(obj).attr("ClientCss","rofield") ;
                        $(obj).addClass("rofield");
                        $(obj).removeClass("mdtfield");
                    }
                }

                if (!isreadonly && !ismandatory)  // both readonly and mandatory then set to mandatory
                {
                    if (needchange)
                    {
                        $(obj).attr("ClientCss","textline") ;
                        $(obj).removeClass("mdtfield");
                        $(obj).removeClass("rofield");
                    }
                }

                if (isreadonly)
                {
                    $(obj).attr('readonly', true);
                    $("#lku" + controlid).hide();
                    $("#tlk" + controlid).hide();
                    $("#dtp" + controlid).hide();
                }
                else
                {
                    $(obj).attr('readonly', false);
                    $("#lku" + controlid).show();
                    $("#tlk" + controlid).show();
                    $("#dtp" + controlid).show();
                }


            }
        }



   <%-- $(document).ready(function () {
        setpanelheight(1);
        //var inp = document.getElementById('SelEstLabour');
        //var data = inp.value;
        if (!IsOldBrowser(10) )
            inittimertoolitems("<%= ucHeader1.TimerStatus%>");
        //if (data != "") {
        //var rowsData = data.split(":");
        //var i = 0;
        //while (typeof (rowsData[i]) != "undefined") {
        //  if (rowsData[i] != "") {
        //    disabledestlabour[i] = rowsData[i];
        //  }
        //  i++;
        //}
        //}

        if ("<%=focus %>" == "focus") {
            setTimeout("window.focus();", 100);
        }
    })--%>

        function newquery(module)
        {
            $('input:text').each(function () {
                $(this).val("");
            });
            $('textarea').each(function () {
                $(this).val("");
            });
            //loadadditionaldata();
            resetscreen(module,"query");
        }

        function toggleallgrids(mode)
        {
            var show = false;
            if (mode == "edit")
                show = true;
            togglegrid("grdwolabouract",show);
            togglegrid("grdwolabourest",show);

            togglegrid("grdwomatlact",show);
            togglegrid("grdwomatlest",show);

            togglegrid("grdwoserviceact",show);
            togglegrid("grdwoserviceest",show);

            togglegrid("grdwotasksact",show);
            togglegrid("grdwotasksest",show);

            togglegrid("grdwotoolsact",show);
            togglegrid("grdwotoolsest",show);

            togglegrid("grdlabourschedule",show);

            setgridbuttons("grdwolabouract");
            setgridbuttons("grdwolabourest");

            setgridbuttons("grdwomatlact");
            setgridbuttons("grdwomatlest");

            setgridbuttons("grdwotoolsact");
            setgridbuttons("grdwotoolsest");

            setgridbuttons("grdwotasksact");
            setgridbuttons("grdwotasksest");

            setgridbuttons("grdwoserviceact");
            setgridbuttons("grdwoserviceest");


        }

        function togglegrid(id,show)
        {
            var grid = $find(id);
            if (grid!=null)
                grid.set_visible(show);
        }

        function resettabs()
        {
            var mode = getvalue("hidMode");
            var statuscode = getvalue("txtstatuscode");
            if(isDecimal(statuscode))
                statuscode = statuscode *1;
            else
                statuscode = 0;
            //Main
            var link = $("#ucHeader1_tabhlkMain");
            var img = $("#ucHeader1_tabimgMain");
            var lbl = $("#ucHeader1_tablblMain");

            link.attr("href","javascript:void(null)");
            img.attr("src", "../images/tabbutton_down.png");
            lbl.addClass("toptabover");

            //accounts
            link = $("#ucHeader1_tabhlkAccounts");
            img = $("#ucHeader1_tabimgAccounts");
            lbl = $("#ucHeader1_tablblAccounts");

            var wonum = getvalue("hidkeyvalue");
            if (mode == "edit")
            {
                link.removeAttr("disabled");
                link.attr("href","Woaccounts.aspx?wonum=" +  getvalue("hidkeyvalue"));
                //link.mousemove(function () {imageChangeover('ucHeader1_tabimgAccounts','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblAccounts','toptabover') });
                //link.removeAttr("onmouseover");
                //link.removeAttr("onmouseout");
                link.attr("onmouseover","imageChangeover('ucHeader1_tabimgAccounts','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblAccounts','toptabover')");
                link.attr("onmouseout","imageChangeover('ucHeader1_tabimgAccounts','../images/tabbutton.png');classChangeover('ucHeader1_tablblAccounts','toptabout')");
                lbl.removeClass("toptabinactive");
                lbl.addClass("toptabout");
            }
            else
            {
                link.attr("disabled","disabled");
                //link.unbind("mouseover");
                //link.unbind("mouseout");
                link.removeAttr("onmouseover");
                link.removeAttr("onmouseout");

                lbl.addClass("toptabinactive");
                lbl.removeClass("toptabout");
                img.attr("src","../images/tabbutton.png");

            }

            link = $("#ucHeader1_tabhlkHistory");
            img = $("#ucHeader1_tabimgHistory");
            lbl = $("#ucHeader1_tablblHistory");
            if (mode == "edit")
            {
                link.removeAttr("disabled");
                link.attr("href","Wohistory.aspx?wonum=" +  getvalue("hidkeyvalue"));
                link.removeAttr("onmouseover");
                link.removeAttr("onmouseout");
                link.attr("onmouseover","imageChangeover('ucHeader1_tabimgHistory','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblHistory','toptabover')");
                link.attr("onmouseout","imageChangeover('ucHeader1_tabimgHistory','../images/tabbutton.png');classChangeover('ucHeader1_tablblHistory','toptabout')");
                lbl.removeClass("toptabinactive");
                lbl.addClass("toptabout");
            }
            else
            {
                link.attr("disabled","disabled");
                link.removeAttr("onmouseover");
                link.removeAttr("onmouseout");
                lbl.addClass("toptabinactive");
                lbl.removeClass("toptabout");
                img.attr("src","../images/tabbutton.png");
            }

            //Complete
            link = $("#ucHeader1_tabhlkComplete");
            img = $("#ucHeader1_tabimgComplete");
            lbl = $("#ucHeader1_tablblComplete");

            if (mode == "edit" && statuscode >=200)
            {
                link.removeAttr("disabled");
                link.attr("href","Wocomp.aspx?wonum=" +  getvalue("hidkeyvalue"));
                link.removeAttr("onmouseover");
                link.removeAttr("onmouseout");
                link.attr("onmouseover","imageChangeover('ucHeader1_tabimgComplete','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblComplete','toptabover')");
                link.attr("onmouseout","imageChangeover('ucHeader1_tabimgComplete','../images/tabbutton.png');classChangeover('ucHeader1_tablblComplete','toptabout')");
                lbl.removeClass("toptabinactive");
                lbl.addClass("toptabout");
            }
            else
            {
                link.attr("disabled","disabled");
                link.removeAttr("onmouseover");
                link.removeAttr("onmouseout");
                lbl.addClass("toptabinactive");
                lbl.removeClass("toptabout");
                img.attr("src","../images/tabbutton.png");
            }

            //Work Request
            link = $("#ucHeader1_tabhlkWorkRequest");
            img = $("#ucHeader1_tabimgWorkRequest");
            lbl = $("#ucHeader1_tablblWorkRequest");

            if (mode == "edit")
            {
                link.removeAttr("disabled");
                link.attr("href","relatewr.aspx?wonum=" +  getvalue("hidkeyvalue"));
                link.removeAttr("onmouseover");
                link.removeAttr("onmouseout");
                link.attr("onmouseover","imageChangeover('ucHeader1_tabimgWorkRequest','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblWorkRequest','toptabover')");
                link.attr("onmouseout","imageChangeover('ucHeader1_tabimgWorkRequest','../images/tabbutton.png');classChangeover('ucHeader1_tablblWorkRequest','toptabout')");
                lbl.removeClass("toptabinactive");
                lbl.addClass("toptabout");
            }
            else
            {
                link.attr("disabled","disabled");
                link.removeAttr("onmouseover");
                link.removeAttr("onmouseout");
                lbl.addClass("toptabinactive");
                lbl.removeClass("toptabout");
                img.attr("src","../images/tabbutton.png");
            }
        }

        function newmode(tablename,method,keyfield,module)
        {
            $.ajax({
                type: "GET",
                url: "../InternalServices/ServiceGeneral.svc/NewRecord",
                data: { "tablename":tablename,"method": method,"keyfield":keyfield },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, status) {
                    var result = data.d;
                    var valid = true;
                    try
                    {
                        var json = JSON.parse(result);
                    }
                    catch( err)
                    {
                        valid= false;
                        alert(result);
                    }
                    if (valid)
                    {
                        setvalue("hiddata",data.d);
                        setvalue("hidkeyvalue","");
                        populatescreen(data.d);
                        resetscreen(module,"new");
                    }

                },
                error: OnGetMemberError
            });

        }

        function resettoolbar()
        {
            //return;
            var toolbar = $find("ucHeader1_toolbar");
            var items = toolbar.get_allItems();
            for (var i=0;i<=items.length;i++)
            {
                var item = items[i];
                settoolbaritem(item);
            }

            inittimertoolitems(getTimerStatus());
            updatePanelBar();

            $(".hidden").hide();
            $(".regular").each(function () {
                var dad = $(this).parent();
                if (!dad.hasClass("hidden"))
                    dad.show();
            });
        }

        function settoolbaritem(item)
        {

            var commandname = "";
            try // if it's drop down , just return
            {
                commandname = item.get_commandName();
            }
            catch (err)
            {
                return;
            }

            var mode = getvalue("hidMode");
            var show = false;
            var statuscode = getvalue("txtstatuscode");
            if (! isDecimal(statuscode))
                statuscode = 0;
            else
                statuscode = statuscode * 1;

            switch (commandname)
            {
                case "search":
                case "batchprint":
                case "print list":
                case "linkeddoc":
                case "picture":
                case "calendar":
                case "request":
                    show = true;
                    break;
                case "lookup":
                    if (mode == "query")
                        show = true;
                    break;
                case "savequery":
                    show = true;
                    break;
                case "newwo":
                case "autowo":
                    if ("<%=m_rights["urAddNew"]%>" == "1" )
                        show = true;
                    break;
                case "save":
                    if ((mode == "edit" && "<%=m_rights["urEdit"]%>" == "1" && divallowedit() && statuscode>0 && statuscode<400)
                        || ((mode == "new" || mode == "duplicate") && "<%=m_rights["urAddNew"]%>" == "1"))
                        show = true;
                    break;
                case "status":
                    if ("<%=m_rights["urAppr"]%>" == "1" && mode == "edit")
                        show = true;
                case "duplicate":
                    if ("<%=m_rights["urAddNew"]%>" == "1" && mode == "edit" )
                        show = true;
                    break;
                case "print":
                case "email":
                case "updateprocedure":
                case "map":
                case "modifyequipment":
                    if (mode == "edit")
                        show = true;
                    break;
                case "schedule":
                    if ("<%=Scheduler.checkSchedulerAccess("woScheduler")%>" == "true")
                        show = true;
                    break;
                case "timerinfo":
                    if (mode == "edit" && statuscode >=200)
                        show = true;
                    break;
                case "timerstart":
                case "timerresume":
                case "timerpause":
                case "timerstop":
                    if (mode == "edit" && statuscode >=200 && statuscode <300 && "<%=m_rights["urActualLabour"]%>" == "1")
                        show=true;
                    break;
            }

            var d = $(item.get_element());
            if (show)
                d.removeClass("hidden");
            else
                d.addClass("hidden");
        }

        function divallowedit()
        {
            var divok = false;
            var div = getdatafield("division");
            var editablediv = "<%=Session["EditableDivision"].ToString()%>";
            if (div == "")
                divok = true;
            else
            {
                var divarray = editablediv.split(",");
                div = "'" + div + "'";
                if ($.inArray(div,divarray)>=0)
                    divok = true;
            }
            return divok;
        }

        function resetscreen(module, mode)
        {
            setmode(mode);
            //resettoolbar();
            toggleallgrids(mode);
            resetfieldstatus();
            resettabs();

            loadlinkdocument(module);
            setvalue("hidDataChanged","0");

            if (module.toLowerCase() == "workorder")
            {
                setvalue("hidTargetStatusCode",getvalue("txtstatuscode"));
                //setvalue("hidworate",getvalue("txtdivision"));

                object = document.getElementById("rblchargeback");
                if (object != null)
                {
                    if (mode == "query")
                    {
                        $('input[name="rblchargeback"][value=""]').parent().show();
                        $('input[name="rblchargeback"][value=""]').prop('checked', true);
                    }
                    else
                    {
                        $('input[name="rblchargeback"][value=""]').parent().hide();
                    }
                    if (mode == "new")
                    {

                        $('input[name="rblchargeback"][value="0"]').prop('checked', true);
                    }
                }
                loadadditionaldata();

            }
            else
                resettoolbar();
        }

        function  loadlinkdocument(module){
            $.ajax({
                type: "GET",
                url: "../InternalServices/ServiceLinks.svc/GetLinkedDoc",
                data: { "module": module, "keyvalue": getvalue("hidkeyvalue") },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, status) {
                    reloadLinkDocuments(data.d);
                },
                error: OnGetMemberError
            });
        }

        function loadadditionaldata()
        {
            if (getvalue("hidMode") != "edit")
            {
                setvalue("hidMobile","1");
                setvalue("hidworate","0");
                setvalue("hidwrcount","0");
                setvalue("ucHeader1_hidtimerstatus","");
                resettoolbar();
            }
            else
            {
                $.ajax({
                    type: "GET",
                    url: "../InternalServices/ServiceWO.svc/GetAdditionalData",
                    data: { "wonum": getvalue("hidkeyvalue") },
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, status) {
                        var result = data.d;
                        var valid = true;
                        var json;
                        try
                        {
                            json = JSON.parse(result);
                        }
                        catch( err)
                        {
                            valid= false;
                            alert(result);
                        }
                        if (valid)
                        {
                            setvalue("hidMobile",json["mobile"]);
                            setvalue("hidworate",json["worate"]);
                            setvalue("hidwrcount",json["wrcount"]);
                            setvalue("ucHeader1_hidtimerstatus",json["timerstatus"]);

                            resettoolbar();

                            var statuscode = getvalue("txtstatuscode");
                            if (!isDecimal(statuscode))
                                statuscode = 1;
                            else
                                statuscode = statuscode * 1;
                            var openwrcount = getvalue("hidwrcount");
                            if (!isDecimal(openwrcount))
                                openwrcount = 0;
                            else
                                openwrcount = openwrcount*1;

                            if (statuscode <0 && openwrcount>0)
                            {
                                showwrlist(getvalue("hidkeyvalue"));
                            }

                        }


                    },
                    error: OnGetMemberError
                });
            }

        }

        function resourceclick(sender, args)
        {
            var grid = sender;
            var id = grid.get_id();
            var estimate = "1";
            if (id.substr(id.length-3) == "act")
                estimate = "0";
            var counter = args.getDataKeyValue("Counter");
            var batchnum = args.getDataKeyValue("BatchNum");
            if (id.indexOf("labour")>=0)
                editLabour(counter,estimate);
            if (id.indexOf("tool")>=0)
                editTools(counter,estimate);
            if (id.indexOf("task")>=0)
                editTask(counter,estimate);
            if (id.indexOf("matl")>=0)
            {
                if (estimate == "1")
                    editMaterial(counter,estimate);
                else
                    editActMaterial(batchnum);
            }
            if (id.indexOf("service")>=0)
            {
                if (estimate == "1")
                    editService(counter,estimate);
                else
                    editActService(batchnum);
            }
        }

        function setgridbuttons(gridid)
        {
            var grid = $find(gridid);
            if (grid == null)
                return;
            var statuscode = getvalue("txtstatuscode");
            if (!isDecimal(statuscode))
                statuscode = -1;
            else
                statuscode = statuscode *1;
            var mode = getvalue("hidMode");
            var autoapprove = "<%=Application["AutoApproveWo"].ToString().ToLower()%>";
            var jsonrights = JSON.parse(getvalue("hiduserrights"));
            var editablediv = "<%=Session["EditableDivision"].ToString()%>";
            var allowedit = jsonrights["urEDIT"];
            var div = getdatafield("Division");
            var estimate = 1;
            if (gridid.substr(gridid.length-3) == "act")
                estimate = 0;


            var shownew = false;
            var showcopy = false;
            var statusok = false;
            var copystatusok = false;
            var modeok = false;
            var rightsok = false;
            var divok = false;

            if (estimate == 1)
            {
                if ((statuscode >=1 && statuscode <100) || (statuscode>=200 && statuscode<400 && autoapprove == "yes"))
                    statusok = true;
                if (statuscode>=200 && statuscode<400)
                    copystatusok = true;
            }
            else
            {
                if (statuscode>=200 && statuscode<400)
                    statusok = true;
            }

            if (mode == "edit")
                modeok = true;

            if (allowedit == "1")
                rightsok = true;

            if (div == "")
                divok = true;
            else
            {
                var divarray = editablediv.split(",");
                div = "'" + div + "'";
                if ($.inArray(div,divarray)>=0)
                    divok = true;
            }
            shownew = statusok && modeok && rightsok && divok;
            showcopy = copystatusok && modeok && rightsok && divok;

            control = $telerik.findElement(grid.get_element(), "addFormButton");
            togglegridheadercontrol(control,shownew, false);
            control = $telerik.findElement(grid.get_element(), "batchaddButton");
            togglegridheadercontrol(control,shownew, false);
            control = $telerik.findElement(grid.get_element(), "copyactualButton");
            togglegridheadercontrol(control,showcopy, false);
            //control = $telerik.findElement(grid.get_element(), "rdpDate");
            //togglegridheadercontrol(control,showcopy);
            control = $telerik.findControl(grid.get_element(), "rdpDate");
            togglegridheadercontrol(control,showcopy, true);

            // hide select column
            //if (estimate == 1)
            //{
            //    if (statuscode>=200 && statuscode <400 && rightsok && divok)
            //        grid.get_masterTableView().showColumn(0);
            //    else
            //        grid.get_masterTableView().hideColumn(0);
            //}
            //else
            //{
            //    grid.get_masterTableView().hideColumn(0);
            //}

            grid.repaint();


        }

    function togglegridheadercontrol(control, show, istelerikcontrol)
    {
        if (control!=null)
            if (show)
            {
                if (!istelerikcontrol)
                {
                    control.style["visibility"] = "visible";
                    control.style["display"] = "inline";
                }
                else
                {
                    control.set_visible(true);
                }
            }
            else
            {
                if (!istelerikcontrol)
                {
                    control.style["visibility"] = "hidden";
                    control.style["display"] = "none";
                }
                else
                {
                    control.set_visible(false);
                }
            }
    }

    function getdatafield(columnname)
    {
        var jsondata;
        try
        {
            jsondata = JSON.parse(getvalue("hiddata"));
        }
        catch (err)
        {
            return "";
        }
        if (jsondata[columnname] != null)
            return jsondata[columnname];
        else
            return "";
    }

    function addtorecentlist(keyvalue,module)
    {
        $.ajax({
            type: "GET",
            url: "../InternalServices/ServiceGeneral.svc/AddToRecentList",
            data: { "keyvalue": keyvalue,"module": module },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, status) {
                RefreshPanel();
            },
        });
    }

    function batchaddtask(ordertype, estimate)
    {
        var key = getdatafield("wonum");

        return addfromtasklib(ordertype,key,estimate);
    }

    function rowselecting(sender,args)
    {
        var actual = args.getDataKeyValue("Actual");
        var store = "";
        var id = sender.get_id();
        if (id.indexOf("matl") >=0)
        {
            store = args.getDataKeyValue("Store");
        }

        if (actual == 1 || store != "")
            args.set_cancel(true);
    }

    function rowbound(sender, args)
    {
        var actual = args.get_dataItem()["Actual"];
        var cell = args.get_item().get_cell("SelectColumn");

        var store = "";
        var id = sender.get_id();

        if (id.indexOf("matlest") >=0)
        {
            store = args.get_dataItem()["Store"];
            if (store == null)
                store = "";
        }



        if (actual == 1 || store!= "")
        {

            cell.bgColor = "gray";
        }
        else
        {
            cell.bgColor = "white";
        }
    }

    function loadduplicaterecord(tablename,keyfield, keyvalue, module)
    {
        var oldkey = getvalue("hidkeyvalue");
        $.ajax({
            type: "GET",
            url: "../InternalServices/ServiceWO.svc/GetDuplicateWO",
            data: { "wonum":oldkey },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, status) {
                var result = data.d;
                var valid = true;
                try
                {
                    var json = JSON.parse(result);
                }
                catch( err)
                {
                    valid= false;
                    alert(result);
                }
                if (valid)
                {
                    setvalue("hiddata",data.d);
                    setvalue("hidkeyvalue",keyvalue);
                    populatescreen(data.d, module);
                    resetscreen(module,"duplicate");
                }

            },
            error: OnGetMemberError
        });
    }

    function duplicatemode()
    {
        setmode("duplicate");
        loadduplicaterecord("WorkOrder","wonum",getvalue("hidkeyvalue"),"workorder");
    }

    function reloadLinkDocuments(data) {
        var combo = $find("ucHeader1_cbblinks");
        combo.clearItems();
        var jsonData = JSON.parse(data);
        for ( var i = 0; i < jsonData.length; i ++ )
        {
            var comboItem = new Telerik.Web.UI.RadComboBoxItem();
            comboItem.set_text(jsonData[i]["LinkTitle"]);
            combo.trackChanges();
            combo.get_items().add(comboItem);
            var comboItemElement = comboItem.get_element(),
                json = jsonData[i],
                linkUrl = json["LinkURL"],
                linkType = json["LinkType"],
                subFolder = json["SubFolder"] == null? "": json["SubFolder"];
            if (linkType == "http")
                if (linkUrl.indexOf("ftp://") < 0 && linkUrl.indexOf("http://") < 0 && linkUrl.indexOf("https://") < 0 && linkUrl.indexOf("news://") < 0)
                    linkUrl = "http://" + linkUrl;
            linkUrl = linkType + "~" + linkUrl;
            $(comboItemElement).attr({"linkUrl": linkUrl, "subFolder": subFolder});
            $(comboItemElement).click(function(){
                var linkUrl = $(this).attr("linkUrl"),
                    subFolder = $(this).attr("subFolder");
                openlink(linkUrl,subFolder);
            });
            combo.commitChanges();
        }
    }

    $(window).load(function() {
    //$(document).ready(function () {
    //function pageLoad()
    //{
        setpanelheight(1);
       initToolbar();
        var mode = "<%=querymode%>";
        var keyvalue = "<%=m_wonum%>";
        var numbering = "<%=numbering%>";
        if (mode == "query")
            newquery("workorder");
        else if (mode == "edit")
        {
            if (keyvalue == "")
                newquery("workorder");
            else
                loadrecord("WorkOrder","WoNum",keyvalue,"workorder");
        }
        else  //new, autonew, duplicate
        {
            if (keyvalue == "")
            {
                if (numbering == "auto")
                    newmode("Workorder","auto","WoNum","workorder");
                else
                    newmode("Workorder","manual","WoNum","workorder");
            }
            else
            {
                setvalue("hidkeyvalue",keyvalue);
                duplicatemode();
            }
        }

        //if (getvalue("hidkeyvalue") != "")
        //    loadrecord("WorkOrder","WoNum",getvalue("hidkeyvalue"))
        //else
        //{
        //    newquery("workorder");
        //}

        if ("<%=focus %>" == "focus") {
            setTimeout("window.focus();", 100);
        }
    });


</script>
</html>
