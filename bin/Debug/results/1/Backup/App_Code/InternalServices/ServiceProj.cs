using System;
using System.Web;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using Telerik.Web.UI;
using System.Xml;
using System.Xml.Linq;

[ServiceContract(Namespace = "")]
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class ServiceProj
{
    protected DataClassesDataContext dc = new DataClassesDataContext(AzzierData.LinqConnectstring);
	// To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
	// To create an operation that returns XML,
	//     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
	//     and include the following line in the operation body:
	//         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
    [OperationContract]
    public ResultProjectLinq GetProjectList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultProjectLinq result = new ResultProjectLinq();
        string lookupfilter = HttpContext.Current.Request.QueryString.Get("where");

        if (!string.IsNullOrEmpty(lookupfilter))
        {
            lookupfilter = lookupfilter.Replace("'", "\"");
            if (!string.IsNullOrEmpty(filterExpression))
                filterExpression = filterExpression + " and " + lookupfilter;
            else
                filterExpression = lookupfilter;
        }

        var query = from proj in dc.ProjectLinqs select proj;
        query = query.OrderBy(x => x.ProjectId);
        GridLinqBindingData<ProjectLinq> resultData = RadGrid.GetBindingData<ProjectLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        result.Data = resultData.Data.ToList<ProjectLinq>();
        result.Count = resultData.Count;

        return result;
    }

    [OperationContract]
    [WebGet]
    public ResultProjectLinq QueryResultProject(int startRowIndex, int maximumRows, string sortExpression, string filterExpression, string jsonData)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultProjectLinq result = new ResultProjectLinq();
        string lookupfilter = HttpContext.Current.Request.QueryString.Get("where");

        NameValueCollection nvc = AzzierData.DeserializeJSON(jsonData);
        object empid = HttpContext.Current.Session["LogIn"];
        string userid = (empid != null) ? empid.ToString() : "";
        Project objProject = new Project(userid, "Projects", "ProjectId");
        string wherestring = "", linqwherestring = "";
        if (jsonData.Contains("queryID"))
          nvc = UserQuery.CreateNameValueCollection(nvc["queryID"]);
        objProject.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
        lookupfilter = linqwherestring;

        if (!string.IsNullOrEmpty(lookupfilter))
        {
            lookupfilter = lookupfilter.Replace("'", "\"");
            if (!string.IsNullOrEmpty(filterExpression))
                filterExpression = filterExpression + " and " + lookupfilter;
            else
                filterExpression = lookupfilter;
        }

        var query = from proj in dc.ProjectLinqs select proj;
        query = query.OrderBy(x => x.ProjectId);
        GridLinqBindingData<ProjectLinq> resultData = RadGrid.GetBindingData<ProjectLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        result.Data = resultData.Data.ToList<ProjectLinq>();
        result.Count = resultData.Count;

        return result;
    }

    [OperationContract]
    public ResultProjectRecentList RecentListDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultProjectRecentList result = new ResultProjectRecentList();

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

        var query = from v in dc.v_ProjectRecentLists where v.UserId.ToLower() == login_user.ToLower() orderby v.AccessDate descending select v;

        GridLinqBindingData<v_ProjectRecentList> resultData = RadGrid.GetBindingData<v_ProjectRecentList>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        result.Data = resultData.Data.ToList<v_ProjectRecentList>();
        result.Count = resultData.Count;

        return result;
    }

    [OperationContract]
    [WebGet]
    public string AddtoWorkList(string paramXML)
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
        var uwlif = from c in dc.UserWorkListLinqs
                    where c.LinkModule == "Project" && c.LinkType == "worklist" && c.UserId.ToLower() == login_user.ToLower()
                    select c.LinkId;
        var idresult = from id in doc.Descendants("IDS")
                       where !(from c in dc.UserWorkListLinqs
                               where c.LinkModule == "Project" && c.LinkType == "worklist" && c.UserId.ToLower() == login_user.ToLower()
                               select c.LinkId).Contains(id.Value)
                       select id.Value;
        var ins = from id in dc.ProjectLinqs
                  where idresult.ToList().Contains(id.ProjectId)
                  select new
                  {
                      LinkId = id.ProjectId,
                      UserId = login_user,
                      LinkModule = "Project",
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
    [WebGet]
    public string RemoveFromWorkList(string paramXML)
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

        try
        {
            var result = from userwl in dc.UserWorkListLinqs
                         where idresult.ToList().Contains(userwl.LinkId) && userwl.LinkModule == "Project" &&
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
    public ResultProjHistoryLinq GetProjectHistoryList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        ResultProjHistoryLinq result = new ResultProjHistoryLinq();

        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

        string projectid = HttpContext.Current.Request.QueryString.Get("projectid");

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

        }


        if (projectid == null || projectid == "")
        {
            return result;
        }

        var query = dc.ProjHistoryLinqs.Where(x => x.ProjectId == projectid).OrderByDescending(x=>x.TransDate);
        GridLinqBindingData<ProjHistoryLinq> data = RadGrid.GetBindingData<ProjHistoryLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = data.Data.ToList<ProjHistoryLinq>();
        result.Count = data.Count;
        return result;
    }

    [OperationContract]
    public ResultProjectLinq GetWorkList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

        ResultProjectLinq result = new ResultProjectLinq();

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
                    join i in dc.ProjectLinqs on u.LinkId equals i.ProjectId
                    where i.Inactive == 0 && u.LinkModule == "Project" && u.LinkType == "worklist" && u.UserId.ToLower() == login_user.ToLower()
                    orderby u.LinkId
                    select i;

        GridLinqBindingData<ProjectLinq> data = RadGrid.GetBindingData<ProjectLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = data.Data.ToList<ProjectLinq>();
        result.Count = data.Count;

        return result;
    }

    [OperationContract]
    [WebGet]
    public string SaveProject(string paramXML)
    {
        string returnStr = "OK";
        bool success = false;
        string userid = "";
        string mode = "";
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
        }
        userid = userid.ToUpper();

        if (context != null)
        {
            if (context.Request.Headers["mode"] != null)
                mode = context.Request.Headers["mode"];
        }

        NameValueCollection nvc;
        nvc = new NameValueCollection();

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(paramXML);

        XmlNode root = doc.DocumentElement.FirstChild;

        XmlNodeList list = root.ChildNodes;

        for (int i = 0; i < list.Count; i++)
        {
            string name = list[i].Name;
            string val = list[i].InnerText;
            nvc.Add(name, val);
        }


        if (nvc["undefined"] != null)
        {
            nvc.Remove("undefined");
        }
        Project objtc = new Project(context.Session["Login"].ToString(), "Projects", "ProjectId");
        switch (mode)
        {
            case "new":
                objtc = new Project(context.Session["Login"].ToString(), "Projects", "ProjectId");
                success = objtc.Create(nvc);
                UserWorkList worklist = new UserWorkList();
                worklist.AddToRecentList(context.Session["Login"].ToString(), "Projects", objtc.ModuleData["projectid"]);
                break;
            case "edit":
                objtc = new Project(context.Session["Login"].ToString(), "Projects", "ProjectId", nvc["projectid"]);
                success = objtc.Update(nvc);
                break;
        }



        if (!success)
        {
            returnStr = objtc.ErrorMessage;
        }

        return returnStr;
    }

    [OperationContract]
    [WebGet]
    public string DelProject(string projectid)
    {
        string retstr = "Success";

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


        try
        {
            UserWorkList uwl = new UserWorkList();
            Project objproj = new Project(login_user, "Projects", "ProjectId", projectid);   

            if (objproj.Delete())
            {
                uwl.RemoveDeletedRecords("Project", projectid);
       
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
    public string CreatePhase(string paramXML)
    {
        string retstr = "OK";

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

        bool success = false;
        NameValueCollection nvc;
        nvc = new NameValueCollection();

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(paramXML);

        XmlNode root = doc.DocumentElement.FirstChild;

        XmlNodeList fieldlist = root.ChildNodes;

        for (int i = 0; i < fieldlist.Count; i++)
        {
            string name = fieldlist[i].Name;
            string val = fieldlist[i].InnerText;
            nvc.Add(name, val);
        }

        if (nvc["Counter"] != null)
        {
            nvc.Remove("Counter");
        }
        ModuleoObject objmeter = new ModuleoObject(login_user, "Phase", "Counter");
        //nvc.Remove("Counter");
        success = objmeter.Create(nvc);
        if (success)
        {
            retstr = "OK";
        }
        else
        {
            string err = "Create failed:" + objmeter.ErrorMessage.Replace("\r\n", "");
            retstr = err;
        }
        return retstr;
    }

    [OperationContract]
    [WebGet]
    public string UpdatePhase(string paramXML)
    {
        string retstr = "OK";

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

        bool success = false;
        NameValueCollection nvc;
        nvc = new NameValueCollection();

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(paramXML);

        XmlNode root = doc.DocumentElement.FirstChild;

        XmlNodeList fieldlist = root.ChildNodes;

        for (int i = 0; i < fieldlist.Count; i++)
        {
            string name = fieldlist[i].Name;
            string val = fieldlist[i].InnerText;
            nvc.Add(name, val);
        }

        if (nvc["Counter"] == null)
        {
            return "Invalid Data. Missing key fields.";
        }
        ModuleoObject objmeter = new ModuleoObject(login_user, "Phase", "Counter", nvc["Counter"].ToString());
        //nvc.Remove("Counter");
        success = objmeter.Update(nvc);
        if (success)
        {
            retstr = "OK";
        }
        else
        {
            string err = "Update failed:" + objmeter.ErrorMessage.Replace("\r\n", "");
            retstr = err;
        }
        return retstr;
    }

    [OperationContract]
    [WebGet]
    public string AddNewWOtoPhase(string paramXML, string projectid, string phase)
    {
        string retstr = "OK";

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

        try
        {
            var result = from wl in dc.WOLinqs
                         where idresult.ToList().Contains(wl.WoNum)
                         select wl;
            if (result.Count() > 0)
            {
                foreach (WOLinq wo in result)
                {
                    wo.Phase = phase;
                    wo.ProjectId = projectid;
                }
            }
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
    public string DeletePhase(string counter)
    {
        string retstr = "OK";

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
        string projectid = "";
        string phase = "";
        PhaseLinq phaseobj = dc.PhaseLinqs.Where(x => x.Counter.ToString() == counter).FirstOrDefault();
        if (phaseobj != null)
        {
            projectid = phaseobj.ProjectId;
            phase = phaseobj.Phase;

            int count = dc.WOLinqs.Where(x => x.ProjectId == projectid && x.Phase == phase).Count();
            if (count == 0)
            {
                try
                {
                    dc.PhaseLinqs.DeleteOnSubmit(phaseobj);
                    dc.SubmitChanges();
                }
                catch (Exception e)
                {
                    retstr = e.Message;
                }
            }
            else
            {
                return "This phase is not empty.";
            }
        }
        else
        {
            return "Invalid key fields.";
        }

        return retstr;
    }

    [OperationContract]
    [WebGet]
    public string DeleteWO(string wonum)
    {
        string retstr = "OK";

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

        var query = dc.WOLinqs.Where(x => x.WoNum.ToLower() == wonum.ToLower());
        if (query.Count() >0)
        {
            foreach (WOLinq wo in query)
            {
                wo.Phase = null;
                wo.ProjectId = null;
            }
        }
        else
        {
            return "Invalid key fields.";
        }

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
}

public class ResultProjectLinq
{
    public Int32 Count { get; set; }
    public List<ProjectLinq> Data { get; set; }
}

public class ResultProjectRecentList
{
    public Int32 Count { get; set; }
    public List<v_ProjectRecentList> Data{get; set;}
}

public class ResultProjHistoryLinq
{
    public Int32 Count {get;set;}
    public List<ProjHistoryLinq> Data { get; set; }
}
