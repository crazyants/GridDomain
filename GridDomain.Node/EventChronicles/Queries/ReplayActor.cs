using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka;
using Akka.Persistence;
using Akka.Persistence.Query;
using Akka.Persistence.Query.Sql;
using Akka.Streams;
using Akka.Streams.Dsl;

namespace GridDomain.Node.EventChronicles.Queries
{
    class ReplayActor<TAggregate> : ReceivePersistentActor
    {
        public ReplayActor()
        {
            var queries = PersistenceQuery.Get(Context.System).ReadJournalFor<SqlReadJournal>("akka.persistence.query.journal.sql");
          //  var mat = ActorMaterializer.Create(Context.System);
            Source<string, NotUsed> src = queries.AllPersistenceIds();
        }

        public override string PersistenceId => Self.Path.Name;
    }
}
