<%@ Page Language="C#" AutoEventWireup="true" CodeFile="projectcompletion.aspx.cs" Inherits="project_projectcompletion"
    MaintainScrollPositionOnPostback="true" %>

<%@ Register TagPrefix="uc1" TagName="ProjectHeader" Src="projectheader.ascx" %>
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
    <style>
    .InnerHeaderStyle
    {
      background:  #98f5ff !important;
      font-size: 13px !important;
      color: white !important; /*add more style definitions here*/
    }
    .InnerItemStyle
    {
      background: white !important;
      color: brown !important; /*add more style definitions here*/
    }
    .InnerAlernatingItemStyle
    {
      background: white !important;
      color: brown !important; /*add more style definitions here*/
    }
    .MostInnerHeaderStyle
    {
      background: #FFEBCD !important;
      /*font-size: 15px !important;*/
      color: white !important; /*add more style definitions here*/
    }
    .MostInnerItemStyle
    {
      background: white !important;
      color: olive !important; /*add more style definitions here*/
    }
    .MostInnerAlernatingItemStyle
    {
      background: white !important;
      color: olive !important; /*add more style definitions here*/
    }
    </style>
</head>
<body style="background-image: url(../IMAGES/back.png); background-repeat: repeat-x;
    margin: 0px; padding: 0px;" onload = "pageinit()">
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
    <script type="text/javascript">


    </script>
    <uc1:projectHeader ID="ucHeader1" runat="server" />
    <asp:Panel ID="MainControlsPanel" runat="server" CssClass="MainControlsPanelClass">
        <telerik:RadScriptManager ID="RadScriptManager" runat="server">
        </telerik:RadScriptManager>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false">
        </telerik:RadWindowManager>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="grdphaselist">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdphaselist" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
            <ClientEvents OnRequestStart="RequestStart" OnResponseEnd="ResponseEnd" />
        </telerik:RadAjaxManager>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
        <asp:SqlDataSource ID="PhaseListSqlDataSource" runat="server" ProviderName="System.Data.OleDb"></asp:SqlDataSource>
        <asp:SqlDataSource ID="WOListSqlDataSource" runat="server" ProviderName="System.Data.OleDb"></asp:SqlDataSource>

    </asp:Panel>
    <asp:HiddenField ID="hidMode" runat="server" />


    <input type="hidden" id="hidNoPromt" value="0" />
    <iframe id="ifValidate" src="" frameborder="0" scrolling="no" style="visibility: hidden;
        display: none;"></iframe>
    </form>
    <asp:Literal ID="litScript1" runat="server"></asp:Literal>
</body>
<script type="text/javascript">
    $(window).load(function () {
        setpanelheight(1);
        initToolbar();
    });

    function pageinit() {
        $(document).ready(function () {
            try {
                setTimeout(parent.controlpanel.RefreshPanelGrids(), 0);
                setTimeout(parent.controlpanel._updatecounts(), 200);
                setTimeout(updateworklist(), 200);

            }
            catch (err) {
                alert(err);
            }
        });
    }


   function opennewmodalwindow(url, name) {
       /* This function is used to open a new modal window for admin panel*/
       var oWnd;
       if (GetRadWindow() == null) {
           oWnd = radopen(url, null);
           oWnd.set_modal(true);
       }
       else {
           var radwin = GetRadWindow();
           var oManager = GetRadWindow().get_windowManager();
           oWnd = oManager.open(url, null);
           oWnd.set_modal(true);
       }
   }

   function updateworklist() { 
        var grid = $find("<%=grdwolist.ClientID %>");
        var dataItems = grid.get_masterTableView().get_dataItems();
        var projectid = "<%=m_projectid %>"

        if (projectid != "") {
            for (var i = 0; i < dataItems.length; i++) {
                var statuscode = dataItems[i].getDataKeyValue("statuscode") * 1;
                if (statuscode<300 || statuscode>=400) {
                    dataItems[i].get_element().style.backgroundColor = "yellow";

                }
                else {
                    dataItems[i].get_element().style.backgroundColor = '';
                }
            }
        }
        else {
            alert(1);
            for (var i = 0; i < dataItems.length; i++) {
                dataItems[i].get_element().style.backgroundColor = '';
            }
        }  
   }

   function refreshGrid() {
       var grid = $find("grdphaselist");
       var mastertableview = grid.get_masterTableView();
       mastertableview.rebind();
   }

   
</script>
</html>
