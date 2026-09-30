using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System.Net;

namespace InstallerUI.Helpers
{
    public class InternetConnectionValidator
    {
        public static bool IsInternetConnectionAvailabe(Engine engine)
        {
            return InternetConnectionValidator.TestHeadRequest("http://www.google.com", engine);
        }
        private static bool TestHeadRequest(string url, Engine engine)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Timeout = 10000;
            httpWebRequest.Method = "HEAD";
            bool result;
            try
            {
                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    if (httpWebResponse.StatusCode != HttpStatusCode.OK)
                    {
                        engine.Log(LogLevel.Standard, "URL not reachable: " + url);
                        result = false;
                    }
                    else
                    {
                        engine.Log(LogLevel.Standard, "URL reachable: " + url);
                        result = true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                engine.Log(LogLevel.Standard, "SendTestHeadRequest exception: " + ex.Message);
                result = false;
            }
            return result;
        }
    }
}