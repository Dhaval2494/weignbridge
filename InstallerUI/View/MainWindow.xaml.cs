using InstallerUI.ViewModel;
using System.Windows.Interop;

namespace InstallerUI.View
{
    /// <summary>
    /// Interaction logic for WelcomePageView.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow(CustomBootstrapper wixBootstrapper)
        {
            InitializeComponent();
            var viewModel = new MainViewModel(this, wixBootstrapper) { Hwnd = new WindowInteropHelper(this).Handle };
            this.DataContext = viewModel;
        }

        public MainViewModel ViewModel
        {
            get { return (MainViewModel)DataContext; }
        }
    }
}
