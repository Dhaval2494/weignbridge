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
using Cell = DocumentFormat.OpenXml.Spreadsheet.Cell;
using Row = DocumentFormat.OpenXml.Spreadsheet.Row;

namespace WeighingSolution.ViewModel
{
    class vmProductManagementList : DataSetupListViewModelBase<ProductBindingListModel>
    {
        public DelegateCommand<ProductBindingListModel> ActiveDeActiveProductCommand { get; private set; }
        private string strRestoreFileName = null;
        public vmProductManagementList()
        {
            ActiveDeActiveProductCommand = new DelegateCommand<ProductBindingListModel>(OnActiveDeActiveProductCommand);
            RestoreFileLocationCommand = new DelegateCommand(OnRestoreFileLocationCommand);
            RestoreDBCommand = new DelegateCommand(RestoreDBSaveExecute, RestoreDBSaveCaneExecute);
            lblRestoreFileLocation = "Select File to save excel data from.";
        }
        public async void OnActiveDeActiveProductCommand(ProductBindingListModel objProduct)
        {
            DisallowtodeactivemasterEventArgs ObjAllowtoDeactive = new DisallowtodeactivemasterEventArgs();
            ObjAllowtoDeactive.ObjectID = objProduct.ProductID;
            ObjAllowtoDeactive.Objectinstance = objProduct;
            if (objProduct.IsActive)
            {
                EventAggregator.GetEvent<DisallowtodeactivemasterEvent>().Publish(ObjAllowtoDeactive);
                if (dataRepository.Context.TransactionDatas.Any(x => x.TransactionDetails.Any(y => y.ProductID == objProduct.ProductID) && x.Status == TransactionStatus.Active.ToString()))
                    ObjAllowtoDeactive.Message = "it use in active transaction";
            }
            if (!string.IsNullOrEmpty(ObjAllowtoDeactive.Message))
                await MessageBoxHelper.ShowInfoDialogAsync("Warning", "You can not deactive, " + ObjAllowtoDeactive.Message);
            else
            {
                MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Are you sure?", "You want to " + (objProduct.IsActive ? "deactivate" : "activate") + " this product?");
                if (msgbox_result == MessageDialogResult.Affirmative)
                {
                    Product product = dataRepository.Context.Products.Find(objProduct.ProductID);
                    product.IsActive = !objProduct.IsActive;
                    objProduct.IsActive = !objProduct.IsActive;
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
            AllObjectList = new ObservableCollection<ProductBindingListModel>(await dataRepository.Context.Products.Select(x => new ProductBindingListModel { ProductID = x.ProductID, ProductName = x.ProductName, ProductCode = x.ProductCode, Notes = x.Notes, IsActive = x.IsActive }).ToListAsync());
            CurrentPageIndex = 0;
            OnFilterData();
        }

        public override void OnFilterData()
        {
            if (!String.IsNullOrEmpty(SearchString))
            {
                FilteredObjectList = new ObservableCollection<ProductBindingListModel>(AllObjectList.Where(x => (!string.IsNullOrEmpty(x.ProductName) && x.ProductName.ToLower().Contains(SearchString.ToLower())) || (!string.IsNullOrEmpty(x.ProductCode) && x.ProductCode.ToLower().Contains(SearchString.ToLower()))).ToList());
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
                string sheetName = "Product";

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
                File.AppendAllText(Constant.ApplicationDataLocation + @"\ReadProductExcel.txt", json);
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
                        //int columnIndex = 0;

                        //foreach (Cell cell in row.Elements<Cell>())
                        //{
                        //    string cellValue = GetCellValue(cell, workbookPart);
                        //    if (isHeader)
                        //    {
                        //        // Add columns to the DataTable based on the first row (Header Row)
                        //        dataTable.Columns.Add(cellValue);
                        //    }
                        //    else
                        //    {
                        //        dataRow[columnIndex++] = cellValue;
                        //    }
                        //}

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
                File.AppendAllText(Constant.ApplicationDataLocation + @"\ReadProductExcel.txt", json);
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
                        string ProductCode = row.Field<string>("ProductCode");
                        string ProductName = row.Field<string>("ProductName") ?? string.Empty;
                        ProductName = ProductName.TrimStart();
                        string checkQuery = "SELECT COUNT(1) FROM Product WHERE ProductCode = @ProductCode";
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                        {
                            checkCmd.Parameters.AddWithValue("@ProductCode", ProductCode);
                            bool productExists = (int)checkCmd.ExecuteScalar() > 0;

                            if (productExists)
                            {
                                string updateQuery = $@"UPDATE Product 
                                        SET ProductName = '{ProductName}'
                                        WHERE ProductCode = '{ProductCode}'";
                                bool value = DatabaseFunctions.ExecuteQuery(updateQuery);
                            }
                            else
                            {
                                string insertQuery = $@"
                                            INSERT INTO Product (ProductCode,ProductName,  IsActive)
                                            VALUES ('{ProductCode}', '{ProductName}', 1)";
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
                File.AppendAllText(Constant.ApplicationDataLocation + @"\ReadProductExcel.txt", json);
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

    public class ProductBindingListModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string Notes { get; set; }
        public bool IsActive { get; set; }

    }
}
