using MahApps.Metro.SimpleChildWindow;
using System.Linq;
using System.Windows;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    public partial class SMSSettingPage : ChildWindow
    {
        VmSMSSetting viewModel;
        public SMSSettingPage()
        {
            InitializeComponent();
            this.Unloaded += CurrentPage_Unloaded;
            viewModel = new VmSMSSetting(this);
            this.DataContext = viewModel;

            if (viewModel.Providers.Count() > 0 && viewModel.Providers.FirstOrDefault().SMSType == "API")
            {
                rdbAPI.IsChecked = true;
            }
            if (viewModel.Providers.Count() > 0 && viewModel.Providers.FirstOrDefault().SMSType == "GSM")
            {
                rdbGSMModem.IsChecked = true;
            }
        }

        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
        }

    }
}
