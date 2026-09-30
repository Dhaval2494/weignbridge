using Prism.Events;

namespace WeighingSolution.Events
{
    public class AfterDetailSaveCancelledEvent : PubSubEvent<AfterDetailSaveCancelledEventArgs>
    {
    }

    public class AfterDetailSaveCancelledEventArgs
    {
        public object ObjectInstance { get; set; }
    }
}
