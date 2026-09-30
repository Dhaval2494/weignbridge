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
    class vmEmailScheduleList : NotifyPropertyChanged, IDisposable
    {
        Page parentPage;
        private DataRepository dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));

        public vmEmailScheduleList(Page page)
        {
            parentPage = page;

            AllObjectList = new ObservableCollection<EmailSetup>();
            FilteredObjectList = new ObservableCollection<EmailSetup>();
            ViewList = new CollectionViewSource();
            ViewList.Filter += new FilterEventHandler(view_Filter);

            FirstCommand = new DelegateCommand(ShowFirstPage, OnFirstCanExecute);
            PreviousCommand = new DelegateCommand(ShowPreviousPage, OnPreviousCanExecute);
            NextCommand = new DelegateCommand(ShowNextPage, OnNextCanExecute);
            LastCommand = new DelegateCommand(ShowLastPage, OnLastCanExecute);
            BackToDataSetup = new DelegateCommand(NavigateBackToDataSetup);
            FilterData = new DelegateCommand(OnFilterData);
            EditObject = new DelegateCommand<object>(OnEditClicked);
            EnableDisableScheduleCommand = new DelegateCommand<EmailSetup>(OnEnableDisableScheduleCommand);
            DeleteScheduleCommand = new DelegateCommand<EmailSetup>(OnDeleteScheduleCommand);
        }

        public async void OnDeleteScheduleCommand(EmailSetup obj)
        {
            MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Are you sure?", "You want to delete this email schedule?");
            if (msgbox_result == MessageDialogResult.Affirmative)
            {
                dataRepository.Context.EmailSetups.Remove(obj);
                await dataRepository.SaveAsync();
                await LoadAsync();
                ToastHelper.ShowSuccess("Email schedule deleted successfully");
            }
        }
        public async void OnEnableDisableScheduleCommand(EmailSetup objEmailSetup)
        {
            MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Are you sure?", "You want to " + (objEmailSetup.IsEnable ? "disable" : "enable") + " this email schedule?");
            if (msgbox_result == MessageDialogResult.Affirmative)
            {
                objEmailSetup.IsEnable = !objEmailSetup.IsEnable;
                await dataRepository.SaveAsync();
                ViewList.View.Refresh();
            }
        }

        public async Task LoadAsync()
        {
            AllObjectList = new ObservableCollection<EmailSetup>(await dataRepository.Context.EmailSetups.ToListAsync());
            OnFilterData();
        }
        public void OnFilterData()
        {
            if (!String.IsNullOrEmpty(SearchString))
            {
                FilteredObjectList = new ObservableCollection<EmailSetup>(AllObjectList.Where(x => (!string.IsNullOrEmpty(x.ScheduleName) && x.ScheduleName.ToLower().Contains(SearchString.ToLower())) || (!string.IsNullOrEmpty(x.EmailType) && x.EmailType.ToLower().Contains(SearchString.ToLower()))).ToList());
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
        public DelegateCommand<EmailSetup> EnableDisableScheduleCommand { get; private set; }
        public DelegateCommand<EmailSetup> DeleteScheduleCommand { get; private set; }
        #endregion

        #region Fields And Properties
        private int itemPerPage = 10;
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

        private ObservableCollection<EmailSetup> _allObjectList;
        public ObservableCollection<EmailSetup> AllObjectList
        {
            get { return _allObjectList; }
            set
            {
                _allObjectList = value;
                OnPropertyChanged();
                OnPropertyChanged("HasData");
            }
        }

        private ObservableCollection<EmailSetup> _filteredObjectList;

        public ObservableCollection<EmailSetup> FilteredObjectList
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
            int index = FilteredObjectList.IndexOf((EmailSetup)e.Item);
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
