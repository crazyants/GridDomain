using System;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors
{
    public class CommandExecutionFailedException : Exception
    {
        public ICommand Command { get; }

        public CommandExecutionFailedException(ICommand command,Exception innerException):base("Command execution failed", innerException)
        {
            Command = command;
        }
    }
}