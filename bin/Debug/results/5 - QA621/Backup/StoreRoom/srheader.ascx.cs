using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Data;
using System.Data.OleDb;


public partial class StoreRoom_srheader : System.Web.UI.UserControl
{
    private string m_division;
    private string m_mode;
    private string m_tabname;
    private string m_operationlabel;
    protected string m_storeroom;
    private AzzierScreen screen;
    protected string linksrvr = "";

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

    public string Division
    {
        set { m_division = value; }
    }

    public string StoreRoom
    {
        set { m_storeroom = value; }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            AzzierHeader.InitToolBar(toolbar, "StoreRoom", ShowToolbarButtonRights);
        }

        litOperation.Text = m_operationlabel;
        AzzierHeader.CreateLinksDropDown(plhLinkDoc, "StoreRoom", m_storeroom, out linksrvr);
        AzzierHeader.CreateReportsDropDown(plhReports, "StoreRoom");
        AzzierHeader.CreateQueryDropDown(plhQuery, "StoreRoom");

        if (Request.Form["__EVENTTARGET"] == "lookup")
        {
            LookUpClicked();
        }
    }

    private void LookUpClicked()
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        if (CntlPanel != null)
        {
            screen = new AzzierScreen("StoreRoom/srmain.aspx", "MainForm", CntlPanel.Controls);
            screen.LCID = Session.LCID;
            nvc = screen.CollectFormValues("StoreRoom", true);  // use the view name
        }
        else
            nvc = null;
        ModuleoObject obj = new ModuleoObject(Session["Login"].ToString(), "StoreRoom", "StoreRoom");   // use the view name
        string wherestring = "", linqwherestring = "";
        //string[] empids = obj.Query(nvc, ref wherestring);
        obj.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
        string jscript = "<script type=\"text/javascript\">";
        jscript += "var wherestring = '" + Server.UrlEncode(linqwherestring) + "'; var obj=parent.controlpanel.document.location.href='srpanel.aspx?showlist=queryresult&wherestring='+wherestring;";

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
            case "delete":
                if (m_mode == "edit")
                    if (dr["urDelete"].ToString() != "")
                        if (Convert.ToInt32(dr["urDelete"].ToString()) > 0)
                            show = true;
                break;
            case "multipleissue":
                if ((m_mode == "edit") && (alloweditdiv))
                {
                    show = true;
                }
                break;
            case "linkeddoc":
                show = true;
                break;
            case "email":
                if (m_mode == "edit")
                    show = true;
                break;
            default:
                show = false;
                break;
        }
        return show;
    }
}