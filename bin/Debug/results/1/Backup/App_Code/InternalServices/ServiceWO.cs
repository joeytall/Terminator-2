using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Web.Services;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Web.Script.Services;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.ServiceModel;
using Telerik.Web.UI;
using System.Runtime.Serialization;



[ServiceContract(Namespace = "")]
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class ServiceWO
{
    protected DataClassesDataContext dc = new DataClassesDataContext(AzzierData.LinqConnectstring);
    //protected System.Web.HttpContext.Current.Session.LCID=2057;

    // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
    // To create an operation that returns XML,
    //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
    //     and include the following line in the operation body:
    //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";


    /// <summary>
    /// Save workorder
    /// </summary>
    /// <param name="xmlnvc"></param>
    /// <returns></returns>
    [OperationContract]
    [WebGet]
    public string SaveWorkorder(string xmlnvc)
    {
        string returnStr = "", oldprocnum = "", newprocnum = "", userid = "";

        HttpContext context = HttpContext.Current;

        if (context != null)
        {
            if (context.Request.Headers["hidprocnum"] != null)
                oldprocnum = context.Request.Headers["hidprocnum"];
            else oldprocnum = "";
        }

        object o = context.Session["Login"];
        if (o == null)
        {
            returnStr = "FALSE^Not authorized.";
            return returnStr;
        }
        else
        {
            userid = o.ToString();
            if (!ValidateUser(userid))
            {
                returnStr = "FALSE^Security mismatched.";
                return returnStr;
            }
        }

        bool success = false;
        NameValueCollection nvc;
        nvc = new NameValueCollection();

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xmlnvc);

        XmlNode root = doc.DocumentElement.FirstChild;

        XmlNodeList wolist = root.ChildNodes;

        for (int i = 0; i < wolist.Count; i++)
        {
            string name = wolist[i].Name;
            string val = wolist[i].InnerText;
            nvc.Add(name, val);
        }

        Workorder objWorkorder;

        string wo = nvc["wonum"];
        objWorkorder = new Workorder(o.ToString(), "Workorder", "WoNum", wo);
        success = objWorkorder.Update(nvc);
        if (success)
        {
            //string newprocnum;
            if (nvc["procnum"] == null)
                newprocnum = "";
            else newprocnum = nvc["procnum"].ToString();

            //string newprocnum = nvc["procnum"];
            //if (newprocnum == null)
            //    newprocnum = "";

            if ((newprocnum != "") && (newprocnum != oldprocnum))
            {
                Labour l = new Labour(userid, "WOLabour", "Counter");
                l.DeleteAllLabour("workorder", nvc["wonum"], 1);
                l.CopyLabour("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                Service s = new Service(userid, "WOService", "Counter");
                s.DeleteAllService("workorder", nvc["wonum"], 1);
                s.CopyService("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                Material m = new Material(userid, "WOMaterial", "Counter");
                m.DeleteAllMaterial("workorder", nvc["wonum"], 1);
                m.CopyMaterial("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                Tool t = new Tool(userid, "WOTools", "Counter");
                t.DeleteAllTools("workorder", nvc["wonum"], 1);
                t.CopyTools("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                Task task = new Task(userid, "WOTasks", "Counter");
                task.DeleteAllTask("workorder", nvc["wonum"], 1);
                task.CopyTasks("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                CostAccount ca = new CostAccount(userid, "CostAccounts", "Counter");
                ca.CopyAccounts("procedure", nvc["procnum"], "workorder", nvc["wonum"]);

            }
            returnStr = "TRUE^" + nvc["wonum"];
        }
        else
        {
            string err = "FALSE^" + objWorkorder.ErrorMessage.Replace("\r\n", "");
            returnStr = err;
        }

        return returnStr;
    }

    [OperationContract]
    [WebGet]
    public string SaveWorkorderStatus(string xmlnvc,string statuscode, string status, string remark)
    {
        string returnStr = "", oldprocnum = "", newprocnum = "", userid = "";

        HttpContext context = HttpContext.Current;

        if (context != null)
        {
            if (context.Request.Headers["hidprocnum"] != null)
                oldprocnum = context.Request.Headers["hidprocnum"];
            else oldprocnum = "";
        }

        object o = context.Session["Login"];
        if (o == null)
        {
            returnStr = "FALSE^Not authorized.";
            return returnStr;
        }
        else
        {
            userid = o.ToString();
            if (!ValidateUser(userid))
            {
                returnStr = "FALSE^Security mismatched.";
                return returnStr;
            }
        }

        bool success = false;
        NameValueCollection nvc;
        nvc = new NameValueCollection();

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xmlnvc);

        XmlNode root = doc.DocumentElement.FirstChild;

        XmlNodeList wolist = root.ChildNodes;

        for (int i = 0; i < wolist.Count; i++)
        {
            string name = wolist[i].Name;
            string val = wolist[i].InnerText;
            nvc.Add(name, val);
        }

        Workorder objWorkorder;

        string wo = nvc["wonum"];
        objWorkorder = new Workorder(o.ToString(), "Workorder", "WoNum", wo);

        nvc.Set("Status", status);
        nvc.Set("StatusCode", statuscode);
        nvc.Set("Remark", remark);

        success = objWorkorder.UpdateStatus(nvc);
        if (success)
        {
            //string newprocnum;
            if (nvc["procnum"] == null)
                newprocnum = "";
            else newprocnum = nvc["procnum"].ToString();

            //string newprocnum = nvc["procnum"];
            //if (newprocnum == null)
            //    newprocnum = "";

            if ((newprocnum != "") && (newprocnum != oldprocnum))
            {
                Labour l = new Labour(userid, "WOLabour", "Counter");
                l.DeleteAllLabour("workorder", nvc["wonum"], 1);
                l.CopyLabour("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                Service s = new Service(userid, "WOService", "Counter");
                s.DeleteAllService("workorder", nvc["wonum"], 1);
                s.CopyService("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                Material m = new Material(userid, "WOMaterial", "Counter");
                m.DeleteAllMaterial("workorder", nvc["wonum"], 1);
                m.CopyMaterial("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                Tool t = new Tool(userid, "WOTools", "Counter");
                t.DeleteAllTools("workorder", nvc["wonum"], 1);
                t.CopyTools("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                Task task = new Task(userid, "WOTasks", "Counter");
                task.DeleteAllTask("workorder", nvc["wonum"], 1);
                task.CopyTasks("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                CostAccount ca = new CostAccount(userid, "CostAccounts", "Counter");
                ca.CopyAccounts("procedure", nvc["procnum"], "workorder", nvc["wonum"]);

            }

            //NameValueCollection nvcstatus = new NameValueCollection();
            //nvcstatus.Add("status", status);
            //nvcstatus.Add("statuscode", statuscode);
            //nvcstatus.Add("remark", remark);
            //if (Convert.ToInt16(statuscode) < 300 && Convert.ToInt16(statuscode) > 0)
            //    nvcstatus.Add("CompDate", "");
            //objWorkorder.UpdateStatus(nvcstatus);

            returnStr = "TRUE^" + nvc["wonum"];
        }
        else
        {
            string err = "FALSE^" + objWorkorder.ErrorMessage.Replace("\r\n", "");
            returnStr = err;
        }

        return returnStr;
    }

    /// <summary>
    /// Create new workorder
    /// </summary>
    /// <param name="userid"></param>
    /// <param name="xmlnvc"></param>
    /// <returns></returns>
    [OperationContract]
    [WebGet]
    public string CreateWorkorder(string userid, string xmlnvc, string oldwonum = "", int estimate = 1)
    {
        string returnStr = "";
        bool success = false;
        userid = userid.ToUpper();
        NameValueCollection nvc;
        nvc = new NameValueCollection();

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xmlnvc);

        XmlNode root = doc.DocumentElement.FirstChild;

        XmlNodeList wolist = root.ChildNodes;

        for (int i = 0; i < wolist.Count; i++)
        {
            string name = wolist[i].Name;
            string val = wolist[i].InnerText;
            nvc.Add(name, val);
        }

        Workorder objWorkorder;

        objWorkorder = new Workorder(userid, "Workorder", "WoNum");
        success = objWorkorder.Create(nvc);

        if (success)
        {
            if (oldwonum == "" || oldwonum == null)
            {
                if (nvc["procnum"] != "")
                {
                    Labour l = new Labour(userid, "WOLabour", "Counter");
                    l.CopyLabour("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                    Service s = new Service(userid, "WOService", "Counter");
                    s.CopyService("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                    Material m = new Material(userid, "WOMaterial", "Counter");
                    m.CopyMaterial("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                    Tool t = new Tool(userid, "WOTools", "Counter");
                    t.CopyTools("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                    Task task = new Task(userid, "WOTasks", "Counter");
                    task.CopyTasks("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                    CostAccount ca = new CostAccount(userid, "CostAccounts", "Counter");
                    ca.CopyAccounts("procedure", nvc["procnum"], "workorder", nvc["wonum"]);
                }

            }
            else  //duplicate
            {
                Labour l = new Labour(userid, "WOLabour", "Counter");
                l.CopyLabour("workorder", oldwonum, "workorder", nvc["wonum"], estimate);
                Service s = new Service(userid, "WOService", "Counter");
                s.CopyService("workorder", oldwonum, "workorder", nvc["wonum"], estimate);
                Material m = new Material(userid, "WOMaterial", "Counter");
                m.CopyMaterial("workorder", oldwonum, "workorder", nvc["wonum"], estimate);
                Tool t = new Tool(userid, "WOTools", "Counter");
                t.CopyTools("workorder", oldwonum, "workorder", nvc["wonum"], estimate);
                Task task = new Task(userid, "WOTasks", "Counter");
                task.CopyTasks("workorder", oldwonum, "workorder", nvc["wonum"], estimate);
                CostAccount ca = new CostAccount(userid, "CostAccounts", "Counter");
                ca.CopyAccounts("workorder",oldwonum, "workorder", nvc["wonum"]);
            }
            returnStr = nvc["wonum"];
        }

        return returnStr;
    }


    [OperationContract]
    [WebGet]
    public string GetTotals(string wonum)
    {
        string result = "";
        string userid = "";
        HttpContext context = HttpContext.Current;
        object o = context.Session["Login"];
        if (o == null)
        {
            result = "Not authorized.";
            return result;
        }
        else
        {
            userid = o.ToString();
            if (!ValidateUser(userid))
            {
                result = "Security mismatched.";
                return result;
            }
        }

        Workorder w = new Workorder(userid, "Workorder", "WONum", wonum);
        result = w.ModuleData["EstHours"] + "^" + w.ModuleData["EstLabor"] + "^" + w.ModuleData["EstMaterial"] + "^" + w.ModuleData["EstTools"] + "^" + w.ModuleData["EstService"] + "^"
                + w.ModuleData["ActHours"] + "^" + w.ModuleData["ActLabor"] + "^" + w.ModuleData["ActMaterial"] + "^" + w.ModuleData["ActTools"] + "^" + w.ModuleData["ActService"];

        return result;
    }

    /// <summary>
    /// Check the login user is same with session["Login"]
    /// </summary>
    /// <param name="userid"></param>
    /// <returns></returns>
    private static bool ValidateUser(string userid)
    {
        HttpContext context = HttpContext.Current;
        if (context != null)
        {
            string headerid = context.Request.Headers["userid"];
            if (headerid.ToLower() != userid.ToLower())
            {
                return false;
            }
        }
        else { return false; }

        return true;
    }

    /// <summary>
    /// XML: work order and meter data
    /// </summary>
    /// <param name="xmlnvc"></param>
    /// <returns></returns>
    [OperationContract]
    [WebGet]
    public string CompleteWO(string xmlnvc)
    {
        string returnStr = "";
        string userid = "";
        bool success = false;
        NameValueCollection nvcWO, nvcMeter;

        HttpContext context = HttpContext.Current;

        object o = context.Session["Login"];
        if (o == null)
        {
            returnStr = "FALSE^Not authorized.";
            return returnStr;
        }
        else
        {
            userid = o.ToString();
            if (!ValidateUser(userid))
            {
                returnStr = "FALSE^Security mismatched.";
                return returnStr;
            }
        }

        nvcWO = new NameValueCollection();

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xmlnvc);

        XmlNode root = doc.DocumentElement.FirstChild;

        XmlNodeList wolist = root.ChildNodes;

        for (int i = 0; i < wolist.Count; i++)
        {
            string name = wolist[i].Name;
            string val = wolist[i].InnerText;
            nvcWO.Add(name, val);
        }

        nvcMeter = new NameValueCollection();
        XmlNodeList nodelist = doc.DocumentElement.ChildNodes;
        if (nodelist.Count > 1)
        {
            XmlNodeList metlist = nodelist[1].ChildNodes;
            for (int i = 0; i < metlist.Count; i++)
            {
                string name = metlist[i].Name;
                string val = metlist[i].InnerText;
                nvcMeter.Add(name, val);
            }
        }

        nvcMeter.Add("WoNum", nvcWO["WoNum"]);
        nvcMeter.Add("meterdate", nvcWO["compdate"]);
        nvcMeter.Add("operator", userid);

        ModuleoObject Reading = new ModuleoObject(userid, "MeterRead", "Counter");

        if (!Reading.Create(nvcMeter))
        {
            returnStr = "FALSE^" + Reading.ErrorMessage;
            return returnStr;
        }

        Workorder objWorkorder;

        string wo = nvcWO["wonum"];
        objWorkorder = new Workorder(o.ToString(), "Workorder", "WoNum", wo);
        success = objWorkorder.Update(nvcWO);
        if (success)
        {
            returnStr = "TRUE^" + nvcWO["wonum"];
        }
        else
        {
            string err = "FALSE^" + objWorkorder.ErrorMessage.Replace("\r\n", "");
            returnStr = err;
        }

        return returnStr;
    }

    [OperationContract]
    [WebGet]
    public string UpdateWRForCancelledWorkOrder(string wonum, string disconnectlist,string connectlist)
    {
      string returnStr = "";
      string userid = "";
      bool success = false;

      HttpContext context = HttpContext.Current;

      object o = context.Session["Login"];
      if (o == null)
      {
        returnStr = "FALSE^Not authorized.";
        return returnStr;
      }
      else
      {
        userid = o.ToString();
        if (!ValidateUser(userid))
        {
          returnStr = "FALSE^Security mismatched.";
          return returnStr;
        }
      }

      WorkRequest objwr = new WorkRequest(userid, "WorkRequest", "WrNum");
      objwr.UpdateAfterWorkorderCancelled(wonum, disconnectlist, connectlist);
      

      return "OK";
    }

    [OperationContract]
    [WebGet]
    public ResultWOLinq QueryResultWO(int startRowIndex, int maximumRows, string sortExpression, string filterExpression, string jsonData = "{}")
    {
        ResultWOLinq result = new ResultWOLinq();

        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

        string searchcriteria;
        List<string> strlist = new List<string>();

        searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";

        NameValueCollection nvc = AzzierData.DeserializeJSON(jsonData);
        object empid = HttpContext.Current.Session["LogIn"];
        string userid = (empid != null) ? empid.ToString() : "";
        Workorder objWorkorder = new Workorder(userid, "workorder", "Wonum");
        string wherestring = "", linqwherestring = "";
        if (jsonData.Contains("queryID"))
          nvc = UserQuery.CreateNameValueCollection(nvc["queryID"]);
        objWorkorder.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
        searchcriteria = linqwherestring;

        string login_user = "";
        if (System.Web.HttpContext.Current.Session["Login"] != null)
        {
            login_user = HttpContext.Current.Session["Login"].ToString();
            if (login_user == "")
            {
                login_user = "";
                result.Data = null;
                result.Count = 0;
                return result;
            }
        }
        else
        {
            login_user = "";
            result.Data = null;
            result.Count = 0;
            return result;
        }

        if (searchcriteria.Length > 1)
        {
            searchcriteria = searchcriteria.Replace("'", "\"");

            if (filterExpression == null || filterExpression.Length < 1)
            {
                filterExpression = "";
            }

            filterExpression = filterExpression != "" ? filterExpression + " and " : "";
            filterExpression = filterExpression + searchcriteria;
        }

        var query = from w in dc.WOLinqs select w;

        //List<POLinq> pl = new List<POLinq>();
        //pl.Add(new POLinq {PoNum= });
        //pl.
        //POLinq pl = new POLinq();
        //pl.PoNum = pl.PoNum.n

        GridLinqBindingData<WOLinq> resultData = RadGrid.GetBindingData<WOLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        //GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "WOLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = resultData.Data.OfType<WOLinq>().ToList();
        result.Count = resultData.Count;

        return result;
    }

    [OperationContract]
    public ResultWOLinq SearchDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        ResultWOLinq result = new ResultWOLinq();

        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

        string searchcriteria;
        List<string> strlist = new List<string>();

        searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";

        string login_user = "";
        if (System.Web.HttpContext.Current.Session["Login"] != null)
        {
            login_user = HttpContext.Current.Session["Login"].ToString();
            if (login_user == "")
            {
                login_user = "";
                result.Data = null;
                result.Count = 0;
                return result;
            }
        }
        else
        {
            login_user = "";
            result.Data = null;
            result.Count = 0;
            return result;
        }

        if (searchcriteria.Length > 1)
        {
            searchcriteria = searchcriteria.Replace("'", "\"");

            if (filterExpression == null || filterExpression.Length < 1)
            {
                filterExpression = "";
            }

            filterExpression = filterExpression != "" ? filterExpression + " and " : "";
            filterExpression = filterExpression + searchcriteria;
        }

        var query = from w in dc.WOLinqs select w;

        //List<POLinq> pl = new List<POLinq>();
        //pl.Add(new POLinq {PoNum= });
        //pl.
        //POLinq pl = new POLinq();
        //pl.PoNum = pl.PoNum.n

        GridLinqBindingData<WOLinq> resultData = RadGrid.GetBindingData<WOLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        //GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "WOLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = resultData.Data.OfType<WOLinq>().ToList();
        result.Count = resultData.Count;

        return result;
    }

    [OperationContract]
    public ResultTaskLinq LookupTasks(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
      ResultTaskLinq result = new ResultTaskLinq();

      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

      string searchcriteria;
      List<string> strlist = new List<string>();

      searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";

      string login_user = "";
      if (System.Web.HttpContext.Current.Session["Login"] != null)
      {
        login_user = HttpContext.Current.Session["Login"].ToString();
        if (login_user == "")
        {
          login_user = "";
          result.Data = null;
          result.Count = 0;
          return result;
        }
      }
      else
      {
        login_user = "";
        result.Data = null;
        result.Count = 0;
        return result;
      }

      if (searchcriteria.Length > 1)
      {
        searchcriteria = searchcriteria.Replace("'", "\"");

        if (filterExpression == null || filterExpression.Length < 1)
        {
          filterExpression = "";
        }

        filterExpression = filterExpression != "" ? filterExpression + " and " : "";
        filterExpression = filterExpression + searchcriteria;
      }

      var query = from w in dc.WOTaskLinqs select w;

      //List<POLinq> pl = new List<POLinq>();
      //pl.Add(new POLinq {PoNum= });
      //pl.
      //POLinq pl = new POLinq();
      //pl.PoNum = pl.PoNum.n

      GridLinqBindingData<WOTaskLinq> resultData = RadGrid.GetBindingData<WOTaskLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

      //GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "WOLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
      result.Data = resultData.Data.OfType<WOTaskLinq>().ToList();
      result.Count = resultData.Count;

      return result;
    }


    [OperationContract]
    [WebGet]
    public string GetWORate(string wonum)
    {
        string worate="";
        WOLinq woobj = dc.WOLinqs.Where(x => x.WoNum.ToLower() == wonum.ToLower()).FirstOrDefault();
        WorkTypeLinq wtobj = new WorkTypeLinq();
        if (woobj != null)
        {
            wtobj = dc.WorkTypeLinqs.Where(x => x.WOType.ToLower() == woobj.WoType.ToLower()).FirstOrDefault();
            if (wtobj != null)
            {
                worate = (Math.Round(wtobj.Rate*100,2)).ToString();
            }
        }
        return worate;
    }

    [OperationContract]
    public ResultWOTypeLinq GetWorkTypeList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
      ResultWOTypeLinq result = new ResultWOTypeLinq();

      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

      string searchcriteria;
      List<string> strlist = new List<string>();

      searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";

      string login_user = "";
      if (System.Web.HttpContext.Current.Session["Login"] != null)
      {
        login_user = HttpContext.Current.Session["Login"].ToString();
        if (login_user == "")
        {
          login_user = "";
          result.Data = null;
          result.Count = 0;
          return result;
        }
      }
      else
      {
        login_user = "";
        result.Data = null;
        result.Count = 0;
        return result;
      }

      if (searchcriteria.Length > 1)
      {
        searchcriteria = searchcriteria.Replace("'", "\"");

        if (filterExpression == null || filterExpression.Length < 1)
        {
          filterExpression = "";
        }

        filterExpression = filterExpression != "" ? filterExpression + " and " : "";
        filterExpression = filterExpression + searchcriteria;
      }

      var query = from w in dc.WorkTypeLinqs select w;

      //List<POLinq> pl = new List<POLinq>();
      //pl.Add(new POLinq {PoNum= });
      //pl.
      //POLinq pl = new POLinq();
      //pl.PoNum = pl.PoNum.n

      GridLinqBindingData<WorkTypeLinq> resultData = RadGrid.GetBindingData<WorkTypeLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

      //GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "WOLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
      result.Data = resultData.Data.OfType<WorkTypeLinq>().ToList();
      result.Count = resultData.Count;

      return result;
    }

    [OperationContract]
    [WebGet]
    public string WOCalendarLookUp(string xml, string year, string month)
    {
      string login_user = "";
      if (System.Web.HttpContext.Current.Session["Login"] != null)
        login_user = HttpContext.Current.Session["Login"].ToString();
      else
        return "";
      Workorder objwo = new Workorder(login_user);
      DataTable dt = objwo.CalendarLookUp(xml, year, month);
      System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
      List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
      Dictionary<string, object> row;
      foreach (DataRow dr in dt.Rows)
      {
          row = new Dictionary<string, object>();
          foreach (DataColumn col in dt.Columns)
          {
              row.Add(col.ColumnName, dr[col]);
          }
          rows.Add(row);
      }
      return serializer.Serialize(rows);

    }

    [OperationContract]
    public ResultCloseRemarkLinq GetCompRemarkList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
      ResultCloseRemarkLinq result = new ResultCloseRemarkLinq();

      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

      string searchcriteria;
      List<string> strlist = new List<string>();

      searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";

      string login_user = "";
      if (System.Web.HttpContext.Current.Session["Login"] != null)
      {
        login_user = HttpContext.Current.Session["Login"].ToString();
        if (login_user == "")
        {
          login_user = "";
          result.Data = null;
          result.Count = 0;
          return result;
        }
      }
      else
      {
        login_user = "";
        result.Data = null;
        result.Count = 0;
        return result;
      }

      if (searchcriteria.Length > 1)
      {
        searchcriteria = searchcriteria.Replace("'", "\"");

        if (filterExpression == null || filterExpression.Length < 1)
        {
          filterExpression = "";
        }

        filterExpression = filterExpression != "" ? filterExpression + " and " : "";
        filterExpression = filterExpression + searchcriteria;
      }

      var query = from w in dc.CloseRemarkLinqs select w;

      //List<POLinq> pl = new List<POLinq>();
      //pl.Add(new POLinq {PoNum= });
      //pl.
      //POLinq pl = new POLinq();
      //pl.PoNum = pl.PoNum.n

      GridLinqBindingData<CloseRemarkLinq> resultData = RadGrid.GetBindingData<CloseRemarkLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

      //GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "WOLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
      result.Data = resultData.Data.OfType<CloseRemarkLinq>().ToList();
      result.Count = resultData.Count;

      return result;
    }


    [OperationContract]
    public ResultWOLinq WorkListDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        ResultWOLinq result = new ResultWOLinq();
        
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        string login_user = "";
        if (System.Web.HttpContext.Current.Session["Login"] != null)
        {
            login_user = HttpContext.Current.Session["Login"].ToString();
            if (login_user == "")
            {
                login_user = "";
                result.Data = null;
                result.Count = 0;
                return result;
            }
        }
        else
        {
            login_user = "";
            result.Data = null;
            result.Count = 0;
            return result;
        }

        var query = from u in dc.UserWorkListLinqs
                    join w in dc.WOLinqs on u.LinkId equals w.WoNum
                    where w.Inactive == 0 && u.LinkModule.ToLower() == "workorder" && u.LinkType == "worklist" && u.UserId.ToLower() == login_user.ToLower()
                    orderby w.WoNum
                    select w;

        GridLinqBindingData<WOLinq> data = RadGrid.GetBindingData<WOLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = data.Data.ToList<WOLinq>();
        result.Count = data.Count;

        return result;
    }


    [OperationContract]
    [WebGet]
    public string AddtoWorkList(string paramXML)
    {
        string retstr = "SUCCESS.";

        //XmlDocument doc = new XmlDocument();
        //doc.LoadXml(paramXML);

        string login_user = "";
        if (System.Web.HttpContext.Current.Session["Login"] != null)
        {
            login_user = HttpContext.Current.Session["Login"].ToString();
            if (login_user == "")
            {
                return "ERROR";
            }
        }
        else
        {
            return "ERROR";
        }

        XDocument doc = XDocument.Parse(paramXML);
        var idresult = from id in doc.Descendants("IDS")
                       where !(from c in dc.UserWorkListLinqs
                               where c.LinkModule.ToLower() == "workorder" && c.LinkType.ToLower() == "worklist" && c.UserId.ToLower() == login_user.ToLower()
                               select c.LinkId).Contains(id.Value)
                       select id.Value;

        var ins = from id in dc.WOLinqs
                  where idresult.ToList().Contains(id.WoNum)
                  select new
                  {
                      LinkId = id.WoNum,
                      UserId = login_user,
                      LinkModule = "workorder",
                      LinkType = "worklist",
                      TransDate = DateTime.Now
                  };

        List<UserWorkListLinq> worklist = new List<UserWorkListLinq>();
        foreach (var da in ins)
        {
            worklist.Add(new UserWorkListLinq()
            {
                LinkId = da.LinkId,
                LinkType = da.LinkType,
                LinkModule = da.LinkModule,
                UserId = da.UserId,
                TransDate = da.TransDate
            });
        }

        try
        {
            dc.UserWorkListLinqs.InsertAllOnSubmit(worklist);
            dc.SubmitChanges();
        }
        catch (Exception e)
        {
            retstr = e.Message;
        }

        return retstr;
    }

    [OperationContract]
    [WebGet]
    public string DelWorkList(string paramXML)
    {
        string retstr = "SUCCESS.";

        string login_user = "";
        if (System.Web.HttpContext.Current.Session["Login"] != null)
        {
            login_user = HttpContext.Current.Session["Login"].ToString();
            if (login_user == "")
            {
                return "ERROR";
            }
        }
        else
        {
            return "ERROR";
        }

        XDocument doc = XDocument.Parse(paramXML);

        var idresult = from id in doc.Descendants("IDS") select id.Value;

        string module = doc.Root.Elements("LinkModule").First().Value.ToLower();

        try
        {
                var result = from userwl in dc.UserWorkListLinqs
                             where idresult.ToList().Contains(userwl.LinkId) && userwl.LinkModule.ToLower() == module &&
                             userwl.LinkType.ToLower() == "worklist" && userwl.UserId.ToLower() == login_user.ToLower()
                             select userwl;
                dc.UserWorkListLinqs.DeleteAllOnSubmit(result);
                dc.SubmitChanges();
        }
        catch
        {
            retstr = "ERROR.";
        }

        return retstr;
    }

    [OperationContract]
    public RecentworklistResult RecentListDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        RecentworklistResult result = new RecentworklistResult();

        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

        string login_user = "";
        if (System.Web.HttpContext.Current.Session["Login"] != null)
        {
            login_user = HttpContext.Current.Session["Login"].ToString();
        }

        var qq = from v in dc.v_WORecentlists
                 where v.UserId.ToLower() == login_user.ToLower()
                 orderby v.AccessDate descending
                 select v;

        GridLinqBindingData<v_WORecentlist> resultData = RadGrid.GetBindingData<v_WORecentlist>(qq, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = resultData.Data.ToList<v_WORecentlist>();
        result.Count = resultData.Count;

        return result;
    }

    [OperationContract]
    [WebGet]
    public string  CopyTasks(string wonum, string counterlist, string transdate)
    {
      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

      string login_user = "";
        if (System.Web.HttpContext.Current.Session["Login"] != null)
        {
            login_user = HttpContext.Current.Session["Login"].ToString();
            if (login_user == "")
            {
                return "ERROR";
            }
        }
        else
        {
            return "ERROR";
        }
        if (Workorder.AllowResourceChange(login_user, wonum, "0"))
        {
          Task objtask = new Task(login_user, "WOTasks", "Counter");

          objtask.CopyEstimateToActual(counterlist, transdate);
          return "OK";
        }
        else
        {
          return "Workorder status does not allow copy resources.";
        }
      
    }

    [OperationContract]
    [WebGet]
    public string CopyLabour(string wonum, string counterlist, string transdate)
    {
      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

      string login_user = "";
      if (System.Web.HttpContext.Current.Session["Login"] != null)
      {
        login_user = HttpContext.Current.Session["Login"].ToString();
        if (login_user == "")
        {
          return "ERROR";
        }
      }
      else
      {
        return "ERROR";
      }
      if (Workorder.AllowResourceChange(login_user, wonum, "0"))
      {
        Labour obj = new Labour(login_user, "WOLabour", "Counter",wonum,1);

        obj.CopyEstimateToActual(counterlist, transdate);
        return "OK";
      }
      else
      {
        return "Workorder status does not allow copy resources.";
      }

    }

    [OperationContract]
    [WebGet]
    public string CopyService(string wonum, string counterlist, string transdate)
    {
      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

      string login_user = "";
      if (System.Web.HttpContext.Current.Session["Login"] != null)
      {
        login_user = HttpContext.Current.Session["Login"].ToString();
        if (login_user == "")
        {
          return "ERROR";
        }
      }
      else
      {
        return "ERROR";
      }
      if (Workorder.AllowResourceChange(login_user, wonum, "0"))
      {
        Service obj = new Service(login_user, "WOService", "Counter",wonum,1);

        obj.CopyEstimateToActual(counterlist, transdate);
        return "OK";
      }
      else
      {
        return "Workorder status does not allow copy resources.";
      }

    }

    [OperationContract]
    [WebGet]
    public string CopyTools(string wonum, string counterlist, string transdate)
    {
      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

      string login_user = "";
      if (System.Web.HttpContext.Current.Session["Login"] != null)
      {
        login_user = HttpContext.Current.Session["Login"].ToString();
        if (login_user == "")
        {
          return "ERROR";
        }
      }
      else
      {
        return "ERROR";
      }
      if (Workorder.AllowResourceChange(login_user, wonum, "0"))
      {
        Tool obj = new Tool(login_user, "WOTools", "Counter",wonum,1);

        obj.CopyEstimateToActual(counterlist, transdate);
        return "OK";
      }
      else
      {
        return "Workorder status does not allow copy resources.";
      }

    }

    [OperationContract]
    [WebGet]
    public string CopyMaterial(string wonum, string counterlist, string transdate)
    {
      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

      string login_user = "";
      if (System.Web.HttpContext.Current.Session["Login"] != null)
      {
        login_user = HttpContext.Current.Session["Login"].ToString();
        if (login_user == "")
        {
          return "ERROR";
        }
      }
      else
      {
        return "ERROR";
      }
      if (Workorder.AllowResourceChange(login_user, wonum, "0"))
      {
        Material obj = new Material(login_user, "WOMaterial", "Counter",wonum,1);

        obj.CopyEstimateToActual(counterlist, transdate);
        return "OK";
      }
      else
      {
        return "Workorder status does not allow copy resources.";
      }

    }

    [OperationContract]
    public ResultFailureCodeLinq GetFailureCodeList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        ResultFailureCodeLinq result = new ResultFailureCodeLinq();

        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

        string searchcriteria;
        List<string> strlist = new List<string>();

        searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";

        string login_user = "";
        if (System.Web.HttpContext.Current.Session["Login"] != null)
        {
            login_user = HttpContext.Current.Session["Login"].ToString();
            if (login_user == "")
            {
                login_user = "";
                result.Data = null;
                result.Count = 0;
                return result;
            }
        }
        else
        {
            login_user = "";
            result.Data = null;
            result.Count = 0;
            return result;
        }

        if (searchcriteria.Length > 1)
        {
            searchcriteria = searchcriteria.Replace("'", "\"");

            if (filterExpression == null || filterExpression.Length < 1)
            {
                filterExpression = "";
            }

            filterExpression = filterExpression != "" ? filterExpression + " and " : "";
            filterExpression = filterExpression + searchcriteria;
        }

        var query = from w in dc.FailureCodeLinqs select w;

        GridLinqBindingData<FailureCodeLinq> resultData = RadGrid.GetBindingData<FailureCodeLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        result.Data = resultData.Data.OfType<FailureCodeLinq>().ToList();
        result.Count = resultData.Count;

        return result;
    }

    [OperationContract]
    [WebGet]
    public ResultWOLinq GetWorkOrderList(string workorders, int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        ResultWOLinq result = new ResultWOLinq();

        HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        
        if (HttpContext.Current.Session["Login"] == null)
        {
            result.Data = null;
            result.Count = 0;
            return result;
        }

        string[] wonums = workorders.Split(' ');

        var query = from tc in dc.WOLinqs
                    where wonums.Contains(tc.WoNum)
                    select tc;

        GridLinqBindingData<WOLinq> resultData = RadGrid.GetBindingData<WOLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        result.Data = resultData.Data.OfType<WOLinq>().ToList();
        result.Count = resultData.Count;

        return result;
    }


    [OperationContract]
    [WebGet]
    public string TimerStop(string wonum, string description, Boolean iscompletewo)
    {
        string result = "OK";
        string empid = HttpContext.Current.Session["Login"].ToString();
        decimal rate = dc.EmployeeLinqs.Where(x => x.Empid.ToLower() == empid.ToLower()).FirstOrDefault().Rate;
        decimal chargeback = dc.WOLinqs.Where(x => x.WoNum == wonum).FirstOrDefault().Chargeback == 1 ? 1 : 0;

        HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

        if (HttpContext.Current.Session["Login"] == null)
        {
            return "Invalid Login";
        }

        LabourTimeStampLinq timer = dc.LabourTimeStampLinqs.Where(x => (x.status == "running" || x.status == "pause")
                                                && x.wonum == wonum && x.empid.ToLower() == HttpContext.Current.Session["Login"].ToString().ToLower()).FirstOrDefault();


        if (timer != null)
        {
            timer.ModifyBy = HttpContext.Current.Session["Login"].ToString();
            timer.ModifyDate = DateTime.Now;
            if (timer.status == "running")
            {
                timer.endtime = DateTime.Now;
            }
            timer.status = "stop";
        }
        else
        {
            result = "Invalid request.";
        }
        int batchnum = timer.batchnum;

        LabourTimeStampLinq[] timers = dc.LabourTimeStampLinqs.Where(x => x.batchnum == batchnum).OrderBy(x => x.starttime).ToArray();

        List<WOLabourLinq> wolabours = new List<WOLabourLinq>();
        WOLabourLinq wolabour = new WOLabourLinq();




        DateTime starttime = timers[0].starttime;
        double hours = 0;
        DateTime endtime;
        for (int i = 0; i < timers.Count(); i++)
        {
            if (starttime.Date != timers[i].starttime.Date)
            {
                //finish the day befor and insert to wolabor

                wolabour = new WOLabourLinq();
                wolabour.Empid = empid.ToUpper();
                wolabour.WoNum = wonum;
                wolabour.TransDate = starttime;
                wolabour.Hours = Convert.ToDecimal(hours);
                wolabour.Description = description;
                wolabour.Rate = rate;
                wolabour.Tax1 = 0;
                wolabour.Tax2 = 0;
                wolabour.CBTax1 = 0;
                wolabour.CBTax2 = 0;
                wolabour.AddCost = 0;
                wolabour.Inactive = 0;
                wolabour.LaborType = "REG";
                wolabour.Scale = dc.LaborTypeLinqs.Where(x => x.LabType == wolabour.LaborType).FirstOrDefault().Scale;
                wolabour.Estimate = 0;
                wolabour.ordertype = "workorder";
                wolabour.Actual = 0;
                wolabour.WORate = 0;
                wolabour.ChargeBack = chargeback;
                wolabour.TotalCost = (rate + wolabour.AddCost) * wolabour.Hours * wolabour.Scale * (1 + wolabour.Tax1 + wolabour.Tax2) * (1 + wolabour.WORate);
                wolabour.ChargeBackAmount = (rate + wolabour.AddCost) * wolabour.Hours * wolabour.Scale * (1 + wolabour.CBTax1 + wolabour.CBTax2) * (1 + wolabour.WORate) * chargeback;

                wolabours.Add(wolabour);


                hours = 0;
            }
            starttime = timers[i].starttime;
            if (timers[i].endtime != null)
            {
                endtime = (DateTime)timers[i].endtime;
                while (endtime.Date > starttime.Date)
                {
                    DateTime endtoday = starttime.Date.AddDays(1);
                    hours = hours + endtoday.Subtract(starttime).TotalHours;

                    //finish yesterday and inster to wolabour
                    wolabour = new WOLabourLinq();
                    wolabour.Empid = empid.ToUpper();
                    wolabour.WoNum = wonum;
                    wolabour.TransDate = starttime;
                    wolabour.Description = description;
                    wolabour.Hours = Convert.ToDecimal(hours);
                    wolabour.Rate = rate;
                    wolabour.Tax1 = 0;
                    wolabour.Tax2 = 0;
                    wolabour.CBTax1 = 0;
                    wolabour.CBTax2 = 0;
                    wolabour.AddCost = 0;
                    wolabour.Inactive = 0;
                    wolabour.LaborType = "REG";
                    wolabour.Scale = dc.LaborTypeLinqs.Where(x => x.LabType == wolabour.LaborType).FirstOrDefault().Scale;
                    wolabour.Estimate = 0;
                    wolabour.ordertype = "workorder";
                    wolabour.Actual = 0;
                    wolabour.WORate = 0;
                    wolabour.ChargeBack = chargeback;
                    wolabour.TotalCost = (rate + wolabour.AddCost) * wolabour.Hours * wolabour.Scale * (1 + wolabour.Tax1 + wolabour.Tax2) * (1 + wolabour.WORate);
                    wolabour.ChargeBackAmount = (rate + wolabour.AddCost) * wolabour.Hours * wolabour.Scale * (1 + wolabour.CBTax1 + wolabour.CBTax2) * (1 + wolabour.WORate) * chargeback;

                    wolabours.Add(wolabour);


                    starttime = starttime.Date.AddDays(1);
                    hours = 0;
                }

                hours = hours + endtime.Subtract(starttime).TotalHours;
            }
        }

        if (hours != 0)
        {
            wolabour = new WOLabourLinq();
            wolabour.Empid = empid.ToUpper();
            wolabour.WoNum = wonum;
            wolabour.TransDate = starttime;
            wolabour.Description = description;
            wolabour.Hours = Convert.ToDecimal(hours);
            wolabour.Rate = rate;
            wolabour.Tax1 = 0;
            wolabour.Tax2 = 0;
            wolabour.CBTax1 = 0;
            wolabour.CBTax2 = 0;
            wolabour.AddCost = 0;
            wolabour.Inactive = 0;
            wolabour.LaborType = "REG";
            wolabour.Scale = dc.LaborTypeLinqs.Where(x => x.LabType == wolabour.LaborType).FirstOrDefault().Scale;
            wolabour.Estimate = 0;
            wolabour.ordertype = "workorder";
            wolabour.Actual = 0;
            wolabour.WORate = 0;
            wolabour.ChargeBack = chargeback;
            wolabour.TotalCost = (rate + wolabour.AddCost) * wolabour.Hours * wolabour.Scale * (1 + wolabour.Tax1 + wolabour.Tax2) * (1 + wolabour.WORate);
            wolabour.ChargeBackAmount = (rate + wolabour.AddCost) * wolabour.Hours * wolabour.Scale * (1 + wolabour.CBTax1 + wolabour.CBTax2) * (1 + wolabour.WORate) * chargeback;

            wolabours.Add(wolabour);
        }

        if (iscompletewo)
        {
            Workorder wo = new Workorder(empid,"Workorder","wonum",wonum);
            NameValueCollection statusnvc = new NameValueCollection();
            statusnvc.Add("statuscode","300");
            statusnvc.Add("status", "COMP");
            statusnvc.Add("compdate", DateTime.Now.ToString());
            wo.Complete(statusnvc, null);
        }


        dc.WOLabourLinqs.InsertAllOnSubmit(wolabours);

        try
        {
            dc.SubmitChanges();
        }
        catch (Exception e)
        {   
            result = e.Message;
        }


        return result;
    }



    [OperationContract]
    [WebGet]
    public string TimerResume(string wonum) 
    {
        string result = "OK";

        HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

        if (HttpContext.Current.Session["Login"] == null)
        {
            return "Invalid Login";
        }

        LabourTimeStampLinq pausetimer = new LabourTimeStampLinq();
        pausetimer = dc.LabourTimeStampLinqs.Where(x => x.status == "pause" && x.wonum == wonum && x.empid.ToLower() == HttpContext.Current.Session["Login"].ToString().ToLower()).FirstOrDefault();

        if (pausetimer != null)
        {
            pausetimer.status = "stop";
            pausetimer.endtime = DateTime.Now;
            pausetimer.ModifyBy = HttpContext.Current.Session["Login"].ToString();
            pausetimer.ModifyDate = DateTime.Now;
            LabourTimeStampLinq newtimer = new LabourTimeStampLinq() ;


            newtimer.batchnum = pausetimer.batchnum;
            newtimer.wonum = wonum;
            newtimer.empid = HttpContext.Current.Session["Login"].ToString();
            newtimer.starttime = DateTime.Now;
            newtimer.CreatedBy = newtimer.empid = HttpContext.Current.Session["Login"].ToString();
            newtimer.ModifyBy = newtimer.empid = HttpContext.Current.Session["Login"].ToString();
            newtimer.CreationDate = DateTime.Now;
            newtimer.ModifyDate = DateTime.Now;
            newtimer.labtype = "REG";
            newtimer.status = "running";
            dc.LabourTimeStampLinqs.InsertOnSubmit(newtimer);
        }
        else
        {
            result = "Invalid request.";
        }

        try
        {
            dc.SubmitChanges();
        }
        catch (Exception e)
        {
            result = e.Message;
        }

        return result;
    }

    [OperationContract]
    [WebGet]
    public string TimerPause(string wonum)
    {
        string result = "OK";


        HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

        if (HttpContext.Current.Session["Login"] == null)
        {
            return "Invalid Login";
        }

        LabourTimeStampLinq runningtimer = new LabourTimeStampLinq();
        runningtimer = dc.LabourTimeStampLinqs.Where(x => x.status == "running" && x.wonum == wonum && x.empid.ToLower() == HttpContext.Current.Session["Login"].ToString().ToLower()).FirstOrDefault();
        if (runningtimer != null)
        {
            runningtimer.status = "pause";
            runningtimer.endtime = DateTime.Now;
            runningtimer.ModifyBy = HttpContext.Current.Session["Login"].ToString();
            runningtimer.ModifyDate = DateTime.Now;
        }
        else 
        {
            result = "Invalid request.";
        }
        try
        {
            dc.SubmitChanges();
        }
        catch(Exception e)
        {
            result = e.Message;
        }
        
        
        return result;
    }

    [OperationContract]
    [WebGet]
    public string GetAdditionalData(string wonum)
    {
        string result = "{}";
        string login = "";
        HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

        if (HttpContext.Current.Session["Login"] == null)
        {
        //    return "Invalid Login";
            return "Session timed out. Please login again.";
        }
        else
            login = HttpContext.Current.Session["Login"].ToString();

        Workorder obj = new Workorder(login,"Workorder","wonum",wonum);
        return obj.GetAdditionalInfo();

    }

    [OperationContract]
    [WebGet]
    public string GetDuplicateWO(string wonum)
    {
        string result = "{}";
        string login = "";
        HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

        if (HttpContext.Current.Session["Login"] == null)
        {
            //    return "Invalid Login";
            return "{}";
        }
        else
            login = HttpContext.Current.Session["Login"].ToString();

        Workorder obj = new Workorder(login, "Workorder", "wonum", wonum);

        return obj.CreateDuplicateData();

    }

    [OperationContract]
    [WebGet]
    public string TimerStart(string wonum)
    {
        string result = "OK";


        HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

        if (HttpContext.Current.Session["Login"] == null)
        {
            return "Invalid Login";
        }

        LabourTimeStampLinq newtimer = new LabourTimeStampLinq();

        int maxbatchnum;
        if (dc.LabourTimeStampLinqs.Count() > 0)
        {
            maxbatchnum = dc.LabourTimeStampLinqs.Max(x => x.batchnum) + 1;
        }
        else
        {
            maxbatchnum = 0;
        }

        newtimer.batchnum = maxbatchnum;
        newtimer.wonum = wonum;
        newtimer.empid = HttpContext.Current.Session["Login"].ToString();
        newtimer.starttime = DateTime.Now;
        newtimer.CreatedBy = newtimer.empid = HttpContext.Current.Session["Login"].ToString();
        newtimer.ModifyBy = newtimer.empid = HttpContext.Current.Session["Login"].ToString();
        newtimer.CreationDate = DateTime.Now;
        newtimer.ModifyDate = DateTime.Now;
        newtimer.labtype = "REG";
        newtimer.status = "running";
        dc.LabourTimeStampLinqs.InsertOnSubmit(newtimer);
        try
        {

            dc.SubmitChanges();
        }
        catch (Exception e)
        {
            result = e.Message;
        }

        return result;
    }

    [OperationContract]
    public ResultLabourTimeStampLinq GetTimerStampList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        ResultLabourTimeStampLinq result = new ResultLabourTimeStampLinq();

        string login_user = "";
        int batchnum = -999;
        if (System.Web.HttpContext.Current.Session["Login"] != null)
        {
            login_user = HttpContext.Current.Session["Login"].ToString();
            if (login_user == "")
            {
                login_user = "";
                result.Data = null;
                result.Count = 0;
                return result;
            }
        }
        else
        {
            login_user = "";
            result.Data = null;
            result.Count = 0;
            return result;
        }


        
        string wonum =  HttpContext.Current.Request.QueryString.Get("wonum") ?? "";
        string mode = HttpContext.Current.Request.QueryString.Get("mode") ?? "";
        if (wonum != "")
        {
            if (dc.LabourTimeStampLinqs.Where(x => x.empid.ToLower() == login_user.ToLower() && x.wonum == wonum && (x.status =="running" || x.status == "pause")).Count() > 0)
            {
                batchnum = dc.LabourTimeStampLinqs.Where(x => x.empid.ToLower() == login_user.ToLower() && x.wonum == wonum && (x.status == "running" || x.status == "pause")).First().batchnum;
            }

        }
        
        //var query = dc.LabourTimeStampLinqs.Where(x => x.wonum == wonum && x.empid.ToLower() == login_user.ToLower() && x.batchnum == batchnum);

        var query = dc.LabourTimeStampLinqs.Where(x => x.wonum == wonum);
        if (mode != "info")
        {
            query = query.Where(x=>x.empid.ToLower() == login_user.ToLower() && x.batchnum == batchnum);
        }
      


        GridLinqBindingData<LabourTimeStampLinq> resultData = RadGrid.GetBindingData<LabourTimeStampLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        result.Data = resultData.Data.OfType<LabourTimeStampLinq>().ToList();
        result.Count = resultData.Count;


        return result;
    }

    public class RecentworklistResult
    {
        public Int32 Count { get; set; }
        public List<v_WORecentlist> Data { get; set; }
    }

    public class ResultTaskLinq
    {
      public Int32 Count { get; set; }
      public List<WOTaskLinq> Data { get; set; }
    }

    public class ResultWOTypeLinq
    {
      public Int32 Count { get; set; }
      public List<WorkTypeLinq> Data { get; set; }
    }

    public class ResultCloseRemarkLinq
    {
      public Int32 Count { get; set; }
      public List<CloseRemarkLinq> Data { get; set; }
    }
}

public class ResultWOLinq
{
    public Int32 Count { get; set; }
    public List<WOLinq> Data { get; set; }
}

public class ResultFailureCodeLinq
{
    public Int32 Count { get; set; }
    public List<FailureCodeLinq> Data { get; set; }
}

public class ResultLabourTimeStampLinq
{
    public Int32 Count { get; set; }
    public List<LabourTimeStampLinq> Data {get; set;}
}
