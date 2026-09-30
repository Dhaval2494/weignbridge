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
using WeighingSolution.SharedComponents;

namespace WeighingSolution.ViewModel
{
    public class vmStampCertificateList : NotifyPropertyChanged, IDisposable
    {
        Page parentPage;
        private DataRepository dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));
        public vmStampCertificateList(Page page)
        {
            parentPage = page;
            AllObjectList = new ObservableCollection<StampCertificate>();
            FilteredObjectList = new ObservableCollection<StampCertificate>();
            ViewList = new CollectionViewSource();
            ViewList.Filter += new FilterEventHandler(view_Filter);

            FirstCommand = new DelegateCommand(ShowFirstPage, OnFirstCanExecute);
            PreviousCommand = new DelegateCommand(ShowPreviousPage, OnPreviousCanExecute);
            NextCommand = new DelegateCommand(ShowNextPage, OnNextCanExecute);
            LastCommand = new DelegateCommand(ShowLastPage, OnLastCanExecute);
            BackToSetting = new DelegateCommand(NavigateBackToSetting);
            FilterData = new DelegateCommand(OnFilterData);
            ViewCertificate = new DelegateCommand<StampCertificate>(OnViewCertificateClicked);
        }



        #region Commands
        public DelegateCommand FirstCommand { get; private set; }
        public DelegateCommand PreviousCommand { get; private set; }
        public DelegateCommand NextCommand { get; private set; }
        public DelegateCommand LastCommand { get; private set; }
        public DelegateCommand BackToSetting { get; private set; }
        public DelegateCommand FilterData { get; private set; }
        public DelegateCommand<StampCertificate> ViewCertificate { get; private set; }
        #endregion

        public async Task LoadAsync()
        {
            AllObjectList = new ObservableCollection<StampCertificate>(await dataRepository.Context.StampCertificates.ToListAsync());
            OnFilterData();
        }

        private void OnViewCertificateClicked(StampCertificate currentObject)
        {

        }

        public void OnFilterData()
        {
            if (!String.IsNullOrEmpty(SearchString))
            {
                FilteredObjectList = new ObservableCollection<StampCertificate>(AllObjectList.Where(x => (!string.IsNullOrEmpty(x.StampName) && x.StampName.ToLower().Contains(SearchString.ToLower()))).ToList());
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

        private ObservableCollection<StampCertificate> _allObjectList;
        public ObservableCollection<StampCertificate> AllObjectList
        {
            get { return _allObjectList; }
            set
            {
                _allObjectList = value;
                OnPropertyChanged();
                OnPropertyChanged("HasData");
            }
        }

        private ObservableCollection<StampCertificate> _filteredObjectList;

        public ObservableCollection<StampCertificate> FilteredObjectList
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

        #region Pagination Methods and Commands

        private void NavigateBackToSetting()
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
            int index = FilteredObjectList.IndexOf((StampCertificate)e.Item);
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
