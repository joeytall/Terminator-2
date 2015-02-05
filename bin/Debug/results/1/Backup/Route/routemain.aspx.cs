using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;
using Telerik.Web.UI;
using System.Configuration;
using System.Linq;

public partial class route_routemain : System.Web.UI.Page
{
    AzzierScreen screen;
    private string connstring;
    protected string querymode = "";
    protected string m_routename, m_oldroutename = "";
    Route objRoute;
    NameValueCollection nvcroute;
    protected bool isquery = false;
    protected NameValueCollection m_msg = new NameValueCollection();
    protected NameValueCollection m_rights;
    protected int m_allowedit = 0;
    protected RadGrid grddetail;
    protected RadGrid grdavailable;
    

    protected void Page_Init(object sender, EventArgs e)
    {
        RetrieveMessage();
        UserRights.CheckAccess("route");
        UserRights r = new UserRights(Session["Login"].ToString(), "UserRights", "counter");
        m_rights = r.GetRights(Session["Login"].ToString(), "Route");

        m_allowedit = Convert.ToInt16(m_rights["urEdit"].ToString());

        Session.LCID = Convert.ToInt32(Session["LCID"]);
        if (Request.QueryString["mode"] != null)
        {
            querymode = Request.QueryString["mode"].ToString();
            if (querymode == "")
                querymode = "query";
        }
        else
            querymode = "query";

        if (Request.QueryString["RouteName"] != null)
        {
          m_routename = Request.QueryString["RouteName"].ToString();
          m_oldroutename = Request.QueryString["RouteName"].ToString();
          HidOldRouteName.Value = Request.QueryString["RouteName"].ToString();
          
        }
        else
            m_routename = "";

        if (querymode == "query")
            isquery = true;

        if (querymode == "edit")
        {
            objRoute = new Route(Session["Login"].ToString(), "Route", "RouteName", m_routename);

            nvcroute = objRoute.ModuleData;
            if (nvcroute["routename"].ToString() == "")
            {
                querymode = "query";
                isquery = true;
                hidMode.Value = querymode;
                m_routename = "";
            }
            UserWorkList list = new UserWorkList();
            list.AddToRecentList(Session["Login"].ToString(), "Route", m_routename);
        }
        else if (querymode == "duplicate" && m_oldroutename.Length > 0)
        {
            objRoute = new Route(Session["Login"].ToString(), "Route", "RouteName", m_oldroutename);
            nvcroute = objRoute.ModuleData;
            UserWorkList list = new UserWorkList();
        }
        else
        {
            objRoute = new Route(Session["Login"].ToString(), "Route", "RouteName");
            nvcroute = objRoute.ModuleData;
        }
        connstring = Application["ConnString"].ToString();
        
        InitScreen();
    }

    protected void AddDetail(object sender, EventArgs e)
    {
      for (int i = 0; i < grdavailable.SelectedItems.Count; i++)
      {
        GridDataItem item = grdavailable.SelectedItems[i] as GridDataItem;
      }
    }

    private void InitScreen()
    {
        screen = new AzzierScreen("route/routemain.aspx", "MainForm", MainControlsPanel.Controls, querymode);
        screen.LCID = Session.LCID;

        if (querymode == "edit")
        {
          grddetail = new RadGrid();
          grddetail.ID = "grddetail";
          grddetail.GroupingSettings.CaseSensitive = false;
          grddetail.AutoGenerateColumns = false;
          grddetail.AllowPaging = true;
          grddetail.PageSize = 100;
          grddetail.AllowSorting = true;
          grddetail.MasterTableView.AllowMultiColumnSorting = true;
          grddetail.AllowFilteringByColumn = true;
          grddetail.ClientSettings.EnableRowHoverStyle = true;
          grddetail.ClientSettings.Selecting.AllowRowSelect = true;
          grddetail.ClientSettings.ClientEvents.OnRowClick = "rowClicked";
          grddetail.MasterTableView.ClientDataKeyNames = new string[] { "Counter" };
          grddetail.MasterTableView.DataKeyNames = new string[] { "Counter" };
          grddetail.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.Top;
          grddetail.ClientSettings.Scrolling.AllowScroll = true;
          grddetail.Skin = "Outlook";
          grddetail.SortingSettings.EnableSkinSortStyles = false;
          grddetail.ClientSettings.Selecting.UseClientSelectColumnOnly = true;
          grddetail.ClientSettings.DataBinding.SortParameterType = GridClientDataBindingParameterType.Linq;
          grddetail.ClientSettings.DataBinding.FilterParameterType = GridClientDataBindingParameterType.Linq;
          grddetail.ClientSettings.DataBinding.SelectMethod = "GetRouteDetail?routename=" + m_routename;
          grddetail.ClientSettings.DataBinding.Location = "../InternalServices/ServiceRoute.svc";
          grddetail.AllowMultiRowSelection = true;
          grddetail.PagerStyle.Visible = true;
          grddetail.PagerStyle.AlwaysVisible = true;
          grddetail.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate("Route Readings");
          

          GridClientSelectColumn cbColumn = new GridClientSelectColumn();
          cbColumn.HeaderStyle.Width = 25;
          grddetail.Columns.Add(cbColumn);

          screen.SetGridColumns("detail", grddetail);
          grddetail.ItemCreated+=new GridItemEventHandler(grddetail_ItemCreated);
          MainControlsPanel.Controls.Add(grddetail);

          grdavailable = new RadGrid();
          grdavailable.ID = "grdavailable";
          grdavailable.GroupingSettings.CaseSensitive = false;
          grdavailable.AutoGenerateColumns = false;
          grdavailable.AllowPaging = true;
          grdavailable.PageSize = 100;
          grdavailable.AllowSorting = true;
          grdavailable.MasterTableView.AllowMultiColumnSorting = true;
          grdavailable.AllowFilteringByColumn = true;
          grdavailable.ClientSettings.EnableRowHoverStyle = true;
          grdavailable.ClientSettings.Selecting.AllowRowSelect = true;
          grdavailable.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.Top;
          grdavailable.ClientSettings.Scrolling.AllowScroll = true;
          grdavailable.Skin = "Outlook";
          grdavailable.SortingSettings.EnableSkinSortStyles = false;
          grdavailable.ClientSettings.DataBinding.SortParameterType = GridClientDataBindingParameterType.Linq;
          grdavailable.ClientSettings.DataBinding.FilterParameterType = GridClientDataBindingParameterType.Linq;
          Validation v = new Validation();
          string wherestr = v.AddLinqConditions("", "route/routemain.aspx", "available", "v_AvailableAttributes", null, null, "edit");
          grdavailable.ClientSettings.DataBinding.SelectMethod = "GetAvailableRoute?routename=" + m_routename + "&wherestr=" + wherestr;
          grdavailable.ClientSettings.DataBinding.Location = "../InternalServices/ServiceRoute.svc";
          grdavailable.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate("Available Readings");
          cbColumn = new GridClientSelectColumn();
          cbColumn.HeaderStyle.Width = 25;
          grdavailable.Columns.Add(cbColumn);
          screen.SetGridColumns("available", grdavailable);
          grdavailable.ItemCreated += new GridItemEventHandler(grdavailable_ItemCreated);
          grdavailable.PagerStyle.Visible = true;
          grdavailable.PagerStyle.AlwaysVisible = true;
          grdavailable.AllowMultiRowSelection = true;
          MainControlsPanel.Controls.Add(grdavailable);

        }
        
        screen.LoadScreen();

        if (!isquery)
        {
          screen.SetValidationControls();
        }
    }

    protected void grddetail_ItemCreated(object sender, GridItemEventArgs e)
    {
      screen.GridItemCreated(e, "route/routemain.aspx", "MainForm", "detail", grddetail);
    }

    protected void grdavailable_ItemCreated(object sender, GridItemEventArgs e)
    {
      screen.GridItemCreated(e, "route/routemain.aspx", "MainForm", "available", grdavailable);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
      if (!Page.IsPostBack)
      {
        if (querymode == "edit")
        {
          screen.PopulateScreen("route", nvcroute);
          HidDivision.Value = nvcroute["division"];
        }
        if (querymode == "duplicate")
        {
          NewNumber n = new NewNumber();
          string routename = "";
          nvcroute["RouteName"] = routename;
          screen.PopulateScreen("Route", nvcroute);
        }
        if (querymode != "edit")
        {
          btnadd.Visible = false;
          btnremove.Visible = false;
        }
      }
      else
      {

        if (Request.Form["__EVENTTARGET"] == "Query")
        {
          Query();
        }
      }
      
        ucHeader1.Mode = querymode;
        ucHeader1.TabName = "Main";
        ucHeader1.ModuleData = nvcroute;

        string oper = "";
        if (querymode == "query")
        {
            oper = "Query";
        }
        else if (querymode == "new")
        {
            oper = "New";
            if (Request.QueryString["numbering"] == "auto")
            {
              TextBox t = MainControlsPanel.FindControl("txtroutename") as TextBox;
              if (t != null)
              {
                if (!Page.IsPostBack)
                {
                  NewNumber num = new NewNumber();
                  t.Text = num.GetNextNumber("Route");
                }
              }
            }

            screen.GetDefaultValue("Route", nvcroute, "Route", "new");
            screen.PopulateScreen("Route", nvcroute);
        }
        else if (querymode == "duplicate")
        {
            oper = "Duplicate";
            litScript1.Text = "<script>clearnonduplicatefields();</script>";
        }
        else if (querymode == "edit")
        {
            oper = "Edit";
        }
        ucHeader1.OperationLabel = oper;
        hidMode.Value = querymode;
    }
  
  
    private void Query()
    {
      NameValueCollection nvc;
      
      nvc = screen.CollectFormValues("Route", true);  // use the view name

      Validation v = new Validation();
      string routename = nvc["RouteName"].ToString() ?? "";
      if (routename.Length > 1)
      {
        if (v.SpecialStrValidator(routename))
        {
          nvc = new NameValueCollection();
          nvc.Add("RouteName", routename);
        }
      }


      Route obj = new Route(Session["Login"].ToString(), "Route", "RouteName");   // use the view name
      string wherestring = "", linqwherestring = "";
      obj.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
      string jscript = "<script type=\"text/javascript\">";
      jscript += "var wherestring = '" + Server.UrlEncode(linqwherestring) + "'; var obj=parent.controlpanel.document.location.href='routepanel.aspx?showlist=queryresult&wherestring='+wherestring;";
      jscript += "</script>";
      litScript1.Text = jscript;
      //litFrameScript.Text = jscript;
    }
   
  
    private void RetrieveMessage()
    {
        SystemMessage msg = new SystemMessage("pdm/pdmmain.aspx");
        m_msg = msg.GetSystemMessage();
        msg.SetJsMessage(litMessage);
    }

}