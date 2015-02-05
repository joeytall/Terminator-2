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

public partial class receiving_receiveheader : System.Web.UI.UserControl
{
    private string m_mode;
    private string m_tabname;
    private string m_operationlabel;
    private NameValueCollection m_nvc;
    private AzzierScreen screen;
    protected string linksrvr = "";
    private int m_AllowEdit;
    protected RadComboBox toolbarcombobox;
    private string m_division;

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
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        RetrieveMessage();
        toolbarcombobox = new RadComboBox();

        //mainmodul_ScrnSize = 260;// Convert.ToInt32(Session["ScreenWidth"].ToString());
        mainmodul_ScrnSize = Convert.ToInt32(Session["ScreenWidth"].ToString());
        mainmodul_ScrnSize = (int)Math.Floor(mainmodul_ScrnSize * 0.67) - 100;
        //mainmodul_ScrnSize = 260;// 500;// 260;// 500;// (int)Math.Floor(mainmodul_ScrnSize * 0.67) - 32;
        decimal t_iconnum = mainmodul_ScrnSize / 50;
        tmp_ScrnIconNum = (int)Math.Floor(t_iconnum);
        //c_005 = false; c_006 = false; c_007 = false; c_008 = false; c_009 = false; c_010 = false; c_011 = false; c_012 = false;

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
            tabhlkMain.NavigateUrl = "receivemain.aspx";
            tabhlkMain.Attributes.Add("onmouseover", "imageChangeover('ucHeader1_tabimgMain','../images/tabbutton_over.png');classChangeover('ucHeader1_tablblMain','toptabover')");
            tabhlkMain.Attributes.Add("onmouseout", "imageChangeover('ucHeader1_tabimgMain','../images/tabbutton.png');classChangeover('ucHeader1_tablblMain','toptabout')");
        }

        if (!Page.IsPostBack)
        {
            AzzierHeader.InitToolBar(toolbar, "Receiving", ShowToolbarButtonRights);
        }

        litOperation.Text = m_operationlabel;
        AzzierHeader.CreateLinksDropDown(plhLinkDoc, "Receiving", "", out linksrvr);
        AzzierHeader.CreateReportsDropDown(plhReports, "Receiving");
        AzzierHeader.CreateQueryDropDown(plhQuery, "Receiving");

      /*
        LinkDoc links = new LinkDoc();
        linksrvr = links.LinkServer;
        DataTable alllinks = links.RetrieveLinkDoc("Inventory", m_itemnum);
        if (alllinks.Rows.Count > 0)
        {
            DropDownList ddl = new DropDownList();
            RadComboBox rcb = new RadComboBox();
            Label lbl = new Label();
            lbl.Text = "Linked Documents: ";
            lbl.Style["top"] = "-5px";
            lbl.Style["Position"] = "relative";

            plhLinkDoc.Controls.Add(lbl);
            for (int i = 0; i < alllinks.Rows.Count; i++)
            {
                //Button button = new Button();
                //ListItem button = new ListItem();
                RadComboBoxItem rcbitem = new RadComboBoxItem();
                string linkurl = alllinks.Rows[i]["LinkURL"].ToString();
                string linktype = alllinks.Rows[i]["LinkType"].ToString();
                if (linktype == "http")
                {
                    if (linkurl.IndexOf("ftp://") < 0 && linkurl.IndexOf("http://") < 0 && linkurl.IndexOf("https://") < 0 && linkurl.IndexOf(@"news://") < 0)
                    {
                        linkurl = "http://" + linkurl;
                    }
                }
                linkurl = linktype + "~" + linkurl;

                rcbitem.Text = alllinks.Rows[i]["LinkTitle"].ToString();
                rcbitem.Attributes.Add("onclick", "javascript:openlink('" + linkurl + "','" + alllinks.Rows[i]["SubFolder"].ToString() + "')");
                //ddl.Items.Add(button);
                rcbitem.ToolTip = alllinks.Rows[i]["Description"].ToString();
                rcb.Items.Add(rcbitem);

            }
            rcb.AllowCustomText = false;

            rcb.Style["Top"] = "-5px";
            rcb.Style["Position"] = "relative";
            rcb.Style["height"] = "23px";
            //plhToolbar.Controls.Add(ddl);
            plhLinkDoc.Controls.Add(rcb);
        }
        ImageButton managelinks = new ImageButton();
        managelinks.ImageUrl = "../images/folder_24.png";
        managelinks.BackColor = System.Drawing.Color.Transparent;
        managelinks.Height = Unit.Pixel(20);
        managelinks.OnClientClick = "managelink();return false;";
        managelinks.Style["Position"] = "relative";
        managelinks.Style["Top"] = "0px";
        plhLinkDoc.Controls.Add(managelinks);
      */
        if (Page.IsPostBack)
        {
            if (Request.Form["__EVENTTARGET"] == "lookup")
            {
                lbnLookUpReceiving_Click(null, null);
            }
            if (Request.Form["__EVENTTARGET"] == "lookupreturn")
            {
              lbnLookUpReturn_Click(null, null);
            }
        }
    }
    protected void toolbarcombobox_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        if (e.Value == "lookup")
          lbnLookUpReceiving_Click(null, null);
        toolbarcombobox.SelectedIndex = 0;
    }

    protected void lbnLookUpReceiving_Click(object sender, EventArgs e)
    {
        NameValueCollection nvc;
        Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
        if (CntlPanel != null)
        {
            screen = new AzzierScreen("receiving/receivemain.aspx", "MainForm", CntlPanel.Controls);
            screen.LCID = Session.LCID;
            nvc = screen.CollectFormValues("v_ReceivingPOLine", true);  // use the view name
            //m_division = nvc["division"];
        }
        else
            nvc = null;

        nvc.Add("StatusCode", ">=200");
        nvc.Add("StatusCode", "<400");

        ModuleoObject obj = new ModuleoObject(Session["Login"].ToString(), "v_ReceivingPOLine", "PONum");   
        string wherestring = "", linqwherestring = "";
        obj.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
        
        string jscript = "<script type=\"text/javascript\">";
        jscript += "var wherestring = '" + Server.UrlEncode(linqwherestring) + "';var obj=parent.controlpanel.document.location.href='receivepanel.aspx?showlist=queryresult&wherestring='+wherestring;";
        jscript += "</script>";
        litFrameScript.Text = jscript;
    }

    protected void lbnLookUpReturn_Click(object sender, EventArgs e)
    {
      NameValueCollection nvc;
      Panel CntlPanel = Page.FindControl("MainControlsPanel") as Panel;
      if (CntlPanel != null)
      {
        screen = new AzzierScreen("receiving/receivemain.aspx", "MainForm", CntlPanel.Controls);
        screen.LCID = Session.LCID;
        nvc = screen.CollectFormValues("v_ReceivingPOLine", true);  // use the view name
        //m_division = nvc["division"];
      }
      else
        nvc = null;

      nvc.Add("StatusCode", ">=200");
      nvc.Add("StatusCode", "<400");

      ModuleoObject obj = new ModuleoObject(Session["Login"].ToString(), "v_ReceivingPOLine", "PONum");
      string wherestring = "", linqwherestring = "";
      obj.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);

      string jscript = "<script type=\"text/javascript\">";
      jscript += "var wherestring = '" + Server.UrlEncode(linqwherestring) + "';var obj=parent.controlpanel.document.location.href='receivepanel.aspx?showlist=queryresult&lookupreturn=return&wherestring='+wherestring;";
      jscript += "</script>";
      litFrameScript.Text = jscript;
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
            case "lookupreturn":
                if (m_mode == "query")
                  show = true;
                break;
            
            default:
                break;
        }
        return show;
    }
    private void RetrieveMessage()
    {
        SystemMessage msg = new SystemMessage("receiving/receiveheader.ascx");
        m_msg = msg.GetSystemMessage();
    }

}