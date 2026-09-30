using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using WeighingSolution.DataLayer;

namespace WeighingSolution.SharedComponents
{
    public class SMS
    {
        public SMS()
        {

        }

        public static ObservableCollection<SMSAPIDATA> ReadSmsApiConfig()
        {
            ObservableCollection<SMSAPIDATA> Providers = new ObservableCollection<SMSAPIDATA>();
            try
            {
                string JsonFilePath = Constant.CustomSettingsPath + $@"\SmsApiConfig.json";

                if (File.Exists(JsonFilePath))
                {
                    string json = File.ReadAllText(JsonFilePath);
                    if (!string.IsNullOrEmpty(json))
                        Providers = JsonConvert.DeserializeObject<ObservableCollection<SMSAPIDATA>>(json);

                    return Providers;
                }
            }
            catch (Exception ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.CustomSettingsPath + @"\SmsApiConfigError.txt", json);
            }
            return Providers;
        }

        public static void WriteSmsApiConfig(string UpdateJson)
        {
            try
            {
                string JsonPath = Constant.CustomSettingsPath + $@"\SmsApiConfig.json";
                File.WriteAllText(JsonPath, UpdateJson);
            }
            catch (Exception ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.CustomSettingsPath + @"\SmsApiConfigError.txt", json);
            }
        }

        public string SendSms(string TOMobileNo, string Message, string ProviderName, string ApiUrl = null)
        {
            try
            {
                if (string.IsNullOrEmpty(ApiUrl))
                {
                    ObservableCollection<SMSAPIDATA> items = ReadSmsApiConfig();

                    if (items == null)
                        return "Setting File Not Configure";

                    if (items.Count < 1 || items.Where(p => p.ProviderName == ProviderName).Count() < 1)
                        return "Setting File Not Configure";
                    if (string.IsNullOrEmpty(ApiUrl))
                        ApiUrl = items.Where(p => p.ProviderName == ProviderName).FirstOrDefault().ApiUrl;
                }

                return Send_API_SMS(ApiUrl, TOMobileNo, Message);
            }
            catch (Exception ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.PluginFileLocation + @"\Error.txt", json);
                return "Error in Sending SMS." +
                    "";
            }


        }


        public string SendGSMSMS(string ReceiverMobileNo, string Message, string portName, int baudRate, System.IO.Ports.Parity parity, int dataBits, System.IO.Ports.StopBits stopBits)
        {
            try
            {
                System.IO.Ports.SerialPort e = new System.IO.Ports.SerialPort(portName, baudRate, parity, dataBits, stopBits);
                e.Handshake = System.IO.Ports.Handshake.None;
                e.Open();
                e.WriteLine("AT+CMGF=1");
                Console.WriteLine(e.ReadLine() + "\n");
                e.WriteLine(string.Format(@"AT+CMGS=""{0}""", ReceiverMobileNo));
                Console.WriteLine(e.ReadLine() + "\n");
                string sms = Message + (char)26;
                Console.WriteLine(e.ReadLine() + "\n");
                e.WriteLine(sms);
                System.Threading.Thread.Sleep(3000);
                Console.WriteLine(e.ReadLine() + "\n");
                e.Close();
                return "SMS send successfully";
            }
            catch (Exception ex)
            {
                _ = ex.Message;
                return "Error in Sending SMS";
            }
        }

        public System.IO.Ports.Parity GetParity(string parity)
        {
            switch (parity)
            {
                case "None":
                    return System.IO.Ports.Parity.None;
                case "Odd":
                    return System.IO.Ports.Parity.Odd;
                case "Even":
                    return System.IO.Ports.Parity.Even;
                case "Mark":
                    return System.IO.Ports.Parity.Mark;
                case "Space":
                    return System.IO.Ports.Parity.Space;
                default:
                    return System.IO.Ports.Parity.None;
            }
        }

        public System.IO.Ports.StopBits GetStopBits(string parity)
        {
            switch (parity)
            {
                case "None":
                    return System.IO.Ports.StopBits.None;
                case "":
                    return System.IO.Ports.StopBits.None;
                case "One":
                    return System.IO.Ports.StopBits.One;
                case "Two":
                    return System.IO.Ports.StopBits.Two;
                case "OnePointFive":
                    return System.IO.Ports.StopBits.OnePointFive;
                default:
                    return System.IO.Ports.StopBits.One;
            }
        }


        #region "Helper For Send SMS"
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ApiUrl">ApiUrl</param>
        /// <param name="ToNumber">Receiver Mobile Number</param>
        /// <param name="Message">Message</param>
        /// <returns></returns>
        private string Send_API_SMS(string ApiUrl, string ToNumber, string Message)
        {
            string Url = string.Format(ApiUrl, ToNumber, Message);

            Uri targetUri = new Uri(Url);
            HttpWebRequest webRequest = (HttpWebRequest)System.Net.HttpWebRequest.Create(targetUri);
            webRequest.Method = WebRequestMethods.Http.Get;
            try
            {
                string webResponse = string.Empty;
                using (HttpWebResponse getresponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    using (StreamReader reader = new
                    StreamReader(getresponse.GetResponseStream()))
                    {
                        webResponse = reader.ReadToEnd();
                        reader.Close();
                    }
                    getresponse.Close();
                }
                return webResponse;
            }
            catch (System.Net.WebException ex)
            {
                _ = ex.Message;
                return "Request-Timeout";
            }
            catch (Exception ex)
            {
                _ = ex.Message;
                return "error";
            }
            finally { webRequest.Abort(); }
        }

        #endregion

        #region "Get Template"

        #endregion
    }

    public class SMSAPIDATA
    {
        public string SMSType { get; set; }
        public int ProviderId { get; set; }
        public string ProviderName { get; set; }
        public string ApiUrl { get; set; }
        public string ComPort { get; set; }
        public string BaudRate { get; set; }
        public string Parity { get; set; }
        public string DataBits { get; set; }
        public string StopBits { get; set; }
        public string HandShack { get; set; }
    }

    public enum SMSType
    {
        API = 1,
        GSM = 2
    }
}


