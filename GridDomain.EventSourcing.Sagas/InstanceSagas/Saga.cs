using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Automatonymous;
using GridDomain.CQRS;
using GridDomain.Logging;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class Saga<TSagaData> : AutomatonymousStateMachine<TSagaData> where TSagaData : class, ISagaState
    {
        public readonly List<ICommand> CommandsToDispatch = new List<ICommand>();

        public void Dispatch(ICommand cmd)
        {
            CommandsToDispatch.Add(cmd);
        }
        public Saga()
        {
            InstanceState(d => d.CurrentStateName);
        }

        private readonly List<Type> _dispatchedCommands = new List<Type>(); 
        private readonly IDictionary<Type,Event> _messagesToEventsMap = new Dictionary<Type, Event>();
        public IReadOnlyCollection<Type> DispatchedCommands => _dispatchedCommands;
        protected void Command<TCommand>()
        {
            _dispatchedCommands.Add(typeof(TCommand));
        } 

        protected override void Event<TEventData>(Expression<Func<Event<TEventData>>> propertyExpression)
        {
            var machineEvent = propertyExpression.Compile().Invoke();
            _messagesToEventsMap[typeof(TEventData)] = machineEvent;

            base.Event(propertyExpression);
            
            DuringAny(When(machineEvent).Then(
                         ctx =>
                             OnEventReceived.Invoke(this,
                                 new EventReceivedData<TEventData, TSagaData>(ctx.Event, ctx.Data, ctx.Instance))));
        }

        public event EventHandler<StateChangedData<TSagaData>> OnStateEnter = delegate { };
        public event EventHandler<EventReceivedData<TSagaData>> OnEventReceived = delegate { };
        public event EventHandler<MessageReceivedData<TSagaData>> OnMessageReceived = delegate { };

        protected override void State(Expression<Func<State>> propertyExpression)
        {
            var state = propertyExpression.Compile().Invoke();
            WhenEnter(state, x => x.Then(ctx => 
                OnStateEnter.Invoke(this, new StateChangedData<TSagaData>(state, ctx.Instance))));

            base.State(propertyExpression);
        }

        public void Transit<T>(TSagaData progress, T message) where T:class
        {
            if (message == null)
                throw new NullMessageTransitException(progress);

            OnMessageReceived.Invoke(this, new MessageReceivedData<TSagaData>(message, progress));

            var machineEvent = GetMachineEvent(message);

            try
            {
                this.RaiseEvent(progress, machineEvent, message).Wait();
            }
            catch(Exception ex)
            {
                throw new SagaTransitionException(message, progress, ex);
            }
        }

        protected virtual Event<T> GetMachineEvent<T>(T message, TSagaData data = null) where T:class
        {
            Event ev;
            var type = message.GetType();
            if (!_messagesToEventsMap.TryGetValue(type, out ev))
                throw new UnbindedMessageReceivedException(message, type);
            return (Event<T>)ev;
        }

    }

    public class SagaTransitionException : Exception
    {
        public object TransitionMessage { get; }
        public ISagaState SagaData { get; }

        public SagaTransitionException(object message, ISagaState progress, Exception inner)
            :base("Saga transition raised an error",inner)
        {
            SagaData = progress;
            TransitionMessage = message;
        }
    }

    public class NullMessageTransitException : Exception
    {
        public readonly object SagaData;

        public NullMessageTransitException(object sagaData):base("Saga was transitioned by null message")
        {
            SagaData = sagaData;
        }
    }
}