using Prism.Events;
using System;
using WeighingSolution.DataLayer;

namespace WeighingSolution.Events
{
    public class TransactionDetailSavedEvent : PubSubEvent<TransactionDetailSavedEventArgs>
    {
    }
    public class TransactionStatusUpdatedEvent : PubSubEvent<TransactionDetailSavedEventArgs>
    {
    }


    public class TransactionDetailSavedEventArgs
    {
        public Guid ReceiptTicketID { get; set; }
        public TransactionData tranData { get; set; }
        public TransactionDetail tranDetail { get; set; }
        public bool IsTransactionComplete { get; set; }
    }

}
