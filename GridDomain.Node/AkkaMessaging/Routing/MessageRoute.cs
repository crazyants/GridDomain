using System;

namespace GridDomain.Node.AkkaMessaging.Routing
{
    public class MessageRoute
    {
        public MessageRoute(Type messageType, string correlationField = null)
        {
            MessageType = messageType;
            CorrelationField = correlationField;
            Topic = MessageType.FullName;
        }

        public string Topic { get; }
        public Type MessageType { get; }
        public string CorrelationField { get; }
    }
}