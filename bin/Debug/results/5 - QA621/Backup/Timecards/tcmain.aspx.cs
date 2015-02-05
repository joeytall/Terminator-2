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

public partial class Timecards_tcmain : System.Web.UI.Page
{
    protected AzzierScreen screen;
    protected string querymode = "";
    protected RadGrid grdtclist;
    protected NameValueCollection m_msg = new NameValueCollection();
    protected bool isquery = false;
    protected string counter;
    protected NameValueCollection nvctc;
    protected string m_empid = "";
    protected string datestring = "";


    protected void Page_Init(object sender, EventArgs e) 
    {
        RetrieveMessage();
        UserRights.CheckAccess("timecards");

        //Set panel query mode
        if (Request.QueryString["mode"] != null)
        {
            querymode = Request.QueryString["mode"].ToString();
            if (querymode == "")
            {
                querymode = "new";
            }

        }
        else
        {
            querymode = "new";
        }
        

        if (Page.IsPostBack)
        {
            if (Request.Form["__EVENTTARGET"] == "rowclick")
            {
                querymode = "edit";
                if (Request.Form["__EVENTARGUMENT"] != null)
                {
                    if (Request.Form["__EVENTARGUMENT"].ToString() != "")
                    {
                        counter = Request.Form["__EVENTARGUMENT"].ToString();
                    }
                }

            }
            
        }
        else 
        {
            if (Request.QueryString["Counter"] != null)
            {
                if (Request.QueryString["Counter"].ToString() != "")
                {
                    counter = Request.QueryString["Counter"].ToString();
                    querymode = "edit";
                }
            }
            if (Request.QueryString["empid"] != null)
            {
                if (Request.QueryString["empid"].ToString() != "")
                {
                    m_empid = Request.QueryString["empid"].ToString();
                }
            }
        }

        initScreen();


        ucHeader1.Mode = querymode;
        ucHeader1.TabName = "Main";
        hidMode.Value = querymode;
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

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
          UserRights objur = new UserRights(Session["Login"].ToString(), "UserRights", "Counter");
          NameValueCollection nvcrights = objur.GetRights(Session["Login"].ToString(), "TIMECARDS");
            RadioButtonList rl1 = (RadioButtonList)MainControlsPanel.FindControl("rbltctype");
            RadioButtonList rl2 = (RadioButtonList)MainControlsPanel.FindControl("rblinactive");

            if (rl1 != null)
            {
                rl1.RepeatDirection = RepeatDirection.Horizontal;
                ListItem litm1 = new ListItem("Unposted", "1");
                ListItem litm2 = new ListItem("Posted", "0");
                rl1.Items.Add(litm1);
                rl1.Items.Add(litm2);
                rl1.Style.Add("font-size", "12px");
                rl1.SelectedIndex = 0;
            }
            if (rl2 != null)
            {
                rl2.RepeatDirection = RepeatDirection.Horizontal;
                ListItem litm1 = new ListItem("Unposted", "1");
                ListItem litm2 = new ListItem("Posted", "0");
                rl2.Items.Add(litm1);
                rl2.Items.Add(litm2);
                rl2.Style.Add("font-size", "12px");
                
                if (querymode == "new")
                {
                  rl2.SelectedIndex = 0;
                  rl2.Attributes.Add("onclick", "roradiobutton('rblinactive'," + rl2.SelectedIndex.ToString() + ")");
                }
                else if (querymode == "query")
                {
                  ListItem litm3 = new ListItem("All", "");
                  rl2.Items.Add(litm3);
                  rl2.SelectedIndex = 2;
                }
                else if (querymode == "edit")
                {
                  ModuleoObject obj = new ModuleoObject(Session["Login"].ToString(), "v_WOLabourDetail", "Counter", counter);
                  if (obj.ModuleData["Inactive"] == "1")
                  {
                    rl2.SelectedIndex = 0;
                    rl2.Attributes.Add("onclick", "roradiobutton('rblinactive'," + rl2.SelectedIndex.ToString() + ")");
                  }
                  else
                  {
                    rl2.SelectedIndex = 1;
                    rl2.Attributes.Add("onclick", "roradiobutton('rblinactive'," + rl2.SelectedIndex.ToString() + ")");
                  }

                }
                

            }
            if (nvcrights["urReadyToPost"] != "1")
            {
              btnsetreadytopost.Visible = false;
            }
            if (nvcrights["urPost"] != "1")
            {
              btnpostcard.Visible = false;
            }

        }
        SetObjectNVC();
        screen.PopulateScreen("v_WOLabourDetail", nvctc);
        txtempid_TextChanged(m_empid);
    }

    private void SetObjectNVC()
    {
        nvctc = new NameValueCollection();
        switch (querymode)
        {
            case "query":
                nvctc = new NameValueCollection();
                break;
            case "new":
                nvctc = new NameValueCollection();
                if (m_empid != "")
                {
                    Employee emp = new Employee(Session["Login"].ToString(), "Employee", "empid", m_empid);
                    nvctc.Add("empid", m_empid);
                    nvctc.Add("rate", Convert.ToDouble(emp.ModuleData["rate"]).ToString());
                    nvctc.Add("craft", emp.ModuleData["craft"]);
                    nvctc.Add("draccount", emp.ModuleData["draccount"]);
                }
                nvctc.Add("labortype", "REG");
                nvctc.Add("scale", "1");
                nvctc.Add("addcost", "0");
                nvctc.Add("totalcost", "0");
                nvctc.Add("chargebackamount", "0");
                if (datestring != "")
                {
                    DateFormat df = new DateFormat(Session.LCID);
                    nvctc.Add("transdate", df.FormatOutputDate(datestring));
                }
                else 
                {
                    DateFormat df = new DateFormat(Session.LCID);
                    nvctc.Add("transdate", df.FormatOutputDate(DateTime.Today.ToString()));
                }
                break;
            case "edit":
                Labour obj = new Labour(Session["Login"].ToString(), "v_WOLabourDetail", "Counter", counter);
                nvctc = obj.ModuleData;
                if (nvctc["empid"] != null)
                {
                    m_empid = nvctc["empid"];
                }
                if (nvctc["counter"] == "")
                {
                    querymode = "query";
                    isquery = true;
                    counter = "";
                }
                UserWorkList list = new UserWorkList();
                list.AddToRecentList(Session["Login"].ToString(), "TimeCards", counter);
                break;
        }
    }

   


    private void initScreen()
    {
        //Loading Screen Controls
        screen = new AzzierScreen("timecards/tcmain.aspx", "mainform", MainControlsPanel.Controls, querymode);
        Session.LCID = Convert.ToInt32(Session["LCID"]);
        screen.LCID = Session.LCID;

        //Initialize Grid
        grdtclist = new RadGrid();
        grdtclist.ID = "grdtclist";
        grdtclist.Attributes.Add("rules", "all");
        grdtclist.AutoGenerateColumns = true;
        grdtclist.AllowPaging = true;
        grdtclist.PagerStyle.Position = GridPagerPosition.Bottom;
        grdtclist.PageSize = 100;
        grdtclist.AllowSorting = true;
        grdtclist.MasterTableView.AllowMultiColumnSorting = true;
        grdtclist.AllowFilteringByColumn = true;
        grdtclist.MasterTableView.DataKeyNames = new string[] { "Counter", "ReadyToPost" };
        grdtclist.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
        grdtclist.ClientSettings.EnableRowHoverStyle = true;
        grdtclist.ClientSettings.Selecting.AllowRowSelect = true;
        grdtclist.ClientSettings.Scrolling.SaveScrollPosition = true;
        grdtclist.MasterTableView.ClientDataKeyNames = new string[] { "Counter", "ReadyToPost" };
        grdtclist.ClientSettings.Scrolling.AllowScroll = true;
        grdtclist.Skin = "Outlook";
        grdtclist.SortingSettings.EnableSkinSortStyles = false;
        grdtclist.ClientSettings.Selecting.AllowRowSelect = true;
        grdtclist.AllowMultiRowSelection = true;
        grdtclist.ShowFooter = true;
        
        

        grdtclist.ClientSettings.Selecting.UseClientSelectColumnOnly = true;
        grdtclist.PagerStyle.Visible = true;
        grdtclist.PagerStyle.AlwaysVisible = true;
        GridClientSelectColumn cbColumn1 = new GridClientSelectColumn();
        cbColumn1.HeaderStyle.Width = 25;
        grdtclist.Columns.Add(cbColumn1);

        grdtclist.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate("WOLabour");
        grdtclist.ClientSettings.DataBinding.FilterParameterType = GridClientDataBindingParameterType.Linq;
        grdtclist.ClientSettings.DataBinding.SortParameterType = GridClientDataBindingParameterType.Linq;
        
        grdtclist.ItemCreated +=new GridItemEventHandler(grdtclist_ItemCreated);
        grdtclist.MasterTableView.EnableViewState = false;
        grdtclist.MasterTableView.ShowHeadersWhenNoRecords = true;
        grdtclist.MasterTableView.ShowHeader = true;
        grdtclist.EnableLinqExpressions = true;

        grdtclist.ClientSettings.ClientEvents.OnCommand = "Grid_OnCommand";
        grdtclist.ClientSettings.ClientEvents.OnRowClick = "rowClicked";

        screen.SetGridColumns("tclist", grdtclist);
        MainControlsPanel.Controls.Add(grdtclist);

        screen.LoadScreen();

        screen.SetValidationControls();

        grdtclist.DataSource = new DataTable();

        //Add event handler for radio button list.
        RadioButtonList rl1 = (RadioButtonList)MainControlsPanel.FindControl("rbltctype");
        rl1.Attributes.Add("OnChange", "selectTCType();");   
    }

    protected void grdtclist_ItemCreated(object sender, GridItemEventArgs e)
    {
      screen.GridItemCreated(e, "timecards/tcmain.aspx", "MainForm", "tclist", grdtclist);
    }


    protected void txtempid_TextChanged(string empid)
    {
        ModuleoObject obj = new ModuleoObject(Session["Login"].ToString(), "Employee", "empid", empid);
        TextBox txtempname = (TextBox)MainControlsPanel.FindControl("txtempname");
        txtempname.Text = obj.ModuleData["firstname"] + " " + obj.ModuleData["lastname"];
    }

    private void RetrieveMessage()
    {
        SystemMessage msg = new SystemMessage("labour/labourpanel.aspx");
        m_msg = msg.GetSystemMessage();
        msg.SetJsMessage(litMessage);
    }

    private void showResultList(bool isshow)
    {
        isshow = true;
        RadioButtonList rl = (RadioButtonList)MainControlsPanel.FindControl("rbltctype");
        grdtclist.Visible = isshow;
        rl.Visible = isshow;
        btndelete.Visible = isshow;
        btnpostcard.Visible = isshow;
        setEditButtons();
    }

    private void prefillCells()
    {
        Employee emp;
        NameValueCollection nvc = new NameValueCollection();
        TextBox txtrate = (TextBox)MainControlsPanel.FindControl("txtrate");
        TextBox txtcraft = (TextBox)MainControlsPanel.FindControl("txtcraft");      
        TextBox txtscale = (TextBox)MainControlsPanel.FindControl("txtscale");
        TextBox txtaddcost = (TextBox)MainControlsPanel.FindControl("txtaddcost");
        TextBox txttotalcost = (TextBox)MainControlsPanel.FindControl("txttotalcost");
        TextBox txtlabortype = (TextBox)MainControlsPanel.FindControl("txtlabortype");
        TextBox txtchargebackamount = (TextBox)MainControlsPanel.FindControl("txtchargebackamount");
        TextBox txttransdate = (TextBox)MainControlsPanel.FindControl("txttransdate");
       
    
        if (m_empid != "")
        {
            emp = new Employee(Session["Login"].ToString(), "Employee", "empid", m_empid);
            nvc = emp.ModuleData;
            txtrate.Text = Convert.ToDouble(nvc["rate"]).ToString();
            txtcraft.Text = nvc["craft"];     
        }
        txtlabortype.Text = "REG";
        txtscale.Text = "1";
        txtaddcost.Text = "0";
        txttotalcost.Text = "0";
        txtchargebackamount.Text = "0";
        if (datestring != "")
        {
            DateFormat df = new DateFormat(Session.LCID);
            txttransdate.Text = df.FormatOutputDate(datestring);
            //txttransdate.Text = Convert.ToDateTime(datestring).ToString();
        }
        else 
        {
            DateFormat df = new DateFormat(Session.LCID);
            txttransdate.Text = df.FormatOutputDate(DateTime.Today.ToString());
        }
    }

    private void setEditButtons()
    {

        //Load user right information
        NameValueCollection drRights;
        UserRights right = new UserRights(Session["Login"].ToString(), "UserRights", "Counter");
        LinkButton btnpostcard = (LinkButton)MainControlsPanel.FindControl("btnpostcard");
        LinkButton btndelete = (LinkButton)MainControlsPanel.FindControl("btndelete");
        drRights = right.GetRights(Session["Login"].ToString(), "TimeCards");

        if ((drRights["urEdit"].ToString() == "0") ||  (drRights["urEdit"].ToString() == ""))
        {
            btndelete.Visible = false;
        }
        if ((drRights["urPost"].ToString() == "0") || (drRights["urPost"].ToString() == ""))
        {
            btnpostcard.Visible = false;
        }
    }
}