using log4net;
using Microsoft.Win32;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WeighingSolution.DataLayer
{
    public class DatabaseFunctions
    {
        public static string defaultDBUser = "sa";
        public static string customDBUser = "sa";
        public static string defaultDBSaPWD = "S@user4wb";
        public static string defaultDBServer = ".\\ENDELSQLEXPRESS";
        public static string defaultDBName = "Endel_weighbridge";
        public bool IsDefaultServerExists()
        {
            bool Retval = false;
            List<string> lstInstances = new List<string>();

            RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            {
                RegistryKey instanceKey = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL", false);
                if (instanceKey != null)
                {
                    foreach (var instanceName in instanceKey.GetValueNames())
                    {
                        lstInstances.Add(instanceName);
                    }
                }
            }
            if (lstInstances.Count > 0 && lstInstances.Contains("ENDELSQLEXPRESS", StringComparer.InvariantCultureIgnoreCase))
                Retval = true;
            return Retval;
        }
        public bool IsDefaultDBExists(string serverName, int Port, string DBName, string UserID, string password)
        {
            bool Retval = false;
            try
            {
                SqlConnection tmpConn = new SqlConnection("server=" + serverName + (Port > 0 ? "," + Port.ToString() : "") + ";user id=" + UserID + ";password=" + password + ";");
                string sqlCheckDBQuery = string.Format("SELECT database_id FROM sys.databases WHERE Name = '{0}'", DBName);
                using (tmpConn)
                {
                    using (SqlCommand sqlCmd = new SqlCommand(sqlCheckDBQuery, tmpConn))
                    {
                        tmpConn.Open();
                        int DatabaseID = 0;
                        object resultObj = sqlCmd.ExecuteScalar();
                        if (resultObj != null)
                        {
                            int.TryParse(resultObj.ToString(), out DatabaseID);
                            if (DatabaseID > 0)
                                Retval = true;
                        }
                        tmpConn.Close();

                        if (Retval)
                        {
                            Add_salt(serverName, Port, DBName, UserID, password);
                            CreateMissingTable(serverName, Port, DBName, UserID, password);
                            InsertApplicationSettingData(serverName, Port, DBName, UserID, password);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return Retval;
        }
        public static DataTable LocalMSSQLExecuteReaderQuery(string sql)
        {
            string Datastring = "Data Source=" + Constant.DBSERVER + ";Initial Catalog=" + Constant.DBNAME + ";user id=" + Constant.DBUSERNAME + ";password=" + Constant.DBPASSWORD + ";MultipleActiveResultSets=True;Connection Timeout=30";
            SqlConnection conn = new SqlConnection(Datastring);


            DataTable Updatetable = new DataTable();
            try
            {
                conn.Open();
                using (SqlDataAdapter dau = new SqlDataAdapter(sql, conn))
                {
                    dau.Fill(Updatetable);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                conn.Close();
            }
            if (Updatetable != null && Updatetable.Rows.Count > 0)
            {
                return Updatetable;
            }
            else
            {
                return null;
            }
        }
        public void Add_salt(string serverName, int Port, string DBName, string UserID, string password)
        {
            string Datastring;
            if (Port > 0)
                Datastring = "Data Source=" + serverName + "," + Port + ";Initial Catalog=" + DBName + ";user id=" + UserID + ";password=" + password + ";";
            else
                Datastring = "Data Source=" + serverName + ";Initial Catalog=" + DBName + ";user id=" + UserID + ";password=" + password + ";";

            try
            {
                using (SqlConnection tempCon = new SqlConnection(Datastring))
                {
                    if (tempCon.State != ConnectionState.Open)
                        tempCon.Open();
                    var table = new DataTable();
                    using (var da = new SqlDataAdapter("SELECT * FROM sys.columns where Name = N'salt' AND Object_ID = Object_ID(N'UserMaster')", tempCon))
                    {
                        da.Fill(table);

                        if (table.Rows.Count < 1)
                        {
                            string SetSchemaSQL = WeighingSolution.DataLayer.Properties.Resources.Add_salt;
                            bool flag = ExecuteBatchNonQuery(SetSchemaSQL, new SqlConnection("server=" + serverName + (Port > 0 ? "," + Port.ToString() : "") + ";initial catalog=" + DBName + ";user id=" + UserID + ";password=" + password + ";"));

                            if (flag)
                            {
                                var Updatetable = new DataTable();
                                using (var dau = new SqlDataAdapter("SELECT * FROM UserMaster", tempCon))
                                {
                                    dau.Fill(Updatetable);

                                    if (Updatetable.Rows.Count > 0)
                                    {
                                        string updateQuery = string.Empty;

                                        for (int i = 0; i < Updatetable.Rows.Count; i++)
                                        {
                                            string UpdateUserID = Updatetable.Rows[i]["UserID"].ToString();
                                            string UpdateUserName = Updatetable.Rows[i]["UserName"].ToString();
                                            string DBPassword = Updatetable.Rows[i]["Password"].ToString();
                                            string NewPassword = CommonEncription.EncryptData(DBPassword);
                                            string NewSalt = CommonEncription.createSalt(UpdateUserName, DBPassword);
                                            if (UpdateUserName != "endelsupport")
                                                updateQuery += "UPDATE UserMaster SET Password ='" + NewPassword + "',Salt = '" + NewSalt + "'  WHERE UserID = " + UpdateUserID + ";\n  GO \n";
                                        }

                                        bool updateflag = ExecuteBatchNonQuery(updateQuery, new SqlConnection("server=" + serverName + (Port > 0 ? "," + Port.ToString() : "") + ";initial catalog=" + DBName + ";user id=" + UserID + ";password=" + password + ";"));

                                    }
                                }
                            }
                        }
                        else
                        {
                            var Updatetable = new DataTable();
                            using (var dau = new SqlDataAdapter("SELECT * FROM UserMaster WHERE Salt IS NULL OR Salt = ' '", tempCon))
                            {
                                dau.Fill(Updatetable);

                                if (Updatetable.Rows.Count > 0)
                                {
                                    string updateQuery = string.Empty;

                                    for (int i = 0; i < Updatetable.Rows.Count; i++)
                                    {
                                        string UpdateUserID = Updatetable.Rows[i]["UserID"].ToString();
                                        string UpdateUserName = Updatetable.Rows[i]["UserName"].ToString();
                                        string DBPassword = Updatetable.Rows[i]["Password"].ToString();
                                        string DBSalt = Updatetable.Rows[i]["salt"].ToString();

                                        string NewPassword = CommonEncription.EncryptData(DBPassword);
                                        string NewSalt = CommonEncription.createSalt(UpdateUserName, DBPassword);
                                        if (UpdateUserName != "endelsupport" && string.IsNullOrEmpty(DBSalt))
                                            updateQuery += "UPDATE UserMaster SET Password ='" + NewPassword + "',Salt = '" + NewSalt + "'  WHERE UserID = " + UpdateUserID + ";\n  GO \n";
                                    }

                                    bool updateflag = ExecuteBatchNonQuery(updateQuery, new SqlConnection("server=" + serverName + (Port > 0 ? "," + Port.ToString() : "") + ";initial catalog=" + DBName + ";user id=" + UserID + ";password=" + password + ";"));

                                }
                            }
                        }
                    }
                    tempCon.Close();
                }
            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }
        }

        public void InsertApplicationSettingData(string serverName, int Port, string DBName, string UserID, string password)
        {
            string Datastring;
            if (Port > 0)
                Datastring = "Data Source=" + serverName + "," + Port + ";Initial Catalog=" + DBName + ";user id=" + UserID + ";password=" + password + ";";
            else
                Datastring = "Data Source=" + serverName + ";Initial Catalog=" + DBName + ";user id=" + UserID + ";password=" + password + ";";

            try
            {
                using (SqlConnection tempCon = new SqlConnection(Datastring))
                {
                    string SetSchemaSQL = WeighingSolution.DataLayer.Properties.Resources.ApplicationSettingData;
                    bool flag = ExecuteBatchNonQuery(SetSchemaSQL, new SqlConnection("server=" + serverName + (Port > 0 ? "," + Port.ToString() : "") + ";initial catalog=" + DBName + ";user id=" + UserID + ";password=" + password + ";"));

                }
            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }
        }

        public static bool InsertUpdateDataSettingData(string serverName, int Port, string DBName, string UserID, string password)
        {
            string Datastring;
            if (Port > 0)
                Datastring = "Data Source=" + serverName + "," + Port + ";Initial Catalog=" + DBName + ";user id=" + UserID + ";password=" + password + ";";
            else
                Datastring = "Data Source=" + serverName + ";Initial Catalog=" + DBName + ";user id=" + UserID + ";password=" + password + ";";

            try
            {
                using (SqlConnection tempCon = new SqlConnection(Datastring))
                {
                    string SetSchemaSQL = WeighingSolution.DataLayer.Properties.Resources.Update_Data;
                    bool flag = ExecuteBatchNonQuery(SetSchemaSQL, new SqlConnection("server=" + serverName + (Port > 0 ? "," + Port.ToString() : "") + ";initial catalog=" + DBName + ";user id=" + UserID + ";password=" + password + ";"));
                    return flag;
                }
            }
            catch (Exception ex)
            {
                _ = ex.Message;
                return false;
            }
        }

        public void CreateMissingTable(string serverName, int Port, string DBName, string UserID, string password)
        {
            string Datastring;
            if (Port > 0)
                Datastring = "Data Source=" + serverName + "," + Port + ";Initial Catalog=" + DBName + ";user id=" + UserID + ";password=" + password + ";";
            else
                Datastring = "Data Source=" + serverName + ";Initial Catalog=" + DBName + ";user id=" + UserID + ";password=" + password + ";";

            try
            {
                using (SqlConnection tempCon = new SqlConnection(Datastring))
                {
                    string EmailSetupSQL = WeighingSolution.DataLayer.Properties.Resources.EmailSetup;
                    bool EmailSetupflag = ExecuteBatchNonQuery(EmailSetupSQL, tempCon);

                    string CameraSetupSQL = WeighingSolution.DataLayer.Properties.Resources.CameraSetup;
                    bool CameraSetupflag = ExecuteBatchNonQuery(CameraSetupSQL, tempCon);

                    string SMSSetupSQL = WeighingSolution.DataLayer.Properties.Resources.SMSSetup;
                    bool SMSSetupflag = ExecuteBatchNonQuery(SMSSetupSQL, tempCon);

                    string BarcodeImageSQL = WeighingSolution.DataLayer.Properties.Resources.BarcodeImage;
                    bool BarcodeImageflag = ExecuteBatchNonQuery(BarcodeImageSQL, tempCon);

                    string UnmmanedSQL = WeighingSolution.DataLayer.Properties.Resources.Unmanned;
                    bool UnmmanedSQLflag = ExecuteBatchNonQuery(UnmmanedSQL, tempCon);
					
					 string InstallSyncSQL = WeighingSolution.DataLayer.Properties.Resources.InstallSync;
                    bool InstallSyncSQLflag = ExecuteBatchNonQuery(InstallSyncSQL, tempCon);
                }

            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }
        }

        public bool CreateCustomUser(SqlConnection con)
        {
            bool retVal = false;

            if (con.State != System.Data.ConnectionState.Open)
                con.Open();
            try
            {
                SqlCommand sqlIfExists = new SqlCommand("IF EXISTS (SELECT * FROM [sys].[sql_logins] WHERE name = '" + customDBUser + "') SELECT 1 AS BIT ELSE SELECT 0 AS BIT", con);
                bool isUserExists = Convert.ToBoolean(sqlIfExists.ExecuteScalar());
                if (!isUserExists)
                {
                    string createUser = "CREATE LOGIN [" + customDBUser + "] WITH PASSWORD=N'" + defaultDBSaPWD + "', DEFAULT_DATABASE=[master], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF";
                    SqlCommand sqlCmd = new SqlCommand(createUser, con);
                    sqlCmd.ExecuteNonQuery();
                    //Assign dbcreator role to newly created user.
                    SqlCommand sqlAddCreatorRole = new SqlCommand("ALTER SERVER ROLE [dbcreator] ADD MEMBER [" + customDBUser + "]", con);
                    sqlAddCreatorRole.ExecuteNonQuery();
                    retVal = true;
                }
                else
                    retVal = true;
            }
            catch (Exception)
            {
            }
            return retVal;
        }
        public bool CreateDefaultDB(string serverName, int Port, string DBName, string UserID, string password)
        {
            bool Retval = true;
            try
            {
                string createSQL = "CREATE DATABASE " + DBName + ";";
                SqlConnection tmpConn = new SqlConnection("server=" + serverName + (Port > 0 ? "," + Port.ToString() : "") + ";user id=" + UserID + ";password=" + password + ";");
                using (tmpConn)
                {
                    using (SqlCommand sqlCmd = new SqlCommand(createSQL, tmpConn))
                    {
                        tmpConn.Open();
                        sqlCmd.ExecuteNonQuery();
                        tmpConn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                _ = ex.Message;
                Retval = false;
            }
            return Retval;
        }
        public bool SetSQLSchema(string serverName, int Port, string DBName, string UserID, string password)
        {
            bool Retval = true;
            try
            {
                string SetSchemaSQL = WeighingSolution.DataLayer.Properties.Resources.Schema;
                ExecuteBatchNonQuery(SetSchemaSQL, new SqlConnection("server=" + serverName + (Port > 0 ? "," + Port.ToString() : "") + ";initial catalog=" + DBName + ";user id=" + UserID + ";password=" + password + ";"));

                CreateMissingTable(serverName, Port, DBName, UserID, password);
               
            }
            catch (Exception)
            {
                Retval = false;
            }
            return Retval;
        }
        public bool InsertDefaultData(string serverName, int Port, string DBName, string UserID, string password)
        {
            bool Retval = true;
            try
            {
                string DefaultDataSQL = WeighingSolution.DataLayer.Properties.Resources.Data;
                ExecuteBatchNonQuery(DefaultDataSQL, new SqlConnection("server=" + serverName + (Port > 0 ? "," + Port.ToString() : "") + ";initial catalog=" + DBName + ";user id=" + UserID + ";password=" + password + ";"));
            }
            catch (Exception)
            {
                Retval = false;
            }
            return Retval;
        }

        public static DataTable MSSQLExecuteReaderQuery(string DBSERVER, string DBNAME, string DBUSERNAME, string DBPASSWORD, string sql)
        {
            string Datastring = "Data Source=" + DBSERVER + ";Initial Catalog=" + DBNAME + ";user id=" + DBUSERNAME + ";password=" + DBPASSWORD + ";MultipleActiveResultSets=True;Connection Timeout=30";
            SqlConnection conn = new SqlConnection(Datastring);

            DataTable Updatetable = new DataTable();
            conn.Open();
            try
            {
                using (SqlDataAdapter dau = new SqlDataAdapter(sql, conn))
                {
                    dau.Fill(Updatetable);
                }
            }
            catch (Exception ex)
            {
                _ = ex.Message;
                return null;
            }
            finally
            {
                conn.Close();
            }
            if (Updatetable != null && Updatetable.Rows.Count > 0)
            {
                return Updatetable;
            }
            else
            {
                return null;
            }
        }

        public static DataTable SQLExecuteReaderQuery(string sql)
        {
            string Datastring = "Data Source=" + Constant.DBSERVER + ";Initial Catalog=" + Constant.DBNAME + ";user id=" + Constant.DBUSERNAME + ";password=" + Constant.DBPASSWORD + ";MultipleActiveResultSets=True;Connection Timeout=30";
            SqlConnection conn = new SqlConnection(Datastring);

            DataTable Updatetable = new DataTable();
            conn.Open();
            try
            {
                using (SqlDataAdapter dau = new SqlDataAdapter(sql, conn))
                {
                    dau.Fill(Updatetable);
                }
            }
            catch (Exception ex)
            {
                _ = ex.Message;
                return null;
            }
            finally
            {
                conn.Close();
            }
            if (Updatetable != null && Updatetable.Rows.Count > 0)
            {
                return Updatetable;
            }
            else
            {
                return null;
            }
        }
        public static bool ExecuteQuery(string sql)
        {
            string Datastring = "Data Source=" + Constant.DBSERVER + ";Initial Catalog=" + Constant.DBNAME + ";user id=" + Constant.DBUSERNAME + ";password=" + Constant.DBPASSWORD + ";MultipleActiveResultSets=True;Connection Timeout=30";
            SqlConnection conn = new SqlConnection(Datastring);

            bool Retval = true;
            SqlCommand cmd = new SqlCommand(string.Empty, conn);
            conn.Open();
            try
            {
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.WriteAllText(Path.Combine(Constant.PluginFileLocation, $"ExecuteQueryError{DateTime.Now:yyyy_MM_dd}.txt"), json);
                _ = ex.Message;
                Retval = false;
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
            return Retval;
        }

        public static bool ExecuteBatchNonQuery(string sql, SqlConnection conn)
        {
            string sqlBatch = string.Empty;
            bool Retval = true;
            SqlCommand cmd = new SqlCommand(string.Empty, conn);
            conn.Open();
            sql += "\nGO";   // make sure last batch is executed.
            try
            {
                foreach (string line in sql.Split(new string[2] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (line.ToUpperInvariant().Trim() == "GO")
                    {
                        if (!string.IsNullOrEmpty(sqlBatch))
                        {
                            cmd.CommandText = sqlBatch;
                            cmd.ExecuteNonQuery();
                            sqlBatch = string.Empty;
                        }
                    }
                    else
                    {
                        sqlBatch += line + "\n";
                    }
                }

            }
            catch (Exception ex)
            {

                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine +
                   $"Query = {sqlBatch} " + Environment.NewLine +
                    "Error = " + JsonConvert.SerializeObject(ex) + Environment.NewLine;
                File.WriteAllText(Path.Combine(Constant.PluginFileLocation, $"ExecuteBatchNonQuery_{DateTime.Now:yyyy_MM_dd}.txt"), json);

                _ = ex.Message;
                Retval = false;
            }
            finally
            {
                conn.Close();
            }
            return Retval;
        }
        public static string GetSMO(string servicename)
        {
            // NOTE: initialize new SMO object with (hostname\instance) 
            // however if instance is the default "MSSQLSERVER", then initialize with (hostname) only!
            string n = servicename; // e.g. "MSSQLSERVER\boobies";
            string[] s = n.Split('\\');
            string initSMO = servicename;
            if (s.Length == 2)
            {
                if (s[1].ToUpper().Equals("MSSQLSERVER"))
                    initSMO = s[0];
            }
            return initSMO;
        }

        public string GetDBServiceName(string serverName)
        {
            string serviceName = "MSSQLSERVER";
            string n = serverName;
            string[] s = n.Split('\\');
            if (s.Length == 2)
                serviceName = s[1];
            return serviceName;
        }

        public static bool ExecuteMySqlQuery(string Config, string sql)
        {
            string Constring = Config;
            bool result = true;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(Constring))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {

                _ = ex.Message;
                string json1 = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(sql);
                File.WriteAllText(Constant.ApplicationDataLocation + @"\MYSQLQuery.txt", json1);

                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.WriteAllText(Constant.ApplicationDataLocation + @"\MYSQLCatch.txt", json);
                result = false;
            }

            return result;
        }
    }

    public class CommonEncription
    {

        public static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static string EncKey = "JiBWUOES5Pau4sfm0hqgHUbIoZrlyBKG4sE8SzP2zNM=";
        public static string PublicKey = "MIIBKjCB4wYHKoZIzj0CATCB1wIBATAsBgcqhkjOPQEBAiEA/////wAAAAEAAAAAAAAAAAAAAAD///////////////8wWwQg/////wAAAAEAAAAAAAAAAAAAAAD///////////////wEIFrGNdiqOpPns+u9VXaYhrxlHQawzFOw9jvOPD4n0mBLAxUAxJ02CIbnBJNqZnjhE50mt4GffpAEIQNrF9Hy4SxCR/i85uVjpEDydwN9gS3rM6D0oTlF2JjClgIhAP////8AAAAA//////////+85vqtpxeehPO5ysL8YyVRAgEBA0IABHMGXb4zv0P8LWqnREQduAMXFmNZ9b4lr8oum3kJcXobgkrm191r4DZjXGXLA5fvBSBJFKkZb8V1keuwHNuMJz4=";
        public static string BackupDB = "BackupWeighbridge";
        public static string BackupDBTable = "Backuptbl";
        public static string BackupDBColumn = "BackupData";

        public static string EncryptData(string Str)
        {
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(Str);

                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(EncKey));

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string DecryptData(string Str)
        {
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = Convert.FromBase64String(Str);
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(EncKey));
                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static bool IsSaltMatch(string username, string password, UserMaster objUser)
        {
            try
            {
                if (!string.IsNullOrEmpty(objUser.Salt))
                    return objUser.Salt.Equals(createSalt(username, password));
                else
                    return false;
            }
            catch (Exception ex)
            {
                logErrorInDB(ex.Message, ex);
                return false;
            }
        }

        public static string createSalt(string username, string password)
        {
            try
            {
                string secret = "jm3UIgFC7CCQwqtr";
                string userpwdComb = username.ToLower() + password.ToLower();
                var encoding = new UTF8Encoding();
                byte[] keyByte = encoding.GetBytes(secret);
                byte[] messageBytes = encoding.GetBytes(userpwdComb);
                using (var hmacsha256 = new HMACSHA256(keyByte))
                {
                    byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                    return (Convert.ToBase64String(hashmessage));
                }
            }
            catch (Exception ex)
            {
                logErrorInDB(ex.Message, ex);
                return "";
            }
        }

        public static void logErrorInDB(string Message, Exception ex = null)
        {
            if (ex != null)
                Log.Error(Message, ex);
            else
                Log.Error(Message);
        }
    }
}
