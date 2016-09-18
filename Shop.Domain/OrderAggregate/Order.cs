using System;
using System.Collections.Generic;
using System.Linq;
using CommonDomain;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using NMoneys;

namespace Shop.Domain
{
    public sealed class Order : Aggregate
    {
        public int Number { get; private set; }
        public IReadOnlyCollection<Item> Items => _items;
        public Money TotalPrice { get; private set; }
        public Money TotalDiscount { get; private set; }
        public Money DiscountedPrice { get; private set; }

        private List<Item> _items = new List<Item>();
        private Order(Guid id) : base(id) { }

        public Order(Guid id, int number, params Item[] items) : base(id)
        {
            RaiseEvent(new OrderCreated(id, number, items));
        }

        public void AddItem(Item itemToAdd)
        {
            RaiseEvent(new ItemAdded(Id, itemToAdd));
        }

        public void AddDiscount(Money discount, Item item)
        {
            if (DiscountedPrice <= discount)
                throw new DiscountsMoreThenTotalPriceException();

           RaiseEvent(new DiscountAdded(Id,discount, item));
        }
        private void Apply(OrderCreated e)
        {
            Number = e.Number;
            Id = e.OrderId;
            _items = e.Items.ToList();
            RefreshTotals();
        }

        private void RefreshTotals()
        {
            TotalPrice = _items.Aggregate(Money.Zero(),(a,i) => a+i.Price);
            RefreshDiscounted();
        }

        private void Apply(ItemAdded e)
        {
            _items.Add(e.Item);
            RefreshTotals();
        }

        private void Apply(DiscountAdded e)
        {
            TotalDiscount += e.Discount;
            RefreshDiscounted();
        }

        private void RefreshDiscounted()
        {
            DiscountedPrice = TotalPrice - TotalDiscount;
        }
    }
}