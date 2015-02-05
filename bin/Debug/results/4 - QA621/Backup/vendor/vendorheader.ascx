<%@ Control Language="C#" AutoEventWireup="true" CodeFile="vendorheader.ascx.cs"
    Inherits="vendor_vendorheader" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<link type="text/css" href="../styles/mainmenu.css" rel="stylesheet" />
<script type="text/javascript" src="../Jscript/Header.js"></script>
<script type="text/javascript" src="../Jscript/Codes.js"></script>
<script type="text/javascript" src="../Jscript/Validation.js"></script>
<script type="text/javascript" src="../Jscript/StopDoubleClick.js"></script>
<script type="text/javascript" src="../Jscript/RadControls.js"></script>
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

    function managelink() {
        return false;
    }

    function modifylocation(loc) {
        var dWnd;
        dWnd = radopen('vendormodify.aspx?vendor=' + loc, null);
        dWnd.set_modal(true);
    }

    function toolbarclicked(sender, e) {
        var button = e.get_item();
        var commandname = button.get_commandName();
        switch (commandname) {
            case "search":
                if (WarnUser())
                    settoQueryMode("../vendor/vendormain.aspx");
                break;
            case "new":
                if (WarnUser())
                    window.location.href = 'vendormain.aspx?mode=new';
                break;
              case "autovendor":
                if (WarnUser())
                  window.location.href = 'vendormain.aspx?mode=new&numbering=auto';
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
            case "duplicate":
                if (WarnUser())
                    window.location.href = 'vendormain.aspx?mode=duplicate&vendor=<%=m_vendor %>';
                break;
            case "print":
                PrintForm("<%=defaultprintcounter %>", "VendorPrintForm", "<%=m_vendor %>");
                break;
            case "batchprint":
                PrintBatchForms("<%=defaultprintcounter %>", "VendorPrintForm", "CompanyCode");
                break;
            case "specsearch":
                break;
            case "picture":
                break;
            case "linkeddoc":
                LinkDoc('vendor', '<%=m_vendor %>');
                break;
            case "delete":
                var resp = window.confirm('<%=m_msg["T1"] %>');
                //'Are you sure want to delete this location? If Deleted, all child locations will not reference this location as parent.');
                if (resp) {
                    noPrompt();
                    __doPostBack("delete", "");
                }
                break;
            case "email":
                sendEmail("Vendor", "<%=m_vendor%>");
                break;
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
                <asp:HyperLink ID="tabhlkVendor" runat="server">
                    <asp:Image ID="tabimgSpecs" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
                        Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
                    <asp:Label ID="tablblSpecs" runat="server" CssClass="toptabout" Text="Vendor Items" /></asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="tabhlkServices" runat="server">
                    <asp:Image ID="tabimgServices" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
                        Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
                    <asp:Label ID="tablblServices" runat="server" CssClass="toptabout" Text="Services" /></asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="tabhlkContacts" runat="server">
                    <asp:Image ID="tabimgContacts" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
                        Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
                    <asp:Label ID="tablblContacts" runat="server" CssClass="toptabout" Text="Contacts" /></asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink ID="tabhlkBranch" runat="server">
                    <asp:Image ID="tabimgBranch" runat="server" ImageUrl="../IMAGES/tabbutton.png" Height="30px"
                        Width="70px" BorderStyle="None" BorderWidth="0" Style="vertical-align: bottom;" />
                    <asp:Label ID="tablblBranch" runat="server" CssClass="toptabout" Text="Branches" /></asp:HyperLink>
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
