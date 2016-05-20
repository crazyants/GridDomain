using System;

namespace GridDomain.EventSourcing
{
    internal class DontNowHowToApplyEventException : Exception
    {
        private readonly DomainEvent _domainEvent;

        public DontNowHowToApplyEventException(DomainEvent domainEvent)
        {
            _domainEvent = domainEvent;
        }
    }
}