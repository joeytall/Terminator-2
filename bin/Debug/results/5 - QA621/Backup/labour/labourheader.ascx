<%@ Control Language="C#" AutoEventWireup="true" CodeFile="labourheader.ascx.cs"
    Inherits="labour_labourheader" %>
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
                    settoQueryMode("labourmain.aspx");
                break;
            case "new":
                if (WarnUser())
                    window.location.href = 'labourmain.aspx?mode=new';
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
                  if (!allowsubmit())
                    return;
                  __doPostBack("save", "");
                }
                break;
            case "delete":
                var resp = window.confirm('Are you sure want to delete this employee?');
                if (resp) {
                    noPrompt();
                    __doPostBack("delete", "");
                }
                break;
            case "duplicate":
                if (WarnUser())
                    window.location.href = 'labourmain.aspx?mode=duplicate&empid=<%=m_empid %>';
                break;
            case "print":
                PrintForm("<%=defaultprintcounter %>", "LabourPrintForm", "<%=m_empid %>");
                break;
            case "batchprint":
                PrintBatchForms("<%=defaultprintcounter %>", "LabourPrintForm", "Empid");
                break;
            case "specsearch":
                break;
            case "picture":
                break;
            case "linkeddoc":
                LinkDoc('labour', '<%=m_empid %>');
                break;
            case "template":
                URL = "../util/specificationsframe.aspx?linktype=labour&id=<%=m_empid %>";
                var labWnd = radopen(URL, null);
                labWnd.set_modal(true);
                return false;
                break;
            case "scheduler":
                openScheduler();
                break;
            case "email":
                sendEmail("Labour", "<%=m_empid%>");
                break;
        }
    }

    function openScheduler() {
      var URL = "../scheduler/labourScheduler.aspx";
      var maxwidth = "<%= Session["ScreenWidTh"] %>" - 10;
      var maxheight = "<%= Session["ScreenHeight"] %>" - 10;
      newwin = window.open(URL, "Scheduler", 'width=<%=Session["ScreenWidTh"] %>,height=<%=Session["ScreenHeight"] %>, Left=0,Top=0,resizable=yes,menubar=yes,scrollbars=yes, statusbar=yes');
      newwin.focus();
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
