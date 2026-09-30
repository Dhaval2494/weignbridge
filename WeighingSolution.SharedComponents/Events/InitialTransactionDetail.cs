using Prism.Events;
using System.Windows.Controls;
using WeighingSolution.DataLayer;

namespace WeighingSolution.Events
{
    public class InitialTransactionEvent : PubSubEvent<InitialTransactionEventArgs>
    {
    }

    public class InitialTransactionEventArgs
    {
        public TransactionDetail TranDetail { get; set; }
        public TransactionData TranData { get; set; }
        public Page page { get; set; }
    }
}
