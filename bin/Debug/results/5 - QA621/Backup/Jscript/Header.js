function imageChangeover(id, src) {
  var obj = document.getElementById(id);
  obj.src = src;
}

function classChangeover(id, classid) {
  var obj = document.getElementById(id);
  obj.className = classid;
}

function LinkDoc(module, recordnum) {
  var URL = "../util/LinkDocframe.aspx?module=" + module + "&recordnum=" + recordnum;
  var oWnd = radopen(URL, null);
  oWnd.set_modal(true);
}

function PrintForm(reportcounter, windowname, key) {
  var URL = "../printform/printreport.aspx?printformcounter=" + reportcounter + "&keyvalues=" + key;
  var rptwindow = window.open(URL, windowname, "menubar=yes,resizable=yes,width=800,height=600,top=100,left=0, scrollbars=yes");
}

function emailnotifyquery(tablename) {
  var xml = collectformall(tablename);
}

function settoQueryMode(URL) {
    document.location.href = URL;
}

function openreport(counter) {
    var URL = "../reportbuilder/runtimecondition.aspx?counter=" + counter;
    newwin = window.open(URL);
    setTimeout('newwin.focus();', 100);
}

function ModuleMenu() {
  //mainmenuwindow = dhtmlmodal.open('MainMenu', 'iframe', '../Mainmenu/Mainmenu.aspx', 'Main Menu', 'width=900px,height=600px,top=0,left=0,resize=1,scrolling=1');
  var URL = "../Mainmenu/Mainmenu.aspx";
  var dWnd;
  if (GetRadWindow() == null) {
    dWnd = radopen(URL, null);
    dWnd.set_modal(true);
    $telerik.$(dWnd.get_popupElement()).addClass("MainMenuRadWin");
  }
}

function PrintList(module, key) {
    var URL = "../codes/printformlist.aspx?usermodule=" + module + "&keyvalue=" + key;
    var oWnd = radopen(URL, null);
    oWnd.set_modal(true);
}

function PrintBatchForms(reportcounter, windowname, keyname) {
    var URL = "../printform/batchprintform.aspx?printformcounter=" + reportcounter + "&keyvalues=";
    var mode = parent.controlpanel.document.getElementById("hidMode").value;
    var grid;
    var selecteditems;
    var keys = "";
    switch (mode) {
        case "worklist":
            grid = parent.controlpanel.$find("grdworklist");
            break;
        case "queryresult":
            grid = parent.controlpanel.$find("grdresults");
            break;
        case "recentlist":
            grid = parent.controlpanel.$find("grdrecentlist");
            break;
    }
    if (grid != null) {
        selecteditems = grid.get_masterTableView().get_selectedItems();
        if (selecteditems.length > 0) {
            for (i = 0; i < selecteditems.length; i++) {
                if (i > 0) {
                    keys = keys + "," + selecteditems[i].getDataKeyValue(keyname);
                }
                else {
                    keys = selecteditems[i].getDataKeyValue(keyname);
                }
            }
        }
        else {
            alert("Please select records first.");
            return false;
        }
    }
    else {
        return false;
    }

    URL = URL + keys;
    var rptwindow = window.open(URL, windowname, "menubar=yes,resizable=yes,width=800,height=600,top=100,left=0, scrollbars=yes");
}

function sendEmail(module,key)
{
    var URL = "../emailnotify/sendemail.aspx?module=" + module + "&keyvalue=" + key;
    var oWnd = radopen(URL, null);
    oWnd.set_modal(true);
}

    var rtime = new Date(1, 1, 2000, 12, 00, 00);
    var timeout = false;
    var delta = 200;
    $(window).resize(function () {
        rtime = new Date();
        if (timeout === false) {
            timeout = true;
            setTimeout(resizeend, delta);
        }
    });

    function resizeend() {
        if (new Date() - rtime < delta) {
            setTimeout(resizeend, delta);
        } else {
            timeout = false;
            updatePanelBar();
            var module = GetFromUrl("module"),
                shorthandModule = LookupModule(module);
            if (shorthandModule != "module not found")
                chgframesize(shorthandModule);
        }
    }

// $(window).load(function() {
//         $(".overflow, .DropDownOverflow").parent().hide();
//         $(".regular").parent().addClass("onbar");
//         if (GetFromUrl("page") !== "Womain.aspx" && $(".timer") !== null)
//             $(".timer").parent().remove();
//         updatePanelBar();
//         if (!IsPanel())
//         {
//             RefreshPanel();
//             if (GetFromUrl("page") == "Womain.aspx")
//             {
//                 if (IsOldBrowser(10))
//                     inittimertoolitems(getTimerStatus());
//             }
//         }
//     });

function initToolbar() {
        $(".overflow, .DropDownOverflow").parent().hide();
        $(".regular").parent().addClass("onbar");
        if (GetFromUrl("page") !== "womain.aspx" && $(".timer") !== null)
            $(".timer").parent().remove();
        updatePanelBar();
        if (!IsPanel()) {
            RefreshPanel();
            if (GetFromUrl("page") == "womain.aspx") {
                if (IsOldBrowser(10))
                    inittimertoolitems(getTimerStatus());
            }
        }
    }

    function GetFromUrl(parameter) {
        var pathName = window.location.pathname,
            pathBreakdown = pathName.split('/'),
            module = pathBreakdown[1],
            page = pathBreakdown[2];
        switch (parameter) {
            case "module":
                return module;
            case "page":
                return page.toLowerCase();
            default:
                return "parameter not defined!";
        }
    }

    function LookupModule(module) {
        var table = {
            "equipment": "EQP",
            "interface": "INTERFACE",
            "inventory": "INV",
            "kpi": "KPI",
            "labour": "LAB",
            "location": "LOC",
            "pdm": "PDM",
            "pm": "PM",
            "purchase": "PO",
            "proc": "PROC",
            "project": "PROJECT",
            "receiving": "RECV",
            "requisition": "REQ",
            "route": "REQ",
            "storeroom": "STOREROOM",
            "timecards": "TIMECARD",
            "vendor": "VENDOR",
            "workorder": "WO",
            "workrequest": "WR"
        },
        usermodule = table[module];
        return usermodule === undefined ? "module not found" : usermodule;
    }

    function IsPanel() {
        var form = $("form");
        if (form == undefined)
            return "form not found";
        return form.attr("action").indexOf("panel") == -1 ? false : true;
    }

    function updatePanelBar() {
        var width = $(window).width(),
            icons = $(".onbar").not(".hidden").length,
            ratio = 58,
            offset = IsPanel() ? 50 : 160,
            availableIcons = (parseInt(width) - offset) / ratio,
            difference = Math.floor(availableIcons - icons),
            absDiff = Math.abs(difference);
        //console.log(width);
        if (difference > 0) {
            for (var i = 0; i < absDiff; i++)
                swapShowIcon();
        }
        else if (difference < 0) {
            for (var i = 0; i < absDiff; i++)
                swapHideIcon();
        }

        updateDropDown();
    }

    function updateDropDown() {
        var dropdown = $(".rtbDropDown"),
            nextSibling = dropdown.next(),
            prevSibling = dropdown.prev();
        while (prevSibling.length != 0)
        {
            if (!prevSibling.hasClass("hidden") && !prevSibling.hasClass("onbar"))
            {
                prevSibling.addClass("onbar").show();
                swapHideIcon();
                prevSibling = prevSibling.prev();
            }
            prevSibling = prevSibling.prev();
        }
        while (nextSibling.length != 0)
        {
            nextSibling.hide().removeClass("onbar");
            nextSibling = nextSibling.next();
        }

        var iconList = $(".rtbGroup .rtbItem").not(".hidden"),
            totalIcons = iconList.length,
            icons = $(".onbar").not(".hidden").length,
            hideIcons = totalIcons - icons;
        if (hideIcons <= 0 )
            dropdown.hide();
        else
        {
            dropdown.show();
            iconList.slice(0, icons).hide();
            iconList.slice(-hideIcons).show();
        }
    }

    function swapHideIcon() {
        var dropDown = $(".rtbDropDown"),
            prevSibling = dropDown.prev();
        while(prevSibling.length != 0)
        {
            if (!prevSibling.hasClass("hidden")) {
                prevSibling.hide().before(dropDown).removeClass("onbar").addClass("overflow");
                return;
            }
            else
                prevSibling = prevSibling.prev();
        }
    }

    function swapShowIcon() {
        var dropDown = $(".rtbDropDown"),
            nextSibling = dropDown.next();
        $(".DropDownOverflow").last().removeClass("DropDownRegular").addClass("DropDownOverflow").parent().hide();
        while(nextSibling.length != 0)
        {
            if (!nextSibling.hasClass("hidden"))
            {
                nextSibling.show().after(dropDown).removeClass("overflow").addClass("onbar");
                return;
            }
            else
                nextSibling = nextSibling.next();
        }

        //if (nextSibling.length != 0 && !nextSibling.hasClass("hidden"))
        //    nextSibling.show().after(dropDown).removeClass("overflow").addClass("onbar");
        //else
        //    dropDown.hide();
    }

    function RefreshPanel() {
        var keyName = $("#hidKeyName").val(),
            keyValue = $("#"+keyName).val(),
              panelcontrols = parent.window.frames[1];
        if (keyValue != undefined)
            try {
                //setTimeout(panelcontrols.HighlightSelection(keyValue), 500);
                panelcontrols.RefreshAllGrids();
            }
            catch (e) {
                //console.log(e.message);
            }
    }

    function SaveQuery() {
        var module = GetFromUrl("module"),
        URL = '../util/userquerymain.aspx?counter=&queryname=&module=' + module + '&mode=new',
        oWnd;
        if (GetRadWindow() == null) {
            oWnd = radopen(URL, null);
            oWnd.set_modal(true);
        }
    }
