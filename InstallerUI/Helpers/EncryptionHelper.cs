using System;
using System.Security.Cryptography;
using System.Text;

namespace InstallerUI.Helpers
{
    class EncryptionHelper
    {
        private static string EncKey = "JiBWUOES5Pau4sfm0hqgHUbIoZrlyBKG4sE8SzP2zNM=";
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
