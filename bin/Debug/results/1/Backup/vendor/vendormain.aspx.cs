using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;
using Telerik.Web.UI;
using System.Configuration;
using System.Linq;
using System.Globalization;

public partial class vendor_vendormain : System.Web.UI.Page
{
    AzzierScreen screen;
    private string connstring;
    protected string querymode = "";
    protected string m_vendor, m_oldvendor = "";

    Vendor objVendor;
    NameValueCollection nvcVendor;
    //protected DateTime mydate;
    protected bool isquery = false;
    protected int statuscode = 0;
    protected NameValueCollection m_msg = new NameValueCollection();
    protected NameValueCollection m_rights;
    protected int m_allowedit = 0;
    protected string t_duplicate;

    protected void Page_Init(object sender, EventArgs e)
    {
        RetrieveMessage();
        UserRights.CheckAccess("vendor");
        UserRights r = new UserRights(Session["Login"].ToString(), "UserRights", "counter");
        m_rights = r.GetRights(Session["Login"].ToString(), "vendor");

        m_allowedit = Convert.ToInt16(m_rights["urEdit"].ToString());

        Session.LCID = Convert.ToInt32(Session["LCID"]);
        if (Request.QueryString["mode"] != null)
            querymode = Request.QueryString["mode"].ToString();
        else
            querymode = "query";

        if (Request.QueryString["vendor"] != null)
        {
            m_vendor = Request.QueryString["vendor"].ToString();
            m_oldvendor = Request.QueryString["vendor"].ToString();
           HidOldVendor.Value = Request.QueryString["vendor"].ToString();
        }
        else
            m_vendor = "";

        if (querymode == "query")
            isquery = true;

        if (querymode == "edit")
        {
            objVendor = new Vendor(Session["Login"].ToString(), "vendor", "companycode", m_vendor);

            nvcVendor = objVendor.ModuleData;
            if (nvcVendor["companycode"].ToString() == "")
            {
              querymode = "query";
              isquery = true;
              hidMode.Value = querymode;
              m_vendor = "";
            }

            UserWorkList list = new UserWorkList();
            list.AddToRecentList(Session["Login"].ToString(), "Vendor", m_vendor);
        }
        else if (querymode == "duplicate" && m_oldvendor.Length>0)
        {
            objVendor = new Vendor(Session["Login"].ToString(), "Vendor", "companycode", m_oldvendor);

            nvcVendor = objVendor.ModuleData;
            //UserWorkList list = new UserWorkList();
            //list.AddToRecentList(Session["Login"].ToString(), "Labour", m_oldempid);
            litScript1.Text = "clearoldvendor()";
        }
        else
        {
            objVendor = new Vendor(Session["Login"].ToString(), "Vendor", "companycode");
            nvcVendor = objVendor.ModuleData;
            statuscode = 1;
        }
        hidMode.Value = querymode;
        connstring = Application["ConnString"].ToString();
        InitScreen();
    }

    private void InitScreen()
    {
        screen = new AzzierScreen("vendor/vendormain.aspx", "MainForm", MainControlsPanel.Controls, querymode);
        screen.LCID = Session.LCID;

        screen.LoadScreen();

        if (!isquery)
            screen.SetValidationControls();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string oper = "";
        if (!Page.IsPostBack)
        {
            if (querymode == "edit")
            {
                screen.PopulateScreen("Vendor", nvcVendor);
                HidDivision.Value = nvcVendor["division"];
            }
            if (querymode == "duplicate") 
            {
                screen.PopulateScreen("Vendor", nvcVendor);
            }

            if (!Page.IsPostBack && (querymode == "duplicate"))
            {
              NewNumber num = new NewNumber();
              TextBox text = MainControlsPanel.FindControl("txtcompanycode") as TextBox;
              text.Text = num.GetNextNumber("Vendor");
            }
            
            
            RadioButtonList r = (RadioButtonList)MainControlsPanel.FindControl("rblmasterrec");
            
            if (r != null)
            {
              r.Style.Add("valign", "top");
              r.RepeatDirection = RepeatDirection.Horizontal;
              ListItem litm1 = new ListItem("Yes", "1");
              r.Items.Add(litm1);
              ListItem litm2 = new ListItem("No", "0");
              r.Items.Add(litm2);
              if (querymode == "query")
              {
                ListItem litm3 = new ListItem("All", "2");
                r.Items.Add(litm3);
                r.SelectedIndex = 2;
              }
              else if (querymode == "edit")
              {
                if (nvcVendor["masterrec"].ToString() == "1")
                    r.SelectedIndex = 0;
                  else
                    r.SelectedIndex = 1;
              }
              else
              {
                r.SelectedIndex = 1;
              }
            }          

            r = (RadioButtonList)MainControlsPanel.FindControl("rblinactive");
            if (r != null)
            {
              r.RepeatDirection = RepeatDirection.Horizontal;
              ListItem litm1 = new ListItem("Yes", "1");
              r.Items.Add(litm1);
              ListItem litm2 = new ListItem("No", "0");
              r.Items.Add(litm2);
              if (querymode == "query")
              {
                ListItem litm3 = new ListItem("All", "");
                r.Items.Add(litm3);
                r.SelectedIndex = 2;
              }
              else if (querymode == "edit")
              {
                if (nvcVendor["inactive"].ToString() == "1")
                  r.SelectedIndex = 0;
                else
                  r.SelectedIndex = 1;
                r.Attributes.Add("onclick", "roradiobutton('rblinactive'," + r.SelectedIndex.ToString() + ")");
              }
              else
              {
                r.SelectedIndex = 1;
                r.Attributes.Add("onclick", "roradiobutton('rblinactive',1");
              }
            }

            if (querymode == "query")
            {
                oper = "Query";
            }
            else if (querymode == "new")
            {
                oper = "New";
                screen.GetDefaultValue("Vendor", nvcVendor, "Vendor", "new");
                if (Request.QueryString["numbering"] == "auto")
                {
                  NewNumber numauto = new NewNumber();
                  nvcVendor["companycode"] = numauto.GetNextNumber("Vendor");
                }
                screen.PopulateScreen("Vendor", nvcVendor);
            }
            else if (querymode == "duplicate")
            {
                oper = "Duplicate";
            }
            else if (querymode == "edit")
            {
                oper = "Edit";
            }
        }
        else
        {
            if (Request.Form["__EVENTTARGET"] == "Query")
            {
                Query();
            }
            if (Request.Form["__EVENTTARGET"] == "ModifyVendor")
            {
              ModifyVendor();
            }
        }

        ucHeader1.Mode = querymode;
        ucHeader1.TabName = "Main";
        ucHeader1.ModuleData = nvcVendor;


        
        ucHeader1.OperationLabel = oper;
    }

    private void Query()
    {
        NameValueCollection nvc;
        nvc = screen.CollectFormValues("Vendor", true);
        Vendor objvendor = new Vendor(Session["Login"].ToString(), "Vendor", "companycode");

        string wherestring = "", linqwherestring = "";
       // string[] locations = objLocation.Query(nvc, ref wherestring);
        objvendor.CreateLinqCondition(nvc, ref wherestring,ref linqwherestring);
        string jscript = "";
        jscript += "var wherestring = '" + Server.UrlEncode(linqwherestring) + "';var obj=parent.controlpanel.document.location.href='vendorpanel.aspx?showlist=queryresult&wherestring='+wherestring;";
        litScript1.Text = jscript;
    }

    private Boolean ModifyVendor()
    {
      SaveVendor();
      string newdivision = hidNewDivision.Value;
      string newparentid = hidNewParentID.Value;
      string newstatus = hidNewStatus.Value;
      string newparentdesc = hidNewParentDesc.Value;
      string moddate = hidModDate.Value;
      //bool updateopen = true;
      //bool updatehistory = false;

      //if (hidUpdateOpen.Value == "true")
      //  updateopen = true;
      //else
      //  updateopen = false;

      //if (hidUpdateHistory.Value == "true")
      //  updatehistory = true;
      //else
      //  updatehistory = false;
      Vendor vendor = new Vendor(Session["Login"].ToString(),"Vendor","companycode",m_vendor);
      
      CultureInfo c = new CultureInfo(Session.LCID);
      DateTime TransDate = Convert.ToDateTime(moddate, c);
      
      litScript1.Text = "document.location.href='vendormain.aspx?mode=edit&vendor=" + m_vendor + "'";
      return true;
    }

    private Boolean SaveVendor()
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        TextBox tbx = null;
        string dirtylog = "0";
        if (CntlPanel != null)
        {
            nvc = screen.CollectFormValues("vendor", false);  // use the table name
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

        Vendor objVendor;
        bool success = false;
        if (querymode == "new")
        {
            objVendor = new Vendor(Session["Login"].ToString(), "Vendor", "companycode");
            success = objVendor.Create(nvc);
        }
        else if (querymode == "duplicate")
        {
            objVendor = new Vendor(Session["Login"].ToString(), "Vendor", "companycode");
            success = objVendor.Create(nvc);
        }
        else if (querymode == "edit")      //  else if (querymode == "edit" && m_allowedit == 1)
        {
            objVendor = new Vendor(Session["Login"].ToString(), "vendor", "companycode", nvc["vendor"]);
            success = objVendor.Update(nvc);
        }
        else
            objVendor = new Vendor(Session["Login"].ToString(), "vendor", "companycode");
        return success;
    }

    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
    }

    private void RetrieveMessage()
    {
        SystemMessage msg = new SystemMessage("location/locationmain.aspx");
        m_msg = msg.GetSystemMessage();
        msg.SetJsMessage(litMessage);
    }
}