using InstallerUI.Helpers;
using InstallerUI.View;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace InstallerUI.ViewModel
{
    public class MainViewModel : NotifyPropertyChanged
    {
        private bool _newerVersionAlreadyInstalled;
        public readonly MainWindow mainWindow;
        public LaunchAction RunMode { get; set; }
        public Display DisplayMode { get; set; }
        public IEnumerable<string> CommandLine { get; set; }
        public IntPtr Hwnd { get; set; }
        public bool IsCancelled { get; set; }
        public bool RestartRequired { get; set; }
        public BundleInfo BundleInfo { get; set; }
        public InstallationMode InstallationMode { get; set; }
        public InstallationResult InstallationResult { get; set; }
        public int FinalResult { get; private set; }
        public BootstrapperApplication Bootstrapper { get; set; }
        private readonly IList<string> _bundlesToUpgrade;

        private FrameworkElement _currentControl;
        public FrameworkElement CurrentControl
        {
            get { return _currentControl; }
            set { _currentControl = value; OnPropertyChanged(); }
        }

        private TransitionType _transitionType;
        public TransitionType TransitionType
        {
            get { return _transitionType; }
            set { _transitionType = value; OnPropertyChanged(); }
        }
        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; OnPropertyChanged(); }
        }
        private BurnInstallationState _burnInstallationState;
        public BurnInstallationState BurnInstallationState
        {
            get { return _burnInstallationState; }
            set { _burnInstallationState = value; OnPropertyChanged(); }
        }
        private bool _removedata;
        public bool RemoveData
        {
            get { return _removedata; }
            set { _removedata = value; OnPropertyChanged(); }
        }


        public MainViewModel(MainWindow mainMetroWindow, BootstrapperApplication bootstrapper)
        {
            mainWindow = mainMetroWindow;
            Bootstrapper = bootstrapper;
            _bundlesToUpgrade = new List<string>();
            TransitionType = TransitionType.Left;
            this.BurnInstallationState = BurnInstallationState.NotPresent;
            BundleInfo = BundleInfoLoader.Load();
            this.InitEvents();
            this.InitCommandLineParams();

        }

        private void InitCommandLineParams()
        {
            RunMode = this.Bootstrapper.Command.Action;
            DisplayMode = this.Bootstrapper.Command.Display;
            CommandLine = this.Bootstrapper.Command.GetCommandLineArgs();
        }

        private void InitEvents()
        {
            this.Bootstrapper.DetectBegin += Bootstrapper_DetectBegin;
            this.Bootstrapper.DetectRelatedBundle += HandleExistingBundleDetected;
            this.Bootstrapper.DetectComplete += Bootstrapper_DetectComplete;

            this.Bootstrapper.PlanRelatedBundle += Bootstrapper_PlanRelatedBundle;
            this.Bootstrapper.PlanComplete += Bootstrapper_PlanComplete;
            this.Bootstrapper.ApplyComplete += Bootstrapper_ApplyComplete;

            this.Bootstrapper.ResolveSource += Bootstrapper_ResolveSource;
            this.Bootstrapper.Error += Bootstrapper_Error;
            this.Bootstrapper.Shutdown += Bootstrapper_Shutdown;
        }

        private void Bootstrapper_DetectBegin(object sender, DetectBeginEventArgs e)
        {
            BurnInstallationState = e.Installed ? BurnInstallationState.Present : BurnInstallationState.NotPresent;
        }
        void Bootstrapper_DetectComplete(object sender, DetectCompleteEventArgs e)
        {
            this.Bootstrapper.Engine.CloseSplashScreen();
            if (this._newerVersionAlreadyInstalled)
            {
                this.Bootstrapper.Engine.Log(LogLevel.Standard, "Downgrade process...");
                CustomBootstrapper.BootstrapperDispatcher.BeginInvoke(new Action(async delegate
                {
                    await mainWindow.ShowMessageAsync("Error", "Newer version of Product is already installed.\nThe setup will now exit.");
                    CustomBootstrapper.BootstrapperDispatcher.InvokeShutdown();
                }));
            }
            else if (BurnInstallationState == BurnInstallationState.NotPresent)
            {
                CustomBootstrapper.BootstrapperDispatcher.BeginInvoke(new Action(async delegate
                {
                    this.InstallationMode = InstallationMode.Install;
                    this.Bootstrapper.Engine.Log(LogLevel.Standard, "Initializing WelcomePage");
                    CurrentControl = new InstallWelcomeScreen(this);
                    if (this._bundlesToUpgrade != null && this._bundlesToUpgrade.Count > 0)
                        await mainWindow.ShowMessageAsync("Upgrade Info!", "A previous version of Product is already installed.\nPrevious version will be uninstalled first.\nThe setup will then install current newer version.\nData and Settings will not be affected.");
                }));
            }
            else if (BurnInstallationState == BurnInstallationState.Present)
            {
                if (RunMode == LaunchAction.Uninstall && (DisplayMode == Display.Embedded || DisplayMode == Display.None))
                {
                    //this.PlanAction(LaunchAction.Uninstall);
                    CustomBootstrapper.BootstrapperDispatcher.BeginInvoke(new Action(delegate
                    {
                        this.InstallationMode = InstallationMode.Uninstall;
                        this.Bootstrapper.Engine.Log(LogLevel.Standard, "Initializing UnInstallProgressPage");
                        CurrentControl = new UnInstallProgressScreen(this);
                    }));
                }
                else
                {
                    CustomBootstrapper.BootstrapperDispatcher.BeginInvoke(new Action(delegate
                    {
                        this.InstallationMode = InstallationMode.Uninstall;
                        this.Bootstrapper.Engine.Log(LogLevel.Standard, "Initializing Uninstall WelcomePage");
                        CurrentControl = new UnInstallWelcomeScreen(this);
                    }));
                }
            }
        }

        private void Bootstrapper_PlanComplete(object sender, PlanCompleteEventArgs e)
        {
            this.Bootstrapper.Engine.Apply(this.Hwnd);
        }

        void Bootstrapper_Shutdown(object sender, ShutdownEventArgs e)
        {
            if (this.RestartRequired)
                e.Result = Result.Restart;
        }
        void HandleExistingBundleDetected(object sender, DetectRelatedBundleEventArgs e)
        {
            if (e.Operation == RelatedOperation.Downgrade)
            {
                this.Bootstrapper.Engine.Log(LogLevel.Error, "newer version of product is already installed");
                this._newerVersionAlreadyInstalled = true;
            }
            if (e.Operation == RelatedOperation.MajorUpgrade || e.Operation == RelatedOperation.MinorUpdate)
            {
                this._bundlesToUpgrade.Add(e.ProductCode);
            }
        }

        void Bootstrapper_PlanRelatedBundle(object sender, PlanRelatedBundleEventArgs e)
        {
            this.Bootstrapper.Engine.Log(LogLevel.Standard,
                string.Format("Setting Bundle to Absent for processing upgrade: {0}", e.BundleId));

            foreach (var bundle in _bundlesToUpgrade)
            {
                this.Bootstrapper.Engine.Log(LogLevel.Standard, string.Format("check for upgrade: product code: {0}, bundleId = {1}", bundle, e.BundleId));
                if (string.CompareOrdinal(bundle, e.BundleId) == 0)
                {
                    this.Bootstrapper.Engine.Log(LogLevel.Standard, "setting bundle to Absent");
                    e.State = RequestState.Absent;
                }
            }
        }

        void Bootstrapper_ApplyComplete(object sender, ApplyCompleteEventArgs e)
        {
            bool lockObject = false;
            try
            {
                Monitor.Enter(this, ref lockObject);
                //TODO: If FinalResult < 0, it implies error condition. Handle this.
                this.FinalResult = e.Status;

                if (BurnInstallationState == BurnInstallationState.NotPresent)
                {
                    if (this.InstallationResult == InstallationResult.Error || this.InstallationResult == InstallationResult.Canceled)
                    {
                        //CustomBootstrapper.BootstrapperDispatcher.BeginInvoke(new Action(
                        //    () => this.SetCurrentPage(Pages.FinishErrorPage)));
                        CustomBootstrapper.BootstrapperDispatcher.BeginInvoke(new Action(delegate
                        {
                            this.Bootstrapper.Engine.Log(LogLevel.Standard, "Initializing FinishPage");
                            CurrentControl = new InstallFinishScreen(this);
                            InstallFinishScreenViewModel vm = ((InstallFinishScreenViewModel)CurrentControl.DataContext);
                            vm.FinishMessage = this.ErrorMessage;
                            vm.ShowLaunchApplication = false;
                            vm.LaunchApplication = false;
                            vm.ScreenTitle = "Installation failed!";
                        }));
                    }
                    else
                    {
                        this.InstallationResult = e.Restart == ApplyRestart.RestartRequired ? InstallationResult.SuccessRestartRequired : InstallationResult.Success;
                        CustomBootstrapper.BootstrapperDispatcher.BeginInvoke(new Action(delegate
                        {
                            this.Bootstrapper.Engine.Log(LogLevel.Standard, "Initializing FinishPage");
                            CurrentControl = new InstallFinishScreen(this);
                            if (e.Restart == ApplyRestart.RestartRequired)
                                ((InstallFinishScreenViewModel)CurrentControl.DataContext).FinishMessage += (Environment.NewLine + "A restart is required before continuing");
                        }));
                    }
                }
                else if (BurnInstallationState == BurnInstallationState.Present)
                {
                    if (this.InstallationResult == InstallationResult.Error || this.InstallationResult == InstallationResult.Canceled)
                    {
                        CustomBootstrapper.BootstrapperDispatcher.BeginInvoke(new Action(delegate
                        {
                            this.Bootstrapper.Engine.Log(LogLevel.Standard, "Initializing UnInstallFinishScreen");
                            CurrentControl = new UnInstallFinishScreen(this);
                            ((UnInstallFinishScreenViewModel)CurrentControl.DataContext).FinishMessage = this.ErrorMessage;
                        }));
                    }
                    else
                    {
                        if (RunMode == LaunchAction.Uninstall && (DisplayMode == Display.Embedded || DisplayMode == Display.None))
                        {
                            System.Environment.Exit(0);
                        }
                        else
                        {
                            if (RemoveData)
                            {
                                string pluginPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\WeighBridgeSolution\Plugins";

                                foreach (var item in System.IO.Directory.GetDirectories(pluginPath))
                                {
                                    string plDir = item;
                                    if (System.IO.File.Exists(plDir + @"\ForceUninstall.bat"))
                                    {
                                        System.Diagnostics.Process proc = null;
                                        try
                                        {
                                            proc = new System.Diagnostics.Process();
                                            proc.StartInfo.WorkingDirectory = plDir;
                                            proc.StartInfo.FileName = "ForceUninstall.bat";
                                            proc.StartInfo.CreateNoWindow = true;
                                            proc.Start();
                                            proc.WaitForExit();
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }

                                string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\WeighBridgeSolution";
                                try
                                {
                                    System.IO.Directory.Delete(path, true);
                                }
                                catch (System.IO.IOException)
                                {
                                    Thread.Sleep(0);
                                    System.IO.Directory.Delete(path, true);
                                }
                                catch (UnauthorizedAccessException)
                                {
                                    Thread.Sleep(0);
                                    System.IO.Directory.Delete(path, true);
                                }
                            }

                            this.InstallationResult = e.Restart == ApplyRestart.RestartRequired ? InstallationResult.SuccessRestartRequired : InstallationResult.Success;
                            CustomBootstrapper.BootstrapperDispatcher.BeginInvoke(new Action(delegate
                            {
                                this.Bootstrapper.Engine.Log(LogLevel.Standard, "Initializing UnInstallFinishPage");
                                CurrentControl = new UnInstallFinishScreen(this);
                                if (e.Restart == ApplyRestart.RestartRequired)
                                    ((InstallFinishScreenViewModel)CurrentControl.DataContext).FinishMessage += (Environment.NewLine + "A restart is required before continuing");
                            }));
                        }
                    }
                }
            }
            finally
            {
                if (lockObject)
                {
                    Monitor.Exit(this);
                }
            }
        }

        void Bootstrapper_ResolveSource(object sender, ResolveSourceEventArgs e)
        {
            this.Bootstrapper.Engine.Log(LogLevel.Standard, string.Format("ResolveSource Event: {0}", e.DownloadSource));

            if (e.DownloadSource.Contains("http") &&
                !InternetConnectionValidator.IsInternetConnectionAvailabe(this.Bootstrapper.Engine))
            {
                this.Bootstrapper.Engine.Log(LogLevel.Error, "No internet connection available");
                var task = mainWindow.ShowMessageAsync("Error", "No internet connection available");
                task.Wait();
                e.Result = Result.Cancel;
                this.IsCancelled = true;
            }
            else
            {
                e.Result = Result.Download;
            }
        }

        void Bootstrapper_Error(object sender, ErrorEventArgs e)
        {
            if (e != null)
            {
                this.InstallationResult = InstallationResult.Error;
                this.ErrorMessage = e.ErrorMessage;
            }
        }
        public void RequestCancel(bool showMessage)
        {
            if (showMessage)
            {

                var task = mainWindow.ShowMessageAsync("Question", "Are you sure you want to cancel installation?",
                    MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings
                    {
                        AffirmativeButtonText = "Yes",
                        ColorScheme = MetroDialogColorScheme.Theme,
                        NegativeButtonText = "No",
                        AnimateHide = true,
                        AnimateShow = true,
                    });

                task.ContinueWith(delegate
                {
                    if (task.Result == MessageDialogResult.Affirmative)
                    {
                        this.IsCancelled = true;
                        CustomBootstrapper.BootstrapperDispatcher.InvokeShutdown();
                    }
                });

            }
        }
        public void PlanAction(LaunchAction action)
        {
            this.Bootstrapper.Engine.Plan(action);
        }
    }
}