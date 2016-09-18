using System;
using GridDomain.EventSourcing;

namespace Shop.Domain
{
    public sealed class ItemAdded : DomainEvent
    {
        public Item Item { get; }

        public ItemAdded(Guid orderId, Item item) : base(orderId)
        {
            Item = item;
        }
    }
}