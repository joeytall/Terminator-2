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
public class ServiceWR
{
  protected DataClassesDataContext dc = new DataClassesDataContext(AzzierData.LinqConnectstring);
  //protected System.Web.HttpContext.Current.Session.LCID=2057;

  // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
  // To create an operation that returns XML,
  //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
  //     and include the following line in the operation body:
  //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";


  /// <summary>
  /// Save workrequest
  /// </summary>
  /// <param name="xmlnvc"></param>
  /// <returns></returns>
  [OperationContract]
  [WebGet]
  public string SaveWorkRequest(string xmlnvc)
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

    WorkRequest objWorkorder;

    string wrnum = nvc["wrnum"];
    objWorkorder = new WorkRequest(o.ToString(), "WorkRequest", "WrNum", wrnum);
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
        l.DeleteAllLabour("workrequest", wrnum, 1);
        l.CopyLabour("procedure", nvc["procnum"], "workrequest", wrnum);
        Service s = new Service(userid, "WOService", "Counter");
        s.DeleteAllService("workrequest", wrnum, 1);
        s.CopyService("procedure", nvc["procnum"], "workrequest", wrnum);
        Material m = new Material(userid, "WOMaterial", "Counter");
        m.DeleteAllMaterial("workrequest", wrnum, 1);
        m.CopyMaterial("procedure", nvc["procnum"], "workrequest", wrnum);
        Tool t = new Tool(userid, "WOTools", "Counter");
        t.DeleteAllTools("workrequest", wrnum, 1);
        t.CopyTools("procedure", nvc["procnum"], "workrequest", wrnum);
        Task task = new Task(userid, "WOTasks", "Counter");
        task.DeleteAllTask("workrequest", wrnum, 1);
        task.CopyTasks("procedure", nvc["procnum"], "workrequest", wrnum);
      }
      returnStr = "TRUE^" + nvc["wrnum"];
    }
    else
    {
      string err = "FALSE^" + objWorkorder.ErrorMessage.Replace("\r\n", "");
      returnStr = err;
    }

    return returnStr;
  }

  /// <summary>
  /// Create new workrequest
  /// </summary>
  /// <param name="userid"></param>
  /// <param name="xmlnvc"></param>
  /// <returns></returns>
  [OperationContract]
  [WebGet]
  public string CreateWorkRequest(string userid, string xmlnvc, string oldwrnum = "", int estimate = 1)
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

    WorkRequest objWorkorder;

    objWorkorder = new WorkRequest(userid, "WorkRequest", "WrNum");
    success = objWorkorder.Create(nvc);

    if (success)
    {
      if (oldwrnum == "" || oldwrnum == null)
      {
        if (nvc["procnum"].ToString() != "")
        {
          Labour l = new Labour(userid, "WOLabour", "Counter");
          l.CopyLabour("procedure", nvc["procnum"], "workrequest", nvc["wrnum"]);
          Service s = new Service(userid, "WOService", "Counter");
          s.CopyService("procedure", nvc["procnum"], "workrequest", nvc["wrnum"]);
          Material m = new Material(userid, "WOMaterial", "Counter");
          m.CopyMaterial("procedure", nvc["procnum"], "workrequest", nvc["wrnum"]);
          Tool t = new Tool(userid, "WOTools", "Counter");
          t.CopyTools("procedure", nvc["procnum"], "workrequest", nvc["wrnum"]);
          Task task = new Task(userid, "WOTasks", "Counter");
          task.CopyTasks("procedure", nvc["procnum"], "workrequest", nvc["wrnum"]);
        }

      }
      else  //duplicate
      {
        Labour l = new Labour(userid, "WOLabour", "Counter");
        l.CopyLabour("workrequest", oldwrnum, "workrequest", nvc["wrnum"], estimate);
        Service s = new Service(userid, "WOService", "Counter");
        s.CopyService("workrequest", oldwrnum, "workrequest", nvc["wrnum"], estimate);
        Material m = new Material(userid, "WOMaterial", "Counter");
        m.CopyMaterial("workrequest", oldwrnum, "workrequest", nvc["wrnum"], estimate);
        Tool t = new Tool(userid, "WOTools", "Counter");
        t.CopyTools("workrequest", oldwrnum, "workrequest", nvc["wrnum"], estimate);
        Task task = new Task(userid, "WOTasks", "Counter");
        task.CopyTasks("workrequest", oldwrnum, "workrequest", nvc["wrnum"], estimate);
      }
      returnStr = nvc["wrnum"];
    }

    return returnStr;
  }

  [OperationContract]
  [WebGet]
  public string GetTotals(string wrnum)
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

    WorkRequest w = new WorkRequest(userid, "WorkRequest", "WRNum", wrnum);
    result = w.ModuleData["EstHours"] + "^" + w.ModuleData["EstLabor"] + "^" + w.ModuleData["EstMaterial"] + "^" + w.ModuleData["EstTools"] + "^" + w.ModuleData["EstService"];

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
  /*
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
   * */

  [OperationContract]
  public ResultWRLinq SearchDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {
    ResultWRLinq result = new ResultWRLinq();

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

    var query = from w in dc.WRLinqs select w;
    GridLinqBindingData<WRLinq> data = RadGrid.GetBindingData<WRLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

      result.Data = data.Data.OfType<WRLinq>().ToList();
    result.Count = data.Count;

    return result;
  }

  [OperationContract]
  [WebGet]
  public ResultWRLinq QueryResultWR(int startRowIndex, int maximumRows, string sortExpression, string filterExpression, string jsonData = "{}")
  {
    ResultWRLinq result = new ResultWRLinq();

    System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

    string searchcriteria;
    List<string> strlist = new List<string>();

    searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";

    NameValueCollection nvc = AzzierData.DeserializeJSON(jsonData);
    object empid = HttpContext.Current.Session["LogIn"];
    string userid = (empid != null) ? empid.ToString() : "";
    WorkRequest objWorkrequest = new WorkRequest(userid, "workrequest", "wrnum");
    string wherestring = "", linqwherestring = "";
    if (jsonData.Contains("queryID"))
      nvc = UserQuery.CreateNameValueCollection(nvc["queryID"]);
    objWorkrequest.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
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

    var query = from w in dc.WRLinqs select w;
    GridLinqBindingData<WRLinq> data = RadGrid.GetBindingData<WRLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

      result.Data = data.Data.OfType<WRLinq>().ToList();
    result.Count = data.Count;

    return result;
  }

  [OperationContract]
  public ResultWRLinq WorkListDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {
    ResultWRLinq result = new ResultWRLinq();

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
                join w in dc.WRLinqs on u.LinkId equals w.WRNum
                where w.Inactive == 0 && u.LinkModule.ToLower() == "workrequest" && u.LinkType == "worklist" && u.UserId.ToLower() == login_user.ToLower()
                orderby w.WRNum
                select w;

    GridLinqBindingData<WRLinq> data = RadGrid.GetBindingData<WRLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
    result.Data = data.Data.ToList<WRLinq>();
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
                           where c.LinkModule.ToLower() == "workrequest" && c.LinkType.ToLower() == "worklist" && c.UserId.ToLower() == login_user.ToLower()
                           select c.LinkId).Contains(id.Value)
                   select id.Value;

    var ins = from id in dc.WRLinqs
              where idresult.ToList().Contains(id.WRNum)
              select new
              {
                LinkId = id.WRNum,
                UserId = login_user,
                LinkModule = "workrequest",
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
  public string SetPending(string paramXML)
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

    XmlDocument doc = new XmlDocument();
    doc.LoadXml(paramXML);
    XmlNode root = doc.DocumentElement.FirstChild;
    XmlNodeList wrlist = root.ChildNodes;
    List<string> wrnums = new List<string>();
    for (int i = 0; i < wrlist.Count; i++)
    {
      string name = wrlist[i].Name;
      string val = wrlist[i].InnerText;
      if (name.ToUpper() == "WRNUM")
      {
        wrnums.Add(val);
      }
    }

    if (wrnums.Count > 0)
    {
      WorkRequest objwr = new WorkRequest(login_user, "WorkRequest", "WRNum");
      objwr.SetPending(wrnums);
    }

    return retstr;
  }

  [OperationContract]
  [WebGet]
  public string CreateWO(string wrnum, string copyestimate)
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

    WorkRequest objwr = new WorkRequest(login_user, "WorkRequest", "WRNum", wrnum);
    NameValueCollection nvcwr = objwr.ModuleData;
    NameValueCollection nvcwo = new NameValueCollection();

    Workorder objwo = new Workorder(login_user, "WorkOrder", "WONum");
    NewNumber n = new NewNumber();
    string wonum = n.GetNextNumber("WO");
    nvcwo.Add(nvcwr);
    nvcwo.Remove("WRNum");
    nvcwo.Add("WONum", wonum);
    nvcwo["Status"] = "NEWWO";
    nvcwo["StatusCode"] = "1";
    nvcwo.Remove("Pending");
    nvcwo.Remove("AssignDate");
    nvcwo["EstMaterial"] = "0";
    nvcwo["EstHours"] = "0";
    nvcwo["EstTools"] = "0";
    nvcwo["EstService"] = "0";
    nvcwo["EstLabor"] = "0";
    nvcwo["OpenDate"] = DateTime.Today.ToString("yyyy/MM/dd");

    if (objwo.Create(nvcwo))
    {
      if (copyestimate.ToUpper() == "TRUE")
      {
        Task t = new Task(login_user, "WOTasks", "Counter");
        t.CopyTasks("WorkRequest", wrnum, "Workorder", wonum);
        Labour l = new Labour(login_user, "WOLabour", "Counter");
        l.CopyLabour("WorkRequest", wrnum, "Workorder", wonum);
        Material m = new Material(login_user, "WOMaterial", "Counter");
        m.CopyMaterial("WorkRequest", wrnum, "Workorder", wonum);
        Service s = new Service(login_user, "WOService", "Counter");
        s.CopyService("WorkRequest", wrnum, "Workorder", wonum);
        Tool tool = new Tool(login_user, "WOTools", "Counter");
        tool.CopyTools("WorkRequest", wrnum, "Workorder", wonum);

      }
      ModuleoObject objrelation = new ModuleoObject(login_user, "WRWORelation", "Counter");
      NameValueCollection nvc = new NameValueCollection();
      nvc.Add("WRNum", wrnum);
      nvc.Add("WONum", wonum);
      nvc.Add("AssignedBy", login_user);
      nvc.Add("AssignedDate", DateTime.Today.ToString("yyyy/MM/dd"));
      objrelation.Create(nvc);

      nvc.Clear();
      nvc.Add("Status", "ASSIGNED");
      nvc.Add("StatusCode", "300");
      nvc.Add("Pending", "0");
      objwr.UpdateStatus(nvc);


      retstr = "OK^" + wonum;
    }
    else
    {
      retstr = objwo.ErrorMessage;
    }



    return retstr;
  }

  [OperationContract]
  [WebGet]
  public string LinkWO(string wrnum, string wonum, string copyestimate)
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

    WorkRequest objwr = new WorkRequest(login_user, "WorkRequest", "WRNum", wrnum);
    NameValueCollection nvcwr = objwr.ModuleData;
    NameValueCollection nvcwo = new NameValueCollection();

    Workorder objwo = new Workorder(login_user, "WorkOrder", "WONum",wonum);
    decimal statuscode = Convert.ToDecimal(objwo.ModuleData["StatusCode"]);
    if (copyestimate.ToUpper() == "TRUE" && statuscode<100 && statuscode >=1)
    {
      Task t = new Task(login_user, "WOTasks", "Counter");
      t.CopyTasks("WorkRequest", wrnum, "Workorder", wonum);
      Labour l = new Labour(login_user, "WOLabour", "Counter");
      l.CopyLabour("WorkRequest", wrnum, "Workorder", wonum);
      Material m = new Material(login_user, "WOMaterial", "Counter");
      m.CopyMaterial("WorkRequest", wrnum, "Workorder", wonum);
      Service s = new Service(login_user, "WOService", "Counter");
      s.CopyService("WorkRequest", wrnum, "Workorder", wonum);
      Tool tool = new Tool(login_user, "WOTools", "Counter");
      tool.CopyTools("WorkRequest", wrnum, "Workorder", wonum);
    }
    ModuleoObject objrelation = new ModuleoObject(login_user, "WRWORelation", "Counter");
    NameValueCollection nvc = new NameValueCollection();
    nvc.Add("WRNum", wrnum);
    nvc.Add("WONum", wonum);
    nvc.Add("AssignedBy", login_user);
    nvc.Add("AssignDate", DateTime.Today.ToString("yyyy/MM/dd"));
    objrelation.Create(nvc);
    nvc.Clear();
    nvc.Add("Status", "ASSIGNED");
    nvc.Add("StatusCode", "300");
    nvc.Add("Pending", "0");
    objwr.UpdateStatus(nvc);

    retstr = "OK^" + wonum;
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
  public RecentWRworklistResult RecentListDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {
    RecentWRworklistResult result = new RecentWRworklistResult();

    System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

    string login_user = "";
    if (System.Web.HttpContext.Current.Session["Login"] != null)
    {
      login_user = HttpContext.Current.Session["Login"].ToString();
    }

    var qq = from v in dc.v_WorkRequestRecentlists
             where v.UserId.ToLower() == login_user.ToLower()
             orderby v.AccessDate descending
             select v;

    GridLinqBindingData<v_WorkRequestRecentlist> resultData = RadGrid.GetBindingData<v_WorkRequestRecentlist>(qq, startRowIndex, maximumRows, sortExpression, filterExpression);
    result.Data = resultData.Data.ToList<v_WorkRequestRecentlist>();
    result.Count = resultData.Count;

    return result;
  }

  public class RecentWRworklistResult
  {
    public Int32 Count { get; set; }
    public List<v_WorkRequestRecentlist> Data { get; set; }
  }
}

public class ResultWRLinq
{
  public Int32 Count { get; set; }
  public List<WRLinq> Data { get; set; }
}
