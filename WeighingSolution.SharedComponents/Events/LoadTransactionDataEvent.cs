using Prism.Events;
using System.Collections.ObjectModel;
using WeighingSolution.DataLayer;

namespace WeighingSolution.SharedComponents.Events
{
    public class LoadTransactionDataEvent : PubSubEvent<LoadTransactionDataEventArgs>
    {
    }

    public class LoadTransactionDataEventArgs
    {
        public ObservableCollection<TransactionData> lstTranData { get; set; }
    }

    public class LoadTransactionDataBindingModelEvent : PubSubEvent<LoadTransactionDataBindingModelEventArgs>
    {
    }

    public class LoadTransactionDataBindingModelEventArgs
    {
        public ObservableCollection<TransactionDataBindingModel> lstTranData { get; set; }
    }
}
