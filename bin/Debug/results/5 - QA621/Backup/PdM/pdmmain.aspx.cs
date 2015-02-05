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

public partial class pdm_pdmmain : System.Web.UI.Page
{
    AzzierScreen screen;
    private string connstring;
    protected string querymode = "";
    protected string m_pdmnum, m_oldpdmnum = "";
    PDM objPDM;
    NameValueCollection nvcPDM;
    protected bool isquery = false;
    protected int statuscode = 0;
    protected NameValueCollection m_msg = new NameValueCollection();
    protected NameValueCollection m_rights;
    protected string t_duplicate;
    protected string m_lastreading = "";
    protected DataClassesDataContext dc = new DataClassesDataContext(AzzierData.LinqConnectstring);

    protected void Page_Init(object sender, EventArgs e)
    {
        RetrieveMessage();
        UserRights.CheckAccess("pdm");

        Session.LCID = Convert.ToInt32(Session["LCID"]);
        if (Request.QueryString["mode"] != null)
        {
            querymode = Request.QueryString["mode"].ToString();
            if (querymode == "")
                querymode = "query";
        }
        else
            querymode = "query";

        if (Request.QueryString["pdmnum"] != null)
        {
            m_pdmnum = Request.QueryString["pdmnum"].ToString();
            m_oldpdmnum = Request.QueryString["pdmnum"].ToString();
            HidOldPDMNum.Value = Request.QueryString["pdmnum"].ToString();

        }
        else
            m_pdmnum = "";

        if (querymode == "query")
            isquery = true;

        if (querymode == "edit")
        {
            objPDM = new PDM(Session["Login"].ToString(), "PDM", "PDMNum", m_pdmnum);

            nvcPDM = objPDM.ModuleData;
            if (nvcPDM["pdmnum"].ToString() == "")
            {
                querymode = "query";
                isquery = true;
                hidMode.Value = querymode;
                m_pdmnum = "";
            }
            UserWorkList list = new UserWorkList();
            list.AddToRecentList(Session["Login"].ToString(), "PDM", m_pdmnum);
        }
        else if (querymode == "duplicate" && m_oldpdmnum.Length > 0)
        {
            objPDM = new PDM(Session["Login"].ToString(), "PDM", "PDMNum", m_oldpdmnum);

            nvcPDM = objPDM.ModuleData;
            UserWorkList list = new UserWorkList();
            //list.AddToRecentList(Session["Login"].ToString(), "Labour", m_oldempid);
            litScript1.Text = "clearoldlocationval()";
        }
        else
        {
            objPDM = new PDM(Session["Login"].ToString(), "PDM", "PDMNum");
            nvcPDM = objPDM.ModuleData;
            statuscode = 1;
        }
        hidMode.Value = querymode;
        connstring = Application["ConnString"].ToString();

        InitScreen();
    }

    private void InitScreen()
    {
        screen = new AzzierScreen("pdm/pdmmain.aspx", "MainForm", MainControlsPanel.Controls, querymode);
        screen.LCID = Session.LCID;
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
                screen.PopulateScreen("pdm", nvcPDM);
                HidDivision.Value = nvcPDM["division"];
                string m_equipID = nvcPDM["equipment"];
                if (m_equipID != "")
                {
                    Equipment objEquipment = new Equipment(Session["Login"].ToString(), "equipment", "equipment", m_equipID);
                    NameValueCollection nvcEQUIP = objEquipment.ModuleData;
                    if (nvcEQUIP != null)
                    {
                        HidMobile.Value = nvcEQUIP["mobileequipment"].ToString();
                    }
                }
            }
            if (querymode == "duplicate")
            {
                NewNumber n = new NewNumber();
                string pmnum = n.GetNextNumber("PDM");
                nvcPDM["PDMNum"] = pmnum;
                screen.PopulateScreen("pdm", nvcPDM);
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
                        if (nvcPDM["inactive"].ToString() == "1")
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
                        if (nvcPDM["openwo"].ToString() == "1")
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

            if (objPDM.ModuleData["MeasurementName"] != "" && objPDM.ModuleData["Equipment"] != "")
            {
                TextBox t = (TextBox)MainControlsPanel.FindControl("txtcurrentreading");
                MeasurementReading m = new MeasurementReading(Session["Login"].ToString(), "MeasurementReading", "Counter");
                m_lastreading = m.GetLastReading("EQUIPMENT", objPDM.ModuleData["Equipment"], objPDM.ModuleData["MeasurementName"]);
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
        ucHeader1.ModuleData = nvcPDM;

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
                TextBox t = MainControlsPanel.FindControl("txtpdmnum") as TextBox;
                if (t != null)
                {
                    if (!Page.IsPostBack)
                    {
                        NewNumber num = new NewNumber();
                        nvcPDM["PDMNum"] = num.GetNextNumber("PDM");
                    }
                }
            }
            screen.GetDefaultValue("PDM", nvcPDM, "PDM", "new");
            screen.PopulateScreen("PDM", nvcPDM);
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

        if (querymode != "query")
        {
            TextBox tbx = MainControlsPanel.FindControl("txtmeasurementname") as TextBox;
            tbx.Attributes.Add("onfocus", "measurementFocus()");
            HyperLink h = MainControlsPanel.FindControl("lkumeasurementname") as HyperLink;
            if (h != null)
            {
                h.NavigateUrl = "javascript:measurementlookup()";
            }
        }
        ucHeader1.OperationLabel = oper;
        SetMeasUnit();
    }


    private void Query()
    {
        NameValueCollection nvc;

        nvc = screen.CollectFormValues("PDM", true);  // use the view name

        Validation v = new Validation();
        string pdmnum = nvc["PDMNum"].ToString() ?? "";
        if (pdmnum.Length > 1)
        {
            if (v.SpecialStrValidator(pdmnum))
            {
                nvc = new NameValueCollection();
                nvc.Add("PDMNum", pdmnum);
            }
        }


        PDM obj = new PDM(Session["Login"].ToString(), "PDM", "PDMNum");   // use the view name
        string wherestring = "", linqwherestring = "";
        obj.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
        string jscript = "<script type=\"text/javascript\">";
        jscript += "var wherestring = '" + Server.UrlEncode(linqwherestring) + "'; var obj=parent.controlpanel.document.location.href='pdmpanel.aspx?showlist=queryresult&wherestring='+wherestring;";
        jscript += "</script>";
        litScript1.Text = jscript;
        //litFrameScript.Text = jscript;
    }

    private void SetMeasUnit()
    {
        TextBox txtmeasurementname = MainControlsPanel.FindControl("txtmeasurementname") as TextBox;
        TextBox txtequipment = MainControlsPanel.FindControl("txtequipment") as TextBox;
        TextBox txtmeasunit = MainControlsPanel.FindControl("txtmeasunit") as TextBox;
        if (txtmeasurementname != null && txtequipment != null)
        {
            if (txtmeasurementname.Text != "" && txtequipment.Text != "")
            {
                if (txtmeasunit != null)
                {
                    v_LastMeasurementReadingDetail unit = new v_LastMeasurementReadingDetail();
                    unit = dc.v_LastMeasurementReadingDetails.Where(x => x.LinkType.ToLower() == "equipment" && x.LinkId.ToLower() == txtequipment.Text.ToLower() && x.MeasurementName.ToLower() == txtmeasurementname.Text.ToLower()).FirstOrDefault();
                    if (unit != null)
                    {
                        txtmeasunit.Text = unit.MeasUnit;
                    }
                }
            }
        }
    }

    private void RetrieveMessage()
    {
        SystemMessage msg = new SystemMessage("pdm/pdmmain.aspx");
        m_msg = msg.GetSystemMessage();
        msg.SetJsMessage(litMessage);
    }

}