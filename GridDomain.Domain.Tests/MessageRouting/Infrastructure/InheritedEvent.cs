using System;

namespace GridDomain.Tests.MessageRouting.Infrastructure
{
    public sealed class InheritedEvent : BaseEvent
    {
        public int MorePayload { get; }

        public InheritedEvent(Guid sourceId, string payload, int morePayload) : base(sourceId, payload)
        {
            MorePayload = morePayload;
        }
    }
}