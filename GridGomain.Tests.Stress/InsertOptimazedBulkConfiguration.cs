using System;
using GridDomain.Node.Actors;

namespace GridGomain.Tests.Stress
{
    public class InsertOptimazedBulkConfiguration : IPersistentChildsRecycleConfiguration
    {
        public TimeSpan ChildClearPeriod => TimeSpan.FromSeconds(10);
        public TimeSpan ChildMaxInactiveTime => TimeSpan.FromSeconds(5);
    }
}