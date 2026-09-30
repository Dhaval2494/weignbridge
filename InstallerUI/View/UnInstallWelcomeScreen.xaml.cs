using InstallerUI.ViewModel;

namespace InstallerUI.View
{
    public partial class UnInstallWelcomeScreen
    {
        public UnInstallWelcomeScreen(MainViewModel mainViewModel)
        {
            this.DataContext = new UnInstallWelcomeScreenViewModel(mainViewModel);
            InitializeComponent();
        }
    }
}
