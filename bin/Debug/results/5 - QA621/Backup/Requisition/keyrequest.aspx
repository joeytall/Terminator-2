<%@ Page Language="C#" AutoEventWireup="true" CodeFile="keyrequest.aspx.cs" Inherits="requisition_keyrequest" %>

<%@ Register TagPrefix="uc1" TagName="Header" Src="reqheader.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
  <title></title>
  <link type="text/css" href="../Styles/Azzier.css" rel="stylesheet" />
  <link type="text/css" href="../Styles/Customer.css" rel="stylesheet" />
  <link type="text/css" href="../Styles/Header.css" rel="stylesheet" />
  <link type="text/css" href="../css/custom-theme/jquery-ui-1.8.21.custom.css" rel="stylesheet" />
</head>
<body style="background-image: url(../IMAGES/back.png); background-repeat: repeat-x;
  margin: 0px; padding: 0px;">
  <asp:Literal ID="litMessage" runat="server" />
  <form id="MainForm" runat="server">
  <uc1:Header ID="ucHeader1" runat="server" />
  <asp:Panel ID="MainControlsPanel" runat="server" CssClass="MainControlsPanelClass">
    <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
    </telerik:RadScriptManager>
    <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false">
    </telerik:RadWindowManager>
    <telerik:RadAjaxManager ID="ajaxmanager" runat="server">
    </telerik:RadAjaxManager>
    <asp:LinkButton ID="btndeptsign" runat="server" OnClientClick="sign('department');return false;">
      <asp:Image ID="Image1" runat="server" ImageUrl="../images/edit.gif"
        Height="24px" Width="24px" ToolTip="Department Sign" />
    </asp:LinkButton>

    <asp:LinkButton ID="btndivisionsign" runat="server" OnClientClick="sign('division');return false;">
      <asp:Image ID="Image2" runat="server" ImageUrl="../images/edit.gif"
        Height="24px" Width="24px" ToolTip="Division Sign" />
    </asp:LinkButton>

      <asp:LinkButton ID="btnauthorizedsign" runat="server" OnClientClick="sign('authorized');return false;">
      <asp:Image ID="Image3" runat="server" ImageUrl="../images/edit.gif"
        Height="24px" Width="24px" ToolTip="Authorization" />
    </asp:LinkButton>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
    ShowSummary="False" />
  </asp:Panel>
  <asp:Literal ID="litFrameScript" runat="server" />
  <asp:HiddenField ID="hidMode" Value="edit" runat="server" />
  </form>
</body>
<script type="text/javascript" src="../js/jquery-1.7.2.min.js"></script>
  <script type="text/javascript" src="../js/jquery-ui-1.8.21.custom.min.js"></script>
  <script type="text/javascript" src="../Jscript/AutoComplete.js"></script>
  <script type="text/javascript" src="../Jscript/CodeValidate.js"></script>
  <script type="text/javascript" src="../Jscript/SetValue.js"></script>
  <script type="text/javascript" src="../Jscript/Codes.js"></script>
  <script type="text/javascript" src="../Jscript/CallBackFunction.js"></script>
  <script type="text/javascript" src="../Jscript/RadWindow.js"></script>
  <script type="text/javascript" src="../Jscript/Stopdoubleclick.js"></script>
<script type="text/javascript">
    $(window).load(function () {
        setpanelheight(1);
        initToolbar();
    });

    function sign(signtype) {
        var reqnum = "<%=m_reqnum%>";
        $.ajax({
            type: "GET",
            url: "../InternalServices/ServiceReq.svc/SignKeyRequest",
            data: { "reqnum": reqnum,"signtype":signtype },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function (xhr, settings) {
                xhr.setRequestHeader("userid", "<%=Session["Login"]%>");
            },

            success: function (data, status) {

                var result = data.d;
                var d_array = result.split("^");
                if (d_array[0] == "OK") {
                    if (signtype == "department") {
                        $("#txtdeptapprovalid").val(d_array[1]);
                        $("#txtdeptsignature").val(d_array[2]);
                        $("#txtdeptapprovaldate").val(d_array[3]);
                    }
                    else if (signtype == "division") {
                        $("#txtdivisionapprovalid").val(d_array[1]);
                        $("#txtdivisionsignature").val(d_array[2]);
                        $("#txtdivisionapprovaldate").val(d_array[2]);
                    }
                    else if (signtype == "authorized") {
                        $("#txtauthorizationid").val(d_array[1]);
                        $("#txtauthorizationsignature").val(d_array[2]);
                        $("#txtauthorizationdate").val(d_array[2]);
                    }
                }
                else {
                    alert(d_array[1]);
                }


            },
            error: OnGetMemberError
        });
    }

    function OnGetMemberError(request, status, error) {
        alert(status + "--" + error + "--" + request.responseText);
    }

    function EditKeys(index, reqnum) {
        var counter = ""
        if (index != "") {
            var grid = $find("grdkeys");
            var MasterTable = grid.get_masterTableView();
            var row = MasterTable.get_dataItems()[index];
            counter = row.getDataKeyValue("Counter");
        }
        var URL = "keydetail.aspx?counter=" + counter + "&reqnum=" + reqnum;
        if (GetRadWindow() == null) {
            oWnd = radopen(URL, null);
            window.setTimeout(function () { oWnd.setActive(true); oWnd.set_modal(true); });
        }
        else {
            var radwin = GetRadWindow();
            var oManager = GetRadWindow().get_windowManager();
            oWnd = oManager.open(URL, null);
            window.setTimeout(function () { oWnd.setActive(true); oWnd.set_modal(true); });
        }
        return false;

    }

    function refreshGrid() {
        var masterTable = $find('grdkeys').get_masterTableView();
        masterTable.rebind();
    }
</script>
</html>
