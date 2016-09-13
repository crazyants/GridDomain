using System;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Framework;
using GridDomain.Tests.MessageRouting.Infrastructure;
using NUnit.Framework;

namespace GridDomain.Tests.MessageRouting
{
    public abstract class EventInheritanceTestsBase : ExtendedNodeCommandTest
    {
        protected EventInheritanceTestsBase() : base(true)
        {
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(5);
        protected override IContainerConfiguration CreateConfiguration()
        {
            return new CustomContainerConfiguration(
                c => c.RegisterAggregate<TestAggregate, TestAggregatesCommandHandler>(),
                c => c.RegisterSaga<SampleSaga, SampleSagaData, SampleSagaFactory, BaseEvent>(SampleSaga.Descriptor));
        }        

        protected override IMessageRouteMap CreateMap()
        {
            return new TestRouteMap();
        }
    }

    [TestFixture]
    public class
        GivenAggregateSendsInheretedEventsAndInstanceSagaAffectedByBaseEvent_WhenDerivedEventRaised_ThenSagaShouldIgnoreIt :
            EventInheritanceTestsBase
    {
        [Test]
        public void Test()
        {
            var aggregateId = Guid.NewGuid();
            var task = GridNode.Execute(new SampleCommand1(aggregateId, "test"),
                new[] {new ExpectedMessage(typeof(InheritedEvent), 1, nameof(InheritedEvent.SourceId), aggregateId)});
            Assert.DoesNotThrow(() =>
            {
                var result = task.Result;
            });
        }
    }
}