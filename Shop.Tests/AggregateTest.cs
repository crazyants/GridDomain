using System;
using System.Collections.Generic;
using System.Linq;
using CommonDomain;
using GridDomain.EventSourcing;
using GridDomain.Tests.AsyncAggregates;
using GridDomain.Tests.Framework;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Shop.Tests
{
    public abstract class AggregateTest<T> where T : IAggregate
    {
        [TestFixtureSetUp]
        public void InitAggregate()
        {
            Aggregate = GridDomain.EventSourcing.Sagas.FutureEvents.Aggregate.Empty<T>(Guid.NewGuid());
            Aggregate.ApplyEvents(GivenEvents().ToArray());
            When();
        }

        protected T Aggregate { get; private set; }
        protected abstract IEnumerable<DomainEvent> GivenEvents();
        protected virtual void When() { }

        protected readonly Fixture Generator = new Fixture(); 
    }
}