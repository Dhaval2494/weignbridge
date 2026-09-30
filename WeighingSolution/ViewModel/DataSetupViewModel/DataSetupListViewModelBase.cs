using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WeighingSolution.DataLayer;
using WeighingSolution.SharedComponents;

namespace WeighingSolution.ViewModel
{
    class DataSetupListViewModelBase<T> : NotifyPropertyChanged, IDisposable
    {
        public DataRepository dataRepository;
        public readonly IEventAggregator EventAggregator = AppEventManager.Instance.EventAggregator;
        public DataSetupListViewModelBase()
        {
            dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));

            AllObjectList = new ObservableCollection<T>();
            FilteredObjectList = new ObservableCollection<T>();
            ViewList = new CollectionViewSource();
            ViewList.Filter += new FilterEventHandler(view_Filter);

            FirstCommand = new DelegateCommand(ShowFirstPage, OnFirstCanExecute);
            PreviousCommand = new DelegateCommand(ShowPreviousPage, OnPreviousCanExecute);
            NextCommand = new DelegateCommand(ShowNextPage, OnNextCanExecute);
            LastCommand = new DelegateCommand(ShowLastPage, OnLastCanExecute);
            BackToDataSetup = new DelegateCommand(NavigateBackToDataSetup);
            FilterData = new DelegateCommand(OnFilterData);
            EditObject = new DelegateCommand<object>(OnEditClicked);
            BackToSetting = new DelegateCommand(NavigateBackToSetting);
        }

        #region Commands
        public DelegateCommand FirstCommand { get; private set; }
        public DelegateCommand PreviousCommand { get; private set; }
        public DelegateCommand NextCommand { get; private set; }
        public DelegateCommand LastCommand { get; private set; }
        public DelegateCommand BackToDataSetup { get; private set; }
        public DelegateCommand FilterData { get; private set; }
        public DelegateCommand<object> EditObject { get; private set; }
        public DelegateCommand BackToSetting { get; private set; }
        #endregion

        #region Fields And Properties
        public int itemPerPage = 10;
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

        private ObservableCollection<T> _allObjectList;
        public ObservableCollection<T> AllObjectList
        {
            get { return _allObjectList; }
            set
            {
                _allObjectList = value;
                OnPropertyChanged();
                OnPropertyChanged("HasData");
            }
        }

        private ObservableCollection<T> _filteredObjectList;

        public ObservableCollection<T> FilteredObjectList
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
            Window parentwnd = ((Window)App.Current.MainWindow);
            Frame MainFrame = parentwnd.FindName("MainFrame") as Frame;
            MainFrame.NavigationService.Navigate(Activator.CreateInstance(type, ID));
        }
        private void NavigateBackToDataSetup()
        {
            Uri uri = new Uri("/View/DatasetupView/DataSetup_Page.Xaml", UriKind.Relative);
            Window parentwnd = ((Window)App.Current.MainWindow);
            Frame MainFrame = parentwnd.FindName("MainFrame") as Frame;
            MainFrame.NavigationService.Navigate(uri);
        }
        private void NavigateBackToSetting()
        {
            Uri uri = new Uri("/View/SettingView/Setting_Page.Xaml", UriKind.Relative);
            Window parentwnd = ((Window)App.Current.MainWindow);
            Frame MainFrame = parentwnd.FindName("MainFrame") as Frame;
            MainFrame.NavigationService.Navigate(uri);
        }

        public virtual void OnFilterData()
        {
            //Let Child Classes implement
        }
        private bool OnFirstCanExecute()
        {
            return CurrentPageIndex != 0;
        }
        public void ShowFirstPage()
        {
            CurrentPageIndex = 0;
            OnFilterData();
            //ViewList.View.Refresh();
        }
        private bool OnPreviousCanExecute()
        {
            return CurrentPageIndex != 0;
        }
        public void ShowPreviousPage()
        {
            CurrentPageIndex--;
            OnFilterData();
            //ViewList.View.Refresh();
        }
        private bool OnNextCanExecute()
        {
            return TotalPages - 1 > CurrentPageIndex;
        }
        public void ShowNextPage()
        {
            CurrentPageIndex++;
            OnFilterData();
            //ViewList.View.Refresh();
        }
        private bool OnLastCanExecute()
        {
            return CurrentPage != TotalPages;
        }
        public void ShowLastPage()
        {
            CurrentPageIndex = TotalPages - 1;
            OnFilterData();
            //ViewList.View.Refresh();
        }

        void view_Filter(object sender, FilterEventArgs e)
        {
            int index = FilteredObjectList.IndexOf((T)e.Item);
            if (index >= itemPerPage * CurrentPageIndex && index < itemPerPage * (CurrentPageIndex + 1))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }
        public void CalculateTotalPages(int _count = 0)
        {
            int itemcount = _count == 0 ? FilteredObjectList.Count : _count;
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