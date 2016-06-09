using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommonDomain;
using GridDomain.CQRS;
using Stateless;

namespace GridDomain.EventSourcing.Sagas
{

    public static class SagaInfo<TSaga>
    {
        public static IReadOnlyCollection<Type> KnownMessages()
        {
            var allInterfaces = typeof(TSaga).GetInterfaces();

            var handlerInterfaces =
                allInterfaces.Where(i => i.IsGenericType &&
                                         i.GetGenericTypeDefinition() == typeof(IHandler<>))
                             .ToArray();

            return handlerInterfaces.SelectMany(s => s.GetGenericArguments()).ToArray();
        }
    }

    public class SagaDescriptor
    {
        
    }
public class StateSaga<TSagaStates, TSagaTriggers, TStateData, TStartMessage> : 
                                                                 IDomainSaga,
                                                                 ISagaDescriptor
                                                                 where TSagaTriggers : struct
                                                                 where TSagaStates : struct
                                                                 where TStateData : SagaStateAggregate<TSagaStates, TSagaTriggers>
    {
        private readonly IDictionary<Type, StateMachine<TSagaStates, TSagaTriggers>.TriggerWithParameters>
            _eventsToTriggersMapping
                = new Dictionary<Type, StateMachine<TSagaStates, TSagaTriggers>.TriggerWithParameters>();

        public IReadOnlyCollection<Type> AcceptMessages => _eventsToTriggersMapping.Keys.ToArray();

        public Type StartMessage { get; } = typeof (TStartMessage);
        public Type StateType { get; } = typeof (TStateData);
        public Type SagaType => this.GetType();

        public readonly StateMachine<TSagaStates, TSagaTriggers> Machine;

        protected readonly TStateData StateData;

        protected StateSaga(TStateData stateData)
        {
            StateData = stateData;

            //to include start message into list of accept messages
            _eventsToTriggersMapping[typeof (TStartMessage)] = null; 
            Machine = new StateMachine<TSagaStates, TSagaTriggers>(StateData.MachineState);
            Machine.OnTransitioned(t => StateData.StateChanged(t.Trigger, t.Destination));
            _transitMethod = GetType().GetMethod(nameof(TransitState),BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public TSagaStates DomainState => StateData.MachineState;
        private readonly List<Command> _messagesToDispatch = new List<Command>();

        public IReadOnlyCollection<ICommand> CommandsToDispatch => _messagesToDispatch;

        public void ClearCommandsToDispatch()
        {
            _messagesToDispatch.Clear();
        }

        IAggregate IDomainSaga.State => StateData;
 
        private readonly MethodInfo _transitMethod;
        public void Transit(DomainEvent msg)
        {
            MethodInfo genericTransit = _transitMethod.MakeGenericMethod(msg.GetType());
            genericTransit.Invoke(this, new object[] { msg});
        }

        protected void Dispatch(Command command)
        {
            command.SagaId = StateData.Id;
            _messagesToDispatch.Add(command);
        }

        protected StateMachine<TSagaStates, TSagaTriggers>.TriggerWithParameters<TEvent> RegisterEvent<TEvent>(
                                                                                                    TSagaTriggers trigger)
        {
            var triggerWithParameters = Machine.SetTriggerParameters<TEvent>(trigger);
            _eventsToTriggersMapping[typeof(TEvent)] = triggerWithParameters;
            return triggerWithParameters;
        }

        //TODO: make method non-generic
        protected internal void TransitState<T>(T message)
        {
            StateMachine<TSagaStates, TSagaTriggers>.TriggerWithParameters triggerWithParameters;
            if (!_eventsToTriggersMapping.TryGetValue(typeof(T), out triggerWithParameters))
                throw new UnbindedMessageRecievedException(message);

            if (Machine.CanFire(triggerWithParameters.Trigger))
            {
                var withParameters = (StateMachine<TSagaStates, TSagaTriggers>.TriggerWithParameters<T>) triggerWithParameters;
                Machine.Fire(withParameters, message);
            }
        }
    }

    //TODO: add policy for command unexpected failure handling
    public class StateSaga<TState, TTrigger, TStartMessage> :
        StateSaga<TState, TTrigger, SagaStateAggregate<TState, TTrigger>, TStartMessage>
        where TTrigger : struct
        where TState : struct
    {
        public StateSaga(SagaStateAggregate<TState, TTrigger> stateData) : base(stateData)
        {
        }
    }
}