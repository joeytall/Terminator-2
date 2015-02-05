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

public partial class vendor_vendorheader : System.Web.UI.UserControl
{
    private string m_mode;
    private string m_tabname;
    private string m_operationlabel;
    // private string m_empid = "";
    public string m_vendor = "";
    private NameValueCollection m_nvc;
    private AzzierScreen screen;
    protected string linksrvr = "";
    private int m_AllowEdit;
    protected RadComboBox toolbarcombobox;
    private string m_division;
    protected int m_masterrec;
    protected int defaultprintcounter = AzzierPrintForm.GetDefaultReport("vendor");
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
            m_vendor = m_nvc["companycode"];
            m_division = m_nvc["division"];
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
            tabhlkMain.NavigateUrl = "vendormain.aspx?mode=edit&vendor=" + m_nvc["companycode"];
            tabhlkMain.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgMain','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblMain','toptabover')");
            tabhlkMain.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgMain','../images/tabbutton.png');classChangeover('ucHeader1_tablblMain','toptabout')");
        }

        if (m_tabname == "itemvendor")
        {
            tabhlkVendor.NavigateUrl = "javascript:void(null)";
            tabimgSpecs.ImageUrl = "../images/tabbutton_down.png";
            tablblSpecs.CssClass = "toptabover";
        }
        else
        {
            if (m_mode == "edit")
            {
                tabhlkVendor.NavigateUrl = "vendoritem.aspx?vendor=" + m_nvc["companycode"];
                //tabhlkVendor.NavigateUrl = "vendorhistory.aspx?vendor=" + m_nvc["companycode"];
                tabhlkVendor.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgSpecs','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblSpecs','toptabover')");
                tabhlkVendor.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgSpecs','../images/tabbutton.png');classChangeover('ucHeader1_tablblSpecs','toptabout')");
            }
            else
            {
                tabhlkVendor.Enabled = false;
                tablblSpecs.CssClass = "toptabinactive";
            }
        }

        if (m_tabname == "contacts")
        {
            tabhlkContacts.NavigateUrl = "javascript:void(null)";
            tabimgContacts.ImageUrl = "../images/tabbutton_down.png";
            tablblContacts.CssClass = "toptabover";
        }
        else
        {
            if (m_mode == "edit")
            {
                tabhlkContacts.NavigateUrl = "vendorcontacts.aspx?vendor=" + m_nvc["companycode"];
                tabhlkContacts.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgContacts','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblContacts','toptabover')");
                tabhlkContacts.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgContacts','../images/tabbutton.png');classChangeover('ucHeader1_tablblContacts','toptabout')");
            }
            else
            {
                tabhlkContacts.Enabled = false;
                tablblContacts.CssClass = "toptabinactive";
            }
        }

        if (m_tabname == "services")
        {
            tabhlkServices.NavigateUrl = "javascript:void(null)";
            tabimgServices.ImageUrl = "../images/tabbutton_down.png";
            tablblServices.CssClass = "toptabover";
        }
        else
        {
            if (m_mode == "edit")
            {
                tabhlkServices.NavigateUrl = "vendorsrvc.aspx?vendor=" + m_nvc["companycode"];
                tabhlkServices.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgServices','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblServices','toptabover')");
                tabhlkServices.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgServices','../images/tabbutton.png');classChangeover('ucHeader1_tablblServices','toptabout')");
            }
            else
            {
                tabhlkServices.Enabled = false;
                tablblServices.CssClass = "toptabinactive";
            }
        }

        if (m_tabname == "Hierarchy")
        {
            tabhlkBranch.NavigateUrl = "javascript:void(null)";
            tabimgBranch.ImageUrl = "../images/tabbutton_down.png";
            tablblBranch.CssClass = "toptabover";
        }
        else
        {
            if (m_mode == "edit")
            {
                tabhlkBranch.NavigateUrl = "vendortree.aspx?vendor=" + m_nvc["companycode"];
                tabhlkBranch.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgBranch','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblBranch','toptabover')");
                tabhlkBranch.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgBranch','../images/tabbutton.png');classChangeover('ucHeader1_tablblBranch','toptabout')");
            }
            else
            {
                tabhlkBranch.Enabled = false;
                tablblBranch.CssClass = "toptabinactive";
            }

        }

        if (!Page.IsPostBack)
        {
            AzzierHeader.InitToolBar(toolbar, "vendor", ShowToolbarButtonRights);
        }

        litOperation.Text = m_operationlabel;
        AzzierHeader.CreateLinksDropDown(plhLinkDoc, "vendor", m_vendor, out linksrvr);
        AzzierHeader.CreateReportsDropDown(plhReports, "vendor");
        AzzierHeader.CreateQueryDropDown(plhQuery, "vendor");

        if (Page.IsPostBack)
        {
            if (Request.Form["__EVENTTARGET"] == "save")
            {
                SaveVendor();
            }
            if (Request.Form["__EVENTTARGET"] == "lookup")
            {
                LookUpVendor();
            }
            if (Request.Form["__EVENTTARGET"] == "delete")
            {
                DeleteVendor();
            }
        }
    }

    protected void DeleteVendor()
    {
        Vendor vendor = new Vendor(Session["Login"].ToString(), "vendor", "companycode", m_vendor);
        //Location loc = new Location(Session["Login"].ToString(), "Location", "Location", m_vendor);
        //loc.Delete();
        vendor.Delete();
        string jscript = "<script type=\"text/javascript\">";
        jscript += "setTimeout(parent.controlpanel.RefreshPanelGridsOnDelete(), 0); document.location.href='vendormain.aspx';";
        jscript += "</script>";
        litFrameScript.Text = jscript;
    }

    protected void LookUpVendor()
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        if (CntlPanel != null)
        {
            screen = new AzzierScreen("vendor/vendormain.aspx", "MainForm", CntlPanel.Controls);
            screen.LCID = Session.LCID;
            nvc = screen.CollectFormValues("vendor", true);  // use the view name
            //m_division = nvc["division"];
        }
        else
            nvc = null;
        Vendor obj = new Vendor(Session["Login"].ToString(), "vendor", "companycode");
        //Location obj = new Location(Session["Login"].ToString(), "Location", "location");   // use the view name
        string wherestring = "", wherestringlinq = "";

        //string[] locations = obj.LinqQuery(nvc, ref wherestring, ref wherestringlinq);
        obj.CreateLinqCondition(nvc, ref wherestring, ref wherestringlinq);
        string jscript = "<script type=\"text/javascript\">";
        //int len = locations.Length;
        jscript += "var wherestring = '" + Server.UrlEncode(wherestringlinq) + "';var obj=parent.controlpanel.document.location.href='vendorpanel.aspx?showlist=queryresult&wherestring='+wherestring;";
        jscript += "</script>";
        litFrameScript.Text = jscript;
    }

    private void SaveVendor()
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        TextBox tbx = null;
        string dirtylog = "0";
        if (CntlPanel != null)
        {
            screen = new AzzierScreen("vendor/vendormain.aspx", "MainForm", CntlPanel.Controls);
            screen.LCID = Session.LCID;
            nvc = screen.CollectFormValues("Vendor", false);  // use the table name
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

        Vendor vendor;
        //Location vendor;
        bool success = false;
        if (m_mode == "new")
        {
            vendor = new Vendor(Session["Login"].ToString(), "Vendor", "companycode");   // use the table name
            string t1 = nvc["companycode"];
            success = vendor.Create(nvc);
        }
        else if (m_mode == "duplicate")
        {
            //string newvendor, oldvendor;
            vendor = new Vendor(Session["Login"].ToString(), "Vendor", "companycode");
            //HiddenField hid;
            //hid = CntlPanel.FindControl("HidOldLocation") as HiddenField;
            //if (hid != null)
            //{
            //oldvendor = hid.Value;
            //newvendor = nvc["companycode"];

            success = vendor.Create(nvc);
            //}
        }
        else if (m_mode == "edit")
        {
            string vendorstr = nvc["companycode"];
            vendor = new Vendor(Session["Login"].ToString(), "Vendor", "companycode", vendorstr);   // use the table name
            success = vendor.Update(nvc);
        }
        else
            vendor = new Vendor(Session["Login"].ToString(), "Vendor", "companycode");

        if (!success)
        {
            string jscript = "<script>alert(\"" + vendor.ErrorMessage.Replace("\r\n", "") + "\");</script>";
            litFrameScript.Text = jscript;
        }
        else
        {
            if (m_mode == "new")
                Response.Redirect("vendormain.aspx?mode=edit&vendor=" + Server.UrlEncode(nvc["companycode"]));
            else
                if (m_mode == "duplicate")
                    Response.Redirect("vendormain.aspx?mode=edit&vendor=" + Server.UrlEncode(nvc["companycode"]));
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
        Division d = new Division();
        bool alloweditdiv = d.Editable("'" + m_division + "'");
        bool allowmaster = false;
        if (dr["urMaster"].ToString() != "")
            if (Convert.ToInt32(dr["urAddNew"].ToString()) != 0)
                allowmaster = true;
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
            case "autovendor":
                if (dr["urAddNew"].ToString() != "")
                  if (Convert.ToInt32(dr["urAddNew"].ToString()) != 0)
                    show = true;
                break;
            case "save":
                if (m_masterrec == 1 && allowmaster == false)
                {
                    show = false;
                }
                else
                {
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
                }
                break;
            case "duplicate":
              /*
               * removed as in web work there is no duplicate vendor.
                if (m_mode == "edit")
                    if (dr["urAddNew"].ToString() != "")
                        if (Convert.ToInt32(dr["urAddNew"].ToString()) > 0)
                            show = true;
               * */
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
                if (m_masterrec == 1 && allowmaster == false)
                {
                    show = false;
                }
                else
                {
                    if (m_tabname == "Main" && m_mode == "edit")
                    {
                        if (dr["urDelete"].ToString() != "")
                            if (Convert.ToInt32(dr["urDelete"]) != 0 && alloweditdiv)
                                show = true;
                    }
                }
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