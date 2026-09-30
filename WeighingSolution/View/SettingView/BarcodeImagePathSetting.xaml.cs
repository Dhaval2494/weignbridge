using MahApps.Metro.SimpleChildWindow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Forms;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for BarcodeImagePathSetting.xaml
    /// </summary>
    public partial class BarcodeImagePathSetting : ChildWindow
    {
        public weighbridgeEntities ctxBarcode = new weighbridgeEntities(System.Windows.Application.Current.Properties["EntityConnectionString"].ToString());
        List<ApplicationSetting> lstBarcodeSetting;
        private string BarcodeImagePathType = "";
        private string BarcodeImagePath = "";
        public BarcodeImagePathSetting()
        {
            InitializeComponent();
            lstBarcodeSetting = ctxBarcode.ApplicationSettings.Where(x => x.ModuleName == "BarcodeSetting").ToList();
            if (lstBarcodeSetting != null)
            {
                BarcodeImagePathType = lstBarcodeSetting.FirstOrDefault(x => x.SettingName == "BarcodeImagePathType").SettingValue;
                BarcodeImagePath = lstBarcodeSetting.FirstOrDefault(x => x.SettingName == "BarcodeImagePath").SettingValue;
                if (BarcodeImagePathType != "")
                {
                    if (BarcodeImagePathType == "LocalImagePath" && BarcodeImagePath != "")
                    {
                        rbLocalImgPath.IsChecked = true;
                        rbNetworkImgPath.IsChecked = false;
                        stkLocalImgpath.Visibility = Visibility.Visible;
                        stkNetworkImgpath.Visibility = Visibility.Collapsed;
                        lblFilePath.Content = BarcodeImagePath;
                    }
                    if (BarcodeImagePathType == "NetworkImagePath" && BarcodeImagePath != "")
                    {
                        rbLocalImgPath.IsChecked = false;
                        rbNetworkImgPath.IsChecked = true;
                        stkLocalImgpath.Visibility = Visibility.Collapsed;
                        stkNetworkImgpath.Visibility = Visibility.Visible;
                        txtFilePath.Text = BarcodeImagePath;
                    }
                }
            }
        }

        private void btnBrowseFile_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog Folderpath = new FolderBrowserDialog();
            if (Folderpath.ShowDialog() == DialogResult.OK)
            {
                DirectoryInfo di = new DirectoryInfo(Folderpath.SelectedPath);
                DirectorySecurity security = di.GetAccessControl();
                security.AddAccessRule(new FileSystemAccessRule(@"Users", FileSystemRights.Modify, AccessControlType.Allow));
                di.SetAccessControl(security);
                lblFilePath.Content = Folderpath.SelectedPath;
            }
        }

        private void btnBrowseNetworkFile_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog Folderpath = new FolderBrowserDialog();
            if (Folderpath.ShowDialog() == DialogResult.OK)
            {
                //DirectoryInfo di = new DirectoryInfo(Folderpath.SelectedPath);
                //DirectorySecurity security = di.GetAccessControl();
                //security.AddAccessRule(new FileSystemAccessRule(@"Users", FileSystemRights.Modify, AccessControlType.Allow));
                //di.SetAccessControl(security);
                txtFilePath.Text = Folderpath.SelectedPath;
            }
        }

        private void btnSavePath_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool IsSuccess = false;
                if (rbLocalImgPath.IsChecked.Value)
                {
                    if (!string.IsNullOrEmpty(lblFilePath.Content.ToString()))
                    {
                        BarcodeImagePathType = "LocalImagePath";
                        BarcodeImagePath = lblFilePath.Content.ToString();
                        IsSuccess = true;
                    }
                    else
                        ToastHelper.ShowWarning("Please browse image file path");
                }
                if (rbNetworkImgPath.IsChecked.Value)
                {
                    if (Directory.Exists(txtFilePath.Text))
                    {
                        DirectoryInfo di = new DirectoryInfo(txtFilePath.Text);
                        DirectorySecurity security = di.GetAccessControl();
                        security.AddAccessRule(new FileSystemAccessRule(@"Users", FileSystemRights.Modify, AccessControlType.Allow));
                        di.SetAccessControl(security);
                        BarcodeImagePathType = "NetworkImagePath";
                        BarcodeImagePath = txtFilePath.Text;
                        IsSuccess = true;
                    }
                    else
                        ToastHelper.ShowWarning("Image file path not exists");
                }
                if (IsSuccess)
                {
                    lstBarcodeSetting.FirstOrDefault(x => x.SettingName == "BarcodeImagePathType").SettingValue = BarcodeImagePathType;
                    lstBarcodeSetting.FirstOrDefault(x => x.SettingName == "BarcodeImagePath").SettingValue = BarcodeImagePath;
                    ctxBarcode.SaveChanges();
                    ToastHelper.ShowSuccess("Barcode image path saved successfully");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                ToastHelper.ShowError("Fail to save barcode imagepath:" + ex.Message);
            }
        }

        private void rbLocalImgPath_Checked(object sender, RoutedEventArgs e)
        {
            rbNetworkImgPath.IsChecked = false;
            stkLocalImgpath.Visibility = Visibility.Visible;
            stkNetworkImgpath.Visibility = Visibility.Collapsed;
            if (BarcodeImagePathType == "LocalImagePath")
                lblFilePath.Content = BarcodeImagePath;
            else
                lblFilePath.Content = "";
        }
        private void rbNetworkImgPath_Checked(object sender, RoutedEventArgs e)
        {
            rbLocalImgPath.IsChecked = false;
            stkLocalImgpath.Visibility = Visibility.Collapsed;
            stkNetworkImgpath.Visibility = Visibility.Visible;
            if (BarcodeImagePathType == "NetworkImagePath")
                txtFilePath.Text = BarcodeImagePath;
            else
                txtFilePath.Text = "";
        }


    }
}
