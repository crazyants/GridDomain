using System;
using System.Collections.Generic;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaFactory<out TSaga, in TStartMessage> where TSaga : ISagaInstance
    {
        TSaga Create(TStartMessage message);
    }

    public interface ISagaProducer<out TSaga> where TSaga : ISagaInstance
    {
        TSaga Create(object data);
        
        //TODO: extract to separate type? 
        IReadOnlyCollection<Type> KnownDataTypes { get; }
    }

    public interface ISagaProducer<out TSaga, in TData> : ISagaProducer<TSaga> where TSaga : ISagaInstance
    {
        TSaga Create(TData data);
    }

    public class SagaProducer<TSaga> : ISagaProducer<TSaga> where TSaga : ISagaInstance
    {
        private readonly Dictionary<Type, Func<object, TSaga>> _factories = new Dictionary<Type,Func<object,TSaga>>();

        public void Register<TMessage>(ISagaFactory<TSaga, TMessage> factory)
        {
            Register(typeof(TMessage), m => factory.Create((TMessage) m));
        }

        public void Register(Type dataType, Func<object, TSaga> factory)
        {
            if (_factories.ContainsKey(dataType))
                throw new FactoryAlreadyRegisteredException(dataType);

            _factories[dataType] = factory.Invoke;
        }

        public TSaga Create(object data)
        {
            var type = data.GetType();
            Func<object, TSaga> factory;
            if (!_factories.TryGetValue(type, out factory))
                throw new CannotFindFactoryForSagaCreation(typeof(TSaga), data);

            return factory.Invoke(data);
        }

        public IReadOnlyCollection<Type> KnownDataTypes => _factories.Keys;
    }

    public class CannotFindFactoryForSagaCreation : Exception
    {
        public Type Saga { get; set; }
        public object Data { get; set; }

        public CannotFindFactoryForSagaCreation(Type saga,object data)
        {
            Saga = saga;
            Data = data;
        }
    }

    public class FactoryAlreadyRegisteredException : Exception
    {
        public Type Type { get; set; }

        public FactoryAlreadyRegisteredException(Type type)
        {
            Type = type;
        }
    }
}