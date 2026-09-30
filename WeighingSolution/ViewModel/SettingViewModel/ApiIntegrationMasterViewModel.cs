using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Scriban;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;
using WeighingSolution.SharedComponents;
using ScribanTemplate = Scriban.Template;


namespace WeighingSolution.ViewModel
{
    public class ApiIntegrationMasterViewModel : NotifyPropertyChanged
    {
        private readonly DataRepository dataRepository;
        private readonly ApiIntegrationService apiService = new ApiIntegrationService();

        public ApiIntegrationMasterViewModel()
        {
            dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));

            NewCommand = new DelegateCommand(OnNewExecute);
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute, () => SelectedIntegration != null);
            // was: TriggerEvents = new ObservableCollection<string> { "OnSave", "OnEndTransaction", "Both" };
            TriggerEvents = new ObservableCollection<string> { "Active", "Complete", "Manual" };
            //AddHeaderCommand = new DelegateCommand(() => SelectedIntegration?.ApiHeaderRows.Add(NewHeaderRow()));
            //RemoveHeaderCommand = new DelegateCommand<ApiHeaderRow>(row => SelectedIntegration?.ApiHeaderRows.Remove(row));

            //AddParamCommand = new DelegateCommand(() => SelectedIntegration?.ApiParamRows.Add(NewParamRow()));
            //RemoveParamCommand = new DelegateCommand<ApiParamRow>(row => SelectedIntegration?.ApiParamRows.Remove(row));

            //AddBodyFieldCommand = new DelegateCommand(() => SelectedIntegration?.ApiBodyFieldRows.Add(NewBodyFieldRow()));
            AddHeaderCommand = new DelegateCommand(() => SelectedIntegration?.ApiHeaderRows.Add(NewHeaderRow()));
            RemoveHeaderCommand = new DelegateCommand<ApiHeaderRow>(row =>
            {
                if (row == null || SelectedIntegration == null) return;
                SelectedIntegration.ApiHeaderRows.Remove(row);
                if (row.HeaderRowID != 0)               // adjust to actual PK name
                    dataRepository.Context.ApiHeaderRows.Remove(row);
            });

            AddParamCommand = new DelegateCommand(() => SelectedIntegration?.ApiParamRows.Add(NewParamRow()));
            RemoveParamCommand = new DelegateCommand<ApiParamRow>(row =>
            {
                if (row == null || SelectedIntegration == null) return;
                SelectedIntegration.ApiParamRows.Remove(row);
                if (row.ParamRowID != 0)                // adjust to actual PK name
                    dataRepository.Context.ApiParamRows.Remove(row);
            });

            AddBodyFieldCommand = new DelegateCommand(() => SelectedIntegration?.ApiBodyFieldRows.Add(NewBodyFieldRow()));
            RemoveBodyFieldCommand = new DelegateCommand<ApiBodyFieldRow>(row =>
            {
                if (row == null || SelectedIntegration == null) return;
                SelectedIntegration.ApiBodyFieldRows.Remove(row);
                if (row.BodyFieldRowID != 0)            // adjust to actual PK name
                    dataRepository.Context.ApiBodyFieldRows.Remove(row);
            });
            RemoveBodyFieldCommand = new DelegateCommand<ApiBodyFieldRow>(row => SelectedIntegration?.ApiBodyFieldRows.Remove(row));

            SendTestCommand = new DelegateCommand(async () => await OnSendTestExecuteAsync(), () => SelectedIntegration != null && !IsTesting);

            HttpMethods = new ObservableCollection<string> { "GET", "POST", "PUT", "PATCH", "DELETE" };
            AuthModes = new ObservableCollection<string> { "NoAuth", "ApiKey", "BearerToken", "JWT", "BasicAuth", "OAuth2" };
            BodyModes = new ObservableCollection<string> { "None", "RawJson", "RawText", "RawXml", "FormData", "UrlEncoded", "WhatsApp" };
            TriggerEvents = new ObservableCollection<string> { "OnSave", "OnEndTransaction", "Manual", "Both" };

            LoadIntegrations();
            BuildFieldPickerLists();
        }



        #region Master list / selection

        private ObservableCollection<ApiIntegrationMaster> _integrations;
        public ObservableCollection<ApiIntegrationMaster> Integrations
        {
            get => _integrations;
            set { _integrations = value; OnPropertyChanged(); }
        }

        private ApiIntegrationMaster _selectedIntegration;
        public ApiIntegrationMaster SelectedIntegration
        {
            get => _selectedIntegration;
            set
            {
                if (_selectedIntegration != null)
                    _selectedIntegration.PropertyChanged -= SelectedIntegration_PropertyChanged;

                _selectedIntegration = value;

                if (_selectedIntegration != null)
                    _selectedIntegration.PropertyChanged += SelectedIntegration_PropertyChanged;

                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedAuthFieldDefinitions));
                DeleteCommand.RaiseCanExecuteChanged();
                SendTestCommand.RaiseCanExecuteChanged();
                SaveCommand.RaiseCanExecuteChanged();   // <-- also here, on selection change
                TestResponseBody = string.Empty;
                TestStatusCode = null;
            }
        }

        private string _fieldSearchText;
        public string FieldSearchText
        {
            get => _fieldSearchText;
            set
            {
                _fieldSearchText = value;
                OnPropertyChanged();
                TransactionFieldsView?.Refresh();
                DetailFieldsView?.Refresh();
            }
        }

        public ICollectionView TransactionFieldsView { get; private set; }
        public ICollectionView DetailFieldsView { get; private set; }

        private bool FilterField(object obj)
        {
            if (string.IsNullOrWhiteSpace(FieldSearchText)) return true;
            if (!(obj is TemplateFieldItem item)) return true;

            return item.DisplayName.IndexOf(FieldSearchText, StringComparison.OrdinalIgnoreCase) >= 0
                || item.Placeholder.IndexOf(FieldSearchText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void SelectedIntegration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveCommand.RaiseCanExecuteChanged();
        }
        public ObservableCollection<string> HttpMethods { get; }
        public ObservableCollection<string> AuthModes { get; }
        public ObservableCollection<string> BodyModes { get; }
        public ObservableCollection<string> TriggerEvents { get; }

        public ObservableCollection<TemplateFieldItem> TransactionFields { get; private set; }
        public ObservableCollection<TemplateFieldItem> DetailFields { get; private set; }
        public ObservableCollection<TemplateFieldItem> SpecialSnippets { get; private set; }

        #endregion

        #region Auth field definitions per AuthMode (drives the dynamic Authorization panel)

        // Field names shown for the currently selected AuthMode - the XAML ItemsControl
        // binds to this list to render the right textboxes without any hardcoded XAML per mode.
        private static readonly Dictionary<string, string[]> AuthFieldMap = new Dictionary<string, string[]>
        {
            ["NoAuth"] = new string[0],
            ["ApiKey"] = new[] { "Key", "Value", "AddTo" },              // AddTo = "Header" or "Query"
            ["BearerToken"] = new[] { "Token", "Prefix" },
            ["JWT"] = new[] { "Secret", "Claims", "HeaderPrefix" },
            ["BasicAuth"] = new[] { "Username", "Password" },
            ["OAuth2"] = new[] { "TokenUrl", "ClientId", "ClientSecret", "Scope", "AccessToken" },
        };

        public IEnumerable<string> SelectedAuthFieldDefinitions =>
            SelectedIntegration != null && AuthFieldMap.ContainsKey(SelectedIntegration.AuthMode)
                ? AuthFieldMap[SelectedIntegration.AuthMode]
                : Enumerable.Empty<string>();

        // Call this whenever the AuthMode ComboBox selection changes (see xaml.cs SelectionChanged handler)
        //public void RegenerateAuthFields()
        //{
        //    if (SelectedIntegration == null) return;

        //    var required = AuthFieldMap.ContainsKey(SelectedIntegration.AuthMode)
        //        ? AuthFieldMap[SelectedIntegration.AuthMode]
        //        : new string[0];

        //    var existing = SelectedIntegration.ApiAuthFields.ToDictionary(f => f.FieldName, f => f.FieldValue);
        //    SelectedIntegration.ApiAuthFields.Clear();

        //    foreach (var name in required)
        //    {
        //        SelectedIntegration.ApiAuthFields.Add(new ApiAuthField
        //        {
        //            ApiIntegrationID = SelectedIntegration.ApiIntegrationID,
        //            FieldName = name,
        //            FieldValue = existing.ContainsKey(name) ? existing[name] : string.Empty,
        //            IsSecret = name.IndexOf("Secret", StringComparison.OrdinalIgnoreCase) >= 0
        //                    || name.IndexOf("Password", StringComparison.OrdinalIgnoreCase) >= 0
        //                    || name.IndexOf("Token", StringComparison.OrdinalIgnoreCase) >= 0
        //        });
        //    }

        //    OnPropertyChanged(nameof(SelectedAuthFieldDefinitions));
        //}

        private void BuildFieldPickerLists()
        {
            var customFieldDisplayNames = LoadCustomFieldDisplayNames();

            TransactionFields = new ObservableCollection<TemplateFieldItem>(
                ApiTemplateBuilder.GetTransactionFieldMetadata(customFieldDisplayNames)
                    .Select(f => new TemplateFieldItem
                    {
                        DisplayName = f.DisplayName,
                        Placeholder = $"{{{{ transaction.{f.Key} }}}}"
                    }));

            DetailFields = new ObservableCollection<TemplateFieldItem>(
                ApiTemplateBuilder.GetDetailFieldMetadata()
                    .Select(f => new TemplateFieldItem
                    {
                        DisplayName = f.DisplayName,
                        Placeholder = $"{{{{ detail.{f.Key} }}}}"
                    }));

            SpecialSnippets = new ObservableCollection<TemplateFieldItem>
            {

                new TemplateFieldItem { DisplayName = "Reciept(Pdf)", Placeholder = "data:application/pdf;base64,{{ transaction.receipt_pdf_base64 }}" },
                new TemplateFieldItem { DisplayName = "Phone Number (Manual)", Placeholder = "{{ transaction.manual_to_number }}" },

            };
            TransactionFieldsView = CollectionViewSource.GetDefaultView(TransactionFields);
            TransactionFieldsView.Filter = FilterField;

            DetailFieldsView = CollectionViewSource.GetDefaultView(DetailFields);
            DetailFieldsView.Filter = FilterField;

            OnPropertyChanged(nameof(TransactionFieldsView));
            OnPropertyChanged(nameof(DetailFieldsView));
        }

        private IDictionary<int, string> LoadCustomFieldDisplayNames()
        {
            var result = new Dictionary<int, string>();
            var customFields = dataRepository.Context.CustomFields.Where(x => x.IsVisible).ToList();

            foreach (var cf in customFields)
            {
                var match = Regex.Match(cf.FieldName ?? "", @"^field(\d+)$", RegexOptions.IgnoreCase);
                if (match.Success && int.TryParse(match.Groups[1].Value, out int num))
                    result[num] = cf.DisplayName;
            }
            return result;
        }
        // "vehicle_number" -> "Vehicle Number", "ticket_id" -> "Ticket Id", etc.
        private static string ToDisplayName(string snakeCaseKey)
        {
            var parts = snakeCaseKey.Split('_');
            return string.Join(" ", parts.Select(p => p.Length > 0 ? char.ToUpper(p[0]) + p.Substring(1) : p));
        }

        public void RegenerateAuthFields()
        {
            if (SelectedIntegration == null) return;

            var required = AuthFieldMap.ContainsKey(SelectedIntegration.AuthMode)
                ? AuthFieldMap[SelectedIntegration.AuthMode]
                : new string[0];

            var existing = SelectedIntegration.ApiAuthFields.ToDictionary(f => f.FieldName, f => f.FieldValue);

            // Don't just Clear() the collection - EF tries to null the FK on the removed
            // children, which throws because ApiIntegrationID is non-nullable. Delete the
            // rows from the context explicitly instead.
            var toRemove = SelectedIntegration.ApiAuthFields.ToList();
            foreach (var row in toRemove)
            {
                SelectedIntegration.ApiAuthFields.Remove(row);
                if (row.AuthFieldID != 0) // only rows already persisted need explicit delete
                    dataRepository.Context.ApiAuthFields.Remove(row);
            }

            foreach (var name in required)
            {
                SelectedIntegration.ApiAuthFields.Add(new ApiAuthField
                {
                    ApiIntegrationID = SelectedIntegration.ApiIntegrationID,
                    FieldName = name,
                    FieldValue = existing.ContainsKey(name) ? existing[name] : string.Empty,
                    IsSecret = name.IndexOf("Secret", StringComparison.OrdinalIgnoreCase) >= 0
                            || name.IndexOf("Password", StringComparison.OrdinalIgnoreCase) >= 0
                            || name.IndexOf("Token", StringComparison.OrdinalIgnoreCase) >= 0
                });
            }

            OnPropertyChanged(nameof(SelectedAuthFieldDefinitions));
        }

        #endregion

        #region Commands

        public DelegateCommand NewCommand { get; }
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand DeleteCommand { get; }

        public DelegateCommand AddHeaderCommand { get; }
        public DelegateCommand<ApiHeaderRow> RemoveHeaderCommand { get; }

        public DelegateCommand AddParamCommand { get; }
        public DelegateCommand<ApiParamRow> RemoveParamCommand { get; }

        public DelegateCommand AddBodyFieldCommand { get; }
        public DelegateCommand<ApiBodyFieldRow> RemoveBodyFieldCommand { get; }

        public DelegateCommand SendTestCommand { get; }

        #endregion

        #region Test / Send panel

        private bool _isTesting;
        public bool IsTesting
        {
            get => _isTesting;
            set { _isTesting = value; OnPropertyChanged(); SendTestCommand.RaiseCanExecuteChanged(); }
        }

        private string _testResponseBody;
        public string TestResponseBody
        {
            get => _testResponseBody;
            set { _testResponseBody = value; OnPropertyChanged(); }
        }

        private int? _testStatusCode;
        public int? TestStatusCode
        {
            get => _testStatusCode;
            set { _testStatusCode = value; OnPropertyChanged(); }
        }

        private bool _testIsSuccess;
        public bool TestIsSuccess
        {
            get => _testIsSuccess;
            set { _testIsSuccess = value; OnPropertyChanged(); }
        }

        private string _testRequestPreview;
        public string TestRequestPreview
        {
            get => _testRequestPreview;
            set { _testRequestPreview = value; OnPropertyChanged(); }
        }



        #endregion

        #region Methods

        private void LoadIntegrations()
        {
            var list = dataRepository.Context.ApiIntegrationMasters
                .Include(x => x.ApiHeaderRows)
                .Include(x => x.ApiParamRows)
                .Include(x => x.ApiAuthFields)
                .Include(x => x.ApiBodyFieldRows)
                .OrderBy(x => x.SortOrder)
                .ToList();

            Integrations = new ObservableCollection<ApiIntegrationMaster>(list);
        }

        private void OnNewExecute()
        {
            var newItem = new ApiIntegrationMaster
            {
                ClientCode = "NEW_CLIENT",     // your MetadataType buddy class may mark this [Required] - adjust default as needed
                Name = "New Integration",
                BaseUrl = string.Empty,
                Endpoint = string.Empty,
                HttpMethod = "POST",
                AuthMode = "NoAuth",
                BodyMode = "RawJson",
                RawBodyTemplate = string.Empty,
                TriggerOn = "Complete",
                IsActive = true,
                SortOrder = (Integrations?.Count ?? 0) + 1,
                CreatedTime = DateTime.Now
            };
            Integrations.Add(newItem);
            SelectedIntegration = newItem;
            SaveCommand.RaiseCanExecuteChanged();
        }

        private bool OnSaveCanExecute() => SelectedIntegration != null && !string.IsNullOrWhiteSpace(SelectedIntegration.Name);

        private void OnSaveExecute()
        {
            try
            {
                if (SelectedIntegration.ApiIntegrationID == 0)
                    dataRepository.Context.ApiIntegrationMasters.Add(SelectedIntegration);

                SelectedIntegration.ModifiedTime = DateTime.Now;
                dataRepository.Context.SaveChanges();
                ToastHelper.ShowSuccess("Integration saved successfully");
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // The top-level message is always the same generic string - the real
                // reason is inside EntityValidationErrors. Build a readable summary and
                // also log the full detail to file, same pattern as TransactionViewModel.
                var summary = new System.Text.StringBuilder();
                var logDetail = new System.Text.StringBuilder();
                logDetail.AppendLine(Environment.NewLine + "-------------------  " + DateTime.Now);

                foreach (var entityError in ex.EntityValidationErrors)
                {
                    logDetail.AppendLine($"Entity \"{entityError.Entry.Entity.GetType().Name}\" in state \"{entityError.Entry.State}\":");
                    foreach (var propError in entityError.ValidationErrors)
                    {
                        string line = $"- Property: \"{propError.PropertyName}\", Error: \"{propError.ErrorMessage}\"";
                        logDetail.AppendLine(line);
                        summary.AppendLine($"{propError.PropertyName}: {propError.ErrorMessage}");
                    }
                }

                try
                {
                    System.IO.File.AppendAllText(
                        Constant.PluginFileLocation + @"\ApiIntegrationSaveError.txt",
                        logDetail.ToString());
                }
                catch { /* logging failure shouldn't block the error toast */ }

                ToastHelper.ShowError("Failed to save integration:" + Environment.NewLine + summary.ToString());
            }
            catch (Exception ex)
            {
                ToastHelper.ShowError("Failed to save integration: " + ex.Message);
            }
            finally
            {
                SaveCommand.RaiseCanExecuteChanged();   // <-- add this
            }
        }

        private async void OnDeleteExecute()
        {
            if (SelectedIntegration == null) return;

            MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Do you wish to Delete integration?", "");
            if (!(msgbox_result == MessageDialogResult.Affirmative))
            {
                return;
            }
            if (SelectedIntegration.ApiIntegrationID != 0)
            {
                try
                {
                    // ApiIntegrationLog rows reference this integration via a non-cascading FK,
                    // so they have to go first or SaveChanges throws FK_ApiIntegrationLog_Master.
                    var relatedLogs = dataRepository.Context.ApiIntegrationLogs
                        .Where(x => x.ApiIntegrationID == SelectedIntegration.ApiIntegrationID)
                        .ToList();

                    if (relatedLogs.Count > 0)
                        dataRepository.Context.ApiIntegrationLogs.RemoveRange(relatedLogs);

                    dataRepository.Context.ApiIntegrationMasters.Remove(SelectedIntegration);
                    dataRepository.Context.SaveChanges();
                }
                catch (Exception ex)
                {
                    ToastHelper.ShowError("Failed to delete integration: " + ex.Message);
                    return; // don't touch the in-memory list if the DB delete failed
                }
            }

            Integrations.Remove(SelectedIntegration);
            SelectedIntegration = Integrations.FirstOrDefault();
            ToastHelper.ShowSuccess("Integration Deleted Successfully!");
        }

        // Fires the configured API using either sample/mock data or a real recent ticket,
        // exactly like clicking "Send" in Postman.
        private async Task OnSendTestExecuteAsync(TransactionData sampleTransaction = null, TransactionDetail sampleDetail = null)
        {
            if (SelectedIntegration == null) return;

            IsTesting = true;
            TestResponseBody = string.Empty;
            TestStatusCode = null;

            try
            {
                var transaction = sampleTransaction ?? BuildSampleTransaction();
                var detail = sampleDetail ?? transaction.TransactionDetails.FirstOrDefault();

                var result = await apiService.ExecuteAsync(SelectedIntegration, transaction, detail);

                TestRequestPreview = result.RequestBody;
                TestResponseBody = PrettyPrintIfJson(result.ResponseBody);
                TestStatusCode = result.StatusCode;
                TestIsSuccess = result.IsSuccess;

                if (result.IsSuccess)
                    ToastHelper.ShowSuccess($"Test call succeeded ({result.ElapsedMilliseconds} ms)");
                else
                    ToastHelper.ShowError("Test call failed: " + (result.ErrorMessage ?? "See response body"));
            }
            finally
            {
                IsTesting = false;
            }
        }

        // Public wrapper so the code-behind's Send button can call this directly
        public Task RunTestAsync() => OnSendTestExecuteAsync();

        private string PrettyPrintIfJson(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return raw;
            try { return JToken.Parse(raw).ToString(Formatting.Indented); }
            catch { return raw; }
        }

        // Your generated ApiHeaderRow / ApiParamRow / ApiBodyFieldRow entities don't share a
        // common base class (each is a separate EDMX-generated partial class), so instead of
        // one generic NewRow<T>, each row type gets its own small factory method.
        private ApiHeaderRow NewHeaderRow() => new ApiHeaderRow
        {
            ApiIntegrationID = SelectedIntegration.ApiIntegrationID,
            IsEnabled = true
        };

        private ApiParamRow NewParamRow() => new ApiParamRow
        {
            ApiIntegrationID = SelectedIntegration.ApiIntegrationID,
            IsEnabled = true,
            Location = "Query"
        };

        private ApiBodyFieldRow NewBodyFieldRow() => new ApiBodyFieldRow
        {
            ApiIntegrationID = SelectedIntegration.ApiIntegrationID,
            IsEnabled = true
        };

        // Builds a representative sample transaction so "Send" can be tested
        // without needing a real live ticket.
        private TransactionData BuildSampleTransaction()
        {
            var sample = new TransactionData
            {
                TicketID = 999,
                Status = "Complete",
                TransactionMode = "Single",
                TransactionType = "Incoming",
                Vehicle_Number = "GJ01AB1234",
                Account_Name = "Sample Account",
                Transporter_Name = "Sample Transporter",
                CreationTime = DateTime.Now,
                ReceiptTicketID = Guid.NewGuid()
            };
            sample.TransactionDetails = new ObservableCollection<TransactionDetail>
            {
                new TransactionDetail
                {
                    Product_Name = "Sample Product",
                    GrossWeight = 12500,
                    TareWeight = 10000,
                    NetWeight = 2500,
                    GrossTime = DateTime.Now.AddMinutes(-30),
                    TareTime = DateTime.Now
                }
            };
            return sample;
        }

        #endregion
    }


}


/// <summary>
/// Generic, config-driven API caller. One instance handles ANY client integration
/// stored in ApiIntegrationMaster - nothing here is client-specific.
/// bindingContext is normally the TransactionData (+ optional TransactionDetail)
/// exposed to templates as "transaction" / "detail" / "lines".
/// </summary>
/// <summary>
/// Plain result DTO returned from ExecuteAsync. NOT a database entity -
/// it doesn't need to exist in the EDMX, it just carries the outcome
/// of a single API call back to whoever called ExecuteAsync.
/// </summary>
/// 

public class ApiCallResult
{
    public int ApiIntegrationID { get; set; }
    public bool IsSuccess { get; set; }
    public int? StatusCode { get; set; }
    public string RequestUrl { get; set; }
    public string RequestBody { get; set; }
    public string ResponseBody { get; set; }
    public string ExternalReferenceId { get; set; }
    public string ErrorMessage { get; set; }
    public long ElapsedMilliseconds { get; set; }
}

/// <summary>
/// Generic, config-driven API caller. One instance handles ANY client integration
/// stored in ApiIntegrationMaster - nothing here is client-specific.
/// bindingContext is normally the TransactionData (+ optional TransactionDetail)
/// exposed to templates as "transaction" / "detail" / "lines".
/// </summary>
public class ApiIntegrationService
{
    private static readonly Regex PlaceholderRegex = new Regex(@"\{\{\s*([\w\.]+)\s*\}\}", RegexOptions.Compiled);
    private static void WritePayloadFile(
     ApiIntegrationMaster cfg,
     TransactionData model,
     string url,
     string bodyPreview)
    {
        try
        {
            string folder = Path.Combine(
                Constant.PluginFileLocation,
                "ApiPayloadLogs");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string safeIntegrationName = string.Join(
                "_",
                (cfg.Name ?? "Integration")
                    .Split(Path.GetInvalidFileNameChars()));

            string fileName =
                $"Ticket{model.TicketID}_{safeIntegrationName}_{DateTime.Now:yyyyMMdd_HHmmss_fff}.txt";

            string filePath = Path.Combine(folder, fileName);

            // ---------------------------------------------------------
            // PAYLOAD SIZE DIAGNOSTIC LOGS
            // ---------------------------------------------------------

            int payloadChars = bodyPreview?.Length ?? 0;

            long payloadBytes = 0;

            if (!string.IsNullOrEmpty(bodyPreview))
            {
                payloadBytes = Encoding.UTF8.GetByteCount(bodyPreview);
            }

            // Try to find Base64 data inside the payload
            int base64Length = 0;

            if (!string.IsNullOrEmpty(bodyPreview))
            {
                int base64Index = bodyPreview.IndexOf(
                    "base64,",
                    StringComparison.OrdinalIgnoreCase);

                if (base64Index >= 0)
                {
                    base64Length =
                        bodyPreview.Length - (base64Index + "base64,".Length);
                }
            }

            string content =
                Environment.NewLine +
                "-------------------  " +
                DateTime.Now +
                Environment.NewLine +

                $"Integration: {cfg.Name} (ID: {cfg.ApiIntegrationID})" +
                Environment.NewLine +

                $"TicketID: {model.TicketID}" +
                Environment.NewLine +

                $"ReceiptTicketID: {model.ReceiptTicketID}" +
                Environment.NewLine +

                $"Method: {cfg.HttpMethod}" +
                Environment.NewLine +

                $"URL: {url}" +
                Environment.NewLine +

                "---- Payload Diagnostics ----" +
                Environment.NewLine +

                $"Payload Characters: {payloadChars:N0}" +
                Environment.NewLine +

                $"Payload UTF8 Bytes: {payloadBytes:N0}" +
                Environment.NewLine +

                $"Base64 Characters: {base64Length:N0}" +
                Environment.NewLine +

                $"Approx Payload Size MB: {(payloadBytes / 1024.0 / 1024.0):N2} MB" +
                Environment.NewLine +

                $"Payload Starts With: " +
                (string.IsNullOrEmpty(bodyPreview)
                    ? "[EMPTY]"
                    : bodyPreview.Substring(
                        0,
                        Math.Min(200, bodyPreview.Length))) +
                Environment.NewLine +

                $"Payload Ends With: " +
                (string.IsNullOrEmpty(bodyPreview)
                    ? "[EMPTY]"
                    : bodyPreview.Substring(
                        Math.Max(0, bodyPreview.Length - 200))) +
                Environment.NewLine +

                "---- Payload ----" +
                Environment.NewLine +

                bodyPreview;

            File.WriteAllText(filePath, content);
        }
        catch
        {
            // Payload logging is diagnostic only.
        }
    }
    public async Task<ApiCallResult> ExecuteAsync(ApiIntegrationMaster cfg, TransactionData model,
      TransactionDetail activeDetail = null, IDictionary<string, object> extraFields = null)
    {
        var postRenderReplacements = new Dictionary<string, string>();
        var result = new ApiCallResult { ApiIntegrationID = cfg.ApiIntegrationID };
        var sw = Stopwatch.StartNew();

        try
        {
            // ONE stable set of field names for every client - see ApiTemplateBuilder.
            // The RawBodyTemplate per integration decides the JSON shape, not this code.
            var (transaction, detail, lines) = ApiTemplateBuilder.Build(model, activeDetail, extraFields);

            //if (cfg.AttachReceiptPdf)
            //{
            //    string receiptBase64 =
            //        await GetReceiptPdfBase64Async(model.ReceiptTicketID);

            //    WriteDiagnosticLog(
            //        cfg,
            //        model,
            //        "PDF BASE64 GENERATED",
            //        $"Base64 Length: {receiptBase64?.Length:N0}" +
            //        Environment.NewLine +
            //        $"Base64 Size MB: {(receiptBase64?.Length ?? 0) / 1024.0 / 1024.0:N2}");

            //    transaction["receipt_pdf_base64"] = receiptBase64;
            //}

            bool alreadyHasPdf = extraFields != null
      && extraFields.TryGetValue("receipt_pdf_base64", out var existingPdf)
      && !string.IsNullOrEmpty(existingPdf as string);

            //if (cfg.AttachReceiptPdf)
            //{
            //    try
            //    {
            //        // ---- TEMP DEV OVERRIDE - REMOVE BEFORE PRODUCTION ----
            //        // Feeds the client's real test.pdf straight in, bypassing ReceiptPreview/
            //        // GetReceiptPdfBase64Async entirely, so you can isolate whether a failure
            //        // is in the report-rendering path or somewhere downstream (template,
            //        // HttpClient, vendor API) using a known-good, already-complete PDF.
            //        const string tempTestPdfPath = @"C:\Users\Hetul shah\Desktop\New folder\test.pdf";

            //        if (File.Exists(tempTestPdfPath))
            //        {
            //            byte[] tempBytes = File.ReadAllBytes(tempTestPdfPath);
            //            string receiptBase64 = Convert.ToBase64String(tempBytes);
            //            transaction["receipt_pdf_base64"] = receiptBase64;
            //            System.Diagnostics.Debug.WriteLine(
            //                $"[TEMP] Loaded test PDF from disk: {tempBytes.Length:N0} bytes, " +
            //                $"base64 length: {(transaction["receipt_pdf_base64"] as string)?.Length:N0}");

            //            string placeholder = "__PDF_BASE64_" + Guid.NewGuid().ToString("N") + "__";
            //            transaction["receipt_pdf_base64"] = placeholder;
            //            postRenderReplacements[placeholder] = receiptBase64;
            //        }
            //        else
            //        {
            //            throw new FileNotFoundException("Temp test PDF not found at " + tempTestPdfPath);
            //        }
            //        // ---- END TEMP DEV OVERRIDE ----

            //        // Real path (put this back once you're done debugging):
            //        // transaction["receipt_pdf_base64"] = await GetReceiptPdfBase64Async(model.ReceiptTicketID);
            //    }
            //    catch (Exception pdfEx)
            //    {
            //        result.IsSuccess = false;
            //        result.ErrorMessage = "Receipt PDF generation failed: " + pdfEx.Message;
            //        return result;
            //    }
            //}

            if (cfg.AttachReceiptPdf && !alreadyHasPdf)
            {
                try
                {
                    string receiptBase64 = await GetReceiptPdfBase64Async(model.ReceiptTicketID);
                    string placeholder = "__PDF_BASE64_" + Guid.NewGuid().ToString("N") + "__";
                    transaction["receipt_pdf_base64"] = placeholder;
                    postRenderReplacements[placeholder] = receiptBase64;
                }
                catch (Exception pdfEx)
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = "Receipt PDF generation failed: " + pdfEx.Message;
                    return result;
                }
            }


            var scriptObject = new ScriptObject();
            scriptObject["transaction"] = transaction;
            scriptObject["detail"] = detail;
            scriptObject["lines"] = lines;

            var templateContext = new TemplateContext();
            templateContext.PushGlobal(scriptObject);

            // Scriban caps total rendered output at ~1MB by default (a safety net against
            // runaway loops). Receipt PDFs embedded as base64 can legitimately exceed that,
            // so raise the ceiling explicitly rather than let it silently truncate output.
            //templateContext.MaxOutputByteSize = 20 * 1024 * 1024; // e.g. 20MB, adjust to your max realistic PDF size

            string url = BuildUrl(cfg, transaction, detail);

            var request = new HttpRequestMessage(new HttpMethod(cfg.HttpMethod), url);
            request.Headers.ExpectContinue = false;   // add this line in ExecuteAsync
            request.Headers.UserAgent.ParseAdd("WeighingSolution/1.0");
            request.Headers.Accept.ParseAdd("*/*");
            if (request.Headers.Connection.Count == 0)
                request.Headers.Connection.Add("keep-alive");
            // ---- Headers ----
            foreach (var h in cfg.ApiHeaderRows.Where(x => x.IsEnabled && !string.IsNullOrWhiteSpace(x.Key)))
                request.Headers.TryAddWithoutValidation(h.Key, ResolvePlaceholders(h.Value, transaction, detail));

            // ---- Authorization ----
            ApplyAuth(request, cfg, transaction, detail);

            // ---- Body ----
            var (bodyContent, bodyPreview) = await BuildBodyAsync(cfg, templateContext, transaction, detail);

            // Swap placeholders back to the real base64 AFTER Scriban has finished -
            // this is plain string.Replace, not template interpolation, so no size cap applies.
            if (postRenderReplacements.Count > 0 && bodyPreview != null)
            {
                foreach (var kv in postRenderReplacements)
                    bodyPreview = bodyPreview.Replace(kv.Key, kv.Value);

                bodyContent = new StringContent(bodyPreview, Encoding.UTF8, cfg.ContentTypeFor(cfg.BodyMode) ?? "application/json");
            }

            long bodyBytes = Encoding.UTF8.GetByteCount(bodyPreview ?? "");
            WriteDiagnosticLog(cfg, model, "BODY GENERATED", ""); // unchanged



            WriteDiagnosticLog(
                cfg,
                model,
                "BODY GENERATED",
                $"Body Characters: {bodyPreview?.Length:N0}" +
                Environment.NewLine +
                $"Body UTF8 Bytes: {bodyBytes:N0}" +
                Environment.NewLine +
                $"Body Size MB: {bodyBytes / 1024.0 / 1024.0:N2}");
            request.Content = bodyContent;
            if (request.Content != null && !string.IsNullOrEmpty(cfg.ContentTypeFor(cfg.BodyMode)))
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(cfg.ContentTypeFor(cfg.BodyMode));

            WritePayloadFile(cfg, model, url, bodyPreview);

            using (var client = new HttpClient())
            {
                int bodySizeMB = (int)(bodyBytes / (1024 * 1024));
                client.Timeout = TimeSpan.FromSeconds(Math.Max(30, 30 + bodySizeMB * 20));

                WriteDiagnosticLog(
    cfg,
    model,
    "BEFORE SEND",
    $"Content-Length: " +
    $"{request.Content?.Headers.ContentLength?.ToString() ?? "NULL"}" +
    Environment.NewLine +
    $"Body Characters: {bodyPreview?.Length:N0}" +
    Environment.NewLine +
    $"Body Bytes: {Encoding.UTF8.GetByteCount(bodyPreview ?? ""):N0}");

                var response = await client.SendAsync(request);
                string respBody = await response.Content.ReadAsStringAsync();
                WriteDiagnosticLog(
    cfg,
    model,
    "AFTER SEND",
    $"HTTP Status: {(int)response.StatusCode}" +
    Environment.NewLine +
    $"Status: {response.StatusCode}" +
    Environment.NewLine +
    $"Response: {respBody}");
                result.RequestUrl = request.RequestUri.ToString();
                // Use the preview captured BEFORE SendAsync - request.Content's stream is
                // already consumed/disposed by HttpClient after the send, re-reading it
                // here throws ObjectDisposedException.
                result.RequestBody = bodyPreview;
                result.StatusCode = (int)response.StatusCode;
                result.ResponseBody = respBody;
                result.IsSuccess = EvaluateSuccess(cfg, respBody, response.IsSuccessStatusCode);

                if (result.IsSuccess && !string.IsNullOrEmpty(cfg.ResponseExternalIdPath))
                {
                    try
                    {
                        var token = JObject.Parse(respBody).SelectToken(cfg.ResponseExternalIdPath);
                        result.ExternalReferenceId = token?.ToString();
                    }
                    catch { /* response wasn't JSON or path missing - ignore */ }
                }
            }
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;
        }
        finally
        {
            sw.Stop();
            result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        }

        return result;
    }

    //private static Task<string> GetReceiptPdfBase64Async(Guid receiptTicketId)
    //{
    //    var dispatcher = System.Windows.Application.Current?.Dispatcher;
    //    if (dispatcher == null)
    //        throw new InvalidOperationException("No WPF dispatcher available to render the receipt.");

    //    // ReceiptPreview is a WPF Page hosting a WinForms ReportViewer - both need to be
    //    // created on the app's UI/STA thread, not a background ThreadPool thread. Since
    //    // ApiIntegrationSyncService's 30-minute sweep runs off-thread, this hops back onto
    //    // the UI thread just for the render, then returns.
    //    return dispatcher.InvokeAsync(() =>
    //    {
    //        var preview = new WeighingSolution.View.ReceiptPreview(receiptTicketId, endtransaction: false);
    //        return Convert.ToBase64String(preview.RenderToPdfBytes());
    //    }).Task;
    //}
    private static void WriteDiagnosticLog(
    ApiIntegrationMaster cfg,
    TransactionData model,
    string stage,
    string details)
    {
        try
        {
            string folder = Path.Combine(
                Constant.PluginFileLocation,
                "ApiPayloadLogs");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string safeName = string.Join(
                "_",
                (cfg.Name ?? "Integration")
                    .Split(Path.GetInvalidFileNameChars()));

            string fileName =
                $"Diagnostic_Ticket{model.TicketID}_{DateTime.Now:yyyyMMdd_HHmmss_fff}.txt";

            string filePath = Path.Combine(folder, fileName);

            string content =
                $"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}" +
                Environment.NewLine +
                $"Integration: {cfg.Name} (ID: {cfg.ApiIntegrationID})" +
                Environment.NewLine +
                $"TicketID: {model.TicketID}" +
                Environment.NewLine +
                $"ReceiptTicketID: {model.ReceiptTicketID}" +
                Environment.NewLine +
                $"Stage: {stage}" +
                Environment.NewLine +
                details +
                Environment.NewLine +
                "----------------------------------------" +
                Environment.NewLine;

            File.AppendAllText(filePath, content);
        }
        catch
        {
            // Diagnostic logging must never affect API execution.
        }
    }
    //private static Task<string> GetReceiptPdfBase64Async(Guid receiptTicketId)
    //{
    //    var dispatcher = System.Windows.Application.Current?.Dispatcher;

    //    if (dispatcher == null)
    //        throw new InvalidOperationException(
    //            "No WPF dispatcher available to render the receipt.");

    //    return dispatcher.InvokeAsync(() =>
    //    {
    //        var preview = new WeighingSolution.View.ReceiptPreview(
    //            receiptTicketId,
    //            endtransaction: false);

    //        byte[] pdfBytes = preview.RenderToPdfBytes();

    //        Debug.WriteLine($"Receipt PDF size: {pdfBytes.Length:N0} bytes");

    //        string base64 = Convert.ToBase64String(pdfBytes);

    //        Debug.WriteLine($"Base64 size: {base64.Length:N0} characters");

    //        string dataUri = "data:application/pdf;base64," + base64;

    //        Debug.WriteLine($"Data URI size: {dataUri.Length:N0} characters");

    //        // Verify conversion
    //        byte[] decodedBytes = Convert.FromBase64String(base64);

    //        Debug.WriteLine(
    //            $"Decoded PDF size: {decodedBytes.Length:N0} bytes");

    //        if (decodedBytes.Length != pdfBytes.Length)
    //        {
    //            throw new InvalidOperationException(
    //                "Base64 conversion verification failed.");
    //        }

    //        return dataUri;

    //    }).Task;
    //}

    private static async Task<string> GetReceiptPdfBase64Async(Guid receiptTicketId)
    {
        var dispatcher = System.Windows.Application.Current?.Dispatcher;
        if (dispatcher == null)
            throw new InvalidOperationException("No WPF dispatcher available to render the receipt.");

        const int maxAttempts = 4;
        const int retryDelayMs = 700;

        byte[] lastBytes = null;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            // Synchronous Invoke (not InvokeAsync) + Send priority so this call does not
            // return until the report is fully built and rendered on the UI thread -
            // removes any chance of grabbing bytes mid-render.
            byte[] bytes = await Task.Run(() =>
                (byte[])dispatcher.Invoke(new Func<byte[]>(() =>
                {
                    var preview = new WeighingSolution.View.ReceiptPreview(receiptTicketId, endtransaction: false);
                    return preview.RenderToPdfBytes();
                }), System.Windows.Threading.DispatcherPriority.Send));

            lastBytes = bytes;

            if (IsValidPdf(bytes))
                return Convert.ToBase64String(bytes);

            // Not a complete PDF yet (still binding/loading) - wait briefly and try again
            // rather than sending a truncated payload.
            if (attempt < maxAttempts)
                await Task.Delay(retryDelayMs);
        }

        // Never got a valid render after all retries - fail loudly instead of
        // silently sending a broken/partial PDF.
        throw new InvalidOperationException(
            $"Receipt PDF render did not complete after {maxAttempts} attempts for ticket {receiptTicketId} " +
            $"(last size: {lastBytes?.Length ?? 0} bytes).");
    }

    private static bool IsValidPdf(byte[] bytes)
    {
        // %PDF header + a sane minimum size - cheap guard against a truncated/half-built render
        return bytes != null && bytes.Length > 2048
            && bytes[0] == 0x25 && bytes[1] == 0x50 && bytes[2] == 0x44 && bytes[3] == 0x46; // "%PDF"
    }

    private string BuildUrl(ApiIntegrationMaster cfg, IDictionary<string, object> transaction, IDictionary<string, object> detail)
    {
        string baseUrl = (cfg.BaseUrl ?? string.Empty).TrimEnd('/');
        string endpoint = (cfg.Endpoint ?? string.Empty).TrimStart('/');
        string url = string.IsNullOrEmpty(endpoint) ? baseUrl : $"{baseUrl}/{endpoint}";
        url = ResolvePlaceholders(url, transaction, detail);   // supports {{transaction.ticket_id}} in path

        var queryParams = cfg.ApiParamRows
            .Where(p => p.IsEnabled && p.Location == "Query" && !string.IsNullOrWhiteSpace(p.Key))
            .Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(ResolvePlaceholders(p.Value, transaction, detail) ?? string.Empty)}")
            .ToList();

        if (queryParams.Count > 0)
            url += (url.Contains("?") ? "&" : "?") + string.Join("&", queryParams);

        return url;
    }

    private void ApplyAuth(HttpRequestMessage request, ApiIntegrationMaster cfg, IDictionary<string, object> transaction, IDictionary<string, object> detail)
    {
        string Get(string name) => cfg.ApiAuthFields.FirstOrDefault(f => f.FieldName == name)?.FieldValue;

        switch (cfg.AuthMode)
        {
            case "ApiKey":
                {
                    string addTo = Get("AddTo") ?? "Header";
                    string key = Get("Key");
                    string val = ResolvePlaceholders(Get("Value"), transaction, detail);
                    if (string.IsNullOrWhiteSpace(key)) break;

                    if (addTo == "Header")
                        request.Headers.TryAddWithoutValidation(key, val);
                    else
                    {
                        var sep = request.RequestUri.ToString().Contains("?") ? "&" : "?";
                        request.RequestUri = new Uri($"{request.RequestUri}{sep}{Uri.EscapeDataString(key)}={Uri.EscapeDataString(val ?? string.Empty)}");
                    }
                    break;
                }

            case "BearerToken":
                {
                    string prefix = Get("Prefix");
                    prefix = string.IsNullOrWhiteSpace(prefix) ? "Bearer" : prefix;
                    request.Headers.TryAddWithoutValidation("Authorization", $"{prefix} {Get("Token")}");
                    break;
                }

            case "JWT":
                {
                    // Plug in your JWT library of choice here (System.IdentityModel.Tokens.Jwt).
                    // Left as a hook - claims/secret are already captured as dynamic auth fields.
                    string prefix = Get("HeaderPrefix");
                    prefix = string.IsNullOrWhiteSpace(prefix) ? "Bearer" : prefix;
                    string jwt = Get("PrecomputedToken"); // or build dynamically from Secret + Claims
                    request.Headers.TryAddWithoutValidation("Authorization", $"{prefix} {jwt}");
                    break;
                }

            case "BasicAuth":
                {
                    var bytes = Encoding.UTF8.GetBytes($"{Get("Username")}:{Get("Password")}");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));
                    break;
                }

            case "OAuth2":
                {
                    // Token fetch typically cached; hook up your token-fetch/refresh logic here.
                    string token = Get("AccessToken");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    break;
                }

            case "NoAuth":
            default:
                break;
        }
    }

    private async Task<(HttpContent Content, string Preview)> BuildBodyAsync(ApiIntegrationMaster cfg, TemplateContext templateContext, IDictionary<string, object> transaction, IDictionary<string, object> detail)
    {
        switch (cfg.BodyMode)
        {
            case "None":
                return (null, string.Empty);

            case "RawJson":
            case "RawText":
            case "RawXml":
                {
                    var template = ScribanTemplate.Parse(cfg.RawBodyTemplate ?? string.Empty);
                    if (template.HasErrors)
                        throw new InvalidOperationException("Body template error: " + string.Join("; ", template.Messages));
                    string rendered = await template.RenderAsync(templateContext);

                    // Sanity check only - does NOT log the huge body, just proves it's well-formed
                    bool looksComplete = rendered.TrimEnd().EndsWith("}");
                    File.AppendAllText(Constant.PluginFileLocation + @"\RenderSanityCheck.txt",
                        $"{DateTime.Now}: Length={rendered.Length}, EndsWithBrace={looksComplete}, Last40Chars=\"{rendered.Substring(Math.Max(0, rendered.Length - 40))}\"{Environment.NewLine}");

                    if (!rendered.TrimEnd().EndsWith("}") && (cfg.BodyMode == "RawJson"))
                    {
                        throw new InvalidOperationException(
                            $"Rendered body appears truncated (length={rendered.Length}, does not end with '}}'). " +
                            "This usually means the template engine's output size limit was hit.");
                    }
                    string mediaType = cfg.BodyMode == "RawJson" ? "application/json"
                                      : cfg.BodyMode == "RawXml" ? "application/xml" : "text/plain";
                    return (new StringContent(rendered, Encoding.UTF8, mediaType), rendered);
                }

            case "UrlEncoded":
                {
                    var kvs = cfg.ApiBodyFieldRows.Where(f => f.IsEnabled && !string.IsNullOrWhiteSpace(f.Key))
                        .Select(f => new System.Collections.Generic.KeyValuePair<string, string>(
                            f.Key, ResolvePlaceholders(f.Value, transaction, detail) ?? string.Empty))
                        .ToList();
                    string preview = string.Join("&", kvs.Select(kv => $"{kv.Key}={kv.Value}"));
                    return (new FormUrlEncodedContent(kvs), preview);
                }

            case "FormData":
                {
                    var multipart = new MultipartFormDataContent();
                    var previewParts = new System.Collections.Generic.List<string>();
                    foreach (var f in cfg.ApiBodyFieldRows.Where(x => x.IsEnabled && !string.IsNullOrWhiteSpace(x.Key)))
                    {
                        if (f.IsFile && File.Exists(f.Value))
                        {
                            var bytes = File.ReadAllBytes(f.Value);
                            var fileContent = new ByteArrayContent(bytes);
                            multipart.Add(fileContent, f.Key, Path.GetFileName(f.Value));
                            previewParts.Add($"{f.Key}=@{Path.GetFileName(f.Value)}");
                        }
                        else
                        {
                            string val = ResolvePlaceholders(f.Value, transaction, detail) ?? string.Empty;
                            multipart.Add(new StringContent(val), f.Key);
                            previewParts.Add($"{f.Key}={val}");
                        }
                    }
                    return (multipart, string.Join("&", previewParts));
                }

            default:
                return (null, string.Empty);
        }
    }

    private bool EvaluateSuccess(ApiIntegrationMaster cfg, string responseBody, bool httpSuccess)
    {
        if (string.IsNullOrEmpty(cfg.ResponseSuccessPath))
            return httpSuccess;

        try
        {
            var token = JObject.Parse(responseBody).SelectToken(cfg.ResponseSuccessPath);
            return token != null && string.Equals(token.ToString(), cfg.ResponseSuccessValue, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return httpSuccess;
        }
    }

    /// <summary>
    /// Simple {{transaction.field}} / {{detail.field}} substitution for single-value
    /// fields (headers, params, auth values, URL). Looks up the SAME dictionary keys
    /// ApiTemplateBuilder produces - keeps this in sync with the Scriban body renderer,
    /// no reflection or property-name guessing involved.
    /// </summary>
    private string ResolvePlaceholders(string input, IDictionary<string, object> transaction, IDictionary<string, object> detail)
    {
        if (string.IsNullOrEmpty(input)) return input;

        return PlaceholderRegex.Replace(input, match =>
        {
            string path = match.Groups[1].Value;              // e.g. "transaction.vehicle_number"
            var parts = path.Split('.');
            if (parts.Length != 2) return match.Value;

            IDictionary<string, object> root = parts[0].Equals("transaction", StringComparison.OrdinalIgnoreCase) ? transaction
                                              : parts[0].Equals("detail", StringComparison.OrdinalIgnoreCase) ? detail
                                              : null;
            if (root == null) return match.Value;

            return root.TryGetValue(parts[1], out var value) ? (value?.ToString() ?? string.Empty) : string.Empty;
        });
    }
}

internal static class ApiIntegrationMasterExtensions
{
    // Central place mapping BodyMode -> default content-type (kept here so
    // the model class itself stays a plain POCO/EF entity).
    public static string ContentTypeFor(this ApiIntegrationMaster cfg, string bodyMode)
    {
        switch (bodyMode)
        {
            case "RawJson": return "application/json";
            case "RawXml": return "application/xml";
            case "RawText": return "text/plain";
            default: return null; // UrlEncoded / FormData set their own content-type
        }
    }
}


/// <summary>
/// Converts a TransactionData (+ its TransactionDetails) into a flat, predictable
/// dictionary of field names that every client integration template can rely on.
///
/// WHY THIS EXISTS:
/// Different clients want the same data in different SHAPES (flat JSON, wrapped in
/// "Data": {...}, nested inside "vals_list": [[0,0,{...}]], etc). Rather than writing
/// C# code per client, we expose ONE stable set of field names here, and let each
/// client's RawBodyTemplate (written in the API Master UI, per integration) decide
/// how to arrange them. Add a new field here once and it's available to every
/// client's template immediately - "transaction.xxx", "detail.xxx", or inside the
/// "lines" loop as "line.xxx".
/// </summary>
//public static class ApiTemplateBuilder
//{
//    private const string DateFormat = "yyyy-MM-dd HH:mm:ss";

//    /// <summary>
//    /// Builds the three context objects handed to the template engine:
//    /// - transaction : header-level ticket fields (vehicle, account, driver, ticket id...)
//    /// - detail      : the CURRENT pass/weighment being saved right now
//    /// - lines       : ALL passes/weighments on this ticket so far (for clients that
//    ///                 want every product/line in one call, e.g. Odoo's vals_list)
//    /// </summary>
//    public static (IDictionary<string, object> Transaction, IDictionary<string, object> Detail, List<IDictionary<string, object>> Lines)
//    Build(TransactionData model, TransactionDetail activeDetail, IDictionary<string, object> extraFields = null)
//    {
//        var transaction = BuildTransactionFields(model);
//        var detail = BuildLineFields(activeDetail);
//        var lines = (model.TransactionDetails ?? new ObservableCollection<TransactionDetail>()).Select(BuildLineFields).ToList();

//        if (extraFields != null)
//            foreach (var kv in extraFields)
//                transaction[kv.Key] = kv.Value;

//        return (transaction, detail, lines);
//    }



//    private static IDictionary<string, object> BuildTransactionFields(TransactionData model)
//    {
//        var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
//        if (model == null) return dict;

//        // ---- identifiers ----
//        dict["ticket_id"] = model.TicketID;
//        dict["name"] = $"Ticket {model.TicketID:000}";              // e.g. Odoo's "name": "Ticket 001"
//        dict["receipt_ticket_id"] = model.ReceiptTicketID;

//        // ---- vehicle / parties ----
//        dict["vehicle_number"] = model.Vehicle_Number;
//        dict["vehicle_id"] = model.VehicleID;
//        dict["driver_name"] = model.Driver_Name;
//        dict["driver_id"] = model.DriverID;
//        dict["account_name"] = model.Account_Name;
//        dict["account_id"] = model.AccountID;
//        dict["transporter_name"] = model.Transporter_Name;
//        dict["transporter_id"] = model.TransporterID;

//        // ---- status / type ----
//        dict["status"] = model.Status;
//        dict["transaction_type"] = model.TransactionType;
//        dict["transaction_mode"] = model.TransactionMode;

//        // ---- timestamps ----
//        dict["creation_time"] = model.CreationTime.ToString(DateFormat);
//        dict["print_time"] = DateTime.Now.ToString(DateFormat);

//        // ---- misc / integration bookkeeping ----
//        dict["source"] = "WeighingSolution";                        // override per-client in the template if needed

//        // ---- custom_value_1 .. custom_value_20 ----
//        // Your software already has a per-transaction "Custom Fields" feature
//        // (TransactionData.CustomValue1..CustomValue20, editable per-ticket via
//        // TransactionViewModel.CustomFieldss). Any field a client wants that has
//        // NO matching column here (SWAMA's WasteType, DumpingZoneCode, MaterialType,
//        // OperatorName, etc.) gets mapped to one of these when you onboard that
//        // client - decide "WasteType = CustomValue1" for THIS integration, tell the
//        // operator to fill that custom field in, then reference it in the template
//        // as {{ transaction.custom_value_1 }}. A different client can reuse
//        // CustomValue1 for something completely different - the mapping lives in
//        // the template text, not in this code.
//        foreach (var prop in typeof(TransactionData).GetProperties())
//        {
//            var match = Regex.Match(prop.Name, @"^CustomValue(\d+)$");
//            if (match.Success)
//            {
//                string key = $"custom_value_{match.Groups[1].Value}";
//                dict[key] = prop.GetValue(model);
//            }
//        }

//        return dict;
//    }

//    private static IDictionary<string, object> BuildLineFields(TransactionDetail detail)
//    {
//        var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
//        if (detail == null) return dict;

//        dict["product_id"] = detail.ProductID;
//        dict["product_name"] = detail.Product_Name;

//        dict["gross_weight"] = detail.GrossWeight;
//        dict["tare_weight"] = detail.TareWeight;
//        dict["net_weight"] = detail.NetWeight;

//        dict["gross_time"] = detail.GrossTime?.ToString(DateFormat);
//        dict["tare_time"] = detail.TareTime?.ToString(DateFormat);
//        dict["capture_time"] = detail.CaptureTime?.ToString(DateFormat);

//        dict["weighment_type"] = detail.WeighmentType;
//        dict["sequence_no"] = detail.SequenceNo;
//        dict["weight_unit"] = detail.WeightUnit;
//        dict["gate_name"] = detail.Gate_Name;

//        return dict;
//    }
//}


// NEW Dynamic field render 
public static class ApiTemplateBuilder
{
    private const string DateFormat = "yyyy-MM-dd HH:mm:ss";

    // Properties on TransactionData/TransactionDetail that are NOT template-able transaction
    // fields - navigation properties, child collections, EF internals. Add to this list if a
    // future nav property (e.g. a new master FK) starts leaking into the picker.
    private static readonly HashSet<string> ExcludedProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "TransactionDetails", "Vehicle", "Account", "Transporter", "Driver", "Product", "Gate",
        "AccountMaster", "TransporterMaster", "DriverMaster", "ProductMaster", "GateMaster", "ChargesMaster",
        "WeightBridge", "CameraImages"
    };

    public static (IDictionary<string, object> Transaction, IDictionary<string, object> Detail, List<IDictionary<string, object>> Lines)
        Build(TransactionData model, TransactionDetail activeDetail, IDictionary<string, object> extraFields = null,
              IDictionary<int, string> customFieldDisplayNames = null)
    {
        try
        {
            var transaction = BuildTransactionFields(model);
            var detail = BuildLineFields(activeDetail);
            var lines = (model.TransactionDetails ?? new ObservableCollection<TransactionDetail>()).Select(BuildLineFields).ToList();

            if (extraFields != null)
                foreach (var kv in extraFields)
                    transaction[kv.Key] = kv.Value;
            return (transaction, detail, lines);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message + " asdsfasdf");
            return (null, null, null);
        }


    }

    /// <summary>
    /// Ordered field list for the API Master field-picker panel: every real scalar column on
    /// TransactionData, auto-discovered via reflection in declaration order (so a new client
    /// column shows up here with zero code changes), followed by that client's actual custom
    /// fields at the END, labeled with whatever name the client gave them in Custom Field setup
    /// ("Challan", "Custom Charges"...) instead of "Custom Field 3".
    /// customFieldDisplayNames is keyed by field number (1-20) - pass the same
    /// CustomField.DisplayName lookup ReceiptPreview/SetupCustomFields already build.
    /// A custom field with no configured display name is skipped entirely, so the panel
    /// doesn't get cluttered with 17 unused "Custom Field N" entries.
    /// </summary>
    public static List<(string Key, string DisplayName, bool IsCustomField)> GetTransactionFieldMetadata(
        IDictionary<int, string> customFieldDisplayNames = null)
    {
        var result = new List<(string, string, bool)>();

        foreach (var prop in typeof(TransactionData).GetProperties())
        {
            if (ExcludedProperties.Contains(prop.Name)) continue;
            if (Regex.IsMatch(prop.Name, @"^CustomValue\d+$")) continue; // handled separately, in order, below
            if (!IsTemplatableType(prop.PropertyType)) continue;

            string key = ToSnakeCase(prop.Name);
            result.Add((key, ToDisplayName(key), false));
        }

        result.Add(("name", "Ticket Display Name", false)); // synthetic - not a real column, see below

        for (int i = 1; i <= 20; i++)
        {
            string displayName = customFieldDisplayNames != null
                && customFieldDisplayNames.TryGetValue(i, out var name)
                && !string.IsNullOrWhiteSpace(name)
                    ? name : null;

            if (displayName == null) continue; // client hasn't configured this one - don't show it
            result.Add(($"custom_value_{i}", displayName, true));
        }

        return result;
    }

    public static List<(string Key, string DisplayName)> GetDetailFieldMetadata()
    {
        var result = new List<(string, string)>();
        foreach (var prop in typeof(TransactionDetail).GetProperties())
        {
            if (ExcludedProperties.Contains(prop.Name)) continue;
            if (!IsTemplatableType(prop.PropertyType)) continue;

            string key = ToSnakeCase(prop.Name);
            result.Add((key, ToDisplayName(key)));
        }
        return result;
    }

    private static bool IsTemplatableType(Type t)
    {
        t = Nullable.GetUnderlyingType(t) ?? t;
        return t.IsPrimitive || t.IsEnum || t == typeof(string) || t == typeof(decimal)
            || t == typeof(DateTime) || t == typeof(Guid);
    }

    // "BagsWeight" -> "bags_weight"
    private static string ToSnakeCase(string pascalCase) =>
        Regex.Replace(pascalCase, @"(?<!^)([A-Z])", "_$1").ToLowerInvariant();

    // "bags_weight" -> "Bags Weight"
    private static string ToDisplayName(string snakeCaseKey)
    {
        var parts = snakeCaseKey.Split('_');
        return string.Join(" ", parts.Select(p => p.Length > 0 ? char.ToUpper(p[0]) + p.Substring(1) : p));
    }

    private static IDictionary<string, object> BuildTransactionFields(TransactionData model)
    {
        var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        if (model == null) return dict;

        foreach (var prop in typeof(TransactionData).GetProperties())
        {
            if (ExcludedProperties.Contains(prop.Name)) continue;
            if (Regex.IsMatch(prop.Name, @"^CustomValue\d+$")) continue; // added below, alongside their real labels
            if (!IsTemplatableType(prop.PropertyType)) continue;

            string key = ToSnakeCase(prop.Name);
            var rawValue = prop.GetValue(model);

            // Dates keep the exact same pre-formatted-string behavior as before (existing
            // client templates already expect "yyyy-MM-dd HH:mm:ss" strings here, not a raw
            // DateTime) - this applies automatically to ANY date column, present or future,
            // not just the ones that used to be hardcoded.
            var underlyingType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            dict[key] = underlyingType == typeof(DateTime) && rawValue != null
                ? ((DateTime)rawValue).ToString(DateFormat)
                : rawValue;
        }

        // Synthetic fields that aren't real columns - kept explicit rather than reflected.
        dict["name"] = $"Ticket {model.TicketID:000}";
        dict["print_time"] = DateTime.Now.ToString(DateFormat);
        dict["source"] = "WeighingSolution";

        for (int i = 1; i <= 20; i++)
        {
            var prop = typeof(TransactionData).GetProperty($"CustomValue{i}");
            if (prop != null)
                dict[$"custom_value_{i}"] = prop.GetValue(model);
        }

        return dict;
    }

    private static IDictionary<string, object> BuildLineFields(TransactionDetail detail)
    {
        var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        if (detail == null) return dict;

        foreach (var prop in typeof(TransactionDetail).GetProperties())
        {
            if (ExcludedProperties.Contains(prop.Name)) continue;
            if (!IsTemplatableType(prop.PropertyType)) continue;

            string key = ToSnakeCase(prop.Name);
            var rawValue = prop.GetValue(detail);
            var underlyingType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            dict[key] = underlyingType == typeof(DateTime) && rawValue != null
                ? ((DateTime)rawValue).ToString(DateFormat)
                : rawValue;
        }

        return dict;
    }
}



public class TemplateFieldItem
{
    public string DisplayName { get; set; }
    public string Placeholder { get; set; }   // e.g. "{{ transaction.vehicle_number }}"
}

