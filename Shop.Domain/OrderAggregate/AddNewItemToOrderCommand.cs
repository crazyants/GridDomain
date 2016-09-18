using System;
using GridDomain.CQRS;

namespace Shop.Domain
{
    public class AddNewItemToOrderCommand : Command
    {
        public Guid OrderId { get; set; }
        public Item Item { get; set; }

        public AddNewItemToOrderCommand(Guid orderId, Item item)
        {
            OrderId = orderId;
            Item = item;
        }
    }
}