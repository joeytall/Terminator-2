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
public class ServiceProc
{
    protected DataClassesDataContext dc = new DataClassesDataContext(AzzierData.LinqConnectstring);
    protected string lookcriteria, mode = "", login_user = "";
    protected List<string> division = new List<string>();

    // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
    // To create an operation that returns XML,
    //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
    //     and include the following line in the operation body:
    //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
    [OperationContract]
    public void DoWork()
    {
        // Add your operation implementation here
        return;
    }

    // Add more operations here and mark them with [OperationContract]
    /// <summary>
    /// Create new workorder
    /// </summary>
    /// <param name="userid"></param>
    /// <param name="xmlnvc"></param>
    /// <returns></returns>
    [OperationContract]
    [WebGet]
    public string CreateProcedure(string userid, string xmlnvc)
    {
        string returnStr = "";
        bool success = false;
        userid = userid.ToUpper();
        NameValueCollection nvc;
        nvc = new NameValueCollection();

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xmlnvc);

        XmlNode root = doc.DocumentElement.FirstChild;

        XmlNodeList proclist = root.ChildNodes;

        for (int i = 0; i < proclist.Count; i++)
        {
            string name = proclist[i].Name;
            string val = proclist[i].InnerText;
            nvc.Add(name, val);
        }

        Procedure objProcedure;

        objProcedure = new Procedure(userid, "procedures", "ProcNum");
        success = objProcedure.Create(nvc);

        if (success)
        {
            returnStr = nvc["procnum"];
        }

        return returnStr;
    }

    [OperationContract]
    [WebGet]
    public string AddFromTaskLibrary(string counters, string ordertype, string ordernum,string estimate)
    {
      string returnStr = "";
      bool success = false;
      string userid = "";
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

      Task objtask;

      objtask = new Task(userid, "WOTasks", "Counter");
      success = objtask.AddFromTaskLibrary(counters, ordertype, ordernum,estimate);

      if (success)
        return "OK";
      else
        return objtask.ErrorMessage;
    }

    /// <summary>
    /// Save workorder
    /// </summary>
    /// <param name="xmlnvc"></param>
    /// <returns></returns>
    [OperationContract]
    [WebGet]
    public string SaveProcedure(string xmlnvc)
    {
        //string returnStr = "", oldprocnum = "", newprocnum = "", userid = "";
        string returnStr = "", oldprocnum = "", userid = "";

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

        Procedure objProcedure;

        string proc = nvc["procnum"];
        objProcedure = new Procedure(o.ToString(), "Procedures", "ProcNum", proc);
        success = objProcedure.Update(nvc);
        if (success)
        {
            /*
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
            }*/
            returnStr = "TRUE^" + nvc["procnum"];
        }
        else
        {
            string err = "FALSE^" + objProcedure.ErrorMessage.Replace("\r\n", "");
            returnStr = err;
        }

        return returnStr;
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

    [OperationContract]
    [WebGet]
    public string ProcDelWorkList(string procxml)
    {
        string retstr = "SUCCESS.";
        string login_user = "";
        if (System.Web.HttpContext.Current.Session["Login"] != null)
        {
            login_user = HttpContext.Current.Session["Login"].ToString();
            if (login_user == "")
            {
                return "ERROR.";
            }
        }
        XDocument doc = XDocument.Parse(procxml);

        var idresult = from id in doc.Descendants("IDS") select id.Value;

        try
        {
            var result = from userwl in dc.UserWorkListLinqs
                         where idresult.ToList().Contains(userwl.LinkId)
                             && userwl.UserId.ToLower() == login_user.ToLower()
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
    [WebGet]
    public string ProcAddtoWorkList(string paramXML)
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
        var idresult = from id in doc.Descendants("IDS")
                       where !(from c in dc.UserWorkListLinqs
                               where c.LinkModule.ToLower() == "procedure" && c.LinkType.ToLower() == "worklist" && c.UserId.ToLower() == login_user.ToLower()
                               select c.LinkId).Contains(id.Value)
                       select id.Value;

        var ins = from id in dc.ProcedureLinqs
                  where idresult.ToList().Contains(id.ProcNum)
                  select new
                  {
                      LinkId = id.ProcNum,
                      UserId = login_user,
                      LinkModule = "Procedure",
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

        dc.UserWorkListLinqs.InsertAllOnSubmit(worklist);
        try
        {
            dc.SubmitChanges();
        }
        catch (Exception e)
        {
            retstr = e.Message;
        }

        return retstr;
    }

    /// <summary>
    /// Procedure module: top menu button -- "Look Up"
    /// </summary>
    /// <param name="startRowIndex"></param>
    /// <param name="maximumRows"></param>
    /// <param name="sortExpression"></param>
    /// <param name="filterExpression"></param>
    /// <returns></returns>
    [OperationContract]
    public ResultProcLinq LookupDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        ResultProcLinq result = new ResultProcLinq();
        string searchcriteria;
        List<string> proclist = new List<string>();

        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
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

        var query = from p in dc.ProcedureLinqs select p;
        GridLinqBindingData<ProcedureLinq> data = RadGrid.GetBindingData<ProcedureLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        result.Data = data.Data.OfType<ProcedureLinq>().ToList();
        result.Count = data.Count;

        return result;
    }

    [OperationContract]
    [WebGet]
    public ResultProcLinq QueryResultProc(int startRowIndex, int maximumRows, string sortExpression, string filterExpression, string jsonData)
    {
        ResultProcLinq result = new ResultProcLinq();
        string searchcriteria;
        List<string> proclist = new List<string>();

        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";

        NameValueCollection nvc = AzzierData.DeserializeJSON(jsonData);
        object empid = HttpContext.Current.Session["LogIn"];
        string userid = (empid != null) ? empid.ToString() : "";
        ModuleoObject obj = new ModuleoObject(userid, "Procedures", "ProcNum");
        string wherestring = "", linqwherestring = "";
        if (jsonData.Contains("queryID"))
          nvc = UserQuery.CreateNameValueCollection(nvc["queryID"]);
        obj.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
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

        var query = from p in dc.ProcedureLinqs select p;
        GridLinqBindingData<ProcedureLinq> data = RadGrid.GetBindingData<ProcedureLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        result.Data = data.Data.OfType<ProcedureLinq>().ToList();
        result.Count = data.Count;

        return result;
    }
    [OperationContract]
    public ResultProcLinq GetWorkList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        ResultProcLinq result = new ResultProcLinq();

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
                    join p in dc.ProcedureLinqs on u.LinkId equals p.ProcNum
                    where p.Inactive == 0 && u.LinkModule.ToLower() == "procedure" && u.LinkType.ToLower() == "worklist" && u.UserId.ToLower() == login_user.ToLower()
                    orderby u.LinkId
                    select p;

        GridLinqBindingData<ProcedureLinq> data = RadGrid.GetBindingData<ProcedureLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = data.Data.ToList<ProcedureLinq>();
        result.Count = data.Count;

        return result;
    }

    [OperationContract]
    public RecentWorkListResult RecentListDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        RecentWorkListResult result = new RecentWorkListResult();
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

        var q = from p in dc.v_ProcRecentlists
                where p.UserId.ToLower() == login_user.ToLower()
                orderby p.AccessDate descending
                select p;

        GridLinqBindingData<v_ProcRecentlist> resultData = RadGrid.GetBindingData<v_ProcRecentlist>(q, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = resultData.Data.ToList<v_ProcRecentlist>();
        result.Count = resultData.Count;

        return result;
    }

    public class RecentWorkListResult
    {
        public int Count { get; set; }
        public List<v_ProcRecentlist> Data { get; set; }
    }
}

public class ResultProcLinq
{
    public int Count { get; set; }
    public List<ProcedureLinq> Data { get; set; }
}
