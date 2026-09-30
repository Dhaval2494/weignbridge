using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Prism.Commands;
using WeighingSolution.DataLayer;
using WeighingSolution.View;
using System.Data.Entity;
using WeighingSolution.SharedComponents;
using MahApps.Metro.Controls.Dialogs;
using WeighingSolution.Helpers;
using Prism.Events;
using WeighingSolution.Events;
using WeighingSolution.SharedComponents.Events;
using PluginInterfaces;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;

namespace WeighingSolution.ViewModel
{
    class TransactionListViewModel : NotifyPropertyChanged, IDisposable
    {
        Page parentPage;
        public DelegateCommand ViewReceipt { get; set; }
        public DataRepository dataRepository;
        protected readonly IEventAggregator EventAggregator = AppEventManager.Instance.EventAggregator;
        public PluginLoader<IPluginControl> mef = new PluginLoader<IPluginControl>("TransactionManagementList_Page");
        public List<CustomField> CustomFields;
        public TransactionListViewModel(Page page)
        {
            parentPage = page;
            dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));

            ViewList = new ObservableCollection<TransactionDataBindingModel>();
            FirstCommand = new DelegateCommand(ShowFirstPage, OnFirstCanExecute);
            PreviousCommand = new DelegateCommand(ShowPreviousPage, OnPreviousCanExecute);
            NextCommand = new DelegateCommand(ShowNextPage, OnNextCanExecute);
            LastCommand = new DelegateCommand(ShowLastPage, OnLastCanExecute);
            FilterData = new DelegateCommand(OnFilterData);
            ViewReceipt = new DelegateCommand(OpenPopup);
            EditTransaction = new DelegateCommand<TransactionDataBindingModel>(OnEditClicked);
            VoidTransaction = new DelegateCommand<TransactionDataBindingModel>(OnVoidClicked);

            CustomFields = new List<CustomField>();

            CustomFields = dataRepository.Context.CustomFields.Where(x => x.IsVisible).OrderBy(x => x.CustomFieldID).ToList();

            foreach (var plugin in mef.loadedPlugins.Keys)
            {
                plugin.SetupPlugin(parentPage);
            }

        }

        public void OpenPopup()
        {
            if (SelectedTransaction != null)
            {
                TransactionManagementList_Page pg = parentPage as TransactionManagementList_Page;
                pg.btnViewDetail_Click(null, null);
            }
        }
        #region Commands
        public DelegateCommand FirstCommand { get; private set; }
        public DelegateCommand PreviousCommand { get; private set; }
        public DelegateCommand NextCommand { get; private set; }
        public DelegateCommand LastCommand { get; private set; }
        public DelegateCommand FilterData { get; private set; }
        public DelegateCommand<TransactionDataBindingModel> EditTransaction { get; private set; }
        public DelegateCommand<TransactionDataBindingModel> VoidTransaction { get; private set; }
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
                OnPropertyChanged("HasData");
            }
        }

        public bool HasData
        {
            get { return TotalPages > 0; }
        }

        private ObservableCollection<TransactionDataBindingModel> _viewList;

        public ObservableCollection<TransactionDataBindingModel> ViewList
        {
            get { return _viewList; }
            set
            {
                _viewList = value;
                OnPropertyChanged();
            }
        }

        private TransactionDataBindingModel _selectedTransaction;

        public TransactionDataBindingModel SelectedTransaction
        {
            get { return _selectedTransaction; }
            set { _selectedTransaction = value; OnPropertyChanged(); }
        }



        private readonly DebounceDispatcher debounceTopicsFilter = new DebounceDispatcher();
        private string _searchString;
        public string SearchString
        {
            get { return _searchString; }
            set
            {
                _searchString = value;
                OnPropertyChanged();
                debounceTopicsFilter.Debounce(350, e => { CurrentPageIndex = 0; OnFilterData(); });
            }
        }

        private bool _IsSearchByTicket;
        public bool IsSearchByTicket
        {
            get { return _IsSearchByTicket; }
            set
            {
                _IsSearchByTicket = value;
                OnPropertyChanged();
                OnFilterData();
            }
        }

        private TabItem _SelectedTabItem;
        public TabItem SelectedTabItem
        {
            get { return _SelectedTabItem; }
            set { _SelectedTabItem = value; OnPropertyChanged(); OnPropertyChanged("VoidVisible"); OnPropertyChanged("EditVisible"); CurrentPageIndex = 0; OnFilterData(); }
        }

        public bool VoidVisible { get { return _SelectedTabItem != null && _SelectedTabItem.Tag.ToString() != "Void" && Common.ValidatePageRequest(PageEntity.VoidTransaction, PageAccess.Execute); } }
        public bool EditVisible { get { return _SelectedTabItem != null && _SelectedTabItem.Tag.ToString() == "Active" && Common.ValidatePageRequest(PageEntity.Transaction, PageAccess.Create); } }

        #endregion

        #region "Data Filtering and Paging"
        public void OnFilterData()
        {
            try
            {
                int itemcount = 0;
                if (!String.IsNullOrEmpty(SearchString))
                {
                    string _Status;

                    try { _Status = SelectedTabItem.Tag.ToString(); } catch { _Status = "Active"; }

                    if (IsSearchByTicket)
                    {
                        itemcount = dataRepository.Context.TransactionDatas.Count(x => x.Status == _Status && x.TicketID.ToString().Contains(SearchString));
                    }
                    else
                    {
                        itemcount = dataRepository.Context.TransactionDatas.Count(x => x.Status == _Status &&
                                (x.TicketID.ToString().Contains(SearchString) || x.TransactionMode.Contains(SearchString)
                            || (x.Vehicle != null && x.Vehicle.VehicleNumber.ToLower().Contains(SearchString))
                            || x.CustomValue1.Contains(SearchString) || x.CustomValue2.Contains(SearchString)
                            || x.CustomValue2.Contains(SearchString) || x.CustomValue4.Contains(SearchString)
                            || x.CustomValue5.Contains(SearchString) || x.CustomValue6.Contains(SearchString)
                            || x.CustomValue7.Contains(SearchString) || x.CustomValue8.Contains(SearchString)
                            || x.CustomValue9.Contains(SearchString) || x.CustomValue10.Contains(SearchString)
                            || x.CustomValue11.Contains(SearchString) || x.CustomValue12.Contains(SearchString)
                            || x.CustomValue12.Contains(SearchString) || x.CustomValue14.Contains(SearchString)
                            || x.CustomValue15.Contains(SearchString) || x.CustomValue16.Contains(SearchString)
                            || x.CustomValue17.Contains(SearchString) || x.CustomValue18.Contains(SearchString)
                            || x.CustomValue19.Contains(SearchString) || x.CustomValue20.Contains(SearchString)
                            )
                            );
                    }
                }
                else
                {
                    itemcount = dataRepository.Context.TransactionDatas.Count(x => x.Status == SelectedTabItem.Tag.ToString());
                }

                if (itemcount % itemPerPage == 0)
                {
                    TotalPages = itemcount / itemPerPage;
                }
                else
                {
                    TotalPages = (itemcount / itemPerPage) + 1;
                }
            }
            catch (Exception Ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(Ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\TransactionListError.txt", json);
            }
            OnPageChange();
        }

        private bool _isLoading = false;
      
        public void OnPageChange()
        {
            if (_isLoading)
                return;

            try
            {

                string status = "Active";
                try { status = SelectedTabItem?.Tag?.ToString() ?? "Active"; } catch { }

                var query = dataRepository.Context.TransactionDatas.AsNoTracking()
                               .Where(x => x.Status == status);

                if (!string.IsNullOrWhiteSpace(SearchString))
                {
                    string search = SearchString.ToLower();

                    if (IsSearchByTicket)
                    {
                        query = query.Where(x => x.TicketID.ToString().Contains(search));
                    }
                    else
                    {
                        query = query.Where(x => x.TicketID.ToString().Contains(search)
                            || x.TransactionMode.Contains(search)
                            || (x.Vehicle != null && x.Vehicle.VehicleNumber.Contains(search))
                            || x.CustomValue1.Contains(SearchString) || x.CustomValue2.Contains(SearchString)
                            || x.CustomValue2.Contains(SearchString) || x.CustomValue4.Contains(SearchString)
                            || x.CustomValue5.Contains(SearchString) || x.CustomValue6.Contains(SearchString)
                            || x.CustomValue7.Contains(SearchString) || x.CustomValue8.Contains(SearchString)
                            || x.CustomValue9.Contains(SearchString) || x.CustomValue10.Contains(SearchString)
                            || x.CustomValue11.Contains(SearchString) || x.CustomValue12.Contains(SearchString)
                            || x.CustomValue12.Contains(SearchString) || x.CustomValue14.Contains(SearchString)
                            || x.CustomValue15.Contains(SearchString) || x.CustomValue16.Contains(SearchString)
                            || x.CustomValue17.Contains(SearchString) || x.CustomValue18.Contains(SearchString)
                            || x.CustomValue19.Contains(SearchString) || x.CustomValue20.Contains(SearchString)
                        );
                    }
                }

                // Apply pagination
                var transactionList = query
                    .OrderByDescending(x => x.TicketID)
                    .Skip(CurrentPageIndex * itemPerPage)
                    .Take(itemPerPage)
                    .Select(p => new TransactionDataBindingModel
                    {
                        TicketID = p.TicketID,
                        DriverID = p.DriverID,
                        Driver = p.DriverID.HasValue ? new DriverBindingModel { DriverID = p.Driver.DriverID, FirstName = p.Driver.FirstName, LastName = p.Driver.LastName } : null,
                        VehicleID = p.VehicleID,
                        Status = p.Status,
                        TransactionMode = p.TransactionMode,
                        ReceiptTicketID = p.ReceiptTicketID,
                        Charges = p.Charges,
                        TransactionType = p.TransactionType,
                        CreationTime = p.CreationTime,
                        CreatedBy = p.CreatedBy,
                        Account_Name = p.Account_Name,
                        Transporter_Name = p.Transporter_Name,
                        Vehicle_Number = p.Vehicle_Number,
                        DateTime_Format = Constant.DateTimeFormat,
                        TokenName = p.TokenName,
                        CustomValue1 = p.CustomValue1,
                        CustomValue2 = p.CustomValue2,
                        CustomValue3 = p.CustomValue3,
                        CustomValue4 = p.CustomValue4,
                        CustomValue5 = p.CustomValue5,
                        CustomValue6 = p.CustomValue6,
                        CustomValue7 = p.CustomValue7,
                        CustomValue8 = p.CustomValue8,
                        CustomValue9 = p.CustomValue9,
                        CustomValue10 = p.CustomValue10,
                        CustomValue11 = p.CustomValue11,
                        CustomValue12 = p.CustomValue12,
                        CustomValue13 = p.CustomValue13,
                        CustomValue14 = p.CustomValue14,
                        CustomValue15 = p.CustomValue15,
                        CustomValue16 = p.CustomValue16,
                        CustomValue17 = p.CustomValue17,
                        CustomValue18 = p.CustomValue18,
                        CustomValue19 = p.CustomValue19,
                        CustomValue20 = p.CustomValue20,
                    })
                    .ToList();

                var receiptIds = transactionList.Select(x => x.ReceiptTicketID).Distinct().ToList();

                var allDetails = dataRepository.Context.TransactionDetails
                    .AsNoTracking()
                    .Where(p => receiptIds.Contains(p.ReceiptTicketID))
                    .Select(p => new TransactionDetailBindingModel { TransactionDetailID = p.TransactionDetailID, WeightBridgeID = p.WeightBridgeID, WeightBridgeName = p.WeightBridgeName, SequenceNo = p.SequenceNo, ProductID = p.ProductID, GrossWeight = p.GrossWeight, TareWeight = p.TareWeight, GrossTime = p.GrossTime, TareTime = p.TareTime, WeighmentType = p.WeighmentType, GateID = p.GateID, UserID = p.UserID, CaptureWeight = p.CaptureWeight, CaptureTime = p.CaptureTime, NetWeight = p.NetWeight, WeightUnit = p.WeightUnit, IsTareManual = p.IsTareManual, IsGrossManual = p.IsGrossManual, IsCapturedManual = p.IsCapturedManual, ReceiptTicketID = p.ReceiptTicketID, GateName = p.Gate_Name, ProductName = p.Product_Name, Wbusername = p.WB_User_Name, }).ToList();

                ViewList = new ObservableCollection<TransactionDataBindingModel>(transactionList);

                foreach (var item in ViewList)
                {
                    item.CustomFieldViewModels = ParsecustomValueBindingModels(item);

                    item.TransactionDetails = new ObservableCollection<TransactionDetailBindingModel>(allDetails.Where(p => p.ReceiptTicketID == item.ReceiptTicketID).ToList());
                    //item.TransactionDetails = ParseDetailModel(item.ReceiptTicketID);
                }

                LoadTransactionDataBindingModelEventArgs viewlist = new LoadTransactionDataBindingModelEventArgs();
                viewlist.lstTranData = ViewList;
                EventAggregator.GetEvent<LoadTransactionDataBindingModelEvent>().Publish(viewlist);
                //EventAggregator.GetEvent<LoadTransactionDataEvent>().Publish(viewlist);
                ViewList = viewlist.lstTranData;
                ((DelegateCommand)FirstCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)PreviousCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)NextCommand).RaiseCanExecuteChanged();
                ((DelegateCommand)LastCommand).RaiseCanExecuteChanged();

            }
            catch (Exception Ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(Ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\TransactionListError.txt", json);
            }
            finally
            {
                _isLoading = false;
            }
        }

        private static readonly Dictionary<int, Func<TransactionDataBindingModel, string>> CustomFieldMap = new Dictionary<int, Func<TransactionDataBindingModel, string>>
        {
            {1, x => x.CustomValue1},{2, x => x.CustomValue2},{3, x => x.CustomValue3},{4, x => x.CustomValue4},{5, x => x.CustomValue5},
            {6, x => x.CustomValue6},{7, x => x.CustomValue7},{8, x => x.CustomValue8},{9, x => x.CustomValue9},{10, x => x.CustomValue10},
            {11, x => x.CustomValue11},{12, x => x.CustomValue12},{13, x => x.CustomValue13},{14, x => x.CustomValue14},{15, x => x.CustomValue15},
            {16, x => x.CustomValue16},{17, x => x.CustomValue17},{18, x => x.CustomValue18},{19, x => x.CustomValue19},{20, x => x.CustomValue20},
        };

        public ObservableCollection<CustomFieldViewModel> ParsecustomValueBindingModels(TransactionDataBindingModel transaction)
        {
            ObservableCollection<CustomFieldViewModel> customValueBindingModels = new ObservableCollection<CustomFieldViewModel>();

            foreach (var field in CustomFields)
            {
                CustomFieldMap.TryGetValue(field.CustomFieldID, out var getter);

                var vm = new CustomFieldViewModel
                {
                    CustomFieldID = field.CustomFieldID,
                    DisplayName = field.DisplayName,
                    CustomValue = getter?.Invoke(transaction)
                };

                //// Map column: CustomFieldID=1 → CustomValue1
                //string columnName = $"CustomValue{field.CustomFieldID}";

                //var prop = transaction.GetType().GetProperty(columnName);

                //if (prop != null)
                //    vm.CustomValue = prop.GetValue(transaction)?.ToString();

                customValueBindingModels.Add(vm);
            }

            return customValueBindingModels;
        }


        public ObservableCollection<TransactionDetailBindingModel> ParseDetailModel(Guid ReceiptTicketID)
        {
            ObservableCollection<TransactionDetailBindingModel> transactionDetailBindingModels = new ObservableCollection<TransactionDetailBindingModel>();

            var transactionDetails = dataRepository.Context.TransactionDetails.AsNoTracking().Where(p => p.ReceiptTicketID == ReceiptTicketID)
                .Select(p => new TransactionDetailBindingModel
                {
                    TransactionDetailID = p.TransactionDetailID,
                    WeightBridgeID = p.WeightBridgeID,
                    WeightBridgeName = p.WeightBridgeName,
                    SequenceNo = p.SequenceNo,
                    ProductID = p.ProductID,
                    GrossWeight = p.GrossWeight,
                    TareWeight = p.TareWeight,
                    GrossTime = p.GrossTime,
                    TareTime = p.TareTime,
                    WeighmentType = p.WeighmentType,
                    GateID = p.GateID,
                    UserID = p.UserID,
                    CaptureWeight = p.CaptureWeight,
                    CaptureTime = p.CaptureTime,
                    NetWeight = p.NetWeight,
                    WeightUnit = p.WeightUnit,
                    IsTareManual = p.IsTareManual,
                    IsGrossManual = p.IsGrossManual,
                    IsCapturedManual = p.IsCapturedManual,
                    ReceiptTicketID = p.ReceiptTicketID,
                    GateName = p.Gate_Name,
                    ProductName = p.Product_Name,
                    Wbusername = p.WB_User_Name,
                    //Gate = p.Gate != null ? new GateBindingModel { GateID = p.Gate.GateID, GateName = p.Gate.GateName } : null,
                    //Product = p.Product != null ? new ProductBindingModel { ProductID = p.Product.ProductID, ProductName = p.Product.ProductName } : null
                }).ToList();

            foreach (var item in transactionDetails)
            {
                transactionDetailBindingModels.Add(item);
            }

            return transactionDetailBindingModels;
        }

        public ObservableCollection<TransactionDetailBindingModel> ParseDetailModel(ObservableCollection<TransactionDetail> transactionDetails)
        {
            ObservableCollection<TransactionDetailBindingModel> transactionDetailBindingModels = new ObservableCollection<TransactionDetailBindingModel>();

            foreach (var item in transactionDetails)
            {
                TransactionDetailBindingModel transactionDetail = new TransactionDetailBindingModel()
                {
                    TransactionDetailID = item.TransactionDetailID,
                    WeightBridgeID = item.WeightBridgeID,
                    SequenceNo = item.SequenceNo,
                    ProductID = item.ProductID,
                    GrossWeight = item.GrossWeight,
                    TareWeight = item.TareWeight,
                    GrossTime = item.GrossTime,
                    TareTime = item.TareTime,
                    WeighmentType = item.WeighmentType,
                    GateID = item.GateID,
                    UserID = item.UserID,
                    CaptureWeight = item.CaptureWeight,
                    CaptureTime = item.CaptureTime,
                    NetWeight = item.NetWeight,
                    WeightUnit = item.WeightUnit,
                    IsTareManual = item.IsTareManual,
                    IsGrossManual = item.IsGrossManual,
                    IsCapturedManual = item.IsCapturedManual,
                    ReceiptTicketID = item.ReceiptTicketID,
                    GateName = item.Gate_Name,
                    ProductName = item.Product_Name,
                    Wbusername = item.WB_User_Name,
                    //Gate = item.Gate != null ? new GateBindingModel { GateID = item.Gate.GateID, GateName = item.Gate.GateName } : null,
                    //Product = item.Product != null ? new ProductBindingModel { ProductID = item.Product.ProductID, ProductName = item.Product.ProductName } : null
                };
                transactionDetailBindingModels.Add(transactionDetail);
            }

            return transactionDetailBindingModels;
        }
        #endregion

        #region Action Commands
        public void OnEditClicked(TransactionDataBindingModel objTransactionData)
        {
            MainWindow.weighCommand = false;
            parentPage.NavigationService.Navigate(new WeighingSolution.View.TransactionManagement(objTransactionData.ReceiptTicketID));
        }
        public async void OnVoidClicked(TransactionDataBindingModel objTransactionData)
        {
            MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("This action cannot be reverted.", "Are you sure you want to void this transaction?");
            if (msgbox_result == MessageDialogResult.Affirmative)
            {
                dataRepository.Context.TransactionDatas.Where(p => p.ReceiptTicketID == objTransactionData.ReceiptTicketID).FirstOrDefault().Status = TransactionStatus.Void.ToString();
                await dataRepository.SaveAsync();
                var tranData = dataRepository.Context.TransactionDatas.Where(p => p.ReceiptTicketID == objTransactionData.ReceiptTicketID).FirstOrDefault();

                EventAggregator.GetEvent<TransactionStatusUpdatedEvent>().Publish(new TransactionDetailSavedEventArgs { tranData = tranData, ReceiptTicketID = objTransactionData.ReceiptTicketID, IsTransactionComplete = true });
                OnFilterData();

                //objTransactionData.Status = TransactionStatus.Void.ToString();
                //await dataRepository.SaveAsync();
                //EventAggregator.GetEvent<TransactionStatusUpdatedEvent>().Publish(new TransactionDetailSavedEventArgs { tranData = objTransactionData, ReceiptTicketID = objTransactionData.ReceiptTicketID, IsTransactionComplete = true });
                //OnFilterData();
            }
        }
        #endregion

        #region Pagination Methods and Commands
        private bool OnFirstCanExecute()
        {
            return CurrentPageIndex != 0;
        }
        public void ShowFirstPage()
        {
            CurrentPageIndex = 0;
            OnPageChange();
        }
        private bool OnPreviousCanExecute()
        {
            return CurrentPageIndex != 0;
        }
        public void ShowPreviousPage()
        {
            CurrentPageIndex--;
            OnPageChange();
        }
        private bool OnNextCanExecute()
        {
            return TotalPages - 1 > CurrentPageIndex;
        }
        public void ShowNextPage()
        {
            CurrentPageIndex++;
            OnPageChange();
        }
        private bool OnLastCanExecute()
        {
            return CurrentPage != TotalPages;
        }
        public void ShowLastPage()
        {
            CurrentPageIndex = TotalPages - 1;
            OnPageChange();
        }
        #endregion
        public void Dispose()
        {
            foreach (var plugin in mef.loadedPlugins.Keys)
            {
                plugin.DisposePlugin();
            }
            dataRepository.Context.Dispose();
        }
    }
}