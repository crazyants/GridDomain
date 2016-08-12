using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tests.SynchroniousCommandExecute;
using GridDomain.Tools;
using GridDomain.Tools.Console;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Tools.Console
{

    [TestFixture]
    [Ignore("Ignored until fix failure in bulk tests run")]
    public class GridConsole_connect_to_existing_environment
    {
        private GridConsole _console;
        private GridDomainNode _node;

        [TestFixtureSetUp]
        public void Given_existing_GridNode()
        {
           // var jakkuEnvConfig = new AkkaNetworkAddress("Membership", "40.118.71.240", 8085);
            var jakkuEnvConfig = new AkkaNetworkAddress("LocalSystem", "127.0.0.1", 8080);
            _console = new GridConsole(jakkuEnvConfig);
            _console.Connect();
        }

        [TestFixtureTearDown]
        public void TurnOffNode()
        {
            _console.Dispose();
        }

        [Then]
        public void Can_manual_reconnect_several_times()
        {
            _console.Connect();
        }

        [Then]
        public void NodeController_is_located()
        {
            Assert.NotNull(_console.NodeController);
        }

        [Then]
        public void Console_commands_are_executed()
        {
            var command = new CreateSampleAggregateCommand(42, Guid.NewGuid(), Guid.NewGuid());

            var expect = ExpectedMessage.Once<SampleAggregateCreatedEvent>(e => e.SourceId, command.AggregateId);

            var evt = _console.Execute(command, TimeSpan.FromSeconds(30), expect);
            Assert.AreEqual(command.Parameter.ToString(), evt.Value);
        }

    }
}
