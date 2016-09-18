using System;
using System.Collections.Generic;
using System.Linq;
using CommonDomain;
using GridDomain.EventSourcing;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace GridDomain.Tests.Framework
{
    public abstract class AgregateTest<T> where T : IAggregate
    {
        [TestFixtureSetUp]
        public void InitAggregate()
        {
            Aggregate = EventSourcing.Sagas.FutureEvents.Aggregate.Empty<T>(Guid.NewGuid());
            Aggregate.ApplyEvents(GivenEvents().ToArray());
        }
        protected T Aggregate { get; private set; }
        protected abstract IEnumerable<DomainEvent> GivenEvents();
        protected virtual void When() { }

        protected readonly Fixture Generator = new Fixture(); 
    }
}