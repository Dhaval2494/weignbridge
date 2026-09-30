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
using WeighingSolution.SharedComponents;

namespace WeighingSolution.ViewModel
{
    class vmAccountManagementList : DataSetupListViewModelBase<AccountBindingListModel>
    {
        public DelegateCommand<AccountBindingListModel> ActiveDeActiveAccountCommand { get; private set; }
        private string strRestoreFileName = null;

        public vmAccountManagementList()
        {
            ActiveDeActiveAccountCommand = new DelegateCommand<AccountBindingListModel>(OnActiveDeActiveAccountCommand);
            RestoreFileLocationCommand = new DelegateCommand(OnRestoreFileLocationCommand);
            RestoreDBCommand = new DelegateCommand(RestoreDBSaveExecute, RestoreDBSaveCaneExecute);
            lblRestoreFileLocation = "Select File to save excel data from.";
        }

        public async void OnActiveDeActiveAccountCommand(AccountBindingListModel objAccount)
        {
            DisallowtodeactivemasterEventArgs ObjAllowtoDeactive = new DisallowtodeactivemasterEventArgs();
            ObjAllowtoDeactive.ObjectID = objAccount.AccountID;
            ObjAllowtoDeactive.Objectinstance = objAccount;
            if (objAccount.Active)
            {
                EventAggregator.GetEvent<DisallowtodeactivemasterEvent>().Publish(ObjAllowtoDeactive);
                if (dataRepository.Context.TransactionDatas.Any(x => x.AccountID == objAccount.AccountID && x.Status == TransactionStatus.Active.ToString()))
                    ObjAllowtoDeactive.Message = "it use in active transaction";
            }
            if (!string.IsNullOrEmpty(ObjAllowtoDeactive.Message))
                await MessageBoxHelper.ShowInfoDialogAsync("Warning", "You can not deactive, " + ObjAllowtoDeactive.Message);
            else
            {
                MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("Are you sure?", "You want to " + (objAccount.Active ? "deactivate" : "activate") + " this account?");
                if (msgbox_result == MessageDialogResult.Affirmative)
                {
                    Account _account = dataRepository.Context.Accounts.Find(objAccount.AccountID);
                    _account.Active = !objAccount.Active;
                    objAccount.Active = !objAccount.Active;
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
            AllObjectList = new ObservableCollection<AccountBindingListModel>(await dataRepository.Context.Accounts.Select(x => new AccountBindingListModel
            {
                AccountID = x.AccountID,
                AccountCode = x.AccountCode,
                CompanyName = x.CompanyName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                ContactNo = x.ContactNo,
                Email = x.Email,
                IsAccount = x.IsAccount,
                IsTransporter = x.IsTransporter,
                Notes = x.Notes,
                Address = x.Address,
                City = x.City,
                Pincode = x.Pincode,
                Active = x.Active,
            }).ToListAsync());
            CurrentPageIndex = 0;
            OnFilterData();
        }
        public override void OnFilterData()
        {
            if (!String.IsNullOrEmpty(SearchString))
            {
                FilteredObjectList = new ObservableCollection<AccountBindingListModel>(AllObjectList.Where(x => (!string.IsNullOrEmpty(x.FirstName) && x.FirstName.ToLower().Contains(SearchString.ToLower())) || (!string.IsNullOrEmpty(x.LastName) && x.LastName.ToLower().Contains(SearchString.ToLower())) || (!string.IsNullOrEmpty(x.Email) && x.Email.ToLower().Contains(SearchString.ToLower())) || (!string.IsNullOrEmpty(x.CompanyName) && x.CompanyName.ToLower().Contains(SearchString.ToLower()))).ToList());
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
                string filePath = strRestoreFileName;
                string sheetName = "Account";
                DataTable dt = ReadExcelData(filePath, sheetName);
                SaveDataToDatabase(dt);

                MessageBox.Show("Data import completed successfully.");
            }
            catch (Exception ex)
            {
                _ = MessageBoxHelper.ShowInfoDialogAsync("Error!", "All Records are not,inserted successfully.");
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\ReadAccountExcel.txt", json);
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
                File.AppendAllText(Constant.ApplicationDataLocation + @"\ReadAccountExcel.txt", json);
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
                        //string AccountCode = row.Field<string>("AccountCode") ?? string.Empty;
                        string CompanyName = row.Field<string>("CompanyName");
                        string FirstName = row.Field<string>("FirstName") ?? string.Empty;
                        string LastName = row.Field<string>("LastName") ?? string.Empty;
                        string Email = row.Field<string>("Email") ?? string.Empty;
                        string ContactNo = row.Field<string>("ContactNo") ?? string.Empty;
                        string Address = row.Field<string>("Address") ?? string.Empty;
                        string City = row.Field<string>("City") ?? string.Empty;
                        string Pincode = row.Field<string>("Pincode") ?? string.Empty;

                        string isAccountStr = row.Field<string>("IsAccount") ?? "false";
                        isAccountStr = isAccountStr.Trim().ToLower();
                        bool IsAccount = isAccountStr == "1" || isAccountStr == "true";

                        string IsTransporterStr = row.Field<string>("IsTransporter") ?? "false";
                        IsTransporterStr = IsTransporterStr.Trim().ToLower();
                        bool IsTransporter = IsTransporterStr == "1" || IsTransporterStr == "true";

                        CompanyName = CompanyName.TrimStart();
                        string checkQuery = "SELECT COUNT(1) FROM Account WHERE CompanyName = @CompanyName";
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                        {
                            checkCmd.Parameters.AddWithValue("@CompanyName", CompanyName);
                            bool productExists = (int)checkCmd.ExecuteScalar() > 0;

                            if (productExists)
                            {
                                string updateQuery = $@"UPDATE Account 
                                        SET FirstName = '{FirstName}', LastName = '{LastName}', Email = '{Email}', ContactNo = '{ContactNo}', Address = '{Address}', City = '{City}', Pincode = '{Pincode}', IsAccount = '{IsAccount}', IsTransporter = '{IsTransporter}'
                                        WHERE CompanyName = '{CompanyName}'";
                                bool value = DatabaseFunctions.ExecuteQuery(updateQuery);
                            }
                            else
                            {
                                string insertQuery = $@"
                                            INSERT INTO Account (CompanyName, Active, FirstName, LastName, Email,  ContactNo, Address, City, Pincode, IsAccount, IsTransporter )
                                            VALUES ('{CompanyName}',  1, '{FirstName}', '{LastName}', '{Email}', '{ContactNo}', '{Address}', '{City}', '{Pincode}', '{IsAccount}', '{IsTransporter}' )";
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
                File.AppendAllText(Constant.ApplicationDataLocation + @"\ReadAccountExcel.txt", json);
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
}
