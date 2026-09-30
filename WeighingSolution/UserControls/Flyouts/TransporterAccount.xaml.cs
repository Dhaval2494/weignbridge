using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.Events;
using WeighingSolution.ViewModel;

namespace WeighingSolution.UserControls.Flyouts
{
    /// <summary>
    /// Interaction logic for TransporterAccount.xaml
    /// </summary>
    public partial class TransporterAccount : UserControl
    {
        vmAccountManagement viewModel;
        private String compName = null;
        private bool isAccount;
        private void SetupPage()
        {
            this.Unloaded += UserControl_Unloaded;
            this.Loaded += UserControl_Loaded;
            viewModel = new vmAccountManagement(0);
            this.DataContext = viewModel;
        }
        public TransporterAccount(bool IsAccount)
        {
            InitializeComponent();
            isAccount = IsAccount;
            SetupPage();
        }
        public TransporterAccount(bool IsAccount, string CompanyName)
        {
            InitializeComponent();
            compName = CompanyName;
            isAccount = IsAccount;
            SetupPage();
        }
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();

            if (!string.IsNullOrEmpty(compName))
                viewModel.Model.CompanyName = compName;
            if (isAccount)
            {
                viewModel.Model.IsAccount = true;
                cbaccount.IsEnabled = false;
            }
            else
            {
                viewModel.Model.IsTransporter = true;
                cbTransporter.IsEnabled = false;
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
