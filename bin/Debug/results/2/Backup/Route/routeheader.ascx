<%@ Control Language="C#" AutoEventWireup="true" CodeFile="routeheader.ascx.cs" Inherits="route_routeheader" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<link type="text/css" href="../styles/mainmenu.css" rel="stylesheet" />
<script type="text/javascript" src="../js/jquery-1.7.2.min.js"></script>
<script type="text/javascript" src="../js/jquery-ui-1.8.21.custom.min.js"></script>
<script type="text/javascript" src="../Jscript/Header.js"></script>
<script type="text/javascript" src="../Jscript/Codes.js"></script>
<script type="text/javascript" src="../Jscript/Validation.js"></script>
<script type="text/javascript" src="../Jscript/StopDoubleClick.js"></script>
<script type="text/javascript">
  
  function openreadingscreen(routename) {
    var URL = "../route/readings.aspx?routename=<%=m_routename %>";
    var dWnd;
    if (GetRadWindow() == null) {
      dWnd = radopen(URL, null);
      dWnd.set_modal(true);
    }
  }

  function openlink(URL, subfolder) {
    var urltype = URL.substr(0, 4);
    var urladdr = URL.substr(5);

    if (urltype == "file")
      URL = "<%=linksrvr%>" + subfolder + "/" + urladdr;
    else
      URL = urladdr;
    newwin = window.open(URL);
    setTimeout('newwin.focus();', 100);
    //  window.close();
  }

  function toolbarclicked(sender, e) {
    var button = e.get_item();
    var commandname = button.get_commandName();
    switch (commandname) {
      case "search":
        if (WarnUser())
          settoQueryMode("routemain.aspx");
        break;
      case "new":
        if (WarnUser())
          window.location.href = 'routemain.aspx?mode=new';
        break;
      case "auto":
        if (WarnUser())
          window.location.href = 'routemain.aspx?mode=new&numbering=auto';
        break;
      case "lookup":
          LookupClicked();
          break;
      case "savequery":
          SaveQuery();
          break;
      case "save":
        if (Page_ClientValidate('')) {
          var mode = $('#hidMode', parent.mainmodule.document).val();
          if (mode == "new") {
            createroute();
          }
          else if (mode == "edit") {
            updateroute();
          }
          else if (mode == "duplicate") {
            duplicateroute();
          }
        }
        break;
      case "duplicate":
        if (WarnUser())
          window.location.href = 'routemain.aspx?mode=duplicate&routename=<%=m_routename %>';
        break;
      case "print":
        PrintForm("<%=defaultprintcounter %>", "RoutePrintForm", "<%=m_routename %>");
        break;
      case "batchprint":
        PrintBatchForms("<%=defaultprintcounter %>", "RoutePrintForm", "RouteName");
        break;
      case "picture":
        break;
      case "linkeddoc":
        LinkDoc('route','<%=m_routename %>');
        break;
      case "reading":
        openreadingscreen("m_routename");
        break;
      case "delete":
        var resp = window.confirm('Are you sure you want to delete this Route?'); //('Are you sure want to delete this PM? ');
        if (resp) {
          noPrompt();
          deleteroute();
        }
        break;
      case "email":
        sendEmail("Route", "<%=m_routename%>");
        break;
     }
  }

  function updateroute() {
    
    $(document).ready(function () {
      var dbfield = "", dbval = "";
      var userid = '<%=Session["Login"] %>';
      var dirtylog = $('#txtdirtylog').val();

      var xml = "<?xml version='1.0' encoding='UTF-8'?><routes><route>";

      xml = xml + collectform("Route");

      $('input:radio[name=rblinactive]:checked').each(function () {
        dbval = $(this).val();
        //if (dbval != '') {
          xml = xml + "<inactive>" + $(this).val() + "</inactive>";
        //}
      });

      if (dirtylog != '') {
        xml = xml + "<dirtylog>" + dirtylog + "</dirtylog>";
      }

      xml = xml + "</route></routes>";
      if (!allowsubmit())
        return;
      $.ajax({
        type: "GET",
        url: "../InternalServices/ServiceRoute.svc/UpdateRoute",
        //url: "../InternalServices/ServicePM.svc/DoWork",
        data: { "xml": xml },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function (xhr, settings) {
          xhr.setRequestHeader("userid", userid);
        },
        

        success: function (data, status) {
        
          var result = data.d;
          var d_array = result.split("^");
          if (d_array[0] == "TRUE") {
            var newdirtylog;
            newdirtylog = parseInt(dirtylog) + 1;
            $('#txtdirtylog').val(newdirtylog);
            savesucceeded(d_array[1]);
            //savesucceeded(d_array[1], status);
          }
          else if (d_array[0] == "FALSE") {
            alert(d_array[1]);
          }
        },
        error: OnGetMemberError
      });

    });

  }

  function createroute() {
    $(document).ready(function () {
      var userid = '<%=Session["Login"] %>';
      var dbfield, dbval;
      var xml = "<?xml version='1.0' encoding='UTF-8'?><routes><route>";
      xml = xml + collectform("Route");
      $('input:radio[name=rblinactive]:checked').each(function () {
        dbval = $(this).val();
        if (dbval != '') {
          xml = xml + "<inactive>" + $(this).val() + "</inactive>";
        }
      });

      xml = xml + "</route></routes>";
      if (!allowsubmit())
        return;
      $.ajax({
        type: "GET",
        url: "../InternalServices/ServiceRoute.svc/CreateRoute",
        data: { "userid": userid, "xml": xml },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, status) {
          createsucceeded(data);
        },
        error: OnGetMemberError
      });

    });
  }

  function duplicateroute() {
    
    $(document).ready(function () {
      var userid = '<%=Session["Login"] %>';
      var dbfield, dbval;
      var xml = "<?xml version='1.0' encoding='UTF-8'?><routes><route>";

      xml = xml + collectform("Route");

      $('input:radio[name=rblinactive]:checked').each(function () {
        dbval = $(this).val();
        if (dbval != '') {
          xml = xml + "<inactive>" + $(this).val() + "</inactive>";
        }
      });

      xml = xml + "</route></routes>";
      if (!allowsubmit())
        return;
      $.ajax({
        type: "GET",
        url: "../InternalServices/ServiceRoute.svc/DuplicateRoute",
        data: { "userid": userid, "xml": xml, "oldroutename": document.getElementById("HidOldRouteName").value },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, status) {
          createsucceeded(data);
        },
        error: OnGetMemberError
      });

    });
  }

  function deleteroute() {
    if (!allowsubmit())
      return;
    $.ajax({
      type: "GET",
      url: "../InternalServices/ServiceRoute.svc/DeleteRoute",
      data: { "userid": "<%=Session["Login"] %>", "routename": "<%= m_routename %>" },
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (data, status) {
        setTimeout(parent.controlpanel.RefreshPanelGridsOnDelete(), 0);
        window.location.href = "routemain.aspx?mode=query";
      },
      error: OnGetMemberError
    });
  }

  function createsucceeded(result) {
    var routename = result.d;
    window.location.href = 'routemain.aspx?mode=edit&routename=' + routename;
  }

  function savesucceeded(routename) {
    window.location.href = 'routemain.aspx?mode=edit&routename=' + routename;
  }

  function OnGetMemberError(request, status, error) {
    alert(status + "--" + error + "--" + request.responseText);
  }

</script>

<asp:Literal ID="litFrameScript" runat="server" />
<asp:Literal ID="litDateLCIDScript" runat="server" />

<div id="header-wrap">
  <div style="position:fixed; width:50px; height:32px; top:0px; left:10px; z-index:500;">
    <a href="javascript:ModuleMenu();" onmouseover="imageChangeover('imgMainMenu','../IMAGES/tablogo_over.png')" onmouseout="imageChangeover('imgMainMenu','../IMAGES/tablogo.png')"><img id="imgMainMenu" src="../IMAGES/tablogo.png" width="46" height="35" style=" position:relative; top:-3px; padding:0 20px 0 0;" border="0" alt="" title="" /></a>
  </div>

  <div>
  <ul id="topmainmenu_table" class="topmainmenu_menulist" style="display:block; width:100%; height:30px; position:relative; padding:0 0 0 10px; margin:0px; background-image:url(../IMAGES/tabback.png); background-repeat:repeat-x; z-index:3;">
    <li>
      <img id="topmainmenu_1" src="../IMAGES/tablogo.png" width="46" height="35" style=" position:relative; top:-40px; padding:0 20px 0 0;" border="0" alt="" title="" />
    </li>
    <li>
      <asp:HyperLink ID="tabhlkMain" runat="server">
      <asp:Image ID="tabimgMain" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px" Width="70px" BorderStyle="None" BorderWidth="0" style="vertical-align:bottom;" />
      <asp:Label ID="tablblMain" runat="server" CssClass="toptabout" Text="Main" /></asp:HyperLink>
    </li>
  </ul>
  </div>
  <br /><br />
  <div style="background-image:url(../IMAGES/toolbarback.png); background-repeat:repeat-x;position:relative; top:-28px; display:block; width:100%; height:64px; padding:5px 0 0 20px; vertical-align: top; z-index:5;">
  <telerik:RadToolBar ID="toolbar" runat="server" ></telerik:RadToolBar>
  </div>
  <div style="position:relative; top:-34px; display:block; padding:0px; margin:0px; width:100%; height:30px; background-image:url(../IMAGES/statbar_back.png); background-repeat:repeat-x; z-index:5;">
        <table width="100%">
            <tr valign="middle">
                <td width="20%" style="font-family: Arial, Helvetica, sans-serif; font-size: 8pt;
                    color: #FFFFFF;" valign="middle">
                    Current Operation:
                    <span id="spnoperation"><asp:Literal ID="litOperation" runat="server">Query</asp:Literal></span>
                </td>
                <td width="30%" style="font-family: Arial, Helvetica, sans-serif; font-size: 8pt;
                    color: #FFFFFF;" valign="middle">
                    <asp:PlaceHolder ID="plhLinkDoc" runat="server" />
                </td>
                <td width="30%" style="font-family: Arial, Helvetica, sans-serif; font-size: 8pt;
                    color: #FFFFFF;" valign="middle">
                    <asp:PlaceHolder ID="plhReports" runat="server" />
                </td>
                <td width="20%" style="font-family: Arial, Helvetica, sans-serif; font-size: 8pt;
                    color: #FFFFFF;" valign="middle">
                    <asp:PlaceHolder ID="plhQuery" runat="server" />
                </td>
            </tr>
        </table>
  </div>
    <asp:HiddenField ID="hidCurrentRecord" runat="server" />
</div>
