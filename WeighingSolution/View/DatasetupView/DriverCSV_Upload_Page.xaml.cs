using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for CSV_Upload_Page.xaml
    /// </summary>
    public partial class DriverCSV_Upload_Page : Page
    {
        public int IntId = 0;

        public DataRepository dataRepository;

        #region Csv Variable

        private string Excel03ConString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR={1}'";
        private string CSVConString = @"Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0}; Extended Properties='text;HDR={1};FMT=Delimited'";     // ;FMT=Delimited  
        private string Excel10ConString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR={1}'";

        DataTable CSVDataTable = new DataTable();

        List<string> csvColumns = new List<string>();
        public ObservableCollection<DataMapping> MappingList = new ObservableCollection<DataMapping>();
        List<DataMapping> dataMappingList
        {
            get
            {
                List<DataMapping> dataMappings = new List<DataMapping>();

                dataMappings.Add(new DataMapping { Display_Field = "First Name", Target_Field = "FirstName", Source_Field = "", Discription = "Enter First Name.", InputType = "Text", IsMandatory = true });
                dataMappings.Add(new DataMapping { Display_Field = "Last Name", Target_Field = "LastName", Source_Field = "", Discription = "Enter Last Name.", InputType = "Text", IsMandatory = false });
                dataMappings.Add(new DataMapping { Display_Field = "ID Proof Type", Target_Field = "IDProofType", Source_Field = "", Discription = "Enter ID Proof Type.", InputType = "Text", IsMandatory = false });
                dataMappings.Add(new DataMapping { Display_Field = "ID Proof No", Target_Field = "IDProofNo", Source_Field = "", Discription = "Enter ID Proof Number.", InputType = "Text", IsMandatory = false });
                dataMappings.Add(new DataMapping { Display_Field = "ID Proof Scan", Target_Field = "IDProofScan", Source_Field = "", Discription = "Enter File Path or Name of ID Proof Scan.", InputType = "Text", IsMandatory = false });
                dataMappings.Add(new DataMapping { Display_Field = "Active", Target_Field = "Active", Source_Field = "", Discription = "Is Active (true/false).", InputType = "Checkbox", IsMandatory = false, DefaultValue = "true" });

                return dataMappings;
            }
        }


        string NotsetColumnValue = "Not Set";

        #endregion Csv Variable
        public DriverCSV_Upload_Page()
        {
            InitializeComponent();
            dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));
        }
        private void ServiceEntryManagementCancel_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }
        private void GoBack()
        {
            Uri uri = new Uri("/View/DatasetupView/DataSetup_Page.Xaml", UriKind.Relative);
            this.NavigationService.Navigate(uri);
        }
        private async void btnChooseCSVFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string filePath = openFileDialog.FileName;
                    string extension = System.IO.Path.GetExtension(filePath);
                    string header = "YES";
                    string conStr, sheetName;
                    string directoryFullPath = System.IO.Path.GetDirectoryName(filePath);
                    bool IsCsv = false;
                    conStr = string.Empty;
                    switch (extension.ToLower())
                    {

                        case ".xls": //Excel 97-03
                            conStr = string.Format(Excel03ConString, filePath, header);
                            break;

                        case ".xlsx": //Excel 07
                            conStr = string.Format(Excel10ConString, filePath, header);
                            break;

                        case ".csv": //Excel 07
                            conStr = string.Format(CSVConString, directoryFullPath, header);
                            IsCsv = true;
                            break;
                    }

                    //Get the name of the First Sheet.
                    //Get the name of the First Sheet.
                    // Get the name of the First Sheet and verify if "Sheet1$" exists
                    using (OleDbConnection con = new OleDbConnection(conStr))
                    {
                        using (OleDbCommand cmd = new OleDbCommand())
                        {
                            try
                            {
                                cmd.Connection = con;
                                con.Open();

                                DataTable dtExcelSchema = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                                try
                                {
                                    // Check if "Sheet1$" exists
                                    bool sheet1Exists = dtExcelSchema?.Rows
                                        .Cast<DataRow>()
                                        .Any(row => row["TABLE_NAME"].ToString().Equals("Sheet1$", StringComparison.OrdinalIgnoreCase)) ?? false;

                                    if (!sheet1Exists)
                                    {
                                        await MessageBoxHelper.ShowInfoDialogAsync("Error", "The Excel file is not in the expected format. Please ensure it contains a worksheet named 'Sheet1' and includes all required columns.");
                                        string log = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + "Sheet1$ not found.";
                                        File.AppendAllText(Constant.ApplicationDataLocation + @"\ExcelCSVUploadError.txt", log);
                                        return;
                                    }
                                }
                                catch
                                {
                                    await MessageBoxHelper.ShowInfoDialogAsync("Error", "The Excel file is not in the expected format. Please ensure it contains a worksheet named 'Sheet1' and includes all required columns.");
                                    string log = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + "Sheet1$ not found.";
                                    File.AppendAllText(Constant.ApplicationDataLocation + @"\ExcelCSVUploadError.txt", log);
                                    return;
                                }

                                sheetName = "Sheet1$";
                            }
                            catch (Exception ex)
                            {
                                await MessageBoxHelper.ShowInfoDialogAsync("Error", "The Excel Engine is not installed, Please install it.");
                                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                                File.AppendAllText(Constant.ApplicationDataLocation + @"\ExcelCSVEngineError.txt", json);
                                return;
                            }
                            finally
                            {
                                con.Close();
                            }
                        }
                    }


                    //Read Data from the First Sheet.
                    using (OleDbConnection con = new OleDbConnection(conStr))
                    {
                        using (OleDbCommand cmd = new OleDbCommand())
                        {
                            using (OleDbDataAdapter oda = new OleDbDataAdapter())
                            {
                                DataTable dt = new DataTable();

                                if (IsCsv)
                                    cmd.CommandText = "SELECT * From [" + System.IO.Path.GetFileName(filePath) + "]; ";
                                else
                                    cmd.CommandText = "SELECT * From [" + sheetName + "]; ";

                                cmd.Connection = con;
                                con.Open();
                                oda.SelectCommand = cmd;
                                oda.Fill(dt);
                                con.Close();

                                CSVDataTable = dt;

                                CsvDataGridView.ItemsSource = dt.AsDataView();

                                lblRows.Content = "Total Rows = " + dt.Rows.Count.ToString();

                                csvColumns.Add(NotsetColumnValue);

                                foreach (var column in CsvDataGridView.Columns)
                                {
                                    string ColumnName = column.Header.ToString();

                                    csvColumns.Add(ColumnName);
                                }

                                foreach (var item in dataMappingList)
                                {
                                    DataMapping dataMapping = new DataMapping
                                    {
                                        Target_Field = item.Target_Field,
                                        Display_Field = item.Display_Field,
                                        Source_Field = !string.IsNullOrEmpty(item.Source_Field) ? item.Source_Field : NotsetColumnValue,
                                        Discription = item.Discription,
                                        InputType = item.InputType,
                                        Source_FieldList = csvColumns,
                                        ISCombo = item.InputType == "Combo",
                                        IsMandatory = item.IsMandatory,
                                        DefaultValue = !string.IsNullOrEmpty(item.DefaultValue) ? item.DefaultValue : NotsetColumnValue,
                                    };

                                    if (dataMapping.InputType == "Combo")
                                    {
                                        string Target_Field = dataMapping.Target_Field;

                                        switch (Target_Field)
                                        {

                                            default:
                                                break;
                                        }
                                    }

                                    MappingList.Add(dataMapping);
                                }

                                MappingListView.ItemsSource = MappingList;

                                btnCSVImport.IsEnabled = true;
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine("---------------------------------------");
                    Console.WriteLine("Message");
                    Console.WriteLine(exc.Message);
                    Console.WriteLine("---------------------------------------");
                    Console.WriteLine("InnerException");
                    Console.WriteLine(exc.InnerException);
                    Console.WriteLine("---------------------------------------");
                    Console.WriteLine("StackTrace");
                    Console.WriteLine(exc.StackTrace);
                    Console.WriteLine("---------------------------------------");
                    string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(exc);
                    File.AppendAllText(Constant.ApplicationDataLocation + @"\ExcelCSVUploadError.txt", json);
                }
            }
        }
        private async void btnCSVImport_Click(object sender, RoutedEventArgs e)
        {
            bool errors = false;
            List<DataMapping> dataMappings = new List<DataMapping>();

            for (int i = 0; i < MappingList.Count; i++)
            {
                string Target_Field = MappingList[i].Target_Field;
                string Source_Field = MappingList[i].Source_Field;
                string DefaultValue = MappingList[i].DefaultValue;
                string InputType = MappingList[i].InputType;

                bool rowerror = false;

                switch (Target_Field)
                {
                    case "FirstName" when Source_Field == NotsetColumnValue:
                        rowerror = true;
                        break;

                    default:
                        break;
                }

                if (rowerror)
                {
                    MappingList[i].ErrorColor = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                    errors = true;
                }
                else
                {
                    dataMappings.Add(new DataMapping
                    {
                        Target_Field = Target_Field,
                        Source_Field = Source_Field,
                        DefaultValue = DefaultValue,
                        InputType = InputType
                    });
                }
            }

            if (errors)
            {
                MappingListView.ItemsSource = null;
                MappingListView.ItemsSource = MappingList;
                return;
            }

            StringBuilder errorMessage = new StringBuilder();
            List<DriverCSV_Service_EntryPO> vehicleEntries = new List<DriverCSV_Service_EntryPO>();

            int checkRow = 1;
            for (int i = 0; i < CSVDataTable.Rows.Count; i++)
            {
                try
                {
                    DriverCSV_Service_EntryPO entry = PreparedDriverField(dataMappings, i);
                    vehicleEntries.Add(entry);

                    if (string.IsNullOrEmpty(entry.FirstName))
                        errorMessage.Append($"FirstName Name is empty at row {checkRow}\n");

                    checkRow++;
                }
                catch (Exception exp)
                {
                    await MessageBoxHelper.ShowInfoDialogAsync("Error", exp.Message);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(errorMessage.ToString()))
            {
                await MessageBoxHelper.ShowInfoDialogAsync("Validation Errors", errorMessage.ToString());
                return;
            }

            int importCount = 1;

            foreach (DriverCSV_Service_EntryPO entry in vehicleEntries)
            {
                try
                {
                    ProcessDriverData(entry);
                    lblwaiting.Content = $"{importCount}  Driver(s) imported";
                    lblwaiting.Refresh();
                    await Task.Delay(100);
                    importCount++;
                }
                catch (Exception exp)
                {
                    await MessageBoxHelper.ShowInfoDialogAsync("Error", exp.Message);
                    string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(exp);
                    File.AppendAllText(Constant.ApplicationDataLocation + @"\ExcelCSVUploadError.txt", json);
                    return;
                }
            }


            try
            {
                await dataRepository.SaveAsync();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        await MessageBoxHelper.ShowInfoDialogAsync("Error", validationError.ErrorMessage);

                    }
                }
                return;
            }



            btnCSVImport.IsEnabled = false;
            lblmsg.Content = "Successfully Imported Driver Data";
            await MessageBoxHelper.ShowInfoDialogAsync("Success", $"Successfully imported {importCount - 1} Drivers.");
            GoBack();
        }
        private void ProcessDriverData(DriverCSV_Service_EntryPO entry)
        {
            if (!string.IsNullOrEmpty(entry.FirstName))
            {
                var driver = dataRepository.Context.Drivers
                    .FirstOrDefault(d => d.FirstName == entry.FirstName && d.LastName == entry.LastName);

                if (driver == null)
                {
                    driver = new Driver
                    {
                        FirstName = entry.FirstName,
                        LastName = entry.LastName,
                        IDProofType = entry.IDProofType,
                        IDProofNo = entry.IDProofNo,
                        IDProofScan = entry.IDProofScan,
                        Active = true
                    };
                    dataRepository.Context.Drivers.Add(driver);
                }
                else
                {
                    driver.LastName = entry.LastName;
                    driver.IDProofType = entry.IDProofType;
                    driver.IDProofNo = entry.IDProofNo;
                    driver.IDProofScan = entry.IDProofScan;
                }
            }
        }



        public int InsertDriver(string firstName)
        {
            try
            {
                firstName = firstName.ToUpper().Trim();

                using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                {
                    var driver = ctx.Drivers
                        .FirstOrDefault(d => d.FirstName.Trim().ToLower() == firstName.ToLower());

                    if (driver != null)
                    {
                        return driver.DriverID;
                    }

                    Driver newDriver = new Driver
                    {
                        FirstName = firstName,
                        Active = true
                    };

                    ctx.Drivers.Add(newDriver);
                    ctx.SaveChanges();

                    return newDriver.DriverID;
                }
            }
            catch (Exception ex)
            {
                _ = ex.Message;
                return 0;
            }
        }



        public DriverCSV_Service_EntryPO PreparedDriverField(List<DataMapping> dataMappings, int rowIndex)
        {
            DriverCSV_Service_EntryPO driverField = new DriverCSV_Service_EntryPO();

            string GetValue(string targetField) =>
                         dataMappings.FirstOrDefault(p => p.Target_Field == targetField)?.Source_Field;

            string GetSafeValue(string fieldName) =>
                        !string.IsNullOrEmpty(fieldName) && fieldName != NotsetColumnValue
                             ? CSVDataTable.Rows[rowIndex][fieldName].ToString()
                             : null;

            driverField.FirstName = GetSafeValue(GetValue("FirstName"));
            driverField.LastName = GetSafeValue(GetValue("LastName"));
            driverField.IDProofType = GetSafeValue(GetValue("IDProofType"));
            driverField.IDProofNo = GetSafeValue(GetValue("IDProofNo"));
            driverField.IDProofScan = GetSafeValue(GetValue("IDProofScan"));

            string activeField = GetValue("Active");
            if (!string.IsNullOrEmpty(activeField) && activeField != NotsetColumnValue &&
               bool.TryParse(CSVDataTable.Rows[rowIndex][activeField].ToString(), out bool isActive))
            {
                driverField.Active = isActive;
            }

            return driverField;
        }




        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            string CSVFieldValue = comboBox.Text;
            List<string> List = new List<string>
            {
                NotsetColumnValue
            };

            foreach (string item in csvColumns)
            {
                if (item == CSVFieldValue && item != NotsetColumnValue)
                { List.Add(item); }
                else
                {
                    bool exist = false;
                    for (int i = 0; i < MappingList.Count; i++)
                    {
                        string val = MappingList[i].Source_Field.ToString();
                        if (val == item)
                        {
                            exist = true;
                            break;
                        }
                    }

                    if (!exist)
                        List.Add(item);
                }
            }
            comboBox.ItemsSource = null;
            comboBox.ItemsSource = List;
        }

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            DataMapping anonType = ((ComboBox)sender).DataContext as DataMapping;

            if (anonType == null)
                return;

            ComboBox comboBox = sender as ComboBox;

            string CSVFieldValue = comboBox.Text;
            if (CSVFieldValue != NotsetColumnValue)
            {
                MappingList.SingleOrDefault(p => p.Target_Field == anonType.Target_Field).ErrorColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));

                MappingListView.ItemsSource = null;
                MappingListView.ItemsSource = MappingList;
            }
        }
        private void ComboBox_DropDownClosed_1(object sender, EventArgs e)
        {
            DataMapping anonType = ((ComboBox)sender).DataContext as DataMapping;

            if (anonType == null)
                return;

            ComboBox comboBox = sender as ComboBox;

            string CSVFieldValue = comboBox.Text;
            if (CSVFieldValue != NotsetColumnValue)
            {
                MappingList.SingleOrDefault(p => p.Target_Field == anonType.Target_Field).ErrorColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));

                MappingListView.ItemsSource = null;
                MappingListView.ItemsSource = MappingList;
            }
        }
        public T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }
                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }
    }

    public partial class DriverCSV_Service_EntryPO
    {
        public int DriverID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IDProofScan { get; set; }
        public string IDProofType { get; set; }
        public string IDProofNo { get; set; }
        public bool Active { get; set; }
    }


}
