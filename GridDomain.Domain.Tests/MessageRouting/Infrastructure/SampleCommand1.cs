using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.MessageRouting.Infrastructure
{
    public sealed class SampleCommand1 : Command
    {
        public SampleCommand1(Guid aggregateId, string payload)
        {
            AggregateId = aggregateId;
            Payload = payload;
        }

        public Guid AggregateId { get; }
        public string Payload { get; }
    }
}