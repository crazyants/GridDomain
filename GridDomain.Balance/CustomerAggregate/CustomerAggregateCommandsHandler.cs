using BusinessNews.Domain.OfferAggregate;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace BusinessNews.Domain.BusinessAggregate
{
    public class CustomerAggregateCommandsHandler : AggregateCommandsHandler<Customer>
    {
        public CustomerAggregateCommandsHandler() : base(null)
        {
            Map<OrderSubscriptionCommand>(command => command.BusinessId,
                                         (command, aggregate) => aggregate.OrderSubscription(command.SubscriptionId, command.OfferId));

            Map<CompleteBusinessSubscriptionOrderCommand>(c => c.BusinessId,
                                                         (cmd, agr) => agr.PurchaseSubscription(cmd.SubscriptionId));

            Map<RegisterNewCustomerCommand>(c => c.BusinessId,
                                           cmd => new Customer(cmd.BusinessId, cmd.Name, FreeSubscription.ID, cmd.AccountId));
        }
    }
}