using System;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for ProductManagementList_Page.xaml
    /// </summary>
    public partial class ProductManagementList_Page : Page
    {
        vmProductManagementList viewModel;
        public ProductManagementList_Page()
        {
            InitializeComponent();
            viewModel = new vmProductManagementList();
            DataContext = viewModel;
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
        }

        private async void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();
            if (Constant.IsAdmin || Constant.IsSuperAdmin || Constant.IsSupportUser)
            {
                //exceldata.Visibility = Visibility.Visible;
                //btnsaveExceldata.Visibility = Visibility.Visible;
            }
            if (Common.ValidatePageRequest(PageEntity.Product, PageAccess.Create))
                txtbAddProduct.Visibility = Visibility.Visible;
            if (Common.ValidatePageRequest(PageEntity.Product, PageAccess.Update))
            {
                //DatagridProductManagement.Columns[3].Visibility = Visibility.Visible;
                foreach (DataGridColumn col in DatagridProductManagement.Columns)
                {
                    if (col != null && col.Header != null && col.Header.ToString() == "Action")
                        col.Visibility = Visibility.Visible;
                }
            }
        }
        private void btnAddMaster_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/DatasetupView/ProductManagement_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
        }

        private void CSV_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/DatasetupView/ProductCSV_Upload_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
    }
}
