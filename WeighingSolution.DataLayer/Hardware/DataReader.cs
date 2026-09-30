using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace WeighingSolution.DataLayer
{
    public class DataReader
    {
        private Thread readerThread = null;
        private bool readData = false;

        public SerialPort ComPort;
        public Socket socket;

        public WeightBridge objWeighBridge;
        //https://www.codeproject.com/Tips/1111432/Update-data-to-WPF-control-from-another-thread
        public System.Threading.SynchronizationContext SyncContext;

        public DataReader(WeightBridge weighbridge, SynchronizationContext syncContext)
        {
            objWeighBridge = weighbridge;
            objWeighBridge.IsCablePlugged = true;
            SyncContext = syncContext;
        }

        #region "Helper Methods"
        private string Mid(string s, int startindex, int length)
        {
            if (startindex + length <= s.Length)
                return s.Substring(startindex, length);
            else
                return s.Substring(startindex);
        }

        private string StrReverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        #endregion

        #region "Port Operation Methods - OpenPort, ClosePort, IsPortOpen"
        //Opens Communcation Port and starts reading weight continuously on readerThread
        public void OpenPort()
        {
            try
            {
                readerThread = new Thread(new ThreadStart(ReaderThreadStart));
                readerThread.Start();
            }
            catch (Exception ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.WriteAllText(Constant.CustomSettingsPath + @"\OpenPortError.txt", json);
                SyncContext.Send(x => { objWeighBridge.RawData = ex.Message; }, null);
                objWeighBridge.IsCablePlugged = false;
                objWeighBridge.OnPropertyChanged("IsCablePlugged");
            }
        }

        private void ReaderThreadStart()
        {
            try
            {
                if (objWeighBridge != null)
                {
                    if (objWeighBridge.ConnectionType == CommunicationProtocol.COM.ToString())
                    {
                        this.ComPort = new SerialPort(objWeighBridge.COMPort, Convert.ToInt32(objWeighBridge.BaudRate), (Parity)Enum.Parse(typeof(Parity), objWeighBridge.Parity), Convert.ToInt32(objWeighBridge.DataBits), (StopBits)Enum.Parse(typeof(StopBits), objWeighBridge.StopBits));
                        this.ComPort.Encoding = System.Text.Encoding.UTF8;
                        this.ComPort.Handshake = (Handshake)Enum.Parse(typeof(Handshake), objWeighBridge.HandShake);
                        this.ComPort.DtrEnable = objWeighBridge.IsDRT;
                        this.ComPort.RtsEnable = objWeighBridge.IsRTS;
                        if (!this.ComPort.IsOpen)
                        {
                            this.ComPort.Open();
                            SyncContext.Send(x => { objWeighBridge.OnPropertyChanged("IsOpen"); }, null);
                            readData = true;
                        }
                    }
                    else if (objWeighBridge.ConnectionType == CommunicationProtocol.LAN.ToString())
                    {
                        this.socket = new Socket(IPAddress.Parse(objWeighBridge.IPAddress).AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 5000);
                        this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);
                        this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, 1);
                        //this.socket.Connect(new IPEndPoint(IPAddress.Parse(objWeighBridge.IPAddress), Convert.ToInt32(objWeighBridge.IPPort)));

                        IAsyncResult result = socket.BeginConnect(new IPEndPoint(IPAddress.Parse(objWeighBridge.IPAddress), Convert.ToInt32(objWeighBridge.IPPort)), null, null);
                        bool success = result.AsyncWaitHandle.WaitOne(5000, true);
                        if (socket.Connected)
                        {
                            socket.EndConnect(result);
                            SyncContext.Send(x => { objWeighBridge.OnPropertyChanged("IsOpen"); }, null);
                            readData = true;
                        }
                        else
                        {
                            // NOTE, MUST CLOSE THE SOCKET
                            socket.Close();
                            SyncContext.Send(x => { objWeighBridge.RawData = "Failed to connect server."; }, null);
                            Constant.IndicatorWeight = 0;
                            //throw new ApplicationException("Failed to connect server.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string json = Environment.NewLine + "-------------------  " + DateTime.Now + Environment.NewLine + JsonConvert.SerializeObject(ex);
                File.WriteAllText(Constant.CustomSettingsPath + @"\OpenPortError.txt", json);
                SyncContext.Send(x => { objWeighBridge.RawData = ex.Message; }, null);
                objWeighBridge.IsCablePlugged = false;
                objWeighBridge.OnPropertyChanged("IsCablePlugged");
                Constant.IndicatorWeight = 0;
            }
            while (readData)
            {
                string strBuffer = "";
                try
                {
                    if (objWeighBridge.ConnectionType == CommunicationProtocol.COM.ToString())
                    {
                        if (this.ComPort.IsOpen)
                        {
                            try
                            {
                                if (this.ComPort.ReceivedBytesThreshold > 0)
                                {
                                    objWeighBridge.IsCablePlugged = true;
                                    objWeighBridge.OnPropertyChanged("IsCablePlugged");
                                    strBuffer = this.ComPort.ReadExisting();
                                    objWeighBridge.RawData = strBuffer;
                                    SyncContext.Send(x => { objWeighBridge.RawData = strBuffer; }, null);
                                    //this.ComPort.DiscardInBuffer();
                                }
                            }
                            catch (Exception)
                            {
                                objWeighBridge.IsCablePlugged = false;
                                objWeighBridge.OnPropertyChanged("IsCablePlugged");
                                Constant.IndicatorWeight = 0;
                            }
                        }
                        else
                        {
                            objWeighBridge.IsCablePlugged = false;
                            objWeighBridge.OnPropertyChanged("IsCablePlugged");
                            Constant.IndicatorWeight = 0;
                        }
                    }
                    else if (objWeighBridge.ConnectionType == CommunicationProtocol.LAN.ToString())
                    {
                        if (this.socket != null && !(this.socket.Poll(1000, SelectMode.SelectRead) && this.socket.Available == 0))
                        {
                            //if (this.socket.Connected && this.socket.Available > 0)
                        
                            try
                            {
                                objWeighBridge.IsCablePlugged = true;
                                objWeighBridge.OnPropertyChanged("IsCablePlugged");
                                byte[] buffer = new byte[this.socket.Available + 1];
                                int len = this.socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                                strBuffer = System.Text.Encoding.UTF8.GetString(buffer, 0, len);
                                SyncContext.Send(x => { objWeighBridge.RawData = strBuffer; }, null);
                            }
                            catch (Exception)
                            {
                                objWeighBridge.IsCablePlugged = false;
                                objWeighBridge.OnPropertyChanged("IsCablePlugged");
                                Constant.IndicatorWeight = 0;
                            }
                        }
                        else
                        {
                            objWeighBridge.IsCablePlugged = false;
                            objWeighBridge.OnPropertyChanged("IsCablePlugged");
                            Constant.IndicatorWeight = 0;
                        }
                    }

                    if (objWeighBridge.IsFormattedApply)
                    {
                        try
                        {
                            if (!String.IsNullOrEmpty(strBuffer) && strBuffer.Length <= 2000) //Ignore first time large data and read only the chunks at 100ms
                            {
                                if (!String.IsNullOrEmpty(objWeighBridge.StartingCharacter) && (objWeighBridge.StartingCharacter == "`"))
                                {
                                    strBuffer = strBuffer.Replace("\r\n", "`");
                                }

                                //Remove any junk data in the beginning of the string.
                                if (!String.IsNullOrEmpty(objWeighBridge.StartingCharacter))
                                {
                                    int startPosition = strBuffer.IndexOf(objWeighBridge.StartingCharacter);
                                    if (startPosition >= 0)
                                        strBuffer = strBuffer.Substring(startPosition);
                                }

                                if (strBuffer.Length > 1)
                                {
                                    bool WeightFound = false;

                                    for (int j = 0; j < strBuffer.Length; j += objWeighBridge.TotalStringLength.Value)
                                    {
                                        if (!WeightFound)
                                        {

                                            if ((j + objWeighBridge.TotalStringLength) <= strBuffer.Length) //Extract parts, ignoring last incomplete section
                                            {
                                                string strDataChunk = strBuffer.Substring(j, objWeighBridge.TotalStringLength.Value);

                                                //Remove any junk data, until the start_char is encountered again.
                                                if (!String.IsNullOrEmpty(objWeighBridge.StartingCharacter) && !strDataChunk.StartsWith(objWeighBridge.StartingCharacter))
                                                {
                                                    strBuffer = strBuffer.Substring(j);

                                                    int startPosition = strBuffer.IndexOf(objWeighBridge.StartingCharacter);
                                                    if (startPosition >= 0)
                                                        strBuffer = strBuffer.Substring(startPosition);
                                                    else
                                                        continue;

                                                    if (objWeighBridge.TotalStringLength <= strBuffer.Length) //Extract parts, ignoring last incomplete section
                                                    {
                                                        strDataChunk = strBuffer.Substring(0, objWeighBridge.TotalStringLength.Value);
                                                    }
                                                    else
                                                    {
                                                        strDataChunk = strBuffer;
                                                    }
                                                    if (strDataChunk.Length != objWeighBridge.TotalStringLength)
                                                    {
                                                        continue;
                                                    }
                                                    j = 0;
                                                }

                                                int weightlength = 0;
                                                if (!Int32.TryParse(objWeighBridge.WEIGHTLENGTH, out weightlength) || weightlength == 0)
                                                {
                                                    weightlength = 1;
                                                }

                                                //Reverse if needed
                                                if (objWeighBridge.IsReverseString.GetValueOrDefault(false))
                                                {
                                                    strDataChunk = StrReverse(strDataChunk);
                                                }

                                                //Get the Weight portion from the entire data string/chunk
                                                strDataChunk = Mid(strDataChunk, Convert.ToInt32(objWeighBridge.WEIGHTSTARTFROM), weightlength).Trim();

                                                //Processing of Numeric, Decimal point (.) and Negative Sign (-)
                                                String negativeSign = "";
                                                StringBuilder sb1 = new StringBuilder();
                                                Double dummy;
                                                for (int k = 0; k < strDataChunk.Length; k++)
                                                {
                                                    string currentChar = strDataChunk[k].ToString();
                                                    if (String.IsNullOrEmpty(negativeSign) && currentChar == "-")
                                                        negativeSign = "-";
                                                    if (Double.TryParse(currentChar, out dummy) || (currentChar == "."))
                                                    {
                                                        sb1.Append(currentChar);
                                                    }
                                                }

                                                string processedWeight = sb1.ToString();
                                                if (!String.IsNullOrEmpty(processedWeight))
                                                {
                                                    processedWeight = processedWeight.Trim();

                                                    if (objWeighBridge.IsDecimalInString.GetValueOrDefault(false) && !processedWeight.Contains("."))
                                                    {
                                                        string input = processedWeight;
                                                        int Decimal_point = Convert.ToInt32(objWeighBridge.DecimalPointLocation);

                                                        // Convert to decimal and adjust
                                                        decimal number = decimal.Parse(input);
                                                        decimal divisor = (decimal)Math.Pow(10, Decimal_point);
                                                        decimal result = number / divisor;

                                                        processedWeight = result.ToString($"F{Decimal_point}");
                                                    }
                                                    else
                                                    {
                                                        //If Decimal Point (.) is required, but not found, then add decimal point.
                                                        if (processedWeight.IndexOf(".") < 0 && objWeighBridge.IsDecimalInString.GetValueOrDefault(false) && Convert.ToDecimal(processedWeight) != 0.0m)
                                                        {
                                                            if (processedWeight.Length < objWeighBridge.DecimalPointLocation)
                                                            {
                                                                if ((objWeighBridge.DecimalPointLocation - processedWeight.Length) == 1)
                                                                {
                                                                    processedWeight = "0.0" + processedWeight;
                                                                }
                                                                if ((objWeighBridge.DecimalPointLocation - processedWeight.Length) == 2)
                                                                {
                                                                    processedWeight = "0.00" + processedWeight;
                                                                }
                                                                if ((objWeighBridge.DecimalPointLocation - processedWeight.Length) == 3)
                                                                {
                                                                    processedWeight = "0.000" + processedWeight;
                                                                }
                                                                if ((objWeighBridge.DecimalPointLocation - processedWeight.Length) == 4)
                                                                {
                                                                    processedWeight = "0.0000" + processedWeight;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                processedWeight = processedWeight.Substring(0, processedWeight.Length - objWeighBridge.DecimalPointLocation.GetValueOrDefault(0)) + "." + processedWeight.Substring((processedWeight.Length - objWeighBridge.DecimalPointLocation.GetValueOrDefault(0)) + 1);
                                                            }
                                                        }
                                                    }
                                                    //Format the processed weight and store it in FormatedData variable
                                                    if (objWeighBridge.WeightFormat.Contains("."))
                                                    {
                                                        Decimal readWeight = 0;
                                                        if (Decimal.TryParse(processedWeight, out readWeight))
                                                        {
                                                            Constant.IndicatorWeight = readWeight;
                                                            SyncContext.Send(x => { objWeighBridge.FormatedData = negativeSign + readWeight.ToString(objWeighBridge.WeightFormat); }, null);
                                                            WeightFound = true;
                                                        }
                                                        else
                                                        {
                                                            Constant.IndicatorWeight = 0;
                                                            SyncContext.Send(x => { objWeighBridge.FormatedData = "0"; }, null);
                                                            SyncContext.Send(x => { objWeighBridge.RawData = "Unable to fetch valid weight value from: " + processedWeight; }, null);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Int64 readWeight = 0;
                                                        if (Int64.TryParse(processedWeight, out readWeight))
                                                        {
                                                            Constant.IndicatorWeight = readWeight;
                                                            SyncContext.Send(x => { objWeighBridge.FormatedData = negativeSign + readWeight.ToString(); }, null);
                                                            WeightFound = true;
                                                        }
                                                        else
                                                        {
                                                            Constant.IndicatorWeight = 0;
                                                            SyncContext.Send(x => { objWeighBridge.FormatedData = "0"; }, null);
                                                            SyncContext.Send(x => { objWeighBridge.RawData = "Unable to fetch valid weight value from: " + processedWeight; }, null);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    Constant.IndicatorWeight = 0;
                                                    SyncContext.Send(x => { objWeighBridge.FormatedData = "0"; }, null);
                                                }
                                            }
                                        }
                                    }
                                }
                                //Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                            }
                            else
                            {
                                Constant.IndicatorWeight = 0;
                                SyncContext.Send(x => { objWeighBridge.FormatedData = "0"; }, null);
                                SyncContext.Send(x => { objWeighBridge.RawData = "Unable to fetch valid weight value from Indicator"; }, null);
                            }
                        }
                        catch (Exception)
                        {
                            //MessageBox.Show(strBuffer);
                            //Common.logErrorInDB("Following detail can not be formatted into capture weight. \n" + strBuffer, ex);
                        }
                    }
                }
                catch (Exception)
                {
                    //MessageBox.Show(strBuffer);
                    //Common.logErrorInDB(ex.Message, ex);
                }


                int speed = objWeighBridge.WeightSpeed.HasValue ? (int)objWeighBridge.WeightSpeed : 750;
                Thread.Sleep(speed);

            }
        }

        //Closes the Communcation Port, Disbles the Timer
        public void ClosePort()
        {
            try
            {
                if (readerThread != null)
                {
                    // Stop the TCP/IP Server.
                    readData = false;

                    // Wait for one second for the the thread to stop.
                    readerThread.Join(1000);

                    // If still alive; Get rid of the thread.
                    if (readerThread.IsAlive)
                    {
                        readerThread.Abort();
                    }
                    readerThread = null;
                }

                if (objWeighBridge != null)
                {
                    if (objWeighBridge.ConnectionType == CommunicationProtocol.COM.ToString())
                    {
                        if (this.ComPort != null && this.ComPort.IsOpen)
                        {
                            this.ComPort.DiscardInBuffer();
                            this.ComPort.Close();
                            SyncContext.Send(x => { objWeighBridge.OnPropertyChanged("IsOpen"); }, null);
                        }
                    }
                    else if (objWeighBridge.ConnectionType == CommunicationProtocol.LAN.ToString())
                    {
                        if (this.socket != null && this.socket.Connected)
                        {
                            this.socket.Shutdown(SocketShutdown.Both);
                            this.socket.Close();
                            SyncContext.Send(x => { objWeighBridge.OnPropertyChanged("IsOpen"); }, null);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion
    }
}
