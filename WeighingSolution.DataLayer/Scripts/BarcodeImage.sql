
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