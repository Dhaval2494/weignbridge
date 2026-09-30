using System;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for UHFManagementList_Page.xaml
    /// </summary>
    public partial class UHFManagementList_Page : Page
    {
        vmUHFManagementList viewModel;
        public UHFManagementList_Page()
        {
            InitializeComponent();
            viewModel = new vmUHFManagementList();
            DataContext = viewModel;
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
        }

        private async void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();
            if (Common.ValidatePageRequest(PageEntity.UHF, PageAccess.Create))
                txtbAddUHF.Visibility = Visibility.Visible;
            if (Common.ValidatePageRequest(PageEntity.UHF, PageAccess.Update))
            {             
                foreach (DataGridColumn col in DatagridUHFManagement.Columns)
                {
                    if (col != null && col.Header != null && col.Header.ToString() == "Action")
                        col.Visibility = Visibility.Visible;
                }
            }
        }
        private void btnAddMaster_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/DatasetupView/UHFManagement_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
        }
    }
}
