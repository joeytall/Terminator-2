// This file contains the JavaScript for checking date, decimal and numeric format

function daysOfMonth(nMonth, nYear) {
    switch (nMonth) {
        case 1:
        case 3:
        case 5:
        case 7:
        case 8:
        case 10:
        case 12:
            return 31;
        case 4:
        case 6:
        case 9:
        case 11:
            return 30;
        case 2:
            if (nYear % 4 == 0)
                return 29;
            else
                return 28;
        default:
            return 0;
    }
}

function isDate(str) {
    var i = str.indexOf(String.fromCharCode(47), 0);
    if (i == -1)
        return false;
    else {
        var monthPart = str.substr(0, i);
        var tempstr = str.substr(i + 1);
    }

    i = tempstr.indexOf(String.fromCharCode(47), 0);
    if (i == -1)
        return false;
    else {
        var dayPart = tempstr.substr(0, i);
        var yearPart = tempstr.substr(i + 1);
    }

    if (g_LCID == 1033) {
        // m/d/y
        if (yearPart.length != 4 && yearPart.length != 2)
            return false;
        if (yearPart.length == 2)
            yearPart = "20" + yearPart;
        if (validDate(monthPart, dayPart, yearPart))
            return true;
        else
            return false;
    }
    else if (g_LCID == 7177) {
        // y/m/d
        if (monthPart.length != 4 && monthPart.length != 2)
            return false;
        if (monthPart.length == 2)
            monthPart = "20" + monthPart;
        if (validDate(dayPart, yearPart, monthPart))
            return true;
        else
            return false;
    }
    else {
        // d/m/y
        if (yearPart.length != 4 && yearPart.length != 2)
            return false;
        if (yearPart.length == 2)
            yearPart = "20" + yearPart;
        if (yearPart.length != 4 && yearPart.length != 2)
            return false;
        if (validDate(dayPart, monthPart, yearPart))
            return true;
        else
            return false;
    }
}

function isDateTime(str) {

    var i = str.indexOf(String.fromCharCode(47), 0);
    var strs = str.split(" ");
    var datestr = "";
    var timestr = "";
    if (strs.length == 2) {
        if (isDate(strs[0]) && isTime(strs[1]))
            return true;
        else
            return false;
    }
    else {
        return isDate(str);
    }
}

function isTime(str) {
    var strs = str.split(":");
    if (strs.length < 2)
        return false;
    if (strs.length == 3) {
        var h = strs[0];
        var m = strs[1];
        var s = strs[2];
        if (isNumeric(h) && isNumeric(m) && isNumeric(s)) {

            if (h * 1 >= 0 && h * 1 <= 23 && m * 1 >= 0 && m * 1 <= 59 & s * 1 >= 0 && s * 1 <= 59)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    if (strs.length == 2) {
        var h = strs[0];
        var m = strs[1];
        if (isNumeric(h) && isNumeric(m)) {
            if (h * 1 >= 0 && h * 1 <= 23 && m * 1 >= 0 && m * 1 <= 59)
                return true;
            else
                return false;
        }
        else
            return false;
    }
}

function isDecimal(str) {
    if (!str.length) return false;

    var hasDecimal = false;
    var ch;
    if (str.substr(0, 1) == "-") {
        for (var i = 1; i < str.length; i++) {
            ch = str.substr(i, 1);
            if (ch == "." || ch == ",") {
                if (hasDecimal)
                    return false;
                else
                    hasDecimal = true;
            }
            else if (ch < "0" || ch > "9")
                return false;
        }
    }
    else {
        for (var i = 0; i < str.length; i++) {
            ch = str.substr(i, 1);
            if (ch == "." || ch == ",") {
                if (hasDecimal)
                    return false;
                else
                    hasDecimal = true;
            }
            else if (ch < "0" || ch > "9")
                return false;
        }
    }
    return true;
}

function isNumeric(str) {
    if (!str.length) return false;

    var ch;
    if (str.substr(0, 1) == "-") {
        for (var i = 1; i < str.length; i++) {
            ch = str.substr(i, 1);
            if (ch < "0" || ch > "9")
                return false;
        }
    }
    else {
        for (var i = 0; i < str.length; i++) {
            ch = str.substr(i, 1);
            if (ch < "0" || ch > "9")
                return false;
        }
    }
    return true;
}

function validDate(strMonth, strDay, strYear) {
    if (validMonth(strMonth) && isNumeric(strDay) && isNumeric(strYear)) {
        var nYear = strYear * 1;
        var nMonth = strMonth * 1;
        var nDay = strDay * 1;
        var maxDays = daysOfMonth(nMonth, nYear);
        if (nDay < 1 || nDay > maxDays)
            return false;
        else
            return true;
    }
    else
        return false;
}

function validMonth(str) {
    if (isNumeric(str)) {
        var nMonth = str * 1;
        if (nMonth < 1 || nMonth > 12)
            return false;
        else
            return true;
    }
    else
        return false;
}

// trim function removes leading and trailing white spaces
function trim(str) {
  var ch;
    var i;
    str = str + "";
    for (i = 0; i < str.length; i++) {
        ch = str.substr(i, 1)
        if (ch != " ")
          break;
      }

    //alert(str+"----"+"---"+i + " " + str.length);//test code
    var tempstr = str.substr(i)

    for (i = tempstr.length - 1; i >= 0; i--) {
        ch = tempstr.substr(i, 1)
        if (ch != " ")
            break
    }
    return tempstr.substr(0, i + 1);
}

// replace function replaces search char with replace char
function replaceChar(str, srch, repl) {
    var newstr = "";
    var leng = str.length;
    var l = srch.length;
    var x;

    for (var i = 0; i < leng; i++) {
        x = str.substr(i, l);
        if (x == srch) {
            newstr += repl;
            i = i + l - 1;
        }
        else {
            newstr += str.substr(i, 1);
        }
    }
    return newstr;
}

function retMonth(str) {
    if (!isDate(str))
        return 0;
    var monthPart = 0;
    if (g_LCID == 1033) {
        i = str.indexOf(String.fromCharCode(47), 0);
        monthPart = str.substr(0, i);
    }
    else if (g_LCID == 2057 || g_LCID == 7177) {
        i = str.indexOf(String.fromCharCode(47), 0);
        j = str.lastIndexOf(String.fromCharCode(47));
        monthPart = str.substring(i + 1, j);
    }
    return monthPart * 1;
}

function retYear(str) {
    if (!isDate(str))
        return 0;
    var yearPart = 0;
    if (g_LCID == 1033 || g_LCID == 2057) {
        i = str.lastIndexOf(String.fromCharCode(47));
        yearPart = str.substr(i + 1);
    }
    else if (g_LCID == 7177) {
        i = str.indexOf(String.fromCharCode(47), 0);
        yearPart = str.substr(0, i);
    }
    yearPart *= 1;
    if (yearPart < 100)
        yearPart += 2000;
    return yearPart;
}

function retDay(str) {
    if (!isDate(str))
        return 0;
    var dayPart = 0;
    if (g_LCID == 1033) {
        i = str.indexOf(String.fromCharCode(47), 0);
        j = str.lastIndexOf(String.fromCharCode(47));
        dayPart = str.substring(i + 1, j);
    }
    else if (g_LCID == 2057) {
        i = str.indexOf(String.fromCharCode(47), 0);
        dayPart = str.substr(0, i);
    }
    else if (g_LCID == 7177) {
        i = str.lastIndexOf(String.fromCharCode(47));
        dayPart = str.substr(i + 1);
    }
    return dayPart * 1;
}

function roundup(num, d) {
    var m = Math.pow(10, d);
    if (num < 0) {
        num *= -1;
        return (Math.round(num * m)) * -1 / m;
    }
    else
        return (Math.round(num * m)) / m;
}

function checkURL(str) {
    var charArray = new Array("#", "&", "%", "+", "?", ",", ";", "'", "\"", "=");
    var l = charArray.length;
    for (var i = 0; i < l; i++) {
        if (str.indexOf(charArray[i]) > -1)
            return false;
    }
    return true;
}

function Today() {
    var d = new Date();
    var thisYear = d.getFullYear();
    var thisMonth = d.getMonth();
    var thisDate = d.getDate();
    return new Date(thisYear, thisMonth, thisDate, 0, 0, 0, 0);
}

function CustomDate(str) {
    var thisYear = retYear(str);
    var thisMonth = retMonth(str) - 1;
    var thisDate = retDay(str);
    return new Date(thisYear, thisMonth, thisDate, 0, 0, 0, 0);
}

function toDateString(date) {
    var yy = date.getFullYear();
    var mm = date.getMonth() + 1;
    var dd = date.getDate();

    if (g_LCID == 1033)
        return mm + "/" + dd + "/" + yy;
    else if (g_LCID == 2057)
        return dd + "/" + mm + "/" + yy;
    else if (g_LCID == 7177)
        return yy + "/" + mm + "/" + dd;
  }

  function MeaningfulDate(datefield, meaning) {
    var datevalue = getvalue(datefield);
    if (!isDate(datevalue)) {
      return false;
    }
    var mdate;
    try {
      mdate = CustomDate(datevalue);
    }
    catch (err) {
      return false;
    }

    var today = new Date();
    if (meaning == "past") {
      if (mdate > today)
        return false;
      else
        return true;
    }
    else if (meaning == "future") {
      if (mdate < today)
        return false;
      else
        return true;
    }
    else
      return true;
  }

function dataChanged() {
    var modeobj = document.getElementById("hidMode");
    var mode = "";
    if (modeobj != null)
        mode = modeobj.value;
    // alert(mode);
    if (mode != "query") {
        var obj = document.getElementById("hidDataChanged");
        if (obj != null) {
            if (document.location.href.indexOf("womain.aspx")>=0)
                window.status = "Editing...";
            obj.value = "1";
        }
    }
}

function getmessage(msgarray, msgid) {
    if (msgarray == null)
        return "";
    if (msgid == "")
        return "";
    if (msgarray[msgid] != null)
        return msgarray[msgid];
    else
        return "";
}

// Allow the user to be warned : unsaved changes.
function WarnUser() {
    var modifyflag, nopromptflag;

    var objhide = document.getElementById("hidDataChanged");
    var objprompt = document.getElementById("hidNoPromt");

    if (objhide != null)
        modifyflag = objhide.value;

    if (objprompt != null)
        nopromptflag = objprompt.value;

    if (nopromptflag == 0 && modifyflag == 1) {
        var resp = window.confirm(getmessage(msgcodes, "T1"));
        if (resp)
            return true;
        else
            return false;
    }
    else {
        if (objprompt != null)
            objprompt.value = "0";
        return true;
    }
}

function noPrompt() {
    var obj = document.getElementById("hidNoPromt");
    if (obj != null)
        obj.value = "1";
}

function roradiobutton(name, index) {
    $('input:radio[name=' + name + ']')[index].checked = true;
}

//bad function, do not use
function rocheckbox(name, value) {
    $('input:checkbox[name=' + name + ']').checked = value;
}


$(document).unbind('keydown').bind('keydown', function (event) {
    var doPrevent = false;
    if (event.keyCode === 8) {
        var d = event.srcElement || event.target;
        if ((d.tagName.toUpperCase() === 'INPUT' && (d.type.toUpperCase() === 'TEXT' || d.type.toUpperCase() === 'PASSWORD')) || d.tagName.toUpperCase() === 'TEXTAREA') {
            doPrevent = d.readOnly || d.disabled;
        }
        else {
            doPrevent = true;
        }
    }

    if (doPrevent) {
        event.preventDefault();
    }
    });

    $(window).resize(function () {
      if (GetRadWindow() == null) {
        setpanelheight(1);
      }

    });

function setpanelheight(paneltype) {
    var aw = window.screen.availWidth;
    var ah = window.screen.availHeight;

    var innerheight = window.innerHeight;
    var clientheight = window.clientHeight;
    var bodyheight = document.body.clientHeight;
    //ah = window.innerHeight || window.clientHeight || document.body.clientHeight;
    //alert(ah);
    ah = window.innerHeight || window.clientHeight || document.documentElement.clientHeight;
    aw = window.innerWidth || window.clientWidth || document.body.clientWidth;
    // if safari then use window.screen.availwidth and height
    var ua = navigator.userAgent.toLowerCase();

    var issafari = false;
    var isiphone = false;
    var isipad = false;

    if (ua.indexOf('ipad') != -1) {
      //isipad = true;
    }

    /*
    if (ua.indexOf('safari') != -1) {
      if (ua.indexOf('chrome') > -1) {

      }
      else
      {
      issafari = true;
      }
    }
    issafari = false;
    if (ua.indexOf('iphone') != -1) {
      isiphone = true;
    }

    if (issafari) {
      aw = window.screen.availWidth;
      ah = window.screen.availHeight;
      if (aw < ah) {
        var temp = aw;
        aw = ah;
        ah = temp;
      }
    }
    */

    var cw;
    if (window.frameElement != null) {
      var framename = window.frameElement.id;
      cw = GetFrameWidth(framename);
    }

    //if (paneltype == 1)
    //    cw = roundup(aw * 2 / 3, 0);
    //else
    //    cw = roundup(aw * 1 / 3, 0);
    //var ch = document.documentElement.clientHeight;

    if (!isipad)
      cw = aw;
    var ch = ah;

    /*
    alert("ah:" + ah);
    alert("ch:" + ch);
    alert("clientwidth:" + window.clientWidth);
    alert("innerwidth:" + window.innerWidth);
    alert("bodywidth:" + document.body.clientWidth);
    alert("cw:" +cw);
    */
    var mpanel;
    //the original value deducted from ch is 135
    //this will cause a vertical scrroll bar appear for the frame is a horizon scroll bar required for the panel
    mpanel = document.getElementById("MainControlsPanel");
    if (mpanel != null) {
        mpanel.style.height = (ch - 150) + "px";
        mpanel.style.width = (cw - 20) + "px";
    }
    mpanel = document.getElementById("MainControlsPanel2");
    if (mpanel != null) {
        mpanel.style.height = (ch - 150) + "px";
        mpanel.style.width = (cw - 20) + "px";
    }
    mpanel = document.getElementById("MainControlsPanel3");
    if (mpanel != null) {
        mpanel.style.height = (ch - 150) + "px";
        mpanel.style.width = (cw - 20) + "px";
    }

    }

function GetFrameWidth(framename) {
  if (top.document.getElementById(framename) != null)
    return top.document.getElementById(framename).scrollWidth;
  else
    return 0;
}

function divmainredirect_xxx(counter, selectedval) {
    alert(selectedval);
    if (selectedval.toLowerCase() == "logo") {
        //            alert("1");
        window.location.href('../codes/divlogomain.aspx?counter=' + counter + '&referer=' + selectedval);
    } else if (selectedval.toLowerCase() == "tax1" || selectedval.toLowerCase() == "tax2") window.location.href('../codes/divtaxmain.aspx?counter=' + counter + '&referer=' + selectedval);
    else
        window.location.href('../codes/divdefaultmain.aspx?counter=' + counter + '&referer=' + selectedval);
}


function divmainredirect(counter, selectedval) {
    //    alert(selectedval);
    var url;
    if (selectedval.toLowerCase() == "logo") {
        //            alert("1");
        //    window.location.href('../codes/divlogomain.aspx?counter=' + counter + '&referer=' + selectedval);
        url = '../codes/divlogomain.aspx?counter=' + counter + '&referer=' + selectedval;
    }
    else if (selectedval.toLowerCase() == "tax1" || selectedval.toLowerCase() == "tax2") {
        //window.location.href('../codes/divtaxmain.aspx?counter=' + counter + '&referer=' + selectedval);
        url = '../codes/divtaxmain.aspx?counter=' + counter + '&referer=' + selectedval;
    }
    else if (selectedval.toLowerCase() == "storeroom") {
        url = '../codes/divstoreroommain.aspx?counter=' + counter + '&referer=' + selectedval;
    }
    else if (selectedval.toLowerCase() == "timeoffset") {
        url = '../codes/divtimeoffsetmain.aspx?counter=' + counter + '&referer=' + selectedval;
    }
    else {
        url = '../codes/divdefaultmain.aspx?counter=' + counter + '&referer=' + selectedval;
        //window.location.href('../codes/divdefaultmain.aspx?counter=' + counter + '&referer=' + selectedval);
    }
    return url;
  }

  function collectform(dbtable) {
    var xml = "";
    var value = "";
    var dbfield = "";
    $('input:text[DBTable=' + dbtable + ']').each(function () {
      value = $(this).val();
      if (value != '') {
        dbfield = $(this).attr('DBField');
        var percent = $(this).attr('Percent');
        if (percent == "yes") {
          if (isNaN(parseFloat(value)))
            value = 0;
          else
            value = value / 100;
        }
      }
      xml = xml + "<" + $(this).attr('DBField') + ">" + escapexml(value) + "</" + $(this).attr('DBField') + ">";
    });

    $('textarea[DBTable=' + dbtable + ']').each(function () {
      value = $(this).val();
      dbfield = $(this).attr('DBField');
      xml = xml + "<" + $(this).attr('DBField') + ">" + escapexml(value) + "</" + $(this).attr('DBField') + ">";
    });

    return xml;
  }

  function collectform2(dbtable) {
    var xml = "";
    var value = "";
    var dbfield = "";
    $('input:text[DBTable=' + dbtable + ']').each(function () {
      value = $(this).val();
      if (value != '') {
        dbfield = $(this).attr('DBField');
        var percent = $(this).attr('Percent');
        if (percent == "yes") {
          if (isNaN(parseFloat(value)))
            value = 0;
          else
            value = value / 100;

        }
        if (trim(value)!="")
          xml = xml + "<" + $(this).attr('DBField') + ">" + escapexml(value) + "</" + $(this).attr('DBField') + ">";
      }
    });

    $('textarea[DBTable=' + dbtable + ']').each(function () {
      value = $(this).val();
      dbfield = $(this).attr('DBField');
      if (trim(value) != "")
        xml = xml + "<" + $(this).attr('DBField') + ">" + escapexml(value) + "</" + $(this).attr('DBField') + ">";
    });

    return xml;
  }

  function collectformall(dbtable) {
    var xml = "";
    var value = "";
    var dbfield = "";
    $('input:text[DBTable=' + dbtable + ']').each(function () {
      value = $(this).val();
      if (value != '') {
        dbfield = $(this).attr('DBField');
        var percent = $(this).attr('Percent');
        if (percent == "yes") {
          if (isNaN(parseFloat(value)))
            value = 0;
          else
            value = value / 100;
        }
      }
      xml = xml + "<" + $(this).attr('DBField') + ">" + escapexml(value) + "</" + $(this).attr('DBField') + ">";
    });

    $('textarea[DBTable=' + dbtable + ']').each(function () {
      value = $(this).val();
      dbfield = $(this).attr('DBField');
      xml = xml + "<" + $(this).attr('DBField') + ">" + escapexml(trim($(this).val())) + "</" + $(this).attr('DBField') + ">";
    });

    $('input[type=checkbox]').each(function () {
      if ($(this).attr("DBTable") == dbtable) {
        if ($(this).attr('checked') == "checked")
          xml = xml + "<" + $(this).attr('DBField') + ">1</" + $(this).attr('DBField') + ">";
        else
          xml = xml + "<" + $(this).attr('DBField') + ">0</" + $(this).attr('DBField') + ">";
      }
    });

    $('input:radio:checked').each(function () {
      //alert($(this).parent().parent().parent().parent().parent().html());
      var name = $(this).attr("name");
      try {
        if ($("#" + name).attr("DBTable") == dbtable);
        {
          xml = xml + "<" + $("#" + name).attr("DBField") + ">" + escapexml($(this).val()) + "</" + $("#" + name).attr("DBField") + ">";
        }
      }
      catch (error) {
        //alert( error.message );
      }
    });
    /*
    var i;
    var combos = Telerik.Web.UI.RadComboBox.ComboBoxes;
    for (i = 0; i < combos.length; i++) {
      if combos.
    }
    */



    return xml;
  }

  function escapexml(str) {
    str = str + "";
    return str.replace(/&/g, '&amp;')
              .replace(/</g, '&lt;')
              .replace(/>/g, '&gt;')
              .replace(/"/g, '&quot;')
              .replace(/'/g, '&apos;');
  }


  function writeline(str) {
      var obj = document.getElementById("txtrequest");
      if (obj != null)
          obj.value = obj.value + ";" + str;
  }

  function chgframesize(module) {
      var temp = top.document.getElementById("mainmodule").scrollWidth;
      var scrwidth = screen.width;
      var scrheight = screen.height;
      if (scrheight > scrwidth) {
        scrwidth = scrheight;
      }
      
      var newframewidth = Math.round((temp * 1) / (scrwidth * 1) * 100);
      var newframewidth1 = 100 - newframewidth;
      var datakey = module + "FrameMain";
      var datakey1 = module + "FrameControlPanel";

          $.ajax({
            type: "get",
            data: { "key": datakey, "value": newframewidth, "key1": datakey1, "value1": newframewidth1, "UserModule": module },
            url: "../InternalServices/ServiceGeneral.svc/SetFrameSession",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
            },
            error: function (request, status, err) {
              //alert(err);
            }
          });
  }

  function chgframesizewithvalue(module) {
    var curwidth = 70;

    //var oriscrwidth = scrwidth;
//    if (document.getElementById) {
//      var frame = parent.document.getElementById("mainmodule");
//      if (frame.scrollWidth) {
//        curwidth = frame.scrollWidth;
//      }
//    }

    var resetframe = true;

    do {
      var promptvalue = window.prompt("Please enter a number from 10 to 90 (The column size in percent of the available space)", curwidth);
      if (promptvalue == null) {
        resetframe = false;
        break;
      }
      var selection = parseInt(promptvalue);
    } while (isNaN(selection) || selection > 90 || selection < 10);

    if (resetframe) {
      var newframewidth = selection;
      var newframewidth1 = 100 - newframewidth;
      var datakey = module + "FrameMain";
      var datakey1 = module + "FrameControlPanel";

      alert(newframewidth);
      $.ajax({
        type: "get",
        data: { "key": datakey, "value": newframewidth, "key1": datakey1, "value1": newframewidth1, "UserModule": module },
        url: "../InternalServices/ServiceGeneral.svc/SetFrameSession",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
        },
        error: function (request, status, err) {
          //alert(err);
        }
      });

      //parent.document.getElementsByTagName('frameset')[0].setAttribute('cols', '10%,*');

      //var frameset = parent.document.getElementsByTagName("frameset")[0];
      //alert(frameset.cols);
      //frameset.cols = "50%,*";
      //window.frameElement.parentNode.cols = "0,10,*";

      //parent.document.getElementById("MainFrameSet").cols = "500,*";         
      //alert(top.frames['mainmodule'].location.href);
      //parent.window.location.reload();
      //$('#MainFrameSet').attr('src', $('#MainFrameSet').attr('src'));

      //var temp = window.parent.MainFrameSet.cols;
      //alert(temp);
      //window.parent.MainFrameSet.cols = "20%,80%";

      var subURL = top.frames['mainmodule'].location.href;
      var URL1 = "";
      if (subURL.indexOf("?") != -1) {
        URL1 = "&" + subURL.substring(subURL.indexOf("?") + 1);
      }

      var URL = "";

      switch (module) {
        case "WO":
          URL = "woframe.aspx?URL=Womain.aspx" + URL1;
          break;
        case "WR":
          URL = "wrframe.aspx";
          break;
        case "TIMECARD":
          URL = "tcframe.aspx";
          break;
        case "PM":
          URL = "pmframe.aspx?URL=pmmain.aspx" + URL1;
          break;
        case "PROC":
          URL = "procframe.aspx?URL=procmain.aspx" + URL1;
          break;
        case "PDM":
          URL = "pdmframe.aspx?URL=pdmmain.aspx" + URL1;
          break;
        case "EQP":
          URL = "equipmentframe.aspx?URL=equipmentmain.aspx" + URL1;
          break;
        case "ROUTE":
          URL = "routeframe.aspx?URL=routemain.aspx" + URL1;
          break;
        case "LOC":
          URL = "locationframe.aspx?URL=locationmain.aspx" + URL1;
          break;
        case "LAB":
          URL = "labourframe.aspx?URL=labourmain.aspx" + URL1;
          break;
        case "INV":
          URL = "invframe.aspx?URL=invmain.aspx" + URL1;
          break;
        case "STOREROOM":
          URL = "srframe.aspx";
          break;
        case "VENDOR":
          URL = "vendorframe.aspx?URL=vendormain.aspx" + URL1;
          break;
        case "PO":
          URL = "poframe.aspx?URL=pomain.aspx" + URL1;
          break;
        case "RECV":
          URL = "receiveframe.aspx?URL=receivemain.aspx" + URL1;
          break;
        case "REQ":
          URL = "reqframe.aspx?URL=reqmain.aspx" + URL1;
          break;
        case "PROJECT":
          URL = "projectframe.aspx?URL=projectmain.aspx" + URL1;
          break;
        case "KPI":
          URL = "kpiframe.aspx?URL=kpimain.aspx" + URL1;
          break;
        case "INTERFACE":
          URL = "infframe.aspx?URL=infmain.aspx" + URL1;
          break;
      }

      parent.document.location.href = URL;
    }
  }

  //  function RefreshRecentListGrids() {
//      var grid = $find("grdrecentlist");
//      var MasterTable;
//      if (grid != null) {
//          MasterTable = grid.get_masterTableView();
//      }
//      if (MasterTable != null) {
//          MasterTable.rebind();
//      }
//  }


  function RefreshPanelGridsOnDelete() {
      var grdrecentlist = $find("grdrecentlist");
      var recentlistMasterTable;
      if (grdrecentlist != null) {
          recentlistMasterTable = grdrecentlist.get_masterTableView();
      }
      if (recentlistMasterTable != null) {
          recentlistMasterTable.rebind();
      }

      var grdresults = $find("grdresults");
      var resultsMasterTable;
      if (grdresults != null) {
          resultsMasterTable = grdresults.get_masterTableView();
      }
      if (resultsMasterTable != null) {

          resultsMasterTable.rebind();
      }

      var grdworklist = $find("grdworklist");
      var worklistMasterTable;
      if (grdworklist != null) {
          worklistMasterTable = grdworklist.get_masterTableView();
      }
      if (worklistMasterTable != null) {
          worklistMasterTable.rebind();
      }
  }

  function RefreshPanelGrids() {
      var hidmode = parent.mainmodule.document.getElementById("hidMode");
      if (hidmode.value == "edit") {
          var grdrecentlist = $find("grdrecentlist");
          var recentlistMasterTable;
          if (grdrecentlist != null) {
              recentlistMasterTable = grdrecentlist.get_masterTableView();
          }
          if (recentlistMasterTable != null) {
              recentlistMasterTable.rebind();
          }

          var grdresults = $find("grdresults");
          var resultsMasterTable;
          if (grdresults != null) {
              resultsMasterTable = grdresults.get_masterTableView();
          }
          if (resultsMasterTable != null) {
              resultsMasterTable.rebind();
          }

          var grdworklist = $find("grdworklist");
          var worklistMasterTable;
          if (grdworklist != null) {
              worklistMasterTable = grdworklist.get_masterTableView();
          }
          if (worklistMasterTable != null) {
              worklistMasterTable.rebind();
          }
      }
  }


  function setReadOnlyControl(controlid) {
      var obj = document.getElementById(controlid);
      var x = trim(obj.value);
      if (x.length) {
          obj.focus();
      }
  }

  function setReadOnlyLookup(checkedid, readonlyid, message) {
      var obj = document.getElementById(checkedid);
      var x = trim(obj.value);
      if (x.length) {
          alert(message);
      }
      else {
          generalLookup(readonlyid);
      }
      }

  function validateEmail(emailField) {
    var value = getvalue(emailField);
    var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
    if (reg.test(value) == false) {
      return false;
    }
    return true;
  }

  function resizeGrid(panelname, gridname) {
    var panel = document.getElementById(panelname);
    var grid = $find(gridname);
    if (panel != null && grid != null) {
      if (grid.get_element().style.height != panel.style.height) {
        grid.get_element().style.height = panel.style.height + "px";
        grid.repaint();
      }
    }
  }

  function ClearSelection(obj, args) {
    try
    {
      var MasterTable = obj.get_masterTableView();
      var selectedRows = MasterTable.get_selectedItems();

      for (var i = 0; i < selectedRows.length; i++) {
        var row = selectedRows[i];
        row.set_selected(false);
      }
    }  
    catch (err)
    {
    }
  }

  function collectformalltostring(dbtable) {
    var retstr = "";
    var value = "";
    var dbfield = "";
    $('input:text[DBTable=' + dbtable + ']').each(function () {
      value = $(this).val();
      if (value != '') {
        dbfield = $(this).attr('DBField');
        var percent = $(this).attr('Percent');
        if (percent == "yes") {
          if (isNaN(parseFloat(value)))
            value = 0;
          else
            value = value / 100;
        }
        retstr = retstr + $(this).attr('DBField') + "^" + value + ";";
      }
    });

    $('textarea[DBTable=' + dbtable + ']').each(function () {
      value = $(this).val();
      dbfield = $(this).attr('DBField');
      if (trim(value) != "")
        retstr = retstr + $(this).attr('DBField') + "^" + $(this).val() + ";";
    });

    $('input[type=checkbox]').each(function () {
      if ($(this).attr("DBTable") == dbtable) {
        if ($(this).attr('checked') == "checked")
          retstr = retstr + $(this).attr('DBField') + "^1;";
        else
          retstr = retstr + $(this).attr('DBField') + "^0;";
      }
    });

    $('input:radio:checked').each(function () {
      var name = $(this).attr("name");
      try {
        if ($("#" + name).attr("DBTable") == dbtable);
        {
          if ($(this).val() != "" && $(this).val() != null) {
            retstr = retstr + $("#" + name).attr("DBField") + "^" + $(this).val() + ";";
          }
        }
      }
      catch (error) {
        //alert( error.message );
      }
    });

    return retstr;
  }

function IsOldBrowser(v) {
    var browser = $.browser,
        ie = browser.msie,
        version = parseInt(browser.version);
    if (v == undefined)
        return ie;
    if (ie)
        if (version <= v)
            return true;
        else
            return false;
    else
        return false;
}

function setoriginalclass(controlid) {
    document.getElementById(controlid).className = $("#" + controlid).attr("ClientCss");
}