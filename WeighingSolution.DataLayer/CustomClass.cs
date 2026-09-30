using System;

namespace WeighingSolution.DataLayer
{
    public class DropdownListData
    {
        public string DataTextField { get; set; }
        public int DataValueField { get; set; }
    }
   
    public class DropdownListDataString
    {
        public string DataTextField { get; set; }
        public string DataValueField { get; set; }
    }
    public class StableWeigthClass
    {
        public DateTime DateTime { get; set; }
        public int StableWeigth { get; set; }
    }
    public class PrintSettingClass
    {
        public string PrintType { get; set; }
        public string PrinterName { get; set; }
    }

    public class DBSettingData : NotifyDataErrorInfoBase
    {
        private bool _InstallDB;
        private string _AttachDB;
        private string _Server;
        private string _DBName;
        private string _UserId;
        private string _password;
        private int _Port;
        public bool InstallDB
        {
            get { return _InstallDB; }
            set { _InstallDB = value; OnPropertyChanged(); }
        }
        public string Server
        {
            get { return _Server; }
            set { _Server = value; OnPropertyChanged(); }
        }

        public string DBName
        {
            get { return _DBName; }
            set { _DBName = value; OnPropertyChanged(); }
        }
        public string AttachDB
        {
            get { return _AttachDB; }
            set { _AttachDB = value; OnPropertyChanged(); }
        }
        public string UserId
        {
            get { return _UserId; }
            set { _UserId = value; OnPropertyChanged(); }
        }
        public string password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(); }
        }
        public int Port
        {
            get { return _Port; }
            set { _Port = value; OnPropertyChanged(); }
        }
    }
}
