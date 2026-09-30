using Prism.Events;

namespace WeighingSolution.Events
{

    public class DisallowtodeactivemasterEvent : PubSubEvent<DisallowtodeactivemasterEventArgs>
    {
    }
    public class DisallowtodeactivemasterEventArgs
    {
        public int ObjectID { get; set; }
        public object Objectinstance { get; set; }
        public string Message { get; set; }
    }
}
