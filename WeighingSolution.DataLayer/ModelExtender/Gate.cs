using System.Collections.Generic;
using System.Linq;

namespace WeighingSolution.DataLayer
{
    public partial class Gate : NotifyDataErrorInfoBase
    {
        protected override IEnumerable<string> VerifyBusinessLogic(string propertyName)
        {
            switch (propertyName)
            {
                case "GateName":
                    using (var ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                    {
                        if (GateID == 0)
                        {
                            if (ctx.Gates.Any(x => x.GateName.ToUpper() == GateName.ToUpper()))
                            {
                                yield return "Gate name already exist, can not add duplicate gate.";
                            }
                        }
                        else
                        {
                            if (ctx.Gates.Any(x => x.GateName.ToUpper() == GateName.ToUpper() && x.GateID != GateID))
                            {
                                yield return "Gate name already exist, can not add duplicate gate.";
                            }
                        }
                    }
                    break;
            }
        }
    }
}
