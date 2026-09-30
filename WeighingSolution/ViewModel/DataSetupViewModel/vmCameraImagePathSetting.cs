using MahApps.Metro.SimpleChildWindow;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Windows.Forms;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;

namespace WeighingSolution.ViewModel
{
    public class vmCameraImagePathSetting : WeighingSolution.SharedComponents.NotifyPropertyChanged, IDisposable
    {
        public DataRepository CameradataRepository;
        public List<ApplicationSetting> CameraSetting;
        ChildWindow childWindow;
        private string Camimgpathtype;
        private string Camimg;

        public vmCameraImagePathSetting(ChildWindow cw)
        {
            childWindow = cw;
            CameradataRepository = new DataRepository(new weighbridgeEntities(Constant.EntityConnectionString));
            CameraSetting = CameradataRepository.Context.ApplicationSettings.Where(x => x.ModuleName == "CameraSetting").ToList();
            SelectFile = new DelegateCommand<String>(OnSelectFile);
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            cmdRadioChecked = new DelegateCommand<string>(oncmdRadioChecked);
            Camimgpathtype = CameraSetting.FirstOrDefault(x => x.SettingName == "CameraImagePathType").SettingValue;
            Camimg = CameraSetting.FirstOrDefault(x => x.SettingName == "CameraImagePath").SettingValue;
        }


        public DelegateCommand<String> SelectFile { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand<string> cmdRadioChecked { get; private set; }

        public string BrowseFile
        {
            get { return CameraSetting.FirstOrDefault(x => x.SettingName == "CameraImagePath").SettingValue; }
            set
            {
                if (CameraSetting.FirstOrDefault(x => x.SettingName == "CameraImagePath").SettingValue != value)
                {
                    haschange = true;
                    SaveCommand.RaiseCanExecuteChanged();
                }
                CameraSetting.FirstOrDefault(x => x.SettingName == "CameraImagePath").SettingValue = value; OnPropertyChanged();
            }
        }

        private bool haschange;
        public bool HasChange
        {
            get { return haschange; }
            set { haschange = value; OnPropertyChanged(); }
        }

        public string CameraImagePath
        {
            get { return CameraSetting.FirstOrDefault(x => x.SettingName == "CameraImagePath").SettingValue; }
            set
            {
                if (CameraSetting.FirstOrDefault(x => x.SettingName == "CameraImagePath").SettingValue != value)
                {
                    haschange = true;
                    SaveCommand.RaiseCanExecuteChanged();
                }
                CameraSetting.FirstOrDefault(x => x.SettingName == "CameraImagePath").SettingValue = value; OnPropertyChanged();
            }
        }

        public string CameraImagePathType
        {
            get { return CameraSetting.FirstOrDefault(x => x.SettingName == "CameraImagePathType").SettingValue; }
            set { CameraSetting.FirstOrDefault(x => x.SettingName == "CameraImagePathType").SettingValue = value; OnPropertyChanged(); }
        }
        private void oncmdRadioChecked(string obj)
        {
            if (obj == "LocalImagePath")
            {
                if (Camimgpathtype == "LocalImagePath")
                    BrowseFile = Camimg;
                else
                    BrowseFile = "";
                CameraImagePathType = "LocalImagePath";
            }
            if (obj == "NetworkImagePath")
            {
                if (Camimgpathtype == "NetworkImagePath")
                    CameraImagePath = Camimg;
                else
                    CameraImagePath = "";
                CameraImagePathType = "NetworkImagePath";
            }
        }
        private bool OnSaveCanExecute()
        {
            return haschange;
        }

        private void OnSelectFile(string obj)
        {
            FolderBrowserDialog Folderpath = new FolderBrowserDialog();
            if (Folderpath.ShowDialog() == DialogResult.OK)
            {
                DirectoryInfo di = new DirectoryInfo(Folderpath.SelectedPath);
                DirectorySecurity security = di.GetAccessControl();
                security.AddAccessRule(new FileSystemAccessRule(@"Users", FileSystemRights.Modify, AccessControlType.Allow));
                di.SetAccessControl(security);
                BrowseFile = Folderpath.SelectedPath;
            }
        }
        private void OnSaveExecute()
        {
            try
            {
                bool IsSuccess = false;
                if (CameraImagePathType == "NetworkImagePath")
                {
                    if (Directory.Exists(CameraImagePath))
                        IsSuccess = true;
                    else
                        ToastHelper.ShowWarning("Image file path not exist.");
                }
                if (CameraImagePathType == "LocalImagePath")
                {
                    if (!string.IsNullOrEmpty(BrowseFile))
                        IsSuccess = true;
                    else
                        ToastHelper.ShowWarning("Please browse image file path.");
                }
                if (IsSuccess)
                {
                    CameradataRepository.Context.SaveChanges();
                    ToastHelper.ShowSuccess("Camera image path saved successfully");
                    if (CameraSetting != null && CameraSetting.FirstOrDefault(x => x.SettingName == "CameraImagePath").SettingValue != "")
                        CameraConstant.CameraImageLocation = CameraSetting.FirstOrDefault(x => x.SettingName == "CameraImagePath").SettingValue;
                    childWindow.Close();
                }
            }
            catch (Exception ex)
            {
                ToastHelper.ShowError("Fail to save camera imagepath:" + ex.Message);
            }
        }
        public void Dispose()
        {
            CameradataRepository.Context.Dispose();
        }
    }
}
