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
public class ServicePM
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
  [WebGet]
  public string GeneratePM(string pmnums, string pmtype, string days, string orderbyfield, string sortorder)
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

    PM objpm = new PM(userid,"PM","PMNum");
    string[] pmnumlist;
    List<string> allwonums = new List<string>();
    if (pmnums != "")
    {
      pmnumlist = pmnums.Split(',');
      for (int i = 0; i < pmnumlist.Length; i++)
      {
        List<string> wonums = objpm.GeneratePM(pmnumlist[i], pmtype, Convert.ToInt16(days));
        if (wonums.Count > 0)
        {
          allwonums.AddRange(wonums);
        }
      }
    }
    else
    {
      allwonums = objpm.GenerateAllPM(pmtype, Convert.ToInt16(days));
    }

    var result = String.Join(",", allwonums);
    return result;
  }

  [OperationContract]
  [WebGet]
  public string SetSequence(string pmnum, string sequence)
  {
    string returnStr = "", userid = "";

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

    PM objpm = new PM(userid, "PM", "PMNum",pmnum);

    if (objpm.SetSequence(pmnum, Convert.ToInt16(sequence)))
    {
      returnStr = "OK";
    }
    else
    {
      returnStr = "Set sequence failed:" + objpm.ErrorMessage;
    }

    return returnStr;
  }



    /// <summary>
    /// update pm
    /// </summary>
    /// <param name="xmlnvc"></param>
    /// <returns></returns>
    /// 
    [OperationContract]
    [WebGet]
    public string UpdatePM(string xmlnvc)
    {
        string returnStr = "", oldprocnum = "", newprocnum = "", userid = "";

        HttpContext context = HttpContext.Current;

        if (context != null)
        {
            if (context.Request.Headers["hidprocnum"] != null)
                oldprocnum = context.Request.Headers["hidprocnum"].ToUpper();
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
            if (name.ToLower() != "nestedpm")
              nvc.Add(name, val);
        }

        PM objPM;

        string pmnum = nvc["pmnum"];
        objPM = new PM(o.ToString(), "PM", "PMNum", pmnum);
        success = objPM.Update(nvc);
        if (success)
        {
            //string newprocnum;
            if (nvc["procnum"] == null)
                newprocnum = "";
            else newprocnum = nvc["procnum"].ToString().ToUpper();

            //string newprocnum = nvc["procnum"];
            //if (newprocnum == null)
            //    newprocnum = "";

            if ((newprocnum != "") && (newprocnum != oldprocnum))
            {
                Labour l = new Labour(userid, "WOLabour", "Counter");
                l.DeleteAllLabour("pm", nvc["pmnum"], 1);
                l.CopyLabour("procedure", nvc["procnum"], "pm", nvc["pmnum"]);
                Service s = new Service(userid, "WOService", "Counter");
                s.DeleteAllService("pm", nvc["pmnum"], 1);
                s.CopyService("procedure", nvc["procnum"], "pm", nvc["pmnum"]);
                Material m = new Material(userid, "WOMaterial", "Counter");
                m.DeleteAllMaterial("pm", nvc["pmnum"], 1);
                m.CopyMaterial("procedure", nvc["procnum"], "pm", nvc["pmnum"]);
                Tool t = new Tool(userid, "WOTools", "Counter");
                t.DeleteAllTools("pm", nvc["pmnum"], 1);
                t.CopyTools("procedure", nvc["procnum"], "pm", nvc["pmnum"]);
                Task task = new Task(userid, "WOTasks", "Counter");
                task.DeleteAllTask("pm", nvc["pmnum"], 1);
                task.CopyTasks("procedure", nvc["procnum"], "pm", nvc["pmnum"]);
                CostAccount ca = new CostAccount(userid, "CostAccounts", "Counter");
                ca.CopyAccounts("procedure", nvc["procnum"], "pm", nvc["pmnum"]);
            }
            returnStr = "TRUE^" + nvc["pmnum"];
        }
        else
        {
            string err = "FALSE^" + objPM.ErrorMessage.Replace("\r\n", "");
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
    public string CreatePM(string userid, string xmlnvc)
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
          if (name.ToLower()!="nestedpm")
            nvc.Add(name, val);
        }

        PM objPM;

        objPM = new PM(userid, "PM", "PMNum");
        success = objPM.Create(nvc);

        if (success)
        {
            if (nvc["procnum"] != "")
            {
                Labour l = new Labour(userid, "WOLabour", "Counter");
                l.CopyLabour("procedure", nvc["procnum"], "pm", nvc["pmnum"]);
                Service s = new Service(userid, "WOService", "Counter");
                s.CopyService("procedure", nvc["procnum"], "pm", nvc["pmnum"]);
                Material m = new Material(userid, "WOMaterial", "Counter");
                m.CopyMaterial("procedure", nvc["procnum"], "pm", nvc["pmnum"]);
                Tool t = new Tool(userid, "WOTools", "Counter");
                t.CopyTools("procedure", nvc["procnum"], "pm", nvc["pmnum"]);
                Task task = new Task(userid, "WOTasks", "Counter");
                task.CopyTasks("procedure", nvc["procnum"], "pm", nvc["pmnum"]);
                CostAccount ca = new CostAccount(userid, "CostAccounts", "Counter");
                ca.CopyAccounts("procedure", nvc["procnum"], "pm", nvc["pmnum"]);
            }
            returnStr = nvc["pmnum"];
        }

        return returnStr;

    }

    [OperationContract]
    [WebGet]
    public string DuplicatePM(string userid, string xmlnvc, string oldpmnum, string resourcesource = "PM")  //resource source is "pm" or "procedure", if pm, then copy resources from pm , otherwise from procedure
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
        if (name.ToLower()!="nestedpm")
          nvc.Add(name, val);
      }

      PM objPM;
      PM objoldpm = new PM(userid, "PM", "PMNum", oldpmnum);
      nvc.Add("nestedpm", objoldpm.ModuleData["nestedpm"]);
       
      objPM = new PM(userid, "PM", "PMNum");
      success = objPM.Create(nvc);

      if (success)
      {
        if (resourcesource.ToLower() == "procedure")
        {
          if (nvc["procnum"] != "")
          {
            Labour l = new Labour(userid, "WOLabour", "Counter");
            l.CopyLabour("procedure", nvc["procnum"], "pm", nvc["pmnum"]);
            Service s = new Service(userid, "WOService", "Counter");
            s.CopyService("procedure", nvc["procnum"], "pm", nvc["pmnum"]);
            Material m = new Material(userid, "WOMaterial", "Counter");
            m.CopyMaterial("procedure", nvc["procnum"], "pm", nvc["pmnum"]);
            Tool t = new Tool(userid, "WOTools", "Counter");
            t.CopyTools("procedure", nvc["procnum"], "pm", nvc["pmnum"]);
            Task task = new Task(userid, "WOTasks", "Counter");
            task.CopyTasks("procedure", nvc["procnum"], "pm", nvc["pmnum"]);
            CostAccount c = new CostAccount(userid, "CostAccounts", "Counter");
            c.CopyAccounts("procedure", nvc["procnum"], "pm", nvc["pmnum"]);
          }
        }
        else
        {
          Labour l = new Labour(userid, "WOLabour", "Counter");
          l.CopyLabour("pm", oldpmnum, "pm", nvc["pmnum"]);
          Service s = new Service(userid, "WOService", "Counter");
          s.CopyService("pm", oldpmnum, "pm", nvc["pmnum"]);
          Material m = new Material(userid, "WOMaterial", "Counter");
          m.CopyMaterial("pm", oldpmnum, "pm", nvc["pmnum"]);
          Tool t = new Tool(userid, "WOTools", "Counter");
          t.CopyTools("pm", oldpmnum, "pm", nvc["pmnum"]);
          Task task = new Task(userid, "WOTasks", "Counter");
          task.CopyTasks("pm", oldpmnum, "pm", nvc["pmnum"]);
          CostAccount c = new CostAccount(userid, "CostAccounts", "Counter");
          c.CopyAccounts("pm", oldpmnum, "pm", nvc["pmnum"]);
        }
        objPM.CopyOffSeason(oldpmnum, nvc["pmnum"]);
        if (nvc["NestedPM"] != "0")
          objPM.CopyNestedProc(oldpmnum, nvc["PMNum"]);

        returnStr = nvc["pmnum"];
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
    public ResultPMLinq GetWorkList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
      ResultPMLinq result = new ResultPMLinq();

      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
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
                  join p in dc.PMLinqs on u.LinkId equals p.PmNum
                  where p.Inactive == 0 && u.LinkModule.ToLower() == "pm" && u.LinkType.ToLower() == "worklist" && u.UserId.ToLower() == login_user.ToLower()
                  orderby u.LinkId
                  select p;

      GridLinqBindingData<PMLinq> data = RadGrid.GetBindingData<PMLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
      result.Data = data.Data.ToList<PMLinq>();
      result.Count = data.Count;

      return result;
    }

    [OperationContract]
    public RecentWorklistResult RecentListDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        RecentWorklistResult result = new RecentWorklistResult();

        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
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

        var q = from v in dc.v_PMRecentlists where v.UserId == login_user orderby v.AccessDate descending select v;

        GridLinqBindingData<v_PMRecentlist> resultData = RadGrid.GetBindingData<v_PMRecentlist>(q, startRowIndex, maximumRows, sortExpression, filterExpression);

        result.Data = resultData.Data.ToList<v_PMRecentlist>();
        result.Count = resultData.Count;

        return result;
    }

    //[OperationContract]
    //public ResultPMLinq GetRecentList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    //{        //???
    //  ResultPMLinq result = new ResultPMLinq();
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

    //  var qq = from p in dc.PMLinqs
    //           join u in dc.UserWorkListLinqs on p.PmNum equals u.LinkId
    //           where u.LinkType.ToLower()=="recent" && u.UserId.ToLower()==login_user.ToLower() && u.LinkModule.ToLower()=="pm"
    //           orderby u.TransDate descending
    //           select p;

    //  GridLinqBindingData<PMLinq> resultData = RadGrid.GetBindingData<PMLinq>(qq, startRowIndex, maximumRows, sortExpression, filterExpression);
    //  result.Data = resultData.Data.ToList<PMLinq>();
    //  result.Count = resultData.Count;

    //  return result;
    //}

    [OperationContract]
    public ResultPMLinq LookupDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        string lookupcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";
        string mode = HttpContext.Current.Request.QueryString.Get("mode") ?? "";

        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
      ResultPMLinq result = new ResultPMLinq();
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
      /*
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

      var query = from e in dc.PMLinqs
                  where division.Contains(e.Division) || e.Division.Equals(null)
                  select e;

      if (mode != "query")
      {
        query = query.Where(a => a.Inactive == 0);
      }*/

      //var query = from e in dc.PMLinqs
      //            where division.Contains(e.Division) || e.Division.Equals(null)
      //            select e;

      var query = from e in dc.PMLinqs
                  select e;

      GridLinqBindingData<PMLinq> data = RadGrid.GetBindingData<PMLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
      result.Data = data.Data.OfType<PMLinq>().ToList();
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
                             where c.LinkModule.ToLower() == "pm" && c.LinkType.ToLower() == "worklist" && c.UserId.ToLower() == login_user.ToLower()
                             select c.LinkId).Contains(id.Value)
                     select id.Value;

      var ins = from id in dc.PMLinqs
                where idresult.ToList().Contains(id.PmNum)
                select new
                {
                  LinkId = id.PmNum,
                  UserId = login_user,
                  LinkModule = "PM",
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
    public ResultPMLinq SearchDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
      ResultPMLinq result = new ResultPMLinq();
      string searchcriteria;
      List<string> eqptlist = new List<string>();

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

      var query = from p in dc.PMLinqs select p;
      GridLinqBindingData<PMLinq> data = RadGrid.GetBindingData<PMLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

//      GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "PMLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
      result.Data = data.Data.OfType<PMLinq>().ToList();
      result.Count = data.Count;

      return result;
    }
    
    [OperationContract]
    [WebGet]
    public ResultPMLinq QueryResultPM(int startRowIndex, int maximumRows, string sortExpression, string filterExpression, string jsonData)
    {
      ResultPMLinq result = new ResultPMLinq();
      string searchcriteria;
      List<string> eqptlist = new List<string>();

      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
      searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";

      NameValueCollection nvc = AzzierData.DeserializeJSON(jsonData);
      object empid = HttpContext.Current.Session["LogIn"];
      string userid = (empid != null) ? empid.ToString() : "";
      PM objPM = new PM(userid, "PM", "PmNum");
      string wherestring = "", linqwherestring = "";
      if (jsonData.Contains("queryID"))
        nvc = UserQuery.CreateNameValueCollection(nvc["queryID"]);
      objPM.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
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

      var query = from p in dc.PMLinqs select p;
      GridLinqBindingData<PMLinq> data = RadGrid.GetBindingData<PMLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

//      GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "PMLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
      result.Data = data.Data.OfType<PMLinq>().ToList();
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
          var result = from userwl in dc.UserWorkListLinqs
                       where idresult.ToList().Contains(userwl.LinkId) && userwl.LinkModule.ToLower() == "pm" &&
                       userwl.LinkType == "worklist" && userwl.UserId.ToLower() == login_user.ToLower()
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
    public string DoWork()
    {
      return "OK";
    }

    public class RecentWorklistResult
    {
       public int Count { get; set; }
       public List<v_PMRecentlist> Data { get; set; }
    }
}

public class ResultPMLinq
{
  public int Count { get; set; }
  public List<PMLinq> Data { get; set; }
}
