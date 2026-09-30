using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Reporting.WinForms;
using Newtonsoft.Json;
using PluginInterfaces;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.DataLayer;
using WeighingSolution.Events;
using WeighingSolution.Helpers;
using WeighingSolution.Report;
using WeighingSolution.SharedComponents;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for ReceiptPreview.xaml
    /// </summary>
    public partial class ReceiptPrint : Page
    {
        DataTable childDT = new DataTable();
        DataTable childDTCloned = new DataTable();
        private decimal TotalNetWeight = 0;
        DataTable dt = new DataTable();
        TransactionData objTD;
        public PluginLoader<IPluginControl> mef = new PluginLoader<IPluginControl>("ReceiptPreview");
        weighbridgeEntities ctxGlobal = new weighbridgeEntities(System.Windows.Application.Current.Properties["EntityConnectionString"].ToString());
        protected readonly IEventAggregator EventAggregator = AppEventManager.Instance.EventAggregator;
        Guid TicketID;
        bool endtransaction = false;
        public string receiverMailId = "";
        private ObservableCollection<ApplicationSetting> PrintSetting;

        public ReceiptPrint(Guid TicketID, bool endtransaction = false)
        {
            InitializeComponent();
            this.TicketID = TicketID;
            this.endtransaction = endtransaction;
            this.Loaded += ReceiptPreview_Loaded;
            this.Unloaded += ReceiptPreview_Unloaded;
        }

        void ReceiptPreview_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string SelectedPrintType = "Laser";
                string SelectedPrinter = string.Empty;

                objTD = ctxGlobal.TransactionDatas.FirstOrDefault(x => x.ReceiptTicketID == TicketID);

                PrintSetting = new ObservableCollection<ApplicationSetting>(ctxGlobal.ApplicationSettings.Where(x => x.ModuleName == "PrintSetting").ToList());

                if (PrintSetting != null && PrintSetting.Count > 0)
                {
                    if (PrintSetting.Where(x => x.SettingName == "PrintType" && !string.IsNullOrEmpty(x.SettingValue)).Count() > 0)
                    {
                        SelectedPrintType = PrintSetting.Where(x => x.SettingName == "PrintType" && !string.IsNullOrEmpty(x.SettingValue)).FirstOrDefault().SettingValue;
                    }

                    if (PrintSetting.Where(x => x.SettingName == "PrinterName" && !string.IsNullOrEmpty(x.SettingValue)).Count() > 0)
                    {
                        SelectedPrinter = PrintSetting.Where(x => x.SettingName == "PrinterName" && !string.IsNullOrEmpty(x.SettingValue)).FirstOrDefault().SettingValue;
                    }
                }

                SqlConnection con = new SqlConnection(System.Windows.Application.Current.Properties["DefalutConnectionString"].ToString());

                /************** Patch for Error  *****************/
                //https://stackoverflow.com/questions/22422838/report-viewer-2010-issue-on-non-admin-accounts
                //An error occurred during local report processing. ---> Microsoft.Reporting.DefinitionInvalidException: The definition of the report 'C:\Program Files (x86)\Sinuvera Technologies\WeighBridge Solution\\Report\Receipt\SingleDoublePass_Receipt.rdlc' is invalid. ---> Microsoft.ReportingServices.ReportProcessing.ReportProcessingException: An unexpected error occurred while compiling expressions. Native compiler return value: `[BC2012] can't open 'C:\Users\Admin\AppData\Local\Temp\expression_host_2da15411db254da395d8ec032f529e6e.dll' for writing'.
                PermissionSet permissions = new PermissionSet(PermissionState.None);
                permissions.AddPermission(new FileIOPermission(PermissionState.Unrestricted));
                permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));  // The missing piece

                reportViewer.LocalReport.SetBasePermissionsForSandboxAppDomain(permissions);
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.ShowExportButton = false;
                if (!Common.ValidatePageRequest(PageEntity.PrintReceipt, PageAccess.Execute))
                    reportViewer.ShowPrintButton = false;

                /************ End Patch  ************************/
                foreach (var plugin in mef.loadedPlugins.Keys)
                {
                    plugin.SetupPlugin(this);
                }
                BeforeReceiptDetailLoadEventArgs reciptargs = new BeforeReceiptDetailLoadEventArgs();
                reciptargs.IsReceiptDetailLoad = true;
                reciptargs.tranData = objTD;
                EventAggregator.GetEvent<BeforeReceiptDetailLoadEvent>().Publish(reciptargs);
                if (reciptargs.IsReceiptDetailLoad)
                {
                    if (objTD.TransactionMode == TransactionMode.Multi.ToString())
                    {
                        //Summary Report
                        #region New Query (Parent)
                        string parentSQL = string.Empty;
                        parentSQL = $"SELECT TData.TicketID,TData.Account_Name AS AccountName,TData.Transporter_Name AS TransaporterName, " +
                            $"TData.TransactionMode,TData.Charges,TData.TransactionType,TData.Driver_Name AS DriverName, TData.Vehicle_Number AS VehicleNumber, " +
                            $"CF.Field1DisplayName,CF.Field2DisplayName,CF.Field3DisplayName,CF.Field4DisplayName, CF.Field5DisplayName,CF.Field6DisplayName,CF.Field7DisplayName,CF.Field8DisplayName,CF.Field9DisplayName," +
                            $"CF.Field10DisplayName,CF.Field11DisplayName,CF.Field12DisplayName,CF.Field13DisplayName,CF.Field14DisplayName,CF.Field15DisplayName,CF.Field16DisplayName, CF.Field17DisplayName,CF.Field18DisplayName," +
                            $"CF.Field19DisplayName,CF.Field20DisplayName,Field1Value = TData.CustomValue1,Field2Value = TData.CustomValue2,Field3Value = TData.CustomValue3, Field4Value = TData.CustomValue4," +
                            $"Field5Value = TData.CustomValue5,Field6Value = TData.CustomValue6, Field7Value = TData.CustomValue7,Field8Value = TData.CustomValue8,Field9Value = TData.CustomValue9," +
                            $"Field10Value = TData.CustomValue10,Field11Value = TData.CustomValue11,Field12Value = TData.CustomValue12, Field13Value = TData.CustomValue13,Field14Value = TData.CustomValue14," +
                            $"Field15Value = TData.CustomValue15, Field16Value = TData.CustomValue16,Field17Value = TData.CustomValue17,Field18Value = TData.CustomValue18, Field19Value = TData.CustomValue19," +
                            $"Field20Value = TData.CustomValue20  " +
                            $"FROM TransactionDetail TXD INNER JOIN TransactionData TData ON TXD.ReceiptTicketID = TData.ReceiptTicketID " +
                            $"CROSS APPLY (SELECT MAX(CASE WHEN FieldName = 'field1' THEN DisplayName END) AS Field1DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field2' THEN DisplayName END) AS Field2DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field3' THEN DisplayName END) AS Field3DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field4' THEN DisplayName END) AS Field4DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field5' THEN DisplayName END) AS Field5DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field6' THEN DisplayName END) AS Field6DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field7' THEN DisplayName END) AS Field7DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field8' THEN DisplayName END) AS Field8DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field9' THEN DisplayName END) AS Field9DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field10' THEN DisplayName END) AS Field10DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field11' THEN DisplayName END) AS Field11DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field12' THEN DisplayName END) AS Field12DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field13' THEN DisplayName END) AS Field13DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field14' THEN DisplayName END) AS Field14DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field15' THEN DisplayName END) AS Field15DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field16' THEN DisplayName END) AS Field16DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field17' THEN DisplayName END) AS Field17DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field18' THEN DisplayName END) AS Field18DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field19' THEN DisplayName END) AS Field19DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field20' THEN DisplayName END) AS Field20DisplayName " +
                            $" FROM CustomField WHERE ISVisible = 1) CF " +
                            $"WHERE TData.ReceiptTicketID = '" + TicketID + "'";

                        //LEFT JOIN Account A ON A.AccountID = TData.AccountID
                        //    LEFT JOIN Account TR ON TR.AccountID = TData.TransporterID
                        //    LEFT JOIN Vehicle V ON V.VehicleID = TData.VehicleID
                        //    LEFT JOIN WeightBridge WB ON WB.WeightBridgeID = TXD.WeightBridgeID
                        //    LEFT JOIN Driver D ON D.DriverID = TData.DriverID
                        #endregion

                        SqlCommand cmd = new SqlCommand(parentSQL, con);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 45;

                        //Transaction Detail
                        #region Query (Child)
                        var ChildSQL = $"SELECT TData.SequenceNo, TData.WeightBridgeName AS ScaleName, ISNULL(TData.TareWeight, 0) AS TareWeight, TData.TareTime, " +
                            $"ISNULL(TData.GrossWeight, 0) AS GrossWeight, TData.GrossTime, ISNULL(TData.NetWeight, 0) AS NetWeight, TData.IsTareManual, TData.IsGrossManual, " +
                            $"TData.WeighmentType, TData.WeightUnit, TData.Product_Name AS ProductName, TData.Gate_Name AS GateName, TData.WB_User_Name AS UserName, " +
                            $"CAST(0 as numeric(18,2)) As TotalNetWeight FROM TransactionDetail TData " +
                            $"INNER JOIN TransactionData T ON T.ReceiptTicketID = TData.ReceiptTicketID " +
                            $"WHERE (((TData.GrossWeight IS NOT NULL AND TData.GrossWeight <> 0) AND (TData.TareWeight IS NOT NULL AND TData.TareWeight <> 0)) " +
                            $"OR " +
                            $"(T.STATUS = 'Active' AND " +
                            $"(T.TransactionMode = 'Double' OR " +
                            $"(T.TransactionMode = 'Multi' AND (SELECT SUM(SequenceNo) FROM TransactionDetail TXD WHERE TXD.ReceiptTicketID = TData.ReceiptTicketID) = 0)))) " +
                            $"AND TData.ReceiptTicketID = '" + TicketID + "'";

                        //INNER JOIN WeightBridge WB ON WB.WeightBridgeID = TData.WeightBridgeID 
                        //LEFT JOIN UserMaster UM ON UM.UserID = TData.UserID 
                        //LEFT JOIN Product P ON P.ProductID = TData.ProductID 
                        //LEFT JOIN Gate G ON G.GateID = TData.GateID 
                        #endregion

                        SqlCommand cmdChild = new SqlCommand(ChildSQL, con);
                        cmdChild.CommandType = CommandType.Text;
                        cmdChild.CommandTimeout = 45;

                        con.Open();
                        dt.Load(cmd.ExecuteReader());
                        dt.Columns.Add("TotalNetweight", typeof(decimal));
                        childDT.Load(cmdChild.ExecuteReader());

                        childDTCloned = childDT.Clone();
                        childDTCloned.Columns["GrossWeight"].ReadOnly = false;
                        childDTCloned.Columns["GrossWeight"].DataType = typeof(String);
                        childDTCloned.Columns["TareWeight"].ReadOnly = false;
                        childDTCloned.Columns["TareWeight"].DataType = typeof(String);
                        childDTCloned.Columns["NetWeight"].ReadOnly = false;
                        childDTCloned.Columns["NetWeight"].DataType = typeof(String);
                        childDTCloned.Columns["GrossTime"].ReadOnly = false;
                        childDTCloned.Columns["GrossTime"].DataType = typeof(DateTime);
                        childDTCloned.Columns["TareTime"].ReadOnly = false;
                        childDTCloned.Columns["TareTime"].DataType = typeof(DateTime);
                        childDTCloned.Columns["TotalNetweight"].ReadOnly = false;
                        foreach (DataRow item in childDT.Rows)
                        {
                            childDTCloned.ImportRow(item);
                            childDTCloned.Rows[childDTCloned.Rows.Count - 1]["GrossWeight"] = string.Format("{0: " + Constant.DecimalFormat + "}", item["GrossWeight"]).Trim() + " " + ((Common.ValidatePageRequest(PageEntity.PrintStarForManualEntry, PageAccess.Execute) && Convert.ToBoolean(item["IsGrossManual"]) == true) ? "*" : " ");
                            childDTCloned.Rows[childDTCloned.Rows.Count - 1]["TareWeight"] = string.Format("{0: " + Constant.DecimalFormat + "}", item["TareWeight"]).Trim() + " " + ((Common.ValidatePageRequest(PageEntity.PrintStarForManualEntry, PageAccess.Execute) && Convert.ToBoolean(item["IsTareManual"]) == true) ? "*" : " ");
                            childDTCloned.Rows[childDTCloned.Rows.Count - 1]["NetWeight"] = string.Format("{0:" + Constant.DecimalFormat + "}", item["NetWeight"]).Trim();
                            //childDTCloned.Rows[childDTCloned.Rows.Count - 1]["GrossTime"] = item["GrossTime"] != DBNull.Value ? Convert.ToDateTime(item["GrossTime"]).ToString(Constant.DateTimeFormat) : "";
                            //childDTCloned.Rows[childDTCloned.Rows.Count - 1]["TareTime"] = item["TareTime"] != DBNull.Value ? Convert.ToDateTime(item["TareTime"]).ToString(Constant.DateTimeFormat) : "";
                            TotalNetWeight += Convert.ToDecimal(item["NetWeight"]);
                        }
                        foreach (DataRow item in childDTCloned.Rows)
                        {
                            item["TotalNetweight"] = string.Format("{0:" + Constant.DecimalFormat + "}", TotalNetWeight).Trim();
                        }
                        con.Close();

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "Data"; // Name of the DataSet we set in .rdlc
                        reportDataSource.Value = dt;

                        if (SelectedPrintType == PrintType.DotMatrix.ToString())
                            reportViewer.LocalReport.ReportPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Report\\Receipt\\MultiPass_Receipt_courier.rdlc";
                        else
                            reportViewer.LocalReport.ReportPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Report\\Receipt\\MultiPass_Receipt.rdlc";

                        reportViewer.LocalReport.EnableExternalImages = true;
                        string LoggedINUserName = string.Empty;
                        if (Application.Current.Properties["LoggedInUserName"] != null)
                        {
                            LoggedINUserName = Application.Current.Properties["LoggedInUserName"].ToString();
                        }
                        List<ReportParameter> lstParams = new List<ReportParameter>();
                        lstParams.Add(new ReportParameter("LoggedInUser", LoggedINUserName));
                        lstParams.Add(new ReportParameter("CompanyName", Common.getCompanyName()));
                        lstParams.Add(new ReportParameter("ExecutionTime", DateTime.Now.ToString(Constant.DateTimeFormat)));
                        reportViewer.LocalReport.SetParameters(lstParams);

                        EventAggregator.GetEvent<ReceiptDetailLoadEvent>().Publish(new ReceiptDetailLoadEventArgs { reportViewer = reportViewer, tranData = objTD, windform = winform });

                        reportViewer.LocalReport.DataSources.Add(reportDataSource);
                        reportViewer.RefreshReport();
                        reportViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SetSubDataSource);
                        //https://stackoverflow.com/questions/915328/report-viewer-problem-on-winforms-some-parameters-or-credentials-have-not-been
                        //used reportViewer.Refresh() instead of reportViewer.LocalReport.Refresh()
                        //Alternative fix: https://stackoverflow.com/questions/301679/one-or-more-parameters-required-to-run-the-report-have-not-been-specified
                        reportViewer.Refresh();

                        if (endtransaction)
                            EventAggregator.GetEvent<ReceiptDetailAfterLoadEvent>().Publish(new ReceiptDetailLoadEventArgs { reportViewer = reportViewer, tranData = objTD });

                    }
                    else
                    {
                        //Summary Report
                        #region Old Query
                        //var parentSQL = "SELECT TData.TicketID,A.CompanyName AS AccountName,TR.CompanyName AS TransaporterName,TData.TransactionMode,(D.FirstName + ' ' + ISNULL(D.LastName, '')) AS DriverName,V.VehicleNumber,Field1DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field1'),Field2DisplayName = (SELECT DisplayName	FROM CustomField WHERE FieldName = 'field2'),Field3DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field3'),Field4DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field4'),Field5DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field5'),Field6DisplayName = (SELECT DisplayName	FROM CustomField WHERE FieldName = 'field6'),Field7DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field7'),Field8DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field8'),Field1Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField	WHERE FieldName = 'field1')	AND TicketID = TData.TicketID),Field2Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field2') AND TicketID = TData.TicketID),Field3Value = (SELECT CustomValue FROM CustomValue	WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field3') AND TicketID = TData.TicketID),Field4Value = (SELECT CustomValue	FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field4') AND TicketID = TData.TicketID),Field5Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField	WHERE FieldName = 'field5')	AND TicketID = TData.TicketID),Field6Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field6') AND TicketID = TData.TicketID),Field7Value = (SELECT CustomValue FROM CustomValue	WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field7') AND TicketID = TData.TicketID),Field8Value = (SELECT CustomValue	FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field8') AND TicketID = TData.TicketID),TareWeight = (SELECT CASE WHEN STATUS = 'Active' AND (MIN(TareWeight) IS NULL	OR MIN(TareWeight) = 0) THEN 0 WHEN TData.TransactionMode = 'Single' THEN MIN(TareWeight) ELSE MIN(CaptureWeight) END FROM TransactionDetail WHERE TicketID = TData.TicketID),GrossWeight = (SELECT CASE WHEN STATUS = 'Active' AND (MIN(GrossWeight) IS NULL OR MIN(GrossWeight) = 0) THEN 0 WHEN TData.TransactionMode = 'Single' THEN MAX(GrossWeight) ELSE Max(CaptureWeight)	END	FROM TransactionDetail WHERE TicketID = TData.TicketID),TareTime = (SELECT CASE WHEN STATUS = 'Active' AND (MIN(TareTime) IS NULL) THEN NULL WHEN TData.TransactionMode = 'Single'	THEN MIN(TareTime) ELSE (Select CaptureTime FROM TransactionDetail Tx WHERE CaptureWeight = (SELECT CASE WHEN STATUS = 'Active' AND (MIN(TareWeight) IS NULL	OR MIN(TareWeight) = 0)	THEN 0 WHEN TData.TransactionMode = 'Single' THEN MIN(TareWeight) ELSE MIN(CaptureWeight) END FROM TransactionDetail WHERE TicketID = TData.TicketID) AND Tx.TicketID = TData.TicketID) END FROM TransactionDetail WHERE TicketID = TData.TicketID),GrossTime = (SELECT CASE WHEN STATUS = 'Active' AND (MIN(GrossTime) IS NULL)	THEN NULL WHEN TData.TransactionMode = 'Single' THEN MAX(GrossTime) ELSE (Select CaptureTime FROM TransactionDetail Tx WHERE CaptureWeight = (SELECT CASE WHEN STATUS = 'Active' AND (MIN(GrossWeight) IS NULL OR MIN(GrossWeight) = 0) THEN 0 WHEN TData.TransactionMode = 'Single' THEN MAX(GrossWeight)	ELSE Max(CaptureWeight) END	FROM TransactionDetail WHERE TicketID = TData.TicketID) AND Tx.TicketID = TData.TicketID)	END	FROM TransactionDetail WHERE TicketID = TData.TicketID),NetWeight = (SELECT CASE WHEN TData.TransactionMode = 'Single' THEN (MAX(GrossWeight) - MIN(TareWeight)) ELSE (MAX(CaptureWeight) - MIN(CaptureWeight)) END FROM TransactionDetail WHERE TicketID = TData.TicketID),WeightUnit = (SELECT MIN(WeightUnit)	FROM TransactionDetail TxD WHERE TxD.TicketID = TData.TicketID) FROM TransactionData TData LEFT JOIN Account A ON A.AccountID = TData.AccountID LEFT JOIN Account TR ON TR.AccountID = TData.TransporterID LEFT JOIN Vehicle V ON V.VehicleID = TData.VehicleID LEFT JOIN Driver D ON D.DriverID = TData.DriverID WHERE TData.TicketID = " + TicketID;
                        #endregion

                        string parentSQL = string.Empty;
                        #region New Parent Query
                        parentSQL = $"SELECT TData.TicketID,TData.Account_Name AS AccountName,TData.Transporter_Name AS TransaporterName, TData.Charges, " +
                            $"TData.TransactionType,TData.TransactionMode,TXD.TareTime, TXD.GrossTime, ISNULL(TXD.TareWeight, 0) AS TareWeight, " +
                            $"ISNULL(TXD.GrossWeight, 0) AS GrossWeight, ISNULL(TXD.NetWeight, 0) AS NetWeight,TXD.IsTareManual, TXD.IsGrossmanual, " +
                            $"TXD.WeightUnit,TXD.Product_Name AS ProductName, TXD.Gate_Name AS GateName, TXD.WB_User_Name AS UserName,TData.Driver_Name AS DriverName, TData.Vehicle_Number AS VehicleNumber," +
                            $"CF.Field1DisplayName,CF.Field2DisplayName,CF.Field3DisplayName,CF.Field4DisplayName, CF.Field5DisplayName,CF.Field6DisplayName,CF.Field7DisplayName,CF.Field8DisplayName,CF.Field9DisplayName," +
                            $"CF.Field10DisplayName,CF.Field11DisplayName,CF.Field12DisplayName,CF.Field13DisplayName,CF.Field14DisplayName,CF.Field15DisplayName,CF.Field16DisplayName, CF.Field17DisplayName,CF.Field18DisplayName," +
                            $"CF.Field19DisplayName,CF.Field20DisplayName,Field1Value = TData.CustomValue1,Field2Value = TData.CustomValue2,Field3Value = TData.CustomValue3, Field4Value = TData.CustomValue4," +
                            $"Field5Value = TData.CustomValue5,Field6Value = TData.CustomValue6, Field7Value = TData.CustomValue7,Field8Value = TData.CustomValue8,Field9Value = TData.CustomValue9," +
                            $"Field10Value = TData.CustomValue10,Field11Value = TData.CustomValue11,Field12Value = TData.CustomValue12, Field13Value = TData.CustomValue13,Field14Value = TData.CustomValue14," +
                            $"Field15Value = TData.CustomValue15, Field16Value = TData.CustomValue16,Field17Value = TData.CustomValue17,Field18Value = TData.CustomValue18, Field19Value = TData.CustomValue19," +
                            $"Field20Value = TData.CustomValue20  " +
                            $"FROM TransactionData TData INNER JOIN TransactionDetail TXD ON TData.ReceiptTicketID = TXD.ReceiptTicketID " +
                            $"AND (((TXD.GrossWeight IS NOT NULL AND TXD.GrossWeight <> 0) AND (TXD.TareWeight IS NOT NULL AND TXD.TareWeight <> 0))" +
                            $"OR (TData.STATUS = 'Active' AND (TData.TransactionMode = 'Double'))) " +
                            $"CROSS APPLY (SELECT MAX(CASE WHEN FieldName = 'field1' THEN DisplayName END) AS Field1DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field2' THEN DisplayName END) AS Field2DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field3' THEN DisplayName END) AS Field3DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field4' THEN DisplayName END) AS Field4DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field5' THEN DisplayName END) AS Field5DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field6' THEN DisplayName END) AS Field6DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field7' THEN DisplayName END) AS Field7DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field8' THEN DisplayName END) AS Field8DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field9' THEN DisplayName END) AS Field9DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field10' THEN DisplayName END) AS Field10DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field11' THEN DisplayName END) AS Field11DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field12' THEN DisplayName END) AS Field12DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field13' THEN DisplayName END) AS Field13DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field14' THEN DisplayName END) AS Field14DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field15' THEN DisplayName END) AS Field15DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field16' THEN DisplayName END) AS Field16DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field17' THEN DisplayName END) AS Field17DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field18' THEN DisplayName END) AS Field18DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field19' THEN DisplayName END) AS Field19DisplayName," +
                            $"MAX(CASE WHEN FieldName = 'field20' THEN DisplayName END) AS Field20DisplayName" +
                            $" FROM CustomField WHERE ISVisible = 1) CF " +
                            $"WHERE TData.ReceiptTicketID = '" + TicketID + "'";
                        #endregion

                        SqlCommand cmd = new SqlCommand(parentSQL, con);
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 45;
                        con.Open();
                        dt.Load(cmd.ExecuteReader());
                        con.Close();

                        DataTable dtCloned = dt.Clone();
                        dtCloned.Columns["GrossWeight"].ReadOnly = false;
                        dtCloned.Columns["GrossWeight"].DataType = typeof(String);
                        dtCloned.Columns["TareWeight"].ReadOnly = false;
                        dtCloned.Columns["TareWeight"].DataType = typeof(String);
                        dtCloned.Columns["NetWeight"].ReadOnly = false;
                        dtCloned.Columns["NetWeight"].DataType = typeof(String);
                        dtCloned.Columns["GrossTime"].ReadOnly = false;
                        dtCloned.Columns["GrossTime"].DataType = typeof(DateTime);
                        dtCloned.Columns["TareTime"].ReadOnly = false;
                        dtCloned.Columns["TareTime"].DataType = typeof(DateTime);
                        foreach (DataRow item in dt.Rows)
                        {
                            dtCloned.ImportRow(item);
                            dtCloned.Rows[dtCloned.Rows.Count - 1]["GrossWeight"] = string.Format("{0: " + Constant.DecimalFormat + "}", item["GrossWeight"]).Trim() + " " + ((Common.ValidatePageRequest(PageEntity.PrintStarForManualEntry, PageAccess.Execute) && Convert.ToBoolean(item["IsGrossManual"]) == true) ? "*" : " ");
                            dtCloned.Rows[dtCloned.Rows.Count - 1]["TareWeight"] = string.Format("{0: " + Constant.DecimalFormat + "}", item["TareWeight"]).Trim() + " " + ((Common.ValidatePageRequest(PageEntity.PrintStarForManualEntry, PageAccess.Execute) && Convert.ToBoolean(item["IsTareManual"]) == true) ? "*" : " ");
                            dtCloned.Rows[dtCloned.Rows.Count - 1]["NetWeight"] = string.Format("{0: " + Constant.DecimalFormat + "}", item["NetWeight"]).Trim();
                            //dtCloned.Rows[dtCloned.Rows.Count - 1]["GrossTime"] = item["GrossTime"] != DBNull.Value ? Convert.ToDateTime(item["GrossTime"]).ToString(Constant.DateTimeFormat) : "";
                            //dtCloned.Rows[dtCloned.Rows.Count - 1]["TareTime"] = item["TareTime"] != DBNull.Value ? Convert.ToDateTime(item["TareTime"]).ToString(Constant.DateTimeFormat) : "";
                        }

                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "Data"; // Name of the DataSet we set in .rdlc
                        reportDataSource.Value = dtCloned;

                        if (SelectedPrintType == PrintType.DotMatrix.ToString())
                            reportViewer.LocalReport.ReportPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Report\\Receipt\\SingleDoublePass_Receipt_courier.rdlc";
                        else
                            reportViewer.LocalReport.ReportPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Report\\Receipt\\SingleDoublePass_Receipt.rdlc";


                        //reportViewer.LocalReport.ReportPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Report\\Receipt\\SingleDoublePass_Receipt.rdlc";
                        reportViewer.LocalReport.EnableExternalImages = true;
                        string LoggedINUserName = string.Empty;
                        if (Application.Current.Properties["LoggedInUserName"] != null)
                        {
                            LoggedINUserName = Application.Current.Properties["LoggedInUserName"].ToString();
                        }
                        List<ReportParameter> lstParams = new List<ReportParameter>();
                        lstParams.Add(new ReportParameter("LoggedInUser", LoggedINUserName));
                        lstParams.Add(new ReportParameter("CompanyName", Common.getCompanyName()));
                        lstParams.Add(new ReportParameter("ExecutionTime", DateTime.Now.ToString(Constant.DateTimeFormat)));
                        reportViewer.LocalReport.SetParameters(lstParams);

                        EventAggregator.GetEvent<ReceiptDetailLoadEvent>().Publish(new ReceiptDetailLoadEventArgs { reportViewer = reportViewer, tranData = objTD, windform = winform });

                        reportViewer.LocalReport.DataSources.Add(reportDataSource);

                        File.WriteAllText(Constant.ApplicationDataLocation + @"\ReportPageSettings.txt", string.Empty);

                        ReportPageSettings rps = reportViewer.LocalReport.GetDefaultPageSettings();
                        if (rps.IsLandscape && rps.PaperSize.Height > rps.PaperSize.Width)
                        {
                            System.Drawing.Printing.PageSettings ps = reportViewer.GetPageSettings();
                            ps.PaperSize = new System.Drawing.Printing.PaperSize(rps.PaperSize.Kind.ToString(), rps.PaperSize.Height, rps.PaperSize.Width);
                            ps.Landscape = false;
                            reportViewer.SetPageSettings(ps);
                        }

                        reportViewer.RefreshReport();
                        //reportViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SetSubDataSource);
                        //https://stackoverflow.com/questions/915328/report-viewer-problem-on-winforms-some-parameters-or-credentials-have-not-been
                        //used reportViewer.Refresh() instead of reportViewer.LocalReport.Refresh()
                        //Alternative fix: https://stackoverflow.com/questions/301679/one-or-more-parameters-required-to-run-the-report-have-not-been-specified
                        reportViewer.Refresh();

                        if (endtransaction)
                            EventAggregator.GetEvent<ReceiptDetailAfterLoadEvent>().Publish(new ReceiptDetailLoadEventArgs { reportViewer = reportViewer, tranData = objTD });
                    }
                    if (endtransaction)
                    {

                        //SendEmail = new Thread(new ThreadStart(Backgroungprocess));
                        //SendEmail.Start();

                        //System.ComponentModel.BackgroundWorker backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
                        //backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
                        //backgroundWorker1.RunWorkerAsync(10000);

                        Task.Run(Backgroungprocess);
                    }


                    //reportViewer.PrinterSettings.PrinterName = SelectedPrinter;

                    //reportViewer.Refresh();

                    //Warning[] warnings;
                    //string[] streamids;
                    //string mimeType;
                    //string encoding;
                    //string filenameExtension;

                    //byte[] bytes = reportViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
                    //string PDFFilename = Constant.ApplicationDataLocation + @"\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                    //using (FileStream fs = new FileStream(PDFFilename, FileMode.Create))
                    //{
                    //    fs.Write(bytes, 0, bytes.Length);
                    //}

                    if (reportViewer.PrinterSettings.IsValid)
                    {
                        System.Drawing.Printing.PageSettings ps = reportViewer.GetPageSettings();
                        string orientationNEW = ps.Landscape ? "Landscape" : "portrait";
                        string Sizevalnew = $" Print orientation = {orientationNEW} , Print Height = {ps.PaperSize.Height} ,Print Width = {ps.PaperSize.Width}";
                        File.AppendAllText(Constant.ApplicationDataLocation + @"\ReportPageSettings.txt", Environment.NewLine + Sizevalnew);
                        reportViewer.LocalReport.Print(SelectedPrinter,0);
                    }


                    Uri uri = new Uri("/View/TransactionView/TransactionManagement.Xaml", UriKind.Relative);
                    this.NavigationService.Navigate(uri);
                }
                else
                {
                    Uri uri = new Uri("/View/TransactionView/TransactionManagementList_Page.Xaml", UriKind.Relative);
                    this.NavigationService.Navigate(uri);
                }
            }
            catch (LocalProcessingException ex)
            {
                //https://stackoverflow.com/questions/30310652/reportviewer-2010-io-exception
                //http://alsaydi.blogspot.com/2011/07/report-rendering-fails-in-local-mode.html
                //https://stackoverflow.com/questions/22422838/report-viewer-2010-issue-on-non-admin-accounts
                App.Log.Error(ex.Message, ex);
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\ReceiptPreviewError.txt", json);
                winform.Visibility = Visibility.Collapsed;
                ((MetroWindow)App.Current.MainWindow).ShowMessageAsync("An error occured", "Unable to open report. Please try again.");
            }
        }

        //private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    Thread.Sleep(5000);
        //    SendEmail = new Thread(new ThreadStart(Backgroungprocess));
        //    SendEmail.Start();
        //}

        public async void Backgroungprocess()
        {
            using (var ctx = new weighbridgeEntities(System.Windows.Application.Current.Properties["EntityConnectionString"].ToString()))
            {
                SendSMSEveryTransaction();
                List<EmailSetup> lstEmailsetup = await ctx.EmailSetups.Where(x => x.EmailType == "Every Transaction" && x.IsEnable).ToListAsync();
                if (lstEmailsetup != null && lstEmailsetup.Count > 0)
                {
                    List<ApplicationSetting> EmailSetting = await ctx.ApplicationSettings.Where(x => x.ModuleName == "EmailSetting").ToListAsync();
                    if (EmailSetting != null && EmailSetting.Count > 0)
                    {
                        foreach (var item in lstEmailsetup)
                        {
                            try
                            {
                                SendMail(EmailSetting, item, item.EmailTo);
                            }
                            catch (Exception ex)
                            {
                                Common.logErrorInDB(ex.Message, ex);
                            }
                        }
                    }
                }
            }
        }

        private async void SendSMSEveryTransaction()
        {
            using (var ctx = new weighbridgeEntities(System.Windows.Application.Current.Properties["EntityConnectionString"].ToString()))
            {
                try
                {
                    ObservableCollection<SMSAPIDATA> items = SMS.ReadSmsApiConfig();
                    if (items == null)
                        return;
                    if (items.Count < 1)
                        return;

                    List<SMSSetup> lstSMSSetup = await ctx.SMSSetups.Where(x => x.SMSType == "Every Transaction" && x.IsEnable).ToListAsync();
                    ;
                    if (lstSMSSetup != null && lstSMSSetup.Count > 0)
                    {
                        await Task.Run(() =>
                        {
                            foreach (var item in lstSMSSetup)
                            {
                                if (items.Select(p => p.SMSType.ToString()).FirstOrDefault() == "API")
                                {
                                    string providerName = items.FirstOrDefault().ProviderName;
                                    if (string.IsNullOrEmpty(providerName))
                                        return;

                                    try
                                    {
                                        string TOMobileNo = item.SMSTo;
                                        //string Message = item.SMSBody + " - " + Common.GetSMSBody(objTD, item);
                                        string Message = !string.IsNullOrEmpty(item.SMSBody) ? item.SMSBody + Environment.NewLine + Common.GetSMSBody(objTD, item.SMSSetupID) : Common.GetSMSBody(objTD, item.SMSSetupID) + " ";
                                        Message += Common.GetSMSEnd(item.SMSSetupID);
                                        SMS _SMS = new SMS();
                                        string SMSResponse = _SMS.SendSms(TOMobileNo, Message, providerName);
                                        string Response = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine;
                                        Response += SMSResponse;
                                        File.AppendAllText(Constant.PluginFileLocation + @"\SmsRespronse.txt", Response);
                                    }
                                    catch (Exception ex)
                                    {
                                        Common.logErrorInDB(ex.Message, ex);
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        string TOMobileNo = item.SMSTo;
                                        string Response = string.Empty;
                                        //string Message = item.SMSBody + " - " + Common.GetSMSBody(objTD, item);
                                        string Message = !string.IsNullOrEmpty(item.SMSBody) ? item.SMSBody + Environment.NewLine + Common.GetSMSBody(objTD, item.SMSSetupID) : Common.GetSMSBody(objTD, item.SMSSetupID) + " ";
                                        Message += Common.GetSMSEnd(item.SMSSetupID);
                                        //SMS _SMS = new SMS();
                                        //string SMSResponse = _SMS.SendGSMSMS(TOMobileNo, Message,);

                                        SMS GsmSMS = new SMS();
                                        string JsonBaudRate = items.FirstOrDefault().BaudRate;
                                        string JsonParity = items.FirstOrDefault().Parity;
                                        string JsonStopBits = items.FirstOrDefault().StopBits;
                                        string JsonComPort = items.FirstOrDefault().ComPort;
                                        string JsonDataBits = items.FirstOrDefault().DataBits;
                                        string JsonHandShack = items.FirstOrDefault().HandShack;

                                        bool IsSuccessbauderate = int.TryParse(JsonBaudRate, out int resultBaudRate);
                                        int BaudRate;
                                        if (IsSuccessbauderate)
                                        {
                                            BaudRate = resultBaudRate;
                                        }
                                        else
                                        {
                                            BaudRate = 9600;
                                        }
                                        bool IsSuccessDatabits = int.TryParse(JsonDataBits, out int resultDataBits);
                                        int DataBits;
                                        if (IsSuccessDatabits)
                                        {
                                            DataBits = resultDataBits;
                                        }
                                        else
                                        {
                                            DataBits = 8;
                                        }
                                        System.IO.Ports.Parity parity = GsmSMS.GetParity(JsonParity);
                                        System.IO.Ports.StopBits stopBits = GsmSMS.GetStopBits(JsonStopBits);

                                        if (!string.IsNullOrEmpty(TOMobileNo))
                                        {
                                            if (TOMobileNo.Contains(','))
                                            {
                                                string[] ArrayOfMobileNo = TOMobileNo.Split(',');

                                                foreach (var mobileNo in ArrayOfMobileNo)
                                                {
                                                    if (!string.IsNullOrEmpty(mobileNo))
                                                        Response = GsmSMS.SendGSMSMS(mobileNo.Trim(), Message, JsonComPort, BaudRate, parity, DataBits, stopBits);
                                                }
                                            }
                                            else
                                            {
                                                Response = GsmSMS.SendGSMSMS(TOMobileNo, Message, JsonComPort, BaudRate, parity, DataBits, stopBits);
                                            }
                                            if (Response == "SMS send successfully")
                                            {
                                                ToastHelper.ShowSuccess(Response);
                                            }
                                            else
                                            {
                                                //ToastHelper.ShowSuccess(Response);
                                            }
                                        }

                                        if (Response == "SMS send successfully")
                                        {
                                            ToastHelper.ShowSuccess(Response);
                                        }
                                        else
                                        {
                                            //ToastHelper.ShowSuccess(Response);
                                        }
                                        //string Response = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine;
                                        //Response += SMSResponse;
                                        File.AppendAllText(Constant.PluginFileLocation + @"\SmsRespronse.txt", Response);
                                    }
                                    catch (Exception ex)
                                    {
                                        _ = ex.Message;
                                        //Common.logErrorInDB(ex.Message, ex);
                                    }
                                }
                            }
                        });
                    }
                    //string ProviderName = SMSSetting.FirstOrDefault().SettingValue;
                    //List<SMSSetup> lstSMSSetup = ctxGlobal.SMSSetups.Where(x => x.SMSType == "Every Transaction" && x.IsEnable).ToList(); ;
                    //if (lstSMSSetup != null && lstSMSSetup.Count > 0)
                    //{
                    //    await Task.Run(() =>
                    //    {
                    //        foreach (var item in lstSMSSetup)
                    //        {
                    //            try
                    //            {
                    //                string TOMobileNo = item.SMSTo;
                    //                //string Message = item.SMSBody + " - " + Common.GetSMSBody(objTD, item);
                    //                string Message = item.SMSBody + " - " + Common.GetSMSBody(objTD, item.SMSSetupID);
                    //                SMS _SMS = new SMS();
                    //                string SMSResponse = _SMS.SendSms(TOMobileNo, Message, ProviderName);
                    //                string Response = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine;
                    //                Response += SMSResponse;
                    //                File.AppendAllText(Constant.PluginFileLocation + @"\SmsRespronse.txt", Response);
                    //            }
                    //            catch (Exception ex)
                    //            {
                    //                Common.logErrorInDB(ex.Message, ex);
                    //            }
                    //        }
                    //    });
                    //}
                }
                catch (Exception) { }
            }
        }
        private bool SendMail(List<ApplicationSetting> EmailSetting, EmailSetup item, string Mailto)
        {
            try
            {
                Email objEmail = new Email();
                objEmail.SMTPServer = EmailSetting.FirstOrDefault(x => x.SettingName == "ServerName").SettingValue;
                int port = Convert.ToInt32(EmailSetting.FirstOrDefault(x => x.SettingName == "Port").SettingValue);
                objEmail.SMTPUserName = (EmailSetting.FirstOrDefault(x => x.SettingName == "UserName").SettingValue != "") ? EmailSetting.FirstOrDefault(x => x.SettingName == "UserName").SettingValue : "";
                objEmail.SMTPPassword = (EmailSetting.FirstOrDefault(x => x.SettingName == "PassWord").SettingValue != "") ? Common.DecryptData(EmailSetting.FirstOrDefault(x => x.SettingName == "PassWord").SettingValue) : "";
                objEmail.EmailFrom = (EmailSetting.FirstOrDefault(x => x.SettingName == "EmailFrom").SettingValue != "") ? EmailSetting.FirstOrDefault(x => x.SettingName == "EmailFrom").SettingValue : "no-reply@weighingsolution.com";
                objEmail.Subject = item.EmailSubject;
                objEmail.EmailAddress = Mailto;
                objEmail.Body = item.EmailBody + Common.GetmailBody(objTD, item, dt);
                if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\WeighBridgeSolution\\AttachmentFile\\"))
                {
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\WeighBridgeSolution\\AttachmentFile\\");
                }

                if (item.ColumnSelection.Contains("IsAttachmentFile"))
                {
                    Warning[] warnings;
                    string[] streamIds;
                    string mimeType = string.Empty;
                    string encoding = string.Empty;
                    string extension = string.Empty;
                    string fileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\WeighBridgeSolution\\AttachmentFile\\TransactionDetail_" + objTD.TicketID + ".pdf";
                    byte[] bytes = reportViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                    FileStream fs = new FileStream(fileName, FileMode.Create);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        objEmail.Attachments = new Dictionary<int, string>();
                        objEmail.Attachments.Add(0, fileName);
                    }
                }
                return objEmail.SendHtmlEmail(port, Convert.ToBoolean(EmailSetting.FirstOrDefault(x => x.SettingName == "UseSSL").SettingValue));
            }
            catch (Exception ex)
            {
                Common.logErrorInDB(ex.Message, ex);
                return false;
            }
        }

        void ReceiptPreview_Unloaded(object sender, RoutedEventArgs e)
        {
            reportViewer.Dispose();
            foreach (var plugin in mef.loadedPlugins.Keys)
            {
                plugin.DisposePlugin();
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void SetSubDataSource(object sender, SubreportProcessingEventArgs e)
        {
            e.DataSources.Add(new ReportDataSource("Data", childDTCloned));
            //e.DataSources.Add(new ReportDataSource("HasManualPrintPermission", Common.ValidatePageRequest(PageEntity.PrintManualEntry, PageAccess.Execute).ToString()));
            // e.DataSources.Add(new ReportDataSource("ImageData", ImagesDT));
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MetroWindow parentwnd = ((MetroWindow)App.Current.MainWindow);
            Frame MainFrame = parentwnd.FindName("MainFrame") as Frame;
            MainFrame.Content = new View.TransactionManagementList_Page();
        }

        public bool IsValid(string emailaddress)
        {
            try
            {
                System.Net.Mail.MailAddress m = new System.Net.Mail.MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
