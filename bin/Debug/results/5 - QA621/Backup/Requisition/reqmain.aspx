<%@ Page Language="C#" AutoEventWireup="true" CodeFile="reqmain.aspx.cs" Inherits="requisition_reqmain"
  MaintainScrollPositionOnPostback="true" %>

<%@ Register TagPrefix="uc1" TagName="ReqHeader" Src="reqheader.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
  <title>Requisition</title>
  <meta http-equiv="content-type" content="text/html; charset=utf-8" />
  <link type="text/css" href="../Styles/Header.css" rel="stylesheet" />
  <link type="text/css" href="../Styles/Azzier.css" rel="stylesheet" />
  <link type="text/css" href="../Styles/Customer.css" rel="stylesheet" />
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
  <uc1:ReqHeader ID="ucHeader1" runat="server" />
  <asp:Panel ID="MainControlsPanel" runat="server" CssClass="MainControlsPanelClass">
    <telerik:RadScriptManager ID="RadScriptManager" runat="server">
    </telerik:RadScriptManager>
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false">
    </telerik:RadWindowManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
      <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="grdreqline">
          <UpdatedControls>
            <telerik:AjaxUpdatedControl ControlID="grdreqline" />
          </UpdatedControls>
        </telerik:AjaxSetting>
      </AjaxSettings>
      <ClientEvents OnRequestStart="RequestStart" OnResponseEnd="ResponseEnd" />
    </telerik:RadAjaxManager>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
      ShowSummary="False" />
  </asp:Panel>
  <asp:HiddenField ID="hidMode" runat="server" />
  <asp:HiddenField ID="HidOldReqNum" runat="server" />
  <asp:HiddenField ID="HidDivision" runat="server" />
  <input type="hidden" id="hidDataChanged" value="0" />
  <asp:HiddenField ID="hidNewDivision" Value="" runat="server" />
  <asp:HiddenField ID="hidNewParentID" Value="" runat="server" />
  <asp:HiddenField ID="hidNewParentDesc" Value="" runat="server" />
  <asp:HiddenField ID="hidNewStatus" Value="" runat="server" />
  <asp:HiddenField ID="hidModDate" Value="" runat="server" />
  <asp:HiddenField ID="hidUpdateOpen" Value="" runat="server" />
  <asp:HiddenField ID="hidUpdateHistory" Value="" runat="server" />
  <asp:HiddenField ID="hidKeyName" value="txtreqnum" runat="server" />
  <input type="hidden" id="hidNoPromt" value="0" />
  <input type="hidden" id="hidempid" value="<%=Session["Login"].ToString() %>" />
  <asp:HiddenField ID="hidTargetStatusCode" Value="<%=statuscode %>" runat="server" />
  <asp:HiddenField ID="hidStatusComments" Value="" runat="server" />
  <asp:HiddenField ID="hidTargetStatus" Value="" runat="server" />
  <asp:HiddenField ID="hidStatusApplyTo" Value="" runat="server" />
  <asp:HiddenField ID="hidToDo" Value="" runat="server" />
  
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

  function ModifyPO() {
    var changed = document.getElementById("hidDataChanged");
    if (changed == null)
      changed = 0;
    else {
      if (changed.value == "0")
        changed = 0;
      else
        changed = 1;
    }
    var resp = 1; ;
    if (changed == 1) {
      resp = window.confirm('<%=m_msg["T2"] %>');
    }
    if (resp) {
      if (Page_ClientValidate('')) {
        noPrompt();
        __doPostBack("ModifyPO", "");
      }
    }
  }

  function EditReqItems(counter, reqnum, vendor, reqstatus) {
    var URL = "reqitem.aspx?counter=" + counter + "&reqnum=" + reqnum + "&vendor=" + vendor + "&reqstatus=" + reqstatus;
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

  function EditReqService(counter, reqnum, vendor, reqstatus) {
    var URL = "reqservice.aspx?counter=" + counter + "&reqnum=" + reqnum + "&vendor=" + vendor + "&reqstatus=" + reqstatus;
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

  function EditLine(index, reqnum, vendor, reqstatus)
  {
      var counter = "";
      var isservice = "";
      var grid = $find("grdreqline");
      var MasterTable = grid.get_masterTableView();
      var row = MasterTable.get_dataItems()[index];
      counter = row.getDataKeyValue("Counter");
      isservice = row.getDataKeyValue("IsService") + "";

      if (counter != "")
      {
          if (isservice == "0")
              return EditReqItems(counter,reqnum,vendor,reqstatus);
          else
              return EditReqService(counter,reqnum,vendor,reqstatus);
      }
      else
      {
          alert("invalid counter.");
          return false;
      }

  }


  function refreshGrid() {
    var masterTable = $find('grdreqline').get_masterTableView();
    masterTable.rebind();
    refreshReqTotalAmount();
  }

  function saveStatus() {
    var obj = document.getElementById("txtreqnum");
    var changed = document.getElementById("hidDataChanged");
    if (changed == null) {
      changed = 0;
    }
    else {
      changed = document.getElementById("hidDataChanged").value * 1;
    }

    var resp = 1;
    if (changed == 1) {
      resp = window.confirm("Requisition has been changed. Do you want to save the changes? Click Yes to save requisition and change status\nClick No to abort. ");
    }
    if (resp) {
      if (Page_ClientValidate('')) {
        __doPostBack("saveStatus", "");
      }
      else {
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

  function refreshReqTotalAmount() {
    $(document).ready(function () {
      $.ajax({
        type: "GET",
        url: "../InternalServices/ServiceReq.svc/getReqTotalAmount",
        data: { "reqnum": $('#txtreqnum').val() },
        dataType: "json",
        success: function (data) {
          var obj = jQuery.parseJSON(data.d);
            //$('#txttotalcost').val(obj.total);
          setvalue("txttotalcost",obj.total);
        }
      });
    });
  }
</script>
<script type="text/javascript">
    <asp:Literal id="litScript1" runat="server" />
</script>
</html>
