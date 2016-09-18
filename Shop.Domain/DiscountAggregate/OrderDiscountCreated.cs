using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.DiscountAggregate
{
    public class OrderDiscountCreated : DomainEvent
    {
        public IDiscountOffer Offer { get; set; }
        public Guid OrderId { get; set; }
        public Guid DiscountId => SourceId; 
        public OrderDiscountCreated(Guid discountId, IDiscountOffer offer, Guid orderId):base(discountId)
        {
            Offer = offer;
            OrderId = orderId;
        }
    }
}