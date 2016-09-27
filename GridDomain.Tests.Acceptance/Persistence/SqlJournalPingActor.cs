using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Persistence;

namespace GridDomain.Tests.Acceptance.Persistence
{
    internal class SqlJournalPingActor : ReceivePersistentActor
    {
        private List<string> _events = new List<string>();

        public SqlJournalPingActor(string id)
        {
            PersistenceId = id;
            var plugin = Akka.Persistence.Persistence.Instance.Apply(Context.System).JournalFor(null);
            
            Command<SqlJournalPing>(m =>
            {
                plugin.Ask<ReadHighestSequenceNrSuccess>(new ReadHighestSequenceNr(1, PersistenceId, Self)).Wait();
                _events.Add(m.Payload);
                SaveSnapshot(_events);
                Sender.Tell(new Persisted { Payload = m.Payload });
            });
            Recover<SnapshotOffer>(o => _events = (List<string>)o.Snapshot);
        }

        protected override bool Receive(object message)
        {
            return base.Receive(message);
        }

        public override string PersistenceId { get; }
    }
}