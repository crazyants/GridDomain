using System;
using GridDomain.CQRS;

namespace Shop.Domain.DiscountAggregate
{
    public class CalculateDiscountCommand : Command
    {
        public Item Item { get; }
        public Guid DiscountId { get; }


        public CalculateDiscountCommand(Guid discountId, Item item)
        {
            Item = item;
            DiscountId = discountId;
        }
    }
}