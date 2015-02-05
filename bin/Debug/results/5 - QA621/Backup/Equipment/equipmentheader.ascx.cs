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

public partial class equipment_equipmentheader : System.Web.UI.UserControl
{
    private string m_mode;
    private string m_tabname;
    private string m_operationlabel;
    // private string m_empid = "";
    public string m_equipment = "";
    private NameValueCollection m_nvc;
    private AzzierScreen screen;
    protected string linksrvr = "";
    private bool m_allowedit;
    protected RadComboBox toolbarcombobox;
    private string m_division;
    protected string m_masterrec;
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
            m_equipment = m_nvc["equipment"];
            m_division = m_nvc["division"];
            m_masterrec =m_nvc["MasterRec"];
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
            tabhlkMain.NavigateUrl = "equipmentmain.aspx?mode=edit&equipment=" + m_nvc["equipment"];
            tabhlkMain.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgMain','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblMain','toptabover')");
            tabhlkMain.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgMain','../images/tabbutton.png');classChangeover('ucHeader1_tablblMain','toptabout')");
        }

        if (m_tabname == "Specifications")
        {
            tabhlkSpecs.NavigateUrl = "javascript:void(null)";
            tabimgSpecs.ImageUrl = "../images/tabbutton_down.png";
            tablblSpecs.CssClass = "toptabover";
        }
        else
        {
            if (m_mode == "edit")
            {
                tabhlkSpecs.NavigateUrl = "equipmentspecs.aspx?equipment=" + m_equipment;
                tabhlkSpecs.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgSpecs','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblSpecs','toptabover')");
                tabhlkSpecs.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgSpecs','../images/tabbutton.png');classChangeover('ucHeader1_tablblSpecs','toptabout')");
            }
            else
            {
                tabhlkSpecs.Enabled = false;
                tablblSpecs.CssClass = "toptabinactive";
            }
        }

        if (m_tabname == "Measurements")
        {
          tabhlkMeasurements.NavigateUrl = "javascript:void(null)";
          tabimgMeasurements.ImageUrl = "../images/tabbutton_down.png";
          tablblMeasurements.CssClass = "toptabover";
        }
        else
        {
          if (m_mode == "edit")
          {
            tabhlkMeasurements.NavigateUrl = "equipmentmeasurement.aspx?equipment=" + m_equipment;
            tabhlkMeasurements.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgMeasurements','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblMeasurements','toptabover')");
            tabhlkMeasurements.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgMeasurements','../images/tabbutton.png');classChangeover('ucHeader1_tablblMeasurements','toptabout')");
          }
          else
          {
            tabhlkMeasurements.Enabled = false;
            tablblMeasurements.CssClass = "toptabinactive";
          }
        }

        if (m_tabname == "Meter")
        {
          tabhlkMeter.NavigateUrl = "javascript:void(null)";
          tabimgMeter.ImageUrl = "../images/tabbutton_down.png";
          tablblMeter.CssClass = "toptabover";
        }
        else
        {
          if (m_mode == "edit")
          {
            tabhlkMeter.NavigateUrl = "equipmentmeter.aspx?equipment=" + m_equipment;
            tabhlkMeter.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgMeter','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblMeter','toptabover')");
            tabhlkMeter.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgMeter','../images/tabbutton.png');classChangeover('ucHeader1_tablblMeter','toptabout')");
          }
          else
          {
            tabhlkMeter.Enabled = false;
            tablblMeter.CssClass = "toptabinactive";
          }
        }

        if (m_tabname == "History")
        {
          tabhlkHistory.NavigateUrl = "javascript:void(null)";
          tabimgHistory.ImageUrl = "../images/tabbutton_down.png";
          tablblHistory.CssClass = "toptabover";
        }
        else
        {
          if (m_mode == "edit")
          {
            tabhlkHistory.NavigateUrl = "equipmenthistory.aspx?equipment=" + m_nvc["equipment"];
            tabhlkHistory.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgHistory','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblHistory','toptabover')");
            tabhlkHistory.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgHistory','../images/tabbutton.png');classChangeover('ucHeader1_tablblHistory','toptabout')");
          }
          else
          {
            tabhlkHistory.Enabled = false;
            tablblHistory.CssClass = "toptabinactive";
          }
        }

        if (m_tabname == "Hierarchy")
        {
          tabhlkHierarchy.NavigateUrl = "javascript:void(null)";
          tabimgHierarchy.ImageUrl = "../images/tabbutton_down.png";
          tablblHierarchy.CssClass = "toptabover";
        }
        else
        {
          if (m_mode == "edit")
          {
            tabhlkHierarchy.NavigateUrl = "equipmenttree.aspx?equipment=" + m_nvc["equipment"];
            tabhlkHierarchy.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgHierarchy','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblHierarchy','toptabover')");
            tabhlkHierarchy.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgHierarchy','../images/tabbutton.png');classChangeover('ucHeader1_tablblHierarchy','toptabout')");
          }
          else
          {
            tabhlkHierarchy.Enabled = false;
            tablblHierarchy.CssClass = "toptabinactive";
          }
        }

        if (m_tabname == "PMList")
        {
          tabhlkPMList.NavigateUrl = "javascript:void(null)";
          tabimgPMList.ImageUrl = "../images/tabbutton_down.png";
          tablblPMList.CssClass = "toptabover";
        }
        else
        {
          if (m_mode == "edit")
          {
            tabhlkPMList.NavigateUrl = "equipmentpmlist.aspx?equipment=" + m_nvc["equipment"];
            tabhlkPMList.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgPMList','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblPMList','toptabover')");
            tabhlkPMList.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgPMList','../images/tabbutton.png');classChangeover('ucHeader1_tablblPMList','toptabout')");
          }
          else
          {
            tabhlkPMList.Enabled = false;
            tablblPMList.CssClass = "toptabinactive";
          }
        }

        if (m_tabname == "Parts List")
        {
          tabhlkPartsList.NavigateUrl = "javascript:void(null)";
          tabimgPartsList.ImageUrl = "../images/tabbutton_down.png";
          tablblPartsList.CssClass = "toptabover";
        }
        else
        {
          if (m_mode == "edit")
          {
            tabhlkPartsList.NavigateUrl = "partslist.aspx?equipment=" + m_nvc["equipment"];
            tabhlkPartsList.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgPartsList','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblPartsList','toptabover')");
            tabhlkPartsList.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgPartsList','../images/tabbutton.png');classChangeover('ucHeader1_tablblPartsList','toptabout')");
          }
          else
          {
            tabhlkPartsList.Enabled = false;
            tablblPartsList.CssClass = "toptabinactive";
          }
        }

        if (!Page.IsPostBack)
        {
          AzzierHeader.InitToolBar(toolbar, "Equipment", ShowToolbarButtonRights);
        }

        litOperation.Text = m_operationlabel;
        AzzierHeader.CreateLinksDropDown(plhLinkDoc, "Equipment", m_equipment, out linksrvr);
        AzzierHeader.CreateReportsDropDown(plhReports, "Equipment");
        AzzierHeader.CreateQueryDropDown(plhQuery, "Equipment");

        defaultprintcounter = AzzierPrintForm.GetDefaultReport("Equipment");

        if (Page.IsPostBack)
        {
          if (Request.Form["__EVENTTARGET"] == "save")
          {
            SaveEquipment();
          }
          if (Request.Form["__EVENTTARGET"] == "lookup")
          {
            LookUpEquipment();
          }
          if (Request.Form["__EVENTTARGET"] == "delete")
          {
            DeleteEquipment();
          }

        }
    }

    protected void DeleteEquipment()
    {
      Equipment e = new Equipment(Session["Login"].ToString(), "Equipment", "Equipment", m_equipment);
      string message = "";
      e.Delete();
      string jscript = "<script type=\"text/javascript\">";
      jscript += "setTimeout(parent.controlpanel.RefreshPanelGridsOnDelete(), 0); document.location.href='equipmentmain.aspx';";
      jscript += "</script>";
      litFrameScript.Text = jscript;
    }

    public void LookUpEquipment()
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        if (CntlPanel != null)
        {
            screen = new AzzierScreen("equipment/equipmentmain.aspx", "MainForm", CntlPanel.Controls);
            screen.LCID = Session.LCID;
            nvc = screen.CollectFormValues("Equipment",true);  // use the view name
            //m_division = nvc["division"];
            Validation v = new Validation();
            string eqptid = nvc["Equipment"].ToString() ?? "";
            if (eqptid.Length > 1)
            {
                if (v.SpecialStrValidator(eqptid))
                {
                    nvc = new NameValueCollection();
                    nvc.Add("Equipment", eqptid);
                }
            }

        }
        else
            nvc = null;
        Equipment obj = new Equipment(Session["Login"].ToString(), "Equipment", "Equipment");  


        string wherestring = "",linqwherestring="";
        obj.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
        string jscript = "<script type=\"text/javascript\">";
        jscript += "var wherestring = '" + Server.UrlEncode(linqwherestring) + "';var obj=parent.controlpanel.document.location.href='equipmentpanel.aspx?showlist=queryresult&wherestring='+wherestring;";
        jscript += "</script>";
        litFrameScript.Text = jscript;
        //string[] equipments = obj.Query(nvc, ref wherestring);
      /*
        string[] equipments = obj.LinqQuery(nvc, ref wherestring,ref linqwherestring);
        string jscript = "<script type=\"text/javascript\">";
        int len = equipments.Length;
        if (len > 0)
        {
            // jscript += "var wherestring = '" + Server.UrlEncode(wherestring) + "'; document.location.href='equipmentmain.aspx?mode=edit&equipment=" + Server.UrlEncode(equipments[0]) + "';var obj=parent.controlpanel.document.location.href='equipmentpanel.aspx?showlist=queryresult&wherestring='+wherestring;";
            jscript += "var wherestring = '" + Server.UrlEncode(linqwherestring) + "'; document.location.href='equipmentmain.aspx?mode=edit&equipment=" + Server.UrlEncode(equipments[0]) + "';var obj=parent.controlpanel.document.location.href='equipmentpanel.aspx?showlist=queryresult&wherestring='+wherestring;";
        }
        else
            jscript += "document.location.href='equipmentmain.aspx';";
        jscript += "</script>";
        litFrameScript.Text = jscript;
       * */
    }

    private void SaveEquipment()
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        TextBox tbx = null;
        string dirtylog = "0";
        if (CntlPanel != null)
        {
            screen = new AzzierScreen("equipment/equipmentmain.aspx", "MainForm", CntlPanel.Controls);
            screen.LCID = Session.LCID;
            nvc = screen.CollectFormValues("Equipment", false);  // use the table name
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

        Equipment obj;
        bool success = false;
        if (m_mode == "new")
        {
            if (nvc["inactive"].ToString() == "1")
              nvc["Status"] = "INACTIVE";
            if (nvc["Status"].ToString().ToUpper() == "INACTIVE")
              nvc["INACTIVE"] = "1";
            obj = new Equipment(Session["Login"].ToString(), "Equipment", "equipment");   // use the table name
            string t1 = nvc["Equipment"];
            success = obj.Create(nvc);

            //if (success)
            //{
            //    //string wonum = nvc["wonum"].ToString().ToUpper();
            //    //string[] wonums = { wonum };
            //}

        }
        else if (m_mode == "duplicate")
        {
            string newequipment, oldequipment;
            if (nvc["inactive"].ToString() == "1")
              nvc["Status"] = "INACTIVE";
            if (nvc["Status"].ToString().ToUpper() == "INACTIVE")
              nvc["INACTIVE"] = "1";
            obj = new Equipment(Session["Login"].ToString(), "Equipment", "equipment");
            HiddenField hid;
            hid = CntlPanel.FindControl("HidOldEquipment") as HiddenField;
            if (hid != null)
            {
                oldequipment = hid.Value;
                newequipment = nvc["equipment"];
                //username = Session["Login"].ToString();

                success = obj.Create(nvc);

                if (success)
                {
                  Specification scopy = new Specification(Session["Login"].ToString(), "Specification", "Counter");
                  scopy.SpecCopy(oldequipment, newequipment, "Equipment");

                  Equipment objeqp = new Equipment(Session["Login"].ToString(), "Equipment", "Equipment");
                  objeqp.MeterCopy(oldequipment, newequipment);
                  objeqp.MeasurementCopy(oldequipment, newequipment);
                }
            }
        }
        else if (m_mode == "edit")
        {
            string equipment = nvc["equipment"];
            obj = new Equipment(Session["Login"].ToString(), "Equipment", "equipment", equipment);   // use the table name
            success = obj.Update(nvc);
        }
        else
            obj = new Equipment(Session["Login"].ToString(), "Equipment","equipment");

        if (!success)
        {
            //string jscript = "<script type=\"text/javascript\">alert(\"" + objWorkorder.ErrorMessage.Replace("'", "\'") + "\"); document.location.href='Womain.aspx?mode=edit&wonum=" + Server.UrlEncode(nvc["wonum"]) + "';</script>";
            //string jscript = "<script>alert(\"" + objWorkorder.ErrorMessage.Replace("\r\n","") + "\"); document.location.href='Womain.aspx?mode=edit&wonum=" + Server.UrlEncode(nvc["wonum"]) + "';</script>";
            string jscript = "<script>alert(\"" + obj.ErrorMessage.Replace("\r\n", "") + "\");</script>";
            litFrameScript.Text = jscript;
        }
        else
        {
            if (m_mode == "new")
                Response.Redirect("equipmentmain.aspx?mode=edit&equipment=" + Server.UrlEncode(nvc["equipment"]));
            else
                if (m_mode == "duplicate")
                    Response.Redirect("equipmentmain.aspx?mode=edit&equipment=" + Server.UrlEncode(nvc["equipment"]));
                else if (m_mode == "edit")
                {
                    //lblErrorMessage.Text = "";
                    //lblErrorMessage.Visible = false;
                    // no more reload after save, increment dirtylog or can't save again
                    tbx.Text = (Convert.ToInt32(dirtylog) + 1).ToString();
                }
        }
    }

    private Boolean ShowToolbarButtonRights(string commandname, NameValueCollection dr)
    {
        if (dr == null)
            return false;
        Boolean show = false;
        m_allowedit = UserRights.AllowEdit(Session["Login"].ToString(), "Equipment", m_division, m_masterrec);
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
            case "autonew":
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
                    if (m_allowedit)
                      show = true;
                  }
                  if (m_mode == "duplicate")
                  {
                    if (dr["urAddNew"].ToString() != "")
                      if (Convert.ToInt32(dr["urAddNew"]) != 0)
                        show = true;
                  }
                }
                break;
            case "duplicate":
                if (m_mode == "edit")
                    if (dr["urAddNew"].ToString() != "")
                        if (Convert.ToInt32(dr["urAddNew"].ToString()) > 0)
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
            case "delete":
                if (m_tabname == "Main" && m_mode == "edit")
                {
                  if (UserRights.AllowDelete(Session["Login"].ToString(),"Equipment",m_division,m_masterrec))
                    show = true;
                }
                break;
          case "modify":
                if (m_tabname == "Main" && m_mode == "edit")
                {
                  if (m_allowedit)
                    show = true;
                }
                break;
          case "addmeter":
                if (m_tabname == "Meter" && m_mode == "edit")
                {
                  if (m_allowedit)
                      show = true;
                }
                break;
            
          case "addmeasurement":
              if (m_tabname == "Measurement" && m_mode == "edit")
              {
                if (m_allowedit)
                  show = true;
              }
              break;
            case "template":
              if (m_tabname == "Specifications")
              {
                if (m_allowedit)
                  show = true;
              }
              break;
            case "downtimereset":
                if (m_mode == "edit")
                  if (m_allowedit)
                    show = true;
                break;
          default:
              break;
        }
        return show;
    }
  
    private void RetrieveMessage()
    {
        SystemMessage msg = new SystemMessage("equipment/equipmentheader.ascx");
        m_msg = msg.GetSystemMessage();
        //msg.SetJsMessage(litMessage);
    }

}