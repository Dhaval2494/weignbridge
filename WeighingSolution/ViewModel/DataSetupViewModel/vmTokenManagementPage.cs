using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using WeighingSolution.DataLayer;
using System.ComponentModel;
using WeighingSolution.Helpers;
using MahApps.Metro.Controls.Dialogs;
using WeighingSolution.Events;
using System.Data.Entity.Infrastructure;
using System.Windows;
using System.Collections.ObjectModel;
using WeighingSolution.SharedComponents;
using System.IO;
using Newtonsoft.Json;

namespace WeighingSolution.ViewModel
{
    public class vmTokenManagementPage : NotifyDataErrorInfoBase, IDisposable 
    {
        public DataRepository dataRepository;
        public readonly IEventAggregator EventAggregator = AppEventManager.Instance.EventAggregator;

        public vmTokenManagementPage(int TokenId)
        {
            Id = TokenId;
            dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));
            AllocateSaveCommand = new DelegateCommand(OnAllocateSaveExecute);
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            CloseDetailViewCommand = new DelegateCommand<string>(OnCloseDetailViewExecute);
        }

        #region Command and Properties Declaration
        public DelegateCommand AllocateSaveCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }

        public DelegateCommand<string> CloseDetailViewCommand { get; set; }
        private int _id;
        public int Id
        {
            get { return _id; }
            protected set { _id = value; }
        }
        private Token _model;
        public Token Model
        {
            get { return _model; }
            set
            {
                _model = value;
                OnPropertyChanged();
            }
        }
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
        private ObservableCollection<AccountBindingModel> _accounts;
        public ObservableCollection<AccountBindingModel> Accounts
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

        private ObservableCollection<AccountBindingModel> _transporters;
        public ObservableCollection<AccountBindingModel> Transporters
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

        #endregion

        #region CustomFeilds and values

        private ObservableCollection<string> _CustomFields1; public ObservableCollection<string> CustomFields1 { get { return _CustomFields1; } set { _CustomFields1 = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _CustomFields2; public ObservableCollection<string> CustomFields2 { get { return _CustomFields2; } set { _CustomFields2 = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _CustomFields3; public ObservableCollection<string> CustomFields3 { get { return _CustomFields3; } set { _CustomFields3 = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _CustomFields4; public ObservableCollection<string> CustomFields4 { get { return _CustomFields4; } set { _CustomFields4 = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _CustomFields5; public ObservableCollection<string> CustomFields5 { get { return _CustomFields5; } set { _CustomFields5 = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _CustomFields6; public ObservableCollection<string> CustomFields6 { get { return _CustomFields6; } set { _CustomFields6 = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _CustomFields7; public ObservableCollection<string> CustomFields7 { get { return _CustomFields7; } set { _CustomFields7 = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _CustomFields8; public ObservableCollection<string> CustomFields8 { get { return _CustomFields8; } set { _CustomFields8 = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _CustomFields9; public ObservableCollection<string> CustomFields9 { get { return _CustomFields9; } set { _CustomFields9 = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _CustomFields10; public ObservableCollection<string> CustomFields10 { get { return _CustomFields10; } set { _CustomFields10 = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _CustomFields11; public ObservableCollection<string> CustomFields11 { get { return _CustomFields11; } set { _CustomFields11 = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _CustomFields12; public ObservableCollection<string> CustomFields12 { get { return _CustomFields12; } set { _CustomFields12 = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _CustomFields13; public ObservableCollection<string> CustomFields13 { get { return _CustomFields13; } set { _CustomFields13 = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _CustomFields14; public ObservableCollection<string> CustomFields14 { get { return _CustomFields14; } set { _CustomFields14 = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _CustomFields15; public ObservableCollection<string> CustomFields15 { get { return _CustomFields15; } set { _CustomFields15 = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _CustomFields16; public ObservableCollection<string> CustomFields16 { get { return _CustomFields16; } set { _CustomFields16 = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _CustomFields17; public ObservableCollection<string> CustomFields17 { get { return _CustomFields17; } set { _CustomFields17 = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _CustomFields18; public ObservableCollection<string> CustomFields18 { get { return _CustomFields18; } set { _CustomFields18 = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _CustomFields19; public ObservableCollection<string> CustomFields19 { get { return _CustomFields19; } set { _CustomFields19 = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _CustomFields20; public ObservableCollection<string> CustomFields20 { get { return _CustomFields20; } set { _CustomFields20 = value; OnPropertyChanged(); } }


        #endregion CustomFeilds and values

        public async Task SetupMasterListsAsync()
        {
            try
            {
                Vehicles = new ObservableCollection<VehicleBindingModel>(await dataRepository.Context.Vehicles.Where(x => x.IsActive).Select(x => new VehicleBindingModel { VehicleID = x.VehicleID, VehicleNumber = x.VehicleNumber, TareWeight = x.TareWeight }).ToListAsync());

                Accounts = new ObservableCollection<AccountBindingModel>(await dataRepository.Context.Accounts.Where(x => x.IsAccount != null && x.IsAccount.Value && x.Active == true).Select(x => new AccountBindingModel { AccountID = x.AccountID, CompanyName = x.CompanyName }).ToListAsync());

                Transporters = new ObservableCollection<AccountBindingModel>(await dataRepository.Context.Accounts.Where(x => x.IsTransporter != null && x.IsTransporter.Value && x.Active == true).Select(x => new AccountBindingModel { AccountID = x.AccountID, CompanyName = x.CompanyName }).ToListAsync());

                Drivers = new ObservableCollection<DriverBindingModel>(await dataRepository.Context.Drivers.Where(x => x.Active).Select(x => new DriverBindingModel { DriverID = x.DriverID, FirstName = x.FirstName, LastName = x.LastName }).ToListAsync());

                Products = new ObservableCollection<ProductBindingModel>(await dataRepository.Context.Products.Where(x => x.IsActive).Select(x => new ProductBindingModel { ProductID = x.ProductID, ProductName = x.ProductName }).ToListAsync());

                Gates = new ObservableCollection<Gate>(await dataRepository.Context.Gates.Where(x => x.IsActive).ToListAsync());

            }
            catch (Exception ex)
            {
            }
        }

        public async Task LoadAsync()
        {
            var token = Id > 0
              ? await dataRepository.Context.Tokens.SingleAsync(x => x.TokenID == Id)
              : CreateNewToken();

            await SetupMasterListsAsync();

            Model = token;
            Model.PropertyChanged -= Model_PropertyChanged;
            Model.PropertyChanged += Model_PropertyChanged;
            if (Model.TokenID > 0 && Model.TokenInterval != null)
            {
                if (Model.TokenInterval == "Daily" && DateTime.Now > Model.TokenExpireDate)
                    Model.TokenExpireDate = DateTime.Now.AddDays(1);
                if (Model.TokenInterval == "Weekly" && DateTime.Now > Model.TokenExpireDate)
                    Model.TokenExpireDate = DateTime.Now.AddDays(7);
                if (Model.TokenInterval == "Monthly" && DateTime.Now > Model.TokenExpireDate)
                    Model.TokenExpireDate = DateTime.Now.AddMonths(1);
            }

            if (Model.Vehicle != null)
                VehicleNumberText = Model.Vehicle.VehicleNumber;

            if (Model.Account1 != null)
                TransaporterNameText = Model.Account1.CompanyName;

            if (Model.Account != null)
                AccountNameText = Model.Account.CompanyName;

            if (Model.Driver != null)
                DriverNameText = Model.Driver.DriverName;


            if (Model.Product != null)
                ProductNameText = Model.Product.ProductName;

            if (Model.Gate != null)
                GateNameText = Model.Gate.GateName;


            SaveCommand.RaiseCanExecuteChanged();
        }
        private Token CreateNewToken()
        {
            var token = new Token();
            token.TokenStatus = "Free";
            token.PreferredWeighmentType = WeighmentType.Gross.ToString();
            token.TokenType = "Temporary";
            dataRepository.Context.Tokens.Add(token);
            return token;
        }
        protected void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TokenType" && Model.TokenType == "Permanent")
            {
                Model.PreferredTransactionMode = TransactionMode.Double.ToString();
                Model.PreferredTransactionType = TransactionType.Outgoing.ToString();
                Model.TokenInterval = "Custom";
            }
            if (e.PropertyName == "TokenInterval")
            {
                if (Model.TokenInterval == "Daily")
                    Model.TokenExpireDate = DateTime.Now.AddDays(1);
                if (Model.TokenInterval == "Weekly")
                    Model.TokenExpireDate = DateTime.Now.AddDays(7);
                if (Model.TokenInterval == "Monthly")
                    Model.TokenExpireDate = DateTime.Now.AddMonths(1);
                else
                    Model.TokenExpireDate = DateTime.Now.AddYears(1);
            }

            if (!HasChanges)
                HasChanges = dataRepository.HasChanges();
            if (e.PropertyName == "HasErrors")
                SaveCommand.RaiseCanExecuteChanged();
        }
        private async void OnCloseDetailViewExecute(string obj)
        {
            if (HasChanges)
            {
                var result = await MessageBoxHelper.ShowOkCancelDialogAsync("Are you sure?", "You've made changes. Close this item?");
                if (result == MessageDialogResult.Negative)
                {
                    return;
                }
            }
            EventAggregator.GetEvent<AfterDetailSaveCancelledEvent>().Publish(new AfterDetailSaveCancelledEventArgs { ObjectInstance = Model });
        }

        //private bool OnAllocateSaveCanExecute()
        //{
        //    return Model != null && !Model.HasErrors && HasChanges && Model.TokenType == "Permanent";
        //}

        private void OnAllocateSaveExecute()
        {
            if (Model.IsValid())
            {
                //if (Model.TokenExpireDate < DateTime.Now)
                //{
                //    await MessageBoxHelper.ShowInfoDialogAsync("Warning", "Token is expired");
                //    return;
                //}
                //if (Model.PreferredTransactionMode == null)
                //{
                //    await MessageBoxHelper.ShowInfoDialogAsync("Warning", "Transaction mode is required");
                //    return;
                //}
                //if (Model.PreferredTransactionMode != null && Model.PreferredTransactionMode == "Single" && Model.VehicleID != null && dataRepository.Context.Vehicles.FirstOrDefault(x => x.VehicleID == Model.VehicleID).TareWeight == null)
                //{
                //    await MessageBoxHelper.ShowInfoDialogAsync("Warning", "Selected vehicle must have tareweight to perform single pass transaction.");
                //    return;
                //}
                //if (Model.PreferredTransactionMode != null && Model.PreferredTransactionMode == "Single" && Model.PreferredWeighmentType != "Gross")
                //{
                //    await MessageBoxHelper.ShowInfoDialogAsync("Warning", "Selected weighment type sholuld be gross to perform single pass transaction.");
                //    return;
                //}
                Model.TokenStatus = "Allocated";
                SaveChange();
            }
        }
        private bool OnSaveCanExecute()
        {
            if (Model != null && !string.IsNullOrEmpty(Model.Vehicle_Number))
            {
                var FindToken = dataRepository.Context.Tokens.Any(x => x.Vehicle_Number.ToUpper() == Model.Vehicle_Number.ToUpper() && x.TokenID != Model.TokenID);
                if (FindToken)
                {
                    ToastHelper.ShowError("Vehicle Number already exist , can not add duplicate Vehicle Number.");
                    return false;
                }
            }

            return Model != null && !Model.HasErrors && HasChanges;
        }

        private void OnSaveExecute()
        {
            try
            {
                if (Model.TokenType == "Temporary")
                {
                    Model.AccountID = null;
                    Model.VehicleID = null;
                    Model.PreferredTransactionMode = null;
                    Model.PreferredTransactionType = null;
                    Model.TransporterID = null;
                    Model.GateID = null;
                    Model.ProductID = null;
                    Model.DriverID = null;
                    Model.Charges = null;
                    Model.TokenInterval = null;
                    Model.TokenExpireDate = null;
                }
                if (Model.IsValid())
                    SaveChange();
            }
            catch(Exception ex)
            {

            }
           
        }
        private async void SaveChange()
        {
            try
            {
                await dataRepository.SaveAsync();
                ToastHelper.ShowSuccess("Token information recorded successfully");
                RaiseDetailSavedEvent(Model.TokenID, Model);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var databaseValues = ex.Entries.Single().GetDatabaseValues();
                if (databaseValues == null)
                {
                    MessageBoxHelper.ShowInfoDialogAsync("Save Failed!", "The entity has been deleted by another user").Wait();
                    return;
                }

                var result = MessageBoxHelper.ShowOkCancelDialogAsync("Save Failed!", "The entity has been changed in "
                 + "the meantime by someone else. Click OK to save your changes anyway, click Cancel "
                 + "to reload the entity from the database.").Result;

                if (result == MessageDialogResult.Affirmative)
                {
                    // Update the original values with database-values
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    dataRepository.SaveAsync().Wait();
                    ToastHelper.ShowSuccess("Token information recorded successfully");
                    RaiseDetailSavedEvent(Model.TokenID, Model);
                }
                else
                {
                    // Reload entity from database
                    ex.Entries.Single().ReloadAsync().Wait();
                    LoadAsync().Wait();
                }
            }
        }
        protected virtual void RaiseDetailSavedEvent(int ObjectID, Object ObjectInstance)
        {
            EventAggregator.GetEvent<AfterDetailSavedEvent>().Publish(new AfterDetailSavedEventArgs
            {
                ObjectID = ObjectID,
                ObjectInstance = ObjectInstance,
                ObjectType = ObjectInstance.GetType().Name
            });
        }
        public void Dispose()
        {
            dataRepository.Context.Dispose();
        }

        public void OnVehicleSave(string VehicleNumber)
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
                }
                else
                {
                    string query = $"INSERT INTO Vehicle (VehicleNumber, IsActive) VALUES('{VehicleNumber}', 1)";
                    bool flag = DatabaseFunctions.ExecuteQuery(query);
                    if (flag)
                    {
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

        public VehicleBindingModel GetVehiclebyVehicleNumber(string VehicleNumber)
        {
            try
            {
                using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                {
                    if (ctx.Vehicles.Where(p => p.VehicleNumber.ToUpper() == VehicleNumber.ToUpper()).Count() > 0)
                    {
                        Vehicle account = ctx.Vehicles.Where(p => p.VehicleNumber.ToUpper() == VehicleNumber.ToUpper()).FirstOrDefault();

                        return new VehicleBindingModel { VehicleID = account.VehicleID, VehicleNumber = account.VehicleNumber };

                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        } 
        public AccountBindingModel GetAccountbyCompanyName(string CompanyName)
        {
            try
            {
                using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                {
                    if (ctx.Accounts.Where(p => p.CompanyName.ToUpper() == CompanyName.ToUpper() && p.IsAccount == true).Count() > 0)
                    {
                        Account account = ctx.Accounts.Where(p => p.CompanyName.ToUpper() == CompanyName.ToUpper() && p.IsAccount == true).FirstOrDefault();

                        return new AccountBindingModel { AccountID = account.AccountID, CompanyName = account.CompanyName };

                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public AccountBindingModel GetTraspoterbyCompanyName(string CompanyName)
        {
            try
            {
                using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                {
                    if (ctx.Accounts.Where(p => p.CompanyName.ToUpper() == CompanyName.ToUpper() && p.IsTransporter == true).Count() > 0)
                    {
                        Account account = ctx.Accounts.Where(p => p.CompanyName.ToUpper() == CompanyName.ToUpper() && p.IsTransporter == true).FirstOrDefault();

                        return new AccountBindingModel { AccountID = account.AccountID, CompanyName = account.CompanyName };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public ProductBindingModel GetProductbyProductName(string productName)
        {
            try
            {
                using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                {
                    if (ctx.Products.Where(p => p.ProductName.ToUpper() == productName.ToUpper()).Count() > 0)
                    {
                        Product account = ctx.Products.Where(p => p.ProductName.ToUpper() == productName.ToUpper()).FirstOrDefault();

                        return new ProductBindingModel { ProductID = account.ProductID, ProductName = account.ProductName };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public DriverBindingModel GetDriverbyDriverName(string DriverName)
        {
            try
            {
                using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                {
                    if (ctx.Drivers.Where(p => p.FirstName.ToUpper() == DriverName.ToUpper()).Count() > 0)
                    {
                        Driver account = ctx.Drivers.Where(p => p.FirstName.ToUpper() == DriverName.ToUpper()).FirstOrDefault();

                        return new DriverBindingModel { DriverID = account.DriverID, FirstName = account.FirstName };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public Gate GetGatebyGateName(string GateName)
        {
            try
            {
                using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                {
                    if (ctx.Gates.Where(p => p.GateName.ToUpper() == GateName.ToUpper()).Count() > 0)
                    {
                        Gate account = ctx.Gates.Where(p => p.GateName.ToUpper() == GateName.ToUpper()).FirstOrDefault();

                        return new Gate { GateName = account.GateName, GateID = account.GateID };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}

