using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace WeighingSolution.DataLayer
{
    public partial class WeightBridge : NotifyDataErrorInfoBase
    {
        public DataReader dr;
        public bool StartStringIsNewLineChar
        {
            get { return StartStringChar == "\n"; }
            set { StartStringChar = value ? "\n" : ""; OnPropertyChanged(); }
        }

        public string StartingCharacter
        {
            get
            {
                if (!string.IsNullOrEmpty(StartStringChar))
                {
                    if (StartStringChar.ToUpper() == "<LF>")
                    {
                        return "\n";
                    }
                    else if (StartStringChar.ToUpper() == "<CR>")
                    {
                        return "\r";
                    }
                    else if (StartStringChar.ToUpper() == "<CRLF>")
                    {
                        return "\r\n";
                    }
                    else if (StartStringChar.ToUpper() == "<SPACE>")
                    {
                        return " ";
                    }
                }
                return StartStringChar;
            }
        }

        public string WeightFormat
        {
            get
            {
                if (String.IsNullOrEmpty(Format))
                    return "#######.00";
                else
                    return Format;
            }
        }

        public bool canDelete
        {
            get
            {
                using (var ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                {
                    if (ctx.TransactionDetails.Any(x => x.WeightBridgeID == WeightBridgeID))
                        return false;
                    else
                        return true;
                }
            }
        }
        #region Comport Data

        private bool _IsCablePlugged;

        public bool IsCablePlugged
        {
            get { return _IsCablePlugged; }
            set { _IsCablePlugged = value; OnPropertyChanged(); }
        }

        private string _rawData;
        public string RawData
        {
            get { return _rawData; }
            set
            {
                //_rawData = value;
                _rawData = !string.IsNullOrEmpty(value) ? value + "\r\n" + _rawData : "";
                if (_rawData != null && _rawData.Length >= 1000) //keep a max of 5000 characters in the dump text box.
                {
                    _rawData = _rawData.Substring(0, 1000);
                }
                OnPropertyChanged("RawData");
            }
        }

        private string _formatedData;
        public string FormatedData
        {
            get { return _formatedData; }
            set { _formatedData = value; OnPropertyChanged(); }
        }

        private bool _isFormattedApply;
        public bool IsFormattedApply
        {
            get { return _isFormattedApply; }
            set { _isFormattedApply = value; OnPropertyChanged(); }
        }

        public bool IsOpen
        {
            get
            {
                bool retval = false;
                if (dr != null)
                {
                    if (this.ConnectionType == CommunicationProtocol.COM.ToString())
                    {
                        if (dr.ComPort != null)
                            retval = dr.ComPort.IsOpen;
                    }
                    else if (this.ConnectionType == CommunicationProtocol.LAN.ToString())
                    {
                        if (dr.socket != null)
                            retval = dr.socket.Connected;
                    }
                }
                return retval;
            }
        }

        #endregion

        protected override IEnumerable<string> VerifyBusinessLogic(string propertyName)
        {
            switch (propertyName)
            {
                //case "COMPort":
                //    if (ConnectionType == "COM")
                //    {
                //        if (string.IsNullOrEmpty(COMPort))
                //            yield return "Serial port is required.";
                //        using (var ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                //        {
                //            if (WeightBridgeID == 0)
                //            {
                //                if (ctx.WeightBridges.Any(x => x.COMPort.ToUpper() == COMPort.ToUpper() && x.IsActive))
                //                {
                //                    yield return "Serial Port already exist , can not add duplicate Serial Port.";
                //                }
                //            }
                //            else
                //            {
                //                if (ctx.WeightBridges.Any(x => x.COMPort.ToUpper() == COMPort.ToUpper() && x.WeightBridgeID != WeightBridgeID && x.IsActive))
                //                {
                //                    yield return "Serial Port already exist , can not add duplicate Serial Port.";
                //                }
                //            }
                //        }
                //    }
                //    break;
                case "ScaleName":
                    using (var ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                    {
                        if (WeightBridgeID == 0)
                        {
                            if (ctx.WeightBridges.Any(x => x.ScaleName.ToUpper() == ScaleName.ToUpper() && x.IsActive))
                            {
                                yield return "Scale name already exist , can not add duplicate Scale name.";
                            }
                        }
                        else
                        {
                            if (ctx.WeightBridges.Any(x => x.ScaleName.ToUpper() == ScaleName.ToUpper() && x.WeightBridgeID != WeightBridgeID && x.IsActive))
                            {
                                yield return "Scale name already exist , can not add duplicate scale name.";
                            }
                        }
                    }
                    break;
            }
        }
        protected override IEnumerable<string> ValidateProperty(string propertyName)
        {
            switch (propertyName)
            {
                case "IPAddress":
                    if (ConnectionType == "LAN")
                    {
                        if (string.IsNullOrEmpty(IPAddress))
                            yield return "IP Address is required.";
                    }
                    break;
                case "IPPort":
                    if (ConnectionType == "LAN")
                    {
                        if (string.IsNullOrEmpty(IPPort))
                            yield return "IP Port is required.";
                    }
                    break;
                case "StartStringChar":
                    if (!StartStringIsNewLineChar)
                    {
                        if (string.IsNullOrEmpty(StartStringChar))
                            yield return "Start String Character is required.";
                    }
                    break;
                case "DecimalPointLocation":
                    if (IsDecimalInString.HasValue && IsDecimalInString.Value)
                    {
                        if (DecimalPointLocation == null)
                            yield return "Decimal Point Location is required.";
                    }
                    break;
            }
        }
        public void InitializeReaders(bool isFormattedApply)
        {
            this.IsFormattedApply = isFormattedApply;
            if (dr == null)
                dr = new DataReader(this, SynchronizationContext.Current);
            if (this.IsOpen) dr.ClosePort();
            dr.OpenPort();
        }

        public void ReleaseReaders()
        {
            if (dr != null)
                dr.ClosePort();
        }
    }
}