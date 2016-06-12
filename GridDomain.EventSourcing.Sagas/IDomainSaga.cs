using System;
using System.Collections.Generic;
using CommonDomain;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing.Sagas
{
    public interface IDomainSaga
    {
        IReadOnlyCollection<object> CommandsToDispatch { get; }
        void ClearCommandsToDispatch();
        IAggregate State { get; }
        void Transit(DomainEvent message);
    }

    public interface ISagaDescriptor
    {
        //TODO: enforce check all messages are DomainEvents
        IReadOnlyCollection<Type> AcceptEvents { get; }
        IReadOnlyCollection<Type> ProduceCommands { get; }
        Type StartMessage { get; } 
        Type StateType { get; }
        Type SagaType { get; }
    }
}