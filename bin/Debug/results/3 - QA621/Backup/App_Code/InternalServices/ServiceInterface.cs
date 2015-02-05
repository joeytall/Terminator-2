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
using System.Data.OleDb;

[ServiceContract(Namespace = "")]
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class ServiceInterface
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
	public void DoWork()
	{
		// Add your operation implementation here
		return;
	}
  

    [OperationContract]
    [WebGet]
    public string CreateInterface(string xml)
    {
      bool success = false;
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

      Interface objinf = new Interface(userid, "InfMaster", "InterfaceName");
      success = objinf.Create(nvc);

      if (success)
      {
        returnStr = "TRUE^" + nvc["interfacename"];
      }
      else
        returnStr = "FALSE^" + objinf.ErrorMessage.Replace("\r\n", "");

      return returnStr;

    }

    [OperationContract]
    [WebGet]
    public string DuplicateInterface(string xml)
    {
        bool success = false;
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
        NameValueCollection nvc;
        nvc = new NameValueCollection();

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);

        XmlNode root = doc.DocumentElement.FirstChild;

        XmlNodeList wolist = root.ChildNodes;

        string oldinterfacename = doc.DocumentElement["oldinterfacename"].InnerText; 

        for (int i = 0; i < wolist.Count; i++)
        {
            string name = wolist[i].Name;
            string val = wolist[i].InnerText;
            nvc.Add(name, val);
        }

        Interface objinf = new Interface(userid, "InfMaster", "InterfaceName");
        success = objinf.Create(nvc);

        if (success)
        {
            success = objinf.CopyInfInputMap(oldinterfacename,nvc["interfacename"]);
        }

        if (success)
        {
            returnStr = "TRUE^" + nvc["interfacename"];
        }
        else
            returnStr = "FALSE^" + objinf.ErrorMessage.Replace("\r\n", "");

        return returnStr;

    }

    [OperationContract]
    public ResultInfMasterLinq SearchDataAndCount(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
      ResultInfMasterLinq result = new ResultInfMasterLinq();

      string searchcriteria;
      List<string> strlist = new List<string>();

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

      var query = from i in dc.InfMasterLinqs select i;
      GridLinqBindingData<InfMasterLinq> data = RadGrid.GetBindingData<InfMasterLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

      //GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "InfMasterLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
      result.Data = data.Data.OfType<InfMasterLinq>().ToList();
      result.Count = data.Count;

      return result;
    }

    [OperationContract]
    [WebGet]
    public ResultInfMasterLinq QueryResultInterface(int startRowIndex, int maximumRows, string sortExpression, string filterExpression, string jsonData)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
        ResultInfMasterLinq result = new ResultInfMasterLinq();

        string searchcriteria;
        List<string> strlist = new List<string>();

        searchcriteria = HttpContext.Current.Request.QueryString.Get("wherestring") ?? "";

        NameValueCollection nvc = AzzierData.DeserializeJSON(jsonData);
        object empid = HttpContext.Current.Session["LogIn"];
        string userid = (empid != null) ? empid.ToString() : "";
        ModuleoObject objInterface = new ModuleoObject(userid, "Infmaster", "InterfaceName");
        string wherestring = "", linqwherestring = "";
        if (jsonData.Contains("queryID"))
          nvc = UserQuery.CreateNameValueCollection(nvc["queryID"]);
        objInterface.CreateLinqCondition(nvc, ref wherestring, ref linqwherestring);
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

        var query = from i in dc.InfMasterLinqs select i;
        GridLinqBindingData<InfMasterLinq> data = RadGrid.GetBindingData<InfMasterLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);

        //GridBindingData data = RadGrid.GetBindingData("DataClassesDataContext", "InfMasterLinqs", startRowIndex, maximumRows, sortExpression, filterExpression);
        result.Data = data.Data.OfType<InfMasterLinq>().ToList();
        result.Count = data.Count;

        return result;
    }

    [OperationContract]
    public ResultInfMasterLinq GetWorkList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
      ResultInfMasterLinq result = new ResultInfMasterLinq();

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
                  join i in dc.InfMasterLinqs on u.LinkId equals i.InterfaceName
                  where u.LinkModule.ToLower() == "interface" && u.LinkType == "worklist" && u.UserId.ToLower() == login_user.ToLower()
                  orderby i.InterfaceName
                  select i;

      GridLinqBindingData<InfMasterLinq> data = RadGrid.GetBindingData<InfMasterLinq>(query, startRowIndex, maximumRows, sortExpression, filterExpression);
      result.Data = data.Data.ToList<InfMasterLinq>();
      result.Count = data.Count;

      return result;
    }

    [OperationContract]
    [WebGet]
    public string UpdateInterface(string xml)
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

      XmlNodeList inflist = root.ChildNodes;

      for (int i = 0; i < inflist.Count; i++)
      {
        string name = inflist[i].Name;
        string val = inflist[i].InnerText;
        nvc.Add(name, val);
      }

      Interface objinf;

      string interfacename = nvc["interfacename"];
      objinf = new Interface(o.ToString(), "InfMaster", "InterfaceName", interfacename);
      success = objinf.Update(nvc);
      if (success)
      {
        returnStr = "TRUE^" + nvc["interfacename"];
      }
      else
      {
        string err = "FALSE^" + objinf.ErrorMessage.Replace("\r\n", "");
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
    public RecentInterfaceResult GetRecentList(int startRowIndex, int maximumRows, string sortExpression, string filterExpression)
    {
        System.Web.HttpContext.Current.Session.LCID = Convert.ToInt32(System.Web.HttpContext.Current.Session["LCID"]);
      RecentInterfaceResult result = new RecentInterfaceResult();

      string login_user = "";
      if (System.Web.HttpContext.Current.Session["Login"] != null)
      {
        login_user = HttpContext.Current.Session["Login"].ToString();
      }

      var qq = from v in dc.v_InterfaceRecentLists
               where v.UserId.ToLower() == login_user.ToLower()
               orderby v.AccessDate descending
               select v;

      GridLinqBindingData<v_InterfaceRecentList> resultData = RadGrid.GetBindingData<v_InterfaceRecentList>(qq, startRowIndex, maximumRows, sortExpression, filterExpression);
      result.Data = resultData.Data.ToList<v_InterfaceRecentList>();
      result.Count = resultData.Count;

      return result;
    }

    [OperationContract]
    [WebGet]
    public string SaveInterfaceTable(string xml)
    {
        string result = "OK";
       
        string login_user = "";
        if (System.Web.HttpContext.Current.Session["Login"] != null)
        {
            login_user = HttpContext.Current.Session["Login"].ToString();
            if (login_user == "")
            {
                return "ERROR! Invalid Login.";
            }
        }
        else
        {
            return "ERROR! Invalid Login.";
        }

        

        string counter = "";
        XmlDocument xmldoc = new XmlDocument();
        xmldoc.LoadXml(xml);
        if (xmldoc.DocumentElement["counter"] != null)
        {
            counter = xmldoc.DocumentElement["counter"].InnerText;
        }
        else
        {
            return "Error! Invalid interface table key.";
        }


        ModuleoObject inftableobj = new ModuleoObject(login_user, "InfTable", "Counter",counter);
        NameValueCollection inftablenvc = new NameValueCollection();
        inftablenvc = inftableobj.ModuleData;
        inftablenvc["tagname"] = xmldoc.DocumentElement["tagname"].InnerText;

        if (!inftableobj.Update(inftablenvc)) 
        {
            return inftableobj.ErrorMessage;
        }

        return result;
    }

    [OperationContract]
    [WebGet]
    public string InterfaceImport(string xmlstring, string interfacename)
    {
        string result = "OK";
        OleDbConnection conn = new OleDbConnection(System.Web.HttpContext.Current.Application["ConnString"].ToString());
        OleDbCommand cmd;
        string sql = "";
        
        string login_user = "";
        if (System.Web.HttpContext.Current.Session["Login"] != null)
        {
            login_user = HttpContext.Current.Session["Login"].ToString();
            if (login_user == "")
            {
                return "ERROR! Invalid Login.";
            }
        }
        else
        {
            return "ERROR! Invalid Login.";
        }


        XmlDocument xmldoc = new XmlDocument();
        xmldoc.LoadXml(xmlstring);
        Interface obj = new Interface(login_user, "InfMaster", "Counter");
        List<InfTableLinq> inftablelist = dc.InfTableLinqs.Where(x=>x.InterfaceName == interfacename).ToList();

        conn.Open();
        foreach (InfTableLinq inftab in inftablelist)
        {
            string temptablename = "Interface" + "_" + inftab.SourceTable + "_temp";
            sql = "IF OBJECT_ID (N'" + temptablename + "', N'U') IS NOT NULL SELECT 1 res ELSE SELECT 0";
            cmd = new OleDbCommand(sql, conn);
            int temptablecount = Convert.ToInt32(cmd.ExecuteScalar());
            if (temptablecount == 0)
            {
                sql = "SELECT COUNT(*) FROM " + inftab.temptablename;
                cmd = new OleDbCommand(sql, conn);
                int datacount = Convert.ToInt32(cmd.ExecuteScalar());
                if (datacount <= 0)
                {
                    obj.DropTempTable(inftab.Counter.ToString());
                    obj.GenerateTempTable(inftab.Counter.ToString());
                }
                else
                {
                    if (inftab.temptablename == null)
                    {
                        inftab.temptablename = temptablename;
                        try
                        {
                            dc.SubmitChanges();
                        }
                        catch (Exception e)
                        {
                            result = e.Message;
                            return result;
                        }
                    }
                }
            }
            else 
            {
                obj.GenerateTempTable(inftab.Counter.ToString());
            }
            
        }

        int batchnumber = 0;

        NameValueCollection inflognvc = new NameValueCollection();
        inflognvc["empid"] = login_user;
        inflognvc["transdate"] = DateTime.Today.ToString();
        inflognvc["interfacename"] = "Interface Importing";
        inflognvc["datastring"] = xmlstring;
        ModuleoObject inflogobj = new ModuleoObject(login_user, "InterfaceLog","Counter");
        inflogobj.Create(inflognvc);
        batchnumber = Convert.ToInt32(inflogobj.ModuleData["counter"]);

        


        if (!obj.ImportInterfaceXmlFile(xmldoc, interfacename, batchnumber))
        {
            return obj.ErrorMessage;
        }

        inftablelist = dc.InfTableLinqs.Where(x => x.InterfaceName == interfacename).ToList();

        foreach (InfTableLinq inftable in inftablelist)
        {
            if (!obj.ValidateTableData(interfacename, inftable.SourceTable, batchnumber, inftable.temptablename))
            {
                result = result + obj.ErrorMessage;
            }
        }
        obj.ValidateNetedData(interfacename, "", batchnumber, null, 0);
        obj.InsertDatatoTable(interfacename, "", batchnumber, "", new NameValueCollection());
        return result;
    }
    [OperationContract]
    [WebGet]
    public string DeleteInterface(string userid, string interfacename)
    {
        string result = "OK";
        InfMasterLinq inf = dc.InfMasterLinqs.Where(x => x.InterfaceName.ToLower() == interfacename.ToLower()).FirstOrDefault();
        List<InfTableLinq> tables = dc.InfTableLinqs.Where(x => x.InterfaceName.ToLower() == interfacename.ToLower()).ToList();
        foreach (InfTableLinq table in tables)
        {
            List<InfInputMapLinq> mappings = dc.InfInputMapLinqs.Where(x => x.InterfaceName.ToLower() == interfacename.ToLower() && x.SourceTable.ToLower() == table.SourceTable.ToLower()).ToList();
            if (mappings.Count > 0)
            {

                dc.InfInputMapLinqs.DeleteAllOnSubmit(mappings);
            }

        }
        if (tables.Count > 0)
        {

            dc.InfTableLinqs.DeleteAllOnSubmit(tables);
        }
        if (inf != null)
        {
            dc.InfMasterLinqs.DeleteOnSubmit(inf);
        }

        try
        {
            dc.SubmitChanges();
        }
        catch (Exception e)
        {
            result = e.Message;
            return result;
        }

        return result;
    } 


	// Add more operations here and mark them with [OperationContract]
}
public class RecentInterfaceResult
{
  public Int32 Count { get; set; }
  public List<v_InterfaceRecentList> Data { get; set; }
}

public class ResultInfMasterLinq
{
  public int Count { get; set; }
  public List<InfMasterLinq> Data { get; set; }
}
