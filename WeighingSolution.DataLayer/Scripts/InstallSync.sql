IF NOT EXISTS (SELECT *  FROM information_schema.table_constraints  WHERE CONSTRAINT_NAME = 'PK_Account_Tracking'   AND table_name = 'Account_Tracking')
BEGIN
	ALTER TABLE [Account_Tracking] ADD CONSTRAINT [PK_Account_Tracking] PRIMARY KEY CLUSTERED ( [TranID] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END

IF NOT EXISTS (SELECT *  FROM information_schema.table_constraints  WHERE CONSTRAINT_NAME = 'PK_ApplicationSettings_Tracking'   AND table_name = 'ApplicationSettings_Tracking')
BEGIN
	ALTER TABLE [ApplicationSettings_Tracking] ADD CONSTRAINT [PK_ApplicationSettings_Tracking] PRIMARY KEY CLUSTERED ( [TranID] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END

IF NOT EXISTS (SELECT *  FROM information_schema.table_constraints  WHERE CONSTRAINT_NAME = 'PK_CustomField_Tracking'   AND table_name = 'CustomField_Tracking')
BEGIN
	ALTER TABLE [CustomField_Tracking] ADD CONSTRAINT [PK_CustomField_Tracking] PRIMARY KEY CLUSTERED ( [TranID] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END

IF NOT EXISTS (SELECT *  FROM information_schema.table_constraints  WHERE CONSTRAINT_NAME = 'PK_CustomValue_Tracking'   AND table_name = 'CustomValue_Tracking')
BEGIN
	ALTER TABLE [CustomValue_Tracking] ADD CONSTRAINT [PK_CustomValue_Tracking] PRIMARY KEY CLUSTERED ( [TranID] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END

IF NOT EXISTS (SELECT *  FROM information_schema.table_constraints  WHERE CONSTRAINT_NAME = 'PK_Driver_Tracking'   AND table_name = 'Driver_Tracking')
BEGIN
	ALTER TABLE [Driver_Tracking] ADD CONSTRAINT [PK_Driver_Tracking] PRIMARY KEY CLUSTERED ( [TranID] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END

IF NOT EXISTS (SELECT *  FROM information_schema.table_constraints  WHERE CONSTRAINT_NAME = 'PK_Gate_Tracking'   AND table_name = 'Gate_Tracking')
BEGIN
	ALTER TABLE [Gate_Tracking] ADD CONSTRAINT [PK_Gate_Tracking] PRIMARY KEY CLUSTERED ( [TranID] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END

IF NOT EXISTS (SELECT *  FROM information_schema.table_constraints  WHERE CONSTRAINT_NAME = 'PK_MasterField_Tracking'   AND table_name = 'MasterField_Tracking')
BEGIN
	ALTER TABLE [MasterField_Tracking] ADD CONSTRAINT [PK_MasterField_Tracking] PRIMARY KEY CLUSTERED ( [TranID] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END

IF NOT EXISTS (SELECT *  FROM information_schema.table_constraints  WHERE CONSTRAINT_NAME = 'PK_Product_Tracking'   AND table_name = 'Product_Tracking')
BEGIN
	ALTER TABLE [Product_Tracking] ADD CONSTRAINT [PK_Product_Tracking] PRIMARY KEY CLUSTERED ( [TranID] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END

IF NOT EXISTS (SELECT *  FROM information_schema.table_constraints  WHERE CONSTRAINT_NAME = 'PK_Template_Tracking'   AND table_name = 'Template_Tracking')
BEGIN
	ALTER TABLE [Template_Tracking] ADD CONSTRAINT [PK_Template_Tracking] PRIMARY KEY CLUSTERED ( [TranID] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END

IF NOT EXISTS (SELECT *  FROM information_schema.table_constraints  WHERE CONSTRAINT_NAME = 'PK_TemplatePermission_Tracking'   AND table_name = 'TemplatePermission_Tracking')
BEGIN
	ALTER TABLE [TemplatePermission_Tracking] ADD CONSTRAINT [PK_TemplatePermission_Tracking] PRIMARY KEY CLUSTERED ( [TranID] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END

IF NOT EXISTS (SELECT *  FROM information_schema.table_constraints  WHERE CONSTRAINT_NAME = 'PK_TransactionData_Tracking'   AND table_name = 'TransactionData_Tracking')
BEGIN
	ALTER TABLE [TransactionData_Tracking] ADD CONSTRAINT [PK_TransactionData_Tracking] PRIMARY KEY CLUSTERED ( [TranID] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END

IF NOT EXISTS (SELECT *  FROM information_schema.table_constraints  WHERE CONSTRAINT_NAME = 'PK_TransactionDetail_Tracking'   AND table_name = 'TransactionDetail_Tracking')
BEGIN
	ALTER TABLE [TransactionDetail_Tracking] ADD CONSTRAINT [PK_TransactionDetail_Tracking] PRIMARY KEY CLUSTERED ( [TranID] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END

IF NOT EXISTS (SELECT *  FROM information_schema.table_constraints  WHERE CONSTRAINT_NAME = 'PK_Unit_Tracking'   AND table_name = 'Unit_Tracking')
BEGIN
	ALTER TABLE [Unit_Tracking] ADD CONSTRAINT [PK_Unit_Tracking] PRIMARY KEY CLUSTERED ( [TranID] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END

IF NOT EXISTS (SELECT *  FROM information_schema.table_constraints  WHERE CONSTRAINT_NAME = 'PK_UserMaster_Tracking'   AND table_name = 'UserMaster_Tracking')
BEGIN
	ALTER TABLE [UserMaster_Tracking] ADD CONSTRAINT [PK_UserMaster_Tracking] PRIMARY KEY CLUSTERED ( [TranID] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END

IF NOT EXISTS (SELECT *  FROM information_schema.table_constraints  WHERE CONSTRAINT_NAME = 'PK_Vehicle_Tracking'   AND table_name = 'Vehicle_Tracking')
BEGIN
	ALTER TABLE [Vehicle_Tracking] ADD CONSTRAINT [PK_Vehicle_Tracking] PRIMARY KEY CLUSTERED ( [TranID] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END

IF NOT EXISTS (SELECT *  FROM information_schema.table_constraints  WHERE CONSTRAINT_NAME = 'PK_WeightBridge_Tracking'   AND table_name = 'WeightBridge_Tracking')
BEGIN
	ALTER TABLE [WeightBridge_Tracking] ADD CONSTRAINT [PK_WeightBridge_Tracking] PRIMARY KEY CLUSTERED ( [TranID] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END

IF NOT EXISTS (SELECT *  FROM information_schema.table_constraints  WHERE CONSTRAINT_NAME = 'PK_Log'   AND table_name = 'Log')
BEGIN
	ALTER TABLE [Log] ADD CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED ( [Id] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
END