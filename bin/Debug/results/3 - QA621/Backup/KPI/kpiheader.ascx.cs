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

public partial class kpi_kpiheader : System.Web.UI.UserControl
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
            tabhlkMain.NavigateUrl = "kpimain.aspx";
            tabhlkMain.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgMain','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblMain','toptabover')");
            tabhlkMain.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgMain','../images/tabbutton.png');classChangeover('ucHeader1_tablblMain','toptabout')");
        }

        if (!Page.IsPostBack)
        {
            InitToolBar();
        }

        litOperation.Text = m_operationlabel;
        AzzierHeader.CreateLinksDropDown(plhLinkDoc, "KPI", "", out linksrvr);
        AzzierHeader.CreateReportsDropDown(plhReports, "KPI");
        AzzierHeader.CreateQueryDropDown(plhQuery, "KPI");
    }


    private void RetrieveMessage()
    {
        SystemMessage msg = new SystemMessage("labour/labourheader.ascx");//"workorder/woheader.ascx");
        m_msg = msg.GetSystemMessage();
        //msg.SetJsMessage(litMessage);
    }

    private void InitToolBar()
    {
        int iconwidth = 55;
        int screenwidth = Convert.ToInt32(Session["ScreenWidth"].ToString());
        int panelwidth = (int)Math.Floor(screenwidth * 0.67) - 100;
        if (Session["KPIFrameMain"] != null)
        {
            panelwidth = (int)Math.Floor(screenwidth * (Convert.ToDecimal(Session["KPIFrameMain"].ToString()) / 100)) - 100;
        }
        int totaliconnum = (int)Math.Floor((decimal)panelwidth / iconwidth);
        RadToolBarDropDown toolbarcombobox = new RadToolBarDropDown();

        string connstring = Application["ConnString"].ToString();
        OleDbConnection conn = new OleDbConnection(connstring);
        conn.Open();
        string sql = "SELECT * FROM UserToolbar WHERE UserId='" + Session["Login"].ToString() + "' AND UserModule='KPI' ORDER BY orderby";
        OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
        DataTable dt = new DataTable();
        da.Fill(dt);
        da.Dispose();

        if (dt.Rows.Count == 0)
        {
            sql = "SELECT * FROM UserToolbar WHERE UserId='DEFAULT' AND UserModule='KPI' ORDER BY orderby";
            da = new OleDbDataAdapter(sql, conn);
            da.Fill(dt);
            da.Dispose();
        }


        NameValueCollection drRights;
        UserRights right = new UserRights(Session["Login"].ToString(), "UserRights", "Counter");
        drRights = right.GetRights(Session["Login"].ToString(), "Labour");

        int iconnum = 0;
        //toolbar = new RadToolBar();
        //toolbar.Height = Unit.Pixel(64);
        toolbar.Height = Unit.Percentage(100);
        toolbar.OnClientButtonClicking = "toolbarclicked";
        toolbar.AutoPostBack = false;
        //toolbar.ButtonClick += new RadToolBarEventHandler(toolbar_ButtonClick);
        toolbar.Width = Unit.Percentage(100);
        foreach (DataRow dr in dt.Rows)
        {
            string commandname = dr["CommandName"].ToString().ToLower();
            //if (!ShowToolbarButtonRights(commandname, drRights))
                //continue;

            RadToolBarButton btn = new RadToolBarButton();
            btn.ImagePosition = ToolBarImagePosition.AboveText;
            btn.Text = dr["LabelText"].ToString();
            btn.ToolTip = dr["ToolTip"].ToString();
            btn.BackColor = System.Drawing.Color.Transparent;
            btn.CommandName = commandname;
            btn.Font.Size = 9;
            btn.ImageUrl = dr["ImageURL"].ToString();
            btn.HoveredImageUrl = dr["HoverUrl"].ToString();

            //setup images
            switch (commandname)
            {
                case "search":
                    //btn.ImageUrl = "../images/labour/labour_query_32.png";
                    //btn.HoveredImageUrl = "../images/labour/labour_query_32_over.png";
                    btn.CausesValidation = false;
                    break;
                case "new":
                    //btn.ImageUrl = "../images/labour/new_labour_32.png";
                    //btn.HoveredImageUrl = "../images/labour/new_labour_32_over.png";
                    btn.CausesValidation = false;
                    break;

                case "autonew":
                    //btn.ImageUrl = "../images/labour/new_auto_labour_32.png";
                    //btn.HoveredImageUrl = "../images/labour/new_auto_labour_32_over.png";
                    btn.CausesValidation = false;
                    break;

                case "lookup":
                    //btn.ImageUrl = "../images/labour/labour_search_32.png";
                    //btn.HoveredImageUrl = "../images/labour/labour_search_32_over.png";
                    btn.CausesValidation = false;
                    break;
                case "save":
                    //btn.ImageUrl = "../images/save_32.png";
                    //btn.HoveredImageUrl = "../images/save_32_over.png";
                    btn.CausesValidation = false;
                    break;
                case "delete":
                    //btn.ImageUrl = "../images/labour/labour_delete_32.png";
                    //btn.HoveredImageUrl = "../images/labour/labour_delete_32_over.png";
                    btn.CausesValidation = false;
                    break;

                case "duplicate":
                    //btn.ImageUrl = "../images/labour/duplicate_labour_32.png";
                    //btn.HoveredImageUrl = "../images/labour/duplicate_labour_32_over.png";
                    btn.CausesValidation = false;
                    break;
                case "print":
                    //btn.ImageUrl = "../images/print32.png";
                    //btn.HoveredImageUrl = "../images/print32_over.png";
                    btn.CausesValidation = false;
                    break;
                case "picture":
                    //btn.ImageUrl = "../images/picture32.png";
                    //btn.HoveredImageUrl = "../images/picture32_over.png";
                    btn.CausesValidation = false;
                    break;
                case "linkeddoc":
                    //btn.ImageUrl = "../images/links32.png";
                    //btn.HoveredImageUrl = "../images/links32_over.png";
                    btn.CausesValidation = false;
                    break;
            }

            if (iconnum >= totaliconnum - 1)
            {

                if (iconnum == totaliconnum - 1)
                {
                    toolbar.Items.Add(toolbarcombobox);
                }
                btn.ForeColor = System.Drawing.Color.Black;
                toolbarcombobox.Buttons.Add(btn);
            }
            else
            {
                btn.Width = Unit.Pixel(iconwidth);
                btn.ForeColor = System.Drawing.Color.FloralWhite;
                toolbar.Items.Add(btn);
            }
            iconnum++;
        }
    }
}