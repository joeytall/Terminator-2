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

public partial class interface_infmain : System.Web.UI.Page
{
    AzzierScreen screen;
    private string connstring;
    protected string querymode = "";
    protected string m_interfacename, m_oldinterface = "";
    protected string m_interfacemodule, m_interfacetype = "";

    ModuleoObject objinf;
    NameValueCollection nvcinf;
    protected RadGrid grdinftable;
    protected bool isquery = false;
    protected int statuscode = 0;
    protected NameValueCollection m_msg = new NameValueCollection();
    protected NameValueCollection m_rights;
    protected int m_allowedit = 0;
    protected string t_duplicate;

    protected void Page_Init(object sender, EventArgs e)
    {
        RetrieveMessage();
        UserRights.CheckAccess("Interface");
        UserRights r = new UserRights(Session["Login"].ToString(), "UserRights", "counter");
        m_rights = r.GetRights(Session["Login"].ToString(), "interface");
        
        m_allowedit = Convert.ToInt16(m_rights["urEdit"].ToString());

        Session.LCID = Convert.ToInt32(Session["LCID"]);
        if (Request.QueryString["mode"] != null)
        {
          querymode = Request.QueryString["mode"].ToString();
          if (querymode=="")
            querymode = "query";
        }
        else
          querymode = "query";

        if (Request.QueryString["interfacename"] != null)
        {
          m_interfacename = Request.QueryString["interfacename"].ToString();
          m_oldinterface = Request.QueryString["interfacename"].ToString();
          HidOldInterface.Value = Request.QueryString["interfacename"].ToString();
        }
        else
            m_interfacename = "";


        if (querymode == "query")
            isquery = true;

        if (querymode == "edit")
        {
            objinf = new ModuleoObject(Session["Login"].ToString(), "infmaster", "interfacename", m_interfacename);

            nvcinf = objinf.ModuleData;
            m_interfacemodule = nvcinf["interfacemodule"];
            m_interfacetype = nvcinf["interfacetype"];
            if (nvcinf["interfacename"].ToString() == "")
            {
              querymode = "query";
              isquery = true;
              m_interfacename = "";
            }
            UserWorkList list = new UserWorkList();
            list.AddToRecentList(Session["Login"].ToString(), "Interface", m_interfacename);
        }
        else if (querymode == "duplicate" && m_oldinterface.Length>0)
        {
            objinf = new ModuleoObject(Session["Login"].ToString(), "infmaster", "interfacename", m_oldinterface);

            nvcinf = objinf.ModuleData;
            UserWorkList list = new UserWorkList();
            list.AddToRecentList(Session["Login"].ToString(), "Interface", m_oldinterface);
            litScript1.Text = "clearoldempval()";
        }
        else
        {
            objinf = new ModuleoObject(Session["Login"].ToString(), "infmaster", "interfacename");
            nvcinf = objinf.ModuleData;
            statuscode = 1;
        }
        hidMode.Value = querymode;
        connstring = Application["ConnString"].ToString();
        screen = new AzzierScreen("interface/infmain.aspx", "MainForm", MainControlsPanel.Controls, querymode);
        Session.LCID = Convert.ToInt32(Session["LCID"]);
        screen.LCID = Session.LCID;
        if (querymode == "edit")
        {
          InitGrid();
        }

        screen.LoadScreen();

        if (!isquery)
          screen.SetValidationControls();

        RadComboBox cbbinterfacetype = (RadComboBox)MainControlsPanel.FindControl("cbbinterfacetype");
        RadComboBox cbbusermodule = (RadComboBox)MainControlsPanel.FindControl("cbbusermodule");

        if ((querymode != "edit") && (querymode != "duplicate"))
        {

            UserModuleListSqlDataSource.ConnectionString = connstring;
            UserModuleListSqlDataSource.SelectCommand = "SELECT DISTINCT InterfaceModule FROM InfTable WHERE InterfaceName = 'system'";

            InterfaceTypeListSqlDataSource.ConnectionString = connstring;
            InterfaceTypeListSqlDataSource.SelectCommand = "SELECT DISTINCT InterfaceType FROM InfTable WHERE InterfaceName = 'system'";

            cbbinterfacetype.DataSourceID = "InterfaceTypeListSqlDataSource";
            cbbinterfacetype.DataTextField = "InterfaceType";
            cbbinterfacetype.DataValueField = "InterfaceType";

            cbbinterfacetype.DefaultItem.Text = "Select type";
            cbbinterfacetype.DefaultItem.Value = "";

            cbbusermodule.DataSourceID = "UserModuleListSqlDataSource";
            cbbusermodule.DataTextField = "InterfaceModule";
            cbbusermodule.DataValueField = "InterfaceModule";

            cbbusermodule.DefaultItem.Text = "Select module";
            cbbusermodule.DefaultItem.Value = "";

        }
        else
        {
            if (nvcinf["usermodule"] != "")
            {
                cbbusermodule.SelectedValue = nvcinf["usermodule"];
                RadComboBoxItem currentmodule = new RadComboBoxItem();
                currentmodule.Value =nvcinf["usermodule"];
                currentmodule.Text = nvcinf["usermodule"];
                cbbusermodule.Items.Add(currentmodule);

            }
            if (nvcinf["interfacetype"] != "")
            {
                cbbinterfacetype.SelectedValue = nvcinf["interfacetype"];
                RadComboBoxItem currenttype = new RadComboBoxItem();
                currenttype.Value = nvcinf["interfacetype"];
                currenttype.Text = nvcinf["interfacetype"];
                cbbinterfacetype.Items.Add(currenttype);
            }
        }
     
    }

    private void InitGrid()
    {
      grdinftable = new RadGrid();
      grdinftable.ID = "grdinftable";
      grdinftable.DataSourceID = "InfTablesSqlDataSource";
      grdinftable.ShowHeader = true;
      grdinftable.MasterTableView.ShowHeadersWhenNoRecords = true;
      grdinftable.AutoGenerateColumns = false;
      grdinftable.MasterTableView.AllowMultiColumnSorting = true;
      grdinftable.MasterTableView.AllowSorting = true;

      InfTablesSqlDataSource.ConnectionString = connstring;
      InfTablesSqlDataSource.SelectCommand = "SELECT * FROM inftable WHERE interfacename='" + m_interfacename + "'";
      grdinftable.MasterTableView.DataKeyNames = new string[] { "Counter" };
      grdinftable.PageSize = 100;
      grdinftable.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.Top;
      grdinftable.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(1, true, "Interface Tables", "", null);
      grdinftable.MasterTableView.ShowHeadersWhenNoRecords = true;
      grdinftable.ItemDataBound+=new GridItemEventHandler(grdinftable_ItemDataBound);
      grdinftable.Skin = "Outlook";
      if (m_allowedit == 1)
      {
        GridEditCommandColumn EditColumn = new GridEditCommandColumn();
        EditColumn.HeaderText = "";
        EditColumn.UniqueName = "EditCommand";
        EditColumn.ButtonType = GridButtonColumnType.ImageButton;
        EditColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        EditColumn.HeaderStyle.Width = 30;
        grdinftable.MasterTableView.Columns.Add(EditColumn);
        //grdinftable.ClientSettings.EnablePostBackOnRowClick = true;
        
      }
      screen.SetGridColumns("inftable", grdinftable);

      MainControlsPanel.Controls.Add(grdinftable);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            RadComboBox cbbusermodule = MainControlsPanel.FindControl("cbbusermodule") as RadComboBox;
            RadComboBox cbbinterfacetype = MainControlsPanel.FindControl("cbbinterfacetype") as RadComboBox;
            
            if (querymode == "edit")
            {
                screen.PopulateScreen("infmaster", nvcinf);
                //HidDivision.Value = nvcinf["division"];
            }
            if (querymode == "duplicate") 
            {
                nvcinf["interfacename"] = "";
                screen.PopulateScreen("infmaster", nvcinf);

                
                cbbinterfacetype.AllowCustomText = false;
                cbbinterfacetype.MarkFirstMatch = false;
                cbbinterfacetype.EnableLoadOnDemand = false;

                cbbusermodule.AllowCustomText = false;
                cbbusermodule.MarkFirstMatch = false;
                cbbusermodule.EnableLoadOnDemand = false;

            }
            

            RadioButtonList r = MainControlsPanel.FindControl("rblinactive") as RadioButtonList;
            if (r != null)
            {
              r.Style.Add("valign", "top");
              r.RepeatDirection = RepeatDirection.Horizontal;
              ListItem litm1 = new ListItem("Yes", "1");
              r.Items.Add(litm1);
              ListItem litm2 = new ListItem("No", "0");
              r.Items.Add(litm2);
              if (querymode == "query")
              {
                ListItem litm3 = new ListItem("All", "");
                r.Items.Add(litm3);
                r.SelectedIndex = 1;
              }
              else if (querymode == "edit")
              {
                if (nvcinf["inactive"].ToString() == "1")
                  r.SelectedIndex = 0;
                else
                  r.SelectedIndex = 1;
              }
              else
              {
                r.SelectedIndex = 1;
              }
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
        ucHeader1.ModuleData = nvcinf;
        ucHeader1.OldInterfacename = m_oldinterface;

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
        /*
        if (querymode == "edit")
        {
            TextBox tbx = MainControlsPanel.FindControl("txtempid") as TextBox;
            tbx.Attributes.Add("readonly", "readonly");
        }
         * */


        screen.PopulateScreen("InfMaster", nvcinf);
    }

    private void Query()
    {
        NameValueCollection nvc;
        nvc = screen.CollectFormValues("infmaster", true);
        Interface objinf = new Interface(Session["Login"].ToString(), "infmaster", "interfacename");

        string wherestring = "", wherestringlinq = "";
        //string[] empids = objEmpolyee.Query(nvc, ref wherestring);
        //string[] infs = objinf.LinqQuery(nvc, ref wherestring,ref wherestringlinq);
        objinf.CreateLinqCondition(nvc, ref wherestring, ref wherestringlinq);
        string jscript = "";
        jscript += "var wherestring = '" + Server.UrlEncode(wherestringlinq) + "';var obj=parent.controlpanel.document.location.href='infpanel.aspx?showlist=queryresult&wherestring='+wherestring;";
        litScript1.Text = jscript;
    }

    protected void grdinftable_ItemDataBound(object sender, GridItemEventArgs e)
    {

      if (e.Item is GridDataItem && !e.Item.IsInEditMode)
      {
        GridDataItem item = (GridDataItem)e.Item;

        if (m_allowedit == 1)
        {
          ImageButton btn = (ImageButton)item["EditCommand"].Controls[0];
          btn.ImageUrl = "~/images/Edit.gif";
          btn.OnClientClick = "return editMap('" + item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString() + "')";
        }
      }
      //screen.GridItemDataBound(e, "pm/pmmain.aspx", "MainForm", "tools");
    }

    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
    }

    private void RetrieveMessage()
    {
        SystemMessage msg = new SystemMessage("labour/labourmain.aspx");
        m_msg = msg.GetSystemMessage();
        msg.SetJsMessage(litMessage);
    }
}