using Microsoft.Reporting.WinForms;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;


namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for AccountReportViewer_Page.xaml
    /// </summary>
    public partial class AccountReportViewer_Page : Page
    {
        string Fnm = "";
        bool Acnt = false;
        bool Trans = false;
        bool Actv = false;

        public AccountReportViewer_Page()
        {
            InitializeComponent();
            WSCommon.DisableWheelNavigation(reportViewer);
            ConnectData("");
        }

        private void btnfilter_Click(object sender, RoutedEventArgs e)
        {
            Acnt = false;
            Trans = false;
            Actv = false;
            Fnm = txtfilter.Text;
            string strFilter = string.Empty;
            if (!string.IsNullOrEmpty(Fnm))
                strFilter = "Account  Name Like " + Fnm + " and ";
            if (cbAccount.IsChecked == true)
            {
                Acnt = true;
                strFilter += "IsAccount is True" + " and ";
            }
            if (cbTransporter.IsChecked == true)
            {
                Trans = true;
                strFilter += "IsTransorter is True" + " and ";
            }
            if (cbActive.IsChecked == true)
            {
                Actv = true;
                strFilter += "Active is True" + " and ";
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
            DataTable dt = new DataTable();
            string query;
            if ((Acnt == true && Trans == true && Actv == true))
            {
                query = "Select * From Account where ( @Fnm = N'' or FirstName LIKE'%'+@Fnm+'%'  or LastName LIKE'%'+@Fnm+'%'  or CompanyName LIKE'%'+@Fnm+'%') AND(IsAccount=@Acnt)  AND(ISTransporter=@Trans) AND (Active=@Actv) ";
            }
            else if ((Acnt == true) && (Trans == true))
            {
                query = "Select * From Account where ( @Fnm = N'' or FirstName LIKE'%'+@Fnm+'%'  or LastName LIKE'%'+@Fnm+'%'  or CompanyName LIKE'%'+@Fnm+'%') AND (IsAccount=@Acnt)  AND(ISTransporter=@Trans)";
            }
            else if ((Trans == true) && (Actv == true))
            {
                query = "Select * From Account where ( @Fnm = N'' or FirstName LIKE'%'+@Fnm+'%'  or LastName LIKE'%'+@Fnm+'%'  or CompanyName LIKE'%'+@Fnm+'%') AND (Active=@Actv) AND(ISTransporter=@Trans)";
            }
            else if ((Acnt == true) && (Actv == true))
            {
                query = "Select * From Account where ( @Fnm = N'' or FirstName LIKE'%'+@Fnm+'%'  or LastName LIKE'%'+@Fnm+'%'  or CompanyName LIKE'%'+@Fnm+'%') AND (Active=@Actv) AND(ISAccount=@Acnt)";
            }
            else if (Acnt == true)
            {
                query = "Select * From Account where ( @Fnm = N'' or FirstName LIKE'%'+@Fnm+'%'  or LastName LIKE'%'+@Fnm+'%'  or CompanyName LIKE'%'+@Fnm+'%') AND (ISAccount=@Acnt)";
            }
            else if (Trans == true)
            {
                query = "Select * From Account where ( @Fnm = N'' or FirstName LIKE'%'+@Fnm+'%'  or LastName LIKE'%'+@Fnm+'%'  or CompanyName LIKE'%'+@Fnm+'%') AND (ISTransporter=@Trans)";
            }
            else if (Actv == true)
            {
                query = "Select * From Account where ( @Fnm = N'' or FirstName LIKE'%'+@Fnm+'%'  or LastName LIKE'%'+@Fnm+'%'  or CompanyName LIKE'%'+@Fnm+'%') AND (Active=@Actv)";
            }
            else
            {
                query = "Select * From Account  where ( @Fnm = N'' or FirstName LIKE'%'+@Fnm+'%'  or LastName LIKE'%'+@Fnm+'%'  or CompanyName LIKE'%'+@Fnm+'%')";
            }
            SqlConnection con = new SqlConnection(System.Windows.Application.Current.Properties["DefalutConnectionString"].ToString());
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Fnm", SqlDbType.VarChar).Value = Fnm;
            cmd.Parameters.AddWithValue("@Acnt", SqlDbType.Bit).Value = Acnt;
            cmd.Parameters.AddWithValue("@Trans", SqlDbType.Bit).Value = Trans;
            cmd.Parameters.AddWithValue("@Actv", SqlDbType.Bit).Value = Actv;
            con.Open();
            dt.Load(cmd.ExecuteReader());
            con.Close();
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "AccountDataSet"; // Name of the DataSet we set in .rdlc
            reportDataSource.Value = dt;
            if (dt.Rows.Count > 0)
            {
                winform.Visibility = Visibility.Visible;
                txtmsg.Visibility = Visibility.Collapsed;
                reportViewer.LocalReport.ReportPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Report\\Master Report\\AccountReport.rdlc";
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
