using System;
using GridDomain.EventSourcing.Sagas.FutureEvents;

namespace GridDomain.Tests.MessageRouting.Infrastructure
{
    public class TestAggregate : Aggregate
    {
        public string TextData { get; private set; }
        public int NumericData { get; private set; }

        public TestAggregate(Guid id) : base(id)
        {
        }

        public void ApplyCommand1(string commandPayload)
        {
            RaiseEvent(new BaseEvent(Id, commandPayload));
        }

        public void ApplyCommand2(string textPayload, int numericPayload)
        {
            RaiseEvent(new InheritedEvent(Id, textPayload, numericPayload));
        }

        private void Apply(BaseEvent @event)
        {
            TextData = @event.Payload;
        }

        private void Apply(InheritedEvent @event)
        {
            TextData = @event.Payload;
            NumericData = @event.MorePayload;
        }
    }
}