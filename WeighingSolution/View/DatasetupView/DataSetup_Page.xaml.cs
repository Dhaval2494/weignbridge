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
    /// Interaction logic for MainHeader.xaml
    /// </summary>
    public partial class DataSetup_Page : Page
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
                gridDatasetup.RowDefinitions.Add(rd);
                RowCount++;
            }
        }

        public DataSetup_Page()
        {
            InitializeComponent();
            if (!Constant.IsBasicversion)
            {
                if (Common.ValidatePageRequest(PageEntity.UserRole, PageAccess.Read))
                {
                    SetRowColumn(gbTemplate);
                }
                if (Common.ValidatePageRequest(PageEntity.User, PageAccess.Read))
                {
                    SetRowColumn(gbUser);
                }
            }
            if (Common.ValidatePageRequest(PageEntity.Unit, PageAccess.Read))
            {
                SetRowColumn(gbUnit);
            }
            if (Common.ValidatePageRequest(PageEntity.WeighBridge, PageAccess.Read))
            {
                SetRowColumn(gbWeighBridge);
            }
            if (Common.ValidatePageRequest(PageEntity.Account, PageAccess.Read))
            {
                SetRowColumn(gbAccount);
            }
            if (Common.ValidatePageRequest(PageEntity.Vehicle, PageAccess.Read))
            {
                SetRowColumn(gbVehicle);
            }
            if (Common.ValidatePageRequest(PageEntity.Driver, PageAccess.Read))
            {
                SetRowColumn(gbDriver);
            }
            if (Common.ValidatePageRequest(PageEntity.Product, PageAccess.Read))
            {
                SetRowColumn(gbProduct);
            }
            if (Common.ValidatePageRequest(PageEntity.Gate, PageAccess.Read))
            {
                SetRowColumn(gbGate);
            }
            bool IsCameraPlugin = LicensePluginHalper.CheckPluginInstall(PluginNameEnum.CameraPlugin.ToString());
            if (Common.ValidatePageRequest(PageEntity.Camera, PageAccess.Read) && IsCameraPlugin)
            {
                SetRowColumn(gbCameraTile);
            }

            bool IsUnmannedPlugin = Constant.hasUnmannedPlugin;

            if (Common.ValidatePageRequest(PageEntity.UHF, PageAccess.Read) && IsUnmannedPlugin)
            {
                SetRowColumn(gbUhf);
            }
            if (Common.ValidatePageRequest(PageEntity.Token, PageAccess.Read) && IsUnmannedPlugin)
            {
                SetRowColumn(gbTokenTile);
            }
            //Socket to Plugins            
            try
            {
                PluginLoader<IPluginControl> mef = new PluginLoader<IPluginControl>("DataSetup");
                foreach (var plugin in mef.loadedPlugins.Keys)
                {
                    plugin.SetupPlugin(this);
                }
            }
            catch (Exception Ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(Ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\DataSetup_PageError.txt", json);
                //exec sp_updatestats
            }
        }

        private void BtnGoToDriverManagement_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/DatasetupView/DriverManagementList_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        private void BtnGoToProductManagement_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/DatasetupView/ProductManagementList_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        private void BtnGoToGateManagement_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/DatasetupView/GateManagementList_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        private void btnGoToUserManagementList_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/DatasetupView/UserManagementList_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        private void BtnGoToAccountManagementList_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/DatasetupView/AccountManagementList_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        private void BtnGoToWeighBridgeManagement_Click(object sender, RoutedEventArgs e)
        {
            bool IsMultiWeighBridgePlugin = LicensePluginHalper.CheckPluginInstall(PluginNameEnum.MultiWeighBridgePlugin.ToString());

            if (IsMultiWeighBridgePlugin)
            {
                Uri uri = new Uri("/View/DatasetupView/WeighBridgeManagementList_Page.xaml", UriKind.Relative);
                this.NavigationService.Navigate(uri);
            }
            else
            {
                Uri uri = new Uri("/View/DatasetupView/WeighBridgeManagement_Page.Xaml", UriKind.Relative);
                this.NavigationService.Navigate(uri);
            }
        }

        private void BtnGoToVehicleManagement_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/DatasetupView/VehicleManagementList_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        private void BtnGoToUnit_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/DatasetupView/UnitManagementList_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        private void BtnGoToTemplateManagement_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/DatasetupView/TemplateMangementList_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        private void BtnGoToCameraManagement_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/DatasetupView/CameraManagementList.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        private void BtnGoToTokenManagement_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/DatasetupView/TokenManagementList.xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        private void BtnGoToUHFManagement_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/DatasetupView/UHFManagementList_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
    }
}

