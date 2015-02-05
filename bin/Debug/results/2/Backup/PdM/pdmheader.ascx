<%@ Control Language="C#" AutoEventWireup="true" CodeFile="pdmheader.ascx.cs" Inherits="pdm_pdmheader" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<link type="text/css" href="../styles/mainmenu.css" rel="stylesheet" />
<script type="text/javascript" src="../js/jquery-1.7.2.min.js"></script>
<script type="text/javascript" src="../js/jquery-ui-1.8.21.custom.min.js"></script>
<script type="text/javascript" src="../Jscript/Header.js"></script>
<script type="text/javascript" src="../Jscript/Codes.js"></script>
<script type="text/javascript" src="../Jscript/Validation.js"></script>
<script type="text/javascript" src="../Jscript/StopDoubleClick.js"></script>
<script type="text/javascript">
  
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

  function managelink() {
    return false;
  }

  function validatelimitwarning() {
      var result = true;
      var highlimit = document.getElementById("txthighlimit").value;
      var highwarning = document.getElementById("txthighwarning").value;
      var lowlimit = document.getElementById("txtlowlimit").value;
      var lowwarning = document.getElementById("txtlowwarning").value;
      if (highlimit != "") {
          if (highwarning != "") {
              if (highlimit * 1 <= highwarning * 1) {
                  result = false;
              }
          }
          if (lowlimit != "") {
              if (highlimit * 1 <= lowlimit * 1) {
                  result = false;
              }
          }
          if (lowwarning != "") {
              if (highlimit * 1 < lowwarning * 1) {
                  result = false;
              }
          }
      }
      if (highwarning != "") { 
          if (lowlimit != "") {
              if (highwarning * 1 <= lowlimit * 1) {
                  result = false;
              }
          }
          if (lowwarning != "") {
              if (highwarning * 1 < lowwarning * 1) {
                  result = false;
              }
          }
      }
      if (lowwarning != "") {
          if (lowlimit != "") {
              if (lowwarning * 1 <= lowlimit * 1) {
                  result = false;
              }
          }
      }
      if (!result)
      {
          alert("Invalid Limits and Warnings");
      }

      return result;
  }

  function toolbarclicked(sender, e) {
    var button = e.get_item();
    var commandname = button.get_commandName();
    switch (commandname) {
      case "search":
        if (WarnUser())
          settoQueryMode("pdmmain.aspx");
        break;
      case "new":
        if (WarnUser())
          window.location.href = 'pdmmain.aspx?mode=new';
        break;
      case "autopdm":
        if (WarnUser())
          window.location.href = 'pdmmain.aspx?mode=new&numbering=auto';
        break;
      case "lookup":
          LookupClicked();
          break;
      case "savequery":
          SaveQuery();
          break;
      case "save":
          if (Page_ClientValidate('')) {
            if (!validatelimitwarning()) { return false; }
            var mode = $('#hidMode', parent.mainmodule.document).val();
            if (mode == "new") {
                if (pdmvalidation())
                    createpdm();
            }
            else if (mode == "edit") {
                if (pdmvalidation())
                    updatepdm();
            }
            else if (mode == "duplicate") {
                duplicatepdm();
            }
        }
        break;
      case "duplicate":
        if (WarnUser())
          window.location.href = 'pdmmain.aspx?mode=duplicate&pdmnum=<%=m_pdmnum %>';
        break;
      case "print":
        PrintForm("<%=defaultprintcounter %>", "PDMPrintForm", "<%=m_pdmnum %>");
        break;
    case "batchprint":
        PrintBatchForms("<%=defaultprintcounter %>", "PDMPrintForm", "PDMNum");
        break;
      case "picture":
        break;
      case "linkeddoc":
        LinkDoc('pdm','<%=m_pdmnum %>');
        break;
      case "delete":
        var resp = window.confirm('<%=m_msg["T2"] %>');//('Are you sure want to delete this PMD? ');
        if (resp) {
          noPrompt();
          __doPostBack("delete", "");
        }
        break;
      case "email":
        sendEmail("PDM", "<%=m_pdmnum%>");
        break;
    }

  }

  function updatepdm() {
    if (!allowsubmit())
      return;
    $(document).ready(function () {
      var dbfield = "", dbval = "";
      var userid = '<%=Session["Login"] %>';
      var dirtylog = $('#txtdirtylog').val();

      var pdmxml = "<?xml version='1.0' encoding='UTF-8'?><pdms><pdm>";

      pdmxml = pdmxml + collectformall("PDM");

      if (dirtylog != '') {
        pdmxml = pdmxml + "<dirtylog>" + dirtylog + "</dirtylog>";
      }

      pdmxml = pdmxml + "</pdm></pdms>";

      $.ajax({
        type: "GET",
        url: "../InternalServices/ServicePDM.svc/UpdatePDM",
        //url: "../InternalServices/ServicePM.svc/DoWork",
        data: { "xmlnvc": pdmxml },

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

  function createpdm() {
    if (!allowsubmit())
      return;
  $(document).ready(function () {
      var userid = '<%=Session["Login"] %>';
      var dbfield, dbval;
      var pdmxml = "<?xml version='1.0' encoding='UTF-8'?><pdms><pdm>";
      pdmxml = pdmxml + collectform("PDM");
      $('input:radio[name=rblinactive]:checked').each(function () {
          dbval = $(this).val();
          if (dbval != '') {
              pdmxml = pdmxml + "<inactive>" + $(this).val() + "</inactive>";
          }
      });

      pdmxml = pdmxml + "</pdm></pdms>";

      $.ajax({
          type: "GET",
          url: "../InternalServices/ServicePDM.svc/CreatePDM",
          data: { "userid": userid, "xmlnvc": pdmxml },
          contentType: "application/json; charset=utf-8",
          dataType: "json",
          success: function (data, status) {
              createsucceeded(data);
          },
          error: OnGetMemberError
      });

  });
  }

  function duplicatepdm() {
    if (!allowsubmit())
      return;
  $(document).ready(function () {
      var userid = '<%=Session["Login"] %>';
      var dbfield, dbval;
      var pdmxml = "<?xml version='1.0' encoding='UTF-8'?><pdms><pdm>";
      var oldpdmnum = document.getElementById("HidOldPDMNum").value;
      pdmxml = pdmxml + collectformall("PDM");

      pdmxml = pdmxml + "</pdm></pdms>";
      $.ajax({
          type: "GET",
          url: "../InternalServices/ServicePDM.svc/DuplicatePDM",
          data: { "userid": userid, "xmlnvc": pdmxml, "oldpdmnum": oldpdmnum },
          contentType: "application/json; charset=utf-8",
          dataType: "json",
          success: function (data, status) {
              FormSubmitted = false;
              createsucceeded(data);
          },
          error: OnGetMemberError
      });

  });
  }

  function createsucceeded(result) {
    var pdmnum = result.d;
    window.location.href = 'pdmmain.aspx?mode=edit&pdmnum=' + pdmnum;
  }

  function savesucceeded(pdmnum) {
    window.location.href = 'pdmmain.aspx?mode=edit&pdmnum=' + pdmnum;
  }

  function OnGetMemberError(request, status, error) {
      alert(status + "--" + error + "--" + request.responseText);
  }

  function pdmvalidation() {
      var x = getvalue("txthighlimit");
      var y = getvalue("txthighlimitproc");
      var z = getvalue("txthighlimitemail");
      if (x!="" && (y =="" && z == ""))
      {
        alert("Please enter high limit procedure number or high limit email notification.");
        document.getElementById("txthighlimitproc").focus();
        return false;
      }
      if (x=="" && (y !="" || z!="")) 
      {
        alert("Please enter high limit number.");
        document.getElementById("txthighlimit").focus();
        return false;
      }

      x = getvalue("txthighwarning");
      y = getvalue("txthighwarningproc");
      z = getvalue("txthighwarningemail");
      if (x!="" && (y =="" && z=="")) 
      {
        alert("Please enter high warning procedure number.");
        document.getElementById("txthighwarningproc").focus();
        return false;
      }
      if (x=="" && (y !="" || z!= "") ) 
      {
        alert("Please enter high warning number or high warning email notification.");
        document.getElementById("txthighwarning").focus();
        return false;
      }

      x = getvalue("txtlowwarning");
      y = getvalue("txtlowwarningproc");
      z = getvalue("txtlowwarningemail");
      if (x!="" && (y =="" && z == "")) 
      {
        alert("Please enter low warning procedure number or low warning email notification.");
        document.getElementById("txtlowwarningproc").focus();
        return false;
      }
      if (x=="" && (y !="" || z!="")) 
      {
        alert("Please enter low warning number.");
        document.getElementById("txtlowwarning").focus();
        return false;
      }

      x = getvalue("txtlowlimit");
      y = getvalue("txtlowlimitproc");
      z = getvalue("txtlowlimitemail");
      if (x!="" && (y =="" && z=="")) 
      {
        alert("Please enter low limit procedure number or low limit email notification.");
        document.getElementById("txtlowlimitproc").focus();
        return false;
      }
      if (x=="" && (y !="" || z!="")) 
      {
        alert("Please enter low limit number.");
        document.getElementById("txtlowlimit").focus();
        return false;
      }

      return true;
    }

    function measurementfocus() {
        //alert("checkit1");
    }
    function measurementlookup() {
        //alert("checkit2");
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
    <!--
    <li>
      <asp:HyperLink ID="tabhlkAccounts" runat="server">
      <asp:Image ID="tabimgAccounts" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px" Width="70px" BorderStyle="None" BorderWidth="0" style="vertical-align:bottom;" />
      <asp:Label ID="tablblAccounts" runat="server" CssClass="toptabout" Text="Accounts" /></asp:HyperLink>
    </li>
    -->
    <li>
      <asp:HyperLink ID="tabhlkWO" runat="server">
      <asp:Image ID="tabimgWO" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px" Width="70px" BorderStyle="None" BorderWidth="0" style="vertical-align:bottom;" />
      <asp:Label ID="tablblWO" runat="server" CssClass="toptabout" Text="Show WOs" /></asp:HyperLink>
    </li>

    <li>
      <asp:HyperLink ID="tabhlkHistory" runat="server">
      <asp:Image ID="tabimgHistory" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px" Width="70px" BorderStyle="None" BorderWidth="0" style="vertical-align:bottom;" />
      <asp:Label ID="tablblHistory" runat="server" CssClass="toptabout" Text="PDM History" /></asp:HyperLink>
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
                    <asp:Literal ID="litOperation" runat="server">Query</asp:Literal>
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
