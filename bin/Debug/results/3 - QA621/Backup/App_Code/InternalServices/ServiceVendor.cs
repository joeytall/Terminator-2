using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using Telerik.Web.UI;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Specialized;

[ServiceContract(Namespace = "")]
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class ServiceVendor
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
    public ResultVendorLinq LookupDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultVendorLinq result = new ResultVendorLinq();
        string lookupfilter = HttpContext.Current.Request.QueryString.Get("where");
        string itemnum = HttpContext.Current.Request.QueryString.Get("itemnum") + "";

        if (!string.IsNullOrEmpty(lookupfilter))
        {
          lookupfilter = lookupfilter.Replace("'", "\"");
          if (!string.IsNullOrEmpty(filterExpression))
            filterExpression = filterExpression + " and " + lookupfilter;
          else
            filterExpression = lookupfilter;
        }
        //var query = from ven in dc.VendorLinqs orderby ven.CompanyCode select ven;
        var query = from ven in dc.VendorLinqs select ven;

        if (itemnum != "")
        {
          /*
          var vendorlist = from iv in dc.ItemVendorLinqs
                           where iv.ItemNum.ToLower() == itemnum.ToLower()
                           select iv.Vendor;
          query = from ven in dc.VendorLinqs
                  where vendorlist (ven.CompanyCode)
                  select ven;
           * */
            query = from ven in dc.VendorLinqs
                  join iv in dc.ItemVendorLinqs on ven.CompanyCode equals iv.Vendor
                  where iv.ItemNum.ToLower() == itemnum.ToLower()
                  select ven;

                  
        }
        query = query.OrderBy(x => x.CompanyCode);
        GridLinqBindingData<VendorLinq> resultData = RadGrid.GetBindingData<VendorLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        result.Data = resultData.Data.ToList<VendorLinq>();
        result.Count = resultData.Count;

        return result;
    }

    [OperationContract]
    public ResultItemVendorLinq GetItemVendor(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
      ResultItemVendorLinq result = new ResultItemVendorLinq();
      string lookupfilter = HttpContext.Current.Request.QueryString.Get("where");

      if (!string.IsNullOrEmpty(lookupfilter))
      {
        lookupfilter = lookupfilter.Replace("'", "\"");
        if (!string.IsNullOrEmpty(filterExpression))
          filterExpression = filterExpression + " and " + lookupfilter;
        else
          filterExpression = lookupfilter;
      }

      var query = from ven in dc.ItemVendorLinqs select ven;
      query = query.OrderBy(x => x.Vendor);
      GridLinqBindingData<ItemVendorLinq> resultData = RadGrid.GetBindingData<ItemVendorLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

      result.Data = resultData.Data.ToList<ItemVendorLinq>();
      result.Count = resultData.Count;

      return result;
    }

    [OperationContract]
    public ResultVendorLinq WorklistDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultVendorLinq result = new ResultVendorLinq();

        string loginUser = "";
        if (System.Web.HttpContext.Current.Session["Login"] != null)
        {
            loginUser = System.Web.HttpContext.Current.Session["Login"].ToString();
        }
        else
        {
            result.Data = null;
            result.Count = 0;
            return result;
        }

        var query = from u in dc.UserWorkListLinqs
                    join v in dc.VendorLinqs on u.LinkId equals v.CompanyCode
                    where u.LinkModule.ToLower() == "vendor" && u.UserId.ToLower() == loginUser.ToLower() && u.LinkType.ToLower() == "worklist"
                    orderby v.CompanyCode
                    select v;

        GridLinqBindingData<VendorLinq> resultdata = RadGrid.GetBindingData<VendorLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = resultdata.Data.ToList<VendorLinq>();
        result.Count = resultdata.Count;

        return result;
    }

    [OperationContract]
    public ResultVendorRecentlist RecentlistDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
        ResultVendorRecentlist result = new ResultVendorRecentlist();

        string loginUser = "";
        if (System.Web.HttpContext.Current.Session["Login"] != null)
        {
            loginUser = System.Web.HttpContext.Current.Session["Login"].ToString();
        }
        else
        {
            result.Data = null;
            result.Count = 0;
            return result;
        }

        var query = from v in dc.v_VendorRecentlists where v.UserId.ToLower() == loginUser.ToLower() orderby v.AccessDate descending select v;
        GridLinqBindingData<v_VendorRecentlist> resultData = RadGrid.GetBindingData<v_VendorRecentlist>(query,startRowIndex,maximumRows,sortExpression,filterExpression);
        result.Data = resultData.Data.ToList<v_VendorRecentlist>();
        result.Count = resultData.Count;

        return result;
    }

    
    [OperationContract]
    public ResultVendorLinq SearchlistDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultVendorLinq result = new ResultVendorLinq();

        string searhcriteria = "", loginUser = "";
        if (System.Web.HttpContext.Current.Session["Login"] != null)
        {
            loginUser = System.Web.HttpContext.Current.Session["Login"].ToString();
        }
        else
        {
            result.Data = null;
            result.Count = 0;
            return result;
        }

        if (System.Web.HttpContext.Current.Request.QueryString.Get("wherestring") != null)
        {
            searhcriteria = System.Web.HttpContext.Current.Request.QueryString.Get("wherestring");
            if (!string.IsNullOrEmpty(filterExpression))
            {
                filterExpression = filterExpression + " and " + searhcriteria;
            }
            else
            {
                filterExpression = searhcriteria;
            }
        }

        var query = from v in dc.VendorLinqs select v;

        GridLinqBindingData<VendorLinq> resultdata = RadGrid.GetBindingData<VendorLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = resultdata.Data.ToList<VendorLinq>();
        result.Count = resultdata.Count;

        return result;
    }

    [OperationContract]
    [WebGet]
    public ResultVendorLinq QueryResultVendor(int startRowIndex, int maximumRows, string sortExpression, string filterExpression, string jsonData)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultVendorLinq result = new ResultVendorLinq();

        string searchcriteria;
        List<string> strlist = new List<string>();

        searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";

        NameValueCollection nvc = AzzierData.DeserializeJSON(jsonData);
        object empid = HttpContext.Current.Session["LogIn"];
        string userid = (empid != null) ? empid.ToString() : "";
        ModuleoObject objVendor = new ModuleoObject(userid, "Vendor", "CompanyCode");
        string wherestring = "", linqwherestring = "";
        if (jsonData.Contains("queryID"))
          nvc = UserQuery.CreateNameValueCollection(nvc["queryID"]);
        objVendor.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
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

        var query = from v in dc.VendorLinqs select v;

        GridLinqBindingData<VendorLinq> resultdata = RadGrid.GetBindingData<VendorLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = resultdata.Data.ToList<VendorLinq>();
        result.Count = resultdata.Count;

        return result;
    }

    [OperationContract]
    [WebGet]
    public string AddtoWorklist(string paramXML)
    {
        string retStr = "Success", LoginUser = "";

        if (System.Web.HttpContext.Current.Session["Login"] != null)
        {
            LoginUser = System.Web.HttpContext.Current.Session["Login"].ToString();
        }
        else
            return "Please login again.";

        XDocument doc = XDocument.Parse(paramXML);

        var idlist = from id in doc.Descendants("vendor")
                     join v in dc.VendorLinqs on id.Value equals v.CompanyCode
                     where !(from u in dc.UserWorkListLinqs
                             where u.LinkModule.ToLower() == "vendor"
                                 && u.LinkType.ToLower() == "worklist"
                                 && u.UserId.ToLower()==LoginUser.ToLower()
                             select u.LinkId).Contains(id.Value) 
                     select id.Value;

        List<UserWorkListLinq> worklist = new List<UserWorkListLinq>();

        foreach (var id in idlist)
        {
            worklist.Add(new UserWorkListLinq() { LinkId = id, UserId = LoginUser, LinkModule = "Vendor", LinkType = "worklist", TransDate = DateTime.Now });
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

    [OperationContract]
    [WebGet]
    public string DelfromWorklist(string paramXML)
    {
        string retStr = "Success.", LoginUser = "";

        if (System.Web.HttpContext.Current.Session["Login"] != null)
        {
            LoginUser = System.Web.HttpContext.Current.Session["Login"].ToString();
        }
        else
            return "Please login again.";

        XDocument xdoc = XDocument.Parse(paramXML);

        try
        {
            var worlist = from w in dc.UserWorkListLinqs
                          where (from x in xdoc.Descendants("vendor") select x.Value).Contains(w.LinkId) && w.LinkModule.ToLower() == "vendor"
                          && w.LinkType.ToLower() == "worklist" && w.UserId.ToLower() == LoginUser.ToLower()
                          select w;
            dc.UserWorkListLinqs.DeleteAllOnSubmit(worlist);
            dc.SubmitChanges();
        }
        catch (Exception e)
        { retStr = e.Message; }

        return retStr;
    }

    public class ResultVendorRecentlist
    {
        public Int32 Count { get; set; }
        public List<v_VendorRecentlist> Data { get; set; }
    }

    [OperationContract]
    public ResultVendorServiceLinq GetVendorServiceLinq(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultVendorServiceLinq result = new ResultVendorServiceLinq();
        string lookupfilter = HttpContext.Current.Request.QueryString.Get("where");

        if (!string.IsNullOrEmpty(lookupfilter))
        {
            lookupfilter = lookupfilter.Replace("'", "\"");
            if (!string.IsNullOrEmpty(filterExpression))
                filterExpression = filterExpression + " and " + lookupfilter;
            else
                filterExpression = lookupfilter;
        }

        var query = from ven in dc.VendorServiceLinqs select ven;
        query = query.OrderBy(x => x.Vendor);
        GridLinqBindingData<VendorServiceLinq> resultData = RadGrid.GetBindingData<VendorServiceLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        result.Data = resultData.Data.ToList<VendorServiceLinq>();
        result.Count = resultData.Count;

        return result;
    }
}

public class ResultVendorLinq
{
    public Int32 Count { get; set; }
    public List<VendorLinq> Data { get; set; }
}

public class ResultItemVendorLinq
{
  public Int32 Count { get; set; }
  public List<ItemVendorLinq> Data { get; set; }
}

public class ResultVendorServiceLinq
{
    public Int32 Count { get; set; }
    public List<VendorServiceLinq> Data { get; set; }
}
