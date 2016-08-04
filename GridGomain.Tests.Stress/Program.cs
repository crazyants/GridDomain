using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Dispatch.SysMsg;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tests.SynchroniousCommandExecute;
using Microsoft.Practices.Unity;
using Ploeh.AutoFixture;

namespace GridGomain.Tests.Stress
{
    public class Program
    {
        public static void Main(params string[] args)
        {
            var unityContainer = new UnityContainer();
            unityContainer.Register(new SampleDomainContainerConfiguration());

            var cfg = new CustomContainerConfiguration(
                c => c.Register(new SampleDomainContainerConfiguration()),
                c => c.RegisterType<IPersistentChildsRecycleConfiguration, InsertOptimazedBulkConfiguration>(),
                c => c.RegisterType<IQuartzConfig,PersistedQuartzConfig>());

            Func<ActorSystem[]> actorSystemFactory = () => new[] {new StressTestAkkaConfiguration().CreateSystem()};

            var node = new GridDomainNode(cfg, new SampleRouteMap(unityContainer),actorSystemFactory);

            node.Start(new LocalDbConfiguration());

            var timer = new Stopwatch();
            
            var count = 10000000;
            var batchSize = 100;

            var batchesNum = count/batchSize;
            var minRefreshInterval = TimeSpan.FromSeconds(1);
            var lastStatisticTime = DateTime.Now;

            foreach(var batchCount in Enumerable.Range(0, batchesNum))
            {
                timer.Restart();
                var executionPlan = Enumerable.Range(0, batchSize)
                                              .Select(i => CreateTask(node))
                                              .ToArray();


                Task.WaitAll(executionPlan);

                if (DateTime.Now - lastStatisticTime >= minRefreshInterval)
                {
                    Console.Clear();
                    Console.WriteLine($"done latest {batchSize*4} commands in {timer.ElapsedMilliseconds} milliseconds");
                    Console.WriteLine($"percent done: {100.0 * batchCount*batchSize/count}%");
                }

            };

            Console.WriteLine($"Executed {count} batches in {timer.Elapsed}");

            Console.WriteLine("Sleeping");
            Thread.Sleep(60);

            node.Stop();
        }

        private static Task<Task<SampleAggregateChangedEvent>> CreateTask(GridDomainNode node)
        {
            var data = new Fixture();
            var createAggregateCommand = data.Create<CreateAggregateCommand>();
            var changeAggregateCommandA = new ChangeAggregateCommand(data.Create<int>(), createAggregateCommand.AggregateId);
            var changeAggregateCommandB = new ChangeAggregateCommand(data.Create<int>(), createAggregateCommand.AggregateId);
            var changeAggregateCommandC = new ChangeAggregateCommand(data.Create<int>(), createAggregateCommand.AggregateId);

            var createExpect = ExpectedMessage.Once<SampleAggregateCreatedEvent>(e => e.SourceId,
                createAggregateCommand.AggregateId);
            var changeAExpect = ExpectedMessage.Once<SampleAggregateChangedEvent>(e => e.SourceId,
                changeAggregateCommandA.AggregateId);
            var changeBExpect = ExpectedMessage.Once<SampleAggregateChangedEvent>(e => e.SourceId,
                changeAggregateCommandB.AggregateId);
            var changeCExpect = ExpectedMessage.Once<SampleAggregateChangedEvent>(e => e.SourceId,
                changeAggregateCommandC.AggregateId);

            var executionPlan = node.Execute(createAggregateCommand, createExpect)
                .ContinueWith(t1 => node.Execute(changeAggregateCommandA, changeAExpect))
                .ContinueWith(t2 => node.Execute(changeAggregateCommandB, changeBExpect))
                .ContinueWith(t3 => node.Execute(changeAggregateCommandC, changeCExpect));

            return executionPlan;
        }
    }
}
