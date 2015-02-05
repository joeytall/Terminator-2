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

public partial class receiving_receivepanel : System.Web.UI.Page
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
    protected string lookupreturn = "";
    protected NameValueCollection m_msg = new NameValueCollection();
    protected AzzierScreen screen;


    protected void Page_Init(object sender, EventArgs e)
    {
        RetrieveMessage();
        if (Session["Login"] == null)
        {
            //Response.Write("<script>alert('Your session has expired. Please login again.');top.document.location.href='../Login.aspx';</script>");
            Response.Write("<script>alert('" + m_msg["T1"] + "');top.document.location.href='../Login.aspx';</script>");
            Response.End();
        }
        Session.LCID = Convert.ToInt32(Session["LCID"]);
        if (Request.QueryString["showlist"] != null)
            mode = Request.QueryString["showlist"].ToString();
        else
            mode = "queryresult";

        string wherestring = "";
        if (Request.QueryString["wherestring"] != null)
        {
            wherestring = Request.QueryString["wherestring"].ToString();
            if (wherestring == "")
                wherestring = " 1=1 ";
        }
        else
            wherestring = " 1=2 ";

        //hidGetFirstRecord.Value = UserSettings.GetFirstRecordSetting("RECV");

        if (Request.QueryString["LookupReturn"] != null)
          lookupreturn = Request.QueryString["LookupReturn"].ToString();

        //grdworklist = new RadGrid();
        grdresults = new RadGrid();
        //grdrecentlist = new RadGrid();



        //Result Grid

        grdresults.ID = "grdresults";
        //grdresults.DataSourceID = "ResultsSqlDataSource";

        screen = new AzzierScreen("receiving/receivepanel.aspx", "MainForm", MainControlsPanel.Controls, "edit");

        //ResultsSqlDataSource.ConnectionString = connstring;
        //ResultsSqlDataSource.SelectCommand = "SELECT * FROM Employee WHERE " + wherestring;
        grdresults.Attributes.Add("rules", "all");
        //grdresults.DataSourceID = "ResultsSqlDataSource";
        grdresults.AutoGenerateColumns = false;
        grdresults.AllowPaging = true;
        grdresults.PagerStyle.Position = GridPagerPosition.Bottom;

        grdresults.PageSize = 100;
        grdresults.AllowSorting = true;
        grdresults.MasterTableView.AllowMultiColumnSorting = true;
        grdresults.AllowFilteringByColumn = true;
        grdresults.MasterTableView.DataKeyNames = new string[] { "Counter" };
        grdresults.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
        grdresults.ClientSettings.EnableRowHoverStyle = true;
        grdresults.ClientSettings.Selecting.AllowRowSelect = true;
        grdresults.AllowMultiRowSelection = true;
        grdresults.ClientSettings.Scrolling.SaveScrollPosition = true;
        //grdresults.ClientSettings.ClientEvents.OnScroll = "HandleScrollingResults";
        grdresults.ClientSettings.Selecting.UseClientSelectColumnOnly = true;
        //grdresults.ClientSettings.ClientEvents.OnGridCreated = "updatecounts";
        grdresults.PreRender += new EventHandler(grdresults_PreRender);
        //grdresults.ItemEvent += new GridItemEventHandler(grdresults_ItemEvent);
        grdresults.PagerStyle.Visible = false;
        
        if (lookupreturn == "return")
        {
          grdresults.ClientSettings.ClientEvents.OnRowClick = "returnrowClicked";
        }
        else
        {
          grdresults.ClientSettings.ClientEvents.OnRowClick = "rowClicked";
          grdresults.ClientSettings.ClientEvents.OnRowSelecting = "rowSelecting";
        }
        grdresults.ClientSettings.ClientEvents.OnDataBound = "griddatabound";
        grdresults.ClientSettings.Scrolling.AllowScroll = true;
        grdresults.SortingSettings.EnableSkinSortStyles = false;
        grdresults.MasterTableView.ClientDataKeyNames = new string[] { "Counter","Serialized" };

        //grdresults.ClientSettings.ClientEvents.OnGridCreated = "GridCreated";
        GridClientSelectColumn cbColumn = new GridClientSelectColumn();
        grdresults.Columns.Add(cbColumn);
        cbColumn.UniqueName = "SelectColumn";
        cbColumn.HeaderStyle.Width = 25;
        screen.SetGridColumns("results", grdresults);
        MainControlsPanel.Controls.Add(grdresults);
        screen.LoadScreen();

        grdresults.Skin = "Outlook";
        //ModuleoObject o = new ModuleoObject(Session["Login"].ToString(), "", "");
        if (lookupreturn.ToLower() == "return")
        {
          if (wherestring == "")
            wherestring = " ReceiveQty>ReturnQty ";
          else
            wherestring = wherestring + " AND ReceiveQty>ReturnQty ";
        }
        else
        {
          if (wherestring == "")
            wherestring = " OrderQty>ReceiveQty ";
          else
            wherestring = wherestring + " AND OrderQty>ReceiveQty";
        }
        wherestring = wherestring + " And StatusCode>=200 And StatusCode<400";
        //grdresults.ClientSettings.DataBinding.SelectMethod = "ReceivingLineQuery?wherestring=" + wherestring;
        //grdresults.ClientSettings.DataBinding.Location = "../InternalServices/ServiceInventory.svc";
        grdresults.ClientSettings.ClientEvents.OnCommand = "ClientGrid_OnCommand";
        grdresults.ClientSettings.Selecting.EnableDragToSelectRows = true;
        grdresults.Attributes.Add("serviceURL", "../InternalServices/ServiceInventory.svc/QueryResultReceive");
        grdresults.ClientSettings.DataBinding.SortParameterType = GridClientDataBindingParameterType.Linq;
        grdresults.ClientSettings.DataBinding.FilterParameterType = GridClientDataBindingParameterType.Linq;

        grdresults.ItemCreated += new GridItemEventHandler(grdresults_ItemCreated);

        grdresults.PreRender += new EventHandler(grdresults_PreRender);

        grdresults.EnableLinqExpressions = false;


        grdresults.Style["Width"] = "98%";
        grdresults.Style["Height"] = "98%";

        grdresults.ClientSettings.DataBinding.ShowEmptyRowsOnLoad = false;
        grdresults.DataSource = new DataTable();

    }

    protected void grdresults_ItemCreated(object sender, GridItemEventArgs e)
    {
      screen.GridItemCreated(e, "receiving/receivepanel.aspx", "MainForm", "results",grdresults);
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
            if (lookupreturn.ToLower() != "return")
                AzzierHeader.InitToolBar(toolbar, "ReceivingPanel");

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

  /*
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
   * */

    private void RetrieveMessage()
    {
        //SystemMessage msg = new SystemMessage("receiving/receivepanel.aspx");
        SystemMessage msg = new SystemMessage("inventory/invpanel.aspx");
        m_msg = msg.GetSystemMessage();
        msg.SetJsMessage(litMessage);
    }
}