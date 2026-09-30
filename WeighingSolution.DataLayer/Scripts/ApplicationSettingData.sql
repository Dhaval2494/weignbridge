
IF COL_LENGTH('dbo.CustomField', 'DefaultValue') IS NULL
BEGIN
    ALTER TABLE [dbo].[CustomField]
    ADD [DefaultValue] [nvarchar](50) NULL;
END
GO

IF COL_LENGTH('dbo.CustomField', 'Editable') IS NULL
BEGIN
    ALTER TABLE [dbo].[CustomField]
    ADD [Editable] [bit] NOT NULL
        CONSTRAINT [DF_CustomField_Editable] DEFAULT (1);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.TransactionData')
      AND name = 'PushAPI'
)
BEGIN
    ALTER TABLE dbo.TransactionData
    ADD PushAPI bit NOT NULL
        CONSTRAINT DF_TransactionData_PushAPI DEFAULT (0);
END
GO


IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE Name = N'AttachReceiptPdf' AND Object_ID = Object_ID(N'dbo.ApiIntegrationMaster')
)
BEGIN
    ALTER TABLE dbo.ApiIntegrationMaster
    ADD AttachReceiptPdf BIT NOT NULL CONSTRAINT DF_ApiIntegrationMaster_AttachReceiptPdf DEFAULT (0);
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'IsManualSendAvailable' AND Object_ID = Object_ID(N'dbo.ApiIntegrationMaster'))
    ALTER TABLE dbo.ApiIntegrationMaster ADD IsManualSendAvailable BIT NOT NULL CONSTRAINT DF_AIM_IsManualSendAvailable DEFAULT (0);

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'ManualSendLabel' AND Object_ID = Object_ID(N'dbo.ApiIntegrationMaster'))
    ALTER TABLE dbo.ApiIntegrationMaster ADD ManualSendLabel NVARCHAR(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'RequiresManualNumber' AND Object_ID = Object_ID(N'dbo.ApiIntegrationMaster'))
    ALTER TABLE dbo.ApiIntegrationMaster ADD RequiresManualNumber BIT NOT NULL CONSTRAINT DF_AIM_RequiresManualNumber DEFAULT (0);

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'Plc_Port' AND Object_ID = Object_ID(N'WeightBridge')) 
BEGIN 
ALTER TABLE dbo.WeightBridge ADD Plc_Port nvarchar(10) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'LTG' AND Object_ID = Object_ID(N'WeightBridge'))
BEGIN
    ALTER TABLE dbo.WeightBridge ADD LTG NVARCHAR(10) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'LTR' AND Object_ID = Object_ID(N'WeightBridge'))
BEGIN
    ALTER TABLE dbo.WeightBridge ADD LTR NVARCHAR(10) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'RTG' AND Object_ID = Object_ID(N'WeightBridge'))
BEGIN
    ALTER TABLE dbo.WeightBridge ADD RTG NVARCHAR(10) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'RTR' AND Object_ID = Object_ID(N'WeightBridge'))
BEGIN
    ALTER TABLE dbo.WeightBridge ADD RTR NVARCHAR(10) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'LBO' AND Object_ID = Object_ID(N'WeightBridge'))
BEGIN
    ALTER TABLE dbo.WeightBridge ADD LBO NVARCHAR(10) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'LBC' AND Object_ID = Object_ID(N'WeightBridge'))
BEGIN
    ALTER TABLE dbo.WeightBridge ADD LBC NVARCHAR(10) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'RBO' AND Object_ID = Object_ID(N'WeightBridge'))
BEGIN
    ALTER TABLE dbo.WeightBridge ADD RBO NVARCHAR(10) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'RBC' AND Object_ID = Object_ID(N'WeightBridge'))
BEGIN
    ALTER TABLE dbo.WeightBridge ADD RBC NVARCHAR(10) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'JumboDisplay' AND Object_ID = Object_ID(N'WeightBridge'))
BEGIN
    ALTER TABLE dbo.WeightBridge ADD [JumboDisplay] BIT NULL DEFAULT ((0))
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'Modbus_ISBypass' AND Object_ID = Object_ID(N'WeightBridge'))
BEGIN
    ALTER TABLE dbo.WeightBridge ADD [Modbus_ISBypass] BIT NULL DEFAULT ((0))
END
GO

 IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'WeightSpeed' AND Object_ID = Object_ID(N'WeightBridge'))
BEGIN
    ALTER TABLE dbo.WeightBridge ADD WeightSpeed INT NULL DEFAULT ((0))
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CaptureWeightCount' AND Object_ID = Object_ID(N'WeightBridge'))
BEGIN
    ALTER TABLE dbo.WeightBridge ADD CaptureWeightCount INT NULL    
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'MinimumWeight' AND Object_ID = Object_ID(N'WeightBridge'))
BEGIN
    ALTER TABLE dbo.WeightBridge ADD MinimumWeight INT NULL DEFAULT ((0))   
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'AllowCycleTime' AND Object_ID = Object_ID(N'WeightBridge'))
BEGIN
    ALTER TABLE dbo.WeightBridge ADD AllowCycleTime INT NULL DEFAULT ((0))

   
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'StableWeightCount' AND Object_ID = Object_ID(N'WeightBridge'))
BEGIN
    ALTER TABLE dbo.WeightBridge ADD StableWeightCount INT NULL DEFAULT ((0))    
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CycleEndTime' AND Object_ID = Object_ID(N'WeightBridge'))
BEGIN
    ALTER TABLE dbo.WeightBridge ADD CycleEndTime INT NULL DEFAULT ((0))

   
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'WeightBypass' AND Object_ID = Object_ID(N'WeightBridge'))
BEGIN
    ALTER TABLE dbo.WeightBridge ADD [WeightBypass] BIT NULL DEFAULT ((0))    
END
GO


IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'JumboDisplay_Port' AND Object_ID = Object_ID(N'WeightBridge')) 
BEGIN 
ALTER TABLE dbo.WeightBridge ADD JumboDisplay_Port nvarchar(20) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'JumboDisplay_BaudRate' AND Object_ID = Object_ID(N'WeightBridge')) 
BEGIN 
ALTER TABLE dbo.WeightBridge ADD JumboDisplay_BaudRate nvarchar(20) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'Region' AND Object_ID = Object_ID(N'WeightBridge')) 
BEGIN 
ALTER TABLE dbo.WeightBridge ADD Region nvarchar(100) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'LiveWeight' AND Object_ID = Object_ID(N'WeightBridge')) 
BEGIN 
ALTER TABLE dbo.WeightBridge ADD LiveWeight decimal(18, 4) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'Plc_Port' AND Object_ID = Object_ID(N'WeightBridge')) 
BEGIN 
ALTER TABLE dbo.WeightBridge ADD Plc_Port nvarchar(10) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'IsDRT' AND Object_ID = Object_ID(N'WeightBridge')) 
BEGIN 
ALTER TABLE dbo.WeightBridge ADD IsDRT bit NOT NULL CONSTRAINT DF_WeightBridge_IsDRT DEFAULT (0);
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'IsRTS' AND Object_ID = Object_ID(N'WeightBridge')) 
BEGIN 
ALTER TABLE dbo.WeightBridge ADD IsRTS bit NOT NULL CONSTRAINT DF_WeightBridge_IsRTS DEFAULT (0);
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'TareWeightTime' AND Object_ID = Object_ID(N'Vehicle')) 
BEGIN 
ALTER TABLE dbo.Vehicle ADD [TareWeightTime] datetime NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'IsAnpr' AND Object_ID = Object_ID(N'Camera')) 
BEGIN 
ALTER TABLE dbo.Camera ADD [IsAnpr] [bit] NULL
END
GO

IF EXISTS (SELECT * FROM Camera WHERE IsAnpr is null) 
update Camera set IsAnpr = 0 where IsAnpr is null
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'VideosourceMainURL' AND Object_ID = Object_ID(N'Camera')) 
BEGIN 
ALTER TABLE dbo.Camera ADD [VideosourceMainURL] [nvarchar](200) NULL
END
GO

IF EXISTS (SELECT * FROM Camera WHERE VideosourceMainURL is null or VideosourceMainURL = '') 
update Camera set VideosourceMainURL = VideosourceURL where VideosourceMainURL is null or VideosourceMainURL = ''
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'WBSide' AND Object_ID = Object_ID(N'Camera')) 
BEGIN 
ALTER TABLE dbo.Camera ADD [WBSide] [nvarchar](50) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'IsBypass' AND Object_ID = Object_ID(N'Camera')) 
BEGIN 
ALTER TABLE dbo.Camera ADD [IsBypass] [bit] NULL
END
GO

IF EXISTS (SELECT * FROM Camera WHERE IsBypass is null) 
update Camera set IsBypass = 0 where IsBypass is null
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'AutoMaticReceipt') 
INSERT INTO ApplicationSettings ([ModuleName], [SettingName], [SettingType], [SettingValue]) VALUES ('GlobalSetting', 'AutoMaticReceipt', 'boolean', 'False')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'CollectToken') 
INSERT INTO ApplicationSettings ([ModuleName], [SettingName], [SettingType], [SettingValue]) VALUES ('GlobalSetting', 'CollectToken', 'boolean', 'False')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'SAPApiBaseUrl')
INSERT INTO ApplicationSettings ([ModuleName], [SettingName], [SettingType], [SettingValue]) VALUES ('GlobalSetting', 'SAPApiBaseUrl', 'String', '')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'EntryDateEditable') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'EntryDateEditable', 'boolean', 'True')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'DisableSpecialCharactersInVehicle') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'DisableSpecialCharactersInVehicle', 'boolean', 'False')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'DisplayLastVehicleTransactionDetails') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'DisplayLastVehicleTransactionDetails', 'boolean', 'False')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'VehicleNumberLength') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'VehicleNumberLength', 'Integer', '13')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'AllowManualEntry') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'AllowManualEntry', 'boolean', 'False')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'DisbleFrom2ndPass') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'DisbleFrom2ndPass', 'boolean', 'True')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'DisplayReceiptAfter1pass') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'DisplayReceiptAfter1pass', 'boolean', 'True')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'TareWeightDiff') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'TareWeightDiff', 'Integer', '1')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'AllowWithoutTareWeightChange') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'AllowWithoutTareWeightChange', 'boolean', 'False')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'PrintSlipNotes') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'PrintSlipNotes', 'boolean', 'False')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'DateFormat') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'DateFormat', 'String', 'dd-MM-yyyy')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'TimeFormat') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'TimeFormat', 'String', 'hh:mm tt')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'DecimalFormat') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'DecimalFormat', 'String', '0.####')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'IsOpenVehicleFO') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'IsOpenVehicleFO', 'boolean', 'True')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'IsOpenTransporterFO') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'IsOpenTransporterFO', 'boolean', 'True')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'IsOpenAccountFO') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'IsOpenAccountFO', 'boolean', 'True')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'IsOpenProductFO') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'IsOpenProductFO', 'boolean', 'True')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'IsOpenDriverFO') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'IsOpenDriverFO', 'boolean', 'True')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'SelectedReceipt') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'SelectedReceipt', 'String', '')
GO


IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'CompanySetting' AND SettingName = 'CompanyName') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('CompanySetting', 'CompanyName', 'String', 'Company Name')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'CompanySetting' AND SettingName = 'Address') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('CompanySetting', 'Address', 'String', 'Address')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'CompanySetting' AND SettingName = 'Phone') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('CompanySetting', 'Phone', 'String', '1234567890')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'CompanySetting' AND SettingName = 'Email') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('CompanySetting', 'Email', 'String', 'ID@server.com')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'CompanySetting' AND SettingName = 'ClientCompanyName') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('CompanySetting', 'ClientCompanyName', 'String', 'Client Company')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'CompanySetting' AND SettingName = 'ClientContactPerson') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('CompanySetting', 'ClientContactPerson', 'String', 'Contact Person')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'CompanySetting' AND SettingName = 'ClientAddress') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('CompanySetting', 'ClientAddress', 'String', 'Client Address')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'CompanySetting' AND SettingName = 'SupportID') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('CompanySetting', 'SupportID', 'String', 'support@mail.com')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'CompanySetting' AND SettingName = 'Logo') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('CompanySetting', 'Logo', 'String', 'logo.png')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'ThemeSetting' AND SettingName = 'Color') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('ThemeSetting', 'Color', 'String', 'Blue')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'ThemeSetting' AND SettingName = 'BaseColor') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('ThemeSetting', 'BaseColor', 'String', 'BaseLight')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'EmailSetting' AND SettingName = 'ServerName') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('EmailSetting', 'ServerName', 'String', 'Localhost')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'EmailSetting' AND SettingName = 'Port') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('EmailSetting', 'Port', 'String', '25')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'EmailSetting' AND SettingName = 'UserName') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('EmailSetting', 'UserName', 'String', '')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'EmailSetting' AND SettingName = 'PassWord') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('EmailSetting', 'PassWord', 'String', '')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'EmailSetting' AND SettingName = 'UseSSL') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('EmailSetting', 'UseSSL', 'String', 'False')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'EmailSetting' AND SettingName = 'EmailFrom') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('EmailSetting', 'EmailFrom', 'String', '')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'EndelEmailSetting' AND SettingName = 'EndelServerName')
INSERT INTO ApplicationSettings ([ModuleName], [SettingName], [SettingType], [SettingValue]) VALUES ('EndelEmailSetting', 'EndelServerName', 'String', 'smtp.gmail.com')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'EndelEmailSetting' AND SettingName = 'EndelPort')
INSERT INTO ApplicationSettings ([ModuleName], [SettingName], [SettingType], [SettingValue]) VALUES ('EndelEmailSetting', 'EndelPort', 'Integer', '587')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'EndelEmailSetting' AND SettingName = 'EndelUserName')
INSERT INTO ApplicationSettings ([ModuleName], [SettingName], [SettingType], [SettingValue]) VALUES ('EndelEmailSetting', 'EndelUserName', 'String', 'Weighmast@gmail.com')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'EndelEmailSetting' AND SettingName = 'EndelPassWord')
INSERT INTO ApplicationSettings ([ModuleName], [SettingName], [SettingType], [SettingValue]) VALUES ('EndelEmailSetting', 'EndelPassWord', 'String', '0lo2xFqzm84mcGHoMYOpjhxVT6ey8d/2')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'EndelEmailSetting' AND SettingName = 'EndelUseSSL')
INSERT INTO ApplicationSettings ([ModuleName], [SettingName], [SettingType], [SettingValue]) VALUES ('EndelEmailSetting', 'EndelUseSSL', 'Boolean', 'TRUE')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'EndelEmailSetting' AND SettingName = 'EndelEmailFrom')
INSERT INTO ApplicationSettings ([ModuleName], [SettingName], [SettingType], [SettingValue]) VALUES ('EndelEmailSetting', 'EndelEmailFrom', 'String', 'Weighmast@gmail.com')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'EndelEmailSetting' AND SettingName = 'EndelEmailTo')
INSERT INTO ApplicationSettings ([ModuleName], [SettingName], [SettingType], [SettingValue]) VALUES ('EndelEmailSetting', 'EndelEmailTo', 'String', 'parimal.makwana@endel.digital')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'DatabaseSetting' AND SettingName = 'AutoBackupEnable') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('DatabaseSetting', 'AutoBackupEnable', 'Boolean', 'False')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'DatabaseSetting' AND SettingName = 'AutoBackupHour') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('DatabaseSetting', 'AutoBackupHour', 'Integer', '1')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'DatabaseSetting' AND SettingName = 'AutoBackupMinute') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('DatabaseSetting', 'AutoBackupMinute', 'Integer', '0')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'DatabaseSetting' AND SettingName = 'AutoBackupMeridiem') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('DatabaseSetting', 'AutoBackupMeridiem', 'String', 'AM')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'DatabaseSetting' AND SettingName = 'AutoBackupFilePath') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('DatabaseSetting', 'AutoBackupFilePath', 'String', '')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'PrintSetting' AND SettingName = 'PrintType')
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('PrintSetting', 'PrintType', 'String', 'Laser')
GO

IF NOT EXISTS(SELECT * FROM ApplicationSettings WHERE ModuleName = 'PrintSetting' AND SettingName = 'PrinterName')
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('PrintSetting', 'PrinterName', 'String', '')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'SMSSetting' AND SettingName = 'ProviderName') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('SMSSetting', 'ProviderName', 'String', '')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'BarcodeSetting' AND SettingName = 'BarcodeOrQR') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('BarcodeSetting', 'BarcodeOrQR', 'String', '') 
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'BarcodeSetting' AND SettingName = 'BarcodeImagePath') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('BarcodeSetting', 'BarcodeImagePath', 'String', 'C:\\ProgramData\\WeighBridgeSolution\\CustomImages\\BarcodeImages') 
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'BarcodeSetting' AND SettingName = 'BarcodeImagePathType')
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('BarcodeSetting', 'BarcodeImagePathType', 'String', 'LocalImagePath')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'Transaction' AND SettingName = 'LastTicket')
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('Transaction', 'LastTicket', 'int', '0')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'SelectedReceipt') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'SelectedReceipt', 'String', '')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'MultiSelectedReceipt') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'MultiSelectedReceipt', 'String', '')
GO


IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'AnprSetting' AND SettingName = 'AnprImagePath') 
INSERT INTO ApplicationSettings ([ModuleName], [SettingName], [SettingType], [SettingValue]) VALUES ('AnprSetting', 'AnprImagePath', 'String', 'C:\\ProgramData\\WeighBridgeSolution\\CustomImages\\TransactionImages') 
GO 

IF NOT EXISTS(SELECT* FROM ApplicationSettings WHERE ModuleName = 'AnprSetting' AND SettingName = 'AnprImagePathType')
INSERT INTO ApplicationSettings ([ModuleName], [SettingName], [SettingType], [SettingValue]) VALUES ('AnprSetting', 'AnprImagePathType', 'String', 'LocalImagePath') 
GO 

IF NOT EXISTS(SELECT* FROM ApplicationSettings WHERE ModuleName = 'SyncDBSetting' AND SettingName = 'DestinationDBConnection')
INSERT INTO ApplicationSettings ([ModuleName], [SettingName], [SettingType], [SettingValue]) VALUES  ('SyncDBSetting', 'DestinationDBConnection', 'String', '') 
GO 

IF NOT EXISTS(SELECT* FROM ApplicationSettings WHERE ModuleName = 'SyncDBSetting' AND SettingName = 'DestinationImagePath')
INSERT INTO ApplicationSettings ([ModuleName], [SettingName], [SettingType], [SettingValue]) VALUES  ('SyncDBSetting', 'DestinationImagePath', 'String', '') 
GO 

IF NOT EXISTS(SELECT* FROM ApplicationSettings WHERE ModuleName = 'SyncDBSetting' AND SettingName = 'FTPUserID')
INSERT INTO ApplicationSettings ([ModuleName], [SettingName], [SettingType], [SettingValue]) VALUES  ('SyncDBSetting', 'FTPUserID', 'String', '') 
GO 

IF NOT EXISTS(SELECT* FROM ApplicationSettings WHERE ModuleName = 'SyncDBSetting' AND SettingName = 'FTPUserPWD')
INSERT INTO ApplicationSettings ([ModuleName], [SettingName], [SettingType], [SettingValue]) VALUES  ('SyncDBSetting', 'FTPUserPWD', 'String', '') 
GO 

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'TokenName' AND Object_ID = Object_ID(N'TransactionData'))
BEGIN ALTER TABLE dbo.TransactionData ADD TokenName nvarchar(250) NULL
END
GO

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TransactionData'AND COLUMN_NAME = 'TokenName' AND CHARACTER_MAXIMUM_LENGTH <> 250)
BEGIN ALTER TABLE dbo.TransactionData ALTER COLUMN TokenName NVARCHAR(250) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'TokenValue' AND Object_ID = Object_ID(N'TransactionData'))
BEGIN ALTER TABLE dbo.TransactionData ADD TokenValue nvarchar(250) NULL
END
GO

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TransactionData'AND COLUMN_NAME = 'TokenValue' AND CHARACTER_MAXIMUM_LENGTH <> 250)
BEGIN ALTER TABLE dbo.TransactionData ALTER COLUMN TokenValue NVARCHAR(250) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'Vehicle_Number' AND Object_ID = Object_ID(N'TransactionData'))
BEGIN ALTER TABLE dbo.TransactionData ADD Vehicle_Number nvarchar(250) NULL
END
GO

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TransactionData'AND COLUMN_NAME = 'Vehicle_Number' AND CHARACTER_MAXIMUM_LENGTH <> 250)
BEGIN ALTER TABLE dbo.TransactionData ALTER COLUMN Vehicle_Number NVARCHAR(250) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'Driver_Name' AND Object_ID = Object_ID(N'TransactionData'))
BEGIN ALTER TABLE dbo.TransactionData ADD Driver_Name nvarchar(250) NULL
END
GO

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TransactionData'AND COLUMN_NAME = 'Driver_Name' AND CHARACTER_MAXIMUM_LENGTH <> 250)
BEGIN ALTER TABLE dbo.TransactionData ALTER COLUMN Driver_Name NVARCHAR(250) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'Account_Name' AND Object_ID = Object_ID(N'TransactionData'))
BEGIN ALTER TABLE dbo.TransactionData ADD Account_Name nvarchar(250) NULL
END
GO

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TransactionData'AND COLUMN_NAME = 'Account_Name' AND CHARACTER_MAXIMUM_LENGTH <> 250)
BEGIN ALTER TABLE dbo.TransactionData ALTER COLUMN Account_Name NVARCHAR(250) NULL
END
GO


IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'Transporter_Name' AND Object_ID = Object_ID(N'TransactionData'))
BEGIN ALTER TABLE dbo.TransactionData ADD Transporter_Name nvarchar(250) NULL
END
GO

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TransactionData'AND COLUMN_NAME = 'Transporter_Name' AND CHARACTER_MAXIMUM_LENGTH <> 250)
BEGIN ALTER TABLE dbo.TransactionData ALTER COLUMN Transporter_Name NVARCHAR(250) NULL
END
GO


IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'WeightBridgeName' AND Object_ID = Object_ID(N'TransactionDetail'))
BEGIN ALTER TABLE dbo.TransactionDetail ADD WeightBridgeName nvarchar(250) NULL
END
GO

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TransactionDetail'AND COLUMN_NAME = 'WeightBridgeName' AND CHARACTER_MAXIMUM_LENGTH <> 250)
BEGIN ALTER TABLE dbo.TransactionDetail ALTER COLUMN WeightBridgeName NVARCHAR(250) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'Product_Name' AND Object_ID = Object_ID(N'TransactionDetail'))
BEGIN ALTER TABLE dbo.TransactionDetail ADD Product_Name nvarchar(250) NULL
END
GO

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TransactionDetail'AND COLUMN_NAME = 'Product_Name' AND CHARACTER_MAXIMUM_LENGTH <> 250)
BEGIN ALTER TABLE dbo.TransactionDetail ALTER COLUMN Product_Name NVARCHAR(250) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'Gate_Name' AND Object_ID = Object_ID(N'TransactionDetail'))
BEGIN ALTER TABLE dbo.TransactionDetail ADD Gate_Name nvarchar(250) NULL
END
GO

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TransactionDetail'AND COLUMN_NAME = 'Gate_Name' AND CHARACTER_MAXIMUM_LENGTH <> 250)
BEGIN ALTER TABLE dbo.TransactionDetail ALTER COLUMN Gate_Name NVARCHAR(250) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'WB_User_Name' AND Object_ID = Object_ID(N'TransactionDetail'))
BEGIN ALTER TABLE dbo.TransactionDetail ADD WB_User_Name nvarchar(250) NULL
END
GO

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TransactionDetail'AND COLUMN_NAME = 'WB_User_Name' AND CHARACTER_MAXIMUM_LENGTH <> 250)
BEGIN ALTER TABLE dbo.TransactionDetail ALTER COLUMN WB_User_Name NVARCHAR(250) NULL
END
GO



IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'SMSEnd' AND Object_ID = Object_ID(N'SMSSetup'))
BEGIN ALTER TABLE dbo.SMSSetup ADD SMSEnd nvarchar(MAX) NULL
END
GO


IF NOT EXISTS (SELECT * FROM [dbo].[Feature] WHERE [FeatureName] = 'EmailSetup') 
BEGIN
SET IDENTITY_INSERT [dbo].[Feature] ON
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (39, N'EmailSetup',N'CRU',CAST(4.07 AS Decimal(5,3)),32,0)
SET IDENTITY_INSERT [dbo].[Feature] OFF 
END
GO


IF NOT EXISTS (SELECT * FROM [dbo].[Feature] WHERE [FeatureName] = 'PrintSetting') 
BEGIN
SET IDENTITY_INSERT [dbo].[Feature] ON
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (40, N'PrintSetting',N'CRU',CAST(4.07 AS Decimal(5,3)),32,0)
SET IDENTITY_INSERT [dbo].[Feature] OFF
END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Feature] WHERE [FeatureName] = 'SMSSetup') 
BEGIN
SET IDENTITY_INSERT [dbo].[Feature] ON
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (41, N'SMSSetup',N'CRU',CAST(4.07 AS Decimal(5,3)),32,0)
SET IDENTITY_INSERT [dbo].[Feature] OFF
END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Feature] WHERE [FeatureName] = 'BarcodeSetup') 
BEGIN
SET IDENTITY_INSERT [dbo].[Feature] ON
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (42, N'BarcodeSetup',N'CRU',CAST(4.07 AS Decimal(5,3)),32,0)
SET IDENTITY_INSERT [dbo].[Feature] OFF
END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Feature] WHERE [FeatureName] = 'Camera') 
BEGIN
SET IDENTITY_INSERT [dbo].[Feature] ON
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (43, N'Camera',N'CRU',CAST(4.07 AS Decimal(5,3)),7,0)
SET IDENTITY_INSERT [dbo].[Feature] OFF
END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Feature] WHERE [FeatureName] = 'UHF') 
BEGIN
SET IDENTITY_INSERT [dbo].[Feature] ON
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (44, N'UHF',N'CRU',CAST(4.07 AS Decimal(5,3)),32,0)
SET IDENTITY_INSERT [dbo].[Feature] OFF
END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Feature] WHERE [FeatureName] = 'Token') 
BEGIN
SET IDENTITY_INSERT [dbo].[Feature] ON
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (45, N'Token',N'CRU',CAST(4.07 AS Decimal(5,3)),32,0)
SET IDENTITY_INSERT [dbo].[Feature] OFF
END
GO



IF NOT EXISTS (SELECT * FROM TemplatePermission WHERE FeatureID = 39) 
BEGIN
INSERT INTO TemplatePermission VALUES(39,1,N'11111')
INSERT INTO TemplatePermission VALUES (39, 2, N'00000')
END
GO

IF NOT EXISTS (SELECT * FROM TemplatePermission WHERE FeatureID = 40) 
BEGIN
INSERT INTO TemplatePermission VALUES(40,1,N'11111')
INSERT INTO TemplatePermission VALUES (40, 2, N'00000')
END
GO

IF NOT EXISTS (SELECT * FROM TemplatePermission WHERE FeatureID = 41) 
BEGIN
INSERT INTO TemplatePermission VALUES(41,1,N'11111')
INSERT INTO TemplatePermission VALUES (41, 2, N'00000')
END
GO

IF NOT EXISTS (SELECT * FROM TemplatePermission WHERE FeatureID = 42) 
BEGIN
INSERT INTO TemplatePermission VALUES(42,1,N'11111')
INSERT INTO TemplatePermission VALUES (42, 2, N'00000')
END
GO

IF NOT EXISTS (SELECT * FROM TemplatePermission WHERE FeatureID = 43) 
BEGIN
INSERT INTO TemplatePermission VALUES(43,1,N'11111')
INSERT INTO TemplatePermission VALUES (43, 2, N'00000')
END
GO

IF NOT EXISTS (SELECT * FROM TemplatePermission WHERE FeatureID = 44) 
BEGIN
INSERT INTO TemplatePermission VALUES(44,1,N'11111')
INSERT INTO TemplatePermission VALUES (44, 2, N'00000')
END
GO

IF NOT EXISTS (SELECT * FROM TemplatePermission WHERE FeatureID = 45) 
BEGIN
INSERT INTO TemplatePermission VALUES(45,1,N'11111')
INSERT INTO TemplatePermission VALUES (45, 2, N'00000')
END
GO



IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'LiveWeightSetting' AND SettingName = 'ApiUrl')
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('LiveWeightSetting', 'ApiUrl', 'string', '')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'LiveWeightSetting' AND SettingName = 'WBID')
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('LiveWeightSetting', 'WBID', 'string', '')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'LiveWeightSetting' AND SettingName = 'PassKey')
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('LiveWeightSetting', 'PassKey', 'string', '')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'AnprApiUrl')
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'AnprApiUrl', 'string', '')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'AnprPassKey')
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'AnprPassKey', 'string', '')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'PreFixTicket')
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'PreFixTicket', 'string', '')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'PostFixTicket')
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'PostFixTicket', 'string', '')
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'DisplayTicketID' AND Object_ID = Object_ID(N'TransactionData')) 
BEGIN 
ALTER TABLE dbo.TransactionData ADD [DisplayTicketID] [nvarchar](50) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'CustomValue1' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD CustomValue1 Nvarchar(MAX) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'CustomValue2' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD CustomValue2 Nvarchar(MAX) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'CustomValue3' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD CustomValue3 Nvarchar(MAX) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'CustomValue4' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD CustomValue4 Nvarchar(MAX) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'CustomValue5' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD CustomValue5 Nvarchar(MAX) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'CustomValue6' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD CustomValue6 Nvarchar(MAX) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'CustomValue7' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD CustomValue7 Nvarchar(MAX) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'CustomValue8' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD CustomValue8 Nvarchar(MAX) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'CustomValue9' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD CustomValue9 Nvarchar(MAX) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'CustomValue10' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD CustomValue10 Nvarchar(MAX) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'CustomValue11' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD CustomValue11 Nvarchar(MAX) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'CustomValue12' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD CustomValue12 Nvarchar(MAX) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'CustomValue13' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD CustomValue13 Nvarchar(MAX) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'CustomValue14' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD CustomValue14 Nvarchar(MAX) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'CustomValue15' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD CustomValue15 Nvarchar(MAX) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'CustomValue16' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD CustomValue16 Nvarchar(MAX) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'CustomValue17' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD CustomValue17 Nvarchar(MAX) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'CustomValue18' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD CustomValue18 Nvarchar(MAX) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'CustomValue19' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD CustomValue19 Nvarchar(MAX) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'CustomValue20' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD CustomValue20 Nvarchar(MAX) NULL 
END
GO


IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CustomValue1' AND Object_ID = Object_ID(N'dbo.TransactionData'))
BEGIN
ALTER TABLE dbo.TransactionData ADD CustomValue1 nvarchar(max) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CustomValue2' AND Object_ID = Object_ID(N'dbo.TransactionData'))
BEGIN
ALTER TABLE dbo.TransactionData ADD CustomValue2 nvarchar(max) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CustomValue3' AND Object_ID = Object_ID(N'dbo.TransactionData'))
BEGIN
ALTER TABLE dbo.TransactionData ADD CustomValue3 nvarchar(max) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CustomValue4' AND Object_ID = Object_ID(N'dbo.TransactionData'))
BEGIN
ALTER TABLE dbo.TransactionData ADD CustomValue4 nvarchar(max) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CustomValue5' AND Object_ID = Object_ID(N'dbo.TransactionData'))
BEGIN
ALTER TABLE dbo.TransactionData ADD CustomValue5 nvarchar(max) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CustomValue6' AND Object_ID = Object_ID(N'dbo.TransactionData'))
BEGIN
ALTER TABLE dbo.TransactionData ADD CustomValue6 nvarchar(max) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CustomValue7' AND Object_ID = Object_ID(N'dbo.TransactionData'))
BEGIN
ALTER TABLE dbo.TransactionData ADD CustomValue7 nvarchar(max) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CustomValue8' AND Object_ID = Object_ID(N'dbo.TransactionData'))
BEGIN
ALTER TABLE dbo.TransactionData ADD CustomValue8 nvarchar(max) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CustomValue9' AND Object_ID = Object_ID(N'dbo.TransactionData'))
BEGIN
ALTER TABLE dbo.TransactionData ADD CustomValue9 nvarchar(max) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CustomValue10' AND Object_ID = Object_ID(N'dbo.TransactionData'))
BEGIN
ALTER TABLE dbo.TransactionData ADD CustomValue10 nvarchar(max) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CustomValue11' AND Object_ID = Object_ID(N'dbo.TransactionData'))
BEGIN
ALTER TABLE dbo.TransactionData ADD CustomValue11 nvarchar(max) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CustomValue12' AND Object_ID = Object_ID(N'dbo.TransactionData'))
BEGIN
ALTER TABLE dbo.TransactionData ADD CustomValue12 nvarchar(max) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CustomValue13' AND Object_ID = Object_ID(N'dbo.TransactionData'))
BEGIN
ALTER TABLE dbo.TransactionData ADD CustomValue13 nvarchar(max) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CustomValue14' AND Object_ID = Object_ID(N'dbo.TransactionData'))
BEGIN
    ALTER TABLE dbo.TransactionData ADD CustomValue14 nvarchar(max) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CustomValue15' AND Object_ID = Object_ID(N'dbo.TransactionData'))
BEGIN
ALTER TABLE dbo.TransactionData ADD CustomValue15 nvarchar(max) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CustomValue16' AND Object_ID = Object_ID(N'dbo.TransactionData'))
BEGIN
ALTER TABLE dbo.TransactionData ADD CustomValue16 nvarchar(max) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CustomValue17' AND Object_ID = Object_ID(N'dbo.TransactionData'))
BEGIN
ALTER TABLE dbo.TransactionData ADD CustomValue17 nvarchar(max) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CustomValue18' AND Object_ID = Object_ID(N'dbo.TransactionData'))
BEGIN
ALTER TABLE dbo.TransactionData ADD CustomValue18 nvarchar(max) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CustomValue19' AND Object_ID = Object_ID(N'dbo.TransactionData'))
BEGIN
ALTER TABLE dbo.TransactionData ADD CustomValue19 nvarchar(max) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CustomValue20' AND Object_ID = Object_ID(N'dbo.TransactionData'))
BEGIN
ALTER TABLE dbo.TransactionData ADD CustomValue20 nvarchar(max) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CreatedName' AND Object_ID = Object_ID(N'dbo.TransactionData'))
BEGIN
ALTER TABLE dbo.TransactionData ADD CreatedName nvarchar(250) NULL
END
GO

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TransactionData'AND COLUMN_NAME = 'CreatedName' AND CHARACTER_MAXIMUM_LENGTH <> 250)
BEGIN ALTER TABLE dbo.TransactionData ALTER COLUMN CreatedName NVARCHAR(250) NULL
END
GO


IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'RunCustomValue') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'RunCustomValue', 'int', '0')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'CameraSetting' AND SettingName = 'CameraImagePath')
INSERT INTO ApplicationSettings ([ModuleName] ,[SettingName],[SettingType],[SettingValue]) VALUES ('CameraSetting', 'CameraImagePath', 'String', 'C:\\ProgramData\\WeighBridgeSolution\\CustomImages\\TransactionImages') 
GO

IF NOT EXISTS(SELECT* FROM ApplicationSettings WHERE ModuleName = 'CameraSetting' AND SettingName = 'CameraImagePathType')
INSERT INTO ApplicationSettings ([ModuleName] ,[SettingName],[SettingType],[SettingValue]) VALUES ('CameraSetting', 'CameraImagePathType', 'String', 'LocalImagePath') 
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Feature] WHERE [FeatureName] = 'Camera') 
BEGIN
SET IDENTITY_INSERT [dbo].[Feature] ON
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (43, N'Camera',N'CRU',CAST(4.07 AS Decimal(5,3)),32,0)
SET IDENTITY_INSERT [dbo].[Feature] OFF
END
GO

IF NOT EXISTS (SELECT * FROM TemplatePermission WHERE FeatureID = 43) 
BEGIN
INSERT INTO TemplatePermission VALUES(43,1,N'11111')
INSERT INTO TemplatePermission VALUES (43, 2, N'11111')
END
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'CollectToken') 
INSERT INTO ApplicationSettings ([ModuleName], [SettingName], [SettingType], [SettingValue])  VALUES ('GlobalSetting', 'CollectToken', 'boolean', 'False')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'ApiBaseUrl')
INSERT INTO ApplicationSettings ([ModuleName], [SettingName], [SettingType], [SettingValue]) VALUES ('GlobalSetting', 'ApiBaseUrl', 'String', 'http://115.243.51.136:50000/')
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Endel_ANPR]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Endel_ANPR](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[AnprNumber] [nvarchar](50) NOT NULL,
	[ActualNumber] [nvarchar](50) NOT NULL,
	[ImageName] [nvarchar](50) NOT NULL,
	[CameraID] [int] NOT NULL,
	[CameraName] [nvarchar](50) NULL,
	[WBName] [nvarchar](50) NULL,
	[CaptureTime] [datetime] NOT NULL,
	[MatchPer] [int] NOT NULL,
	[MatchResult] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Endel_ANPR] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UhfMaster]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[UhfMaster](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[IpAddress] [nvarchar](50) NOT NULL,
	[Port] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[WeightbridgeID] [int] NOT NULL,
	[WeightbridgeSide] [nvarchar](50) NULL,
	[ReaderData] [nvarchar](max) NULL,
	[LastRead] [datetime] NULL,
	[IsRunning] [bit] NULL,
	[UHFType] [nvarchar](50) NULL,
 CONSTRAINT [PK_UhfMaster] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


ALTER TABLE [dbo].[UhfMaster]  WITH CHECK ADD  CONSTRAINT [FK_UhfMaster_WeightBridge] FOREIGN KEY([WeightbridgeID])
REFERENCES [dbo].[WeightBridge] ([WeightBridgeID])


ALTER TABLE [dbo].[UhfMaster] CHECK CONSTRAINT [FK_UhfMaster_WeightBridge]
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'UHFType' AND Object_ID = Object_ID(N'UhfMaster')) 
BEGIN 
ALTER TABLE dbo.UhfMaster ADD [UHFType] [nvarchar](50) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'StringPrifix' AND Object_ID = Object_ID(N'UhfMaster')) 
BEGIN 
ALTER TABLE dbo.UhfMaster ADD [StringPrifix] [nvarchar](50) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'ISBypass' AND Object_ID = Object_ID(N'UhfMaster')) 
BEGIN 
ALTER TABLE dbo.UhfMaster ADD [ISBypass] [bit] NOT NULL DEFAULT ((0))
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'LeftPlateImageName' AND Object_ID = Object_ID(N'TransactionDetail')) 
BEGIN 
ALTER TABLE dbo.TransactionDetail ADD [LeftPlateImageName] [nvarchar](100) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'LeftAnprNumber' AND Object_ID = Object_ID(N'TransactionDetail')) 
BEGIN 
ALTER TABLE dbo.TransactionDetail ADD [LeftAnprNumber] [nvarchar](100) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'RightPlateImageName' AND Object_ID = Object_ID(N'TransactionDetail')) 
BEGIN 
ALTER TABLE dbo.TransactionDetail ADD [RightPlateImageName] [nvarchar](100) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'RightAnprNumber' AND Object_ID = Object_ID(N'TransactionDetail')) 
BEGIN 
ALTER TABLE dbo.TransactionDetail ADD [RightAnprNumber] [nvarchar](100) NULL
END
GO



IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'RFID_EPC' AND Object_ID = Object_ID(N'vehicle')) 
BEGIN 
ALTER TABLE dbo.vehicle ADD [RFID_EPC] [nvarchar](100) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'WeightBridgeName' AND Object_ID = Object_ID(N'TransactionDetail'))
BEGIN ALTER TABLE dbo.TransactionDetail ADD WeightBridgeName nvarchar(250) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'Product_Name' AND Object_ID = Object_ID(N'TransactionDetail'))
BEGIN ALTER TABLE dbo.TransactionDetail ADD Product_Name nvarchar(250) NULL
END
GO


IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'Gate_Name' AND Object_ID = Object_ID(N'TransactionDetail'))
BEGIN ALTER TABLE dbo.TransactionDetail ADD Gate_Name nvarchar(250) NULL
END
GO


IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'WB_User_Name' AND Object_ID = Object_ID(N'TransactionDetail'))
BEGIN ALTER TABLE dbo.TransactionDetail ADD WB_User_Name nvarchar(250) NULL
END
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'RunCustomValue') 
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'RunCustomValue', 'int', '0')
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'Vehicle_Number' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD Vehicle_Number Nvarchar(250) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'Driver_Name' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD Driver_Name Nvarchar(250) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'Account_Name' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD Account_Name Nvarchar(250) NULL 
END
GO


IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'Transporter_Name' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD Transporter_Name Nvarchar(250) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'Product_Name' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD Product_Name Nvarchar(250) NULL 
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'Gate_Name' AND Object_ID = Object_ID(N'Token')) 
BEGIN ALTER TABLE dbo.Token ADD Gate_Name Nvarchar(250) NULL 
END
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[APILog]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[APILog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RequestUrl] [nvarchar](max) NULL,
	[RequestData] [nvarchar](max) NULL,
	[RequestData_time] [datetime] NULL,
	[ResponseData] [nvarchar](max) NULL,
	[ResponseData_time] [datetime] NULL,
 CONSTRAINT [PK_APILog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO