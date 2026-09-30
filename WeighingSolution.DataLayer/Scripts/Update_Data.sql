---------------------------------------------------------------
-- Disable Triggers Before All Updates
---------------------------------------------------------------
IF EXISTS (
    SELECT 1 FROM sys.triggers 
    WHERE name = 'TRACKUSERAUDIT_TRANSACTIONDATA_UPDATE' 
    AND parent_id = OBJECT_ID('TransactionData')
)
BEGIN
    ALTER TABLE TransactionData DISABLE TRIGGER TRACKUSERAUDIT_TRANSACTIONDATA_UPDATE;
END;

IF EXISTS (
    SELECT 1 FROM sys.triggers 
    WHERE name = 'TRACKUSERAUDIT_TRANSACTIONDETAIL_UPDATE' 
    AND parent_id = OBJECT_ID('TransactionDetail')
)
BEGIN
    ALTER TABLE TransactionDetail DISABLE TRIGGER TRACKUSERAUDIT_TRANSACTIONDETAIL_UPDATE;
END;
GO

---------------------------------------------------------------
-- Delete Duplicate CustomValue Records
---------------------------------------------------------------
IF EXISTS (
    SELECT 1 
    FROM CustomValue
    GROUP BY ReceiptTicketID, CustomFieldID
    HAVING COUNT(*) > 1
)
BEGIN
    ;WITH Duplicates AS (
        SELECT 
            CustomValueID,
            ROW_NUMBER() OVER (
                PARTITION BY ReceiptTicketID, CustomFieldID 
                ORDER BY CustomValueID
            ) AS rn
        FROM CustomValue
    )
    DELETE FROM CustomValue
    WHERE CustomValueID IN (
        SELECT CustomValueID FROM Duplicates WHERE rn > 1
    );
END;
GO

---------------------------------------------------------------
-- Update TransactionData CustomValue Fields (1..20)
---------------------------------------------------------------
-- CustomValue1
IF EXISTS (SELECT 1 FROM TransactionData WHERE CustomValue1 IS NULL OR CustomValue1 = '')
BEGIN
    UPDATE td
    SET td.CustomValue1 = cv.CustomValue
    FROM TransactionData td
    INNER JOIN CustomValue cv ON cv.ReceiptTicketID = td.ReceiptTicketID AND cv.CustomFieldID = 1
    WHERE td.CustomValue1 IS NULL OR td.CustomValue1 = '';
END
GO

-- CustomValue2
IF EXISTS (SELECT 1 FROM TransactionData WHERE CustomValue2 IS NULL OR CustomValue2 = '')
BEGIN
    UPDATE td
    SET td.CustomValue2 = cv.CustomValue
    FROM TransactionData td
    INNER JOIN CustomValue cv ON cv.ReceiptTicketID = td.ReceiptTicketID AND cv.CustomFieldID = 2
    WHERE td.CustomValue2 IS NULL OR td.CustomValue2 = '';
END
GO

-- CustomValue3
IF EXISTS (SELECT 1 FROM TransactionData WHERE CustomValue3 IS NULL OR CustomValue3 = '')
BEGIN
    UPDATE td
    SET td.CustomValue3 = cv.CustomValue
    FROM TransactionData td
    INNER JOIN CustomValue cv ON cv.ReceiptTicketID = td.ReceiptTicketID AND cv.CustomFieldID = 3
    WHERE td.CustomValue3 IS NULL OR td.CustomValue3 = '';
END
GO

-- CustomValue4
IF EXISTS (SELECT 1 FROM TransactionData WHERE CustomValue4 IS NULL OR CustomValue4 = '')
BEGIN
    UPDATE td
    SET td.CustomValue4 = cv.CustomValue
    FROM TransactionData td
    INNER JOIN CustomValue cv ON cv.ReceiptTicketID = td.ReceiptTicketID AND cv.CustomFieldID = 4
    WHERE td.CustomValue4 IS NULL OR td.CustomValue4 = '';
END
GO

-- CustomValue5
IF EXISTS (SELECT 1 FROM TransactionData WHERE CustomValue5 IS NULL OR CustomValue5 = '')
BEGIN
    UPDATE td
    SET td.CustomValue5 = cv.CustomValue
    FROM TransactionData td
    INNER JOIN CustomValue cv ON cv.ReceiptTicketID = td.ReceiptTicketID AND cv.CustomFieldID = 5
    WHERE td.CustomValue5 IS NULL OR td.CustomValue5 = '';
END
GO

-- CustomValue6
IF EXISTS (SELECT 1 FROM TransactionData WHERE CustomValue6 IS NULL OR CustomValue6 = '')
BEGIN
    UPDATE td
    SET td.CustomValue6 = cv.CustomValue
    FROM TransactionData td
    INNER JOIN CustomValue cv ON cv.ReceiptTicketID = td.ReceiptTicketID AND cv.CustomFieldID = 6
    WHERE td.CustomValue6 IS NULL OR td.CustomValue6 = '';
END
GO

-- CustomValue7
IF EXISTS (SELECT 1 FROM TransactionData WHERE CustomValue7 IS NULL OR CustomValue7 = '')
BEGIN
    UPDATE td
    SET td.CustomValue7 = cv.CustomValue
    FROM TransactionData td
    INNER JOIN CustomValue cv ON cv.ReceiptTicketID = td.ReceiptTicketID AND cv.CustomFieldID = 7
    WHERE td.CustomValue7 IS NULL OR td.CustomValue7 = '';
END
GO

-- CustomValue8
IF EXISTS (SELECT 1 FROM TransactionData WHERE CustomValue8 IS NULL OR CustomValue8 = '')
BEGIN
    UPDATE td
    SET td.CustomValue8 = cv.CustomValue
    FROM TransactionData td
    INNER JOIN CustomValue cv ON cv.ReceiptTicketID = td.ReceiptTicketID AND cv.CustomFieldID = 8
    WHERE td.CustomValue8 IS NULL OR td.CustomValue8 = '';
END
GO

-- CustomValue9
IF EXISTS (SELECT 1 FROM TransactionData WHERE CustomValue9 IS NULL OR CustomValue9 = '')
BEGIN
    UPDATE td
    SET td.CustomValue9 = cv.CustomValue
    FROM TransactionData td
    INNER JOIN CustomValue cv ON cv.ReceiptTicketID = td.ReceiptTicketID AND cv.CustomFieldID = 9
    WHERE td.CustomValue9 IS NULL OR td.CustomValue9 = '';
END
GO

-- CustomValue10
IF EXISTS (SELECT 1 FROM TransactionData WHERE CustomValue10 IS NULL OR CustomValue10 = '')
BEGIN
    UPDATE td
    SET td.CustomValue10 = cv.CustomValue
    FROM TransactionData td
    INNER JOIN CustomValue cv ON cv.ReceiptTicketID = td.ReceiptTicketID AND cv.CustomFieldID = 10
    WHERE td.CustomValue10 IS NULL OR td.CustomValue10 = '';
END
GO

-- CustomValue11
IF EXISTS (SELECT 1 FROM TransactionData WHERE CustomValue11 IS NULL OR CustomValue11 = '')
BEGIN
    UPDATE td
    SET td.CustomValue11 = cv.CustomValue
    FROM TransactionData td
    INNER JOIN CustomValue cv ON cv.ReceiptTicketID = td.ReceiptTicketID AND cv.CustomFieldID = 11
    WHERE td.CustomValue11 IS NULL OR td.CustomValue11 = '';
END
GO

-- CustomValue12
IF EXISTS (SELECT 1 FROM TransactionData WHERE CustomValue12 IS NULL OR CustomValue12 = '')
BEGIN
    UPDATE td
    SET td.CustomValue12 = cv.CustomValue
    FROM TransactionData td
    INNER JOIN CustomValue cv ON cv.ReceiptTicketID = td.ReceiptTicketID AND cv.CustomFieldID = 12
    WHERE td.CustomValue12 IS NULL OR td.CustomValue12 = '';
END
GO

-- CustomValue13
IF EXISTS (SELECT 1 FROM TransactionData WHERE CustomValue13 IS NULL OR CustomValue13 = '')
BEGIN
    UPDATE td
    SET td.CustomValue13 = cv.CustomValue
    FROM TransactionData td
    INNER JOIN CustomValue cv ON cv.ReceiptTicketID = td.ReceiptTicketID AND cv.CustomFieldID = 13
    WHERE td.CustomValue13 IS NULL OR td.CustomValue13 = '';
END
GO

-- CustomValue14
IF EXISTS (SELECT 1 FROM TransactionData WHERE CustomValue14 IS NULL OR CustomValue14 = '')
BEGIN
    UPDATE td
    SET td.CustomValue14 = cv.CustomValue
    FROM TransactionData td
    INNER JOIN CustomValue cv ON cv.ReceiptTicketID = td.ReceiptTicketID AND cv.CustomFieldID = 14
    WHERE td.CustomValue14 IS NULL OR td.CustomValue14 = '';
END
GO

-- CustomValue15
IF EXISTS (SELECT 1 FROM TransactionData WHERE CustomValue15 IS NULL OR CustomValue15 = '')
BEGIN
    UPDATE td
    SET td.CustomValue15 = cv.CustomValue
    FROM TransactionData td
    INNER JOIN CustomValue cv ON cv.ReceiptTicketID = td.ReceiptTicketID AND cv.CustomFieldID = 15
    WHERE td.CustomValue15 IS NULL OR td.CustomValue15 = '';
END
GO

-- CustomValue16
IF EXISTS (SELECT 1 FROM TransactionData WHERE CustomValue16 IS NULL OR CustomValue16 = '')
BEGIN
    UPDATE td
    SET td.CustomValue16 = cv.CustomValue
    FROM TransactionData td
    INNER JOIN CustomValue cv ON cv.ReceiptTicketID = td.ReceiptTicketID AND cv.CustomFieldID = 16
    WHERE td.CustomValue16 IS NULL OR td.CustomValue16 = '';
END
GO

-- CustomValue17
IF EXISTS (SELECT 1 FROM TransactionData WHERE CustomValue17 IS NULL OR CustomValue17 = '')
BEGIN
    UPDATE td
    SET td.CustomValue17 = cv.CustomValue
    FROM TransactionData td
    INNER JOIN CustomValue cv ON cv.ReceiptTicketID = td.ReceiptTicketID AND cv.CustomFieldID = 17
    WHERE td.CustomValue17 IS NULL OR td.CustomValue17 = '';
END
GO

-- CustomValue18
IF EXISTS (SELECT 1 FROM TransactionData WHERE CustomValue18 IS NULL OR CustomValue18 = '')
BEGIN
    UPDATE td
    SET td.CustomValue18 = cv.CustomValue
    FROM TransactionData td
    INNER JOIN CustomValue cv ON cv.ReceiptTicketID = td.ReceiptTicketID AND cv.CustomFieldID = 18
    WHERE td.CustomValue18 IS NULL OR td.CustomValue18 = '';
END
GO

-- CustomValue19
IF EXISTS (SELECT 1 FROM TransactionData WHERE CustomValue19 IS NULL OR CustomValue19 = '')
BEGIN
    UPDATE td
    SET td.CustomValue19 = cv.CustomValue
    FROM TransactionData td
    INNER JOIN CustomValue cv ON cv.ReceiptTicketID = td.ReceiptTicketID AND cv.CustomFieldID = 19
    WHERE td.CustomValue19 IS NULL OR td.CustomValue19 = '';
END
GO

-- CustomValue20
IF EXISTS (SELECT 1 FROM TransactionData WHERE CustomValue20 IS NULL OR CustomValue20 = '')
BEGIN
    UPDATE td
    SET td.CustomValue20 = cv.CustomValue
    FROM TransactionData td
    INNER JOIN CustomValue cv ON cv.ReceiptTicketID = td.ReceiptTicketID AND cv.CustomFieldID = 20
    WHERE td.CustomValue20 IS NULL OR td.CustomValue20 = '';
END
GO

---------------------------------------------------------------
-- TransactionDetail Table Updates (All with IF EXISTS guards)
---------------------------------------------------------------

-- WB_User_Name
IF EXISTS (
    SELECT 1 FROM TransactionDetail 
    WHERE UserID > 0 AND (WB_User_Name IS NULL OR WB_User_Name = '')
)
BEGIN
    UPDATE td
    SET td.WB_User_Name = um.UserName
    FROM TransactionDetail td
    INNER JOIN UserMaster um ON um.UserID = td.UserID
    WHERE td.UserID > 0 AND (td.WB_User_Name IS NULL OR td.WB_User_Name = '');
END;
GO

-- Gate_Name
IF EXISTS (
    SELECT 1 FROM TransactionDetail 
    WHERE GateID > 0 AND (Gate_Name IS NULL OR Gate_Name = '')
)
BEGIN
    UPDATE td
    SET td.Gate_Name = g.GateName
    FROM TransactionDetail td
    INNER JOIN Gate g ON g.GateID = td.GateID
    WHERE td.GateID > 0 AND (td.Gate_Name IS NULL OR td.Gate_Name = '');
END;
GO

-- Product_Name
IF EXISTS (
    SELECT 1 FROM TransactionDetail 
    WHERE ProductID > 0 AND (Product_Name IS NULL OR Product_Name = '')
)
BEGIN
    UPDATE td
    SET td.Product_Name = p.ProductName
    FROM TransactionDetail td
    INNER JOIN Product p ON p.ProductID = td.ProductID
    WHERE td.ProductID > 0 AND (td.Product_Name IS NULL OR td.Product_Name = '');
END;
GO

-- WeightBridgeName
IF EXISTS (
    SELECT 1 FROM TransactionDetail 
    WHERE WeightBridgeID > 0 AND (WeightBridgeName IS NULL OR WeightBridgeName = '')
)
BEGIN
    UPDATE td
    SET td.WeightBridgeName = wb.ScaleName
    FROM TransactionDetail td
    INNER JOIN WeightBridge wb ON wb.WeightBridgeID = td.WeightBridgeID
    WHERE td.WeightBridgeID > 0 AND (td.WeightBridgeName IS NULL OR td.WeightBridgeName = '');
END;
GO

---------------------------------------------------------------
-- TransactionData Table Updates (All with IF EXISTS guards)
---------------------------------------------------------------

-- Transporter_Name
IF EXISTS (
    SELECT 1 FROM TransactionData 
    WHERE TransporterID > 0 AND (Transporter_Name IS NULL OR Transporter_Name = '')
)
BEGIN
    UPDATE td
    SET td.Transporter_Name = a.CompanyName
    FROM TransactionData td
    INNER JOIN Account a ON a.AccountID = td.TransporterID
    WHERE td.TransporterID > 0 AND (td.Transporter_Name IS NULL OR td.Transporter_Name = '');
END;
GO

-- Account_Name
IF EXISTS (
    SELECT 1 FROM TransactionData 
    WHERE AccountID > 0 AND (Account_Name IS NULL OR Account_Name = '')
)
BEGIN
    UPDATE td
    SET td.Account_Name = a.CompanyName
    FROM TransactionData td
    INNER JOIN Account a ON a.AccountID = td.AccountID
    WHERE td.AccountID > 0 AND (td.Account_Name IS NULL OR td.Account_Name = '');
END;
GO

-- Driver_Name
IF EXISTS (
    SELECT 1 FROM TransactionData 
    WHERE DriverID > 0 AND (Driver_Name IS NULL OR Driver_Name = '')
)
BEGIN
    UPDATE td
    SET td.Driver_Name = d.FirstName + ' ' + d.LastName
    FROM TransactionData td
    INNER JOIN Driver d ON d.DriverID = td.DriverID
    WHERE td.DriverID > 0 AND (td.Driver_Name IS NULL OR td.Driver_Name = '');
END;
GO

-- Vehicle_Number
IF EXISTS (
    SELECT 1 FROM TransactionData 
    WHERE VehicleID > 0 AND (Vehicle_Number IS NULL OR Vehicle_Number = '')
)
BEGIN
    UPDATE td
    SET td.Vehicle_Number = v.VehicleNumber
    FROM TransactionData td
    INNER JOIN Vehicle v ON v.VehicleID = td.VehicleID
    WHERE td.VehicleID > 0 AND (td.Vehicle_Number IS NULL OR td.Vehicle_Number = '');
END;
GO

-- CreatedName
IF EXISTS (
    SELECT 1 FROM TransactionData 
    WHERE CreatedBy > 0 AND (CreatedName IS NULL OR CreatedName = '')
)
BEGIN
    UPDATE td
    SET td.CreatedName = um.UserName
    FROM TransactionData td
    INNER JOIN UserMaster um ON um.UserID = td.CreatedBy
    WHERE td.CreatedBy > 0 AND (td.CreatedName IS NULL OR td.CreatedName = '');
END;
GO

---------------------------------------------------------------
-- Enable Triggers After All Updates
---------------------------------------------------------------
IF EXISTS (
    SELECT 1 FROM sys.triggers 
    WHERE name = 'TRACKUSERAUDIT_TRANSACTIONDATA_UPDATE' 
    AND parent_id = OBJECT_ID('TransactionData')
)
BEGIN
    ALTER TABLE TransactionData ENABLE TRIGGER TRACKUSERAUDIT_TRANSACTIONDATA_UPDATE;
END;

IF EXISTS (
    SELECT 1 FROM sys.triggers 
    WHERE name = 'TRACKUSERAUDIT_TRANSACTIONDETAIL_UPDATE' 
    AND parent_id = OBJECT_ID('TransactionDetail')
)
BEGIN
    ALTER TABLE TransactionDetail ENABLE TRIGGER TRACKUSERAUDIT_TRANSACTIONDETAIL_UPDATE;
END;
GO
