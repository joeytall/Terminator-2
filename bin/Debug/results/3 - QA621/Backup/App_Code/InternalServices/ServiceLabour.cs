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
public class ServiceLabour
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

        try
        {
            var result = from userwl in dc.UserWorkListLinqs
                         where idresult.ToList().Contains(userwl.LinkId) && userwl.LinkModule == "Labour" &&
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
                               where c.LinkModule == "Labour" && c.LinkType == "worklist" && c.UserId.ToLower() == login_user.ToLower()
                               select c.LinkId).Contains(id.Value)
                       select id.Value;

        var ins = from id in dc.EmployeeLinqs
                  where idresult.ToList().Contains(id.Empid)
                  select new
                  {
                      LinkId = id.Empid,
                      UserId = login_user,
                      LinkModule = "Labour",
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
    public ResultEmployeeRecentlist RecentListDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultEmployeeRecentlist result = new ResultEmployeeRecentlist();

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

        var query = from v in dc.v_EmployeeRecentlists where v.UserId.ToLower() == login_user.ToLower() orderby v.AccessDate descending select v;

        GridLinqBindingData<v_EmployeeRecentlist> resultData = RadGrid.GetBindingData<v_EmployeeRecentlist>(query,startRowIndex,maximumRows,sortExpression,filterExpression);

        result.Data = resultData.Data.ToList<v_EmployeeRecentlist>();
        result.Count = resultData.Count;

        return result;
    }

    [OperationContract]
    public ResultTimeCardsRecentlist TimeCardsRecentListDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultTimeCardsRecentlist result = new ResultTimeCardsRecentlist();
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

        var query = from wol in dc.WOLabourLinqs.Where(x=>x.Estimate == 0)
                    join uwl in dc.UserWorkListLinqs.Where(x => x.LinkType == "recent" && x.LinkModule == "TimeCards") on wol.Counter.ToString() equals uwl.LinkId
                   // orderby uwl.TransDate descending
                    where uwl.UserId.ToLower() == login_user.ToLower()
                    select new v_TimeCardsRecentlist
                    {
                        Counter =wol.Counter,
                        Empid = wol.Empid,
                        TransDate = wol.TransDate,
                        WoNum = wol.WoNum,
                        Craft = wol.Craft,
                        CRAccount= wol.CRAccount,
                        DrAccount = wol.DrAccount,
                        Description = wol.Description,
                        TaskNum  = wol.TaskNum,
                        LaborType = wol.LaborType,
                        Scale = wol.Scale,
                        Hours = wol.Hours,
                        Rate = wol.Rate,
                        Tax1 = wol.Tax1,
                        Tax2 = wol.Tax2,
                        AddCost  = wol.AddCost,
                        TotalCost = wol.TotalCost,
                        ChargeBack = wol.ChargeBack,
                        MarkupAmount = wol.MarkupAmount,
                        CBTax1 = wol.CBTax1,
                        CBTax2 = wol.CBTax2,
                        ChargeBackAmount = wol.ChargeBackAmount,
                        Estimate = wol.Estimate,
                        Inactive = wol.Inactive,
                        Actual = wol.Actual,
                        RefNum = wol.RefNum,
                        Remark = wol.Remark,
                        PostDate = wol.PostDate,
                        PrintDate =wol.PrintDate,
                        PrintPerson  = wol.PrintPerson,
                        ModifyBy = wol.ModifyBy,
                        ModifyDate = wol.ModifyDate,
                        DirtyLog = wol.DirtyLog,
                        CreatedBy  = wol.CreatedBy,
                        CreationDate = wol.CreationDate,
                        ordertype = wol.ordertype,
                        ChangeRemark = wol.ChangeRemark,
                        UserId  = uwl.UserId,
                        AccessDate = uwl.TransDate
                    };

        GridLinqBindingData<v_TimeCardsRecentlist> resultData = RadGrid.GetBindingData<v_TimeCardsRecentlist>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        result.Data = resultData.Data.ToList<v_TimeCardsRecentlist>();
        result.Count = resultData.Count;

        return result;
    }


    [OperationContract]
    public ResultEmployeeLinq WorkListDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultEmployeeLinq result = new ResultEmployeeLinq();

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
                    join e in dc.EmployeeLinqs on u.LinkId equals e.Empid
                    where e.Inactive == 0 && u.LinkModule.ToLower() == "labour" && u.LinkType.ToLower() == "worklist" && u.UserId.ToLower()==login_user.ToLower()
                    orderby e.Empid
                    select e;

        GridLinqBindingData<EmployeeLinq> data = RadGrid.GetBindingData<EmployeeLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = data.Data.ToList<EmployeeLinq>();
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
    public ResultEmployeeLinq SearchDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultEmployeeLinq result = new ResultEmployeeLinq();
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
                result.Count  = 0;
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

        var query = from e in dc.EmployeeLinqs select e;
        GridLinqBindingData<EmployeeLinq> data = RadGrid.GetBindingData<EmployeeLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        //GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "EmployeeLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = data.Data.OfType<EmployeeLinq>().ToList();
        result.Count = data.Count;

        return result;
    }

    [OperationContract]
    [WebGet]
    public ResultEmployeeLinq QueryResultLabour(int startRowIndex, int maximumRows, string sortExpression, string filterExpression, string jsonData)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultEmployeeLinq result = new ResultEmployeeLinq();
        string searchcriteria;
        List<string> eqptlist = new List<string>();

        searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";

        NameValueCollection nvc = AzzierData.DeserializeJSON(jsonData);
        object empid = HttpContext.Current.Session["LogIn"];
        string userid = (empid != null) ? empid.ToString() : "";
        Labour objLabour = new Labour(userid, "Employee", "Empid");
        string wherestring = "", linqwherestring = "";
        if (jsonData.Contains("queryID"))
          nvc = UserQuery.CreateNameValueCollection(nvc["queryID"]);
        objLabour.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
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

        var query = from e in dc.EmployeeLinqs select e;
        GridLinqBindingData<EmployeeLinq> data = RadGrid.GetBindingData<EmployeeLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        //GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "EmployeeLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = data.Data.OfType<EmployeeLinq>().ToList();
        result.Count = data.Count;

        return result;
    }

    [OperationContract]
    [WebGet]
    public ResultEmployeeLinq GetEmployeeList(string paramXML, int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        ResultEmployeeLinq result = new ResultEmployeeLinq();

        HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

        List<string> strlist = new List<string>();

        if (HttpContext.Current.Session["Login"] == null)
        {
            result.Data = null;
            result.Count = 0;
            return result;
        }

        XDocument doc = XDocument.Parse(paramXML);

        string wonum = doc.Root.Element("Workorder").Value;

        var idresult = from id in doc.Descendants("counter") select id.Value;

        var query = from tc in dc.EmployeeLinqs
                    where idresult.Contains(tc.Craft)
                    select tc;
        GridLinqBindingData<EmployeeLinq> resultData = RadGrid.GetBindingData<EmployeeLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        result.Data = resultData.Data.OfType<EmployeeLinq>().ToList();
        result.Count = resultData.Count;

        return result;
    }

    [OperationContract]
    [WebGet]
    public ResultLabourLinq GetLabourList(string paramXML, int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        ResultLabourLinq result = new ResultLabourLinq();

        HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

        List<string> strlist = new List<string>();

        if (HttpContext.Current.Session["Login"] == null)
        {
            result.Data = null;
            result.Count = 0;
            return result;
        }

        XDocument doc = XDocument.Parse(paramXML);

        string wonum = doc.Root.Element("Workorder").Value;

        var idresult = from id in doc.Descendants("counter") select id.Value;

        var query = from tc in dc.WOLabourLinqs
                    where idresult.Contains(tc.WoNum) && tc.Estimate == 1 && tc.ordertype == "workorder"
                    select tc;
        GridLinqBindingData<WOLabourLinq> resultData = RadGrid.GetBindingData<WOLabourLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        result.Data = resultData.Data.OfType<WOLabourLinq>().ToList();
        result.Count = resultData.Count;

        return result;
    }

    [OperationContract]
    [WebGet]
    public ResultTimeCardLinq2 ClientLookupTimeCardDataAndCount(string empid, string inactive, int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultTimeCardLinq2 result = new ResultTimeCardLinq2();
        string lookupWhereStr = "";
        string login_user = "";
        lookupWhereStr = HttpContext.Current.Request.QueryString.Get("where") ?? "";

        if (!string.IsNullOrEmpty(lookupWhereStr))
        {
            lookupWhereStr = lookupWhereStr.Replace("'", "\"");
            if (!string.IsNullOrEmpty(filterExpression))
            {
                filterExpression = filterExpression + " and " + lookupWhereStr;
            }
            else
                filterExpression = lookupWhereStr;
        }

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

        var query = from tc in dc.WOLabourLinqs
                    where tc.Empid.ToLower() == empid.ToLower() && tc.Inactive.ToString() == inactive && tc.ordertype.ToLower() == "workorder" && tc.Estimate == 0
                    select tc
                    ;
        GridLinqBindingData<WOLabourLinq> resultData = RadGrid.GetBindingData<WOLabourLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = resultData.Data.ToList<WOLabourLinq>();
        result.Count = resultData.Count;

        return result;
    }



    [OperationContract]
    [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
    public ResultTimeCardLinq LookupTimeCardDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultTimeCardLinq result = new ResultTimeCardLinq();
        string lookupWhereStr = "";
        string login_user = "";
        lookupWhereStr = HttpContext.Current.Request.QueryString.Get("where") ?? "";

        if (!string.IsNullOrEmpty(lookupWhereStr))
        {
            lookupWhereStr = lookupWhereStr.Replace("'", "\"");
            if (!string.IsNullOrEmpty(filterExpression))
            {
                filterExpression = filterExpression + " and " + lookupWhereStr;
            }
            else
                filterExpression = lookupWhereStr;
        }

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

        var query = from wl in dc.WOLabourLinqs
                    join wo in dc.WOLinqs on wl.WoNum equals wo.WoNum into lj
                    from woe in lj.DefaultIfEmpty()
                    where (wl.ordertype.ToLower() == "workorder") && (wl.Estimate == 0)
                    select new v_WOLabourDetail
                    {
                        Counter = wl.Counter,
                        Empid = wl.Empid,
                        TransDate = wl.TransDate,
                        WoNum = wl.WoNum,
                        Craft = wl.Craft,
                        CRAccount = wl.CRAccount,
                        DrAccount = wl.DrAccount,
                        Description = wl.Description,
                        TaskNum = wl.TaskNum,
                        LaborType = wl.LaborType,
                        Scale = wl.Scale,
                        Hours = wl.Hours,
                        Rate = wl.Rate,
                        Tax1 = wl.Tax1,
                        Tax2 = wl.Tax2,
                        AddCost = wl.AddCost,
                        TotalCost = wl.TotalCost,
                        ChargeBack = wl.ChargeBack,
                        MarkupAmount = wl.MarkupAmount,
                        CBTax1 = wl.CBTax1,
                        CBTax2 = wl.CBTax2,
                        ChargeBackAmount = wl.ChargeBackAmount,
                        Estimate = wl.Estimate,
                        Inactive = wl.Inactive,
                        Actual = wl.Actual,
                        RefNum = wl.RefNum,
                        Remark = wl.Remark,
                        PostDate = wl.PostDate,
                        PrintDate = wl.PrintDate,
                        PrintPerson = wl.PrintPerson,
                        ModifyBy = wl.ModifyBy,
                        ModifyDate = wl.ModifyDate,
                        DirtyLog = wl.DirtyLog,
                        CreatedBy = wl.CreatedBy,
                        CreationDate = wl.CreationDate,
                        ordertype = wl.ordertype,
                        ChangeRemark = wl.ChangeRemark,
                        PublicNote = wl.PublicNote,
                        ReadyToPost = wl.ReadyToPost,
                        WORate = wl.WORate,
                        WoType = woe.WoType,
                        WOStatus = woe.Status,
                        Request = woe.Request,
                        PostBy = wl.PostBy,
                        StatusCode = woe.StatusCode,
                        WOSubType = woe.WOSubType,
                        Division = woe.Division
                    };

        GridLinqBindingData<v_WOLabourDetail> resultData = RadGrid.GetBindingData<v_WOLabourDetail>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        result.Data = resultData.Data.ToList<v_WOLabourDetail>();
        result.Count = resultData.Count;

        return result;
    }

    [OperationContract]
    [WebGet]
    public ResultTimeCardLinq QueryResultTimeCard(int startRowIndex, int maximumRows, string sortExpression, string filterExpression, string jsonData)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultTimeCardLinq result = new ResultTimeCardLinq();
        string lookupWhereStr = "";
        string login_user = "";
        lookupWhereStr = HttpContext.Current.Request.QueryString.Get("where") ?? "";

        jsonData = jsonData == null ? "{}" : jsonData;
        if (jsonData != "{}")
        {
            NameValueCollection nvc = AzzierData.DeserializeJSON(jsonData);
            object empid = HttpContext.Current.Session["LogIn"];
            string userid = (empid != null) ? empid.ToString() : "";
            ModuleoObject objview = new ModuleoObject(userid, "v_WOLabourDetail", "Counter");
            string wherestring = "", linqwherestring = "";
            if (jsonData.Contains("queryID"))
                nvc = UserQuery.CreateNameValueCollection(nvc["queryID"]);
            objview.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
            lookupWhereStr = linqwherestring;
        }

        if (!string.IsNullOrEmpty(lookupWhereStr))
        {
            lookupWhereStr = lookupWhereStr.Replace("'", "\"");
            if (!string.IsNullOrEmpty(filterExpression))
            {
                filterExpression = filterExpression + " and " + lookupWhereStr;
            }
            else
                filterExpression = lookupWhereStr;
        }

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

        var query = from wl in dc.WOLabourLinqs
                    join wo in dc.WOLinqs on wl.WoNum equals wo.WoNum into lj
                    from woe in lj.DefaultIfEmpty()
                    where (wl.ordertype.ToLower() == "workorder") && (wl.Estimate == 0)
                    select new v_WOLabourDetail
                    {
                        Counter = wl.Counter,
                        Empid = wl.Empid,
                        TransDate = wl.TransDate,
                        WoNum = wl.WoNum,
                        Craft = wl.Craft,
                        CRAccount = wl.CRAccount,
                        DrAccount = wl.DrAccount,
                        Description = wl.Description,
                        TaskNum = wl.TaskNum,
                        LaborType = wl.LaborType,
                        Scale = wl.Scale,
                        Hours = wl.Hours,
                        Rate = wl.Rate,
                        Tax1 = wl.Tax1,
                        Tax2 = wl.Tax2,
                        AddCost = wl.AddCost,
                        TotalCost = wl.TotalCost,
                        ChargeBack = wl.ChargeBack,
                        MarkupAmount = wl.MarkupAmount,
                        CBTax1 = wl.CBTax1,
                        CBTax2 = wl.CBTax2,
                        ChargeBackAmount = wl.ChargeBackAmount,
                        Estimate = wl.Estimate,
                        Inactive = wl.Inactive,
                        Actual = wl.Actual,
                        RefNum = wl.RefNum,
                        Remark = wl.Remark,
                        PostDate = wl.PostDate,
                        PrintDate = wl.PrintDate,
                        PrintPerson = wl.PrintPerson,
                        ModifyBy = wl.ModifyBy,
                        ModifyDate = wl.ModifyDate,
                        DirtyLog = wl.DirtyLog,
                        CreatedBy = wl.CreatedBy,
                        CreationDate = wl.CreationDate,
                        ordertype = wl.ordertype,
                        ChangeRemark = wl.ChangeRemark,
                        PublicNote = wl.PublicNote,
                        ReadyToPost = wl.ReadyToPost,
                        WORate = wl.WORate,
                        WoType = woe.WoType,
                        WOStatus = woe.Status,
                        Request = woe.Request,
                        PostBy = wl.PostBy,
                        StatusCode = woe.StatusCode,
                        WOSubType = woe.WOSubType,
                        Division = woe.Division
                    };

        GridLinqBindingData<v_WOLabourDetail> resultData = RadGrid.GetBindingData<v_WOLabourDetail>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        result.Data = resultData.Data.ToList<v_WOLabourDetail>();
        result.Count = resultData.Count;

        return result;
    }
    [OperationContract]
    public ResultEmployeeLinq SpecSearchDatandCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        string searchcriteria = "";
        List<string> laborlist = new List<string>();

        ResultEmployeeLinq result = new ResultEmployeeLinq();

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
                var query = from ca in dc.EmployeeLinqs
                            where ca.Inactive == 0 && laborlist.Contains(ca.Empid) && (ca.Division == null ||
                            (from o in dc.UserDivisionLinqs where o.Empid == login_user select o.Division).Contains(ca.Division))
                            select ca;

                GridLinqBindingData<EmployeeLinq> resultData = RadGrid.GetBindingData<EmployeeLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
                result.Data = resultData.Data.ToList<EmployeeLinq>();
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

    public ResultEmployeeLinq LookupDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultEmployeeLinq result = new ResultEmployeeLinq();

        string lookupWhereStr = HttpContext.Current.Request.QueryString.Get("where");
        string employee = HttpContext.Current.Request.QueryString.Get("employee");
        if (!string.IsNullOrEmpty(lookupWhereStr))
        {
            lookupWhereStr = lookupWhereStr.Replace("'","\"");
            if (!string.IsNullOrEmpty(filterExpression))
            {
                filterExpression = filterExpression + " and " + lookupWhereStr;
            }
            else
                filterExpression = lookupWhereStr;
        }

        var querya = from emp in dc.EmployeeLinqs  select emp;
        if (!string.IsNullOrEmpty(employee) && employee != "-1")
        { querya = querya.Where(a => a.Employee.ToString() == employee).OrderBy(a => a.Empid); }
        GridLinqBindingData<EmployeeLinq> resultData = RadGrid.GetBindingData<EmployeeLinq>(querya,startRowIndex,maximumRows,sortExpression,filterExpression);
        result.Data = resultData.Data.ToList<EmployeeLinq>();
        result.Count = resultData.Count;

        return result;
    }

    [OperationContract]
    [WebGet]
    public string DelWOLabor(string paramXML)
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

        XDocument doc = XDocument.Parse(paramXML);

        var idresult = from id in doc.Descendants("IDS") select id.Value;

        try
        {
            UserWorkList uwl = new UserWorkList();
            var result = from wl in dc.WOLabourLinqs
                         where idresult.ToList().Contains(wl.Counter.ToString())
                         select wl;

            if (result.Count() > 0)
            {
                foreach (WOLabourLinq wl in result)
                {
                    uwl.RemoveDeletedRecords("timecards", wl.Counter.ToString());
                }
            }
            dc.WOLabourLinqs.DeleteAllOnSubmit(result);
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
    public string SetReadytoPostTimeCards(string paramXML)
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

        XDocument doc = XDocument.Parse(paramXML);

        var idresult = from id in doc.Descendants("IDS") select id.Value;

        try
        {
            UserWorkList uwl = new UserWorkList();
            var result = from wl in dc.WOLabourLinqs
                         where idresult.ToList().Contains(wl.Counter.ToString())
                         select wl;
            if (result.Count() > 0)
            {
                foreach (WOLabourLinq r in result)
                {
                    r.ReadyToPost = 1;
                 //   uwl.AddToRecentList(login_user, "timecards", r.Counter.ToString());
                }
            }
            dc.SubmitChanges();
            SendEmailClass.SendEmailByDataChange();
        }
        catch
        {
            retstr = "ERROR.";
        }

        return retstr;
    }

    [OperationContract]
    [WebGet]
    public string PostTimeCards(string paramXML)
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

        XDocument doc = XDocument.Parse(paramXML);

        var idresult = from id in doc.Descendants("IDS") select id.Value;

        try
        {
            UserWorkList uwl = new UserWorkList();
            var result = from wl in dc.WOLabourLinqs
                                        where idresult.ToList().Contains(wl.Counter.ToString())
                                        select wl;
            if (result.Count()>0)
            {
                foreach (WOLabourLinq r in result)
                {
                    r.Inactive = 0;
                    r.ReadyToPost = 0;
                    r.PostBy = login_user;
                    uwl.AddToRecentList(login_user,"timecards",r.Counter.ToString());
                }
            }
            dc.SubmitChanges();
        }
        catch
        {
            retstr = "ERROR.";
        }

        try
        {
            List<String> wonumlist = (from wl in dc.WOLabourLinqs
                                      where idresult.ToList().Contains(wl.Counter.ToString())
                                      select wl.WoNum).ToList();
            var wolist = from wo in dc.WOLinqs
                         where wonumlist.Contains(wo.WoNum.ToString())
                         select wo;
            if (wolist.Count() >= 0)
            {
                foreach (var w in wolist)
                {
                    w.ActHours = dc.WOLabourLinqs.Where(x => x.WoNum == w.WoNum && x.Estimate == 0 && x.Inactive == 0).Sum(x => x.Hours);
                    w.ActLabor = dc.WOLabourLinqs.Where(x => x.WoNum == w.WoNum && x.Estimate == 0 && x.Inactive == 0).Sum(x => x.TotalCost);
                }
            }
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
    public string SaveTimeCard(string paramXML)
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
        Labour objtc = new Labour(context.Session["Login"].ToString(), "WOLabour", "Counter");
        switch (mode)
        {
            case "new":
                if (nvc["Counter"] != null)
                {
                    nvc.Remove("Counter");
                }
                objtc = new Labour(context.Session["Login"].ToString(), "WOLabour", "Counter");
                success = objtc.Create(nvc);
                UserWorkList worklist = new UserWorkList();
                worklist.AddToRecentList(context.Session["Login"].ToString(), "TimeCards", objtc.ModuleData["counter"]);
                break;
            case "edit":
                objtc = new Labour(context.Session["Login"].ToString(), "WOLabour", "Counter", nvc["Counter"]);
                success = objtc.Update(nvc);
                break;
        }



        if (!success)
        {
            returnStr = objtc.ErrorMessage;
        }

        return returnStr;
    }


    //[OperationContract]

    //public ResultTestItem GetTestItem()
    //{
    //    ResultTestItem result = new ResultTestItem();

    //    List<TestItem> datelist = new List<TestItem>();
    //    for (int i = 0; i < 10; i++)
    //    {
    //        TestItem a = new TestItem();
    //        a.TestName = "test" + i.ToString();
    //        a.TestDate = DateTime.Today.AddDays(i);
    //        datelist.Add(a);

    //    }
    //    result.Data = datelist;
    //    result.Count = datelist.Count;

    //    return result;
    //}

    [OperationContract]
    public ResultLaborTypeLinq GetLaborTypeList (int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultLaborTypeLinq result = new ResultLaborTypeLinq();
        string lookupfilter = HttpContext.Current.Request.QueryString.Get("where");

        if (!string.IsNullOrEmpty(lookupfilter))
        {
            lookupfilter = lookupfilter.Replace("'", "\"");
            if (!string.IsNullOrEmpty(filterExpression))
                filterExpression = filterExpression + " and " + lookupfilter;
            else
                filterExpression = lookupfilter;
        }

        var query = from lab in dc.LaborTypeLinqs select lab;
        query = query.OrderBy(x => x.LabType);
        GridLinqBindingData<LaborTypeLinq> resultData = RadGrid.GetBindingData<LaborTypeLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        result.Data = resultData.Data.ToList<LaborTypeLinq>();
        result.Count = resultData.Count;

        return result;
    }

    [OperationContract]
    public ResultScheduleLinq GetScheduleList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultScheduleLinq result = new ResultScheduleLinq();
        string wonum = HttpContext.Current.Request.QueryString.Get("wonum");

        var labourQuery = from l in dc.WOLabourLinqs
                              where l.WoNum == wonum
                          select l.Counter.ToString();

        var query = from s in dc.ScheduleLinqs
                        where s.ActivityType == "wolabour" && labourQuery.ToList().Contains(s.ActivityID)
                        select s;

        query = query.OrderBy(x => x.Counter);
        GridLinqBindingData<ScheduleLinq> resultData = RadGrid.GetBindingData<ScheduleLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        result.Data = resultData.Data.ToList<ScheduleLinq>();
        result.Count = resultData.Count;

        return result;
    }

    [OperationContract]
      [WebGet]
    public void ApplyEmployeeTemplate(string toApply, string employees, int option)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        Scheduler.ApplyTemplate(toApply, employees, option);
        return ;
    }

    [OperationContract]
    [WebGet]
    public string DeleteTemplate(string template, string type)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        Scheduler.DeleteTemplate(template, type);
        return template;
    }

    [OperationContract]
    [WebGet]
    public void ApplySchedulerSetting(string toApply, string employees, int option)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        Scheduler.ApplySetting(toApply, employees);
        return;
    }

    [OperationContract]
    [WebGet]
    public void GetUserSetting(string empid)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        return;
    }
}

public class ResultScheduleLinq
{
    public Int32 Count { get; set; }
    public List<ScheduleLinq> Data { get; set; }
}

public class ResultEmployeeRecentlist
{
    public int Count { get; set; }
    public List<v_EmployeeRecentlist> Data { get; set; }
}

public class ResultEmployeeLinq
{
    public Int32 Count { get; set; }
    public List<EmployeeLinq> Data { get; set; }
}

public class ResultTimeCardLinq
{
    public Int32 Count { get; set; }
    public List<v_WOLabourDetail> Data { get; set; }
}

public class ResultTimeCardLinq2
{
    public Int32 Count { get; set; }
    public List<WOLabourLinq> Data { get; set; }
}

public class ResultLabourLinq
{
    public Int32 Count { get; set; }
    public List<WOLabourLinq> Data { get; set; }
}

public class ResultTimeCardsRecentlist
{
    public Int32 Count { get; set; }
    public List<v_TimeCardsRecentlist> Data { get; set; }
}

public class ResultTestItem
{
    public Int32 Count { get; set; }
    public List<TestItem> Data { get; set; }
}

public class ResultLaborTypeLinq
{
    public Int32 Count { get; set; }
    public List<LaborTypeLinq> Data { get; set; }
}

public class TestItem
{
    public string TestName { set; get; }
    public DateTime TestDate { set; get; }
}
