using System;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.Events;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for UnitManagement_Page.xaml
    /// </summary>
    public partial class UnitManagement_Page : Page
    {
        public int IntUnitID = 0;
        vmUnitManagement viewModel;
        private void SetupPage()
        {
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            viewModel = new vmUnitManagement(IntUnitID);
            this.DataContext = viewModel;
        }
        private void GoBack(object obj)
        {
            Uri uri = new Uri("/View/DatasetupView/UnitManagementList_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        public UnitManagement_Page()
        {
            InitializeComponent();
            SetupPage();
        }
        public UnitManagement_Page(int UnitId)
        {
            InitializeComponent();
            IntUnitID = UnitId;
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
