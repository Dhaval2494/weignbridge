using MahApps.Metro.SimpleChildWindow;
using System.Windows;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for EmailServerSettingPage.xaml
    /// </summary>
    public partial class EmailServerSettingPage : ChildWindow
    {
        vmEmailServerSetting viewModel;
        public EmailServerSettingPage()
        {
            InitializeComponent();
            this.Unloaded += CurrentPage_Unloaded;
            viewModel = new vmEmailServerSetting(this);
            this.DataContext = viewModel;
        }

        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
        }
    }
}
