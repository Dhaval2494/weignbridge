using System;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EmailService
{
    public class DBSettingData
    {
        private bool _InstallDB;
        private string _AttachDB;
        private string _Server;
        private string _DBName;
        private string _UserId;
        private string _password;
        private int _Port;
        public bool InstallDB
        {
            get { return _InstallDB; }
            set { _InstallDB = value; }
        }
        public string Server
        {
            get { return _Server; }
            set { _Server = value; }
        }

        public string DBName
        {
            get { return _DBName; }
            set { _DBName = value; }
        }
        public string AttachDB
        {
            get { return _AttachDB; }
            set { _AttachDB = value; }
        }
        public string UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }
        public string password
        {
            get { return _password; }
            set { _password = value; }
        }
        public int Port
        {
            get { return _Port; }
            set { _Port = value; }
        }
    }
    public class EmailServiceConstant
    {
        private static string EncKey = "JiBWUOES5Pau4sfm0hqgHUbIoZrlyBKG4sE8SzP2zNM=";
        public static string CustomSettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\WeighBridgeSolution\CustomSettings";

        private static string _emailserviceconnectionstring;

        public static string EmailServiceConnectionString
        {
            get
            {
                if (String.IsNullOrEmpty(_emailserviceconnectionstring))
                {
                    DBSettingData objDBSetting = new DBSettingData();
                    if (File.Exists(CustomSettingsPath + @"\Configuration.dat"))
                    {
                        string Data = File.ReadAllText(CustomSettingsPath + @"\Configuration.dat");
                        if (!string.IsNullOrEmpty(Data))
                        {
                            string decryptedData = DecryptData(Data);
                            if (!string.IsNullOrEmpty(decryptedData))
                            {
                                objDBSetting = Newtonsoft.Json.JsonConvert.DeserializeObject<DBSettingData>(decryptedData);
                                if (objDBSetting != null)
                                {
                                    if (objDBSetting.Port > 0)
                                        _emailserviceconnectionstring = "Data Source=" + objDBSetting.Server + "," + objDBSetting.Port + ";Initial Catalog=" + objDBSetting.DBName + ";user id=" + objDBSetting.UserId + ";password=" + objDBSetting.password + ";";
                                    else
                                        _emailserviceconnectionstring = "Data Source=" + objDBSetting.Server + ";Initial Catalog=" + objDBSetting.DBName + ";user id=" + objDBSetting.UserId + ";password=" + objDBSetting.password + ";";
                                }
                            }
                        }
                    }
                }
                return _emailserviceconnectionstring;
            }
            set { _emailserviceconnectionstring = value; }
        }


        private static string _emailserviceentityconnectionstring;
        public static string EmailServiceEntityConnectionString
        {
            get
            {
                if (String.IsNullOrEmpty(_emailserviceentityconnectionstring))
                {
                    Configuration myDllConfig = ConfigurationManager.OpenExeConfiguration(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\EmailService.exe");
                    // Get the appSettings section
                    ConnectionStringsSection Constring = myDllConfig.GetSection("connectionStrings") as ConnectionStringsSection;
                    string Constr = Constring.ConnectionStrings["Endel_weighbridgeEntities"].ConnectionString;
                    EntityConnectionStringBuilder entityStringBuilder = new EntityConnectionStringBuilder(Constr);
                    entityStringBuilder.ProviderConnectionString = EmailServiceConnectionString;
                    _emailserviceentityconnectionstring = entityStringBuilder.ToString();
                }
                return _emailserviceentityconnectionstring;
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

    }
}
