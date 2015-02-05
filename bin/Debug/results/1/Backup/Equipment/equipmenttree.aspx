


<%@ Page Language="C#" AutoEventWireup="true" CodeFile="equipmenttree.aspx.cs" Inherits="equipment_equipmenttree" %>

<%@ Register TagPrefix="uc1" TagName="EqpHeader" Src="equipmentheader.ascx" %>
<%@ Register TagPrefix="uceqptip" TagName="EquipmentToolTip" Src="../tooltips/equipmenttooltip.ascx" %>
<%@Reference VirtualPath="../tooltips/equipmenttooltip.ascx" %> 
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link type="text/css" href="~/Styles/Azzier.css" rel="stylesheet" />
    <link type="text/css" href="~/Styles/Customer.css" rel="stylesheet" />
    <link type="text/css" href="~/Styles/Header.css" rel="stylesheet" />
    <link type="text/css" href="../css/custom-theme/jquery-ui-1.8.21.custom.css" rel="stylesheet" />
    <script type="text/javascript" src="../js/jquery-1.7.2.min.js"></script>
</head>
<body style="background-image: url(../IMAGES/back.png); background-repeat: repeat-x;
    margin: 0px; padding: 0px;">
    <form id="MainForm" runat="server">
    <uc1:EqpHeader ID="ucHeader1" runat="server" />
    <asp:Literal ID="litMessage" runat="server" />
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server">
        </telerik:RadScriptManager>
        <telerik:RadWindowManager ID="RadWindowManager1" runat="server" ShowOnTopWhenMaximized="false">
        </telerik:RadWindowManager>
    <telerik:RadToolTipManager runat="server" ID="RadToolTipManager1" Position="BottomCenter"
      RelativeTo="Element" Animation="Resize" HideEvent="LeaveTargetAndToolTip" AutoCloseDelay="0"
      Skin="Default" OnAjaxUpdate="OnAjaxUpdate" OnClientHide="OnClientHide" RenderInPageRoot="true"
      AnimationDuration="200">
    </telerik:RadToolTipManager>
    <asp:Panel ID="MainControlsPanel" runat="server" Style="position: absolute; left: 10px; top: 130px; border: thin solid #003300; background-color: transparent; border-radius: 1em;">
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
    <iframe id="ifValidate" src="" frameborder="0" scrolling="no" style="visibility: hidden;
        display: none;"></iframe>
    <span id="spndummy" style="display:none;"><%=template %></span>
    </form>
</body>
<script type="text/javascript" src="../js/jquery-1.7.2.min.js"></script>
<script type="text/javascript" src="../js/jquery-ui-1.8.21.custom.min.js"></script>
<script type="text/javascript" src="../jscript/codes.js"></script>
<script type="text/javascript" src="../jscript/codevalidate.js"></script>
<script type="text/javascript" src="../jscript/setvalue.js"></script>
<script type="text/javascript" src="../jscript/callbackfunction.js"></script>
<script type="text/javascript" src="../jscript/radwindow.js"></script>
<script type="text/javascript" src="../jscript/autocomplete.js"></script>
<script type="text/javascript" src="../jscript/stopdoubleclick.js"></script>

<script type="text/javascript">
    window.onload = function () {
      setpanelheight(1);
      initToolbar();
      var tree = $find("<%= trvequipment.ClientID %>");
      var node = tree.findNodeByValue("<%=m_equipment %>");
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
        var equipment = source.get_value();
        var parent = dest.get_value();
        updateequipmentparent(equipment, parent, sender,args)
      }
      else {
        var target = args.get_htmlElement();
        if (target.id == "txtnewparent")
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
      var obj = document.getElementById("txtnewparent");
      var newparent = obj.value;
      var value = args.get_sourceNode().get_value();
      //target.value = args.get_sourceNode().get_text();
      args.set_cancel(true);
      if (!allowsubmit())
        return;
      $(document).ready(function () {
        $.ajax({
          type: "POST",
          url: "../InternalServices/ServiceTreeView.asmx/UpdateEquipmentParent",
          //data: "{ location": value, "parentid": newparent },
          data: "{ 'equipment':'" + value + "','parentid':'" + newparent + "'}",
          dataType: "json",
          contentType: 'application/json; charset=utf-8',
          success: function (data) {
            var returnStr = data.d; // this.parent.JSON.stringify(data.d); //JSON.stringify(data.d);
            FormSubmitted = false;
            if (returnStr != "OK") {
              alert(returnStr);
            }
            else {
              //__doPostBack("ReloadEquipmentTree", "");
              document.location.href = "equipmenttree.aspx?equipment=" + "<%=m_equipment %>"
            }
          },
          error: function (xhr, status, error) { FormSubmitted = false; alert("Error\n-----\n" + xhr.status + '\n' + xhr.responseText); }
        });
      });
      return true;
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
    

    function clientSideEdit(sender, args) {
      var destinationNode = args.get_destNode();

      if (destinationNode) {
        var firstTreeView = $find('<%= trvequipment.ClientID %>');
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

    function updateequipmentparent(value, parent, sender, args) {
        if (!allowsubmit())
          return;
        $(document).ready(function () {
          $.ajax({
            type: "POST",
            url: "../InternalServices/ServiceTreeView.asmx/UpdateEquipmentParent",
            //data: "{ location": value, "parentid": newparent },
            data: "{ 'equipment':'" + value + "','parentid':'" + parent + "'}",
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
              var returnStr = data.d; // this.parent.JSON.stringify(data.d); //JSON.stringify(data.d);
              FormSubmitted = false;
              if (returnStr != "OK") {
                alert(returnStr);
              }
              else {
                clientSideEdit(sender, args);
              }
            },
            error: function (xhr, status, error) { FormSubmitted = false; alert("Error\n-----\n" + xhr.status + '\n' + xhr.responseText); }
          });
        });
      }

      
</script>
</html>
