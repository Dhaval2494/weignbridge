using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;


namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for AuditReport.xaml
    /// </summary>
    public partial class AuditReport : Page
    {
        public weighbridgeEntities ctx = new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString());
        List<DropdownListData> ddlDataUser = new List<DropdownListData>();
        public DateTime Sdate;
        public DateTime Edate;
        string Tablemaster = "";
        string Operation = "";
        int UserDetail = 0;
        string strFilter = string.Empty;

        public AuditReport()
        {
            InitializeComponent();
            List<string> lstAudit = ctx.Database.SqlQuery<string>("select distinct TableMaster from AuditData").ToList();
            lstAudit.Insert(0, "Select Table Master");
            Cmbfilter.ItemsSource = lstAudit;
            List<string> lstOperation = ctx.Database.SqlQuery<string>("select distinct Operation from AuditData").ToList();
            lstOperation.Insert(0, "Select Operation");
            CmbOperation.ItemsSource = lstOperation;

            DropdownListData objDataInitial = new DropdownListData();
            objDataInitial.DataTextField = "Select FirstName LastName-UserName";
            objDataInitial.DataValueField = 0;
            ddlDataUser.Add(objDataInitial);
            List<UserMaster> lstuser = ctx.UserMasters.Where(x => x.UserName != "svt_sauser" && x.Active).ToList();
            foreach (UserMaster item in lstuser)
            {
                DropdownListData objData = new DropdownListData();
                objData.DataTextField = item.FirstName + " " + item.LastName + " - " + item.UserName;
                objData.DataValueField = item.UserID;
                ddlDataUser.Add(objData);
            }
            CmbUserDetail.ItemsSource = ddlDataUser;
            CmbUserDetail.DisplayMemberPath = "DataTextField";
            CmbUserDetail.SelectedValuePath = "DataValueField";
            CmbUserDetail.SelectedValue = "0";

            CultureInfo ci = new CultureInfo(Thread.CurrentThread.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = Constant.SelectedDateFormat;
            Thread.CurrentThread.CurrentCulture = ci;
            datepicStart.Text = DateTime.Now.Date.AddDays(-15).ToString();
            datepicend.Text = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59).ToString();
            strFilter += "Transaction Date between " + datepicStart.SelectedDate + " To " + datepicend.SelectedDate;
            if (!string.IsNullOrEmpty(strFilter))
            {
                strFilter = "(Filtered for: " + strFilter + " )";
            }
            WSCommon.DisableWheelNavigation(reportViewer);
            ConnectData(strFilter);
        }

        private void btnfilter_Click(object sender, RoutedEventArgs e)
        {
            strFilter = string.Empty;
            if (Cmbfilter.SelectedIndex > 0)
            {
                Tablemaster = Cmbfilter.SelectedItem.ToString();
                strFilter = "Table Master is " + Tablemaster + " and ";
            }
            if (CmbOperation.SelectedIndex > 0)
            {
                Operation = CmbOperation.SelectedItem.ToString();
                strFilter += "Operation is " + Operation + " and ";
            }
            if (CmbUserDetail.SelectedIndex > 0)
            {
                DropdownListData UserNamedetail = CmbUserDetail.SelectedItem as DropdownListData;
                if (UserNamedetail != null)
                {
                    strFilter += "User is " + UserNamedetail.DataTextField + " and ";
                    UserDetail = UserNamedetail.DataValueField;
                }
            }
            if (datepicStart.SelectedDate == null || datepicend.SelectedDate == null)
            {
                datepicStart.Text = DateTime.Now.AddDays(-15).ToString();
                datepicend.Text = DateTime.Now.Date.ToString();
            }
            strFilter += "Transaction Date between " + datepicStart.SelectedDate + " To " + datepicend.SelectedDate + " and";
            if (!string.IsNullOrEmpty(strFilter))
            {
                strFilter = "(Filtered for: " + ReplaceLastOccurrence(strFilter, " and", "") + " )";
            }
            ConnectData(strFilter);
        }

        private void btnBackToReport_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/ReportView/Reports_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
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
            Sdate = datepicStart.SelectedDate.Value.Date;
            Edate = datepicend.SelectedDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            DataTable dt = new DataTable();
            String Query;
            // Query = "Select * From AuditData where (TranDate between @Sdate AND @Edate) AND (@Operation=N'' OR Operation=@Operation) AND (@UserDetail=0 OR TranUserID=@UserDetail )) AND (@TableMaster=N'' OR TableMaster=@Tablemaster)";
            if (Cmbfilter.SelectedIndex > 0 && CmbOperation.SelectedIndex > 0 && CmbUserDetail.SelectedIndex > 0)
                Query = "Select * From AuditData where  TableMaster=@Tablemaster AND Operation=@Operation AND TranUserID=@UserDetail AND TranDate between @Sdate AND @Edate";
            else if (Cmbfilter.SelectedIndex > 0 && CmbOperation.SelectedIndex > 0)
                Query = "Select * From AuditData where TableMaster=@Tablemaster AND Operation=@Operation AND TranDate between @Sdate AND @Edate";
            else if (CmbOperation.SelectedIndex > 0 && CmbUserDetail.SelectedIndex > 0)
                Query = "Select * From AuditData where  Operation=@Operation AND TranUserID=@UserDetail AND TranDate between @Sdate AND @Edate";
            else if (CmbUserDetail.SelectedIndex > 0 && Cmbfilter.SelectedIndex > 0)
                Query = "Select * From AuditData where TranUserID=@UserDetail AND TableMaster=@Tablemaster AND TranDate between @Sdate AND @Edate";
            else if (Cmbfilter.SelectedIndex > 0)
                Query = "Select * From AuditData where TableMaster=@Tablemaster AND TranDate between @Sdate AND @Edate";
            else if (CmbOperation.SelectedIndex > 0)
                Query = "Select * From AuditData where  Operation=@Operation AND TranDate between @Sdate AND @Edate";
            else if (CmbUserDetail.SelectedIndex > 0)
                Query = "Select * From AuditData where TranUserID=@UserDetail AND TranDate between @Sdate AND @Edate";
            else
                Query = "Select TableMaster, Operation,UserActivity,TranDate From AuditData where TranDate between @Sdate AND @Edate";

            Query = Query + " ORDER BY TranDate DESC";
            SqlConnection con = new SqlConnection(System.Windows.Application.Current.Properties["DefalutConnectionString"].ToString());
            SqlCommand cmd = new SqlCommand(Query, con);
            cmd.Parameters.AddWithValue("@Tablemaster", SqlDbType.VarChar).Value = Tablemaster;
            cmd.Parameters.AddWithValue("@Operation", SqlDbType.VarChar).Value = Operation;
            cmd.Parameters.AddWithValue("@UserDetail", SqlDbType.VarChar).Value = UserDetail;
            cmd.Parameters.AddWithValue("@Sdate", SqlDbType.DateTime).Value = Sdate;
            cmd.Parameters.AddWithValue("@Edate", SqlDbType.DateTime).Value = Edate;
            con.Open();
            dt.Load(cmd.ExecuteReader());
            con.Close();
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "AuditDataSet"; // Name of the DataSet we set in .rdlc
            reportDataSource.Value = dt;
            if (dt.Rows.Count > 0)
            {
                winform.Visibility = Visibility.Visible;
                txtmsg.Visibility = Visibility.Collapsed;
                reportViewer.LocalReport.ReportPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Report\\Audit Report\\AuditReport.rdlc";
                reportViewer.LocalReport.DataSources.Clear();
                ReportParameter rp = new ReportParameter("DateFormatParameter", Constant.DateTimeFormat);
                this.reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp });
                ReportParameter rp1 = new ReportParameter("FilterInfo", strFilter);
                this.reportViewer.LocalReport.SetParameters(new ReportParameter[] { rp1 });
                ReportParameter rp2 = new ReportParameter("CompanyName", Common.getCompanyName());
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
    }
    #endregion
}



