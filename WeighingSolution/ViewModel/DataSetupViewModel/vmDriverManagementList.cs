using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WeighingSolution.DataLayer;
using WeighingSolution.Events;
using WeighingSolution.Helpers;


namespace WeighingSolution.ViewModel
{
    class vmDriverManagementList : DataSetupListViewModelBase<DriverBindingListModel>
    {
        public DelegateCommand<DriverBindingListModel> ActiveDeActiveDriverCommand { get; private set; }
        private string strRestoreFileName = null;

        public vmDriverManagementList()
        {
            ActiveDeActiveDriverCommand = new DelegateCommand<DriverBindingListModel>(OnActiveDeActiveDriverCommand);
            RestoreFileLocationCommand = new DelegateCommand(OnRestoreFileLocationCommand);
            RestoreDBCommand = new DelegateCommand(RestoreDBSaveExecute, RestoreDBSaveCaneExecute);
            lblRestoreFileLocation = "Select File to save excel data from.";
        }

        public async void OnActiveDeActiveDriverCommand(DriverBindingListModel objdriver)
        {
            DisallowtodeactivemasterEventArgs ObjAllowtoDeactive = new DisallowtodeactivemasterEventArgs();
            ObjAllowtoDeactive.ObjectID = objdriver.DriverID;
            ObjAllowtoDeactive.Objectinstance = objdriver;
            if (objdriver.Active)
            {
                EventAggregator.GetEvent<DisallowtodeactivemasterEvent>().Publish(ObjAllowtoDeactive);
                if (dataRepository.Context.TransactionDatas.Any(x => x.DriverID == objdriver.DriverID && x.Status == TransactionStatus.Active.ToString()))
                    ObjAllowtoDeactive.Message = "it use in active transaction";
            }
            if (!string.IsNullOrEmpty(ObjAllowtoDeactive.Message))
                await MessageBoxHelper.ShowInfoDialogAsync("Warning", "You can not deactive, " + ObjAllowtoDeactive.Message);
            else
            {
                MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Are you sure?", "You want to " + (objdriver.Active ? "deactivate" : "activate") + " this driver?");
                if (msgbox_result == MessageDialogResult.Affirmative)
                {
                    Driver _driver = dataRepository.Context.Drivers.Find(objdriver.DriverID);
                    _driver.Active = !objdriver.Active;
                    objdriver.Active = !objdriver.Active;
                    await dataRepository.SaveAsync();
                    ViewList.View.Refresh();
                }
            }
        }


        public DelegateCommand RestoreFileLocationCommand { get; private set; }
        public DelegateCommand RestoreDBCommand { get; private set; }

        private string _lblRestoreFileLocation;
        public string lblRestoreFileLocation
        {
            get { return _lblRestoreFileLocation; }
            set
            {
                _lblRestoreFileLocation = value;
                OnPropertyChanged();
                ((DelegateCommand)RestoreDBCommand).RaiseCanExecuteChanged();
            }
        }
        public async Task LoadAsync()
        {
            AllObjectList = new ObservableCollection<DriverBindingListModel>(await dataRepository.Context.Drivers.Select(x => new DriverBindingListModel
            {
                DriverID = x.DriverID,
                FirstName = x.FirstName,
                LastName = x.LastName,
                IDProofScan = x.IDProofScan,
                IDProofType = x.IDProofType,
                IDProofNo = x.IDProofNo,
                Active = x.Active,
            }).ToListAsync());
            CurrentPageIndex = 0;
            OnFilterData();
        }
        public override void OnFilterData()
        {
            if (!String.IsNullOrEmpty(SearchString))
            {
                FilteredObjectList = new ObservableCollection<DriverBindingListModel>(AllObjectList.Where(x => (!string.IsNullOrEmpty(x.FirstName) && x.FirstName.ToLower().Contains(SearchString.ToLower())) || (!string.IsNullOrEmpty(x.LastName) && x.LastName.ToLower().Contains(SearchString.ToLower())) || (!string.IsNullOrEmpty(x.IDProofType) && x.IDProofType.ToLower().Contains(SearchString.ToLower())) || (!string.IsNullOrEmpty(x.IDProofNo) && x.IDProofNo.ToLower().Contains(SearchString.ToLower()))).ToList());
            }
            else
            {
                FilteredObjectList = AllObjectList;
            }
            //CurrentPageIndex = 0;
            CalculateTotalPages(FilteredObjectList.Count);
            ViewList.Source = FilteredObjectList.Skip(CurrentPageIndex * itemPerPage).Take(itemPerPage);
            ViewList.View.Refresh();
        }

        private void OnRestoreFileLocationCommand()
        {
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog { Filter = "Excel Files|*.xlsx|CSV Files|*.csv", };
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && !string.IsNullOrEmpty(fileDialog.FileName))
            {
                lblRestoreFileLocation = "Click \"Save Excel Data\" to begin Save from: " + fileDialog.FileName;
                strRestoreFileName = fileDialog.FileName;
            }
        }
        private bool RestoreDBSaveCaneExecute()
        {
            if (!string.IsNullOrWhiteSpace(strRestoreFileName))
                return true;
            else
                return false;
        }




        private void RestoreDBSaveExecute()
        {
            try
            {
                // Get the file path (you can use a FileOpenDialog)
                string filePath = strRestoreFileName;
                string sheetName = "Driver";

                // Read the data from the Excel file
                DataTable dt = ReadExcelData(filePath, sheetName);

                // Save the data to the database
                SaveDataToDatabase(dt);

                MessageBox.Show("Data import completed successfully.");
            }
            catch (Exception ex)
            {
                _ = MessageBoxHelper.ShowInfoDialogAsync("Error!", "All Records are not,inserted successfully.");
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\ReadDriverExcel.txt", json);
            }
        }

        public DataTable ReadExcelData(string filePath, string sheetName)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Open(filePath, false))
                {
                    WorkbookPart workbookPart = document.WorkbookPart;
                    // Find the sheet by name
                    Sheet sheet = workbookPart.Workbook.Sheets.Elements<Sheet>()
                        .FirstOrDefault(s => s.Name == sheetName);

                    if (sheet == null)
                    {
                        throw new Exception($"Sheet with name '{sheetName}' not found in the Excel file.");
                    }

                    WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
                    SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                    bool isHeader = true;
                    foreach (Row row in sheetData.Elements<Row>())
                    {
                        System.Data.DataRow dataRow = dataTable.NewRow();
                        int expectedCellIndex = 0;

                        foreach (Cell cell in row.Elements<Cell>())
                        {
                            int actualCellIndex = GetColumnIndexFromName(GetColumnName(cell.CellReference));

                            while (expectedCellIndex < actualCellIndex)
                            {
                                if (isHeader)
                                    dataTable.Columns.Add("Column" + expectedCellIndex);
                                else
                                    dataRow[expectedCellIndex] = DBNull.Value;

                                expectedCellIndex++;
                            }

                            string cellValue = GetCellValue(cell, workbookPart);
                            if (isHeader)
                                dataTable.Columns.Add(cellValue);
                            else
                                dataRow[expectedCellIndex] = cellValue;

                            expectedCellIndex++;
                        }

                        if (!isHeader)
                        {
                            dataTable.Rows.Add(dataRow);
                        }
                        isHeader = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = MessageBoxHelper.ShowInfoDialogAsync("Error!", "All Records are not,inserted successfully.");
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\ReadDriverExcel.txt", json);
            }

            return dataTable;
        }

        private string GetCellValue(Cell cell, WorkbookPart workbookPart)
        {
            string value = string.Empty;

            if (cell.CellValue != null)
            {
                value = cell.CellValue.Text;
            }

            // Check for shared strings
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                int sharedStringIndex = int.Parse(value);
                SharedStringTablePart sharedStringPart = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                value = sharedStringPart.SharedStringTable.Elements<SharedStringItem>().ElementAt(sharedStringIndex).Text.Text;
            }

            return value;
        }
        public async void SaveDataToDatabase(DataTable dt)
        {
            string connectionString = "Data Source=" + Constant.DBSERVER + ";Initial Catalog=" + Constant.DBNAME + ";user id=" + Constant.DBUSERNAME + ";password=" + Constant.DBPASSWORD + ";MultipleActiveResultSets=True;Connection Timeout=30";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    foreach (System.Data.DataRow row in dt.Rows)
                    {
                        string FirstName = row.Field<string>("FirstName");
                        string LastName = row.Field<string>("LastName") ?? string.Empty;
                        string IDProofScan = row.Field<string>("IDProofScan") ?? string.Empty;
                        string IDProofType = row.Field<string>("IDProofType") ?? string.Empty;
                        string IDProofNo = row.Field<string>("IDProofNo") ?? string.Empty;
                        FirstName = FirstName.TrimStart();
                        string checkQuery = "SELECT COUNT(1) FROM Driver WHERE FirstName = @FirstName";
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                        {
                            checkCmd.Parameters.AddWithValue("@FirstName", FirstName);
                            bool productExists = (int)checkCmd.ExecuteScalar() > 0;

                            if (productExists)
                            {
                                string updateQuery = $@"UPDATE Driver 
                                        SET LastName = '{LastName}',IDProofScan = '{IDProofScan}',IDProofType = '{IDProofType}',IDProofNo = '{IDProofNo}'
                                        WHERE FirstName = '{FirstName}'";
                                bool value = DatabaseFunctions.ExecuteQuery(updateQuery);
                            }
                            else
                            {
                                string insertQuery = $@"
                                            INSERT INTO Driver (FirstName,LastName,  Active, IDProofScan, IDProofType, IDProofNo)
                                            VALUES ('{FirstName}', '{LastName}', 1, '{IDProofScan}', '{IDProofType}', '{IDProofNo}' )";
                                bool value = DatabaseFunctions.ExecuteQuery(insertQuery);
                            }
                        }
                    }

                    con.Close();
                    lblRestoreFileLocation = string.Empty;
                }
            }
            catch (Exception ex)
            {
                await MessageBoxHelper.ShowInfoDialogAsync("Error!", "All Records are not,inserted successfully.");
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\ReadDriverExcel.txt", json);
            }
        }

        private string GetColumnName(string cellReference)
        {
            // Example: "A1" => "A"
            return new string(cellReference.Where(c => Char.IsLetter(c)).ToArray());
        }

        private int GetColumnIndexFromName(string columnName)
        {
            int columnIndex = 0;
            foreach (char c in columnName.ToUpper())
            {
                columnIndex *= 26;
                columnIndex += (c - 'A' + 1);
            }
            return columnIndex - 1; // Zero-based
        }


    }

    public class DriverBindingListModel
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
