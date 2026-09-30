
/****** Object:  Table [dbo].[SMSFieldSetting]    Script Date: 12-04-2022 17:36:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

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
GO

ALTER TABLE [dbo].[SMSFieldSetting]  WITH CHECK ADD  CONSTRAINT [FK_SMSFieldSetting_SMSSetup] FOREIGN KEY([SMSSetupId])
REFERENCES [dbo].[SMSSetup] ([SMSSetupID])
GO

ALTER TABLE [dbo].[SMSFieldSetting] CHECK CONSTRAINT [FK_SMSFieldSetting_SMSSetup]
GO

