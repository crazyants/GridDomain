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
    public class Order_should_change_prices_after_discount_apply : AggregateTest<Order>
    {
        private readonly Item[] _items;
        private readonly Item _addedItem;
        private Money _initialTotal;
        private Money _discount;
        private Money _initialDiscount;

        public Order_should_change_prices_after_discount_apply()
        {
            _items = Generator.CreateMany<Item>().ToArray();
            _addedItem = Generator.Create<Item>();
        }

        protected override IEnumerable<DomainEvent> GivenEvents()
        {
            yield return new OrderCreated(Aggregate.Id, 123, _items);
            yield return new ItemAdded(Aggregate.Id, _addedItem);
        }

        protected override void When()
        {
            _initialTotal = Aggregate.TotalPrice;
            _initialDiscount = Aggregate.TotalDiscount;
            _discount = new Money(1);
            Aggregate.AddDiscount(_discount,null);
        }

        [Then]
        public void Total_sum_is_decreased_by_discount()
        {
            Assert.AreEqual(_initialTotal - _discount, Aggregate.DiscountedPrice);
        }

        [Then]
        public void Total_discount_is_increased_by_discount()
        {
            Assert.AreEqual(_initialDiscount + _discount, Aggregate.TotalDiscount);
        }
    }
}