using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;
using WeighingSolution.SharedComponents;

namespace WeighingSolution.ViewModel
{
    public class vmGlobelSetting : NotifyDataErrorInfoBase, IDisposable
    {
        private ObservableCollection<ApplicationSetting> Global_Setting;
    
        private ObservableCollection<ApplicationSetting> LiveWeightSetting;
        private DataRepository dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));
        private Page parentpage;

        string SerialKey = WhoAmI.fingerPrintValue();

        #region CTOR
        public vmGlobelSetting(Page page)
        {
            parentpage = page;
            Global_Setting = new ObservableCollection<ApplicationSetting>(dataRepository.Context.ApplicationSettings.Where(x => x.ModuleName == PageEntity.GlobalSetting.ToString()).ToList());
            LiveWeightSetting = new ObservableCollection<ApplicationSetting>(dataRepository.Context.ApplicationSettings.Where(x => x.ModuleName == "LiveWeightSetting").ToList());
            SaveSettingCommand = new DelegateCommand(OnSettingSaveExecute, OnSettingSaveCanExecute);
            CloseDetailViewCommand = new DelegateCommand<string>(OnCloseDetailViewExecute);

            WeightBridges = dataRepository.Context.WeightBridges.AsNoTracking().Where(x => x.IsActive == true).OrderBy(p => p.ScaleName).ToList();
            WeightBridges.Insert(0, new WeightBridge() { WeightBridgeID = 0, ScaleName = "-- Select Scale --" });
            //SetupReceiptList();

            WeightBridgesConfig = ReadWeightBridgeConfig();

            if (WeightBridgesConfig.Count() < 1)
            {
                WeightBridgeConfigBindingModel weightBridgeConfigBindingModel = new WeightBridgeConfigBindingModel
                {
                    SerialNo = SerialKey,
                    WeightBridgeID = 1,
                    ScaleName = WeightBridges.FirstOrDefault(p => p.WeightBridgeID == 1).ScaleName
                };

                WeightBridgesConfig.Add(weightBridgeConfigBindingModel);
            }
        }

        private ObservableCollection<WeightBridgeConfigBindingModel> _WeightBridgesConfig;
        public ObservableCollection<WeightBridgeConfigBindingModel> WeightBridgesConfig
        {
            get { return _WeightBridgesConfig; }
            set { _WeightBridgesConfig = value; OnPropertyChanged(); }
        }

        private List<WeightBridge> weightBridges;
        public List<WeightBridge> WeightBridges
        {
            get { return weightBridges; }
            set { weightBridges = value; OnPropertyChanged(); }
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
            get { return Global_Setting.FirstOrDefault(x => x.SettingName == "EntryDateEditable").SettingValue; }
            set { Global_Setting.FirstOrDefault(x => x.SettingName == "EntryDateEditable").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }
        public string DisableSpecialCharactersInVehicle
        {
            get { return Global_Setting.FirstOrDefault(x => x.SettingName == "DisableSpecialCharactersInVehicle").SettingValue; }
            set { Global_Setting.FirstOrDefault(x => x.SettingName == "DisableSpecialCharactersInVehicle").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }
        public string DisplayLastVehicleTransactionDetails
        {
            get { return Global_Setting.FirstOrDefault(x => x.SettingName == "DisplayLastVehicleTransactionDetails").SettingValue; }
            set { Global_Setting.FirstOrDefault(x => x.SettingName == "DisplayLastVehicleTransactionDetails").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }
        public string DisbleFrom2ndPass
        {
            get { return Global_Setting.FirstOrDefault(x => x.SettingName == "DisbleFrom2ndPass").SettingValue; }
            set { Global_Setting.FirstOrDefault(x => x.SettingName == "DisbleFrom2ndPass").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }
        public string DisplayReceiptAfter1pass
        {
            get { return Global_Setting.FirstOrDefault(x => x.SettingName == "DisplayReceiptAfter1pass").SettingValue; }
            set { Global_Setting.FirstOrDefault(x => x.SettingName == "DisplayReceiptAfter1pass").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }

        public string AllowWithoutTareWeightChange
        {
            get { return Global_Setting.FirstOrDefault(x => x.SettingName == "AllowWithoutTareWeightChange").SettingValue; }
            set { Global_Setting.FirstOrDefault(x => x.SettingName == "AllowWithoutTareWeightChange").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }

        [RegularExpression("^[0-9]+$|^[0-9]*[.]{0,1}[0-9]*$", ErrorMessage = "Only decimal values are allowed")]
        [Range(0.1, 99.99, ErrorMessage = "Value must be between 0.1 to 99.99")]
        public string TareWeightDiff
        {
            get { return Global_Setting.FirstOrDefault(x => x.SettingName == "TareWeightDiff").SettingValue; }
            set { Global_Setting.FirstOrDefault(x => x.SettingName == "TareWeightDiff").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); ValidatePropertyInternal("TareWeightDiff", value); }
        }
        public string DateFormat
        {
            get { return Global_Setting.FirstOrDefault(x => x.SettingName == "DateFormat").SettingValue; }
            set { Global_Setting.FirstOrDefault(x => x.SettingName == "DateFormat").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }
        public string TimeFormat
        {
            get { return Global_Setting.FirstOrDefault(x => x.SettingName == "TimeFormat").SettingValue; }
            set { Global_Setting.FirstOrDefault(x => x.SettingName == "TimeFormat").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }
        public string DecimalFormat
        {
            get { return Global_Setting.FirstOrDefault(x => x.SettingName == "DecimalFormat").SettingValue; }
            set { Global_Setting.FirstOrDefault(x => x.SettingName == "DecimalFormat").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }
        public string IsOpenVehicleFO
        {
            get { return Global_Setting.FirstOrDefault(x => x.SettingName == "IsOpenVehicleFO").SettingValue; }
            set { Global_Setting.FirstOrDefault(x => x.SettingName == "IsOpenVehicleFO").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }

        public string IsOpenTransporterFO
        {
            get { return Global_Setting.FirstOrDefault(x => x.SettingName == "IsOpenTransporterFO").SettingValue; }
            set { Global_Setting.FirstOrDefault(x => x.SettingName == "IsOpenTransporterFO").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }
        public string IsOpenAccountFO
        {
            get { return Global_Setting.FirstOrDefault(x => x.SettingName == "IsOpenAccountFO").SettingValue; }
            set { Global_Setting.FirstOrDefault(x => x.SettingName == "IsOpenAccountFO").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }
        public string IsOpenProductFO
        {
            get { return Global_Setting.FirstOrDefault(x => x.SettingName == "IsOpenProductFO").SettingValue; }
            set { Global_Setting.FirstOrDefault(x => x.SettingName == "IsOpenProductFO").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }
        public string IsOpenDriverFO
        {
            get { return Global_Setting.FirstOrDefault(x => x.SettingName == "IsOpenDriverFO").SettingValue; }
            set { Global_Setting.FirstOrDefault(x => x.SettingName == "IsOpenDriverFO").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }

        public string AutoMaticReceipt
        {
            get { return Global_Setting.FirstOrDefault(x => x.SettingName == "AutoMaticReceipt").SettingValue; }
            set { Global_Setting.FirstOrDefault(x => x.SettingName == "AutoMaticReceipt").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }

        public string CollectToken
        {
            get { return Global_Setting.FirstOrDefault(x => x.SettingName == "CollectToken").SettingValue; }
            set { Global_Setting.FirstOrDefault(x => x.SettingName == "CollectToken").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }
        public string SAPApiBaseUrl
        {
            get { return Global_Setting.FirstOrDefault(x => x.SettingName == "SAPApiBaseUrl").SettingValue; }
            set { Global_Setting.FirstOrDefault(x => x.SettingName == "SAPApiBaseUrl").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }

        public int WeightBridge
        {
            get
            {
                if (WeightBridgesConfig.Where(x => x.SerialNo == SerialKey).Count() > 0)
                    return WeightBridgesConfig.FirstOrDefault(x => x.SerialNo == SerialKey).WeightBridgeID;
                else
                    return 0;
            }
            set
            {
                if (WeightBridgesConfig.Where(x => x.SerialNo == SerialKey).Count() > 0)
                {
                    WeightBridgesConfig.FirstOrDefault(x => x.SerialNo == SerialKey).WeightBridgeID = value;

                    OnPropertyChanged();
                    ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged();
                    HasChanges = true;
                }
            }
        }

        public string ApiUrl
        {
            get { return LiveWeightSetting.FirstOrDefault(x => x.SettingName == "ApiUrl").SettingValue; }
            set { LiveWeightSetting.FirstOrDefault(x => x.SettingName == "ApiUrl").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }

        public string WBID
        {
            get { return LiveWeightSetting.FirstOrDefault(x => x.SettingName == "WBID").SettingValue; }
            set { LiveWeightSetting.FirstOrDefault(x => x.SettingName == "WBID").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }

        public string PassKey
        {
            get { return LiveWeightSetting.FirstOrDefault(x => x.SettingName == "PassKey").SettingValue; }
            set { LiveWeightSetting.FirstOrDefault(x => x.SettingName == "PassKey").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }

        public string PostFixTicket
        {
            get { return Global_Setting.FirstOrDefault(x => x.SettingName == "PostFixTicket").SettingValue; }
            set { Global_Setting.FirstOrDefault(x => x.SettingName == "PostFixTicket").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }
        public string PreFixTicket
        {
            get { return Global_Setting.FirstOrDefault(x => x.SettingName == "PreFixTicket").SettingValue; }
            set { Global_Setting.FirstOrDefault(x => x.SettingName == "PreFixTicket").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }

        public string AnprApiUrl
        {
            get { return Global_Setting.FirstOrDefault(x => x.SettingName == "AnprApiUrl").SettingValue; }
            set { Global_Setting.FirstOrDefault(x => x.SettingName == "AnprApiUrl").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }
        public string AnprPassKey
        {
            get { return Global_Setting.FirstOrDefault(x => x.SettingName == "AnprPassKey").SettingValue; }
            set { Global_Setting.FirstOrDefault(x => x.SettingName == "AnprPassKey").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
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



                if (WeightBridge != 0)
                {

                    var weightBridgeConfigBindingModel = new WeightBridgeConfigBindingModel();

                    var _weightBridgesConfig = WeightBridgesConfig.Where(p => p.SerialNo == SerialKey).ToList();
                    if (_weightBridgesConfig.Count > 0)
                    {
                        WeightBridgesConfig.FirstOrDefault().WeightBridgeID = WeightBridge;
                        WeightBridgesConfig.FirstOrDefault().ScaleName = WeightBridges.FirstOrDefault(p => p.WeightBridgeID == WeightBridge).ScaleName;
                    }
                    else
                    {
                        weightBridgeConfigBindingModel = new WeightBridgeConfigBindingModel
                        {
                            SerialNo = SerialKey,
                            WeightBridgeID = WeightBridge,
                            ScaleName = WeightBridges.FirstOrDefault(p => p.WeightBridgeID == WeightBridge).ScaleName
                        };

                        WeightBridgesConfig.Add(weightBridgeConfigBindingModel);
                    }

                    string UpdateJson = JsonConvert.SerializeObject(WeightBridgesConfig);
                    WriteWeightBridgeConfig(UpdateJson);
                }

                ToastHelper.ShowSuccess("Slip information recorded successfully");
                MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("WeightBridge", "WeightBridge Data Change Need To Restart");
                if (msgbox_result == MessageDialogResult.Affirmative)
                {
                    try
                    {
                        var current = System.Diagnostics.Process.GetCurrentProcess();
                        System.Diagnostics.Process.GetProcessesByName(current.ProcessName).ToList().ForEach(t => t.Kill());
                    }
                    catch
                    {
                        System.Windows.Application.Current.Shutdown();
                    }
                }
                else
                {
                    try
                    {
                        var current = System.Diagnostics.Process.GetCurrentProcess();
                        System.Diagnostics.Process.GetProcessesByName(current.ProcessName).ToList().ForEach(t => t.Kill());
                    }
                    catch
                    {
                        System.Windows.Application.Current.Shutdown();
                    }
                }
                //Uri uri = new Uri("/View/SettingView/Setting_Page.Xaml", UriKind.Relative);
                //parentpage.NavigationService.Navigate(uri);
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


        public static ObservableCollection<WeightBridgeConfigBindingModel> ReadWeightBridgeConfig()
        {
            ObservableCollection<WeightBridgeConfigBindingModel> WeightBridges = new ObservableCollection<WeightBridgeConfigBindingModel>();
            try
            {
                string JsonFilePath = Constant.CustomSettingsPath + $@"\WeightBridgeConfig.json";

                if (File.Exists(JsonFilePath))
                {
                    string json = File.ReadAllText(JsonFilePath);
                    if (!string.IsNullOrEmpty(json))
                        WeightBridges = JsonConvert.DeserializeObject<ObservableCollection<WeightBridgeConfigBindingModel>>(json);

                    return WeightBridges;
                }
            }
            catch (Exception ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.CustomSettingsPath + @"\WeightBridgeConfigError.txt", json);
            }
            return WeightBridges;
        }

        public static void WriteWeightBridgeConfig(string UpdateJson)
        {
            try
            {
                string JsonPath = Constant.CustomSettingsPath + $@"\WeightBridgeConfig.json";
                File.WriteAllText(JsonPath, UpdateJson);
            }
            catch (Exception ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.CustomSettingsPath + @"\WeightBridgeConfigError.txt", json);
            }
        }


        /// for getting receipt list from backend 
        //private List<Receipt> _receiptlist;
        //public List<Receipt> Receipts
        //{
        //    get { return _receiptlist; }
        //    set { _receiptlist = value; OnPropertyChanged(); }
        //}

        //private List<Receipt> _multiSelectedReceipt;
        //public List<Receipt> MultiReceipts
        //{
        //    get { return _multiSelectedReceipt; }
        //    set { _multiSelectedReceipt = value; OnPropertyChanged(); }
        //}


        ///// for getting receipt list from backend 
        //private void SetupReceiptList()
        //{
        //    string receiptDoc = System.IO.File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "Receipt.json");
        //    List<Receipt> lstReceipts = JsonConvert.DeserializeObject<List<Receipt>>(receiptDoc).ToList();

        //    Receipts = lstReceipts.Where(r => !r.IsMulti).ToList();
        //    MultiReceipts = lstReceipts.Where(r => r.IsMulti).ToList();
        //}
    }
}