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

public partial class pm_pmheader : System.Web.UI.UserControl
{
    protected string m_mode;
    private string m_tabname;
    private string m_operationlabel;
    // private string m_empid = "";
    public string m_pmnum = "";
    private NameValueCollection m_nvc;
    private AzzierScreen screen;
    protected string linksrvr = "";
    private int m_AllowEdit;
    protected RadComboBox toolbarcombobox;
    private string m_division;
    protected int m_masterrec;
    protected int simplepm = 1;
    protected int defaultprintcounter = -1;

    protected NameValueCollection m_msg = new NameValueCollection();
    protected int mainmodul_ScrnSize = 0, tmp_ScrnIconNum = 0, tmp_ActIconNum = 0;
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
            m_pmnum = m_nvc["pmnum"];
            m_division = m_nvc["division"];
        }        
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        RetrieveMessage();
        toolbarcombobox = new RadComboBox();

        mainmodul_ScrnSize = Convert.ToInt32(Session["ScreenWidth"].ToString());
        mainmodul_ScrnSize = (int)Math.Floor(mainmodul_ScrnSize * 0.67) - 32;
        decimal t_iconnum = mainmodul_ScrnSize / 50;
        tmp_ScrnIconNum = (int)Math.Floor(t_iconnum);

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
            tabhlkMain.NavigateUrl = "pmmain.aspx?mode=edit&pmnum=" + m_nvc["pmnum"];
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
                tabhlkAccounts.NavigateUrl = "pmaccounts.aspx?pmnum=" + m_pmnum;
                tabhlkAccounts.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgAccounts','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblAccounts','toptabover')");
                tabhlkAccounts.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgAccounts','../images/tabbutton.png');classChangeover('ucHeader1_tablblAccounts','toptabout')");
            }
            else
            {
                tabhlkAccounts.Enabled = false;
                tablblAccounts.CssClass = "toptabinactive";
            }
        }

        if (m_tabname == "WO")
        {
          tabhlkWO.NavigateUrl = "javascript:void(null)";
          tabimgWO.ImageUrl = "../images/tabbutton_down.png";
          tablblWO.CssClass = "toptabover";
        }
        else
        {
          if (m_mode == "edit")
          {
            tabhlkWO.NavigateUrl = "pmwolist.aspx?pmnum=" + m_pmnum;
            tabhlkWO.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgWO','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblWO','toptabover')");
            tabhlkWO.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgWO','../images/tabbutton.png');classChangeover('ucHeader1_tablblWO','toptabout')");
          }
          else
          {
            tabhlkWO.Enabled = false;
            tablblWO.CssClass = "toptabinactive";
          }
        }


        if (!Page.IsPostBack)
        {
          AzzierHeader.InitToolBar(toolbar, "PM", ShowToolbarButtonRights);
        }

        litOperation.Text = m_operationlabel;
        AzzierHeader.CreateLinksDropDown(plhLinkDoc, "PM", m_pmnum, out linksrvr);
        AzzierHeader.CreateReportsDropDown(plhReports, "PM");
        AzzierHeader.CreateQueryDropDown(plhQuery, "PM");

        defaultprintcounter = AzzierPrintForm.GetDefaultReport("PM");

        if (Page.IsPostBack)
        {
          if (Request.Form["__EVENTTARGET"] == "lookup")
          {
            LookUpPM();
          }
          if (Request.Form["__EVENTTARGET"] == "delete")
          {
            DeletePM();
          }
          

        }
        if (m_nvc["PMInterval"] != "" && m_nvc["MeterInterval"] != "")
          simplepm = 0;
        if (m_nvc["MeterInterval2"] != "" && m_nvc["MeterInterval"] != "")
          simplepm = 0;
    }

    public void DeletePM()
    {
      PM objpm = new PM(Session["Login"].ToString(), "PM", "PMNum", m_pmnum);
      if (objpm.Delete())
      {
        string jscript = "<script type=\"text/javascript\">";
        jscript += "setTimeout(parent.controlpanel.RefreshPanelGridsOnDelete(), 0); document.location.href='pmmain.aspx'";
        jscript += "</script>";
        litFrameScript.Text = jscript;
      }
    }

    public void LookUpPM()
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        if (CntlPanel != null)
        {
            screen = new AzzierScreen("pm/pmmain.aspx", "MainForm", CntlPanel.Controls);
            screen.LCID = Session.LCID;
            nvc = screen.CollectFormValues("PM",true);  // use the view name
            //m_division = nvc["division"];

            Validation v = new Validation();
            string pmnum = nvc["PMNum"].ToString() ?? "";
            if (pmnum.Length > 1)
            {
              if (v.SpecialStrValidator(pmnum))
              {
                nvc = new NameValueCollection();
                nvc.Add("PMNum", pmnum);
              }
            }
        }
        else
            nvc = null;


        PM obj = new PM(Session["Login"].ToString(), "PM", "PMNum");   // use the view name
        string wherestring = "",linqwherestring="";
        //string[] equipments = obj.Query(nvc, ref wherestring);
        //string[] pms = obj.LinqQuery(nvc, ref wherestring,ref linqwherestring);
        obj.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
        string jscript = "<script type=\"text/javascript\">";
        
            // jscript += "var wherestring = '" + Server.UrlEncode(wherestring) + "'; document.location.href='equipmentmain.aspx?mode=edit&equipment=" + Server.UrlEncode(equipments[0]) + "';var obj=parent.controlpanel.document.location.href='equipmentpanel.aspx?showlist=queryresult&wherestring='+wherestring;";
        jscript += "var wherestring = '" + Server.UrlEncode(linqwherestring) + "';var obj=parent.controlpanel.document.location.href='pmpanel.aspx?showlist=queryresult&wherestring='+wherestring;";
        
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
            case "autopm":
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
            case "delete":
                if (m_tabname == "Main" && m_mode == "edit")
                {
                  if (dr["urDelete"].ToString() != "")
                    if (Convert.ToInt32(dr["urDelete"]) != 0 && alloweditdiv)
                      show = true;
                }
                break;
            case "generate":
                if (dr["urGenerate"].ToString() != "")
                  if (Convert.ToInt32(dr["urGenerate"].ToString()) > 0)
                    show = true;
                break;
            case "nested":
                if (m_mode == "edit")
                  if (dr["urEdit"].ToString() != "")
                    if (Convert.ToInt32(dr["urEdit"].ToString()) > 0)
                      show = true;
                break;
            case "sequence":
                if (m_mode == "edit")
                  if (dr["urEdit"].ToString() != "")
                    if (Convert.ToInt32(dr["urEdit"].ToString()) > 0)
                      show = true;
                break;
            /*
            case "secondmeter":
                if (m_mode == "edit")
                  if (dr["urEdit"].ToString() != "")
                    if (Convert.ToInt32(dr["urEdit"].ToString()) > 0)
                      show = true;
                break;
             * */
            case "seasonalpm":
                if (m_mode == "edit")
                  if (dr["urEdit"].ToString() != "")
                    if (Convert.ToInt32(dr["urEdit"].ToString()) > 0)
                      show = true;
                break;

          default:
              break;
        }
        return show;
    }
  
    private void RetrieveMessage()
    {
        SystemMessage msg = new SystemMessage("pm/pmheader.ascx");
        m_msg = msg.GetSystemMessage();
        //msg.SetJsMessage(litMessage);
    }

}