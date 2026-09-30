namespace InstallerUI.Helpers
{
    public class DBSettingData
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
            set { _InstallDB = value; }
        }
        public string Server
        {
            get { return _Server; }
            set { _Server = value; }
        }

        public string DBName
        {
            get { return _DBName; }
            set { _DBName = value; }
        }
        public string AttachDB
        {
            get { return _AttachDB; }
            set { _AttachDB = value; }
        }
        public string UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }
        public string password
        {
            get { return _password; }
            set { _password = value; }
        }
        public int Port
        {
            get { return _Port; }
            set { _Port = value; }
        }
    }
}
