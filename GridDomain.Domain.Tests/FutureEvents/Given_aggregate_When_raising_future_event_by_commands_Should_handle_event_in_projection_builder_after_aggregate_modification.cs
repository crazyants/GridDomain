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
        private DateTime _scheduledTime;
        private TestDomainEvent _producedEvent;
        private RaiseEventInFutureCommand _testCommand;
        private FutureEventScheduledEvent _futureEventEnvelop;

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(5);

        [TestFixtureSetUp]

        public void When_raising_future_event()
        {
            _scheduledTime = DateTime.Now.AddSeconds(1);
            _testCommand = new RaiseEventInFutureCommand(_scheduledTime, Guid.NewGuid(), "test value");

            _futureEventEnvelop = (FutureEventScheduledEvent)ExecuteAndWaitFor<FutureEventScheduledEvent>(_testCommand).Recieved.First();
            _producedEvent = (TestDomainEvent)WaitFor<TestDomainEvent>().Recieved.First();

            _aggregate = LoadAggregate<TestAggregate>(_testCommand.AggregateId);
        }

        [Then]
        public void Future_event_applies_to_aggregate_before_applies_in_event_handler()
        {
            Assert.IsTrue(_aggregate.ProcessedTime < _producedEvent.HandledOn);
        }
    }
}