using NMoneys;

namespace Shop.Domain.DiscountAggregate
{
    public class TenPercentDiscountOffer : IDiscountOffer
    {
        public Money Calculate(Item item)
        {
            return new Money(item.Price.Amount * 0.1m, item.Price.CurrencyCode);
        }
    }
}