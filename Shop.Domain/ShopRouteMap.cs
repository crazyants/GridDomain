using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using Shop.Domain.DiscountAggregate;

namespace Shop.Domain
{
    class ShopRouteMap : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
            router.RegisterAggregate<Order,OrderAggregateCommandsHandler>();
            router.RegisterAggregate(DiscountAggregateCommandsHandler.Descriptor);
        }
    }
}
