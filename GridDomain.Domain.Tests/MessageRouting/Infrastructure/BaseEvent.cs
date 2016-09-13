using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.MessageRouting.Infrastructure
{
    public class BaseEvent : DomainEvent
    {
        public string Payload { get; }

        public BaseEvent(Guid sourceId, string payload) : base(sourceId)
        {
            Payload = payload;
        }
    }
}