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
    public class Order_with_items_cannot_apply_discount_to_full_price : AggregateTest<Order>
    {
        private readonly Item[] _items;

        public Order_with_items_cannot_apply_discount_to_full_price()
        {
            _items = Generator.CreateMany<Item>().ToArray();
        }
        protected override IEnumerable<DomainEvent> GivenEvents()
        {
            yield return new OrderCreated(Aggregate.Id, 123, _items);
        }

        protected override void When()
        {
            Assert.Throws<DiscountsMoreThenTotalPriceException>( () => 
                Aggregate.AddDiscount(new Money(Aggregate.TotalPrice), null));
        }

        [Then]
        public void Exception_should_be_thrown_when_invalid_discount_is_applied()
        {
        }

    }
}