using log4net;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;

namespace WeighingSolution.DataLayer
{

    public class Common
    {


        public static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static string getCompanyName()
        {
            string CompanyName = "";
            using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
            {
                ApplicationSetting cname = ctx.ApplicationSettings.FirstOrDefault(x => x.ModuleName == PageEntity.CompanySetting.ToString() && x.SettingName == "CompanyName");
                if (cname != null)
                    CompanyName = cname.SettingValue;
            }
            return CompanyName;
        }
        public static string CameraImageLocation()
        {
            try
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
                        return CameraSetting.SettingValue;
                    }
                    else
                        return string.Empty;

                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string getAddress()
        {
            string CompanyName = "";
            using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
            {
                ApplicationSetting cname = ctx.ApplicationSettings.FirstOrDefault(x => x.ModuleName == PageEntity.CompanySetting.ToString() && x.SettingName == "Address");
                if (cname != null)
                    CompanyName = cname.SettingValue;
            }
            return CompanyName;
        }

        public static ReportParameter[] getmasterFieldVisbility()
        {
            ReportParameter[] cmfVisibleParamCollection = new ReportParameter[6];
            using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
            {
                MasterField[] lstmstfld = ctx.MasterFields.ToArray();
                for (int i = 1; i <= lstmstfld.Length; i++)
                {
                    cmfVisibleParamCollection[i - 1] = new ReportParameter("mfVisible" + i, (!lstmstfld[i - 1].Isvisible).ToString());
                }
            }
            return cmfVisibleParamCollection;
        }

        //public static string SetDisplayTicketID(int ticketid)
        //{
        //    string displayticket = "";
        //    string PreFixtct = "";
        //    string PostFixTct = "";


        //    using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
        //    {
        //        ApplicationSetting PreFixTicket = ctx.ApplicationSettings.FirstOrDefault(x => x.ModuleName == PageEntity.GlobalSetting.ToString() && x.SettingName == "PreFixTicket");
        //        ApplicationSetting PostFixTicket = ctx.ApplicationSettings.FirstOrDefault(x => x.ModuleName == PageEntity.GlobalSetting.ToString() && x.SettingName == "PostFixTicket");
        //        if (PreFixTicket != null)
        //            PreFixtct = PreFixTicket.SettingValue;

        //        if (PostFixTicket != null)
        //            PostFixTct = PostFixTicket.SettingValue;
        //    }

        //        if (!string.IsNullOrEmpty(PreFixtct))
        //            displayticket += PreFixtct + "/";

        //        displayticket += ticketid.ToString();

        //        if (!string.IsNullOrEmpty(PostFixTct))
        //            displayticket += "/" + PostFixTct;

        //        return displayticket;
        //}

        public static bool DisableSpecialCharactersInVehicle
        {
            get
            {
                bool flag = false;

                try
                {
                    using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                    {
                        ApplicationSetting cname = ctx.ApplicationSettings.FirstOrDefault(x => x.ModuleName == PageEntity.GlobalSetting.ToString() && x.SettingName == "DisableSpecialCharactersInVehicle");
                        if (cname != null)
                            flag = Convert.ToBoolean(cname.SettingValue);
                    }
                }
                catch { }
                return flag;
            }
        }

        public static int VehicleNumberLength
        {
            get
            {
                int flag = 13;

                try
                {
                    using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                    {
                        ApplicationSetting cname = ctx.ApplicationSettings.FirstOrDefault(x => x.ModuleName == PageEntity.GlobalSetting.ToString() && x.SettingName == "VehicleNumberLength");
                        if (cname != null)
                            flag = Convert.ToInt32(cname.SettingValue);
                    }
                }
                catch { }
                return flag;
            }
        }

        public static bool ValidatePageRequest(PageEntity FeatureMaster, PageAccess access)
        {
            bool RetVal = false;
            int FeatureID = (int)FeatureMaster;

            using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
            {
                //  List<UserPermission> lstPermission;
                List<TemplatePermission> lstPermission = null;
                if (Constant.lstPermission != null && Constant.lstPermission.Count > 0)
                    // lstPermission = Constant.lstPermission as List<UserPermission>;
                    lstPermission = Constant.lstPermission as List<TemplatePermission>;
                else
                {
                    //  lstPermission = ctx.UserPermissions.Where(x => x.UserID == Constant.LoggedInUserID).ToList();
                    var templateid = ctx.UserMasters.FirstOrDefault(x => x.UserID == Constant.LoggedInUserID).TemplateID;
                    if (templateid != null && templateid > 0)
                        lstPermission = ctx.TemplatePermissions.Where(x => x.TemplateID == templateid && x.Template.Active).ToList();
                    Constant.lstPermission = lstPermission;
                }
                if (lstPermission != null && lstPermission.Count > 0)
                {
                    //UserPermission objUP = lstPermission.FirstOrDefault(x => x.FeatureID == FeatureID);
                    TemplatePermission objUP = lstPermission.FirstOrDefault(x => x.FeatureID == FeatureID);
                    if (objUP != null)
                    {
                        if (access == PageAccess.Create)
                        {
                            if (objUP.PermissionSet[0].ToString().Equals("1"))
                                RetVal = true;
                        }
                        else if (access == PageAccess.Read)
                        {
                            if (objUP.PermissionSet[1].ToString().Equals("1"))
                                RetVal = true;
                        }
                        else if (access == PageAccess.Update)
                        {
                            if (objUP.PermissionSet[2].ToString().Equals("1"))
                                RetVal = true;
                        }
                        else if (access == PageAccess.Delete)
                        {
                            if (objUP.PermissionSet[3].ToString().Equals("1"))
                                RetVal = true;
                        }
                        else if (access == PageAccess.Execute)
                        {
                            if (objUP.PermissionSet[4].ToString().Equals("1"))
                                RetVal = true;
                        }
                    }
                }
            }
            return RetVal;
        }

        private static string EncKey = "JiBWUOES5Pau4sfm0hqgHUbIoZrlyBKG4sE8SzP2zNM=";
        public static string PublicKey = "MIIBKjCB4wYHKoZIzj0CATCB1wIBATAsBgcqhkjOPQEBAiEA/////wAAAAEAAAAAAAAAAAAAAAD///////////////8wWwQg/////wAAAAEAAAAAAAAAAAAAAAD///////////////wEIFrGNdiqOpPns+u9VXaYhrxlHQawzFOw9jvOPD4n0mBLAxUAxJ02CIbnBJNqZnjhE50mt4GffpAEIQNrF9Hy4SxCR/i85uVjpEDydwN9gS3rM6D0oTlF2JjClgIhAP////8AAAAA//////////+85vqtpxeehPO5ysL8YyVRAgEBA0IABHMGXb4zv0P8LWqnREQduAMXFmNZ9b4lr8oum3kJcXobgkrm191r4DZjXGXLA5fvBSBJFKkZb8V1keuwHNuMJz4=";
        public static string BackupDB = "BackupWeighbridge";
        public static string BackupDBTable = "Backuptbl";
        public static string BackupDBColumn = "BackupData";

        public static string EncryptData(string Str)
        {
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(Str);

                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(EncKey));

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string DecryptData(string Str)
        {
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = Convert.FromBase64String(Str);
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(EncKey));
                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static bool IsSaltMatch(string username, string password, UserMaster objUser)
        {
            try
            {
                if (!string.IsNullOrEmpty(objUser.Salt))
                    return objUser.Salt.Equals(createSalt(username, password, objUser));
                else
                    return false;
            }
            catch (Exception ex)
            {
                logErrorInDB(ex.Message, ex);
                return false;
            }
        }

        public static string createSalt(string username, string password, UserMaster objUser)
        {
            try
            {
                string secret = "jm3UIgFC7CCQwqtr";
                string userpwdComb = username.ToLower() + password.ToLower();
                var encoding = new UTF8Encoding();
                byte[] keyByte = encoding.GetBytes(secret);
                byte[] messageBytes = encoding.GetBytes(userpwdComb);
                using (var hmacsha256 = new HMACSHA256(keyByte))
                {
                    byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                    return (Convert.ToBase64String(hashmessage));
                }
            }
            catch (Exception ex)
            {
                logErrorInDB(ex.Message, ex);
                return "";
            }
        }

        public static void logErrorInDB(string Message, Exception ex = null)
        {
            if (ex != null)
                Log.Error(Message, ex);
            else
                Log.Error(Message);
        }
        public static void SetFullAccessForFile(string filepath)
        {
            try
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(filepath);
                System.Security.AccessControl.FileSecurity security = fi.GetAccessControl();
                System.Security.AccessControl.AuthorizationRuleCollection arcFile = security.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
                foreach (System.Security.AccessControl.AuthorizationRule arFile in arcFile)
                {
                    string account = arFile.IdentityReference.Translate(typeof(System.Security.Principal.NTAccount)).Value;
                    if (account.Contains("\\"))
                        account = account.Substring(account.LastIndexOf("\\") + 1);
                    if (account.ToLower() == "users")
                    {
                        System.Security.AccessControl.FileSystemAccessRule fasrFile = (System.Security.AccessControl.FileSystemAccessRule)arFile;
                        if (fasrFile.AccessControlType == System.Security.AccessControl.AccessControlType.Allow && fasrFile.FileSystemRights.HasFlag(System.Security.AccessControl.FileSystemRights.Modify))
                        {
                            return;
                        }
                    }
                }
                security.AddAccessRule(new System.Security.AccessControl.FileSystemAccessRule("Users", System.Security.AccessControl.FileSystemRights.Modify, System.Security.AccessControl.AccessControlType.Allow));
                fi.SetAccessControl(security);
            }
            catch (Exception ex)
            {
                logErrorInDB(ex.Message, ex);
            }
        }
        public static void CreateDirectoryWithFullAccess(string DirectoryPath)
        {
            try
            {
                if (!System.IO.Directory.Exists(DirectoryPath))
                {
                    System.IO.DirectoryInfo di = System.IO.Directory.CreateDirectory(DirectoryPath);
                    System.Security.AccessControl.DirectorySecurity security = di.GetAccessControl();
                    security.AddAccessRule(new System.Security.AccessControl.FileSystemAccessRule(@"Users", System.Security.AccessControl.FileSystemRights.FullControl, System.Security.AccessControl.InheritanceFlags.ContainerInherit | System.Security.AccessControl.InheritanceFlags.ObjectInherit, System.Security.AccessControl.PropagationFlags.None, System.Security.AccessControl.AccessControlType.Allow));
                    di.SetAccessControl(security);
                }
            }
            catch (Exception ex)
            {
                logErrorInDB(ex.Message, ex);
            }
        }
        public static string GetmailBody(TransactionData objTran, EmailSetup item, DataTable dtPass)
        {
            string mailBody = "";
            string[] columnselection = item.ColumnSelection.Split(',');
            if (columnselection != null)
            {
                if (objTran.TransactionMode == TransactionMode.Single.ToString() || objTran.TransactionMode == TransactionMode.Double.ToString())
                {
                    if (dtPass != null && dtPass.Rows.Count > 0)
                    {
                        mailBody = "<br/><table width='30%' style='border:none;'>";
                        decimal GrossWT = dtPass.Rows[0]["GrossWeight"] != null ? Convert.ToDecimal(dtPass.Rows[0]["GrossWeight"]) : 0.00m;
                        decimal TareWT = dtPass.Rows[0]["TareWeight"] != null ? Convert.ToDecimal(dtPass.Rows[0]["TareWeight"]) : 0.00m;
                        decimal NetWT = dtPass.Rows[0]["NetWeight"] != null ? Convert.ToDecimal(dtPass.Rows[0]["NetWeight"]) : 0.00m;
                        DateTime GrossTime = dtPass.Rows[0]["GrossTime"] != null ? Convert.ToDateTime(dtPass.Rows[0]["GrossTime"]) : DateTime.Now;
                        DateTime TareTime = dtPass.Rows[0]["TareTime"] != null ? Convert.ToDateTime(dtPass.Rows[0]["TareTime"]) : DateTime.Now;
                        foreach (var columnname in columnselection)
                        {
                            if (columnname == "TicketNo")
                                mailBody += "<tr><td align='left'>TicketNo :</td><td align='left'>" + dtPass.Rows[0]["TicketID"].ToString() + "</td></tr>";
                            if (columnname == "VehicleNo")
                                mailBody += "<tr><td align='left'>Vehicle No :</td><td align='left'>" + dtPass.Rows[0]["VehicleNumber"].ToString() + "</td></tr>";
                            if (columnname == "AccountName")
                                mailBody += "<tr><td align='left'>Account Name :</td><td align='left'>" + dtPass.Rows[0]["AccountName"].ToString() + "</td></tr>";
                            //if (columnname == "TokenValue")
                            //    mailBody += "<tr><td align='left'>Token Value :</td><td align='left'>" + dtPass.Rows[0]["TokenValue"].ToString() + "</td></tr>";
                            if (columnname == "GrossWeight" && GrossWT > 0)
                                mailBody += "<tr><td align='left'>Gross Weight :</td><td align='left'>" + GrossWT.ToString(Constant.DecimalFormat) + " " + dtPass.Rows[0]["WeightUnit"] + "</td></tr>";
                            if (columnname == "GrossTime")
                                mailBody += "<tr><td align='left'>Gross Time :</td><td align='left'>" + GrossTime.ToString(Constant.SelectedDateFormat + " " + Constant.SelectedTimeFormat) + "</td></tr>";
                            if (columnname == "TareWeight" && TareWT > 0)
                                mailBody += "<tr><td align='left'>TareWeight :</td><td align='left'>" + TareWT.ToString(Constant.DecimalFormat) + " " + dtPass.Rows[0]["WeightUnit"] + "</td></tr>";
                            if (columnname == "TareTime")
                                mailBody += "<tr><td align='left'>Tare Time :</td><td align='left'>" + TareTime.ToString(Constant.SelectedDateFormat + " " + Constant.SelectedTimeFormat) + "</td></tr>";
                            if (columnname == "NetWeight" && NetWT > 0)
                                mailBody += "<tr><td align='left'>Net Weight :</td><td align='left'>" + NetWT.ToString(Constant.DecimalFormat) + " " + dtPass.Rows[0]["WeightUnit"] + "</td></tr>";
                            if (columnname == "TransactionType")
                                mailBody += "<tr><td align='left'>TransactionType :</td><td align='left'>" + dtPass.Rows[0]["TransactionType"].ToString() + "</td></tr>";
                            if (columnname == "Charges")
                                mailBody += "<tr><td align='left'>Charges :</td><td align='left'>" + dtPass.Rows[0]["Charges"].ToString() + "</td></tr>";
                            if (columnname == "Field1")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field1DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field1Value"].ToString() + "</td></tr>";
                            if (columnname == "Field2")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field2DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field2Value"].ToString() + "</td></tr>";
                            if (columnname == "Field3")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field3DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field3Value"].ToString() + "</td></tr>";
                            if (columnname == "Field4")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field4DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field4Value"].ToString() + "</td></tr>";
                            if (columnname == "Field5")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field5DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field5Value"].ToString() + "</td></tr>";
                            if (columnname == "Field6")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field6DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field6Value"].ToString() + "</td></tr>";
                            if (columnname == "Field7")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field7DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field7Value"].ToString() + "</td></tr>";
                            if (columnname == "Field8")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field8DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field8Value"].ToString() + "</td></tr>";
                            if (columnname == "Field9")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field9DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field9Value"].ToString() + "</td></tr>";
                            if (columnname == "Field10")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field10DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field10Value"].ToString() + "</td></tr>";
                            if (columnname == "Field11")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field11DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field11Value"].ToString() + "</td></tr>";
                            if (columnname == "Field12")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field12DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field12Value"].ToString() + "</td></tr>";
                            if (columnname == "Field13")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field13DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field13Value"].ToString() + "</td></tr>";
                            if (columnname == "Field14")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field14DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field14Value"].ToString() + "</td></tr>";
                            if (columnname == "Field15")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field15DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field15Value"].ToString() + "</td></tr>";
                            if (columnname == "Field16")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field16DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field16Value"].ToString() + "</td></tr>";
                            if (columnname == "Field17")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field17DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field17Value"].ToString() + "</td></tr>";
                            if (columnname == "Field18")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field18DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field18Value"].ToString() + "</td></tr>";
                            if (columnname == "Field19")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field19DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field19Value"].ToString() + "</td></tr>";
                            if (columnname == "Field20")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field20DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field20Value"].ToString() + "</td></tr>";
                        }
                        mailBody += "</table>";
                    }
                }
                else
                {
                    string GrossWeight = objTran.TransactionDetails.Max(x => x.CaptureWeight).Value.ToString(Constant.DecimalFormat);
                    string TareWeight = objTran.TransactionDetails.Min(x => x.CaptureWeight).Value.ToString(Constant.DecimalFormat);
                    string NetWeight = (Convert.ToDecimal(GrossWeight) - Convert.ToDecimal(TareWeight)).ToString(Constant.DecimalFormat);
                    decimal GWt = Convert.ToDecimal(GrossWeight);
                    string GrossTime = objTran.TransactionDetails.FirstOrDefault(x => x.GrossWeight == GWt).GrossTime.Value.ToString(Constant.DateTimeFormat);
                    decimal Twt = Convert.ToDecimal(TareWeight);
                    string TareTime = objTran.TransactionDetails.FirstOrDefault(x => x.TareWeight == Twt).TareTime.Value.ToString(Constant.DateTimeFormat);
                    if (dtPass != null && dtPass.Rows.Count > 0)
                    {
                        mailBody = "<br/><table width='50%' style='border:none;'>";
                        foreach (var columnname in columnselection)
                        {
                            if (columnname == "TicketNo")
                                mailBody += "<tr><td align='left'>TicketNo:</td><td align='left'>" + dtPass.Rows[0]["TicketID"].ToString() + "</td></tr>";
                            if (columnname == "VehicleNo")
                                mailBody += "<tr><td align='left'>Vehicle No:</td><td align='left'>" + dtPass.Rows[0]["VehicleNumber"].ToString() + "</td></tr>";
                            if (columnname == "AccountName")
                                mailBody += "<tr><td align='left'>Account Name:</td><td align='left'>" + dtPass.Rows[0]["AccountName"].ToString() + "</td></tr>";
                            //if (columnname == "TokenValue")
                            //    mailBody += "<tr><td align='left'>Token Value:</td><td align='left'>" + dtPass.Rows[0]["TokenValue"].ToString() + "</td></tr>";
                            if (columnname == "GrossWeight")
                                mailBody += "<tr><td align='left'>Gross Weight:</td><td align='left'>" + GrossWeight + " " + objTran.TransactionDetails.FirstOrDefault(x => x.GrossWeight == GWt).WeightUnit + "</td></tr>";
                            if (columnname == "GrossTime")
                                mailBody += "<tr><td align='left'>Gross Time:</td><td align='left'>" + GrossTime + "</td></tr>";
                            if (columnname == "TareWeight")
                                mailBody += "<tr><td align='left'>TareWeight:</td><td align='left'>" + TareWeight + " " + objTran.TransactionDetails.FirstOrDefault(x => x.TareWeight == Twt).WeightUnit + "</td></tr>";
                            if (columnname == "TareTime")
                                mailBody += "<tr><td align='left'>Tare Time:</td><td align='left'>" + TareTime + "</td></tr>";
                            if (columnname == "NetWeight")
                                mailBody += "<tr><td align='left'>Net Weight:</td><td align='left'>" + NetWeight + " " + objTran.TransactionDetails.FirstOrDefault(x => x.GrossWeight == GWt).WeightUnit + "</td></tr>";
                            if (columnname == "TransactionType")
                                mailBody += "<tr><td align='left'>TransactionType :</td><td align='left'>" + dtPass.Rows[0]["TransactionType"].ToString() + "</td></tr>";
                            if (columnname == "Charges")
                                mailBody += "<tr><td align='left'>Charges:</td><td align='left'>" + dtPass.Rows[0]["Charges"].ToString() + "</td></tr>";
                            if (columnname == "Field1")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field1DisplayName"] + "</td><td align='left'>" + dtPass.Rows[0]["Field1Value"].ToString() + "</td></tr>";
                            if (columnname == "Field2")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field2DisplayName"] + "</td><td align='left'>" + dtPass.Rows[0]["Field2Value"].ToString() + "</td></tr>";
                            if (columnname == "Field3")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field3DisplayName"] + "</td><td align='left'>" + dtPass.Rows[0]["Field3Value"].ToString() + "</td></tr>";
                            if (columnname == "Field4")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field4DisplayName"] + "</td><td align='left'>" + dtPass.Rows[0]["Field4Value"].ToString() + "</td></tr>";
                            if (columnname == "Field5")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field5DisplayName"] + "</td><td align='left'>" + dtPass.Rows[0]["Field5Value"].ToString() + "</td></tr>";
                            if (columnname == "Field6")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field6DisplayName"] + "</td><td align='left'>" + dtPass.Rows[0]["Field6Value"].ToString() + "</td></tr>";
                            if (columnname == "Field7")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field7DisplayName"] + "</td><td align='left'>" + dtPass.Rows[0]["Field7Value"].ToString() + "</td></tr>";
                            if (columnname == "Field8")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field8DisplayName"] + "</td><td align='left'>" + dtPass.Rows[0]["Field8Value"].ToString() + "</td></tr>";
                            if (columnname == "Field9")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field9DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field9Value"].ToString() + "</td></tr>";
                            if (columnname == "Field10")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field10DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field10Value"].ToString() + "</td></tr>";
                            if (columnname == "Field11")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field11DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field11Value"].ToString() + "</td></tr>";
                            if (columnname == "Field12")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field12DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field12Value"].ToString() + "</td></tr>";
                            if (columnname == "Field13")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field13DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field13Value"].ToString() + "</td></tr>";
                            if (columnname == "Field14")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field14DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field14Value"].ToString() + "</td></tr>";
                            if (columnname == "Field15")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field15DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field15Value"].ToString() + "</td></tr>";
                            if (columnname == "Field16")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field16DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field16Value"].ToString() + "</td></tr>";
                            if (columnname == "Field17")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field17DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field17Value"].ToString() + "</td></tr>";
                            if (columnname == "Field18")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field18DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field18Value"].ToString() + "</td></tr>";
                            if (columnname == "Field19")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field19DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field19Value"].ToString() + "</td></tr>";
                            if (columnname == "Field20")
                                mailBody += "<tr><td align='left'>" + dtPass.Rows[0]["Field20DisplayName"] + " :</td><td align='left'>" + dtPass.Rows[0]["Field20Value"].ToString() + "</td></tr>";
                        }
                        mailBody += "</table>";
                    }
                }
            }
            return mailBody;
        }

        //public static string GetSMSBody(TransactionData objTran, SMSSetup item)
        //{
        //    string smsBody = "";
        //    SqlConnection con = new SqlConnection(System.Windows.Application.Current.Properties["DefalutConnectionString"].ToString());

        //    //Single OR Double Pass
        //    DataTable dt = new DataTable();
        //    var parentSQL = "SELECT TD.TicketID,A.CompanyName AS AccountName,TR.CompanyName AS TransaporterName,TD.Charges,TD.TransactionMode,TD.TransactionType,TXD.TareTime,TXD.GrossTime, ISNULL(TXD.TareWeight, 0) AS TareWeight, ISNULL(TXD.GrossWeight, 0) AS GrossWeight, ISNULL(TXD.NetWeight, 0) AS NetWeight,TXD.IsTareManual, TXD.IsGrossmanual, TXD.WeightUnit,P.ProductName,G.GateName,TD.Charges,(D.FirstName + ' ' + D.LastName) AS DriverName,V.VehicleNumber,Field1DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field1'),Field2DisplayName = (SELECT DisplayName	FROM CustomField WHERE FieldName = 'field2'),Field3DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field3'),Field4DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field4'),Field5DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field5'),Field6DisplayName = (SELECT DisplayName	FROM CustomField WHERE FieldName = 'field6'),Field7DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field7'),Field8DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field8'),Field9DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field9'),Field10DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field10'),Field11DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field11'),Field12DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field12'),Field13DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field13'),Field14DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field14'),Field15DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field15'),Field16DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field16'),Field17DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field17'),Field18DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field18'),Field19DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field19'),Field20DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field20'),Field1Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field1') AND ReceiptTicketID = TD.ReceiptTicketID),Field2Value = (SELECT CustomValue FROM CustomValue	WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field2') AND ReceiptTicketID = TD.ReceiptTicketID),Field3Value = (SELECT CustomValue	FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field3') AND ReceiptTicketID = TD.ReceiptTicketID),Field4Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField	WHERE FieldName = 'field4') AND ReceiptTicketID = TD.ReceiptTicketID),Field5Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field5') AND ReceiptTicketID = TD.ReceiptTicketID),Field6Value = (SELECT CustomValue FROM CustomValue	WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field6') AND ReceiptTicketID = TD.ReceiptTicketID),Field7Value = (SELECT CustomValue	FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field7') AND ReceiptTicketID = TD.ReceiptTicketID),Field8Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField	WHERE FieldName = 'field8')	AND ReceiptTicketID = TD.ReceiptTicketID),Field9Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field9') AND ReceiptTicketID = TD.ReceiptTicketID),Field10Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field10') AND ReceiptTicketID = TD.ReceiptTicketID),Field11Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field11') AND ReceiptTicketID = TD.ReceiptTicketID),Field12Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field12') AND ReceiptTicketID = TD.ReceiptTicketID),Field13Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field13') AND ReceiptTicketID = TD.ReceiptTicketID),Field14Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field14') AND ReceiptTicketID = TD.ReceiptTicketID),Field15Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field15') AND ReceiptTicketID = TD.ReceiptTicketID),Field16Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field16') AND ReceiptTicketID = TD.ReceiptTicketID),Field17Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field17') AND ReceiptTicketID = TD.ReceiptTicketID),Field18Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field18') AND ReceiptTicketID = TD.ReceiptTicketID),Field19Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field19') AND ReceiptTicketID = TD.ReceiptTicketID),Field20Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field20') AND ReceiptTicketID = TD.ReceiptTicketID) FROM TransactionData TD LEFT JOIN Account A ON A.AccountID = TD.AccountID INNER JOIN TransactionDetail TXD ON TD.ReceiptTicketID = TXD.ReceiptTicketID AND (((TXD.GrossWeight IS NOT NULL AND TXD.GrossWeight <> 0) AND (TXD.TareWeight IS NOT NULL AND TXD.TareWeight <> 0))	OR (TD.STATUS = 'Active' AND (TD.TransactionMode = 'Double'))) LEFT JOIN Product P ON P.ProductID = TXD.ProductID LEFT JOIN Gate G ON G.GateID = TXD.GateID LEFT JOIN Account TR ON TR.AccountID = TD.TransporterID LEFT JOIN Vehicle V ON V.VehicleID = TD.VehicleID LEFT JOIN Driver D ON D.DriverID = TD.DriverID WHERE TD.ReceiptTicketID = '" + objTran.ReceiptTicketID + "'";
        //    SqlCommand cmd = new SqlCommand(parentSQL, con)
        //    {
        //        CommandType = CommandType.Text,
        //        CommandTimeout = 45
        //    };

        //    //MultiPass
        //    DataTable dtMultipass = new DataTable();
        //    var MultipassSQL = "SELECT TD.TicketID,A.CompanyName AS AccountName,TR.CompanyName AS TransaporterName	,TD.TransactionMode,TD.TransactionType,TD.Charges,(D.FirstName + ' ' + D.LastName) AS DriverName	,V.VehicleNumber,Field1DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field1'),Field2DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field2'),Field3DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field3'),Field4DisplayName = (SELECT DisplayName	FROM CustomField WHERE FieldName = 'field4'),Field5DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field5'),Field6DisplayName = (SELECT DisplayName FROM CustomField WHERE FieldName = 'field6'),Field7DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field7'),Field8DisplayName = (SELECT DisplayName	FROM CustomField WHERE FieldName = 'field8'),Field11DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field11'),Field12DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field12'),Field13DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field13'),Field14DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field14'),Field15DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field15'),Field16DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field16'),Field17DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field17'),Field18DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field18'),Field19DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field19'),Field20DisplayName = (SELECT DisplayName FROM CustomField	WHERE FieldName = 'field20'),Field1Value = (SELECT CustomValue FROM CustomValue	WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field1') AND ReceiptTicketID = TD.ReceiptTicketID),Field2Value = (SELECT CustomValue	FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field2') AND ReceiptTicketID = TD.ReceiptTicketID ),Field3Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field3') AND ReceiptTicketID = TD.ReceiptTicketID),Field4Value = (SELECT CustomValue FROM CustomValue	WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field4') AND ReceiptTicketID = TD.ReceiptTicketID),Field5Value = (SELECT CustomValue	FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field5') AND ReceiptTicketID = TD.ReceiptTicketID),Field6Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField	WHERE FieldName = 'field6')	AND ReceiptTicketID = TD.ReceiptTicketID),Field7Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field7') AND ReceiptTicketID = TD.ReceiptTicketID),Field8Value = (SELECT CustomValue FROM CustomValue	WHERE CustomFieldID IN (SELECT CustomFieldID FROM CustomField WHERE FieldName = 'field8') AND ReceiptTicketID = TD.ReceiptTicketID),Field9Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field9') AND ReceiptTicketID = TD.ReceiptTicketID),Field10Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field10') AND ReceiptTicketID = TD.ReceiptTicketID),Field11Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field11') AND ReceiptTicketID = TD.ReceiptTicketID),Field12Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field12') AND ReceiptTicketID = TD.ReceiptTicketID),Field13Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field13') AND ReceiptTicketID = TD.ReceiptTicketID),Field14Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field14') AND ReceiptTicketID = TD.ReceiptTicketID),Field15Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field15') AND ReceiptTicketID = TD.ReceiptTicketID),Field16Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field16') AND ReceiptTicketID = TD.ReceiptTicketID),Field17Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field17') AND ReceiptTicketID = TD.ReceiptTicketID),Field18Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field18') AND ReceiptTicketID = TD.ReceiptTicketID),Field19Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field19') AND ReceiptTicketID = TD.ReceiptTicketID),Field20Value = (SELECT CustomValue FROM CustomValue WHERE CustomFieldID IN (SELECT CustomFieldID	FROM CustomField WHERE FieldName = 'field20') AND ReceiptTicketID = TD.ReceiptTicketID) FROM TransactionData TD LEFT JOIN Account A ON A.AccountID = TD.AccountID LEFT JOIN Account TR ON TR.AccountID = TD.TransporterID LEFT JOIN Vehicle V ON V.VehicleID = TD.VehicleID LEFT JOIN Driver D ON D.DriverID = TD.DriverID WHERE TD.ReceiptTicketID = '" + objTran.ReceiptTicketID + "'";
        //    SqlCommand cmdMultipass = new SqlCommand(MultipassSQL, con)
        //    {
        //        CommandType = CommandType.Text,
        //        CommandTimeout = 45
        //    };

        //    con.Open();
        //    dt.Load(cmd.ExecuteReader());
        //    dtMultipass.Load(cmdMultipass.ExecuteReader());
        //    con.Close();
        //    string[] columnselection = item.ColumnSelection.Split(',');
        //    if (columnselection != null)
        //    {
        //        if (objTran.TransactionMode == WeighingSolution.DataLayer.TransactionMode.Single.ToString() || objTran.TransactionMode == WeighingSolution.DataLayer.TransactionMode.Double.ToString())
        //        {
        //            if (dt != null && dt.Rows.Count > 0)
        //            {
        //                decimal GrossWT = dt.Rows[0]["GrossWeight"] != null ? Convert.ToDecimal(dt.Rows[0]["GrossWeight"]) : 0.00m;
        //                decimal TareWT = dt.Rows[0]["TareWeight"] != null ? Convert.ToDecimal(dt.Rows[0]["TareWeight"]) : 0.00m;
        //                decimal NetWT = dt.Rows[0]["NetWeight"] != null ? Convert.ToDecimal(dt.Rows[0]["NetWeight"]) : 0.00m;
        //                DateTime GrossTime = dt.Rows[0]["GrossTime"] != null ? Convert.ToDateTime(dt.Rows[0]["GrossTime"]) : DateTime.Now;
        //                DateTime TareTime = dt.Rows[0]["TareTime"] != null ? Convert.ToDateTime(dt.Rows[0]["TareTime"]) : DateTime.Now;
        //                foreach (var columnname in columnselection)
        //                {
        //                    if (columnname == "TicketNo")
        //                        smsBody += "TicketNo :" + dt.Rows[0]["TicketID"].ToString() + ";";
        //                    if (columnname == "VehicleNo")
        //                        smsBody += "Vehicle No :" + dt.Rows[0]["VehicleNumber"].ToString() + ";";
        //                    if (columnname == "AccountName")
        //                        smsBody += "Account Name :" + dt.Rows[0]["AccountName"].ToString() + ";";
        //                    //if (columnname == "TokenValue")
        //                    //    smsBody += "Token Value :" + dt.Rows[0]["TokenValue"].ToString() + ";";
        //                    if (columnname == "GrossWeight" && GrossWT > 0)
        //                        smsBody += "Gross Weight :" + GrossWT.ToString(Constant.DecimalFormat) + " " + dt.Rows[0]["WeightUnit"] + ";";
        //                    if (columnname == "GrossTime")
        //                        smsBody += "Gross Time :" + GrossTime.ToString(Constant.SelectedDateFormat + " " + Constant.SelectedTimeFormat) + ";";
        //                    if (columnname == "TareWeight" && TareWT > 0)
        //                        smsBody += "Tare Weight :" + TareWT.ToString(Constant.DecimalFormat) + " " + dt.Rows[0]["WeightUnit"] + ";";
        //                    if (columnname == "TareTime")
        //                        smsBody += "Tare Time :" + TareTime.ToString(Constant.SelectedDateFormat + " " + Constant.SelectedTimeFormat) + ";";
        //                    if (columnname == "NetWeight" && NetWT > 0)
        //                        smsBody += "Net Weight :" + NetWT.ToString(Constant.DecimalFormat) + " " + dt.Rows[0]["WeightUnit"] + ";";
        //                    if (columnname == "TransactionType")
        //                        smsBody += "TransactionType :" + dt.Rows[0]["TransactionType"].ToString() + ";";
        //                    if (columnname == "Charges")
        //                        smsBody += "Charges :" + dt.Rows[0]["Charges"].ToString() + ";";
        //                    if (columnname == "Field1")
        //                        smsBody += dt.Rows[0]["Field1DisplayName"] + " :" + dt.Rows[0]["Field1Value"].ToString() + ";";
        //                    if (columnname == "Field2")
        //                        smsBody += dt.Rows[0]["Field2DisplayName"] + " :" + dt.Rows[0]["Field2Value"].ToString() + ";";
        //                    if (columnname == "Field3")
        //                        smsBody += dt.Rows[0]["Field3DisplayName"] + " :" + dt.Rows[0]["Field3Value"].ToString() + ";";
        //                    if (columnname == "Field4")
        //                        smsBody += dt.Rows[0]["Field4DisplayName"] + " :" + dt.Rows[0]["Field4Value"].ToString() + ";";
        //                    if (columnname == "Field5")
        //                        smsBody += dt.Rows[0]["Field5DisplayName"] + " :" + dt.Rows[0]["Field5Value"].ToString() + ";";
        //                    if (columnname == "Field6")
        //                        smsBody += dt.Rows[0]["Field6DisplayName"] + " :" + dt.Rows[0]["Field6Value"].ToString() + ";";
        //                    if (columnname == "Field7")
        //                        smsBody += dt.Rows[0]["Field7DisplayName"] + " :" + dt.Rows[0]["Field7Value"].ToString() + ";";
        //                    if (columnname == "Field8")
        //                        smsBody += dt.Rows[0]["Field8DisplayName"] + " :" + dt.Rows[0]["Field8Value"].ToString() + ";";
        //                    if (columnname == "Field9")
        //                        smsBody += dt.Rows[0]["Field9DisplayName"] + " :" + dt.Rows[0]["Field9Value"].ToString() + ";";
        //                    if (columnname == "Field10")
        //                        smsBody += dt.Rows[0]["Field10DisplayName"] + " :" + dt.Rows[0]["Field10Value"].ToString() + ";";
        //                    if (columnname == "Field11")
        //                        smsBody += dt.Rows[0]["Field11DisplayName"] + " :" + dt.Rows[0]["Field11Value"].ToString() + ";";
        //                    if (columnname == "Field12")
        //                        smsBody += dt.Rows[0]["Field12DisplayName"] + " :" + dt.Rows[0]["Field12Value"].ToString() + ";";
        //                    if (columnname == "Field13")
        //                        smsBody += dt.Rows[0]["Field13DisplayName"] + " :" + dt.Rows[0]["Field13Value"].ToString() + ";";
        //                    if (columnname == "Field14")
        //                        smsBody += dt.Rows[0]["Field14DisplayName"] + " :" + dt.Rows[0]["Field14Value"].ToString() + ";";
        //                    if (columnname == "Field15")
        //                        smsBody += dt.Rows[0]["Field15DisplayName"] + " :" + dt.Rows[0]["Field15Value"].ToString() + ";";
        //                    if (columnname == "Field16")
        //                        smsBody += dt.Rows[0]["Field16DisplayName"] + " :" + dt.Rows[0]["Field16Value"].ToString() + ";";
        //                    if (columnname == "Field17")
        //                        smsBody += dt.Rows[0]["Field17DisplayName"] + " :" + dt.Rows[0]["Field17Value"].ToString() + ";";
        //                    if (columnname == "Field18")
        //                        smsBody += dt.Rows[0]["Field18DisplayName"] + " :" + dt.Rows[0]["Field18Value"].ToString() + ";";
        //                    if (columnname == "Field19")
        //                        smsBody += dt.Rows[0]["Field19DisplayName"] + " :" + dt.Rows[0]["Field19Value"].ToString() + ";";
        //                    if (columnname == "Field20")
        //                        smsBody += dt.Rows[0]["Field20DisplayName"] + " :" + dt.Rows[0]["Field20Value"].ToString() + ";";
        //                }
        //            }
        //        }
        //        else
        //        {
        //            string GrossWeight = objTran.TransactionDetails.Max(x => x.CaptureWeight).Value.ToString(Constant.DecimalFormat);
        //            string TareWeight = objTran.TransactionDetails.Min(x => x.CaptureWeight).Value.ToString(Constant.DecimalFormat);
        //            string NetWeight = (Convert.ToDecimal(GrossWeight) - Convert.ToDecimal(TareWeight)).ToString(Constant.DecimalFormat);
        //            decimal GWt = Convert.ToDecimal(GrossWeight);
        //            string GrossTime = objTran.TransactionDetails.FirstOrDefault(x => x.GrossWeight == GWt).GrossTime.Value.ToString(Constant.DateTimeFormat);
        //            decimal Twt = Convert.ToDecimal(TareWeight);
        //            string TareTime = objTran.TransactionDetails.FirstOrDefault(x => x.TareWeight == Twt).TareTime.Value.ToString(Constant.DateTimeFormat);
        //            if (dtMultipass != null && dtMultipass.Rows.Count > 0)
        //            {
        //                foreach (var columnname in columnselection)
        //                {
        //                    if (columnname == "TicketNo")
        //                        smsBody += "TicketNo:" + dtMultipass.Rows[0]["TicketID"].ToString() + ";";
        //                    if (columnname == "VehicleNo")
        //                        smsBody += "Vehicle No:" + dtMultipass.Rows[0]["VehicleNumber"].ToString() + ";";
        //                    if (columnname == "AccountName")
        //                        smsBody += "Account Name:" + dtMultipass.Rows[0]["AccountName"].ToString() + ";";
        //                    //if (columnname == "TokenValue")
        //                    //    smsBody += "Token Value:" + dtMultipass.Rows[0]["TokenValue"].ToString() + ";";
        //                    if (columnname == "GrossWeight")
        //                        smsBody += "Gross Weight:" + GrossWeight + " " + objTran.TransactionDetails.FirstOrDefault(x => x.GrossWeight == GWt).WeightUnit + ";";
        //                    if (columnname == "GrossTime")
        //                        smsBody += "Gross Time:" + GrossTime + ";";
        //                    if (columnname == "TareWeight")
        //                        smsBody += "Tare Weight:" + TareWeight + " " + objTran.TransactionDetails.FirstOrDefault(x => x.TareWeight == Twt).WeightUnit + ";";
        //                    if (columnname == "TareTime")
        //                        smsBody += "Tare Time:" + TareTime + ";";
        //                    if (columnname == "NetWeight")
        //                        smsBody += "Net Weight:" + NetWeight + " " + objTran.TransactionDetails.FirstOrDefault(x => x.GrossWeight == GWt).WeightUnit + ";";
        //                    if (columnname == "TransactionType")
        //                        smsBody += "TransactionType :" + dtMultipass.Rows[0]["TransactionType"].ToString() + ";";
        //                    if (columnname == "Charges")
        //                        smsBody += "Charges:" + dtMultipass.Rows[0]["Charges"].ToString() + ";";
        //                    if (columnname == "Field1")
        //                        smsBody += dtMultipass.Rows[0]["Field1DisplayName"] + " :" + dtMultipass.Rows[0]["Field1Value"].ToString() + ";";
        //                    if (columnname == "Field2")
        //                        smsBody += dtMultipass.Rows[0]["Field2DisplayName"] + " :" + dtMultipass.Rows[0]["Field2Value"].ToString() + ";";
        //                    if (columnname == "Field3")
        //                        smsBody += dtMultipass.Rows[0]["Field3DisplayName"] + " :" + dtMultipass.Rows[0]["Field3Value"].ToString() + ";";
        //                    if (columnname == "Field4")
        //                        smsBody += dtMultipass.Rows[0]["Field4DisplayName"] + " :" + dtMultipass.Rows[0]["Field4Value"].ToString() + ";";
        //                    if (columnname == "Field5")
        //                        smsBody += dtMultipass.Rows[0]["Field5DisplayName"] + " :" + dtMultipass.Rows[0]["Field5Value"].ToString() + ";";
        //                    if (columnname == "Field6")
        //                        smsBody += dtMultipass.Rows[0]["Field6DisplayName"] + " :" + dtMultipass.Rows[0]["Field6Value"].ToString() + ";";
        //                    if (columnname == "Field7")
        //                        smsBody += dtMultipass.Rows[0]["Field7DisplayName"] + " :" + dtMultipass.Rows[0]["Field7Value"].ToString() + ";";
        //                    if (columnname == "Field8")
        //                        smsBody += dtMultipass.Rows[0]["Field8DisplayName"] + " :" + dtMultipass.Rows[0]["Field8Value"].ToString() + ";";
        //                    if (columnname == "Field9")
        //                        smsBody += dtMultipass.Rows[0]["Field9DisplayName"] + " :" + dtMultipass.Rows[0]["Field9Value"].ToString() + ";";
        //                    if (columnname == "Field10")
        //                        smsBody += dtMultipass.Rows[0]["Field10DisplayName"] + " :" + dtMultipass.Rows[0]["Field10Value"].ToString() + ";";
        //                    if (columnname == "Field11")
        //                        smsBody += dtMultipass.Rows[0]["Field11DisplayName"] + " :" + dtMultipass.Rows[0]["Field11Value"].ToString() + ";";
        //                    if (columnname == "Field12")
        //                        smsBody += dtMultipass.Rows[0]["Field12DisplayName"] + " :" + dtMultipass.Rows[0]["Field12Value"].ToString() + ";";
        //                    if (columnname == "Field13")
        //                        smsBody += dtMultipass.Rows[0]["Field13DisplayName"] + " :" + dtMultipass.Rows[0]["Field13Value"].ToString() + ";";
        //                    if (columnname == "Field14")
        //                        smsBody += dtMultipass.Rows[0]["Field14DisplayName"] + " :" + dtMultipass.Rows[0]["Field14Value"].ToString() + ";";
        //                    if (columnname == "Field15")
        //                        smsBody += dtMultipass.Rows[0]["Field15DisplayName"] + " :" + dtMultipass.Rows[0]["Field15Value"].ToString() + ";";
        //                    if (columnname == "Field16")
        //                        smsBody += dtMultipass.Rows[0]["Field16DisplayName"] + " :" + dtMultipass.Rows[0]["Field16Value"].ToString() + ";";
        //                    if (columnname == "Field17")
        //                        smsBody += dtMultipass.Rows[0]["Field17DisplayName"] + " :" + dtMultipass.Rows[0]["Field17Value"].ToString() + ";";
        //                    if (columnname == "Field18")
        //                        smsBody += dtMultipass.Rows[0]["Field18DisplayName"] + " :" + dtMultipass.Rows[0]["Field18Value"].ToString() + ";";
        //                    if (columnname == "Field19")
        //                        smsBody += dtMultipass.Rows[0]["Field19DisplayName"] + " :" + dtMultipass.Rows[0]["Field19Value"].ToString() + ";";
        //                    if (columnname == "Field20")
        //                        smsBody += dtMultipass.Rows[0]["Field20DisplayName"] + " :" + dtMultipass.Rows[0]["Field20Value"].ToString() + ";";
        //                }
        //            }
        //        }
        //    }
        //    return smsBody;
        //}

        public static string GetSMSEnd(int SMSSetupId)
        {
            using (weighbridgeEntities context = new weighbridgeEntities(System.Windows.Application.Current.Properties["EntityConnectionString"].ToString()))
            {
                string smsEnd = context.SMSSetups.Where(x => x.SMSSetupID == SMSSetupId).Select(x => x.SMSEnd).FirstOrDefault();
                return smsEnd;
            }
        }

        public static string GetSMSBody(TransactionData ObjTData, int SMSSetupId)
        {
            string SMSstring = "";

            if (ObjTData.Status == "Complete")
            {
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
                    GrossTime = objtranDetail.GrossTime.Value.ToString("dd/MM/yyyy HH:mm:tt");
                    TareTime = objtranDetail.TareTime.Value.ToString("dd/MM/yyyy HH:mm:tt");
                }
                else if (ObjTData.TransactionMode == TransactionMode.Double.ToString())
                {
                    objtranDetail = ObjTData.TransactionDetails.FirstOrDefault(x => x.SequenceNo == 1);
                    GrossWeight = objtranDetail.GrossWeight.Value.ToString(WeighingSolution.DataLayer.Constant.DecimalFormat);
                    TareWeight = objtranDetail.TareWeight.Value.ToString(WeighingSolution.DataLayer.Constant.DecimalFormat);
                    NetWeight = objtranDetail.NetWeight.Value.ToString(WeighingSolution.DataLayer.Constant.DecimalFormat);
                    GrossTime = objtranDetail.GrossTime.Value.ToString("dd/MM/yyyy HH:mm:tt");
                    TareTime = objtranDetail.TareTime.Value.ToString("dd/MM/yyyy HH:mm:tt");
                }
                else
                {
                    GrossWeight = ObjTData.TransactionDetails.Max(x => x.CaptureWeight).Value.ToString(WeighingSolution.DataLayer.Constant.DecimalFormat);
                    TareWeight = ObjTData.TransactionDetails.Min(x => x.CaptureWeight).Value.ToString(WeighingSolution.DataLayer.Constant.DecimalFormat);
                    NetWeight = (Convert.ToDecimal(GrossWeight) - Convert.ToDecimal(TareWeight)).ToString(WeighingSolution.DataLayer.Constant.DecimalFormat);
                    decimal GWt = Convert.ToDecimal(GrossWeight);
                    GrossTime = ObjTData.TransactionDetails.FirstOrDefault(x => x.GrossWeight == GWt).GrossTime.Value.ToString("dd/MM/yyyy HH:mm:tt");
                    decimal Twt = Convert.ToDecimal(TareWeight);
                    TareTime = ObjTData.TransactionDetails.FirstOrDefault(x => x.TareWeight == Twt).TareTime.Value.ToString("dd/MM/yyyy HH:mm:tt");
                }
                List<SMSFieldSetting> lstSMS;
                using (weighbridgeEntities ctxsms = new weighbridgeEntities(System.Windows.Application.Current.Properties["EntityConnectionString"].ToString()))
                {
                    lstSMS = ctxsms.SMSFieldSettings.Where(x => x.SMSSetupId == SMSSetupId && x.IsSelected == true).ToList();
                }
                if (lstSMS != null && lstSMS.Count > 0)
                {
                    foreach (var item in lstSMS)
                    {
                        if (!string.IsNullOrEmpty(item.SMSDisplayName))
                        {
                            if (item.SMSFieldName == "Ticket No")
                                SMSstring = item.SMSDisplayName + ":" + ObjTData.TicketID + Environment.NewLine;
                            if (item.SMSFieldName == "Vehicle No")
                            {
                                if (ObjTData.Vehicle != null && !string.IsNullOrEmpty(ObjTData.Vehicle.VehicleNumber))
                                    SMSstring += item.SMSDisplayName + ":" + ObjTData.Vehicle.VehicleNumber + Environment.NewLine;
                            }
                            if (item.SMSFieldName == "Field1")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue1))
                                    SMSstring += "*" + item.SMSFieldName + "-" + ObjTData.CustomValue1;
                            }
                            if (item.SMSFieldName == "Field2")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue2))
                                    SMSstring += "*" + item.SMSFieldName + "-" + ObjTData.CustomValue2;
                            }
                            if (item.SMSFieldName == "Field3")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue3))
                                    SMSstring += "*" + item.SMSFieldName + "-" + ObjTData.CustomValue3;
                            }
                            if (item.SMSFieldName == "Field4")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue4))
                                    SMSstring += "*" + item.SMSFieldName + "-" + ObjTData.CustomValue4;
                            }
                            if (item.SMSFieldName == "Field5")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue5))
                                    SMSstring += "*" + item.SMSFieldName + "-" + ObjTData.CustomValue5;
                            }
                            if (item.SMSFieldName == "Field6")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue6))
                                    SMSstring += "*" + item.SMSFieldName + "-" + ObjTData.CustomValue6;
                            }
                            if (item.SMSFieldName == "Field7")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue7))
                                    SMSstring += "*" + item.SMSFieldName + "-" + ObjTData.CustomValue7;
                            }
                            if (item.SMSFieldName == "Field8")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue8))
                                    SMSstring += "*" + item.SMSFieldName + "-" + ObjTData.CustomValue8;
                            }

                            if (item.SMSFieldName == "Field9")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue9))
                                    SMSstring += "*" + item.SMSFieldName + "-" + ObjTData.CustomValue9;
                            }
                            if (item.SMSFieldName == "Field10")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue10))
                                    SMSstring += "*" + item.SMSFieldName + "-" + ObjTData.CustomValue10;
                            }
                            if (item.SMSFieldName == "Field11")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue11))
                                    SMSstring += "*" + item.SMSFieldName + "-" + ObjTData.CustomValue11;
                            }
                            if (item.SMSFieldName == "Field12")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue12))
                                    SMSstring += "*" + item.SMSFieldName + "-" + ObjTData.CustomValue12;
                            }
                            if (item.SMSFieldName == "Field13")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue13))
                                    SMSstring += "*" + item.SMSFieldName + "-" + ObjTData.CustomValue13;
                            }
                            if (item.SMSFieldName == "Field14")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue14))
                                    SMSstring += "*" + item.SMSFieldName + "-" + ObjTData.CustomValue14;
                            }
                            if (item.SMSFieldName == "Field15")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue15))
                                    SMSstring += "*" + item.SMSFieldName + "-" + ObjTData.CustomValue15;
                            }
                            if (item.SMSFieldName == "Field16")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue16))
                                    SMSstring += "*" + item.SMSFieldName + "-" + ObjTData.CustomValue16;
                            }
                            if (item.SMSFieldName == "Field17")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue17))
                                    SMSstring += "*" + item.SMSFieldName + "-" + ObjTData.CustomValue17;
                            }
                            if (item.SMSFieldName == "Field18")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue18))
                                    SMSstring += "*" + item.SMSFieldName + "-" + ObjTData.CustomValue18;
                            }
                            if (item.SMSFieldName == "Field19")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue19))
                                    SMSstring += "*" + item.SMSFieldName + "-" + ObjTData.CustomValue19;
                            }
                            if (item.SMSFieldName == "Field20")
                            {
                                if (!string.IsNullOrEmpty(ObjTData.CustomValue20))
                                    SMSstring += "*" + item.SMSFieldName + "-" + ObjTData.CustomValue20;
                            }

                            if (item.SMSFieldName == "Gross Time")
                                SMSstring += item.SMSDisplayName + ":" + GrossTime + Environment.NewLine;
                            if (item.SMSFieldName == "Gross Weight")
                                SMSstring += item.SMSDisplayName + ":" + GrossWeight + Environment.NewLine;
                            if (item.SMSFieldName == "Tare Weight")
                                SMSstring += item.SMSDisplayName + ":" + TareWeight + Environment.NewLine;
                            if (item.SMSFieldName == "Net Weight")
                                SMSstring += item.SMSDisplayName + ":" + NetWeight + Environment.NewLine;
                            if (item.SMSFieldName == "Account Name")
                            {
                                if (ObjTData.Account != null && !string.IsNullOrEmpty(ObjTData.Account.CompanyName))
                                    SMSstring += item.SMSDisplayName + ":" + ObjTData.Account.CompanyName + Environment.NewLine;
                            }
                            //if (item.SMSFieldName == "Token Value")
                            //{
                            //    if (ObjTData.RFToken != null && !string.IsNullOrEmpty(ObjTData.RFToken.TokenValue))
                            //        SMSstring += "," + item.SMSDisplayName + "-" + ObjTData.RFToken.TokenValue;
                            //}

                            if (item.SMSFieldName == "Tare Time")
                                SMSstring += item.SMSDisplayName + ":" + TareTime + Environment.NewLine;

                            if (item.SMSFieldName == "Charges" && ObjTData.Charges != null)
                                SMSstring += item.SMSDisplayName + ":" + ObjTData.Charges + Environment.NewLine;
                            if (item.SMSFieldName == "Product" && ObjTData.Charges != null)
                                SMSstring += item.SMSDisplayName + ":" + ObjTData.TransactionProducts + Environment.NewLine;


                        }
                    }
                    if (SMSstring != "")
                    {
                        return SMSstring;
                    }
                }
            }
            return SMSstring;
        }

        public static char GetCharFromKey(Key key)
        {
            char ch = ' ';

            int virtualKey = KeyInterop.VirtualKeyFromKey(key);
            byte[] keyboardState = new byte[256];
            GetKeyboardState(keyboardState);

            uint scanCode = MapVirtualKey((uint)virtualKey, MapType.MAPVK_VK_TO_VSC);
            StringBuilder stringBuilder = new StringBuilder(2);

            int result = ToUnicode((uint)virtualKey, scanCode, keyboardState, stringBuilder, stringBuilder.Capacity, 0);
            switch (result)
            {
                case -1:
                    break;
                case 0:
                    break;
                case 1:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
                default:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
            }
            return ch;
        }

        public enum MapType : uint
        {
            MAPVK_VK_TO_VSC = 0x0,
            MAPVK_VSC_TO_VK = 0x1,
            MAPVK_VK_TO_CHAR = 0x2,
            MAPVK_VSC_TO_VK_EX = 0x3,
        }

        [DllImport("user32.dll")]
        public static extern int ToUnicode(
            uint wVirtKey,
            uint wScanCode,
            byte[] lpKeyState,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)]
            StringBuilder pwszBuff,
            int cchBuff,
            uint wFlags);

        [DllImport("user32.dll")]
        public static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, MapType uMapType);

    }
    public class GenericSorter<T>
    {
        public static IEnumerable<T> Sort(IEnumerable<T> source, string sortBy, string sortDirection)
        {
            System.Linq.Expressions.ParameterExpression param = System.Linq.Expressions.Expression.Parameter(typeof(T), "item");
            Expression<Func<T, object>> sortExpression = System.Linq.Expressions.Expression.Lambda<Func<T, object>>(System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Property(param, sortBy), typeof(object)), param);

            switch (sortDirection.ToLower())
            {
                case "ascending":
                    return source.AsQueryable().OrderBy(sortExpression);
                default:
                    return source.AsQueryable().OrderByDescending(sortExpression);
            }
        }
    }

    public class DirectoryImageCleaner
    {
        public static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp" };

        public static void CleanDirectoryImage(string path)
        {
            long maxSizeBytes = 5L * 1024 * 1024 * 1024; // 5 GB

            if (!Directory.Exists(path))
                return;

            while (true)
            {
                long dirSize = GetDirectorySizeSafe(path);

                Console.WriteLine($"Current Size: {FormatSize(dirSize)}");

                if (dirSize <= maxSizeBytes)
                    break;

                var filesToDelete = Directory
                    .EnumerateFiles(path, "*", SearchOption.AllDirectories)
                    .Where(f => ImageExtensions.Contains(Path.GetExtension(f).ToLower()))
                    .Select(f => new FileInfo(f))
                    .OrderBy(f => f.LastWriteTimeUtc)
                    .Take(100)
                    .ToList();

                if (!filesToDelete.Any())
                    break;

                foreach (var file in filesToDelete)
                {
                    try
                    {
                        file.Delete();
                        Console.WriteLine($"Deleted: {file.FullName}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Skip: {file.FullName} ({ex.Message})");
                    }
                }               
            }
        }

        public static long GetDirectorySizeSafe(string path)
        {
            long size = 0;

            try
            {
                foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        size += new FileInfo(file).Length;
                    }
                    catch { }
                }
            }
            catch { }

            return size;
        }

        public static string FormatSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }
    }    
}
