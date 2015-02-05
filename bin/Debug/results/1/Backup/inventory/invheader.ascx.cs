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

public partial class inventory_invheader : System.Web.UI.UserControl
{
    private string m_mode;
    private string m_tabname;
    private string m_operationlabel;
    public string m_itemnum = "";
    private NameValueCollection m_nvc;
    private AzzierScreen screen;
    protected string linksrvr = "";
    private int m_AllowEdit;
    protected RadComboBox toolbarcombobox;
    private string m_division;
    protected int defaultprintcounter = AzzierPrintForm.GetDefaultReport("inventory");
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
            m_itemnum = m_nvc["itemnum"];
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
        else
        {
            tabhlkMain.NavigateUrl = "invmain.aspx?mode=edit&itemnum=" + m_nvc["itemnum"];
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
                tabhlkHistory.NavigateUrl = "invhistory.aspx?itemnum=" + m_nvc["itemnum"];
                tabhlkHistory.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgHistory','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblHistory','toptabover')");
                tabhlkHistory.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgHistory','../images/tabbutton.png');classChangeover('ucHeader1_tablblHistory','toptabout')");
            }
            else
            {
                tabhlkHistory.Enabled = false;
                tablblHistory.CssClass = "toptabinactive";
            }
        }

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
                tabhlkSpecs.NavigateUrl = "invspecs.aspx?itemnum=" + m_nvc["itemnum"];
                tabhlkSpecs.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgSpecs','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblSpecs','toptabover')");
                tabhlkSpecs.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgSpecs','../images/tabbutton.png');classChangeover('ucHeader1_tablblSpecs','toptabout')");
            }
            else
            {
                tabhlkSpecs.Enabled = false;
                tablblSpecs.CssClass = "toptabinactive";
            }
        }

        if (m_tabname == "Where Used")
        {
          tabhlkWhereused.NavigateUrl = "javascript:void(null)";
          tabimgWhereused.ImageUrl = "../images/tabbutton_down.png";
          tablblWhereused.CssClass = "toptabover";
        }
        else
        {
          if (m_mode == "edit")
          {
            tabhlkWhereused.NavigateUrl = "whereused.aspx?itemnum=" + m_nvc["itemnum"];
            tabhlkWhereused.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgWhereused','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblWhereused','toptabover')");
            tabhlkWhereused.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgWhereused','../images/tabbutton.png');classChangeover('ucHeader1_tablblWhereused','toptabout')");
          }
          else
          {
            tabhlkWhereused.Enabled = false;
            tablblWhereused.CssClass = "toptabinactive";
          }
        }

        if (m_tabname == "Serialized")
        {
          tabhlkEqpList.NavigateUrl = "javascript:void(null)";
          tabimgEqpList.ImageUrl = "../images/tabbutton_down.png";
          tablblEqpList.CssClass = "toptabover";
        }
        else
        {
          if (m_mode == "edit")
          {
            tabhlkEqpList.NavigateUrl = "serializedeqplist.aspx?itemnum=" + m_nvc["itemnum"];
            tabhlkEqpList.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgEqpList','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblEqpList','toptabover')");
            tabhlkEqpList.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgEqpList','../images/tabbutton.png');classChangeover('ucHeader1_tablblEqpList','toptabout')");
          }
          else
          {
            tabhlkEqpList.Enabled = false;
            tablblEqpList.CssClass = "toptabinactive";
          }
        }

        if (m_tabname == "Vendors")
        {
          tabhlkVendor.NavigateUrl = "javascript:void(null)";
          tabimgVendor.ImageUrl = "../images/tabbutton_down.png";
          tablblVendor.CssClass = "toptabover";
        }
        else
        {
          if (m_mode == "edit")
          {
            tabhlkVendor.NavigateUrl = "invvendor.aspx?itemnum=" + m_nvc["itemnum"];
            tabhlkVendor.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgVendor','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblVendor','toptabover')");
            tabhlkVendor.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgVendor','../images/tabbutton.png');classChangeover('ucHeader1_tablblVendor','toptabout')");
          }
          else
          {
            tabhlkVendor.Enabled = false;
            tablblVendor.CssClass = "toptabinactive";
          }
        }

        if (m_tabname == "Alternate Part")
        {
          tabhlkAlternatePart.NavigateUrl = "javascript:void(null)";
          tabimgAlternatePart.ImageUrl = "../images/tabbutton_down.png";
          tablblAlternatePart.CssClass = "toptabover";
        }
        else
        {
          if (m_mode == "edit")
          {
            tabhlkAlternatePart.NavigateUrl = "alternatepart.aspx?itemnum=" + m_nvc["itemnum"];
            tabhlkAlternatePart.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgAlternatePart','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblAlternatePart','toptabover')");
            tabhlkAlternatePart.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgAlternatePart','../images/tabbutton.png');classChangeover('ucHeader1_tablblAlternatePart','toptabout')");
          }
          else
          {
            tabhlkAlternatePart.Enabled = false;
            tablblAlternatePart.CssClass = "toptabinactive";
          }
        }
        if (m_tabname == "Open PO")
        {
          tabhlkOpenPO.NavigateUrl = "javascript:void(null)";
          tabimgOpenPO.ImageUrl = "../images/tabbutton_down.png";
          tablblOpenPO.CssClass = "toptabover";
        }
        else
        {
          if (m_mode == "edit")
          {
            tabhlkOpenPO.NavigateUrl = "openpo.aspx?itemnum=" + m_nvc["itemnum"];
            tabhlkOpenPO.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgOpenPO','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblOpenPO','toptabover')");
            tabhlkOpenPO.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgOpenPO','../images/tabbutton.png');classChangeover('ucHeader1_tablblOpenPO','toptabout')");
          }
          else
          {
            tabhlkOpenPO.Enabled = false;
            tablblOpenPO.CssClass = "toptabinactive";
          }
        }

        if (!Page.IsPostBack)
        {
            AzzierHeader.InitToolBar(toolbar, "Inventory", ShowToolbarButtonRights);
        }

        plhLinkDoc.Controls.Clear();
        litOperation.Text = m_operationlabel;
        LinkDoc links = new LinkDoc();
        linksrvr = links.LinkServer;
        DataTable alllinks = links.RetrieveLinkDoc("Inventory", m_itemnum);
        //if (alllinks.Rows.Count > 0)
        //{
        DropDownList lkddl = new DropDownList();
        RadComboBox lkrcb = new RadComboBox();
        Label lklbl = new Label();
        lklbl.Text = "Linked Documents: ";
        lklbl.Style["top"] = "0px";
        lklbl.Style["Position"] = "relative";

        plhLinkDoc.Controls.Add(lklbl);
        for (int i = 0; i < alllinks.Rows.Count; i++)
        {
            //Button button = new Button();
            //ListItem button = new ListItem();
            RadComboBoxItem rcbitem = new RadComboBoxItem();
            string linkurl = alllinks.Rows[i]["LinkURL"].ToString();
            string linktype = alllinks.Rows[i]["LinkType"].ToString();
            if (linktype == "http")
            {
                if (linkurl.IndexOf("ftp://") < 0 && linkurl.IndexOf("http://") < 0 && linkurl.IndexOf("https://") < 0 && linkurl.IndexOf(@"news://") < 0)
                {
                    linkurl = "http://" + linkurl;
                }
            }
            linkurl = linktype + "~" + linkurl;

            rcbitem.Text = alllinks.Rows[i]["LinkTitle"].ToString();
            rcbitem.Attributes.Add("onclick", "javascript:openlink('" + linkurl + "','" + alllinks.Rows[i]["SubFolder"].ToString() + "')");
            //ddl.Items.Add(button);
            rcbitem.ToolTip = alllinks.Rows[i]["Description"].ToString();
            lkrcb.Items.Add(rcbitem);

        }
        lkrcb.AllowCustomText = false;

        lkrcb.Style["Top"] = "0px";
        lkrcb.Style["Position"] = "relative";
        lkrcb.Style["height"] = "23px";
        //plhToolbar.Controls.Add(ddl);
        if (lkrcb.Items.Count > 0)
        {
            lkrcb.EmptyMessage = "...Select...";
        }
        else
        {
            lkrcb.EmptyMessage = "None";
        }
        plhLinkDoc.Controls.Add(lkrcb);
        //}
        //ImageButton managelinks = new ImageButton();
        //managelinks.ImageUrl = "../images/folder_24.png";
        //managelinks.BackColor = System.Drawing.Color.Transparent;
        //managelinks.Height = Unit.Pixel(20);
        //managelinks.OnClientClick = "managelink();return false;";
        //managelinks.Style["Position"] = "relative";
        //managelinks.Style["Top"] = "0px";
        //plhLinkDoc.Controls.Add(managelinks);



        plhReports.Controls.Clear();
        ReportBuilder objrpt = new ReportBuilder(Session["Login"].ToString(), "TeroReport", "Counter");
        DataTable allreports = objrpt.GetReportsByModule("Inventory");
        //DataTable allreports = objrpt.GetReportsByModule("labour");//testing code
        //DataTable allreports = new DataTable();
        //if (allreports.Rows.Count > 0)
        //{
        DropDownList rptddl = new DropDownList();
        RadComboBox rptrcb = new RadComboBox();
        Label rptlbl = new Label();
        rptlbl.Text = "Reports: ";
        rptlbl.Style["top"] = "0px";
        rptlbl.Style["Position"] = "relative";

        plhReports.Controls.Add(rptlbl);
        for (int i = 0; i < allreports.Rows.Count; i++)
        {
            RadComboBoxItem rcbitem = new RadComboBoxItem();
            string rptname = allreports.Rows[i]["ReportName"].ToString();
            string rptcounter = allreports.Rows[i]["Counter"].ToString();

            rcbitem.Text = rptname;
            rcbitem.Attributes.Add("onclick", "javascript:openreport('" + rptcounter + "')");
            rcbitem.ToolTip = allreports.Rows[i]["ReportDesc"].ToString();
            rptrcb.Items.Add(rcbitem);

        }
        rptrcb.AllowCustomText = false;

        rptrcb.Style["Top"] = "0px";
        rptrcb.Style["Position"] = "relative";
        rptrcb.Style["height"] = "23px";
        rptrcb.Width = Unit.Percentage(50);
        //plhToolbar.Controls.Add(ddl);
        if (rptrcb.Items.Count > 0)
        {
            rptrcb.EmptyMessage = "...Select...";
        }
        else
        {
            rptrcb.EmptyMessage = "None";
        }
        plhReports.Controls.Add(rptrcb);



        if (Page.IsPostBack)
        {
            if (Request.Form["__EVENTTARGET"] == "lookup")
            {
                lbnLookUpInventory_Click(null, null);
            }
        }
    }
    protected void toolbarcombobox_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        if (e.Value == "lookup")
          lbnLookUpInventory_Click(null, null);
        toolbarcombobox.SelectedIndex = 0;
    }

    protected void lbnLookUpInventory_Click(object sender, EventArgs e)
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        if (CntlPanel != null)
        {
            screen = new AzzierScreen("inventory/invmain.aspx", "MainForm", CntlPanel.Controls);
            screen.LCID = Session.LCID;
            nvc = screen.CollectFormValues("items", true);  // use the view name
            //m_division = nvc["division"];
        }
        else
            nvc = null;
        Items obj = new Items(Session["Login"].ToString(), "Items", "Itemnum");   // use the view name
        string wherestring = "", linqwherestring = "";
        //string[] empids = obj.Query(nvc, ref wherestring);
        obj.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
        string jscript = "<script type=\"text/javascript\">";
        jscript += "var wherestring = '" + Server.UrlEncode(linqwherestring) + "'; var obj=parent.controlpanel.document.location.href='invpanel.aspx?showlist=queryresult&wherestring='+wherestring;";

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
            case "addtostore":
                if (m_mode == "edit")
                  if (dr["urEdit"].ToString() != "")
                    if (Convert.ToInt32(dr["urEdit"].ToString()) > 0)
                      show = true;
                break;
            case "issue":
                show = true;
                break;
            case "return":
                show = true;
                break;
            case "reserve":
                show = true;
                break;
            case "editissue":
                show = true;
                break;
            case "stagingissue":
                show = true;
                break;
            case "template":
                if (m_tabname == "Specifications")
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
    private void RetrieveMessage()
    {
        SystemMessage msg = new SystemMessage("inventory/invheader.ascx");
        m_msg = msg.GetSystemMessage();
    }

}