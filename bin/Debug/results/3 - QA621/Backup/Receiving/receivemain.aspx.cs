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

public partial class receiving_receivemain : System.Web.UI.Page
{
    AzzierScreen screen;
    private string connstring;
    protected string querymode = "query";

    //NameValueCollection nvcitems;
    protected NameValueCollection m_msg = new NameValueCollection();
    protected NameValueCollection m_rights;
    protected int m_allowedit = 0;

    protected void Page_Init(object sender, EventArgs e)
    {
        RetrieveMessage();
        if (Session["Login"] == null)
        {
            Response.Write("<script>alert('" + m_msg["T1"] + "');top.document.location.href='../Login.aspx';</script>");
            Response.End();
        }
        UserRights r = new UserRights(Session["Login"].ToString(), "UserRights", "counter");
        m_rights = r.GetRights(Session["Login"].ToString(), "receiving");

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

        hidMode.Value = querymode;
        connstring = Application["ConnString"].ToString();
        InitScreen();
    }

    private void InitScreen()
    {
        screen = new AzzierScreen("receiving/receivemain.aspx", "MainForm", MainControlsPanel.Controls, querymode);
        Session.LCID = Convert.ToInt16(Session["LCID"]);
        screen.LCID = Session.LCID;
        
        screen.LoadScreen();

        //if (!isquery)
            //screen.SetValidationControls();
    }

    

    protected void Page_Load(object sender, EventArgs e)
    {
      if (!Page.IsPostBack)
      {
        RadioButtonList r = (RadioButtonList)MainControlsPanel.FindControl("rblisservice");
        r.Style.Add("valign", "top");
        ListItem litm;
        if (r != null)
        {
          r.RepeatDirection = RepeatDirection.Horizontal;
          litm = new ListItem("Yes", "1");
          //litm.Selected = true;
          r.Items.Add(litm);
          litm = new ListItem("No", "0");
          r.Items.Add(litm);
          litm = new ListItem("All", "");
          r.Items.Add(litm);
          r.SelectedIndex = 2;
        }

        r = (RadioButtonList)MainControlsPanel.FindControl("rblisinventory");
        r.Style.Add("valign", "top");
        if (r != null)
        {
          r.RepeatDirection = RepeatDirection.Horizontal;
          litm = new ListItem("Yes", "1");
          //litm.Selected = true;
          r.Items.Add(litm);
          litm = new ListItem("No", "0");
          r.Items.Add(litm);
          litm = new ListItem("All", "");
          r.Items.Add(litm);
          r.SelectedIndex = 2;
        }

        r = (RadioButtonList)MainControlsPanel.FindControl("rblserialized");
        r.Style.Add("valign", "top");
        if (r != null)
        {
          r.RepeatDirection = RepeatDirection.Horizontal;
          litm = new ListItem("Yes", "1");
          //litm.Selected = true;
          r.Items.Add(litm);
          litm = new ListItem("No", "0");
          r.Items.Add(litm);
          litm = new ListItem("All", "");
          r.Items.Add(litm);
          r.SelectedIndex = 2;
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
        //ucHeader1.ModuleData = nvcitems;

        string oper = "";
        if (querymode == "query")
        {
            oper = "Query";
        }
        else if (querymode == "new")
        {
            oper = "New";
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
        nvc = screen.CollectFormValues("v_ReceivingPOLine", true);
        ModuleoObject obj = new ModuleoObject(Session["Login"].ToString(), "v_ReceivingPOLine", "Counter");

        string wherestring = "", wherestringlinq = "";
        //string[] lines = obj.LinqQuery(nvc, ref wherestring,ref wherestringlinq);
        obj.CreateLinqCondition(nvc, ref wherestring, ref wherestringlinq);
        string jscript = "";
        jscript += "var wherestring = '" + Server.UrlEncode(wherestringlinq) + "'; var obj=parent.controlpanel.document.location.href='receivepanel.aspx?showlist=queryresult&wherestring='+wherestring;";
        litScript1.Text = jscript;
    }

    private void RetrieveMessage()
    {
        SystemMessage msg = new SystemMessage("receiving/receivemain.aspx");
        m_msg = msg.GetSystemMessage();
        msg.SetJsMessage(litMessage);
    }
}