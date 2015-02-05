<%@ Page Language="C#" AutoEventWireup="true" CodeFile="projectmain.aspx.cs" Inherits="project_projectmain"
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
      background: white;
      color: brown !important; /*add more style definitions here*/
      cursor: pointer !important;
    }
    .InnerAlernatingItemStyle
    {
      background: brown !important;
      color: brown !important; /*add more style definitions here*/
      cursor: pointer !important;
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
    <asp:HiddenField ID="hidStatusComments" Value="" runat="server" />
    <asp:HiddenField ID="hidStatusApplyTo" Value="" runat="server" />
    <asp:HiddenField ID="hidTargetStatusCode" Value="<%=statuscode %>" runat="server" />
        <asp:HiddenField ID="hidKeyName" Value="txtprojectid" runat="server" />
        <asp:HiddenField ID="hidTargetStatus" Value="" runat="server" />
    <asp:HiddenField ID="hidToDo" Value="" runat="server" />
    <input type="hidden" id="hidempid" value="<%=Session["Login"].ToString() %>" />
    <input type="hidden" id="hidDataChanged" value="0" />


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

    function rowClicked(sender, eventArgs) {
        if (eventArgs.get_tableView().get_name() == "wolist")
        {
            var wonum = eventArgs.getDataKeyValue("WoNum");
            var url= "../Workorder/woframe.aspx?mode=edit&wonum="+wonum;
            var oWnd;
            oWnd = window.open(url);
        }
    }

    function keyPressed(code) {
        if (code == 13 && "<%=querymode %>" == "query") {
            LookupClicked();
        }
    }
    function addnewphase() {
       var projectid = "<%=m_projectid %>";
       url = "../Project/projectphase.aspx?mode=new&projectid=" + projectid;
       opennewmodalwindow(url, null);

    }
    function addnewworkorder(phase) {
       var projectid = "<%=m_projectid %>";
       url = "../Project/wolist.aspx?phase=" + phase;
       opennewmodalwindow(url, null);
    }
    function editphase(counter) {
       var projectid = "<%=m_projectid %>";
       url = "../Project/projectphase.aspx?mode=edit&projectid=" + projectid + "&counter=" + counter;
       opennewmodalwindow(url, null);
    }
    function deletewo(wonum) {
       if(!allowsubmit()) return;
       if (!confirm("Are you sure to remove the Work Order record from this phase?")) return;
       $.ajax({
            type: "GET",
            url: "../InternalServices/ServiceProj.svc/DeleteWO",
            data: { "wonum": wonum },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr, settings) {
                xhr.setRequestHeader("userid","<%=Session["Login"] %>");
            },
            success: function (data, status) {
            FormSubmitted=false;
                var result = data.d;
                if (result != "OK")                    
                    alert(result);
                else
                {
                    refreshGrid();
                }
            },
            error: OnGetMemberError
        });
    }

    function addnewseletedWO(paramXML,phase) {
       var projectid = "<%=m_projectid %>";
       if(!allowsubmit()) return;
        $.ajax({
            type: "GET",
            url: "../InternalServices/ServiceProj.svc/AddNewWOtoPhase",
            data: { "paramXML": paramXML,"projectid": projectid, "phase": phase },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr, settings) {
                xhr.setRequestHeader("userid","<%=Session["Login"] %>");
            },
            success: function (data, status) {
            FormSubmitted=false;
                var result = data.d;
                if (result != "OK")                    
                    alert(result);
                else
                {
                    refreshGrid();
                }
            },
            error: OnGetMemberError
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

    function refreshGrid() {
       var grid = $find("grdphaselist");
       var mastertableview = grid.get_masterTableView();
       mastertableview.rebind();
    }

    function mainpostback(postbackrequest)
    {
        alert(postbackrequest);
        __doPostBack(postbackrequest, "");
    }

    function saveStatus() {
        var obj = document.getElementById("txtprojectid");
        var changed = document.getElementById("hidDataChanged");
        
        if (changed == null)
        {
            changed = 0;
        }
        else
        {
            changed = document.getElementById("hidDataChanged").value * 1;
        }
        var resp = 1;
        if (changed == 1) {
            resp = window.confirm("Project has been changed. Do you want to save the changes? Click Yes to save workorder and change status\nClick No to abort. ");
            //resp = window.confirm('<%=m_msg["T33"] %>');
            //if (WarnUser())
        }
        if (resp) {
            if (Page_ClientValidate(''))
            {
                 saveproject("saveStatus");
//                __doPostBack("saveStatus", "");
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

</script>
</html>
