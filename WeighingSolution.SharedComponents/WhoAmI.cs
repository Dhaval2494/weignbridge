using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WeighingSolution.DataLayer
{
    public class WhoAmI
    {
        public static void SetRegistry(string name, string value)
        {
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WeighingSolution");
            key.SetValue(name, value);
            key.Close();
        }
        public static string GetRegistry(string name)
        {
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\WeighingSolution");
            if (key != null && key.GetValue(name) != null && !string.IsNullOrEmpty(key.GetValue(name).ToString()))
            {
                return key.GetValue(name).ToString();
            }

            return string.Empty;
        }
        public static string fingerPrintValue()
        {
            //return GetHash("CPU >> " + cpuId() + "\nBIOS >> " +
            //biosId() + "\nBASE >> " + baseId() + videoId() + "\nMAC >> " + macId());
            //return cpuId();

            //string machineFingerPrint = machineFingerPrintUuid();
            //string GetH = GetHash(machineFingerPrint);
            //string GetHR = GetH.Replace("-", "").Substring(0, 10);

            string key = GetHash(machineFingerPrintUpdated()).Replace("-", "").Substring(0, 10);
            return key;
        }

        public static string fingerPrintValueFile()
        {
            string key = GetHash(machineFingerPrintUpdated()).Replace("-", "").Substring(0, 10);
            return key;
        }
        private static string GetHash(string s)
        {
            MD5 sec = new MD5CryptoServiceProvider();
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] bt = enc.GetBytes(s);
            return GetHexString(sec.ComputeHash(bt));
        }
        private static string GetHexString(byte[] bt)
        {
            string s = string.Empty;
            for (int i = 0; i < bt.Length; i++)
            {
                byte b = bt[i];
                int n, n1, n2;
                n = (int)b;
                n1 = n & 15;
                n2 = (n >> 4) & 15;
                if (n2 > 9)
                    s += ((char)(n2 - 10 + (int)'A')).ToString();
                else
                    s += n2.ToString();
                if (n1 > 9)
                    s += ((char)(n1 - 10 + (int)'A')).ToString();
                else
                    s += n1.ToString();
                if ((i + 1) != bt.Length && (i + 1) % 2 == 0) s += "-";
            }
            return s;
        }

        #region Original Device ID Getting Code
        //Return a hardware identifier
        private static string identifier
        (string wmiClass, string wmiProperty, string wmiMustBeTrue)
        {
            string result = "";
            System.Management.ManagementClass mc =
        new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                if (mo[wmiMustBeTrue].ToString() == "True")
                {
                    //Only get the first one
                    if (result == "")
                    {
                        try
                        {
                            result = mo[wmiProperty].ToString();
                            break;
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return result;
        }
        //Return a hardware identifier
        private static string identifier(string wmiClass, string wmiProperty)
        {
            string result = "";
            System.Management.ManagementClass mc =
        new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                //Only get the first one
                if (result == "")
                {
                    try
                    {
                        if (mo[wmiProperty] != null)
                        {
                            result = mo[wmiProperty].ToString();
                            break;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            return result;
        }

        private static string machineFingerPrintUuid()
        {
            string retVal = string.Empty;
            try
            {
                retVal = identifier("Win32_ComputerSystemProduct", "UUID");
            }
            catch
            {
                retVal = "DDF42F45-1B2B-EB11-8108-3024A99EC2BE";
            }

            return retVal;
        }

        private static string machineFingerPrintUpdated()
        {
            string retVal = "";
            string UUID = machineFingerPrintUuid();

            if (string.IsNullOrEmpty(GetRegistry("UniqueId")))
                SetRegistry("UniqueId", UUID);

            string DateTikes = CommonEncription.EncryptData(DateTime.Now.Ticks.ToString());

            string InstallationFile = Constant.CustomSettingsPath + @"\Installation.txt";

            string reginstallation = GetRegistry("Installation");

            string _DateTikes = string.Empty;
            try
            {
                if (File.Exists(InstallationFile))
                    _DateTikes = File.ReadAllText(InstallationFile);
            }
            catch { }

            if (string.IsNullOrEmpty(reginstallation))
            {
                if (string.IsNullOrEmpty(_DateTikes))
                {
                    SetRegistry("Installation", DateTikes);
                    File.WriteAllText(InstallationFile, DateTikes);
                }
                else
                {
                    SetRegistry("Installation", _DateTikes);
                    DateTikes = _DateTikes;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(_DateTikes))
                {
                    DateTikes = reginstallation;
                    File.WriteAllText(InstallationFile, DateTikes);
                }
                else
                {
                    DateTikes = _DateTikes;
                }
            }

            retVal = UUID + DateTikes;

            //string retVal = identifier("Win32_Processor", "UniqueId");
            //retVal += identifier("Win32_Processor", "ProcessorId");
            //retVal += identifier("Win32_Processor", "Revision");
            //retVal += identifier("Win32_Processor", "Name");
            //retVal += identifier("Win32_Processor", "MaxClockSpeed");
            //retVal += identifier("Win32_BaseBoard", "SerialNumber");
            return retVal;
        }

        #endregion
    }

}
