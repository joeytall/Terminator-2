using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Xml.Linq;
using Telerik.Web.UI;
using System.Collections.Specialized;
using System.Xml;
/// <summary>
/// Summary description for ServiceREQ
/// </summary>

[ServiceContract(Namespace = "")]
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class ServiceREQ
{
  protected DataClassesDataContext dc = new DataClassesDataContext(AzzierData.LinqConnectstring);
	
  [OperationContract]
  public void DoWork()
	{
		//
		// TODO: Add constructor logic here
		//
	}

  [OperationContract]
  [WebGet]
  public string SaveItemRequestKeys(string xml)
  {
      bool success = false;
      string userid = "";
      HttpContext context = HttpContext.Current;

      object o = context.Session["Login"];
      if (o == null)
      {
          return "Not authorized.";
      }
      else
      {
          userid = o.ToString();
      }
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

      ModuleoObject obj;
      if (nvc["Counter"] == "")
      {
          obj = new ModuleoObject(userid, "ItemRequestKeys", "Counter");
          success = obj.Create(nvc);
      }
      else
      {
          obj = new ModuleoObject(userid, "ItemRequestKeys", "Counter", nvc["Counter"]);
          success = obj.Update(nvc);
      }
      if (success)
          return "OK";
      else
          return obj.ErrorMessage;
  }

  [OperationContract]
  [WebGet]
  public string DeleteItemRequestKeys(string counter)
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
      }
      userid = userid.ToUpper();
      ModuleoObject q = new ModuleoObject(userid, "ItemRequestKeys", "Counter", counter);
      if (q.Delete())
          return "OK";
      else
          return q.ErrorMessage;
  }


  [OperationContract]
  [WebGet]
  public string CreateRequest(string userid, string xmlnvc)
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

      REQ objreq;

      objreq = new REQ(userid, "ItemRequest", "ReqNum");
      success = objreq.Create(nvc);

      if (success)
      {
          returnStr = nvc["reqnum"];
      }
      else
          returnStr = objreq.ErrorMessage;

      return returnStr;

  }

  [OperationContract]
  [WebGet]
  public string UpdateRequest(string xmlnvc)
  {
      string returnStr = "", userid = "";

      HttpContext context = HttpContext.Current;
      object o = null;
      if (context != null)
      {
          o = context.Session["Login"];
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

      REQ objreq;

      string reqnum = nvc["reqnum"];
      objreq = new REQ(o.ToString(), "ItemRequest", "ReqNum", reqnum);
      success = objreq.Update(nvc);
      if (success)
      {
          returnStr = "TRUE^" + nvc["reqnum"];
      }
      else
      {
          string err = "FALSE^" + objreq.ErrorMessage.Replace("\r\n", "");
          returnStr = err;
      }

      return returnStr;
  }

  [OperationContract]
  [WebGet]
  public string SignKeyRequest(string reqnum, string signtype)
  {
      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
      string returnStr = "";
      bool success = false;
      string userid = "";
      NameValueCollection nvc;
      nvc = new NameValueCollection();

      HttpContext context = HttpContext.Current;
      object o = null;
      if (context != null)
      {
          o = context.Session["Login"];
          if (o == null)
          {
              returnStr = "FALSE^Not authorized.";
              return returnStr;
          }
          else
              userid = o.ToString();
      }

      REQ objreq;

      objreq = new REQ(userid, "ItemRequest", "ReqNum",reqnum);
      success = objreq.SignKeyRequest(reqnum,signtype);

      if (success)
      {
          Employee objemp = new Employee(userid,"Employee","Empid",userid);
          returnStr = "OK^" + userid + "^" + (objemp.ModuleData["FirstName"] + " " + objemp.ModuleData["LastName"]).Trim() + "," + "SIGNED^" + DateTime.Today.ToShortDateString(); ;
      }
      else
          returnStr = "FALSE^" + objreq.ErrorMessage;

      return returnStr;

  }

  [OperationContract]
  public ResultsREQLinq worklistData(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {
    System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
    ResultsREQLinq result = new ResultsREQLinq();

    string loginUser = "";
    if (HttpContext.Current.Session["Login"] != null)
    {
      loginUser = HttpContext.Current.Session["Login"].ToString();
    }
    else
    {
      result.Count = 0;
      result.Data = null;
      return result;
    }

    var query = from u in dc.UserWorkListLinqs
                join p in dc.ItemRequestLinqs on u.LinkId equals p.ReqNum
                where u.UserId == loginUser && u.LinkModule.ToLower() == "requisition" && u.LinkType.ToLower() == "worklist"
                orderby p.ReqNum
                select p;

    GridLinqBindingData<ItemRequestLinq> resultData = RadGrid.GetBindingData<ItemRequestLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
    result.Data = resultData.Data.ToList<ItemRequestLinq>();
    result.Count = resultData.Count;

    return result;
  }

  [OperationContract]
  public ResultsREQLinq SearchListData(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {
    System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
    ResultsREQLinq result = new ResultsREQLinq();
    string loginUser = "", searchcriteria = "";

    if (HttpContext.Current.Session["Login"] != null)
    {
      loginUser = HttpContext.Current.Session["Login"].ToString();
    }
    else
    {
      result.Data = null;
      result.Count = 0;
      return result;
    }

    if (System.Web.HttpContext.Current.Request.QueryString.Get("wherestring") != null)
    {
      searchcriteria = System.Web.HttpContext.Current.Request.QueryString.Get("wherestring");
      searchcriteria = searchcriteria.Replace("'", "\"");
      if (!string.IsNullOrEmpty(filterExpression))
      {
        filterExpression = filterExpression + " and " + searchcriteria;
      }
      else
      {
        filterExpression = searchcriteria;
      }
    }

    var query = from p in dc.ItemRequestLinqs select p;

    GridLinqBindingData<ItemRequestLinq> resultData = RadGrid.GetBindingData<ItemRequestLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

    result.Data = resultData.Data.ToList<ItemRequestLinq>();
    result.Count = resultData.Count;

    return result;
  }

  [OperationContract]
  [WebGet]
  public ResultsREQLinq QueryResultReq(int startRowIndex, int maximumRows, string sortExpression, string filterExpression, string jsonData)
  {
      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
      ResultsREQLinq result = new ResultsREQLinq();

      string searchcriteria;
      List<string> strlist = new List<string>();

      searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";

      NameValueCollection nvc = AzzierData.DeserializeJSON(jsonData);
      object empid = HttpContext.Current.Session["LogIn"];
      string userid = (empid != null) ? empid.ToString() : "";
      ModuleoObject objReq = new ModuleoObject(userid, "ItemRequest", "ReqNum");
      string wherestring = "", linqwherestring = "";
      if (jsonData.Contains("queryID"))
        nvc = UserQuery.CreateNameValueCollection(nvc["queryID"]);
      objReq.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
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

      var query = from p in dc.ItemRequestLinqs select p;

      GridLinqBindingData<ItemRequestLinq> resultData = RadGrid.GetBindingData<ItemRequestLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

      result.Data = resultData.Data.ToList<ItemRequestLinq>();
      result.Count = resultData.Count;

      return result;
  }
  [OperationContract]
  [WebGet]
  public string getReqTotalAmount(string reqnum)
  {
      string retStr = "";
      var req = (from p in dc.ItemRequestLinqs where p.ReqNum == reqnum select p).SingleOrDefault();
      //retStr = "{\"total\":\"" + req.PoTotal.ToString() + "\",\"subtotal\":\"" + po.SubTotal.ToString() + "\",\"tax1\":\"" + po.Tax1Total.ToString() + "\",\"tax2\":\"" + po.Tax2Total.ToString() + "\"}";
      retStr = "{\"total\":\"" + req.TotalCost.ToString() + "\"}";
      return retStr;
  }

  [OperationContract]
  public ResultsReqLineLinq GetReqLine(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {
      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
      ResultsReqLineLinq result = new ResultsReqLineLinq();
      string loginUser = "", searchcriteria = "";

      if (HttpContext.Current.Session["Login"] != null)
      {
          loginUser = HttpContext.Current.Session["Login"].ToString();
      }
      else
      {
          result.Data = null;
          result.Count = 0;
          return result;
      }

      if (System.Web.HttpContext.Current.Request.QueryString.Get("wherestring") != null)
      {
          searchcriteria = System.Web.HttpContext.Current.Request.QueryString.Get("wherestring");
          searchcriteria = searchcriteria.Replace("'", "\"");

          if (!string.IsNullOrEmpty(filterExpression))
          {
              filterExpression = filterExpression + " and " + searchcriteria;
          }
          else
          {
              filterExpression = searchcriteria;
          }
      }

      var query = from p in dc.RequestLineLinqs select p;

      GridLinqBindingData<RequestLineLinq> resultData = RadGrid.GetBindingData<RequestLineLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

      result.Data = resultData.Data.ToList<RequestLineLinq>();
      result.Count = resultData.Count;

      return result;
  }

  [OperationContract]
  public ResultsItemRequestKeysLinq GetReqKeyLine(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {
      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
      ResultsItemRequestKeysLinq result = new ResultsItemRequestKeysLinq();
      string loginUser = "", searchcriteria = "";

      if (HttpContext.Current.Session["Login"] != null)
      {
          loginUser = HttpContext.Current.Session["Login"].ToString();
      }
      else
      {
          result.Data = null;
          result.Count = 0;
          return result;
      }

      if (System.Web.HttpContext.Current.Request.QueryString.Get("wherestring") != null)
      {
          searchcriteria = System.Web.HttpContext.Current.Request.QueryString.Get("wherestring");
          searchcriteria = searchcriteria.Replace("'", "\"");
          if (!string.IsNullOrEmpty(filterExpression))
          {
              filterExpression = filterExpression + " and " + searchcriteria;
          }
          else
          {
              filterExpression = searchcriteria;
          }
      }

      var query = from p in dc.ItemRequestKeysLinqs select p;

      GridLinqBindingData<ItemRequestKeysLinq> resultData = RadGrid.GetBindingData<ItemRequestKeysLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

      result.Data = resultData.Data.ToList<ItemRequestKeysLinq>();
      result.Count = resultData.Count;

      return result;
  }

  [OperationContract]
  public ResultREQRecentlist RecentlistDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {
    System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
    ResultREQRecentlist result = new ResultREQRecentlist();
    string loginUser = "";

    if (HttpContext.Current.Session["Login"] != null)
    {
      loginUser = HttpContext.Current.Session["Login"].ToString();
    }
    else
    {
      result.Data = null;
      result.Count = 0;
      return result;
    }

    var query = from v in dc.v_REQRecentLists where v.UserId.ToLower() == loginUser.ToLower() orderby v.AccessDate descending select v;
    GridLinqBindingData<v_REQRecentList> resultData = RadGrid.GetBindingData<v_REQRecentList>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
    result.Data = resultData.Data.ToList<v_REQRecentList>();
    result.Count = resultData.Count;

    return result;
  }

  [OperationContract]
  [WebGet]
  public string AddtoWorklist(string paramXML)
  {
    string retStr = "Success.", loginUser;
    if (System.Web.HttpContext.Current.Session["Login"] != null)
    {
      loginUser = System.Web.HttpContext.Current.Session["Login"].ToString();
    }
    else
    {
      return retStr = "Please Login.";
    }

    XDocument xdoc = XDocument.Parse(paramXML);

    var q = from r in xdoc.Descendants("reqnum")
            join p in dc.ItemRequestLinqs on r.Value equals p.ReqNum
            where !(from u in dc.UserWorkListLinqs
                    where u.LinkModule.ToLower() == "requisition" && u.LinkType.ToLower() == "worklist" && u.UserId == loginUser
                    select u.LinkId).Contains(r.Value)
            select r.Value;

    List<UserWorkListLinq> worklist = new List<UserWorkListLinq>();
    foreach (var reqnum in q)
    {
      worklist.Add(new UserWorkListLinq { LinkId = reqnum, LinkModule = "requisition", LinkType = "worklist", UserId = loginUser, TransDate = System.DateTime.Now });
    }
    try
    {
      dc.UserWorkListLinqs.InsertAllOnSubmit(worklist);
      dc.SubmitChanges();
    }
    catch (Exception e)
    {
      retStr = e.Message;
    }

    return retStr;
  }

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
  public string RemovefromWorklist(string paramXML)
  {
    string retStr = "Success.", loginUser;
    if (System.Web.HttpContext.Current.Session["Login"] != null)
    { 
      loginUser = System.Web.HttpContext.Current.Session["Login"].ToString(); 
    }
    else 
    { 
      return "Please login."; 
    }

    XDocument reqnum = XDocument.Parse(paramXML);

    var worklist = from u in dc.UserWorkListLinqs
                   where u.LinkModule.ToLower() == "requisition" && u.UserId == loginUser && u.LinkType.ToLower() == "worklist" &&
                   (from p in reqnum.Descendants("reqnum") select p.Value).Contains(u.LinkId)
                   select u;

    try
    {
      dc.UserWorkListLinqs.DeleteAllOnSubmit(worklist);
      dc.SubmitChanges();
    }
    catch (Exception e)
    { 
      retStr = e.Message; 
    }

    return retStr;
  }

  public class ResultREQRecentlist
  {
    public int Count { get; set; }
    public List<v_REQRecentList> Data { get; set; }
  }
}

public class ResultsREQLinq
{
  public int Count { get; set; }
  public List<ItemRequestLinq> Data { get; set; }
}

public class ResultsReqLineLinq
{
    public int Count { get; set; }
    public List<RequestLineLinq> Data { get; set; }
}

public class ResultsItemRequestKeysLinq
{
    public int Count { get; set; }
    public List<ItemRequestKeysLinq> Data { get; set; }
}
