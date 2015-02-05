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

public partial class labour_Labourmain : System.Web.UI.Page
{
    AzzierScreen screen;
    private string connstring;
    protected string querymode = "";
    protected string m_empid, m_oldempid = "";

    Employee objEmployee;
    NameValueCollection nvcEMP;
    protected DateTime mydate;
    protected bool isquery = false;
    protected int statuscode = 0;
    protected NameValueCollection m_msg = new NameValueCollection();
    protected NameValueCollection m_rights;
    protected int m_allowedit = 0;
    protected string t_duplicate;

    protected void Page_Init(object sender, EventArgs e)
    {
        RetrieveMessage();
        UserRights.CheckAccess("labour");
        UserRights r = new UserRights(Session["Login"].ToString(), "UserRights", "counter");
        m_rights = r.GetRights(Session["Login"].ToString(), "labour");

        m_allowedit = Convert.ToInt16(m_rights["urEdit"].ToString());

        Session.LCID = Convert.ToInt32(Session["LCID"]);
        if (Request.QueryString["mode"] != null)
        {
          querymode = Request.QueryString["mode"].ToString();
          if (querymode=="")
            querymode = "query";
        }
        else
          querymode = "query";

        if (Request.QueryString["empid"] != null)
        {
            m_empid = Request.QueryString["empid"].ToString();
            m_oldempid = Request.QueryString["empid"].ToString();
            HidOldempid.Value = Request.QueryString["empid"].ToString();
        }
        else
            m_empid = "";


        if (querymode == "query")
            isquery = true;

        if (querymode == "edit")
        {
            objEmployee = new Employee(Session["Login"].ToString(), "employee", "empid", m_empid);

            nvcEMP = objEmployee.ModuleData;
            if (nvcEMP["empid"].ToString() == "")
            {
              querymode = "query";
              isquery = true;
              m_empid = "";
            }
            UserWorkList list = new UserWorkList();
            list.AddToRecentList(Session["Login"].ToString(), "Labour", m_empid);
        }
        else if (querymode == "duplicate" && m_oldempid.Length>0)
        {
            objEmployee = new Employee(Session["Login"].ToString(), "employee", "empid", m_oldempid);

            nvcEMP = objEmployee.ModuleData;
            UserWorkList list = new UserWorkList();
            list.AddToRecentList(Session["Login"].ToString(), "Labour", m_oldempid);
            litScript1.Text = "clearoldempval()";
        }
        else
        {
            objEmployee = new Employee(Session["Login"].ToString(), "employee", "empid");
            nvcEMP = objEmployee.ModuleData;
            statuscode = 1;
        }
        hidMode.Value = querymode;
        connstring = Application["ConnString"].ToString();
        InitScreen();
    }

    private void InitScreen()
    {
        screen = new AzzierScreen("labour/labourmain.aspx", "MainForm", MainControlsPanel.Controls, querymode);
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
                screen.PopulateScreen("employee", nvcEMP);
                HidDivision.Value = nvcEMP["division"];
            }
            if (querymode == "duplicate") 
            {
                screen.PopulateScreen("employee", nvcEMP);
            }

            RadioButtonList r = (RadioButtonList)MainControlsPanel.FindControl("rblinactive");
            r.Style.Add("valign", "top");
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
                if (nvcEMP["inactive"].ToString() == "1")
                  r.SelectedIndex = 0;
                else
                  r.SelectedIndex = 1;
              }
              else
              {
                r.SelectedIndex = 1;
              }
            }

            r = (RadioButtonList)MainControlsPanel.FindControl("rblemailnotify");
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
                if (nvcEMP["emailnotify"].ToString() == "1")
                  r.SelectedIndex = 0;
                else
                  r.SelectedIndex = 1;
              }
              else
              {
                r.SelectedIndex = 1;
              }
            }

            r = (RadioButtonList)MainControlsPanel.FindControl("rblemployee");
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
                if (nvcEMP["employee"].ToString() == "1")
                  r.SelectedIndex = 0;
                else
                  r.SelectedIndex = 1;
              }
              else
              {
                r.SelectedIndex = 1;
              }
            }
        }
        else
        {
            if (Request.Form["__EVENTTARGET"] == "Query")
            {
                Query();
            }
        }

        ucHeader1.Mode = querymode;
        ucHeader1.TabName = "Main";
        ucHeader1.ModuleData = nvcEMP;

        string oper = "";
        if (querymode == "query")
        {
            oper = "Query";
        }
        else if (querymode == "new")
        {
            oper = "New";
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
        /*
        if (querymode == "edit")
        {
            TextBox tbx = MainControlsPanel.FindControl("txtempid") as TextBox;
            tbx.Attributes.Add("readonly", "readonly");
        }
         * */
    }

    private void Query()
    {
        NameValueCollection nvc;
        nvc = screen.CollectFormValues("employee", true);
        Employee objEmpolyee = new Employee(Session["Login"].ToString(), "employee", "empid");

        string wherestring = "", wherestringlinq = "";
        //string[] empids = objEmpolyee.Query(nvc, ref wherestring);
        objEmpolyee.CreateLinqCondition(nvc, ref wherestring,ref wherestringlinq);
        string jscript = "";
        jscript += "var wherestring = '" + Server.UrlEncode(wherestringlinq) + "';var obj=parent.controlpanel.document.location.href='labourpanel.aspx?showlist=queryresult&wherestring='+wherestring;";
        litScript1.Text = jscript;
    }

    private Boolean SaveLabour()
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        TextBox tbx = null;
        string dirtylog = "0";
        if (CntlPanel != null)
        {
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

        Employee objEmployee;
        bool success = false;
        if (querymode == "new")
        {
            objEmployee = new Employee(Session["Login"].ToString(), "employee", "empid");
            success = objEmployee.Create(nvc);
        }
        else if (querymode == "duplicate")
        {
            objEmployee = new Employee(Session["Login"].ToString(), "employee", "empid");
            success = objEmployee.Create(nvc);
        }
        else if (querymode == "edit")      //  else if (querymode == "edit" && m_allowedit == 1)
        {
            string erpid = nvc["empid"];
            objEmployee = new Employee(Session["Login"].ToString(), "employee", "empid", erpid);
        }
        else
            objEmployee = new Employee(Session["Login"].ToString(), "employee", "empid");
        return success;
    }

    private void SaveStatus()
    {
        SaveLabour();
    }

    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
    }

    private void RetrieveMessage()
    {
        SystemMessage msg = new SystemMessage("labour/labourmain.aspx");
        m_msg = msg.GetSystemMessage();
        msg.SetJsMessage(litMessage);
    }
}