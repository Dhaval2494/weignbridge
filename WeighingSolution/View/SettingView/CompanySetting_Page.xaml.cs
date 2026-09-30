using System.Windows;
using System.Windows.Controls;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for CompanySetting_Page.xaml
    /// </summary>
    public partial class CompanySetting_Page : Page
    {
        vmCompanySetting viewModel;
        public CompanySetting_Page()
        {
            InitializeComponent();
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            viewModel = new vmCompanySetting(this);
            this.DataContext = viewModel;
        }

        private void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            //if (Constant.IsSuperAdmin || Constant.IsSupportUser)
            CompanySettingSave.Visibility = Visibility.Visible;
        }

        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
        }
    }
}
