using MahApps.Metro.SimpleChildWindow;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WeighingSolution.DataLayer;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    public partial class CameraManagementList : Page
    {
        vmCameraManagementList viewModel;
        public CameraManagementList()
        {
            InitializeComponent();
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            viewModel = new vmCameraManagementList(this);
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

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/DatasetupView/CameraManagement.xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        private void CameraManagementListCancel_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/DatasetupView/DataSetup_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        private void DatagridCameraManagement_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var grid = (DataGrid)sender;
            if (Key.Delete == e.Key)
            {
                foreach (Camera row in grid.SelectedItems)
                {
                    viewModel.OnDeleteCameraCommand(row);
                }
            }
        }

        private void btNCameraSetting_Click(object sender, RoutedEventArgs e)
        {
            CameraImagePathSetting childWindow = new CameraImagePathSetting();
            ChildWindowManager.ShowChildWindowAsync(Application.Current.MainWindow, childWindow);
        }
    }
}
