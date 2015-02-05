function getColumnIndexFromGrid(grid, uniqueName) {
  var grd = $find(grid);
  if ( grd === null )
    return "Grid not found";

  var MasterTable = grd.get_masterTableView(),
      column = MasterTable.getColumnByUniqueName(uniqueName);

  if ( column === null )
    return "Column not found!";
  else
    return column.get_element().cellIndex;
}

function getSpecificGridItem(grid, rowIndex, column) {
  //column must be in its original datafield name
  var grd = $find(grid);
  if ( grd === null )
    return "Grid not found";

  var MasterTable = grd.get_masterTableView(),
      rows = MasterTable.get_dataItems(),
      row = rows[rowIndex];
  if ( row === undefined )
    return "rowIndex out of range!";

  var tr = row.get_cell(column);
  if ( tr === null )
    if ( row.getDataKeyValue(column) !== undefined )
      return row.getDataKeyValue(column);
    else
      return "Column not found!";

  var input = $(tr).children();
  if ( input.length === 0 )
    return $(tr).text();
  else
    return input.val();
}

function getSelectedLookupItems() {
    var gridID = $(".RadGrid").attr("id"),
        radwin = GetRadWindow(),
        openerwin = GetOpener(radwin),
        columnName = $("#hidControlId").val(),
        fieldName = $("#hidFieldId").val(),
        items = getSelectedGridItems(gridID, columnName),
        values = items.toString(),
        mode = getUrlParameter("mode"),
        opener = (openerwin.document == null) ? "RadWindow" : "BrowerWindow";

    if (values == "unselected")
    {
        alert("please select items!");
        return;
    }
    else if (items.length > 1 && mode != "report")
        values = values.replace(/,/g, '+');

    if (opener == "RadWindow")
    {
        var contentWindow = openerwin.get_contentFrame().contentWindow;
        contentWindow.setvalue(fieldName, values);
        contentWindow.generalValidateField(fieldName);
        contentWindow.dataChanged();
    }else if (opener == "BrowerWindow")
    {
        $("#" + fieldName, openerwin.document).val(values);
        openerwin.generalValidateField(fieldName);
        openerwin.dataChanged();
    }

    radwindowClose();
}

function getSelectedGridItems(grid, column) {
  var grd = $find(grid);
  if ( grd === null )
    return "Grid not found";

  var MasterTable = grd.get_masterTableView(),
      selectedRows = MasterTable.get_selectedItems(),
      itemsCount = selectedRows.length,
      dataList = [],
      singleDataItem;

  try{
    singleDataItem = selectedRows[0].get_dataItem();
  }catch(e){
    return "unselected";
  }

  if (itemsCount === 1)
    if ( column !== undefined && singleDataItem !== null && singleDataItem[column] !== undefined)
      return [singleDataItem[column]];
    //else
    //  return singleDataItem;

  for ( var i in selectedRows )
  {
    if (singleDataItem === null)
    {
      var row = selectedRows[i],
          value = row.get_cell(column);
      if ( value !== null )
        dataList.push(value.innerHTML)
    }
    else
    {
      var dataItem = selectedRows[i].get_dataItem();
      if ( column !== undefined && dataItem[column] !== undefined)
        dataList.push(dataItem[column]);
      else
        dataList.push(dataItem);
    }
  }

  return dataList;
}

function GetSelectedDropDown(cbb, valueType){
  var dropDownList = $find(cbb);
  if (dropDownList === null)
    return "not found";

  var dropDownItem = dropDownList.get_selectedItem();
  if (dropDownItem === null)
    return "unselected";
  else
  {
    if ( valueType == "text" )
      return dropDownItem.get_text();
    else if ( valueType === "value" )
      return dropDownItem.get_value();
    else
      return "type not found";
  }
}

function resetDropDown(cbb, text){
  var dropDownList = $find(cbb);
  if (dropDownList === null)
    return "not found";

  dropDownList.set_text(text);
  dropDownList.set_value("unselected");
}

function getUrlParameter(sParam, symbol){
  var sPageURL = window.location.search.substring(1),
  separator = (symbol === undefined)? '&' : symbol,
  sURLVariables = sPageURL.split(separator);
  for (var i = 0; i < sURLVariables.length; i++){
    var sParameterName = sURLVariables[i].split('=');
    if (sParameterName[0] == sParam)
      return sParameterName[1];
  }
}

function postbackScheduler() {
    __doPostBack('SchedulerPanel', 'refresh');
}

function OpenRadWindow(url) {
  var oWnd;
  if (GetRadWindow() === null) {
    oWnd = radopen(url, null);
    window.setTimeout( bringRadWinToTop(oWnd) );
  }
  else {
    var radwin = GetRadWindow(),
    oManager = GetRadWindow().get_windowManager();
    oWnd = oManager.open(url, null);
    window.setTimeout( bringRadWinToTop(oWnd) );
  }
}

function bringRadWinToTop(oWnd){
  oWnd.setActive(true);
  oWnd.set_modal(true);
  oWnd.__openerRadWindow = GetRadWindow();
}

function getDateNow(){
  var d = new Date(),
      month = d.getMonth()+1,
      day = d.getDate(),
      output = d.getFullYear() + '/' +
        (month<10 ? '0' : '') + month + '/' +
        (day<10 ? '0' : '') + day;

  return output;
}

function getSumValue(className, type){
  var method = (type === undefined)?"val":"text",
  values = $('.'+className).map(function(){
    if ( method == "val" )
    return $(this).val();
    else
    return $(this).text();
  }),
      sum = 0;
  $.each(values,function(){sum+=parseFloat(this) || 0;});
  return sum;
}

function OpenRadWindow(url) {
    var oWnd;
    if (GetRadWindow() === null) {
        oWnd = radopen(url, null);
        window.setTimeout(bringRadWinToTop(oWnd));
    }
    else {
        var radwin = GetRadWindow(),
        oManager = GetRadWindow().get_windowManager();
        oWnd = oManager.open(url, null);
        window.setTimeout(bringRadWinToTop(oWnd));
    }
}

  function CollectDataToJSON(table) {
      var data = {};
      $("input[dbtable=" + table + "], textarea[dbtable=" + table + "], table[dbtable=" + table + "] :checked, .RadComboBox[dbtable=" + table + "] .rcbInput").each(function () {
          var name = $(this).attr("name"),
          value = $(this).hasClass("rcbInput")?GetSelectedDropDown(name,"value"):$(this).val();
          if (value !== "" && value !== "unselected")
              data[name.slice(3)] = value;
      });
      return JSON.stringify(data);
  }

  function ClientGrid_OnCommand(sender, args) {
      var command = args.get_commandName(),
      commands = ["Page", "PageSize", "Filter", "Sort"];
      if ($.inArray(command, commands) != -1)
          TriggerGrid(sender, args);
  }

  function TriggerGrid(sender, args) {
      if (args !== undefined)
          args.set_cancel(true);

      var grdID = sender.get_id(),
          currentPageIndex = sender.get_masterTableView().get_currentPageIndex(),
      pageSize = sender.get_masterTableView().get_pageSize(),
      sortExpressions = sender.get_masterTableView().get_sortExpressions(),
      filterExpressions = sender.get_masterTableView().get_filterExpressions(),
      sortExpressionsAsSQL = sortExpressions.toString(),
      filterExpressionsAsSQL = filterExpressions.toDynamicLinq(),
      maximumRows = 100,
      searchQuery = $("#" + grdID).attr("searchQuery"),
      data = {
          "startRowIndex": currentPageIndex * pageSize,
          "maximumRows": maximumRows,
          "sortExpression": sortExpressionsAsSQL,
          "filterExpression": filterExpressionsAsSQL
      };
      $("#" + grdID).attr("rebindType", "OnCommand");

      if (searchQuery != undefined)
          data["jsonData"] = searchQuery;

      RebindDataToGrid(data, sender);
  }

  function LookupGrid(dbtable, grdID) {
      var jsonData = parent.window.frames[0].CollectDataToJSON(dbtable);
      QueryDataBind(jsonData, grdID);
      $("#" + grdID).attr("rebindType", "Lookup");
  }

  function UserQuery(queryID, grdID) {
      var jsonData = "{ 'queryID': " + queryID + " }";
      $("#" + grdID).attr("rebindType", "Query");
      QueryDataBind(jsonData, grdID);
  }

  function QueryDataBind(jsonData, grdID) {
      var data = CreateData(jsonData);
      $("#hidpushrecord").val("1");
      RebindDataToGrid(data, grdID);
      $("#hidMode").val("queryresult");
      $("#" + grdID).attr("searchQuery", jsonData)
  }

  function CreateData(jsonData) {
      var data = {
          "startRowIndex": 0,
          "maximumRows": 100,
          "sortExpression": "",
          "filterExpression": "",
          "jsonData": jsonData
      };
      return data;
  }

  function RebindDataToGrid(data, grid, type) {
      var grid = $.type(grid) == "string" ? $find(grid) : grid;
      if (grid == null)
          return "grid not found!";

      var grdID = grid.get_id(),
          url = $("#" + grdID).attr("serviceURL"),
          type = type == undefined ? "GET" : type;

      $.ajax({
          type: type,
          url: url,
          data: data,
          dataType: "json",
          contentType: "application/json; charset=utf-8",
          success: function (data, status) {
              FormSubmitted = false;
              OnGetListSuccess(grid, data);
          },
          error: OnGetListError
      });
  }

  function OnGetListError(request, status, error) {
      FormSubmitted = false;
      alert(status + "--" + error + "--" + request.responseText);
  }

  function OnGetListSuccess(grid, result) {
      var tableview = grid.get_masterTableView(),
          grdID = grid.get_id(),
          rebindType = $("#" + grdID).attr("rebindType"),
          highlightedItemKey = $("#" + grdID).attr("highlightedItemKey");
      tableview.set_dataSource(result.d.Data);
      tableview.set_virtualItemCount(result.d.Count);
      tableview.dataBind();
      ClearSelection(grid, null);
      SwitchPanel();
      if ( $("#hidpushrecord").val() == "1" && GetGridRowsCount(grdID) !== 0 )
          getRecord();

      if (grdID == "grdresults")
          SetColor(grid, null);

  }

  function RefreshAllGrids() {
      var grids = $(".RadGrid");
      for (var i = 0; i < grids.length; i++)
      {
          var grdID = grids.eq(i).attr("id"),
              grid = $find(grdID),
              masterTableView = grid.get_masterTableView(),
              searchQuery = $("#" + grdID).attr("searchQuery"),
              data;

          if ( grdID == "grdresults")
          {
              if (searchQuery != undefined)
              {
                  data = CreateData(searchQuery);
                  RebindDataToGrid(data, grid);
              }
              else
                  masterTableView.rebind();
          }
          else
              masterTableView.rebind();

      }
  }

  function SwitchPanel(panel) {
      var mode = $("#hidMode").val(),
          label = "", grdID = "", number,
      gridlist = [
          { "mode": "queryresult", "label": "Query Result" },
          { "mode": "worklist", "label": "Work List" },
          { "mode": "recentlist", "label": "Recent List" } ];

      if (panel != undefined)
      {
          var grid = gridlist[panel - 1];
          mode = grid.mode;
          label = grid.label;
          number = panel;
          $("#hidMode").val(mode);
      }
      else {
          for (var i = 0; i < gridlist.length; i++) {
              var grid = gridlist[i];
              if (grid.mode == mode) {
                  mode = grid.mode;
                  label = grid.label;
                  number = i == 0 ? "" : i + 1;
                  break;
              }
          }
      }
      $("#lblListType").text(label);
      $(".PanelControlsPanelClass").hide().promise().done(function () {
          $("#MainControlsPanel" + number).show();
      });
  }

function HighlightSelection(fieldValue, color)
{        
    var keyName = $("#hidKeyName", parent.window.frames[0].document).val(),
        keyValue = $("#" + keyName, parent.window.frames[0].document).val(),
        mode = $("#hidMode").val(),
        grdID = "grd" + mode;

    fieldValue = keyValue;
    color = color == undefined ? "lime" : color;
    grdID = mode == "queryresult" ? "grdresults" : grdID;
    /*
    var grid = $find(grdID),
        masterTable = grid.get_masterTableView(),
        fieldName = masterTable.get_clientDataKeyNames()[0],
        selectedRow = GetRowFromGrid(grdID, fieldName, fieldValue);
        allRows = masterTable.get_dataItems();

        for (var i = 0; i < allRows.length; i++) {
            if (allRows[i].get_dataItem()[fieldName] == fieldValue)
                allRows[i].get_element().style.backgroundColor = 'lime';
            else
                allRows[i].get_element().style.backgroundColor = '';
        }
        */
    PaintGrid("grdworklist", fieldValue);
    PaintGrid("grdrecentlist", fieldValue);
    PaintGrid("grdresults", fieldValue);
    //if (typeof (selectedRow) != "string") {
    //    PaintRow(selectedRow, color);
    //    $("#" + grdID).attr("highlightedItemKey", fieldValue);
    //}
    //else
    //    return selectedRow;
  }


function GetRowFromGrid(grdID, field, match)
{
    var grid;
    try{
        grid = $find(grdID);
    }
    catch (e) { return e.error;}

    if (grid === null)
        return 'Grid not found';

  var rows = grid.get_masterTableView().get_dataItems();
  for (var i = 0; i < rows.length; i++)
  {
    var rowItem = rows[i],
    value = rowItem.get_dataItem()[field];
    if (value == undefined)
        return 'Field not found!'
    else if ( value == match)
        return rowItem;
  }
    return 'Row not found!'
}

function SetColor(sender, args) {
    var keyName = $("#hidKeyName", parent.window.frames[0].document).val(),
        keyValue = $("#" + keyName, parent.window.frames[0].document).val();
    PaintGrid(sender.get_id(), keyValue);
}

function PaintGrid(grdID,fieldvalue) {
    var grid = parent.window.frames[1].$find(grdID);
    if (grid != null) {
        var masterTable = grid.get_masterTableView(),
            fieldName = masterTable.get_clientDataKeyNames()[0];
            //selectedRow = GetRowFromGrid(grdID, fieldName, fieldValue);
        var allRows = masterTable.get_dataItems();

        for (var i = 0; i < allRows.length; i++) {
            if (allRows[i].get_dataItem()[fieldName] == fieldvalue)
                allRows[i].get_element().style.backgroundColor = 'lime';
            else
                allRows[i].get_element().style.backgroundColor = '';
        }
    }
}

function PaintRow(row, color) {
    row.get_element().style.backgroundColor = color;
}


function GetGridRowsCount(grdID) {
  var grd = $find(grdID);
  if ( grd === null )
    return "Grid not found";

  var masterTable = grd.get_masterTableView(),
      rows = masterTable.get_dataItems(),
      rowsCount = rows.length;

  return rowsCount;
}

function LookupClicked() {
    var mainPanel = parent.window.frames[0],
        controlPanel = parent.window.frames[1],
        keyName = $("#hidKeyName", mainPanel.document).val(),
        dbtable = $("#" + keyName).attr("dbtable"),
        queryStr = collectformalltostring(dbtable);
    $("#hidQueryStr", controlPanel.document).val(queryStr);
    controlPanel.LookupGrid(dbtable, "grdresults");
}

function SpecSearch(data) {
    var mainPanel = parent.window.frames[0],
        controlPanel = parent.window.frames[1],
        keyName = $("#hidKeyName", mainPanel.document).val(),
        jsonData = {};
    jsonData[keyName.slice(3)] = data.replace(/\,/g, '+');
    controlPanel.QueryDataBind(JSON.stringify(jsonData), "grdresults");
    $("#grdresults", controlPanel.document).attr("rebindType", "Lookup");
}