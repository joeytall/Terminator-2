using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;
using Telerik.Web.UI;
using System.Runtime.Serialization;
using System.Xml.Linq;

[ServiceContract(Namespace = "")]
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class ServicePDM
{
  protected DataClassesDataContext dc = new DataClassesDataContext(AzzierData.LinqConnectstring);
  protected string lookcriteria, mode = "", login_user = "";
  protected List<string> division = new List<string>();
  // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
  // To create an operation that returns XML,
  //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
  //     and include the following line in the operation body:
  //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";


  



  /// <summary>
  /// update pm
  /// </summary>
  /// <param name="xmlnvc"></param>
  /// <returns></returns>
  /// 
  [OperationContract]
  [WebGet]
  public string UpdatePDM(string xmlnvc)
  {
    string returnStr = "", userid = "";

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

    Route objPDM;

    string pdmnum = nvc["pdmnum"];
    objPDM = new Route(o.ToString(), "PDM", "PDMNum", pdmnum);
    success = objPDM.Update(nvc);
    if (success)
    {
      returnStr = "TRUE^" + nvc["pdmnum"];
    }
    else
    {
      string err = "FALSE^" + objPDM.ErrorMessage.Replace("\r\n", "");
      returnStr = err;
    }

    return returnStr;
  }

  /// <summary>
  /// Create new pm
  /// </summary>
  /// <param name="userid"></param>
  /// <param name="xmlnvc"></param>
  /// <returns></returns>
  [OperationContract]
  [WebGet]
  public string CreatePDM(string userid, string xmlnvc)
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

    PDM objPDM;

    objPDM = new PDM(userid, "PDM", "PDMNum");
    success = objPDM.Create(nvc);

    if (success)
    {
      returnStr = nvc["pdmnum"];
    }
    else
      returnStr = objPDM.ErrorMessage;

    return returnStr;

  }

  [OperationContract]
  [WebGet]
  public string DuplicatePDM(string userid, string xmlnvc, string oldpdmnum)  //resource source is "pm" or "procedure", if pm, then copy resources from pm , otherwise from procedure
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

    PDM objPDM;

    objPDM = new PDM(userid, "PDM", "PDMNum");
    success = objPDM.Create(nvc);

    if (success)
    {
      returnStr = nvc["pdmnum"];
    }
    else
      returnStr = objPDM.ErrorMessage;

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
  public ResultPDMLinq GetWorkList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {

      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
    ResultPDMLinq result = new ResultPDMLinq();

    string login_user = "";
    if (System.Web.HttpContext.Current.Session["Login"] != null)
    {
      login_user = Convert.ToString(HttpContext.Current.Session["Login"]);
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
                join p in dc.PDMLinqs on u.LinkId equals p.PDMNum
                where p.Inactive == 0 && u.LinkModule.ToLower() == "pdm" && u.LinkType.ToLower() == "worklist" && u.UserId.ToLower() == login_user.ToLower()
                orderby u.LinkId
                select p;

    GridLinqBindingData<PDMLinq> data = RadGrid.GetBindingData<PDMLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
    result.Data = data.Data.ToList<PDMLinq>();
    result.Count = data.Count;

    return result;
  }


  //[OperationContract]
  //public ResultPDMLinq GetRecentList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  //{
  //  ResultPDMLinq result = new ResultPDMLinq();
  //  string login_user = "";
  //  if (System.Web.HttpContext.Current.Session["Login"] != null)
  //  {
  //    login_user = Convert.ToString(HttpContext.Current.Session["Login"]);
  //    if (login_user == "")
  //    {
  //      login_user = "";
  //      result.Data = null;
  //      result.Count = 0;
  //      return result;
  //    }
  //  }
  //  else
  //  {
  //    login_user = "";
  //    result.Data = null;
  //    result.Count = 0;
  //    return result;
  //  }

  //  var qq = from p in dc.PDMLinqs
  //           join u in dc.UserWorkListLinqs on p.PDMNum equals u.LinkId
  //           where u.LinkType.ToLower() == "recent" && u.UserId.ToLower() == login_user.ToLower() && u.LinkModule.ToLower() == "pdm"
  //           orderby u.TransDate descending
  //           select p;

  //  GridLinqBindingData<PDMLinq> resultData = RadGrid.GetBindingData<PDMLinq>(qq, startRowIndex, maximumRows, sortExpression, filterExpression);
  //  result.Data = resultData.Data.ToList<PDMLinq>();
  //  result.Count = resultData.Count;

  //  return result;
  //}

  [OperationContract]
  public ResultRecentlist RecentListDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {
      ResultRecentlist result = new ResultRecentlist();
      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

      if (System.Web.HttpContext.Current.Session["Login"] != null)
      {
          login_user = Convert.ToString(HttpContext.Current.Session["Login"]);
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

      var query = from v in dc.v_PDMRecentlists where v.UserId.ToLower()==login_user.ToLower() 
                  orderby v.AccessDate descending 
                  select v;

      GridLinqBindingData<v_PDMRecentlist> resultData = RadGrid.GetBindingData<v_PDMRecentlist>(query,startRowIndex,maximumRows,sortExpression,filterExpression);
      result.Data = resultData.Data.ToList<v_PDMRecentlist>();
      result.Count = resultData.Count;

      return result;
  }

  [OperationContract]
  public ResultPDMLinq LookupDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {
    string lookupcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";
    string mode = HttpContext.Current.Request.QueryString.Get("mode") ?? "";

    System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
    ResultPDMLinq result = new ResultPDMLinq();
    if (System.Web.HttpContext.Current.Session["Login"] != null)
    {
      login_user = Convert.ToString(HttpContext.Current.Session["Login"]);
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

    if (lookupcriteria.Length > 1)
    {
      lookupcriteria = lookupcriteria.Replace("'", "\"");

      if (filterExpression == null || filterExpression.Length < 1)
      {
        filterExpression = "";
      }

      filterExpression = filterExpression != "" ? filterExpression + " and " : "";
      filterExpression = filterExpression + lookupcriteria;
    }

    if (mode.Length > 1)
    {
      string tmp_div = "";

      if (mode != "query")
      {
        if (System.Web.HttpContext.Current.Session["EditableDivision"] != null)
        {
          tmp_div = System.Web.HttpContext.Current.Session["EditableDivision"].ToString();
          division = new List<string>(tmp_div.Split(','));
        }
      }
      else
      {
        if (System.Web.HttpContext.Current.Session["AllDivision"] != null)
        {
          tmp_div = System.Web.HttpContext.Current.Session["AllDivision"].ToString();
          division = new List<string>(tmp_div.Split(','));
        }
      }
    }

    var query = from e in dc.PDMLinqs
                where division.Contains(e.Division) || e.Division.Equals(null)
                select e;

    if (mode != "query")
    {
      query = query.Where(a => a.Inactive == 0);
    }

    GridLinqBindingData<PDMLinq> data = RadGrid.GetBindingData<PDMLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
    result.Data = data.Data.OfType<PDMLinq>().ToList();
    result.Count = data.Count;

    return result;
  }

  [OperationContract]
  [WebGet]
  public string AddtoWorkList(string xml)
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

    XDocument doc = XDocument.Parse(xml);
    var idresult = from id in doc.Descendants("IDS")
                   where !(from c in dc.UserWorkListLinqs
                           where c.LinkModule.ToLower() == "pdm" && c.LinkType.ToLower() == "worklist" && c.UserId.ToLower() == login_user.ToLower()
                           select c.LinkId).Contains(id.Value)
                   select id.Value;

    var ins = from id in dc.PDMLinqs
              where idresult.ToList().Contains(id.PDMNum)
              select new
              {
                LinkId = id.PDMNum,
                UserId = login_user,
                LinkModule = "PDM",
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

  [OperationContract]
  public ResultPDMLinq SearchDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {

      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
    ResultPDMLinq result = new ResultPDMLinq();
    string searchcriteria;
    List<string> eqptlist = new List<string>();

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
    var query = from p in dc.PDMLinqs select p;
    GridLinqBindingData<PDMLinq> data = RadGrid.GetBindingData<PDMLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

//    GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "PDMLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
    result.Data = data.Data.OfType<PDMLinq>().ToList();
    result.Count = data.Count;

    return result;
  }

  [OperationContract]
  [WebGet]
  public ResultPDMLinq QueryResultPDM(int startRowIndex, int maximumRows, string sortExpression, string filterExpression, string jsonData)
  {

      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
      ResultPDMLinq result = new ResultPDMLinq();
      string searchcriteria;
      List<string> eqptlist = new List<string>();

      searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";

      NameValueCollection nvc = AzzierData.DeserializeJSON(jsonData);
      object empid = HttpContext.Current.Session["LogIn"];
      string userid = (empid != null) ? empid.ToString() : "";
      PDM objPDM = new PDM(userid, "PDM", "PDMNum");
      string wherestring = "", linqwherestring = "";
      if (jsonData.Contains("queryID"))
        nvc = UserQuery.CreateNameValueCollection(nvc["queryID"]);
      objPDM.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
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
      var query = from p in dc.PDMLinqs select p;
      GridLinqBindingData<PDMLinq> data = RadGrid.GetBindingData<PDMLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

      //    GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "PDMLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
      result.Data = data.Data.OfType<PDMLinq>().ToList();
      result.Count = data.Count;

      return result;
  }
  [OperationContract]
  public ResultPDMHistoryLinq GetPDMHistory(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {

    System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
    ResultPDMHistoryLinq result = new ResultPDMHistoryLinq();
    string searchcriteria;
    
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
    var query = from p in dc.PDMHistoryLinqs orderby p.TransDate descending, p.Counter descending select p;
    GridLinqBindingData<PDMHistoryLinq> data = RadGrid.GetBindingData<PDMHistoryLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

    //    GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "PDMLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
    result.Data = data.Data.OfType<PDMHistoryLinq>().ToList();
    result.Count = data.Count;

    return result;
  }

  [OperationContract]
  [WebGet]
  public string DelWorkList(string xml)
  {
    string retstr = "SUCCESS.";

    if (System.Web.HttpContext.Current.Session["Login"] != null)
    {
      login_user = HttpContext.Current.Session["Login"].ToString();
      if (login_user == "")
      { return "ERROR."; }
    }
    else { return "ERROR."; }

    XDocument doc = XDocument.Parse(xml);

    var idresult = from id in doc.Descendants("IDS") select id.Value;

    try
    {
      using (DataClassesDataContext dc = new DataClassesDataContext(AzzierData.LinqConnectstring))
      {
        var result = from userwl in dc.UserWorkListLinqs
                     where idresult.ToList().Contains(userwl.LinkId) && userwl.LinkModule.ToLower() == "pdm" &&
                     userwl.LinkType == "worklist" && userwl.UserId.ToLower() == login_user.ToLower()
                     select userwl;
        dc.UserWorkListLinqs.DeleteAllOnSubmit(result);
        dc.SubmitChanges();
      }
    }
    catch
    {
      retstr = "ERROR.";
    }

    return retstr;
  }


  [OperationContract]
  [WebGet]
  public string DoWork()
  {
    return "OK";
  }

  public class ResultRecentlist
  {
      public int Count { get; set; }
      public List<v_PDMRecentlist> Data { get; set; }
  }

  public class ResultPDMHistoryLinq
  {
    public int Count { get; set; }
    public List<PDMHistoryLinq> Data { get; set; }
  }
}

public class ResultPDMLinq
{
  public int Count { get; set; }
  public List<PDMLinq> Data { get; set; }
}
