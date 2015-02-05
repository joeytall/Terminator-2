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

public partial class labour_labourheader : System.Web.UI.UserControl
{
    private string m_mode;
    private string m_tabname;
    private string m_operationlabel;
    // private string m_empid = "";
    public string m_empid = "";
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
            m_empid = m_nvc["empid"];
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
        defaultprintcounter = AzzierPrintForm.GetDefaultReport("Labour");
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
            tabhlkMain.NavigateUrl = "labourmain.aspx?mode=edit&empid=" + m_nvc["empid"];
            tabhlkMain.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgMain','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblMain','toptabover')");
            tabhlkMain.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgMain','../images/tabbutton.png');classChangeover('ucHeader1_tablblMain','toptabout')");
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
                tabhlkHistory.NavigateUrl = "labourhistory.aspx?empid=" + m_nvc["empid"];
                tabhlkHistory.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgHistory','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblHistory','toptabover')");
                tabhlkHistory.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgHistory','../images/tabbutton.png');classChangeover('ucHeader1_tablblHistory','toptabout')");
            }
            else
            {
                tabhlkHistory.Enabled = false;
                tablblHistory.CssClass = "toptabinactive";
            }
        }

        if (m_tabname == "Additional Info")
        {
            tabhlkSpecs.NavigateUrl = "javascript:void(null)";
            tabimgSpecs.ImageUrl = "../images/tabbutton_down.png";
            tablblSpecs.CssClass = "toptabover";
        }
        else
        {
            if (m_mode == "edit")
            {
                tabhlkSpecs.NavigateUrl = "labourinfo.aspx?empid=" + m_nvc["empid"];
                tabhlkSpecs.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgSpecs','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblSpecs','toptabover')");
                tabhlkSpecs.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgSpecs','../images/tabbutton.png');classChangeover('ucHeader1_tablblSpecs','toptabout')");
            }
            else
            {
                tabhlkSpecs.Enabled = false;
                tablblSpecs.CssClass = "toptabinactive";
            }
        }

        if (!Page.IsPostBack)
        {
            AzzierHeader.InitToolBar(toolbar, "Labour", ShowToolbarButtonRights);
        }

        litOperation.Text = m_operationlabel;
        AzzierHeader.CreateLinksDropDown(plhLinkDoc, "Labour", m_empid, out linksrvr);
        AzzierHeader.CreateReportsDropDown(plhReports, "Labour");
        AzzierHeader.CreateQueryDropDown(plhQuery, "Labour");

        if (Page.IsPostBack)
        {
            if (Request.Form["__EVENTTARGET"] == "save")
            {
                SaveEmployee();
            }
            if (Request.Form["__EVENTTARGET"] == "lookup")
            {
                lbnLookUpEmployee_Click(null, null);
            }
            if (Request.Form["__EVENTTARGET"] == "delete")
            {
                DeleteEmployee();
            }

        }
    }
    protected void toolbarcombobox_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        if (e.Value == "lookup")
            lbnLookUpEmployee_Click(null, null);
        else if (e.Value == "save")
            SaveEmployee();
        toolbarcombobox.SelectedIndex = 0;
    }

    protected void lbnLookUpEmployee_Click(object sender, EventArgs e)
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        if (CntlPanel != null)
        {
            screen = new AzzierScreen("labour/labourmain.aspx", "MainForm", CntlPanel.Controls);
            screen.LCID = Session.LCID;
            nvc = screen.CollectFormValues("employee", true);  // use the view name
            //m_division = nvc["division"];
        }
        else
            nvc = null;
        Employee obj = new Employee(Session["Login"].ToString(), "employee", "empid");   // use the view name
        string wherestring = "", linqwherestring = "";
        //string[] empids = obj.Query(nvc, ref wherestring);
        obj.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
        string jscript = "<script type=\"text/javascript\">";
        jscript += "var wherestring = '" + Server.UrlEncode(linqwherestring) + "'; var obj=parent.controlpanel.document.location.href='labourpanel.aspx?showlist=queryresult&wherestring='+wherestring;";

        jscript += "</script>";
        litFrameScript.Text = jscript;
    }

    protected void lbnSaveEmployee_Click(object sender, EventArgs e)
    {
        SaveEmployee();
    }

    private void SaveEmployee()
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        TextBox tbx = null;
        string dirtylog = "0";
        if (CntlPanel != null)
        {
            screen = new AzzierScreen("labour/labourmain.aspx", "MainForm", CntlPanel.Controls);
            screen.LCID = Session.LCID;
            nvc = screen.CollectFormValues("employee", false);  // use the table name
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

        Employee obj;
        bool success = false;
        if (m_mode == "new")
        {
            obj = new Employee(Session["Login"].ToString(), "Employee", "empid");   // use the table name
            string t1 = nvc["empid"];
            success = obj.Create(nvc);

            //if (success)
            //{
            //    //string wonum = nvc["wonum"].ToString().ToUpper();
            //    //string[] wonums = { wonum };
            //}

        }
        else if (m_mode == "duplicate")
        {
            string newempid, oldempid, username;
            obj = new Employee(Session["Login"].ToString(), "Employee", "empid");
            HiddenField hid;
            hid = CntlPanel.FindControl("HidOldempid") as HiddenField;
            if (hid != null)
            {
                oldempid = hid.Value;
                newempid = nvc["empid"];
                username = Session["Login"].ToString();
                nvc.Remove("Emppword");
                success = obj.Create(nvc);

                if (success)
                {
                  Specification scopy = new Specification(Session["Login"].ToString(), "Specification", "Counter");
                    scopy.SpecCopy(oldempid, newempid, "Labour");
                }
            }
        }
        else if (m_mode == "edit")
        {
            string empid = nvc["empid"];
            obj = new Employee(Session["Login"].ToString(), "employee", "empid", empid);   // use the table name
            success = obj.Update(nvc);
        }
        else
        {
            obj = new Employee(Session["Login"].ToString(), "Employee", "empid");
        }

        if (!success)
        {
            //string jscript = "<script type=\"text/javascript\">alert(\"" + objWorkorder.ErrorMessage.Replace("'", "\'") + "\"); document.location.href='Womain.aspx?mode=edit&wonum=" + Server.UrlEncode(nvc["wonum"]) + "';</script>";
            //string jscript = "<script>alert(\"" + objWorkorder.ErrorMessage.Replace("\r\n","") + "\"); document.location.href='Womain.aspx?mode=edit&wonum=" + Server.UrlEncode(nvc["wonum"]) + "';</script>";
            string jscript = "<script>alert(\"" + obj.ErrorMessage.Replace("\r\n", "") + "\");</script>";
            litFrameScript.Text = jscript;
        }
        else
        {
            Response.Redirect("labourmain.aspx?mode=edit&empid=" + Server.UrlEncode(nvc["empid"]));
        }
    }

    private void DeleteEmployee()
    {
        Employee obj;
        bool success = false;
        obj = new Employee(Session["Login"].ToString(), "employee", "empid", m_empid);   // use the table name
        success = obj.Delete();

        if (!success)
        {
            string jscript = "<script>alert(\"" + obj.ErrorMessage.Replace("\r\n", "") + "\");</script>";
            litFrameScript.Text = jscript;
        }
        else
        {
            litFrameScript.Text = "<script> setTimeout(parent.controlpanel.RefreshPanelGridsOnDelete(), 0); </script>";
            Response.Redirect("labourmain.aspx?mode=query");
        }
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
            case "autonew":
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
            case "delete":
                if (m_mode == "edit")
                    if (dr["urDelete"].ToString() != "")
                        if (Convert.ToInt32(dr["urDelete"].ToString()) > 0)
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
            case "scheduler":
                if (Scheduler.checkSchedulerAccess("labourScheduler") == true)
                    show = true;
                break;
            case "template":
                if (m_tabname == "Additional Info")
                {
                  if (dr["urEdit"].ToString() != "")
                    if (Convert.ToInt32(dr["urEdit"]) != 0 && alloweditdiv)
                      show = true;
                }
                break;
            default:
                break;
        }
        return show;
    }
    /*
      private void AddButtonToComboBox()
      {
          if (toolbarcombobox.Items.Count > 0)
              toolbarcombobox.Items.Clear();
          toolbarcombobox.AllowCustomText = false;
          RadComboBoxItem item = new RadComboBoxItem();
          item.Text = "";
          item.Height = 1;
          toolbarcombobox.Items.Add(item);
      }
      private void AddButtonToComboBox(DataRow row)
      {
          RadComboBoxItem item = new RadComboBoxItem();
          item.Text = row["MenuItemDesc"].ToString();
          item.ImageUrl = row["ImageURL"].ToString();
          toolbarcombobox.Items.Add(item);
          toolbarcombobox.EmptyMessage = "";
          item.Value = row["CommandName"].ToString();

      }
    */
    /*
      private void LayoutToolbar(Control c, string txt)
      {
          HtmlGenericControl li = new HtmlGenericControl("li");
          li.Controls.Add(c);
          toolbar_table.Controls.Add(li);
      }
     * */

    private void RetrieveMessage()
    {
        SystemMessage msg = new SystemMessage("labour/labourheader.ascx");//"workorder/woheader.ascx");
        m_msg = msg.GetSystemMessage();
        //msg.SetJsMessage(litMessage);
    }

}
