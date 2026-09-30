using System;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.Events;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for GateManagement_Page.xaml
    /// </summary>
    public partial class GateManagement_Page : Page
    {
        public int IntGateid = 0;
        vmGateManagement viewModel;

        private void SetupPage()
        {
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            viewModel = new vmGateManagement(IntGateid);
            this.DataContext = viewModel;
        }
        private void GoBack(object obj)
        {
            Uri uri = new Uri("/View/DatasetupView/GateManagementList_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        public GateManagement_Page()
        {
            InitializeComponent();
            SetupPage();
        }
        public GateManagement_Page(int GateId)
        {
            InitializeComponent();
            IntGateid = GateId;
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