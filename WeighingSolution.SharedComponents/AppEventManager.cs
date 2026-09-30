using Prism.Events;

namespace WeighingSolution.DataLayer
{
    //Singleton Implementation for shared EventAggregator
    public sealed class AppEventManager
    {
        private static readonly AppEventManager instance = new AppEventManager();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static AppEventManager()
        {
        }

        private AppEventManager()
        {
        }

        public static AppEventManager Instance
        {
            get
            {
                return instance;
            }
        }
        public IEventAggregator EventAggregator = new EventAggregator();
    }
}
