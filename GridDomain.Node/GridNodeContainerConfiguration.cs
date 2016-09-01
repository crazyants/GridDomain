using System;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Quartz;
using Microsoft.Practices.Unity;

namespace GridDomain.Node
{
    public class GridNodeContainerConfiguration : IContainerConfiguration
    {
        private readonly ActorSystem _actorSystem;
        private readonly IDbConfiguration _conf;
        private readonly TransportMode _transportMode;
        private readonly IQuartzConfig _config;

        public GridNodeContainerConfiguration(ActorSystem actorSystem,
            TransportMode transportMode,
            IQuartzConfig config = null)
        {
            _config = config;
            _transportMode = transportMode;
            _actorSystem = actorSystem;
        }

        public void Register(IUnityContainer container)
        {
            //TODO: replace with config
            IActorTransport transport;
            switch (_transportMode)
            {
                case TransportMode.Standalone:
                    transport = new AkkaEventBusTransport(_actorSystem);
                    break;
                case TransportMode.Cluster:
                    transport = new DistributedPubSubTransport(_actorSystem);
                    break;
                default:
                    throw new ArgumentException(nameof(_transportMode));
            }

            container.RegisterInstance<IPublisher>(transport);
            container.RegisterInstance<IActorSubscriber>(transport);
            container.RegisterInstance<IActorTransport>(transport);
            container.RegisterType<IHandlerActorTypeFactory, DefaultHandlerActorTypeFactory>();
            container.RegisterType<IAggregateActorLocator, DefaultAggregateActorLocator>();
            container.RegisterType<IPersistentChildsRecycleConfiguration, DefaultPersistentChildsRecycleConfiguration>();
            container.RegisterInstance<IAppInsightsConfiguration>(AppInsightsConfigSection.Default ??
                                                                  new DefaultAppInsightsConfiguration());
            container.RegisterInstance<IPerformanceCountersConfiguration>(PerformanceCountersConfigSection.Default ??
                                                                  new DefaultPerfCountersConfiguration());
            container.RegisterInstance(_actorSystem);
            container.Register(new SchedulerConfiguration(_config ?? new PersistedQuartzConfig()));
        }
    }
}