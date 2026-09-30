using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Windows;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;
using WeighingSolution.SharedComponents;

namespace WeighingSolution.UserControls
{
    /// <summary>
    /// Interaction logic for SyncDBSetting.xaml
    /// </summary>
    public partial class SyncDBSetting : ChildWindow
    {
        private List<WeighingSolution.SharedComponents.PluginManagement> lstPluginmgmt = new List<WeighingSolution.SharedComponents.PluginManagement>();
        private WeighingSolution.SharedComponents.PluginManagement objplugnmgmt;
        private static string EncKey = "JiBWUOES5Pau4sfm0hqgHUbIoZrlyBKG4sE8SzP2zNM=";
        private static string ServiceName = "DBSyncService";
        private string syncconnectionstring;
        public static string CustomSettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\WeighBridgeSolution\CustomSettings";

        public static string traceFileLocation = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + $"\\WeighBridgeSolution\\Plugins\\SyncServiceTrace_{DateTime.Now:YYYY_MM_dd_HH}.txt";
        public static string LogFileLocation = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + $"\\WeighBridgeSolution\\Plugins\\SyncLogServiceTrace_{DateTime.Now:YYYY_MM_dd_HH}.txt";

        private static string _syncLocalserviceconnectionstring;
        public static string SyncLocalServiceConnectionString
        {
            get
            {
                if (String.IsNullOrEmpty(_syncLocalserviceconnectionstring))
                {
                    DBSettingData objDBSetting = new DBSettingData();
                    if (File.Exists(CustomSettingsPath + @"\Configuration.dat"))
                    {
                        string Data = File.ReadAllText(CustomSettingsPath + @"\Configuration.dat");
                        if (!string.IsNullOrEmpty(Data))
                        {
                            string decryptedData = DecryptData(Data);
                            if (!string.IsNullOrEmpty(decryptedData))
                            {
                                objDBSetting = Newtonsoft.Json.JsonConvert.DeserializeObject<DBSettingData>(decryptedData);
                                if (objDBSetting != null)
                                {
                                    if (objDBSetting.Port > 0)
                                        _syncLocalserviceconnectionstring = "Data Source=" + objDBSetting.Server + "," + objDBSetting.Port + ";Initial Catalog=" + objDBSetting.DBName + ";user id=" + objDBSetting.UserId + ";password=" + objDBSetting.password + ";";
                                    else
                                        _syncLocalserviceconnectionstring = "Data Source=" + objDBSetting.Server + ";Initial Catalog=" + objDBSetting.DBName + ";user id=" + objDBSetting.UserId + ";password=" + objDBSetting.password + ";";
                                }
                            }
                        }
                    }
                }
                return _syncLocalserviceconnectionstring;
            }
            set { _syncLocalserviceconnectionstring = value; }
        }

        public SyncDBSetting()
        {
            try
            {
                InitializeComponent();
                if (File.Exists(Constant.PluginFileLocation + @"\PluginConfiguration.dat"))
                {
                    string Data = File.ReadAllText(Constant.PluginFileLocation + @"\PluginConfiguration.dat");
                    if (!string.IsNullOrEmpty(Data))
                        lstPluginmgmt = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WeighingSolution.SharedComponents.PluginManagement>>(Data);
                }
                if (lstPluginmgmt != null)
                {
                    objplugnmgmt = lstPluginmgmt.FirstOrDefault(x => x.PluginName == "SyncPlugin");
                    ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == ServiceName);
                    if (ctl == null || ctl.Status != ServiceControllerStatus.Running)
                        txtToInstall.Visibility = Visibility.Visible;
                    else
                        txtInstalled.Visibility = Visibility.Visible;

                    DBSettingData objDBSetting = new DBSettingData();
                    if (File.Exists(CustomSettingsPath + @"\Configuration.dat"))
                    {
                        string Data = File.ReadAllText(CustomSettingsPath + @"\Configuration.dat");
                        if (!string.IsNullOrEmpty(Data))
                        {
                            string decryptedData = DecryptData(Data);
                            if (!string.IsNullOrEmpty(decryptedData))
                            {
                                objDBSetting = Newtonsoft.Json.JsonConvert.DeserializeObject<DBSettingData>(decryptedData);
                                if (objDBSetting != null)
                                {
                                    if (objDBSetting.Port > 0)
                                        syncconnectionstring = "Data Source=" + objDBSetting.Server + "," + objDBSetting.Port + ";Initial Catalog=" + objDBSetting.DBName + ";user id=" + objDBSetting.UserId + ";password=" + objDBSetting.password + ";";
                                    else
                                        syncconnectionstring = "Data Source=" + objDBSetting.Server + ";Initial Catalog=" + objDBSetting.DBName + ";user id=" + objDBSetting.UserId + ";password=" + objDBSetting.password + ";";
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(syncconnectionstring))
                    {
                        using (SqlConnection con = new SqlConnection(syncconnectionstring))
                        {
                            try
                            {
                                con.Open();
                                SqlCommand oCmd = new SqlCommand("SELECT SettingValue FROM ApplicationSettings WHERE ModuleName = 'SyncDBSetting' AND SettingName = 'DestinationDBConnection'");
                                oCmd.Connection = con;
                                string DestinationDBConnection = oCmd.ExecuteScalar().ToString();
                                string[] dbConfig = DestinationDBConnection.Split(';');
                                txtDestinationDBIP.Text = dbConfig[0].Replace("Data Source=", "");
                                txtDBName.Text = dbConfig[1].Replace("Initial Catalog=", "");
                                txtUsername.Text = dbConfig[2].Replace("user id=", "");
                                txtPassword.Password = dbConfig[3].Replace("password=", "");

                                SqlCommand oCmdPath = new SqlCommand("SELECT SettingValue FROM ApplicationSettings WHERE ModuleName = 'SyncDBSetting' AND SettingName = 'DestinationImagePath'");
                                oCmdPath.Connection = con;
                                txtImagePath.Text = oCmdPath.ExecuteScalar().ToString();

                                SqlCommand oCmdUserID = new SqlCommand("SELECT SettingValue FROM ApplicationSettings WHERE ModuleName = 'SyncDBSetting' AND SettingName = 'FTPUserID'");
                                oCmdUserID.Connection = con;
                                txtFTPUserID.Text = oCmdUserID.ExecuteScalar().ToString();

                                SqlCommand oCmdUserPWD = new SqlCommand("SELECT SettingValue FROM ApplicationSettings WHERE ModuleName = 'SyncDBSetting' AND SettingName = 'FTPUserPWD'");
                                oCmdUserPWD.Connection = con;
                                txtFTPUserPWD.Password = oCmdUserPWD.ExecuteScalar().ToString();

                                con.Close();
                            }
                            catch (Exception ex)
                            {
                                con.Close();
                                Common.logErrorInDB(ex.Message, ex);
                            }
                        }
                    }
                    else
                    {
                        ToastHelper.ShowWarning("Unable to connect database.");
                    }
                }
            }
            catch (Exception ex)
            {
                ToastHelper.ShowError("Something went wrong, please contact support team.");
                Common.logErrorInDB(ex.Message, ex);
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

        private void lnkSVCInstall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string serviceFilePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\SyncService.exe";
                string strCmdText = "/C sc create " + ServiceName + " binPath= \"" + serviceFilePath + "\" start= delayed-auto displayName= \"" + ServiceName + "\"";

                try
                {
                    //Check for window service.
                    ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == ServiceName);
                    if (ctl == null)
                    {
                        Process process = new Process();
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.FileName = "cmd.exe";
                        startInfo.Arguments = strCmdText;
                        startInfo.Verb = "runas";
                        process.StartInfo = startInfo;
                        process.Start();
                        process.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    Common.logErrorInDB(ex.Message + $" {strCmdText}", ex);
                    File.WriteAllText(Path.Combine( Constant.PluginFileLocation, "SyncService_InstallError.txt"), JsonConvert.SerializeObject(ex));
                }

                try
                {
                    ServiceController sc = new ServiceController(ServiceName);

                    if (sc.Status == ServiceControllerStatus.Running)
                    {
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                    }

                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                }
                catch (Exception ex)
                {
                    Common.logErrorInDB(ex.Message, ex);
                }
            }
            catch
            {

            }           

        }

        private void lnkSVCStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceController sc = new ServiceController(ServiceName);

                if (sc.Status == ServiceControllerStatus.Running)
                {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                }

                //sc.Start();
                //sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
            }
            catch (Exception ex)
            {
                Common.logErrorInDB(ex.Message, ex);
            }
        }



        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool hasError = false;
                string remoteConnectionString = "Data Source=XXXIPXXX;Initial Catalog=XXXDBNameXXX;user id=XXXUserNameXXX;password=XXXPasswordXXX;";
                remoteConnectionString = remoteConnectionString.Replace("XXXIPXXX", txtDestinationDBIP.Text);
                remoteConnectionString = remoteConnectionString.Replace("XXXDBNameXXX", txtDBName.Text);
                remoteConnectionString = remoteConnectionString.Replace("XXXUserNameXXX", txtUsername.Text);
                remoteConnectionString = remoteConnectionString.Replace("XXXPasswordXXX", txtPassword.Password);
                try
                {
                    using (SqlConnection con = new SqlConnection(remoteConnectionString))
                    {
                        con.Open();
                    }
                }
                catch (Exception ex)
                {
                    hasError = true;
                    ToastHelper.ShowError("Unable to connect remote database kindly check remote DB credential.");
                    Common.logErrorInDB(ex.Message, ex);
                }
                if (!hasError)
                {
                    using (SqlConnection conLocal = new SqlConnection(syncconnectionstring))
                    {
                        conLocal.Open();
                        SqlCommand oCmd = new SqlCommand();
                        oCmd.CommandText = "UPDATE ApplicationSettings SET SettingValue = '" + remoteConnectionString + "' WHERE ModuleName = 'SyncDBSetting' AND SettingName = 'DestinationDBConnection'";
                        oCmd.Connection = conLocal;
                        oCmd.ExecuteNonQuery();

                        if (!string.IsNullOrEmpty(txtImagePath.Text))
                        {
                            SqlCommand oCmdImage = new SqlCommand();
                            oCmdImage.CommandText = "UPDATE ApplicationSettings SET SettingValue = '" + txtImagePath.Text + "' WHERE ModuleName = 'SyncDBSetting' AND SettingName = 'DestinationImagePath'";
                            oCmdImage.Connection = conLocal;
                            oCmdImage.ExecuteNonQuery();

                            SqlCommand oCmdUserID = new SqlCommand();
                            oCmdUserID.CommandText = "UPDATE ApplicationSettings SET SettingValue = '" + txtFTPUserID.Text + "' WHERE ModuleName = 'SyncDBSetting' AND SettingName = 'FTPUserID'";
                            oCmdUserID.Connection = conLocal;
                            oCmdUserID.ExecuteNonQuery();

                            SqlCommand oCmdUserPWD = new SqlCommand();
                            oCmdUserPWD.CommandText = "UPDATE ApplicationSettings SET SettingValue = '" + txtFTPUserPWD.Password + "' WHERE ModuleName = 'SyncDBSetting' AND SettingName = 'FTPUserPWD'";
                            oCmdUserPWD.Connection = conLocal;
                            oCmdUserPWD.ExecuteNonQuery();
                        }
                        conLocal.Close();
                        ToastHelper.ShowSuccess("Details updated successfully");
                    }
                }
            }
            catch (Exception ex)
            {
                ToastHelper.ShowError("Something went wrong, please contact support team.");
                Common.logErrorInDB(ex.Message, ex);
            }
        }

        private async void btnDeProvision_Click(object sender, RoutedEventArgs e)
        {
            var result = await MessageBoxHelper.ShowOkCancelDialogAsync("Are you sure?", "You want to de provision the DB? \n IMPORTANT: Sync plugin service will be stopped.");
            if (result == MessageDialogResult.Affirmative)
            {
                try
                {
                    string ServiceName = "DBSyncService";
                    ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == ServiceName);
                    if (ctl != null)
                    {
                        ctl.ExecuteCommand(200);
                        ToastHelper.ShowSuccess("De provision of DB is successfull.");
                    }
                }
                catch
                {

                }
            }
        }
        public string SyncServerServiceConnectionString()
        {
            return "Data Source=" + txtDestinationDBIP.Text + ";Initial Catalog=" + txtDBName.Text + ";user id=" + txtUsername.Text + ";password=" + txtPassword.Password + ";";
        }

        public bool CheckConnection()
        {
            bool hasError = false;
            try
            {
                string serverConnection = SyncServerServiceConnectionString();
                using (SqlConnection con = new SqlConnection(serverConnection))
                {
                    con.Open();
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                hasError = true;
                ToastHelper.ShowError("Unable to connect remote database kindly check remote DB credential.");
            }

            return hasError;
        }

        private async void btnServerProvision_Click(object sender, RoutedEventArgs e)
        {
            var result = await MessageBoxHelper.ShowOkCancelDialogAsync("Are you sure?", "You want to provision the DB?");
            if (result == MessageDialogResult.Affirmative)
            {
                if (!CheckConnection())
                {
                    bool hasBarcode = false;
                    bool hasCamera = false;
                    using (SqlConnection con = new SqlConnection(SyncLocalServiceConnectionString))
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
                            File.AppendAllText(traceFileLocation, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "------------------------------------------------------------------" + Environment.NewLine);
                        }
                        con.Close();
                    }

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
                    string serverConnection = SyncServerServiceConnectionString();
                    syncHelper.Provision(serverConnection, tables);
                }

            }
        }

        private async void btnServerDeProvision_Click(object sender, RoutedEventArgs e)
        {
            var result = await MessageBoxHelper.ShowOkCancelDialogAsync("Are you sure?", "You want to de provision the DB? ");
            if (result == MessageDialogResult.Affirmative)
            {
                if (!CheckConnection())
                {
                    string serverConnection = SyncServerServiceConnectionString();
                    syncHelper.Deprovision(serverConnection);
                }
            }
        }

    }
}
