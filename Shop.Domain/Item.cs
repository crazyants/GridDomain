using System;
using System.Text;
using System.Threading.Tasks;
using NMoneys;

namespace Shop.Domain
{
    public class Item
    {
        public string Name { get; private set; }
        public Guid Id { get; private set; }
        public int Quantity { get; private set; }

        public Money Price { get; private set; }

        public Item(Guid id, string name, Money price, int quantity = 1)
        {
            Id = id;
            Name = name;
            Price = price;
            Quantity = quantity;
        }
    }
}

