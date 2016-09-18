using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS.Messaging.MessageRouting;
using Microsoft.Practices.Unity;

namespace Shop.Domain.DiscountAggregate
{
    public class DiscountAggregateCommandsHandler : AggregateCommandsHandler<OrderDiscount>
    {
        public DiscountAggregateCommandsHandler(IUnityContainer container)
        {
            Map<CreateOrderDiscountCommand>(
                c => c.DiscountId, 
                c => new OrderDiscount(c.DiscountId, c.OrderId, container.Resolve<IDiscountOffer>()));

            Map<CalculateDiscountCommand>(
                c => c.DiscountId, 
                (c,a) => a.Calculate(c.Item, container.Resolve<IPartnerService>()));
        }

        public static IAggregateCommandsHandlerDesriptor Descriptor { get; } = new DiscountAggregateCommandsHandler(null);
    }
}
