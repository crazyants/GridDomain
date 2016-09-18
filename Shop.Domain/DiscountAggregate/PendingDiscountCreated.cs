using System;
using GridDomain.EventSourcing;
using NMoneys;

namespace Shop.Domain.DiscountAggregate
{
    public class PendingDiscountCreated : DomainEvent
    {
        public Money BaseDiscount { get; }
        public Money PartnerDiscount { get; }
        public Guid PendingDiscountId => SourceId; 
        public PendingDiscountCreated(Guid id, Money baseDiscount, Money partnerDiscount):base(id)
        {
            BaseDiscount = baseDiscount;
            PartnerDiscount = partnerDiscount;
        }
    }
}