using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.Data;
using HttpUtils;
using Newtonsoft.Json.Linq;
using System.Net.Security;
using System.Net;
using System.IO;
using Newtonsoft.Json;
// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in code, svc and config file together.
public class Service : IService
{
    private String path;
    private DBConnection dbconcloud;
    private String NetworkServicePath;




    public Service()
    {
        path = "C:\\kpconfig\\userloginagentglobal.ini";

        IniFile ini = new IniFile(path);

        // ---- connect to cloud database 

        String Serv = ini.IniReadValue("cloud_config", "server");
        String DB = ini.IniReadValue("cloud_config", "database");
        String UID = ini.IniReadValue("cloud_config", "uid"); ;
        String Password = ini.IniReadValue("cloud_config", "password"); ;
        String pool = ini.IniReadValue("cloud_config", "pool");
        Int32 maxcon = Convert.ToInt32(ini.IniReadValue("cloud_config", "maxcon"));
        Int32 mincon = Convert.ToInt32(ini.IniReadValue("cloud_config", "mincon"));
        Int32 tout = Convert.ToInt32(ini.IniReadValue("cloud_config", "tout"));
        dbconcloud = new DBConnection(Serv, DB, UID, Password, pool, maxcon, mincon, tout);

        NetworkServicePath = ini.IniReadValue("NetworkService", "Services");

        // ---- connect to network database 



    }


    #region 1st Calling Cloud

    /// --------------------- START OF  USERLOGINAGENT CLOUD ---------------------------------------------------------------------------------------------------

    //sysuseraccounts
    
    public response sysuseraccounts()
    
    {

        try
        {
            String maxmodified = string.Empty;
            String maxcreated = string.Empty;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
            String endpoint = NetworkServicePath + "/getdatasysuseraccounts";
           // String endpoint = "http://localhost:59245/UserloginAgentNetworkService/Service.svc/getdatasysuseraccounts";
            using (MySqlConnection con = dbconcloud.getConnection())
            {
                con.Open();

                using (MySqlCommand comm = con.CreateCommand())
                {
                    comm.CommandText = "Select DATE_FORMAT(MAX(syscreated),'%Y-%m-%d %T') as maxcreated, DATE_FORMAT(MAX(sysmodified),'%Y-%m-%d %T') as maxmodified FROM `kpusers`.`sysuseraccounts`;";
                    
                    MySqlDataReader rdr = comm.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        rdr.Read();
                        maxmodified = rdr["maxmodified"].ToString();
                        maxcreated = rdr["maxcreated"].ToString();

                        rdr.Close();
                        con.Close();
                        var client = new HttpUtils.RestClient(endpoint);
                        var jsonresponse = client.MakeRequest("?modified=" + maxmodified + "&created=" + maxcreated);
                        dynamic result = JObject.Parse(jsonresponse.ToString());
                       // Int32 statcode = result.respcode;
                        

                        if (result.respcode == 1)
                        {
                            return new response { respcode = 1, respmsg = result.respmsg};

                        }
                        else
                        {
                            return new response { respcode = 0, respmsg = result.respmsg };
                        }

                    }
                    else
                    {
                        rdr.Close();
                        con.Close();
                        return new response { respcode = 0, respmsg = "No updated data." };
                    }

                }
            }
        }
        
        catch (Exception ex)
        {
            return new response { respmsg = ex.ToString() };
        }

    }

    //branchusers
    public response branchusers()
    {
        try
        {
            String maxmodified = string.Empty;
            String maxcreated = string.Empty;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
            String endpoint = NetworkServicePath + "/getdatabranchusers/";
            using (MySqlConnection con = dbconcloud.getConnection())
            {
                con.Open();
                using (MySqlCommand comm = con.CreateCommand())
                {
                    comm.CommandText = "Select DATE_FORMAT(MAX(syscreated),'%Y-%m-%d %T') as maxcreated, DATE_FORMAT(MAX(sysmodified),'%Y-%m-%d %T') as maxmodified FROM `kpusers`.`branchusers`;";
                    MySqlDataReader rdr = comm.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        rdr.Read();
                        maxmodified = rdr["maxmodified"].ToString();
                        maxcreated = rdr["maxcreated"].ToString();

                        rdr.Close();
                        con.Close();
                        var client = new HttpUtils.RestClient(endpoint);
                        var jsonresponse = client.MakeRequest("?modified=" + maxmodified + "&created=" + maxcreated);
                        dynamic result = JObject.Parse(jsonresponse.ToString());
                        Int32 statcode = result.respcode;
                       

                        if (statcode == 1)
                        {
                            return new response { respcode = 1 , respmsg = result.respmsg};

                        }
                        else
                        {
                            return new response { respcode = 0, respmsg = result.respmsg };
                        }

                    }
                    else
                    {
                        rdr.Close();
                        con.Close();
                        return new response { respcode = 0, respmsg = "No updated data." };
                    }

                }
            }
        }
        catch (Exception ex)
        {
            return new response { respmsg = ex.ToString() };
        }

    }

    public response mlbranchesstations()
    {

        try
        {
            String maxmodified = string.Empty;
            String maxcreated = string.Empty;

          
            using (MySqlConnection con = dbconcloud.getConnection())
            {
                con.Open();
           
                using (MySqlCommand comm = con.CreateCommand())
                {
                    comm.CommandText = "Select DATE_FORMAT(MAX(syscreated),'%Y-%m-%d %T') as maxcreated, DATE_FORMAT(MAX(sysmodified),'%Y-%m-%d %T') as maxmodified FROM `kpusers`.`mlbranchesstations`;";
                    MySqlDataReader reader = comm.ExecuteReader();

                    if (reader.HasRows) {
                        reader.Read();
                        maxmodified = reader["maxmodified"].ToString();
                        maxcreated = reader["maxcreated"].ToString();
                        reader.Close();
                    }
                    
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
                    String endpoint = NetworkServicePath + "/getdatamlbranchesstations";
                    var client = new HttpUtils.RestClient(endpoint);
                    var jsonresponse = client.MakeRequest("/?modified=" + maxmodified + "&created="+maxcreated+"");
                    dynamic result = JObject.Parse(jsonresponse.ToString());
                    Int32 statcode = result.respcode;
                    

                    if (statcode == 1)
                    {
                        con.Close();
                        return new response { respcode = 1, respmsg = result.respmsg };

                    }
                    else
                    {
                        con.Close();
                        return new response { respcode = 0, respmsg = result.respmsg };
                    }
                 
                }
                
                
            }
        }
        catch (Exception ex)
        {
            return new response { respcode = 101, respmsg = ex.ToString() };
        }
        }

    //adminusers

    public response adminusers() 
    {

        try
        {
            string maxmodified = string.Empty;
            string maxcreated = string.Empty;
            try
            {

                using (MySqlConnection con = dbconcloud.getConnection())
                {
                    con.Open();
                    using (MySqlCommand comm = con.CreateCommand())
                    {
                        try
                        {
                            comm.Parameters.Clear();
                            comm.CommandText = "Select DATE_FORMAT(MAX(syscreated),'%Y-%m-%d %T') as maxcreated, DATE_FORMAT(MAX(sysmodified),'%Y-%m-%d %T') as maxmodified FROM `kpusers`.`adminusers`;";
                            MySqlDataReader rdr =  comm.ExecuteReader();
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                maxmodified = rdr["maxmodified"].ToString();
                                maxcreated = rdr["maxcreated"].ToString();
                                rdr.Close();
                            }
                            else
                            {
                                return new response { respcode = 0, respmsg = "Not Found!" };
                            }

                            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
                            String endpoint = NetworkServicePath + "/getdataadminusers";
                           // String endpoint = NetworkServicePath + "/getdataadminusers";
                            var client = new HttpUtils.RestClient(endpoint);
                            var jsonresponse = client.MakeRequest("/?modified=" + maxmodified + "&created=" + maxcreated + "");
                            dynamic result = JObject.Parse(jsonresponse.ToString());
                            Int32 statcode = result.respcode;
                            string msg = result.respmsg;

                            if (statcode == 1)
                            {
                                con.Close();
                                return new response { respcode = 1, respmsg = msg };

                            }
                            else
                            {
                                con.Close();
                                return new response { respcode = 0, respmsg = msg };
                            }
                          

                        }
                        catch (Exception ex)
                        {

                            return new response { respcode = 0, respmsg = ex.ToString()};
                        }
   
                    }

                }
            }
            catch (Exception ex)
            {

                return new response { respcode = 0 , respmsg = ex.ToString()};
            }
           
        }
        catch (Exception ex)
        {

            return new response { respcode = 0 , respmsg = ex.ToString() };
        }
      
    }

    //area

    public response area()
    {

        try
        {
            string maxmodified = string.Empty;
            string maxcreated = string.Empty;
            try
            {

                using (MySqlConnection con = dbconcloud.getConnection())
                {
                    con.Open();
                    using (MySqlCommand comm = con.CreateCommand())
                    {
                        try
                        {
                            comm.Parameters.Clear();
                            comm.CommandText = "Select DATE_FORMAT(MAX(syscreated),'%Y-%m-%d %T') as maxcreated, DATE_FORMAT(MAX(sysmodified),'%Y-%m-%d %T') as maxmodified FROM `kpusers`.`area`;";
                            MySqlDataReader rdr = comm.ExecuteReader();
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                maxmodified = rdr["maxmodified"].ToString();
                                maxcreated = rdr["maxcreated"].ToString();
                                rdr.Close();
                            }
                            else
                            {
                                return new response { respcode = 0, respmsg = "Not Found!" };
                            }

                            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
                            String endpoint = NetworkServicePath + "/getdataarea";
                            var client = new HttpUtils.RestClient(endpoint);
                            var jsonresponse = client.MakeRequest("/?modified=" + maxmodified + "&created=" + maxcreated + "");
                            dynamic result = JObject.Parse(jsonresponse.ToString());
                            Int32 statcode = result.respcode;
                            string msg = result.respmsg;

                            if (statcode == 1)
                            {
                                con.Close();
                                return new response { respcode = 1, respmsg = msg };

                            }
                            else
                            {
                                con.Close();
                                return new response { respcode = 0, respmsg = msg };
                            }


                        }
                        catch (Exception ex)
                        {

                            return new response { respcode = 0, respmsg = ex.ToString() };
                        }

                    }

                }
            }
            catch (Exception ex)
            {

                return new response { respcode = 0, respmsg = ex.ToString() };
            }

        }
        catch (Exception ex)
        {

            return new response { respcode = 0, respmsg = ex.ToString() };
        }

    }

    //--------------BRANCHES---------------

    public response branches()
    {

        try
        {
            string maxmodified = string.Empty;
            string maxcreated = string.Empty;
            try
            {

                using (MySqlConnection con = dbconcloud.getConnection())
                {
                    con.Open();
                    using (MySqlCommand comm = con.CreateCommand())
                    {
                        try
                        {
                            comm.Parameters.Clear();
                            comm.CommandText = "Select DATE_FORMAT(MAX(syscreated),'%Y-%m-%d %T') as maxcreated, DATE_FORMAT(MAX(sysmodified),'%Y-%m-%d %T') as maxmodified FROM `kpusers`.`branches`;";
                            MySqlDataReader rdr = comm.ExecuteReader();
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                maxmodified = rdr["maxmodified"].ToString();
                                maxcreated = rdr["maxcreated"].ToString();
                                rdr.Close();
                            }
                            else
                            {
                                return new response { respcode = 0, respmsg = "Not Found!" };
                            }

                            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
                            String endpoint = NetworkServicePath + "/getdatabranches";
                            var client = new HttpUtils.RestClient(endpoint);
                            var jsonresponse = client.MakeRequest("/?modified=" + maxmodified + "&created=" + maxcreated + "");
                            dynamic result = JObject.Parse(jsonresponse.ToString());
                            Int32 statcode = result.respcode;
                            string msg = result.respmsg;

                            if (statcode == 1)
                            {
                                con.Close();
                                return new response { respcode = 1, respmsg = msg };

                            }
                            else
                            {
                                con.Close();
                                return new response { respcode = 0, respmsg = msg };
                            }


                        }
                        catch (Exception ex)
                        {

                            return new response { respcode = 0, respmsg = ex.ToString() };
                        }

                    }

                }
            }
            catch (Exception ex)
            {

                return new response { respcode = 0, respmsg = ex.ToString() };
            }

        }
        catch (Exception ex)
        {

            return new response { respcode = 0, respmsg = ex.ToString() };
        }

    }

    //---------------------------------ZONECODES------------------------

    public response zonecodes()
    {

        try
        {
            string maxmodified = string.Empty;
            string maxcreated = string.Empty;
            try
            {

                using (MySqlConnection con = dbconcloud.getConnection())
                {
                    con.Open();
                    using (MySqlCommand comm = con.CreateCommand())
                    {
                        try
                        {
                            comm.Parameters.Clear();
                            comm.CommandText = "Select DATE_FORMAT(MAX(syscreated),'%Y-%m-%d %T') as maxcreated, DATE_FORMAT(MAX(sysmodified),'%Y-%m-%d %T') as maxmodified FROM `kpusers`.`zonecodes`;";
                            MySqlDataReader rdr = comm.ExecuteReader();
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                maxmodified = rdr["maxmodified"].ToString();
                                maxcreated = rdr["maxcreated"].ToString();
                                rdr.Close();
                            }
                            else
                            {
                                return new response { respcode = 0, respmsg = "Not Found!" };
                            }

                            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
                          //  String endpoint = NetworkServicePath + "/getdatazonecodes";
                            String endpoint = NetworkServicePath + "/getdatazonecodes";
                            var client = new HttpUtils.RestClient(endpoint);
                            var jsonresponse = client.MakeRequest("/?modified=" + maxmodified + "&created=" + maxcreated + "");
                            dynamic result = JObject.Parse(jsonresponse.ToString());
                            Int32 statcode = result.respcode;
                            string msg = result.respmsg;

                            if (statcode == 1)
                            {
                                con.Close();
                                return new response { respcode = 1, respmsg = msg };

                            }
                            else
                            {
                                con.Close();
                                return new response { respcode = 0, respmsg = msg };
                            }


                        }
                        catch (Exception ex)
                        {

                            return new response { respcode = 0, respmsg = ex.ToString() };
                        }

                    }

                }
            }
            catch (Exception ex)
            {

                return new response { respcode = 0, respmsg = ex.ToString() };
            }

        }
        catch (Exception ex)
        {

            return new response { respcode = 0, respmsg = ex.ToString() };
        }

    }

    //--------------------------------REGIONS--------------------------------

    public response region()
    {

        try
        {
            string maxmodified = string.Empty;
            string maxcreated = string.Empty;
            try
            {

                using (MySqlConnection con = dbconcloud.getConnection())
                {
                    con.Open();
                    using (MySqlCommand comm = con.CreateCommand())
                    {
                        try
                        {
                            comm.Parameters.Clear();
                            comm.CommandText = "Select DATE_FORMAT(MAX(syscreated),'%Y-%m-%d %T') as maxcreated, DATE_FORMAT(MAX(sysmodified),'%Y-%m-%d %T') as maxmodified FROM `kpusers`.`region`;";
                            MySqlDataReader rdr = comm.ExecuteReader();
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                maxmodified = rdr["maxmodified"].ToString();
                                maxcreated = rdr["maxcreated"].ToString();
                                rdr.Close();
                            }
                            else
                            {
                                return new response { respcode = 0, respmsg = "Not Found!" };
                            }

                            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
                            String endpoint = NetworkServicePath + "/getdataregion";
                            var client = new HttpUtils.RestClient(endpoint);
                            var jsonresponse = client.MakeRequest("/?modified=" + maxmodified + "&created=" + maxcreated + "");
                            dynamic result = JObject.Parse(jsonresponse.ToString());
                            Int32 statcode = result.respcode;
                            string msg = result.respmsg;

                            if (statcode == 1)
                            {
                                con.Close();
                                return new response { respcode = 1, respmsg = msg };

                            }
                            else
                            {
                                con.Close();
                                return new response { respcode = 0, respmsg = msg };
                            }


                        }
                        catch (Exception ex)
                        {

                            return new response { respcode = 0, respmsg = ex.ToString() };
                        }

                    }

                }
            }
            catch (Exception ex)
            {

                return new response { respcode = 0, respmsg = ex.ToString() };
            }

        }
        catch (Exception ex)
        {

            return new response { respcode = 0, respmsg = ex.ToString() };
        }

    }
    
    //---------------------------SYSUSERROLES--------------------

    public response sysuserroles()
    {

        try
        {
            string maxmodified = string.Empty;
            string maxcreated = string.Empty;
            try
            {

                using (MySqlConnection con = dbconcloud.getConnection())
                {
                    con.Open();
                    using (MySqlCommand comm = con.CreateCommand())
                    {
                        try
                        {
                            comm.Parameters.Clear();
                            comm.CommandText = "Select DATE_FORMAT(MAX(syscreated),'%Y-%m-%d %T') as maxcreated, DATE_FORMAT(MAX(sysmodified),'%Y-%m-%d %T') as maxmodified FROM `kpusers`.`sysuserroles`;";
                            MySqlDataReader rdr = comm.ExecuteReader();
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                maxmodified = rdr["maxmodified"].ToString();
                                maxcreated = rdr["maxcreated"].ToString();
                                rdr.Close();
                            }
                            else
                            {
                                return new response { respcode = 0, respmsg = "Not Found!" };
                            }

                            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
                            String endpoint = NetworkServicePath + "/getdatasysuserroles";
                          // String endpoint = NetworkServicePath + "/getdatasysuserroles";
                            var client = new HttpUtils.RestClient(endpoint);
                            var jsonresponse = client.MakeRequest("/?modified=" + maxmodified + "&created=" + maxcreated + "");
                            dynamic result = JObject.Parse(jsonresponse.ToString());
                            Int32 statcode = result.respcode;
                          

                            if (statcode == 1)
                            {
                                con.Close();
                                return new response { respcode = 1, respmsg = result.respmsg };

                            }
                            else
                            {
                                con.Close();
                                return new response { respcode = 0, respmsg = result.respmsg };
                            }


                        }
                        catch (Exception ex)
                        {

                            return new response { respcode = 0, respmsg = ex.ToString() };
                        }

                    }

                }
            }
            catch (Exception ex)
            {

                return new response { respcode = 0, respmsg = ex.ToString() };
            }

        }
        catch (Exception ex)
        {

            return new response { respcode = 0, respmsg = ex.ToString() };
        }

    }


    public response roles()
    {

        try
        {
            string maxmodified = string.Empty;
            string maxcreated = string.Empty;
            try
            {

                using (MySqlConnection con = dbconcloud.getConnection())
                {
                    con.Open();
                    using (MySqlCommand comm = con.CreateCommand())
                    {
                        try
                        {
                            comm.Parameters.Clear();
                            comm.CommandText = "Select DATE_FORMAT(MAX(syscreated),'%Y-%m-%d %T') as maxcreated, DATE_FORMAT(MAX(sysmodified),'%Y-%m-%d %T') as maxmodified FROM `kpusers`.`roles`;";
                            MySqlDataReader rdr = comm.ExecuteReader();
                            if (rdr.HasRows)
                            {
                                rdr.Read();
                                maxmodified = rdr["maxmodified"].ToString();
                                maxcreated = rdr["maxcreated"].ToString();
                                rdr.Close();
                            }
                            else
                            {
                                return new response { respcode = 0, respmsg = "Not Found!" };
                            }

                            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
                           // String endpoint = NetworkServicePath + "/getdatarr";
                            String endpoint = NetworkServicePath + "/getdataroles";
                            var client = new HttpUtils.RestClient(endpoint);
                            var jsonresponse = client.MakeRequest("/?modified=" + maxmodified + "&created=" + maxcreated + "");
                            dynamic result = JObject.Parse(jsonresponse.ToString());
                            Int32 statcode = result.respcode;
                            string msg = result.respmsg;

                            if (statcode == 1)
                            {
                                con.Close();
                                return new response { respcode = 1, respmsg = result.respmsg };

                            }
                            else
                            {
                                con.Close();
                                return new response { respcode = 0, respmsg = result.respmsg };
                            }


                        }
                        catch (Exception ex)
                        {

                            return new response { respcode = 0, respmsg = ex.ToString() };
                        }

                    }

                }
            }
            catch (Exception ex)
            {

                return new response { respcode = 0, respmsg = ex.ToString() };
            }

        }
        catch (Exception ex)
        {

            return new response { respcode = 0, respmsg = ex.ToString() };
        }

    }


    public versionsresponse checkversionGlobal(String callType)
    {
        String endpoint = String.Empty;
        String info = String.Empty;
        List<updatesdetails> updatesclientresp = new List<updatesdetails>();
        List<updatesdetails> updatesadminresp = new List<updatesdetails>();
       List<versionsdetails> versionsclientresp = new List<versionsdetails>();
     //   List<versionsdetails> versionsadminresp = new List<versionsdetails>();

        String apptype = string.Empty;
        String filename = string.Empty;
        String crc = string.Empty;
        Decimal version = 0;

        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
       
        endpoint = NetworkServicePath + "/getversion";
        //endpoint = "http://localhost:59245/UserloginAgentNetworkService/Service.svc/getversion";
        var client = new HttpUtils.RestClient(endpoint);
        var jsonresponse = client.MakeRequest("/?");
        dynamic result = JObject.Parse(jsonresponse.ToString());

        Double netupdatesclient = result.updatesclient;
        Double netupdatesadmin = result.updatesadmin;
       // Double netversionclient = result.versionclient;
       // Double netversionadmin = result.versionadmin;
        String netdatelreleased = result.datereleased;

        Double cloudupdatesclient = 0.0;
        Double cloudupdatesadmin = 0.0;
     //   Double cloudversionclient = 0.0;
      //  Double cloudversionadmin = 0.0;
        String clouddatereleased = string.Empty;

        string msgadminupdates = string.Empty;
        string msgclientupdates = string.Empty;
        string msgclientversions = string.Empty;
        string msgadminversions = string.Empty;

        response versionresp = new response();
        response versionrespadmin = new response();
        response updatesresp = new response();
        response updatesrespadmin = new response();

        versionsresponse cloudversions = new versionsresponse();
        cloudversions = getversion();
        clouddatereleased = getdatereleased();
        if (cloudversions.respcode == 1)
        {

            cloudupdatesclient = cloudversions.updatesclient;
            cloudupdatesadmin = cloudversions.updatesadmin;
        //    cloudversionclient = cloudversions.versionclient;
          //  cloudversionadmin = cloudversions.versionadmin;

        }
        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
        endpoint = NetworkServicePath + "/getupdatedversionsGlobal";
        Int32 type = 0;
        menuReturn mr = new menuReturn();

        if (callType == "Updates")
        {

            if (cloudupdatesclient != netupdatesclient)
            {
                double versions = 0.0;
                type = 1;
                // call network's method for updates
                client = new HttpUtils.RestClient(endpoint);
                var response = client.MakeRequest("/?updatesversion=" + cloudupdatesclient + "&type=" + type + "&datereleased=" + string.Empty + "");
                mr = JsonConvert.DeserializeObject<menuReturn>(response);
                updatesclientresp = mr.updatesresp;
                for (int i = 0; i < updatesclientresp.Count; i++)
                {
                    apptype = updatesclientresp[i].apptype;
                    filename = updatesclientresp[i].filename;
                    crc = updatesclientresp[i].crc;
                    version = updatesclientresp[i].version;

                    updatesresp = saveupdatesGlobal(apptype, filename, crc, version);
                    if (updatesresp.respcode == 0)
                    {
                        return new versionsresponse { respcode = 0, respmsg = updatesresp.respmsg };

                    }

                }

                msgclientupdates = "\n  Created New Data: ClientUpdatesCount: " + updatesclientresp.Count;

            }
            if (cloudupdatesadmin != netupdatesadmin)
            {

                type = 0;
                // call network's method for updates
                client = new HttpUtils.RestClient(endpoint);
                var response = client.MakeRequest("/?updatesversion=" + cloudupdatesadmin + "&type=" + type + "&datereleased=" + string.Empty + "");
                mr = JsonConvert.DeserializeObject<menuReturn>(response);
                updatesadminresp = mr.updatesresp;
                for (int i = 0; i < updatesadminresp.Count; i++)
                {
                    apptype = updatesadminresp[i].apptype;
                    filename = updatesadminresp[i].filename;
                    crc = updatesadminresp[i].crc;
                    version = updatesadminresp[i].version;

                    updatesrespadmin = saveupdatesGlobal(apptype, filename, crc, version);
                    if (updatesrespadmin.respcode == 0)
                    {
                        return new versionsresponse { respcode = 0, respmsg = updatesrespadmin.respmsg };

                    }

                }

                msgadminupdates = "\n  Created New Data: AdminUpdatesCount: " + updatesadminresp.Count;


            }
        }
        else if (callType == "Versions")
        {
            if (netdatelreleased != clouddatereleased)
            {
                type = 0;
                //call network's method for version
                client = new HttpUtils.RestClient(endpoint);
                var response = client.MakeRequest("/?&updatesversion=0.0&type=" + type + "&datereleased=" + clouddatereleased + "");
                mr = JsonConvert.DeserializeObject<menuReturn>(response);
                versionsclientresp = mr.versionresp;
                for (int i = 0; i < versionsclientresp.Count; i++)
                {
                    int apptypeversion = versionsclientresp[i].AppType;
                    string datereleased = versionsclientresp[i].datereleased;
                    string description = versionsclientresp[i].description;
                    int forDowngrade = versionsclientresp[i].forDowngrade;
                    string path = versionsclientresp[i].path;
                    int releasetype = versionsclientresp[i].releasetype;
                    decimal versionno = versionsclientresp[i].versionno;

                    versionresp = saveversionsGlobal(apptypeversion, datereleased, description, forDowngrade, path, releasetype, versionno);

                    if (versionresp.respcode == 0)
                    {
                        return new versionsresponse { respcode = 0, respmsg = versionresp.respmsg };

                    }

                }

                msgclientversions = "\n  Created New Data: Client Version Count: " + versionsclientresp.Count;
            }
        }


        if (updatesresp.respcode == 1 && updatesrespadmin.respcode == 1)
        {

            return new versionsresponse { respcode = 1, respmsg = "Successfully Synch ! : " + msgclientupdates + msgadminupdates + "" };

        }
        else if (updatesresp.respcode == 1)
        {

            return new versionsresponse { respcode = 1, respmsg = "Successfully Synch ! : " + msgclientupdates + "" };

        }
        else if (updatesrespadmin.respcode == 1)
        {

            return new versionsresponse { respcode = 1, respmsg = "Successfully Synch ! : " + msgadminupdates + "" };

        }
        if (versionresp.respcode == 1)
        {

            return new versionsresponse { respcode = 1, respmsg = "Successfully Synch ! : " + msgclientversions };

        }

     
        return new versionsresponse { respcode = 1, respmsg = "Datas are up to date...!" };
    }

   

    #endregion 
    ///---------------------- SAVING DATAS FROM NETWORK TO CLOUD FOR USERLOGINAGENT CLOUD--------------------------------------------------------------------------- ------------------

    #region Saving Cloud
    public response savesysuseraccountsglobal(String resourceID, String UserLogin, String UserPassword, String sysmodified, String BranchCode, String DeptCode, String IsReliever, String TempBranchCode, String ZoneCode, String StartDate, String EndDate, String IsActive, String syscreated, String syscreator, String sysmodifier, String IsResign)
    {
        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
        using (MySqlConnection connet = dbconcloud.getConnection())
        {

            if (sysmodified == string.Empty)
            {
                sysmodified = null;
            }

            if (syscreated == string.Empty)
            {
                syscreated = null;
            }

            connet.Open();
            using (MySqlCommand cmd = connet.CreateCommand())
            {
                String checkdata = "SELECT * from kpusers.sysuseraccounts where ResourceID = '"+resourceID+"'";
                cmd.CommandText = checkdata;
                MySqlDataReader rdr = cmd.ExecuteReader();
                if(rdr.HasRows)
                {
                    rdr.Close();
                    return new response { respcode = 1 , respmsg = "Already Exists!"};
                
                }
                rdr.Close();


                String insertnew = "INSERT INTO kpusers.sysuseraccounts (resourceID,UserLogin,UserPassword,BranchCode,DeptCode,IsReliever,TempBranchCode, " +
                                   "ZoneCode,StartDate,EndDate,IsActive,syscreated,sysmodified,syscreator,sysmodifier,IsResign) VALUES " +
                                    "(@resourceID,@UserLogin,@UserPassword,@BranchCode,@DeptCode,@IsReliever,@TempBranchCode," +
                                     "@ZoneCode,DATE_FORMAT(@StartDate,'%Y-%m-%d %T'),DATE_FORMAT(@EndDate,'%Y-%m-%d %T'),@IsActive, DATE_FORMAT(@syscreated,'%Y-%m-%d %T'), DATE_FORMAT(@sysmodified,'%Y-%m-%d %T'),@syscreator,@sysmodifier,@IsResign);";
                cmd.CommandText = insertnew;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("UserPassword", UserPassword);
                cmd.Parameters.AddWithValue("resourceID", resourceID);
                cmd.Parameters.AddWithValue("syscreated", Convert.ToDateTime(syscreated));
                cmd.Parameters.AddWithValue("sysmodified", sysmodified);
                cmd.Parameters.AddWithValue("UserLogin", UserLogin);
                cmd.Parameters.AddWithValue("BranchCode", BranchCode);
              //  cmd.Parameters.AddWithValue("RoleID", RoleID);
                cmd.Parameters.AddWithValue("DeptCode", DeptCode);
                cmd.Parameters.AddWithValue("IsReliever", IsReliever);
                cmd.Parameters.AddWithValue("TempBranchCode", TempBranchCode);
                //cmd.Parameters.AddWithValue("TempZoneCode", TempZoneCode);
                cmd.Parameters.AddWithValue("ZoneCode", ZoneCode);
                cmd.Parameters.AddWithValue("StartDate", StartDate);
                cmd.Parameters.AddWithValue("EndDate", EndDate);
                cmd.Parameters.AddWithValue("IsActive", IsActive); 
                cmd.Parameters.AddWithValue("syscreator", syscreator);
                cmd.Parameters.AddWithValue("sysmodifier", sysmodifier);
                cmd.Parameters.AddWithValue("IsResign", IsResign);
             //   cmd.Parameters.AddWithValue("CellNo", CellNo);
                //cmd.Parameters.AddWithValue("EmailAddress", EmailAddress);
                int createnew = cmd.ExecuteNonQuery();
                if (createnew < 1)
                {
                    connet.Close();
                    return new response { respcode = 0, respmsg = "Error in updating data." };
                }
                else if (createnew > 0)
                {
                    connet.Close();
                    return new response { respcode = 1, respmsg = "Success." };
                }
            }
            connet.Close();
            return new response { respcode = 0, respmsg = "Error in updating data." };
        }

    }

    // save modified in sysuseraccounts
    public response savemodifiedsysuserglobal(string resourceID, string UserPassword, string sysmodified)
    {
        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
        using (MySqlConnection connet = dbconcloud.getConnection())
        {
            connet.Open();
            using (MySqlCommand cmd = connet.CreateCommand())
            {
                String updatekpusers = "UPDATE kpuserscloud.sysuseraccounts SET userpassword=@pword, sysmodified=@modified where resourceid=@resourceid;";
                cmd.CommandText = updatekpusers;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("pword", UserPassword);
                cmd.Parameters.AddWithValue("resourceID", resourceID);
                cmd.Parameters.AddWithValue("modified", sysmodified);
                int updtusers = cmd.ExecuteNonQuery();
                if (updtusers < 1)
                {
                    connet.Close();
                    return new response { respcode = 0, respmsg = "Error in updating data.", errordetail = updtusers.ToString() };
                }
                else if (updtusers > 0)
                {
                    connet.Close();
                    return new response { respcode = 1, respmsg = "Success." };
                }

            }
            connet.Close();
        }
        return new response { respcode = 0 };
    }

    /// --------------- BRANCHUSERS ---------------------------------
    /// 
    // save newly added in branchusers
    public response savebranchuser(String BranchCode, String resourceID, Int32 IsReliever, Int32 IsActiveReliever, String syscreated, String sysmodified, Int32 ZoneCode, String fullname, String lastname, String firstname, String middlename, String syscreator, String sysmodifier)
    {

        using (MySqlConnection connet = dbconcloud.getConnection())
        {

            if (sysmodified == string.Empty)
            {
                sysmodified = null;
            }

            if (syscreated == string.Empty)
            {
                syscreated = null;
            }
            // Convert.ToDateTime(sysmodified)
            connet.Open();
            using (MySqlCommand cmd = connet.CreateCommand())
            {

                cmd.Parameters.Clear();
                cmd.CommandText = "Select * from kpusers.`branchusers` WHERE ResourceID=@ResourceID;";
                cmd.Parameters.AddWithValue("ResourceID", resourceID);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    connet.Close();
                    rdr.Close();
                    return new response { respcode = 1, respmsg = "Already Exist!" };
                }
                rdr.Close();

                String insertnew = "INSERT INTO kpusers.`branchusers` (`BranchCode`,`ResourceID`,`IsReliever`,`IsActiveReliever`,`syscreated`,`sysmodified`,`ZoneCode`,`fullname`,`lastname`,`firstname`,`middlename`,`syscreator`,`sysmodifier`) " +
                        "VALUES (@BranchCode,@ResourceID,@IsReliever,@IsActiveReliever,DATE_FORMAT(@syscreated,'%Y-%m-%d %T'),DATE_FORMAT(@sysmodified,'%Y-%m-%d %T'),@ZoneCode,@fullname,@lastname,@firstname,@middlename,@syscreator,@sysmodifier);";
                cmd.CommandText = insertnew;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("BranchCode", BranchCode);
                cmd.Parameters.AddWithValue("ResourceID", resourceID);
                cmd.Parameters.AddWithValue("IsReliever", IsReliever);
                cmd.Parameters.AddWithValue("IsActiveReliever", IsActiveReliever);
                cmd.Parameters.AddWithValue("syscreated", Convert.ToDateTime(syscreated));
                cmd.Parameters.AddWithValue("sysmodified", Convert.ToDateTime(sysmodified));
                cmd.Parameters.AddWithValue("ZoneCode", ZoneCode);
                cmd.Parameters.AddWithValue("fullname", fullname);
                cmd.Parameters.AddWithValue("lastname", lastname);
                cmd.Parameters.AddWithValue("firstname", firstname);
                cmd.Parameters.AddWithValue("middlename", middlename);
                cmd.Parameters.AddWithValue("syscreator", syscreator);
                cmd.Parameters.AddWithValue("sysmodifier", sysmodifier);

                int createnew = cmd.ExecuteNonQuery();
                if (createnew < 1)
                {
                    connet.Close();
                    return new response { respcode = 0, respmsg = "Error in updating data." };
                }
                else if (createnew > 0)
                {
                    connet.Close();
                    return new response { respcode = 1, respmsg = "Success." };
                }
            }
            connet.Close();
            return new response { respcode = 0, respmsg = "asdadasd." };
        }

    }

    // save modified in branchusers
    public response savemodifiedbranchuser(String BranchCode, String resourceID, Int32 IsReliever, Int32 IsActiveReliever, String syscreated, String sysmodified, Int32 ZoneCode, String fullname, String lastname, String firstname, String middlename, String syscreator, String sysmodifier)
    {

        using (MySqlConnection connet = dbconcloud.getConnection())
        {
            if (sysmodified == string.Empty)
            {
                sysmodified = null;
            }

            if (syscreated == string.Empty)
            {
                syscreated = null;
            }
            connet.Open();
            using (MySqlCommand cmd = connet.CreateCommand())
            {
                String updatekpusers = "UPDATE kpusers.`branchusers` SET `BranchCode`=@BranchCode,`ResourceID`=@ResourceID,`IsReliever`=@IsReliever,`IsActiveReliever`=@IsActiveReliever,`syscreated`=DATE_FORMAT(@syscreated,'%Y-%m-%d %T'), " +
                                    "`sysmodified`=DATE_FORMAT(@sysmodified,'%Y-%m-%d %T'),`ZoneCode`=@ZoneCode,`fullname`=@fullname,`lastname`=@lastname,`firstname`=@firstname,`middlename`=@middlename,`syscreator`=@syscreator,`sysmodifier`=@sysmodifier WHERE ResourceID=@ResourceID;";
                cmd.CommandText = updatekpusers;
                cmd.Parameters.Clear();

                cmd.Parameters.AddWithValue("BranchCode", BranchCode);
                cmd.Parameters.AddWithValue("ResourceID", resourceID);
                cmd.Parameters.AddWithValue("IsReliever", IsReliever);
                cmd.Parameters.AddWithValue("IsActiveReliever", IsActiveReliever);
                cmd.Parameters.AddWithValue("syscreated", Convert.ToDateTime(syscreated));
                cmd.Parameters.AddWithValue("sysmodified", Convert.ToDateTime(sysmodified));
                cmd.Parameters.AddWithValue("ZoneCode", ZoneCode);
                cmd.Parameters.AddWithValue("fullname", fullname);
                cmd.Parameters.AddWithValue("lastname", lastname);
                cmd.Parameters.AddWithValue("firstname", firstname);
                cmd.Parameters.AddWithValue("middlename", middlename);
                cmd.Parameters.AddWithValue("syscreator", syscreator);
                cmd.Parameters.AddWithValue("sysmodifier", sysmodifier);
                int updtusers = cmd.ExecuteNonQuery();
                if (updtusers < 1)
                {
                    connet.Close();
                    return new response { respcode = 0, respmsg = "Error in updating data.", errordetail = updtusers.ToString() };
                }
                else if (updtusers > 0)
                {
                    connet.Close();
                    return new response { respcode = 1, respmsg = "Success." };
                }

            }
            connet.Close();
        }
        return new response { respcode = 0 };
    }

    /// ------------ MLBRANCHESSTATIONS -----------------
    /// 
    public response savemlbranchesstations(String StationCode, Int32 StationNo, String BranchCode, String syscreated, String syscreator, String sysmodified, String sysmodifier, Int32 ZoneCode, String branchstat, Int32 IsUpdated, String version, Int32 isdowngraded, String adminversion, Int32 releasetype, Int32 isallowupdate, Int32 isallowupdateadmin, String type)
    {
        try
        {

            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
            using (MySqlConnection con = dbconcloud.getConnection())
            {
                try
                {


                    if (sysmodified == string.Empty)
                    {
                        sysmodified = null;
                    }

                    if (syscreated == string.Empty)
                    {
                        syscreated = null;
                    }

                    con.Open();
                    using (MySqlCommand cmd = con.CreateCommand())
                    {

                        cmd.Parameters.Clear();
                        String query = String.Empty;

                        if (type == "Create")
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandText = "Select * from kpusers.`mlbranchesstations` WHERE stationcode=@stationcode;";
                            cmd.Parameters.AddWithValue("stationcode", StationCode);
                            MySqlDataReader rdr = cmd.ExecuteReader();
                            if (rdr.HasRows)
                            {
                                con.Close();
                                rdr.Close();
                                return new response { respcode = 1, respmsg = "Already Exist!" };
                            }
                            rdr.Close();

                            query = "INSERT INTO kpusers.mlbranchesstations (StationCode,StationNo,BranchCode,syscreated,syscreator,sysmodified,sysmodifier,ZoneCode,branchstat,IsUpdated,`version`,isdowngraded,adminversion,releasetype,isallowupdate,isallowupdateadmin) " +
                                               "VALUES (@StationCode,@StationNo,@BranchCode,DATE_FORMAT(@syscreated,'%Y-%m-%d %T'),@syscreator,DATE_FORMAT(@sysmodified,'%Y-%m-%d %T'),@sysmodifier,@ZoneCode,@branchstat,@IsUpdated,@version,@isdowngraded,@adminversion,@releasetype,@isallowupdate,@isallowupdateadmin);";


                        }

                        else if (type == "Modify")
                        {

                            query = "UPDATE kpusers.mlbranchesstations SET StationCode=@StationCode,StationNo=@StationNo,BranchCode=@BranchCode,syscreated=DATE_FORMAT(@sysmodified,'%Y-%m-%d %T'),syscreator=@syscreator,sysmodified=DATE_FORMAT(@sysmodified,'%Y-%m-%d %T'),sysmodifier=@sysmodifier,ZoneCode=@ZoneCode,branchstat=@ZoneCode,IsUpdated=@IsUpdated, " +
                                "`version`=@version,isdowngraded=@isdowngraded,adminversion=@adminversion,releasetype=@releasetype,isallowupdate=@isallowupdate,isallowupdateadmin=@isallowupdateadmin WHERE stationcode=@stationcode;";
                        }
                        cmd.CommandText = query;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("StationCode", StationCode);
                        cmd.Parameters.AddWithValue("StationNo", StationNo);
                        cmd.Parameters.AddWithValue("BranchCode", BranchCode);
                        cmd.Parameters.AddWithValue("syscreated", Convert.ToDateTime(syscreated));
                        cmd.Parameters.AddWithValue("sysmodified", Convert.ToDateTime(sysmodified));
                        cmd.Parameters.AddWithValue("syscreator", syscreator);
                        cmd.Parameters.AddWithValue("sysmodifier", sysmodifier);
                        cmd.Parameters.AddWithValue("ZoneCode", ZoneCode);
                        cmd.Parameters.AddWithValue("branchstat", branchstat);
                        cmd.Parameters.AddWithValue("IsUpdated", IsUpdated);
                        cmd.Parameters.AddWithValue("version", version);
                        cmd.Parameters.AddWithValue("isdowngraded", isdowngraded);
                        cmd.Parameters.AddWithValue("adminversion", adminversion);
                        cmd.Parameters.AddWithValue("releasetype", releasetype);
                        cmd.Parameters.AddWithValue("isallowupdate", isallowupdate);
                        cmd.Parameters.AddWithValue("isallowupdateadmin", isallowupdateadmin);

                        int createnew = cmd.ExecuteNonQuery();
                        if (createnew < 1)
                        {
                            con.Close();
                            return new response { respcode = 0, respmsg = "Error in updating/inserting data." };
                        }
                        else if (createnew > 0)
                        {
                            con.Close();
                            return new response { respcode = 1, respmsg = "Success." };
                        }
                    }
                    con.Close();
                    return new response { respcode = 0, respmsg = "asdasdsada" };
                }
                catch (Exception ex)
                {
                    return new response { respcode = 0, respmsg = "Error: " + ex.ToString() };
                }

             }
          
        }
        catch (Exception ex)
        {

            return new response { respcode = 0, respmsg = "System Error! " + ex.ToString()};
        }

        }

     //---------------------ADMINUSERS-----------------
    public response saveadminusers(Int32 ResourceID, String DeptCode, String Fullname, String Firstname, String Lastname, String Middlename, Int32 ZoneCode, String syscreator, String syscreated, String sysmodifier, String sysmodified, String type)
    {
    try
    {

        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
        using (MySqlConnection con = dbconcloud.getConnection())
        {

                if (sysmodified == string.Empty)
                {
                    sysmodified = null;
                }

                if (syscreated == string.Empty)
                {
                    syscreated = null;
                }

                con.Open();
                using (MySqlCommand cmd = con.CreateCommand())
                {

                    try

                    {

                        cmd.Parameters.Clear();
                        String query = String.Empty;

                        if (type == "Create")
                        {

                            cmd.Parameters.Clear();
                            cmd.CommandText = "Select * from kpusers.adminusers where ResourceID = @ResourceID;";
                            cmd.Parameters.AddWithValue("ResourceID", ResourceID);
                            MySqlDataReader rdr = cmd.ExecuteReader();
                            if (rdr.HasRows)
                            {
                   
                                rdr.Close();
                                query = "UPDATE kpusers.`adminusers` SET Fullname=@Fullname,Firstname=@Firstname,Lastname=@Lastname,Middlename=@Middlename,ZoneCode=@ZoneCode,syscreated=DATE_FORMAT(@syscreated,'%Y-%m-%d %T'),syscreator=@syscreator,sysmodifier=@sysmodifier" +
                                " WHERE ResourceID=@ResourceID;";
                   
                            }                        
                            else
                            {
                                rdr.Close();
                                query = "INSERT INTO kpusers.adminusers (ResourceID,DeptCode,FullName,Firstname,Lastname,Middlename,ZoneCode,syscreated,syscreator,sysmodified,sysmodifier) " +
                                               "VALUES (@ResourceID,@DeptCode,@FullName,@Firstname,@Lastname,@Middlename,@ZoneCode,DATE_FORMAT(@syscreated,'%Y-%m-%d %T'),@syscreator,DATE_FORMAT(@sysmodified,'%Y-%m-%d %T'),@sysmodifier);";

                            }
                        }

                       if (type == "Modify")
                        {

                            query = "UPDATE kpusers.`adminusers` SET Fullname=@Fullname,Firstname=@Firstname,Lastname=@Lastname,Middlename=@Middlename,ZoneCode=@ZoneCode,syscreated=DATE_FORMAT(@syscreated,'%Y-%m-%d %T'),syscreator=@syscreator,sysmodified=DATE_FORMAT(@sysmodified,'%Y-%m-%d %T'),sysmodifier=@sysmodifier " +
                                "WHERE ResourceID=@ResourceID;";
                        }
                        cmd.CommandText = query;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("ResourceID", ResourceID);
                        cmd.Parameters.AddWithValue("DeptCode", DeptCode);
                        cmd.Parameters.AddWithValue("Fullname", Fullname);
                        cmd.Parameters.AddWithValue("Firstname", Firstname);
                        cmd.Parameters.AddWithValue("Lastname", Lastname);
                        cmd.Parameters.AddWithValue("Middlename", Middlename);
                        cmd.Parameters.AddWithValue("ZoneCode", ZoneCode);
                        cmd.Parameters.AddWithValue("syscreated", syscreated);
                        cmd.Parameters.AddWithValue("sysmodified", sysmodified);
                        cmd.Parameters.AddWithValue("syscreator", syscreator);
                        cmd.Parameters.AddWithValue("sysmodifier", sysmodifier);


                        int createnew = cmd.ExecuteNonQuery();
                        if (createnew < 1)
                        {
                            con.Close();
                            return new response { respcode = 0, respmsg = "Error in updating adminusers"};
                        }
                        else if (createnew > 0)
                        {
                            con.Close();
                            return new response { respcode = 1, respmsg = "Success." };
                        }
                    }
                        catch (Exception ex)
                    {

                        con.Close();
                        return new response { respcode = 0, respmsg = ex.ToString() };
                    }
                  }
                    con.Close();
                    return new response { respcode = 0, respmsg = "Error in updating data." };

            }

        }
        catch (Exception ex)
        {

            return new response { respcode = 0, respmsg = "System Error! " + ex.ToString() };
        }

    }
 
    //-----------------AREA-------------------------------

    public response savearea(String AreaName, String AreaCode, Int32 RegionCode, Int32 ZoneCode, String syscreated, String syscreator, String sysmodified, String sysmodifier, String type)
    {
        try
        {

            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
            using (MySqlConnection con = dbconcloud.getConnection())
            {
                try
                {


                    if (sysmodified == string.Empty)
                    {
                        sysmodified = null;
                    }

                    if (syscreated == string.Empty)
                    {
                        syscreated = null;
                    }

                    con.Open();
                    using (MySqlCommand cmd = con.CreateCommand())
                    {

                        cmd.Parameters.Clear();
                        String query = String.Empty;

                        if (type == "Create")
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandText = "Select * from kpusers.area where AreaCode = @AreaCode AND RegionCode = @RegionCode ANd ZoneCode = @ZoneCode;";
                            cmd.Parameters.AddWithValue("AreaCode", AreaCode);
                            cmd.Parameters.AddWithValue("RegionCode", RegionCode);
                            cmd.Parameters.AddWithValue("ZoneCode", ZoneCode);
                            MySqlDataReader rdr = cmd.ExecuteReader();
                            if (rdr.HasRows)
                            {
                                con.Close();
                                rdr.Close();
                                return new response { respcode = 1, respmsg = "Already Exist!" };
                            }
                            rdr.Close();

                            query = "INSERT INTO kpusers.area (AreaName,AreaCode,RegionCode,ZoneCode,syscreated,syscreator,sysmodified,sysmodifier) " +
                                               "VALUES (@AreaName,@AreaCode,@RegionCode,@ZoneCode,DATE_FORMAT(@syscreated,'%Y-%m-%d %T'),@syscreator,DATE_FORMAT(@sysmodified,'%Y-%m-%d %T'),@sysmodifier);";


                        }

                        else if (type == "Modify")
                        {

                            query = "UPDATE kpusers.area SET AreaName=@AreaName,AreaCode=@AreaCode,RegionCode=@RegionCode,ZoneCode=@ZoneCode,syscreated=DATE_FORMAT(@syscreated,'%Y-%m-%d %T'),syscreator=@syscreator,sysmodified=DATE_FORMAT(@sysmodified,'%Y-%m-%d %T'),sysmodifier=@sysmodifier " +
                                "WHERE AreaCode=@AreaCode AND RegionCode=@RegionCode AND ZoneCode=@ZoneCode;";
                        }
                        cmd.CommandText = query;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("AreaName", AreaName);
                        cmd.Parameters.AddWithValue("AreaCode", AreaCode);
                        cmd.Parameters.AddWithValue("RegionCode", RegionCode);
                        cmd.Parameters.AddWithValue("ZoneCode", ZoneCode);
                        cmd.Parameters.AddWithValue("syscreated",syscreated);
                        cmd.Parameters.AddWithValue("sysmodified", sysmodified);
                        cmd.Parameters.AddWithValue("syscreator", syscreator);
                        cmd.Parameters.AddWithValue("sysmodifier", sysmodifier);

                        int createnew = cmd.ExecuteNonQuery();
                        if (createnew < 1)
                        {
                            con.Close();
                            return new response { respcode = 0, respmsg = "Error in updating/inserting data." };
                        }
                        else if (createnew > 0)
                        {
                            con.Close();
                            return new response { respcode = 1, respmsg = "Success." };
                        }
                    }
                    con.Close();
                    return new response { respcode = 0, respmsg = "Error in updating data." };
                }
                catch (Exception ex)
                {
                    return new response { respcode = 0, respmsg = "Error: " + ex.ToString() };
                }

            }

        }
        catch (Exception ex)
        {

            return new response { respcode = 0, respmsg = "System Error! " + ex.ToString() };
        }

    }

    //--------------------------------Branches-----------------------------

    public response savebranches(Int32 ZoneCode, String AreaCode, String BranchCode, String BranchName, String Address, String TINNo, Int32 Status, String ClosedDate, String CorpName, Decimal VAT, String TelNo, String CellNo, String syscreator, String syscreated, String sysmodifier, String sysmodified, String PermitNo, String KP4CODE, String FaxNo, String Regioncode, Int32 is24Hours, Int32 allowBeyondOffHour, String timeFrom, String timeTo, String type)
    {
        try
        {

            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
            using (MySqlConnection con = dbconcloud.getConnection())
            {
                try
                {


                    if (sysmodified == string.Empty)
                    {
                        sysmodified = null;
                    }

                    if (syscreated == string.Empty)
                    {
                        syscreated = null;
                    }
                    if(ClosedDate == string.Empty)
                    {
                        ClosedDate = null;
                    }

                    con.Open();
                    using (MySqlCommand cmd = con.CreateCommand())
                    {

                        cmd.Parameters.Clear();
                        String query = String.Empty;

                        if (type == "Create")
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandText = "Select * from kpusers.branches WHERE (ZoneCode=@ZoneCode AND AreaCode=@AreaCode AND BranchCode=@BranchCode) OR (AreaCode=@AreaCode AND BranchCode=@BranchCode AND BranchName=@BranchName);";
                            cmd.Parameters.AddWithValue("AreaCode", AreaCode);
                            cmd.Parameters.AddWithValue("BranchCode", BranchCode);
                            cmd.Parameters.AddWithValue("ZoneCode", ZoneCode);
                            cmd.Parameters.AddWithValue("BranchName", BranchName);
                            MySqlDataReader rdr = cmd.ExecuteReader();
                            if (rdr.HasRows)
                            {
                                con.Close();
                                rdr.Close();
                                return new response { respcode = 1, respmsg = "Already Exist!" };
                            }
                            rdr.Close();

                            query = "INSERT INTO kpusers.branches (ZoneCode,AreaCode,BranchCode,BranchName,Address,TINNo,Status,ClosedDate,CorpName,VAT,TelNo,CellNo,syscreated,syscreator,sysmodified,sysmodifier,PermitNo,KP4CODE,FaxNo,Regioncode,is24Hours,allowBeyondOffHour,timeFrom,timeTo) " +
                                               "VALUES (@ZoneCode,@AreaCode,@BranchCode,@BranchName,@Address,@TINNo,@Status,DATE_FORMAT(@ClosedDate,'%Y-%m-%d %T'),@CorpName,@VAT,@TelNo,@CellNo,DATE_FORMAT(@syscreated,'%Y-%m-%d %T'),@syscreator,DATE_FORMAT(@sysmodified,'%Y-%m-%d %T'),@sysmodifier,@PermitNo,@KP4CODE,@FaxNo,@Regioncode,@is24Hours,@allowBeyondOffHour,@timeFrom,@timeTo);";


                        }

                        else if (type == "Modify")
                        {

                            query = "UPDATE kpusers.branches SET ZoneCode=@ZoneCode,AreaCode=@AreaCode,BranchCode=@BranchCode,BranchName=@BranchName,Address=@Address,TINNo=@TINNo,`Status`=@`Status`,ClosedDate=DATE_FORMAT(@ClosedDate,'%Y-%m-%d %T'),CorpName=@CorpName,VAT=@VAT,TelNo=@TelNo,CellNo=@CellNo"
                                + ",syscreated=DATE_FORMAT(@syscreated,'%Y-%m-%d %T'),syscreator=@syscreator,sysmodified=DATE_FORMAT(@sysmodified,'%Y-%m-%d %T'),sysmodifier=@sysmodifier,PermitNo=@PermitNo,KP4CODE=@KP4CODE,FaxNo=@FaxNo,Regioncode=@Regioncode,is24Hours=@is24Hours,allowBeyondOffHour=@allowBeyondOffHour,timeFrom=@timeFrom,timeTo=@timeTo " +
                                "WHERE (ZoneCode=@ZoneCode AND AreaCode=@AreaCode AND BranchCode=@BranchCode) OR (AreaCode=@AreaCode AND BranchCode=@BranchCode AND BranchName=@BranchName);";
                        }
                        cmd.CommandText = query;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("ZoneCode", ZoneCode);
                        cmd.Parameters.AddWithValue("AreaCode", AreaCode);
                        cmd.Parameters.AddWithValue("BranchCode", BranchCode);
                        cmd.Parameters.AddWithValue("BranchName", BranchName);

                        cmd.Parameters.AddWithValue("Address", Address);
                        cmd.Parameters.AddWithValue("TINNo", TINNo);
                        cmd.Parameters.AddWithValue("Status", Status);
                        cmd.Parameters.AddWithValue("ClosedDate", Convert.ToDateTime(ClosedDate));

                        cmd.Parameters.AddWithValue("CorpName", CorpName);
                        cmd.Parameters.AddWithValue("VAT", VAT);
                        cmd.Parameters.AddWithValue("TelNo", TelNo);
                        cmd.Parameters.AddWithValue("CellNo", CellNo);


                        cmd.Parameters.AddWithValue("syscreated", syscreated);
                        cmd.Parameters.AddWithValue("sysmodified", sysmodified);
                        cmd.Parameters.AddWithValue("syscreator", syscreator);
                        cmd.Parameters.AddWithValue("sysmodifier", sysmodifier);

                        cmd.Parameters.AddWithValue("PermitNo", PermitNo);
                        cmd.Parameters.AddWithValue("KP4CODE", KP4CODE);
                        cmd.Parameters.AddWithValue("FaxNo", FaxNo);
                        cmd.Parameters.AddWithValue("Regioncode", Regioncode);

                        cmd.Parameters.AddWithValue("is24Hours", is24Hours);
                        cmd.Parameters.AddWithValue("allowBeyondOffHour", allowBeyondOffHour);
                        cmd.Parameters.AddWithValue("timeFrom", timeFrom);
                        cmd.Parameters.AddWithValue("timeTo", timeTo);



                        int createnew = cmd.ExecuteNonQuery();
                        if (createnew < 1)
                        {
                            con.Close();
                            return new response { respcode = 0, respmsg = "Error in updating/inserting data." };
                        }
                        else if (createnew > 0)
                        {
                            con.Close();
                            return new response { respcode = 1, respmsg = "Success." };
                        }
                    }
                    con.Close();
                    return new response { respcode = 0, respmsg = "asdsadsad" };
                }
                catch (Exception ex)
                {
                    return new response { respcode = 0, respmsg = "Error: " + ex.ToString() };
                }

            }

        }
        catch (Exception ex)
        {

            return new response { respcode = 0, respmsg = "System Error! " + ex.ToString() };
        }

    }

    //----------------REGION---------------------

    public response saveregion(Int32 RegionCode, String RegionName, Int32 ZoneCode,  String syscreator, String syscreated, String sysmodified, String sysmodifier, String type)
    {
        try
        {

            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
            using (MySqlConnection con = dbconcloud.getConnection())
            {
                try
                {


                    if (sysmodified == string.Empty)
                    {
                        sysmodified = null;
                    }

                    if (syscreated == string.Empty)
                    {
                        syscreated = null;
                    }
             

                    con.Open();
                    using (MySqlCommand cmd = con.CreateCommand())
                    {

                        cmd.Parameters.Clear();
                        String query = String.Empty;

                        if (type == "Create")
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandText = "Select * from kpusers.`region` WHERE (ZoneCode=@ZoneCode AND RegionCode=@RegionCode) OR (RegionCode=@RegionCode AND RegionName=@RegionName);";
                            cmd.Parameters.AddWithValue("ZoneCode", ZoneCode);
                            cmd.Parameters.AddWithValue("RegionCode", RegionCode);
                            cmd.Parameters.AddWithValue("RegionName", RegionName);
                            MySqlDataReader rdr = cmd.ExecuteReader();
                            if (rdr.HasRows)
                            {
                                con.Close();
                                rdr.Close();
                                return new response { respcode = 1, respmsg = "Already Exist!" };
                            }
                            rdr.Close();

                            query = "INSERT INTO kpusers.region (RegionCode,RegionName,ZoneCode,syscreated,syscreator,sysmodified,sysmodifier) " +
                                               "VALUES (@RegionCode,@RegionName,@ZoneCode,DATE_FORMAT(@syscreated,'%Y-%m-%d %T'),@syscreator,DATE_FORMAT(@sysmodified,'%Y-%m-%d %T'),@sysmodifier);";


                        }

                        else if (type == "Modify")
                        {

                            query = "UPDATE kpusers.region SET RegionCode=@RegionCode,RegionName=@RegionName,ZoneCode=@ZoneCode"
                                + ",syscreated=DATE_FORMAT(@syscreated,'%Y-%m-%d %T'),syscreator=@syscreator,sysmodified=DATE_FORMAT(@sysmodified,'%Y-%m-%d %T'),sysmodifier=@sysmodifier " +
                                "WHERE ZoneCode=@ZoneCode AND RegionCode=@RegionCode;";
                        }
                        cmd.CommandText = query;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("ZoneCode", ZoneCode);
                        cmd.Parameters.AddWithValue("RegionCode", RegionCode);
                        cmd.Parameters.AddWithValue("RegionName", RegionName);
                        cmd.Parameters.AddWithValue("syscreated", Convert.ToDateTime(syscreated));
                        cmd.Parameters.AddWithValue("sysmodified", sysmodified);
                        cmd.Parameters.AddWithValue("syscreator", syscreator);
                        cmd.Parameters.AddWithValue("sysmodifier", sysmodifier);
                      

  
                        int createnew = cmd.ExecuteNonQuery();
                        if (createnew < 1)
                        {
                            con.Close();
                            return new response { respcode = 0, respmsg = "Error in updating/inserting data." };
                        }
                        else if (createnew > 0)
                        {
                            con.Close();
                            return new response { respcode = 1, respmsg = "Success." };
                        }
                    }
                    con.Close();
                    return new response { respcode = 0, respmsg = "Error in updating data." };
                }
                catch (Exception ex)
                {
                    return new response { respcode = 0, respmsg = "Error: " + ex.ToString() };
                }

            }

        }
        catch (Exception ex)
        {

            return new response { respcode = 0, respmsg = "System Error! " + ex.ToString() };
        }

    }


    //--------------------------------SYSUSERROLES-----------------

    public response savesysuserroles(Int32 ResourceID, Int32 Type, String Role, Int32 ZoneCode, String syscreator, String syscreated, String sysmodified, String sysmodifier, String agenttype)
    {
        try
        {

            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
            using (MySqlConnection con = dbconcloud.getConnection())
            {
                try
                {


                    if (sysmodified == string.Empty)
                    {
                        sysmodified = null;
                    }

                    if (syscreated == string.Empty)
                    {
                        syscreated = null;
                    }


                    con.Open();
                    using (MySqlCommand cmd = con.CreateCommand())
                    {

           
                        String query = String.Empty;

                        if (agenttype == "Create")
                        {
                            cmd.Parameters.Clear();
                            cmd.CommandText = "Select * from kpusers.`sysuserroles` WHERE ResourceID='" + ResourceID + "';";

                            MySqlDataReader rdr = cmd.ExecuteReader();
                            if (rdr.HasRows)
                            {
                                con.Close();
                                rdr.Close();
                                return new response { respcode = 1, respmsg = "Already Exist!" };
                            }
                            rdr.Close();

                            query = "INSERT INTO kpusers.sysuserroles (ResourceID,ZoneCode,`Type`,Role,syscreated,syscreator,sysmodified,sysmodifier) VALUES ('"+ResourceID+"','"+ZoneCode+"','"+Type+"','"+Role+"',DATE_FORMAT('"+syscreated+"','%Y-%m-%d %T'),'"+syscreator+"',DATE_FORMAT('"+sysmodified+"','%Y-%m-%d %T'),'"+sysmodifier+"');";


                        }

                        else if (agenttype == "Modify")
                        {

                            query = "UPDATE kpusers.sysuserroles SET ResourceID='"+ResourceID+"',Type='"+Type+"',Role='"+Role+"',ZoneCode='"+ZoneCode+"'"
                                + ",syscreated=DATE_FORMAT('"+syscreated+"','%Y-%m-%d %T'),syscreator='"+syscreator+"',sysmodified=DATE_FORMAT('"+sysmodified+"','%Y-%m-%d %T'),sysmodifier='"+sysmodifier+"' " +
                                "WHERE ResourceID='"+ResourceID+"';";
                        }

                        cmd.Parameters.Clear();
                        cmd.CommandText = query;
                     
                        int createnew = cmd.ExecuteNonQuery();
                        if (createnew < 1)
                        {
                            con.Close();
                            return new response { respcode = 0, respmsg = "Error in updating/inserting data." };
                        }
                        else if (createnew > 0)
                        {
                            con.Close();
                            return new response { respcode = 1, respmsg = "Success." };
                        }
                    }
                    con.Close();
                    return new response { respcode = 0, respmsg = "Error in updating data." };
                }
                catch (Exception ex)
                {
                    return new response { respcode = 0, respmsg = "Error: " + ex.ToString() };
                }

            }

        }
        catch (Exception ex)
        {

            return new response { respcode = 0, respmsg = "System Error! " + ex.ToString() };
        }

    }

    //-------------------ROLES-------------------------

    public response saveroles(String Role, String Description, Int32 RoleType, String syscreator, String syscreated, String sysmodified, String sysmodifier, String type)
    {
        try
        {

            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
            using (MySqlConnection con = dbconcloud.getConnection())
            {

                try
                {


                    if (sysmodified == string.Empty)
                    {
                        sysmodified = null;
                    }

                    if (syscreated == string.Empty)
                    {
                        syscreated = null;
                    }


                    con.Open();
                    using (MySqlCommand cmd = con.CreateCommand())
                    {

                        cmd.Parameters.Clear();
                        String query = String.Empty;

                        if (type == "Create")
                        {

                            cmd.Parameters.Clear();
                            cmd.CommandText = "Select * from kpusers.`roles` WHERE role=@role and roletype=@roletype;";
                            cmd.Parameters.AddWithValue("role", Role);
                            cmd.Parameters.AddWithValue("roletype", RoleType);

                            MySqlDataReader rdr = cmd.ExecuteReader();
                            if (rdr.HasRows)
                            {
                                con.Close();
                                rdr.Close();
                                return new response { respcode = 1, respmsg = "Already Exist!" };
                            }
                            rdr.Close();

                            query = "insert into kpusers.roles(role,description,roletype,syscreated,syscreator) values (@role,@description,@roletype,@syscreated,@syscreator);";


                        }

                        else if (type == "Modify")
                        {

                            query = "update kpusers.roles set description=@description, sysmodified=@sysmodified where role=@role and roletype=@roletype;";
                        }
                        cmd.CommandText = query;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("description", Description);
                        cmd.Parameters.AddWithValue("role", Role);
                        cmd.Parameters.AddWithValue("roletype", RoleType);
                        cmd.Parameters.AddWithValue("syscreated", syscreated);
                        cmd.Parameters.AddWithValue("sysmodified", sysmodified);
                        cmd.Parameters.AddWithValue("syscreator", syscreator);
                        cmd.Parameters.AddWithValue("sysmodifier", sysmodifier);



                        int createnew = cmd.ExecuteNonQuery();
                        if (createnew < 1)
                        {
                            con.Close();
                            return new response { respcode = 0, respmsg = "Error in updating/inserting data." };
                        }
                        else if (createnew > 0)
                        {
                            con.Close();
                            return new response { respcode = 1, respmsg = "Success." };
                        }
                    }
                    con.Close();
                    return new response { respcode = 0, respmsg = "Error in updating data." };
                }
                catch (Exception ex)
                {
                    return new response { respcode = 0, respmsg = "Error: " + ex.ToString() };
                }

            }

        }
        catch (Exception ex)
        {

            return new response { respcode = 0, respmsg = "System Error! " + ex.ToString() };
        }

    }

    public response saverr(string ric , string ryan) 
    {

        return new response { respcode = 0 , respmsg = ric + ryan};

    }

    // --------------ZoneCodes --------------

    public response savezonecodes(Int32 ZoneCode, String ZoneName, String syscreator, String syscreated, String sysmodifier, String sysmodified, String agenttype)
    {
        try
        {

            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
            using (MySqlConnection con = dbconcloud.getConnection())
            {
                try
                {


                    if (sysmodified == string.Empty)
                    {
                        sysmodified = null;
                    }

                    if (syscreated == string.Empty)
                    {
                        syscreated = null;
                    }

                    con.Open();
                    using (MySqlCommand cmd = con.CreateCommand())
                    {

                        cmd.Parameters.Clear();
                        String query = String.Empty;

                if(agenttype == "Create")
                {

                        cmd.Parameters.Clear();
                        cmd.CommandText = "Select * from kpusers.`zonecodes` WHERE zonecode = @zonecode;";
                        cmd.Parameters.AddWithValue("zonecode", ZoneCode);

                        MySqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            con.Close();
                            rdr.Close();
                            return new response { respcode = 1, respmsg = "Already Exist!" };
                        }
                        rdr.Close();


                        query = "INSERT INTO kpusers.`zonecodes` (ZoneCode,ZoneName,syscreated,sysmodified,syscreator,sysmodifier) " +
                                         "VALUES (@ZoneCode,@ZoneName,DATE_FORMAT(@syscreated,'%Y-%m-%d %T'),DATE_FORMAT(@sysmodified,'%Y-%m-%d %T'),@syscreator,@sysmodifier);";

                    }

                else if(agenttype == "Modify")
                {


                    query = "UPDATE kpusers.`zonecodes` SET ZoneCode=@ZoneCode,ZoneName=@ZoneName,syscreated= DATE_FORMAT(@syscreated,'%Y-%m-%d %T'),sysmodified = DATE_FORMAT(@sysmodified,'%Y-%m-%d %T'),syscreator = @syscreator,sysmodifier = @sysmodifier " +
                                     "WHERE ZoneCode = @ZoneCode;";
            
                
                }
                      
                        cmd.CommandText = query;
                        cmd.Parameters.Clear();
            
                        cmd.Parameters.AddWithValue("ZoneCode", ZoneCode);
                        cmd.Parameters.AddWithValue("ZoneName", ZoneName);
                        cmd.Parameters.AddWithValue("syscreated", Convert.ToDateTime(syscreated));
                        cmd.Parameters.AddWithValue("sysmodified", Convert.ToDateTime(sysmodified));
                        cmd.Parameters.AddWithValue("syscreator", syscreator);
                        cmd.Parameters.AddWithValue("sysmodifier", sysmodifier);

                        int createnew = cmd.ExecuteNonQuery();
                        if (createnew < 1)
                        {
                            con.Close();
                            return new response { respcode = 0, respmsg = "Error in inserting data." };
                        }
                        else if (createnew > 0)
                        {
                            con.Close();
                            return new response { respcode = 1, respmsg = "Success." };
                        }
                    }
                    con.Close();
                    return new response { respcode = 0, respmsg = "Error in inserting data." };
                }
                catch (Exception ex)
                {
                    return new response { respcode = 0, respmsg = "Error: " + ex.ToString() };
                }

            }

        }
        catch (Exception ex)
        {

            return new response { respcode = 0, respmsg = "System Error! " + ex.ToString() };
        }

    }

    private response saveversionsGlobal(Int32 apptypeversion, String datereleased, String description, Int32 forDowngrade, String path, Int32 releasetype, Decimal versionno)
    {
        using (MySqlConnection con = dbconcloud.getConnection())
        {
            con.Open();
            using (MySqlCommand cmd = con.CreateCommand())
            {
          
                cmd.CommandText = "Select * from kpformsglobal.versions where versionno = @versionno and apptype=@apptype and datereleased = @datereleased;";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("apptype", apptypeversion);
                cmd.Parameters.AddWithValue("datereleased", Convert.ToDateTime(datereleased));
                cmd.Parameters.AddWithValue("versionno", versionno);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {

                    rdr.Close();
                    con.Clone();

                    return new response { respcode = 1, respmsg = "Success!" };
                }
                else
                {
                    rdr.Close();
                    cmd.Parameters.Clear();
                    cmd.CommandText = "INSERT INTO kpformsglobal.versions (apptype,datereleased,description,forDowngrade,path,releasetype,versionno) " +
                                   "VALUES (@apptype,@datereleased,@description,@forDowngrade,@path,@releasetype,@versionno);";
                   
                    cmd.Parameters.AddWithValue("apptype", apptypeversion);
                    cmd.Parameters.AddWithValue("datereleased", Convert.ToDateTime(datereleased));
                    cmd.Parameters.AddWithValue("versionno", versionno);
                    cmd.Parameters.AddWithValue("description", description);
                    cmd.Parameters.AddWithValue("forDowngrade", forDowngrade);
                    cmd.Parameters.AddWithValue("path", path);
                    cmd.Parameters.AddWithValue("releasetype", releasetype);
                    int x = cmd.ExecuteNonQuery();
                    if (x < 1)
                    {
                        con.Close();
                        return new response { respcode = 0, respmsg = "Error  inserting into versions" };


                    }
                    else
                    {
                        con.Close();
                        return new response { respcode = 1, respmsg = "Success!" };

                    }


                }


            }

        }

    }

    private response saveupdatesGlobal(String apptype, String filename, String crc, Decimal version)
    {

        using (MySqlConnection con = dbconcloud.getConnection())
        {
            con.Open();
            using (MySqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "Select * from kpformsglobal.updates where crc = @crc and apptype=@apptype and version = @version;";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("apptype", apptype);
                cmd.Parameters.AddWithValue("filename", filename);
                cmd.Parameters.AddWithValue("crc", crc);
                cmd.Parameters.AddWithValue("version", version);
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {

                    rdr.Close();
                    con.Clone();

                    return new response { respcode = 1, respmsg = "Success!" };
                }
                else
                {
                    rdr.Close();
                    cmd.CommandText = "INSERT INTO kpformsglobal.updates (apptype,filename,crc,version) " +
                                   "VALUES (@apptype,@filename,@crc,@version);";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("apptype", apptype);
                    cmd.Parameters.AddWithValue("filename", filename);
                    cmd.Parameters.AddWithValue("crc", crc);
                    cmd.Parameters.AddWithValue("version", version);
                    int x = cmd.ExecuteNonQuery();
                    if (x < 1)
                    {
                        con.Close();
                        return new response { respcode = 0, respmsg = "Error  inserting into updates" };


                    }
                    else
                    {
                        con.Close();
                        return new response { respcode = 1, respmsg = "Success!" };

                    }


                }


            }
            con.Close();
        }

        return new response { respcode = 1, respmsg = "Success!" };
    }

    #endregion


    #region GettingData for Cloud Agent
    //---------------------------------- GETTING DATA IN NETWORK! FOR USERLOGINAGENT IN CLOUD  -----------------------------------------------------------------------------

    public versionsresponse getversion()
    {
        double sysmenuversion = 0.0;
        double sysmenuadminversion = 0.0;
        double clientupdatesversion = 0.0;
        double adminupdatesversion = 0.0;
        double clientversion = 0.0;
        double adminversion = 0.0;
        String maxreleased = string.Empty;
        Int32 syspluginregistrycnt = 0;
        Int32 sysassemblycnt = 0;
        MySqlDataReader rdr;
        using (MySqlConnection connet = dbconcloud.getConnection())
        {
            connet.Open();
            using (MySqlCommand cmd = connet.CreateCommand())
            {
                String getclientsysmenuversion = "SELECT MAX(`version`) as clientmaxversion FROM kpformsglobal.sysmenu WHERE `type`=0;";
                cmd.CommandText = getclientsysmenuversion;
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    sysmenuversion = Convert.ToDouble(rdr["clientmaxversion"]);
                    rdr.Close();
                }
                rdr.Close();

                String getadminsysmenuversion = "SELECT MAX(`version`) as adminversion FROM kpformsglobal.sysmenu WHERE `type`=1;";
                cmd.CommandText = getadminsysmenuversion;
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    sysmenuadminversion = Convert.ToDouble(rdr["adminversion"]);
                    rdr.Close();
                }
                rdr.Close();

                String getclientupdates = "SELECT MAX(`version`) as clientupdatesversion FROM kpformsglobal.updates WHERE apptype = 'Client';";
                cmd.CommandText = getclientupdates;
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    clientupdatesversion = Convert.ToDouble(rdr["clientupdatesversion"]);
                    rdr.Close();
                }
                rdr.Close();


                String getadminupdates = "SELECT MAX(`version`) as adminupdatesversion FROM kpformsglobal.updates WHERE apptype = 'Admin';";
                cmd.CommandText = getadminupdates;
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    adminupdatesversion = Convert.ToDouble(rdr["adminupdatesversion"]);
                    rdr.Close();
                }
                rdr.Close();

                String getclientversion = "SELECT MAX(`versionno`) as clientversion FROM kpformsglobal.versions WHERE apptype = 0;";
                cmd.CommandText = getclientversion;
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    clientversion = Convert.ToDouble(rdr["clientversion"]);
                    rdr.Close();
                }
                rdr.Close();

                String getadminversion = "SELECT MAX(`versionno`) as adminversion FROM kpformsglobal.versions WHERE apptype = 1;";
                cmd.CommandText = getadminversion;
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    adminversion = Convert.ToDouble(rdr["adminversion"]);
                    rdr.Close();
                }
                rdr.Close();

                cmd.CommandText = "SELECT COUNT(*) as count FROM kpformsglobal.syspluginregistry;";
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    syspluginregistrycnt = Convert.ToInt32(rdr["count"]);
                    rdr.Close();
                }
                rdr.Close();

                cmd.CommandText = "SELECT COUNT(*) as count FROM kpformsglobal.sysassembly;";
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    sysassemblycnt = Convert.ToInt32(rdr["count"]);
                    rdr.Close();
                }
                rdr.Close();

                cmd.CommandText = "Select DATE_FORMAT(MAX(datereleased),'%Y-%m-%d %T') as maxreleased FROM `kpformsglobal`.`versions`;";
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    maxreleased = rdr["maxreleased"].ToString();
                    rdr.Close();
                }
                rdr.Close();

                return new versionsresponse { respcode = 1 ,  clientsysmenu = sysmenuversion, adminsysmenu = sysmenuadminversion, updatesclient = clientupdatesversion, updatesadmin = adminupdatesversion, versionclient = clientversion, versionadmin = adminversion, syspluginregistrycount = syspluginregistrycnt, sysassemblycount = sysassemblycnt, datereleased = maxreleased };
            }

           // connet.Close();

        }
        return new versionsresponse { respcode = 0, respmsg = "" };
    }

    public response getdatasysuseraccounts(String modified, String created)
    {
        try
        {
            String maxmodified = string.Empty;
            String maxcreated = string.Empty;
            String resourceID = string.Empty;
            String UserLogin = string.Empty;
            String UserPassword = string.Empty;
            String sysmodified = string.Empty;
            String BranchCode = string.Empty;
            String RoleID = string.Empty;
            String DeptCode = string.Empty;
            String IsReliever = string.Empty;
            String TempBranchCode = string.Empty;
            String TempZoneCode = string.Empty;
            String ZoneCode = string.Empty;
            String StartDate = string.Empty;
            String EndDate = string.Empty;
            String IsActive = string.Empty;
            String syscreated = string.Empty;
            String syscreator = string.Empty;
            String sysmodifier = string.Empty;
            String IsResign = string.Empty;
            String CellNo = string.Empty;
            String EmailAddress = string.Empty;
            String endpoint;
            String info;
            DateTime sysmod;
            Byte[] dataresp;
            dynamic resultparse = 0;
            Int32 resultcodecreate = 0;
            Int32 resultcodemod = 0;

            using (MySqlConnection con = dbconcloud.getConnection())
            {
                con.Open();
                using (MySqlCommand comm = con.CreateCommand())
                {
                    comm.CommandText = "Select DATE_FORMAT(MAX(syscreated),'%Y-%m-%d %T') as maxcreated, DATE_FORMAT(MAX(sysmodified),'%Y-%m-%d %T') as maxmodified FROM `kpusers`.`sysuseraccounts`;";
                    MySqlDataReader rdr = comm.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        rdr.Read();
                        maxmodified = rdr["maxmodified"].ToString();
                        maxcreated = rdr["maxcreated"].ToString();

                        rdr.Close();
                        con.Close();

                    }
                    if (maxcreated == created && maxmodified == modified)
                    {

                        return new response { respcode = 1, respmsg = "Datas are already synch!" };
                    }


                    // sysuser accounts newly created 

                    comm.CommandText = "Select *, DATE_FORMAT(sysmodified,'%Y-%m-%d %T') as modi From `kpusers`.`sysuseraccounts` where syscreated > @created ORDER BY syscreated ASC;";
                    comm.Parameters.Clear();
                    comm.Parameters.AddWithValue("created", created);
                    MySqlDataAdapter adptcreate = new MySqlDataAdapter();
                    DataTable dtacreate = new DataTable("createuser");
                    DataSet dtasetcreate = new DataSet();
                    adptcreate.SelectCommand = comm;
                    adptcreate.Fill(dtasetcreate);
                    dtacreate = dtasetcreate.Tables[0];
                    int createnew = 0;

                    for (int x = 0; x < dtacreate.Rows.Count; x++)
                    {
                        resourceID = dtacreate.Rows[x]["resourceID"].ToString();
                        UserLogin = dtacreate.Rows[x]["UserLogin"].ToString();
                        UserPassword = dtacreate.Rows[x]["UserPassword"].ToString();
                        sysmodified = dtacreate.Rows[x]["modi"].ToString();
                        BranchCode = dtacreate.Rows[x]["BranchCode"].ToString();
                        // RoleID = dtacreate.Rows[x]["RoleID"].ToString();
                        DeptCode = dtacreate.Rows[x]["DeptCode"].ToString();
                        IsReliever = dtacreate.Rows[x]["IsReliever"].ToString();
                        TempBranchCode = dtacreate.Rows[x]["TempBranchCode"].ToString();
                        // TempZoneCode = dtacreate.Rows[x]["TempZoneCode"].ToString();
                        ZoneCode = dtacreate.Rows[x]["ZoneCode"].ToString();
                        StartDate = dtacreate.Rows[x]["StartDate"].ToString();
                        EndDate = dtacreate.Rows[x]["EndDate"].ToString();
                        IsActive = dtacreate.Rows[x]["IsActive"].ToString();
                        //syscreated = dtacreate.Rows[x]["syscreated"].ToString();
                        syscreated = dtacreate.Rows[x]["syscreated"].ToString();
                        syscreator = dtacreate.Rows[x]["syscreator"].ToString();
                        sysmodifier = dtacreate.Rows[x]["sysmodifier"].ToString();
                        IsResign = dtacreate.Rows[x]["IsResign"].ToString();
                        //CellNo = dtacreate.Rows[x]["CellNo"].ToString();
                        //EmailAddress = dtacreate.Rows[x]["EmailAddress"].ToString();

                        if (sysmodified == string.Empty)
                        {
                            sysmodified = null;
                        }
                        if (syscreated == string.Empty)
                        {
                            syscreated = null;
                        }
                        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

                        //endpoint = NetworkServicePath + "/savesysuseraccountsglobal";
                        endpoint = NetworkServicePath + "/savesysuseraccountsglobal";
                        info = "{\"resourceID\":\"" + resourceID + "\",\"UserLogin\":\"" + UserLogin + "\",\"UserPassword\":\"" + UserPassword + "\",\"sysmodified\":\"" + sysmodified + "\",\"BranchCode\":\"" + BranchCode + "\",\"DeptCode\":\"" + DeptCode + "\",\"IsReliever\":\"" + IsReliever + "\",\"TempBranchCode\":\"" + TempBranchCode +
                           "\",\"ZoneCode\":\"" + ZoneCode + "\",\"StartDate\":\"" + StartDate + "\",\"EndDate\":\"" + EndDate + "\",\"IsActive\":\"" + IsActive + "\",\"syscreated\":\"" + syscreated + "\",\"syscreator\":\"" + syscreator + "\",\"sysmodifier\":\"" + sysmodifier + "\", \"IsResign\":\"" + IsResign + "\"}";
                        var uri = new Uri(endpoint);
                        dataresp = Encoding.UTF8.GetBytes(info);
                        String resp = SendRequest(uri, dataresp, "application/json", "POST");
                        resultparse = JObject.Parse(resp);
                        resultcodecreate = resultparse.respcode;

                        if (resultcodecreate == 0)
                        {
                            return new response { respcode = 0, respmsg = resultparse.respmsg };
                        }
                    }


                    //getting modified datas

                    comm.CommandText = "Select *, DATE_FORMAT(sysmodified,'%Y-%m-%d %T') as modi From `kpusers`.`sysuseraccounts` where sysmodified > @modified ORDER BY modi ASC;";
                    comm.Parameters.Clear();
                    comm.Parameters.AddWithValue("modified", modified);
                    MySqlDataAdapter adptr = new MySqlDataAdapter();
                    adptr.SelectCommand = comm;
                    DataTable data = new DataTable("users");
                    DataSet dtaset = new DataSet();
                    adptr.Fill(dtaset);
                    data = dtaset.Tables[0];
                    // int updtusers = 0;
                    for (int x = 0; x < data.Rows.Count; x++)
                    {
                        resourceID = data.Rows[x]["resourceID"].ToString();
                        UserPassword = data.Rows[x]["UserPassword"].ToString();
                        sysmodified = data.Rows[x]["modi"].ToString();

                        //updating modified datas

                        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });


                        //   endpoint = NetworkServicePath + "/savemodifiedsysuserglobal";
                        endpoint = NetworkServicePath + "/savemodifiedsysuserglobal";
                        info = "{\"resourceID\":\"" + resourceID + "\",\"UserPassword\":\"" + UserPassword + "\",\"sysmodified\":\"" + sysmodified + "\"}";
                        var urimod = new Uri(endpoint);
                        dataresp = Encoding.UTF8.GetBytes(info);
                        String respmod = SendRequest(urimod, dataresp, "application/json", "POST");
                        resultparse = JObject.Parse(respmod);
                        resultcodemod = resultparse.respcode;
                        if (resultcodemod == 0)
                        {
                            return new response { respcode = 0, respmsg = "Error updating datas in sysuseraccounts" };
                        }
                    }

                    if (resultcodecreate == 1 && resultcodemod == 1)
                    {

                        return new response { respcode = 1, respmsg = "Successfully migrated data. Newly CREATED: " + dtacreate.Rows.Count + " UPDATED: " + data.Rows.Count, count = data.Rows.Count + dtacreate.Rows.Count };
                    }
                    else if (resultcodecreate == 1)
                    {

                        return new response { respcode = 1, respmsg = "Successfully migrated data. Newly Created: " + dtacreate.Rows.Count + "", count = data.Rows.Count + dtacreate.Rows.Count };
                    }
                    else if (resultcodemod == 1)
                    {

                        return new response { respcode = 1, respmsg = "Successfully migrated data. Newly Modified: " + data.Rows.Count + "", count = data.Rows.Count };
                    }


                }
                con.Close();
            }
            return new response { respcode = 102 };
        }
        catch (Exception ex)
        {
            return new response { respcode = 101, respmsg = ex.ToString() };
        }
    }

    public response getdatabranchusers(String modified, String created)
    {
        try
        {
            String maxmodified = string.Empty;
            String maxcreated = string.Empty;
            String BranchCode = string.Empty;
            String resourceID = string.Empty;
            Int32 IsReliever = 0;
            Int32 IsActiveReliever = 0;
            String syscreated = string.Empty;
            String sysmodified = string.Empty;
            Int32 ZoneCode = 0;
            String fullname = string.Empty;
            String lastname = string.Empty;
            String firstname = string.Empty;
            String middlename = string.Empty;
            String syscreator = string.Empty;
            String sysmodifier = string.Empty;
            String endpoint;
            String info;
            Byte[] dataresp;
            dynamic resultparse = 0;
            Int32 resultcodecreate = 0;
            Int32 resultcodemod = 0;
            using (MySqlConnection con = dbconcloud.getConnection())
            {
                con.Open();
                using (MySqlCommand comm = con.CreateCommand())
                {
                    comm.CommandText = "Select DATE_FORMAT(MAX(syscreated),'%Y-%m-%d %T') as maxcreated, DATE_FORMAT(MAX(sysmodified),'%Y-%m-%d %T') as maxmodified FROM `kpusers`.`branchusers`;";
                    MySqlDataReader reader = comm.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        maxmodified = reader["maxmodified"].ToString();
                        maxcreated = reader["maxcreated"].ToString();

                        reader.Close();

                    }

                    if (maxcreated == created && maxmodified == modified)
                    {

                        return new response { respcode = 1, respmsg = "Datas are already synch!" };

                    }

                    // sysuser accounts newly created 
                    comm.CommandText = "Select *, DATE_FORMAT(syscreated, '%Y-%m-%d %T') as creaty,DATE_FORMAT(sysmodified, '%Y-%m-%d %T') as modi From `kpusers`.`branchusers` where syscreated > @created ORDER BY creaty ASC;";
                    comm.Parameters.Clear();
                    comm.Parameters.AddWithValue("created", created);
                    MySqlDataAdapter adptcreate = new MySqlDataAdapter();
                    DataTable dtacreate = new DataTable("creatuser");
                    DataSet dtasetcreate = new DataSet();
                    adptcreate.SelectCommand = comm;
                    adptcreate.Fill(dtasetcreate);
                    dtacreate = dtasetcreate.Tables[0];
                    // int createnew = 0;

                    for (int x = 0; x < dtacreate.Rows.Count; x++)
                    {
                        BranchCode = dtacreate.Rows[x]["BranchCode"].ToString();
                        resourceID = dtacreate.Rows[x]["ResourceID"].ToString();
                        IsReliever = Convert.ToInt32(dtacreate.Rows[x]["IsReliever"]);
                        IsActiveReliever = Convert.ToInt32(dtacreate.Rows[x]["IsActiveReliever"]);
                        syscreated = dtacreate.Rows[x]["creaty"].ToString();
                        sysmodified = dtacreate.Rows[x]["modi"].ToString();
                        ZoneCode = Convert.ToInt32(dtacreate.Rows[x]["ZoneCode"]);
                        fullname = dtacreate.Rows[x]["fullname"].ToString();
                        lastname = dtacreate.Rows[x]["lastname"].ToString();
                        firstname = dtacreate.Rows[x]["firstname"].ToString();
                        middlename = dtacreate.Rows[x]["middlename"].ToString();
                        syscreator = dtacreate.Rows[x]["syscreated"].ToString();
                        sysmodifier = dtacreate.Rows[x]["sysmodifier"].ToString();

                        if (sysmodified == string.Empty)
                        {
                            sysmodified = null;
                        }
                        if (syscreated == string.Empty)
                        {
                            syscreated = null;
                        }
                        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

                        endpoint = NetworkServicePath + "/savebranchuser";
                        info = "{\"BranchCode\":\"" + BranchCode + "\",\"resourceID\":\"" + resourceID + "\",\"IsReliever\":\"" + IsReliever + "\", \"IsActiveReliever\":\"" + IsActiveReliever + "\", \"syscreated\":\"" + syscreated +
                            "\", \"sysmodified\":\"" + sysmodified + "\",\"ZoneCode\":\"" + ZoneCode + "\",\"fullname\":\"" + fullname + "\",\"lastname\":\"" + lastname + "\",\"firstname\":\"" + firstname + "\",\"middlename\":\"" + middlename + "\", \"syscreator\":\"" + syscreator + "\", \"sysmodifier\":\"" + sysmodifier + "\"}";
                        var uri = new Uri(endpoint);
                        dataresp = Encoding.UTF8.GetBytes(info);
                        String resp = SendRequest(uri, dataresp, "application/json", "POST");
                        resultparse = JObject.Parse(resp);
                        resultcodecreate = resultparse.respcode;
                        if (resultcodecreate == 0)
                        {
                            return new response { respcode = 0, respmsg = "Error in inserting new data in branchusers ." };
                        }
                    }


                    comm.CommandText = "Select *, DATE_FORMAT(sysmodified,'%Y-%m-%d %T') as modi, DATE_FORMAT(syscreated,'%Y-%m-%d %T') as creaty From `kpusers`.`branchusers` where sysmodified > @modified ORDER BY modi ASC;";
                    comm.Parameters.Clear();
                    comm.Parameters.AddWithValue("modified", modified);
                    MySqlDataAdapter adptr = new MySqlDataAdapter();
                    adptr.SelectCommand = comm;
                    DataTable data = new DataTable("users");
                    DataSet dtaset = new DataSet();
                    adptr.Fill(dtaset);
                    data = dtaset.Tables[0];
                    // int updtusers = 0;
                    for (int x = 0; x < data.Rows.Count; x++)
                    {
                        BranchCode = data.Rows[x]["BranchCode"].ToString();
                        resourceID = data.Rows[x]["ResourceID"].ToString();
                        IsReliever = Convert.ToInt32(data.Rows[x]["IsReliever"]);
                        IsActiveReliever = Convert.ToInt32(data.Rows[x]["IsActiveReliever"]);
                        syscreated = data.Rows[x]["creaty"].ToString();
                        sysmodified = data.Rows[x]["modi"].ToString();
                        ZoneCode = Convert.ToInt32(data.Rows[x]["ZoneCode"]);
                        fullname = data.Rows[x]["fullname"].ToString();
                        lastname = data.Rows[x]["lastname"].ToString();
                        firstname = data.Rows[x]["firstname"].ToString();
                        middlename = data.Rows[x]["middlename"].ToString();
                        syscreator = data.Rows[x]["syscreator"].ToString();
                        sysmodifier = data.Rows[x]["sysmodifier"].ToString();

                        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

                        endpoint = NetworkServicePath + "/savemodifiedbranchuser";
                        info = "{\"BranchCode\":\"" + BranchCode + "\",\"resourceID\":\"" + resourceID + "\",\"IsReliever\":\"" + IsReliever + "\", \"IsActiveReliever\":\"" + IsActiveReliever + "\", \"syscreated\":\"" + syscreated +
                            "\", \"sysmodified\":\"" + sysmodified + "\",\"ZoneCode\":\"" + ZoneCode + "\",\"fullname\":\"" + fullname + "\",\"lastname\":\"" + lastname + "\",\"firstname\":\"" + firstname + "\",\"middlename\":\"" + middlename + "\", \"syscreator\":\"" + syscreator + "\", \"sysmodifier\":\"" + sysmodifier + "\"}";
                        var urimod = new Uri(endpoint);
                        dataresp = Encoding.UTF8.GetBytes(info);
                        String respmod = SendRequest(urimod, dataresp, "application/json", "POST");
                        resultparse = JObject.Parse(respmod);
                        resultcodemod = resultparse.respcode;
                        if (resultcodemod == 0)
                        {
                            return new response { respcode = 0, respmsg = "Error in updating data in branchusers." };
                        }
                    }


                    if (resultcodecreate == 1 && resultcodemod == 1)
                    {
                        return new response { respcode = 1, respmsg = "Successfully migrated data . Newly created: " + dtacreate.Rows.Count + ": Updated: " + data.Rows.Count + "", count = dtacreate.Rows.Count + data.Rows.Count };
                    }
                    else if (resultcodecreate == 1)
                    {
                        return new response { respcode = 1, respmsg = "Successfully migrated data . Newly created: " + dtacreate.Rows.Count, count = dtacreate.Rows.Count };
                    }
                    else if (resultcodemod == 1)
                    {
                        return new response { respcode = 1, respmsg = "Successfully migrated data . Newly updated: " + data.Rows.Count, count = data.Rows.Count };
                    }
                }
                con.Close();
            }
            return new response { respcode = 102 };
        }
        catch (Exception ex)
        {
            return new response { respcode = 101, respmsg = ex.ToString() };
        }
    }

    //Retrieve newly created and modified mlbranchesstations..!!

    public response getdatamlbranchesstations(String modified, String created)
    {
        try
        {
            String StationCode = string.Empty;
            Int32 StationNo;
            String BranchCode = string.Empty;
            String syscreated = string.Empty;
            String syscreator = string.Empty;
            String sysmodified = string.Empty;
            String sysmodifier = string.Empty;
            Int32 ZoneCode = 0;
            String branchstat = string.Empty;
            Int32 IsUpdated = 0;
            String version = string.Empty;
            Int32 isdowngraded = 0;
            String adminversion = string.Empty;
            Int32 releasetype = 0;
            Int32 isallowupdate = 0;
            Int32 curnotification = 0;
            String datenotified = string.Empty;
            String updatedcurr = string.Empty;
            Int32 isallowupdateadmin = 0;

            String endpoint;
            String info;
            Byte[] dataresp;
            dynamic resultparse = 0;
            Int32 resultcodecreate = 0;
            Int32 resultcodemod = 0;
            String type = string.Empty;
            String maxmodified = string.Empty;
            string maxcreated = string.Empty;
            using (MySqlConnection con = dbconcloud.getConnection())
            {
                con.Open();
                using (MySqlCommand comm = con.CreateCommand())
                {
                    comm.CommandText = "Select DATE_FORMAT(MAX(syscreated),'%Y-%m-%d %T') as maxcreated, DATE_FORMAT(MAX(sysmodified),'%Y-%m-%d %T') as maxmodified FROM `kpusers`.`mlbranchesstations`;";
                    MySqlDataReader reader = comm.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        maxmodified = reader["maxmodified"].ToString();
                        maxcreated = reader["maxcreated"].ToString();
                        reader.Close();
                    }

                    if (maxcreated == created && maxmodified == modified)
                    {

                        return new response { respcode = 1, respmsg = "Datas are already synch!" };
                    }


                    // mlbranchestations newly created 
                    comm.Parameters.Clear();
                    comm.CommandText = "Select *, DATE_FORMAT(syscreated,'%Y-%m-%d %T') as creaty From `kpusers`.`mlbranchesstations` where syscreated > @created ORDER BY creaty ASC;";
                    comm.Parameters.AddWithValue("created", created);
                    MySqlDataAdapter adptcreate = new MySqlDataAdapter();
                    DataTable dtacreate = new DataTable("creatuser");
                    DataSet dtasetcreate = new DataSet();
                    adptcreate.SelectCommand = comm;
                    adptcreate.Fill(dtasetcreate);
                    dtacreate = dtasetcreate.Tables[0];
                    type = "Create";
                    for (int x = 0; x < dtacreate.Rows.Count; x++)
                    {
                        StationCode = dtacreate.Rows[x]["StationCode"].ToString();
                        StationNo = Convert.ToInt32(dtacreate.Rows[x]["StationNo"]);
                        BranchCode = dtacreate.Rows[x]["BranchCode"].ToString();
                        syscreated = dtacreate.Rows[x]["creaty"].ToString();
                        syscreator = dtacreate.Rows[x]["syscreator"].ToString();
                        sysmodified = dtacreate.Rows[x]["sysmodified"].ToString();
                        sysmodifier = dtacreate.Rows[x]["sysmodifier"].ToString();
                        ZoneCode = Convert.ToInt32(dtacreate.Rows[x]["ZoneCode"]);
                        branchstat = dtacreate.Rows[x]["branchstat"].ToString();
                        IsUpdated = Convert.ToInt32(dtacreate.Rows[x]["IsUpdated"]);
                        version = String.IsNullOrEmpty(dtacreate.Rows[x].ToString()) ? null : dtacreate.Rows[x]["version"].ToString();
                        isdowngraded = Convert.ToInt32(dtacreate.Rows[x]["isdowngraded"]);
                        adminversion = String.IsNullOrEmpty(dtacreate.Rows[x].ToString()) ? null : dtacreate.Rows[x]["adminversion"].ToString();
                        releasetype = Convert.ToInt32(dtacreate.Rows[x]["releasetype"]);
                        isallowupdate = Convert.ToInt32(dtacreate.Rows[x]["isallowupdate"]);
                        isallowupdateadmin = Convert.ToInt32(dtacreate.Rows[x]["isallowupdateadmin"]);

                        if (sysmodified == string.Empty)
                        {
                            sysmodified = null;
                        }
                        if (syscreated == string.Empty)
                        {
                            syscreated = null;
                        }
                        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

                        endpoint = NetworkServicePath + "/savemlbranchesstations";
                        info = "{\"StationCode\":\"" + StationCode + "\",\"StationNo\":\"" + StationNo + "\", \"BranchCode\":\"" + BranchCode + "\", \"syscreated\":\"" + syscreated +
                            "\", \"syscreator\":\"" + syscreator + "\", \"sysmodified\":\"" + sysmodified + "\", \"sysmodifier\":\"" + sysmodifier + "\",\"ZoneCode\":\"" + ZoneCode +
                            "\", \"branchstat\":\"" + branchstat + "\", \"IsUpdated\":\"" + IsUpdated + "\", \"version\":\"" + version + "\", \"isdowngraded\":\"" + isdowngraded +
                            "\",\"adminversion\":\"" + adminversion + "\", \"releasetype\":\"" + releasetype + "\", \"isallowupdate\":\"" + isallowupdate + "\",\"isallowupdateadmin\":\"" + isallowupdateadmin + "\", \"type\":\"" + type + "\"}";
                        var uri = new Uri(endpoint);
                        dataresp = Encoding.UTF8.GetBytes(info);
                        String resp = SendRequest(uri, dataresp, "application/json", "POST");
                        resultparse = JObject.Parse(resp);
                        resultcodecreate = resultparse.respcode;
                        if (resultcodecreate == 0)
                        {
                            return new response { respcode = 0, respmsg = resultparse.respmsg };
                        }
                    }


                    comm.CommandText = "Select *, DATE_FORMAT(sysmodified,'%Y-%m-%d %T') as modi From `kpusers`.`mlbranchesstations` where sysmodified > @modified ORDER BY modi ASC;";
                    comm.Parameters.Clear();
                    comm.Parameters.AddWithValue("modified", modified);
                    MySqlDataAdapter adptr = new MySqlDataAdapter();
                    adptr.SelectCommand = comm;
                    DataTable data = new DataTable("users");
                    DataSet dtaset = new DataSet();
                    adptr.Fill(dtaset);
                    data = dtaset.Tables[0];
                    type = "Modify";
                    for (int x = 0; x < data.Rows.Count; x++)
                    {
                        StationCode = data.Rows[x]["StationCode"].ToString();
                        StationNo = Convert.ToInt32(data.Rows[x]["StationNo"]);
                        BranchCode = data.Rows[x]["BranchCode"].ToString();
                        syscreated = data.Rows[x]["syscreated"].ToString();
                        syscreator = data.Rows[x]["syscreator"].ToString();
                        sysmodified = data.Rows[x]["modi"].ToString();
                        sysmodifier = data.Rows[x]["sysmodifier"].ToString();
                        ZoneCode = Convert.ToInt32(data.Rows[x]["ZoneCode"]);
                        branchstat = data.Rows[x]["branchstat"].ToString();
                        IsUpdated = Convert.ToInt32(data.Rows[x]["IsUpdated"]);
                        version = String.IsNullOrEmpty(data.Rows[x].ToString()) ? null : data.Rows[x]["version"].ToString();
                        isdowngraded = Convert.ToInt32(data.Rows[x]["isdowngraded"]);
                        adminversion = String.IsNullOrEmpty(data.Rows[x].ToString()) ? null : data.Rows[x]["adminversion"].ToString();
                        releasetype = Convert.ToInt32(data.Rows[x]["releasetype"]);
                        isallowupdate = Convert.ToInt32(data.Rows[x]["isallowupdate"]);
                        isallowupdateadmin = Convert.ToInt32(data.Rows[x]["isallowupdateadmin"]);


                        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

                        endpoint = NetworkServicePath + "/savemlbranchesstations";
                        info = "{\"StationCode\":\"" + StationCode + "\",\"StationNo\":\"" + StationNo + "\", \"BranchCode\":\"" + BranchCode + "\", \"syscreated\":\"" + syscreated +
                            "\", \"syscreator\":\"" + syscreator + "\", \"sysmodified\":\"" + sysmodified + "\", \"sysmodifier\":\"" + sysmodifier + "\",\"ZoneCode\":\"" + ZoneCode +
                            "\", \"branchstat\":\"" + branchstat + "\", \"IsUpdated\":\"" + IsUpdated + "\", \"version\":\"" + version + "\", \"isdowngraded\":\"" + isdowngraded +
                            "\",\"adminversion\":\"" + adminversion + "\", \"releasetype\":\"" + releasetype + "\", \"isallowupdate\":\"" + isallowupdate + "\",\"isallowupdateadmin\":\"" + isallowupdateadmin + "\" , \"type\":\"" + type + "\"}";
                        var uri = new Uri(endpoint);
                        dataresp = Encoding.UTF8.GetBytes(info);
                        String resp = SendRequest(uri, dataresp, "application/json", "POST");
                        resultparse = JObject.Parse(resp);
                        resultcodemod = resultparse.respcode;
                        if (resultcodemod == 0)
                        {
                            return new response { respcode = 0, respmsg = resultparse.respmsg };
                        }

                    }



                    if (resultcodecreate == 1 && resultcodemod == 1)
                    {
                        return new response { respcode = 1, respmsg = "Successfully migrated data . Newly created: " + dtacreate.Rows.Count + " Newly Modified: " + data.Rows.Count, count = (dtacreate.Rows.Count + data.Rows.Count) };
                    }
                    else if (resultcodecreate == 1)
                    {

                        return new response { respcode = 1, respmsg = "Successfully Created New Data . Count : " + dtacreate.Rows.Count };
                    }
                    else if (resultcodemod == 1)
                    {
                        return new response { respcode = 1, respmsg = "Successfully Modified New Datas. Count : " + data.Rows.Count };
                    }


                }
                con.Close();
            }
            return new response { respcode = 102 };
        }
        catch (Exception ex)
        {
            return new response { respcode = 101, respmsg = ex.ToString() };
        }
    }

    public response getdataadminusers(String modified, String created)
    {

        Int32 ResourceID = 0;
        String DeptCode = string.Empty;
        String Fullname = string.Empty;
        String Firstname = string.Empty;
        String Lastname = string.Empty;
        String Middlename = string.Empty;
        Int32 ZoneCode = 0;
        String syscreated = string.Empty;
        String syscreator = string.Empty;
        String sysmodified = string.Empty;
        String sysmodifier = string.Empty;


        String endpoint;
        String info;
        Byte[] dataresp;
        dynamic resultparse = 0;
        Int32 resultcodecreate = 0;
        Int32 resultcodemod = 0;
        String maxcreated = string.Empty;
        String maxmodified = string.Empty;
        String type = string.Empty;
        using (MySqlConnection con = dbconcloud.getConnection())
        {
            con.Open();
            using (MySqlCommand comm = con.CreateCommand())
            {
                comm.Parameters.Clear();
                comm.CommandText = "Select DATE_FORMAT(MAX(syscreated),'%Y-%m-%d %T') as maxcreated, DATE_FORMAT(MAX(sysmodified),'%Y-%m-%d %T') as maxmodified FROM `kpusers`.`adminusers`;";
                MySqlDataReader rdr = comm.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    maxmodified = rdr["maxmodified"].ToString();
                    maxcreated = rdr["maxcreated"].ToString();
                    rdr.Close();
                }
                else
                {
                    return new response { respcode = 0, respmsg = "Not Found!" };
                }
                if (maxcreated == created && maxmodified == modified)
                {
                    return new response { respcode = 1, respmsg = "Datas are already synch!" };

                }

                // mlbranchestations newly created 
                comm.Parameters.Clear();
                comm.CommandText = "Select *, DATE_FORMAT(syscreated,'%Y-%m-%d %T') as creaty,DATE_FORMAT(sysmodified,'%Y-%m-%d %T') as modi From `kpusers`.`adminusers` where syscreated > @created ORDER BY creaty ASC;";
                comm.Parameters.AddWithValue("created", created);
                MySqlDataAdapter adptcreate = new MySqlDataAdapter();
                DataTable dtacreate = new DataTable("creatuser");
                DataSet dtasetcreate = new DataSet();
                adptcreate.SelectCommand = comm;
                adptcreate.Fill(dtasetcreate);
                dtacreate = dtasetcreate.Tables[0];
                type = "Create";
                for (int x = 0; x < dtacreate.Rows.Count; x++)
                {
                    ResourceID = Convert.ToInt32(dtacreate.Rows[x]["ResourceID"]);
                    DeptCode = dtacreate.Rows[x]["DeptCode"].ToString();
                    Fullname = dtacreate.Rows[x]["Fullname"].ToString();
                    Firstname = dtacreate.Rows[x]["Firstname"].ToString();
                    Lastname = dtacreate.Rows[x]["Lastname"].ToString();
                    Middlename = dtacreate.Rows[x]["Middlename"].ToString();
                    ZoneCode = Convert.ToInt32(dtacreate.Rows[x]["ZoneCode"]);
                    syscreator = dtacreate.Rows[x]["syscreator"].ToString();
                    syscreated = dtacreate.Rows[x]["creaty"].ToString();
                    sysmodifier = dtacreate.Rows[x]["sysmodifier"].ToString();
                    sysmodified = dtacreate.Rows[x]["modi"].ToString();


                    if (sysmodified == string.Empty)
                    {
                        sysmodified = null;
                    }
                    if (syscreated == string.Empty)
                    {
                        syscreated = null;
                    }
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

                    endpoint = NetworkServicePath + "/saveadminusers";
                    info = "{\"ResourceID\":\"" + ResourceID + "\",\"DeptCode\":\"" + DeptCode + "\", \"Fullname\":\"" + Fullname + "\", \"Firstname\":\"" + Firstname +
                        "\", \"Lastname\":\"" + Lastname + "\", \"Middlename\":\"" + Middlename + "\", \"ZoneCode\":\"" + ZoneCode + "\",\"syscreator\":\"" + syscreator +
                        "\", \"syscreated\":\"" + syscreated + "\", \"sysmodifier\":\"" + sysmodifier + "\", \"sysmodified\":\"" + sysmodified + "\", \"type\":\"" + type + "\"}";
                    var uri = new Uri(endpoint);
                    dataresp = Encoding.UTF8.GetBytes(info);
                    String resp = SendRequest(uri, dataresp, "application/json", "POST");
                    resultparse = JObject.Parse(resp);
                    resultcodecreate = resultparse.respcode;
                    if (resultcodecreate == 0)
                    {
                        return new response { respcode = 0, respmsg = resultparse.respmsg };
                    }
                }


                comm.CommandText = "Select *, DATE_FORMAT(sysmodified,'%Y-%m-%d %T') as modi,DATE_FORMAT(syscreated,'%Y-%m-%d %T') as creaty From `kpusers`.`adminusers` where sysmodified > @modified ORDER BY modi ASC;";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("modified", modified);
                MySqlDataAdapter adptr = new MySqlDataAdapter();
                adptr.SelectCommand = comm;
                DataTable data = new DataTable("users");
                DataSet dtaset = new DataSet();
                adptr.Fill(dtaset);
                data = dtaset.Tables[0];
                type = "Modify";
                for (int x = 0; x < data.Rows.Count; x++)
                {
                    ResourceID = Convert.ToInt32(data.Rows[x]["ResourceID"]);
                    DeptCode = data.Rows[x]["DeptCode"].ToString();
                    Fullname = data.Rows[x]["Fullname"].ToString();
                    Firstname = data.Rows[x]["Firstname"].ToString();
                    Lastname = data.Rows[x]["Lastname"].ToString();
                    Middlename = data.Rows[x]["Middlename"].ToString();
                    ZoneCode = Convert.ToInt32(data.Rows[x]["ZoneCode"]);
                    syscreator = data.Rows[x]["syscreator"].ToString();
                    syscreated = data.Rows[x]["creaty"].ToString();
                    sysmodifier = data.Rows[x]["sysmodifier"].ToString();
                    sysmodified = data.Rows[x]["modi"].ToString();


                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

                    endpoint = NetworkServicePath + "/saveadminusers";
                    info = "{\"ResourceID\":\"" + ResourceID + "\",\"DeptCode\":\"" + DeptCode + "\", \"Fullname\":\"" + Fullname + "\", \"Firstname\":\"" + Firstname +
                        "\", \"Lastname\":\"" + Lastname + "\", \"Middlename\":\"" + Middlename + "\", \"ZoneCode\":\"" + ZoneCode + "\",\"syscreator\":\"" + syscreator +
                        "\", \"syscreated\":\"" + syscreated + "\", \"sysmodifier\":\"" + sysmodifier + "\", \"sysmodified\":\"" + sysmodified + "\", \"type\":\"" + type + "\"}";
                    var uri = new Uri(endpoint);
                    dataresp = Encoding.UTF8.GetBytes(info);
                    String resp = SendRequest(uri, dataresp, "application/json", "POST");
                    resultparse = JObject.Parse(resp);
                    resultcodemod = resultparse.respcode;
                    if (resultcodemod == 0)
                    {
                        return new response { respcode = 0, respmsg = resultparse.respmsg };
                    }

                }

                if (resultcodecreate == 1 && resultcodemod == 1)
                {
                    return new response { respcode = 1, respmsg = "Successfully migrated data . Newly created: " + dtacreate.Rows.Count + " Newly Modified: " + data.Rows.Count, count = (dtacreate.Rows.Count + data.Rows.Count) };
                }
                else if (resultcodecreate == 1)
                {

                    return new response { respcode = 1, respmsg = "Successfully Created New Data . Count : " + dtacreate.Rows.Count };
                }
                else if (resultcodemod == 1)
                {
                    return new response { respcode = 1, respmsg = "Successfully Modified New Data . Count : " + data.Rows.Count };
                }
                con.Close();
            }


            return new response { respcode = 101 };

        }
    }


    //----AREA------------------------------------------------------------------------------------------------


    public response getdataarea(String modified, String created)
    {

        String AreaName = string.Empty;
        String AreaCode = string.Empty;
        Int32 RegionCode = 0;
        Int32 ZoneCode = 0;
        String syscreated = string.Empty;
        String syscreator = string.Empty;
        String sysmodified = string.Empty;
        String sysmodifier = string.Empty;
        //   Int32 ID = 0;


        String endpoint;
        String info;
        Byte[] dataresp;
        dynamic resultparse = 0;
        Int32 resultcodecreate = 0;
        Int32 resultcodemod = 0;
        String maxcreated = string.Empty;
        String maxmodified = string.Empty;
        String type = string.Empty;
        using (MySqlConnection con = dbconcloud.getConnection())
        {
            con.Open();
            using (MySqlCommand comm = con.CreateCommand())
            {
                comm.Parameters.Clear();
                comm.CommandText = "Select DATE_FORMAT(MAX(syscreated),'%Y-%m-%d %T') as maxcreated, DATE_FORMAT(MAX(sysmodified),'%Y-%m-%d %T') as maxmodified FROM `kpusers`.`area`;";
                MySqlDataReader rdr = comm.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    maxmodified = rdr["maxmodified"].ToString();
                    maxcreated = rdr["maxcreated"].ToString();
                    rdr.Close();
                }
                else
                {
                    return new response { respcode = 0, respmsg = "Not Found!" };
                }
                if (maxcreated == created && maxmodified == modified)
                {
                    return new response { respcode = 1, respmsg = "Datas are already synch!" };

                }

                // area newly created 
                comm.Parameters.Clear();
                comm.CommandText = "Select *, DATE_FORMAT(syscreated,'%Y-%m-%d %T') as creaty,DATE_FORMAT(sysmodified,'%Y-%m-%d %T') as modi From `kpusers`.`area` where syscreated > @created ORDER BY creaty ASC;";
                comm.Parameters.AddWithValue("created", created);
                MySqlDataAdapter adptcreate = new MySqlDataAdapter();
                DataTable dtacreate = new DataTable("creatuser");
                DataSet dtasetcreate = new DataSet();
                adptcreate.SelectCommand = comm;
                adptcreate.Fill(dtasetcreate);
                dtacreate = dtasetcreate.Tables[0];
                type = "Create";
                for (int x = 0; x < dtacreate.Rows.Count; x++)
                {
                    // ID = Convert.ToInt32(dtacreate.Rows[x]["ID"]);
                    AreaName = dtacreate.Rows[x]["AreaName"].ToString();
                    AreaCode = dtacreate.Rows[x]["AreaCode"].ToString();
                    RegionCode = Convert.ToInt32(dtacreate.Rows[x]["RegionCode"]);
                    ZoneCode = Convert.ToInt32(dtacreate.Rows[x]["ZoneCode"]);
                    syscreator = dtacreate.Rows[x]["syscreator"].ToString();
                    syscreated = dtacreate.Rows[x]["creaty"].ToString();
                    sysmodifier = dtacreate.Rows[x]["sysmodifier"].ToString();
                    sysmodified = dtacreate.Rows[x]["modi"].ToString();


                    if (sysmodified == string.Empty)
                    {
                        sysmodified = null;
                    }
                    if (syscreated == string.Empty)
                    {
                        syscreated = null;
                    }
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

                    endpoint = NetworkServicePath + "/savearea";
                    info = "{\"AreaName\":\"" + AreaName + "\",\"AreaCode\":\"" + AreaCode + "\", \"RegionCode\":\"" + RegionCode + "\", \"ZoneCode\":\"" + ZoneCode + "\", \"syscreator\":\"" + syscreator +
                        "\", \"syscreated\":\"" + syscreated + "\", \"sysmodifier\":\"" + sysmodifier + "\", \"sysmodified\":\"" + sysmodified + "\" , \"type\":\"" + type + "\"}";
                    var uri = new Uri(endpoint);
                    dataresp = Encoding.UTF8.GetBytes(info);
                    String resp = SendRequest(uri, dataresp, "application/json", "POST");
                    resultparse = JObject.Parse(resp);
                    resultcodecreate = resultparse.respcode;
                    if (resultcodecreate == 0)
                    {
                        return new response { respcode = 0, respmsg = resultparse.respmsg };
                    }
                }


                comm.CommandText = "Select *, DATE_FORMAT(sysmodified,'%Y-%m-%d %T') as modi,DATE_FORMAT(syscreated,'%Y-%m-%d %T') as creaty From `kpusers`.`area` where sysmodified > @modified ORDER BY modi ASC;";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("modified", modified);
                MySqlDataAdapter adptr = new MySqlDataAdapter();
                adptr.SelectCommand = comm;
                DataTable data = new DataTable("users");
                DataSet dtaset = new DataSet();
                adptr.Fill(dtaset);
                data = dtaset.Tables[0];
                type = "Modify";
                for (int x = 0; x < data.Rows.Count; x++)
                {

                    //     ID = Convert.ToInt32(dtacreate.Rows[x]["ID"]);
                    AreaName = data.Rows[x]["AreaName"].ToString();
                    AreaCode = data.Rows[x]["AreaCode"].ToString();
                    RegionCode = Convert.ToInt32(data.Rows[x]["RegionCode"]);
                    ZoneCode = Convert.ToInt32(data.Rows[x]["ZoneCode"]);
                    syscreator = data.Rows[x]["syscreator"].ToString();
                    syscreated = data.Rows[x]["creaty"].ToString();
                    sysmodifier = data.Rows[x]["sysmodifier"].ToString();
                    sysmodified = data.Rows[x]["modi"].ToString();


                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

                    endpoint = NetworkServicePath + "/savearea";
                    info = "{\"AreaName\":\"" + AreaName + "\",\"AreaCode\":\"" + AreaCode + "\", \"RegionCode\":\"" + RegionCode + "\", \"ZoneCode\":\"" + ZoneCode + "\", \"syscreator\":\"" + syscreator +
                        "\", \"syscreated\":\"" + syscreated + "\", \"sysmodifier\":\"" + sysmodifier + "\", \"sysmodified\":\"" + sysmodified + "\" , \"type\":\"" + type + "\"}";
                    var uri = new Uri(endpoint);
                    dataresp = Encoding.UTF8.GetBytes(info);
                    String resp = SendRequest(uri, dataresp, "application/json", "POST");
                    resultparse = JObject.Parse(resp);
                    resultcodemod = resultparse.respcode;
                    if (resultcodemod == 0)
                    {
                        return new response { respcode = 0, respmsg = "Error in updating area." };
                    }

                }

                if (resultcodecreate == 1 && resultcodemod == 1)
                {
                    return new response { respcode = 1, respmsg = "Successfully migrated data . Newly created: " + dtacreate.Rows.Count + " Newly Modified: " + data.Rows.Count, count = (dtacreate.Rows.Count + data.Rows.Count) };
                }
                else if (resultcodecreate == 1)
                {

                    return new response { respcode = 1, respmsg = "Successfully Created New Data . Count : " + dtacreate.Rows.Count };
                }
                else if (resultcodemod == 1)
                {
                    return new response { respcode = 1, respmsg = "Successfully Modified New Data . Count : " + data.Rows.Count };
                }
                con.Close();
            }


            return new response { respcode = 101 };

        }
    }


    public response getdatabranches(String modified, String created)
    {
        Int32 ZoneCode = 0;
        String AreaCode = string.Empty;
        String BranchCode = string.Empty;
        String BranchName = string.Empty;
        String Address = string.Empty;
        String TINNo = string.Empty;
        Int32 Status = 0;
        String ClosedDate = string.Empty;
        String CorpName = string.Empty;
        Decimal VAT = 0;
        String TelNo = string.Empty;
        String CellNo = string.Empty;
        String syscreated = string.Empty;
        String syscreator = string.Empty;
        String sysmodified = string.Empty;
        String sysmodifier = string.Empty;
        String PermitNo = string.Empty;
        String KP4CODE = string.Empty;
        String FaxNo = string.Empty;
        String Regioncode = string.Empty;
        Int32 is24Hours = 0;
        Int32 allowBeyondOffHour = 0;
        String timeFrom = string.Empty;
        String timeTo = string.Empty;
        //   Int32 ID = 0;


        String endpoint;
        String info;
        Byte[] dataresp;
        dynamic resultparse = 0;
        Int32 resultcodecreate = 0;
        Int32 resultcodemod = 0;
        String maxcreated = string.Empty;
        String maxmodified = string.Empty;
        String type = string.Empty;
        using (MySqlConnection con = dbconcloud.getConnection())
        {
            con.Open();
            using (MySqlCommand comm = con.CreateCommand())
            {
                comm.Parameters.Clear();
                comm.CommandText = "Select DATE_FORMAT(MAX(syscreated),'%Y-%m-%d %T') as maxcreated, DATE_FORMAT(MAX(sysmodified),'%Y-%m-%d %T') as maxmodified FROM `kpusers`.`branches`;";
                MySqlDataReader rdr = comm.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    maxmodified = rdr["maxmodified"].ToString();
                    maxcreated = rdr["maxcreated"].ToString();
                    rdr.Close();
                }
                else
                {
                    return new response { respcode = 0, respmsg = "Not Found!" };
                }
                if (maxcreated == created && maxmodified == modified)
                {
                    return new response { respcode = 1, respmsg = "Datas are already synch!" };

                }

                // mlbranchestations newly created 
                comm.Parameters.Clear();
                comm.CommandText = "Select *, DATE_FORMAT(syscreated,'%Y-%m-%d %T') as creaty, DATE_FORMAT(sysmodified,'%Y-%m-%d %T') as modi  From `kpusers`.`branches` where syscreated > @created ORDER BY creaty ASC;";
                comm.Parameters.AddWithValue("created", created);
                MySqlDataAdapter adptcreate = new MySqlDataAdapter();
                DataTable dtacreate = new DataTable("creatuser");
                DataSet dtasetcreate = new DataSet();
                adptcreate.SelectCommand = comm;
                adptcreate.Fill(dtasetcreate);
                dtacreate = dtasetcreate.Tables[0];
                type = "Create";
                for (int x = 0; x < dtacreate.Rows.Count; x++)
                {
                    // ID = Convert.ToInt32(dtacreate.Rows[x]["ID"]);
                    ZoneCode = Convert.ToInt32(dtacreate.Rows[x]["ZoneCode"]);
                    AreaCode = dtacreate.Rows[x]["AreaCode"].ToString();
                    BranchCode = dtacreate.Rows[x]["BranchCode"].ToString();
                    BranchName = dtacreate.Rows[x]["BranchName"].ToString();
                    Address = dtacreate.Rows[x]["Address"].ToString();
                    TINNo = dtacreate.Rows[x]["TINNo"].ToString();
                    Status = Convert.ToInt32(dtacreate.Rows[x]["Status"]);
                    ClosedDate = dtacreate.Rows[x]["ClosedDate"].ToString();
                    CorpName = dtacreate.Rows[x]["CorpName"].ToString();
                    VAT = Convert.ToDecimal(dtacreate.Rows[x]["VAT"]);
                    TelNo = dtacreate.Rows[x]["TelNo"].ToString();
                    CellNo = dtacreate.Rows[x]["Cellno"].ToString();

                    syscreator = dtacreate.Rows[x]["syscreator"].ToString();
                    syscreated = dtacreate.Rows[x]["creaty"].ToString();
                    sysmodifier = dtacreate.Rows[x]["sysmodifier"].ToString();
                    sysmodified = dtacreate.Rows[x]["modi"].ToString();

                    PermitNo = dtacreate.Rows[x]["PermitNo"].ToString();
                    KP4CODE = dtacreate.Rows[x]["KP4CODE"].ToString();
                    FaxNo = dtacreate.Rows[x]["FaxNo"].ToString();
                    Regioncode = String.IsNullOrEmpty(dtacreate.Rows[x]["Regioncode"].ToString()) ? null : dtacreate.Rows[x]["Regioncode"].ToString();
                    is24Hours = Convert.ToInt32(dtacreate.Rows[x]["is24Hours"]);
                    allowBeyondOffHour = Convert.ToInt32(dtacreate.Rows[x]["allowBeyondOffHour"]);
                    timeFrom = dtacreate.Rows[x]["timeFrom"].ToString();
                    timeTo = dtacreate.Rows[x]["timeTo"].ToString();




                    if (sysmodified == string.Empty)
                    {
                        sysmodified = null;
                    }
                    if (syscreated == string.Empty)
                    {
                        syscreated = null;
                    }
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

                    endpoint = NetworkServicePath + "/savebranches";
                    info = "{\"ZoneCode\":\"" + ZoneCode + "\",\"AreaCode\":\"" + AreaCode + "\", \"BranchCode\":\"" + BranchCode + "\", \"BranchName\":\"" + BranchName + "\", \"Address\":\"" + Address +
                        "\", \"TINNo\":\"" + TINNo + "\", \"Status\":\"" + Status + "\", \"ClosedDate\":\"" + ClosedDate + "\" , \"CorpName\":\"" + CorpName + "\", \"VAT\":\"" + VAT + "\", \"TelNo\":\"" + TelNo + "\", \"CellNo\":\"" + CellNo +
                        "\", \"syscreator\":\"" + syscreator + "\", \"syscreated\":\"" + syscreated + "\", \"sysmodifier\":\"" + sysmodifier + "\", \"sysmodified\":\"" + sysmodified + "\", \"PermitNo\":\"" + PermitNo + "\" ,\"KP4CODE\":\"" + KP4CODE +
                        "\", \"FaxNo\":\"" + FaxNo + "\", \"Regioncode\":\"" + Regioncode + "\" , \"is24Hours\":\"" + is24Hours + "\", \"allowBeyondOffHour\":\"" + allowBeyondOffHour + "\" , \"timeFrom\":\"" + timeFrom + "\" ,\"timeTo\":\"" + timeTo + "\", \"type\":\"" + type + "\"}";
                    var uri = new Uri(endpoint);
                    dataresp = Encoding.UTF8.GetBytes(info);
                    String resp = SendRequest(uri, dataresp, "application/json", "POST");
                    resultparse = JObject.Parse(resp);
                    resultcodecreate = resultparse.respcode;
                    if (resultcodecreate == 0)
                    {
                        return new response { respcode = 0, respmsg = resultparse.respmsg };
                    }
                }


                comm.CommandText = "Select *, DATE_FORMAT(syscreated,'%Y-%m-%d %T') as creaty, DATE_FORMAT(sysmodified,'%Y-%m-%d %T') as modi From `kpusers`.`branches` where sysmodified > @modified ORDER BY modi ASC;";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("modified", modified);
                MySqlDataAdapter adptr = new MySqlDataAdapter();
                adptr.SelectCommand = comm;
                DataTable data = new DataTable("users");
                DataSet dtaset = new DataSet();
                adptr.Fill(dtaset);
                data = dtaset.Tables[0];
                type = "Modify";
                for (int x = 0; x < data.Rows.Count; x++)
                {

                    ZoneCode = Convert.ToInt32(data.Rows[x]["ZoneCode"]);
                    AreaCode = data.Rows[x]["AreaCode"].ToString();
                    BranchCode = data.Rows[x]["BranchCode"].ToString();
                    BranchName = data.Rows[x]["BranchName"].ToString();
                    Address = data.Rows[x]["Address"].ToString();
                    TINNo = data.Rows[x]["TINNo"].ToString();
                    Status = Convert.ToInt32(data.Rows[x]["Status"]);
                    ClosedDate = data.Rows[x]["ClosedDate"].ToString();
                    CorpName = data.Rows[x]["CorpName"].ToString();
                    VAT = Convert.ToDecimal(data.Rows[x]["VAT"]);
                    TelNo = data.Rows[x]["TelNo"].ToString();
                    CellNo = data.Rows[x]["Cellno"].ToString();

                    syscreator = data.Rows[x]["syscreator"].ToString();
                    syscreated = data.Rows[x]["creaty"].ToString();
                    sysmodifier = data.Rows[x]["sysmodifier"].ToString();
                    sysmodified = data.Rows[x]["modi"].ToString();

                    PermitNo = data.Rows[x]["PermitNo"].ToString();
                    KP4CODE = data.Rows[x]["KP4CODE"].ToString();
                    FaxNo = data.Rows[x]["FaxNo"].ToString();
                    Regioncode = String.IsNullOrEmpty(data.Rows[x]["Regioncode"].ToString()) ? null : data.Rows[x]["Regioncode"].ToString();
                    is24Hours = Convert.ToInt32(data.Rows[x]["is24Hours"]);
                    allowBeyondOffHour = Convert.ToInt32(data.Rows[x]["allowBeyondOffHour"]);
                    timeFrom = data.Rows[x]["timeFrom"].ToString();
                    timeTo = data.Rows[x]["timeTo"].ToString();



                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

                    endpoint = NetworkServicePath + "/savebranches";
                    info = "{\"ZoneCode\":\"" + ZoneCode + "\",\"AreaCode\":\"" + AreaCode + "\", \"BranchCode\":\"" + BranchCode + "\", \"BranchName\":\"" + BranchName + "\", \"Address\":\"" + Address +
                        "\", \"TINNo\":\"" + TINNo + "\", \"Status\":\"" + Status + "\", \"ClosedDate\":\"" + ClosedDate + "\" , \"CorpName\":\"" + CorpName + "\", \"VAT\":\"" + VAT + "\", \"TelNo\":\"" + TelNo + "\", \"CellNo\":\"" + CellNo +
                        "\", \"syscreator\":\"" + syscreator + "\", \"syscreated\":\"" + syscreated + "\", \"sysmodifier\":\"" + sysmodifier + "\", \"sysmodified\":\"" + sysmodified + "\", \"PermitNo\":\"" + PermitNo + "\" ,\"KP4CODE\":\"" + KP4CODE +
                        "\", \"FaxNo\":\"" + FaxNo + "\", \"Regioncode\":\"" + Regioncode + "\" , \"is24Hours\":\"" + is24Hours + "\", \"allowBeyondOffHour\":\"" + allowBeyondOffHour + "\" , \"timeFrom\":\"" + timeFrom + "\" ,\"timeTo\":\"" + timeTo + "\", \"type\":\"" + type + "\"}";
                    var uri = new Uri(endpoint);
                    dataresp = Encoding.UTF8.GetBytes(info);
                    String resp = SendRequest(uri, dataresp, "application/json", "POST");
                    resultparse = JObject.Parse(resp);
                    resultcodemod = resultparse.respcode;
                    if (resultcodemod == 0)
                    {
                        return new response { respcode = 0, respmsg = "Error in updating branches." };
                    }

                }

                if (resultcodecreate == 1 && resultcodemod == 1)
                {
                    return new response { respcode = 1, respmsg = "Successfully migrated data . Newly created: " + dtacreate.Rows.Count + " Newly Modified: " + data.Rows.Count, count = (dtacreate.Rows.Count + data.Rows.Count) };
                }
                else if (resultcodecreate == 1)
                {

                    return new response { respcode = 1, respmsg = "Successfully Created New Data . Count : " + dtacreate.Rows.Count };
                }
                else if (resultcodemod == 1)
                {
                    return new response { respcode = 1, respmsg = "Successfully Modified New Data . Count : " + data.Rows.Count };
                }
                con.Close();
            }


            return new response { respcode = 101 };

        }
    }

    //-----------------------REGIONS------------

    public response getdataregion(String modified, String created)
    {
        Int32 ZoneCode = 0;
        Int32 RegionCode = 0;
        String RegionName = string.Empty;


        String syscreated = string.Empty;
        String syscreator = string.Empty;
        String sysmodified = string.Empty;
        String sysmodifier = string.Empty;

        //   Int32 ID = 0;


        String endpoint;
        String info;
        Byte[] dataresp;
        dynamic resultparse = 0;
        Int32 resultcodecreate = 0;
        Int32 resultcodemod = 0;
        String maxcreated = string.Empty;
        String maxmodified = string.Empty;
        String type = string.Empty;
        using (MySqlConnection con = dbconcloud.getConnection())
        {
            con.Open();
            using (MySqlCommand comm = con.CreateCommand())
            {
                comm.Parameters.Clear();
                comm.CommandText = "Select DATE_FORMAT(MAX(syscreated),'%Y-%m-%d %T') as maxcreated, DATE_FORMAT(MAX(sysmodified),'%Y-%m-%d %T') as maxmodified FROM `kpusers`.`region`;";
                MySqlDataReader rdr = comm.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    maxmodified = rdr["maxmodified"].ToString();
                    maxcreated = rdr["maxcreated"].ToString();
                    rdr.Close();
                }
                else
                {
                    return new response { respcode = 0, respmsg = "Not Found!" };
                }
                if (maxcreated == created && maxmodified == modified)
                {
                    return new response { respcode = 1, respmsg = "Datas are already synch!" };

                }

                // mlbranchestations newly created 
                comm.Parameters.Clear();
                comm.CommandText = "Select *, DATE_FORMAT(syscreated,'%Y-%m-%d %T') as creaty, DATE_FORMAT(sysmodified,'%Y-%m-%d %T') as modi  From `kpusers`.`region` where syscreated > @created ORDER BY creaty ASC;";
                comm.Parameters.AddWithValue("created", created);
                MySqlDataAdapter adptcreate = new MySqlDataAdapter();
                DataTable dtacreate = new DataTable("creatuser");
                DataSet dtasetcreate = new DataSet();
                adptcreate.SelectCommand = comm;
                adptcreate.Fill(dtasetcreate);
                dtacreate = dtasetcreate.Tables[0];
                type = "Create";
                for (int x = 0; x < dtacreate.Rows.Count; x++)
                {
                    // ID = Convert.ToInt32(dtacreate.Rows[x]["ID"]);
                    ZoneCode = Convert.ToInt32(dtacreate.Rows[x]["ZoneCode"]);
                    RegionCode = Convert.ToInt32(dtacreate.Rows[x]["RegionCode"]);
                    RegionName = dtacreate.Rows[x]["RegionName"].ToString();
                    syscreator = dtacreate.Rows[x]["syscreator"].ToString();
                    syscreated = dtacreate.Rows[x]["creaty"].ToString();
                    sysmodifier = dtacreate.Rows[x]["sysmodifier"].ToString();
                    sysmodified = dtacreate.Rows[x]["modi"].ToString();


                    if (sysmodified == string.Empty)
                    {
                        sysmodified = null;
                    }
                    if (syscreated == string.Empty)
                    {
                        syscreated = null;
                    }
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

                    endpoint = NetworkServicePath + "/saveregion";
                    info = "{\"RegionCode\":\"" + RegionCode + "\",\"RegionName\":\"" + RegionName + "\",\"ZoneCode\":\"" + ZoneCode + "\", \"syscreator\":\"" + syscreator + "\", \"syscreated\":\"" + syscreated + "\", \"sysmodifier\":\"" + sysmodifier + "\", \"sysmodified\":\"" + sysmodified + "\", \"type\":\"" + type + "\"}";
                    var uri = new Uri(endpoint);
                    dataresp = Encoding.UTF8.GetBytes(info);
                    String resp = SendRequest(uri, dataresp, "application/json", "POST");
                    resultparse = JObject.Parse(resp);
                    resultcodecreate = resultparse.respcode;
                    if (resultcodecreate == 0)
                    {
                        return new response { respcode = 0, respmsg = resultparse.respmsg };
                    }
                }


                comm.CommandText = "Select *, DATE_FORMAT(sysmodified,'%Y-%m-%d %T') as modi, DATE_FORMAT(syscreated,'%Y-%m-%d %T') as creaty From `kpusers`.`region` where sysmodified > @modified ORDER BY modi ASC;";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("modified", modified);
                MySqlDataAdapter adptr = new MySqlDataAdapter();
                adptr.SelectCommand = comm;
                DataTable data = new DataTable("users");
                DataSet dtaset = new DataSet();
                adptr.Fill(dtaset);
                data = dtaset.Tables[0];
                type = "Modify";
                for (int x = 0; x < data.Rows.Count; x++)
                {

                    ZoneCode = Convert.ToInt32(data.Rows[x]["ZoneCode"]);
                    RegionCode = Convert.ToInt32(data.Rows[x]["RegionCode"]);
                    RegionName = data.Rows[x]["RegionName"].ToString();
                    syscreator = data.Rows[x]["syscreator"].ToString();
                    syscreated = data.Rows[x]["creaty"].ToString();
                    sysmodifier = data.Rows[x]["sysmodifier"].ToString();
                    sysmodified = data.Rows[x]["modi"].ToString();


                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

                    endpoint = NetworkServicePath + "/saveregion";
                    info = "{\"RegionCode\":\"" + RegionCode + "\",\"RegionName\":\"" + RegionName + "\",\"ZoneCode\":\"" + ZoneCode + "\", \"syscreator\":\"" + syscreator + "\", \"syscreated\":\"" + syscreated + "\", \"sysmodifier\":\"" + sysmodifier + "\", \"sysmodified\":\"" + sysmodified + "\", \"type\":\"" + type + "\"}";
                    var uri = new Uri(endpoint);
                    dataresp = Encoding.UTF8.GetBytes(info);
                    String resp = SendRequest(uri, dataresp, "application/json", "POST");
                    resultparse = JObject.Parse(resp);
                    resultcodemod = resultparse.respcode;
                    if (resultcodemod == 0)
                    {
                        return new response { respcode = 0, respmsg = resultparse.respmsg };
                    }

                }

                if (resultcodecreate == 1 && resultcodemod == 1)
                {
                    return new response { respcode = 1, respmsg = "Successfully migrated data . Newly created: " + dtacreate.Rows.Count + " Newly Modified: " + data.Rows.Count, count = (dtacreate.Rows.Count + data.Rows.Count) };
                }
                else if (resultcodecreate == 1)
                {

                    return new response { respcode = 1, respmsg = "Successfully Created New Data . Count : " + dtacreate.Rows.Count };
                }
                else if (resultcodemod == 1)
                {
                    return new response { respcode = 1, respmsg = "Successfully Modified New Data . Count : " + data.Rows.Count };
                }
                con.Close();
            }


            return new response { respcode = 101 };

        }
    }

    //----------------------SYSUSERROLES---------------------

    public response getdatasysuserroles(String modified, String created)
    {
        Int32 ResourceID = 0;
        Int32 Type2 = 0;
        String Role = string.Empty;
        Int32 ZoneCode = 0;
        String syscreated = string.Empty;
        String syscreator = string.Empty;
        String sysmodified = string.Empty;
        String sysmodifier = string.Empty;

        //   Int32 ID = 0;


        String endpoint;
        String info;
        Byte[] dataresp;
        dynamic resultparse = 0;
        Int32 resultcodecreate = 0;
        Int32 resultcodemod = 0;
        String maxcreated = string.Empty;
        String maxmodified = string.Empty;
        String agenttype = string.Empty;
        try
        {


            using (MySqlConnection con = dbconcloud.getConnection())
            {
                try
                {


                    using (MySqlCommand comm = con.CreateCommand())
                    {
                        con.Open();
                        comm.Parameters.Clear();
                        comm.CommandText = "Select DATE_FORMAT(MAX(syscreated),'%Y-%m-%d %T') as maxcreated, DATE_FORMAT(MAX(sysmodified),'%Y-%m-%d %T') as maxmodified FROM `kpusers`.`sysuserroles`;";
                        MySqlDataReader rdr = comm.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            rdr.Read();
                            maxmodified = rdr["maxmodified"].ToString();
                            maxcreated = rdr["maxcreated"].ToString();
                            rdr.Close();
                        }
                        else
                        {
                            return new response { respcode = 0, respmsg = "Not Found!" };
                        }
                        if (maxcreated == created && maxmodified == modified)
                        {
                            return new response { respcode = 1, respmsg = "Datas are already synch!" };

                        }

                        // mlbranchestations newly created 

                        comm.Parameters.Clear();
                        comm.CommandText = "Select *, DATE_FORMAT(syscreated,'%Y-%m-%d %T') as creaty, DATE_FORMAT(sysmodified,'%Y-%m-%d %T') as modi  From `kpusers`.`sysuserroles` where syscreated > @created ORDER BY creaty ASC;";
                        comm.Parameters.AddWithValue("created", created);
                        MySqlDataAdapter adptcreate = new MySqlDataAdapter();
                        DataTable dtacreate = new DataTable("creatuser");
                        DataSet dtasetcreate = new DataSet();
                        adptcreate.SelectCommand = comm;
                        adptcreate.Fill(dtasetcreate);
                        dtacreate = dtasetcreate.Tables[0];
                        agenttype = "Create";
                        for (int x = 0; x < dtacreate.Rows.Count; x++)
                        {
                            // ID = Convert.ToInt32(dtacreate.Rows[x]["ID"]);
                            ZoneCode = Convert.ToInt32(dtacreate.Rows[x]["ZoneCode"]);
                            ResourceID = Convert.ToInt32(dtacreate.Rows[x]["ResourceID"]);
                            Type2 = Convert.ToInt32(dtacreate.Rows[x]["Type"]);
                            Role = dtacreate.Rows[x]["Role"].ToString();
                            syscreator = dtacreate.Rows[x]["syscreator"].ToString();
                            syscreated = dtacreate.Rows[x]["creaty"].ToString();
                            sysmodifier = dtacreate.Rows[x]["sysmodifier"].ToString();
                            sysmodified = dtacreate.Rows[x]["modi"].ToString();


                            if (sysmodified == string.Empty)
                            {
                                sysmodified = null;
                            }
                            if (syscreated == string.Empty)
                            {
                                syscreated = null;
                            }
                            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

                            endpoint = NetworkServicePath + "/savesysuserroles";
                            info = "{\"ResourceID\":\"" + ResourceID + "\",\"Type\":\"" + Type2 + "\",\"Role\":\"" + Role + "\",\"ZoneCode\":\"" + ZoneCode + "\", \"syscreator\":\"" + syscreator + "\", \"syscreated\":\"" + syscreated + "\", \"sysmodifier\":\"" + sysmodifier + "\", \"sysmodified\":\"" + sysmodified + "\", \"agenttype\":\"" + agenttype + "\"}";
                            var uri = new Uri(endpoint);
                            dataresp = Encoding.UTF8.GetBytes(info);
                            String resp = SendRequest(uri, dataresp, "application/json", "POST");
                            resultparse = JObject.Parse(resp);
                            resultcodecreate = resultparse.respcode;
                            if (resultcodecreate == 0)
                            {
                                return new response { respcode = 0, respmsg = resultparse.respmsg };
                            }
                        }


                        comm.CommandText = "Select *, DATE_FORMAT(sysmodified,'%Y-%m-%d %T') as modi From `kpusers`.`sysuserroles` where sysmodified > @modified ORDER BY modi ASC;";
                        comm.Parameters.Clear();
                        comm.Parameters.AddWithValue("modified", modified);
                        MySqlDataAdapter adptr = new MySqlDataAdapter();
                        adptr.SelectCommand = comm;
                        DataTable data = new DataTable("users");
                        DataSet dtaset = new DataSet();
                        adptr.Fill(dtaset);
                        data = dtaset.Tables[0];
                        agenttype = "Modify";
                        for (int x = 0; x < data.Rows.Count; x++)
                        {

                            ZoneCode = Convert.ToInt32(data.Rows[x]["ZoneCode"]);
                            ResourceID = Convert.ToInt32(data.Rows[x]["ResourceID"]);
                            Type2 = Convert.ToInt32(data.Rows[x]["Type"]);
                            Role = data.Rows[x]["Role"].ToString();
                            syscreator = data.Rows[x]["syscreator"].ToString();
                            syscreated = data.Rows[x]["creaty"].ToString();
                            sysmodifier = data.Rows[x]["sysmodifier"].ToString();
                            sysmodified = data.Rows[x]["modi"].ToString();


                            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

                            endpoint = NetworkServicePath + "/savesysuserroles";
                            info = "{\"ResourceID\":\"" + ResourceID + "\",\"Type\":\"" + Type2 + "\",\"Role\":\"" + Role + "\",\"ZoneCode\":\"" + ZoneCode + "\", \"syscreator\":\"" + syscreator + "\", \"syscreated\":\"" + syscreated + "\", \"sysmodifier\":\"" + sysmodifier + "\", \"sysmodified\":\"" + sysmodified + "\", \"agenttype\":\"" + agenttype + "\"}";
                            var uri = new Uri(endpoint);
                            dataresp = Encoding.UTF8.GetBytes(info);
                            String resp = SendRequest(uri, dataresp, "application/json", "POST");
                            resultparse = JObject.Parse(resp);
                            resultcodemod = resultparse.respcode;
                            if (resultcodemod == 0)
                            {
                                return new response { respcode = 0, respmsg = resultparse.respmsg };
                            }

                        }

                        if (resultcodecreate == 1 && resultcodemod == 1)
                        {
                            return new response { respcode = 1, respmsg = "Successfully migrated data . Newly created: " + dtacreate.Rows.Count + " Newly Modified: " + data.Rows.Count, count = (dtacreate.Rows.Count + data.Rows.Count) };
                        }
                        else if (resultcodecreate == 1)
                        {

                            return new response { respcode = 1, respmsg = "Successfully Created New Data . Count : " + dtacreate.Rows.Count };
                        }
                        else if (resultcodemod == 1)
                        {
                            return new response { respcode = 1, respmsg = "Successfully Modified New Data . Count : " + data.Rows.Count };
                        }
                        con.Close();

                    }

                }
                catch (Exception ex)
                {

                    return new response { respcode = 101, respmsg = ex.ToString() };
                }

                return new response { respcode = 1, respmsg = "Cloud database is more up to date than Network" };

            }
        }
        catch (Exception ex)
        {

            return new response { respcode = 101, respmsg = ex.ToString() };
        }
    }

    //----------------ROLES---------------------------------------



    public response getdataroles(String modified, String created)
    {
        Int32 RoleType = 0;
        String Description;
        String Role = string.Empty;
        String syscreated = string.Empty;
        String syscreator = string.Empty;
        String sysmodified = string.Empty;
        String sysmodifier = string.Empty;

        //   Int32 ID = 0;


        String endpoint;
        String info;
        Byte[] dataresp;
        dynamic resultparse = 0;
        Int32 resultcodecreate = 0;
        Int32 resultcodemod = 0;
        String maxcreated = string.Empty;
        String maxmodified = string.Empty;
        String type = string.Empty;
        using (MySqlConnection con = dbconcloud.getConnection())
        {
            con.Open();
            using (MySqlCommand comm = con.CreateCommand())
            {
                comm.Parameters.Clear();
                comm.CommandText = "Select DATE_FORMAT(MAX(syscreated),'%Y-%m-%d %T') as maxcreated, DATE_FORMAT(MAX(sysmodified),'%Y-%m-%d %T') as maxmodified FROM `kpusers`.`roles`;";
                MySqlDataReader rdr = comm.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    maxmodified = rdr["maxmodified"].ToString();
                    maxcreated = rdr["maxcreated"].ToString();
                    rdr.Close();
                }
                else
                {
                    return new response { respcode = 0, respmsg = "Not Found!" };
                }
                if (maxcreated == created && maxmodified == modified)
                {
                    return new response { respcode = 1, respmsg = "Datas are already synch!" };

                }


                // mlbranchestations newly created 
                comm.Parameters.Clear();
                comm.CommandText = "Select *, DATE_FORMAT(syscreated,'%Y-%m-%d %T') as creaty, DATE_FORMAT(sysmodified,'%Y-%m-%d %T') as modi  From `kpusers`.`roles` where syscreated > @created ORDER BY creaty ASC;";
                comm.Parameters.AddWithValue("created", created);
                MySqlDataAdapter adptcreate = new MySqlDataAdapter();
                DataTable dtacreate = new DataTable("creatuser");
                DataSet dtasetcreate = new DataSet();
                adptcreate.SelectCommand = comm;
                adptcreate.Fill(dtasetcreate);
                dtacreate = dtasetcreate.Tables[0];
                type = "Create";
                for (int x = 0; x < dtacreate.Rows.Count; x++)
                {


                    RoleType = Convert.ToInt32(dtacreate.Rows[x]["RoleType"].ToString());
                    Description = dtacreate.Rows[x]["Description"].ToString();
                    Role = dtacreate.Rows[x]["Role"].ToString();
                    syscreator = dtacreate.Rows[x]["syscreator"].ToString();
                    syscreated = dtacreate.Rows[x]["creaty"].ToString();
                    sysmodifier = dtacreate.Rows[x]["sysmodifier"].ToString();
                    sysmodified = dtacreate.Rows[x]["modi"].ToString();


                    if (sysmodified == string.Empty)
                    {
                        sysmodified = null;
                    }
                    if (syscreated == string.Empty)
                    {
                        syscreated = null;
                    }
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

                    endpoint = NetworkServicePath + "/saveroles";
                    info = "{\"Role\":\"" + Role + "\",\"Description\":\"" + Description + "\",\"RoleType\":\"" + RoleType + "\", \"syscreator\":\"" + syscreator + "\", \"syscreated\":\"" + syscreated + "\", \"sysmodified\":\"" + sysmodified + "\", \"sysmodifier\":\"" + sysmodifier + "\", \"type\":\"" + type + "\"}";
                    // info = "{\"ric\":\"" + Role + "\",\"ryan\":\"" + Description + "\"}";
                    var uri = new Uri(endpoint);
                    dataresp = Encoding.UTF8.GetBytes(info);
                    String resp = SendRequest(uri, dataresp, "application/json", "POST");
                    resultparse = JObject.Parse(resp);
                    resultcodecreate = resultparse.respcode;
                    if (resultcodecreate == 0)
                    {
                        return new response { respcode = 0, respmsg = resultparse.respmsg };
                    }
                }


                comm.CommandText = "Select *, DATE_FORMAT(sysmodified,'%Y-%m-%d %T') as modi,DATE_FORMAT(syscreated,'%Y-%m-%d %T') as creaty From `kpusers`.`roles` where sysmodified > @modified ORDER BY modi ASC;";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("modified", modified);
                MySqlDataAdapter adptr = new MySqlDataAdapter();
                adptr.SelectCommand = comm;
                DataTable data = new DataTable("users");
                DataSet dtaset = new DataSet();
                adptr.Fill(dtaset);
                data = dtaset.Tables[0];
                type = "Modify";
                for (int x = 0; x < data.Rows.Count; x++)
                {

                    RoleType = Convert.ToInt32(data.Rows[x]["RoleType"].ToString());
                    Description = data.Rows[x]["Description"].ToString();
                    Role = data.Rows[x]["Role"].ToString();
                    syscreator = data.Rows[x]["syscreator"].ToString();
                    syscreated = data.Rows[x]["creaty"].ToString();
                    sysmodifier = data.Rows[x]["sysmodifier"].ToString();
                    sysmodified = data.Rows[x]["modi"].ToString();


                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });


                    endpoint = NetworkServicePath + "/saveroles";
                    //   info = "{\"ric\":\"" + Role + "\",\"ryan\":\"" + Description + "\"}";
                    info = "{\"Role\":\"" + Role + "\",\"Description\":\"" + Description + "\",\"RoleType\":\"" + RoleType + "\", \"syscreator\":\"" + syscreator + "\", \"syscreated\":\"" + syscreated + "\", \"sysmodified\":\"" + sysmodified + "\", \"sysmodifier\":\"" + sysmodifier + "\", \"type\":\"" + type + "\"}";
                    var uri = new Uri(endpoint);
                    dataresp = Encoding.UTF8.GetBytes(info);
                    String resp = SendRequest(uri, dataresp, "application/json", "POST");
                    resultparse = JObject.Parse(resp);
                    resultcodemod = resultparse.respcode;
                    if (resultcodemod == 0)
                    {
                        return new response { respcode = 0, respmsg = resultparse.respmsg };
                    }

                }

                if (resultcodecreate == 1 && resultcodemod == 1)
                {
                    return new response { respcode = 1, respmsg = "Successfully migrated data . Newly created: " + dtacreate.Rows.Count + " Newly Modified: " + data.Rows.Count, count = (dtacreate.Rows.Count + data.Rows.Count) };
                }
                else if (resultcodecreate == 1)
                {

                    return new response { respcode = 1, respmsg = "Successfully Created New Data . Count : " + dtacreate.Rows.Count };
                }
                else if (resultcodemod == 1)
                {
                    return new response { respcode = 1, respmsg = "Successfully Modified New Data . Count : " + data.Rows.Count };
                }
                con.Close();
            }


            return new response { respcode = 1, respmsg = "Cloud is more up to date than Network" };

        }
    }

    // ZONECODES -------------------------------------------

    public response getdatazonecodes(String modified, String created)
    {
        Int32 ZoneCode = 0;
        String ZoneName = string.Empty;
        String syscreated = string.Empty;
        String syscreator = string.Empty;
        String sysmodified = string.Empty;
        String sysmodifier = string.Empty;



        String endpoint;
        String info;
        Byte[] dataresp;
        dynamic resultparse = 0;
        Int32 resultcodecreate = 0;
        Int32 resultcodemod = 0;

        String maxcreated = string.Empty;
        String maxmodified = string.Empty;
        String agenttype = string.Empty;
        using (MySqlConnection con = dbconcloud.getConnection())
        {
            con.Open();
            using (MySqlCommand comm = con.CreateCommand())
            {
                comm.Parameters.Clear();
                comm.CommandText = "Select DATE_FORMAT(MAX(syscreated),'%Y-%m-%d %T') as maxcreated,DATE_FORMAT(MAX(sysmodified),'%Y-%m-%d %T') as maxmodified FROM `kpusers`.`zonecodes`;";
                MySqlDataReader rdr = comm.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    maxcreated = rdr["maxcreated"].ToString();
                    maxmodified = rdr["maxmodified"].ToString();
                    rdr.Close();
                }
                else
                {
                    return new response { respcode = 0, respmsg = "Not Found!" };
                }
                if (maxcreated == created && maxmodified == modified)
                {
                    return new response { respcode = 1, respmsg = "Datas are already synch!" };

                }

                // mlbranchestations newly created 
                comm.Parameters.Clear();
                comm.CommandText = "Select *, DATE_FORMAT(syscreated,'%Y-%m-%d %T') as creaty, DATE_FORMAT(sysmodified,'%Y-%m-%d %T') as modi  From `kpusers`.`zonecodes` where syscreated > @created ORDER BY creaty ASC;";
                comm.Parameters.AddWithValue("created", created);
                MySqlDataAdapter adptcreate = new MySqlDataAdapter();
                DataTable dtacreate = new DataTable("creatuser");
                DataSet dtasetcreate = new DataSet();
                adptcreate.SelectCommand = comm;
                adptcreate.Fill(dtasetcreate);
                dtacreate = dtasetcreate.Tables[0];
                agenttype = "Create";
                for (int x = 0; x < dtacreate.Rows.Count; x++)
                {
                    // ID = Convert.ToInt32(dtacreate.Rows[x]["ID"]);
                    ZoneCode = Convert.ToInt32(dtacreate.Rows[x]["ZoneCode"]);
                    ZoneName = dtacreate.Rows[x]["ZoneName"].ToString();

                    syscreator = dtacreate.Rows[x]["syscreator"].ToString();
                    syscreated = dtacreate.Rows[x]["creaty"].ToString();
                    sysmodifier = dtacreate.Rows[x]["sysmodifier"].ToString();
                    sysmodified = dtacreate.Rows[x]["modi"].ToString();


                    if (sysmodified == string.Empty)
                    {
                        sysmodified = null;
                    }
                    if (syscreated == string.Empty)
                    {
                        syscreated = null;
                    }
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

                    // endpoint = NetworkServicePath + "/savezonecodes";
                    endpoint = NetworkServicePath + "/savezonecodes";
                    info = "{\"ZoneCode\":\"" + ZoneCode + "\",\"ZoneName\":\"" + ZoneName + "\", \"syscreator\":\"" + syscreator + "\", \"syscreated\":\"" + syscreated + "\", \"sysmodifier\":\"" + sysmodifier + "\", \"sysmodified\":\"" + sysmodified + "\",\"agenttype\":\"" + agenttype + "\"}";
                    var uri = new Uri(endpoint);
                    dataresp = Encoding.UTF8.GetBytes(info);
                    String resp = SendRequest(uri, dataresp, "application/json", "POST");
                    resultparse = JObject.Parse(resp);
                    resultcodecreate = resultparse.respcode;
                    if (resultcodecreate == 0)
                    {
                        return new response { respcode = 0, respmsg = resultparse.respmsg };
                    }
                }

                comm.CommandText = "Select *, DATE_FORMAT(sysmodified,'%Y-%m-%d %T') as modi, DATE_FORMAT(syscreated,'%Y-%m-%d %T') as creaty From `kpusers`.`zonecodes` where sysmodified > @modified ORDER BY modi ASC;";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("modified", modified);
                MySqlDataAdapter adptr = new MySqlDataAdapter();
                adptr.SelectCommand = comm;
                DataTable data = new DataTable("users");
                DataSet dtaset = new DataSet();
                adptr.Fill(dtaset);
                data = dtaset.Tables[0];
                agenttype = "Modify";
                for (int x = 0; x < data.Rows.Count; x++)
                {

                    ZoneCode = Convert.ToInt32(data.Rows[x]["ZoneCode"]);
                    ZoneName = data.Rows[x]["ZoneName"].ToString();

                    syscreator = data.Rows[x]["syscreator"].ToString();
                    syscreated = data.Rows[x]["creaty"].ToString();
                    sysmodifier = data.Rows[x]["sysmodifier"].ToString();
                    sysmodified = data.Rows[x]["modi"].ToString();


                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });


                    endpoint = NetworkServicePath + "/savezonecodes";
                    //   info = "{\"ric\":\"" + Role + "\",\"ryan\":\"" + Description + "\"}";
                    info = "{\"ZoneCode\":\"" + ZoneCode + "\",\"ZoneName\":\"" + ZoneName + "\", \"syscreator\":\"" + syscreator + "\", \"syscreated\":\"" + syscreated + "\", \"sysmodifier\":\"" + sysmodifier + "\", \"sysmodified\":\"" + sysmodified + "\",\"agenttype\":\"" + agenttype + "\"}";
                    var uri = new Uri(endpoint);
                    dataresp = Encoding.UTF8.GetBytes(info);
                    String resp = SendRequest(uri, dataresp, "application/json", "POST");
                    resultparse = JObject.Parse(resp);
                    resultcodemod = resultparse.respcode;
                    if (resultcodemod == 0)
                    {
                        return new response { respcode = 0, respmsg = resultparse.respmsg };
                    }

                }


                if (resultcodecreate == 1)
                {

                    return new response { respcode = 1, respmsg = "Successfully Created New Data . Count : " + dtacreate.Rows.Count };
                }
                if (resultcodemod == 1)
                {
                    return new response { respcode = 1, respmsg = "Successfully Modified  Data . Count : " + data.Rows.Count };
                }

            }


            return new response { respcode = 101 };

        }
    }



    //public versionsresponse getupdatedversions(double sysmenuversion,  double updatesversion,  double versions, Int32 pluginCount, Int32 assemblyCount, Int32 type)
    // {
    //     using (MySqlConnection connet = dbconcloud.getConnection())
    //     {

    //         connet.Open();
    //         using (MySqlCommand cmd = connet.CreateCommand())
    //         {
    //             versionsresponse sysmenudetails = new versionsresponse();
    //             MySqlDataAdapter adptr = new MySqlDataAdapter();
    //             DataTable table = new DataTable("version");
    //             DataSet dtaset = new DataSet();
    //             if (sysmenuversion != 0.0)
    //             {
    //                 cmd.CommandText = "SELECT * FROM kpforms.sysmenu WHERE `version` >= @sysmenuversion and `type`=@type;";
    //                 cmd.Parameters.Clear();                 
    //                 cmd.Parameters.AddWithValue("sysmenuversion", sysmenuversion);
    //                 cmd.Parameters.AddWithValue("type", type); 
    //                 adptr.SelectCommand = cmd;
    //                 adptr.Fill(dtaset);
    //                 table = dtaset.Tables[0];
    //                 List<sysmenudetails> sysmenudetail = new List<sysmenudetails>();

    //                 for (int x = 0; x < table.Rows.Count; x++)
    //                 {
    //                     sysmenudetail.Add(new sysmenudetails
    //                     {
    //                         MID = Convert.ToInt64(table.Rows[x]["MID"]),
    //                         parentID = Convert.ToInt64(table.Rows[x]["ParentID"]),
    //                         RoleCode = table.Rows[x]["RoleCode"].ToString(),
    //                         DisplayName = table.Rows[x]["DisplayName"].ToString(),
    //                         MenuIndex = Convert.ToInt64(table.Rows[x]["MenuIndex"]),
    //                         isDropDown = Convert.ToInt16(table.Rows[x]["IsDropDown"]),
    //                         PID = Convert.ToInt16(table.Rows[x]["PID"]),
    //                         version = Convert.ToDouble(table.Rows[x]["version"]),
    //                         type =Convert.ToInt16(table.Rows[x]["type"])
    //                     });
    //                 }
    //                 sysmenudetails.symenuresp = sysmenudetail;
    //                 return sysmenudetails;
    //             }
    //             else if (updatesversion !=0.0)
    //             {
    //                 String apptype = String.Empty;
    //                 if (type == 1)
    //                 {
    //                     apptype = "Client";
    //                 }
    //                 else if (type == 0)
    //                 {
    //                     apptype = "Admin";
    //                 }
    //                 cmd.CommandText = "SELECT * FROM kpforms.updates WHERE `version` >= @updatesversion AND apptype =@apptype;";
    //                 cmd.Parameters.Clear();
    //                 cmd.Parameters.AddWithValue("updatesversion", updatesversion);
    //                 cmd.Parameters.AddWithValue("apptype", apptype);
    //                 adptr.SelectCommand = cmd;
    //                 adptr.Fill(dtaset);
    //                 table = dtaset.Tables[0];
    //                 List<updatesdetails> details = new List<updatesdetails>();

    //                 for (int x = 0; x < table.Rows.Count; x++)
    //                 {
    //                     details.Add(new updatesdetails
    //                     {
    //                         filename = table.Rows[x]["filename"].ToString(),
    //                         crc = table.Rows[x]["crc"].ToString(),
    //                         apptype = table.Rows[x]["apptype"].ToString(),
    //                         version = Convert.ToDecimal(table.Rows[x]["version"])

    //                     });
    //                 }
    //                 sysmenudetails.updatesresp = details;
    //                 return sysmenudetails;
    //             }
    //             else if (versions != 0)
    //             {
    //                 cmd.CommandText = "SELECT * FROM kpforms.versions WHERE `versionno`>=@versions AND apptype =@type;";
    //                 cmd.Parameters.Clear();
    //                 cmd.Parameters.AddWithValue("versions", versions);
    //                 cmd.Parameters.AddWithValue("type", type);
    //                 adptr.SelectCommand = cmd;
    //                 adptr.Fill(dtaset);
    //                 table = dtaset.Tables[0];
    //                 List<versionsdetails> details = new List<versionsdetails>();

    //                 for (int x = 0; x < table.Rows.Count; x++)
    //                 {
    //                     details.Add(new versionsdetails
    //                     {
    //                         versionno = Convert.ToDecimal(table.Rows[x]["versionno"]),
    //                         description = table.Rows[x]["description"].ToString(),
    //                         path = table.Rows[x]["path"].ToString(),
    //                         datereleased = table.Rows[x]["datereleased"].ToString(),
    //                         AppType = Convert.ToInt16(table.Rows[x]["AppType"]),
    //                         forDowngrade = Convert.ToInt16(table.Rows[x]["forDowngrade"]),
    //                         releasetype = Convert.ToInt16(table.Rows[x]["releasetype"])

    //                     });
    //                 }
    //                 sysmenudetails.versionresp = details;
    //                 return sysmenudetails;
    //             }

    //             if (pluginCount != 0)
    //             {
    //                 cmd.CommandText = "SELECT * FROM kpforms.syspluginregistry;";
    //                 cmd.Parameters.Clear();                    
    //                 adptr.SelectCommand = cmd;
    //                 adptr.Fill(dtaset);
    //                 table = dtaset.Tables[0];
    //                 List<pluginregistrydetails> details = new List<pluginregistrydetails>();
    //                 sysmenudetails.pluginregistryresp = details;
    //                 return sysmenudetails;
    //             }

    //             if (assemblyCount != 0)
    //             {
    //                 cmd.CommandText = "SELECT * FROM kpformsglobal.sysassembly;";
    //                 cmd.Parameters.Clear();
    //                 adptr.SelectCommand = cmd;
    //                 adptr.Fill(dtaset);
    //                 table = dtaset.Tables[0];
    //                 List<sysassemblydetails> details = new List<sysassemblydetails>();
    //                 sysmenudetails.assemblyresp = details;
    //                 return sysmenudetails;
    //             }

    //             return new versionsresponse { respcode = 0 };
    //         }
    //     }
    // }


    public versionsresponse getupdatedversionsGlobal(double updatesversion, Int32 type, String datereleased)
    {
        using (MySqlConnection connet = dbconcloud.getConnection())
        {
            connet.Open();
            using (MySqlCommand cmd = connet.CreateCommand())
            {
                versionsresponse sysmenudetails = new versionsresponse();
                MySqlDataAdapter adptr = new MySqlDataAdapter();
                DataTable table = new DataTable("version");
                DataSet dtaset = new DataSet();

                if (updatesversion != 0.0)
                {
                    String apptype = String.Empty;
                    if (type == 1)
                    {
                        apptype = "Client";
                    }
                    else if (type == 0)
                    {
                        apptype = "Admin";
                    }
                    cmd.CommandText = "SELECT * FROM kpformsglobal.updates WHERE `version` >= @updatesversion AND apptype = @apptype;";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("updatesversion", updatesversion);
                    cmd.Parameters.AddWithValue("apptype", apptype);
                    adptr.SelectCommand = cmd;
                    adptr.Fill(dtaset);
                    table = dtaset.Tables[0];
                    List<updatesdetails> details = new List<updatesdetails>();

                    for (int x = 0; x < table.Rows.Count; x++)
                    {
                        details.Add(new updatesdetails
                        {
                            filename = table.Rows[x]["filename"].ToString(),
                            crc = table.Rows[x]["crc"].ToString(),
                            apptype = table.Rows[x]["apptype"].ToString(),
                            version = Convert.ToDecimal(table.Rows[x]["version"])

                        });
                    }
                    sysmenudetails.updatesresp = details;
                    return sysmenudetails;
                }
                else if (!datereleased.Equals(string.Empty))
                {
                    cmd.CommandText = "Select *, DATE_FORMAT(datereleased,'%Y-%m-%d %T') as rely  From `kpformsglobal`.`versions` where datereleased > @released ORDER BY rely ASC;";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("released", datereleased);
                    adptr.SelectCommand = cmd;
                    adptr.Fill(dtaset);
                    table = dtaset.Tables[0];
                    List<versionsdetails> details = new List<versionsdetails>();

                    for (int x = 0; x < table.Rows.Count; x++)
                    {
                        details.Add(new versionsdetails
                        {
                            versionno = Convert.ToDecimal(table.Rows[x]["versionno"]),
                            description = table.Rows[x]["description"].ToString(),
                            path = table.Rows[x]["path"].ToString(),
                            datereleased = table.Rows[x]["datereleased"].ToString(),
                            AppType = Convert.ToInt16(table.Rows[x]["AppType"]),
                            forDowngrade = Convert.ToInt16(table.Rows[x]["forDowngrade"]),
                            releasetype = Convert.ToInt16(table.Rows[x]["releasetype"])

                        });
                    }
                    sysmenudetails.versionresp = details;
                    return sysmenudetails;
                }

                return new versionsresponse { respcode = 0 };
            }
        }
    }


    #endregion

    #region Others

    private String SendRequest(Uri uri, Byte[] jsonDataBytes, String contentType, String method)
    {
        try
        {
            WebRequest req = WebRequest.Create(uri);
            req.ContentType = contentType;
            req.Method = method;
            req.ContentLength = jsonDataBytes.Length;

            Stream stream = req.GetRequestStream();
            stream.Write(jsonDataBytes, 0, jsonDataBytes.Length);
            stream.Close();

            WebResponse webresponse = req.GetResponse();
            Stream response = webresponse.GetResponseStream();

            String res = null;
            if (response != null)
            {
                var reader = new StreamReader(response);
                res = reader.ReadToEnd();
                reader.Close();
                response.Close();
            }
            return res;
        }
        catch (Exception ex)
        {
            //Kplog.Fatal(ex.ToString());
            return "Unable to process request. The system encountered some technical problem. Sorry for the inconvenience.";
        }
    }

    private string getdatereleased()
    {
        string datereleased = string.Empty;
        try
        {
            using (MySqlConnection con = dbconcloud.getConnection())
            {
                con.Open();
                using (MySqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "Select DATE_FORMAT(MAX(datereleased),'%Y-%m-%d %T') as maxreleased FROM kpformsglobal.`versions`;";
                    cmd.Parameters.Clear();
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        rdr.Read();
                        datereleased = rdr["maxreleased"].ToString();
                        rdr.Close();
                    }

                }

            }
        }
        catch (Exception ex)
        {

            return ex.ToString();
        }

        return datereleased;
    }

    #endregion
}
