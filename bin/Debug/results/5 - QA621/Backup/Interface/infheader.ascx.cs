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
using System.Xml;

public partial class interface_infheader : System.Web.UI.UserControl
{
    private string m_mode;
    private string m_tabname;
    private string m_operationlabel;
    // private string m_empid = "";
    public string m_interfacename = "";
    private NameValueCollection m_nvc;
    private AzzierScreen screen;
    //protected RadToolBar toolbar = new RadToolBar();
    protected string linksrvr = "";
    private int m_AllowEdit;
    protected string m_oldinterfacename;
    protected RadComboBox toolbarcombobox;
    //private string m_division;

    protected string m_xmlstr = "<?xml version='1.0' encoding='UTF-8'?><root><Workorder2><workordernum>skyinterfacetest1</workordernum>"+
		                        "<empid>xxxboy</empid><WOLabour><workordernum>skyinterfacetest1</workordernum><employee>joey</employee><time>2"+
                                "</time></WOLabour><WOLabour><workordernum>skyinterfacetest1</workordernum><employee>joey</employee>"+
			                    "<time>2</time></WOLabour><WOLabour><workordernum>skyinterfacetest1</workordernum><employee>joey</employee>"+
	                            "<time>3</time></WOLabour><WOLabour><workordernum>skyinterfacetest1</workordernum><employee>joey</employee>"+
			                    "<time>5</time></WOLabour></Workorder2><Workorder2><workordernum>skyinterfacetest2</workordernum><empid>sky</empid>"+
		                        "<WOLabour><workordernum>skyinterfacetest2</workordernum><employee>sky</employee><time>4.5</time></WOLabour><WOLabour>"+
			                    "<workordernum>skyinterfacetest2</workordernum><employee>sky</employee><time>2.6</time></WOLabour><WOLabour>"+
			                    "<workordernum>skyinterfacetest2</workordernum><employee>sky</employee><time>3.3</time></WOLabour><WOLabour>"+
			                    "<workordernum>skyinterfacetest2</workordernum><employee>sky</employee><time>5.7</time></WOLabour></Workorder2></root>";

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
    public string OldInterfacename
    {
        set { m_oldinterfacename = value; }
    }


    public NameValueCollection ModuleData
    {
        set
        {
            m_nvc = value;
            m_interfacename = m_nvc["interfacename"];
            //m_division = m_nvc["division"];
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

        if (!Page.IsPostBack)
        {
            AzzierHeader.InitToolBar(toolbar, "Interface", ShowToolbarButtonRights);
        }
        litOperation.Text = m_operationlabel;
        AzzierHeader.CreateLinksDropDown(plhLinkDoc, "Interface", m_interfacename, out linksrvr);
        AzzierHeader.CreateReportsDropDown(plhReports, "Interface");
        AzzierHeader.CreateQueryDropDown(plhQuery, "Interface");
    }

    protected void lbnLookUp_Click(object sender, EventArgs e)
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        if (CntlPanel != null)
        {
            screen = new AzzierScreen("interface/infmain.aspx", "MainForm", CntlPanel.Controls);
            screen.LCID = Session.LCID;
            nvc = screen.CollectFormValues("infmaster", true);  // use the view name
            //m_division = nvc["division"];
        }
        else
            nvc = null;
        Interface obj = new Interface(Session["Login"].ToString(), "infmaster", "interfacename");   // use the view name
        string wherestring = "", linqwherestring = "";
        obj.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
        string jscript = "<script type=\"text/javascript\">";
        jscript += "var wherestring = '" + Server.UrlEncode(linqwherestring) + "'; var obj=parent.controlpanel.document.location.href='infpanel.aspx?showlist=queryresult&wherestring='+wherestring;";
        jscript += "</script>";
        litFrameScript.Text = jscript;
    }


    private void ExportTemplate()
    {
        Interface infobj = new Interface(Session["Login"].ToString(), "InfMaster", "InterfaceName");
        string xmlstring = infobj.GenertateXMLTemplate(m_interfacename);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xmlstring);


        Response.ClearContent();
        Response.AddHeader("Content-Disposition", "attachment; filename=" + m_interfacename + ".xml");
        Response.AddHeader("Content-Length", xmlstring.Length.ToString());
        Response.ContentType = "text/xml";
        Response.Write(doc.InnerXml);
        Response.End();
    }

    private Boolean ShowToolbarButtonRights(string commandname, NameValueCollection dr)
    {
        if (dr == null)
            return false;
        Boolean show = false;
        Division d = new Division();
        //bool alloweditdiv = d.Editable("'" + m_division + "'");
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
                            if (Convert.ToInt32(dr["urEdit"]) != 0)
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
            case "exporttemplate":
                if (m_mode =="edit")
                    show = true;
                break;
            default:
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