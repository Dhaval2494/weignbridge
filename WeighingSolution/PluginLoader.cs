using PluginInterfaces;
using Portable.Licensing;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows;
using WeighingSolution.DataLayer;
using WeighingSolution.SharedComponents;

namespace WeighingSolution
{
    public class PluginLoader<T>
    {
        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<Lazy<T, IPluginMetadata>> pluginControls { get; set; }

        public Dictionary<T, IPluginMetadata> loadedPlugins = new Dictionary<T, IPluginMetadata>();
        public Dictionary<T, IPluginMetadata> MakeDisbalePlugins = new Dictionary<T, IPluginMetadata>();

        public PluginLoader()
        {
            if (!Constant.IsBasicversion)
            {
                string licenseKey = "";
                License LicenseInfo = LicenseHelper.LicenseInfo;
                if (LicenseInfo == null)
                {
                    try
                    {
                        string licensepath = Constant.CustomSettingsPath + @"\license.lic";
                        if (File.Exists(licensepath))
                        {
                            licenseKey = File.ReadAllText(licensepath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Common.logErrorInDB(ex.Message, ex);
                    }
                    if (!string.IsNullOrEmpty(licenseKey))
                    {
                        string decryptedData = Common.DecryptData(licenseKey);
                        if (!string.IsNullOrEmpty(decryptedData))
                        {
                            LicenseInfo = License.Load(decryptedData);
                        }
                    }
                }
                if (LicenseInfo != null)
                {
                    App.pluginContainer.ComposeParts(this);
                    List<PluginManagement> lstPlugin = new List<PluginManagement>();
                    if (File.Exists(Constant.PluginFileLocation + @"\PluginConfiguration.dat"))
                    {
                        string Data = File.ReadAllText(Constant.PluginFileLocation + @"\PluginConfiguration.dat");
                        if (!string.IsNullOrEmpty(Data))
                        {
                            lstPlugin = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PluginManagement>>(Data);
                        }
                    }
                    foreach (var cntrl in this.pluginControls.ToList())
                    {
                        DateTime dummyDate;
                        if (LicenseInfo.AdditionalAttributes.Contains(cntrl.Metadata.PluginName))
                        {
                            string pluginExpiry = LicenseInfo.AdditionalAttributes.Get(cntrl.Metadata.PluginName);
                            if (!string.IsNullOrEmpty(pluginExpiry) && DateTime.TryParseExact(pluginExpiry, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dummyDate)
                                && dummyDate != null && dummyDate > DateTime.Now)
                            {
                                //TODO: Check if plugin signature is valid, plugin is licensed if the plugintype is paid and that plugin is compatible with this software version.
                                loadedPlugins.Add(cntrl.Value, cntrl.Metadata);
                                if (cntrl.Metadata.PluginName == "UnmannedPlugin" && lstPlugin.FirstOrDefault(x => x.PluginName == "UnmannedPlugin" && x.IsEnable) != null)
                                    Constant.hasUnmannedPlugin = true;
                            }
                            else
                            {
                                if (lstPlugin != null && lstPlugin.Count > 0 && lstPlugin.Any(x => x.PluginName == cntrl.Metadata.PluginName && x.IsEnable))
                                {
                                    MakeDisbalePlugins.Add(cntrl.Value, cntrl.Metadata);
                                    PluginInstallerBase plg = cntrl.Value as PluginInstallerBase;
                                    plg.DisablePlugin();
                                }
                            }
                        }
                        else
                        {
                            if (lstPlugin != null && lstPlugin.Count > 0 && lstPlugin.Any(x => x.PluginName == cntrl.Metadata.PluginName && x.IsEnable))
                            {
                                MakeDisbalePlugins.Add(cntrl.Value, cntrl.Metadata);
                                PluginInstallerBase plg = cntrl.Value as PluginInstallerBase;
                                plg.DisablePlugin();
                            }
                        }
                    }
                }
            }
        }

        public PluginLoader(string CompatibleScreenLayout, bool IncludeDisabled = false)
        {
            if (!Constant.IsBasicversion)
            {
                string licenseKey = "";
                License LicenseInfo = LicenseHelper.LicenseInfo;
                if (LicenseInfo == null)
                {
                    try
                    {
                        string licensepath = Constant.CustomSettingsPath + @"\license.lic";
                        if (File.Exists(licensepath))
                        {
                            licenseKey = File.ReadAllText(licensepath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Common.logErrorInDB(ex.Message, ex);
                    }
                    if (!string.IsNullOrEmpty(licenseKey))
                    {
                        string decryptedData = Common.DecryptData(licenseKey);
                        if (!string.IsNullOrEmpty(decryptedData))
                        {
                            LicenseInfo = License.Load(decryptedData);
                        }
                    }
                }
                if (LicenseInfo != null && App.pluginContainer != null)
                {
                    App.pluginContainer.ComposeParts(this);
                    weighbridgeEntities ctx = new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString());
                    List<PluginManagement> lstPlugin = new List<PluginManagement>();
                    if (File.Exists(Constant.PluginFileLocation + @"\PluginConfiguration.dat"))
                    {
                        string Data = File.ReadAllText(Constant.PluginFileLocation + @"\PluginConfiguration.dat");
                        if (!string.IsNullOrEmpty(Data))
                        {
                            lstPlugin = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PluginManagement>>(Data);
                        }
                    }
                    lstPlugin = lstPlugin.Where(x => x.IsEnable == true).ToList();
                    List<Lazy<T, IPluginMetadata>> loadablePlugins = this.pluginControls.Where(s => (string)s.Metadata.CompatibleScreenLayout == CompatibleScreenLayout).ToList();
                    foreach (var cntrl in loadablePlugins)
                    {
                        DateTime dummyDate;
                        if (LicenseInfo.AdditionalAttributes.Contains(cntrl.Metadata.PluginName))
                        {
                            string pluginExpiry = LicenseInfo.AdditionalAttributes.Get(cntrl.Metadata.PluginName);
                            if (!string.IsNullOrEmpty(pluginExpiry) && DateTime.TryParseExact(pluginExpiry, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dummyDate)
                                && dummyDate != null && dummyDate > DateTime.Now)
                            {
                                //TODO: Check if plugin signature is valid, plugin is licensed if the plugintype is paid and that plugin is compatible with this software version.
                                var TrackedPlugin = lstPlugin.FirstOrDefault(x => x.PluginName == cntrl.Metadata.PluginName);
                                if (TrackedPlugin != null && TrackedPlugin.ISInstall.GetValueOrDefault(false) && (TrackedPlugin.IsEnable || IncludeDisabled))
                                    loadedPlugins.Add(cntrl.Value, cntrl.Metadata);
                                if (cntrl.Metadata.PluginName == "UnmannedPlugin" && lstPlugin != null && lstPlugin.FirstOrDefault(x => x.PluginName == "UnmannedPlugin") != null)
                                    Constant.hasUnmannedPlugin = true;
                            }
                            //else
                            //{
                            //    if (lstPlugin != null && lstPlugin.Count > 0 && lstPlugin.Any(x => x.PluginName == cntrl.Metadata.PluginName && x.IsEnable == true))
                            //    {
                            //        MakeDisbalePlugins.Add(cntrl.Value, cntrl.Metadata);
                            //        PluginInstallerBase plg = cntrl.Value as PluginInstallerBase;
                            //        plg.DisablePlugin();
                            //    }
                            //}
                        }
                        //else
                        //{
                        //    if (lstPlugin != null && lstPlugin.Count > 0 && lstPlugin.Any(x => x.PluginName == cntrl.Metadata.PluginName && x.IsEnable == true))
                        //    {
                        //        var plg = this.pluginControls.FirstOrDefault(x => x.Metadata.PluginName == cntrl.Metadata.PluginName && x.Metadata.CompatibleScreenLayout == "PluginManagement_Page");
                        //        MakeDisbalePlugins.Add(plg.Value, plg.Metadata);
                        //        PluginInstallerBase plgCntrl = plg.Value as PluginInstallerBase;
                        //        plgCntrl.DisablePlugin();
                        //        lstPlugin.Remove(lstPlugin.FirstOrDefault(x => x.PluginName == cntrl.Metadata.PluginName));
                        //    }
                        //}
                    }
                }
            }
        }
    }
}
