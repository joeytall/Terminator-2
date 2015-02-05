using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.OleDb;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Web.UI.HtmlControls;

public partial class workorder_Woheader : System.Web.UI.UserControl
{
  protected string m_mode;
  protected string m_tabname = "";
  private string m_operationlabel;
  private bool m_completed = false;
  private bool m_approved = false;
  protected string m_wonum = "";
  protected string m_equipment = "";
  protected string m_location = "";
  private NameValueCollection m_nvc;
  private AzzierScreen screen;
  protected int defaultprintcounter = -1;
  //    protected RadToolBar toolbar = new RadToolBar();
  protected string linksrvr = "";
  private int m_AllowEdit;
  private string m_wodiv;
  //protected RadComboBox toolbarcombobox;

  protected NameValueCollection m_msg = new NameValueCollection();
  protected RadToolBarDropDown toolbarcombobox = new RadToolBarDropDown();

  protected RadComboBox tmp_rcb;
  protected RadComboBoxItem tmpcbi;
  protected int mainmodul_ScrnSize = 0, tmp_ScrnIconNum = 0, tmp_ActIconNum = 0;

  private string timerstatus = "";
  //private bool c_005, c_006, c_007, c_008, c_009, c_010, c_011, c_012;

  public string TimerStatus
  {
      set 
      { 
          timerstatus = value;
          hidtimerstatus.Value = value;
      }
      get
      {
          return timerstatus;
      }
  }
  public string Mode
  {
    set { m_mode = value; }
  }

  public string TabName
  {
    set { m_tabname = value; }
  }

  public string OperationLabel
  {
    set { m_operationlabel = value; }
  }

  public NameValueCollection ModuleData
  {
    set
    {
      m_nvc = value;
      if (m_nvc["statuscode"].ToString() != "")
      {
        if (Convert.ToInt16(m_nvc["statuscode"]) >= 200)
          m_approved = true;
        if (Convert.ToInt16(m_nvc["statuscode"]) >= 300)
          m_completed = true;
      }
      if (m_nvc["equipment"].ToString() != "")
      {
        m_equipment = m_nvc["equipment"].ToString();
      }
      if (m_nvc["location"].ToString() != "")
      {
        m_location = m_nvc["location"].ToString();
      }

      m_wonum = m_nvc["wonum"].ToString();
      m_wodiv = m_nvc["division"].ToString();
    }
  }

  protected void Page_Load(object sender, EventArgs e)
  {

    RetrieveMessage();
    //if (!Page.IsPostBack)
    //{
    tmp_rcb = new RadComboBox();
    RadComboBox toolbarcombobox = new RadComboBox();

    //mainmodul_ScrnSize = 260;// Convert.ToInt32(Session["ScreenWidth"].ToString());
    mainmodul_ScrnSize = Convert.ToInt32(Session["ScreenWidth"].ToString());
    mainmodul_ScrnSize = (int)Math.Floor(mainmodul_ScrnSize * 0.67) - 32;
    //mainmodul_ScrnSize = (int)Math.Floor(mainmodul_ScrnSize * (Convert.ToDecimal(Session["WOFrameMain"].ToString()) / 100)) - 32;
    //mainmodul_ScrnSize = 260;// 500;// 260;// 500;// (int)Math.Floor(mainmodul_ScrnSize * 0.67) - 32;
    decimal t_iconnum = mainmodul_ScrnSize / 50;
    //decimal t_iconnum = mainmodul_ScrnSize * (Convert.ToDecimal(Session["WOFrameMain"].ToString()) / 100) / 50;
    tmp_ScrnIconNum = (int)Math.Floor(t_iconnum);
    //c_005 = false; c_006 = false; c_007 = false; c_008 = false; c_009 = false; c_010 = false; c_011 = false; c_012 = false;

    Session.LCID = Convert.ToInt32(Session["LCID"]);
    string connstring = Application["ConnString"].ToString();

    litFrameScript.Text = "";
    litDateLCIDScript.Text = "<script type=\"text/javascript\">var g_LCID = " + Session.LCID.ToString() + ";</script>";

    defaultprintcounter = AzzierPrintForm.GetDefaultReport("Workorder");

    if (m_tabname == "Main")
    {
      tabhlkMain.NavigateUrl = "javascript:void(null)";
      tabimgMain.ImageUrl = "../images/tabbutton_down.png";
      tablblMain.CssClass = "toptabover";
    }
    else
    {
      tabhlkMain.NavigateUrl = "Womain.aspx?mode=edit&wonum=" + m_nvc["wonum"];
      tabhlkMain.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgMain','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblMain','toptabover')");
      tabhlkMain.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgMain','../images/tabbutton.png');classChangeover('ucHeader1_tablblMain','toptabout')");
    }

    if (m_tabname == "Accounts")
    {
      tabhlkAccounts.NavigateUrl = "javascript:void(null)";
      tabimgAccounts.ImageUrl = "../images/tabbutton_down.png";
      tablblAccounts.CssClass = "toptabover";
    }
    else
    {
      if (m_mode == "edit")
      {
        tabhlkAccounts.NavigateUrl = "Woaccounts.aspx?wonum=" + m_nvc["wonum"];
        tabhlkAccounts.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgAccounts','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblAccounts','toptabover')");
        tabhlkAccounts.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgAccounts','../images/tabbutton.png');classChangeover('ucHeader1_tablblAccounts','toptabout')");
      }
      else
      {
        tabhlkAccounts.Enabled = false;
        tablblAccounts.CssClass = "toptabinactive";
      }
    }

    if (m_tabname == "History")
    {
      tabhlkHistory.NavigateUrl = "javascript:void(null)";
      tabimgHistory.ImageUrl = "../images/tabbutton_down.png";
      tablblHistory.CssClass = "toptabover";
    }
    else
    {
      if (m_mode == "edit")
      {
        tabhlkHistory.NavigateUrl = "Wohistory.aspx?wonum=" + m_nvc["wonum"];
        tabhlkHistory.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgHistory','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblHistory','toptabover')");
        tabhlkHistory.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgHistory','../images/tabbutton.png');classChangeover('ucHeader1_tablblHistory','toptabout')");
      }
      else
      {
        tabhlkHistory.Enabled = false;
        tablblHistory.CssClass = "toptabinactive";
      }
    }

    if (m_tabname == "Complete")
    {
      tabhlkComplete.NavigateUrl = "javascript:void(null)";
      tabimgComplete.ImageUrl = "../images/tabbutton_down.png";
      tablblComplete.CssClass = "toptabover";
    }
    else
    {
      if (m_mode == "edit" && m_approved)
      {
        tabhlkComplete.NavigateUrl = "Wocomp.aspx?wonum=" + m_nvc["wonum"];
        tabhlkComplete.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgComplete','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblComplete','toptabover')");
        tabhlkComplete.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgComplete','../images/tabbutton.png');classChangeover('ucHeader1_tablblComplete','toptabout')");
      }
      else
      {
        tabhlkComplete.Enabled = false;
        tablblComplete.CssClass = "toptabinactive";
      }
    }

    if (m_tabname == "Work Request")
    {
      tabhlkWorkRequest.NavigateUrl = "javascript:void(null)";
      tabimgWorkRequest.ImageUrl = "../images/tabbutton_down.png";
      tablblWorkRequest.CssClass = "toptabover";
    }
    else
    {
      if (m_mode == "edit")
      {
        tabhlkWorkRequest.NavigateUrl = "relatewr.aspx?wonum=" + m_nvc["wonum"];
        tabhlkWorkRequest.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgWorkRequest','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblWorkRequest','toptabover')");
        tabhlkWorkRequest.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgWorkRequest','../images/tabbutton.png');classChangeover('ucHeader1_tablblWorkRequest','toptabout')");
      }
      else
      {
        tabhlkWorkRequest.Enabled = false;
        tablblWorkRequest.CssClass = "toptabinactive";
      }
    }

    if (!Page.IsPostBack)
    {
        AzzierHeader.InitToolBar(toolbar, "WorkOrder", ShowToolbarButtonRights);
    }

    litOperation.Text = m_operationlabel;
    AzzierHeader.CreateLinksDropDown(plhLinkDoc, "WorkOrder", m_wonum, out linksrvr);
    AzzierHeader.CreateReportsDropDown(plhReports, "WorkOrder");
    AzzierHeader.CreateQueryDropDown(plhQuery, "WorkOrder");
    
    if (Page.IsPostBack)
    {
      if (Request.Form["__EVENTTARGET"] == "complete")
      {
        lbnCompleteWorkOrder_Click(null, null);
      }
      if (Request.Form["__EVENTTARGET"] == "lookup")
      {
        string querystr = Request["__EVENTARGUMENT"];

        lbnLookUpWorkOrder_Click(null, null, querystr);
      }
      if (Request.Form["__EVENTTARGET"] == "openquery")
      {
        string parameter = Request["__EVENTARGUMENT"]; // parameter

        lbnOpenQuery_Click(null, null, parameter);
      }
    }
     
  }

  protected void lbnLookUpWorkOrder_Click(object sender, EventArgs e, string querystr)
  {
      NameValueCollection nvc;
      Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
      if (CntlPanel != null)
      {
          screen = new AzzierScreen("workorder/womain.aspx", "MainForm", CntlPanel.Controls);
          screen.LCID = Session.LCID;
          nvc = screen.CollectFormValues("workorder", true);  // use the view name
      }
      else
          nvc = null;

      Workorder objWorkorder = new Workorder(Session["Login"].ToString(), "workorder", "Wonum");   // use the view name

      string wherestring = "", linqwherestring = "";
      objWorkorder.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);

      string jscript = "";
      jscript = "<script>";
      jscript += "var wherestring = '" + Server.UrlEncode(linqwherestring) + "'; var querystr = '" + Server.UrlEncode(querystr) + "'; var obj=parent.controlpanel.document.location.href='wopanel.aspx?showlist=queryresult&wherestring='+wherestring+'&querystr='+querystr;";
    jscript += "</script>";
    litFrameScript.Text = jscript;

  }

  protected void lbnOpenQuery_Click(object sender, EventArgs e, string qid)
  {
    NameValueCollection nvc = new NameValueCollection();

    nvc = UserQuery.CreateNameValueCollection(qid);

    Workorder objWorkorder = new Workorder(Session["Login"].ToString(), "workorder", "Wonum");   // use the view name

    string wherestring = "", linqwherestring = "";
    objWorkorder.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);

    string jscript = "";
    jscript = "<script>";
    jscript += "var wherestring = '" + Server.UrlEncode(linqwherestring) + "'; var obj=parent.controlpanel.document.location.href='wopanel.aspx?showlist=queryresult&wherestring='+wherestring;";
    jscript += "</script>";
    litFrameScript.Text = jscript;
  }


  private void SaveWO() //
  {
    NameValueCollection nvc;
    Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
    TextBox tbx = null;
    string dirtylog = "0";
    if (CntlPanel != null)
    {
      screen = new AzzierScreen("workorder/womain.aspx", "MainForm", CntlPanel.Controls);
      screen.LCID = Session.LCID;
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
    if (m_mode == "new")
    {
      objWorkorder = new Workorder(Session["Login"].ToString(), "Workorder", "WoNum");   // use the table name
      success = objWorkorder.Create(nvc);
      if (success)
      {
        string wonum = nvc["wonum"].ToString().ToUpper();
        string[] wonums = { wonum };
      }
    }
    else if (m_mode == "edit")
    {
      string wo = nvc["wonum"];
      objWorkorder = new Workorder(Session["Login"].ToString(), "Workorder", "WoNum", wo);   // use the table name
      success = objWorkorder.Update(nvc);
    }
    else
      objWorkorder = new Workorder(Session["Login"].ToString(), "Workorder", "WoNum");

    if (!success)
    {
      string jscript = "<script>alert(\"" + objWorkorder.ErrorMessage.Replace("\r\n", "") + "\");</script>";
      litFrameScript.Text = jscript;
    }
    else
    {
      if (m_mode == "new")
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
        Response.Redirect("Womain.aspx?mode=edit&wonum=" + Server.UrlEncode(nvc["wonum"]));
      }
      else if (m_mode == "edit")
      {
        HiddenField h = Page.FindControl("hidprocnum") as HiddenField;
        string oldproc = h.Value;
        if ((nvc["procnum"].ToString() != "") && (nvc["procnum"].ToString() != oldproc))
        {
          Labour l = new Labour(Session["Login"].ToString(), "WOLabour", "Counter");
          l.DeleteAllLabour("workorder", nvc["wonum"], 1);
          l.CopyLabour("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
          Service s = new Service(Session["Login"].ToString(), "WOService", "Counter");
          s.DeleteAllService("workorder", nvc["wonum"], 1);
          s.CopyService("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
          Material m = new Material(Session["Login"].ToString(), "WOMaterial", "Counter");
          m.DeleteAllMaterial("workorder", nvc["wonum"], 1);
          m.CopyMaterial("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
          Tool t = new Tool(Session["Login"].ToString(), "WOTools", "Counter");
          t.DeleteAllTools("workorder", nvc["wonum"], 1);
          t.CopyTools("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
          Task task = new Task(Session["Login"].ToString(), "WOTasks", "Counter");
          task.DeleteAllTask("workorder", nvc["wonum"], 1);
          task.CopyTasks("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
          h.Value = nvc["procnum"];
          RadGrid g = Page.FindControl("grdwolabourest") as RadGrid;
          g.Rebind();
          g = Page.FindControl("grdwomatlest") as RadGrid;
          g.Rebind();
          g = Page.FindControl("grdwotasksest") as RadGrid;
          g.Rebind();
          g = Page.FindControl("grdwotoolsest") as RadGrid;
          g.Rebind();
          g = Page.FindControl("grdwoserviceest") as RadGrid;
          g.Rebind();

        }
        tbx.Text = (Convert.ToInt32(dirtylog) + 1).ToString();
      }
    }
  }

  protected void lbnCompleteWorkOrder_Click(object sender, EventArgs e)
  {
    NameValueCollection nvc;
    Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
    TextBox tbx = null;
    RadComboBox radcbb = null;
    string hourminute = "";

    string dirtylog = "0";
    if (CntlPanel != null)
    {
      AzzierScreen screen = new AzzierScreen("workorder/wocomp.aspx", "MainForm", CntlPanel.Controls);
      screen.LCID = Session.LCID;
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

      radcbb = CntlPanel.FindControl("cbbcomphour") as RadComboBox;
      if (radcbb != null)
      {
        hourminute = hourminute + radcbb.SelectedValue;
      }
      radcbb = CntlPanel.FindControl("cbbcompminute") as RadComboBox;
      if (radcbb != null)
      {
        hourminute = hourminute + ":" + radcbb.SelectedValue;
      }

      if (nvc["compdate"] != null)
      {
        nvc["compdate"] = nvc["compdate"].ToString() + " " + hourminute;
      }
    }
    else
      nvc = null;

    if (nvc["status"] == null)
      nvc.Add("status", "COMP");
    else if (nvc["status"] == "")
      nvc["status"] = "COMP";
    int statuscode = 300;
    NameValueCollection tempnvc = new NameValueCollection();
    tempnvc.Add("tfield", "wostatus");
    tempnvc.Add("tcode", nvc["status"]);
    ModuleoObject o = new ModuleoObject(Session["Login"].ToString(), "Codes", "Counter");
    string[] counterlist = o.Query(tempnvc);
    if (counterlist.Length > 0)
    {
      o = new ModuleoObject(Session["Login"].ToString(), "Codes", "Counter", counterlist[0]);
      statuscode = Convert.ToInt16(o.ModuleData["Tcode2"]);

    }

    nvc.Set("StatusCode", statuscode.ToString());

    if (nvc["compremark"] != null)
      nvc.Add("Remark", nvc["compremark"]);

    Workorder objWorkorder = new Workorder(Session["Login"].ToString(), "Workorder", "WoNum", nvc["wonum"]);   // use the table name
    NameValueCollection nvcworkorder = objWorkorder.ModuleData;
    for (int i = 0; i < nvc.Count; i++)
    {
      if (nvc.GetKey(i).ToString() != "dirtylog")
      {
        if (nvc[i].ToString() != "")
        {
          if (nvcworkorder[nvc.GetKey(i)] != null)
            nvcworkorder[nvc.GetKey(i)] = nvc[i].ToString();
        }
      }
    }

    AzzierScreen screenmain = new AzzierScreen("workorder/womain.aspx", "MainForm", CntlPanel.Controls);
    string msg = "";
    if (!screenmain.AllMandaotryFilled("Workorder", 300, nvcworkorder, out msg))
    {
      string jscript = "<script type=\"text/javascript\">alert('" + msg + "'); document.location.href='Wocomp.aspx?mode=edit&wonum=" + Server.UrlEncode(nvc["wonum"]) + "';</script>";
      litFrameScript.Text = jscript;
    }
    else  // passed validation for mandatory fields
    {
      RadGrid grdmeter = CntlPanel.FindControl("grdmeter") as RadGrid;
      NameValueCollection meterlist = new NameValueCollection();
      List<string> resetmeterlist = new List<string>();
      if (grdmeter != null)
      {
        foreach (GridEditableItem item in grdmeter.MasterTableView.Items)
        {

          string meterreading = (item["MeterReading"].Controls[0] as TextBox).Text;
          string rolloverflag = (item["Rolloverflag"].Controls[0] as TextBox).Text;
          //string rollover = (item["Rollover"].Controls[0] as TextBox).Text;
          MeterReading Reading = new MeterReading(Session["Login"].ToString(), "MeterReading", "Counter");

          GridDataItem data = (GridDataItem)item;
          string counter = "";
          if (data.SavedOldValues["Counter"] != null)
            counter = data.SavedOldValues["Counter"].ToString();
          if (meterreading != "" && (item["MeterDate"].Controls[0] as RadDatePicker).SelectedDate != null)
          {
            NameValueCollection meter = new NameValueCollection();
            string metername = (item["MeterName"].Controls[0] as TextBox).Text;
            DateTime meterdate = (DateTime)(item["MeterDate"].Controls[0] as RadDatePicker).SelectedDate;

            ServiceMeter servicemeter = new ServiceMeter();
            string returnstr = servicemeter.SaveMeterReading(objWorkorder.ModuleData["equipment"], metername, counter, meterdate.ToString("yyyy/MM/dd"), meterreading, rolloverflag, m_wonum);

            CheckBox chkreset = item["ResetMeter"].Controls[0] as CheckBox;
            if (chkreset != null)
              if (chkreset.Checked)
              {
                Reading.ResetMeter(objWorkorder.ModuleData["equipment"], metername, m_wonum, meterdate.ToString("yyyy/MM/dd"));
              }

            meterlist.Add(metername, meterreading);
          }
        }
      }

      //bool success = objWorkorder.UpdateStatus(nvc);
      bool success = objWorkorder.Complete(nvc, meterlist);
      if (!success)
      {
        string jscript = "<script type=\"text/javascript\">alert('" + objWorkorder.ErrorMessage.Replace("'", "\'") + "'); document.location.href='Wocomp.aspx?mode=edit&wonum=" + Server.UrlEncode(nvc["wonum"]) + "';</script>";
        litFrameScript.Text = jscript;
      }
      else
      {
        //lblErrorMessage.Text = "";
        //lblErrorMessage.Visible = false;
        // increment dirtylog or can't save again
        tbx.Text = (Convert.ToInt32(dirtylog) + 1).ToString();
        // go back to mian screen; if not, just stay in the complete screen
        Response.Redirect("Womain.aspx?mode=edit&wonum=" + Server.UrlEncode(nvc["wonum"]));
      }
    }
  }

  private Boolean ShowToolbarButtonRights(string commandname, NameValueCollection dr)
  {
    //show all buttons first
      if (m_tabname == "Main")
      return true;
    if (dr == null)
      return false;
    Boolean show = false;
    Division d = new Division();
    bool alloweditdiv = d.Editable("'" + m_wodiv + "'");
    switch (commandname)
    {
      case "search":
        show = true;
        break;
      case "lookup":
        if (m_mode == "query")
          show = true;
        break;
      case "newwo":
        if (dr["urAddNew"].ToString() != "")
          if (Convert.ToInt32(dr["urAddNew"].ToString()) != 0)
            show = true;
        break;
      case "autowo":
        if (dr["urAddNew"].ToString() != "")
          if (Convert.ToInt32(dr["urAddNew"].ToString()) != 0)
            show = true;
        break;
      case "save":
        if (m_tabname == "Main")
        {
          if (m_mode == "new")
          {
            if (dr["urAddNew"].ToString() != "")
              if (Convert.ToInt32(dr["urAddNew"].ToString()) != 0)
                show = true;
          }
          if (m_mode == "edit")
          {
            if (dr["urEdit"].ToString() != "")
              if (Convert.ToInt32(dr["urEdit"]) != 0 && alloweditdiv)
                show = true;
          }
          if (m_mode == "duplicate")
          {
            if (dr["urAddNew"].ToString() != "")
              if (Convert.ToInt32(dr["urAddNew"]) != 0)
                show = true;
          }
        }
        break;
      case "complete":
        if (m_tabname == "Complete" && !m_completed && m_approved)
          if (dr["urClose"].ToString() != "")
            if (Convert.ToInt32(dr["urClose"].ToString()) != 0 && alloweditdiv)
              show = true;
        break;
      case "status":
        //if (m_mode == "edit" )
        if (m_tabname == "Main" && m_mode == "edit")
          if (dr["urApprove"].ToString() != "")
            if (Convert.ToDecimal(dr["urApprove"].ToString()) > 0 && alloweditdiv)
              show = true;
        break;
      case "duplicate":
        if (m_mode == "edit")
          if (dr["urAddNew"].ToString() != "")
            if (Convert.ToInt32(dr["urAddNew"].ToString()) > 0)
              show = true;
        break;
      case "print":
        if (m_mode == "edit")
          show = true;
        break;
      case "batchprint":
          show = true;
        break;
      case "print list":
          show = true;
          break;
      case "email":
        if (m_mode == "edit")
          show = true;
        break;
      case "picture":
        show = true;
        break;
      case "linkeddoc":
        show = true;
        break;
      case "calendar":
        show = true;
        break;
      case "updateprocedure":
        if (m_mode == "edit")
          show = true;
        break;
      case "map":
        if (m_mode == "edit")
          show = true;
        break;
      case "modifyequipment":
        if (m_mode == "edit" && m_equipment != "")
          show = true;
        break;
      case "request":
        show = true;
        break;
      case "schedule":
        if (Scheduler.checkSchedulerAccess("woScheduler") == true)
            show = true;
        break;
      case "savequery": 
        show = true;
        break;
      case "timerinfo":
        if (m_mode == "edit" && m_approved && !m_completed)
        show = true;
        break;
      case "timerstart":
      case "timerresume":
      case "timerpause":
      case "timerstop":
        if ((m_mode == "edit") && (m_approved) && (!m_completed))
            if (dr["urACTUALLABOUR"].ToString() != "")
                if (Convert.ToInt32(dr["urACTUALLABOUR"]) > 0)
                    show = true;
        break;
      default:
        break;
    }
    return show;
  }

  private void RetrieveMessage()
  {
    SystemMessage msg = new SystemMessage("workorder/woheader.ascx");
    m_msg = msg.GetSystemMessage();
  }
}