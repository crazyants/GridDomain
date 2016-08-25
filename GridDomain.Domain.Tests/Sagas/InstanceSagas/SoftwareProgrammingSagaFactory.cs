using System;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;

namespace GridDomain.Tests.Sagas.InstanceSagas
{


    class SoftwareProgrammingSagaFactory:
             ISagaFactory<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>, SagaDataAggregate<SoftwareProgrammingSagaData>>,
             ISagaFactory<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>, GotTiredEvent>,
             ISagaFactory<ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData>, Guid> 
    {
        private readonly AggregateFactory _emptyAggregateFactory = new AggregateFactory();

        public ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData> Create(SagaDataAggregate<SoftwareProgrammingSagaData> message)
        {
           return SagaInstance.New(new SoftwareProgrammingSaga(), message);
        }

        public ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData> Create(GotTiredEvent message)
        {
            var saga = new SoftwareProgrammingSaga();
            var data = new SagaDataAggregate<SoftwareProgrammingSagaData>(message.SagaId,
                                                                          new SoftwareProgrammingSagaData(saga.Coding.Name)
                                                                          );
            return SagaInstance.New(saga, data);
        }

        public ISagaInstance<SoftwareProgrammingSaga, SoftwareProgrammingSagaData> Create(Guid id)
        {
            var saga = new SoftwareProgrammingSaga();
            var data = _emptyAggregateFactory.Build<SagaDataAggregate<SoftwareProgrammingSagaData>>(id);
            return SagaInstance.New(saga,data);
        }

    }
}