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

    //descriptor should be created separately due to unknown command type \ success message type
    public class ScheduledCommandProcessSaga: Saga<ScheduledCommandProcessSagaData>
    {
        //public static ISagaDescriptor Descriptor =
            //SagaExtensions.CreateDescriptor<ScheduledCommandProcessSaga,
                                            //ScheduledCommandProcessSagaData,
                                            ////ScheduledCommandProcessingStarted>();
 
        private readonly ISoloLogger _log = LogManager.GetLogger();

        public ScheduledCommandProcessSaga()
        {
            Event(()=>ProcessSuccess);
            Event(()=>ProcessFailure);
            Event(()=>StartProcess);

            State(() => Created);
            State(() => MessageSent);
            State(() => ProcessingFailed);
            State(() => ProcessingSucceded);

            During(Created,
                When(StartProcess).Then(context =>
                {
                    var key = context.Instance.Key;
                    Dispatch(new CompleteJob(key.Name, key.Group));
                    Dispatch(context.Instance.Command);
                })
                .TransitionTo(MessageSent));

            During(MessageSent,
                When(ProcessSuccess, ctx => ctx.Data.GetType() == ctx.Instance.SuccessEventType)
                    .Then(context =>_log.Info("Scheduled command {Command} successfully processed, received event: {Data}", context.Instance.Command, context.Data))
                    .TransitionTo(ProcessingSucceded),
                When(ProcessFailure)
                    .Then(context => _log.Error(context.Data.Exception, "Scheduled command processing failure, command: {Data}", context.Data))
                    .TransitionTo(ProcessingFailed));
        }

        protected override Event GetMachineEvent(object message, ScheduledCommandProcessSagaData data)
        {
            if (message is ICommandFault)
                return ProcessFailure;

            if (message is ScheduledCommandProcessingStarted)
                return StartProcess;

            if(message.GetType() == data.SuccessEventType)
                return ProcessSuccess;

            return base.GetMachineEvent(message, data);
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