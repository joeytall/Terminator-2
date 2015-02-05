<%@ Control Language="C#" AutoEventWireup="true" CodeFile="equipmentheader.ascx.cs" Inherits="equipment_equipmentheader" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<link type="text/css" href="../styles/mainmenu.css" rel="stylesheet" />
<script type="text/javascript" src="../Jscript/Header.js"></script>
<script type="text/javascript" src="../Jscript/Codes.js"></script>
<script type="text/javascript" src="../Jscript/Validation.js"></script>
<script type="text/javascript" src="../Jscript/StopDoubleClick.js"></script>
<script type="text/javascript" src="../Jscript/Setvalue.js"></script>
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
  }

  function downtimereset(equipment) {
      $.ajax({
            type: "GET",
            url: "../InternalServices/ServiceEqpt.svc/ResetDownTime",
            data: { "equipment": equipment},
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: OnDownTimeResetSuccess,
            error: OnError
        });
  }
  function OnError(request, status, error) {
      FormSubmitted = false;
      alert(status + "--" + error + "--" + request.responseText);
  }
  function OnDownTimeResetSuccess (data, status)
  {
        var result = data.d;
        if (result != "OK")
        {
            alert(result);
        }
        else
        {
            window.location.href = "equipmentmain.aspx?mode=edit&equipment=" + '<%=m_equipment %>';
        }


  }
  function modifyequipment(equipment) {
    var dWnd;
    dWnd = radopen('modifyequipment.aspx?equipment=' + equipment, null);
    dWnd.set_modal(true);
  }
  function addmeter(equipment) {
    var dWnd;
    dWnd = radopen('addmeter.aspx?equipment=' + equipment, null);
    dWnd.set_modal(true);
  }
  function addmeasurement(equipment) {
    var dWnd;
    dWnd = radopen('addmeasurement.aspx?linktype=equipment&linkid=' + equipment, null);
    dWnd.set_modal(true);
  }

  function checkserialnum() {
    var itemnum = getvalue("txtitemnum");
    var serialnum = getvalue("txtserialnum");
    if (itemnum != "" && serialnum == "") {
      alert("Please enter serial number for serialized item.");
      document.getElementById("txtserialnum").focus();
      return false;
    }
    return true; 
  }

  function toolbarclicked(sender, e) {
    var button = e.get_item();
    var commandname = button.get_commandName();
    switch (commandname) {
      case "search":
        if (WarnUser())
          settoQueryMode("equipmentmain.aspx");
        break;
      case "new":
        if (WarnUser())
          window.location.href = 'equipmentmain.aspx?mode=new';
        break;
      case "autonew":
        if (WarnUser())
          window.location.href = 'equipmentmain.aspx?mode=new&numbering=auto';
        break;
      case "lookup":
          LookupClicked();
          break;
      case "savequery":
          SaveQuery();
          break;
      case "save":
          noPrompt();
        if (Page_ClientValidate('')) {
          if (checkserialnum()) {
            var mode = $('#hidMode', parent.mainmodule.document).val();
            if (mode == "new") {
              createequipment();
            }
            else if (mode == "edit") {
              saveequipment();
            }
            else if (mode == "duplicate") {
              __doPostBack("save", "");
            }
          }
        }
        break;
      
      case "duplicate":
        if (WarnUser())
          window.location.href = 'equipmentmain.aspx?mode=duplicate&equipment=<%=m_equipment %>';
        break;
      case "print":
        PrintForm("<%=defaultprintcounter %>", "EQPPrintForm", "<%=m_equipment %>");
        break;
      case "batchprint":
        PrintBatchForms("<%=defaultprintcounter %>", "EQPPrintForm", "Equipment");
        break;
      case "specsearch":
        break;
      case "picture":
          break;
      case "email":
          sendEmail("Equipment", "<%=m_equipment%>");
          break;
      case "linkeddoc":
        LinkDoc('equipment','<%=m_equipment %>');
        break;
      case "delete":
        if (getvalue("hidDeleteMessage") == "") {
          var resp = window.confirm('<%=m_msg["T1"] %>'); //'Are you sure want to delete this equipment? If Deleted, all child equipment will not reference this equipment as parent.');
          if (resp) {
            noPrompt();
            if (allowsubmit())
              __doPostBack("delete", "");
          }
        }
        else
          alert(getvalue("hidDeleteMessage"));

        break;
      case "addmeter":
        addmeter('<%=m_equipment %>');
        break;
      case "addmeasurement":
        addmeasurement('<%=m_equipment %>');
        break;
      case "modify":
        modifyequipment('<%=m_equipment %>');
        break;
      case "template":
        URL = "../util/specificationsframe.aspx?linktype=Equipment&id=<%=m_equipment %>";
        //URL = "../util/specificationsframe.aspx";
        var labWnd = radopen(URL, null);
        labWnd.set_modal(true);
        return false;
        break;
    case "downtimereset":
        var resp = window.confirm('Are you sure want to reset Down Time?')
        if (resp) {
            downtimereset('<%=m_equipment %>');
        }
        break;
    case "savequery":
        SaveQuery();
}

}

  function saveequipment() {
    $(document).ready(function () {
      var equipment = "";
      var userid = '<%=Session["Login"] %>';
      var dirtylog = $('#txtdirtylog').val();

      var xml = "<?xml version='1.0' encoding='UTF-8'?><equipments><equipment>";
      xml = xml + collectformall("Equipment")

      xml = xml + "</equipment></equipments>";
      //alert(xml);
      if (!allowsubmit())
        return;
      $.ajax({
        type: "GET",
        url: "../InternalServices/ServiceEqpt.svc/SaveEquipment",
        data: { "xmlnvc": xml },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function (xhr, settings) {
          xhr.setRequestHeader("userid", userid);
        },
        success: function (data, status) {
          var result = data.d;
          var d_array = result.split("^");
          if (d_array[0] == "TRUE") {
            var newdirtylog
            newdirtylog = parseInt(dirtylog) + 1;

            $('#txtdirtylog').val(newdirtylog);
            equipment = d_array[1];
            window.location.href = 'equipmentmain.aspx?mode=edit&equipment=' + equipment;
          }
          else if (d_array[0] == "FALSE") {
            alert(d_array[1]);
          }
        },
        error: OnGetMemberError
      });
    });
  }

  function createequipment() {
    $(document).ready(function () {
      var userid = '<%=Session["Login"] %>';
      var xml = "<?xml version='1.0' encoding='UTF-8'?><equipments><equipment>";
      xml = xml + collectformall("Equipment");
      xml = xml + "</equipment></equipments>";

      if (!allowsubmit())
        return;
      $.ajax({
        type: "GET",
        url: "../InternalServices/ServiceEqpt.svc/CreateEquipment",
        data: { "userid": userid, "xmlnvc": xml },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, status) {
          var equipment = data.d;
          window.location.href = 'equipmentmain.aspx?mode=edit&equipment=' + equipment;
        },
        error: OnGetMemberError
      });

    });
  }

  function OnGetMemberError(request, status, error) {
    alert(status + "--" + error + "--" + request.responseText);
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
      <asp:HyperLink ID="tabhlkSpecs" runat="server">
      <asp:Image ID="tabimgSpecs" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px" Width="70px" BorderStyle="None" BorderWidth="0" style="vertical-align:bottom;" />
      <asp:Label ID="tablblSpecs" runat="server" CssClass="toptabout" Text="Additional Info" /></asp:HyperLink>
    </li>
    <li>
      <asp:HyperLink ID="tabhlkMeasurements" runat="server">
      <asp:Image ID="tabimgMeasurements" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px" Width="70px" BorderStyle="None" BorderWidth="0" style="vertical-align:bottom;" />
      <asp:Label ID="tablblMeasurements" runat="server" CssClass="toptabout" Text="Measurements" /></asp:HyperLink>
    </li>
    <li>
      <asp:HyperLink ID="tabhlkMeter" runat="server">
      <asp:Image ID="tabimgMeter" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px" Width="70px" BorderStyle="None" BorderWidth="0" style="vertical-align:bottom;" />
      <asp:Label ID="tablblMeter" runat="server" CssClass="toptabout" Text="Meter" /></asp:HyperLink>
    </li>
    <li>
      <asp:HyperLink ID="tabhlkHierarchy" runat="server">
      <asp:Image ID="tabimgHierarchy" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px" Width="70px" BorderStyle="None" BorderWidth="0" style="vertical-align:bottom;" />
      <asp:Label ID="tablblHierarchy" runat="server" CssClass="toptabout" Text="Hierarchy" /></asp:HyperLink>
    </li>

    <li>
      <asp:HyperLink ID="tabhlkHistory" runat="server">
      <asp:Image ID="tabimgHistory" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px" Width="70px" BorderStyle="None" BorderWidth="0" style="vertical-align:bottom;" />
      <asp:Label ID="tablblHistory" runat="server" CssClass="toptabout" Text="History" /></asp:HyperLink>
    </li>
    <li>
      <asp:HyperLink ID="tabhlkPMList" runat="server">
      <asp:Image ID="tabimgPMList" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px" Width="70px" BorderStyle="None" BorderWidth="0" style="vertical-align:bottom;" />
      <asp:Label ID="tablblPMList" runat="server" CssClass="toptabout" Text="PM List" /></asp:HyperLink>
    </li>
    <li>
      <asp:HyperLink ID="tabhlkPartsList" runat="server">
      <asp:Image ID="tabimgPartsList" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px" Width="70px" BorderStyle="None" BorderWidth="0" style="vertical-align:bottom;" />
      <asp:Label ID="tablblPartsList" runat="server" CssClass="toptabout" Text="Parts List" /></asp:HyperLink>
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
        </table>>
  </div>
    <asp:HiddenField ID="hidCurrentRecord" runat="server" />
</div>
