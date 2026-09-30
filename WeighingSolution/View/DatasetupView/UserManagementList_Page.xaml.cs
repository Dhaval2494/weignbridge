using System;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for UserManagementList_Page.xaml
    /// </summary>
    public partial class UserManagementList_Page : Page
    {
        vmUserManagementList viewModel;
        public UserManagementList_Page()
        {
            InitializeComponent();
            viewModel = new vmUserManagementList();
            DataContext = viewModel;
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
        }

        private async void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();
            if (Common.ValidatePageRequest(PageEntity.User, PageAccess.Create))
                txtAddUser.Visibility = Visibility.Visible;
            if (Common.ValidatePageRequest(PageEntity.User, PageAccess.Update))
            {
                // DatagridUserManagement.Columns[4].Visibility = Visibility.Visible;
                foreach (DataGridColumn col in DatagridUserManagement.Columns)
                {
                    if (col != null && col.Header != null && col.Header.ToString() == "Action")
                        col.Visibility = Visibility.Visible;
                }
            }
        }
        private void btnAddMaster_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/DatasetupView/UserManagement_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
        }
    }
}
