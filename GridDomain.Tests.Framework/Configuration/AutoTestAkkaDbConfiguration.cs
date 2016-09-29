using System.Configuration;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tests.Framework.Configuration
{

   
    public class AutoTestAkkaDbConfiguration : IAkkaDbConfiguration
    {
        public string SnapshotConnectionString
        => "Data Source=(local);Database=AutoTestAkka;Integrated Security = true";

        public string JournalConnectionString
            => "Data Source=(local);Database=AutoTestAkka;Integrated Security = true";

        public string MetadataTableName => "Metadata";
        public string JournalTableName => "Journal";
        public string SnapshotTableName => "Snapshots";
    }



    public class ConfigAkkaDbConfiguration : IAkkaDbConfiguration
    {
        public const string WriteDatabaseConnectionStringName = "GridDomainWriteTestString";

        public string SnapshotConnectionString => ConfigurationManager.ConnectionStrings[WriteDatabaseConnectionStringName].ConnectionString;

        public string JournalConnectionString => ConfigurationManager.ConnectionStrings[WriteDatabaseConnectionStringName].ConnectionString;

        public string MetadataTableName => "Metadata";
        public string JournalTableName => "Journal";
        public string SnapshotTableName => "Snapshots";
    }
}