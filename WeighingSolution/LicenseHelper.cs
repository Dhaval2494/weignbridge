using Newtonsoft.Json;
using Portable.Licensing;
using Portable.Licensing.Validation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WeighingSolution.DataLayer;

namespace WeighingSolution
{
    public class LicenseHelper
    {
        public static License LicenseInfo = null;
        public async static Task<string> ValidateLicense(string licenseKey = "")
        {
            if (String.IsNullOrEmpty(licenseKey))
            {
                try
                {
                    string licensepath = Constant.CustomSettingsPath + @"\license.lic";
                    if (File.Exists(licensepath))
                    {
                        licenseKey = File.ReadAllText(licensepath);
                    }
                    else
                    {
                        return "ERROR: License File not found";
                    }
                }
                catch (Exception ex)
                {
                    string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + "ERROR: Unable to read license file." + Environment.NewLine + JsonConvert.SerializeObject(ex);
                    File.AppendAllText(Constant.CustomSettingsPath + @"\LicenseError.txt", json);
                    return "ERROR: Unable to read license file. " + ex.Message;
                }
            }
            else
            {
                try
                {
                    File.AppendAllText(Constant.CustomSettingsPath + @"\LicenseInfo.txt", Environment.NewLine + WhoAmI.fingerPrintValueFile());
                }
                catch { }
            }

            string LicenseCheckResult = await Task.Run(() =>
            {
                string returnValue = "Success";
                try
                {

                    IValidationFailure failureHardware = null;
                    List<IValidationFailure> validationFailures = null;
                    string decryptedData = Common.DecryptData(licenseKey);
                    if (!string.IsNullOrEmpty(decryptedData))
                    {
                        LicenseInfo = License.Load(decryptedData);
                        validationFailures = LicenseInfo.Validate().ExpirationDate()
                                            .And()
                                            .AssertThat(lic => lic.ProductFeatures.Get("HardwareID").Contains(WhoAmI.fingerPrintValue()), failureHardware).When(lic => lic.ProductFeatures.Contains("HardwareID"))
                                            .And()
                                            .Signature(Common.PublicKey)
                                            .AssertValidLicense().ToList();

                        if (validationFailures.Any())
                        {
                            returnValue = "ERROR: \n";
                            foreach (var failure in validationFailures)
                                returnValue += failure.Message + "\n";
                        }
                    }
                    else
                        returnValue = "ERROR: Processing license key.";
                }
                catch (Exception ex)
                {
                    string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + "ERROR: Processing license key." + Environment.NewLine + JsonConvert.SerializeObject(ex);
                    File.AppendAllText(Constant.CustomSettingsPath + @"\LicenseError.txt", json);
                    returnValue = "ERROR: Processing license key. " + ex.Message;
                }

                if (returnValue != "Success")
                {
                    string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + returnValue;
                    File.AppendAllText(Constant.CustomSettingsPath + @"\LicenseError.txt", json);
                }

                return returnValue;
            });
            return LicenseCheckResult;
        }

        public static void CleanupData()
        {
            try
            {
                if (File.Exists(Constant.CustomSettingsPath + @"\Configuration.dat"))
                {
                    DBSettingData lgobjDBSetting = new DBSettingData();
                    string Data = File.ReadAllText(Constant.CustomSettingsPath + @"\Configuration.dat");
                    if (!string.IsNullOrEmpty(Data))
                    {
                        lgobjDBSetting = Newtonsoft.Json.JsonConvert.DeserializeObject<DBSettingData>(Common.DecryptData(Data));
                        if (lgobjDBSetting != null)
                        {
                            string Datastring;
                            if (lgobjDBSetting.Port > 0)
                                Datastring = "Data Source=" + lgobjDBSetting.Server + "," + lgobjDBSetting.Port + ";Initial Catalog=" + lgobjDBSetting.DBName + ";user id=" + lgobjDBSetting.UserId + ";password=" + lgobjDBSetting.password + ";";
                            else
                                Datastring = "Data Source=" + lgobjDBSetting.Server + ";Initial Catalog=" + lgobjDBSetting.DBName + ";user id=" + lgobjDBSetting.UserId + ";password=" + lgobjDBSetting.password + ";";

                            using (var conn = new SqlConnection(Datastring))
                            {
                                conn.Open();
                                if (conn.State == ConnectionState.Open)
                                {
                                    conn.ChangeDatabase("master");
                                    var query1 = String.Format("ALTER DATABASE [{0}] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE", lgobjDBSetting.DBName);
                                    using (var command1 = new SqlCommand(query1, conn))
                                    {
                                        command1.ExecuteNonQuery();
                                    }
                                    var query2 = String.Format("DROP DATABASE [{0}]", lgobjDBSetting.DBName);
                                    using (var command2 = new SqlCommand(query2, conn))
                                    {
                                        command2.ExecuteNonQuery();
                                    }
                                    conn.Close();

                                    System.IO.DirectoryInfo di = new DirectoryInfo(Constant.CustomImagesPath + @"\TransactionImages\");
                                    foreach (FileInfo file in di.GetFiles())
                                    {
                                        file.Delete();
                                    }


                                    System.IO.DirectoryInfo diSettings = new DirectoryInfo(Constant.CustomSettingsPath);
                                    foreach (FileInfo file in diSettings.GetFiles())
                                    {
                                        file.Delete();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.logErrorInDB(ex.Message, ex);
            }
        }
    }
}
