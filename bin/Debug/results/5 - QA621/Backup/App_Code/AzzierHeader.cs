using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.SessionState;
using System.Data;
using System.Data.OleDb;
using System.Xml;
using System.Web.Script.Serialization;
using Telerik.Web.UI;

/// <summary>
/// Summary description for AzzierHeader
/// </summary>
public class AzzierHeader
{
    public AzzierHeader()
    {
    }

    static public bool defaultDelegateFunction(string s, NameValueCollection nvc)
    {
        return true;
    }
    static public void InitToolBar(RadToolBar toolbar, string module, Func<string, NameValueCollection, bool> ShowToolbarButtonRights = null)
    {
        HttpSessionState Session = HttpContext.Current.Session;
        HttpApplicationState Application = HttpContext.Current.Application;
        int iconwidth = 50;
        int screenwidth = Convert.ToInt32(Session["ScreenWidth"].ToString());
        int panelwidth = (int)Math.Floor(screenwidth * 0.66) - 100;
        if (Session["WOFrameMain"] != null)
        {
            panelwidth = (int)Math.Floor(screenwidth * (Convert.ToDecimal(Session["WOFrameMain"].ToString()) / 100)) - 100;
        }
        int totaliconnum = (int)Math.Floor((decimal)panelwidth / iconwidth);

        string connstring = Application["ConnString"].ToString();
        OleDbConnection conn = new OleDbConnection(connstring);
        conn.Open();
        string sql = "SELECT * FROM UserToolbar WHERE UserId='" + Session["Login"].ToString() + "' AND UserModule='" + module + "' ORDER BY orderby";
        OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
        DataTable dt = new DataTable();
        da.Fill(dt);
        da.Dispose();

        if (dt.Rows.Count == 0)
        {
            sql = "SELECT * FROM UserToolbar WHERE UserId='DEFAULT' AND UserModule='" + module + "' ORDER BY orderby";
            da = new OleDbDataAdapter(sql, conn);
            da.Fill(dt);
            da.Dispose();
        }

        if (module.ToLower().Contains("panel"))
            module = module.Remove(module.Length - 5);
        NameValueCollection drRights;
        UserRights right = new UserRights(Session["Login"].ToString(), "UserRights", "Counter");
        drRights = right.GetRights(Session["Login"].ToString(), module);

        int iconnum = 0;
        //toolbar = new RadToolBar();
        //toolbar.Height = Unit.Pixel(64);
        toolbar.Height = Unit.Percentage(100);
        toolbar.OnClientButtonClicking = "toolbarclicked";
        toolbar.AutoPostBack = false;
        //toolbar.ButtonClick += new RadToolBarEventHandler(toolbar_ButtonClick);
        toolbar.Width = Unit.Percentage(100);
        RadToolBarDropDown toolbarcombobox = new RadToolBarDropDown();
        foreach (DataRow dr in dt.Rows)
        {
            string commandname = dr["CommandName"].ToString().ToLower();

            if (commandname != "savequery" && ShowToolbarButtonRights != null)
                if (!ShowToolbarButtonRights(commandname, drRights))
                    continue;

            RadToolBarButton btn = new RadToolBarButton();
            btn.ImagePosition = ToolBarImagePosition.AboveText;
            btn.Text = dr["LabelText"].ToString();
            btn.ToolTip = dr["ToolTip"].ToString();
            btn.BackColor = System.Drawing.Color.Transparent;
            btn.CommandName = commandname;
            btn.Font.Size = 9;
            //setup images            
            btn.ImageUrl = dr["ImageURL"].ToString();
            btn.HoveredImageUrl = dr["HoverUrl"].ToString();
            btn.CausesValidation = false;

            
            if ( iconnum >= totaliconnum - 1 )
            {
                if (iconnum == totaliconnum - 1)
                    toolbar.Items.Add(toolbarcombobox);

                btn.CssClass = "overflow";
                btn.ForeColor = System.Drawing.Color.Black;
                toolbar.Items.Add(btn);
            }
            else
            {
                btn.CssClass = "regular";
                btn.Width = Unit.Pixel(iconwidth);
                btn.ForeColor = System.Drawing.Color.FloralWhite;
                toolbar.Items.Add(btn);
            }

            if (commandname.Contains("timer"))
                btn.CssClass += " timer paneltoolbar";

            iconnum++;
        }
        CreateToolBarDropDown(toolbar, toolbarcombobox, toolbar.Items);
    }

    static protected void CreateToolBarDropDown(RadToolBar toolbar, RadToolBarDropDown toolbarcombobox, RadToolBarItemCollection items)
    {
        bool dropDownExist = false;
        foreach (RadToolBarItem item in items)
        {
            RadToolBarButton tocopy = item as RadToolBarButton;
            if (tocopy == null)
            {
                dropDownExist = true;
                continue;
            }
            else
            {
                RadToolBarButton btn = new RadToolBarButton();
                btn.CssClass = tocopy.CssClass.Contains("regular") ? "DropDownOverflow" : "DropDownRegular";
                if (tocopy.CommandName.Contains("timer"))
                    btn.CssClass += " timer dropdown";
                btn.ImagePosition = tocopy.ImagePosition;
                btn.Text = tocopy.Text;
                btn.ToolTip = tocopy.ToolTip;
                btn.BackColor = tocopy.BackColor;
                btn.CommandName = tocopy.CommandName;
                btn.Font.Size = tocopy.Font.Size;
                btn.ImageUrl = tocopy.ImageUrl;
                btn.HoveredImageUrl = tocopy.HoveredImageUrl;
                btn.CausesValidation = tocopy.CausesValidation;

                toolbarcombobox.Buttons.Add(btn);
            }
        }
        if (!dropDownExist)
        {
            toolbarcombobox.Visible = true;
            toolbar.Items.Add(toolbarcombobox);
        }
    }

    static public bool checkTimerBtn(string commandname)
    {
            string [] hideTimerBtns = {"timerspause","timerstop","timerresume"};
            foreach (string btn in hideTimerBtns)
                if (commandname == btn)
                    return true;
            return false;
    }

    static public void CreateLinksDropDown(PlaceHolder plhLinkDoc, string module, string keyValue, out string linksrvr )
    {
        HttpSessionState Session = HttpContext.Current.Session;

        plhLinkDoc.Controls.Clear();
        LinkDoc links = new LinkDoc();
        linksrvr = links.LinkServer;
        DataTable alllinks = links.RetrieveLinkDoc(module, keyValue);
        DropDownList lkddl = new DropDownList();
        RadComboBox lkrcb = new RadComboBox();
        lkrcb.ID = "cbblinks";
        Label lklbl = new Label();
        lklbl.Text = "Links: ";
        lklbl.Style["top"] = "0x";
        lklbl.Style["Position"] = "relative";

        plhLinkDoc.Controls.Add(lklbl);
        for (int i = 0; i < alllinks.Rows.Count; i++)
        {
            RadComboBoxItem rcbitem = new RadComboBoxItem();
            string linkurl = alllinks.Rows[i]["LinkURL"].ToString();
            string linktype = alllinks.Rows[i]["LinkType"].ToString();
            if (linktype == "http")
            {
                if (linkurl.IndexOf("ftp://") < 0 && linkurl.IndexOf("http://") < 0 && linkurl.IndexOf("https://") < 0 && linkurl.IndexOf(@"news://") < 0)
                    linkurl = "http://" + linkurl;
            }
            linkurl = linktype + "~" + linkurl;

            rcbitem.Text = alllinks.Rows[i]["LinkTitle"].ToString();
            rcbitem.Attributes.Add("onclick", "javascript:openlink('" + linkurl + "','" + alllinks.Rows[i]["SubFolder"].ToString() + "')");
            rcbitem.ToolTip = alllinks.Rows[i]["Description"].ToString();
            lkrcb.Items.Add(rcbitem);

        }
        lkrcb.AllowCustomText = false;

        lkrcb.Style["Top"] = "0px";
        lkrcb.Style["Position"] = "relative";
        lkrcb.Style["height"] = "23px";
        lkrcb.Width = Unit.Percentage(50);
        //plhToolbar.Controls.Add(ddl);
        if (lkrcb.Items.Count > 0)
            lkrcb.EmptyMessage = "...Select...";
        else
            lkrcb.EmptyMessage = "None";
        plhLinkDoc.Controls.Add(lkrcb);
    }


    static public void CreateReportsDropDown(PlaceHolder plhReports, string module)
    {
        HttpSessionState Session = HttpContext.Current.Session;
        plhReports.Controls.Clear();
        ReportBuilder objrpt = new ReportBuilder(Session["Login"].ToString(), "TeroReport", "Counter");
        DataTable allreports = objrpt.GetReportsByModule(module);
        DropDownList rptddl = new DropDownList();
        RadComboBox rptrcb = new RadComboBox();
        Label rptlbl = new Label();
        rptlbl.Text = "Reports: ";
        rptlbl.Style["top"] = "0px";
        rptlbl.Style["Position"] = "relative";

        plhReports.Controls.Add(rptlbl);
        for (int i = 0; i < allreports.Rows.Count; i++)
        {
            RadComboBoxItem rcbitem = new RadComboBoxItem();
            string rptname = allreports.Rows[i]["ReportName"].ToString();
            string rptcounter = allreports.Rows[i]["Counter"].ToString();

            rcbitem.Text = rptname;
            rcbitem.Attributes.Add("onclick", "javascript:openreport('" + rptcounter + "')");
            rcbitem.ToolTip = allreports.Rows[i]["ReportDesc"].ToString();
            rptrcb.Items.Add(rcbitem);

        }
        rptrcb.AllowCustomText = false;

        rptrcb.Style["Top"] = "0px";
        rptrcb.Style["Position"] = "relative";
        rptrcb.Style["height"] = "23px";
        rptrcb.Width = Unit.Percentage(50);
        //rptrcb.OnClientLoad = "OnClientToolBarDropDownLoad";

        if (rptrcb.Items.Count > 0)
            rptrcb.EmptyMessage = "...Select...";
        else
            rptrcb.EmptyMessage = "None";
        plhReports.Controls.Add(rptrcb);

    }

    static public void CreateQueryDropDown(PlaceHolder plhQuery, string module)
    {
        HttpSessionState Session = HttpContext.Current.Session;
        plhQuery.Controls.Clear();
        UserQuery objuserquery = new UserQuery(Session["Login"].ToString(), "UserQuery", "ID");
        DataTable dtquery = objuserquery.GetQueryByModule(module, Session["Login"].ToString());

        Label lblQuery = new Label();
        lblQuery.Text = "Querys: ";
        lblQuery.Style["top"] = "0px";
        lblQuery.Style["Position"] = "relative";
        plhQuery.Controls.Add(lblQuery);

        RadComboBox rcbQuery = new RadComboBox();
        for (int i = 0; i < dtquery.Rows.Count; i++)
        {
            RadComboBoxItem rcbQueryitem = new RadComboBoxItem();
            string queryname = dtquery.Rows[i]["QueryName"].ToString();
            string qid = dtquery.Rows[i]["ID"].ToString();

            rcbQueryitem.Text = queryname;
            rcbQueryitem.Attributes.Add("onclick", "javascript:openquery('" + qid + "')");
            rcbQuery.Items.Add(rcbQueryitem);
        }
        rcbQuery.AllowCustomText = true;
        rcbQuery.Style["Top"] = "0px";
        rcbQuery.Style["Position"] = "relative";
        rcbQuery.Style["height"] = "23px";
        rcbQuery.Width = Unit.Percentage(50);
        if (rcbQuery.Items.Count > 0)
            rcbQuery.EmptyMessage = "...Select...";
        else
            rcbQuery.EmptyMessage = "None";
        plhQuery.Controls.Add(rcbQuery);
    }

}