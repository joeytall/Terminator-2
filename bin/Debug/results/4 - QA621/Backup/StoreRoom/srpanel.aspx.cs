using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Data;
using System.Data.OleDb;
using System.Collections.Specialized;

public partial class StoreRoom_srpanel : System.Web.UI.Page
{
    private string m_division;
    private string mode;
    private string m_tabname;
    protected NameValueCollection m_msg = new NameValueCollection();
    protected AzzierScreen screen1, screen2, screen3;
    private string querymode = "";
    protected RadGrid grdresults;
    protected RadGrid grdrecentlist;
    protected RadGrid grdworklist;
    protected string wherestring = "";


    protected void Page_Init(object sender, EventArgs e)
    {
        RetrieveMessage();
        UserRights.CheckAccess("storeroom");
        if (Request.QueryString["showlist"] != null)
        {
            mode = Request.QueryString["showlist"].ToString();
        }
        else
        {
            mode = "worklist";
        }
        if (Request.QueryString["wherestring"] != null)
        {
            wherestring = Request.QueryString["wherestring"].ToString();
        }
        else 
        {
            wherestring = "1=2";
        }  
        InitGrids();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        grdrecentlist.ClientSettings.DataBinding.SelectMethod = "StoreRoomRecentListDataAndCount";
        grdrecentlist.ClientSettings.DataBinding.Location = "../InternalServices/ServiceInventory.svc";
        //grdresults.ClientSettings.DataBinding.SelectMethod = "LookupStoreRoomDataAndCount?where=" + wherestring;
        //grdresults.ClientSettings.DataBinding.Location = "../InternalServices/ServiceInventory.svc";
        grdworklist.ClientSettings.DataBinding.SelectMethod = "StoreRoomWorkListDataAndCount";
        grdworklist.ClientSettings.DataBinding.Location = "../InternalServices/ServiceInventory.svc";
        if (!Page.IsPostBack)
        {
            AzzierHeader.InitToolBar(toolbar, "StoreRoomPanel");
        }
        SwitchGridPanel();
        hidMode.Value = mode;
        hidGetFirstRecord.Value = UserSettings.GetFirstRecordSetting("Storeroom");
    }

    private void InitGrids()
    {
        //Loading Screen Controls
        screen1 = new AzzierScreen("storeroom/srpanel.aspx", "mainform", MainControlsPanel.Controls);
        Session.LCID = Convert.ToInt32(Session["LCID"]);
        screen1.LCID = Session.LCID;

        screen2 = new AzzierScreen("storeroom/srpanel.aspx", "mainform", MainControlsPanel2.Controls);
        Session.LCID = Convert.ToInt32(Session["LCID"]);
        screen2.LCID = Session.LCID;

        screen3 = new AzzierScreen("storeroom/srpanel.aspx", "mainform", MainControlsPanel3.Controls);
        Session.LCID = Convert.ToInt32(Session["LCID"]);
        screen3.LCID = Session.LCID;

        //Init Query Result grid
        grdresults = new RadGrid();
        grdresults.ID = "grdresults";
        grdresults.Attributes.Add("rules", "all");
        grdresults.AutoGenerateColumns = true;
        grdresults.AllowPaging = true;
        grdresults.PagerStyle.Position = GridPagerPosition.Bottom;
        grdresults.PageSize = 100;
        grdresults.AllowSorting = true;
        grdresults.MasterTableView.AllowMultiColumnSorting = true;
        grdresults.AllowFilteringByColumn = true;
        grdresults.MasterTableView.DataKeyNames = new string[] { "StoreRoom" };
        grdresults.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
        grdresults.ClientSettings.EnableRowHoverStyle = true;
        grdresults.ClientSettings.Selecting.AllowRowSelect = true;
        grdresults.ClientSettings.Scrolling.SaveScrollPosition = true;
        grdresults.MasterTableView.ClientDataKeyNames = new string[] { "StoreRoom" };
        grdresults.ClientSettings.Scrolling.AllowScroll = true;
        grdresults.Skin = "Outlook";
        grdresults.SortingSettings.EnableSkinSortStyles = false;
        grdresults.ClientSettings.Selecting.AllowRowSelect = true;
        grdresults.AllowMultiRowSelection = true;

        grdresults.ClientSettings.Selecting.UseClientSelectColumnOnly = true;
        grdresults.PagerStyle.Visible = true;
        grdresults.PagerStyle.AlwaysVisible = true;
        GridClientSelectColumn cbColumn1 = new GridClientSelectColumn();
        cbColumn1.HeaderStyle.Width = 25;
        grdresults.Columns.Add(cbColumn1);

        grdresults.ClientSettings.ClientEvents.OnRowClick = "rowClicked";
        grdresults.ClientSettings.ClientEvents.OnGridCreated = "updatecounts";

        grdresults.ClientSettings.ClientEvents.OnCommand = "ClientGrid_OnCommand";
        grdresults.ClientSettings.Selecting.EnableDragToSelectRows = true;
        grdresults.Attributes.Add("serviceURL", "../InternalServices/ServiceInventory.svc/QueryResultStoreRoom");

        grdresults.ClientSettings.DataBinding.FilterParameterType = GridClientDataBindingParameterType.Linq;
        grdresults.ClientSettings.DataBinding.SortParameterType = GridClientDataBindingParameterType.Linq;
        grdresults.DataSource = new DataTable();

        screen1.SetGridColumns("results", grdresults);
        MainControlsPanel.Controls.Add(grdresults);

        //Init Recent List grid
        grdrecentlist = new RadGrid();
        grdrecentlist.ID = "grdrecentlist";

        grdrecentlist.Attributes.Add("rules", "all");
        grdrecentlist.AutoGenerateColumns = true;
        grdrecentlist.AllowPaging = true;
        grdrecentlist.PagerStyle.Position = GridPagerPosition.Bottom;
        grdrecentlist.PageSize = 100;
        grdrecentlist.AllowSorting = true;
        grdrecentlist.MasterTableView.AllowMultiColumnSorting = true;
        grdrecentlist.AllowFilteringByColumn = true;
        grdrecentlist.MasterTableView.DataKeyNames = new string[] { "StoreRoom" };
        grdrecentlist.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
        grdrecentlist.ClientSettings.EnableRowHoverStyle = true;
        grdrecentlist.ClientSettings.Selecting.AllowRowSelect = true;
        grdrecentlist.ClientSettings.Scrolling.SaveScrollPosition = true;
        grdrecentlist.MasterTableView.ClientDataKeyNames = new string[] { "StoreRoom" };
        grdrecentlist.ClientSettings.Scrolling.AllowScroll = true;
        grdrecentlist.Skin = "Outlook";
        grdrecentlist.SortingSettings.EnableSkinSortStyles = false;
        grdrecentlist.ClientSettings.Selecting.AllowRowSelect = true;
        grdrecentlist.AllowMultiRowSelection = true;

        grdrecentlist.ClientSettings.Selecting.UseClientSelectColumnOnly = true;
        grdrecentlist.PagerStyle.Visible = true;
        grdrecentlist.PagerStyle.AlwaysVisible = true;
        GridClientSelectColumn cbColumn2 = new GridClientSelectColumn();
        cbColumn2.HeaderStyle.Width = 25;
        grdrecentlist.Columns.Add(cbColumn2);

        grdrecentlist.ClientSettings.ClientEvents.OnRowClick = "rowClicked";
        //grdrecentlist.ClientSettings.ClientEvents.OnGridCreated = "updatecounts";

        grdrecentlist.ClientSettings.DataBinding.FilterParameterType = GridClientDataBindingParameterType.Linq;
        grdrecentlist.ClientSettings.DataBinding.SortParameterType = GridClientDataBindingParameterType.Linq;
        grdrecentlist.ClientSettings.ClientEvents.OnDataBound = "SetColor";

        screen2.SetGridColumns("recentlist", grdrecentlist);
        MainControlsPanel2.Controls.Add(grdrecentlist);

        //Init Work List grid
        grdworklist = new RadGrid();
        grdworklist.ID = "grdworklist";

        grdworklist.Attributes.Add("rules", "all");
        grdworklist.AutoGenerateColumns = true;
        grdworklist.AllowPaging = true;
        grdworklist.PagerStyle.Position = GridPagerPosition.Bottom;
        grdworklist.PageSize = 100;
        grdworklist.AllowSorting = true;
        grdworklist.MasterTableView.AllowMultiColumnSorting = true;
        grdworklist.AllowFilteringByColumn = true;
        grdworklist.MasterTableView.DataKeyNames = new string[] { "StoreRoom" };
        grdworklist.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
        grdworklist.ClientSettings.EnableRowHoverStyle = true;
        grdworklist.ClientSettings.Selecting.AllowRowSelect = true;
        grdworklist.ClientSettings.Scrolling.SaveScrollPosition = true;
        grdworklist.MasterTableView.ClientDataKeyNames = new string[] { "StoreRoom" };
        grdworklist.ClientSettings.Scrolling.AllowScroll = true;
        grdworklist.Skin = "Outlook";
        grdworklist.SortingSettings.EnableSkinSortStyles = false;
        grdworklist.ClientSettings.Selecting.AllowRowSelect = true;
        grdworklist.AllowMultiRowSelection = true;

        grdworklist.ClientSettings.Selecting.UseClientSelectColumnOnly = true;
        grdworklist.PagerStyle.Visible = true;
        grdworklist.PagerStyle.AlwaysVisible = true;
        GridClientSelectColumn cbColumn3 = new GridClientSelectColumn();
        cbColumn3.HeaderStyle.Width = 25;
        grdworklist.Columns.Add(cbColumn3);

        grdworklist.ClientSettings.ClientEvents.OnRowClick = "rowClicked";
        grdworklist.ClientSettings.ClientEvents.OnDataBound = "SetColor";

        grdworklist.ClientSettings.DataBinding.FilterParameterType = GridClientDataBindingParameterType.Linq;
        grdworklist.ClientSettings.DataBinding.SortParameterType = GridClientDataBindingParameterType.Linq;

        screen3.SetGridColumns("worklist", grdworklist);
        MainControlsPanel3.Controls.Add(grdworklist);

        screen1.LoadScreen();
        screen2.LoadScreen();
        screen3.LoadScreen();

        grdresults.Style["Width"] = "98%";
        grdresults.Style["Height"] = "98%";
        grdrecentlist.Style["Width"] = "98%";
        grdrecentlist.Style["Height"] = "98%";
        grdworklist.Style["Width"] = "98%";
        grdworklist.Style["Height"] = "98%";
    }

    private void SwitchGridPanel()
    {
        switch (mode)
        {
            case "recentlist":
                MainControlsPanel2.Style.Add("visibility", "visible");
                MainControlsPanel.Style.Add("visibility", "hidden");
                MainControlsPanel2.Style.Add("display", "block");
                MainControlsPanel.Style.Add("display", "none");
                MainControlsPanel3.Style.Add("visibility", "hidden");
                MainControlsPanel3.Style.Add("display", "none");
                lblListType.Text = m_msg["T2"];
                break;
            case "queryresult":
                MainControlsPanel.Style.Add("visibility", "visible");
                MainControlsPanel2.Style.Add("visibility", "hidden");
                MainControlsPanel.Style.Add("display", "block");
                MainControlsPanel2.Style.Add("display", "none");
                MainControlsPanel3.Style.Add("visibility", "hidden");
                MainControlsPanel3.Style.Add("display", "none");
                lblListType.Text = m_msg["T3"];
                grdresults.ClientSettings.ClientEvents.OnDataBound = "SetColor";
                break;
            case "worklist":
                MainControlsPanel3.Style.Add("display", "block");
                MainControlsPanel3.Style.Add("visibility", "visible");
                MainControlsPanel2.Style.Add("visibility", "hidden");
                MainControlsPanel2.Style.Add("display", "none");
                MainControlsPanel.Style.Add("visibility", "hidden");
                MainControlsPanel.Style.Add("display", "none");
                lblListType.Text = m_msg["T4"];
                break;
            default:
                break;
        }
    }

    private Boolean ShowToolbarButtonRights(string commandname, NameValueCollection dr)
    {
        if (dr == null)
            return false;
        Boolean show = false;
        Division d = new Division();
        switch (commandname)
        {
            case "queryresult":
                show = true;
                break;
            case "recentlist":
                show = true;
                break;
            case "worklist":
                show = true;
                break;
            case "addworklist":
                show = true;
                break;
            case "removeworklist":
                show = true;
                break;
            default:
                show = false;
                break;
        }
        return show;
    }

    private void RetrieveMessage()
    {
        SystemMessage msg = new SystemMessage("labour/labourpanel.aspx");
        m_msg = msg.GetSystemMessage();
        msg.SetJsMessage(litMessage);
    } 
}