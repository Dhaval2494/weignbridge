using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for StampCertificateList_Page.xaml
    /// </summary>
    public partial class StampCertificateList_Page : Page
    {
        vmStampCertificateList viewModel;
        public StampCertificateList_Page()
        {
            InitializeComponent();
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            viewModel = new vmStampCertificateList(this);
            this.DataContext = viewModel;
        }
        private async void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();
        }
        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
        }

        private void BtnAddCertificate_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/SettingView/StampCertificate_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        private void DatagridStampCertificate_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
