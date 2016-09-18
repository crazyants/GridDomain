using System;
using GridDomain.CQRS;

namespace Shop.Domain.DiscountAggregate
{
    public class CreateOrderDiscountCommand : Command
    {
        public Guid OrderId { get; set; }
        public Guid DiscountId { get; set; }

        public CreateOrderDiscountCommand(Guid orderId, Guid discountId)
        {
            OrderId = orderId;
            DiscountId = discountId;
        }
    }
}