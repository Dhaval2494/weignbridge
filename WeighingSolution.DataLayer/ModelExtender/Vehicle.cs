using System.Collections.Generic;
using System.Linq;

namespace WeighingSolution.DataLayer
{
    public partial class Vehicle : NotifyDataErrorInfoBase
    {

        public virtual string AccountName
        {
            get { return this.Account != null ? this.Account.CompanyName : ""; }
        }
        protected override IEnumerable<string> VerifyBusinessLogic(string propertyName)
        {
            switch (propertyName)
            {
                case "VehicleNumber":
                    using (var ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                    {
                        if (VehicleID == 0)
                        {
                            if (ctx.Vehicles.Any(x => x.VehicleNumber.ToUpper() == VehicleNumber.ToUpper()))
                            {
                                yield return "Vehicle Number already exist, can not add duplicate Vehicle.";
                            }
                        }
                        else
                        {
                            if (ctx.Vehicles.Any(x => x.VehicleNumber.ToUpper() == VehicleNumber.ToUpper() && x.VehicleID != VehicleID))
                            {
                                yield return "Vehicle Number already exist, can not add duplicate Vehicle.";
                            }
                        }
                    }
                    break;
            }
        }
    }
}
