

/****** Object:  Table [dbo].[Camera]    Script Date: 08/10/2018 3:58:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Camera]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Camera](
	[CameraID] [int] IDENTITY(1,1) NOT NULL,
	[CameraName] [nvarchar](100) NOT NULL,
	[WeightBridgeID] [int] NULL,
	[CameraType] [nvarchar](20) NULL,
	[IPAddress] [nvarchar](20) NULL,
	[Port] [nvarchar](10) NULL,
	[VideosourceURL] [nvarchar](200) NULL,
	[UserName] [nvarchar](50) NULL,
	[CameraPassword] [nvarchar](50) NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_Camera] PRIMARY KEY CLUSTERED 
(
	[CameraID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Camera_Tracking]    Script Date: 08/10/2018 3:58:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Camera_Tracking]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Camera_Tracking](
	[TranID] [int] IDENTITY(1,1) NOT NULL,
	[TranDate] [datetime] NULL,
	[Operation] [varchar](20) NULL,
	[UserActivity] [varchar](max) NULL,
	[TranUserID] [int] NULL,
	[TranEnvironment] [varchar](20) NULL,
	[CameraID_old] [int] NOT NULL,
	[CameraID_new] [int] NOT NULL,
	[CameraName_old] [nvarchar](100) NOT NULL,
	[CameraName_new] [nvarchar](100) NOT NULL,
	[WeightBridgeID_old] [int] NULL,
	[WeightBridgeID_new] [int] NULL,
	[CameraType_old] [nvarchar](20) NULL,
	[CameraType_new] [nvarchar](20) NULL,
	[IPAddress_old] [nvarchar](20) NULL,
	[IPAddress_new] [nvarchar](20) NULL,
	[Port_old] [nvarchar](10) NULL,
	[Port_new] [nvarchar](10) NULL,
	[VideosourceURL_old] [nvarchar](200) NULL,
	[VideosourceURL_new] [nvarchar](200) NULL,
	[UserName_old] [nvarchar](50) NULL,
	[UserName_new] [nvarchar](50) NULL,
	[CameraPassword_old] [nvarchar](50) NULL,
	[CameraPassword_new] [nvarchar](50) NULL,
	[Active_old] [bit] NOT NULL,
	[Active_new] [bit] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CameraImage]    Script Date: 08/10/2018 3:58:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CameraImage]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CameraImage](
	[CameraImageID] [int] IDENTITY(1,1) NOT NULL,
	[CameraID] [int] NOT NULL,
	[TransactionDetailID] [int] NULL,
	[ImageLocation] [nvarchar](500) NOT NULL,
	[IsSelected] [bit] NOT NULL,
 CONSTRAINT [PK_CameraImage] PRIMARY KEY CLUSTERED 
(
	[CameraImageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[CameraImage_Tracking]    Script Date: 08/10/2018 3:58:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CameraImage_Tracking]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CameraImage_Tracking](
	[TranID] [int] IDENTITY(1,1) NOT NULL,
	[TranDate] [datetime] NULL,
	[Operation] [varchar](20) NULL,
	[UserActivity] [varchar](max) NULL,
	[TranUserID] [int] NULL,
	[TranEnvironment] [varchar](20) NULL,
	[CameraImageID_old] [int] NOT NULL,
	[CameraImageID_new] [int] NOT NULL,
	[CameraID_old] [int] NOT NULL,
	[CameraID_new] [int] NOT NULL,
	[TransactionDetailID_old] [int] NULL,
	[TransactionDetailID_new] [int] NULL,
	[ImageLocation_old] [nvarchar](500) NOT NULL,
	[ImageLocation_new] [nvarchar](500) NOT NULL,
	[IsSelected_old] [bit] NOT NULL,
	[IsSelected_new] [bit] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Camera_WeightBridge]') AND parent_object_id = OBJECT_ID(N'[dbo].[Camera]'))
ALTER TABLE [dbo].[Camera]  WITH CHECK ADD  CONSTRAINT [FK_Camera_WeightBridge] FOREIGN KEY([WeightBridgeID])
REFERENCES [dbo].[WeightBridge] ([WeightBridgeID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Camera_WeightBridge]') AND parent_object_id = OBJECT_ID(N'[dbo].[Camera]'))
ALTER TABLE [dbo].[Camera] CHECK CONSTRAINT [FK_Camera_WeightBridge]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CameraImage_Camera]') AND parent_object_id = OBJECT_ID(N'[dbo].[CameraImage]'))
ALTER TABLE [dbo].[CameraImage]  WITH CHECK ADD  CONSTRAINT [FK_CameraImage_Camera] FOREIGN KEY([CameraID])
REFERENCES [dbo].[Camera] ([CameraID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CameraImage_Camera]') AND parent_object_id = OBJECT_ID(N'[dbo].[CameraImage]'))
ALTER TABLE [dbo].[CameraImage] CHECK CONSTRAINT [FK_CameraImage_Camera]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CameraImage_TransactionDetail]') AND parent_object_id = OBJECT_ID(N'[dbo].[CameraImage]'))
ALTER TABLE [dbo].[CameraImage]  WITH CHECK ADD  CONSTRAINT [FK_CameraImage_TransactionDetail] FOREIGN KEY([TransactionDetailID])
REFERENCES [dbo].[TransactionDetail] ([TransactionDetailID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CameraImage_TransactionDetail]') AND parent_object_id = OBJECT_ID(N'[dbo].[CameraImage]'))
ALTER TABLE [dbo].[CameraImage] CHECK CONSTRAINT [FK_CameraImage_TransactionDetail]
GO
/****** Object:  Trigger [dbo].[TRACKUSERAUDIT_CAMERA_DELETE]    Script Date: 08/10/2018 3:58:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TRACKUSERAUDIT_CAMERA_DELETE]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TRACKUSERAUDIT_CAMERA_DELETE] ON [dbo].[Camera] FOR DELETE AS DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;
SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())
IF @UserID > 0 BEGIN SELECT @UserName = UserName FROM [UserMaster] WHERE UserID = @UserID END ELSE BEGIN SET @UserName = ''System Service'' END
INSERT INTO [dbo].[Camera_Tracking] (TranDate, Operation,UserActivity, TranUserID,TranEnvironment,
  [CameraID_old] , [CameraID_new] ,
  [CameraName_old] , [CameraName_new] ,
  [WeightBridgeID_old] , [WeightBridgeID_new] ,
  [CameraType_old], [CameraType_new] ,
  [IPAddress_old], [IPAddress_new],
  [Port_old], [Port_new],
  [VideosourceURL_old], [VideosourceURL_new],
  [UserName_old] , [UserName_new] ,
  [CameraPassword_old] , [CameraPassword_new] ,
  [Active_old] , [Active_new] 
) SELECT GetDate(), ''DELETE'', ''Camera information deleted by : '' + @UserName + '' at : '' + CONVERT(VARCHAR(10), GETDATE(), 103) ,dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()),
  [CameraID] , [CameraID] ,
  [CameraName] , [CameraName] ,
  [WeightBridgeID] , [WeightBridgeID] ,
  [CameraType], [CameraType] ,
  [IPAddress], [IPAddress],
  [Port], [Port],
  [VideosourceURL], [VideosourceURL],
  [UserName] , [UserName] ,
  [CameraPassword] , [CameraPassword] ,
  [Active] , [Active] 
FROM Deleted 

' 
GO
/****** Object:  Trigger [dbo].[TRACKUSERAUDIT_CAMERA_INSERT]    Script Date: 08/10/2018 3:58:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TRACKUSERAUDIT_CAMERA_INSERT]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TRACKUSERAUDIT_CAMERA_INSERT] ON [dbo].[Camera] FOR INSERT AS DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;
SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())
IF @UserID > 0 BEGIN SELECT @UserName = UserName FROM [UserMaster] WHERE UserID = @UserID END ELSE BEGIN SET @UserName = ''System Service'' END
INSERT INTO [dbo].[Camera_Tracking] (TranDate, Operation,UserActivity, TranUserID,TranEnvironment,
  [CameraID_old] , [CameraID_new] ,
  [CameraName_old] , [CameraName_new] ,
  [WeightBridgeID_old] , [WeightBridgeID_new] ,
  [CameraType_old], [CameraType_new] ,
  [IPAddress_old], [IPAddress_new],
  [Port_old], [Port_new],
  [VideosourceURL_old], [VideosourceURL_new],
  [UserName_old] , [UserName_new] ,
  [CameraPassword_old] , [CameraPassword_new] ,
  [Active_old] , [Active_new] 
) SELECT GetDate(), ''INSERT'', ''Camera information created by : '' + @UserName + '' at : '' + CONVERT(VARCHAR(10), GETDATE(), 103) ,dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()),
  [CameraID] , [CameraID] ,
  [CameraName] , [CameraName] ,
  [WeightBridgeID] , [WeightBridgeID] ,
  [CameraType], [CameraType] ,
  [IPAddress], [IPAddress],
  [Port], [Port],
  [VideosourceURL], [VideosourceURL],
  [UserName] , [UserName] ,
  [CameraPassword] , [CameraPassword] ,
  [Active] , [Active] 
FROM Inserted 

' 
GO
/****** Object:  Trigger [dbo].[TRACKUSERAUDIT_CAMERA_UPDATE]    Script Date: 08/10/2018 3:58:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TRACKUSERAUDIT_CAMERA_UPDATE]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TRACKUSERAUDIT_CAMERA_UPDATE] ON [dbo].[Camera] FOR UPDATE AS DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;
SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())
IF @UserID > 0 BEGIN SELECT @UserName = UserName FROM [UserMaster] WHERE UserID = @UserID END ELSE BEGIN SET @UserName = ''System Service'' END
INSERT INTO [dbo].[Camera_Tracking] (TranDate, Operation,UserActivity, TranUserID, TranEnvironment,
  [CameraID_old] , [CameraID_new] ,
  [CameraName_old] , [CameraName_new] ,
  [WeightBridgeID_old] , [WeightBridgeID_new] ,
  [CameraType_old], [CameraType_new] ,
  [IPAddress_old], [IPAddress_new],
  [Port_old], [Port_new],
  [VideosourceURL_old], [VideosourceURL_new],
  [UserName_old] , [UserName_new] ,
  [CameraPassword_old] , [CameraPassword_new] ,
  [Active_old] , [Active_new] 
) SELECT GetDate(), ''UPDATE'', ''Camera information modified by : '' + @UserName + '' at : '' + CONVERT(VARCHAR(10), GETDATE(), 103) ,dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()),
  D.[CameraID] , I.[CameraID] ,
  D.[CameraName] , I.[CameraName] ,
  D.[WeightBridgeID] , I.[WeightBridgeID] ,
  D.[CameraType], I.[CameraType] ,
  D.[IPAddress], I.[IPAddress],
  D.[Port], I.[Port],
  D.[VideosourceURL], I.[VideosourceURL],
  D.[UserName] , I.[UserName] ,
  D.[CameraPassword] , I.[CameraPassword] ,
  D.[Active] , I.[Active] 
FROM Deleted D INNER JOIN Inserted I ON I.CameraID = D.CameraID

' 
GO
/****** Object:  Trigger [dbo].[TRACKUSERAUDIT_CAMERAIMAGE_DELETE]    Script Date: 08/10/2018 3:58:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TRACKUSERAUDIT_CAMERAIMAGE_DELETE]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TRACKUSERAUDIT_CAMERAIMAGE_DELETE] ON [dbo].[CameraImage] FOR DELETE AS DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;
SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())
IF @UserID > 0 BEGIN SELECT @UserName = UserName FROM [UserMaster] WHERE UserID = @UserID END ELSE BEGIN SET @UserName = ''System Service'' END
INSERT INTO [dbo].[CameraImage_Tracking] (TranDate, Operation,UserActivity, TranUserID,TranEnvironment,
  [CameraImageID_old] , [CameraImageID_new] ,
  [CameraID_old] , [CameraID_new] ,
  [TransactionDetailID_old] , [TransactionDetailID_new] ,
  [ImageLocation_old] , [ImageLocation_new] ,
  [IsSelected_old] , [IsSelected_new] 
) SELECT GetDate(), ''DELETE'', ''Camera Image information deleted by : '' + @UserName + '' at : '' + CONVERT(VARCHAR(10), GETDATE(), 103) ,dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()),
  [CameraImageID] , [CameraImageID] ,
  [CameraID] , [CameraID] ,
  [TransactionDetailID] , [TransactionDetailID] ,
  [ImageLocation] , [ImageLocation] ,
  [IsSelected] , [IsSelected] 
FROM Deleted 

' 
GO
/****** Object:  Trigger [dbo].[TRACKUSERAUDIT_CAMERAIMAGE_INSERT]    Script Date: 08/10/2018 3:58:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TRACKUSERAUDIT_CAMERAIMAGE_INSERT]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TRACKUSERAUDIT_CAMERAIMAGE_INSERT] ON [dbo].[CameraImage] FOR INSERT AS DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;
SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())
IF @UserID > 0 BEGIN SELECT @UserName = UserName FROM [UserMaster] WHERE UserID = @UserID END ELSE BEGIN SET @UserName = ''System Service'' END
INSERT INTO [dbo].[CameraImage_Tracking] (TranDate, Operation,UserActivity, TranUserID,TranEnvironment,
  [CameraImageID_old] , [CameraImageID_new] ,
  [CameraID_old] , [CameraID_new] ,
  [TransactionDetailID_old] , [TransactionDetailID_new] ,
  [ImageLocation_old] , [ImageLocation_new] ,
  [IsSelected_old] , [IsSelected_new] 
) SELECT GetDate(), ''INSERT'', ''Camera Image information created by : '' + @UserName + '' at : '' + CONVERT(VARCHAR(10), GETDATE(), 103) ,dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()),
  [CameraImageID] , [CameraImageID] ,
  [CameraID] , [CameraID] ,
  [TransactionDetailID] , [TransactionDetailID] ,
  [ImageLocation] , [ImageLocation] ,
  [IsSelected] , [IsSelected] 
FROM Inserted ' 
GO

/****** Object:  Trigger [dbo].[TRACKUSERAUDIT_CAMERAIMAGE_UPDATE]    Script Date: 08/10/2018 3:58:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[TRACKUSERAUDIT_CAMERAIMAGE_UPDATE]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[TRACKUSERAUDIT_CAMERAIMAGE_UPDATE] ON [dbo].[CameraImage] FOR UPDATE AS DECLARE @UserName NVARCHAR(MAX);
DECLARE @UserID INT;
SELECT @UserID = dbo.GetUserIDFromAppName(APP_NAME())
IF @UserID > 0 BEGIN SELECT @UserName = UserName FROM [UserMaster] WHERE UserID = @UserID END ELSE BEGIN SET @UserName = ''System Service'' END
INSERT INTO [dbo].[CameraImage_Tracking] (TranDate, Operation,UserActivity, TranUserID, TranEnvironment,
  [CameraImageID_old] , [CameraImageID_new] ,
  [CameraID_old] , [CameraID_new] ,
  [TransactionDetailID_old] , [TransactionDetailID_new] ,
  [ImageLocation_old] , [ImageLocation_new] ,
  [IsSelected_old] , [IsSelected_new] 
) SELECT GetDate(), ''UPDATE'', ''Camera Image information modified by : '' + @UserName + '' at : '' + CONVERT(VARCHAR(10), GETDATE(), 103) ,dbo.GetUserIDFromAppName(App_Name()), dbo.GetEnvironmentFromAppName(App_Name()),
  D.[CameraImageID] , I.[CameraImageID] ,
  D.[CameraID] , I.[CameraID] ,
  D.[TransactionDetailID] , I.[TransactionDetailID] ,
  D.[ImageLocation] , I.[ImageLocation] ,
  D.[IsSelected] , I.[IsSelected] 
FROM Deleted D INNER JOIN Inserted I ON I.CameraImageID = D.CameraImageID
' 
GO