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
using System.Globalization;
using System.Web.Script.Serialization;

public partial class workorder_Womain : System.Web.UI.Page
{
    protected AzzierScreen screen;
    private string connstring;
    protected string querymode = "";
    protected string m_wonum;
    protected string m_equipID;
    protected NameValueCollection nvcEQUIP;
    Workorder objWorkOrder;
    protected NameValueCollection nvcWO;
    protected RadGrid grdwolabourest;
    protected RadGrid grdwolabouract;
    protected RadGrid grdwomatlact;
    protected RadGrid grdwomatlest;
    protected RadGrid grdwoserviceact;
    protected RadGrid grdwoserviceest;
    protected RadGrid grdwotasksact;
    protected RadGrid grdwotasksest;
    protected RadGrid grdwotoolsest;
    protected RadGrid grdwotoolsact;
    protected RadGrid grdlabourschedule;
    protected DateTime mydate;
    protected bool isquery = false;
    protected int statuscode = 0;
    protected string defstatus = "NEWWO";
    protected NameValueCollection m_msg = new NameValueCollection();
    protected NameValueCollection m_rights;
    protected int m_allowedit = 0;
    protected RadCalendar sharedcalendar;
    protected DataTable decimalplacedatable;
    protected AzzierData ad=new AzzierData();
    protected string focus = "";
    protected decimal worate = 0;
    protected string userrightsjson = "{}";
    protected string numbering = "";

    protected void Page_Init(object sender, EventArgs e)
    {
        RetrieveMessage();
        UserRights.CheckAccess("workorder");
        UserRights r = new UserRights(Session["Login"].ToString(), "UserRights", "counter");
        m_rights = r.GetRights(Session["Login"].ToString(), "workorder");
        userrightsjson = r.RecordInJson(m_rights);
        
        m_allowedit = Convert.ToInt16(m_rights["urEdit"].ToString());


        Session.LCID = Convert.ToInt32(Session["LCID"]);
        if (Request.QueryString["mode"] != null)
            querymode = Request.QueryString["mode"].ToString();
        else
            querymode = "query";
        if (querymode == "")
          querymode = "query";

        if (Request.QueryString["wonum"] != null)
            m_wonum = Request.QueryString["wonum"].ToString();
        else
            m_wonum = "";

        if (Request.QueryString["numbering"] != null)
            numbering = Request.QueryString["numbering"].ToString();
        else
            numbering = "";

        if (querymode == "query")
            isquery = true;

        sharedcalendar = new RadCalendar();
        sharedcalendar.SkinID = "Outlook";
        sharedcalendar.ID = "SharedCalendar";
        MainControlsPanel.Controls.Add(sharedcalendar);

        //Panel dummypanel = new Panel();
        //dummypanel.Style["visibility"] = "hidden";
        //dummypanel.Style["display"] = "none";
        Button btndummy = new Button();
        btndummy.ID = "btndummy";
        btndummy.OnClientClick = "return false;";
        btndummy.Style["Left"] = "-500px";
        btndummy.Style["Position"] = "absolute";
        //dummypanel.Controls.Add(btndummy);
        MainControlsPanel.Controls.Add(btndummy);
        

        InitScreen();
    }

    private void InitLabourScheduleGrid()
    {
        grdlabourschedule = new RadGrid();
        grdlabourschedule.ID = "grdlabourschedule";
        grdlabourschedule.ClientSettings.Scrolling.AllowScroll = true;
        grdlabourschedule.ClientSettings.Scrolling.SaveScrollPosition = true;
        grdlabourschedule.ClientSettings.Scrolling.UseStaticHeaders = true;
        grdlabourschedule.ClientSettings.EnableRowHoverStyle = true;
        //grdloclist.ClientSettings.ClientEvents.OnScroll = "HandleScrolling";
        grdlabourschedule.MasterTableView.TableLayout = GridTableLayout.Fixed;
        //grdloclist.PagerStyle.Visible = false;
        grdlabourschedule.PagerStyle.Visible = true;
        grdlabourschedule.PagerStyle.AlwaysVisible = true;
        grdlabourschedule.Skin = "Outlook";

        grdlabourschedule.Attributes.Add("rules", "all");
        //grdloclist.DataSourceID = "LocListSqlDataSource";
        grdlabourschedule.AutoGenerateColumns = false;
        grdlabourschedule.AllowPaging = true;
        grdlabourschedule.PageSize = 100;
        grdlabourschedule.AllowSorting = true;
        grdlabourschedule.MasterTableView.AllowMultiColumnSorting = true;
        grdlabourschedule.AllowFilteringByColumn = true;
        grdlabourschedule.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.Top;
        grdlabourschedule.MasterTableView.DataKeyNames = new string[] { "Counter" };
        grdlabourschedule.MasterTableView.ClientDataKeyNames = new string[] { "Counter" };

        grdlabourschedule.ClientSettings.Selecting.AllowRowSelect = true;
        grdlabourschedule.ClientSettings.ClientEvents.OnRowSelected = "getGridSelectedItems";
        //grdloclist.ClientSettings.Scrolling.FrozenColumnsCount = 1;
        //grdloclist.PagerStyle.Mode = GridPagerMode.NextPrevAndNumeric;

        grdlabourschedule.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate("Labour Schedule");
        //grdlabourschedule.ItemCreated += new GridItemEventHandler(grdloclist_ItemCreated);

        grdlabourschedule.ClientSettings.DataBinding.FilterParameterType = GridClientDataBindingParameterType.Linq;
        grdlabourschedule.ClientSettings.DataBinding.SortParameterType = GridClientDataBindingParameterType.Linq;

        grdlabourschedule.ClientSettings.DataBinding.SelectMethod = "GetScheduleList?wonum=" + m_wonum;
        grdlabourschedule.ClientSettings.DataBinding.Location = "../InternalServices/ServiceLabour.svc";

        screen.SetGridColumns("labourschedule", grdlabourschedule);
        MainControlsPanel.Controls.Add(grdlabourschedule);

        
    }

    private void InitScreen()
    {
        screen = new AzzierScreen("workorder/womain.aspx", "MainForm", MainControlsPanel.Controls,"query");
        screen.LCID = Session.LCID;
        grdwoserviceact = new RadGrid();
        grdwoserviceest = new RadGrid();
        grdwolabourest = new RadGrid();
        grdwolabouract = new RadGrid();
        grdwomatlact = new RadGrid();
        grdwomatlest = new RadGrid();
        grdwotasksest = new RadGrid();
        grdwotasksact = new RadGrid();
        grdwotoolsest = new RadGrid();
        grdwotoolsact = new RadGrid();
        grdlabourschedule = new RadGrid();

          
          InitAllGrid(grdwoserviceact, "woserviceact", "WOServiceActSqlDataSource", 0);
          grdwoserviceact.Attributes.Add("serviceURL", "../InternalServices/ServiceResource.svc/ClientActService");
          //grdwoserviceact.ItemDataBound += new GridItemEventHandler(grdwoserviceact_ItemDataBound);
          
          InitAllGrid(grdwoserviceest, "woserviceest", "WOServiceEstSqlDataSource", 1);
          grdwoserviceest.Attributes.Add("serviceURL", "../InternalServices/ServiceResource.svc/ClientService");
          //grdwoserviceest.ItemDataBound += new GridItemEventHandler(grdwoserviceest_ItemDataBound);
          
          InitAllGrid(grdwolabourest, "wolabourest", "WOLabourEstSqlDataSource", 1);
          grdwolabourest.Attributes.Add("serviceURL", "../InternalServices/ServiceResource.svc/ClientLabour");
          //grdwolabourest.ItemDataBound += new GridItemEventHandler(grdwolabourest_ItemDataBound);

        // Actual Loabour 
          InitAllGrid(grdwolabouract, "wolabouract", "WOLabourSqlDataSource",0);
          grdwolabouract.Attributes.Add("serviceURL", "../InternalServices/ServiceResource.svc/ClientLabour");
          // Actual Material
          
          InitAllGrid(grdwomatlact, "womatlact", "WOMatlAcctSqlDataSource", 0);
          grdwomatlact.Attributes.Add("serviceURL", "../InternalServices/ServiceResource.svc/ClientActMaterial");
          //grdwomatlact.ItemDataBound += new GridItemEventHandler(grdwomatlact_ItemDataBound);

          // Estimate Material
          InitAllGrid(grdwomatlest, "womatlest", "WOMatlEstSqlDataSource", 1);
          grdwomatlest.Attributes.Add("serviceURL", "../InternalServices/ServiceResource.svc/ClientMaterial");
          //grdwomatlest.ItemDataBound += new GridItemEventHandler(grdwomatlest_ItemDataBound);

        // Tasks Estimate
          InitAllGrid(grdwotasksest, "wotasksest", "WOTasksEstSqlDataSource", 1);
          grdwotasksest.Attributes.Add("serviceURL", "../InternalServices/ServiceResource.svc/ClientTasks");
          //grdwotasksest.ItemDataBound += new GridItemEventHandler(grdwotasksest_ItemDataBound);

        // Tasks Actual
          InitAllGrid(grdwotasksact, "wotasksact", "WOTasksActSqlDataSource", 0);
          grdwotasksact.Attributes.Add("serviceURL", "../InternalServices/ServiceResource.svc/ClientTasks");
          //grdwotasksact.ItemDataBound += new GridItemEventHandler(grdwotasksact_ItemDataBound);

          // Tools Estimate
          InitAllGrid(grdwotoolsest, "wotoolsest", "WOToolsEstSqlDataSource", 1);
          grdwotoolsest.Attributes.Add("serviceURL", "../InternalServices/ServiceResource.svc/ClientTools");
          //grdwotoolsest.ItemDataBound += new GridItemEventHandler(grdwotoolsest_ItemDataBound);

        // Tools Actual
          InitAllGrid(grdwotoolsact, "wotoolsact", "WOToolsActSqlDataSource", 0);
          grdwotoolsact.Attributes.Add("serviceURL", "../InternalServices/ServiceResource.svc/ClientTools");

          InitAllGrid(grdlabourschedule, "labourschedule", "", 0);
          grdlabourschedule.Attributes.Add("serviceURL", "../InternalServices/ServiceResource.svc/ClientGetScheduleList");
          //grdwotoolsact.ItemDataBound += new GridItemEventHandler(grdwotoolsact_ItemDataBound);

          grdwolabourest.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(1,true, "Estimate Labour", "return CopyLabourToActual();", "return editLabour('',1)", m_allowedit,"",false,"","",null,"",Application["AutoApproveWo"].ToString());
          grdwolabouract.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(200, false, "Actual Labour", null, "return editLabour('',0)",1,"",false);
          grdwomatlact.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(200, false, "Actual Material", null, "return editActMaterial('')", m_allowedit,"",false);
          grdwomatlest.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(1, true, "Estimate Material", "return CopyMaterialToActual();", "return editMaterial('',1)", m_allowedit, "", false, "", "", null, "", Application["AutoApproveWo"].ToString());
          grdwotasksest.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(1, true, "Estimate Tasks", "return CopyTasksToActual();", "return editTask('',1)", m_allowedit, "",false, "", "", "return batchaddtask('WORKORDER','1');", "Add Task Library", Application["AutoApproveWo"].ToString());
          grdwotasksact.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(200, false, "Actual Tasks", null, "return editTask('',0)", m_allowedit, "", false, "", "", "return batchaddtask('WORKORDER','0');", "Add Task Library", Application["AutoApproveWo"].ToString());
          grdwoserviceest.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(1, true, "Estimate Services", "return CopyServicesToActual();", "return editService('',1)", m_allowedit, "", false, "", "", null, "", Application["AutoApproveWo"].ToString());
          grdwoserviceact.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(200, false, "Actual Services", null, "return editActService('')", m_allowedit,"",false);
          grdwotoolsest.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(1, true, "Estimate Tools", "return CopyToolsToActual();", "return editTools('',1)", m_allowedit,"",false);
          grdwotoolsact.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(200, false, "Actual Tools", null, "return editTools('',0)", m_allowedit, "", false, "", "", null, "", Application["AutoApproveWo"].ToString());

          MainControlsPanel.Controls.Add(grdwomatlact);
          MainControlsPanel.Controls.Add(grdwomatlest);
          MainControlsPanel.Controls.Add(grdwolabouract);
          MainControlsPanel.Controls.Add(grdwolabourest);
          MainControlsPanel.Controls.Add(grdwoserviceact);
          MainControlsPanel.Controls.Add(grdwoserviceest);
          MainControlsPanel.Controls.Add(grdwotasksest);
          MainControlsPanel.Controls.Add(grdwotasksact);
          MainControlsPanel.Controls.Add(grdwotoolsest);
          MainControlsPanel.Controls.Add(grdwotoolsact);

          //InitLabourScheduleGrid();

          screen.LoadScreen();
          screen.SetValidationControls();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
            // add radio button options
            RadioButtonList r = (RadioButtonList) MainControlsPanel.FindControl("rblchargeback");
            if (r != null)
            {
                r.Items.Clear();
              r.RepeatDirection = RepeatDirection.Horizontal;
              ListItem litm1 = new ListItem("Yes", "1");
              r.Items.Add(litm1);
              ListItem litm2 = new ListItem("No", "0");
              r.Items.Add(litm2);
              ListItem litm3 = new ListItem("All", "");
              r.Items.Add(litm3);
              r.SelectedIndex = 2;

            }

        hidkeyvalue.Value = m_wonum;
        hidMode.Value = querymode;
        hiduserrights.Value = userrightsjson;
        objWorkOrder = new Workorder(Session["Login"].ToString(), "Workorder", "WONum");
        nvcWO = objWorkOrder.ModuleData;
        ucHeader1.Mode = querymode;
        ucHeader1.TabName = "Main";
        ucHeader1.ModuleData = nvcWO;

        TextBox txtlocation = MainControlsPanel.FindControl("txtlocation") as TextBox;
        txtlocation.Attributes.Add("onFocus", "locFocus()");
        HyperLink h = MainControlsPanel.FindControl("lkulocation") as HyperLink;
        if (h != null)
        {
            h.NavigateUrl = "javascript:loclookup('1')";
        }
        h = MainControlsPanel.FindControl("tlklocation") as HyperLink;
        if (h != null)
        {
            h.NavigateUrl = "javascript:loclookup('2')";
        }

        TextBox tbx = MainControlsPanel.FindControl("txtphase") as TextBox;
        //tbx.Attributes.Add("onFocus", "phaseFocus()");
        //tbx.Attributes.Add("onblur", "phaseblur()");
        if (tbx.Attributes["onblur"] != null)
            tbx.Attributes["onblur"] = tbx.Attributes["onblur"] + ";phaseblur();";
        else
            tbx.Attributes.Add("onblur", "phaseblur();");


        h = MainControlsPanel.FindControl("lkuphase") as HyperLink;
        if (h != null)
        {
            h.NavigateUrl = "javascript:phaselookup()";
        }
    }

    private void ProcessCancelledWorkorder()
    {
      if (Workorder.OpenWorkRequestCount(m_wonum) > 0)
      {
        RadAjaxManager1.ResponseScripts.Add("showwrlist('" + m_wonum + "')");
      }
    }

    protected void grdwolabouract_ItemDataBound(object sender, GridItemEventArgs e)
    {
    }

    protected void grdwolabourest_ItemDataBound(object sender, GridItemEventArgs e)
    {
    }

    protected void grdwomatlact_ItemDataBound(object sender, GridItemEventArgs e)
    {
    }

    protected void grdwomatlest_ItemDataBound(object sender, GridItemEventArgs e)
    {
    }

    private Boolean SaveWO() 
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        TextBox tbx = null;
        string dirtylog = "0";
        if (CntlPanel != null)
        {
            nvc = screen.CollectFormValues("workorder", false);  // use the table name
            tbx = CntlPanel.FindControl("txtdirtylog") as TextBox;
            if (nvc["dirtylog"] == null)
            {
                if (tbx != null)
                {
                    dirtylog = tbx.Text;
                    nvc.Add("dirtylog", dirtylog);
                }
            }
            else
                dirtylog = nvc["dirtylog"];
        }
        else
            nvc = null;

        Workorder objWorkorder;
        bool success = false;
        if (querymode == "new")
        {
            objWorkorder = new Workorder(Session["Login"].ToString(), "Workorder", "WoNum");   // use the table name
            success = objWorkorder.Create(nvc);
            if (success)
            {
              if (nvc["procnum"] != "")
              {
                Labour l = new Labour(Session["Login"].ToString(), "WOLabour", "Counter");
                l.CopyLabour("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                Service s = new Service(Session["Login"].ToString(), "WOService", "Counter");
                s.CopyService("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                Material m = new Material(Session["Login"].ToString(), "WOMaterial", "Counter");
                m.CopyMaterial("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                Tool t = new Tool(Session["Login"].ToString(), "WOTools", "Counter");
                t.CopyTools("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                Task task = new Task(Session["Login"].ToString(), "WOTasks", "Counter");
                task.CopyTasks("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
              }
            }
        }
        else if (querymode == "edit")
        {
            string wo = nvc["wonum"];
            objWorkorder = new Workorder(Session["Login"].ToString(), "Workorder", "WoNum", wo);   // use the table name
            success = objWorkorder.Update(nvc);
            if (success)
            {
              string oldproc = hidprocnum.Value;
              if ((nvc["procnum"].ToString() != "") && (nvc["procnum"].ToString() != oldproc))
              {
                Labour l = new Labour(Session["Login"].ToString(), "WOLabour", "Counter");
                l.DeleteAllLabour("workorder", nvc["wonum"], 1);
                l.CopyLabour("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                Service s = new Service(Session["Login"].ToString(), "WOService", "Counter");
                s.CopyService("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                Material m = new Material(Session["Login"].ToString(), "WOMaterial", "Counter");
                m.CopyMaterial("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                Tool t = new Tool(Session["Login"].ToString(), "WOTools", "Counter");
                t.CopyTools("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                Task task = new Task(Session["Login"].ToString(), "WOTasks", "Counter");
                task.CopyTasks("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                hidprocnum.Value = nvc["procnum"];
              }
            }
        }
        else
            objWorkorder = new Workorder(Session["Login"].ToString(), "Workorder", "WoNum");
        return success;

    }

    private void SaveStatus()
    {
        //SaveWO();

        //NameValueCollection nvc = new NameValueCollection();
        //nvc.Add("statuscode", hidTargetStatusCode.Value);
        //nvc.Add("status", hidTargetStatus.Value);
        //nvc.Add("Remark", hidStatusComments.Value);
        //if (Convert.ToInt32(hidTargetStatusCode.Value) < 300)
        //    nvc.Add("compdate", "");

        //Workorder objWorkOrder = new Workorder(Session["Login"].ToString(), "workorder", "wonum", m_wonum);
        //int statuscode = Convert.ToInt16(hidTargetStatusCode.Value);
        //if (statuscode >= 100 && statuscode < 200)
        //{
        //    ApproveClass appr = new ApproveClass("WORKORDER", m_wonum, statuscode);
        //    if (appr.IsFinalApprove(hidTargetStatus.Value) == true)
        //    {
        //        nvc["statuscode"] = "200";
        //        nvc["status"] = "APPR";
        //    }
        //}

        //if (objWorkOrder.UpdateStatus(nvc))
        //{
        //    //hidUpdateSuccess.Value = "success";
        //    litScript1.Text = "document.location.href='Womain.aspx?mode=edit&wonum=" + m_wonum + "';";
        //}
        //else
        //  litScript1.Text = "alert('" + objWorkOrder.ErrorMessage + "');";
    }

    protected void grdwoserviceact_ItemDataBound(object sender, GridItemEventArgs e)
    {
    }

    protected void grdwoserviceest_ItemDataBound(object sender, GridItemEventArgs e)
    {
    }

    protected void grdwotoolsest_ItemDataBound(object sender, GridItemEventArgs e)
    {
    }

    protected void grdwotoolsact_ItemDataBound(object sender, GridItemEventArgs e)
    {
    }

    protected void grdwotasksest_ItemDataBound(object sender, GridItemEventArgs e)
    {
    }

    protected void grdwotasksact_ItemDataBound(object sender, GridItemEventArgs e)
    {
    }

    
    protected void InitAllGrid(RadGrid grd, string controlid, string grddatasource, int Estflag)
    {
        grd.ID = string.Concat("grd", controlid);
        //grd.DataSourceID = grddatasource;
        grd.DataSource = new DataTable();
        //grd.GridLines = GridLines.Both;
        //grd.MasterTableView.GridLines = GridLines.Both;
        grd.ClientSettings.EnableRowHoverStyle = true;
        grd.AutoGenerateColumns = false;
        grd.AllowSorting = true;
        grd.MasterTableView.AllowMultiColumnSorting = true;
        grd.ShowHeader = true;
        grd.ShowFooter = true;
        grd.ValidationSettings.EnableValidation = true;
        grd.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.Top;
        grd.Skin = "Outlook";
        grd.ClientSettings.ClientEvents.OnCommand = "ClientGrid_OnCommand";
        grd.PagerStyle.AlwaysVisible = true;
        grd.EnableViewState = false;

        if (Estflag == 1)
        {
            grd.ClientSettings.ClientEvents.OnRowSelecting = "rowselecting";
            grd.ClientSettings.ClientEvents.OnRowDataBound = "rowbound";
        }

        grd.ClientSettings.ClientEvents.OnRowClick = "resourceclick";

        grd.AllowPaging = true;
        grd.PageSize = 100;

        if (Estflag == 1)
        {
            grd.ClientSettings.Selecting.AllowRowSelect = true;
            grd.AllowMultiRowSelection = true;

            grd.ClientSettings.Selecting.UseClientSelectColumnOnly = true;
            //grd.ClientSettings.ClientEvents.OnRowSelecting = "Selecting";

            GridClientSelectColumn cbColumn = new GridClientSelectColumn();
            cbColumn.UniqueName = "SelectColumn";

            grd.Columns.Add(cbColumn);
            cbColumn.HeaderStyle.Width = 25;
        }
        screen.SetGridColumns(controlid, grd,true);
        
        if (controlid == "womatlact" || controlid == "woserviceact")
        {
          grd.MasterTableView.DataKeyNames = new string[] { "BatchNum" };
          grd.MasterTableView.ClientDataKeyNames = new string[] { "BatchNum" };
        }
        else if (controlid == "womatlest")
        {
          grd.MasterTableView.DataKeyNames = new string[] { "Counter","Store","Actual" };
          grd.MasterTableView.ClientDataKeyNames = new string[] { "Counter","Store","Actual" };
        }
        else
        {
          grd.MasterTableView.DataKeyNames = new string[] { "Counter","Actual" };
          grd.MasterTableView.ClientDataKeyNames = new string[] { "Counter","Actual" };
        }
        
    }

    private void RetrieveMessage()
    {
        SystemMessage msg = new SystemMessage("workorder/womain.aspx");
        m_msg = msg.GetSystemMessage();
        msg.SetJsMessage(litMessage);
    }
}