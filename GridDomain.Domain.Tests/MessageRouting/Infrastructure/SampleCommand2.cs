using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.MessageRouting.Infrastructure
{
    public sealed class SampleCommand2 : Command
    {

        public SampleCommand2(Guid aggregateId, string payload, int payload2)
        {
            AggregateId = aggregateId;
            Payload = payload;
            Payload2 = payload2;
        }

        public Guid AggregateId { get; }
        public string Payload { get; }
        public int Payload2 { get; }
    }
}