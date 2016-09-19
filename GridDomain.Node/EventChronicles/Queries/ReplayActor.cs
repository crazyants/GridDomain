using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Persistence;
using Akka.Persistence.Query;
using Akka.Persistence.Query.Sql;
using Akka.Streams;
using Akka.Streams.Dsl;

namespace GridDomain.Node.EventChronicles.Queries
{
    class ReplayActor<TAggregate> : ReceiveActor
    {
        public int LastSequenceNumber;
        public string ReplayPersistenceId; 

        public ReplayActor()
        {
           var journal = PersistenceQuery.Get(Context.System)
                                         .ReadJournalFor<SqlReadJournal>("akka.persistence.query.journal.sql");

           var mat = ActorMaterializer.Create(Context.System);
              Source<string, NotUsed> allPersistenceIds = journal.AllPersistenceIds();
            //  var materizlizer = ActorMaterializer.Create(Context.System);
            //
            //  var dbWriter = new SqlBut
            //  materizlizer.Materialize(allPersistenceIds)
            Receive<StartAll>(s =>
            {
                allPersistenceIds.SelectAsync(1, e =>
                {
                    var props = Props.Create(() => new PersistentIdResumableDumper(e));
                    var idDumper = Context.ActorOf(props);
                    idDumper.Ask(start)
                })
            })
        }

        internal class StartAll
        {
        }

        public override string PersistenceId => Self.Path.Name;
    }

    public class PersistentIdResumableDumper : ReceiveActor// ReceivePersistentActor
    {
        public PersistentIdResumableDumper(string id)
        {
            var journal = PersistenceQuery.Get(Context.System)
                                     .ReadJournalFor<SqlReadJournal>("akka.persistence.query.journal.sql");
        }
    }

    public class EventStreamDebugWriter : ActorBase
    {
        private readonly IDebugJournalStorage _storage;
        private readonly string _id;

        public EventStreamDebugWriter(IDebugJournalStorage storage)
        {
            _storage = storage;
        }

        protected override bool Receive(object message)
        {
           // _state = UpdateState(_state, message);
           // if (_state.IsReadyToSave())
           //     _store.Save(new Record(_state));
            return true;
        }
    }
}
