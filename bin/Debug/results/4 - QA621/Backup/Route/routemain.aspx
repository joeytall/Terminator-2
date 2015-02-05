<%@ Page Language="C#" AutoEventWireup="true" CodeFile="routemain.aspx.cs" Inherits="route_routemain"
    MaintainScrollPositionOnPostback="true" %>

<%@ Register TagPrefix="uc1" TagName="RouteHeader" Src="routeheader.ascx" %>
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
        $(document).ready(function () {
          var pmnum = $('#txtpdmnum').val();
          window.location.href = 'pdmmain.aspx?mode=duplicate&pdmnum=' + pdmnum;
        });
      } 

      function clearnonduplicatefields() {
        $(document).ready(function () {
          $('input:radio[name=rdbinactive][value=0]').click();
        });
      }

      

    </script>
    <uc1:RouteHeader ID="ucHeader1" runat="server" />
    <asp:Panel ID="MainControlsPanel" runat="server"  CssClass="MainControlsPanelClass">
        <telerik:RadScriptManager ID="RadScriptManager" runat="server">
        </telerik:RadScriptManager>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false">
        </telerik:RadWindowManager>
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
            <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="grddetail">
              <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="grddetail" />
                <telerik:AjaxUpdatedControl ControlID="grdavailable" />
              </UpdatedControls>
            </telerik:AjaxSetting>
            </AjaxSettings>
            <ClientEvents OnRequestStart="RequestStart" OnResponseEnd="ResponseEnd" />
        </telerik:RadAjaxManager>

        <asp:LinkButton ID="btnadd" runat="server" CausesValidation="False" OnClientClick="adddetail();return false;" >
          <asp:PlaceHolder runat="server">
          <asp:Image ID="Image4" runat="server" ImageUrl="../images/equipment/add_meter_32.png" Height="24px"
            Width="24px" /><br />
          <label>Add to Route</label>
          </asp:PlaceHolder>
        </asp:LinkButton>
        <asp:LinkButton ID="btnremove" runat="server" CausesValidation="False" OnClientClick="removedetail();return false;">
          <asp:PlaceHolder ID="PlaceHolder1" runat="server">
          <asp:Image ID="Image1" runat="server" ImageUrl="../images/del.gif" Height="24px"
            Width="24px" /><br />
          <label>Remove</label>
          </asp:PlaceHolder>
        </asp:LinkButton>
        


        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </asp:Panel>
    <asp:HiddenField ID="hidMode" runat="server" />
    <asp:HiddenField ID="HidOldRouteName" runat="server" /> 
    <asp:HiddenField ID="HidDivision" runat="server" />
    <input type="hidden" id="hidDataChanged" value="0" />
    <input type="hidden" id="hidNoPromt" value="0" />
    <input type="hidden" id="hidempid" value="<%=Session["Login"].ToString() %>" />
        <asp:HiddenField ID="hidKeyName" Value="txtroutename" runat="server" />
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

      function rowClicked(sender, eventArgs) {
        var counter=eventArgs.getDataKeyValue("Counter");
        var URL = "detailorder.aspx?counter=" + counter;
        var dWnd;
        dWnd = radopen(URL, null);
        dWnd.set_modal(true);
      }

      function refreshGrid()
      {
        var grid=$find("grddetail");
        grid.get_masterTableView().rebind();
        var grid2=$find("grdavailable");
        grid2.get_masterTableView().rebind();
      }

     function detaildatabind(sender, eventArgs) {
       //alert("in");
      }
      /*
      function detaildatabind(sender, eventArgs) {
        alert("in");
        var grid = $find("<%= grddetail %>");
        if (grid != null)
        {
          var tableview = grddetail.get_masterTableView();
          //var txtempid = document.getElementById('txtempid');
          //alert(txtempid.value);
          var routename = "<%=m_routename %>";
          alert(routename);
          $.ajax({
              type: "POST",
              contentType: "application/json; charset=utf-8",
              url: "<%= VirtualPathUtility.ToAbsolute("~/InternalServices/ServiceRoute.svc/GetRouteDetail?routename=" + m_routename)%>",
              data:{"startRowIndex":"0","maximumRows":"100","sortExpression":"","filterExpression":""},
              dataType: "json",
              success: function (result) {
                  tableview.set_dataSource(result.d.Data)
                  tableview.dataBind();
                  tableview.set_virtualItemCount(result.d.Count);
              },
              error: OnGetMemberError
          });
        }
      }
      */

      

      function adddetail() {
        var grid = $find("grdavailable");
        var masterview = grid.get_masterTableView();
        var selectedrows = masterview.get_selectedItems();
        if (selectedrows.length == 0) {
          alert("No record is selected.")
          return;
        }
        var i = 0;
        var xml = ""
        for (i = 0; i<selectedrows.length;i++)
        {
          
          var item = selectedrows[i];
          
          var readingtype = item.get_cell("ReadingType").innerHTML;
          
          var equipment = item.get_cell("Equipment").innerHTML;
          
          var spectag = item.get_cell("Attribute").innerHTML;
          

          xml = xml + "<Detail>" + "<ReadingType>" + readingtype + "</ReadingType>" +
                      "<Equipment>" + equipment + "</Equipment>" +
                      "<SpecTag>" + spectag + "</SpecTag>" + "</Detail>";

        }
        xml = "<?xml version='1.0' encoding='UTF-8'?><Details>" + xml + "</Details>";
        if (!allowsubmit()) return;

        $.ajax({
          type: "GET",
          url: "../InternalServices/ServiceRoute.svc/AddRouteDetail",
          data: { "routename":"<%=m_routename %>", "xml": xml },
          contentType: "application/json; charset=utf-8",
          dataType: "json",
          beforeSend: function (xhr, settings) {
            xhr.setRequestHeader("userid", "<%=Session["Login"].ToString() %>");},
          success: function (data, status) {
            if (data.d == "OK")
            {
              document.location.href = "routemain.aspx?mode=edit&routename=<%=m_routename %>";
              refreshGrid();
            }
            else
            {
              FormSubmitted = false;
              alert(data.d);
            }
            
          },
          error: OnGetMemberError
        });

      }

      function removedetail() {
        var grid = $find("grddetail");
        var masterview = grid.get_masterTableView();
        var selectedrows = masterview.get_selectedItems();
        if (selectedrows.length == 0) {
          alert("No record is selected.")
          return;
        }
        var i = 0;
        var counterlist = ""
        for (i = 0; i<selectedrows.length;i++)
        {
          var item = selectedrows[i];
          var counter = item.get_dataItem().Counter;
          if (counterlist == "")
            counterlist = counter;
          else
            counterlist = counterlist + "," + counter;

        }
        
        if (!allowsubmit()) return;

        $.ajax({
          type: "GET",
          url: "../InternalServices/ServiceRoute.svc/RemoveRouteDetail",
          data: { "counterlist": counterlist },
          contentType: "application/json; charset=utf-8",
          dataType: "json",
          beforeSend: function (xhr, settings) {
            xhr.setRequestHeader("userid", "<%=Session["Login"].ToString() %>");},
          success: function (data, status) {
            if (data.d == "OK")
            {
              document.location.href = "routemain.aspx?mode=edit&routename=<%=m_routename %>";
                           //refreshGrid();
            }
            else
            {
              FormSubmitted = false;
              alert(data.d);
            }
            
          },
          error: OnGetMemberError
        });

      }

      function OnGetMemberError(request, status, error) {
        alert(status + "--" + error + "--" + request.responseText);
      }

</script>
</html>

