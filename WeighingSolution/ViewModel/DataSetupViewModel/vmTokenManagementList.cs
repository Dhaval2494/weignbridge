using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data.Entity;
using WeighingSolution.Helpers;
using WeighingSolution.SharedComponents;
using WeighingSolution.DataLayer;
using System.Windows;

namespace WeighingSolution.ViewModel
{
    public class vmTokenManagementList : NotifyPropertyChanged, IDisposable
    {
        Page parentPage;
        public DataRepository unmanneddataRepository;
        public vmTokenManagementList(Page page)
        {
            parentPage = page;
            unmanneddataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));

            AllObjectList = new ObservableCollection<Token>();
            FilteredObjectList = new ObservableCollection<Token>();
            ViewList = new CollectionViewSource();
            ViewList.Filter += new FilterEventHandler(view_Filter);

            FirstCommand = new DelegateCommand(ShowFirstPage, OnFirstCanExecute);
            PreviousCommand = new DelegateCommand(ShowPreviousPage, OnPreviousCanExecute);
            NextCommand = new DelegateCommand(ShowNextPage, OnNextCanExecute);
            LastCommand = new DelegateCommand(ShowLastPage, OnLastCanExecute);
            FilterData = new DelegateCommand(OnFilterData);
            EditObject = new DelegateCommand<object>(OnEditClicked);
            StatusChangeCommand = new DelegateCommand<Token>(OnStatusChangeCommand);
        }

        private async void OnStatusChangeCommand(Token objtoken)
        {
            string Status = "";
            if (objtoken.TokenStatus == "Free")
                Status = "Disable";
            if (objtoken.TokenStatus == "Disable" || objtoken.TokenStatus == "Deallocated" || objtoken.TokenStatus == "Allocated")
                Status = "Free";
            if (Status != "")
            {
                MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Are you sure?", "You want to " + Status.ToLower() + " this token?");
                if (msgbox_result == MessageDialogResult.Affirmative)
                {
                    objtoken.TokenStatus = Status;
                    await unmanneddataRepository.SaveAsync();
                    ViewList.View.Refresh();
                }
            }
        }

        public async Task LoadAsync()
        {
            AllObjectList = new ObservableCollection<Token>(await unmanneddataRepository.Context.Tokens.ToListAsync());
            OnFilterData();
        }
        public void OnFilterData()
        {
            if (!String.IsNullOrEmpty(SearchString))
            {
                FilteredObjectList = new ObservableCollection<Token>(AllObjectList.Where(x => (!string.IsNullOrEmpty(x.TokenName) && x.TokenName.ToLower().Contains(SearchString.ToLower())) || (!string.IsNullOrEmpty(x.TokenValue) && x.TokenValue.ToLower().Contains(SearchString.ToLower())) || (!string.IsNullOrEmpty(x.TokenType) && x.TokenType.ToLower().Contains(SearchString.ToLower())) || ((x.Account != null) && !string.IsNullOrEmpty(x.Account.CompanyName) && x.Account.CompanyName.ToLower().Contains(SearchString.ToLower())) || ((x.Vehicle != null) && !string.IsNullOrEmpty(x.Vehicle.VehicleNumber) && x.Vehicle.VehicleNumber.ToLower().Contains(SearchString.ToLower())) || (!string.IsNullOrEmpty(x.TokenStatus) && x.TokenStatus.ToLower().Contains(SearchString.ToLower())) || (!string.IsNullOrEmpty(x.TokenInterval) && x.TokenInterval.ToLower().Contains(SearchString.ToLower()))).ToList());
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
        public DelegateCommand FilterData { get; private set; }
        public DelegateCommand<object> EditObject { get; private set; }
        public DelegateCommand<Token> StatusChangeCommand { get; private set; }
        #endregion

        #region Fields And Properties
        private int itemPerPage = 20;
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

        private ObservableCollection<Token> _allObjectList;
        public ObservableCollection<Token> AllObjectList
        {
            get { return _allObjectList; }
            set
            {
                _allObjectList = value;
                OnPropertyChanged();
                OnPropertyChanged("HasData");
            }
        }

        private ObservableCollection<Token> _filteredObjectList;

        public ObservableCollection<Token> FilteredObjectList
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
            int index = FilteredObjectList.IndexOf((Token)e.Item);
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
            unmanneddataRepository.Context.Dispose();
        }
    }
}
