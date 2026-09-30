using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WeighingSolution.DataLayer;
using System.Data.Entity;
using Prism.Commands;
using MahApps.Metro.Controls.Dialogs;
using WeighingSolution.Helpers;
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Prism.Events;
using System.ComponentModel.DataAnnotations;
using Microsoft.Win32;
using System.Data.Entity.Infrastructure;
using System.IO;

namespace WeighingSolution.ViewModel
{
    public class vmSlipSetting : NotifyDataErrorInfoBase, IDisposable
    {
        private ObservableCollection<ApplicationSetting> SlipSetting;
        private DataRepository dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));
        private Page parentpage;

        #region CTOR
        public vmSlipSetting(Page page)
        {
            parentpage = page;




            SlipSetting = new ObservableCollection<ApplicationSetting>(dataRepository.Context.ApplicationSettings.Where(x => x.ModuleName == PageEntity.GlobalSetting.ToString()).ToList());
            SaveSettingCommand = new DelegateCommand(OnSettingSaveExecute, OnSettingSaveCanExecute);
            CloseDetailViewCommand = new DelegateCommand<string>(OnCloseDetailViewExecute);
        }
        #endregion

        #region Command and Properties Declaration
        public DelegateCommand SaveSettingCommand { get; private set; }
        public DelegateCommand<string> CloseDetailViewCommand { get; set; }

        private bool _hasChanges;
        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                if (_hasChanges != value)
                {
                    _hasChanges = value;
                    OnPropertyChanged();
                    ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged();
                }
            }
        }
        #endregion

        #region Binding Property
        public string EntryDateEditable
        {
            get { return SlipSetting.FirstOrDefault(x => x.SettingName == "EntryDateEditable").SettingValue; }
            set { SlipSetting.FirstOrDefault(x => x.SettingName == "EntryDateEditable").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }
        public string DisbleFrom2ndPass
        {
            get { return SlipSetting.FirstOrDefault(x => x.SettingName == "DisbleFrom2ndPass").SettingValue; }
            set { SlipSetting.FirstOrDefault(x => x.SettingName == "DisbleFrom2ndPass").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }
        public string DisplayReceiptAfter1pass
        {
            get { return SlipSetting.FirstOrDefault(x => x.SettingName == "DisplayReceiptAfter1pass").SettingValue; }
            set { SlipSetting.FirstOrDefault(x => x.SettingName == "DisplayReceiptAfter1pass").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }
        [RegularExpression("^[0-9]+$|^[0-9]*[.]{0,1}[0-9]*$", ErrorMessage = "Only decimal values are allowed")]
        [Range(0.1, 99.99, ErrorMessage = "Value must be between 0.1 to 99.99")]
        public string TareWeightDiff
        {
            get { return SlipSetting.FirstOrDefault(x => x.SettingName == "TareWeightDiff").SettingValue; }
            set { SlipSetting.FirstOrDefault(x => x.SettingName == "TareWeightDiff").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); ValidatePropertyInternal("TareWeightDiff", value); }
        }
        public string DateFormat
        {
            get { return SlipSetting.FirstOrDefault(x => x.SettingName == "DateFormat").SettingValue; }
            set { SlipSetting.FirstOrDefault(x => x.SettingName == "DateFormat").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }
        public string TimeFormat
        {
            get { return SlipSetting.FirstOrDefault(x => x.SettingName == "TimeFormat").SettingValue; }
            set { SlipSetting.FirstOrDefault(x => x.SettingName == "TimeFormat").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }
        public string DecimalFormat
        {
            get { return SlipSetting.FirstOrDefault(x => x.SettingName == "DecimalFormat").SettingValue; }
            set { SlipSetting.FirstOrDefault(x => x.SettingName == "DecimalFormat").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }
        #endregion

        #region Save
        private bool OnSettingSaveCanExecute()
        {
            return !this.HasErrors && HasChanges;
        }
        private async void OnSettingSaveExecute()
        {
            try
            {
                await dataRepository.SaveAsync();
                if (!string.IsNullOrEmpty(DateFormat) && !string.IsNullOrEmpty(TimeFormat))
                {
                    Constant.SelectedDateFormat = DateFormat;
                    Constant.SelectedTimeFormat = TimeFormat;
                }
                else if (!string.IsNullOrEmpty(DateFormat))
                    Constant.SelectedDateFormat = DateFormat;
                else if (!string.IsNullOrEmpty(TimeFormat))
                    Constant.SelectedTimeFormat = TimeFormat;
                else
                {
                    Constant.SelectedDateFormat = "dd-MM-yyyy";
                    Constant.SelectedTimeFormat = "hh:mm tt";
                }
                Constant.DateTimeFormat = Constant.SelectedDateFormat + " " + Constant.SelectedTimeFormat;
                if (!string.IsNullOrEmpty(DecimalFormat))
                    Constant.DecimalFormat = DecimalFormat;
                else
                    Constant.DecimalFormat = "0.####";
                ToastHelper.ShowSuccess("Slip information recorded successfully");
                Uri uri = new Uri("/View/SettingView/Setting_Page.Xaml", UriKind.Relative);
                parentpage.NavigationService.Navigate(uri);
            }
            catch (Exception ex)
            {
                App.Log.Error(ex.Message, ex);
                ToastHelper.ShowError("Failed to update Global information. Error: " + ex.Message);
            }
        }
        #endregion

        #region Method
        private string SavePhoto(string FullFilePath, object RemovePhotoPath)
        {
            string FileExtn = System.IO.Path.GetExtension(FullFilePath);
            string FileName = Guid.NewGuid().ToString("N");
            string NewFilePath = Constant.CustomImagesPath + @"\Logos\" + FileName + FileExtn;
            File.Copy(FullFilePath, NewFilePath, true);

            if (RemovePhotoPath != null && !String.IsNullOrEmpty(RemovePhotoPath.ToString()))
            {
                string oldPath = Constant.CustomImagesPath + @"\Logos\" + RemovePhotoPath;
                if (File.Exists(oldPath))
                    File.Delete(oldPath);
            }

            return FileName + FileExtn;
        }
        private void ModelPropertyChanged()
        {
            if (!HasChanges)
            {
                HasChanges = dataRepository.HasChanges();
            }
        }
        public void Dispose()
        {
            dataRepository.Context.Dispose();
        }
        protected async virtual void OnCloseDetailViewExecute(string NavigateURL)
        {
            if (HasChanges)
            {
                var result = await MessageBoxHelper.ShowOkCancelDialogAsync("Are you sure?", "You've made changes. Close this item?");
                if (result == MessageDialogResult.Negative)
                {
                    return;
                }
            }
            Uri uri = new Uri(NavigateURL, UriKind.Relative);
            parentpage.NavigationService.Navigate(uri);
        }
        #endregion
    }
}