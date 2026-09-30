using Prism.Events;

namespace WeighingSolution.Events
{
    public class AfterDetailSavedEvent : PubSubEvent<AfterDetailSavedEventArgs>
    {
    }

    public class AfterDetailSavedEventArgs
    {
        public int ObjectID { get; set; }
        public object ObjectInstance { get; set; }
        public string ObjectType { get; set; }
    }

    public class Change_tokenEvent : PubSubEvent<Change_tokenEventArgs>
    {
    }
    public class AfterChange_TokenEvent : PubSubEvent<Change_tokenEventArgs>
    {
    }
    public class Change_tokenEventArgs
    {
        public int TokenID { get; set; }
        public string TokenName { get; set; }
        public string TokenValue { get; set; }
    }

    public class AfterLoginEvent : PubSubEvent<AfterLoginEventargs>
    {

    }

    public class AfterLoginEventargs
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int? TemplateID { get; set; }

    }
}
