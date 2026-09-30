using MahApps.Metro.SimpleChildWindow;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WeighingSolution.DataLayer;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for SMSPage.xaml
    /// </summary>
    public partial class SMSScheduleList_Page : Page
    {
        vmSMSScheduleList viewModel;
        public SMSScheduleList_Page()
        {
            InitializeComponent();
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            viewModel = new vmSMSScheduleList(this);
            this.DataContext = viewModel;
        }
        private async void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();
        }
        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
        }

        private void SMSScheduleListCancel_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/SettingView/Setting_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        private void btnAddSchedule_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/SettingView/SMSSchedulePage.xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        private void DatagridSMSSchedule_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var grid = (DataGrid)sender;
            if (Key.Delete == e.Key)
            {
                foreach (SMSSetup row in grid.SelectedItems)
                {
                    viewModel.OnDeleteScheduleCommand(row);
                }
            }
        }

        private void btnSMSServerSetting_Click(object sender, RoutedEventArgs e)
        {
            SMSSettingPage childWindow = new SMSSettingPage();
            ChildWindowManager.ShowChildWindowAsync(Application.Current.MainWindow, childWindow);
        }
    }
}
