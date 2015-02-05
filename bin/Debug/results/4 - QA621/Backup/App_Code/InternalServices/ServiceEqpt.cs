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
public class ServiceEqpt
{
    protected DataClassesDataContext dc = new DataClassesDataContext(AzzierData.LinqConnectstring);
    protected string lookcriteria, mode = "", login_user = "";
    protected List<string> division = new List<string>();

    // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
    // To create an operation that returns XML,
    //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
    //     and include the following line in the operation body:
    //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
    //[OperationContract]
    //public void DoWork()
    //{
    //    return;
    //}

    // Add more operations here and mark them with [OperationContract]
    // public ResultDataEquipment GetEqptRecentList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)

    [OperationContract]
    [WebGet]
    public string ModifyEquipment(string equipment, string newdivision, string newparentid, string newstatus, string newparentdesc, string moddate, string newoperator, string newlocation, string newlocationdesc,
                                    string updateopen,string updatehistory, string keeprelation, string updatechildlocation)
    {
      
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
      
      Equipment eqp = new Equipment(login_user, "Equipment", "Equipment", equipment);
      if (newparentid != "" && newparentid != eqp.ModuleData["ParentId"])
      {
        if (eqp.IsAncestor(newparentid, equipment))
        {
          return "Invalid new parent equipment. Check equipment hierarchy.";
        }
        if (newparentid.ToLower() == equipment.ToLower())
        {
          return "Cannot set equipment parent as equipment itself";
        }
      }
      eqp.ModifyEquipment(newlocation, newlocationdesc, newparentid, newparentdesc, newdivision, newstatus, newoperator, moddate, ConvertBool(updateopen), ConvertBool(updatehistory), ConvertBool(keeprelation), ConvertBool(updatechildlocation));
      if (eqp.ErrorMessage == "")
        return "OK";
      else
        //litScript1.Text = "<script>alert('" + eqp.ErrorMessage + "');document.location.href='equipmentmain.aspx?mode=edit&equipment=" + m_equipment + "'</script>";
        return eqp.ErrorMessage;
      
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
    public string EqptDelWorkList(string eqptxml)
    {
        string retstr = "SUCCESS.";

        if (System.Web.HttpContext.Current.Session["Login"] != null)
        {
            login_user = HttpContext.Current.Session["Login"].ToString();
            if (login_user == "")
            { return "ERROR."; }
        }
        else { return "ERROR."; }

        XDocument doc = XDocument.Parse(eqptxml);

        var idresult = from id in doc.Descendants("IDS") select id.Value;
        //string conn = System.Configuration.ConfigurationManager.ConnectionStrings["AzzierConnectionString"].ConnectionString;
        try
        {
            using (DataClassesDataContext dc = new DataClassesDataContext(AzzierData.LinqConnectstring))
            {
                var result = from userwl in dc.UserWorkListLinqs
                             where idresult.ToList().Contains(userwl.LinkId) && userwl.LinkModule.ToLower() == "equipment" &&
                             userwl.LinkType == "worklist" && userwl.UserId == login_user
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
    public string EqptAddtoWorkList(string paramXML)
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
                               where c.LinkModule.ToLower() == "equipment" && c.LinkType.ToLower() == "worklist" && c.UserId.ToLower() == login_user.ToLower()
                               select c.LinkId).Contains(id.Value)
                       select id.Value;

        var ins = from id in dc.EquipmentLinqs
                  where idresult.ToList().Contains(id.Equipment)
                  select new
                  {
                      LinkId = id.Equipment,
                      UserId = login_user,
                      LinkModule = "Equipment",
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
    public ResultEquipmentrecentlist RecentListDataAndCount(int startRowIndex,int maximumRows,string sortExpression,string filterExpression)
    {
        ResultEquipmentrecentlist result = new ResultEquipmentrecentlist();
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
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

        var query = from v in dc.v_EquipmentRecentlists where v.UserId.ToLower() == login_user.ToLower()
                    orderby v.AccessDate descending 
                    select v;

        GridLinqBindingData<v_EquipmentRecentlist> resultData = RadGrid.GetBindingData<v_EquipmentRecentlist>(query,startRowIndex,maximumRows,sortExpression,filterExpression);
        result.Data = resultData.Data.ToList<v_EquipmentRecentlist>();
        result.Count = resultData.Count;

        return result;
    }


    [OperationContract]
    public ResultEquipmentLinq GetEqptWorkList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        ResultEquipmentLinq result = new ResultEquipmentLinq();

        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(HttpContext.Current.Session["LCID"]);
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
                    join e in dc.EquipmentLinqs on u.LinkId equals e.Equipment
                    where e.Inactive == 0 && u.LinkModule == "equipment" && u.LinkType == "worklist" && u.UserId.ToLower() == login_user.ToLower()
                    orderby e.Equipment
                    select e;

        GridLinqBindingData<EquipmentLinq> data = RadGrid.GetBindingData<EquipmentLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = data.Data.ToList<EquipmentLinq>();
        result.Count = data.Count;

        return result;
    }

    [OperationContract]
    public ResultEqTypeLinq GetEqTypeList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
        ResultEqTypeLinq result = new ResultEqTypeLinq();
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

        var query = from e in dc.EQTypeLinqs select e;

        GridLinqBindingData<EQTypeLinq> data = RadGrid.GetBindingData<EQTypeLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        //GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "EquipmentLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = data.Data.OfType<EQTypeLinq>().ToList();
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
    public ResultEquipmentLinq SearchDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
        ResultEquipmentLinq result = new ResultEquipmentLinq();
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

        var query = from e in dc.EquipmentLinqs select e;

        GridLinqBindingData<EquipmentLinq> data = RadGrid.GetBindingData<EquipmentLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        //GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "EquipmentLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = data.Data.OfType<EquipmentLinq>().ToList();
        result.Count = data.Count;

        return result;
    }

    [OperationContract]
    [WebGet]
    public ResultEquipmentLinq QueryResultEqpt(int startRowIndex, int maximumRows, string sortExpression, string filterExpression, string jsonData)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
        ResultEquipmentLinq result = new ResultEquipmentLinq();
        string searchcriteria;
        List<string> eqptlist = new List<string>();

        searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";

        NameValueCollection nvc = AzzierData.DeserializeJSON(jsonData);
        object empid = HttpContext.Current.Session["LogIn"];
        string userid = (empid != null) ? empid.ToString() : "";
        Equipment objEqpt = new Equipment(userid, "Equipment", "Equipment");
        string wherestring = "", linqwherestring = "";
        if (jsonData.Contains("queryID"))
          nvc = UserQuery.CreateNameValueCollection(nvc["queryID"]);
        objEqpt.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
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

        var query = from e in dc.EquipmentLinqs select e;

        GridLinqBindingData<EquipmentLinq> data = RadGrid.GetBindingData<EquipmentLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        //GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "EquipmentLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = data.Data.OfType<EquipmentLinq>().ToList();
        result.Count = data.Count;

        return result;
    }

    [OperationContract]
    public ResultEquipmentLinq SpecSearchDatandCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
        string searchcriteria = "";
        List<string> eqptlist = new List<string>();

        ResultEquipmentLinq result = new ResultEquipmentLinq();

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
            eqptlist = new List<string>(searchcriteria.Split(','));
            if (eqptlist.Count() > 0)
            {
                var query = from ca in dc.EquipmentLinqs
                            where ca.Inactive == 0 && eqptlist.Contains(ca.Equipment) && (ca.Division == null ||
                            (from o in dc.UserDivisionLinqs where o.Empid == login_user select o.Division).Contains(ca.Division))
                            select ca;

                GridLinqBindingData<EquipmentLinq> resultData = RadGrid.GetBindingData<EquipmentLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
                result.Data = resultData.Data.ToList<EquipmentLinq>();
                //result.Count = resultData.Data.Count();
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
    public ResultEquipmentPartslist GetPartsList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
      System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
      string equipment = "";
      List<string> eqptlist = new List<string>();

      ResultEquipmentPartslist result = new ResultEquipmentPartslist();

      equipment = HttpContext.Current.Request.QueryString.Get("equipment") ?? "";

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

      var query = from c in dc.EquipCompLinqs select c;

      if (equipment != "")
      {
          query = from c in dc.EquipCompLinqs
                  where c.Equipment.ToLower() == equipment.ToLower()
                  select c;

      }
      GridLinqBindingData<EquipCompLinq> resultData = RadGrid.GetBindingData<EquipCompLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
      result.Data = resultData.Data.ToList<EquipCompLinq>();
      //result.Count = resultData.Data.Count();
      result.Count = resultData.Count;
      return result;
    }

    [OperationContract]
    public ResultEquipmentLinq LookupDataAndCount_Mobile(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
        string lookupcriteria = HttpContext.Current.Request.QueryString.Get("wherestr") ?? "";
        string mode = HttpContext.Current.Request.QueryString.Get("mode") ?? "";
        string mobileequipment = HttpContext.Current.Request.QueryString.Get("mobileequipment") ?? ""; 
        string location = HttpContext.Current.Request.QueryString.Get("location") ?? "";
        System.Web.HttpContext.Current.Session.LCID = 2057;
        ResultEquipmentLinq result = new ResultEquipmentLinq();
        //if (location == "1" && mobileequipment == "1")
        //{
        //    result.Data = null;
        //    result.Count = 0;
        //    return result;
        //}

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

        if (lookupcriteria.Length > 1)
        {
            lookupcriteria = lookupcriteria.Replace("'", "\"");

            if (string.IsNullOrEmpty(filterExpression))
            {
                filterExpression = "";
            }

            filterExpression = filterExpression != "" ? filterExpression + " and " : "";

            if (location == "1" && mobileequipment == "1")
            {
                filterExpression = filterExpression + "(" + lookupcriteria + " or MobileEquipment=1)";
            }
            else
            {
                filterExpression = filterExpression + lookupcriteria;
            }
        }


        var query = from e in dc.EquipmentLinqs
                    //where division.Contains(e.Division) || e.Division.Equals(null)
                    select e;

        if (mode != "query")
        {
            query = query.Where(a => a.Inactive == 0);
        }

        GridLinqBindingData<EquipmentLinq> data = RadGrid.GetBindingData<EquipmentLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = data.Data.OfType<EquipmentLinq>().ToList();
        result.Count = data.Count;

        return result;
    }

    [OperationContract]
    [WebGet]
    public string SavePartList(string xml, string counter)
    {
      string login_user = "";
      if (System.Web.HttpContext.Current.Session["Login"] != null)
      {
        login_user = HttpContext.Current.Session["Login"].ToString();
        if (login_user == "")
          login_user = "";
      }
      else
        login_user = "";

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
      if (nvc["Counter"]!=null)
        nvc.Remove("Counter");
      ModuleoObject obj;
      if (counter == "")  //new
      {
        obj = new ModuleoObject(login_user, "EquipComp", "Counter");
        success = obj.Create(nvc);
      }
      else
      {
        obj = new ModuleoObject(login_user, "EquipComp", "Counter",counter);
        success = obj.Update(nvc);
      }

      if (success)
        return "OK";
      else
        return obj.ErrorMessage;
    }

    [OperationContract]
    [WebGet]
    public string DeletePartList(string counter)
    {
      string login_user = "";
      if (System.Web.HttpContext.Current.Session["Login"] != null)
      {
        login_user = HttpContext.Current.Session["Login"].ToString();
        if (login_user == "")
          login_user = "";
      }
      else
        login_user = "";

      bool success = false;
      ModuleoObject obj = new ModuleoObject(login_user, "EquipComp", "Counter", counter);
      success = obj.Delete();

      if (success)
        return "OK";
      else
        return obj.ErrorMessage;
    }

    [OperationContract]
    [WebGet]
    public string ResetDownTime(string equipment) 
    {
        string resultstr = "OK";
        var eqpt = dc.EquipmentLinqs.Where(x => x.Equipment.ToLower() == equipment.ToLower()).FirstOrDefault();
        if (eqpt != null)
        {
            eqpt.DownTime = 0;
        }
        dc.SubmitChanges();
        return resultstr;
    }


    [OperationContract]
    [WebGet]
    public string CreateEquipment(string userid, string xmlnvc)
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

      Equipment objeqp;

      objeqp = new Equipment(userid, "Equipment", "Equipment");
      success = objeqp.Create(nvc);

      if (success)
      {
        returnStr = nvc["Equipment"];
      }

      return returnStr;
    }

    [OperationContract]
    [WebGet]
    public string SaveEquipment(string xmlnvc)
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

      Equipment objeqp;

      string equipment = nvc["Equipment"];
      objeqp = new Equipment(o.ToString(), "Equipment", "Equipment", equipment);
      success = objeqp.Update(nvc);
      if (success)
      {

        returnStr = "TRUE^" + nvc["equipment"];
      }
      else
      {
        string err = "FALSE^" + objeqp.ErrorMessage.Replace("\r\n", "");
        returnStr = err;
      }

      return returnStr;
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
    public ResultManufacturerLinq GetManufacturerList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
        ResultManufacturerLinq result = new ResultManufacturerLinq();
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

        var query = from m in dc.ManufacturerLinqs select m;

        GridLinqBindingData<ManufacturerLinq> data = RadGrid.GetBindingData<ManufacturerLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        //GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "EquipmentLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = data.Data.OfType<ManufacturerLinq>().ToList();
        result.Count = data.Count;

        return result;
    }

    


    [OperationContract]
    public string UpdateSpecification(string param)
    {
        return "";
    }

    public class ResultEquipmentrecentlist
    {
        public Int32 Count { get; set; }
        public List<v_EquipmentRecentlist> Data { get; set; }
    }

    public class ResultEquipmentPartslist
    {
      public Int32 Count { get; set; }
      public List<EquipCompLinq> Data { get; set; }
    }
}

public class ResultEquipmentLinq
{
    public Int32 Count { get; set; }
    public List<EquipmentLinq> Data { get; set; }
}

public class ResultEqTypeLinq
{
    public Int32 Count { get; set; }
    public List<EQTypeLinq> Data { get; set; }
}

public class ResultManufacturerLinq
{
    public Int32 Count { get; set; }
    public List<ManufacturerLinq> Data { get; set; }
}
