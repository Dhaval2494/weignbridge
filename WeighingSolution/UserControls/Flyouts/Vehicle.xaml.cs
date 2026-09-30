using MahApps.Metro.Controls;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WeighingSolution.DataLayer;
using WeighingSolution.Events;
using WeighingSolution.ViewModel;

namespace WeighingSolution.UserControls.Flyouts
{
    /// <summary>
    /// Interaction logic for Vehicle.xaml
    /// </summary>
    public partial class Vehicle : UserControl
    {
        public int IntVehicleid = 0;
        private String VehicleNo = null;
        vmVehicleManagement viewModel;
        private void SetupPage()
        {
            this.Unloaded += UserControl_Unloaded;
            this.Loaded += UserControl_Loaded;
            viewModel = new vmVehicleManagement(IntVehicleid);
            this.DataContext = viewModel;
        }
        public Vehicle()
        {
            InitializeComponent();
            SetupPage();
        }
        public Vehicle(int VehicleId)
        {
            InitializeComponent();
            IntVehicleid = VehicleId;
            SetupPage();
        }
        public Vehicle(string VehicleNumber)
        {
            InitializeComponent();
            VehicleNo = VehicleNumber;
            SetupPage();
        }
        public Vehicle(int VehicleId, string VehicleNumber)
        {
            InitializeComponent();
            IntVehicleid = VehicleId;
            VehicleNo = VehicleNumber;
            SetupPage();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();

            if (!string.IsNullOrEmpty(VehicleNo))
                viewModel.Model.VehicleNumber = VehicleNo;

            viewModel.EventAggregator.GetEvent<AfterDetailSaveCancelledEvent>().Subscribe(CloseFlyout);
            viewModel.EventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(CloseFlyout);
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.EventAggregator.GetEvent<AfterDetailSaveCancelledEvent>().Unsubscribe(CloseFlyout);
            viewModel.EventAggregator.GetEvent<AfterDetailSavedEvent>().Unsubscribe(CloseFlyout);
            viewModel.Dispose();
        }
        private void CloseFlyout(object obj)
        {
            try
            {
                DependencyObject ucParent = this.Parent;
                while (!(ucParent is Flyout))
                {
                    ucParent = LogicalTreeHelper.GetParent(ucParent);
                }
                if (ucParent != null)
                    ((Flyout)ucParent).IsOpen = false;
            }
            catch { }
        }

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

        private void txtVehicleNo_KeyDown(object sender, KeyEventArgs e)
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

        //private void txtVehicleNo_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if(!string.IsNullOrWhiteSpace(viewModel.Model.VehicleNumber))
        //    {
        //        viewModel.Model.VehicleNumber = viewModel.Model.VehicleNumber.Replace(" ", "");
        //        viewModel.Model.VehicleNumber = viewModel.Model.VehicleNumber.Trim();
        //    }
        //    txtVehicleNo.CaretIndex = Int32.MaxValue;
        //}
    }
}
