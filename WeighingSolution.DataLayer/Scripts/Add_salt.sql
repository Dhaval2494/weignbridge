BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.UserMaster ADD
	Salt nvarchar(500) NULL
GO
ALTER TABLE dbo.UserMaster SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.UserMaster_Tracking ADD
	Salt_old nvarchar(500) NULL,
	Salt_new nvarchar(500) NULL
GO
ALTER TABLE dbo.UserMaster_Tracking SET (LOCK_ESCALATION = TABLE)
GO

ALTER TRIGGER [dbo].[TRACKUSERAUDIT_USERMASTER_DELETE] ON [dbo].[UserMaster] FOR DELETE AS DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;
SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())
IF @UserID > 0 BEGIN SELECT @UserName = UserName FROM [UserMaster] WHERE UserID = @UserID END ELSE BEGIN SET @UserName = 'System Service' END
INSERT INTO [dbo].[UserMaster_Tracking] (TranDate, Operation,UserActivity, TranUserID,TranEnvironment,
  [UserID_old] , [UserID_new] ,
  [FirstName_old] , [FirstName_new] ,
  [LastName_old] , [LastName_new] ,
  [ContactNo_old] , [ContactNo_new] ,
  [Email_old] , [Email_new] ,
  [Address_old] , [Address_new] ,
  [Notes_old] , [Notes_new] ,
  [UserName_old] , [UserName_new] ,
  [Password_old] , [Password_new] ,
  [Active_old] , [Active_new] ,
  [Salt_old] , [Salt_new] 
) SELECT GetDate(), 'DELETE', 'User Detail information deleted by : ' + @UserName + ' at : ' + CONVERT(VARCHAR(10), GETDATE(), 103) ,dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()),
  [UserID] , [UserID] ,
  [FirstName] , [FirstName] ,
  [LastName] , [LastName] ,
  [ContactNo] , [ContactNo] ,
  [Email] , [Email] ,
  [Address] , [Address] ,
  [Notes] , [Notes] ,
  [UserName] , [UserName] ,
  [Password] , [Password] ,
  [Active] , [Active]   ,
  [Salt] , [Salt] 
FROM Deleted 
GO

ALTER TRIGGER [dbo].[TRACKUSERAUDIT_USERMASTER_INSERT] ON [dbo].[UserMaster] FOR INSERT AS DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;
SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())
IF @UserID > 0 BEGIN SELECT @UserName = UserName FROM [UserMaster] WHERE UserID = @UserID END ELSE BEGIN SET @UserName = 'System Service' END
INSERT INTO [dbo].[UserMaster_Tracking] (TranDate, Operation,UserActivity, TranUserID,TranEnvironment,
  [UserID_old] , [UserID_new] ,
  [FirstName_old] , [FirstName_new] ,
  [LastName_old] , [LastName_new] ,
  [ContactNo_old] , [ContactNo_new] ,
  [Email_old] , [Email_new] ,
  [Address_old] , [Address_new] ,
  [Notes_old] , [Notes_new] ,
  [UserName_old] , [UserName_new] ,
  [Password_old] , [Password_new] ,
  [Active_old] , [Active_new] ,
  [Salt_old] , [Salt_new] 
) SELECT GetDate(), 'INSERT', 'User Detail information created by : ' + @UserName + ' at : ' + CONVERT(VARCHAR(10), GETDATE(), 103) ,dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()),
  [UserID] , [UserID] ,
  [FirstName] , [FirstName] ,
  [LastName] , [LastName] ,
  [ContactNo] , [ContactNo] ,
  [Email] , [Email] ,
  [Address] , [Address] ,
  [Notes] , [Notes] ,
  [UserName] , [UserName] ,
  [Password] , [Password] ,
  [Active] , [Active] ,
  [Salt] , [Salt] 
FROM Inserted 
GO

ALTER TRIGGER [dbo].[TRACKUSERAUDIT_USERMASTER_UPDATE] ON [dbo].[UserMaster] FOR UPDATE AS DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;
SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())
IF @UserID > 0 BEGIN SELECT @UserName = UserName FROM [UserMaster] WHERE UserID = @UserID END ELSE BEGIN SET @UserName = 'System Service' END
INSERT INTO [dbo].[UserMaster_Tracking] (TranDate, Operation,UserActivity, TranUserID, TranEnvironment,
  [UserID_old] , [UserID_new] ,
  [FirstName_old] , [FirstName_new] ,
  [LastName_old] , [LastName_new] ,
  [ContactNo_old] , [ContactNo_new] ,
  [Email_old] , [Email_new] ,
  [Address_old] , [Address_new] ,
  [Notes_old] , [Notes_new] ,
  [UserName_old] , [UserName_new] ,
  [Password_old] , [Password_new] ,
  [Active_old] , [Active_new]   ,
  [Salt_old] , [Salt_new] 
) SELECT GetDate(), 'UPDATE', 'User Detail information modified by : ' + @UserName + ' at : ' + CONVERT(VARCHAR(10), GETDATE(), 103) ,dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()),
  D.[UserID] , I.[UserID] ,
  D.[FirstName] , I.[FirstName] ,
  D.[LastName] , I.[LastName] ,
  D.[ContactNo] , I.[ContactNo] ,
  D.[Email] , I.[Email] ,
  D.[Address] , I.[Address] ,
  D.[Notes] , I.[Notes] ,
  D.[UserName] , I.[UserName] ,
  D.[Password] , I.[Password] ,
  D.[Active] , I.[Active] ,
  D.[Salt] , I.[Salt] 
FROM Deleted D INNER JOIN Inserted I ON I.UserID = D.UserID
GO

IF NOT EXISTS (SELECT * FROM UserMaster WHERE UserName = 'endelsupport') 
INSERT INTO UserMaster([FirstName],[LastName],[ContactNo],[Email],[Address],[Notes],[UserName],[Password],[Active],[TemplateID],[Salt]) 
VALUES('EndelSupport','User',NULL,NULL,NUll,NUll,'endelsupport','drGhifYnz+KI9nYIj/XGyGD5WHODs27x',1,1,'YA6a0wIlIlVHqIr6Cl9QKClEL0NzugdIQiGu1mONtnQ=')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'EntryDateEditable') 
INSERT INTO ApplicationSettings VALUES ('GlobalSetting', 'EntryDateEditable', 'boolean', 'True')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'AllowManualEntry') 
INSERT INTO ApplicationSettings VALUES ('GlobalSetting', 'AllowManualEntry', 'boolean', 'False')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'DisbleFrom2ndPass') 
INSERT INTO ApplicationSettings VALUES ('GlobalSetting', 'DisbleFrom2ndPass', 'boolean', 'True')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'DisplayReceiptAfter1pass') 
INSERT INTO ApplicationSettings VALUES ('GlobalSetting', 'DisplayReceiptAfter1pass', 'boolean', 'True')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'TareWeightDiff') 
INSERT INTO ApplicationSettings VALUES ('GlobalSetting', 'TareWeightDiff', 'Integer', '1')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'PrintSlipNotes') 
INSERT INTO ApplicationSettings VALUES ('GlobalSetting', 'PrintSlipNotes', 'boolean', 'False')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'DateFormat') 
INSERT INTO ApplicationSettings VALUES ('GlobalSetting', 'DateFormat', 'String', 'dd-MM-yyyy')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'TimeFormat') 
INSERT INTO ApplicationSettings VALUES ('GlobalSetting', 'TimeFormat', 'String', 'hh:mm tt')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'GlobalSetting' AND SettingName = 'DecimalFormat') 
INSERT INTO ApplicationSettings VALUES ('GlobalSetting', 'DecimalFormat', 'String', '0.####')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'CompanySetting' AND SettingName = 'CompanyName') 
INSERT INTO ApplicationSettings VALUES ('CompanySetting', 'CompanyName', 'String', 'Company Name')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'CompanySetting' AND SettingName = 'Address') 
INSERT INTO ApplicationSettings VALUES ('CompanySetting', 'Address', 'String', 'Address')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'CompanySetting' AND SettingName = 'Phone') 
INSERT INTO ApplicationSettings VALUES ('CompanySetting', 'Phone', 'String', '1234567890')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'CompanySetting' AND SettingName = 'Email') 
INSERT INTO ApplicationSettings VALUES ('CompanySetting', 'Email', 'String', 'ID@server.com')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'CompanySetting' AND SettingName = 'ClientCompanyName') 
INSERT INTO ApplicationSettings VALUES ('CompanySetting', 'ClientCompanyName', 'String', 'Client Company')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'CompanySetting' AND SettingName = 'ClientContactPerson') 
INSERT INTO ApplicationSettings VALUES ('CompanySetting', 'ClientContactPerson', 'String', 'Contact Person')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'CompanySetting' AND SettingName = 'ClientAddress') 
INSERT INTO ApplicationSettings VALUES ('CompanySetting', 'ClientAddress', 'String', 'Client Address')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'CompanySetting' AND SettingName = 'SupportID') 
INSERT INTO ApplicationSettings VALUES ('CompanySetting', 'SupportID', 'String', 'support@mail.com')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'CompanySetting' AND SettingName = 'Logo') 
INSERT INTO ApplicationSettings VALUES ('CompanySetting', 'Logo', 'String', 'logo.png')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'ThemeSetting' AND SettingName = 'Color') 
INSERT INTO ApplicationSettings VALUES ('ThemeSetting', 'Color', 'String', 'Blue')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'ThemeSetting' AND SettingName = 'BaseColor') 
INSERT INTO ApplicationSettings VALUES ('ThemeSetting', 'BaseColor', 'String', 'BaseLight')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'EmailSetting' AND SettingName = 'ServerName') 
INSERT INTO ApplicationSettings VALUES ('EmailSetting', 'ServerName', 'String', 'Localhost')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'EmailSetting' AND SettingName = 'Port') 
INSERT INTO ApplicationSettings VALUES ('EmailSetting', 'Port', 'String', '25')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'EmailSetting' AND SettingName = 'UserName') 
INSERT INTO ApplicationSettings VALUES ('EmailSetting', 'UserName', 'String', '')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'EmailSetting' AND SettingName = 'PassWord') 
INSERT INTO ApplicationSettings VALUES ('EmailSetting', 'PassWord', 'String', '')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'EmailSetting' AND SettingName = 'UseSSL') 
INSERT INTO ApplicationSettings VALUES ('EmailSetting', 'UseSSL', 'String', 'False')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'EmailSetting' AND SettingName = 'EmailFrom') 
INSERT INTO ApplicationSettings VALUES ('EmailSetting', 'EmailFrom', 'String', '')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'DatabaseSetting' AND SettingName = 'AutoBackupEnable') 
INSERT INTO ApplicationSettings VALUES ('DatabaseSetting', 'AutoBackupEnable', 'Boolean', 'False')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'DatabaseSetting' AND SettingName = 'AutoBackupHour') 
INSERT INTO ApplicationSettings VALUES ('DatabaseSetting', 'AutoBackupHour', 'Integer', '1')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'DatabaseSetting' AND SettingName = 'AutoBackupMinute') 
INSERT INTO ApplicationSettings VALUES ('DatabaseSetting', 'AutoBackupMinute', 'Integer', '0')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'DatabaseSetting' AND SettingName = 'AutoBackupMeridiem') 
INSERT INTO ApplicationSettings VALUES ('DatabaseSetting', 'AutoBackupMeridiem', 'String', 'AM')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'DatabaseSetting' AND SettingName = 'AutoBackupFilePath') 
INSERT INTO ApplicationSettings VALUES ('DatabaseSetting', 'AutoBackupFilePath', 'String', '')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'PrintSetting' AND SettingName = 'PrintType')
INSERT INTO ApplicationSettings VALUES('PrintSetting', 'PrintType', 'String', 'Laser')
GO

IF NOT EXISTS(SELECT * FROM ApplicationSettings WHERE ModuleName = 'PrintSetting' AND SettingName = 'PrinterName')
INSERT INTO ApplicationSettings VALUES('PrintSetting', 'PrinterName', 'String', '')
GO

IF NOT EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'SMSSetting' AND SettingName = 'ProviderName') 
INSERT INTO ApplicationSettings VALUES ('SMSSetting', 'ProviderName', 'String', '')
GO
ALTER TABLE dbo.WeightBridge ADD
	IndicatorID int NULL
GO
ALTER TABLE dbo.WeightBridge SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE dbo.WeightBridge_Tracking ADD
	IndicatorID_old int NULL,
	IndicatorID_new int NULL
GO
ALTER TABLE dbo.WeightBridge_Tracking SET (LOCK_ESCALATION = TABLE)
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmailSetup]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[EmailSetup](
	[EmailSetupID] [int] IDENTITY(1,1) NOT NULL,
	[ScheduleName] [nvarchar](50) NOT NULL,
	[ScheduleDescription] [nvarchar](max) NULL,
	[IsEnable] [bit] NOT NULL,
	[EmailType] [nvarchar](50) NOT NULL,
	[EmailTo] [nvarchar](max) NOT NULL,
	[EmailSubject] [nvarchar](max) NOT NULL,
	[EmailBody] [nvarchar](max) NULL,
	[ColumnSelection] [nvarchar](max) NULL,
	[ScheduleType] [nvarchar](50) NULL,
	[ScheduleTime] [nvarchar](200) NULL,
	[NextRunTime] [datetime] NULL,
	[FilterDetail] [nvarchar](max) NULL,
	[WhereClause] [nvarchar](max) NULL,
	[Filterstring] [nvarchar](max) NULL
 CONSTRAINT [PK_EmailSetup] PRIMARY KEY CLUSTERED 
(
	[EmailSetupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmailSetup_Tracking]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[EmailSetup_Tracking](
	[TranID] [int] IDENTITY(1,1) NOT NULL,
	[TranDate] [datetime] NULL,
	[Operation] [varchar](20) NULL,
	[UserActivity] [varchar](max) NULL,
	[TranUserID] [int] NULL,
	[TranEnvironment] [varchar](20) NULL,
	[EmailSetupID_old] [int] NOT NULL,
	[EmailSetupID_new] [int] NOT NULL,
	[ScheduleName_old] [nvarchar](50) NOT NULL,
	[ScheduleName_new] [nvarchar](50) NOT NULL,
	[ScheduleDescription_old] [nvarchar](max) NULL,
	[ScheduleDescription_new] [nvarchar](max) NULL,
	[IsEnable_old] [bit] NOT NULL,
	[IsEnable_new] [bit] NOT NULL,
	[EmailType_old] [nvarchar](50) NOT NULL,
	[EmailType_new] [nvarchar](50) NOT NULL,
	[EmailTo_old] [nvarchar](max) NOT NULL,
	[EmailTo_new] [nvarchar](max) NOT NULL,
	[EmailSubject_old] [nvarchar](max) NOT NULL,
	[EmailSubject_new] [nvarchar](max) NOT NULL,
	[EmailBody_old] [nvarchar](max) NULL,
	[EmailBody_new] [nvarchar](max) NULL,
	[ColumnSelection_old] [nvarchar](max) NULL,
	[ColumnSelection_new] [nvarchar](max) NULL,
	[ScheduleType_old] [nvarchar](50) NULL,
	[ScheduleType_new] [nvarchar](50) NULL,
	[ScheduleTime_old] [nvarchar](200) NULL,
	[ScheduleTime_new] [nvarchar](200) NULL,
	[NextRunTime_old] [datetime] NULL,
	[NextRunTime_new] [datetime] NULL,
	[FilterDetail_old] [nvarchar](max) NULL,
	[FilterDetail_new] [nvarchar](max) NULL,
	[WhereClause_old] [nvarchar](max) NULL,
	[WhereClause_new] [nvarchar](max) NULL,
	[Filterstring_old] [nvarchar](max) NULL,
	[Filterstring_new] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

CREATE TRIGGER [dbo].[TRACKUSERAUDIT_EMAILSETUP_DELETE] ON [dbo].[EmailSetup] FOR DELETE AS DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;
SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())
IF @UserID > 0 BEGIN SELECT @UserName = UserName FROM [UserMaster] WHERE UserID = @UserID END ELSE BEGIN SET @UserName = 'System Service' END
INSERT INTO [dbo].[EmailSetup_Tracking] (TranDate, Operation,UserActivity, TranUserID,TranEnvironment,
  [EmailSetupID_old] , [EmailSetupID_new] ,
  [ScheduleName_old] , [ScheduleName_new] ,
  [ScheduleDescription_old] , [ScheduleDescription_new] ,
  [IsEnable_old] , [IsEnable_new] ,
  [EmailType_old] , [EmailType_new] ,
  [EmailTo_old] , [EmailTo_new] ,
  [EmailSubject_old] , [EmailSubject_new] ,
  [EmailBody_old] , [EmailBody_new] ,
  [ColumnSelection_old] , [ColumnSelection_new] ,
  [ScheduleType_old] , [ScheduleType_new] ,
  [ScheduleTime_old] , [ScheduleTime_new] ,
  [NextRunTime_old] , [NextRunTime_new] ,
  [FilterDetail_old] , [FilterDetail_new] ,
  [WhereClause_old] , [WhereClause_new] ,
  [Filterstring_old] ,[Filterstring_new]
) SELECT GetDate(), 'DELETE', 'EmailSetup information deleted by : ' + @UserName + ' at : ' + CONVERT(VARCHAR(10), GETDATE(), 103) ,dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()),
  [EmailSetupID] , [EmailSetupID] ,
  [ScheduleName] , [ScheduleName] ,
  [ScheduleDescription] , [ScheduleDescription] ,
  [IsEnable] , [IsEnable] ,
  [EmailType] , [EmailType] ,
  [EmailTo] , [EmailTo] ,
  [EmailSubject] , [EmailSubject] ,
  [EmailBody] , [EmailBody] ,
  [ColumnSelection] , [ColumnSelection] ,
  [ScheduleType] , [ScheduleType] ,
  [ScheduleTime] , [ScheduleTime] ,
  [NextRunTime] , [NextRunTime] ,
  [FilterDetail] , [FilterDetail] ,
  [WhereClause] , [WhereClause] ,
  [Filterstring] ,[Filterstring]
FROM Deleted 
GO
/****** Object:  Trigger [dbo].[TRACKUSERAUDIT_EMAILSETUP_INSERT]    Script Date: 05/11/2018 1:25:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[TRACKUSERAUDIT_EMAILSETUP_INSERT] ON [dbo].[EmailSetup] FOR INSERT AS DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;
SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())
IF @UserID > 0 BEGIN SELECT @UserName = UserName FROM [UserMaster] WHERE UserID = @UserID END ELSE BEGIN SET @UserName = 'System Service' END
INSERT INTO [dbo].[EmailSetup_Tracking] (TranDate, Operation,UserActivity, TranUserID,TranEnvironment,
  [EmailSetupID_old] , [EmailSetupID_new] ,
  [ScheduleName_old] , [ScheduleName_new] ,
  [ScheduleDescription_old] , [ScheduleDescription_new] ,
  [IsEnable_old] , [IsEnable_new] ,
  [EmailType_old] , [EmailType_new] ,
  [EmailTo_old] , [EmailTo_new] ,
  [EmailSubject_old] , [EmailSubject_new] ,
  [EmailBody_old] , [EmailBody_new] ,
  [ColumnSelection_old] , [ColumnSelection_new] ,
  [ScheduleType_old] , [ScheduleType_new] ,
  [ScheduleTime_old] , [ScheduleTime_new] ,
  [NextRunTime_old] , [NextRunTime_new] ,
  [FilterDetail_old] , [FilterDetail_new] ,
  [WhereClause_old] , [WhereClause_new] ,
  [Filterstring_old] ,[Filterstring_new]
) SELECT GetDate(), 'INSERT', 'EmailSetup information created by : ' + @UserName + ' at : ' + CONVERT(VARCHAR(10), GETDATE(), 103) ,dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()),
  [EmailSetupID] , [EmailSetupID] ,
  [ScheduleName] , [ScheduleName] ,
  [ScheduleDescription] , [ScheduleDescription] ,
  [IsEnable] , [IsEnable] ,
  [EmailType] , [EmailType] ,
  [EmailTo] , [EmailTo] ,
  [EmailSubject] , [EmailSubject] ,
  [EmailBody] , [EmailBody] ,
  [ColumnSelection] , [ColumnSelection] ,
  [ScheduleType] , [ScheduleType] ,
  [ScheduleTime] , [ScheduleTime] ,
  [NextRunTime] , [NextRunTime] ,
  [FilterDetail] , [FilterDetail] ,
  [WhereClause] , [WhereClause] ,
  [Filterstring] ,[Filterstring]
FROM Inserted 

GO

/****** Object:  Trigger [dbo].[TRACKUSERAUDIT_EMAILSETUP_UPDATE]    Script Date: 05/11/2018 1:25:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[TRACKUSERAUDIT_EMAILSETUP_UPDATE] ON [dbo].[EmailSetup] FOR UPDATE AS DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;
SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())
IF @UserID > 0 BEGIN SELECT @UserName = UserName FROM [UserMaster] WHERE UserID = @UserID END ELSE BEGIN SET @UserName = 'System Service' END
INSERT INTO [dbo].[EmailSetup_Tracking] (TranDate, Operation,UserActivity, TranUserID, TranEnvironment,
  [EmailSetupID_old] , [EmailSetupID_new] ,
  [ScheduleName_old] , [ScheduleName_new] ,
  [ScheduleDescription_old] , [ScheduleDescription_new] ,
  [IsEnable_old] , [IsEnable_new] ,
  [EmailType_old] , [EmailType_new] ,
  [EmailTo_old] , [EmailTo_new] ,
  [EmailSubject_old] , [EmailSubject_new] ,
  [EmailBody_old] , [EmailBody_new] ,
  [ColumnSelection_old] , [ColumnSelection_new] ,
  [ScheduleType_old] , [ScheduleType_new] ,
  [ScheduleTime_old] , [ScheduleTime_new] ,
  [NextRunTime_old] , [NextRunTime_new] ,
  [FilterDetail_old] , [FilterDetail_new] ,
  [WhereClause_old] , [WhereClause_new] ,
  [Filterstring_old] ,[Filterstring_new]
) SELECT GetDate(), 'UPDATE', 'EmailSetup information modified by : ' + @UserName + ' at : ' + CONVERT(VARCHAR(10), GETDATE(), 103) ,dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()),
  D.[EmailSetupID] , I.[EmailSetupID] ,
  D.[ScheduleName] , I.[ScheduleName] ,
  D.[ScheduleDescription] , I.[ScheduleDescription] ,
  D.[IsEnable] , I.[IsEnable] ,
  D.[EmailType] , I.[EmailType] ,
  D.[EmailTo] , I.[EmailTo] ,
  D.[EmailSubject] , I.[EmailSubject] ,
  D.[EmailBody] , I.[EmailBody] ,
  D.[ColumnSelection] , I.[ColumnSelection] ,
  D.[ScheduleType] , I.[ScheduleType] ,
  D.[ScheduleTime] , I.[ScheduleTime] ,
  D.[NextRunTime] , I.[NextRunTime] ,
  D.[FilterDetail] , I.[FilterDetail] ,
  D.[WhereClause] , I.[WhereClause] ,
  D.[Filterstring] ,I.[Filterstring]
FROM Deleted D CROSS JOIN Inserted I 
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StampCertificate]') AND type in (N'U'))
BEGIN
CREATE TABLE dbo.StampCertificate
	(
	StampId int NOT NULL IDENTITY (1, 1),
	StampName nvarchar(100) NOT NULL,
	IssueDate datetime NOT NULL,
	ExpiryDate datetime NOT NULL,
	IssuedAuth nvarchar(100) NULL,
	CreatedBy int NULL,
	StampPdf varbinary(MAX) NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
ALTER TABLE dbo.StampCertificate ADD CONSTRAINT
	PK_StampCertificate PRIMARY KEY CLUSTERED 
	(
	StampId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

ALTER TABLE dbo.StampCertificate ADD CONSTRAINT
	FK_StampCertificate_UserMaster FOREIGN KEY
	(
	CreatedBy
	) REFERENCES dbo.UserMaster
	(
	UserID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 	

END
GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [dbo].[SMSSetup]    Script Date: 05-10-2020 12:05:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SMSSetup]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[SMSSetup](
	[SMSSetupID] [int] IDENTITY(1,1) NOT NULL,
	[ScheduleName] [nvarchar](50) NOT NULL,
	[ScheduleDescription] [nvarchar](max) NULL,
	[IsEnable] [bit] NOT NULL,
	[SMSType] [nvarchar](50) NOT NULL,
	[SMSTo] [nvarchar](max) NOT NULL,
	[SMSSubject] [nvarchar](max) NULL,
	[SMSBody] [nvarchar](max) NULL,
	[ColumnSelection] [nvarchar](max) NULL,
	[ScheduleType] [nvarchar](50) NULL,
	[ScheduleTime] [nvarchar](200) NULL,
	[NextRunTime] [datetime] NULL,
	[FilterDetail] [nvarchar](max) NULL,
	[WhereClause] [nvarchar](max) NULL,
	[Filterstring] [nvarchar](max) NULL,
 CONSTRAINT [PK_[SMSSetup] PRIMARY KEY CLUSTERED 
(
	[SMSSetupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SMSSetup_Tracking]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[SMSSetup_Tracking](
	[TranID] [int] IDENTITY(1,1) NOT NULL,
	[TranDate] [datetime] NULL,
	[Operation] [varchar](20) NULL,
	[UserActivity] [varchar](max) NULL,
	[TranUserID] [int] NULL,
	[TranEnvironment] [varchar](20) NULL,
	[SMSSetupID_old] [int] NOT NULL,
	[SMSSetupID_new] [int] NOT NULL,
	[ScheduleName_old] [nvarchar](50) NOT NULL,
	[ScheduleName_new] [nvarchar](50) NOT NULL,
	[ScheduleDescription_old] [nvarchar](max) NULL,
	[ScheduleDescription_new] [nvarchar](max) NULL,
	[IsEnable_old] [bit] NOT NULL,
	[IsEnable_new] [bit] NOT NULL,
	[SMSType_old] [nvarchar](50) NOT NULL,
	[SMSType_new] [nvarchar](50) NOT NULL,
	[SMSTo_old] [nvarchar](max) NOT NULL,
	[SMSTo_new] [nvarchar](max) NOT NULL,
	[SMSSubject_old] [nvarchar](max) NULL,
	[SMSSubject_new] [nvarchar](max) NULL,
	[SMSBody_old] [nvarchar](max) NULL,
	[SMSBody_new] [nvarchar](max) NULL,
	[ColumnSelection_old] [nvarchar](max) NULL,
	[ColumnSelection_new] [nvarchar](max) NULL,
	[ScheduleType_old] [nvarchar](50) NULL,
	[ScheduleType_new] [nvarchar](50) NULL,
	[ScheduleTime_old] [nvarchar](200) NULL,
	[ScheduleTime_new] [nvarchar](200) NULL,
	[NextRunTime_old] [datetime] NULL,
	[NextRunTime_new] [datetime] NULL,
	[FilterDetail_old] [nvarchar](max) NULL,
	[FilterDetail_new] [nvarchar](max) NULL,
	[WhereClause_old] [nvarchar](max) NULL,
	[WhereClause_new] [nvarchar](max) NULL,
	[Filterstring_old] [nvarchar](max) NULL,
	[Filterstring_new] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[TRACKUSERAUDIT_SMSSETUP_DELETE] ON [dbo].[SMSSetup] FOR DELETE AS DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;
SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())
IF @UserID > 0 BEGIN SELECT @UserName = UserName FROM [UserMaster] WHERE UserID = @UserID END ELSE BEGIN SET @UserName = 'System Service' END
INSERT INTO [dbo].[SMSSetup_Tracking] (TranDate, Operation,UserActivity, TranUserID,TranEnvironment,
 [SMSSetupID_old],[SMSSetupID_new]
,[ScheduleName_old],[ScheduleName_new]
,[ScheduleDescription_old],[ScheduleDescription_new]
,[IsEnable_old],[IsEnable_new]
,[SMSType_old],[SMSType_new]
,[SMSTo_old],[SMSTo_new]
,[SMSSubject_old],[SMSSubject_new]
,[SMSBody_old],[SMSBody_new]
,[ColumnSelection_old],[ColumnSelection_new]
,[ScheduleType_old],[ScheduleType_new]
,[ScheduleTime_old],[ScheduleTime_new]
,[NextRunTime_old],[NextRunTime_new]
,[FilterDetail_old],[FilterDetail_new]
,[WhereClause_old],[WhereClause_new]
,[Filterstring_old],[Filterstring_new]
) SELECT GetDate(), 'DELETE', 'SMS Setup information deleted by : ' + @UserName + ' at : ' + CONVERT(VARCHAR(10), GETDATE(), 103) ,dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()),
  [SMSSetupID],[SMSSetupID],
  [ScheduleName],[ScheduleName],
  [ScheduleDescription],[ScheduleDescription],
  [IsEnable],[IsEnable],
  [SMSType],[SMSType],
  [SMSTo],[SMSTo],
  [SMSSubject],[SMSSubject],
  [SMSBody],[SMSBody],
  [ColumnSelection], [ColumnSelection],
  [ScheduleType],[ScheduleType],
  [ScheduleTime],[ScheduleTime],
  [NextRunTime],[NextRunTime],
  [FilterDetail],[FilterDetail],
  [WhereClause],[WhereClause],
  [Filterstring],[Filterstring]
FROM Deleted 
GO
ALTER TABLE [dbo].[SMSSetup] ENABLE TRIGGER [TRACKUSERAUDIT_SMSSETUP_DELETE]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[TRACKUSERAUDIT_SMSSETUP_INSERT] ON [dbo].[SMSSetup] FOR INSERT AS DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;
SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())
IF @UserID > 0 BEGIN SELECT @UserName = UserName FROM [UserMaster] WHERE UserID = @UserID END ELSE BEGIN SET @UserName = 'System Service' END
INSERT INTO [dbo].[SMSSetup_Tracking] (TranDate, Operation,UserActivity, TranUserID,TranEnvironment,
  [SMSSetupID_old],[SMSSetupID_new]
,[ScheduleName_old],[ScheduleName_new]
,[ScheduleDescription_old],[ScheduleDescription_new]
,[IsEnable_old],[IsEnable_new]
,[SMSType_old],[SMSType_new]
,[SMSTo_old],[SMSTo_new]
,[SMSSubject_old],[SMSSubject_new]
,[SMSBody_old],[SMSBody_new]
,[ColumnSelection_old],[ColumnSelection_new]
,[ScheduleType_old],[ScheduleType_new]
,[ScheduleTime_old],[ScheduleTime_new]
,[NextRunTime_old],[NextRunTime_new]
,[FilterDetail_old],[FilterDetail_new]
,[WhereClause_old],[WhereClause_new]
,[Filterstring_old],[Filterstring_new]
) SELECT GetDate(), 'INSERT', 'SMS Setup information created by : ' + @UserName + ' at : ' + CONVERT(VARCHAR(10), GETDATE(), 103) ,dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()),
  [SMSSetupID],[SMSSetupID],
  [ScheduleName],[ScheduleName],
  [ScheduleDescription],[ScheduleDescription],
  [IsEnable],[IsEnable],
  [SMSType],[SMSType],
  [SMSTo],[SMSTo],
  [SMSSubject],[SMSSubject],
  [SMSBody],[SMSBody],
  [ColumnSelection], [ColumnSelection],
  [ScheduleType],[ScheduleType],
  [ScheduleTime],[ScheduleTime],
  [NextRunTime],[NextRunTime],
  [FilterDetail],[FilterDetail],
  [WhereClause],[WhereClause],
  [Filterstring],[Filterstring]
FROM Inserted 

GO
ALTER TABLE [dbo].[SMSSetup] ENABLE TRIGGER [TRACKUSERAUDIT_SMSSETUP_INSERT]
GO
/****** Object:  Trigger [dbo].[TRACKUSERAUDIT_SMSSETUP_UPDATE]    Script Date: 05-10-2020 12:05:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[TRACKUSERAUDIT_SMSSETUP_UPDATE] ON [dbo].[SMSSetup] FOR UPDATE AS DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;
SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())
IF @UserID > 0 BEGIN SELECT @UserName = UserName FROM [UserMaster] WHERE UserID = @UserID END ELSE BEGIN SET @UserName = 'System Service' END
INSERT INTO [dbo].[SMSSetup_Tracking] (TranDate, Operation,UserActivity, TranUserID, TranEnvironment,
  [SMSSetupID_old] , [SMSSetupID_new] ,
  [ScheduleName_old] , [ScheduleName_new] ,
  [ScheduleDescription_old] , [ScheduleDescription_new] ,
  [IsEnable_old] , [IsEnable_new] ,
  [SMSType_old] , [SMSType_new] ,
  [SMSTo_old] , [SMSTo_new] ,
  [SMSSubject_old] , [SMSSubject_new] ,
  [SMSBody_old] , [SMSBody_new] ,
  [ColumnSelection_old] , [ColumnSelection_new] ,
  [ScheduleType_old] , [ScheduleType_new] ,
  [ScheduleTime_old] , [ScheduleTime_new] ,
  [NextRunTime_old] , [NextRunTime_new] ,
  [FilterDetail_old] , [FilterDetail_new] ,
  [WhereClause_old] , [WhereClause_new] ,
  [Filterstring_old] ,[Filterstring_new]
) SELECT GetDate(), 'UPDATE', 'SMS Setup information modified by : ' + @UserName + ' at : ' + CONVERT(VARCHAR(10), GETDATE(), 103) ,dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()),
  D.[SMSSetupID] , I.[SMSSetupID] ,
  D.[ScheduleName] , I.[ScheduleName] ,
  D.[ScheduleDescription] , I.[ScheduleDescription] ,
  D.[IsEnable] , I.[IsEnable] ,
  D.[SMSType] , I.[SMSType] ,
  D.[SMSTo] , I.[SMSTo] ,
  D.[SMSSubject] , I.[SMSSubject] ,
  D.[SMSBody] , I.[SMSBody] ,
  D.[ColumnSelection] , I.[ColumnSelection] ,
  D.[ScheduleType] , I.[ScheduleType] ,
  D.[ScheduleTime] , I.[ScheduleTime] ,
  D.[NextRunTime] , I.[NextRunTime] ,
  D.[FilterDetail] , I.[FilterDetail] ,
  D.[WhereClause] , I.[WhereClause] ,
  D.[Filterstring] ,I.[Filterstring]
FROM Deleted D CROSS JOIN Inserted I 

GO
ALTER TABLE [dbo].[SMSSetup] ENABLE TRIGGER [TRACKUSERAUDIT_SMSSETUP_UPDATE]
GO

/****** Object:  Table [dbo].[BarcodeImage]    Script Date: 11/10/2018 7:24:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BarcodeImage]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BarcodeImage](
	[BarcodeImageID] [int] IDENTITY(1,1) NOT NULL,
	[ReceiptTicketID] [uniqueidentifier] NOT NULL,
	[ImagePath] [nvarchar](100) NOT NULL,
	[BarcodeString] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_BarcodeImage] PRIMARY KEY CLUSTERED 
(
	[BarcodeImageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[BarcodeSetting]    Script Date: 11/10/2018 7:24:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BarcodeSetting]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BarcodeSetting](
	[BarcodeSettingID] [int] IDENTITY(1,1) NOT NULL,
	[BarcodeFieldName] [nvarchar](50) NOT NULL,
	[BarcodeDisplayName] [nvarchar](50) NOT NULL,
	[IsSelected] [bit] NOT NULL,
 CONSTRAINT [PK_BarcodeSetting] PRIMARY KEY CLUSTERED 
(
	[BarcodeSettingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BarcodeImage_TransactionData]') AND parent_object_id = OBJECT_ID(N'[dbo].[BarcodeImage]'))
ALTER TABLE [dbo].[BarcodeImage]  WITH CHECK ADD  CONSTRAINT [FK_BarcodeImage_TransactionData] FOREIGN KEY([ReceiptTicketID])
REFERENCES [dbo].[TransactionData] ([ReceiptTicketID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BarcodeImage_TransactionData]') AND parent_object_id = OBJECT_ID(N'[dbo].[BarcodeImage]'))
ALTER TABLE [dbo].[BarcodeImage] CHECK CONSTRAINT [FK_BarcodeImage_TransactionData]
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BarcodeSetting_Tracking]') AND type in (N'U'))
BEGIN
--Audit script
CREATE TABLE [BarcodeSetting_Tracking] (
	[TranID] [int] IDENTITY(1, 1) NOT NULL
	,[TranDate] [datetime] NULL
	,[Operation] [varchar](20) NULL
	,[UserActivity] [varchar](MAX) NULL
	,[TranUserID] [int] NULL
	,[TranEnvironment] [varchar](20) NULL
	,[BarcodeSettingID_old] INT NOT NULL
	,[BarcodeSettingID_new] INT NOT NULL
	,[BarcodeFieldName_old] NVARCHAR(50) NOT NULL
	,[BarcodeFieldName_new] NVARCHAR(50) NOT NULL
	,[BarcodeDisplayName_old] NVARCHAR(50) NOT NULL
	,[BarcodeDisplayName_new] NVARCHAR(50) NOT NULL
	,[IsSelected_old] BIT NOT NULL
	,[IsSelected_new] BIT NOT NULL
	)
END
GO

IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TRACKUSERAUDIT_BARCODESETTING_UPDATE]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TRACKUSERAUDIT_BARCODESETTING_UPDATE] ON [dbo].[BarcodeSetting]
FOR UPDATE
AS
DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;

SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())

IF @UserID > 0
BEGIN
	SELECT @UserName = UserName
	FROM [UserMaster]
	WHERE UserID = @UserID
END
ELSE
BEGIN
	SET @UserName = ''System Service''
END

INSERT INTO [dbo].[BarcodeSetting_Tracking] (
	TranDate
	,Operation
	,UserActivity
	,TranUserID
	,TranEnvironment
	,[BarcodeSettingID_old]
	,[BarcodeSettingID_new]
	,[BarcodeFieldName_old]
	,[BarcodeFieldName_new]
	,[BarcodeDisplayName_old]
	,[BarcodeDisplayName_new]
	,[IsSelected_old]
	,[IsSelected_new]
	)
SELECT GetDate()
	,''UPDATE''
	,''Barcode Setting information modified by : '' + @UserName + '' at : '' + CONVERT(VARCHAR(10), GETDATE(), 103)
	,dbo.GetUserIDFromAppName(App_Name())
	,dbo.GetEnvironmentFromAppName(App_Name())
	,D.[BarcodeSettingID]
	,I.[BarcodeSettingID]
	,D.[BarcodeFieldName]
	,I.[BarcodeFieldName]
	,D.[BarcodeDisplayName]
	,I.[BarcodeDisplayName]
	,D.[IsSelected]
	,I.[IsSelected]
FROM Deleted D
CROSS JOIN Inserted I '
GO

IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TRACKUSERAUDIT_BARCODESETTING_DELETE]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TRACKUSERAUDIT_BARCODESETTING_DELETE] ON [dbo].[BarcodeSetting]
FOR DELETE
AS
DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;

SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())

IF @UserID > 0
BEGIN
	SELECT @UserName = UserName
	FROM [UserMaster]
	WHERE UserID = @UserID
END
ELSE
BEGIN
	SET @UserName = ''System Service''
END

INSERT INTO [dbo].[BarcodeSetting_Tracking] (
	TranDate
	,Operation
	,UserActivity
	,TranUserID
	,TranEnvironment
	,[BarcodeSettingID_old]
	,[BarcodeSettingID_new]
	,[BarcodeFieldName_old]
	,[BarcodeFieldName_new]
	,[BarcodeDisplayName_old]
	,[BarcodeDisplayName_new]
	,[IsSelected_old]
	,[IsSelected_new]
	)
SELECT GetDate()
	,''DELETE''
	,''Barcode Setting information deleted by : '' + @UserName + '' at : '' + CONVERT(VARCHAR(10), GETDATE(), 103)
	,dbo.GetUserIDFromAppName(App_Name())
	,dbo.GetEnvironmentFromAppName(App_Name())
	,[BarcodeSettingID]
	,[BarcodeSettingID]
	,[BarcodeFieldName]
	,[BarcodeFieldName]
	,[BarcodeDisplayName]
	,[BarcodeDisplayName]
	,[IsSelected]
	,[IsSelected]
FROM Deleted '
GO

IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TRACKUSERAUDIT_BARCODESETTING_INSERT]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TRACKUSERAUDIT_BARCODESETTING_INSERT] ON [dbo].[BarcodeSetting]
FOR INSERT
AS
DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;

SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())

IF @UserID > 0
BEGIN
	SELECT @UserName = UserName
	FROM [UserMaster]
	WHERE UserID = @UserID
END
ELSE
BEGIN
	SET @UserName = ''System Service''
END

INSERT INTO [dbo].[BarcodeSetting_Tracking] (
	TranDate
	,Operation
	,UserActivity
	,TranUserID
	,TranEnvironment
	,[BarcodeSettingID_old]
	,[BarcodeSettingID_new]
	,[BarcodeFieldName_old]
	,[BarcodeFieldName_new]
	,[BarcodeDisplayName_old]
	,[BarcodeDisplayName_new]
	,[IsSelected_old]
	,[IsSelected_new]
	)
SELECT GetDate()
	,''INSERT''
	,''Barcode Setting information created by : '' + @UserName + '' at : '' + CONVERT(VARCHAR(10), GETDATE(), 103)
	,dbo.GetUserIDFromAppName(App_Name())
	,dbo.GetEnvironmentFromAppName(App_Name())
	,[BarcodeSettingID]
	,[BarcodeSettingID]
	,[BarcodeFieldName]
	,[BarcodeFieldName]
	,[BarcodeDisplayName]
	,[BarcodeDisplayName]
	,[IsSelected]
	,[IsSelected]
FROM Inserted' 
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BarcodeImage_Tracking]') AND type in (N'U'))
BEGIN
CREATE TABLE [BarcodeImage_Tracking] ([TranID] [int] IDENTITY(1, 1) NOT NULL, [TranDate] [datetime] NULL, [Operation] [varchar](20) NULL, [UserActivity] [varchar](MAX) NULL, [TranUserID] [int] NULL, [TranEnvironment] [varchar](20) NULL, [BarcodeImageID_old] INT NOT NULL, [BarcodeImageID_new] INT NOT NULL, [ReceiptTicketID_old] UNIQUEIDENTIFIER NOT NULL, [ReceiptTicketID_new] UNIQUEIDENTIFIER NOT NULL, [ImagePath_old] NVARCHAR(100) NOT NULL, [ImagePath_new] NVARCHAR(100) NOT NULL, [BarcodeString_old] NVARCHAR(MAX) NOT NULL, [BarcodeString_new] NVARCHAR(MAX) NOT NULL)
END
GO

IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TRACKUSERAUDIT_BARCODEIMAGE_UPDATE]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TRACKUSERAUDIT_BARCODEIMAGE_UPDATE] ON [dbo].[BarcodeImage]
FOR UPDATE
AS
DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;

SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())

IF @UserID > 0
BEGIN
	SELECT @UserName = UserName
	FROM [UserMaster]
	WHERE UserID = @UserID
END
ELSE
BEGIN
	SET @UserName = ''System Service''
END

INSERT INTO [dbo].[BarcodeImage_Tracking] (TranDate, Operation, UserActivity, TranUserID, TranEnvironment, [BarcodeImageID_old], [BarcodeImageID_new], [ReceiptTicketID_old], [ReceiptTicketID_new], [ImagePath_old], [ImagePath_new], [BarcodeString_old], [BarcodeString_new])
SELECT GetDate(), ''UPDATE'', ''Barcode Image information modified by : '' + @UserName + '' at : '' + CONVERT(VARCHAR(10), GETDATE(), 103), dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()), D.[BarcodeImageID], I.[BarcodeImageID], D.[ReceiptTicketID], I.[ReceiptTicketID], D.[ImagePath], I.[ImagePath], D.[BarcodeString], I.[BarcodeString]
FROM Deleted D
CROSS JOIN Inserted I '
GO

IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TRACKUSERAUDIT_BARCODEIMAGE_DELETE]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TRACKUSERAUDIT_BARCODEIMAGE_DELETE] ON [dbo].[BarcodeImage]
FOR DELETE
AS
DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;

SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())

IF @UserID > 0
BEGIN
	SELECT @UserName = UserName
	FROM [UserMaster]
	WHERE UserID = @UserID
END
ELSE
BEGIN
	SET @UserName = ''System Service''
END

INSERT INTO [dbo].[BarcodeImage_Tracking] (TranDate, Operation, UserActivity, TranUserID, TranEnvironment, [BarcodeImageID_old], [BarcodeImageID_new], [ReceiptTicketID_old], [ReceiptTicketID_new], [ImagePath_old], [ImagePath_new], [BarcodeString_old], [BarcodeString_new])
SELECT GetDate(), ''DELETE'', ''Barcode Image information deleted by : '' + @UserName + '' at : '' + CONVERT(VARCHAR(10), GETDATE(), 103), dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()), [BarcodeImageID], [BarcodeImageID], [ReceiptTicketID], [ReceiptTicketID], [ImagePath], [ImagePath], [BarcodeString], [BarcodeString]
FROM Deleted '
GO

IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TRACKUSERAUDIT_BARCODEIMAGE_DELETE]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TRACKUSERAUDIT_BARCODEIMAGE_INSERT] ON [dbo].[BarcodeImage]
FOR INSERT
AS
DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;

SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())

IF @UserID > 0
BEGIN
	SELECT @UserName = UserName
	FROM [UserMaster]
	WHERE UserID = @UserID
END
ELSE
BEGIN
	SET @UserName = ''System Service''
END

INSERT INTO [dbo].[BarcodeImage_Tracking] (TranDate, Operation, UserActivity, TranUserID, TranEnvironment, [BarcodeImageID_old], [BarcodeImageID_new], [ReceiptTicketID_old], [ReceiptTicketID_new], [ImagePath_old], [ImagePath_new], [BarcodeString_old], [BarcodeString_new])
SELECT GetDate(), ''INSERT'', ''Barcode Image information created by : '' + @UserName + '' at : '' + CONVERT(VARCHAR(10), GETDATE(), 103), dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()), [BarcodeImageID], [BarcodeImageID], [ReceiptTicketID], [ReceiptTicketID], [ImagePath], [ImagePath], [BarcodeString], [BarcodeString]
FROM Inserted '
GO


IF NOT EXISTS (SELECT * FROM [dbo].[Feature] WHERE [FeatureName] = 'EmailSetup') 
BEGIN
SET IDENTITY_INSERT [dbo].[Feature] ON;
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (39, N'EmailSetup',N'CRU',CAST(4.07 AS Decimal(5,3)),32,0);
SET IDENTITY_INSERT [dbo].[Feature] OFF; 
END
GO


IF NOT EXISTS (SELECT * FROM [dbo].[Feature] WHERE [FeatureName] = 'PrintSetting') 
BEGIN
SET IDENTITY_INSERT [dbo].[Feature] ON;INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (40, N'PrintSetting',N'CRU',CAST(4.07 AS Decimal(5,3)),32,0);
SET IDENTITY_INSERT [dbo].[Feature] OFF; 
END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[Feature] WHERE [FeatureName] = 'SMSSetup') 
BEGIN
SET IDENTITY_INSERT [dbo].[Feature] ON;
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (41, N'SMSSetup',N'CRU',CAST(4.07 AS Decimal(5,3)),32,0);
SET IDENTITY_INSERT [dbo].[Feature] OFF; 
END
GO

IF NOT EXISTS (SELECT * FROM TemplatePermission WHERE FeatureID = 39) 
BEGIN
INSERT INTO TemplatePermission VALUES(39,1,N'11111');
INSERT INTO TemplatePermission VALUES (39, 2, N'11111');
END
GO
IF NOT EXISTS (SELECT * FROM TemplatePermission WHERE FeatureID = 40) 
BEGIN
INSERT INTO TemplatePermission VALUES(40,1,N'11111');
INSERT INTO TemplatePermission VALUES (40, 2, N'11111');
END
GO

IF NOT EXISTS (SELECT * FROM TemplatePermission WHERE FeatureID = 41) 
BEGIN
INSERT INTO TemplatePermission VALUES(41,1,N'11111');
INSERT INTO TemplatePermission VALUES (41, 2, N'11111');
END
GO

COMMIT
GO
