using Prism.Events;
using System.Collections.ObjectModel;
using WeighingSolution.DataLayer;

namespace WeighingSolution.Events
{
    public class EditTransactionDetailEvent : PubSubEvent<EditTransactionDetailEventArgs>
    {
    }

    public class EditTransactionDetailEventArgs
    {
        public ObservableCollection<TransactionDetail> lstTranDetails { get; set; }
    }

}
