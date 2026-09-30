using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;

namespace WeighingSolution.DataLayer
{
    public class Constant
    {
        public static bool IsUnmanConnect { get; set; }
        public static string ApiBaseUrl { get; set; }
        public static bool IsWeighStart { get; set; }
        public static bool IsWeighDashStart { get; set; }
        public static bool IsVehicleEnter { get; set; }
        public static bool IsWeighSaved { get; set; }
        public static bool Final_end { get; set; }
        public static bool Final_Reset { get; set; }
        public static int CycleCount { get; set; }
        public static bool IsUnmanCycle_end { get; set; }
        public static bool IsFirstSensorOn { get; set; }
        public static bool IsSecoundSensorOn { get; set; }
        public static bool IsTrirdSensorOn { get; set; }
        public static bool IsForthSensorOn { get; set; }
        public static decimal IndicatorWeight { get; set; }
        public static decimal lastStableWeight { get; set; }
        public static string WBSIDE { get; set; }
        public static string WBSIDE_DASHBORD { get; set; }
        public static bool ISManual { get; set; }




        public static string RFID { get; set; }
        public static string UHF1IP { get; set; }
        public static string UHF2IP { get; set; }
        public static string UHFLEFT { get; set; }
        public static string UHFRIGHT { get; set; }
        public static bool IsProcessUHF { get; set; }
        public static decimal Stableweight { get; set; }
        public static bool IsBasicversion { get; set; }

        public static bool IsTL { get; set; }
        public static bool IsTR { get; set; }
        public static bool IsBoomLeft { get; set; }
        public static bool IsBoomRight { get; set; }


        public static string DisplayRFID_Left { get; set; }
        public static string DisplayRFID_Right { get; set; }
        public static string Display_Message { get; set; }
        public static string Display_Vehicle { get; set; }
        public static string Color_Vehicle { get; set; }
       

        public static bool HasAnprLicense { get; set; }
        public static string AnprApiBaseUrl { get; set; }
        public static string AnprPasskey { get; set; }
        public static string AssemblyVersion { get; set; }
       
        public static DateTime AmcExpire { get; set; }
   
        public static int LoggedInUserID { get; set; }
        public static bool IsAdmin { get; set; }
        public static bool IsSuperAdmin { get; set; }
        public static bool IsSupportUser { get; set; }
        public static string EntityConnectionString { get; set; }
        //public static List<UserPermission> lstPermission { get; set; }
        public static List<TemplatePermission> lstPermission { get; set; }
        public static void setLoggedInUserID(int _LoggedInUserID)
        {
            LoggedInUserID = _LoggedInUserID;
        }
        public static string DecimalFormat { get; set; }
        public static string TareWeightDecimal { get { return "(Tare Wt:" + DecimalFormat + ")"; } }
        public static string SelectedDateFormat { get; set; }
        public static string SelectedTimeFormat { get; set; }
        public static string DateTimeFormat { get; set; }


        public static string Licensekey
        {
            get
            {
                if (string.IsNullOrEmpty(Licensekey))
                {
                    try
                    {
                        string licensepath = Constant.CustomSettingsPath + @"\license.lic";
                        if (File.Exists(licensepath))
                        {
                            return File.ReadAllText(licensepath);
                        }
                        else
                        {
                            return "ERROR: License File not found";
                        }
                    }
                    catch (Exception ex)
                    {
                        return "ERROR: Unable to read license file. " + ex.Message;
                    }
                }
                else
                    return Licensekey;
            }
        }
        public static bool hasUnmannedPlugin { get; set; }
        public static string CustomImagesPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\WeighBridgeSolution\CustomImages";
        public static string CustomSettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\WeighBridgeSolution\CustomSettings";
        public static string PluginFileLocation = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\WeighBridgeSolution\Plugins";
        public static string ApplicationDataLocation = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\WeighBridgeSolution";
        public static string EmailFileLocation = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\WeighBridgeSolution\AttachmentFile";
        public static string ANPRCSVLocation = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\WeighBridgeSolution\CustomImages\CSV";
        public static string ModbusDataLocation = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\WeighBridgeSolution\Modbus";
		public static string SyncLog = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\WeighBridgeSolution\Sync";
        public static string DBNAME { get; set; }
        public static string DBSERVER { get; set; }
        public static string DBUSERNAME { get; set; }
        public static string DBPASSWORD { get; set; }
        public static int DBPORT { get; set; }

        private static string _cameraimagelocation;

        public static string CameraImageLocation
        {
            get
            {
                if (String.IsNullOrEmpty(_cameraimagelocation))
                {
                    using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                    {
                        ApplicationSetting CameraSetting = ctx.ApplicationSettings.FirstOrDefault(x => x.ModuleName == "CameraSetting" && x.SettingName == "CameraImagePath");
                        if (CameraSetting != null && CameraSetting.SettingValue != "")
                        {
                            try
                            {
                                if (!Directory.Exists(CameraSetting.SettingValue))
                                {
                                    DirectoryInfo di = Directory.CreateDirectory(CameraSetting.SettingValue);
                                    DirectorySecurity security = di.GetAccessControl();
                                    security.AddAccessRule(new FileSystemAccessRule(@"Users", FileSystemRights.Modify, AccessControlType.Allow));
                                    di.SetAccessControl(security);
                                }
                            }
                            catch (Exception)
                            {
                            }
                            _cameraimagelocation = CameraSetting.SettingValue;
                        }
                    }
                }
                return _cameraimagelocation;
            }
            set { _cameraimagelocation = value; }
        }

        /// <summary>
        /// Gives Formated datetime as per globle setting in string formate.
        /// </summary>
        /// <param name="dateTime">datetime which is required to be formated</param>
        /// <returns></returns>
        public static string FormateDateTimeOutString(DateTime dateTime)
        {
            try
            {
                return dateTime.ToString(DateTimeFormat, CultureInfo.InvariantCulture);
            }
            catch
            {
                return dateTime.ToString();
            }
        }

        /// <summary>
        /// Gives Formated datetime as per globle setting in DateTime formate.
        /// </summary>
        /// <param name="dateTime">datetime which is required to be formated</param>
        /// <returns></returns>
        public static DateTime FormateDateTimeOutDateTime(DateTime dateTime)
        {
            try
            {
                return Convert.ToDateTime(dateTime.ToString(DateTimeFormat, CultureInfo.InvariantCulture));
            }
            catch
            {
                return dateTime;
            }
        }

        /// <summary>
        /// Gives Formated decimal as per globle setting.
        /// </summary>
        /// <param name="value">decimal which is required to be formated</param>
        /// <returns></returns>
        public static Decimal FormateDecimal(Decimal value)
        {
            return Convert.ToDecimal(value.ToString(DecimalFormat));
        }

        //public static decimal? FormateNullableDecimal(decimal? value)
        //{
        //    if (value == null)
        //    {
        //        return null;
        //    }

        //    var decimalValue = Convert.ToDecimal(value);
        //    return Convert.ToDecimal(decimalValue.ToString("kk"));
        //}
    }
}
