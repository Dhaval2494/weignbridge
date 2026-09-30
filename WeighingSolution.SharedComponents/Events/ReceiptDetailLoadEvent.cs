using Microsoft.Reporting.WinForms;
using Prism.Events;
using System.Windows.Forms.Integration;
using WeighingSolution.DataLayer;


namespace WeighingSolution.Events
{
    public class ReceiptDetailLoadEvent : PubSubEvent<ReceiptDetailLoadEventArgs>
    {
    }
    public class ReceiptDetailAfterLoadEvent : PubSubEvent<ReceiptDetailLoadEventArgs>
    {
    }
    public class ReceiptDetailLoadEventArgs
    {
        public ReportViewer reportViewer { get; set; }
        public TransactionData tranData { get; set; }
        public WindowsFormsHost windform { get; set; }
    }
    public class BeforeReceiptDetailLoadEvent : PubSubEvent<BeforeReceiptDetailLoadEventArgs>
    {
    }
    public class BeforeReceiptDetailLoadEventArgs
    {
        public bool IsReceiptDetailLoad { get; set; }
        public TransactionData tranData { get; set; }

    }
}
