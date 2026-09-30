using System;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.Events;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for ProductManagement_Pagexaml.xaml
    /// </summary>
    public partial class ProductManagement_Page : Page
    {
        public int IntProductid = 0;
        vmProductManagement viewModel;
        private void SetupPage()
        {
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            viewModel = new vmProductManagement(IntProductid);
            this.DataContext = viewModel;
        }
        private void GoBack(object obj)
        {
            Uri uri = new Uri("/View/DatasetupView/ProductManagementList_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        public ProductManagement_Page()
        {
            InitializeComponent();
            SetupPage();
        }
        public ProductManagement_Page(int Productid)
        {
            InitializeComponent();
            IntProductid = Productid;
            SetupPage();
        }
        private async void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();
            viewModel.EventAggregator.GetEvent<AfterDetailSaveCancelledEvent>().Subscribe(GoBack);
            viewModel.EventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(GoBack);
        }
        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.EventAggregator.GetEvent<AfterDetailSaveCancelledEvent>().Unsubscribe(GoBack);
            viewModel.EventAggregator.GetEvent<AfterDetailSavedEvent>().Unsubscribe(GoBack);
            viewModel.Dispose();
        }
    }
}
