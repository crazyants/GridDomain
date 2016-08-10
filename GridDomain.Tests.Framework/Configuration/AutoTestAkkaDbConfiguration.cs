using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tests.Framework.Configuration
{
    public class AutoTestAkkaDbConfiguration : IAkkaDbConfiguration
    {
        public string SnapshotConnectionString
            => "Server=tcp:soloinfra.cloudapp.net,5099;Database=stageMembershipWrite;User ID=solomoto;Password=s0l0moto;Connection Timeout=30;MultipleActiveResultSets=True";

        public string JournalConnectionString
            => "Server=tcp:soloinfra.cloudapp.net,5099;Database=stageMembershipWrite;User ID=solomoto;Password=s0l0moto;Connection Timeout=30;MultipleActiveResultSets=True";

        public string MetadataTableName => "Metadata";
        public string JournalTableName => "JournalEntry";
        public string SnapshotTableName => "Snapshots";
    }
}