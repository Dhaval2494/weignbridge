using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using MahApps.Metro;
using Newtonsoft.Json;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows;
using System.Windows.Threading;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;

namespace WeighingSolution
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public DBSettingData objDBSetting = new DBSettingData();
        DatabaseFunctions DBFunctions = new DatabaseFunctions();
        SplashScreen splashScreen = new SplashScreen("/Images/loading.jpg");
        public static CompositionContainer pluginContainer;
        System.Threading.Mutex singletonMutex = null;
        bool hasHandle = false;
        public static NamedPipeManager PipeManager = null;

        protected readonly IEventAggregator EventAggregator = AppEventManager.Instance.EventAggregator;
        public App()
        {
            System.Net.ServicePointManager.Expect100Continue = false;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;

            //string pass = Common.DecryptData("drGhifYnz+KI9nYIj/XGyGD5WHODs27x");

            string dbCreationError = "";
            AppMode stVal = (AppMode)Enum.Parse(typeof(AppMode), System.Configuration.ConfigurationManager.AppSettings["AppMode"]);

            //Single Instance Check
            // get application GUID as defined in AssemblyInfo.cs
            string appGuid = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value.ToString();

            // unique id for global mutex - Global prefix means it is global to the machine
            string mutexId = string.Format("Global\\{{{0}}}", appGuid);
            bool createdNew;

            // setting up security for multi-user usage
            // To work also on localized systems (don't use just "Everyone") 
            var allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
            var securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(allowEveryoneRule);

            // prevent race condition on security settings
            singletonMutex = new System.Threading.Mutex(false, mutexId, out createdNew, securitySettings);
            try
            {
                // note, you may want to time out here instead of waiting forever
                // mutex.WaitOne(Timeout.Infinite, false);
                hasHandle = singletonMutex.WaitOne(2500, false);
                if (hasHandle == false)
                {
                    //Another instance of same program is running.
                    //throw new TimeoutException("Timeout waiting for exclusive access");
                    NamedPipeManager.Write("ACTIVATE");
                    Application.Current.Shutdown();
                    return;
                }
            }
            catch (System.Threading.AbandonedMutexException)
            {
                // Log the fact that the mutex was abandoned in another process, it will still get acquired
                hasHandle = true;
            }

            PipeManager = new NamedPipeManager();
            PipeManager.StartServer();

            //IMPORTANT: Do not remove, patch to allow decimal inputs on a bound textbox with ‘UpdateSourceTrigger = PropertyChanged’
            //https://stackoverflow.com/questions/34326667/wpf-text-box-allowing-decimal-entries
            System.Windows.FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;

            SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);
            splashScreen.Show(false);

            Common.CreateDirectoryWithFullAccess(Constant.CustomImagesPath);
            Common.CreateDirectoryWithFullAccess(Constant.CustomImagesPath + @"\Logos");

            Common.CreateDirectoryWithFullAccess(Constant.PluginFileLocation);
            Common.CreateDirectoryWithFullAccess(Constant.CustomSettingsPath);
            Common.CreateDirectoryWithFullAccess(Constant.EmailFileLocation);
            Common.CreateDirectoryWithFullAccess(Constant.ANPRCSVLocation);
            Common.CreateDirectoryWithFullAccess(Constant.ModbusDataLocation);
            Common.CreateDirectoryWithFullAccess(Constant.SyncLog);

            if (!System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\HCNetSDK"))
            {
                ZipArchiveHelper.ExtractFile(AppDomain.CurrentDomain.BaseDirectory + @"\HCNetSDK.zip", AppDomain.CurrentDomain.BaseDirectory);
            }
            if (!System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\LibVlc"))
            {
                ZipArchiveHelper.ExtractFile(AppDomain.CurrentDomain.BaseDirectory + @"\LibVlc.zip", AppDomain.CurrentDomain.BaseDirectory);
            }
            //If application started with command line arguments, handle them...
            string RemovePlugin = string.Empty;
            string[] args = Environment.GetCommandLineArgs();
            foreach (string arg in args)
            {
                if (arg.StartsWith("RemovePlugin="))
                {
                    RemovePlugin = arg.Substring(13).Trim();
                }
            }
            //Create Plugin Catalog and Container to identify all the exports
            if (!Constant.IsBasicversion)
            {
                AggregateCatalog catalog = new AggregateCatalog(new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly()), new DirectoryCatalog(Constant.PluginFileLocation));
                foreach (var path in Directory.EnumerateDirectories(Constant.PluginFileLocation, "*", SearchOption.TopDirectoryOnly))
                {
                    if (String.IsNullOrEmpty(RemovePlugin) || Path.GetFileName(path).ToLower() != RemovePlugin.ToLower())
                        catalog.Catalogs.Add(new DirectoryCatalog(path));
                }
                pluginContainer = new CompositionContainer(catalog);
            }
            #region Basic License File Existence Check

            if (!File.Exists(Constant.CustomSettingsPath + @"\license.lic"))
                CheckOnlineLicense();

            if (!File.Exists(Constant.CustomSettingsPath + @"\license.lic"))
            {
                this.StartupUri = new Uri("LicenseForm.Xaml", UriKind.Relative);
                splashScreen.Close(new TimeSpan(0, 0, 0, 0, 100));
                return;
            }
            else
            {
                try
                {
                    string licenseKey = File.ReadAllText(Constant.CustomSettingsPath + @"\license.lic");
                    string decryptedData = Common.DecryptData(licenseKey);
                    if (decryptedData.Contains("HardwareID"))
                    {
                        Portable.Licensing.License LicenseInfo = Portable.Licensing.License.Load(decryptedData);
                        Constant.IsBasicversion = false;
                        if (LicenseInfo != null && LicenseInfo.ProductFeatures != null && LicenseInfo.ProductFeatures.Get("SoftwareType") != null)
                        {
                            LicenseHelper.LicenseInfo = LicenseInfo;
                            string SoftwareType = LicenseInfo.ProductFeatures.Get("SoftwareType").ToString();
                            if (SoftwareType == "Basic Version")
                                Constant.IsBasicversion = true;

                            if (decryptedData.Contains("AMCExpiry"))
                            {
                                DateTime expiryDate;
                                if (DateTime.TryParse(LicenseInfo.ProductFeatures.Get("AMCExpiry").ToString(), out expiryDate))
                                {
                                    //Constant.AmcExpire = DateTime.Now.AddDays(-15); // true if expired
                                    Constant.AmcExpire = expiryDate;
                                }
                            }

                        }
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(new Action(async () =>
                        {
                            await MessageBoxHelper.ShowInfoDialogAsync("License Error", "License File corrupted. Connect Internet & Restart Application");
                            File.Delete(Constant.CustomSettingsPath + @"\license.lic");
                            RestartApplication();
                        }));
                    }
                }
                catch (Exception ex)
                {
                    _ = ex.Message;
                }

            }

            #endregion



            bool IsConnectionStringSet = false;
            log4net.Config.XmlConfigurator.Configure();
            if (File.Exists(Constant.CustomSettingsPath + @"\Configuration.dat"))
            {
                string Data = File.ReadAllText(Constant.CustomSettingsPath + @"\Configuration.dat");
                if (!string.IsNullOrEmpty(Data))
                {
                    string decryptedData = Common.DecryptData(Data);
                    if (!string.IsNullOrEmpty(decryptedData))
                    {
                        objDBSetting = Newtonsoft.Json.JsonConvert.DeserializeObject<DBSettingData>(decryptedData);
                        string Datastring = "";
                        if (objDBSetting != null)
                        {

                            Constant.DBNAME = objDBSetting.DBName;
                            Constant.DBSERVER = objDBSetting.Server;
                            Constant.DBUSERNAME = objDBSetting.UserId;
                            Constant.DBPASSWORD = objDBSetting.password;
                            Constant.DBPORT = objDBSetting.Port;

                            if (DBFunctions.IsDefaultDBExists(objDBSetting.Server, objDBSetting.Port, objDBSetting.DBName, objDBSetting.UserId, objDBSetting.password))
                            {
                                if (objDBSetting.Port > 0)
                                    Datastring = "Data Source=" + objDBSetting.Server + "," + objDBSetting.Port + ";Initial Catalog=" + objDBSetting.DBName + ";user id=" + objDBSetting.UserId + ";password=" + objDBSetting.password + ";MultipleActiveResultSets=true;";
                                else
                                    Datastring = "Data Source=" + objDBSetting.Server + ";Initial Catalog=" + objDBSetting.DBName + ";user id=" + objDBSetting.UserId + ";password=" + objDBSetting.password + ";MultipleActiveResultSets=true;";
                                IsConnectionStringSet = true;
                            }
                            else
                            {
                                //TODO: Not working correctly. Need to move this out from app.xaml to seperate page with information about SQL server not running.
                                //string ServiceName = DBFunctions.GetDBServiceName(objDBSetting.Server);
                                //System.ServiceProcess.ServiceController ctl = System.ServiceProcess.ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == ServiceName);
                                //if (ctl != null && ctl.Status != System.ServiceProcess.ServiceControllerStatus.Running)
                                //{
                                //    Process process = new Process();
                                //    process.StartInfo.FileName = "cmd.exe";
                                //    process.StartInfo.Arguments = "/C net start \"" + ServiceName + "\"";
                                //    process.StartInfo.Verb = "runas";
                                //    process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                                //    process.Start();
                                //    process.WaitForExit();

                                //    for (int i = 0; i < 30; i++)
                                //    {
                                //        //Give some time for server to become ready.
                                //        System.Threading.Thread.Sleep(5000);
                                //        ctl = System.ServiceProcess.ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == ServiceName);
                                //        if (ctl != null && ctl.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                                //            break;
                                //    }

                                //    //Try connecting two times and then fail.
                                //    for (int i = 0; i < 2; i++)
                                //    {
                                //        if (DBFunctions.IsDefaultDBExists(objDBSetting.Server, objDBSetting.Port, objDBSetting.DBName, objDBSetting.UserId, objDBSetting.password))
                                //        {
                                //            if (objDBSetting.Port > 0)
                                //                Datastring = "Data Source=" + objDBSetting.Server + "," + objDBSetting.Port + ";Initial Catalog=" + objDBSetting.DBName + ";user id=" + objDBSetting.UserId + ";password=" + objDBSetting.password + ";";
                                //            else
                                //                Datastring = "Data Source=" + objDBSetting.Server + ";Initial Catalog=" + objDBSetting.DBName + ";user id=" + objDBSetting.UserId + ";password=" + objDBSetting.password + ";";
                                //            IsConnectionStringSet = true;
                                //        }
                                //        if (IsConnectionStringSet)
                                //            break;
                                //        System.Threading.Thread.Sleep(2000);
                                //    }
                                //}
                            }
                        }
                    }
                }
            }
            if (!IsConnectionStringSet)
            {
                #region Check SQL Exists or Not
                if (DBFunctions.IsDefaultServerExists())
                {
                    if (DBFunctions.IsDefaultDBExists(DatabaseFunctions.defaultDBServer, 0, DatabaseFunctions.defaultDBName, DatabaseFunctions.defaultDBUser, DatabaseFunctions.defaultDBSaPWD))
                    {
                        try
                        {
                            DBSettingData objNewDBSetting = new DBSettingData();
                            objNewDBSetting.Server = DatabaseFunctions.defaultDBServer;
                            objNewDBSetting.DBName = DatabaseFunctions.defaultDBName;
                            objNewDBSetting.Port = 0;
                            objNewDBSetting.UserId = DatabaseFunctions.defaultDBUser;
                            objNewDBSetting.password = DatabaseFunctions.defaultDBSaPWD;
                            string Data = Newtonsoft.Json.JsonConvert.SerializeObject(objNewDBSetting);
                            File.WriteAllText(Constant.CustomSettingsPath + @"\Configuration.dat", Common.EncryptData(Data));
                            Common.SetFullAccessForFile(Constant.CustomSettingsPath + @"\Configuration.dat");
                            objDBSetting = objNewDBSetting;
                            IsConnectionStringSet = true;
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show(ex.Message);
                            Log.Error(ex.Message, ex);
                            System.Windows.Application.Current.Shutdown();
                        }
                    }
                    else
                    {
                        if (DBFunctions.CreateDefaultDB(DatabaseFunctions.defaultDBServer, 0, DatabaseFunctions.defaultDBName, DatabaseFunctions.defaultDBUser, DatabaseFunctions.defaultDBSaPWD))
                        {
                            if (DBFunctions.SetSQLSchema(DatabaseFunctions.defaultDBServer, 0, DatabaseFunctions.defaultDBName, DatabaseFunctions.defaultDBUser, DatabaseFunctions.defaultDBSaPWD))
                            {
                                if (DBFunctions.InsertDefaultData(DatabaseFunctions.defaultDBServer, 0, DatabaseFunctions.defaultDBName, DatabaseFunctions.defaultDBUser, DatabaseFunctions.defaultDBSaPWD))
                                {
                                    try
                                    {
                                        DBSettingData objNewDBSetting = new DBSettingData();
                                        objNewDBSetting.Server = DatabaseFunctions.defaultDBServer;
                                        objNewDBSetting.DBName = DatabaseFunctions.defaultDBName;
                                        objNewDBSetting.Port = 0;
                                        objNewDBSetting.UserId = DatabaseFunctions.defaultDBUser;
                                        objNewDBSetting.password = DatabaseFunctions.defaultDBSaPWD;
                                        string Data = Newtonsoft.Json.JsonConvert.SerializeObject(objNewDBSetting);
                                        File.WriteAllText(Constant.CustomSettingsPath + @"\Configuration.dat", Common.EncryptData(Data));
                                        Common.SetFullAccessForFile(Constant.CustomSettingsPath + @"\Configuration.dat");
                                        objDBSetting = objNewDBSetting;
                                        IsConnectionStringSet = true;
                                    }
                                    catch (Exception ex)
                                    {
                                        //MessageBox.Show(ex.Message);
                                        Log.Error(ex.Message, ex);
                                        System.Windows.Application.Current.Shutdown();
                                    }
                                }
                                else
                                {
                                    dbCreationError = "Unable to set default value, please contact support team";
                                }
                            }
                            else
                            {
                                dbCreationError = "Unable to create database schema, please contact support team.";
                            }
                        }
                        else
                        {
                            dbCreationError = "Unable to create database, please contact support team.";
                        }
                    }
                }
                else
                {
                    dbCreationError = "No server found";
                }
                #endregion
            }

            if (!String.IsNullOrEmpty(dbCreationError))
                Log.Error(dbCreationError);

            if (IsConnectionStringSet)
            {
                SetApplicationVariable(objDBSetting);
                CheckOnlineAnprLicense();
            }
            else
            {
                this.StartupUri = new Uri("DBConfigureation.Xaml", UriKind.Relative);
                splashScreen.Close(new TimeSpan(0, 0, 0, 0, 100));
                //MessageBox.Show(dbCreationError);
                return;
            }
        }


        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            Log.Error(ex.Message, ex);
            MessageBox.Show("Unexpected error occured. Please inform the admin."
        + Environment.NewLine + e.Exception.Message, "Unexpected error");
            e.Handled = true;
        }

        #region Set Application Variable
        List<ApplicationSetting> GlobalAppSettings = null;
        public void SetApplicationVariable(DBSettingData objDBData)
        {
            string Datastring;
            if (objDBSetting.Port > 0)
                Datastring = "Data Source=" + objDBSetting.Server + "," + objDBSetting.Port + ";Initial Catalog=" + objDBSetting.DBName + ";user id=" + objDBSetting.UserId + ";password=" + objDBSetting.password + ";MultipleActiveResultSets=true;";
            else
                Datastring = "Data Source=" + objDBSetting.Server + ";Initial Catalog=" + objDBSetting.DBName + ";user id=" + objDBSetting.UserId + ";password=" + objDBSetting.password + ";MultipleActiveResultSets=true;";

            try
            {
                using (SqlConnection tempCon = new SqlConnection(Datastring))
                {
                    if (tempCon.State != ConnectionState.Open)
                        tempCon.Open();
                    var table = new DataTable();
                    using (var da = new SqlDataAdapter("SELECT * FROM ApplicationSettings", tempCon))
                    {
                        da.Fill(table);
                        GlobalAppSettings = CollectionExtensions.GetEntities<ApplicationSetting>(table);
                    }
                    tempCon.Close();
                }
            }
            catch (Exception ex)
            {
                Common.logErrorInDB(ex.Message, ex);
            }

            if (GlobalAppSettings != null && GlobalAppSettings.Count > 0)
            {
                Hierarchy hier = LogManager.GetRepository() as Hierarchy;

                if (hier != null)
                {
                    //get ADONetAppender
                    var adoAppender = (AdoNetAppender)hier.GetAppenders().Where(appender => appender.Name.Equals("AdoNetAppender", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                    if (adoAppender != null)
                    {
                        // update connectionstring
                        adoAppender.ConnectionString = Datastring;
                        //refresh settings of appender
                        adoAppender.ActivateOptions();
                    }
                }

                Application.Current.Properties["DefalutConnectionString"] = Datastring;

                EntityConnectionStringBuilder entityStringBuilder = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["weighbridgeEntities"].ConnectionString);
                entityStringBuilder.ProviderConnectionString = Datastring;
                Application.Current.Properties["EntityConnectionString"] = entityStringBuilder.ToString();
                Constant.EntityConnectionString = entityStringBuilder.ToString();

                ApplicationSetting dateformat = GlobalAppSettings.FirstOrDefault(x => x.ModuleName == PageEntity.GlobalSetting.ToString() && x.SettingName == "DateFormat");
                ApplicationSetting timeformat = GlobalAppSettings.FirstOrDefault(x => x.ModuleName == PageEntity.GlobalSetting.ToString() && x.SettingName == "TimeFormat");
                if (dateformat != null && !string.IsNullOrEmpty(dateformat.SettingValue) && timeformat != null && !string.IsNullOrEmpty(timeformat.SettingValue))
                {
                    Constant.SelectedDateFormat = dateformat.SettingValue;
                    Constant.SelectedTimeFormat = timeformat.SettingValue;
                }
                else if (dateformat != null && !string.IsNullOrEmpty(dateformat.SettingValue))
                    Constant.SelectedDateFormat = dateformat.SettingValue;
                else if (timeformat != null && !string.IsNullOrEmpty(timeformat.SettingValue))
                    Constant.SelectedTimeFormat = timeformat.SettingValue;
                else
                {
                    Constant.SelectedDateFormat = "dd-MM-yyyy";
                    Constant.SelectedTimeFormat = "hh:mm tt";
                }
                Constant.DateTimeFormat = Constant.SelectedDateFormat + " " + Constant.SelectedTimeFormat;
                ApplicationSetting DecimalFormat = GlobalAppSettings.FirstOrDefault(x => x.ModuleName == PageEntity.GlobalSetting.ToString() && x.SettingName == "DecimalFormat");
                if (DecimalFormat != null && !string.IsNullOrEmpty(DecimalFormat.SettingValue))
                    Constant.DecimalFormat = DecimalFormat.SettingValue;
                else
                    Constant.DecimalFormat = "0.####";

                bool IsUnmannedPlugin = LicensePluginHalper.CheckPluginInstall(PluginNameEnum.UnmannedPlugin.ToString());

                if (IsUnmannedPlugin)
                {
                    string UnmannedBypass = "NO";
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["IsUnmannedBypass"]))
                    {
                        UnmannedBypass = ConfigurationManager.AppSettings["IsUnmannedBypass"].ToString();
                    }

                    if (UnmannedBypass == "YES")
                        Constant.hasUnmannedPlugin = false;
                    else
                        Constant.hasUnmannedPlugin = true;
                }
                else
                {
                    Constant.hasUnmannedPlugin = false;
                }

                this.StartupUri = new Uri("Login.Xaml", UriKind.Relative);
                splashScreen.Close(new TimeSpan(0, 0, 0, 0, 100));
            }
            else
            {
                this.StartupUri = new Uri("DBConfigureation.Xaml", UriKind.Relative);
                splashScreen.Close(new TimeSpan(0, 0, 0, 0, 100));
                //MessageBox.Show("Unable to connect database, please provide database information");
            }
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            Constant.AssemblyVersion = fvi.FileVersion;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (GlobalAppSettings != null && GlobalAppSettings.Count > 0)
            {
                ApplicationSetting Color = GlobalAppSettings.FirstOrDefault(x => x.ModuleName == "ThemeSetting" && x.SettingName == "Color");
                ApplicationSetting BaseColor = GlobalAppSettings.FirstOrDefault(x => x.ModuleName == "ThemeSetting" && x.SettingName == "BaseColor");
                if (Color != null && BaseColor != null && !String.IsNullOrEmpty(Color.SettingValue) && !String.IsNullOrEmpty(BaseColor.SettingValue))
                    ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(Color.SettingValue), ThemeManager.GetAppTheme(BaseColor.SettingValue));
            }
            Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);
            DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(e);
                File.AppendAllText(Constant.ApplicationDataLocation + $"\\Current_DispatcherUnhandledException_{DateTime.Now.Date:yyyy_MM_dd}.txt", json);
            }
            catch { }
            e.Handled = true;
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(e);
                File.AppendAllText(Constant.ApplicationDataLocation + $"\\App_DispatcherUnhandledException_{DateTime.Now.Date:yyyy_MM_dd}.txt", json);
            }
            catch { }
            e.Handled = true;
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(e);
                File.AppendAllText(Constant.ApplicationDataLocation + $"\\CurrentDomain_UnhandledException_{DateTime.Now.Date:yyyy_MM_dd}.txt", json);
            }
            catch { }
            // we cannot handle this, but not to worry, I have not encountered this exception yet.  
            // However, you can show/log the exception message and show a message that if the application is terminating or not.  

            var isTerminating = e.IsTerminating;
        }
        #endregion

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            EventAggregator.GetEvent<Events.ApplicationExit>().Publish();
            if (hasHandle)
            {
                PipeManager.StopServer();
                singletonMutex.ReleaseMutex();
            }
        }

        public static void RestartApplication(string Arguments = "", bool RunAsAdministrator = false)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            if (RunAsAdministrator)
                startInfo.Verb = "runas";
            if (!String.IsNullOrEmpty(Arguments))
                startInfo.Arguments = Arguments;
            try
            {
                Process p = Process.Start(startInfo);
                //p.WaitForInputIdle(500);
                System.Windows.Application.Current.Shutdown();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                return;
            }
        }

        public static void CheckOnlineAnprLicense()
        {
            try
            {
                Constant.HasAnprLicense = false;

                string Anprlicensepath = Constant.CustomSettingsPath + @"\Anprlicense.lic";

                string Passkey = AnprLicenseHelper.Get_AnprPassKey();

                CheckAnprLicense_Response checkAnprLicense_Response = AnprLicenseHelper.CheckAnprLicense();

                if (checkAnprLicense_Response != null && checkAnprLicense_Response.success == true)
                {
                    string AnprLicense = JsonConvert.SerializeObject(checkAnprLicense_Response);

                    string statusDescription = CommonEncription.EncryptData(AnprLicense);

                    if (File.Exists(Anprlicensepath))
                    {
                        string licenseKey = File.ReadAllText(Anprlicensepath);
                        if (!licenseKey.Equals(statusDescription))
                        {
                            File.WriteAllText(Anprlicensepath, statusDescription);
                            Common.SetFullAccessForFile(Anprlicensepath);
                        }
                    }
                    else
                    {
                        File.WriteAllText(Anprlicensepath, statusDescription);
                        Common.SetFullAccessForFile(Anprlicensepath);
                    }
                }

                if (File.Exists(Anprlicensepath))
                {
                    try
                    {
                        string AnprlicenseKey = File.ReadAllText(Anprlicensepath);
                        string AnprLicensestring = CommonEncription.DecryptData(AnprlicenseKey);

                        CheckAnprLicense_Response AnprLicense = JsonConvert.DeserializeObject<CheckAnprLicense_Response>(AnprLicensestring);

                        string Licensekey = CommonEncription.DecryptData(AnprLicense.LicenseKey);
                        DateTime ExpDate = Convert.ToDateTime(Licensekey);
                        if (ExpDate > DateTime.Now && AnprLicense.message == Passkey)
                        {
                            Constant.HasAnprLicense = true;
                        }
                        else
                        {
                            File.Delete(Anprlicensepath);
                        }
                    }
                    catch
                    {
                        File.Delete(Anprlicensepath);
                    }
                }
            }
            catch { }
        }


        public void CheckOnlineLicense()
        {
            //string URL = "http://license.e-global.in/handshake.ashx";
            string URL = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ServerUri"]) ? ConfigurationManager.AppSettings["ServerUri"] : "https://edspl.license.weighmast.com/handshake.ashx";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            string licensepath = Constant.CustomSettingsPath + @"\license.lic";
            try
            {
                request.Headers.Add("shakeID", WhoAmI.fingerPrintValue());
                request.Method = "GET";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string statusDescription = "";
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        statusDescription = reader.ReadToEnd();
                    }
                    if (statusDescription == "License Not Found")
                    {
                        File.AppendAllText(Constant.CustomSettingsPath + @"\LicenseInfo.txt", Environment.NewLine + WhoAmI.fingerPrintValueFile());
                        string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + "License not registered in our system.";
                        File.AppendAllText(Constant.CustomSettingsPath + @"\LicenseError.txt", json);
                        File.Delete(licensepath);
                    }
                    else if (statusDescription != "Weighing Solution Portal" && !statusDescription.Contains("html"))
                    {
                        if (!File.Exists(licensepath))
                        {
                            File.WriteAllText(licensepath, statusDescription);
                            Common.SetFullAccessForFile(licensepath);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + Newtonsoft.Json.JsonConvert.SerializeObject(ex);
                File.WriteAllText(Constant.CustomSettingsPath + @"\LicenseError.txt", json);
            }
        }

        private static bool CertificateValidationCallBack(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }
            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                if (chain != null && chain.ChainStatus != null)
                {
                    foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
                    {
                        if ((certificate.Subject == certificate.Issuer) &&
                           (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot))
                        {
                            continue;
                        }
                        else
                        {
                            if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
