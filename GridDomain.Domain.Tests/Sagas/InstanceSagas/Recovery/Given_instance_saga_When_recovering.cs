﻿using System;
using System.Collections.Generic;
using System.Linq;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas.Recovery
{
    [TestFixture]
    class Given_instance_saga_When_recovering
    {
        private SagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData> _sagaInstance;
        private IReadOnlyCollection<object> _dispatchedCommands;
        private SagaDataAggregate<SoftwareProgrammingSagaData> _data;
        private CoffeMakeFailedEvent _coffeMakeFailedEvent;

        [OneTimeSetUp]
        public void Given_instance_saga_When_recovering_from_creation()
        {
            var aggregateFactory = new AggregateFactory();
            var sagaId = Guid.NewGuid();

            _data = aggregateFactory.Build<SagaDataAggregate<SoftwareProgrammingSagaData>>(sagaId);
            var saga = new SoftwareProgrammingSaga();
            var initialState = new SoftwareProgrammingSagaData(saga.MakingCoffee.Name);

            var eventsToReplay = new DomainEvent[]
            {
                new SagaCreatedEvent<SoftwareProgrammingSagaData>(initialState, sagaId)
            };

            _data.ApplyEvents(eventsToReplay);

            _sagaInstance = SagaInstance.New(saga, _data);

            //Try to transit saga by message, available only in desired state
            _coffeMakeFailedEvent = new CoffeMakeFailedEvent(Guid.NewGuid(),Guid.NewGuid());
            _sagaInstance.Transit(_coffeMakeFailedEvent);
            _dispatchedCommands = _sagaInstance.CommandsToDispatch;
        }

        [Then]
        public void Saga_produce_commands_only_one_command()
        {
            Assert.AreEqual(1, _dispatchedCommands.Count);
        }

        [Then]
        public void Produced_command_has_right_person_id()
        {
           var sleepCommand = _dispatchedCommands.OfType<GoSleepCommand>().First();
            Assert.AreEqual(_coffeMakeFailedEvent.ForPersonId, sleepCommand.PersonId);
        }

        [Then]
        public void Produced_command_has_right_sofa_id()
        {
            var sleepCommand = _dispatchedCommands.OfType<GoSleepCommand>().First();
            Assert.AreEqual(_data.Data.SofaId, sleepCommand.SofaId);
        }

        [Then]
        [Ignore("Need to think how to set sagaId from sagaInstance or saga efficiently. For now saga actor with set saga id")]
        public void Produced_command_has_empty_saga_id()
        {
            var sleepCommand = _dispatchedCommands.OfType<GoSleepCommand>().First();
            Assert.AreEqual(_data.Id, sleepCommand.SagaId);
        }

        [Then]
        public void Saga_produce_command_from_given_state()
        {
            Assert.IsInstanceOf<GoSleepCommand>(_dispatchedCommands.FirstOrDefault());
        }
    }
}
