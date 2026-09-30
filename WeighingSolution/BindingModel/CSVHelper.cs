
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows;
using System.Windows.Media;
using WeighingSolution.DataLayer;

namespace WeighingSolution
{
    public class CSVHelper<T>
    {


        public static DataTable ReadCSV(string filePath)
        {
            try
            {
                string Excel03ConString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR={1}'";
                string CSVConString = @"Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0}; Extended Properties='text;HDR={1};FMT=Delimited'";     // ;FMT=Delimited  
                string Excel10ConString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR={1}'";

                string extension = System.IO.Path.GetExtension(filePath);
                string header = "YES";
                string conStr, sheetName;
                string directoryFullPath = System.IO.Path.GetDirectoryName(filePath);
                bool IsCsv = false;
                conStr = string.Empty;
                switch (extension)
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
                using (OleDbConnection con = new OleDbConnection(conStr))
                {
                    using (OleDbCommand cmd = new OleDbCommand())
                    {
                        cmd.Connection = con;
                        con.Open();
                        DataTable dtExcelSchema = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        sheetName = "Sheet1$";// dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                        con.Close();
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

                            return dt;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(Ex);
                File.AppendAllText(Constant.ApplicationDataLocation + @"\ReadCSVError.txt", json);
            }

            return null;

        }
    }

    public class DataMapping
    {
        public string Display_Field { get; set; }
        public string Target_Field { get; set; }
        public string Source_Field { get; set; }
        public string Discription { get; set; }
        public string InputType { get; set; }
        public bool ISCombo { get; set; }
        public bool IsMandatory { get; set; }
        public string DefaultValue { get; set; }
        public List<string> Source_FieldList { get; set; }
        public List<string> DefaultValueList { get; set; }
        public string ErrorMessage { get; set; }
        public SolidColorBrush ErrorColor { get; set; } = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));

    }

    public static class ExtensionMethods
    {
        private static Action EmptyDelegate = delegate () { };

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, EmptyDelegate);
        }
    }

    //[ChoCSVFileHeader]
    public class CSV_Processs_Order_Confirmation
    {
        [DisplayName("Sr No")]
        public int Sr_No { get; set; }

        [DisplayName("Date")]
        public Nullable<System.DateTime> Date { get; set; }

        [DisplayName("Slip No")]
        public string Slip_No { get; set; }

        [DisplayName("Vehicle Number")]
        public string Vehicle_Number { get; set; }

        [DisplayName("Party Name")]
        public string Party_Name { get; set; }

        [DisplayName("Vender Code")]
        public Nullable<int> Vender_Code { get; set; }

        [DisplayName("Product")]
        public string Product { get; set; }

        [DisplayName("Material Code")]
        public Nullable<int> Material_Code { get; set; }

        [DisplayName("From Plant Code")]
        public string From_Plant_Code { get; set; }

        [DisplayName("To Plant code")]
        public string To_Plant_code { get; set; }

        [DisplayName("Gross Wt")]
        public Nullable<decimal> Gross_Wt { get; set; }

        [DisplayName("Gross Date")]
        public Nullable<System.DateTime> Gross_Date { get; set; }

        [DisplayName("Gross Time")]
        public Nullable<System.TimeSpan> Gross_Time { get; set; }

        [DisplayName("Tare Wt")]
        public Nullable<decimal> Tare_Wt { get; set; }

        [DisplayName("Tare Date")]
        public Nullable<System.DateTime> Tare_Date { get; set; }

        [DisplayName("Tare Time")]
        public Nullable<System.TimeSpan> Tare_Time { get; set; }

        [DisplayName("Net Wt")]
        public Nullable<decimal> Net_Wt { get; set; }

        [DisplayName("Operations")]
        public string Operations { get; set; }

        [DisplayName("Operation remark")]
        public string Operation_remark { get; set; }

        [DisplayName("Remark")]
        public string Remark { get; set; }
    }

    //[ChoCSVFileHeader]
    public class CSV_ProcesssTemplate
    {
        [DisplayName("Sr no")]
        public int Sr_no { get; set; }

        [DisplayName("Process order")]
        public int Process_po_no { get; set; }

        [DisplayName("Component")]
        public string Component { get; set; }

        [DisplayName("Componet quantity")]
        public string Componet_quantity { get; set; }

        [DisplayName("Componet quantity MT")]
        public string Componet_quantity_MT { get; set; }

        [DisplayName("Date")]
        public string Process_Date { get; set; }

        [DisplayName("Time")]
        public string Process_Time { get; set; }

        [DisplayName("Trips")]
        public int No_Trips { get; set; }

    }
}

