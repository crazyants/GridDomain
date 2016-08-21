using System;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Persistence;
using GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Actors;
using GridDomain.Tests.SampleDomain.Commands;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Infrastructure
{
    class FaultyPersistenceActor : PersistentActor
    {

        public FaultyPersistenceActor()
        {
        }

        protected override bool ReceiveRecover(object message)
        {
           throw new RecoverFailedException();
        }

        protected override bool ReceiveCommand(object message)
        {
            Persist(message,o =>{});
            return true;
        }
      
        public override string PersistenceId { get; }
    }

    internal class RecoverFailedException
        : Exception
    {
    }

    class AggregatePersistedHub_fault_on_recover : IPersistentActorTestsInfrastructure
    {

        public AggregatePersistedHub_fault_on_recover(ActorSystem system)
        {
            ChildId = Guid.NewGuid();
            HubProps = system.DI().Props<FaultyAggregateHub>();
            ChildCreateMessage = new object();
            ChildActivateMessage = new object();
        }

        public Props HubProps { get; }
        public object ChildCreateMessage { get; }
        public object ChildActivateMessage { get; }
        public Guid ChildId { get; }
    }

    class AggregatePersistedHub_fault_on_persist : IPersistentActorTestsInfrastructure
    {
        public Props HubProps { get; }
        public object ChildCreateMessage { get; }
        public object ChildActivateMessage { get; }
        public Guid ChildId { get; }
    }
    class AggregatePersistedHub_child_already_exist : IPersistentActorTestsInfrastructure
    {
        public Props HubProps { get; }
        public object ChildCreateMessage { get; }
        public object ChildActivateMessage { get; }
        public Guid ChildId { get; }
    }

    class AggregatePersistedHub_Infrastructure : IPersistentActorTestsInfrastructure
    {
        public Props HubProps { get; }
        public object ChildCreateMessage { get; }
        public object ChildActivateMessage { get; }
        public Guid ChildId { get; }

        public AggregatePersistedHub_Infrastructure(ActorSystem system)
        {
            ChildId = Guid.NewGuid();
            ChildCreateMessage = new CreateSampleAggregateCommand(42, ChildId, ChildId);
            ChildActivateMessage = new ChangeSampleAggregateCommand(100, ChildId);
            HubProps = system.DI().Props<TestAggregateHub>();
        }
    }
}