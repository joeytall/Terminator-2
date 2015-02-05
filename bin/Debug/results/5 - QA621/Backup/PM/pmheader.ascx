<%@ Control Language="C#" AutoEventWireup="true" CodeFile="pmheader.ascx.cs" Inherits="pm_pmheader" %>
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

  function generatePM() {
    var URL = "generatepm.aspx?pmnum=<%=m_pmnum %>";
    var dWnd;
    dWnd = radopen(URL, null);
    dWnd.set_modal(true);
  }

  function nestedPM() {
    if (<%= simplepm%> == 1)
    {
      var x = trim(document.getElementById("txtpminterval").value);
      if (x == "") {
        x = trim(document.getElementById("txtmetername").value);
        if (x != "") {
          x = trim(document.getElementById("txtmeterinterval").value);
        }
      }
      var URL = "nestedpm.aspx?pmnum=<%=m_pmnum %>&pminterval=" + x;
      var dWnd;
      dWnd = radopen(URL, null);
      dWnd.set_modal(true);
    }
    else
    {
    alert('<%=m_msg["T1"] %>');
      //alert("Nested PM can't be set for multiple trigger pm.");
    }
  }

  function seasonalPM() {
    var URL = "pmseason.aspx?pmnum=<%=m_pmnum %>";
    var dWnd;
    dWnd = radopen(URL, null);
    dWnd.set_modal(true);
  }

  function sequence() {
    var URL = "nestsel.aspx?pmnum=<%=m_pmnum %>";
    var dWnd;
    dWnd = radopen(URL, null);
    dWnd.set_modal(true);
  }

  function toolbarclicked(sender, e) {
    var button = e.get_item();
    var commandname = button.get_commandName();
    switch (commandname) {
      case "search":
        if (WarnUser())
          settoQueryMode("pmmain.aspx");
        break;
      case "new":
        if (WarnUser())
          window.location.href = 'pmmain.aspx?mode=new';
        break;
      case "autopm":
        if (WarnUser())
          window.location.href = 'pmmain.aspx?mode=new&numbering=auto';
        break;
      case "lookup":
          LookupClicked();
          break;
      case "savequery":
          SaveQuery();
          break;
      case "save":
          if (Page_ClientValidate('')) {
          if (pmvalidation())
          {
            var mode = $('#hidMode', parent.mainmodule.document).val();
            if (mode == "new") {
              if (checkprocedure()) 
                createpm();
            }
            else if (mode == "edit") {
              if (checkprocedure()) 
                updatepm();
            }
            else if (mode == "duplicate") {
              if (checkprocedure())
                duplicatepm();
            }
          }
        }
        break;
      case "duplicate":
        if (WarnUser())
          window.location.href = 'pmmain.aspx?mode=duplicate&pmnum=<%=m_pmnum %>';
        break;
      case "print":
        PrintForm("<%=defaultprintcounter %>","PMPrintForm","<%=m_pmnum %>");
        break;
      case "batchprint":
        PrintBatchForms("<%=defaultprintcounter %>","PMPrintForm","PmNum");
        break;
      case "picture":
        break;
      case "linkeddoc":
        LinkDoc('pm','<%=m_pmnum %>');
        break;
      case "generate":
        generatePM();
        break;
      case "nested":
        nestedPM();
        break;
      case "seasonalpm":
        seasonalPM();
        break;
      case "sequence":
        sequence();
        break;
      case "delete":
        var resp = window.confirm('<%=m_msg["T2"] %>');//('Are you sure want to delete this PM? ');
        if (resp) {
          noPrompt();
          __doPostBack("delete", "");
        }
        break;
      case "email":
        sendEmail("PM", "<%=m_pmnum%>");
        break;
     }
  }

  function checkprocedure() {

    if ("<%=m_mode %>" == "edit") {
      var proc = document.getElementById("txtprocnum").value;
      var oldproc = document.getElementById("hidprocnum").value;

      if ((proc != "") && (proc != oldproc)) {
        resp = window.confirm('<%=m_msg["T3"] %>');
        //"Procedure has been changed. By clicking OK, you will replace all estimated resources with those from the new procedure.\n\rClick Cancel to abort the operation.")
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
    else if ("<%=m_mode %>" == "duplicate") {
      var proc = document.getElementById("txtprocnum").value;
      var oldproc = document.getElementById("hidprocnum").value;
      if (proc != oldproc) {
        resp = window.confirm('<%=m_msg["T4"] %>');
        //"Procedure has been changed. By clicking OK, you will replace all estimated resources with those from the new procedure.\n\rClick Cancel to copy estimated resources from original PM.")
        if (resp) {
          document.getElementById("hidresourcesource").value = "Procedure";
        }
        else {
          document.getElementById("hidresourcesource").value = "PM";
        }
        noPrompt();
        return true;
      }
      else {
        return true;
      }
      return true;
    }
    else {
      noPrompt();
      return true;
    }
    
  }    

  function updatepm() {

    $(document).ready(function () {
      var dbfield = "", dbval = "", procnum = "";
      var userid = '<%=Session["Login"] %>';
      var hidprocnum = $('input:hidden[name=hidprocnum]').val();
      var dirtylog = $('#txtdirtylog').val();

      var pmxml = "<?xml version='1.0' encoding='UTF-8'?><pms><pm>";
      pmxml = pmxml + collectformall("PM");

      pmxml = pmxml + "</pm></pms>";
      //alert(pmxml);
      //alert(userid);
      //alert(hidprocnum);
      if (!allowsubmit())
        return;
      $.ajax({
        type: "GET",
        url: "../InternalServices/ServicePM.svc/UpdatePM",
        //url: "../InternalServices/ServicePM.svc/DoWork",
        data: { "xmlnvc": pmxml },

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
            var newdirtylog;
            newdirtylog = parseInt(dirtylog) + 1;

            $('#txtdirtylog').val(newdirtylog);

            if (hidprocnum != procnum) {
              $('input:hidden[name=hidprocnum]').val(procnum);
              //alert("procnum: " + $('#hidprocnum').val());
              //gridrefresh(); //???
            }
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

  function createpm() {

    $(document).ready(function () {
      var userid = '<%=Session["Login"] %>';
      var dbfield, dbval;
      var pmxml = "<?xml version='1.0' encoding='UTF-8'?><pms><pm>";
      
      pmxml = pmxml + collectformall("PM");

      pmxml = pmxml + "</pm></pms>";
      if (!allowsubmit())
        return;
      $.ajax({
        type: "GET",
        url: "../InternalServices/ServicePM.svc/CreatePM",
        data: { "userid": userid, "xmlnvc": pmxml },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, status) {
          createsucceeded(data);
        },
        error: OnGetMemberError
      });

    });
  }

  function duplicatepm() {
    $(document).ready(function () {
      var userid = '<%=Session["Login"] %>';
      var dbfield, dbval;
      var oldpmnum = document.getElementById("HidOldPMNum").value;
      var resourcesource = document.getElementById("hidresourcesource").value;
      var pmxml = "<?xml version='1.0' encoding='UTF-8'?><pms><pm>";
      
      pmxml = pmxml + collectformall("PM");
      pmxml = pmxml + "</pm></pms>";
      if (!allowsubmit())
        return;
      $.ajax({
        type: "GET",
        url: "../InternalServices/ServicePM.svc/DuplicatePM",
        data: { "userid": userid, "xmlnvc": pmxml, "oldpmnum": oldpmnum, "resourcesource": resourcesource },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, status) {
          createsucceeded(data);
        },
        error: OnGetMemberError
      });

    });
  }

  function createsucceeded(result) {
    var pmnum = result.d;
    window.location.href = 'pmmain.aspx?mode=edit&pmnum=' + pmnum;
  }

  function savesucceeded(pmnum) {
    window.location.href = 'pmmain.aspx?mode=edit&pmnum=' + pmnum;
  }

  function OnGetMemberError(request, status, error) {
    alert(status + "--" + error + "--" + request.responseText);
  }

  function pmvalidation() {
      var calendar = false;
      var usage1 = false;
      var usage2 = false;
      var nextduedate = trim(getvalue("txtnextduedate"));
      var overridedate = trim(getvalue("txtoverridedate"));
      var interval = trim(getvalue("txtpmineterval"));
      var intervalunit = trim(getvalue("txtinetervalunit"));

      if (nextduedate!="" || overridedate!="" || interval!= "" || intervalunit!="") {
        var y = trim(document.getElementById("txtpminterval").value);
        y = y * 1;
        if (y < 1) {
          //Message: Interval cannot be smaller than 1.
          alert('<%=m_msg["T6"] %>');//"Interval cannot be smaller than 1.");
          document.getElementById("txtpminterval").focus();
          return false;
        }
        y = trim(document.getElementById("txtintervalunit").value);
        if (!y.length) {
          //Message: "Please enter interval unit."
          alert('<%=m_msg["T7"] %>');//"");
          document.getElementById("txtintervalunit").focus();
          return false;
        }
        y = trim(document.getElementById("txtnextduedate").value);
        if (!isDate(y)) {
          alert('<%=m_msg["T8"] %>');//"Please enter Due Date.");
          document.getElementById("txtnextduedate").focus();
          return false;
        }
        y = trim(document.getElementById("txtoverridedate").value);
        if (y.length) {
          if (!isDate(y)) {
            //Message: "Please enter Override Date."
            alert('<%=m_msg["T9"] %>');//"");
            document.getElementById("txtoverridedate").focus();
            return false;
          } 
        }
        calendar= true;
      }

      var metername = trim(getvalue("txtmetername"));
      var nextreading = trim(getvalue("txtnextpmreading"));
      var meterrange = trim(getvalue("txtmeterrange"));
      var meterinterval = trim(getvalue("txtmeterinterval"));
      if (metername != "" || nextreading != "" || meterrange!="" || meterinterval != "") {
        var y = trim(document.getElementById("txtmetername").value);
        if (!y.length) {
          //Message: "Please enter Meter Name."
          alert('<%=m_msg["T10"] %>');//"");
          document.getElementById("txtmetername").focus();
          return false;
        }
        y = trim(document.getElementById("txtnextpmreading").value);
        if (!isDecimal(y)) {
          //Message: "Please enter meter reading for Next PM."
          alert('<%=m_msg["T11"] %>');//"");
          document.getElementById("txtnextpmreading").focus();
          return false;
        }
        y = trim(document.getElementById("txtmeterrange").value);
        if (!isDecimal(y)) {
          //Message: "Please enter PM meter range."
          alert('<%=m_msg["T12"] %>');//"");
          document.getElementById("txtmeterrange").focus();
          return false;
        }
        y = trim(document.getElementById("txtmeterinterval").value);
        if (!isDecimal(y)) {
          //Message: "Please enter PM meter range."
          alert('<%=m_msg["T12"] %>');//"");
          document.getElementById("txtmeterinterval").focus();
          return false;
        }
        usage1 = true;
      }

      var metername2 = trim(getvalue("txtmetername2"));
      var nextreading2 = trim(getvalue("txtnextpmreading2"));
      var meterrange2 = trim(getvalue("txtmeterrange2"));
      var meterinterval2 = trim(getvalue("txtmeterinterval2"));
      if (metername2 != "" || nextreading2 != "" || meterrange2!="" || meterinterval2 != "") {
        var y = trim(document.getElementById("txtmetername2").value);
        if (!y.length) {
          //Message: "Please enter Meter Name."
          alert('Please enter Second Meter Name.');//"");
          document.getElementById("txtmetername2").focus();
          return false;
        }
        y = trim(document.getElementById("txtnextpmreading2").value);
        if (!isDecimal(y)) {
          //Message: "Please enter meter reading for Next PM."
          alert('Please enter second meter reading for next PM.');//"");
          document.getElementById("txtnextpmreading2").focus();
          return false;
        }
        y = trim(document.getElementById("txtmeterrange2").value);
        if (!isDecimal(y)) {
          //Message: "Please enter PM meter range."
          alert('Please enter second PM meter range.');//"");
          document.getElementById("txtmeterrange2").focus();
          return false;
        }
        y = trim(document.getElementById("txtmeterinterval2").value);
        if (!isDecimal(y)) {
          //Message: "Please enter PM meter range."
          alert('Please enter second meter interval.');//"");
          document.getElementById("txtmeterinterval2").focus();
          return false;
        }
        usage2 = true;
      }

      if (metername2 == metername && metername!="")
      {
        alert("Second MeterName can not be same as first metername.");
        document.getElementById("txtmetername2").focus();
        return false;
      }

      if (!usage1 && usage2) {
        alert('Please enter first meter before entering second meter.');//"Please enter Interval.");
        return false;
      }

      if (!calendar && !usage1) {
        //Message: "Please enter Interval."
        alert('<%=m_msg["T5"] %>');//"Please enter Interval.");
        document.getElementById("txtpminterval").focus();
        return false;
      }


      return true;
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
      <asp:HyperLink ID="tabhlkAccounts" runat="server">
      <asp:Image ID="tabimgAccounts" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px" Width="70px" BorderStyle="None" BorderWidth="0" style="vertical-align:bottom;" />
      <asp:Label ID="tablblAccounts" runat="server" CssClass="toptabout" Text="Accounts" /></asp:HyperLink>
    </li>
    <li>
      <asp:HyperLink ID="tabhlkWO" runat="server">
      <asp:Image ID="tabimgWO" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px" Width="70px" BorderStyle="None" BorderWidth="0" style="vertical-align:bottom;" />
      <asp:Label ID="tablblWO" runat="server" CssClass="toptabout" Text="Show WOs" /></asp:HyperLink>
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
