using System;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for TemplateMangementList_Page.xaml
    /// </summary>
    public partial class TemplateMangementList_Page : Page
    {
        vmTemplateManagementList viewModel;
        public TemplateMangementList_Page()
        {
            InitializeComponent();
            viewModel = new vmTemplateManagementList();
            DataContext = viewModel;
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
        }

        private async void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();
            if (Common.ValidatePageRequest(PageEntity.UserRole, PageAccess.Create))
                txtbAddTemplate.Visibility = Visibility.Visible;
            if (Common.ValidatePageRequest(PageEntity.UserRole, PageAccess.Update))
            {
                foreach (DataGridColumn col in DatagridTemplateManagement.Columns)
                {
                    if (col != null && col.Header != null && col.Header.ToString() == "Action")
                        col.Visibility = Visibility.Visible;
                }
            }
        }

        private void btnAddMaster_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/DatasetupView/TemplateManagement_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
        }
    }
}
