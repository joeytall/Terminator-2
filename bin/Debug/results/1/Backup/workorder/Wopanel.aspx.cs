using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.OleDb;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

public partial class workorder_Wopanel : System.Web.UI.Page
{
  protected string mode;

  protected RadGrid grdresults;
  protected RadGrid grdworklist;
  protected RadGrid grdrecentlist;

  private OleDbConnection conn;
  private bool setPanel;
  private bool setwolist;
  protected int woresultcount = 0;
  protected int wolistcount = 0;
  protected int recentlistcount = 0;
  protected string selectedresults;
  protected string selectedworklist;
  protected string selectedrecentlist;
  protected NameValueCollection m_msg = new NameValueCollection();
  protected AzzierScreen screen, screen2, screen3;
  protected string querystr = "";
  protected RadToolBarDropDown toolbarcombobox = new RadToolBarDropDown();

  protected void Page_Init(object sender, EventArgs e)
  {
    RetrieveMessage();
    UserRights.CheckAccess("workorder");
    Session.LCID = Convert.ToInt32(Session["LCID"]);
    if (Request.QueryString["showlist"] != null)
      mode = Request.QueryString["showlist"].ToString();
    else
      mode = "worklist";
    string wherestring = "";
    if (Request.QueryString["wherestring"] != null)
    {
      wherestring = Request.QueryString["wherestring"].ToString();
      if (wherestring == "")
      {
        wherestring = " 1=1 ";
      }
    }
    else
      wherestring = " 1=2 ";
    if (Request.QueryString["querystr"] != null)
    {
      querystr = Request.QueryString["querystr"].ToString();
    }
    hidQueryStr.Value = querystr;

    hidGetFirstRecord.Value = UserSettings.GetFirstRecordSetting("Workorder");

    grdworklist = new RadGrid();
    grdresults = new RadGrid();
    grdrecentlist = new RadGrid();

    grdrecentlist.ID = "grdrecentlist";
    grdworklist.ID = "grdworklist";
    grdresults.ID = "grdresults";


    grdworklist.GroupingSettings.CaseSensitive = false;
    grdresults.GroupingSettings.CaseSensitive = false;
    grdrecentlist.GroupingSettings.CaseSensitive = false;



    // ***** grd results

    //AzzierScreen screen = new AzzierScreen("workorder/wopanel.aspx", "MainForm", MainControlsPanel.Controls, "edit");
    screen = new AzzierScreen("workorder/wopanel.aspx", "MainForm", MainControlsPanel.Controls, "edit");

    grdresults.Attributes.Add("rules", "all");
    grdresults.AutoGenerateColumns = false;
    grdresults.AllowPaging = true;
    grdresults.PagerStyle.Position = GridPagerPosition.Bottom;
    grdresults.PagerStyle.AlwaysVisible = true;
    grdresults.PagerStyle.Visible = true;

    grdresults.PageSize = 100;
    grdresults.AllowSorting = true;
    grdresults.AllowMultiRowSelection = true;
    grdresults.AllowFilteringByColumn = true;
    grdresults.MasterTableView.AllowMultiColumnSorting = true;
    grdresults.MasterTableView.DataKeyNames = new string[] { "WoNum" };
    grdresults.MasterTableView.ClientDataKeyNames = new string[] { "WoNum" };
    grdresults.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
    grdresults.ClientSettings.EnableRowHoverStyle = true;
    grdresults.ClientSettings.Selecting.AllowRowSelect = true;
    grdresults.ClientSettings.Scrolling.SaveScrollPosition = true;
    grdresults.ClientSettings.Selecting.UseClientSelectColumnOnly = true;
    //grdresults.ClientSettings.ClientEvents.OnDataBound = "updatecounts";
    grdresults.ClientSettings.ClientEvents.OnDataBound = "SetColor";
    grdresults.ClientSettings.ClientEvents.OnRowClick = "rowClicked";
    grdresults.ClientSettings.Scrolling.AllowScroll = true;
    grdresults.SortingSettings.EnableSkinSortStyles = false;
    grdresults.ClientSettings.ClientEvents.OnCommand = "ClientGrid_OnCommand";
    grdresults.ClientSettings.Selecting.EnableDragToSelectRows = true;
    grdresults.Attributes.Add("serviceURL", "../InternalServices/ServiceWO.svc/QueryResultWO");

    grdresults.PreRender += new EventHandler(grdresults_PreRender);
    grdresults.ItemCreated += new GridItemEventHandler(grdresults_ItemCreated);
    //grdresults.ItemCreated

    GridClientSelectColumn cbColumn = new GridClientSelectColumn();
    grdresults.Columns.Add(cbColumn);
    cbColumn.HeaderStyle.Width = 25;

    screen.SetGridColumns("results", grdresults);
    MainControlsPanel.Controls.Add(grdresults);
    screen.LoadScreen();

    grdresults.Skin = "Outlook";

    //grdresults.ClientSettings.DataBinding.SelectMethod = "SearchDataAndCount?wherestring=" + wherestring;
    //grdresults.ClientSettings.DataBinding.Location = "../InternalServices/ServiceWO.svc";
    grdresults.ClientSettings.DataBinding.SortParameterType = GridClientDataBindingParameterType.Linq;
    grdresults.ClientSettings.DataBinding.FilterParameterType = GridClientDataBindingParameterType.Linq;
    grdresults.DataSource = new DataTable();

    // ***** grd worklist


    grdworklist.Attributes.Add("rules", "all");
    grdworklist.AutoGenerateColumns = false;
    grdworklist.AllowPaging = true;
    grdworklist.AllowSorting = true;
    grdworklist.AllowFilteringByColumn = true;
    grdworklist.AllowMultiRowSelection = true;
    grdworklist.PagerStyle.Position = GridPagerPosition.Bottom;
    grdworklist.PageSize = 100;
    grdworklist.MasterTableView.AllowMultiColumnSorting = true;
    grdworklist.MasterTableView.DataKeyNames = new string[] { "WoNum" };
    grdworklist.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
    grdworklist.MasterTableView.ClientDataKeyNames = new string[] { "WoNum" };
    grdworklist.ClientSettings.EnableRowHoverStyle = true;
    grdworklist.ClientSettings.Selecting.AllowRowSelect = true;
    grdworklist.ClientSettings.Scrolling.SaveScrollPosition = true;
    grdworklist.ClientSettings.ClientEvents.OnRowClick = "rowClicked";
    grdworklist.ClientSettings.Scrolling.AllowScroll = true;
    grdworklist.Skin = "Outlook";
    //grdworklist.ClientSettings.ClientEvents.OnDataBound = "updatecounts";
    grdworklist.ClientSettings.ClientEvents.OnDataBound = "SetColor";
    grdworklist.SortingSettings.EnableSkinSortStyles = false;
    grdworklist.ClientSettings.Selecting.AllowRowSelect = true;

    grdworklist.ClientSettings.Selecting.UseClientSelectColumnOnly = true;

    grdworklist.PagerStyle.Visible = true;

    //AzzierScreen screen2 = new AzzierScreen("workorder/wopanel.aspx", "MainForm", MainControlsPanel2.Controls);
    screen2 = new AzzierScreen("workorder/wopanel.aspx", "MainForm", MainControlsPanel2.Controls);


    grdworklist.PreRender += new EventHandler(grdworklist_PreRender);
    grdworklist.ItemCreated += new GridItemEventHandler(grdworklist_ItemCreated);

    GridClientSelectColumn cbColumn2 = new GridClientSelectColumn();
    cbColumn2.HeaderStyle.Width = 25;
    grdworklist.Columns.Add(cbColumn2);

    screen2.SetGridColumns("worklist", grdworklist);
    MainControlsPanel2.Controls.Add(grdworklist);
    screen2.LoadScreenWithoutDirtyLog();

    grdworklist.ClientSettings.DataBinding.SelectMethod = "WorkListDataAndCount?wherestring=" + wherestring;
    grdworklist.ClientSettings.DataBinding.Location = "../InternalServices/ServiceWO.svc";
    grdworklist.ClientSettings.DataBinding.SortParameterType = GridClientDataBindingParameterType.Linq;
    grdworklist.ClientSettings.DataBinding.FilterParameterType = GridClientDataBindingParameterType.Linq;


    // ***** recent list


    grdrecentlist.Attributes.Add("rules", "all");
    grdrecentlist.AutoGenerateColumns = false;
    grdrecentlist.AllowPaging = true;
    grdrecentlist.PagerStyle.Position = GridPagerPosition.Bottom;
    grdrecentlist.PageSize = 100;
    grdrecentlist.AllowSorting = true;
    grdrecentlist.AllowFilteringByColumn = true;
    grdrecentlist.AllowMultiRowSelection = true;
    grdrecentlist.MasterTableView.AllowMultiColumnSorting = true;
    grdrecentlist.MasterTableView.DataKeyNames = new string[] { "WoNum" };
    grdrecentlist.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
    grdrecentlist.MasterTableView.ClientDataKeyNames = new string[] { "WoNum" };
    grdrecentlist.ClientSettings.EnableRowHoverStyle = true;
    grdrecentlist.ClientSettings.Selecting.AllowRowSelect = true;
    grdrecentlist.ClientSettings.Scrolling.SaveScrollPosition = true;
    grdrecentlist.ClientSettings.Selecting.AllowRowSelect = true;
    grdrecentlist.ClientSettings.ClientEvents.OnRowClick = "rowClicked";
    grdrecentlist.ClientSettings.ClientEvents.OnDataBound = "SetColor";
    grdrecentlist.ClientSettings.Scrolling.AllowScroll = true;
    grdrecentlist.ClientSettings.Selecting.UseClientSelectColumnOnly = true;
    grdrecentlist.SortingSettings.EnableSkinSortStyles = false;
    grdrecentlist.PagerStyle.Visible = true;

    grdrecentlist.PreRender += new EventHandler(grdrecentlist_PreRender);
    grdrecentlist.ItemCreated += new GridItemEventHandler(grdrecentlist_ItemCreated);

    GridClientSelectColumn cbColumnRecentlist = new GridClientSelectColumn();
    cbColumnRecentlist.HeaderStyle.Width = 25;
    grdrecentlist.Columns.Add(cbColumnRecentlist);

    //AzzierScreen screen3 = new AzzierScreen("workorder/wopanel.aspx", "MainForm", MainControlsPanel3.Controls);
    screen3 = new AzzierScreen("workorder/wopanel.aspx", "MainForm", MainControlsPanel3.Controls);
    screen3.SetGridColumns("recentlist", grdrecentlist);
    MainControlsPanel3.Controls.Add(grdrecentlist);

    screen3.LoadScreenWithoutDirtyLog();

    grdrecentlist.Skin = "Outlook";

    grdrecentlist.ClientSettings.DataBinding.SelectMethod = "RecentListDataAndCount?wherestring=" + wherestring;
    grdrecentlist.ClientSettings.DataBinding.Location = "../InternalServices/ServiceWO.svc";
    grdrecentlist.ClientSettings.DataBinding.SortParameterType = GridClientDataBindingParameterType.Linq;
    grdrecentlist.ClientSettings.DataBinding.FilterParameterType = GridClientDataBindingParameterType.Linq;

    grdresults.Style["Width"] = "98%";
    grdresults.Style["Height"] = "98%";

    //grdresults.Width = Unit.Percentage(98);
    //grdresults.Height = Unit.Percentage(98);
    grdworklist.Style["Width"] = "98%";
    grdworklist.Style["Height"] = "98%";
    grdrecentlist.Style["Width"] = "98%";
    grdrecentlist.Style["Height"] = "98%";
  }

  protected void grdrecentlist_ItemCreated(object sender, GridItemEventArgs e)
  {
    screen3.GridItemCreated(e, "workorder/wopanel.aspx", "MainForm", "recentlist", grdrecentlist);
  }

  protected void grdworklist_ItemCreated(object sender, GridItemEventArgs e)
  {
    screen2.GridItemCreated(e, "workorder/wopanel.aspx", "MainForm", "worklist", grdworklist);
  }

  protected void grdresults_ItemCreated(object sender, GridItemEventArgs e)
  {
    screen.GridItemCreated(e, "workorder/wopanel.aspx", "MainForm", "results", grdresults);
  }

  protected void grdworklist_PreRender(object sender, EventArgs e)
  {
    if ((setPanel == true) && (setwolist == true))
    {
      MainControlsPanel2.Style.Add("visibility", "visible");
      MainControlsPanel2.Style.Add("display", "block");
      MainControlsPanel.Style.Add("visibility", "hidden");
      MainControlsPanel.Style.Add("display", "none");
      MainControlsPanel3.Style.Add("visibility", "hidden");
      MainControlsPanel3.Style.Add("display", "none");
    }

    if (hidMode.Value == "worklist")
    {
      lblListType.Text = "Work List";
    }
  }

  protected void grdresults_PreRender(object sender, EventArgs e)
  {
    if (hidMode.Value == "queryresult")
    {
      lblListType.Text = "Query Result";
    }
  }

  protected void grdrecentlist_PreRender(object sender, EventArgs e)
  {
    if (hidMode.Value == "recentlist")
    {
      lblListType.Text = m_msg["T2"];
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    litScript.Text = "";

    if (!IsPostBack)
    {
      setPanel = false;
      if (mode != "queryresult")
        setwolist = true;
      if (mode == "queryresult")
      {
        MainControlsPanel.Style.Add("visibility", "visible");
        MainControlsPanel2.Style.Add("visibility", "hidden");
        MainControlsPanel.Style.Add("display", "block");
        MainControlsPanel2.Style.Add("display", "none");
        MainControlsPanel3.Style.Add("visibility", "hidden");
        MainControlsPanel3.Style.Add("display", "none");
        lblListType.Text = m_msg["T3"];
      }
      else if (mode == "worklist")
      {
        MainControlsPanel2.Style.Add("visibility", "visible");
        MainControlsPanel.Style.Add("visibility", "hidden");
        MainControlsPanel2.Style.Add("display", "block");
        MainControlsPanel.Style.Add("display", "none");
        MainControlsPanel3.Style.Add("visibility", "hidden");
        MainControlsPanel3.Style.Add("display", "none");
        lblListType.Text = m_msg["T4"];
      }
      else if (mode == "recentlist")
      {
        MainControlsPanel2.Style.Add("visibility", "hidden");
        MainControlsPanel.Style.Add("visibility", "hidden");
        MainControlsPanel2.Style.Add("display", "none");
        MainControlsPanel.Style.Add("display", "none");
        MainControlsPanel3.Style.Add("visibility", "visible");
        MainControlsPanel3.Style.Add("display", "block");
        lblListType.Text = m_msg["T2"];
      }
      hidMode.Value = mode;

      GridSortExpression exp = new GridSortExpression();
      exp.FieldName = "wonum";
      exp.SortOrder = GridSortOrder.Ascending;
      grdworklist.MasterTableView.SortExpressions.Add(exp);
      AzzierHeader.InitToolBar(toolbar, "WorkOrderPanel");
    }
    else
    {
      setPanel = true;
      //if (Request.Form["__EVENTTARGET"] == "addworklist")
      //{
      //    AddToWorkList();
      //}
      //if (Request.Form["__EVENTTARGET"] == "removeworklist")
      //{
      //    DeleteWorkList();
      //}
    }
  }

    protected bool ShowToolbarButtonRights(string commandname, NameValueCollection nvc)
    {
        return true ;
    }

  protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
  { }

  protected void grdworklist_ItemEvent(object sender, GridItemEventArgs e)
  {
    //if (e.EventInfo is GridInitializePagerItem)
    //{
    //    /*
    //    if (!IsPostBack)
    //    {
    //      // you could save the total row count
    //      int rowTotalCount = (e.EventInfo as GridInitializePagerItem).PagingManager.DataSourceCount;
    //    }
    //     * */
    //    // here the value gets overwritten with the latest grid's items count
    //    wolistcount = (e.EventInfo as GridInitializePagerItem).PagingManager.DataSourceCount;
    //}
  }

  protected void grdresults_ItemEvent(object sender, GridItemEventArgs e)
  {
    //if (e.EventInfo is GridInitializePagerItem)
    //{
    //    woresultcount = (e.EventInfo as GridInitializePagerItem).PagingManager.DataSourceCount;
    //}
  }

  protected void grdrecentlist_ItemEvent(object sender, GridItemEventArgs e)
  {
    //if (e.EventInfo is GridInitializePagerItem)
    //{
    //    /*
    //    if (!IsPostBack)
    //    {
    //      // you could save the total row count
    //      int rowTotalCount = (e.EventInfo as GridInitializePagerItem).PagingManager.DataSourceCount;
    //    }
    //     * */
    //    // here the value gets overwritten with the latest grid's items count
    //    recentlistcount = (e.EventInfo as GridInitializePagerItem).PagingManager.DataSourceCount;
    //}
  }

  private void RetrieveMessage()
  {
    SystemMessage msg = new SystemMessage("workorder/wopanel.aspx");
    m_msg = msg.GetSystemMessage();
    msg.SetJsMessage(litMessage);
  }

}