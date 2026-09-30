using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using WeighingSolution.DataLayer;

namespace WeighingSolution
{
    public class LicensePluginHalper
    {
        public static bool CheckPluginInstall(string PluginName)
        {

            string licenseKey = string.Empty;
            Portable.Licensing.License LicenseInfo = LicenseHelper.LicenseInfo;
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
                        LicenseInfo = Portable.Licensing.License.Load(decryptedData);
                    }
                }
            }
            if (LicenseInfo != null && App.pluginContainer != null)
            {
                string listplugin = LicenseInfo.AdditionalAttributes.Get(PluginName);
                if (!string.IsNullOrEmpty(listplugin))
                    return true;
            }

            return false;
        }
    }

    public static class GetControll
    {
        public static T GetChildOfType<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }
    }
}
