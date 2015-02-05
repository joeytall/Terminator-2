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

public partial class workrequest_Wrmain : System.Web.UI.Page
{
    protected AzzierScreen screen;
    private string connstring;
    protected string querymode = "";
    protected string m_wrnum;
    protected string m_equipID;
    protected NameValueCollection nvcEQUIP;
    WorkRequest objWorkRequest;
    NameValueCollection nvcwr;
    protected RadGrid grdlabour;
    protected RadGrid grdmaterial;
    protected RadGrid grdservice;
    protected RadGrid grdtasks;
    protected RadGrid grdtools;
    protected DateTime mydate;
    protected bool isquery = false;
    protected int statuscode = 1;
    protected string defstatus = "NEWREQ";
    protected NameValueCollection m_msg = new NameValueCollection();
    protected NameValueCollection m_rights;
    protected int m_allowedit = 0;
    protected RadCalendar sharedcalendar;
    protected DataTable decimalplacedatable;
    protected AzzierData ad=new AzzierData();

    protected void Page_Init(object sender, EventArgs e)
    {
        RetrieveMessage();
        UserRights.CheckAccess("workrequest");
        UserRights r = new UserRights(Session["Login"].ToString(), "UserRights", "counter");
        m_rights = r.GetRights(Session["Login"].ToString(), "workrequest");
        m_allowedit = Convert.ToInt16(m_rights["urEdit"].ToString());

        Session.LCID = Convert.ToInt32(Session["LCID"]);
        if (Request.QueryString["mode"] != null)
            querymode = Request.QueryString["mode"].ToString();
        else
            querymode = "query";
        if (querymode == "")
          querymode = "query";

        if (Request.QueryString["wrnum"] != null)
            m_wrnum = Request.QueryString["wrnum"].ToString();
        else
            m_wrnum = "";

        if (querymode == "query")
            isquery = true;

        if (querymode == "edit")
        {
            objWorkRequest = new WorkRequest(Session["Login"].ToString(), "WorkRequest", "WrNum", m_wrnum);  // use the view name
            nvcwr = objWorkRequest.ModuleData;
            if (nvcwr["wrnum"].ToString() == "")
            {
              querymode = "query";
              isquery = true;
              m_wrnum = "";
            }
            statuscode = Convert.ToInt16(nvcwr["StatusCode"]);
            UserWorkList list = new UserWorkList();
            list.AddToRecentList(Session["Login"].ToString(), "WorkRequest", m_wrnum);
            Division d = new Division();
            if (!d.Editable("'" + nvcwr["Division"].ToString() + "'"))
              m_allowedit = 0;
        }
        else if (querymode == "duplicate")
        {
            objWorkRequest = new WorkRequest(Session["Login"].ToString(), "WorkRequest", "WrNum", m_wrnum);  // use the view name
            nvcwr = objWorkRequest.ModuleData;
            if (nvcwr["wrnum"].ToString() == "")
            {
                querymode = "query";
                isquery = true;
                m_wrnum = "";
            }
            //statuscode = Convert.ToInt16(nvcWO["StatusCode"]);
            UserWorkList list = new UserWorkList();
            list.AddToRecentList(Session["Login"].ToString(), "WorkRequest", m_wrnum);
            Division d = new Division();
            if (!d.Editable("'" + nvcwr["Division"].ToString() + "'"))
                m_allowedit = 0;

            defstatus = "NEWREQ";
            statuscode = 1;
          
            if (Application["AutoApproveWR"] != null)
            {
              if (Application["AutoApproveWR"].ToString().ToLower() == "yes")
              {
                defstatus = "APPR";
                statuscode = 200;
              }

            }
          
            NameValueCollection tempnvc = new NameValueCollection();
            tempnvc.Add("tfield", "wrstatus");
            tempnvc.Add("tcode", defstatus);
            ModuleoObject o = new ModuleoObject(Session["Login"].ToString(), "Codes", "Counter");
            string[] counterlist = o.Query(tempnvc);
            if (counterlist.Length > 0)
            {
              o = new ModuleoObject(Session["Login"].ToString(), "Codes", "Counter", counterlist[0]);
              statuscode = Convert.ToInt16(o.ModuleData["Tcode2"]);

            }
            nvcwr["statuscode"] = statuscode.ToString();
            nvcwr["status"] = defstatus;
        }
        else // new
        {
            objWorkRequest = new WorkRequest(Session["Login"].ToString(), "WorkRequest", "WrNum");
            nvcwr = objWorkRequest.ModuleData;
          
            if (Application["AutoApproveWr"] != null)
            {
              if (Application["AutoApproveWr"].ToString().ToLower() == "yes")
              {
                defstatus = "APPR";
              }
            }
          
            NameValueCollection tempnvc = new NameValueCollection();
            tempnvc.Add("tfield", "wrstatus");
            tempnvc.Add("tcode", defstatus);
            ModuleoObject o = new ModuleoObject(Session["Login"].ToString(), "Codes", "Counter");
            string[] counterlist = o.Query(tempnvc);
            
            if (counterlist.Length  > 0)
            {
              o = new ModuleoObject(Session["Login"].ToString(), "Codes", "Counter", counterlist[0]);
              statuscode = Convert.ToInt16(o.ModuleData["Tcode2"]);
              nvcwr["statuscode"] = o.ModuleData["TCode2"];
            }
            
            nvcwr["status"] = defstatus;
            if (Application["DefWOPriority"] != null)
            {
              nvcwr["Priority"] = Application["DefWOPriority"].ToString();
            }
            if (Application["DefWOType"] != null)
            {
              nvcwr["WOType"] = Application["DefWOType"].ToString();
            }
            nvcwr["Requester"] = Session["Login"].ToString();
            if (Application["LocOnRequest"].ToString() != "No")
            {
                nvcwr["Location"] = Session["DefLocation"].ToString();
            }
            else
                nvcwr["Location"] = "";
            
        }
        hidMode.Value = querymode;
        connstring = Application["ConnString"].ToString();
        SetGrdDataSource(connstring);
        sharedcalendar = new RadCalendar();
        sharedcalendar.SkinID = "Outlook";
        sharedcalendar.ID = "SharedCalendar";
        MainControlsPanel.Controls.Add(sharedcalendar);

        InitScreen();       
    }

    private void InitScreen()
    {

        //if (nvcWO["StatusCode"].ToString() != "" || nvcWO["StatusCode"].ToString().Length > 0)
        //    statuscode = Convert.ToInt32(nvcWO["StatusCode"].ToString());
                
        screen = new AzzierScreen("workrequest/wrmain.aspx", "MainForm", MainControlsPanel.Controls, querymode, statuscode);
        screen.LCID = Session.LCID;
        //screen.Top = 20;
        grdtasks = new RadGrid();
        grdservice = new RadGrid();
        grdlabour = new RadGrid();
        grdmaterial = new RadGrid();
        grdtools = new RadGrid();
        
        decimalplacedatable = AzzierData.decimalDatadatable;
        
        if (querymode == "edit")
        {
          // service

          InitAllGrid(grdservice, "service", "ServiceSqlDataSource", 1);

          grdservice.ItemDataBound += new GridItemEventHandler(grdservice_ItemDataBound);
          grdservice.DeleteCommand += new GridCommandEventHandler(grdservice_DeleteCommand);
          grdservice.UpdateCommand += new GridCommandEventHandler(grdservice_UpdateCommand);
          grdservice.InsertCommand += new GridCommandEventHandler(grdservice_InsertCommand);
          grdservice.ItemCommand += new GridCommandEventHandler(grdservice_ItemCommand);
          grdservice.DataBound += new EventHandler(grid_DataBound);

          // Labour 
          
          InitAllGrid(grdlabour, "labour", "LabourSqlDataSource", 1);

          grdlabour.ItemDataBound += new GridItemEventHandler(grdlabour_ItemDataBound);
          grdlabour.DeleteCommand += new GridCommandEventHandler(grdlabour_DeleteCommand);
          grdlabour.UpdateCommand += new GridCommandEventHandler(grdlabour_UpdateCommand);
          grdlabour.InsertCommand += new GridCommandEventHandler(grdlabour_InsertCommand);
          grdlabour.ItemCommand += new GridCommandEventHandler(grdlabour_ItemCommand);
          grdlabour.DataBound += new EventHandler(grid_DataBound);


          // Material
          
          InitAllGrid(grdmaterial, "material", "MaterialSqlDataSource", 1);

          grdmaterial.ItemDataBound += new GridItemEventHandler(grdmaterial_ItemDataBound);
          grdmaterial.DeleteCommand += new GridCommandEventHandler(grdmaterial_DeleteCommand);
          grdmaterial.UpdateCommand += new GridCommandEventHandler(grdmaterial_UpdateCommand);
          grdmaterial.InsertCommand += new GridCommandEventHandler(grdmaterial_InsertCommand);
          grdmaterial.ItemCommand += new GridCommandEventHandler(grdmaterial_ItemCommand);
          grdmaterial.DataBound += new EventHandler(grid_DataBound);


          // Tasks 
          
          InitAllGrid(grdtasks, "tasks", "TasksSqlDataSource", 1);

          grdtasks.ItemDataBound += new GridItemEventHandler(grdtasks_ItemDataBound);
          grdtasks.DeleteCommand += new GridCommandEventHandler(grdtasks_DeleteCommand);
          grdtasks.UpdateCommand += new GridCommandEventHandler(grdtasks_UpdateCommand);
          grdtasks.InsertCommand += new GridCommandEventHandler(grdtasks_InsertCommand);
          grdtasks.ItemCommand += new GridCommandEventHandler(grdtasks_ItemCommand);

          // Tools Estimate
          
          InitAllGrid(grdtools, "tools", "ToolsSqlDataSource", 1);

          grdtools.ItemDataBound += new GridItemEventHandler(grdtools_ItemDataBound);
          grdtools.DeleteCommand += new GridCommandEventHandler(grdtools_DeleteCommand);
          grdtools.UpdateCommand += new GridCommandEventHandler(grdtools_UpdateCommand);
          grdtools.InsertCommand += new GridCommandEventHandler(grdtools_InsertCommand);
          grdtools.ItemCommand += new GridCommandEventHandler(grdtools_ItemCommand);
          grdtools.DataBound += new EventHandler(grid_DataBound);


          grdlabour.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(statuscode, true, "Labour", null, "return editLabour('',1)", m_allowedit,"",true,"","",null,"",Application["AutoApproveWr"].ToString());
          grdmaterial.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(statuscode, true, "Material", null, "return editMaterial('',1)", m_allowedit,"",true,"","",null,"",Application["AutoApproveWr"].ToString());
          //grdtasks.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(statuscode, true, "Tasks", null, "return editTask('',1)", m_allowedit);
          grdtasks.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(statuscode, true, "Tasks", null, "return editTask('',1)", m_allowedit, "", true, "", "", "return addfromtasklib('WORKREQUEST','" + m_wrnum + "','1');", "Add Task Library", Application["AutoApproveWr"].ToString());
          grdservice.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(statuscode, true, "Services", null, "return editService('',1)", m_allowedit,"",true,"","",null,"",Application["AutoApproveWr"].ToString());
          grdtools.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(statuscode, true, "Tools", null, "return editTools('',1)", m_allowedit,"",true,"","",null,"",Application["AutoApproveWr"].ToString());


          //Add three hidden fileds (HidSysPrice, HidLastPrice, HidQuotedPrice) in grdwomatlact, grdwomatlest  
          GridBoundColumn newcol = new GridBoundColumn();
          newcol.DataField = "markuppercent";
          newcol.UniqueName = "markuppercent";
          newcol.Display = false;
          grdmaterial.MasterTableView.Columns.Add(newcol);

          newcol = new GridBoundColumn();
          newcol.DataField = "HidLastPrice";
          newcol.UniqueName = "HidLastPrice";
          newcol.Display = false;
          grdmaterial.MasterTableView.Columns.Add(newcol);

          newcol = new GridBoundColumn();
          newcol.DataField = "HidAvgPrice";
          newcol.UniqueName = "HidAvgPrice";
          newcol.Display = false;
          grdmaterial.MasterTableView.Columns.Add(newcol);

          newcol = new GridBoundColumn();
          newcol.DataField = "HidQuotedPrice";
          newcol.UniqueName = "HidQuotedPrice";
          newcol.Display = false;
          grdmaterial.MasterTableView.Columns.Add(newcol);

          newcol = new GridBoundColumn();
          newcol.DataField = "HidFixPrice";
          newcol.UniqueName = "HidFixPrice";
          newcol.Display = false;
          grdmaterial.MasterTableView.Columns.Add(newcol);


          newcol = new GridBoundColumn();
          newcol.DataField = "IssuePrice";
          newcol.UniqueName = "HidIssuePrice";
          newcol.Display = false;
          grdmaterial.MasterTableView.Columns.Add(newcol);

          newcol = new GridBoundColumn();
          newcol.DataField = "HidLastPrice";
          newcol.UniqueName = "HidLastPrice";
          newcol.Display = false;
          grdservice.MasterTableView.Columns.Add(newcol);

          newcol = new GridBoundColumn();
          newcol.DataField = "HidQuotedPrice";
          newcol.UniqueName = "HidQuotedPrice";
          newcol.Display = false;
          grdservice.MasterTableView.Columns.Add(newcol);
          

          MainControlsPanel.Controls.Add(grdmaterial);
          MainControlsPanel.Controls.Add(grdlabour);
          MainControlsPanel.Controls.Add(grdservice);
          MainControlsPanel.Controls.Add(grdtasks);
          MainControlsPanel.Controls.Add(grdtools);
        }

        screen.LoadScreen();

         //GridBoundColumn colNumber =
         //       grdwolabouract.Columns.FindByUniqueName("hours") as GridBoundColumn;
         //colNumber.DataFormatString = "{0:#.#####}";

        if (!isquery)
            screen.SetValidationControls();
    }



    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (querymode == "edit")
            {
                screen.PopulateScreen("Workorder", nvcwr);
                m_equipID = nvcwr["equipment"];
                if (m_equipID != "")
                {
                    Equipment objEquipment = new Equipment(Session["Login"].ToString(), "equipment", "equipment", m_equipID);
                    nvcEQUIP = objEquipment.ModuleData;
                    if (nvcEQUIP != null)
                    {
                        HidMobile.Value = nvcEQUIP["mobileequipment"].ToString();
                    }
                }
                if (Convert.ToInt16(nvcwr["StatusCode"]) >= 100)
                  screen.SetTextControlReadonly("procnum", MainControlsPanel);
            }
            if (querymode == "duplicate") //???
            {
                //screen.PopulateScreen("Workorder", nvcWO);

                
            }
            if (querymode != "query")
            {
              TextBox tbx = MainControlsPanel.FindControl("txtlocation") as TextBox;
              tbx.Attributes.Add("onFocus", "locFocus()");

              HyperLink h = MainControlsPanel.FindControl("lkulocation") as HyperLink;
              if (h != null)
              {
                h.NavigateUrl = "javascript:loclookup()";
              }
            }
            //grdwolabouract.Rebind();

            string tax1 = "";
            string tax2 = "";
            string seltype = "E";

            HidLabTax1.Value = tax1;
            HidLabTax2.Value = tax2;
            HidLabSelType.Value = seltype;
            HidLabCost.Value = Application["LabCost"].ToString();

            //HidSysMatPrice.Value = Application["IssuePrice"].ToString();
            HidMatCost.Value = Application["MatCost"].ToString();
            HidMatTax1.Value = Application["Tax1"].ToString();
            HidMatTax2.Value = Application["Tax2"].ToString();

            if (Application["IncludeTax1Issuing"].ToString().ToLower() == "yes")
              HidMatTax1.Value = Application["Tax1"].ToString();
            else
              HidMatTax1.Value = "0";
            if (Application["IncludeTax2Issuing"].ToString().ToLower() == "yes")
              HidMatTax2.Value = Application["Tax2"].ToString();
            else
              HidMatTax2.Value = "0";

            HidSysServicePrice.Value = Application["IssueServicePrice"].ToString();
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
            RadioButtonList r = (RadioButtonList) MainControlsPanel.FindControl("rblchargeback");
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
                if (nvcwr["chargeback"].ToString() == "1")
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
            if (Request.Form["__EVENTTARGET"] == "saveStatus")
            {
                SaveStatus();
            }
            if (Request.Form["__EVENTTARGET"] == "Query")
            {
                Query();
            }
        }

        ucHeader1.Mode = querymode;
        ucHeader1.TabName = "Main";
        ucHeader1.ModuleData = nvcwr;
        string oper = "";
        if (querymode == "query")
        {
            oper = "Query";
        }
        else if (querymode == "new")
        {
            oper = "New";
            litScript1.Text += "getlocationinfo();";
        }
        else if (querymode == "edit")
        {
            oper = "Edit";

        }
        ucHeader1.OperationLabel = oper;
        if (querymode == "edit")
        {
            TextBox rdp = MainControlsPanel.FindControl("txtopendate") as TextBox;
            if (rdp != null)
                rdp.Enabled = false;
        }
        else if (querymode == "new" || querymode == "duplicate")
        {
          if (querymode == "new")
          {
            screen.GetDefaultValue("Workorder", nvcwr, "WorkRequest", "new");
          }
          screen.PopulateScreen("WorkRequest", nvcwr);

          TextBox t = MainControlsPanel.FindControl("txtwrnum") as TextBox;
          if (t != null)
          {
            if (!Page.IsPostBack && (Request.QueryString["numbering"] == "auto" || querymode == "duplicate"))
            {
              NewNumber num = new NewNumber();
              t.Text = num.GetNextNumber("WR");
            }
            else
              t.Text = "";
          }
          t = MainControlsPanel.FindControl("txtopendate") as TextBox;
          if (t != null)
          {
              if (!Page.IsPostBack)
                  t.Text = DateTime.Today.ToShortDateString();
          }

          if (querymode == "duplicate")
          {
            t = MainControlsPanel.FindControl("txtprocnum") as TextBox;
            if (t != null)
            {
              t.CssClass = "rofield";
              t.Attributes.Add("Readonly", "readonly");
            }
            HyperLink h = MainControlsPanel.FindControl("lkuprocnum") as HyperLink;
            if (h != null)
              h.Visible = false;

          }

        }

        if (Application["LocOnRequest"].ToString() == "Force")
        {
            if (querymode != "query")
            {
                screen.SetTextControlReadonly("location", MainControlsPanel);
                screen.SetTextControlReadonly("locationdesc", MainControlsPanel);
            }
        }
        
        if (Application["ViewOther"].ToString() == "No")
        {
            screen.SetTextControlReadonly("requester", MainControlsPanel);            
            if (querymode == "query")
            {
                TextBox t = MainControlsPanel.FindControl("txtrequester") as TextBox;
                t.Text = Session["Login"].ToString();
            }
        }
        hidTargetStatusCode.Value = statuscode.ToString();
        hidprocnum.Value = nvcwr["ProcNum"];
    }

    private void Query()
    {
        NameValueCollection nvc;
        nvc = screen.CollectFormValues("workrequest", true);  // use the view name
        if (nvc["chargeback"] == ",")//???
            nvc.Remove("chargeback");

        WorkRequest objwr = new WorkRequest(Session["Login"].ToString(), "workrequest", "Wrnum");   // use the view name
        string wherestring = "", linqwherestring = "";
        //string[] wonums = objWorkorder.LinqQuery(nvc, ref wherestring,ref linqwherestring);
        objwr.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);

        string jscript = "";

        jscript += "var wherestring = '" + Server.UrlEncode(linqwherestring) + "'; var obj=parent.controlpanel.document.location.href='wrpanel.aspx?showlist=queryresult&wherestring='+wherestring;";
        litScript1.Text = jscript;
    }


    protected void grdlabour_DeleteCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        //Get the GridDataItem of the RadGrid
        GridDataItem item = (GridDataItem)e.Item;
        //Get the primary key value using the DataKeyValue.       
        string counter = item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString();

        Labour objResource = new Labour(Session["Login"].ToString(), "WOLabour", "Counter", counter);
        bool success = objResource.Delete();
        if (!success)
        {
            grdlabour.Controls.Add(new LiteralControl(m_msg["T3"] + objResource.ErrorMessage));
            e.Canceled = true;
        }
        SetCurrentWorkOrderNVC();
    }


    protected void grdlabour_UpdateCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        //Get the GridEditableItem of the RadGrid
        NameValueCollection nvcFT = screen.GetGridFieldTypes("labour", "WOLabour");
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
                    else if (dbtype == "system.datetime")
                    {
                        string sdate = (editedItem[field].Controls[0] as RadDatePicker).SelectedDate.ToString();
                        nvc.Add(field, sdate);
                    }
                }
            }
        }

        Labour objResource = new Labour(Session["Login"].ToString(), "WOLabour", "Counter", counter);
        bool success = objResource.Update(nvc);
        if (!success)
        {
            grdlabour.Controls.Add(new LiteralControl(m_msg["T5"] + objResource.ErrorMessage));
            e.Canceled = true;
        }
        SetCurrentWorkOrderNVC();
    }

    protected void grdlabour_InsertCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        //Get the GridEditFormInsertItem of the RadGrid


        DateFormat objDateFormat = new DateFormat(Session.LCID);
        NameValueCollection nvcFT = screen.GetGridFieldTypes("labour", "WOLabour");
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
                    else if (dbtype == "system.datetime")
                    {
                        string sdate = (insertedItem[field].Controls[0] as RadDatePicker).SelectedDate.ToString();
                        nvc.Add(field, sdate);
                    }
                }
            }
        }
        
        nvc.Add("wonum", m_wrnum);
        nvc.Add("estimate", "1");
        nvc.Add("ordertype", "workrequest");

        Labour objResource = new Labour(Session["Login"].ToString(), "WOLabour", "Counter");
        bool success = objResource.Create(nvc);

        if (!success)
        {
            grdlabour.Controls.Add(new LiteralControl(m_msg["T7"] + objResource.ErrorMessage));
            e.Canceled = true;
        }
        SetCurrentWorkOrderNVC();
    }

    protected void grdmaterial_DeleteCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        //Get the GridDataItem of the RadGrid
        GridDataItem item = (GridDataItem)e.Item;
        //Get the primary key value using the DataKeyValue.
        string counter = item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString();

        Material objResource = new Material(Session["Login"].ToString(), "WOMaterial", "Counter", counter);
        bool success = objResource.Delete();
        if (!success)
        {
            grdmaterial.Controls.Add(new LiteralControl(m_msg["T8"] + objResource.ErrorMessage));
            e.Canceled = true;
        }
        SetCurrentWorkOrderNVC();
    }

    protected void grdmaterial_UpdateCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        //Get the GridEditableItem of the RadGrid
        GridEditableItem editedItem = e.Item as GridEditableItem;
        //Get the primary key value using the DataKeyValue.
        string counter = editedItem.OwnerTableView.DataKeyValues[editedItem.ItemIndex]["Counter"].ToString();

        DateFormat objDateFormat = new DateFormat(Session.LCID);
        NameValueCollection nvcFT = screen.GetGridFieldTypes("material", "WOMaterial");
        string[] fields = nvcFT.AllKeys;
        NameValueCollection nvc = new NameValueCollection();
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
                    //string result = "";
                    //bool a = objDateFormat.ValidateInputDate(sdate, out result);    // convert date from screen format to db date format
                    //sdate = result;
                    nvc.Add(field, sdate);
                }
            }
        }

        nvc.Add("estimate", "1");
        nvc.Add("wonum", m_wrnum);
        nvc.Add("ordertype", "workrequest");

        Material objResource = new Material(Session["Login"].ToString(), "WOMaterial", "Counter", counter);
        bool success = objResource.Update(nvc);
        if (!success)
        {
            grdmaterial.Controls.Add(new LiteralControl(m_msg["T10"] + objResource.ErrorMessage));
            e.Canceled = true;
        }
        SetCurrentWorkOrderNVC();
    }

    protected void grdmaterial_InsertCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        //Get the GridEditFormInsertItem of the RadGrid
        GridDataInsertItem insertedItem = e.Item as GridDataInsertItem;

        DateFormat objDateFormat = new DateFormat(Session.LCID);
        NameValueCollection nvcFT = screen.GetGridFieldTypes("material", "WOMaterial");
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
                    //string result = "";
                    //bool a = objDateFormat.ValidateInputDate(sdate, out result);    // convert date from screen format to db date format
                    //sdate = result;
                    nvc.Add(field, sdate);
                }
            }
        }
        nvc.Add("wonum", m_wrnum);
        nvc.Add("estimate", "1");
        nvc.Add("ordertype", "workrequest");

        Material objResource = new Material(Session["Login"].ToString(), "WOMaterial", "Counter");
        bool success = objResource.Create(nvc);

        if (!success)
        {
            grdmaterial.Controls.Add(new LiteralControl(m_msg["T13"] + objResource.ErrorMessage));
            e.Canceled = true;
        }
        SetCurrentWorkOrderNVC();
    }

    protected void gridFieldsDecimalPlaces(RadGrid radGrid,GridDataItem dataItem, string tablename)
    {
        string fieldnames, decimalplaceStr = "";
        List<string> fieldname = new List<string>();
        foreach (GridColumn gridColumn in radGrid.MasterTableView.RenderColumns)
        { fieldname.Add(gridColumn.UniqueName.ToString()); }

        if (decimalplacedatable.Rows.Count > 1)
        {
            var results = from rows in decimalplacedatable.AsEnumerable()
                          where rows.Field<string>("WWTableName").ToLower() == tablename && fieldname.Contains(rows.Field<string>("WWFieldName"))
                          select new
                          {
                              FieldName = rows.Field<string>("WWFieldName"),
                              DecimalPlaces = rows.Field<decimal>("DecimalPlaces").ToString()
                          };
 
            foreach (var columns in results)
            {
                fieldnames = columns.FieldName;
                decimalplaceStr = columns.DecimalPlaces;
                int decimalplaces = Convert.ToInt32(decimalplaceStr);
                decimal fldUnitRev = Decimal.Parse(dataItem[fieldnames].Text);
                //decimalItem[teststr1].Text = Math.Round(fldUnitRev * 1.0000m, decimalplaces, MidpointRounding.AwayFromZero).ToString();// fldUnitRev.ToString("C2");
                dataItem[fieldnames].Text = Math.Round(fldUnitRev, decimalplaces).ToString();
                dataItem[fieldnames].HorizontalAlign = HorizontalAlign.Right;
            }
        }

        //if (e.Item is GridEditableItem && e.Item.IsInEditMode)
        //{
        //    GridEditableItem editedItem = e.Item as GridEditableItem;
        //    string istr = "3";
        //    //TextBox textboxID = editedItem["ID"].Controls[0] as TextBox;
        //    TextBox textboxID = editedItem["ID"].Controls[0] as TextBox;
        //    //String strTemp = tex .Text;
        //    //strTemp = (editedItem["ID"].Controls[0] as TextBox).Text;
        //    double fldUnitRev = Double.Parse(textboxID.Text);
        //    textboxID.Text = fldUnitRev.ToString("F" + istr);
        //    textboxID.Style["Text-Align"] = "right";// HorizontalAlign.Right;// "right";

        //    //double fldUnitReva = Double.Parse((editItem["ID"].Controls[0] as Literal).Text);
        //    //editItem["ID"].Text = fldUnitReva.ToString("C2");
        //}

    }

    protected void grdlabour_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridCommandItem)
        {
            GridCommandItem commandItem = (GridCommandItem)e.Item;

            ImageButton t_addInPlaceButton = (ImageButton)commandItem.FindControl("addInPlaceButton");
            if (t_addInPlaceButton != null)
                t_addInPlaceButton.ValidationGroup = "labour";
        }

        if (e.Item is GridEditableItem && e.Item.IsInEditMode)
        {
            GridEditableItem editedItem = (GridEditableItem)e.Item;
            ImageButton t_imagebtn;
            if (e.Item.OwnerTableView.IsItemInserted)
            {
                t_imagebtn = ((ImageButton)editedItem.FindControl("PerformInsertButton"));
                if (t_imagebtn != null)
                    t_imagebtn.ValidationGroup = "labour";
            }
            else
            {
                t_imagebtn = ((ImageButton)editedItem.FindControl("UpdateButton"));
                if (t_imagebtn != null)
                    t_imagebtn.ValidationGroup = "labour";
            }

            ConditionalChecking.AddLabourCheck(editedItem,"labour");
            if (e.Item.OwnerTableView.IsItemInserted)
            {
                screen.InitResourceRecord("workrequest", null, e, "mainpanel", "labour", "1", m_wrnum);
            }
        }

        if (e.Item is GridDataItem && !e.Item.IsInEditMode)
        {
            GridDataItem item = (GridDataItem)e.Item;

            //if (statuscode < 100 && statuscode > 0 && m_allowedit == 1)
            //{
                // ImageButton btn = (ImageButton)item["EstLabEditCommand"].Controls[0];
                ImageButton btn = (ImageButton)item["EditCommand"].Controls[0];
                btn.ImageUrl = "~/images/Edit.gif";
                btn.OnClientClick = "return editLabour(" + item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString() + ",1)";
                //btn.OnClientClick = "return editLabour(1,1)";
            //}


        }

        screen.GridItemDataBound(e, "workrequest/wrmain.aspx", "MainForm", "labour");
    }

    protected void grdlabour_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        if (e.CommandName.ToString() == "InPlaceInitInsert")
        {
            e.Canceled = true;
            System.Collections.Specialized.ListDictionary newValues = new System.Collections.Specialized.ListDictionary();
            newValues["WoNum"] = m_wrnum;
            newValues["TransDate"] = DateTime.Today;
            newValues["Estimate"] = 1;
            newValues["Actual"] = 0;
            newValues["ChargeBack"] = nvcwr["ChargeBack"];
            grdlabour.MasterTableView.EditMode = GridEditMode.InPlace;
            e.Item.OwnerTableView.InsertItem(newValues);
        }
        else if (e.CommandName == "RowClick")
        {
            GridDataItem item = (GridDataItem)e.Item;
            item.Edit = true;
            grdlabour.MasterTableView.EditMode = GridEditMode.InPlace;
            grdlabour.Rebind();
        }
    }


    protected void grdmaterial_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        if (e.CommandName.ToString() == "InPlaceInitInsert")
        {
            // cancel the default operation
            e.Canceled = true;
            //Prepare an IDictionary with the predefined values
            System.Collections.Specialized.ListDictionary newValues = new System.Collections.Specialized.ListDictionary();
            newValues["WoNum"] = m_wrnum;
            newValues["TransDate"] = DateTime.Today;
            newValues["Estimate"] = 1;
            newValues["ChargeBack"] = nvcwr["ChargeBack"];
            //Insert the item and rebind
            e.Item.OwnerTableView.InsertItem(newValues);
        }
        else if (e.CommandName == "RowClick")
        {
            GridDataItem item = (GridDataItem)e.Item;
            item.Edit = true;
            grdmaterial.MasterTableView.EditMode = GridEditMode.InPlace;
            grdmaterial.Rebind();
        }
    }

    
    protected void grdmaterial_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem && !e.Item.IsInEditMode)
        {
            GridDataItem item = (GridDataItem)e.Item;

            //if (statuscode < 100 && statuscode > 0 && m_allowedit == 1)
            //{
                ImageButton btn = (ImageButton)item["EditCommand"].Controls[0];
                btn.ImageUrl = "~/images/Edit.gif";
                btn.OnClientClick = "return editMaterial(" + item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString() + ",1)";
            //}
        }


        if (e.Item is GridCommandItem)
        {
            GridCommandItem commandItem = (GridCommandItem)e.Item;

            ImageButton t_addInPlaceButton = (ImageButton)commandItem.FindControl("addInPlaceButton");
            if (t_addInPlaceButton != null)
                t_addInPlaceButton.ValidationGroup = "material";
        }


        screen.GridItemDataBound(e, "workrequest/wrmain.aspx", "MainForm", "material");

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
              t_imagebtn.ValidationGroup = "material";

            //itemnumtext.Attributes["fieldlist"] = assemble.ReplaceControlID("itemnum^_RT_itemnum^13,itemdesc^_RT_description^11,issueunit^_RT_unit,avgprice^_RT_HidAvgPrice^11,lastprice^_RT_HidLastPrice^11,quotedprice^_RT_HidQuotedPrice^11,fixprice^_RT_HidFixPrice,issueprice^_RT_HidIssuePrice");

          }
          else
          {
            t_imagebtn = ((ImageButton)editedItem.FindControl("UpdateButton"));
            if (t_imagebtn != null)
              t_imagebtn.ValidationGroup = "material";
            GridDataItem data = editedItem.DataItem as GridDataItem;
            string store = ((DataRowView)e.Item.DataItem)["Store"].ToString();
            if (store != "")
              //itemnumtext.Attributes["fieldlist"] = assemble.ReplaceControlID("itemnum^_RT_itemnum^11,itemdesc^_RT_description^11,issueunit^_RT_unit,avgprice^_RT_HidAvgPrice^11,lastprice^_RT_HidLastPrice^11,quotedprice^_RT_HidQuotedPrice^11,fixprice^_RT_HidFixPrice,issueprice^_RT_HidIssuePrice");
              itemnumtext.Attributes["FieldList"] = itemnumtext.Attributes["FieldList"].Replace("_ItemNum^13", "_ItemNum^11");
            //if (store == "")
            //  itemnumtext.Attributes["fieldlist"] = assemble.ReplaceControlID("itemnum^_RT_itemnum^13,itemdesc^_RT_description^11,issueunit^_RT_unit,avgprice^_RT_HidAvgPrice^11,lastprice^_RT_HidLastPrice^11,quotedprice^_RT_HidQuotedPrice^11,fixprice^_RT_HidFixPrice,issueprice^_RT_HidIssuePrice");
            //else
            //  itemnumtext.Attributes["fieldlist"] = assemble.ReplaceControlID("itemnum^_RT_itemnum^11,itemdesc^_RT_description^11,issueunit^_RT_unit,avgprice^_RT_HidAvgPrice^11,lastprice^_RT_HidLastPrice^11,quotedprice^_RT_HidQuotedPrice^11,fixprice^_RT_HidFixPrice,issueprice^_RT_HidIssuePrice");
          }
          if (e.Item.OwnerTableView.IsItemInserted)
          {
              screen.InitResourceRecord("workrequest", null, e, "mainpanel", "material", "1", m_wrnum);
          }
        }  
      /*
        if (e.Item is GridEditableItem && e.Item.IsInEditMode)
        {
            GridEditableItem editedItem = (GridEditableItem)e.Item;
            ImageButton t_imagebtn;
            if (e.Item.OwnerTableView.IsItemInserted)
            {
                t_imagebtn = ((ImageButton)editedItem.FindControl("PerformInsertButton"));
                if (t_imagebtn != null)
                    t_imagebtn.ValidationGroup = "material";
            }
            else
            {
                t_imagebtn = ((ImageButton)editedItem.FindControl("UpdateButton"));
                if (t_imagebtn != null)
                    t_imagebtn.ValidationGroup = "material";
            }
        }

        screen.GridItemDataBound(e, "workrequest/wrmain.aspx", "MainForm", "material");
       * */
    }

    private Boolean SaveWR() 
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        TextBox tbx = null;
        string dirtylog = "0";
        if (CntlPanel != null)
        {
            nvc = screen.CollectFormValues("workrequest", false);  // use the table name
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

        WorkRequest objwr;
        bool success = false;
        if (querymode == "new")
        {
            objwr = new WorkRequest(Session["Login"].ToString(), "WorkRequest", "WRNum");   // use the table name
            success = objwr.Create(nvc);
            if (success)
            {
              if (nvc["procnum"] != "")
              {
                Labour l = new Labour(Session["Login"].ToString(), "WOLabour", "Counter");
                l.CopyLabour("procedure", nvc["procnum"], "workrequest", nvc["wrnum"]);
                Service s = new Service(Session["Login"].ToString(), "WOService", "Counter");
                s.CopyService("procedure", nvc["procnum"], "workrequest", nvc["wrnum"]);
                Material m = new Material(Session["Login"].ToString(), "WOMaterial", "Counter");
                m.CopyMaterial("procedure", nvc["procnum"], "workrequest", nvc["wrnum"]);
                Tool t = new Tool(Session["Login"].ToString(), "WOTools", "Counter");
                t.CopyTools("procedure", nvc["procnum"], "workrequest", nvc["wrnum"]);
                Task task = new Task(Session["Login"].ToString(), "WOTasks", "Counter");
                task.CopyTasks("procedure", nvc["procnum"], "workrequest", nvc["wrnum"]);
              }
            }
        }
        else if (querymode == "edit")
        {
            objwr = new WorkRequest(Session["Login"].ToString(), "WorkRequest", "WRNum", m_wrnum);   // use the table name
            success = objwr.Update(nvc);
            if (success)
            {
              string oldproc = hidprocnum.Value;
              if ((nvc["procnum"].ToString() != "") && (nvc["procnum"].ToString() != oldproc))
              {
                Labour l = new Labour(Session["Login"].ToString(), "WOLabour", "Counter");
                l.DeleteAllLabour("workrequest", nvc["wonum"], 1);
                l.CopyLabour("procedure", nvc["procnum"], "workrequest", nvc["wonum"]);
                Service s = new Service(Session["Login"].ToString(), "WOService", "Counter");
                s.DeleteAllService("workrequest", nvc["wonum"], 1);
                s.CopyService("procedure", nvc["procnum"], "workrequest", nvc["wonum"]);
                Material m = new Material(Session["Login"].ToString(), "WOMaterial", "Counter");
                m.DeleteAllMaterial("workrequest", nvc["wonum"], 1);
                m.CopyMaterial("procedure", nvc["procnum"], "workrequest", nvc["wonum"]);
                Tool t = new Tool(Session["Login"].ToString(), "WOTools", "Counter");
                t.DeleteAllTools("workrequest", nvc["wonum"], 1);
                t.CopyTools("procedure", nvc["procnum"], "workrequest", nvc["wonum"]);
                Task task = new Task(Session["Login"].ToString(), "WOTasks", "Counter");
                task.DeleteAllTask("workrequest", nvc["wonum"], 1);
                task.CopyTasks("procedure", nvc["procnum"], "workrequest", nvc["wonum"]);
                hidprocnum.Value = nvc["procnum"];
              }
            }
        }
        else
            objwr = new WorkRequest(Session["Login"].ToString(), "Workrequest", "WRNum");
        return success;

    }
    

    private void SaveStatus()
    {
        SaveWR();

        NameValueCollection nvc = new NameValueCollection();
        //string wonum = hidWoNum.Value;
        //nvc.Add("wonum", m_wonum);
        nvc.Add("statuscode", hidTargetStatusCode.Value);
        nvc.Add("status", hidTargetStatus.Value);
        nvc.Add("Remark", hidStatusComments.Value);
        if (Convert.ToInt32(hidTargetStatusCode.Value) < 300)
            nvc.Add("compdate", "");

        WorkRequest objwr = new WorkRequest(Session["Login"].ToString(), "workrequest", "wrnum", m_wrnum);
        int statuscode = Convert.ToInt16(hidTargetStatusCode.Value);
        if (statuscode >= 100 && statuscode < 200)
        {
            ApproveClass appr = new ApproveClass("WORKREQUEST", m_wrnum, statuscode);
            if (appr.IsFinalApprove(hidTargetStatus.Value) == true)
            {
                nvc["statuscode"] = "200";
                nvc["status"] = "APPR";
            }
        }

        if (objwr.UpdateStatus(nvc))
        {
            //hidUpdateSuccess.Value = "success";
            litScript1.Text = "document.location.href='Wrmain.aspx?mode=edit&wrnum=" + m_wrnum + "';";
        }
        else
          litScript1.Text = "alert('" + objwr.ErrorMessage + "');";
    }

    protected void grdservice_ItemDataBound(object sender, GridItemEventArgs e)
    {
        

        if (e.Item is GridCommandItem)
        {
            GridCommandItem commandItem = (GridCommandItem)e.Item;

            ImageButton t_addInPlaceButton = (ImageButton)commandItem.FindControl("addInPlaceButton");
            if (t_addInPlaceButton != null)
                t_addInPlaceButton.ValidationGroup = "service";
        }

        if (e.Item is GridDataItem && !e.Item.IsInEditMode)
        {
            GridDataItem item = (GridDataItem)e.Item;

            //if (statuscode < 100 && statuscode > 0 && m_allowedit == 1)
            //{
                ImageButton btn = (ImageButton)item["EditCommand"].Controls[0];
                btn.ImageUrl = "~/images/Edit.gif";
                btn.OnClientClick = "return editService(" + item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString() + ",1)";
            //}
        }

        if (e.Item is GridEditableItem && e.Item.IsInEditMode)
        {
            GridEditableItem editedItem = (GridEditableItem)e.Item;
            ImageButton t_imagebtn;
            if (e.Item.OwnerTableView.IsItemInserted)
            {
                t_imagebtn = ((ImageButton)editedItem.FindControl("PerformInsertButton"));
                if (t_imagebtn != null)
                    t_imagebtn.ValidationGroup = "service";
            }
            else
            {
                t_imagebtn = ((ImageButton)editedItem.FindControl("UpdateButton"));
                if (t_imagebtn != null)
                    t_imagebtn.ValidationGroup = "service";
            }
            if (e.Item.OwnerTableView.IsItemInserted)
            {
                screen.InitResourceRecord("workrequest", null, e, "mainpanel", "service", "1", m_wrnum);
            }
        }

        screen.GridItemDataBound(e, "workrequest/wrmain.aspx", "MainForm", "service");

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

    protected void grdservice_DeleteCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        GridDataItem item = (GridDataItem)e.Item;
        string counter = item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString();

        Service objResource = new Service(Session["Login"].ToString(), "WOService", "Counter", counter);
        bool success = objResource.Delete();
        if (!success)
        {
            grdservice.Controls.Add(new LiteralControl(m_msg["T16"] + objResource.ErrorMessage));
            e.Canceled = true;
        }
        SetCurrentWorkOrderNVC();
    }

    protected void grdservice_UpdateCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        //Get the GridEditableItem of the RadGrid
        NameValueCollection nvcFT = screen.GetGridFieldTypes("service", "WOService");
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

        Service objResource = new Service(Session["Login"].ToString(), "WOService", "Counter", counter);
        bool success = objResource.Update(nvc);
        if (!success)
        {
            grdservice.Controls.Add(new LiteralControl(m_msg["T17"] + objResource.ErrorMessage));
            e.Canceled = true;
        }
        SetCurrentWorkOrderNVC();
    }

    protected void grdservice_InsertCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        DateFormat objDateFormat = new DateFormat(Session.LCID);
        NameValueCollection nvcFT = screen.GetGridFieldTypes("service", "WOService");
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
        
        nvc.Add("wonum", m_wrnum);
        nvc.Add("estimate", "1");
        nvc.Add("ordertype", "workrequest");
        Service objResource = new Service(Session["Login"].ToString(), "WOService", "Counter");
        bool success = objResource.Create(nvc);

        if (!success)
        {
            grdservice.Controls.Add(new LiteralControl(m_msg["T18"] + objResource.ErrorMessage));
            e.Canceled = true;
        }
        SetCurrentWorkOrderNVC();
    }

    protected void grdservice_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        #region void grdwoserviceest_ItemCommand
        if (e.CommandName.ToString() == "InPlaceInitInsert")
        {
            e.Canceled = true;
            System.Collections.Specialized.ListDictionary newValues = new System.Collections.Specialized.ListDictionary();
            newValues["WoNum"] = m_wrnum;
            newValues["TransDate"] = DateTime.Today;
            newValues["Estimate"] = 1;
            newValues["Actual"] = 0;
            newValues["ChargeBack"] = nvcwr["ChargeBack"];
            grdservice.MasterTableView.EditMode = GridEditMode.InPlace;
            e.Item.OwnerTableView.InsertItem(newValues);
        }
        else if (e.CommandName == RadGrid.EditCommandName)
        {
            grdservice.MasterTableView.EditMode = GridEditMode.PopUp;
        }
        else if (e.CommandName == "RowClick")
        {
            GridDataItem item = (GridDataItem)e.Item;
            item.Edit = true;
            grdservice.MasterTableView.EditMode = GridEditMode.InPlace;
            grdservice.Rebind();
        }
        #endregion
    }

    #region Work Order Tools Estimate

    protected void grdtools_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridCommandItem)
        {
            GridCommandItem commandItem = (GridCommandItem)e.Item;

            ImageButton t_addInPlaceButton = (ImageButton)commandItem.FindControl("addInPlaceButton");
            if (t_addInPlaceButton != null)
                t_addInPlaceButton.ValidationGroup = "tools";

            
        }

        if (e.Item is GridDataItem && !e.Item.IsInEditMode)
        {
            GridDataItem item = (GridDataItem)e.Item;

            //if (statuscode < 100 && statuscode > 0 && m_allowedit == 1)
            //{
                ImageButton btn = (ImageButton)item["EditCommand"].Controls[0];
                btn.ImageUrl = "~/images/Edit.gif";
                btn.OnClientClick = "return editTools(" + item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString() + ",1)";
            //}
        }

        if (e.Item is GridEditableItem && e.Item.IsInEditMode)
        {
            GridEditableItem editedItem = (GridEditableItem)e.Item;
            ImageButton t_imagebtn;
            if (e.Item.OwnerTableView.IsItemInserted)
            {
                t_imagebtn = ((ImageButton)editedItem.FindControl("PerformInsertButton"));
                if (t_imagebtn != null)
                    t_imagebtn.ValidationGroup = "tools";
            }
            else
            {
                t_imagebtn = ((ImageButton)editedItem.FindControl("UpdateButton"));
                if (t_imagebtn != null)
                    t_imagebtn.ValidationGroup = "tools";
            }

            ConditionalChecking.AddToolsCheck(editedItem, "tools");
            if (e.Item.OwnerTableView.IsItemInserted)
            {
                screen.InitResourceRecord("workrequest", null, e, "mainpanel", "tool", "1", m_wrnum);
            }
        }

        screen.GridItemDataBound(e, "workrequest/wrmain.aspx", "MainForm", "tools");
    }

    protected void grdtools_DeleteCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        GridDataItem item = (GridDataItem)e.Item;
        string counter = item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString();

        Tool objResource = new Tool(Session["Login"].ToString(), "WOTools", "Counter", counter);
        bool success = objResource.Delete();
        if (!success)
        {
            grdtools.Controls.Add(new LiteralControl(m_msg["T19"] + objResource.ErrorMessage));
            e.Canceled = true;
        }
        SetCurrentWorkOrderNVC();
    }

    protected void grdtools_UpdateCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        //Get the GridEditableItem of the RadGrid
        NameValueCollection nvcFT = screen.GetGridFieldTypes("tools", "WOTools");
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
                        //string result = "";
                        //bool a = objDateFormat.ValidateInputDate(sdate, out result);    // convert date from screen format to db date format
                        //sdate = result;
                        nvc.Add(field, sdate);
                    }
                }
            }
        }

        Tool objResource = new Tool(Session["Login"].ToString(), "WOTools", "Counter", counter);
        bool success = objResource.Update(nvc);
        if (!success)
        {
            grdtools.Controls.Add(new LiteralControl(m_msg["T20"] + objResource.ErrorMessage));
            e.Canceled = true;
        }
        SetCurrentWorkOrderNVC();
    }

    protected void grdtools_InsertCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        DateFormat objDateFormat = new DateFormat(Session.LCID);
        NameValueCollection nvcFT = screen.GetGridFieldTypes("tools", "WOTools");
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
                        //string result = "";
                        //bool a = objDateFormat.ValidateInputDate(sdate, out result);
                        //sdate = result;
                        nvc.Add(field, sdate);
                    }
                }
            }
        }
        
        nvc.Add("wonum", m_wrnum);
        nvc.Add("estimate", "1");
        nvc.Add("ordertype", "workrequest");
        Tool objResource = new Tool(Session["Login"].ToString(), "WOTools", "Counter");
        bool success = objResource.Create(nvc);

        if (!success)
        {
            grdtools.Controls.Add(new LiteralControl(m_msg["T21"] + objResource.ErrorMessage));
            e.Canceled = true;
        }
        SetCurrentWorkOrderNVC();
    }

    protected void grdtools_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        if (e.CommandName.ToString() == "InPlaceInitInsert")
        {
            e.Canceled = true;
            System.Collections.Specialized.ListDictionary newValues = new System.Collections.Specialized.ListDictionary();
            newValues["WoNum"] = m_wrnum;
            newValues["TransDate"] = DateTime.Today;
            newValues["Estimate"] = 1;
            newValues["Actual"] = 0;
            newValues["ChargeBack"] = nvcwr["ChargeBack"];
            grdtools.MasterTableView.EditMode = GridEditMode.InPlace;
            e.Item.OwnerTableView.InsertItem(newValues);
        }
        else if (e.CommandName == "RowClick")
        {
            GridDataItem item = (GridDataItem)e.Item;
            item.Edit = true;
            grdtools.MasterTableView.EditMode = GridEditMode.InPlace;
            grdtools.Rebind();
        }
    }

    #endregion


    protected void grdtasks_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridCommandItem)
        {
            GridCommandItem commandItem = (GridCommandItem)e.Item;

            ImageButton t_addInPlaceButton = (ImageButton)commandItem.FindControl("addInPlaceButton");
            if (t_addInPlaceButton != null)
                t_addInPlaceButton.ValidationGroup = "tasks";
            
        }
        if (e.Item is GridDataItem && !e.Item.IsInEditMode)
        {
            GridDataItem item = (GridDataItem)e.Item;

            //if (statuscode < 100 && statuscode > 0 && m_allowedit == 1)
            //{
                ImageButton btn = (ImageButton)item["EditCommand"].Controls[0];
                btn.ImageUrl = "~/images/Edit.gif";
                btn.OnClientClick = "return editTask(" + item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString() + ",1)";
            //}
        }


        if (e.Item is GridEditableItem && e.Item.IsInEditMode)
        {
            GridEditableItem editedItem = (GridEditableItem)e.Item;
            ImageButton t_imagebtn;
            if (e.Item.OwnerTableView.IsItemInserted)
            {
                t_imagebtn = ((ImageButton)editedItem.FindControl("PerformInsertButton"));
                if (t_imagebtn != null)
                    t_imagebtn.ValidationGroup = "tasks";
            }
            else
            {
                t_imagebtn = ((ImageButton)editedItem.FindControl("UpdateButton"));
                if (t_imagebtn != null)
                    t_imagebtn.ValidationGroup = "tasks";
            }
        }

        screen.GridItemDataBound(e, "workrequest/wrmain.aspx", "MainForm", "tasks");
    }

    protected void grdtasks_DeleteCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        GridDataItem item = (GridDataItem)e.Item;
        string counter = item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString();

        Task objResource = new Task(Session["Login"].ToString(), "WOTasks", "Counter", counter);
        bool success = objResource.Delete();
        if (!success)
        {
            grdtasks.Controls.Add(new LiteralControl(m_msg["T25"] + objResource.ErrorMessage));
            e.Canceled = true;
        }
    }

    protected void grdtasks_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        if (e.CommandName.ToString() == "InPlaceInitInsert")
        {
            e.Canceled = true;
            System.Collections.Specialized.ListDictionary newValues = new System.Collections.Specialized.ListDictionary();
            newValues["WoNum"] = m_wrnum;
            newValues["TransDate"] = DateTime.Today;
            newValues["Estimate"] = 1;
            newValues["Actual"] = 0;
            grdtasks.MasterTableView.EditMode = GridEditMode.InPlace;
            e.Item.OwnerTableView.InsertItem(newValues);
        }
        else if (e.CommandName == "RowClick")
        {
            GridDataItem item = (GridDataItem)e.Item;
            item.Edit = true;
            grdtasks.MasterTableView.EditMode = GridEditMode.InPlace;
            grdtasks.Rebind();
        }
    }

    protected void grdtasks_InsertCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        DateFormat objDateFormat = new DateFormat(Session.LCID);
        NameValueCollection nvcFT = screen.GetGridFieldTypes("tasks", "WOTasks");
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
                        string result = "";
                        bool a = objDateFormat.ValidateInputDate(sdate, out result);    // convert date from screen format to db date format
                        sdate = result;
                        nvc.Add(field, sdate);
                    }
                }
            }
        }

        nvc.Add("wonum", m_wrnum);
        nvc.Add("estimate", "1");
        nvc.Add("ordertype", "workrequest");
        Task objResource = new Task(Session["Login"].ToString(), "WOTasks", "Counter");
        bool success = objResource.Create(nvc);

        if (!success)
        {
            grdtasks.Controls.Add(new LiteralControl(m_msg["T26"] + objResource.ErrorMessage));
            e.Canceled = true;
        }
    }

    protected void grdtasks_UpdateCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
        //Get the GridEditableItem of the RadGrid
        NameValueCollection nvcFT = screen.GetGridFieldTypes("tasks", "WOTasks");
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
                        //string result = "";
                        //bool a = objDateFormat.ValidateInputDate(sdate, out result);    // convert date from screen format to db date format
                        //sdate = result;
                        nvc.Add(field, sdate);
                    }
                }
            }
        }

        Task objResource = new Task(Session["Login"].ToString(), "WOTasks", "Counter", counter);
        bool success = objResource.Update(nvc);
        if (!success)
        {
            grdtasks.Controls.Add(new LiteralControl(m_msg["T27"] + objResource.ErrorMessage));
            e.Canceled = true;
        }
    }

    protected void InitAllGrid(RadGrid grd, string controlid, string grddatasource, int Estflag)
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
        //grd.EnableViewState = false;

        GridEditCommandColumn EditColumn = new GridEditCommandColumn();
        EditColumn.HeaderText = "Edit";
        EditColumn.UniqueName = "EditCommand";
        EditColumn.ButtonType = GridButtonColumnType.ImageButton;

        EditColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        EditColumn.HeaderStyle.Width = 30;
        grd.MasterTableView.Columns.Add(EditColumn);

        if (Estflag == 1)
        {
            // New Request
            if (statuscode >= 0 && statuscode < 100 && m_allowedit == 1)
            {
                grd.ClientSettings.EnablePostBackOnRowClick = true;
            }

            screen.SetGridColumns(controlid, grd);

            if (statuscode >= 0 && statuscode < 100 && m_allowedit == 1)
            {
                GridButtonColumn DeleteColumn = new GridButtonColumn();
                DeleteColumn.HeaderText = "Delete";
                DeleteColumn.UniqueName = "DeleteButton";
                DeleteColumn.CommandName = "Delete";
                DeleteColumn.ButtonType = GridButtonColumnType.ImageButton;
                DeleteColumn.ImageUrl = "~/images/Delete.gif";
                DeleteColumn.Text = "Delete";
                DeleteColumn.ConfirmText = m_msg["T32"];
                DeleteColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                DeleteColumn.HeaderStyle.Width = 30;
                grd.MasterTableView.Columns.Add(DeleteColumn);

                grd.ShowHeader = true;
            }
        }
        grd.MasterTableView.DataKeyNames = new string[] { "Counter" };
        grd.MasterTableView.EditMode = GridEditMode.InPlace;
        
    }

    public void SetGrdDataSource(string sqlStr)
    {
        LabourSqlDataSource.ConnectionString = sqlStr;
        LabourSqlDataSource.SelectCommand = "SELECT * FROM WOLabour WHERE WoNum='" + m_wrnum + "' AND ordertype='workrequest'";

        MaterialSqlDataSource.ConnectionString = sqlStr;
        MaterialSqlDataSource.SelectCommand = "SELECT * FROM WOMaterial WHERE WoNum='" + m_wrnum + "' And ordertype='workrequest'";

        ServiceSqlDataSource.ConnectionString = sqlStr;
        ServiceSqlDataSource.SelectCommand = "SELECT * FROM woservice WHERE WoNum='" + m_wrnum + "' And ordertype='workrequest'";

        TasksSqlDataSource.ConnectionString = sqlStr;
        TasksSqlDataSource.SelectCommand = "SELECT * FROM wotasks WHERE WoNum='" + m_wrnum + "' And ordertype='workrequest'";

        ToolsSqlDataSource.ConnectionString = sqlStr;
        ToolsSqlDataSource.SelectCommand = "SELECT * FROM wotools WHERE WoNum='" + m_wrnum + "' AND ordertype='workrequest'";
    }

    private void SetGridInvisible(RadGrid grid, HiddenField hid)
    {
        grid.Style["visibility"] = "hidden";
        grid.Style["display"] = "none";
        hid.Value = "0";
    }

    private void RetrieveMessage()
    {
        SystemMessage msg = new SystemMessage("workorder/womain.aspx");
        m_msg = msg.GetSystemMessage();
        msg.SetJsMessage(litMessage);
    }


    private void SetCurrentWorkOrderNVC()
    {
        WorkRequest objWorkRequest = new WorkRequest(Session["Login"].ToString(), "WorkRequest", "WrNum", m_wrnum);
        nvcwr = objWorkRequest.ModuleData;


        //TextBox txtacthours = (TextBox)MainControlsPanel.FindControl("txtacthours");
        //TextBox txtactlabor = (TextBox)MainControlsPanel.FindControl("txtactlabor");
        //TextBox txtactmaterial = (TextBox)MainControlsPanel.FindControl("txtactmaterial");
        //TextBox txtactservice = (TextBox)MainControlsPanel.FindControl("txtactservice");
        //TextBox txtacttools = (TextBox)MainControlsPanel.FindControl("txtacttools");
        //TextBox txtesthours = (TextBox)MainControlsPanel.FindControl("txtesthours");
        //TextBox txtestlabor = (TextBox)MainControlsPanel.FindControl("txtestlabor");
        //TextBox txtestmaterial = (TextBox)MainControlsPanel.FindControl("txtestmaterial");
        //TextBox txtestservice = (TextBox)MainControlsPanel.FindControl("txtestservice");
        //TextBox txtesttools = (TextBox)MainControlsPanel.FindControl("txtesttools");


        //txtacthours.Text = nvcWO.GetValues("acthours")[0];
        //txtactlabor.Text = nvcWO.GetValues("actlabor")[0];
        //txtactmaterial.Text = nvcWO.GetValues("actmaterial")[0];
        //txtactservice.Text = nvcWO.GetValues("actservice")[0];
        //txtacttools.Text = nvcWO.GetValues("acttools")[0];

        //txtesthours.Text = nvcWO.GetValues("esthours")[0];
        //txtestlabor.Text = nvcWO.GetValues("estlabor")[0];
        //txtestmaterial.Text = nvcWO.GetValues("estmaterial")[0];
        //txtestservice.Text = nvcWO.GetValues("estservice")[0];
        //txtesttools.Text = nvcWO.GetValues("esttools")[0];
    }

    private void SetTextboxControls()
    {
        
        TextBox txtesthours = (TextBox)MainControlsPanel.FindControl("txtesthours");
        TextBox txtestlabor = (TextBox)MainControlsPanel.FindControl("txtestlabor");
        TextBox txtestmaterial = (TextBox)MainControlsPanel.FindControl("txtestmaterial");
        TextBox txtestservice = (TextBox)MainControlsPanel.FindControl("txtestservice");
        TextBox txtesttools = (TextBox)MainControlsPanel.FindControl("txtesttools");

        txtesthours.Text = nvcwr.GetValues("esthours")[0];
        txtestlabor.Text = nvcwr.GetValues("estlabor")[0];
        txtestmaterial.Text = nvcwr.GetValues("estmaterial")[0];
        txtestservice.Text = nvcwr.GetValues("estservice")[0];
        txtesttools.Text = nvcwr.GetValues("esttools")[0];
    }

    private void grid_DataBound(object sender, EventArgs e)
    {
        SetTextboxControls();
    }



}