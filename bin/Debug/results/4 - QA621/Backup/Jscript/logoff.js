function logoff(usermodule) {
  var msglogoff = "";

    $.ajax({
      type: "GET",
      data: { "key": "logoff","value":"1" },
      //url: "../Services/ServiceSession.svc/SetSession",
      url: "../InternalServices/ServiceGeneral.svc/SetSession",
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (data, status) {
        var exdate = new Date()
        exdate.setDate(exdate.getDate() + 30)
        document.cookie = "logoff=1; expires=" + exdate + "; path=/";
        top.document.location.href = "../login.aspx";
//        var objWin = parent;
//        objWin.open('', '_self', '');
//        objWin.close();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
                 alert(textStatus + "--" + errorThrown + "--" + XMLHttpRequest);
           }
    });
//  };
           }

function checklogoff() {
  setInterval("timer2();", 3000);
  //if (getLogoffCookie('logoff') == '1')
  //  window.close();
}

function timer2() {
  var value = getLogoffCookie('logoff');
  if (value =='1')
  {
    //alert("off");
    parent.open('', '_self', '');
    parent.close();
  }
  else {
    //alert(value);
  }

}

function timer()
{
  $.ajax({
    type: "GET",
    data: { "key": "logoff","dummy":Date.now() },
    //url: "../Services/ServiceSession.svc/GetSession",
    url: "../InternalServices/ServiceGeneral.svc/GetSession",
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function (data, status) {
      //alert(data.d);
      if (data.d != "0") {
        var objWin = parent;
        objWin.open('', '_self', '');
        objWin.close();
      }
      else {
      }
    },
    error: function (XMLHttpRequest, textStatus, errorThrown) {
      //alert(textStatus + "--" + errorThrown + "--" + XMLHttpRequest);
    }
  });
}

function setLogoffCookie(c_name, value, usermodule) {
  if (value == '1') {
    var resp = window.confirm('The session will be ended and all windows will be closed. Are you sure to logoff?');
    if (resp) {
      logoff(usermodule);
    }
  }
  else {
    var exdate = new Date()
    exdate.setDate(exdate.getDate() + 30)
    document.cookie = "logoff=0; expires=" + exdate + "; path=/";
  }
}

function getLogoffCookie(c_name) {
  if (document.cookie.length>0)
  {
    c_start=document.cookie.indexOf(c_name + "=")
    if (c_start!=-1)
    { 
      c_start=c_start + c_name.length+1 
      c_end=document.cookie.indexOf(";",c_start)
      if (c_end == -1) c_end = document.cookie.length
        return unescape(document.cookie.substring(c_start,c_end))
    } 
  }
  return ""
}
