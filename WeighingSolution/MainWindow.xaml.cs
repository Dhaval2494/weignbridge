using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
using Newtonsoft.Json;
using PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.IO.Ports;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Navigation;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;
using WeighingSolution.Report;
using WeighingSolution.SharedComponents;
using WeighingSolution.ViewModel;

namespace WeighingSolution
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public weighbridgeEntities ctxGlobal = new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString());
        private PluginLoader<IPluginControl> mef = new PluginLoader<IPluginControl>("MainWindow");

        MainWindowViewModel viewModel;
        Thread licenceCheckThread;
        Thread UnmandCheckThread;
        Thread TakeDBBackup;
        Thread UpdateRecord;
        Thread AnprThread;
        Thread SendAnprReportThread;
        Thread _WeightToJumbo1;
        Thread WeightThread;

        public string LocalTicketid;
        public int ONlyLocalTicketid;
        public DataTable onlyDataTable;
        public string AnprImageLocation = string.Empty;
        public bool NetWeightNegative = false;
        public static ModbusHelper modbusHelper = new ModbusHelper();
        System.Timers.Timer timerUnman = new System.Timers.Timer();
        public static WeightBridge ActiveWeighBridge;
        private ObservableCollection<ApplicationSetting> LiveWeightSetting;
        System.Timers.Timer timerModbus = new System.Timers.Timer();
        public static string GlobalRFIDToken = string.Empty;
        public ObservableCollection<ApplicationSetting> EmailSetting;
        public ObservableCollection<ApplicationSetting> GlobalSettings;
        private static Dictionary<string, string> RFIDTokens = new Dictionary<string, string>();
        Thread SendEmail;
        public ReportViewer reportViewerLocal;
        public MainWindow(int UserID)
        {
            InitializeComponent();

            this.Title = "Weighbridge Solution" + " [Version: " + Constant.AssemblyVersion + "]";

            this.Unloaded += CurrentPage_Unloaded;
            this.Closing += MainWindow_Closing;
            this.Loaded += CurrentPage_Loaded;
            ToastHelper.InitToaster();
            viewModel = new MainWindowViewModel(this, UserID);
            this.DataContext = viewModel;
            Application.Current.MainWindow = this;
            //!Constant.IsBasicversion &&
            //if ((Constant.IsSupportUser || Constant.IsSuperAdmin) &&
            //    (viewModel.CompanyName == "Company Name") && (viewModel.Address == "Address") && (viewModel.Phone == "1234567890") && (viewModel.Logo == "logo.png") && (viewModel.Email == "ID@server.com"))
            //{
            //    stkEditComapnysetting.Visibility = Visibility.Visible;
            //}

            if (Constant.hasUnmannedPlugin)
            {
                btnResetTransaction.Visibility = Visibility.Visible;
                lblModBusText.Visibility = Visibility.Visible;
                lblRFIDMSGText.Visibility = Visibility.Visible;
                uhfstk.Visibility = Visibility.Visible;
            }

            if (Common.ValidatePageRequest(PageEntity.Transaction, PageAccess.Read))
            {
                btnTransaction.Visibility = Visibility.Visible;
                if (Common.ValidatePageRequest(PageEntity.Transaction, PageAccess.Create))
                    MainFrame.Visibility = Visibility.Visible;
            }
            if (Common.ValidatePageRequest(PageEntity.DataSetup, PageAccess.Read))
                btnDataSetup.Visibility = Visibility.Visible;
            if (Common.ValidatePageRequest(PageEntity.Reports, PageAccess.Read))
                btnReports.Visibility = Visibility.Visible;
            if (Common.ValidatePageRequest(PageEntity.Settings, PageAccess.Read))
                btnSettings.Visibility = Visibility.Visible;
            if (Common.ValidatePageRequest(PageEntity.Settings, PageAccess.Read) && Constant.hasUnmannedPlugin)
                btnMonitor.Visibility = Visibility.Visible;
            if (Common.ValidatePageRequest(PageEntity.Settings, PageAccess.Read) && Constant.hasUnmannedPlugin)
                btndashbord.Visibility = Visibility.Visible;
            if (Common.ValidatePageRequest(PageEntity.Transaction, PageAccess.Create))
            {
                btnCreateTransaction.Visibility = Visibility.Visible;
                rectang.Visibility = Visibility.Visible;
            }


            try
            {
                string sourceFileName = System.AppDomain.CurrentDomain.BaseDirectory + "\\Report\\Receipt\\manual.pdf";
                if (!File.Exists(sourceFileName))
                {
                    btnCreatepdfDownload.Visibility = Visibility.Collapsed;
                }
            }
            catch { }

            try
            {
                if (Constant.AmcExpire != null)
                {
                    if (Constant.AmcExpire != DateTime.MinValue && Constant.AmcExpire != new DateTime(1900, 1, 1))
                    {
                        Amcdate.Content = Constant.AmcExpire.ToString(Constant.SelectedDateFormat, CultureInfo.InvariantCulture);

                        if (Constant.AmcExpire < DateTime.Now)
                        {
                            btnamcexpire.Visibility = Visibility.Visible;

                            StartBlinking(Amcdate);
                            // txamcdate.Foreground = new SolidColorBrush(Colors.Red);
                        }
                        else
                        {
                            txamcdate.Foreground = new SolidColorBrush(Colors.Green);
                        }
                    }
                }
            }
            catch { }

            try
            {
                foreach (var plugin in mef.loadedPlugins.Keys)
                {
                    plugin.SetupPlugin(this);
                }
                timerUnman.Elapsed += new System.Timers.ElapsedEventHandler(CheckUnmaned);
                timerUnman.Interval = 1000;  //30 Secound
                timerUnman.Start();
                timerUnman.Enabled = true;

                if (Constant.hasUnmannedPlugin)
                {
                    try
                    {
                        using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                        {
                            ObservableCollection<WeightBridgeConfigBindingModel> WeightBridgesConfig = vmGlobelSetting.ReadWeightBridgeConfig();
                            EmailSetting = new ObservableCollection<ApplicationSetting>(ctx.ApplicationSettings.Where(x => x.ModuleName == "EmailSetting").ToList());
                            GlobalSettings = new ObservableCollection<ApplicationSetting>(ctx.ApplicationSettings.Where(x => x.ModuleName == PageEntity.GlobalSetting.ToString()).ToList());


                            if (WeightBridgesConfig != null && WeightBridgesConfig.Count() > 0)
                            {
                                var _WeightBridgeID = WeightBridgesConfig.Where(p => p.SerialNo == WhoAmI.fingerPrintValue()).FirstOrDefault().WeightBridgeID;
                                WeightBridge _weightBridge = ctx.WeightBridges.Where(x => x.WeightBridgeID == _WeightBridgeID && x.IsActive).Count() > 0 ?
                                    ctx.WeightBridges.Where(x => x.WeightBridgeID == _WeightBridgeID && x.IsActive).FirstOrDefault() : null;
                                if (_weightBridge != null && !string.IsNullOrEmpty(_weightBridge.Plc_Port))
                                {

                                    if (_weightBridge.Modbus_ISBypass != true)
                                    {
                                        modbusHelper.PortName = _weightBridge.Plc_Port;
                                        modbusHelper._LTG = !string.IsNullOrEmpty(_weightBridge.LTG) ? _weightBridge.LTG : "0";
                                        modbusHelper._LTR = !string.IsNullOrEmpty(_weightBridge.LTR) ? _weightBridge.LTR : "1";
                                        modbusHelper._RTG = !string.IsNullOrEmpty(_weightBridge.RTG) ? _weightBridge.RTG : "2";
                                        modbusHelper._RTR = !string.IsNullOrEmpty(_weightBridge.RTR) ? _weightBridge.RTR : "3";
                                        modbusHelper._LBO = !string.IsNullOrEmpty(_weightBridge.LBO) ? _weightBridge.LBO : "4";
                                        modbusHelper._LBC = !string.IsNullOrEmpty(_weightBridge.LBC) ? _weightBridge.LBC : "5";
                                        modbusHelper._RBO = !string.IsNullOrEmpty(_weightBridge.RBO) ? _weightBridge.RBO : "6";
                                        modbusHelper._RBC = !string.IsNullOrEmpty(_weightBridge.RBC) ? _weightBridge.RBC : "7";



                                        modbusHelper.DoConnect();
                                        Constant.Final_Reset = true;

                                        timerModbus.Elapsed += new System.Timers.ElapsedEventHandler(CheckModbus);
                                        timerModbus.Interval = 500; //1000;  //30 Secound
                                        timerModbus.Start();
                                        timerModbus.Enabled = true;

                                        Constant.IsUnmanConnect = true;

                                        try
                                        {
                                            string imageLocation = Constant.ApplicationDataLocation + @"\Modbus\";
                                            System.IO.DirectoryInfo folderInfo = new DirectoryInfo(imageLocation);
                                            foreach (FileInfo file in folderInfo.GetFiles())
                                            {
                                                if (file.CreationTime <= DateTime.Now.AddDays(-1))
                                                    file.Delete();
                                            }
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        Constant.IsFirstSensorOn = false;
                        Constant.IsSecoundSensorOn = false;
                        Constant.IsTrirdSensorOn = false;
                    }
                }
            }
            catch (Exception Ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(Ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\MainPageError.txt", json);
                //exec sp_updatestats
            }
        }

        private void StartBlinking(Label control)
        {
            var animation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.3,
                Duration = new Duration(TimeSpan.FromSeconds(0.6)),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            control.Foreground = new SolidColorBrush(Colors.Red);
            control.Effect = new DropShadowEffect
            {
                Color = Colors.Red,
                BlurRadius = 15,
                ShadowDepth = 0
            };

            control.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        public void UpdateRecordThread()
        {
            var data = ctxGlobal.ApplicationSettings.Any(x => x.ModuleName == "GlobalSetting" && x.SettingName == "RunCustomValue");
            if (data)
            {
                var record = ctxGlobal.ApplicationSettings.FirstOrDefault(x => x.ModuleName == "GlobalSetting" && x.SettingName == "RunCustomValue");
                if (record != null && int.TryParse(record.SettingValue, out int settingValue))
                {
                    if (settingValue <= 0)
                    {

                        bool Result = DatabaseFunctions.InsertUpdateDataSettingData(Constant.DBSERVER, Constant.DBPORT, Constant.DBNAME, Constant.DBUSERNAME, Constant.DBPASSWORD);
                        if (Result)
                        {
                            record.SettingValue = "1";

                            string UpdateQuery = "Update ApplicationSettings set SettingValue = '1' where ModuleName = 'GlobalSetting' and SettingName = 'RunCustomValue' ";
                            DatabaseFunctions.ExecuteQuery(UpdateQuery);

                            // ctxGlobal.SaveChanges();

                        }
                    }
                }
            }
        }


        public static bool weighCommand
        {
            set
            {
                if (value == true)
                {
                    ReleaseReader();
                    Thread.Sleep(1000);
                }
                if (value == false)
                {
                    Thread.Sleep(1000);
                    InitializeReader();
                }
            }
        }
        private void CheckUnmaned(object sender, System.Timers.ElapsedEventArgs e)
        {
            timerUnman.Enabled = false;
            try
            {
                ChechUnmanedServiceStatus();
            }
            catch (Exception)
            {

            }
            timerUnman.Enabled = true;
        }

        private void HandleNamedPipe_OpenRequest(string command)
        {
            Dispatcher.Invoke(() =>
            {
                if (command.ToUpper() == "ACTIVATE")
                {
                    if (WindowState == WindowState.Minimized)
                        WindowState = WindowState.Normal;

                    this.Topmost = true;
                    this.Activate();
                    Dispatcher.BeginInvoke(new Action(() => { this.Topmost = false; }));
                }
                else if (command == "Transaction Save")
                {
                    Dispatcher.BeginInvoke(new Action(async () => { await viewModel.LoadAsync(); }));

                    viewModel.TxSearchText = string.Empty;
                }
                else if (command == "Database Auto-Backup Completed Successfully")
                {
                    ToastHelper.ShowSuccess(command);
                }
            });
        }
        async void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    ActiveWeighBridge = Get_Currunt_Weightbridge();
                }
                catch { }
                //if (ctxGlobal.WeightBridges.FirstOrDefault() != null)
                //    this.MainFrame.Content = new View.TransactionManagement();
                //else
                //    this.MainFrame.Content = new View.DataSetup_Page();
                //await viewModel.LoadAsync();

                if (ActiveWeighBridge != null)
                {
                    if (Common.ValidatePageRequest(PageEntity.Transaction, PageAccess.Read))
                        this.MainFrame.Content = new View.TransactionManagementList_Page();
                    else if (Common.ValidatePageRequest(PageEntity.DataSetup, PageAccess.Read))
                        this.MainFrame.Content = new View.DataSetup_Page();
                    else
                        this.MainFrame.Content = new AccessDenied();
                }
                else
                {
                    if (Common.ValidatePageRequest(PageEntity.GlobalSetting, PageAccess.Create))
                        this.MainFrame.Content = new View.GlobelSetting_Page();
                }

                Application.Current.MainWindow = this;


                LiveWeightSetting = new ObservableCollection<ApplicationSetting>(ctxGlobal.ApplicationSettings.Where(x => x.ModuleName == "LiveWeightSetting").ToList());
                await viewModel.LoadAsync();

                if (ActiveWeighBridge != null && viewModel.SelectedWeighBridge == null)
                {
                    //SelectedWeighBridge = ActiveWeighBridges.Where(x => x.IsActive).FirstOrDefault();
                    viewModel.SelectedWeighBridge = ActiveWeighBridge;

                    WeightThread = new Thread(new ThreadStart(WeightThreadStart));
                    WeightThread.Start();
                }

                weighCommand = false;

                App.PipeManager.ReceiveString += HandleNamedPipe_OpenRequest;

                UnmandCheckThread = new Thread(new ThreadStart(ChechUnmanedServiceStatus));
                UnmandCheckThread.Start();

                TakeDBBackup = new Thread(new ThreadStart(TakeDB_Backup));
                TakeDBBackup.SetApartmentState(ApartmentState.STA);
                TakeDBBackup.Start();


                // Live Weight into Cloud 
                string _LiveWeightCloud = "NO";
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["LiveWeightCloud"]))
                {
                    _LiveWeightCloud = ConfigurationManager.AppSettings["LiveWeightCloud"].ToString();
                }

                if (_LiveWeightCloud == "YES")
                {
                    await Task.Run(LiveWeightAsync);
                }
                licenceCheckThread = new Thread(new ThreadStart(LicenseCheckThreadStart));
                licenceCheckThread.Start();

                UpdateRecord = new Thread(new ThreadStart(UpdateRecordThread));
                UpdateRecord.Start();


                if (ActiveWeighBridge != null)
                {

                    if (ActiveWeighBridge.JumboDisplay == true)
                    {
                        _WeightToJumbo1 = new Thread(new ThreadStart(WeightToJumbo));
                        _WeightToJumbo1.Start();
                    }

                    AnprImageLocation = getAnprImagePath();

                    string _AnprTesting = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["AnprTesting"]) ? ConfigurationManager.AppSettings["AnprTesting"] : "NO";

                    if (_AnprTesting == "YES")
                    {
                        AnprThread = new Thread(new ThreadStart(ProcessAnprThread));
                        AnprThread.Start();
                    }

                    string IsServer = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["IsServer"]) ? ConfigurationManager.AppSettings["IsServer"] : "NO";
                    if (IsServer == "YES")
                    {
                        SendAnprReportThread = new Thread(new ThreadStart(SendAnprReport));
                        SendAnprReportThread.Start();
                    }
                }

                // Live Weight into File 
                string _LiveWeightFile = "NO";
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["LiveWeightFile"]))
                {
                    _LiveWeightFile = ConfigurationManager.AppSettings["LiveWeightFile"].ToString();
                }

                if (_LiveWeightFile == "YES")
                {
                    await Task.Run(LiveWeightFile);
                }

                bool isProviderInstalled = IsACEOLEDBProviderInstalled();

                if (!isProviderInstalled)
                {
                    InstallAccessDatabaseEngine();
                }
                RemoveDeletedPluginFiles();

                if (Constant.hasUnmannedPlugin)
                {
                    if (ActiveWeighBridge != null)
                    {
                        try
                        {
                            using (weighbridgeEntities ctxuhf = new weighbridgeEntities(Constant.EntityConnectionString))
                            {
                                UpdateOldToken();
                                try
                                {
                                    using (weighbridgeEntities ctToken = new weighbridgeEntities(Constant.EntityConnectionString))
                                    {
                                        TokenLists = ctToken.Tokens.ToList();
                                    }
                                }
                                catch { }

                                List<UhfMaster> LeftuhfMasters = ctxuhf.UhfMasters.Where(p => p.WeightbridgeID == ActiveWeighBridge.WeightBridgeID && p.WeightbridgeSide == "Left" && p.IsActive == true && p.ISBypass != true).OrderBy(p => p.ID).ToList();
                                if (LeftuhfMasters.Count() > 0)
                                {
                                    ThreadProcessUHF = new Thread(new ThreadStart(StartProcessUHF));
                                    ThreadProcessUHF.Start();
                                    UhfMaster uhfMasterLeft = LeftuhfMasters.FirstOrDefault();

                                    ThreadUHFLeft = new Thread(new ParameterizedThreadStart(StartUHFLeft));
                                    ThreadUHFLeft.Start(uhfMasterLeft);
                                }

                                List<UhfMaster> RightUhfMasters = ctxuhf.UhfMasters.Where(p => p.WeightbridgeID == ActiveWeighBridge.WeightBridgeID && p.WeightbridgeSide == "Right" && p.IsActive == true && p.ISBypass != true).OrderBy(p => p.ID).ToList();
                                if (RightUhfMasters.Count() > 0)
                                {
                                    ThreadProcessUHF = new Thread(new ThreadStart(StartProcessUHF));
                                    ThreadProcessUHF.Start();

                                    UhfMaster uhfMasterRight = RightUhfMasters.FirstOrDefault();

                                    ThreadUHFRight = new Thread(new ParameterizedThreadStart(StartUHFRight));
                                    ThreadUHFRight.Start(uhfMasterRight);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                            File.AppendAllText(Constant.ApplicationDataLocation + @"\UhfError.txt", json);
                            viewModel.UHF1Stutas = "UHF1 Disconnceted";
                            viewModel.UHF2Stutas = "UHF 2 Disconnceted";
                        }

                    }
                }


            }
            catch (Exception Ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(Ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\MainPageError.txt", json);
                string queryu = $"exec sp_updatestats";
                bool flagu = DatabaseFunctions.ExecuteQuery(queryu);
                string query = $"dbcc freeproccache";
                bool flag = DatabaseFunctions.ExecuteQuery(query);
                //exec sp_updatestats
            }
        }

        private void WeightThreadStart()
        {
            try
            {
                while (true)
                {
                    //if(!ActiveWeighBridge.IsCablePlugged)
                    //{
                    //    weighCommand = false;
                    //}

                    if (viewModel.SelectedWeighBridge != null && ActiveWeighBridge != null)
                    {
                        viewModel.SelectedWeighBridge.IsCablePlugged = ActiveWeighBridge.IsCablePlugged;
                        if (viewModel.SelectedWeighBridge.IsCablePlugged)
                            viewModel.SelectedWeighBridge.FormatedData = ActiveWeighBridge.FormatedData;
                        else
                            viewModel.SelectedWeighBridge.FormatedData = "";
                    }
                    Thread.Sleep(500);
                }
            }
            catch { }
        }

        public WeightBridge Get_Currunt_Weightbridge()
        {
            WeightBridge weightBridge = null;
            try
            {
                ObservableCollection<WeightBridgeConfigBindingModel> WeightBridgesConfig = vmGlobelSetting.ReadWeightBridgeConfig();
                if (WeightBridgesConfig != null && WeightBridgesConfig.Count > 0)
                {
                    var _WeightBridgeID = WeightBridgesConfig.Where(p => p.SerialNo == WhoAmI.fingerPrintValue()).FirstOrDefault().WeightBridgeID;
                    using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                    {
                        if (ctx.WeightBridges.Where(p => p.WeightBridgeID == _WeightBridgeID).Count() > 0)
                        {
                            weightBridge = ctx.WeightBridges.Where(p => p.WeightBridgeID == _WeightBridgeID).FirstOrDefault();
                            weightBridge.Unit = ctx.Units.Where(p => p.UnitID == weightBridge.UnitID).FirstOrDefault();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }
            return weightBridge;
        }





        /// <summary>
        /// Creates the remote folder (and any missing parent segments) if it doesn't already exist.
        /// </summary>
        private void EnsureRemoteDirectoryExists(string remoteFolderUrl, string userId, string password)
        {
            Uri uri = new Uri(remoteFolderUrl);
            string[] segments = uri.AbsolutePath.Trim('/').Split('/');
            string current = $"{uri.Scheme}://{uri.Host}";

            foreach (var segment in segments)
            {
                current += "/" + segment;
                try
                {
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(current);
                    request.Method = WebRequestMethods.Ftp.MakeDirectory;
                    request.Credentials = new NetworkCredential(userId, password);
                    using (var response = (FtpWebResponse)request.GetResponse())
                    {
                        LogSync($"Created remote folder: {current}");
                    }
                }
                catch (WebException ex)
                {
                    var resp = ex.Response as FtpWebResponse;
                    if (resp != null && resp.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    {
                        // 550 - folder already exists, this is expected and fine, don't log noise
                    }
                    else
                    {
                        LogSync($"WARN - could not create remote folder {current} - {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Checks if a file already exists on the FTP server.
        /// </summary>
        private bool FtpFileExists(string remoteFileUrl, string userId, string password)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(remoteFileUrl);
                request.Method = WebRequestMethods.Ftp.GetFileSize;
                request.Credentials = new NetworkCredential(userId, password);

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    return true;
                }
            }
            catch (WebException ex)
            {
                var resp = ex.Response as FtpWebResponse;
                if (resp != null && resp.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    return false; // 550 - file not found, expected case

                // Any other error - log it, treat as "not confirmed" so we don't silently skip
                LogSync($"WARN - could not check existence of {remoteFileUrl} - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Central logging helper - every meaningful event goes through here.
        /// </summary>
        private void LogSync(string message)
        {
            try
            {
                string line = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} | {message}{Environment.NewLine}";
                File.AppendAllText(SyncConstant.traceFileLocation, line);
            }
            catch
            {
                // last resort - never let logging itself crash the sync
            }
        }

        private static void InitializeReader()
        {
            try
            {
                if (ActiveWeighBridge != null)
                    ActiveWeighBridge.InitializeReaders(true);
            }
            catch { }
        }
        private static void ReleaseReader()
        {
            try
            {
                if (ActiveWeighBridge != null)
                    ActiveWeighBridge.ReleaseReaders();
            }
            catch { }
        }

        static bool IsACEOLEDBProviderInstalled()
        {
            //try
            //{
            //    using (var key = Registry.ClassesRoot.OpenSubKey("Microsoft.ACE.OLEDB.12.0"))
            //    {
            //        string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(key);
            //        File.AppendAllText(Constant.ApplicationDataLocation + @"\ExcelCSVIsACEOLEDBProviderInstalledError12.txt", json);
            //        return key != null;

            //    }
            //}
            string[] providers = {
        "Microsoft.ACE.OLEDB.12.0",
    };

            string[] registryRoots = {
        @"HKEY_CLASSES_ROOT\",
        @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\",
        @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Classes\"
    };

            try
            {
                foreach (var provider in providers)
                {
                    foreach (var root in registryRoots)
                    {
                        using (var key = Registry.ClassesRoot.OpenSubKey(provider) ??
                                        Registry.LocalMachine.OpenSubKey(root + provider))
                        {
                            if (key != null)
                            {
                                string log = Environment.NewLine + "Found: " + provider + " at " + root + " - " + DateTime.Now;
                                File.AppendAllText(Constant.ApplicationDataLocation + @"\ExcelCSVProviderCheckLog.txt", log);
                                return true;
                            }
                        }
                    }
                }

                // Not found
                File.AppendAllText(Constant.ApplicationDataLocation + @"\ExcelCSVProviderCheckLog.txt",
                    Environment.NewLine + "None of the ACE OLEDB providers found - " + DateTime.Now);
                return false;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur during registry access

                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\ExcelCSVIsACEOLEDBProviderInstalledError.txt", json);
                return false;
            }
        }

        #region Excel Engine Download 
        static void InstallAccessDatabaseEngine()
        {
            try
            {
                string installerName = "AccessDatabaseEngine.exe";
                string installerPath = Path.Combine(AppContext.BaseDirectory, installerName);

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = installerPath,
                    Arguments = "/quiet",
                    Verb = "runas",
                    UseShellExecute = true
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    process.WaitForExit();

                    int exitCode = process.ExitCode;
                    if (exitCode == 0)
                    {
                        string json = Environment.NewLine + "Access Database Engine installed successfully.";
                        File.AppendAllText(Constant.ApplicationDataLocation + @"\ExcelCSVInstallAccessDatabaseEngineLogError_install.txt", json);
                    }
                    else
                    {

                        string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(exitCode);
                        File.AppendAllText(Constant.ApplicationDataLocation + @"\ExcelCSVInstallAccessDatabaseEngineLogError_notinstall.txt", json);
                    }
                }
            }
            catch (Exception ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\ExcelCSVInstallAccessDatabaseEngineLogError.txt", json);
            }



            //string accessEngineInstallerPath = string.Empty;
            //try
            //{
            //    if (flag)
            //    {
            //        // accessEngineInstallerPath = Common.CustomPath + @"\ExeFolder\AccessDatabaseEngine.exe";
            //        accessEngineInstallerPath = AppContext.BaseDirectory + @"\AccessDatabaseEngine.exe"
            //        ; // Specify the correct path to the installer
            //    }
            //    else
            //    {
            //        accessEngineInstallerPath = AppContext.BaseDirectory + @"\AccessDatabaseEngine.exe"; // Specify the correct path to the installer

            //    }
            //    ProcessStartInfo startInfo = new ProcessStartInfo
            //    {
            //        WindowStyle = ProcessWindowStyle.Hidden,
            //        FileName = accessEngineInstallerPath,
            //        Arguments = "/quiet", // Use /quiet to install silently
            //        Verb = "runas",
            //        UseShellExecute = true
            //    };

            //    using (Process process = new Process { StartInfo = startInfo })
            //    {
            //        process.Start();
            //        process.WaitForExit();

            //        int exitCode = process.ExitCode;
            //        if (exitCode == 0)
            //        {

            //        }
            //        else
            //        {

            //        }
            //    }
            //}
            //catch (Exception ex)
            //{

            //}


        }

        #endregion

        private void RemoveDeletedPluginFiles()
        {
            string RemovePlugin = string.Empty;
            string[] args = Environment.GetCommandLineArgs();
            foreach (string arg in args)
            {
                if (arg.StartsWith("RemovePlugin="))
                {
                    RemovePlugin = arg.Substring(13).Trim();
                    try
                    {
                        if (System.IO.Directory.Exists(Constant.PluginFileLocation + "\\" + RemovePlugin))
                            System.IO.Directory.Delete(Constant.PluginFileLocation + "\\" + RemovePlugin, true);
                    }
                    catch (Exception ex)
                    {
                        App.Log.Error(ex.Message, ex);
                    }
                }
            }
        }

        public void TakeDB_Backup()
        {
            try
            {
                Thread.Sleep(5000);

                try
                {
                    string Perent_Directory = Directory.GetParent(CameraConstant.CameraImageLocation).FullName;
                    string Temp_Directory = Perent_Directory + "\\Temp_" + Path.GetFileName(CameraConstant.CameraImageLocation.TrimEnd(Path.DirectorySeparatorChar));

                    DirectoryInfo dir = new DirectoryInfo(Temp_Directory);
                    DateTime CutofDate = DateTime.Now.AddDays(-2);

                    // Delete files not saved in DB
                    foreach (FileInfo file in dir.GetFiles())
                    {
                        if (file.LastWriteTime < CutofDate)
                        {
                            file.Delete();
                        }
                    }
                }
                catch { }


                while (true)
                {
                    try
                    {
                        using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                        {
                            List<ApplicationSetting> DatabaseSetting = ctx.ApplicationSettings.Where(p => p.ModuleName == "DatabaseSetting").ToList();

                            string AutoBackupEnable = DatabaseSetting.FirstOrDefault(x => x.SettingName == "AutoBackupEnable").SettingValue;
                            if (!string.IsNullOrEmpty(AutoBackupEnable) && AutoBackupEnable == "True")
                            {
                                string AutoBackupHour = DatabaseSetting.FirstOrDefault(x => x.SettingName == "AutoBackupHour").SettingValue;
                                string AutoBackupMinute = DatabaseSetting.FirstOrDefault(x => x.SettingName == "AutoBackupMinute").SettingValue;
                                string AutoBackupMeridiem = DatabaseSetting.FirstOrDefault(x => x.SettingName == "AutoBackupMeridiem").SettingValue;

                                // Create time string
                                string timeString = $"{AutoBackupHour}:{AutoBackupMinute} {AutoBackupMeridiem}";

                                // Convert to DateTime
                                DateTime backupTime = DateTime.ParseExact(timeString, "h:mm tt", CultureInfo.InvariantCulture);

                                TimeSpan currentTime = DateTime.Now.TimeOfDay;

                                // Run only after configured time
                                if (currentTime >= backupTime.TimeOfDay)
                                {
                                    string AutoBackupPath = DatabaseSetting.FirstOrDefault(x => x.SettingName == "AutoBackupFilePath").SettingValue;

                                    string AutoBackupFilePath = Path.Combine(AutoBackupPath, $"WB_{DateTime.Now:yyyy_MM_dd}.bak");

                                    if (!File.Exists(AutoBackupFilePath))
                                    {
                                        string Updatequery = $"BACKUP DATABASE [{Constant.DBNAME}] TO DISK = '{AutoBackupFilePath}' WITH FORMAT, INIT, STATS = 10;";

                                        DatabaseFunctions.ExecuteQuery(Updatequery);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                        File.WriteAllText(Path.Combine(Constant.CustomSettingsPath, $"DBBackupError_{DateTime.Now:yyyy_MM_dd}.txt"), json);
                    }
                    Thread.Sleep(30000); //30 Seconds
                }

            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }
        }

        public void ChechUnmanedServiceStatus()
        {

            try
            {
                try
                {
                    string EndelANPR_FilePath = System.AppDomain.CurrentDomain.BaseDirectory + "EndelAnpr\\EndelANPR.exe";
                    if (File.Exists(EndelANPR_FilePath))
                    {
                        var EndelANPR_Process = System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(EndelANPR_FilePath)).Count() > 0;
                        if (!EndelANPR_Process)
                        {
                            Process EndelAnpr = new Process();
                            EndelAnpr.StartInfo.FileName = EndelANPR_FilePath;
                            EndelAnpr.StartInfo.WorkingDirectory = Path.GetDirectoryName(EndelANPR_FilePath);
                            EndelAnpr.Start();
                        }
                    }
                }
                catch
                { }

                List<PluginManagement> lstPlugin = new List<PluginManagement>();
                if (File.Exists(Constant.PluginFileLocation + @"\PluginConfiguration.dat"))
                {
                    string Data = File.ReadAllText(Constant.PluginFileLocation + @"\PluginConfiguration.dat");
                    if (!string.IsNullOrEmpty(Data))
                    {
                        lstPlugin = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PluginManagement>>(Data);
                        if (lstPlugin != null && lstPlugin.Count > 0)
                        {
                            foreach (var item in lstPlugin)
                            {
                                if (item.IsEnable)
                                {
                                    DateTime dummyDate;
                                    if (LicenseHelper.LicenseInfo.AdditionalAttributes.Contains(item.PluginName))
                                    {
                                        string pluginExpiry = LicenseHelper.LicenseInfo.AdditionalAttributes.Get(item.PluginName);
                                        if (!string.IsNullOrEmpty(pluginExpiry) && DateTime.TryParseExact(pluginExpiry, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dummyDate)
                                            && dummyDate != null && dummyDate > DateTime.Now)
                                        {
                                            if (item.PluginName == "UnmannedPlugin")
                                            {
                                                try
                                                {
                                                    string ServiceName = "WeightReaderSVC";
                                                    System.ServiceProcess.ServiceController ctl = System.ServiceProcess.ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == ServiceName);
                                                    if (ctl != null && ctl.Status != System.ServiceProcess.ServiceControllerStatus.Running)
                                                    {
                                                        try
                                                        {
                                                            Process process1 = new Process();
                                                            ProcessStartInfo startInfo1 = new ProcessStartInfo();
                                                            startInfo1.WindowStyle = ProcessWindowStyle.Hidden;
                                                            startInfo1.FileName = "cmd.exe";
                                                            startInfo1.Arguments = "/C sc start " + ServiceName;
                                                            startInfo1.Verb = "runas";
                                                            process1.StartInfo = startInfo1;
                                                            process1.Start();
                                                            process1.WaitForExit();
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            _ = ex.Message;
                                                            ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync("Error", "Failed to start service WeightReaderSVC, please contact support team.");
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    _ = ex.Message;
                                                    ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync("Error", "Failed to start service WeightReaderSVC, please contact support team.");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
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
                    else if (statusDescription != "Weighing Solution Portal" && !statusDescription.ToUpper().Contains("<HTML"))
                    {
                        if (File.Exists(licensepath))
                        {
                            string licenseKey = File.ReadAllText(licensepath);
                            if (!licenseKey.Equals(statusDescription))
                            {
                                File.WriteAllText(licensepath, statusDescription);
                                Common.SetFullAccessForFile(licensepath);
                                Dispatcher.BeginInvoke(new Action(async () =>
                                {
                                    await MessageBoxHelper.ShowInfoDialogAsync("License Upgrade", "Your license information has been changed, application needs to restart.");
                                    App.RestartApplication();
                                }));
                            }
                        }
                        else
                        {
                            File.WriteAllText(licensepath, statusDescription);
                            Common.SetFullAccessForFile(licensepath);
                            Dispatcher.BeginInvoke(new Action(async () =>
                            {
                                await MessageBoxHelper.ShowInfoDialogAsync("License Upgrade", "Your license information has been changed, application needs to restart.");
                                App.RestartApplication();
                            }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.WriteAllText(Constant.CustomSettingsPath + @"\LicenseError.txt", json);
            }
        }

        private void LicenseCheckThreadStart()
        {
            App.CheckOnlineAnprLicense();

            CheckOnlineLicense();

            string licensepath = Constant.CustomSettingsPath + @"\license.lic";

            string LicenseCheckResult = LicenseHelper.ValidateLicense().Result;
            if (LicenseCheckResult.Contains("ERROR: Unable to read license file."))
            {
                Dispatcher.BeginInvoke(new Action(async () =>
                {
                    await MessageBoxHelper.ShowInfoDialogAsync("License Error", LicenseCheckResult + "\n" + "Kindly close file if open.");
                    this.Close();
                }));
            }
            else if (LicenseCheckResult != "Success")
            {
                //File.Delete(licensepath);
                Dispatcher.BeginInvoke(new Action(async () =>
                {
                    await MessageBoxHelper.ShowInfoDialogAsync("License Error", LicenseCheckResult + "\n" + "Kindly update license information");
                    Licensing mw = new Licensing();
                    mw.Show();
                    this.Close();
                }));
            }
            else
            {
                bool anyPluginGoingToExpiry = false;
                double DaysUntilExpiry = LicenseHelper.LicenseInfo.Expiration.Subtract(DateTime.Now).TotalDays;
                string coreExpiryString = "";
                if (DaysUntilExpiry <= 15)
                {
                    coreExpiryString = "Core license is about to expire in " + Convert.ToInt32(DaysUntilExpiry).ToString() + " days.\n";
                }
                string pluginExpiryString = "Following plugin is about to expire\n";
                Portable.Licensing.LicenseAttributes lstPlugins = LicenseHelper.LicenseInfo.AdditionalAttributes;
                foreach (KeyValuePair<string, string> item in lstPlugins.GetAll())
                {
                    DateTime expiry;
                    if (!string.IsNullOrEmpty(item.Value) && DateTime.TryParseExact(item.Value, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out expiry) && expiry > DateTime.Now && expiry.Subtract(DateTime.Now).TotalDays < 15)
                    {
                        anyPluginGoingToExpiry = true;
                        pluginExpiryString = pluginExpiryString + item.Key + "is about to expire in " + Convert.ToInt32(expiry.Subtract(DateTime.Now).TotalDays).ToString() + " days.\n";
                    }
                }
                string renewalString = "";
                if (!string.IsNullOrEmpty(coreExpiryString))
                    renewalString = coreExpiryString;
                if (anyPluginGoingToExpiry)
                    renewalString = renewalString + pluginExpiryString;

                if (!string.IsNullOrEmpty(renewalString))
                {
                    Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        await MessageBoxHelper.ShowInfoDialogAsync("License Renewal Alert", renewalString + "Kindly renew license for continuous usage.");
                    }));
                }

                List<SharedComponents.PluginManagement> lstPlugin = new List<SharedComponents.PluginManagement>();
                if (File.Exists(Constant.PluginFileLocation + @"\PluginConfiguration.dat"))
                {
                    string Data = File.ReadAllText(Constant.PluginFileLocation + @"\PluginConfiguration.dat");
                    if (!string.IsNullOrEmpty(Data))
                    {
                        lstPlugin = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SharedComponents.PluginManagement>>(Data);
                    }
                }
                lstPlugin = lstPlugin.Where(x => x.IsEnable == true).ToList();
                PluginLoader<PluginInstallerBase> pls = new PluginLoader<PluginInstallerBase>();
                foreach (SharedComponents.PluginManagement item in lstPlugin)
                {
                    if (LicenseHelper.LicenseInfo.AdditionalAttributes.Contains(item.PluginName))
                    {
                        DateTime dummyDate;
                        string pluginExpiry = LicenseHelper.LicenseInfo.AdditionalAttributes.Get(item.PluginName);
                        if (!string.IsNullOrEmpty(pluginExpiry) && DateTime.TryParseExact(pluginExpiry, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dummyDate)
                            && dummyDate != null && dummyDate > DateTime.Now)
                        {

                        }
                        else
                        {
                            KeyValuePair<PluginInstallerBase, IPluginMetadata> plg = pls.loadedPlugins.FirstOrDefault(c => c.Value.PluginName == item.PluginName);
                            if (plg.Key != null)
                            {
                                try
                                {
                                    plg.Key.DisablePlugin();
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }
                    else
                    {
                        KeyValuePair<PluginInstallerBase, IPluginMetadata> plg = pls.loadedPlugins.FirstOrDefault(c => c.Value.PluginName == item.PluginName);
                        if (plg.Key != null)
                        {
                            try
                            {
                                plg.Key.DisablePlugin();
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }

            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                timerUnman.Stop();
                foreach (var plugin in mef.loadedPlugins.Keys)
                {
                    plugin.DisposePlugin();
                }

                var current = System.Diagnostics.Process.GetCurrentProcess();
                System.Diagnostics.Process.GetProcessesByName(current.ProcessName).ToList().ForEach(t => t.Kill());
            }
            catch (Exception Ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(Ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\MainPageError.txt", json);
                //exec sp_updatestats
            }

        }
        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _isRunning = false;

                weighCommand = true;
                timerUnman.Stop();
                foreach (var plugin in mef.loadedPlugins.Keys)
                {
                    plugin.DisposePlugin();
                }

                var current = System.Diagnostics.Process.GetCurrentProcess();
                System.Diagnostics.Process.GetProcessesByName(current.ProcessName).ToList().ForEach(t => t.Kill());
            }
            catch (Exception Ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(Ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\MainPageError.txt", json);
                //exec sp_updatestats
            }
        }
        private void btnDataSetup_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new View.DataSetup_Page());
        }
        private void btnTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (ctxGlobal.WeightBridges.FirstOrDefault() == null)
                this.ShowMessageAsync("Warning", "Unable to create transaction, master data was not created. Please set master detail. ");
            else
                MainFrame.Navigate(new View.TransactionManagementList_Page());
        }
        private void btnReports_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new View.Reports_Page());
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new View.Setting_Page());
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            weighCommand = true;
            Application.Current.Properties["LoggedInUserID"] = null;
            Application.Current.Properties["LoggedInUserName"] = null;
            Constant.lstPermission = null;

            var current = System.Diagnostics.Process.GetCurrentProcess();
            System.Diagnostics.Process.GetProcessesByName(current.ProcessName).ToList().ForEach(t => t.Kill());
        }

        private void EditCompanySetting_Click(object sender, RoutedEventArgs e)
        {
            if (Constant.IsAdmin || Constant.IsSupportUser)
                this.MainFrame.Content = new View.CompanySetting_Page();
            else
                this.MainFrame.Content = new AccessDenied();
        }
        private void btnCloseEditComapnySetting_Click(object sender, RoutedEventArgs e)
        {
            //stkEditComapnysetting.Visibility = Visibility.Collapsed;
        }

        private void hyperlinkLoggeruser_Click(object sender, RoutedEventArgs e)
        {
            flyChangePassword.IsOpen = true;
            pwbCurrentPassword.Password = "";
            pwbNewPassword.Password = "";
            pwbConfirmPassword.Password = "";
        }

        private void btnChangePasswordCancel_Click(object sender, RoutedEventArgs e)
        {
            flyChangePassword.IsOpen = false;
        }

        private void btnCreateTransaction_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.weighCommand = false;
            if (Common.ValidatePageRequest(PageEntity.Transaction, PageAccess.Create))
            {
                Uri uri = new Uri("/View/TransactionView/TransactionManagement.Xaml", UriKind.Relative);
                MainFrame.NavigationService.Navigate(uri);
            }
        }

        private void btnEditTransaction_Click(object sender, RoutedEventArgs e)
        {
            //TransactionData TransactionInfo = acmbTransactionID.SelectedItem as TransactionData;
            TransactionDataMainpage TransactionInfo = acmbTransactionID.SelectedItem as TransactionDataMainpage;
            if (TransactionInfo != null)
                MainFrame.NavigationService.Navigate(new WeighingSolution.View.TransactionManagement(TransactionInfo.ReceiptTicketID));
            else
                ToastHelper.ShowError("Select a valid active transaction first");
        }
        private void btnDashbord_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new View.DashBoard());
        }
        private void btnMonitor_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new View.Monitoring());
        }
        private void MainFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            Mouse.SetCursor(System.Windows.Input.Cursors.Wait);
            if (e.NavigationMode == NavigationMode.Back || e.NavigationMode == NavigationMode.Refresh)
            {
                e.Cancel = true;
            }
        }
        private void MainFrame_ContentRendered(object sender, EventArgs e)
        {
            Mouse.SetCursor(System.Windows.Input.Cursors.Arrow);
        }

        private void acmbTransactionID_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //TransactionData TransactionInfo = acmbTransactionID.SelectedItem as TransactionData;
                TransactionDataMainpage TransactionInfo = acmbTransactionID.SelectedItem as TransactionDataMainpage;
                if (TransactionInfo != null)
                    MainFrame.NavigationService.Navigate(new WeighingSolution.View.TransactionManagement(TransactionInfo.ReceiptTicketID));
            }
        }

        private void BtnCreateSupportTicket_Click(object sender, RoutedEventArgs e)
        {

            //Check weighbridge warrenty if in warrenty than
            double screenLeft = SystemParameters.VirtualScreenLeft;
            double screenTop = SystemParameters.VirtualScreenTop;
            double screenWidth = SystemParameters.VirtualScreenWidth;
            double screenHeight = SystemParameters.VirtualScreenHeight;

            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\WeighBridgeSolution\\ScreenShot\\"))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\WeighBridgeSolution\\ScreenShot\\");
            }
            string filename = "ScreenCapture-" + DateTime.Now.ToString("ddMMyyyy-hhmmss") + ".png";
            using (Bitmap bmp = new Bitmap((int)screenWidth, (int)screenHeight))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen((int)screenLeft, (int)screenTop, 0, 0, bmp.Size);
                    using (MemoryStream memory = new MemoryStream())
                    {
                        using (FileStream fs = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\WeighBridgeSolution\\ScreenShot\\" + filename, FileMode.Create, FileAccess.ReadWrite))
                        {
                            bmp.Save(memory, ImageFormat.Jpeg);
                            byte[] bytes = memory.ToArray();
                            fs.Write(bytes, 0, bytes.Length);
                        }
                    }

                }
            }

            SupportTicketCreator childWindow = new SupportTicketCreator(filename);
            ChildWindowManager.ShowChildWindowAsync(Application.Current.MainWindow, childWindow);
        }
        private void btnCreatepdfDownload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialogPDF = new SaveFileDialog();
                saveFileDialogPDF.Filter = "PDF|*.pdf";
                saveFileDialogPDF.ShowDialog();

                if (saveFileDialogPDF.FileName != "")
                {
                    string sourceFileName = System.AppDomain.CurrentDomain.BaseDirectory + "\\Report\\Receipt\\manual.pdf";
                    File.Copy(sourceFileName, saveFileDialogPDF.FileName);
                }
            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }
        }

        public async Task LiveWeightFile()
        {
            while (true)
            {
                Thread.Sleep(1000); // Sleep for 1 second
                try
                {
                    string txtFolderpath = Path.Combine(Constant.CustomSettingsPath, "LiveWeight.txt");
                    File.WriteAllText(txtFolderpath, string.Format("{0:G29}", Constant.IndicatorWeight));
                }
                catch (Exception ex)
                {
                    // Optional: log or handle the error
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }


        public async Task LiveWeightAsync()
        {
            string Config = string.Empty;
            if (LiveWeightSetting != null && LiveWeightSetting.Count > 0)
            {
                string ConfigURL = LiveWeightSetting.Where(p => p.SettingName == "ApiUrl").FirstOrDefault().SettingValue;
                string WBID = LiveWeightSetting.Where(p => p.SettingName == "WBID").FirstOrDefault().SettingValue;
                string PassKey = LiveWeightSetting.Where(p => p.SettingName == "PassKey").FirstOrDefault().SettingValue;

                if (string.IsNullOrEmpty(Config) && !string.IsNullOrEmpty(ConfigURL))
                    Config = await GetConfigAsync(ConfigURL, PassKey);

                if (!string.IsNullOrEmpty(Config) && !string.IsNullOrEmpty(WBID) && !string.IsNullOrEmpty(PassKey))
                {
                    while (true)
                    {
                        Thread.Sleep(1000);
                        try
                        {
                            if (ActiveWeighBridge != null && ActiveWeighBridge.WeightBridgeID > 0)
                            {
                                string query = $"update liveweight  set Weight='{Constant.IndicatorWeight}',UpdateAt='{DateTime.Now:yyyy-MM-dd HH:mm:ss}' " +
                                     $" where WeighbridgeId='{WBID}'  and Tenant in (select ID from tenant where Passkey='{PassKey}')";

                                bool fun = DatabaseFunctions.ExecuteMySqlQuery(Common.DecryptData(Config), query);

                            }
                        }
                        catch (Exception ex)
                        {
                            string json1 = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                            File.AppendAllText(Constant.ApplicationDataLocation + @"\MainLiveWTError.txt", json1);
                        }

                    }
                }
            }

        }

        public async Task<string> GetConfigAsync(string APIURL, string Passkey)
        {
            try
            {
                if (!string.IsNullOrEmpty(APIURL) && !string.IsNullOrEmpty(Passkey))
                {
                    APIURL = APIURL.TrimEnd('/');

                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, $"{APIURL}/api/Config/{Passkey}");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    string Response = await response.Content.ReadAsStringAsync();

                    Config_Response config_Response = JsonConvert.DeserializeObject<Config_Response>(Response);

                    if (config_Response != null && !string.IsNullOrEmpty(config_Response.data))
                    {
                        return config_Response.data;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }

        }

        #region Endel ANPR

        private volatile bool _isRunning = true;
        public void ProcessAnprThread()
        {
            try
            {
                List<Camera> cameras = GetAnprCameras(ActiveWeighBridge.WeightBridgeID);
                Camera LeftAnpr = null;
                if (cameras.Where(p => p.WBSide == "LEFT").Count() > 0)
                {
                    LeftAnpr = cameras.Where(p => p.WBSide == "LEFT").FirstOrDefault();
                }
                Camera RightAnpr = null;
                if (cameras.Where(p => p.WBSide == "RIGHT").Count() > 0)
                {
                    RightAnpr = cameras.Where(p => p.WBSide == "RIGHT").FirstOrDefault();
                }

                while (_isRunning)
                {
                    if (Constant.HasAnprLicense)
                    {
                        if (LeftAnpr != null)
                        {
                            Anpr_Number_Request LeftAnpr_Number_Request = new Anpr_Number_Request
                            {
                                CameraId = LeftAnpr.CameraID,
                                Rtsp_url = LeftAnpr.VideosourceMainURL,
                                UserName = LeftAnpr.UserName,
                                Password = LeftAnpr.CameraPassword,
                                InputImagePath = null
                            };
                            Anpr_Number_Response result = GetVehicleNumberAsync(LeftAnpr_Number_Request);

                            if (result != null)
                            {
                                string VehicleNumber = result.Success ? result.VehicleNumber : "";
                                viewModel.Anpr1Number = VehicleNumber;

                                if (!string.IsNullOrEmpty(VehicleNumber))
                                    viewModel.LastAnpr1Number = VehicleNumber;
                            }
                        }

                        if (RightAnpr != null)
                        {
                            Anpr_Number_Request RightAnpr_Number_Request = new Anpr_Number_Request
                            {
                                CameraId = RightAnpr.CameraID,
                                Rtsp_url = RightAnpr.VideosourceMainURL,
                                UserName = RightAnpr.UserName,
                                Password = RightAnpr.CameraPassword,
                                InputImagePath = null
                            };
                            Anpr_Number_Response result = GetVehicleNumberAsync(RightAnpr_Number_Request);

                            if (result != null)
                            {
                                string VehicleNumber = result.Success ? result.VehicleNumber : "";
                                viewModel.Anpr2Number = VehicleNumber;

                                if (!string.IsNullOrEmpty(VehicleNumber))
                                    viewModel.LastAnpr2Number = VehicleNumber;
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(30000);
                        App.CheckOnlineAnprLicense();
                    }

                    Thread.Sleep(2000);
                }
            }
            catch { }


        }

        public static Anpr_Number_Response GetVehicleNumberAsync(Anpr_Number_Request request)
        {
            try
            {
                using (var client = new NamedPipeClientStream(".", "ANPR_PIPE", PipeDirection.InOut))
                {
                    client.Connect(2000);

                    using (var writer = new BinaryWriter(client, Encoding.UTF8, leaveOpen: true))
                    {
                        using (var reader = new BinaryReader(client, Encoding.UTF8, leaveOpen: true))
                        {

                            string json = JsonConvert.SerializeObject(request);
                            byte[] bytes = Encoding.UTF8.GetBytes(json);

                            writer.Write(bytes.Length);
                            writer.Write(bytes);
                            writer.Flush();

                            int respLen = reader.ReadInt32();
                            string respJson = Encoding.UTF8.GetString(reader.ReadBytes(respLen));

                            return JsonConvert.DeserializeObject<Anpr_Number_Response>(respJson);
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public List<Camera> GetAnprCameras(int WeightBridgeID)
        {
            try
            {
                using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                {
                    return ctx.Cameras.Where(p => p.WeightBridgeID == WeightBridgeID && p.Active).ToList();
                }
            }
            catch
            {
                return null;
            }

        }

        public static void Save_In_DB(int CameraID, string AnprNumber, string ActualNumber, string ImageName, int MatchPer, string MatchResult, string CameraName, string WBName)
        {
            try
            {
                string Query = $"INSERT INTO [dbo].[Endel_ANPR] " +
                               $"([AnprNumber],[ActualNumber],[ImageName],[CameraID],[CaptureTime],[MatchPer],[MatchResult],[CameraName],[WBName])" +
                               $" VALUES " +
                               $"('{AnprNumber}','{ActualNumber}','{ImageName}',{CameraID},GETDATE(),{MatchPer},'{MatchResult}','{CameraName}','{WBName}')";
                DataLayer.DatabaseFunctions.ExecuteQuery(Query);
            }
            catch { }

        }

        public static void SaveBase64Image(string base64String, string filePath)
        {
            try
            {
                // Remove Base64 header if present
                if (base64String.Contains(","))
                    base64String = base64String.Substring(base64String.IndexOf(",") + 1);

                byte[] imageBytes = Convert.FromBase64String(base64String);
                File.WriteAllBytes(filePath, imageBytes);
            }
            catch { }

        }

        public static MatchInfo GetMatchResult(string anprNumber, string actualNumber)
        {
            if (string.IsNullOrWhiteSpace(anprNumber) || string.IsNullOrWhiteSpace(actualNumber))
            {
                return new MatchInfo { MatchPer = 0, MatchResult = "Not Matched" };
            }

            string a = anprNumber.Replace(" ", "").ToUpper();
            string b = actualNumber.Replace(" ", "").ToUpper();

            int maxLen = Math.Max(a.Length, b.Length);
            int distance = LevenshteinDistance(a, b);

            int matchPer = (int)Math.Round((1.0 - (double)distance / maxLen) * 100);

            string matchResult = matchPer == 100 ? "Matched" : matchPer >= 85 ? "Likely Matched" : "Not Matched";

            return new MatchInfo
            {
                MatchPer = matchPer,
                MatchResult = matchResult
            };
        }
        public static int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;

            int[,] d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; i++) d[i, 0] = i;
            for (int j = 0; j <= m; j++) d[0, j] = j;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = s[i - 1] == t[j - 1] ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1,      // delete
                                 d[i, j - 1] + 1),     // insert
                        d[i - 1, j - 1] + cost);       // replace
                }
            }

            return d[n, m];
        }

        public static string getAnprImagePath()
        {
            try
            {
                using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                {
                    ApplicationSetting CameraSetting = ctx.ApplicationSettings.FirstOrDefault(x => x.ModuleName == "AnprSetting" && x.SettingName == "AnprImagePath");
                    if (CameraSetting != null && CameraSetting.SettingValue != "")
                    {
                        Common.CreateDirectoryWithFullAccess(CameraSetting.SettingValue);

                        return CameraSetting.SettingValue;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            catch
            {
                return string.Empty;
            }

        }

        public void SendAnprReport()
        {
            Thread.Sleep(10000);
            while (true)
            {
                Thread.Sleep(30000);
                try
                {
                    if (CanSendReport())
                    {
                        Send_Anpr_Report();
                        SaveLastSentDate(DateTime.Now);
                    }
                }
                catch (Exception ex)
                {
                    string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                    File.WriteAllText(Constant.PluginFileLocation + $"\\SendAnprReport_Error_{DateTime.Now:yyyy_MM_dd_HH}.txt", json);
                }
            }
        }

        private void SaveLastSentDate(DateTime date)
        {
            try
            {
                string ReportStateFile = Path.Combine(Constant.CustomSettingsPath, "CSV_report.txt");
                File.WriteAllText(ReportStateFile, date.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch
            {
                // ignore write failure
            }
        }

        private bool CanSendReport()
        {
            try
            {
                string ReportStateFile = Path.Combine(Constant.CustomSettingsPath, "CSV_report.txt");

                if (!File.Exists(ReportStateFile))
                    return true; // never sent before

                string content = File.ReadAllText(ReportStateFile).Trim();

                if (!DateTime.TryParse(content, out DateTime lastSent))
                    return true; // corrupted file → resend

                return (DateTime.Now - lastSent).TotalDays >= 1;
            }
            catch
            {
                return true; // fail-safe: allow send
            }
        }

        private void Send_Anpr_Report()
        {
            try
            {
                using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                {
                    ObservableCollection<ApplicationSetting> EmailSetting = new ObservableCollection<ApplicationSetting>(ctx.ApplicationSettings.Where(x => x.ModuleName == "EndelEmailSetting").ToList());
                    ObservableCollection<ApplicationSetting> CompanySetting = new ObservableCollection<ApplicationSetting>(ctx.ApplicationSettings.Where(x => x.ModuleName == "CompanySetting").ToList());

                    string ServerName = EmailSetting.FirstOrDefault(x => x.SettingName == "EndelServerName").SettingValue;
                    string UserName = EmailSetting.FirstOrDefault(x => x.SettingName == "EndelUserName").SettingValue;
                    string PassWord = Common.DecryptData(EmailSetting.FirstOrDefault(x => x.SettingName == "EndelPassWord").SettingValue);
                    string EmailFrom = EmailSetting.FirstOrDefault(x => x.SettingName == "EndelEmailFrom").SettingValue;
                    string Port = EmailSetting.FirstOrDefault(x => x.SettingName == "EndelPort").SettingValue;
                    bool UseSSL = EmailSetting.FirstOrDefault(x => x.SettingName == "EndelUseSSL").SettingValue != null ? Convert.ToBoolean(EmailSetting.FirstOrDefault(x => x.SettingName == "EndelUseSSL").SettingValue) : true;
                    string EmailTo = EmailSetting.FirstOrDefault(x => x.SettingName == "EndelEmailTo").SettingValue;

                    string CompanyName = CompanySetting.FirstOrDefault(x => x.SettingName == "CompanyName").SettingValue;

                    DateTime StartDate = DateTime.Now.AddDays(-7);
                    DateTime EndDate = DateTime.Now;

                    string TicketSubject = $"ANPR Weekly Report | {CompanyName} | {StartDate:dd-MMM-yyyy} to {EndDate:dd-MMM-yyyy}";
                    string EmailBody = "";

                    CSVResult cSVResult = GetAnprCsv(StartDate, EndDate);
                    if (cSVResult != null)
                    {
                        string CSVString = cSVResult.CSVString;
                        string CSV_FileName = $"CSV_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.csv";
                        string CSV_Path = Path.Combine(Constant.ANPRCSVLocation, CSV_FileName);

                        if (!string.IsNullOrEmpty(CSVString))
                        {
                            File.WriteAllText(CSV_Path, CSVString);
                        }


                        EmailBody += $"Hello,<BR> Please find attached the ANPR report.<BR>" +
                           $"Period : {StartDate:dd-MMM-yyyy} to {EndDate:dd-MMM-yyyy}<BR>" +
                           $"Total Records: {cSVResult.RecordCount}<BR><BR>";

                        //if (File.Exists(CSV_Path))
                        //{
                        //    EmailBody += ConvertCsvToHtml(CSV_Path);
                        //}

                        EmailBody += $"<BR>Regards,<BR>" +
                           $"ANPR System";


                        Email objEmail = new Email();
                        objEmail.SMTPServer = ServerName;
                        if (string.IsNullOrEmpty(UserName))
                            objEmail.SMTPUserName = "";
                        else
                            objEmail.SMTPUserName = UserName;
                        if (string.IsNullOrEmpty(PassWord))
                            objEmail.SMTPPassword = "";
                        else
                            objEmail.SMTPPassword = PassWord;
                        if (string.IsNullOrEmpty(EmailFrom))
                            objEmail.EmailFrom = "parimal.makwana@endel.digital";
                        else
                            objEmail.EmailFrom = EmailFrom;
                        objEmail.Subject = TicketSubject;
                        objEmail.EmailAddress = EmailTo;
                        objEmail.Body = EmailBody;

                        objEmail.Attachments = new Dictionary<int, string>();
                        if (File.Exists(CSV_Path))
                        {
                            objEmail.Attachments.Add(0, CSV_Path);
                        }

                        objEmail.SendHtmlEmail(Convert.ToInt32(Port), UseSSL);
                    }
                }

            }
            catch (Exception ex)
            {
                Common.logErrorInDB(ex.Message, ex);
            }
        }

        public CSVResult GetAnprCsv(DateTime StartDate, DateTime EndDate)
        {
            try
            {
                string query = $"SELECT [AnprNumber],[ActualNumber],[ImageName],[MatchPer],[MatchResult],[CameraName]," +
                               $"[WBName],FORMAT([CaptureTime], 'yyyy/MM/yyyy HH:mm:ss') AS CaptureTime " +
                               $"FROM [Endel_ANPR] " +
                               $" Where CaptureTime Between '{StartDate:yyyy-MM-dd HH:mm:ss}' and '{EndDate:yyyy-MM-dd HH:mm:ss}' ";

                DataTable dt = DatabaseFunctions.SQLExecuteReaderQuery(query);

                if (dt != null && dt.Rows.Count > 0)
                {
                    // 🔹 Convert DataTable to CSV (IN MEMORY)
                    CSVResult csvData = ConvertToCsv(dt);

                    return csvData;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }

        }

        private static CSVResult ConvertToCsv(DataTable table)
        {
            try
            {
                if (table == null)
                {
                    return null;
                }

                StringBuilder sb = new StringBuilder();

                sb.Append("SR,");

                // Header
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    sb.Append(table.Columns[i].ColumnName);
                    if (i < table.Columns.Count - 1)
                        sb.Append(",");
                }
                sb.AppendLine();

                int NO = 1;
                // Rows
                foreach (DataRow row in table.Rows)
                {
                    sb.Append($"{NO},");
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        string value = row[i]?.ToString() ?? "";
                        if (value.Contains(",") || value.Contains("\""))
                            value = "\"" + value.Replace("\"", "\"\"") + "\"";

                        sb.Append(value);

                        if (i < table.Columns.Count - 1)
                            sb.Append(",");
                    }
                    NO++;
                    sb.AppendLine();
                }
                return new CSVResult { RecordCount = table.Rows.Count, CSVString = sb.ToString() };
            }
            catch
            {
                return null;
            }
        }

        public static string ConvertCsvToHtml(string csvFilePath)
        {
            if (!File.Exists(csvFilePath))
                return "<p>CSV file not found</p>";

            string[] lines = File.ReadAllLines(csvFilePath);

            if (lines.Length == 0)
                return "<p>No data available</p>";

            var html = new StringBuilder();

            html.AppendLine("<table border='1' cellpadding='5' cellspacing='0' style='border-collapse:collapse;'>");

            // HEADER
            var headers = lines[0].Split(',');
            html.AppendLine("<thead><tr>");
            foreach (var header in headers)
            {
                html.AppendLine($"<th>{WebUtility.HtmlEncode(header)}</th>");
            }
            html.AppendLine("</tr></thead>");

            // BODY
            html.AppendLine("<tbody>");
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                var cells = lines[i].Split(',');
                html.AppendLine("<tr>");

                foreach (var cell in cells)
                {
                    html.AppendLine($"<td>{WebUtility.HtmlEncode(cell)}</td>");
                }

                html.AppendLine("</tr>");
            }
            html.AppendLine("</tbody></table>");


            return html.ToString();
        }


        #endregion Endel ANPR
        #region uhf

        public bool IsPing(string host, int TimeOut)
        {
            bool result = false;
            try
            {
                Ping p = new Ping();
                PingReply reply = p.Send(host, TimeOut);
                if (reply.Status == IPStatus.Success)
                    return true;
            }
            catch { }
            return result;
        }


        public List<Token> TokenLists = new List<Token>();
        public List<UHFDataClass> UHFDataClasses = new List<UHFDataClass>();
        Thread ThreadProcessUHF;
        public static bool IsNumberListen = true;

        Thread ThreadUHFLeft;
        Thread ThreadUHFRight;

        UhfTcpListener uhfReaderLeft;
        UhfTcpListener uhfReaderRight;

        public bool ProcessClientData_Executing = false;

        public void UpdateOldToken()
        {
            try
            {
                using (weighbridgeEntities ctToken = new weighbridgeEntities(Constant.EntityConnectionString))
                {
                    List<string> prefixes = ctToken.UhfMasters.Select(x => x.StringPrifix).Where(x => !string.IsNullOrEmpty(x)).ToList();

                    List<Token> tokens = ctToken.Tokens.ToList();
                    foreach (Token token in tokens)
                    {
                        if (string.IsNullOrEmpty(token.TokenValue))
                            continue;

                        string newTokenValue = token.TokenValue;

                        foreach (string prefix in prefixes)
                        {
                            newTokenValue = newTokenValue.Replace(prefix, "");
                        }

                        if (newTokenValue.Length == 23)
                        {
                            newTokenValue += "0";
                        }

                        if (newTokenValue != token.TokenValue)
                        {
                            string updateQuery = $"UPDATE Token SET TokenValue = '{newTokenValue}' WHERE TokenID = {token.TokenID}";
                            DatabaseFunctions.ExecuteQuery(updateQuery);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }

        }

        private void StartUHFLeft(object obj)
        {
            UhfMaster uhfMasterLeft = (UhfMaster)obj;
            uhfReaderLeft = new UhfTcpListener(uhfMasterLeft.IpAddress, Convert.ToInt32(uhfMasterLeft.Port), uhfMasterLeft.UHFType);
            string prifix = !string.IsNullOrEmpty(uhfMasterLeft.StringPrifix) ? uhfMasterLeft.StringPrifix : "1100EE00";

            uhfReaderLeft.OnConnectionChanged += (connected) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    viewModel.UHF1Stutas = connected ? "UHF1 Connected" : "UHF1 Disconnected";
                });
            };

            uhfReaderLeft.OnTagReceived += (data) =>
            {
                ProcessUHFLeftRFID(prifix, data);
            };

            uhfReaderLeft.Start();
        }

        public void ProcessUHFLeftRFID(string prifix, string data)
        {
            string temp = data.Trim();


            if (!string.IsNullOrEmpty(temp))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        temp = temp.Replace(prifix, "-");
                        //temp = temp.TrimEnd('0');
                        string Pos1 = temp.Substring(temp.IndexOf("-"));

                        string[] words = Pos1.Split('-');

                        List<string> uniqueWords = words.Distinct().ToList();

                        string temp_RFID_Tag = string.Empty;
                        try
                        {
                            //string val1 = outputString.Substring(0, outputString.Length / 2);
                            //string val2 = outputString.Substring(outputString.Length / 2, outputString.Length / 2);

                            if (uniqueWords.Where(x => x.Length > 10).Count() > 1)
                            {
                                string val1 = uniqueWords[0];
                                string val2 = uniqueWords[1];
                                if (val1 == val2)
                                {
                                    temp_RFID_Tag = val1;
                                }
                                else
                                {
                                    //temp_RFID_Tag = val1 + val2;

                                    if (TokenLists.Where(p => p.TokenValue == val1).Count() > 0)
                                    {
                                        temp_RFID_Tag = val1.Substring(0, 24);
                                    }
                                    else if (TokenLists.Where(p => p.TokenValue == val2).Count() > 0)
                                    {
                                        temp_RFID_Tag = val2.Substring(0, 24); ;
                                    }
                                    else
                                    {
                                        temp_RFID_Tag = val1 + " " + val2;
                                    }
                                }
                            }
                            else
                            {
                                foreach (var item in uniqueWords)
                                {
                                    if (!string.IsNullOrEmpty(item) && item.Length > 10)
                                    {
                                        temp_RFID_Tag = item;
                                        break;
                                    }
                                }
                            }
                        }
                        catch
                        {
                            temp_RFID_Tag = "";
                        }

                        string RFID_Tag = !string.IsNullOrEmpty(temp_RFID_Tag) ? temp_RFID_Tag.Trim() : "";
                        Constant.DisplayRFID_Left = string.Empty;
                        if (!string.IsNullOrEmpty(RFID_Tag) && RFID_Tag.Length >= 24)
                        {
                            Constant.RFID = RFID_Tag;
                            Constant.UHFLEFT = RFID_Tag;
                            Constant.WBSIDE_DASHBORD = "Left";
                            try
                            {
                                UHFDataClasses.RemoveAll(p => p.ReadTime < DateTime.Now.AddSeconds(-20));

                                if (TokenLists.Where(p => p.TokenValue == RFID_Tag).Count() > 0)
                                {
                                    Constant.DisplayRFID_Left = TokenLists.Where(p => p.TokenValue == RFID_Tag).First().TokenName + ":" + RFID_Tag;
                                    int tagCount = UHFDataClasses.Where(p => p.Tag == RFID_Tag).Count();
                                    UHFDataClasses.Add(new UHFDataClass { Tag = RFID_Tag, Count = tagCount, UHFSide = "Left", ReadTime = DateTime.Now });
                                }
                                else
                                {
                                    Constant.DisplayRFID_Left = RFID_Tag + "Token Not Found";
                                    try
                                    {
                                        using (weighbridgeEntities ctToken = new weighbridgeEntities(Constant.EntityConnectionString))
                                        {
                                            if (ctToken.Tokens.Where(p => p.TokenValue == RFID_Tag).Count() > 0)
                                            {
                                                Token token = ctToken.Tokens.Where(p => p.TokenValue == RFID_Tag).FirstOrDefault();
                                                TokenLists.Add(token);
                                            }
                                        }
                                    }
                                    catch { }

                                }

                                viewModel.RFIDText = RFID_Tag;
                            }
                            catch { }
                        }
                    }
                    catch { }
                });
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    viewModel.RFIDText = "";
                    Constant.RFID = "";
                    Constant.DisplayRFID_Left = "";
                    Constant.DisplayRFID_Right = "";
                    UHFDataClasses.RemoveAll(p => p.ReadTime < DateTime.Now.AddSeconds(-1));
                });
            }
        }

        private void StartUHFRight(object obj)
        {
            UhfMaster uhfMasterRight = (UhfMaster)obj;

            uhfReaderRight = new UhfTcpListener(uhfMasterRight.IpAddress, Convert.ToInt32(uhfMasterRight.Port), uhfMasterRight.UHFType);
            string prifix = !string.IsNullOrEmpty(uhfMasterRight.StringPrifix) ? uhfMasterRight.StringPrifix : "1100EE00";

            uhfReaderRight.OnConnectionChanged += (connected) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    viewModel.UHF2Stutas = connected ? "UHF2 Connected" : "UHF2 Disconnected";
                });
            };

            uhfReaderRight.OnTagReceived += (data) =>
            {
                ProcessUHFRightRFID(prifix, data);
            };

            uhfReaderRight.Start();
        }

        public void ProcessUHFRightRFID(string prifix, string data)
        {
            string temp = data.Trim();

            if (!string.IsNullOrEmpty(temp))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        temp = temp.Replace(prifix, "-");
                        //temp = temp.TrimEnd('0');
                        string Pos1 = temp.Substring(temp.IndexOf("-"));

                        string[] words = Pos1.Split('-');
                        List<string> uniqueWords = words.Distinct().ToList();
                        string temp_RFID_Tag = string.Empty;
                        try
                        {
                            //string val1 = outputString.Substring(0, outputString.Length / 2);
                            //string val2 = outputString.Substring(outputString.Length / 2, outputString.Length / 2);

                            if (uniqueWords.Where(x => x.Length > 10).Count() > 1)
                            {
                                string val1 = uniqueWords[0];
                                string val2 = uniqueWords[1];
                                if (val1 == val2)
                                {
                                    temp_RFID_Tag = val1;
                                }
                                else
                                {
                                    //temp_RFID_Tag = val1 + val2;

                                    if (TokenLists.Where(p => p.TokenValue == val1).Count() > 0)
                                    {
                                        temp_RFID_Tag = val1.Substring(0, 24);
                                    }
                                    else if (TokenLists.Where(p => p.TokenValue == val2).Count() > 0)
                                    {
                                        temp_RFID_Tag = val2.Substring(0, 24); ;
                                    }
                                    else
                                    {
                                        temp_RFID_Tag = val1 + " " + val2;
                                    }
                                }
                            }
                            else
                            {
                                foreach (var item in uniqueWords)
                                {
                                    if (!string.IsNullOrEmpty(item) && item.Length > 10)
                                    {
                                        temp_RFID_Tag = item;
                                        break;
                                    }
                                }
                            }
                        }
                        catch
                        {
                            temp_RFID_Tag = "";
                        }

                        string RFID_Tag = !string.IsNullOrEmpty(temp_RFID_Tag) ? temp_RFID_Tag.Trim() : "";
                        Constant.DisplayRFID_Right = string.Empty;
                        if (!string.IsNullOrEmpty(RFID_Tag) && RFID_Tag.Length >= 24)
                        {
                            Constant.RFID = RFID_Tag;
                            Constant.WBSIDE_DASHBORD = "Right";
                            Constant.UHFRIGHT = RFID_Tag;
                            try
                            {
                                UHFDataClasses.RemoveAll(p => p.ReadTime < DateTime.Now.AddSeconds(-20));

                                if (TokenLists.Where(p => p.TokenValue == RFID_Tag).Count() > 0)
                                {
                                    Constant.DisplayRFID_Right = TokenLists.Where(p => p.TokenValue == RFID_Tag).First().TokenName + ":" + RFID_Tag;
                                    int tagCount = UHFDataClasses.Where(p => p.Tag == RFID_Tag).Count();
                                    UHFDataClasses.Add(new UHFDataClass { Tag = RFID_Tag, Count = tagCount, UHFSide = "Right", ReadTime = DateTime.Now });
                                }
                                else
                                {
                                    Constant.DisplayRFID_Left = RFID_Tag + "Token Not Found";
                                    try
                                    {
                                        using (weighbridgeEntities ctToken = new weighbridgeEntities(Constant.EntityConnectionString))
                                        {
                                            if (ctToken.Tokens.Where(p => p.TokenValue == RFID_Tag).Count() > 0)
                                            {
                                                Token token = ctToken.Tokens.Where(p => p.TokenValue == RFID_Tag).FirstOrDefault();
                                                TokenLists.Add(token);
                                            }
                                        }
                                    }
                                    catch { }

                                }

                                viewModel.RFIDText = RFID_Tag;
                            }
                            catch { }
                        }
                        else
                        {

                        }
                    }
                    catch { }
                });
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    viewModel.RFIDText = "";
                    Constant.RFID = "";
                    Constant.DisplayRFID_Left = "";
                    Constant.DisplayRFID_Right = "";
                    UHFDataClasses.RemoveAll(p => p.ReadTime < DateTime.Now.AddSeconds(-1));
                });
            }
        }

        private void StartListionData(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Constant.RFID = string.Empty;
                viewModel.RFIDText = string.Empty;
                Constant.DisplayRFID_Left = string.Empty;
                Constant.DisplayRFID_Right = string.Empty;
                UHFDataClasses.RemoveAll(p => p.ReadTime < DateTime.Now.AddSeconds(-1));

            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }
        }

        private void StartProcessUHF()
        {
            try
            {
                Constant.IsProcessUHF = false;

                while (true)
                {
                    if (Constant.IsProcessUHF == false)
                    {
                        Constant.IsProcessUHF = true;

                        string ProcessRFIDSTR = string.Empty;

                        try
                        {
                            UHFDataClasses.RemoveAll(p => p.ReadTime < DateTime.Now.AddSeconds(-20));
                            if (UHFDataClasses.Count() > 0)
                            {
                                int maxCount = UHFDataClasses.Max(x => x.Count);
                                ProcessRFIDSTR = UHFDataClasses.Where(x => x.Count == maxCount).FirstOrDefault().Tag;

                            }
                        }
                        catch
                        {

                        }

                        try
                        {
                            if (!string.IsNullOrEmpty(ProcessRFIDSTR) && ProcessRFIDSTR.Length >= 24)
                            {
                                int RFIDCount = UHFDataClasses.Where(p => ProcessRFIDSTR.Contains(p.Tag) || p.Tag.Contains(ProcessRFIDSTR)).Count();

                                if (RFIDCount >= 2)
                                {
                                    string RFIDSTR = ProcessRFIDSTR;

                                    string WeightbridgeSide = UHFDataClasses.Where(p => ProcessRFIDSTR.Contains(p.Tag) || p.Tag.Contains(ProcessRFIDSTR)).FirstOrDefault().UHFSide;
                                    try
                                    {
                                        string imageLocation = Constant.PluginFileLocation + @"\Tags\";
                                        System.IO.DirectoryInfo folderInfo = new DirectoryInfo(imageLocation);
                                        if (!folderInfo.Exists)
                                            folderInfo.Create();

                                        string Exceptionjson = Environment.NewLine + $"------------------- {IsNumberListen} ----- " + DateTime.Now + Environment.NewLine + RFIDSTR;
                                        File.AppendAllText(Constant.PluginFileLocation + @"\Tags\RFID_Tags" + DateTime.Now.ToString("yyyyMMdd_HH") + ".txt", Exceptionjson);
                                        try
                                        {
                                            File.WriteAllText(Constant.PluginFileLocation + @"\Tags\RFID.txt", RFIDSTR);
                                        }
                                        catch { }

                                    }
                                    catch (Exception ex)
                                    {
                                        _ = ex.Message;
                                    }

                                    try
                                    {
                                        string imageLocation = Constant.PluginFileLocation + @"\Tags\";
                                        System.IO.DirectoryInfo folderInfo = new DirectoryInfo(imageLocation);
                                        foreach (FileInfo file in folderInfo.GetFiles())
                                        {
                                            if (file.CreationTime <= DateTime.Now.AddDays(-1))
                                                file.Delete();
                                        }
                                    }
                                    catch
                                    {

                                    }

                                    if (IsNumberListen == false)
                                        return;


                                    string RFIDIN = $"RFIDIN:{RFIDSTR}:{WeightbridgeSide}";

                                    File.WriteAllText(Constant.PluginFileLocation + @"\Tags\RFIDOfCurrentTrans.txt", RFIDSTR);


                                    if (!ProcessClientData_Executing)
                                    {
                                        ProcessClientData(RFIDIN, ActiveWeighBridge);
                                        UHFDataClasses = new List<UHFDataClass>();
                                    }


                                    viewModel.RFIDCount = $"RFID Tap 3 Time";
                                }
                                else
                                {
                                    Thread.Sleep(1000);

                                    viewModel.RFIDCount = $"RFID Tap {RFIDCount} Time";
                                }
                            }
                            else
                            {
                                viewModel.RFIDCount = $"RFID Tap 0 Time";
                            }
                        }
                        catch { }

                        Constant.IsProcessUHF = false;
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }

        }

        private void ProcessClientData(string ClientData, WeightBridge weightbridge)
        {
            ProcessClientData_Executing = true;
            try
            {
                using (weighbridgeEntities ctx = new weighbridgeEntities(System.Windows.Application.Current.Properties["EntityConnectionString"].ToString()))
                {
                    TCPLog objTCPLog = new TCPLog();

                    objTCPLog.RequestData = "\n[RCV] " + ClientData;
                    objTCPLog.LogTime = DateTime.Now;

                    string returnData = "0";
                    string displayMessage = "Data not as per required format. Ignored.";

                    string[] DataParts = ClientData.Split(":".ToCharArray());
                    if (DataParts.Length >= 2)
                    {
                        string command = DataParts[0].Trim();
                        string RFIDToken = DataParts[1].Trim();
                        string WeightbridgeSide = DataParts[2].Trim();
                        Token objToken = ctx.Tokens.FirstOrDefault(x => x.TokenValue == RFIDToken);
                        if (objToken != null)
                        {
                            switch (command)
                            {
                                case "RFIDIN":
                                    if (DataParts.Length == 3)
                                    {
                                        if (ActiveWeighBridge != null)
                                        {
                                            lock (RFIDTokens)
                                            {
                                                if (objToken != null)
                                                {
                                                    int AllowCycleTime = ActiveWeighBridge.AllowCycleTime.GetValueOrDefault() == 0 ? 5 : ActiveWeighBridge.AllowCycleTime.Value;
                                                    DateTime CheckDatetime = DateTime.Now.AddMinutes(-AllowCycleTime);

                                                    List<TransactionData> transactionDataList = ctx.TransactionDatas.Where(p => p.TokenValue == p.TokenValue && p.Status == TransactionStatus.Active.ToString() && p.TransactionDetails.Any(td => td.CaptureWeight != null) && p.TransactionDetails.Max(x => x.CaptureTime) > CheckDatetime).ToList();
                                                    if (transactionDataList.Count > 0)
                                                    {
                                                        displayMessage = $"Transation not Allow before {AllowCycleTime} minute.";
                                                        Constant.Display_Vehicle = objToken.Vehicle_Number;
                                                        Constant.Color_Vehicle = "Red";
                                                        //returnData = "0";
                                                        returnData = "NOTOK";
                                                        File.AppendAllText(Constant.PluginFileLocation + "/UnmaneTestreturnData.txt", displayMessage);

                                                    }
                                                    else
                                                    {
                                                        if (objToken.TokenStatus == "Allocated")
                                                        {
                                                            if (objToken.TokenType == "Permanent" || objToken.TokenType == "Temporary" || objToken.TransactionTokenLogs.Any(x => x.FreeTime == null && !x.IsCancel && x.TransactionData.Status == "Active"))
                                                            {
                                                                TransactionTokenLog objTranToken = ctx.TransactionTokenLogs.Where(x => x.TokenID == objToken.TokenID && x.FreeTime == null && !x.IsCancel && x.TransactionData.Status == "Active").OrderByDescending(x => x.TokenID).FirstOrDefault();
                                                                if (objTranToken == null)
                                                                {
                                                                    bool isError = false;
                                                                    int LastTicket = 0;
                                                                    try
                                                                    {
                                                                        LastTicket = Convert.ToInt32(ctx.ApplicationSettings.Where(p => p.ModuleName == "Transaction").FirstOrDefault(x => x.SettingName == "LastTicket").SettingValue);
                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                                                                        File.AppendAllText(Constant.PluginFileLocation + @"\CreateTransactionError.txt", json);
                                                                        try
                                                                        {
                                                                            LastTicket = ctx.TransactionDatas.Max(p => p.TicketID);
                                                                        }
                                                                        catch (Exception exp)
                                                                        {
                                                                            string jsonexp = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(exp);
                                                                            File.AppendAllText(Constant.PluginFileLocation + @"\CreateTransactionError.txt", jsonexp);
                                                                            isError = true;
                                                                        }
                                                                    }
                                                                    int TicketID = LastTicket + 1;

                                                                    TransactionData objTranData = new TransactionData();
                                                                    objTranData.CreationTime = DateTime.Now;
                                                                    objTranData.TicketID = TicketID;
                                                                    objTranData.Status = "Active";
                                                                    objTranData.TransactionMode = "Double";
                                                                    objTranData.TransactionType = "Incoming";
                                                                    objTranData.ReceiptTicketID = Guid.NewGuid();
                                                                    objTranData.CreatedBy = Convert.ToInt32(Application.Current.Properties["LoggedInUserID"].ToString());
                                                                    objTranData.CreatedName = Application.Current.Properties["LoggedInUserName"].ToString();

                                                                    if (objToken.TokenType == "Permanent")
                                                                    {
                                                                        objTranData.AccountID = objToken.AccountID;
                                                                        objTranData.Account_Name = objToken.Account_Name;
                                                                        objTranData.VehicleID = objToken.VehicleID;
                                                                        objTranData.Vehicle_Number = objToken.Vehicle_Number;

                                                                        objTranData.TransporterID = objToken.TransporterID;
                                                                        objTranData.Transporter_Name = objToken.Transporter_Name;
                                                                        objTranData.DriverID = objToken.DriverID;
                                                                        objTranData.Driver_Name = objToken.Driver_Name;

                                                                        objTranData.Charges = objToken.Charges;
                                                                        objTranData.TokenName = objToken.TokenName;
                                                                        objTranData.TokenValue = objToken.TokenValue;
                                                                        if (objToken.PreferredTransactionMode != null)
                                                                            objTranData.TransactionMode = objToken.PreferredTransactionMode;
                                                                        if (objToken.PreferredTransactionType != null)
                                                                            objTranData.TransactionType = objToken.PreferredTransactionType;
                                                                    }
                                                                    objTranData.CreatedName = Application.Current.Properties["LoggedInUserName"].ToString();
                                                                    if (!string.IsNullOrEmpty(objToken.CustomValue1)) objTranData.CustomValue1 = objToken.CustomValue1;
                                                                    if (!string.IsNullOrEmpty(objToken.CustomValue2)) objTranData.CustomValue2 = objToken.CustomValue2;
                                                                    if (!string.IsNullOrEmpty(objToken.CustomValue3)) objTranData.CustomValue3 = objToken.CustomValue3;
                                                                    if (!string.IsNullOrEmpty(objToken.CustomValue4)) objTranData.CustomValue4 = objToken.CustomValue4;
                                                                    if (!string.IsNullOrEmpty(objToken.CustomValue5)) objTranData.CustomValue5 = objToken.CustomValue5;
                                                                    if (!string.IsNullOrEmpty(objToken.CustomValue6)) objTranData.CustomValue6 = objToken.CustomValue6;
                                                                    if (!string.IsNullOrEmpty(objToken.CustomValue7)) objTranData.CustomValue7 = objToken.CustomValue7;
                                                                    if (!string.IsNullOrEmpty(objToken.CustomValue8)) objTranData.CustomValue8 = objToken.CustomValue8;
                                                                    if (!string.IsNullOrEmpty(objToken.CustomValue9)) objTranData.CustomValue9 = objToken.CustomValue9;
                                                                    if (!string.IsNullOrEmpty(objToken.CustomValue10)) objTranData.CustomValue10 = objToken.CustomValue10;
                                                                    if (!string.IsNullOrEmpty(objToken.CustomValue11)) objTranData.CustomValue11 = objToken.CustomValue11;
                                                                    if (!string.IsNullOrEmpty(objToken.CustomValue12)) objTranData.CustomValue12 = objToken.CustomValue12;
                                                                    if (!string.IsNullOrEmpty(objToken.CustomValue13)) objTranData.CustomValue13 = objToken.CustomValue13;
                                                                    if (!string.IsNullOrEmpty(objToken.CustomValue14)) objTranData.CustomValue14 = objToken.CustomValue14;
                                                                    if (!string.IsNullOrEmpty(objToken.CustomValue15)) objTranData.CustomValue15 = objToken.CustomValue15;
                                                                    if (!string.IsNullOrEmpty(objToken.CustomValue16)) objTranData.CustomValue16 = objToken.CustomValue16;
                                                                    if (!string.IsNullOrEmpty(objToken.CustomValue17)) objTranData.CustomValue17 = objToken.CustomValue17;
                                                                    if (!string.IsNullOrEmpty(objToken.CustomValue18)) objTranData.CustomValue18 = objToken.CustomValue18;
                                                                    if (!string.IsNullOrEmpty(objToken.CustomValue19)) objTranData.CustomValue19 = objToken.CustomValue19;
                                                                    if (!string.IsNullOrEmpty(objToken.CustomValue20)) objTranData.CustomValue20 = objToken.CustomValue20;
                                                                    ctx.TransactionDatas.Add(objTranData);
                                                                    ctx.SaveChanges();

                                                                    string queryupdateTicket = $"update ApplicationSettings set SettingValue = '{objTranData.TicketID}' where ModuleName = 'Transaction' and SettingName = 'LastTicket' ";
                                                                    bool flagupdateTicket = DatabaseFunctions.ExecuteQuery(queryupdateTicket);

                                                                    objTranToken = new TransactionTokenLog();
                                                                    objTranToken.AllocateTime = DateTime.Now;
                                                                    objTranToken.ReceiptTicketID = objTranData.ReceiptTicketID;
                                                                    objTranToken.IsCancel = false;
                                                                    objToken.TransactionTokenLogs.Add(objTranToken);
                                                                    ctx.SaveChanges();

                                                                    displayMessage = $"Welcome to Weighbridge. Your Vehicle \"{objToken.Vehicle.VehicleNumber}\" is Authorised.";
                                                                    string VehicleNumber = objToken.Vehicle.VehicleNumber;

                                                                    if (VehicleNumber.Length < 13)
                                                                    {
                                                                        for (int i = VehicleNumber.Length; i < 13; i++)
                                                                            VehicleNumber += "*";
                                                                    }

                                                                    File.AppendAllText(Constant.PluginFileLocation + @"\TCPLog.txt", Environment.NewLine + VehicleNumber);
                                                                    //returnData = "1";
                                                                    returnData = "OKTRUCK:" + VehicleNumber;
                                                                    Constant.Display_Vehicle = objToken.Vehicle.VehicleNumber;
                                                                    Constant.Color_Vehicle = "Green";

                                                                    if (Constant.Final_Reset)
                                                                    {
                                                                        Constant.IsVehicleEnter = false;
                                                                        if (WeightbridgeSide == "Left")
                                                                        {
                                                                            Constant.WBSIDE = "Left";

                                                                        }
                                                                        else
                                                                        {
                                                                            Constant.WBSIDE = "Right";

                                                                        }
                                                                        Constant.Final_Reset = false;
                                                                        Constant.Final_end = false;
                                                                        Constant.IsUnmanCycle_end = false;
                                                                        Constant.IsWeighSaved = false;
                                                                        Constant.IsWeighStart = false;

                                                                        GlobalRFIDToken = RFIDToken;
                                                                        //if (Common.ValidatePageRequest(PageEntity.Transaction, PageAccess.Update))
                                                                        //{
                                                                        //    this.Dispatcher.Invoke(() =>
                                                                        //    {
                                                                        //        MainFrame.NavigationService.Navigate(new WeighingSolution.View.TransactionManagement(objTranData.ReceiptTicketID, true));

                                                                        //    });
                                                                        //}
                                                                        play_card_ok();
                                                                    }

                                                                }
                                                                else
                                                                {
                                                                    if (ctx.TransactionDatas.Where(p => p.ReceiptTicketID == objTranToken.ReceiptTicketID).Count() > 0)
                                                                    {
                                                                        TransactionData transactionData = ctx.TransactionDatas.Include("Vehicle").Where(p => p.ReceiptTicketID == objTranToken.ReceiptTicketID).FirstOrDefault();

                                                                        displayMessage = $"Welcome to Weighbridge. Your Vehicle \"{transactionData.Vehicle.VehicleNumber}\" is Authorised.";
                                                                        //returnData = "1";
                                                                        string VehicleNumber = transactionData.Vehicle.VehicleNumber;
                                                                        if (VehicleNumber.Length < 13)
                                                                        {
                                                                            for (int i = VehicleNumber.Length; i < 13; i++)
                                                                                VehicleNumber += "*";
                                                                        }

                                                                        File.AppendAllText(Constant.PluginFileLocation + @"\TCPLog.txt", Environment.NewLine + VehicleNumber);

                                                                        returnData = "OKTRUCK:" + VehicleNumber;
                                                                        Constant.Display_Vehicle = transactionData.Vehicle.VehicleNumber;
                                                                        Constant.Color_Vehicle = "Green";

                                                                        if (Constant.Final_Reset)
                                                                        {
                                                                            Constant.IsVehicleEnter = false;
                                                                            if (WeightbridgeSide == "Left")
                                                                            {
                                                                                Constant.WBSIDE = "Left";
                                                                            }
                                                                            else
                                                                            {
                                                                                Constant.WBSIDE = "Right";

                                                                            }
                                                                            Constant.Final_Reset = false;
                                                                            Constant.Final_end = false;
                                                                            Constant.IsUnmanCycle_end = false;
                                                                            Constant.IsWeighSaved = false;
                                                                            Constant.IsWeighStart = false;


                                                                            GlobalRFIDToken = RFIDToken;
                                                                            //if (Common.ValidatePageRequest(PageEntity.Transaction, PageAccess.Update))
                                                                            //{
                                                                            //    this.Dispatcher.Invoke(() =>
                                                                            //    {
                                                                            //        MainFrame.NavigationService.Navigate(new WeighingSolution.View.TransactionManagement(transactionData.ReceiptTicketID, true));

                                                                            //    });
                                                                            //}
                                                                            play_card_ok();
                                                                        }

                                                                    }
                                                                    else
                                                                    {
                                                                        displayMessage = "RFID is not allocate , RFID Token Value: " + RFIDToken;
                                                                        returnData = "NOTOK";
                                                                        modbusHelper.Ideal();
                                                                        Constant.Color_Vehicle = "Red";
                                                                        play_card_not_ok();
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                displayMessage = "RFID is expired, RFID Token Value: " + RFIDToken;
                                                                returnData = "NOTOK";
                                                                Constant.Color_Vehicle = "Red";
                                                                modbusHelper.Ideal();
                                                                play_card_not_ok();
                                                            }
                                                        }
                                                        else
                                                        {
                                                            displayMessage = "RFID is not allocate , RFID Token Value: " + RFIDToken;
                                                            returnData = "NOTOK";
                                                            Constant.Color_Vehicle = "Red";
                                                            modbusHelper.Ideal();
                                                            play_card_not_ok();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    displayMessage = "RFID couldn't found in system, RFID Token Value: " + RFIDToken;
                                                    returnData = "NOTOK";
                                                    Constant.Color_Vehicle = "Red";
                                                    modbusHelper.Ideal();
                                                    play_card_not_ok();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            displayMessage = "PLC is not registered with system.";
                                            returnData = "NOTOK";
                                            Constant.Color_Vehicle = "Red";
                                            modbusHelper.Ideal();
                                            play_card_not_ok();
                                        }

                                    }
                                    break;
                            }
                        }
                    }

                    viewModel.CycleMSG = displayMessage;
                    Constant.Display_Message = displayMessage;
                    //Console.WriteLine("[" + PLC_IP_Address + "] " + displayMessage);
                    objTCPLog.ResponseData = returnData + " " + displayMessage;
                    ctx.TCPLogs.Add(objTCPLog);
                    ctx.SaveChanges();
                    try
                    {
                        string DeleteQ = "delete from TCPLog where LogTime < GETDATE()-1";
                        DatabaseFunctions.ExecuteQuery(DeleteQ);
                    }
                    catch { }

                    string returnDataStr = Environment.NewLine + "--------------------------------" + DateTime.Now.ToString("dd/MM/yyyy H:mm:ss") + Environment.NewLine + displayMessage + Environment.NewLine + returnData;
                    System.IO.File.AppendAllText(Constant.PluginFileLocation + "/UnmaneTestreturnData.txt", returnDataStr);
                    //SendDataOnSocket(returnData);
                }
            }
            catch (Exception ex)
            {
                string jsonstring = JsonConvert.SerializeObject(ex);
                string Datatest = Environment.NewLine + "--------------------------------" + DateTime.Now.ToString("dd/MM/yyyy H:mm:ss") + Environment.NewLine + jsonstring;
                File.AppendAllText(Constant.PluginFileLocation + "/UnmaneTest.txt", Datatest);

                File.AppendAllText(Constant.CustomSettingsPath + "/Trace.txt", ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + "--------------------------------" + DateTime.Now.ToString("dd/MM/yyyy H:mm:ss") + Environment.NewLine);
                File.AppendAllText(Constant.CustomSettingsPath + "/Trace.txt", ex.InnerException + Environment.NewLine + "-------Inner Exception---------" + Environment.NewLine + "--------------------------------" + DateTime.Now.ToString("dd/MM/yyyy H:mm:ss") + Environment.NewLine);
            }
            ProcessClientData_Executing = false;
        }

        #endregion
        #region modbusLogiv
        private void CheckModbus(object sender, System.Timers.ElapsedEventArgs e)
        {
            timerModbus.Enabled = false;
            int Cycleendcount = (ActiveWeighBridge.CycleEndTime.GetValueOrDefault() == 0 ? 15 : ActiveWeighBridge.CycleEndTime.Value);
            int CaptureWeightCount = (ActiveWeighBridge.CaptureWeightCount.GetValueOrDefault() == 0 ? 15 : ActiveWeighBridge.CaptureWeightCount.Value);
            int IdleBeforeCapture = Math.Max(CaptureWeightCount - 5, 0);
            try
            {
                if (modbusHelper.PLCPort.IsOpen)
                {
                    string ReadData = modbusHelper.ExecuteReadCommand();

                    if (!string.IsNullOrEmpty(ReadData))
                    {

                        string MSG = $"Time :- {DateTime.Now:HH:mm:ss} , Data = {ReadData} ,";
                        if (string.IsNullOrEmpty(Constant.WBSIDE))
                        {
                            if (!Constant.ISManual)
                            {
                                modbusHelper.Ideal();
                            }
                        }

                        if (Constant.WBSIDE == "Left")
                        {

                            switch (ReadData)
                            {
                                case "2048":
                                    if (Constant.IsWeighSaved && Constant.Final_end == false)
                                    {
                                        Constant.IsUnmanCycle_end = true;
                                    }
                                    MSG += " Forth Sensor Cut ,";

                                    Constant.IsFirstSensorOn = false;
                                    Constant.IsSecoundSensorOn = false;
                                    Constant.IsTrirdSensorOn = false;
                                    Constant.IsForthSensorOn = true;
                                    Constant.CycleCount = 0;
                                    break;
                                case "1024":
                                    if (Constant.IsWeighSaved && Constant.Final_end == false)
                                    {
                                        Constant.IsUnmanCycle_end = false;
                                    }
                                    MSG += " Third Sensor Cut ,";
                                    Constant.IsFirstSensorOn = false;
                                    Constant.IsSecoundSensorOn = false;
                                    Constant.IsTrirdSensorOn = true;
                                    Constant.IsForthSensorOn = false;
                                    Constant.CycleCount = 0;
                                    break;
                                case "3072":
                                    if (Constant.Final_end == false)
                                    {
                                        Constant.IsUnmanCycle_end = false;
                                    }
                                    MSG += " Forth Sensor Cut and Third Sensor Cut ,";
                                    Constant.IsFirstSensorOn = false;
                                    Constant.IsSecoundSensorOn = false;
                                    Constant.IsTrirdSensorOn = true;
                                    Constant.IsForthSensorOn = true;
                                    Constant.CycleCount = 0;
                                    break;

                                case "768":
                                    if (Constant.Final_end == false)
                                    {
                                        Constant.IsUnmanCycle_end = false;
                                    }
                                    MSG += " First Sensor Cut and Second Sensor Cut ,";
                                    Constant.IsFirstSensorOn = true;
                                    Constant.IsSecoundSensorOn = true;
                                    Constant.IsTrirdSensorOn = false;
                                    Constant.IsForthSensorOn = false;
                                    Constant.CycleCount = 0;
                                    break;
                                case "512":
                                    if (Constant.IsWeighSaved && Constant.Final_end == false)
                                    {
                                        Constant.IsUnmanCycle_end = false;
                                    }
                                    MSG += " Second Sensor Cut ,";
                                    Constant.IsFirstSensorOn = false;
                                    Constant.IsSecoundSensorOn = true;
                                    Constant.IsTrirdSensorOn = false;
                                    Constant.IsForthSensorOn = false;
                                    Constant.CycleCount = 0;
                                    break;
                                case "256":
                                    MSG += " First Sensor Cut ,";
                                    Constant.IsFirstSensorOn = true;
                                    Constant.IsSecoundSensorOn = false;
                                    Constant.IsTrirdSensorOn = false;
                                    Constant.IsForthSensorOn = false;
                                    Constant.CycleCount = 0;
                                    break;
                                case "0":

                                    if (Constant.IsSecoundSensorOn)
                                    { Constant.IsWeighStart = true; Constant.IsWeighDashStart = true; }

                                    if (Constant.IsTrirdSensorOn && Constant.IsWeighSaved)
                                        Constant.IsUnmanCycle_end = true;

                                    if (Constant.CycleCount > Cycleendcount && Constant.IsWeighSaved && Constant.IsUnmanCycle_end)
                                    {
                                        //Thread.Sleep(5000);
                                        for (int i = 0; i < 3; i++)
                                        {
                                            modbusHelper.Ideal();
                                            Thread.Sleep(1000);
                                        }
                                        Constant.Final_end = true;
                                        Constant.Final_Reset = true;
                                        Constant.IsUnmanCycle_end = false;
                                        Constant.IsWeighSaved = false;
                                        Constant.IsWeighStart = false;
                                        Constant.WBSIDE = "";
                                        Constant.WBSIDE_DASHBORD = "";
                                        viewModel.CycleMSG = string.Empty;
                                        Constant.Display_Message = string.Empty;
                                        Constant.Display_Vehicle = string.Empty;
                                        Constant.Color_Vehicle = string.Empty;
                                        Constant.DisplayRFID_Left = string.Empty;
                                        Constant.DisplayRFID_Right = string.Empty;


                                    }
                                    else if (Constant.CycleCount > Cycleendcount && Constant.IsUnmanCycle_end && Constant.Final_end)
                                    {
                                        modbusHelper.Ideal();
                                    }
                                    else if (Constant.IsWeighSaved)
                                    {
                                        modbusHelper.Left_Weight_Done();
                                        //Constant.Display_Message = string.Empty;
                                    }
                                    else if (Constant.IsWeighStart)
                                    {
                                        if (Constant.CycleCount > CaptureWeightCount)
                                        {
                                            viewModel.CycleMSG = "Weight reading started. Please wait...";
                                            Constant.Display_Message = "Weight reading started. Please wait...";
                                            play_weight_start();
                                            CaptureWeight();
                                            Constant.CycleCount = 0;
                                            Constant.IsWeighStart = false;
                                        }
                                        else if (Constant.CycleCount > IdleBeforeCapture)
                                        {
                                            modbusHelper.Ideal();
                                        }
                                        else if (Constant.CycleCount > Cycleendcount)
                                        {
                                            modbusHelper.Ideal();
                                        }

                                    }
                                    else if (!NetWeightNegative)
                                    {
                                        modbusHelper.Left_Card_OK();
                                    }

                                    Constant.CycleCount++;

                                    Constant.IsFirstSensorOn = false;
                                    Constant.IsSecoundSensorOn = false;
                                    Constant.IsTrirdSensorOn = false;
                                    Constant.IsForthSensorOn = false;

                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (Constant.WBSIDE == "Right")
                        {
                            switch (ReadData)
                            {
                                case "3072":
                                    if (Constant.IsWeighSaved && Constant.Final_end == false)
                                    {
                                        Constant.IsUnmanCycle_end = false;
                                    }
                                    MSG += " Forth Sensor Cut and Third Sensor Cut ,";
                                    Constant.IsFirstSensorOn = false;
                                    Constant.IsSecoundSensorOn = false;
                                    Constant.IsTrirdSensorOn = true;
                                    Constant.IsForthSensorOn = true;
                                    Constant.CycleCount = 0;
                                    break;
                                case "2048":
                                    if (Constant.IsWeighSaved && Constant.Final_end == false)
                                    {
                                        Constant.IsUnmanCycle_end = false;
                                    }
                                    MSG += " Forth Sensor Cut ,";
                                    Constant.IsFirstSensorOn = false;
                                    Constant.IsSecoundSensorOn = false;
                                    Constant.IsTrirdSensorOn = false;
                                    Constant.IsForthSensorOn = true;
                                    Constant.CycleCount = 0;
                                    break;
                                case "1024":
                                    if (Constant.IsWeighSaved && Constant.Final_end == false)
                                    {
                                        Constant.IsUnmanCycle_end = false;
                                    }
                                    MSG += " Third Sensor Cut ,";
                                    Constant.IsFirstSensorOn = false;
                                    Constant.IsSecoundSensorOn = false;
                                    Constant.IsTrirdSensorOn = true;
                                    Constant.IsForthSensorOn = false;
                                    Constant.CycleCount = 0;
                                    break;
                                case "768":
                                    if (Constant.Final_end == false)
                                    {
                                        Constant.IsUnmanCycle_end = false;
                                    }
                                    MSG += " First Sensor Cut and Second Sensor Cut ,";
                                    Constant.IsFirstSensorOn = true;
                                    Constant.IsSecoundSensorOn = true;
                                    Constant.IsTrirdSensorOn = false;
                                    Constant.IsForthSensorOn = false;
                                    Constant.CycleCount = 0;
                                    break;
                                case "512":
                                    if (Constant.IsWeighSaved && Constant.Final_end == false)
                                    {
                                        Constant.IsUnmanCycle_end = false;
                                    }
                                    MSG += " Second Sensor Cut ,";
                                    Constant.IsFirstSensorOn = false;
                                    Constant.IsSecoundSensorOn = true;
                                    Constant.IsTrirdSensorOn = false;
                                    Constant.IsForthSensorOn = false;
                                    Constant.CycleCount = 0;
                                    break;
                                case "256":
                                    MSG += " First Sensor Cut ,";
                                    Constant.IsFirstSensorOn = true;
                                    Constant.IsSecoundSensorOn = false;
                                    Constant.IsTrirdSensorOn = false;
                                    Constant.IsForthSensorOn = false;
                                    if (Constant.IsUnmanCycle_end == false && Constant.Final_end == false)
                                    {
                                        Constant.IsUnmanCycle_end = true;

                                    }
                                    Constant.CycleCount = 0;
                                    break;
                                case "0":

                                    if (Constant.IsTrirdSensorOn)
                                    { Constant.IsWeighStart = true; Constant.IsWeighDashStart = true; }

                                    if (Constant.IsSecoundSensorOn && Constant.IsWeighSaved)
                                        Constant.IsUnmanCycle_end = true;

                                    if (Constant.CycleCount > Cycleendcount && Constant.IsWeighSaved && Constant.IsUnmanCycle_end)
                                    {
                                        //Thread.Sleep(5000);
                                        for (int i = 0; i < 3; i++)
                                        {
                                            modbusHelper.Ideal();
                                            Thread.Sleep(1000);
                                        }
                                        Constant.Final_end = true;
                                        Constant.Final_Reset = true;
                                        Constant.IsUnmanCycle_end = false;
                                        Constant.IsWeighSaved = false;
                                        Constant.IsWeighStart = false;
                                        Constant.WBSIDE = "";
                                        Constant.WBSIDE_DASHBORD = "";
                                        viewModel.CycleMSG = string.Empty;
                                        Constant.Display_Message = string.Empty;
                                        Constant.Display_Vehicle = string.Empty;
                                        Constant.Color_Vehicle = string.Empty;
                                        Constant.DisplayRFID_Left = string.Empty;
                                        Constant.DisplayRFID_Right = string.Empty;
                                    }
                                    else if (Constant.CycleCount > Cycleendcount && Constant.IsUnmanCycle_end && Constant.Final_end)
                                    {
                                        modbusHelper.Ideal();


                                    }
                                    else if (Constant.IsWeighSaved)
                                    {
                                        modbusHelper.Right_Weight_Done();
                                        //Constant.Display_Message = string.Empty;
                                    }
                                    else if (Constant.IsWeighStart)
                                    {

                                        if (Constant.CycleCount > CaptureWeightCount)
                                        {
                                            viewModel.CycleMSG = "Weight reading started. Please wait...";
                                            Constant.Display_Message = "Weight reading started. Please wait...";
                                            play_weight_start();
                                            CaptureWeight();
                                        }
                                        else if (Constant.CycleCount > IdleBeforeCapture)
                                        {
                                            modbusHelper.Ideal();
                                        }
                                        else if (Constant.CycleCount > Cycleendcount)
                                        {
                                            modbusHelper.Ideal();
                                        }

                                    }
                                    else if (!NetWeightNegative)
                                    {
                                        modbusHelper.Right_Card_OK();
                                    }

                                    Constant.CycleCount++;

                                    Constant.IsFirstSensorOn = false;
                                    Constant.IsSecoundSensorOn = false;
                                    Constant.IsTrirdSensorOn = false;
                                    Constant.IsForthSensorOn = false;

                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            switch (ReadData)
                            {
                                case "3072":

                                    MSG += " Forth Sensor Cut and Third Sensor Cut ,";
                                    Constant.IsFirstSensorOn = false;
                                    Constant.IsSecoundSensorOn = false;
                                    Constant.IsTrirdSensorOn = true;
                                    Constant.IsForthSensorOn = true;
                                    break;
                                case "2048":

                                    MSG += " Forth Sensor Cut ,";
                                    Constant.IsFirstSensorOn = false;
                                    Constant.IsSecoundSensorOn = false;
                                    Constant.IsTrirdSensorOn = false;
                                    Constant.IsForthSensorOn = true;
                                    break;
                                case "1536":

                                    MSG += "Second and Third Sensor Cut,";
                                    Constant.IsFirstSensorOn = false;
                                    Constant.IsSecoundSensorOn = true;
                                    Constant.IsTrirdSensorOn = true;
                                    Constant.IsForthSensorOn = false;
                                    break;
                                case "1024":

                                    MSG += " Third Sensor Cut ,";
                                    Constant.IsFirstSensorOn = false;
                                    Constant.IsSecoundSensorOn = false;
                                    Constant.IsTrirdSensorOn = true;
                                    Constant.IsForthSensorOn = false;
                                    Constant.CycleCount = 0;
                                    break;
                                case "768":

                                    MSG += " First Sensor Cut and Second Sensor Cut ,";
                                    Constant.IsFirstSensorOn = true;
                                    Constant.IsSecoundSensorOn = true;
                                    Constant.IsTrirdSensorOn = false;
                                    Constant.IsForthSensorOn = false;
                                    break;
                                case "512":
                                    MSG += " Second Sensor Cut ,";
                                    Constant.IsFirstSensorOn = false;
                                    Constant.IsSecoundSensorOn = true;
                                    Constant.IsTrirdSensorOn = false;
                                    Constant.IsForthSensorOn = false;
                                    break;
                                case "256":
                                    MSG += " First Sensor Cut ,";
                                    Constant.IsFirstSensorOn = true;
                                    Constant.IsSecoundSensorOn = false;
                                    Constant.IsTrirdSensorOn = false;
                                    Constant.IsForthSensorOn = false;
                                    break;
                                case "0":

                                    Constant.IsFirstSensorOn = false;
                                    Constant.IsSecoundSensorOn = false;
                                    Constant.IsTrirdSensorOn = false;
                                    Constant.IsForthSensorOn = false;

                                    break;
                                default:
                                    break;
                            }
                        }

                        if (Constant.IsUnmanCycle_end)
                        {
                            string ModBusText = MSG + $"Weigh Saved: {Constant.IsWeighSaved} , Unman Cycle end: {Constant.IsUnmanCycle_end}, Final end: {Constant.Final_end} , Count: {Constant.CycleCount}";
                            viewModel.ModBusText = ModBusText;
                            File.AppendAllText(Constant.ModbusDataLocation + $@"\Modbus{DateTime.Now:yyyyMMddHH}.txt", ModBusText + Environment.NewLine);
                        }
                        else
                        {
                            string ModBusText = MSG + $"Weigh Saved: {Constant.IsWeighSaved} , Unman Cycle end: {Constant.IsUnmanCycle_end}, Final end: {Constant.Final_end}";
                            viewModel.ModBusText = ModBusText;
                            File.AppendAllText(Constant.ModbusDataLocation + $@"\Modbus{DateTime.Now:yyyyMMddHH}.txt", ModBusText + Environment.NewLine);
                        }
                    }
                    else
                    {
                        viewModel.ModBusText = "PLC data not read, Check Your Connection";
                    }
                }
                else
                {
                    viewModel.ModBusText = "Not Connected,please check your port";
                    modbusHelper.DoConnect();
                }
            }
            catch (Exception Ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(Ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\Modbus.txt", json);
            }
            timerModbus.Enabled = true;
        }
        public void CaptureWeight()
        {
            string returnData = "0";
            string displayMessage = "Data not as per required format. Ignored.";
            TCPLog objTCPLog = new TCPLog();

            //objTCPLog.RequestData = "\n[RCV] " + ClientData;
            objTCPLog.LogTime = DateTime.Now;
            string ProductErrorMsg = string.Empty;
            using (weighbridgeEntities ctx = new weighbridgeEntities(System.Windows.Application.Current.Properties["EntityConnectionString"].ToString()))
            {
                if (Constant.IsWeighStart) // DataParts[2] == "RFID")
                {
                    string RFID = File.ReadAllText(Constant.PluginFileLocation + @"\Tags\RFIDOfCurrentTrans.txt");
                    Token _objToken = ctx.Tokens.FirstOrDefault(x => x.TokenValue == GlobalRFIDToken);
                    if (_objToken != null)
                    {
                        int AllowCycleTime = ActiveWeighBridge.AllowCycleTime.GetValueOrDefault() == 0 ? 5 : ActiveWeighBridge.AllowCycleTime.Value;
                        DateTime CheckDatetime = DateTime.Now.AddMinutes(-AllowCycleTime);
                        List<TransactionData> transactionDataList = ctx.TransactionDatas.Where(p => p.VehicleID == _objToken.VehicleID && p.Status == TransactionStatus.Active.ToString() && p.TransactionDetails.Max(x => x.CaptureTime) > CheckDatetime).ToList();
                        if (transactionDataList.Count > 0)
                        {
                            displayMessage = $"Transation not Allow before {AllowCycleTime} minute.";
                            //returnData = "0";
                            returnData = "NOTOK";

                            System.IO.File.AppendAllText(Constant.PluginFileLocation + "/UnmaneTestreturnData.txt", displayMessage);

                        }
                        else
                        {
                            //var masterData = ctx.Master_Mapping.FirstOrDefault(x => x.VehicleNumber == vehicle.VehicleNumber);

                            TransactionData transaction = ctx.TransactionDatas.FirstOrDefault(x => x.VehicleID == _objToken.VehicleID && x.Status == TransactionStatus.Active.ToString());
                            if (transaction != null)
                            {
                                Guid ReceiptTicketID = transaction.ReceiptTicketID;
                            }

                            decimal dWeight;
                            try
                            {
                                // dWeight = Convert.ToDecimal(Constant.IndicatorWeight);
                                dWeight = StableWt();

                                File.AppendAllText(Constant.PluginFileLocation + "/letestWeightTest.txt", dWeight.ToString());
                            }
                            catch
                            {
                                dWeight = 0;
                            }

                            File.AppendAllText(Constant.PluginFileLocation + "/WeightTest.txt", dWeight.ToString());
                            File.AppendAllText(Constant.PluginFileLocation + "/DWeightTest.txt", dWeight.ToString());

                            decimal MinAllowWeigh = (ActiveWeighBridge.MinimumWeight.GetValueOrDefault() == 0 ? 100 : ActiveWeighBridge.MinimumWeight.Value);

                            if (dWeight <= 0)
                            {
                                displayMessage = "Weight part is not a valid decimal";
                                //returnData = "0";
                                returnData = "NOTDONE";
                                play_weight_done();
                                //WriteToTheItKitSerial("NOTDONE");
                            }
                            else if (dWeight <= MinAllowWeigh)
                            {
                                displayMessage = $"Minimum Weight Required {MinAllowWeigh}";
                                //returnData = "0";
                                returnData = "NOTDONE";
                                play_weight_not_done();
                                //WriteToTheItKitSerial("NOTDONE");
                            }
                            else
                            {
                                Token _objToken1 = ctx.Tokens.FirstOrDefault(x => x.TokenValue == GlobalRFIDToken);
                                TransactionData objTranData = new TransactionData();
                                TransactionDetail PreviousTransaction = null;
                                if (_objToken1 == null)
                                {
                                    displayMessage = "RFID couldn't found in system, RFID Token Value: " + GlobalRFIDToken;
                                    returnData = "NOTDONE";
                                    play_weight_not_done();
                                    //WriteToTheItKitSerial("NOTDONE");
                                }
                                else
                                {
                                    TransactionTokenLog objTrantokenlog = ctx.TransactionTokenLogs.Where(x => x.TokenID == _objToken.TokenID && x.FreeTime == null && !x.IsCancel && x.TransactionData.Status == "Active").OrderByDescending(x => x.TokenID).FirstOrDefault();
                                    if (objTrantokenlog != null)
                                        objTranData = ctx.TransactionDatas.FirstOrDefault(x => x.ReceiptTicketID == objTrantokenlog.ReceiptTicketID.Value);

                                    if (objTranData != null)
                                    {
                                        decimal prewt = 0;
                                        TransactionDetail ActiveTransaction;
                                        if (objTranData.TransactionDetails.Count > 0)
                                        {
                                            PreviousTransaction = objTranData.TransactionDetails.Where(x => x.CaptureWeight != null && x.CaptureWeight.Value > 0).OrderByDescending(x => x.SequenceNo).FirstOrDefault();
                                            if (PreviousTransaction != null)
                                                prewt = PreviousTransaction.CaptureWeight.Value;
                                        }

                                        string ErrorMsg = string.Empty;

                                        if (PreviousTransaction != null)
                                        {

                                            if (PreviousTransaction.CaptureWeight == dWeight)
                                            {
                                                ErrorMsg = "Gross weight can not same as tare weight ";
                                            }
                                        }

                                        if (string.IsNullOrEmpty(ErrorMsg))
                                        {
                                            if (objTranData.TransactionDetails.Count > 0 && objTranData.TransactionDetails.Any(x => x.CaptureWeight == null || x.CaptureWeight.Value == 0))
                                            {
                                                ActiveTransaction = objTranData.TransactionDetails.FirstOrDefault(x => x.CaptureWeight == null || x.CaptureWeight.Value == 0);
                                                ActiveTransaction.WeightBridgeID = ActiveWeighBridge.WeightBridgeID;
                                                ActiveTransaction.WeightBridgeName = ActiveWeighBridge.ScaleName;
                                                ActiveTransaction.UserID = Convert.ToInt32(Application.Current.Properties["LoggedInUserID"].ToString());
                                                ActiveTransaction.WB_User_Name = Application.Current.Properties["LoggedInUserName"].ToString();
                                            }
                                            else
                                            {
                                                ActiveTransaction = new TransactionDetail();
                                                if (!string.IsNullOrEmpty(objTranData.TransactionType.ToString()))
                                                {
                                                    if (objTranData.TransactionType == "Incoming")
                                                    {
                                                        ActiveTransaction.WeighmentType = "Gross";
                                                    }
                                                    else
                                                    {
                                                        ActiveTransaction.WeighmentType = "Tare";
                                                    }

                                                }
                                                else
                                                {
                                                    ActiveTransaction.WeighmentType = "Gross";
                                                }
                                                ActiveTransaction.WeightBridgeID = ActiveWeighBridge.WeightBridgeID;
                                                ActiveTransaction.WeightBridgeName = ActiveWeighBridge.ScaleName;
                                                ActiveTransaction.WeightUnit = ActiveWeighBridge.Unit != null ? ActiveWeighBridge.Unit.UnitName : "";
                                                ActiveTransaction.SequenceNo = 1;
                                                ActiveTransaction.UserID = Convert.ToInt32(Application.Current.Properties["LoggedInUserID"].ToString());
                                                ActiveTransaction.WB_User_Name = Application.Current.Properties["LoggedInUserName"].ToString();
                                                if (objTranData.TransactionDetails.Count > 0)
                                                {
                                                    //PreviousTransaction = objTranData.TransactionDetails.OrderByDescending(x => x.SequenceNo).FirstOrDefault();
                                                    ActiveTransaction.SequenceNo = (byte)(PreviousTransaction.SequenceNo + 1);
                                                }
                                                ActiveTransaction.IsTareManual = false;
                                                ActiveTransaction.IsGrossManual = false;
                                                ActiveTransaction.IsCapturedManual = false;

                                                if (_objToken.TokenType == "Permanent" && objTranData.TransactionDetails.Count == 0)
                                                {
                                                    ActiveTransaction.GateID = _objToken.GateID != null ? _objToken.GateID : null;
                                                    ActiveTransaction.ProductID = _objToken.ProductID != null ? _objToken.ProductID : null;
                                                    ActiveTransaction.WeighmentType = _objToken.PreferredWeighmentType;
                                                    ActiveTransaction.Product_Name = _objToken.Product_Name;
                                                    ActiveTransaction.Gate_Name = _objToken.Gate_Name;
                                                }

                                                if (PreviousTransaction != null)
                                                {
                                                    if (objTranData.TransactionMode != "Multi")
                                                    {
                                                        ActiveTransaction.ProductID = PreviousTransaction.ProductID;
                                                        ActiveTransaction.Product = PreviousTransaction.Product;
                                                        ActiveTransaction.GateID = PreviousTransaction.GateID;
                                                        ActiveTransaction.Gate = PreviousTransaction.Gate;
                                                        ActiveTransaction.Product_Name = PreviousTransaction.Product_Name;
                                                        ActiveTransaction.Gate_Name = PreviousTransaction.Gate_Name;
                                                    }
                                                    //else
                                                    //{
                                                    //    string Content = Gate_API.GetMulti_Product(objTranData.GatePassID.ToString());
                                                    //    if (!string.IsNullOrEmpty(Content) && Content != "Error")
                                                    //    {
                                                    //        FetchProduct_Response gate_Tokens_Response = JsonConvert.DeserializeObject<FetchProduct_Response>(Content);

                                                    //        if (gate_Tokens_Response != null && gate_Tokens_Response.Data.Count > 0)
                                                    //        {
                                                    //            List<FetchProduct> gate_Tokens = gate_Tokens_Response.Data.ToList();
                                                    //            if (gate_Tokens.Count > 0)
                                                    //            {
                                                    //                foreach (var item in gate_Tokens)
                                                    //                {

                                                    //                    var _product = ctx.Products.Where(p => p.ProductName == item.ProductName);
                                                    //                    if (_product.Count() > 0)
                                                    //                    {
                                                    //                        var pro = _product.FirstOrDefault();
                                                    //                        ActiveTransaction.ProductID = pro.ProductID;
                                                    //                        ActiveTransaction.Product = pro;

                                                    //                    }
                                                    //                    else
                                                    //                    {
                                                    //                        decimal MaxWeight = item.MinWeright != null ? Convert.ToDecimal(item.MinWeright) : 0;
                                                    //                        decimal MinWeright = item.MinWeright != null ? Convert.ToDecimal(item.MinWeright) : 0;
                                                    //                        string query = $"INSERT INTO Product ([ProductName],[ProductCode],[IsActive],[ProductType],[MaxWeight],[MinWeright]) VALUES ('{item.ProductName}','{item.ProductCode}',1,'{item.ProductType}',{MaxWeight},{MinWeright})";

                                                    //                        bool flag = DatabaseFunctions.ExecuteQuery(query);
                                                    //                        if (flag)
                                                    //                        {
                                                    //                            var _proAdd = ctx.Products.Where(p => p.ProductName == item.ProductName);
                                                    //                            if (_proAdd.Count() > 0)
                                                    //                            {
                                                    //                                var pro = _product.FirstOrDefault();
                                                    //                                ActiveTransaction.ProductID = pro.ProductID;
                                                    //                                ActiveTransaction.Product = pro;
                                                    //                            }
                                                    //                        }
                                                    //                    }


                                                    //                }
                                                    //            }
                                                    //        }
                                                    //    }
                                                    //    else
                                                    //    {
                                                    //        displayMessage = "Your all Product Load/unload another are  not found.";
                                                    //        ProductErrorMsg = "Your all Product Load/unload another are  not found.";
                                                    //        returnData = "@";
                                                    //    }
                                                    //    ActiveTransaction.GateID = PreviousTransaction.GateID;
                                                    //    ActiveTransaction.Gate = PreviousTransaction.Gate;
                                                    //}

                                                }

                                                if (string.IsNullOrEmpty(ProductErrorMsg))
                                                {
                                                    objTranData.TransactionDetails.Add(ActiveTransaction);
                                                }
                                            }
                                            if (string.IsNullOrEmpty(ProductErrorMsg))
                                            {
                                                ActiveTransaction.CaptureWeight = dWeight;
                                                ActiveTransaction.CaptureTime = DateTime.Now;
                                                objTranData.TokenName = _objToken.TokenName;
                                                objTranData.TokenValue = _objToken.TokenValue;

                                                if (objTranData.TransactionDetails.Count > 1)
                                                {
                                                    if (objTranData.TransactionDetails.Count == 2)
                                                    {
                                                        bool IsLastGross = PreviousTransaction.GrossWeight != null && PreviousTransaction.GrossWeight.Value > 0;
                                                        bool IsLastTare = PreviousTransaction.TareWeight != null && PreviousTransaction.TareWeight.Value > 0;

                                                        if (IsLastGross && PreviousTransaction.GrossWeight < ActiveTransaction.CaptureWeight)
                                                        {
                                                            PreviousTransaction.TareWeight = PreviousTransaction.GrossWeight;
                                                            PreviousTransaction.GrossWeight = null;
                                                            PreviousTransaction.IsTareManual = PreviousTransaction.IsGrossManual;
                                                            PreviousTransaction.IsGrossManual = false;
                                                            PreviousTransaction.TareTime = PreviousTransaction.GrossTime;
                                                            PreviousTransaction.GrossTime = null;
                                                            PreviousTransaction.WeighmentType = "Tare";
                                                            objTranData.TransactionType = PreviousTransaction.TransactionData.TransactionType;
                                                            displayMessage = "In last pass you entered gross weight, but that was tare weight";
                                                            //returnData = "0";
                                                            //returnData = "NOTDONE";
                                                        }
                                                        if (IsLastTare && PreviousTransaction.TareWeight > ActiveTransaction.CaptureWeight)
                                                        {
                                                            PreviousTransaction.GrossWeight = PreviousTransaction.TareWeight;
                                                            PreviousTransaction.TareWeight = null;
                                                            PreviousTransaction.IsGrossManual = PreviousTransaction.IsTareManual;
                                                            PreviousTransaction.IsTareManual = false;
                                                            PreviousTransaction.GrossTime = PreviousTransaction.TareTime;
                                                            PreviousTransaction.TareTime = null;
                                                            PreviousTransaction.WeighmentType = "Gross";
                                                            objTranData.TransactionType = PreviousTransaction.TransactionData.TransactionType;
                                                            displayMessage = "In last pass you entered tare weight, but that was gross weight.";
                                                            //returnData = "0";
                                                            //returnData = "NOTDONE";
                                                        }
                                                    }
                                                    ActiveTransaction.GrossWeight = Math.Max(PreviousTransaction.CaptureWeight.GetValueOrDefault(0), ActiveTransaction.CaptureWeight.GetValueOrDefault(0));
                                                    ActiveTransaction.TareWeight = Math.Min(PreviousTransaction.CaptureWeight.GetValueOrDefault(0), ActiveTransaction.CaptureWeight.GetValueOrDefault(0));
                                                    if (PreviousTransaction.CaptureWeight > ActiveTransaction.CaptureWeight)
                                                    {
                                                        ActiveTransaction.GrossTime = PreviousTransaction.CaptureTime;
                                                        ActiveTransaction.IsGrossManual = PreviousTransaction.IsCapturedManual;
                                                        ActiveTransaction.TareTime = ActiveTransaction.CaptureTime;
                                                        ActiveTransaction.IsTareManual = ActiveTransaction.IsCapturedManual;
                                                        ActiveTransaction.WeighmentType = "Gross";
                                                        PreviousTransaction.WeighmentType = "Gross";
                                                    }
                                                    else
                                                    {
                                                        ActiveTransaction.TareTime = PreviousTransaction.CaptureTime;
                                                        ActiveTransaction.IsTareManual = PreviousTransaction.IsCapturedManual;
                                                        ActiveTransaction.GrossTime = ActiveTransaction.CaptureTime;
                                                        ActiveTransaction.IsGrossManual = ActiveTransaction.IsCapturedManual;
                                                        ActiveTransaction.WeighmentType = "Tare";
                                                        PreviousTransaction.WeighmentType = "Tare";
                                                    }
                                                    ActiveTransaction.NetWeight = ActiveTransaction.GrossWeight.Value - ActiveTransaction.TareWeight.Value;
                                                    if (ActiveTransaction.NetWeight < 0)
                                                    {
                                                        ActiveTransaction.NetWeight = null;
                                                        ActiveTransaction.CaptureWeight = null;
                                                        ActiveTransaction.GrossWeight = null;
                                                        displayMessage = "Net weight can not be negative. ";
                                                        Constant.Display_Message = displayMessage;
                                                        returnData = "NOTDONE";
                                                        NetWeightNegative = true;
                                                        play_weight_not_done();
                                                        Constant.IsWeighSaved = false;
                                                        viewModel.CycleMSG = displayMessage;
                                                        return;
                                                        // WriteToTheItKitSerial("NOTDONE");
                                                    }
                                                }
                                                else
                                                {
                                                    ActiveTransaction.SequenceNo = 0;
                                                    if (ActiveTransaction.WeighmentType == "Gross")
                                                    {
                                                        ActiveTransaction.GrossTime = ActiveTransaction.CaptureTime;
                                                        ActiveTransaction.GrossWeight = ActiveTransaction.CaptureWeight;
                                                        ActiveTransaction.IsGrossManual = ActiveTransaction.IsCapturedManual;
                                                        ActiveTransaction.TareTime = null;
                                                        ActiveTransaction.TareWeight = null;
                                                        ActiveTransaction.IsTareManual = false;
                                                        if (objTranData.TransactionMode == "Single")
                                                        {
                                                            ActiveTransaction.SequenceNo = 1;
                                                            if (objTranData.Vehicle.TareWeight != null && objTranData.Vehicle.TareWeight.Value > 0)
                                                            {
                                                                ActiveTransaction.TareWeight = objTranData.Vehicle.TareWeight.Value;
                                                                ActiveTransaction.TareTime = DateTime.Now;
                                                            }
                                                            if (ActiveTransaction.TareWeight != null && ActiveTransaction.TareWeight > 0 && ActiveTransaction.GrossWeight != null && ActiveTransaction.GrossWeight > 0)
                                                                ActiveTransaction.NetWeight = ActiveTransaction.GrossWeight.Value - ActiveTransaction.TareWeight.Value;
                                                            else
                                                                ActiveTransaction.NetWeight = 0;

                                                            if (ActiveTransaction.NetWeight < 0)
                                                            {
                                                                ActiveTransaction.NetWeight = null;
                                                                ActiveTransaction.CaptureWeight = null;
                                                                ActiveTransaction.GrossWeight = null;
                                                                displayMessage = "Net weight can not be negative. ";
                                                                Constant.Display_Message = displayMessage;
                                                                returnData = "NOTDONE";
                                                                NetWeightNegative = true;
                                                                play_weight_not_done();
                                                                Constant.IsWeighSaved = false;
                                                                viewModel.CycleMSG = displayMessage;
                                                                return;
                                                                // WriteToTheItKitSerial("NOTDONE");
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ActiveTransaction.TareTime = ActiveTransaction.CaptureTime;
                                                        ActiveTransaction.TareWeight = ActiveTransaction.CaptureWeight;
                                                        ActiveTransaction.IsTareManual = ActiveTransaction.IsCapturedManual;
                                                        ActiveTransaction.GrossTime = null;
                                                        ActiveTransaction.GrossWeight = null;
                                                        ActiveTransaction.IsGrossManual = false;
                                                    }
                                                }



                                                if ((objTranData.TransactionMode == "Single" && objTranData.TransactionDetails.FirstOrDefault().CaptureWeight > 0) || (objTranData.TransactionMode == "Double" && objTranData.TransactionDetails.Where(x => x.CaptureWeight != null && x.CaptureWeight > 0).Count() == 2))
                                                {
                                                    bool CollectToken = Convert.ToBoolean(GlobalSettings.Where(x => x.SettingName == "CollectToken").FirstOrDefault()?.SettingValue);
                                                    if (CollectToken && _objToken.TokenType == "Temporary")
                                                    {
                                                        _objToken.TokenStatus = "Free";
                                                        objTranData.Status = "Complete";
                                                        ctx.SaveChanges();
                                                    }
                                                    else if (_objToken.TokenType == "Permanent")
                                                    {
                                                        objTranData.Status = "Complete";
                                                        ctx.SaveChanges();
                                                    }
                                                    else
                                                    {
                                                        objTrantokenlog.DeallocateTime = DateTime.Now;
                                                        if (_objToken.TokenType == "Temporary" || (_objToken.TokenType == "Permanent" && (objTranData.CreatedBy != null || _objToken.TokenExpireDate < DateTime.Now)))
                                                            _objToken.TokenStatus = "Deallocated";

                                                        ctx.SaveChanges();
                                                    }
                                                }
                                                else
                                                {
                                                    objTranData.Status = "Active";
                                                    ctx.SaveChanges();
                                                }


                                                displayMessage = "Stable Weight of: " + dWeight.ToString() + " Vehicle: " + objTranData.Vehicle.VehicleNumber;
                                                RFIDTokens.Remove(GlobalRFIDToken);
                                                returnData = "DONE";

                                                SaveTransactionImage(ActiveTransaction, objTranData.TicketID);

                                                //objTranData.Weightupdate_Api = false;

                                                NetWeightNegative = false;
                                                Constant.IsWeighSaved = true;

                                                GlobalRFIDToken = string.Empty;
                                                play_weight_done();

                                                try
                                                {

                                                    bool AutoMaticReceipt = Convert.ToBoolean(GlobalSettings.Where(x => x.SettingName == "AutoMaticReceipt").FirstOrDefault()?.SettingValue);

                                                    if (AutoMaticReceipt == true)
                                                    {
                                                        if (ActiveTransaction.NetWeight.HasValue)
                                                        {
                                                            LocalTicketid = Convert.ToString(objTranData.TicketID + ActiveTransaction.SequenceNo);
                                                            ONlyLocalTicketid = objTranData.TicketID;
                                                            ReceiptPreview(objTranData, objTranData.TicketID, ActiveTransaction.NetWeight);
                                                        }
                                                        else
                                                        {
                                                            LocalTicketid = Convert.ToString(objTranData.TicketID + ActiveTransaction.SequenceNo);
                                                            ONlyLocalTicketid = objTranData.TicketID;
                                                            ReceiptPreview(objTranData, objTranData.TicketID, 0);
                                                        }

                                                        if (objTranData.Status == TransactionStatus.Complete.ToString())
                                                        {
                                                            System.ComponentModel.BackgroundWorker backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
                                                            backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
                                                            backgroundWorker1.RunWorkerAsync(10000);
                                                        }
                                                    }


                                                }
                                                catch (Exception ex)
                                                {
                                                    string Error = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                                                    System.IO.File.AppendAllText(Constant.PluginFileLocation + "/ReceiptPreview.txt", Error);
                                                }
                                            }
                                            else
                                            {
                                                displayMessage = ProductErrorMsg;
                                                returnData = "@";
                                                play_weight_not_done();
                                            }
                                        }
                                        else
                                        {
                                            if (ActiveWeighBridge.WeightBypass == true)
                                            {
                                                NetWeightNegative = false;
                                                Constant.IsWeighSaved = true;
                                                displayMessage = ErrorMsg;
                                                GlobalRFIDToken = string.Empty;
                                                play_weight_done();
                                            }
                                            else
                                            {
                                                displayMessage = ErrorMsg;
                                                returnData = "@";
                                                play_weight_not_done();
                                            }


                                        }
                                    }
                                }
                                //}
                            }
                        }
                    }
                    else
                    {
                        displayMessage = "RFIDToken not Found, RFIDToken : " + GlobalRFIDToken;
                        //returnData = "0";
                        returnData = "NOTDONE";
                        // WriteToTheItKitSerial("NOTDONE");

                    }
                }

                viewModel.CycleMSG = displayMessage;
                Constant.Display_Message = displayMessage;

                objTCPLog.ResponseData = returnData + " " + displayMessage;
                ctx.TCPLogs.Add(objTCPLog);
                ctx.SaveChanges();

                try
                {
                    string DeleteQ = "delete from TCPLog where LogTime < GETDATE()-1";
                    DatabaseFunctions.ExecuteQuery(DeleteQ);
                }
                catch { }

                string returnDataStr = Environment.NewLine + "--------------------------------" + DateTime.Now.ToString("dd/MM/yyyy H:mm:ss") + Environment.NewLine + displayMessage + Environment.NewLine + returnData;
                System.IO.File.AppendAllText(Constant.PluginFileLocation + "/UnmaneTestreturnData.txt", returnDataStr);
                //SendDataOnSocket(returnData);
            }
        }
        public decimal StableWt()
        {
            int StableData = 0;

            try
            {
                int MinAllowWeigh = (ActiveWeighBridge.StableWeightCount.GetValueOrDefault() == 0 ? 10 : ActiveWeighBridge.StableWeightCount.Value);

                DateTime dateTime = DateTime.Now.AddSeconds(MinAllowWeigh);
                DateTime EndLoopdateTime = DateTime.Now.AddSeconds(60);

                bool Flag = false;
                List<StableWeigthClass> StableWeigthClasses = new List<StableWeigthClass>();

                while (Flag == false)
                {
                    try
                    {
                        Thread.Sleep(1000);

                        StableWeigthClasses.RemoveAll(p => p.DateTime < DateTime.Now.AddSeconds(-10));

                        int amountString = Convert.ToInt32(Constant.IndicatorWeight);
                        //string FormatedData = Constant.FinelWeight.ToString();

                        try
                        {
                            StableData = Convert.ToInt32(Constant.IndicatorWeight);
                            //FormatedData = FormatedData.Trim();
                            //if (FormatedData.Contains('.'))
                            //    FormatedData = FormatedData.Split('.')[0];

                            //StableData = !string.IsNullOrEmpty(FormatedData) ? Convert.ToDecimal(FormatedData) : 0;
                            System.IO.File.AppendAllText(Constant.PluginFileLocation + $"/StableData_" + DateTime.Now.ToString("yyyyMMdd_HH") + ".txt", Environment.NewLine + DateTime.Now.ToString("dd/MM/yyyy H:mm:ss") + " :-  " + StableData);
                        }
                        catch { }

                        StableWeigthClasses.Add(new StableWeigthClass { DateTime = DateTime.Now, StableWeigth = StableData });

                        if (dateTime < DateTime.Now)
                        {
                            int CommanStableDatahigh = StableWeigthClasses.Where(p => p.StableWeigth != 0).Max(p => p.StableWeigth);
                            int CommanStableDatalow = StableWeigthClasses.Where(p => p.StableWeigth != 0).Min(p => p.StableWeigth);
                            int differ = CommanStableDatahigh - CommanStableDatalow;

                            if (differ > 100 || StableWeigthClasses.Where(p => p.StableWeigth != 0).Count() < 5)
                            {
                                // Not capture
                            }
                            else
                            {
                                if (dateTime < DateTime.Now && StableData != 0)
                                {
                                    return StableData;
                                }
                                // capture                       
                            }

                            if (EndLoopdateTime < DateTime.Now)
                            {
                                return 0;
                            }
                        }
                    }
                    catch
                    {

                    }
                }
                return StableData;

            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Constant.PluginFileLocation + "/StableWt_error.txt", Environment.NewLine + "--------------------------------" + DateTime.Now.ToString("dd/MM/yyyy H:mm:ss") + Environment.NewLine + JsonConvert.SerializeObject(ex));
                return 0;
            }

        }

        public bool TableExists(DbConnection con, string table)
        {
            if (con.State != ConnectionState.Open)
                con.Open();
            var exists = con.GetSchema("Tables", new string[4] { null, null, table, "BASE TABLE" }).Rows.Count > 0;
            con.Close();
            return exists;
        }
        private void SaveTransactionImage(TransactionDetail ActiveTransaction, int TicketId)
        {
            try
            {

                string ImageFolderPath = Path.Combine(Constant.CameraImageLocation, TicketId.ToString());

                if (!Directory.Exists(ImageFolderPath))
                {
                    Directory.CreateDirectory(ImageFolderPath);
                }


                string Datastring = "Data Source=" + Constant.DBSERVER + ";Initial Catalog=" + Constant.DBNAME + ";user id=" + Constant.DBUSERNAME + ";password=" + Constant.DBPASSWORD + ";MultipleActiveResultSets=True;Connection Timeout=30";
                using (SqlConnection con = new SqlConnection(Datastring))
                {
                    if (TableExists(con, "Camera") && TableExists(con, "CameraImage"))
                    {
                        DataTable dtActiveCamera = new DataTable();
                        SqlCommand cmdActiveCamera = new SqlCommand("SELECT * FROM Camera WHERE Active = '1' AND WeightBridgeID = " + ActiveTransaction.WeightBridgeID, con);

                        cmdActiveCamera.CommandType = CommandType.Text;
                        con.Open();
                        dtActiveCamera.Load(cmdActiveCamera.ExecuteReader());
                        con.Close();
                        if (dtActiveCamera != null && dtActiveCamera.Rows.Count > 0)
                        {
                            for (int i = 0; i < dtActiveCamera.Rows.Count; i++)
                            {
                                string fileName = DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + Convert.ToInt32(dtActiveCamera.Rows[i]["CameraId"]) + ".jpg";


                                string UserName = dtActiveCamera.Rows[i]["UserName"].ToString();
                                string Password = dtActiveCamera.Rows[i]["CameraPassword"].ToString();
                                string VideosourceMainURL = dtActiveCamera.Rows[i]["VideosourceMainURL"].ToString();
                                string IpAddress = dtActiveCamera.Rows[i]["IpAddress"].ToString();


                                string VideoURL = string.Empty;
                                if (dtActiveCamera.Rows[i]["CameraType"].ToString() == "RTSPMJPEG")
                                {
                                    //UriBuilder uriBuilder = new UriBuilder(VideosourceMainURL);
                                    //uriBuilder.UserName = UserName;
                                    //uriBuilder.Password = Password;
                                    //VideoURL = uriBuilder.Uri.ToString();
                                    VideoURL = VideosourceMainURL.Replace("rtsp://", $"rtsp://{UserName}:{Password}@");
                                }
                                else
                                {
                                    VideoURL = $"rtsp://{UserName}:{Password}@{IpAddress}:554/Streaming/Channels/101/";
                                }

                                string Argument = "-rtsp_transport tcp -y -i " + VideoURL.ToString() + " -vframes 1 " + ImageFolderPath + "\\" + fileName;

                                // Process Capture Image
                                Process process = new Process();
                                process.StartInfo.FileName = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory) + "\\ffmpeg.exe";
                                process.StartInfo.Arguments = Argument;
                                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                process.StartInfo.UseShellExecute = false;
                                process.StartInfo.RedirectStandardError = true; // capture errors
                                process.Start();

                                if (!process.WaitForExit(6000)) // wait 6s
                                {
                                    process.Kill();
                                }

                                string errorLog = process.StandardError.ReadToEnd();
                                if (!string.IsNullOrEmpty(errorLog))
                                {
                                    try
                                    {
                                        File.AppendAllText(Path.Combine(Constant.PluginFileLocation, "ffmpeg_errors.log"), errorLog + Environment.NewLine);
                                    }
                                    catch { }
                                }

                                //Save Image
                                if (File.Exists(ImageFolderPath + "\\" + fileName))
                                {
                                    SqlCommand cmdCameraImg = new SqlCommand("INSERT INTO CameraImage VALUES(@CameraId,@TransactionDetailID,@ImageLocation ,@IsSelected)", con);
                                    cmdCameraImg.Parameters.Add("@CameraId", SqlDbType.Int).Value = Convert.ToInt32(dtActiveCamera.Rows[i]["CameraId"]);
                                    cmdCameraImg.Parameters.Add("@TransactionDetailID", SqlDbType.Int).Value = ActiveTransaction.TransactionDetailID;
                                    cmdCameraImg.Parameters.Add("@ImageLocation", SqlDbType.NVarChar).Value = fileName;
                                    cmdCameraImg.Parameters.Add("@IsSelected", SqlDbType.Bit).Value = false;
                                    con.Open();
                                    cmdCameraImg.ExecuteNonQuery();
                                    con.Close();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Constant.CustomSettingsPath + "/Trace.txt", ex.StackTrace + Environment.NewLine + "--------------------------------" + DateTime.Now.ToString("dd/MM/yyyy H:mm:ss") + Environment.NewLine);
                //return lstimagePath;
            }
        }

        private string ReceiptPreview(TransactionData objTD, int TicketID, decimal? NetWeight)
        {
            //Only for single and double pass
            try
            {
                DataTable dt = new DataTable();
                string CompanyName = "";
                string DateTimeFormat = "";
                string CameraImagePathRoot = "";
                string CameraImagePath = "";
                string SelectedPrinter = string.Empty;
                using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                {
                    List<ApplicationSetting> lstApplicationsetting = ctx.ApplicationSettings.ToList();
                    objTD = ctx.TransactionDatas.FirstOrDefault(x => x.ReceiptTicketID == objTD.ReceiptTicketID);

                    try
                    {
                        string defaultPrinterName = PrinterSettings.InstalledPrinters.Cast<string>().FirstOrDefault(name => new PrinterSettings { PrinterName = name }.IsDefaultPrinter);
                        SelectedPrinter = defaultPrinterName;

                        System.IO.File.AppendAllText(Constant.PluginFileLocation + "/Print.txt", SelectedPrinter);
                    }
                    catch (Exception ex)
                    {
                        string Error2 = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                        File.AppendAllText(Constant.PluginFileLocation + @"\Printerror.txt", Error2);
                    }

                    if (lstApplicationsetting.Any(x => x.ModuleName == "CameraSetting"))
                    {
                        CameraImagePathRoot = lstApplicationsetting.FirstOrDefault(x => x.ModuleName == "CameraSetting" && x.SettingName == "CameraImagePath").SettingValue;
                        CameraImagePath = Path.Combine(CameraImagePathRoot, objTD.TicketID.ToString());
                    }
                    DateTimeFormat = lstApplicationsetting.FirstOrDefault(x => x.ModuleName == "GlobalSetting" && x.SettingName == "DateFormat").SettingValue + " " + lstApplicationsetting.FirstOrDefault(x => x.ModuleName == "GlobalSetting" && x.SettingName == "TimeFormat").SettingValue;
                }

                try
                {
                    SqlConnection con = new SqlConnection(System.Windows.Application.Current.Properties["DefalutConnectionString"].ToString());

                    /************** Patch for Error  *****************/
                    //https://stackoverflow.com/questions/22422838/report-viewer-2010-issue-on-non-admin-accounts
                    //An error occurred during local report processing. ---> Microsoft.Reporting.DefinitionInvalidException: The definition of the report 'C:\Program Files (x86)\Sinuvera Technologies\WeighBridge Solution\\Report\Receipt\SingleDoublePass_Receipt.rdlc' is invalid. ---> Microsoft.ReportingServices.ReportProcessing.ReportProcessingException: An unexpected error occurred while compiling expressions. Native compiler return value: `[BC2012] can't open 'C:\Users\Admin\AppData\Local\Temp\expression_host_2da15411db254da395d8ec032f529e6e.dll' for writing'.
                    PermissionSet permissions = new PermissionSet(PermissionState.None);
                    permissions.AddPermission(new FileIOPermission(PermissionState.Unrestricted));
                    permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));  // The missing piece
                    ReportViewer reportViewer = new ReportViewer();
                    reportViewer.LocalReport.SetBasePermissionsForSandboxAppDomain(permissions);
                    reportViewer.ProcessingMode = ProcessingMode.Local;
                    reportViewer.ShowExportButton = false;

                    /************ End Patch  ************************/
                    //Summary Report
                    //var parentSQL = "SELECT TData.TicketID,A.CompanyName AS AccountName,TR.CompanyName AS TransaporterName,TData.TransactionMode,(D.FirstName + ' ' + ISNULL(D.LastName, '')) AS DriverName,V.VehicleNumber,Field1DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field1'),Field2DisplayName = (SELECT DisplayName	FROM CustomField WHERE FieldName = 'field2'),Field3DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field3'),Field4DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field4'),Field5DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field5'),Field6DisplayName = (SELECT DisplayName	FROM CustomField WHERE FieldName = 'field6'),Field7DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field7'),Field8DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field8'),Field1Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField	WHERE FieldName = 'field1')	AND TicketID = TData.TicketID),Field2Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field2') AND TicketID = TData.TicketID),Field3Value = (SELECT CustomValue FROM CustomValue	WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field3') AND TicketID = TData.TicketID),Field4Value = (SELECT CustomValue	FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field4') AND TicketID = TData.TicketID),Field5Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField	WHERE FieldName = 'field5')	AND TicketID = TData.TicketID),Field6Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field6') AND TicketID = TData.TicketID),Field7Value = (SELECT CustomValue FROM CustomValue	WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field7') AND TicketID = TData.TicketID),Field8Value = (SELECT CustomValue	FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field8') AND TicketID = TData.TicketID),TareWeight = (SELECT CASE WHEN STATUS = 'Active' AND (MIN(TareWeight) IS NULL	OR MIN(TareWeight) = 0) THEN 0 WHEN TData.TransactionMode = 'Single' THEN MIN(TareWeight) ELSE MIN(CaptureWeight) END FROM TransactionDetail WHERE TicketID = TData.TicketID),GrossWeight = (SELECT CASE WHEN STATUS = 'Active' AND (MIN(GrossWeight) IS NULL OR MIN(GrossWeight) = 0) THEN 0 WHEN TData.TransactionMode = 'Single' THEN MAX(GrossWeight) ELSE Max(CaptureWeight)	END	FROM TransactionDetail WHERE TicketID = TData.TicketID),TareTime = (SELECT CASE WHEN STATUS = 'Active' AND (MIN(TareTime) IS NULL) THEN NULL WHEN TData.TransactionMode = 'Single'	THEN MIN(TareTime) ELSE (Select CaptureTime FROM TransactionDetail Tx WHERE CaptureWeight = (SELECT CASE WHEN STATUS = 'Active' AND (MIN(TareWeight) IS NULL	OR MIN(TareWeight) = 0)	THEN 0 WHEN TData.TransactionMode = 'Single' THEN MIN(TareWeight) ELSE MIN(CaptureWeight) END FROM TransactionDetail WHERE TicketID = TData.TicketID) AND Tx.TicketID = TData.TicketID) END FROM TransactionDetail WHERE TicketID = TData.TicketID),GrossTime = (SELECT CASE WHEN STATUS = 'Active' AND (MIN(GrossTime) IS NULL)	THEN NULL WHEN TData.TransactionMode = 'Single' THEN MAX(GrossTime) ELSE (Select CaptureTime FROM TransactionDetail Tx WHERE CaptureWeight = (SELECT CASE WHEN STATUS = 'Active' AND (MIN(GrossWeight) IS NULL OR MIN(GrossWeight) = 0) THEN 0 WHEN TData.TransactionMode = 'Single' THEN MAX(GrossWeight)	ELSE Max(CaptureWeight) END	FROM TransactionDetail WHERE TicketID = TData.TicketID) AND Tx.TicketID = TData.TicketID)	END	FROM TransactionDetail WHERE TicketID = TData.TicketID),NetWeight = (SELECT CASE WHEN TData.TransactionMode = 'Single' THEN (MAX(GrossWeight) - MIN(TareWeight)) ELSE (MAX(CaptureWeight) - MIN(CaptureWeight)) END FROM TransactionDetail WHERE TicketID = TData.TicketID),WeightUnit = (SELECT MIN(WeightUnit)	FROM TransactionDetail TxD WHERE TxD.TicketID = TData.TicketID) FROM TransactionData TData LEFT JOIN Account A ON A.AccountID = TData.AccountID LEFT JOIN Account TR ON TR.AccountID = TData.TransporterID LEFT JOIN Vehicle V ON V.VehicleID = TData.VehicleID LEFT JOIN Driver D ON D.DriverID = TData.DriverID WHERE TData.TicketID = " + TicketID;
                    var parentSQL = @"SELECT TData.TicketID,
                                         A.CompanyName AS AccountName,TR.CompanyName AS TransaporterName,TData.Charges,TData.TransactionType,TData.TransactionMode,TXD.TareTime, 
                                         TXD.GrossTime, ISNULL(TXD.TareWeight, 0) AS TareWeight, ISNULL(TXD.GrossWeight, 0) AS GrossWeight, ISNULL(TXD.NetWeight, 0) AS NetWeight,
                                         TXD.IsTareManual, TXD.IsGrossmanual, TXD.WeightUnit,P.ProductName,G.GateName,(D.FirstName + ' ' + ISNULL(D.LastName, '')) AS DriverName, V.VehicleNumber,
                                         Field1DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field1'),
                                         Field2DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field2'),
                                         Field3DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field3'),
                                         Field4DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field4'),
                                         Field5DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field5'),
                                         Field6DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field6'),
                                         Field7DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field7'),
                                         Field8DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field8'),
                                         Field9DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field9'),
                                         Field10DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field10'),
                                         Field11DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field11'),
                                         Field12DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field12'),
                                         Field13DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field13'),
                                         Field14DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field14'),
                                         Field15DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field15'),
                                         Field16DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field16'),
                                         Field17DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field17'),
                                         Field18DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field18'),
                                         Field19DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field19'),
                                         Field20DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field20'),
                                         Field1Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN(SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field1') AND ReceiptTicketID = TData.ReceiptTicketID),
                                         Field2Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN(SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field2') AND ReceiptTicketID = TData.ReceiptTicketID),
                                         Field3Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN(SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field3') AND ReceiptTicketID = TData.ReceiptTicketID),
                                         Field4Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN(SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field4') AND ReceiptTicketID = TData.ReceiptTicketID),
                                         Field5Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN(SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field5') AND ReceiptTicketID = TData.ReceiptTicketID),
                                         Field6Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN(SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field6') AND ReceiptTicketID = TData.ReceiptTicketID),
                                         Field7Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN(SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field7') AND ReceiptTicketID = TData.ReceiptTicketID),
                                         Field8Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN(SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field8') AND ReceiptTicketID = TData.ReceiptTicketID),
                                         Field9Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN(SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field9') AND ReceiptTicketID = TData.ReceiptTicketID),
                                         Field10Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN(SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field10') AND ReceiptTicketID = TData.ReceiptTicketID),
                                         Field11Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN(SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field11') AND ReceiptTicketID = TData.ReceiptTicketID),
                                         Field12Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN(SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field12') AND ReceiptTicketID = TData.ReceiptTicketID),
                                         Field13Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN(SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field13') AND ReceiptTicketID = TData.ReceiptTicketID),
                                         Field14Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN(SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field14') AND ReceiptTicketID = TData.ReceiptTicketID) ,
                                         Field15Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN(SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field15') AND ReceiptTicketID = TData.ReceiptTicketID),
                                         Field16Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN(SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field16') AND ReceiptTicketID = TData.ReceiptTicketID) ,
                                         Field17Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN(SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field17') AND ReceiptTicketID = TData.ReceiptTicketID),
                                         Field18Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN(SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field18') AND ReceiptTicketID = TData.ReceiptTicketID),
                                         Field19Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN(SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field19') AND ReceiptTicketID = TData.ReceiptTicketID),
                                         Field20Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN(SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field20') AND ReceiptTicketID = TData.ReceiptTicketID) 
                                         FROM TransactionData TData
                                         LEFT JOIN Account A ON A.AccountID = TData.AccountID
                                         INNER JOIN TransactionDetail TXD ON TData.ReceiptTicketID = TXD.ReceiptTicketID AND TXD.SequenceNo = (select MAX(SequenceNo) from TransactionDetail where ReceiptTicketID = TData.ReceiptTicketID )
                                         LEFT JOIN Product P ON P.ProductID = TXD.ProductID
                                         LEFT JOIN Gate G ON G.GateID = TXD.GateID
                                         LEFT JOIN Account TR ON TR.AccountID = TData.TransporterID
                                         LEFT JOIN Vehicle V ON V.VehicleID = TData.VehicleID
                                         LEFT JOIN Driver D ON D.DriverID = TData.DriverID
                                         WHERE TData.ReceiptTicketID = '" + objTD.ReceiptTicketID + "'";
                    SqlCommand cmd = new SqlCommand(parentSQL, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 45;
                    con.Open();
                    dt.Load(cmd.ExecuteReader());
                    con.Close();

                    DataTable dtCloned = dt.Clone();
                    dtCloned.Columns["GrossWeight"].ReadOnly = false;
                    dtCloned.Columns["GrossWeight"].DataType = typeof(string);
                    dtCloned.Columns["TareWeight"].ReadOnly = false;
                    dtCloned.Columns["TareWeight"].DataType = typeof(string);
                    dtCloned.Columns["NetWeight"].ReadOnly = false;
                    dtCloned.Columns["NetWeight"].DataType = typeof(string);
                    dtCloned.Columns["GrossTime"].ReadOnly = false;
                    dtCloned.Columns["GrossTime"].DataType = typeof(DateTime);
                    dtCloned.Columns["TareTime"].ReadOnly = false;
                    dtCloned.Columns["TareTime"].DataType = typeof(DateTime);
                    foreach (DataRow item in dt.Rows)
                    {
                        dtCloned.ImportRow(item);
                        dtCloned.Rows[dtCloned.Rows.Count - 1]["GrossWeight"] = string.Format("{0: " + Constant.DecimalFormat + "}", item["GrossWeight"]).Trim() + " " + ((Common.ValidatePageRequest(PageEntity.PrintStarForManualEntry, PageAccess.Execute) && Convert.ToBoolean(item["IsGrossManual"]) == true) ? "*" : " ");
                        dtCloned.Rows[dtCloned.Rows.Count - 1]["TareWeight"] = string.Format("{0: " + Constant.DecimalFormat + "}", item["TareWeight"]).Trim() + " " + ((Common.ValidatePageRequest(PageEntity.PrintStarForManualEntry, PageAccess.Execute) && Convert.ToBoolean(item["IsTareManual"]) == true) ? "*" : " ");
                        dtCloned.Rows[dtCloned.Rows.Count - 1]["NetWeight"] = string.Format("{0: " + Constant.DecimalFormat + "}", item["NetWeight"]).Trim();

                        //dtCloned.Rows[dtCloned.Rows.Count - 1]["GrossTime"] = item["GrossTime"] != DBNull.Value ? Convert.ToDateTime(item["GrossTime"])
                        //       .ToString(Constant.DateTimeFormat, CultureInfo.InvariantCulture) : "";

                        //dtCloned.Rows[dtCloned.Rows.Count - 1]["TareTime"] = item["TareTime"] != DBNull.Value ? Convert.ToDateTime(item["TareTime"])
                        //    .ToString(Constant.DateTimeFormat, CultureInfo.InvariantCulture) : "";
                    }

                    ReportDataSource reportDataSource = new ReportDataSource();
                    reportDataSource.Name = "Data"; // Name of the DataSet we set in .rdlc
                    reportDataSource.Value = dtCloned;
                    onlyDataTable = dtCloned;
                    reportViewer.LocalReport.ReportPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Report\\Receipt\\SingleDoublePass_Receipt.rdlc";

                    reportViewer.LocalReport.EnableExternalImages = true;
                    List<ReportParameter> lstParams = new List<ReportParameter>();
                    string LoggedINUserName = string.Empty;
                    if (Application.Current.Properties["LoggedInUserName"] != null)
                    {
                        LoggedINUserName = Application.Current.Properties["LoggedInUserName"].ToString();
                    }
                    lstParams.Add(new ReportParameter("LoggedInUser", LoggedINUserName));
                    lstParams.Add(new ReportParameter("CompanyName", CompanyName));
                    lstParams.Add(new ReportParameter("ExecutionTime", DateTime.Now.ToString(DateTimeFormat)));
                    reportViewer.LocalReport.SetParameters(lstParams);

                    if (TableExists(con, "Camera") && TableExists(con, "CameraImage") && !string.IsNullOrEmpty(CameraImagePath) && Directory.Exists(CameraImagePath))
                    {
                        try
                        {

                            DataTable ImagesDT = new DataTable();
                            var ImagesSQL = "";

                            if (objTD.TransactionMode == "Double")
                                ImagesSQL = "SELECT ISNULL(ImageLocation, '') AS ImageName, C.CameraName, (TD.SequenceNo + 1) As PassNo FROM CameraImage CI INNER JOIN Camera C ON c.CameraID = CI.CameraID INNER JOIN TransactionDetail TD ON TD.TransactionDetailID = CI.TransactionDetailID WHERE CameraImageID IN ( SELECT MAX(CameraImageID) FROM CameraImage CI INNER JOIN TransactionDetail TD ON TD.TransactionDetailID = CI.TransactionDetailID AND TD.ReceiptTicketID = '" + objTD.ReceiptTicketID + "' GROUP BY CameraID, CI.TransactionDetailID)";
                            else
                                ImagesSQL = "SELECT ISNULL(ImageLocation, '') AS ImageName, C.CameraName, TD.SequenceNo As PassNo FROM CameraImage CI INNER JOIN Camera C ON c.CameraID = CI.CameraID INNER JOIN TransactionDetail TD ON TD.TransactionDetailID = CI.TransactionDetailID WHERE CameraImageID IN ( SELECT MAX(CameraImageID) FROM CameraImage CI INNER JOIN TransactionDetail TD ON TD.TransactionDetailID = CI.TransactionDetailID AND TD.ReceiptTicketID = '" + objTD.ReceiptTicketID + "' GROUP BY CameraID, CI.TransactionDetailID)";

                            SqlCommand cmdImages = new SqlCommand(ImagesSQL, con);
                            cmdImages.CommandType = CommandType.Text;
                            cmdImages.CommandTimeout = 45;

                            con.Open();
                            ImagesDT.Load(cmdImages.ExecuteReader());
                            if (ImagesDT != null && ImagesDT.Rows.Count > 0)
                            {
                                foreach (DataColumn col in ImagesDT.Columns)
                                    col.ReadOnly = false;
                                foreach (DataRow item in ImagesDT.Rows)
                                {
                                    item["ImageName"] = "File:\\" + CameraImagePath + "\\" + item["ImageName"];
                                }
                            }
                            con.Close();
                            ReportParameter[] reportParameterCollection = new ReportParameter[16];
                            ReportParameter[] reportParamHasNoImages = new ReportParameter[16];
                            ReportParameter[] reportParamHasNoPasses = new ReportParameter[16];
                            ReportParameter[] reportParameterPasses = new ReportParameter[16];
                            int imagecount = 1;
                            for (int i = 1; i <= 16; i++)
                            {
                                string Embeddsource = string.Empty;
                                string passNo = string.Empty;
                                bool hasNoImage = true;
                                bool hasNoPass = true;
                                if (ImagesDT.Rows.Count >= i && ImagesDT.Rows[i - 1]["ImageName"] != null)
                                {
                                    Embeddsource = ImagesDT.Rows[i - 1]["ImageName"].ToString();
                                    passNo = ImagesDT.Rows[i - 1]["CameraName"].ToString() + " Weighment: #" + ImagesDT.Rows[i - 1]["PassNo"].ToString() + "     ";
                                    hasNoImage = false;
                                    hasNoPass = false;
                                    imagecount++;
                                }
                                reportParameterCollection[i - 1] = new ReportParameter("Image" + i + "source", Embeddsource);
                                reportParamHasNoImages[i - 1] = new ReportParameter("HasNoImage" + i + "source", hasNoImage ? bool.TrueString : bool.FalseString);
                                reportParameterPasses[i - 1] = new ReportParameter("PassNo" + i, passNo);
                                reportParamHasNoPasses[i - 1] = new ReportParameter("HasNoPass" + i + "source", hasNoPass ? bool.TrueString : bool.FalseString);

                            }


                            reportViewer.LocalReport.SetParameters(reportParameterCollection);
                            reportViewer.LocalReport.SetParameters(reportParamHasNoImages);
                            reportViewer.LocalReport.SetParameters(reportParameterPasses);
                            reportViewer.LocalReport.SetParameters(reportParamHasNoPasses);
                        }
                        catch (Exception ex)
                        {
                            string Error2 = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                            File.AppendAllText(Constant.PluginFileLocation + @"\RecieptCameraImageDataError.txt", Error2);
                        }

                    }

                    reportViewer.LocalReport.DataSources.Add(reportDataSource);

                    //TODO: ERROR printer not install
                    ReportPageSettings rps = reportViewer.LocalReport.GetDefaultPageSettings();
                    if (rps.IsLandscape && rps.PaperSize.Height > rps.PaperSize.Width)
                    {
                        System.Drawing.Printing.PageSettings ps = reportViewer.GetPageSettings();
                        //ps.PaperSize = new System.Drawing.Printing.PaperSize(rps.PaperSize.Kind.ToString(), rps.PaperSize.Height, rps.PaperSize.Width);
                        ps.Landscape = false;
                        reportViewer.SetPageSettings(ps);
                    }

                    reportViewer.RefreshReport();
                    reportViewer.Refresh();
                    reportViewerLocal = reportViewer;

                    try
                    {
                        if (NetWeight != 0)
                        {
                            System.Drawing.Printing.PageSettings po = reportViewer.GetPageSettings();
                            string orientationNEW = po.Landscape ? "Landscape" : "portrait";
                            string Sizevalnew = $" Print orientation = {orientationNEW} , Print Height = {po.PaperSize.Height} ,Print Width = {po.PaperSize.Width}";

                            File.AppendAllText(Constant.ApplicationDataLocation + @"\ReportPageSettings.txt", Environment.NewLine + Sizevalnew);

                            var data = GlobalSettings.Any(x => x.SettingName == "AutoMaticReceipt");
                            if (data)
                            {
                                bool AutoMaticReceipt = Convert.ToBoolean(GlobalSettings.Where(x => x.SettingName == "AutoMaticReceipt").FirstOrDefault()?.SettingValue);

                                if (AutoMaticReceipt == true)
                                {
                                    reportViewer.LocalReport.Print(SelectedPrinter, 1);
                                }
                            }
                        }
                    }
                    catch (Exception printEx)
                    {
                        File.AppendAllText(Constant.ApplicationDataLocation + @"\ReportPageSettingsError.txt", Environment.NewLine + JsonConvert.SerializeObject(printEx));
                    }
                    if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\WeighBridgeSolution\\AttachmentFile\\"))
                    {
                        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\WeighBridgeSolution\\AttachmentFile\\");
                    }

                    Warning[] warnings;
                    string[] streamIds;
                    string mimeType = string.Empty;
                    string encoding = string.Empty;
                    string extension = string.Empty;
                    string fileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\WeighBridgeSolution\\AttachmentFile\\TransactionDetail_" + LocalTicketid + ".pdf";
                    byte[] bytes = reportViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                    FileStream fs = new FileStream(fileName, FileMode.Create);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();

                    return fileName;
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Constant.ApplicationDataLocation + @"\ReportPageSettingsError.txt", Environment.NewLine + JsonConvert.SerializeObject(ex));
                    return string.Empty;
                }
            }
            catch (LocalProcessingException lx)
            {
                //https://stackoverflow.com/questions/30310652/reportviewer-2010-io-exception
                //http://alsaydi.blogspot.com/2011/07/report-rendering-fails-in-local-mode.html
                //https://stackoverflow.com/questions/22422838/report-viewer-2010-issue-on-non-admin-accounts
                //App.Log.Error(ex.Message, ex);
                //winform.Visibility = Visibility.Collapsed;
                //((MetroWindow)App.Current.MainWindow).ShowMessageAsync("An error occured", "Unable to open report. Please try again.");
                File.AppendAllText(Constant.ApplicationDataLocation + @"\ReportPageSettingsError.txt", Environment.NewLine + JsonConvert.SerializeObject(lx));
                return string.Empty;
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(2000);
            SendEmail = new Thread(new ThreadStart(Backgroungprocess));
            SendEmail.Start();
        }

        public void Backgroungprocess()
        {

            List<EmailSetup> lstEmailsetup = ctxGlobal.EmailSetups.Where(x => x.EmailType == "Every Transaction" && x.IsEnable).ToList();
            if (lstEmailsetup != null && lstEmailsetup.Count > 0)
            {
                List<ApplicationSetting> EmailSetting = ctxGlobal.ApplicationSettings.Where(x => x.ModuleName == "EmailSetting").ToList();
                if (EmailSetting != null && EmailSetting.Count > 0)
                {
                    foreach (var item in lstEmailsetup)
                    {
                        try
                        {
                            SendMail(EmailSetting, item, item.EmailTo);
                        }
                        catch (Exception ex)
                        {
                            Common.logErrorInDB(ex.Message, ex);
                        }
                    }
                }
            }
        }

        private bool SendMail(List<ApplicationSetting> EmailSetting, EmailSetup item, string Mailto)
        {
            try
            {
                TransactionData transactionData = ctxGlobal.TransactionDatas.Where(x => x.TicketID == ONlyLocalTicketid).FirstOrDefault();
                Email objEmail = new Email();
                objEmail.SMTPServer = EmailSetting.FirstOrDefault(x => x.SettingName == "ServerName").SettingValue;
                int port = Convert.ToInt32(EmailSetting.FirstOrDefault(x => x.SettingName == "Port").SettingValue);
                objEmail.SMTPUserName = (EmailSetting.FirstOrDefault(x => x.SettingName == "UserName").SettingValue != "") ? EmailSetting.FirstOrDefault(x => x.SettingName == "UserName").SettingValue : "";
                objEmail.SMTPPassword = (EmailSetting.FirstOrDefault(x => x.SettingName == "PassWord").SettingValue != "") ? Common.DecryptData(EmailSetting.FirstOrDefault(x => x.SettingName == "PassWord").SettingValue) : "";
                objEmail.EmailFrom = (EmailSetting.FirstOrDefault(x => x.SettingName == "EmailFrom").SettingValue != "") ? EmailSetting.FirstOrDefault(x => x.SettingName == "EmailFrom").SettingValue : "no-reply@weighingsolution.com";
                objEmail.Subject = item.EmailSubject;
                objEmail.EmailAddress = Mailto;
                objEmail.Body = item.EmailBody + Common.GetmailBody(transactionData, item, onlyDataTable);
                if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\WeighBridgeSolution\\AttachmentFile\\"))
                {
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\WeighBridgeSolution\\AttachmentFile\\");
                }

                if (item.ColumnSelection.Contains("IsAttachmentFile"))
                {
                    Warning[] warnings;
                    string[] streamIds;
                    string mimeType = string.Empty;
                    string encoding = string.Empty;
                    string extension = string.Empty;
                    string fileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\WeighBridgeSolution\\AttachmentFile\\TransactionDetail_" + LocalTicketid + ".pdf";
                    if (!File.Exists(fileName))
                    {
                        byte[] bytes = reportViewerLocal.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                        FileStream fs = new FileStream(fileName, FileMode.Create);
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Close();
                    }
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        objEmail.Attachments = new Dictionary<int, string>();
                        objEmail.Attachments.Add(0, fileName);
                    }
                }
                return objEmail.SendHtmlEmail(port, Convert.ToBoolean(EmailSetting.FirstOrDefault(x => x.SettingName == "UseSSL").SettingValue));
            }
            catch (Exception ex)
            {
                Common.logErrorInDB(ex.Message, ex);
                return false;
            }
        }
        #region Sound

        public void play_card_ok()
        {
            try
            {
                System.ComponentModel.BackgroundWorker backgroundWorker = new System.ComponentModel.BackgroundWorker();
                backgroundWorker.DoWork += (object sender1, System.ComponentModel.DoWorkEventArgs DE) =>
                {
                    Thread.Sleep(500);
                    string Audio1 = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Audio\\01_card_ok.wav";
                    if (File.Exists(Audio1))
                    {
                        SoundPlayer my_wave_Audio1 = new SoundPlayer(Audio1);
                        my_wave_Audio1.PlaySync();
                    }
                };
                backgroundWorker.RunWorkerAsync(500);

            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Constant.PluginFileLocation + "/Audio_play_error.txt", Environment.NewLine + "--------------------------------" + DateTime.Now.ToString("dd/MM/yyyy H:mm:ss") + Environment.NewLine + JsonConvert.SerializeObject(ex));
            }
        }

        //public bool Is_play_card_not_ok_Play = false;
        public void play_card_not_ok()
        {

            try
            {
                UHFDataClasses.RemoveAll(p => p.ReadTime < DateTime.Now.AddSeconds(-1));
                //if (Is_play_card_not_ok_Play)
                //    return;
                System.ComponentModel.BackgroundWorker backgroundWorker = new System.ComponentModel.BackgroundWorker();
                backgroundWorker.DoWork += (object sender1, System.ComponentModel.DoWorkEventArgs DE) =>
                {
                    //Is_play_card_not_ok_Play = true;
                    Thread.Sleep(500);

                    string Audio1 = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Audio\\02_card_not_ok.wav";
                    if (File.Exists(Audio1))
                    {
                        SoundPlayer my_wave_Audio1 = new SoundPlayer(Audio1);
                        my_wave_Audio1.PlaySync();
                    }
                    //Is_play_card_not_ok_Play = false;

                };
                backgroundWorker.RunWorkerAsync(500);

            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Constant.PluginFileLocation + "/Audio_play_error.txt", Environment.NewLine + "--------------------------------" + DateTime.Now.ToString("dd/MM/yyyy H:mm:ss") + Environment.NewLine + JsonConvert.SerializeObject(ex));
            }

        }

        public void play_vehicle_not_in_postion()
        {

            try
            {
                System.ComponentModel.BackgroundWorker backgroundWorker = new System.ComponentModel.BackgroundWorker();
                backgroundWorker.DoWork += (object sender1, System.ComponentModel.DoWorkEventArgs DE) =>
                {
                    Thread.Sleep(500);
                    string Audio1 = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Audio\\03_vehicle_not_in_postion.wav";
                    if (File.Exists(Audio1))
                    {
                        SoundPlayer my_wave_Audio1 = new SoundPlayer(Audio1);
                        my_wave_Audio1.PlaySync();
                    }

                };
                backgroundWorker.RunWorkerAsync(500);

            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Constant.PluginFileLocation + "/Audio_play_error.txt", Environment.NewLine + "--------------------------------" + DateTime.Now.ToString("dd/MM/yyyy H:mm:ss") + Environment.NewLine + JsonConvert.SerializeObject(ex));
            }

        }

        public void play_weight_start()
        {

            try
            {
                System.ComponentModel.BackgroundWorker backgroundWorker = new System.ComponentModel.BackgroundWorker();
                backgroundWorker.DoWork += (object sender1, System.ComponentModel.DoWorkEventArgs DE) =>
                {
                    Thread.Sleep(500);
                    for (int i = 0; i < 2; i++)
                    {
                        string Audio1 = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Audio\\04_weight_start.wav";
                        if (File.Exists(Audio1))
                        {
                            SoundPlayer my_wave_Audio1 = new SoundPlayer(Audio1);
                            my_wave_Audio1.PlaySync();
                        }

                        Thread.Sleep(500);
                    }

                };
                backgroundWorker.RunWorkerAsync(500);

            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Constant.PluginFileLocation + "/Audio_play_error.txt", Environment.NewLine + "--------------------------------" + DateTime.Now.ToString("dd/MM/yyyy H:mm:ss") + Environment.NewLine + JsonConvert.SerializeObject(ex));
            }

        }

        public void play_weight_done()
        {

            try
            {
                System.ComponentModel.BackgroundWorker backgroundWorker = new System.ComponentModel.BackgroundWorker();
                backgroundWorker.DoWork += (object sender1, System.ComponentModel.DoWorkEventArgs DE) =>
                {
                    Thread.Sleep(500);
                    for (int i = 0; i < 2; i++)
                    {
                        string Audio1 = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Audio\\05_weight_done.wav";
                        if (File.Exists(Audio1))
                        {
                            SoundPlayer my_wave_Audio1 = new SoundPlayer(Audio1);
                            my_wave_Audio1.PlaySync();
                        }
                        Thread.Sleep(500);
                    }
                };
                backgroundWorker.RunWorkerAsync(500);

            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Constant.PluginFileLocation + "/Audio_play_error.txt", Environment.NewLine + "--------------------------------" + DateTime.Now.ToString("dd/MM/yyyy H:mm:ss") + Environment.NewLine + JsonConvert.SerializeObject(ex));
            }

        }

        public void play_weight_not_done()
        {
            try
            {
                System.ComponentModel.BackgroundWorker backgroundWorker = new System.ComponentModel.BackgroundWorker();
                backgroundWorker.DoWork += (object sender1, System.ComponentModel.DoWorkEventArgs DE) =>
                {
                    Thread.Sleep(500);
                    for (int i = 0; i < 3; i++)
                    {
                        string Audio1 = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Audio\\06_weight_not_done.wav";
                        if (File.Exists(Audio1))
                        {
                            SoundPlayer my_wave_Audio1 = new SoundPlayer(Audio1);
                            my_wave_Audio1.PlaySync();
                        }
                        Thread.Sleep(500);
                    }

                };
                backgroundWorker.RunWorkerAsync(500);

            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Constant.PluginFileLocation + "/Audio_play_error.txt", Environment.NewLine + "--------------------------------" + DateTime.Now.ToString("dd/MM/yyyy H:mm:ss") + Environment.NewLine + JsonConvert.SerializeObject(ex));
            }


        }

        public void play_vehicle_number_wrong()
        {
            try
            {
                System.ComponentModel.BackgroundWorker backgroundWorker = new System.ComponentModel.BackgroundWorker();
                backgroundWorker.DoWork += (object sender1, System.ComponentModel.DoWorkEventArgs DE) =>
                {
                    Thread.Sleep(500);
                    string Audio1 = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Audio\\07_vehicle_number_wrong.wav";
                    if (File.Exists(Audio1))
                    {
                        SoundPlayer my_wave_Audio1 = new SoundPlayer(Audio1);
                        my_wave_Audio1.PlaySync();
                    }

                };
                backgroundWorker.RunWorkerAsync(500);

            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Constant.PluginFileLocation + "/Audio_play_error.txt", Environment.NewLine + "--------------------------------" + DateTime.Now.ToString("dd/MM/yyyy H:mm:ss") + Environment.NewLine + JsonConvert.SerializeObject(ex));
            }
        }
        public void play_send_for_approval()
        {
            try
            {
                System.ComponentModel.BackgroundWorker backgroundWorker = new System.ComponentModel.BackgroundWorker();
                backgroundWorker.DoWork += (object sender1, System.ComponentModel.DoWorkEventArgs DE) =>
                {
                    Thread.Sleep(500);
                    string Audio1 = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Audio\\10_transaction_send_for_approval.wav";
                    if (File.Exists(Audio1))
                    {
                        SoundPlayer my_wave_Audio1 = new SoundPlayer(Audio1);
                        my_wave_Audio1.PlaySync();
                    }

                };
                backgroundWorker.RunWorkerAsync(500);

            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Constant.PluginFileLocation + "/Audio_play_error.txt", Environment.NewLine + "--------------------------------" + DateTime.Now.ToString("dd/MM/yyyy H:mm:ss") + Environment.NewLine + JsonConvert.SerializeObject(ex));
            }

        }
        public void play_vehicle_come()
        {

            try
            {
                System.ComponentModel.BackgroundWorker backgroundWorker = new System.ComponentModel.BackgroundWorker();
                backgroundWorker.DoWork += (object sender1, System.ComponentModel.DoWorkEventArgs DE) =>
                {
                    Thread.Sleep(500);
                    string Audio1 = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Audio\\08_vehicle_come.wav";
                    if (File.Exists(Audio1))
                    {
                        SoundPlayer my_wave_Audio1 = new SoundPlayer(Audio1);
                        my_wave_Audio1.PlaySync();
                    }

                };
                backgroundWorker.RunWorkerAsync(500);

            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Constant.PluginFileLocation + "/Audio_play_error.txt", Environment.NewLine + "--------------------------------" + DateTime.Now.ToString("dd/MM/yyyy H:mm:ss") + Environment.NewLine + JsonConvert.SerializeObject(ex));
            }

        }

        private void btnResetTransaction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessClientData_Executing = false;
                UHFDataClasses.RemoveAll(p => p.ReadTime < DateTime.Now.AddSeconds(-1));
                viewModel.CycleMSG = "";
                if (modbusHelper.PLCPort.IsOpen)
                {
                    Constant.ISManual = false;
                    Constant.Final_Reset = true;
                    Constant.Final_end = false;
                    Constant.IsUnmanCycle_end = false;
                    Constant.IsWeighSaved = false;
                    Constant.IsWeighStart = false;
                    Constant.IsFirstSensorOn = false;
                    Constant.IsSecoundSensorOn = false;
                    Constant.IsTrirdSensorOn = false;
                    Constant.IsForthSensorOn = false;
                    Constant.WBSIDE = "";
                    Constant.WBSIDE_DASHBORD = "";
                    Constant.Display_Message = string.Empty;
                    Constant.Display_Vehicle = string.Empty;
                    Constant.Color_Vehicle = string.Empty;
                    Constant.DisplayRFID_Left = string.Empty;
                    Constant.DisplayRFID_Right = string.Empty;
                    NetWeightNegative = false;
                    modbusHelper.Ideal();

                    Uri uri = new Uri("/View/TransactionView/TransactionManagementList_Page.xaml", UriKind.Relative);
                    MainFrame.NavigationService.Navigate(uri);


                }


            }
            catch
            {

            }



        }

        #endregion Sound

        #region JumboDisplay
        public void WeightToJumbo()
        {
            while (true)
            {
                Thread.Sleep(1000);
                try
                {
                    if (!string.IsNullOrEmpty(ActiveWeighBridge.JumboDisplay_Port) && !string.IsNullOrEmpty(ActiveWeighBridge.JumboDisplay_BaudRate))
                    {

                        string weight = Constant.IndicatorWeight.ToString().PadLeft(6, '0');

                        string data = weight == "000000" ? "$WT:00000" : $"$WT:{weight}";

                        SerialPort e = new SerialPort(ActiveWeighBridge.JumboDisplay_Port, Convert.ToInt32(ActiveWeighBridge.JumboDisplay_BaudRate), Parity.None, 8, StopBits.One);
                        e.Handshake = Handshake.None;
                        e.Open();
                        e.WriteLine(data);
                        e.Close();
                    }


                }
                catch (Exception ex)
                {
                    _ = ex.Message;
                }
            }
        }
        #endregion JumboDisplay

        #endregion modbusLogiv


    }

    public class MatchInfo
    {
        public int MatchPer { get; set; }
        public string MatchResult { get; set; }
    }

    public class CSVResult
    {
        public int RecordCount { get; set; }
        public string CSVString { get; set; }
    }
    public class Anpr_Number_Request
    {
        public int CameraId { get; set; }
        public string Rtsp_url { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string InputImagePath { get; set; }
    }

    public class Anpr_Number_Response
    {
        public bool Success { get; set; }
        public string VehicleNumber { get; set; }
        public string PlateImage_Base64 { get; set; }
        public string Error { get; set; }
    }


    public class UHFDataClass
    {
        public string Tag { get; set; }
        public string UHFSide { get; set; }
        public int Count { get; set; }
        public DateTime ReadTime { get; set; }
    }

    public class Token_Model
    {
        public string VehicleNumber { get; set; }
        public string TokenValue { get; set; }
    }


    public class UhfTcpListener : IDisposable
    {
        private TcpClient _client;
        private Thread _listenerThread;
        private bool _running;

        public string IP { get; private set; }
        public int Port { get; private set; }
        public string UHFType { get; private set; }

        public event Action<string> OnTagReceived;
        public event Action<bool> OnConnectionChanged;

        public UhfTcpListener(string ip, int port, string UHF_Type)
        {
            IP = ip;
            Port = port;
            UHFType = UHF_Type;
        }

        public void Start()
        {
            _running = true;
            _listenerThread = new Thread(ListenLoop);
            _listenerThread.IsBackground = true;
            _listenerThread.Start();
        }

        public void Stop()
        {
            _running = false;

            try
            {
                _client?.Close();
            }
            catch { }
        }

        private void ListenLoop()
        {
            while (_running)
            {
                try
                {
                    if (_client == null || !_client.Connected)
                    {
                        Connect();
                    }

                    if (_client == null)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    NetworkStream stream = _client.GetStream();
                    stream.ReadTimeout = 5000;

                    byte[] buffer = new byte[2000];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                    {
                        string temps = "";

                        if (UHFType == "HAX")
                        {
                            StringBuilder sb = new StringBuilder(bytesRead * 2);
                            for (int i = 0; i < bytesRead; i++)
                            {
                                sb.Append(buffer[i].ToString("X2"));
                            }
                            temps = sb.ToString();
                        }
                        else
                        {
                            temps = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        }
                        OnTagReceived?.Invoke(temps);
                    }
                }
                catch
                {
                    Disconnect();
                    OnTagReceived?.Invoke("");
                    Thread.Sleep(3000);
                }
            }
        }

        private void Connect()
        {
            try
            {
                var client = new TcpClient();
                client.Connect(IP, Port);

                _client = client;

                OnConnectionChanged?.Invoke(true);
            }
            catch (Exception ex)
            {
                _client = null;

                OnConnectionChanged?.Invoke(false);
                Thread.Sleep(5000);
            }
        }

        private void Disconnect()
        {
            try
            {
                _client?.Close();
            }
            catch { }

            _client = null;

            OnConnectionChanged?.Invoke(false);
        }

        public void Dispose()
        {
            Stop();
        }
    }

}