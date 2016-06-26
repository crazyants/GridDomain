using System;
using System.Collections;

namespace GridDomain.Aggregates
{
    public interface IAggregate
    {
        Guid Id { get; }
        int Version { get; }

        void ApplyEvent(object @event);
        ICollection GetUncommittedEvents();
        void ClearUncommittedEvents();
    }
}