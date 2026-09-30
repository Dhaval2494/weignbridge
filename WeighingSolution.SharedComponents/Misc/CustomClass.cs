using System;

namespace WeighingSolution.SharedComponents
{
    [Serializable]
    public class PluginManagement
    {
        private string _PluginName;
        public string PluginName
        {
            get { return _PluginName; }
            set { _PluginName = value; }
        }

        private bool _IsEnable;
        public bool IsEnable
        {
            get { return _IsEnable; }
            set { _IsEnable = value; }
        }

        private Nullable<bool> _ISInstall;
        public Nullable<bool> ISInstall
        {
            get { return _ISInstall; }
            set { _ISInstall = value; }
        }
        private bool isconfigur;
        public bool IsConfigur
        {
            get { return isconfigur; }
            set { isconfigur = value; }
        }
        private bool _IsSVCInstalled;

        public bool IsSVCInstalled
        {
            get { return _IsSVCInstalled; }
            set { _IsSVCInstalled = value; }
        }
    }

    public class IssuedPlugin
    {
        public string PluginName { get; set; }
        public string ExpiredOn { get; set; }
    }

    public class Indicator
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string ConnectionType { get; set; }
        public string IncrementSize { get; set; }
        public string IPAddress { get; set; }
        public string IPPort { get; set; }
        public string COMPort { get; set; }
        public string BaudRate { get; set; }
        public Nullable<byte> DataBits { get; set; }
        public string StopBits { get; set; }
        public string Parity { get; set; }
        public Nullable<short> TotalStringLength { get; set; }
        public string StartStringChar { get; set; }
        public string StringCharLength { get; set; }
        public string Format { get; set; }
        public string StringStartingValue { get; set; }
        public Nullable<bool> IsDecimalInString { get; set; }
        public Nullable<short> DecimalPointLocation { get; set; }
        public Nullable<bool> IsReverseString { get; set; }
        public string WEIGHTSTARTFROM { get; set; }
        public string WEIGHTLENGTH { get; set; }
        public string HandShake { get; set; }
    }
}
