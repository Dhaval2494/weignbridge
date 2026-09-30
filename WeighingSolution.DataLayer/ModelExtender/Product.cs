using System;
using System.Collections.Generic;
using System.Linq;

namespace WeighingSolution.DataLayer
{
    public partial class Product : NotifyDataErrorInfoBase
    {
        protected override IEnumerable<string> VerifyBusinessLogic(string propertyName)
        {
            switch (propertyName)
            {
                case "ProductCode":
                    using (var ctx = new weighbridgeEntities(Constant.EntityConnectionString))
                    {
                        if (!String.IsNullOrEmpty(ProductCode))
                        {
                            if (ProductID == 0)
                            {
                                if (ctx.Products.Any(x => x.ProductCode.ToLower() == ProductCode.ToLower()))
                                {
                                    yield return "Product code already exist, can not add duplicate product code.";
                                }
                            }
                            else
                            {
                                if (ctx.Products.Any(x => x.ProductCode.ToLower() == ProductCode.ToLower() && x.ProductID != ProductID))
                                {
                                    yield return "Product code already exist, can not add duplicate product code.";
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }
}
