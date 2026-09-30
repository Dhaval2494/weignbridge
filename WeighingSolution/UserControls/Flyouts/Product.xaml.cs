using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.Events;
using WeighingSolution.ViewModel;

namespace WeighingSolution.UserControls.Flyouts
{
    /// <summary>
    /// Interaction logic for Product.xaml
    /// </summary>
    public partial class Product : UserControl
    {
        vmProductManagement viewModel;
        private String prodName = null;
        public int IntProductid = 0;
        private void SetupPage()
        {
            this.Unloaded += UserControl_Unloaded;
            this.Loaded += UserControl_Loaded;
            viewModel = new vmProductManagement(IntProductid);
            this.DataContext = viewModel;
        }
        public Product()
        {
            InitializeComponent();
            SetupPage();
        }
        public Product(string ProductName)
        {
            InitializeComponent();
            prodName = ProductName;
            SetupPage();
        }

        public Product(int Productid, string ProductName)
        {
            InitializeComponent();
            prodName = ProductName;
            IntProductid = Productid;
            SetupPage();
        }
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();

            if (!string.IsNullOrEmpty(prodName))
                viewModel.Model.ProductName = prodName;

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
