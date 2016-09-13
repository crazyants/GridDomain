using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.MessageRouting.Infrastructure
{
    class SampleSagaFactory : ISagaFactory<ISagaInstance<SampleSaga, SampleSagaData>, SagaDataAggregate<SampleSagaData>>,
        ISagaFactory<ISagaInstance<SampleSaga, SampleSagaData>, BaseEvent>
    {
        public ISagaInstance<SampleSaga, SampleSagaData> Create(SagaDataAggregate<SampleSagaData> message)
        {
            return SagaInstance.New(new SampleSaga(), message);
        }

        public ISagaInstance<SampleSaga, SampleSagaData> Create(BaseEvent message)
        {
            var saga = new SampleSaga();
            var data = new SagaDataAggregate<SampleSagaData>(message.SagaId, new SampleSagaData(saga.Started.Name));

            return SagaInstance.New(saga, data);
        }
    }
}
