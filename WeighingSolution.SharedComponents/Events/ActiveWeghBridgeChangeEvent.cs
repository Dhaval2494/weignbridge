using Prism.Events;
using WeighingSolution.DataLayer;

namespace WeighingSolution.Events
{
    public class ActiveWeghBridgeChangeEvent : PubSubEvent<ActiveWeghBridgeChangeEventArgs>
    {
    }
    public class ActiveWeghBridgeChangeEventArgs
    {
        public WeightBridge actiavtedWeighBridge { get; set; }
    }
}
