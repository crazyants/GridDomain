using System;
using CommonDomain.Core;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Node.Actors
{
    public class SagaHubActor<TSaga, TSagaState, TStartMessage> :
        PersistentHubActor where TSaga : IDomainSaga
        where TSagaState  : AggregateBase 
        where TStartMessage : DomainEvent
    {
        private readonly Type _actorType = typeof(SagaActor<TSaga, TSagaState, TStartMessage>);
        private readonly IPublisher _publisher;

        public SagaHubActor(IPublisher publisher)
        {
            _publisher = publisher;
        }

        protected override string GetChildActorName(object message)
        {
            if (message is DomainEvent)
            {
                return $"Saga_{typeof(TSaga).Name}_{GetChildActorId(message)}";
            }
            return null;
        }

        protected override Guid GetChildActorId(object message)
        {
            if (message is DomainEvent)
            {
                return (message as DomainEvent).SagaId;
            }
            return Guid.Empty;
        }

        protected override void OnReceive(object message)
        {
            var startMessage = message as TStartMessage;
            if (startMessage != null && startMessage.SagaId == Guid.Empty)
            {
                //send message back to publisher to reroute to some hub according to SagaId
                _publisher.Publish(startMessage.CloneWithSaga(Guid.NewGuid()));
                return;
            }
            
            base.OnReceive(message);
        }

        protected override Type GetChildActorType(object message)
        {
            return _actorType;
        }
    }
}