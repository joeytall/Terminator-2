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

public partial class workrequest_wrheader : System.Web.UI.UserControl
{
    protected string m_mode;
    private string m_tabname;
    private string m_operationlabel;
    private bool m_assigned = false;
    private bool m_approved = false;
    protected string m_wrnum = "";
    protected string m_equipment = "";
    protected string m_location = "";
    private NameValueCollection m_nvc;
    private AzzierScreen screen;
    //    protected RadToolBar toolbar = new RadToolBar();
    protected string linksrvr = "";
    private int m_AllowEdit;
    private string m_wrdiv;
    protected RadComboBox toolbarcombobox;

    protected NameValueCollection m_msg = new NameValueCollection();

    protected RadComboBox tmp_rcb;
    protected RadComboBoxItem tmpcbi;
    protected int mainmodul_ScrnSize = 0, tmp_ScrnIconNum = 0, tmp_ActIconNum = 0;
    protected int defaultprintcounter = -1;
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
            if (m_nvc["statuscode"].ToString() != "")
            {
                if (Convert.ToInt16(m_nvc["statuscode"]) >= 300)
                  m_assigned = true;
                if (Convert.ToInt16(m_nvc["statuscode"]) >= 200)
                  m_approved = true;
            }
            if (m_nvc["equipment"].ToString() != "")
            {
                m_equipment = m_nvc["equipment"].ToString();
            }
            if (m_nvc["location"].ToString() != "")
            {
              m_location = m_nvc["location"].ToString();
            }

            m_wrnum = m_nvc["wrnum"].ToString();
            m_wrdiv = m_nvc["division"].ToString();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        RetrieveMessage();
        //if (!Page.IsPostBack)
        //{
        tmp_rcb = new RadComboBox();
        toolbarcombobox = new RadComboBox();

        
        mainmodul_ScrnSize = Convert.ToInt32(Session["ScreenWidth"].ToString());
        mainmodul_ScrnSize = (int)Math.Floor(mainmodul_ScrnSize * 0.67) - 32;
        decimal t_iconnum = mainmodul_ScrnSize / 50;
        tmp_ScrnIconNum = (int)Math.Floor(t_iconnum);

        Session.LCID = Convert.ToInt32(Session["LCID"]);
        string connstring = Application["ConnString"].ToString();

        litFrameScript.Text = "";
        litDateLCIDScript.Text = "<script type=\"text/javascript\">var g_LCID = " + Session.LCID.ToString() + ";</script>";

        defaultprintcounter = AzzierPrintForm.GetDefaultReport("WorkRequest");

        if (m_tabname == "Main")
        {
            tabhlkMain.NavigateUrl = "javascript:void(null)";
            tabimgMain.ImageUrl = "../images/tabbutton_down.png";
            tablblMain.CssClass = "toptabover";
        }
        else
        {
            tabhlkMain.NavigateUrl = "Wrmain.aspx?mode=edit&wrnum=" + m_nvc["wrnum"];
            tabhlkMain.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgMain','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblMain','toptabover')");
            tabhlkMain.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgMain','../images/tabbutton.png');classChangeover('ucHeader1_tablblMain','toptabout')");
        }
        /*
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
                tabhlkAccounts.NavigateUrl = "Wraccounts.aspx?wrnum=" + m_nvc["wrnum"];
                tabhlkAccounts.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgAccounts','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblAccounts','toptabover')");
                tabhlkAccounts.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgAccounts','../images/tabbutton.png');classChangeover('ucHeader1_tablblAccounts','toptabout')");
            }
            else
            {
                tabhlkAccounts.Enabled = false;
                tablblAccounts.CssClass = "toptabinactive";
            }
        }
      */
        if (m_tabname == "Work Order")
        {
          tabhlkWorkOrder.NavigateUrl = "javascript:void(null)";
          tabimgWorkOrder.ImageUrl = "../images/tabbutton_down.png";
          tablblWorkOrder.CssClass = "toptabover";
        }
        else
        {
          if (m_mode == "edit")
          {
            tabhlkWorkOrder.NavigateUrl = "relatewo.aspx?wrnum=" + m_nvc["wrnum"];
            tabhlkWorkOrder.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgWorkOrder','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblWorkOrder','toptabover')");
            tabhlkWorkOrder.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgWorkOrder','../images/tabbutton.png');classChangeover('ucHeader1_tablblWorkOrder','toptabout')");
          }
          else
          {
            tabhlkWorkOrder.Enabled = false;
            tablblWorkOrder.CssClass = "toptabinactive";
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
                tabhlkHistory.NavigateUrl = "Wrhistory.aspx?wrnum=" + m_nvc["wrnum"];
                tabhlkHistory.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgHistory','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblHistory','toptabover')");
                tabhlkHistory.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgHistory','../images/tabbutton.png');classChangeover('ucHeader1_tablblHistory','toptabout')");
            }
            else
            {
                tabhlkHistory.Enabled = false;
                tablblHistory.CssClass = "toptabinactive";
            }
        }
      
        if (!Page.IsPostBack)
        {
            AzzierHeader.InitToolBar(toolbar, "WorkRequest", ShowToolbarButtonRights);
        }
        
        litOperation.Text = m_operationlabel;
        AzzierHeader.CreateLinksDropDown(plhLinkDoc, "WorkRequest", m_wrnum, out linksrvr);
        AzzierHeader.CreateReportsDropDown(plhReports, "WorkRequest");
        AzzierHeader.CreateQueryDropDown(plhQuery, "WorkRequest");


        if (Page.IsPostBack)
        {
            if (Request.Form["__EVENTTARGET"] == "duplicate")
            {
                Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
                TextBox tbx = null;
                tbx = CntlPanel.FindControl("txtwrnum") as TextBox;


                string wrnum = m_wrnum;
                string newwrnum = tbx.Text;
                int estimate =Convert.ToInt32(  Request["__EVENTARGUMENT"].ToString());

                WorkRequest objwr = new WorkRequest(Session["Login"].ToString(), "WorkRequest", "WRNum");
                bool success = objwr.CopyFrom(wrnum, false, newwrnum);
                if (success)
                {
                    NameValueCollection nvc = objwr.ModuleData;
                    newwrnum = nvc["wonum"].ToString().ToUpper();

                    Labour l = new Labour(Session["Login"].ToString(), "WOLabour", "Counter");
                    l.CopyLabour("workrequest", wrnum, "workrequest", newwrnum,estimate);
                    Service s = new Service(Session["Login"].ToString(), "WOService", "Counter");
                    s.CopyService("workrequest", wrnum, "workrequest", newwrnum,estimate);
                    Material m = new Material(Session["Login"].ToString(), "WOMaterial", "Counter");
                    m.CopyMaterial("workrequest", wrnum, "workrequest", newwrnum,estimate);
                    Tool t = new Tool(Session["Login"].ToString(), "WOTools", "Counter");
                    t.CopyTools("workrequest", wrnum, "workrequest", newwrnum,estimate);
                    Task task = new Task(Session["Login"].ToString(), "WOTasks", "Counter");
                    task.CopyTasks("workrequest", wrnum, "workrequest", newwrnum,estimate);

                    litScript.Text = "<script type=\"text/javascript\">document.location.href='Wrmain.aspx?mode=edit&wrnum=" + newwrnum + "';</script>";
                }
                else
                    litScript.Text = objwr.ErrorMessage;

            }
            
            if (Request.Form["__EVENTTARGET"] == "lookup")
            {
                lbnLookUpWorkRequest_Click(null, null);
            }
        }
    }

    protected void lbnLookUpWorkRequest_Click(object sender, EventArgs e)
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        if (CntlPanel != null)
        {
            screen = new AzzierScreen("workrequest/wrmain.aspx", "MainForm", CntlPanel.Controls);
            screen.LCID = Session.LCID;
            nvc = screen.CollectFormValues("workrequest", true);  // use the view name
        }
        else
            nvc = null;
        
        Workorder objWorkorder = new Workorder(Session["Login"].ToString(), "workrequest", "Wrnum");   // use the view name

        string wherestring = "", linqwherestring = "";
        objWorkorder.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);

        string jscript = "";
        jscript = "<script>";
        jscript += "var wherestring = '" + Server.UrlEncode(linqwherestring) + "'; var obj=parent.controlpanel.document.location.href='wrpanel.aspx?showlist=queryresult&wherestring='+wherestring;";
        jscript += "</script>";
        litFrameScript.Text = jscript;

        /*
        string[] wonums = objWorkorder.LinqQuery(nvc, ref wherestring, ref linqwherestring);
        string jscript = "<script type=\"text/javascript\">";
        int len = wonums.Length;
        if (len > 0)
            jscript += "var wherestring = '" + Server.UrlEncode(linqwherestring) + "'; document.location.href='Womain.aspx?mode=edit&wonum=" + Server.UrlEncode(wonums[0]) + "';var obj=parent.controlpanel.document.location.href='Wopanel.aspx?showlist=queryresult&wherestring='+wherestring;";
        else
            jscript += "document.location.href='Womain.aspx';";
        jscript += "</script>";
        litFrameScript.Text = jscript;
         * */
    }

    //protected void lbnSaveWorkOrder_Click(object sender, EventArgs e)
    //{
    //    SaveWO();
    //}

    #region  create and save workorder

    private void SaveWR() //
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        TextBox tbx = null;
        string dirtylog = "0";
        if (CntlPanel != null)
        {
            screen = new AzzierScreen("workrequest/wrmain.aspx", "MainForm", CntlPanel.Controls);
            screen.LCID = Session.LCID;
            nvc = screen.CollectFormValues("workrequest", false);  // use the table name
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

        WorkRequest objwr;
        bool success = false;
        if (m_mode == "new")
        {
            objwr = new WorkRequest(Session["Login"].ToString(), "WorkRequest", "WrNum");   // use the table name
            success = objwr.Create(nvc);
            if (success)
            {
                string wonum = nvc["wonum"].ToString().ToUpper();
                string[] wonums = { wonum };
            }
        }
        else if (m_mode == "edit")
        {
            string wo = nvc["wonum"];
            objwr = new WorkRequest(Session["Login"].ToString(), "WorkRequest", "WrNum", wo);   // use the table name
            success = objwr.Update(nvc);
        }
        else
            objwr = new WorkRequest(Session["Login"].ToString(), "WorkRequest", "WrNum");

        if (!success)
        {   string jscript = "<script>alert(\"" + objwr.ErrorMessage.Replace("\r\n", "") + "\");</script>";
            litFrameScript.Text = jscript;
        }
        else
        {
            if (m_mode == "new")
            {
                if (nvc["procnum"] != "")
                {
                    Labour l = new Labour(Session["Login"].ToString(), "WOLabour", "Counter");
                    l.CopyLabour("procedure", nvc["procnum"], "workrequest", nvc["wrnum"]);
                    Service s = new Service(Session["Login"].ToString(), "WOService", "Counter");
                    s.CopyService("procedure", nvc["procnum"], "workrequest", nvc["wrnum"]);
                    Material m = new Material(Session["Login"].ToString(), "WOMaterial", "Counter");
                    m.CopyMaterial("procedure", nvc["procnum"], "workrequest", nvc["wrnum"]);
                    Tool t = new Tool(Session["Login"].ToString(), "WOTools", "Counter");
                    t.CopyTools("procedure", nvc["procnum"], "workrequest", nvc["wrnum"]);
                    Task task = new Task(Session["Login"].ToString(), "WOTasks", "Counter");
                    task.CopyTasks("procedure", nvc["procnum"], "workrequest", nvc["wrnum"]);
                }
                Response.Redirect("Wrmain.aspx?mode=edit&wrnum=" + Server.UrlEncode(nvc["wrnum"]));
            }
            else if (m_mode == "edit")
            {
                HiddenField h = Page.FindControl("hidprocnum") as HiddenField;
                string oldproc = h.Value;
                if ((nvc["procnum"].ToString() != "") && (nvc["procnum"].ToString() != oldproc))
                {
                    Labour l = new Labour(Session["Login"].ToString(), "WOLabour", "Counter");
                    l.DeleteAllLabour("workrequest", nvc["wrnum"], 1);
                    l.CopyLabour("procedure", nvc["procnum"], "workrequest", nvc["wrnum"]);
                    Service s = new Service(Session["Login"].ToString(), "WOService", "Counter");
                    s.DeleteAllService("workrequest", nvc["wrnum"], 1);
                    s.CopyService("procedure", nvc["procnum"], "workrequest", nvc["wrnum"]);
                    Material m = new Material(Session["Login"].ToString(), "WOMaterial", "Counter");
                    m.DeleteAllMaterial("workrequest", nvc["wrnum"], 1);
                    m.CopyMaterial("procedure", nvc["procnum"], "workrequest", nvc["wrnum"]);
                    Tool t = new Tool(Session["Login"].ToString(), "WOTools", "Counter");
                    t.DeleteAllTools("workrequest", nvc["wrnum"], 1);
                    t.CopyTools("procedure", nvc["procnum"], "workrequest", nvc["wrnum"]);
                    Task task = new Task(Session["Login"].ToString(), "WOTasks", "Counter");
                    task.DeleteAllTask("workrequest", nvc["wrnum"], 1);
                    task.CopyTasks("procedure", nvc["procnum"], "workrequest", nvc["wrnum"]);
                    h.Value = nvc["procnum"];
                    RadGrid g = Page.FindControl("grdlabour") as RadGrid;
                    g.Rebind();
                    g = Page.FindControl("grdmaterial") as RadGrid;
                    g.Rebind();
                    g = Page.FindControl("grdtasks") as RadGrid;
                    g.Rebind();
                    g = Page.FindControl("grdtools") as RadGrid;
                    g.Rebind();
                    g = Page.FindControl("grdservice") as RadGrid;
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
        bool alloweditdiv = d.Editable("'" + m_wrdiv + "'");
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
                    if (m_mode == "edit" && !m_assigned )
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
            case "status":
                //if (m_mode == "edit" )
                if (m_tabname == "Main" && m_mode == "edit" && !m_assigned)
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
            case "map":
                if (m_mode == "edit")
                {
                    show = true;
                }
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