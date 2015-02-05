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

public partial class proc_procmain : System.Web.UI.Page
{
  AzzierScreen screen;
  private string connstring;
  protected string querymode = "";
  protected string m_procnum;
  protected string m_equipID;
  protected NameValueCollection nvcEQUIP;
  Procedure objProcedure;
  protected NameValueCollection nvcPROC;
  protected RadGrid grdproclabour;
  protected RadGrid grdprocmatl;
  protected RadGrid grdprocservice;
  protected RadGrid grdproctasks;
  protected RadGrid grdproctools;
  protected bool isquery = false;
  //protected int statuscode = 0;
  protected NameValueCollection m_msg = new NameValueCollection();
  protected NameValueCollection m_rights;
  protected int m_allowedit = 0;
  //protected RadCalendar sharedcalendar;

  protected void Page_Init(object sender, EventArgs e)
  {
    RetrieveMessage();
    UserRights.CheckAccess("procedure");

    UserRights r = new UserRights(Session["Login"].ToString(), "UserRights", "counter");
    m_rights = r.GetRights(Session["Login"].ToString(), "procedure");
    
    m_allowedit = Convert.ToInt16(m_rights["urEdit"].ToString());

    Session.LCID = Convert.ToInt32(Session["LCID"]);
    if (Request.QueryString["mode"] != null)
      querymode = Request.QueryString["mode"].ToString();
    else
      querymode = "query";
      
    if (querymode == "")
      querymode = "query";

    if (Request.QueryString["procnum"] != null)
      m_procnum = Request.QueryString["procnum"].ToString();
    else
      m_procnum = "";

    if (querymode == "query")
      isquery = true;

    if (querymode == "edit")
    {
      objProcedure = new Procedure(Session["Login"].ToString(), "Procedures", "procnum", m_procnum);  // use the view name
      nvcPROC = objProcedure.ModuleData;
      if (nvcPROC["procnum"].ToString() == "")
      {
        querymode = "query";
        isquery = true;
        m_procnum = "";
      }
        
      //statuscode = Convert.ToInt16(nvcWO["StatusCode"]);
      UserWorkList list = new UserWorkList();
      list.AddToRecentList(Session["Login"].ToString(), "Procedure", m_procnum);
      Division d = new Division();
      if (!d.Editable("'" + nvcPROC["Division"].ToString() + "'"))
        m_allowedit = 0;
      }
      else // new
      {
        objProcedure = new Procedure(Session["Login"].ToString(), "Procedures", "ProcNum");
        nvcPROC = objProcedure.ModuleData;
        //statuscode = 1;
      }

      if (nvcPROC["MasterRec"] == "1")
      {
        if (m_rights["urMaster"] != "1")
          m_allowedit = 0;

      }

      hidMode.Value = querymode;
      connstring = Application["ConnString"].ToString();
      SetGrdDataSource(connstring);
      //sharedcalendar = new RadCalendar();
      //sharedcalendar.SkinID = "Outlook";
      //sharedcalendar.ID = "SharedCalendar";
      //MainControlsPanel.Controls.Add(sharedcalendar);

      InitScreen();
    }

    private void InitScreen()
    {
      //if (nvcWO["StatusCode"].ToString() != "" || nvcWO["StatusCode"].ToString().Length > 0)
      //statuscode = Convert.ToInt32(nvcWO["StatusCode"].ToString());

      screen = new AzzierScreen("proc/procmain.aspx", "MainForm", MainControlsPanel.Controls, querymode);
      screen.LCID = Session.LCID;

      grdprocservice = new RadGrid();
      grdproclabour = new RadGrid();
      grdprocmatl = new RadGrid();
      grdproctasks = new RadGrid();
      grdproctools = new RadGrid();
      if (querymode == "edit")
      {
        // Service
          
        InitAllGrid(grdprocservice, "procservice", "ProcServiceSqlDataSource");

        grdprocservice.ItemDataBound += new GridItemEventHandler(grdprocservice_ItemDataBound);
        grdprocservice.DeleteCommand += new GridCommandEventHandler(grdprocservice_DeleteCommand);
        grdprocservice.UpdateCommand += new GridCommandEventHandler(grdprocservice_UpdateCommand);
        grdprocservice.InsertCommand += new GridCommandEventHandler(grdprocservice_InsertCommand);
        grdprocservice.ItemCommand += new GridCommandEventHandler(grdprocservice_ItemCommand);
        grdprocservice.DataBound += new EventHandler(grid_DataBound);

        //grdprocservice.MasterTableView.Columns.Add(sercolLastprice);
        //grdprocservice.MasterTableView.Columns.Add(sercolQuoteprice);

        // Labour
          
        InitAllGrid(grdproclabour, "proclabour", "ProcLabourSqlDataSource");

        grdproclabour.ItemDataBound += new GridItemEventHandler(grdproclabour_ItemDataBound);
        grdproclabour.DeleteCommand += new GridCommandEventHandler(grdproclabour_DeleteCommand);
        grdproclabour.UpdateCommand += new GridCommandEventHandler(grdproclabour_UpdateCommand);
        grdproclabour.InsertCommand += new GridCommandEventHandler(grdproclabour_InsertCommand);
        grdproclabour.ItemCommand += new GridCommandEventHandler(grdproclabour_ItemCommand);
        grdproclabour.DataBound += new EventHandler(grid_DataBound);

        // Material
          
        InitAllGrid(grdprocmatl, "procmatl", "ProcMatlSqlDataSource");

        grdprocmatl.ItemDataBound += new GridItemEventHandler(grdprocmatl_ItemDataBound);
        grdprocmatl.DeleteCommand += new GridCommandEventHandler(grdprocmatl_DeleteCommand);
        grdprocmatl.UpdateCommand += new GridCommandEventHandler(grdprocmatl_UpdateCommand);
        grdprocmatl.InsertCommand += new GridCommandEventHandler(grdprocmatl_InsertCommand);
        grdprocmatl.ItemCommand += new GridCommandEventHandler(grdprocmatl_ItemCommand);
        grdprocmatl.DataBound += new EventHandler(grid_DataBound);

        // Tasks
          
        InitAllGrid(grdproctasks, "proctasks", "ProcTasksSqlDataSource");

        grdproctasks.ItemDataBound += new GridItemEventHandler(grdproctasks_ItemDataBound);
        grdproctasks.DeleteCommand += new GridCommandEventHandler(grdproctasks_DeleteCommand);
        grdproctasks.UpdateCommand += new GridCommandEventHandler(grdproctasks_UpdateCommand);
        grdproctasks.InsertCommand += new GridCommandEventHandler(grdproctasks_InsertCommand);
        grdproctasks.ItemCommand += new GridCommandEventHandler(grdproctasks_ItemCommand);

        // Tools
          
        InitAllGrid(grdproctools, "proctools", "ProcToolsSqlDataSource");

        grdproctools.ItemDataBound += new GridItemEventHandler(grdproctools_ItemDataBound);
        grdproctools.DeleteCommand += new GridCommandEventHandler(grdproctools_DeleteCommand);
        grdproctools.UpdateCommand += new GridCommandEventHandler(grdproctools_UpdateCommand);
        grdproctools.InsertCommand += new GridCommandEventHandler(grdproctools_InsertCommand);
        grdproctools.ItemCommand += new GridCommandEventHandler(grdproctools_ItemCommand);
        grdproctools.DataBound += new EventHandler(grid_DataBound);

        grdproclabour.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(1, true, "Labour", null, "return editLabour('',1)", m_allowedit);
        grdprocmatl.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(1, true, "Material", null, "return editMaterial('',1)", m_allowedit);
        grdproctasks.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(1, true, "Tasks", null, "return editTask('',1)", m_allowedit,"",true,"","","return addfromtasklib('PROCEDURE','" + m_procnum + "','1');","Add Task Library");
        grdprocservice.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(1, true, "Services", null, "return editService('',1)", m_allowedit);
        grdproctools.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(1, true, "Tools", null, "return editTools('',1)", m_allowedit);

        //Add three hidden fileds (HidSysPrice, HidLastPrice, HidQuotedPrice) in grdwomatlact, grdprocmatl  
        GridBoundColumn newcol = new GridBoundColumn();
        newcol.DataField = "markuppercent";
        newcol.UniqueName = "markuppercent";
        newcol.Display = false;
        grdprocmatl.MasterTableView.Columns.Add(newcol);

        newcol = new GridBoundColumn();
        newcol.DataField = "HidLastPrice";
        newcol.UniqueName = "HidLastPrice";
        newcol.Display = false;
        grdprocmatl.MasterTableView.Columns.Add(newcol);

        newcol = new GridBoundColumn();
        newcol.DataField = "HidAvgPrice";
        newcol.UniqueName = "HidAvgPrice";
        newcol.Display = false;
        grdprocmatl.MasterTableView.Columns.Add(newcol);

        newcol = new GridBoundColumn();
        newcol.DataField = "HidQuotedPrice";
        newcol.UniqueName = "HidQuotedPrice";
        newcol.Display = false;
        grdprocmatl.MasterTableView.Columns.Add(newcol);

        newcol = new GridBoundColumn();
        newcol.DataField = "HidFixPrice";
        newcol.UniqueName = "HidFixPrice";
        newcol.Display = false;
        grdprocmatl.MasterTableView.Columns.Add(newcol);


        newcol = new GridBoundColumn();
        newcol.DataField = "IssuePrice";
        newcol.UniqueName = "HidIssuePrice";
        newcol.Display = false;
        grdprocmatl.MasterTableView.Columns.Add(newcol);

        

        newcol = new GridBoundColumn();
        newcol.DataField = "HidLastPrice";
        newcol.UniqueName = "HidLastPrice";
        newcol.Display = false;
        grdprocservice.MasterTableView.Columns.Add(newcol);

        newcol = new GridBoundColumn();
        newcol.DataField = "HidQuotedPrice";
        newcol.UniqueName = "HidQuotedPrice";
        newcol.Display = false;
        grdprocservice.MasterTableView.Columns.Add(newcol);

          MainControlsPanel.Controls.Add(grdprocmatl);
          MainControlsPanel.Controls.Add(grdproclabour);
          MainControlsPanel.Controls.Add(grdprocservice);
          MainControlsPanel.Controls.Add(grdproctasks);
          MainControlsPanel.Controls.Add(grdproctools);
        }

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
                screen.PopulateScreen("Procedures", nvcPROC);
                hidprocnum.Value = nvcPROC["ProcNum"];
                
                m_equipID = nvcPROC["equipment"];
                if (m_equipID != "")
                {
                    Equipment objEquipment = new Equipment(Session["Login"].ToString(), "equipment", "equipment", m_equipID);
                    nvcEQUIP = objEquipment.ModuleData;
                    if (nvcEQUIP != null)
                    {
                        HidMobile.Value = nvcEQUIP["MobileEquipment"].ToString();
                    }
                }
                
            }

            RadioButtonList rblmasterrec = (RadioButtonList)MainControlsPanel.FindControl("rblmasterrec");

            if (rblmasterrec != null)
            {
                rblmasterrec.Style.Add("valign", "top");
                rblmasterrec.RepeatDirection = RepeatDirection.Horizontal;
                ListItem litm1 = new ListItem("Yes", "1");
                rblmasterrec.Items.Add(litm1);
                ListItem litm2 = new ListItem("No", "0");
                rblmasterrec.Items.Add(litm2);
                if (querymode == "query")
                {
                  ListItem litm3 = new ListItem("All", "");
                  rblmasterrec.Items.Add(litm3);
                  rblmasterrec.SelectedIndex = 2;
                }
                else if (querymode == "edit" || querymode == "duplicate")
                {
                  if (nvcPROC["MasterRec"].ToString() == "1")
                    rblmasterrec.SelectedIndex = 0;
                  else
                    rblmasterrec.SelectedIndex = 1;
                }
                else if (querymode == "new")
                  rblmasterrec.SelectedIndex = 1;
            }

            if (querymode != "query")
            {
              TextBox tbx = MainControlsPanel.FindControl("txtlocation") as TextBox;
              tbx.Attributes.Add("onFocus", "locFocus()");

              HyperLink h = MainControlsPanel.FindControl("lkulocation") as HyperLink;
              if (h.ToString() != "")
              {
                h.NavigateUrl = "javascript:loclookup('" + m_msg["T38"] + "')";
              }
            }

            string tax1 = "";
            string tax2 = "";
            string seltype = "E";

            HidLabTax1.Value = tax1;
            HidLabTax2.Value = tax2;
            //HidLabSelType.Value = seltype;
            HidLabCost.Value = Application["LabCost"].ToString();

            //HidSysMatPrice.Value = Application["IssuePrice"].ToString();
            HidSysServicePrice.Value = Application["IssueServicePrice"].ToString();
            HidMatCost.Value = Application["MatCost"].ToString();
            if (Application["IncludeTax1Issuing"].ToString().ToLower() == "yes")
              HidMatTax1.Value = Application["Tax1"].ToString();
            else
              HidMatTax1.Value = "0";
            if (Application["IncludeTax2Issuing"].ToString().ToLower() == "yes")
              HidMatTax2.Value = Application["Tax2"].ToString();
            else
              HidMatTax2.Value = "0";

            if (Application["WOTax"].ToString().ToLower() == "yes")
            {
                HidServTax1.Value = Application["Tax1"].ToString();
                HidServTax2.Value = Application["Tax2"].ToString();
            }
            else
            {
                HidServTax1.Value = "0";
                HidServTax2.Value = "0";
            }

            if (Application["WOTax"].ToString().ToLower() == "yes")
            {
                HidToolTax1.Value = Application["Tax1"].ToString();
                HidToolTax2.Value = Application["Tax2"].ToString();
            }
            else
            {
                HidToolTax1.Value = "0";
                HidToolTax2.Value = "0";
            }

            // add radio button options
            RadioButtonList r = (RadioButtonList) MainControlsPanel.FindControl("rblcbcode");
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
                if (nvcPROC["cbcode"].ToString() == "1")
                  r.SelectedIndex = 0;
                else
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
                if (nvcPROC["Inactive"].ToString() == "1")
                  r.SelectedIndex = 0;
                else
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
        ucHeader1.ModuleData = nvcPROC;

        string oper = "";
        if (querymode == "query")
        {
            oper = "Query";
        }
        else if (querymode == "new")
        {
            oper = "New";
            screen.GetDefaultValue("Procedures", nvcPROC, "Procedure", "new");
            screen.PopulateScreen("Procedures", nvcPROC);
        }
        else if (querymode == "edit")
        {
            oper = "Edit";

        }
        ucHeader1.OperationLabel = oper;
        if (querymode == "edit")
        {
            // set controls to disabled in edit mode
            /*
            TextBox tbx = MainControlsPanel.FindControl("txtprocnum") as TextBox;
            tbx.ReadOnly = true;
            tbx.Attributes.Add("readonly", "readonly");
            HyperLink hlk = MainControlsPanel.FindControl("lkuwonum") as HyperLink;
            if (hlk != null)
                hlk.Visible = false;
             * */
        }
        else if (querymode == "new")
        {
          TextBox t;
            if (Request.QueryString["numbering"] == "auto")
            {
                t = MainControlsPanel.FindControl("txtprocnum") as TextBox;
                if (t != null)
                {
                    if (!Page.IsPostBack)
                    {
                        NewNumber num = new NewNumber();
                        t.Text = num.GetNextNumber("PR");
                    }
                }
            }
        }
        //hidTargetStatusCode.Value = statuscode.ToString();
    }

    private void Query()
    {
        NameValueCollection nvc;
        nvc = screen.CollectFormValues("Procedures", true);  // use the view name

        Procedure objProcedure = new Procedure(Session["Login"].ToString(), "Procedures", "procnum");   // use the view name
        string wherestring = "",wherestringlinq = "";
        //string[] procnums = objProcedure.Query(nvc, ref wherestring);
        objProcedure.CreateLinqCondition(nvc, ref wherestring, ref wherestringlinq);
        string jscript = "";
        jscript += "var wherestring = '" + Server.UrlEncode(wherestringlinq) + "'; var obj=parent.controlpanel.document.location.href='procpanel.aspx?showlist=queryresult&wherestring='+wherestring;";
 
        litScript1.Text = jscript;
    }

    protected void grdproclabour_DeleteCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        //Get the GridDataItem of the RadGrid
        GridDataItem item = (GridDataItem)e.Item;
        //Get the primary key value using the DataKeyValue.       
        string counter = item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString();

        Labour objlabour = new Labour(Session["Login"].ToString(), "WOLabour", "Counter", counter);
        bool success = objlabour.Delete();
        objlabour.UpdatePMLabour();

        if (!success)
        {
            grdproclabour.Controls.Add(new LiteralControl(m_msg["T3"] + objlabour.ErrorMessage));
            e.Canceled = true;
        }
        SetCurrentWorkOrderNVC();
    }

    protected void grdproclabour_UpdateCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        //Get the GridEditableItem of the RadGrid
        NameValueCollection nvcFT = screen.GetGridFieldTypes("proclabour", "WoLabour");
        string[] fields = nvcFT.AllKeys;
        NameValueCollection nvc = new NameValueCollection();
        DateFormat objDateFormat = new DateFormat(Session.LCID);
        string counter = "";
        GridEditableItem editedItem = e.Item as GridEditableItem;
        counter = editedItem.OwnerTableView.DataKeyValues[editedItem.ItemIndex]["Counter"].ToString();
        if (e.Item.OwnerTableView.EditMode == GridEditMode.InPlace)
        {
            //Get the primary key value using the DataKeyValue.

            foreach (string field in fields)
            {
                if (field != "counter" && field != "wonum" && field != "estimate" && field!="ordertype")
                {
                    string dbtype = nvcFT[field].ToString();
                    if (dbtype == "system.string" || dbtype == "system.decimal")
                    {
                      if (field.ToLower() == "worate")
                      {
                        string worate = (editedItem[field].Controls[0] as TextBox).Text;
                        if (worate != "")
                        {
                          worate = (Convert.ToDecimal(worate) / 100).ToString();
                          nvc.Add(field, worate);
                        }
                        else
                          nvc.Add(field, "0");
                      }
                      else
                        nvc.Add(field, (editedItem[field].Controls[0] as TextBox).Text);
                    }
                    //else if (dbtype == "system.decimal")
                    //{
                    //   nvc.Add(field, (editedItem[field].Controls[0] as RadNumericTextBox).Text);
                    //}
                    else if (dbtype == "system.datetime")
                    {
                        string sdate = (editedItem[field].Controls[0] as RadDatePicker).SelectedDate.ToString();
                        nvc.Add(field, sdate);
                    }
                }
            }
        }

        Labour objlabour = new Labour(Session["Login"].ToString(), "WoLabour", "Counter", counter);
        bool success = objlabour.Update(nvc);
        objlabour.UpdatePMLabour();

        if (!success)
        {
            grdproclabour.Controls.Add(new LiteralControl(m_msg["T5"] + objlabour.ErrorMessage));
            e.Canceled = true;
        }
        SetCurrentWorkOrderNVC();
    }

    protected void grdproclabour_InsertCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        //Get the GridEditFormInsertItem of the RadGrid

        DateFormat objDateFormat = new DateFormat(Session.LCID);
        NameValueCollection nvcFT = screen.GetGridFieldTypes("proclabour", "WoLabour");
        string[] fields = nvcFT.AllKeys;
        NameValueCollection nvc = new NameValueCollection();

        if (e.Item.OwnerTableView.EditMode == GridEditMode.InPlace)
        {
            GridDataInsertItem insertedItem = e.Item as GridDataInsertItem;
            foreach (string field in fields)
            {
                if (field != "counter" && field != "wonum" && field != "estimate" && field!="ordertype")
                {
                    string dbtype = nvcFT[field].ToString();
                    if (dbtype == "system.string" || dbtype == "system.decimal")
                    {
                      if (field.ToLower() == "worate")
                      {
                        string worate = (insertedItem[field].Controls[0] as TextBox).Text;
                        if (worate != "")
                        {
                          worate = (Convert.ToDecimal(worate) / 100).ToString();
                          nvc.Add(field, worate);
                        }
                        else
                          nvc.Add(field, "0");
                      }
                      else
                        nvc.Add(field, (insertedItem[field].Controls[0] as TextBox).Text);
                    }
                    /*
                  else if (dbtype == "system.decimal")
                  {
                    nvc.Add(field, (insertedItem[field].Controls[0] as RadNumericTextBox).Text);
                  }
                     * */
                    else if (dbtype == "system.datetime")
                    {
                        string sdate = (insertedItem[field].Controls[0] as RadDatePicker).SelectedDate.ToString();
                        nvc.Add(field, sdate);
                    }
                }
            }
        }
        else
        {
            GridEditableItem insertedItem = e.Item as GridEditableItem;
            UserControl uc = (UserControl)insertedItem.FindControl(GridEditFormItem.EditFormUserControlID);
            foreach (string field in fields)
            {
                if (field != "counter" && field != "wonum" && field != "estimate")
                {
                    string dbtype = nvcFT[field].ToString();
                    nvc.Add(field, (uc.FindControl("txt" + field) as TextBox).Text);
                }
            }
        }
        nvc.Add("wonum", m_procnum);
        nvc.Add("estimate", "1");
        nvc.Add("ordertype", "procedure");

        Labour objlabour = new Labour(Session["Login"].ToString(), "WoLabour", "Counter");
        bool success = objlabour.Create(nvc);
        objlabour.UpdatePMLabour();

        if (!success)
        {
          grdproclabour.Controls.Add(new LiteralControl(m_msg["T7"] + objlabour.ErrorMessage));
          e.Canceled = true;
        }
        SetCurrentWorkOrderNVC();
    }

    protected void grdproclabour_ItemDataBound(object sender, GridItemEventArgs e)
    {
      if (e.Item is GridCommandItem)
      {
        GridCommandItem commandItem = (GridCommandItem)e.Item;

        ImageButton t_addInPlaceButton = (ImageButton)commandItem.FindControl("addInPlaceButton");
        if (t_addInPlaceButton != null)
        {
          t_addInPlaceButton.ValidationGroup = "proclabour";
        }
      }

      if (e.Item is GridEditableItem && e.Item.IsInEditMode)
      {
        GridEditableItem editedItem = (GridEditableItem)e.Item;
        ImageButton t_imagebtn;
        if (e.Item.OwnerTableView.IsItemInserted)
        {
          t_imagebtn = ((ImageButton)editedItem.FindControl("PerformInsertButton"));
          if (t_imagebtn != null)
          {
            t_imagebtn.ValidationGroup = "proclabour";
            t_imagebtn.OnClientClick = "return addtopm('labour')";
          }
        }
        else
        {
          t_imagebtn = ((ImageButton)editedItem.FindControl("UpdateButton"));
          if (t_imagebtn != null)
          {
            t_imagebtn.ValidationGroup = "proclabour";
            t_imagebtn.OnClientClick = "return addtopm('labour')";
          }
        }
        ConditionalChecking.AddLabourCheck(editedItem,"labour");
        if (e.Item.OwnerTableView.IsItemInserted)
        {
            screen.InitResourceRecord("procedure", null, e, "mainpanel", "labour", "1", m_procnum);
        }
      }

      if (e.Item is GridDataItem && !e.Item.IsInEditMode)
      {
        GridDataItem item = (GridDataItem)e.Item;
        ImageButton btn = (ImageButton)item["EditCommand"].Controls[0];
        btn.ImageUrl = "~/Images/Edit.gif";
        btn.OnClientClick = "return editLabour(" + item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString() + ",1)";

        if (m_allowedit == 1)
        {
          btn = (ImageButton)item["DeleteButton"].Controls[0];
          btn.OnClientClick = "return addtopm('labour')";
        }
      }

      screen.GridItemDataBound(e, "proc/procmain.aspx", "MainForm", "proclabour");
    }

    protected void grdproclabour_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
      if (e.CommandName.ToString() == "InPlaceInitInsert")
      {
        e.Canceled = true;
        System.Collections.Specialized.ListDictionary newValues = new System.Collections.Specialized.ListDictionary();
        newValues["WoNum"] = m_procnum;
        newValues["TransDate"] = DateTime.Today;
        newValues["Estimate"] = 1;
        newValues["Actual"] = 0;
        newValues["ChargeBack"] = nvcPROC["ChargeBack"];
        grdproclabour.MasterTableView.EditMode = GridEditMode.InPlace;
        e.Item.OwnerTableView.InsertItem(newValues);
      }
      else if (e.CommandName == "RowClick")
      {
        GridDataItem item = (GridDataItem)e.Item;
        item.Edit = true;
        grdproclabour.MasterTableView.EditMode = GridEditMode.InPlace;
        grdproclabour.Rebind();
      }
    }

    protected void grdprocmatl_DeleteCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        //Get the GridDataItem of the RadGrid
        GridDataItem item = (GridDataItem)e.Item;
        //Get the primary key value using the DataKeyValue.
        string counter = item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString();

        Material objmaterial = new Material(Session["Login"].ToString(), "WoMaterial", "Counter", counter);
        bool success = objmaterial.Delete();
        objmaterial.UpdatePMMaterial();

        if (!success)
        {
            grdprocmatl.Controls.Add(new LiteralControl(m_msg["T9"] + objmaterial.ErrorMessage));
            e.Canceled = true;
        }
        SetCurrentWorkOrderNVC();
    }

    protected void grdprocmatl_UpdateCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        //Get the GridEditableItem of the RadGrid
        GridEditableItem editedItem = e.Item as GridEditableItem;
        //Get the primary key value using the DataKeyValue.
        string counter = editedItem.OwnerTableView.DataKeyValues[editedItem.ItemIndex]["Counter"].ToString();

        DateFormat objDateFormat = new DateFormat(Session.LCID);
        NameValueCollection nvcFT = screen.GetGridFieldTypes("procmatl", "WoMaterial");
        string[] fields = nvcFT.AllKeys;
        NameValueCollection nvc = new NameValueCollection();
        foreach (string field in fields)
        {
            if (field != "counter" && field != "wonum" && field != "estimate" && field!="ordertype")
            {
                string dbtype = nvcFT[field].ToString();
                if (dbtype == "system.string" || dbtype == "system.decimal")
                {
                    nvc.Add(field, (editedItem[field].Controls[0] as TextBox).Text);
                }
                else if (dbtype == "system.datetime")
                {
                    string sdate = (editedItem[field].Controls[0] as RadDatePicker).SelectedDate.ToString();
                    nvc.Add(field, sdate);
                }
            }
        }

        nvc.Add("estimate", "1");
        nvc.Add("wonum", m_procnum);
        nvc.Add("ordertype", "procedure");

        Material objmaterial = new Material(Session["Login"].ToString(), "WoMaterial", "Counter", counter);
        bool success = objmaterial.Update(nvc);
        objmaterial.UpdatePMMaterial();

        if (!success)
        {
            e.Canceled = true;
        }
        SetCurrentWorkOrderNVC();
    }

    protected void grdprocmatl_InsertCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        //Get the GridEditFormInsertItem of the RadGrid
        GridDataInsertItem insertedItem = e.Item as GridDataInsertItem;

        DateFormat objDateFormat = new DateFormat(Session.LCID);
        NameValueCollection nvcFT = screen.GetGridFieldTypes("procmatl", "WoMaterial");
        string[] fields = nvcFT.AllKeys;
        NameValueCollection nvc = new NameValueCollection();
        foreach (string field in fields)
        {
            if (field != "counter" && field != "wonum" && field != "estimate" && field != "ordertype")
            {
                string dbtype = nvcFT[field].ToString();
                if (dbtype == "system.string" || dbtype == "system.decimal")
                {
                    nvc.Add(field, (insertedItem[field].Controls[0] as TextBox).Text);
                }
                else if (dbtype == "system.datetime")
                {
                    string sdate = (insertedItem[field].Controls[0] as RadDatePicker).SelectedDate.ToString();
                    nvc.Add(field, sdate);
                }
            }
        }
        nvc.Add("wonum", m_procnum);
        nvc.Add("estimate", "1");
        nvc.Add("ordertype", "procedure");

        Material objmaterial = new Material(Session["Login"].ToString(), "WoMaterial", "Counter");
        bool success = objmaterial.Create(nvc);

        if (!success)
        {
            //grdwolabouract.Controls.Add(new LiteralControl(m_msg["T13"] + objResource.ErrorMessage));
            e.Canceled = true;
        }
        SetCurrentWorkOrderNVC();
    }

    protected void grdprocmatl_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        if (e.CommandName.ToString() == "InPlaceInitInsert")
        {
            // cancel the default operation
            e.Canceled = true;
            //Prepare an IDictionary with the predefined values
            System.Collections.Specialized.ListDictionary newValues = new System.Collections.Specialized.ListDictionary();
            newValues["WoNum"] = m_procnum;
            newValues["TransDate"] = DateTime.Today;
            newValues["Estimate"] = 1;
            newValues["ChargeBack"] = nvcPROC["ChargeBack"];
            //Insert the item and rebind
            e.Item.OwnerTableView.InsertItem(newValues);
        }
        else if (e.CommandName == "RowClick")
        {
            GridDataItem item = (GridDataItem)e.Item;
            item.Edit = true;
            grdprocmatl.MasterTableView.EditMode = GridEditMode.InPlace;
            grdprocmatl.Rebind();
        }
    }

    protected void grdprocmatl_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem && !e.Item.IsInEditMode)
        {
            GridDataItem item = (GridDataItem)e.Item;
            ImageButton btn = (ImageButton)item["EditCommand"].Controls[0];
            btn.ImageUrl = "~/Images/Edit.gif";
            btn.OnClientClick = "return editMaterial(" + item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString() + ",1)";
            if (m_allowedit == 1)
            {
              btn = (ImageButton)item["DeleteButton"].Controls[0];
              btn.OnClientClick = "return addtopm('material')";
            }
        }

        if (e.Item is GridCommandItem)
        {
            GridCommandItem commandItem = (GridCommandItem)e.Item;

            ImageButton t_addInPlaceButton = (ImageButton)commandItem.FindControl("addInPlaceButton");
            if (t_addInPlaceButton != null)
                t_addInPlaceButton.ValidationGroup = "procmatl";
        }

        screen.GridItemDataBound(e, "proc/procmain.aspx", "MainForm", "procmatl");
        if (e.Item is GridEditableItem && e.Item.IsInEditMode)
        {
            GridEditableItem editedItem = (GridEditableItem)e.Item;

            AssembleString assemble = new AssembleString(editedItem);
            ImageButton t_imagebtn;
            TextBox itemnumtext = editedItem["ItemNum"].Controls[0] as TextBox;
            TextBox storetext = editedItem["Store"].Controls[0] as TextBox;
            HyperLink lkustoreroom = editedItem["Store"].FindControl("lkustore") as HyperLink;
            storetext.Attributes.Add("onFocus", "gridstoreroomfocus('" + itemnumtext.ClientID + "')");
            storetext.Attributes["onchange"] = storetext.Attributes["onchange"] + ";resetitemnumattr('" + itemnumtext.ClientID + "','" + storetext.ClientID + "')";
            lkustoreroom.NavigateUrl = "javascript:gridstoreroomlookup('" + storetext.ClientID + "','" + itemnumtext.ClientID + "')";
            if (e.Item.OwnerTableView.IsItemInserted)
            {
              t_imagebtn = ((ImageButton)editedItem.FindControl("PerformInsertButton"));
              if (t_imagebtn != null)
                t_imagebtn.ValidationGroup = "procmatl";

              //itemnumtext.Attributes["fieldlist"] = assemble.ReplaceControlID("itemnum^_RT_itemnum^13,itemdesc^_RT_description^11,issueunit^_RT_unit,avgprice^_RT_HidAvgPrice^11,lastprice^_RT_HidLastPrice^11,quotedprice^_RT_HidQuotedPrice^11,fixprice^_RT_HidFixPrice,issueprice^_RT_HidIssuePrice");

            }
            else
            {
              t_imagebtn = ((ImageButton)editedItem.FindControl("UpdateButton"));
              if (t_imagebtn != null)
                t_imagebtn.ValidationGroup = "procmatl";
              GridDataItem data = editedItem.DataItem as GridDataItem;
              string store = ((DataRowView)e.Item.DataItem)["Store"].ToString();
              /*
              if (store == "")
                itemnumtext.Attributes["fieldlist"] = assemble.ReplaceControlID("itemnum^_RT_itemnum^13,itemdesc^_RT_description^11,issueunit^_RT_unit,avgprice^_RT_HidAvgPrice^11,lastprice^_RT_HidLastPrice^11,quotedprice^_RT_HidQuotedPrice^11,fixprice^_RT_HidFixPrice,issueprice^_RT_HidIssuePrice");
              else
                itemnumtext.Attributes["fieldlist"] = assemble.ReplaceControlID("itemnum^_RT_itemnum^11,itemdesc^_RT_description^11,issueunit^_RT_unit,avgprice^_RT_HidAvgPrice^11,lastprice^_RT_HidLastPrice^11,quotedprice^_RT_HidQuotedPrice^11,fixprice^_RT_HidFixPrice,issueprice^_RT_HidIssuePrice");
               * */
              if (store != "")
                //itemnumtext.Attributes["fieldlist"] = assemble.ReplaceControlID("itemnum^_RT_itemnum^11,itemdesc^_RT_description^11,issueunit^_RT_unit,avgprice^_RT_HidAvgPrice^11,lastprice^_RT_HidLastPrice^11,quotedprice^_RT_HidQuotedPrice^11,fixprice^_RT_HidFixPrice,issueprice^_RT_HidIssuePrice");
                itemnumtext.Attributes["FieldList"] = itemnumtext.Attributes["FieldList"].Replace("_ItemNum^13", "_ItemNum^11");
            }
            if (e.Item.OwnerTableView.IsItemInserted)
            {
                screen.InitResourceRecord("procedure", null, e, "mainpanel", "material", "1", m_procnum);
            }
            /*
            ImageButton t_imagebtn;
            if (e.Item.OwnerTableView.IsItemInserted)
            {
                t_imagebtn = ((ImageButton)editedItem.FindControl("PerformInsertButton"));
                if (t_imagebtn != null)
                {
                  t_imagebtn.ValidationGroup = "procmatl";
                  t_imagebtn.OnClientClick = "return addtopm('material')";
                }
            }
            else
            {
                t_imagebtn = ((ImageButton)editedItem.FindControl("UpdateButton"));
                if (t_imagebtn != null)
                {
                  t_imagebtn.ValidationGroup = "procmatl";
                  t_imagebtn.OnClientClick = "return addtopm('material')";
                }
            }
             * */
        }

        
    }

    private Boolean SaveWO() //??
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        TextBox tbx = null;
        string dirtylog = "0";
        if (CntlPanel != null)
        {
            nvc = screen.CollectFormValues("Procedures", false);  // use the table name
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

        Procedure objProcedure;
        bool success = false;
        if (querymode == "new")
        {
            objProcedure = new Procedure(Session["Login"].ToString(), "Procedures", "ProcNum");   // use the table name
            success = objProcedure.Create(nvc);
            if (success)
            {
              if (nvc["procnum"] != "")
              {
                //Labour l = new Labour(Session["Login"].ToString(), "ProcLabor", "Counter");
                //l.CopyLabour("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                //Service s = new Service(Session["Login"].ToString(), "WOService", "Counter");
                //s.CopyService("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                //Material m = new Material(Session["Login"].ToString(), "WOMaterial", "Counter");
                //m.CopyMaterial("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                //Tool t = new Tool(Session["Login"].ToString(), "WOTools", "Counter");
                //t.CopyTools("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                //Task task = new Task(Session["Login"].ToString(), "Tasks", "Counter");
                //task.CopyTasks("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
              }
            }
        }
        else if (querymode == "edit")
        {
            string wo = nvc["procnum"];
            objProcedure = new Procedure(Session["Login"].ToString(), "Procedures", "ProcNum", wo);   // use the table name
            success = objProcedure.Update(nvc);
            if (success)
            {
              string oldproc = hidprocnum.Value;
              if ((nvc["procnum"].ToString() != "") && (nvc["procnum"].ToString() != oldproc))
              {
                Labour l = new Labour(Session["Login"].ToString(), "WoLabour", "Counter");
                l.DeleteAllLabour("procedures", nvc["procnum"], 1);
                //l.CopyLabour("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                //Service s = new Service(Session["Login"].ToString(), "WOService", "Counter");
                //s.CopyService("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                //Material m = new Material(Session["Login"].ToString(), "WOMaterial", "Counter");
                //m.CopyMaterial("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                //Tool t = new Tool(Session["Login"].ToString(), "WOTools", "Counter");
                //t.CopyTools("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                //Task task = new Task(Session["Login"].ToString(), "WOTasks", "Counter");
                //task.CopyTasks("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                hidprocnum.Value = nvc["procnum"];
              }
            }
        }
        else
            objProcedure = new Procedure(Session["Login"].ToString(), "Procedures", "ProcNum");
        return success;

    }

    private void CopyTasks()
    {
        string counters = "";
        for (int i = 0; i < grdproctasks.SelectedIndexes.Count; i++)
        {
            string counter = grdproctasks.SelectedItems[i].OwnerTableView.DataKeyValues[grdproctasks.SelectedItems[i].ItemIndex]["Counter"].ToString();
            if (counters == "")
                counters = counter;
            else
                counters = counters + "," + counter;
        }
        if (counters != "")
        {
            Task task = new Task(Session["Login"].ToString(), "Tasks", "Counter", m_procnum, 0);

            RadDatePicker dtp = null;
            GridItem item = grdproclabour.MasterTableView.GetItems(GridItemType.CommandItem)[0];
            dtp = (RadDatePicker)item.FindControl("rdpDate");
            //string transdate;
            DateTime transdate;
            if (dtp == null)
                transdate = DateTime.Today;
            else
                transdate = (DateTime)dtp.SelectedDate;
            if (task.CopyEstimateToActual(counters, transdate))
            {
                grdproctasks.Rebind();
            }
        }
    }

    protected void grdprocservice_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridCommandItem)
        {
            GridCommandItem commandItem = (GridCommandItem)e.Item;

            ImageButton t_addInPlaceButton = (ImageButton)commandItem.FindControl("addInPlaceButton");
            if (t_addInPlaceButton != null)
                t_addInPlaceButton.ValidationGroup = "porcserv";
        }

        if (e.Item is GridDataItem && !e.Item.IsInEditMode)
        {
            GridDataItem item = (GridDataItem)e.Item;
            ImageButton btn = (ImageButton)item["EditCommand"].Controls[0];
            btn.ImageUrl = "~/Images/Edit.gif";
            btn.OnClientClick = "return editService(" + item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString() + ",1)";
            if (m_allowedit == 1)
            {
                btn = (ImageButton)item["DeleteButton"].Controls[0];
                btn.OnClientClick = "return addtopm('service')";
            }
        }

        if (e.Item is GridEditableItem && e.Item.IsInEditMode)
        {
            GridEditableItem editedItem = (GridEditableItem)e.Item;
            ImageButton t_imagebtn;
            if (e.Item.OwnerTableView.IsItemInserted)
            {
                t_imagebtn = ((ImageButton)editedItem.FindControl("PerformInsertButton"));
                if (t_imagebtn != null)
                {
                  t_imagebtn.ValidationGroup = "procservice";
                  t_imagebtn.OnClientClick = "return addtopm('service')";
                }
            }
            else
            {
                t_imagebtn = ((ImageButton)editedItem.FindControl("UpdateButton"));
                if (t_imagebtn != null)
                {
                  t_imagebtn.ValidationGroup = "procservice";
                  t_imagebtn.OnClientClick = "return addtopm('service')";
                }
            }
            if (e.Item.OwnerTableView.IsItemInserted)
            {
                screen.InitResourceRecord("procedure", null, e, "mainpanel", "service", "1", m_procnum);
            }
        }

        screen.GridItemDataBound(e, "proc/procmain.aspx", "MainForm", "procservice");

        if (e.Item is GridEditableItem && e.Item.IsInEditMode)
        {
            GridEditableItem editedItem = (GridEditableItem)e.Item;
            TextBox txtservicecode = editedItem["ServiceCode"].Controls[0] as TextBox;
            TextBox txtvendor = editedItem["Vendor"].Controls[0] as TextBox;
            HyperLink lkuvendor = editedItem["Vendor"].FindControl("lkuvendor") as HyperLink;
            txtvendor.Attributes.Add("onFocus", "setReadOnlyControl('" + txtservicecode.ClientID + "')");
            lkuvendor.NavigateUrl = "javascript:setReadOnlyLookup('" + txtservicecode.ClientID + "','" + txtvendor.ClientID + "','Cannot Change vendor for fixed service code.')";
        }
    }

    protected void grdprocservice_DeleteCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        GridDataItem item = (GridDataItem)e.Item;
        string counter = item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString();

        Service objservice = new Service(Session["Login"].ToString(), "WoService", "Counter", counter);
        bool success = objservice.Delete();
        objservice.UpdatePMService();

        if (!success)
        {
            grdprocservice.Controls.Add(new LiteralControl(m_msg["T16"] + objservice.ErrorMessage));
            e.Canceled = true;
        }
        SetCurrentWorkOrderNVC();
    }

    protected void grdprocservice_UpdateCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        //Get the GridEditableItem of the RadGrid
        NameValueCollection nvcFT = screen.GetGridFieldTypes("procservice", "WoService");
        string[] fields = nvcFT.AllKeys;
        NameValueCollection nvc = new NameValueCollection();
        DateFormat objDateFormat = new DateFormat(Session.LCID);
        string counter = "";
        GridEditableItem editedItem = e.Item as GridEditableItem;
        counter = editedItem.OwnerTableView.DataKeyValues[editedItem.ItemIndex]["Counter"].ToString();
        if (e.Item.OwnerTableView.EditMode == GridEditMode.InPlace)
        {
            //Get the primary key value using the DataKeyValue.
            foreach (string field in fields)
            {
                if (field != "counter" && field != "wonum" && field != "estimate" && field != "ordertype")
                {
                    string dbtype = nvcFT[field].ToString();
                    if (dbtype == "system.string" || dbtype == "system.decimal")
                    {
                        nvc.Add(field, (editedItem[field].Controls[0] as TextBox).Text);
                    }
                    else if (dbtype == "system.datetime")
                    {
                        string sdate = (editedItem[field].Controls[0] as RadDatePicker).SelectedDate.ToString();
                        string result = "";
                        bool a = objDateFormat.ValidateInputDate(sdate, out result);    // convert date from screen format to db date format
                        sdate = result;
                        nvc.Add(field, sdate);
                    }
                }
            }
        }

        Service objservice = new Service(Session["Login"].ToString(), "WoService", "Counter", counter);
        bool success = objservice.Update(nvc);
        objservice.UpdatePMService();

        if (!success)
        {
            grdprocservice.Controls.Add(new LiteralControl(m_msg["T17"] + objservice.ErrorMessage));
            e.Canceled = true;
        }
        SetCurrentWorkOrderNVC();
    }

    protected void grdprocservice_InsertCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        DateFormat objDateFormat = new DateFormat(Session.LCID);
        NameValueCollection nvcFT = screen.GetGridFieldTypes("procservice", "WoService");
        string[] fields = nvcFT.AllKeys;
        NameValueCollection nvc = new NameValueCollection();

        if (e.Item.OwnerTableView.EditMode == GridEditMode.InPlace)
        {
            GridDataInsertItem insertedItem = e.Item as GridDataInsertItem;
            foreach (string field in fields)
            {
                if (field != "counter" && field != "wonum" && field != "estimate" && field!="ordertype")
                {
                    string dbtype = nvcFT[field].ToString();
                    if (dbtype == "system.string" || dbtype == "system.decimal")
                    {
                        nvc.Add(field, (insertedItem[field].Controls[0] as TextBox).Text);
                    }
                    else if (dbtype == "system.datetime")
                    {
                        string sdate = (insertedItem[field].Controls[0] as RadDatePicker).SelectedDate.ToString();
                        string result = "";
                        bool a = objDateFormat.ValidateInputDate(sdate, out result);
                        sdate = result;
                        nvc.Add(field, sdate);
                    }
                }
            }
        }
        
        nvc.Add("wonum", m_procnum);
        nvc.Add("estimate", "1");
        nvc.Add("ordertype", "procedure");
        Service objservice = new Service(Session["Login"].ToString(), "WoService", "Counter");
        bool success = objservice.Create(nvc);
        objservice.UpdatePMService();

        if (!success)
        {
            grdprocservice.Controls.Add(new LiteralControl(m_msg["T18"] + objservice.ErrorMessage));
            e.Canceled = true;
        }
        SetCurrentWorkOrderNVC();
    }

    protected void grdprocservice_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        #region void grdprocservice_ItemCommand
        if (e.CommandName.ToString() == "InPlaceInitInsert")
        {
            e.Canceled = true;
            System.Collections.Specialized.ListDictionary newValues = new System.Collections.Specialized.ListDictionary();
            newValues["WoNum"] = m_procnum;
            newValues["TransDate"] = DateTime.Today;
            newValues["Estimate"] = 1;
            newValues["Actual"] = 0;
            newValues["ChargeBack"] = nvcPROC["ChargeBack"];
            grdprocservice.MasterTableView.EditMode = GridEditMode.InPlace;
            e.Item.OwnerTableView.InsertItem(newValues);
        }
        else if (e.CommandName == RadGrid.EditCommandName)
        {
            grdprocservice.MasterTableView.EditMode = GridEditMode.PopUp;
        }
        else if (e.CommandName == "RowClick")
        {
            GridDataItem item = (GridDataItem)e.Item;
            item.Edit = true;
            grdprocservice.MasterTableView.EditMode = GridEditMode.InPlace;
            grdprocservice.Rebind();
        }
        #endregion
    }

    protected void grdproctools_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridCommandItem)
        {
            GridCommandItem commandItem = (GridCommandItem)e.Item;

            ImageButton t_addInPlaceButton = (ImageButton)commandItem.FindControl("addInPlaceButton");
            if (t_addInPlaceButton != null)
                t_addInPlaceButton.ValidationGroup = "proctools";
        }

        if (e.Item is GridDataItem && !e.Item.IsInEditMode)
        {
            GridDataItem item = (GridDataItem)e.Item;

            ImageButton btn = (ImageButton)item["EditCommand"].Controls[0];
            btn.ImageUrl = "~/Images/Edit.gif";
            btn.OnClientClick = "return editTools(" + item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString() + ",1)";

            if (m_allowedit == 1)
            {
                

                btn = (ImageButton)item["DeleteButton"].Controls[0];
                btn.OnClientClick = "return addtopm('tool')";
            }
        }

        if (e.Item is GridEditableItem && e.Item.IsInEditMode)
        {
            GridEditableItem editedItem = (GridEditableItem)e.Item;
            ImageButton t_imagebtn;
            if (e.Item.OwnerTableView.IsItemInserted)
            {
                t_imagebtn = ((ImageButton)editedItem.FindControl("PerformInsertButton"));
                if (t_imagebtn != null)
                {
                  t_imagebtn.ValidationGroup = "proctools";
                  t_imagebtn.OnClientClick = "return addtopm('tool')";
                }
            }
            else
            {
                t_imagebtn = ((ImageButton)editedItem.FindControl("UpdateButton"));
                if (t_imagebtn != null)
                {
                  t_imagebtn.ValidationGroup = "proctools";
                  t_imagebtn.OnClientClick = "return addtopm('tool')";
                }
            }
            ConditionalChecking.AddToolsCheck(editedItem, "proctools");
            if (e.Item.OwnerTableView.IsItemInserted)
            {
                screen.InitResourceRecord("procedure", null, e, "mainpanel", "tool", "1", m_procnum);
            }
        }

        screen.GridItemDataBound(e, "proc/procmain.aspx", "MainForm", "proctools");
    }

    protected void grdproctools_DeleteCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        GridDataItem item = (GridDataItem)e.Item;
        string counter = item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString();

        Tool objtool = new Tool(Session["Login"].ToString(), "WoTools", "Counter", counter);
        bool success = objtool.Delete();
        objtool.UpdatePMTool();
        if (!success)
        {
            grdproctools.Controls.Add(new LiteralControl(m_msg["T19"] + objtool.ErrorMessage));
            e.Canceled = true;
        }
        SetCurrentWorkOrderNVC();
    }

    protected void grdproctools_UpdateCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        //Get the GridEditableItem of the RadGrid
        NameValueCollection nvcFT = screen.GetGridFieldTypes("proctools", "WoTools");
        string[] fields = nvcFT.AllKeys;
        NameValueCollection nvc = new NameValueCollection();
        DateFormat objDateFormat = new DateFormat(Session.LCID);
        string counter = "";
        GridEditableItem editedItem = e.Item as GridEditableItem;
        counter = editedItem.OwnerTableView.DataKeyValues[editedItem.ItemIndex]["Counter"].ToString();
        if (e.Item.OwnerTableView.EditMode == GridEditMode.InPlace)
        {
            //Get the primary key value using the DataKeyValue.
            foreach (string field in fields)
            {
                if (field != "counter" && field != "wonum" && field != "estimate" && field != "ordertype")
                {
                    string dbtype = nvcFT[field].ToString();
                    if (dbtype == "system.string" || dbtype == "system.decimal")
                    {
                        nvc.Add(field, (editedItem[field].Controls[0] as TextBox).Text);
                    }
                    else if (dbtype == "system.datetime")
                    {
                        string sdate = (editedItem[field].Controls[0] as RadDatePicker).SelectedDate.ToString();
                        nvc.Add(field, sdate);
                    }
                }
            }
        }

        Tool objtool = new Tool(Session["Login"].ToString(), "WoTools", "Counter", counter);
        bool success = objtool.Update(nvc);
        objtool.UpdatePMTool();
        if (!success)
        {
            grdproctools.Controls.Add(new LiteralControl(m_msg["T20"] + objtool.ErrorMessage));
            e.Canceled = true;
        }
        SetCurrentWorkOrderNVC();
    }

    protected void grdproctools_InsertCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        DateFormat objDateFormat = new DateFormat(Session.LCID);
        NameValueCollection nvcFT = screen.GetGridFieldTypes("proctools", "WoTools");
        string[] fields = nvcFT.AllKeys;
        NameValueCollection nvc = new NameValueCollection();

        if (e.Item.OwnerTableView.EditMode == GridEditMode.InPlace)
        {
            GridDataInsertItem insertedItem = e.Item as GridDataInsertItem;
            foreach (string field in fields)
            {
                if (field != "counter" && field != "wonum" && field != "estimate")
                {
                    string dbtype = nvcFT[field].ToString();
                    if (dbtype == "system.string" || dbtype == "system.decimal")
                    {
                        nvc.Add(field, (insertedItem[field].Controls[0] as TextBox).Text);
                    }
                    else if (dbtype == "system.datetime")
                    {
                        string sdate = (insertedItem[field].Controls[0] as RadDatePicker).SelectedDate.ToString();
                        nvc.Add(field, sdate);
                    }
                }
            }
        }
        else
        {
            GridEditableItem insertedItem = e.Item as GridEditableItem;
            UserControl uc = (UserControl)insertedItem.FindControl(GridEditFormItem.EditFormUserControlID);
            foreach (string field in fields)
            {
                if (field != "counter" && field != "wonum" && field != "estimate")
                {
                    string dbtype = nvcFT[field].ToString();
                    nvc.Add(field, (uc.FindControl("txt" + field) as TextBox).Text);
                }
            }
        }
        nvc.Add("wonum", m_procnum);
        nvc.Add("estimate", "1");
        //nvc.Add("ordertype","procedure");
        nvc["ordertype"] = "procedure";
        
        Tool objtool = new Tool(Session["Login"].ToString(), "WoTools", "Counter");
        bool success = objtool.Create(nvc);
        objtool.UpdatePMTool();

        if (!success)
        {
            grdproctools.Controls.Add(new LiteralControl(m_msg["T21"] + objtool.ErrorMessage));
            e.Canceled = true;
        }
        SetCurrentWorkOrderNVC();
    }

    protected void grdproctools_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        if (e.CommandName.ToString() == "InPlaceInitInsert")
        {
          e.Canceled = true;
          System.Collections.Specialized.ListDictionary newValues = new System.Collections.Specialized.ListDictionary();
          newValues["WoNum"] = m_procnum;
          newValues["TransDate"] = DateTime.Today;
          newValues["Estimate"] = 1;
          newValues["Actual"] = 0;
          newValues["ChargeBack"] = nvcPROC["ChargeBack"];
          grdproctools.MasterTableView.EditMode = GridEditMode.InPlace;
          e.Item.OwnerTableView.InsertItem(newValues);
        }
        else if (e.CommandName == RadGrid.EditCommandName)
        {
          grdproctools.MasterTableView.EditMode = GridEditMode.PopUp;
        }
        else if (e.CommandName == "RowClick")
        {
          GridDataItem item = (GridDataItem)e.Item;
          item.Edit = true;
          grdproctools.MasterTableView.EditMode = GridEditMode.InPlace;
          grdproctools.Rebind();
        }
    }

    protected void grdproctasks_ItemDataBound(object sender, GridItemEventArgs e)
    {
      if (e.Item is GridCommandItem)
      {
        GridCommandItem commandItem = (GridCommandItem)e.Item;

        ImageButton t_addInPlaceButton = (ImageButton)commandItem.FindControl("addInPlaceButton");
        if (t_addInPlaceButton != null)
          t_addInPlaceButton.ValidationGroup = "proctasks";
      }
      if (e.Item is GridDataItem && !e.Item.IsInEditMode)
      {
        GridDataItem item = (GridDataItem)e.Item;

        ImageButton btn = (ImageButton)item["EditCommand"].Controls[0];
        btn.ImageUrl = "~/Images/Edit.gif";
        btn.OnClientClick = "return editTask(" + item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString() + ",1)";

        if (m_allowedit == 1)
        {
          

          btn = (ImageButton)item["DeleteButton"].Controls[0];
          btn.OnClientClick = "return addtopm('task')";
        }
      }

      if (e.Item is GridEditableItem && e.Item.IsInEditMode)
      {
        GridEditableItem editedItem = (GridEditableItem)e.Item;
        ImageButton t_imagebtn;
        if (e.Item.OwnerTableView.IsItemInserted)
        {
          t_imagebtn = ((ImageButton)editedItem.FindControl("PerformInsertButton"));
          if (t_imagebtn != null)
          {
            t_imagebtn.ValidationGroup = "proctasks";
            t_imagebtn.OnClientClick = "return addtopm('task')";
          }
        }
        else
        {
          t_imagebtn = ((ImageButton)editedItem.FindControl("UpdateButton"));
          if (t_imagebtn != null)
          {
            t_imagebtn.ValidationGroup = "proctasks";
            t_imagebtn.OnClientClick = "return addtopm('task')";
          }
        }
      }

      screen.GridItemDataBound(e, "proc/procmain.aspx", "MainForm", "proctasks");
    }

    protected void grdproctasks_DeleteCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        GridDataItem item = (GridDataItem)e.Item;
        string counter = item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString();

        Task objtask = new Task(Session["Login"].ToString(), "WOTasks", "Counter", counter);
        bool success = objtask.Delete();
        objtask.UpdatePMTask();
        if (!success)
        {
            grdproclabour.Controls.Add(new LiteralControl(m_msg["T25"] + objtask.ErrorMessage));
            e.Canceled = true;
        }
    }

    protected void grdproctasks_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        if (e.CommandName.ToString() == "InPlaceInitInsert")
        {
            e.Canceled = true;
            System.Collections.Specialized.ListDictionary newValues = new System.Collections.Specialized.ListDictionary();
            newValues["WoNum"] = m_procnum;
            newValues["TransDate"] = DateTime.Today;
            newValues["Estimate"] = 1;
            newValues["Actual"] = 0;
            grdproctasks.MasterTableView.EditMode = GridEditMode.InPlace;
            e.Item.OwnerTableView.InsertItem(newValues);
        }
        else if (e.CommandName == RadGrid.EditCommandName)
        {
          grdproctasks.MasterTableView.EditMode = GridEditMode.PopUp;
        }
        else if (e.CommandName == "RowClick")
        {
            GridDataItem item = (GridDataItem)e.Item;
            item.Edit = true;
            grdproctasks.MasterTableView.EditMode = GridEditMode.InPlace;
            grdproctasks.Rebind();
        }
    }

    protected void grdproctasks_InsertCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        DateFormat objDateFormat = new DateFormat(Session.LCID);
        NameValueCollection nvcFT = screen.GetGridFieldTypes("proctasks", "WOTasks");
        string[] fields = nvcFT.AllKeys;
        NameValueCollection nvc = new NameValueCollection();

        if (e.Item.OwnerTableView.EditMode == GridEditMode.InPlace)
        {
            GridDataInsertItem insertedItem = e.Item as GridDataInsertItem;
            foreach (string field in fields)
            {
                if (field != "counter" && field != "wonum" && field != "estimate" && field != "ordertype")
                {
                    string dbtype = nvcFT[field].ToString();
                    if (dbtype == "system.string" || dbtype == "system.decimal")
                    {
                        nvc.Add(field, (insertedItem[field].Controls[0] as TextBox).Text);
                    }
                    else if (dbtype == "system.datetime")
                    {
                        string sdate = (insertedItem[field].Controls[0] as RadDatePicker).SelectedDate.ToString();
                        nvc.Add(field, sdate);
                    }
                }
            }
        }

        nvc.Add("wonum", m_procnum);
        nvc.Add("estimate", "1");
        nvc.Add("ordertype", "procedure");
        Task objtask = new Task(Session["Login"].ToString(), "WOTasks", "Counter");
        bool success = objtask.Create(nvc);
        objtask.UpdatePMTask();

        if (!success)
        {
            grdproctasks.Controls.Add(new LiteralControl(m_msg["T26"] + objtask.ErrorMessage));
            e.Canceled = true;
        }
    }

    protected void grdproctasks_UpdateCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        //Get the GridEditableItem of the RadGrid
        NameValueCollection nvcFT = screen.GetGridFieldTypes("proctasks", "WOTasks");
        string[] fields = nvcFT.AllKeys;
        NameValueCollection nvc = new NameValueCollection();
        DateFormat objDateFormat = new DateFormat(Session.LCID);
        string counter = "";
        GridEditableItem editedItem = e.Item as GridEditableItem;
        counter = editedItem.OwnerTableView.DataKeyValues[editedItem.ItemIndex]["Counter"].ToString();
        if (e.Item.OwnerTableView.EditMode == GridEditMode.InPlace)
        {
            //Get the primary key value using the DataKeyValue.

            foreach (string field in fields)
            {
                if (field != "counter" && field != "wonum" && field != "estimate" && field != "ordertype")
                {
                    string dbtype = nvcFT[field].ToString();
                    if (dbtype == "system.string" || dbtype == "system.decimal")
                    {
                        nvc.Add(field, (editedItem[field].Controls[0] as TextBox).Text);
                    }
                    //else if (dbtype == "system.decimal")
                    //{
                    //   nvc.Add(field, (editedItem[field].Controls[0] as RadNumericTextBox).Text);
                    //}
                    else if (dbtype == "system.datetime")
                    {
                        string sdate = (editedItem[field].Controls[0] as RadDatePicker).SelectedDate.ToString();
                        nvc.Add(field, sdate);
                    }
                }
            }
        }

        Task objtask = new Task(Session["Login"].ToString(), "WOTasks", "Counter", counter);
        bool success = objtask.Update(nvc);
        objtask.UpdatePMTask();
        if (!success)
        {
            grdproctasks.Controls.Add(new LiteralControl(m_msg["T27"] + objtask.ErrorMessage));
            e.Canceled = true;
        }
    }

    protected void InitAllGrid(RadGrid grd, string controlid, string grddatasource)
    {
      grd.ID = string.Concat("grd", controlid);
      grd.DataSourceID = grddatasource;
      //grd.GridLines = GridLines.Both;
      //grd.MasterTableView.GridLines = GridLines.Both;
      grd.ClientSettings.EnableRowHoverStyle = true;
      grd.AutoGenerateColumns = false;
      grd.AllowSorting = true;
      grd.MasterTableView.AllowMultiColumnSorting = true;
      grd.ShowHeader = true;
      grd.ShowFooter = true;
      grd.ValidationSettings.EnableValidation = true;
      grd.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.Top;
      grd.Skin = "Outlook";

      GridEditCommandColumn EditColumn = new GridEditCommandColumn();
      EditColumn.HeaderText = "Edit";
      EditColumn.UniqueName = "EditCommand";
      EditColumn.ButtonType = GridButtonColumnType.ImageButton;
      EditColumn.EditImageUrl = "~/Images/Edit.gif";
      EditColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
      EditColumn.HeaderStyle.Width = 30;
      grd.MasterTableView.Columns.Add(EditColumn);

      screen.SetGridColumns(controlid, grd);

      if (m_allowedit == 1)
      {
        GridButtonColumn DeleteColumn = new GridButtonColumn();
        DeleteColumn.HeaderText = "Delete";
        DeleteColumn.UniqueName = "DeleteButton";
        DeleteColumn.CommandName = "Delete";
        DeleteColumn.ButtonType = GridButtonColumnType.ImageButton;
        DeleteColumn.ImageUrl = "~/Images/Delete.gif";
        DeleteColumn.Text = "Delete";
        DeleteColumn.ConfirmText = m_msg["T31"];
        DeleteColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        DeleteColumn.HeaderStyle.Width = 30;
        grd.MasterTableView.Columns.Add(DeleteColumn);
        grd.ClientSettings.EnablePostBackOnRowClick = true;
      }

      grd.MasterTableView.DataKeyNames = new string[] { "Counter" };
      grd.MasterTableView.EditMode = GridEditMode.InPlace;
    }

    public void SetGrdDataSource(string sqlStr)
    {
        ProcLabourSqlDataSource.ConnectionString = sqlStr;
        ProcLabourSqlDataSource.SelectCommand = "SELECT * FROM WOLabour WHERE WoNum='" + m_procnum + "' And ordertype='procedure'";

        ProcMatlSqlDataSource.ConnectionString = sqlStr;
        ProcMatlSqlDataSource.SelectCommand = "SELECT * FROM WOMaterial WHERE WoNum='" + m_procnum + "' AND ordertype='procedure'";

        ProcServiceSqlDataSource.ConnectionString = sqlStr;
        ProcServiceSqlDataSource.SelectCommand = "SELECT * FROM woservice WHERE WoNum='" + m_procnum + "' AND ordertype='procedure'";

        ProcTasksSqlDataSource.ConnectionString = sqlStr;
        ProcTasksSqlDataSource.SelectCommand = "SELECT * FROM wotasks WHERE WoNum='" + m_procnum + "' AND ordertype='procedure'";

        ProcToolsSqlDataSource.ConnectionString = sqlStr;
        ProcToolsSqlDataSource.SelectCommand = "SELECT * FROM wotools WHERE WoNum='" + m_procnum + "' AND ordertype='procedure'";
    }

    private void SetGridInvisible(RadGrid grid, HiddenField hid)
    {
        grid.Style["visibility"] = "hidden";
        grid.Style["display"] = "none";
        hid.Value = "0";
    }

    private void RetrieveMessage()
    {
        SystemMessage msg = new SystemMessage("proc/procmain.aspx");
        m_msg = msg.GetSystemMessage();
        msg.SetJsMessage(litMessage);
    }

    private void SetCurrentWorkOrderNVC()
    {
        Procedure objProcedure = new Procedure(Session["Login"].ToString(), "Procedures", "procnum", m_procnum);
        nvcPROC = objProcedure.ModuleData;

    }

    private void SetTextboxControls()
    {
        TextBox txtesthours = (TextBox)MainControlsPanel.FindControl("txtesthours");
        TextBox txtestlabor = (TextBox)MainControlsPanel.FindControl("txtestlabor");
        TextBox txtestmaterial = (TextBox)MainControlsPanel.FindControl("txtestmaterial");
        TextBox txtestservice = (TextBox)MainControlsPanel.FindControl("txtestservice");
        TextBox txtesttools = (TextBox)MainControlsPanel.FindControl("txtesttools");

        txtesthours.Text = nvcPROC.GetValues("esthours")[0];
        txtestlabor.Text = nvcPROC.GetValues("estlabor")[0];
        txtestmaterial.Text = nvcPROC.GetValues("estmaterial")[0];
        txtestservice.Text = nvcPROC.GetValues("estservice")[0];
        txtesttools.Text = nvcPROC.GetValues("esttools")[0];
    }

    private void grid_DataBound(object sender, EventArgs e)
    {
        SetTextboxControls();
    }
}