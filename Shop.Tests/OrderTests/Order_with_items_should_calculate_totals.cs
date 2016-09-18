using System.Collections.Generic;
using System.Linq;
using GridDomain.EventSourcing;
using GridDomain.Tests;
using NMoneys;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Shop.Domain;

namespace Shop.Tests
{
    [TestFixture]
    public class Order_with_items_should_calculate_totals : AggregateTest<Order>
    {
        private readonly Item[] _items;

        public Order_with_items_should_calculate_totals()
        {
            _items = Generator.CreateMany<Item>().ToArray();
        }
        protected override IEnumerable<DomainEvent> GivenEvents()
        {
            yield return new OrderCreated(Aggregate.Id, 123, _items);
        }

        [Then]
        public void Total_sum_should_be_equal_to_items_prices_sum()
        {
            var totalSum = _items.Aggregate(Money.Zero(), (a, i) => a + i.Price);
            Assert.AreEqual(totalSum, Aggregate.TotalPrice);
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
    }
}