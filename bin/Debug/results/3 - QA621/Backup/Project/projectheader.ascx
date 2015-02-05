<%@ Control Language="C#" AutoEventWireup="true" CodeFile="projectheader.ascx.cs"
    Inherits="project_projectheader" %>
<link type="text/css" href="../styles/mainmenu.css" rel="stylesheet" />
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
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
    }

    function toolbarclicked(sender, e) {
        var button = e.get_item();
        var commandname = button.get_commandName();
        switch (commandname) {
            case "search":
                if (WarnUser())
                    settoQueryMode("projectmain.aspx");
                break;
            case "new":
                if (WarnUser())
                    window.location.href = 'projectmain.aspx?mode=new';
                break;
            case "lookup":
                if (WarnUser())
                    LookupClicked();
                break;
            case "savequery":
                SaveQuery();
                break;
            case "save":
                noPrompt();
                if (Page_ClientValidate('')) {
                    saveproject("");
                }
                  break;
            case "status":
                changeStatus();
                break;
            case "delete":
                var resp = window.confirm('Are you sure want to delete this Project?');
                if (resp) {
                    noPrompt();
                    deleteproject();
                  }
                break;
            case "complete":
                __doPostBack("completeproject", "");
                break;
            case "linkeddoc":
                LinkDoc('project', '<%=m_projectid %>');
                break;
            case "print":
              PrintForm("<%=defaultprintcounter %>", "ProjectPrintForm", "<%=m_projectid %>");

              break;
           case "batchprint":
              PrintBatchForms("<%=defaultprintcounter %>", "ProjectPrintForm", "ProjectId");
              break;
           case "email":
              sendEmail("Project", "<%=m_projectid%>");
              break;
        }
    }

    function completeProject() {
        var isreadytocomplete = "<%=isReadytoComplete %>";
        if (isreadytocomplete == "True") {
            savecompletedproject();
        }
        else {
            alert("There are some incompleted work orders in this project. Please complete them first.");
        }
    }

      function changeStatus() {
        var projectid = "<%=m_projectid %>";
        var oWnd;
        if (GetRadWindow() == null) {
          oWnd = radopen('projectstatus.aspx?projectid=' + projectid, null);
          oWnd.set_modal(true);
        }
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
                <asp:HyperLink ID="tabhlkComplete" runat="server">
                    <asp:Image ID="tabimgComplete" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
                        Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
                    <asp:Label ID="tablblComplete" runat="server" CssClass="toptabout" Text="Complete" /></asp:HyperLink>
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

<script type="text/javascript">
    function saveproject(postbackrequest) {
        if(!allowsubmit()) return;
        $(document).ready(function () {
            var mode = "<%=m_mode %>";
            var paramXML = "<?xml version='1.0' encoding='UTF-8'?><Projects><Project>";
            paramXML = paramXML + collectformall("Projects");
            paramXML = paramXML + "</Project></Projects>";
            $.ajax({
                type: "GET",
                url: "../InternalServices/ServiceProj.svc/SaveProject",
                data: { "paramXML": paramXML},
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend: function (xhr, settings) {
                    xhr.setRequestHeader("userid","<%=Session["Login"] %>");
                    xhr.setRequestHeader("mode", mode);
                },
                success:function (data, status) {
                    FormSubmitted = false;
                    if (postbackrequest == ""){
                        OnSaveSuccess();  
                    }
                    else{
                        __doPostBack(postbackrequest, "");
                    }
                 },
                error: OnGetMemberError
            });

        });

    }

    function OnGetMemberError(request, status, error) {
        FormSubmitted = false;
        alert(status + "--" + error + "--" + request.responseText);
    }

  


  function managelink() {
      return false;
  }


  function OnSaveSuccess() {
      var txtprojectid = document.getElementById("txtprojectid");
      window.location.href = 'projectmain.aspx?mode=edit&projectid=' + txtprojectid.value;
  }
  function OnDeleteSuccess()
  {
      FormSubmitted = false; 
      setTimeout(parent.controlpanel.RefreshPanelGridsOnDelete(), 200);
      window.location.href = 'projectmain.aspx?mode=query';
           
  }

  function deleteproject(){
    var projectid = "<%=m_projectid %>";
    if (!allowsubmit()) return;
    $.ajax({
        type: "GET",
        url: "../InternalServices/ServiceProj.svc/DelProject",
        data: { "projectid": projectid},
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function (xhr, settings) {
            xhr.setRequestHeader("userid","<%=Session["Login"] %>");
        },
        success: OnDeleteSuccess,
        error: OnGetMemberError
    });

  }

</script>
