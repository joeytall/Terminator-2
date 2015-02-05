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
using System.Text;

public partial class requisition_reqmain : System.Web.UI.Page
{
  AzzierScreen screen;
  private string connstring;
  protected string querymode = "";
  protected string m_reqnum, m_oldreqnum = "", vendor = "";
  protected string iskeyrequest = "0";
  ModuleoObject objReq;
  NameValueCollection nvcReq;
  protected bool isquery = false;
  protected int statuscode = 0;
  protected NameValueCollection m_msg = new NameValueCollection();
  protected NameValueCollection m_rights;
  protected int m_allowedit = 0;
  protected string t_duplicate;
  protected RadGrid grdreqline;
  protected string defstatuscode = "30";
  private OleDbConnection conn;
  protected OleDbCommand cmd;
  protected OleDbDataReader reader;
  protected string cmdstr = "";
  protected string vendorname = "";

  protected void Page_Init(object sender, EventArgs e)
  {
    RetrieveMessage();
    UserRights.CheckAccess("itemrequest");
    UserRights r = new UserRights(Session["Login"].ToString(), "UserRights", "counter");
    m_rights = r.GetRights(Session["Login"].ToString(), "itemrequest");

    m_allowedit = Convert.ToInt16(m_rights["urEdit"].ToString());

    Session.LCID = Convert.ToInt32(Session["LCID"]);
    if (Request.QueryString["mode"] != null)
      querymode = Request.QueryString["mode"].ToString();
    else
      querymode = "query";

    if (Request.QueryString["reqnum"] != null)
    {
      m_reqnum = Request.QueryString["reqnum"].ToString();
      m_oldreqnum = Request.QueryString["reqnum"].ToString();
      HidOldReqNum.Value = Request.QueryString["reqnum"].ToString();
    }
    else
      m_reqnum = "";

    if (querymode == "query")
      isquery = true;

    string defstatus = "WTAPPR";
    defstatuscode = "1";
    NameValueCollection tempnvc = new NameValueCollection();
    tempnvc.Add("tfield", "reqstatus");
    tempnvc.Add("tcode", defstatus);
    ModuleoObject o = new ModuleoObject(Session["Login"].ToString(), "Codes", "Counter");
    string[] counterlist = o.Query(tempnvc);
    if (counterlist.Length > 0)
    {
        o = new ModuleoObject(Session["Login"].ToString(), "Codes", "Counter", counterlist[0]);
        defstatuscode = o.ModuleData["Tcode2"];
    }


    if (querymode == "edit")
    {
      objReq = new ModuleoObject(Session["Login"].ToString(), "itemrequest", "reqnum", m_reqnum);

      nvcReq = objReq.ModuleData;
      if (nvcReq["reqnum"].ToString() == "")
      {
        querymode = "query";
        isquery = true;
        hidMode.Value = querymode;
        m_reqnum = "";
      }
      if (!string.IsNullOrEmpty(nvcReq["Vendor"]))
      { vendor = nvcReq["Vendor"].ToString(); }

      UserWorkList list = new UserWorkList();
      list.AddToRecentList(Session["Login"].ToString(), "Requisition", m_reqnum);
    }
    else if (querymode == "duplicate" && m_oldreqnum.Length > 0)
    {
      objReq = new ModuleoObject(Session["Login"].ToString(), "itemrequest", "reqnum", m_oldreqnum);

      nvcReq = objReq.ModuleData;
      nvcReq["status"] = "WTAPPR";
      nvcReq["statuscode"] = defstatuscode;
    }
    else if (querymode == "new")                //new
    {
      objReq = new ModuleoObject(Session["Login"].ToString(), "itemrequest", "reqnum");
      nvcReq = objReq.ModuleData;
      nvcReq["status"] = "WTAPPR";
      nvcReq["statuscode"] = defstatuscode;
    }
    else
    {
      objReq = new ModuleoObject(Session["Login"].ToString(), "itemrequest", "reqnum");
      nvcReq = objReq.ModuleData;
    }
    hidMode.Value = querymode;

    string connstring = Application["ConnString"].ToString();
    conn = new OleDbConnection(connstring);

    cmdstr = "SELECT tcode2 FROM codes WHERE tfield='reqstatus' and tcode='WTAPPR'";
    OleDbCommand cmd = new OleDbCommand(cmdstr, conn);
    OleDbDataReader reader;

    // Try to open database and read information.
    try
    {
      conn.Open();
      reader = cmd.ExecuteReader();

      // For each item, add the author name to the displayed
      // list box text, and store the unique ID in the Value property.
      while (reader.Read())
      {
        defstatuscode = reader[0].ToString();
      }
      reader.Close();

      if (nvcReq["Vendor"] != "")
      {
        cmdstr = "SELECT Vendname FROM Vendor WHERE CompanyCode = '" + nvcReq["Vendor"] + "'";
        cmd = new OleDbCommand(cmdstr, conn);
        reader = cmd.ExecuteReader();
        while (reader.Read())
        {
          vendorname = reader[0].ToString();
        }
        reader.Close();
      }
    }
    catch (Exception err)
    {
      Response.Write(err.Message);
    }
    finally
    {
      conn.Close();
    }

    InitScreen();

    //Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "alertMessage", "refreshPoTotalAmount();", true);
  }

  private void InitScreen()
  {
    screen = new AzzierScreen("requisition/reqmain.aspx", "MainForm", MainControlsPanel.Controls, querymode);
    screen.LCID = Session.LCID;

    if (querymode == "edit")
    { InitGrid(); }
    screen.LoadScreen();
    if (!isquery)
      screen.SetValidationControls();
  }

  private void InitGrid()
  {
    grdreqline = new RadGrid();
    grdreqline.ID = "grdreqline";
    //grdreqline.DataSourceID = "reqItems_SqlDataSource";
    grdreqline.ShowFooter = true;
    grdreqline.ShowHeader = true;
    grdreqline.MasterTableView.ShowHeadersWhenNoRecords = true;
    grdreqline.AutoGenerateColumns = false;
    grdreqline.AllowPaging = true;
    grdreqline.PageSize = 100;
    //string connstring = Application["ConnString"].ToString();
    //reqItems_SqlDataSource.ConnectionString = connstring;
    //reqItems_SqlDataSource.SelectCommand = "SELECT * FROM requestline WHERE reqnum = '" + m_reqnum + "'";
    grdreqline.MasterTableView.DataKeyNames = new string[] { "Counter", "IsService" };
    grdreqline.MasterTableView.ClientDataKeyNames = new string[] { "Counter", "IsService" };
    GridEditCommandColumn editColumn = new GridEditCommandColumn();
    editColumn.HeaderText = "Edit";
    editColumn.UniqueName = "EditCommand";
    editColumn.ButtonType = GridButtonColumnType.ImageButton;
    editColumn.EditImageUrl = "~/images/Edit.gif";
    editColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
    editColumn.HeaderStyle.Width = 30;
    grdreqline.MasterTableView.Columns.Add(editColumn);
    grdreqline.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.Top;

    screen.SetGridColumns("reqline", grdreqline);
    /*
    if (m_allowedit == 1)
    {
      GridButtonColumn buttonColumn = new GridButtonColumn();
      buttonColumn.HeaderText = "Delete";
      buttonColumn.UniqueName = "DeleteButton";
      buttonColumn.CommandName = "Delete";
      buttonColumn.ButtonType = GridButtonColumnType.ImageButton;
      buttonColumn.ImageUrl = "~/images/Delete.gif";
      buttonColumn.Text = "Delete";
      buttonColumn.HeaderStyle.Width = 30;
      buttonColumn.ConfirmText = "Are you sure you want to delete this record?";
      grdreqline.MasterTableView.Columns.Add(buttonColumn);
    }
     * */

    if (Convert.ToInt32(nvcReq["statuscode"]) >= 1 && Convert.ToInt32(nvcReq["statuscode"]) <= 99)
    {
      grdreqline.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(1, true, "Line Items", null, "return EditReqService('','" + m_reqnum + "','" + vendor + "','" + nvcReq["statuscode"] + "')", m_allowedit,"",true,"Add Item","Add Service");
      
    }
    else
    {
      grdreqline.MasterTableView.CommandItemTemplate = new CodesCommandItem("Line Items", 0);
    }

    MainControlsPanel.Controls.Add(grdreqline);

    grdreqline.ItemDataBound += new GridItemEventHandler(grdreqline_ItemDataBound);
    string wherestring = "";
    Validation v = new Validation();
    wherestring = v.AddLinqConditions("ReqNum^" + m_reqnum,"requisition/reqmain.aspx.cs","reqline","RequestLine",null,null,"edit");
    grdreqline.ClientSettings.DataBinding.SelectMethod = "GetReqLine?wherestring=" + wherestring;
    grdreqline.ClientSettings.DataBinding.Location = "../InternalServices/ServiceREQ.svc";
    grdreqline.ClientSettings.DataBinding.SortParameterType = GridClientDataBindingParameterType.Linq;
    grdreqline.ClientSettings.DataBinding.FilterParameterType = GridClientDataBindingParameterType.Linq;
    //grdreqline.DeleteCommand += new GridCommandEventHandler(grdreqline_DeleteCommand);
  }
  /*
  protected void grdreqline_DeleteCommand(object sender, GridCommandEventArgs e)
  {
    GridEditableItem editedItem = e.Item as GridEditableItem;
    ////Get the primary key value using the DataKeyValue.
    string counter = editedItem.OwnerTableView.DataKeyValues[editedItem.ItemIndex]["Counter"].ToString();

    ReqLine obj = new ReqLine(Session["Login"].ToString(), "requestline", "counter", counter);
    bool success = obj.DeleteReqline();
    if (!success)
    {
      grdreqline.Controls.Add(new LiteralControl(m_msg["T3"] + obj.ErrorMessage));
      e.Canceled = true;
    }
    else
    {
      obj.UpdateReqTotalAmount(m_reqnum);
      ScriptManager.RegisterClientScriptBlock(this, typeof(System.Web.UI.Page), "MyJSFunction", "refreshPoTotalAmount();", true);
    }
  }
   * */

  protected void grdreqline_ItemDataBound(object sender, GridItemEventArgs e)
  {
      if (e.Item is GridDataItem && !e.Item.IsInEditMode)
      {
        GridDataItem item = (GridDataItem)e.Item;

        ImageButton btn = (ImageButton)item["EditCommand"].Controls[0];
        btn.ImageUrl = "~/images/Edit.gif";
        btn.OnClientClick = "return EditLine('" + item.ItemIndex.ToString() + "','" + m_reqnum + "','" + vendor + "','" + nvcReq["statuscode"] + "')";
      }      

      if (e.Item is GridCommandItem)
      {
        GridCommandItem commandItem = (GridCommandItem)e.Item;

        ImageButton t_addInPlaceButton = (ImageButton)commandItem.FindControl("addInPlaceButton");
        if (t_addInPlaceButton != null)
          t_addInPlaceButton.OnClientClick = "return EditReqItems('','" + m_reqnum + "','" + vendor + "','"+ nvcReq["statuscode"] + "')"; ;
      }
    

    screen.GridItemDataBound(e, "requisition/reqmain.aspx", "MainForm", "reqnum");
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!Page.IsPostBack)
    {
      string autonum = "";
      if (Request.QueryString["numbering"] != null)
      {
        autonum = Request.QueryString["numbering"].ToString().ToLower();
      }

      if ((querymode == "new" && autonum == "auto") || querymode == "duplicate")
      {
        NewNumber num = new NewNumber();
        nvcReq["reqnum"] = num.GetNextNumber("REQUEST");
      } 
      
      if (querymode == "edit")
      {
        HidDivision.Value = nvcReq["division"];
      }

      if (querymode == "new")
      {
        screen.GetDefaultValue("itemrequest", nvcReq, "ItemRequest", "new");
      }

      screen.PopulateScreen("itemrequest", nvcReq);

      RadioButtonList r = MainControlsPanel.FindControl("rbliskeyrequest") as RadioButtonList;
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
              if (nvcReq["iskeyrequest"].ToString() == "1")
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
      if (Request.Form["__EVENTTARGET"] == "ModifyReq")
      {
        ModifyReq();
      }
      if (Request.Form["__EVENTTARGET"] == "saveStatus")
      {
        SaveStatus();
      }
    }

    ucHeader1.Mode = querymode;
    ucHeader1.TabName = "Main";
    ucHeader1.ModuleData = nvcReq;

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

    if (querymode == "new" || querymode == "duplicate")
    {
      TextBox d = (TextBox)MainControlsPanel.FindControl("txtopendate");
      if (d != null)
      {
        if (!Page.IsPostBack)
          d.Text = DateTime.Today.ToShortDateString();
      }
    }

    if (querymode == "edit")
    {
      TextBox d = (TextBox)MainControlsPanel.FindControl("txtvendordesc");
      if (d != null)
      {
        if (!Page.IsPostBack)
          d.Text = vendorname;
      }
    }
  }

  private void Query()
  {
    NameValueCollection nvc;
    nvc = screen.CollectFormValues("itemrequest", true);

    ModuleoObject obj = new ModuleoObject(Session["Login"].ToString(), "itemrequest", "reqnum");
    string wherestring = "", wherestringlinq = "";

    obj.CreateLinqCondition(nvc, ref wherestring, ref wherestringlinq);
    //string jscript = "<script type=\"text/javascript\">";
    string jscript = "";
    jscript += "var wherestring = '" + Server.UrlEncode(wherestringlinq) + "';var obj=parent.controlpanel.document.location.href='reqpanel.aspx?showlist=queryresult&wherestring='+wherestring;";
    //jscript += "</script>";
    litScript1.Text = jscript;

  }

  private Boolean ModifyReq()
  {
    SaveReq();

    string newdivision = hidNewDivision.Value;
    string newparentid = hidNewParentID.Value;
    string newstatus = hidNewStatus.Value;
    string newparentdesc = hidNewParentDesc.Value;
    string moddate = hidModDate.Value;

    ModuleoObject req = new ModuleoObject(Session["Login"].ToString(), "itemrequest", "reqnum", m_reqnum);

    CultureInfo c = new CultureInfo(Session.LCID);
    DateTime TransDate = Convert.ToDateTime(moddate, c);

    litScript1.Text = "document.location.href='reqmain.aspx?mode=edit&reqnum=" + m_reqnum + "'";
    return true;
  }

  private Boolean SaveReq()
  {
    NameValueCollection nvc;
    Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
    TextBox tbx = null;
    string dirtylog = "0";
    if (CntlPanel != null)
    {
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
    bool success = false;
    if (querymode == "new")
    {
      nvc["statuscode"] = defstatuscode;
      reqobj = new REQ(Session["Login"].ToString(), "itemrequest", "reqnum");
      success = reqobj.CreateREQ(nvc);
    }
    else if (querymode == "duplicate")
    {
      //string oldreqnum, newreqnum;
      reqobj = new REQ(Session["Login"].ToString(), "itemrequest", "reqnum");
      success = reqobj.CreateREQ(nvc);
      if (success)
      {
        //HiddenField hid;
        //hid = CntlPanel.FindControl("HidOldPONum") as HiddenField;
        //oldreqnum = hid.Value;
        //newreqnum = nvc["reqnum"].ToString();
        //success = reqobj.DuplicatePoline(oldreqnum, newreqnum);
      }
    }
    else if (querymode == "edit")
    {
      reqobj = new REQ(Session["Login"].ToString(), "itemrequest", "reqnum", m_reqnum);
      success = reqobj.UpdateREQ(nvc);
      if (success)
      {
        //success = reqobj.UpdateReqTotalAmount(m_reqnum);
      }
    }
    else
    { reqobj = new REQ(Session["Login"].ToString(), "itemrequest", "reqnum"); }

    return success;
  }

  private void SaveStatus()
  {
    SaveReq();

    NameValueCollection nvc = new NameValueCollection();
    nvc.Add("statuscode", hidTargetStatusCode.Value);
    nvc.Add("status", hidTargetStatus.Value);
    nvc.Add("Remark", hidStatusComments.Value);
    REQ objreq = new REQ(Session["Login"].ToString(), "itemrequest", "reqnum", m_reqnum);

    int statuscode = Convert.ToInt32(hidTargetStatusCode.Value);
    if (statuscode >= 300 && statuscode < 500)
    {
      if (nvc["closedate"] == "" || nvc["closedate"] == null)
      {
        nvc["closedate"] = DateTime.Now.ToShortDateString();
      }
    }

    bool success = objreq.UpdateStatus(nvc);
    if (success)
    {
      litScript1.Text = "document.location.href='reqmain.aspx?mode=edit&reqnum=" + m_reqnum + "';";
    }
    else
      litScript1.Text = "alert('" + objreq.ErrorMessage + "');";
  }

  protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
  { }

  private void RetrieveMessage()
  {
    SystemMessage msg = new SystemMessage("location/locationmain.aspx");
    m_msg = msg.GetSystemMessage();
    msg.SetJsMessage(litMessage);
  }
}