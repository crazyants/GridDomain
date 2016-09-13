using Automatonymous;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.MessageRouting.Infrastructure
{
    public class SampleSagaData : ISagaState
    {
        public string CurrentStateName { get; set; }

        public SampleSagaData(string stateName)
        {            
            CurrentStateName = stateName;
        }

        public SampleSagaData(State state)
        {            
            CurrentStateName = state.Name;
        }
    }
}