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

