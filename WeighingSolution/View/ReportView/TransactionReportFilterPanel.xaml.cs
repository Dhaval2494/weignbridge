using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WeighingSolution.DataLayer;
using WeighingSolution.SharedComponents;
using WeighingSolution.UserControls;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for TransactionReportFilterPanel.xaml
    /// </summary>
    public partial class TransactionReportFilterPanel : Page
    {
        public weighbridgeEntities ctx = new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString());
        vmReportFilterPanel viewModel;
        public string selectedVehicle = string.Empty;
        public string selectedTransaporter = string.Empty;
        public string selectedAccount = string.Empty;
        public string selectedDriver = string.Empty;
        public string selectedProduct = string.Empty;
        public string selectedGate = string.Empty;
        public string selectedWeighbridge = string.Empty;
        public int LargeDataSearchLength = 1;
        public int SmallDataSearchLength = 1;
        int VehicleNo = 0;
        int ProductNo = 0;
        int AccountNo = 0;
        int WeighbridgeNo = 0;
        string S_ProductCode = string.Empty;
        string S_ProductName = string.Empty;
        public DateTime Sdate;
        public DateTime Edate;

        public TransactionReportFilterPanel()
        {
            InitializeComponent();
            viewModel = new vmReportFilterPanel();
            this.DataContext = viewModel;
            this.Unloaded += TransactionReportFilterPanel_Unloaded;
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                bool isChecked = false;
                if (Common.ValidatePageRequest(PageEntity.TranSummaryReport, PageAccess.Read))
                {
                    if (!isChecked)
                    {
                        rbTransactionSummary.IsChecked = true;
                        isChecked = true;
                    }
                    rbTransactionSummary.Visibility = Visibility.Visible;
                }
                if (Common.ValidatePageRequest(PageEntity.TranDetailReport, PageAccess.Read))
                {
                    if (!isChecked)
                    {
                        rbTransactionDetail.IsChecked = true;
                        isChecked = true;
                    }
                    rbTransactionDetail.Visibility = Visibility.Visible;
                }
                List<string> lstTransMode = new List<string>() { "Single", "Double", "Multi" };
                lstTransMode.Insert(0, "Select Transaction Mode");
                CmbTranMode.ItemsSource = lstTransMode;

                //List<string> lstTransStatus = ctx.Database.SqlQuery<string>("select distinct Status from TransactionData").ToList();
                List<string> lstTransStatus = new List<string>() { "Active", "Complete", "Void" };
                lstTransStatus.Insert(0, "Select Transaction Status");
                CmbTranStatus.ItemsSource = lstTransStatus;

                //List<string> lstTransType = ctx.Database.SqlQuery<string>("select distinct TransactionType from TransactionData").ToList();
                List<string> lstTransType = new List<string>() { "Gross", "Tare" };
                lstTransType.Insert(0, "Select Transaction Type");
                CmbTranType.ItemsSource = lstTransType;

                List<CustomField> LstCustomField = ctx.CustomFields.ToList();

                for (int i = 1; i <= 20; i++)
                {
                    string fieldName = $"Field{i}";
                    var customField = LstCustomField.FirstOrDefault(p => p.FieldName == fieldName && p.IsVisible);

                    if (customField != null)
                    {
                        // Dynamically get Label and TextBox controls by name
                        var lbl = this.FindName($"lbl{fieldName}") as Label;
                        var txt = this.FindName($"txt{fieldName}") as AutoCompleteBox;

                        // Set label content
                        if (lbl != null)
                        {
                            lbl.Content = customField.DisplayName;
                            lbl.Visibility = Visibility.Visible;
                        }

                        if (txt != null)
                            txt.Visibility = Visibility.Visible;

                        // Fetch distinct custom values from DB
                        string columnName = $"CustomValue{i}";
                        string sqlQuery = $"SELECT DISTINCT({columnName}) FROM TransactionData";
                        var customValues = ctx.Database.SqlQuery<string>(sqlQuery).ToList();

                        // Dynamically assign to viewModel.CustomFieldsX
                        var prop = viewModel.GetType().GetProperty($"CustomFields{i}");
                        if (prop != null)
                        {
                            var collection = new ObservableCollection<string>(customValues);
                            prop.SetValue(viewModel, collection);
                        }
                    }
                }


                CultureInfo ci = new CultureInfo(Thread.CurrentThread.CurrentCulture.Name);
                ci.DateTimeFormat.ShortDatePattern = Constant.SelectedDateFormat;
                Thread.CurrentThread.CurrentCulture = ci;
                //dtStartDate.SelectedDate = DateTime.Now.AddDays(-15).AddHours(-23).AddMinutes(-59).AddSeconds(-59).ToString();
                //dtEndDate.SelectedDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1).ToString();
                dtStartDate.SelectedDate = DateTime.Now.AddDays(-15).Date;
                dtEndDate.SelectedDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);

            }
            catch (Exception Ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(Ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\TransactionReportFilterPanelError.txt", json);
            }
            Mouse.OverrideCursor = null;
        }

        private void TransactionReportFilterPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            ctx.Dispose();
        }
        #region Master Mathed

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
                                    var _Accounts = new ObservableCollection<Account>(ctx.Accounts.Where(x => x.CompanyName.Contains(abc.Text) && x.IsAccount != null && x.IsAccount.Value && x.Active == true).Take(15).ToList());
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
            try
            {
                var abc = e.OriginalSource as TextBox;
                if (abc != null && !abc.IsKeyboardFocused)
                {
                    await Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        if (!string.IsNullOrEmpty(viewModel.AccountNameText))
                        {
                            if (ctx.Accounts.Where(x => x.CompanyName == viewModel.AccountNameText).Count() > 0)
                                AccountNo = ctx.Accounts.Where(x => x.CompanyName == viewModel.AccountNameText).FirstOrDefault().AccountID;
                            else
                                AccountNo = 0;
                        }
                        else
                        {
                            AccountNo = 0;
                        }
                    }));
                }
            }
            catch (Exception ex)
            {

            }

        }
        #endregion cmbAccount

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
                                if (abc.Text.Length >= SmallDataSearchLength)
                                {
                                    var _Materials = new ObservableCollection<ProductBindingModel>(ctx.Products.Where(x => (x.ProductName.Contains(abc.Text) || x.ProductCode.Contains(abc.Text)) && x.IsActive == true).Take(15).Select(x => new ProductBindingModel { ProductCode = x.ProductCode, ProductName = x.ProductName }).ToList());
                                    viewModel.AutofillProducts = new ObservableCollection<string>(_Materials.OrderBy(P => P.ProductName).Select(p => p.ProductName).Distinct().ToList());
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
            try
            {
                var abc = e.OriginalSource as TextBox;
                if (abc != null && !abc.IsKeyboardFocused)
                {
                    await Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(viewModel.ProductNameText))
                            {
                                viewModel.ProductNameText = viewModel.ProductNameText.Trim();
                                //string ProductCode = viewModel.ProductNameText.Split('~')[0].Trim();
                                //string ProductName = viewModel.ProductNameText.Split('~')[1].Trim();
                                var _Products = ctx.Products.Where(p => p.ProductName.ToUpper() == viewModel.ProductNameText.ToUpper());
                                if (_Products.Count() > 0)
                                {
                                    var Product = _Products.FirstOrDefault();
                                    ProductNo = Product.ProductID;
                                    S_ProductCode = Product.ProductCode;
                                    S_ProductName = Product.ProductName;
                                }
                                else
                                {
                                    S_ProductCode = string.Empty;
                                    S_ProductName = string.Empty;
                                    ProductNo = 0;
                                }
                            }
                            else
                            {
                                S_ProductCode = string.Empty;
                                S_ProductName = string.Empty;
                                ProductNo = 0;
                            }
                        }
                        catch { }
                    }));
                }
            }
            catch (Exception ex)
            {

            }

        }
        #endregion cmbProduct

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
                                    var _Vehicles = new ObservableCollection<VehicleBindingModel>(ctx.Vehicles.Where(x => x.VehicleNumber.Contains(abc.Text) && x.IsActive).Take(15).Select(x => new VehicleBindingModel { VehicleID = x.VehicleID, VehicleNumber = x.VehicleNumber, TareWeight = x.TareWeight }).ToList());
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
            try
            {
                var abc = e.OriginalSource as TextBox;
                if (abc != null && !abc.IsKeyboardFocused)
                {
                    await Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        if (!string.IsNullOrEmpty(viewModel.VehicleNumberText))
                        {
                            if (ctx.Vehicles.Where(x => x.VehicleNumber == viewModel.VehicleNumberText).Count() > 0)
                                VehicleNo = ctx.Vehicles.Where(x => x.VehicleNumber == viewModel.VehicleNumberText).FirstOrDefault().VehicleID;
                            else
                                VehicleNo = 0;
                        }
                        else
                            VehicleNo = 0;
                    }));
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion cmbVehicalNo

        #region cmbWeighbridge
        private void cmbWeighbridge_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var abc = e.Source as AutoCompleteBox;

            if (abc != null && !abc.IsKeyboardFocused)
            {
                if (abc.SelectedItem != null)
                {
                    cmbWeighbridge.Text = abc.SelectedItem.ToString();
                    selectedWeighbridge = abc.SelectedItem.ToString();
                }
            }
        }

        private async void cmbWeighbridge_TextChanged(object sender, RoutedEventArgs e)
        {
            var abc = e.OriginalSource as AutoCompleteBoxEx;
            if (abc != null)
            {
                if (!string.IsNullOrEmpty(abc.Text))
                {
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (selectedWeighbridge == abc.Text)
                            return;

                        try
                        {
                            if (!string.IsNullOrEmpty(abc.Text))
                            {
                                if (abc.Text.Length >= LargeDataSearchLength)
                                {
                                    var _weightBridges = new ObservableCollection<WeightBridge>(ctx.WeightBridges.Where(x => x.ScaleName.Contains(abc.Text) && x.IsActive == true).Take(15).ToList());
                                    viewModel.AutofillWeighbridge = new ObservableCollection<string>(_weightBridges.OrderBy(P => P.ScaleName).Select(p => p.ScaleName).Distinct().ToList());
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
        private async void cmbWeighbridge_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                var abc = e.OriginalSource as TextBox;
                if (abc != null && !abc.IsKeyboardFocused)
                {
                    await Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        if (!string.IsNullOrEmpty(viewModel.WeighbridgeNameText))
                        {
                            if (ctx.WeightBridges.Where(x => x.ScaleName == viewModel.WeighbridgeNameText).Count() > 0)
                                WeighbridgeNo = ctx.WeightBridges.Where(x => x.ScaleName == viewModel.WeighbridgeNameText).FirstOrDefault().WeightBridgeID;
                            else
                                WeighbridgeNo = 0;
                        }
                        else
                        {
                            WeighbridgeNo = 0;
                        }
                    }));
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion cmbWeighbridge

        #endregion Master Method
        private void btnfilter_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                string strFilter = string.Empty;
                if (dtStartDate.SelectedDate == null || dtEndDate.SelectedDate == null)
                {
                    dtStartDate.SelectedDate = DateTime.Now.AddDays(-15).Date;
                    dtEndDate.SelectedDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
                }
                //Sdate = dtStartDate.SelectedDate.Value.Date;
                //Edate = dtEndDate.SelectedDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                Sdate = dtStartDate.SelectedDate.Value;
                Edate = dtEndDate.SelectedDate.Value;
                strFilter = "Transaction Date between " + dtStartDate.SelectedDate + " To " + dtEndDate.SelectedDate + " and ";
                string StartTicket = txtTicketFrom.Text;
                string EndTicket = txtTicketTo.Text;
                if (!string.IsNullOrEmpty(StartTicket) && !string.IsNullOrEmpty(EndTicket))
                    strFilter += "Ticket between " + StartTicket + " To " + EndTicket + " and ";
                string TranMode = "";
                if (CmbTranMode.SelectedIndex > 0)
                {
                    TranMode = CmbTranMode.SelectedItem.ToString();
                    strFilter += "Transcation Mode is " + TranMode + " and ";
                }
                string TranStatus = "";
                if (CmbTranStatus.SelectedIndex > 0)
                {
                    TranStatus = CmbTranStatus.SelectedItem.ToString();
                    strFilter += "Transcation Status is " + TranStatus + " and ";
                }
                string TranType = "";
                if (CmbTranType.SelectedIndex > 0)
                {
                    TranType = CmbTranType.SelectedItem.ToString();
                    strFilter += "Transcation Type is " + TranType + " and ";
                }
                string Acnti = "";
                if (!string.IsNullOrEmpty(viewModel.AccountNameText))
                {
                    strFilter += "Account is " + viewModel.AccountNameText + " and ";
                    Acnti = viewModel.AccountNameText;
                }
                string Productai = "";
                if (!string.IsNullOrEmpty(S_ProductName))
                {
                    strFilter += "Product is " + S_ProductName + " and ";
                    Productai = S_ProductName;
                }
                string Vehicleai = "";
                if (!string.IsNullOrEmpty(viewModel.VehicleNumberText))
                {
                    strFilter += "Vehicle is " + viewModel.VehicleNumberText + " and ";
                    Vehicleai = viewModel.VehicleNumberText;
                }

                string Weighbridgeai = "";
                if (!string.IsNullOrEmpty(viewModel.WeighbridgeNameText))
                {
                    strFilter += "Weighbridge is " + viewModel.WeighbridgeNameText + " and ";
                    Weighbridgeai = viewModel.WeighbridgeNameText;
                }
                string field1 = "";
                if (!string.IsNullOrEmpty(viewModel.CustomValue1))
                {
                    field1 = viewModel.CustomValue1;
                    strFilter += "Field1 is " + viewModel.CustomValue1 + " and ";
                }
                string field2 = "";
                if (!string.IsNullOrEmpty(viewModel.CustomValue2))
                {
                    field2 = viewModel.CustomValue2;
                    strFilter += "Field2 is " + viewModel.CustomValue2 + " and ";
                }
                string field3 = "";
                if (!string.IsNullOrEmpty(viewModel.CustomValue3))
                {
                    field3 = viewModel.CustomValue3;
                    strFilter += "Field3 is " + viewModel.CustomValue3 + " and ";

                }
                string field4 = "";
                if (!string.IsNullOrEmpty(viewModel.CustomValue4))
                {

                    field4 = viewModel.CustomValue4;
                    strFilter += "Field4 is " + viewModel.CustomValue4 + " and ";

                }
                string field5 = "";
                if (!string.IsNullOrEmpty(viewModel.CustomValue5))
                {
                    field5 = viewModel.CustomValue5;
                    strFilter += "Field5 is " + viewModel.CustomValue5 + " and ";
                }
                string field6 = "";
                if (!string.IsNullOrEmpty(viewModel.CustomValue6))
                {
                    field6 = viewModel.CustomValue6;
                    strFilter += "Field6 is " + viewModel.CustomValue6 + " and ";

                }
                string field7 = "";
                if (!string.IsNullOrEmpty(viewModel.CustomValue7))
                {
                    field7 = viewModel.CustomValue7;
                    strFilter += "Field7 is " + viewModel.CustomValue7 + " and ";

                }
                string field8 = "";
                if (!string.IsNullOrEmpty(viewModel.CustomValue8))
                {
                    field8 = viewModel.CustomValue8;
                    strFilter += "Field8 is " + viewModel.CustomValue8 + " and ";
                }
                string field9 = "";
                if (!string.IsNullOrEmpty(viewModel.CustomValue9))
                {
                    field9 = viewModel.CustomValue9;
                    strFilter += "Field9 is " + viewModel.CustomValue9 + " and ";

                }
                string field10 = "";
                if (!string.IsNullOrEmpty(viewModel.CustomValue10))
                {
                    field10 = viewModel.CustomValue10;
                    strFilter += "Field10 is " + viewModel.CustomValue10 + " and ";

                }
                string field11 = "";
                if (!string.IsNullOrEmpty(viewModel.CustomValue11))
                {
                    field11 = viewModel.CustomValue11;
                    strFilter += "Field11 is " + viewModel.CustomValue11 + " and ";

                }
                string field12 = "";
                if (!string.IsNullOrEmpty(viewModel.CustomValue12))
                {
                    field12 = viewModel.CustomValue12;
                    strFilter += "Field12 is " + viewModel.CustomValue12 + " and ";

                }
                string field13 = "";
                if (!string.IsNullOrEmpty(viewModel.CustomValue13))
                {
                    field13 = viewModel.CustomValue13;
                    strFilter += "Field13 is " + viewModel.CustomValue13 + " and ";

                }
                string field14 = "";
                if (!string.IsNullOrEmpty(viewModel.CustomValue14))
                {
                    field14 = viewModel.CustomValue14;
                    strFilter += "Field14 is " + viewModel.CustomValue14 + " and ";

                }
                string field15 = "";
                if (!string.IsNullOrEmpty(viewModel.CustomValue15))
                {
                    field15 = viewModel.CustomValue15;
                    strFilter += "Field15 is " + viewModel.CustomValue15 + " and ";

                }
                string field16 = "";
                if (!string.IsNullOrEmpty(viewModel.CustomValue16))
                {
                    field16 = viewModel.CustomValue16;
                    strFilter += "Field16 is " + viewModel.CustomValue16 + " and ";

                }
                string field17 = "";
                if (!string.IsNullOrEmpty(viewModel.CustomValue17))
                {
                    field17 = viewModel.CustomValue17;
                    strFilter += "Field17 is " + viewModel.CustomValue17 + " and ";

                }
                string field18 = "";
                if (!string.IsNullOrEmpty(viewModel.CustomValue18))
                {
                    field18 = viewModel.CustomValue18;
                    strFilter += "Field18 is " + viewModel.CustomValue18 + " and ";

                }
                string field19 = "";
                if (!string.IsNullOrEmpty(viewModel.CustomValue19))
                {
                    field19 = viewModel.CustomValue19;
                    strFilter += "Field19 is " + viewModel.CustomValue19 + " and ";

                }
                string field20 = "";
                if (!string.IsNullOrEmpty(viewModel.CustomValue20))
                {
                    field20 = viewModel.CustomValue20;
                    strFilter += "Field20 is " + viewModel.CustomValue20 + " and ";

                }

                DataTable dt = new DataTable();
                DataTable dtTranDetail = new DataTable();
                string query = string.Empty;
                string queryTranDetail = string.Empty;

                if (TranStatus == "")
                {
                    query += $"SELECT TData.ReceiptTicketID, TData.TicketID,TData.Charges, TData.Account_Name AS AccountName, TDetail.Product_Name AS ProductName, TData.Vehicle_Number AS VehicleNumber, TData.Transporter_Name AS TransporterName, GrossTime, GrossWeight, ";
                    query += $"TareTime, Tdetail.TareWeight, NetWeight, IsGrossManual, IsTareManual,TData.Charges,TData.TransactionType,TDetail.WeightBridgeName as 'ScaleName',TDetail.WeightUnit ";
                    query += $"FROM TransactionData TData ";
                    query += $"INNER JOIN TransactionDetail TDetail ON TData.ReceiptTicketID = TDetail.ReceiptTicketID  AND ";
                    query += $"(((TDetail.GrossWeight IS NOT NULL AND TDetail.GrossWeight <> 0) AND (TDetail.TareWeight IS NOT NULL AND TDetail.TareWeight <> 0)) OR ";
                    query += $"(TData.STATUS = 'Active' AND (TData.TransactionMode = 'Double' OR (	TData.TransactionMode = 'Multi' AND (SELECT SUM(SequenceNo) ";
                    query += $"FROM TransactionDetail TXD1  WHERE TXD1.ReceiptTicketID = TData.ReceiptTicketID) = 0)))) ";
                    query += $"WHERE (TDetail.CaptureTime BETWEEN @Sdate AND @Edate)";
                    if (!string.IsNullOrEmpty(StartTicket) && !string.IsNullOrEmpty(EndTicket))
                        query += $"AND ((@StartTicket = N'' OR TData.TicketID >= @StartTicket) AND (@EndTicket = N'' OR TData.TicketID <= @EndTicket))";
                    if (!string.IsNullOrEmpty(TranMode)) query += $"AND (@TranMode = N'' OR TData.TransactionMode = @TranMode) ";
                    if (!string.IsNullOrEmpty(TranStatus)) query += $"AND (@TranStatus = N'' OR TData.STATUS = @TranStatus) ";
                    if (!string.IsNullOrEmpty(TranType)) query += $"AND (@TranType = N'' OR TData.TransactionType = @TranType) ";
                    if (!string.IsNullOrEmpty(Productai)) query += $"AND (@Product = N'' OR  TDetail.Product_Name = @Product) ";
                    if (!string.IsNullOrEmpty(Vehicleai)) query += $"AND (@Vehicle = N'' OR TData.Vehicle_Number = @Vehicle) ";
                    if (!string.IsNullOrEmpty(Acnti)) query += $"AND (@Acnt = N'' OR TData.Account_Name = @Acnt) ";
                    if (!string.IsNullOrEmpty(Weighbridgeai)) query += $"AND (@Weighbridge = N'' OR TDetail.WeightBridgeName = @Weighbridge) ";
                    if (!string.IsNullOrEmpty(field1)) query += $"AND (@field1 = N'' OR TData.CustomValue1 LIKE '%'+@field1+'%') ";
                    if (!string.IsNullOrEmpty(field2)) query += $"AND (@field2 = N'' OR TData.CustomValue2 LIKE '%'+@field2+'%') ";
                    if (!string.IsNullOrEmpty(field3)) query += $"AND (@field3 = N'' OR TData.CustomValue3 LIKE '%'+@field3+'%') ";
                    if (!string.IsNullOrEmpty(field4)) query += $"AND (@field4 = N'' OR TData.CustomValue4 LIKE '%'+@field4+'%') ";
                    if (!string.IsNullOrEmpty(field5)) query += $"AND (@field5 = N'' OR TData.CustomValue5 LIKE '%'+@field5+'%') ";
                    if (!string.IsNullOrEmpty(field6)) query += $"AND (@field6 = N'' OR TData.CustomValue6 LIKE '%'+@field6+'%') ";
                    if (!string.IsNullOrEmpty(field7)) query += $"AND (@field7 = N'' OR TData.CustomValue7 LIKE '%'+@field7+'%') ";
                    if (!string.IsNullOrEmpty(field8)) query += $"AND (@field8 = N'' OR TData.CustomValue8 LIKE '%'+@field8+'%') ";
                    if (!string.IsNullOrEmpty(field9)) query += $"AND (@field9 = N'' OR TData.CustomValue9 LIKE '%'+@field9+'%') ";
                    if (!string.IsNullOrEmpty(field10)) query += $"AND (@field10 = N'' OR TData.CustomValue10 LIKE '%'+@field10+'%') ";
                    if (!string.IsNullOrEmpty(field11)) query += $"AND (@field11 = N'' OR TData.CustomValue11 LIKE '%'+@field11+'%') ";
                    if (!string.IsNullOrEmpty(field12)) query += $"AND (@field12 = N'' OR TData.CustomValue12 LIKE '%'+@field12+'%') ";
                    if (!string.IsNullOrEmpty(field13)) query += $"AND (@field13 = N'' OR TData.CustomValue13 LIKE '%'+@field13+'%') ";
                    if (!string.IsNullOrEmpty(field14)) query += $"AND (@field14 = N'' OR TData.CustomValue14 LIKE '%'+@field14+'%') ";
                    if (!string.IsNullOrEmpty(field15)) query += $"AND (@field15 = N'' OR TData.CustomValue15 LIKE '%'+@field15+'%') ";
                    if (!string.IsNullOrEmpty(field16)) query += $"AND (@field16 = N'' OR TData.CustomValue16 LIKE '%'+@field16+'%') ";
                    if (!string.IsNullOrEmpty(field17)) query += $"AND (@field17 = N'' OR TData.CustomValue17 LIKE '%'+@field17+'%') ";
                    if (!string.IsNullOrEmpty(field18)) query += $"AND (@field18 = N'' OR TData.CustomValue18 LIKE '%'+@field18+'%') ";
                    if (!string.IsNullOrEmpty(field19)) query += $"AND (@field19 = N'' OR TData.CustomValue19 LIKE '%'+@field19+'%') ";
                    if (!string.IsNullOrEmpty(field20)) query += $"AND (@field20 = N'' OR TData.CustomValue20 LIKE '%'+@field20+'%') ";
                    query += $" ORDER BY TData.TicketID";

                    queryTranDetail += $"SELECT TData.ReceiptTicketID, TData.TicketID,TData.Charges,Account_Name AS AccountName, Transporter_Name AS TransaporterName, TData.TransactionMode, TData.STATUS AS TransactionStatus,TData.Charges,";
                    queryTranDetail += $"TData.TransactionType,Driver_Name AS DriverName, Vehicle_Number AS VehicleNumber, ";
                    queryTranDetail += $"Field1Value = TData.CustomValue1,";
                    queryTranDetail += $"Field2Value = TData.CustomValue2,";
                    queryTranDetail += $"Field3Value = TData.CustomValue3,";
                    queryTranDetail += $"Field4Value = TData.CustomValue4,";
                    queryTranDetail += $"Field5Value = TData.CustomValue5,";
                    queryTranDetail += $"Field6Value = TData.CustomValue6,";
                    queryTranDetail += $"Field7Value = TData.CustomValue7,";
                    queryTranDetail += $"Field8Value = TData.CustomValue8,";
                    queryTranDetail += $"Field9Value = TData.CustomValue9,";
                    queryTranDetail += $"Field10Value = TData.CustomValue10,";
                    queryTranDetail += $"Field11Value = TData.CustomValue11,";
                    queryTranDetail += $"Field12Value = TData.CustomValue12,";
                    queryTranDetail += $"Field13Value = TData.CustomValue13,";
                    queryTranDetail += $"Field14Value = TData.CustomValue14,";
                    queryTranDetail += $"Field15Value = TData.CustomValue15,";
                    queryTranDetail += $"Field16Value = TData.CustomValue16,";
                    queryTranDetail += $"Field17Value = TData.CustomValue17,";
                    queryTranDetail += $"Field18Value = TData.CustomValue18,";
                    queryTranDetail += $"Field19Value = TData.CustomValue19,";
                    queryTranDetail += $"Field20Value = TData.CustomValue20,";
                    queryTranDetail += $"TxD.SequenceNo, ISNULL(TxD.TareWeight, 0) AS TareWeight, TxD.TareTime, TXD.IsTareManual, ISNULL(TxD.GrossWeight, 0) AS GrossWeight, TxD.GrossTime, TxD.IsGrossManual, TxD.NetWeight, TxD.WeighmentType, TxD.WeightUnit, ";
                    queryTranDetail += $"TxD.Product_Name AS ProductName, TxD.Gate_Name AS GateName, TxD.WB_User_Name AS UserName,TXD.WeightBridgeName as ScaleName ";
                    queryTranDetail += $"FROM TransactionData TData ";
                    queryTranDetail += $"INNER JOIN TransactionDetail TXD ON TXD.ReceiptTicketID = TData.ReceiptTicketID ";
                    queryTranDetail += $"WHERE (((TxD.GrossWeight IS NOT NULL AND TxD.GrossWeight <> 0) AND (TxD.TareWeight IS NOT NULL AND TxD.TareWeight <> 0)) OR ";
                    queryTranDetail += $"(TData.STATUS = 'Active' AND (TData.TransactionMode = 'Double' OR (TData.TransactionMode = 'Multi' AND ";
                    queryTranDetail += $"(SELECT SUM(SequenceNo) FROM TransactionDetail TXD1	WHERE TXD1.ReceiptTicketID = TData.ReceiptTicketID) = 0)))) AND ";
                    queryTranDetail += $"((TXD.CaptureTime BETWEEN @Sdate AND @Edate) ";
                    if (!string.IsNullOrEmpty(StartTicket) && !string.IsNullOrEmpty(EndTicket))
                        queryTranDetail += $"AND ((@StartTicket = N'' OR TData.TicketID >= @StartTicket) AND (@EndTicket = N'' OR TData.TicketID <= @EndTicket))";
                    if (!string.IsNullOrEmpty(TranMode)) queryTranDetail += $"AND (@TranMode = N'' OR TData.TransactionMode = @TranMode) ";
                    if (!string.IsNullOrEmpty(TranStatus)) queryTranDetail += $"AND (@TranStatus = N'' OR TData.STATUS = @TranStatus) ";
                    if (!string.IsNullOrEmpty(TranType)) queryTranDetail += $"AND (@TranType = N'' OR TData.TransactionType = @TranType) ";
                    if (!string.IsNullOrEmpty(Productai)) queryTranDetail += $"AND (@Product = N'' OR  TxD.Product_Name = @Product) ";
                    if (!string.IsNullOrEmpty(Vehicleai)) queryTranDetail += $"AND (@Vehicle = N'' OR TData.Vehicle_Number = @Vehicle) ";
                    if (!string.IsNullOrEmpty(Acnti)) queryTranDetail += $"AND (@Acnt = N'' OR TData.Account_Name = @Acnt) ";
                    if (!string.IsNullOrEmpty(Weighbridgeai)) queryTranDetail += $"AND (@Weighbridge = N'' OR TxD.WeightBridgeName = @Weighbridge) ";
                    if (!string.IsNullOrEmpty(field1)) queryTranDetail += $"AND (@field1 = N'' OR TData.CustomValue1 LIKE '%'+@field1+'%')";
                    if (!string.IsNullOrEmpty(field2)) queryTranDetail += $"AND (@field2 = N'' OR TData.CustomValue2 LIKE '%'+@field2+'%')";
                    if (!string.IsNullOrEmpty(field3)) queryTranDetail += $"AND (@field3 = N'' OR TData.CustomValue3 LIKE '%'+@field3+'%')";
                    if (!string.IsNullOrEmpty(field4)) queryTranDetail += $"AND (@field4 = N'' OR TData.CustomValue4 LIKE '%'+@field4+'%')";
                    if (!string.IsNullOrEmpty(field5)) queryTranDetail += $"AND (@field5 = N'' OR TData.CustomValue5 LIKE '%'+@field5+'%')";
                    if (!string.IsNullOrEmpty(field6)) queryTranDetail += $"AND (@field6 = N'' OR TData.CustomValue6 LIKE '%'+@field6+'%')";
                    if (!string.IsNullOrEmpty(field7)) queryTranDetail += $"AND (@field7 = N'' OR TData.CustomValue7 LIKE '%'+@field7+'%')";
                    if (!string.IsNullOrEmpty(field8)) queryTranDetail += $"AND (@field8 = N'' OR TData.CustomValue8 LIKE '%'+@field8+'%')";
                    if (!string.IsNullOrEmpty(field9)) queryTranDetail += $"AND (@field9 = N'' OR TData.CustomValue9 LIKE '%'+@field9+'%')";
                    if (!string.IsNullOrEmpty(field10)) queryTranDetail += $"AND (@field10 = N'' OR TData.CustomValue10 LIKE '%'+@field10+'%')";
                    if (!string.IsNullOrEmpty(field11)) queryTranDetail += $"AND (@field11 = N'' OR TData.CustomValue11 LIKE '%'+@field11+'%')";
                    if (!string.IsNullOrEmpty(field12)) queryTranDetail += $"AND (@field12 = N'' OR TData.CustomValue12 LIKE '%'+@field12+'%')";
                    if (!string.IsNullOrEmpty(field13)) queryTranDetail += $"AND (@field13 = N'' OR TData.CustomValue13 LIKE '%'+@field13+'%')";
                    if (!string.IsNullOrEmpty(field14)) queryTranDetail += $"AND (@field14 = N'' OR TData.CustomValue14 LIKE '%'+@field14+'%')";
                    if (!string.IsNullOrEmpty(field15)) queryTranDetail += $"AND (@field15 = N'' OR TData.CustomValue15 LIKE '%'+@field15+'%')";
                    if (!string.IsNullOrEmpty(field16)) queryTranDetail += $"AND (@field16 = N'' OR TData.CustomValue16 LIKE '%'+@field16+'%')";
                    if (!string.IsNullOrEmpty(field17)) queryTranDetail += $"AND (@field17 = N'' OR TData.CustomValue17 LIKE '%'+@field17+'%')";
                    if (!string.IsNullOrEmpty(field18)) queryTranDetail += $"AND (@field18 = N'' OR TData.CustomValue18 LIKE '%'+@field18+'%')";
                    if (!string.IsNullOrEmpty(field19)) queryTranDetail += $"AND (@field19 = N'' OR TData.CustomValue19 LIKE '%'+@field19+'%')";
                    if (!string.IsNullOrEmpty(field20)) queryTranDetail += $"AND(@field20 = N'' OR TData.CustomValue20 LIKE '%' + @field20 + '%')";
                    queryTranDetail += $") ORDER BY TData.TicketID";
                }
                else
                {
                    query += $"SELECT TData.ReceiptTicketID, TData.TicketID,TData.Charges, Account_Name AS AccountName, Product_Name AS ProductName, Vehicle_Number AS VehicleNumber, GrossTime, GrossWeight, ";
                    query += $"TareTime, Tdetail.TareWeight, NetWeight, IsGrossManual, IsTareManual,TData.Charges,TData.TransactionType,TDetail.WeightBridgeName as 'ScaleName',TDetail.WeightUnit ";
                    query += $"FROM TransactionData TData ";
                    query += $"INNER JOIN TransactionDetail TDetail ON TData.ReceiptTicketID = TDetail.ReceiptTicketID AND ";
                    query += $"(((TDetail.GrossWeight IS NOT NULL AND TDetail.GrossWeight <> 0) AND (TDetail.TareWeight IS NOT NULL AND TDetail.TareWeight <> 0)) OR ";
                    query += $"(TData.STATUS = 'Active' AND (TData.TransactionMode = 'Double' OR ";
                    query += $"(TData.TransactionMode = 'Multi' AND (SELECT SUM(SequenceNo) FROM TransactionDetail TXD1 WHERE TXD1.ReceiptTicketID = TData.ReceiptTicketID) = 0)))) ";
                    query += $"WHERE (TDetail.CaptureTime BETWEEN @Sdate AND @Edate)";

                    if (!string.IsNullOrEmpty(StartTicket) && !string.IsNullOrEmpty(EndTicket))
                        query += $"AND ((@StartTicket = N'' OR TData.TicketID >= @StartTicket) AND (@EndTicket = N'' OR TData.TicketID <= @EndTicket))";
                    if (!string.IsNullOrEmpty(TranMode)) query += $"AND (@TranMode = N'' OR TData.TransactionMode = @TranMode) ";
                    if (!string.IsNullOrEmpty(TranStatus)) query += $"AND (@TranStatus = N'' OR TData.STATUS = @TranStatus) ";
                    if (!string.IsNullOrEmpty(TranType)) query += $"AND (@TranType = N'' OR TData.TransactionType = @TranType) ";
                    if (!string.IsNullOrEmpty(Productai)) query += $"AND (@Product = N'' OR  TDetail.Product_Name = @Product) ";
                    if (!string.IsNullOrEmpty(Vehicleai)) query += $"AND (@Vehicle = N'' OR TData.Vehicle_Number = @Vehicle) ";
                    if (!string.IsNullOrEmpty(Acnti)) query += $"AND (@Acnt = N'' OR TData.Account_Name = @Acnt) ";
                    if (!string.IsNullOrEmpty(Weighbridgeai)) query += $"AND (@Weighbridge = N'' OR TDetail.WeightBridgeName = @Weighbridge) ";
                    if (!string.IsNullOrEmpty(field1)) query += $"AND (@field1 = N'' OR TData.CustomValue1 LIKE '%'+@field1+'%')";
                    if (!string.IsNullOrEmpty(field2)) query += $"AND (@field2 = N'' OR TData.CustomValue2 LIKE '%'+@field2+'%')";
                    if (!string.IsNullOrEmpty(field3)) query += $"AND (@field3 = N'' OR TData.CustomValue3 LIKE '%'+@field3+'%')";
                    if (!string.IsNullOrEmpty(field4)) query += $"AND (@field4 = N'' OR TData.CustomValue4 LIKE '%'+@field4+'%')";
                    if (!string.IsNullOrEmpty(field5)) query += $"AND (@field5 = N'' OR TData.CustomValue5 LIKE '%'+@field5+'%')";
                    if (!string.IsNullOrEmpty(field6)) query += $"AND (@field6 = N'' OR TData.CustomValue6 LIKE '%'+@field6+'%')";
                    if (!string.IsNullOrEmpty(field7)) query += $"AND (@field7 = N'' OR TData.CustomValue7 LIKE '%'+@field7+'%')";
                    if (!string.IsNullOrEmpty(field8)) query += $"AND (@field8 = N'' OR TData.CustomValue8 LIKE '%'+@field8+'%')";
                    if (!string.IsNullOrEmpty(field9)) query += $"AND (@field9 = N'' OR TData.CustomValue9 LIKE '%'+@field9+'%')";
                    if (!string.IsNullOrEmpty(field10)) query += $"AND (@field10 = N'' OR TData.CustomValue10 LIKE '%'+@field10+'%')";
                    if (!string.IsNullOrEmpty(field11)) query += $"AND (@field11 = N'' OR TData.CustomValue11 LIKE '%'+@field11+'%')";
                    if (!string.IsNullOrEmpty(field12)) query += $"AND (@field12 = N'' OR TData.CustomValue12 LIKE '%'+@field12+'%')";
                    if (!string.IsNullOrEmpty(field13)) query += $"AND (@field13 = N'' OR TData.CustomValue13 LIKE '%'+@field13+'%')";
                    if (!string.IsNullOrEmpty(field14)) query += $"AND (@field14 = N'' OR TData.CustomValue14 LIKE '%'+@field14+'%')";
                    if (!string.IsNullOrEmpty(field15)) query += $"AND (@field15 = N'' OR TData.CustomValue15 LIKE '%'+@field15+'%')";
                    if (!string.IsNullOrEmpty(field16)) query += $"AND (@field16 = N'' OR TData.CustomValue16 LIKE '%'+@field16+'%')";
                    if (!string.IsNullOrEmpty(field17)) query += $"AND (@field17 = N'' OR TData.CustomValue17 LIKE '%'+@field17+'%')";
                    if (!string.IsNullOrEmpty(field18)) query += $"AND (@field18 = N'' OR TData.CustomValue18 LIKE '%'+@field18+'%')";
                    if (!string.IsNullOrEmpty(field19)) query += $"AND (@field19 = N'' OR TData.CustomValue19 LIKE '%'+@field19+'%')";
                    if (!string.IsNullOrEmpty(field20)) query += $"AND (@field20 = N'' OR TData.CustomValue20 LIKE '%'+@field20+'%')";
                    query += $" ORDER BY TData.TicketID";

                    queryTranDetail += $"SELECT TData.ReceiptTicketID, TData.TicketID,TData.Charges, Account_Name AS AccountName, Transporter_Name AS TransaporterName, TData.TransactionMode, TData.STATUS AS TransactionStatus,TData.Charges,TData.TransactionType,";
                    queryTranDetail += $"Driver_Name AS DriverName, Vehicle_Number AS VehicleNumber, ";
                    queryTranDetail += $"Field1Value = TData.CustomValue1, ";
                    queryTranDetail += $"Field2Value = TData.CustomValue2, ";
                    queryTranDetail += $"Field3Value = TData.CustomValue3, ";
                    queryTranDetail += $"Field4Value = TData.CustomValue4, ";
                    queryTranDetail += $"Field5Value = TData.CustomValue5, ";
                    queryTranDetail += $"Field6Value = TData.CustomValue6, ";
                    queryTranDetail += $"Field7Value = TData.CustomValue7, ";
                    queryTranDetail += $"Field8Value = TData.CustomValue8, ";
                    queryTranDetail += $"Field9Value = TData.CustomValue9, ";
                    queryTranDetail += $"Field10Value = TData.CustomValue10, ";
                    queryTranDetail += $"Field11Value = TData.CustomValue11, ";
                    queryTranDetail += $"Field12Value = TData.CustomValue12, ";
                    queryTranDetail += $"Field13Value = TData.CustomValue13, ";
                    queryTranDetail += $"Field14Value = TData.CustomValue14, ";
                    queryTranDetail += $"Field15Value = TData.CustomValue15, ";
                    queryTranDetail += $"Field16Value = TData.CustomValue16, ";
                    queryTranDetail += $"Field17Value = TData.CustomValue17, ";
                    queryTranDetail += $"Field18Value = TData.CustomValue18, ";
                    queryTranDetail += $"Field19Value = TData.CustomValue19, ";
                    queryTranDetail += $"Field20Value = TData.CustomValue20, ";
                    queryTranDetail += $"TxD.SequenceNo, ISNULL(TxD.TareWeight, 0) AS TareWeight, TxD.TareTime, TXD.IsTareManual, ISNULL(TxD.GrossWeight, 0) AS GrossWeight, TxD.GrossTime, TxD.IsGrossManual, TxD.NetWeight, ";
                    queryTranDetail += $"TxD.WeighmentType, TxD.WeightUnit, TxD.Product_Name AS ProductName, TxD.Gate_Name AS GateName, TxD.WB_User_Name AS UserName, TxD.WeightBridgeName as ScaleName ";
                    queryTranDetail += $"FROM TransactionData TData ";
                    ////queryTranDetail += $"FROM TransactionData TData LEFT JOIN Account A ON A.AccountID = TData.AccountID LEFT JOIN Account TR ON TR.AccountID = TData.TransporterID ";
                    //queryTranDetail += $"LEFT JOIN Vehicle V ON V.VehicleID = TData.VehicleID LEFT JOIN Driver D ON D.DriverID = TData.DriverID ";
                    queryTranDetail += $"INNER JOIN TransactionDetail TXD ON TXD.ReceiptTicketID = TData.ReceiptTicketID ";
                    //queryTranDetail += $"INNER JOIN WeightBridge WB ON WB.WeightBridgeID = TxD.WeightBridgeID ";
                    //queryTranDetail += $"LEFT JOIN UserMaster UM ON UM.UserID = TxD.UserID LEFT JOIN Product P ON P.ProductID = TxD.ProductID ";
                    //queryTranDetail += $"LEFT JOIN Gate G ON G.GateID = TXD.GateID ";
                    queryTranDetail += $"WHERE (((TxD.GrossWeight IS NOT NULL AND TxD.GrossWeight <> 0) AND ";
                    queryTranDetail += $"(TxD.TareWeight IS NOT NULL AND TxD.TareWeight <> 0)) OR (TData.STATUS = 'Active' AND (TData.TransactionMode = 'Double' OR ";
                    queryTranDetail += $"(TData.TransactionMode = 'Multi' AND (SELECT SUM(SequenceNo) FROM TransactionDetail TXD1	WHERE TXD1.ReceiptTicketID = TData.ReceiptTicketID) = 0)))) AND ";
                    queryTranDetail += $"((TXD.CaptureTime BETWEEN @Sdate AND @Edate)";
                    if (!string.IsNullOrEmpty(StartTicket) && !string.IsNullOrEmpty(EndTicket))
                        queryTranDetail += $"AND ((@StartTicket = N'' OR TData.TicketID >= @StartTicket) AND (@EndTicket = N'' OR TData.TicketID <= @EndTicket))";
                    if (!string.IsNullOrEmpty(TranMode)) queryTranDetail += $"AND (@TranMode = N'' OR TData.TransactionMode = @TranMode) ";
                    if (!string.IsNullOrEmpty(TranStatus)) queryTranDetail += $"AND (@TranStatus = N'' OR TData.STATUS = @TranStatus) ";
                    if (!string.IsNullOrEmpty(TranType)) queryTranDetail += $"AND (@TranType = N'' OR TData.TransactionType = @TranType) ";
                    if (!string.IsNullOrEmpty(Productai)) queryTranDetail += $"AND (@Product = N'' OR  TxD.Product_Name = @Product) ";
                    if (!string.IsNullOrEmpty(Vehicleai)) queryTranDetail += $"AND (@Vehicle = N'' OR TData.Vehicle_Number = @Vehicle) ";
                    if (!string.IsNullOrEmpty(Acnti)) queryTranDetail += $"AND (@Acnt = N'' OR TData.Account_Name = @Acnt) ";
                    if (!string.IsNullOrEmpty(Weighbridgeai)) queryTranDetail += $"AND (@Weighbridge = N'' OR TxD.WeightBridgeName = @Weighbridge) ";
                    if (!string.IsNullOrEmpty(field1)) queryTranDetail += $"AND (@field1 = N'' OR TData.CustomValue1 LIKE '%'+@field1+'%')";
                    if (!string.IsNullOrEmpty(field2)) queryTranDetail += $"AND (@field2 = N'' OR TData.CustomValue2 LIKE '%'+@field2+'%')";
                    if (!string.IsNullOrEmpty(field3)) queryTranDetail += $"AND (@field3 = N'' OR TData.CustomValue3 LIKE '%'+@field3+'%')";
                    if (!string.IsNullOrEmpty(field4)) queryTranDetail += $"AND (@field4 = N'' OR TData.CustomValue4 LIKE '%'+@field4+'%')";
                    if (!string.IsNullOrEmpty(field5)) queryTranDetail += $"AND (@field5 = N'' OR TData.CustomValue5 LIKE '%'+@field5+'%')";
                    if (!string.IsNullOrEmpty(field6)) queryTranDetail += $"AND (@field6 = N'' OR TData.CustomValue6 LIKE '%'+@field6+'%')";
                    if (!string.IsNullOrEmpty(field7)) queryTranDetail += $"AND (@field7 = N'' OR TData.CustomValue7 LIKE '%'+@field7+'%')";
                    if (!string.IsNullOrEmpty(field8)) queryTranDetail += $"AND (@field8 = N'' OR TData.CustomValue8 LIKE '%'+@field8+'%')";
                    if (!string.IsNullOrEmpty(field9)) queryTranDetail += $"AND (@field9 = N'' OR TData.CustomValue9 LIKE '%'+@field9+'%')";
                    if (!string.IsNullOrEmpty(field10)) queryTranDetail += $"AND (@field10 = N'' OR TData.CustomValue10 LIKE '%'+@field10+'%')  ";
                    if (!string.IsNullOrEmpty(field11)) queryTranDetail += $"AND (@field11 = N'' OR TData.CustomValue11 LIKE '%'+@field11+'%')  ";
                    if (!string.IsNullOrEmpty(field12)) queryTranDetail += $"AND (@field12 = N'' OR TData.CustomValue12 LIKE '%'+@field12+'%')  ";
                    if (!string.IsNullOrEmpty(field13)) queryTranDetail += $"AND (@field13 = N'' OR TData.CustomValue13 LIKE '%'+@field13+'%')  ";
                    if (!string.IsNullOrEmpty(field14)) queryTranDetail += $"AND (@field14 = N'' OR TData.CustomValue14 LIKE '%'+@field14+'%')  ";
                    if (!string.IsNullOrEmpty(field15)) queryTranDetail += $"AND (@field15 = N'' OR TData.CustomValue15 LIKE '%'+@field15+'%')  ";
                    if (!string.IsNullOrEmpty(field16)) queryTranDetail += $"AND (@field16 = N'' OR TData.CustomValue16 LIKE '%'+@field16+'%')  ";
                    if (!string.IsNullOrEmpty(field17)) queryTranDetail += $"AND (@field17 = N'' OR TData.CustomValue17 LIKE '%'+@field17+'%')  ";
                    if (!string.IsNullOrEmpty(field18)) queryTranDetail += $"AND (@field18 = N'' OR TData.CustomValue18 LIKE '%'+@field18+'%')  ";
                    if (!string.IsNullOrEmpty(field19)) queryTranDetail += $"AND (@field19 = N'' OR TData.CustomValue19 LIKE '%'+@field19+'%')  ";
                    if (!string.IsNullOrEmpty(field20)) queryTranDetail += $"AND (@field20 = N'' OR TData.CustomValue20 LIKE '%'+@field20+'%')  ";
                    queryTranDetail += $") ORDER BY TData.TicketID";
                }

                SqlConnection con = new SqlConnection(System.Windows.Application.Current.Properties["DefalutConnectionString"].ToString());
                SqlCommand cmd = new SqlCommand(query, con);
                SqlCommand cmdTranDetail = new SqlCommand(queryTranDetail, con);
                cmd.Parameters.AddWithValue("@TranMode", SqlDbType.VarChar).Value = TranMode;
                cmd.Parameters.AddWithValue("@TranStatus", SqlDbType.VarChar).Value = TranStatus;
                cmd.Parameters.AddWithValue("@TranType", SqlDbType.VarChar).Value = TranType;
                cmd.Parameters.AddWithValue("@Sdate", SqlDbType.DateTime).Value = Sdate;
                cmd.Parameters.AddWithValue("@Edate", SqlDbType.DateTime).Value = Edate;
                cmd.Parameters.AddWithValue("@StartTicket", SqlDbType.DateTime).Value = StartTicket;
                cmd.Parameters.AddWithValue("@EndTicket", SqlDbType.DateTime).Value = EndTicket;
                cmd.Parameters.AddWithValue("@Acnt", SqlDbType.VarChar).Value = Acnti;
                cmd.Parameters.AddWithValue("@Product", SqlDbType.VarChar).Value = Productai;
                cmd.Parameters.AddWithValue("@Weighbridge", SqlDbType.VarChar).Value = Weighbridgeai;
                cmd.Parameters.AddWithValue("@Vehicle", SqlDbType.VarChar).Value = Vehicleai;
                cmd.Parameters.AddWithValue("@field1", SqlDbType.VarChar).Value = field1;
                cmd.Parameters.AddWithValue("@field2", SqlDbType.VarChar).Value = field2;
                cmd.Parameters.AddWithValue("@field3", SqlDbType.VarChar).Value = field3;
                cmd.Parameters.AddWithValue("@field4", SqlDbType.VarChar).Value = field4;
                cmd.Parameters.AddWithValue("@field5", SqlDbType.VarChar).Value = field5;
                cmd.Parameters.AddWithValue("@field6", SqlDbType.VarChar).Value = field6;
                cmd.Parameters.AddWithValue("@field7", SqlDbType.VarChar).Value = field7;
                cmd.Parameters.AddWithValue("@field8", SqlDbType.VarChar).Value = field8;
                cmd.Parameters.AddWithValue("@field9", SqlDbType.VarChar).Value = field9;
                cmd.Parameters.AddWithValue("@field10", SqlDbType.VarChar).Value = field10;
                cmd.Parameters.AddWithValue("@field11", SqlDbType.VarChar).Value = field11;
                cmd.Parameters.AddWithValue("@field12", SqlDbType.VarChar).Value = field12;
                cmd.Parameters.AddWithValue("@field13", SqlDbType.VarChar).Value = field13;
                cmd.Parameters.AddWithValue("@field14", SqlDbType.VarChar).Value = field14;
                cmd.Parameters.AddWithValue("@field15", SqlDbType.VarChar).Value = field15;
                cmd.Parameters.AddWithValue("@field16", SqlDbType.VarChar).Value = field16;
                cmd.Parameters.AddWithValue("@field17", SqlDbType.VarChar).Value = field17;
                cmd.Parameters.AddWithValue("@field18", SqlDbType.VarChar).Value = field18;
                cmd.Parameters.AddWithValue("@field19", SqlDbType.VarChar).Value = field19;
                cmd.Parameters.AddWithValue("@field20", SqlDbType.VarChar).Value = field20;
                cmdTranDetail.Parameters.AddWithValue("@TranMode", SqlDbType.VarChar).Value = TranMode;
                cmdTranDetail.Parameters.AddWithValue("@TranStatus", SqlDbType.VarChar).Value = TranStatus;
                cmdTranDetail.Parameters.AddWithValue("@TranType", SqlDbType.VarChar).Value = TranType;
                cmdTranDetail.Parameters.AddWithValue("@Sdate", SqlDbType.DateTime).Value = Sdate;
                cmdTranDetail.Parameters.AddWithValue("@Edate", SqlDbType.DateTime).Value = Edate;
                cmdTranDetail.Parameters.AddWithValue("@StartTicket", SqlDbType.DateTime).Value = StartTicket;
                cmdTranDetail.Parameters.AddWithValue("@EndTicket", SqlDbType.DateTime).Value = EndTicket;
                cmdTranDetail.Parameters.AddWithValue("@Acnt", SqlDbType.VarChar).Value = Acnti;
                cmdTranDetail.Parameters.AddWithValue("@Product", SqlDbType.VarChar).Value = Productai;
                cmdTranDetail.Parameters.AddWithValue("@Vehicle", SqlDbType.VarChar).Value = Vehicleai;
                cmdTranDetail.Parameters.AddWithValue("@Weighbridge", SqlDbType.VarChar).Value = Weighbridgeai;
                cmdTranDetail.Parameters.AddWithValue("@field1", SqlDbType.VarChar).Value = field1;
                cmdTranDetail.Parameters.AddWithValue("@field2", SqlDbType.VarChar).Value = field2;
                cmdTranDetail.Parameters.AddWithValue("@field3", SqlDbType.VarChar).Value = field3;
                cmdTranDetail.Parameters.AddWithValue("@field4", SqlDbType.VarChar).Value = field4;
                cmdTranDetail.Parameters.AddWithValue("@field5", SqlDbType.VarChar).Value = field5;
                cmdTranDetail.Parameters.AddWithValue("@field6", SqlDbType.VarChar).Value = field6;
                cmdTranDetail.Parameters.AddWithValue("@field7", SqlDbType.VarChar).Value = field7;
                cmdTranDetail.Parameters.AddWithValue("@field8", SqlDbType.VarChar).Value = field8;
                cmdTranDetail.Parameters.AddWithValue("@field9", SqlDbType.VarChar).Value = field9;
                cmdTranDetail.Parameters.AddWithValue("@field10", SqlDbType.VarChar).Value = field10;
                cmdTranDetail.Parameters.AddWithValue("@field11", SqlDbType.VarChar).Value = field11;
                cmdTranDetail.Parameters.AddWithValue("@field12", SqlDbType.VarChar).Value = field12;
                cmdTranDetail.Parameters.AddWithValue("@field13", SqlDbType.VarChar).Value = field13;
                cmdTranDetail.Parameters.AddWithValue("@field14", SqlDbType.VarChar).Value = field14;
                cmdTranDetail.Parameters.AddWithValue("@field15", SqlDbType.VarChar).Value = field15;
                cmdTranDetail.Parameters.AddWithValue("@field16", SqlDbType.VarChar).Value = field16;
                cmdTranDetail.Parameters.AddWithValue("@field17", SqlDbType.VarChar).Value = field17;
                cmdTranDetail.Parameters.AddWithValue("@field18", SqlDbType.VarChar).Value = field18;
                cmdTranDetail.Parameters.AddWithValue("@field19", SqlDbType.VarChar).Value = field19;
                cmdTranDetail.Parameters.AddWithValue("@field20", SqlDbType.VarChar).Value = field20;


                con.Open();
                if (rbTransactionDetail.IsChecked.GetValueOrDefault(false))
                {
                    dtTranDetail.Load(cmdTranDetail.ExecuteReader());
                    if (dtTranDetail.Rows.Count > 0)
                        this.NavigationService.Navigate(new TransactionDetailReport(dtTranDetail, strFilter));
                    else
                        ((MetroWindow)App.Current.MainWindow).ShowMessageAsync(" ", "No Record Exist");
                }
                else
                {
                    dt.Load(cmd.ExecuteReader());
                    if (dt.Rows.Count > 0)
                        this.NavigationService.Navigate(new TransactionReport(dt, strFilter));
                    else
                        ((MetroWindow)App.Current.MainWindow).ShowMessageAsync(" ", "No Record Exist");
                }
                con.Close();
            }
            catch (Exception Ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(Ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\TransactionReportFilterPanelError.txt", json);
            }
            Mouse.OverrideCursor = null;
        }

        public static string ReplaceLastOccurrence(string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find);
            if (place == -1)
                return Source;

            string result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }

        private void btnBackToReport_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/ReportView/Reports_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }




    }

    public class vmReportFilterPanel : NotifyPropertyChanged
    {
        #region CustomFeilds and values

        private string _CustomValue1; public string CustomValue1 { get { return _CustomValue1; } set { if (_CustomValue1 != value) { _CustomValue1 = value; OnPropertyChanged(); } } }
        private string _CustomValue2; public string CustomValue2 { get { return _CustomValue2; } set { if (_CustomValue2 != value) { _CustomValue2 = value; OnPropertyChanged(); } } }
        private string _CustomValue3; public string CustomValue3 { get { return _CustomValue3; } set { if (_CustomValue3 != value) { _CustomValue3 = value; OnPropertyChanged(); } } }
        private string _CustomValue4; public string CustomValue4 { get { return _CustomValue4; } set { if (_CustomValue4 != value) { _CustomValue4 = value; OnPropertyChanged(); } } }
        private string _CustomValue5; public string CustomValue5 { get { return _CustomValue5; } set { if (_CustomValue5 != value) { _CustomValue5 = value; OnPropertyChanged(); } } }
        private string _CustomValue6; public string CustomValue6 { get { return _CustomValue6; } set { if (_CustomValue6 != value) { _CustomValue6 = value; OnPropertyChanged(); } } }
        private string _CustomValue7; public string CustomValue7 { get { return _CustomValue7; } set { if (_CustomValue7 != value) { _CustomValue7 = value; OnPropertyChanged(); } } }
        private string _CustomValue8; public string CustomValue8 { get { return _CustomValue8; } set { if (_CustomValue8 != value) { _CustomValue8 = value; OnPropertyChanged(); } } }
        private string _CustomValue9; public string CustomValue9 { get { return _CustomValue9; } set { if (_CustomValue9 != value) { _CustomValue9 = value; OnPropertyChanged(); } } }
        private string _CustomValue10; public string CustomValue10 { get { return _CustomValue10; } set { if (_CustomValue10 != value) { _CustomValue10 = value; OnPropertyChanged(); } } }
        private string _CustomValue11; public string CustomValue11 { get { return _CustomValue11; } set { if (_CustomValue11 != value) { _CustomValue11 = value; OnPropertyChanged(); } } }
        private string _CustomValue12; public string CustomValue12 { get { return _CustomValue12; } set { if (_CustomValue12 != value) { _CustomValue12 = value; OnPropertyChanged(); } } }
        private string _CustomValue13; public string CustomValue13 { get { return _CustomValue13; } set { if (_CustomValue13 != value) { _CustomValue13 = value; OnPropertyChanged(); } } }
        private string _CustomValue14; public string CustomValue14 { get { return _CustomValue14; } set { if (_CustomValue14 != value) { _CustomValue14 = value; OnPropertyChanged(); } } }
        private string _CustomValue15; public string CustomValue15 { get { return _CustomValue15; } set { if (_CustomValue15 != value) { _CustomValue15 = value; OnPropertyChanged(); } } }
        private string _CustomValue16; public string CustomValue16 { get { return _CustomValue16; } set { if (_CustomValue16 != value) { _CustomValue16 = value; OnPropertyChanged(); } } }
        private string _CustomValue17; public string CustomValue17 { get { return _CustomValue17; } set { if (_CustomValue17 != value) { _CustomValue17 = value; OnPropertyChanged(); } } }
        private string _CustomValue18; public string CustomValue18 { get { return _CustomValue18; } set { if (_CustomValue18 != value) { _CustomValue18 = value; OnPropertyChanged(); } } }
        private string _CustomValue19; public string CustomValue19 { get { return _CustomValue19; } set { if (_CustomValue19 != value) { _CustomValue19 = value; OnPropertyChanged(); } } }
        private string _CustomValue20; public string CustomValue20 { get { return _CustomValue20; } set { if (_CustomValue20 != value) { _CustomValue20 = value; OnPropertyChanged(); } } }


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

        private ObservableCollection<string> _AutofillWeighbridge;
        public ObservableCollection<string> AutofillWeighbridge
        {
            get { return _AutofillWeighbridge; }
            set { _AutofillWeighbridge = value; OnPropertyChanged(); }
        }

        private ObservableCollection<WeightBridge> _activeweighbridges;
        public ObservableCollection<WeightBridge> ActiveWeighBridges
        {
            get { return _activeweighbridges; }
            set { _activeweighbridges = value; OnPropertyChanged(); }
        }

        #endregion

        #region Command and Properties Declaration


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

        private string _WeighbridgeNameText;
        public string WeighbridgeNameText
        {
            get { return _WeighbridgeNameText; }
            set
            {
                if (_WeighbridgeNameText != value)
                {
                    _WeighbridgeNameText = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion
    }
}


