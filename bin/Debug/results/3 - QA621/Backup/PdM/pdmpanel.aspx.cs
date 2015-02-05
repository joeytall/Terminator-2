﻿using System;
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

public partial class pdm_pdmpanel : System.Web.UI.Page
{
    protected string mode;
    protected RadGrid grdresults;
    protected RadGrid grdworklist;
    protected RadGrid grdrecentlist;
    private OleDbConnection conn;
    private bool setPanel;
    private bool setworklist;
    protected int resultcount = 0;
    protected int worklistcount = 0;
    protected int recentlistcount = 0;
    protected string selectedresults;
    protected string selectedworklist;
    protected string selectedrecentlist;
    protected int resulttotal = 0;
    protected string specsearch = "";
    protected NameValueCollection m_msg = new NameValueCollection();
    protected AzzierScreen screen, screen2, screen3;


    protected void Page_Init(object sender, EventArgs e)
    {
        RetrieveMessage();
        UserRights.CheckAccess("pdm");
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
            { wherestring = " 1=1 "; }
            //    ////Response.Write("<script>alert('This is Alert');</script>");
            //    ////Response.Write(wherestring);
            //    ////Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "alertMessage", "alert('Hello')", true);
            //    ////ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert(' " + wherestring + " Hello')", true);
            
        }
        else
            wherestring = " 1=2 ";

        hidGetFirstRecord.Value = UserSettings.GetFirstRecordSetting("PDM");

        grdworklist = new RadGrid();
        grdresults = new RadGrid();
        grdrecentlist = new RadGrid();

        screen = new AzzierScreen("pdm/pdmpanel.aspx", "MainForm", MainControlsPanel.Controls, "edit");

        string connstring = Application["ConnString"].ToString();
        conn = new OleDbConnection(connstring);
        conn.Open();

        grdresults.ID = "grdresults";

        grdresults.GroupingSettings.CaseSensitive = false;

        grdresults.Attributes.Add("rules", "all");
        grdresults.AutoGenerateColumns = false;
        grdresults.AllowPaging = true;
        grdresults.PagerStyle.Position = GridPagerPosition.Bottom;
        grdresults.PagerStyle.Visible = true;
        grdresults.PageSize = 100;
        
        grdresults.AllowSorting = true;
        grdresults.AllowFilteringByColumn = true;
        grdresults.AllowMultiRowSelection = true;
        
        grdresults.MasterTableView.AllowMultiColumnSorting = true;
        grdresults.MasterTableView.DataKeyNames = new string[] { "PDMNum" };
        grdresults.MasterTableView.ClientDataKeyNames = new string[] { "PDMNum" };
        grdresults.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
        grdresults.SortingSettings.EnableSkinSortStyles = false;

        //ClientSetting
        grdresults.ClientSettings.EnableRowHoverStyle = true;
        grdresults.ClientSettings.Selecting.AllowRowSelect = true;
        grdresults.ClientSettings.Scrolling.SaveScrollPosition = true;
        grdresults.ClientSettings.Selecting.UseClientSelectColumnOnly = true;
        grdresults.ClientSettings.Scrolling.AllowScroll = true;
        //grdresults.ClientSettings.ClientEvents.OnGridCreated = "updatecounts";
        grdresults.ClientSettings.ClientEvents.OnRowClick = "rowClicked";
        //grdresults.ClientSettings.ClientEvents.OnScroll = "HandleScrollingResults";
        grdresults.ClientSettings.ClientEvents.OnDataBound = "SetColor";//.OnRowDataBound = "updatecounts";???
        grdresults.PreRender += new EventHandler(grdresults_PreRender);
        //grdresults.PreRender += new EventHandler(grdresults_PreRender);
        //grdresults.ItemEvent += new GridItemEventHandler(grdresults_ItemEvent);

        grdresults.ClientSettings.ClientEvents.OnCommand = "ClientGrid_OnCommand";
        grdresults.ClientSettings.Selecting.EnableDragToSelectRows = true;
        grdresults.Attributes.Add("serviceURL", "../InternalServices/ServicePDM.svc/QueryResultPDM");

        //grdresults.ClientSettings.DataBinding.SelectMethod = "LookupDataAndCount?wherestring=" + wherestring;
        //grdresults.ClientSettings.DataBinding.SelectMethod = "SearchDataAndCount?wherestring=" + wherestring;
        //grdresults.ClientSettings.DataBinding.Location = "../InternalServices/ServicePDM.svc";
        grdresults.ClientSettings.DataBinding.SortParameterType = GridClientDataBindingParameterType.Linq;
        grdresults.ClientSettings.DataBinding.FilterParameterType = GridClientDataBindingParameterType.Linq;
        
        grdresults.Skin = "Outlook";
        grdresults.EnableLinqExpressions = false;
        grdresults.DataSource = new DataTable();

        //grdresults.ClientSettings.ClientEvents.OnGridCreated = "GridCreated";
        GridClientSelectColumn cbColumn = new GridClientSelectColumn();
        grdresults.Columns.Add(cbColumn);
        cbColumn.HeaderStyle.Width = 25;

        screen.SetGridColumns("results", grdresults);
        MainControlsPanel.Controls.Add(grdresults);
        screen.LoadScreen();


        // Work List

        grdworklist.ID = "grdworklist";

        grdworklist.GroupingSettings.CaseSensitive = false;

        grdworklist.Attributes.Add("rules", "all");
        //grdworklist.GridLines = GridLines.Both;
        //grdworklist.MasterTableView.GridLines = GridLines.Both;

        grdworklist.AutoGenerateColumns = false;
        grdworklist.AllowPaging = true;
        //grdworklist.PagerStyle.Position = GridPagerPosition.Bottom;
        grdworklist.PageSize = 100;
        grdworklist.AllowSorting = true;
        grdworklist.MasterTableView.AllowMultiColumnSorting = true;
        grdworklist.AllowFilteringByColumn = true;
        grdworklist.MasterTableView.DataKeyNames = new string[] { "PDMNum" };
        grdworklist.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
        grdworklist.ClientSettings.EnableRowHoverStyle = true;
        grdworklist.ClientSettings.Selecting.AllowRowSelect = true;
        grdworklist.ClientSettings.Scrolling.SaveScrollPosition = true;
        //grdworklist.ClientSettings.ClientEvents.OnScroll = "HandleScrollingWorkList";
        grdworklist.PreRender += new EventHandler(grdworklist_PreRender);
        //grdworklist.ItemEvent += new GridItemEventHandler(grdworklist_ItemEvent);
        //grdworklist.ItemDataBound +=new GridItemEventHandler(grdworklist_ItemDataBound);
        grdworklist.ClientSettings.ClientEvents.OnRowClick = "rowClicked";
        grdworklist.MasterTableView.ClientDataKeyNames = new string[] { "PDMNum" };
        grdworklist.ClientSettings.Scrolling.AllowScroll = true;
        //grdworklist.ItemCommand += new GridCommandEventHandler(grdworklist_ItemCommand);
        grdworklist.Skin = "Outlook";
        //grdworklist.ClientSettings.ClientEvents.OnGridCreated = "updatecounts";
        grdworklist.SortingSettings.EnableSkinSortStyles = false;

        screen2 = new AzzierScreen("pdm/pdmpanel.aspx", "MainForm", MainControlsPanel2.Controls);

        grdworklist.ClientSettings.Selecting.AllowRowSelect = true;
        grdworklist.AllowMultiRowSelection = true;

        grdworklist.ClientSettings.Selecting.UseClientSelectColumnOnly = true;
        grdworklist.PagerStyle.Visible = true;

        GridClientSelectColumn cbColumn2 = new GridClientSelectColumn();
        cbColumn2.HeaderStyle.Width = 25;
        grdworklist.Columns.Add(cbColumn2);

        grdworklist.EnableLinqExpressions = false;

        grdworklist.ClientSettings.DataBinding.SelectMethod = "GetWorkList";
        //grdworklist.ClientSettings.DataBinding.SelectMethod = "Geteqptwlist";
        grdworklist.ClientSettings.DataBinding.Location = "../InternalServices/ServicePDM.svc";
        grdworklist.ClientSettings.DataBinding.SortParameterType = GridClientDataBindingParameterType.Linq;
        grdworklist.ClientSettings.DataBinding.FilterParameterType = GridClientDataBindingParameterType.Linq;
        grdworklist.ClientSettings.ClientEvents.OnDataBound = "SetColor";

        screen2.SetGridColumns("worklist", grdworklist);
        MainControlsPanel2.Controls.Add(grdworklist);
        screen2.LoadScreenWithoutDirtyLog();


        //Recent List

        grdrecentlist.ID = "grdrecentlist";

        grdrecentlist.GroupingSettings.CaseSensitive = false;

        grdrecentlist.Attributes.Add("rules", "all");
        //grdrecentlist.GridLines = GridLines.Both;
        //grdrecentlist.MasterTableView.GridLines = GridLines.Both;
        grdrecentlist.AutoGenerateColumns = false;
        grdrecentlist.AllowPaging = true;
        grdrecentlist.PagerStyle.Position = GridPagerPosition.Bottom;
        grdrecentlist.PagerStyle.Visible = true;
        grdrecentlist.PageSize = 100;
        grdrecentlist.AllowSorting = true;
        grdrecentlist.AllowFilteringByColumn = true;

        grdrecentlist.MasterTableView.AllowMultiColumnSorting = true;
        grdrecentlist.MasterTableView.DataKeyNames = new string[] { "PDMNum" };
        grdrecentlist.MasterTableView.ClientDataKeyNames = new string[] { "PDMNum" };
        grdrecentlist.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;

        grdrecentlist.ClientSettings.EnableRowHoverStyle = true;
        grdrecentlist.ClientSettings.Selecting.AllowRowSelect = true;
        //grdrecentlist.ClientSettings.Selecting.AllowRowSelect = true;
        grdrecentlist.ClientSettings.Scrolling.SaveScrollPosition = true;
        //grdrecentlist.ClientSettings.ClientEvents.OnScroll = "HandleScrollingRecentList";
        grdrecentlist.ClientSettings.ClientEvents.OnRowClick = "rowClicked";
        grdrecentlist.ClientSettings.ClientEvents.OnDataBound = "SetColor";
        grdrecentlist.ClientSettings.Scrolling.AllowScroll = true;

        grdrecentlist.PreRender += new EventHandler(grdrecentlist_PreRender);
        //grdrecentlist.ItemEvent += new GridItemEventHandler(grdrecentlist_ItemEvent);

        screen3 = new AzzierScreen("pdm/pdmpanel.aspx", "MainForm", MainControlsPanel3.Controls);

        grdrecentlist.AllowMultiRowSelection = true;
        grdrecentlist.SortingSettings.EnableSkinSortStyles = false;

        grdrecentlist.EnableLinqExpressions = false;
        grdrecentlist.Skin = "Outlook";

//        grdrecentlist.ClientSettings.DataBinding.SelectMethod = "GetRecentList";
        grdrecentlist.ClientSettings.DataBinding.SelectMethod = "RecentListDataAndCount";
        grdrecentlist.ClientSettings.DataBinding.Location = "../InternalServices/ServicePDM.svc";
        grdrecentlist.ClientSettings.DataBinding.SortParameterType = GridClientDataBindingParameterType.Linq;
        grdrecentlist.ClientSettings.DataBinding.FilterParameterType = GridClientDataBindingParameterType.Linq;

        grdrecentlist.ClientSettings.Selecting.UseClientSelectColumnOnly = true;
        GridClientSelectColumn cbColumnRecentlist = new GridClientSelectColumn();
        cbColumnRecentlist.HeaderStyle.Width = 25;
        grdrecentlist.Columns.Add(cbColumnRecentlist);
        screen3.SetGridColumns("recentlist", grdrecentlist);
        MainControlsPanel3.Controls.Add(grdrecentlist);

        screen3.LoadScreenWithoutDirtyLog();

        grdresults.Style["Width"] = "98%";
        grdresults.Style["Height"] = "98%";
        grdworklist.Style["Width"] = "98%";
        grdworklist.Style["Height"] = "98%";
        grdrecentlist.Style["Width"] = "98%";
        grdrecentlist.Style["Height"] = "98%";

        grdrecentlist.ItemCreated += new GridItemEventHandler(grdrecentlist_ItemCreated);
        grdresults.ItemCreated += new GridItemEventHandler(grdresults_ItemCreated);
        grdworklist.ItemCreated += new GridItemEventHandler(grdworklist_ItemCreated);
    }

    protected void grdworklist_ItemCreated(object sender, GridItemEventArgs e)
    {
        screen2.GridItemCreated(e,"pmd/pmdpanel.aspx","MainForm","worklist",grdworklist);
    }

    protected void grdresults_ItemCreated(object sender, GridItemEventArgs e)
    {
        screen.GridItemCreated(e, "pmd/pmdpanel.aspx", "MainForm", "results", grdresults);
    }

    protected void grdrecentlist_ItemCreated(object sender, GridItemEventArgs e)
    {
        screen3.GridItemCreated(e, "pmd/pmdpanel.aspx", "MainForm", "recentlist", grdrecentlist);
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
            lblListType.Text =m_msg["T4"];// "Work List";
        }
    }

    protected void grdresults_PreRender(object sender, EventArgs e)
    {
        if (hidMode.Value == "queryresult")
        {
            lblListType.Text =m_msg["T3"];// "Query Result";
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
                //MainControlsPanel.Visible = false;
                //MainControlsPanel2.Visible = true;
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
            exp.FieldName = "PDMNum";
            exp.SortOrder = GridSortOrder.Ascending;
            grdworklist.MasterTableView.SortExpressions.Add(exp);

            AzzierHeader.InitToolBar(toolbar, "PDMPanel");
            //plhToolBar.Controls.Add(toolbar);

        }
        else
        {
            setPanel = true;
        }

    }

    private void RetrieveMessage()
    {
        SystemMessage msg = new SystemMessage("pdm/pdmpanel.aspx");
        m_msg = msg.GetSystemMessage();
        msg.SetJsMessage(litMessage);
    }
}