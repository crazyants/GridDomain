using System;
using GridDomain.EventSourcing;

namespace BusinessNews.Domain.BusinessAggregate
{
    public class CustomerCreatedEvent : DomainEvent
    {
        public Guid AccountId;
        public string Name;
        public Guid SubscriptionId;

        public CustomerCreatedEvent(Guid sourceId, DateTime? createdTime = null) : base(sourceId, createdTime)
        {
        }
    }
}