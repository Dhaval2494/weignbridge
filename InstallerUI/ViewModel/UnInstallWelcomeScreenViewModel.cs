using InstallerUI.Helpers;
using InstallerUI.View;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System.Windows.Input;

namespace InstallerUI.ViewModel
{
    public class UnInstallWelcomeScreenViewModel : ViewModelBase
    {
        public UnInstallWelcomeScreenViewModel(MainViewModel mainViewModel) : base(mainViewModel)
        {

        }
        public ICommand UnInstallCommand
        {
            get
            {
                return new BaseCommand(delegate
                {
                    this.Bootstrapper.Engine.Log(LogLevel.Standard, "Initializing UnInstallProgressPage");
                    this.MainViewModel.CurrentControl = new UnInstallProgressScreen(this.MainViewModel);
                }, delegate { return true; });
            }
        }
        public virtual ICommand CancelCommand
        {
            get
            {
                return new BaseCommand(delegate { this.MainViewModel.RequestCancel(true); }, delegate { return true; });
            }
        }
    }
}