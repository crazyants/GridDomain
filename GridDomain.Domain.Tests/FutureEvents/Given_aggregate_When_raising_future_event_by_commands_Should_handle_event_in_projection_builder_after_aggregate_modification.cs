using GridDomain.EventSourcing.Sagas.FutureEvents;
using GridDomain.Tests.FutureEvents.Infrastructure;
using NUnit.Framework;
using System;
using System.Linq;

namespace GridDomain.Tests.FutureEvents
{
    [TestFixture]

    public class Given_aggregate_When_raising_future_event_by_commands_Should_handle_event_in_projection_builder_after_aggregate_modification : FutureEventsTest_InMemory
    {
        private TestAggregate _aggregate;
        private TestDomainEvent _producedEvent;
        private RaiseEventInFutureCommand _testCommand;

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(500);

        [TestFixtureSetUp]

        public void When_raising_future_event()
        {
            _testCommand = new RaiseEventInFutureCommand(DateTime.Now.AddSeconds(1), Guid.NewGuid(), "test value");

            ExecuteAndWaitFor<FutureEventScheduledEvent>(_testCommand);
            
            _producedEvent = (TestDomainEvent)WaitFor<TestDomainEvent>().Recieved.First();

            _aggregate = LoadAggregate<TestAggregate>(_testCommand.AggregateId);
        }

        [Then]
        public void Future_event_applies_to_aggregate_before_applies_in_event_handler()
        {
            Assert.IsTrue(_aggregate.ProcessedTime.Ticks < _producedEvent.HandledOn.Ticks,
                $"Aggregate process time {_aggregate.ProcessedTime.Ticks} was greater than handler time {_producedEvent.HandledOn.Ticks}," +
                $"it means handler was working before aggregate ");
        }
    }
}