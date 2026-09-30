using MahApps.Metro.Controls.Dialogs;
using Portable.Licensing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;
using WeighingSolution.SharedComponents;

namespace WeighingSolution.View.SettingView
{
    /// <summary>
    /// Interaction logic for LicenseManagement_Page.xaml
    /// </summary>
    public partial class LicenseManagement_Page : Page
    {
        public LicenseManagement_Page()
        {
            InitializeComponent();
            this.Loaded += CurrentPage_Loaded;
        }


        private void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            string licenseKey = "";
            string licensepath = Constant.CustomSettingsPath + @"\license.lic";
            if (File.Exists(licensepath))
                licenseKey = File.ReadAllText(licensepath);
            if (!string.IsNullOrEmpty(licenseKey))
            {
                string decryptedData = Common.DecryptData(licenseKey);
                License LicenseInfo = License.Load(decryptedData);
                if (LicenseInfo != null)
                {
                    lblAssignTo.Content = LicenseInfo.Customer.Name + " (" + LicenseInfo.ProductFeatures.Get("SightName").ToString() + ")";
                    lblLicenseType.Content = LicenseInfo.Type.ToString();
                    lblsoftwaretype.Content = LicenseInfo.ProductFeatures.Get("SoftwareType").ToString();
                    lblExpireOn.Content = LicenseInfo.Expiration.ToString(Constant.SelectedDateFormat, CultureInfo.InvariantCulture);   //Constant.FormateDateTimeOutString(LicenseInfo.Expiration); 
                    lblHardwareID.Content = WhoAmI.fingerPrintValue();
                    if (Constant.AmcExpire != null)
                    {
                        lblamcexpire.Visibility = Visibility.Visible;
                        txtamcexpire.Visibility = Visibility.Visible;
                        txtamcexpire.Content = Constant.AmcExpire.ToString(Constant.SelectedDateFormat, CultureInfo.InvariantCulture);
                    }


                    if (!Constant.IsBasicversion)
                    {
                        List<IssuedPlugin> IncludedPlugins = new List<IssuedPlugin>();
                        foreach (KeyValuePair<string, string> item in LicenseInfo.AdditionalAttributes.GetAll())
                        {
                            DateTime dummyDate;
                            if (!string.IsNullOrEmpty(item.Value) && DateTime.TryParseExact(item.Value, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dummyDate))
                            {
                                IssuedPlugin objPlugin = new IssuedPlugin();
                                objPlugin.PluginName = item.Key;
                                objPlugin.ExpiredOn = dummyDate.ToString(Constant.SelectedDateFormat, CultureInfo.InvariantCulture);    //Constant.FormateDateTimeOutString(dummyDate);
                                IncludedPlugins.Add(objPlugin);
                            }
                        }
                        if (IncludedPlugins.Count > 0)
                        {
                            drgPlugin.ItemsSource = IncludedPlugins;
                            drgPlugin.Visibility = Visibility.Visible;
                            lblNoPlugin.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            lblNoPlugin.Visibility = Visibility.Visible;
                            drgPlugin.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
            else
                ToastHelper.ShowError("License file is not exist");
        }

        private async void btnUploadFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
            fileDialog = new System.Windows.Forms.OpenFileDialog { Filter = "License Files(.lic)|*.lic" };
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && !string.IsNullOrEmpty(fileDialog.FileName))
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(fileDialog.FileName);
                txtSignatureKey.Text = sr.ReadToEnd();
                sr.Close();
                lblStatus.Text = "Validating...";
                lblStatus.Visibility = Visibility.Visible;
                await VerifyLicense();
            }
        }

        private void rdbLicenseKey_Checked(object sender, RoutedEventArgs e)
        {
            if (rdbUploadFile != null && rdbUploadFile.IsChecked.GetValueOrDefault(false))
            {
                if (stkLicenseFile != null)
                    stkLicenseFile.Visibility = Visibility.Visible;
                if (stkSignatureKey != null)
                    stkSignatureKey.Visibility = Visibility.Collapsed;
                if (stkVerify != null)
                    stkVerify.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (stkLicenseFile != null)
                    stkLicenseFile.Visibility = Visibility.Collapsed;
                if (stkSignatureKey != null)
                    stkSignatureKey.Visibility = Visibility.Visible;
                if (stkVerify != null)
                    stkVerify.Visibility = Visibility.Visible;
            }
        }

        private void rdbUploadFile_Checked(object sender, RoutedEventArgs e)
        {
            if (rdbUploadFile.IsChecked.GetValueOrDefault(false))
            {
                if (stkLicenseFile != null)
                    stkLicenseFile.Visibility = Visibility.Visible;
                if (stkSignatureKey != null)
                    stkSignatureKey.Visibility = Visibility.Collapsed;
                if (stkVerify != null)
                    stkVerify.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (stkLicenseFile != null)
                    stkLicenseFile.Visibility = Visibility.Collapsed;
                if (stkSignatureKey != null)
                    stkSignatureKey.Visibility = Visibility.Visible;
                if (stkVerify != null)
                    stkVerify.Visibility = Visibility.Visible;
            }
        }

        private void lnkCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(lblHardwareID.Content.ToString());
            ToastHelper.ShowSuccess("Serial no copied to clipboard");
        }

        private async void btnRenewLicense_Click(object sender, RoutedEventArgs e)
        {
            await VerifyLicense();
        }
        private async Task VerifyLicense()
        {
            bool isLicenseValid = false;
            try
            {
                if (!string.IsNullOrEmpty(txtSignatureKey.Text))
                {
                    string LicenseCheckResult = await LicenseHelper.ValidateLicense(txtSignatureKey.Text);
                    if (LicenseCheckResult != "Success")
                    {
                        ToastHelper.ShowError("License key is not valid.\n" + LicenseCheckResult);
                        lblStatus.Visibility = Visibility.Collapsed;
                        return;
                    }
                    else
                        isLicenseValid = true;
                }
                else
                {
                    ToastHelper.ShowError("License key is required");
                    lblStatus.Visibility = Visibility.Collapsed;
                    return;
                }
            }
            catch (Exception ex)
            {
                Common.logErrorInDB(ex.Message, ex);
                ToastHelper.ShowError("License key is not valid");
            }

            if (isLicenseValid)
            {
                try
                {
                    MessageDialogResult result = MessageDialogResult.Affirmative;
                    //File.Exists(Constant.CustomSettingsPath + @"\Configuration.dat") &&
                    if (LicenseHelper.LicenseInfo.Type == LicenseType.Trial)
                    {
                        result = await MessageBoxHelper.ShowYesNoDialog("Install Trial License?", "You are installing a trial license.\nThis will recreate fresh database.\nContinue installing trial?");
                        if (result == MessageDialogResult.Affirmative)
                            LicenseHelper.CleanupData();
                    }

                    if (result == MessageDialogResult.Affirmative)
                    {
                        string licensepath = Constant.CustomSettingsPath + @"\license.lic";
                        File.WriteAllText(licensepath, txtSignatureKey.Text);
                        Common.SetFullAccessForFile(licensepath);

                        await MessageBoxHelper.ShowInfoDialogAsync("Success", "License information saved.\nThe application will now restart.");
                        App.RestartApplication(RunAsAdministrator: true);
                    }
                    else
                    {
                        await MessageBoxHelper.ShowInfoDialogAsync("Success", "License installation cancelled.\nThe application will now close.");
                        System.Windows.Application.Current.Shutdown();
                    }
                }
                catch (Exception ex)
                {
                    ToastHelper.ShowError("Error Updating License Information. " + ex.Message);
                }
            }
            else
            {
                ToastHelper.ShowError("Unable to validate entered license");
            }
        }

        private void LicenseCancel_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/SettingView/Setting_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
    }
}
