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

public partial class pm_pmmain : System.Web.UI.Page
{
    AzzierScreen screen;
    private string connstring;
    protected string querymode = "";
    protected string m_pmnum, m_oldpmnum = "";
    PM objPM;
    protected NameValueCollection nvcPM;
    protected bool isquery = false;
    protected int statuscode = 0;
    protected NameValueCollection m_msg = new NameValueCollection();
    protected NameValueCollection m_rights;
    protected int m_allowedit = 0;
    protected string t_duplicate;
    protected RadGrid grdlabour;
    protected RadGrid grdmaterial;
    protected RadGrid grdservice;
    protected RadGrid grdtasks;
    protected RadGrid grdtools;

    protected void Page_Init(object sender, EventArgs e)
    {
        RetrieveMessage();
        UserRights.CheckAccess("pm");
        UserRights r = new UserRights(Session["Login"].ToString(), "UserRights", "counter");
        m_rights = r.GetRights(Session["Login"].ToString(), "pm");

        m_allowedit = Convert.ToInt16(m_rights["urEdit"].ToString());

        Session.LCID = Convert.ToInt32(Session["LCID"]);
        if (Request.QueryString["mode"] != null)
        {
            querymode = Request.QueryString["mode"].ToString();
            if (querymode == "")
                querymode = "query";
        }
        else
            querymode = "query";

        if (Request.QueryString["pmnum"] != null)
        {
            m_pmnum = Request.QueryString["pmnum"].ToString();
            m_oldpmnum = Request.QueryString["pmnum"].ToString();
            HidOldPMNum.Value = Request.QueryString["pmnum"].ToString();
          
        }
        else
            m_pmnum = "";

        if (querymode == "query")
            isquery = true;

        if (querymode == "edit")
        {
            objPM = new PM(Session["Login"].ToString(), "PM", "PMNum", m_pmnum);

            nvcPM = objPM.ModuleData;
            if (nvcPM["pmnum"].ToString() == "")
            {
                querymode = "query";
                isquery = true;
                hidMode.Value = querymode;
                m_pmnum = "";
            }
            UserWorkList list = new UserWorkList();
            list.AddToRecentList(Session["Login"].ToString(), "PM", m_pmnum);
        }
        else if (querymode == "duplicate" && m_oldpmnum.Length > 0)
        {
            objPM = new PM(Session["Login"].ToString(), "PM", "PMNum", m_oldpmnum);

            nvcPM = objPM.ModuleData;
            UserWorkList list = new UserWorkList();
            //list.AddToRecentList(Session["Login"].ToString(), "Labour", m_oldempid);
            litScript1.Text = "clearoldlocationval()";
        }
        else
        {
            objPM = new PM(Session["Login"].ToString(), "PM", "PMNum");
            nvcPM = objPM.ModuleData;
            statuscode = 1;
        }
        hidMode.Value = querymode;
        connstring = Application["ConnString"].ToString();
        
        InitScreen();
    }

    private void InitScreen()
    {
        screen = new AzzierScreen("pm/pmmain.aspx", "MainForm", MainControlsPanel.Controls, querymode);
        screen.LCID = Session.LCID;

        grdservice = new RadGrid();
        grdlabour = new RadGrid();
        grdmaterial = new RadGrid();
        grdtasks = new RadGrid();
        grdtools = new RadGrid();
        
        if (querymode == "edit")
        {
          //service
          SetGrdDataSource(connstring);
          InitAllGrid(grdservice, "service", "ServiceSqlDataSource");

          grdservice.ItemDataBound += new GridItemEventHandler(grdservice_ItemDataBound);
          grdservice.DeleteCommand += new GridCommandEventHandler(grdservice_DeleteCommand);
          grdservice.UpdateCommand += new GridCommandEventHandler(grdservice_UpdateCommand);
          grdservice.InsertCommand += new GridCommandEventHandler(grdservice_InsertCommand);
          grdservice.ItemCommand += new GridCommandEventHandler(grdservice_ItemCommand);
          grdservice.DataBound += new EventHandler(grid_DataBound);


          //Labour
          InitAllGrid(grdlabour, "labour", "LabourSqlDataSource");
          
          grdlabour.ItemDataBound += new GridItemEventHandler(grdlabour_ItemDataBound);
          grdlabour.DeleteCommand += new GridCommandEventHandler(grdlabour_DeleteCommand);
          grdlabour.UpdateCommand += new GridCommandEventHandler(grdlabour_UpdateCommand);
          grdlabour.InsertCommand += new GridCommandEventHandler(grdlabour_InsertCommand);
          grdlabour.ItemCommand += new GridCommandEventHandler(grdlabour_ItemCommand);
          grdlabour.DataBound += new EventHandler(grid_DataBound);
          
          //Material
          InitAllGrid(grdmaterial, "material", "MaterialSqlDataSource");
          
          grdmaterial.ItemDataBound += new GridItemEventHandler(grdmaterial_ItemDataBound);
          grdmaterial.DeleteCommand += new GridCommandEventHandler(grdmaterial_DeleteCommand);
          grdmaterial.UpdateCommand += new GridCommandEventHandler(grdmaterial_UpdateCommand);
          grdmaterial.InsertCommand += new GridCommandEventHandler(grdmaterial_InsertCommand);
          grdmaterial.ItemCommand += new GridCommandEventHandler(grdmaterial_ItemCommand);
          grdmaterial.DataBound += new EventHandler(grid_DataBound);

          //Tasks
          InitAllGrid(grdtasks, "tasks", "TasksSqlDataSource");
          
          grdtasks.ItemDataBound += new GridItemEventHandler(grdtasks_ItemDataBound);
          grdtasks.DeleteCommand += new GridCommandEventHandler(grdtasks_DeleteCommand);
          grdtasks.UpdateCommand += new GridCommandEventHandler(grdtasks_UpdateCommand);
          grdtasks.InsertCommand += new GridCommandEventHandler(grdtasks_InsertCommand);
          grdtasks.ItemCommand += new GridCommandEventHandler(grdtasks_ItemCommand);
          
          // Tools 
          InitAllGrid(grdtools, "tools", "ToolsSqlDataSource");
          grdtools.ItemDataBound += new GridItemEventHandler(grdtools_ItemDataBound);
          grdtools.DeleteCommand += new GridCommandEventHandler(grdtools_DeleteCommand);
          grdtools.UpdateCommand += new GridCommandEventHandler(grdtools_UpdateCommand);
          grdtools.InsertCommand += new GridCommandEventHandler(grdtools_InsertCommand);
          grdtools.ItemCommand += new GridCommandEventHandler(grdtools_ItemCommand);
          grdtools.DataBound += new EventHandler(grid_DataBound);


          grdlabour.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(1, true, "Labour", null, "return editLabour('',1)", m_allowedit);
          grdmaterial.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(1, true, "Material", null, "return editMaterial('',1)", m_allowedit);
          //grdtasks.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(1, true, "Tasks", null, "return editTask('',1)", m_allowedit);
          grdtasks.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(1, true, "Tasks", null, "return editTask('',1)", m_allowedit, "", true, "", "", "return addfromtasklib('PM','" + m_pmnum + "','1');", "Add Task Library");
          grdservice.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(1, true, "Services", null, "return editService('',1)", m_allowedit);
          grdtools.MasterTableView.CommandItemTemplate = new MultiFunctionItemTemplate(1, true, "Tools", null, "return editTools('',1)", m_allowedit);

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

        if (!isquery)
            screen.SetValidationControls();
    }



    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
          if (querymode == "edit")
          {
            screen.PopulateScreen("pm", nvcPM);
            HidDivision.Value = nvcPM["division"];
            string m_equipID = nvcPM["equipment"];
            if (m_equipID != "")
            {
              Equipment objEquipment = new Equipment(Session["Login"].ToString(), "equipment", "equipment", m_equipID);
              NameValueCollection nvcEQUIP = objEquipment.ModuleData;
              if (nvcEQUIP != null)
              {
                HidMobile.Value = nvcEQUIP["mobileequipment"].ToString();
              }
            }

            UserRights routeright = new UserRights(Session["Login"].ToString(), "UserRights", "Counter");
            NameValueCollection nvcrouteright = routeright.GetRights(Session["Login"].ToString(), "Route");
            if (nvcrouteright["AllowAccess"] != "1")
              btnshowroute.Visible = false;
          }
          else
          {
            btnshowroute.Visible = false;
          }
            if (querymode == "duplicate")
            {
              NewNumber n = new NewNumber();
              string pmnum = n.GetNextNumber("PM");
              nvcPM["PMNum"] = pmnum;
              screen.PopulateScreen("pm", nvcPM);
            }

            RadioButtonList r = (RadioButtonList)MainControlsPanel.FindControl("rblinactive");

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
                    {
                        if (nvcPM["inactive"].ToString() == "1")
                            r.SelectedIndex = 0;
                        else
                            r.SelectedIndex = 1;
                    }
                }
                else
                {
                    r.SelectedIndex = 1;
                }
            }

            r = (RadioButtonList)MainControlsPanel.FindControl("rblislocked");

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
                {
                  if (nvcPM["islocked"].ToString() == "1")
                    r.SelectedIndex = 0;
                  else
                    r.SelectedIndex = 1;
                }
              }
              else
              {
                r.SelectedIndex = 1;
              }
            }

            r = (RadioButtonList)MainControlsPanel.FindControl("rblondue");

            if (r != null)
            {
              r.Style.Add("valign", "top");
              r.Font.Size = 14;
              r.ForeColor = System.Drawing.Color.Navy;
              r.Width = Unit.Pixel(300);
              r.RepeatDirection = RepeatDirection.Horizontal;
              ListItem litm1 = new ListItem("On Due", "1");
              r.Items.Add(litm1);
              ListItem litm2 = new ListItem("On Complete", "0");
              r.Items.Add(litm2);
              if (querymode == "query")
              {
                ListItem litm3 = new ListItem("All", "");
                r.Items.Add(litm3);
                r.SelectedIndex = 2;
              }
              else if (querymode == "edit")
              {
                {
                  if (nvcPM["ondue"].ToString() == "1")
                    r.SelectedIndex = 0;
                  else
                    r.SelectedIndex = 1;
                }
              }
              else
              {
                r.SelectedIndex = 0;
              }
            }

            r = (RadioButtonList)MainControlsPanel.FindControl("rblopenwo");

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
                {
                  if (nvcPM["openwo"].ToString() == "1")
                    r.SelectedIndex = 0;
                  else
                    r.SelectedIndex = 1;
                  r.Attributes.Add("onclick", "roradiobutton('rblopenwo'," + r.SelectedIndex.ToString() + ")");
                }
                //r.Enabled = false;
              }
              else
              {
                r.SelectedIndex = 1;
                r.Attributes.Add("onclick", "roradiobutton('rblopenwo',1)");
                //r.Enabled = false;
              }
            }


            r = (RadioButtonList)MainControlsPanel.FindControl("rblcbcode");
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
                if (nvcPM["cbcode"].ToString() == "1")
                  r.SelectedIndex = 0;
                else
                  r.SelectedIndex = 1;
              }
            }

            r = (RadioButtonList)MainControlsPanel.FindControl("rblnestedpm");
            if (r != null)
            {
              r.RepeatDirection = RepeatDirection.Horizontal;
              ListItem litm1 = new ListItem("Yes", ">0");
              r.Items.Add(litm1);
              ListItem litm2 = new ListItem("No", "0");
              r.Items.Add(litm2);
              if (querymode == "query")
              {
                ListItem litm3 = new ListItem("All", "");
                r.Items.Add(litm3);
                r.SelectedIndex = 2;
              }
              else if (querymode == "new")
              {
                r.SelectedIndex = 1;
              }
              else if (querymode == "edit" || querymode == "duplicate")
              {
                if (objPM.ModuleData["nestedpm"].ToString() == "0")
                  r.SelectedIndex = 1;
                else
                  r.SelectedIndex = 0;
              }
              
              if (querymode != "query")
              {

                r.Attributes.Add("onclick", "roradiobutton('rblnestedpm'," + r.SelectedIndex.ToString() + ")");
              }
            }


            if (objPM.ModuleData["MeterName"] != "" && objPM.ModuleData["Equipment"] != "")
            {
              TextBox t = (TextBox) MainControlsPanel.FindControl("txtcurrentreading");
              MeterReading m = new MeterReading(Session["Login"].ToString(), "MeterReading", "Counter");
              double reading,offset;
              m.GetCurrentReading(objPM.ModuleData["Equipment"], objPM.ModuleData["MeterName"], out reading, out offset);
              t.Text = (reading - offset).ToString();
            }

            if (objPM.ModuleData["MeterName2"] != "" && objPM.ModuleData["Equipment"] != "")
            {
              TextBox t = (TextBox)MainControlsPanel.FindControl("txtcurrentreading2");
              MeterReading m = new MeterReading(Session["Login"].ToString(), "MeterReading", "Counter");
              double reading, offset;
              m.GetCurrentReading(objPM.ModuleData["Equipment"], objPM.ModuleData["MeterName2"], out reading, out offset);
              t.Text = (reading - offset).ToString();
            }

            CheckBox chk = MainControlsPanel.FindControl("chkdualmeter") as CheckBox;
            if (chk != null)
            {
              chk.Attributes.Add("onclick", "return false");
              chk.Attributes.Add("onkeydown", "return false");
              if (querymode == "edit")
              {
                chk.Checked = objPM.IsDualMeter;
              }

            }
          /*
            chk = MainControlsPanel.FindControl("chknestedpm") as CheckBox;
            if (chk != null && querymode != "query")
            {
              chk.Attributes.Add("onclick", "return false");
              chk.Attributes.Add("onkeydown", "return false");
            }
           * */

            chk = MainControlsPanel.FindControl("chkseasonalpm") as CheckBox;
            if (chk != null)
            {
              chk.Attributes.Add("onclick", "return false");
              chk.Attributes.Add("onkeydown", "return false");
              if (querymode == "edit")
              {
                chk.Checked = objPM.IsSeasonal;
              }
            }

            if (querymode != "query")
            {
              TextBox tbx = MainControlsPanel.FindControl("txtlocation") as TextBox;
              tbx.Attributes.Add("onFocus", "locFocus()");

              HyperLink h = MainControlsPanel.FindControl("lkulocation") as HyperLink;
              if (h.ToString() != "")
              {
                h.NavigateUrl = "javascript:loclookup(1)";
              }

              h = MainControlsPanel.FindControl("tlklocation") as HyperLink;
              if (h != null)
              {
                h.NavigateUrl = "javascript:loclookup(2)";
              }

              
              tbx = MainControlsPanel.FindControl("txtmetername") as TextBox;
              tbx.Attributes.Add("onFocus", "meterfocus()");

              tbx = MainControlsPanel.FindControl("txtmetername2") as TextBox;
              tbx.Attributes.Add("onFocus", "meterfocus()");

              h = MainControlsPanel.FindControl("lkumetername") as HyperLink;
              if (h != null)
              {
                h.NavigateUrl = "javascript:meterlookup(1)";
              }

              h = MainControlsPanel.FindControl("lkumetername2") as HyperLink;
              if (h != null)
              {
                h.NavigateUrl = "javascript:meterlookup(2)";
              }

            }
            TextBox tr = MainControlsPanel.FindControl("txtcurrentreading") as TextBox;
            if (tr != null)
              tr.Attributes.Add("decimalplaces", "2");
            tr = MainControlsPanel.FindControl("txtcurrentreading2") as TextBox;
            if (tr != null)
              tr.Attributes.Add("decimalplaces", "2");

            HidLabTax1.Value = "0";
            HidLabTax2.Value = "0";
            //HidLabSelType.Value = seltype;
            HidLabCost.Value = Application["LabCost"].ToString();

            //HidSysMatPrice.Value = Application["IssuePrice"].ToString();
            HidMatCost.Value = Application["MatCost"].ToString();
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
        ucHeader1.ModuleData = nvcPM;

        string oper = "";
        if (querymode == "query")
        {
            oper = "Query";
        }
        else if (querymode == "new")
        {
            oper = "New";
            if (Request.QueryString["numbering"] == "auto")
            {
              TextBox t = MainControlsPanel.FindControl("txtpmnum") as TextBox;
              if (t != null)
              {
                if (!Page.IsPostBack)
                {
                  NewNumber num = new NewNumber();
                  //t.Text = num.GetNextNumber("PM");
                  nvcPM["PMNum"] = num.GetNextNumber("PM");
                }
              }
            }
            screen.GetDefaultValue("PM", nvcPM, "PM", "new");
            screen.PopulateScreen("PM", nvcPM);
        }
        else if (querymode == "duplicate")
        {
            oper = "Duplicate";
            litScript1.Text = "<script>clearnonduplicatefields();</script>";
        }
        else if (querymode == "edit")
        {
            oper = "Edit";
        }
        ucHeader1.OperationLabel = oper;
        hidprocnum.Value = nvcPM["ProcNum"];
    }
  
    private void Query()
    {
      NameValueCollection nvc;
      
      nvc = screen.CollectFormValues("PM", true);  // use the view name

      Validation v = new Validation();
      string pmnum = nvc["PMNum"].ToString() ?? "";
      if (pmnum.Length > 1)
      {
        if (v.SpecialStrValidator(pmnum))
        {
          nvc = new NameValueCollection();
          nvc.Add("PMNum", pmnum);
        }
      }


      PM obj = new PM(Session["Login"].ToString(), "PM", "PMNum");   // use the view name
      string wherestring = "", linqwherestring = "";
      //string[] equipments = obj.Query(nvc, ref wherestring);
      //string[] pms = obj.LinqQuery(nvc, ref wherestring, ref linqwherestring);
      obj.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
      string jscript = "<script type=\"text/javascript\">";
      jscript += "var wherestring = '" + Server.UrlEncode(linqwherestring) + "';var obj=parent.controlpanel.document.location.href='pmpanel.aspx?showlist=queryresult&wherestring='+wherestring;";
      jscript += "</script>";
      litScript1.Text = jscript;
      //litFrameScript.Text = jscript;
    }
  
    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
    }

    private void RetrieveMessage()
    {
        SystemMessage msg = new SystemMessage("pm/pmmain.aspx");
        m_msg = msg.GetSystemMessage();
        msg.SetJsMessage(litMessage);
    }

    protected void InitAllGrid(RadGrid grd, string controlid, string grddatasource)
    {
      grd.ID = string.Concat("grd", controlid);
      grd.DataSourceID = grddatasource;
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

      if (m_allowedit == 1)
      {
        GridEditCommandColumn EditColumn = new GridEditCommandColumn();
        EditColumn.HeaderText = "Edit";
        EditColumn.UniqueName = "EditCommand";
        EditColumn.ButtonType = GridButtonColumnType.ImageButton;
        EditColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
        EditColumn.HeaderStyle.Width = 30;
        grd.MasterTableView.Columns.Add(EditColumn);
        grd.ClientSettings.EnablePostBackOnRowClick = true;
      }

      screen.SetGridColumns(controlid, grd);

      // New Request
      if (m_allowedit == 1)
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
      grd.MasterTableView.DataKeyNames = new string[] { "Counter" };
      grd.MasterTableView.EditMode = GridEditMode.InPlace;
    }

    public void SetGrdDataSource(string sqlStr)
    {
      LabourSqlDataSource.ConnectionString = sqlStr;
      LabourSqlDataSource.SelectCommand = "SELECT * FROM WOLabour WHERE WoNum='" + m_pmnum + "' AND ordertype='pm'";

      MaterialSqlDataSource.ConnectionString = sqlStr;
      MaterialSqlDataSource.SelectCommand = "SELECT * FROM WOMaterial WHERE WoNum='" + m_pmnum + "' AND orderType='pm'";

      ServiceSqlDataSource.ConnectionString = sqlStr;
      ServiceSqlDataSource.SelectCommand = "SELECT * FROM woservice WHERE WoNum='" + m_pmnum + "' AND ordertype='pm'";
      
      TasksSqlDataSource.ConnectionString = sqlStr;
      TasksSqlDataSource.SelectCommand = "SELECT * FROM wotasks WHERE WoNum='" + m_pmnum + "' AND ordertype='pm'";

      ToolsSqlDataSource.ConnectionString = sqlStr;
      ToolsSqlDataSource.SelectCommand = "SELECT * FROM wotools WHERE WoNum='" + m_pmnum + "' And ordertype='pm'";
      
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

        if (m_allowedit == 1)
        {
          ImageButton btn = (ImageButton)item["EditCommand"].Controls[0];
          btn.ImageUrl = "~/images/Edit.gif";
          btn.OnClientClick = "return editService(" + item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString() + ",1)";
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
            screen.InitResourceRecord("pm", null, e, "mainpanel", "service", "1", m_pmnum);
        }
      }

      screen.GridItemDataBound(e, "pm/pmmain.aspx", "MainForm", "service");
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

      Service obj = new Service(Session["Login"].ToString(), "WOService", "Counter", counter);
      bool success = obj.Delete();
      if (!success)
      {
        grdservice.Controls.Add(new LiteralControl(m_msg["T14"] + obj.ErrorMessage));
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
              //string result = "";
              //bool a = objDateFormat.ValidateInputDate(sdate, out result);    // convert date from screen format to db date format
              //sdate = result;
              nvc.Add(field, sdate);
            }
          }
        }
      }

      Service objResource = new Service(Session["Login"].ToString(), "WOService", "Counter", counter);
      bool success = objResource.Update(nvc);
      if (!success)
      {
        grdservice.Controls.Add(new LiteralControl(m_msg["T15"] + objResource.ErrorMessage));
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

      nvc.Add("wonum", m_pmnum);
      nvc.Add("estimate", "1");
      nvc.Add("ordertype", "pm");

      Service objResource = new Service(Session["Login"].ToString(), "WOService", "Counter");
      bool success = objResource.Create(nvc);

      if (!success)
      {
        grdservice.Controls.Add(new LiteralControl("Unable to insert actual service record. Reason: " + objResource.ErrorMessage));
        e.Canceled = true;
      }
      SetCurrentWorkOrderNVC();
    }

    protected void grdservice_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
      if (e.CommandName.ToString() == "InPlaceInitInsert")
      {
        e.Canceled = true;
        System.Collections.Specialized.ListDictionary newValues = new System.Collections.Specialized.ListDictionary();
        newValues["WoNum"] = m_pmnum;
        newValues["Estimate"] = 1;
        newValues["Actual"] = 0;
        newValues["ChargeBack"] = nvcPM["ChargeBack"];
        grdservice.MasterTableView.EditMode = GridEditMode.InPlace;
        e.Item.OwnerTableView.InsertItem(newValues);
      }
      else if (e.CommandName == "RowClick")
      {
        GridDataItem item = (GridDataItem)e.Item;
        item.Edit = true;
        grdservice.MasterTableView.EditMode = GridEditMode.InPlace;
        grdservice.Rebind();
      }
    }

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

        if (m_allowedit == 1)// if (statuscode < 100 && statuscode > 0)
        {
          ImageButton btn = (ImageButton)item["EditCommand"].Controls[0];
          btn.ImageUrl = "~/images/Edit.gif";
          btn.OnClientClick = "return editTask(" + item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString() + ",1)";
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
            t_imagebtn.ValidationGroup = "tasks";
        }
        else
        {
          t_imagebtn = ((ImageButton)editedItem.FindControl("UpdateButton"));
          if (t_imagebtn != null)
            t_imagebtn.ValidationGroup = "tasks";
        }
      }

      screen.GridItemDataBound(e, "pm/pmmain.aspx", "MainForm", "tasks");
    }

    protected void grdtasks_DeleteCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
      GridDataItem item = (GridDataItem)e.Item;
      string counter = item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString();

      Task objResource = new Task(Session["Login"].ToString(), "WOTasks", "Counter", counter);
      bool success = objResource.Delete();
      if (!success)
      {
        grdtasks.Controls.Add(new LiteralControl(m_msg["T28"] + objResource.ErrorMessage));
        e.Canceled = true;
      }
    }

    protected void grdtasks_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
      if (e.CommandName.ToString() == "InPlaceInitInsert")
      {
        e.Canceled = true;
        System.Collections.Specialized.ListDictionary newValues = new System.Collections.Specialized.ListDictionary();
        newValues["WoNum"] = m_pmnum;
        newValues["Estimate"] = 1;
        newValues["Actual"] = 0;
        grdtasks.MasterTableView.EditMode = GridEditMode.InPlace;
        e.Item.OwnerTableView.InsertItem(newValues);
      }
      else if (e.CommandName == RadGrid.EditCommandName)
      {
        grdtasks.MasterTableView.EditMode = GridEditMode.PopUp;
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
              //string result = "";
              //bool a = objDateFormat.ValidateInputDate(sdate, out result);    // convert date from screen format to db date format
              //sdate = result;
              nvc.Add(field, sdate);
            }
          }
        }
      }
      nvc.Add("wonum", m_pmnum);
      nvc.Add("estimate", "1");
      nvc.Add("ordertype", "pm");

      Task objResource = new Task(Session["Login"].ToString(), "WOTasks", "Counter");
      bool success = objResource.Create(nvc);
      if (!success)
      {
        grdtasks.Controls.Add(new LiteralControl(m_msg["T29"] + objResource.ErrorMessage));
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
        grdtasks.Controls.Add(new LiteralControl(m_msg["T30"] + objResource.ErrorMessage));
        e.Canceled = true;
      }
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
        grdlabour.Controls.Add(new LiteralControl(m_msg["T2"] + objResource.ErrorMessage));
        e.Canceled = true;
      }
      SetCurrentWorkOrderNVC();
    }

    protected void grdlabour_UpdateCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
      //Get the GridEditableItem of the RadGrid
      GridEditableItem editedItem = e.Item as GridEditableItem;
      //Get the primary key value using the DataKeyValue.
      string counter = editedItem.OwnerTableView.DataKeyValues[editedItem.ItemIndex]["Counter"].ToString();

      DateFormat objDateFormat = new DateFormat(Session.LCID);

      NameValueCollection nvcFT = screen.GetGridFieldTypes("labour", "WOLabour");
      string[] fields = nvcFT.AllKeys;
      NameValueCollection nvc = new NameValueCollection();
      foreach (string field in fields)
      {
        if (field != "counter" && field != "wonum" && field != "estimate" && field != "ordertype")
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

      Labour objResource = new Labour(Session["Login"].ToString(), "WOLabour", "Counter", counter);
      bool success = objResource.Update(nvc);
      if (!success)
      {
        grdlabour.Controls.Add(new LiteralControl(m_msg["T4"] + objResource.ErrorMessage));
        e.Canceled = true;
      }
      SetCurrentWorkOrderNVC();
    }

    protected void grdlabour_InsertCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
      //Get the GridEditFormInsertItem of the RadGrid
      GridDataInsertItem insertedItem = e.Item as GridDataInsertItem;

      DateFormat objDateFormat = new DateFormat(Session.LCID);
      NameValueCollection nvcFT = screen.GetGridFieldTypes("labour", "WOLabour");
      string[] fields = nvcFT.AllKeys;
      NameValueCollection nvc = new NameValueCollection();
      foreach (string field in fields)
      {
        if (field != "counter" && field != "wonum" && field != "estimate" && field != "ordertype")
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
      nvc.Add("wonum", m_pmnum);
      nvc.Add("estimate", "1");
      nvc.Add("ordertype", "pm");

      Labour objResource = new Labour(Session["Login"].ToString(), "WOLabour", "Counter");
      bool success = objResource.Create(nvc);

      if (!success)
      {
        grdlabour.Controls.Add(new LiteralControl(m_msg["T6"] + objResource.ErrorMessage));
        e.Canceled = true;
      }
      else
      {
        grdlabour.Rebind();
      }
      SetCurrentWorkOrderNVC();
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

      if (e.Item is GridDataItem && !e.Item.IsInEditMode)
      {
        if (m_allowedit == 1)
        {
          GridDataItem item = (GridDataItem)e.Item;
          ImageButton btn = (ImageButton)item["EditCommand"].Controls[0];
          btn.ImageUrl = "~/images/Edit.gif";
          btn.OnClientClick = "return editLabour(" + item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString() + ",1)";
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
            screen.InitResourceRecord("pm", null, e, "mainpanel", "labour", "1", m_pmnum);
        }
      }

      screen.GridItemDataBound(e, "pm/pmmain.aspx", "MainForm", "labour");
    }

    protected void grdlabour_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
      if (e.CommandName.ToString() == "InPlaceInitInsert")
      {
        e.Canceled = true;
        System.Collections.Specialized.ListDictionary newValues = new System.Collections.Specialized.ListDictionary();
        newValues["WoNum"] = m_pmnum;
        newValues["Estimate"] = 1;
        newValues["Actual"] = 0;
        newValues["ChargeBack"] = nvcPM["ChargeBack"];
        grdlabour.MasterTableView.EditMode = GridEditMode.InPlace;
        e.Item.OwnerTableView.InsertItem(newValues);
      }
      else if (e.CommandName == "RowClick")
      {
        GridDataItem item = (GridDataItem)e.Item;
        item.Edit = true;
        grdlabour.MasterTableView.EditMode = GridEditMode.InPlace;
        grdlabour.Rebind();
        //e.Canceled = true;
      }
    }

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

        if (m_allowedit == 1)
        {
          ImageButton btn = (ImageButton)item["EditCommand"].Controls[0];
          btn.ImageUrl = "~/images/Edit.gif";
          btn.OnClientClick = "return editTools(" + item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString() + ",0)";
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
            screen.InitResourceRecord("pm", null, e, "mainpanel", "tool", "1", m_pmnum);
        }
      }

      screen.GridItemDataBound(e, "pm/pmmain.aspx", "MainForm", "tools");
    }

    protected void grdtools_DeleteCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
      GridDataItem item = (GridDataItem)e.Item;
      string counter = item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString();

      Tool objResource = new Tool(Session["Login"].ToString(), "WOTools", "Counter", counter);
      bool success = objResource.Delete();
      if (!success)
      {
        grdtools.Controls.Add(new LiteralControl(m_msg["T22"] + objResource.ErrorMessage));
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
              nvc.Add(field, sdate);
            }
          }
        }
      }

      Tool objResource = new Tool(Session["Login"].ToString(), "WOTools", "Counter", counter);
      bool success = objResource.Update(nvc);
      if (!success)
      {
        grdtools.Controls.Add(new LiteralControl(m_msg["T23"] + objResource.ErrorMessage));
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
              nvc.Add(field, sdate);
            }
          }
        }
      }

      nvc.Add("wonum", m_pmnum);
      nvc.Add("estimate", "1");
      nvc.Add("ordertype", "pm");

      Tool objResource = new Tool(Session["Login"].ToString(), "WOTools", "Counter");
      bool success = objResource.Create(nvc);

      if (!success)
      {
        grdtools.Controls.Add(new LiteralControl(m_msg["T24"] + objResource.ErrorMessage));
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
        newValues["WoNum"] = m_pmnum;
        newValues["Estimate"] = 1;
        newValues["Actual"] = 0;
        newValues["ChargeBack"] = nvcPM["ChargeBack"];
        grdtools.MasterTableView.EditMode = GridEditMode.InPlace;
        e.Item.OwnerTableView.InsertItem(newValues);
      }
      else if (e.CommandName == RadGrid.EditCommandName)
      {
        grdtools.MasterTableView.EditMode = GridEditMode.PopUp;
      }
      else if (e.CommandName == "RowClick")
      {
        GridDataItem item = (GridDataItem)e.Item;
        item.Edit = true;
        grdtools.MasterTableView.EditMode = GridEditMode.InPlace;
        grdtools.Rebind();
      }
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
        grdmaterial.Controls.Add(new LiteralControl(m_msg["T9"] + objResource.ErrorMessage));
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
            nvc.Add(field, sdate);
          }
        }
      }

      nvc.Add("estimate", "1");
      nvc.Add("wonum", m_pmnum);
      nvc.Add("ordertype", "pm");

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
      DateFormat objDateFormat = new DateFormat(Session.LCID);
      NameValueCollection nvcFT = screen.GetGridFieldTypes("material", "WOMaterial");
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

      nvc.Add("wonum", m_pmnum);
      nvc.Add("estimate", "1");
      nvc.Add("ordertype", "pm");

      Material objResource = new Material(Session["Login"].ToString(), "WOMaterial", "Counter");
      bool success = objResource.Create(nvc);

      if (!success)
      {
        grdmaterial.Controls.Add(new LiteralControl(m_msg["T24"] + objResource.ErrorMessage));
        e.Canceled = true;
      }
      SetCurrentWorkOrderNVC();
    }

    protected void grdmaterial_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
    {
      if (e.CommandName.ToString() == "InPlaceInitInsert")
      {
        e.Canceled = true;
        System.Collections.Specialized.ListDictionary newValues = new System.Collections.Specialized.ListDictionary();
        newValues["WoNum"] = m_pmnum;
        newValues["Estimate"] = 1;
        newValues["Actual"] = 0;
        newValues["ChargeBack"] = nvcPM["ChargeBack"];
        grdmaterial.MasterTableView.EditMode = GridEditMode.InPlace;
        e.Item.OwnerTableView.InsertItem(newValues);
      }
      else if (e.CommandName == RadGrid.EditCommandName)
      {
        grdmaterial.MasterTableView.EditMode = GridEditMode.PopUp;
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
      if (e.Item is GridCommandItem)
      {

        GridCommandItem commandItem = (GridCommandItem)e.Item;
        ImageButton t_addInPlaceButton = (ImageButton)commandItem.FindControl("addInPlaceButton");
        if (t_addInPlaceButton != null)
          t_addInPlaceButton.ValidationGroup = "material";
      }

      if (e.Item is GridDataItem && !e.Item.IsInEditMode)
      {
        GridDataItem item = (GridDataItem)e.Item;

        if (m_allowedit == 1)
        {
          ImageButton btn = (ImageButton)item["EditCommand"].Controls[0];
          btn.ImageUrl = "~/images/Edit.gif";
          btn.OnClientClick = "return editMaterial(" + item.OwnerTableView.DataKeyValues[item.ItemIndex]["Counter"].ToString() + ",0)";
        }
      }

      screen.GridItemDataBound(e, "pm/pmmain.aspx", "MainForm", "material");

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
          /*
          if (store == "")
            itemnumtext.Attributes["fieldlist"] = assemble.ReplaceControlID("itemnum^_RT_itemnum^13,itemdesc^_RT_description^11,issueunit^_RT_unit,avgprice^_RT_HidAvgPrice^11,lastprice^_RT_HidLastPrice^11,quotedprice^_RT_HidQuotedPrice^11,fixprice^_RT_HidFixPrice,issueprice^_RT_HidIssuePrice");
          else
            itemnumtext.Attributes["fieldlist"] = assemble.ReplaceControlID("itemnum^_RT_itemnum^11,itemdesc^_RT_description^11,issueunit^_RT_unit,avgprice^_RT_HidAvgPrice^11,lastprice^_RT_HidLastPrice^11,quotedprice^_RT_HidQuotedPrice^11,fixprice^_RT_HidFixPrice,issueprice^_RT_HidIssuePrice");
           * */
        }
        if (e.Item.OwnerTableView.IsItemInserted)
        {
            screen.InitResourceRecord("pm", null, e, "mainpanel", "material", "1", m_pmnum);
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

      screen.GridItemDataBound(e, "pm/pmmain.aspx", "MainForm", "material");
       * */
    }

    private void SetCurrentWorkOrderNVC()
    {
        objPM = new PM(Session["Login"].ToString(), "PM", "PMNum", m_oldpmnum);
        nvcPM = objPM.ModuleData;


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

        txtesthours.Text = nvcPM.GetValues("esthours")[0];
        txtestlabor.Text = nvcPM.GetValues("estlabor")[0];
        txtestmaterial.Text = nvcPM.GetValues("estmaterial")[0];
        txtestservice.Text = nvcPM.GetValues("estservice")[0];
        txtesttools.Text = nvcPM.GetValues("esttools")[0];
    }

    void grid_DataBound(object sender, EventArgs e)
    {
        SetTextboxControls();
    }
}