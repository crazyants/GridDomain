using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Persistence.Query;
using Akka.Persistence.Query.Sql;
using Akka.Streams;
using Akka.Streams.Dsl;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Node.EventChronicles.Queries;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.Framework;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Chronicles
{
    [TestFixture]
    class DebugJournalBuild : PersistentSampleDomainTests
    {
        public DebugJournalBuild() : base()
        {

        }

        [Test]
        public void ReadEventStreams_from_journal()
        {
            var actor = Sys.ActorOf(Props.Create(() => new DebugJournalProjectionActor()));

            actor.Tell(new DebugJournalProjectionActor.StartAll());
            Thread.Sleep(10000000);
        }

        [Test]
        public void ReadJournal_test()
        {
            var id = SqlReadJournal.Identifier;
            var journal = PersistenceQuery.Get(Sys)
                .ReadJournalFor<SqlReadJournal>(id);

            var mat = ActorMaterializer.Create(Sys);
            Source<string, NotUsed> allPersistenceIds = journal.AllPersistenceIds();
            //  var materizlizer = ActorMaterializer.Create(Context.System);
            //
            //  var dbWriter = new SqlBut
            //  materizlizer.Materialize(allPersistenceIds)


            allPersistenceIds.Select(persistenceId =>
            {
                var props = Props.Create(() => new PersistentIdResumableDumper(persistenceId, null));
                var idDumper = Sys.ActorOf(props, persistenceId);
                return idDumper.Ask(new PersistentIdResumableDumper.Start());
            }).RunForeach(t => { },mat);//RunWith(Sink.Ignore<object>(), mat);
            ;
            Thread.Sleep(100000);
        }
    }
}
