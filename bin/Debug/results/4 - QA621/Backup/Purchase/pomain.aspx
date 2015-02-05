<%@ Page Language="C#" AutoEventWireup="true" CodeFile="pomain.aspx.cs" Inherits="purchase_purchasemain"
  MaintainScrollPositionOnPostback="true" %>

<%@ Register TagPrefix="uc1" TagName="PoHeader" Src="poheader.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
  <title>PO</title>
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
  <uc1:PoHeader ID="ucHeader1" runat="server" />
  <asp:Panel ID="MainControlsPanel" runat="server" CssClass="MainControlsPanelClass">
    <telerik:RadScriptManager ID="RadScriptManager" runat="server">
    </telerik:RadScriptManager>
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false">
    </telerik:RadWindowManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
      <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="grdpoline">
          <UpdatedControls>
            <telerik:AjaxUpdatedControl ControlID="grdpoline" />
          </UpdatedControls>
        </telerik:AjaxSetting>
      </AjaxSettings>
      <ClientEvents OnRequestStart="RequestStart" OnResponseEnd="ResponseEnd" />
    </telerik:RadAjaxManager>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
      ShowSummary="False" />
    <asp:SqlDataSource ID="poItems_SqlDataSource" runat="server" ProviderName="System.Data.OleDb">
    </asp:SqlDataSource>
  </asp:Panel>
  <asp:HiddenField ID="hidMode" runat="server" />
  <asp:HiddenField ID="HidOldPONum" runat="server" />
  <asp:HiddenField ID="HidDivision" runat="server" />
  <input type="hidden" id="hidDataChanged" value="0" />
  <asp:HiddenField ID="hidNewDivision" Value="" runat="server" />
  <asp:HiddenField ID="hidNewParentID" Value="" runat="server" />
  <asp:HiddenField ID="hidNewParentDesc" Value="" runat="server" />
  <asp:HiddenField ID="hidNewStatus" Value="" runat="server" />
  <asp:HiddenField ID="hidModDate" Value="" runat="server" />
  <asp:HiddenField ID="hidUpdateOpen" Value="" runat="server" />
  <asp:HiddenField ID="hidUpdateHistory" Value="" runat="server" />
  <input type="hidden" id="hidNoPromt" value="0" />
  <input type="hidden" id="hidempid" value="<%=Session["Login"].ToString() %>" />
  <asp:HiddenField ID="hidTargetStatusCode" Value="<%=statuscode %>" runat="server" />
  <asp:HiddenField ID="hidStatusComments" Value="" runat="server" />
  <asp:HiddenField ID="hidTargetStatus" Value="" runat="server" />
  <asp:HiddenField ID="hidStatusApplyTo" Value="" runat="server" />
  <asp:HiddenField ID="hidKeyName" value="txtponum" runat="server" />
  <asp:HiddenField ID="hidToDo" Value="" runat="server" />
  <iframe id="ifValidate" src="" frameborder="0" scrolling="no" style="visibility: hidden;
    display: none;"></iframe>
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
      //"Location has been changed. Do you want to save the changes? Click Yes to save location and modify location.\nClick No to abort. ");
      //            alert(resp);
    }
    if (resp) {
      if (Page_ClientValidate('')) {
        noPrompt();
        __doPostBack("ModifyPO", "");
      }
    }
  }

  function EditPOItems(counter, ponum, vendor, postatus) {
    var URL = "polinedetail.aspx?counter=" + counter + "&ponum=" + ponum + "&vendor=" + vendor + "&postatus=" + postatus;
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

  function EditPOService(counter, ponum, vendor, postatus) {
    var URL = "polineservice.aspx?counter=" + counter + "&ponum=" + ponum + "&vendor=" + vendor + "&postatus=" + postatus;
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

  function refreshGrid() {
    var masterTable = $find('grdpoline').get_masterTableView();
    masterTable.rebind();
    refreshPoTotalAmount();
  }

  function saveStatus() {
    var obj = document.getElementById("txtponum");
    var changed = document.getElementById("hidDataChanged");
    //var changed = document.getElementById("hidChanged");
    if (changed == null) {
      //changed = 1;
      changed = 0;
    }
    else {
      //changed = 0;
      changed = document.getElementById("hidDataChanged").value * 1;
    }

    var resp = 1;
    if (changed == 1) {
      resp = window.confirm("Purchase order has been changed. Do you want to save the changes? Click Yes to save purchase order and change status\nClick No to abort. ");
    }
    if (resp) {
      if (Page_ClientValidate('')) {
        __doPostBack("saveStatus", "");
      }
      else {
        //{
        var targetStatusCodeField = document.getElementById("hidTargetStatusCode");
        if (targetStatusCodeField != null) {
          var statuscodefield = document.getElementById("txtstatuscode");
          var oldstatuscode = 1;
          if (statuscodefield != null)
            oldstatuscode = statuscodefield.value;

          targetStatusCodeField.value = oldstatuscode;
        }
        //}
      }
    }
  }

  function refreshPoTotalAmount() {
    $(document).ready(function () {
      $.ajax({
        type: "GET",
        url: "../InternalServices/ServicePO.svc/getPOTotalAmount",
        data: { "ponum": $('#txtponum').val() },
        dataType: "json",
        success: function (data) {
          var obj = jQuery.parseJSON(data.d);
          $('#txtpototal').val(obj.total);
          $('#txtsubtotal').val(obj.subtotal);
          $('#txttax1total').val(obj.tax1);
          $('#txttax2total').val(obj.tax2);
        }
      });
    });
  }

  function vendorchanged() {
      if (getvalue("hidMode") != "query")
      {
          clearname();
          var vendor = getvalue("txtvendor");
          if (vendor != "")
          {
              $.ajax({
                  type: "GET",
                  url: "../InternalServices/ServicePO.svc/getterm",
                  data: { "vendor": vendor },
                  dataType: "json",
                  success: function (data) {
                      obj = data.d;
                      $('#txtterms').val(obj);
                  }
              });
          }
          generalValidateField('txtvendorcurrency');
          
      }
  }

  function clearname()
  {
  }
</script>
<script type="text/javascript">
    <asp:Literal id="litScript1" runat="server" />
</script>
</html>
