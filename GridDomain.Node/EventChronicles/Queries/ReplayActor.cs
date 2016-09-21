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
    class DebugJournalProjectionActor : ReceiveActor
    {
        public int LastSequenceNumber;
        public string ReplayPersistenceId; 

        public DebugJournalProjectionActor()
        {
            var id = SqlReadJournal.Identifier;
           var journal = PersistenceQuery.Get(Context.System)
                                         .ReadJournalFor<SqlReadJournal>(id);

           var mat = ActorMaterializer.Create(Context.System);
           Source<string, NotUsed> allPersistenceIds = journal.AllPersistenceIds();
            //  var materizlizer = ActorMaterializer.Create(Context.System);
            //
            //  var dbWriter = new SqlBut
            //  materizlizer.Materialize(allPersistenceIds)
            
           Receive<StartAll>(s =>
           {
               allPersistenceIds.SelectAsyncUnordered(1,persistenceId =>
               {
                   var props = Props.Create(() => new PersistentIdResumableDumper(persistenceId, null));
                   var idDumper = Context.ActorOf(props, persistenceId);
                   return idDumper.Ask(new PersistentIdResumableDumper.Start());
               }).RunWith(Sink.Ignore<object>(), mat); ;
           });
        }

        internal class StartAll
        {
        }

     //   public override string PersistenceId => Self.Path.Name;
    }

    public class PersistentIdResumableDumper : ReceivePersistentActor
    {
        private long _lastSeqNumber = 0;
        public override string PersistenceId { get; }
        public PersistentIdResumableDumper(string id, IDebugJournalStorage storage)
        {
            PersistenceId = "DebugJournalWriter_" + id;

            var journal = PersistenceQuery.Get(Context.System)
                                          .ReadJournalFor<SqlReadJournal>("akka.persistence.query.journal.sql");

            var mat = ActorMaterializer.Create(Context.System);

            Command<Start>(s =>
            {
                var query = journal.CurrentEventsByPersistenceId(id, _lastSeqNumber, long.MaxValue);
                var totalSaved = SaveAll(query, mat);
                _lastSeqNumber += totalSaved;

                Sender.Tell(new Started());
                Self.Tell(new MonitorCurrent());
            });

            Command<MonitorCurrent>(c =>
            {
                
            });

        }

        private static long SaveAll(Source<EventEnvelope, NotUsed> query, ActorMaterializer mat)
        {
            int totalProcessed = 0;
            List<EventEnvelope> entriesToSave;
            query.Select(e =>
            {
                totalProcessed++;
                return totalProcessed;
            }).RunWith(Sink.Ignore<int>(), mat).Wait();

            return totalProcessed;
        }

        public class MonitorCurrent
        {
        }

        public class Started
        {
        }

        public class Start
        {
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
