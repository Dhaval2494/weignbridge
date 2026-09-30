using MahApps.Metro.Controls;
using System.Windows;
using WeighingSolution.ViewModel;

namespace WeighingSolution
{
    /// <summary>
    /// Interaction logic for DBConfigureation.xaml
    /// </summary>
    public partial class DBConfigureation : MetroWindow
    {
        DBConfigurationViewModel viewModel;
        public DBConfigureation()
        {
            InitializeComponent();
            viewModel = new DBConfigurationViewModel();
            this.DataContext = viewModel;
            Application.Current.MainWindow = this;
        }
    }
}
