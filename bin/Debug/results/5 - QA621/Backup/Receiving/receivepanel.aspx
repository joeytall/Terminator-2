<%@ Page Language="C#" AutoEventWireup="true" CodeFile="receivepanel.aspx.cs" Inherits="receiving_receivepanel" %>

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
    <script type="text/javascript">
      function batchreceive() {
        if (!checkwindow())
          return;
        var polinecounterlist = "";
        var grid = $find("grdresults");
        var MasterTable = grid.get_masterTableView();
        for (var i = 0; i < grid.get_masterTableView().get_selectedItems().length; i++) {
          var selectedRow = grid.get_masterTableView().get_selectedItems()[i];
          var polinecounter = selectedRow.getDataKeyValue("Counter");
          if (polinecounterlist == "")
            polinecounterlist = polinecounter;
          else
            polinecounterlist = polinecounterlist + "," + polinecounter;
        }
        if (polinecounterlist == "") {
          alert("No record selected for receiving.");
          return;
        }
        else
        {
            var proceed = true;
            try {
              proceed = parent.mainmodule.BatchReceive(polinecounterlist);
            }
            catch (err) {
              
            }
        }
      }
      
      function refreshgrid() {
        var grid = $find("<%=grdresults.ClientID %>");
        var MasterTable = grid.get_masterTableView();
        var selectedRows = MasterTable.get_selectedItems();

        for (var i = 0; i < selectedRows.length; i++) {
          selectedRows[i].set_selected(false);
        }
        grid.get_masterTableView().rebind();
      }
        function gotomain() {
            $(document).ready(function () {
                var grid = $find("<%=grdresults.ClientID %>");
                var MasterTable = grid.get_masterTableView();
                var allRows = grid.get_masterTableView().get_dataItems();
                var row = allRows[0];
                var cell = MasterTable.getCellByColumnUniqueName(row, "ItemNum");
                var itemnum = cell.innerHTML;
                parent.mainmodule.document.location.href = "invmain.aspx?mode=edit&itemnum=" + itemnum;
            });
        }

        function rowClicked(sender, eventArgs) {
          if (!checkwindow())
            return;
            var counter = eventArgs.getDataKeyValue("Counter");
            var proceed = true;
            try {
                proceed = parent.mainmodule.ReceiveLine(counter)
            }
            catch (err) {
            }
          }

          function checkwindow() {
            if (top.mainmodule.checkmodalwindow() > 0) {
              alert("Please close popup window first.");
              return false;
            }
            else
              return true;
          }

          function returnrowClicked(sender, eventArgs) {
            if (!checkwindow())
              return;
          var counter = eventArgs.getDataKeyValue("Counter");
          var proceed = true;
          try {
            proceed = parent.mainmodule.ReturnLine(counter)
          }
          catch (err) {
          }
        }

            function rowSelecting(sender, args) {
              var isserialized = args.getDataKeyValue("Serialized");
              if (isserialized == "1")
                args._cancel = true;
            }

            function griddatabound(sender, args) {
              var grid = $find("grdresults");
              var masterTableView = grid.get_masterTableView();
              var allitems = masterTableView.get_dataItems();
              var count = allitems.length;

              for (i = 0; i < count; i++) {
                var row = allitems[i];
                var cell = masterTableView.getCellByColumnUniqueName(row, "Serialized");
                var serialized = cell.innerHTML;
                //cell = masterTableView.getCellByColumnUniqueName(row, "OrderQty");
                //var orderqty = cell.innerHTML;
                //cell = masterTableView.getCellByColumnUniqueName(row, "ReceiveQty");
                //var receiveqty = cell.innerHTML;
                if (serialized*1 == 1)
                  row.get_cell("SelectColumn").firstChild.disabled = "disabled";
                  
              }

            }



    </script>
    <form id="MainForm" runat="server">
    <telerik:RadScriptManager ID="RadScriptManager" runat="server">
    </telerik:RadScriptManager>
    <telerik:RadWindowManager ID="RadWindowManagerPanel" runat="server" ShowOnTopWhenMaximized="false">
    </telerik:RadWindowManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" >
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdresults" />
                </UpdatedControls>
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdworklist" />
                </UpdatedControls>
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdrecentlist" />
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
                        <a href="javascript:setLogoffCookie('logoff','1','RECV')">Log Off</a>
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
                            color: #FFFFFF;" onclick="chgframesizewithvalue('RECV')"/>
                    </td>
                    <td width="60%" align="right" valign="middle" style="position: relative; top: 0px">
                        <%--<asp:HyperLink ID="hlkNavFirst" NavigateUrl="javascript:navigate(-2)" runat="server"><img src="../IMAGES/First_24.png" border="0" alt="" title="First Record" /></asp:HyperLink>--%>
                        <%--<asp:HyperLink ID="hlkNavPrevious" NavigateUrl="javascript:navigate(-1)" runat="server"><img src="../IMAGES/Previous_24.png" border="0" alt="" title="Previous Record" /></asp:HyperLink>--%>
                        <asp:Label ID="lblRecords" runat="server" Style="position: relative; top: -5px; font-family: Arial, Helvetica, sans-serif;
                            font-size: 8pt; color: #FFFFFF; display: none" Text="0/100" />
                        <%--<asp:HyperLink ID="hlkNavNext" NavigateUrl="javascript:navigate(1)" runat="server"><img src="../IMAGES/Next_24.png" border="0" alt="" title="Next Record" /></asp:HyperLink>--%>
                        <%--<asp:HyperLink ID="hlkNavLast" NavigateUrl="javascript:navigate(2)" runat="server"><img src="../IMAGES/Last_24.png" border="0" alt="" title="Last Record" /></asp:HyperLink>--%>
                    </td>
                </tr>
            </table>
        </div>
        <asp:HiddenField ID="hidCurrentRecord" runat="server" />
    </div>
    <asp:Panel ID="MainControlsPanel" runat="server" CssClass="PanelControlsPanelClass">
    </asp:Panel>
    <asp:Panel ID="MainControlsPanel2" runat="server"  CssClass="PanelControlsPanelClass">
    </asp:Panel>
    <asp:Panel ID="MainControlsPanel3" runat="server" CssClass="PanelControlsPanelClass">
    </asp:Panel>
    <asp:HiddenField ID="hidQueryResultList" runat="server" />
    <asp:HiddenField ID="hidMyWorkList" runat="server" />
    <asp:HiddenField ID="hidMyRecentList" runat="server" />
    <asp:HiddenField ID="hidCurrentList" runat="server" />
    <asp:HiddenField ID="hidCurrentIndex" runat="server" />
    <asp:HiddenField ID="hidQueryStr" runat="server" />
    <asp:HiddenField ID="hidGetFirstRecord" runat="server" />
    <asp:HiddenField ID="hidMode" runat="server" />
    <asp:LinqDataSource ID="POLinqDataSource" runat="server" ContextTypeName="DataClassesDataContext" >
    </asp:LinqDataSource>
    </form>
</body>
<asp:literal id="litScript" runat="server" />
<script type="text/javascript">
    setpanelheight(2);
    $(window).load(function () {
        initToolbar();
        checklogoff();
    });

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
        }
    }


    function RadGrid_RowSelected(sender, args) {
        var mailID = args.getDataKeyValue("Counter");
        if (!selected[mailID]) {
            selected[mailID] = true;
        }
    }

    function RadGrid_RowDeselected(sender, args) {
        var mailID = args.getDataKeyValue("Counter");
        if (selected[mailID]) {
            selected[mailID] = null;
        }
    }

    function RadGrid_RowCreated(sender, args) {
        alert(sender.get_id());
        var mailID = args.getDataKeyValue("Counter");
        //alert(mailID);
        if (selected[mailID]) {
            args.get_gridDataItem().set_selected(true);
        }
        //alert("rowcreated finished");
    }

    
    function toolbarclicked(sender, args) {
        var button = args.get_item();
        var commandname = button.get_commandName();
        var hidmode = $('input:hidden[name=hidMode]').val();
        switch (commandname) {
          case "batchreceive":
            batchreceive();
            return false;
            break;
          case "removeworklist":
              if (hidmode != "worklist") {
                  alert('<%=m_msg["T16"] %>');//'You are not in work list.');
              }
              else {
                  removeworklist();
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
          default:
              return false;
        }
          }

          function OnGetMemberError(request, status, error) {
            FormSubmitted = false;
            alert(status + "--" + error + "--" + request.responseText);
          }

</script>
</html>
