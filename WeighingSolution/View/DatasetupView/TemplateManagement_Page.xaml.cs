using System;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.Events;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for TemplateManagement_Page.xaml
    /// </summary>
    public partial class TemplateManagement_Page : Page
    {
        int intTemplateID = 0;
        vmTemplateManagement viewModel;
        private void SetupPage()
        {
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            viewModel = new vmTemplateManagement(intTemplateID);
            this.DataContext = viewModel;
        }
        private void GoBack(object obj)
        {
            Uri uri = new Uri("/View/DatasetupView/TemplateMangementList_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        public TemplateManagement_Page()
        {
            InitializeComponent();
            SetupPage();
        }
        public TemplateManagement_Page(int TemplateId)
        {
            InitializeComponent();
            intTemplateID = TemplateId;
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