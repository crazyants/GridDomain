using GridDomain.Common;
using GridDomain.Node.Configuration.Composition;
using Microsoft.Practices.Unity;
using Shop.Domain.DiscountAggregate;

namespace Shop.Domain
{
    class ShopContainerConfiguration : IContainerConfiguration
    {
        public void Register(IUnityContainer container)
        {
            container.RegisterAggregate<Order, OrderAggregateCommandsHandler>();
            container.RegisterAggregate<OrderDiscount, DiscountAggregateCommandsHandler>();
            container.RegisterType<IDiscountOffer, TenPercentDiscountOffer>();
            container.RegisterType<IPartnerService, PartnerService>();
        }
    }
}