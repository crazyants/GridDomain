using System;
using System.Collections.Generic;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{
    static internal class PersistentHubTestsStatus
    {
        public static readonly TimeSpan ChildMaxLifetime = TimeSpan.FromSeconds(2);
        public static readonly TimeSpan ChildClearTime = TimeSpan.FromSeconds(1);

        public enum PersistenceCase
        {
            Aggregate,
            IstanceSaga,
            StateSaga,
        }
    }
}