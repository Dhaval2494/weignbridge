using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using Newtonsoft.Json;
using PluginInterfaces;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UnmannedPlugin;
using WeighingSolution.DataLayer;
using WeighingSolution.Events;
using WeighingSolution.Helpers;
using WeighingSolution.SharedComponents;

namespace WeighingSolution.ViewModel
{
    public class TransactionViewModel : NotifyPropertyChanged, IDisposable
    {
        public Page parentPage;
        public DataRepository dataRepository;
        IInputElement _lastFocusedElement;
        public readonly IEventAggregator EventAggregator = AppEventManager.Instance.EventAggregator;
        public PluginLoader<IPluginControl> mef = new PluginLoader<IPluginControl>("TransactionManagement");
        public AppMode stVal = AppMode.Test;
        Button btnContinue;
        //Button btnContinueAndPrint;
        Button btnEndTransaction;
        //Button btnEndTransactionAndPrint;
        Grid gridAutomanualweight;
        RadioButton rdbAutomatic;
        RadioButton rdbManual;
        private int ActiveWeighBridgeID = 0;
        string FileExtension = ".jpeg";
        public string AnprImageLocation = string.Empty;
        private InitialTransactionEventArgs InitaialTranobj;
        public bool isUnmannedEdit;
        //private ObservableCollection<ApplicationSetting> PrintSetting; 

        public TransactionViewModel(Page page, Guid ReceiptTicketID, bool IsUnmannedEdit)
        {
            Id = ReceiptTicketID;
            parentPage = page;
            isUnmannedEdit = IsUnmannedEdit;

            SetupViewModel();
        }

        public void SetupViewModel()
        {

            CustomFieldss = new ObservableCollection<CustomFieldViewModel>();
            btnContinue = parentPage.FindName("btnContinue") as Button;
            //btnContinueAndPrint = parentPage.FindName("btnContinueAndPrint") as Button;
            btnEndTransaction = parentPage.FindName("btnEndTransaction") as Button;
            //btnEndTransactionAndPrint = parentPage.FindName("btnEndTransactionAndPrint") as Button;
            gridAutomanualweight = parentPage.FindName("gridAutomanualweight") as Grid;
            rdbAutomatic = parentPage.FindName("rdbAutomatic") as RadioButton;
            rdbManual = parentPage.FindName("rdbManual") as RadioButton;
            dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));
            SaveCommand = new DelegateCommand<string>(OnSaveExecute, OnSaveCanExecute);
            CloseDetailViewCommand = new DelegateCommand<string>(OnCloseDetailViewExecute);
            CaptureWeightCommand = new DelegateCommand<object>(OnCaptureWeightExecute, OnCaptureWeightCanExecute);
            EventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(DetailsSaved);
            EventAggregator.GetEvent<AfterLoginEvent>().Subscribe(LoginSaves);
            EventAggregator.GetEvent<ApplicationExit>().Subscribe(ReleaseReader);



            DisplayLastVehicleTransactionDetails = false;
            lstCamImage = new ObservableCollection<CameraImage>();
            AnprImageLocation = getAnprImagePath();

            try
            {
                DirectoryImageCleaner.CleanDirectoryImage(AnprImageLocation);
            }
            catch { }

            if (Constant.hasUnmannedPlugin)
            {
                EventAggregator.GetEvent<Change_tokenEvent>().Subscribe(Get_TokenName);
                Tokens = new ObservableCollection<Token>(dataRepository.Context.Tokens.Where(x => x.TokenStatus == "Free" && (x.TokenExpireDate >= DateTime.Now || x.TokenType == "Temporary")).ToList());
                ChangeTokenCommand = new DelegateCommand(OnChangeTokenExecute);
                EventAggregator.GetEvent<AfterChangeTokenEvent>().Subscribe(OnAfterchangeTokenName);
            }

        }

        private void ReleaseReader()
        {
            ActiveWeighBridges.ToList().ForEach(x => x.ReleaseReaders());
        }

        public void LoadLastTransactionList(int VehicleID)
        {
            PreviousModel = new ObservableCollection<TransactionData>(dataRepository.Context.TransactionDatas.Include("Vehicle")
                 .Where(x => x.VehicleID == VehicleID && x.Status == TransactionStatus.Complete.ToString()).OrderByDescending(x => x.CreationTime).Take(1).ToList()).FirstOrDefault();

            var lstMasterField = dataRepository.Context.MasterFields.Where(x => x.Isvisible).ToList();
            if (PreviousModel != null)
            {
                PreviousModel.AccountMaster = lstMasterField.FirstOrDefault(x => x.FieldName == MasterFieldEnum.Account.ToString());
                PreviousModel.TransporterMaster = lstMasterField.FirstOrDefault(x => x.FieldName == MasterFieldEnum.Transporter.ToString());
                PreviousModel.DriverMaster = lstMasterField.FirstOrDefault(x => x.FieldName == MasterFieldEnum.Driver.ToString());
                PreviousModel.ProductMaster = lstMasterField.FirstOrDefault(x => x.FieldName == MasterFieldEnum.Product.ToString());
                PreviousModel.GateMaster = lstMasterField.FirstOrDefault(x => x.FieldName == MasterFieldEnum.Gate.ToString());
                PreviousModel.ChargesMaster = lstMasterField.FirstOrDefault(x => x.FieldName == MasterFieldEnum.Charges.ToString());
            }
            List<ApplicationSetting> lstSlipSetting = dataRepository.Context.ApplicationSettings.Where(x => x.ModuleName == PageEntity.GlobalSetting.ToString()).ToList();

            ApplicationSetting entryDisplayLastVehicleTransactionDetails = lstSlipSetting.FirstOrDefault(x => x.SettingName == "DisplayLastVehicleTransactionDetails");
            if (entryDisplayLastVehicleTransactionDetails != null && entryDisplayLastVehicleTransactionDetails.SettingValue == "True")
                DisplayLastVehicleTransactionDetails = true;
            else
                DisplayLastVehicleTransactionDetails = false;

        }

        private bool _isSaving;
        public bool IsSaving
        {
            get { return _isSaving; }
            set { _isSaving = value; OnPropertyChanged(); SaveCommand.RaiseCanExecuteChanged(); }
        }

        // Tracks WHICH button triggered the save ("Continue" or "EndTransaction") so only
        // that specific button shows its spinner - the other stays showing normal text.
        private string _savingAction;
        public string SavingAction
        {
            get { return _savingAction; }
            set { _savingAction = value; OnPropertyChanged(); }
        }

        #region Master Lists Properties


        private ObservableCollection<Account> _accounts;
        public ObservableCollection<Account> Accounts
        {
            get { return _accounts; }
            set { _accounts = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _AutofillAccounts;
        public ObservableCollection<string> AutofillAccounts
        {
            get { return _AutofillAccounts; }
            set { _AutofillAccounts = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Account> _transporters;
        public ObservableCollection<Account> Transporters
        {
            get { return _transporters; }
            set { _transporters = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _Autofilltransporters;
        public ObservableCollection<string> AutofillTransporters
        {
            get { return _Autofilltransporters; }
            set { _Autofilltransporters = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Gate> _gates;
        public ObservableCollection<Gate> Gates
        {
            get { return _gates; }
            set { _gates = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _AutofillGates;
        public ObservableCollection<string> AutofillGates
        {
            get { return _AutofillGates; }
            set { _AutofillGates = value; OnPropertyChanged(); }
        }

        private ObservableCollection<VehicleBindingModel> _vehicles;
        public ObservableCollection<VehicleBindingModel> Vehicles
        {
            get { return _vehicles; }
            set { _vehicles = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _Autofillvehicles;
        public ObservableCollection<string> Autofillvehicles
        {
            get { return _Autofillvehicles; }
            set { _Autofillvehicles = value; OnPropertyChanged("Autofillvehicles"); }
        }

        private ObservableCollection<DriverBindingModel> _drivers;
        public ObservableCollection<DriverBindingModel> Drivers
        {
            get { return _drivers; }
            set { _drivers = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _AutofillDrivers;
        public ObservableCollection<string> AutofillDrivers
        {
            get { return _AutofillDrivers; }
            set { _AutofillDrivers = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ProductBindingModel> _products;
        public ObservableCollection<ProductBindingModel> Products
        {
            get { return _products; }
            set { _products = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _AutofillProducts;
        public ObservableCollection<string> AutofillProducts
        {
            get { return _AutofillProducts; }
            set { _AutofillProducts = value; OnPropertyChanged(); }
        }

        private ObservableCollection<WeightBridge> _activeweighbridges;
        public ObservableCollection<WeightBridge> ActiveWeighBridges
        {
            get { return _activeweighbridges; }
            set { _activeweighbridges = value; OnPropertyChanged(); }
        }

        private ObservableCollection<CustomFieldViewModel> _customFieldss;
        public ObservableCollection<CustomFieldViewModel> CustomFieldss
        {
            get => _customFieldss;
            set
            {
                if (_customFieldss != value)
                {
                    _customFieldss = value;
                    OnPropertyChanged(nameof(CustomFieldss));
                }
            }
        }

        private Camera _ActiveCamera;
        public Camera ActiveCamera
        {
            get { return _ActiveCamera; }
            set
            {
                _ActiveCamera = value;
                OnPropertyChanged();
            }
        }

        private bool _IsviewMoreCamera;
        public bool IsviewMoreCamera
        {
            get { return _IsviewMoreCamera; }
            set { _IsviewMoreCamera = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Camera> _CameraList;
        public ObservableCollection<Camera> CameraList
        {
            get => _CameraList;
            set
            {
                _CameraList = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CameraImage> _lstCamImage;
        public ObservableCollection<CameraImage> lstCamImage
        {
            get { return _lstCamImage; }
            set
            {
                _lstCamImage = value;
                OnPropertyChanged();
            }
        }
        private int? _selectedCameraId;
        public int? SelectedCameraId
        {
            get => _selectedCameraId;
            set
            {
                _selectedCameraId = value;
                OnPropertyChanged(nameof(SelectedCameraId));
            }
        }

        #endregion

        #region Application Settings

        private bool _isDisplayReceiptAfter1pass;
        public bool IsDisplayReceiptAfter1pass
        {
            get { return _isDisplayReceiptAfter1pass; }
            set { _isDisplayReceiptAfter1pass = value; OnPropertyChanged(); }
        }

        private bool _isAllowWithoutTareWeightChange;
        public bool IsAllowWithoutTareWeightChange
        {
            get { return _isAllowWithoutTareWeightChange; }
            set { _isAllowWithoutTareWeightChange = value; OnPropertyChanged(); }
        }
        private bool _controlsEnabled;
        public bool ControlsEnabled
        {
            get { return _controlsEnabled; }
            set { _controlsEnabled = value; OnPropertyChanged(); }
        }

        private bool _entryDateEditable;
        public bool EntryDateEditable
        {
            get { return _entryDateEditable; }
            set { _entryDateEditable = value; OnPropertyChanged(); }
        }

        private bool _DisplayLastVehicleTransactionDetails;
        public bool DisplayLastVehicleTransactionDetails
        {
            get { return _DisplayLastVehicleTransactionDetails; }
            set { _DisplayLastVehicleTransactionDetails = value; OnPropertyChanged(); }
        }

        //private bool _isPrintSetup;
        //public bool IsPrintSetup
        //{
        //    get { return _isPrintSetup; }
        //    set { _isPrintSetup = value; OnPropertyChanged("IsPrintSetup"); }
        //}

        private decimal _tareWeightDiff;
        public decimal TareWeightDiff
        {
            get { return _tareWeightDiff; }
            set { _tareWeightDiff = value; }
        }

        private bool _editMode;
        public bool EditMode
        {
            get { return _editMode; }
            set { _editMode = value; OnPropertyChanged(); }
        }

        private Decimal _LastTicketID;
        public Decimal LastTicketID
        {
            get { return _LastTicketID; }
            set { _LastTicketID = value; OnPropertyChanged(); }
        }
        public List<ApplicationSetting> ApplicationSettingsList { get; set; }
        #endregion

        #region Command and Properties Declaration
        public DelegateCommand<string> SaveCommand { get; private set; }
        public DelegateCommand<string> CloseDetailViewCommand { get; private set; }
        public DelegateCommand<object> CaptureWeightCommand { get; private set; }
        public DelegateCommand ChangeTokenCommand { get; private set; }
        public int? LastTicket
        {
            get
            {
                try
                {
                    return getMaxTicketid();
                    //return Convert.ToInt32(dataRepository.Context.ApplicationSettings.Where(p => p.ModuleName == "Transaction").FirstOrDefault(x => x.SettingName == "LastTicket").SettingValue);
                }
                catch
                {
                    return null;
                }
            }

        }

        public int? getMaxTicketid()
        {
            try
            {
                string Query = "select Max(Ticketid) as 'TicketCount' from transactionData";
                DataTable dataTable = DatabaseFunctions.SQLExecuteReaderQuery(Query);
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    int? ticketCount = dataTable.Rows[0].Field<int?>("TicketCount");
                    return ticketCount ?? 0;   // ✅ return 0 if NULL
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }


        private ObservableCollection<Token> _Tokens;
        public ObservableCollection<Token> Tokens
        {
            get { return _Tokens; }
            set
            {
                _Tokens = value; OnPropertyChanged();
                //EventAggregator.GetEvent<ChangetokenEvent>().Publish(new ChangetokenEventArgs { objTranTokenLog = TokenModel });
            }
        }

        private string _SelectedToken;

        public string SelectedToken
        {
            get { return _SelectedToken; }
            set
            {
                _SelectedToken = value;
                if (SelectedToken != null)
                {
                    OnchangeToken();


                }
                OnPropertyChanged();
            }
        }

        private TransactionTokenLog _TokenModel;

        public TransactionTokenLog TokenModel
        {
            get { return _TokenModel; }
            set { _TokenModel = value; OnPropertyChanged(); }
        }

        private bool _IsChangeTokenVisible;

        public bool IsChangeTokenVisible
        {
            get { return _IsChangeTokenVisible; }
            set { _IsChangeTokenVisible = value; OnPropertyChanged(); }
        }

        //Used to reload entity in case of DB Concurrency issue
        private Guid _id;
        public Guid Id
        {
            get { return _id; }
            protected set { _id = value; }
        }
        //Used to track changes to the Model
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
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _VehicleNumberText;
        public string VehicleNumberText
        {
            get { return _VehicleNumberText; }
            set
            {
                if (_VehicleNumberText != value)
                {
                    _VehicleNumberText = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _AnprNumberText;
        public string AnprNumberText
        {
            get { return _AnprNumberText; }
            set
            {
                if (_AnprNumberText != value)
                {
                    _AnprNumberText = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _TransaporterNameText;
        public string TransaporterNameText
        {
            get { return _TransaporterNameText; }
            set
            {
                if (_TransaporterNameText != value)
                {
                    _TransaporterNameText = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _AccountNameText;
        public string AccountNameText
        {
            get { return _AccountNameText; }
            set
            {
                if (_AccountNameText != value)
                {
                    _AccountNameText = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _DriverNameText;
        public string DriverNameText
        {
            get { return _DriverNameText; }
            set
            {
                if (_DriverNameText != value)
                {
                    _DriverNameText = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _ProductNameText;
        public string ProductNameText
        {
            get { return _ProductNameText; }
            set
            {
                if (_ProductNameText != value)
                {
                    _ProductNameText = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _GateNameText;
        public string GateNameText
        {
            get { return _GateNameText; }
            set
            {
                if (_GateNameText != value)
                {
                    _GateNameText = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _PageLoadText;
        public string PageLoadText
        {
            get { return _PageLoadText; }
            set { _PageLoadText = value; OnPropertyChanged(); }
        }

        private bool _IsPageLoaded;
        public bool IsPageLoaded
        {
            get { return _IsPageLoaded; }
            set { _IsPageLoaded = value; OnPropertyChanged(); }
        }

        private TransactionData _model;
        public TransactionData Model
        {
            get { return _model; }
            set { _model = value; OnPropertyChanged(); }
        }

        private TransactionData _Previousmodel;
        public TransactionData PreviousModel
        {
            get { return _Previousmodel; }
            set { _Previousmodel = value; OnPropertyChanged(); }
        }

        private WeightBridge _selectedWeighBridge;
        public WeightBridge SelectedWeighBridge
        {
            get { return _selectedWeighBridge; }
            set
            {
                _selectedWeighBridge = value;
                if (_activeTransaction != null)
                {
                    _activeTransaction.WeightBridgeID = _selectedWeighBridge.WeightBridgeID;
                    _activeTransaction.WeightBridge = _selectedWeighBridge;
                    LoadCamera(SelectedWeighBridge);
                }
                OnPropertyChanged();
                EventAggregator.GetEvent<ActiveWeghBridgeChangeEvent>().Publish(new ActiveWeghBridgeChangeEventArgs { actiavtedWeighBridge = _selectedWeighBridge });
            }
        }

        private TransactionDetail _activeTransaction;
        public TransactionDetail ActiveTransaction
        {
            get { return _activeTransaction; }
            set { _activeTransaction = value; OnPropertyChanged(); }
        }

        private TransactionDetail _previousTransaction;
        public TransactionDetail PreviousTransaction
        {
            get { return _previousTransaction; }
            set { _previousTransaction = value; OnPropertyChanged(); }
        }

        private Camera _LeftAnpr;
        public Camera LeftAnpr
        {
            get { return _LeftAnpr; }
            set { _LeftAnpr = value; OnPropertyChanged(); }
        }

        private Camera _RightAnpr;
        public Camera RightAnpr
        {
            get { return _RightAnpr; }
            set { _RightAnpr = value; OnPropertyChanged(); }
        }

        private string _Base64ImageString;
        public string Base64ImageString
        {
            get { return _Base64ImageString; }
            set
            {
                if (_Base64ImageString != value)
                {
                    _Base64ImageString = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        public static string getAnprImagePath()
        {
            try
            {
                using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                {
                    ApplicationSetting CameraSetting = ctx.ApplicationSettings.FirstOrDefault(x => x.ModuleName == "AnprSetting" && x.SettingName == "AnprImagePath");
                    if (CameraSetting != null && CameraSetting.SettingValue != "")
                    {
                        Common.CreateDirectoryWithFullAccess(CameraSetting.SettingValue);

                        return CameraSetting.SettingValue;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            catch
            {
                return string.Empty;
            }

        }

        #region Tocken Methods
        private void OnTransactionInitial(InitialTransactionEventArgs obj)
        {
            if (obj != null && obj.TranData != null)
            {
                TransactionData Tranobj = obj.TranData as TransactionData;
                TokenModel = dataRepository.Context.TransactionTokenLogs.FirstOrDefault(x => x.ReceiptTicketID == Tranobj.ReceiptTicketID && !x.IsCancel);
                if (TokenModel != null)
                {
                    ControlsEnabled = false;        //Token Combobox Control
                    IsChangeTokenVisible = true;    //Change Token Button visibility
                    if ((Tranobj.TransactionMode == "Single" || Tranobj.TransactionMode == "Double") && Tranobj.TransactionDetails.Any(x => x.NetWeight != null))
                        IsChangeTokenVisible = false;
                    if (Tokens.All(x => x.TokenID != TokenModel.TokenID))
                    {
                        Token objToken = dataRepository.Context.Tokens.FirstOrDefault(x => x.TokenID == TokenModel.TokenID);
                        Tokens.Add(objToken);
                    }
                    SelectedToken = dataRepository.Context.Tokens.FirstOrDefault(x => x.TokenID == TokenModel.TokenID).TokenID.ToString();
                    parentPage = obj.page as Page;
                }
                else
                {
                    //If token is not selected in first transaction
                    //if (Tranobj.TransactionDetails.Count > 1)  
                    //{
                    //    ControlsEnabled = false;
                    //    IsChangeTokenVisible = false;
                    //}
                    //else
                    //{
                    ControlsEnabled = true;
                    IsChangeTokenVisible = false;
                    TokenModel = new TransactionTokenLog();
                    //dataRepository.Context.TransactionTokenLogs.Add(TokenModel);
                    Model.TransactionTokenLogs.Add(TokenModel);
                    InitaialTranobj = obj;
                    //}
                }
                TokenModel.PropertyChanged -= Model_PropertyChanged;
                TokenModel.PropertyChanged += Model_PropertyChanged;
            }
        }

        private void OnSaveValidModel(CheckModelvalidEventArgs obj)
        {
            if (TokenModel != null)
            {
                bool Currentrmodel = TokenModel.IsValid();
                if (obj.IsValid)
                    obj.IsValid = Currentrmodel;
            }
        }
        private void OnTransactionDetailSaved(TransactionDetailSavedEventArgs obj)
        {
            try
            {
                if (obj != null && TokenModel != null && TokenModel.Token != null)
                {
                    if (obj.tranData.TransactionDetails.Count == 1)
                    {
                        TokenModel.IsCancel = false;
                        TokenModel.ReceiptTicketID = obj.tranData.ReceiptTicketID;
                        TokenModel.AllocateTime = DateTime.Now;
                        TokenModel.Token.TokenStatus = "Allocated";
                    }
                    if (obj.IsTransactionComplete)
                    {
                        TokenModel.DeallocateTime = DateTime.Now;
                        if (TokenModel.Token.TokenType == "Temporary" || (TokenModel.Token.TokenType == "Permanent" && (obj.tranData.CreatedBy != null || TokenModel.Token.TokenExpireDate < DateTime.Now)))
                            TokenModel.Token.TokenStatus = "Deallocated";
                    }
                    dataRepository.Context.SaveChanges();
                }
            }
            catch { }
        }
        private void OnAfterchangeTokenName(ChangetokenEventArgs obj)
        {
            if (obj != null && obj.objTranTokenLog != null)
            {
                TokenModel = dataRepository.Context.TransactionTokenLogs.FirstOrDefault(x => x.TransactionTokenLogID == obj.objTranTokenLog.TransactionTokenLogID);
                SelectedToken = TokenModel.TokenID.ToString();
            }
        }
        private void OnChangeTokenExecute()
        {
            //fillmapping();
            ChangeTokenWindow childWindow = new ChangeTokenWindow(parentPage);
            ChildWindowManager.ShowChildWindowAsync(Application.Current.MainWindow, childWindow);
            EventAggregator.GetEvent<ChangetokenEvent>().Publish(new ChangetokenEventArgs { objTranTokenLog = TokenModel });
        }
        private void OnchangeToken()
        {
            if (InitaialTranobj != null)
            {
                TransactionData Tranobj = InitaialTranobj.TranData as TransactionData;
                TransactionDetail TranDetail = InitaialTranobj.TranDetail as TransactionDetail;
                Tranobj.VehicleID = null;
                Tranobj.TransactionType = null;
                Tranobj.AccountID = null;
                Tranobj.TransporterID = null;
                TranDetail.GateID = null;
                Tranobj.DriverID = null;
                TranDetail.ProductID = null;

                Tranobj.Charges = null;
                TokenModel.Token = Tokens.FirstOrDefault(x => x.TokenID == Convert.ToInt32(SelectedToken));
                TokenModel.TokenID = TokenModel.Token.TokenID;
                if (TokenModel.Token != null)
                {
                    string Tokenname = TokenModel.Token.TokenName;
                    //int _Tokenname = Convert.ToInt32(Tokenname);

                    if (TokenModel.Token.VehicleID != null)
                        Tranobj.VehicleID = TokenModel.Token.VehicleID;
                    if (TokenModel.Token.AccountID != null)
                        Tranobj.AccountID = TokenModel.Token.AccountID;
                    if (TokenModel.Token.TransporterID != null)
                        Tranobj.TransporterID = TokenModel.Token.TransporterID;
                    if (TokenModel.Token.GateID != null)
                        TranDetail.GateID = TokenModel.Token.GateID;
                    if (TokenModel.Token.DriverID != null)
                        Tranobj.DriverID = TokenModel.Token.DriverID;
                    if (TokenModel.Token.ProductID != null)
                        TranDetail.ProductID = TokenModel.Token.ProductID;
                    if (TokenModel.Token.Charges != null)
                        Tranobj.Charges = TokenModel.Token.Charges;
                    if (TokenModel.Token.PreferredTransactionMode != null)
                        Tranobj.TransactionMode = TokenModel.Token.PreferredTransactionMode;

                    if (TokenModel.Token.PreferredWeighmentType != null)
                        TranDetail.WeighmentType = TokenModel.Token.PreferredWeighmentType;

                    if (TokenModel.Token.PreferredTransactionType != null)
                        Tranobj.TransactionType = TokenModel.Token.PreferredTransactionType;

                    if (!string.IsNullOrEmpty(TokenModel.Token.Vehicle_Number))
                    {
                        VehicleNumberText = TokenModel.Token.Vehicle_Number;
                        Tranobj.Vehicle_Number = TokenModel.Token.Vehicle_Number;
                    }

                    if (!string.IsNullOrEmpty(TokenModel.Token.Driver_Name))
                    {
                        Tranobj.Driver_Name = TokenModel.Token.Driver_Name;
                        DriverNameText = TokenModel.Token.Driver_Name;
                    }
                    if (!string.IsNullOrEmpty(TokenModel.Token.Transporter_Name))
                    {
                        Tranobj.Transporter_Name = TokenModel.Token.Transporter_Name;
                        TransaporterNameText = TokenModel.Token.Transporter_Name;
                    }
                    if (!string.IsNullOrEmpty(TokenModel.Token.Account_Name))
                    {
                        Tranobj.Account_Name = TokenModel.Token.Account_Name;
                        AccountNameText = TokenModel.Token.Account_Name;
                    }

                    if (!string.IsNullOrEmpty(TokenModel.Token.Product_Name))
                    {
                        TranDetail.Product_Name = TokenModel.Token.Product_Name;
                        ProductNameText = TokenModel.Token.Product_Name;
                    }

                    if (!string.IsNullOrEmpty(TokenModel.Token.Gate_Name))
                    {
                        TranDetail.Gate_Name = TokenModel.Token.Gate_Name;
                        GateNameText = TokenModel.Token.Gate_Name;
                    }

                    if (!string.IsNullOrEmpty(TokenModel.Token.CustomValue1))
                        Tranobj.CustomValue1 = TokenModel.Token.CustomValue1;

                    if (!string.IsNullOrEmpty(TokenModel.Token.CustomValue2))
                        Tranobj.CustomValue2 = TokenModel.Token.CustomValue2;

                    if (!string.IsNullOrEmpty(TokenModel.Token.CustomValue3))
                        Tranobj.CustomValue3 = TokenModel.Token.CustomValue3;

                    if (!string.IsNullOrEmpty(TokenModel.Token.CustomValue4))
                        Tranobj.CustomValue4 = TokenModel.Token.CustomValue4;

                    if (!string.IsNullOrEmpty(TokenModel.Token.CustomValue5))
                        Tranobj.CustomValue5 = TokenModel.Token.CustomValue5;

                    if (!string.IsNullOrEmpty(TokenModel.Token.CustomValue6))
                        Tranobj.CustomValue6 = TokenModel.Token.CustomValue6;

                    if (!string.IsNullOrEmpty(TokenModel.Token.CustomValue7))
                        Tranobj.CustomValue7 = TokenModel.Token.CustomValue7;

                    if (!string.IsNullOrEmpty(TokenModel.Token.CustomValue8))
                        Tranobj.CustomValue8 = TokenModel.Token.CustomValue8;

                    if (!string.IsNullOrEmpty(TokenModel.Token.CustomValue9))
                        Tranobj.CustomValue9 = TokenModel.Token.CustomValue9;

                    if (!string.IsNullOrEmpty(TokenModel.Token.CustomValue10))
                        Tranobj.CustomValue10 = TokenModel.Token.CustomValue10;

                    if (!string.IsNullOrEmpty(TokenModel.Token.CustomValue11))
                        Tranobj.CustomValue11 = TokenModel.Token.CustomValue11;

                    if (!string.IsNullOrEmpty(TokenModel.Token.CustomValue12))
                        Tranobj.CustomValue12 = TokenModel.Token.CustomValue12;

                    if (!string.IsNullOrEmpty(TokenModel.Token.CustomValue13))
                        Tranobj.CustomValue13 = TokenModel.Token.CustomValue13;

                    if (!string.IsNullOrEmpty(TokenModel.Token.CustomValue14))
                        Tranobj.CustomValue14 = TokenModel.Token.CustomValue14;

                    if (!string.IsNullOrEmpty(TokenModel.Token.CustomValue15))
                        Tranobj.CustomValue15 = TokenModel.Token.CustomValue15;

                    if (!string.IsNullOrEmpty(TokenModel.Token.CustomValue16))
                        Tranobj.CustomValue16 = TokenModel.Token.CustomValue16;

                    if (!string.IsNullOrEmpty(TokenModel.Token.CustomValue17))
                        Tranobj.CustomValue17 = TokenModel.Token.CustomValue17;

                    if (!string.IsNullOrEmpty(TokenModel.Token.CustomValue18))
                        Tranobj.CustomValue18 = TokenModel.Token.CustomValue18;

                    if (!string.IsNullOrEmpty(TokenModel.Token.CustomValue19))
                        Tranobj.CustomValue19 = TokenModel.Token.CustomValue19;

                    if (!string.IsNullOrEmpty(TokenModel.Token.CustomValue20))
                        Tranobj.CustomValue20 = TokenModel.Token.CustomValue20;
                    PopulateExistingModel(Tranobj);
                }
            }
        }
        #endregion
        public async Task LoadAsync()
        {
            try
            {
                IsPageLoaded = false;
                PageLoadText = ".";
                stVal = (AppMode)Enum.Parse(typeof(AppMode), System.Configuration.ConfigurationManager.AppSettings["AppMode"]);
                foreach (var plugin in mef.loadedPlugins.Keys)
                {
                    try
                    {
                        plugin.SetupPlugin(parentPage);
                    }
                    catch (Exception Ex)
                    {
                        string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(Ex);
                        File.AppendAllText(Constant.ApplicationDataLocation + @"\TransactionManagementError.txt", json);
                    }
                }

                ObservableCollection<WeightBridgeConfigBindingModel> WeightBridgesConfig = vmGlobelSetting.ReadWeightBridgeConfig();

                bool IsMultiWeighBridgePlugin = LicensePluginHalper.CheckPluginInstall(PluginNameEnum.MultiWeighBridgePlugin.ToString());
                if (IsMultiWeighBridgePlugin)
                {
                    Page txPage = parentPage;
                    DockPanel dockMain = txPage.FindName("dockMain") as DockPanel;
                    StackPanel stkMultiWeighBridge = txPage.FindName("stkMultiWeighBridge") as StackPanel;
                    using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                    {
                        List<WeightBridge> lstWB = new List<WeightBridge>();
                        if (WeightBridgesConfig != null && WeightBridgesConfig.Count() > 0)
                        {
                            var _WeightBridgeID = WeightBridgesConfig.FirstOrDefault().WeightBridgeID;
                            lstWB = ctx.WeightBridges.Where(x => x.WeightBridgeID == _WeightBridgeID && x.IsActive).ToList();
                        }
                        else
                            lstWB = ctx.WeightBridges.Where(x => x.IsActive).ToList();


                        //List<WeightBridge> lstWB = ctx.WeightBridges.Where(x => x.IsActive).ToList();
                        if (lstWB != null && lstWB.Count > 1)
                        {
                            stkMultiWeighBridge.Visibility = Visibility.Visible;
                            stkMultiWeighBridge.Width = 120;
                            ListView listView = GetControll.GetChildOfType<ListView>(stkMultiWeighBridge);
                            listView.MaxHeight = 630;
                        }
                    }
                }
                var transaction = Id != Guid.Empty
                  ? await dataRepository.Context.TransactionDatas.SingleAsync(x => x.ReceiptTicketID == Id)
                  : CreateNewModel();

                if (WeightBridgesConfig != null && WeightBridgesConfig.Count() > 0)
                {
                    var _WeightBridgeID = WeightBridgesConfig.FirstOrDefault().WeightBridgeID;
                    ActiveWeighBridges = new ObservableCollection<WeightBridge>(await dataRepository.Context.WeightBridges.Where(x => x.WeightBridgeID == _WeightBridgeID && x.IsActive == true).ToListAsync());
                }
                else
                    ActiveWeighBridges = new ObservableCollection<WeightBridge>(await dataRepository.Context.WeightBridges.Where(x => x.IsActive == true).ToListAsync());


                InitializeModel(transaction);

                if (SelectedWeighBridge == null)
                {
                    SelectedWeighBridge = ActiveWeighBridges.Where(x => x.IsActive).FirstOrDefault();
                }

                if (Model.TransactionDetails.Count > 0)
                {
                    if (!Constant.hasUnmannedPlugin)
                        PreviousTransaction = Model.TransactionDetails.OrderByDescending(x => x.SequenceNo).FirstOrDefault(x => x.CaptureWeight != null && x.CaptureWeight.Value > 0);
                    else
                        PreviousTransaction = Model.TransactionDetails.OrderByDescending(x => x.SequenceNo).FirstOrDefault();

                }

                SetupCustomFields();
                InitializeActiveTransaction();
                Model.OnPropertyChanged("TransactionMode");
                SetupApplicationSettings();

                if (Constant.hasUnmannedPlugin)
                    OnTransactionInitial(new InitialTransactionEventArgs { TranDetail = ActiveTransaction, TranData = Model, page = parentPage });
                SetupMasterFields();
                EventAggregator.GetEvent<EditTransactionDetailEvent>().Publish(new EditTransactionDetailEventArgs { lstTranDetails = Model.TransactionDetails });
                EventAggregator.GetEvent<InitialTransactionEvent>().Publish(new InitialTransactionEventArgs { TranDetail = ActiveTransaction, TranData = Model, page = parentPage });
                LoadCamera(SelectedWeighBridge);
                await SetupMasterListsAsync();
            }
            catch (Exception ex)
            {

            }
        }

        public async Task SetupMasterListsAsync()
        {
            try
            {
                //var timer = new Stopwatch();
                //timer.Start();

                //Vehicles = new ObservableCollection<Vehicle>(await dataRepository.Context.Vehicles.Where(x => x.IsActive).ToListAsync());

                Vehicles = new ObservableCollection<VehicleBindingModel>(await dataRepository.Context.Vehicles.Where(x => x.IsActive).Select(x => new VehicleBindingModel { VehicleID = x.VehicleID, VehicleNumber = x.VehicleNumber, TareWeight = x.TareWeight }).ToListAsync());
                //Autofillvehicles = new ObservableCollection<string>(Vehicles.OrderBy(P => P.VehicleNumber).Select(p => p.VehicleNumber).Distinct().ToList());

                if (Model.AccountMaster != null)
                {
                    Accounts = new ObservableCollection<Account>(await dataRepository.Context.Accounts.Where(x => x.IsAccount != null && x.IsAccount.Value && x.Active).ToListAsync());
                    //Accounts = new ObservableCollection<AccountBindingModel>(await dataRepository.Context.Accounts.Where(x => x.IsAccount != null && x.IsAccount.Value && x.Active).Select(x => new AccountBindingModel { AccountID = x.AccountID, CompanyName = x.CompanyName }).ToListAsync());
                    //AutofillAccounts = new ObservableCollection<string>(Accounts.OrderBy(P => P.CompanyName).Select(p => p.CompanyName).Distinct().ToList());
                }

                if (Model.TransporterMaster != null)
                {
                    Transporters = new ObservableCollection<Account>(await dataRepository.Context.Accounts.Where(x => x.IsTransporter != null && x.IsTransporter.Value && x.Active == true).ToListAsync());
                    //Transporters = new ObservableCollection<AccountBindingModel>(await dataRepository.Context.Accounts.Where(x => x.IsTransporter != null && x.IsTransporter.Value && x.Active == true).Select(x => new AccountBindingModel { AccountID = x.AccountID, CompanyName = x.CompanyName }).ToListAsync());
                    //AutofillTransporters = new ObservableCollection<string>(Transporters.OrderBy(P => P.CompanyName).Select(p => p.CompanyName).Distinct().ToList());
                }

                if (Model.DriverMaster != null)
                {
                    //Drivers = new ObservableCollection<Driver>(await dataRepository.Context.Drivers.Where(x => x.Active).ToListAsync());
                    Drivers = new ObservableCollection<DriverBindingModel>(await dataRepository.Context.Drivers.Where(x => x.Active).Select(x => new DriverBindingModel { DriverID = x.DriverID, FirstName = x.FirstName, LastName = x.LastName }).ToListAsync());
                    //AutofillDrivers = new ObservableCollection<string>(Drivers.OrderBy(P => P.DriverName).Select(p => p.DriverName).Distinct().ToList());
                }

                if (Model.ProductMaster != null)
                {
                    //Products = new ObservableCollection<Product>(await dataRepository.Context.Products.Where(x => x.IsActive).ToListAsync());
                    Products = new ObservableCollection<ProductBindingModel>(await dataRepository.Context.Products.Where(x => x.IsActive).Select(x => new ProductBindingModel { ProductID = x.ProductID, ProductName = x.ProductName }).ToListAsync());
                    //AutofillProducts = new ObservableCollection<string>(Products.OrderBy(P => P.ProductName).Select(p => p.ProductName).Distinct().ToList());
                }

                if (Model.GateMaster != null)
                {
                    Gates = new ObservableCollection<Gate>(await dataRepository.Context.Gates.Where(x => x.IsActive).ToListAsync());
                    //AutofillGates = new ObservableCollection<string>(Gates.OrderBy(P => P.GateName).Select(p => p.GateName).Distinct().ToList());
                }
                PageLoadText = "";

                IsPageLoaded = true;

                SaveCommand.RaiseCanExecuteChanged();

                //timer.Stop();

                //TimeSpan timeTaken = timer.Elapsed;
                //string ExecuteTime = "Time taken: " + timeTaken.ToString(@"m\:ss\.fff");

                //string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + ExecuteTime;
                //File.AppendAllText(Constant.PluginFileLocation + @"\testtime.txt", json);
            }
            catch (Exception ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.PluginFileLocation + @"\TransactionError.txt", json);
            }
        }

        private void SetupMasterFields()
        {
            var lstMasterField = dataRepository.Context.MasterFields.Where(x => x.Isvisible).ToList();
            Model.AccountMaster = lstMasterField.FirstOrDefault(x => x.FieldName == MasterFieldEnum.Account.ToString());
            Model.TransporterMaster = lstMasterField.FirstOrDefault(x => x.FieldName == MasterFieldEnum.Transporter.ToString());
            Model.DriverMaster = lstMasterField.FirstOrDefault(x => x.FieldName == MasterFieldEnum.Driver.ToString());
            Model.ProductMaster = lstMasterField.FirstOrDefault(x => x.FieldName == MasterFieldEnum.Product.ToString());
            //Model.RFIDTokenMaster = lstMasterField.FirstOrDefault(x => x.FieldName == MasterFieldEnum.RFIDToken.ToString());
            Model.GateMaster = lstMasterField.FirstOrDefault(x => x.FieldName == MasterFieldEnum.Gate.ToString());
            Model.ChargesMaster = lstMasterField.FirstOrDefault(x => x.FieldName == MasterFieldEnum.Charges.ToString());
        }
        private void SetupApplicationSettings()
        {
            EditMode = (Model.TransactionDetails.Count > 1);
            if (Constant.hasUnmannedPlugin)
            {
                EditMode = Model.TransactionMode == TransactionMode.Single.ToString() ? Model.TransactionDetails.Any(x => x.NetWeight != null) : EditMode;
            }

            ApplicationSettingsList = dataRepository.Context.ApplicationSettings.ToList();
            List<ApplicationSetting> lstSlipSetting = ApplicationSettingsList?.Where(x => x.ModuleName == PageEntity.GlobalSetting.ToString()).ToList();

            ApplicationSetting disble2ndpass = lstSlipSetting.FirstOrDefault(x => x.SettingName == "DisbleFrom2ndPass");
            if (disble2ndpass != null && Convert.ToBoolean(disble2ndpass.SettingValue) == true && EditMode)
                ControlsEnabled = false;
            else
                ControlsEnabled = true;

            ApplicationSetting AllowWithoutTareWeightChange = lstSlipSetting.FirstOrDefault(x => x.SettingName == "AllowWithoutTareWeightChange");
            if (AllowWithoutTareWeightChange != null && Convert.ToBoolean(AllowWithoutTareWeightChange.SettingValue) == true && EditMode)
                IsAllowWithoutTareWeightChange = false;
            else
                IsAllowWithoutTareWeightChange = true;

            ApplicationSetting displayReceiptAfter1pass = lstSlipSetting.FirstOrDefault(x => x.SettingName == "DisplayReceiptAfter1pass");
            if (displayReceiptAfter1pass != null && Convert.ToBoolean(displayReceiptAfter1pass.SettingValue) == true)
                IsDisplayReceiptAfter1pass = true;
            else
                IsDisplayReceiptAfter1pass = false;

            ApplicationSetting entryDateEditable = lstSlipSetting.FirstOrDefault(x => x.SettingName == "EntryDateEditable");
            if (entryDateEditable != null && entryDateEditable.SettingValue == "True")
                EntryDateEditable = true;
            else
                EntryDateEditable = false;


            ApplicationSetting tareweightDiff = lstSlipSetting.FirstOrDefault(x => x.SettingName == "TareWeightDiff");
            if (tareweightDiff != null && !String.IsNullOrEmpty(tareweightDiff.SettingValue) && Decimal.TryParse(tareweightDiff.SettingValue, out decimal dummy))
                TareWeightDiff = dummy;

            //IsPrintSetup = false;

            //PrintSetting = new ObservableCollection<ApplicationSetting>(dataRepository.Context.ApplicationSettings.Where(x => x.ModuleName == "PrintSetting").ToList());

            //if (PrintSetting != null && PrintSetting.Count > 0)
            //{
            //    if (PrintSetting.Where(x => x.SettingName == "PrintType" && !string.IsNullOrEmpty(x.SettingValue)).Count() > 0)
            //    {
            //        if (PrintSetting.Where(x => x.SettingName == "PrinterName" && !string.IsNullOrEmpty(x.SettingValue)).Count() > 0)
            //        {
            //            IsPrintSetup = true;
            //        }
            //    }
            //}

        }


        //private void SetupCustomFields()
        //{
        //    var lstCustomFields = dataRepository.Context.CustomFields.Where(x => x.IsVisible && x.DisplayName != "").ToList();

        //    var newList = new ObservableCollection<CustomFieldViewModel>();
        //    foreach (var x in lstCustomFields)
        //    {
        //        string columnName = $"CustomValue{x.CustomFieldID}";
        //        string sql = $"SELECT DISTINCT [{columnName}] FROM TransactionData";
        //        var values = dataRepository.Context.Database.SqlQuery<string>(sql).ToList();
        //        newList.Add(new CustomFieldViewModel
        //        {
        //            DisplayName = x.DisplayName,
        //            IsVisible = x.IsVisible,
        //            IsRequired = x.IsRequired,
        //            FieldValues = new ObservableCollection<string>(values),
        //            CustomValue = string.Empty,
        //            ColumnName = columnName
        //        });
        //    }

        //    CustomFieldss = newList;
        //}

        private void SetupCustomFields()
        {
            var lstCustomFields = dataRepository.Context.CustomFields
                .Where(x => x.IsVisible && x.DisplayName != "")
                .ToList();

            var newList = new ObservableCollection<CustomFieldViewModel>();

            foreach (var x in lstCustomFields)
            {
                string columnName = $"CustomValue{x.CustomFieldID}";

                string sql = $"SELECT DISTINCT [{columnName}] FROM TransactionData";

                var values = dataRepository.Context.Database
     .SqlQuery<string>(sql)
     .Where(v => !string.IsNullOrWhiteSpace(v))
     .ToList();




                var defaultVal = x.DefaultValue ?? string.Empty;
                if (!string.IsNullOrEmpty(defaultVal) && !values.Contains(defaultVal))
                    values.Insert(0, defaultVal);

                newList.Add(new CustomFieldViewModel
                {
                    DisplayName = x.DisplayName,
                    IsVisible = x.IsVisible,
                    IsRequired = x.IsRequired,
                    Editable = x.Editable,          // <-- NEW
                    FieldValues = new ObservableCollection<string>(values),
                    CustomValue = defaultVal,
                    ColumnName = columnName
                });
                //Debug.WriteLine($"Setting CustomValue = '{x.DefaultValue}' for {x.DisplayName}");
            }

            CustomFieldss = newList;
        }

        #region Methods
        public void OpenFlyout(UserControl uc)
        {
            _lastFocusedElement = FocusManager.GetFocusedElement(parentPage);
            MetroWindow mw = ((MetroWindow)App.Current.MainWindow);
            Flyout flyoutContainer = (Flyout)mw.FindName("flyoutContainer");
            Flyout flyChangePassword = (Flyout)mw.FindName("flyChangePassword");
            flyoutContainer.Content = uc;
            flyChangePassword.IsOpen = false;
            flyoutContainer.IsOpen = true;
            flyoutContainer.ClosingFinished -= flyoutContainer_ClosingFinished;
            flyoutContainer.ClosingFinished += flyoutContainer_ClosingFinished;
        }

        void flyoutContainer_ClosingFinished(object sender, RoutedEventArgs e)
        {
            if (_lastFocusedElement != null)
            {
                Keyboard.Focus(_lastFocusedElement);
            }

        }

        private void LoginSaves(AfterLoginEventargs obj)
        {
            try
            {
                if (obj.TemplateID == 1)
                {
                    rdbAutomatic.Visibility = Visibility.Visible;
                    rdbManual.Visibility = Visibility.Visible;
                }

            }
            catch (Exception ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.PluginFileLocation + @"\ManualLoginError.txt", json);
            }
        }

        public void DetailsSaved(AfterDetailSavedEventArgs obj)
        {
            try
            {
                if (obj.ObjectInstance is Driver)
                {
                    Driver objDriver = obj.ObjectInstance as Driver;
                    Driver objAttachedEntity = dataRepository.Context.Drivers.FirstOrDefault(x => x.DriverID == objDriver.DriverID);
                    //Drivers.Add(objAttachedEntity);
                    Drivers.Add(new DriverBindingModel { DriverID = objAttachedEntity.DriverID, FirstName = objAttachedEntity.FirstName, LastName = objAttachedEntity.LastName });
                    //AutofillDrivers.Add(objAttachedEntity.DriverName);
                    Model.DriverID = objAttachedEntity.DriverID;
                    Model.Driver_Name = objAttachedEntity.DriverName;
                    Model.Driver = objAttachedEntity;
                    if (DriverNameText != objDriver.DriverName)
                        DriverNameText = objDriver.DriverName;
                }
                else if (obj.ObjectInstance is Vehicle)
                {

                    Vehicle objVehicle = obj.ObjectInstance as Vehicle;

                    VehicleBindingModel vehicleBindingModel = Vehicles.Where(p => p.VehicleID == objVehicle.VehicleID).FirstOrDefault();

                    Vehicles.Remove(vehicleBindingModel);
                    Vehicles.Add(new VehicleBindingModel { VehicleID = objVehicle.VehicleID, VehicleNumber = objVehicle.VehicleNumber, TareWeight = objVehicle.TareWeight });

                    //VehicleBindingModel objAttachedEntity;
                    //Vehicle dbVehicleEntity;
                    //if (Vehicles.Any(x => x.VehicleID == objVehicle.VehicleID))
                    //{
                    //    dbVehicleEntity = dataRepository.Context.Vehicles.FirstOrDefault(x => x.VehicleID == objVehicle.VehicleID);
                    //    dataRepository.Context.Entry(dbVehicleEntity).Reload();
                    //    objAttachedEntity = Vehicles.FirstOrDefault(x => x.VehicleID == objVehicle.VehicleID);
                    //}
                    //else
                    //{
                    //    dbVehicleEntity = dataRepository.Context.Vehicles.FirstOrDefault(x => x.VehicleID == objVehicle.VehicleID);
                    //    objAttachedEntity = new VehicleBindingModel { VehicleID = dbVehicleEntity.VehicleID, VehicleNumber = dbVehicleEntity.VehicleNumber, TareWeight = dbVehicleEntity.TareWeight };
                    //    Vehicles.Add(objAttachedEntity);
                    //    //Autofillvehicles.Add(objAttachedEntity.VehicleNumber);
                    //}
                    Model.VehicleID = objVehicle.VehicleID;
                    Model.Vehicle_Number = objVehicle.VehicleNumber;
                    Model.Vehicle = objVehicle;

                    if (VehicleNumberText != objVehicle.VehicleNumber)
                        VehicleNumberText = objVehicle.VehicleNumber;

                }
                else if (obj.ObjectInstance is Product)
                {
                    Product objProduct = obj.ObjectInstance as Product;
                    Product objAttachedEntity = dataRepository.Context.Products.FirstOrDefault(x => x.ProductID == objProduct.ProductID);
                    //Products.Add(objAttachedEntity);
                    Products.Add(new ProductBindingModel { ProductID = objAttachedEntity.ProductID, ProductName = objAttachedEntity.ProductName, IsActive = objAttachedEntity.IsActive });
                    //AutofillProducts.Add(objAttachedEntity.ProductName);
                    ActiveTransaction.ProductID = objAttachedEntity.ProductID;
                    ActiveTransaction.Product_Name = objAttachedEntity.ProductName;
                    ActiveTransaction.Product = objAttachedEntity;
                    if (ProductNameText != objProduct.ProductName)
                        ProductNameText = objProduct.ProductName;
                }
                else if (obj.ObjectInstance is Gate)
                {
                    Gate objGate = obj.ObjectInstance as Gate;
                    Gate objAttachedEntity = dataRepository.Context.Gates.FirstOrDefault(x => x.GateID == objGate.GateID);
                    Gates.Add(objAttachedEntity);
                    //AutofillGates.Add(objAttachedEntity.GateName);
                    ActiveTransaction.GateID = objAttachedEntity.GateID;
                    ActiveTransaction.Gate_Name = objAttachedEntity.GateName;
                    ActiveTransaction.Gate = objAttachedEntity;
                    if (GateNameText != objGate.GateName)
                        GateNameText = objGate.GateName;
                }
                else if (obj.ObjectInstance is Account)
                {
                    Account objAccount = obj.ObjectInstance as Account;
                    Account objAttachedEntity = dataRepository.Context.Accounts.FirstOrDefault(x => x.AccountID == objAccount.AccountID);
                    if (objAttachedEntity.IsAccount.GetValueOrDefault(false))
                    {
                        Accounts.Add(objAttachedEntity);
                        //Accounts.Add(new AccountBindingModel { AccountID = objAttachedEntity.AccountID, CompanyName = objAttachedEntity.CompanyName });
                        //AutofillAccounts.Add(objAttachedEntity.CompanyName);
                        Model.AccountID = objAttachedEntity.AccountID;
                        Model.Account_Name = objAttachedEntity.CompanyName;
                        Model.Account = objAttachedEntity;
                        if (AccountNameText != objAccount.CompanyName)
                            AccountNameText = objAccount.CompanyName;
                    }
                    if (objAttachedEntity.IsTransporter.GetValueOrDefault(false))
                    {
                        Transporters.Add(objAttachedEntity);
                        //Transporters.Add(new AccountBindingModel { AccountID = objAttachedEntity.AccountID, CompanyName = objAttachedEntity.CompanyName });
                        //AutofillTransporters.Add(objAttachedEntity.CompanyName);
                        Model.TransporterID = objAttachedEntity.AccountID;
                        Model.Transporter_Name = objAttachedEntity.CompanyName;
                        Model.Transporter = objAttachedEntity;
                        if (TransaporterNameText != objAccount.CompanyName)
                            TransaporterNameText = objAccount.CompanyName;
                    }
                }
            }
            catch (Exception ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.PluginFileLocation + @"\TransactionError.txt", json);
            }

        }

        public int? GetTicketNO()
        {
            try
            {
                int? LastTicketid = LastTicket;
                if (LastTicketid == null)
                    return null;
                else
                {
                    int? Last_Ticket = LastTicketid + 1;
                    return Last_Ticket;
                }
            }
            catch
            {
                return null;
            }
        }

        protected bool OnSaveCanExecute(string action)
        {
            if (IsSaving)
                return false;

            if (string.IsNullOrWhiteSpace(AccountNameText) &&
                Model?.AccountMaster?.IsRequired == true)
            {
                return false;
            }

            if (action == "Continue" && btnContinue.Visibility != Visibility.Visible)
                return false;
            else if (IsPageLoaded == false)
                return false;
            else if (ActiveTransaction != null && ((!Constant.hasUnmannedPlugin && ActiveTransaction.CaptureWeight.GetValueOrDefault(0) <= 0) || (Constant.hasUnmannedPlugin && Model.Vehicle == null)))
                return false;
            else if (Model != null && ActiveTransaction != null && ((!Constant.hasUnmannedPlugin && Model.TransactionMode == TransactionMode.Single.ToString() && ActiveTransaction.NetWeight.GetValueOrDefault(0) <= 0) || (Constant.hasUnmannedPlugin && Model.TransactionMode == TransactionMode.Single.ToString() && Model.Vehicle.TareWeight.GetValueOrDefault(0) <= 0)))
                return false;
            return Model != null && !Model.HasErrors && ActiveTransaction != null && !ActiveTransaction.HasErrors;
        }
        protected void OnCloseDetailViewExecute(string NavigateURL)
        {
            Uri uri = new Uri(NavigateURL, UriKind.Relative);
            parentPage.NavigationService.Navigate(uri);
            EventAggregator.GetEvent<AfterDetailSaveCancelledEvent>().Publish(new AfterDetailSaveCancelledEventArgs() { ObjectInstance = Model });
        }
        public void OnVehicleSave(string VehicleNumber, bool ForActive = false)
        {
            if (string.IsNullOrEmpty(VehicleNumber.Trim()))
                return;

            try
            {
                VehicleNumber = VehicleNumber.Trim();
                VehicleNumber = VehicleNumber.ToUpper();
                VehicleNumber = VehicleNumber.Replace(" ", "");

                if (ForActive)
                {
                    string query = $"Update Vehicle set IsActive = 1 where VehicleNumber = '{VehicleNumber}'";
                    bool flag = DatabaseFunctions.ExecuteQuery(query);
                    Thread.Sleep(500);
                }

                //var _vehicles = VehicledataRepository.Context.Vehicles.Where(p => p.VehicleNumber == VehicleNumber);
                var _vehicles = dataRepository.Context.Vehicles.Where(p => p.VehicleNumber == VehicleNumber);
                if (_vehicles.Count() > 0)
                {
                    var vehicle = _vehicles.FirstOrDefault();
                    EventAggregator.GetEvent<AfterDetailSavedEvent>().Publish(new AfterDetailSavedEventArgs
                    {
                        ObjectID = vehicle.VehicleID,
                        ObjectInstance = vehicle,
                        ObjectType = vehicle.GetType().Name
                    });
                }
                else
                {
                    string query = $"INSERT INTO Vehicle (VehicleNumber, IsActive) VALUES('{VehicleNumber}', 1)";
                    bool flag = DatabaseFunctions.ExecuteQuery(query);
                    if (flag)
                    {
                        Thread.Sleep(500);
                        //var _vehiclesAdd = VehicledataRepository.Context.Vehicles.Where(p => p.VehicleNumber == VehicleNumber);
                        var _vehiclesAdd = dataRepository.Context.Vehicles.Where(p => p.VehicleNumber == VehicleNumber);
                        if (_vehiclesAdd.Count() > 0)
                        {
                            var vehicle = _vehiclesAdd.FirstOrDefault();
                            EventAggregator.GetEvent<AfterDetailSavedEvent>().Publish(new AfterDetailSavedEventArgs
                            {
                                ObjectID = vehicle.VehicleID,
                                ObjectInstance = vehicle,
                                ObjectType = vehicle.GetType().Name
                            });
                        }

                        ToastHelper.ShowSuccess($"Vehicle Number {VehicleNumber} save Successfully");
                    }
                }
            }
            catch (Exception ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.PluginFileLocation + @"\TransactionError.txt", json);
            }
        }

        public void OnVehicleRefresh(string VehicleNumber)
        {
            if (string.IsNullOrEmpty(VehicleNumber.Trim()))
                return;

            try
            {
                VehicleNumber = VehicleNumber.Trim();
                VehicleNumber = VehicleNumber.ToUpper();
                VehicleNumber = VehicleNumber.Replace(" ", "");

                //var _vehicles = VehicledataRepository.Context.Vehicles.Where(p => p.VehicleNumber == VehicleNumber);
                var _vehicles = dataRepository.Context.Vehicles.Where(p => p.VehicleNumber == VehicleNumber);
                if (_vehicles.Count() > 0)
                {
                    var vehicle = _vehicles.FirstOrDefault();
                    EventAggregator.GetEvent<AfterDetailSavedEvent>().Publish(new AfterDetailSavedEventArgs
                    {
                        ObjectID = vehicle.VehicleID,
                        ObjectInstance = vehicle,
                        ObjectType = vehicle.GetType().Name
                    });
                }
            }
            catch (Exception ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.PluginFileLocation + @"\TransactionError.txt", json);
            }
        }

        public async Task SaveNewTransaction(bool EndTransaction, bool IsPrint)
        {
            if (EndTransaction && Model.TransactionDetails.Any(x => x.CaptureWeight == null || x.CaptureWeight.Value == 0) && Model.TransactionMode != TransactionMode.Multi.ToString())
            {
                await MessageBoxHelper.ShowInfoDialogAsync("Error!", "This transaction can not complete, due to this transaction has incompelete pass.");
                return;
            }
            Model.Status = EndTransaction ? TransactionStatus.Complete.ToString() : TransactionStatus.Active.ToString();

            if (Constant.hasUnmannedPlugin)
            {
                if (TokenModel != null || TokenModel.TokenID != 0)
                {
                    Model.TokenName = TokenModel.Token.TokenName;
                    Model.TokenValue = TokenModel.Token.TokenValue;
                }
            }

            if (Model.TransactionMode == TransactionMode.Double.ToString() && Model.TransactionDetails.Count == 2)
            {
                Model.Status = TransactionStatus.Complete.ToString();
            }

            if (Model.TransactionMode == TransactionMode.Double.ToString() && Model.TransactionDetails.Count > 2)
            {
                // DETACH the ActiveTransaction before nulling it
                if (ActiveTransaction != null)
                {
                    dataRepository.Context.Entry(ActiveTransaction).State = EntityState.Detached;
                }
                ActiveTransaction = null;
                Model.Status = TransactionStatus.Complete.ToString();
                Frame MainFrame = ((MetroWindow)App.Current.MainWindow).FindName("MainFrame") as Frame;
                MainFrame.Content = new View.ReceiptPreview(Model.ReceiptTicketID, true);
            }

            bool IsCameraPlugin = LicensePluginHalper.CheckPluginInstall(PluginNameEnum.CameraPlugin.ToString());
            if (IsCameraPlugin)
            {
                CameraImageSaved(new TransactionDetailSavedEventArgs { tranData = Model, tranDetail = ActiveTransaction, ReceiptTicketID = Model.ReceiptTicketID, IsTransactionComplete = EndTransaction });
            }

            if (Model.TicketID == 0)
            {
                int? ticketid = GetTicketNO();
                if (ticketid != null)
                {
                    Model.TicketID = (int)ticketid;
                }
                else
                {
                    MessageBox.Show("Please wait Network has gone click again");
                    return;
                }
            }
            OnTransactionDetailSaved(new TransactionDetailSavedEventArgs { tranData = Model, tranDetail = ActiveTransaction, ReceiptTicketID = Model.ReceiptTicketID, IsTransactionComplete = EndTransaction });

            await dataRepository.SaveAsync();
            EventAggregator.GetEvent<TransactionDetailSavedEvent>().Publish(new TransactionDetailSavedEventArgs { tranData = Model, tranDetail = ActiveTransaction, ReceiptTicketID = Model.ReceiptTicketID, IsTransactionComplete = EndTransaction });
            await dataRepository.SaveAsync();
            Thread.Sleep(500);

            // ---- Fire configured client API integrations (SWAMA, Odoo, etc.) ----
            // Runs on every save (OnSave) and/or only when the ticket is finalized
            // (OnEndTransaction), per how each integration is configured in the API Master screen.
            try
            {
                string trigger = EndTransaction ? "OnEndTransaction" : "OnSave";
                var activeIntegrations = dataRepository.Context.ApiIntegrationMasters
                    .Where(x => x.IsActive && (x.TriggerOn == trigger || x.TriggerOn == "Both"))
                    .OrderBy(x => x.SortOrder)
                    .ToList();

                if (activeIntegrations.Count > 0)
                {
                    var apiService = new ApiIntegrationService();
                    bool allSucceeded = true;   // becomes false if ANY configured integration fails

                    foreach (var integ in activeIntegrations)
                    {
                        var apiResult = await apiService.ExecuteAsync(integ, Model, ActiveTransaction);
                        if (!apiResult.IsSuccess) allSucceeded = false;

                        dataRepository.Context.ApiIntegrationLogs.Add(new ApiIntegrationLog
                        {
                            ApiIntegrationID = integ.ApiIntegrationID,
                            TicketID = Model.TicketID,
                            ReceiptTicketID = Model.ReceiptTicketID,
                            RequestUrl = apiResult.RequestUrl,
                            RequestBody = RedactBase64ForLogging(apiResult.RequestBody),
                            ResponseBody = apiResult.ResponseBody,
                            StatusCode = apiResult.StatusCode,
                            IsSuccess = apiResult.IsSuccess,
                            ErrorMessage = apiResult.ErrorMessage,
                            CreatedTime = DateTime.Now
                        });

                        if (!apiResult.IsSuccess)
                            ToastHelper.ShowError($"{integ.Name} sync failed: Please check log!");
                        //ToastHelper.ShowError($"{integ.Name} sync failed: {apiResult.ErrorMessage ?? apiResult.ResponseBody}");
                    }

                    // Overall flag on the ticket itself - true only if EVERY configured
                    // integration succeeded this time. Lets you filter the transaction
                    // list for "PushApi = 0" to find tickets that still need re-sending.
                    Model.PushAPI = allSucceeded;
                    dataRepository.Context.SaveChanges();
                }
            }
            catch (Exception apiEx)
            {
                // A client API being down should never break the local weighing save.
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(apiEx);
                File.AppendAllText(Constant.PluginFileLocation + @"\ApiIntegrationCallError.txt", json);
            }
            ToastHelper.ShowSuccess("Transaction compeleted successfully");

            UnloadCamera();

            //if (EndTransaction)
            //{
            //    Post_Weighmentdetails(Model);
            //}

            if (EndTransaction && Model.TransactionMode == TransactionMode.Double.ToString() && IsAllowWithoutTareWeightChange)
            {
                decimal tareDiff = 0.00m;
                Vehicle vehicleInTran = Model.Vehicle;
                if (vehicleInTran != null && vehicleInTran.TareWeight != null)
                {
                    tareDiff = Math.Abs(100 - (Math.Min(vehicleInTran.TareWeight.Value, ActiveTransaction.TareWeight.Value) / Math.Max(ActiveTransaction.TareWeight.Value, vehicleInTran.TareWeight.Value) * 100));

                    if (vehicleInTran != null && ActiveTransaction.TareWeight != vehicleInTran.TareWeight && tareDiff > 0 && TareWeightDiff > 0 && tareDiff > TareWeightDiff)
                    {
                        MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Information", "Vehicle tare weight change, you wish to change tare weight of this vehicle?");
                        if (msgbox_result == MessageDialogResult.Affirmative)
                        {
                            Model.Vehicle.TareWeight = ActiveTransaction.TareWeight.Value;
                            Model.Vehicle.TareWeightTime = ActiveTransaction.TareTime.Value;
                            await dataRepository.SaveAsync();
                        }
                    }
                }
                else
                {
                    Model.Vehicle.TareWeight = ActiveTransaction.TareWeight.Value;
                    Model.Vehicle.TareWeightTime = ActiveTransaction.TareTime.Value;
                    await dataRepository.SaveAsync();
                }
            }
            NamedPipeManager.Write("Transaction Save");
            if (Model.TransactionDetails.Count == 1 && (Model.TransactionDetails.FirstOrDefault().CaptureWeight == null || Model.TransactionDetails.FirstOrDefault().CaptureWeight.Value == 0))
            {
                Uri uri = new Uri("/View/TransactionView/TransactionManagementList_Page.xaml", UriKind.Relative);
                parentPage.NavigationService.Navigate(uri);
            }
            else
            {
                if (IsDisplayReceiptAfter1pass)
                {
                    Frame MainFrame = ((MetroWindow)App.Current.MainWindow).FindName("MainFrame") as Frame;
                    if (!IsPrint)
                        MainFrame.Content = new View.ReceiptPreview(Model.ReceiptTicketID, true);
                    else
                        MainFrame.Content = new View.ReceiptPrint(Model.ReceiptTicketID, true);
                }
                else
                {
                    Frame MainFrame = ((MetroWindow)App.Current.MainWindow).FindName("MainFrame") as Frame;
                    if (EndTransaction)
                    {
                        if (!IsPrint)
                            MainFrame.Content = new View.ReceiptPreview(Model.ReceiptTicketID, true);
                        else
                            MainFrame.Content = new View.ReceiptPrint(Model.ReceiptTicketID, true);
                    }
                    else
                    {
                        Uri uri = new Uri("/View/TransactionView/TransactionManagementList_Page.xaml", UriKind.Relative);
                        parentPage.NavigationService.Navigate(uri);
                    }
                }
            }
        }

        private static readonly Regex Base64DataUriRegex =
           new Regex(@"data:[^;""]+;base64,[A-Za-z0-9+/]+={0,2}", RegexOptions.Compiled);

        private static string RedactBase64ForLogging(string requestBody)
        {
            if (string.IsNullOrEmpty(requestBody))
                return requestBody;

            return Base64DataUriRegex.Replace(requestBody, m =>
            {
                string prefix = m.Value.Substring(0, m.Value.IndexOf("base64,") + "base64,".Length);
                int base64Length = m.Value.Length - prefix.Length;
                return $"{prefix}[REDACTED:{base64Length} chars]";
            });
        }

        protected async void OnSaveExecute(string action)
        {

            if (!Constant.hasUnmannedPlugin)
            {
                if (ActiveTransaction.CaptureWeight.GetValueOrDefault(0) <= 0)
                {
                    ToastHelper.ShowError("Please capture weight");
                    return;
                }
                else if (Model.TransactionMode == TransactionMode.Single.ToString() && ActiveTransaction.NetWeight.GetValueOrDefault(0) <= 0)
                {
                    ToastHelper.ShowError("Please capture weight");
                    return;
                }
            }
            else
            {
                if (TokenModel == null || TokenModel.TokenID == 0)
                {
                    ToastHelper.ShowError("Enter Token  no");
                    return;
                }
            }
            //Model Validations (Main Data, Child Data and all Custom Values
            CheckModelvalidEventArgs Isvalidpluginmodel = new CheckModelvalidEventArgs();
            Isvalidpluginmodel.IsValid = true;
            OnSaveValidModel(Isvalidpluginmodel);
            EventAggregator.GetEvent<CheckModelvalidEvent>().Publish(Isvalidpluginmodel);
            if (Model.IsValid() && Isvalidpluginmodel.IsValid && ActiveTransaction.IsValid())
            {

                bool IsPrint = false;
                if (action.Contains("Print"))
                    IsPrint = true;

                bool EndTransaction = true;
                if (Model.TransactionMode == TransactionMode.Multi.ToString() || Model.TransactionMode == TransactionMode.Double.ToString() || (Model.TransactionMode == TransactionMode.Single.ToString() && Constant.hasUnmannedPlugin))
                    EndTransaction = action == "EndTransaction" || action == "EndTransactionAndPrint";

                if (EndTransaction && Model.TransactionMode == TransactionMode.Double.ToString() && !IsAllowWithoutTareWeightChange)
                {
                    decimal tareDiff = 0.00m;
                    Vehicle vehicleInTran = Model.Vehicle;
                    if (vehicleInTran != null && vehicleInTran.TareWeight != null)
                    {
                        tareDiff = Math.Abs(100 - (Math.Min(vehicleInTran.TareWeight.Value, ActiveTransaction.TareWeight.Value) / Math.Max(ActiveTransaction.TareWeight.Value, vehicleInTran.TareWeight.Value) * 100));

                        if (vehicleInTran != null && ActiveTransaction.TareWeight != vehicleInTran.TareWeight && tareDiff > 0 && TareWeightDiff > 0 && tareDiff > TareWeightDiff)
                        {
                            MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Information", "Vehicle tare weight change, you wish to change tare weight of this vehicle?");
                            if (msgbox_result == MessageDialogResult.Negative)
                            {
                                return;
                            }
                            else
                            {
                                Model.Vehicle.TareWeight = ActiveTransaction.TareWeight.Value;
                                Model.Vehicle.TareWeightTime = ActiveTransaction.TareTime.Value;
                            }
                        }
                    }
                }

                // assign custom values dynamically and safely
                if (CustomFieldss != null && CustomFieldss.Count > 0)
                {
                    for (int i = 0; i < CustomFieldss.Count && i < 20; i++)
                    {
                        var field = CustomFieldss[i];
                        if (field == null || !field.Editable)   // <-- skip non-editable fields
                            continue;
                        if (field == null)
                            continue; // skip invalid entry

                        var value = field.CustomValue;

                        // ✅ Check if required field is blank
                        if (field.IsRequired && string.IsNullOrWhiteSpace(value))
                        {
                            ToastHelper.ShowError($"Please fill in required field: {field.DisplayName} Missing Required Field");
                            // Stop further processing and return (exit the method)
                            return;
                        }

                        // Default to empty if not required and blank
                        value = string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

                        // Get corresponding property on TransactionData
                        var property = typeof(TransactionData).GetProperty($"CustomValue{i + 1}");
                        if (property != null && property.CanWrite)
                        {
                            try
                            {
                                property.SetValue(Model, value);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error setting CustomValue{i + 1}: {ex.Message}");
                            }
                        }
                    }
                }

                try
                {
                    IsSaving = true;
                    SavingAction = action;
                    await SaveNewTransaction(EndTransaction, IsPrint);
                }
                catch (DbEntityValidationException ex)
                {

                    string Error = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine;
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        Error += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        Error += Environment.NewLine;
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Error += string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                            Error += Environment.NewLine;
                        }
                    }

                    File.AppendAllText(Constant.PluginFileLocation + @"\TransactionSaveError.txt", Error);

                    throw;
                }
                catch (Exception ex)
                {
                    ToastHelper.ShowError("Failed to save transaction. Error: " + ex.Message);
                    string Error = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                    File.AppendAllText(Constant.PluginFileLocation + @"\TransactionSaveError.txt", Error);
                    App.Log.Error(ex.Message, ex);
                }
                finally
                {
                    IsSaving = false;
                    SavingAction = null;
                }
            }
        }
        private bool OnCaptureWeightCanExecute(object param = null)
        {
            return Model != null && Model.VehicleID > 0 && Model.Vehicle != null && gridAutomanualweight.Visibility == Visibility.Visible;
        }
        //Set values for: WeightUnit, CaptureTime,CapturedWeight, (IsCapturedManual - Already bound)
        protected async void OnCaptureWeightExecute(object param)
        {
            //Evaluates to false only if live mode, with PLC Enabled and Non-Alignment
            //|| !ActiveTransaction.WeightBridge.PLCEnabled || SelectedWeighBridge.IsAligned
            BeforeWeightCaptureEventArgs objCaptureArgs = new BeforeWeightCaptureEventArgs();
            objCaptureArgs.isAligned = true;
            CheckModelvalidEventArgs Isvalidpluginmodel = new CheckModelvalidEventArgs();
            Isvalidpluginmodel.IsValid = true;
            if (Constant.hasUnmannedPlugin)
                OnSaveValidModel(Isvalidpluginmodel);
            EventAggregator.GetEvent<CheckModelvalidEvent>().Publish(Isvalidpluginmodel);
            if (Constant.hasUnmannedPlugin)
                OnSaveValidModel(Isvalidpluginmodel);
            if (Isvalidpluginmodel.IsValid)
            {
                if (stVal == AppMode.Live)
                {
                    objCaptureArgs.SelectedWeighBridge = _selectedWeighBridge;
                    EventAggregator.GetEvent<BeforeWeightCaptureEvent>().Publish(objCaptureArgs);
                }
                if (stVal != AppMode.Live || objCaptureArgs.isAligned)
                {
                    object[] parameters = (object[])param;
                    string ManualReading = parameters[0].ToString();
                    string AutoReading = parameters[1].ToString();

                    ActiveTransaction.CaptureWeight = 0;

                    string CapturedWeight = "0";
                    if (!string.IsNullOrEmpty(ManualReading) && ActiveTransaction.IsCapturedManual)
                        CapturedWeight = ManualReading;
                    else if (!string.IsNullOrEmpty(AutoReading))
                        CapturedWeight = AutoReading;

                    if (Decimal.TryParse(CapturedWeight, out decimal dWeight) && dWeight > 0)
                    {
                        ActiveTransaction.CaptureWeight = dWeight;
                        ActiveTransaction.WeightUnit = SelectedWeighBridge.Unit.UnitName;
                        if (!EntryDateEditable)
                            ActiveTransaction.CaptureTime = DateTime.Now;
                    }

                    if (ActiveTransaction.CaptureWeight > 0)
                    {
                        if (Model.TransactionMode == TransactionMode.Single.ToString() &&
                            (Model.Vehicle == null || Model.Vehicle.TareWeight.GetValueOrDefault(0) <= 0))
                        {
                            SaveCommand.RaiseCanExecuteChanged();
                            await MessageBoxHelper.ShowInfoDialogAsync("Warning", "Vehicle tare weight is required, please add vehicle tare weight.");
                            UserControls.Flyouts.Vehicle ucVehicle = new UserControls.Flyouts.Vehicle(Model.Vehicle.VehicleID);
                            OpenFlyout(ucVehicle);
                            return;
                        }
                        await CalculateNetWeight();
                        Model.OnPropertyChanged("ValidTransactionDetails");
                        Model.OnPropertyChanged("SummaryTransaction");
                        Model.OnPropertyChanged("NetWeight");

                        Mouse.OverrideCursor = Cursors.Wait;
                        bool flag = await CaptureImages();
                        Mouse.OverrideCursor = null;
                        EventAggregator.GetEvent<AfterCaptureWeightEvent>().Publish();
                    }
                    else
                    {
                        ToastHelper.ShowError("Invalid Weight, Zero(0) or Negative weight cannot be captured");
                    }
                    SaveCommand.RaiseCanExecuteChanged();
                }
                else
                    await MessageBoxHelper.ShowInfoDialogAsync("Warning", "Vehicle is not properly aligned on weighbridge.");
            }
        }
        //Set values for:
        //GrossTime,GrossWeight,IsGrossManual
        //TareTime,TareWeight,IsTareManual
        //NetWeight,WeighmentType
        public async Task CalculateNetWeight()
        {
            if (EditMode) //Double/Multi mode and PreviousTransaction is present
            {
                //Special case to handle null values that may have entered in first transaction.
                //Correct previous transaction if required.
                if (Model.TransactionDetails.Count == 2)
                {
                    bool IsLastGross = PreviousTransaction.GrossWeight != null && PreviousTransaction.GrossWeight.Value > 0;
                    bool IsLastTare = PreviousTransaction.TareWeight != null && PreviousTransaction.TareWeight.Value > 0;

                    if (IsLastGross && PreviousTransaction.GrossWeight < ActiveTransaction.CaptureWeight)
                    {
                        PreviousTransaction.TareWeight = PreviousTransaction.GrossWeight;
                        PreviousTransaction.GrossWeight = null;
                        PreviousTransaction.IsTareManual = PreviousTransaction.IsGrossManual;
                        PreviousTransaction.IsGrossManual = false;
                        PreviousTransaction.TareTime = PreviousTransaction.GrossTime;
                        PreviousTransaction.GrossTime = null;
                        PreviousTransaction.WeighmentType = WeighmentType.Tare.ToString();
                        //Model.TransactionType = TransactionType.Outgoing.ToString();
                        await MessageBoxHelper.ShowInfoDialogAsync("Warning", "In last pass you entered gross weight, but that was tare weight.");
                    }
                    if (IsLastTare && PreviousTransaction.TareWeight > ActiveTransaction.CaptureWeight)
                    {
                        PreviousTransaction.GrossWeight = PreviousTransaction.TareWeight;
                        PreviousTransaction.TareWeight = null;
                        PreviousTransaction.IsGrossManual = PreviousTransaction.IsTareManual;
                        PreviousTransaction.IsTareManual = false;
                        PreviousTransaction.GrossTime = PreviousTransaction.TareTime;
                        PreviousTransaction.TareTime = null;
                        PreviousTransaction.WeighmentType = WeighmentType.Gross.ToString();
                        //Model.TransactionType = TransactionType.Incoming.ToString();
                        await MessageBoxHelper.ShowInfoDialogAsync("Warning", "In last pass you entered tare weight, but that was gross weight.");
                    }
                }

                ActiveTransaction.GrossWeight = Math.Max(PreviousTransaction.CaptureWeight.GetValueOrDefault(0), ActiveTransaction.CaptureWeight.GetValueOrDefault(0));
                ActiveTransaction.TareWeight = Math.Min(PreviousTransaction.CaptureWeight.GetValueOrDefault(0), ActiveTransaction.CaptureWeight.GetValueOrDefault(0));
                if (PreviousTransaction.CaptureWeight > ActiveTransaction.CaptureWeight)
                {
                    ActiveTransaction.GrossTime = PreviousTransaction.CaptureTime;
                    ActiveTransaction.IsGrossManual = PreviousTransaction.IsCapturedManual;
                    ActiveTransaction.TareTime = ActiveTransaction.CaptureTime;
                    ActiveTransaction.IsTareManual = ActiveTransaction.IsCapturedManual;
                    ActiveTransaction.WeighmentType = WeighmentType.Gross.ToString();
                    PreviousTransaction.WeighmentType = WeighmentType.Gross.ToString();
                }
                else
                {
                    ActiveTransaction.TareTime = PreviousTransaction.CaptureTime;
                    ActiveTransaction.IsTareManual = PreviousTransaction.IsCapturedManual;
                    ActiveTransaction.GrossTime = ActiveTransaction.CaptureTime;
                    ActiveTransaction.IsGrossManual = ActiveTransaction.IsCapturedManual;
                    ActiveTransaction.WeighmentType = WeighmentType.Tare.ToString();
                    PreviousTransaction.WeighmentType = WeighmentType.Tare.ToString();
                }
                ActiveTransaction.NetWeight = ActiveTransaction.GrossWeight.Value - ActiveTransaction.TareWeight.Value;
            }
            else //first transaction
            {
                ActiveTransaction.SequenceNo = 0;
                if (ActiveTransaction.WeighmentType == WeighmentType.Gross.ToString())
                {
                    ActiveTransaction.GrossTime = ActiveTransaction.CaptureTime;
                    ActiveTransaction.GrossWeight = ActiveTransaction.CaptureWeight;
                    ActiveTransaction.IsGrossManual = ActiveTransaction.IsCapturedManual;
                    ActiveTransaction.TareTime = null;
                    ActiveTransaction.TareWeight = null;
                    ActiveTransaction.IsTareManual = false;
                    ActiveTransaction.NetWeight = null;

                    //Additional Net Weight Calculation Logic for Single-Pass Transaction
                    if (Model.TransactionMode == TransactionMode.Single.ToString())
                    {
                        ActiveTransaction.SequenceNo = 1;
                        if (Model.Vehicle.TareWeight != null && Model.Vehicle.TareWeight.Value > 0)
                        {
                            ActiveTransaction.TareWeight = Model.Vehicle.TareWeight.Value;
                            ActiveTransaction.TareTime = Model.Vehicle.TareWeightTime.HasValue ? Model.Vehicle.TareWeightTime : DateTime.Now;
                        }

                        if (ActiveTransaction.TareWeight != null && ActiveTransaction.TareWeight > 0 && ActiveTransaction.GrossWeight != null && ActiveTransaction.GrossWeight > 0)
                            ActiveTransaction.NetWeight = ActiveTransaction.GrossWeight.Value - ActiveTransaction.TareWeight.Value;
                        else
                            ActiveTransaction.NetWeight = 0;

                        if (ActiveTransaction.NetWeight < 0)
                        {
                            ActiveTransaction.NetWeight = 0;
                            ActiveTransaction.CaptureWeight = 0;
                            ActiveTransaction.GrossWeight = null;
                            await MessageBoxHelper.ShowInfoDialogAsync("Warning", "Net weight can not be negative.");
                        }
                    }
                }
                else
                {
                    ActiveTransaction.TareTime = ActiveTransaction.CaptureTime;
                    ActiveTransaction.TareWeight = ActiveTransaction.CaptureWeight;
                    ActiveTransaction.IsTareManual = ActiveTransaction.IsCapturedManual;
                    ActiveTransaction.GrossTime = null;
                    ActiveTransaction.GrossWeight = null;
                    ActiveTransaction.IsGrossManual = false;
                }
            }
        }
        void CustomValue_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = dataRepository.HasChanges();
            }
            if (e.PropertyName == "HasErrors")
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        void ActiveTransaction_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = dataRepository.HasChanges();
            }
            if (e.PropertyName == "HasErrors")
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
            else if (e.PropertyName == "IsCapturedManual")
            {
                ActiveTransaction.CaptureWeight = 0;
                ActiveTransaction.GrossTime = null;
                ActiveTransaction.GrossWeight = null;
                if (Model.TransactionMode != TransactionMode.Single.ToString())
                {
                    ActiveTransaction.TareTime = null;
                    ActiveTransaction.TareWeight = null;
                }
                Model.OnPropertyChanged("ValidTransactionDetails");
                Model.OnPropertyChanged("SummaryTransaction");
                Model.OnPropertyChanged("NetWeight");
                SaveCommand.RaiseCanExecuteChanged();
            }
            else if (e.PropertyName == "CaptureWeight")
            {
                btnContinue.Visibility = Visibility.Collapsed;
                //btnContinueAndPrint.Visibility = Visibility.Collapsed;

                btnEndTransaction.Visibility = Visibility.Collapsed;
                //btnEndTransactionAndPrint.Visibility = Visibility.Collapsed;

                int TotalTransactionsSaved = Model.TransactionDetails.Where(x => x.CaptureWeight != null && x.CaptureWeight.Value > 0).ToList().Count;
                if (ActiveTransaction != null)
                    TotalTransactionsSaved--;

                if (Model.TransactionMode == TransactionMode.Single.ToString())
                {
                    btnEndTransaction.Visibility = Visibility.Visible;
                    //if (IsPrintSetup)
                    //    btnEndTransactionAndPrint.Visibility = Visibility.Visible;

                    btnContinue.Visibility = Visibility.Collapsed;
                    //btnContinueAndPrint.Visibility = Visibility.Collapsed;
                    if (ActiveTransaction.CaptureWeight.GetValueOrDefault(0) <= 0 && Constant.hasUnmannedPlugin)
                    {
                        btnEndTransaction.Visibility = Visibility.Collapsed;
                        //btnEndTransactionAndPrint.Visibility = Visibility.Collapsed;

                        btnContinue.Visibility = Visibility.Visible;
                        //if (IsPrintSetup)
                        //    btnContinueAndPrint.Visibility = Visibility.Visible;
                    }
                    else if (ActiveTransaction.CaptureWeight.GetValueOrDefault(0) > 0 && Constant.hasUnmannedPlugin)
                    {
                        btnEndTransaction.Visibility = Visibility.Visible;
                        //if (IsPrintSetup)
                        //    btnEndTransactionAndPrint.Visibility = Visibility.Visible;

                        btnContinue.Visibility = Visibility.Collapsed;
                        //btnContinueAndPrint.Visibility = Visibility.Collapsed;
                    }
                    ActiveTransaction.WeighmentType = WeighmentType.Gross.ToString();
                }
                else if (Model.TransactionMode == TransactionMode.Double.ToString())
                {
                    if (TotalTransactionsSaved >= 1)
                    {
                        btnEndTransaction.Visibility = Visibility.Visible;
                        //if (IsPrintSetup)
                        //    btnEndTransactionAndPrint.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        btnContinue.Visibility = Visibility.Visible;
                        //if (IsPrintSetup)
                        //    btnContinueAndPrint.Visibility = Visibility.Visible;
                    }
                }
                else if (Model.TransactionMode == TransactionMode.Multi.ToString())
                {
                    btnContinue.Visibility = Visibility.Visible;
                    //if (IsPrintSetup)
                    //    btnContinueAndPrint.Visibility = Visibility.Visible;

                    if (TotalTransactionsSaved >= 1)
                    {
                        btnEndTransaction.Visibility = Visibility.Visible;
                        //if (IsPrintSetup)
                        //    btnEndTransactionAndPrint.Visibility = Visibility.Visible;
                    }
                }
            }
        }
        protected async void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = dataRepository.HasChanges();
            }
            if (e.PropertyName == "HasErrors")
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
            else if (e.PropertyName == "TransactionMode")
            {
                btnContinue.Visibility = Visibility.Collapsed;
                //btnContinueAndPrint.Visibility = Visibility.Collapsed;

                btnEndTransaction.Visibility = Visibility.Collapsed;
                //btnEndTransactionAndPrint.Visibility = Visibility.Collapsed;

                gridAutomanualweight.Visibility = Visibility.Visible;

                int TotalTransactionsSaved = Model.TransactionDetails.Where(x => x.CaptureWeight != null && x.CaptureWeight.Value > 0).ToList().Count;
                //if (ActiveTransaction != null && ActiveTransaction.CaptureWeight != null && ActiveTransaction.CaptureWeight > 0)
                //    TotalTransactionsSaved--;

                if (Model.TransactionMode == TransactionMode.Single.ToString())
                {
                    ActiveTransaction.WeighmentType = WeighmentType.Gross.ToString();
                    if (ActiveTransaction.NetWeight == null)
                    {
                        if (ActiveTransaction.CaptureWeight != null && ActiveTransaction.CaptureWeight.GetValueOrDefault(0) > 0)
                        {
                            if (Model.Vehicle == null || Model.Vehicle.TareWeight.GetValueOrDefault(0) <= 0)
                            {
                                await MessageBoxHelper.ShowInfoDialogAsync("Warning", "Vehicle tare weight is required, please add vehicle tare weight.");
                                UserControls.Flyouts.Vehicle ucVehicle = new UserControls.Flyouts.Vehicle(Model.Vehicle.VehicleID);
                                OpenFlyout(ucVehicle);
                                return;
                            }
                            ActiveTransaction.GrossWeight = ActiveTransaction.CaptureWeight;
                            ActiveTransaction.GrossTime = ActiveTransaction.GrossTime;
                            ActiveTransaction.TareWeight = Model.Vehicle.TareWeight.Value;
                            ActiveTransaction.TareTime = Model.Vehicle.TareWeightTime.HasValue ? Model.Vehicle.TareWeightTime : DateTime.Now;
                            await CalculateNetWeight();
                            Model.OnPropertyChanged("NetWeight");
                            if (Constant.hasUnmannedPlugin)
                            {
                                btnContinue.Visibility = Visibility.Visible;
                                //if (IsPrintSetup)
                                //    btnContinueAndPrint.Visibility = Visibility.Visible;
                            }
                            btnEndTransaction.Visibility = Visibility.Visible;
                            //if (IsPrintSetup)
                            //    btnEndTransactionAndPrint.Visibility = Visibility.Visible;
                            SaveCommand.RaiseCanExecuteChanged();
                        }
                        else if (ActiveTransaction.CaptureWeight.GetValueOrDefault(0) <= 0 && Constant.hasUnmannedPlugin)
                        {
                            btnEndTransaction.Visibility = Visibility.Collapsed;
                            //btnEndTransactionAndPrint.Visibility = Visibility.Collapsed;

                            btnContinue.Visibility = Visibility.Visible;
                            //if (IsPrintSetup)
                            //    btnContinueAndPrint.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        btnEndTransaction.Visibility = Visibility.Visible;
                        //if (IsPrintSetup)
                        //    btnEndTransactionAndPrint.Visibility = Visibility.Visible;

                        gridAutomanualweight.Visibility = Visibility.Collapsed;
                        if (Constant.hasUnmannedPlugin)
                        {
                            btnContinue.Visibility = Visibility.Visible;
                            //if (IsPrintSetup)
                            //    btnContinueAndPrint.Visibility = Visibility.Visible;
                        }
                    }
                    //else if (ActiveTransaction.CaptureWeight.GetValueOrDefault(0) > 0 && Constant.hasUnmannedPlugin)
                    //{
                    //    btnEndTransaction.Visibility = Visibility.Visible;
                    //    btnContinue.Visibility = Visibility.Collapsed;
                    //}
                }
                else if (Model.TransactionMode == TransactionMode.Double.ToString())
                {
                    if (ActiveTransaction.NetWeight != null)
                        gridAutomanualweight.Visibility = Visibility.Collapsed;
                    if (TotalTransactionsSaved < 2)
                        gridAutomanualweight.Visibility = Visibility.Visible;
                    if (TotalTransactionsSaved == 2)
                    {
                        btnEndTransaction.Visibility = Visibility.Visible;
                        //if (IsPrintSetup)
                        //    btnEndTransactionAndPrint.Visibility = Visibility.Visible;
                    }

                    btnContinue.Visibility = Visibility.Visible;
                    //if (IsPrintSetup)
                    //    btnContinueAndPrint.Visibility = Visibility.Visible;
                }
                else if (Model.TransactionMode == TransactionMode.Multi.ToString())
                {
                    // Comment by Parimal
                    //if (ActiveTransaction.NetWeight != null && ActiveTransaction.NetWeight != 0)
                    //    gridAutomanualweight.Visibility = Visibility.Collapsed;

                    btnContinue.Visibility = Visibility.Visible;
                    //if (IsPrintSetup)
                    //    btnContinueAndPrint.Visibility = Visibility.Visible;

                    if (TotalTransactionsSaved >= 1)
                    {
                        btnEndTransaction.Visibility = Visibility.Visible;
                        //if (IsPrintSetup)
                        //    btnEndTransactionAndPrint.Visibility = Visibility.Visible;
                    }
                }
                SaveCommand.RaiseCanExecuteChanged();
            }
            else if (e.PropertyName == "TransactionType")
            {
                if (Model.TransactionType == TransactionType.Incoming.ToString())
                {
                    ActiveTransaction.WeighmentType = WeighmentType.Gross.ToString();
                }
                else if (Model.TransactionType == TransactionType.Outgoing.ToString())
                {
                    ActiveTransaction.WeighmentType = WeighmentType.Tare.ToString();
                }
                else
                {
                    ActiveTransaction.WeighmentType = WeighmentType.Gross.ToString();
                }
            }

            else if (e.PropertyName == "VehicleID") //e.PropertyName == "Vehicle" || 
            {
                if (Model.VehicleID > 0) //&& Model.Vehicle != null
                {
                    Model.Vehicle = dataRepository.Context.Vehicles.FirstOrDefault(x => x.VehicleID == Model.VehicleID);
                    if (Model.TransactionMode == TransactionMode.Single.ToString())
                    {
                        //if (Model.Vehicle.TareWeight != null && Model.Vehicle.TareWeight.Value > 0)
                        //{
                        //    ActiveTransaction.TareWeight = Model.Vehicle.TareWeight.Value;
                        //    ActiveTransaction.TareTime = DateTime.Now;
                        //}
                        //else
                        //{
                        //    ActiveTransaction.TareWeight = null;
                        //    ActiveTransaction.TareTime = null;
                        //    ActiveTransaction.NetWeight = 0.00m;
                        //}
                        if (ActiveTransaction.NetWeight.GetValueOrDefault(0) > 0)
                        {
                            if (Model.Vehicle.TareWeight != null && Model.Vehicle.TareWeight.Value > 0)
                            {
                                ActiveTransaction.TareWeight = Model.Vehicle.TareWeight.Value;
                                ActiveTransaction.TareTime = DateTime.Now;
                            }
                            else
                            {
                                ActiveTransaction.TareWeight = null;
                                ActiveTransaction.TareTime = null;
                                ActiveTransaction.NetWeight = 0;
                            }

                            if (ActiveTransaction.TareWeight != null && ActiveTransaction.TareWeight > 0 && ActiveTransaction.GrossWeight != null && ActiveTransaction.GrossWeight > 0)
                                ActiveTransaction.NetWeight = ActiveTransaction.GrossWeight.Value - ActiveTransaction.TareWeight.Value;
                            else
                                ActiveTransaction.NetWeight = 0;

                            if (ActiveTransaction.NetWeight < 0)
                            {
                                ActiveTransaction.TareWeight = null;
                                ActiveTransaction.TareTime = null;
                                ActiveTransaction.NetWeight = 0;
                            }
                            Model.OnPropertyChanged("ValidTransactionDetails");
                            Model.OnPropertyChanged("SummaryTransaction");
                            Model.OnPropertyChanged("NetWeight");
                        }
                    }

                    if (Model.Vehicle.AccountID != null)
                    {
                        if (Accounts != null && Accounts.Any(x => x.AccountID == Model.Vehicle.AccountID.Value))
                        {
                            Model.Account = Model.Vehicle.Account;
                            Model.AccountID = Model.Vehicle.AccountID.Value;
                            AccountNameText = Model.Vehicle.Account.CompanyName;
                        }
                        if (Transporters != null && Transporters.Any(x => x.AccountID == Model.Vehicle.AccountID.Value))
                        {
                            Model.Transporter = Model.Vehicle.Account;
                            Model.TransporterID = Model.Vehicle.AccountID.Value;
                            TransaporterNameText = Model.Vehicle.Account.CompanyName;
                        }
                    }
                }
                else if (e.PropertyName == "TokenId")
                {
                    Model.IsValid();
                }
                else
                {
                    Model.Vehicle = null;
                }
            }

            CaptureWeightCommand.RaiseCanExecuteChanged();
        }
        public void InitializeModel(TransactionData model)
        {
            Model = model;
            Model.PropertyChanged -= Model_PropertyChanged;
            Model.PropertyChanged += Model_PropertyChanged;
            SaveCommand.RaiseCanExecuteChanged();
            CaptureWeightCommand.RaiseCanExecuteChanged();
        }
        public void InitializeActiveTransaction()
        {
            if (Model.TransactionDetails != null && Model.TransactionDetails.Count > 0 && Model.TransactionMode != TransactionMode.Multi.ToString() &&
                Model.TransactionDetails.OrderByDescending(x => x.SequenceNo).FirstOrDefault().NetWeight != null &&
                Model.TransactionDetails.OrderByDescending(x => x.SequenceNo).FirstOrDefault().NetWeight >= 0 && Constant.hasUnmannedPlugin)
            {
                ActiveTransaction = Model.TransactionDetails.OrderByDescending(x => x.SequenceNo).FirstOrDefault();
            }
            else if (Model.TransactionDetails.Any(x => x.CaptureWeight == null || x.CaptureWeight.Value == 0) && Constant.hasUnmannedPlugin)
            {
                ActiveTransaction = Model.TransactionDetails.FirstOrDefault(x => x.CaptureWeight == null || x.CaptureWeight.Value == 0);
            }
            else if (isUnmannedEdit && Constant.hasUnmannedPlugin && Model.TransactionDetails.Count > 0)
            {
                ActiveTransaction = Model.TransactionDetails.OrderByDescending(p => p.SequenceNo).FirstOrDefault();
            }
            else
            {
                var newTransaction = new TransactionDetail();
                newTransaction.WeighmentType = WeighmentType.Gross.ToString();
                newTransaction.UserID = Convert.ToInt32(Application.Current.Properties["LoggedInUserID"].ToString());
                newTransaction.WB_User_Name = Application.Current.Properties["LoggedInUserName"].ToString();
                newTransaction.WeightBridgeID = SelectedWeighBridge.WeightBridgeID;
                newTransaction.WeightBridgeName = SelectedWeighBridge.ScaleName;
                newTransaction.WeightBridge = SelectedWeighBridge;
                if (SelectedWeighBridge.Unit != null)
                    newTransaction.WeightUnit = SelectedWeighBridge.Unit.UnitName;
                else
                    newTransaction.WeightUnit = "";
                newTransaction.SequenceNo = 1;
                if (Model.TransactionDetails.Count > 0 && PreviousTransaction != null)
                    newTransaction.SequenceNo = (byte)(PreviousTransaction.SequenceNo + 1);
                newTransaction.CaptureTime = DateTime.Now;
                newTransaction.IsTareManual = false;
                newTransaction.IsGrossManual = false;
                newTransaction.IsCapturedManual = false;
                if (PreviousTransaction != null)
                {
                    newTransaction.ProductID = PreviousTransaction.ProductID;
                    newTransaction.Product_Name = PreviousTransaction.Product_Name;
                    newTransaction.Product = PreviousTransaction.Product;
                    newTransaction.GateID = PreviousTransaction.GateID;
                    newTransaction.Gate_Name = PreviousTransaction.Gate_Name;
                    newTransaction.Gate = PreviousTransaction.Gate;
                }
                Model.TransactionDetails.Add(newTransaction);
                ActiveTransaction = newTransaction;
            }
            ActiveTransaction.PropertyChanged -= ActiveTransaction_PropertyChanged;
            ActiveTransaction.PropertyChanged += ActiveTransaction_PropertyChanged;

            if (Model.Vehicle != null)
            {
                VehicleNumberText = Model.Vehicle.VehicleNumber;
                LoadLastTransactionList((int)Model.VehicleID);
            }

            if (Model.Transporter != null)
                TransaporterNameText = Model.Transporter.CompanyName;

            if (Model.Account != null)
                AccountNameText = Model.Account.CompanyName;

            if (Model.Driver != null)
                DriverNameText = Model.Driver.DriverName;

            PopulateExistingModel(Model);

            if (ActiveTransaction != null)
            {
                if (ActiveTransaction.Product != null)
                    ProductNameText = ActiveTransaction.Product.ProductName;

                if (ActiveTransaction.Gate != null)
                    GateNameText = ActiveTransaction.Gate.GateName;

                if (Model.TransactionDetails != null)
                {
                    List<int> TransactionDetailIDs = Model.TransactionDetails.Select(x => x.TransactionDetailID).ToList();
                    List<CameraImage> lstTxImages = dataRepository.Context.CameraImages.Where(x => x.TransactionDetailID != null && TransactionDetailIDs.Contains(x.TransactionDetailID.Value)).ToList();
                    if (lstTxImages != null && lstTxImages.Count > 0)
                    {

                        foreach (CameraImage cameraImage in lstTxImages)
                        {
                            cameraImage.TicketID = Model.TicketID;
                        }

                        lstCamImage = new ObservableCollection<CameraImage>(lstTxImages);
                    }
                }
            }

        }
        public TransactionData CreateNewModel()
        {
            var model = new TransactionData();
            model.ReceiptTicketID = Guid.NewGuid();
            model.Status = TransactionStatus.Active.ToString();
            model.TransactionMode = TransactionMode.Double.ToString();
            model.TransactionType = TransactionType.Incoming.ToString();
            model.CreationTime = DateTime.Now;
            model.CreatedBy = Convert.ToInt32(Application.Current.Properties["LoggedInUserID"].ToString());
            model.CreatedName = Application.Current.Properties["LoggedInUserName"].ToString();

            dataRepository.Context.TransactionDatas.Add(model);
            //LastTicketID = dataRepository.Context.Database.SqlQuery<Decimal>("IF EXISTS (SELECT TOP 1 * FROM TransactionData) OR IDENT_CURRENT('transactiondata') <> 1 BEGIN SELECT IDENT_CURRENT('transactiondata') END ELSE BEGIN SELECT IDENT_CURRENT('transactiondata') - 1 END").FirstOrDefault();
            return model;
        }

        public void UnloadCamera()
        {
            try
            {
                ItemsControl icCameraList = (ItemsControl)parentPage.FindName("icCameraList");

                if (icCameraList != null)
                {
                    for (int i = 0; i < icCameraList.Items.Count; i++)
                    {
                        DependencyObject container = icCameraList.ItemContainerGenerator.ContainerFromIndex(i);
                        if (container == null) continue;

                        CamViewer ipcam = FindChild<CamViewer>(container, "ipcam");
                        if (ipcam == null || ipcam.vcVideoController == null) continue;

                        ipcam.vcVideoController.StopVideo();
                        Thread.Sleep(30);
                        ipcam.vcVideoController = null;
                    }
                }
            }
            catch { }

            Thread.Sleep(200);
        }

        public void Dispose()
        {
            UnloadCamera();
            try
            {
                EventAggregator.GetEvent<AfterDetailSavedEvent>().Unsubscribe(DetailsSaved);
                EventAggregator.GetEvent<AfterLoginEvent>().Subscribe(LoginSaves);
                EventAggregator.GetEvent<ApplicationExit>().Unsubscribe(ReleaseReader);
                EventAggregator.GetEvent<Change_tokenEvent>().Unsubscribe(Get_TokenName);
                ActiveWeighBridges.ToList().ForEach(x => x.ReleaseReaders());
                dataRepository.Context.Dispose();
                //VehicledataRepository.Context.Dispose();
                foreach (var plugin in mef.loadedPlugins.Keys)
                {
                    try
                    {
                        plugin.DisposePlugin();
                    }
                    catch { }
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                Common.logErrorInDB(ex.Message, ex);
            }
        }

        public void Get_TokenName(Change_tokenEventArgs obj)
        {
            try
            {

                if (Model != null && obj != null)
                {

                    Model.TokenName = obj.TokenName;
                    Model.TokenValue = obj.TokenValue;
                }
            }
            catch (Exception ex)
            {
                string Error = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.WriteAllText(Constant.PluginFileLocation + @"\Get_TokenName_DataTransactionError.txt", Error);
            }
        }

        //public void PopulateExistingModel(TransactionData model)
        //{
        //    if (model == null || CustomFieldss == null) return;

        //    foreach (var fieldVM in CustomFieldss)
        //    {
        //        var prop = typeof(TransactionData).GetProperty(fieldVM.ColumnName);
        //        if (prop == null) continue;

        //        var raw = prop.GetValue(model);
        //        string value = raw?.ToString()?.Trim() ?? string.Empty;

        //        fieldVM.CustomValue = value;

        //        if (!string.IsNullOrWhiteSpace(value) && !fieldVM.FieldValues.Contains(value))
        //        {
        //            fieldVM.FieldValues.Add(value);
        //        }
        //    }
        //}

        public void PopulateExistingModel(TransactionData model)
        {
            if (model == null || CustomFieldss == null) return;

            foreach (var fieldVM in CustomFieldss)
            {
                var prop = typeof(TransactionData).GetProperty(fieldVM.ColumnName);
                if (prop == null) continue;

                var raw = prop.GetValue(model);
                string value = raw?.ToString()?.Trim() ?? string.Empty;

                // Only overwrite if the model actually has a saved value.
                // Otherwise keep whatever SetupCustomFields already put there (e.g. the default).
                if (!string.IsNullOrWhiteSpace(value))
                {
                    fieldVM.CustomValue = value;

                    if (!fieldVM.FieldValues.Contains(value))
                        fieldVM.FieldValues.Add(value);
                }
            }
        }

        #endregion

        #region camera 

        private void LoadCamera(WeightBridge actiavtedWeighBridge)
        {
            if (ActiveWeighBridgeID != actiavtedWeighBridge.WeightBridgeID)
            {
                ActiveWeighBridgeID = actiavtedWeighBridge.WeightBridgeID;

                try
                {
                    if (actiavtedWeighBridge != null)
                    {
                        ActiveCamera = dataRepository.Context.Cameras.Where(x => x.Active && x.WeightBridgeID == actiavtedWeighBridge.WeightBridgeID).FirstOrDefault();

                        // Get the list of all active cameras
                        CameraList = new ObservableCollection<Camera>(dataRepository.Context.Cameras
                            .Where(x => x.Active && x.WeightBridgeID == actiavtedWeighBridge.WeightBridgeID).ToList()
                        );

                        List<Camera> cameras = GetAnprCameras(SelectedWeighBridge.WeightBridgeID);
                        if (cameras.Where(p => p.WBSide == "LEFT").Count() > 0)
                        {
                            LeftAnpr = cameras.Where(p => p.WBSide == "LEFT").FirstOrDefault();
                        }
                        if (cameras.Where(p => p.WBSide == "RIGHT").Count() > 0)
                        {
                            RightAnpr = cameras.Where(p => p.WBSide == "RIGHT").FirstOrDefault();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _ = ex.Message;
                }

                if (ActiveCamera == null)
                {
                    //dockDefault.Visibility = Visibility.Visible;
                    //dockCamera.Visibility = Visibility.Collapsed;
                    //borderWeightDetail.Visibility = Visibility.Collapsed;
                }
                else
                {
                    //dockDefault.Visibility = Visibility.Collapsed;
                    //dockCamera.Visibility = Visibility.Visible;
                    //borderWeightDetail.Visibility = Visibility.Visible;
                }

                if (dataRepository.Context.Cameras.Count(x => x.Active && x.WeightBridgeID == actiavtedWeighBridge.WeightBridgeID) > 1)
                {
                    IsviewMoreCamera = true;

                    // lstActiveCamera = new ObservableCollection<Camera>(dataRepository.Context.Cameras.Where(x => x.Active && x.WeightBridgeID == actiavtedWeighBridge.WeightBridgeID).ToList().Skip(1).ToList());

                }
                else
                    IsviewMoreCamera = false;
            }
        }

        //public List<Control> AllCameraChildren(DependencyObject parent)
        //{
        //    var _List = new List<Control> { };
        //    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        //    {
        //        var _child = VisualTreeHelper.GetChild(parent, i);
        //        if (_child is Control)
        //            _List.Add(_child as Control);
        //        _List.AddRange(AllCameraChildren(_child));

        //    }
        //    return _List;
        //}

        private async Task<bool> CaptureImages()
        {
            bool Flag = true;
            try
            {
                int posibleTickitid = 0;
                if (EditMode == true)
                {
                    posibleTickitid = Model.TicketID;
                }
                else
                {
                    int? ticket = GetTicketNO();
                    if (ticket != null)
                        posibleTickitid = (int)ticket;
                }

                if (posibleTickitid == 0)
                    return false;


                if (dataRepository.Context.Cameras.Count(x => x.Active && x.WeightBridgeID == SelectedWeighBridge.WeightBridgeID) < 1)
                {
                    return false;
                }

                DeleteUnsavedImages();
                //CamViewer ipcam = (CamViewer)parentPage.FindName("ipcam");

                ItemsControl icCameraList = (ItemsControl)parentPage.FindName("icCameraList");
                string Perent_Directory = Directory.GetParent(CameraConstant.CameraImageLocation).FullName;
                string Temp_Directory = Perent_Directory + "\\Temp_" + Path.GetFileName(CameraConstant.CameraImageLocation.TrimEnd(Path.DirectorySeparatorChar));
                if (!Directory.Exists(Temp_Directory))
                {
                    Directory.CreateDirectory(Temp_Directory);
                }

                if (icCameraList != null)
                {
                    for (int i = 0; i < icCameraList.Items.Count; i++)
                    {
                        DependencyObject container = icCameraList.ItemContainerGenerator.ContainerFromIndex(i);
                        if (container == null) continue;

                        CamViewer ipcam = FindChild<CamViewer>(container, "ipcam");
                        if (ipcam == null || ipcam.vcVideoController == null) continue;

                        if (ipcam != null && ipcam.vcVideoController != null && ipcam.IsPlaying && ipcam.VideoSourceURL == ipcam.VideoSourceMainURL)
                        {
                            string fileName = DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + ipcam.CameraId + FileExtension;
                            await ipcam.vcVideoController.TakeSnapshot(Temp_Directory + "\\" + fileName);

                            if (!Directory.Exists(CameraConstant.CameraImageLocation + "\\" + posibleTickitid))
                                Directory.CreateDirectory(CameraConstant.CameraImageLocation + "\\" + posibleTickitid);

                            if (File.Exists(Temp_Directory + "\\" + fileName))
                            {
                                File.Copy(Temp_Directory + "\\" + fileName, CameraConstant.CameraImageLocation + "\\" + posibleTickitid + "\\" + fileName);

                                CameraImage objCamImage = new CameraImage();
                                objCamImage.CameraID = ipcam.CameraId;
                                objCamImage.ImageLocation = fileName;
                                objCamImage.TicketID = posibleTickitid;
                                lstCamImage.Add(objCamImage);
                            }
                        }
                        else
                        {
                            string fileName = DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + ipcam.CameraId + FileExtension;


                            string VideoURL = string.Empty;
                            if (ipcam.VideoFormat.ToString().ToUpper() == "RTSPMJPEG")
                            {
                                UriBuilder uriBuilder = new UriBuilder(ipcam.VideoSourceMainURL);
                                uriBuilder.UserName = ipcam.UserName;
                                uriBuilder.Password = ipcam.Password;
                                VideoURL = uriBuilder.Uri.ToString();
                            }
                            else
                            {
                                VideoURL = $"rtsp://{ipcam.UserName}:{ipcam.Password}@{ipcam.IpAddress}:554/Streaming/Channels/101/";
                            }

                            string Argument = "-rtsp_transport tcp -y -i " + VideoURL.ToString() + " -vframes 1 " + Temp_Directory + "\\" + fileName;

                            // Process Capture Image
                            Process process = new Process();
                            process.StartInfo.FileName = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory) + "\\ffmpeg.exe";
                            process.StartInfo.Arguments = Argument;
                            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.RedirectStandardError = true; // capture errors
                            process.Start();

                            if (!process.WaitForExit(6000)) // wait 6s
                            {
                                process.Kill();
                            }

                            string errorLog = process.StandardError.ReadToEnd();
                            if (!string.IsNullOrEmpty(errorLog))
                            {
                                try
                                {
                                    File.AppendAllText(Path.Combine(Constant.PluginFileLocation, "ffmpeg_errors.log"), errorLog + Environment.NewLine);
                                }
                                catch { }
                            }

                            if (File.Exists(Temp_Directory + "\\" + fileName))
                            {

                                if (!Directory.Exists(CameraConstant.CameraImageLocation + "\\" + posibleTickitid))
                                    Directory.CreateDirectory(CameraConstant.CameraImageLocation + "\\" + posibleTickitid);

                                Bitmap image = new Bitmap(Temp_Directory + "\\" + fileName);
                                Save(image, 480, 360, 50, CameraConstant.CameraImageLocation + "\\" + posibleTickitid + "\\" + fileName);

                                CameraImage objCamImage = new CameraImage();
                                objCamImage.CameraID = ipcam.CameraId;
                                objCamImage.ImageLocation = fileName;
                                objCamImage.TicketID = posibleTickitid;
                                lstCamImage.Add(objCamImage);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }

            return Flag;
        }

        private T FindChild<T>(DependencyObject parent, string childName = null) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T tChild && (string.IsNullOrEmpty(childName) || (child as FrameworkElement)?.Name == childName))
                {
                    return tChild;
                }

                var result = FindChild<T>(child, childName);
                if (result != null)
                    return result;
            }

            return null;
        }
        private void DeleteUnsavedImages()
        {
            if (lstCamImage != null && lstCamImage.Count > 0)
            {
                List<CameraImage> ImageObjectsToRemove = lstCamImage.Where(x => x.CameraImageID == 0).ToList();
                if (ImageObjectsToRemove != null && ImageObjectsToRemove.Count > 0)
                {
                    List<String> ImagesToDelete = lstCamImage.Where(x => x.CameraImageID == 0).Select(y => y.CameraImagePath).ToList();
                    foreach (var item in ImageObjectsToRemove)
                    {
                        lstCamImage.Remove(item);
                    }
                    OnPropertyChanged("lstCamImage");
                    foreach (var item in ImagesToDelete)
                    {
                        try
                        {
                            System.IO.File.Delete(item);
                        }
                        catch { }

                    }
                }
            }
        }
        private void CameraImageSaved(TransactionDetailSavedEventArgs obj)
        {
            if (lstCamImage != null)
            {
                foreach (var item in lstCamImage.Where(x => x.CameraImageID == 0))
                {
                    dataRepository.Context.CameraImages.Add(item);
                    item.TransactionDetailID = obj.tranDetail.TransactionDetailID;
                    item.IsSelected = false;
                }
            }
        }

        public void Save(Bitmap image, int maxWidth, int maxHeight, int quality, string filePath)
        {
            // Get the image's original width and height
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // To preserve the aspect ratio
            float ratioX = (float)maxWidth / (float)originalWidth;
            float ratioY = (float)maxHeight / (float)originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            // New width and height based on aspect ratio
            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            // Convert other formats (including CMYK) to RGB.
            Bitmap newImage = new Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            // Draws the image in the specified size with quality mode set to HighQuality
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            // Get an ImageCodecInfo object that represents the JPEG codec.
            ImageCodecInfo imageCodecInfo = this.GetEncoderInfo(ImageFormat.Jpeg);

            // Create an Encoder object for the Quality parameter.
            System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object. 
            EncoderParameters encoderParameters = new EncoderParameters(1);

            // Save the image as a JPEG file with quality level.
            EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);
            encoderParameters.Param[0] = encoderParameter;
            newImage.Save(filePath, imageCodecInfo, encoderParameters);
        }
        private ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
        }

        #endregion

        #region Endel ANPR

        //public Endel_ANPR_service endel_ANPR_Service = new Endel_ANPR_service("");
        public void GetAnprNumber(string Side)
        {
            int posibleTickitid = 0;
            if (EditMode == true)
            {
                posibleTickitid = Model.TicketID;
            }
            else
            {
                int? ticket = GetTicketNO();
                if (ticket != null)
                    posibleTickitid = (int)ticket;
            }

            if (posibleTickitid == 0)
                return;

            string Anpr_Directory = Path.Combine(CameraConstant.CameraImageLocation, "Temp_Anpr");

            if (!Directory.Exists(Anpr_Directory))
            {
                Directory.CreateDirectory(Anpr_Directory);
            }

            if (LeftAnpr != null && Side == "Left")
            {
                string fileName = $"{SelectedWeighBridge.WeightBridgeID}_{LeftAnpr.CameraID}{FileExtension}";
                string InputImagePath = Path.Combine(Anpr_Directory, fileName);
                if (File.Exists(InputImagePath)) File.Delete(InputImagePath);
                ImageCapture(LeftAnpr, InputImagePath);
                if (File.Exists(InputImagePath))
                {
                    string LeftPlatfileName = DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + LeftAnpr.CameraID + "_Left" + FileExtension;
                    string OutputImagePath = Path.Combine(CameraConstant.CameraImageLocation, posibleTickitid.ToString(), LeftPlatfileName);
                    string AnprNumber = ProcessAnprCamera(LeftAnpr, InputImagePath, OutputImagePath);
                    ActiveTransaction.LeftPlateImageName = LeftPlatfileName;
                    ActiveTransaction.LeftAnprNumber = AnprNumber;
                    AnprNumberText = AnprNumber;
                }
            }

            if (RightAnpr != null && Side == "Right")
            {
                string fileName = $"{SelectedWeighBridge.WeightBridgeID}_{RightAnpr.CameraID}{FileExtension}";
                string InputImagePath = Path.Combine(Anpr_Directory, fileName);
                if (File.Exists(InputImagePath)) File.Delete(InputImagePath);
                ImageCapture(RightAnpr, InputImagePath);
                if (File.Exists(InputImagePath))
                {
                    string RightPlatfileName = DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + RightAnpr.CameraID + "_Left" + FileExtension;
                    string OutputImagePath = Path.Combine(CameraConstant.CameraImageLocation, posibleTickitid.ToString(), RightPlatfileName);
                    string AnprNumber = ProcessAnprCamera(RightAnpr, InputImagePath, OutputImagePath);
                    ActiveTransaction.RightPlateImageName = RightPlatfileName;
                    ActiveTransaction.RightAnprNumber = AnprNumber;
                    AnprNumberText = AnprNumber;
                }
            }
        }
        public void ImageCapture(Camera camera, string InputImagePath)
        {
            try
            {
                string VideosourceMainURL = "";

                if (camera.CameraType.ToString().ToUpper() == "RTSPMJPEG")
                    VideosourceMainURL = camera.VideosourceMainURL;
                else
                    VideosourceMainURL = $"rtsp://{camera.IPAddress}:554/Streaming/Channels/101/";

                UriBuilder uriBuilder = new UriBuilder(camera.VideosourceMainURL)
                {
                    UserName = System.Net.WebUtility.UrlEncode(camera.UserName),
                    Password = System.Net.WebUtility.UrlEncode(camera.CameraPassword)
                };
                Uri CompleteURL = uriBuilder.Uri;
                string VideoURL = CompleteURL.ToString();

                //string Argument = "-rtsp_transport tcp -y -i " + VideoURL.ToString() + " -vframes 1 " + InputImagePath;
                //string Argument = $"-rtsp_transport tcp -fflags nobuffer -flags low_delay -analyzeduration 0 -probesize 32 -i {VideoURL} -frames:v 1 -q:v 1 -y {InputImagePath}";

                string Argument =
                                "-rtsp_transport tcp " +
                                "-stimeout 3000000 " +        // 3 seconds RTSP timeout
                                "-fflags nobuffer " +
                                "-flags low_delay " +
                                "-analyzeduration 0 " +
                                "-probesize 32 " +
                                $"-i \"{VideoURL}\" " +
                                "-frames:v 1 " +
                                "-q:v 1 " +
                                $"-y \"{InputImagePath}\"";

                string ffmpegFileName = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory) + "\\ffmpeg.exe";

                if (File.Exists(ffmpegFileName))
                {
                    // Process Capture Image
                    using (Process process = new Process())
                    {
                        process.StartInfo = new ProcessStartInfo
                        {
                            FileName = ffmpegFileName,
                            Arguments = Argument,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardError = true,
                            RedirectStandardOutput = true
                        };

                        process.Start();

                        if (!process.WaitForExit(5000)) // hard limit
                        {
                            try { process.Kill(); } catch { }
                        }

                        string errorLog = process.StandardError.ReadToEnd();

                        if (!File.Exists(InputImagePath))
                        {
                            File.WriteAllText(Path.Combine(Constant.PluginFileLocation, $"ffmpeg_errors_{camera.CameraID}.log"), errorLog);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Path.Combine(Constant.PluginFileLocation, $"ImageCapture_errors_{camera.CameraID}.log"), JsonConvert.SerializeObject(ex) + Environment.NewLine);
            }

        }
        public string ProcessAnprCamera(Camera camera, string InputImagePath, string OutputImagePath)
        {
            string ReturnNumber = string.Empty;
            try
            {
                int CameraId = camera.CameraID;
                string CameraName = camera.CameraName;
                string WBName = SelectedWeighBridge.ScaleName;
                string API_Region = SelectedWeighBridge.Region;

                string VehicleImage_Base64 = string.Empty;

                try
                {
                    byte[] fileBytes = File.ReadAllBytes(InputImagePath);
                    VehicleImage_Base64 = Convert.ToBase64String(fileBytes);
                }
                catch { }

                Anpr_Number_Request anpr_Number_Request = new Anpr_Number_Request
                {
                    CameraId = CameraId,
                    Rtsp_url = camera.VideosourceMainURL,
                    UserName = camera.UserName,
                    Password = camera.CameraPassword,
                    InputImagePath = InputImagePath
                };
                Anpr_Number_Response plateResults = MainWindow.GetVehicleNumberAsync(anpr_Number_Request);

                if (plateResults == null)
                {
                    try
                    {
                        Anpr_Number_API_Request anpr_Number_API_Request = new Anpr_Number_API_Request { API_Region = API_Region, VehicleImage_Base64 = VehicleImage_Base64, VehicleNumber = VehicleNumberText };
                        Anpr_Number_API_Response anpr_Number_API_Response = AnprLicenseHelper.GetAnprNumber(anpr_Number_API_Request);

                        if (anpr_Number_API_Response != null)
                        {

                            Base64ImageString = anpr_Number_API_Response.PlateImage_Base64;

                            plateResults = new Anpr_Number_Response
                            {
                                VehicleNumber = anpr_Number_API_Response.AnprNumber,
                                PlateImage_Base64 = anpr_Number_API_Response.PlateImage_Base64,
                                Success = anpr_Number_API_Response.Success,
                                Error = anpr_Number_API_Response.Error
                            };
                        }
                    }
                    catch
                    {
                        return ReturnNumber;
                    }
                }

                if (plateResults == null)
                    return ReturnNumber;

                if (plateResults != null)
                {
                    string VehicleNumber = plateResults.Success ? plateResults.VehicleNumber : "";

                    if (!string.IsNullOrEmpty(plateResults.PlateImage_Base64) && !string.IsNullOrEmpty(VehicleNumber))
                    {
                        string ImageName = CameraId + "_Left" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".jpg";
                        string sourceFile = Path.Combine(AnprImageLocation, ImageName);

                        MainWindow.SaveBase64Image(plateResults.PlateImage_Base64, sourceFile);

                        var Anpr_Match_result = MainWindow.GetMatchResult(VehicleNumber, VehicleNumberText);
                        int MatchPer = Anpr_Match_result.MatchPer;
                        string MatchResult = Anpr_Match_result.MatchResult;

                        MainWindow.Save_In_DB(CameraId, VehicleNumber, VehicleNumberText, ImageName, MatchPer, MatchResult, CameraName, WBName);

                    }
                    ReturnNumber = VehicleNumber;
                }
            }
            catch
            {

            }

            return ReturnNumber;
        }

        public List<Camera> GetAnprCameras(int WeightBridgeID)
        {
            try
            {
                using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                {
                    return ctx.Cameras.Where(p => p.WeightBridgeID == WeightBridgeID && p.IsAnpr == true && p.Active == true).ToList();
                }
            }
            catch
            {
                return null;
            }

        }

        #endregion Endel ANPR


    }
}