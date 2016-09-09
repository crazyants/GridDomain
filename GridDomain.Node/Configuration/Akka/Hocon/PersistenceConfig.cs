namespace GridDomain.Node.Configuration.Akka.Hocon
{
    internal class PersistenceConfig : IAkkaConfig
    {
        private readonly AkkaConfiguration _akka;

        public PersistenceConfig(AkkaConfiguration akka)
        {
            _akka = akka;
        }

        public string Build()
        {
            var akkaPersistenceConfig =
        @"persistence {
                    publish-plugin-commands = on
" + new PersistenceJournalConfig(_akka,new DomainEventAdaptersConfig()).Build() + @"
" + new PersistenceSnapshotConfig(_akka).Build() + @"
" + new PersistenceReadJournalConfig().Build() + @"
        }";
            return akkaPersistenceConfig;
        }
    }

    internal class PersistenceReadJournalConfig : IAkkaConfig
    {
        public string Build()
        {
          return @"akka.persistence.query.journal.sql {
                                class = ""Akka.Persistence.Query.Sql.SqlReadJournalProvider, Akka.Persistence.Query.Sql""
                                write-plugin = ""
                                refresh-interval = 1s
                                max-buffer-size = 100
                          }";
        }
    }
}