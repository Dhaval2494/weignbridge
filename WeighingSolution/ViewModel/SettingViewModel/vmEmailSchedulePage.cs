using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;
using WeighingSolution.SharedComponents;

namespace WeighingSolution.ViewModel
{
    public class vmEmailSchedulePage : NotifyDataErrorInfoBase, IDisposable
    {
        private DataRepository dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));
        public readonly IEventAggregator EventAggregator = WeighingSolution.DataLayer.AppEventManager.Instance.EventAggregator;
        Page ParentPage;
        public vmEmailSchedulePage(Page page, int EmailId)
        {
            ParentPage = page;
            Id = EmailId;
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            CloseDetailViewCommand = new DelegateCommand<string>(OnCloseDetailViewExecute);
            CBCheckedUnchecked = new DelegateCommand<Nullable<bool>>(OnCBCheckedUnchecked);
        }


        #region Command and Properties Declaration
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand<string> CloseDetailViewCommand { get; set; }
        public DelegateCommand<Nullable<bool>> CBCheckedUnchecked { get; set; }

        private int _id;
        public int Id
        {
            get { return _id; }
            protected set { _id = value; }
        }
        private EmailSetup _model;
        public EmailSetup Model
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

        private List<AccountBindingModel> _accounts;
        public List<AccountBindingModel> Accounts
        {
            get { return _accounts; }
            set { _accounts = value; OnPropertyChanged(); }
        }
        private List<ProductBindingModel> _products;
        public List<ProductBindingModel> Products
        {
            get { return _products; }
            set { _products = value; OnPropertyChanged(); }
        }
        private List<VehicleBindingModel> _vehicles;
        public List<VehicleBindingModel> Vehicles
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

        private EmailModel _EmailModel;
        public EmailModel EmailModel
        {
            get { return _EmailModel; }
            set { _EmailModel = value; OnPropertyChanged(); }
        }

        private SceheduleTime _st;
        public SceheduleTime ST
        {
            get { return _st; }
            set { _st = value; OnPropertyChanged(); }
        }

        #endregion
        public async Task LoadAsync()
        {
            try
            {
                var emailsetup = Id > 0
                  ? await dataRepository.Context.EmailSetups.SingleAsync(x => x.EmailSetupID == Id)
                  : CreateNewEmailSetup();
                Model = emailsetup;
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
                        EmailModel = Newtonsoft.Json.JsonConvert.DeserializeObject<EmailModel>(Model.FilterDetail);
                        if (customField.FirstOrDefault(x => x.FieldName == "Field1") != null)
                            customField.FirstOrDefault(x => x.FieldName == "Field1").SelectedfieldValues = EmailModel.field1;
                        if (customField.FirstOrDefault(x => x.FieldName == "Field2") != null)
                            customField.FirstOrDefault(x => x.FieldName == "Field2").SelectedfieldValues = EmailModel.field2;
                        if (customField.FirstOrDefault(x => x.FieldName == "Field3") != null)
                            customField.FirstOrDefault(x => x.FieldName == "Field3").SelectedfieldValues = EmailModel.field3;
                        if (customField.FirstOrDefault(x => x.FieldName == "Field4") != null)
                            customField.FirstOrDefault(x => x.FieldName == "Field4").SelectedfieldValues = EmailModel.field4;
                        if (customField.FirstOrDefault(x => x.FieldName == "Field5") != null)
                            customField.FirstOrDefault(x => x.FieldName == "Field5").SelectedfieldValues = EmailModel.field5;
                        if (customField.FirstOrDefault(x => x.FieldName == "Field6") != null)
                            customField.FirstOrDefault(x => x.FieldName == "Field6").SelectedfieldValues = EmailModel.field6;
                        if (customField.FirstOrDefault(x => x.FieldName == "Field7") != null)
                            customField.FirstOrDefault(x => x.FieldName == "Field7").SelectedfieldValues = EmailModel.field7;
                        if (customField.FirstOrDefault(x => x.FieldName == "Field8") != null)
                            customField.FirstOrDefault(x => x.FieldName == "Field8").SelectedfieldValues = EmailModel.field8;
                        if (customField.FirstOrDefault(x => x.FieldName == "Field9") != null)
                            customField.FirstOrDefault(x => x.FieldName == "Field9").SelectedfieldValues = EmailModel.field9;
                        if (customField.FirstOrDefault(x => x.FieldName == "Field10") != null)
                            customField.FirstOrDefault(x => x.FieldName == "Field10").SelectedfieldValues = EmailModel.field10;
                        if (customField.FirstOrDefault(x => x.FieldName == "Field11") != null)
                            customField.FirstOrDefault(x => x.FieldName == "Field11").SelectedfieldValues = EmailModel.field11;
                        if (customField.FirstOrDefault(x => x.FieldName == "Field12") != null)
                            customField.FirstOrDefault(x => x.FieldName == "Field12").SelectedfieldValues = EmailModel.field12;
                        if (customField.FirstOrDefault(x => x.FieldName == "Field13") != null)
                            customField.FirstOrDefault(x => x.FieldName == "Field13").SelectedfieldValues = EmailModel.field13;
                        if (customField.FirstOrDefault(x => x.FieldName == "Field14") != null)
                            customField.FirstOrDefault(x => x.FieldName == "Field14").SelectedfieldValues = EmailModel.field14;
                        if (customField.FirstOrDefault(x => x.FieldName == "Field15") != null)
                            customField.FirstOrDefault(x => x.FieldName == "Field15").SelectedfieldValues = EmailModel.field15;
                        if (customField.FirstOrDefault(x => x.FieldName == "Field16") != null)
                            customField.FirstOrDefault(x => x.FieldName == "Field16").SelectedfieldValues = EmailModel.field16;
                        if (customField.FirstOrDefault(x => x.FieldName == "Field17") != null)
                            customField.FirstOrDefault(x => x.FieldName == "Field17").SelectedfieldValues = EmailModel.field17;
                        if (customField.FirstOrDefault(x => x.FieldName == "Field18") != null)
                            customField.FirstOrDefault(x => x.FieldName == "Field18").SelectedfieldValues = EmailModel.field18;
                        if (customField.FirstOrDefault(x => x.FieldName == "Field19") != null)
                            customField.FirstOrDefault(x => x.FieldName == "Field19").SelectedfieldValues = EmailModel.field19;
                        if (customField.FirstOrDefault(x => x.FieldName == "Field20") != null)
                            customField.FirstOrDefault(x => x.FieldName == "Field20").SelectedfieldValues = EmailModel.field20;
                    }
                    if (!string.IsNullOrEmpty(Model.ScheduleTime))
                        ST = Newtonsoft.Json.JsonConvert.DeserializeObject<SceheduleTime>(Model.ScheduleTime);
                }
                Products = await dataRepository.Context.Products.Where(x => x.IsActive).Select(p => new ProductBindingModel { ProductID = p.ProductID, ProductName = p.ProductName, ProductCode = p.ProductCode }).ToListAsync();
                Vehicles = await dataRepository.Context.Vehicles.Where(x => x.IsActive).Select(p => new VehicleBindingModel { VehicleID = p.VehicleID, VehicleNumber = p.VehicleNumber }).ToListAsync();
                Accounts = await dataRepository.Context.Accounts.Where(x => x.Active).Select(p => new AccountBindingModel { AccountID = p.AccountID, CompanyName = p.CompanyName }).ToListAsync();

                //lstTransMode = await dataRepository.Context.Database.SqlQuery<string>("select distinct TransactionMode from TransactionData").ToListAsync();
                lstTransMode = new List<string> { "Single", "Double", "Multi" };
                //lstTransStatus = await dataRepository.Context.Database.SqlQuery<string>("select distinct Status from TransactionData").ToListAsync();
                lstTransStatus = new List<string> { "Active", "Complete", "Void" };
                //lstTranType = await dataRepository.Context.Database.SqlQuery<string>("select distinct TransactionType from TransactionData").ToListAsync();
                lstTranType = new List<string> { "Incoming", "Outgoing", "Both" };

                foreach (var item in customField)
                {
                    item.FieldValues = new List<string>();
                    string propName = $"CustomValue{item.CustomFieldID}";

                    string sqlQuery = $"select distinct {propName} from TransactionData";

                    DataTable dataTable = DatabaseFunctions.SQLExecuteReaderQuery(sqlQuery);
                    foreach (DataRow row in dataTable.Rows)
                    {
                        item.FieldValues.Add(row[0]?.ToString());
                    }
                    //item.FieldValues = dataRepository.Context.TransactionDatas.AsEnumerable()
                    //    .Select(t =>
                    //    {
                    //        var prop = typeof(TransactionData).GetProperty(propName);
                    //        return prop?.GetValue(t)?.ToString();
                    //    })
                    //    .Where(v => !string.IsNullOrWhiteSpace(v)).Distinct().ToList();
                    item.PropertyChanged += item_PropertyChanged;
                }

                Model.PropertyChanged -= Model_PropertyChanged;
                Model.PropertyChanged += Model_PropertyChanged;
                if (EmailModel != null)
                {
                    EmailModel.PropertyChanged -= EmailModel_Model_PropertyChanged;
                    EmailModel.PropertyChanged += EmailModel_Model_PropertyChanged;
                }
                if (ST != null)
                {
                    ST.PropertyChanged -= ScheduleTime_Model_PropertyChanged;
                    ST.PropertyChanged += ScheduleTime_Model_PropertyChanged;
                }
                SaveCommand.RaiseCanExecuteChanged();
            }
            catch
            {

            }
        }
        private EmailSetup CreateNewEmailSetup()
        {
            EmailModel = new EmailModel();
            ST = new SceheduleTime();
            var emailsetup = new EmailSetup();
            emailsetup.IsEnable = true;
            emailsetup.EmailType = "Every Transaction";
            emailsetup.ScheduleType = "Daily";
            ST.Hour = 0;
            ST.Date = 1;
            ST.Day = "Monday";
            ST.Month = 1;
            dataRepository.Context.EmailSetups.Add(emailsetup);
            return emailsetup;
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

        private void EmailModel_Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
            if (e.PropertyName == "EmailType")
            {
                OnPropertyChanged("IsReport");
            }
            if (e.PropertyName == "HasErrors")
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        private void OnCBCheckedUnchecked(bool? obj)
        {
            SetColumnselection();  // for Property change of ColumnSelection --> Every Transaction 
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

        private async void OnSaveExecute()
        {
            if (Model.IsValid())
            {
                try
                {
                    if ((Model.EmailSetupID == 0 && Model.EmailType == "Custom On Transaction" && dataRepository.Context.EmailSetups.Any(x => x.EmailType == "Custom On Transaction")) || (Model.EmailSetupID > 0 && Model.EmailType == "Custom On Transaction" && dataRepository.Context.EmailSetups.Any(x => x.EmailSetupID != Model.EmailSetupID && x.EmailType == "Custom On Transaction")))
                    {
                        ToastHelper.ShowWarning("Can not create more than one 'Custom On Transaction' Email schedule.");
                    }
                    else
                    {
                        SetColumnselection();   //For Every Transaction - Column Selection
                        SetFilterDetail();     // For Transaction Detail & Transaction summary- Filter Detail
                        SetWhereClauseAndFilterstring();    // For Transaction Detail & Transaction summary - Where Clause & Filterstring
                        SetScheduleTime();    // For Transaction Detail & Transaction summary  - Schedule Time
                        Model.EmailTo = Model.EmailTo.TrimEnd(';');
                        await dataRepository.SaveAsync();
                        ToastHelper.ShowSuccess("Email Setup information recorded successfully");
                        GoBack();
                    }
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
                        ToastHelper.ShowSuccess("Email setup information recorded successfully");
                        GoBack();
                    }
                    else
                    {
                        // Reload entity from database
                        ex.Entries.Single().ReloadAsync().Wait();
                        LoadAsync().Wait();
                    }
                }
                ;
            }
        }
        public void SetColumnselection()
        {
            if (Model != null)
            {
                string Columnselection = "";
                CheckBox cbIsAttachmentFile = ParentPage.FindName("cbIsAttachmentFile") as CheckBox;
                if (cbIsAttachmentFile != null && cbIsAttachmentFile.IsChecked.GetValueOrDefault(false))
                    Columnselection += "IsAttachmentFile" + ",";

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
            if (EmailModel != null && Model != null)
            {
                if (customField.FirstOrDefault(x => x.FieldName == "Field1") != null)
                    EmailModel.field1 = customField.FirstOrDefault(x => x.FieldName == "Field1").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field2") != null)
                    EmailModel.field2 = customField.FirstOrDefault(x => x.FieldName == "Field2").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field3") != null)
                    EmailModel.field3 = customField.FirstOrDefault(x => x.FieldName == "Field3").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field4") != null)
                    EmailModel.field4 = customField.FirstOrDefault(x => x.FieldName == "Field4").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field5") != null)
                    EmailModel.field5 = customField.FirstOrDefault(x => x.FieldName == "Field5").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field6") != null)
                    EmailModel.field6 = customField.FirstOrDefault(x => x.FieldName == "Field6").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field7") != null)
                    EmailModel.field7 = customField.FirstOrDefault(x => x.FieldName == "Field7").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field8") != null)
                    EmailModel.field8 = customField.FirstOrDefault(x => x.FieldName == "Field8").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field9") != null)
                    EmailModel.field9 = customField.FirstOrDefault(x => x.FieldName == "Field9").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field10") != null)
                    EmailModel.field10 = customField.FirstOrDefault(x => x.FieldName == "Field10").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field11") != null)
                    EmailModel.field11 = customField.FirstOrDefault(x => x.FieldName == "Field11").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field12") != null)
                    EmailModel.field12 = customField.FirstOrDefault(x => x.FieldName == "Field12").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field13") != null)
                    EmailModel.field13 = customField.FirstOrDefault(x => x.FieldName == "Field13").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field14") != null)
                    EmailModel.field14 = customField.FirstOrDefault(x => x.FieldName == "Field14").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field15") != null)
                    EmailModel.field15 = customField.FirstOrDefault(x => x.FieldName == "Field15").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field16") != null)
                    EmailModel.field16 = customField.FirstOrDefault(x => x.FieldName == "Field16").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field17") != null)
                    EmailModel.field17 = customField.FirstOrDefault(x => x.FieldName == "Field17").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field18") != null)
                    EmailModel.field18 = customField.FirstOrDefault(x => x.FieldName == "Field18").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field19") != null)
                    EmailModel.field19 = customField.FirstOrDefault(x => x.FieldName == "Field19").SelectedfieldValues;
                if (customField.FirstOrDefault(x => x.FieldName == "Field20") != null)
                    EmailModel.field20 = customField.FirstOrDefault(x => x.FieldName == "Field20").SelectedfieldValues;
                Model.FilterDetail = Newtonsoft.Json.JsonConvert.SerializeObject(EmailModel);
            }
        }
        private void SetWhereClauseAndFilterstring()
        {
            if (EmailModel != null)
            {
                string WhereClause = "";
                string strFilterstring = "";
                if (!string.IsNullOrEmpty(EmailModel.TransactionMode))
                {
                    WhereClause = "TData.TransactionMode = '" + EmailModel.TransactionMode + "' AND ";
                    strFilterstring += "TransactionMode is " + EmailModel.TransactionMode + " and";
                }
                if (!string.IsNullOrEmpty(EmailModel.Status))
                {
                    WhereClause += "TData.STATUS = '" + EmailModel.Status + "' AND ";
                    strFilterstring += "Transcation Status is " + EmailModel.Status + " and ";
                }
                if (!string.IsNullOrEmpty(EmailModel.TransactionType))
                {
                    WhereClause += "TData.TransactionType = '" + EmailModel.TransactionType + "' AND ";
                    strFilterstring += "Transcation Type is " + EmailModel.TransactionType + " and ";
                }
                if (EmailModel.AccountID > 0)
                {
                    WhereClause += "TData.AccountID = " + EmailModel.AccountID + " AND ";
                    strFilterstring += "Account is " + Accounts.FirstOrDefault(x => x.AccountID == EmailModel.AccountID).CompanyName + " and ";
                }
                if (EmailModel.ProductID > 0)
                {
                    WhereClause += "TDetail.ProductID = " + EmailModel.ProductID + " AND ";
                    strFilterstring += "Product is " + Products.FirstOrDefault(x => x.ProductID == EmailModel.ProductID).ProductName + " and ";
                }
                if (EmailModel.VehicleID > 0)
                {
                    WhereClause += "TData.VehicleID = " + EmailModel.VehicleID + " AND ";
                    strFilterstring += "Vehicle is " + Vehicles.FirstOrDefault(x => x.VehicleID == EmailModel.VehicleID).VehicleNumber + " and ";
                }
                if (!string.IsNullOrEmpty(EmailModel.field1))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + EmailModel.field1 + "%' AND CV.CustomFieldID = 1 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field1 is " + EmailModel.field1 + " and ";
                }
                if (!string.IsNullOrEmpty(EmailModel.field2))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + EmailModel.field2 + "%'AND CV.CustomFieldID = 2 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field2 is " + EmailModel.field2 + " and ";
                }
                if (!string.IsNullOrEmpty(EmailModel.field3))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + EmailModel.field3 + "%'AND CV.CustomFieldID = 3 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field3 is " + EmailModel.field3 + " and ";
                }
                if (!string.IsNullOrEmpty(EmailModel.field4))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + EmailModel.field4 + "%'AND CV.CustomFieldID = 4 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field4 is " + EmailModel.field4 + " and ";
                }
                if (!string.IsNullOrEmpty(EmailModel.field5))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + EmailModel.field5 + "%'AND CV.CustomFieldID = 5 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field5 is " + EmailModel.field5 + " and ";
                }
                if (!string.IsNullOrEmpty(EmailModel.field6))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + EmailModel.field6 + "%'AND CV.CustomFieldID = 6 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field6 is " + EmailModel.field6 + " and ";
                }
                if (!string.IsNullOrEmpty(EmailModel.field7))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + EmailModel.field7 + "%'AND CV.CustomFieldID = 7 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field7 is " + EmailModel.field7 + " and ";
                }
                if (!string.IsNullOrEmpty(EmailModel.field8))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + EmailModel.field8 + "%'AND CV.CustomFieldID = 8 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field8 is " + EmailModel.field8 + " and ";
                }
                if (!string.IsNullOrEmpty(EmailModel.field9))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + EmailModel.field9 + "%'AND CV.CustomFieldID = 9 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field9 is " + EmailModel.field9 + " and ";
                }
                if (!string.IsNullOrEmpty(EmailModel.field10))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + EmailModel.field10 + "%'AND CV.CustomFieldID = 10 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field10 is " + EmailModel.field10 + " and ";
                }
                if (!string.IsNullOrEmpty(EmailModel.field11))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + EmailModel.field11 + "%'AND CV.CustomFieldID = 11 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field11 is " + EmailModel.field11 + " and ";
                }
                if (!string.IsNullOrEmpty(EmailModel.field12))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + EmailModel.field12 + "%'AND CV.CustomFieldID = 12 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field12 is " + EmailModel.field12 + " and ";
                }
                if (!string.IsNullOrEmpty(EmailModel.field13))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + EmailModel.field13 + "%'AND CV.CustomFieldID = 13 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field13 is " + EmailModel.field13 + " and ";
                }
                if (!string.IsNullOrEmpty(EmailModel.field14))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + EmailModel.field14 + "%'AND CV.CustomFieldID = 14 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field14 is " + EmailModel.field14 + " and ";
                }
                if (!string.IsNullOrEmpty(EmailModel.field15))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + EmailModel.field15 + "%'AND CV.CustomFieldID = 15 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field15 is " + EmailModel.field15 + " and ";
                }
                if (!string.IsNullOrEmpty(EmailModel.field16))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + EmailModel.field16 + "%'AND CV.CustomFieldID = 16 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field16 is " + EmailModel.field16 + " and ";
                }
                if (!string.IsNullOrEmpty(EmailModel.field17))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + EmailModel.field17 + "%'AND CV.CustomFieldID = 17 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field17 is " + EmailModel.field17 + " and ";
                }
                if (!string.IsNullOrEmpty(EmailModel.field18))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + EmailModel.field18 + "%'AND CV.CustomFieldID = 18 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field18 is " + EmailModel.field18 + " and ";
                }
                if (!string.IsNullOrEmpty(EmailModel.field19))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + EmailModel.field19 + "%'AND CV.CustomFieldID = 19 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field19 is " + EmailModel.field19 + " and ";
                }
                if (!string.IsNullOrEmpty(EmailModel.field20))
                {
                    WhereClause += "TData.ReceiptTicketID = (SELECT ReceiptTicketID	FROM CustomValue CV	WHERE CV.CustomValue LIKE '%" + EmailModel.field20 + "%'AND CV.CustomFieldID = 20 AND CV.ReceiptTicketID = TData.ReceiptTicketID) AND ";
                    strFilterstring += "Field20 is " + EmailModel.field20 + " and ";
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
            Uri uri = new Uri("/View/SettingView/EmailScheduleList_Page.Xaml", UriKind.Relative);
            ParentPage.NavigationService.Navigate(uri);
        }
        public void Dispose()
        {
            dataRepository.Context.Dispose();
        }
    }
    public class EmailModel : INotifyPropertyChanged
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
    public class SceheduleTime : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _Hour;
        public int Hour
        {
            get { return _Hour; }
            set { _Hour = value; OnPropertyChanged(); }
        }

        private int _Date;
        public int Date
        {
            get { return _Date; }
            set { _Date = value; OnPropertyChanged(); }
        }

        private int _Month;
        public int Month
        {
            get { return _Month; }
            set { _Month = value; OnPropertyChanged(); }
        }

        private string _Day;
        public string Day
        {
            get { return _Day; }
            set { _Day = value; OnPropertyChanged(); }
        }
    }
}
