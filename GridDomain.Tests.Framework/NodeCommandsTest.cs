using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.DI.Core;
using Akka.TestKit.NUnit;
using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Logging;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Tests.Framework.Configuration;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Framework
{
    public abstract class NodeCommandsTest : TestKit
    {
        protected static readonly AkkaConfiguration AkkaConf = new AutoTestAkkaConfiguration();
        protected GridDomainNode GridNode;
        private readonly Stopwatch _watch = new Stopwatch();
        private IActorSubscriber _actorSubscriber;
        private readonly bool _clearDataOnStart;

        protected NodeCommandsTest(string config, string name = null, bool clearDataOnStart = true) : base(config, name)
        {
            _clearDataOnStart = clearDataOnStart;
        }

        protected abstract TimeSpan Timeout { get; }

        [TestFixtureTearDown]
        public void DeleteSystems()
        {
            Console.WriteLine();
            Console.WriteLine("Stopping node");
            GridNode.Stop();
        }

        [TestFixtureSetUp]
        protected void Init()
        {
            var autoTestGridDomainConfiguration = TestEnvironment.Configuration;
            if (_clearDataOnStart)
                TestDbTools.ClearData(autoTestGridDomainConfiguration, AkkaConf.Persistence);

            GridNode = CreateGridDomainNode(AkkaConf, autoTestGridDomainConfiguration);
            GridNode.Start(autoTestGridDomainConfiguration);
            _actorSubscriber = GridNode.Container.Resolve<IActorSubscriber>();

        }

        public T LoadAggregate<T>(Guid id) where T : AggregateBase
        {
            var props = GridNode.System.DI().Props<AggregateActor<T>>();
            var name = AggregateActorName.New<T>(id).ToString();
            var actor = ActorOfAsTestActorRef<AggregateActor<T>>(props, name);
            Thread.Sleep(1000); //wait for actor recover
            return actor.UnderlyingActor.Aggregate;
        }



        public TSagaState LoadSagaState<TSaga, TSagaState, TStartMessage>(Guid id) where TStartMessage : DomainEvent where TSagaState : AggregateBase where TSaga : IDomainSaga
        {
            var props = GridNode.System.DI().Props<SagaActor<TSaga, TSagaState, TStartMessage>>();
            var name = AggregateActorName.New<TSagaState>(id).ToString();
            var actor = ActorOfAsTestActorRef<SagaActor<TSaga, TSagaState, TStartMessage>>(props, name);
            Thread.Sleep(1000); //wait for actor recover
            return (TSagaState)actor.UnderlyingActor.Saga.State;
        }

        protected abstract GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig);

        private Type[] GetFaults(ICommand[] commands)
        {
            var faultGeneric = typeof(CommandFault<>);
            return commands.Select(c => faultGeneric.MakeGenericType(c.GetType())).ToArray();
        }
        protected ExpectedMessagesRecieved ExecuteAndWaitFor<TEvent>(params ICommand[] commands)
        {
            var messageTypes = GetFaults(commands).Concat(new[] { typeof(TEvent) }).ToArray();
            return ExecuteAndWaitFor(messageTypes, commands);
        }
        protected ExpectedMessagesRecieved ExecuteAndWaitFor<TMessage1, TMessage2>(params ICommand[] commands)
        {
            var messageTypes = GetFaults(commands).Concat(new[] { typeof(TMessage1), typeof(TMessage2) }).ToArray();
            return ExecuteAndWaitFor(messageTypes, commands);
        }

        private ExpectedMessagesRecieved WaitForFirstOf(Action act, bool failOnCommandFault = true, params Type[] messageTypes)
        {
            var toWait = messageTypes.Select(m => new MessageToWait(m, 1)).ToArray();
            var actor = GridNode.System
                                .ActorOf(Props.Create(() => new MessageWaiter(toWait, TestActor)),
                                         "MessageWaiter_" + Guid.NewGuid());

            foreach (var m in messageTypes)
                _actorSubscriber.Subscribe(m, actor);

            act();

            Console.WriteLine();
            Console.WriteLine($"Execution finished, wait started with timeout {Timeout}");

            var msg = (ExpectedMessagesRecieved)FishForMessage(m => m is ExpectedMessagesRecieved, Timeout);
            _watch.Stop();

            Console.WriteLine();
            Console.WriteLine($"Wait ended, total wait time: {_watch.Elapsed}");
            Console.WriteLine("Stopped after message received:");
            Console.WriteLine("------begin of message-----");
            Console.WriteLine(msg.ToPropsString());
            Console.WriteLine("------end of message-----");

            if (failOnCommandFault && msg.Message is ICommandFault)
            {
                Assert.Fail($"Command fault received: {msg.ToPropsString()}");
            }

            return msg;
        }

        protected ExpectedMessagesRecieved ExecuteAndWaitFor(Type[] messageTypes, params ICommand[] commands)
        {
            return WaitForFirstOf(() => Execute(commands), true, messageTypes);
        }

        protected ExpectedMessagesRecieved WaitFor<TMessage>(bool failOnFault = true)
        {
            return WaitForFirstOf(() => { }, failOnFault, typeof(TMessage));
        }

        private void Execute(ICommand[] commands)
        {
            Console.WriteLine("Starting execute");

            var commandTypes = commands.Select(c => c.GetType())
                .GroupBy(c => c.Name)
                .Select(g => new { Name = g.Key, Count = g.Count() });

            foreach (var commandStat in commandTypes)
            {
                Console.WriteLine($"Executing {commandStat.Count} of {commandStat.Name}");
            }

            _watch.Restart();

            foreach (var c in commands)
            {
                GridNode.Execute(c);
            }
        }
    }
}