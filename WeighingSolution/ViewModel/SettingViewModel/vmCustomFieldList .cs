using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;
using WeighingSolution.SharedComponents;

namespace WeighingSolution.ViewModel
{
    class vmCustomFieldList : NotifyPropertyChanged, IDisposable
    {
        private Page MainPage;
        public DelegateCommand<string> CloseDetailViewCommand { get; set; }

        private TabItem _SelectedTabItem;
        public TabItem SelectedTabItem
        {
            get { return _SelectedTabItem; }
            set { _SelectedTabItem = value; OnPropertyChanged(); OnPropertyChanged("IsMasterSelected"); OnPropertyChanged("IsCustomSelected"); }
        }

        public bool IsMasterSelected { get { return _SelectedTabItem != null && _SelectedTabItem.Tag.ToString() == "Master"; } }
        public bool IsCustomSelected { get { return _SelectedTabItem != null && _SelectedTabItem.Tag.ToString() == "Custom"; } }

        public vmCustomFieldList(Page page)
        {
            MainPage = page;
            dataRepositoryCustom = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));
            dataRepositoryMaster = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));
            SaveCustomFieldCommand = new DelegateCommand(OnSaveExecute, OnSavecanExecute);
            CloseDetailViewCommand = new DelegateCommand<string>(OnCloseDetailViewExecute);
            SaveMasterFieldCommand = new DelegateCommand(OnSaveExecuteMaster, OnSavecanExecuteMaster);
        }
        public async Task LoadAsync()
        {
            CustomFields = new ObservableCollection<CustomField>(await dataRepositoryCustom.Context.CustomFields.ToListAsync());
            foreach (var item in CustomFields)
            {
                item.PropertyChanged -= new PropertyChangedEventHandler(CustomField_PropertyChanged);
                item.PropertyChanged += new PropertyChangedEventHandler(CustomField_PropertyChanged);
            }
            SaveCustomFieldCommand.RaiseCanExecuteChanged();

            MastersFields = new ObservableCollection<MasterField>(await dataRepositoryMaster.Context.MasterFields.ToListAsync());
            foreach (var item in MastersFields)
            {
                item.PropertyChanged -= new PropertyChangedEventHandler(MasterField_PropertyChanged);
                item.PropertyChanged += new PropertyChangedEventHandler(MasterField_PropertyChanged);
            }
            SaveMasterFieldCommand.RaiseCanExecuteChanged();
        }
        #region Custom Fileds
        public DataRepository dataRepositoryCustom;
        #region  Command
        public DelegateCommand SaveCustomFieldCommand { get; private set; }

        #endregion
        private ObservableCollection<CustomField> _customFields;

        public ObservableCollection<CustomField> CustomFields
        {
            get { return _customFields; }
            set { _customFields = value; OnPropertyChanged(); }
        }

        private bool _customHasChanges;
        public bool CustomHasChanges
        {
            get { return _customHasChanges; }
            set
            {
                if (_customHasChanges != value)
                {
                    _customHasChanges = value;
                    OnPropertyChanged();
                    SaveCustomFieldCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool OnSavecanExecute()
        {
            if (CustomFields == null || CustomFields.Count == 0) return false;
            return CustomFields.All(x => !x.HasErrors) && CustomHasChanges;
        }
        private async void OnSaveExecute()
        {
            await dataRepositoryCustom.SaveAsync();
            ToastHelper.ShowSuccess("CustomField information recorded successfully");
            Uri uri = new Uri("/View/SettingView/Setting_Page.Xaml", UriKind.Relative);
            MainPage.NavigationService.Navigate(uri);
        }

        private void CustomField_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!CustomHasChanges)
            {
                CustomHasChanges = dataRepositoryCustom.HasChanges();
            }
            if (e.PropertyName == "HasErrors")
            {
                SaveCustomFieldCommand.RaiseCanExecuteChanged();
            }
            else if (e.PropertyName == "IsRequired")
            {
                CustomField cf = (sender as CustomField);
                if (cf.IsRequired)
                {
                    cf.IsVisible = true;
                    cf.Editable = true; // required fields must remain editable
                }
            }
            else if (e.PropertyName == "IsVisible")
            {
                CustomField cf = (sender as CustomField);
                if (!cf.IsVisible)
                    cf.IsRequired = false;
                cf.ValidatePropertyInternal("DisplayName", cf.DisplayName);
            }
        }

        #endregion

        #region Master Field
        public DataRepository dataRepositoryMaster;
        #region  Command
        public DelegateCommand SaveMasterFieldCommand { get; private set; }

        #endregion
        private ObservableCollection<MasterField> _masterFields;
        public ObservableCollection<MasterField> MastersFields
        {
            get { return _masterFields; }
            set { _masterFields = value; OnPropertyChanged(); }
        }

        private bool _masterHasChanges;
        public bool MasterHasChanges
        {
            get { return _masterHasChanges; }
            set
            {
                if (_masterHasChanges != value)
                {
                    _masterHasChanges = value;
                    OnPropertyChanged();
                    SaveMasterFieldCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool OnSavecanExecuteMaster()
        {
            if (MastersFields == null || MastersFields.Count == 0) return false;
            return MastersFields.All(x => !x.HasErrors) && MasterHasChanges;
        }
        private async void OnSaveExecuteMaster()
        {
            await dataRepositoryMaster.SaveAsync();
            ToastHelper.ShowSuccess("MasterField information recorded successfully");
            Uri uri = new Uri("/View/SettingView/Setting_Page.Xaml", UriKind.Relative);
            MainPage.NavigationService.Navigate(uri);
        }

        private void MasterField_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!MasterHasChanges)
            {
                MasterHasChanges = dataRepositoryMaster.HasChanges();
            }
            if (e.PropertyName == "HasErrors")
            {
                SaveMasterFieldCommand.RaiseCanExecuteChanged();
            }
            else if (e.PropertyName == "IsRequired")
            {
                MasterField mf = (sender as MasterField);
                if (mf.IsRequired)
                    mf.Isvisible = true;
            }
            else if (e.PropertyName == "Isvisible")
            {
                MasterField mf = (sender as MasterField);
                if (!mf.Isvisible)
                    mf.IsRequired = false;
            }
        }
        #endregion
        private void OnCloseDetailViewExecute(string NavigateURL)
        {
            Uri uri = new Uri(NavigateURL, UriKind.Relative);
            MainPage.NavigationService.Navigate(uri);
        }
        public void Dispose()
        {
            dataRepositoryCustom.Context.Dispose();
            dataRepositoryMaster.Context.Dispose();
        }
    }
}
