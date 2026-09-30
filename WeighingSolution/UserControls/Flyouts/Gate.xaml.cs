using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.Events;
using WeighingSolution.ViewModel;

namespace WeighingSolution.UserControls.Flyouts
{
    /// <summary>
    /// Interaction logic for Gate.xaml
    /// </summary>
    public partial class Gate : UserControl
    {
        vmGateManagement viewModel;
        private String gateName = null;
        private void SetupPage()
        {
            this.Unloaded += UserControl_Unloaded;
            this.Loaded += UserControl_Loaded;
            viewModel = new vmGateManagement(0);
            this.DataContext = viewModel;
        }
        public Gate()
        {
            InitializeComponent();
            SetupPage();
        }
        public Gate(string GateName)
        {
            InitializeComponent();
            gateName = GateName;
            SetupPage();
        }
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();

            if (!string.IsNullOrEmpty(gateName))
                viewModel.Model.GateName = gateName;

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
