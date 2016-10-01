using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Tests;
using NMoneys;
using NUnit.Framework;
using Shop.Domain.DiscountAggregate;

namespace Shop.Tests.DiscountTests
{
    [TestFixture]
    class Empty_OrderDiscount_should_have_zero_discounts : AggregateTest<OrderDiscount>
    {
        private readonly TenPercentDiscountOffer _offer;

        public Empty_OrderDiscount_should_have_zero_discounts()
        {
            _offer = new TenPercentDiscountOffer();
        }

        protected override IEnumerable<DomainEvent> GivenEvents()
        {
            yield return new OrderDiscountCreated(Aggregate.Id, _offer, Guid.NewGuid());
        }

        [Then]
        public void Base_discount_is_zero()
        {
            Assert.AreEqual(Money.Zero(), Aggregate.BaseDiscount);
        }

        [Then]
        public void Partner_discount_is_zero()
        {
            Assert.AreEqual(Money.Zero(), Aggregate.PartnerDiscount);
        }
    }
}
