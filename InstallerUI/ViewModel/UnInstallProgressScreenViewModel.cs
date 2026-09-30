using InstallerUI.Helpers;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InstallerUI.ViewModel
{
    public class UnInstallProgressScreenViewModel : ViewModelBase
    {
        public ICommand CancelCommand
        {
            get
            {
                return new BaseCommand(delegate { this.MainViewModel.RequestCancel(true); }, delegate { return true; });
            }
        }
        public UnInstallProgressScreenViewModel(MainViewModel mainViewModel)
            : base(mainViewModel)
        {
            this.Bootstrapper.ExecuteProgress += Bootstrapper_ExecuteProgress;
            //this.Bootstrapper.ExecuteFilesInUse += EventProviderOnExecuteFilesInUse;
            IsProcessing = true;
            this.MainViewModel.PlanAction(LaunchAction.Uninstall);
        }

        void Bootstrapper_ExecuteProgress(object sender, ExecuteProgressEventArgs e)
        {
            this.Progress = e.OverallPercentage;
            IsProcessing = !this.MainViewModel.IsCancelled;
            e.Result = this.MainViewModel.IsCancelled ? Result.Cancel : Result.Ok;
        }

        public async Task<MessageDialogResult> ShowMessageOnMainWindow(string message)
        {
            return await MainViewModel.mainWindow.ShowMessageAsync("Files In Use", message, MessageDialogStyle.AffirmativeAndNegative);
        }

        private async void EventProviderOnExecuteFilesInUse(object sender, ExecuteFilesInUseEventArgs executeFilesInUseEventArgs)
        {
            //TODO: Cancel the installation. Notify the user of the running processes. Handle the e.Result < 0 on ApplyComplete side to provide error screen. Make InstallationResult = Error.
            //Check all possible outcomes of executeFilesInUseEventArgs.Result
            //Later uncomment the event handler
            executeFilesInUseEventArgs.Result = Result.Cancel;

            var message = new System.Text.StringBuilder("The following files are in use. Please close the applications that are using them.\n");
            foreach (var file in executeFilesInUseEventArgs.Files)
            {
                message.AppendLine(" - " + file);
            }
            var userButton = await CustomBootstrapper.BootstrapperDispatcher.Invoke<Task<MessageDialogResult>>(() => ShowMessageOnMainWindow(message.ToString()));

            //if (userButton == MessageDialogResult.Affirmative)
            //    executeFilesInUseEventArgs.Result = Result.Retry;
        }
        private int _progress;
        public int Progress
        {
            get { return _progress; }
            set { _progress = value; OnPropertyChanged(); }
        }

        private bool _isProcessing;
        public bool IsProcessing
        {
            get { return _isProcessing; }
            set { _isProcessing = value; OnPropertyChanged(); }
        }
    }
}