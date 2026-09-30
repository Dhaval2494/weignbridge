using System;
using System.Collections.Generic;
using System.Linq;

namespace WeighingSolution.DataLayer
{
    public partial class UhfMaster : NotifyDataErrorInfoBase
    {
        protected override IEnumerable<string> VerifyBusinessLogic(string propertyName)
        {
            switch (propertyName)
            {
                case "WeightbridgeID":

                    if (WeightbridgeID < 1)
                    {
                        yield return "Please select Weightbridge Side";
                    }
                    break;

                case "IpAddress":
                    using (var ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                    {
                        if (!String.IsNullOrEmpty(IpAddress))
                        {
                            if (ID == 0)
                            {
                                if (ctx.UhfMasters.Any(x => x.IpAddress.ToLower().Trim() == IpAddress.ToLower().Trim()))
                                {
                                    yield return "Ip Address already exist, can not add duplicate Ip Address.";
                                }
                            }
                            else
                            {
                                if (ctx.UhfMasters.Any(x => x.IpAddress.ToLower().Trim() == IpAddress.ToLower().Trim() && x.ID != ID))
                                {
                                    yield return "Ip Address already exist, can not add duplicate Ip Address.";
                                }
                            }
                        }
                        else
                        {
                            yield return "Please Add Ip Address";
                        }
                    }
                    break;
                case "WeightbridgeSide":
                    using (var ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                    {
                        if (!String.IsNullOrEmpty(WeightbridgeSide))
                        {
                            if (ID == 0)
                            {
                                if (ctx.UhfMasters.Any(x => x.WeightbridgeID == WeightbridgeID && x.WeightbridgeSide.ToLower().Trim() == WeightbridgeSide.ToLower().Trim()))
                                {
                                    yield return "Weightbridge Side already exist, can not add duplicate Weightbridge Side.";
                                }
                            }
                            else
                            {
                                if (ctx.UhfMasters.Any(x => x.WeightbridgeID == WeightbridgeID && x.WeightbridgeSide.ToLower().Trim() == WeightbridgeSide.ToLower().Trim() && x.ID != ID))
                                {
                                    yield return "Weightbridge Side already exist, can not add duplicate Weightbridge Side.";
                                }
                            }
                        }
                        else
                        {
                            yield return "Please select Weightbridge Side";
                        }
                    }
                    break;
            }
        }
    }
}
