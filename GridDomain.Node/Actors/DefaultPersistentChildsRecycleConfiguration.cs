using System;

namespace GridDomain.Node.Actors
{
    public class DefaultPersistentChildsRecycleConfiguration : IPersistentChildsRecycleConfiguration
    {
        public TimeSpan ChildClearPeriod { get; } = TimeSpan.FromMinutes(1);
        public TimeSpan ChildMaxInactiveTime { get; } = TimeSpan.FromMinutes(30);
    }
}