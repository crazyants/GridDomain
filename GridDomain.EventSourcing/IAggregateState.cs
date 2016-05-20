using System;
using System.Collections.Generic;

namespace GridDomain.EventSourcing
{
    public interface IAggregateState
    {
        void Produce<T>(T domainEvent) where T : DomainEvent;
        IReadOnlyCollection<DomainEvent> UncommitedEvents();
        void ClearUncommited();
    }
}