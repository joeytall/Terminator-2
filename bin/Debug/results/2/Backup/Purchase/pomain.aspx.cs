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

public partial class purchase_purchasemain : System.Web.UI.Page
{
  AzzierScreen screen;
  private string connstring;
  protected string querymode = "";
  protected string m_ponum, m_oldponum = "", vendor = "";

  ModuleoObject objPurchase;
  NameValueCollection nvcPO;
  protected bool isquery = false;
  protected int statuscode = 0;
  protected NameValueCollection m_msg = new NameValueCollection();
  protected NameValueCollection m_rights;
  protected int m_allowedit = 0;
  protected string t_duplicate;
  protected RadGrid grdpoline;
  protected string defstatuscode = "1";
  protected string defstatus = "WTAPPR";
  private OleDbConnection conn;
  protected OleDbCommand cmd;
  protected OleDbDataReader reader;
  protected string cmdstr = "";

  protected void Page_Init(object sender, EventArgs e)
  {
    RetrieveMessage();
    UserRights.CheckAccess("purchase");
    UserRights r = new UserRights(Session["Login"].ToString(), "UserRights", "counter");
    m_rights = r.GetRights(Session["Login"].ToString(), "purchase");

    m_allowedit = Convert.ToInt16(m_rights["urEdit"].ToString());

    Session.LCID = Convert.ToInt32(Session["LCID"]);
    if (Request.QueryString["mode"] != null)
      querymode = Request.QueryString["mode"].ToString();
    else
      querymode = "query";

    if (querymode == "")
      querymode = "query";

    if (Request.QueryString["ponum"] != null)
    {
      m_ponum = Request.QueryString["ponum"].ToString();
      m_oldponum = Request.QueryString["ponum"].ToString();
      HidOldPONum.Value = Request.QueryString["ponum"].ToString();
    }
    else
      m_ponum = "";

    if (querymode == "query")
      isquery = true;

    string connstring = Application["ConnString"].ToString();
    conn = new OleDbConnection(connstring);
    /*
    cmdstr = "SELECT tcode2 FROM codes WHERE tfield='postatus' and tcode='WTAPPR'";
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
    }
    catch (Exception err)
    {
      Response.Write(err.Message);
    }
    finally
    {
      conn.Close();
    }
     * */

    //TextBox txtponum = MainControlsPanel.FindControl("txtponum") as TextBox;
    if (querymode == "edit")
    {
      objPurchase = new ModuleoObject(Session["Login"].ToString(), "po", "ponum", m_ponum);

      nvcPO = objPurchase.ModuleData;
      statuscode = Convert.ToInt16(nvcPO["StatusCode"]);
      if (nvcPO["ponum"].ToString() == "")
      {
        querymode = "query";
        isquery = true;
        hidMode.Value = querymode;
        m_ponum = "";
      }
      if (!string.IsNullOrEmpty(nvcPO["Vendor"]))
      { vendor = nvcPO["Vendor"].ToString(); }

      UserWorkList list = new UserWorkList();
      list.AddToRecentList(Session["Login"].ToString(), "Purchase", m_ponum);
      //InitGrid();
    }
    else if (querymode == "duplicate" && m_oldponum.Length > 0)
    {
      //NewNumber num = new NewNumber();
      //txtponum.Text = num.GetNextNumber("PO");
      objPurchase = new ModuleoObject(Session["Login"].ToString(), "po", "ponum", m_oldponum);
      nvcPO = objPurchase.ModuleData;

      defstatus = "WTAPPR";
      statuscode = 1;
      if (Application["AutoApprovePO"] != null)
      {
        if (Application["AutoApprovePO"].ToString().ToLower() == "yes")
          defstatus = "APPR";
      }

      NameValueCollection tempnvc = new NameValueCollection();
      tempnvc.Add("tfield", "postatus");
      tempnvc.Add("tcode", defstatus);
      ModuleoObject o = new ModuleoObject(Session["Login"].ToString(), "Codes", "Counter");
      string[] counterlist = o.Query(tempnvc);
      if (counterlist.Length > 0)
      {
        o = new ModuleoObject(Session["Login"].ToString(), "Codes", "Counter", counterlist[0]);
        statuscode = Convert.ToInt16(o.ModuleData["Tcode2"]);

      }
      nvcPO["statuscode"] = statuscode.ToString();
      nvcPO["status"] = defstatus;
      nvcPO["empid"] = Session["Login"].ToString();

      
      //nvcPO["ponum"] = num.GetNextNumber("PO");
      //litScript1.Text = "resetpostatus('" + num.GetNextNumber("PO") + "')";
    }
    else if (querymode == "new") //new
    {
      //NewNumber num = new NewNumber();
      objPurchase = new ModuleoObject(Session["Login"].ToString(), "po", "ponum");
      nvcPO = objPurchase.ModuleData;

      defstatus = "WTAPPR";
      statuscode = 1;
      if (Application["AutoApprovePO"] != null)
      {
        if (Application["AutoApprovePO"].ToString().ToLower() == "yes")
          defstatus = "APPR";
      }

      NameValueCollection tempnvc = new NameValueCollection();
      tempnvc.Add("tfield", "postatus");
      tempnvc.Add("tcode", defstatus);
      ModuleoObject o = new ModuleoObject(Session["Login"].ToString(), "Codes", "Counter");
      string[] counterlist = o.Query(tempnvc);
      if (counterlist.Length > 0)
      {
        o = new ModuleoObject(Session["Login"].ToString(), "Codes", "Counter", counterlist[0]);
        statuscode = Convert.ToInt16(o.ModuleData["Tcode2"]);

      }
      nvcPO["statuscode"] = statuscode.ToString();
      nvcPO["status"] = defstatus;
      nvcPO["empid"] = Session["Login"].ToString();

      //if (Request.QueryString["numbering"] == "auto")
      //{
        //nvcPO["ponum"] = num.GetNextNumber("PO");
        //litScript1.Text = "resetpostatus('" + num.GetNextNumber("PO") + "')";
      //}
      //else
      //litScript1.Text = "resetpostatus('')";
    }
    else
    {
      objPurchase = new ModuleoObject(Session["Login"].ToString(), "po", "ponum");
      nvcPO = objPurchase.ModuleData;
    }

    if (querymode == "new")
    {
        nvcPO["BillTo"] = Division.GetDivDefaults("Bill To");
        nvcPO["ShipTo"] = Division.GetDivDefaults("Ship To");
        if (nvcPO["BillTo"] != "")
        {
            ModuleoObject obj = new ModuleoObject(Session["Login"].ToString(), "ShipTo", "ShipName", nvcPO["BillTo"]);
            nvcPO["BillAddress1"] = obj.ModuleData["Address1"];
            nvcPO["BillAddress2"] = obj.ModuleData["Address2"];
            nvcPO["BillAddress3"] = obj.ModuleData["Address3"];
            nvcPO["BillAddress3"] = obj.ModuleData["Address3"];
            nvcPO["BillPhone"] = obj.ModuleData["PhoneNumber"];
        }
        if (nvcPO["ShipTo"] != "")
        {
            ModuleoObject obj = new ModuleoObject(Session["Login"].ToString(), "ShipTo", "ShipName", nvcPO["ShipTo"]);
            nvcPO["ShipAddress1"] = obj.ModuleData["Address1"];
            nvcPO["ShipAddress2"] = obj.ModuleData["Address2"];
            nvcPO["ShipAddress3"] = obj.ModuleData["Address3"];
            nvcPO["ShipPhone"] = obj.ModuleData["PhoneNumber"];
        }

    }
    hidMode.Value = querymode;
    connstring = Application["ConnString"].ToString();
    InitScreen();

    //Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "alertMessage", "refreshPoTotalAmount();", true);
  }

  private void InitScreen()
  {
    screen = new AzzierScreen("purchase/pomain.aspx", "MainForm", MainControlsPanel.Controls, querymode);
    screen.LCID = Session.LCID;

    if (querymode == "edit")
    { InitGrid(); }
    screen.LoadScreen();
    if (!isquery)
      screen.SetValidationControls();
  }

  private void InitGrid()
  {
    grdpoline = new RadGrid();
    grdpoline.ID = "grdpoline";
    grdpoline.DataSourceID = "poItems_SqlDataSource";
    grdpoline.ShowFooter = true;
    grdpoline.ShowHeader = true;
    grdpoline.MasterTableView.ShowHeadersWhenNoRecords = true;
    grdpoline.AutoGenerateColumns = false;

    string connstring = Application["ConnString"].ToString();
    poItems_SqlDataSource.ConnectionString = connstring;
    poItems_SqlDataSource.SelectCommand = "SELECT * FROM poline WHERE ponum='" + m_ponum + "'";
    grdpoline.MasterTableView.DataKeyNames = new string[] { "Counter", "IsService" };

    GridEditCommandColumn editColumn = new GridEditCommandColumn();
    editColumn.HeaderText = "Edit";
    editColumn.UniqueName = "EditCommand";
    editColumn.ButtonType = GridButtonColumnType.ImageButton;
    editColumn.EditImageUrl = "~/Images/Edit.gif";
    editColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
    editColumn.HeaderStyle.Width = 30;
    grdpoline.MasterTableView.Columns.Add(editColumn);
    grdpoline.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.Top;

    screen.SetGridColumns("poline", grdpoline);

    //if (m_allowedit == 1)
    //{
    //  GridButtonColumn buttonColumn = new GridButtonColumn();
    //  buttonColumn.HeaderText = "Delete";
    //  buttonColumn.UniqueName = "DeleteButton";
    //  buttonColumn.CommandName = "Delete";
    //  buttonColumn.ButtonType = GridButtonColumnType.ImageButton;
    //  buttonColumn.ImageUrl = "~/images/Delete.gif";
    //  buttonColumn.Text = "Delete";
    //  buttonColumn.HeaderStyle.Width = 30;
    //  buttonColumn.ConfirmText = "Are you sure you want to delete this record?";
    //  grdpoline.MasterTableView.Columns.Add(buttonColumn);
    //}

    grdpoline.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(statuscode, true, "Line Items", null, "return EditPOService('','" + m_ponum + "','" + vendor + "','20')", m_allowedit,"",true,"Add Physical Item","Add Service",null,null,Application["AutoApprovePo"].ToString());
    
    //grdpoline.ClientSettings.ClientEvents.OnDataBound = "grdpoline_calculatetotal";
    
                                                         
    MainControlsPanel.Controls.Add(grdpoline);

    grdpoline.ItemDataBound += new GridItemEventHandler(grdpoline_ItemDataBound);
    
    //grdpoline.DeleteCommand += new GridCommandEventHandler(grdpoline_DeleteCommand);
  }

  protected void grdpoline_DeleteCommand(object sender, GridCommandEventArgs e)
  {
    GridEditableItem editedItem = e.Item as GridEditableItem;
    ////Get the primary key value using the DataKeyValue.
    string counter = editedItem.OwnerTableView.DataKeyValues[editedItem.ItemIndex]["Counter"].ToString();

    POLine obj = new POLine(Session["Login"].ToString(), "poline", "counter", counter);
    bool success = obj.Delete();//.Delete();
    if (!success)
    {
      grdpoline.Controls.Add(new LiteralControl(m_msg["T3"] + obj.ErrorMessage));
      e.Canceled = true;
    }
    else
    {
      obj.UpdatePOTotalAmount(m_ponum);
      ScriptManager.RegisterClientScriptBlock(this, typeof(System.Web.UI.Page), "MyJSFunction", "refreshPoTotalAmount();", true);
      //string jscript = "<script type=\"text/javascript\">";
      //jscript += "refreshPoTotalAmount()";
      //jscript += "</script>";
      //litScript1.Text = jscript;
    }
  }

  protected void grdpoline_ItemDataBound(object sender, GridItemEventArgs e)
  {
    if (m_allowedit == 1)
    {
      if (e.Item is GridDataItem && !e.Item.IsInEditMode)
      {
        GridDataItem item = (GridDataItem)e.Item;

        ImageButton btn = (ImageButton)item["EditCommand"].Controls[0];
        btn.ImageUrl = "~/images/Edit.gif";
        if (item.OwnerTableView.DataKeyValues[item.ItemIndex]["IsService"].ToString() == "1")
        {
          btn.OnClientClick = "return EditPOService('" + item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString() + "','" + m_ponum + "','" + vendor + "','" + nvcPO["statuscode"] + "')";
        }
        else
        {
          btn.OnClientClick = "return EditPOItems('" + item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString() + "','" + m_ponum + "','" + vendor + "','" + nvcPO["statuscode"] + "')";
        }
      }

      if (e.Item is GridCommandItem)
      {
        GridCommandItem commandItem = (GridCommandItem)e.Item;

        ImageButton t_addInPlaceButton = (ImageButton)commandItem.FindControl("addInPlaceButton");
        if (t_addInPlaceButton != null)
          t_addInPlaceButton.OnClientClick = "return EditPOItems('','" + m_ponum + "','" + vendor + "','20')"; ;
      }
    }

    screen.GridItemDataBound(e, "purchase/pomain.aspx", "MainForm", "ponum");
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
        nvcPO["ponum"] = num.GetNextNumber("PO");
      }

      if (querymode == "new")
      {
        screen.GetDefaultValue("po", nvcPO, "Purchase", "new"); 
      }
      
      if (querymode == "edit")
      {
        HidDivision.Value = nvcPO["division"];
      }

      if (querymode == "duplicate")
      {
        TextBox text = MainControlsPanel.FindControl("txtvendor") as TextBox;
        if (text != null)
        {
          text.ReadOnly = true;
        }
        HyperLink curlink = MainControlsPanel.FindControl("lkuvendor") as HyperLink;
        if (curlink != null)
        {
          curlink.Visible = false;
        }
      }
      if (querymode == "query")
      {
          TextBox t = MainControlsPanel.FindControl("txtvendorname") as TextBox;
          if (t != null)
              screen.SetTextControlReadonly("vendorname", MainControlsPanel);
      }
      else
      {
          string vendor = nvcPO["Vendor"];
          ModuleoObject objvendor = new ModuleoObject(Session["Login"].ToString(), "Vendor", "CompanyCode", vendor);
          TextBox t = MainControlsPanel.FindControl("txtvendorname") as TextBox;
          if (t != null)
              t.Text = objvendor.ModuleData["VendName"];
      }

      screen.PopulateScreen("po", nvcPO);
    }
    else
    {
      if (Request.Form["__EVENTTARGET"] == "Query")
      {
        Query();
      }
      if (Request.Form["__EVENTTARGET"] == "ModifyPO")
      {
        ModifyPO();
      }
      if (Request.Form["__EVENTTARGET"] == "saveStatus")
      {
        SaveStatus();
      }
    }

    ucHeader1.Mode = querymode;
    ucHeader1.TabName = "Main";
    ucHeader1.ModuleData = nvcPO;

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

    //if (querymode == "edit")
    //{
    //    TextBox rdp = MainControlsPanel.FindControl("rdpopendate") as TextBox;
    //    if (rdp != null)
    //        rdp.Enabled = false;
    //}
    //else 
    if (querymode == "new" || querymode == "duplicate")
    {
      TextBox d = (TextBox)MainControlsPanel.FindControl("txtopendate");
      if (d != null)
      {
        if (!Page.IsPostBack)
          d.Text = DateTime.Today.ToShortDateString();
      }
    }
  }

  private void Query()
  {
    NameValueCollection nvc;
    //Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
    //if (CntlPanel != null)
    //{
    //    screen = new AzzierScreen("purchase/pomain.aspx", "MainForm", CntlPanel.Controls);
    //    screen.LCID = Session.LCID;
    nvc = screen.CollectFormValues("po", true);
    //}
    //else
    //    nvc = null;

    ModuleoObject obj = new ModuleoObject(Session["Login"].ToString(), "po", "ponum");
    string wherestring = "", wherestringlinq = "";

    obj.CreateLinqCondition(nvc, ref wherestring, ref wherestringlinq);
    //string jscript = "<script type=\"text/javascript\">";
    string jscript = "";
    jscript += "var wherestring = '" + Server.UrlEncode(wherestringlinq) + "';var obj=parent.controlpanel.document.location.href='popanel.aspx?showlist=queryresult&wherestring='+wherestring;";
    //jscript += "</script>";
    litScript1.Text = jscript;
  }

  private Boolean ModifyPO()
  {
    SavePO();
    string newdivision = hidNewDivision.Value;
    string newparentid = hidNewParentID.Value;
    string newstatus = hidNewStatus.Value;
    string newparentdesc = hidNewParentDesc.Value;
    string moddate = hidModDate.Value;

    ModuleoObject po = new ModuleoObject(Session["Login"].ToString(), "po", "ponum", m_ponum);

    CultureInfo c = new CultureInfo(Session.LCID);
    DateTime TransDate = Convert.ToDateTime(moddate, c);

    litScript1.Text = "document.location.href='pomain.aspx?mode=edit&ponum=" + m_ponum + "'";
    return true;
  }

  private Boolean SavePO()
  {
    NameValueCollection nvc;
    Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
    TextBox tbx = null;
    string dirtylog = "0";
    if (CntlPanel != null)
    {
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
    bool success = false;
    if (querymode == "new")
    {
      poobj = new PO(Session["Login"].ToString(), "po", "ponum");
      success = poobj.Create(nvc);
    }
    else if (querymode == "duplicate")
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
    else if (querymode == "edit")
    {
      poobj = new PO(Session["Login"].ToString(), "po", "ponum", m_ponum);
      success = poobj.Update(nvc);
      if (success)
      {
        success = poobj.UpdatePOTotalAmount(m_ponum);
      }
    }
    else
    { poobj = new PO(Session["Login"].ToString(), "po", "ponum"); }

    //if (!success)
    //{
    //    string jscript = "<script>alert(\"" + poobj.ErrorMessage.Replace("\r\n", "") + "\");</script>";
    //    litFrameScript.Text = jscript;
    //}
    //else
    //{
    //    if (m_mode == "new")
    //    { Response.Redirect("pomain.aspx?mode=edit&ponum=" + Server.UrlEncode(nvc["ponum"])); }
    //    else
    //    {
    //        if (m_mode == "duplicate")
    //            Response.Redirect("pomain.aspx?mode=edit&ponum=" + Server.UrlEncode(nvc["ponum"]));
    //        else if (m_mode == "edit")
    //        {
    //            //tbx.Text = (Convert.ToInt32(dirtylog) + 1).ToString();
    //            Response.Redirect("pomain.aspx?mode=edit&ponum=" + Server.UrlEncode(m_ponum));// Server.UrlEncode(nvc["ponum"]));
    //        }
    //    }
    //}

    return success;
  }

  private void SaveStatus()
  {
    SavePO();

    NameValueCollection nvc = new NameValueCollection();
    nvc.Add("statuscode", hidTargetStatusCode.Value);
    nvc.Add("status", hidTargetStatus.Value);
    nvc.Add("Remark", hidStatusComments.Value);
    PO objpo = new PO(Session["Login"].ToString(), "po", "ponum", m_ponum);

    int statuscode = Convert.ToInt32(hidTargetStatusCode.Value);
    if (statuscode >= 300 && statuscode < 500)
    {
      if (nvc["closedate"] == "" || nvc["closedate"] == null)
      {
        nvc["closedate"] = DateTime.Now.ToShortDateString();
      }
    }

    bool success = objpo.UpdateStatus(nvc);
    if (success)
    {
      litScript1.Text = "document.location.href='pomain.aspx?mode=edit&ponum=" + m_ponum + "';";
    }
    else
      litScript1.Text = "alert('" + objpo.ErrorMessage + "');";
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