using System;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Tests.MessageRouting.Infrastructure
{
    public class TestAggregatesCommandHandler : AggregateCommandsHandler<TestAggregate>,
        IAggregateCommandsHandlerDesriptor

    {
        public static readonly IAggregateCommandsHandlerDesriptor Descriptor = new TestAggregatesCommandHandler();

        public TestAggregatesCommandHandler() : base(null)
        {
            Map<SampleCommand1>(c => c.AggregateId,
                (c, a) => a.ApplyCommand1(c.Payload));

            Map<SampleCommand2>(c => c.AggregateId,
                (c, a) => a.ApplyCommand2(c.Payload, c.Payload2));
        }

        public Type AggregateType => typeof(TestAggregate);
    }
}