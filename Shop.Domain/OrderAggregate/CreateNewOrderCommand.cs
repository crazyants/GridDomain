using System;
using System.Collections.Generic;
using GridDomain.CQRS;

namespace Shop.Domain
{
    public class CreateNewOrderCommand : Command
    {
        public IReadOnlyCollection<Item> Items { get; }
        public int Number { get; }
        public Guid OrderId { get; }
        public CreateNewOrderCommand(Guid orderId, int number, params Item[] items)
        {
            Items = items;
            Number = number;
            OrderId = orderId;
        }
    }
}