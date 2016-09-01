using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Logging;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly IActorRef _commandExecutorActor;
        private readonly TimeSpan _defaultCommandTimeout;

        public static CommandExecutor New(ActorSystem system, TimeSpan defaultCommandTimeout)
        {
            var props = system.DI().Props<CommandExecutorActor>();
            var executeActor = system.ActorOf(props, nameof(CommandExecutorActor));
            return new CommandExecutor(executeActor, defaultCommandTimeout);
        }

        public CommandExecutor(IActorRef commandExecutorActor, TimeSpan defaultCommandTimeout)
        {
            _defaultCommandTimeout = defaultCommandTimeout;
            _commandExecutorActor = commandExecutorActor;
        }

        public void Execute(params ICommand[] commands)
        {
            foreach (var cmd in commands)
                _commandExecutorActor.Tell(cmd);
        }

        public Task<object> Execute(ICommand command, ExpectedMessage[] expectedMessage, TimeSpan? timeout = null)
        {
            var maxWaitTime = timeout ?? _defaultCommandTimeout;
            return _commandExecutorActor.Ask<object>(new CommandPlan(command, expectedMessage), maxWaitTime)
                .ContinueWith(t =>
                {
                    if (t.IsCanceled)
                        throw new TimeoutException("Command execution timed out");

                    object result = null;
                    t.Result.Match()
                        .With<ICommandFault>(fault =>
                        {
                            var domainExcpetion = fault.Exception.UnwrapSingle();
                            ExceptionDispatchInfo.Capture(domainExcpetion).Throw();
                        })
                        .With<CommandExecutionFinished>(finish => result = finish.ResultMessage)
                        .Default(m => { throw new InvalidMessageException(m.ToPropsString()); });

                    return result;
                });
        }
    }
}