using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for VehicleManagementList_Page.xaml
    /// </summary>
    public partial class WeighBridgeManagementList_Page : Page
    {
        vmWeighBridgeManagementList viewModel;
        public WeighBridgeManagementList_Page()
        {
            InitializeComponent();
            viewModel = new vmWeighBridgeManagementList();
            DataContext = viewModel;
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
        }

        private async void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();
            if (Common.ValidatePageRequest(PageEntity.WeighBridge, PageAccess.Create))
                txtbAddWeighBridge.Visibility = Visibility.Visible;
            if (Common.ValidatePageRequest(PageEntity.WeighBridge, PageAccess.Update))
            {
                foreach (DataGridColumn col in DatagridWeighBridgeManagement.Columns)
                {
                    if (col != null && col.Header != null && col.Header.ToString() == "Action")
                        col.Visibility = Visibility.Visible;
                }
            }
        }
        private void btnAddMaster_Click(object sender, RoutedEventArgs e)
        {
            //Uri uri = new Uri("/View/DatasetupView/WeighBridgeManagement_Page.Xaml?", UriKind.Relative);
            //this.NavigationService.Navigate(uri);
            this.NavigationService.Navigate(new WeighBridgeManagement_Page(0));
        }
        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
        }
    }
}
