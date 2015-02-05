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
using System.Xml.Linq;
using System.Linq.Dynamic;
using System.Data;
using System.Data.OleDb;
using System.Collections.Specialized;

[ServiceContract(Namespace = "")]
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class ServicePO
{
  protected DataClassesDataContext dc = new DataClassesDataContext(AzzierData.LinqConnectstring);
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
  [OperationContract]
  public ResultsPOLinq WorklistData(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {
    System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
    ResultsPOLinq result = new ResultsPOLinq();

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

    //--var q = dc.v_ReceivingPOLineLinqs.Where(loginUser).Select("");
    //var query = dc.VendorLinqs.Where(loginUser).OrderBy("CompanyName").Select("new(CompanyName as Name, Phone");
    //IFilterCondition t;
    // var q = dc.VendorLinqs.Where("");
    // string strwhere = "apsodghap";
    // var q = dc.VendorLinqs.Where(strwhere);
    var query = from u in dc.UserWorkListLinqs
                join p in dc.POLinqs on u.LinkId equals p.PoNum
                where u.UserId == loginUser && u.LinkModule.ToLower() == "purchase" && u.LinkType.ToLower() == "worklist"
                orderby p.PoNum
                select p;

    GridLinqBindingData<POLinq> resultData = RadGrid.GetBindingData<POLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
    result.Data = resultData.Data.ToList<POLinq>();
    result.Count = resultData.Count;

    return result;
  }

  [OperationContract]
  public ResultItemsVendor ItemsVendorLookup(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {
    System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
    ResultItemsVendor result = new ResultItemsVendor();
    string filter = HttpContext.Current.Request.QueryString.Get("filter");
    string vendor = HttpContext.Current.Request.QueryString.Get("vendor");
    GridLinqBindingData<v_itemvendorlist> resultData = null;
    if (filter == "1")
    {
      var q = from v in dc.v_itemvendorlists select v;

      if (vendor != "")
      {
        q = q.Where(x => x.Vendor == vendor);//from v in dc.v_itemvendorlists where v.Vendor == vendor select v;
      }
      resultData = RadGrid.GetBindingData<v_itemvendorlist>(q, startRowIndex, maximumRows, sortExpression, filterExpression);
    }
    //else if (filter == "0")
    //{
    //    var q0 = (from i in dc.ItemLinqs
    //              join v in dc.ItemVendorLinqs on i.ItemNum equals v.ItemNum into iv
    //              from sub in iv.DefaultIfEmpty()
    //              orderby i.ItemNum
    //              select new v_itemvendorlist
    //              {
    //                  ItemNum = i.ItemNum,
    //                  ItemDesc = i.ItemDesc,
    //                  IssueUnit = i.IssueUnit
    //              }).Distinct();

    //    resultData = RadGrid.GetBindingData<v_itemvendorlist>(q0, startRowIndex, maximumRows, sortExpression, filterExpression);
    //}
    result.Data = resultData.Data.ToList<v_itemvendorlist>();
    result.Count = resultData.Count;

    return result;
  }

  [OperationContract]
  public ResultPORecentlist RecentlistDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {
    System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
    ResultPORecentlist result = new ResultPORecentlist();
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

    var query = from v in dc.v_PORecentlists where v.UserId.ToLower() == loginUser.ToLower() orderby v.AccessDate descending select v;
    GridLinqBindingData<v_PORecentlist> resultData = RadGrid.GetBindingData<v_PORecentlist>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
    result.Data = resultData.Data.ToList<v_PORecentlist>();
    result.Count = resultData.Count;

    return result;
  }

  [OperationContract]
  public POGenerateLinq GeneratePODataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {
    System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
    POGenerateLinq result = new POGenerateLinq();
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

    var query = from v in dc.v_generatepos select v;
    GridLinqBindingData<v_generatepo> resultData = RadGrid.GetBindingData<v_generatepo>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
    result.Data = resultData.Data.ToList<v_generatepo>();
    result.Count = resultData.Count;

    return result;
  }



  [OperationContract]
  public AddRequestToPOLinq AddRequestToPODataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {
    System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
    AddRequestToPOLinq result = new AddRequestToPOLinq();
    //string whereclause = HttpContext.Current.Request.QueryString.Get("wherestring");
    string ponum = HttpContext.Current.Request.QueryString.Get("ponum");
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

    var query = from v in dc.v_addrequesttopos select v;
    string vnum = dc.POLinqs.Where(x => x.PoNum == ponum).Select(x=>x.Vendor).FirstOrDefault().ToString();
    query = query.Where(x => x.Vendor == null || x.Vendor == vnum );
    if (HttpContext.Current.Application["UseDivision"] != null)
    {
      if (HttpContext.Current.Application["UseDivision"].ToString().ToLower() == "yes")
      {
        if (HttpContext.Current.Session["EditableDivision"].ToString() != null)
        {
          query = query.Where(x => x.Division == null || x.Division == HttpContext.Current.Session["EditableDivision"].ToString());
        }
        else
        {
          query = query.Where(x => x.Division == null);
        }
      }
    }

    GridLinqBindingData<v_addrequesttopo> resultData = RadGrid.GetBindingData<v_addrequesttopo>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
    result.Data = resultData.Data.ToList<v_addrequesttopo>();
    result.Count = resultData.Count;

    return result;
  }

  [OperationContract]
  public ResultsPOLinq SearchListData(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {
    System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
    ResultsPOLinq result = new ResultsPOLinq();
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
    //filterExpression = "StatusCode >= 200  and  StatusCode < 300 AND  (Division.Equals(null)  or Division.Equals(\"1\")  or Division.Equals(\"1.1\")  or Division.Equals(\"1.1.1\")  or Division.Equals(\"2\")  or Division.Equals(\"2.1\")  or Division.Equals(\"2.1.1\")  or Division.Equals(\"2.2\")  or Division.Equals(\"2.3\")  or Division.Equals(\"3\")  or Division.Equals(\"3.1\")  or Division.Equals(\"3.1.1\")  or Division.Equals(\"4\") )";
    var query = from p in dc.POLinqs select p;

    //List<POLinq> polist = query.ToList();

    //List<POLinq> pl = new List<POLinq>();
    //pl.Add(new POLinq {PoNum= });
    //pl.
    //POLinq pl = new POLinq();
    //pl.PoNum = pl.PoNum.n

    GridLinqBindingData<POLinq> resultData = RadGrid.GetBindingData<POLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

    result.Data = resultData.Data.ToList<POLinq>();
    result.Count = resultData.Count;

    return result;
  }
    
  [OperationContract]
  [WebGet]
  public ResultsPOLinq QueryResultPO(int startRowIndex, int maximumRows, string sortExpression, string filterExpression, string jsonData)
  {
    System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
    ResultsPOLinq result = new ResultsPOLinq();
    string searchcriteria;
    List<string> strlist = new List<string>();

    searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";

    NameValueCollection nvc = AzzierData.DeserializeJSON(jsonData);
    object empid = HttpContext.Current.Session["LogIn"];
    string userid = (empid != null) ? empid.ToString() : "";
    PO objPO = new PO(userid, "PO", "PoNum");
    string wherestring = "", linqwherestring = "";
    if (jsonData.Contains("queryID"))
      nvc = UserQuery.CreateNameValueCollection(nvc["queryID"]);
    objPO.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
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
    
      var query = from p in dc.POLinqs select p;

    //List<POLinq> polist = query.ToList();

    //List<POLinq> pl = new List<POLinq>();
    //pl.Add(new POLinq {PoNum= });
    //pl.
    //POLinq pl = new POLinq();
    //pl.PoNum = pl.PoNum.n

    GridLinqBindingData<POLinq> resultData = RadGrid.GetBindingData<POLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

    result.Data = resultData.Data.ToList<POLinq>();
    result.Count = resultData.Count;

    return result;
  }
  [OperationContract]
  public ResultCurrencyRateLinq GetCurrencyAndRate(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {
    System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
    ResultCurrencyRateLinq result = new ResultCurrencyRateLinq();
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

    if (System.Web.HttpContext.Current.Request.QueryString.Get("where") != null)
    {
      searchcriteria = System.Web.HttpContext.Current.Request.QueryString.Get("where");
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
    var query = from c in dc.v_currchangerates select c;

    //List<POLinq> pl = new List<POLinq>();
    //pl.Add(new POLinq {PoNum= });
    //pl.
    //POLinq pl = new POLinq();
    //pl.PoNum = pl.PoNum.n

    GridLinqBindingData<v_currchangerate> resultData = RadGrid.GetBindingData<v_currchangerate>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

    result.Data = resultData.Data.ToList<v_currchangerate>();
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

    var q = from r in xdoc.Descendants("ponum")
            join p in dc.POLinqs on r.Value equals p.PoNum
            where !(from u in dc.UserWorkListLinqs
                    where u.LinkModule.ToLower() == "purchase" && u.LinkType.ToLower() == "worklist" && u.UserId == loginUser
                    select u.LinkId).Contains(r.Value)
            select r.Value;

    List<UserWorkListLinq> worklist = new List<UserWorkListLinq>();
    foreach (var ponum in q)
    {
      worklist.Add(new UserWorkListLinq { LinkId = ponum, LinkModule = "purchase", LinkType = "worklist", UserId = loginUser, TransDate = System.DateTime.Now });
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
  public string RemovefromWorklist(string paramXML)
  {
    string retStr = "Success.", loginUser;
    if (System.Web.HttpContext.Current.Session["Login"] != null)
    { loginUser = System.Web.HttpContext.Current.Session["Login"].ToString(); }
    else { return "Please login."; }

    XDocument ponum = XDocument.Parse(paramXML);

    var worklist = from u in dc.UserWorkListLinqs
                   where u.LinkModule.ToLower() == "purchase" && u.UserId == loginUser && u.LinkType.ToLower() == "worklist" &&
                   (from p in ponum.Descendants("ponum") select p.Value).Contains(u.LinkId)
                   select u;

    try
    {
      dc.UserWorkListLinqs.DeleteAllOnSubmit(worklist);
      dc.SubmitChanges();
    }
    catch (Exception e)
    { retStr = e.Message; }

    return retStr;
  }

  [OperationContract]
  [WebGet]
  public string getPOTotalAmount(string ponum)
  {
    string retStr = "";
    var po = (from p in dc.POLinqs where p.PoNum == ponum select p).SingleOrDefault();
    retStr = "{\"total\":\"" + po.PoTotal.ToString() + "\",\"subtotal\":\"" + po.SubTotal.ToString() + "\",\"tax1\":\"" + po.Tax1Total.ToString() + "\",\"tax2\":\"" + po.Tax2Total.ToString() + "\"}";
    return retStr;
  }

  [OperationContract]
  [WebGet]
  public string getterm(string vendor)
  {
    string retStr = "";
    Vendor objvendor = new Vendor(System.Web.HttpContext.Current.Session["Login"].ToString());
    retStr = objvendor.GetTermDescription(vendor);
    return retStr;
  }

  protected Decimal tax1 = 0;
  protected Decimal tax2 = 0;
  protected string vendterms = "";
  protected string vendcurr = "";
  protected Decimal exchrate = 1;
  protected Boolean HasPOLine = false;
  protected Decimal subtotal = 0;
  protected Decimal tax1total = 0;
  protected Decimal tax2total = 0;
  protected Decimal total = 0;
  protected Decimal pobasetotal = 0;
  protected int linenum = 1;
  protected OleDbCommand cmdInv;
  protected OleDbDataReader drInv;
  protected string curponum = "";
  protected string status = "WTAPPR";
  protected string statuscode = "30";
  protected int po10 = 0;
  protected string curvend = "@@@@";
  protected string ship = "Null";
  protected string sadd1 = "Null";
  protected string sadd2 = "Null";
  protected string sadd3 = "Null";
  protected string sphone = "Null";
  protected string bill = "Null";
  protected string badd1 = "Null";
  protected string badd2 = "Null";
  protected string badd3 = "Null";
  protected string bphone = "Null";
  protected string userdiv = "";
  protected string[] povend;

  [OperationContract]
  [WebGet(BodyStyle = WebMessageBodyStyle.WrappedRequest,
      RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
  public string GeneratePO(string counterlist, string autonum, string ponumlist, string vendorlist)
  {
    // string returnStr = "Entered Code does not exist or does not meet applied conditions.";
    string returnStr = "";
    string shipto = "";
    string billto = "";
    string userdiv = "";

    string connectionString = HttpContext.Current.Application["ConnString"].ToString();

    OleDbConnection conn = new OleDbConnection(connectionString);
    conn.Open();

    if (HttpContext.Current.Application["Tax1"] != null)
    {
      tax1 = Convert.ToDecimal(HttpContext.Current.Application["Tax1"].ToString());
    }

    if (HttpContext.Current.Application["Tax2"] != null)
    {
      tax2 = Convert.ToDecimal(HttpContext.Current.Application["Tax2"].ToString());
    }

    if (HttpContext.Current.Application["AutoApprovePO"] != null)
    {
      if (HttpContext.Current.Application["AutoApprovePO"].ToString().ToUpper() == "YES")
      {
        status = "APPR";
        statuscode = "200";
        po10 = 1;
      }
    }

    if (HttpContext.Current.Application["UserDiv"] != null)
    {
      userdiv = HttpContext.Current.Session["UserDiv"].ToString();
    }

    if (ponumlist != "")
    {
      povend = ponumlist.Split(';');
    }

    OleDbCommand cmd = new OleDbCommand();
    OleDbDataReader dr;
    cmd.Connection = conn;

    cmd.CommandText = "SELECT Description FROM Terms WHERE CodeType='PO' AND DefaultCode <> 0";
    dr = cmd.ExecuteReader();
    while (dr.Read())
    {
      vendterms = dr[0].ToString();
    }
    dr.Close();

    cmd.CommandText = "SELECT * FROM ShipTo WHERE DefaultAddress <> 0";
    dr = cmd.ExecuteReader();
    while (dr.Read())
    {
      if (dr[dr.GetOrdinal("ShipType")].ToString().ToUpper() == "SHIP")
      {
        ship = ConvString(dr[dr.GetOrdinal("ShipName")].ToString());
        sadd1 = ConvString(dr[dr.GetOrdinal("Address1")].ToString());
        sadd2 = ConvString(dr[dr.GetOrdinal("Address2")].ToString());
        sadd3 = ConvString(dr[dr.GetOrdinal("Address3")].ToString());
        sphone = ConvString(dr[dr.GetOrdinal("PhoneNumber")].ToString());
      }
      else if (dr[dr.GetOrdinal("ShipType")].ToString().ToUpper() == "BILL")
      {
        bill = ConvString(dr[dr.GetOrdinal("ShipName")].ToString());
        badd1 = ConvString(dr[dr.GetOrdinal("Address1")].ToString());
        badd2 = ConvString(dr[dr.GetOrdinal("Address2")].ToString());
        badd3 = ConvString(dr[dr.GetOrdinal("Address3")].ToString());
        bphone = ConvString(dr[dr.GetOrdinal("PhoneNumber")].ToString());
      }
      else if (dr[dr.GetOrdinal("ShipType")].ToString().ToUpper() == "BOTH")
      {
        ship = ConvString(dr[dr.GetOrdinal("ShipName")].ToString());
        sadd1 = ConvString(dr[dr.GetOrdinal("Address1")].ToString());
        sadd2 = ConvString(dr[dr.GetOrdinal("Address2")].ToString());
        sadd3 = ConvString(dr[dr.GetOrdinal("Address3")].ToString());
        sphone = ConvString(dr[dr.GetOrdinal("PhoneNumber")].ToString());
        bill = ConvString(dr[dr.GetOrdinal("ShipName")].ToString());
        badd1 = ConvString(dr[dr.GetOrdinal("Address1")].ToString());
        badd2 = ConvString(dr[dr.GetOrdinal("Address2")].ToString());
        badd3 = ConvString(dr[dr.GetOrdinal("Address3")].ToString());
        bphone = ConvString(dr[dr.GetOrdinal("PhoneNumber")].ToString());
      }
    }
    dr.Close();

    if (HttpContext.Current.Session["UserDiv"] != null)
    {
      userdiv = HttpContext.Current.Session["UserDiv"].ToString();
    }

    if (HttpContext.Current.Application["UseDivision"] != null)
    {
      if (HttpContext.Current.Application["UseDivision"].ToString().ToLower() == "yes")
      {
        if (HttpContext.Current.Session["ShipTo"] == "")
        {
          shipto = AzzierData.GetDivDefault("Ship To", userdiv);
          billto = AzzierData.GetDivDefault("Bill To", userdiv);
        }
        else
        {
          if (HttpContext.Current.Session["ShipTo"] != null)
          {
            shipto = HttpContext.Current.Session["ShipTo"].ToString();
          }
          if (HttpContext.Current.Session["BillTo"] != null)
          {
            billto = HttpContext.Current.Session["BillTo"].ToString();
          }
        }

        cmd.CommandText = "SELECT * FROM ShipTo WHERE ShipName = '" + shipto + "'";
        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
          ship = ConvString(dr[dr.GetOrdinal("ShipName")].ToString());
          sadd1 = ConvString(dr[dr.GetOrdinal("Address1")].ToString());
          sadd2 = ConvString(dr[dr.GetOrdinal("Address2")].ToString());
          sadd3 = ConvString(dr[dr.GetOrdinal("Address3")].ToString());
          sphone = ConvString(dr[dr.GetOrdinal("PhoneNumber")].ToString());
        }
        dr.Close();

        cmd.CommandText = "SELECT * FROM ShipTo WHERE ShipName = '" + billto + "'";
        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
          bill = ConvString(dr[dr.GetOrdinal("ShipName")].ToString());
          badd1 = ConvString(dr[dr.GetOrdinal("Address1")].ToString());
          badd2 = ConvString(dr[dr.GetOrdinal("Address2")].ToString());
          badd3 = ConvString(dr[dr.GetOrdinal("Address3")].ToString());
          bphone = ConvString(dr[dr.GetOrdinal("PhoneNumber")].ToString());
        }
        dr.Close();
      }
    }

    cmd.CommandText = "SELECT Description FROM Terms WHERE CodeType='PO' AND DefaultCode <> 0";
    dr = cmd.ExecuteReader();
    while (dr.Read())
    {
      vendterms = dr[0].ToString();
    }
    dr.Close();

    cmdInv = new OleDbCommand();
    cmdInv.CommandText = "SELECT V.* FROM v_InventoryStoreroom V WHERE V.Counter IN (" + counterlist + ") ORDER BY V.DefVendor,V.ItemNum,V.StoreRoom";
    cmdInv.Connection = conn;
    drInv = cmdInv.ExecuteReader();

    while (drInv.Read())
    {
      if (drInv[drInv.GetOrdinal("defvendor")].ToString() != curvend)
      {
        if (HasPOLine)
        {
          CreatePORecord(curvend);
        }

        GetVendorTax(drInv[drInv.GetOrdinal("defvendor")].ToString());

        if (autonum == "1")
        {
          NewNumber num = new NewNumber();
          curponum = num.GetNextNumber("PO");
        }
        else
        {
          foreach (string item1 in povend)
          {
            if (item1 != "")
            {
              string[] ponumvendor = item1.Split('^');

              if (ponumvendor[0] == drInv[drInv.GetOrdinal("defvendor")].ToString())
              {
                curponum = ponumvendor[1];
              }
            }
          }
        }

        curvend = drInv[drInv.GetOrdinal("defvendor")].ToString();

        HasPOLine = false;
        subtotal = 0;
        tax1total = 0;
        tax2total = 0;
        total = 0;
        pobasetotal = 0;
        linenum = 1;

        returnStr = returnStr + curvend + "^" + curponum + ";";
      }

      CreatePOLine(curvend);
    }
    drInv.Close();

    if (HasPOLine)
    {
      CreatePORecord(curvend);
    }

    return returnStr;
  }

  protected void CreatePORecord(string vend)
  {
    string connectionString = HttpContext.Current.Application["ConnString"].ToString();

    OleDbConnection conn = new OleDbConnection(connectionString);
    conn.Open();

    OleDbCommand cmd = new OleDbCommand();

    string sSQL = "INSERT INTO PO (PoNum,OpenDate,Status,OpenPO,Vendor,SubTotal,Tax1Total,Tax2Total,PoTotal,Freight,ShipTo,ShipAddress1,ShipAddress2,ShipAddress3,";
    sSQL = sSQL + "ShipPhone,BillTo,BillAddress1,BillAddress2,BillAddress3,BillPhone,Empid,Terms,po10,Division,DirtyLog,VendorCurrency,ExchangeRate,BaseCurrencyTotal) VALUES ('";
    sSQL = sSQL + curponum + "',getdate(),'" + status + "',1,'" + vend + "'," + subtotal + "," + tax1total + "," + tax2total + "," + total + ",0," + ship + ",";
    sSQL = sSQL + sadd1 + "," + sadd2 + "," + sadd3 + "," + sphone + "," + bill + "," + badd1 + "," + badd2 + "," + badd3 + "," + bphone + ",'";
    sSQL = sSQL + HttpContext.Current.Session["Login"] + "'," + ConvString(vendterms) + "," + po10 + "," + ConvString(userdiv) + ",0,";
    sSQL = sSQL + ConvString(vendcurr) + "," + exchrate + "," + pobasetotal + ")";

    cmd.CommandText = sSQL;
    cmd.Connection = conn;
    cmd.ExecuteNonQuery();

    conn.Close();
  }

  protected void CreatePOLine(string vend)
  {
    Decimal linetotal = 0;
    Decimal linetax1 = 0;
    Decimal linetax2 = 0;
    Decimal thistotal = 0;
    Decimal basetotal = 0;

    string connectionString = HttpContext.Current.Application["ConnString"].ToString();

    OleDbConnection conn = new OleDbConnection(connectionString);
    conn.Open();

    OleDbCommand cmd = new OleDbCommand();
    OleDbCommand cmd1 = new OleDbCommand();

    cmd.CommandText = "SELECT iv.*, i.purchaseprice FROM ItemVendor iv LEFT JOIN items i ON iv.itemnum = i.itemnum WHERE iv.ItemNum = '" + drInv[drInv.GetOrdinal("ItemNum")].ToString() + "' AND iv.Vendor = '" + vend + "'";
    cmd.Connection = conn;
    OleDbDataReader dr = cmd.ExecuteReader();
    if (dr.Read())
    {
      Decimal qtyonordr = Convert.ToDecimal(drInv[drInv.GetOrdinal("QtyOnOrder")].ToString());
      Decimal maxstock = Convert.ToDecimal(drInv[drInv.GetOrdinal("MaxStock")].ToString());
      Decimal stock = Convert.ToDecimal(drInv[drInv.GetOrdinal("QtyOnHand")].ToString());
      Decimal cfactor = Convert.ToDecimal(dr[dr.GetOrdinal("Conversion")].ToString());
      Decimal vendprice = Convert.ToDecimal(dr[dr.GetOrdinal(dr[dr.GetOrdinal("PurchasePrice")].ToString())].ToString());
      Decimal reserves = Convert.ToDecimal(drInv[drInv.GetOrdinal("QtyReserved")].ToString());
      Decimal qty = 0;
      if (drInv[drInv.GetOrdinal("ReorderMethod")].ToString() == "")
      {
        qty = maxstock / cfactor;
      }
      else
      {
        qty = (maxstock - stock - qtyonordr + reserves) / cfactor;
      }
      if (qty < 0) qty = 0;

      linetotal = Math.Round(vendprice * qty, 2);
      linetax1 = Math.Round(linetotal * tax1, 2);
      linetax2 = Math.Round(linetotal * tax2, 2);
      thistotal = Math.Round(linetotal + linetax1 + linetax2, 2);
      basetotal = Math.Round(thistotal * exchrate, 2);

      string sSQL = "INSERT INTO POLine (PoNum,LineNum,ItemNum,ItemDesc,IsInventory,StoreRoom,DrAccount,Vendor,OpenFlag,OrderQty,IssueUnit,PurchaseUnit,Conversion,";
      sSQL = sSQL + "Price,Tax1,Tax2,Subtotal,TotalCost,Operator,Division,VendorCurrency,ExchangeRate,BaseCurrencyTotal) VALUES ('" + curponum + "',";
      sSQL = sSQL + Convert.ToString(linenum) + ",'" + drInv[drInv.GetOrdinal("ItemNum")].ToString() + "'," + ConvString(drInv[drInv.GetOrdinal("ItemDesc")].ToString());
      sSQL = sSQL + ",1,'" + drInv[drInv.GetOrdinal("StoreRoom")].ToString() + "'," + ConvString(drInv[drInv.GetOrdinal("DrAccount")].ToString()) + ",'" + vend + "','O'," + qty + ",'";
      sSQL = sSQL + drInv[drInv.GetOrdinal("IssueUnit")].ToString() + "','" + dr[dr.GetOrdinal("PurchaseUnit")].ToString() + "',";
      sSQL = sSQL + Convert.ToString(cfactor) + "," + vendprice + "," + linetax1 + "," + linetax2 + "," + linetotal + "," + thistotal + ",'" + HttpContext.Current.Session["Login"] + "',";
      sSQL = sSQL + ConvString(userdiv) + "," + ConvString(vendcurr) + "," + exchrate + "," + basetotal + ")";

      cmd1.CommandText = sSQL;
      cmd1.Connection = conn;
      cmd1.ExecuteNonQuery();

      subtotal = subtotal + linetotal;
      tax1total = tax1total + linetax1;
      tax2total = tax2total + linetax2;
      total = total + thistotal;
      pobasetotal = pobasetotal + basetotal;
      HasPOLine = true;

      // ** Update quantity in Inventory table
      if (po10 == 1)
      {
        sSQL = "UPDATE Inventory SET QtyOnOrder=QtyOnOrder + " + (qty * cfactor) + ",ModifyDate=getdate() WHERE ItemNum='" + drInv[drInv.GetOrdinal("ItemNum")].ToString() + "' AND StoreRoom='" + drInv[drInv.GetOrdinal("StoreRoom")].ToString() + "'";
        cmd1.CommandText = sSQL;
        cmd1.ExecuteNonQuery();
      }

      //ItemVendor curiv = new ItemVendor(HttpContext.Current.Session["Login"].ToString(), "ItemVendor", "ItemNum");
      //curiv.UpdateVendorLastPrice(drInv[drInv.GetOrdinal("ItemNum")].ToString(), vend, vendprice, false);
    }
    dr.Close();

  }

  protected void GetVendorTax(string vendor)
  {
    string connectionString = HttpContext.Current.Application["ConnString"].ToString();

    OleDbConnection conn = new OleDbConnection(connectionString);
    conn.Open();

    OleDbCommand cmd = new OleDbCommand();
    cmd.CommandText = "SELECT V.Tax1,V.Tax2,V.VendTerms,V.VendorCurrency,T.Description FROM Vendor V LEFT JOIN Terms T ON V.VendTerms=T.Code WHERE CompanyCode='" + vendor + "'";
    cmd.Connection = conn;
    OleDbDataReader dr = cmd.ExecuteReader();
    while (dr.Read())
    {
      if (dr[0].ToString() != "")
      {
        tax1 = Convert.ToDecimal(dr[0].ToString());
      }
      if (dr[1].ToString() != "")
      {
        tax2 = Convert.ToDecimal(dr[1].ToString());
      }
      if (dr[3].ToString() != "")
      {
        vendcurr = dr[3].ToString();
      }
      if (dr[4].ToString() != "")
      {
        vendterms = dr[4].ToString();
      }
    }
    dr.Close();

    string homecurr = HttpContext.Current.Application["BaseCurrency"].ToString();
    if (vendcurr != "" && homecurr != "" && vendcurr != homecurr)
    {
      cmd.CommandText = "SELECT TOP 1 ExchangeRate FROM ExchangeRate WHERE BaseCurrency='" + homecurr + "' AND ForeignCurrency='" + vendcurr + "' ORDER BY ExchangeDate DESC";
      dr = cmd.ExecuteReader();
      if (dr.Read())
      {
        exchrate = Convert.ToDecimal(dr[0].ToString());
      }
      dr.Close();
      if (exchrate == 0)
      {
        exchrate = 1;
      }
    }
  }

  protected string ConvString(string sendfield)
  {
    string resultStr = "";

    if (sendfield == null || sendfield == "")
    {
      resultStr = "NULL";
    }
    else
    {
      resultStr = "'" + sendfield.Replace("'", "''") + "'";
    }

    return resultStr;
  }

  [OperationContract]
  public ResultTermsLinq GetTermList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {
      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
      ResultTermsLinq result = new ResultTermsLinq();
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

      var query = from term in dc.TermsLinqs select term;
      GridLinqBindingData<TermsLinq> resultData = RadGrid.GetBindingData<TermsLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
      result.Data = resultData.Data.ToList<TermsLinq>();
      result.Count = resultData.Count;

      return result;
  }

  [OperationContract]
  public ResultShipToLinq GetShipToList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {
      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
      ResultShipToLinq result = new ResultShipToLinq();
      string lookupfilter = HttpContext.Current.Request.QueryString.Get("where");

      if (!string.IsNullOrEmpty(lookupfilter))
      {
          lookupfilter = lookupfilter.Replace("'", "\"");
          if (!string.IsNullOrEmpty(filterExpression))
              filterExpression = filterExpression + " and " + lookupfilter;
          else
              filterExpression = lookupfilter;
      }

      var query = from s in dc.ShipToLinqs select s;
      GridLinqBindingData<ShipToLinq> resultData = RadGrid.GetBindingData<ShipToLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

      result.Data = resultData.Data.ToList<ShipToLinq>();
      result.Count = resultData.Count;

      return result;
  }

  [OperationContract]
  public ResultsPOLineLinq GetUnapprPOList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
  {
    System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
    ResultsPOLineLinq result = new ResultsPOLineLinq();
    string lookupfilter = HttpContext.Current.Request.QueryString.Get("where");
    string[] curpolist;
    curpolist = lookupfilter.Split(',');
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

    var query = from p in dc.v_ReceivingPOLines where curpolist.Contains(p.PoNum) select p ;
    GridLinqBindingData<v_ReceivingPOLine> resultData = RadGrid.GetBindingData<v_ReceivingPOLine>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
    result.Data = resultData.Data.ToList<v_ReceivingPOLine>();
    result.Count = resultData.Count;

    return result;
  }

  protected class PoTotalResult
  {
    public string subtotal { get; set; }
    public string tax1 { get; set; }
    public string tax2 { get; set; }
    public string total { get; set; }
  }

  public class ResultPORecentlist
  {
    public int Count { get; set; }
    public List<v_PORecentlist> Data { get; set; }
  }

  public class ResultCurrencyRateLinq
  {
    public int Count { get; set; }
    public List<v_currchangerate> Data { get; set; }
  }

  public class POGenerateLinq
  {
    public int Count { get; set; }
    public List<v_generatepo> Data { get; set; }
  }

  public class AddRequestToPOLinq
  {
    public int Count { get; set; }
    public List<v_addrequesttopo> Data { get; set; }
  }
}

public class ResultsPOLinq
{
  public int Count { get; set; }
  public List<POLinq> Data { get; set; }
}

public class ResultsPOLineLinq
{
  public int Count { get; set; }
  public List<v_ReceivingPOLine> Data { get; set; }
}

public class ResultItemsVendor
{
  public int Count { get; set; }
  //public List<ItemLinq> Data { get; set; }
  public List<v_itemvendorlist> Data { get; set; }
}

public class ResultTermsLinq
{
    public int Count { get; set; }
    public List<TermsLinq> Data { get; set; }
}

public class ResultShipToLinq
{
    public int Count { get; set; }
    public List<ShipToLinq> Data { get; set; }
}

