using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
    /// Interaction logic for TransactionManagement.xaml
    /// </summary>
    public partial class TransactionManagement : Page
    {
        public TransactionViewModel viewModel;
        public Guid GuidTicketID = Guid.Empty;

        public string selectedVehicle = string.Empty;
        public string selectedTransaporter = string.Empty;
        public string selectedAccount = string.Empty;
        public string selectedDriver = string.Empty;
        public string selectedProduct = string.Empty;
        public string selectedGate = string.Empty;
        public int LargeDataSearchLength = 2;
        public int SmallDataSearchLength = 1;
        public bool IsunmannedEdit = false;
        Thread WeightThread;


        //DispatcherTimer Timer;
        //decimal AllowDiffer = 5;
        //List<StableWeigthClass> StableWeigthClasses = new List<StableWeigthClass>();

        #region CTOR + Window Function
        private void SetupPage()
        {
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            this.KeyUp += OnKeyUpHandler;

            viewModel = new TransactionViewModel(this, GuidTicketID, IsunmannedEdit);
            this.DataContext = viewModel;
            MainWindow.weighCommand = false;
        }
        public TransactionManagement()
        {
            InitializeComponent();
            SetupPage();
        }
        public TransactionManagement(Guid ReceiptTicketID)
        {
            InitializeComponent();
            GuidTicketID = ReceiptTicketID;
            SetupPage();
        }

        public List<T> FindAllControlsOfType<T>(DependencyObject parent) where T : Control
        {
            var list = new List<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t)
                    list.Add(t);

                list.AddRange(FindAllControlsOfType<T>(child));
            }
            return list;
        }
        private async void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await viewModel.LoadAsync();

                var camViewers = FindAllControlsOfType<CamViewer>(icCameraList);
                foreach (var viewer in camViewers)
                {
                    //await viewer.InitializeVideoController();
                    //_ = viewer.vcVideoController?.StartVideo();

                    _ = Task.Run(async () =>
                    {
                        await viewer.InitializeVideoController();
                        _ = Dispatcher.Invoke(() => viewer.vcVideoController?.StartVideo());
                    });

                }

                if (Constant.HasAnprLicense)
                {
                    splAnprLabel.Visibility = Visibility.Visible;
                    grdAnpr.Visibility = Visibility.Visible;
                }

                //For caps charecter in combobox textbox
                //(cmbVehicalNo.Template.FindName("PART_EditableTextBox", cmbVehicalNo) as TextBox).CharacterCasing = CharacterCasing.Upper;

                //Access based visibility (this will override any visibility xaml bindings)


                if (Constant.hasUnmannedPlugin)
                {
                    gridtoken.Visibility = Visibility.Visible;
                    stktoken.Visibility = Visibility.Visible;
                }
                if (!Common.ValidatePageRequest(PageEntity.Account, PageAccess.Create))
                    iAccountAdd.Visibility = Visibility.Collapsed;
                if (!Common.ValidatePageRequest(PageEntity.Account, PageAccess.Create))
                    btnAddTransporter.Visibility = Visibility.Collapsed;
                if (!Common.ValidatePageRequest(PageEntity.Vehicle, PageAccess.Create))
                    btnAddVehical.Visibility = Visibility.Collapsed;
                if (!Common.ValidatePageRequest(PageEntity.Driver, PageAccess.Create))
                    btnAddDriver.Visibility = Visibility.Collapsed;
                if (!Common.ValidatePageRequest(PageEntity.Product, PageAccess.Create))
                    btnAddProduct.Visibility = Visibility.Collapsed;
                if (!Common.ValidatePageRequest(PageEntity.Gate, PageAccess.Create))
                    btnAddGate.Visibility = Visibility.Collapsed;

                if (!Common.ValidatePageRequest(PageEntity.AllowManualEntry, PageAccess.Execute))
                {
                    rdbAutomatic.Visibility = Visibility.Collapsed;
                    rdbManual.Visibility = Visibility.Collapsed;
                }
                else
                {
                    rdbAutomatic.Visibility = Visibility.Visible;
                    rdbManual.Visibility = Visibility.Visible;
                }

                //Edit Mode settings
                if (viewModel.EditMode)
                {
                    btnAddVehical.Visibility = Visibility.Collapsed;
                    if (viewModel.Model.TransactionMode == TransactionMode.Double.ToString())
                    {
                        rdbSinglePass.IsEnabled = false;
                        rdbDoublePass.IsChecked = true;
                    }
                    else if (viewModel.Model.TransactionMode == TransactionMode.Multi.ToString())
                    {
                        rdbSinglePass.IsEnabled = false;
                        rdbDoublePass.IsEnabled = false;
                        rdbMultiPass.IsChecked = true;
                    }

                    if (Constant.hasUnmannedPlugin)
                    {
                        if (viewModel.Model.TransactionMode == TransactionMode.Single.ToString())
                        {
                            rdbTransactionMode_Checked(sender, e);
                        }
                    }

                }

                try
                {
                    //Propagate Key Bindings to top window.
                    Window window = Window.GetWindow(this);
                    if (window != null)
                    {
                        foreach (InputBinding ib in this.InputBindings)
                        {
                            window.InputBindings.Add(ib);
                        }

                    }

                    WeightThread = new Thread(new ThreadStart(WeightThreadStart));
                    WeightThread.Start();
                }
                catch { }
            }
            catch (Exception Ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(Ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\TransactionManagementError.txt", json);
            }
        }
        private void WeightThreadStart()
        {
            try
            {
                while (true)
                {
                    if (viewModel.SelectedWeighBridge != null && MainWindow.ActiveWeighBridge != null)
                    {
                        viewModel.SelectedWeighBridge.IsCablePlugged = MainWindow.ActiveWeighBridge.IsCablePlugged;
                        if (viewModel.SelectedWeighBridge.IsCablePlugged)
                            viewModel.SelectedWeighBridge.FormatedData = MainWindow.ActiveWeighBridge.FormatedData;
                        else
                            viewModel.SelectedWeighBridge.FormatedData = "";
                    }
                    Thread.Sleep(500);
                }
            }
            catch { }
        }
        private void OnKeyUpHandler(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.F6)
                {
                    if (!string.IsNullOrEmpty(viewModel.VehicleNumberText))
                    {
                        if (Common.ValidatePageRequest(PageEntity.Vehicle, PageAccess.Create))
                        {
                            viewModel.OnVehicleSave(cmbVehicalNo.Text);
                        }
                        //Keyboard.Focus(cmbVehicalNo);
                    }
                }
                if (e.Key == Key.F5)
                {
                    int UserID = Convert.ToInt32(Application.Current.Properties["LoggedInUserID"].ToString());
                    int data = (int)viewModel.dataRepository.Context.UserMasters.Where(x => x.UserID == UserID).FirstOrDefault().TemplateID;
                    if (data > 1)
                    {
                        UserControls.Flyouts.LoginPOPUP loginPOPUP = new UserControls.Flyouts.LoginPOPUP();
                        viewModel.OpenFlyout(loginPOPUP);
                    }
                }
            }
            catch { }
        }

        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (WeightThread != null)
            {
                WeightThread.Abort();
                WeightThread = null;
            }


            var camViewers = FindAllControlsOfType<CamViewer>(icCameraList);

            foreach (var viewer in camViewers)
            {
                try
                {
                    // Stop the video controller if it exists
                    if (viewer.vcVideoController != null)
                    {
                        viewer.AutoPlay = false;
                        viewer.vcVideoController.StopVideo();
                        viewer.vcVideoController = null; // Release the reference
                    }
                }
                catch (Exception ex)
                {
                    _ = ex.Message;
                }
            }

            viewModel.CameraList = null;


            MainWindow mw = (App.Current.MainWindow) as MainWindow;
            if (mw != null)
            {
                Flyout flyoutContainer = (Flyout)mw.FindName("flyoutContainer");
                Flyout flyChangePassword = (Flyout)mw.FindName("flyChangePassword");
                flyChangePassword.IsOpen = false;
                flyoutContainer.IsOpen = false;

                //Remove Key Bindings from top window.
                foreach (InputBinding ib in this.InputBindings)
                {
                    mw.InputBindings.Remove(ib);
                }
            }
            viewModel.Dispose();
        }

        #endregion

        #region FrontEnd Elements Events
        private void rdbTransactionMode_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                btnContinue.Visibility = Visibility.Collapsed;
                //btnContinueAndPrint.Visibility = Visibility.Collapsed;
                btnEndTransaction.Visibility = Visibility.Collapsed;
                //btnEndTransactionAndPrint.Visibility = Visibility.Collapsed;

                int TotalTransactionsSaved = viewModel.Model.TransactionDetails.Count;
                if (viewModel.ActiveTransaction != null)
                    TotalTransactionsSaved--;

                if (viewModel.Model.TransactionMode == TransactionMode.Single.ToString())
                {
                    if (!Constant.hasUnmannedPlugin || viewModel.ActiveTransaction.NetWeight.HasValue)
                    {
                        btnEndTransaction.Visibility = Visibility.Visible;
                        //if (viewModel.IsPrintSetup)
                        //    btnEndTransactionAndPrint.Visibility = Visibility.Visible;
                    }
                    if (Constant.hasUnmannedPlugin && !viewModel.EditMode)
                        btnContinue.Visibility = Visibility.Visible;

                    viewModel.ActiveTransaction.WeighmentType = WeighmentType.Gross.ToString();
                }
                else if (viewModel.Model.TransactionMode == TransactionMode.Double.ToString())
                {
                    if (TotalTransactionsSaved >= 1)
                    {
                        btnEndTransaction.Visibility = Visibility.Visible;
                        //if (viewModel.IsPrintSetup)
                        //    btnEndTransactionAndPrint.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        btnContinue.Visibility = Visibility.Visible;
                        //if (viewModel.IsPrintSetup)
                        //    btnContinueAndPrint.Visibility = Visibility.Visible;
                    }
                }
                else if (viewModel.Model.TransactionMode == TransactionMode.Multi.ToString())
                {
                    btnContinue.Visibility = Visibility.Visible;
                    //if (viewModel.IsPrintSetup)
                    //    btnContinueAndPrint.Visibility = Visibility.Visible;
                    if (TotalTransactionsSaved >= 1)
                    {
                        btnEndTransaction.Visibility = Visibility.Visible;
                        //if (viewModel.IsPrintSetup)
                        //    btnEndTransactionAndPrint.Visibility = Visibility.Visible;
                    }
                }
            }
            catch
            { }
        }

        private void rdbManual_Checked(object sender, RoutedEventArgs e)
        {
            txtWeightMeter.Clear();
            txtWeightMeter.Focus();
        }
        #endregion

        #region Add New Details (via Flyouts)

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

                            var _vehicles = viewModel.dataRepository.Context.Vehicles.Where(p => p.VehicleNumber == viewModel.VehicleNumberText && p.IsActive == false);
                            if (_vehicles.Count() > 0)
                            {
                                if (Constant.IsAdmin)
                                {
                                    MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Vehicle is not Active", "Do you wish to Active This Vehicle?");
                                    if (msgbox_result == MessageDialogResult.Affirmative)
                                    {
                                        string query = $"Update Vehicle set IsActive = 1 where VehicleNumber = '{viewModel.VehicleNumberText}'";
                                        bool flag = DatabaseFunctions.ExecuteQuery(query);
                                        Thread.Sleep(500);

                                        _ = Vehicle_LostFocusAsync();
                                    }
                                    else
                                    {
                                        viewModel.VehicleNumberText = string.Empty;
                                    }
                                }
                                else
                                {
                                    await MessageBoxHelper.ShowInfoDialogAsync("Error!", "This Vehicle is deactive, please contact admin.");
                                    viewModel.Model.VehicleID = null;
                                    viewModel.Model.Vehicle_Number = string.Empty;
                                    return;
                                }
                            }
                            else
                            {
                                _ = Vehicle_LostFocusAsync();
                            }
                        }
                        else
                        {
                            await MessageBoxHelper.ShowInfoDialogAsync("Error!", "This Vehicle field is empty, please Enter Vehicle.");
                            viewModel.Model.VehicleID = null;
                            viewModel.Model.Vehicle_Number = string.Empty;
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        _ = ex.Message;
                    }
                }));
            }
        }

        public async Task Vehicle_LostFocusAsync()
        {
            var _Vehicles = viewModel.Vehicles?.Where(p => string.Equals(p.VehicleNumber, viewModel.VehicleNumberText, StringComparison.CurrentCultureIgnoreCase));
            if (_Vehicles.Count() > 0)
            {
                var vehicle = _Vehicles.FirstOrDefault();
                if (viewModel.Model.TransactionMode == TransactionMode.Single.ToString() && (vehicle.TareWeight == null || vehicle.TareWeight == 0))
                {
                    UserControls.Flyouts.Vehicle ucaVehicle = new UserControls.Flyouts.Vehicle(vehicle.VehicleID, cmbVehicalNo.Text);
                    viewModel.OpenFlyout(ucaVehicle);
                }

                string AllowTruckIfTransactionActive = "YES";

                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AllowTruckIfTransactionActive"]))
                    AllowTruckIfTransactionActive = ConfigurationManager.AppSettings["AllowTruckIfTransactionActive"].ToString();

                if (AllowTruckIfTransactionActive == "NO")
                {
                    if (viewModel.dataRepository.Context.TransactionDatas.Where(p => p.VehicleID == vehicle.VehicleID && p.Status == TransactionStatus.Active.ToString()).Count() > 0)
                    {
                        var transaction = viewModel.dataRepository.Context.TransactionDatas.Where(p => p.VehicleID == vehicle.VehicleID && p.Status == TransactionStatus.Active.ToString()).FirstOrDefault();
                        await MessageBoxHelper.ShowInfoDialogAsync($"Vehicle exist in active transaction with Ticket No: {transaction.TicketID} ", "Please complate it first.");
                        return;
                    }
                }
                viewModel.Model.VehicleID = vehicle.VehicleID;
                viewModel.Model.Vehicle_Number = vehicle.VehicleNumber;
                viewModel.LoadLastTransactionList(vehicle.VehicleID);
            }
            else
            {
                viewModel.OnVehicleRefresh(viewModel.VehicleNumberText);

                var _Vehicles1 = viewModel.Vehicles.Where(p => string.Equals(p.VehicleNumber, viewModel.VehicleNumberText, StringComparison.CurrentCultureIgnoreCase));
                if (_Vehicles1.Count() > 0)
                {
                    var vehicle = _Vehicles1.FirstOrDefault();

                    viewModel.Model.VehicleID = vehicle.VehicleID;
                    viewModel.Model.Vehicle_Number = vehicle.VehicleNumber;
                    viewModel.LoadLastTransactionList(vehicle.VehicleID);
                }
                else
                {
                    if (Common.ValidatePageRequest(PageEntity.Vehicle, PageAccess.Create))
                    {
                        MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Vehicle is not exist in Master Data", "Do you wish to add Vehicle?");
                        if (msgbox_result == MessageDialogResult.Affirmative)
                        {
                            var isOpenVehicleFO = Convert.ToBoolean(viewModel.ApplicationSettingsList?.FirstOrDefault(x => x.SettingName == "IsOpenVehicleFO")?.SettingValue);
                            if (viewModel.Model.TransactionMode == TransactionMode.Single.ToString() || isOpenVehicleFO)
                            {
                                UserControls.Flyouts.Vehicle ucaVehicle = new UserControls.Flyouts.Vehicle(cmbVehicalNo.Text);
                                viewModel.OpenFlyout(ucaVehicle);
                            }
                            else
                            {
                                viewModel.OnVehicleSave(viewModel.VehicleNumberText);
                            }
                        }
                        else if (msgbox_result == MessageDialogResult.Negative)
                        {
                            viewModel.Model.VehicleID = null;
                            viewModel.Model.Vehicle_Number = string.Empty;
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
                    //cmbVehicalNo.Text = "";
                    Keyboard.Focus(cmbVehicalNo);
                }
            }

            if (viewModel.Model.VehicleID > 0) //&& viewModel.Model.Vehicle != null
                btnCaptureWeight.Visibility = Visibility.Visible;
            else
                btnCaptureWeight.Visibility = Visibility.Collapsed;
        }

        private void btnAddVehical_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UserControls.Flyouts.Vehicle ucVehicle = new UserControls.Flyouts.Vehicle();
            viewModel.OpenFlyout(ucVehicle);
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
                        var _Transporters = viewModel.Transporters?.Where(p => p.CompanyName.ToUpper() == viewModel.TransaporterNameText.ToUpper());
                        if (_Transporters.Count() > 0)
                        {
                            var Transporter = _Transporters.FirstOrDefault();
                            viewModel.Model.TransporterID = Transporter.AccountID;
                            viewModel.Model.Transporter_Name = Transporter.CompanyName;
                            viewModel.Model.Transporter = Transporter;
                        }
                        else
                        {
                            if (Common.ValidatePageRequest(PageEntity.Account, PageAccess.Create))
                            {
                                MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Transporter is not exist in Master Data", "Do you wish to add Transporter?");
                                if (msgbox_result == MessageDialogResult.Affirmative)
                                {
                                    var account = viewModel.Transporters.Any(x => x.CompanyName.ToUpper() == viewModel.TransaporterNameText.ToUpper());
                                    if (!account)
                                    {
                                        var IsOpenTransFO = Convert.ToBoolean(viewModel.ApplicationSettingsList?.FirstOrDefault(x => x.SettingName == "IsOpenTransporterFO")?.SettingValue);
                                        if (IsOpenTransFO)
                                        {
                                            UserControls.Flyouts.TransporterAccount ucTransporter = new UserControls.Flyouts.TransporterAccount(false, cmbTransaporter.Text);
                                            viewModel.OpenFlyout(ucTransporter);
                                        }
                                        else
                                        {
                                            string query = $"INSERT INTO [dbo].[Account]([CompanyName],[FirstName],[IsAccount],[IsTransporter],[Active]) " +
                                            $"VALUES ('{viewModel.TransaporterNameText}', '{viewModel.TransaporterNameText}', 0, 1, 1)";
                                            bool flag = DatabaseFunctions.ExecuteQuery(query);
                                            if (flag)
                                            {
                                                //var _vehiclesAdd = VehicledataRepository.Context.Vehicles.Where(p => p.VehicleNumber == VehicleNumber);
                                                var _transporterAdd = viewModel.dataRepository.Context.Accounts.Where(p => p.CompanyName.ToUpper() == viewModel.TransaporterNameText.ToUpper());
                                                if (_transporterAdd.Count() > 0)
                                                {
                                                    var transporter = _transporterAdd.FirstOrDefault();
                                                    await RaiseDetailSavedEvent(transporter.AccountID, transporter);
                                                }

                                                ToastHelper.ShowSuccess($"Transporter {viewModel.TransaporterNameText} save Successfully");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        string query = $"UPDATE [dbo].[Account] SET [IsTransporter] = 1 WHERE CompanyName = '{viewModel.TransaporterNameText}'";
                                        bool flag = DatabaseFunctions.ExecuteQuery(query);
                                        if (flag)
                                        {
                                            Thread.Sleep(500);
                                            //var _vehiclesAdd = VehicledataRepository.Context.Vehicles.Where(p => p.VehicleNumber == VehicleNumber);
                                            var _transporterAdd = viewModel.dataRepository.Context.Accounts.Where(p => p.CompanyName.ToUpper() == viewModel.TransaporterNameText.ToUpper());
                                            if (_transporterAdd.Count() > 0)
                                            {
                                                var transporter = _transporterAdd.FirstOrDefault();
                                                await RaiseDetailSavedEvent(transporter.AccountID, transporter);
                                            }
                                            ToastHelper.ShowSuccess($"Transporter {viewModel.TransaporterNameText} save Successfully");
                                        }
                                    }
                                }
                                else
                                {
                                    viewModel.Model.TransporterID = null;
                                    viewModel.Model.Transporter_Name = string.Empty;
                                    viewModel.Model.Transporter = null;
                                }
                            }
                            else
                            {
                                await MessageBoxHelper.ShowInfoDialogAsync("Transporter is not exist in Master Data", "Please Contact Admin to Add Transporter.");
                                viewModel.TransaporterNameText = string.Empty;
                                return;
                            }

                            //cmbTransaporter.Text = "";
                            //Keyboard.Focus(cmbTransaporter);
                        }
                    }
                }));
            }
        }
        private void btnAddTransporter_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UserControls.Flyouts.TransporterAccount ucTransporter = new UserControls.Flyouts.TransporterAccount(false);
            viewModel.OpenFlyout(ucTransporter);
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
                        var _Accounts = viewModel.Accounts?.Where(p => p.CompanyName.ToUpper() == viewModel.AccountNameText.ToUpper());
                        if (_Accounts.Count() > 0)
                        {
                            var Account = _Accounts.FirstOrDefault();
                            viewModel.Model.AccountID = Account.AccountID;
                            viewModel.Model.Account_Name = Account.CompanyName;
                            viewModel.Model.Account = Account;
                        }
                        else
                        {
                            if (Common.ValidatePageRequest(PageEntity.Account, PageAccess.Create))
                            {
                                MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Account is not exist in Master Data", "Do you wish to add Account?");
                                if (msgbox_result == MessageDialogResult.Affirmative)
                                {
                                    //UserControls.Flyouts.TransporterAccount ucAccount = new UserControls.Flyouts.TransporterAccount(true, cmbAccount.Text);
                                    //viewModel.OpenFlyout(ucAccount);

                                    var transporter = viewModel.Accounts?.FirstOrDefault(x => x.CompanyName.ToUpper() == viewModel.AccountNameText.ToUpper());
                                    if (transporter == null)
                                    {
                                        var IsOpenAccountFO = Convert.ToBoolean(viewModel.ApplicationSettingsList?.FirstOrDefault(x => x.SettingName == "IsOpenAccountFO")?.SettingValue);
                                        if (IsOpenAccountFO)
                                        {
                                            UserControls.Flyouts.TransporterAccount ucAccount = new UserControls.Flyouts.TransporterAccount(true, cmbAccount.Text);
                                            viewModel.OpenFlyout(ucAccount);
                                        }
                                        else
                                        {
                                            string query = $"INSERT INTO [dbo].[Account]([CompanyName],[FirstName],[IsAccount],[IsTransporter],[Active]) VALUES ('{viewModel.AccountNameText}', '{viewModel.AccountNameText}', 1, 0, 1)";
                                            bool flag = DatabaseFunctions.ExecuteQuery(query);
                                            if (flag)
                                            {
                                                //var _vehiclesAdd = VehicledataRepository.Context.Vehicles.Where(p => p.VehicleNumber == VehicleNumber);
                                                var _accountAdd = viewModel.dataRepository.Context.Accounts.Where(p => p.CompanyName.ToUpper() == viewModel.AccountNameText.ToUpper());
                                                if (_accountAdd.Count() > 0)
                                                {
                                                    var account = _accountAdd.FirstOrDefault();
                                                    await RaiseDetailSavedEvent(account.AccountID, account);
                                                }
                                                ToastHelper.ShowSuccess($"Account {viewModel.TransaporterNameText} save Successfully");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        string query = $"UPDATE [dbo].[Account] SET [IsAccount] = 1 WHERE CompanyName = '{viewModel.AccountNameText}'";
                                        bool flag = DatabaseFunctions.ExecuteQuery(query);
                                        if (flag)
                                        {
                                            Thread.Sleep(500);
                                            //var _vehiclesAdd = VehicledataRepository.Context.Vehicles.Where(p => p.VehicleNumber == VehicleNumber);
                                            var _accountAdd = viewModel.dataRepository.Context.Accounts.Where(p => p.CompanyName.ToUpper() == viewModel.AccountNameText.ToUpper());
                                            if (_accountAdd.Count() > 0)
                                            {
                                                var account = _accountAdd.FirstOrDefault();
                                                await RaiseDetailSavedEvent(account.AccountID, account);
                                            }
                                            ToastHelper.ShowSuccess($"Account {viewModel.TransaporterNameText} save Successfully");
                                        }
                                    }

                                }
                                else
                                {
                                    viewModel.Model.AccountID = null;
                                    viewModel.Model.Account_Name = string.Empty;
                                    viewModel.Model.Account = null;
                                }
                            }
                            else
                            {
                                await MessageBoxHelper.ShowInfoDialogAsync("Account is not exist in Master Data", "Please Contact Admin to Add Account.");
                                viewModel.AccountNameText = string.Empty;
                                return;
                            }

                            //cmbAccount.Text = "";
                            //Keyboard.Focus(cmbAccount);

                        }
                    }
                    else
                    {
                        ToastHelper.ShowError($"Please Enter Account Name!");

                    }

                }));
            }
        }
        private void iAccountAdd_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UserControls.Flyouts.TransporterAccount ucAccount = new UserControls.Flyouts.TransporterAccount(true);
            viewModel.OpenFlyout(ucAccount);
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
                                    var _Drivers = new ObservableCollection<DriverBindingModel>(viewModel.dataRepository.Context.Drivers.Where(x => (x.FirstName.Contains(abc.Text) || x.LastName.Contains(abc.Text)) && x.Active)?.Take(15)?.Select(x => new DriverBindingModel { DriverID = x.DriverID, FirstName = x.FirstName, LastName = x.LastName }).ToList());
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
                            viewModel.Model.Driver_Name = Driver.FirstName + " " + Driver.LastName;
                            viewModel.Model.Driver = viewModel.dataRepository.Context.Drivers.Where(p => p.DriverID == Driver.DriverID)?.FirstOrDefault();
                        }
                        else
                        {
                            if (Common.ValidatePageRequest(PageEntity.Driver, PageAccess.Create))
                            {
                                MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Driver is not exist in Master Data", "Do you wish to add Driver?");
                                if (msgbox_result == MessageDialogResult.Affirmative)
                                {
                                    var IsOpenDriverFO = Convert.ToBoolean(viewModel.ApplicationSettingsList?.FirstOrDefault(x => x.SettingName == "IsOpenDriverFO")?.SettingValue);
                                    if (IsOpenDriverFO)
                                    {
                                        WeighingSolution.UserControls.Flyouts.Driver ucDriver = new UserControls.Flyouts.Driver(cmbDriver.Text);
                                        viewModel.OpenFlyout(ucDriver);
                                    }
                                    //WeighingSolution.UserControls.Flyouts.Driver ucDriver = new UserControls.Flyouts.Driver(cmbDriver.Text);
                                    //viewModel.OpenFlyout(ucDriver);
                                    else
                                    {
                                        string query = $"INSERT INTO [dbo].[Driver] ([FirstName],[Active]) VALUES ('{viewModel.DriverNameText}', 1)";
                                        bool flag = DatabaseFunctions.ExecuteQuery(query);
                                        if (flag)
                                        {
                                            //var _vehiclesAdd = VehicledataRepository.Context.Vehicles.Where(p => p.VehicleNumber == VehicleNumber);
                                            var _DriverAdd = viewModel.dataRepository.Context.Drivers.Where(p => p.FirstName.ToUpper() == viewModel.DriverNameText.ToUpper());
                                            if (_DriverAdd.Count() > 0)
                                            {
                                                var driver = _DriverAdd.FirstOrDefault();
                                                await RaiseDetailSavedEvent(driver.DriverID, driver);
                                            }
                                            ToastHelper.ShowSuccess($"Driver {viewModel.DriverNameText} save Successfully");
                                        }
                                    }
                                }
                                else
                                {
                                    viewModel.Model.DriverID = null;
                                    viewModel.Model.Driver_Name = string.Empty;
                                    viewModel.Model.Driver = null;
                                }
                            }
                            else
                            {
                                await MessageBoxHelper.ShowInfoDialogAsync("Driver is not exist in Master Data", "Please Contact Admin to Add Driver.");
                                viewModel.DriverNameText = string.Empty;
                                return;
                            }

                            //cmbDriver.Text = "";
                            //Keyboard.Focus(cmbDriver);
                        }
                    }
                }));
            }
        }
        private void btnAddDriver_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            WeighingSolution.UserControls.Flyouts.Driver ucDriver = new UserControls.Flyouts.Driver();
            viewModel.OpenFlyout(ucDriver);
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

                        var _products = viewModel.dataRepository.Context.Products.Where(p => p.ProductName == viewModel.ProductNameText && p.IsActive == false);
                        if (_products.Count() > 0)
                        {
                            if (Constant.IsAdmin)
                            {
                                MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Product is not Active", "Do you wish to Active This Product?");
                                if (msgbox_result == MessageDialogResult.Affirmative)
                                {
                                    string query = $"Update Product set IsActive = 1 where ProductName = '{viewModel.ProductNameText}'";
                                    bool flag = DatabaseFunctions.ExecuteQuery(query);

                                    if (flag)
                                    {                                        //var _vehiclesAdd = VehicledataRepository.Context.Vehicles.Where(p => p.VehicleNumber == VehicleNumber);
                                        var _ProductAdd = viewModel.dataRepository.Context.Products.Where(p => p.ProductName.ToUpper() == viewModel.ProductNameText.ToUpper());
                                        if (_ProductAdd.Count() > 0)
                                        {
                                            var product = _ProductAdd.FirstOrDefault();
                                            await RaiseDetailSavedEvent(product.ProductID, product);
                                        }
                                        ToastHelper.ShowSuccess($"Product {viewModel.ProductNameText} save Successfully");
                                    }
                                    Thread.Sleep(500);

                                }
                                else
                                {
                                    viewModel.ProductNameText = string.Empty;
                                    return;
                                }
                            }
                            else
                            {
                                await MessageBoxHelper.ShowInfoDialogAsync("Error!", "This Product is deactive, please contact admin.");
                                viewModel.ActiveTransaction.ProductID = null;
                                viewModel.ActiveTransaction.Product_Name = string.Empty;
                                return;
                            }
                        }

                        var _Products = viewModel.Products?.Where(p => p.ProductName.ToUpper() == viewModel.ProductNameText.ToUpper());
                        if (_Products.Count() > 0)
                        {
                            var Product = _Products.FirstOrDefault();
                            viewModel.ActiveTransaction.ProductID = Product.ProductID;
                            viewModel.ActiveTransaction.Product_Name = Product.ProductName;
                            viewModel.ActiveTransaction.Product = viewModel.dataRepository.Context.Products.Where(p => p.ProductID == Product.ProductID)?.FirstOrDefault();
                        }
                        else
                        {
                            if (Common.ValidatePageRequest(PageEntity.Product, PageAccess.Create))
                            {
                                MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Product is not exist in Master Data", "Do you wish to add Product?");
                                if (msgbox_result == MessageDialogResult.Affirmative)
                                {
                                    var IsOpenProductFO = Convert.ToBoolean(viewModel.ApplicationSettingsList?.FirstOrDefault(x => x.SettingName == "IsOpenProductFO")?.SettingValue);
                                    if (IsOpenProductFO)
                                    {
                                        UserControls.Flyouts.Product ucProduct = new UserControls.Flyouts.Product(cmbProduct.Text);
                                        viewModel.OpenFlyout(ucProduct);
                                    }
                                    //UserControls.Flyouts.Product ucProduct = new UserControls.Flyouts.Product(cmbProduct.Text);
                                    //viewModel.OpenFlyout(ucProduct);
                                    else
                                    {
                                        string query = $"INSERT INTO [dbo].[Product] ([ProductName],[IsActive]) VALUES ('{viewModel.ProductNameText}', 1)";
                                        bool flag = DatabaseFunctions.ExecuteQuery(query);
                                        if (flag)
                                        {
                                            //var _vehiclesAdd = VehicledataRepository.Context.Vehicles.Where(p => p.VehicleNumber == VehicleNumber);
                                            var _ProductAdd = viewModel.dataRepository.Context.Products.Where(p => p.ProductName.ToUpper() == viewModel.ProductNameText.ToUpper());
                                            if (_ProductAdd.Count() > 0)
                                            {
                                                var product = _ProductAdd.FirstOrDefault();
                                                await RaiseDetailSavedEvent(product.ProductID, product);
                                            }
                                            ToastHelper.ShowSuccess($"Product {viewModel.ProductNameText} save Successfully");
                                        }
                                    }
                                }
                                else
                                {
                                    viewModel.ActiveTransaction.ProductID = null;
                                    viewModel.ActiveTransaction.Product_Name = null;
                                }
                            }
                            else
                            {
                                await MessageBoxHelper.ShowInfoDialogAsync("Product is not exist in Master Data", "Please Contact Admin to Add Product.");
                                viewModel.ProductNameText = string.Empty;
                                return;
                            }

                            //cmbProduct.Text = "";
                            //Keyboard.Focus(cmbProduct);
                        }
                    }
                }));
            }
        }
        private void btnAddProduct_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UserControls.Flyouts.Product ucProduct = new UserControls.Flyouts.Product();
            viewModel.OpenFlyout(ucProduct);
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
                            viewModel.ActiveTransaction.GateID = Gate.GateID;
                            viewModel.ActiveTransaction.Gate_Name = Gate.GateName;
                            viewModel.ActiveTransaction.Gate = Gate;
                        }
                        else
                        {
                            if (Common.ValidatePageRequest(PageEntity.Gate, PageAccess.Create))
                            {
                                MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Gate is not exist in Master Data", "Do you wish to add Gate?");
                                if (msgbox_result == MessageDialogResult.Affirmative)
                                {
                                    UserControls.Flyouts.Gate ucGate = new UserControls.Flyouts.Gate(cmbGate.Text);
                                    viewModel.OpenFlyout(ucGate);
                                }
                                else
                                {
                                    viewModel.ActiveTransaction.GateID = null;
                                    viewModel.ActiveTransaction.Gate_Name = string.Empty;
                                    viewModel.ActiveTransaction.Gate = null;
                                }
                            }
                            else
                            {
                                await MessageBoxHelper.ShowInfoDialogAsync("Gate is not exist in Master Data", "Please Contact Admin to Add Gate.");
                                viewModel.GateNameText = string.Empty;
                                return;
                            }

                            //cmbGate.Text = "";
                            //Keyboard.Focus(cmbGate);
                        }
                    }
                }));
            }
        }
        private void btnAddGate_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UserControls.Flyouts.Gate ucGate = new UserControls.Flyouts.Gate();
            viewModel.OpenFlyout(ucGate);
        }

        #endregion cmbGate

        #endregion

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

        private async Task RaiseDetailSavedEvent(int ObjectID, Object ObjectInstance)
        {
            await Task.Run(() =>
            {
                //viewModel.EventAggregator.GetEvent<AfterDetailSavedEvent>().Publish(new AfterDetailSavedEventArgs
                //{
                //    ObjectID = ObjectID,
                //    ObjectInstance = ObjectInstance,
                //    ObjectType = ObjectInstance.GetType().Name
                //});

                viewModel.DetailsSaved(new AfterDetailSavedEventArgs
                {
                    ObjectID = ObjectID,
                    ObjectInstance = ObjectInstance,
                    ObjectType = ObjectInstance.GetType().Name
                });
            });
        }


        #region Camera
        private void OpenFullscreen_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int camId && DataContext is TransactionViewModel vm)
            {
                vm.SelectedCameraId = camId;

                foreach (var item in FindVisualChildren<ContentPresenter>(icCameraList))
                {
                    if (item.Content is Camera cam)
                    {
                        var border = FindChild<Border>(item, "CameraBorder");
                        var btnOpen = FindChild<Button>(item, "btnOpen");
                        var btnClose = FindChild<Button>(item, "btnClose");

                        if (cam.CameraID == camId)
                        {
                            //border.Width = 650;
                            //border.Height = 400;
                            AdjustCameraWidth(border, true);
                            Panel.SetZIndex(border, 10);

                            if (btnOpen != null) btnOpen.Visibility = Visibility.Collapsed;
                            if (btnClose != null) btnClose.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            //border.Width = 310;
                            //border.Height = 190;
                            AdjustCameraWidth(border, false);
                            Panel.SetZIndex(border, 1);

                            if (btnOpen != null) btnOpen.Visibility = Visibility.Visible;
                            if (btnClose != null) btnClose.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
        }
        private void CloseFullscreen_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TransactionViewModel vm)
            {
                vm.SelectedCameraId = null;

                foreach (var item in FindVisualChildren<ContentPresenter>(icCameraList))
                {
                    if (item.Content is Camera cam)
                    {
                        var border = FindChild<Border>(item, "CameraBorder");
                        var btnOpen = FindChild<Button>(item, "btnOpen");
                        var btnClose = FindChild<Button>(item, "btnClose");

                        if (border != null)
                        {
                            // border.Width = 310;
                            //border.Height = 190;
                            AdjustCameraWidth(border, false);
                            Panel.SetZIndex(border, 1);
                        }

                        if (btnOpen != null) btnOpen.Visibility = Visibility.Visible;
                        if (btnClose != null) btnClose.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild && ((FrameworkElement)child).Name == childName)
                {
                    return typedChild;
                }

                var result = FindChild<T>(child, childName);
                if (result != null) return result;
            }

            return null;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T t) yield return t;

                foreach (var childOfChild in FindVisualChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }

        private void CameraBorder_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Border border && DataContext is TransactionViewModel vm)
            {
                DependencyObject parent = border;
                while (parent != null && !(parent is ItemsControl))
                {
                    parent = VisualTreeHelper.GetParent(parent);
                }

                if (parent is ItemsControl itemsControl && itemsControl.ActualWidth > 0)
                {
                    double fullWidth = itemsControl.ActualWidth;

                    if (border.Tag is int camId)
                    {
                        bool isFullscreen = vm.SelectedCameraId == camId;

                        // If fullscreen, take full width, else half
                        double marginPadding = 32; // Account for margins/padding (adjust if needed)
                        double targetWidth = isFullscreen
                            ? fullWidth - marginPadding             // 100%
                            : (fullWidth / 2) - marginPadding;

                        border.Width = Math.Max(250, targetWidth);
                    }
                }
            }
        }

        private void AdjustCameraWidth(Border border, bool isFullscreen)
        {
            if (border == null) return;

            // Find the parent ItemsControl to get available width
            DependencyObject parent = border;
            while (parent != null && !(parent is ItemsControl))
                parent = VisualTreeHelper.GetParent(parent);

            if (parent is ItemsControl itemsControl)
            {
                double availableWidth = itemsControl.ActualWidth;

                double marginPadding = 32; // Account for margins/padding 
                double targetWidth = isFullscreen
                    ? availableWidth - marginPadding             // 100%
                    : (availableWidth / 2) - marginPadding;      // 50%

                border.Width = Math.Max(250, targetWidth); // Set a minimum width
            }

            //border.Height = isFullscreen ? 380 : 190; // Height 
        }

        #endregion

        private void btnLeftAnpr_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                viewModel.GetAnprNumber("Left");
            }
            catch { }
        }

        private void btnRightAnpr_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                viewModel.GetAnprNumber("Right");
            }
            catch { }
        }
    }
}