using MahApps.Metro.Controls;
using System;
using System.Windows;
using WeighingSolution.DataLayer;
using WeighingSolution.ViewModel;

namespace WeighingSolution
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : MetroWindow
    {
        LoginViewModel viewModel;
        public Login()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            Application.Current.MainWindow = this;
            this.Unloaded += CurrentPage_Unloaded;
            viewModel = new LoginViewModel(this);
            this.DataContext = viewModel;
            txtAssemblyVersion.Text = "  " + Constant.AssemblyVersion;
        }

        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
