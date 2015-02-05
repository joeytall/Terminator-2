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

public partial class Timecards_tcheader : System.Web.UI.UserControl
{

    protected string m_mode;
    private string m_tabname;
    private string m_operationlabel;
    public string m_empid = "";
    private NameValueCollection m_nvc;
    private AzzierScreen screen;
    protected string linksrvr = "";
    private int m_AllowEdit;
    protected RadComboBox toolbarcombobox;
    private string m_division;

    protected NameValueCollection m_msg = new NameValueCollection();
    protected int mainmodul_ScrnSize = 0, tmp_ScrnIconNum = 0, tmp_ActIconNum = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack) 
        {
            AzzierHeader.InitToolBar(toolbar, "Timecards", ShowToolbarButtonRights);
        }

        litOperation.Text = m_operationlabel;
        AzzierHeader.CreateLinksDropDown(plhLinkDoc, "Timecards", m_empid, out linksrvr);
        AzzierHeader.CreateReportsDropDown(plhReports, "Timecards");
        AzzierHeader.CreateQueryDropDown(plhQuery, "Timecards");

        if (Page.IsPostBack)
        {
            if (Request.Form["__EVENTTARGET"] == "lookup")
            {
                lbnLookUpEmployee_Click(null, null);
            }
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
            m_empid = m_nvc["empid"];
            m_division = m_nvc["division"];
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
            case "lookup":
                if (m_mode == "query")
                    show = true;
                break;
            default:
                show = false;
                break;
        }
        return show;
    }

    protected void lbnLookUpEmployee_Click(object sender, EventArgs e)
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        if (CntlPanel != null)
        {
            screen = new AzzierScreen("timecards/tcmain.aspx", "MainForm", CntlPanel.Controls);
            screen.LCID = Session.LCID;
            nvc = screen.CollectFormValues("WOLabour", true);  // use the view name
            //m_division = nvc["division"];
        }
        else
            nvc = null;
        ModuleoObject obj = new ModuleoObject(Session["Login"].ToString(),"WOLabour", "Counter");   // use the view name
        string wherestring = "", linqwherestring = "";
        //string[] empids = obj.Query(nvc, ref wherestring);
        obj.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
        string jscript = "<script type=\"text/javascript\">";
        jscript += "var wherestring = '" + Server.UrlEncode(linqwherestring) + "'; var obj=parent.controlpanel.document.location.href='tcpanel.aspx?showlist=queryresult&wherestring='+wherestring;";

        jscript += "</script>";
        litFrameScript.Text = jscript;
    }

//    private bool SaveTimeCards()
//    {
//        bool result= false;
//        NameValueCollection nvc = new NameValueCollection();
//        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
//        TextBox tbx = null;
        


//        string dirtylog = "0";
        
//        if (CntlPanel != null)
//        {
//            screen = new AzzierScreen("timecards/tcmain.aspx", "MainForm", CntlPanel.Controls);
//            screen.LCID = Session.LCID;
//            nvc = screen.CollectFormValues("WOLabour", false);  // use the table name
//            tbx = CntlPanel.FindControl("txtdirtylog") as TextBox;
//            if (nvc["dirtylog"] == null)
//            {
//                if (tbx != null)
//                {
//                    dirtylog = tbx.Text;
//                    nvc.Add("dirtylog", dirtylog);
//                }
//            }
//            else
//            {
//                dirtylog = nvc["dirtylog"];
//            }
//        }
//        else
//        {
//            nvc = null;
//        }


//        Labour obj = new Labour(Session["Login"].ToString(), "WOLabour", "Counter");
//        TextBox txtcounter = CntlPanel.FindControl("txtcounter") as TextBox;
//        switch (m_mode)
//        {
//            case "new":
//                obj = new Labour(Session["Login"].ToString(), "WOLabour", "Counter");
//                result = obj.CreateLabour(nvc);
//                //string counter = obj.ModuleData["Counter"];
//                UserWorkList list = new UserWorkList();
//                list.AddToRecentList(Session["Login"].ToString(), "TimeCards", obj.ModuleData["counter"]);
//                break;
//            case "edit":
//                obj = new Labour(Session["Login"].ToString(), "WOLabour", "Counter", txtcounter.Text);
//                nvc.Remove("Counter");
//                result =  obj.UpdateLabour(nvc);
//                break;
//            default:
//                break;
//        }
//        if (!result)
//        {
//            string jscript = "<script>alert(\"" + obj.ErrorMessage.Replace("\r\n", "") + "\");</script>";
//            litFrameScript.Text = jscript;
//        }
//        else 
//        {
//            TextBox transdate = CntlPanel.FindControl("txttransdate") as TextBox;
//            Response.Redirect("tcmain.aspx?mode=new&empid=" + Server.UrlEncode(nvc["empid"]) + "&transdate=" + transdate.Text);
//        }
        
//        return result;
//    }
}