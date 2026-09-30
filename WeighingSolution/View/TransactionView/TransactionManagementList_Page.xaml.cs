using MahApps.Metro.Controls;
using MahApps.Metro.SimpleChildWindow;
using PluginInterfaces;
using Prism.Events;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WeighingSolution.DataLayer;
using WeighingSolution.Events;
using WeighingSolution.ViewModel;
using System.Linq;

namespace WeighingSolution.View
{
    public partial class TransactionManagementList_Page : Page
    {
        TransactionListViewModel viewModel;
        UserControls.TransactionReceiptPopup receiptPopup;
        protected readonly IEventAggregator EventAggregator = AppEventManager.Instance.EventAggregator;

        public TransactionManagementList_Page()
        {
            InitializeComponent();
            viewModel = new TransactionListViewModel(this);
            DataContext = viewModel;
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;

            if (Common.ValidatePageRequest(PageEntity.Transaction, PageAccess.Create))
                txtbAddTransaction.Visibility = Visibility.Visible;
            if (!Common.ValidatePageRequest(PageEntity.Transaction, PageAccess.Create))
                DatagridTransactionManagement.Columns[5].Visibility = Visibility.Collapsed;
            if (!Common.ValidatePageRequest(PageEntity.VoidTransaction, PageAccess.Execute))
                DatagridTransactionManagement.Columns[6].Visibility = Visibility.Collapsed;
            //if (Common.ValidatePageRequest(PageEntity.PrintReceipt, PageAccess.Execute))
            //    btnPrintTransaction.Visibility = Visibility.Visible;
        }
        private void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.OnFilterData();
        }

        private void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            MetroWindow parentwnd = ((MetroWindow)App.Current.MainWindow);
            Frame MainFrame = parentwnd.FindName("MainFrame") as Frame;
            MainFrame.Content = new View.ReceiptPreview(viewModel.SelectedTransaction.ReceiptTicketID);
        }
        public void btnViewDetail_Click(object sender, RoutedEventArgs e)
        {
            if (receiptPopup != null && receiptPopup.IsOpen)
                receiptPopup.Close();
            else
            {
                receiptPopup = new UserControls.TransactionReceiptPopup();
                receiptPopup.ClosingFinished -= ReceiptPopup_ClosingFinished;
                receiptPopup.ClosingFinished += ReceiptPopup_ClosingFinished;
                receiptPopup.DataContext = this.DataContext;
            }
            
            ChildWindowManager.ShowChildWindowAsync(App.Current.MainWindow, receiptPopup);
            EventAggregator.GetEvent<ViewTransactionDetailEvent>().Publish(new ViewTransactionDetailEventArgs { tranData = viewModel.SelectedTransaction });
        }

        private void ReceiptPopup_ClosingFinished(object sender, RoutedEventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            //FocusManager.SetFocusedElement(this, DatagridTransactionManagement.Items[DatagridTransactionManagement.SelectedIndex]);
            DataGridRow row = (DataGridRow)DatagridTransactionManagement.ItemContainerGenerator.ContainerFromIndex(DatagridTransactionManagement.SelectedIndex);
            row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            //DatagridTransactionManagement.Focus();
        }


        private void btnCreateTransaction_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/TransactionView/TransactionManagement.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        private void btnprint_Click_1(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(DatagridTransactionManagement, "Transaction");
            }
        }

        private void Expander_Toggle(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as System.Windows.Media.Visual; vis != null; vis = System.Windows.Media.VisualTreeHelper.GetParent(vis) as System.Windows.Media.Visual)
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                }
        }

        private void ucPager_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}