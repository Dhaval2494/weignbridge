using MahApps.Metro.SimpleChildWindow;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;
using WeighingSolution.SharedComponents;

namespace WeighingSolution.ViewModel
{
    public class vmSupportTicket : NotifyDataErrorInfoBase, IDisposable
    {
        public DataRepository dataRepository;
        public ObservableCollection<ApplicationSetting> EmailSetting;
        public ObservableCollection<ApplicationSetting> CompanySetting;
        public ChildWindow ChildWindow;
        public vmSupportTicket(ChildWindow childwindow)
        {
            ChildWindow = childwindow;
            dataRepository = new DataRepository(new weighbridgeEntities(Constant.EntityConnectionString));
            EmailSetting = new ObservableCollection<ApplicationSetting>(dataRepository.Context.ApplicationSettings.Where(x => x.ModuleName == "EmailSetting").ToList());
            CompanySetting = new ObservableCollection<ApplicationSetting>(dataRepository.Context.ApplicationSettings.Where(x => x.ModuleName == "CompanySetting").ToList());
            SendTicket = new DelegateCommand(OnSendTicketExecute, OnSendTicketCanExecute);
            this.PropertyChanged -= vmSupportTicket_PropertyChanged;
            this.PropertyChanged += vmSupportTicket_PropertyChanged;
        }

        public DelegateCommand SendTicket { get; private set; }

        #region Binding Property

        private string _TicketSubject;
        [Required(ErrorMessage = "Ticket subject is required")]
        public string TicketSubject
        {
            get { return _TicketSubject; }
            set { _TicketSubject = value; OnPropertyChanged(); ValidatePropertyInternal("TicketSubject", value); }
        }

        private string _TicketBody;
        [Required(ErrorMessage = "Ticket body is required")]
        public string TicketBody
        {
            get { return _TicketBody; }
            set { _TicketBody = value; OnPropertyChanged(); ValidatePropertyInternal("TicketBody", value); }
        }
        public string ScreenShotName { get; set; }

        public List<string> lstAttachmentPath { get; set; }
        #endregion


        private bool OnSendTicketCanExecute()
        {
            return !HasErrors;
        }

        private void OnSendTicketExecute()
        {
            try
            {
                if (this.IsValid())
                {
                    string ServerName = EmailSetting.FirstOrDefault(x => x.SettingName == "ServerName").SettingValue;
                    string UserName = EmailSetting.FirstOrDefault(x => x.SettingName == "UserName").SettingValue;
                    string PassWord = Common.DecryptData(EmailSetting.FirstOrDefault(x => x.SettingName == "PassWord").SettingValue);
                    string EmailFrom = EmailSetting.FirstOrDefault(x => x.SettingName == "EmailFrom").SettingValue;
                    string Port = EmailSetting.FirstOrDefault(x => x.SettingName == "Port").SettingValue;
                    bool UseSSL = EmailSetting.FirstOrDefault(x => x.SettingName == "UseSSL").SettingValue != null ? Convert.ToBoolean(EmailSetting.FirstOrDefault(x => x.SettingName == "UseSSL").SettingValue) : true;
                    string emailTo = dataRepository.Context.ApplicationSettings.FirstOrDefault(x => x.ModuleName == "CompanySetting" && x.SettingName == "SupportID").SettingValue;

                    string EmailBody = "";
                    var assembly = Assembly.GetExecutingAssembly();
                    var resourceName = "WeighingSolution.EmailTemplate.CustomerDetail.html";

                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        EmailBody = reader.ReadToEnd();
                    }

                    EmailBody = EmailBody.Replace("XXXCompanyNameXXX", CompanySetting.FirstOrDefault(x => x.SettingName == "ClientCompanyName").SettingValue);
                    EmailBody = EmailBody.Replace("XXXContactNameXXX", CompanySetting.FirstOrDefault(x => x.SettingName == "ClientContactPerson").SettingValue);
                    EmailBody = EmailBody.Replace("XXXAddressXXX", CompanySetting.FirstOrDefault(x => x.SettingName == "ClientAddress").SettingValue);
                    EmailBody = EmailBody.Replace("XXXCustomerMessageXXX", TicketBody);
                    EmailBody = EmailBody.Replace("XXXCreatedAtXXX", DateTime.Now.ToString(" dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture));

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
                    objEmail.Subject = TicketSubject;
                    objEmail.EmailAddress = emailTo;
                    objEmail.Body = EmailBody;

                    objEmail.Attachments = new Dictionary<int, string>();
                    if (!string.IsNullOrEmpty(ScreenShotName))
                        objEmail.Attachments.Add(0, Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\WeighBridgeSolution\\ScreenShot\\" + this.ScreenShotName);

                    if (lstAttachmentPath != null && lstAttachmentPath.Count > 0)
                    {
                        int counter = 1;
                        foreach (string item in lstAttachmentPath)
                        {
                            objEmail.Attachments.Add(counter, item);
                            counter = counter + 1;
                        }
                    }

                    if (objEmail.SendHtmlEmail(Convert.ToInt32(Port), UseSSL))
                        ToastHelper.ShowSuccess("Ticket created successfully");
                    else
                        ToastHelper.ShowError("Fail to create ticket, kindly check your email credential");
                }
            }
            catch (Exception ex)
            {
                Common.logErrorInDB(ex.Message, ex);
                ToastHelper.ShowError("Fail to send mail, kindly check your credential");
            }
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
        void vmSupportTicket_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ((DelegateCommand)SendTicket).RaiseCanExecuteChanged();
        }
        public void Dispose()
        {
            dataRepository.Context.Dispose();
        }
    }
}
