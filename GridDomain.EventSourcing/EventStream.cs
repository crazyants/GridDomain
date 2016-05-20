using System;
using System.Collections.Generic;

namespace GridDomain.EventSourcing
{
public class EventStream: IAggregateState
    {
        private readonly List<DomainEvent> _events = new List<DomainEvent>();
        protected IDictionary<Type, Action<object>> _eventApplyRules; 

        public EventStream(Guid id)
        {
            Id = id;
            _eventApplyRules = new Dictionary<Type, Action<object>>();
        }


        public Guid Id { get; }

        public void Produce<T>(T domainEvent) where T : DomainEvent
        {
            Action<object> applyRule;
            if (!_eventApplyRules.TryGetValue(typeof (T), out applyRule))
                throw new DontNowHowToApplyEventException(domainEvent);

            applyRule.Invoke(domainEvent);
        }

        public void OnApply<T>(Action<T> applyRule)
        {
            _eventApplyRules[typeof (T)] = o => applyRule.Invoke((T) o);
        }

        public IReadOnlyCollection<DomainEvent> UncommitedEvents()
        {
            return _events;
        }

        public void ClearUncommited()
        {
            _events.Clear();
        }
    }
}