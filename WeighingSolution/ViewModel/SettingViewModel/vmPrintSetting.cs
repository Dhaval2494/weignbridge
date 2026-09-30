using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;

namespace WeighingSolution.ViewModel
{
    public class vmPrintSetting : NotifyDataErrorInfoBase, IDisposable
    {
        private DataRepository dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));
        private Page parentpage;
        public PrintSettingClass PrintSettingModel = new PrintSettingClass();
        private ObservableCollection<ApplicationSetting> PrintSetting;

        #region CTOR
        public vmPrintSetting(Page page)
        {
            parentpage = page;
            SaveSettingCommand = new DelegateCommand(OnSettingSaveExecute, OnSettingSaveCanExecute);
            CloseDetailViewCommand = new DelegateCommand<string>(OnCloseDetailViewExecute);
            PrintSetting = new ObservableCollection<ApplicationSetting>(dataRepository.Context.ApplicationSettings.Where(x => x.ModuleName == "PrintSetting").ToList());

            if (PrintSetting != null && PrintSetting.Count > 0)
            {
                string SelectedPrinter = string.Empty;
                if (PrintSetting.Where(x => x.SettingName == "PrinterName" && !string.IsNullOrEmpty(x.SettingValue)).Count() > 0)
                {
                    SelectedPrinter = PrintSetting.Where(x => x.SettingName == "PrinterName" && !string.IsNullOrEmpty(x.SettingValue)).FirstOrDefault().SettingValue;
                }
                SetupPrinters(SelectedPrinter);
            }
            else
            {
                string Query = "IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'PrintSetting' AND SettingName = 'PrintType')" +
                                "INSERT INTO ApplicationSettings VALUES('PrintSetting', 'PrintType', 'String', 'Laser')" +
                                "IF NOT EXISTS(SELECT * FROM ApplicationSettings WHERE ModuleName = 'PrintSetting' AND SettingName = 'PrinterName')" +
                                "INSERT INTO ApplicationSettings VALUES('PrintSetting', 'PrinterName', 'String', '')";

                bool flag = DatabaseFunctions.ExecuteQuery(Query);

                string SelectedPrinter = string.Empty;
                SetupPrinters(SelectedPrinter);
            }
        }
        #endregion

        #region Command and Properties Declaration
        public DelegateCommand<string> CloseDetailViewCommand { get; set; }
        public DelegateCommand SaveSettingCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }

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

        private List<String> _installedPrinter;
        public List<String> InstalledPrinter
        {
            get { return _installedPrinter; }
            set { _installedPrinter = value; OnPropertyChanged(); }
        }

        public bool AutoBackupEnable
        {
            get { return Convert.ToBoolean(PrintSetting.FirstOrDefault(x => x.SettingName == "AutoBackupEnable").SettingValue); }
            set { PrintSetting.FirstOrDefault(x => x.SettingName == "AutoBackupEnable").SettingValue = value.ToString(); OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }

        #endregion

        #region Binding Property

        [Required(ErrorMessage = "Print Type is required")]
        public string PrintType
        {
            get { return PrintSetting.FirstOrDefault(x => x.SettingName == "PrintType").SettingValue; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    PrintSetting.FirstOrDefault(x => x.SettingName == "PrintType").SettingValue = value;
                    HasChanges = true;
                }
                OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ValidatePropertyInternal("PrintType", value);
            }
        }

        [Required(ErrorMessage = "Printer Name is required")]
        public string PrinterName
        {
            get
            {
                if (string.IsNullOrEmpty(PrintSetting.FirstOrDefault(x => x.SettingName == "PrinterName").SettingValue))
                    return "--Select Printer--";
                else
                    return PrintSetting.FirstOrDefault(x => x.SettingName == "PrinterName").SettingValue;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (value == "--Select Printer--")
                    {
                        PrintSetting.FirstOrDefault(x => x.SettingName == "PrinterName").SettingValue = string.Empty;
                    }
                    else
                    {
                        PrintSetting.FirstOrDefault(x => x.SettingName == "PrinterName").SettingValue = value;
                    }
                    HasChanges = true;
                };
                OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ValidatePropertyInternal("PrinterName", value);
            }
        }

        #endregion

        #region Save
        private bool OnSettingSaveCanExecute()
        {
            if (HasChanges)
                return true;
            else
                return false;
        }
        private async void OnSettingSaveExecute()
        {
            try
            {
                await dataRepository.SaveAsync();
                ToastHelper.ShowSuccess("Printer information recorded successfully");
                Uri uri = new Uri("/View/SettingView/Setting_Page.Xaml", UriKind.Relative);
                parentpage.NavigationService.Navigate(uri);
            }
            catch (Exception ex)
            {
                string Error = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.CustomSettingsPath + @"\PrinterSettingSaveError.txt", Error);
            }

        }
        #endregion

        #region Method
        private void SetupPrinters(string SelectedPrinter)
        {
            List<String> printers = new List<string>();
            printers.Add("--Select Printer--");
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                printers.Add(printer);
            }
            InstalledPrinter = printers;
            if (!string.IsNullOrEmpty(SelectedPrinter) && InstalledPrinter.All(x => x != SelectedPrinter))
            {
                InstalledPrinter.Add(SelectedPrinter);
            }
        }
        public void Dispose()
        {
            dataRepository.Context.Dispose();
        }
        private void ModelPropertyChanged()
        {
            if (!HasChanges)
                HasChanges = dataRepository.HasChanges();
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
