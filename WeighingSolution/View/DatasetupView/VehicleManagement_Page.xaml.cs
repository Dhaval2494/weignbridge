using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;
using WeighingSolution.Events;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for VehicleManagement_Page.xaml
    /// </summary>
    public partial class VehicleManagement_Page : Page
    {
        public int IntVehicleid = 0;
        vmVehicleManagement viewModel;
        private void SetupPage()
        {
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            viewModel = new vmVehicleManagement(IntVehicleid);
            this.DataContext = viewModel;
        }
        private void GoBack(object obj)
        {
            Uri uri = new Uri("/View/DatasetupView/VehicleManagementList_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        public VehicleManagement_Page()
        {
            InitializeComponent();
            SetupPage();
        }
        public VehicleManagement_Page(int VehicleId)
        {
            InitializeComponent();
            IntVehicleid = VehicleId;
            SetupPage();
        }
        private async void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();
            viewModel.EventAggregator.GetEvent<AfterDetailSaveCancelledEvent>().Subscribe(GoBack);
            viewModel.EventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(GoBack);
        }
        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.EventAggregator.GetEvent<AfterDetailSaveCancelledEvent>().Unsubscribe(GoBack);
            viewModel.EventAggregator.GetEvent<AfterDetailSavedEvent>().Unsubscribe(GoBack);
            viewModel.Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        //private void txtVehicleNo_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (!string.IsNullOrWhiteSpace(viewModel.Model.VehicleNumber))
        //    {
        //        viewModel.Model.VehicleNumber = viewModel.Model.VehicleNumber.Replace(" ", "");
        //        viewModel.Model.VehicleNumber = viewModel.Model.VehicleNumber.Trim();
        //    }
        //    txtVehicleNo.CaretIndex = Int32.MaxValue;
        //}

        private void txtVehicleNo_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(viewModel.Model.VehicleNumber))
            {
                viewModel.Model.VehicleNumber = viewModel.Model.VehicleNumber.Replace(" ", "");
                viewModel.Model.VehicleNumber = viewModel.Model.VehicleNumber.Trim();
                if (Common.DisableSpecialCharactersInVehicle)
                    viewModel.Model.VehicleNumber = Regex.Replace(viewModel.Model.VehicleNumber, @"[^0-9a-zA-Z]+", "");
            }
        }

        private void txtVehicleNo_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.Tab)
                {
                    e.Handled = false;
                }
                else if (Common.VehicleNumberLength <= txtVehicleNo.Text.Length)
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
    }
}
