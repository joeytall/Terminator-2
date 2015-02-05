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
using System.Web.UI.HtmlControls;

public partial class purchase_purchasepanel : System.Web.UI.Page
{
    protected string mode;
    protected RadGrid grdresults;
    protected RadGrid grdworklist;
    protected RadGrid grdrecentlist;
    private bool setPanel;
    private bool setworklist;
    protected int resultcount = 0;
    protected int worklistcount = 0;
    protected int recentlistcount = 0;
    protected string selectedresults;
    protected string selectedworklist;
    protected string selectedrecentlist;
    protected string specsearch = "";
    protected NameValueCollection m_msg = new NameValueCollection();
    protected AzzierScreen screen, screen2, screen3;

    protected void Page_Init(object sender, EventArgs e)
    {
        RetrieveMessage();
        UserRights.CheckAccess("purchase");

        Session.LCID = Convert.ToInt32(Session["LCID"]);

        if (Request.QueryString["showlist"] != null)
            mode = Request.QueryString["showlist"].ToString();
        else
            mode = "worklist";

        if (Request.QueryString["specsearch"] != null)
        {
            specsearch = Request.QueryString["specsearch"];
        }

        string wherestring = "";
        if (Request.QueryString["wherestring"] != null)
        {
            wherestring = Request.QueryString["wherestring"].ToString();
            if (wherestring == "")
                wherestring = " 1=1 ";
        }
        else
            wherestring = " 1=2 ";

        hidGetFirstRecord.Value = UserSettings.GetFirstRecordSetting("purchase");

        grdworklist = new RadGrid();
        grdresults = new RadGrid();
        grdrecentlist = new RadGrid();

        grdworklist.ID = "grdworklist";
        grdresults.ID = "grdresults";
        grdrecentlist.ID = "grdrecentlist";

        screen = new AzzierScreen("purchase/popanel.aspx", "MainForm", MainControlsPanel.Controls, "edit");

        grdworklist.GroupingSettings.CaseSensitive = false;
        grdresults.GroupingSettings.CaseSensitive = false;
        grdrecentlist.GroupingSettings.CaseSensitive = false;

        // Purchase Results Grid

        grdresults.Attributes.Add("rules", "all");
        grdresults.AutoGenerateColumns = false;
        grdresults.AllowPaging = true;
        grdresults.PagerStyle.Position = GridPagerPosition.Bottom;
        grdresults.PageSize = 100;
        grdresults.AllowSorting = true;
        grdresults.MasterTableView.AllowMultiColumnSorting = true;
        grdresults.AllowFilteringByColumn = true;
        grdresults.MasterTableView.DataKeyNames = new string[] { "PoNum" };
        grdresults.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
        grdresults.ClientSettings.EnableRowHoverStyle = true;
        grdresults.ClientSettings.Selecting.AllowRowSelect = true;
        grdresults.AllowMultiRowSelection = true;
        grdresults.ClientSettings.Scrolling.SaveScrollPosition = true;
        grdresults.ClientSettings.Selecting.UseClientSelectColumnOnly = true;
        grdresults.PreRender += new EventHandler(grdresults_PreRender);
        grdresults.ItemEvent += new GridItemEventHandler(grdresults_ItemEvent);
        grdresults.ItemCreated += new GridItemEventHandler(grdresults_ItemCreated);
        grdresults.PagerStyle.Visible = true;
        grdresults.ClientSettings.ClientEvents.OnRowClick = "rowClicked";
        grdresults.ClientSettings.Scrolling.AllowScroll = true;
        grdresults.SortingSettings.EnableSkinSortStyles = false;
        grdresults.MasterTableView.ClientDataKeyNames = new string[] { "PoNum" };

        GridClientSelectColumn cbColumn = new GridClientSelectColumn();
        grdresults.Columns.Add(cbColumn);
        cbColumn.HeaderStyle.Width = 25;
        screen.SetGridColumns("results", grdresults);
        MainControlsPanel.Controls.Add(grdresults);
        screen.LoadScreen();

        grdresults.Skin = "Outlook";

        grdresults.ClientSettings.ClientEvents.OnCommand = "ClientGrid_OnCommand";
        grdresults.ClientSettings.Selecting.EnableDragToSelectRows = true;
        grdresults.Attributes.Add("serviceURL", "../InternalServices/ServicePO.svc/QueryResultPO");
        //grdresults.ClientSettings.DataBinding.SelectMethod = "SearchlistData?wherestring=" + wherestring;
        //grdresults.ClientSettings.DataBinding.Location = "../InternalServices/ServicePO.svc";
        grdresults.ClientSettings.DataBinding.SortParameterType = GridClientDataBindingParameterType.Linq;
        grdresults.ClientSettings.DataBinding.FilterParameterType = GridClientDataBindingParameterType.Linq;

        grdresults.DataSource = new DataTable();

        // Purchase worklist grid

        grdworklist.Attributes.Add("rules", "all");
        grdworklist.AutoGenerateColumns = false;
        grdworklist.AllowPaging = true;
        grdworklist.PageSize = 100;
        grdworklist.AllowSorting = true;
        grdworklist.MasterTableView.AllowMultiColumnSorting = true;
        grdworklist.AllowFilteringByColumn = true;
        grdworklist.MasterTableView.DataKeyNames = new string[] { "PoNum" };
        grdworklist.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
        grdworklist.ClientSettings.EnableRowHoverStyle = true;
        grdworklist.ClientSettings.Selecting.AllowRowSelect = true;
        grdworklist.ClientSettings.Scrolling.SaveScrollPosition = true;
        grdworklist.PreRender += new EventHandler(grdworklist_PreRender);
        grdworklist.ItemEvent += new GridItemEventHandler(grdworklist_ItemEvent);
        grdworklist.ItemCreated += new GridItemEventHandler(grdworklist_ItemCreated);
        grdworklist.ClientSettings.ClientEvents.OnRowClick = "rowClicked";
        grdworklist.ClientSettings.ClientEvents.OnDataBound = "SetColor";
        grdworklist.MasterTableView.ClientDataKeyNames = new string[] { "PoNum" };
        grdworklist.ClientSettings.Scrolling.AllowScroll = true;
        grdworklist.Skin = "Outlook";
        grdworklist.SortingSettings.EnableSkinSortStyles = false;

        screen2 = new AzzierScreen("purchase/popanel.aspx", "MainForm", MainControlsPanel2.Controls);

        grdworklist.ClientSettings.Selecting.AllowRowSelect = true;
        grdworklist.AllowMultiRowSelection = true;

        grdworklist.ClientSettings.Selecting.UseClientSelectColumnOnly = true;
        grdworklist.PagerStyle.Visible = true;
        GridClientSelectColumn cbColumn2 = new GridClientSelectColumn();
        cbColumn2.HeaderStyle.Width = 25;
        grdworklist.Columns.Add(cbColumn2);

        screen2.SetGridColumns("worklist", grdworklist);
        MainControlsPanel2.Controls.Add(grdworklist);
        screen2.LoadScreenWithoutDirtyLog();

        grdworklist.ClientSettings.DataBinding.SelectMethod = "WorkListData?wherestring=" + wherestring;
        grdworklist.ClientSettings.DataBinding.Location = "../InternalServices/ServicePO.svc";
        grdworklist.ClientSettings.DataBinding.SortParameterType = GridClientDataBindingParameterType.Linq;
        grdworklist.ClientSettings.DataBinding.FilterParameterType = GridClientDataBindingParameterType.Linq;


        // Purchase Recent list Grid

        grdrecentlist.Attributes.Add("rules", "all");
        grdrecentlist.AutoGenerateColumns = false;
        grdrecentlist.AllowPaging = true;
        grdrecentlist.PagerStyle.Position = GridPagerPosition.Bottom;
        grdrecentlist.PageSize = 100;
        grdrecentlist.AllowSorting = true;
        grdrecentlist.MasterTableView.AllowMultiColumnSorting = true;
        grdrecentlist.AllowFilteringByColumn = true;
        grdrecentlist.MasterTableView.DataKeyNames = new string[] { "PoNum" };
        grdrecentlist.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
        grdrecentlist.ClientSettings.EnableRowHoverStyle = true;
        grdrecentlist.ClientSettings.Selecting.AllowRowSelect = true;
        grdrecentlist.ClientSettings.Scrolling.SaveScrollPosition = true;
        grdrecentlist.PreRender += new EventHandler(grdrecentlist_PreRender);
        grdrecentlist.ItemEvent += new GridItemEventHandler(grdrecentlist_ItemEvent);
        grdrecentlist.ItemCreated += new GridItemEventHandler(grdrecentlist_ItemCreated);

        screen3 = new AzzierScreen("purchase/popanel.aspx", "MainForm", MainControlsPanel3.Controls);

        grdrecentlist.ClientSettings.Selecting.AllowRowSelect = true;
        grdrecentlist.AllowMultiRowSelection = true;
        grdrecentlist.PagerStyle.Visible = true;
        grdrecentlist.ClientSettings.ClientEvents.OnRowClick = "rowClicked";
        grdrecentlist.ClientSettings.ClientEvents.OnDataBound = "SetColor";
        grdrecentlist.ClientSettings.Scrolling.AllowScroll = true;
        grdrecentlist.SortingSettings.EnableSkinSortStyles = false;
        grdrecentlist.MasterTableView.ClientDataKeyNames = new string[] { "PoNum" };
        grdrecentlist.ClientSettings.Selecting.UseClientSelectColumnOnly = true;
        GridClientSelectColumn cbColumnRecentlist = new GridClientSelectColumn();
        cbColumnRecentlist.HeaderStyle.Width = 25;

        grdrecentlist.Columns.Add(cbColumnRecentlist);
        screen3.SetGridColumns("recentlist", grdrecentlist);
        MainControlsPanel3.Controls.Add(grdrecentlist);

        screen3.LoadScreenWithoutDirtyLog();

        grdrecentlist.Skin = "Outlook";

        //grdrecentlist.ClientSettings.DataBinding.SelectMethod = "RecentListData?wherestring=" + wherestring;
        grdrecentlist.ClientSettings.DataBinding.SelectMethod = "RecentlistDataAndCount?wherestring=" + wherestring;
        grdrecentlist.ClientSettings.DataBinding.Location = "../InternalServices/ServicePO.svc";
        grdrecentlist.ClientSettings.DataBinding.SortParameterType = GridClientDataBindingParameterType.Linq;
        grdrecentlist.ClientSettings.DataBinding.FilterParameterType = GridClientDataBindingParameterType.Linq;

        grdresults.PreRender += new EventHandler(grdresults_PreRender);

        grdworklist.EnableLinqExpressions = false;
        grdrecentlist.EnableLinqExpressions = false;
        grdresults.EnableLinqExpressions = false;

        grdresults.Style["Width"] = "98%";
        grdresults.Style["Height"] = "98%";
        grdworklist.Style["Width"] = "98%";
        grdworklist.Style["Height"] = "98%";
        grdrecentlist.Style["Width"] = "98%";
        grdrecentlist.Style["Height"] = "98%";
    }

    protected void grdrecentlist_ItemCreated(object sender, GridItemEventArgs e)
    {
        screen3.GridItemCreated(e, "purchase/popanel.aspx", "MainForm", "recentlist", grdrecentlist);
    }

    protected void grdworklist_ItemCreated(object sender, GridItemEventArgs e)
    {
        screen2.GridItemCreated(e, "purchase/popanel.aspx", "MainForm", "worklist", grdworklist);
    }

    protected void grdresults_ItemCreated(object sender, GridItemEventArgs e)
    {
        screen.GridItemCreated(e, "purchase/popanel.aspx", "MainForm", "results", grdresults);
    }

    protected void grdworklist_PreRender(object sender, EventArgs e)
    {
        if ((setPanel == true) && (setworklist == true))
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
                setworklist = true;
            if (mode == "queryresult")
            {
                MainControlsPanel.Style.Add("visibility", "visible");
                MainControlsPanel2.Style.Add("visibility", "hidden");
                MainControlsPanel.Style.Add("display", "block");
                MainControlsPanel2.Style.Add("display", "none");
                MainControlsPanel3.Style.Add("visibility", "hidden");
                MainControlsPanel3.Style.Add("display", "none");
                lblListType.Text = m_msg["T3"];

                grdresults.ClientSettings.ClientEvents.OnDataBound = "SetColor";
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

            //GridSortExpression exp = new GridSortExpression();
            //exp.FieldName = "PoNum";
            //exp.SortOrder = GridSortOrder.Ascending;
            //grdworklist.MasterTableView.SortExpressions.Add(exp);

            AzzierHeader.InitToolBar(toolbar, "PoPanel");
        }
        else
        {
            setPanel = true;
        }
    }

    protected void grdresults_ItemDataBound(object sender, GridItemEventArgs e)
    {
    }

    protected void grdworklist_ItemDataBound(object sender, GridItemEventArgs e)
    {
    }

    protected void grdrecentlist_ItemDataBound(object sender, GridItemEventArgs e)
    {
    }

    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
        //AzzierScreen screen = new AzzierScreen();
        //if (e.Argument == "LoadMoreResults")
        //{
        //    screen.SaveSelectedList(grdresults, "location", out selectedresults);
        //    grdresults.PageSize = 100 + grdresults.PageSize;
        //    grdresults.Rebind();
        //    screen.SetSelecctedList(grdresults, "location", selectedresults);
        //}
        //else if (e.Argument == "LoadMoreWorkList")
        //{
        //    screen.SaveSelectedList(grdworklist, "location", out selectedworklist);
        //    grdworklist.PageSize = 100 + grdworklist.PageSize;
        //    grdworklist.Rebind();
        //    screen.SetSelecctedList(grdworklist, "location", selectedworklist);
        //}
        //else if (e.Argument == "LoadMoreRecentList")
        //{
        //    screen.SaveSelectedList(grdrecentlist, "location", out selectedrecentlist);
        //    grdrecentlist.PageSize = 100 + grdrecentlist.PageSize;
        //    grdrecentlist.Rebind();
        //    screen.SetSelecctedList(grdrecentlist, "location", selectedrecentlist);
        //}
    }

    protected void grdworklist_ItemEvent(object sender, GridItemEventArgs e)
    {
        if (e.EventInfo is GridInitializePagerItem)
        {
            worklistcount = (e.EventInfo as GridInitializePagerItem).PagingManager.DataSourceCount;
        }
    }

    protected void grdresults_ItemEvent(object sender, GridItemEventArgs e)
    {
        if (e.EventInfo is GridInitializePagerItem)
        {
            resultcount = (e.EventInfo as GridInitializePagerItem).PagingManager.DataSourceCount;
        }
    }

    protected void grdrecentlist_ItemEvent(object sender, GridItemEventArgs e)
    {
        if (e.EventInfo is GridInitializePagerItem)
        {
            recentlistcount = (e.EventInfo as GridInitializePagerItem).PagingManager.DataSourceCount;
        }
    }

    private void RetrieveMessage()
    {
        SystemMessage msg = new SystemMessage("location/locationpanel.aspx");
        m_msg = msg.GetSystemMessage();
        msg.SetJsMessage(litMessage);
    }
}