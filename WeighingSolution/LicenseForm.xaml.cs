using MahApps.Metro.Controls;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using WeighingSolution.BindingModel;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;

namespace WeighingSolution
{
    /// <summary>
    /// Interaction logic for LicenseForm.xaml
    /// </summary>
    public partial class LicenseForm : MetroWindow
    {
        public string OTP = string.Empty;
        public LicenseForm()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            ToastHelper.InitToaster();
            lblHardwareID.Text = WhoAmI.fingerPrintValue();
            Application.Current.MainWindow = this;
        }


        private void btnGetOTP_Click(object sender, RoutedEventArgs e)
        {
            Whatsapp();
            SubmitBTN.IsEnabled = true;
        }


        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCompanyName.Text) ||
                string.IsNullOrWhiteSpace(txtSiteAddress.Text) ||
                string.IsNullOrWhiteSpace(txtContactPerson.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtContactNo.Text) ||
                string.IsNullOrWhiteSpace(txtSalesPerson.Text) ||
                cmbSoftwareType.SelectedItem == null)
            {
                ToastHelper.ShowError("All Fields are Mandatory");
                return;
            }
            else if (!IsValidEmail(txtEmail.Text))
            {
                ToastHelper.ShowError("Please enter a valid Email address.");
                return;
            }
            else if (string.IsNullOrEmpty(txtOTP.Text))
            {
                ToastHelper.ShowError("Please enter OTP.");
                return;
            }

            //create request object for adding details

            var request = new LicenseRequest
            {
                MachineKey = lblHardwareID.Text.Trim(),
                CompanyName = txtCompanyName.Text.Trim(),
                ContactPerson = txtContactPerson.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                ContactNo = txtContactNo.Text.Trim(),
                SiteAddress = txtSiteAddress.Text.Trim(),
                SoftwareType = (cmbSoftwareType.SelectedItem as ComboBoxItem)?.Content.ToString(),
                SalesPerson = txtSalesPerson.Text.Trim()
            };

            if (OTP == txtOTP.Text)
            {
                string licensepath = Constant.CustomSettingsPath + @"\license.lic";
                string URL = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["LicenceUri"]) ? ConfigurationManager.AppSettings["LicenceUri"] : "https://edspl.license.weighmast.com/FreeLicense.ashx";
                HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(URL);
                try
                {
                    request1.Headers.Add("HardwareID", request.MachineKey);
                    request1.Headers.Add("ClientName", request.CompanyName);
                    request1.Headers.Add("CompanyEmail", request.Email);
                    request1.Headers.Add("SightName", request.SiteAddress);
                    request1.Headers.Add("ContactPerson", request.ContactPerson);
                    request1.Headers.Add("ContactNo", request.ContactNo);
                    request1.Headers.Add("SoftwareType", request.SoftwareType);
                    request1.Headers.Add("SalesPerson", request.SalesPerson);
                    request1.Method = "POST";
                    request1.ContentLength = 0;
                    HttpWebResponse response = (HttpWebResponse)request1.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string statusDescription = "";
                        using (Stream stream = response.GetResponseStream())
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            statusDescription = reader.ReadToEnd();
                        }
                        if (statusDescription != "Weighing Solution Portal" && !statusDescription.Contains("html"))
                        {
                            if (!File.Exists(licensepath))
                            {
                                File.WriteAllText(licensepath, statusDescription);
                                Common.SetFullAccessForFile(licensepath);

                                Licensing mw = new Licensing(statusDescription);
                                mw.Show();
                                this.Close();
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + Newtonsoft.Json.JsonConvert.SerializeObject(ex);
                    File.WriteAllText(Constant.CustomSettingsPath + @"\LicenseError.txt", json);
                }
            }

            else
            {
                ToastHelper.ShowError("Please Enter Valid OTP");
                return;
            }



        }



        public void Whatsapp()
        {
            GettransactionResponse gettransaction = new GettransactionResponse();

            OTP = GenerateOTP();
            gettransaction.number = txtContactNo.Text;
            gettransaction.fileName = string.Empty;
            gettransaction.massage = string.Empty; //Base64();
            gettransaction.MassageContent = $@"Please Use this Otp in Software for registration {OTP}";
            var data = Whatsapp_API.WbCheckWeighments(gettransaction);

        }

        public string Base64()
        {
            //string custommanualPath = Path.Combine(Common.CustomPath, "FreeVersionSoftwareWorkingSOP.rtf");
            //if (File.Exists(custommanualPath))
            //{
            //    byte[] filebite = File.ReadAllBytes(custommanualPath);
            //    string base64 = Convert.ToBase64String(filebite);
            //    return base64;
            //}
            return string.Empty;
        }

        static string GenerateOTP()
        {
            // Define the length of the OTP
            int otpLength = 6;

            // Use a random number generator to create the OTP
            Random random = new Random();
            string otp = "";

            for (int i = 0; i < otpLength; i++)
            {
                // Append a random digit to the OTP
                otp += random.Next(0, 9).ToString();
            }

            return otp;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public class LicenseRequest
        {
            public string MachineKey { get; set; }
            public string CompanyName { get; set; }
            public string ContactPerson { get; set; }
            public string Email { get; set; }
            public string ContactNo { get; set; }
            public string SiteAddress { get; set; }
            public string SoftwareType { get; set; }
            public string SalesPerson { get; set; }
        }

        private void Skipbtn_Click(object sender, RoutedEventArgs e)
        {
            Licensing licenseForm = new Licensing();
            licenseForm.Show();
            this.Close();

        }
    }
}