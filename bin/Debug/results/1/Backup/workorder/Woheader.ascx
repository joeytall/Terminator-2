<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Woheader.ascx.cs" Inherits="workorder_Woheader" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<link type="text/css" href="../styles/mainmenu.css" rel="stylesheet" />
<script type="text/javascript" src="../Jscript/Header.js"></script>
<script type="text/javascript" src="../Jscript/Codes.js"></script>
<script type="text/javascript" src="../Jscript/Validation.js"></script>
<script type="text/javascript" src="../Jscript/StopDoubleClick.js"></script>
<script type="text/javascript" src="../Jscript/RadControls.js"></script>
<script type="text/javascript">

    // $(window).load(function() {
    //    initToolbar();
    //    });
    //
    //
    // //$(document).ready(function () {
    // //function pageLoad()
    // //{
    //     setpanelheight(1);
    //     var mode = getvalue("hidMode"),
    //         numbering = "";
    //     if (mode == "query")
    //         newquery("workorder");
    //     else if (mode == "edit")
    //     {
    //         if (keyvalue == "")
    //             newquery("workorder");
    //         else
    //             loadrecord("WorkOrder","WoNum",keyvalue,"workorder");
    //     }
    //     else  //new, autonew, duplicate
    //     {
    //         if (keyvalue == "")
    //         {
    //             if (numbering == "auto")
    //                 newmode("Workorder","auto","WoNum","workorder");
    //             else
    //                 newmode("Workorder","manual","WoNum","workorder");
    //         }
    //         else
    //         {
    //             setvalue("hidkeyvalue",keyvalue);
    //             duplicatemode();
    //         }
    //     }
    //
    //     //if (getvalue("hidkeyvalue") != "")
    //     //    loadrecord("WorkOrder","WoNum",getvalue("hidkeyvalue"))
    //     //else
    //     //{
    //     //    newquery("workorder");
    //     //}
    //
    // //}
    // //}
    // })

    function changeStatus() {
        var wonum = getvalue("txtwonum");
        //statuswindow = dhtmlmodal.open('StatusBox', 'iframe', 'Wostatus.aspx?wonum=' + wonum, 'Change Status', 'width=700px,height=600px,center=1,resize=1,scrolling=1')
        var oWnd;
        if (GetRadWindow() == null) {
            oWnd = radopen('Wostatus.aspx?wonum=' + wonum, null);
            oWnd.set_modal(true);
        }
    }

    function customizevalidation()
    {
        var projectid = document.getElementById("txtprojectid").value;
        var phase = document.getElementById("txtphase").value;

        if (((projectid != "")&&(phase == "")) || ((projectid =="") && (phase != "")))
        {
            alert("Invalid Project ID and Phase.");
            return false;
        }


        return true;

    }

    function updateprocedure() {
        var obj = document.getElementById("txtwonum");
        var wonum = obj.value;
        alert('<%= m_msg["T2"] %>');

        var url = "../codes/loclist.aspx?fieldlist=location^txtlocation";
        newwin = window.open(url, "Dispatch Work Request", "Left = 10, Top = 10, resizable = yes, menubar = yes, statusbar= yes");
      }

    function checkrequest() {
      var url = "wrlist.aspx";
      newwin = window.open(url, "Dispatch Work Request", "Left = 10, Top = 10, resizable = yes, menubar = yes, statusbar= yes");
    }

    function checkScheduler() {
      var url = "../scheduler/mainScheduler.aspx";
      var maxwidth = "<%= Session["ScreenWidTh"] %>" - 10;
      var maxheight = "<%= Session["ScreenHeight"] %>" - 10;
      newwin = window.open(url, "Scheduler", 'width=<%=Session["ScreenWidTh"] %>,height=<%=Session["ScreenHeight"] %>, Left=0,Top=0,resizable=yes,menubar=yes,scrollbars=yes, statusbar=yes');
      newwin.focus();
    }

    function modifyequipment() {
        //var equipment = "<%=m_equipment %>";
        alert('<%= m_msg["T3"] %>');
    }

    function opencalendar() {
        var URL;
        if ("<%= m_tabname%>" == "Main")
            URL = "Wocalendar.aspx?wonum=" + getdatafield("wonum");
        else
            URL = "Wocalendar.aspx?wonum=<%=m_wonum %>";
        var oWnd = radopen('Wocalendar.aspx?wonum=<%=m_wonum %>', null);

        oWnd.set_modal(true);
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

      function openquery(counter) {
        parent.window.frames[1].UserQuery(counter, "grdresults");
      }

    function showmap() {
        //var URL = "../codes/map.aspx?location=<%=m_location %>";
        var URL;
        if ("<%= m_tabname%>" == "Main")
            URL = "../codes/map.aspx?location=" + getdatafield("location");
        else
            URL = "../codes/map.aspx?location=<%=m_location %>";

      var dWnd;
      if (GetRadWindow() == null) {
        dWnd = radopen(URL, null);
        dWnd.set_modal(true);
      }
    }

    function toolbarclicked(sender, eventArgs) {
        var button = eventArgs.get_item();
        var commandname = button.get_commandName();
        switch (commandname) {
            case "search":
                if (WarnUser())
                {
                  if ("<%=m_tabname%>" == "Main")
                        newquery("workorder");
                    else
                        settoQueryMode("womain.aspx");
				}
                break;
            case "newwo":
                if (WarnUser())
				  if ("<%=m_tabname%>" == "Main") {
                        newmode("WorkOrder","manual","WONum","workorder");
                    }
                    else
                        window.location.href = 'Womain.aspx?mode=new&numbering=manual';
                break;
            case "autowo":
                if (WarnUser())
                    if ("<%=m_tabname%>" == "Main") {
                        newmode("WorkOrder","auto","WONum","workorder");
                    }
                    else
                    window.location.href = 'Womain.aspx?mode=new&numbering=auto';
                break;
                break;
            case "lookup":
                LookupClicked();
                break;
              case "save":
                //button.disable();
                if (!customizevalidation()) { return false;}
                if (checksave()) {
                  if (Page_ClientValidate('')) {

                    var mode = $('#hidMode', parent.mainmodule.document).val();


                    if (mode == "new") {
                      createworkorder();
                    }
                    else if (mode == "edit") {
                      // alert("2 " + mode);
                        saveworkorder(false);
                      //savewodata();
                      //gridrefresh();
                    }

                    else if (mode == "duplicate") {
                      var estimate = 1;
                      var resp = window.confirm('Duplicate Estimated Resources From "Estimate"');
                      if (resp) {
                        estimate = 1;
                      }
                      else {
                        estimate = 0;
                      }

                      duplicateworkorder(estimate);
                    }
                  }
                }
                break;
              case "complete":
                noPrompt();
                if (chkCompDateTime()) {
                  if (Page_ClientValidate('')) {
                    if (allowsubmit())
                      __doPostBack("complete", "");
                    //compworkorder();
                  }
                }
                break;
            case "status":
                changeStatus();
                break;
            case "map":
                showmap();
                break;
            case "duplicate":
                if (WarnUser()) {
                    if ("<%=m_tabname%>" == "Main") {
                        duplicatemode();
                    }
                    else {
                        var obj = document.getElementById("txtwonum");
                        var wonum = obj.value;
                        window.location.href = 'Womain.aspx?mode=duplicate&wonum=' + wonum;
                    }
                }
                //duplicateWorkOrder();
                break;
            case "print":
                if ("<%=m_tabname%>" == "Main")
                    PrintForm("<%=defaultprintcounter %>", "WOPrintForm", getdatafield("wonum"));
                else
                    PrintForm("<%=defaultprintcounter %>", "WOPrintForm", getvalue("txtwonum"));
                break;
            case "print list":
                if ("<%=m_tabname%>" == "Main")
                    PrintList("workorder", getdatafield("wonum"));
                else
                    PrintList("workorder", getvalue("txtwonum"));
                break;
            case "batchprint":
              PrintBatchForms("<%=defaultprintcounter %>","WOPrintForm","WoNum");
                break;
            case "email":
                if ("<%=m_tabname%>" == "Main")
                    sendEmail("Workorder", getdatafield("wonum"));
                else
                    sendEmail("Workorder", getvalue("txtwonum"));
                break;
              case "picture":
                emailnotifyquery("WorkOrder");
                break;
            case "linkeddoc":
                if ("<%=m_tabname%>" == "Main")
                    LinkDoc('workorder', getdatafield("wonum"));
                else
                {
                    LinkDoc('workorder', getvalue("txtwonum"));
                }

                break;
            case "calendar":
                opencalendar();
                break;
            case "updateprocedure":
                updateprocedure();
                break;
            case "modifyequipment":
                modifyequipment();
                break;
            case "schedule":
                checkScheduler();
                break;
            case "request":
                checkrequest();
                break;
            case "savequery":
                SaveQuery();
                break;
            case "timerinfo":
                timerinfo();
                break;
            case "timerstart":
                timerstart();
                break;
            case "timerresume":
                timerresume();
                break;
            case "timerpause":
                timerpause();
                break;
            case "timerstop":
                timerstop();
                break;
        }
    }

    function inittimertoolitems(timerstatus)
    {
        $(".timer").each(function () {
            var timerType = $(this).find(".rtbText").text(),
                parent = $(this).parent();
            if ($(this).hasClass("dropdown"))
                parent.addClass("DropDownTimer");
            else
                parent.addClass("PanelBarTimer");
            if ( !parent.hasClass("timerParent"))
            {
                parent.addClass("timerParent");
                parent.addClass(timerType);
            }
        });

        $(".timerParent").hide().addClass("hidden").removeClass("onbar");

        var mode = getvalue("hidMode");
        var statuscode = getvalue("txtstatuscode");

        if (timerstatus == "")
            return;
        if (mode != "edit")
            return;

        if (!isDecimal(statuscode))
            statuscode = 1;
        else
            statuscode = statuscode * 1;
        if (statuscode < 200 || statuscode >= 300)
            return;

        $(".Info.DropDownTimer").show().removeClass("hidden");
        $(".Info.PanelBarTimer").removeClass("hidden");
        showTimer("Info");

        //$(".Stop.PanelBarTimer").after($(".rtbDropDown"));
        switch (timerstatus) {
            case "new":
                showTimer("Start");
                break;
            case "running":
                showTimer("Pause");
                showTimer("Stop");
                break;
            case "pause":
                showTimer("Resume");
                showTimer("Stop");
                break;
        }

        updatePanelBar();
    }

    function changeTimerStatus(timerStatus)
    {
        switch (timerstatus) {
            case "new":
                showTimer("Start");
                break;
            case "running":
                showTimer("Pause");
                showTimer("Stop");
                break;
            case "pause":
                showTimer("Resume");
                showTimer("Stop");
                break;
        }
    }

    function showTimer(timer) {
        var dropDown = $(".rtbDropDown"),
            dropDownIndex = dropDown.index(),
            panelBarTimer = $("." + timer + ".PanelBarTimer"),
            dropDownTimer = $("." + timer + ".DropDownTimer");
        if (timer === null)
            return "timer not found";

        $("." + timer).removeClass("hidden");
        var diff = dropDownIndex - panelBarTimer.index();
        if (diff > 0)
            panelBarTimer.show().addClass("onbar").removeClass("overflow");
        else
            dropDownTimer.show();

    }

    function timerinfo() {
        var wonum = getdatafield("wonum");
        var URL = "../workorder/stoptimer.aspx?mode=info&wonum=" + wonum;
        var oWnd;
        if (GetRadWindow() == null) {
            oWnd = radopen(URL, null);
            oWnd.set_modal(true);
        }

    }

    function timerstart(){
        var wonum = getdatafield("wonum");
        if (!allowsubmit)
        {
            return;
        }
        $.ajax({
            type: "GET",
            url: "../InternalServices/ServiceWO.svc/TimerStart",
            data: { "wonum": wonum },
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data,status) {
                var result = data.d;
                if (result == "OK"){
                    //inittimertoolitems("running");
                    loadadditionaldata();
                    FormSubmitted = false;
                }
                else
                {
                    alert(result);
                }
            },
            error: OnGetMemberError
        });
    }

    function timerresume(){
        var wonum = getdatafield("wonum");
        if (!allowsubmit) {
            return;
        }
        $.ajax({
            type: "GET",
            url: "../InternalServices/ServiceWO.svc/TimerResume",
            data: { "wonum": wonum },
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data,status) {
            var result = data.d;
                if (result == "OK"){
                    //inittimertoolitems("running");
                    loadadditionaldata();
                    FormSubmitted = false;
                }
                else
                {
                    alert(result);
                }
            },
            error: OnGetMemberError
        });
    }

    function timerpause(){
        var wonum = getdatafield("wonum");
        if (!allowsubmit) {
            return;
        }
        $.ajax({
            type: "GET",
            url: "../InternalServices/ServiceWO.svc/TimerPause",
            data: { "wonum": wonum },
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data,status) {
            var result = data.d;
                if (result == "OK"){
                    //inittimertoolitems("pause");
                    loadadditionaldata();
                    FormSubmitted = false;
                }
                else
                {
                    alert(result);
                }
            },
            error: OnGetMemberError
        });

    }

    function timerstop(){
        var wonum = getdatafield("wonum");
        var URL = "../workorder/stoptimer.aspx?mode=edit&wonum="+wonum;
        var oWnd;
        if (GetRadWindow() == null) {
            oWnd = radopen(URL, null);
            oWnd.set_modal(true);
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

      function chkCompDateTime() {
        var objcompdate = document.getElementById("txtcompdate");
        var objcomphour = document.getElementById("cbbcomphour");
        var objcompminu = document.getElementById("cbbcompminute");
        var current = new Date();

        if (objcompdate != null && objcomphour != null && objcompminu != null) {
          var curdate = new Date(objcompdate.value);
          var curhours = objcomphour.value;
          var curminutes = objcompminu.value;
          var curyear = curdate.getFullYear();
          var curmonth = curdate.getMonth();
          var curday = curdate.getDate();
          var curdatetime = new Date(curyear, curmonth, curday, curhours, curminutes, current.getSeconds(), current.getMilliseconds())

          if (curdatetime > current) {
            alert('Could not enter the date and time which is greater than current date and time.');
            return false;
          }
          else {
            return true;
          }
        }
        else {
          alert('Please enter the complete date and time.');
          return false;
        }
      }

</script>
<script type="text/javascript">

    function OnClientSelectedIndexChanged(sender, eventArgs) {
        var item = eventArgs.get_item();
    }

    function checksave() {
      var mode = getvalue("hidMode");
        if (mode == "edit") {

            var proc = document.getElementById("txtprocnum").value;
            //var oldproc = document.getElementById("hidprocnum").value;
            var oldproc = getdatafield("procnum");
            if ((proc != "") && (proc != oldproc)) {
                resp = window.confirm('<%= m_msg["T4"] %>')
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

    function createworkorder() {

      $(document).ready(function () {
        var userid = '<%=Session["Login"] %>';
        var dbfield, dbval;
        var statuscode = $('input:hidden[name=hidTargetStatusCode]').val();
        var hasstatuscode = false;
        var woxml = "<?xml version='1.0' encoding='UTF-8'?><workorders><workorder>";

        woxml = woxml + collectformall("WorkOrder");

        woxml = woxml + "</workorder></workorders>";

        if (!allowsubmit()) return;
        $.ajax({
          type: "GET",
          url: "../InternalServices/ServiceWO.svc/CreateWorkorder",
          data: { "userid": userid, "xmlnvc": woxml },
          contentType: "application/json; charset=utf-8",
          dataType: "json",
          success: function (data, status) {
            createsucceeded(data);

            FormSubmitted = false;
          },
          error: OnGetMemberError
        });
      });
          }

          function duplicateworkorder(copyfrom) {

              var userid = '<%=Session["Login"] %>';
              var dbfield, dbval;
              var wonum = "";
              if ("<%=m_tabname%>" == "Main")
                  wonum = getdatafield("wonum");
              else
                  wonum = getvalue("txtwonum");

              //var statuscode = $('input:hidden[name=hidTargetStatusCode]').val();
              var woxml = "<?xml version='1.0' encoding='UTF-8'?><workorders><workorder>";
              woxml = woxml + collectformall("WorkOrder");
              woxml = woxml + "</workorder></workorders>";

              if (!allowsubmit()) return;
              $.ajax({
                type: "GET",
                url: "../InternalServices/ServiceWO.svc/CreateWorkorder",
                data: { "userid": userid, "xmlnvc": woxml, "oldwonum": wonum, "estimate": copyfrom },
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, status) {
                  createsucceeded(data);

                  FormSubmitted = false;
                },
                error: OnGetMemberError
              });
          }

    function createsucceeded(result) {
        var wonum = result.d;
        loadrecord("WorkOrder", "WoNum", wonum,"WorkOrder");
    }

    function savesucceeded(wonum) {
        loadrecord("WorkOrder", "WoNum", wonum,"WorkOrder");
        // window.location.href =
        //alert(result.d);
        // alert(result.d + "--" + stat);
    }

    function succeeded(wonum) {
        loadrecord("WorkOrder", "WoNum", wonum,"WorkOrder");
    }

    function OnGetMemberError(request, status, error) {
        alert(status + "--" + error + "--" + request.responseText);
        // alert(status);
        FormSubmitted = false;
    }

    // not used
    function compworkorder() {
      $(document).ready(function () {
        var dirtylog = $('#txtdirtylog').val();
        var statuscode = $('input:hidden[name=hidTargetStatusCode]').val();
        var equipment = $('input:hidden[name=hidequipment]').val();
        var userid = '<%=Session["Login"] %>';

        var woxml = "<?xml version='1.0' encoding='UTF-8'?><workorders><workorder>";
        $('input:text[DBTable=workorder]').each(function () {
          dbval = $(this).val();
          //if (dbval != '') {
          dbfield = $(this).attr('DBField');
          woxml = woxml + "<" + $(this).attr('DBField') + ">" + $(this).val() + "</" + $(this).attr('DBField') + ">";
          //}
        });

        $('textarea[DBTable=workorder]').each(function () {
          dbval = $(this).val();
          //if (dbval != '') {
          dbfield = $(this).attr('DBField');
          woxml = woxml + "<" + $(this).attr('DBField') + ">" + $(this).val() + "</" + $(this).attr('DBField') + ">";
          //}
        });

        if (statuscode != '') {
          woxml = woxml + "<statuscode>" + statuscode + "</statuscode>";
        }

        woxml = woxml + "</workorder>";
        woxml = woxml + "<meter><odometer>11</odometer><hours>12</hours>";

        if (equipment != '') {
          woxml = woxml + "<equipment>" + equipment + "</equipment>";
        }

        woxml = woxml + "</meter></workorders>";

        if (!allowsubmit()) return;
        $.ajax({
          type: "GET",
          url: "../InternalServices/ServiceWO.svc/CompleteWO",
          data: { "xmlnvc": woxml },
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

              succeeded(d_array[1]); //complete succeeded
            }
            else if (d_array[0] == "FALSE") {
              alert(d_array[1]);
            }
            FormSubmitted = false;
          },
          error: OnGetMemberError
        });

      });
    }
</script>
<asp:Literal ID="litFrameScript" runat="server" />
<asp:Literal ID="litDateLCIDScript" runat="server" />
<asp:Literal ID="litScript" runat="server" />
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
            <li>
                <asp:HyperLink ID="tabhlkHistory" runat="server">
                    <asp:Image ID="tabimgHistory" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
                        Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
                    <asp:Label ID="tablblHistory" runat="server" CssClass="toptabout" Text="History" /></asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="tabhlkComplete" runat="server">
                    <asp:Image ID="tabimgComplete" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
                        Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
                    <asp:Label ID="tablblComplete" runat="server" CssClass="toptabout" Text="Complete" /></asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="tabhlkWorkRequest" runat="server">
                    <asp:Image ID="tabimgWorkRequest" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
                        Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
                    <asp:Label ID="tablblWorkRequest" runat="server" CssClass="toptabout" Text="Work Request" /></asp:HyperLink>
            </li>
        </ul>
    </div>
    <br />
    <br />
    <div style="background-image: url(../IMAGES/toolbarback.png); background-repeat: repeat-x;
        position: relative; top: -28px; display: block; width: 100%; height: 64px; padding: 5px 0 0 20px;
        vertical-align: top; z-index: 5;; overflow:hidden">
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
        <!--  <div style="  position:relative; top:8px; left:0px; font-family: Arial, Helvetica, sans-serif; font-size:8pt; color: #FFFFFF;">
        Current Operation: <asp:Literal ID="litOperation1" runat="server">Query</asp:Literal>
    </div>-->
    </div>
    <asp:HiddenField ID="hidCurrentRecord" runat="server" />
    <asp:HiddenField ID="hidtimerstatus" runat="server" />

</div>
