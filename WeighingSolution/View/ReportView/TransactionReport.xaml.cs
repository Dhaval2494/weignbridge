using Microsoft.Reporting.WinForms;
using Newtonsoft.Json;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for TransactionReport.xaml
    /// </summary>
    public partial class TransactionReport : Page
    {

        public TransactionReport()
        {
            InitializeComponent();
        }

        public TransactionReport(DataTable dt, string strFilter)
        {
            InitializeComponent();
            WSCommon.DisableWheelNavigation(reportViewer);
            try
            {

                DataTable dtable = dt;
                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "TransactionDataSet"; // Name of the DataSet we set in .rdlc
                reportDataSource.Value = dtable;
                reportViewer.LocalReport.ReportPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Report\\Transaction Report\\TransactionSummary.rdlc";
                reportViewer.LocalReport.DataSources.Clear();
                ReportParameter rp = new ReportParameter("DateFormatParameter", Constant.DateTimeFormat);
                ReportParameter hsPrintPermission = new ReportParameter("HasManualPrintPermission", Common.ValidatePageRequest(PageEntity.PrintStarForManualEntry, PageAccess.Execute).ToString());
                this.reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp, hsPrintPermission });
                ReportParameter rp1 = new ReportParameter("FilterInfo", strFilter);
                this.reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp1 });
                ReportParameter rp2 = new ReportParameter("CompanyName", Common.getCompanyName());
                this.reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp2 });
                ReportParameter rp3 = new ReportParameter("DecimalFormat", Constant.DecimalFormat);
                this.reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp3 });

                using (weighbridgeEntities ctx = new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()))
                {
                    MasterField[] lstmstfld = ctx.MasterFields.ToArray();
                    for (int i = 0; i < lstmstfld.Length; i++)
                    {
                        string MFieldName = lstmstfld[i].FieldName;
                        string Name = MFieldName + "Visible";

                        switch (MFieldName)
                        {
                            case "Account":
                                {
                                    ReportParameter rpv = new ReportParameter(Name, (!lstmstfld[i].Isvisible).ToString());
                                    this.reportViewer.LocalReport.SetParameters(new ReportParameter[] { rpv });
                                    break;
                                }

                            case "Product":
                                {
                                    ReportParameter rpv = new ReportParameter(Name, (!lstmstfld[i].Isvisible).ToString());
                                    this.reportViewer.LocalReport.SetParameters(new ReportParameter[] { rpv });
                                    break;
                                }

                            case "Charges":
                                {
                                    ReportParameter rpv = new ReportParameter(Name, (!lstmstfld[i].Isvisible).ToString());
                                    this.reportViewer.LocalReport.SetParameters(new ReportParameter[] { rpv });
                                    break;
                                }

                            default:
                                break;
                        }
                    }
                }

                reportViewer.LocalReport.DataSources.Add(reportDataSource);
                reportViewer.RefreshReport();
            }
            catch (Exception Ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(Ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\TransactionReportError.txt", json);
            }
        }

        private void btnfilterback_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new TransactionReportFilterPanel());
        }
    }
}

