using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Collections.Specialized;
using System.Data.OleDb;
using System.Data;

public partial class StoreRoom_srmain : System.Web.UI.Page
{

  protected AzzierScreen screen;
  protected string querymode = "";
  protected NameValueCollection m_msg = new NameValueCollection();
  protected bool isquery = false;
  protected string counter;
  protected NameValueCollection storeroomnvc;
  protected RadGrid grditemlist, grdpositionlist, grdlotlist;
  protected string m_storeroom;
  private string connstring;
  protected string m_itemnum;



  protected void Page_Init(object sender, EventArgs e)
  {
    RetrieveMessage();
    UserRights.CheckAccess("storeroom");

    if (Request.QueryString["mode"] != null)
    {
      if (Request.QueryString["mode"].ToString() != "")
      {
        querymode = Request.QueryString["mode"].ToString();
      }
      else
      {
        querymode = "query";
      }
    }
    else
    {
      querymode = "query";
    }

    if (Request.QueryString["storeroom"] != null)
    {
      if (Request.QueryString["storeroom"].ToString() != "")
      {
        m_storeroom = Request.QueryString["storeroom"].ToString();
      }
      else
      {
        m_storeroom = "";
      }
    }
    else
    {
      m_storeroom = "";

    }
    connstring = Application["ConnString"].ToString();
    InitScreen();

    if ((querymode == "edit") && (m_storeroom != ""))
    {
      StoreRoom obj = new StoreRoom(Session["Login"].ToString(), "StoreRoom", "StoreRoom", m_storeroom);
      storeroomnvc = obj.ModuleData;
    }

    if (querymode == "new")
    {
      StoreRoom obj = new StoreRoom(Session["Login"].ToString(), "StoreRoom", "StoreRoom");
      storeroomnvc = obj.ModuleData;
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {

    if ((querymode == "edit") && (m_storeroom != ""))
    {
      screen.PopulateScreen("StoreRoom", storeroomnvc);
      UserWorkList list = new UserWorkList();
      list.AddToRecentList(Session["Login"].ToString(), "StoreRoom", m_storeroom);
      grditemlist.Visible = true;
    }
    else
    {
      grditemlist.Visible = false;
    }
    SetHeaderLable();
    TextBox txtdivision = (TextBox)MainControlsPanel.FindControl("txtdivision");
    hidMode.Value = querymode;
    ucHeader1.Mode = querymode;
    ucHeader1.TabName = "Main";
    ucHeader1.Division = txtdivision.Text;
    ucHeader1.StoreRoom = m_storeroom;
    if (Request.Form["__EVENTTARGET"] == "Query")
    {
      Query();
    }

    if (querymode == "new")
    {
      screen.GetDefaultValue("StoreRoom", storeroomnvc, "Storeroom", "new");
      screen.PopulateScreen("StoreRoom", storeroomnvc);
    }
  }

  private void Query()
  {
    NameValueCollection nvc;
    Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
    if (CntlPanel != null)
    {
      screen = new AzzierScreen("StoreRoom/srmain.aspx", "MainForm", CntlPanel.Controls);
      screen.LCID = Session.LCID;
      nvc = screen.CollectFormValues("StoreRoom", true);  // use the view name
    }
    else
      nvc = null;
    ModuleoObject obj = new ModuleoObject(Session["Login"].ToString(), "StoreRoom", "StoreRoom");   // use the view name
    string wherestring = "", linqwherestring = "";
    //string[] empids = obj.Query(nvc, ref wherestring);
    obj.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
    string jscript = "<script type=\"text/javascript\">";
    jscript += "var wherestring = '" + Server.UrlEncode(linqwherestring) + "'; var obj=parent.controlpanel.document.location.href='srpanel.aspx?showlist=queryresult&wherestring='+wherestring;";

    jscript += "</script>";
    litScript1.Text = jscript;
  }


  private void SetHearder()
  {
    switch (querymode)
    {
      case "new":
        ucHeader1.OperationLabel = "New";
        break;
      case "query":
        ucHeader1.OperationLabel = "Query";
        break;
      case "edit":
        ucHeader1.OperationLabel = "Edit";
        break;
    }
  }

  private void InitScreen()
  {
    //Loading Screen Controls
    screen = new AzzierScreen("storeroom/srmain.aspx", "mainform", MainControlsPanel.Controls, querymode);
    Session.LCID = Convert.ToInt32(Session["LCID"]);
    screen.LCID = Session.LCID;
    InitGrid();
    screen.LoadScreen();
    screen.SetValidationControls();
  }

  protected void grditemlist_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
  {
    grditemlist.DataSource = GetDataTable("Select * From v_InventoryStoreroom Where StoreRoom='" + m_storeroom + "'");
  }

  private void InitGrid()
  {
    grditemlist = new RadGrid();
    grditemlist.ID = "grditemlist";
    grditemlist.DataSourceID = "StoreRoomItemListSqlDataSource";
    grditemlist.PageSize = 100;
    grditemlist.AllowPaging = true;
    grditemlist.AllowSorting = true;
    grditemlist.MasterTableView.AllowMultiColumnSorting = true;
    grditemlist.MasterTableView.AutoGenerateColumns = false;
    grditemlist.AllowFilteringByColumn = true;
    string sql = "Select * From v_InventoryStoreroom Where StoreRoom='" + m_storeroom + "'";
    if (Application["usedivision"].ToString() == "1")
    {
      if (Session["AllDivision"].ToString() != "")
      {
        sql = sql + " And (Division is null Or  Division in (" + Session["AllDivision"].ToString() + "))";
      }
      else
        sql = sql + " And (Division is null)";
    }
    StoreRoomItemListSqlDataSource.SelectCommand = sql;

    StoreRoomItemListSqlDataSource.ConnectionString = connstring;
    grditemlist.ClientSettings.EnableAlternatingItems = false;
    StoreRoomPositionListSqlDataSource.SelectCommand = "Select * From v_InventoryPosition";
    StoreRoomPositionListSqlDataSource.ConnectionString = connstring;

    StoreRoomLotListSqlDataSource.SelectCommand = "Select * From v_InventoryLot";
    StoreRoomLotListSqlDataSource.ConnectionString = connstring;



    GridHyperLinkColumn IssueColumn = new GridHyperLinkColumn();
    IssueColumn.HeaderText = "Issue";
    IssueColumn.UniqueName = "IssueCommand";
    IssueColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
    IssueColumn.HeaderStyle.Width = 30;
    IssueColumn.AllowFiltering = false;
    grditemlist.MasterTableView.Columns.Add(IssueColumn);

    GridEditCommandColumn EditColumn = new GridEditCommandColumn();
    EditColumn.HeaderText = "Edit";
    EditColumn.UniqueName = "EditCommand";
    EditColumn.ButtonType = GridButtonColumnType.ImageButton;
    EditColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
    EditColumn.HeaderStyle.Width = 30;
    grditemlist.MasterTableView.Columns.Add(EditColumn);
    grditemlist.MasterTableView.DataKeyNames = new string[] { "Counter", "ItemNum", "Storeroom", "Division" };
    screen.SetGridColumns("itemlist", grditemlist);
    //grditemlist.MasterTableView.CommandItemTemplate = new InsertFormItemTemplate("Inventory", null, "return editstoremain('');", m_allowedit);
    //grditemlist.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(1, true, "Inventory", null, "return editstoremain('','" + m_itemnum + "');", m_allowedit, "", false);
    grditemlist.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.Top;
    grditemlist.ItemDataBound += new GridItemEventHandler(grditemlist_ItemDataBound);
    grditemlist.ItemCreated += new GridItemEventHandler(grditemlist_ItemCreated);
    grditemlist.DetailTableDataBind += new GridDetailTableDataBindEventHandler(grditemlist_DetailTableDataBind);
    //grditemlist.NeedDataSource += new GridNeedDataSourceEventHandler(grditemlist_NeedDataSource);
    grditemlist.MasterTableView.HierarchyLoadMode = GridChildLoadMode.Conditional;
    grdpositionlist = new RadGrid();
    grdpositionlist.AutoGenerateColumns = false;
    //grdpositionlist.DataSourceID = "StoreRoomPositionListSqlDataSource";

    screen.SetGridColumns("positionlist", grdpositionlist);
    StoreRoomPositionListSqlDataSource.SelectCommand = "Select * From v_InventoryPosition where itemnum=? and storeroom=?";

    grdlotlist = new RadGrid();
    grdlotlist.AutoGenerateColumns = false;
    //grdlotlist.DataSourceID = "StoreRoomLotListSqlDataSource";
    screen.SetGridColumns("lotlist", grdlotlist);
    StoreRoomLotListSqlDataSource.SelectCommand = "Select * From v_InventoryLot where itemnum=? and storeroom=? and position=?";

    GridTableView positionview = new GridTableView();
    positionview.Name = "positionlist";
    positionview.AutoGenerateColumns = false;
    //positionview.SkinID = "Telerik";
    positionview.HeaderStyle.CssClass = "InnerHeaderStyle";
    positionview.ItemStyle.CssClass = "InnerItemStyle";
    positionview.AlternatingItemStyle.CssClass = "InnerItemStyle";

    positionview.HierarchyLoadMode = GridChildLoadMode.Conditional;
    for (int i = 0; i < grdpositionlist.Columns.Count; i++)
    {
      positionview.Columns.Add(grdpositionlist.Columns[i]);
    }

    grditemlist.MasterTableView.CommandItemTemplate = new InsertFormItemTemplate("Items ", null, "return addnewitem()", 1);

    //positionview.DataSourceID = "InvPositionSqlDataSource";
    positionview.DataKeyNames = new string[] { "ItemNum", "Storeroom", "Position" };

    positionview.ShowHeadersWhenNoRecords = true;
    positionview.AutoGenerateColumns = false;

    GridTableView lotview = new GridTableView();
    lotview.Name = "lotlist";
    lotview.AutoGenerateColumns = false;
    lotview.HeaderStyle.CssClass = "MostInnerHeaderStyle";
    lotview.ItemStyle.CssClass = "MostInnerItemStyle";
    lotview.AlternatingItemStyle.CssClass = "MostInnerItemStyle";
    for (int i = 0; i < grdlotlist.Columns.Count; i++)
    {
      lotview.Columns.Add(grdlotlist.Columns[i]);
    }

    //lotview.DataSourceID = "InvLotSqlDataSource";
    lotview.DataKeyNames = new string[] { "ItemNum", "Storeroom", "Position", "LotNum", "Counter" };

    positionview.DetailTables.Add(lotview);
    grditemlist.MasterTableView.DetailTables.Add(positionview);

    //grditemlist.MasterTableView.DetailTables[0].DetailTables.Add(lotview);

    MainControlsPanel.Controls.Add(grditemlist);
  }

  protected void grditemlist_ItemCreated(object sender, GridItemEventArgs e)
  {
    screen.GridItemCreated(e, "storeroom/srmain.aspx", "MainForm", "itemlist", grditemlist);
  }

  private void SetHeaderLable()
  {
    string oper = "";
    if (querymode == "query")
    {
      oper = "Query";
    }
    else if (querymode == "new")
    {
      oper = "New";
    }
    else if (querymode == "duplicate")
    {
      oper = "Duplicate";
    }
    else if (querymode == "edit")
    {
      oper = "Edit";
    }
    ucHeader1.OperationLabel = oper;
  }


  private void grditemlist_DetailTableDataBind(object source, Telerik.Web.UI.GridDetailTableDataBindEventArgs e)
  {
    GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;
    switch (e.DetailTableView.Name)
    {
      case "positionlist":
        {
          string itemnum = dataItem.GetDataKeyValue("ItemNum").ToString();
          string storeroom = dataItem.GetDataKeyValue("Storeroom").ToString();
          e.DetailTableView.DataSource = GetDataTable("SELECT * FROM v_inventoryposition WHERE ItemNum = '" + itemnum + "' And Storeroom='" + storeroom + "'");
          break;
        }

      case "lotlist":
        {
          string itemnum = dataItem.GetDataKeyValue("ItemNum").ToString();
          string storeroom = dataItem.GetDataKeyValue("Storeroom").ToString();
          string position = dataItem.GetDataKeyValue("Position").ToString();
          e.DetailTableView.DataSource = GetDataTable("SELECT * FROM v_inventorylot WHERE ItemNum = '" + itemnum + "' And Storeroom='" + storeroom + "' And Position='" + position + "'");
          break;
        }
    }
  }

  public DataTable GetDataTable(string query)
  {
    String ConnString = Application["ConnString"].ToString();
    OleDbConnection conn = new OleDbConnection(ConnString);
    OleDbDataAdapter adapter = new OleDbDataAdapter();
    adapter.SelectCommand = new OleDbCommand(query, conn);

    DataTable myDataTable = new DataTable();

    conn.Open();
    try
    {
      adapter.Fill(myDataTable);
    }
    finally
    {
      conn.Close();
    }

    return myDataTable;
  }


  protected void grditemlist_ItemDataBound(object sender, GridItemEventArgs e)
  {
    if (e.Item is GridDataItem && !e.Item.IsInEditMode)
    {
      GridDataItem item = (GridDataItem)e.Item;

      if (item.OwnerTableView.Name != "positionlist" && item.OwnerTableView.Name != "lotlist")
      {
        ImageButton btn = item["EditCommand"].Controls[0] as ImageButton;
        if (btn != null)
        {
          m_itemnum = item.OwnerTableView.DataKeyValues[item.ItemIndex]["ItemNum"].ToString();
          btn.ImageUrl = "~/images/Edit.gif";
          btn.OnClientClick = "editstoremain('" + item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString() + "','" + m_itemnum + "');return false;";
        }

        HyperLink h = item["IssueCommand"].Controls[0] as HyperLink;
        if (h != null)
        {
          Division d = new Division();
          string div = item.OwnerTableView.DataKeyValues[item.ItemIndex]["Division"].ToString();
          if (!d.Editable("'" + div + "'"))
          {
            h.Visible = false;
          }
          else
          {
            string itemnum = item.OwnerTableView.DataKeyValues[item.ItemIndex]["ItemNum"].ToString();
            string storeroom = item.OwnerTableView.DataKeyValues[item.ItemIndex]["Storeroom"].ToString();
            h.ImageUrl = "../images/inventory/issue_24.png";
            h.NavigateUrl = "javascript:issue('" + itemnum + "','" + storeroom + "')";
          }
        }
      }
    }
  }

  private void RetrieveMessage()
  {
    SystemMessage msg = new SystemMessage("labour/labourmain.aspx");
    m_msg = msg.GetSystemMessage();
    msg.SetJsMessage(litMessage);
  }
}