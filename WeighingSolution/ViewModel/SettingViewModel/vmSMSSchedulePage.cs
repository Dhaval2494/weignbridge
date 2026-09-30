using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;

namespace WeighingSolution.ViewModel
{
    public class vmSMSSchedulePage : NotifyDataErrorInfoBase, IDisposable
    {
        private DataRepository dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));

        public readonly IEventAggregator EventAggregator = WeighingSolution.DataLayer.AppEventManager.Instance.EventAggregator;
        Page ParentPage;


        public vmSMSSchedulePage(Page page, int SMSId)
        {
            ParentPage = page;
            Id = SMSId;
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            CloseDetailViewCommand = new DelegateCommand<string>(OnCloseDetailViewExecute);
        }


        #region Command and Properties Declaration
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand<string> CloseDetailViewCommand { get; set; }

        private int _id;
        public int Id
        {
            get { return _id; }
            protected set { _id = value; }
        }
        private SMSSetup _model;
        public SMSSetup Model
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

        private ObservableCollection<CustomField> _customField;
        public ObservableCollection<CustomField> customField
        {
            get { return _customField; }
            set { _customField = value; OnPropertyChanged(); }
        }
        private List<CustomValue> _customvalue;
        public List<CustomValue> customvalue
        {
            get { return _customvalue; }
            set { _customvalue = value; OnPropertyChanged(); }
        }
        private List<Account> _accounts;
        public List<Account> Accounts
        {
            get { return _accounts; }
            set { _accounts = value; OnPropertyChanged(); }
        }
        private List<Product> _products;
        public List<Product> Products
        {
            get { return _products; }
            set { _products = value; OnPropertyChanged(); }
        }
        private List<Vehicle> _vehicles;
        public List<Vehicle> Vehicles
        {
            get { return _vehicles; }
            set { _vehicles = value; OnPropertyChanged(); }
        }

        private List<string> _lstTransMode;
        public List<string> lstTransMode
        {
            get { return _lstTransMode; }
            set { _lstTransMode = value; OnPropertyChanged(); }
        }

        private List<string> _lstTransStatus;
        public List<string> lstTransStatus
        {
            get { return _lstTransStatus; }
            set { _lstTransStatus = value; OnPropertyChanged(); }
        }

        private List<string> _lstTranType;
        public List<string> lstTranType
        {
            get { return _lstTranType; }
            set { _lstTranType = value; OnPropertyChanged(); }
        }

        private ObservableCollection<int> _lsthour = new ObservableCollection<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 };
        public ObservableCollection<int> lsthour
        {
            get { return _lsthour; }
            set { _lsthour = value; OnPropertyChanged(); }
        }
        private ObservableCollection<string> _lstday = new ObservableCollection<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        public ObservableCollection<string> lstday
        {
            get { return _lstday; }
            set { _lstday = value; OnPropertyChanged(); }
        }
        private ObservableCollection<int> _lstdate = new ObservableCollection<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };
        public ObservableCollection<int> lstdate
        {
            get { return _lstdate; }
            set { _lstdate = value; OnPropertyChanged(); }
        }
        private ObservableCollection<int> _lstmonth = new ObservableCollection<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        public ObservableCollection<int> lstmonth
        {
            get { return _lstmonth; }
            set { _lstmonth = value; OnPropertyChanged(); }
        }

        private SMSModel _SMSModel;
        public SMSModel SMSModel
        {
            get { return _SMSModel; }
            set { _SMSModel = value; OnPropertyChanged(); }
        }

        private SceheduleTime _st;
        public SceheduleTime ST
        {
            get { return _st; }
            set { _st = value; OnPropertyChanged(); }
        }

        private Visibility _SMSToVisibility;

        public Visibility SMSToVisibility
        {
            get { return _SMSToVisibility; }
            set
            {
                _SMSToVisibility = value;
                OnPropertyChanged();
            }
        }

        public String _SMSType;
        public String SMSType
        {
            get { return _SMSType; }
            set
            {
                _SMSType = value;

                if (value == "Every Transaction")
                    SMSToVisibility = Visibility.Visible;
                else
                    SMSToVisibility = Visibility.Hidden;
                OnPropertyChanged();
            }
        }

        #endregion
        public async Task LoadAsync()
        {
            var SMSSetup = Id > 0
              ? await dataRepository.Context.SMSSetups.SingleAsync(x => x.SMSSetupID == Id)
              : CreateNewSMSSetup();
            Model = SMSSetup;
            SMSType = SMSSetup.SMSType;
            customField = new ObservableCollection<CustomField>(dataRepository.Context.CustomFields.Where(x => x.IsVisible).AsNoTracking().ToList());
            if (Id > 0)
            {
                if (!string.IsNullOrEmpty(Model.ColumnSelection))
                {
                    string[] columnselection = Model.ColumnSelection.Split(',');
                    foreach (var item in columnselection)
                    {
                        string checboxstr = "cb" + item.ToString();
                        Grid grid = ParentPage.FindName("gridDataSelection") as Grid;
                        CheckBox checkbox = grid.FindName(checboxstr) as CheckBox;
                        if (checkbox != null)
                            checkbox.IsChecked = true;

                        foreach (var Field in customField)
                        {
                            if (Field.FieldName == item)
                            {
                                Field.Isselected = true;
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Model.FilterDetail))
                {
                    SMSModel = Newtonsoft.Json.JsonConvert.DeserializeObject<SMSModel>(Model.FilterDetail);
                    if (customField.FirstOrDefault(x => x.FieldName == "Field1") != null)
                        customField.FirstOrDefault(x => x.FieldName == "Field1").SelectedfieldValues = SMSModel.field1;
                    if (customField.FirstOrDefault(x => x.FieldName == "Field2") != null)
                        customField.FirstOrDefault(x => x.FieldName == "Field2").SelectedfieldValues = SMSModel.field2;
                    if (customField.FirstOrDefault(x => x.FieldName == "Field3") != null)
                        customField.FirstOrDefault(x => x.FieldName == "Field3").SelectedfieldValues = SMSModel.field3;
                    if (customField.FirstOrDefault(x => x.FieldName == "Field4") != null)
                        customField.FirstOrDefault(x => x.FieldName == "Field4").SelectedfieldValues = SMSModel.field4;
                    if (customField.FirstOrDefault(x => x.FieldName == "Field5") != null)
                        customField.FirstOrDefault(x => x.FieldName == "Field5").SelectedfieldValues = SMSModel.field5;
                    if (customField.FirstOrDefault(x => x.FieldName == "Field6") != null)
                        customField.FirstOrDefault(x => x.FieldName == "Field6").SelectedfieldValues = SMSModel.field6;
                    if (customField.FirstOrDefault(x => x.FieldName == "Field7") != null)
                        customField.FirstOrDefault(x => x.FieldName == "Field7").SelectedfieldValues = SMSModel.field7;
                    if (customField.FirstOrDefault(x => x.FieldName == "Field8") != null)
                        customField.FirstOrDefault(x => x.FieldName == "Field8").SelectedfieldValues = SMSModel.field8;
                    if (customField.FirstOrDefault(x => x.FieldName == "Field9") != null)
                        customField.FirstOrDefault(x => x.FieldName == "Field9").SelectedfieldValues = SMSModel.field9;
                    if (customField.FirstOrDefault(x => x.FieldName == "Field10") != null)
                        customField.FirstOrDefault(x => x.FieldName == "Field10").SelectedfieldValues = SMSModel.field10;
                    if (customField.FirstOrDefault(x => x.FieldName == "Field11") != null)
                        customField.FirstOrDefault(x => x.FieldName == "Field11").SelectedfieldValues = SMSModel.field11;
                    if (customField.FirstOrDefault(x => x.FieldName == "Field12") != null)
                        customField.FirstOrDefault(x => x.FieldName == "Field12").SelectedfieldValues = SMSModel.field12;
                    if (customField.FirstOrDefault(x => x.FieldName == "Field13") != null)
                        customField.FirstOrDefault(x => x.FieldName == "Field13").SelectedfieldValues = SMSModel.field13;
                    if (customField.FirstOrDefault(x => x.FieldName == "Field14") != null)
                        customField.FirstOrDefault(x => x.FieldName == "Field14").SelectedfieldValues = SMSModel.field14;
                    if (customField.FirstOrDefault(x => x.FieldName == "Field15") != null)
                        customField.FirstOrDefault(x => x.FieldName == "Field15").SelectedfieldValues = SMSModel.field15;
                    if (customField.FirstOrDefault(x => x.FieldName == "Field16") != null)
                        customField.FirstOrDefault(x => x.FieldName == "Field16").SelectedfieldValues = SMSModel.field16;
                    if (customField.FirstOrDefault(x => x.FieldName == "Field17") != null)
                        customField.FirstOrDefault(x => x.FieldName == "Field17").SelectedfieldValues = SMSModel.field17;
                    if (customField.FirstOrDefault(x => x.FieldName == "Field18") != null)
                        customField.FirstOrDefault(x => x.FieldName == "Field18").SelectedfieldValues = SMSModel.field18;
                    if (customField.FirstOrDefault(x => x.FieldName == "Field19") != null)
                        customField.FirstOrDefault(x => x.FieldName == "Field19").SelectedfieldValues = SMSModel.field19;
                    if (customField.FirstOrDefault(x => x.FieldName == "Field20") != null)
                        customField.FirstOrDefault(x => x.FieldName == "Field20").SelectedfieldValues = SMSModel.field20;
                }
                if (!string.IsNullOrEmpty(Model.ScheduleTime))
                    ST = Newtonsoft.Json.JsonConvert.DeserializeObject<SceheduleTime>(Model.ScheduleTime);
            }
            Products = await dataRepository.Context.Products.Where(x => x.IsActive).ToListAsync();
            Vehicles = await dataRepository.Context.Vehicles.Where(x => x.IsActive).ToListAsync();
            Accounts = await dataRepository.Context.Accounts.Where(x => x.Active).ToListAsync();
            lstTransMode = await dataRepository.Context.Database.SqlQuery<string>("select distinct TransactionMode from TransactionData").ToListAsync();
            lstTransStatus = await dataRepository.Context.Database.SqlQuery<string>("select distinct Status from TransactionData").ToListAsync();
            lstTranType = await dataRepository.Context.Database.SqlQuery<string>("select distinct TransactionType from TransactionData").ToListAsync();
            foreach (var item in customField)
            {
                item.FieldValues = new List<string>();
                string propName = $"CustomValue{item.CustomFieldID}";
                item.FieldValues = dataRepository.Context.TransactionDatas.AsEnumerable()
                    .Select(t =>
                    {
                        var prop = typeof(TransactionData).GetProperty(propName);
                        return prop?.GetValue(t)?.ToString();
                    })
                    .Where(v => !string.IsNullOrWhiteSpace(v)).Distinct().ToList();

                item.PropertyChanged += item_PropertyChanged;
            }

            Model.PropertyChanged -= Model_PropertyChanged;
            Model.PropertyChanged += Model_PropertyChanged;
            if (SMSModel != null)
            {
                SMSModel.PropertyChanged -= SMSModel_Model_PropertyChanged;
                SMSModel.PropertyChanged += SMSModel_Model_PropertyChanged;
            }
            if (ST != null)
            {
                ST.PropertyChanged -= ScheduleTime_Model_PropertyChanged;
                ST.PropertyChanged += ScheduleTime_Model_PropertyChanged;
            }
            SaveCommand.RaiseCanExecuteChanged();
        }
        private SMSSetup CreateNewSMSSetup()
        {
            SMSModel = new SMSModel();
            ST = new SceheduleTime();
            var SMSSetup = new SMSSetup();
            SMSSetup.IsEnable = true;
            SMSSetup.SMSType = "Every Transaction";
            SMSSetup.ScheduleType = "Daily";
            ST.Hour = 0;
            ST.Date = 1;
            ST.Day = "Monday";
            ST.Month = 1;
            dataRepository.Context.SMSSetups.Add(SMSSetup);
            return SMSSetup;
        }
        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedfieldValues")
                SetFilterDetail();
        }
        private void ScheduleTime_Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Model.ScheduleTime = Newtonsoft.Json.JsonConvert.SerializeObject(ST);
        }

        private void SMSModel_Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AccountID" || e.PropertyName == "ProductID" || e.PropertyName == "VehicleID" || e.PropertyName == "Status" || e.PropertyName == "TransactionMode" || e.PropertyName == "TransactionType" || e.PropertyName == "SelectedfieldValues")
                SetFilterDetail();
        }

        protected void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
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

        private async void OnCloseDetailViewExecute(string obj)
        {
            if (HasChanges)
            {
                var result = await MessageBoxHelper.ShowOkCancelDialogAsync("Are you sure?", "You've made changes. Close this item?");
                if (result == MessageDialogResult.Negative)
                    return;
            }
            GoBack();
        }

        private bool OnSaveCanExecute()
        {
            return Model != null && !Model.HasErrors && HasChanges;
        }

        public void SaveData()
        {
            if (Model.IsValid())
            {
                try
                {
                    SetColumnselection();   //For Every Transaction - Column Selection
                    SetFilterDetail();     // For Transaction Detail & Transaction summary- Filter Detail
                    SetWhereClauseAndFilterstring();    // For Transaction Detail & Transaction summary - Where Clause & Filterstring
                    SetScheduleTime();    // For Transaction Detail & Transaction summary  - Schedule Time
                    if (!string.IsNullOrEmpty(Model.SMSTo))
                    {
                        Model.SMSTo = Model.SMSTo.Trim();
                        Model.SMSTo = Model.SMSTo.Replace(" ", "");
                    }
                    Model.SMSType = SMSType;
                    dataRepository.Save();
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
                        ToastHelper.ShowSuccess("SMS setup information recorded successfully");
                        GoBack();
                    }
                    else
                    {
                        // Reload entity from database
                        ex.Entries.Single().ReloadAsync().Wait();
                        LoadAsync().Wait();
                    }
                }
                catch (Exception ex)
                {
                    string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                    File.AppendAllText(Constant.PluginFileLocation + @"\Error.txt", json);
                }
                finally
                {

                }

            }
        }

        private async void OnSaveExecute()
        {
            if (Model.IsValid())
            {
                try
                {
                    SetColumnselection();   //For Every Transaction - Column Selection
                    SetFilterDetail();     // For Transaction Detail & Transaction summary- Filter Detail
                    SetWhereClauseAndFilterstring();    // For Transaction Detail & Transaction summary - Where Clause & Filterstring
                    SetScheduleTime();    // For Transaction Detail & Transaction summary  - Schedule Time
                    Model.SMSTo = Model.SMSTo.Trim();
                    Model.SMSTo = Model.SMSTo.Replace(" ", "");
                    await dataRepository.SaveAsync();
                    ToastHelper.ShowSuccess("SMS Setup information recorded successfully");
                    GoBack();
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
                        ToastHelper.ShowSuccess("SMS setup information recorded successfully");
                        GoBack();
                    }
                    else
                    {
                        // Reload entity from database
                        ex.Entries.Single().ReloadAsync().Wait();
                        LoadAsync().Wait();
                    }
                }
                catch (Exception ex)
                {
                    string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                    File.AppendAllText(Constant.PluginFileLocation + @"\Error.txt", json);
                }
                finally
                {

                }

            }
        }
        public void SetColumnselection()
        {
            if (Model != null)
            {
                string Columnselection = "";
                Grid grid = ParentPage.FindName("gridDataSelection") as Grid;
                if (grid != null && grid.Children.Count > 0)
                {
                    foreach (Control cntrl in grid.Children)
                    {
                        if (cntrl.GetType() == typeof(CheckBox))
                        {
                            CheckBox checkbox = cntrl as CheckBox;
                            if (checkbox.IsChecked.Value && checkbox.Name != "cbField")
                                Columnselection += checkbox.Name.Remove(0, 2) + ",";
                        }
                    }
                    foreach (var item in customField)
                    {
                        if (item.Isselected)
                        {
                            Columnselection += item.FieldName + ",";
                        }
                    }
                    if (!string.IsNullOrEmpty(Columnselection))
                        Model.ColumnSelection = Columnselection.TrimEnd(',');
                }
            }
        }
        private void SetFilterDetail()
        {
            if (SMSModel != null && Model != null)
            {
                if (customField.FirstOrDefault(x => x.FieldName == "Field1") != null)
                    SMSModel.field1 = customField.FirstOrDefault(x => x.FieldName == "Field1").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field2") != null)
                    SMSModel.field2 = customField.FirstOrDefault(x => x.FieldName == "Field2").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field3") != null)
                    SMSModel.field3 = customField.FirstOrDefault(x => x.FieldName == "Field3").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field4") != null)
                    SMSModel.field4 = customField.FirstOrDefault(x => x.FieldName == "Field4").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field5") != null)
                    SMSModel.field5 = customField.FirstOrDefault(x => x.FieldName == "Field5").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field6") != null)
                    SMSModel.field6 = customField.FirstOrDefault(x => x.FieldName == "Field6").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field7") != null)
                    SMSModel.field7 = customField.FirstOrDefault(x => x.FieldName == "Field7").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field8") != null)
                    SMSModel.field8 = customField.FirstOrDefault(x => x.FieldName == "Field8").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field9") != null)
                    SMSModel.field9 = customField.FirstOrDefault(x => x.FieldName == "Field9").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field10") != null)
                    SMSModel.field10 = customField.FirstOrDefault(x => x.FieldName == "Field10").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field11") != null)
                    SMSModel.field11 = customField.FirstOrDefault(x => x.FieldName == "Field11").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field12") != null)
                    SMSModel.field12 = customField.FirstOrDefault(x => x.FieldName == "Field12").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field13") != null)
                    SMSModel.field13 = customField.FirstOrDefault(x => x.FieldName == "Field13").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field14") != null)
                    SMSModel.field14 = customField.FirstOrDefault(x => x.FieldName == "Field14").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field15") != null)
                    SMSModel.field15 = customField.FirstOrDefault(x => x.FieldName == "Field15").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field16") != null)
                    SMSModel.field16 = customField.FirstOrDefault(x => x.FieldName == "Field16").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field17") != null)
                    SMSModel.field17 = customField.FirstOrDefault(x => x.FieldName == "Field17").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field18") != null)
                    SMSModel.field18 = customField.FirstOrDefault(x => x.FieldName == "Field18").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field19") != null)
                    SMSModel.field19 = customField.FirstOrDefault(x => x.FieldName == "Field19").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field20") != null)
                    SMSModel.field20 = customField.FirstOrDefault(x => x.FieldName == "Field20").SelectedfieldValues;
                Model.FilterDetail = Newtonsoft.Json.JsonConvert.SerializeObject(SMSModel);
            }
        }
        private void SetWhereClauseAndFilterstring()
        {
            if (SMSModel != null)
            {
                string WhereClause = "";
                string strFilterstring = "";
                if (!string.IsNullOrEmpty(SMSModel.TransactionMode))
                {
                    WhereClause = "TData.TransactionMode = '" + SMSModel.TransactionMode + "' AND ";
                    strFilterstring += "TransactionMode is " + SMSModel.TransactionMode + " and";
                }
                if (!string.IsNullOrEmpty(SMSModel.Status))
                {
                    WhereClause += "TData.STATUS = '" + SMSModel.Status + "' AND ";
                    strFilterstring += "Transcation Status is " + SMSModel.Status + " and ";
                }
                if (!string.IsNullOrEmpty(SMSModel.TransactionType))
                {
                    WhereClause += "TData.TransactionType = '" + SMSModel.TransactionType + "' AND ";
                    strFilterstring += "Transcation Type is " + SMSModel.TransactionType + " and ";
                }
                if (SMSModel.AccountID > 0)
                {
                    WhereClause += "TData.AccountID = " + SMSModel.AccountID + " AND ";
                    strFilterstring += "Account is " + Accounts.FirstOrDefault(x => x.AccountID == SMSModel.AccountID).CompanyName + " and ";
                }
                if (SMSModel.ProductID > 0)
                {
                    WhereClause += "TDetail.ProductID = " + SMSModel.ProductID + " AND ";
                    strFilterstring += "Product is " + Products.FirstOrDefault(x => x.ProductID == SMSModel.ProductID).ProductName + " and ";
                }
                if (SMSModel.VehicleID > 0)
                {
                    WhereClause += "TData.VehicleID = " + SMSModel.VehicleID + " AND ";
                    strFilterstring += "Vehicle is " + Vehicles.FirstOrDefault(x => x.VehicleID == SMSModel.VehicleID).VehicleNumber + " and ";
                }
                if (!string.IsNullOrEmpty(SMSModel.field1))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + SMSModel.field1 + "%' AND CV.CustomFieldID = 1 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field1 is " + SMSModel.field1 + " and ";
                }
                if (!string.IsNullOrEmpty(SMSModel.field2))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + SMSModel.field2 + "%'AND CV.CustomFieldID = 2 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field2 is " + SMSModel.field2 + " and ";
                }
                if (!string.IsNullOrEmpty(SMSModel.field3))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + SMSModel.field3 + "%'AND CV.CustomFieldID = 3 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field3 is " + SMSModel.field3 + " and ";
                }
                if (!string.IsNullOrEmpty(SMSModel.field4))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + SMSModel.field4 + "%'AND CV.CustomFieldID = 4 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field4 is " + SMSModel.field4 + " and ";
                }
                if (!string.IsNullOrEmpty(SMSModel.field5))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + SMSModel.field5 + "%'AND CV.CustomFieldID = 5 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field5 is " + SMSModel.field5 + " and ";
                }
                if (!string.IsNullOrEmpty(SMSModel.field6))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + SMSModel.field6 + "%'AND CV.CustomFieldID = 6 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field6 is " + SMSModel.field6 + " and ";
                }
                if (!string.IsNullOrEmpty(SMSModel.field7))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + SMSModel.field7 + "%'AND CV.CustomFieldID = 7 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field7 is " + SMSModel.field7 + " and ";
                }
                if (!string.IsNullOrEmpty(SMSModel.field8))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + SMSModel.field8 + "%'AND CV.CustomFieldID = 8 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field8 is " + SMSModel.field8 + " and ";
                }
                if (!string.IsNullOrEmpty(SMSModel.field9))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + SMSModel.field9 + "%'AND CV.CustomFieldID = 9 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field9 is " + SMSModel.field9 + " and ";
                }
                if (!string.IsNullOrEmpty(SMSModel.field10))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + SMSModel.field10 + "%'AND CV.CustomFieldID = 10 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field10 is " + SMSModel.field10 + " and ";
                }
                if (!string.IsNullOrEmpty(SMSModel.field11))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + SMSModel.field11 + "%'AND CV.CustomFieldID = 11 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field11 is " + SMSModel.field11 + " and ";
                }
                if (!string.IsNullOrEmpty(SMSModel.field12))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + SMSModel.field12 + "%'AND CV.CustomFieldID = 12 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field12 is " + SMSModel.field12 + " and ";
                }
                if (!string.IsNullOrEmpty(SMSModel.field13))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + SMSModel.field13 + "%'AND CV.CustomFieldID = 13 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field13 is " + SMSModel.field13 + " and ";
                }
                if (!string.IsNullOrEmpty(SMSModel.field14))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + SMSModel.field14 + "%'AND CV.CustomFieldID = 14 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field14 is " + SMSModel.field14 + " and ";
                }
                if (!string.IsNullOrEmpty(SMSModel.field15))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + SMSModel.field15 + "%'AND CV.CustomFieldID = 15 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field15 is " + SMSModel.field15 + " and ";
                }
                if (!string.IsNullOrEmpty(SMSModel.field16))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + SMSModel.field16 + "%'AND CV.CustomFieldID = 16 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field16 is " + SMSModel.field16 + " and ";
                }
                if (!string.IsNullOrEmpty(SMSModel.field17))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + SMSModel.field17 + "%'AND CV.CustomFieldID = 17 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field17 is " + SMSModel.field17 + " and ";
                }
                if (!string.IsNullOrEmpty(SMSModel.field18))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + SMSModel.field18 + "%'AND CV.CustomFieldID = 18 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field18 is " + SMSModel.field18 + " and ";
                }
                if (!string.IsNullOrEmpty(SMSModel.field19))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + SMSModel.field19 + "%'AND CV.CustomFieldID = 19 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field19 is " + SMSModel.field19 + " and ";
                }
                if (!string.IsNullOrEmpty(SMSModel.field20))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + SMSModel.field20 + "%'AND CV.CustomFieldID = 20 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field20 is " + SMSModel.field20 + " and ";
                }
                if (WhereClause != "")
                {
                    WhereClause = ReplaceLastOccurrence(WhereClause, "AND", "");
                    Model.WhereClause = WhereClause;
                }
                else
                    Model.WhereClause = "";
                if (strFilterstring != "")
                {
                    Model.Filterstring = "(Filtered for: Transaction Date between xxxStartDatexxx To xxxEndDatexxx and " + ReplaceLastOccurrence(strFilterstring, " and", "") + " )";
                }
                else
                {
                    Model.Filterstring = "(Filtered for: Transaction Date between xxxStartDatexxx To xxxEndDatexxx)";
                }
            }
        }
        private void SetScheduleTime()
        {
            if (ST != null)
            {
                Model.ScheduleTime = Newtonsoft.Json.JsonConvert.SerializeObject(ST);
                DateTime DTScTimeDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ST.Hour, 0, 0);
                if (Model.ScheduleType == "Daily")
                {
                    if (DTScTimeDay > DateTime.Now)
                        Model.NextRunTime = DTScTimeDay;
                    else
                        Model.NextRunTime = DTScTimeDay.AddDays(1);
                }
                if (Model.ScheduleType == "Week")
                {
                    DayOfWeek dow = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), ST.Day);
                    if (ST.Day != DateTime.Now.DayOfWeek.ToString())
                        Model.NextRunTime = GetNextWeekday(DateTime.Today, dow).AddHours(ST.Hour);
                    else
                    {
                        DateTime DTScTimeWeek = DateTime.Today.AddHours(ST.Hour);
                        if (DTScTimeWeek > DateTime.Now)
                            Model.NextRunTime = DTScTimeWeek;
                        else
                            Model.NextRunTime = DTScTimeWeek.AddDays(7);
                    }
                }
                if (Model.ScheduleType == "Month")
                {
                    DateTime dtScMonth;
                    string CurrentDate = "";
                    CurrentDate = ST.Date.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString() + " " + ST.Hour.ToString() + ":00:00";
                    if (DateTime.TryParseExact(CurrentDate, "dd/MM/yyyy H:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dtScMonth))
                        dtScMonth = Convert.ToDateTime(CurrentDate);
                    else
                        dtScMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month), ST.Hour, 0, 0);
                    if (dtScMonth > DateTime.Now)
                        Model.NextRunTime = dtScMonth;
                    else
                        Model.NextRunTime = dtScMonth.AddMonths(1);
                }
                if (Model.ScheduleType == "Year")
                {
                    DateTime DTScTimeYear = new DateTime(DateTime.Now.Year, ST.Month, ST.Date, ST.Hour, 0, 0);
                    if (DTScTimeYear > DateTime.Now)
                        Model.NextRunTime = DTScTimeYear;
                    else
                        Model.NextRunTime = DTScTimeYear.AddYears(1);
                }
            }
        }
        public static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }
        public static string ReplaceLastOccurrence(string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find);

            if (place == -1)
                return Source;

            string result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }
        public void GoBack()
        {
            Uri uri = new Uri("/View/SettingView/SMSScheduleList_Page.Xaml", UriKind.Relative);
            ParentPage.NavigationService.Navigate(uri);
        }
        public void Dispose()
        {
            dataRepository.Context.Dispose();
            dataRepository.Context.Dispose();
        }
    }
    public class SMSModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private int _Acnt;
        public int AccountID
        {
            get { return _Acnt; }
            set { _Acnt = value; OnPropertyChanged(); }
        }

        private int _Product;
        public int ProductID
        {
            get { return _Product; }
            set { _Product = value; OnPropertyChanged(); }
        }

        private int _Vehicle;
        public int VehicleID
        {
            get { return _Vehicle; }
            set { _Vehicle = value; OnPropertyChanged(); }
        }

        private string _TranStatus;
        public string Status
        {
            get { return _TranStatus; }
            set { _TranStatus = value; OnPropertyChanged(); }
        }

        private string _TransactionType;
        public string TransactionType
        {
            get { return _TransactionType; }
            set { _TransactionType = value; OnPropertyChanged(); }
        }

        private string _TranMode;
        public string TransactionMode
        {
            get { return _TranMode; }
            set { _TranMode = value; OnPropertyChanged(); }
        }

        private string _field1;
        public string field1
        {
            get { return _field1; }
            set { _field1 = value; OnPropertyChanged(); }
        }

        private string _field2;
        public string field2
        {
            get { return _field2; }
            set { _field2 = value; OnPropertyChanged(); }
        }

        private string _field3;
        public string field3
        {
            get { return _field3; }
            set { _field3 = value; OnPropertyChanged(); }
        }

        private string _field4;
        public string field4
        {
            get { return _field4; }
            set { _field4 = value; OnPropertyChanged(); }
        }

        private string _field5;
        public string field5
        {
            get { return _field5; }
            set { _field5 = value; OnPropertyChanged(); }
        }

        private string _field6;
        public string field6
        {
            get { return _field6; }
            set { _field6 = value; OnPropertyChanged(); }
        }

        private string _field7;
        public string field7
        {
            get { return _field7; }
            set { _field7 = value; OnPropertyChanged(); }
        }

        private string _field8;
        public string field8
        {
            get { return _field8; }
            set { _field8 = value; OnPropertyChanged(); }
        }

        private string _field9;
        public string field9
        {
            get { return _field9; }
            set { _field9 = value; OnPropertyChanged(); }
        }

        private string _field10;
        public string field10
        {
            get { return _field10; }
            set { _field10 = value; OnPropertyChanged(); }
        }

        private string _field11;
        public string field11
        {
            get { return _field11; }
            set { _field11 = value; OnPropertyChanged(); }
        }

        private string _field12;
        public string field12
        {
            get { return _field12; }
            set { _field12 = value; OnPropertyChanged(); }
        }

        private string _field13;
        public string field13
        {
            get { return _field13; }
            set { _field13 = value; OnPropertyChanged(); }
        }

        private string _field14;
        public string field14
        {
            get { return _field14; }
            set { _field14 = value; OnPropertyChanged(); }
        }
        private string _field15;
        public string field15
        {
            get { return _field15; }
            set { _field15 = value; OnPropertyChanged(); }
        }

        private string _field16;
        public string field16
        {
            get { return _field16; }
            set { _field16 = value; OnPropertyChanged(); }
        }

        private string _field17;
        public string field17
        {
            get { return _field17; }
            set { _field17 = value; OnPropertyChanged(); }
        }

        private string _field18;
        public string field18
        {
            get { return _field18; }
            set { _field18 = value; OnPropertyChanged(); }
        }

        private string _field19;
        public string field19
        {
            get { return _field19; }
            set { _field19 = value; OnPropertyChanged(); }
        }

        private string _field20;
        public string field20
        {
            get { return _field20; }
            set { _field20 = value; OnPropertyChanged(); }
        }

    }

}
