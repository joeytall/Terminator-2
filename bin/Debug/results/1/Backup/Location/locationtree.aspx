<%@ Page Language="C#" AutoEventWireup="true" CodeFile="locationtree.aspx.cs" Inherits="location_locationtree" %>
<%@ Register TagPrefix="uc1" TagName="LocHeader" Src="locationheader.ascx" %>
<%@ Register TagPrefix="uceqptip" TagName="EquipmentToolTip" Src="../tooltips/equipmenttooltip.ascx" %>
<%@ Register TagPrefix="ucloctip" TagName="LocationToolTip" Src="../tooltips/locationtooltip.ascx" %>
<%@Reference VirtualPath="../tooltips/equipmenttooltip.ascx" %> 
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE HTML />
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link type="text/css" href="~/Styles/Azzier.css" rel="stylesheet" />
    <link type="text/css" href="~/Styles/Customer.css" rel="stylesheet" />
    <link type="text/css" href="~/Styles/Header.css" rel="stylesheet" />
    <link type="text/css" href="../css/custom-theme/jquery-ui-1.8.21.custom.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.7.2.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.21.custom.min.js"></script>
</head>
<body style="background-image: url(../IMAGES/back.png); background-repeat: repeat-x;
    margin: 0px; padding: 0px;">
    <form id="MainForm" runat="server">
    <uc1:LocHeader ID="ucHeader1" runat="server" />
    <asp:Literal ID="litMessage" runat="server" />
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        </telerik:RadScriptManager>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false">
        </telerik:RadWindowManager>
    <telerik:RadToolTipManager runat="server" ID="RadToolTipManager1" Position="BottomCenter"
      RelativeTo="Element" Animation="Resize" HideEvent="LeaveTargetAndToolTip"   AutoCloseDelay="0"
      Skin="Default" OnAjaxUpdate="OnAjaxUpdate" OnClientHide="OnClientHide" RenderInPageRoot="true"
      AnimationDuration="200">
    </telerik:RadToolTipManager>
    <telerik:RadToolTipManager runat="server" ID="RadToolTipManager2" Position="BottomCenter"
      RelativeTo="Element" Animation="Resize" HideEvent="LeaveTargetAndToolTip"  AutoCloseDelay="0"
      Skin="Default" OnAjaxUpdate="OnAjaxUpdateLocation" OnClientHide="OnClientHide" RenderInPageRoot="true"
      AnimationDuration="200">
    </telerik:RadToolTipManager>
    <asp:Panel ID="MainControlsPanel" runat="server"  CssClass="MainControlsPanelClass">
        <telerik:RadAjaxManager ID="ajaxmanager" runat="server">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="trvlocation">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="trvlocation" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="trvequipment">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="trvequipment" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
            <ClientEvents OnRequestStart="RequestStart" OnResponseEnd="ResponseEnd" />
        </telerik:RadAjaxManager>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
            ShowSummary="False" />
    </asp:Panel>
    <asp:Literal ID="litFrameScript" runat="server" />
    <asp:HiddenField ID="hidMode" Value="edit" runat="server" />
    <asp:HiddenField ID="hidLocationNodeTemplate" runat="server" />
    <span id="spndummy" style="display:none;"><%=template %></span>
    </form>
</body>

<script type="text/javascript" src="../jscript/codes.js"></script>
<script type="text/javascript" src="../jscript/codevalidate.js"></script>
<script type="text/javascript" src="../jscript/setvalue.js"></script>
<script type="text/javascript" src="../jscript/callbackfunction.js"></script>
<script type="text/javascript" src="../jscript/radwindow.js"></script>
<script type="text/javascript" src="../jscript/autocomplete.js"></script>
<script type="text/javascript" src="../jscript/stopdoubleclick.js"></script>

<script type="text/javascript">
    $(window).load(function () {
        setpanelheight(1);
        initToolbar();
    });
    
    var aw = window.screen.availWidth;
    var ah = window.screen.availHeight;
    var cw = roundup(aw * 2 / 3, 0);
    var ch = document.documentElement.clientHeight;
    var mpanel = document.getElementById("MainControlsPanel");
    mpanel.style.width = (cw - 20) + "px";
    mpanel.style.height = (ch - 135) + "px";

    window.onload = function () {
      var tree = $find("<%= trvlocation.ClientID %>");
      var node = tree.findNodeByValue("<%=m_location %>");
      if (node != null) {
        node.expand();
        node = node.get_parent();

        while (node != null) {
          if (node.expand) {
            node.expand();
          }

          node = node.get_parent();
        }

      }
    }

    function onNodeDragging(sender, args) {
      var target = args.get_htmlElement();

      if (!target) return;
      if (target.tagName == "input") {
        target.style.cursor = "hand";
      }
      /*
      var grid = isMouseOverGrid(target)
      if (grid) {
        grid.style.cursor = "hand";
      }
      */
    }

    function onNodeDropping(sender, args) {
      var dest = args.get_destNode();
      var source = args.get_sourceNode();
//      alert(source.get_element().innerHTML);
      if (dest) {
        var location = source.get_value();
        var parent = dest.get_value();
        updatelocationparent(location, parent, sender, args)
      }
      else {
        var target = args.get_htmlElement();
        if (target.id == "txtnewlocationparent")
          dropOnNewParent(args);
      }
    }

    function dropOnTree(args) {
      var text = "";

      if (args.get_sourceNodes().length) {
        var i;
        for (i = 0; i < args.get_sourceNodes().length; i++) {
          var node = args.get_sourceNodes()[i];
          text = text + ', ' + node.get_text();
        }
      }
    }

    function dropOnNewParent(args) {
      var obj = document.getElementById("txtnewlocationparent");
      var newparent = obj.value;
      var value = args.get_sourceNode().get_value();
      //target.value = args.get_sourceNode().get_text();
      args.set_cancel(true);
      if (!allowsubmit())
        return;
      $(document).ready(function () {
        $.ajax({
          type: "POST",
          url: "../InternalServices/ServiceTreeView.asmx/UpdateLocationParent",
          //data: "{ location": value, "parentid": newparent },
          data: "{ 'location':'" + value + "','parentid':'" + newparent + "'}",
          dataType: "json",
          contentType: 'application/json; charset=utf-8',
          success: function (data) {
            var returnStr = data.d; // this.parent.JSON.stringify(data.d); //JSON.stringify(data.d);
            FormSubmitted = false;
            if (returnStr != "OK") {
              alert(returnStr);
            }
            else {
              __doPostBack("ReloadLocationTree", "");
            }
          },
          error: function (xhr, status, error) { FormSubmitted = false; alert("Error\n-----\n" + xhr.status + '\n' + xhr.responseText); }
        });
      });
      return true;
    }

    function loadequipmenttree(location) {
      //__doPostBack("LoadEquipment", location);
      var tree = $find("<%=trvequipment.ClientID %>");
      if (tree != null) {
        var node = tree.findNodeByAttribute("nodetype", "Location");
        if (node != null) {
          //tree.trackChanges();
          node.get_nodes().clear();
          node.collapse();
//          node.value = location;
          node.set_value(location);
          //node.set_expanded(false);
          node.set_text("Equipment in " + location);
           node.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.WebService);
           //tree.commitChanges();
          node.expand();
        }
      }
     }

    function CloseActiveToolTip() {
      var tooltip = Telerik.Web.UI.RadToolTip.getCurrent();
      if (tooltip) tooltip.hide();
    }

    function OnClientHide(sender, args) {
    }

    function OnClientMouseOver(sender, args) {
      var nodeElem = args.get_node();
      if (nodeElem.get_level() != 0) {
        var node = nodeElem.get_textElement();

        var tooltipManager = $find("<%= RadToolTipManager1.ClientID%>");

        //If the user hovers the image before the page has loaded, there is no manager created
        if (!tooltipManager) return;

        //Find the tooltip for this element if it has been created
        var tooltip = tooltipManager.getToolTipByElement(node);

        //Create a tooltip if no tooltip exists for such element
        if (!tooltip) {
          tooltip = tooltipManager.createToolTip(node);
          tooltip.set_value(nodeElem.get_value());
          setTimeout(function () {
            tooltip.show();
          }, 50);
        }
      }
    }

    function OnClientMouseOverLocation(sender, args) {
      var nodeElem = args.get_node();
      var node = nodeElem.get_textElement();

        var tooltipManager = $find("<%= RadToolTipManager2.ClientID%>");

        //If the user hovers the image before the page has loaded, there is no manager created
        if (!tooltipManager) return;

        //Find the tooltip for this element if it has been created
        var tooltip = tooltipManager.getToolTipByElement(node);

        //Create a tooltip if no tooltip exists for such element
        if (!tooltip) {
          tooltip = tooltipManager.createToolTip(node);
          tooltip.set_value(nodeElem.get_value());
          setTimeout(function () {
            tooltip.show();
          }, 50);
        }
    }


    function clientSideEdit(sender, args) {
      
      var destinationNode = args.get_destNode();
      
      if (destinationNode) {
        var firstTreeView = $find('<%= trvlocation.ClientID %>');
        //var secondTreeView = $find('<%= trvequipment.ClientID %>');
        firstTreeView.trackChanges();
        //secondTreeView.trackChanges();
        var sourceNodes = args.get_sourceNodes();
        var dropPosition = args.get_dropPosition();
        //Needed to preserve the order of the dragged items
        if (dropPosition == "below") {
          /*
          for (var i = sourceNodes.length - 1; i >= 0; i--) {
            var sourceNode = sourceNodes[i];
            sourceNode.get_parent().get_nodes().remove(sourceNode);

            insertAfter(destinationNode, sourceNode);
           
          }
          */
        }
        else {
          for (var i = 0; i < sourceNodes.length; i++) {
            var sourceNode = sourceNodes[i];
            var location= sourceNode.get_value();
            //sourceNode.get_parent().get_nodes().remove(sourceNode);

            if (dropPosition == "over") {
              //sourceNode.set_clientTemplate(document.getElementById("hidLocationNodeTemplate").value);
              sourceNode.expand();
              sourceNode.set_clientTemplate(document.getElementById("spndummy").innerHTML);
              sourceNode.bindTemplate();
              destinationNode.get_nodes().add(sourceNode);

              var childnodes = sourceNode.get_allNodes();
              for (var j = 0; j < childnodes.length; j++) {
                var childnode = childnodes[j];
                //alert(childnode.get_value());
                
                childnode.set_clientTemplate(document.getElementById("spndummy").innerHTML);
                
                childnode.bindTemplate();
                
              }
            }
            if (dropPosition == "above")
            {
              //insertBefore(destinationNode, sourceNode);
            }
          }
        }
        destinationNode.set_expanded(true);
        firstTreeView.commitChanges();
       // secondTreeView.commitChanges();
      }
    }

    function updatelocationparent(value, parent, sender, args) {
      if (!allowsubmit())
        return;
      $(document).ready(function () {
        $.ajax({
          type: "POST",
          url: "../InternalServices/ServiceTreeView.asmx/UpdateLocationParent",
          //data: "{ location": value, "parentid": newparent },
          data: "{ 'location':'" + value + "','parentid':'" + parent + "'}",
          dataType: "json",
          contentType: 'application/json; charset=utf-8',
          success: function (data) {
            var returnStr = data.d; // this.parent.JSON.stringify(data.d); //JSON.stringify(data.d);
            var parentDesc = returnStr;
            FormSubmitted = false;
            if (returnStr == "Unspecified error occured.") {
              alert(returnStr);
            }
            else {
              clientSideEdit(sender, args);
              if (value == "<%=m_location %>")
              {
                setvalue("txtparentid", parent);
                setvalue("txtparentdesc", parentDesc);
              }
            }
          },
          error: function (xhr, status, error) { FormSubmitted = false; alert("Error\n-----\n" + xhr.status + '\n' + xhr.responseText); }
        });
      });
      }

      function equipmentnodeclick(sender, eventArgs) {

        var node = eventArgs.get_node();
        var equipment = node.get_value();
        if (node.get_parent() != node.get_treeView()) {
          var url = "../equipment/equipmentframe.aspx?URL=equipmentmain.aspx&mode=edit&equipment=" + equipment;
          var width = <%=Session["ScreenWidth"] %>;
          var height = <%=Session["ScreenHeight"] %>;
          //var feature = "resizable=yes,width=" + width + ",height=" + height + ",top=100,left=0";
          
          //newwin = window.open(url,null,"resizable=yes,width=" + width + ",height=" + height + ",top=10,left=10");  This will only open 1 equipment module if previous one is not closed;
          newwin = window.open(url,"","resizable=yes,width=" + width + ",height=" + height + ",top=10,left=10");  //This will only always open new equipment module even if previous one is not closed; 
          newwin.focus();
        }
      }
      
</script>
</html>
