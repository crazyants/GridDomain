using Automatonymous;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.MessageRouting.Infrastructure
{
    public class SampleSaga : Saga<SampleSagaData>
    {
        public static readonly ISagaDescriptor Descriptor
            = SagaExtensions.CreateDescriptor<SampleSaga,
                                              SampleSagaData,
                                              BaseEvent>();

        public SampleSaga()
        {
            Event(() => Created);
            State(() => Started);
            State(() => Ended);
            Command<SampleCommand2>();

            During(Started,When(Created).Then(context =>
            {
                Dispatch(new SampleCommand2(context.Data.SourceId, "another test", 2));
            }).TransitionTo(Ended));
        }

        public State Ended { get; set; }

        public State Started { get; set; }

        public Event<BaseEvent> Created { get; private set; }
    }
}