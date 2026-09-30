using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;

namespace WeighingSolution.ViewModel
{
    public class vmDataBaseSetting : NotifyDataErrorInfoBase, IDisposable
    {
        private DataRepository dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));
        private Page parentpage;
        public DBSettingData DbSettingModel = new DBSettingData();
        private string strBackupFileName = null;
        private string strRestoreFileName = null;
        private ObservableCollection<ApplicationSetting> DatabaseSetting;

        #region CTOR
        public vmDataBaseSetting(Page page)
        {
            parentpage = page;
            SaveSettingCommand = new DelegateCommand(OnSettingSaveExecute, OnSettingSaveCanExecute);
            CloseDetailViewCommand = new DelegateCommand<string>(OnCloseDetailViewExecute);
            BackupFileLocationCommand = new DelegateCommand(OnBackupFileLocationCommand);
            BackUpDBCommand = new DelegateCommand(OnBackUpDBSaveExecute, OnBackUpDBSaveCanExecute);
            RestoreFileLocationCommand = new DelegateCommand(OnRestoreFileLocationCommand);
            RestoreDBCommand = new DelegateCommand(RestoreDBSaveExecute, RestoreDBSaveCaneExecute);
            SelectFile = new DelegateCommand<String>(OnSelectFile);
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DatabaseSetting = new ObservableCollection<ApplicationSetting>(dataRepository.Context.ApplicationSettings.Where(x => x.ModuleName == "DatabaseSetting").ToList());
            lblFileLocation = "Choose File to backup database to...";
            lblRestoreFileLocation = "Select File to restore database from.";
            try
            {
                if (File.Exists(Constant.CustomSettingsPath + @"\Configuration.dat"))
                {
                    string Data = File.ReadAllText(Constant.CustomSettingsPath + @"\Configuration.dat");
                    if (!string.IsNullOrEmpty(Data))
                    {
                        string decryptedData = Common.DecryptData(Data);
                        if (!string.IsNullOrEmpty(decryptedData))
                        {
                            DbSettingModel = Newtonsoft.Json.JsonConvert.DeserializeObject<DBSettingData>(decryptedData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.logErrorInDB(ex.Message, ex);
            }
        }
        #endregion

        #region Command and Properties Declaration
        public DelegateCommand<string> CloseDetailViewCommand { get; set; }
        public DelegateCommand SaveSettingCommand { get; private set; }
        public DelegateCommand BackupFileLocationCommand { get; private set; }
        public DelegateCommand BackUpDBCommand { get; private set; }
        public DelegateCommand RestoreFileLocationCommand { get; private set; }
        public DelegateCommand RestoreDBCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand<String> SelectFile { get; private set; }

        private bool _hasChanges;
        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                if (_hasChanges != value)
                {
                    _hasChanges = value;
                    OnPropertyChanged();
                    ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private string _lblFileLocation;
        public string lblFileLocation
        {
            get { return _lblFileLocation; }
            set
            {
                _lblFileLocation = value;
                OnPropertyChanged();
                ((DelegateCommand)BackUpDBCommand).RaiseCanExecuteChanged();
            }
        }
        private string _lblRestoreFileLocation;
        public string lblRestoreFileLocation
        {
            get { return _lblRestoreFileLocation; }
            set
            {
                _lblRestoreFileLocation = value;
                OnPropertyChanged();
                ((DelegateCommand)RestoreDBCommand).RaiseCanExecuteChanged();
            }
        }

        private bool _hasChangesAutoBackup;
        public bool HasChangesAutoBackup
        {
            get { return _hasChangesAutoBackup; }
            set
            {
                if (_hasChangesAutoBackup != value)
                {
                    _hasChangesAutoBackup = value;
                    OnPropertyChanged();
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public bool AutoBackupEnable
        {
            get { return Convert.ToBoolean(DatabaseSetting.FirstOrDefault(x => x.SettingName == "AutoBackupEnable").SettingValue); }
            set { DatabaseSetting.FirstOrDefault(x => x.SettingName == "AutoBackupEnable").SettingValue = value.ToString(); OnPropertyChanged(); ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }

        public string AutoBackupHour
        {
            get { return DatabaseSetting.FirstOrDefault(x => x.SettingName == "AutoBackupHour").SettingValue; }
            set { DatabaseSetting.FirstOrDefault(x => x.SettingName == "AutoBackupHour").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }

        public string AutoBackupMinute
        {
            get { return DatabaseSetting.FirstOrDefault(x => x.SettingName == "AutoBackupMinute").SettingValue; }
            set { DatabaseSetting.FirstOrDefault(x => x.SettingName == "AutoBackupMinute").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }

        public string AutoBackupMeridiem
        {
            get { return DatabaseSetting.FirstOrDefault(x => x.SettingName == "AutoBackupMeridiem").SettingValue; }
            set { DatabaseSetting.FirstOrDefault(x => x.SettingName == "AutoBackupMeridiem").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }

        [Required(ErrorMessage = "Auto backup file path is required")]
        public string AutoBackupFilePath
        {
            get { return DatabaseSetting.FirstOrDefault(x => x.SettingName == "AutoBackupFilePath").SettingValue; }
            set { DatabaseSetting.FirstOrDefault(x => x.SettingName == "AutoBackupFilePath").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged(); ValidatePropertyInternal("AutoBackupFilePath", value); ModelPropertyChanged(); }
        }

        #endregion

        #region Binding Property
        [Required(ErrorMessage = "Server Name is required")]
        public string Server
        {
            get { return DbSettingModel.Server; }
            set
            {
                if (DbSettingModel.Server != value)
                {
                    DbSettingModel.Server = value;
                    HasChanges = true;
                }
                OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ValidatePropertyInternal("Server", value);
            }
        }
        [Required(ErrorMessage = "DBName is required")]
        public string DBName
        {
            get { return DbSettingModel.DBName; }
            set
            {
                if (DbSettingModel.DBName != value)
                {
                    DbSettingModel.DBName = value;
                    HasChanges = true;
                }; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ValidatePropertyInternal("DBName", value);
            }
        }
        [Required(ErrorMessage = "User name is required")]
        public string UserId
        {
            get { return DbSettingModel.UserId; }
            set
            {
                if (DbSettingModel.UserId != value)
                {
                    DbSettingModel.UserId = value;
                    HasChanges = true;
                }
                OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ValidatePropertyInternal("UserId", value);
            }
        }
        [Required(ErrorMessage = "Password is required")]
        public string password
        {
            get { return DbSettingModel.password; }
            set
            {
                if (DbSettingModel.password != value)
                {
                    DbSettingModel.password = value;
                    HasChanges = true;
                }
                OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ValidatePropertyInternal("password", value);
            }
        }
        public int Port
        {
            get { return DbSettingModel.Port; }
            set
            {
                if (DbSettingModel.Port != value)
                {
                    DbSettingModel.Port = value;
                    HasChanges = true;
                }
                OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ValidatePropertyInternal("Port", value);
            }
        }

        #endregion

        #region Save
        private bool OnSettingSaveCanExecute()
        {
            if (HasChanges)
                return true;
            else
                return false;
        }
        private async void OnSettingSaveExecute()
        {

            SqlConnection conn = new SqlConnection();
            bool ConnectionEstablished = true;
            try
            {
                string ConnectionString = "";
                if (DbSettingModel.Port > 0)
                {
                    ConnectionString = "Data Source=" + DbSettingModel.Server + "," + DbSettingModel.Port + ";Initial Catalog=" + DbSettingModel.DBName + ";user id=" + DbSettingModel.UserId + ";password=" + DbSettingModel.password + ";";
                    conn.ConnectionString = ConnectionString;
                }
                else
                {
                    ConnectionString = "Data Source=" + DbSettingModel.Server + ";Initial Catalog=" + DbSettingModel.DBName + ";user id=" + DbSettingModel.UserId + ";password=" + DbSettingModel.password + ";";
                    conn.ConnectionString = ConnectionString;
                }
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    Application.Current.Properties["DefalutConnectionString"] = ConnectionString;
                    EntityConnectionStringBuilder entityStringBuilder = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["weighbridgeEntities"].ConnectionString);
                    entityStringBuilder.ProviderConnectionString = ConnectionString;
                    Application.Current.Properties["EntityConnectionString"] = entityStringBuilder.ToString();
                    conn.Close();
                    using (weighbridgeEntities ctx = new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()))
                    {
                        if (ctx.UserMasters.FirstOrDefault() != null)
                        {
                            string Data = Newtonsoft.Json.JsonConvert.SerializeObject(DbSettingModel);
                            File.WriteAllText(Constant.CustomSettingsPath + @"\Configuration.dat", Common.EncryptData(Data));
                            Common.SetFullAccessForFile(Constant.CustomSettingsPath + @"\Configuration.dat");
                        }
                        else
                        {
                            await MessageBoxHelper.ShowInfoDialogAsync("Error", "Unable to connect server. ");
                            return;
                        }
                    }
                }

            }
            catch
            {
                ConnectionEstablished = false;
                if (conn.State == ConnectionState.Open)
                    conn.Close();

            }
            if (!ConnectionEstablished)
                await MessageBoxHelper.ShowInfoDialogAsync("Error", "Unable to connect server. ");
            else
            {
                await dataRepository.SaveAsync();
                //ToastHelper.ShowSuccess("Database information recorded successfully");
                //Uri uri = new Uri("/View/SettingView/Setting_Page.Xaml", UriKind.Relative);
                //parentpage.NavigationService.Navigate(uri);
                var msgboxsettings = new MetroDialogSettings { AffirmativeButtonText = "OK" };
                msgboxsettings.DefaultButtonFocus = MessageDialogResult.Affirmative;
                MessageDialogResult msgboxresult = await ((MetroWindow)App.Current.MainWindow).ShowMessageAsync("Success", "Database information recorded successfully. An application restart is required to use fresh database. The application will now close.", MessageDialogStyle.Affirmative, msgboxsettings);
                App.Current.Shutdown();
            }
        }
        #endregion

        #region Backup DB
        private void OnBackupFileLocationCommand()
        {
            System.Windows.Forms.SaveFileDialog fileDialog = new System.Windows.Forms.SaveFileDialog { Filter = "Backup Files(.bak)|*.bak" };
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && !string.IsNullOrEmpty(fileDialog.FileName))
            {
                lblFileLocation = "Click \"Backup Database\" to begin backup at: " + fileDialog.FileName;
                strBackupFileName = fileDialog.FileName;
            }
        }
        private bool OnBackUpDBSaveCanExecute()
        {
            if (!string.IsNullOrWhiteSpace(strBackupFileName))
                return true;
            else
                return false;
        }
        private async void OnBackUpDBSaveExecute()
        {
            if (!string.IsNullOrEmpty(strBackupFileName))
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                try
                {
                    if (DbSettingModel != null)
                    {
                        using (SqlConnection tmpConn = new SqlConnection("server=" + DbSettingModel.Server + (DbSettingModel.Port > 0 ? "," + DbSettingModel.Port.ToString() : "") + ";user id=" + DbSettingModel.UserId + ";password=" + DbSettingModel.password + ";"))
                        {
                            tmpConn.Open();
                            //GET Sqlserver Backup Directory
                            SqlCommand filepathcmd = new SqlCommand("USE " + DbSettingModel.DBName + ";select dbo.fn_SQLServerBackupDir()", tmpConn);
                            string BackupFilepathRemote = filepathcmd.ExecuteScalar().ToString();
                            BackupFilepathRemote += "\\" + DbSettingModel.DBName + ".bak";
                            //SQL Backup Command, To Backup at Remote Machine
                            string sqlCheckDBQuery = string.Format("BACKUP DATABASE " + DbSettingModel.DBName + " TO DISK='" + BackupFilepathRemote + "' WITH FORMAT");
                            SqlCommand sqlCmd = new SqlCommand(sqlCheckDBQuery, tmpConn);
                            sqlCmd.ExecuteNonQuery();
                            Mouse.OverrideCursor = null;
                            //Get Backup file Content into string
                            byte[] BackupData = dataRepository.Context.Database.SqlQuery<byte[]>("SELECT * FROM OPENROWSET(BULK N'" + BackupFilepathRemote + "', SINGLE_BLOB) AS Contents").FirstOrDefault();
                            File.WriteAllBytes(strBackupFileName, BackupData);
                            await MessageBoxHelper.ShowInfoDialogAsync("", "DataBase Backup saved Successfully.");
                            strBackupFileName = null;
                            lblFileLocation = "Choose File to backup database to...";
                            tmpConn.Close();
                        }
                    }

                }
                catch (Exception ex)
                {
                    Common.logErrorInDB(ex.Message, ex);
                }
            }
            else
                ToastHelper.ShowError("You must select a location to save the backup file.");
        }

        public static void BackUpDBSave(string BackupFileName)
        {
            if (!string.IsNullOrEmpty(BackupFileName))
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                try
                {
                    DBSettingData DbSettingModel = new DBSettingData();
                    DataRepository dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));

                    try
                    {
                        if (File.Exists(Constant.CustomSettingsPath + @"\Configuration.dat"))
                        {
                            string Data = File.ReadAllText(Constant.CustomSettingsPath + @"\Configuration.dat");
                            if (!string.IsNullOrEmpty(Data))
                            {
                                string decryptedData = Common.DecryptData(Data);
                                if (!string.IsNullOrEmpty(decryptedData))
                                {
                                    DbSettingModel = Newtonsoft.Json.JsonConvert.DeserializeObject<DBSettingData>(decryptedData);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Common.logErrorInDB(ex.Message, ex);
                    }

                    if (DbSettingModel != null)
                    {
                        using (SqlConnection tmpConn = new SqlConnection("server=" + DbSettingModel.Server + (DbSettingModel.Port > 0 ? "," + DbSettingModel.Port.ToString() : "") + ";user id=" + DbSettingModel.UserId + ";password=" + DbSettingModel.password + ";"))
                        {
                            tmpConn.Open();
                            //GET Sqlserver Backup Directory
                            SqlCommand filepathcmd = new SqlCommand("USE " + DbSettingModel.DBName + ";select dbo.fn_SQLServerBackupDir()", tmpConn);
                            string BackupFilepathRemote = filepathcmd.ExecuteScalar().ToString();
                            BackupFilepathRemote += "\\" + DbSettingModel.DBName + ".bak";
                            //SQL Backup Command, To Backup at Remote Machine
                            string sqlCheckDBQuery = string.Format("BACKUP DATABASE " + DbSettingModel.DBName + " TO DISK='" + BackupFilepathRemote + "' WITH FORMAT");
                            SqlCommand sqlCmd = new SqlCommand(sqlCheckDBQuery, tmpConn);
                            sqlCmd.ExecuteNonQuery();
                            Mouse.OverrideCursor = null;
                            //Get Backup file Content into string
                            byte[] BackupData = dataRepository.Context.Database.SqlQuery<byte[]>("SELECT * FROM OPENROWSET(BULK N'" + BackupFilepathRemote + "', SINGLE_BLOB) AS Contents").FirstOrDefault();
                            File.WriteAllBytes(BackupFileName, BackupData);

                            BackupFileName = null;
                            tmpConn.Close();
                        }
                    }

                }
                catch (Exception ex)
                {

                }
            }
        }

        public static void TakeDBBackup()
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(Constant.CustomSettingsPath + @"\DB_BACKUP\");
            if (di.Exists)
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            else
            {
                Common.CreateDirectoryWithFullAccess(Constant.CustomSettingsPath + @"\DB_BACKUP\");
            }

            string BackupFileName = Constant.CustomSettingsPath + @"\DB_BACKUP\WB_Backup_" + DateTime.Now.ToString("dd_MMM_yyyy_hh_mm_ss") + ".bak";

            BackUpDBSave(BackupFileName);
        }

        private void OnSelectFile(string obj)
        {
            using (System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (!string.IsNullOrEmpty(dialog.SelectedPath))
                    AutoBackupFilePath = dialog.SelectedPath;
            }
        }
        private bool OnSaveCanExecute()
        {
            return HasChangesAutoBackup && !HasErrors;
        }

        private async void OnSaveExecute()
        {
            if (string.IsNullOrWhiteSpace(AutoBackupFilePath))
            {
                ToastHelper.ShowError("Please Choose folder location for auto backup");
                return;
            }
            else if (!Directory.Exists(AutoBackupFilePath))
            {
                ToastHelper.ShowError("Auto backup file path is not exists");
                return;
            }
            try
            {
                if (AutoBackupMinute == null)
                    AutoBackupMinute = "00";

                if (AutoBackupHour != null && AutoBackupMeridiem != null)
                {
                    string BackupAction = "Disable";
                    if (AutoBackupEnable)
                        BackupAction = "Enabled";

                    string time = AutoBackupHour + ":" + AutoBackupMinute + ":00 " + AutoBackupMeridiem;
                    string URL = "http://localhost:9865?BackupAction=" + BackupAction + "&BackupSchedule=" + time;
                    //Create http client to call web service
                    using (System.Net.WebClient wc = new System.Net.WebClient())
                    {
                        wc.Headers[System.Net.HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        string HtmlResult = wc.UploadString(new Uri(URL), "");
                    }
                }
            }
            catch (Exception)
            {

            }
            await dataRepository.SaveAsync();
            await MessageBoxHelper.ShowInfoDialogAsync("", "Auto backup detail saved Successfully.");
            HasChangesAutoBackup = false;
        }

        private void ModelPropertyChanged()
        {
            if (!HasChangesAutoBackup)
                HasChangesAutoBackup = dataRepository.HasChanges();
        }
        #endregion

        #region Restore DB
        private bool RestoreDBSaveCaneExecute()
        {
            if (!string.IsNullOrWhiteSpace(strRestoreFileName))
                return true;
            else
                return false;
        }
        private void OnRestoreFileLocationCommand()
        {
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog { Filter = "Backup Files(.bak)|*.bak" };
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && !string.IsNullOrEmpty(fileDialog.FileName))
            {
                lblRestoreFileLocation = "Click \"Restore Database\" to begin restore from: " + fileDialog.FileName;
                strRestoreFileName = fileDialog.FileName;
            }
        }
        private async void RestoreDBSaveExecute()
        {
            bool isError = false;
            try
            {
                if (!string.IsNullOrEmpty(strRestoreFileName))
                {
                    byte[] BackupData = File.ReadAllBytes(strRestoreFileName);
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                    if (DbSettingModel != null)
                    {
                        using (SqlConnection tmpConn = new SqlConnection("server=" + DbSettingModel.Server + (DbSettingModel.Port > 0 ? "," + DbSettingModel.Port.ToString() : "") + ";user id=" + DbSettingModel.UserId + ";password=" + DbSettingModel.password + ";"))
                        {
                            tmpConn.Open();
                            //Configuration
                            SqlCommand Configure1cmd = new SqlCommand("sp_configure 'show advanced options', 1 ; RECONFIGURE;", tmpConn);
                            Configure1cmd.ExecuteNonQuery();
                            SqlCommand Configure2cmd = new SqlCommand("sp_configure 'Ole Automation Procedures', 1 ; RECONFIGURE; ", tmpConn);
                            Configure2cmd.ExecuteNonQuery();
                            //USE DataBase
                            SqlCommand UseDBcmd = new SqlCommand("USE " + DbSettingModel.DBName, tmpConn);
                            UseDBcmd.ExecuteNonQuery();
                            //SP for Byte Data store in Remote server
                            SqlCommand Filepathcmd = new SqlCommand("WriteFile", tmpConn);
                            Filepathcmd.CommandType = CommandType.StoredProcedure;
                            Filepathcmd.Parameters.Add(new SqlParameter("@ByteData", BackupData));
                            Filepathcmd.Parameters.Add(new SqlParameter("@FileName", DbSettingModel.DBName));
                            Filepathcmd.ExecuteNonQuery();

                            SqlCommand fileDirectrycmd = new SqlCommand("SELECT dbo.fn_SQLServerBackupDir()", tmpConn);
                            string BackupFilepathRemote = fileDirectrycmd.ExecuteScalar().ToString() + "\\" + DbSettingModel.DBName + ".bak";
                            string Logfilepath = dataRepository.Context.Database.SqlQuery<string>("SELECT Physical_Name AS Location FROM sys.master_files mf INNER JOIN sys.databases db ON db.database_id = mf.database_id WHERE db.name = '" + DbSettingModel.DBName + "' and type_desc = 'LOG'").FirstOrDefault();
                            string Mdffilepath = dataRepository.Context.Database.SqlQuery<string>("SELECT Physical_Name AS Location FROM sys.master_files mf INNER JOIN sys.databases db ON db.database_id = mf.database_id WHERE db.name = '" + DbSettingModel.DBName + "' and type_desc = 'ROWS'").FirstOrDefault();
                            string restoresql = "RESTORE DATABASE [" + DbSettingModel.DBName + "] FROM  DISK = N'" + BackupFilepathRemote + "' WITH  FILE = 1,  MOVE N'" + DbSettingModel.DBName + "' TO N'" + Mdffilepath + "',  MOVE N'" + DbSettingModel.DBName + "_log' TO N'" + Logfilepath + "',  NOUNLOAD,  REPLACE,  STATS = 5";
                            SqlCommand sqlRestorecmd = new SqlCommand("USE master; ALTER DATABASE " + DbSettingModel.DBName + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE; " + restoresql + " ; ALTER DATABASE " + DbSettingModel.DBName + " SET MULTI_USER;", tmpConn);
                            sqlRestorecmd.ExecuteNonQuery();

                            tmpConn.Close();
                            Mouse.OverrideCursor = null;
                            await MessageBoxHelper.ShowInfoDialogAsync("", "DataBase Restored Successfully.");
                            MainWindow mwWindow = ((MainWindow)App.Current.MainWindow);
                            Login loginpage = new Login();
                            loginpage.Show();
                            mwWindow.Close();
                        }
                    }
                }
                else
                    ToastHelper.ShowError("You must select file to restore database.");
            }
            catch (Exception ex)
            {
                Common.logErrorInDB(ex.Message, ex);
                Mouse.OverrideCursor = null;
                isError = true;
            }
            if (isError)
            {
                await MessageBoxHelper.ShowInfoDialogAsync("Error", "Somthing went wrong, please contact support.");
            }
        }
        #endregion

        #region Method
        public void Dispose()
        {
            dataRepository.Context.Dispose();
        }
        protected async virtual void OnCloseDetailViewExecute(string NavigateURL)
        {
            if (HasChanges)
            {
                var result = await MessageBoxHelper.ShowOkCancelDialogAsync("Are you sure?", "You've made changes. Close this item?");
                if (result == MessageDialogResult.Negative)
                {
                    return;
                }
            }
            Uri uri = new Uri(NavigateURL, UriKind.Relative);
            parentpage.NavigationService.Navigate(uri);
        }
        #endregion
    }
}
