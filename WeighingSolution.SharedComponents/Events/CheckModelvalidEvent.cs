using Prism.Events;

namespace WeighingSolution.Events
{

    public class CheckModelvalidEvent : PubSubEvent<CheckModelvalidEventArgs>
    {
    }
    public class CheckModelvalidEventArgs
    {
        public bool IsValid { get; set; }
    }
}
