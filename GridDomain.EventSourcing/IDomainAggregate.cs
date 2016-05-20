using System;
using System.Security.Cryptography.X509Certificates;

namespace GridDomain.EventSourcing
{
    public interface IDomainAggregate
    {
        Guid Id { get; }
        IAggregateState Events { get; }
    }
}