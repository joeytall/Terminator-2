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

public partial class pdm_pdmheader : System.Web.UI.UserControl
{
    protected string m_mode;
    private string m_tabname;
    private string m_operationlabel;
    // private string m_empid = "";
    public string m_pdmnum = "";
    private NameValueCollection m_nvc;
    private AzzierScreen screen;
    protected string linksrvr = "";
    private int m_AllowEdit;
    protected RadComboBox toolbarcombobox;
    private string m_division;
    protected int m_masterrec;
    protected int simplepm = 1;
    protected int defaultprintcounter = AzzierPrintForm.GetDefaultReport("PDM");
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
            m_pdmnum = m_nvc["pdmnum"];
            m_division = m_nvc["division"];
        }        
    }

    protected void Page_Load(object sender, EventArgs e)
    {
      UserRights.CheckAccess("PDM");
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
            tabhlkMain.NavigateUrl = "pdmmain.aspx?mode=edit&pdmnum=" + m_nvc["pdmnum"];
            tabhlkMain.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgMain','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblMain','toptabover')");
            tabhlkMain.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgMain','../images/tabbutton.png');classChangeover('ucHeader1_tablblMain','toptabout')");
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
            tabhlkWO.NavigateUrl = "pdmwolist.aspx?pdmnum=" + m_pdmnum;
            tabhlkWO.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgWO','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblWO','toptabover')");
            tabhlkWO.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgWO','../images/tabbutton.png');classChangeover('ucHeader1_tablblWO','toptabout')");
          }
          else
          {
            tabhlkWO.Enabled = false;
            tablblWO.CssClass = "toptabinactive";
          }
        }

        if (m_tabname == "PDM HISTORY")
        {
          tabhlkHistory.NavigateUrl = "javascript:void(null)";
          tabimgHistory.ImageUrl = "../images/tabbutton_down.png";
          tablblHistory.CssClass = "toptabover";
        }
        else
        {
          if (m_mode == "edit")
          {
            tabhlkHistory.NavigateUrl = "pdmhistory.aspx?pdmnum=" + m_pdmnum;
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
          AzzierHeader.InitToolBar(toolbar, "PDM", ShowToolbarButtonRights);
        }

        litOperation.Text = m_operationlabel;
        AzzierHeader.CreateLinksDropDown(plhLinkDoc, "PdM", m_pdmnum, out linksrvr);
        AzzierHeader.CreateReportsDropDown(plhReports, "PdM");
        AzzierHeader.CreateQueryDropDown(plhQuery, "PdM");

        if (Page.IsPostBack)
        {
          if (Request.Form["__EVENTTARGET"] == "lookup")
          {
            LookUpPdM();
          }
          if (Request.Form["__EVENTTARGET"] == "delete")
          {
            DeletePdM();
          }
        }
        if (m_nvc["PMInterval"] != "" && m_nvc["MeterInterval"] != "")
          simplepm = 0;
        if (m_nvc["MeterInterval2"] != "" && m_nvc["MeterInterval"] != "")
          simplepm = 0;
    }

    protected void DeletePdM()
    {
      ModuleoObject pdm = new ModuleoObject(Session["Login"].ToString(), "pdm", "pdmnum", m_pdmnum);
      pdm.Delete();
      string jscript = "<script type=\"text/javascript\">";
      jscript += "setTimeout(parent.controlpanel.RefreshPanelGridsOnDelete(), 0); document.location.href='pdmmain.aspx';";
      jscript += "</script>";
      litFrameScript.Text = jscript;
    }

    public void LookUpPdM()
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        if (CntlPanel != null)
        {
            screen = new AzzierScreen("pdm/pdmmain.aspx", "MainForm", CntlPanel.Controls);
            screen.LCID = Session.LCID;
            nvc = screen.CollectFormValues("PDM",true);  // use the view name
            //m_division = nvc["division"];

            Validation v = new Validation();
            string pdmnum = nvc["PDMNum"].ToString() ?? "";
            if (pdmnum.Length > 1)
            {
              if (v.SpecialStrValidator(pdmnum))
              {
                nvc = new NameValueCollection();
                nvc.Add("PDMNum", pdmnum);
              }
            }
        }
        else
            nvc = null;


        PDM obj = new PDM(Session["Login"].ToString(), "PDM", "PDMNum");   // use the view name
        string wherestring = "",linqwherestring="";
        obj.CreateLinqCondition(nvc, ref wherestring,ref linqwherestring);
        string jscript = "<script type=\"text/javascript\">";
        jscript += "var wherestring = '" + Server.UrlEncode(linqwherestring) + "'; var obj=parent.controlpanel.document.location.href='pdmpanel.aspx?showlist=queryresult&wherestring='+wherestring;";
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
            case "autopdm":
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
        SystemMessage msg = new SystemMessage("pdm/pdmheader.ascx");
        m_msg = msg.GetSystemMessage();
        //msg.SetJsMessage(litMessage);
    }

}