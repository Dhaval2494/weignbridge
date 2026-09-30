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
	[SMSEnd] [nvarchar](max) NULL,
 CONSTRAINT [PK_[SMSSetup] PRIMARY KEY CLUSTERED 
(
	[SMSSetupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SMSFieldSetting]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[SMSFieldSetting](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SMSSetupId] [int] NOT NULL,
	[SMSFieldName] [nvarchar](50) NULL,
	[SMSDisplayName] [nvarchar](50) NULL,
	[IsSelected] [bit] NOT NULL,
 CONSTRAINT [PK_SMSFieldSetting] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[SMSFieldSetting]  WITH CHECK ADD  CONSTRAINT [FK_SMSFieldSetting_SMSSetup] FOREIGN KEY([SMSSetupId])
REFERENCES [dbo].[SMSSetup] ([SMSSetupID])

ALTER TABLE [dbo].[SMSFieldSetting] CHECK CONSTRAINT [FK_SMSFieldSetting_SMSSetup]

End
GO