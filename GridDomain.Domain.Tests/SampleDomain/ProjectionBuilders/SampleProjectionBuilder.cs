using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.SampleDomain.Events;

namespace GridDomain.Tests.SampleDomain.ProjectionBuilders
{
    public class SampleProjectionBuilder : IHandler<SampleAggregateChangedEvent>
    {
        private readonly IPublisher _publisher;

        public SampleProjectionBuilder(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Handle(SampleAggregateChangedEvent msg)
        {
            _publisher.Publish(new AggregateChangedEventNotification() { AggregateId = msg.SourceId} );
        }
    }
}