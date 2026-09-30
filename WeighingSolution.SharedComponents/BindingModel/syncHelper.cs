using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using WeighingSolution.DataLayer;

namespace WeighingSolution.SharedComponents
{
    public static class syncHelper
    {

        public static void Provision(string ClientConnectionString, Dictionary<string, List<string>> tables)
        {
            try
            {
                // connect to server database
                SqlConnection conn = new SqlConnection(ClientConnectionString);

                // create a server scope provisioning object based on the syncscope
                SqlSyncScopeProvisioning serverprovision = new SqlSyncScopeProvisioning(conn);
                serverprovision.ObjectPrefix = "syncfx";

                //Check if scope not exists than and only provision DB
                if (!serverprovision.ScopeExists("wbSyncScope"))
                {
                    // define a new scope named syncscope
                    DbSyncScopeDescription scopedesc = new DbSyncScopeDescription("wbSyncScope");
                    // get the description of the customer & product table from server database
                    foreach (KeyValuePair<string, List<string>> item in tables)
                    {
                        DbSyncTableDescription datatabledesc = SqlSyncDescriptionBuilder.GetDescriptionForTable(item.Key, conn);
                        foreach (string strFk in item.Value)
                        {
                            datatabledesc.Constraints.Add(new DbSyncForeignKeyConstraint(strFk));
                        }
                        scopedesc.Tables.Add(datatabledesc);
                    }

                    //note that it is important to call this after the tables have been added to the scope
                    serverprovision.PopulateFromScopeDescription(scopedesc);

                    // skipping the creation of table since table already exists on server
                    serverprovision.SetCreateTableDefault(DbSyncCreationOption.Skip);
                    // start the provisioning process
                    serverprovision.Apply();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Path.Combine(Constant.SyncLog, $"SyncServiceTrace_{DateTime.Now:yyyy_MM_dd_HH}.txt"), ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "------------------------------------------------------------------" + Environment.NewLine);
            }
        }

        public static void ExecuteSync(string ClientConnectionString, string ServerConnectionString)
        {
            try
            {
                SqlConnection serverConn = new SqlConnection(ServerConnectionString);
                SqlConnection clientConn = new SqlConnection(ClientConnectionString);
                // create the sync orhcestrator
                SyncOrchestrator syncOrchestrator = new SyncOrchestrator();

                // set local provider of orchestrator to a sync provider associated with the 
                // SyncScope in the client database
                syncOrchestrator.LocalProvider = new SqlSyncProvider("wbSyncScope", clientConn, "syncfx");

                // set the remote provider of orchestrator to a server sync provider associated with
                // the SyncScope in the server database
                syncOrchestrator.RemoteProvider = new SqlSyncProvider("wbSyncScope", serverConn, "syncfx");
                //((SqlSyncProvider)syncOrchestrator.RemoteProvider).Configuration.CollisionConflictResolutionPolicy = CollisionConflictResolutionPolicy.SourceWins;
                //((SqlSyncProvider)syncOrchestrator.RemoteProvider).Configuration.ConflictResolutionPolicy = ConflictResolutionPolicy.SourceWins;

                // set the direction of sync session to Upload and Download
                syncOrchestrator.Direction = SyncDirectionOrder.UploadAndDownload;
                //syncOrchestrator.Direction = SyncDirectionOrder.Upload;

                // subscribe for errors that occur when applying changes to the client
                //((SqlSyncProvider)syncOrchestrator.LocalProvider).ApplyChangeFailed += new EventHandler<DbApplyChangeFailedEventArgs>(Program_ApplyChangeFailed);
                ((SqlSyncProvider)syncOrchestrator.RemoteProvider).ApplyChangeFailed += new EventHandler<DbApplyChangeFailedEventArgs>(Program_ApplyChangeFailed);

                // execute the synchronization process
                SyncOperationStatistics syncStats = syncOrchestrator.Synchronize();
            }
            catch (Exception ex)
            {
                File.AppendAllText(Path.Combine(Constant.SyncLog, $"SyncServiceTrace_{DateTime.Now:yyyy_MM_dd_HH}.txt"), ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "------------------------------------------------------------------" + Environment.NewLine);
            }

        }

        static void Program_ApplyChangeFailed(object sender, DbApplyChangeFailedEventArgs e)
        {
            // display conflict type

            string ConflictMsg = $"Conflict: {e.Conflict.ErrorMessage} {Environment.NewLine}" +
                $"Conflict: {e.Conflict.Type} {Environment.NewLine}";

            File.AppendAllText(Path.Combine(Constant.SyncLog, $"SyncServiceTrace_{DateTime.Now:yyyy_MM_dd_HH}.txt"), ConflictMsg);

            string ConflictType = e.Conflict.Type.ToString();
            if (!String.IsNullOrEmpty(ConflictType))
            {
                if (e.Conflict.LocalChange != null && e.Conflict.LocalChange.Rows != null && e.Conflict.LocalChange.Rows.Count > 0)
                {
                    for (int i = 0; i < e.Conflict.LocalChange.Rows.Count; i++)
                    {
                        if (ConflictType == "LocalDeleteRemoteUpdate")
                            File.AppendAllText(Path.Combine(Constant.SyncLog, $"SyncServiceTrace_{DateTime.Now:yyyy_MM_dd_HH}.txt"), "Server has already deleted this row. Would you like to overwrite your changes on server?" + Environment.NewLine);

                        else if (ConflictType == "LocalUpdateRemoteDelete")
                            File.AppendAllText(Path.Combine(Constant.SyncLog, $"SyncServiceTrace_{DateTime.Now:yyyy_MM_dd_HH}.txt"), "Server had Updated this row. Would you like to continue Deleting it?" + Environment.NewLine);

                        else if (ConflictType == "LocalUpdateRemoteUpdate")
                            File.AppendAllText(Path.Combine(Constant.SyncLog, $"SyncServiceTrace_{DateTime.Now:yyyy_MM_dd_HH}.txt"), "Server is also Updating this value. Would you like to overwrite your changes on server?" + Environment.NewLine);
                    }
                }

                if (e.Conflict.RemoteChange != null && e.Conflict.RemoteChange.Rows != null && e.Conflict.RemoteChange.Rows.Count > 0)
                {
                    for (int i = 0; i < e.Conflict.RemoteChange.Rows.Count; i++)
                    {
                        if (ConflictType == "LocalDeleteRemoteUpdate")
                            File.AppendAllText(Path.Combine(Constant.SyncLog, $"SyncServiceTrace_{DateTime.Now:yyyy_MM_dd_HH}.txt"), "Server has already deleted this row. Would you like to overwrite your changes on server?" + Environment.NewLine);

                        else if (ConflictType == "LocalUpdateRemoteDelete")
                            File.AppendAllText(Path.Combine(Constant.SyncLog, $"SyncServiceTrace_{DateTime.Now:yyyy_MM_dd_HH}.txt"), "Server had Updated this row. Would you like to continue Deleting it?" + Environment.NewLine);

                        else if (ConflictType == "LocalUpdateRemoteUpdate")
                            File.AppendAllText(Path.Combine(Constant.SyncLog, $"SyncServiceTrace_{DateTime.Now:yyyy_MM_dd_HH}.txt"), "Server is also Updating this value. Would you like to overwrite your changes on server?" + Environment.NewLine);
                    }
                }
            }
            e.Action = ApplyAction.Continue;
            // display error message 
            if (e.Error != null)
                File.AppendAllText(Path.Combine(Constant.SyncLog, $"SyncServiceTrace_{DateTime.Now:yyyy_MM_dd_HH}.txt"), e.Error.ToString() + Environment.NewLine);
        }
        public static void Deprovision(string ClientConnectionString)
        {
            try
            {
                // connect to server database
                using (SqlConnection conn = new SqlConnection(ClientConnectionString))
                {
                    SqlSyncScopeDeprovisioning serverdeProvision = new SqlSyncScopeDeprovisioning(conn);
                    // start the deprovisioning process
                    serverdeProvision.ObjectPrefix = "syncfx";
                    serverdeProvision.DeprovisionScope("wbSyncScope");
                    serverdeProvision.DeprovisionStore();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Path.Combine(Constant.SyncLog, $"SyncServiceTrace_{DateTime.Now:yyyy_MM_dd_HH}.txt"), ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "------------------------------------------------------------------" + Environment.NewLine);
            }
        }
    }

    public class SyncConstant
    {
        public static string CustomSettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\WeighBridgeSolution\CustomSettings";

        private static string _syncserviceconnectionstring;
        public static string traceFileLocation = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + $"\\WeighBridgeSolution\\Plugins\\SyncServiceTrace_{DateTime.Now:yyyy_MM_dd_HH}.txt";
        public static string LogFileLocation = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + $"\\WeighBridgeSolution\\Plugins\\SyncLogServiceTrace_{DateTime.Now:yyyy_MM_dd_HH}.txt";
        public static string SyncServiceConnectionString
        {
            get
            {
                if (String.IsNullOrEmpty(_syncserviceconnectionstring))
                {
                    DBSettingData objDBSetting = new DBSettingData();
                    if (File.Exists(CustomSettingsPath + @"\Configuration.dat"))
                    {
                        string Data = File.ReadAllText(CustomSettingsPath + @"\Configuration.dat");
                        if (!string.IsNullOrEmpty(Data))
                        {
                            string decryptedData = Common.DecryptData(Data);
                            if (!string.IsNullOrEmpty(decryptedData))
                            {
                                objDBSetting = Newtonsoft.Json.JsonConvert.DeserializeObject<DBSettingData>(decryptedData);
                                if (objDBSetting != null)
                                {
                                    if (objDBSetting.Port > 0)
                                        _syncserviceconnectionstring = "Data Source=" + objDBSetting.Server + "," + objDBSetting.Port + ";Initial Catalog=" + objDBSetting.DBName + ";user id=" + objDBSetting.UserId + ";password=" + objDBSetting.password + ";";
                                    else
                                        _syncserviceconnectionstring = "Data Source=" + objDBSetting.Server + ";Initial Catalog=" + objDBSetting.DBName + ";user id=" + objDBSetting.UserId + ";password=" + objDBSetting.password + ";";
                                }
                            }
                        }
                    }
                }
                return _syncserviceconnectionstring;
            }
            set { _syncserviceconnectionstring = value; }
        }

    }
}
