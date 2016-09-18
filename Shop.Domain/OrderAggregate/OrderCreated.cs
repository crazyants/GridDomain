using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;

namespace Shop.Domain
{
    public sealed class OrderCreated : DomainEvent
    {
        public int Number { get; }
        public IReadOnlyCollection<Item> Items { get;}
        public Guid OrderId => SourceId;

        public OrderCreated(Guid orderId, int number, params Item[] items) : base(orderId)
        {
            Number = number;
            Items = items;
        }
    }
}