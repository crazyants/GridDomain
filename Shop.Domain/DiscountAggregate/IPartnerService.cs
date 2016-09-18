using NMoneys;

namespace Shop.Domain.DiscountAggregate
{
    public interface IPartnerService
    {
        Money Calculate(Item item);
    }
}