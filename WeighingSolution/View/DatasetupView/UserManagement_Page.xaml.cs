using System;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.Events;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for UserManagement_Page.xaml
    /// </summary>
    public partial class UserManagement_Page : Page
    {
        vmUserManagement viewModel;
        public int IntUserid = 0;

        private void SetupPage()
        {
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            viewModel = new vmUserManagement(IntUserid);
            this.DataContext = viewModel;
        }
        private void GoBack(object obj)
        {
            Uri uri = new Uri("/View/DatasetupView/UserManagementList_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        private async void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();
            viewModel.EventAggregator.GetEvent<AfterDetailSaveCancelledEvent>().Subscribe(GoBack);
            viewModel.EventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(GoBack);
        }

        public UserManagement_Page(int Userid)
        {
            InitializeComponent();
            IntUserid = Userid;
            txtUserName.IsEnabled = false;
            SetupPage();
        }
        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.EventAggregator.GetEvent<AfterDetailSaveCancelledEvent>().Unsubscribe(GoBack);
            viewModel.EventAggregator.GetEvent<AfterDetailSavedEvent>().Unsubscribe(GoBack);
            viewModel.Dispose();
        }
        public UserManagement_Page()
        {
            InitializeComponent();
            SetupPage();
        }
    }
}
