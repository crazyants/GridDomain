using System;
using GridDomain.EventSourcing;
using NMoneys;

namespace Shop.Domain
{
    public class DiscountAdded : DomainEvent
    {
        public Money Discount { get; }
        public Item Item { get; }
        public Guid OrderId => SourceId;

        public DiscountAdded(Guid id, Money discount, Item item) : base(id)
        {
            Discount = discount;
            Item = item;
        }
    }
}