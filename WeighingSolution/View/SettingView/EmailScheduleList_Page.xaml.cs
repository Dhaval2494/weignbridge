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
    /// Interaction logic for EmailScheduleList_Page.xaml
    /// </summary>
    public partial class EmailScheduleList_Page : Page
    {
        vmEmailScheduleList viewModel;
        public EmailScheduleList_Page()
        {
            InitializeComponent();
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            viewModel = new vmEmailScheduleList(this);
            this.DataContext = viewModel;
            if (Common.ValidatePageRequest(PageEntity.EmailSetup, PageAccess.Create))
                txtbAddSchedule.Visibility = Visibility.Visible;
        }
        private async void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();
        }
        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
        }

        private void EmailScheduleListCancel_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/SettingView/Setting_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        private void btnAddSchedule_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/SettingView/EmailSchedulePage.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        private void DatagridEmailSchedule_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var grid = (DataGrid)sender;
            if (Key.Delete == e.Key)
            {
                foreach (EmailSetup row in grid.SelectedItems)
                {
                    viewModel.OnDeleteScheduleCommand(row);
                }
            }
        }

        private void BtnMailServerSetting_Click(object sender, RoutedEventArgs e)
        {
            EmailServerSettingPage childWindow = new EmailServerSettingPage();
            ChildWindowManager.ShowChildWindowAsync(Application.Current.MainWindow, childWindow);
        }
    }
}
