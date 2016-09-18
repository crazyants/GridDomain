using NMoneys;

namespace Shop.Domain.DiscountAggregate
{
    public interface IPartnerService
    {
        Money Calculate(Item item);
    }

    public class PartnerService : IPartnerService
    {
        public Money Calculate(Item item)
        {
            return item.Name.Contains("a") ? 
                    new Money(item.Price.Amount*0.15m, item.Price.CurrencyCode) 
                    : Money.Zero();
        }
    }
}