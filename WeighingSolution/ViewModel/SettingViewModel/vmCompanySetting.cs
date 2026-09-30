using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;
using WeighingSolution.Events;
using WeighingSolution.Helpers;

namespace WeighingSolution.ViewModel
{
    public class vmCompanySetting : NotifyDataErrorInfoBase, IDisposable
    {
        public ObservableCollection<ApplicationSetting> CompanySetting;
        private DataRepository dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));
        protected readonly IEventAggregator EventAggregator = AppEventManager.Instance.EventAggregator;

        private Page parentpage;

        #region CTOR
        public vmCompanySetting(Page page)
        {
            parentpage = page;
            CompanySetting = new ObservableCollection<ApplicationSetting>(dataRepository.Context.ApplicationSettings.Where(x => x.ModuleName == "CompanySetting").ToList());
            SaveSettingCommand = new DelegateCommand(OnSettingSaveExecute, OnSettingSaveCanExecute);
            CloseDetailViewCommand = new DelegateCommand<string>(OnCloseDetailViewExecute);
            SelectFile = new DelegateCommand<String>(OnSelectFile);
            setDefaultImage = new DelegateCommand<string>(OnSetDefaultImage);
        }
        #endregion

        #region Command and Properties Declaration
        public DelegateCommand SaveSettingCommand { get; private set; }
        public DelegateCommand<string> CloseDetailViewCommand { get; set; }
        public DelegateCommand<string> SelectFile { get; private set; }
        public DelegateCommand<string> setDefaultImage { get; private set; }

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
        [Required(ErrorMessage = "Company Name is required")]
        public string CompanyName
        {
            get { return CompanySetting.FirstOrDefault(x => x.SettingName == "CompanyName").SettingValue; }
            set { CompanySetting.FirstOrDefault(x => x.SettingName == "CompanyName").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); ValidatePropertyInternal("CompanyName", value); }
        }

        [Required(ErrorMessage = "Address is required")]
        public string Address
        {
            get { return CompanySetting.FirstOrDefault(x => x.SettingName == "Address").SettingValue; }
            set { CompanySetting.FirstOrDefault(x => x.SettingName == "Address").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); ValidatePropertyInternal("Address", value); }
        }

        [Required(ErrorMessage = "Phone number is required")]
        public string Phone
        {
            get { return CompanySetting.FirstOrDefault(x => x.SettingName == "Phone").SettingValue; }
            set { CompanySetting.FirstOrDefault(x => x.SettingName == "Phone").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); ValidatePropertyInternal("Phone", value); }
        }

        [EmailAddress]
        public string Email
        {
            get { return CompanySetting.FirstOrDefault(x => x.SettingName == "Email").SettingValue; }
            set
            {
                if (value == null)
                    CompanySetting.FirstOrDefault(x => x.SettingName == "Email").SettingValue = string.Empty;
                else
                    CompanySetting.FirstOrDefault(x => x.SettingName == "Email").SettingValue = value;
                ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); ValidatePropertyInternal("Email", value);
            }
        }

        [Required(ErrorMessage = "Client company name is required")]
        public string ClientCompanyName
        {
            get { return CompanySetting.FirstOrDefault(x => x.SettingName == "ClientCompanyName").SettingValue; }
            set { CompanySetting.FirstOrDefault(x => x.SettingName == "ClientCompanyName").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); ValidatePropertyInternal("ClientCompanyName", value); }
        }

        [Required(ErrorMessage = "Client contact person is required")]
        public string ClientContactPerson
        {
            get { return CompanySetting.FirstOrDefault(x => x.SettingName == "ClientContactPerson").SettingValue; }
            set { CompanySetting.FirstOrDefault(x => x.SettingName == "ClientContactPerson").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); ValidatePropertyInternal("ClientContactPerson", value); }
        }

        [Required(ErrorMessage = "Client address is required")]
        public string ClientAddress
        {
            get { return CompanySetting.FirstOrDefault(x => x.SettingName == "ClientAddress").SettingValue; }
            set { CompanySetting.FirstOrDefault(x => x.SettingName == "ClientAddress").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); ValidatePropertyInternal("ClientAddress", value); }
        }


        [EmailAddress]
        public string SupportID
        {
            get { return CompanySetting.FirstOrDefault(x => x.SettingName == "SupportID").SettingValue; }
            set
            {
                if (value == null)
                    CompanySetting.FirstOrDefault(x => x.SettingName == "SupportID").SettingValue = string.Empty;
                else
                    CompanySetting.FirstOrDefault(x => x.SettingName == "SupportID").SettingValue = value;
                ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); ValidatePropertyInternal("SupportID", value);
            }
        }

        public string Logo
        {
            get { return CompanySetting.FirstOrDefault(x => x.SettingName == "Logo").SettingValue; }
            set { CompanySetting.FirstOrDefault(x => x.SettingName == "Logo").SettingValue = value; OnPropertyChanged(); ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged(); ModelPropertyChanged(); }
        }

        #endregion

        #region Save
        private bool OnSettingSaveCanExecute()
        {
            return HasChanges && !HasErrors;
        }
        private string SaveLogo(string FullFilePath, object RemovePhotoPath)
        {
            string FileExtn = System.IO.Path.GetExtension(Logo);
            string FileName = Guid.NewGuid().ToString("N");
            string NewFilePath = Constant.CustomImagesPath + @"\Logos\" + FileName + FileExtn;
            File.Copy(Logo, NewFilePath, true);
            return FileName + FileExtn;
        }
        private async void OnSettingSaveExecute()
        {
            try
            {
                if (Logo != "logo.png")
                {
                    ApplicationSetting ObjAppsetting = CompanySetting.FirstOrDefault(x => x.SettingName == "Logo");
                    DbPropertyEntry prop = dataRepository.Context.Entry(ObjAppsetting).Property("SettingValue");
                    if ((prop.EntityEntry.State == EntityState.Added && prop.CurrentValue != null && !String.IsNullOrEmpty(prop.CurrentValue.ToString()))
                        || (prop.IsModified && !prop.CurrentValue.Equals(prop.OriginalValue)))
                    {
                        ObjAppsetting.SettingValue = SaveLogo(ObjAppsetting.SettingValue, prop.OriginalValue);
                    }

                }
                await dataRepository.SaveAsync();
                EventAggregator.GetEvent<AfterDetailSavedEvent>().Publish(new AfterDetailSavedEventArgs { ObjectID = 0, ObjectInstance = CompanySetting, ObjectType = CompanySetting.GetType().Name });
                ToastHelper.ShowSuccess("Company information recorded successfully");
                Uri uri = new Uri("/View/SettingView/Setting_Page.Xaml", UriKind.Relative);
                parentpage.NavigationService.Navigate(uri);
            }
            catch (Exception ex)
            {
                App.Log.Error(ex.Message, ex);
                ToastHelper.ShowError("Failed to update Company information. Error: " + ex.Message);
            }
        }
        #endregion

        #region Method
        private void OnSelectFile(String ImageSource)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = false;
            open.Filter = "Image Files(*.jpg; *.png; *.jpeg; *.gif; *.bmp)|*.jpg; *.png; *.jpeg; *.gif; *.bmp";
            bool? result = open.ShowDialog();
            if (result == true)
                Logo = open.FileName;
        }

        private void OnSetDefaultImage(string obj)
        {
            Logo = "logo.png";
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