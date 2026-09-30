using System.Collections.Generic;
using System.Linq;

namespace WeighingSolution.DataLayer
{
    public partial class Account : NotifyDataErrorInfoBase
    {
        protected override IEnumerable<string> VerifyBusinessLogic(string propertyName)
        {
            switch (propertyName)
            {
                case "CompanyName":
                    using (var ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                    {
                        if (AccountID == 0)
                        {
                            if (ctx.Accounts.Any(x => x.CompanyName.ToUpper() == CompanyName.ToUpper()))
                            {
                                yield return "Company name already exist , can not add duplicate company name.";
                            }
                        }
                        else
                        {
                            if (ctx.Accounts.Any(x => x.CompanyName.ToUpper() == CompanyName.ToUpper() && x.AccountID != AccountID))
                            {
                                yield return "Company name already exist , can not add duplicate company name.";
                            }
                        }
                    }
                    break;
            }
        }
    }
}
