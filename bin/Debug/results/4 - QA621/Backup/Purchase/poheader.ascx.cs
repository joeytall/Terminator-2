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

public partial class purchase_purchaseheader : System.Web.UI.UserControl
{
  private string m_mode;
  private string m_tabname;
  private string m_operationlabel;
  // private string m_empid = "";
  public string m_ponum = "", m_vendor = "";
  private NameValueCollection m_nvc;
  private AzzierScreen screen;
  protected string linksrvr = "";
  private int m_AllowEdit;
  protected RadComboBox toolbarcombobox;
  private string m_division;
  private string m_statuscode;
  protected int m_masterrec;
  protected int defaultprintcounter = -1;
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
      m_ponum = m_nvc["ponum"];
      m_division = m_nvc["division"];
      m_statuscode = m_nvc["statuscode"];
      m_vendor = m_nvc["vendor"];
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
      tabhlkMain.NavigateUrl = "pomain.aspx?mode=edit&ponum=" + m_nvc["ponum"];
      tabhlkMain.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgMain','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblMain','toptabover')");
      tabhlkMain.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgMain','../images/tabbutton.png');classChangeover('ucHeader1_tablblMain','toptabout')");
    }

    if (m_tabname == "invoices")
    {
      tabhlkInvoices.NavigateUrl = "javascript:void(null)";
      tabimgInvoices.ImageUrl = "../images/tabbutton_down.png";
      tablblInvoices.CssClass = "toptabover";
    }
    else
    {
      if (m_mode == "edit")
      {
        tabhlkInvoices.NavigateUrl = "poinvoices.aspx?ponum=" + m_nvc["ponum"];
        tabhlkInvoices.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgInvoices','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblInvoices','toptabover')");
        tabhlkInvoices.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgInvoices','../images/tabbutton.png');classChangeover('ucHeader1_tablblInvoices','toptabout')");
      }
      else
      {
        tabhlkInvoices.Enabled = false;
        tablblInvoices.CssClass = "toptabinactive";
      }
    }

    if (m_tabname == "Accounts")
    {
      tabhlkAccounts.NavigateUrl = "javascript:void(null)";
      tabimgAccounts.ImageUrl = "../images/tabbutton_down.png";
      tablblAccounts.CssClass = "toptabover";
    }
    else
    {
      if (m_mode == "edit")
      {
        tabhlkAccounts.NavigateUrl = "poaccounts.aspx?ponum=" + m_nvc["ponum"];
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
        tabhlkHistory.NavigateUrl = "pohistory.aspx?ponum=" + m_nvc["ponum"];
        tabhlkHistory.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgHistory','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblHistory','toptabover')");
        tabhlkHistory.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgHistory','../images/tabbutton.png');classChangeover('ucHeader1_tablblHistory','toptabout')");
      }
      else
      {
        tabhlkHistory.Enabled = false;
        tablblHistory.CssClass = "toptabinactive";
      }
    }

    if (!Page.IsPostBack)
    {
      AzzierHeader.InitToolBar(toolbar, "purchase", ShowToolbarButtonRights);
    }
    litOperation.Text = m_operationlabel;
    AzzierHeader.CreateLinksDropDown(plhLinkDoc, "purchase", m_ponum, out linksrvr);
    AzzierHeader.CreateReportsDropDown(plhReports, "purchase");
    AzzierHeader.CreateQueryDropDown(plhQuery, "purchase");

    defaultprintcounter = AzzierPrintForm.GetDefaultReport("Purchase");

    if (Page.IsPostBack)
    {
      if (Request.Form["__EVENTTARGET"] == "save")
      {
        SavePurchaseOrder();
      }
      if (Request.Form["__EVENTTARGET"] == "lookup")
      {
        LookUpPurchase();
      }
      if (Request.Form["__EVENTTARGET"] == "delete")
      {
        DeletePurchase();
      }
    }
  }

  protected void DeletePurchase()
  {
    ModuleoObject purchase = new ModuleoObject(Session["Login"].ToString(), "po", "ponum", m_ponum);
    purchase.Delete();
    string jscript = "<script type=\"text/javascript\">";
    jscript += "setTimeout(parent.controlpanel.RefreshPanelGridsOnDelete(), 0); document.location.href='pomain.aspx';";
    jscript += "</script>";
    litFrameScript.Text = jscript;
  }

  protected void LookUpPurchase()
  {
    NameValueCollection nvc;
    Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
    if (CntlPanel != null)
    {
      screen = new AzzierScreen("purchase/pomain.aspx", "MainForm", CntlPanel.Controls);
      screen.LCID = Session.LCID;
      nvc = screen.CollectFormValues("po", true);
    }
    else
      nvc = null;

    ModuleoObject obj = new ModuleoObject(Session["Login"].ToString(), "po", "ponum");
    string wherestring = "", wherestringlinq = "";

    //string[] po = obj.LinqQuery(nvc, ref wherestring, ref wherestringlinq);
    obj.CreateLinqCondition(nvc, ref wherestring, ref wherestringlinq);
    string jscript = "<script type=\"text/javascript\">";
    jscript += "var wherestring = '" + Server.UrlEncode(wherestringlinq) + "';var obj=parent.controlpanel.document.location.href='popanel.aspx?showlist=queryresult&wherestring='+wherestring;";
    jscript += "</script>";
    litFrameScript.Text = jscript;
  }

  private void SavePurchaseOrder()
  {
    NameValueCollection nvc;
    Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
    TextBox tbx = null;
    string dirtylog = "0";
    TextBox opendate = CntlPanel.FindControl("txtopendate") as TextBox;
    if (CntlPanel != null)
    {
      screen = new AzzierScreen("purchase/pomain.aspx", "MainForm", CntlPanel.Controls);
      screen.LCID = Session.LCID;
      nvc = screen.CollectFormValues("po", false);
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

    PO poobj;
    POLine polineobj = new POLine();
    bool success = false;
    if (m_mode == "new")
    {
      poobj = new PO(Session["Login"].ToString(), "po", "ponum");
      success = poobj.Create(nvc);
    }
    else if (m_mode == "duplicate")
    {
      string oldponum, newponum;
      poobj = new PO(Session["Login"].ToString(), "po", "ponum");
      success = poobj.Create(nvc);
      if (success)
      {
        HiddenField hid;
        hid = CntlPanel.FindControl("HidOldPONum") as HiddenField;
        oldponum = hid.Value;
        newponum = nvc["ponum"].ToString();
        success = poobj.DuplicatePoline(oldponum, newponum);
      }
    }
    else if (m_mode == "edit")
    {
      poobj = new PO(Session["Login"].ToString(), "po", "ponum", m_ponum);
      success = poobj.Update(nvc);
      if (success)
      {
        success = polineobj.UpdatePOTotalAmount(m_ponum);
      }
    }
    else
    { poobj = new PO(Session["Login"].ToString(), "po", "ponum"); }

    if (!success)
    {
      string jscript = "<script>alert(\"" + poobj.ErrorMessage.Replace("\r\n", "") + "\");</script>";
      litFrameScript.Text = jscript;
    }
    else
    {
      if (m_mode == "new")
      { Response.Redirect("pomain.aspx?mode=edit&ponum=" + Server.UrlEncode(nvc["ponum"])); }
      else
      {
        if (m_mode == "duplicate")
          Response.Redirect("pomain.aspx?mode=edit&ponum=" + Server.UrlEncode(nvc["ponum"]));
        else if (m_mode == "edit")
        {
          //tbx.Text = (Convert.ToInt32(dirtylog) + 1).ToString();
          Response.Redirect("pomain.aspx?mode=edit&ponum=" + Server.UrlEncode(m_ponum));// Server.UrlEncode(nvc["ponum"]));
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
    bool allowmaster = false;
    if (dr["urMaster"].ToString() != "")
      if (Convert.ToInt32(dr["urAddNew"].ToString()) != 0)
        allowmaster = true;
    bool allowchangevendor = false;
    bool allowaddrequest = false;

    int postatuscode = 0;
    if (m_statuscode != "")
    {
      postatuscode = Convert.ToInt32(m_statuscode);
      if (postatuscode >= 1 && postatuscode < 100)
      {
        allowchangevendor = true;
        allowaddrequest = true;
      }
    }

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
      case "autopo":
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
      case "status":
        if (m_mode == "edit" && alloweditdiv)
          show = true;
        break;
      case "duplicate":
        if (m_mode == "edit")
          if (dr["urAddNew"].ToString() != "")
            if (Convert.ToInt32(dr["urAddNew"].ToString()) > 0)
              show = true;
        break;
      case "chgvendor":
        if (m_mode == "edit" && allowchangevendor)
          show = true;
        break;
      case "addrequest":
        if (m_mode == "edit" && allowaddrequest)
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
      case "batchpicture":
        show = true;
        break;
      case "linkeddoc":
        show = true;
        break;
      case "generatepo":
        show = true;
        break;
      //case "delete":
      //  if (m_masterrec == 1 && allowmaster == false)
      //  {
      //    show = false;
      //  }
      //  else
      //  {
      //    if (m_tabname == "Main" && m_mode == "edit")
      //    {
      //      if (dr["urDelete"].ToString() != "")
      //        if (Convert.ToInt32(dr["urDelete"]) != 0 && alloweditdiv)
      //          show = true;
      //    }
      //  }
      //  break;
      //case "modify":
      //    if (m_masterrec == 1 && allowmaster == false)
      //    {
      //        show = false;
      //    }
      //    else
      //    {
      //        if (m_tabname == "Main" && m_mode == "edit")
      //        {
      //            if (dr["urEdit"].ToString() != "")
      //                if (Convert.ToInt32(dr["urEdit"]) != 0 && alloweditdiv)
      //                    show = true;
      //        }
      //    }
      //    break;
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