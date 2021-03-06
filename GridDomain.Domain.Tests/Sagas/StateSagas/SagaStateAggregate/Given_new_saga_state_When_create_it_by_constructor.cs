﻿using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.StateSagas;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.StateSagas.SagaStateAggregate
{
    [TestFixture]
    internal class Given_new_saga_state_When_create_it_by_constructor :
        DomainEventsProductionSpecification<SagaStateAggregate<TestState, TestTransition>>
    {
        private Guid _sagaId  = Guid.NewGuid();

        [Then]
        public void SagaCreatedEvent_should_be_raised()
        {
            VerifyRaisedEvents();
        }
        protected override SagaStateAggregate<TestState, TestTransition> CreateAggregate()
        {
            return new SagaStateAggregate<TestState, TestTransition>(_sagaId, TestState.Idle);
        }

        protected override IEnumerable<DomainEvent> ExpectedEvents()
        {
            yield return new SagaCreatedEvent<TestState>(TestState.Idle, _sagaId).CloneWithSaga(_sagaId);
        }
    }
}