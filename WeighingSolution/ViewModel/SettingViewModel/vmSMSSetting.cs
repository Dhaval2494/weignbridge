using MahApps.Metro.SimpleChildWindow;
using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;
using WeighingSolution.SharedComponents;

namespace WeighingSolution.ViewModel
{
    public class VmSMSSetting : NotifyDataErrorInfoBase, IDisposable
    {
        private DataRepository dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));

        public ObservableCollection<ApplicationSetting> SMSSetting;
        public ChildWindow ChildWindow;

        public VmSMSSetting(ChildWindow childwindow)
        {
            try
            {
                Providers = SMS.ReadSmsApiConfig();

                ChildWindow = childwindow;
                SMSSetting = new ObservableCollection<ApplicationSetting>(dataRepository.Context.ApplicationSettings.Where(x => x.ModuleName == "SMSSetting").ToList());
                SendTestSMS = new DelegateCommand(OnSendTestSMSExecute, OnSendTestSMSCanExecute);
                SaveSettingCommand = new DelegateCommand(OnSettingSaveExecute, OnSettingSaveCanExecute);
                SaveAPISettingCommand = new DelegateCommand(OnJsonAPISettingSaveExecute, OnJsonAPISettingSaveCanExecute);
                SaveGSMSettingCommand = new DelegateCommand(OnJsonGSMSettingSaveExecute, OnJsonGSMSettingSaveCanExecute);
                this.PropertyChanged -= vmSMSSetting_PropertyChanged;
                this.PropertyChanged += vmSMSSetting_PropertyChanged;


                if (Providers.Count > 0)
                {
                    JsonProvider = Providers.FirstOrDefault().ProviderName;
                    JsonApiUrl = Providers.FirstOrDefault().ApiUrl;
                    JsonComPort = Providers.FirstOrDefault().ComPort;
                    JsonBaudRate = Providers.FirstOrDefault().BaudRate;
                    JsonParity = Providers.FirstOrDefault().Parity;
                    JsonStopBits = Providers.FirstOrDefault().StopBits;
                    JsonDataBits = Providers.FirstOrDefault().DataBits;
                    JsonHandShake = Providers.FirstOrDefault().HandShack;
                }

                SetupPorts(JsonComPort);

            }
            catch (Exception ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.PluginFileLocation + @"\Error.txt", json);
            }
        }

        #region Command and Properties Declaration       
        public DelegateCommand SendTestSMS { get; private set; }
        public DelegateCommand SaveSettingCommand { get; private set; }
        public DelegateCommand SaveAPISettingCommand { get; private set; }
        public DelegateCommand SaveGSMSettingCommand { get; private set; }

        private List<String> _comports;
        public List<String> COMPorts
        {
            get { return _comports; }
            set { _comports = value; OnPropertyChanged(); }
        }


        private bool _hasChanges;
        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                if (_hasChanges != value)
                {
                    _hasChanges = value;
                    OnPropertyChanged();
                    SaveSettingCommand.RaiseCanExecuteChanged();
                }
            }
        }



        private bool _hasJsonAPIChanges;
        public bool HasJsonAPIChanges
        {
            get { return _hasJsonAPIChanges; }
            set
            {
                if (_hasJsonAPIChanges != value)
                {
                    _hasJsonAPIChanges = value;
                    OnPropertyChanged();
                    SaveAPISettingCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool _hasJsonGSMChanges;
        public bool HasJsonGSMChanges
        {
            get { return _hasJsonGSMChanges; }
            set
            {
                if (_hasJsonGSMChanges != value)
                {
                    _hasJsonGSMChanges = value;
                    OnPropertyChanged();
                    SaveGSMSettingCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        #region Binding Property

        private string _SMSTo;
        public string SMSTo
        {
            get { return _SMSTo; }
            set
            {
                _SMSTo = value;
                ValidatePropertyInternal(nameof(SMSTo), value);
                SendTestSMS.RaiseCanExecuteChanged();
            }
        }

        private string _JsonProvider;
        public string JsonProvider
        {
            get { return _JsonProvider; }
            set
            {
                _JsonProvider = value;
                HasJsonAPIChanges = JsonProvider != value;
                ProviderName = value;
                //ValidatePropertyInternal(nameof(JsonProvider), value);
                SaveAPISettingCommand.RaiseCanExecuteChanged();
            }
        }

        private string _JsonApiUrl;
        public string JsonApiUrl
        {
            get { return _JsonApiUrl; }
            set
            {
                _JsonApiUrl = value;
                //HasJsonAPIChanges = JsonApiUrl != value;
                //ValidatePropertyInternal(nameof(JsonApiUrl), value);
                SaveAPISettingCommand.RaiseCanExecuteChanged();
            }
        }

        private string _JsonComPort;
        public string JsonComPort
        {
            get { return _JsonComPort; }
            set
            {
                HasJsonGSMChanges = _JsonComPort != value;
                _JsonComPort = value;
                ValidatePropertyInternal(nameof(JsonComPort), value);
                SaveGSMSettingCommand.RaiseCanExecuteChanged();
            }
        }

        private string _JsonBaudRate;
        public string JsonBaudRate
        {
            get { return _JsonBaudRate; }
            set
            {
                HasJsonGSMChanges = _JsonBaudRate != value;
                _JsonBaudRate = value;
                ValidatePropertyInternal(nameof(JsonBaudRate), value);
                SaveGSMSettingCommand.RaiseCanExecuteChanged();
            }
        }

        private string _JsonParity;
        public string JsonParity
        {
            get { return _JsonParity; }
            set
            {
                HasJsonGSMChanges = _JsonParity != value;
                _JsonParity = value;
                ValidatePropertyInternal(nameof(JsonParity), value);
                SaveGSMSettingCommand.RaiseCanExecuteChanged();
            }
        }

        private string _JsonDataBits;
        public string JsonDataBits
        {
            get { return _JsonDataBits; }
            set
            {
                HasJsonGSMChanges = _JsonDataBits != value;
                _JsonDataBits = value;
                ValidatePropertyInternal(nameof(JsonDataBits), value);
                SaveGSMSettingCommand.RaiseCanExecuteChanged();
            }
        }

        private string _JsonStopBits;
        public string JsonStopBits
        {
            get { return _JsonStopBits; }
            set
            {
                HasJsonGSMChanges = _JsonStopBits != value;
                _JsonStopBits = value;
                ValidatePropertyInternal(nameof(JsonStopBits), value);
                SaveGSMSettingCommand.RaiseCanExecuteChanged();
            }
        }

        private string _JsonHandShake;
        public string JsonHandShake
        {
            get { return _JsonHandShake; }
            set
            {
                HasJsonGSMChanges = _JsonHandShake != value;
                _JsonHandShake = value;
                ValidatePropertyInternal(nameof(JsonHandShake), value);
                SaveGSMSettingCommand.RaiseCanExecuteChanged();
            }
        }
        public string ProviderName
        {
            get { return SMSSetting.FirstOrDefault(x => x.SettingName == "ProviderName").SettingValue; }
            set
            {
                SMSSetting.FirstOrDefault(x => x.SettingName == "ProviderName").SettingValue = value;
                OnPropertyChanged();
                ValidatePropertyInternal("ProviderName", value);
                SaveSettingCommand.RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<SMSAPIDATA> _Providers;
        public ObservableCollection<SMSAPIDATA> Providers
        {
            get { return _Providers; }
            set { _Providers = value; OnPropertyChanged(); }
        }

        #endregion

        private void SetupPorts(string COMPort)
        {
            List<String> SystemPorts = System.IO.Ports.SerialPort.GetPortNames().ToList();
            COMPorts = new List<string>(SystemPorts);
            if (!string.IsNullOrEmpty(COMPort) && COMPorts.All(x => x != COMPort))
            {
                COMPorts.Add(COMPort);
            }
        }


        private bool OnSendTestSMSCanExecute()
        {
            return !HasErrors;
        }

        private void OnSendTestSMSExecute()
        {
            try
            {
                string TOMobileNo = SMSTo;
                string Message = "This is Test SMS";

                ObservableCollection<SMSAPIDATA> items = SMS.ReadSmsApiConfig();

                if (items == null || items.Count < 1)
                {
                    ToastHelper.ShowError("Setting File Not Configure");
                    return;
                }

                string ApiUrl = items.FirstOrDefault().ApiUrl;

                SMS _SMS = new SMS();
                string providerName = items.FirstOrDefault().ProviderName;
                if (!string.IsNullOrEmpty(providerName) && items.Select(p => p.SMSType.ToString()).FirstOrDefault() == "API")
                {
                    string Response = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine;
                    Response += _SMS.SendSms(TOMobileNo, Message, providerName, JsonApiUrl);

                    File.AppendAllText(Constant.PluginFileLocation + @"\SmsRespronse.txt", Response);
                }
                else
                {
                    SMS GsmSMS = new SMS();
                    bool IsSuccessbauderate = int.TryParse(JsonBaudRate, out int resultBaudRate);
                    int BaudRate;
                    if (IsSuccessbauderate)
                    {
                        BaudRate = resultBaudRate;
                    }
                    else
                    {
                        BaudRate = 9600;
                    }
                    bool IsSuccessDatabits = int.TryParse(JsonDataBits, out int resultDataBits);
                    int DataBits;
                    if (IsSuccessDatabits)
                    {
                        DataBits = resultDataBits;
                    }
                    else
                    {
                        DataBits = 8;
                    }
                    System.IO.Ports.Parity parity = GsmSMS.GetParity(JsonParity);
                    System.IO.Ports.StopBits stopBits = GsmSMS.GetStopBits(JsonStopBits);
                    string sendMessage = GsmSMS.SendGSMSMS(TOMobileNo, Message, JsonComPort, BaudRate, parity, DataBits, stopBits);
                    if (sendMessage == "SMS send successfully")
                    {
                        ToastHelper.ShowSuccess(sendMessage);
                    }
                    else
                    {
                        ToastHelper.ShowError(sendMessage);
                    }
                }

                ChildWindow.IsModal = false;
                ChildWindow.Close();

            }
            catch (Exception ex)
            {
                Common.logErrorInDB(ex.Message, ex);
                ToastHelper.ShowError("Fail to send SMS, kindly Configure SmsApiConfig.json ");
            }
        }

        private bool OnSettingSaveCanExecute()
        {
            return HasChanges && !HasErrors;
        }

        private async void OnSettingSaveExecute()
        {
            try
            {
                await dataRepository.SaveAsync();
                ToastHelper.ShowSuccess("SMS Setting information recorded successfully");
                ChildWindow.IsModal = false;
                ChildWindow.Close();
            }
            catch (Exception ex)
            {
                Common.logErrorInDB(ex.Message, ex);
                ToastHelper.ShowError("Failed to update SMS setting. Error: " + ex.Message);
            }
        }

        private bool OnJsonAPISettingSaveCanExecute()
        {
            if (string.IsNullOrEmpty(JsonApiUrl) || string.IsNullOrEmpty(JsonProvider))
            {
                return false;
            }
            if (!JsonApiUrl.Contains("{0}") || !JsonApiUrl.Contains("{1}"))
            {
                return false;
            }
            if (HasJsonAPIChanges && !HasErrors)
            {
                return true;
            }
            return false;
        }

        private async void OnJsonAPISettingSaveExecute()
        {
            try
            {
                if (this.IsValid())
                {
                    if (!string.IsNullOrEmpty(JsonProvider) && !string.IsNullOrEmpty(JsonApiUrl))
                    {
                        if (!JsonApiUrl.Contains("{0}") || !JsonApiUrl.Contains("{1}"))
                        {
                            await MessageBoxHelper.ShowInfoDialogAsync("SMS Setting", "Please set SMS Receiver No = {0}, Message Text = {1}");
                            return;
                        }

                        var sMSAPIDATA = new SMSAPIDATA();

                        if (Providers.Count > 0)
                        {
                            Providers.Remove(Providers.FirstOrDefault());
                            sMSAPIDATA.SMSType = SMSType.API.ToString();
                            sMSAPIDATA.ProviderId = 1;
                            sMSAPIDATA.ProviderName = JsonProvider;
                            sMSAPIDATA.ApiUrl = JsonApiUrl;

                            Providers.Add(sMSAPIDATA);
                        }
                        else
                        {
                            int ProviderId = Providers != null ? Providers.Count + 1 : 1;

                            sMSAPIDATA = new SMSAPIDATA
                            {
                                SMSType = SMSType.API.ToString(),
                                ProviderId = ProviderId,
                                ProviderName = JsonProvider,
                                ApiUrl = JsonApiUrl,
                            };

                            Providers.Add(sMSAPIDATA);
                        }

                        string UpdateJson = JsonConvert.SerializeObject(Providers);
                        SMS.WriteSmsApiConfig(UpdateJson);

                        //await dataRepository.SaveAsync();
                        ToastHelper.ShowSuccess("SMS Setting information recorded successfully");
                        ChildWindow.IsModal = false;
                        ChildWindow.Close();
                    }
                    else
                    {
                        await MessageBoxHelper.ShowInfoDialogAsync("SMS Setting", "Please fill all the required details");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.logErrorInDB(ex.Message, ex);
                ToastHelper.ShowError("Failed to update SMS setting. Error: " + ex.Message);
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.PluginFileLocation + @"\Error.txt", json);
            }
        }

        private bool OnJsonGSMSettingSaveCanExecute()
        {
            if (!string.IsNullOrEmpty(JsonComPort) && !string.IsNullOrEmpty(JsonBaudRate) && !string.IsNullOrEmpty(JsonDataBits) && JsonComPort.Contains("COM") &&
                        !string.IsNullOrEmpty(JsonParity) && !string.IsNullOrEmpty(JsonHandShake) && !string.IsNullOrEmpty(JsonStopBits))
            {
                return true;
            }
            return false;
        }

        private async void OnJsonGSMSettingSaveExecute()
        {
            try
            {
                if (this.IsValid())
                {
                    if (!string.IsNullOrEmpty(JsonComPort) && !string.IsNullOrEmpty(JsonBaudRate) && !string.IsNullOrEmpty(JsonDataBits) && JsonComPort.Contains("COM") &&
                        !string.IsNullOrEmpty(JsonParity) && !string.IsNullOrEmpty(JsonHandShake) && !string.IsNullOrEmpty(JsonStopBits))
                    {
                        var sMSAPIDATA = new SMSAPIDATA();

                        if (Providers.Count > 0)
                        {
                            Providers.Remove(Providers.FirstOrDefault());
                            sMSAPIDATA.SMSType = SMSType.GSM.ToString();
                            sMSAPIDATA.ProviderId = 1;
                            sMSAPIDATA.ComPort = JsonComPort;
                            sMSAPIDATA.BaudRate = JsonBaudRate;
                            sMSAPIDATA.DataBits = JsonDataBits;
                            sMSAPIDATA.Parity = JsonParity;
                            sMSAPIDATA.HandShack = JsonHandShake;
                            sMSAPIDATA.StopBits = JsonStopBits;

                            Providers.Add(sMSAPIDATA);
                        }
                        else
                        {
                            int ProviderId = Providers != null ? Providers.Count + 1 : 1;

                            sMSAPIDATA = new SMSAPIDATA
                            {
                                SMSType = SMSType.GSM.ToString(),
                                ProviderId = ProviderId,
                                ComPort = JsonComPort,
                                BaudRate = JsonBaudRate,
                                DataBits = JsonDataBits,
                                HandShack = JsonHandShake,
                                Parity = JsonParity,
                                StopBits = JsonStopBits
                            };

                            Providers.Add(sMSAPIDATA);
                        }

                        string UpdateJson = JsonConvert.SerializeObject(Providers);
                        SMS.WriteSmsApiConfig(UpdateJson);

                        //await dataRepository.SaveAsync();
                        ToastHelper.ShowSuccess("SMS Setting information recorded successfully");
                        ChildWindow.IsModal = false;
                        ChildWindow.Close();
                    }
                    else
                    {
                        await MessageBoxHelper.ShowInfoDialogAsync("SMS Setting", "Please fill all the required details");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.logErrorInDB(ex.Message, ex);
                ToastHelper.ShowError("Failed to update SMS setting. Error: " + ex.Message);
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.AppendAllText(Constant.PluginFileLocation + @"\Error.txt", json);
            }
        }

        void vmSMSSetting_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (!HasChanges)
            //{
            //    HasChanges = dataRepository.HasChanges();
            //}
            //SaveSettingCommand.RaiseCanExecuteChanged();

            if (!HasJsonAPIChanges)
            {
                HasJsonAPIChanges = dataRepository.HasChanges();
            }
            if (!HasJsonGSMChanges)
            {
                HasJsonGSMChanges = dataRepository.HasChanges();
            }
            SaveAPISettingCommand.RaiseCanExecuteChanged();
            SaveGSMSettingCommand.RaiseCanExecuteChanged();
        }

        public void Dispose()
        {
            dataRepository.Context.Dispose();
        }
    }
}
