using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using Telerik.Web.UI;
using System.Web;
using System.Xml.Linq;
using System.Collections.Specialized;
using System.Xml;
using System.Data;
using System.Data.OleDb;

[ServiceContract(Namespace = "")]
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class ServiceInventory
{
    protected DataClassesDataContext dc = new DataClassesDataContext(AzzierData.LinqConnectstring);
    protected string lookcriteria, mode = "", login_user = "";
    protected List<string> division = new List<string>();
    protected HttpContext context = HttpContext.Current;
    // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
    // To create an operation that returns XML,
    //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
    //     and include the following line in the operation body:
    //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";


    [OperationContract]
    public ResultInventoryTransDetailLinq GetTransactionHistory(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
      ResultInventoryTransDetailLinq result = new ResultInventoryTransDetailLinq();
      string lookupfilter = HttpContext.Current.Request.QueryString.Get("where");

      if (!string.IsNullOrEmpty(lookupfilter))
      {
        lookupfilter = lookupfilter.Replace("'", "\"");
        if (!string.IsNullOrEmpty(filterExpression))
          filterExpression = filterExpression + " and " + lookupfilter;
        else
          filterExpression = lookupfilter;
      }

      var query = from log in dc.v_InventoryTransDetails orderby log.TransDate descending,log.Counter descending select log;

      GridLinqBindingData<v_InventoryTransDetail> resultData = RadGrid.GetBindingData<v_InventoryTransDetail>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
      result.Data = resultData.Data.ToList<v_InventoryTransDetail>();
      //        result.Data = resultData.Data.OfType<LocationLinq>().ToList();
      result.Count = resultData.Count;// query.Count();//800;// resultData.Count;

      return result;
    }

    [OperationContract]
    public ResultCompEquipmentLinq GetwhereUsed(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
      ResultCompEquipmentLinq result = new ResultCompEquipmentLinq();
      string lookupfilter = HttpContext.Current.Request.QueryString.Get("wherestring");

      if (!string.IsNullOrEmpty(lookupfilter))
      {
        lookupfilter = lookupfilter.Replace("'", "\"");
        if (!string.IsNullOrEmpty(filterExpression))
          filterExpression = filterExpression + " and " + lookupfilter;
        else
          filterExpression = lookupfilter;
      }

      var query = from e in dc.v_CompEquipmentLists orderby e.Equipment select e;

      GridLinqBindingData<v_CompEquipmentList> resultData = RadGrid.GetBindingData<v_CompEquipmentList>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
      result.Data = resultData.Data.ToList<v_CompEquipmentList>();
      //        result.Data = resultData.Data.OfType<LocationLinq>().ToList();
      result.Count = resultData.Count;// query.Count();//800;// resultData.Count;

      return result;
    }

    [OperationContract]
    public ResultInventoryPosition GetPositionList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
      ResultInventoryPosition result = new ResultInventoryPosition();
      string lookupfilter = HttpContext.Current.Request.QueryString.Get("wherestring");

      if (!string.IsNullOrEmpty(lookupfilter))
      {
        lookupfilter = lookupfilter.Replace("'", "\"");
        if (!string.IsNullOrEmpty(filterExpression))
          filterExpression = filterExpression + " and " + lookupfilter;
        else
          filterExpression = lookupfilter;
      }

      var query = from e in dc.v_InventoryPositions select e;

      GridLinqBindingData<v_InventoryPosition> resultData = RadGrid.GetBindingData<v_InventoryPosition>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
      result.Data = resultData.Data.ToList<v_InventoryPosition>();
      //        result.Data = resultData.Data.OfType<LocationLinq>().ToList();
      result.Count = resultData.Count;// query.Count();//800;// resultData.Count;

      return result;
    }

    [OperationContract]
    public ResultActiveInventoryUsage GetActiveInventoryUsage(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
      ResultActiveInventoryUsage result = new ResultActiveInventoryUsage();
      string lookupfilter = HttpContext.Current.Request.QueryString.Get("wherestring");

      if (!string.IsNullOrEmpty(lookupfilter))
      {
        lookupfilter = lookupfilter.Replace("'", "\"");
        if (!string.IsNullOrEmpty(filterExpression))
          filterExpression = filterExpression + " and " + lookupfilter;
        else
          filterExpression = lookupfilter;
      }

      var query = from e in dc.v_ActiveInventoryUsages orderby e.OrderType.ToUpper(),e.WoNum select e  ;

      GridLinqBindingData<v_ActiveInventoryUsage> resultData = RadGrid.GetBindingData<v_ActiveInventoryUsage>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
      result.Data = resultData.Data.ToList<v_ActiveInventoryUsage>();
      //        result.Data = resultData.Data.OfType<LocationLinq>().ToList();
      result.Count = resultData.Count;// query.Count();//800;// resultData.Count;

      return result;
    }

    [OperationContract]
    public ResultItemsLinq SearchDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultItemsLinq result = new ResultItemsLinq();
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

        var query = from i in dc.ItemLinqs select i;
        GridLinqBindingData<ItemLinq> data = RadGrid.GetBindingData<ItemLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

//        GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "ItemLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = data.Data.OfType<ItemLinq>().ToList();
        result.Count = data.Count;

        return result;
    }

    [OperationContract]
    [WebGet]
    public ResultItemsLinq QueryResultInv(int startRowIndex, int maximumRows, string sortExpression, string filterExpression, string jsonData)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultItemsLinq result = new ResultItemsLinq();
        string searchcriteria;
        List<string> eqptlist = new List<string>();

        searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";

        NameValueCollection nvc = AzzierData.DeserializeJSON(jsonData);
        object empid = HttpContext.Current.Session["LogIn"];
        string userid = (empid != null) ? empid.ToString() : "";
        ModuleoObject objInv = new ModuleoObject(userid, "Items", "ItemNum");
        string wherestring = "", linqwherestring = "";
        if (jsonData.Contains("queryID"))
          nvc = UserQuery.CreateNameValueCollection(nvc["queryID"]);
        objInv.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
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

        var query = from i in dc.ItemLinqs select i;
        GridLinqBindingData<ItemLinq> data = RadGrid.GetBindingData<ItemLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        //        GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "ItemLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = data.Data.OfType<ItemLinq>().ToList();
        result.Count = data.Count;

        return result;
    }

    [OperationContract]
    public ResultItemsLinq GetWorkList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        
        ResultItemsLinq result = new ResultItemsLinq();

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
                    join i in dc.ItemLinqs on u.LinkId equals i.ItemNum
                    where i.Inactive == 0 && u.LinkModule == "inventory" && u.LinkType == "worklist" && u.UserId.ToLower() == login_user.ToLower()
                    orderby u.LinkId
                    select i;

        GridLinqBindingData<ItemLinq> data = RadGrid.GetBindingData<ItemLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = data.Data.ToList<ItemLinq>();
        result.Count = data.Count;

        return result;
    }

    [OperationContract]
    [WebGet]
    public string SaveItemVendor(string counter, string xml)
    {
        string returnStr = "";
        bool success = false;
        string userid = "";
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

        ItemVendor obj;
        if (counter == "")
        {
            obj = new ItemVendor(userid, "ItemVendor", "Counter");
            success = obj.Create(nvc);
        }
        else
        {
            obj = new ItemVendor(userid, "ItemVendor", "Counter", counter);
            success = obj.Update(nvc);
        }

        if (success)
            return "OK";
        else
            return obj.ErrorMessage;
    }

    [OperationContract]
    [WebGet]
    public string DeleteItemVendor(string counter)
    {
        string returnStr = "";
        bool success = false;
        string userid = "";
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
        NameValueCollection nvc;
        nvc = new NameValueCollection();

        ModuleoObject obj = new ModuleoObject(userid, "ItemVendor", "Counter", counter);
        success = obj.Delete();

        if (success)
            return "OK";
        else
            return obj.ErrorMessage;

    }

    [OperationContract]
    [WebGet]
    public string UpdatePrice(string counter, string lastprice, string avgprice)
    {
        string returnStr = "";
        bool success = false;
        string userid = "";
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

        Inventory obj = new Inventory(userid, counter);
        success = obj.ManualUpdatePrice(counter, lastprice, avgprice);
        
        if (success)
            return "OK";
        else
            return obj.ErrorMessage;

    }

    [OperationContract]
    [WebGet]
    public string UpdateStockLevel(string counter, string stocklevel)
    {
        string returnStr = "";
        bool success = false;
        string userid = "";
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

        Inventory obj = new Inventory(userid, counter);
        success = obj.ManualUpdateStockLevel(counter, stocklevel);

        if (success)
            return "OK";
        else
            return obj.ErrorMessage;

    }

    [OperationContract]
    [WebGet]
    public string DeleteInventoryItem(string counter)
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
        NameValueCollection tempnvc = new NameValueCollection();
        ModuleoObject obj = new ModuleoObject(userid, "v_InventoryStoreRoom", "Counter", counter);
        tempnvc = obj.ModuleData;

        returnStr = tempnvc["QtyOnHand"].ToString() + "---" + tempnvc["QtyReserved"];

        if ((Convert.ToDecimal(tempnvc["QtyOnHand"].ToString()) > 0) || (Convert.ToDecimal(tempnvc["QtyReserved"].ToString()) > 0))
        {
            returnStr = "The item cannot be deleted.";
        }
        else 
        {
            InvMain item = new InvMain(userid, "InvMain", "Counter",counter);
            string itemnum = item.ModuleData["itemnum"].ToString();
            string storeroom = item.ModuleData["storeroom"].ToString();
            item.Delete();
            List<InvLotLinq> invlotlist = dc.InvLotLinqs.Where(x => (x.StoreRoom == storeroom) && (x.ItemNum == itemnum)).ToList();
            try
            {
                dc.InvLotLinqs.DeleteAllOnSubmit(invlotlist);
                dc.SubmitChanges();
                returnStr = "OK";
            }
            catch (Exception e)
            {
                returnStr = e.Message.ToString();
            }
        }
        return returnStr;
    }


    [OperationContract]
    [WebGet]
    public string DeleteItem(string itemnum)
    {
        string returnStr = "";
        bool success = false;
        string userid = "";
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
        NameValueCollection nvc;
        nvc = new NameValueCollection();

        Items obj = new Items(userid, "Items", "ItemNum", itemnum);
        success = obj.Delete();

        if (success)
            return "OK";
        else
            return obj.ErrorMessage;

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
                               where c.LinkModule.ToLower() == "inventory" && c.LinkType.ToLower() == "worklist" && c.UserId.ToLower() == login_user.ToLower()
                               select c.LinkId).Contains(id.Value)
                       select id.Value;

        var ins = from id in dc.ItemLinqs
                  where idresult.ToList().Contains(id.ItemNum)
                  select new
                  {
                      LinkId = id.ItemNum,
                      UserId = login_user,
                      LinkModule = "INVENTORY",
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
    public ResultPOLinq ReceivingQuery(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        
        ResultPOLinq result = new ResultPOLinq();
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

        string searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";

        var qq = from p in dc.POLinqs
                 select p;

        GridLinqBindingData<POLinq> resultData = RadGrid.GetBindingData<POLinq>(qq, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = resultData.Data.ToList<POLinq>();
        result.Count = resultData.Count;

        return result;
    }

    [OperationContract]
    public ResultAllTransBatchLinq ReceiptList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

      ResultAllTransBatchLinq result = new ResultAllTransBatchLinq();
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

      string searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";
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

      var qq = from p in dc.v_AllTransBatches
               select p;

      GridLinqBindingData<v_AllTransBatch> resultData = RadGrid.GetBindingData<v_AllTransBatch>(qq, startRowIndex, maximumRows, sortExpression, filterExpression);
      result.Data = resultData.Data.ToList<v_AllTransBatch>();
      result.Count = resultData.Count;

      return result;
    }

    [OperationContract]
    public ResultReturnVendorFrom GetReturnFromList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

      ResultReturnVendorFrom result = new ResultReturnVendorFrom();
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

      string searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";
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

      var qq = from p in dc.v_returnvendorfroms
               select p;

      GridLinqBindingData<v_returnvendorfrom> resultData = RadGrid.GetBindingData<v_returnvendorfrom>(qq, startRowIndex, maximumRows, sortExpression, filterExpression);
      result.Data = resultData.Data.ToList<v_returnvendorfrom>();
      result.Count = resultData.Count;

      return result;
    }

    [OperationContract]
    public ResultReceivingPOLineLinq ReceivingLineQuery(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

        ResultReceivingPOLineLinq result = new ResultReceivingPOLineLinq();
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

        string searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";
        //searchcriteria = "";

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

        //var query = from l in dc.v_ReceivingPOLineLinqs
 //                   where l.OrderQty > l.ReceiveQty
   //                 select l;

        var query = from l in dc.v_ReceivingPOLines
                    select l;
        
        GridLinqBindingData<v_ReceivingPOLine> data = RadGrid.GetBindingData<v_ReceivingPOLine>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        //GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "v_ReceivingPOLineLinq", startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = data.Data.OfType<v_ReceivingPOLine>().ToList();
        result.Count = data.Count;

        return result;
    }

    [OperationContract]
    [WebGet]
    public ResultReceivingPOLineLinq QueryResultReceive(int startRowIndex, int maximumRows, string sortExpression, string filterExpression, string jsonData)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultReceivingPOLineLinq result = new ResultReceivingPOLineLinq();
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

        string searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";

        jsonData = jsonData == null ? "{}" : jsonData;
        if (jsonData != "{}")
        {
            NameValueCollection nvc = AzzierData.DeserializeJSON(jsonData);
            object empid = HttpContext.Current.Session["LogIn"];
            string userid = (empid != null) ? empid.ToString() : "";
            ModuleoObject objReceive = new ModuleoObject(userid, "v_receivingpoline", "PoNum");
            string wherestring = "", linqwherestring = "";
            if (jsonData.Contains("queryID"))
                nvc = UserQuery.CreateNameValueCollection(nvc["queryID"]);
            objReceive.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
            searchcriteria = linqwherestring;
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

        var query = from l in dc.v_ReceivingPOLines
                    where l.StatusCode >= 200 && l.StatusCode < 400 && l.OrderQty>l.ReceiveQty
                    select l;

        GridLinqBindingData<v_ReceivingPOLine> data = RadGrid.GetBindingData<v_ReceivingPOLine>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = data.Data.OfType<v_ReceivingPOLine>().ToList();
        result.Count = data.Count;

        return result;
    }

    [OperationContract]
    [WebGet]
    public ResultReceivingPOLineLinq QueryResultReceiveWithReturn(int startRowIndex, int maximumRows, string sortExpression, string filterExpression, string jsonData)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultReceivingPOLineLinq result = new ResultReceivingPOLineLinq();
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

        string searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";

        jsonData = jsonData == null ? "{}" : jsonData;
        if (jsonData != "{}")
        {
            NameValueCollection nvc = AzzierData.DeserializeJSON(jsonData);
            object empid = HttpContext.Current.Session["LogIn"];
            string userid = (empid != null) ? empid.ToString() : "";
            ModuleoObject objReceive = new ModuleoObject(userid, "v_receivingpoline", "PoNum");
            string wherestring = "", linqwherestring = "";
            if (jsonData.Contains("queryID"))
                nvc = UserQuery.CreateNameValueCollection(nvc["queryID"]);
            objReceive.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
            searchcriteria = linqwherestring;
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

        var query = from l in dc.v_ReceivingPOLines
                    where l.StatusCode >= 200 && l.StatusCode < 400 && l.ReceiveQty>l.ReturnQty
                    select l;

        GridLinqBindingData<v_ReceivingPOLine> data = RadGrid.GetBindingData<v_ReceivingPOLine>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = data.Data.OfType<v_ReceivingPOLine>().ToList();
        result.Count = data.Count;

        return result;
    }

    [OperationContract]
    public ResultInventoryTransBatchLinq TransBatchQuery(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

      ResultInventoryTransBatchLinq result = new ResultInventoryTransBatchLinq();
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

      string searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";
      //searchcriteria = "";
      //string issuetype = HttpContext.Current.Request.QueryString.Get("IssueType") ?? "";
      //string issueto = HttpContext.Current.Request.QueryString.Get("IssueTo") ?? "";

      //if (issuetype == "" || issueto == "")
      //  searchcriteria = " (BatchNum.Equals(null)) ";
      //else
      //{
      //  if (issuetype.ToLower() == "workorder")
      //  {
      //    searchcriteria = " (WONum.ToLower() =='" + issueto + "')";
      //  }
      //  if (issuetype.ToLower() == "equipment")
      //  {
      //    searchcriteria = " (WONum.Equals(null)) And (Equipment.ToLower() =='" + issueto + "')";
      //  }
      //}

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
      /*
      from ca in dc.EquipmentLinqs
                            where ca.Inactive == 0 && eqptlist.Contains(ca.Equipment) && (ca.Division == null ||
                            (from o in dc.UserDivisionLinqs where o.Empid == login_user select o.Division).Contains(ca.Division))
                            select ca;
       * */
      //var query2 = from 
      /*
      var woset = (from w in dc.WOLinqs
                  where (w.Division == null || (from o in dc.UserDivisionLinqs where o.Empid == login_user && o.AllowEdit==1 select o.Division).Contains(w.Division))
                  select w.WoNum).ToList();

      var storeset = (from s in dc.StoreRoomLinqs
                   where (s.Division == null || (from o in dc.UserDivisionLinqs where o.Empid == login_user && o.AllowEdit == 1 select o.Division).Contains(s.Division))
                   select s.StoreRoom).ToList();
       * */

      /*
      var query = from l in dc.v_InventoryTransBatches
                  where l.TransType == "ISSUE"
                    &&
                    ((from w in dc.WOLinqs
                      where (w.Division == null || (from o in dc.UserDivisionLinqs where o.Empid == login_user && o.AllowEdit == 1 select o.Division).Contains(w.Division))
                            && (w.StatusCode <400 && w.StatusCode>=200)
                      select w.WoNum).Contains(l.WONum) || l.WONum == null)
                      &&
                    ((from s in dc.StoreRoomLinqs
                      where (s.Division == null || (from a in dc.UserDivisionLinqs where a.Empid == login_user && a.AllowEdit == 1 select a.Division).Contains(s.Division))
                      select s.StoreRoom).Contains(l.Storeroom))

                  select l;
       * */
      var query = from l in dc.v_InventoryTransBatches where l.TransType.ToLower() == "issue" && l.Quantity>0 select l;

      //var query = from l in dc.v_InventoryTransBatches where (woset.Contains(l.WONum) || l.WONum==null) && (storeset.Contains(l.Storeroom) || l.Storeroom == null)
                  //select l;

      GridLinqBindingData<v_InventoryTransBatch> data = RadGrid.GetBindingData<v_InventoryTransBatch>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

      
      result.Data = data.Data.OfType<v_InventoryTransBatch>().ToList();
      result.Count = data.Count;

      return result;
    }

    [OperationContract]
    public ResultItemsLinq GetAlternatePart(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);

      ResultItemsLinq result = new ResultItemsLinq();
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

      string searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";
      string itemnum = HttpContext.Current.Request.QueryString.Get("itemnum") ?? "";

      if (itemnum == "")
        return result;

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
      
      var list = from alt in dc.AlternatePartLinqs
                 where alt.ItemNum.ToLower() == itemnum.ToLower()
                 select alt.AlternateItemNum;

      var query = from l in dc.ItemLinqs
                  where list.Contains(l.ItemNum)
                  select l;

      GridLinqBindingData<ItemLinq> data = RadGrid.GetBindingData<ItemLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);


      result.Data = data.Data.OfType<ItemLinq>().ToList();
      result.Count = data.Count;

      return result;
    }

    [OperationContract]
    public ResultItemRecentlist RecentListDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultItemRecentlist result = new ResultItemRecentlist();

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

        var query = from v in dc.v_ItemRecentlists where v.UserId.ToLower() == login_user.ToLower() orderby v.AccessDate descending select v;
        
        GridLinqBindingData<v_ItemRecentlist> resultData = RadGrid.GetBindingData<v_ItemRecentlist>(query,startRowIndex,maximumRows,sortExpression,filterExpression);
        result.Data = resultData.Data.ToList<v_ItemRecentlist>();
        result.Count = resultData.Count;

        return result;
    }

    

    [OperationContract]
    [WebGet]
    public string CreateItem(string userid, string xmlnvc)
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

        Items objItem;

        objItem = new Items(userid, "Items", "ItemNum");
        success = objItem.Create(nvc);

        if (success)
        {
            returnStr = nvc["itemnum"];
        }

        return returnStr;

    }



    [OperationContract]
    [WebGet]
    public string CreateAdjustInvLot(string xml)
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
        Inventory inv = new Inventory(userid, nvc["itemnum"], nvc["storeroom"]);
        if (inv.CreateInventoryLot(nvc))
        {
            return "OK";
        }
        else
        {
            return inv.ErrorMessage;
        }

    }

    [OperationContract]
    [WebGet]
    public string UpdateInvLot(string counter, string xml)
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
      Inventory inv = new Inventory(userid);
      if (inv.UpdateInventoryLot(counter,nvc))
      {
        return "OK";
      }
      else
      {
        return inv.ErrorMessage;
      }

    }


    [OperationContract]
    [WebGet]
    public string Reserve(string xml)
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
        XmlNodeList nodelist = doc.DocumentElement.ChildNodes;
        List<NameValueCollection> nvcreserveinfo = new List<NameValueCollection>();
        for (int i = 0; i < nodelist.Count; i++)
        {
            if (nodelist[i].Name.ToLower() == "reserveinfo")
            {
                XmlNodeList reserveinfolist = nodelist[i].ChildNodes;
                NameValueCollection infonvc = new NameValueCollection();
                for (int j = 0; j < reserveinfolist.Count; j++)
                {
                    string infoname = reserveinfolist[j].Name;
                    string infoval = reserveinfolist[j].InnerText;
                    infonvc.Add(infoname, infoval);
                }
                //infonvc.Add(nvcheader);
                nvcreserveinfo.Add(infonvc);
            }
        }

        Inventory objinv = new Inventory(userid);
        if (objinv.Reserve(nvcreserveinfo))
            return "OK";
        else
            return objinv.ErrorMessage;
    }


    [OperationContract]
    [WebGet]
    public string MultipleIssue(string xml)
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

        XmlNodeList nodelist = doc.DocumentElement.ChildNodes;

        List<NameValueCollection> nvcissueinfo = new List<NameValueCollection>();
        for (int i = 0; i < nodelist.Count; i++)
        {
            if (nodelist[i].Name.ToLower() != "issueinfo")
            {
                string name = nodelist[i].Name;
                string val = nodelist[i].InnerText;
                nvcheader.Add(name, val);
            }
        }

        for (int i = 0; i < nodelist.Count; i++)
        {
            if (nodelist[i].Name.ToLower() == "issueinfo")
            {
                XmlNodeList issueinfolist = nodelist[i].ChildNodes;
                NameValueCollection infonvc = new NameValueCollection();
                for (int j = 0; j < issueinfolist.Count; j++)
                {
                    string infoname = issueinfolist[j].Name;
                    string infoval = issueinfolist[j].InnerText;
                    infonvc.Add(infoname, infoval);
                }
                infonvc.Add(nvcheader);
                nvcissueinfo.Add(infonvc);
            }

        }
        Inventory inv = new Inventory(userid);

        bool issueok = inv.MultipleIssue(nvcissueinfo);
        if (!issueok)
            returnStr= inv.ErrorMessage;
        else
            returnStr= "OK";
        return returnStr;
    }


    [OperationContract]
    [WebGet]
    public string Issue(string xml)
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

        List<NameValueCollection> nvcissueinfo = new List<NameValueCollection>();
        for (int i = 0; i < nodelist.Count; i++)
        {
            if (nodelist[i].Name.ToLower() != "issueinfo")
            {
                string name = nodelist[i].Name;
                string val = nodelist[i].InnerText;
                nvcheader.Add(name, val);
            }
        }

        for (int i = 0; i < nodelist.Count; i++)
        {
            if (nodelist[i].Name.ToLower() == "issueinfo")
            {
                XmlNodeList issueinfolist = nodelist[i].ChildNodes;
                NameValueCollection infonvc = new NameValueCollection();
                for (int j = 0; j < issueinfolist.Count; j++)
                {
                    string infoname = issueinfolist[j].Name;
                    string infoval = issueinfolist[j].InnerText;
                    infonvc.Add(infoname, infoval);
                }
                infonvc.Add(nvcheader);
                nvcissueinfo.Add(infonvc);
            }

        }

        Inventory inv = new Inventory(userid, nvcheader["itemnum"], nvcheader["storeroom"]);

        /*
        string wonum = nvcheader["wonum"];
        string reqnum = nvcheader["reqnum"];
        double totalquantity = Convert.ToDouble(nvcheader["totalquantity"]);
        double maxquantity = 0;
        bool invok = true;
        if (reqnum != "")
            invok = inv.CheckReserve(reqnum, "request", nvcheader["itemnum"], nvcheader["storeroom"], totalquantity, out maxquantity);
        else if (wonum != "")
            invok = inv.CheckReserve(wonum, "workorder", nvcheader["itemnum"], nvcheader["storeroom"], totalquantity, out maxquantity);
        else
            invok = inv.CheckReserve(null, "other", nvcheader["itemnum"], nvcheader["storeroom"], totalquantity, out maxquantity);

        if (!invok)
            return "Maximum allowed issue quantity is " + maxquantity.ToString();
      */
          bool issueok = inv.IssueInventory(nvcissueinfo);
          if (!issueok)
            return inv.ErrorMessage;
          else
            return "OK";
    }

    [OperationContract]
    [WebGet]
    public string IssueReserved(string xml)
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

      List<NameValueCollection> nvcissueinfo = new List<NameValueCollection>();

      for (int i = 0; i < nodelist.Count; i++)
      {
          XmlNodeList issueinfolist = nodelist[i].ChildNodes;
          NameValueCollection infonvc = new NameValueCollection();
          for (int j = 0; j < issueinfolist.Count; j++)
          {
            string infoname = issueinfolist[j].Name;
            string infoval = issueinfolist[j].InnerText;
            infonvc.Add(infoname, infoval);
          }
          infonvc.Add(nvcheader);
          nvcissueinfo.Add(infonvc);
      }

      Inventory inv = new Inventory(userid);
      
      bool issueok = inv.IssueReserved(nvcissueinfo);
      if (!issueok)
        return inv.ErrorMessage;
      else
        return "OK";
    }

    [OperationContract]
    [WebGet]
    public string StagingIssue(string xml)
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

      XmlNodeList nodelist = doc.DocumentElement.ChildNodes;

      List<NameValueCollection> nvcissueinfo = new List<NameValueCollection>();

      for (int i = 0; i < nodelist.Count; i++)
      {
        XmlNodeList issueinfolist = nodelist[i].ChildNodes;
        NameValueCollection infonvc = new NameValueCollection();
        for (int j = 0; j < issueinfolist.Count; j++)
        {
          string infoname = issueinfolist[j].Name;
          string infoval = issueinfolist[j].InnerText;
          infonvc.Add(infoname, infoval);
        }
        infonvc.Add(nvcheader);
        nvcissueinfo.Add(infonvc);
      }

      Inventory inv = new Inventory(userid);

      bool issueok = inv.BatchStagingIssue(nvcissueinfo);
      if (!issueok)
        return inv.ErrorMessage;
      else
        return "OK";
    }

    [OperationContract]
    [WebGet]
    public string InvAutoReturn(string xml,string returndate)
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

      List<NameValueCollection> nvcreturninfo = new List<NameValueCollection>();

      for (int i = 0; i < nodelist.Count; i++)
      {
        XmlNodeList returnlist = nodelist[i].ChildNodes;
        NameValueCollection infonvc = new NameValueCollection();
        for (int j = 0; j < returnlist.Count; j++)
        {
          string infoname = returnlist[j].Name;
          string infoval = returnlist[j].InnerText;
          infonvc.Add(infoname, infoval);
        }
        nvcreturninfo.Add(infonvc);
      }

      Inventory inv = new Inventory(userid);

      bool returnok = inv.AutoReturn(nvcreturninfo,returndate);
      if (!returnok)
        return inv.ErrorMessage;
      else
        return "OK";
    }

    [OperationContract]
    [WebGet]
    public string InvReturnDetail(string xml, string batchnum, string returndate)
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

      List<NameValueCollection> nvcreturninfo = new List<NameValueCollection>();

      for (int i = 0; i < nodelist.Count; i++)
      {
        XmlNodeList returnlist = nodelist[i].ChildNodes;
        NameValueCollection infonvc = new NameValueCollection();
        for (int j = 0; j < returnlist.Count; j++)
        {
          string infoname = returnlist[j].Name;
          string infoval = returnlist[j].InnerText;
          infonvc.Add(infoname, infoval);
        }
        nvcreturninfo.Add(infonvc);
      }

      Inventory inv = new Inventory(userid);
      bool returnok = inv.ReturnInventory(batchnum,nvcreturninfo,returndate);
      if (!returnok)
        return inv.ErrorMessage;
      else
        return "OK";
    }



    [OperationContract]
    [WebGet]
    public string UpdateIssue(string xml, string batchnum)
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

      List<NameValueCollection> nvcissueinfo = new List<NameValueCollection>();
      for (int i = 0; i < nodelist.Count; i++)
      {
        if (nodelist[i].Name.ToLower() != "issueinfo")
        {
          string name = nodelist[i].Name;
          string val = nodelist[i].InnerText;
          nvcheader.Add(name, val);
        }
      }

      for (int i = 0; i < nodelist.Count; i++)
      {
        if (nodelist[i].Name.ToLower() == "issueinfo")
        {
          XmlNodeList issueinfolist = nodelist[i].ChildNodes;
          NameValueCollection infonvc = new NameValueCollection();
          for (int j = 0; j < issueinfolist.Count; j++)
          {
            string infoname = issueinfolist[j].Name;
            string infoval = issueinfolist[j].InnerText;
            infonvc.Add(infoname, infoval);
          }
          infonvc.Add(nvcheader);
          nvcissueinfo.Add(infonvc);
        }

      }

      Inventory inv = new Inventory(userid, nvcheader["itemnum"], nvcheader["storeroom"]);
      string wonum = nvcheader["wonum"];
      string reqnum = nvcheader["reqnum"];
      double totalquantity = Convert.ToDouble(nvcheader["totalquantity"]);
      double maxquantity = 0;
      bool invok = true;
      string linecounter = "0";
      if (reqnum != "")
        invok = inv.CheckReserve(reqnum, linecounter, "R", nvcheader["itemnum"], nvcheader["storeroom"], totalquantity, out maxquantity);
      else if (wonum != "")
        invok = inv.CheckReserve(wonum, linecounter, "W", nvcheader["itemnum"], nvcheader["storeroom"], totalquantity, out maxquantity);
      else
      {
        //invok = inv.CheckReserve(null, linecounter, "other", nvcheader["itemnum"], nvcheader["storeroom"], totalquantity, out maxquantity);
      }

      if (!invok)
        return "Maximum allowed issue quantity is " + maxquantity.ToString();

      bool issueok = inv.UpdateInventoryIssue(nvcissueinfo, batchnum);
      if (!issueok)
        return inv.ErrorMessage;
      else
        return "OK";
    }

    [OperationContract]
    [WebGet]
    public string CreateNonInventoryIssue(string xml)
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

        NameValueCollection nvcissueinfo = new NameValueCollection();
        for (int i = 0; i < nodelist.Count; i++)
        {
            string name = nodelist[i].Name;
            string val = nodelist[i].InnerText;
            nvcissueinfo.Add(name, val);
        }

        Inventory inv = new Inventory(userid);
        bool issueok = inv.IssueNonInventory(nvcissueinfo);
        if (!issueok)
            return inv.ErrorMessage;
        else
            return "OK";
    }

    [OperationContract]
    [WebGet]
    public string IssueService(string xml)
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

      NameValueCollection nvcissueinfo = new NameValueCollection();
      for (int i = 0; i < nodelist.Count; i++)
      {
        string name = nodelist[i].Name;
        string val = nodelist[i].InnerText;
        nvcissueinfo.Add(name, val);
      }

      Inventory inv = new Inventory(userid);
      bool issueok = inv.IssueService(nvcissueinfo);
      if (!issueok)
        return inv.ErrorMessage;
      else
        return "OK";
    }

    [OperationContract]
    [WebGet]
    public string UpdateIssueService(string xml, string batchnum)
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

      NameValueCollection nvcissueinfo = new NameValueCollection();
      for (int i = 0; i < nodelist.Count; i++)
      {
        string name = nodelist[i].Name;
        string val = nodelist[i].InnerText;
        nvcissueinfo.Add(name, val);
      }

      Inventory inv = new Inventory(userid);

      bool issueok = inv.UpdateServiceIssue(nvcissueinfo, batchnum);
      if (!issueok)
        return inv.ErrorMessage;
      else
        return "OK";
    }

    [OperationContract]
    [WebGet]
    public string UpdateNonInventoryIssue(string xml, string batchnum)
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

        NameValueCollection nvcissueinfo = new NameValueCollection();
        for (int i = 0; i < nodelist.Count; i++)
        {
            string name = nodelist[i].Name;
            string val = nodelist[i].InnerText;
            nvcissueinfo.Add(name, val);
        }

        Inventory inv = new Inventory(userid);

        bool issueok = inv.UpdateNonInventoryIssue(nvcissueinfo, batchnum);
        if (!issueok)
            return inv.ErrorMessage;
        else
            return "OK";
    }

    [OperationContract]
    [WebGet]
    public string DeleteNonInventoryIssue(string batchnum)
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

        Inventory inv = new Inventory(userid);

        bool issueok = inv.DeleteNonInventoryIssue(batchnum);
        if (!issueok)
            return inv.ErrorMessage;
        else
            return "OK";
    }

    [OperationContract]
    [WebGet]
    public string DeleteServiceIssue(string batchnum)
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

      Inventory inv = new Inventory(userid);

      bool issueok = inv.DeleteServiceIssue(batchnum);
      if (!issueok)
        return inv.ErrorMessage;
      else
        return "OK";
    }

    [OperationContract]
    [WebGet]
    public string ReceivePOLine(string counter, string xml)
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

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);

        XmlNodeList nodelist = doc.DocumentElement.ChildNodes;
        NameValueCollection nvc = new NameValueCollection();
        List<NameValueCollection> eqplist = new List<NameValueCollection>();
        for (int i = 0; i < nodelist.Count; i++)
        {
            if (nodelist[i].Name == "equiplist")
            {
                XmlNode node = nodelist[i];
                for (int j = 0; j < node.ChildNodes.Count; j++)
                {
                    NameValueCollection nvceqp = new NameValueCollection();
                    XmlNode eqpnode = node.ChildNodes[j];
                    for (int k = 0; k < eqpnode.ChildNodes.Count; k++)
                    {
                        nvceqp.Add(eqpnode.ChildNodes[k].Name, eqpnode.ChildNodes[k].InnerText);
                    }
                    eqplist.Add(nvceqp);
                }
            }
            else
                nvc.Add(nodelist[i].Name, nodelist[i].InnerText);
        }

        Inventory objinv = new Inventory(userid);
        if (!objinv.ReceivePOLine(counter, nvc, eqplist))
        {
          return objinv.ErrorMessage;
        }
        else
        {
          string substr = "";
          foreach (NameValueCollection temp in eqplist)
          {
            foreach (string key in temp.Keys)
            {
              if (key.ToLower() == "serialnumid")
              {
                substr = substr + "'" + temp[key] + "',";
              }
            }
          }

          result = "T::";

          if (substr.Length > 0) {
            OleDbConnection conn = new OleDbConnection();
            conn.ConnectionString = HttpContext.Current.Application["ConnString"].ToString();
            conn.Open();

            OleDbCommand cmd = new OleDbCommand();
            string sSQL =  "SELECT DISTINCT tb.EquipSerial, tb.SerialNum FROM Transbatch tb INNER JOIN InventoryTrans it ON tb.batchnum = it.batchnum ";
            sSQL = sSQL + "WHERE it.PoNum = (SELECT PoNum FROM PoLine WHERE Counter = " + counter + ") AND tb.EquipSerial is not NULL AND ";
            sSQL = sSQL + "tb.SerialNum is not null AND tb.SerialNum in (" + substr.Substring(0, substr.Length - 1) + ")";
            cmd.CommandText = sSQL;
            cmd.Connection = conn;
            OleDbDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
              result = result + dr[0].ToString() + "," + dr[1].ToString() + ";";
            }
            dr.Close();
            conn.Close();
          }

          return result;
        }
    }

    [OperationContract]
    [WebGet]
    public string ReturnVendor(string receiptbatch, string transcounter, string lotcounter, string returnqty)
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

      Inventory objinv = new Inventory(userid);
      if (!objinv.ReturnVendor(receiptbatch, transcounter, lotcounter, Convert.ToDecimal(returnqty)))
      {
        return objinv.ErrorMessage;
      }

      return "OK";
    }

    [OperationContract]
    [WebGet]
    public string BatchReceive(string polinecounterlist,string doctype, string docnum)
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

        NameValueCollection nvc = new NameValueCollection();
        List<NameValueCollection> eqplist = new List<NameValueCollection>();
        Inventory objinv = new Inventory(userid);

        if (!objinv.BatchReceive(polinecounterlist, doctype, docnum))
        {
                return objinv.ErrorMessage;
        }

        return "OK";
    }

    [OperationContract]
    [WebGet]
    ///This function accept the itemnum, storeroom and quantity
    ///returns the lot infomation from inventory
    ///the return value will be in a 2d array like string
    ///in a format of: OK^counter^stocklevel^oldprice^oldquantity^quantity^price^tax1^tax2^addcost^totalcost,counter^stocklevel^oldprice^oldquantity^quantity^price^tax1^tax2^addcost^totalcost ..
    ///In case of non inventory item, counter would be empty, stocklevel would be 0, price would be 0, 1 record would be returned
    ///In case of non stock item , price would be set to item's default price, 1 record would be returned
    ///this array would be used for user to see the detail later and calculate the total
    ///in case of error, the OK is not added and the error message would be returned.
    public string GetInvLots(string itemnum, string storeroom, string quantity, string batchnum)
    {
        string result = "";

        string userid = "";
        int lotcount = 0;
        int inventoryflag = 0;

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

        bool isinventory = false;
        string issueprice = "";
        Items objitem = new Items(userid, "Items", "ItemNum", itemnum);
        if (objitem.ModuleData["itemnum"] != "")
        {
            isinventory = true;
            inventoryflag = 1;
            issueprice = objitem.ModuleData["IssuePrice"];
        }
        DataTable dt = new DataTable();
        if (isinventory)
        {
            Inventory objinv = new Inventory(userid, itemnum, storeroom);
            dt = objinv.GetInvIssue(itemnum, storeroom, batchnum);
            if (dt.Rows.Count != 0)
            {
                //stocked = true;
            }
        }
        double price = 0;
        double tax1rate = 0;
        double tax2rate = 0;
        double addrate = (double)context.Application["MatCost"];
        //result = "OK^";
        if (dt.Rows.Count > 0)
        {

            lotcount = dt.Rows.Count;
            result = "OK^" + inventoryflag.ToString() + "^" + lotcount.ToString();
            double totalqty = Convert.ToDouble(quantity);
            double remainqty = totalqty;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                double tax1 = 0;
                double tax2 = 0;
                double addcost = 0;
                double totalcost = 0;
                double qty = 0;
                double stocklevel = Convert.ToDouble(row["stocklevel"]);
                ////OK^counter^stocklevel^oldprice^oldquantity^quantity^price^tax1^tax2^addcost^totalcost
                if (batchnum != "")
                {
                    if (i == 0)
                        result = result + ":" + row["counter"].ToString() + "^" + row["stocklevel"].ToString() + "^" + row["price"].ToString() + "^" + row["quantity"].ToString() + "^" + row["quantity"].ToString() + "^"
                               + row["price"].ToString() + "^" + row["tax1"].ToString() + "^" + row["tax2"].ToString() + "^" + row["addcost"].ToString() + "^" + row["totalcost"].ToString();
                    else
                        result = result + "," + row["counter"].ToString() + "^" + row["stocklevel"].ToString() + "^" + row["price"].ToString() + "^" + row["quantity"].ToString() + "^" + row["quantity"].ToString() + "^"
                               + row["price"].ToString() + "^" + row["tax1"].ToString() + "^" + row["tax2"].ToString() + "^" + row["addcost"].ToString() + "^" + row["totalcost"].ToString();
                }
                else
                {
                    if (issueprice == "AVGPRICE")
                        price = Convert.ToDouble(row["Avgprice"]);
                    else if (issueprice == "LASTPRICE")
                        price = Convert.ToDouble(row["LastPrice"]);
                    else if (issueprice == "QUOTEDPRICE")
                        price = Convert.ToDouble(row["QUOTEDPRICE"]);
                    else if (issueprice == "FIXPRICE")
                        price = Convert.ToDouble(row["FIXPRICE"]);
                    else if (issueprice == "LOTPRICE")
                        price = Convert.ToDouble(row["COST"]);

                    tax1 = Math.Round(price * qty * tax1rate, 2);
                    tax2 = Math.Round(price * qty * tax2rate, 2);
                    totalcost = Math.Round(price * qty + tax1 + tax2, 2);
                    addcost = Math.Round(price * addrate, 2);
                    if (remainqty - stocklevel > 0.00001)
                    {
                        qty = stocklevel;
                        remainqty = remainqty - qty;
                    }
                    else
                    {
                        qty = remainqty;
                        remainqty = 0;
                    }

                    tax1 = Math.Round(price * qty * tax1rate, 2);
                    tax2 = Math.Round(price * qty * tax2rate, 2);
                    totalcost = Math.Round(price * qty + tax1 + tax2, 2);

                    if (i == 0)
                        result = result + ":" + row["counter"].ToString() + "^" + row["stocklevel"].ToString() + "^" + row["price"].ToString() + "^0^" + qty.ToString() + "^"
                               + price.ToString() + "^" + tax1.ToString() + "^" + tax2.ToString() + "^" + addcost.ToString() + "^" + totalcost.ToString();
                    else
                        result = result + "," + row["counter"].ToString() + "^" + row["stocklevel"].ToString() + "^" + row["price"].ToString() + "^0^" + qty.ToString() + "^"
                               + price.ToString() + "^" + tax1.ToString() + "^" + tax2.ToString() + "^" + addcost.ToString() + "^" + totalcost.ToString();
                }
            }

        }
        else
        {
            //to be finished for non-inventory items

            ////OK^counter^stocklevel^oldprice^oldquantity^quantity^price^tax1^tax2^addcost^totalcost

            //if (isinventory)
            //{
            //  if (issueprice == "FIXPRICE")
            //    price = objitem.ModuleData["fixprice"];

            //}

            //double totalcost = 0;
            //double tax1 = 0, tax2 = 0;
            //tax1 = Math.Round(Convert.ToDouble(price) * Convert.ToDouble(quantity) * tax1rate, 2);
            //tax2 = Math.Round(Convert.ToDouble(price) * Convert.ToDouble(quantity) * tax2rate, 2);
            //totalcost = Math.Round(Convert.ToDouble(price) * Convert.ToDouble(quantity) + tax1 + tax2,2);
            //result = "OK^" + "" + "^0" + "^" + price + "^0^" + quantity + "^" + price + "^" + tax1.ToString() + "^" + tax2.ToString() + "^0" + "^" + totalcost.ToString();
        }

        return result;


    }

    [OperationContract]
    [WebGet]
    public string DuplicateItem(string userid, string xmlnvc, string olditemnum)
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

        Items objItem;

        objItem = new Items(userid, "Items", "ItemNum");
        success = objItem.Duplicate(nvc, olditemnum);

        if (success)
        {
            returnStr = objItem.ModuleData["itemnum"];
        }

        return returnStr;

    }

    [OperationContract]
    [WebGet]
    public string UpdateItem(string xmlnvc)
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

        Items objItem;

        string itemnum = nvc["itemnum"];
        objItem = new Items(o.ToString(), "Items", "ItemNum", itemnum);
        success = objItem.Update(nvc);
        if (success)
        {
            returnStr = "TRUE^" + nvc["itemnum"];
        }
        else
        {
            string err = "FALSE^" + objItem.ErrorMessage.Replace("\r\n", "");
            returnStr = err;
        }

        return returnStr;
    }

    [OperationContract]
    [WebGet]
    public string GetDataFromItems(string itemnum)
    {
        string retStr = "";

        var query = (from i in dc.ItemLinqs where i.ItemNum == itemnum select i).FirstOrDefault();
        
        retStr = query.ItemNum + "^" + query.ItemDesc + "^" + query.IssueUnit;
        return retStr;
    }

    /// <summary>
    /// Create or Update InvMain record.
    /// </summary>
    /// <param name="counter"></param>
    /// <param name="xml"></param>
    /// <returns></returns>
    [OperationContract]
    [WebGet]
    public string SaveInventoryItem(string counter, string xml)
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

        InvMain obj;
        string newcounter = "";
        if (counter == "")
        {
            obj = new InvMain(userid, "InvMain", "Counter");
            success = obj.Create(nvc);
            newcounter = obj.ModuleData["Counter"];
        }
        else
        {
            obj = new InvMain(userid, "InvMain", "Counter", counter);
            success = obj.Update(nvc);
        }

        if (success)
        {
            returnStr = "OK";
            if (newcounter != "")
              returnStr = returnStr + newcounter;
        }
        else
        {
            returnStr = obj.ErrorMessage.Replace("\r\n", " ");
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
    public ResultItemInvLinq PartInvList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultItemInvLinq result = new ResultItemInvLinq();

        string isinventory = "",equipment = "", wherestring = "";
        isinventory = HttpContext.Current.Request.QueryString.Get("isinventory");
        equipment = HttpContext.Current.Request.QueryString.Get("equipment");
        wherestring = HttpContext.Current.Request.QueryString.Get("wherestring");

        var tempquery = from v in dc.v_InventoryStorerooms select v;

        if (isinventory == "0")
        {
            wherestring = null;
            tempquery = from i in dc.ItemLinqs
                    select new v_InventoryStoreroom
                    {
                        ItemNum = i.ItemNum,
                        ItemDesc = i.ItemDesc,
                        IssueUnit = i.IssueUnit,
                        IssueMethod = i.IssueMethod,
                        IssuePrice = i.IssuePrice,
                        FixPrice = i.FixPrice,
                        Inactive = i.Inactive,
                        AvgPrice = 0,
                        LastPrice = 0,
                        QuotedPrice = 0,
                        QtyOnHand = 0
                    };
        }
        var query = from tq in tempquery
                    join pl in dc.EquipCompLinqs on tq.ItemNum equals pl.ItemNum
                    where pl.Equipment.ToLower() == equipment.ToLower()
                    select tq;
        GridLinqBindingData<v_InventoryStoreroom> resultData = RadGrid.GetBindingData<v_InventoryStoreroom>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Count = resultData.Count;
        result.Data = resultData.Data.ToList<v_InventoryStoreroom>();


        return result;
    }

    [OperationContract]
    public ResultItemInvLinq ItemInvList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultItemInvLinq result = new ResultItemInvLinq();

        string isinventory = "", wherestring = "";
        isinventory = HttpContext.Current.Request.QueryString.Get("isinventory");
        wherestring = HttpContext.Current.Request.QueryString.Get("wherestring");

        var query = from v in dc.v_InventoryStorerooms select v;

        if (isinventory == "0")
        {
            wherestring = null;
            query = from i in dc.ItemLinqs
                    select new v_InventoryStoreroom
                    {
                        ItemNum = i.ItemNum,
                        ItemDesc = i.ItemDesc,
                        IssueUnit = i.IssueUnit,
                        IssueMethod = i.IssueMethod,
                        IssuePrice = i.IssuePrice,
                        FixPrice = i.FixPrice,
                        Inactive = i.Inactive,
                        AvgPrice = 0,
                        LastPrice = 0,
                        QuotedPrice = 0,
                        QtyOnHand = 0
                    };
        }

        if (!string.IsNullOrEmpty(wherestring))
        {
            wherestring = wherestring.Replace("'", "\"");
            if (string.IsNullOrEmpty(filterExpression))
            { filterExpression = wherestring; }
            else
            {
                filterExpression = filterExpression + " and " + wherestring;
            }
        }

        GridLinqBindingData<v_InventoryStoreroom> resultData = RadGrid.GetBindingData<v_InventoryStoreroom>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Count = resultData.Count;
        result.Data = resultData.Data.ToList<v_InventoryStoreroom>();

        return result;
    }

    public class ResultItemRecentlist
    {
        public int Count { get; set; }
        public List<v_ItemRecentlist> Data { get; set; }
    }

    public class ResultItemInvLinq
    {
        public int Count { get; set; }
        public List<v_InventoryStoreroom> Data { get; set; }
    }

    [OperationContract]
    public ResultStoreRoomLinq LookupStoreRoomDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultStoreRoomLinq result = new ResultStoreRoomLinq();

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
        var query = from sr in dc.StoreRoomLinqs
                    select sr;
        GridLinqBindingData<StoreRoomLinq> resultData = RadGrid.GetBindingData<StoreRoomLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = resultData.Data.ToList<StoreRoomLinq>();
        result.Count = resultData.Count;


        return result;
    }

    [OperationContract]
    [WebGet]
    public ResultStoreRoomLinq QueryResultStoreRoom(int startRowIndex, int maximumRows, string sortExpression, string filterExpression, string jsonData)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultStoreRoomLinq result = new ResultStoreRoomLinq();

        string lookupWhereStr = "";
        string login_user = "";
        lookupWhereStr = HttpContext.Current.Request.QueryString.Get("where") ?? "";

        jsonData = jsonData == null ? "{}" : jsonData;
        if (jsonData != "{}")
        {
            NameValueCollection nvc = AzzierData.DeserializeJSON(jsonData);
            object empid = HttpContext.Current.Session["LogIn"];
            string userid = (empid != null) ? empid.ToString() : "";
            ModuleoObject obj = new ModuleoObject(userid, "Storeroom", "StoreRoom");
            string wherestring = "", linqwherestring = "";
            if (jsonData.Contains("queryID"))
                nvc = UserQuery.CreateNameValueCollection(nvc["queryID"]);
            obj.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
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
        var query = from sr in dc.StoreRoomLinqs
                    select sr;
        GridLinqBindingData<StoreRoomLinq> resultData = RadGrid.GetBindingData<StoreRoomLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = resultData.Data.ToList<StoreRoomLinq>();
        result.Count = resultData.Count;


        return result;
    }
    [OperationContract]
    public ResultStoreRoomRecentList StoreRoomRecentListDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultStoreRoomRecentList result = new ResultStoreRoomRecentList();

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

        var query = from v in dc.v_StoreRoomRecentLists where v.UserId.ToLower() == login_user.ToLower() orderby v.AccessDate descending select v;

        GridLinqBindingData<v_StoreRoomRecentList> resultData = RadGrid.GetBindingData<v_StoreRoomRecentList>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        result.Data = resultData.Data.ToList<v_StoreRoomRecentList>();
        result.Count = resultData.Count;

        return result;
    }

    [OperationContract]
    [WebGet]
    public string DelStoreRoomWorkList(string paramXML)
    {
        string resstr = "Success";
        
  

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
            resstr = "ERROR.";
        }

        return resstr;
    }

    [OperationContract]
    [WebGet]
    public string DelWorkList(string paramXML)
    {
      string resstr = "Success";



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
        resstr = "ERROR.";
      }

      return resstr;
    }

    [OperationContract]
    [WebGet]
    public string AddStoreRoomWorkList(string paramXML)
    {
        string resstr = "Success";

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
                               where c.LinkModule.ToLower() == "storeroom" && c.LinkType.ToLower() == "worklist" && c.UserId.ToLower() == login_user.ToLower()
                               select c.LinkId).Contains(id.Value)
                       select id.Value;

        var ins = from sr in dc.StoreRoomLinqs
                  where idresult.ToList().Contains(sr.StoreRoom)
                  select new
                  {
                      LinkId = sr.StoreRoom,
                      UserId = login_user,
                      LinkModule = "storeroom",
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
            resstr = e.Message;
        }

        return resstr;
    }

    [OperationContract]
    public ResultStoreRoomLinq StoreRoomWorkListDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        ResultStoreRoomLinq result = new ResultStoreRoomLinq();
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
                    join sr in dc.StoreRoomLinqs on u.LinkId equals sr.StoreRoom
                    where u.LinkModule.ToLower() == "storeroom" && u.LinkType == "worklist" && u.UserId.ToLower() == login_user.ToLower()
                    orderby sr.StoreRoom
                    select sr;

        GridLinqBindingData<StoreRoomLinq> data = RadGrid.GetBindingData<StoreRoomLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = data.Data.ToList<StoreRoomLinq>();
        result.Count = data.Count; 

        return result;
    }

    [OperationContract]
    [WebGet]
    public string DeleteStoreRoom(string storeroom, string check = "1")
    {
        string resstr = "Success";

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

        StoreRoom objectstoreroom = new StoreRoom(context.Session["Login"].ToString(), "StoreRoom", "StoreRoom", storeroom);

        if (objectstoreroom.ModuleData["storeroom"] != null)
        {
          int result = objectstoreroom.Delete(check);
          return result.ToString() + "::" + objectstoreroom.ErrorMessage;
        }
        else
        {
          return "-1::Storeroom does not exist.";
        }
    }

    [OperationContract]
    [WebGet]
    public string DeleteLot(string counter)
    {
      string resstr = "Success";

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

      ModuleoObject objlot = new ModuleoObject(context.Session["Login"].ToString(), "InvLot", "Counter", counter);
      decimal stocklevel = Convert.ToDecimal(objlot.ModuleData["StockLevel"]);
      if (stocklevel > 0)
      {
        return "Cannot delete lot when quantity is more than 0.";
      }
      bool success = objlot.Delete();
      if (success)
        return "OK";
      else
        return objlot.ErrorMessage;
      
    }

    [OperationContract]
    [WebGet]
    public string SaveStoreRoom(string paramXML)
    {
        string resstr = "Success";

        bool success = false;
        NameValueCollection nvc = new NameValueCollection();

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
        XmlNodeList srlist = root.ChildNodes;

        for (int i = 0; i < srlist.Count; i++)
        {
            string name = srlist[i].Name;
            string val = srlist[i].InnerText;
            nvc.Add(name, val);
        }

        StoreRoom objectstoreroom;

        string storeroom = nvc["storeroom"];

        objectstoreroom = new StoreRoom(context.Session["Login"].ToString(), "StoreRoom", "StoreRoom", storeroom);

        if (objectstoreroom.ModuleData["storeroom"] != "")
        {
            success = objectstoreroom.Update(nvc);
        }
        else
        {
            success = objectstoreroom.Create(nvc);
        }

        if (!success)
        {
          resstr = objectstoreroom.ErrorMessage;
        }
        else
        {
            resstr = "Success";
        }

        UserWorkList list = new UserWorkList();
        list.AddToRecentList(context.Session["Login"].ToString(), "StoreRoom",storeroom);

        return resstr;
    }

    

    [OperationContract]
    [WebGet]
    public string SaveAlternatePart(string counter,string itemnum, string alternateitemnum, string reverse)
    {
      string resstr = "Success";

      bool success = false;
      NameValueCollection nvc = new NameValueCollection();

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

      AlternatePart objalter;
      if (counter == "")
      {
        objalter = new AlternatePart(login_user, "AlternatePart", "Counter");
        success = objalter.Create(itemnum, alternateitemnum, reverse);
      }
      else
      {
        objalter = new AlternatePart(login_user, "AlternatePart", "Counter",counter);
        success = objalter.Update(itemnum, alternateitemnum, reverse);
      }

      if (!success)
      {
        resstr = objalter.ErrorMessage;
      }
      else
      {
        resstr = "OK";
      }

      return resstr;
    }

    [OperationContract]
    [WebGet]
    public string DeleteAlternatePart(string counter, string reverse)
    {
      string resstr = "Success";

      bool success = false;
      NameValueCollection nvc = new NameValueCollection();

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

      AlternatePart objalter = new AlternatePart(login_user, "AlternatePart", "Counter", counter);
      success = objalter.Delete(counter, reverse);

      if (!success)
      {
        resstr = objalter.ErrorMessage;
      }
      else
      {
        resstr = "OK";
      }
      return resstr;
    }

    [OperationContract]
    [WebGet]
    public string Transfer(string counter, string storeroom, string position, string quantity)
    {
      string retstr = "";

      bool success = false;
      NameValueCollection nvc = new NameValueCollection();

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

      Inventory objinv = new Inventory(login_user);
      success = objinv.Transfer(counter, storeroom, position, Convert.ToDouble(quantity));

      if (!success)
      {
        retstr = objinv.ErrorMessage;
      }
      else
      {
        retstr = "OK";
      }
      return retstr;
    }
      

    public class ResultCompEquipmentLinq
    {
      public int Count { get; set; }
      public List<v_CompEquipmentList> Data { get; set; }

    }

    [OperationContract]
    public ResultItemTypeLinq GetItemTypeList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultItemTypeLinq result = new ResultItemTypeLinq();

        string lookupfilter = HttpContext.Current.Request.QueryString.Get("where");

        if (!string.IsNullOrEmpty(lookupfilter))
        {
            lookupfilter = lookupfilter.Replace("'", "\"");
            if (!string.IsNullOrEmpty(filterExpression))
                filterExpression = filterExpression + " and " + lookupfilter;
            else
                filterExpression = lookupfilter;
        }

        var query = from item in dc.ItemTypeLinqs orderby item.Counter select item;

        GridLinqBindingData<ItemTypeLinq> resultData = RadGrid.GetBindingData<ItemTypeLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = resultData.Data.ToList<ItemTypeLinq>();
        result.Count = resultData.Count;

        return result;
    }

    [OperationContract]
    public ResultItemListLinq GetItemList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
        ResultItemListLinq result = new ResultItemListLinq();

        string lookupfilter = HttpContext.Current.Request.QueryString.Get("where");

        if (!string.IsNullOrEmpty(lookupfilter))
        {
            lookupfilter = lookupfilter.Replace("'", "\"");
            if (!string.IsNullOrEmpty(filterExpression))
                filterExpression = filterExpression + " and " + lookupfilter;
            else
                filterExpression = lookupfilter;
        }

        var query = from item in dc.ItemLinqs orderby item.ItemNum select item;

        GridLinqBindingData<ItemLinq> resultData = RadGrid.GetBindingData<ItemLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = resultData.Data.ToList<ItemLinq>();
        result.Count = resultData.Count;

        return result;
    }

    [OperationContract]
    [WebGet]
    public string RequisitionComplete(string reqnum, string reqlinecounter, string itemnum, string storeroom, string reservecounter)
    {
      string returnStr = "OK";
      Boolean iscomplete = false;

      if (reqlinecounter != "")
      {
        OleDbConnection conn = new OleDbConnection();
        conn.ConnectionString = HttpContext.Current.Application["ConnString"].ToString();
        conn.Open();

        OleDbCommand cmd = new OleDbCommand();
        string sSQL = "Select counter From RequestLine Where ReqNum = '" + reqnum + "' And Status='COMPLETED' AND Counter = " + reqlinecounter;
        cmd.CommandText = sSQL;
        cmd.Connection = conn;
        OleDbDataReader dr = cmd.ExecuteReader();
        while (dr.Read())
        {
          iscomplete = true;
        }
        dr.Close();
        conn.Close();
      }

      if (iscomplete)
      {
        returnStr = "The request item for\nRequisition number: " + reqnum + "\nItem number: " + itemnum + "\nhas already been issued. Please refresh your screen to see current data.";
      }

      returnStr = returnStr + "^" + itemnum + "^" + storeroom + "^" + reservecounter + "^" + reqlinecounter;

      return returnStr;
    }
}

public class ResultAllTransBatchLinq
{
  public int Count { get; set; }
  public List<v_AllTransBatch> Data { get; set; }

}

public class ResultItemsLinq
{
    public int Count { get; set; }
    public List<ItemLinq> Data { get; set; }
}

public class ResultPOLinq
{
    public int Count { get; set; }
    public List<POLinq> Data { get; set; }
}

public class ResultReceivingPOLineLinq
{
    public int Count { get; set; }
    public List<v_ReceivingPOLine> Data { get; set; }
}

public class ResultInventoryTransDetailLinq
{
  public int Count { get; set; }
  public List<v_InventoryTransDetail> Data { get; set; }
}

public class ResultInventoryTransBatchLinq
{
  public int Count { get; set; }
  public List<v_InventoryTransBatch> Data { get; set; }
}

public class ResultStoreRoomLinq
{
    public int Count { get; set; }
    public List<StoreRoomLinq> Data { get; set; }
}


public class ResultStoreRoomRecentList
{
    public int Count { get; set; }
    public List<v_StoreRoomRecentList> Data { get; set; }
}

public class ResultReturnVendorFrom
{
  public int Count { get; set; }
  public List<v_returnvendorfrom> Data { get; set; }
}

public class ResultInventoryPosition
{
  public int Count { get; set; }
  public List<v_InventoryPosition> Data { get; set; }
}

public class ResultActiveInventoryUsage
{
  public int Count { get; set; }
  public List<v_ActiveInventoryUsage> Data { get; set; }
}

public class ResultItemTypeLinq
{
    public Int32 Count { get; set; }
    public List<ItemTypeLinq> Data { get; set; }
}

public class ResultItemListLinq
{
    public Int32 Count { get; set; }
    public List<ItemLinq> Data { get; set; }
}
