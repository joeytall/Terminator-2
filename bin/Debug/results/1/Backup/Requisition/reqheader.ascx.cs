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

public partial class requisition_reqheader : System.Web.UI.UserControl
{
  protected string m_mode;
  protected string m_tabname;
  private string m_operationlabel;
  public string m_reqnum = "";
  private NameValueCollection m_nvc;
  private AzzierScreen screen;
  protected string linksrvr = "";
  private int m_AllowEdit;
  protected RadComboBox toolbarcombobox;
  private string m_division;
  protected int m_masterrec;
  protected string m_iskeyrequest = "0";
  protected int m_statuscode = 0;
  protected int defaultprintcounter = -1;
  protected NameValueCollection m_msg = new NameValueCollection();
  protected bool keyapproved = false;
  protected int mainmodul_ScrnSize =0, tmp_ScrnIconNum = 0, tmp_ActIconNum = 0;

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
      m_reqnum = m_nvc["reqnum"];
      m_division = m_nvc["division"];
      m_iskeyrequest = m_nvc["IsKeyRequest"];
      if (m_nvc["statuscode"] != "")
        m_statuscode = Convert.ToInt16(m_nvc["StatusCode"]);
      if (m_nvc["DeptApprovalId"].Trim() != "" || m_nvc["DivisionApprovalId"].Trim() != "" || m_nvc["AuthorizationId"].Trim() != "")
          keyapproved = true;
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
      tabhlkMain.NavigateUrl = "reqmain.aspx?mode=edit&reqnum=" + m_nvc["reqnum"];
      tabhlkMain.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgMain','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblMain','toptabover')");
      tabhlkMain.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgMain','../images/tabbutton.png');classChangeover('ucHeader1_tablblMain','toptabout')");
    }

    if (m_tabname == "accounts")
    {
      tabhlkAccounts.NavigateUrl = "javascript:void(null)";
      tabimgAccounts.ImageUrl = "../images/tabbutton_down.png";
      tablblAccounts.CssClass = "toptabover";
    }
    else
    {
      if (m_mode == "edit")
      {
        tabhlkAccounts.NavigateUrl = "reqaccounts.aspx?reqnum=" + m_nvc["reqnum"];
        tabhlkAccounts.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgAccounts','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblAccounts','toptabover')");
        tabhlkAccounts.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgAccounts','../images/tabbutton.png');classChangeover('ucHeader1_tablblAccounts','toptabout')");
      }
      else
      {
        tabhlkAccounts.Enabled = false;
        tablblAccounts.CssClass = "toptabinactive";
      }
    }

    if (m_tabname == "history")
    {
      tabhlkHistory.NavigateUrl = "javascript:void(null)";
      tabimgHistory.ImageUrl = "../images/tabbutton_down.png";
      tablblHistory.CssClass = "toptabover";
    }
    else
    {
      if (m_mode == "edit")
      {
        tabhlkHistory.NavigateUrl = "reqhistory.aspx?reqnum=" + m_nvc["reqnum"];
        tabhlkHistory.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgHistory','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblHistory','toptabover')");
        tabhlkHistory.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgHistory','../images/tabbutton.png');classChangeover('ucHeader1_tablblHistory','toptabout')");
      }
      else
      {
        tabhlkHistory.Enabled = false;
        tablblHistory.CssClass = "toptabinactive";
      }
    }

    if (m_tabname == "keyrequest")
    {
        tabhlkKey.NavigateUrl = "javascript:void(null)";
        tabimgKey.ImageUrl = "../images/tabbutton_down.png";
        tablblKey.CssClass = "toptabover";
    }
    else
    {
        if (m_mode == "edit")
        {
            tabhlkKey.NavigateUrl = "keyrequest.aspx?reqnum=" + m_nvc["reqnum"];
            tabhlkKey.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgKey','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblKey','toptabover')");
            tabhlkKey.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgKey','../images/tabbutton.png');classChangeover('ucHeader1_tablblKey','toptabout')");
        }
        else
        {
            tabhlkHistory.Enabled = false;
            tablblHistory.CssClass = "toptabinactive";
        }
    }

    if (!Page.IsPostBack)
    {
      AzzierHeader.InitToolBar(toolbar, "requisition");
    }
    litOperation.Text = m_operationlabel;
    AzzierHeader.CreateLinksDropDown(plhLinkDoc, "requisition", m_reqnum, out linksrvr);
    AzzierHeader.CreateReportsDropDown(plhReports, "requisition");
    AzzierHeader.CreateQueryDropDown(plhQuery, "requisition");

    defaultprintcounter = AzzierPrintForm.GetDefaultReport("Requisition");

    if (Page.IsPostBack)
    {
      if (Request.Form["__EVENTTARGET"] == "save")
      {
        SaveReq();
      }
      if (Request.Form["__EVENTTARGET"] == "lookup")
      {
        LookUpReq();
      }
      if (Request.Form["__EVENTTARGET"] == "delete")
      {
        DeleteReq();
      }
    }
  }

  protected void DeleteReq()
  {
    ModuleoObject objReq = new ModuleoObject(Session["Login"].ToString(), "itemrequest", "reqnum", m_reqnum);
    objReq.Delete();
    string jscript = "<script type=\"text/javascript\">";
    jscript += "setTimeout(parent.controlpanel.RefreshPanelGridsOnDelete(), 0); document.location.href='reqmain.aspx';";
    jscript += "</script>";
    litFrameScript.Text = jscript;
  }

  protected void LookUpReq()
  {
    NameValueCollection nvc;
    Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
    if (CntlPanel != null)
    {
      screen = new AzzierScreen("requisition/reqmain.aspx", "MainForm", CntlPanel.Controls);
      screen.LCID = Session.LCID;
      nvc = screen.CollectFormValues("itemrequest", true);
    }
    else
      nvc = null;

    ModuleoObject obj = new ModuleoObject(Session["Login"].ToString(), "itemrequest", "reqnum");
    string wherestring = "", wherestringlinq = "";

    obj.CreateLinqCondition(nvc, ref wherestring, ref wherestringlinq);
    string jscript = "<script type=\"text/javascript\">";
    //string jscript = "";
    jscript += "var wherestring = '" + Server.UrlEncode(wherestringlinq) + "';var obj=parent.controlpanel.document.location.href='reqpanel.aspx?showlist=queryresult&wherestring='+wherestring;";
    jscript += "</script>";
    litFrameScript.Text = jscript;
  }

  private void SaveReq()
  {
    NameValueCollection nvc;
    Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
    TextBox tbx = null;
    string dirtylog = "0";
    TextBox opendate = CntlPanel.FindControl("txtopendate") as TextBox;
    if (CntlPanel != null)
    {
      screen = new AzzierScreen("requisition/reqmain.aspx", "MainForm", CntlPanel.Controls);
      screen.LCID = Session.LCID;
      nvc = screen.CollectFormValues("itemrequest", false);
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

    REQ reqobj;
    ReqLine reqlineobj = new ReqLine();
    bool success = false;
    if (m_mode == "new")
    {
      nvc["statuscode"] = "30";
      reqobj = new REQ(Session["Login"].ToString(), "itemrequest", "reqnum");
      success = reqobj.CreateREQ(nvc);
    }
    //else if (m_mode == "duplicate")
    //{
    //  string oldponum, newponum;
    //  reqobj = new REQ(Session["Login"].ToString(), "itemrequest", "reqnum");
    //  success = reqobj.CreateREQ(nvc);
    //  if (success)
    //  {
    //    HiddenField hid;
    //    hid = CntlPanel.FindControl("HidOldPONum") as HiddenField;
    //    oldponum = hid.Value;
    //    newponum = nvc["ponum"].ToString();
    //    success = reqobj.DuplicatePoline(oldponum, newponum);
    //  }
    //}
    else if (m_mode == "edit")
    {
      reqobj = new REQ(Session["Login"].ToString(), "itemrequest", "reqnum", m_reqnum);
      success = reqobj.UpdateREQ(nvc);
      if (success)
      {
        success = reqlineobj.UpdateReqTotalAmount(m_reqnum);
      }
    }
    else
    { reqobj = new REQ(Session["Login"].ToString(), "itemrequest", "reqnum"); }

    if (!success)
    {
      string jscript = "<script>alert(\"" + reqobj.ErrorMessage.Replace("\r\n", "") + "\");</script>";
      litFrameScript.Text = jscript;
    }
    else
    {
      if (m_mode == "new")
      { Response.Redirect("reqmain.aspx?mode=edit&reqnum=" + Server.UrlEncode(nvc["reqnum"])); }
      else
      {
        if (m_mode == "duplicate")
          Response.Redirect("reqmain.aspx?mode=edit&reqnum=" + Server.UrlEncode(nvc["reqnum"]));
        else if (m_mode == "edit")
        {
          Response.Redirect("reqmain.aspx?mode=edit&reqnum=" + Server.UrlEncode(m_reqnum));
        }
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
      case "autoreq":
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
        else if (m_tabname == "keyrequest")
        {
            //if (!keyapproved && m_statuscode>=1 && m_statuscode <100 )
            if (m_statuscode >= 1 && m_statuscode < 100)
            {
                if (dr["urEdit"].ToString() != "")
                    if (Convert.ToInt32(dr["urEdit"]) != 0 && alloweditdiv)
                        show = true;
            }
        }


        break;
      case "status":
        if (m_mode == "edit" && m_tabname=="Main" && alloweditdiv)
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
      default:
        break;
    }
    return show;
  }

  private void RetrieveMessage()
  {
    SystemMessage msg = new SystemMessage("location/locationheader.ascx");
    m_msg = msg.GetSystemMessage();
    //msg.SetJsMessage(litMessage);
  }

}