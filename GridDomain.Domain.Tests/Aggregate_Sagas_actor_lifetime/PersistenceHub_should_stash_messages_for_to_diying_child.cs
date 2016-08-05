using Akka.Actor;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{

    class PersistenceHub_should_stash_messages_for_to_diying_child : PersistentHub_children_lifetime_test
    {
        private LongOperationCommand _longOperationCommand;

        public PersistenceHub_should_stash_messages_for_to_diying_child() : base(PersistentHubTestsStatus.PersistenceCase.Aggregate)
        {
        }

        [SetUp]
        public void When_child_should_be_terminated_but_not_die_until_revive_command()
        {
            When_hub_creates_a_child();
            And_child_is_busy_long_time();
            And_it_is_not_active_until_lifetime_period_is_expired();
        }

        [Then]
        public void Child_should_finish_his_work_first()
        {
            And_command_for_child_is_sent();
            var e = WaitFor<SampleAggregateChangedEvent>();
            var sampleAggregateChangedEvent = ((SampleAggregateChangedEvent)e.Message);

            Assert.AreEqual(Infrastructure.ChildId,sampleAggregateChangedEvent.SourceId);
            Assert.AreEqual(_longOperationCommand.Parameter.ToString(),sampleAggregateChangedEvent.Value);
        }

        private void And_child_is_busy_long_time()
        {
            _longOperationCommand = new LongOperationCommand(10000,Infrastructure.ChildId);
            Infrastructure.Hub.Tell(_longOperationCommand);
        }
    }
}