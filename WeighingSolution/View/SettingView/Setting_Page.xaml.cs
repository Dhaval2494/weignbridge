using Newtonsoft.Json;
using PluginInterfaces;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for Setting_Page.xaml
    /// </summary>
    public partial class Setting_Page : Page
    {
        int RowCount = 0;
        int ColumnCount = 0;
        private void SetRowColumn(UIElement cntrl)
        {
            cntrl.Visibility = Visibility.Visible;
            Grid.SetColumn(cntrl, ColumnCount);
            Grid.SetRow(cntrl, RowCount);
            ColumnCount++;
            if (ColumnCount % 4 == 0)
            {
                ColumnCount = 0;
                RowDefinition rd = new RowDefinition();
                rd.Height = new GridLength(1, GridUnitType.Star);
                gridSettings.RowDefinitions.Add(rd);
                RowCount++;
            }
        }
        public Setting_Page()
        {
            InitializeComponent();

            bool IsSMSPlugin = LicensePluginHalper.CheckPluginInstall(PluginNameEnum.SMSPlugin.ToString());
            if (Common.ValidatePageRequest(PageEntity.DatabaseSetting, PageAccess.Read))
            {
                SetRowColumn(gbDatabaseSetting);
            }
            if (Common.ValidatePageRequest(PageEntity.GlobalSetting, PageAccess.Read))
            {
                SetRowColumn(gbGlobalSetting);
                SetRowColumn(gbReceiptManagement);

            }
            //If super admin
            if (Constant.IsAdmin || Constant.IsSupportUser)
            {
                SetRowColumn(gbCompanySetting);
                SetRowColumn(gbCertificateSetting);
            }
            else
            {
                if (Common.ValidatePageRequest(PageEntity.CompanySetting, PageAccess.Read))
                {
                    SetRowColumn(gbCompanySetting);
                }
            }

            if (Common.ValidatePageRequest(PageEntity.EmailSetup, PageAccess.Read))
            {
                SetRowColumn(gbEmailSetting);
            }
            if (Common.ValidatePageRequest(PageEntity.SMSSetup, PageAccess.Read) && IsSMSPlugin)
            {
                SetRowColumn(gbSMSSetting);
            }

            if (Common.ValidatePageRequest(PageEntity.API, PageAccess.Read) && Constant.IsSupportUser)
            {
                SetRowColumn(gbAPIManagement);
            }

            bool IsBarcodePlugin = LicensePluginHalper.CheckPluginInstall(PluginNameEnum.BarcodePlugin.ToString());
            if (Common.ValidatePageRequest(PageEntity.BarcodeSetup, PageAccess.Read) && IsBarcodePlugin)
            {
                SetRowColumn(gbBarcodeTile);
            }

            //if (Common.ValidatePageRequest(PageEntity.PrintSetting, PageAccess.Read))
            //{
            //    SetRowColumn(gbPrintSetting);
            //}
            if (!Constant.IsBasicversion)
            {
                if (Common.ValidatePageRequest(PageEntity.FieldSetting, PageAccess.Read))
                {
                    SetRowColumn(gbFieldSetting);
                }
                if (Common.ValidatePageRequest(PageEntity.PluginManagement, PageAccess.Read))
                {
                    SetRowColumn(gbPluginSetting);
                }
                SetRowColumn(gbAppTheme);
            }
            if (Common.ValidatePageRequest(PageEntity.LicenseManagement, PageAccess.Read))
            {
                SetRowColumn(gbLicenseManagement);
            }

            try
            {
                PluginLoader<IPluginControl> mef = new PluginLoader<IPluginControl>("Setting_Page");
                foreach (var plugin in mef.loadedPlugins.Keys)
                {
                    plugin.SetupPlugin(this);
                }
            }
            catch (Exception Ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(Ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\Setting_PageError.txt", json);
                //exec sp_updatestats
            }

        }

        private void btnGoToDatabaseSetting_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/SettingView/DatabaseSetting_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        //private void btnGoToPrintSetting_Click(object sender, RoutedEventArgs e)
        //{
        //    Uri uri = new Uri("/View/SettingView/PrintSetting_Page.Xaml", UriKind.Relative);
        //    this.NavigationService.Navigate(uri);
        //}

        private void BtnGoToAPI_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/SettingView/ApiIntegrationMaster_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        private void btnGoToGlobalSetting_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/SettingView/GlobelSetting_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        private void btnGoToCompanySetting_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/SettingView/CompanySetting_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        private void BtnGoToFieldSetting_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/SettingView/CustomFieldsList_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        private void BtnGoToPluginSetting_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/SettingView/PluginManagement_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        private void BtnGoToAppTheme_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/SettingView/ApplicationTheme_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        private void btnGoTORenewLicense_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/SettingView/LicenseManagement_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        private void btnGoToEmailSetting_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/SettingView/EmailScheduleList_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        private void btnGoToSMSSetting_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/SettingView/SMSScheduleList_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        private void BtnGoToCertificateSetting_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/SettingView/StampCertificateList_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        private void btnGoToBarcodeSetting_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/SettingView/BarcodePage.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        private void BtnGoToReceipt_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/SettingView/ReceiptSetting_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

    }
}
