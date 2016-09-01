using System;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.CQRS.Messaging;
using GridDomain.Logging;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors
{
    public class GridNodeController : TypedActor
    {
        private readonly ISoloLogger _log = LogManager.GetLogger();
        private readonly IMessageRouteMap _messageRouting;

        public GridNodeController(IMessageRouteMap messageRouting)
        {
            _messageRouting = messageRouting;
            _monitor = new ActorMonitor(Context);
        }

        public void Handle(Start msg)
        {
            _monitor.IncrementMessagesReceived();
            LogManager.SetLoggerFactory(new DefaultLoggerFactory(new DefaultLoggerConfiguration()));

            var system = Context.System;
            var routingActor = system.ActorOf(system.DI().Props(msg.RoutingActorType),msg.RoutingActorType.Name);

            var actorMessagesRouter = new ActorMessagesRouter(routingActor, new DefaultAggregateActorLocator());
            _messageRouting.Register(actorMessagesRouter);

            //TODO: replace with message from router
            Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(3), Sender, new Started(), Self);
        }
        
        public class Start
        {
            public Type RoutingActorType;
        }

        public class Started
        {
        }

        private readonly ActorMonitor _monitor;

        protected override void PreStart()
        {
            _monitor.IncrementActorStarted();
        }

        protected override void PostStop()
        {
            _monitor.IncrementActorStopped();
        }
        protected override void PreRestart(Exception reason, object message)
        {
            _monitor.IncrementActorRestarted();
        }
    }
}