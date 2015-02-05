<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Procheader.ascx.cs" Inherits="Proc_Procheader" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<link type="text/css" href="../styles/mainmenu.css" rel="stylesheet" />
<script type="text/javascript" src="../Jscript/Header.js"></script>
<script type="text/javascript" src="../Jscript/Codes.js"></script>
<script type="text/javascript" src="../Jscript/Validation.js"></script>
<script type="text/javascript" src="../Jscript/StopDoubleClick.js"></script>
<script type="text/javascript">

  function duplicateProcedure() {
    var obj = document.getElementById("txtprocnum");
    var procnum = obj.value;
    var dWnd;
    if (GetRadWindow() == null) {
      dWnd = radopen('Procduplicate.aspx?procnum=' + procnum, null);
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
  }

  function tasklibrary() {
    var dWnd = radopen("../codes/tasklist.aspx?referer=TaskLibrary", null);
    dWnd.set_modal(true);
    return false;
  }

  function toolbarclicked(sender, eventArgs) {
    var button = eventArgs.get_item();
    var commandname = button.get_commandName();
    switch (commandname) {
      case "search":
        if (WarnUser())
          settoQueryMode("procmain.aspx");
        break;
      case "newproc":
        if (WarnUser())
          window.location.href = 'procmain.aspx?mode=new&numbering=manual';
        break;
      case "autoproc":
        if (WarnUser())
          window.location.href = 'procmain.aspx?mode=new&numbering=auto';
        break;
      case "lookup":
          LookupClicked();
          break;
      case "savequery":
          SaveQuery();
          break;
      case "save":
          if (checksave()) {
          if (Page_ClientValidate('')) {
            var mode = $('#hidMode', parent.mainmodule.document).val();
            if (mode == "new") {
              createprocedure();
            }
            else if (mode == "edit") {
              saveprocedure();
            }
          }
        }
        break;
      case "duplicate":
        duplicateProcedure();
        break;
      case "print":
        PrintForm("<%=defaultprintcounter %>", "ProcPrintForm", "<%=m_procnum %>");
        break;
      case "batchprint":
        PrintBatchForms("<%=defaultprintcounter %>", "ProcPrintForm", "ProcNum");
        break;
      case "email":
        sendEmail("Procedure", "<%=m_procnum%>");
        break;
      case "picture":
        break;
      case "linkeddoc":
        LinkDoc('procedure', '<%=m_procnum %>');
        break;
      case "tasklibrary":
        tasklibrary();
        break;
      case "delete":
        var resp = window.confirm('<%=m_msg["T2"] %>');
        //Are you sure you want to delete this procedure and all associated resources?
        if (resp) {
          noPrompt();
          __doPostBack("delete", "");
        }
        break;
    }
  }

  function setSelectedIndex() {
    var combo = $find("<%= toolbarcombobox.ClientID %>");
    combo.trackChanges();
    combo.get_items().getItem(0).select();
    combo.updateClientState();
    combo.commitChanges();
    if (combo.get_dropDownVisible())
      combo.hideDropDown();
    combo.get_inputDomElement().blur();
    combo._raiseClientBlur(e);
    combo._focused = false;
  }

</script>
<script type="text/javascript">

  function OnClientSelectedIndexChanged(sender, eventArgs) {
    var item = eventArgs.get_item();
    alert("You selected: " + item.get_text());
  }

  function checksave() {

    if ("<%=m_mode %>" == "edit") {

      var proc = document.getElementById("txtprocnum").value;
      var oldproc = document.getElementById("hidprocnum").value;

      if ((proc != "") && (proc != oldproc)) {
        //resp = window.confirm("Procedure has been changed. By clicking OK, you will replace all estimated resources with those from the new procedure.\n\rClick Cancel to abot the operation.")
        resp = window.confirm('<%=m_msg["T1"] %>')
        if (resp) {
          noPrompt();
          return true;
        }
        else
          return false;
      }
      else {
        return true;
      }
    }
    else {
      noPrompt();
      return true;
    }
  }

</script>
<script type="text/javascript">
  function saveprocedure() {
      var dbfield = "", dbval = "", procnum = "";
      var userid = '<%=Session["Login"] %>';
      var hidprocnum = $('input:hidden[name=hidprocnum]').val().toLowerCase();
      var dirtylog = $('#txtdirtylog').val();

      var procxml = "<?xml version='1.0' encoding='UTF-8'?><proceduress><procedures>";
      if (dirtylog != '')
        procxml = procxml + "<dirtylog>" + dirtylog + "</dirtylog>";
      procxml = procxml + collectformall("Procedures")

      procxml = procxml + "</procedures></proceduress>";
      if (!allowsubmit())
        return;
      $.ajax({
        type: "GET",
        url: "../InternalServices/ServiceProc.svc/SaveProcedure",
        data: { "xmlnvc": procxml },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function (xhr, settings) {
          xhr.setRequestHeader("userid", userid);
          xhr.setRequestHeader("hidprocnum", hidprocnum);
        },
        success: function (data, status) {
          var result = data.d;
          var d_array = result.split("^");
          if (d_array[0] == "TRUE") {
            var newdirtylog
            newdirtylog = parseInt(dirtylog) + 1;

            $('#txtdirtylog').val(newdirtylog);

            if (hidprocnum != procnum) {
              $('input:hidden[name=hidprocnum]').val(procnum);
            }
            savesucceeded(d_array[1]);
          }
          else if (d_array[0] == "FALSE") {
            alert(d_array[1]);
          }
        },
        error: OnGetMemberError
    });
  }

  function createprocedure() {
      var userid = '<%=Session["Login"] %>';
      var dbfield, dbval;
      var procxml = "<?xml version='1.0' encoding='UTF-8'?><proceduress><procedures>";
      procxml = procxml + collectformall("Procedures");

      procxml = procxml + "</procedures></proceduress>";

      if (!allowsubmit())
        return;
      $.ajax({
        type: "GET",
        url: "../InternalServices/ServiceProc.svc/CreateProcedure",
        data: { "userid": userid, "xmlnvc": procxml },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, status) {
          createsucceeded(data);
        },
        error: OnGetMemberError
      });
  }

  function createsucceeded(result) {
    var procnum = result.d;
    window.location.href = 'Procmain.aspx?mode=edit&procnum=' + procnum;
  }

  function savesucceeded(procnum) {
    window.location.href = 'Procmain.aspx?mode=edit&procnum=' + procnum;
  }

  function succeeded(procnum) {
    window.location.href = 'Procmain.aspx?mode=edit&wonum=' + procnum;
  }

  function OnGetMemberError(request, status, error) {
    alert(status + "--" + error + "--" + request.responseText);
  }

  function gridrefresh() {
    masterTable = $find("grdproclabourest").get_masterTableView();
    masterTable.rebind();
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
      <li>
        <asp:HyperLink ID="tabhlkAccounts" runat="server">
          <asp:Image ID="tabimgAccounts" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
            Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
          <asp:Label ID="tablblAccounts" runat="server" CssClass="toptabout" Text="Accounts" /></asp:HyperLink>
      </li>
      <!--
      <li>
        <asp:HyperLink ID="tabhlkTaskLibrary" runat="server">
          <asp:Image ID="tabimgTaskLibrary" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
            Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
          <asp:Label ID="tablblTaskLibrary" runat="server" CssClass="toptabout" Text="Task Library" /></asp:HyperLink>
      </li>
      -->
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
              <td width="20%" style="font-family: Arial, Helvetica, sans-serif; font-size: 8pt; color: #FFFFFF;"
                  valign="middle">Current Operation:
                    <asp:Literal ID="litOperation" runat="server">Query</asp:Literal>
              </td>
              <td width="30%" style="font-family: Arial, Helvetica, sans-serif; font-size: 8pt; color: #FFFFFF;"
                  valign="middle">
                  <asp:PlaceHolder ID="plhLinkDoc" runat="server" />
              </td>
              <td width="30%" style="font-family: Arial, Helvetica, sans-serif; font-size: 8pt; color: #FFFFFF;"
                  valign="middle">
                  <asp:PlaceHolder ID="plhReports" runat="server" />
              </td>
              <td width="20%" style="font-family: Arial, Helvetica, sans-serif; font-size: 8pt; color: #FFFFFF;"
                  valign="middle">
                  <asp:PlaceHolder ID="plhQuery" runat="server" />
              </td>
          </tr>
      </table>
  </div>
  <asp:HiddenField ID="hidCurrentRecord" runat="server" />
</div>
