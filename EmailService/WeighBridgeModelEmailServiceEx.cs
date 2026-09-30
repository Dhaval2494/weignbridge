using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace EmailService
{
    public partial class Endel_weighbridgeEntities : DbContext
    {
        public Endel_weighbridgeEntities(string ConnectionString) : base(ConnectionString)
        {
            var objectContext = (this as IObjectContextAdapter).ObjectContext;
            string AppName = "Application Name=UserID:-1;";
            //if (WeighingSolution.DataLayer.Constant.LoggedInUserID > 0)
            //    AppName = "Application Name=UserID:" + WeighingSolution.DataLayer.Constant.LoggedInUserID.ToString() + ";";
            //else
            //     AppName = "Application Name=UserID:-1;";
            (objectContext.Connection as System.Data.Entity.Core.EntityClient.EntityConnection).StoreConnection.ConnectionString += ";" + AppName;
        }
    }
}
