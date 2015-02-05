<%@ Control Language="C#" AutoEventWireup="true" CodeFile="invheader.ascx.cs"
    Inherits="inventory_invheader" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<link type="text/css" href="../styles/mainmenu.css" rel="stylesheet" />
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

    function toolbarclicked(sender, e) {
        var button = e.get_item();
        var commandname = button.get_commandName();
        switch (commandname) {
            case "search":
                if (WarnUser())
                    settoQueryMode("invmain.aspx");
                break;
            case "new":
                if (WarnUser())
                    window.location.href = 'invmain.aspx?mode=new';
                break;
            case "autonew":
                if (WarnUser())
                  window.location.href = 'invmain.aspx?mode=new&numbering=auto';
                break;
            case "lookup":
                LookupClicked();
                break;
            case "savequery":
                SaveQuery();
                break;
              case "save":
                if (Page_ClientValidate('')) {
                  var markup = document.getElementById("txtmarkup").value*1;
                  if ((markup>=1000) || (markup<=-1000))
                  {
                    alert("Markup value must be less than 1000 and greater than -1000.");
                    return;
                  }
                  var mode = $('#hidMode', parent.mainmodule.document).val();
                  if (mode == "new") {
                    createitem();
                  }
                  else if (mode == "edit") {
                    updateitem();
                  }
                  else if (mode == "duplicate") {
                    duplicateitem();
                  }
                }
                break;
            case "delete":
              noPrompt();
              if (Page_ClientValidate(''))
              {
                var r = confirm("Are you sure you want to delete this item?");
                if (r == false) {
                    return; 
                }
                else{
                    deleteitem();
                }
              }
              break;
            case "reserve":
              checkrequesteditem();
              break;
            case "issue":
              issuereserved();
              break;
            case "return":
              invreturn();
              break;
            case "editissue":
              editissue();
              break;
            case "stagingissue":
              stagingissue();
              break;
            case "duplicate":
                if (WarnUser())
                    window.location.href = 'invmain.aspx?mode=duplicate&itemnum=<%=m_itemnum %>';
                break;
            case "print":
                PrintForm("<%=defaultprintcounter %>", "InvPrintForm", "<%=m_itemnum %>");
                break;
            case "batchprint":
                PrintBatchForms("<%=defaultprintcounter %>", "InvPrintForm", "ItemNum");
                break;
            case "specsearch":
                break;
            case "picture":
                break;
            case "linkeddoc":
                LinkDoc('inventory', '<%=m_itemnum %>');
                break;
            case "addtostore":
                addtostoreroom();
                break;
            case "template":
              URL = "../util/specificationsframe.aspx?linktype=inventory&id=<%=m_itemnum %>";
              //URL = "../util/specificationsframe.aspx";
              var labWnd = radopen(URL, null);
              labWnd.set_modal(true);
              return false;
              break;
            case "email":
                sendEmail("Inventory", "<%=m_itemnum%>");
                break;

        }
      }

          function deleteitem() {
            $.ajax({
              type: "GET",
              url: "../InternalServices/ServiceInventory.svc/DeleteItem",
              //url: "../InternalServices/ServicePM.svc/DoWork",
              data: { "itemnum": "<%=m_itemnum %>" },

              contentType: "application/json; charset=utf-8",
              dataType: "json",

              beforeSend: function (xhr, settings) {
                xhr.setRequestHeader("userid", "<%=Session["Login"] %>");
              },


              success: function (data, status) {
                var result = data.d;
                
                if (result != "OK") {
                  alert(result);
                }
                else
                    setTimeout(parent.controlpanel.RefreshPanelGridsOnDelete(), 0);
                  window.location.href = 'invmain.aspx?mode=query';
              },
              error: OnGetMemberError
            });
          }

function updateitem() {
  $(document).ready(function () {
    var dbfield = "", dbval = "";
    var userid = '<%=Session["Login"] %>';
    var dirtylog = $('#txtdirtylog').val();
    
    if (!checkmethodswithprice())
      return;
    
    if (!checkissuemethodupdate())
      return;
    
    var invxml = "<?xml version='1.0' encoding='UTF-8'?><items><item>";
    var cbb = $find("cbbissuemethod");
    var issuemethod = "";
    if (cbb != null) {
      invxml = invxml + "<issuemethod>" + cbb.get_value() + "</issuemethod>";
    }

    cbb = $find("cbbissueprice");
    var pricemethod = "";
    if (cbb != null) {
      invxml = invxml + "<issueprice>" + cbb.get_value() + "</issueprice>";
    }

    cbb = $find("cbbpurchaseprice");
    var costmethod = "";
    if (cbb != null) {
      invxml = invxml + "<purchaseprice>" + cbb.get_value() + "</purchaseprice>";
    }
    if (dirtylog != '') {
      invxml = invxml + "<dirtylog>" + dirtylog + "</dirtylog>";
     
    }
    invxml = invxml + collectformall("Items");
    invxml = invxml + "</item></items>";
    if (!allowsubmit())
      return;
    $.ajax({
      type: "GET",
      url: "../InternalServices/ServiceInventory.svc/UpdateItem",
      //url: "../InternalServices/ServicePM.svc/DoWork",
      data: { "xmlnvc": invxml },

      contentType: "application/json; charset=utf-8",
      dataType: "json",

      beforeSend: function (xhr, settings) {
        xhr.setRequestHeader("userid", userid);
      },


      success: function (data, status) {
        var result = data.d;
        var d_array = result.split("^");
        FormSubmitted = false;
        if (d_array[0] == "TRUE") {
          window.location.href = 'invmain.aspx?mode=edit&itemnum=<%=m_itemnum %>';
        }
        else if (d_array[0] == "FALSE") {
          alert(d_array[1]);
        }
      },
      error: OnGetMemberError
    });

  });

}

function createitem() {
  $(document).ready(function () {
    var userid = '<%=Session["Login"] %>';
    var dbfield, dbval;

    if (!checkmethodswithprice())
      return;

    var invxml = "<?xml version='1.0' encoding='UTF-8'?><items><item>";

    var cbb = $find("cbbissuemethod");
    var issuemethod = "";
    if (cbb != null) {
      invxml = invxml + "<issuemethod>" + cbb.get_value() + "</issuemethod>";
    }

    cbb = $find("cbbissueprice");
    var pricemethod = "";
    if (cbb != null) {
      invxml = invxml + "<issueprice>" + cbb.get_value() + "</issueprice>";
    }

    cbb = $find("cbbpurchaseprice");
    var costmethod = "";
    if (cbb != null) {
      invxml = invxml + "<purchaseprice>" + cbb.get_value() + "</purchaseprice>";
    }
    invxml = invxml + collectformall("Items");

    invxml = invxml + "</item></items>";
    if (!allowsubmit())
      return;
    $.ajax({
      type: "GET",
      url: "../InternalServices/ServiceInventory.svc/CreateItem",
      data: { "userid": userid, "xmlnvc": invxml },
      contentType: "application/json; charset=utf-8",
      dataType: "json",
      success: function (data, status) {
        createsucceeded(data);
      },
      error: OnGetMemberError
    });

  });
          }

  function duplicateitem() {
    if (!checkmethodswithprice())
      return;
    
    if (!checkissuemethodupdate())
      return;
    $(document).ready(function () {
      var userid = '<%=Session["Login"] %>';
      var dbfield, dbval;
      var olditemnum = document.getElementById("HidOldItemNum").value;
      var invxml = "<?xml version='1.0' encoding='UTF-8'?><items><item>";
      
      var cbb = $find("cbbissuemethod");
      var issuemethod = "";
      if (cbb != null) {
        invxml = invxml + "<issuemethod>" + cbb.get_value() + "</issuemethod>";
      }

      cbb = $find("cbbissueprice");
      var pricemethod = "";
      if (cbb != null) {
        invxml = invxml + "<issueprice>" + cbb.get_value() + "</issueprice>";
      }

      cbb = $find("cbbpurchaseprice");
      var costmethod = "";
      if (cbb != null) {
        invxml = invxml + "<purchaseprice>" + cbb.get_value() + "</purchaseprice>";
      }
      invxml = invxml + collectformall("Items");

      invxml = invxml + "</item></items>";
      $.ajax({
        type: "GET",
        url: "../InternalServices/ServiceInventory.svc/DuplicateItem",
        data: { "userid": userid, "xmlnvc": invxml, "olditemnum": olditemnum},
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, status) {
          createsucceeded(data);
        },
        error: OnGetMemberError
      });

    });
  }

  function checkissuemethodupdate() {
    var cbb = $find("cbbissuemethod");
    var newmethod = cbb.get_value();
    var oldmethod = document.getElementById("hidoldissuemethod").value;
    var serialized = "0";
     $('input:radio[name=rblserialized]:checked').each(function () {
        serialized = $(this).val();
     });

    if (newmethod == "MIXED" && serialized == "1")
    {
      alert("Serialized item can't have Mixed issuemethod.");
      return false;
    }

    if (oldmethod == "MIXED" && newmethod != "MIXED" && oldmethod != "") {  // update from mixed to non mixed
      var resp = window.confirm("Issue Method is updated from " + oldmethod + " to " + newmethod + ". Click on OK to continue, click on cancel to abort.");
      return resp;
    }
    if (oldmethod != "MIXED" && newmethod == "MIXED" && oldmethod != "") {  // update from non mixed to mixed
      var resp = window.confirm("Issue Method is updated from " + oldmethod + " to " + newmethod + ". All Lots in the same storeroom position will be combined into one Lot.Click on OK to continue, click on cancel to abort.");
      return resp;
    }
    return true;

  }

function checkmethodswithprice() {
  var cbb = $find("cbbissuemethod");
  var issuemethod = cbb.get_value();

  var cbb2 = $find("cbbissueprice");
  var issueprice = cbb2.get_value();

  if (issuemethod == "MIXED" && issueprice == "LOTPRICE")  // issuemethod is mixed and issueprice is lot price
  {
    alert("Cannot use Lot Price is use Mixed issue method.");
    return false;
  }
  else {
    return true;
  }
    
}

function OnGetMemberError(request, status, error) {
  FormSubmitted = false;
  alert(status + "--" + error + "--" + request.responseText);
}

function createsucceeded(result) {
  var itemnum = result.d;
  window.location.href = 'invmain.aspx?mode=edit&itemnum=' + itemnum;
}

function checkrequesteditem() {
  var URL = "checkrequesteditem.aspx";
  var dWnd;
  dWnd = radopen(URL, null);
  dWnd.set_modal(true);
}

function issuereserved() {
  var URL = "issuereserve.aspx";
  var dWnd;
  dWnd = radopen(URL, null);
  dWnd.set_modal(true);
}

function editissue() {
  var URL = "issuebatchlist.aspx";
  var dWnd;
  dWnd = radopen(URL, null);
  dWnd.set_modal(true);
}

function stagingissue() {
  var URL = "stagingissue.aspx";
  var dWnd;
  dWnd = radopen(URL, null);
  dWnd.set_modal(true);
}

function invreturn() {
  var URL = "return.aspx";
  var dWnd;
  dWnd = radopen(URL, null);
  dWnd.set_modal(true);
}

function addtostoreroom() {
  var URL = "storemain.aspx?itemnum=<%=m_itemnum %>";
  var dWnd;
  dWnd = radopen(URL, null);
  dWnd.set_modal(true);
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
                <asp:HyperLink ID="tabhlkSpecs" runat="server">
                    <asp:Image ID="tabimgSpecs" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
                        Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
                    <asp:Label ID="tablblSpecs" runat="server" CssClass="toptabout" Text="Additional Info" /></asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="tabhlkWhereused" runat="server">
                    <asp:Image ID="tabimgWhereused" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
                        Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
                    <asp:Label ID="tablblWhereused" runat="server" CssClass="toptabout" Text="Where Used" /></asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="tabhlkEqpList" runat="server">
                    <asp:Image ID="tabimgEqpList" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
                        Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
                    <asp:Label ID="tablblEqpList" runat="server" CssClass="toptabout" Text="Serialized" /></asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="tabhlkVendor" runat="server">
                    <asp:Image ID="tabimgVendor" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
                        Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
                    <asp:Label ID="tablblVendor" runat="server" CssClass="toptabout" Text="Vendors" /></asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="tabhlkOpenPO" runat="server">
                    <asp:Image ID="tabimgOpenPO" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
                        Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
                    <asp:Label ID="tablblOpenPO" runat="server" CssClass="toptabout" Text="Open PO" /></asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="tabhlkAlternatePart" runat="server">
                    <asp:Image ID="tabimgAlternatePart" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
                        Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
                    <asp:Label ID="tablblAlternatePart" runat="server" CssClass="toptabout" Text="Alternate Part" /></asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="tabhlkHistory" runat="server">
                    <asp:Image ID="tabimgHistory" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
                        Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
                    <asp:Label ID="tablblHistory" runat="server" CssClass="toptabout" Text="History" /></asp:HyperLink>
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
