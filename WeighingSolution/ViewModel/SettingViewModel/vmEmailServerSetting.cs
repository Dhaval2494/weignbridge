using MahApps.Metro.SimpleChildWindow;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;
using WeighingSolution.SharedComponents;

namespace WeighingSolution.ViewModel
{
    public class vmEmailServerSetting : WeighingSolution.DataLayer.NotifyDataErrorInfoBase, IDisposable
    {
        public DataRepository dataRepository;
        public ObservableCollection<ApplicationSetting> EmailSetting;
        public ChildWindow ChildWindow;
        public vmEmailServerSetting(ChildWindow childwindow)
        {
            ChildWindow = childwindow;
            dataRepository = new DataRepository(new weighbridgeEntities(Constant.EntityConnectionString));
            EmailSetting = new ObservableCollection<ApplicationSetting>(dataRepository.Context.ApplicationSettings.Where(x => x.ModuleName == "EmailSetting").ToList());
            SaveSettingCommand = new DelegateCommand(OnSettingSaveExecute, OnSettingSaveCanExecute);
            SendTestMail = new DelegateCommand(OnSendTestMailExecute, OnSendTestMailCanExecute);
            this.PropertyChanged -= vmEmailServerSetting_PropertyChanged;
            this.PropertyChanged += vmEmailServerSetting_PropertyChanged;
        }

        #region Command and Properties Declaration
        public DelegateCommand SaveSettingCommand { get; private set; }
        public DelegateCommand SendTestMail { get; private set; }

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
        [Required(ErrorMessage = "Server Name is required")]
        public string ServerName
        {
            get { return EmailSetting.FirstOrDefault(x => x.SettingName == "ServerName").SettingValue; }
            set { EmailSetting.FirstOrDefault(x => x.SettingName == "ServerName").SettingValue = value; OnPropertyChanged(); ValidatePropertyInternal("ServerName", value); }
        }

        [Required(ErrorMessage = "Port is required")]
        public string Port
        {
            get { return EmailSetting.FirstOrDefault(x => x.SettingName == "Port").SettingValue; }
            set { EmailSetting.FirstOrDefault(x => x.SettingName == "Port").SettingValue = value; OnPropertyChanged(); ValidatePropertyInternal("Port", value); }
        }

        public string UserName
        {
            get { return EmailSetting.FirstOrDefault(x => x.SettingName == "UserName").SettingValue; }
            set
            {
                EmailSetting.FirstOrDefault(x => x.SettingName == "UserName").SettingValue = value;
                OnPropertyChanged();
            }
        }

        public string PassWord
        {
            get { return Common.DecryptData(EmailSetting.FirstOrDefault(x => x.SettingName == "PassWord").SettingValue); }
            set
            {
                EmailSetting.FirstOrDefault(x => x.SettingName == "PassWord").SettingValue = Common.EncryptData(value);
                OnPropertyChanged();
            }
        }

        public bool UseSSL
        {
            get { return Convert.ToBoolean(EmailSetting.FirstOrDefault(x => x.SettingName == "UseSSL").SettingValue); }
            set { EmailSetting.FirstOrDefault(x => x.SettingName == "UseSSL").SettingValue = value.ToString(); OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); }
        }
        [EmailAddress]
        public string EmailFrom
        {
            get { return EmailSetting.FirstOrDefault(x => x.SettingName == "EmailFrom").SettingValue; }
            set
            {
                EmailSetting.FirstOrDefault(x => x.SettingName == "EmailFrom").SettingValue = value;
                OnPropertyChanged(); ValidatePropertyInternal("EmailFrom", value);
            }
        }

        private string _EmailTo;
        [EmailAddress]
        public string EmailTo
        {
            get { return _EmailTo; }
            set
            {
                _EmailTo = value;
                ValidatePropertyInternal("EmailTo", value);
                ((DelegateCommand)SendTestMail).RaiseCanExecuteChanged();
            }
        }
        #endregion


        private bool OnSendTestMailCanExecute()
        {
            return !HasErrors;
        }

        private void OnSendTestMailExecute()
        {
            try
            {
                Email objEmail = new Email();
                objEmail.SMTPServer = ServerName;
                if (string.IsNullOrEmpty(UserName))
                    objEmail.SMTPUserName = "";
                else
                    objEmail.SMTPUserName = UserName;
                if (string.IsNullOrEmpty(PassWord))
                    objEmail.SMTPPassword = "";
                else
                    objEmail.SMTPPassword = PassWord;
                if (string.IsNullOrEmpty(EmailFrom))
                    objEmail.EmailFrom = "no-reply@weighingsolution.com";
                else
                    objEmail.EmailFrom = EmailFrom;
                objEmail.Subject = "Test Mail";
                objEmail.EmailAddress = EmailTo;
                objEmail.Body = "This is test mail";
                if (objEmail.SendHtmlEmail(Convert.ToInt32(Port), UseSSL))
                    ToastHelper.ShowSuccess("Email sending successfully");
                else
                    ToastHelper.ShowError("Fail to send mail, kindly check your credential");
            }
            catch (Exception ex)
            {
                Common.logErrorInDB(ex.Message, ex);
                ToastHelper.ShowError("Fail to send mail, kindly check your credential");
            }
        }

        private bool OnSettingSaveCanExecute()
        {
            return HasChanges && !HasErrors;
        }

        private async void OnSettingSaveExecute()
        {
            try
            {
                await dataRepository.SaveAsync();
                ToastHelper.ShowSuccess("Email Server Setting information recorded successfully");
                ChildWindow.IsModal = false;
                ChildWindow.Close();
            }
            catch (Exception ex)
            {
                Common.logErrorInDB(ex.Message, ex);
                ToastHelper.ShowError("Failed to update email server setting. Error: " + ex.Message);
            }
        }
        void vmEmailServerSetting_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = dataRepository.HasChanges();
            }
            ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged();
        }
        public void Dispose()
        {
            dataRepository.Context.Dispose();
        }
    }
}
