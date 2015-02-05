<%@ Page Language="C#" AutoEventWireup="true" CodeFile="srpanel.aspx.cs" Inherits="StoreRoom_srpanel" %>
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

<script type="text/javascript" src="../js/jquery-1.7.2.min.js"></script>
<script type="text/javascript" src="../js/jquery-ui-1.8.21.custom.min.js"></script>
<script type="text/javascript" src="../Jscript/Codes.js"></script>
<script type="text/javascript" src="../Jscript/Validation.js"></script>
<script type="text/javascript" src="../Jscript/StopDoubleClick.js"></script>
<script type="text/javascript" src="../Jscript/Header.js"></script>
<script type="text/javascript" src="../Jscript/radwindow.js"></script>
<script type="text/javascript" src="../Jscript/logoff.js"></script>
<script type="text/javascript" src="../Jscript/RadControls.js"></script>

<body style="background-image: url(../IMAGES/back.png); background-repeat: repeat-x;
    margin: 0px; padding: 0px;" >
    <form id="mainform" runat="server">
        <asp:Literal ID="litMessage" runat="server" /> 
        <telerik:RadScriptManager ID="RadScriptManager" runat="server">
        </telerik:RadScriptManager>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false">
        </telerik:RadWindowManager>
        <asp:Panel ID="MainControlsPanel" runat="server" CssClass="PanelControlsPanelClass">
        </asp:Panel>
        <asp:Panel ID="MainControlsPanel2" runat="server"  CssClass="PanelControlsPanelClass">
        </asp:Panel>
        <asp:Panel ID="MainControlsPanel3" runat="server"  CssClass="PanelControlsPanelClass">
        </asp:Panel>
        <div id="header-wrap">
            <div style="display: block; width: 100%; height: 30px; position: relative; padding: 0 0 0 10px;
                margin: 0px; background-image: url(../IMAGES/tabback.png); background-repeat: repeat-x;
                z-index: 3;">
                <table width="90%">
                    <tr>
                        <td align="right">
                            <a href="javascript:setLogoffCookie('logoff','1','STOREROOM')">Log Off</a>
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
                                color: #FFFFFF;" onclick="chgframesizewithvalue('STOREROOM')"/>
                        </td>
                        <td width="60%" align="right" valign="middle" style="position: relative; top: 0px">
                        <asp:Label ID="lblRecords" runat="server" Style="position: relative; top: -5px; font-family: Arial, Helvetica, sans-serif;
                                font-size: 8pt; color: #FFFFFF; display: none" Text="0/100" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <asp:HiddenField ID="hidMode" runat="server" />
        <asp:HiddenField ID="hidQueryStr" runat="server" />
        <asp:HiddenField ID="hidGetFirstRecord" runat="server" />
        <input type="hidden" id="hidpushrecord" value="1" />
    </form> 
</body>
</html>

<script type = "text/javascript">
    setpanelheight(2);

    $(window).load(function () {
        initToolbar();
    });
</script>

<script type="text/javascript">

    function getRecord() {
        var getFirstRecord = $("#hidGetFirstRecord").val(),
        storeroom = getSpecificGridItem("grdresults", 0, "StoreRoom");

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

        parent.mainmodule.document.location.href = "srmain.aspx?mode=edit&storeroom=" + storeroom;
        $("#hidpushrecord").val("0");
    }

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
            case 'worklist':
                showpanel(2);
                break;
            case 'addworklist':
                addworlist();
                break;
            case 'removeworklist':
              var r = confirm("Are you sure you would like to remove the selected records from the work list?");
                if (r == true) {
                    removeworklist();
                }
                else {
                    return;
                }
                break;
            default:
                alert('fail');
                break;
        }
    }

    function showpanel(panelid) {
        var panel1 = document.getElementById("<%=MainControlsPanel.ClientID %>");
        var panel2 = document.getElementById("<%=MainControlsPanel2.ClientID %>");
        var panel3 = document.getElementById("<%=MainControlsPanel3.ClientID %>");
        var lbl = document.getElementById("<%=lblListType.ClientID %>");
        var hidMode = document.getElementById("hidMode");
        switch (panelid) {
            case 0:
                panel1.style.visibility = "visible";
                panel1.style.display = "block";
                panel2.style.visibility = "hidden";
                panel2.style.display = "none";
                panel3.style.visibility = "hidden";
                panel3.style.display = "none";
                lbl.innerHTML = '<%=m_msg["T3"] %>';
                hidMode.value = "queryresult";
                resizeGrid("MainControlsPanel", "grdresults");
                break;
            case 1:
                panel2.style.visibility = "visible";
                panel2.style.display = "block";
                panel1.style.visibility = "hidden";
                panel1.style.display = "none";
                panel3.style.visibility = "hidden";
                panel3.style.display = "none";
                lbl.innerHTML = '<%=m_msg["T2"] %>';
                hidMode.value = "recentlist";
                resizeGrid("MainControlsPanel2", "grdrecentlist");
                
                break;
            case 2:
                panel3.style.visibility = "visible";
                panel3.style.display = "block";
                panel1.style.visibility = "hiddken";
                panel1.style.display = "none";
                panel2.style.visibility = "hidden";
                panel2.style.display = "none";
                lbl.innerHTML = '<%=m_msg["T4"] %>';
                hidMode.value = "worklist";
                resizeGrid("MainControlsPanel3", "grdworklist");
                break;
            default:
                break;
        }
    }

    function updatecounts(sender) {
      var mode = document.getElementById("hidMode").value;
        if ((mode == "queryresult" && sender.get_id() == "grdresults") || (mode == "recentlist" && sender.get_id() == "grdrecentlist")||(mode == "worklist" && sender.get_id() == "grdworklist")) {
            var storeroom = "";

            if (parent.mainmodule.document.getElementById("txtstoreroom") != null)
                storeroom = parent.mainmodule.document.getElementById("txtstoreroom").value;
            var index = -1;

            var grid = sender;
            var allRows = grid.get_masterTableView().get_dataItems();
            if (storeroom != "") {
                for (var i = 0; i < allRows.length; i++) {
                    if (allRows[i].getDataKeyValue("StoreRoom") == storeroom) {
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
        if (mode == "worklist") {
            updatecounts($find("grdworklist"));
        }
        else if (mode == "queryresult") {
            updatecounts($find("grdresults"));
        }
        else if (mode == "recentlist") {
            updatecounts($find("grdrecentlist"));
        }
    }

    function rowClicked(sender, eventArgs) {
        var storeroom = eventArgs.getDataKeyValue("StoreRoom");
        var proceed = true;
        try {
            proceed = parent.mainmodule.WarnUser()
        }
        catch (err) {
        }
        if (proceed) {
            parent.mainmodule.document.location.href = "srmain.aspx?mode=edit&storeroom=" + storeroom;
            var index = 0;
        }
    }


    function addworlist() {
        var mode = document.getElementById("hidMode").value;
        var gridid = "";
        switch (mode) {
            case 'recentlist':
                gridid = "<%=grdrecentlist.ClientID %>";
                break;
            case 'queryresult':
                gridid = "<%=grdresults.ClientID %>";
                break;
            case 'worklist':
                gridid = "<%=grdworklist.ClientID %>";
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
            var cell = MasterTable.getCellByColumnUniqueName(row, "StoreRoom");
            var cellval = cell.innerHTML;
            xmlparam = xmlparam + "<IDS>" + cellval + "</IDS>";
        }
        xmlparam = xmlparam + "</Linkid>";
        xmlparam = xmlparam + "</roots>";

        if (!allowsubmit()) return;
        $.ajax({
            type: "GET",
            url: "../InternalServices/ServiceInventory.svc/AddStoreRoomWorkList",
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

    function removeworklist() {
        var mode = document.getElementById("hidMode").value;
        var gridid = "";
        switch (mode) {
            case 'recentlist':
                gridid = "<%=grdrecentlist.ClientID %>";
                break;
            case 'queryresult':
                gridid = "<%=grdresults.ClientID %>";
                break;
            case 'worklist':
                gridid = "<%=grdworklist.ClientID %>";
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
                    var cell = MasterTable.getCellByColumnUniqueName(row, "StoreRoom");
        var cellval = cell.innerHTML;
            xmlparam = xmlparam + "<IDS>" + cellval + "</IDS>";
        }
        xmlparam = xmlparam + "</Linkid>";
        xmlparam = xmlparam + "<LinkModule>storeroom</LinkModule>"
        xmlparam = xmlparam + "</roots>";

        if (!allowsubmit()) return;
        $.ajax({
            type: "GET",
            url: "../InternalServices/ServiceInventory.svc/DelStoreRoomWorkList",
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
        var mode = document.getElementById("hidMode").value;
        var MasterTable = grid.get_masterTableView();
        var selectedRows = MasterTable.get_selectedItems();
        for (var i = 0; i < selectedRows.length; i++) {
            var row = selectedRows[i];
            row.set_selected(false);
        }

        var gridrecentlist = $find("<%=grdrecentlist.ClientID %>");
        var gridresults = $find("<%=grdresults.ClientID %>");
        var gridworklist = $find("<%=grdworklist.ClientID %>")
        var masterTable1;
        var masterTable2;
        var masterTable3;
        if (gridrecentlist != null) {
            masterTable1 = gridrecentlist.get_masterTableView();
        }
        if (gridresults != null) {
            masterTable2 = gridresults.get_masterTableView();
        }
        if (gridworklist != null) {
            masterTable3 = gridworklist.get_masterTableView();
        }
        if (masterTable1 != null) {
            masterTable1.rebind();
        }
        if (masterTable2 != null) {
            masterTable2.rebind();
        }
        if (masterTable3 != null) {
            masterTable3.rebind();
        }
        showpanel(2);
    }
</script>
