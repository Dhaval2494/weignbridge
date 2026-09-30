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
    /// Interaction logic for ReportView_Page.xaml
    /// </summary>
    public partial class ReportView_Page : Page
    {
        public ReportView_Page()
        {
            InitializeComponent();
            bool selected = false;
            if (!Constant.IsBasicversion)
            {
                if (Common.ValidatePageRequest(PageEntity.UserRoleReport, PageAccess.Read))
                {
                    if (!selected)
                    {
                        tabUserRolereport.IsSelected = true;
                        selected = true;
                    }
                    tabUserRolereport.Visibility = Visibility.Visible;
                }
                if (Common.ValidatePageRequest(PageEntity.UserReport, PageAccess.Read))
                {
                    if (!selected)
                    {
                        tabUserreport.IsSelected = true;
                        selected = true;
                    }
                    tabUserreport.Visibility = Visibility.Visible;
                }
            }
            if (Common.ValidatePageRequest(PageEntity.UnitReport, PageAccess.Read))
            {
                if (!selected)
                {
                    tabUnitreport.IsSelected = true;
                    selected = true;
                }
                tabUnitreport.Visibility = Visibility.Visible;
            }
            if (Common.ValidatePageRequest(PageEntity.AccountReport, PageAccess.Read))
            {
                tabAcntreport.Visibility = Visibility.Visible;
                if (!selected)
                {
                    tabAcntreport.IsSelected = true;
                    selected = true;
                }
            }
            if (Common.ValidatePageRequest(PageEntity.VehicleReport, PageAccess.Read))
            {
                if (!selected)
                {
                    tabVehiclereport.IsSelected = true;
                    selected = true;
                }
                tabVehiclereport.Visibility = Visibility.Visible;
            }
            if (Common.ValidatePageRequest(PageEntity.DriverReport, PageAccess.Read))
            {
                if (!selected)
                {
                    tabDriverreport.IsSelected = true;
                    selected = true;
                }
                tabDriverreport.Visibility = Visibility.Visible;
            }
            if (Common.ValidatePageRequest(PageEntity.ProductReport, PageAccess.Read))
            {
                if (!selected)
                {
                    tabProductreport.IsSelected = true;
                    selected = true;
                }
                tabProductreport.Visibility = Visibility.Visible;
            }
            if (Common.ValidatePageRequest(PageEntity.GateReport, PageAccess.Read))
            {
                if (!selected)
                {
                    tabGatereport.IsSelected = true;
                    selected = true;
                }
                tabGatereport.Visibility = Visibility.Visible;
            }

            try
            {
                PluginLoader<IPluginControl> mef = new PluginLoader<IPluginControl>("ReportView_Page");
                foreach (var plugin in mef.loadedPlugins.Keys)
                {
                    plugin.SetupPlugin(this);
                }
            }
            catch (Exception Ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(Ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\ReportView_PageError.txt", json);
                //exec sp_updatestats
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl && e.AddedItems.Count > 0)
            {
                TabItem SelectedTab = e.AddedItems[0] as TabItem;
                if (SelectedTab != null)
                {
                    if (SelectedTab.Header.ToString() == "UserRole Report")
                    {
                        frameusrole.Navigate(new Uri("/View/ReportView/UserRoleReportViewer_Page.xaml", UriKind.Relative));
                    }
                    if (SelectedTab.Header.ToString() == "User Report")
                    {
                        frameusr.Navigate(new Uri("/View/ReportView/UserReportViewer_Page.xaml", UriKind.Relative));
                    }
                    else if (SelectedTab.Header.ToString() == "Unit Report")
                    {
                        frameur.Navigate(new Uri("/View/ReportView/unitReportViewer_Page.xaml", UriKind.Relative));
                    }
                    else if (SelectedTab.Header.ToString() == "Account Report")
                    {
                        frameAr.Navigate(new Uri("/View/ReportView/AccountReportViewer_Page.xaml", UriKind.Relative));
                    }
                    else if (SelectedTab.Header.ToString() == "Vehicle Report")
                    {
                        framevr.Navigate(new Uri("/View/ReportView/VehicleReportViewer_Page.xaml", UriKind.Relative));
                    }
                    else if (SelectedTab.Header.ToString() == "Driver Report")
                    {
                        framedr.Navigate(new Uri("/View/ReportView/DriverReportViewer_Page.xaml", UriKind.Relative));
                    }
                    else if (SelectedTab.Header.ToString() == "Product Report")
                    {
                        framepr.Navigate(new Uri("/View/ReportView/ProductReportViewer_Page.xaml", UriKind.Relative));
                    }
                    else if (SelectedTab.Header.ToString() == "Gate Report")
                    {
                        framegr.Navigate(new Uri("/View/ReportView/GateReportViewer_Page.xaml", UriKind.Relative));
                    }
                }
            }
        }

        private void btnBackToReport_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/ReportView/Reports_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
    }
}
