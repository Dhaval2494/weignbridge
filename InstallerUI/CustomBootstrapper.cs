using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System.Windows.Threading;

namespace InstallerUI
{
    public class CustomBootstrapper : BootstrapperApplication
    {
        static public Dispatcher BootstrapperDispatcher { get; private set; }

        protected override void Run()
        {
            //#if DEBUG
            //            Debugger.Launch();
            //#endif
            this.Engine.Log(LogLevel.Verbose, "Launching Installation Interface");
            BootstrapperDispatcher = Dispatcher.CurrentDispatcher;

            var mainWindow = new InstallerUI.View.MainWindow(this);
            mainWindow.Closed += delegate
            {
                BootstrapperDispatcher.InvokeShutdown();
            };

            this.Engine.Detect();
            //base.Engine.CloseSplashScreen();
            mainWindow.Show();
            Dispatcher.Run();
            this.Engine.Quit(mainWindow.ViewModel.FinalResult);
        }
    }
}
