using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.CQRS;
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
using NMoneys;
using NUnit.Framework;

namespace GridDomain.Tests.Tools.Console
{
    [TestFixture]

    public class GridConsoleTests
    {
        private GridConsole _console;
        private GridDomainNode _node;

        [MTAThread]
        [TestFixtureSetUp]
        public void Given_existing_GridNode()
        {
            var container = new UnityContainer();
            var sampleDomainContainerConfiguration = new SampleDomainContainerConfiguration();
            container.Register(sampleDomainContainerConfiguration);

            var serverConfig = new TestGridNodeConfiguration();

            _node = new GridDomainNode(sampleDomainContainerConfiguration,
                                       new SampleRouteMap(container),
                                       () => serverConfig.CreateSystem());

            _node.Start(new LocalDbConfiguration());

            Thread.Sleep(5000);

            _console = new GridConsole(serverConfig.Network);
            _console.Connect();
        }

        [MTAThread]
        [TestFixtureTearDown]
        public void TurnOffNode()
        {
            _node.Stop();
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
            var testCommand = new ReplenishForCampaignCommand(new Guid("9B95ACDD-7E16-422A-9B92-658B7C7A7950"), "57ab2468cfb6ef06c47083bb", new Money(1000));


            _console.Execute(testCommand);
        }

    }

    public class ReplenishForCampaignCommand : Command
    {
        public Guid AccountId { get; }

        public string CampaignId { get; }

        public Money Amount { get; }

        public ReplenishForCampaignCommand(Guid accountId, string сampaignId, Money amount)
        {
            AccountId = accountId;
            CampaignId = сampaignId;
            Amount = amount;
        }
    }
}
