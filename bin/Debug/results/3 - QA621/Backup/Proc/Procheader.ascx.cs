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

public partial class Proc_Procheader : System.Web.UI.UserControl
{
    protected string m_mode;
    private string m_tabname;
    private string m_operationlabel;
    private bool m_completed = false;
    private bool m_approved = false;
    protected string m_procnum = "";
    protected string m_equipment = "";
    private NameValueCollection m_nvc;
    private AzzierScreen screen;
    //    protected RadToolBar toolbar = new RadToolBar();
    protected string linksrvr = "";
    private int m_AllowEdit;
    private string m_wodiv;
    protected RadComboBox toolbarcombobox;
    protected int defaultprintcounter = -1;
    protected NameValueCollection m_msg = new NameValueCollection();

    protected RadComboBox tmp_rcb;
    protected RadComboBoxItem tmpcbi;
    protected int mainmodul_ScrnSize = 0, tmp_ScrnIconNum = 0, tmp_ActIconNum = 0;

    protected string m_masterrec;

    //private bool c_005, c_006, c_007, c_008, c_009, c_010, c_011, c_012;
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
            /*if (m_nvc["statuscode"].ToString() != "")
            {
                if ((Convert.ToInt16(m_nvc["statuscode"]) >= 200) && (Convert.ToInt16(m_nvc["statuscode"]) < 300))
                    m_approved = true;
                if (Convert.ToInt16(m_nvc["statuscode"]) >= 300)
                    m_completed = true;
            }*/
            if (m_nvc["equipment"].ToString() != "")
            {
                m_equipment = m_nvc["equipment"].ToString();
            }
            m_procnum = m_nvc["procnum"].ToString();
            m_wodiv = m_nvc["division"].ToString();
            m_masterrec = m_nvc["MasterRec"].ToString();

        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        RetrieveMessage();
      //if (!Page.IsPostBack)
      //{
        tmp_rcb = new RadComboBox();
        toolbarcombobox = new RadComboBox();

        //mainmodul_ScrnSize = 260;// Convert.ToInt32(Session["ScreenWidth"].ToString());
        mainmodul_ScrnSize = Convert.ToInt32(Session["ScreenWidth"].ToString());
        mainmodul_ScrnSize = (int)Math.Floor(mainmodul_ScrnSize * 0.67) - 32;
        //mainmodul_ScrnSize = 260;// 500;// 260;// 500;// (int)Math.Floor(mainmodul_ScrnSize * 0.67) - 32;
        decimal t_iconnum = mainmodul_ScrnSize / 50;
        tmp_ScrnIconNum = (int)Math.Floor(t_iconnum);
        //c_005 = false; c_006 = false; c_007 = false; c_008 = false; c_009 = false; c_010 = false; c_011 = false; c_012 = false;

        Session.LCID = Convert.ToInt32(Session["LCID"]);
        string connstring = Application["ConnString"].ToString();

        litFrameScript.Text = "";
        litDateLCIDScript.Text = "<script type=\"text/javascript\">var g_LCID = " + Session.LCID.ToString() + ";</script>";

        if (m_tabname == "Main")
        {
          tabhlkMain.NavigateUrl = "javascript:void(null)";
          tabimgMain.ImageUrl = "../images/tabbutton_down.png";
          tablblMain.CssClass = "toptabover";
        }
        else
        {
          tabhlkMain.NavigateUrl = "Procmain.aspx?mode=edit&procnum=" + m_nvc["procnum"];
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
            tabhlkAccounts.NavigateUrl = "procaccounts.aspx?procnum=" + m_nvc["procnum"];
            tabhlkAccounts.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgAccounts','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblAccounts','toptabover')");
            tabhlkAccounts.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgAccounts','../images/tabbutton.png');classChangeover('ucHeader1_tablblAccounts','toptabout')");
          }
          else
          {
            tabhlkAccounts.Enabled = false;
            tablblAccounts.CssClass = "toptabinactive";
          }
        }

        if (!Page.IsPostBack)
        {
          AzzierHeader.InitToolBar(toolbar, "procedure", ShowToolbarButtonRights);
        }
        litOperation.Text = m_operationlabel;
        AzzierHeader.CreateLinksDropDown(plhLinkDoc, "Procedure", m_procnum, out linksrvr);
        AzzierHeader.CreateReportsDropDown(plhReports, "Procedure");
        AzzierHeader.CreateQueryDropDown(plhQuery, "Procedure");

        defaultprintcounter = AzzierPrintForm.GetDefaultReport("Procedure");

        if (Page.IsPostBack)
        {
          if (Request.Form["__EVENTTARGET"] == "save")
          {
            SaveWO();
          }
          if (Request.Form["__EVENTTARGET"] == "lookup")
          {
            lbnLookUpWorkOrder_Click(null, null);
          }
          if (Request.Form["__EVENTTARGET"] == "delete")
          {
            DeleteProc();
          }
        }
      //}
    }

    protected void DeleteProc()
    {
      Procedure objProcedure = new Procedure(Session["Login"].ToString(), "procedures", "procnum", m_procnum);
      objProcedure.Delete();     
      string jscript = "<script type=\"text/javascript\">";
      jscript += "setTimeout(parent.controlpanel.RefreshPanelGridsOnDelete(), 0);document.location.href='procmain.aspx';";
      jscript += "</script>";
      litFrameScript.Text = jscript;
    }

    protected void lbnLookUpWorkOrder_Click(object sender, EventArgs e)
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        if (CntlPanel != null)
        {
            screen = new AzzierScreen("Proc/procmain.aspx", "MainForm", CntlPanel.Controls);
            screen.LCID = Session.LCID;
            nvc = screen.CollectFormValues("procedures", true);  // use the view name
        }
        else
            nvc = null;
        Procedure objProcedure = new Procedure(Session["Login"].ToString(), "procedures", "procnum");   // use the view name
        string wherestring = "";
        string wherestringlinq = "";
        //string[] procnums = objProcedure.LinqQuery(nvc, ref wherestring, ref wherestringlinq);
        objProcedure.CreateLinqCondition(nvc, ref wherestring, ref wherestringlinq);
        string jscript = "<script type=\"text/javascript\">";
        jscript += "var wherestring = '" + Server.UrlEncode(wherestringlinq) + "'; var obj=parent.controlpanel.document.location.href='procpanel.aspx?showlist=queryresult&wherestring='+wherestring;";
        jscript += "</script>";
        litFrameScript.Text = jscript;
    }

    #region  create and save workorder

    private void SaveWO() //
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        TextBox tbx = null;
        string dirtylog = "0";
        if (CntlPanel != null)
        {
            screen = new AzzierScreen("proc/procmain.aspx", "MainForm", CntlPanel.Controls);
            screen.LCID = Session.LCID;
            nvc = screen.CollectFormValues("procedures", false);  // use the table name
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

        Procedure objProcedure;
        bool success = false;
        if (m_mode == "new")
        {
            objProcedure = new Procedure(Session["Login"].ToString(), "procedures", "procNum");   // use the table name
            success = objProcedure.Create(nvc);
            if (success)
            {
                string procnum = nvc["procnum"].ToString().ToUpper();
                string[] procnums = { procnum };
            }
        }
        else if (m_mode == "edit")
        {
            string wo = nvc["wonum"];
            objProcedure = new Procedure(Session["Login"].ToString(), "procedures", "procNum", wo);   // use the table name
            success = objProcedure.Update(nvc);
        }
        else
            objProcedure = new Procedure(Session["Login"].ToString(), "procedures", "procNum");

        if (!success)
        {
            //string jscript = "<script type=\"text/javascript\">alert(\"" + objWorkorder.ErrorMessage.Replace("'", "\'") + "\"); document.location.href='Womain.aspx?mode=edit&wonum=" + Server.UrlEncode(nvc["wonum"]) + "';</script>";
            //string jscript = "<script>alert(\"" + objWorkorder.ErrorMessage.Replace("\r\n","") + "\"); document.location.href='Womain.aspx?mode=edit&wonum=" + Server.UrlEncode(nvc["wonum"]) + "';</script>";
            string jscript = "<script>alert(\"" + objProcedure.ErrorMessage.Replace("\r\n", "") + "\");</script>";
            litFrameScript.Text = jscript;
        }
        else
        {
            if (m_mode == "new")
            {
                if (nvc["procnum"] != "")
                {
                    /*Labour l = new Labour(Session["Login"].ToString(), "WOLabour", "Counter");
                    l.CopyLabour("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                    Service s = new Service(Session["Login"].ToString(), "WOService", "Counter");
                    s.CopyService("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                    Material m = new Material(Session["Login"].ToString(), "WOMaterial", "Counter");
                    m.CopyMaterial("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                    Tool t = new Tool(Session["Login"].ToString(), "WOTools", "Counter");
                    t.CopyTools("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                    Task task = new Task(Session["Login"].ToString(), "WOTasks", "Counter");
                    task.CopyTasks("procedure", nvc["procnum"], "workorder", nvc["wonum"]);*/
                }
                Response.Redirect("procmain.aspx?mode=edit&procnum=" + Server.UrlEncode(nvc["procnum"]));
            }
            else if (m_mode == "edit")
            {
                HiddenField h = Page.FindControl("hidprocnum") as HiddenField;
                string oldproc = h.Value;
                if ((nvc["procnum"].ToString() != "") && (nvc["procnum"].ToString() != oldproc))
                {
                    /*Labour l = new Labour(Session["Login"].ToString(), "WOLabour", "Counter");
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
                    task.CopyTasks("procedure", nvc["procnum"], "workorder", nvc["wonum"]);*/
                    h.Value = nvc["procnum"];
                    RadGrid g = Page.FindControl("grdproclabourest") as RadGrid;
                    g.Rebind();
                    g = Page.FindControl("grdprocmatlest") as RadGrid;
                    g.Rebind();
                    g = Page.FindControl("grdproctasksest") as RadGrid;
                    g.Rebind();
                    g = Page.FindControl("grdproctoolsest") as RadGrid;
                    g.Rebind();
                    g = Page.FindControl("grdprocserviceest") as RadGrid;
                    g.Rebind();

                }
                tbx.Text = (Convert.ToInt32(dirtylog) + 1).ToString();
            }
        }
    }

    #endregion


    private Boolean ShowToolbarButtonRights(string commandname, NameValueCollection dr)
    {
        if (dr == null)
            return false;
        Boolean show = false;
        Division d = new Division();
        bool alloweditdiv = d.Editable("'" + m_wodiv + "'");
        bool allowmaster = false;
        if (dr["urMaster"].ToString() != "")
          if (Convert.ToInt32(dr["urMaster"].ToString()) != 0)
            allowmaster = true;
        switch (commandname)
        {
            case "search":
                show = true;
                break;
            case "lookup":
                if (m_mode == "query")
                    show = true;
                break;
            case "newproc":
                if (dr["urAddNew"].ToString() != "")
                    if (Convert.ToInt32(dr["urAddNew"].ToString()) != 0)
                        show = true;
                break;
            case "autoproc":
                if (dr["urAddNew"].ToString() != "")
                    if (Convert.ToInt32(dr["urAddNew"].ToString()) != 0)
                        show = true;
                break;
            case "save":
                if (m_masterrec == "1" && allowmaster == false)
                {
                  show = false;
                }
                else
                {
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
                  }
                }
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
                if (m_mode == "edit")
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
            case "delete":
                if (m_masterrec == "1" && allowmaster == false)
                {
                  show = false;
                }
                else
                {
                  if (m_mode == "edit")
                    if (dr["urDelete"].ToString() != "")
                      if (Convert.ToInt32(dr["urDelete"].ToString()) > 0)
                        show = true;
                }
                break;
            case "tasklibrary":
                  show = true;
                break;

            default:
                break;
        }
        return show;
    }

    private void RetrieveMessage()
    {
        SystemMessage msg = new SystemMessage("proc/procheader.aspx");
        m_msg = msg.GetSystemMessage();
        //msg.SetJsMessage(litMessage);
    }

}