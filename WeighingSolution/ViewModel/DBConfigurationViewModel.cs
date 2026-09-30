using Microsoft.Win32;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;

namespace WeighingSolution.ViewModel
{
    public class DBConfigurationViewModel : NotifyDataErrorInfoBase
    {
        public DelegateCommand CreateDB { get; private set; }
        DatabaseFunctions DBFunctions;
        public DBConfigurationViewModel()
        {
            DBFunctions = new DatabaseFunctions();
            CreateDB = new DelegateCommand(OnCreateDB, OnCanCreateDB);
            LoadAvailableSqlServices();
            this.PropertyChanged -= DBConfigurationViewModel_PropertyChanged;
            this.PropertyChanged += DBConfigurationViewModel_PropertyChanged;
        }

        private void LoadAvailableSqlServices()
        {
            AvailableSqlServices = new List<string>();

            //1. Try to fetch from registry.
            RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            {
                RegistryKey instanceKey = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL", false);
                if (instanceKey != null)
                {
                    foreach (var instanceName in instanceKey.GetValueNames())
                    {
                        AvailableSqlServices.Add(Environment.MachineName + @"\" + instanceName);
                    }
                }
            }
            //2. If that fails, get the name from services.
            if (AvailableSqlServices == null || AvailableSqlServices.Count == 0)
            {
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.UseShellExecute = false;
                    startInfo.RedirectStandardOutput = true;
                    startInfo.CreateNoWindow = true;
                    startInfo.WorkingDirectory = Environment.CurrentDirectory;
                    startInfo.FileName = "powershell.exe";
                    startInfo.Arguments = "-command \"Get-Service | Where-Object {$_.DisplayName -like 'SQL Server (*'} | Select-Object -Property DisplayName | ft -HideTableHeaders\"";
                    Process proc = Process.Start(startInfo);
                    proc.WaitForExit((int)TimeSpan.FromSeconds(10).TotalMilliseconds);

                    //split on newlines
                    string[] servers = proc.StandardOutput.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var srv in servers)
                    {
                        var match = System.Text.RegularExpressions.Regex.Match(srv, @"SQL Server \((.*)\)");
                        if (match.Success && match.Groups.Count > 1)
                        {
                            AvailableSqlServices.Add(Environment.MachineName + @"\" + match.Groups[1].Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    App.Log.Error(ex.Message, ex);
                }
            }
        }

        void DBConfigurationViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CreateDB.RaiseCanExecuteChanged();
        }

        private bool OnCanCreateDB()
        {
            //return !this.HasErrors && this.GetErrors("SelectedSQLService") == null;
            if ((ServerType == "Local Server" && String.IsNullOrEmpty(SelectedSQLService))
                || (ServerType == "Network Server" && String.IsNullOrEmpty(NetworkAddress)))
                return false;
            else if (CustomCredentials && (String.IsNullOrEmpty(SqlUserID) || String.IsNullOrEmpty(SqlPassword)))
                return false;
            else
                return true;
        }
        private async void OnCreateDB()
        {
            string sqlUser = DatabaseFunctions.defaultDBUser;
            string sqlPWD = DatabaseFunctions.defaultDBSaPWD;
            string sqlService = DatabaseFunctions.GetSMO(ServerType == "Local Server" ? SelectedSQLService : NetworkAddress);
            bool connectionEstablished = true;
            string ErrorMessage = string.Empty;

            //Default Service (SVTWBSqlExpress) is selected, connection must work with our default "sa" user and password.
            if (DBFunctions.GetDBServiceName(sqlService).ToLower() == DBFunctions.GetDBServiceName(DatabaseFunctions.defaultDBServer).ToLower())
            {
                try
                {
                    SqlConnection con = new SqlConnection("Data Source=" + sqlService + ";Initial Catalog=master;user id=" + sqlUser + ";password=" + sqlPWD + ";");
                    con.Open();
                    if (con.State != ConnectionState.Open)
                    {
                        ErrorMessage = "Unable to connect DB service, please contact support team.";
                        connectionEstablished = false;
                    }
                }
                catch (Exception ex)
                {
                    Common.logErrorInDB(ex.Message, ex);
                    ErrorMessage = "Unable to connect DB service, please contact support team.";
                    connectionEstablished = false;
                }
            }
            //Any other service, try creating Custom User (svtsa) and connect using that.
            else if (!CustomCredentials)
            {
                try
                {
                    SqlConnection con = new SqlConnection("Data Source=" + sqlService + ";Initial Catalog=master;user id=" + sqlUser + ";password=" + sqlPWD + ";");
                    con.Open();
                    if (con.State != ConnectionState.Open)
                    {
                        ErrorMessage = "Can not connect to SQL Server, please provide login credential.";
                        connectionEstablished = false;
                        CustomCredentials = true;
                    }
                    else
                    {
                        if (!DBFunctions.CreateCustomUser(con))
                        {
                            ErrorMessage = "Can not connect to SQL Server, please provide login credential.";
                            connectionEstablished = false;
                            CustomCredentials = true;
                        }
                        else
                        {
                            sqlUser = DatabaseFunctions.customDBUser;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Common.logErrorInDB(ex.Message, ex);
                    ErrorMessage = "Can not connect to SQL Server, please provide login credential.";
                    CustomCredentials = true;
                    connectionEstablished = false;
                }
            }
            //Connect using the user provided credentials.
            else
            {
                try
                {
                    sqlUser = SqlUserID;
                    sqlPWD = SqlPassword;
                    SqlConnection con = new SqlConnection("Data Source=" + sqlService + ";Initial Catalog=master;user id=" + sqlUser + ";password=" + sqlPWD + ";");
                    con.Open();
                    if (con.State != ConnectionState.Open)
                    {
                        ErrorMessage = "Unable to connect SQL Server, please enter valid login credential.";
                        connectionEstablished = false;
                    }
                }
                catch (Exception ex)
                {
                    Common.logErrorInDB(ex.Message, ex);
                    ErrorMessage = "Unable to connect SQL Server, please enter valid login credential.";
                    connectionEstablished = false;
                }
            }

            if (connectionEstablished)
            {
                if (DBFunctions.IsDefaultDBExists(sqlService, 0, DatabaseFunctions.defaultDBName, sqlUser, sqlPWD))
                {
                    try
                    {
                        DBSettingData objNewDBSetting = new DBSettingData();
                        objNewDBSetting.Server = sqlService;
                        objNewDBSetting.DBName = DatabaseFunctions.defaultDBName;
                        objNewDBSetting.Port = 0;
                        objNewDBSetting.UserId = sqlUser;
                        objNewDBSetting.password = sqlPWD;
                        string Data = Newtonsoft.Json.JsonConvert.SerializeObject(objNewDBSetting);
                        System.IO.File.WriteAllText(Constant.CustomSettingsPath + @"\Configuration.dat", Common.EncryptData(Data));
                        Common.SetFullAccessForFile(Constant.CustomSettingsPath + @"\Configuration.dat");
                        await MessageBoxHelper.ShowInfoDialogAsync("Settings Saved", "Settings saved successfully.\nThe application will now restart");
                        App.RestartApplication();
                        return;
                    }
                    catch (Exception ex)
                    {
                        Common.logErrorInDB(ex.Message, ex);
                        ErrorMessage = "Unable to save configuration, please contact support team.";
                    }
                }
                else if (DBFunctions.CreateDefaultDB(sqlService, 0, DatabaseFunctions.defaultDBName, sqlUser, sqlPWD))
                {
                    if (DBFunctions.SetSQLSchema(sqlService, 0, DatabaseFunctions.defaultDBName, sqlUser, sqlPWD))
                    {
                        if (DBFunctions.InsertDefaultData(sqlService, 0, DatabaseFunctions.defaultDBName, sqlUser, sqlPWD))
                        {
                            try
                            {
                                DBSettingData objNewDBSetting = new DBSettingData();
                                objNewDBSetting.Server = sqlService;
                                objNewDBSetting.DBName = DatabaseFunctions.defaultDBName;
                                objNewDBSetting.Port = 0;
                                objNewDBSetting.UserId = sqlUser;
                                objNewDBSetting.password = sqlPWD;
                                string Data = Newtonsoft.Json.JsonConvert.SerializeObject(objNewDBSetting);
                                System.IO.File.WriteAllText(Constant.CustomSettingsPath + @"\Configuration.dat", Common.EncryptData(Data));
                                Common.SetFullAccessForFile(Constant.CustomSettingsPath + @"\Configuration.dat");
                                await MessageBoxHelper.ShowInfoDialogAsync("New Database Created", "Settings saved successfully.\nThe application will now restart");
                                App.RestartApplication();
                                return;
                            }
                            catch (Exception ex)
                            {
                                Common.logErrorInDB(ex.Message, ex);
                                ErrorMessage = "Unable to save configuration, please contact support team.";
                            }
                        }
                        else { ErrorMessage = "Unable to set default value, please contact support team."; }
                    }
                    else { ErrorMessage = "Unable to create database schema, please contact support team."; }
                }
                else { ErrorMessage = "Unable to create database, please contact support team."; }
            }

            if (!String.IsNullOrEmpty(ErrorMessage))
            {
                await MessageBoxHelper.ShowInfoDialogAsync("Error", ErrorMessage);
            }
        }

        #region Model Properties
        private bool _customcredentials;
        public bool CustomCredentials
        {
            get { return _customcredentials; }
            set { _customcredentials = value; OnPropertyChanged(); }
        }

        private List<string> _AvilableServer = new List<string>() { "Local Server", "Network Server" };
        public List<string> AvilableServer
        {
            get { return _AvilableServer; }
        }

        private string _serverType = "Local Server";
        public string ServerType
        {
            get { return _serverType; }
            set { _serverType = value; OnPropertyChanged(); }
        }

        private string _networkAddress;
        public string NetworkAddress
        {
            get { return _networkAddress; }
            set { _networkAddress = value; OnPropertyChanged(); }
        }

        private string _sqlUserID;
        public string SqlUserID
        {
            get { return _sqlUserID; }
            set { _sqlUserID = value; OnPropertyChanged(); }
        }

        private string _sqlPassword;
        public string SqlPassword
        {
            get { return _sqlPassword; }
            set { _sqlPassword = value; OnPropertyChanged(); }
        }

        private string _selectedSQLService;
        public string SelectedSQLService
        {
            get { return _selectedSQLService; }
            set { _selectedSQLService = value; OnPropertyChanged(); }
        }

        private List<string> _availableSqlServices;
        public List<string> AvailableSqlServices
        {
            get { return _availableSqlServices; }
            set { _availableSqlServices = value; OnPropertyChanged(); }
        }
        #endregion
    }
}