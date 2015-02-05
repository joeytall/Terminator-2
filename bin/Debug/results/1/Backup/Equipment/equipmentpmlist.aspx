﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="equipmentpmlist.aspx.cs" Inherits="equipment_equipmentpmlist" %>
<%@ Register TagPrefix="uc1" TagName="EqpHeader" Src="equipmentheader.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<title></title>
<link type="text/css" href="~/Styles/Azzier.css" rel="stylesheet" />
<link type="text/css" href="~/Styles/Customer.css" rel="stylesheet" />
<link type="text/css" href="~/Styles/Header.css" rel="stylesheet" />
<style type="text/css">
    .rgAltRow, .rgRow
    {
        cursor: pointer !important;
    }
</style>
<script type="text/javascript" src="../js/jquery-1.7.2.min.js"></script>
</head>

<body style="background-image:url(../IMAGES/back.png); background-repeat:repeat-x; margin:0px; padding:0px;">
<asp:Literal ID="litMessage" runat="server" />
<form id="MainForm" runat="server">
<uc1:EqpHeader id="ucHeader1" runat="server" />

<asp:Panel ID="MainControlsPanel" runat="server" CssClass="MainControlsPanelClass">
  <telerik:RadScriptManager ID="RadScriptManager1" runat="server"></telerik:RadScriptManager>
  <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false">
    </telerik:RadWindowManager>
  <telerik:RadAjaxManager ID="ajaxmanager" runat="server">
  </telerik:RadAjaxManager>
<asp:SqlDataSource ID="PMListSqlDataSource" runat="server" ProviderName="System.Data.OleDb"></asp:SqlDataSource>

</asp:Panel>
<asp:Literal ID="litFrameScript" runat="server" />
<asp:HiddenField id="hidMode" value="edit" runat="server" />

<iframe id="ifValidate" src="" frameborder="0" scrolling="no" style="visibility:hidden;display:none;"></iframe>
</form>
</body>
<script type="text/javascript" src="../jscript/codes.js"></script>
<script type="text/javascript" src="../Jscript/CodeValidate.js"></script>
<script type="text/javascript" src="../Jscript/radwindow.js"></script>

<script type="text/javascript">
  $(window).load(function () {
      setpanelheight(1);
      initToolbar();
  });


  function gotopm(sender, eventArgs) {
    var pmnum = eventArgs.getDataKeyValue("PmNum");
    //parent.document.location.href = "../workorder/Woframe.aspx?URL=womain.aspx&mode=edit&wonum=" + wonum;
    url = "../pm/pmframe.aspx?URL=pmmain.aspx&mode=edit&pmnum=" + pmnum;
    if (window.event.shiftKey) {
      handler = window.open(url, '_blank', 'height=' + screen.height + ',width=' + screen.width + ',resizable=yes,scrollbars=yes,toolbar=yes,location=yes,status=yes')
    }
    else if (window.event.ctrlKey) {
      //handler = window.open(url, '_new');
      handler = window.open('', '_new').location.href = url;
    }
    else
      top.document.location = url;
  }
</script>
</html>
