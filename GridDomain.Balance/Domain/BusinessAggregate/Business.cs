using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonDomain.Core;
using GridDomain.EventSourcing;

namespace GridDomain.Balance.Domain.BusinessAggregate
{
    class Business : IDomainAggregate
    {
        Guid BalanceId;
        Guid SubscriptionId;
        public string Name;

        public Guid Id { get; private set; }
        public IAggregateState Events { get; }

        public Business(Guid id, string name, Guid subscriptionId, Guid balanceId) : this(id)
        {
            Events.Produce(new BusinessCreatedEvent(id)
            {
                Names = name,
                SubscriptionId = subscriptionId,
                BalanceId = balanceId
            });
        }

        public Business(Guid id)
        {
            var events = new EventStream(id);
            events.OnApply<BusinessCreatedEvent>(e =>
            {
                Id = e.SourceId;
                BalanceId = e.BalanceId;
                SubscriptionId = e.SubscriptionId;
            });
            Events = events;
        }

    }
}
