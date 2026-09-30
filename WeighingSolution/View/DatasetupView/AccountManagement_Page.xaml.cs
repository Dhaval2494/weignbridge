using System;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.Events;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for AccountManagement_Page.xaml
    /// </summary>
    public partial class AccountManagement_Page : Page
    {
        vmAccountManagement viewModel;
        public int IntAccID = 0;
        private void SetupPage()
        {
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            viewModel = new vmAccountManagement(IntAccID);
            this.DataContext = viewModel;

        }
        private void GoBack(object obj)
        {
            Uri uri = new Uri("/View/DatasetupView/AccountManagementList_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        public AccountManagement_Page()
        {
            InitializeComponent();
            SetupPage();
        }

        public AccountManagement_Page(int Accid)
        {
            InitializeComponent();
            IntAccID = Accid;
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
