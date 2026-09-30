using System;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using WeighingSolution.DataLayer;
using WeighingSolution.Events;
using WeighingSolution.Helpers;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for UHFManagement_Page.xaml
    /// </summary>
    public partial class UHFManagement_Page : Page
    {
        public int IntUHFid = 0;
        vmUHFManagement viewModel;

        private void SetupPage()
        {
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            viewModel = new vmUHFManagement(IntUHFid);
            this.DataContext = viewModel;            
        }
        private void GoBack(object obj)
        {
            Uri uri = new Uri("/View/DatasetupView/UHFManagementList_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        public UHFManagement_Page()
        {
            InitializeComponent();
            SetupPage();
        }
        public UHFManagement_Page(int Id)
        {
            InitializeComponent();
            IntUHFid = Id;
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
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

    }
}
