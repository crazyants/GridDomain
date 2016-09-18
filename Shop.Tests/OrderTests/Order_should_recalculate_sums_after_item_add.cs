using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using GridDomain.Tests.Framework;
using NMoneys;
using NUnit.Framework;
using Shop.Domain;
using Ploeh.AutoFixture;
using GridDomain.Tests;
using GridDomain.Tests.DependencyInjection.NamedDependencies;

namespace Shop.Tests
{

    [TestFixture]
    public class Order_should_recalculate_sums_after_item_add : AggregateTest<Order>
    {
        private readonly Item[] _items;
        private readonly Item _addedItem;
        private Money _initialTotal;

        public Order_should_recalculate_sums_after_item_add()
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
            Aggregate.AddItem(_addedItem);
        }

        [Then]
        public void TotalSum_should_be_increased_by_item_price()
        {
            Assert.AreEqual(_initialTotal + _addedItem.Price, Aggregate.TotalPrice);
        }
    }
}
