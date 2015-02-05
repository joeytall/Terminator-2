<%@ Page Language="C#" AutoEventWireup="true" CodeFile="locationmain.aspx.cs" Inherits="location_locationmain"
    MaintainScrollPositionOnPostback="true" %>

<%@ Register TagPrefix="uc1" TagName="LocHeader" Src="locationheader.ascx" %>
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

//        var FormSubmitted = false;
/*
        var labourtax1 = 0;
        var labourtax2 = 0;
        var sevicetax1 = 0.05;
        var sevicetax2 = 0.07;
        */
        function duplicate() {
            $(document).ready(function () {
                var location = $('#txtlocation').val();
                window.location.href = 'locationmain.aspx?mode=duplicate&location=' + location;
            });
        }

        function clearoldlocationval() {
            $(document).ready(function () {
                $('#txtlocation').val('');

            });
        }   
    </script>
    <uc1:LocHeader ID="ucHeader1" runat="server" />
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
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </asp:Panel>
    <asp:HiddenField ID="hidMode" runat="server" />
    <asp:HiddenField ID="HidOldLocation" runat="server" />    
    <asp:HiddenField ID="HidDivision" runat="server" />
    <input type="hidden" id="hidDataChanged" value="0" />
    <asp:HiddenField ID="hidNewDivision" Value="" runat="server" />
    <asp:HiddenField ID="hidNewParentID" Value="" runat="server" />
    <asp:HiddenField ID="hidNewParentDesc" Value="" runat="server" />
    <asp:HiddenField ID="hidNewStatus" Value="" runat="server" />
    <asp:HiddenField ID="hidModDate" Value="" runat="server" />
    <asp:HiddenField ID="hidUpdateOpen" Value="" runat="server" />
    <asp:HiddenField ID="hidUpdateHistory" Value="" runat="server" />
    <asp:HiddenField ID="hidfirstname" Value="" runat="server" />
    <asp:HiddenField ID="hidlastname" Value="" runat="server" />
    <asp:HiddenField ID="HidFilename" runat="server" Value="Location/Locationmain.aspx" />
        <asp:HiddenField ID="hidKeyName" Value="txtlocation" runat="server" />
        <input type="hidden" id="hidNoPromt" value="0" />
    <input type="hidden" id="hidempid" value="<%=Session["Login"].ToString() %>" />
    <input type="hidden" id="hidDeleteMessage" value="<%=m_deletemessage %>" />
    </form>
</body>
<script type="text/javascript" language="javascript">
    <asp:Literal ID="litScript1" runat="server"></asp:Literal>
</script>
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

    function modifyLocation() {
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
      }
      if (resp) {
        if (Page_ClientValidate('')) {
          noPrompt();
          __doPostBack("ModifyLocation", "");
        }
      }
}
function updatecontactname() {
    document.getElementById("txtcontactname").value = getvalue("hidfirstname") + " " + getvalue("hidlastname");
}
</script>
</html>
