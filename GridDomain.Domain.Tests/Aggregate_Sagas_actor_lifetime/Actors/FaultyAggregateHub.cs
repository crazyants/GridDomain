using System;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.Actors;
using GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Infrastructure;
using GridDomain.Tests.SampleDomain;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Actors
{
    class FaultyAggregateHub : AggregateHubActor<SampleAggregate>
    {
        public FaultyAggregateHub(ICommandAggregateLocator<SampleAggregate> locator) : base(locator, new TestPersistentChildsRecycleConfiguration())
        {
        }

        protected override Type GetChildActorType(object message)
        {
            return typeof(FaultyPersistenceActor);
        }
    }
}