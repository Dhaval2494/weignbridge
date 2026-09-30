using InstallerUI.Helpers;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Linq;
using System.Windows.Input;

namespace InstallerUI.ViewModel
{
    public class InstallProgressScreenViewModel : ViewModelBase
    {
        public ICommand CancelCommand
        {
            get
            {
                return new BaseCommand(delegate { this.MainViewModel.RequestCancel(true); }, delegate { return true; });
            }
        }
        public InstallProgressScreenViewModel(MainViewModel mainViewModel) : base(mainViewModel)
        {
            InitEvents();
            IsProcessing = true;
            WriteVariableToBundle("UNINSTALLER_PATH", string.Format(@"{0}\{1}\{2}\{3}.exe", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Package Cache", ReadVariableFromBundle("WixBundleProviderKey"), "WeighingSolutionSetup.exe"));
            this.MainViewModel.PlanAction(LaunchAction.Install);
        }

        private void InitEvents()
        {
            this.Bootstrapper.CacheAcquireBegin += Bootstrapper_CacheAcquireBegin;
            this.Bootstrapper.CacheAcquireProgress += Bootstrapper_CacheAcquireProgress;
            this.Bootstrapper.CacheAcquireComplete += Bootstrapper_CacheAcquireComplete;
            this.Bootstrapper.ExecutePackageBegin += Bootstrapper_ExecuteBegin;
            this.Bootstrapper.ExecuteProgress += Bootstrapper_ExecuteProgress;
            this.Bootstrapper.ExecuteFilesInUse += EventProviderOnExecuteFilesInUse;
        }
        void Bootstrapper_CacheAcquireBegin(object sender, CacheAcquireBeginEventArgs e)
        {
            //this.DownloadProgress = 0;
            var package = this.MainViewModel.BundleInfo.Packages.FirstOrDefault(x => x.Id == e.PackageOrContainerId);
            this.PackageDownloadName = package != null ? package.DisplayName : "";
        }
        private void Bootstrapper_CacheAcquireProgress(object sender, CacheAcquireProgressEventArgs e)
        {
            this.DownloadProgress = e.OverallPercentage; // / this.Bootstrapper.Engine.PackageCount;
            this.Progress = (this.DownloadProgress + this.InstallProgress) / 2;
            e.Result = this.MainViewModel.IsCancelled ? Result.Cancel : Result.Ok;
        }
        void Bootstrapper_CacheAcquireComplete(object sender, CacheAcquireCompleteEventArgs e)
        {
            this.DownloadProgress = 100;
            this.Progress = (this.DownloadProgress + this.InstallProgress) / 2;
        }
        private void Bootstrapper_ExecuteBegin(object sender, ExecutePackageBeginEventArgs executePackageBeginEventArgs)
        {
            var package = this.MainViewModel.BundleInfo.Packages.FirstOrDefault(x => x.Id == executePackageBeginEventArgs.PackageId);
            this.PackageInstallName = package != null ? package.DisplayName : "";
        }
        void Bootstrapper_ExecuteProgress(object sender, ExecuteProgressEventArgs e)
        {
            this.InstallProgress = e.ProgressPercentage;
            this.Progress = (this.DownloadProgress + e.OverallPercentage) / 2;
            e.Result = this.MainViewModel.IsCancelled ? Result.Cancel : Result.Ok;
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

            var userButton = await MainViewModel.mainWindow.ShowMessageAsync("Files In Use", message.ToString(), MessageDialogStyle.AffirmativeAndNegative);

            //if (userButton != MessageDialogResult.Affirmative)
            //    executeFilesInUseEventArgs.Result = Result.Cancel;
        }
        private int _progress;
        public int Progress
        {
            get { return _progress; }
            set { _progress = value; OnPropertyChanged(); }
        }
        private string _packageDownloadName;
        public string PackageDownloadName
        {
            get { return _packageDownloadName; }
            set { _packageDownloadName = value; OnPropertyChanged(); }
        }
        private string _packageInstallName;
        public string PackageInstallName
        {
            get { return _packageInstallName; }
            set { _packageInstallName = value; OnPropertyChanged(); }
        }
        private int _downloadProgress;
        public int DownloadProgress
        {
            get { return _downloadProgress; }
            set { _downloadProgress = value; OnPropertyChanged(); }
        }
        private int _installProgress;
        public int InstallProgress
        {
            get { return _installProgress; }
            set { _installProgress = value; OnPropertyChanged(); }
        }
        private bool _isProcessing;
        public bool IsProcessing
        {
            get { return _isProcessing; }
            set { _isProcessing = value; OnPropertyChanged(); }
        }
    }
}