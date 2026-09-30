using System;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for GateManagementList_Page.xaml
    /// </summary>
    public partial class GateManagementList_Page : Page
    {
        vmGateManagementList viewModel;

        public GateManagementList_Page()
        {
            InitializeComponent();
            viewModel = new vmGateManagementList();
            DataContext = viewModel;
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
        }

        async void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();

            if (Common.ValidatePageRequest(PageEntity.Gate, PageAccess.Create))
            {
                txtbAddGate.Visibility = Visibility.Visible;
            }
            if (Common.ValidatePageRequest(PageEntity.Gate, PageAccess.Update))
            {
                // DatagridGateManagement.Columns[3].Visibility = Visibility.Visible;
                foreach (DataGridColumn col in DatagridGateManagement.Columns)
                {
                    if (col != null && col.Header != null && col.Header.ToString() == "Action")
                        col.Visibility = Visibility.Visible;
                }
            }
        }
        private void btnAddMaster_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/DatasetupView/GateManagement_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
        }
    }
}