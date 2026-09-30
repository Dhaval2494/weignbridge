namespace WeighingSolution.DataLayer
{
    public enum TransactionType
    {
        Outgoing,
        Incoming,
        Both
    }
    public enum Operations
    {
        INSERT,
        UPDATE,
        DELETE
    }
    public enum WeighmentType
    {
        Gross = 1,
        Tare = 2
    }
    public enum TransactionMode
    {
        Single,
        Double,
        Multi
    }
    public enum WeighType
    {
        WeighBridge,
        WeighingScale
    }
    public enum TransactionStatus
    {
        Active,
        Complete,
        Void
    }
    public enum AppMode
    {
        Test,
        Live
    }

    public enum MasterFieldEnum
    {
        //Vehicle,
        Account,
        Transporter,
        Driver,
        Product,
        Gate,
        Charges
    }
    public enum PageEntity
    {
        TransactionManagement = 1,
        Transaction = 2,
        PrintReceipt = 3,
        VoidTransaction = 4,
        AllowManualEntry = 5,
        PrintStarForManualEntry = 6,
        DataSetup = 7,
        UserRole = 8,
        User = 9,
        Unit = 10,
        WeighBridge = 11,
        Account = 12,
        Vehicle = 13,
        Driver = 14,
        Product = 15,
        Gate = 16,
        Reports = 17,
        MasterReport = 18,
        UserRoleReport = 19,
        UserReport = 20,
        UnitReport = 21,
        WeighBridgeReport = 22,
        AccountReport = 23,
        VehicleReport = 24,
        DriverReport = 25,
        ProductReport = 26,
        GateReport = 27,
        AuditReport = 28,
        TransactionReport = 29,
        TranSummaryReport = 30,
        TranDetailReport = 31,
        Settings = 32,
        DatabaseSetting = 33,
        GlobalSetting = 34,
        CompanySetting = 35,
        FieldSetting = 36,
        PluginManagement = 37,
        LicenseManagement = 38,
        EmailSetup = 39,
        PrintSetting = 40,
        SMSSetup = 41,
        BarcodeSetup = 42,
        Camera = 43,
        UHF = 44,
        Token = 45,
        API = 45
    }
    public enum PageAccess
    {
        Create = 1,
        Read = 2,
        Update = 3,
        Delete = 4,
        Execute = 5
    }
    public enum PrintType
    {
        Laser,
        DotMatrix
    }

    public enum PluginNameEnum
    {
        AutoBackupPlugin,
        BarcodePlugin,
        CameraPlugin,
        EmailPlugin,
        MultiWeighBridgePlugin,
        UnmannedPlugin,
        SyncPlugin,
        SMSPlugin,
        ANPRPlugin,
    }

    public enum PLCLOGCODEEnum
    {
        CardPunch = 1,
        Tokenstatus = 2,
        Cyclestart = 3,
        BoomBarrierINopentime = 4,
        Sensorintime = 5,
        mainsensor = 6,
        BoomBarrierINclosetime = 7,
        BoomBarrierINstatus = 8,
        Weightstarttime = 9,
        Weightdonetime = 10,
        Weightstatus = 11,
        BoomBarrierOUTopentime = 12,
        SensorOutTime = 13,
        BoomBarrierOUTclosetime = 14,
        BoomBarrierOUTStatus = 15
    }
}
