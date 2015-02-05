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

public partial class location_locationheader : System.Web.UI.UserControl
{
    private string m_mode;
    private string m_tabname;
    private string m_operationlabel;
    // private string m_empid = "";
    public string m_location = "";
    private NameValueCollection m_nvc;
    private AzzierScreen screen;
    protected string linksrvr = "";
    private bool m_AllowEdit;
    private bool m_AllowDelete;
    protected RadComboBox toolbarcombobox;
    private string m_division;
    protected string m_masterrec;
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
            m_location = m_nvc["location"];
            m_division = m_nvc["division"];
            m_masterrec = m_nvc["MasterRec"];
            m_AllowDelete = UserRights.AllowDelete(Session["Login"].ToString(), "Location", m_division, m_masterrec);
            m_AllowEdit = UserRights.AllowEdit(Session["Login"].ToString(), "Location", m_division, m_masterrec);
        }
        
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        RetrieveMessage();
        toolbarcombobox = new RadComboBox();

        mainmodul_ScrnSize = Convert.ToInt32(Session["ScreenWidth"].ToString());
        mainmodul_ScrnSize = (int)Math.Floor(mainmodul_ScrnSize * 0.67) - 100;
        decimal t_iconnum = mainmodul_ScrnSize / 50;
        tmp_ScrnIconNum = (int)Math.Floor(t_iconnum);

        Session.LCID = Convert.ToInt32(Session["LCID"]);
        string connstring = Application["ConnString"].ToString();
        defaultprintcounter = AzzierPrintForm.GetDefaultReport("Location");
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
            tabhlkMain.NavigateUrl = "locationmain.aspx?mode=edit&location=" + m_nvc["location"];
            tabhlkMain.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgMain','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblMain','toptabover')");
            tabhlkMain.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgMain','../images/tabbutton.png');classChangeover('ucHeader1_tablblMain','toptabout')");
        }
        /*
        if (m_tabname == "Equipment List")
        {
            tabhlkEQList.NavigateUrl = "javascript:void(null)";
            tabimgEQList.ImageUrl = "../images/tabbutton_down.png";
            tablblEQList.CssClass = "toptabover";
        }
        else
        {
            if (m_mode == "edit")
            {
                tabhlkEQList.NavigateUrl = "locationeqplist.aspx?location=" + m_nvc["location"];
                tabhlkEQList.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgEQList','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblEQList','toptabover')");
                tabhlkEQList.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgEQList','../images/tabbutton.png');classChangeover('ucHeader1_tablblEQList','toptabout')");
            }
            else
            {
                tabhlkEQList.Enabled = false;
                tablblEQList.CssClass = "toptabinactive";
            }
        }
         * */

        if (m_tabname == "Specifications")
        {
            tabhlkSpecs.NavigateUrl = "javascript:void(null)";
            tabimgSpecs.ImageUrl = "../images/tabbutton_down.png";
            tablblSpecs.CssClass = "toptabover";
        }
        else
        {
            if (m_mode == "edit")
            {
                tabhlkSpecs.NavigateUrl = "locationspecs.aspx?location=" + m_nvc["location"];
                tabhlkSpecs.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgSpecs','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblSpecs','toptabover')");
                tabhlkSpecs.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgSpecs','../images/tabbutton.png');classChangeover('ucHeader1_tablblSpecs','toptabout')");
            }
            else
            {
                tabhlkSpecs.Enabled = false;
                tablblSpecs.CssClass = "toptabinactive";
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
            tabhlkHistory.NavigateUrl = "locationhistory.aspx?location=" + m_nvc["location"];
            tabhlkHistory.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgHistory','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblHistory','toptabover')");
            tabhlkHistory.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgHistory','../images/tabbutton.png');classChangeover('ucHeader1_tablblHistory','toptabout')");
          }
          else
          {
            tabhlkHistory.Enabled = false;
            tablblHistory.CssClass = "toptabinactive";
          }
        }

        if (m_tabname == "Hierarchy")
        {
          tabhlkHierarchy.NavigateUrl = "javascript:void(null)";
          tabimgHierarchy.ImageUrl = "../images/tabbutton_down.png";
          tablblHierarchy.CssClass = "toptabover";
        }
        else
        {
          if (m_mode == "edit")
          {
            tabhlkHierarchy.NavigateUrl = "locationtree.aspx?location=" + m_nvc["location"];
            tabhlkHierarchy.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgHierarchy','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblHierarchy','toptabover')");
            tabhlkHierarchy.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgHierarchy','../images/tabbutton.png');classChangeover('ucHeader1_tablblHierarchy','toptabout')");
          }
          else
          {
            tabhlkHierarchy.Enabled = false;
            tablblHierarchy.CssClass = "toptabinactive";
          }
        }

        if (!Page.IsPostBack)
        {
          AzzierHeader.InitToolBar(toolbar, "Location", ShowToolbarButtonRights);
        }

        litOperation.Text = m_operationlabel;
        AzzierHeader.CreateLinksDropDown(plhLinkDoc, "Location", m_location, out linksrvr);
        AzzierHeader.CreateReportsDropDown(plhReports, "Location");
        AzzierHeader.CreateQueryDropDown(plhQuery, "Location");

        if (Page.IsPostBack)
        {
          if (Request.Form["__EVENTTARGET"] == "save")
          {
            SaveLocation();
          }
          if (Request.Form["__EVENTTARGET"] == "lookup")
          {
            LookUpLocation();
          }
          if (Request.Form["__EVENTTARGET"] == "delete")
          {
            DeleteLocation();
          }

        }
    }

    protected void DeleteLocation()
    {
      Location loc = new Location(Session["Login"].ToString(), "Location", "Location", m_location);
      loc.Delete();
      string jscript = "<script type=\"text/javascript\">";
      jscript += "setTimeout(parent.controlpanel.RefreshPanelGridsOnDelete(), 0); document.location.href='locationmain.aspx';";
      jscript += "</script>";
      litFrameScript.Text = jscript;
    }

    protected void LookUpLocation()
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        if (CntlPanel != null)
        {
            screen = new AzzierScreen("location/locationmain.aspx", "MainForm", CntlPanel.Controls);
            screen.LCID = Session.LCID;
            nvc = screen.CollectFormValues("Location", true);  // use the view name
            //m_division = nvc["division"];
        }
        else
            nvc = null;
        Location obj = new Location(Session["Login"].ToString(), "Location", "location");   // use the view name
        string wherestring = "", wherestringlinq = "";

        obj.CreateLinqCondition(nvc, ref wherestring, ref wherestringlinq);
        string jscript = "<script type=\"text/javascript\">";
        jscript += "var wherestring = '" + Server.UrlEncode(wherestringlinq) + "';var obj=parent.controlpanel.document.location.href='locationpanel.aspx?showlist=queryresult&wherestring='+wherestring;";
        jscript += "</script>";
        litFrameScript.Text = jscript;
    }

    private void SaveLocation()
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        TextBox tbx = null;
        string dirtylog = "0";
        if (CntlPanel != null)
        {
            screen = new AzzierScreen("location/locationmain.aspx", "MainForm", CntlPanel.Controls);
            screen.LCID = Session.LCID;
            nvc = screen.CollectFormValues("Location", false);  // use the table name
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

        Location obj;
        bool success = false;
        if (m_mode == "new")
        {
            obj = new Location(Session["Login"].ToString(), "Location", "location");   // use the table name
            string t1 = nvc["location"];
            success = obj.Create(nvc);

            //if (success)
            //{
            //    //string wonum = nvc["wonum"].ToString().ToUpper();
            //    //string[] wonums = { wonum };
            //}

        }
        else if (m_mode == "duplicate")
        {
            string newlocation, oldlocation;
            obj = new Location(Session["Login"].ToString(), "Location", "location");
            HiddenField hid;
            hid = CntlPanel.FindControl("HidOldLocation") as HiddenField;
            if (hid != null)
            {
                oldlocation = hid.Value;
                newlocation = nvc["location"];
                //username = Session["Login"].ToString();

                success = obj.Create(nvc);

                if (success)
                {
                  Specification scopy = new Specification(Session["Login"].ToString(), "Specification", "Counter");
                  scopy.SpecCopy(oldlocation, newlocation, "Location");
                }
            }
        }
        else if (m_mode == "edit")
        {
            string location = nvc["location"];
            obj = new Location(Session["Login"].ToString(), "Location", "location", location);   // use the table name
            success = obj.Update(nvc);
        }
        else
            obj = new Location(Session["Login"].ToString(), "Location","location");

        if (!success)
        {
            //string jscript = "<script type=\"text/javascript\">alert(\"" + objWorkorder.ErrorMessage.Replace("'", "\'") + "\"); document.location.href='Womain.aspx?mode=edit&wonum=" + Server.UrlEncode(nvc["wonum"]) + "';</script>";
            //string jscript = "<script>alert(\"" + objWorkorder.ErrorMessage.Replace("\r\n","") + "\"); document.location.href='Womain.aspx?mode=edit&wonum=" + Server.UrlEncode(nvc["wonum"]) + "';</script>";
            string jscript = "<script>alert(\"" + obj.ErrorMessage.Replace("\r\n", "") + "\");</script>";
            litFrameScript.Text = jscript;
        }
        else
        {
            if (m_mode == "new")
                Response.Redirect("locationmain.aspx?mode=edit&location=" + Server.UrlEncode(nvc["location"]));
            else
                if (m_mode == "duplicate")
                    Response.Redirect("locationmain.aspx?mode=edit&location=" + Server.UrlEncode(nvc["location"]));
                else if (m_mode == "edit")
                {
                    //lblErrorMessage.Text = "";
                    //lblErrorMessage.Visible = false;
                    // no more reload after save, increment dirtylog or can't save again
                    tbx.Text = (Convert.ToInt32(dirtylog) + 1).ToString();
                }
        }
      
    }

    private Boolean ShowToolbarButtonRights(string commandname, NameValueCollection dr)
    {
        if (dr == null)
            return false;
        Boolean show = false;
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
                  if (m_mode == "edit")
                  {
                    if (m_AllowEdit)
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
                  if (m_AllowDelete)
                    show = true;
                }
                break;
          case "modify":
                if (m_tabname == "Main" && m_mode == "edit")
                {
                  if (m_AllowEdit)
                    show = true;
                }
                break;
          case "map":
                if (m_mode == "edit")
                {
                  show = true;
                }
                break;
          case "template":
                if (m_tabname == "Specifications")
                {
                  if (m_AllowEdit)
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
        SystemMessage msg = new SystemMessage("location/locationheader.ascx"); //location/locationmain.aspx
        m_msg = msg.GetSystemMessage();
        //msg.SetJsMessage(litMessage);
    }

}