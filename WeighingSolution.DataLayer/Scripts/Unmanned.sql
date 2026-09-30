
IF NOT EXISTS (SELECT * FROM sys.columns where Name = N'UserID' AND Object_ID = Object_ID(N'TransactionDetail'))
BEGIN
ALTER TABLE TransactionDetail ALTER COLUMN UserID INT NULL
ALTER TABLE TransactionDetail_Tracking ALTER COLUMN UserID_old INT NULL
ALTER TABLE TransactionDetail_Tracking ALTER COLUMN UserID_new INT NULL
end
go

/****** Object:  Table [dbo].[PLCConfiguration]    Script Date: 31/01/2019 11:45:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PLCConfiguration]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PLCConfiguration](
	[PLCID] [int] IDENTITY(1,1) NOT NULL,
	[PLCName] [nvarchar](100) NULL,
	[IPAddress] [nvarchar](40) NULL,
	[WeighBridgeID] [int] NULL,
 CONSTRAINT [PK_PLCConfiguration] PRIMARY KEY CLUSTERED 
(
	[PLCID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[PLCConfiguration_Tracking]    Script Date: 31/01/2019 11:45:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PLCConfiguration_Tracking]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PLCConfiguration_Tracking](
	[TranID] [int] IDENTITY(1,1) NOT NULL,
	[TranDate] [datetime] NULL,
	[Operation] [varchar](20) NULL,
	[UserActivity] [varchar](max) NULL,
	[TranUserID] [int] NULL,
	[TranEnvironment] [varchar](20) NULL,
	[PLCID_old] [int] NOT NULL,
	[PLCID_new] [int] NOT NULL,
	[PLCName_old] [nvarchar](100) NULL,
	[PLCName_new] [nvarchar](100) NULL,
	[IPAddress_old] [nvarchar](40) NULL,
	[IPAddress_new] [nvarchar](40) NULL,
	[WeighBridgeID_old] [int] NULL,
	[WeighBridgeID_new] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TCPLog]    Script Date: 31/01/2019 11:45:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TCPLog]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TCPLog](
	[TCPLogID] [bigint] IDENTITY(1,1) NOT NULL,
	[RequestData] [nvarchar](max) NULL,
	[ResponseData] [nvarchar](max) NULL,
	[LogTime] [datetime] NULL,
 CONSTRAINT [PK_TCPLog] PRIMARY KEY CLUSTERED 
(
	[TCPLogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Token]    Script Date: 31/01/2019 11:45:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Token]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Token](
	[TokenID] [int] IDENTITY(1,1) NOT NULL,
	[TokenName] [nvarchar](200) NULL,
	[TokenValue] [nvarchar](500) NULL,
	[TokenType] [nvarchar](20) NULL,
	[PreferredTransactionMode] [nvarchar](10) NULL,
	[PreferredWeighmentType] [nvarchar](10) NULL,
	[PreferredTransactionType] [nvarchar](10) NULL,
	[AccountID] [int] NULL,
	[TransporterID] [int] NULL,
	[VehicleID] [int] NULL,
	[DriverID] [int] NULL,
	[ProductID] [int] NULL,
	[GateID] [int] NULL,
	[Charges] [decimal](18, 2) NULL,
	[TokenExpireDate] [datetime] NULL,
	[TokenInterval] [nvarchar](50) NULL,
	[TokenStatus] [nvarchar](50) NULL,
	[CustomValue1] [nvarchar](max) NULL,
	[CustomValue2] [nvarchar](max) NULL,
	[CustomValue3] [nvarchar](max) NULL,
	[CustomValue4] [nvarchar](max) NULL,
	[CustomValue5] [nvarchar](max) NULL,
	[CustomValue6] [nvarchar](max) NULL,
	[CustomValue7] [nvarchar](max) NULL,
	[CustomValue8] [nvarchar](max) NULL,
	[CustomValue9] [nvarchar](max) NULL,
	[CustomValue10] [nvarchar](max) NULL,
	[CustomValue11] [nvarchar](max) NULL,
	[CustomValue12] [nvarchar](max) NULL,
	[CustomValue13] [nvarchar](max) NULL,
	[CustomValue14] [nvarchar](max) NULL,
	[CustomValue15] [nvarchar](max) NULL,
	[CustomValue16] [nvarchar](max) NULL,
	[CustomValue17] [nvarchar](max) NULL,
	[CustomValue18] [nvarchar](max) NULL,
	[CustomValue19] [nvarchar](max) NULL,
	[CustomValue20] [nvarchar](max) NULL,
	[Vehicle_Number] [nvarchar](250) NULL,
	[Driver_Name] [nvarchar](250) NULL,
	[Account_Name] [nvarchar](250) NULL,
	[Transporter_Name] [nvarchar](250) NULL,
	[Product_Name] [nvarchar](250) NULL,
	[Gate_Name] [nvarchar](250) NULL,
 CONSTRAINT [PK_Token] PRIMARY KEY CLUSTERED 
(
	[TokenID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Token_Tracking]    Script Date: 31/01/2019 11:45:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Token_Tracking]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Token_Tracking](
	[TranID] [int] IDENTITY(1,1) NOT NULL,
	[TranDate] [datetime] NULL,
	[Operation] [varchar](20) NULL,
	[UserActivity] [varchar](max) NULL,
	[TranUserID] [int] NULL,
	[TranEnvironment] [varchar](20) NULL,
	[TokenID_old] [int] NOT NULL,
	[TokenID_new] [int] NOT NULL,
	[TokenName_old] [nvarchar](200) NULL,
	[TokenName_new] [nvarchar](200) NULL,
	[TokenValue_old] [nvarchar](500) NULL,
	[TokenValue_new] [nvarchar](500) NULL,
	[TokenType_old] [nvarchar](20) NULL,
	[TokenType_new] [nvarchar](20) NULL,
	[PreferredTransactionMode_old] [nvarchar](10) NULL,
	[PreferredTransactionMode_new] [nvarchar](10) NULL,
	[PreferredWeighmentType_old] [nvarchar](10) NULL,
	[PreferredWeighmentType_new] [nvarchar](10) NULL,
	[PreferredTransactionType_old] [nvarchar](10) NULL,
	[PreferredTransactionType_new] [nvarchar](10) NULL,
	[AccountID_old] [int] NULL,
	[AccountID_new] [int] NULL,
	[TransporterID_old] [int] NULL,
	[TransporterID_new] [int] NULL,
	[VehicleID_old] [int] NULL,
	[VehicleID_new] [int] NULL,
	[DriverID_old] [int] NULL,
	[DriverID_new] [int] NULL,
	[ProductID_old] [int] NULL,
	[ProductID_new] [int] NULL,
	[GateID_old] [int] NULL,
	[GateID_new] [int] NULL,
	[Charges_old] [decimal](18, 0) NULL,
	[Charges_new] [decimal](18, 0) NULL,
	[TokenExpireDate_old] [datetime] NULL,
	[TokenExpireDate_new] [datetime] NULL,
	[TokenInterval_old] [nvarchar](50) NULL,
	[TokenInterval_new] [nvarchar](50) NULL,
	[TokenStatus_old] [nvarchar](50) NULL,
	[TokenStatus_new] [nvarchar](50) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TransactionTokenLog]    Script Date: 31/01/2019 11:45:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransactionTokenLog]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TransactionTokenLog](
	[TransactionTokenLogID] [int] IDENTITY(1,1) NOT NULL,
	[ReceiptTicketID] [uniqueidentifier] NULL,
	[TokenID] [int] NOT NULL,
	[FreeTime] [datetime] NULL,
	[AllocateTime] [datetime] NULL,
	[DeallocateTime] [datetime] NULL,
	[IsCancel] [bit] NOT NULL,
	[CancelReason] [nvarchar](max) NULL,
 CONSTRAINT [PK_TransactionTokenLog] PRIMARY KEY CLUSTERED 
(
	[TransactionTokenLogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[TransactionTokenLog_Tracking]    Script Date: 31/01/2019 11:45:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TransactionTokenLog_Tracking]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TransactionTokenLog_Tracking](
	[TranID] [int] IDENTITY(1,1) NOT NULL,
	[TranDate] [datetime] NULL,
	[Operation] [varchar](20) NULL,
	[UserActivity] [varchar](max) NULL,
	[TranUserID] [int] NULL,
	[TranEnvironment] [varchar](20) NULL,
	[TransactionTokenLogID_old] [int] NOT NULL,
	[TransactionTokenLogID_new] [int] NOT NULL,
	[ReceiptTicketID_old] [uniqueidentifier] NULL,
	[ReceiptTicketID_new] [uniqueidentifier] NULL,
	[TokenID_old] [int] NOT NULL,
	[TokenID_new] [int] NOT NULL,
	[FreeTime_old] [datetime] NULL,
	[FreeTime_new] [datetime] NULL,
	[AllocateTime_old] [datetime] NULL,
	[AllocateTime_new] [datetime] NULL,
	[DeallocateTime_old] [datetime] NULL,
	[DeallocateTime_new] [datetime] NULL,
	[IsCancel_old] [bit] NOT NULL,
	[IsCancel_new] [bit] NOT NULL,
	[CancelReason_old] [nvarchar](max) NULL,
	[CancelReason_new] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PLCConfiguration_WeightBridge]') AND parent_object_id = OBJECT_ID(N'[dbo].[PLCConfiguration]'))
ALTER TABLE [dbo].[PLCConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_PLCConfiguration_WeightBridge] FOREIGN KEY([WeighBridgeID])
REFERENCES [dbo].[WeightBridge] ([WeightBridgeID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PLCConfiguration_WeightBridge]') AND parent_object_id = OBJECT_ID(N'[dbo].[PLCConfiguration]'))
ALTER TABLE [dbo].[PLCConfiguration] CHECK CONSTRAINT [FK_PLCConfiguration_WeightBridge]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Token_Account]') AND parent_object_id = OBJECT_ID(N'[dbo].[Token]'))
ALTER TABLE [dbo].[Token]  WITH CHECK ADD  CONSTRAINT [FK_Token_Account] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Account] ([AccountID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Token_Account]') AND parent_object_id = OBJECT_ID(N'[dbo].[Token]'))
ALTER TABLE [dbo].[Token] CHECK CONSTRAINT [FK_Token_Account]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Token_Driver]') AND parent_object_id = OBJECT_ID(N'[dbo].[Token]'))
ALTER TABLE [dbo].[Token]  WITH CHECK ADD  CONSTRAINT [FK_Token_Driver] FOREIGN KEY([DriverID])
REFERENCES [dbo].[Driver] ([DriverID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Token_Driver]') AND parent_object_id = OBJECT_ID(N'[dbo].[Token]'))
ALTER TABLE [dbo].[Token] CHECK CONSTRAINT [FK_Token_Driver]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Token_Gate]') AND parent_object_id = OBJECT_ID(N'[dbo].[Token]'))
ALTER TABLE [dbo].[Token]  WITH CHECK ADD  CONSTRAINT [FK_Token_Gate] FOREIGN KEY([GateID])
REFERENCES [dbo].[Gate] ([GateID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Token_Gate]') AND parent_object_id = OBJECT_ID(N'[dbo].[Token]'))
ALTER TABLE [dbo].[Token] CHECK CONSTRAINT [FK_Token_Gate]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Token_Product]') AND parent_object_id = OBJECT_ID(N'[dbo].[Token]'))
ALTER TABLE [dbo].[Token]  WITH CHECK ADD  CONSTRAINT [FK_Token_Product] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Product] ([ProductID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Token_Product]') AND parent_object_id = OBJECT_ID(N'[dbo].[Token]'))
ALTER TABLE [dbo].[Token] CHECK CONSTRAINT [FK_Token_Product]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Token_Transporter]') AND parent_object_id = OBJECT_ID(N'[dbo].[Token]'))
ALTER TABLE [dbo].[Token]  WITH CHECK ADD  CONSTRAINT [FK_Token_Transporter] FOREIGN KEY([TransporterID])
REFERENCES [dbo].[Account] ([AccountID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Token_Transporter]') AND parent_object_id = OBJECT_ID(N'[dbo].[Token]'))
ALTER TABLE [dbo].[Token] CHECK CONSTRAINT [FK_Token_Transporter]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Token_Vehicle]') AND parent_object_id = OBJECT_ID(N'[dbo].[Token]'))
ALTER TABLE [dbo].[Token]  WITH CHECK ADD  CONSTRAINT [FK_Token_Vehicle] FOREIGN KEY([VehicleID])
REFERENCES [dbo].[Vehicle] ([VehicleID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Token_Vehicle]') AND parent_object_id = OBJECT_ID(N'[dbo].[Token]'))
ALTER TABLE [dbo].[Token] CHECK CONSTRAINT [FK_Token_Vehicle]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TransactionTokenLog_Token]') AND parent_object_id = OBJECT_ID(N'[dbo].[TransactionTokenLog]'))
ALTER TABLE [dbo].[TransactionTokenLog]  WITH CHECK ADD  CONSTRAINT [FK_TransactionTokenLog_Token] FOREIGN KEY([TokenID])
REFERENCES [dbo].[Token] ([TokenID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TransactionTokenLog_Token]') AND parent_object_id = OBJECT_ID(N'[dbo].[TransactionTokenLog]'))
ALTER TABLE [dbo].[TransactionTokenLog] CHECK CONSTRAINT [FK_TransactionTokenLog_Token]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TransactionTokenLog_TransactionData]') AND parent_object_id = OBJECT_ID(N'[dbo].[TransactionTokenLog]'))
ALTER TABLE [dbo].[TransactionTokenLog]  WITH CHECK ADD  CONSTRAINT [FK_TransactionTokenLog_TransactionData] FOREIGN KEY([ReceiptTicketID])
REFERENCES [dbo].[TransactionData] ([ReceiptTicketID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_TransactionTokenLog_TransactionData]') AND parent_object_id = OBJECT_ID(N'[dbo].[TransactionTokenLog]'))
ALTER TABLE [dbo].[TransactionTokenLog] CHECK CONSTRAINT [FK_TransactionTokenLog_TransactionData]
GO
/****** Object:  Trigger [dbo].[TRACKUSERAUDIT_PLCCONFIGURATION_DELETE]    Script Date: 31/01/2019 11:45:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TRACKUSERAUDIT_PLCCONFIGURATION_DELETE]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TRACKUSERAUDIT_PLCCONFIGURATION_DELETE] ON [dbo].[PLCConfiguration] FOR DELETE AS DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;
SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())
IF @UserID > 0 BEGIN SELECT @UserName = UserName FROM [UserMaster] WHERE UserID = @UserID END ELSE BEGIN SET @UserName = ''System Service'' END
INSERT INTO [dbo].[PLCConfiguration_Tracking] (TranDate, Operation,UserActivity, TranUserID,TranEnvironment,
  [PLCID_old] , [PLCID_new] ,
  [PLCName_old] , [PLCName_new] ,
  [IPAddress_old] , [IPAddress_new] ,
  [WeighBridgeID_old] , [WeighBridgeID_new] 
) SELECT GetDate(), ''DELETE'', ''PLCConfiguration information deleted by : '' + @UserName + '' at : '' + CONVERT(VARCHAR(10), GETDATE(), 103) ,dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()),
  [PLCID] , [PLCID] ,
  [PLCName] , [PLCName] ,
  [IPAddress] , [IPAddress] ,
  [WeighBridgeID] , [WeighBridgeID] 
FROM Deleted' 
GO
/****** Object:  Trigger [dbo].[TRACKUSERAUDIT_PLCCONFIGURATION_INSERT]    Script Date: 31/01/2019 11:45:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TRACKUSERAUDIT_PLCCONFIGURATION_INSERT]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TRACKUSERAUDIT_PLCCONFIGURATION_INSERT] ON [dbo].[PLCConfiguration] FOR INSERT AS DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;
SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())
IF @UserID > 0 BEGIN SELECT @UserName = UserName FROM [UserMaster] WHERE UserID = @UserID END ELSE BEGIN SET @UserName = ''System Service'' END
INSERT INTO [dbo].[PLCConfiguration_Tracking] (TranDate, Operation,UserActivity, TranUserID,TranEnvironment,
  [PLCID_old] , [PLCID_new] ,
  [PLCName_old] , [PLCName_new] ,
  [IPAddress_old] , [IPAddress_new] ,
  [WeighBridgeID_old] , [WeighBridgeID_new] 
) SELECT GetDate(), ''INSERT'', ''PLCConfiguration information created by : '' + @UserName + '' at : '' + CONVERT(VARCHAR(10), GETDATE(), 103) ,dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()),
  [PLCID] , [PLCID] ,
  [PLCName] , [PLCName] ,
  [IPAddress] , [IPAddress] ,
  [WeighBridgeID] , [WeighBridgeID] 
FROM Inserted' 
GO
/****** Object:  Trigger [dbo].[TRACKUSERAUDIT_PLCCONFIGURATION_UPDATE]    Script Date: 31/01/2019 11:45:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TRACKUSERAUDIT_PLCCONFIGURATION_UPDATE]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TRACKUSERAUDIT_PLCCONFIGURATION_UPDATE] ON [dbo].[PLCConfiguration] FOR UPDATE AS DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;
SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())
IF @UserID > 0 BEGIN SELECT @UserName = UserName FROM [UserMaster] WHERE UserID = @UserID END ELSE BEGIN SET @UserName = ''System Service'' END
INSERT INTO [dbo].[PLCConfiguration_Tracking] (TranDate, Operation,UserActivity, TranUserID, TranEnvironment,
  [PLCID_old] , [PLCID_new] ,
  [PLCName_old] , [PLCName_new] ,
  [IPAddress_old] , [IPAddress_new] ,
  [WeighBridgeID_old] , [WeighBridgeID_new] 
) SELECT GetDate(), ''UPDATE'', ''PLCConfiguration information modified by : '' + @UserName + '' at : '' + CONVERT(VARCHAR(10), GETDATE(), 103) ,dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()),
  D.[PLCID] , I.[PLCID] ,
  D.[PLCName] , I.[PLCName] ,
  D.[IPAddress] , I.[IPAddress] ,
  D.[WeighBridgeID] , I.[WeighBridgeID] 
FROM Deleted D CROSS JOIN Inserted I' 
GO
/****** Object:  Trigger [dbo].[TRACKUSERAUDIT_TOKEN_DELETE]    Script Date: 31/01/2019 11:45:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TRACKUSERAUDIT_TOKEN_DELETE]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TRACKUSERAUDIT_TOKEN_DELETE] ON [dbo].[Token] FOR DELETE AS DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;
SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())
IF @UserID > 0 BEGIN SELECT @UserName = UserName FROM [UserMaster] WHERE UserID = @UserID END ELSE BEGIN SET @UserName = ''System Service'' END
INSERT INTO [dbo].[Token_Tracking] (TranDate, Operation,UserActivity, TranUserID,TranEnvironment,
  [TokenID_old] , [TokenID_new] ,
  [TokenName_old] , [TokenName_new] ,
  [TokenValue_old] , [TokenValue_new] ,
  [TokenType_old] , [TokenType_new] ,
  [PreferredTransactionMode_old] , [PreferredTransactionMode_new] ,
  [PreferredWeighmentType_old] , [PreferredWeighmentType_new] ,
  [PreferredTransactionType_old] , [PreferredTransactionType_new] ,
  [AccountID_old] , [AccountID_new] ,
  [TransporterID_old] , [TransporterID_new] ,
  [VehicleID_old] , [VehicleID_new] ,
  [DriverID_old] , [DriverID_new] ,
  [ProductID_old] , [ProductID_new] ,
  [GateID_old] , [GateID_new] ,
  [Charges_old] , [Charges_new] ,
  [TokenExpireDate_old] , [TokenExpireDate_new] ,
  [TokenInterval_old] , [TokenInterval_new] ,
  [TokenStatus_old] , [TokenStatus_new] 
) SELECT GetDate(), ''DELETE'', ''Token information deleted by : '' + @UserName + '' at : '' + CONVERT(VARCHAR(10), GETDATE(), 103) ,dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()),
  [TokenID] , [TokenID] ,
  [TokenName] , [TokenName] ,
  [TokenValue] , [TokenValue] ,
  [TokenType] , [TokenType] ,
  [PreferredTransactionMode] , [PreferredTransactionMode] ,
  [PreferredWeighmentType] , [PreferredWeighmentType] ,
  [PreferredTransactionType] , [PreferredTransactionType] ,
  [AccountID] , [AccountID] ,
  [TransporterID] , [TransporterID] ,
  [VehicleID] , [VehicleID] ,
  [DriverID] , [DriverID] ,
  [ProductID] , [ProductID] ,
  [GateID] , [GateID] ,
  [Charges] , [Charges] ,
  [TokenExpireDate] , [TokenExpireDate] ,
  [TokenInterval] , [TokenInterval] ,
  [TokenStatus] , [TokenStatus] 
FROM Deleted ' 
GO
/****** Object:  Trigger [dbo].[TRACKUSERAUDIT_TOKEN_INSERT]    Script Date: 31/01/2019 11:45:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TRACKUSERAUDIT_TOKEN_INSERT]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TRACKUSERAUDIT_TOKEN_INSERT] ON [dbo].[Token] FOR INSERT AS DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;
SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())
IF @UserID > 0 BEGIN SELECT @UserName = UserName FROM [UserMaster] WHERE UserID = @UserID END ELSE BEGIN SET @UserName = ''System Service'' END
INSERT INTO [dbo].[Token_Tracking] (TranDate, Operation,UserActivity, TranUserID,TranEnvironment,
  [TokenID_old] , [TokenID_new] ,
  [TokenName_old] , [TokenName_new] ,
  [TokenValue_old] , [TokenValue_new] ,
  [TokenType_old] , [TokenType_new] ,
  [PreferredTransactionMode_old] , [PreferredTransactionMode_new] ,
  [PreferredWeighmentType_old] , [PreferredWeighmentType_new] ,
  [PreferredTransactionType_old] , [PreferredTransactionType_new] ,
  [AccountID_old] , [AccountID_new] ,
  [TransporterID_old] , [TransporterID_new] ,
  [VehicleID_old] , [VehicleID_new] ,
  [DriverID_old] , [DriverID_new] ,
  [ProductID_old] , [ProductID_new] ,
  [GateID_old] , [GateID_new] ,
  [Charges_old] , [Charges_new] ,
  [TokenExpireDate_old] , [TokenExpireDate_new] ,
  [TokenInterval_old] , [TokenInterval_new] ,
  [TokenStatus_old] , [TokenStatus_new] 
) SELECT GetDate(), ''INSERT'', ''Token information created by : '' + @UserName + '' at : '' + CONVERT(VARCHAR(10), GETDATE(), 103) ,dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()),
  [TokenID] , [TokenID] ,
  [TokenName] , [TokenName] ,
  [TokenValue] , [TokenValue] ,
  [TokenType] , [TokenType] ,
  [PreferredTransactionMode] , [PreferredTransactionMode] ,
  [PreferredWeighmentType] , [PreferredWeighmentType] ,
  [PreferredTransactionType] , [PreferredTransactionType] ,
  [AccountID] , [AccountID] ,
  [TransporterID] , [TransporterID] ,
  [VehicleID] , [VehicleID] ,
  [DriverID] , [DriverID] ,
  [ProductID] , [ProductID] ,
  [GateID] , [GateID] ,
  [Charges] , [Charges] ,
  [TokenExpireDate] , [TokenExpireDate] ,
  [TokenInterval] , [TokenInterval] ,
  [TokenStatus] , [TokenStatus] 
FROM Inserted ' 
GO
/****** Object:  Trigger [dbo].[TRACKUSERAUDIT_TOKEN_UPDATE]    Script Date: 31/01/2019 11:45:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TRACKUSERAUDIT_TOKEN_UPDATE]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TRACKUSERAUDIT_TOKEN_UPDATE] ON [dbo].[Token] FOR UPDATE AS DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;
SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())
IF @UserID > 0 BEGIN SELECT @UserName = UserName FROM [UserMaster] WHERE UserID = @UserID END ELSE BEGIN SET @UserName = ''System Service'' END
INSERT INTO [dbo].[Token_Tracking] (TranDate, Operation,UserActivity, TranUserID, TranEnvironment,
  [TokenID_old] , [TokenID_new] ,
  [TokenName_old] , [TokenName_new] ,
  [TokenValue_old] , [TokenValue_new] ,
  [TokenType_old] , [TokenType_new] ,
  [PreferredTransactionMode_old] , [PreferredTransactionMode_new] ,
  [PreferredWeighmentType_old] , [PreferredWeighmentType_new] ,
  [PreferredTransactionType_old] , [PreferredTransactionType_new] ,
  [AccountID_old] , [AccountID_new] ,
  [TransporterID_old] , [TransporterID_new] ,
  [VehicleID_old] , [VehicleID_new] ,
  [DriverID_old] , [DriverID_new] ,
  [ProductID_old] , [ProductID_new] ,
  [GateID_old] , [GateID_new] ,
  [Charges_old] , [Charges_new] ,
  [TokenExpireDate_old] , [TokenExpireDate_new] ,
  [TokenInterval_old] , [TokenInterval_new] ,
  [TokenStatus_old] , [TokenStatus_new] 
) SELECT GetDate(), ''UPDATE'', ''Token information modified by : '' + @UserName + '' at : '' + CONVERT(VARCHAR(10), GETDATE(), 103) ,dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()),
  D.[TokenID] , I.[TokenID] ,
  D.[TokenName] , I.[TokenName] ,
  D.[TokenValue] , I.[TokenValue] ,
  D.[TokenType] , I.[TokenType] ,
  D.[PreferredTransactionMode] , I.[PreferredTransactionMode] ,
  D.[PreferredWeighmentType] , I.[PreferredWeighmentType] ,
  D.[PreferredTransactionType] , I.[PreferredTransactionType] ,
  D.[AccountID] , I.[AccountID] ,
  D.[TransporterID] , I.[TransporterID] ,
  D.[VehicleID] , I.[VehicleID] ,
  D.[DriverID] , I.[DriverID] ,
  D.[ProductID] , I.[ProductID] ,
  D.[GateID] , I.[GateID] ,
  D.[Charges] , I.[Charges] ,
  D.[TokenExpireDate] , I.[TokenExpireDate] ,
  D.[TokenInterval] , I.[TokenInterval] ,
  D.[TokenStatus] , I.[TokenStatus] 
FROM Deleted D CROSS JOIN Inserted I ' 
GO
/****** Object:  Trigger [dbo].[TRACKUSERAUDIT_TRANSACTIONTOKENLOG_DELETE]    Script Date: 31/01/2019 11:45:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TRACKUSERAUDIT_TRANSACTIONTOKENLOG_DELETE]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TRACKUSERAUDIT_TRANSACTIONTOKENLOG_DELETE] ON [dbo].[TransactionTokenLog] FOR DELETE AS DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;
SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())
IF @UserID > 0 BEGIN SELECT @UserName = UserName FROM [UserMaster] WHERE UserID = @UserID END ELSE BEGIN SET @UserName = ''System Service'' END
INSERT INTO [dbo].[TransactionTokenLog_Tracking] (TranDate, Operation,UserActivity, TranUserID,TranEnvironment,
  [TransactionTokenLogID_old] , [TransactionTokenLogID_new] ,
  [ReceiptTicketID_old] , [ReceiptTicketID_new] ,
  [TokenID_old] , [TokenID_new] ,
  [FreeTime_old] , [FreeTime_new] ,
  [AllocateTime_old] , [AllocateTime_new] ,
  [DeallocateTime_old] , [DeallocateTime_new] ,
  [IsCancel_old] , [IsCancel_new] ,
  [CancelReason_old] , [CancelReason_new] 
) SELECT GetDate(), ''DELETE'', ''TransactionTokenLog information deleted by : '' + @UserName + '' at : '' + CONVERT(VARCHAR(10), GETDATE(), 103) ,dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()),
  [TransactionTokenLogID] , [TransactionTokenLogID] ,
  [ReceiptTicketID] , [ReceiptTicketID] ,
  [TokenID] , [TokenID] ,
  [FreeTime] , [FreeTime] ,
  [AllocateTime] , [AllocateTime] ,
  [DeallocateTime] , [DeallocateTime] ,
  [IsCancel] , [IsCancel] ,
  [CancelReason] , [CancelReason] 
FROM Deleted ' 
GO
/****** Object:  Trigger [dbo].[TRACKUSERAUDIT_TRANSACTIONTOKENLOG_INSERT]    Script Date: 31/01/2019 11:45:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TRACKUSERAUDIT_TRANSACTIONTOKENLOG_INSERT]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TRACKUSERAUDIT_TRANSACTIONTOKENLOG_INSERT] ON [dbo].[TransactionTokenLog] FOR INSERT AS DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;
SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())
IF @UserID > 0 BEGIN SELECT @UserName = UserName FROM [UserMaster] WHERE UserID = @UserID END ELSE BEGIN SET @UserName = ''System Service'' END
INSERT INTO [dbo].[TransactionTokenLog_Tracking] (TranDate, Operation,UserActivity, TranUserID,TranEnvironment,
  [TransactionTokenLogID_old] , [TransactionTokenLogID_new] ,
  [ReceiptTicketID_old] , [ReceiptTicketID_new] ,
  [TokenID_old] , [TokenID_new] ,
  [FreeTime_old] , [FreeTime_new] ,
  [AllocateTime_old] , [AllocateTime_new] ,
  [DeallocateTime_old] , [DeallocateTime_new] ,
  [IsCancel_old] , [IsCancel_new] ,
  [CancelReason_old] , [CancelReason_new] 
) SELECT GetDate(), ''INSERT'', ''TransactionTokenLog information created by : '' + @UserName + '' at : '' + CONVERT(VARCHAR(10), GETDATE(), 103) ,dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()),
  [TransactionTokenLogID] , [TransactionTokenLogID] ,
  [ReceiptTicketID] , [ReceiptTicketID] ,
  [TokenID] , [TokenID] ,
  [FreeTime] , [FreeTime] ,
  [AllocateTime] , [AllocateTime] ,
  [DeallocateTime] , [DeallocateTime] ,
  [IsCancel] , [IsCancel] ,
  [CancelReason] , [CancelReason] 
FROM Inserted GO' 
GO
/****** Object:  Trigger [dbo].[TRACKUSERAUDIT_TRANSACTIONTOKENLOG_UPDATE]    Script Date: 31/01/2019 11:45:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TRACKUSERAUDIT_TRANSACTIONTOKENLOG_UPDATE]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TRACKUSERAUDIT_TRANSACTIONTOKENLOG_UPDATE] ON [dbo].[TransactionTokenLog] FOR UPDATE AS DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;
SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())
IF @UserID > 0 BEGIN SELECT @UserName = UserName FROM [UserMaster] WHERE UserID = @UserID END ELSE BEGIN SET @UserName = ''System Service'' END
INSERT INTO [dbo].[TransactionTokenLog_Tracking] (TranDate, Operation,UserActivity, TranUserID, TranEnvironment,
  [TransactionTokenLogID_old] , [TransactionTokenLogID_new] ,
  [ReceiptTicketID_old] , [ReceiptTicketID_new] ,
  [TokenID_old] , [TokenID_new] ,
  [FreeTime_old] , [FreeTime_new] ,
  [AllocateTime_old] , [AllocateTime_new] ,
  [DeallocateTime_old] , [DeallocateTime_new] ,
  [IsCancel_old] , [IsCancel_new] ,
  [CancelReason_old] , [CancelReason_new] 
) SELECT GetDate(), ''UPDATE'', ''TransactionTokenLog information modified by : '' + @UserName + '' at : '' + CONVERT(VARCHAR(10), GETDATE(), 103) ,dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()),
  D.[TransactionTokenLogID] , I.[TransactionTokenLogID] ,
  D.[ReceiptTicketID] , I.[ReceiptTicketID] ,
  D.[TokenID] , I.[TokenID] ,
  D.[FreeTime] , I.[FreeTime] ,
  D.[AllocateTime] , I.[AllocateTime] ,
  D.[DeallocateTime] , I.[DeallocateTime] ,
  D.[IsCancel] , I.[IsCancel] ,
  D.[CancelReason] , I.[CancelReason] 
FROM Deleted D CROSS JOIN Inserted I ' 
GO
select Has_Perms_By_Name(N'dbo.Token', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Token', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Token', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO





