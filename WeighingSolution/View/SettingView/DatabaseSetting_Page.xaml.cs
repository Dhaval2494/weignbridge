using log4net;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using Newtonsoft.Json;
using PluginInterfaces;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;
using WeighingSolution.UserControls;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for DatabaseSetting_Page.xaml
    /// </summary>
    public partial class DatabaseSetting_Page : Page
    {
        vmDataBaseSetting viewModel;
        public static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public PluginLoader<IPluginControl> mef = new PluginLoader<IPluginControl>("DatabaseSetting_Page");
        public DatabaseSetting_Page()
        {
            InitializeComponent();
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            viewModel = new vmDataBaseSetting(this);
            this.DataContext = viewModel;

            try
            {
                bool IsSyncPlugin = LicensePluginHalper.CheckPluginInstall(PluginNameEnum.SyncPlugin.ToString());

                if (IsSyncPlugin)
                {
                    SyncSettingsPanel.Visibility = Visibility.Visible;
                }
                foreach (var plugin in mef.loadedPlugins.Keys)
                {
                    plugin.SetupPlugin(this);
                }
            }
            catch (Exception Ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(Ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\DatabaseSetting_PageError.txt", json);
                //exec sp_updatestats
            }
            stkServiceStarted.Visibility = Visibility.Visible;
        }

        private void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (Common.ValidatePageRequest(PageEntity.DatabaseSetting, PageAccess.Update))
                DatabaseSettingSave.Visibility = Visibility.Visible;
        }

        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var plugin in mef.loadedPlugins.Keys)
                {
                    plugin.DisposePlugin();
                }
            }
            catch (Exception Ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(Ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\DatabaseSetting_PageError.txt", json);
                //exec sp_updatestats
            }
            viewModel.Dispose();
        }


        #region Delete TransactionData
        private async void DeleteTransactionDataCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var msgbox_settings = new MetroDialogSettings { AffirmativeButtonText = "Yes With reset Ticket No", FirstAuxiliaryButtonText = "Yas Without reset Ticket No ", NegativeButtonText = "NO" };
            msgbox_settings.DefaultButtonFocus = MessageDialogResult.Negative;

            MessageDialogResult msgbox_result = await ((MetroWindow)App.Current.MainWindow).ShowMessageAsync("Warning", "This will delete all your transaction data, it is highly recommended you backup your database first.\nAre you really really sure you want to continue?", MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, msgbox_settings);
            if (msgbox_result == MessageDialogResult.Affirmative)
            {
                try
                {
                    ResetTransaction(true);
                }
                catch (Exception ex)
                {
                    ToastHelper.ShowError("Error occured deleting the database: " + ex.Message);
                    App.Log.Error(ex.Message, ex);
                }
                Mouse.OverrideCursor = null;
            }
            if (msgbox_result == MessageDialogResult.FirstAuxiliary)
            {
                try
                {
                    ResetTransaction(false);
                }
                catch (Exception ex)
                {
                    ToastHelper.ShowError("Error occured deleting the database: " + ex.Message);
                    App.Log.Error(ex.Message, ex);
                }
                Mouse.OverrideCursor = null;
            }
        }

        public async void ResetTransaction(bool IsResetTicket = false)
        {
            using (var conn = new SqlConnection(Application.Current.Properties["DefalutConnectionString"].ToString()))
            {
                Mouse.OverrideCursor = Cursors.Wait;
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    using (SqlCommand cmdResetProc = new SqlCommand("ResetTransaction", conn))
                    {
                        cmdResetProc.CommandType = CommandType.StoredProcedure;
                        cmdResetProc.ExecuteNonQuery();
                        System.IO.DirectoryInfo di = new DirectoryInfo(Constant.CustomImagesPath + @"\TransactionImages\");

                        if (di.Exists)
                        {
                            foreach (FileInfo file in di.GetFiles())
                            {
                                file.Delete();
                            }
                        }
                    }
                    if (IsResetTicket)
                    {
                        using (SqlCommand cmdResetProc = new SqlCommand("update ApplicationSettings set SettingValue ='0' where ModuleName = 'Transaction' and SettingName = 'LastTicket'", conn))
                        {
                            cmdResetProc.CommandType = CommandType.Text;
                            cmdResetProc.ExecuteNonQuery();
                        }
                    }
                    conn.Close();
                }
                Mouse.OverrideCursor = null;
                var msgboxsettings = new MetroDialogSettings { AffirmativeButtonText = "OK" };
                msgboxsettings.DefaultButtonFocus = MessageDialogResult.Affirmative;
                MessageDialogResult msgboxresult = await ((MetroWindow)App.Current.MainWindow).ShowMessageAsync("Success", "Database deleted successfully. An application restart is required to use fresh database. The application will now close.", MessageDialogStyle.Affirmative, msgboxsettings);
                App.Current.Shutdown();
            }
        }
        #endregion

        private void btnSyncServerSetting_Click(object sender, RoutedEventArgs e)
        {
            SyncDBSetting childWindow = new SyncDBSetting();
            ChildWindowManager.ShowChildWindowAsync(Application.Current.MainWindow, childWindow);
        }
    }
}
