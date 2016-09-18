using System;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using NMoneys;

namespace Shop.Domain.DiscountAggregate
{
    public class OrderDiscount : Aggregate
    {
        //Should be serializable to be stored in snapshots
        public IDiscountOffer Offer { get; private set; }
        public Guid OrderId { get; private set; }
        public Money BaseDiscount { get; private set; }
        public Money PartnerDiscount { get; private set; }

        private OrderDiscount(Guid id) : base(id) { }
        public OrderDiscount(Guid id, Guid orderId, IDiscountOffer offer) : base(id)
        {
            RaiseEvent(new OrderDiscountCreated(id, offer, orderId));
        }

        public void Calculate(Item item, IPartnerService service)
        {
            var partnerDiscount = service.Calculate(item);
            var baseDiscount = Offer.Calculate(item);

            if (baseDiscount + partnerDiscount == Money.Zero())
                throw new DiscountNotEvailableException();

            RaiseEvent(new PendingDiscountCreated(Guid.NewGuid(),baseDiscount, partnerDiscount));
        }

     

        private void Apply(PendingDiscountCreated e)
        {
            BaseDiscount += e.BaseDiscount;
            PartnerDiscount += e.PartnerDiscount;
        }

        private void Apply(OrderDiscountCreated e)
        {
            Offer = e.Offer;
            OrderId = e.OrderId;
        }
    }
}