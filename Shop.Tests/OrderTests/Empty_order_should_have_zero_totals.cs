using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.Tests;
using NMoneys;
using NUnit.Framework;
using Shop.Domain;

namespace Shop.Tests
{
    [TestFixture]
    public class Empty_order_should_have_zero_totals : AggregateTest<Order>
    {
        protected override IEnumerable<DomainEvent> GivenEvents()
        {
            yield return new OrderCreated(Aggregate.Id, 123);
        }

        [Then]
        public void Order_total_price_should_be_zero()
        {
            Assert.AreEqual(Money.Zero(), Aggregate.TotalPrice);
        }

        [Then]
        public void Order_total_discount_price_should_be_zero()
        {
            Assert.AreEqual(Money.Zero(), Aggregate.DiscountedPrice);
        }

        [Then]
        public void Order_discounted_price_should_be_zero()
        {
            Assert.AreEqual(Money.Zero(), Aggregate.TotalDiscount);
        }

        [Then]
        public void Order_should_not_have_items()
        {
            CollectionAssert.IsEmpty(Aggregate.Items);
        }

    }
}