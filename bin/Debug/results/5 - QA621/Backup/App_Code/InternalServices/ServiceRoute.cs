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
public class ServiceRoute
{
  protected DataClassesDataContext dc = new DataClassesDataContext(AzzierData.LinqConnectstring);
  protected string lookcriteria, mode = "", login_user = "";
  protected List<string> division = new List<string>();
  
  /// <summary>
  /// update Route
  /// </summary>
  /// <param name="xmlnvc"></param>
  /// <returns></returns>
  /// 
  [OperationContract]
  [WebGet]
  public string UpdateRoute(string xml)
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
    doc.LoadXml(xml);

    XmlNode root = doc.DocumentElement.FirstChild;

    XmlNodeList wolist = root.ChildNodes;

    for (int i = 0; i < wolist.Count; i++)
    {
      string name = wolist[i].Name;
      string val = wolist[i].InnerText;
      nvc.Add(name, val);
    }

    Route objroute;

    string routename = nvc["routename"];
    objroute = new Route(o.ToString(), "Route", "RouteName", routename);
    success = objroute.Update(nvc);
    if (success)
    {
      returnStr = "TRUE^" + nvc["routename"];
    }
    else
    {
      string err = "FALSE^" + objroute.ErrorMessage.Replace("\r\n", "");
      returnStr = err;
    }

    return returnStr;
  }

  /// <summary>
  /// Create new route
  /// </summary>
  /// <param name="userid"></param>
  /// <param name="xmlnvc"></param>
  /// <returns></returns>
  [OperationContract]
  [WebGet]
  public string CreateRoute(string userid, string xml)
  {
    string returnStr = "";
    bool success = false;
    userid = userid.ToUpper();
    NameValueCollection nvc;
    nvc = new NameValueCollection();

    XmlDocument doc = new XmlDocument();
    doc.LoadXml(xml);

    XmlNode root = doc.DocumentElement.FirstChild;

    XmlNodeList wolist = root.ChildNodes;

    for (int i = 0; i < wolist.Count; i++)
    {
      string name = wolist[i].Name;
      string val = wolist[i].InnerText;
      nvc.Add(name, val);
    }

    Route objroute;

    objroute = new Route(userid, "Route", "RouteName");
    success = objroute.Create(nvc);

    if (success)
    {
      returnStr = nvc["RouteName"];
    }
    else
      returnStr = objroute.ErrorMessage;

    return returnStr;

  }

  [OperationContract]
  [WebGet]
  public string DuplicateRoute(string userid, string xml, string oldroutename)  
  {
    string returnStr = "";
    bool success = false;
    userid = userid.ToUpper();
    NameValueCollection nvc;
    nvc = new NameValueCollection();

    XmlDocument doc = new XmlDocument();
    doc.LoadXml(xml);

    XmlNode root = doc.DocumentElement.FirstChild;

    XmlNodeList wolist = root.ChildNodes;

    for (int i = 0; i < wolist.Count; i++)
    {
      string name = wolist[i].Name;
      string val = wolist[i].InnerText;
      nvc.Add(name, val);
    }

    Route objroute;

    objroute = new Route(userid, "Route", "RouteName");
    success = objroute.DuplicateRoute(nvc,oldroutename);

    if (success)
    {
      returnStr = nvc["routename"];
    }
    else
      returnStr = objroute.ErrorMessage;

    return returnStr;

  }

  [OperationContract]
  [WebGet]
  public string DeleteRoute(string userid, string routename)
  {
    string returnStr = "";
    bool success = false;
    userid = userid.ToUpper();
    NameValueCollection nvc;
    nvc = new NameValueCollection();

    Route objroute = new Route(userid, "Route", "RouteName",routename);
    success = objroute.Delete();

    if (success)
    {
      return "OK";
    }
    else
      return objroute.ErrorMessage;

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
  public ResultRouteLinq GetWorkList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {

      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
      ResultRouteLinq result = new ResultRouteLinq();

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
                join p in dc.RouteLinqs on u.LinkId equals p.RouteName
                where p.Inactive == 0 && u.LinkModule.ToLower() == "route" && u.LinkType.ToLower() == "worklist" && u.UserId.ToLower() == login_user.ToLower()
                orderby u.LinkId
                select p;

    GridLinqBindingData<RouteLinq> data = RadGrid.GetBindingData<RouteLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
    result.Data = data.Data.ToList<RouteLinq>();
    result.Count = data.Count;

    return result;
  }

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

      var query = from v in dc.v_RouteRecentLists where v.UserId.ToLower()==login_user.ToLower() 
                  orderby v.AccessDate descending 
                  select v;

      GridLinqBindingData<v_RouteRecentList> resultData = RadGrid.GetBindingData<v_RouteRecentList>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
      result.Data = resultData.Data.ToList<v_RouteRecentList>();
      result.Count = resultData.Count;

      return result;
  }

  /*
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

    

    var query = from e in dc.PDMLinqs
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
   * */

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
                           where c.LinkModule.ToLower() == "route" && c.LinkType.ToLower() == "worklist" && c.UserId.ToLower() == login_user.ToLower()
                           select c.LinkId).Contains(id.Value)
                   select id.Value;

    var ins = from id in dc.RouteLinqs
              where idresult.ToList().Contains(id.RouteName)
              select new
              {
                LinkId = id.RouteName,
                UserId = login_user,
                LinkModule = "Route",
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
  public ResultRouteLinq SearchDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {
      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
      ResultRouteLinq result = new ResultRouteLinq();
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
      var query = from p in dc.RouteLinqs select p;
      GridLinqBindingData<RouteLinq> data = RadGrid.GetBindingData<RouteLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

  //    GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "PDMLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
      result.Data = data.Data.OfType<RouteLinq>().ToList();
      result.Count = data.Count;

      return result;
  }

  [OperationContract]
  [WebGet]
  public ResultRouteLinq QueryResultRoute(int startRowIndex, int maximumRows, string sortExpression, string filterExpression, string jsonData)
  {
      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
      ResultRouteLinq result = new ResultRouteLinq();
      string searchcriteria;
      List<string> eqptlist = new List<string>();

      searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";

      NameValueCollection nvc = AzzierData.DeserializeJSON(jsonData);
      object empid = HttpContext.Current.Session["LogIn"];
      string userid = (empid != null) ? empid.ToString() : "";
      Route objRoute = new Route(userid, "Route", "RouteName");
      string wherestring = "", linqwherestring = "";
      if (jsonData.Contains("queryID"))
        nvc = UserQuery.CreateNameValueCollection(nvc["queryID"]);
      objRoute.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
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
      var query = from p in dc.RouteLinqs select p;
      GridLinqBindingData<RouteLinq> data = RadGrid.GetBindingData<RouteLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

      //    GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "PDMLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
      result.Data = data.Data.OfType<RouteLinq>().ToList();
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
                     where idresult.ToList().Contains(userwl.LinkId) && userwl.LinkModule.ToLower() == "route" &&
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
  public string RemoveRouteDetail(string counterlist)
  {
    string returnStr = "";
    string userid = "";
    HttpContext context = HttpContext.Current;
    object o = context.Session["Login"];
    if (o == null)
    {
      returnStr = "Not authorized.";
      return returnStr;
    }
    else
    {
      userid = o.ToString();
      if (!ValidateUser(userid))
      {
        returnStr = "Security mismatched.";
        return returnStr;
      }
    }

    Route objroute = new Route(userid, "Route", "RouteName");
    if (objroute.BatchRemoveDetail(counterlist))
      return "OK";
    else
      return objroute.ErrorMessage;
  }


  [OperationContract]
  [WebGet]
  public string AddRouteDetail(string routename,string xml)
  {
    string returnStr = "";
    string userid = "";
    HttpContext context = HttpContext.Current;
    object o = context.Session["Login"];
    if (o == null)
    {
      returnStr = "Not authorized.";
      return returnStr;
    }
    else
    {
      userid = o.ToString();
      if (!ValidateUser(userid))
      {
        returnStr = "Security mismatched.";
        return returnStr;
      }
    }

    NameValueCollection nvcheader;
    nvcheader = new NameValueCollection();

    XmlDocument doc = new XmlDocument();
    doc.LoadXml(xml);

    //XmlNode root = doc.DocumentElement.FirstChild;


    XmlNodeList nodelist = doc.DocumentElement.ChildNodes;

    List<NameValueCollection> nvcdetails = new List<NameValueCollection>();
    for (int i = 0; i < nodelist.Count; i++)
    {
      NameValueCollection nvc = new NameValueCollection();
      XmlNode node = nodelist[i];
      for (int j = 0; j < node.ChildNodes.Count; j++)
      {
        string name = node.ChildNodes[j].Name;
        string val = node.ChildNodes[j].InnerText;
        nvc.Add(name, val);
      }
      nvc.Add("RouteName", routename);
      nvcdetails.Add(nvc);
    }

    Route objroute = new Route(userid, "Route","RouteName",routename);
    bool ok = true;
    for (int i = 0; i < nvcdetails.Count && ok; i++)
    {
      objroute.AddDetail(nvcdetails[i]);
    }
   
    if (!ok)
      return objroute.ErrorMessage;
    else
      return "OK";
  }

  [OperationContract]
  [WebGet]
  public string UpdateOrder(string routename, string equipment, string idorder, string xml)
  {
    string returnStr = "";
    string userid = "";
    HttpContext context = HttpContext.Current;
    object o = context.Session["Login"];
    if (o == null)
    {
      returnStr = "Not authorized.";
      return returnStr;
    }
    else
    {
      userid = o.ToString();
      if (!ValidateUser(userid))
      {
        returnStr = "Security mismatched.";
        return returnStr;
      }
    }

    XmlDocument doc = new XmlDocument();
    doc.LoadXml(xml);

    XmlNodeList nodelist = doc.DocumentElement.ChildNodes;

    List<NameValueCollection> nvcdetails = new List<NameValueCollection>();
    for (int i = 0; i < nodelist.Count; i++)
    {
      NameValueCollection nvc = new NameValueCollection();
      XmlNode node = nodelist[i];
      for (int j = 0; j < node.ChildNodes.Count; j++)
      {
        string name = node.ChildNodes[j].Name;
        string val = node.ChildNodes[j].InnerText;
        nvc.Add(name, val);
      }
      nvcdetails.Add(nvc);
    }

    Route objroute = new Route(userid, "Route", "RouteName", routename);
    bool ok = true;
    for (int i = 0; i < nvcdetails.Count && ok; i++)
    {
      ok = objroute.UpdateTagOrder(nvcdetails[i]["Counter"], nvcdetails[i]["TagOrder"]);
    }

    if (ok)
    {
      ok = objroute.UpdateIdOrder(equipment, idorder);
    }
    else
    {
      return objroute.ErrorMessage;
    }

    if (!ok)
      return objroute.ErrorMessage;
    else
      return "OK";
  }

  public class ResultRecentlist
  {
      public int Count { get; set; }
      public List<v_RouteRecentList> Data { get; set; }
  }

  /*
  [OperationContract]
  public ResultRouteDetailLinq ClientGetRouteDetail(string routename)
  {
    var query = from d in dc.RouteDetailLinqs
                where d.RouteName.ToLower() == routename
                select d;
    GridLinqBindingData<RouteDetailLinq> data = RadGrid.GetBindingData<RouteDetailLinq>(query, 0, 100, "", "");

    //    GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "PDMLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
    result.Data = data.Data.OfType<RouteLinq>().ToList();
    result.Count = data.Count;

    return result;
  }
   * */

  [OperationContract]
  public ResultRouteDetailLinq GetRouteDetail(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {
    System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
    ResultRouteDetailLinq result = new ResultRouteDetailLinq();
    string login_user = "";
    string routename = HttpContext.Current.Request.QueryString.Get("routename") ?? "";

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

    var query = from d in dc.v_RouteReadings
                where d.RouteName.ToLower() == routename.ToLower()
                orderby d.idorder,d.Equipment,d.tagorder,d.spectag
                select d;

    GridLinqBindingData<v_RouteReading> resultData = RadGrid.GetBindingData<v_RouteReading>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
    result.Data = resultData.Data.ToList<v_RouteReading>();
    result.Count = resultData.Count;

    return result;
  }
  
  [OperationContract]
  public ResultAvailableAttribute GetAvailableRoute(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {
    System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
    ResultAvailableAttribute result = new ResultAvailableAttribute();
    string login_user = "";
    string routename = HttpContext.Current.Request.QueryString.Get("routename") ?? "";
    string wherestr = HttpContext.Current.Request.QueryString.Get("wherestr") ?? "";

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
    var existing = from r in dc.RouteDetailLinqs
                   where r.RouteName.ToLower() == routename.ToLower()
                   select r;

    var query = from a in dc.v_AvailableAttributes
                select a;

    var list = from a in query
               join d in existing on new { readingtype = Convert.ToString(a.ReadingType), equipment = Convert.ToString(a.Equipment), spectag = Convert.ToString(a.Attribute) }
               equals new { readingtype = Convert.ToString(d.ReadingType), equipment = Convert.ToString(d.Equipment), spectag = Convert.ToString(d.SpecTag) } into newlist
               from nl in newlist.DefaultIfEmpty()
               where nl  == null
               select a;
    
    GridLinqBindingData<v_AvailableAttribute> resultData = RadGrid.GetBindingData<v_AvailableAttribute>(list, startRowIndex, maximumRows, sortExpression, filterExpression);
    result.Data = resultData.Data.ToList<v_AvailableAttribute>();
    result.Count = resultData.Count;
    /*
    join d in detail on new { readingtype = a.ReadingType.ToString(), equipment = a.equipment.ToString(), specttag = a.Attribute.ToString() }
               equals new { readingtype = d.ReadingType.ToString(), equipment = d.Equipment.ToString(), spectag = d.SpecTag.ToString() } into newlist
     * */
    /*
    var list = from a in available
               join d in detail on new { readingtype = Convert.ToString(a.ReadingType), equipment = Convert.ToString(a.equipment), spectag = Convert.ToString(a.Attribute) }
               equals new { readingtype = Convert.ToString(d.ReadingType), equipment = Convert.ToString(d.Equipment), spectag = Convert.ToString(d.SpecTag) } into newlist
               from nl in newlist.DefaultIfEmpty()
               select nl;
     * */

    return result;
  }
   
}

public class ResultRouteLinq
{
  public int Count { get; set; }
  public List<RouteLinq> Data { get; set; }
}

public class ResultRouteDetailLinq
{
  public int Count { get; set; }
  public List<v_RouteReading> Data { get; set; }
}

public class ResultAvailableAttribute
{
  public int Count { get; set; }
  public List<v_AvailableAttribute> Data { get; set; }
}

