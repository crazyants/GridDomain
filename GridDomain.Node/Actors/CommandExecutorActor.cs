using System;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.Logging;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node.Actors
{
    public class CommandExecutorActor : TypedActor
    {
        private readonly ISoloLogger _log = LogManager.GetLogger();

        private readonly IActorTransport _transport;
        private readonly ActorMonitor _monitor;

        public CommandExecutorActor(IActorTransport transport)
        {
            _transport = transport;
            _monitor = new ActorMonitor(Context);
        }

        public void Handle(ICommand cmd)
        {
            _log.Trace("Starting execution of {command}",cmd);
            _monitor.IncrementMessagesReceived();
            _transport.Publish(cmd);
        }

        public void Handle(CommandPlan commandPlan)
        {
            _log.Trace("Starting execution of plan {command}", commandPlan);

            var props = Props.Create(() => new CommandWaiter(Sender, commandPlan.Command, commandPlan.ExpectedMessages));
            IActorRef waitActor;
            try
            {
                waitActor = Context.System.ActorOf(props, "MessageWaiter_command_" + commandPlan.Command.Id);
            }
            catch (Exception ex) when (ex is IllegalActorNameException || ex is InvalidActorNameException)
            {
                var newGuid = Guid.NewGuid();
                waitActor = Context.System.ActorOf(props, "MessageWaiter_command_" + newGuid);
                _log.Warn("executing command plan {id} with not-default waiter {waiterId} as plan id is already occupied by a waiter", commandPlan.Command.Id,newGuid);
            }

            foreach (var expectedMessage in commandPlan.ExpectedMessages)
                    _transport.Subscribe(expectedMessage.MessageType, waitActor);

            Handle(commandPlan.Command);
        }

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