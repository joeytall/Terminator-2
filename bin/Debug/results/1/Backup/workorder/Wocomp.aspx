<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Wocomp.aspx.cs" Inherits="workorder_Wocomp" %>

<%@ Register TagPrefix="uc1" TagName="Header" Src="Woheader.ascx" %>
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
<script type="text/javascript" src="../js/jquery-1.7.2.min.js"></script>
<script type="text/javascript" src="../js/jquery-ui-1.8.21.custom.min.js"></script>
<script type="text/javascript" src="../jscript/codes.js"></script>
<script type="text/javascript" src="../jscript/codevalidate.js"></script>
<script type="text/javascript" src="../jscript/setvalue.js"></script>
<script type="text/javascript" src="../jscript/callbackfunction.js"></script>
<script type="text/javascript" src="../jscript/radwindow.js"></script>
<script type="text/javascript" src="../jscript/autocomplete.js"></script>
<script type="text/javascript" src="../jscript/stopdoubleclick.js"></script>
<body style="background-image: url(../IMAGES/back.png); background-repeat: repeat-x;
    margin: 0px; padding: 0px;">
    <form id="MainForm" runat="server">
    <uc1:Header ID="ucHeader1" runat="server" />
    <asp:Literal ID="litMessage" runat="server" />
    <asp:Panel ID="MainControlsPanel" runat="server" CssClass="MainControlsPanelClass">
        <telerik:RadScriptManager ID="RadScriptManager" runat="server">
        </telerik:RadScriptManager>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false">
        </telerik:RadWindowManager>
        <asp:SqlDataSource ID="MeterSqlDataSource" runat="server" ProviderName="System.Data.OleDb">
        </asp:SqlDataSource>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
        <asp:HiddenField ID="hidmeter" runat="server" />
    </asp:Panel>
    <asp:HiddenField ID="hidMode" runat="server" Value="edit" />
    <asp:HiddenField ID="hidTargetStatusCode" Value="300" runat="server" />
    <asp:HiddenField ID="hidStatusComments" Value="" runat="server" />
    <asp:HiddenField ID="hidTargetStatus" Value="COMP" runat="server" />
    <asp:HiddenField ID="hidStatusApplyTo" Value="" runat="server" />
    <asp:HiddenField ID="hidToDo" Value="" runat="server" />
    <asp:HiddenField ID="hidequipment" runat="server" />
    <iframe id="ifValidate" src="" frameborder="1" scrolling="no" style="visibility: hidden;
        display: none;"></iframe>
    </form>
</body>
<script type="text/javascript">
  var g_LCID = <%= Session.LCID %>;
    function validateReading(reading, equipment, metername, fieldid, rolloverfieldid,datecontrolid,counter) {
        if (reading != "") {
            //var date = document.getElementById("txtcompdate").value;
            
            var datecontrol = $find(datecontrolid);
            var date = datecontrol.get_selectedDate();
            
            if (date ==null) {
                //alert("Invalid completion date.");
                alert('<%= m_msg["T3"] %>');
                //resetReading(fieldid, "");
                
                setTimeout(datecontrol.get_textBox.focus(),50);
            }
            else {
                
                var d = toDateString(date);
                //var obj = document.getElementById("ifValidate");
                //obj.src = "../Codes/Readingvalid.aspx?reading=" + reading + "&equipment=" + equipment + "&metername=" + metername + "&compdate=" + d + "&fieldid=" + fieldid;
                if (isDecimal(reading))
                  readingchanged(metername,equipment,datecontrol,reading,fieldid,rolloverfieldid,counter,1);
                else
                {
                alert('<%= m_msg["T6"] %>');
//                  alert("Please enter valid reading.");
                  resetReading(fieldid, "");
                  delayedsetfocus(fieldid);
                }
            }
        }
    }

    function dateselected(sender,arg) {
      //var obj = sender.get_element();
      //var obj = sender.get_dateInput();
      //var index = obj.getAttribute("index");
      var id = sender.get_element().id + "_wrapper";
      var readingfield = $get(id).getAttribute("readingfield");
      var rolloverfield = $get(id).getAttribute("rolloverfield");
      var metername = $get(id).getAttribute("metername");
      var counter = $get(id).getAttribute("counter");
      datechanged(readingfield,rolloverfield,metername,sender, counter);
    }

    function datechanged(readingfield,rolloverfield,metername, datecontrol,counter) {
//      var obj = document.getElementById(readingfield);
      var reading = document.getElementById(readingfield).value;
      var date = datecontrol.get_selectedDate();
      if (reading != "") {
            if (date ==null) {
                //alert("Invalid completion date.");
                //alert('<%= m_msg["T3"] %>');
                //resetReading(fieldid, "");
                //alert(datecontrol.get_textBox.id);
            }
            else {
                
                var d = toDateString(date);
                if (isDecimal(reading))
                  readingchanged(metername,"<%=m_equip %>",datecontrol,reading,readingfield,rolloverfield,counter,2);
            }
        }
    }




    function readingchanged(metername,equipment,datecontrol,reading,readingfield, rolloverfieldid, counter,fieldchanged)
    {
      var date = datecontrol.get_selectedDate();
      var meterdate = toDateString(date);
      $(document).ready(function () {
        $.ajax({
          type: 'GET',
          url: "../InternalServices/ServiceMeter.svc/ValidateMeter",
          data: { "MeterName": metername, "Equipment": equipment, "counter": counter, "meterdate": meterdate, "reading": reading },
          dataType: "json",
          contentType: 'application/json; charset=utf-8',
          success: function (data) {
            var returnStr = data.d; // this.parent.JSON.stringify(data.d); //JSON.stringify(data.d);
            if (returnStr != "0") {
              if (returnStr == "2") // possible rollover
              {
                //resp = window.confirm("Meter reading is smaller than the current reading. Click OK to keep the reading if the new reading is the result of meter rollover or Cancel to erase the reading and enter again.");
                resp = window.confirm('<%= m_msg["T5"] %>');
                if (resp) {
                  document.getElementById(rolloverfieldid).value = "1";
                }
                else {
                  document.getElementById(rolloverfieldid).value = "0";
                  if (fieldchanged==1) // reading
                  {
                    resetReading(readingfield, "");
                    delayedsetfocus(readingfield);
                  }
                  else  //date
                  {
                    datecontrol.set_selectedDate(null);
                    //datecontrol.get_textBox.focus();
                    setTimeout(datecontrol.get_dateInput().focus(),50);
                  }
                }
              }
              else {
                //alert("Meter reading does not fit into current meter reading.");
                alert('<%= m_msg["T4"] %>');

                if (fieldchanged==1) // reading
                {
                  resetReading(readingfield, "");
                  delayedsetfocus(readingfield);
                }
                else  //date
                {
                  datecontrol.set_selectedDate(null);
                  //datecontrol.get_textBox.focus();
                  
                  setTimeout(datecontrol.get_dateInput().focus(),50);
                }
              }
            }
          },
          error: function (xhr, status, error) { alert("Error\n-----\n" + xhr.status + '\n' + xhr.responseText); }
        });
      });

    }

    function resetReading(fieldid, val) {
        var tfield = $find(fieldid);
        if (tfield == null)
            tfield = document.getElementById(fieldid);

        if (tfield != null) {
            try {
                tfield.clear();
            }
            catch (err) {
                tfield.value = val;
            }
        }
    }

    function chkCompDate(txt) {
      if (new Date(txt.value) > new Date()) {
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1;      //January is 0!
        var yyyy = today.getFullYear();

        /*if(dd<10) {
          dd='0'+dd
        } 

        if(mm<10) {
          mm='0'+mm
        }*/ 

        today = mm+'/'+dd+'/'+yyyy;
        txt.value = today;
        txt.focus();        
        alert("You cannot select a day future than today!");
        return false;
      }
    }
</script>
<script type="text/javascript">
    window.onload = function () {
        setpanelheight(1);
        initToolbar();
    }
</script>
</html>
