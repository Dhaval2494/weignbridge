using InstallerUI.ViewModel;

namespace InstallerUI.View
{
    public partial class InstallWelcomeScreen
    {
        public InstallWelcomeScreen(MainViewModel mainViewModel)
        {
            this.DataContext = new InstallWelcomeScreenViewModel(mainViewModel);
            InitializeComponent();
        }
    }
}
