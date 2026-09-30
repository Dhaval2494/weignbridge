using InstallerUI.ViewModel;

namespace InstallerUI.View
{
    public partial class InstallProgressScreen
    {
        public InstallProgressScreen(MainViewModel mainViewModel)
        {
            this.DataContext = new InstallProgressScreenViewModel(mainViewModel);
            InitializeComponent();
        }
    }
}
