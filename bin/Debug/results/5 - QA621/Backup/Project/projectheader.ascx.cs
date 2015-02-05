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

public partial class project_projectheader : System.Web.UI.UserControl
{
    protected string m_mode;
    private string m_tabname;
    private string m_operationlabel;
    // private string m_empid = "";
    public string m_projectid = "";
    private NameValueCollection m_nvc;
    private AzzierScreen screen;
    //protected RadToolBar toolbar = new RadToolBar();
    protected string linksrvr = "";
    private int m_AllowEdit;
    protected RadComboBox toolbarcombobox;
    private string m_division;
    protected int defaultprintcounter = -1;
    protected NameValueCollection m_msg = new NameValueCollection();
    protected int mainmodul_ScrnSize = 0, tmp_ScrnIconNum = 0, tmp_ActIconNum = 0;
    protected bool isReadytoComplete = false;

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
            m_projectid = m_nvc["projectid"];
            m_division = m_nvc["division"];
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        RetrieveMessage();
        toolbarcombobox = new RadComboBox();

        //mainmodul_ScrnSize = 260;// Convert.ToInt32(Session["ScreenWidth"].ToString());
        mainmodul_ScrnSize = Convert.ToInt32(Session["ScreenWidth"].ToString());
        mainmodul_ScrnSize = (int)Math.Floor(mainmodul_ScrnSize * 0.67) - 100;
        //mainmodul_ScrnSize = 260;// 500;// 260;// 500;// (int)Math.Floor(mainmodul_ScrnSize * 0.67) - 32;
        decimal t_iconnum = mainmodul_ScrnSize / 50;
        tmp_ScrnIconNum = (int)Math.Floor(t_iconnum);
        //c_005 = false; c_006 = false; c_007 = false; c_008 = false; c_009 = false; c_010 = false; c_011 = false; c_012 = false;

        Session.LCID = Convert.ToInt32(Session["LCID"]);
        string connstring = Application["ConnString"].ToString();
        defaultprintcounter = AzzierPrintForm.GetDefaultReport("Project");
        litFrameScript.Text = "";
        litDateLCIDScript.Text = "<script type=\"text/javascript\">var g_LCID = " + Session.LCID.ToString() + ";</script>";
        
        int statuscode = 0;

        if (m_nvc["statuscode"] != null)
        {
            if (m_nvc["statuscode"] != "")
            {
                statuscode = Convert.ToInt32(m_nvc["statuscode"]);
            }
        }
        if (m_tabname == "Main")
        {
            tabhlkMain.NavigateUrl = "javascript:void(null)";
            tabimgMain.ImageUrl = "../images/tabbutton_down.png";
            tablblMain.CssClass = "toptabover";
        }
        else
        {
          tabhlkMain.NavigateUrl = "projectmain.aspx?mode=edit&projectid=" + m_nvc["projectid"];
          tabhlkMain.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgMain','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblMain','toptabover')");
          tabhlkMain.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgMain','../images/tabbutton.png');classChangeover('ucHeader1_tablblMain','toptabout')");
        }

        if (m_tabname == "Complete")
        {
          tabhlkComplete.NavigateUrl = "javascript:void(null)";
          tabimgComplete.ImageUrl = "../images/tabbutton_down.png";
          tablblComplete.CssClass = "toptabover";
        }
        else
        {
          if ((m_mode == "edit") && (statuscode>=200) && (statuscode <300))
          {
            tabhlkComplete.NavigateUrl = "projectcompletion.aspx?projectid=" + m_nvc["projectid"];
            tabhlkComplete.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgComplete','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblComplete','toptabover')");
            tabhlkComplete.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgComplete','../images/tabbutton.png');classChangeover('ucHeader1_tablblComplete','toptabout')");
          }
          else
          {
            tabhlkComplete.Enabled = false;
            tablblComplete.CssClass = "toptabinactive";
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
                tabhlkHistory.NavigateUrl = "projecthistory.aspx?projectid=" + m_nvc["projectid"];
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
            AzzierHeader.InitToolBar(toolbar, "Project", ShowToolbarButtonRights);
        }
        litOperation.Text = m_operationlabel;
        AzzierHeader.CreateLinksDropDown(plhLinkDoc, "Project", m_projectid, out linksrvr);
        AzzierHeader.CreateReportsDropDown(plhReports, "Project");
        AzzierHeader.CreateQueryDropDown(plhQuery, "Project");

        if (Page.IsPostBack)
        {
            if (Request.Form["__EVENTTARGET"] == "lookup")
            {
                LookUpClicked();
            }
        }

        if ((m_mode == "edit") && (m_projectid != ""))
        {
            Project tempobj = new Project(Session["Login"].ToString(), "Projects", "projectid", m_projectid);
            isReadytoComplete = tempobj.IsReadytoComplete();
        }
        
    }

    protected void LookUpClicked()
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        if (CntlPanel != null)
        {
            screen = new AzzierScreen("project/projectmain.aspx", "MainForm", CntlPanel.Controls);
            screen.LCID = Session.LCID;
            nvc = screen.CollectFormValues("projects", true);  // use the view name
            //m_division = nvc["division"];
        }
        else
            nvc = null;
        Project obj = new Project(Session["Login"].ToString(), "Projects", "projectid");   // use the view name
        string wherestring = "", linqwherestring = "";
        obj.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
        string jscript = "<script type=\"text/javascript\">";
        jscript += "var wherestring = '" + Server.UrlEncode(linqwherestring) + "'; var obj=parent.controlpanel.document.location.href='projectpanel.aspx?showlist=queryresult&wherestring='+wherestring;";
        jscript += "</script>";
        litFrameScript.Text = jscript;
    }


    private Boolean ShowToolbarButtonRights(string commandname, NameValueCollection dr)
    {
        if (dr == null)
            return false;
        Boolean show = false;
        Division d = new Division();
        bool alloweditdiv = d.Editable("'" + m_division + "'");
        switch (commandname)
        {
          case "search":
            show = true;
            break;
          case "lookup":
            if (m_mode == "query")
              show = true;
            break;
          case "new":
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
              if (m_mode == "edit" )
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
            if (m_tabname == "Main" && m_mode == "edit")
              if (dr["urApprove"].ToString() != "")
                if (Convert.ToDecimal(dr["urApprove"].ToString()) > 0 && alloweditdiv)
                  show = true;
            break;
          case "delete":
            if (m_mode == "edit" && m_tabname == "Main")
                if (dr["urDelete"].ToString() != "")
                    if (Convert.ToInt32(dr["urDelete"].ToString()) > 0)
                        show = true;
            break;
          case "complete":
            if ((m_mode == "edit") && (m_tabname == "Complete"))
                if (dr["urEdit"].ToString() != "")
                    if (Convert.ToInt32(dr["urEdit"].ToString()) > 0)
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
          case "linkeddoc":
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
        SystemMessage msg = new SystemMessage("labour/labourheader.ascx");//"workorder/woheader.ascx");
        m_msg = msg.GetSystemMessage();
        //msg.SetJsMessage(litMessage);
    }

}