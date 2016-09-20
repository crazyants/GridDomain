using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
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
        public DebugJournalBuild():base()
        {
            
        }

        [Test]
        public void ReadEventStreams_from_journal()
        {
            var actor = Sys.ActorOf(Props.Create(() => new DebugJournalProjectionActor()));

            actor.Tell(new DebugJournalProjectionActor.StartAll());
            Thread.Sleep(10000000);
        }
    }
}
