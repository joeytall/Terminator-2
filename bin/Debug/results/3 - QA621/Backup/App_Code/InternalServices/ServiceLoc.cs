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
using System.Globalization;

using Telerik.Web.UI;

using System.Runtime.Serialization;

[ServiceContract(Namespace = "")]
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class ServiceLoc
{

    protected DataClassesDataContext dc = new DataClassesDataContext(AzzierData.LinqConnectstring);

    //// To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
    //// To create an operation that returns XML,
    ////     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
    ////     and include the following line in the operation body:
    ////         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
    //[OperationContract]
    //public void DoWork()
    //{
    //    // Add your operation implementation here
    //    return;
    //}

    // Add more operations here and mark them with [OperationContract]


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
                         userwl.LinkType == "worklist" && userwl.UserId == login_user
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
    public string AddtoWorkList(string paramXML)
    {
        string retstr = "SUCCESS.";
        //string KeyList = "";

        //XDocument doc = new XDocument();
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

        //string retstr = "SUCCESS.";
        XDocument doc = XDocument.Parse(paramXML);
        var idresult = from id in doc.Descendants("IDS")
                       where !(from c in dc.UserWorkListLinqs
                               where c.LinkModule.ToLower() == "location" && c.LinkType.ToLower() == "worklist" && c.UserId.ToLower() == login_user.ToLower()
                               select c.LinkId).Contains(id.Value)
                       select id.Value;

        var ins = from id in dc.LocationLinqs
                  where idresult.ToList().Contains(id.Location)
                  select new
                  {
                      LinkId = id.Location,
                      UserId = login_user,
                      LinkModule = "Location",
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
    public ResultLocationRecentlist RecentListDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultLocationRecentlist result = new ResultLocationRecentlist();

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

        var query = from v in dc.v_LocationRecentlists where v.UserId.ToLower() == login_user.ToLower() orderby v.AccessDate descending select v;

        GridLinqBindingData<v_LocationRecentlist> resultData = RadGrid.GetBindingData<v_LocationRecentlist>(query,startRowIndex,maximumRows,sortExpression,filterExpression);
        result.Data=resultData.Data.ToList<v_LocationRecentlist>();
        result.Count = resultData.Count;
        return result;
    }


    [OperationContract]
    public ResultLocationLinq WorkListDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultLocationLinq result = new ResultLocationLinq();

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
                    join e in dc.LocationLinqs on u.LinkId equals e.Location
                    where e.Inactive == 0 && u.LinkModule.ToLower() == "location" && u.LinkType.ToLower() == "worklist" && u.UserId.ToLower()==login_user.ToLower()
                    orderby e.Location
                    select e;

        GridLinqBindingData<LocationLinq> data = RadGrid.GetBindingData<LocationLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = data.Data.ToList<LocationLinq>();
        result.Count = data.Count;

        return result;
    }

    /// <summary>
    /// Equipment module: top menu button -- "Look Up"
    /// </summary>
    /// <param name="startRowIndex"></param>
    /// <param name="maximumRows"></param>
    /// <param name="sortExpression"></param>
    /// <param name="filterExpression"></param>
    /// <returns></returns>
    [OperationContract]
    public ResultLocationLinq SearchDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultLocationLinq result = new ResultLocationLinq();
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

        var query = from l in dc.LocationLinqs select l;
        GridLinqBindingData<LocationLinq> data = RadGrid.GetBindingData<LocationLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

//        GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "LocationLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = data.Data.OfType<LocationLinq>().ToList();
        //result.Data = data.Data.ToList<LocationLinq>();//.ToList<LocationLinq>();//.OfType<LocationLinq>().ToList();
        result.Count = data.Count;

        return result;
    }

    [OperationContract]
    [WebGet]
    public ResultLocationLinq QueryResultLocation(int startRowIndex, int maximumRows, string sortExpression, string filterExpression, string jsonData)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultLocationLinq result = new ResultLocationLinq();
        string searchcriteria;

        searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";

        NameValueCollection nvc = AzzierData.DeserializeJSON(jsonData);
        object empid = HttpContext.Current.Session["LogIn"];
        string userid = (empid != null) ? empid.ToString() : "";
        Location objLoc = new Location(userid, "Location", "Location");
        string wherestring = "", linqwherestring = "";
        if (jsonData.Contains("queryID"))
          nvc = UserQuery.CreateNameValueCollection(nvc["queryID"]);
        objLoc.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
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

        var query = from l in dc.LocationLinqs select l;
        GridLinqBindingData<LocationLinq> data = RadGrid.GetBindingData<LocationLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        //        GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "LocationLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = data.Data.OfType<LocationLinq>().ToList();
        //result.Data = data.Data.ToList<LocationLinq>();//.ToList<LocationLinq>();//.OfType<LocationLinq>().ToList();
        result.Count = data.Count;

        return result;
    }

    [OperationContract]
    public ResultLocationLinq SpecSearchDatandCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        string searchcriteria = "";
        List<string> laborlist = new List<string>();

        ResultLocationLinq result = new ResultLocationLinq();

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
            laborlist = new List<string>(searchcriteria.Split(','));
            if (laborlist.Count() > 0)
            {
                var query = from ca in dc.LocationLinqs
                            where ca.Inactive == 0 && laborlist.Contains(ca.Location) && (ca.Division == null ||
                            (from o in dc.UserDivisionLinqs where o.Empid == login_user select o.Division).Contains(ca.Division))
                            select ca;

                GridLinqBindingData<LocationLinq> resultData = RadGrid.GetBindingData<LocationLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
                result.Data = resultData.Data.ToList<LocationLinq>();
                result.Count = resultData.Count;
            }
            else
            {
                result.Data = null;
                result.Count = 0;
                return result;
            }
        }
        return result;
    }

    [OperationContract]
    public ResultLocationLinq LookupDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultLocationLinq result = new ResultLocationLinq();

        string lookupfilter =HttpContext.Current.Request.QueryString.Get("where");

        if (!string.IsNullOrEmpty(lookupfilter))
        {
            lookupfilter = lookupfilter.Replace("'","\"");
            if (!string.IsNullOrEmpty(filterExpression))
                filterExpression = filterExpression + " and " + lookupfilter;
            else
                filterExpression = lookupfilter;
        }

        var query = from loc in dc.LocationLinqs orderby loc.Location select loc;

        GridLinqBindingData<LocationLinq> resultData = RadGrid.GetBindingData<LocationLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = resultData.Data.ToList<LocationLinq>();
//        result.Data = resultData.Data.OfType<LocationLinq>().ToList();
        result.Count = resultData.Count;// query.Count();//800;// resultData.Count;

        return result;
    }

    private bool ConvertBool(string val)
    {
      if (val.ToLower() == "true" || val.ToLower() == "on")
        return true;
      else
        return false;
    }

    [OperationContract]
    [WebGet]
    public string ModifyLocation(string location, string newdivision, string newparentid, string newstatus, string newparentdesc, string moddate, string updateopen, string updatehistory)
    {
      string login_user = "";
      if (System.Web.HttpContext.Current.Session["Login"] != null)
      {
        login_user = HttpContext.Current.Session["Login"].ToString();
        if (login_user == "")
        {
          return "Session timed out.";
        }
      }
      else
      {
        return "Session time out.";
      }
      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
      Location loc = new Location(login_user, "Location", "Location", location);
      CultureInfo c = new CultureInfo(System.Web.HttpContext.Current.Session.LCID);
      DateTime TransDate = Convert.ToDateTime(moddate, c);


      if (loc.IsAncestor(newparentid, location) || (location == newparentid))
      {
          return "New parent loction is not valid parent loction.";
      }
   
      if (loc.ModifyLocation(newparentid, newparentdesc, newdivision, newstatus, ConvertBool(updateopen), ConvertBool(updatehistory), TransDate))
        return "OK";
      else
        return loc.ErrorMessage;
      
    }


    [OperationContract]
    [WebGet]
    public string CreateLocation(string userid, string xmlnvc, string oldlocation)
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

      Location objloc;

      objloc = new Location(userid, "Location", "Location");
      success = objloc.Create(nvc);

      if (success)
      {
        if (oldlocation != "")
        {
          Specification scopy = new Specification(userid, "Specification", "Counter");
          scopy.SpecCopy(oldlocation, nvc["location"], "Location");
        }
        returnStr = nvc["location"];
      }

      return returnStr;
    }

    [OperationContract]
    [WebGet]
    public string SaveLocation(string xmlnvc)
    {
      //string returnStr = "", oldprocnum = "", newprocnum = "", userid = "";
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

      Location objloc;

      string location = nvc["location"];
      objloc = new Location(o.ToString(), "Location", "Location", location);
      success = objloc.Update(nvc);
      if (success)
      {
        
        returnStr = "TRUE^" + nvc["location"];
      }
      else
      {
        string err = "FALSE^" + objloc.ErrorMessage.Replace("\r\n", "");
        returnStr = err;
      }

      return returnStr;
    }


    public class ResultLocationRecentlist
    {
        public Int32 Count { get; set; }
        public List<v_LocationRecentlist> Data { get; set; }
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
    public ResultDistrictsLinq GetDistrictsList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultDistrictsLinq result = new ResultDistrictsLinq();

        string lookupfilter = HttpContext.Current.Request.QueryString.Get("where");

        if (!string.IsNullOrEmpty(lookupfilter))
        {
            lookupfilter = lookupfilter.Replace("'", "\"");
            if (!string.IsNullOrEmpty(filterExpression))
                filterExpression = filterExpression + " and " + lookupfilter;
            else
                filterExpression = lookupfilter;
        }

        var query = from dis in dc.DistrictLinqs orderby dis.District select dis;

        GridLinqBindingData<DistrictLinq> resultData = RadGrid.GetBindingData<DistrictLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = resultData.Data.ToList<DistrictLinq>();
        //        result.Data = resultData.Data.OfType<LocationLinq>().ToList();
        result.Count = resultData.Count;// query.Count();//800;// resultData.Count;

        return result;
    }
}

public class ResultLocationLinq
{
    public Int32 Count { get; set; }
    public List<LocationLinq> Data { get; set; }
}

public class ResultDistrictsLinq
{
    public Int32 Count { get; set; }
    public List<DistrictLinq> Data { get; set; }
}

