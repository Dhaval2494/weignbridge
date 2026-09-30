using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
using WeighingSolution.SharedComponents;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for CSV_Upload_Page.xaml
    /// </summary>
    public partial class VehicleCSV_Upload_Page : Page
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

                dataMappings.Add(new DataMapping { Display_Field = "Vehicle Number", Target_Field = "VehicleNumber", Source_Field = "", Discription = "Enter Vehicle NO.", InputType = "Text", IsMandatory = true });
                dataMappings.Add(new DataMapping { Display_Field = "Vehicle Type", Target_Field = "VehicleType", Source_Field = "", Discription = "Enter Vehicle Type.", InputType = "Text", IsMandatory = false });
                dataMappings.Add(new DataMapping { Display_Field = "Tare Weight", Target_Field = "TareWeight", Source_Field = "", Discription = "Enter Tare Weight.", InputType = "Text", IsMandatory = false });
                dataMappings.Add(new DataMapping { Display_Field = "Notes", Target_Field = "Notes", Source_Field = "", Discription = "Enter Notes", InputType = "Text", IsMandatory = false });
                dataMappings.Add(new DataMapping { Display_Field = "IsActive", Target_Field = "IsActive", Source_Field = "", Discription = "Enter IsActive.", InputType = "Text", IsMandatory = false, DefaultValue = "true" });
                dataMappings.Add(new DataMapping { Display_Field = "TareWeightTime", Target_Field = "TareWeight Time", Source_Field = "", Discription = "Enter TareWeight Time.", InputType = "Text", IsMandatory = false });

                return dataMappings;
            }
        }

        string NotsetColumnValue = "Not Set";

        #endregion Csv Variable
        public VehicleCSV_Upload_Page()
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
                    case "VehicleNumber" when Source_Field == NotsetColumnValue:
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
            List<CSV_Service_EntryPO> vehicleEntries = new List<CSV_Service_EntryPO>();

            int checkRow = 1;
            for (int i = 0; i < CSVDataTable.Rows.Count; i++)
            {
                try
                {
                    CSV_Service_EntryPO entry = PreparedTransactionFeild(dataMappings, i);
                    vehicleEntries.Add(entry);

                    if (string.IsNullOrEmpty(entry.VehicleNumber))
                        errorMessage.Append($"Vehicle Number is empty at row {checkRow}\n");

                    checkRow++;
                }
                catch (Exception exp)
                {
                    await MessageBoxHelper.ShowInfoDialogAsync("Error", exp.Message);
                    string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(exp);
                    File.AppendAllText(Constant.ApplicationDataLocation + @"\ExcelCSVUploadError.txt", json);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(errorMessage.ToString()))
            {
                await MessageBoxHelper.ShowInfoDialogAsync("Validation Errors", errorMessage.ToString());
                return;
            }

            int importCount = 1;

            foreach (CSV_Service_EntryPO entry in vehicleEntries)
            {
                try
                {
                    ProcessVehicleData(entry);
                    lblwaiting.Content = $"{importCount}  vehicle(s) imported";
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

            await dataRepository.SaveAsync();
            btnCSVImport.IsEnabled = false;
            lblmsg.Content = "Successfully Imported Vehicle Data";
            await MessageBoxHelper.ShowInfoDialogAsync("Success", $"Successfully imported {importCount - 1} vehicles.");
            GoBack();
        }
        private void ProcessVehicleData(CSV_Service_EntryPO entry)
        {
            if (!string.IsNullOrEmpty(entry.VehicleNumber))
            {
                var vehicle = dataRepository.Context.Vehicles
                    .FirstOrDefault(v => v.VehicleNumber == entry.VehicleNumber);

                if (vehicle == null)
                {
                    vehicle = new Vehicle
                    {
                        VehicleNumber = entry.VehicleNumber,
                        VehicleType = entry.VehicleType,
                        TareWeight = entry.TareWeight,
                        Notes = entry.Notes,
                        IsActive = true
                    };
                    dataRepository.Context.Vehicles.Add(vehicle);
                }
                else
                {
                    vehicle.VehicleType = entry.VehicleType;
                    vehicle.TareWeight = entry.TareWeight;
                    vehicle.Notes = entry.Notes;
                }
            }
        }
        public int InsertVehicle(string VehicleNumber)
        {
            try
            {
                VehicleNumber = VehicleNumber.ToUpper();
                VehicleNumber = VehicleNumber.Replace(" ", string.Empty);

                using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                {
                    var vehicleList = ctx.Vehicles.Where(p => p.VehicleNumber.Trim().ToLower() == VehicleNumber.Trim().ToLower()).Select(p => new VehicleBindingListModel { VehicleNumber = p.VehicleNumber, VehicleType = p.VehicleType, TareWeight = p.TareWeight }).ToList();

                    if (vehicleList.Count > 0)
                    {
                        return vehicleList.FirstOrDefault().VehicleID;
                    }
                    else
                    {
                        Vehicle vehicle = new Vehicle
                        {
                            VehicleNumber = VehicleNumber,
                            IsActive = true
                        };
                        ctx.Vehicles.Add(vehicle);
                        ctx.SaveChanges();

                        return vehicle.VehicleID;
                    }
                }
            }
            catch (Exception exc)
            {
                _ = exc.Message;
                return 0;
            }
        }
        public CSV_Service_EntryPO PreparedTransactionFeild(List<DataMapping> dataMappings, int rowIndex)
        {
            CSV_Service_EntryPO transactionFeild = new CSV_Service_EntryPO();

            // VehicleNumber
            string columnVehicleNumber = dataMappings.FirstOrDefault(p => p.Target_Field == "VehicleNumber")?.Source_Field;
            if (!string.IsNullOrEmpty(columnVehicleNumber) && columnVehicleNumber != NotsetColumnValue)
                transactionFeild.VehicleNumber = CSVDataTable.Rows[rowIndex][columnVehicleNumber].ToString();

            // VehicleType
            string columnVehicleType = dataMappings.FirstOrDefault(p => p.Target_Field == "VehicleType")?.Source_Field;
            if (!string.IsNullOrEmpty(columnVehicleType) && columnVehicleType != NotsetColumnValue)
                transactionFeild.VehicleType = CSVDataTable.Rows[rowIndex][columnVehicleType].ToString();

            // TareWeight
            string columnTareWeight = dataMappings.FirstOrDefault(p => p.Target_Field == "TareWeight")?.Source_Field;
            if (!string.IsNullOrEmpty(columnTareWeight) && columnTareWeight != NotsetColumnValue)
            {
                string value = CSVDataTable.Rows[rowIndex][columnTareWeight].ToString();
                if (decimal.TryParse(value, out decimal tare))
                    transactionFeild.TareWeight = tare;
            }

            // Notes (optional)
            string columnNotes = dataMappings.FirstOrDefault(p => p.Target_Field == "Notes")?.Source_Field;
            if (!string.IsNullOrEmpty(columnNotes) && columnNotes != NotsetColumnValue)
                transactionFeild.Notes = CSVDataTable.Rows[rowIndex][columnNotes].ToString();

            return transactionFeild;
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

    public partial class CSV_Service_EntryPO
    {
        public string VehicleNumber { get; set; }
        public string VehicleType { get; set; }
        public decimal? TareWeight { get; set; }
        public string Notes { get; set; }

    }


}
