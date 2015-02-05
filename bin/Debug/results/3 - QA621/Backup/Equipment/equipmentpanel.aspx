<%@ Page Language="C#" AutoEventWireup="true" CodeFile="equipmentpanel.aspx.cs" Inherits="equipment_equipmentpanel" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta http-equiv="content-type" content="text/html; charset=utf-8" />
    <link type="text/css" href="../Styles/Header.css" rel="stylesheet" />
    <link type="text/css" href="../Styles/Azzier.css" rel="stylesheet" />
    <link type="text/css" href="~/Styles/Customer.css" rel="stylesheet" />
</head>
<body style="background-image: url(../IMAGES/back.png); background-repeat: repeat-x;
    margin: 0px; padding: 0px;" onunload="chgframesize('EQP')">
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
    <script type="text/javascript">

        function rowClicked(sender, eventArgs) {
            var equipment = eventArgs.getDataKeyValue("Equipment");
            var proceed = true;
            try {
                proceed = parent.mainmodule.WarnUser()
            }
            catch (err) {
            }
            if (proceed) {
                parent.mainmodule.document.location.href = "equipmentmain.aspx?mode=edit&equipment=" + equipment;
            }
        }

    </script>
    <form id="MainForm" runat="server">
    <telerik:RadScriptManager ID="RadScriptManager" runat="server">
    </telerik:RadScriptManager>
    <telerik:RadWindowManager ID="RadWindowManagerPanel" runat="server" ShowOnTopWhenMaximized="false">
    </telerik:RadWindowManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdresults" />
                </UpdatedControls>
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdworklist" LoadingPanelID="RadAjaxLoadingPanel1" />
                </UpdatedControls>
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdrecentlist" LoadingPanelID="RadAjaxLoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="grdworklist">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdworklist" />
                </UpdatedControls>
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lblListType" />
                </UpdatedControls>
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="hidMyWorkList" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="grdresults">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdresults" />
                </UpdatedControls>
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lblListType" />
                </UpdatedControls>
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="hidQueryResultList" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="grdrecentlist">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdrecentlist" />
                </UpdatedControls>
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lblListType" />
                </UpdatedControls>
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="hidMyRecentList" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <div id="header-wrap">
        <div style="display: block; width: 100%; height: 30px; position: relative; padding: 0 0 0 10px;
            margin: 0px; background-image: url(../IMAGES/tabback.png); background-repeat: repeat-x;
            z-index: 3;">
            <table width="90%">
                <tr>
                    <td align="right">
                        <a href="javascript:setLogoffCookie('logoff','1','EQP')">Log Off</a>
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
                            color: #FFFFFF;" onclick="chgframesizewithvalue('EQP')" />
                    </td>
                    <td width="30%">
                    </td>
                    <td width="40%" align="right" valign="middle" style="position: relative; top: 0px">
                    </td>
                </tr>
            </table>
        </div>
        <asp:HiddenField ID="hidCurrentRecord" runat="server" />
    </div>
    <asp:Panel ID="MainControlsPanel" runat="server" CssClass="PanelControlsPanelClass">
    </asp:Panel>
    <asp:Panel ID="MainControlsPanel2" runat="server" CssClass="PanelControlsPanelClass">
    </asp:Panel>
    <asp:Panel ID="MainControlsPanel3" runat="server" CssClass="PanelControlsPanelClass">
    </asp:Panel>
    <asp:HiddenField ID="hidQueryResultList" runat="server" />
    <asp:HiddenField ID="hidMyWorkList" runat="server" />
    <asp:HiddenField ID="hidMyRecentList" runat="server" />
    <asp:HiddenField ID="hidCurrentList" runat="server" />
    <asp:HiddenField ID="hidCurrentIndex" runat="server" />
    <asp:HiddenField ID="hidMode" runat="server" />
    <asp:HiddenField ID="hidResultCount" runat="server" />
    <asp:HiddenField ID="hidQueryStr" runat="server" />
    <asp:HiddenField ID="hidGetFirstRecord" runat="server" />
    <input type="hidden" id="hidpushrecord" value="1" />
    <iframe id="ifValidate" src="" frameborder="0" scrolling="no" style="visibility: hidden;
        display: none;"></iframe>
    </form>
    <asp:Literal ID="litScript" runat="server" />
</body>
<script type="text/javascript">
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

    function updatecounts(sender) {
        var mode = document.getElementById("hidMode").value;
        if ((mode == "queryresult" && sender.get_id() == "grdresults") || (mode == "worklist" && sender.get_id() == "grdworklist") || (mode == "recentlist" && sender.get_id() == "grdrecentlist")) {

            var obj = document.getElementById("lblRecords");
            var equipment = "";
            if (parent.mainmodule.document.getElementById("txtequipment") != null)
                equipment = parent.mainmodule.document.getElementById("txtequipment").value;
            var index = -1;

            var grid = sender;
            var allRows = grid.get_masterTableView().get_dataItems();

            if (equipment != "") {
                for (var i = 0; i < allRows.length; i++) {
                    if (allRows[i].getDataKeyValue("Equipment") == equipment) {
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

            if (obj != null) {
                if (index == -1)
                    index = index + 1;
                obj.innerHTML = (index) + "/" + allRows.length;
            }
            obj = document.getElementById("hidCurrentIndex");
            obj.value = index;
        }
    }

</script>
<script type="text/javascript">

    $(window).load(function () {
        initToolbar();
        checklogoff();
    });

  setpanelheight(2);    

    function showpanel(panelid) {
        var panel1 = document.getElementById("<%=MainControlsPanel.ClientID %>");
        var panel2 = document.getElementById("<%=MainControlsPanel2.ClientID %>");
        var panel3 = document.getElementById("<%=MainControlsPanel3.ClientID %>");
        var lbl = document.getElementById("<%=lblListType.ClientID %>");
        if (panelid == 1) {
            panel1.style.visibility = "visible";
            panel1.style.display = "block";
            panel2.style.visibility = "hidden";
            panel2.style.display = "none";
            panel3.style.visibility = "hidden";
            panel3.style.display = "none";
            lbl.innerHTML = '<%=m_msg["T3"] %>';
            document.getElementById("hidMode").value = "queryresult";
            document.getElementById("hidCurrentList").value = document.getElementById("hidQueryResultList").value;
            updatecounts($find("grdresults"));
            resizeGrid("MainControlsPanel", "grdresults");
        }
        else if (panelid == 2) {
            panel2.style.visibility = "visible";
            panel2.style.display = "block";
            panel1.style.visibility = "hidden";
            panel1.style.display = "none";
            panel3.style.visibility = "hidden";
            panel3.style.display = "none";
            lbl.innerHTML = '<%=m_msg["T4"] %>';
            document.getElementById("hidMode").value = "worklist";
            document.getElementById("hidCurrentList").value = document.getElementById("hidMyWorkList").value;
            updatecounts($find("grdworklist"));
            resizeGrid("MainControlsPanel2", "grdworklist");
        }
        else if (panelid == 3) {
            panel1.style.visibility = "hidden";
            panel1.style.display = "none";
            panel2.style.visibility = "hidden";
            panel2.style.display = "none";
            panel3.style.visibility = "visible";
            panel3.style.display = "block";
            lbl.innerHTML = '<%=m_msg["T2"] %>';
            document.getElementById("hidMode").value = "recentlist";
            document.getElementById("hidCurrentList").value = document.getElementById("hidMyRecentList").value;
            updatecounts($find("grdrecentlist"));
            resizeGrid("MainControlsPanel3", "grdrecentlist");
        }
    }

    function addToWorklist() {
        var grid = null;
        var mode = document.getElementById("hidMode").value;
        if (mode == "worklist") {
            alert('<%=m_msg["T16"] %>');//'You are currently in work list.');
            return false;
        }
        else if (mode == "queryresult") {

            grid = $find("grdresults");
        }
        else if (mode == "recentlist") {
            grid = $find("grdrecentlist");
        }

        //var grid = $find("grdworesults");
        if (grid != null) {
            var masterview = grid.get_masterTableView();
            var selectedrows = masterview.get_selectedItems();
            if (selectedrows.length > 0)
                return true;
            else {
                alert('<%=m_msg["T10"] %>');
                return false;
            }
        }
        else return false;
    }

    //check remove from worklist 
    function chkremoveworklist() {
        var grid = null;
        var mode = document.getElementById("hidMode").value;
        if (mode != "worklist") {
            alert('<%=m_msg["T17"] %>');//'You are not currently in work list.');
            return false;
        }

        grid = $find("grdworklist");

        if (grid != null) {
            var masterview = grid.get_masterTableView();
            var selectedrows = masterview.get_selectedItems();
            if (selectedrows.length > 0)
                return true;
            else {
                alert('<%=m_msg["T10"] %>');
                return false;
            }
        }
        else return false;
    }

    function RadGrid_RowSelected(sender, args) {
        var mailID = args.getDataKeyValue("equipment");
        //alert(mailID);
        if (!selected[mailID]) {
            selected[mailID] = true;
        }
    }

    function RadGrid_RowDeselected(sender, args) {
        var mailID = args.getDataKeyValue("equipment");
        if (selected[mailID]) {
            selected[mailID] = null;
        }
    }

    function RadGrid_RowCreated(sender, args) {
        var mailID = args.getDataKeyValue("equipment");
        //alert(mailID);
        if (selected[mailID]) {
            args.get_gridDataItem().set_selected(true);
        }
        //alert("rowcreated finished");
    }

    function specsearch() {
        var URL;
        URL = "../util/specsearch.aspx?module=equipment";
        oWnd = radopen(URL, null);
        oWnd.set_modal(true);
    }

    function toolbarclicked(sender, args) {
        var button = args.get_item();
        var commandname = button.get_commandName();
        var hidmode = $('input:hidden[name=hidMode]').val();
        switch (commandname) {
            case "addworklist":
                if (addToWorklist()) {
                    var grid;
                    if (hidmode == "queryresult") {
                        grid = $find("<%=grdresults.ClientID %>");
                    }
                    else if (hidmode == "recentlist") {
                        grid = $find("<%=grdrecentlist.ClientID %>");
                    }
                    worklistadd(grid);
                }
                break;
            case "removeworklist":
                if (chkremoveworklist()) {
                  var r = confirm("Are you sure you would like to remove the selected records from the work list?");
                    if (r == true) {
                        removeworklist(hidmode);
                    }
                    else {
                        return;
                    }
                }
                break;
            case "queryresult":
                showpanel(1);
                return false;
                break;
            case "worklist":
                showpanel(2);
                return false;
                break;
            case "recentlist":
                showpanel(3);
                return false;
                break;
            case "searchspec":
                specsearch();
                return false;
                break;
            default:
                return false;
        }
    }

    function worklistadd(grid) {

        $(document).ready(function () {
            var MasterTable = grid.get_masterTableView();
            var selectedRows = MasterTable.get_selectedItems();
            var eqptlist = "";

            var eqptxml = "<?xml version='1.0' encoding='UTF-8'?><equipments><equipment>";

            for (var i = 0; i < selectedRows.length; i++) {
                var row = selectedRows[i];
                var cell = MasterTable.getCellByColumnUniqueName(row, "Equipment");
                var cellval = cell.innerHTML;
                var firstDataItem = grid.get_masterTableView().get_dataItems()[i];
                if (i == 0) {
                    eqptlist = cellval;
                }
                else {
                    eqptlist = eqptlist + "^" + cellval;
                }
                eqptxml = eqptxml + "<IDS>" + cellval + "</IDS>";
            }

            eqptxml = eqptxml + "</equipment></equipments>";
            //            alert(eqptxml);
            if (!allowsubmit()) return;
            $.ajax({
                type: "GET",
                url: "../InternalServices/ServiceEqpt.svc/EqptAddtoWorkList",
                data: { "paramXML": eqptxml },
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (data, status) {
                    OnSuccess(grid);
                    FormSubmitted = false;
                },
                error: OnGetMemberError
            });
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
            selectedRows[i].set_selected(false);
        }

        var gridworklist = $find("<%=grdworklist.ClientID %>");
        var masterTable = gridworklist.get_masterTableView();
        masterTable.rebind();
        showpanel(2);
    }

    function removeworklist(hidmode) {
        if (hidmode == "worklist") {
            hidmode = "worklist";
        }
        else if (hidmode == "queryresult") {
            hidmode = "result";
        }
        else if (hidmode == "recentlist") {
            hidmode = "recent";
        }
        
        $(document).ready(function () {
            var grid = $find("<%=grdworklist.ClientID %>");
            var MasterTable = grid.get_masterTableView();
            var selectedRows = MasterTable.get_selectedItems();

            var eqptxml = "<?xml version='1.0' encoding='UTF-8'?><roots><Linkid>";

            for (var i = 0; i < selectedRows.length; i++) {
                var row = selectedRows[i];
                var cell = MasterTable.getCellByColumnUniqueName(row, "Equipment");
                var cellval = cell.innerHTML;
                var firstDataItem = grid.get_masterTableView().get_dataItems()[i];
                eqptxml = eqptxml + "<IDS>" + cellval + "</IDS>";
            }

            eqptxml = eqptxml + "</Linkid>";

            // eqptxml = "<UserId>" + eqptxml + "</UserId>";
            eqptxml = eqptxml + "<LinkModule>Equipment</LinkModule>";
            eqptxml = eqptxml + "<LinkType>" + hidmode + "</LinkType>";

            eqptxml = eqptxml + "</roots>";

            if (!allowsubmit()) return;

            $.ajax({
                type: "GET",
                url: "../InternalServices/ServiceEqpt.svc/EqptDelWorkList",
                data: { "eqptxml": eqptxml },
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (data, status) {
                    OnSuccess(grid);
                    FormSubmitted = false;
                },
                error: OnGetMemberError
            });

        });
          }

    function getRecord() {
        var getFirstRecord = $("#hidGetFirstRecord").val(),
        equipment = getSpecificGridItem("grdresults", 0, "Equipment");

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

        parent.mainmodule.document.location.href = "equipmentmain.aspx?mode=edit&equipment=" + equipment;
        $("#hidpushrecord").val("0");
    }

</script>
</html>
