using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Runtime.InteropServices;
using MySql.Data.MySqlClient;
using System.Web;
using log4net;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService" in both code and config file together.
[ServiceContract]
public interface IService
{
    [OperationContract]
    [WebInvoke(Method = "GET",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "sysuseraccounts/?")]
    response sysuseraccounts();


    [OperationContract]
    [WebInvoke(Method = "GET",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "checkversionGlobal/?callType={callType}")]
    versionsresponse checkversionGlobal(String callType);

    [OperationContract]
    [WebInvoke(Method = "GET",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "roles/?")]
    response roles();

    [OperationContract]
    [WebInvoke(Method = "GET",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "region/?")]
    response region();

    [OperationContract]
    [WebInvoke(Method = "GET",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "sysuserroles/?")]
    response sysuserroles();

     [OperationContract]
    [WebInvoke(Method = "GET",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "zonecodes/?")]
    response zonecodes();

    [OperationContract]
    [WebInvoke(Method = "GET",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "branchusers/?")]
    response branchusers();

    [OperationContract]
    [WebInvoke(Method = "GET",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "mlbranchesstations/?")]
    response mlbranchesstations();

    [OperationContract]
    [WebInvoke(Method = "GET",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "adminusers/?")]
    response adminusers();

    [OperationContract]
    [WebInvoke(Method = "GET",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "area/?")]
    response area();

    [OperationContract]
    [WebInvoke(Method = "GET",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "branches/?")]
    response branches();

    [OperationContract]
    [WebInvoke(Method = "GET",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "getversion/?")]
    versionsresponse getversion();

    [OperationContract]
    [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/savesysuseraccountsglobal")]
    response savesysuseraccountsglobal(String resourceID, String UserLogin, String UserPassword, String sysmodified, String BranchCode, String DeptCode, String IsReliever, String TempBranchCode, String ZoneCode, String StartDate, String EndDate, String IsActive, String syscreated, String syscreator, String sysmodifier, String IsResign);


    [OperationContract]
    [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/savemodifiedsysuserglobal")]
    response savemodifiedsysuserglobal(string resourceID, string UserPassword, string sysmodified);

    [OperationContract]
    [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/savesysuserroles")]
    response savesysuserroles(Int32 ResourceID, Int32 Type, String Role, Int32 ZoneCode, String syscreator, String syscreated, String sysmodified, String sysmodifier, String agenttype);

    [OperationContract]
    [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/saveroles")]
    response saveroles(String Role, String Description, Int32 RoleType, String syscreator, String syscreated, String sysmodified, String sysmodifier, String type);

    [OperationContract]
    [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/saverr")]
    response saverr(string ric, string ryan);


    [OperationContract]
    [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/savebranchuser")]
    response savebranchuser( String BranchCode, String resourceID ,Int32 IsReliever, Int32 IsActiveReliever ,String syscreated, String sysmodified,Int32 ZoneCode ,String fullname,String lastname,String firstname ,String middlename ,String syscreator , String sysmodifier);


    [OperationContract]
    [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/savemodifiedbranchuser")]
    response savemodifiedbranchuser(String BranchCode, String resourceID, Int32 IsReliever, Int32 IsActiveReliever, String syscreated, String sysmodified, Int32 ZoneCode, String fullname, String lastname, String firstname, String middlename, String syscreator, String sysmodifier);

    [OperationContract]
    [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/savemlbranchesstations")]
    response savemlbranchesstations(String StationCode, Int32 StationNo, String BranchCode, String syscreated, String syscreator, String sysmodified, String sysmodifier, Int32 ZoneCode, String branchstat, Int32 IsUpdated, String version, Int32 isdowngraded, String adminversion, Int32 releasetype, Int32 isallowupdate, Int32 isallowupdateadmin, String type);
	// TODO: Add your service operations here

    [OperationContract]
    [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/saveadminusers")]
    response saveadminusers(Int32 ResourceID, String DeptCode, String Fullname, String Firstname, String Lastname, String Middlename, Int32 ZoneCode, String syscreator, String syscreated, String sysmodifier, String sysmodified, String type);

    [OperationContract]
    [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/savearea")]
    response savearea(String AreaName, String AreaCode, Int32 RegionCode, Int32 ZoneCode, String syscreated, String syscreator, String sysmodified, String sysmodifier, String type);


     [OperationContract]
    [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/savebranches")]
    response savebranches(Int32 ZoneCode, String AreaCode, String BranchCode, String BranchName, String Address, String TINNo, Int32 Status, String ClosedDate, String CorpName, Decimal VAT, String TelNo, String CellNo, String syscreator, String syscreated, String sysmodifier, String sysmodified, String PermitNo, String KP4CODE, String FaxNo, String Regioncode, Int32 is24Hours, Int32 allowBeyondOffHour, String timeFrom, String timeTo, String type);


      [OperationContract]
    [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "/saveregion")]
    response saveregion(Int32 RegionCode, String RegionName, Int32 ZoneCode,  String syscreator, String syscreated, String sysmodified, String sysmodifier, String type);

      [OperationContract]
      [WebInvoke(Method = "POST",
          ResponseFormat = WebMessageFormat.Json,
          BodyStyle = WebMessageBodyStyle.WrappedRequest,
          UriTemplate = "/savezonecodes")]
      response savezonecodes(Int32 ZoneCode, String ZoneName, String syscreator, String syscreated, String sysmodifier, String sysmodified,String agenttype);
     

 // ------------------------------------------------------------------  C L O U D ----------------------------------------------------------------

   [OperationContract]
    [WebInvoke(Method = "GET",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "getdatasysuseraccounts/?modified={modified}&created={created}")]
    response getdatasysuseraccounts(String modified, String created);


    [OperationContract]
    [WebInvoke(Method = "GET",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "getdatabranchusers/?modified={modified}&created={created}")]
    response getdatabranchusers(String modified, String created);


    [OperationContract]
    [WebInvoke(Method = "GET",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "getdatamlbranchesstations/?modified={modified}&created={created}")]
    response getdatamlbranchesstations(String modified, String created);

    [OperationContract]
    [WebInvoke(Method = "GET",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "getupdatedversionsGlobal/?updatesversion={updatesversion}&type={type}&datereleased={datereleased}")]
    versionsresponse getupdatedversionsGlobal(double updatesversion, Int32 type, String datereleased);

    [OperationContract]
    [WebInvoke(Method = "GET",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "getdataadminusers/?modified={modified}&created={created}")]
    response getdataadminusers(String modified, String created);

    [OperationContract]
    [WebInvoke(Method = "GET",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "getdataroles/?modified={modified}&created={created}")]
    response getdataroles(String modified, String created);

    [OperationContract]
    [WebInvoke(Method = "GET",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "getdataarea/?modified={modified}&created={created}")]
    response getdataarea(String modified, String created);

    [OperationContract]
    [WebInvoke(Method = "GET",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "getdataregion/?modified={modified}&created={created}")]
    response getdataregion(String modified, String created);

    [OperationContract]
    [WebInvoke(Method = "GET",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "getdatazonecodes/?modified={modified}&created={created}")]
    response getdatazonecodes(String modified, String created);

    [OperationContract]
    [WebInvoke(Method = "GET",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "getdatasysuserroles/?modified={modified}&created={created}")]
    response getdatasysuserroles(String modified, String created);


    [OperationContract]
    [WebInvoke(Method = "GET",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        UriTemplate = "getdatabranches/?modified={modified}&created={created}")]
    response getdatabranches(String modified, String created);


    //[OperationContract]
    //[WebInvoke(Method = "GET",
    //    ResponseFormat = WebMessageFormat.Json,
    //    BodyStyle = WebMessageBodyStyle.WrappedRequest,
    //    UriTemplate = "getupdatedversions/?sysmenuversion={sysmenuversion}&updatesversion={updatesversion}&versions={versions}&pluginCount={pluginCount}&assemblyCount={assemblyCount}&type={type}")]
    //versionsresponse getupdatedversions(double sysmenuversion, double updatesversion, double versions, Int32 pluginCount, Int32 assemblyCount, Int32 type);

}
// Use a data contract as illustrated in the sample below to add composite types to service operations.
[DataContract]
public class IniFile
{
    public string path;

    [DllImport("kernel32")]
    private static extern long WritePrivateProfileString(string section,
        string key, string val, string filePath);
    [DllImport("kernel32")]
    private static extern int GetPrivateProfileString(string section,
             string key, string def, StringBuilder retVal,
        int size, string filePath);


    public IniFile(string INIPath)
    {
        path = INIPath;
    }

    public void IniWriteValue(string Section, string Key, string Value)
    {
        WritePrivateProfileString(Section, Key, Value, this.path);
    }


    public string IniReadValue(string Section, string Key)
    {
        StringBuilder temp = new StringBuilder(255);
        int i = GetPrivateProfileString(Section, Key, "", temp,
                                        255, this.path);
        return temp.ToString();

    }
}


[DataContract]
public class DBConnection
{
    private MySqlConnection connection;
    private Boolean pool = false;
    String path;
    private static readonly ILog kplog = LogManager.GetLogger(typeof(DBConnection));
    //Constructor
    public DBConnection(String Serv, String DB, String UID, String Password, String pooling, Int32 maxcon, Int32 mincon, Int32 tout)
    {
        Initialize(Serv, DB, UID, Password, pooling, maxcon, mincon, tout);
    }

    //Initialize values
    private void Initialize(String Serv, String DB, String UID, String Password, String pooling, Int32 maxcon, Int32 mincon, Int32 tout)
    {
        try
        {
            if (pooling.Equals("1"))
            {
                pool = true;
            }

            string myconstring = "server = " + Serv + "; database = " + DB + "; uid = " + UID + ";password= " + Password + "; pooling=" + pool + ";min pool size=" + mincon + ";max pool size=" + maxcon + "; Connection Lifetime=0 ;Command Timeout=28800; connection timeout=" + tout + ";Allow Zero Datetime=true";
            connection = new MySqlConnection(myconstring);
        }
        catch (Exception ex)
        {
            kplog.Fatal("Unable to connect", ex);
            throw new Exception(ex.Message);
        }

    }

    public String Path
    {
        get { return path; }
        set { path = value; }
    }
    //open connection to database
    public bool OpenConnection()
    {
        try
        {
            connection.Open();
            return true;
        }
        catch (MySqlException)
        {
            //When handling errors, you can your application's response based 
            //on the error number.
            //The two most common error numbers when connecting are as follows:
            //0: Cannot connect to server.
            //1045: Invalid user name and/or password.
            return false;
        }
    }

    //Close connection
    public bool CloseConnection()
    {
        try
        {
            connection.Close();
            return true;
        }
        catch (MySqlException)
        {
            return false;
        }
    }
        
    public MySqlConnection getConnection()
    {
        return connection;
    }

    public void dispose()
    {
        connection.Dispose();
    }

}


[DataContract]
public class response
{
    [DataMember]
    public int respcode { get; set; }
    [DataMember]
    public String respmsg { get; set; }
    [DataMember]
    public String errordetail { get; set; }
    [DataMember]
    public Int32 count { get; set; }
}

public class listsysmenu
{
    [DataMember]
    public Int64 MID { get; set; }
    [DataMember]
    public Int64 parentID { get; set; }
    [DataMember]
    public String RoleCode { get; set; }
    [DataMember]
    public String DisplayName { get; set; }
    [DataMember]
    public Int64 MenuIndex { get; set; }
    [DataMember]
    public int isDropDown { get; set; }
    [DataMember]
    public Int32 PID { get; set; }
    [DataMember]
    public Double version { get; set; }
    [DataMember]
    public int type { get; set; }
}

[DataContract]
public class updatesdetails
{
    [DataMember]
    public String filename { get; set; }
    [DataMember]
    public String crc { get; set; }
    [DataMember]
    public String apptype { get; set; }
    [DataMember]
    public Decimal version { get; set; }
}

[DataContract]
public class versionsdetails
{
    [DataMember]
    public Decimal versionno { get; set; }
    [DataMember]
    public String path { get; set; }
    [DataMember]
    public String description { get; set; }
    [DataMember]
    public String datereleased { get; set; }
    [DataMember]
    public int AppType { get; set; }
    [DataMember]
    public int forDowngrade { get; set; }
    [DataMember]
    public int releasetype { get; set; }
}

[DataContract]
public class pluginregistrydetails
{
    [DataMember]
    public String PID { get; set; }
    [DataMember]
    public String type { get; set; }
    [DataMember]
    public String AID { get; set; }
    [DataMember]
    public String PluginType { get; set; }
    [DataMember]
    public String syscreated { get; set; }
}

[DataContract]
public class sysassemblydetails
{
    [DataMember]
    public String AID { get; set; }
    [DataMember]
    public String AssemblyName { get; set; }
    [DataMember]
    public String version { get; set; }
    [DataMember]
    public String syscreated { get; set; }
}

[DataContract]
public class menuReturn
{
    [DataMember]
    public int respcode { get; set; }
    [DataMember]
    public string respmsg { get; set; }
    [DataMember]
    public List<listsysmenu> symenuresp { get; set; }
    [DataMember]
    public List<versionsdetails> versionresp { get; set; }
    [DataMember]
    public List<updatesdetails> updatesresp { get; set; }
    [DataMember]
    public List<pluginregistrydetails> pluginResp { get; set; }
    [DataMember]
    public List<sysassemblydetails> assemblyResp { get; set; }
}



[DataContract]
public class versionsresponse
{
    [DataMember]
    public Int16 respcode { get; set; }
    [DataMember]
    public String respmsg { get; set; }
    [DataMember]
    public Double clientsysmenu { get; set; }
    [DataMember]
    public Double adminsysmenu { get; set; }
    [DataMember]
    public Double updatesclient { get; set; }
    [DataMember]
    public Double updatesadmin { get; set; }
    [DataMember]
    public Double versionclient { get; set; }
    [DataMember]
    public Double versionadmin { get; set; }
    [DataMember]
    public Int32 syspluginregistrycount { get; set; }
    [DataMember]
    public Int32 sysassemblycount { get; set; }
    [DataMember]
    public String datereleased { get; set; }
    [DataMember]
    public List<listsysmenu> symenuresp { get; set; }
    [DataMember]
    public List<updatesdetails> updatesresp { get; set; }
    [DataMember]
    public List<versionsdetails> versionresp { get; set; }
    [DataMember]
    public List<pluginregistrydetails> pluginregistryresp { get; set; }
    [DataMember]
    public List<sysassemblydetails> assemblyresp { get; set; }

}