using Prism.Events;
using WeighingSolution.DataLayer;

namespace WeighingSolution.Events
{
    public class BeforeWeightCaptureEvent : PubSubEvent<BeforeWeightCaptureEventArgs>
    {
    }

    public class BeforeWeightCaptureEventArgs
    {
        public bool isAligned { get; set; }
        public WeightBridge SelectedWeighBridge { get; set; }
    }
}
