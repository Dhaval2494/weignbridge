using Microsoft.Reporting.WinForms;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for TransactionDetailReport.xaml
    /// </summary>
    public partial class TransactionDetailReport : Page
    {
        public TransactionDetailReport()
        {
            InitializeComponent();
        }

        public TransactionDetailReport(DataTable dt, string strFilter)
        {
            InitializeComponent();
            WSCommon.DisableWheelNavigation(reportViewer);
            DataTable dtable = dt;

            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "TransactionDetailDataSet"; // Name of the DataSet we set in .rdlc
            reportDataSource.Value = dtable;

            reportViewer.LocalReport.ReportPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Report\\Transaction Report\\TransactionReport.rdlc";
            reportViewer.LocalReport.DataSources.Clear();
            ReportParameter rp = new ReportParameter("DateFormatParameter", Constant.DateTimeFormat);
            this.reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp });
            ReportParameter rp1 = new ReportParameter("FilterInfo", strFilter);
            ReportParameter hsPrintPermission = new ReportParameter("HasManualPrintPermission", Common.ValidatePageRequest(PageEntity.PrintStarForManualEntry, PageAccess.Execute).ToString());
            this.reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp1, hsPrintPermission });
            ReportParameter rp2 = new ReportParameter("CompanyName", Common.getCompanyName());
            this.reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp2 });
            ReportParameter rp3 = new ReportParameter("DecimalFormat", Constant.DecimalFormat);
            this.reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp3 });
            ReportParameter[] reportParameterCollection = new ReportParameter[20];
            ReportParameter[] cmfVisibleParamCollection = new ReportParameter[26];
            using (weighbridgeEntities ctx = new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()))
            {
                MasterField[] lstmstfld = ctx.MasterFields.ToArray();
                for (int i = 1; i <= lstmstfld.Length; i++)
                {
                    int j = i + 20;
                    cmfVisibleParamCollection[j - 1] = new ReportParameter("mfVisible" + i, (!lstmstfld[i - 1].Isvisible).ToString());
                }

                CustomField[] lstCstmfld = ctx.CustomFields.ToArray();
                for (int i = 1; i <= lstCstmfld.Length; i++)
                {
                    cmfVisibleParamCollection[i - 1] = new ReportParameter("cfVisible" + i, (!lstCstmfld[i - 1].IsVisible).ToString());
                    if (lstCstmfld[i - 1].DisplayName != "")
                        reportParameterCollection[i - 1] = new ReportParameter("DisplayField" + i, lstCstmfld[i - 1].DisplayName);
                    else
                        reportParameterCollection[i - 1] = new ReportParameter("DisplayField" + i, "Field" + i);
                }
            }

            reportViewer.LocalReport.SetParameters(cmfVisibleParamCollection);
            reportViewer.LocalReport.SetParameters(reportParameterCollection);
            reportViewer.LocalReport.DataSources.Add(reportDataSource);
            reportViewer.RefreshReport();
        }
        private void btnfilterBack_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new TransactionReportFilterPanel());
        }
    }
}
