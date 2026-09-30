using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.ServiceProcess;
using System.Timers;
using WeighingSolution.DataLayer;
using WeighingSolution.SharedComponents;
using static System.Net.Mime.MediaTypeNames;

namespace SyncService
{
    public partial class SyncScheduler : ServiceBase
    {
        System.Timers.Timer timerSync = new System.Timers.Timer();
        public SyncScheduler()
        {
            InitializeComponent();
        }
        protected override void OnCustomCommand(int command)
        {
            if (command == 200)
            {
                syncHelper.Deprovision(SyncConstant.SyncServiceConnectionString);
                this.Stop();
            }
        }
        protected override void OnStart(string[] args)
        {
            File.AppendAllText(Path.Combine(Constant.SyncLog, $"SyncLogServiceTrace_{DateTime.Now:YYYY_MM_dd_HH}.txt"), Environment.NewLine + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "------------------------------------------------------------------" + Environment.NewLine + "Sync Service Start");
            try
            {
                bool hasBarcode = false;
                bool hasCamera = false;
                using (SqlConnection con = new SqlConnection(SyncConstant.SyncServiceConnectionString))
                {
                    try
                    {
                        con.Open();

                        SqlCommand oCmdBarcode = new SqlCommand("SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BarcodeImage]') AND type in (N'U')", con);
                        DataTable dtBarcode = new DataTable();
                        dtBarcode.Load(oCmdBarcode.ExecuteReader());
                        if (dtBarcode != null && dtBarcode.Rows.Count > 0)
                        {
                            hasBarcode = true;
                        }
                        SqlCommand oCmdCamera = new SqlCommand("SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CameraImage]') AND type in (N'U')", con);
                        DataTable dtCamera = new DataTable();
                        dtCamera.Load(oCmdCamera.ExecuteReader());
                        if (dtCamera != null && dtCamera.Rows.Count > 0)
                        {
                            hasCamera = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(Path.Combine(Constant.SyncLog, $"SyncServiceTrace_{DateTime.Now:yyyy_MM_dd_HH}.txt"), ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "------------------------------------------------------------------" + Environment.NewLine);
                    }
                    con.Close();
                }
                //Provisioning Scope
                var tables = new Dictionary<string, List<string>>()
                {
                    { "Account_Tracking", new List<string>() },
                    { "ApplicationSettings_Tracking", new List<string>() },
                    { "CustomField_Tracking", new List<string>() },
                    { "Gate_Tracking", new List<string>() },
                    { "MasterField_Tracking", new List<string>() },
                    { "Product_Tracking", new List<string>() },
                    { "Template_Tracking", new List<string>() },
                    { "Unit_Tracking", new List<string>() },
                    { "Driver_Tracking", new List<string>() },
                    { "Vehicle_Tracking", new List<string>() },
                    { "UserMaster_Tracking", new List<string>() },
                    { "WeightBridge_Tracking", new List<string>() },
                    { "TemplatePermission_Tracking", new List<string>() },
                    { "TransactionData_Tracking", new List<string>() },
                    { "TransactionDetail_Tracking", new List<string>() },
                    { "CustomValue_Tracking", new List<string>() },
                    { "Account", new List<string>() },
                    { "ApplicationSettings", new List<string>() },
                    { "CustomField", new List<string>() },
                    { "Feature", new List<string>() },
                    { "Gate", new List<string>() },
                    { "MasterField", new List<string>() },
                    { "Product", new List<string>() },
                    { "Template", new List<string>() },
                    { "Unit", new List<string>() },
                    { "Driver", new List<string>() { "FK_Driver_Account" } },
                    { "Vehicle", new List<string>() { "FK_Vehicle_Account" } },
                    { "UserMaster", new List<string>() { "FK_Usermaster_Template" } },
                    { "WeightBridge", new List<string>() { "FK_WeightBridge_Unit" } },
                    { "TemplatePermission", new List<string>() { "FK_TemplatePermission_Feature", "FK_TemplatePermission_Template" } },
                    { "TransactionData", new List<string>() { "FK_TransactionData_Account", "FK_TransactionData_Account1", "FK_TransactionData_Driver", "FK_TransactionData_UserMaster", "FK_TransactionData_Vehicle" } },
                    { "TransactionDetail", new List<string>() { "FK_TransactionDetail_Gate", "FK_TransactionDetail_Product", "FK_TransactionDetail_TransactionData", "FK_TransactionDetail_UserMaster", "FK_TransactionDetail_WeightBridge" } },
                    { "CustomValue", new List<string>() { "FK_CustomValue_CustomField", "FK_CustomValue_TransactionData" } }
                };

                if (hasBarcode)
                {
                    tables.Add("BarcodeImage", new List<string>() { "FK_BarcodeImage_TransactionData" });
                }
                if (hasCamera)
                {
                    tables.Add("Camera", new List<string>() { "FK_Camera_WeightBridge" });
                    tables.Add("CameraImage", new List<string>() { "FK_CameraImage_Camera", "FK_CameraImage_TransactionDetail" });
                }
                syncHelper.Provision(SyncConstant.SyncServiceConnectionString, tables);

                timerSync.Elapsed += new System.Timers.ElapsedEventHandler(SyncDB);
                //timerSync.Interval = 300000;  //5 Mins
                timerSync.Interval = 120000;  //5 Mins
                timerSync.Start();
                timerSync.Enabled = true;

            }
            catch (Exception ex)
            {
                File.AppendAllText(Path.Combine(Constant.SyncLog, $"SyncServiceTrace_{DateTime.Now:yyyy_MM_dd_HH}.txt"), ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "------------------------------------------------------------------" + Environment.NewLine);
            }
        }

        private void SyncDB(object sender, ElapsedEventArgs e)
        {
            timerSync.Enabled = false;
            File.AppendAllText(Path.Combine(Constant.SyncLog, $"SyncLogServiceTrace_{DateTime.Now:YYYY_MM_dd_HH}.txt"), Environment.NewLine + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "------------------------------------------------------------------" + Environment.NewLine + "Sync Start");
            try
            {
                string connectionString = SyncConstant.SyncServiceConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand oCmd = new SqlCommand("SELECT SettingValue FROM ApplicationSettings WHERE ModuleName = 'SyncDBSetting' AND SettingName = 'DestinationDBConnection'", con);
                    string destinationDBConnectionString = oCmd.ExecuteScalar().ToString();

                    if (!string.IsNullOrEmpty(destinationDBConnectionString))
                    {
                        syncHelper.ExecuteSync(connectionString, destinationDBConnectionString);
                        //File.AppendAllText(SyncConstant.LogFileLocation, Environment.NewLine + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "------------------------------------------------------------------" + Environment.NewLine + connectionString + Environment.NewLine + destinationDBConnectionString);
                        File.AppendAllText(Path.Combine(Constant.SyncLog, $"SyncLogServiceTrace_{DateTime.Now:YYYY_MM_dd_HH}.txt"), Environment.NewLine + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "------------------------------------------------------------------" + Environment.NewLine + "DB Sync SuccessFull");

                        SqlCommand oCmdPath = new SqlCommand("SELECT SettingValue FROM ApplicationSettings WHERE ModuleName = 'SyncDBSetting' AND SettingName = 'DestinationImagePath'", con);
                        string destinationImagePath = oCmdPath.ExecuteScalar().ToString();
                        if (!string.IsNullOrEmpty(destinationImagePath))
                        {
                            SqlCommand oCmdBarcode = new SqlCommand("SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BarcodeImage]') AND type in (N'U')", con);
                            DataTable dtBarcode = new DataTable();
                            dtBarcode.Load(oCmdBarcode.ExecuteReader());
                            if (dtBarcode != null && dtBarcode.Rows.Count > 0)
                            {
                                SqlCommand oBarcodeImagePath = new SqlCommand("SELECT SettingValue FROM ApplicationSettings WHERE ModuleName = 'BarcodeSetting' AND SettingName = 'BarcodeImagePath'", con);
                                string barcodeImagePath = oBarcodeImagePath.ExecuteScalar().ToString();
                                if (!String.IsNullOrEmpty(barcodeImagePath))
                                    SyncImages(barcodeImagePath, destinationImagePath + "BarcodeImages");
                            }
                            SqlCommand oCmdCamera = new SqlCommand("SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CameraImage]') AND type in (N'U')", con);
                            DataTable dtCamera = new DataTable();
                            dtCamera.Load(oCmdCamera.ExecuteReader());
                            if (dtCamera != null && dtCamera.Rows.Count > 0)
                            {
                                SqlCommand oCameraImagePath = new SqlCommand("SELECT SettingValue FROM ApplicationSettings WHERE ModuleName = 'CameraSetting' AND SettingName = 'CameraImagePath'", con);
                                string cameraImagePath = oCameraImagePath.ExecuteScalar().ToString();
                                if (!String.IsNullOrEmpty(cameraImagePath))
                                    SyncImages(cameraImagePath, destinationImagePath + "CameraImages");
                            }
                        }
                        con.Close();
                    }
                    else
                    {
                        File.AppendAllText(Path.Combine(Constant.SyncLog, $"SyncLogServiceTrace_{DateTime.Now:YYYY_MM_dd_HH}.txt"), Environment.NewLine + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "------------------------------------------------------------------" + Environment.NewLine + "Destination DB Connection String");
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Path.Combine(Constant.SyncLog, $"SyncServiceTrace_{DateTime.Now:yyyy_MM_dd_HH}.txt"), ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "------------------------------------------------------------------" + Environment.NewLine);
            }
            timerSync.Enabled = true;

        }

        private void SyncImages(string LocalDirectory, string RemoteDirectory)
        {
            try
            {
                string userID = "";
                string FTPPWD = "";
                using (SqlConnection con = new SqlConnection(SyncConstant.SyncServiceConnectionString))
                {
                    con.Open();
                    SqlCommand oCmdUserID = new SqlCommand("SELECT SettingValue FROM ApplicationSettings WHERE ModuleName = 'SyncDBSetting' AND SettingName = 'FTPUserID'");
                    oCmdUserID.Connection = con;
                    userID = oCmdUserID.ExecuteScalar().ToString();
                    SqlCommand oCmdUserPWD = new SqlCommand("SELECT SettingValue FROM ApplicationSettings WHERE ModuleName = 'SyncDBSetting' AND SettingName = 'FTPUserPWD'");
                    oCmdUserPWD.Connection = con;
                    FTPPWD = oCmdUserPWD.ExecuteScalar().ToString();
                    con.Close();
                }

                List<string> FileList = new List<string>();
                try
                {
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(RemoteDirectory);
                    request.Method = WebRequestMethods.Ftp.ListDirectory;

                    request.Credentials = new NetworkCredential(userID, FTPPWD);
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    Stream responseStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(responseStream);
                    string names = reader.ReadToEnd();

                    reader.Close();
                    response.Close();

                    names = names.Replace("\r\n", ",");

                    string[] List = names.Split(',');
                    foreach (var item in List)
                        FileList.Add(item);
                }
                catch (Exception)
                {

                }

                List<string> postedImages = new List<string>();
                List<string> filesInLocal = new List<string>();
                //string[] di = Directory.GetFiles(LocalDirectory, "*.jpeg");
                string[] di = Directory.GetFiles(LocalDirectory);
                for (int i = 0; i < di.Length; i++)
                {
                    string fileName = Path.GetFileName(di[i].ToString().ToLower());
                    if (postedImages.IndexOf(fileName) < 0)
                    {
                        if (!FileList.Contains(fileName))
                            filesInLocal.Add(di[i]);
                    }
                }
                string[] newFilesToPost = filesInLocal.ToArray();

                for (int i = 0; i < newFilesToPost.Length; i++)
                {
                    File.WriteAllText(Path.Combine(Constant.SyncLog, $"SyncLog_ServiceTrace_{DateTime.Now:yyyy_MM_dd_HH}.txt"), RemoteDirectory + "/" + Path.GetFileName(newFilesToPost[i]) + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "------------------------------------------------------------------" + Environment.NewLine);

                    WebRequest request = WebRequest.Create(RemoteDirectory + "/" + Path.GetFileName(newFilesToPost[i]));
                    request.Method = WebRequestMethods.Ftp.UploadFile;
                    request.Credentials = new NetworkCredential(userID, FTPPWD);

                    byte[] fileContents = File.ReadAllBytes(newFilesToPost[i]);
                    request.ContentLength = fileContents.Length;
                    //request.UsePassive = true;                    
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(fileContents, 0, fileContents.Length);
                    requestStream.Close();

                    //FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    WebResponse response = (WebResponse)request.GetResponse();
                }
            }
            catch (WebException ex)
            {
                File.AppendAllText(Path.Combine(Constant.SyncLog, $"SyncServiceTrace_{DateTime.Now:yyyy_MM_dd_HH}.txt"), ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "------------------------------------------------------------------" + Environment.NewLine);
            }
        }

        protected override void OnStop()
        {
            timerSync.Enabled = false;
        }
    }
}
