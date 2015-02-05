<%@ Page Language="C#" AutoEventWireup="true" CodeFile="tcpanel.aspx.cs" Inherits="Timecards_tcpanel" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta http-equiv="content-type" content="text/html; charset=utf-8" />
    <link type="text/css" href="../Styles/Header.css" rel="stylesheet" />
    <link type="text/css" href="../Styles/Azzier.css" rel="stylesheet" />
    <link type="text/css" href="~/Styles/Customer.css" rel="stylesheet" />
    <link type="text/css" href="../css/custom-theme/jquery-ui-1.8.21.custom.css" rel="stylesheet" />
</head>
<body style="background-image: url(../IMAGES/back.png); background-repeat: repeat-x;
    margin: 0px; padding: 0px;" >
    <asp:Literal ID="litMessage" runat="server" />
    <script type="text/javascript" src="../js/jquery-1.7.2.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.21.custom.min.js"></script>
    <script type="text/javascript" src="../Jscript/Header.js"></script>
    <script type="text/javascript" src="../Jscript/radwindow.js"></script>
    <script type="text/javascript" src="../Jscript/Codes.js"></script>
    <script type="text/javascript" src="../Jscript/Validation.js"></script>
    <script type="text/javascript" src="../Jscript/logoff.js"></script>
    <script type="text/javascript" src="../Jscript/StopDoubleClick.js"></script>
    <script type="text/javascript" src="../Jscript/RadControls.js"></script>

    <form id="mainform" runat="server">
        <telerik:RadScriptManager ID="RadScriptManager" runat="server">
        </telerik:RadScriptManager>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false">
        </telerik:RadWindowManager>
        <asp:Panel ID="MainControlsPanel" runat="server" CssClass="PanelControlsPanelClass">
        </asp:Panel>
        <asp:Panel ID="MainControlsPanel2" runat="server"  CssClass="PanelControlsPanelClass">
        </asp:Panel>
        <telerik:AjaxSetting AjaxControlID="RadGrid1">
           <UpdatedControls>
                 <telerik:AjaxUpdatedControl ControlID="grdresults" />
           </UpdatedControls>
        </telerik:AjaxSetting>

        <div id="header-wrap">
            <div style="display: block; width: 100%; height: 30px; position: relative; padding: 0 0 0 10px;
                margin: 0px; background-image: url(../IMAGES/tabback.png); background-repeat: repeat-x;
                z-index: 3;">
                <table width="90%">
                    <tr>
                        <td align="right">
                            <a href="javascript:setLogoffCookie('logoff','1','TIMECARD')">Log Off</a>
                        </td>
                    </tr>
                </table>
            </div>
            <br />
            <br />
            <div style="background-image: url(../IMAGES/toolbarback.png); background-repeat: repeat-x;
                position: relative; top: -28px; display: block; width: 100%; height: 64px; padding: 5px 0px 0px 0px;
                vertical-align: top; z-index: 5;">
                <telerik:RadToolBar ID="toolbar" runat="server">
                </telerik:RadToolBar>
            </div>
            <div style="position: relative; top: -34px; display: block; padding: 0px; margin: 0px;
            width: 100%; height: 30px; background-image: url(../IMAGES/statbar_back.png); background-repeat: repeat-x;
            z-index: 5;">
            <table width="100%">
                <tr>
                    <td width="40%">
                        <asp:Label ID="lblListType" runat="server" Text="" Style="position: absolute; top: 9px;
                            display: block; width: 200px; font-family: Arial, Helvetica, sans-serif; font-size: 8pt;
                            color: #FFFFFF;" onclick="chgframesizewithvalue('TIMECARD')"/>
                    </td>
                    <td width="60%" align="right" valign="middle" style="position: relative; top: 0px">
                    <asp:Label ID="lblRecords" runat="server" Style="position: relative; top: -5px; font-family: Arial, Helvetica, sans-serif;
                            font-size: 8pt; color: #FFFFFF; display: none" Text="0/100" />
                    </td>
                </tr>
            </table>
        </div>
        </div>
        <asp:HiddenField ID="hidQueryResultList" runat="server" />
        <asp:HiddenField ID="hidMyWorkList" runat="server" />
        <asp:HiddenField ID="hidMyRecentList" runat="server" />
        <asp:HiddenField ID="hidCurrentList" runat="server" />
        <asp:HiddenField ID="hidCurrentIndex" runat="server" />
        <asp:HiddenField ID="hidMode" runat="server" />
        <asp:HiddenField ID="hidQueryStr" runat="server" />
        <asp:HiddenField ID="hidGetFirstRecord" runat="server" />
        <input type="hidden" id="hidpushrecord" value="1" />
    </form>
</body>

<script type="text/javascript">
    setpanelheight(2);
    $(window).load(function () {
        initToolbar();
    });

</script>

<script type ="text/javascript">
    function toolbarclicked(sender, args) {   
        var button = args.get_item();
        var commandname = button.get_commandName();
        var hidmode = $('input:hidden[name=hidMode]').val();
        switch (commandname) {
            case 'queryresult':
                showpanel(0);
                break;
            case 'recentlist':
                showpanel(1);
                break;
            case 'postcard':
                
                posttimecards();
                break;
            default:
                alert('fail');
                break;
        }
    }

    function showpanel(panelid) {
        var panel1 = document.getElementById("<%=MainControlsPanel.ClientID %>");
        var panel2 = document.getElementById("<%=MainControlsPanel2.ClientID %>");
        var lbl = document.getElementById("<%=lblListType.ClientID %>");
        var hidMode = document.getElementById("hidMode");
        switch (panelid) {
            case 0:
                panel1.style.visibility = "visible";
                panel1.style.display = "block";
                panel2.style.visibility = "hidden";
                panel2.style.display = "none";
                lbl.innerHTML = '<%=m_msg["T3"] %>';
                hidMode.value = "queryresult";
                resizeGrid("MainControlsPanel", "grdresults");
                break;
            case 1:
                panel2.style.visibility = "visible";
                panel2.style.display = "block";
                panel1.style.visibility = "hidden";
                panel1.style.display = "none";
                lbl.innerHTML = '<%=m_msg["T2"] %>';
                hidMode.value = "recentlist";
                resizeGrid("MainControlsPanel2", "grdrecentlist");
                break;
            default:
                break;
        }
    }

    function rowClicked(sender, eventArgs) {
        var counter = eventArgs.getDataKeyValue("Counter");
        var proceed = true;
        try {
            proceed = parent.mainmodule.WarnUser()
        }
        catch (err) {
        }
        if (proceed) {
            parent.mainmodule.document.location.href = "tcmain.aspx?mode=edit&counter=" + counter;
            var index = 0;

            var e = eventArgs.get_domEvent();
            var grid = $find(sender.get_id());
            var row = eventArgs.get_gridDataItem();
            var allRows = grid.get_masterTableView().get_dataItems();
        }
    }

    function updatecounts(sender) {
        var mode = document.getElementById("hidMode").value;
        if ((mode == "queryresult" && sender.get_id() == "grdresults") || (mode == "recentlist" && sender.get_id() == "grdrecentlist")) {
            var counter = "";
            
            if (parent.mainmodule.document.getElementById("txtcounter") != null)
                counter = parent.mainmodule.document.getElementById("txtcounter").value;
            var index = -1;
            
            var grid = sender;
            var allRows = grid.get_masterTableView().get_dataItems();
            if (counter != "") {
                for (var i = 0; i < allRows.length; i++) {
                    if (allRows[i].getDataKeyValue("Counter") == counter) {
                        allRows[i].get_element().style.backgroundColor = 'lime';
                        index = i + 1;
                    }
                    else {
                        allRows[i].get_element().style.backgroundColor = '';
                    }
                }
            }
            else {
                for (var i = 0; i < allRows.length; i++) {
                    allRows[i].get_element().style.backgroundColor = '';
                }
            }
        }
    }


    function _updatecounts() {
        var mode = document.getElementById("hidMode").value;
        if (mode == "recentlist") {
            updatecounts($find("grdrecentlist"));
        }
        else if (mode == "queryresult") {
            updatecounts($find("grdresults"));
        }
    }

    function posttimecards() {
        var mode = document.getElementById("hidMode").value;
        var gridid = "";
        switch (mode) {
            case 'recentlist':
                gridid = "<%=grdrecentlist.ClientID %>";                
                break;
            case 'queryresult':
                gridid = "<%=grdresults.ClientID %>";
                break;
        }
        var grid = $find(gridid);
        var MasterTable = grid.get_masterTableView();
        var selectedRows = MasterTable.get_selectedItems();
        var xmlparam = "<?xml version='1.0' encoding='UTF-8'?><roots><Linkid>";
        if (selectedRows.length < 1) {
            alert('<%=m_msg["T10"] %>');
            return;
        }
        for (var i = 0; i < selectedRows.length; i++) {
            var row = selectedRows[i];
            var cell = MasterTable.getCellByColumnUniqueName(row, "Counter");
            var cellval = row.getDataKeyValue("Counter");
            xmlparam = xmlparam + "<IDS>" + cellval + "</IDS>";
        }
        xmlparam = xmlparam + "</Linkid>";
        xmlparam = xmlparam + "</roots>";


        if (!allowsubmit()) return;
        $.ajax({
            type: "GET",
            url: "../InternalServices/ServiceLabour.svc/PostTimeCards",
            data: { "paramXML": xmlparam },
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data, status) {
                FormSubmitted = false;
                OnSuccess(grid);
            },
            error: OnGetMemberError
        });
    }

    function OnGetMemberError(request, status, error) {
        FormSubmitted = false;
        alert(status + "--" + error + "--" + request.responseText);
    }

    function OnSuccess(grid) {
        var MasterTable = grid.get_masterTableView();
        var selectedRows = MasterTable.get_selectedItems();
        for (var i = 0; i < selectedRows.length; i++) {
            var row = selectedRows[i];
            row.set_selected(false);
        }

        var gridrecentlist = $find("<%=grdrecentlist.ClientID %>");
        var gridresults = $find("<%=grdresults.ClientID %>");
        var masterTable1;
        var masterTable2;
        if (gridrecentlist != null) {
            masterTable1 = gridrecentlist.get_masterTableView();
        }
        if (gridresults != null) {
            masterTable2 = gridresults.get_masterTableView();
        }
        if (masterTable1 != null) {
            masterTable1.rebind();
        }
        if (masterTable2 != null) {
            masterTable2.rebind();
        }
        showpanel(1);
    }

    function getRecord() {
        var getFirstRecord = $("#hidGetFirstRecord").val(),
        counter = getSpecificGridItem("grdresults", 0, "Counter");

        switch (getFirstRecord) {
            case "":
            case "1":
                if (GetGridRowsCount("grdresults") != 1)
                    return;
                break;
            case "2":
                break;
            case "0":
            default:
                return;
                break;
        }

        parent.mainmodule.document.location.href = "tcmain.aspx?mode=edit&counter=" + counter;
        $("#hidpushrecord").val("0");
    }

</script>
</html>
