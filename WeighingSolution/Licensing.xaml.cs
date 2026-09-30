using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Portable.Licensing;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;

namespace WeighingSolution
{
    /// <summary>
    /// Interaction logic for Licensing.xaml
    /// </summary>
    public partial class Licensing : MetroWindow
    {
        public Licensing()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            ToastHelper.InitToaster();
            lblHardwareID.Text = WhoAmI.fingerPrintValue();
            Application.Current.MainWindow = this;
        }

        public Licensing(string Licence)
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            ToastHelper.InitToaster();
            lblHardwareID.Text = WhoAmI.fingerPrintValue();
            Application.Current.MainWindow = this;

            txtSignatureKey.Text = Licence;
        }

        private void lnkCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(lblHardwareID.Text);
            ToastHelper.ShowSuccess("Serial no copied to clipboard");
        }

        private async void btnVarify_Click(object sender, RoutedEventArgs e)
        {
            await VerifyLicense();
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
                        await MessageBoxHelper.ShowInfoDialogAsync("Warning", "License installation cancelled.\nThe application will now close.");
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
    }
}