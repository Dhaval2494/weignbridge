using System.Windows;
using System.Windows.Controls;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for EmailSchedulePage.xaml
    /// </summary>
    public partial class EmailSchedulePage : Page
    {
        vmEmailSchedulePage viewModel;
        public int Intemailsetupid = 0;

        #region CTOR
        private void SetupPage()
        {
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            viewModel = new vmEmailSchedulePage(this, Intemailsetupid);
            this.DataContext = viewModel;
        }

        public EmailSchedulePage()
        {
            InitializeComponent();
            SetupPage();
        }
        public EmailSchedulePage(int EmailSetupID)
        {
            InitializeComponent();
            Intemailsetupid = EmailSetupID;
            SetupPage();
        }
        #endregion
        private async void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();
        }

        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
        }
    }
}
