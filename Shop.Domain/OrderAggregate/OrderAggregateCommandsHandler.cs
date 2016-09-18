using System.Linq;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace Shop.Domain
{
    public class OrderAggregateCommandsHandler : AggregateCommandsHandler<Order>
    {
        public OrderAggregateCommandsHandler()
        {
            Map<CreateNewOrderCommand>(c => c.OrderId, c => new Order(c.OrderId, c.Number, c.Items.ToArray()));
            Map<AddNewItemToOrderCommand>(c => c.OrderId, (c, a) => a.AddItem(c.Item));
        }
    }
}