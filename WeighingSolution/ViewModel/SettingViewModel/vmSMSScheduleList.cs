using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;
using WeighingSolution.SharedComponents;

namespace WeighingSolution.ViewModel
{
    class vmSMSScheduleList : NotifyPropertyChanged, IDisposable
    {
        Page parentPage;
        private DataRepository dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));

        public vmSMSScheduleList(Page page)
        {
            parentPage = page;

            AllObjectList = new ObservableCollection<SMSSetup>();
            FilteredObjectList = new ObservableCollection<SMSSetup>();
            ViewList = new CollectionViewSource();
            ViewList.Filter += new FilterEventHandler(view_Filter);

            FirstCommand = new DelegateCommand(ShowFirstPage, OnFirstCanExecute);
            PreviousCommand = new DelegateCommand(ShowPreviousPage, OnPreviousCanExecute);
            NextCommand = new DelegateCommand(ShowNextPage, OnNextCanExecute);
            LastCommand = new DelegateCommand(ShowLastPage, OnLastCanExecute);
            BackToDataSetup = new DelegateCommand(NavigateBackToDataSetup);
            FilterData = new DelegateCommand(OnFilterData);
            EditObject = new DelegateCommand<object>(OnEditClicked);
            EnableDisableScheduleCommand = new DelegateCommand<SMSSetup>(OnEnableDisableScheduleCommand);
            DeleteScheduleCommand = new DelegateCommand<SMSSetup>(OnDeleteScheduleCommand);
        }

        public async void OnDeleteScheduleCommand(SMSSetup obj)
        {
            MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Are you sure?", "You want to delete this SMS schedule?");
            if (msgbox_result == MessageDialogResult.Affirmative)
            {
                var smsFieldSettings = dataRepository.Context.SMSFieldSettings.Where(x => x.SMSSetupId == obj.SMSSetupID).ToList();
                foreach (var item in smsFieldSettings)
                {
                    dataRepository.Context.SMSFieldSettings.Remove(item);
                }
                await dataRepository.SaveAsync();
                dataRepository.Context.SMSSetups.Remove(obj);
                await dataRepository.SaveAsync();
                await LoadAsync();
                ToastHelper.ShowSuccess("SMS schedule deleted successfully");
            }
        }
        public async void OnEnableDisableScheduleCommand(SMSSetup objSMSSetup)
        {
            MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Are you sure?", "You want to " + (objSMSSetup.IsEnable ? "disable" : "enable") + " this SMS schedule?");
            if (msgbox_result == MessageDialogResult.Affirmative)
            {
                objSMSSetup.IsEnable = !objSMSSetup.IsEnable;
                await dataRepository.SaveAsync();
                ViewList.View.Refresh();
            }
        }

        public async Task LoadAsync()
        {
            AllObjectList = new ObservableCollection<SMSSetup>(await dataRepository.Context.SMSSetups.ToListAsync());
            OnFilterData();
        }
        public void OnFilterData()
        {
            if (!String.IsNullOrEmpty(SearchString))
            {
                FilteredObjectList = new ObservableCollection<SMSSetup>(AllObjectList.Where(x => (!string.IsNullOrEmpty(x.ScheduleName) && x.ScheduleName.ToLower().Contains(SearchString.ToLower())) || (!string.IsNullOrEmpty(x.SMSType) && x.SMSType.ToLower().Contains(SearchString.ToLower()))).ToList());
            }
            else
            {
                FilteredObjectList = AllObjectList;
            }
            CurrentPageIndex = 0;
            CalculateTotalPages();
            ViewList.Source = FilteredObjectList;
            ViewList.View.Refresh();
        }
        #region Commands
        public DelegateCommand FirstCommand { get; private set; }
        public DelegateCommand PreviousCommand { get; private set; }
        public DelegateCommand NextCommand { get; private set; }
        public DelegateCommand LastCommand { get; private set; }
        public DelegateCommand BackToDataSetup { get; private set; }
        public DelegateCommand FilterData { get; private set; }
        public DelegateCommand<object> EditObject { get; private set; }
        public DelegateCommand<SMSSetup> EnableDisableScheduleCommand { get; private set; }
        public DelegateCommand<SMSSetup> DeleteScheduleCommand { get; private set; }
        #endregion

        #region Fields And Properties
        private int itemPerPage = 12;
        private int _currentPageIndex;
        public int CurrentPageIndex
        {
            get { return _currentPageIndex; }
            set
            {
                _currentPageIndex = value;
                OnPropertyChanged("CurrentPage");
                ((DelegateCommand)FirstCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)PreviousCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)NextCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)LastCommand).RaiseCanExecuteChanged();
            }
        }
        public int CurrentPage
        {
            get { return _currentPageIndex + 1; }
        }

        private int _totalPages;
        public int TotalPages
        {
            get { return _totalPages; }
            private set
            {
                _totalPages = value;
                OnPropertyChanged();
            }
        }

        public bool HasData
        {
            get { return _filteredObjectList.Count > 0; }
        }

        public CollectionViewSource ViewList { get; set; }

        private ObservableCollection<SMSSetup> _allObjectList;
        public ObservableCollection<SMSSetup> AllObjectList
        {
            get { return _allObjectList; }
            set
            {
                _allObjectList = value;
                OnPropertyChanged();
                OnPropertyChanged("HasData");
            }
        }

        private ObservableCollection<SMSSetup> _filteredObjectList;

        public ObservableCollection<SMSSetup> FilteredObjectList
        {
            get { return _filteredObjectList; }
            set
            {
                _filteredObjectList = value;
                OnPropertyChanged();
                OnPropertyChanged("HasData");
            }
        }


        private string _searchString;
        public string SearchString
        {
            get { return _searchString; }
            set
            {
                _searchString = value;
                OnPropertyChanged();
                OnFilterData();
            }
        }
        #endregion

        #region Pagination Methods and Commands
        private void OnEditClicked(object obj)
        {
            object[] parameters = (object[])obj;
            int ID = Convert.ToInt32(parameters[0]);
            string pageTypeFullName = string.Format("{0}.{1}", "WeighingSolution.View", parameters[1].ToString());
            Type type = Type.GetType(pageTypeFullName, true);
            parentPage.NavigationService.Navigate(Activator.CreateInstance(type, ID));
        }
        private void NavigateBackToDataSetup()
        {
            Uri uri = new Uri("/View/SettingView/Setting_Page.Xaml", UriKind.Relative);
            parentPage.NavigationService.Navigate(uri);
        }

        private bool OnFirstCanExecute()
        {
            return CurrentPageIndex != 0;
        }
        public void ShowFirstPage()
        {
            CurrentPageIndex = 0;
            ViewList.View.Refresh();
        }
        private bool OnPreviousCanExecute()
        {
            return CurrentPageIndex != 0;
        }
        public void ShowPreviousPage()
        {
            CurrentPageIndex--;
            ViewList.View.Refresh();
        }
        private bool OnNextCanExecute()
        {
            return TotalPages - 1 > CurrentPageIndex;
        }
        public void ShowNextPage()
        {
            CurrentPageIndex++;
            ViewList.View.Refresh();
        }
        private bool OnLastCanExecute()
        {
            return CurrentPage != TotalPages;
        }
        public void ShowLastPage()
        {
            CurrentPageIndex = TotalPages - 1;
            ViewList.View.Refresh();
        }
        void view_Filter(object sender, FilterEventArgs e)
        {
            int index = FilteredObjectList.IndexOf((SMSSetup)e.Item);
            if (index >= itemPerPage * CurrentPageIndex && index < itemPerPage * (CurrentPageIndex + 1))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }
        public void CalculateTotalPages()
        {
            int itemcount = FilteredObjectList.Count;
            if (itemcount % itemPerPage == 0)
            {
                TotalPages = (itemcount / itemPerPage);
            }
            else
            {
                TotalPages = (itemcount / itemPerPage) + 1;
            }
        }
        #endregion
        public void Dispose()
        {
            dataRepository.Context.Dispose();
        }
    }
}
