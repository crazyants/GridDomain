using System;
using System.Configuration;
using Akka.Actor;
using CommonDomain.Persistence;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.EventSourcing;
using GridDomain.Logging;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Quartz;
using Microsoft.Practices.Unity;

namespace GridDomain.Node
{
    [Obsolete("Use GridNodeConainerConfiguration instead")]
    //TODO: refactor to good config via IContainerConfiguration
    public static class CompositionRoot
    {
    //TODO: refactor to good config via IContainerConfiguration
        public static void Init(IUnityContainer container,
                                ActorSystem actorSystem,
                                TransportMode transportMode,
                                IQuartzConfig config = null)
        {
            container.Register(new GridNodeContainerConfiguration(actorSystem,transportMode,config));
        }
    }

}