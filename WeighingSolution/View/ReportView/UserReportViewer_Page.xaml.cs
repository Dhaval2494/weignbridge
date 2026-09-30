using Microsoft.Reporting.WinForms;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;



namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for UserReportViewer_Page.xaml
    /// </summary>
    public partial class UserReportViewer_Page : Page
    {
        string nm = "";
        bool Actv = false;
        public UserReportViewer_Page()
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
            if (cbActive.IsChecked == true)
            {
                Actv = true;
                strFilter += " Active is True" + " and ";
            }
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
            string query;
            DataTable dt = new DataTable();
            if (Actv == true)
                query = "Select * From UserMaster where ( @nm = N'' or FirstName LIKE'%'+@nm+'%'  or LastName LIKE'%'+@nm+'%'  or UserName LIKE'%'+@nm+'%' ) AND (Active=@Actv) AND (UserName !='svt_sauser')";
            else
                query = "Select * From UserMaster where ( @nm = N'' or FirstName LIKE'%'+@nm+'%'  or LastName LIKE'%'+@nm+'%'  or UserName LIKE'%'+@nm+'%' ) AND (UserName !='svt_sauser') ";
            SqlConnection con = new SqlConnection(System.Windows.Application.Current.Properties["DefalutConnectionString"].ToString());
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@nm", SqlDbType.VarChar).Value = nm;
            cmd.Parameters.AddWithValue("@Actv", SqlDbType.Bit).Value = Actv;
            con.Open();
            dt.Load(cmd.ExecuteReader());
            con.Close();
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "UserDataSet"; // Name of the DataSet we set in .rdlc
            reportDataSource.Value = dt;
            if (dt.Rows.Count > 0)
            {
                winform.Visibility = Visibility.Visible;
                txtmsg.Visibility = Visibility.Collapsed;
                reportViewer.LocalReport.ReportPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Report\\Master Report\\UserReport.rdlc";
                reportViewer.LocalReport.DataSources.Clear();
                ReportParameter rp = new ReportParameter("FilterInfo", strFilter);
                this.reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp });
                ReportParameter rp1 = new ReportParameter("CompanyName", Common.getCompanyName());
                this.reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp1 });
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
