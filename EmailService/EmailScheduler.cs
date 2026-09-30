using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Permissions;
using System.ServiceProcess;
using WeighingSolution.DataLayer;

namespace EmailService
{
    public partial class EmailScheduler : ServiceBase
    {
        System.Timers.Timer timerEmail = new System.Timers.Timer();
        public string strFilter = "";

        public EmailScheduler()
        {
            InitializeComponent();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        [System.Diagnostics.Conditional("DEBUG_SERVICE")]
        private static void DebugMode()
        {
            System.Diagnostics.Debugger.Break();
        }

        public void onDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            timerEmail.Elapsed += new System.Timers.ElapsedEventHandler(SendEmail);
            timerEmail.Interval = 60000;  //1 Mins
            timerEmail.Start();
            timerEmail.Enabled = true;
        }

        private void SendEmail(object sender, System.Timers.ElapsedEventArgs e)
        {
            timerEmail.Enabled = false;

            //Delete 1 day Old Excel File
            DirectoryInfo d = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\WeighBridgeSolution\AttachmentFile\");
            //DirectoryInfo d = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\WeighBridgeSolution" + "\\AttachmentFile\\");
            if (!d.Exists)
            {
                d.Create();
            }
            FileInfo[] Files = d.GetFiles("*.xls"); //Getting Text files
            foreach (FileInfo file in Files)
            {
                if (file.CreationTime < DateTime.Now.AddDays(-1))
                    file.Delete();
            }
            try
            {
                using (Endel_weighbridgeEntities ctxEmail = new Endel_weighbridgeEntities(EmailServiceConstant.EmailServiceEntityConnectionString))
                {
                    List<ApplicationSetting> EmailSetting = ctxEmail.ApplicationSettings.Where(x => x.ModuleName == "EmailSetting").ToList();
                    if (EmailSetting != null && EmailSetting.Count > 0)
                    {
                        Email objEmail = new Email();
                        objEmail.SMTPServer = EmailSetting.FirstOrDefault(x => x.SettingName == "ServerName").SettingValue;
                        int port = Convert.ToInt32(EmailSetting.FirstOrDefault(x => x.SettingName == "Port").SettingValue);
                        objEmail.SMTPUserName = (EmailSetting.FirstOrDefault(x => x.SettingName == "UserName").SettingValue != "") ? EmailSetting.FirstOrDefault(x => x.SettingName == "UserName").SettingValue : "";
                        objEmail.SMTPPassword = (EmailSetting.FirstOrDefault(x => x.SettingName == "PassWord").SettingValue != "") ? Common.DecryptData(EmailSetting.FirstOrDefault(x => x.SettingName == "PassWord").SettingValue) : "";
                        objEmail.EmailFrom = (EmailSetting.FirstOrDefault(x => x.SettingName == "EmailFrom").SettingValue != "") ? EmailSetting.FirstOrDefault(x => x.SettingName == "EmailFrom").SettingValue : "no-reply@weighingsolution.com";
                        List<EmailSetup> lstEmailSetup = ctxEmail.EmailSetups.Where(x => x.IsEnable && (x.EmailType == "Transaction Summary" || x.EmailType == "Transaction Detail")).ToList();
                        foreach (var item in lstEmailSetup)
                        {
                            if (item.NextRunTime != null && item.NextRunTime < DateTime.Now)
                            {
                                objEmail.Subject = item.EmailSubject;
                                objEmail.EmailAddress = item.EmailTo;
                                objEmail.Body = item.EmailBody;
                                DateTime Sdate = item.NextRunTime.Value;
                                DateTime Edate = item.NextRunTime.Value;
                                if (item.ScheduleType == "Daily")
                                    Sdate = Sdate.AddDays(-1);
                                else if (item.ScheduleType == "Week")
                                    Sdate = Sdate.AddDays(-7);
                                else if (item.ScheduleType == "Month")
                                    Sdate = Sdate.AddMonths(-1);
                                else if (item.ScheduleType == "Year")
                                    Sdate = Sdate.AddYears(-1);

                                if (item.Filterstring != null)
                                {
                                    strFilter = item.Filterstring.Replace("xxxStartDatexxx", Sdate.ToString()).Replace("xxxEndDatexxx", Edate.ToString());
                                }
                                string AttachExcelFileName = "";
                                string FileName = ExportReportToExcel(System.IO.Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\WeighBridgeSolution\AttachmentFile\"), Guid.NewGuid().ToString("N") + ".xls", item.WhereClause, Sdate, Edate, strFilter, item.EmailType, ctxEmail);
                                if (!string.IsNullOrEmpty(FileName))
                                {
                                    objEmail.Attachments = new Dictionary<int, string>();
                                    objEmail.Attachments.Add(0, FileName);
                                    if (item.EmailType == "Transaction Detail")
                                        AttachExcelFileName = "TransactionDetail.xls";
                                    else if (item.EmailType == "Transaction Summary")
                                        AttachExcelFileName = "TransactionSummary.xls";
                                }
                                objEmail.SendHtmlEmail(AttachExcelFileName, port, Convert.ToBoolean(EmailSetting.FirstOrDefault(x => x.SettingName == "UseSSL").SettingValue));
                                if (item.ScheduleType == "Daily")
                                    item.NextRunTime = item.NextRunTime.Value.AddDays(1);
                                if (item.ScheduleType == "Week")
                                    item.NextRunTime = item.NextRunTime.Value.AddDays(7);
                                if (item.ScheduleType == "Month")
                                    item.NextRunTime = item.NextRunTime.Value.AddMonths(1);
                                if (item.ScheduleType == "Year")
                                    item.NextRunTime = item.NextRunTime.Value.AddYears(1);
                            }
                        }
                        ctxEmail.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                //TODO:Error
            }
            timerEmail.Enabled = true;
        }

        protected override void OnStop()
        {
            timerEmail.Enabled = false;
        }
        private string ExportReportToExcel(string path, string reportName, string WhereClause, DateTime StartDate, DateTime EndDate, string strFilter, string EmailType, Endel_weighbridgeEntities ctxEmail)
        {
            try
            {
                string filename = path + "\\" + reportName;
                string query = "";
                DataTable dt = new DataTable();
                if (EmailType == "Transaction Summary")
                {
                    if (!string.IsNullOrEmpty(WhereClause))
                        query = "SELECT TData.ReceiptTicketID, TData.TicketID,TData.Account_Name AS AccountName, TDetail.Product_Name AS ProductName,TData.Vehicle_Number AS VehicleNumber,TData.Transporter_Name,TData.Driver_Name, GrossTime, GrossWeight, TareTime, Tdetail.TareWeight, NetWeight, IsGrossManual, IsTareManual ,TData.Charges,TData.TransactionType,TDetail.WeightUnit,TDetail.WeightBridgeName as 'ScaleName' FROM TransactionData TData INNER JOIN TransactionDetail TDetail ON TData.ReceiptTicketID = TDetail.ReceiptTicketID AND (((TDetail.GrossWeight IS NOT NULL AND TDetail.GrossWeight <> 0) AND (TDetail.TareWeight IS NOT NULL AND TDetail.TareWeight <> 0)) OR (TData.STATUS = 'Active' AND (TData.TransactionMode = 'Double' OR (	TData.TransactionMode = 'Multi' AND (SELECT SUM(SequenceNo) FROM TransactionDetail TXD1 WHERE TXD1.ReceiptTicketID = TData.ReceiptTicketID) = 0)))) WHERE (TDetail.CaptureTime BETWEEN @StartDate AND @EndDate) AND " + WhereClause + " ORDER BY TDetail.TransactionDetailID DESC";
                    else
                        query = "SELECT TData.ReceiptTicketID, TData.TicketID,TData.Account_Name AS AccountName, TDetail.Product_Name AS ProductName,TData.Vehicle_Number AS VehicleNumber,TData.Transporter_Name,TData.Driver_Name, GrossTime, GrossWeight, TareTime, Tdetail.TareWeight, NetWeight, IsGrossManual, IsTareManual ,TData.Charges,TData.TransactionType,TDetail.WeightUnit,TDetail.WeightBridgeName as 'ScaleName' FROM TransactionData TData INNER JOIN TransactionDetail TDetail ON TData.ReceiptTicketID = TDetail.ReceiptTicketID AND (((TDetail.GrossWeight IS NOT NULL AND TDetail.GrossWeight <> 0) AND (TDetail.TareWeight IS NOT NULL AND TDetail.TareWeight <> 0)) OR (TData.STATUS = 'Active' AND (TData.TransactionMode = 'Double' OR (	TData.TransactionMode = 'Multi' AND (SELECT SUM(SequenceNo) FROM TransactionDetail TXD1 WHERE TXD1.ReceiptTicketID = TData.ReceiptTicketID) = 0))))  WHERE (TDetail.CaptureTime BETWEEN @StartDate AND @EndDate)  ORDER BY TDetail.TransactionDetailID DESC";
                }
                else if (EmailType == "Transaction Detail")
                {
                    if (!string.IsNullOrEmpty(WhereClause))
                        query = "SELECT TData.ReceiptTicketID ,TData.TicketID,TData.Account_Name AS AccountName,TData.Transporter_Name AS TransaporterName,TData.TransactionMode,TData.STATUS AS TransactionStatus,TData.Charges,TData.TransactionType,TData.Driver_Name AS DriverName,TData.Vehicle_Number As VehicleNumber,Field1Value = TData.CustomValue1,Field2Value = TData.CustomValue2,Field3Value = TData.CustomValue3,Field4Value = TData.CustomValue4,Field5Value = TData.CustomValue5,Field6Value = TData.CustomValue6,Field7Value = TData.CustomValue7,Field8Value = TData.CustomValue8,Field9Value = TData.CustomValue9,Field10Value = TData.CustomValue10,Field11Value = TData.CustomValue11,Field12Value = TData.CustomValue12,Field13Value = TData.CustomValue13,Field14Value = TData.CustomValue14,Field15Value = TData.CustomValue15,Field16Value = TData.CustomValue16,Field17Value = TData.CustomValue17,Field18Value = TData.CustomValue18,Field19Value = TData.CustomValue19,Field20Value = TData.CustomValue20,TDetail.SequenceNo,ISNULL(TDetail.TareWeight, 0) AS TareWeight,TDetail.TareTime ,TDetail.IsTareManual ,ISNULL(TDetail.GrossWeight, 0) AS GrossWeight,TDetail.GrossTime,TDetail.IsGrossManual ,TDetail.NetWeight,TDetail.WeighmentType ,TDetail.WeightUnit,TDetail.Product_Name As ProductName,TDetail.Gate_Name As GateName,TData.CreatedName as 'UserName',TDetail.WeightBridgeName As ScaleName FROM TransactionData TData  INNER JOIN TransactionDetail TDetail ON TDetail.ReceiptTicketID = TData.ReceiptTicketID  WHERE (((TDetail.GrossWeight IS NOT NULL AND TDetail.GrossWeight <> 0)AND (TDetail.TareWeight IS NOT NULL AND TDetail.TareWeight <> 0 ))OR ( TData.STATUS = 'Active' AND ( TData.TransactionMode = 'Double' OR ( TData.TransactionMode = 'Multi' AND (	SELECT SUM(SequenceNo)FROM TransactionDetail TXD1 WHERE TXD1.ReceiptTicketID = TData.ReceiptTicketID) = 0))))AND (TDetail.CaptureTime BETWEEN @StartDate AND @EndDate) AND " + WhereClause + " ORDER BY TDetail.TransactionDetailID DESC";
                    else
                        query = "SELECT TData.ReceiptTicketID, TData.TicketID,TData.Account_Name AS AccountName,TData.Transporter_Name AS TransaporterName,TData.TransactionMode,TData.STATUS AS TransactionStatus,TData.Charges,TData.TransactionType,TData.Driver_Name AS DriverName,TData.Vehicle_Number As VehicleNumber,Field1Value = TData.CustomValue1,Field2Value = TData.CustomValue2,Field3Value = TData.CustomValue3,Field4Value = TData.CustomValue4,Field5Value = TData.CustomValue5,Field6Value = TData.CustomValue6,Field7Value = TData.CustomValue7,Field8Value = TData.CustomValue8,Field9Value = TData.CustomValue9,Field10Value = TData.CustomValue10,Field11Value = TData.CustomValue11,Field12Value = TData.CustomValue12,Field13Value = TData.CustomValue13,Field14Value = TData.CustomValue14,Field15Value = TData.CustomValue15,Field16Value = TData.CustomValue16,Field17Value = TData.CustomValue17,Field18Value = TData.CustomValue18,Field19Value = TData.CustomValue19,Field20Value = TData.CustomValue20,TDetail.SequenceNo,ISNULL(TDetail.TareWeight, 0) AS TareWeight,TDetail.TareTime, TDetail.IsTareManual, ISNULL(TDetail.GrossWeight, 0) AS GrossWeight, TDetail.GrossTime,TDetail.IsGrossManual,TDetail.NetWeight,TDetail.WeighmentType, TDetail.WeightUnit,TDetail.Product_Name As ProductName,TDetail.Gate_Name As GateName,TData.CreatedName as 'UserName',TDetail.WeightBridgeName As ScaleName FROM TransactionData TData   INNER JOIN TransactionDetail TDetail ON TDetail.ReceiptTicketID = TData.ReceiptTicketID WHERE (((TDetail.GrossWeight IS NOT NULL AND TDetail.GrossWeight <> 0) AND (TDetail.TareWeight IS NOT NULL AND TDetail.TareWeight <> 0)) OR (TData.STATUS = 'Active' AND (TData.TransactionMode = 'Double' OR (TData.TransactionMode = 'Multi' AND (SELECT SUM(SequenceNo) FROM TransactionDetail TXD1	WHERE TXD1.ReceiptTicketID = TData.ReceiptTicketID) = 0)))) AND (TDetail.CaptureTime BETWEEN @StartDate AND @EndDate) ORDER BY TDetail.TransactionDetailID DESC";
                }
                File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\WeighBridgeSolution\Plugins\EmailServiceTrace.txt", EmailType + Environment.NewLine + DateTime.Now.ToString("dd /MM/yyyy HH:mm:ss") + "------------------------------------------------------------------" + Environment.NewLine);
                if (!String.IsNullOrEmpty(EmailServiceConstant.EmailServiceConnectionString))
                {
                    using (SqlConnection con = new SqlConnection(EmailServiceConstant.EmailServiceConnectionString))
                    {
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.Add(new SqlParameter("@StartDate", StartDate));
                        cmd.Parameters.Add(new SqlParameter("@EndDate", EndDate.AddDays(1)));
                        con.Open();
                        dt.Load(cmd.ExecuteReader());
                        if (dt.Rows.Count > 0)
                        {
                            Warning[] warnings;
                            string[] streamids;
                            string mimeType;
                            string encoding;
                            string filenameExtension;
                            ReportViewer rv = new ReportViewer();
                            rv.ProcessingMode = ProcessingMode.Local;
                            //LocalReport report = new LocalReport();
                            rv.LocalReport.EnableExternalImages = true;
                            PermissionSet permissions = new PermissionSet(PermissionState.None);
                            permissions.AddPermission(new FileIOPermission(PermissionState.Unrestricted));
                            permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));  // The missing piece
                            rv.LocalReport.SetBasePermissionsForSandboxAppDomain(permissions);
                            ReportDataSource rds = new ReportDataSource();
                            if (EmailType == "Transaction Summary")
                            {
                                //report.ReportPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\EmailReport\\TransactionSummary.rdlc";
                                rv.LocalReport.ReportEmbeddedResource = "EmailService.EmailReport.TransactionSummary.rdlc";
                                rds.Name = "TransactionDataSet";//This refers to the dataset name in the RDLC file
                                File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\WeighBridgeSolution\Plugins\EmailServiceTrace.txt", rv.LocalReport.ReportPath + Environment.NewLine + DateTime.Now.ToString("dd /MM/yyyy HH:mm:ss") + "------------------------------------------------------------------" + Environment.NewLine);
                            }
                            else if (EmailType == "Transaction Detail")
                            {
                                //report.ReportPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\EmailReport\\TransactionDetailReport.rdlc";
                                rv.LocalReport.ReportEmbeddedResource = "EmailService.EmailReport.TransactionDetailReport.rdlc";
                                rds.Name = "TransactionDetailDataSet";//This refers to the dataset name in the RDLC file



                                ReportParameter[] reportParameterCollection = new ReportParameter[20];
                                ReportParameter[] cmfVisibleParamCollection = new ReportParameter[26];

                                MasterField[] lstmstfld = ctxEmail.MasterFields.ToArray();
                                for (int i = 1; i <= lstmstfld.Length; i++)
                                {
                                    int j = i + 20;
                                    cmfVisibleParamCollection[j - 1] = new ReportParameter("mfVisible" + i, (!lstmstfld[i - 1].Isvisible).ToString());
                                }

                                CustomField[] lstCstmfld = ctxEmail.CustomFields.ToArray();
                                for (int i = 1; i <= lstCstmfld.Length; i++)
                                {
                                    cmfVisibleParamCollection[i - 1] = new ReportParameter("cfVisible" + i, (!lstCstmfld[i - 1].IsVisible).ToString());
                                    if (lstCstmfld[i - 1].DisplayName != "")
                                        reportParameterCollection[i - 1] = new ReportParameter("DisplayField" + i, lstCstmfld[i - 1].DisplayName);
                                    else
                                        reportParameterCollection[i - 1] = new ReportParameter("DisplayField" + i, "Field" + i);
                                }


                                rv.LocalReport.SetParameters(cmfVisibleParamCollection);
                                rv.LocalReport.SetParameters(reportParameterCollection);
                            }
                            rds.Value = dt;
                            List<ApplicationSetting> lstGlobalsetting = ctxEmail.ApplicationSettings.Where(x => x.ModuleName == "GlobalSetting").ToList();
                            string DateTimeFormat = lstGlobalsetting.FirstOrDefault(x => x.SettingName == "DateFormat").SettingValue + " " + lstGlobalsetting.FirstOrDefault(x => x.SettingName == "TimeFormat").SettingValue;
                            ReportParameter rp = new ReportParameter("DateFormatParameter", DateTimeFormat);
                            ReportParameter hsPrintPermission = new ReportParameter("HasManualPrintPermission", true.ToString());
                            ReportParameter rp1 = new ReportParameter("FilterInfo", strFilter);
                            rv.LocalReport.SetParameters(new ReportParameter[] { rp1 });
                            rv.LocalReport.SetParameters(new ReportParameter[] { rp, hsPrintPermission });
                            ReportParameter rp2 = new ReportParameter("CompanyName", getCompanyName(ctxEmail));
                            rv.LocalReport.SetParameters(new ReportParameter[] { rp2 });
                            string DecimalFormat = lstGlobalsetting.FirstOrDefault(x => x.SettingName == "DecimalFormat").SettingValue;
                            ReportParameter rp3 = new ReportParameter("DecimalFormat", DecimalFormat);
                            rv.LocalReport.SetParameters(new ReportParameter[] { rp3 });
                            rv.LocalReport.DataSources.Add(rds);
                            rv.LocalReport.Refresh();

                            byte[] bytes = rv.LocalReport.Render("Excel", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
                            FileStream fs = new FileStream(filename, FileMode.Create);
                            fs.Write(bytes, 0, bytes.Length);
                            fs.Close();
                        }
                        con.Close();
                    }
                }
                return filename;
            }
            catch (Exception ex)
            {
                File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\WeighBridgeSolution\Plugins\EmailServiceTrace.txt", ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + DateTime.Now.ToString("dd /MM/yyyy HH:mm:ss") + "------------------------------------------------------------------" + Environment.NewLine);
                return "";
            }
        }
        public string getCompanyName(Endel_weighbridgeEntities ctxEmail)
        {
            string CompanyName = "";
            ApplicationSetting cname = ctxEmail.ApplicationSettings.FirstOrDefault(x => x.ModuleName == "CompanySetting" && x.SettingName == "CompanyName");
            if (cname != null)
                CompanyName = cname.SettingValue;
            return CompanyName;
        }
    }
}
