using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using PluginInterfaces;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Windows;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;
using WeighingSolution.SharedComponents;

namespace WeighingSolution.ViewModel
{
    class vmPluginManagementList : DataSetupListViewModelBase<PluginManagement>
    {
        public PluginLoader<PluginInstallerBase> mef;
        public DelegateCommand<PluginManagement> InstallCommand { get; private set; }
        public DelegateCommand<PluginManagement> UnInstallCommand { get; private set; }
        public DelegateCommand<PluginManagement> EnableDisableCommand { get; private set; }
        public DelegateCommand<PluginManagement> PluginsettingCommand { get; private set; }
        public DelegateCommand<String> SelectFile { get; private set; }

        public vmPluginManagementList()
        {
            dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));
            InstallCommand = new DelegateCommand<PluginManagement>(OnInstallCommand);
            UnInstallCommand = new DelegateCommand<PluginManagement>(OnUnInstallCommand);
            EnableDisableCommand = new DelegateCommand<PluginManagement>(OnEnableDisableCommand);
            PluginsettingCommand = new DelegateCommand<PluginManagement>(OnPluginsettingCommand);
            SelectFile = new DelegateCommand<String>(OnSelectFile);
        }

        private void OnPluginsettingCommand(PluginManagement obj)
        {
            PluginInstallerBase plugin = mef.loadedPlugins.FirstOrDefault(x => x.Value.PluginName == obj.PluginName).Key;
            if (plugin.IsConfigurable)
                plugin.ConfigurationPlugin();
        }

        private async void OnSelectFile(String ImageSource)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = false;
            open.Filter = "Weighing Solution Plugin Files(.vplug)|*.vplug";
            bool? result = open.ShowDialog();

            if (result == true)
            {
                if (!string.IsNullOrEmpty(open.FileName))
                {
                    bool Isbuilt_in = false;
                    string FileName = Path.GetFileNameWithoutExtension(open.FileName).ToLower();

                    if (FileName == PluginNameEnum.AutoBackupPlugin.ToString().ToLower())
                        Isbuilt_in = true;
                    if (FileName == PluginNameEnum.EmailPlugin.ToString().ToLower())
                        Isbuilt_in = true;
                    if (FileName == PluginNameEnum.MultiWeighBridgePlugin.ToString().ToLower())
                        Isbuilt_in = true;
                    if (FileName == PluginNameEnum.SMSPlugin.ToString().ToLower())
                        Isbuilt_in = true;
                    if (FileName == PluginNameEnum.BarcodePlugin.ToString().ToLower())
                        Isbuilt_in = true;
                    if (FileName == PluginNameEnum.CameraPlugin.ToString().ToLower())
                        Isbuilt_in = true;
                    if (FileName == PluginNameEnum.SyncPlugin.ToString().ToLower())
                        Isbuilt_in = true;

                    if (Isbuilt_in)
                    {
                        await MessageBoxHelper.ShowInfoDialogAsync("Infomation", FileName + " is built-in function. It can not be install. ");
                        return;
                    }

                    ZipArchiveHelper.ExtractFile(open.FileName, Constant.PluginFileLocation);
                    AggregateCatalog ctg = (AggregateCatalog)App.pluginContainer.Catalog;
                    ctg.Catalogs.Add(new DirectoryCatalog(Constant.PluginFileLocation + "\\" + Path.GetFileNameWithoutExtension(open.FileName)));
                    LoadPlugin();
                    ViewList.View.Refresh();
                    ToastHelper.ShowSuccess("Plugin recorded successfully");
                }
                else
                {
                    await MessageBoxHelper.ShowInfoDialogAsync("Error!!!", "Please browse plugin file to upload");
                    return;
                }
            }
        }
        public async void OnInstallCommand(PluginManagement objPluginmgmt)
        {
            PluginInstallerBase plugin = mef.loadedPlugins.FirstOrDefault(x => x.Value.PluginName == objPluginmgmt.PluginName).Key;
            MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Are you sure?", "You want to install this plugin?");
            if (msgbox_result == MessageDialogResult.Affirmative)
            {
                plugin.InstallPlugin();

                LoadPlugin();
                ViewList.View.Refresh();
            }
        }
        public async void OnUnInstallCommand(PluginManagement objPluginmgmt)
        {
            PluginInstallerBase plugin = mef.loadedPlugins.FirstOrDefault(x => x.Value.PluginName == objPluginmgmt.PluginName).Key;
            MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Are you sure?",
                "You want to uninstall this plugin?\nIMPORTANT: This will result in application to restart");
            if (msgbox_result == MessageDialogResult.Affirmative)
            {
                plugin.UnInstallPlugin();
                AggregateCatalog ctg = (AggregateCatalog)App.pluginContainer.Catalog;
                DirectoryCatalog ctgDelete = null;
                foreach (var item in ctg.Catalogs)
                {
                    DirectoryCatalog dirctg = item as DirectoryCatalog;
                    if (dirctg != null && dirctg.Path.ToLower() == (Constant.PluginFileLocation + "\\" + objPluginmgmt.PluginName).ToLower())
                    {
                        ctgDelete = dirctg;
                        break;
                    }
                }
                if (ctgDelete != null)
                    ctg.Catalogs.Remove(ctgDelete);

                LoadPlugin();
                ViewList.View.Refresh();

                //Restart the application and remove the plugin folder
                App.RestartApplication("RemovePlugin=" + objPluginmgmt.PluginName, true);
            }
        }
        public async void OnEnableDisableCommand(PluginManagement objPluginmgmt)
        {
            PluginInstallerBase plugin = mef.loadedPlugins.FirstOrDefault(x => x.Value.PluginName == objPluginmgmt.PluginName).Key;
            MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Are you sure?", "You want to " + (objPluginmgmt.IsEnable ? "disable" : "enable") + " this plugin?");
            if (msgbox_result == MessageDialogResult.Affirmative)
            {
                if (objPluginmgmt.IsEnable)
                {
                    plugin.DisablePlugin();
                    if (objPluginmgmt.PluginName == "UnmannedPlugin")
                        Constant.hasUnmannedPlugin = false;
                }
                else
                {
                    plugin.EnablePlugin();
                    if (objPluginmgmt.PluginName == "UnmannedPlugin")
                        Constant.hasUnmannedPlugin = true;
                }
                LoadPlugin();
                ViewList.View.Refresh();
            }
        }
        public void LoadPlugin()
        {
            mef = new PluginLoader<PluginInstallerBase>();

            if (AllObjectList != null)
                AllObjectList.Clear();
            AllObjectList = new ObservableCollection<PluginManagement>();
            List<PluginManagement> lstPlugin = new List<PluginManagement>();
            if (File.Exists(Constant.PluginFileLocation + @"\PluginConfiguration.dat"))
            {
                string Data = File.ReadAllText(Constant.PluginFileLocation + @"\PluginConfiguration.dat");
                if (!string.IsNullOrEmpty(Data))
                {
                    lstPlugin = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PluginManagement>>(Data);
                }
            }
            var TrackedPlugins = lstPlugin;
            foreach (var plugin in mef.loadedPlugins)
            {
                var plug = new PluginManagement();
                plug.PluginName = plugin.Value.PluginName;
                plug.ISInstall = false;
                var TrackedPlugin = TrackedPlugins.FirstOrDefault(x => x.PluginName == plug.PluginName);
                if (TrackedPlugin != null)
                {
                    plug.IsEnable = TrackedPlugin.IsEnable;
                    plug.ISInstall = TrackedPlugin.ISInstall;
                }
                plug.IsConfigur = (plug.ISInstall.Value && plug.IsEnable && plugin.Key.IsConfigurable);
                AllObjectList.Add(plug);
            }
            OnFilterData();
        }
        public override void OnFilterData()
        {
            if (!String.IsNullOrEmpty(SearchString))
            {
                FilteredObjectList = new ObservableCollection<PluginManagement>(AllObjectList.Where(x => (!string.IsNullOrEmpty(x.PluginName) && x.PluginName.ToLower().Contains(SearchString.ToLower()))).ToList());
            }
            else
            {
                FilteredObjectList = AllObjectList;
            }
            CurrentPageIndex = 0;
            CalculateTotalPages();
            ViewList.Source = FilteredObjectList;
            ViewList.View.Refresh();
        }
    }
}