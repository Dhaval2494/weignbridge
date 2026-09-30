using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;
using WeighingSolution.SharedComponents;

namespace WeighingSolution.ViewModel.SettingViewModel
{
    class vmReceiptSetting : NotifyDataErrorInfoBase
    {
        private Page parentpage;
        private ObservableCollection<ApplicationSetting> SlipSetting;
        private DataRepository dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));

        #region CTOR
        public vmReceiptSetting(Page page)
        {
            parentpage = page;
            SetupReceiptList();
            SlipSetting = new ObservableCollection<ApplicationSetting>(dataRepository.Context.ApplicationSettings.Where(x => x.ModuleName == PageEntity.GlobalSetting.ToString()).ToList());
            CloseDetailViewCommand = new DelegateCommand<string>(OnCloseDetailViewExecute);
            SaveSettingCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);


        }
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

        #region Save
        private bool OnSaveCanExecute()
        {
            return !this.HasErrors && HasChanges;
        }

        private async void OnSaveExecute()
        {
            try
            {
                await dataRepository.SaveAsync();
                ToastHelper.ShowSuccess("Slip information recorded successfully");
                Uri uri = new Uri("/View/SettingView/Setting_Page.Xaml", UriKind.Relative);
                parentpage.NavigationService.Navigate(uri);
            }
            catch (Exception ex)
            {
                App.Log.Error(ex.Message, ex);
                ToastHelper.ShowError("Failed to update Receipt information. Error: " + ex.Message);
            }
        }

        #endregion

        #region Binding Property
        private void ModelPropertyChanged()
        {
            HasChanges = dataRepository.HasChanges();
        }

        // for setting the value of selected receipt in backend database 
        public string SelectedReceipt
        {
            get
            {
                return SlipSetting?.FirstOrDefault(x => x.SettingName == "SelectedReceipt")?.SettingValue;
            }
            set
            {
                var setting = SlipSetting?.FirstOrDefault(x => x.SettingName == "SelectedReceipt");
                if (setting != null)
                {
                    setting.SettingValue = value;
                    OnPropertyChanged(nameof(SelectedReceipt));
                    SelectedSlipImage = GetImagePathForReceipt(value, false);
                    HasChanges = true;
                    ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged();
                    ModelPropertyChanged();
                }
            }
        }

        public string MultiSelectedReceipt
        {
            get
            {
                return SlipSetting?.FirstOrDefault(x => x.SettingName == "MultiSelectedReceipt")?.SettingValue;
            }
            set
            {
                var setting = SlipSetting?.FirstOrDefault(x => x.SettingName == "MultiSelectedReceipt");
                if (setting != null)
                {
                    setting.SettingValue = value;
                    HasChanges = true;
                    OnPropertyChanged(nameof(MultiSelectedReceipt));
                    SelectedSlipImage = GetImagePathForReceipt(value, true);
                    ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged();
                    ModelPropertyChanged();
                }
            }
        }
        #endregion

        #region Receipt list display logic 

        /// for getting receipt list from backend 
        private List<Receipt> _receiptlist;
        public List<Receipt> Receipts
        {
            get { return _receiptlist; }
            set { _receiptlist = value; OnPropertyChanged(); }
        }

        private List<Receipt> _multiSelectedReceipt;
        public List<Receipt> MultiReceipts
        {
            get { return _multiSelectedReceipt; }
            set { _multiSelectedReceipt = value; OnPropertyChanged(); }
        }

        /// for getting receipt list from backend 
        private void SetupReceiptList()
        {
            string receiptDoc = System.IO.File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "Receipt.json");
            List<Receipt> lstReceipts = JsonConvert.DeserializeObject<List<Receipt>>(receiptDoc).ToList();

            Receipts = lstReceipts.Where(r => !r.IsMulti).ToList();
            MultiReceipts = lstReceipts.Where(r => r.IsMulti).ToList();
        }
        #endregion

        #region Display Image of selected receipt 

        private string _selectedSlipImage;
        public string SelectedSlipImage
        {
            get => _selectedSlipImage;
            set
            {
                _selectedSlipImage = value;
                OnPropertyChanged(nameof(SelectedSlipImage));
            }
        }

        private string GetImagePathForReceipt(string receiptName, bool IsMulti)
        {
            string basePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Report\\Receipt\\";
            Receipt selectedReceipt = new Receipt();

            if (IsMulti)
            {
                selectedReceipt = MultiReceipts.FirstOrDefault(r => r.Label == receiptName);
            }
            else
            {
                selectedReceipt = Receipts.FirstOrDefault(r => r.Label == receiptName);
            }


            var imagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, basePath, selectedReceipt.Image);
            return System.IO.File.Exists(imagePath) ? imagePath : null;
        }

        #endregion
    }
}
