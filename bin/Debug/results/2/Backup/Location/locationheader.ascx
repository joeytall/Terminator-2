<%@ Control Language="C#" AutoEventWireup="true" CodeFile="locationheader.ascx.cs" Inherits="location_locationheader" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<link type="text/css" href="../styles/mainmenu.css" rel="stylesheet" />
<script type="text/javascript" src="../Jscript/Header.js"></script>
<script type="text/javascript" src="../Jscript/Codes.js"></script>
<script type="text/javascript" src="../Jscript/Validation.js"></script>
<script type="text/javascript" src="../Jscript/StopDoubleClick.js"></script>
<script type="text/javascript">
    
  function showmap() {
    var URL = "../codes/map.aspx?location=<%=m_location %>";
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

  function modifylocation(loc) {
    var dWnd;
    dWnd = radopen('locationmodify.aspx?location=' + loc, null);
    dWnd.set_modal(true);
  }

  function toolbarclicked(sender, e) {
    var button = e.get_item();
    var commandname = button.get_commandName();
    switch (commandname) {
      case "search":
        if (WarnUser())
          settoQueryMode("locationmain.aspx");
        break;
      case "new":
        if (WarnUser())
          window.location.href = 'locationmain.aspx?mode=new';
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
          if (mode == "new" || mode == "duplicate") {
            createlocation();
          }
          else if (mode == "edit") {
            savelocation();
          }
        }
        break;
      
      case "duplicate":
        if (WarnUser())
          window.location.href = 'locationmain.aspx?mode=duplicate&location=<%=m_location %>';
        break;
      case "print":
        PrintForm("<%=defaultprintcounter %>", "LocationPrintForm", "<%=m_location %>");
        break;
    case "batchprint":
        PrintBatchForms("<%=defaultprintcounter %>", "LocationPrintForm", "Location");
        break;
      case "specsearch":
        break;
      case "picture":
        break;
      case "map":
        showmap();
        break;
      case "linkeddoc":
        LinkDoc('location','<%=m_location %>');
        break;
      case "delete":
        if (getvalue("hidDeleteMessage") == "") {
          var resp = window.confirm('<%=m_msg["T1"] %>');
          //'Are you sure want to delete this location? If Deleted, all child locations will not reference this location as parent.');
          if (resp) {
            noPrompt();
            __doPostBack("delete", "");
          }
        }
        else
          alert(getvalue("hidDeleteMessage"));

        break;
      case "modify":
        modifylocation('<%=m_location %>');
        break;
      case "template":
        URL = "../util/specificationsframe.aspx?linktype=location&id=<%=m_location %>";
        //URL = "../util/specificationsframe.aspx";
        var labWnd = radopen(URL, null);
        labWnd.set_modal(true);
        return false;
        break;
      case "email":
          sendEmail("Location", "<%=m_location%>");
          break;
    }

}


  function savelocation() {
    $(document).ready(function () {
      var location = "";
      var userid = '<%=Session["Login"] %>';
      var dirtylog = $('#txtdirtylog').val();

      var locxml = "<?xml version='1.0' encoding='UTF-8'?><locations><location>";
      locxml = locxml + collectformall("Location")

      locxml = locxml + "</location></locations>";
      if (!allowsubmit())
        return;
      $.ajax({
        type: "GET",
        url: "../InternalServices/ServiceLoc.svc/SaveLocation",
        data: { "xmlnvc": locxml },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function (xhr, settings) {
          xhr.setRequestHeader("userid", userid);
        },
        success: function (data, status) {
          var result = data.d;
          var d_array = result.split("^");
          if (d_array[0] == "TRUE") {
            var newdirtylog
            newdirtylog = parseInt(dirtylog) + 1;

            $('#txtdirtylog').val(newdirtylog);
            location = d_array[1];
            window.location.href = 'locationmain.aspx?mode=edit&location=' + location;
          }
          else if (d_array[0] == "FALSE") {
            alert(d_array[1]);
          }
        },
        error: OnGetMemberError
      });
    });
  }

  function createlocation() {
      var userid = '<%=Session["Login"] %>';
      var locxml = "<?xml version='1.0' encoding='UTF-8'?><locations><location>";
      locxml = locxml + collectformall("Location");
      locxml = locxml + "</location></locations>";
      var oldlocation = "";
      var mode = $('#hidMode', parent.mainmodule.document).val();
      if (mode == "duplicate")
        oldlocation = getvalue("HidOldLocation");
      if (!allowsubmit())
        return;
      $.ajax({
        type: "GET",
        url: "../InternalServices/ServiceLoc.svc/CreateLocation",
        data: { "userid": userid, "xmlnvc": locxml,"oldlocation":oldlocation },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, status) {
          var location = data.d;
          window.location.href = 'locationmain.aspx?mode=edit&location=' + location;
        },
        error: OnGetMemberError
      });
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
    <li>
      <asp:HyperLink ID="tabhlkSpecs" runat="server">
      <asp:Image ID="tabimgSpecs" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px" Width="70px" BorderStyle="None" BorderWidth="0" style="vertical-align:bottom;" />
      <asp:Label ID="tablblSpecs" runat="server" CssClass="toptabout" Text="Additional Info" /></asp:HyperLink>
    </li>
    <!--
    <li>
      <asp:HyperLink ID="tabhlkEQList" runat="server">
      <asp:Image ID="tabimgEQList" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px" Width="70px" BorderStyle="None" BorderWidth="0" style="vertical-align:bottom;" />
      <asp:Label ID="tablblEQList" runat="server" CssClass="toptabout" Text="Equipment List" /></asp:HyperLink>
    </li>
    -->
    <li>
      <asp:HyperLink ID="tabhlkHierarchy" runat="server">
      <asp:Image ID="tabimgHierarchy" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px" Width="70px" BorderStyle="None" BorderWidth="0" style="vertical-align:bottom;" />
      <asp:Label ID="tablblHierarchy" runat="server" CssClass="toptabout" Text="Hierarchy" /></asp:HyperLink>
    </li>

    <li>
      <asp:HyperLink ID="tabhlkHistory" runat="server">
      <asp:Image ID="tabimgHistory" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px" Width="70px" BorderStyle="None" BorderWidth="0" style="vertical-align:bottom;" />
      <asp:Label ID="tablblHistory" runat="server" CssClass="toptabout" Text="History" /></asp:HyperLink>
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
