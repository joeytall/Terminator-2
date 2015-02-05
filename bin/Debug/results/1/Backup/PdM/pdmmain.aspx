<%@ Page Language="C#" AutoEventWireup="true" CodeFile="pdmmain.aspx.cs" Inherits="pdm_pdmmain"
    MaintainScrollPositionOnPostback="true" %>

<%@ Register TagPrefix="uc1" TagName="PDMHeader" Src="pdmheader.ascx" %>
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
    <script type="text/javascript" src="../Jscript/CallBackFunction.js"></script>
    <script type="text/javascript" src="../Jscript/RadWindow.js"></script>
    <script type="text/javascript" src="../Jscript/Stopdoubleclick.js"></script>
    <script type="text/javascript" src="../Jscript/RadControls.js"></script>
    <script type="text/javascript">

      function duplicate() {
          var pmnum = $('#txtpdmnum').val();
          window.location.href = 'pdmmain.aspx?mode=duplicate&pdmnum=' + pdmnum;
      } 

      function clearnonduplicatefields() {
          $('input:radio[name=rdbinactive][value=0]').click();
      }

      

    </script>
    <uc1:PDMHeader ID="ucHeader1" runat="server" />
    <asp:Panel ID="MainControlsPanel" runat="server"  CssClass="MainControlsPanelClass">
        <telerik:RadScriptManager ID="RadScriptManager" runat="server">
        </telerik:RadScriptManager>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false">
        </telerik:RadWindowManager>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            <AjaxSettings>
            </AjaxSettings>
            <ClientEvents OnRequestStart="RequestStart" OnResponseEnd="ResponseEnd" />
        </telerik:RadAjaxManager>

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
    </asp:Panel>
    <asp:HiddenField ID="hidMode" runat="server" />
    <asp:HiddenField ID="HidOldPDMNum" runat="server" /> 
    <asp:HiddenField ID="hidprocnum" runat="server" />   
    <asp:HiddenField ID="HidDivision" runat="server" />
        <asp:HiddenField ID="hidKeyName" Value="txtpdmnum" runat="server" />
        <input type="hidden" id="hidDataChanged" value="0" />
        <input type="hidden" id="hidNoPromt" value="0" />
    <input type="hidden" id="hidresourcesource" value="PM" />
    <input type="hidden" id="hidempid" value="<%=Session["Login"].ToString() %>" />
    <input type="hidden" id="HidLabSelType" value="E" />
    <asp:HiddenField ID="HidMobile" runat="server" />
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

    function measurementFocus() {
        var obj = document.getElementById("txtequipment");
        var x = trim(obj.value);
        if (x.length<1)
        obj.focus();
    }

    function measurementlookup() {
        var x = trim($("#txtequipment").val());
        if (x.length <1) {
            alert('Cannot select measurement if equipment is not selected.');
        }
        else {
            generalLookup("txtmeasurementname");
        }
    }
    function clearmeasurementfields() {
        var txtmeasurementname = document.getElementById("txtmeasurementname");
        var txtmeasunit = document.getElementById("txtmeasunit");
        if (txtmeasurementname != null) {
            txtmeasurementname.value = "";
        }
        if (txtmeasunit != null) {
            txtmeasurementname.value = "";
        }
    }

</script>
</html>

