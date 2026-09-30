using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Timers;
using System.ComponentModel;

namespace WeighingSolution.DataLayer
{
    public class ComHelper
    {
        public SerialPort ComPort;
        public Socket socket;
        public WeightBridge objWeighBridge { get; set; }
        //https://www.codeproject.com/Tips/1111432/Update-data-to-WPF-control-from-another-thread
        public System.Threading.SynchronizationContext SyncContext;

        #region Timer - Calls Receive data functionality on Tick Event. Set "FormatedData" property in WeightBridge Object
        public int timerInterval = 250;
        private Timer _readTimer;
        private Timer ReadTimer
        {
            get
            {
                return this._readTimer;
            }
            set
            {
                ElapsedEventHandler handler = new ElapsedEventHandler(this.ReadTimerElapsed);
                if (this._readTimer != null)
                {
                    this._readTimer.Elapsed -= handler;
                }
                this._readTimer = value;
                if (this._readTimer != null)
                {
                    this._readTimer.Elapsed += handler;
                }
            }
        }

        private void ReadTimerElapsed(object sender, EventArgs e)
        {
            //Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            this.ReadTimer.Stop();
            this.ReadWeightValue();
            this.ReadTimer.Start();
            //Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
        }

        #endregion

        public ComHelper(int ReadTimerInterval = 250)
        {
            //Fires at default interval of every 100 milliseconds
            this.timerInterval = ReadTimerInterval;
            this.ReadTimer = new Timer();
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

        private void ReadWeightValue()
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
                                strBuffer = this.ComPort.ReadExisting();
                                objWeighBridge.RawData = strBuffer;
                                //this.ComPort.DiscardInBuffer();
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                else if (objWeighBridge.ConnectionType == CommunicationProtocol.LAN.ToString())
                {
                    if (this.socket.Connected && this.socket.Available > 0)
                    {
                        byte[] buffer = new byte[this.socket.Available + 1];
                        int len = this.socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                        strBuffer = System.Text.Encoding.UTF8.GetString(buffer, 0, len);
                        objWeighBridge.RawData = strBuffer;
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

                                for (int j = 0; j < strBuffer.Length; j += objWeighBridge.TotalStringLength.Value)
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

                                        if (Convert.ToInt32(objWeighBridge.WEIGHTLENGTH) == 0)
                                        {
                                            objWeighBridge.WEIGHTLENGTH = "1";
                                        }

                                        //Reverse if needed
                                        if (objWeighBridge.IsReverseString.GetValueOrDefault(false))
                                        {
                                            strDataChunk = StrReverse(strDataChunk);
                                        }

                                        //Get the Weight portion from the entire data string/chunk
                                        strDataChunk = Mid(strDataChunk, Convert.ToInt32(objWeighBridge.WEIGHTSTARTFROM), Convert.ToInt32(objWeighBridge.WEIGHTLENGTH)).Trim();

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

                                            //Format the processed weight and store it in FormatedData variable
                                            if (objWeighBridge.WeightFormat.Contains("."))
                                            {
                                                objWeighBridge.FormatedData = negativeSign + Decimal.Parse(processedWeight).ToString(objWeighBridge.WeightFormat);
                                            }
                                            else
                                            {
                                                objWeighBridge.FormatedData = negativeSign + Convert.ToInt64(processedWeight).ToString();
                                            }
                                        }
                                        else
                                        {
                                            objWeighBridge.FormatedData = "0";
                                        }
                                    }
                                }
                            }
                            //Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
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
        }
        #endregion

        #region "Port Operation Methods - OpenPort, ClosePort, IsPortOpen"
        //Opens Communcation Port, Enables Timer to start reading weight continuously
        public void OpenPort()
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
                        if (!this.ComPort.IsOpen)
                        {
                            this.ComPort.Open();
                            SyncContext.Send(x => { objWeighBridge.OnPropertyChanged("IsOpen"); }, null);
                        }
                    }
                    else if (objWeighBridge.ConnectionType == CommunicationProtocol.LAN.ToString())
                    {
                        this.socket = new Socket(IPAddress.Parse(objWeighBridge.IPAddress).AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 5000);
                        this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);
                        this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, 1);
                        this.socket.Connect(new IPEndPoint(IPAddress.Parse(objWeighBridge.IPAddress), Convert.ToInt32(objWeighBridge.IPPort)));
                        SyncContext.Send(x => { objWeighBridge.OnPropertyChanged("IsOpen"); }, null);
                    }
                    this.ReadTimer.Interval = this.timerInterval;
                    this.ReadTimer.Start();
                }
            }
            catch (Exception ex)
            {
                objWeighBridge.RawData += ex.Message;
            }
        }

        //Closes the Communcation Port, Disbles the Timer
        public void ClosePort()
        {
            try
            {
                if (objWeighBridge != null)
                {
                    if (objWeighBridge.ConnectionType == CommunicationProtocol.COM.ToString())
                    {
                        if (this.ComPort != null && this.ComPort.IsOpen)
                        {
                            this.ReadTimer.Stop();
                            this.ComPort.DiscardInBuffer();
                            this.ComPort.Close();
                            objWeighBridge.OnPropertyChanged("IsOpen");
                        }
                    }
                    else if (objWeighBridge.ConnectionType == CommunicationProtocol.LAN.ToString())
                    {
                        if (this.socket != null && this.socket.Connected)
                        {
                            this.ReadTimer.Stop();
                            this.socket.Shutdown(SocketShutdown.Both);
                            this.socket.Close();
                            objWeighBridge.OnPropertyChanged("IsOpen");
                        }
                    }
                }
            }
            catch (Exception)
            {
                
            }
        }

        //public bool IsOpen()
        //{
        //    bool retval = false;
        //    if (objWeighBridge != null)
        //    {
        //        if (objWeighBridge.ConnectionType == CommunicationProtocol.COM.ToString())
        //        {
        //            if (this.ComPort != null)
        //                retval = this.ComPort.IsOpen;
        //        }
        //        else if (objWeighBridge.ConnectionType == CommunicationProtocol.LAN.ToString())
        //        {
        //            if (this.socket != null)
        //                retval = this.socket.Connected;
        //        }
        //    }
        //    return retval;
        //}
        #endregion
    }
}
