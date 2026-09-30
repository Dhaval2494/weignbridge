using Newtonsoft.Json;
using RestSharp;
using System;
using System.Configuration;
using WeighingSolution.Helpers;

namespace WeighingSolution.BindingModel
{
    public class Whatsapp_API
    {
        public static string WbCheckWeighments(GettransactionResponse gettransactionResponse)
        {
            string Response = string.Empty;
            try
            {
                string wbKey = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["wbKey"]) ? ConfigurationManager.AppSettings["wbKey"] : "";
                string ApiBaseUrl = "https://wa.smsidea.com/api/v1/sendDocument";
                var client = new RestClient(ApiBaseUrl);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                string body = $@"
                     {{
                        ""key"": ""{wbKey}"",
                        ""to"": ""91{gettransactionResponse.number}"",
                        ""url"": ""data:application/pdf;base64,{gettransactionResponse.massage}"",
                        ""filename"": ""{gettransactionResponse.fileName}"",
                        ""caption"":""{gettransactionResponse.MassageContent}""
                     }}";
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Response = response.Content.ToString();
                    ApiResponse responseData = JsonConvert.DeserializeObject<ApiResponse>(response.Content.ToString());
                    if (responseData.ErrorCode == "000" && responseData.ErrorMessage == "success")
                    {
                        ToastHelper.ShowSuccess("Whatsapp Message sent successfully");
                        return responseData.ErrorCode;
                    }
                    else
                    {
                        ToastHelper.ShowError("Whatsapp Message sending Failed");
                        return string.Empty;
                    }

                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
    public class ApiResponse
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Data { get; set; }
    }
    public class GettransactionResponse
    {
        public string number { get; set; }
        public string massage { get; set; }
        public string fileName { get; set; }
        public string MassageContent { get; set; }
    }

    public class TrasactionDetail
    {
        public string number { get; set; }
        public string massage { get; set; }
        public string fileName { get; set; }
        public string MassageContent { get; set; }
    }

}
