using Prism.Events;
using WeighingSolution.SharedComponents;

namespace WeighingSolution.Events
{
    public class ViewTransactionDetailEvent : PubSubEvent<ViewTransactionDetailEventArgs>
    {
    }

    public class ViewTransactionDetailEventArgs
    {
        public TransactionDataBindingModel tranData { get; set; }
    }

}
