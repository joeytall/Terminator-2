using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Data.OleDb;
using System.Collections.Specialized;
using System.Data;

public partial class Timecards_tcpanel : System.Web.UI.Page
{
    protected AzzierScreen screen1, screen2;
    private string querymode = "";
    protected RadGrid grdresults;
    protected RadGrid grdrecentlist;
    protected NameValueCollection m_msg = new NameValueCollection();
    protected string mode = "";
    protected string wherestring = "";

    protected void Page_Init(object sender, EventArgs e)
    {
        RetrieveMessage();
        UserRights.CheckAccess("timecards");

        if (Request.QueryString["showlist"] != null)
        {
            mode = Request.QueryString["showlist"].ToString();
        }
        else
        {
            mode = "recentlist";
        }


        if (Request.QueryString["wherestring"] != null)
        {
            wherestring = Request.QueryString["wherestring"].ToString();
        }
        else 
        {
            wherestring = "1=2";
        }

        hidGetFirstRecord.Value = UserSettings.GetFirstRecordSetting("TimeCard");

        //Loading Screen Controls
        screen1 = new AzzierScreen("timecards/tcpanel.aspx", "mainform", MainControlsPanel.Controls);
        Session.LCID = Convert.ToInt32(Session["LCID"]);
        screen1.LCID = Session.LCID;

        screen2 = new AzzierScreen("timecards/tcpanel.aspx", "mainform", MainControlsPanel2.Controls);
        Session.LCID = Convert.ToInt32(Session["LCID"]);
        screen2.LCID = Session.LCID;

        //Init Query Result grid
        grdresults = new RadGrid();
        grdresults.ID = "grdresults";
        grdresults.Attributes.Add("rules", "all");
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
        grdresults.ClientSettings.Scrolling.SaveScrollPosition = true;
        grdresults.MasterTableView.ClientDataKeyNames = new string[] { "Counter" };
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
        grdresults.Attributes.Add("serviceURL", "../InternalServices/ServiceLabour.svc/QueryResultTimeCard");

        grdresults.ClientSettings.DataBinding.FilterParameterType = GridClientDataBindingParameterType.Linq;
        grdresults.ClientSettings.DataBinding.SortParameterType = GridClientDataBindingParameterType.Linq;
        grdresults.DataSource = new DataTable();
        
        screen1.SetGridColumns("results", grdresults);
        MainControlsPanel.Controls.Add(grdresults);


        //Init Query Result grid
        grdrecentlist = new RadGrid();
        grdrecentlist.ID = "grdrecentlist";

        grdrecentlist.Attributes.Add("rules", "all");
        grdrecentlist.AutoGenerateColumns = false;
        grdrecentlist.AllowPaging = true;
        grdrecentlist.PagerStyle.Position = GridPagerPosition.Bottom;
        grdrecentlist.PageSize = 100;
        grdrecentlist.AllowSorting = true;
        grdrecentlist.MasterTableView.AllowMultiColumnSorting = true;
        grdrecentlist.AllowFilteringByColumn = true;
        grdrecentlist.MasterTableView.DataKeyNames = new string[] { "Counter" };
        grdrecentlist.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
        grdrecentlist.ClientSettings.EnableRowHoverStyle = true;
        grdrecentlist.ClientSettings.Selecting.AllowRowSelect = true;
        grdrecentlist.ClientSettings.Scrolling.SaveScrollPosition = true;
        grdrecentlist.MasterTableView.ClientDataKeyNames = new string[] { "Counter" };
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
        grdrecentlist.ClientSettings.ClientEvents.OnDataBound = "SetColor";

        grdrecentlist.ClientSettings.DataBinding.FilterParameterType = GridClientDataBindingParameterType.Linq;
        grdrecentlist.ClientSettings.DataBinding.SortParameterType = GridClientDataBindingParameterType.Linq;

        screen2.SetGridColumns("recentlist", grdrecentlist);
        MainControlsPanel2.Controls.Add(grdrecentlist);

        screen1.LoadScreenWithoutDirtyLog();
        screen2.LoadScreenWithoutDirtyLog();

        grdresults.Style["Width"] = "98%";
        grdresults.Style["Height"] = "98%";
        grdrecentlist.Style["Width"] = "98%";
        grdrecentlist.Style["Height"] = "98%";

    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            AzzierHeader.InitToolBar(toolbar, "TimeCardsPanel");
        }
        grdrecentlist.ClientSettings.DataBinding.SelectMethod = "TimeCardsRecentListDataAndCount";
        grdrecentlist.ClientSettings.DataBinding.Location = "../InternalServices/ServiceLabour.svc";
        //grdresults.ClientSettings.DataBinding.SelectMethod = "LookupTimeCardDataAndCount?where=" + wherestring;
        //grdresults.ClientSettings.DataBinding.Location = "../InternalServices/ServiceLabour.svc";
        lblListType.Text = querymode;
        lblListType.Visible = true;
        if (mode == "queryresult")
        {

            MainControlsPanel.Style.Add("visibility", "visible");
            MainControlsPanel2.Style.Add("visibility", "hidden");
            MainControlsPanel.Style.Add("display", "block");
            MainControlsPanel2.Style.Add("display", "none");
            lblListType.Text = m_msg["T3"];
            grdresults.ClientSettings.ClientEvents.OnDataBound = "SetColor";
               
        }
        else if (mode == "recentlist")
        {
            grdresults.DataSource = new DataTable();
            MainControlsPanel2.Style.Add("visibility", "visible");
            MainControlsPanel.Style.Add("visibility", "hidden");
            MainControlsPanel2.Style.Add("display", "block");
            MainControlsPanel.Style.Add("display", "none");
            lblListType.Text = m_msg["T2"];
        }
        hidMode.Value = mode;
    }

    private bool ShowToolbarButtonRights(string commandname)
    {
        NameValueCollection drRights;
        UserRights right = new UserRights(Session["Login"].ToString(), "UserRights", "Counter");
        drRights = right.GetRights(Session["Login"].ToString(), "TimeCards");

        bool result= false;
        switch (commandname)
        {
            case "queryresult":
                result = true;
                break;
            case "recentlist":
                result = true;
                break;
            case "postcard":
                if ((drRights["urPOST"].ToString() != "") && (drRights["urPOST"].ToString() != "0"))
                {
                    result = true;
                }
                break;
            default :
                result = false;
                break;

        }
        return result;
    }

    private void RetrieveMessage()
    {
        SystemMessage msg = new SystemMessage("labour/labourpanel.aspx");
        m_msg = msg.GetSystemMessage();
        msg.SetJsMessage(litMessage);
    } 

}

