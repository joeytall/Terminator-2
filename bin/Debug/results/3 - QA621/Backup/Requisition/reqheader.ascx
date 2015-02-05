<%@ Control Language="C#" AutoEventWireup="true" CodeFile="reqheader.ascx.cs" Inherits="requisition_reqheader" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<link type="text/css" href="../styles/mainmenu.css" rel="stylesheet" />
<script type="text/javascript" src="../js/jquery-1.7.2.min.js"></script>
  <script type="text/javascript" src="../js/jquery-ui-1.8.21.custom.min.js"></script>
<script type="text/javascript" src="../Jscript/Header.js"></script>
<script type="text/javascript" src="../Jscript/Codes.js"></script>
<script type="text/javascript" src="../Jscript/Validation.js"></script>
<script type="text/javascript" src="../Jscript/StopDoubleClick.js"></script>
<script type="text/javascript">

    $(document).ready(function () {
        if ("<%=m_iskeyrequest%>" == "1" && "<%=m_mode%>" == "edit")
            $("#liforkey").show();
        else
            $("#liforkey").hide();
    });

  function openlink(URL, subfolder) {
    var urltype = URL.substr(0, 4);
    var urladdr = URL.substr(5);

    if (urltype == "file")
      URL = "<%=linksrvr%>" + subfolder + "/" + urladdr;
    else
      URL = urladdr;
    newwin = window.open(URL);
    setTimeout('newwin.focus();', 100);
  }

  function toolbarclicked(sender, e) {
    var button = e.get_item();
    var commandname = button.get_commandName();
    switch (commandname) {
        case "search":
            if (WarnUser())
                settoQueryMode("reqmain.aspx");
            break;
        case "new":
            if (WarnUser())
                window.location.href = 'reqmain.aspx?mode=new&numbering=manual';
            break;
        case "autoreq":
            if (WarnUser())
                window.location.href = 'reqmain.aspx?mode=new&numbering=auto';
            break;
        case "lookup":
            LookupClicked();
            break;
        case "save":
            noPrompt();
            if (Page_ClientValidate())
            {
                var mode = $('#hidMode', parent.mainmodule.document).val();
                if (mode == "new") {
                    createrequest();
                }
                else if (mode == "edit") {
                    updaterequest();
                }
            }
            break;
      case "print":
        PrintForm("<%=defaultprintcounter %>", "REQPrintForm", "<%=m_reqnum %>");
        break;
      case "batchprint":
        PrintBatchForms("<%=defaultprintcounter %>", "REQPrintForm", "ReqNum");
        break;
      case "linkeddoc":
        LinkDoc('requisition', '<%=m_reqnum %>');
        break;
      case "delete":
        var resp = window.confirm('<%=m_msg["T1"] %>');
        //'Are you sure want to delete this location? If Deleted, all child locations will not reference this location as parent.');
        if (resp) {
          noPrompt();
          __doPostBack("delete", "");
        }
        break;
      case "status":
        changeStatus();
        break;
      case "email":
        sendEmail("Requisition", "<%=m_reqnum%>");
          break;
      case "savequery":
          SaveQuery();
          break;
  }

    function changeStatus() {
      var reqnum = "<%=m_reqnum %>";
      var oWnd;
      if (GetRadWindow() == null) {
        oWnd = radopen('reqstatus.aspx?reqnum=' + reqnum, null);
        oWnd.set_modal(true);
      }
    }
  }

  function createrequest() {
    var userid = '<%=Session["Login"] %>';
    var dbfield, dbval;
    var reqxml = "<?xml version='1.0' encoding='UTF-8'?><requests><request>";
    reqxml = reqxml + collectformall("ItemRequest");
    reqxml = reqxml + "</request></requests>";
    if (!allowsubmit())
        return;
    $.ajax({
        type: "GET",
        url: "../InternalServices/ServiceReq.svc/CreateRequest",
        data: { "userid": userid, "xmlnvc": reqxml },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, status) {
            createsucceeded(data);
        },
        error: OnGetMemberError
    });
  }

    function updaterequest() {
      var dbfield = "", dbval = "";
      var userid = '<%=Session["Login"] %>';
      var dirtylog = $('#txtdirtylog').val();
      var reqxml = "<?xml version='1.0' encoding='UTF-8'?><requests><request>";
      reqxml = reqxml + collectformall("ItemRequest");
      reqxml = reqxml + "</request></requests>";
      if (!allowsubmit())
        return;
      $.ajax({
          type: "GET",
          url: "../InternalServices/ServiceReq.svc/UpdateRequest",
          data: { "xmlnvc": reqxml },
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
              }
              else if (d_array[0] == "FALSE") {
                  alert(d_array[1]);
              }
          },
          error: OnGetMemberError
      });
    }


  function createsucceeded(result) {
      var reqnum = result.d;
      var iskeyrequest = $('input[name=rbliskeyrequest]:checked').val();
      if (iskeyrequest == 1)
          window.location.href = 'keyrequest.aspx?reqnum=' + reqnum;
      else
        window.location.href = 'reqmain.aspx?mode=edit&reqnum=' + reqnum;
  }

  function savesucceeded(reqnum) {
      if ("<%=m_tabname%>" == "keyrequest")
        window.location.href = 'keyrequest.aspx?reqnum=' + reqnum;
      else
        window.location.href = 'reqmain.aspx?mode=edit&reqnum=' + reqnum;
  }

  function OnGetMemberError(request, status, error) {
      alert(status + "--" + error + "--" + request.responseText);
  }
</script>

<asp:Literal ID="litFrameScript" runat="server" />
<asp:Literal ID="litDateLCIDScript" runat="server" />
<div id="header-wrap">
  <div style="position: fixed; width: 50px; height: 32px; top: 0px; left: 10px; z-index: 500;">
    <a href="javascript:ModuleMenu();" onmouseover="imageChangeover('imgMainMenu','../IMAGES/tablogo_over.png')"
      onmouseout="imageChangeover('imgMainMenu','../IMAGES/tablogo.png')">
      <img id="imgMainMenu" src="../IMAGES/tablogo.png" width="46" height="35" style="position: relative;
        top: -3px; padding: 0 20px 0 0;" border="0" alt="" title="" /></a>
  </div>
  <div>
    <ul id="topmainmenu_table" class="topmainmenu_menulist" style="display: block; width: 100%;
      height: 30px; position: relative; padding: 0 0 0 10px; margin: 0px; background-image: url(../IMAGES/tabback.png);
      background-repeat: repeat-x; z-index: 3;">
      <li>
        <img id="topmainmenu_1" src="../IMAGES/tablogo.png" width="46" height="35" style="position: relative;
          top: -40px; padding: 0 20px 0 0;" border="0" alt="" title="" />
      </li>
      <li>
        <asp:HyperLink ID="tabhlkMain" runat="server">
          <asp:Image ID="tabimgMain" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
            Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
          <asp:Label ID="tablblMain" runat="server" CssClass="toptabout" Text="Main" /></asp:HyperLink>
      </li>
      <li id="liforkey">
        <asp:HyperLink ID="tabhlkKey" runat="server">
          <asp:Image ID="tabimgKey" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
            Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
          <asp:Label ID="tablblKey" runat="server" CssClass="toptabout" Text="Key" /></asp:HyperLink>
      </li>
      <li>
        <asp:HyperLink ID="tabhlkAccounts" runat="server">
          <asp:Image ID="tabimgAccounts" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
            Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
          <asp:Label ID="tablblAccounts" runat="server" CssClass="toptabout" Text="Accounts" /></asp:HyperLink>
      </li>
      <li>
        <asp:HyperLink ID="tabhlkHistory" runat="server">
          <asp:Image ID="tabimgHistory" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
            Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
          <asp:Label ID="tablblHistory" runat="server" CssClass="toptabout" Text="Status History" /></asp:HyperLink>
      </li>
    </ul>
  </div>
  <br />
  <br />
  <div style="background-image: url(../IMAGES/toolbarback.png); background-repeat: repeat-x;
    position: relative; top: -28px; display: block; width: 100%; height: 64px; padding: 5px 0 0 20px;
    vertical-align: top; z-index: 5;">
    <telerik:RadToolBar ID="toolbar" runat="server">
    </telerik:RadToolBar>
  </div>
  <div style="position: relative; top: -34px; display: block; padding: 0px; margin: 0px;
    width: 100%; height: 30px; background-image: url(../IMAGES/statbar_back.png); background-repeat: repeat-x;
    z-index: 5;">
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
