using Newtonsoft.Json;
using RestSharp;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WeighingSolution.DataLayer;

namespace WeighingSolution
{
    public class AnprLicenseHelper
    {
        public static string Get_ApiBaseUrl()
        {
            try
            {
                if (string.IsNullOrEmpty(Constant.AnprApiBaseUrl))
                {
                    using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                    {
                        string ApiBaseUrl = ctx.ApplicationSettings.Where(p => p.ModuleName == "GlobalSetting").FirstOrDefault(x => x.SettingName == "AnprApiUrl").SettingValue;
                        if (ApiBaseUrl[ApiBaseUrl.Length - 1] == '/')
                            ApiBaseUrl = ApiBaseUrl.TrimEnd('/');

                        Constant.AnprApiBaseUrl = ApiBaseUrl;
                        return ApiBaseUrl;
                    }
                }
                else
                {
                    return Constant.AnprApiBaseUrl;
                }

            }
            catch
            {
                Constant.AnprApiBaseUrl = string.Empty;
                return string.Empty;
            }
        }
        public static string Get_AnprPassKey()
        {
            try
            {
                if (string.IsNullOrEmpty(Constant.AnprPasskey))
                {
                    using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                    {
                        string AnprPasskey = ctx.ApplicationSettings.Where(p => p.ModuleName == "GlobalSetting").FirstOrDefault(x => x.SettingName == "AnprPassKey").SettingValue;


                        Constant.AnprPasskey = AnprPasskey;
                        return AnprPasskey;
                    }
                }
                else
                {
                    return Constant.AnprPasskey;
                }

            }
            catch
            {
                Constant.AnprPasskey = string.Empty;
                return string.Empty;
            }
        }
        public static CheckAnprLicense_Response CheckAnprLicense()
        {
            string Response = string.Empty;
            try
            {
                string MachineKey = Get_AnprPassKey();
                string ApiBaseUrl = Get_ApiBaseUrl();

                if (string.IsNullOrEmpty(ApiBaseUrl) || string.IsNullOrEmpty(MachineKey))
                {
                    return null;
                }

                var client = new RestClient(ApiBaseUrl + $"/api/ocr/IsValidPasskey/{MachineKey}");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Content-Type", "application/json");
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Response = response.Content.ToString();
                    CheckAnprLicense_Response responseData = JsonConvert.DeserializeObject<CheckAnprLicense_Response>(response.Content.ToString());
                    return responseData;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.PluginFileLocation + $@"\CheckAnprLicense_{DateTime.Now:yyyy_MMM_dd_HH}_Error.txt", json + Environment.NewLine + "Response = " + Response);
                return null;
            }
        }
        public static Anpr_Number_API_Response GetAnprNumber(Anpr_Number_API_Request anpr_Number_API_Request)
        {
            string Response = string.Empty;
            try
            {
                string MachineKey = Get_AnprPassKey();                
                string ApiBaseUrl = Get_ApiBaseUrl();

                if (string.IsNullOrEmpty(ApiBaseUrl) || string.IsNullOrEmpty(MachineKey))
                {
                    return null;
                }

                var client = new RestClient(ApiBaseUrl + $"/api/ocr/{MachineKey}");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", anpr_Number_API_Request, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Response = response.Content.ToString();
                    Anpr_Number_API_Response responseData = JsonConvert.DeserializeObject<Anpr_Number_API_Response>(response.Content.ToString());
                    return responseData;
                }
                else
                {
                    Response = response.Content.ToString();
                    File.AppendAllText(Constant.PluginFileLocation + $@"\GetAnprNumber_{DateTime.Now:yyyy_MMM_dd_HH}_Response.txt", JsonConvert.SerializeObject(Response) + Environment.NewLine + "Response = " + Response);
                    return null;
                }
            }
            catch (Exception ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.PluginFileLocation + $@"\GetAnprNumber_{DateTime.Now:yyyy_MMM_dd_HH}_Error.txt", json + Environment.NewLine + "Response = " + Response);
                return null;
            }
        }
    }


    public class CheckAnprLicense_Response
    {
        public bool success { get; set; }
        public string LicenseKey { get; set; }
        public string message { get; set; }
    }

    public class Ocr_Request
    {
        public string apI_Region { get; set; }
        public string vehicleImage_Base64 { get; set; }
    }


    public class Anpr_Number_API_Request
    {
        public string API_Region { get; set; }
        public string VehicleNumber { get; set; }
        public string VehicleImage_Base64 { get; set; }
    }
    public class Anpr_Number_API_Response
    {
        public long Id { get; set; }
        public bool Success { get; set; }
        public string AnprNumber { get; set; }
        public string PlateImage_Base64 { get; set; }
        public string Error { get; set; }
    }

    public class Config_Response
    {
        public string data { get; set; }
        public string message { get; set; }
        public int status { get; set; }
    }
}
