using MahApps.Metro.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.Events;
using WeighingSolution.ViewModel;

namespace WeighingSolution.UserControls.Flyouts
{
    /// <summary>
    /// Interaction logic for Driver.xaml
    /// </summary>
    public partial class Driver : UserControl
    {
        vmDriverManagement viewModel;
        private String FnmLnm = null;
        private void SetupPage()
        {
            this.Unloaded += UserControl_Unloaded;
            this.Loaded += UserControl_Loaded;
            viewModel = new vmDriverManagement(0);
            this.DataContext = viewModel;
        }
        public Driver()
        {
            InitializeComponent();
            SetupPage();
        }
        public Driver(string FirstNameLastName)
        {
            InitializeComponent();
            FnmLnm = FirstNameLastName;
            SetupPage();
        }
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();
            string[] DriverName = { };
            if (!string.IsNullOrEmpty(FnmLnm))
            {
                DriverName = FnmLnm.Split(' ').ToArray();
                if (DriverName != null && DriverName.Length > 0 && !string.IsNullOrEmpty(DriverName[0]))
                    viewModel.Model.FirstName = DriverName[0];
                if (DriverName != null && DriverName.Length > 1 && !string.IsNullOrEmpty(DriverName[1]))
                    viewModel.Model.LastName = DriverName[1];
            }
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
    }
}
