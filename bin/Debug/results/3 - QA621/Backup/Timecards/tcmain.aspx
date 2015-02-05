<%@ Page Language="C#" AutoEventWireup="true" CodeFile="tcmain.aspx.cs" Inherits="Timecards_tcmain" MaintainScrollPositionOnPostback="true"  %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc1" TagName="TCHeader" Src="tcheader.ascx" %>
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


  <script type="text/javascript">
    var g_LCID = <%=Session["LCID"].ToString() %>;
</script>
<body style="background-image: url(../IMAGES/back.png); background-repeat: repeat-x;
    margin: 0px; padding: 0px;"  onkeydown="keyPressed(event.keyCode)" onload = "pageinit()">
    <form id="mainform" runat="server">
    <uc1:TCHeader ID="ucHeader1" runat="server" />
    <asp:Literal ID="litMessage" runat="server" />
    <asp:Panel ID="MainControlsPanel" runat="server" CssClass="MainControlsPanelClass">
        <telerik:RadScriptManager ID="RadScriptManager" runat="server">
        </telerik:RadScriptManager>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false">
        </telerik:RadWindowManager>
         <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" >
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="grdtclist">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdtclist" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>
        <asp:LinkButton ID="btnpostcard" runat="server" CausesValidation="False" OnClientClick = "posttimecards();return false;">
            <asp:PlaceHolder id="PlaceHolder2" runat="server">
                <p align="center" >
                    <asp:Image ID="Image1" runat="server" ImageUrl="../IMAGES/Save.gif" Height="24px" Width="24px" /><br />
                    <label>Post Card</label>
                </p>
            </asp:PlaceHolder>
        </asp:LinkButton>
        <asp:LinkButton ID="btnsetreadytopost" runat="server" CausesValidation="False" OnClientClick = "setreadytopost();return false;">
            <asp:PlaceHolder id="PlaceHolder3" runat="server">
                <p align="center" >
                    <asp:Image ID="Image3" runat="server" ImageUrl="../IMAGES/Save.gif" Height="24px" Width="24px" /><br />
                    <label>Ready Post</label>
                </p>
            </asp:PlaceHolder>
        </asp:LinkButton>
        <asp:LinkButton ID="btndelete" runat="server" CausesValidation="False" OnClientClick ="deletetimecards();return false;">
            <asp:PlaceHolder id="PlaceHolder1" runat="server">
                <p align="center" >
                    <asp:Image ID="Image2" runat="server" ImageUrl="../IMAGES/can.gif" Height="24px" Width="24px" /><br />
                    <label>Delete</label>
                </p>
            </asp:PlaceHolder>
        </asp:LinkButton>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </asp:Panel>

    <asp:HiddenField ID="hidMode" runat="server" />
    <asp:HiddenField ID="HidLabTax1" runat="server" />
    <asp:HiddenField ID="HidLabTax2" runat="server" />
    <asp:HiddenField ID="HidLabSelType" runat="server" />
    <asp:HiddenField ID="HidLabCost" runat="server" />
    <asp:HiddenField ID="HidOrderType" runat="server" />
    <asp:HiddenField ID="HidChargeBack" runat="server" />
    <asp:HiddenField ID="HidFirstName" runat="server" />
    <asp:HiddenField ID="HidLastName" runat="server" />
        <asp:HiddenField ID="hidKeyName" Value="txtcounter" runat="server" />

    </form>
<asp:Literal ID="litScript1" runat="server"></asp:Literal>
</body>
<script type = "text/javascript">

    $(document).ready(function () {
        $("#chkchargeback").val("");
    });

    $(window).load(function () {
        setpanelheight(1);
        initToolbar();
    });

    function pageinit() {
        $(document).ready(function () {
            hidepostbuttons();
        });
    }

    function selectTCType() {
        rebinddatatogrid(0,100,"","");
        //updatecounts();
        hidepostbuttons();
        var grdtclist = $find("<%= grdtclist.ClientID %>");
        var rbl = document.getElementById("rbltctype");
        if ((grdtclist != null) && (rbl != null)) {
            var options = rbl.getElementsByTagName("input");
            var mastertableview = grdtclist.get_masterTableView();
            var columns = mastertableview.get_columns();

            mastertableview.get_filterExpressions().clear();
            mastertableview.get_sortExpressions().clear();

            //cleare filter selection on top of each columns
            for (i = 0; i < columns.length; i++) {
                var colname = columns[i].get_uniqueName();
                mastertableview._updateFilterControlValue("", colname, "NoFilter");
              }
            mastertableview.rebind();

              var MasterTable = grdtclist.get_masterTableView();
              var selectedRows = MasterTable.get_selectedItems();
              if (selectedRows.length >0 ) {
                  for (var i = 0; i < selectedRows.length; i++) {
                      var row = selectedRows[i];
                      row.set_selected(false);
                  }
              }
        }  
    }

    function setworate()
    {
        var txtwonum = document.getElementById("txtwonum");
        var wonum= "";
        if (txtwonum !=null)
        {
            wonum = txtwonum.value;
        }
        if (wonum != ""){
            $.ajax({
                type: "GET",
                url: "../InternalServices/ServiceWO.svc/GetWORate",
                data: { "wonum": wonum },
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (data,status) {
                    var result = data.d;
                    FormSubmitted = false;
                    var txtworate = document.getElementById("txtworate");
                    if (txtworate != null)
                    {
                       txtworate.value = result;
                    }
                },
                error: OnGetMemberError
            });
        }
    }
    
//    function AddSortExpression(grid, fieldName, sortOrder) {
//        var sortExpression = new Telerik.Web.UI.GridSortExpression();
//        sortExpression.set_fieldName(fieldName);
//        sortExpression.set_sortOrder(sortOrder);
//        grid.get_masterTableView()._sortExpressions.add(sortExpression);
//        grid.get_masterTableView()._showSortIconForField(fieldName, sortOrder);
//    }
//    function AddFilterExpression(grid, columnUniqueName, dataField, filterFunction, filterValue) {
//        var filterExpression = new Telerik.Web.UI.GridFilterExpression();
//        var column = grid.get_masterTableView().getColumnByUniqueName(columnUniqueName);
//        column.set_filterFunction("NoFilter");
//        filterExpression.set_fieldName(dataField);
//        filterExpression.set_fieldValue(filterValue);
//        filterExpression.set_filterFunction(filterFunction);
//        filterExpression.set_columnUniqueName(columnUniqueName);
//        grid.get_masterTableView()._updateFilterControlValue(filterValue, columnUniqueName, filterFunction);
//        grid.get_masterTableView()._filterExpressions.add(filterExpression);
//      }

/*
    function roradiobutton(name) {
        var selected = $('#rblinactive input:checked').val()
        $('input:radio[name=' + name + ']')[selected].checked = true;
    }
    */


    function calculatetotal(data)
    {
        var totalhours = 0;
        var totalcost = 0;
        for (i=0;i<data.length; i++)
        {
            totalhours = totalhours*1+data[i].Hours*1;
            totalcost = totalcost *1 + data[i].TotalCost*1;
        }
        var grdtclist = $find("grdtclist");
        var footer = grdtclist.get_masterTableViewFooter();
        var obj = footer.get_element();
        obj.rows[1].cells[getColumnIndexByUniqueName("Hours","grdtclist")].innerHTML =  parseFloat(totalhours).toFixed(2);
        obj.rows[1].cells[getColumnIndexByUniqueName("TotalCost","grdtclist")].innerHTML =  parseFloat(totalcost).toFixed(2);
    }

    function getColumnIndexByUniqueName(columnName,gridid) {
        var masterTableView = $find(gridid).get_masterTableView();
        var column = masterTableView.getColumnByUniqueName(columnName);
        if (column == null)
        {
            alert(columnName);
        }
        var element = column.get_element();
        return element.cellIndex;
    }


    function hidepostbuttons()
    {
        var rbltctype = document.getElementById("rbltctype");
        var btnpostcard = document.getElementById("btnpostcard");
        var btnsetreadytopost = document.getElementById("btnsetreadytopost");

        if (rbltctype != null) {
            var options = rbltctype.getElementsByTagName("input");
            if (options[0].checked) {
                if (btnpostcard!=null)
                  btnpostcard.style.display = "inline";
                if (btnsetreadytopost!=null)
                  btnsetreadytopost.style.display = "inline";
            }
            else if (options[1].checked) {
                if (btnpostcard!=null)
                  btnpostcard.style.display = "none";
                if (btnsetreadytopost!=null)
                  btnsetreadytopost.style.display = "none";
            }
        }
    }

      function trypostback() {
          var txtempid = document.getElementById("txtempid");
          var mode = "<%=querymode %>";
          var txtempname = document.getElementById("txtempname");
          txtempname.value = document.getElementById("HidFirstName").value + " "+ document.getElementById("HidLastName").value;
          if ((txtempid != null) && mode != 'query') {
            if (txtempid.value != '') {
              //alert("<%=Page.IsPostBack %>");
                  //if ("<%=Page.IsPostBack %>"  == "False")
                //__doPostBack("empidchanged", "");
                  rebinddatatogrid(0,100,"","");
              }
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
              document.location.href = "tcmain.aspx?mode=edit&counter=" + counter;
              //__doPostBack("rowclick", counter);
          }
      }


      function posttimecards() {
          var mode = document.getElementById("hidMode").value;
          var grid = $find("<%=grdtclist.ClientID %>");
          var MasterTable = grid.get_masterTableView();
          var selectedRows = MasterTable.get_selectedItems();
          var xmlparam = "<?xml version='1.0' encoding='UTF-8'?><roots><Linkid>";
          if (selectedRows.length < 1) {
              alert('<%=m_msg["T10"] %>');
              return;
          }
          for (var i = 0; i < selectedRows.length; i++) {
              var row = selectedRows[i];
              var counter = row.getDataKeyValue("Counter");
              //alert(cell);
              //var cellval = parseInt(replaceChar(cell.innerHTML, ',', ''));
              
              xmlparam = xmlparam + "<IDS>" + counter + "</IDS>";
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

      function deletetimecards() {
          
          var mode = document.getElementById("hidMode").value;
          var grid = $find("<%=grdtclist.ClientID %>");
          var MasterTable = grid.get_masterTableView();
          var selectedRows = MasterTable.get_selectedItems();
          var isselect = false;
          var selectcounter = document.getElementById("txtcounter").value;
          var xmlparam = "<?xml version='1.0' encoding='UTF-8'?><roots><Linkid>";
          if (selectedRows.length < 1) {
              alert('<%=m_msg["T10"] %>');
              return;
          }
          if (!confirm("Are you sure to delete the timecard record?")) return;
          for (var i = 0; i < selectedRows.length; i++) {
              var row = selectedRows[i];
              var counter = row.getDataKeyValue("Counter");
              xmlparam = xmlparam + "<IDS>" + counter + "</IDS>";
              if (counter == selectcounter)
              {
                 isselect = true;
              }
              else 
              {
                isselect = false;
              }
          }
          xmlparam = xmlparam + "</Linkid>";
          xmlparam = xmlparam + "</roots>";

          if (!allowsubmit()) return;
          $.ajax({
              type: "GET",
              url: "../InternalServices/ServiceLabour.svc/DelWOLabor",
              data: { "paramXML": xmlparam },
              dataType: "json",
              contentType: "application/json; charset=utf-8",
              success: function (data, status) {
                  FormSubmitted = false;
                  OnSuccess(grid);
                  parent.mainmodule.document.location.href = "tcmain.aspx?mode=new";
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
        var gridtclist = $find("<%=grdtclist.ClientID %>");
        var masterTable;
        if (gridtclist != null) {
            masterTable = gridtclist.get_masterTableView();
        }
        if (masterTable != null) {
            masterTable.rebind();
        }
    }

    function updatecounts() {
        var sender = $find("<%=grdtclist.ClientID %>");
        var mode = document.getElementById("hidMode").value;
        if (sender.get_id() == "grdtclist") {
            var counter = "";
            if (document.getElementById("txtcounter") != null)
            { counter = parent.mainmodule.document.getElementById("txtcounter").value; }  
            var index = -1;
            var grid = sender;
            var allRows = grid.get_masterTableView().get_dataItems();
            for (var i = 0; i < allRows.length; i++) {
                    allRows[i].get_element().style.backgroundColor = '';
            }
            for (var i = 0; i < allRows.length; i++) {
                if (allRows[i].getDataKeyValue("ReadyToPost") == "1")
                {
                    allRows[i].get_element().style.backgroundColor = "yellow";
                }
            }
            
            if (counter != "") {
                for (var i = 0; i < allRows.length; i++) {
                    if (allRows[i].getDataKeyValue("Counter") == counter) {
                        allRows[i].get_element().style.backgroundColor = "lime";
                        index = i + 1;  
                        
                    }
                }
            }
        }
    }

    function setChargeBack() {
        var chargeback = document.getElementById('HidChargeBack');
        if (chargeback.value == 1) {
            $('#chkchargeback').attr('checked', 'checked');
        }
        else {
            $('#chkchargeback').removeAttr("checked");
        }
    }

    function keyPressed(code) {
          if (code == 13 && "<%=querymode %>" == "query") {
              LookupClicked();
          }
    }

    function Grid_OnCommand(sender,args) {
        args.set_cancel(true);
        var currentPageIndex = sender.get_masterTableView().get_currentPageIndex();
        var pageSize = sender.get_masterTableView().get_pageSize();
        var sortExpressions = sender.get_masterTableView().get_sortExpressions();
        var filterExpressions = sender.get_masterTableView().get_filterExpressions();



        var sortExpressionsAsSQL = sortExpressions.toString();
        var filterExpressionsAsSQL = filterExpressions.toDynamicLinq();

        var maximumRows = <%= (grdtclist.AllowPaging)? grdtclist.PageSize : grdtclist.Items.Count %>;

        
        rebinddatatogrid(currentPageIndex * pageSize,  maximumRows, sortExpressionsAsSQL, filterExpressionsAsSQL);
       
    }

    function rebinddatatogrid(startRowIndex, maximumRows, sortExpression, filterExpression) {
        var grdtclist = $find("<%= grdtclist.ClientID %>");
        var tableview = grdtclist.get_masterTableView();

        var txtempid = document.getElementById('txtempid');
        var empid ="";
        if (txtempid != null){
            empid = txtempid.value;
            }
        var rbl = document.getElementById("rbltctype");
        var inactive = "1";
        if (rbl != null) {
            var options = rbl.getElementsByTagName("input");
            if (options[0].checked) {
                inactive = "1";
            }
            else if (options[1].checked) {
                inactive = "0";
            }
        }
        //if (empid!=""){empid = 'MBAR1';inactive='0';}
        //alert(empid+" -- "+ inactive+ " -- " + startRowIndex + " -- "+ maximumRows + " -- "+ sortExpression + " -- "+ filterExpression);
        
        if (empid != ""){
            $.ajax({
                type: "GET",
                contentType: "application/json; charset=utf-8",
                url: "../InternalServices/ServiceLabour.svc/ClientLookupTimeCardDataAndCount",
                data: { "empid": empid, "inactive": inactive, "startRowIndex":startRowIndex , "maximumRows": maximumRows, "sortExpression": sortExpression, "filterExpression": filterExpression},
                dataType: "json",
                success: function (result) {
                    tableview.set_dataSource(result.d.Data)
                    //result.count should hold the count of the items.
                    tableview.set_virtualItemCount(result.d.Count);
                    tableview.dataBind();
                    updatecounts();
                    calculatetotal(result.d.Data);
                },
                error: OnGetMemberError
            });
        }
        
      } 
      function setreadytopost()
      {
        var mode = document.getElementById("hidMode").value;
          var grid = $find("<%=grdtclist.ClientID %>");
          var MasterTable = grid.get_masterTableView();
          var selectedRows = MasterTable.get_selectedItems();
          var xmlparam = "<?xml version='1.0' encoding='UTF-8'?><roots><Linkid>";
          if (selectedRows.length < 1) {
              alert('<%=m_msg["T10"] %>');
              return;
          }
          for (var i = 0; i < selectedRows.length; i++) {
              var row = selectedRows[i];
              var counter = row.getDataKeyValue("Counter");
              //alert(cell);
              //var cellval = parseInt(replaceChar(cell.innerHTML, ',', ''));
              
              xmlparam = xmlparam + "<IDS>" + counter + "</IDS>";
          }
          xmlparam = xmlparam + "</Linkid>";
          xmlparam = xmlparam + "</roots>";

          if (!allowsubmit()) return;
          $.ajax({
              type: "GET",
              url: "../InternalServices/ServiceLabour.svc/SetReadytoPostTimeCards",
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
</script>
</html>
