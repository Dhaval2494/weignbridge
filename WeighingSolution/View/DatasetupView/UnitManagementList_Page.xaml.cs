using System;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for UnitManagementList_Page.xaml
    /// </summary>
    public partial class UnitManagementList_Page : Page
    {
        vmUnitManagementList viewModel;
        public UnitManagementList_Page()
        {
            InitializeComponent();
            viewModel = new vmUnitManagementList();
            DataContext = viewModel;
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
        }

        private async void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();
            if (Common.ValidatePageRequest(PageEntity.Unit, PageAccess.Create))
                txtbAddUnit.Visibility = Visibility.Visible;
            if (Common.ValidatePageRequest(PageEntity.Unit, PageAccess.Update))
            {
                //  DatagridUnitManagement.Columns[2].Visibility = Visibility.Visible;
                foreach (DataGridColumn col in DatagridUnitManagement.Columns)
                {
                    if (col != null && col.Header != null && col.Header.ToString() == "Action")
                        col.Visibility = Visibility.Visible;
                }
            }
        }
        private void btnAddMaster_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/DatasetupView/UnitManagement_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
        }
    }
}
