using MahApps.Metro.Controls;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WeighingSolution.DataLayer;
using WeighingSolution.Events;
using WeighingSolution.Helpers;

namespace WeighingSolution.ViewModel
{
    public class TransactionDataMainpage
    {
        public Vehicle Vehicle { get; set; }
        public System.Guid ReceiptTicketID { get; set; }
        public int TicketID { get; set; }
        public string TransactionTitle
        {
            get
            {
                if (Vehicle != null && !String.IsNullOrEmpty(Vehicle.VehicleNumber))
                    return "Ticket No: " + TicketID.ToString() + " Vehical No: " + Vehicle.VehicleNumber;
                else
                    return "Ticket No: " + TicketID.ToString();
            }
        }
    }

    public class MainWindowViewModel : NotifyDataErrorInfoBase, IDisposable
    {
        private DataRepository dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));
        private ObservableCollection<ApplicationSetting> CompanySetting;
        protected readonly IEventAggregator EventAggregator = AppEventManager.Instance.EventAggregator;
        private UserMaster objUserMaster;
        private Window parentPage;

        private List<TransactionDataMainpage> activetxdata;
        public List<TransactionDataMainpage> ActiveTxData
        {
            get { return activetxdata; }
            set { activetxdata = value; OnPropertyChanged(); }
        }

        private TransactionDataMainpage _selectedTx;

        public TransactionDataMainpage SelectedTx
        {
            get { return _selectedTx; }
            set { _selectedTx = value; OnPropertyChanged(); }
        }

        private WeightBridge _selectedWeighBridge;
        public WeightBridge SelectedWeighBridge
        {
            get { return _selectedWeighBridge; }
            set
            {
                _selectedWeighBridge = value;
                OnPropertyChanged();
            }
        }

        private string _searchtext;
        public string TxSearchText
        {
            get { return _searchtext; }
            set { _searchtext = value; OnPropertyChanged(); }
        }

        #region Anpr

        private string _Anpr1Number;
        public string Anpr1Number
        {
            get { return _Anpr1Number; }
            set { _Anpr1Number = value; OnPropertyChanged(); }
        }

        private string _LastAnpr1Number;
        public string LastAnpr1Number
        {
            get { return _LastAnpr1Number; }
            set { _LastAnpr1Number = value; OnPropertyChanged(); }
        }

        private string _Anpr2Number;
        public string Anpr2Number
        {
            get { return _Anpr2Number; }
            set { _Anpr2Number = value; OnPropertyChanged(); }
        }

        private string _LastAnpr2Number;
        public string LastAnpr2Number
        {
            get { return _LastAnpr2Number; }
            set { _LastAnpr2Number = value; OnPropertyChanged(); }
        }

        #endregion Anpr

        private string _UHF1Stutas;
        public string UHF1Stutas
        {
            get { return _UHF1Stutas; }
            set { _UHF1Stutas = value; OnPropertyChanged(); }
        }

        private string _UHF2Stutas;
        public string UHF2Stutas
        {
            get { return _UHF2Stutas; }
            set { _UHF2Stutas = value; OnPropertyChanged(); }
        }

        private string _RFIDText;
        public string RFIDText
        {
            get { return _RFIDText; }
            set { _RFIDText = value; OnPropertyChanged(); }
        }

        private string _RFIDCount;
        public string RFIDCount
        {
            get { return _RFIDCount; }
            set { _RFIDCount = value; OnPropertyChanged(); }
        }


        private string _ModBusText;
        public string ModBusText
        {
            get { return "Log = " + _ModBusText; }
            set { _ModBusText = value; OnPropertyChanged(); }
        }


        private string _CycleMSG;
        public string CycleMSG
        {
            get { return "Log = " + _CycleMSG; }
            set { _CycleMSG = value; OnPropertyChanged(); }
        }

        public DelegateCommand SaveCommand { get; private set; }
        public MainWindowViewModel(Window page, int userid)
        {
            parentPage = page;
            EventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(UserDetailChange);
            EventAggregator.GetEvent<TransactionDetailSavedEvent>().Subscribe(TransactionStatusUpdatedAsync);
            EventAggregator.GetEvent<TransactionStatusUpdatedEvent>().Subscribe(TransactionStatusUpdatedAsync);
            SaveCommand = new DelegateCommand(OnSavenExecute, OnSaveCanExecute);
            objUserMaster = dataRepository.Context.UserMasters.FirstOrDefault(x => x.UserID == userid);
            if (objUserMaster != null)
            {
                Application.Current.Properties["LoggedInUserID"] = objUserMaster.UserID;
                Constant.setLoggedInUserID(objUserMaster.UserID);
                Application.Current.Properties["LoggedInUserName"] = objUserMaster.UserName;
            }
            //string licenseKey = System.IO.File.ReadAllText(Constant.CustomSettingsPath + @"\license.lic");
            //string decryptedData = Common.DecryptData(licenseKey);
            //Portable.Licensing.License LicenseInfo = Portable.Licensing.License.Load(decryptedData);
            //if (LicenseInfo != null && Constant.IsBasicversion)
            //{
            //    using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
            //    {
            //        List<ApplicationSetting> lstSetting = ctx.ApplicationSettings.Where(x => x.ModuleName == "CompanySetting").ToList();
            //        lstSetting.FirstOrDefault(x => x.SettingName == "CompanyName").SettingValue = LicenseInfo.Customer.Name;
            //        lstSetting.FirstOrDefault(x => x.SettingName == "Email").SettingValue = LicenseInfo.Customer.Email;
            //        ctx.SaveChanges();
            //    }
            //}
            CompanySetting = new ObservableCollection<ApplicationSetting>(dataRepository.Context.ApplicationSettings.Where(x => x.ModuleName == "CompanySetting").ToList());
        }

        private async void TransactionStatusUpdatedAsync(TransactionDetailSavedEventArgs obj)
        {
            SelectedTx = null;
            TxSearchText = "";
            await LoadAsync();
        }

        public async Task LoadAsync()
        {
            //ActiveTxData = await dataRepository.Context.TransactionDatas.Include("Vehicle").Where(x => x.Status == "Active").OrderByDescending(x => x.TicketID).ToListAsync();
            if (dataRepository.Context.TransactionDatas.Where(x => x.Status == "Active").Count() > 0)
                ActiveTxData = await dataRepository.Context.TransactionDatas.Include("Vehicle").Where(x => x.Status == "Active").OrderByDescending(x => x.TicketID).Select(x => new TransactionDataMainpage { Vehicle = x.Vehicle, TicketID = x.TicketID, ReceiptTicketID = x.ReceiptTicketID }).ToListAsync();
            else
                ActiveTxData = new List<TransactionDataMainpage>();
        }

        private void UserDetailChange(AfterDetailSavedEventArgs obj)
        {
            if (obj.ObjectInstance is UserMaster && obj.ObjectID == objUserMaster.UserID)
            {
                objUserMaster = dataRepository.Context.UserMasters.AsNoTracking().FirstOrDefault(x => x.UserID == obj.ObjectID);
                OnPropertyChanged("LoggedInuser");
                OnPropertyChanged("pwbCurrentPassword");
            }
            else if (obj.ObjectInstance is ObservableCollection<ApplicationSetting>)
            {
                CompanySetting = obj.ObjectInstance as ObservableCollection<ApplicationSetting>;
                OnPropertyChanged("CompanyName");
                OnPropertyChanged("Address"); OnPropertyChanged("Phone"); OnPropertyChanged("Email"); OnPropertyChanged("Logo");
                //StackPanel stkEditComapnysetting = parentPage.FindName("stkEditComapnysetting") as StackPanel;
                //if ((CompanyName == "Company Name") && (Address == "Address") && (Phone == "1234567890") && (Logo == "logo.png") && (Email == "ID@server.com"))
                //    stkEditComapnysetting.Visibility = Visibility.Visible;
                //else
                //    stkEditComapnysetting.Visibility = Visibility.Collapsed;
            }
        }

        private bool OnSaveCanExecute()
        {
            if (HasChanges && !HasErrors)
                return true;
            else
                return false;
        }

        private async void OnSavenExecute()
        {
            ValidatePropertyInternal("pwbCurrentPassword", pwbCurrentPassword);
            ValidatePropertyInternal("pwbNewPassword", pwbNewPassword);
            ValidatePropertyInternal("pwbConfirmPassword", pwbConfirmPassword);

            string ErrorMessage = "";
            bool Error = false;
            if (!HasErrors)
            {
                if (objUserMaster != null && pwbCurrentPassword != Common.DecryptData(objUserMaster.Password))
                    ErrorMessage = "Current Password is incorrect";
                else if (pwbNewPassword != pwbConfirmPassword)
                    ErrorMessage = "Confirm Password does not match";

                if (!string.IsNullOrEmpty(ErrorMessage))
                {
                    ToastHelper.ShowError(ErrorMessage);
                    Error = true;
                    HasChanges = false;
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
            if (!HasErrors && !Error)
            {
                if (objUserMaster != null)
                {
                    objUserMaster.Salt = Common.createSalt(objUserMaster.UserName, pwbNewPassword, objUserMaster);
                    objUserMaster.Password = Common.EncryptData(pwbNewPassword);
                    await dataRepository.SaveAsync();
                    ToastHelper.ShowSuccess("Change Password successfully.");
                    MetroWindow mw = ((MetroWindow)App.Current.MainWindow);
                    Flyout flyChangePassword = (Flyout)mw.FindName("flyChangePassword");
                    flyChangePassword.IsOpen = false;
                }
            }
            else
                SaveCommand.RaiseCanExecuteChanged();

        }
        public string CompanyName { get { return CompanySetting.FirstOrDefault(x => x.SettingName == "CompanyName").SettingValue; } }
        public string Address { get { return CompanySetting.FirstOrDefault(x => x.SettingName == "Address").SettingValue; } }
        public string Phone { get { return CompanySetting.FirstOrDefault(x => x.SettingName == "Phone").SettingValue; } }
        public string Email { get { return CompanySetting.FirstOrDefault(x => x.SettingName == "Email").SettingValue; } }
        public string Logo { get { return CompanySetting.FirstOrDefault(x => x.SettingName == "Logo").SettingValue; } }
        public string LoggedInuser { get { return objUserMaster.FirstName + " " + objUserMaster.LastName; } }

        private string _pwbCurrentPassword;
        [Required(ErrorMessage = "Current Password is required")]
        [MaxLength(50, ErrorMessage = "Current Password can not be larger than 50 character")]
        public string pwbCurrentPassword
        {
            get { return _pwbCurrentPassword; }
            set
            {
                _pwbCurrentPassword = value;
                if (pwbCurrentPassword == "")
                    HasChanges = false;
                else
                    HasChanges = true;
                OnPropertyChanged(); ValidatePropertyInternal("pwbCurrentPassword", pwbCurrentPassword); SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private string _pwbNewPassword;
        [Required(ErrorMessage = " New Password is required")]
        [MaxLength(50, ErrorMessage = "New Password can not be larger than 50 character")]
        public string pwbNewPassword
        {
            get { return _pwbNewPassword; }
            set
            {
                _pwbNewPassword = value;
                if (pwbNewPassword == "")
                    HasChanges = false;
                else
                    HasChanges = true;
                OnPropertyChanged(); ValidatePropertyInternal("pwbNewPassword", pwbNewPassword); SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private string _pwbConfirmPassword;
        [Required(ErrorMessage = " Confirm Password is required")]
        public string pwbConfirmPassword
        {
            get { return _pwbConfirmPassword; }
            set
            {
                _pwbConfirmPassword = value;
                if (pwbConfirmPassword == "")
                    HasChanges = false;
                else
                    HasChanges = true;
                OnPropertyChanged(); ValidatePropertyInternal("pwbConfirmPassword", pwbConfirmPassword); SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _hasChanges;
        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                if (_hasChanges != value)
                    _hasChanges = value; OnPropertyChanged(); SaveCommand.RaiseCanExecuteChanged();
            }
        }
        public void Dispose()
        {
            dataRepository.Context.Dispose();
            EventAggregator.GetEvent<AfterDetailSavedEvent>().Unsubscribe(UserDetailChange);
            EventAggregator.GetEvent<TransactionDetailSavedEvent>().Unsubscribe(TransactionStatusUpdatedAsync);
            EventAggregator.GetEvent<TransactionStatusUpdatedEvent>().Unsubscribe(TransactionStatusUpdatedAsync);
        }
    }
}
