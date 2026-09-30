using System;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for Reports_Page.xaml
    /// </summary>
    public partial class Reports_Page : Page
    {
        public Reports_Page()
        {
            InitializeComponent();
            int RowCount = 0;
            int ColumnCount = 0;
            if (Common.ValidatePageRequest(PageEntity.MasterReport, PageAccess.Read))
            {
                gbMasterReport.Visibility = Visibility.Visible;
                Grid.SetColumn(gbMasterReport, ColumnCount);
                Grid.SetRow(gbMasterReport, RowCount);
                ColumnCount++;
            }
            if (!Constant.IsBasicversion)
            {
                if (Common.ValidatePageRequest(PageEntity.AuditReport, PageAccess.Read))
                {
                    gbAuditReport.Visibility = Visibility.Visible;
                    Grid.SetColumn(gbAuditReport, ColumnCount);
                    Grid.SetRow(gbAuditReport, RowCount);
                    ColumnCount++;
                }
            }
            if (Common.ValidatePageRequest(PageEntity.TransactionReport, PageAccess.Read))
            {
                gbTransactionReport.Visibility = Visibility.Visible;
                Grid.SetColumn(gbTransactionReport, ColumnCount);
                Grid.SetRow(gbTransactionReport, RowCount);
                ColumnCount++;
            }
        }

        private void btnGoToMasterReport_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/ReportView/ReportView_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        private void btnGoToAuditReport_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/ReportView/AuditReport.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }

        private void btnGoToTransactionReport_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/ReportView/TransactionReportFilterPanel.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
    }
}
