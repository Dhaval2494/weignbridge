using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;
using WeighingSolution.SharedComponents;

namespace WeighingSolution.ViewModel
{
    class vmWeightBridgeManagement : DataSetupEditViewModelBase<WeightBridge>
    {
        public DelegateCommand startComReading { get; private set; }
        public DelegateCommand stopComReading { get; private set; }
        public DelegateCommand readWeight { get; private set; }
        public ICommand StartComReading { get; }
        public ICommand StopComReading { get; }
        public vmWeightBridgeManagement(int WeightBridgeID)
        {
            Id = WeightBridgeID;
            startComReading = new DelegateCommand(OnComStartReading, OnCanComStartReading);
            stopComReading = new DelegateCommand(OnComStopReading, OnCanComStopReading);
            readWeight = new DelegateCommand(OnStartReadingWeight, OnCanStartReadingWeight);
        }

        private int _FiveDigitWeight;
        public int FiveDigitWeight
        {
            get { return _FiveDigitWeight; }
            set
            {
                if (_FiveDigitWeight != value)
                {
                    _FiveDigitWeight = value;
                    OnPropertyChanged(nameof(FiveDigitWeight));
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

                }
            }
        }

        private int _TwoDigitWeight;
        public int TwoDigitWeight
        {
            get { return _TwoDigitWeight; }
            set
            {
                if (_TwoDigitWeight != value)
                {
                    _TwoDigitWeight = value;
                    OnPropertyChanged(nameof(TwoDigitWeight));
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private string _WeightShow;
        public string WeightShow
        {
            get { return _WeightShow; }
            set
            {
                if (_WeightShow != value)
                {
                    _WeightShow = value;
                    OnPropertyChanged(nameof(WeightShow)); // REQUIRED
                }
            }
        }

        private bool _isWeightSettingVisible;
        public bool IsWeightSettingVisible
        {
            get => _isWeightSettingVisible;
            set
            {
                _isWeightSettingVisible = value;
                OnPropertyChanged(nameof(IsWeightSettingVisible));
            }
        }

        private void OnComStartReading()
        {
            try
            {
                MainWindow.weighCommand = true;
                Model.InitializeReaders(false);
                IsWeightSettingVisible = true; // SHOW BOX
            }
            catch (Exception ex)
            {
                Model.RawData = ex.Message + "\r\n";
            }
        }

        private bool OnCanComStartReading()
        {

            if (Model == null)
                return true;
            if (Model != null && Model.ConnectionType == CommunicationProtocol.COM.ToString() && Model.GetErrors("COMPort") == null && Model.GetErrors("BaudRate") == null && Model.GetErrors("Parity") == null
                && Model.GetErrors("DataBits") == null && Model.GetErrors("StopBits") == null && Model.GetErrors("HandShake") == null && (Model.dr == null || !Model.IsOpen))
            {
                return true;
            }
            else if (Model != null && Model.ConnectionType == CommunicationProtocol.LAN.ToString() && Model.GetErrors("IPAddress") == null && Model.GetErrors("IPPort") == null)
            {
                return true;
            }
            else
                return false;

        }

        private void OnComStopReading()
        {
            IsWeightSettingVisible = false; // HIDE BOX
            Model.ReleaseReaders();
            MainWindow.weighCommand = false;
        }

        private bool OnCanComStopReading()
        {
            return Model != null && Model.dr != null && Model.IsOpen;
        }

        private void OnStartReadingWeight()
        {
            if (Model.IsValid())
            {
                MainWindow.weighCommand = true;
                Model.InitializeReaders(true);
            }
        }

        private bool OnCanStartReadingWeight()
        {
            if (Model == null)
                return true;
            if (Model != null && Model.GetErrors("TotalStringLength") == null && Model.GetErrors("StartStringChar") == null
                && Model.GetErrors("WEIGHTSTARTFROM") == null && Model.GetErrors("WEIGHTLENGTH") == null && Model.GetErrors("Format") == null)
            {
                return true;
            }
            else
                return false;
        }

        private List<Unit> _units;
        public List<Unit> Units
        {
            get { return _units; }
            set { _units = value; OnPropertyChanged(); }
        }

        private List<Indicator> _indicators;
        public List<Indicator> Indicators
        {
            get { return _indicators; }
            set { _indicators = value; OnPropertyChanged(); }
        }

        private List<String> _comports;
        public List<String> COMPorts
        {
            get { return _comports; }
            set { _comports = value; OnPropertyChanged(); }
        }

        public override async Task LoadAsync()
        {
            WeightBridge weightbridge = Id > 0
                      ? await dataRepository.Context.WeightBridges.SingleAsync(x => x.WeightBridgeID == Id)
                      : CreateNewWeighBridge();
            await SetupUnits();
            SetupIndicator();
            SetupPorts(weightbridge);

            InitializeModel(weightbridge);
            Model.PropertyChanged -= weightbridge_PropertyChanged;
            Model.PropertyChanged += weightbridge_PropertyChanged;
        }
        protected void weightbridge_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TotalStringLength" || e.PropertyName == "StartStringChar" || e.PropertyName == "WEIGHTSTARTFROM" ||
                e.PropertyName == "WEIGHTLENGTH" || e.PropertyName == "Format")
            {
                ((DelegateCommand)readWeight).RaiseCanExecuteChanged();
            }
            else if (e.PropertyName == "COMPort" || e.PropertyName == "BaudRate" || e.PropertyName == "Parity" ||
                e.PropertyName == "DataBits" || e.PropertyName == "StopBits" || e.PropertyName == "HandShake" ||
                e.PropertyName == "IPAddress" || e.PropertyName == "IPPort" || e.PropertyName == "ConnectionType")
            {
                ((DelegateCommand)startComReading).RaiseCanExecuteChanged();
            }
            else if (e.PropertyName == "ConnectionType")
            {
                if (Model.ConnectionType == "COM")
                {
                    OnPropertyChanged("COMPort");
                    OnPropertyChanged("BaudRate");
                    OnPropertyChanged("Parity");
                    OnPropertyChanged("DataBits");
                    OnPropertyChanged("StopBits");
                    OnPropertyChanged("HandShake");
                }
                else
                {
                    OnPropertyChanged("IPAddress");
                    OnPropertyChanged("IPPort");
                }
            }
            else if (e.PropertyName == "IsOpen")
            {
                ((DelegateCommand)stopComReading).RaiseCanExecuteChanged();
                ((DelegateCommand)startComReading).RaiseCanExecuteChanged();
                ((DelegateCommand)readWeight).RaiseCanExecuteChanged();
                ToastHelper.ShowInformation("Port " + (Model.IsOpen ? "opened" : "closed") + " successfully.");
            }
            else if (e.PropertyName == "IndicatorID")
            {
                Indicator selectedIndicator = Indicators.FirstOrDefault(x => x.id == Model.IndicatorID);
                Model.BaudRate = selectedIndicator.BaudRate;
                Model.COMPort = selectedIndicator.COMPort;
                Model.ConnectionType = selectedIndicator.ConnectionType;
                Model.DataBits = selectedIndicator.DataBits;
                Model.DecimalPointLocation = selectedIndicator.DecimalPointLocation;
                Model.Format = selectedIndicator.Format;
                Model.HandShake = selectedIndicator.HandShake;
                Model.IncrementSize = selectedIndicator.IncrementSize;
                Model.IPAddress = selectedIndicator.IPAddress;
                Model.IPPort = selectedIndicator.IPPort;
                Model.IsDecimalInString = selectedIndicator.IsDecimalInString;
                Model.IsReverseString = selectedIndicator.IsReverseString;
                Model.Parity = selectedIndicator.Parity;
                Model.StartStringChar = selectedIndicator.StartStringChar;
                Model.StopBits = selectedIndicator.StopBits;
                Model.StringCharLength = selectedIndicator.StringCharLength;
                Model.StringStartingValue = selectedIndicator.StringStartingValue;
                Model.TotalStringLength = selectedIndicator.TotalStringLength;
                Model.WEIGHTLENGTH = selectedIndicator.WEIGHTLENGTH;
                Model.WEIGHTSTARTFROM = selectedIndicator.WEIGHTSTARTFROM;
                OnPropertyChanged("ConnectionType");
            }
        }

        private async Task SetupUnits()
        {
            Units = await dataRepository.Context.Units.AsNoTracking().ToListAsync();
            Units.Insert(0, new Unit() { UnitID = 0, UnitName = "-- Select Unit --" });
        }

        private void SetupIndicator()
        {
            string indicatorDoc = System.IO.File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "indicators.json");
            List<Indicator> lstIndicators = JsonConvert.DeserializeObject<List<Indicator>>(indicatorDoc).ToList();
            Indicators = lstIndicators;
        }

        private void SetupPorts(WeightBridge weightbridge)
        {
            List<String> SystemPorts = System.IO.Ports.SerialPort.GetPortNames().ToList();
            COMPorts = new List<string>(SystemPorts);
            if (!string.IsNullOrEmpty(weightbridge.COMPort) && COMPorts.All(x => x != weightbridge.COMPort))
            {
                COMPorts.Add(weightbridge.COMPort);
            }
        }
        private WeightBridge CreateNewWeighBridge()
        {
            WeightBridge weighbridge = CreateNewModel();
            weighbridge.ConnectionType = "COM";
            weighbridge.BaudRate = "2400";
            weighbridge.Parity = "None";
            weighbridge.DataBits = 8;
            weighbridge.StopBits = "One";
            weighbridge.HandShake = "None";
            weighbridge.IsActive = true;
            weighbridge.IsReverseString = false;
            weighbridge.IsDecimalInString = false;
            return weighbridge;
        }



        protected override async void AfterSaveExecute()
        {
            ToastHelper.ShowSuccess("WeightBridge information recorded successfully");
            RaiseDetailSavedEvent(Model.WeightBridgeID, Model);
            MainWindow.weighCommand = true;

            MessageDialogResult msgbox_result = await MessageBoxHelper.ShowYesNoDialog("WeightBridge", "WeightBridge Data Change Need To Restart");
            if (msgbox_result == MessageDialogResult.Affirmative)
            {
                try
                {
                    var current = System.Diagnostics.Process.GetCurrentProcess();
                    System.Diagnostics.Process.GetProcessesByName(current.ProcessName).ToList().ForEach(t => t.Kill());
                }
                catch
                {
                    System.Windows.Application.Current.Shutdown();
                }
            }
            else
            {
                try
                {
                    var current = System.Diagnostics.Process.GetCurrentProcess();
                    System.Diagnostics.Process.GetProcessesByName(current.ProcessName).ToList().ForEach(t => t.Kill());
                }
                catch
                {
                    System.Windows.Application.Current.Shutdown();
                }
            }
        }
    }
}