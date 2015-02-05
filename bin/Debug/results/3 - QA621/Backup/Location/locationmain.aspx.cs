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

public partial class location_locationmain : System.Web.UI.Page
{
    AzzierScreen screen;
    private string connstring;
    protected string querymode = "";
    protected string m_location, m_oldlocation = "";

    Location objLocation;
    NameValueCollection nvcLocation;
    //protected DateTime mydate;
    protected bool isquery = false;
    protected int statuscode = 0;
    protected NameValueCollection m_msg = new NameValueCollection();
    protected NameValueCollection m_rights;
    protected int m_allowedit = 0;
    protected string t_duplicate;
    protected string m_deletemessage = "";

    protected void Page_Init(object sender, EventArgs e)
    {
        RetrieveMessage();
        UserRights.CheckAccess("location");
        UserRights r = new UserRights(Session["Login"].ToString(), "UserRights", "counter");
        m_rights = r.GetRights(Session["Login"].ToString(), "location");

        m_allowedit = Convert.ToInt16(m_rights["urEdit"].ToString());

        Session.LCID = Convert.ToInt32(Session["LCID"]);
        if (Request.QueryString["mode"] != null)
            querymode = Request.QueryString["mode"].ToString();
        else
            querymode = "query";

        if (Request.QueryString["location"] != null)
        {
            m_location = Request.QueryString["location"].ToString();
            m_oldlocation = Request.QueryString["location"].ToString();
            HidOldLocation.Value = Request.QueryString["location"].ToString();
        }
        else
            m_location = "";

        if (querymode == "query")
            isquery = true;

        if (querymode == "edit")
        {
            objLocation = new Location(Session["Login"].ToString(), "Location", "location", m_location);
            nvcLocation = objLocation.ModuleData;
            if (objLocation.AllowDelete() != 0)
                m_deletemessage = "This Location is on open workorders and cannot be deleted at this time.";
            if (nvcLocation["location"].ToString() == "")
            {
              querymode = "query";
              isquery = true;
              hidMode.Value = querymode;
              m_location = "";
            }

            UserWorkList list = new UserWorkList();
            list.AddToRecentList(Session["Login"].ToString(), "Location", m_location);
        }
        else if (querymode == "duplicate" && m_oldlocation.Length>0)
        {
            objLocation = new Location(Session["Login"].ToString(), "Location", "location", m_oldlocation);

            nvcLocation = objLocation.ModuleData;
            UserWorkList list = new UserWorkList();
            //list.AddToRecentList(Session["Login"].ToString(), "Labour", m_oldempid);
            litScript1.Text = "clearoldlocationval()";
        }
        else
        {
            objLocation = new Location(Session["Login"].ToString(), "Location", "Location");
            nvcLocation = objLocation.ModuleData;
            statuscode = 1;
        }
        hidMode.Value = querymode;
        connstring = Application["ConnString"].ToString();
        InitScreen();
    }

    private void InitScreen()
    {
        screen = new AzzierScreen("location/locationmain.aspx", "MainForm", MainControlsPanel.Controls, querymode);
        screen.LCID = Session.LCID;
        //screen.Top = 20;

        screen.LoadScreen();

        if (!isquery)
            screen.SetValidationControls();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (querymode == "edit")
            {
                screen.PopulateScreen("Location", nvcLocation);
                HidDivision.Value = nvcLocation["division"];
            }
            if (querymode == "duplicate") 
            {
                screen.PopulateScreen("Location", nvcLocation);
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
                ListItem litm3 = new ListItem("All", "");
                r.Items.Add(litm3);
                r.SelectedIndex = 2;
              }
              else if (querymode == "edit")
              {
                if (nvcLocation["masterrec"].ToString() == "1")
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
                if (nvcLocation["inactive"].ToString() == "1")
                  r.SelectedIndex = 0;
                else
                  r.SelectedIndex = 1;
                r.Attributes.Add("onclick", "roradiobutton('rblinactive'," + r.SelectedIndex.ToString() + ")");
              }
              else
              {
                r.SelectedIndex = 1;
                r.Attributes.Add("onclick", "roradiobutton('rblinactive',1)");
              }
            }          
        }
        else
        {
            if (Request.Form["__EVENTTARGET"] == "Query")
            {
                Query();
            }
            if (Request.Form["__EVENTTARGET"] == "ModifyLocation")
            {
              ModifyLocation();
            }
        }

        ucHeader1.Mode = querymode;
        ucHeader1.TabName = "Main";
        ucHeader1.ModuleData = nvcLocation;

        string oper = "";
        if (querymode == "query")
        {
            oper = "Query";
        }
        else if (querymode == "new")
        {
            oper = "New";
            screen.GetDefaultValue("Location", nvcLocation, "Location", "new");
            screen.PopulateScreen("Location", nvcLocation);
        }
        else if (querymode == "duplicate")
        {
            oper = "Duplicate";
        }
        else if (querymode == "edit")
        {
            oper = "Edit";
        }
        ucHeader1.OperationLabel = oper;
    }

    private void Query()
    {
        NameValueCollection nvc;
        nvc = screen.CollectFormValues("Location", true);
        Location objLocation = new Location(Session["Login"].ToString(), "Location", "location");

        string wherestring = "", linqwherestring = "";
       // string[] locations = objLocation.Query(nvc, ref wherestring);
        objLocation.CreateLinqCondition(nvc, ref wherestring,ref linqwherestring);
        string jscript = "";
        jscript += "var wherestring = '" + Server.UrlEncode(linqwherestring) + "';var obj=parent.controlpanel.document.location.href='locationpanel.aspx?showlist=queryresult&wherestring='+wherestring;";
        litScript1.Text = jscript;
    }

    private Boolean ModifyLocation()
    {
      SaveLocation();
      string newdivision = hidNewDivision.Value;
      string newparentid = hidNewParentID.Value;
      string newstatus = hidNewStatus.Value;
      string newparentdesc = hidNewParentDesc.Value;
      string moddate = hidModDate.Value;
      bool updateopen = true;
      bool updatehistory = false;

      if (hidUpdateOpen.Value == "true")
        updateopen = true;
      else
        updateopen = false;

      if (hidUpdateHistory.Value == "true")
        updatehistory = true;
      else
        updatehistory = false;

      Location loc = new Location(Session["Login"].ToString(), "Location", "Location", m_location);
      CultureInfo c = new CultureInfo(Session.LCID);
      DateTime TransDate = Convert.ToDateTime(moddate, c);
      loc.ModifyLocation(newparentid,hidNewParentDesc.Value, newdivision, newstatus,updateopen,updatehistory,TransDate);
      litScript1.Text = "document.location.href='locationmain.aspx?mode=edit&location=" + m_location + "'";
      return true;
    }

    private Boolean SaveLocation()
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        TextBox tbx = null;
        string dirtylog = "0";
        if (CntlPanel != null)
        {
            nvc = screen.CollectFormValues("location", false);  // use the table name
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

        Location objLocation;
        bool success = false;
        if (querymode == "new")
        {
            objLocation = new Location(Session["Login"].ToString(), "Location", "location");
            success = objLocation.Create(nvc);
        }
        else if (querymode == "duplicate")
        {
            objLocation = new Location(Session["Login"].ToString(), "Location", "location");
            success = objLocation.Create(nvc);
        }
        else if (querymode == "edit")      //  else if (querymode == "edit" && m_allowedit == 1)
        {
            objLocation = new Location(Session["Login"].ToString(), "Location", "Location", nvc["location"]);
            success = objLocation.Update(nvc);
        }
        else
            objLocation = new Location(Session["Login"].ToString(), "Location", "location");
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