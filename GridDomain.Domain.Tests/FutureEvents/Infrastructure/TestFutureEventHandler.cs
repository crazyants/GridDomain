using GridDomain.CQRS.Messaging;
using System;

namespace GridDomain.Tests.FutureEvents.Infrastructure
{
    class TestFutureEventHandler : IEventHandler<TestDomainEvent>
    {
        public void Handle(TestDomainEvent msg)
        {
            msg.HandledOn = DateTime.Now;
        }
    }
}