using InstallerUI.ViewModel;

namespace InstallerUI.View
{
    public partial class UnInstallFinishScreen
    {
        public UnInstallFinishScreen(MainViewModel mainViewModel)
        {
            this.DataContext = new UnInstallFinishScreenViewModel(mainViewModel);
            InitializeComponent();
        }
    }
}
