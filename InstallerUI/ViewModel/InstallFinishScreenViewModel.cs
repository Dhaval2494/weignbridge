using InstallerUI.Helpers;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace InstallerUI.ViewModel
{
    public class InstallFinishScreenViewModel : ViewModelBase
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, int flags);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hwnd);

        public InstallFinishScreenViewModel(MainViewModel mainViewModel)
            : base(mainViewModel)
        {
            SetForegroundWindow(MainViewModel.Hwnd);
        }
        public ICommand CloseCommand
        {
            get
            {
                return new BaseCommand(delegate
                  {
                      if (LaunchApplication)
                      {
                          Process process = new Process();
                          // Configure the process using the StartInfo properties.
                          process.StartInfo.Verb = "runas";
                          process.StartInfo.FileName = ReadFormattedVariableFromBundle("InstallDir") + "WeighingSolution.exe";
                          process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                          process.Start();
                          process.WaitForInputIdle(500);
                          // check if the window is hidden / minimized
                          if (process.MainWindowHandle == IntPtr.Zero)
                          {
                              // the window is hidden so try to restore it before setting focus.
                              //https://stackoverflow.com/questions/2315561/correct-way-in-net-to-switch-the-focus-to-another-application
                              //9 = ShowWindowEnum.Restore
                              ShowWindow(process.Handle, 9);
                          }
                          // set user the focus to the window
                          SetForegroundWindow(process.MainWindowHandle);
                      }
                      System.Environment.Exit(0);
                  }, o => true);
            }
        }
        private string _screentitle = "Installation completed successfully";
        public string ScreenTitle
        {
            get { return _screentitle; }
            set { _screentitle = value; OnPropertyChanged(); }
        }
        private string _finishmessage = "You are now ready to setup the environment";
        public string FinishMessage
        {
            get { return _finishmessage; }
            set { _finishmessage = value; OnPropertyChanged(); }
        }
        private bool _launchapplication = true;
        public bool LaunchApplication
        {
            get { return _launchapplication; }
            set { _launchapplication = value; OnPropertyChanged(); }
        }
        private bool _showlaunchapplication = true;
        public bool ShowLaunchApplication
        {
            get { return _showlaunchapplication; }
            set { _showlaunchapplication = value; OnPropertyChanged(); }
        }

    }
}