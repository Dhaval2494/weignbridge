using InstallerUI.ViewModel;

namespace InstallerUI.View
{
    public partial class InstallFinishScreen
    {
        public InstallFinishScreen(MainViewModel mainViewModel)
        {
            this.DataContext = new InstallFinishScreenViewModel(mainViewModel);
            InitializeComponent();
        }
    }
}
