SET IDENTITY_INSERT [dbo].[CustomField] ON 
GO
INSERT [dbo].[CustomField] ([CustomFieldID], [FieldName], [DisplayName], [IsRequired], [MaxLength], [IsVisible]) VALUES (1, N'Field1', N'Transporter', 0, 0, 1)
GO
INSERT [dbo].[CustomField] ([CustomFieldID], [FieldName], [DisplayName], [IsRequired], [MaxLength], [IsVisible]) VALUES (2, N'Field2', N'Driver', 0, 0, 1)
GO
INSERT [dbo].[CustomField] ([CustomFieldID], [FieldName], [DisplayName], [IsRequired], [MaxLength], [IsVisible]) VALUES (3, N'Field3', N'Challan No', 0, 0, 1)
GO
INSERT [dbo].[CustomField] ([CustomFieldID], [FieldName], [DisplayName], [IsRequired], [MaxLength], [IsVisible]) VALUES (4, N'Field4', N'Field4', 0, 0, 0)
GO
INSERT [dbo].[CustomField] ([CustomFieldID], [FieldName], [DisplayName], [IsRequired], [MaxLength], [IsVisible]) VALUES (5, N'Field5', N'Field5', 0, 0, 0)
GO
INSERT [dbo].[CustomField] ([CustomFieldID], [FieldName], [DisplayName], [IsRequired], [MaxLength], [IsVisible]) VALUES (6, N'Field6', N'Field6', 0, 0, 0)
GO
INSERT [dbo].[CustomField] ([CustomFieldID], [FieldName], [DisplayName], [IsRequired], [MaxLength], [IsVisible]) VALUES (7, N'Field7', N'Field7', 0, 0, 0)
GO
INSERT [dbo].[CustomField] ([CustomFieldID], [FieldName], [DisplayName], [IsRequired], [MaxLength], [IsVisible]) VALUES (8, N'Field8', N'Field8', 0, 0, 0)
GO
INSERT [dbo].[CustomField] ([CustomFieldID], [FieldName], [DisplayName], [IsRequired], [MaxLength], [IsVisible]) VALUES (9, N'Field9', N'Field9', 0, 0, 0)
GO
INSERT [dbo].[CustomField] ([CustomFieldID], [FieldName], [DisplayName], [IsRequired], [MaxLength], [IsVisible]) VALUES (10, N'Field10', N'Field10', 0, 0, 0)
GO
INSERT [dbo].[CustomField] ([CustomFieldID], [FieldName], [DisplayName], [IsRequired], [MaxLength], [IsVisible]) VALUES (11, N'Field11', N'Field11', 0, 0, 0)
GO
INSERT [dbo].[CustomField] ([CustomFieldID], [FieldName], [DisplayName], [IsRequired], [MaxLength], [IsVisible]) VALUES (12, N'Field12', N'Field12', 0, 0, 0)
GO
INSERT [dbo].[CustomField] ([CustomFieldID], [FieldName], [DisplayName], [IsRequired], [MaxLength], [IsVisible]) VALUES (13, N'Field13', N'Field13', 0, 0, 0)
GO
INSERT [dbo].[CustomField] ([CustomFieldID], [FieldName], [DisplayName], [IsRequired], [MaxLength], [IsVisible]) VALUES (14, N'Field14', N'Field14', 0, 0, 0)
GO
INSERT [dbo].[CustomField] ([CustomFieldID], [FieldName], [DisplayName], [IsRequired], [MaxLength], [IsVisible]) VALUES (15, N'Field15', N'Field15', 0, 0, 0)
GO
INSERT [dbo].[CustomField] ([CustomFieldID], [FieldName], [DisplayName], [IsRequired], [MaxLength], [IsVisible]) VALUES (16, N'Field16', N'Field16', 0, 0, 0)
GO
INSERT [dbo].[CustomField] ([CustomFieldID], [FieldName], [DisplayName], [IsRequired], [MaxLength], [IsVisible]) VALUES (17, N'Field17', N'Field17', 0, 0, 0)
GO
INSERT [dbo].[CustomField] ([CustomFieldID], [FieldName], [DisplayName], [IsRequired], [MaxLength], [IsVisible]) VALUES (18, N'Field18', N'Field18', 0, 0, 0)
GO
INSERT [dbo].[CustomField] ([CustomFieldID], [FieldName], [DisplayName], [IsRequired], [MaxLength], [IsVisible]) VALUES (19, N'Field19', N'Field19', 0, 0, 0)
GO
INSERT [dbo].[CustomField] ([CustomFieldID], [FieldName], [DisplayName], [IsRequired], [MaxLength], [IsVisible]) VALUES (20, N'Field20', N'Field20', 0, 0, 0)
GO
SET IDENTITY_INSERT [dbo].[CustomField] OFF
GO

SET IDENTITY_INSERT [dbo].[Feature] ON 
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (1, N'TransactionManagement', N'R', CAST(1.00 AS Decimal(5, 3)), NULL, 1)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (2, N'Transaction', N'CR', CAST(1.01 AS Decimal(5, 3)), 1, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (3, N'PrintReceipt', N'E', CAST(1.02 AS Decimal(5, 3)), 2, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (4, N'VoidTransaction', N'E', CAST(1.03 AS Decimal(5, 3)), 2, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (5, N'AllowManualEntry', N'E', CAST(1.04 AS Decimal(5, 3)), 2, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (6, N'PrintStarForManualEntry', N'E', CAST(1.05 AS Decimal(5, 3)), 2, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (7, N'DataSetup', N'R', CAST(2.00 AS Decimal(5, 3)), NULL, 1)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (8, N'UserRole', N'CRU', CAST(2.01 AS Decimal(5, 3)), 7, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (9, N'User', N'CRU', CAST(2.02 AS Decimal(5, 3)), 7, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (10, N'Unit', N'CRU', CAST(2.03 AS Decimal(5, 3)), 7, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (11, N'WeighBridge', N'CRU', CAST(2.04 AS Decimal(5, 3)), 7, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (12, N'Account', N'CRU', CAST(2.06 AS Decimal(5, 3)), 7, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (13, N'Vehicle', N'CRU', CAST(2.07 AS Decimal(5, 3)), 7, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (14, N'Driver', N'CRU', CAST(2.08 AS Decimal(5, 3)), 7, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (15, N'Product', N'CRU', CAST(2.09 AS Decimal(5, 3)), 7, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (16, N'Gate', N'CRU', CAST(2.10 AS Decimal(5, 3)), 7, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (17, N'Reports', N'R', CAST(3.00 AS Decimal(5, 3)), NULL, 1)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (18, N'MasterReport', N'R', CAST(3.01 AS Decimal(5, 3)), 17, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (19, N'UserRoleReport', N'R', CAST(3.011 AS Decimal(5, 3)), 18, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (20, N'UserReport', N'R', CAST(3.012 AS Decimal(5, 3)), 18, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (21, N'UnitReport', N'R', CAST(3.013 AS Decimal(5, 3)), 18, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (22, N'WeighBridgeReport', N'R', CAST(3.014 AS Decimal(5, 3)), 18, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (23, N'AccountReport', N'R', CAST(3.016 AS Decimal(5, 3)), 18, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (24, N'VehicleReport', N'R', CAST(3.017 AS Decimal(5, 3)), 18, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (25, N'DriverReport', N'R', CAST(3.018 AS Decimal(5, 3)), 18, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (26, N'ProductReport', N'R', CAST(3.019 AS Decimal(5, 3)), 18, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (27, N'GateReport', N'R', CAST(3.0191 AS Decimal(5, 3)), 18, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (28, N'AuditReport', N'R', CAST(3.02 AS Decimal(5, 3)), 17, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (29, N'TransactionReport', N'R', CAST(3.03 AS Decimal(5, 3)), 17, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (30, N'TranSummaryReport', N'R', CAST(3.031 AS Decimal(5, 3)), 29, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (31, N'TranDetailReport', N'R', CAST(3.032 AS Decimal(5, 3)), 29, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (32, N'Settings', N'R', CAST(4.00 AS Decimal(5, 3)), NULL, 1)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (33, N'DatabaseSetting', N'RU', CAST(4.01 AS Decimal(5, 3)), 32, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (34, N'GlobalSetting', N'RU', CAST(4.02 AS Decimal(5, 3)), 32, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (35, N'CompanySetting', N'RU', CAST(4.03 AS Decimal(5, 3)), 32, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (36, N'FieldSetting', N'RU', CAST(4.04 AS Decimal(5, 3)), 32, 0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (37, N'PluginManagement',N'RU',CAST(4.05 AS Decimal(5,3)),32,0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (38, N'LicenseManagement',N'RU',CAST(4.06 AS Decimal(5,3)),32,0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (39, N'EmailSetup',N'CRU',CAST(4.07 AS Decimal(5,3)),32,0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (40, N'PrintSetting',N'CRU',CAST(4.07 AS Decimal(5,3)),32,0);
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (41, N'SMSSetup',N'CRU',CAST(4.07 AS Decimal(5,3)),32,0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (42, N'BarcodeSetup',N'CRU',CAST(4.07 AS Decimal(5,3)),32,0)
GO
INSERT [dbo].[Feature] ([FeatureID], [FeatureName], [Action], [SortOrder], [ParentFeatureID], [IsMaster]) VALUES (43, N'Camera',N'CRU',CAST(4.07 AS Decimal(5,3)),32,0)
GO
SET IDENTITY_INSERT [dbo].[Feature] OFF
GO

SET IDENTITY_INSERT [dbo].[MasterField] ON 
GO
INSERT [dbo].[MasterField] ([MasterFieldID], [FieldName], [Isvisible], [IsRequired]) VALUES (1, N'Account', 0, 0)
GO
INSERT [dbo].[MasterField] ([MasterFieldID], [FieldName], [Isvisible], [IsRequired]) VALUES (2, N'Transporter', 0, 0)
GO
INSERT [dbo].[MasterField] ([MasterFieldID], [FieldName], [Isvisible], [IsRequired]) VALUES (3, N'Driver', 0, 0)
GO
INSERT [dbo].[MasterField] ([MasterFieldID], [FieldName], [Isvisible], [IsRequired]) VALUES (4, N'Product', 1, 0)
GO
INSERT [dbo].[MasterField] ([MasterFieldID], [FieldName], [Isvisible], [IsRequired]) VALUES (5, N'Gate', 0, 0)
GO
INSERT [dbo].[MasterField] ([MasterFieldID], [FieldName], [Isvisible], [IsRequired]) VALUES (6, N'Charges', 0, 0)
GO
SET IDENTITY_INSERT [dbo].[MasterField] OFF
GO

INSERT INTO Template VALUES ('Admin',1)
GO
INSERT INTO Template VALUES ('User',1)
GO
INSERT INTO TemplatePermission VALUES(1,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(2,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(3,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(4,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(5,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(6,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(7,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(8,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(9,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(10,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(11,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(12,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(13,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(14,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(15,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(16,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(17,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(18,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(19,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(20,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(21,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(22,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(23,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(24,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(25,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(26,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(27,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(28,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(29,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(30,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(31,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(32,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(33,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(34,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(35,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(36,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(37,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(38,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(39,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(40,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(41,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(42,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES(43,1,N'11111')
GO
INSERT INTO TemplatePermission VALUES (1, 2, N'11111')
GO
INSERT INTO TemplatePermission VALUES (2, 2, N'11101')
GO
INSERT INTO TemplatePermission VALUES (3, 2, N'11111')
GO
INSERT INTO TemplatePermission VALUES (4, 2, N'11110')
GO
INSERT INTO TemplatePermission VALUES (5, 2, N'11110')
GO
INSERT INTO TemplatePermission VALUES (6, 2, N'11111')
GO
INSERT INTO TemplatePermission VALUES (7, 2, N'11111')
GO
INSERT INTO TemplatePermission VALUES (8, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (9, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (10, 2, N'11111')
GO
INSERT INTO TemplatePermission VALUES (11, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (12, 2, N'11111')
GO
INSERT INTO TemplatePermission VALUES (13, 2, N'11111')
GO
INSERT INTO TemplatePermission VALUES (14, 2, N'11111')
GO
INSERT INTO TemplatePermission VALUES (15, 2, N'11111')
GO
INSERT INTO TemplatePermission VALUES (16, 2, N'11111')
GO
INSERT INTO TemplatePermission VALUES (17, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (18, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (19, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (20, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (21, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (22, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (23, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (24, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (25, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (26, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (27, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (28, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (29, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (30, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (31, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (32, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (33, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (34, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (35, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (36, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (37, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (38, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (39, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (40, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (41, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (42, 2, N'00000')
GO
INSERT INTO TemplatePermission VALUES (43, 2, N'00000')
GO

INSERT INTO UserMaster([FirstName],[LastName],[ContactNo],[Email],[Address],[Notes],[UserName],[Password],[Active],[TemplateID],[Salt]) VALUES('Admin',	'User',	NULL,NULL,NUll,NUll,'admin','19JKo2B/ddE=',1,1,'JssKlcNTxylpQVSSWWxMoi1bQA5xlJhVmUkKeg7PZ8g=')
GO
INSERT INTO UserMaster([FirstName],[LastName],[ContactNo],[Email],[Address],[Notes],[UserName],[Password],[Active],[TemplateID],[Salt]) VALUES('Operator',	'User',	NULL,NULL,NUll,NUll,'operator','d3gvOvf8KR4cVU+nsvHf9g==',1,2,'Ygy7tASmgO69zlcm7vw0bYlOvYREgv4BPadh9vglTi8=')
GO
INSERT INTO UserMaster([FirstName],[LastName],[ContactNo],[Email],[Address],[Notes],[UserName],[Password],[Active],[TemplateID],[Salt]) VALUES('SVT_admin','User',	NULL,NULL,NUll,NUll,'svt_sauser','CsVid5IfoQEcVU+nsvHf9g==',1,1,'lecMngDR08Czk5AIQhtnUZkXrGo4Ih967zjRdRwuxqc=')
GO
INSERT INTO UserMaster([FirstName],[LastName],[ContactNo],[Email],[Address],[Notes],[UserName],[Password],[Active],[TemplateID],[Salt]) VALUES('EndelSupport','User',	NULL,NULL,NUll,NUll,'endelsupport','drGhifYnz+KI9nYIj/XGyGD5WHODs27x',1,1,'YA6a0wIlIlVHqIr6Cl9QKClEL0NzugdIQiGu1mONtnQ=')
GO
INSERT INTO UNIT VALUES ('Kg')
GO
INSERT INTO UNIT VALUES ('Lbs')
GO

INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('GlobalSetting','EntryDateEditable','boolean','False')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('GlobalSetting','AllowManualEntry','boolean','False')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('GlobalSetting','DisbleFrom2ndPass','boolean','True')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('GlobalSetting','DisplayReceiptAfter1pass','boolean','True')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('GlobalSetting','TareWeightDiff','Integer',1)
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('GlobalSetting','AllowWithoutTareWeightChange','boolean','False')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('GlobalSetting','PrintSlipNotes','boolean','False')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('GlobalSetting','DateFormat','String','dd-MM-yyyy')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('GlobalSetting','TimeFormat','String','hh:mm tt')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('GlobalSetting','DecimalFormat','String','0.####')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('GlobalSetting','BarcodeOrQR','String','')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('GlobalSetting','IsOpenVehicleFO','boolean','True')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('GlobalSetting','IsOpenTransporterFO','boolean','True')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('GlobalSetting','IsOpenAccountFO','boolean','True')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('GlobalSetting','IsOpenProductFO','boolean','True')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('GlobalSetting','IsOpenDriverFO','boolean','True')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('CompanySetting','CompanyName','String','Company Name')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('CompanySetting','Address','String','Address')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('CompanySetting','Phone','String','9979612343')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('CompanySetting','Email','String','ID@server.com')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('CompanySetting','ClientCompanyName','String','Client Company')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('CompanySetting','ClientContactPerson','String','Contact Person')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('CompanySetting','ClientAddress','String','Client Address')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('CompanySetting','SupportID','String','support@mail.com')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('CompanySetting','Logo','String','logo.png')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('ThemeSetting','Color','String','Blue')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('ThemeSetting','BaseColor','String','BaseLight')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('EmailSetting', 'ServerName', 'String', 'Localhost') 
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('EmailSetting', 'Port', 'Integer', '25')  
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('EmailSetting', 'UserName', 'String', '')  
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('EmailSetting', 'PassWord', 'String', '')  
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('EmailSetting', 'UseSSL', 'Boolean', 'False')  
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('EmailSetting', 'EmailFrom', 'String', '')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('PrintSetting', 'PrintType', 'String', 'Laser')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES('PrintSetting', 'PrinterName', 'String', '')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'DisableSpecialCharactersInVehicle', 'boolean', 'False')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('GlobalSetting', 'DisplayLastVehicleTransactionDetails', 'boolean', 'False')
GO
INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('Transaction', 'LastTicket', 'int', '0')
GO

IF EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'DatabaseSetting' AND SettingName = 'AutoBackupEnable') 
UPDATE ApplicationSettings SET SettingValue = 'False' WHERE ModuleName = 'DatabaseSetting' AND SettingName = 'AutoBackupEnable' 
ELSE INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('DatabaseSetting', 'AutoBackupEnable', 'Boolean', 'False')
GO
IF EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'DatabaseSetting' AND SettingName = 'AutoBackupHour') 
UPDATE ApplicationSettings SET SettingValue = '1' WHERE ModuleName = 'DatabaseSetting' AND SettingName = 'AutoBackupHour' 
ELSE INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('DatabaseSetting', 'AutoBackupHour', 'Integer', '1')
GO
IF EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'DatabaseSetting' AND SettingName = 'AutoBackupMinute') 
UPDATE ApplicationSettings SET SettingValue = '1' WHERE ModuleName = 'DatabaseSetting' AND SettingName = 'AutoBackupMinute' 
ELSE INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('DatabaseSetting', 'AutoBackupMinute', 'Integer', '0')
GO
IF EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'DatabaseSetting' AND SettingName = 'AutoBackupMeridiem') 
UPDATE ApplicationSettings SET SettingValue = 'AM' WHERE ModuleName = 'DatabaseSetting' AND SettingName = 'AutoBackupMeridiem' 
ELSE INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('DatabaseSetting', 'AutoBackupMeridiem', 'String', 'AM')
GO
IF EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'DatabaseSetting' AND SettingName = 'AutoBackupFilePath') 
UPDATE ApplicationSettings SET SettingValue = '' WHERE ModuleName = 'DatabaseSetting' AND SettingName = 'AutoBackupFilePath' 
ELSE INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('DatabaseSetting', 'AutoBackupFilePath', 'String', '')
GO

IF EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'SMSSetting' AND SettingName = 'ProviderName') 
UPDATE ApplicationSettings SET SettingValue = 'Localhost' WHERE ModuleName = 'SMSSetting' AND SettingName = 'ProviderName' 
ELSE INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('SMSSetting', 'ProviderName', 'String', '')
GO
IF EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'BarcodeSetting' AND SettingName = 'BarcodeImagePath') 
UPDATE ApplicationSettings SET SettingValue = 'C:\\ProgramData\\WeighBridgeSolution\\CustomImages\\BarcodeImages' WHERE ModuleName = 'BarcodeSetting' AND SettingName = 'BarcodeImagePath' 
ELSE INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('BarcodeSetting', 'BarcodeImagePath', 'String', 'C:\\ProgramData\\WeighBridgeSolution\\CustomImages\\BarcodeImages') 
GO

IF EXISTS (SELECT * FROM ApplicationSettings WHERE ModuleName = 'BarcodeSetting' AND SettingName = 'BarcodeImagePathType') 
UPDATE ApplicationSettings SET SettingValue = 'LocalImagePath' WHERE ModuleName = 'BarcodeSetting' AND SettingName = 'BarcodeImagePathType' 
ELSE INSERT INTO ApplicationSettings ([ModuleName],[SettingName],[SettingType],[SettingValue]) VALUES ('BarcodeSetting', 'BarcodeImagePathType', 'String', 'LocalImagePath')
GO

SET IDENTITY_INSERT [dbo].[WeightBridge] ON 
INSERT INTO [dbo].[WeightBridge] ([WeightBridgeID],[ScaleName],[MaxCapacity],[IncrementSize],[UnitID],[IsActive],[ConnectionType],[IPAddress],[IPPort],[COMPort],[BaudRate],[DataBits],[StopBits],[Parity],[TotalStringLength],[StartStringChar],[StringCharLength],[Format],[StringStartingValue],[IsDecimalInString],[DecimalPointLocation],[IsReverseString],[WEIGHTSTARTFROM],[WEIGHTLENGTH],[HandShake])
VALUES (1,'Scale1','100000',NULL,1,1,'COM',NULL,NULL,NULL,'2400',8,'One','None',10,'',NULL,'#######0',NULL,0,0,0,'4','7','None')
SET IDENTITY_INSERT [dbo].[WeightBridge] OFF
GO
