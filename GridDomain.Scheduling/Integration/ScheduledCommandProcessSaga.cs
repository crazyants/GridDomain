using System;
using System.Linq;
using Automatonymous;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.EventSourcing.Sagas.StateSagas;
using GridDomain.Logging;
using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Scheduling.Integration
{
    public class ScheduledCommandProcessSagaData : ISagaState
    {
        public string CurrentStateName { get; set; }
        public Command Command { get;  set; }
        public ScheduleKey Key { get;  set; }
        public Type SuccessEventType { get;  set; }
    }

    public class ScheduledCommandProcessSaga: Saga<ScheduledCommandProcessSagaData>
    {
        public static ISagaDescriptor Descriptor =
            SagaExtensions.CreateDescriptor<ScheduledCommandProcessSaga,
                                            ScheduledCommandProcessSagaData,
                                            ScheduledCommandProcessingStarted>();

        private readonly ISoloLogger _log = LogManager.GetLogger();

        public ScheduledCommandProcessSaga()
        {
            
        }

        protected override Event<TMessage> GetMachineEvent<TMessage>(TMessage message)
        {
            if (message is ICommandFault)
                return ProcessFailure as Event<TMessage>;

            if (message is ScheduledCommandProcessingStarted)
                return StartProcess as Event<TMessage>;

            return ProcessSuccess as Event<TMessage>;
        }

        public Event<ScheduledCommandProcessingStarted> StartProcess { get; private set; }
        public Event<ICommandFault> ProcessFailure { get; private set; }
        public Event<object> ProcessSuccess { get; private set; }

        public State Created { get; private set; }
        public State MessageSent { get; private set; }
        public State ProcessingFailed { get; private set; }
        public State ProcessingSucceded { get; private set; }
    }
}