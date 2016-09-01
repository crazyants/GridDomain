using GridDomain.CQRS;

namespace GridDomain.Scheduling.Akka.Messages
{
    public class ScheduleCommand
    {
        public Command Command { get; }
        public ScheduleKey Key { get; }
        public CommandExecutionOptions Options { get; }

        public ScheduleCommand(Command command, ScheduleKey key, CommandExecutionOptions options)
        {
            Command = command;
            Key = key;
            Options = options;
        }
    }
}