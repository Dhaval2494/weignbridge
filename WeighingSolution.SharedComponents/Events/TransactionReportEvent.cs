using Prism.Events;

namespace WeighingSolution.Events
{

    public class TransactionReportEvent : PubSubEvent<TransactionReportEventArgs>
    {
    }
    public class TransactionReportEventArgs
    {
        public string Strwhereclause { get; set; }

        public string Strfiltertext { get; set; }
    }
}
