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

public partial class kpi_kpipanel : System.Web.UI.Page
{
    protected string mode;
    protected RadGrid grdresults;
    protected RadGrid grdworklist;
    protected RadGrid grdrecentlist;
    //private OleDbConnection conn;
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


    protected void Page_Init(object sender, EventArgs e)
    {
        RetrieveMessage();
        UserRights.CheckAccess("kpi");
        Session.LCID = Convert.ToInt32(Session["LCID"]);
        if (Request.QueryString["showlist"] != null)
            mode = Request.QueryString["showlist"].ToString();
        else
            mode = "queryresult";

        string wherestring = "";

        grdworklist = new RadGrid();
        grdresults = new RadGrid();
        grdrecentlist = new RadGrid();

        //Result Grid

        grdresults.ID = "grdresults";
        //grdresults.DataSourceID = "ResultsSqlDataSource";

        AzzierScreen screen = new AzzierScreen("kpi/kpipanel.aspx", "MainForm", MainControlsPanel.Controls, "query");

        grdresults.Attributes.Add("rules", "all");
        //grdresults.DataSourceID = "ResultsSqlDataSource";
        grdresults.AutoGenerateColumns = false;
        grdresults.AllowPaging = true;
        grdresults.PagerStyle.Position = GridPagerPosition.Bottom;

        grdresults.PageSize = 100;
        grdresults.AllowSorting = true;
        grdresults.MasterTableView.AllowMultiColumnSorting = true;
        grdresults.AllowFilteringByColumn = true;
        grdresults.MasterTableView.DataKeyNames = new string[] { "Empid" };
        grdresults.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
        grdresults.ClientSettings.EnableRowHoverStyle = true;
        grdresults.ClientSettings.Selecting.AllowRowSelect = true;
        grdresults.AllowMultiRowSelection = true;
        grdresults.ClientSettings.Scrolling.SaveScrollPosition = true;
        //grdresults.ClientSettings.ClientEvents.OnScroll = "HandleScrollingResults";
        grdresults.ClientSettings.Selecting.UseClientSelectColumnOnly = true;
        grdresults.ClientSettings.ClientEvents.OnGridCreated = "updatecounts";
        grdresults.PreRender += new EventHandler(grdresults_PreRender);
        grdresults.ItemEvent += new GridItemEventHandler(grdresults_ItemEvent);
        grdresults.PagerStyle.Visible = false;
        grdresults.ClientSettings.ClientEvents.OnRowClick = "rowClicked";
        grdresults.ClientSettings.Scrolling.AllowScroll = true;
        grdresults.SortingSettings.EnableSkinSortStyles = false;
        grdresults.MasterTableView.ClientDataKeyNames = new string[] { "Empid" };

        //grdresults.ClientSettings.ClientEvents.OnGridCreated = "GridCreated";
        GridClientSelectColumn cbColumn = new GridClientSelectColumn();
        grdresults.Columns.Add(cbColumn);
        cbColumn.HeaderStyle.Width = 25;
        screen.SetGridColumns("results", grdresults);
        MainControlsPanel.Controls.Add(grdresults);
        screen.LoadScreen();

        grdresults.Skin = "Outlook";

        if (specsearch == "true")
        {
            grdresults.ClientSettings.DataBinding.SelectMethod = "SpecSearchDatandCount?wherestring=" + wherestring;
            grdresults.ClientSettings.DataBinding.Location = "../InternalServices/ServiceLabour.svc";
            grdresults.ClientSettings.DataBinding.SortParameterType = GridClientDataBindingParameterType.Linq;
            grdresults.ClientSettings.DataBinding.FilterParameterType = GridClientDataBindingParameterType.Linq;
        }
        else
        {
            grdresults.ClientSettings.DataBinding.SelectMethod = "SearchDataAndCount?wherestring=" + wherestring;
            grdresults.ClientSettings.DataBinding.Location = "../InternalServices/ServiceLabour.svc";
            grdresults.ClientSettings.DataBinding.SortParameterType = GridClientDataBindingParameterType.Linq;
            grdresults.ClientSettings.DataBinding.FilterParameterType = GridClientDataBindingParameterType.Linq;
        }




        // WorkList Grid

        grdworklist.ID = "grdworklist";
        //grdworklist.DataSourceID = "WorkListSqlDataSource";

        //WorkListSqlDataSource.ConnectionString = connstring;
        //WorkListSqlDataSource.SelectCommand = "SELECT Empid,FirstName,LastName FROM Employee WHERE Empid In (Select LinkId From UserWorkList Where UserId='" + Session["Login"] + "' And LinkModule='labour' And LinkType='worklist') And Inactive=0";
        grdworklist.Attributes.Add("rules", "all");
        //grdworklist.GridLines = GridLines.Both;
        //grdworklist.MasterTableView.GridLines = GridLines.Both;
        //grdworklist.DataSourceID = "WorkListSqlDataSource";
        grdworklist.AutoGenerateColumns = false;
        grdworklist.AllowPaging = true;
        grdworklist.PagerStyle.Position = GridPagerPosition.Bottom;
        grdworklist.PageSize = 100;
        grdworklist.AllowSorting = true;
        grdworklist.MasterTableView.AllowMultiColumnSorting = true;
        grdworklist.AllowFilteringByColumn = true;
        grdworklist.MasterTableView.DataKeyNames = new string[] { "Empid" };
        grdworklist.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
        grdworklist.ClientSettings.EnableRowHoverStyle = true;
        grdworklist.ClientSettings.Selecting.AllowRowSelect = true;
        grdworklist.ClientSettings.Scrolling.SaveScrollPosition = true;
        //grdworklist.ClientSettings.ClientEvents.OnScroll = "HandleScrollingWorkList";
        grdworklist.PreRender += new EventHandler(grdworklist_PreRender);
        grdworklist.ItemEvent += new GridItemEventHandler(grdworklist_ItemEvent);
        //grdworklist.ItemDataBound +=new GridItemEventHandler(grdworklist_ItemDataBound);
        grdworklist.ClientSettings.ClientEvents.OnRowClick = "rowClicked";
        grdworklist.MasterTableView.ClientDataKeyNames = new string[] { "Empid" };
        grdworklist.ClientSettings.Scrolling.AllowScroll = true;
        //grdworklist.ItemCommand += new GridCommandEventHandler(grdworklist_ItemCommand);
        grdworklist.Skin = "Outlook";
        grdworklist.ClientSettings.ClientEvents.OnGridCreated = "updatecounts";
        grdworklist.SortingSettings.EnableSkinSortStyles = false;
        AzzierScreen screen2 = new AzzierScreen("labour/labourpanel.aspx", "MainForm", MainControlsPanel2.Controls);
        grdworklist.ClientSettings.Selecting.AllowRowSelect = true;
        grdworklist.AllowMultiRowSelection = true;

        grdworklist.ClientSettings.Selecting.UseClientSelectColumnOnly = true;
        grdworklist.PagerStyle.Visible = true;// false;
        GridClientSelectColumn cbColumn2 = new GridClientSelectColumn();
        cbColumn2.HeaderStyle.Width = 25;
        grdworklist.Columns.Add(cbColumn2);

        screen2.SetGridColumns("worklist", grdworklist);
        MainControlsPanel2.Controls.Add(grdworklist);
        screen2.LoadScreenWithoutDirtyLog();


        grdworklist.ClientSettings.DataBinding.SelectMethod = "WorkListDataAndCount?wherestring=" + wherestring;
        grdworklist.ClientSettings.DataBinding.Location = "../InternalServices/ServiceLabour.svc";
        grdworklist.ClientSettings.DataBinding.SortParameterType = GridClientDataBindingParameterType.Linq;
        grdworklist.ClientSettings.DataBinding.FilterParameterType = GridClientDataBindingParameterType.Linq;



        // Recent list

        grdrecentlist.ID = "grdrecentlist";
        //grdrecentlist.DataSourceID = "RecentListSqlDataSource";

        //RecentListSqlDataSource.ConnectionString = connstring;
        //RecentListSqlDataSource.SelectCommand = "SELECT Empid,FirstName,LastName FROM Employee WHERE Empid In (Select LinkId From UserWorkList Where UserId='" + Session["Login"] + "' And LinkModule='labour' And LinkType='recent') And Inactive=0";
        grdrecentlist.Attributes.Add("rules", "all");
        //grdrecentlist.GridLines = GridLines.Both;
        //grdrecentlist.MasterTableView.GridLines = GridLines.Both;
        //grdrecentlist.DataSourceID = "RecentListSqlDataSource";
        grdrecentlist.AutoGenerateColumns = false;
        grdrecentlist.AllowPaging = true;
        grdrecentlist.PagerStyle.Position = GridPagerPosition.Bottom;
        grdrecentlist.PageSize = 100;
        grdrecentlist.AllowSorting = true;
        grdrecentlist.MasterTableView.AllowMultiColumnSorting = true;
        grdrecentlist.AllowFilteringByColumn = true;
        grdrecentlist.MasterTableView.DataKeyNames = new string[] { "Empid" };
        grdrecentlist.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
        grdrecentlist.ClientSettings.EnableRowHoverStyle = true;
        grdrecentlist.ClientSettings.Selecting.AllowRowSelect = true;
        grdrecentlist.ClientSettings.Scrolling.SaveScrollPosition = true;
        //grdrecentlist.ClientSettings.ClientEvents.OnScroll = "HandleScrollingRecentList";
        grdrecentlist.PreRender += new EventHandler(grdrecentlist_PreRender);
        grdrecentlist.ItemEvent += new GridItemEventHandler(grdrecentlist_ItemEvent);
        AzzierScreen screen3 = new AzzierScreen("labour/labourpanel.aspx", "MainForm", MainControlsPanel3.Controls);
        grdrecentlist.ClientSettings.Selecting.AllowRowSelect = true;
        grdrecentlist.AllowMultiRowSelection = true;
        grdrecentlist.PagerStyle.Visible = true;// false;
        grdrecentlist.ClientSettings.ClientEvents.OnRowClick = "rowClicked";
        grdrecentlist.ClientSettings.Scrolling.AllowScroll = true;
        grdrecentlist.SortingSettings.EnableSkinSortStyles = false;
        grdrecentlist.MasterTableView.ClientDataKeyNames = new string[] { "Empid" };


        //grdresults.ClientSettings.ClientEvents.OnRowDblClick = "getGridSelectedItems";
        //grdresults.ClientSettings.ClientEvents.OnRowSelected  = "getGridSelectedItems";
        grdrecentlist.ClientSettings.Selecting.UseClientSelectColumnOnly = true;
        GridClientSelectColumn cbColumnRecentlist = new GridClientSelectColumn();
        cbColumnRecentlist.HeaderStyle.Width = 25;
        grdrecentlist.Columns.Add(cbColumnRecentlist);
        screen3.SetGridColumns("recentlist", grdrecentlist);
        MainControlsPanel3.Controls.Add(grdrecentlist);

        screen3.LoadScreenWithoutDirtyLog();

        grdrecentlist.Skin = "Outlook";


        grdrecentlist.ClientSettings.DataBinding.SelectMethod = "RecentListDataAndCount?wherestring=" + wherestring;
        grdrecentlist.ClientSettings.DataBinding.Location = "../InternalServices/ServiceLabour.svc";
        grdrecentlist.ClientSettings.DataBinding.SortParameterType = GridClientDataBindingParameterType.Linq;
        grdrecentlist.ClientSettings.DataBinding.FilterParameterType = GridClientDataBindingParameterType.Linq;



        grdresults.PreRender += new EventHandler(grdresults_PreRender);
        grdworklist.EnableLinqExpressions = false;
        grdrecentlist.EnableLinqExpressions = false;
        grdresults.EnableLinqExpressions = false;


        grdworklist.GroupingSettings.CaseSensitive = false;
        grdresults.GroupingSettings.CaseSensitive = false;
        grdrecentlist.GroupingSettings.CaseSensitive = false;

        grdresults.Style["Width"] = "98%";
        grdresults.Style["Height"] = "98%";
        grdworklist.Style["Width"] = "98%";
        grdworklist.Style["Height"] = "98%";
        grdrecentlist.Style["Width"] = "98%";
        grdrecentlist.Style["Height"] = "98%";
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
        //        GetEmpids(WorkListSqlDataSource.SelectCommand, "worklist");
        //if (setworklist == true)
        if (hidMode.Value == "worklist")
        {
            lblListType.Text = "Work List";
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

                if (Request.QueryString["specsearch"] != null)
                {
                    string specsearch = Request.QueryString["specsearch"];
                    if (specsearch == "true")
                    {
                        litScript.Text = "gotomain()";
                    }
                }
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
            exp.FieldName = "empid";
            exp.SortOrder = GridSortOrder.Ascending;
            grdworklist.MasterTableView.SortExpressions.Add(exp);

            InitToolBar();
            //AzzierHeader.InitToolBar(toolbar, "LabourPanel");

        }
        else
        {
            setPanel = true;
            //if (Request.Form["__EVENTTARGET"] == "addworklist")
            //{
            //    AddToWorkList(null, null);
            //}
            //if (Request.Form["__EVENTTARGET"] == "removeworklist")
            //{
            //    DeleteWorkList(null, null);
            //}
        }

        //trblbnAddList.Attributes.Add("onmouseover", "imageChangeover('tbrimgAddList','../IMAGES/addlist32_over.png')");
        //trblbnAddList.Attributes.Add("onmouseout", "imageChangeover('tbrimgAddList','../IMAGES/addlist32.png')");
    }

    private void InitToolBar()
    {
        int iconwidth = 55;
        int screenwidth = Convert.ToInt32(Session["ScreenWidth"].ToString());
        int panelwidth = (int)Math.Floor(screenwidth * 0.33) - 32;
        if (Session["LABFrameControlPanel"] != null)
        {
          panelwidth = (int)Math.Floor(screenwidth * (Convert.ToDecimal(Session["LABFrameControlPanel"].ToString()) / 100)) - 32;
        }
        int totaliconnum = (int)Math.Floor((decimal)panelwidth / iconwidth);
        RadToolBarDropDown toolbarcombobox = new RadToolBarDropDown();

        string connstring = Application["ConnString"].ToString();
        OleDbConnection conn = new OleDbConnection(connstring);
        conn.Open();
        string sql = "SELECT * FROM UserToolbar WHERE UserId='" + Session["Login"].ToString() + "' AND UserModule='LabourPanel' ORDER BY orderby";
        OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
        DataTable dt = new DataTable();
        da.Fill(dt);
        da.Dispose();

        if (dt.Rows.Count == 0)
        {
          sql = "SELECT * FROM UserToolbar WHERE UserId='DEFAULT' AND UserModule='LabourPanel' ORDER BY orderby";
          da = new OleDbDataAdapter(sql, conn);
          da.Fill(dt);
          da.Dispose();
        }

        NameValueCollection drRights;
        UserRights right = new UserRights(Session["Login"].ToString(), "UserRights", "Counter");
        drRights = right.GetRights(Session["Login"].ToString(), "Labour");

        int iconnum = 0;
        //toolbar = new RadToolBar();
        toolbar.Height = Unit.Pixel(60);
        toolbar.OnClientButtonClicking = "ToolbarClicked";
        toolbar.AutoPostBack = false;
        //toolbar.ButtonClick +=new RadToolBarEventHandler(toolbar_ButtonClick);
        toolbar.Width = Unit.Percentage(100);
        foreach (DataRow dr in dt.Rows)
        {
            string commandname = dr["CommandName"].ToString().ToLower();
            //if (!ShowToolbarButtonRights(commandname, drRights))
            //        continue;

            RadToolBarButton btn = new RadToolBarButton();
            btn.ImagePosition = ToolBarImagePosition.AboveText;
            btn.Text = dr["LabelText"].ToString();
            btn.ToolTip = dr["ToolTip"].ToString();
            btn.BackColor = System.Drawing.Color.Transparent;
            btn.CommandName = commandname;
            btn.Font.Size = 9;

            btn.ImageUrl = dr["ImageURL"].ToString();
            btn.HoveredImageUrl = dr["HoverUrl"].ToString();

            ////setup images
            //switch (commandname)
            //{
            //    case "addworklist":
            //        btn.ImageUrl = "../images/labour/Add_Labour_Work_List_32.png";
            //        btn.HoveredImageUrl = "../images/labour/Add_Labour_Work_List_32_over.png";
            //        break;
            //    case "queryresult":
            //        btn.ImageUrl = "../images/labour/query_labour_work_list_32.png";
            //        btn.HoveredImageUrl = "../images/labour/query_labour_work_list_32_over.png";
            //        break;
            //    case "worklist":
            //        btn.ImageUrl = "../images/labour/labour_Work_list_32.png";
            //        btn.HoveredImageUrl = "../images/labour/labour_Work_list_32_over.png";
            //        break;
            //    case "recentlist":
            //        btn.ImageUrl = "../images/labour/recent_labour_work_list_32.png";
            //        btn.HoveredImageUrl = "../images/labour/recent_labour_work_list_32_over.png";
            //        break;
            //    case "removeworklist":
            //        btn.ImageUrl = "../images/labour/Remove_Labour_Work_List_32.png";
            //        btn.HoveredImageUrl = "../images/labour/Remove_Labour_Work_List_32_over.png";
            //        break;
            //    case "searchspec":
            //        btn.ImageUrl = "../images/labour_query_32.png";
            //        btn.HoveredImageUrl = "../images/labour_query_32_over.png";
            //        break;
            //}

            if (iconnum >= totaliconnum - 1)
            {

                if (iconnum == totaliconnum - 1)
                {
                    toolbar.Items.Add(toolbarcombobox);
                }
                btn.ForeColor = System.Drawing.Color.Black;
                toolbarcombobox.Buttons.Add(btn);
            }
            else
            {
                btn.Width = Unit.Pixel(iconwidth);
                btn.ForeColor = System.Drawing.Color.FloralWhite;
                toolbar.Items.Add(btn);
            }
            iconnum++;
        }
    }



    //protected void AddToWorkList(object sender, EventArgs e)
    //{
    //    string list = "";
    //    RadGrid grid;
    //    string curmode = hidMode.Value;
    //    if (curmode == "queryresult")
    //    {
    //        grid = grdresults;
    //    }
    //    else if (curmode == "recentlist")
    //    {
    //        grid = grdrecentlist;
    //    }
    //    else
    //    {
    //        grid = null;
    //    }
    //    if (grid != null)
    //    {
    //        for (int i = 0; i < grid.SelectedIndexes.Count; i++)
    //        {
    //            string empid = grid.SelectedItems[i].OwnerTableView.DataKeyValues[grid.SelectedItems[i].ItemIndex]["empid"].ToString();
    //            //string wonum = grid.SelectedItems[i].OwnerTableView.DataKeyValues[grid.SelectedItems[i].ItemIndex][0].ToString();

    //            if (list == "")
    //                list = "'" + empid + "'";
    //            else
    //                list = list + ",'" + empid + "'";
    //        }
    //        if (list != "")
    //        {
    //            list = "(" + list + ")";
    //            UserWorkList obj = new UserWorkList(Session["Login"].ToString(), "UserWorkList", "");
    //            string transdate;
    //            DateFormat df = new DateFormat(Session.LCID);
    //            df.ValidateInputDate(DateTime.Today.ToShortDateString(), out transdate);

    //            //if (obj.BatchAddWorkList(Session["Login"].ToString(), "wo", DateTime.Today.ToShortDateString(), wolist, "wolist"))
    //            if (obj.BatchAddWorkList(Session["Login"].ToString(), "labour", transdate, list, "worklist"))
    //            {
    //                litScript.Text = "<script>alert('" + m_msg["T5"] + "');</script>";
    //                setworklist = false;
    //                grdworklist.Rebind();
    //            }
    //            else
    //            {
    //                litScript.Text = "<script>alert('" + m_msg["T6"] + "');</script>";
    //            }
    //        }
    //        else
    //        {
    //            litScript.Text = "<script>alert('" + m_msg["T7"] + "');</script>";
    //        }
    //    }

    //}

    //protected void DeleteWorkList(object sender, EventArgs e)
    //{
    //    string list = "";
    //    for (int i = 0; i < grdworklist.SelectedIndexes.Count; i++)
    //    {
    //        string empid = grdworklist.SelectedItems[i].OwnerTableView.DataKeyValues[grdworklist.SelectedItems[i].ItemIndex]["empid"].ToString();
    //        if (list == "")
    //            list = "'" + empid + "'";
    //        else
    //            list = list + ",'" + empid + "'";
    //    }
    //    if (list != "")
    //    {
    //        list = "(" + list + ")";
    //        UserWorkList obj = new UserWorkList(Session["Login"].ToString(), "UserWorkList", "");
    //        //if (obj.BatchAddWorkList(Session["Login"].ToString(), "wo", DateTime.Today.ToShortDateString(), wolist, "wolist"))
    //        if (obj.BatchDeleteWorkList(Session["Login"].ToString(), "labour", list, "worklist"))
    //        {
    //            litScript.Text = "<script>alert('" + m_msg["T8"] + "');</script>";
    //        }
    //        else
    //        {
    //            litScript.Text = "<script>alert('" + m_msg["T9"] + "');</script>";
    //        }
    //    }
    //    else
    //    {
    //        litScript.Text = "<script>alert('" + m_msg["T10"] + "');</script>";
    //    }
    //    grdworklist.Rebind();
    //    setworklist = true;
    //}

    private void GetEmpids(string sql, string listtype)
    {
        string empidlist = "";
        if (listtype == "queryresult")
        {
            string filter = grdresults.MasterTableView.FilterExpression;
            if (filter + "" != "")
            {
                sql = sql + " And " + filter;
            }
            string sortorder = grdresults.MasterTableView.SortExpressions.GetSortString();
            if (sortorder + "" != "")
                sql = sql + " Order By " + sortorder;
            else
                sql = sql + " Order By empid";

            OleDbDataAdapter da = new OleDbDataAdapter(sql, Application["ConnString"].ToString());
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (empidlist == "")
                        empidlist = "'" + dt.Rows[i]["Empid"].ToString() + "'";
                    else
                        empidlist = empidlist + ",'" + dt.Rows[i]["Empid"].ToString() + "'";
                }
            }
            hidQueryResultList.Value = empidlist;
        }

        else if (listtype == "worklist")
        {
            string filter = grdworklist.MasterTableView.FilterExpression;
            if (filter + "" != "")
            {
                sql = sql + " And " + filter;
            }
            string sortorder = grdworklist.MasterTableView.SortExpressions.GetSortString();
            if (sortorder + "" != "")
                sql = sql + " Order By " + sortorder;
            else
                sql = sql + " Order By empid";
            OleDbDataAdapter da = new OleDbDataAdapter(sql, Application["ConnString"].ToString());
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (empidlist == "")
                        empidlist = "'" + dt.Rows[i]["Empid"].ToString() + "'";
                    else
                        empidlist = empidlist + ",'" + dt.Rows[i]["Empid"].ToString() + "'";
                }
            }
            hidMyWorkList.Value = empidlist;
        }
        else if (listtype == "recentlist")
        {
            string filter = grdrecentlist.MasterTableView.FilterExpression;
            if (filter + "" != "")
            {
                sql = sql + " And " + filter;
            }
            string sortorder = grdrecentlist.MasterTableView.SortExpressions.GetSortString();
            if (sortorder + "" != "")
                sql = sql + " Order By " + sortorder;
            else
                sql = sql + " Order By empid";

            OleDbDataAdapter da = new OleDbDataAdapter(sql, Application["ConnString"].ToString());
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (empidlist == "")
                        empidlist = "'" + dt.Rows[i]["Empid"].ToString() + "'";
                    else
                        empidlist = empidlist + ",'" + dt.Rows[i]["Empid"].ToString() + "'";
                }
            }
            hidMyRecentList.Value = empidlist;
        }
    }

    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
        AzzierScreen screen = new AzzierScreen();
        if (e.Argument == "LoadMoreResults")
        {
            screen.SaveSelectedList(grdresults, "empid", out selectedresults);
            grdresults.PageSize = 100 + grdresults.PageSize;
            grdresults.Rebind();
            screen.SetSelecctedList(grdresults, "empid", selectedresults);
        }
        else if (e.Argument == "LoadMoreWorkList")
        {
            screen.SaveSelectedList(grdworklist, "empid", out selectedworklist);
            grdworklist.PageSize = 100 + grdworklist.PageSize;
            grdworklist.Rebind();
            screen.SetSelecctedList(grdworklist, "empid", selectedworklist);
        }
        else if (e.Argument == "LoadMoreRecentList")
        {
            screen.SaveSelectedList(grdrecentlist, "empid", out selectedrecentlist);
            grdrecentlist.PageSize = 100 + grdrecentlist.PageSize;
            grdrecentlist.Rebind();
            screen.SetSelecctedList(grdrecentlist, "empid", selectedrecentlist);
        }
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
        SystemMessage msg = new SystemMessage("labour/labourpanel.aspx");
        m_msg = msg.GetSystemMessage();
        msg.SetJsMessage(litMessage);
    }
}