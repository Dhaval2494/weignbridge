using InstallerUI.ViewModel;

namespace InstallerUI.View
{
    public partial class UnInstallProgressScreen
    {
        public UnInstallProgressScreen(MainViewModel mainViewModel)
        {
            this.DataContext = new UnInstallProgressScreenViewModel(mainViewModel);
            InitializeComponent();
        }
    }
}