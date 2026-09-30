using System.Collections.Generic;

namespace WeighingSolution.DataLayer
{
    public partial class TransactionDetail : NotifyDataErrorInfoBase
    {
        protected override IEnumerable<string> ValidateProperty(string propertyName)
        {
            switch (propertyName)
            {
                case "ProductID":
                    if (TransactionData != null && TransactionData.ProductMaster != null && TransactionData.ProductMaster.IsRequired)
                    {
                        if (ProductID == null)
                            yield return "Product is required.";
                    }
                    break;
                case "GateID":
                    if (TransactionData != null && TransactionData.GateMaster != null && TransactionData.GateMaster.IsRequired)
                    {
                        if (GateID == null)
                            yield return "Gate is required.";
                    }
                    break;
            }
        }
    }
}
