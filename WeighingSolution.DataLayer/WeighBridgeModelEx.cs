using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace WeighingSolution.DataLayer
{
    public partial class weighbridgeEntities : DbContext
    {
        public weighbridgeEntities(string ConnectionString)
            : base(ConnectionString)
        {
            var objectContext = (this as IObjectContextAdapter).ObjectContext;
            string AppName = string.Empty;
            if (Constant.LoggedInUserID > 0)
                AppName = "Application Name=UserID:" + Constant.LoggedInUserID.ToString() + ";";
            else
                AppName = "Application Name=UserID:-1;";
            (objectContext.Connection as System.Data.Entity.Core.EntityClient.EntityConnection).StoreConnection.ConnectionString += ";" + AppName;
        }
    }
}
