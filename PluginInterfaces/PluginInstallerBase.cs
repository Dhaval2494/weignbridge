using System;
using WeighingSolution.Helpers;

namespace PluginInterfaces
{
    public abstract class PluginInstallerBase
    {
        public abstract bool IsConfigurable { get; }

        #region Abstract Methods (required to be implemented by child classes)
        public abstract bool InstallDBSchema();
        public abstract bool InstallWindowsServices();
        public abstract bool CreateFolders();
        public abstract bool RemoveDBSchema();
        public abstract bool UnInstallWindowsServices();
        public abstract bool RemoveFolders();
        public abstract bool MarkPluginEnabled();
        public abstract bool MarkPluginDisabled();
        public abstract bool StartWindowsServices();
        public abstract bool StopWindowsServices();
        #endregion

        //1. Should install supporting DB Schema
        //2. Should install supporting Windows Services
        public virtual void InstallPlugin()
        {
            try
            {
                if (InstallDBSchema() && CreateFolders() && InstallWindowsServices())
                {
                    ToastHelper.ShowSuccess("Plugin installed successfully");
                    EnablePlugin();
                }
                else
                {
                    ToastHelper.ShowError("Failed to fully install the Plugin");
                }
            }
            catch (Exception)
            {
                ToastHelper.ShowError("Failed to fully install the Plugin");
            }
        }

        //1. Set IsEnabled = true in PluginManagement DB
        //2. Should Start supporting Windows Services.
        public virtual void EnablePlugin()
        {
            try
            {
                if (StartWindowsServices() && MarkPluginEnabled())
                {
                    ToastHelper.ShowSuccess("Plugin enabled successfully");
                }
                else
                {
                    ToastHelper.ShowError("Failed to enable the Plugin");
                }
            }
            catch (Exception)
            {
                ToastHelper.ShowError("Failed to enable the Plugin");
            }
        }

        //1. Set IsEnabled = false in PluginManagement DB
        //2. Should Stop supporting Windows Services.
        public virtual void DisablePlugin()
        {
            try
            {
                if (StopWindowsServices() && MarkPluginDisabled())
                {
                    ToastHelper.ShowSuccess("Plugin disabled successfully");
                }
                else
                {
                    ToastHelper.ShowError("Failed to disable the Plugin");
                }
            }
            catch (Exception)
            {
                ToastHelper.ShowError("Failed to disable the Plugin");
            }
        }

        //1. Should stop and remove supporting Windows Services
        //2. Should remove relevant DB Schema
        public virtual void UnInstallPlugin()
        {
            try
            {
                DisablePlugin();
                if (UnInstallWindowsServices() && RemoveFolders() && RemoveDBSchema())
                {
                    ToastHelper.ShowSuccess("Plugin uninstalled successfully");
                }
                else
                {
                    ToastHelper.ShowError("Failed to fully uninstall the Plugin");
                }
            }
            catch (Exception)
            {
                ToastHelper.ShowError("Failed to fully uninstall the Plugin");
            }
        }
        public virtual void ConfigurationPlugin()
        {

        }
    }
}
