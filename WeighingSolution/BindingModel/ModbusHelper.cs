using ModbusLib;
using ModbusLib.Protocols;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeighingSolution.DataLayer;

namespace WeighingSolution
{
    public class ModbusHelper
    {
        private int _transactionId;
        private ModbusClient _driver;
        private ICommClient _portClient;
        public SerialPort PLCPort;

        private byte _lastReadCommand = 0;
        private bool _logPaused = false;

        List<IO_Mapping> IO_MappingList
        {
            get
            {
                List<IO_Mapping> dataMappings = new List<IO_Mapping>();
                dataMappings.Add(new IO_Mapping { IOName = "0", IOValue = 256 });
                dataMappings.Add(new IO_Mapping { IOName = "1", IOValue = 512 });
                dataMappings.Add(new IO_Mapping { IOName = "2", IOValue = 1024 });
                dataMappings.Add(new IO_Mapping { IOName = "3", IOValue = 2048 });
                dataMappings.Add(new IO_Mapping { IOName = "4", IOValue = 4096 });
                dataMappings.Add(new IO_Mapping { IOName = "5", IOValue = 8192 });
                dataMappings.Add(new IO_Mapping { IOName = "6", IOValue = 16384 });
                dataMappings.Add(new IO_Mapping { IOName = "7", IOValue = 32768 });
                return dataMappings;
            }
        }

        #region properties

        public string _LTG { get; set; }
        public string _LTR { get; set; }
        public string _RTG { get; set; }
        public string _RTR { get; set; }
        public string _LBO { get; set; }
        public string _LBC { get; set; }
        public string _RBO { get; set; }
        public string _RBC { get; set; }


        protected ushort StartAddress
        {
            get
            {
                return 0;
            }
        }

        protected ushort DataLength
        {
            get
            {
                return 8;
            }
        }

        protected byte SlaveId
        {
            get
            {
                return Byte.Parse("10");
            }

        }

        protected int SlaveDelay
        {
            get
            {
                return int.Parse("1");
            }
        }

        private string _PortName;
        public string PortName
        {
            get
            {
                return _PortName;
            }
            set
            {
                _PortName = value;
            }
        }

        private ushort[] _registerData;
        public ushort[] RegisterData
        {
            get
            {
                return _registerData;
            }
            set
            {
                _registerData = value;
            }
        }

        protected int Baud
        {
            get
            {
                return 19200;
            }
        }

        protected Parity Parity
        {
            get
            {
                return Parity.Even;
            }
        }

        protected int DataBits
        {
            get
            {
                return 8;
            }
        }

        protected StopBits StopBits
        {
            get
            {
                return StopBits.One;
            }
        }

        #endregion properties

        #region Connect/disconnect

        public void DoConnect()
        {
            PLCPort = new SerialPort(PortName, Baud, Parity, DataBits, StopBits);
            PLCPort.Open();
            _portClient = PLCPort.GetClient();
            _driver = new ModbusClient(new ModbusRtuCodec()) { Address = SlaveId };
            _driver.OutgoingData += DriverOutgoingData;
            _driver.IncommingData += DriverIncommingData;
        }


        public void DoDisconnect()
        {
            PLCPort.Close();
            PLCPort.Dispose();
            PLCPort = null;

            _portClient = null;
            _driver = null;
        }

        protected void DriverIncommingData(byte[] data, int len)
        {
            if (_logPaused)
                return;
            var hex = new StringBuilder(len);
            for (int i = 0; i < len; i++)
            {
                hex.AppendFormat("{0:x2} ", data[i]);
            }
        }
        protected void DriverOutgoingData(byte[] data)
        {
            if (_logPaused)
                return;
            var hex = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
                hex.AppendFormat("{0:x2} ", b);
        }

        #endregion


        public string ExecuteReadCommand()
        {
            byte function = ModbusCommand.FuncReadInputDiscretes;
            _lastReadCommand = function;

            try
            {
                var command = new ModbusCommand(function) { Offset = StartAddress, Count = DataLength, TransId = _transactionId++ };
                var result = _driver.ExecuteGeneric(_portClient, command);
                if (result.Status == CommResponse.Ack)
                {
                    RegisterData = command.Data;
                    return RegisterData[0].ToString();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public void ExecuteWriteCommand()
        {
            byte function = ModbusCommand.FuncForceMultipleCoils;
            try
            {
                var command = new ModbusCommand(function)
                {
                    Offset = StartAddress,
                    Count = DataLength,
                    TransId = _transactionId++,
                    Data = new ushort[DataLength]
                };
                for (int i = 0; i < DataLength; i++)
                {
                    var index = StartAddress + i;
                    if (index > RegisterData.Length)
                    {
                        break;
                    }
                    command.Data[i] = RegisterData[index];
                }
                var result = _driver.ExecuteGeneric(_portClient, command);

            }
            catch (Exception ex)
            {
            }
        }


        public void Trafiic_Left_Red()
        {
            try
            {
                List<string> OutputList = new List<string>();
                OutputList.Add(_LTR);
                string OutVal = CalulationIOValue(OutputList);
                RegisterData[0] = ushort.Parse(OutVal);
                ExecuteWriteCommand();
                Constant.IsTL = false;
            }
            catch
            {

            }
        }
        public void Trafiic_Left_Green()
        {
            try
            {
                List<string> OutputList = new List<string>();
                OutputList.Add(_LTG);
                string OutVal = CalulationIOValue(OutputList);
                RegisterData[0] = ushort.Parse(OutVal);
                ExecuteWriteCommand();
                Constant.IsTL = true;
            }
            catch
            {

            }
        }

        public void Trafiic_Right_Red()
        {
            try
            {
                List<string> OutputList = new List<string>();
                OutputList.Add(_RTR);
                string OutVal = CalulationIOValue(OutputList);
                RegisterData[0] = ushort.Parse(OutVal);
                ExecuteWriteCommand();
                Constant.IsTR = false;
            }
            catch
            {

            }
        }
        public void Trafiic_Right_Green()
        {
            try
            {
                List<string> OutputList = new List<string>();
                OutputList.Add(_RTG);
                string OutVal = CalulationIOValue(OutputList);
                RegisterData[0] = ushort.Parse(OutVal);
                ExecuteWriteCommand();
                Constant.IsTR = true;
            }
            catch
            {

            }
        }

        public void Boom_Left_Open()
        {
            try
            {
                List<string> OutputList = new List<string>();
                OutputList.Add(_LBO);
                string OutVal = CalulationIOValue(OutputList);
                RegisterData[0] = ushort.Parse(OutVal);
                ExecuteWriteCommand();
                Constant.IsBoomLeft = true;
            }
            catch { }
        }
        public void Boom_Left_Close()
        {
            try
            {
                List<string> OutputList = new List<string>();
                OutputList.Add(_LBC);
                string OutVal = CalulationIOValue(OutputList);
                RegisterData[0] = ushort.Parse(OutVal);
                ExecuteWriteCommand();
                Constant.IsBoomLeft = false;
            }
            catch { }
        }

        public void Boom_Right_Open()
        {
            try
            {
                List<string> OutputList = new List<string>();
                OutputList.Add(_RBO);
                string OutVal = CalulationIOValue(OutputList);
                RegisterData[0] = ushort.Parse(OutVal);
                ExecuteWriteCommand();
                Constant.IsBoomRight = true;
            }
            catch { }
        }
        public void Boom_Right_Close()
        {
            try
            {
                List<string> OutputList = new List<string>();
                OutputList.Add(_RBC);
                string OutVal = CalulationIOValue(OutputList);
                RegisterData[0] = ushort.Parse(OutVal);
                ExecuteWriteCommand();
                Constant.IsBoomRight = false;
            }
            catch { }
        }


        public void Ideal()
        {
            try
            {
                List<string> OutputList = new List<string>();
                OutputList.Add(_RBC);
                OutputList.Add(_LBC);
                OutputList.Add(_LTR);
                OutputList.Add(_RTR);
                string OutVal = CalulationIOValue(OutputList);

                RegisterData[0] = ushort.Parse(OutVal);//"43520"
                ExecuteWriteCommand();
                Constant.IsTL = false;
                Constant.IsTR = false;
                Constant.IsBoomLeft = false;
                Constant.IsBoomRight = false;
            }
            catch { }
        }

        public void Left_Card_OK()
        {
            try
            {
                List<string> OutputList = new List<string>();
                OutputList.Add(_RBC);
                OutputList.Add(_LBO);
                OutputList.Add(_RTG);
                OutputList.Add(_LTR);
                string OutVal = CalulationIOValue(OutputList);

                RegisterData[0] = ushort.Parse(OutVal);//"38400"
                //RegisterData[0] = ushort.Parse(Left_Card_OK_cmd);
                ExecuteWriteCommand();
                Constant.IsTL = false;
                Constant.IsTR = true;
                Constant.IsBoomLeft = true;
                Constant.IsBoomRight = false;
            }
            catch { }
        }

        public void Left_Weight_Done()
        {
            try
            {
                List<string> OutputList = new List<string>();
                OutputList.Add(_RBO);
                OutputList.Add(_LBC);
                OutputList.Add(_RTG);
                OutputList.Add(_LTR);
                string OutVal = CalulationIOValue(OutputList);
                RegisterData[0] = ushort.Parse(OutVal);
                ExecuteWriteCommand();
                Constant.IsTL = false;
                Constant.IsTR = true;
                Constant.IsBoomLeft = false;
                Constant.IsBoomRight = true;
            }
            catch { }
        }


        public void Right_Card_OK()
        {
            try
            {
                List<string> OutputList = new List<string>();
                OutputList.Add(_RBO);
                OutputList.Add(_LBC);
                OutputList.Add(_RTR);
                OutputList.Add(_LTG);
                string OutVal = CalulationIOValue(OutputList);
                RegisterData[0] = ushort.Parse(OutVal);
                ExecuteWriteCommand();
                Constant.IsTL = true;
                Constant.IsTR = false;
                Constant.IsBoomLeft = false;
                Constant.IsBoomRight = true;
            }
            catch { }
        }

        public void Right_Weight_Done()
        {
            try
            {
                List<string> OutputList = new List<string>();
                OutputList.Add(_RBC);
                OutputList.Add(_LBO);
                OutputList.Add(_RTR);
                OutputList.Add(_LTG);
                string OutVal = CalulationIOValue(OutputList);
                RegisterData[0] = ushort.Parse(OutVal);
                ExecuteWriteCommand();
                Constant.IsTL = true;
                Constant.IsTR = false;
                Constant.IsBoomLeft = true;
                Constant.IsBoomRight = false;

            }
            catch { }
        }

        public string CalulationIOValue(List<string> OutputList)
        {
            try
            {
                int Sumval = 0;
                foreach (var item in OutputList)
                {
                    if (Convert.ToString(item) != "-1")
                    { 
                        int Val = IO_MappingList.Where(p => p.IOName == item).FirstOrDefault().IOValue;

                        Sumval = Sumval + Val;
                    }
                    
                }

                return Sumval.ToString();
            }
            catch
            {
                return "";
            }
        }

    }

    public class IO_Mapping
    {
        public string IOName { get; set; }
        public int IOValue { get; set; }
    }
}

