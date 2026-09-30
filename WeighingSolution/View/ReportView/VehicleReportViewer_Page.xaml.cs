using Microsoft.Reporting.WinForms;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;


namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for VehicleReportViewer_Page.xaml
    /// </summary>
    public partial class VehicleReportViewer_Page : Page
    {
        string nm = "";
        public VehicleReportViewer_Page()
        {
            InitializeComponent();
            WSCommon.DisableWheelNavigation(reportViewer);
            ConnectData("");
        }

        private void btnfilter_Click(object sender, RoutedEventArgs e)
        {
            string strFilter = string.Empty;
            nm = txtfilter.Text;
            if (!string.IsNullOrEmpty(nm))
                strFilter = nm + " and ";
            if (!string.IsNullOrEmpty(strFilter))
            {
                strFilter = "( Filtered for:  " + ReplaceLastOccurrence(strFilter, " and", "") + " )";
            }
            ConnectData(strFilter);
        }

        #region Helper Function
        public static string ReplaceLastOccurrence(string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find);
            if (place == -1)
                return Source;

            string result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }
        public void ConnectData(string strFilter)
        {
            DataTable dt = new DataTable();
            string query = "Select * From Vehicle where ( @nm = N'' or VehicleNumber LIKE'%'+@nm+'%'  or VehicleType LIKE'%'+@nm+'%'  or TareWeight LIKE'%'+@nm+'%')";
            SqlConnection con = new SqlConnection(System.Windows.Application.Current.Properties["DefalutConnectionString"].ToString());
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@nm", SqlDbType.VarChar).Value = nm;
            con.Open();
            dt.Load(cmd.ExecuteReader());
            con.Close();
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "VehicleDataSet"; // Name of the DataSet we set in .rdlc
            reportDataSource.Value = dt;
            if (dt.Rows.Count > 0)
            {
                winform.Visibility = Visibility.Visible;
                txtmsg.Visibility = Visibility.Collapsed;
                reportViewer.LocalReport.ReportPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Report\\Master Report\\VehicleReport.rdlc";
                reportViewer.LocalReport.DataSources.Clear();
                ReportParameter rp = new ReportParameter("FilterInfo", strFilter);
                this.reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp });
                ReportParameter rp1 = new ReportParameter("CompanyName", Common.getCompanyName());
                this.reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp1 });
                ReportParameter rp2 = new ReportParameter("DecimalFormat", Constant.DecimalFormat);
                this.reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp2 });
                reportViewer.LocalReport.DataSources.Add(reportDataSource);
                reportViewer.RefreshReport();
            }
            else
            {
                winform.Visibility = Visibility.Collapsed;
                txtmsg.Visibility = Visibility.Visible;
            }
        }
        #endregion
    }
}
