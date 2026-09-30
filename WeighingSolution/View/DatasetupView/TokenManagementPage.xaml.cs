using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using WeighingSolution.DataLayer;
using WeighingSolution.Events;
using WeighingSolution.Helpers;
using WeighingSolution.SharedComponents;
using WeighingSolution.UserControls;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for TokenManagementPage.xaml
    /// </summary>
    public partial class TokenManagementPage : Page
    {
        vmTokenManagementPage viewModel;
        public int IntTokenID = 0;

        public string selectedVehicle = string.Empty;
        public string selectedTransaporter = string.Empty;
        public string selectedAccount = string.Empty;
        public string selectedDriver = string.Empty;
        public string selectedProduct = string.Empty;
        public string selectedGate = string.Empty;
        public int LargeDataSearchLength = 2;
        public int SmallDataSearchLength = 1;

        private void SetupPage()
        {
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            viewModel = new vmTokenManagementPage(IntTokenID);
            this.DataContext = viewModel;
            visiblecustomefield();
        }
        public TokenManagementPage()
        {
            InitializeComponent();
            SetupPage();
        }
        public TokenManagementPage(int tokenid)
        {
            InitializeComponent();
            IntTokenID = tokenid;
            SetupPage();
        }
        public void visiblecustomefield()
        {
            try
            {
                //List<CustomFieldModel> LstCustomField = viewModel.dataRepository.Context.CustomFields.ToList();
                List<CustomFieldModel> LstCustomField = Get_CustomFields();

                if (LstCustomField.Count > 0)
                {
                    if (LstCustomField.Where(p => p.FieldName == "Field1").Count() > 0)
                    {
                        CustomFieldModel objCF1 = LstCustomField.FirstOrDefault(x => x.FieldName == "Field1");
                        lblField1.Content = objCF1.DisplayName;
                        List<string> lstCustomValue1 = viewModel.dataRepository.Context.Database.SqlQuery<string>("select distinct(CustomValue1) from TransactionData where CustomValue1 <> '' ").ToList();
                        viewModel.CustomFields1 = new ObservableCollection<string>(lstCustomValue1.ToList());

                        lblField1.Visibility = Visibility.Visible;
                        txtField1.Visibility = Visibility.Visible;
                    }

                    if (LstCustomField.Where(p => p.FieldName == "Field2").Count() > 0)
                    {
                        CustomFieldModel objCF2 = LstCustomField.FirstOrDefault(x => x.FieldName == "Field2");
                        lblField2.Content = objCF2.DisplayName;
                        List<string> lstCustomValue2 = viewModel.dataRepository.Context.Database.SqlQuery<string>("select distinct(CustomValue2) from TransactionData where CustomValue2 <> '' ").ToList();
                        viewModel.CustomFields2 = new ObservableCollection<string>(lstCustomValue2.ToList());

                        lblField2.Visibility = Visibility.Visible;
                        txtField2.Visibility = Visibility.Visible;
                    }

                    if (LstCustomField.Where(p => p.FieldName == "Field3").Count() > 0)
                    {
                        CustomFieldModel objCF3 = LstCustomField.FirstOrDefault(x => x.FieldName == "Field3");
                        lblField3.Content = objCF3.DisplayName;
                        List<string> lstCustomValue3 = viewModel.dataRepository.Context.Database.SqlQuery<string>("select distinct(CustomValue3) from TransactionData where CustomValue3 <> '' ").ToList();
                        viewModel.CustomFields3 = new ObservableCollection<string>(lstCustomValue3.ToList());

                        lblField3.Visibility = Visibility.Visible;
                        txtField3.Visibility = Visibility.Visible;
                    }

                    if (LstCustomField.Where(p => p.FieldName == "Field4").Count() > 0)
                    {
                        CustomFieldModel objCF4 = LstCustomField.FirstOrDefault(x => x.FieldName == "Field4");
                        lblField4.Content = objCF4.DisplayName;
                        List<string> lstCustomValue4 = viewModel.dataRepository.Context.Database.SqlQuery<string>("select distinct(CustomValue4) from TransactionData where CustomValue4 <> '' ").ToList();
                        viewModel.CustomFields4 = new ObservableCollection<string>(lstCustomValue4.ToList());


                        lblField4.Visibility = Visibility.Visible;
                        txtField4.Visibility = Visibility.Visible;
                    }

                    if (LstCustomField.Where(p => p.FieldName == "Field5").Count() > 0)
                    {
                        CustomFieldModel objCF5 = LstCustomField.FirstOrDefault(x => x.FieldName == "Field5");
                        lblField5.Content = objCF5.DisplayName;
                        List<string> lstCustomValue5 = viewModel.dataRepository.Context.Database.SqlQuery<string>("select distinct(CustomValue5) from TransactionData where CustomValue5 <> '' ").ToList();
                        viewModel.CustomFields5 = new ObservableCollection<string>(lstCustomValue5.ToList());

                        lblField5.Visibility = Visibility.Visible;
                        txtField5.Visibility = Visibility.Visible;
                    }

                    if (LstCustomField.Where(p => p.FieldName == "Field6").Count() > 0)
                    {
                        CustomFieldModel objCF6 = LstCustomField.FirstOrDefault(x => x.FieldName == "Field6");
                        lblField6.Content = objCF6.DisplayName;
                        List<string> lstCustomValue6 = viewModel.dataRepository.Context.Database.SqlQuery<string>("select distinct(CustomValue6) from TransactionData where CustomValue6 <> '' ").ToList();
                        viewModel.CustomFields6 = new ObservableCollection<string>(lstCustomValue6.ToList());

                        lblField6.Visibility = Visibility.Visible;
                        txtField6.Visibility = Visibility.Visible;
                    }

                    if (LstCustomField.Where(p => p.FieldName == "Field7").Count() > 0)
                    {
                        CustomFieldModel objCF7 = LstCustomField.FirstOrDefault(x => x.FieldName == "Field7");
                        lblField7.Content = objCF7.DisplayName;
                        List<string> lstCustomValue7 = viewModel.dataRepository.Context.Database.SqlQuery<string>("select distinct(CustomValue7) from TransactionData where CustomValue7 <> '' ").ToList();
                        viewModel.CustomFields7 = new ObservableCollection<string>(lstCustomValue7.ToList());
                        lblField7.Visibility = Visibility.Visible;
                        txtField7.Visibility = Visibility.Visible;
                    }

                    if (LstCustomField.Where(p => p.FieldName == "Field8").Count() > 0)
                    {
                        CustomFieldModel objCF8 = LstCustomField.FirstOrDefault(x => x.FieldName == "Field8");
                        lblField8.Content = objCF8.DisplayName;
                        List<string> lstCustomValue8 = viewModel.dataRepository.Context.Database.SqlQuery<string>("select distinct(CustomValue8) from TransactionData where CustomValue8 <> '' ").ToList();
                        viewModel.CustomFields8 = new ObservableCollection<string>(lstCustomValue8.ToList());
                        lblField8.Visibility = Visibility.Visible;
                        txtField8.Visibility = Visibility.Visible;
                    }

                    if (LstCustomField.Where(p => p.FieldName == "Field9").Count() > 0)
                    {
                        CustomFieldModel objCF9 = LstCustomField.FirstOrDefault(x => x.FieldName == "Field9");
                        lblField9.Content = objCF9.DisplayName;
                        List<string> lstCustomValue9 = viewModel.dataRepository.Context.Database.SqlQuery<string>("select  distinct(CustomValue9) from TransactionData where CustomValue9 <> '' ").ToList();
                        viewModel.CustomFields9 = new ObservableCollection<string>(lstCustomValue9.ToList());
                        lblField9.Visibility = Visibility.Visible;
                        txtField9.Visibility = Visibility.Visible;
                    }

                    if (LstCustomField.Where(p => p.FieldName == "Field10").Count() > 0)
                    {
                        CustomFieldModel objCF10 = LstCustomField.FirstOrDefault(x => x.FieldName == "Field10");
                        lblField10.Content = objCF10.DisplayName;
                        List<string> lstCustomValue10 = viewModel.dataRepository.Context.Database.SqlQuery<string>("select distinct(CustomValue10) from TransactionData where CustomValue10 <> '' ").ToList();
                        viewModel.CustomFields10 = new ObservableCollection<string>(lstCustomValue10.ToList());
                        lblField10.Visibility = Visibility.Visible;
                        txtField10.Visibility = Visibility.Visible;
                    }

                    if (LstCustomField.Where(p => p.FieldName == "Field11").Count() > 0)
                    {
                        CustomFieldModel objCF11 = LstCustomField.FirstOrDefault(x => x.FieldName == "Field11");
                        lblField11.Content = objCF11.DisplayName;
                        List<string> lstCustomValue11 = viewModel.dataRepository.Context.Database.SqlQuery<string>("select  distinct(CustomValue11) from TransactionData where CustomValue11 <> '' ").ToList();
                        viewModel.CustomFields11 = new ObservableCollection<string>(lstCustomValue11.ToList());

                        lblField11.Visibility = Visibility.Visible;
                        txtField11.Visibility = Visibility.Visible;
                    }

                    if (LstCustomField.Where(p => p.FieldName == "Field12").Count() > 0)
                    {
                        CustomFieldModel objCF12 = LstCustomField.FirstOrDefault(x => x.FieldName == "Field12");

                        lblField12.Content = objCF12.DisplayName;
                        List<string> lstCustomValue12 = viewModel.dataRepository.Context.Database.SqlQuery<string>("select distinct(CustomValue12) from TransactionData where CustomValue12 <> '' ").ToList();
                        viewModel.CustomFields12 = new ObservableCollection<string>(lstCustomValue12.ToList());
                        lblField12.Visibility = Visibility.Visible;
                        txtField12.Visibility = Visibility.Visible;
                    }

                    if (LstCustomField.Where(p => p.FieldName == "Field13").Count() > 0)
                    {
                        CustomFieldModel objCF13 = LstCustomField.FirstOrDefault(x => x.FieldName == "Field13");
                        lblField13.Content = objCF13.DisplayName;
                        List<string> lstCustomValue13 = viewModel.dataRepository.Context.Database.SqlQuery<string>("select distinct(CustomValue13) from TransactionData where CustomValue13 <> '' ").ToList();
                        viewModel.CustomFields13 = new ObservableCollection<string>(lstCustomValue13.ToList());

                        lblField13.Visibility = Visibility.Visible;
                        txtField13.Visibility = Visibility.Visible;
                    }

                    if (LstCustomField.Where(p => p.FieldName == "Field14").Count() > 0)
                    {
                        CustomFieldModel objCF14 = LstCustomField.FirstOrDefault(x => x.FieldName == "Field14");

                        lblField14.Content = objCF14.DisplayName;
                        List<string> lstCustomValue14 = viewModel.dataRepository.Context.Database.SqlQuery<string>("select distinct(CustomValue14) from TransactionData where CustomValue14 <> '' ").ToList();
                        viewModel.CustomFields14 = new ObservableCollection<string>(lstCustomValue14.ToList());

                        lblField14.Visibility = Visibility.Visible;
                        txtField14.Visibility = Visibility.Visible;
                    }

                    if (LstCustomField.Where(p => p.FieldName == "Field15").Count() > 0)
                    {
                        CustomFieldModel objCF15 = LstCustomField.FirstOrDefault(x => x.FieldName == "Field15");
                        lblField15.Content = objCF15.DisplayName;
                        List<string> lstCustomValue15 = viewModel.dataRepository.Context.Database.SqlQuery<string>("select distinct(CustomValue15) from TransactionData where CustomValue15 <> '' ").ToList();
                        viewModel.CustomFields15 = new ObservableCollection<string>(lstCustomValue15.ToList());
                        lblField15.Visibility = Visibility.Visible;
                        txtField15.Visibility = Visibility.Visible;
                    }

                    if (LstCustomField.Where(p => p.FieldName == "Field16").Count() > 0)
                    {

                        CustomFieldModel objCF16 = LstCustomField.FirstOrDefault(x => x.FieldName == "Field16");
                        lblField16.Content = objCF16.DisplayName;
                        List<string> lstCustomValue16 = viewModel.dataRepository.Context.Database.SqlQuery<string>("select distinct(CustomValue16) from TransactionData where CustomValue16 <> '' ").ToList();
                        viewModel.CustomFields16 = new ObservableCollection<string>(lstCustomValue16.ToList());
                        lblField16.Visibility = Visibility.Visible;
                        txtField16.Visibility = Visibility.Visible;
                    }

                    if (LstCustomField.Where(p => p.FieldName == "Field17").Count() > 0)
                    {
                        CustomFieldModel objCF17 = LstCustomField.FirstOrDefault(x => x.FieldName == "Field17");

                        lblField17.Content = objCF17.DisplayName;
                        List<string> lstCustomValue17 = viewModel.dataRepository.Context.Database.SqlQuery<string>("select distinct(CustomValue17) from TransactionData where CustomValue17 <> '' ").ToList();

                        viewModel.CustomFields17 = new ObservableCollection<string>(lstCustomValue17.ToList());
                        lblField17.Visibility = Visibility.Visible;
                        txtField17.Visibility = Visibility.Visible;
                    }

                    if (LstCustomField.Where(p => p.FieldName == "Field18").Count() > 0)
                    {
                        CustomFieldModel objCF18 = LstCustomField.FirstOrDefault(x => x.FieldName == "Field18");

                        lblField18.Content = objCF18.DisplayName;
                        List<string> lstCustomValue18 = viewModel.dataRepository.Context.Database.SqlQuery<string>("select distinct(CustomValue18) from TransactionData where CustomValue18 <> '' ").ToList();
                        viewModel.CustomFields18 = new ObservableCollection<string>(lstCustomValue18.ToList());
                        lblField18.Visibility = Visibility.Visible;
                        txtField18.Visibility = Visibility.Visible;
                    }

                    if (LstCustomField.Where(p => p.FieldName == "Field19").Count() > 0)
                    {
                        CustomFieldModel objCF19 = LstCustomField.FirstOrDefault(x => x.FieldName == "Field19");

                        lblField19.Content = objCF19.DisplayName;
                        List<string> lstCustomValue19 = viewModel.dataRepository.Context.Database.SqlQuery<string>("select distinct(CustomValue19) from TransactionData where CustomValue19 <> '' ").ToList();
                        viewModel.CustomFields19 = new ObservableCollection<string>(lstCustomValue19.ToList());
                        lblField19.Visibility = Visibility.Visible;
                        txtField19.Visibility = Visibility.Visible;
                    }

                    if (LstCustomField.Where(p => p.FieldName == "Field20").Count() > 0)
                    {
                        CustomFieldModel objCF20 = LstCustomField.FirstOrDefault(x => x.FieldName == "Field20");

                        lblField20.Content = objCF20.DisplayName;
                        List<string> lstCustomValue20 = viewModel.dataRepository.Context.Database.SqlQuery<string>("select distinct(CustomValue20) from TransactionData where CustomValue20 <> '' ").ToList();
                        viewModel.CustomFields20 = new ObservableCollection<string>(lstCustomValue20.ToList());
                        lblField20.Visibility = Visibility.Visible;
                        txtField20.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.PluginFileLocation + @"\Tokenmanagmenterror.txt", json);
            }
        }
        public List<CustomFieldModel> Get_CustomFields()
        {
            try
            {
                List<CustomFieldModel> customFieldModels = new List<CustomFieldModel>();
                DataTable dt = DatabaseFunctions.LocalMSSQLExecuteReaderQuery("select * from  CustomField where IsVisible = 1");
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int CustomFieldID = dt.Rows[i].Field<int>("CustomFieldID");
                        string FieldName = dt.Rows[i].Field<string>("FieldName");
                        string DisplayName = dt.Rows[i].Field<string>("DisplayName");

                        customFieldModels.Add(new CustomFieldModel { CustomFieldID = CustomFieldID, FieldName = FieldName, DisplayName = DisplayName });
                    }
                }

                return customFieldModels;
            }
            catch (Exception ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.PluginFileLocation + @"\GetTokenmanagmenterror.txt", json);
                return new List<CustomFieldModel>();
            }

        }

        private async void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();
            viewModel.EventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(GoBack);
            viewModel.EventAggregator.GetEvent<AfterDetailSaveCancelledEvent>().Subscribe(GoBack);

            try
            {
                UhfData.Text = Constant.RFID;
            }
            catch
            {

            }

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 2);
            dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                string RFID_Tag = Constant.RFID;
                if (!string.IsNullOrEmpty(RFID_Tag))
                {
                    UhfData.Text = RFID_Tag;
                }
                else
                {
                    UhfData.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                string DataNotFound6 = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.PluginFileLocation + @"\TokenDisp_opencomError.txt", DataNotFound6);
            }
        }

        private void BTNCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(UhfData.Text))
                {
                    viewModel.Model.TokenValue = UhfData.Text;
                }
            }
            catch { }
        }

        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.EventAggregator.GetEvent<AfterDetailSavedEvent>().Unsubscribe(GoBack);
            viewModel.EventAggregator.GetEvent<AfterDetailSaveCancelledEvent>().Unsubscribe(GoBack);
            viewModel.Dispose();
        }
        private void GoBack(object obj)
        {
            Uri uri = new Uri("/View/DatasetupView/TokenManagementList.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        #region cmbVehicalNo
        private void cmbVehicalNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var abc = e.Source as AutoCompleteBox;

            if (abc != null && !abc.IsKeyboardFocused)
            {
                if (abc.SelectedItem != null)
                {
                    cmbVehicalNo.Text = abc.SelectedItem.ToString();
                    selectedVehicle = abc.SelectedItem.ToString();
                }
            }
        }
        private async void cmbVehicalNo_TextChanged(object sender, RoutedEventArgs e)
        {
            var abc = e.OriginalSource as AutoCompleteBoxEx;
            if (abc != null)
            {
                if (!string.IsNullOrEmpty(abc.Text))
                {
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (selectedVehicle == abc.Text)
                            return;

                        try
                        {
                            if (!string.IsNullOrEmpty(abc.Text))
                            {
                                if (abc.Text.Length >= LargeDataSearchLength)
                                {
                                    var _Vehicles = new ObservableCollection<VehicleBindingModel>(viewModel.dataRepository.Context.Vehicles.Where(x => x.VehicleNumber.Contains(abc.Text) && x.IsActive).Take(15).Select(x => new VehicleBindingModel { VehicleID = x.VehicleID, VehicleNumber = x.VehicleNumber, TareWeight = x.TareWeight }).ToList());
                                    viewModel.Autofillvehicles = new ObservableCollection<string>(_Vehicles.OrderBy(P => P.VehicleNumber).Select(p => p.VehicleNumber).Distinct().ToList());
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = ex.Message;
                        }
                    }));
                }
            }
        }
        private async void cmbVehicalNo_LostFocus(object sender, RoutedEventArgs e)
        {
            var abc = e.Source as AutoCompleteBox;
            //var abc = e.OriginalSource as TextBox;          

            if (abc != null && !abc.IsKeyboardFocused)
            {
                if (abc.IsDropDownOpen)
                    return;

                await Dispatcher.BeginInvoke(new Action(async () =>
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(viewModel.VehicleNumberText))
                        {
                            string NumberText = viewModel.VehicleNumberText.Trim();
                            NumberText = NumberText.ToUpper();
                            NumberText = NumberText.Replace(" ", "");
                            if (Common.DisableSpecialCharactersInVehicle)
                                NumberText = Regex.Replace(NumberText, @"[^0-9a-zA-Z]+", "");

                            viewModel.VehicleNumberText = NumberText;

                            var _Vehicles = viewModel.Vehicles.Where(p => string.Equals(p.VehicleNumber, viewModel.VehicleNumberText, StringComparison.CurrentCultureIgnoreCase));
                            if (_Vehicles.Count() > 0)
                            {
                                var vehicle = _Vehicles.FirstOrDefault();
                                viewModel.Model.VehicleID = vehicle.VehicleID;
                                viewModel.Model.Vehicle_Number = vehicle.VehicleNumber;
                                viewModel.Model.Vehicle = viewModel.dataRepository.Context.Vehicles.Where(p => p.VehicleID == vehicle.VehicleID).FirstOrDefault();
                            }
                            else
                            {
                                var _Vehicles1 = viewModel.Vehicles.Where(p => string.Equals(p.VehicleNumber, NumberText, StringComparison.CurrentCultureIgnoreCase));
                                if (_Vehicles1.Count() > 0)
                                {
                                    var vehicle = _Vehicles1.FirstOrDefault();
                                    viewModel.Model.VehicleID = vehicle.VehicleID;
                                    viewModel.Model.Vehicle_Number = vehicle.VehicleNumber;
                                    viewModel.Model.Vehicle = viewModel.dataRepository.Context.Vehicles.Where(p => p.VehicleID == vehicle.VehicleID).FirstOrDefault();
                                }
                                else
                                {
                                    if (Common.ValidatePageRequest(PageEntity.Vehicle, PageAccess.Create))
                                    {
                                        MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Vehicle is not exist in Master Data", "Do you wish to add Vehicle?");
                                        if (msgbox_result == MessageDialogResult.Affirmative)
                                        {
                                            viewModel.OnVehicleSave(viewModel.VehicleNumberText);
                                            VehicleBindingModel vehicle = viewModel.GetVehiclebyVehicleNumber(viewModel.VehicleNumberText);
                                            viewModel.Vehicles.Add(vehicle);
                                            viewModel.Model.VehicleID = vehicle.VehicleID;
                                            viewModel.Model.Vehicle_Number = vehicle.VehicleNumber;
                                        }
                                        else if (msgbox_result == MessageDialogResult.Negative)
                                        {
                                            viewModel.Model.VehicleID = null;
                                            viewModel.VehicleNumberText = string.Empty;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        await MessageBoxHelper.ShowInfoDialogAsync("Vehicle is not exist in Master Data", "Please Contact Admin to Add Vehicle.");
                                        viewModel.VehicleNumberText = string.Empty;
                                        return;
                                    }

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _ = ex.Message;
                    }
                }));
            }
        }
        private void cmbVehicalNo_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.Tab)
                {
                    e.Handled = false;
                }
                else if (Common.VehicleNumberLength <= cmbVehicalNo.Text.Length)
                {
                    e.Handled = true;
                }
                else if (Common.DisableSpecialCharactersInVehicle)
                {
                    var regex = new Regex(@"[^a-zA-Z0-9]");
                    char val = Common.GetCharFromKey(e.Key);
                    if (regex.IsMatch(val.ToString()))
                    {
                        e.Handled = true;
                    }
                }
                else
                {

                }
            }
            catch { }

        }

        #endregion cmbVehicalNo

        #region cmbTransaporter
        private void cmbTransaporter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var abc = e.Source as AutoCompleteBox;

            if (abc != null && !abc.IsKeyboardFocused)
            {
                if (abc.SelectedItem != null)
                {
                    cmbTransaporter.Text = abc.SelectedItem.ToString();
                    selectedTransaporter = abc.SelectedItem.ToString();
                }
            }
        }
        private async void cmbTransaporter_TextChanged(object sender, RoutedEventArgs e)
        {
            var abc = e.OriginalSource as AutoCompleteBoxEx;
            if (abc != null)
            {
                if (!string.IsNullOrEmpty(abc.Text))
                {
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (selectedTransaporter == abc.Text)
                            return;

                        try
                        {
                            if (!string.IsNullOrEmpty(abc.Text))
                            {
                                if (abc.Text.Length >= LargeDataSearchLength)
                                {
                                    var _Transporters = new ObservableCollection<Account>(viewModel.dataRepository.Context.Accounts.Where(x => x.CompanyName.Contains(abc.Text) && x.IsTransporter != null && x.IsTransporter.Value && x.Active == true).Take(15).ToList());
                                    viewModel.AutofillTransporters = new ObservableCollection<string>(_Transporters.OrderBy(P => P.CompanyName).Select(p => p.CompanyName).Distinct().ToList());
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = ex.Message;
                        }
                    }));
                }
            }
        }
        private async void cmbTransaporter_LostFocus(object sender, RoutedEventArgs e)
        {
            var abc = e.OriginalSource as TextBox;
            if (abc != null && !abc.IsKeyboardFocused)
            {
                await Dispatcher.BeginInvoke(new Action(async () =>
                {
                    if (!string.IsNullOrEmpty(viewModel.TransaporterNameText))
                    {
                        viewModel.TransaporterNameText = viewModel.TransaporterNameText.Trim();
                        var _Transporters = viewModel.Transporters.Where(p => p.CompanyName.ToUpper() == viewModel.TransaporterNameText.ToUpper());
                        if (_Transporters.Count() > 0)
                        {
                            var Transporter = _Transporters.FirstOrDefault();
                            viewModel.Model.TransporterID = Transporter.AccountID;
                            viewModel.Model.Transporter_Name = Transporter.CompanyName;
                            viewModel.Model.Account1 = viewModel.dataRepository.Context.Accounts.Where(p => p.AccountID == Transporter.AccountID).FirstOrDefault();
                        }
                        else
                        {
                            if (Common.ValidatePageRequest(PageEntity.Account, PageAccess.Create))
                            {
                                var account = viewModel.Transporters.FirstOrDefault(x => x.CompanyName.ToUpper() == viewModel.TransaporterNameText.ToUpper());
                                if (account == null)
                                {
                                    string query = $"INSERT INTO [dbo].[Account]([CompanyName],[FirstName],[IsAccount],[IsTransporter],[Active]) VALUES ('{viewModel.TransaporterNameText}', '{viewModel.TransaporterNameText}', 0, 1, 1)";
                                    bool flag = DatabaseFunctions.ExecuteQuery(query);
                                    if (flag)
                                    {
                                        AccountBindingModel transpoter = viewModel.GetTraspoterbyCompanyName(viewModel.TransaporterNameText);
                                        viewModel.Transporters.Add(transpoter);
                                        viewModel.Model.TransporterID = transpoter.AccountID;
                                        viewModel.Model.Transporter_Name = transpoter.CompanyName;
                                        ToastHelper.ShowSuccess($"Transporter {viewModel.TransaporterNameText} save Successfully");
                                    }
                                }
                                else
                                {
                                    string query = $"UPDATE [dbo].[Account] SET [IsTransporter] = 1 WHERE CompanyName = '{viewModel.TransaporterNameText}'";
                                    bool flag = DatabaseFunctions.ExecuteQuery(query);
                                    if (flag)
                                    {
                                        AccountBindingModel transpoter = viewModel.GetTraspoterbyCompanyName(viewModel.TransaporterNameText);
                                        viewModel.Transporters.Add(transpoter);
                                        viewModel.Model.TransporterID = transpoter.AccountID;
                                        viewModel.Model.Transporter_Name = transpoter.CompanyName;
                                        ToastHelper.ShowSuccess($"Transporter {viewModel.TransaporterNameText} save Successfully");
                                    }
                                }
                            }
                            else
                            {
                                await MessageBoxHelper.ShowInfoDialogAsync("Transporter is not exist in Master Data", "Please Contact Admin to Add Transporter.");
                                viewModel.TransaporterNameText = string.Empty;
                                return;
                            }

                            //await MessageBoxHelper.ShowInfoDialogAsync("Transporter is not exist in Master Data", "Please Add Transporter in Master.");
                            //viewModel.TransaporterNameText = string.Empty;
                            //return;
                        }
                    }
                }));
            }
        }

        #endregion cmbTransaporter

        #region cmbAccount
        private void cmbAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var abc = e.Source as AutoCompleteBox;

            if (abc != null && !abc.IsKeyboardFocused)
            {
                if (abc.SelectedItem != null)
                {
                    cmbAccount.Text = abc.SelectedItem.ToString();
                    selectedAccount = abc.SelectedItem.ToString();
                }
            }
        }
        private async void cmbAccount_TextChanged(object sender, RoutedEventArgs e)
        {
            var abc = e.OriginalSource as AutoCompleteBoxEx;
            if (abc != null)
            {
                if (!string.IsNullOrEmpty(abc.Text))
                {
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (selectedAccount == abc.Text)
                            return;

                        try
                        {
                            if (!string.IsNullOrEmpty(abc.Text))
                            {
                                if (abc.Text.Length >= LargeDataSearchLength)
                                {
                                    var _Accounts = new ObservableCollection<Account>(viewModel.dataRepository.Context.Accounts.Where(x => x.CompanyName.Contains(abc.Text) && x.IsAccount != null && x.IsAccount.Value && x.Active == true).Take(15).ToList());
                                    viewModel.AutofillAccounts = new ObservableCollection<string>(_Accounts.OrderBy(P => P.CompanyName).Select(p => p.CompanyName).Distinct().ToList());
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = ex.Message;
                        }
                    }));
                }
            }
        }
        private async void cmbAccount_LostFocus(object sender, RoutedEventArgs e)
        {
            var abc = e.OriginalSource as TextBox;
            if (abc != null && !abc.IsKeyboardFocused)
            {
                await Dispatcher.BeginInvoke(new Action(async () =>
                {
                    if (!string.IsNullOrEmpty(viewModel.AccountNameText))
                    {
                        viewModel.AccountNameText = viewModel.AccountNameText.Trim();
                        var _Accounts = viewModel.Accounts.Where(p => p.CompanyName.ToUpper() == viewModel.AccountNameText.ToUpper());
                        if (_Accounts.Count() > 0)
                        {
                            var Account = _Accounts.FirstOrDefault();
                            viewModel.Model.AccountID = Account.AccountID;
                            viewModel.Model.Account_Name = Account.CompanyName;
                            viewModel.Model.Account = viewModel.dataRepository.Context.Accounts.Where(p => p.AccountID == Account.AccountID).FirstOrDefault();
                        }
                        else
                        {
                            //await MessageBoxHelper.ShowInfoDialogAsync("Account is not exist in Master Data", "Please Add Account in Master.");
                            //viewModel.AccountNameText = string.Empty;
                            //return;


                            if (Common.ValidatePageRequest(PageEntity.Account, PageAccess.Create))
                            {

                                var transporter = viewModel.Accounts?.FirstOrDefault(x => x.CompanyName.ToUpper() == viewModel.AccountNameText.ToUpper());
                                if (transporter == null)
                                {

                                    string query = $"INSERT INTO [dbo].[Account]([CompanyName],[FirstName],[IsAccount],[IsTransporter],[Active]) VALUES ('{viewModel.AccountNameText}', '{viewModel.AccountNameText}', 1, 0, 1)";
                                    bool flag = DatabaseFunctions.ExecuteQuery(query);
                                    if (flag)
                                    {
                                        AccountBindingModel transpoter = viewModel.GetAccountbyCompanyName(viewModel.AccountNameText);
                                        viewModel.Accounts.Add(transpoter);
                                        viewModel.Model.AccountID = transpoter.AccountID;
                                        viewModel.Model.Account_Name = transpoter.CompanyName;
                                        ToastHelper.ShowSuccess($"Account {viewModel.TransaporterNameText} save Successfully");
                                    }
                                }
                                else
                                {
                                    string query = $"UPDATE [dbo].[Account] SET [IsAccount] = 1 WHERE CompanyName = '{viewModel.AccountNameText}'";
                                    bool flag = DatabaseFunctions.ExecuteQuery(query);
                                    if (flag)
                                    {
                                        AccountBindingModel transpoter = viewModel.GetAccountbyCompanyName(viewModel.AccountNameText);
                                        viewModel.Accounts.Add(transpoter);
                                        viewModel.Model.AccountID = transpoter.AccountID;
                                        viewModel.Model.Account_Name = transpoter.CompanyName;
                                        ToastHelper.ShowSuccess($"Account {viewModel.TransaporterNameText} save Successfully");
                                    }
                                }


                            }
                            else
                            {
                                await MessageBoxHelper.ShowInfoDialogAsync("Account is not exist in Master Data", "Please Contact Admin to Add Account.");
                                viewModel.AccountNameText = string.Empty;
                                return;
                            }
                        }
                    }

                }));
            }
        }

        #endregion cmbAccount

        #region cmbDriver  
        private void cmbDriver_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var abc = e.OriginalSource as AutoCompleteBoxEx;
            if (abc != null && !abc.IsKeyboardFocused)
            {
                if (abc.SelectedItem != null)
                {
                    cmbDriver.Text = abc.SelectedItem.ToString();
                    selectedDriver = abc.SelectedItem.ToString();
                }
            }
        }
        private async void cmbDriver_TextChanged(object sender, RoutedEventArgs e)
        {
            var abc = e.Source as AutoCompleteBox;
            if (abc != null)
            {
                if (!string.IsNullOrEmpty(abc.Text))
                {
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (selectedDriver == abc.Text)
                            return;

                        try
                        {
                            if (!string.IsNullOrEmpty(abc.Text))
                            {
                                if (abc.Text.Length >= LargeDataSearchLength)
                                {
                                    var _Drivers = new ObservableCollection<DriverBindingModel>(viewModel.dataRepository.Context.Drivers.Where(x => (x.FirstName.Contains(abc.Text) || x.LastName.Contains(abc.Text)) && x.Active).Take(15).Select(x => new DriverBindingModel { DriverID = x.DriverID, FirstName = x.FirstName, LastName = x.LastName }).ToList());
                                    viewModel.AutofillDrivers = new ObservableCollection<string>(_Drivers.OrderBy(P => P.DriverName).Select(p => p.DriverName).Distinct().ToList());
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = ex.Message;
                        }
                    }));
                }
            }
        }
        private async void cmbDriver_LostFocus(object sender, RoutedEventArgs e)
        {
            var abc = e.OriginalSource as TextBox;
            if (abc != null && !abc.IsKeyboardFocused)
            {
                await Dispatcher.BeginInvoke(new Action(async () =>
                {
                    if (!string.IsNullOrEmpty(viewModel.DriverNameText))
                    {
                        viewModel.DriverNameText = viewModel.DriverNameText.Trim();
                        var _Drivers = viewModel.Drivers.Where(p => p.DriverName.ToUpper() == viewModel.DriverNameText.ToUpper());
                        if (_Drivers.Count() > 0)
                        {
                            var Driver = _Drivers.FirstOrDefault();
                            viewModel.Model.DriverID = Driver.DriverID;
                            viewModel.Model.Driver_Name = Driver.DriverName;
                            viewModel.Model.Driver = viewModel.dataRepository.Context.Drivers.Where(p => p.DriverID == Driver.DriverID).FirstOrDefault();
                        }
                        else
                        {
                            if (Common.ValidatePageRequest(PageEntity.Driver, PageAccess.Create))
                            {
                                string query = $"INSERT INTO [dbo].[Driver] ([FirstName],[Active]) VALUES ('{viewModel.DriverNameText}', 1)";
                                bool flag = DatabaseFunctions.ExecuteQuery(query);
                                if (flag)
                                {
                                    DriverBindingModel transpoter = viewModel.GetDriverbyDriverName(viewModel.DriverNameText);
                                    viewModel.Drivers.Add(transpoter);
                                    viewModel.Model.DriverID = transpoter.DriverID;
                                    viewModel.Model.Driver_Name = transpoter.DriverName;
                                    ToastHelper.ShowSuccess($"Driver {viewModel.DriverNameText} save Successfully");
                                }
                            }
                            else
                            {
                                await MessageBoxHelper.ShowInfoDialogAsync("Driver is not exist in Master Data", "Please Contact Admin to Add Driver.");
                                viewModel.DriverNameText = string.Empty;
                                return;
                            }
                        }
                    }
                }));
            }
        }
        #endregion cmbDriver

        #region cmbProduct
        private void cmbProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var abc = e.Source as AutoCompleteBox;

            if (abc != null && !abc.IsKeyboardFocused)
            {
                if (abc.SelectedItem != null)
                {
                    cmbProduct.Text = abc.SelectedItem.ToString();
                    selectedProduct = abc.SelectedItem.ToString();
                }
            }
        }
        private async void cmbProduct_TextChanged(object sender, RoutedEventArgs e)
        {
            var abc = e.OriginalSource as AutoCompleteBoxEx;
            if (abc != null)
            {
                if (!string.IsNullOrEmpty(abc.Text))
                {
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (selectedProduct == abc.Text)
                            return;

                        try
                        {
                            if (!string.IsNullOrEmpty(abc.Text))
                            {
                                if (abc.Text.Length >= LargeDataSearchLength)
                                {
                                    var _Products = new ObservableCollection<ProductBindingModel>(viewModel.dataRepository.Context.Products.Where(x => x.ProductName.Contains(abc.Text) && x.IsActive).Select(x => new ProductBindingModel { ProductID = x.ProductID, ProductName = x.ProductName }).ToList());
                                    viewModel.AutofillProducts = new ObservableCollection<string>(_Products.OrderBy(P => P.ProductName).Select(p => p.ProductName).Distinct().ToList());
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = ex.Message;
                        }
                    }));
                }
            }
        }
        private async void cmbProduct_LostFocus(object sender, RoutedEventArgs e)
        {
            var abc = e.OriginalSource as TextBox;
            if (abc != null && !abc.IsKeyboardFocused)
            {

                await Dispatcher.BeginInvoke(new Action(async () =>
                {
                    if (!string.IsNullOrEmpty(viewModel.ProductNameText))
                    {
                        viewModel.ProductNameText = viewModel.ProductNameText.Trim();
                        var _Products = viewModel.Products.Where(p => p.ProductName.ToUpper() == viewModel.ProductNameText.ToUpper());
                        if (_Products.Count() > 0)
                        {
                            var Product = _Products.FirstOrDefault();
                            viewModel.Model.ProductID = Product.ProductID;
                            viewModel.Model.Product_Name = Product.ProductName;
                            viewModel.Model.Product = viewModel.dataRepository.Context.Products.Where(p => p.ProductID == Product.ProductID).FirstOrDefault();
                        }
                        else
                        {
                            if (Common.ValidatePageRequest(PageEntity.Product, PageAccess.Create))
                            {
                                string query = $"INSERT INTO [dbo].[Product] ([ProductName],[IsActive]) VALUES ('{viewModel.ProductNameText}', 1)";
                                bool flag = DatabaseFunctions.ExecuteQuery(query);
                                if (flag)
                                {
                                    ProductBindingModel transpoter = viewModel.GetProductbyProductName(viewModel.ProductNameText);
                                    viewModel.Products.Add(transpoter);
                                    viewModel.Model.ProductID = transpoter.ProductID;
                                    viewModel.Model.Product_Name = transpoter.ProductName;
                                    ToastHelper.ShowSuccess($"Product {viewModel.ProductNameText} save Successfully");
                                }

                            }
                            else
                            {
                                await MessageBoxHelper.ShowInfoDialogAsync("Product is not exist in Master Data", "Please Contact Admin to Add Product.");
                                viewModel.ProductNameText = string.Empty;
                                return;
                            }
                        }
                    }
                }));
            }
        }
        #endregion cmbProduct

        #region cmbGate
        private void cmbGate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var abc = e.Source as AutoCompleteBox;
            if (abc != null && !abc.IsKeyboardFocused)
            {
                if (abc.SelectedItem != null)
                {
                    cmbGate.Text = abc.SelectedItem.ToString();
                    selectedGate = abc.SelectedItem.ToString();
                }
            }
        }
        private async void cmbGate_TextChanged(object sender, RoutedEventArgs e)
        {
            var abc = e.OriginalSource as AutoCompleteBoxEx;
            if (abc != null)
            {
                if (!string.IsNullOrEmpty(abc.Text))
                {
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (selectedGate == abc.Text)
                            return;

                        try
                        {
                            if (!string.IsNullOrEmpty(abc.Text))
                            {
                                if (abc.Text.Length >= SmallDataSearchLength)
                                {
                                    var _Gates = new ObservableCollection<Gate>(viewModel.dataRepository.Context.Gates.Where(x => x.GateName.Contains(abc.Text) && x.IsActive).ToList());
                                    viewModel.AutofillGates = new ObservableCollection<string>(_Gates.OrderBy(P => P.GateName).Select(p => p.GateName).Distinct().ToList());
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _ = ex.Message;
                        }
                    }));
                }
            }
        }
        private async void cmbGate_LostFocus(object sender, RoutedEventArgs e)
        {
            var abc = e.OriginalSource as TextBox;
            if (abc != null && !abc.IsKeyboardFocused)
            {
                await Dispatcher.BeginInvoke(new Action(async () =>
                {
                    if (!string.IsNullOrEmpty(viewModel.GateNameText))
                    {
                        viewModel.GateNameText = viewModel.GateNameText.Trim();
                        var _Gates = viewModel.Gates.Where(p => p.GateName.ToUpper() == viewModel.GateNameText.ToUpper());
                        if (_Gates.Count() > 0)
                        {
                            var Gate = _Gates.FirstOrDefault();
                            viewModel.Model.GateID = Gate.GateID;
                            viewModel.Model.Gate_Name = Gate.GateName;
                            viewModel.Model.Gate = Gate;
                        }
                        else
                        {
                            if (Common.ValidatePageRequest(PageEntity.Gate, PageAccess.Create))
                            {

                                string query = $"INSERT INTO [dbo].[Gate] ([GateName],[GateType],[IsActive]) VALUES ('{viewModel.GateNameText}','IN', 1)";
                                bool flag = DatabaseFunctions.ExecuteQuery(query);
                                if (flag)
                                {
                                    Gate transpoter = viewModel.GetGatebyGateName(viewModel.GateNameText);
                                    viewModel.Gates.Add(transpoter);
                                    viewModel.Model.GateID = transpoter.GateID;
                                    viewModel.Model.Gate_Name = transpoter.GateName;
                                    ToastHelper.ShowSuccess($"Gate {viewModel.GateNameText} save Successfully");
                                }
                            }

                            else
                            {
                                await MessageBoxHelper.ShowInfoDialogAsync("Gate is not exist in Master Data", "Please Contact Admin to Add Gate.");
                                viewModel.GateNameText = string.Empty;
                                return;
                            }
                        }
                    }
                }));
            }
        }

        #endregion cmbGate

    }
}
