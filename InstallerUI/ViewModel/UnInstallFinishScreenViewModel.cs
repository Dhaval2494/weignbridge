using InstallerUI.Helpers;
using System.Windows.Input;

namespace InstallerUI.ViewModel
{
    public class UnInstallFinishScreenViewModel : ViewModelBase
    {
        public UnInstallFinishScreenViewModel(MainViewModel mainViewModel) : base(mainViewModel)
        {

        }
        public ICommand CloseCommand
        {
            get { return new BaseCommand(delegate { System.Environment.Exit(0); }, o => true); }
        }
        private string _finishmessage = "Uninstallation completed successfully";
        public string FinishMessage
        {
            get { return _finishmessage; }
            set { _finishmessage = value; }
        }
    }
}