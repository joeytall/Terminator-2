<%@ Page Language="C#" AutoEventWireup="true" CodeFile="poaccounts.aspx.cs" Inherits="purchase_poaccounts" %>

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
        <telerik:AjaxSetting AjaxControlID="grdpoaccounts">
          <UpdatedControls>
            <telerik:AjaxUpdatedControl ControlID="grdpoaccounts" />
            <telerik:AjaxUpdatedControl ControlID="hidAccountTotal" />
          </UpdatedControls>
        </telerik:AjaxSetting>
      </AjaxSettings>
      <ClientEvents OnRequestStart="RequestStart" OnResponseEnd="ResponseEnd" />
    </telerik:RadAjaxManager>
    <asp:SqlDataSource ID="Accounts_SqlDataSource" runat="server" ProviderName="System.Data.OleDb">
    </asp:SqlDataSource>
    <asp:HiddenField ID="hidAccounts" runat="server" />
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
      ShowSummary="False" ValidationGroup="poaccounts"/>
  </asp:Panel>
  <asp:Literal ID="litFrameScript" runat="server" />
  <asp:HiddenField ID="hidAccountTotal" runat="server" />
  <asp:HiddenField ID="hidTotal" runat="server" />
  <asp:HiddenField ID="hidMode" Value="edit" runat="server" />
  </form>
</body>
<script type="text/javascript" src="../js/jquery-ui-1.8.21.custom.min.js"></script>
<script type="text/javascript" src="../jscript/codes.js"></script>
<script type="text/javascript" src="../jscript/codevalidate.js"></script>
<script type="text/javascript" src="../jscript/setvalue.js"></script>
<script type="text/javascript" src="../jscript/callbackfunction.js"></script>
<script type="text/javascript" src="../jscript/radwindow.js"></script>
<script type="text/javascript" src="../jscript/autocomplete.js"></script>
<script type="text/javascript" src="../jscript/stopdoubleclick.js"></script>
<script type="text/javascript">
    $(window).load(function () {
        setpanelheight(1);
        initToolbar();
    });

  function CostChanged(costamountfield,totalfield,accounttotalfield,percentagefield,oldpercentagefield,accountcostfield,keeppercentfield)
  {
    var costamount = getvalue(costamountfield);
    var total = getvalue(totalfield);
    var accounttotal = getvalue(accounttotalfield);
    var oldamount = getvalue(accountcostfield);
    var percentage = getvalue(percentagefield);
    var oldpercentage = getvalue(oldpercentagefield);
    if (!isDecimal(costamount + ""))
      costamount = 0;
    if (!isDecimal(total + ""))
      total = 0;
    if (!isDecimal(accounttotal))
      accounttotal = 0;
    if (!isDecimal(oldamount))
      oldamount = 0;
    if (!isDecimal(percentage))
      percentage = 0;
    else
      percentage = percentage / 100;
    if (!isDecimal(oldpercentage))
      oldpercentage = 0;
    if (accounttotal * 1 + costamount * 1 - oldamount * 1 <= total * 1) {
      if (total != 0)
        setvalue(percentagefield, 100*costamount / total);
      else
        setvalue(percentagefield, 0);
      setvalue(keeppercentfield, 0);
    }
    else {
      setvalue(costamountfield,oldamount);
      setvalue(percentagefield,oldpercentage*100);
      //alert("Total cost account amount cannot exceed total cost of workorder.");
      alert('<%= m_msg["T6"]%>');
      delayedsetfocus(costamountfield);
    }
  }

  function PercentChanged(costamountfield, totalfield, accounttotalfield, percentagefield, oldpercentagefield, accountcostfield, keeppercentfield) {
      var costamount = getvalue(costamountfield);
      var total = getvalue(totalfield);
      var accounttotal = getvalue(accounttotalfield);
      var oldamount = getvalue(accountcostfield);
      var percentage = getvalue(percentagefield);
      var oldpercentage = getvalue(oldpercentagefield);

      if (!isDecimal(total + ""))
          total = 0;
      if (!isDecimal(accounttotal))
          accounttotal = 0;
      if (!isDecimal(oldamount))
          oldamount = 0;
      if (!isDecimal(percentage) || percentage > 100 || percentage < 0) {
          percentage = 0;
          alert("Percentage must be a number between 0 to 100");
          setvalue(costamountfield, oldamount);
          setvalue(percentagefield, oldpercentage * 100);
          delayedsetfocus(percentagefield);
          return;
      }
      else
          percentage = percentage / 100;
      if (!isDecimal(oldpercentage))
          oldpercentage = 0;
      costamount = total * percentage;

      if (total == 0) {
          var percentageList = GetPercentageValue();
          var sum = 0;
          var addNewMode = true;
          for (var i = 0; i < percentageList.length; i++) {
              var numberPercent = percentageList[i];
              if (percentageList[i][0] == "<") {
                  numberPercent = percentage * 100;
                  addNewMode = false;
              }
              else {
                  numberPercent = parseFloat(percentageList[i]);
              }
              sum += numberPercent;
          }
          if (addNewMode == true) {
              sum += percentage * 100;
          }
          if (sum <= 100) {
              setvalue(costamountfield, roundup(costamount, 2));
              setvalue(keeppercentfield, 1);
          }
          else {
              alert("Percentage sum must equal or less than 100!");
              setvalue(costamountfield, oldamount);
              setvalue(percentagefield, oldpercentage * 100);
              delayedsetfocus(percentagefield);
          }
          return;
      }
      else if (accounttotal * 1 + costamount * 1 - oldamount * 1 <= total * 1) {
          setvalue(costamountfield, roundup(costamount, 2));
          setvalue(keeppercentfield, 1);
      }
      else {
          setvalue(costamountfield, oldamount);
          setvalue(percentagefield, oldpercentage * 100);
          alert('<%= m_msg["T6"]%>');
          delayedsetfocus(percentagefield);
      }

      function GetPercentageValue() {
          var table = document.getElementById('grdpoaccounts_ctl00');
          var headers = $(".rgHeader");
          for (var i = 0; i < headers.length; i++) {
              if (headers[i].innerHTML == "Cost Percentage")
                  break;
          }
          var percentageList = []
          if (table.rows[1].cells.length == 1) {
              percentageList.push("<");
          }
          else {
              for (var r = 1, n = table.rows.length; r < n; r++) {
                  percentageList.push(table.rows[r].cells[i].innerHTML);
              }
          }
          return percentageList;
      }
  }
</script>
</html>
