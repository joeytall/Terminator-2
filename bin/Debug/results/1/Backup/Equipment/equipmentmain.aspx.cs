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

public partial class equipment_equipmentmain : System.Web.UI.Page
{
    AzzierScreen screen;
    private string connstring;
    protected string querymode = "";
    protected string m_equipment, m_oldequipment = "";

    Equipment objEquipment;
    NameValueCollection nvcEquipment;
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
        UserRights.CheckAccess("equipment");
        UserRights r = new UserRights(Session["Login"].ToString(), "UserRights", "counter");
        m_rights = r.GetRights(Session["Login"].ToString(), "equipment");

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

        if (Request.QueryString["equipment"] != null)
        {
            m_equipment = Request.QueryString["equipment"].ToString();
            m_oldequipment = Request.QueryString["equipment"].ToString();
            HidOldEquipment.Value = Request.QueryString["equipment"].ToString();
        }
        else
            m_equipment = "";

        if (querymode == "query")
            isquery = true;

        if (querymode == "edit")
        {
            objEquipment = new Equipment(Session["Login"].ToString(), "Equipment", "Equipment", m_equipment);
            int result = objEquipment.AllowDelete();
            if (result == 1)  //has open wo
              m_deletemessage = "Equipment is on open work order. Deletion is not allowed.";
            else if (result==2) // has open po
              m_deletemessage = "Equipment is on open purchase order. Deletion is not allowed.";
            else if (result == 3) // has open wo and open po
              m_deletemessage = "Equipment is on open work order and purchase order. Deletion is not allowed.";

            nvcEquipment = objEquipment.ModuleData;
            if (nvcEquipment["Equipment"].ToString() == "")
            {
                querymode = "query";
                isquery = true;
                hidMode.Value = querymode;
                m_equipment = "";
            }

            UserWorkList list = new UserWorkList();
            list.AddToRecentList(Session["Login"].ToString(), "Equipment", m_equipment);
        }
        else if (querymode == "duplicate" && m_oldequipment.Length > 0)
        {
            objEquipment = new Equipment(Session["Login"].ToString(), "Equipment", "Equipment", m_oldequipment);

            nvcEquipment = objEquipment.ModuleData;
            UserWorkList list = new UserWorkList();
            //list.AddToRecentList(Session["Login"].ToString(), "Labour", m_oldempid);
            litScript1.Text = "clearoldlocationval()";
        }
        else
        {
            objEquipment = new Equipment(Session["Login"].ToString(), "Equipment", "Equipment");
            nvcEquipment = objEquipment.ModuleData;
            statuscode = 1;
        }
        hidMode.Value = querymode;
        connstring = Application["ConnString"].ToString();
        InitScreen();

    }

    private void InitScreen()
    {
        screen = new AzzierScreen("equipment/equipmentmain.aspx", "MainForm", MainControlsPanel.Controls, querymode);
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
                screen.PopulateScreen("Equipment", nvcEquipment);
                HidDivision.Value = nvcEquipment["division"];
            }
            if (querymode == "duplicate")
            {
                screen.PopulateScreen("Equipment", nvcEquipment);
            }
            if (querymode == "new" || querymode == "duplicate" )
            {
              if (querymode == "new")
              {
                screen.GetDefaultValue("Equipment", nvcEquipment, "Equipment", "new");
                screen.PopulateScreen("Equipment", nvcEquipment);
              }

              TextBox t = MainControlsPanel.FindControl("txtequipment") as TextBox;
              if (t != null)
              {
                if (!Page.IsPostBack && (Request.QueryString["numbering"] == "auto" || querymode == "duplicate"))
                {
                  NewNumber num = new NewNumber();
                  t.Text = num.GetNextNumber("EQUIP");
                }
                else
                  t.Text = "";
              }
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
                    {
                        if (nvcEquipment["masterrec"].ToString() == "1")
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
                    if (nvcEquipment["inactive"].ToString() == "1")
                        r.SelectedIndex = 0;
                    else
                        r.SelectedIndex = 1;
                    r.Attributes.Add("onclick", "roradiobutton('rblinactive'," + r.SelectedIndex.ToString() + ")");
                }
                else
                {
                    r.SelectedIndex = 1;
                }
            }

            r = (RadioButtonList)MainControlsPanel.FindControl("rblmobileequipment");
            if (r != null)
            {
                r.RepeatDirection = RepeatDirection.Horizontal;
                ListItem litm1 = new ListItem("Fixed", "0");
                r.Items.Add(litm1);
                ListItem litm2 = new ListItem("Mobile", "1");
                r.Items.Add(litm2);
                if (querymode == "query")
                {
                    ListItem litm3 = new ListItem("All", "");
                    r.Items.Add(litm3);
                    r.SelectedIndex = 2;
                }
                else if (querymode == "edit" || querymode == "duplicate")
                {
                    if (nvcEquipment["mobileequipment"].ToString() == "1")
                        r.SelectedIndex = 1;
                    else
                        r.SelectedIndex = 0;
                }
                else
                {
                    r.SelectedIndex = 0;
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
                if (nvcEquipment["cbcode"].ToString() == "1")
                  r.SelectedIndex = 0;
                else
                  r.SelectedIndex = 1;
                //r.Attributes.Add("onclick", "roradiobutton('rblcbcode'," + r.SelectedIndex.ToString() + ")");
              }
              else
              {
                r.SelectedIndex = 1;
              }
            }

            r = (RadioButtonList)MainControlsPanel.FindControl("rblhazardous");
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
              else if (querymode == "edit" || querymode == "duplicate")
              {
                if (nvcEquipment["Hazardous"].ToString() == "1")
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
            if (Request.Form["__EVENTTARGET"] == "ModifyEquipment")
            {
                ModifyEquipment();
            }
        }

        ucHeader1.Mode = querymode;
        ucHeader1.TabName = "Main";
        ucHeader1.ModuleData = nvcEquipment;

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
            //TextBox tbx = MainControlsPanel.FindControl("txtequipment") as TextBox;
            //NewNumber n = new NewNumber();
            //tbx.Text = n.GetNextNumber("Equip");
            litScript1.Text = "<script>clearoldequipmentval();</script>";
        }
        else if (querymode == "edit")
        {
            oper = "Edit";
        }
        ucHeader1.OperationLabel = oper;

        if (querymode == "edit")
        {
          if (objEquipment.ModuleData["ItemNum"] != "")
          {
            TextBox t = MainControlsPanel.FindControl("txtitemnum") as TextBox;
            if (t != null)
              if (t.Attributes["readonly"] == null)
                t.Attributes.Add("readonly", "readonly");
              else
                t.Attributes["readonly"] = "readonly";

            HyperLink h = MainControlsPanel.FindControl("lkuitemnum") as HyperLink;
            if (h != null)
              h.Visible = false;

          }
        }
      }

    private void Query()
    {
        Equipment objEquipment = new Equipment(Session["Login"].ToString(), "Equipment", "Equipment");

        NameValueCollection nvc;
        nvc = screen.CollectFormValues("Equipment", true);

        Validation v = new Validation();
        string eqptid = nvc["Equipment"].ToString() ?? "";
        if (eqptid.Length > 1)
        {
            //if (eqptid == null) { }

            if (v.SpecialStrValidator(eqptid))
            {
                nvc = new NameValueCollection();
                nvc.Add("Equipment", eqptid);
            }
        }

        string wherestring = "", linqwherestring = "";
        //string[] equipments = objEquipment.Query(nvc, ref wherestring);
        objEquipment.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
        string jscript = "";
        
        jscript = "<script>";
        jscript += "var wherestring = '" + Server.UrlEncode(linqwherestring) + "'; var obj=parent.controlpanel.document.location.href='equipmentpanel.aspx?showlist=queryresult&wherestring='+wherestring;";
        jscript += "</script>";
        litScript1.Text = jscript;
    }

    private Boolean ModifyEquipment()
    {
        SaveEquipment();
        string newdivision = hidNewDivision.Value;
        string newparentid = hidNewParentID.Value;
        string newstatus = hidNewStatus.Value;
        string newparentdesc = hidNewParentDesc.Value;
        string moddate = hidModDate.Value;
        string newoperator = hidNewOperator.Value;
        string newlocation = hidNewLocation.Value;
        string newlocationdesc = hidNewLocationDesc.Value;

        bool updateopen = true;
        bool updatehistory = false;
        bool updatechildlocation = false;
        bool keepchildrelation = true;

        if (hidUpdateOpen.Value == "true")
            updateopen = true;
        else
            updateopen = false;

        if (hidUpdateHistory.Value == "true")
            updatehistory = true;
        else
            updatehistory = false;

        if (hidKeepChildRelation.Value == "true")
            keepchildrelation = true;
        else
            keepchildrelation = false;

        if (hidUpdateChildLocation.Value == "true")
        {
            updatechildlocation = true;
        }

        Equipment eqp = new Equipment(Session["Login"].ToString(), "Equipment", "Equipment", m_equipment);
        eqp.ModifyEquipment(newlocation, newlocationdesc, newparentid, newparentdesc, newdivision, newstatus, newoperator, moddate, updateopen, updatehistory, keepchildrelation, updatechildlocation);
        if (eqp.ErrorMessage == "")
            litScript1.Text = "<script>document.location.href='equipmentmain.aspx?mode=edit&equipment=" + m_equipment + "'</script>";
        else
            //litScript1.Text = "<script>alert('" + eqp.ErrorMessage + "');document.location.href='equipmentmain.aspx?mode=edit&equipment=" + m_equipment + "'</script>";
          litScript1.Text = "<script>alert('" + eqp.ErrorMessage + "');</script>";
        return true;
    }
    private Boolean SaveEquipment()
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        TextBox tbx = null;
        string dirtylog = "0";
        if (CntlPanel != null)
        {
            nvc = screen.CollectFormValues("equipment", false);  // use the table name
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

        Equipment objEquipment;
        bool success = false;
        if (querymode == "new")
        {
            objEquipment = new Equipment(Session["Login"].ToString(), "Equipment", "Equipment");
            success = objEquipment.Create(nvc);
        }
        else if (querymode == "duplicate")
        {
            objEquipment = new Equipment(Session["Login"].ToString(), "Equipment", "Equipment");
            success = objEquipment.Create(nvc);
        }
        else if (querymode == "edit")
        {
            objEquipment = new Equipment(Session["Login"].ToString(), "Equipment", "Equipment", nvc["Equipment"]);
            success = objEquipment.Update(nvc);
        }
        else
            objEquipment = new Equipment(Session["Login"].ToString(), "Equipment", "Equipment");
        return success;
    }

    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
    }

    private void RetrieveMessage()
    {
        SystemMessage msg = new SystemMessage("equipment/equipmentmain.aspx");
        m_msg = msg.GetSystemMessage();
        msg.SetJsMessage(litMessage);
    }
}