using NMoneys;

namespace Shop.Domain.DiscountAggregate
{
    public interface IDiscountOffer
    {
        Money Calculate(Item item);
    }
}