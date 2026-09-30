using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;

namespace WeighingSolution.ViewModel
{
    public class vmApplicationTheme : NotifyDataErrorInfoBase, IDisposable
    {
        public ObservableCollection<ApplicationSetting> ThemeSetting;
        private DataRepository dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));

        private Page parentpage;

        #region CTOR
        public vmApplicationTheme(Page page)
        {
            parentpage = page;
            ThemeSetting = new ObservableCollection<ApplicationSetting>(dataRepository.Context.ApplicationSettings.Where(x => x.ModuleName == "ThemeSetting").ToList());
            SaveCommand = new DelegateCommand(OnSettingSaveExecute, OnSettingSaveCanExecute);
            CloseDetailViewCommand = new DelegateCommand<string>(OnCloseDetailViewExecute);
        }
        #endregion

        #region Command and Properties Declaration
        public DelegateCommand SaveCommand { get; private set; }
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
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }
        #endregion

        #region Binding Property
        public string Color
        {
            get { return ThemeSetting.FirstOrDefault(x => x.SettingName == "Color").SettingValue; }
            set
            {
                if (ThemeSetting.FirstOrDefault(x => x.SettingName == "Color").SettingValue != value)
                {
                    ThemeSetting.FirstOrDefault(x => x.SettingName == "Color").SettingValue = value;
                    ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(Color), ThemeManager.GetAppTheme(BaseColor));
                }
                OnPropertyChanged(); ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged(); ModelPropertyChanged();
            }
        }

        public string BaseColor
        {
            get { return ThemeSetting.FirstOrDefault(x => x.SettingName == "BaseColor").SettingValue; }
            set
            {
                if (ThemeSetting.FirstOrDefault(x => x.SettingName == "BaseColor").SettingValue != value)
                {
                    ThemeSetting.FirstOrDefault(x => x.SettingName == "BaseColor").SettingValue = value;
                    ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(Color), ThemeManager.GetAppTheme(BaseColor));
                }
                OnPropertyChanged(); ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged(); ModelPropertyChanged();
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
            await dataRepository.SaveAsync();
            ToastHelper.ShowSuccess("Application Theme information recorded successfully");
            Uri uri = new Uri("/View/SettingView/Setting_Page.Xaml", UriKind.Relative);
            parentpage.NavigationService.Navigate(uri);
            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(Color), ThemeManager.GetAppTheme(BaseColor));
        }
        #endregion

        #region Method
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
                else
                {
                    Color = dataRepository.Context.ApplicationSettings.AsNoTracking().FirstOrDefault(x => x.ModuleName == "ThemeSetting" && x.SettingName == "Color").SettingValue;
                    BaseColor = dataRepository.Context.ApplicationSettings.AsNoTracking().FirstOrDefault(x => x.ModuleName == "ThemeSetting" && x.SettingName == "BaseColor").SettingValue;
                    ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(Color), ThemeManager.GetAppTheme(BaseColor));
                }
            }
            Uri uri = new Uri(NavigateURL, UriKind.Relative);
            parentpage.NavigationService.Navigate(uri);
        }
        #endregion
    }
}