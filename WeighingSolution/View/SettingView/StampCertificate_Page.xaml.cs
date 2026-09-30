using System.Windows.Controls;

using WeighingSolution.ViewModel.SettingViewModel;

namespace WeighingSolution.View.SettingView
{
    /// <summary>
    /// Interaction logic for StampCertificate_Page.xaml
    /// </summary>
    public partial class StampCertificate_Page : Page
    {
        vmStampCertificate viewModel;
        public StampCertificate_Page()
        {
            InitializeComponent();
            viewModel = new vmStampCertificate(this);
            this.DataContext = viewModel;
        }
    }
}
