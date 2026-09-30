using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Reporting.WinForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PluginInterfaces;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using WeighingSolution.DataLayer;
using WeighingSolution.Events;
using WeighingSolution.Helpers;
using WeighingSolution.SharedComponents;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for ReceiptPreview.xaml
    /// </summary>
    public partial class ReceiptPreview : Page
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
        bool FirstTimeLoaded = false;
        public string receiverMailId = "";
        public string receiverSMSMobile = "";
        public string SelectedPrintType = "";
        bool IsComplete = false;

        public ReceiptPreview(Guid TicketID, bool endtransaction = false)
        {
            InitializeComponent();
            this.TicketID = TicketID;
            this.endtransaction = endtransaction;
            objTD = ctxGlobal.TransactionDatas.FirstOrDefault(x => x.ReceiptTicketID == TicketID);
            bool IsMulti = objTD.TransactionMode == TransactionMode.Multi.ToString() ? true : false;
            this.IsComplete = objTD.Status == "Complete" ? true : false;
            this.LoadReceipts(IsMulti);
            this.Loaded += ReceiptPreview_Loaded;
            this.Unloaded += ReceiptPreview_Unloaded;
        }

        void ReceiptPreview_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (childDT != null)
                    childDT.Clear();

                if (childDTCloned != null)
                    childDTCloned.Clear();

                if (dt != null)
                    dt.Clear();

                objTD = ctxGlobal.TransactionDatas.FirstOrDefault(x => x.ReceiptTicketID == TicketID);
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
                    if (objTD.Status == "Complete")
                    {
                        stkSendMail.Visibility = Visibility.Visible;
                        bool IsSMSPlugin = LicensePluginHalper.CheckPluginInstall(PluginNameEnum.SMSPlugin.ToString());
                        if (IsSMSPlugin)
                            stkSendSMS.Visibility = Visibility.Visible;
                        winform.Margin = new Thickness { Left = 100, Top = 100, Right = 100, Bottom = 40 };
                    }

                    if (objTD.TransactionMode == TransactionMode.Multi.ToString())
                    {
                        TotalNetWeight = 0;
                        //Summary Report
                        #region Old Query
                        //var parentSQL = "SELECT TData.TicketID,A.CompanyName AS AccountName, TR.CompanyName AS TransaporterName,TData.TransactionMode,TData.Charges,TData.TransactionType,(D.FirstName + ' ' + ISNULL(D.LastName, '')) AS DriverName	,V.VehicleNumber,Field1DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field1'),Field2DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field2'),Field3DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field3'),Field4DisplayName = (SELECT DisplayName	FROM CustomField WHERE FieldName = 'field4'),Field5DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field5'),Field6DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field6'),Field7DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field7'),Field8DisplayName = (SELECT DisplayName	FROM CustomField WHERE FieldName = 'field8'),Field9DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field9'),Field10DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field10'),Field11DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field11'),Field12DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field12'),Field13DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field13'),Field14DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field14'),Field15DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field15'),Field16DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field16'),Field17DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field17'),Field18DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field18'),Field19DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field19'),Field20DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field20'),Field1Value = (SELECT CustomValue FROM CustomValue	WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field1') AND ReceiptTicketID = TData.ReceiptTicketID),Field2Value = (SELECT CustomValue	FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field2') AND ReceiptTicketID = TData.ReceiptTicketID ),Field3Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field3') AND ReceiptTicketID = TData.ReceiptTicketID),Field4Value = (SELECT CustomValue FROM CustomValue	WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field4') AND ReceiptTicketID = TData.ReceiptTicketID),Field5Value = (SELECT CustomValue	FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field5') AND ReceiptTicketID = TData.ReceiptTicketID),Field6Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField	WHERE FieldName = 'field6')	AND ReceiptTicketID = TData.ReceiptTicketID),Field7Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field7') AND ReceiptTicketID = TData.ReceiptTicketID),Field8Value = (SELECT CustomValue FROM CustomValue	WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field8')AND ReceiptTicketID = TData.ReceiptTicketID),Field9Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field9') AND ReceiptTicketID = TData.ReceiptTicketID),Field10Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field10') AND ReceiptTicketID = TData.ReceiptTicketID),Field11Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field11') AND ReceiptTicketID = TData.ReceiptTicketID),Field12Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field12') AND ReceiptTicketID = TData.ReceiptTicketID),Field13Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field13') AND ReceiptTicketID = TData.ReceiptTicketID),Field14Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field14') AND ReceiptTicketID = TData.ReceiptTicketID) ,Field15Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field15') AND ReceiptTicketID = TData.ReceiptTicketID),Field16Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field16') AND ReceiptTicketID = TData.ReceiptTicketID) ,Field17Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field17') AND ReceiptTicketID = TData.ReceiptTicketID),Field18Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field18') AND ReceiptTicketID = TData.ReceiptTicketID),Field19Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field19') AND ReceiptTicketID = TData.ReceiptTicketID),Field20Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field20') AND ReceiptTicketID = TData.ReceiptTicketID)   FROM TransactionData TData LEFT JOIN Account A ON A.AccountID = TData.AccountID LEFT JOIN Account TR ON TR.AccountID = TData.TransporterID LEFT JOIN Vehicle V ON V.VehicleID = TData.VehicleID LEFT JOIN Driver D ON D.DriverID = TData.DriverID WHERE TData.ReceiptTicketID = '" + TicketID + "'";
                        #endregion

                        string parentSQL = string.Empty;
                        #region New Query (Parent)

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
                        if (!dt.Columns.Contains("TotalNetweight"))
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

                        var Globalsetting = ctxGlobal.ApplicationSettings.Any(x => x.ModuleName == "Globalsetting" && x.SettingName == "MultiSelectedReceipt")
                          ? ctxGlobal.ApplicationSettings.Where(x => x.ModuleName == "Globalsetting" && x.SettingName == "MultiSelectedReceipt").FirstOrDefault() : null;

                        string basePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Report\\Receipt\\";

                        if (SelectedPrintType != null && SelectedPrintType != "")
                        {
                            var selectedReceipt = Receipts.FirstOrDefault(r => r.Label == SelectedPrintType);

                            if (selectedReceipt != null && !string.IsNullOrEmpty(selectedReceipt.Name))
                            {
                                string rdlcPath = basePath + selectedReceipt.Name;
                                AddAddressPerameter(rdlcPath);
                                AddMasterFieldParametersToRdlc(rdlcPath);
                                reportViewer.LocalReport.ReportPath = rdlcPath;
                                cmbPrintType.Text = selectedReceipt.Label;
                            }
                            else
                            {
                                string rdlcPath = basePath + "MultiPass_Receipt.rdlc";
                                AddAddressPerameter(rdlcPath);
                                AddMasterFieldParametersToRdlc(rdlcPath);
                                reportViewer.LocalReport.ReportPath = rdlcPath;
                                cmbPrintType.Text = "Default Format - Multi Laser";
                            }
                        }
                        else
                        {
                            var selectedReceipt = Receipts.FirstOrDefault(r => r.Label == Globalsetting.SettingValue);

                            if (selectedReceipt != null && !string.IsNullOrEmpty(selectedReceipt.Name))
                            {
                                string rdlcPath = basePath + selectedReceipt.Name;
                                AddAddressPerameter(rdlcPath);
                                AddMasterFieldParametersToRdlc(rdlcPath);
                                reportViewer.LocalReport.ReportPath = rdlcPath;
                                cmbPrintType.Text = selectedReceipt.Label;
                            }
                            else
                            {
                                string rdlcPath = basePath + "MultiPass_Receipt.rdlc";
                                AddAddressPerameter(rdlcPath);
                                AddMasterFieldParametersToRdlc(rdlcPath);
                                reportViewer.LocalReport.ReportPath = rdlcPath;
                                cmbPrintType.Text = "Default Format - Multi Laser";
                            }
                        }

                        //reportViewer.LocalReport.ReportPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Report\\Receipt\\MultiPass_Receipt.rdlc";

                        reportViewer.LocalReport.EnableExternalImages = true;
                        string LoggedINUserName = string.Empty;
                        if (Application.Current.Properties["LoggedInUserName"] != null)
                        {
                            LoggedINUserName = Application.Current.Properties["LoggedInUserName"].ToString();
                        }
                        List<ReportParameter> lstParams = new List<ReportParameter>();
                        lstParams.Add(new ReportParameter("LoggedInUser", LoggedINUserName));
                        lstParams.Add(new ReportParameter("CompanyName", Common.getCompanyName()));
                        lstParams.Add(new ReportParameter("Address", Common.getAddress()));
                        lstParams.Add(new ReportParameter("ExecutionTime", Constant.FormateDateTimeOutString(DateTime.Now)));
                        lstParams.AddRange(Common.getmasterFieldVisbility());
                        //lstParams.Add(new ReportParameter("WeightBridgeLocation", objTD.CompeletedTransaction.WeightBridge.WeightBridgeLocation));



                        reportViewer.LocalReport.SetParameters(lstParams);
                        bool IsBarcodePlugin = LicensePluginHalper.CheckPluginInstall(PluginNameEnum.BarcodePlugin.ToString());
                        if (IsBarcodePlugin)
                        {
                            LoadBarcodeOrQrImageInReceipt(new ReceiptDetailLoadEventArgs { reportViewer = reportViewer, tranData = objTD, windform = winform });
                        }

                        var data = ctxGlobal.Cameras.ToList();
                        if (data.Count > 0)
                        {
                            LoadImageInReceipt(new ReceiptDetailLoadEventArgs { reportViewer = reportViewer, tranData = objTD, windform = winform });
                        }
                        EventAggregator.GetEvent<ReceiptDetailLoadEvent>().Publish(new ReceiptDetailLoadEventArgs { reportViewer = reportViewer, tranData = objTD, windform = winform });

                        reportViewer.LocalReport.DataSources.Add(reportDataSource);
                        reportViewer.RefreshReport();
                        reportViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SetSubDataSource);
                        //https://stackoverflow.com/questions/915328/report-viewer-problem-on-winforms-some-parameters-or-credentials-have-not-been
                        //used reportViewer.Refresh() instead of reportViewer.LocalReport.Refresh()
                        //Alternative fix: https://stackoverflow.com/questions/301679/one-or-more-parameters-required-to-run-the-report-have-not-been-specified
                        reportViewer.Refresh();

                        this.Visibility = Visibility.Visible;
                        if (endtransaction)
                        {
                            EventAggregator.GetEvent<ReceiptDetailAfterLoadEvent>().Publish(new ReceiptDetailLoadEventArgs { reportViewer = reportViewer, tranData = objTD });
                        }
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
                        dtCloned.Columns["GrossWeight"].DataType = typeof(string);
                        dtCloned.Columns["TareWeight"].ReadOnly = false;
                        dtCloned.Columns["TareWeight"].DataType = typeof(string);
                        dtCloned.Columns["NetWeight"].ReadOnly = false;
                        dtCloned.Columns["NetWeight"].DataType = typeof(string);
                        dtCloned.Columns["GrossTime"].ReadOnly = false;
                        dtCloned.Columns["GrossTime"].DataType = typeof(string);
                        dtCloned.Columns["TareTime"].ReadOnly = false;
                        dtCloned.Columns["TareTime"].DataType = typeof(string);
                        foreach (DataRow item in dt.Rows)
                        {
                            dtCloned.ImportRow(item);
                            dtCloned.Rows[dtCloned.Rows.Count - 1]["GrossWeight"] = string.Format("{0: " + Constant.DecimalFormat + "}", item["GrossWeight"]).Trim() + " " + ((Common.ValidatePageRequest(PageEntity.PrintStarForManualEntry, PageAccess.Execute) && Convert.ToBoolean(item["IsGrossManual"]) == true) ? "*" : " ");
                            dtCloned.Rows[dtCloned.Rows.Count - 1]["TareWeight"] = string.Format("{0: " + Constant.DecimalFormat + "}", item["TareWeight"]).Trim() + " " + ((Common.ValidatePageRequest(PageEntity.PrintStarForManualEntry, PageAccess.Execute) && Convert.ToBoolean(item["IsTareManual"]) == true) ? "*" : " ");
                            dtCloned.Rows[dtCloned.Rows.Count - 1]["NetWeight"] = string.Format("{0: " + Constant.DecimalFormat + "}", item["NetWeight"]).Trim();

                            dtCloned.Rows[dtCloned.Rows.Count - 1]["GrossTime"] = item["GrossTime"] != DBNull.Value ? Convert.ToDateTime(item["GrossTime"])
                               .ToString(Constant.DateTimeFormat, CultureInfo.InvariantCulture) : "";

                            dtCloned.Rows[dtCloned.Rows.Count - 1]["TareTime"] = item["TareTime"] != DBNull.Value ? Convert.ToDateTime(item["TareTime"])
                                .ToString(Constant.DateTimeFormat, CultureInfo.InvariantCulture) : "";
                        }



                        ReportDataSource reportDataSource = new ReportDataSource();
                        reportDataSource.Name = "Data"; // Name of the DataSet we set in .rdlc
                        reportDataSource.Value = dtCloned;

                        var Globalsetting = ctxGlobal.ApplicationSettings.Any(x => x.ModuleName == "Globalsetting" && x.SettingName == "SelectedReceipt")
                          ? ctxGlobal.ApplicationSettings.Where(x => x.ModuleName == "Globalsetting" && x.SettingName == "SelectedReceipt").FirstOrDefault() : null;

                        string basePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Report\\Receipt\\";

                        if (SelectedPrintType != null && SelectedPrintType != "")
                        {
                            var selectedReceipt = Receipts.FirstOrDefault(r => r.Label == SelectedPrintType);

                            if (selectedReceipt != null && !string.IsNullOrEmpty(selectedReceipt.Name))
                            {
                                string rdlcPath = basePath + selectedReceipt.Name;
                                AddAddressPerameter(rdlcPath);
                                AddMasterFieldParametersToRdlc(rdlcPath);
                                reportViewer.LocalReport.ReportPath = rdlcPath;
                                cmbPrintType.Text = selectedReceipt.Label;
                            }
                            else
                            {
                                string rdlcPath = basePath + "SingleDoublePass_Receipt.rdlc";
                                AddAddressPerameter(rdlcPath);
                                AddMasterFieldParametersToRdlc(rdlcPath);
                                reportViewer.LocalReport.ReportPath = rdlcPath;
                                cmbPrintType.Text = "Default Format - Laser";
                            }
                        }
                        else
                        {
                            var selectedReceipt = Receipts.FirstOrDefault(r => r.Label == Globalsetting.SettingValue);

                            if (selectedReceipt != null && !string.IsNullOrEmpty(selectedReceipt.Name))
                            {
                                string rdlcPath = basePath + selectedReceipt.Name;
                                AddAddressPerameter(rdlcPath);
                                AddMasterFieldParametersToRdlc(rdlcPath);
                                reportViewer.LocalReport.ReportPath = rdlcPath;
                                cmbPrintType.Text = selectedReceipt.Label;
                            }
                            else
                            {
                                string rdlcPath = basePath + "SingleDoublePass_Receipt.rdlc";
                                AddAddressPerameter(rdlcPath);
                                AddMasterFieldParametersToRdlc(rdlcPath);
                                reportViewer.LocalReport.ReportPath = rdlcPath;
                                cmbPrintType.Text = "Default Format - Laser";
                            }
                        }

                        // reportViewer.LocalReport.ReportPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Report\\Receipt\\SingleDoublePass_Receipt.rdlc";
                        reportViewer.LocalReport.EnableExternalImages = true;
                        string LoggedINUserName = string.Empty;
                        if (Application.Current.Properties["LoggedInUserName"] != null)
                        {
                            LoggedINUserName = Application.Current.Properties["LoggedInUserName"].ToString();
                        }
                        List<ReportParameter> lstParams = new List<ReportParameter>();
                        lstParams.Add(new ReportParameter("LoggedInUser", LoggedINUserName));
                        lstParams.Add(new ReportParameter("CompanyName", Common.getCompanyName()));
                        lstParams.Add(new ReportParameter("Address", Common.getAddress()));
                        lstParams.Add(new ReportParameter("ExecutionTime", Constant.FormateDateTimeOutString(DateTime.Now)));
                        lstParams.AddRange(Common.getmasterFieldVisbility());
                        reportViewer.LocalReport.SetParameters(lstParams);
                        bool IsBarcodePlugin = LicensePluginHalper.CheckPluginInstall(PluginNameEnum.BarcodePlugin.ToString());
                        if (IsBarcodePlugin)
                        {
                            LoadBarcodeOrQrImageInReceipt(new ReceiptDetailLoadEventArgs { reportViewer = reportViewer, tranData = objTD, windform = winform });
                        }
                        var data = ctxGlobal.Cameras.ToList();
                        if (data.Count > 0)
                        {
                            LoadImageInReceipt(new ReceiptDetailLoadEventArgs { reportViewer = reportViewer, tranData = objTD, windform = winform });
                        }
                        EventAggregator.GetEvent<ReceiptDetailLoadEvent>().Publish(new ReceiptDetailLoadEventArgs { reportViewer = reportViewer, tranData = objTD, windform = winform });

                        reportViewer.LocalReport.DataSources.Add(reportDataSource);
                        // reportViewer.LocalReport.DataSources.Add(DynamicCustomFieldSource);

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

                        this.Visibility = Visibility.Visible;
                        if (endtransaction)
                        {
                            EventAggregator.GetEvent<ReceiptDetailAfterLoadEvent>().Publish(new ReceiptDetailLoadEventArgs { reportViewer = reportViewer, tranData = objTD });
                        }
                    }
                    if (endtransaction)
                    {
                        // Whatsapp Api calling for sending PDF on whatsapp
                        //if(IsComplete)
                        //{
                        //    Task.Run(WhatsAppPDF);
                        //}
                        //SendEmail = new Thread(new ThreadStart(Backgroungprocess));
                        //SendEmail.Start();

                        //System.ComponentModel.BackgroundWorker backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
                        //backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
                        //backgroundWorker1.RunWorkerAsync(10000);

                        Task.Run(Backgroungprocess);
                    }

                }
                else
                {
                    Uri uri = new Uri("/View/TransactionView/TransactionManagementList_Page.Xaml", UriKind.Relative);
                    this.NavigationService.Navigate(uri);
                }

                FirstTimeLoaded = true;

            }
            catch (LocalProcessingException ex)
            {
                App.Log.Error(ex.Message, ex);
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\ReceiptPreviewError.txt", json);
                winform.Visibility = Visibility.Collapsed;
                ((MetroWindow)App.Current.MainWindow).ShowMessageAsync("An error occured", "Unable to open report. Please try again.");
            }
        }

        public void AddMasterFieldParametersToRdlc(string rdlcPath)
        {
            int totalMasterFields = 6;
            string baseParamName = "mfVisible";

            XDocument rdlcXml = XDocument.Load(rdlcPath);
            XNamespace ns = "http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition";

            var report = rdlcXml.Root;

            // Ensure <ReportParameters> exists
            var parameters = report.Elements(ns + "ReportParameters").FirstOrDefault();
            if (parameters == null)
            {
                parameters = new XElement(ns + "ReportParameters");
                report.AddFirst(parameters);
            }

            bool modified = false;

            for (int i = 1; i <= totalMasterFields; i++)
            {
                string paramName = baseParamName + i;

                bool exists = parameters.Elements(ns + "ReportParameter")
                                        .Any(p => (string)p.Attribute("Name") == paramName);

                if (!exists)
                {
                    parameters.Add(new XElement(ns + "ReportParameter",
                        new XAttribute("Name", paramName),
                        new XElement(ns + "DataType", "String"),
                        new XElement(ns + "Nullable", "true"),
                        new XElement(ns + "AllowBlank", "true"),
                        new XElement(ns + "MultiValue", "false"),
                        new XElement(ns + "Prompt", "Master Field Visibility")
                    ));

                    modified = true;
                }
            }

            // Save only if changes were made
            if (modified)
            {
                rdlcXml.Save(rdlcPath);
            }
        }

        public void AddAddressPerameter(string rdlcPath)
        {
            string paramName = "Address"; // Parameter name to check/add

            XDocument rdlcXml = XDocument.Load(rdlcPath);
            XNamespace ns = "http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition";

            var report = rdlcXml.Root;

            // Try to find existing <ReportParameters> block
            var parameters = report.Elements(ns + "ReportParameters").FirstOrDefault();

            // If <ReportParameters> does not exist, create it
            if (parameters == null)
            {
                parameters = new XElement(ns + "ReportParameters");
                report.AddFirst(parameters);
            }

            // Check if the parameter already exists
            bool exists = parameters.Elements(ns + "ReportParameter")
                                    .Any(p => (string)p.Attribute("Name") == paramName);

            if (!exists)
            {
                // Add new parameter
                parameters.Add(new XElement(ns + "ReportParameter",
                    new XAttribute("Name", paramName),
                    new XElement(ns + "DataType", "String"),
                    new XElement(ns + "Nullable", "true"),
                    new XElement(ns + "AllowBlank", "true"),
                    new XElement(ns + "MultiValue", "false"),
                    new XElement(ns + "Prompt", "Enter Address")
                ));

                // Save modified RDLC
                rdlcXml.Save(rdlcPath);
            }
            else
            {
                Console.WriteLine($"Parameter '{paramName}' already exists.");
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

        private void LoadBarcodeOrQrImageInReceipt(ReceiptDetailLoadEventArgs obj)
        {
            TransactionData objTD = obj.tranData as TransactionData;
            ReportViewer reportViewer = obj.reportViewer as ReportViewer;
            if (objTD.Status == "Complete")
            {
                BarcodeSaved(objTD);

                //using (BarcodePlugin.Endel_weighbridgeEntities ctxBarcode = new BarcodePlugin.Endel_weighbridgeEntities(BarcodeConstant.BarcodeEntityConnectionString))
                //{
                string BarcodeImg = "";
                BarcodeImage objbarcodeimg = ctxGlobal.BarcodeImages.FirstOrDefault(x => x.ReceiptTicketID == objTD.ReceiptTicketID);
                if (objbarcodeimg != null && !string.IsNullOrEmpty(objbarcodeimg.ImagePath))
                {
                    ApplicationSetting BarcodeSetting = ctxGlobal.ApplicationSettings.FirstOrDefault(x => x.ModuleName == "BarcodeSetting" && x.SettingName == "BarcodeImagePath");
                    if (BarcodeSetting != null && BarcodeSetting.SettingValue != "")
                    {
                        //if(!File.Exists(BarcodeSetting.SettingValue + "\\" + objbarcodeimg.ImagePath))
                        //{
                        //    BarcodeSaved(objTD);
                        //}

                        BarcodeImg = "File:\\" + BarcodeSetting.SettingValue + "\\" + objbarcodeimg.ImagePath;
                        ReportParameter rpBarcodeImg = new ReportParameter("BarcodeImg", BarcodeImg);
                        reportViewer.LocalReport.SetParameters(rpBarcodeImg);
                        bool hasImagehide = true;
                        if (BarcodeImg != null)
                            hasImagehide = false;
                        ReportParameter rpBarcodeHasImg = new ReportParameter("HasNoBarcodeImg", hasImagehide.ToString());
                        reportViewer.LocalReport.SetParameters(rpBarcodeHasImg);
                    }
                }

                //}
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
                                        File.WriteAllText(Constant.PluginFileLocation + @"\SmsText.txt", Message);
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
                                        File.WriteAllText(Constant.PluginFileLocation + @"\SmsText.txt", Message);//SMS _SMS = new SMS();
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
                }
                catch (Exception ex)
                {
                    Common.logErrorInDB(ex.Message, ex);
                }
            }
        }

        private async void SendSMSCustomOnTransaction()
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

                    List<SMSSetup> lstSMSSetup = await ctx.SMSSetups.Where(x => x.SMSType == "Custom On Transaction" && x.IsEnable).ToListAsync();
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
                                        //string Message = item.SMSBody + " - " + Common.GetSMSBody(objTD, item);
                                        string Message = !string.IsNullOrEmpty(item.SMSBody) ? item.SMSBody + Environment.NewLine + Common.GetSMSBody(objTD, item.SMSSetupID) : Common.GetSMSBody(objTD, item.SMSSetupID) + " ";
                                        Message += Common.GetSMSEnd(item.SMSSetupID);
                                        File.WriteAllText(Constant.PluginFileLocation + @"\SmsText.txt", Message);
                                        SMS _SMS = new SMS();
                                        string SMSResponse = _SMS.SendSms(receiverSMSMobile, Message, providerName);
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
                                        string Response = string.Empty;
                                        //string Message = item.SMSBody + " - " + Common.GetSMSBody(objTD, item);
                                        string Message = !string.IsNullOrEmpty(item.SMSBody) ? item.SMSBody + Environment.NewLine + Common.GetSMSBody(objTD, item.SMSSetupID) : Common.GetSMSBody(objTD, item.SMSSetupID) + " ";
                                        Message += Common.GetSMSEnd(item.SMSSetupID);
                                        File.WriteAllText(Constant.PluginFileLocation + @"\SmsText.txt", Message);
                                        int len = Message.Length;
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

                                        if (!string.IsNullOrEmpty(receiverSMSMobile))
                                        {
                                            if (receiverSMSMobile.Contains(','))
                                            {
                                                string[] ArrayOfMobileNo = receiverSMSMobile.Split(',');

                                                foreach (var mobileNo in ArrayOfMobileNo)
                                                {
                                                    if (!string.IsNullOrEmpty(mobileNo))
                                                        Response = GsmSMS.SendGSMSMS(mobileNo.Trim(), Message, JsonComPort, BaudRate, parity, DataBits, stopBits);
                                                }
                                            }
                                            else
                                            {
                                                Response = GsmSMS.SendGSMSMS(receiverSMSMobile, Message, JsonComPort, BaudRate, parity, DataBits, stopBits);
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
                    else
                    {
                        ToastHelper.ShowError("Please configure SMS of type custome on transaction");
                    }
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

        private void BtnSendMail_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtEmailTo.Text))
            {
                ToastHelper.ShowWarning("Email to is required.");
                return;
            }
            else if (!IsValid(txtEmailTo.Text))
            {
                ToastHelper.ShowWarning("Email to is not in proper format.");
                return;
            }
            receiverMailId = txtEmailTo.Text;
            if (!string.IsNullOrEmpty(receiverMailId))
            {
                EmailSetup objEmailsetup = ctxGlobal.EmailSetups.FirstOrDefault(x => x.EmailType == "Custom On Transaction" && x.IsEnable);
                if (objEmailsetup != null)
                {
                    List<ApplicationSetting> EmailSetting = ctxGlobal.ApplicationSettings.Where(x => x.ModuleName == "EmailSetting").ToList();
                    if (EmailSetting != null && EmailSetting.Count > 0)
                    {
                        SendMail(EmailSetting, objEmailsetup, receiverMailId);
                    }
                }
                else
                {
                    ToastHelper.ShowError("Set up 'Custom On Transaction' in Email schedule.");
                }
            }
        }
        private void BtnSendSMS_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSMSTo.Text))
            {
                ToastHelper.ShowWarning("Mobile No is required.");
                return;
            }
            receiverSMSMobile = txtSMSTo.Text;
            if (!string.IsNullOrEmpty(receiverSMSMobile))
            {
                SendSMSCustomOnTransaction();
            }
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
        public bool IsValidPhone(string Mobile)
        {
            try
            {
                Regex rgx = new Regex(@"^\+[1-9]{1}[0-9]{3,14}$");
                return rgx.IsMatch(Mobile);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private void cmbPrintType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //if (!FirstTimeLoaded)
            //    return;

            //try
            //{
            //    var abc = sender as ComboBox;

            //    if (abc == null || abc.SelectedItem == null)
            //        return;

            //    var item = abc.SelectedItem as ComboBoxItem;

            //    if (item == null || item.Name == null)
            //        return;

            //    string Selected = item.Name.ToString();

            //    if (!string.IsNullOrEmpty(Selected))
            //    {
            //        SelectedPrintType = Selected;
            //        ReceiptPreview_Loaded(null, e);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    string mess = ex.Message;
            //}


            if (!FirstTimeLoaded)
                return;

            try
            {
                var comboBox = sender as ComboBox;
                if (comboBox == null || comboBox.SelectedItem == null)
                    return;

                var selectedReceipt = comboBox.SelectedItem as Receipt;
                if (selectedReceipt == null || string.IsNullOrEmpty(selectedReceipt.Name))
                    return;


                SelectedPrintType = selectedReceipt.Label;

                ReceiptPreview_Loaded(null, e);
            }
            catch (Exception ex)
            {
                string mess = ex.Message;
            }


        }

        // FOR LOADING RECEIPT LISTS FROM RECEIPT.JSON FILE 

        private List<Receipt> _receiptlist;
        public List<Receipt> Receipts
        {
            get { return _receiptlist; }
            set { _receiptlist = value; }
        }

        private void LoadReceipts(bool isMulti)
        {
            string filePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Receipt.json");

            if (File.Exists(filePath))
            {
                string receiptDoc = File.ReadAllText(filePath);
                List<Receipt> allReceipts = JsonConvert.DeserializeObject<List<Receipt>>(receiptDoc).ToList();

                // Filter receipts based on isMulti parameter
                Receipts = allReceipts.Where(r => r.IsMulti == isMulti).ToList();
            }
            else
            {
                Receipts = new List<Receipt>();
            }

            cmbPrintType.ItemsSource = Receipts;
            cmbPrintType.DisplayMemberPath = "Label";
        }


        private void BarcodeSaved(TransactionData ObjTData)
        {
            if (ObjTData.Status == "Complete")
            {
                string Barcodestring = "";
                string GrossWeight;
                string TareWeight;
                string NetWeight;
                string GrossTime;
                string TareTime;
                TransactionDetail objtranDetail;
                if (ObjTData.TransactionMode == TransactionMode.Single.ToString())
                {
                    objtranDetail = ObjTData.TransactionDetails.FirstOrDefault();
                    GrossWeight = objtranDetail.GrossWeight.Value.ToString(WeighingSolution.DataLayer.Constant.DecimalFormat);
                    TareWeight = objtranDetail.TareWeight.Value.ToString(WeighingSolution.DataLayer.Constant.DecimalFormat);
                    NetWeight = objtranDetail.NetWeight.Value.ToString(WeighingSolution.DataLayer.Constant.DecimalFormat);
                    GrossTime = objtranDetail.GrossTime.Value.ToString("dd/MM/yyyyhhmmtt");
                    TareTime = objtranDetail.TareTime.Value.ToString("dd/MM/yyyyhhmmtt");
                }
                else if (ObjTData.TransactionMode == TransactionMode.Double.ToString())
                {
                    objtranDetail = ObjTData.TransactionDetails.FirstOrDefault(x => x.SequenceNo == 1);
                    GrossWeight = objtranDetail.GrossWeight.Value.ToString(WeighingSolution.DataLayer.Constant.DecimalFormat);
                    TareWeight = objtranDetail.TareWeight.Value.ToString(WeighingSolution.DataLayer.Constant.DecimalFormat);
                    NetWeight = objtranDetail.NetWeight.Value.ToString(WeighingSolution.DataLayer.Constant.DecimalFormat);
                    GrossTime = objtranDetail.GrossTime.Value.ToString("dd/MM/yyyyhhmmtt");
                    TareTime = objtranDetail.TareTime.Value.ToString("dd/MM/yyyyhhmmtt");
                }
                else
                {
                    GrossWeight = ObjTData.TransactionDetails.Where(p => p.CaptureWeight.HasValue && p.CaptureWeight > 0).Max(x => x.CaptureWeight).Value.ToString(WeighingSolution.DataLayer.Constant.DecimalFormat);
                    TareWeight = ObjTData.TransactionDetails.Where(p => p.CaptureWeight.HasValue && p.CaptureWeight > 0).Min(x => x.CaptureWeight).Value.ToString(WeighingSolution.DataLayer.Constant.DecimalFormat);
                    NetWeight = (Convert.ToDecimal(GrossWeight) - Convert.ToDecimal(TareWeight)).ToString(WeighingSolution.DataLayer.Constant.DecimalFormat);
                    decimal GWt = Convert.ToDecimal(GrossWeight);
                    GrossTime = ObjTData.TransactionDetails.FirstOrDefault(x => x.GrossWeight == GWt).GrossTime.Value.ToString("dd/MM/yyyyhhmmtt");
                    decimal Twt = Convert.ToDecimal(TareWeight);
                    TareTime = ObjTData.TransactionDetails.FirstOrDefault(x => x.TareWeight == Twt).TareTime.Value.ToString("dd/MM/yyyyhhmmtt");
                }
                List<BarcodeSetting> lstBarcode;
                using (weighbridgeEntities ctxBarcode = new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()))
                {
                    lstBarcode = ctxBarcode.BarcodeSettings.Where(x => x.IsSelected == true).ToList();
                }
                if (lstBarcode != null && lstBarcode.Count > 0)
                {
                    foreach (var item in lstBarcode)
                    {
                        if (!string.IsNullOrEmpty(item.BarcodeDisplayName))
                        {
                            if (item.BarcodeFieldName == "Ticket No")
                                Barcodestring = item.BarcodeDisplayName + "-" + ObjTData.TicketID;
                            if (item.BarcodeFieldName == "Vehicle No")
                            {
                                if (ObjTData.Vehicle != null && !string.IsNullOrEmpty(ObjTData.Vehicle.VehicleNumber))
                                    Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.Vehicle.VehicleNumber;
                            }
                            if (item.BarcodeFieldName == "Account Name")
                            {
                                if (ObjTData.Account != null && !string.IsNullOrEmpty(ObjTData.Account.CompanyName))
                                    Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.Account.CompanyName;
                            }
                            //if (item.BarcodeFieldName == "Token Value")
                            //{
                            //    if (ObjTData.RFToken != null && !string.IsNullOrEmpty(ObjTData.RFToken.TokenValue))
                            //        Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.RFToken.TokenValue;
                            //}
                            if (item.BarcodeFieldName == "Gross Weight")
                                Barcodestring += "*" + item.BarcodeDisplayName + "-" + GrossWeight;
                            if (item.BarcodeFieldName == "Gross Time")
                                Barcodestring += "*" + item.BarcodeDisplayName + "-" + GrossTime;
                            if (item.BarcodeFieldName == "Tare Weight")
                                Barcodestring += "*" + item.BarcodeDisplayName + "-" + TareWeight;
                            if (item.BarcodeFieldName == "Tare Time")
                                Barcodestring += "*" + item.BarcodeDisplayName + "-" + TareTime;
                            if (item.BarcodeFieldName == "Net Weight")
                                Barcodestring += "*" + item.BarcodeDisplayName + "-" + NetWeight;
                            if (item.BarcodeFieldName == "Charges" && ObjTData.Charges != null)
                                Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.Charges;
                            if (item.BarcodeFieldName == "Product" && !string.IsNullOrEmpty(ObjTData.TransactionProducts))
                                Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.TransactionProducts;
                            if (item.BarcodeFieldName == "Field1")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue1))
                                    Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.CustomValue1;
                            }
                            if (item.BarcodeFieldName == "Field2")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue2))
                                    Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.CustomValue2;
                            }
                            if (item.BarcodeFieldName == "Field3")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue3))
                                    Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.CustomValue3;
                            }
                            if (item.BarcodeFieldName == "Field4")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue4))
                                    Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.CustomValue4;
                            }
                            if (item.BarcodeFieldName == "Field5")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue5))
                                    Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.CustomValue5;
                            }
                            if (item.BarcodeFieldName == "Field6")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue6))
                                    Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.CustomValue6;
                            }
                            if (item.BarcodeFieldName == "Field7")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue7))
                                    Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.CustomValue7;
                            }
                            if (item.BarcodeFieldName == "Field8")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue8))
                                    Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.CustomValue8;
                            }

                            if (item.BarcodeFieldName == "Field9")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue9))
                                    Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.CustomValue9;
                            }
                            if (item.BarcodeFieldName == "Field10")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue10))
                                    Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.CustomValue10;
                            }
                            if (item.BarcodeFieldName == "Field11")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue11))
                                    Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.CustomValue11;
                            }
                            if (item.BarcodeFieldName == "Field12")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue12))
                                    Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.CustomValue12;
                            }
                            if (item.BarcodeFieldName == "Field13")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue13))
                                    Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.CustomValue13;
                            }
                            if (item.BarcodeFieldName == "Field14")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue14))
                                    Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.CustomValue14;
                            }
                            if (item.BarcodeFieldName == "Field15")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue15))
                                    Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.CustomValue15;
                            }
                            if (item.BarcodeFieldName == "Field16")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue16))
                                    Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.CustomValue16;
                            }
                            if (item.BarcodeFieldName == "Field17")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue17))
                                    Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.CustomValue17;
                            }
                            if (item.BarcodeFieldName == "Field18")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue18))
                                    Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.CustomValue18;
                            }
                            if (item.BarcodeFieldName == "Field19")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue19))
                                    Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.CustomValue19;
                            }
                            if (item.BarcodeFieldName == "Field20")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue20))
                                    Barcodestring += "*" + item.BarcodeDisplayName + "-" + ObjTData.CustomValue20;
                            }
                        }
                    }
                    if (Barcodestring != "")
                    {
                        Barcodestring = Barcodestring.Replace(" ", string.Empty);
                        int barcodeWidth = (Barcodestring.Length * 18);
                        string fileName = ObjTData.ReceiptTicketID + ".jpeg";


                        System.Drawing.Image barcodeImage = null;
                        try
                        {
                            var isBarcode = ctxGlobal.ApplicationSettings.FirstOrDefault(x => x.SettingName == "BarcodeOrQR").SettingValue;
                            if (isBarcode == "Barcode")
                            {
                                BarcodeLib.Barcode b = new BarcodeLib.Barcode();
                                b.IncludeLabel = true;
                                //===== Encoding performed here =====
                                barcodeImage = b.Encode(BarcodeLib.TYPE.CODE128, Barcodestring.Trim(), System.Drawing.ColorTranslator.FromHtml("#000000"), System.Drawing.ColorTranslator.FromHtml("#FFFFFF"), barcodeWidth, 150);
                                //===================================

                                //===== Static Encoding performed here =====
                                //barcodeImage = BarcodeLib.Barcode.DoEncode(type, this.txtData.Text.Trim(), this.chkGenerateLabel.Checked, this.btnForeColor.BackColor, this.btnBackColor.BackColor);
                                //==========================================
                            }
                            else
                            {
                                //BitmapImage bimg = new BitmapImage();
                                using (var ms = new MemoryStream())
                                {
                                    ZXing.BarcodeWriter writer;
                                    writer = new ZXing.BarcodeWriter() { Format = ZXing.BarcodeFormat.QR_CODE };
                                    writer.Options.Height = 150;
                                    writer.Options.Width = 150;
                                    writer.Options.PureBarcode = true;
                                    barcodeImage = writer.Write(Barcodestring.Trim());
                                    barcodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                            }


                        }//try
                        catch (Exception ex)
                        {
                            //TODO: find a way to return this to display the encoding error message
                        }//catch
                        finally
                        {
                        }//finally

                        if (barcodeImage != null)
                        {
                            using (weighbridgeEntities ctxBarcode = new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()))
                            {
                                ApplicationSetting BarcodeSetting = ctxBarcode.ApplicationSettings.FirstOrDefault(x => x.ModuleName == "BarcodeSetting" && x.SettingName == "BarcodeImagePath");
                                if (BarcodeSetting != null && BarcodeSetting.SettingValue != "")
                                {
                                    Common.CreateDirectoryWithFullAccess(BarcodeSetting.SettingValue);

                                    barcodeImage.Save(BarcodeSetting.SettingValue + "\\" + fileName, System.Drawing.Imaging.ImageFormat.Jpeg);

                                    BarcodeImage barcodeImage1 = ctxGlobal.BarcodeImages.FirstOrDefault(x => x.ReceiptTicketID == objTD.ReceiptTicketID);
                                    if (barcodeImage1 != null && !string.IsNullOrEmpty(barcodeImage1.ImagePath))
                                    {
                                        barcodeImage1.BarcodeString = Barcodestring;
                                        barcodeImage1.ImagePath = fileName;
                                        barcodeImage1.ReceiptTicketID = ObjTData.ReceiptTicketID;
                                        ctxBarcode.SaveChanges();
                                    }
                                    else
                                    {
                                        BarcodeImage objBarcodeImg = new BarcodeImage();
                                        objBarcodeImg.BarcodeString = Barcodestring;
                                        objBarcodeImg.ImagePath = fileName;
                                        objBarcodeImg.ReceiptTicketID = ObjTData.ReceiptTicketID;
                                        ctxBarcode.BarcodeImages.Add(objBarcodeImg);
                                        ctxBarcode.SaveChanges();
                                    }
                                }
                            }
                            //Clean up / Dispose...
                            barcodeImage.Dispose();
                        }
                    }
                }
            }
        }

        #region Whatsapp Api for detail on sending whatsapp

        // Whatsapp Api calling for sending PDF on whatsapp
        //private async void WhatsAppPDF()
        //{
        //    if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\WeighBridgeSolution\\AttachmentFile\\"))
        //    {
        //        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\WeighBridgeSolution\\AttachmentFile\\");
        //    }

        //    Warning[] warnings;
        //    string[] streamIds;
        //    string mimeType = string.Empty;
        //    string encoding = string.Empty;
        //    string extension = string.Empty;
        //    string fileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\WeighBridgeSolution\\AttachmentFile\\TransactionDetail_" + objTD.TicketID + ".pdf";
        //    byte[] bytes = reportViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
        //    FileStream fs = new FileStream(fileName, FileMode.Create);
        //    fs.Write(bytes, 0, bytes.Length);
        //    fs.Close();

        //    string sendfilename = string.Empty;
        //    string Base64 = string.Empty;
        //    if(File.Exists(fileName))
        //    {
        //        byte[] filebite = File.ReadAllBytes(fileName);
        //        Base64 = Convert.ToBase64String(filebite);
        //        sendfilename = "TransactionDetail_" + objTD.TicketID + ".pdf";

        //    }
        //       objTD = ctxGlobal.TransactionDatas.FirstOrDefault(x => x.ReceiptTicketID == TicketID);
        //        var finalDetail = objTD.TransactionDetails
        //                       .OrderByDescending(td => td.SequenceNo)
        //                       .FirstOrDefault();

        //        if (!string.IsNullOrEmpty(objTD.Account?.ContactNo) || !string.IsNullOrEmpty(objTD.Transporter?.ContactNo))
        //        {
        //            GettransactionResponse gettransaction = new GettransactionResponse();
        //            gettransaction.number = !string.IsNullOrEmpty(objTD.Account?.ContactNo) ? objTD.Account.ContactNo : objTD.Transporter.ContactNo;
        //            gettransaction.fileName = sendfilename;
        //            gettransaction.massage = Base64; //Base64();
        //            gettransaction.MassageContent = $@" TareWeight: {finalDetail.TareWeight} kg GrossWeight: {finalDetail.GrossWeight} kg NetWeight: {finalDetail.NetWeight} kg";
        //            var data = SAP_API.WbCheckWeighments(gettransaction);
        //        }
        //}

        public byte[] RenderToPdfBytes()
        {
            ReceiptPreview_Loaded(this, null);
            Warning[] warnings;
            string[] streamIds;
            string mimeType, encoding, extension;
            return reportViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
        }

        private void BtnSendWhatsApp_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtWhatsAppTo.Text))
            {
                ToastHelper.ShowWarning("Mobile number is required.");
                return;
            }
            SendWhatsAppReceipt(txtWhatsAppTo.Text.Trim());
        }


        private async void SendWhatsAppReceipt(string manualNumber)
        {
            try
            {
                var integration = ctxGlobal.ApiIntegrationMasters
                    .Where(x => x.IsActive && x.IsManualSendAvailable)
                    .OrderBy(x => x.SortOrder)
                    .FirstOrDefault();

                if (integration == null)
                {
                    ToastHelper.ShowError("WhatsApp API not configured!");
                    return;
                }

                objTD = ctxGlobal.TransactionDatas.FirstOrDefault(x => x.ReceiptTicketID == TicketID);
                var activeDetail = objTD.TransactionDetails.OrderByDescending(d => d.SequenceNo).FirstOrDefault();
                var extraFields = new Dictionary<string, object>();

                if (integration.RequiresManualNumber)
                    extraFields["manual_to_number"] = manualNumber;

                //if (integration.AttachReceiptPdf)
                //{
                //    // Already on the UI thread here (this IS the page with the loaded report) -
                //    // no dispatcher hop needed, unlike the background sync service would need.
                //    Warning[] warnings;
                //    string[] streamIds;
                //    string mimeType, encoding, extension;
                //    byte[] pdfBytes = reportViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
                //    //extraFields["receipt_pdf_base64"] = Convert.ToBase64String(pdfBytes);
                //}

                var apiService = new ApiIntegrationService();
                var result = await apiService.ExecuteAsync(integration, objTD, activeDetail, extraFields);

                ctxGlobal.ApiIntegrationLogs.Add(new ApiIntegrationLog
                {
                    ApiIntegrationID = integration.ApiIntegrationID,
                    TicketID = objTD.TicketID,
                    ReceiptTicketID = objTD.ReceiptTicketID,
                    RequestUrl = result.RequestUrl,
                    RequestBody = RedactBase64ForLogging(result.RequestBody),
                    ResponseBody = result.ResponseBody,
                    StatusCode = result.StatusCode,
                    IsSuccess = result.IsSuccess,
                    ErrorMessage = result.ErrorMessage,
                    CreatedTime = DateTime.Now
                });
                ctxGlobal.SaveChanges();

                if (result.IsSuccess)
                    ToastHelper.ShowSuccess("Receipt sent successfully.");
                else
                    ToastHelper.ShowError("Send failed: " + (result.ErrorMessage ?? result.ResponseBody));
            }
            catch (Exception ex)
            {
                File.AppendAllText(Constant.PluginFileLocation + @"\WhatsAppSendError.txt",
                    Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex));
                ToastHelper.ShowError("WhatsApp send error: " + ex.Message);
            }
        }

        private static string RedactBase64ForLogging(string requestBody)
        {
            if (string.IsNullOrWhiteSpace(requestBody))
                return requestBody;

            try
            {
                var token = JToken.Parse(requestBody);
                RedactBase64Values(token);
                return token.ToString(Formatting.Indented);
            }
            catch (Exception ex)
            {
                try
                {
                    File.AppendAllText(Constant.PluginFileLocation + @"\WhatsAppPayloadRedactError.txt",
                        Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine
                        + $"requestBody length: {requestBody.Length}" + Environment.NewLine
                        + ex.Message);
                }
                catch { }
                // Purely diagnostic - never surface this to the user, never block the caller.
                return requestBody;
            }
        }

        private static void RedactBase64Values(JToken token)
        {
            if (token is JObject obj)
            {
                foreach (var prop in obj.Properties())
                    RedactBase64Values(prop.Value);
            }
            else if (token is JArray arr)
            {
                foreach (var item in arr)
                    RedactBase64Values(item);
            }
            else if (token.Type == JTokenType.String)
            {
                string value = token.Value<string>();
                if (!string.IsNullOrEmpty(value) && value.StartsWith("data:") && value.Contains(";base64,"))
                {
                    int commaIndex = value.IndexOf(";base64,", StringComparison.Ordinal) + ";base64,".Length;
                    string prefix = value.Substring(0, commaIndex);
                    int blobLength = value.Length - commaIndex;
                    ((JValue)token).Value = $"{prefix}[REDACTED:{blobLength} chars]";
                }
            }
        }

        private void DeleteUnsavedImages(TransactionData objTD)
        {
            try
            {
                if (objTD.TransactionDetails.Count < 1)
                    return;

                // 1️⃣ Collect saved image file names (HashSet = FAST lookup)
                HashSet<string> savedFileNames = objTD.TransactionDetails
                    .SelectMany(td => td.CameraImages)
                    .Select(ci => Path.GetFileName(ci.ImageLocation))
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);


                List<TransactionDetail> List = objTD.TransactionDetails.ToList();

                foreach (var item in List)
                {
                    if (!string.IsNullOrEmpty(item.LeftPlateImageName))
                        savedFileNames.Add(item.LeftPlateImageName);

                    if (!string.IsNullOrEmpty(item.RightPlateImageName))
                        savedFileNames.Add(item.RightPlateImageName);
                }

                // 2️⃣ Image folder path
                string imageFolderPath = Path.Combine(
                    CameraConstant.CameraImageLocation,
                    objTD.TicketID.ToString()
                );



                if (!Directory.Exists(imageFolderPath))
                    return;

                DirectoryInfo dir = new DirectoryInfo(imageFolderPath);

                // 3️⃣ Delete files not saved in DB
                foreach (FileInfo file in dir.GetFiles())
                {
                    if (!savedFileNames.Contains(file.Name))
                    {
                        file.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                // LOG THIS (VERY IMPORTANT)
                // Logger.Error(ex, "DeleteUnsavedImages failed");
            }
        }


        private void LoadImageInReceipt(ReceiptDetailLoadEventArgs obj)
        {
            TransactionData objTD = obj.tranData as TransactionData;

            DeleteUnsavedImages(objTD);

            ReportViewer reportViewer = obj.reportViewer as ReportViewer;
            DataTable ImagesDT = new DataTable();
            var ImagesSQL = "";
            if (objTD.TransactionMode == TransactionMode.Multi.ToString())
                ImagesSQL = "SELECT ISNULL(ImageLocation, '') AS ImageName, C.CameraName, (TD.SequenceNo + 1) As PassNo FROM CameraImage CI INNER JOIN Camera C ON c.CameraID = CI.CameraID INNER JOIN TransactionDetail TD ON TD.TransactionDetailID = CI.TransactionDetailID WHERE CameraImageID IN ( SELECT MAX(CameraImageID) FROM CameraImage CI INNER JOIN TransactionDetail TD ON TD.TransactionDetailID = CI.TransactionDetailID AND TD.ReceiptTicketID = '" + objTD.ReceiptTicketID + "' GROUP BY CameraID, CI.TransactionDetailID)";
            else if (objTD.TransactionMode == TransactionMode.Double.ToString())
                ImagesSQL = "SELECT ISNULL(ImageLocation, '') AS ImageName, C.CameraName, (TD.SequenceNo + 1) As PassNo FROM CameraImage CI INNER JOIN Camera C ON c.CameraID = CI.CameraID INNER JOIN TransactionDetail TD ON TD.TransactionDetailID = CI.TransactionDetailID WHERE CameraImageID IN ( SELECT MAX(CameraImageID) FROM CameraImage CI INNER JOIN TransactionDetail TD ON TD.TransactionDetailID = CI.TransactionDetailID AND TD.ReceiptTicketID = '" + objTD.ReceiptTicketID + "' GROUP BY CameraID, CI.TransactionDetailID)";
            else if (objTD.TransactionMode == TransactionMode.Single.ToString())
                ImagesSQL = "SELECT ISNULL(ImageLocation, '') AS ImageName, C.CameraName, TD.SequenceNo As PassNo FROM CameraImage CI INNER JOIN Camera C ON c.CameraID = CI.CameraID INNER JOIN TransactionDetail TD ON TD.TransactionDetailID = CI.TransactionDetailID WHERE CameraImageID IN ( SELECT MAX(CameraImageID) FROM CameraImage CI INNER JOIN TransactionDetail TD ON TD.TransactionDetailID = CI.TransactionDetailID AND TD.ReceiptTicketID = '" + objTD.ReceiptTicketID + "' GROUP BY CameraID, CI.TransactionDetailID)";
            SqlConnection con = new SqlConnection(System.Windows.Application.Current.Properties["DefalutConnectionString"].ToString());
            SqlCommand cmdImages = new SqlCommand(ImagesSQL, con);
            cmdImages.CommandType = CommandType.Text;
            cmdImages.CommandTimeout = 45;


            con.Open();
            ImagesDT.Load(cmdImages.ExecuteReader());
            if (ImagesDT != null && ImagesDT.Rows.Count > 0)
            {
                foreach (DataColumn col in ImagesDT.Columns)
                    col.ReadOnly = false;
                foreach (DataRow item in ImagesDT.Rows)
                {
                    string basePath = CameraConstant.CameraImageLocation + "\\" + objTD.TicketID; // could be C:\... or \\Server\Share...
                    string fileName = item["ImageName"].ToString();

                    string fullPath = Path.Combine(basePath, fileName);

                    // Convert to proper file:// URI
                    string fileUri = new Uri(fullPath).AbsoluteUri;

                    item["ImageName"] = fileUri;
                }
            }
            con.Close();
            ReportParameter[] reportParameterCollection = new ReportParameter[16];
            ReportParameter[] reportParamHasNoImages = new ReportParameter[16];
            ReportParameter[] reportParamHasNoPasses = new ReportParameter[16];
            ReportParameter[] reportParameterPasses = new ReportParameter[16];
            for (int i = 1; i <= 16; i++)
            {
                string Embeddsource = string.Empty;
                string passNo = string.Empty;
                bool hasNoImage = true;
                bool hasNoPass = true;
                if (ImagesDT.Rows.Count >= i && ImagesDT.Rows[i - 1]["ImageName"] != null)
                {
                    Embeddsource = ImagesDT.Rows[i - 1]["ImageName"].ToString();
                    File.WriteAllText(Constant.ApplicationDataLocation + @"\Receiptimages.txt", Embeddsource + Environment.NewLine);
                    passNo = ImagesDT.Rows[i - 1]["CameraName"].ToString() + " Weighment: #" + ImagesDT.Rows[i - 1]["PassNo"].ToString() + "     ";
                    hasNoImage = false;
                    hasNoPass = false;
                }

                reportParameterCollection[i - 1] = new ReportParameter("Image" + i + "source", Embeddsource);
                reportParamHasNoImages[i - 1] = new ReportParameter("HasNoImage" + i + "source", hasNoImage ? bool.TrueString : bool.FalseString);
                reportParameterPasses[i - 1] = new ReportParameter("PassNo" + i, passNo);
                reportParamHasNoPasses[i - 1] = new ReportParameter("HasNoPass" + i + "source", hasNoPass ? bool.TrueString : bool.FalseString);
            }
            reportViewer.LocalReport.SetParameters(reportParameterCollection);
            reportViewer.LocalReport.SetParameters(reportParamHasNoImages);
            reportViewer.LocalReport.SetParameters(reportParameterPasses);
            reportViewer.LocalReport.SetParameters(reportParamHasNoPasses);


        }
        #endregion
    }
}
