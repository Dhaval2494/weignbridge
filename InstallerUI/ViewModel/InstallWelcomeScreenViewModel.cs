using InstallerUI.Helpers;
using InstallerUI.View;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;

namespace InstallerUI.ViewModel
{
    public class InstallWelcomeScreenViewModel : ViewModelBase
    {
        public InstallWelcomeScreenViewModel(MainViewModel mainViewModel) : base(mainViewModel)
        {
            CheckAllPrerequisites();
        }
        public ICommand InstallCommand
        {
            get
            {
                return new BaseCommand(delegate
                {
                    this.Bootstrapper.Engine.Log(LogLevel.Standard, "Initializing ProgressPage");
                    this.MainViewModel.CurrentControl = new InstallProgressScreen(this.MainViewModel);
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
        public bool InstallSQL
        {
            get { return ReadVariableFromBundle("InstallSQLx64") == "ALL" && ReadVariableFromBundle("InstallSQLx86") == "ALL"; }
            set
            {
                if (value)
                {
                    WriteVariableToBundle("InstallSQLx64", "ALL");
                    WriteVariableToBundle("InstallSQLx86", "ALL");
                }
                else
                {
                    WriteVariableToBundle("InstallSQLx64", "NONE");
                    WriteVariableToBundle("InstallSQLx86", "NONE");
                }
                OnPropertyChanged();

                if (value)
                {
                    IsDotNet35Enabled = CheckDotNet35();
                    IsFirewallEnabled = CheckFirewall();
                }
                else
                {
                    IsDotNet35Enabled = true;
                    IsFirewallEnabled = true;
                }
            }
        }

        //--------------------------------  Prerequisite properties----------------------------
        private bool _isDotNet48Installed;
        public bool IsDotNet48Installed
        {
            get => _isDotNet48Installed;
            set
            {
                _isDotNet48Installed = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DotNet48Color));
                OnPropertyChanged(nameof(AllChecksPassed));
                OnPropertyChanged(nameof(CheckMessage));
            }
        }

        private bool _isDotNet35Enabled;
        public bool IsDotNet35Enabled
        {
            get => _isDotNet35Enabled;
            set
            {
                _isDotNet35Enabled = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DotNet35Color));
                OnPropertyChanged(nameof(AllChecksPassed));
                OnPropertyChanged(nameof(CheckMessage));
            }
        }

        private bool _isFirewallEnabled;
        public bool IsFirewallEnabled
        {
            get => _isFirewallEnabled;
            set
            {
                _isFirewallEnabled = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FirewallColor));
                OnPropertyChanged(nameof(AllChecksPassed));
                OnPropertyChanged(nameof(CheckMessage));
            }
        }

        private bool _isWindowsVersionSupported;
        public bool IsWindowsVersionSupported
        {
            get => _isWindowsVersionSupported;
            set
            {
                _isWindowsVersionSupported = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(WindowsVersionColor));
                OnPropertyChanged(nameof(AllChecksPassed));
                OnPropertyChanged(nameof(CheckMessage));
            }
        }


        public Brush DotNet48Color => IsDotNet48Installed ? Brushes.Green : Brushes.Red;
        public Brush DotNet35Color => IsDotNet35Enabled ? Brushes.Green : Brushes.Red;
        public Brush FirewallColor => IsFirewallEnabled ? Brushes.Green : Brushes.Red;
        public Brush WindowsVersionColor => IsWindowsVersionSupported ? Brushes.Green : Brushes.Red;

        public string CheckMessage => AllChecksPassed ? "✅ All system checks passed. Ready to install." : "❌ Some prerequisites are missing.";
        public bool AllChecksPassed =>
            IsDotNet48Installed &&
            IsDotNet35Enabled &&
            IsFirewallEnabled &&
            IsWindowsVersionSupported;

        private void CheckAllPrerequisites()
        {
            IsDotNet48Installed = CheckDotNet48();
            IsDotNet35Enabled = !InstallSQL ? true : CheckDotNet35();
            IsFirewallEnabled = !InstallSQL ? true : CheckFirewall();
            IsWindowsVersionSupported = CheckWindowsVersion();
        }

        private bool CheckDotNet48()
        {
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
            using (RegistryKey ndpKey = Registry.LocalMachine.OpenSubKey(subkey))
            {
                if (ndpKey?.GetValue("Release") is int releaseKey)
                {
                    return releaseKey >= 528040;
                }
            }
            return false;
        }

        private bool CheckDotNet35()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5"))
                {
                    if (key != null)
                    {
                        object install = key.GetValue("Install");
                        return install != null && (int)install == 1;
                    }
                }
            }
            catch { }

            return false;
        }

        private bool CheckFirewall()
        {
            try
            {
                var psi = new ProcessStartInfo("powershell", "-Command \"(Get-NetFirewallProfile | Where-Object {$_.Enabled -eq $true}).Count\"")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using (var process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd().Trim();
                    return int.TryParse(output, out int count) && count > 0;
                }
            }
            catch { return false; }
        }

        private bool CheckWindowsVersion()
        {
            Version os = Environment.OSVersion.Version;
            return os.Major >= 10;
        }
    }
}