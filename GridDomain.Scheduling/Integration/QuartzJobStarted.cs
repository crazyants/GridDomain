using GridDomain.CQRS;

namespace GridDomain.Scheduling.Integration
{
    public class QuartzJobStarted : QuartzJobStatus
    {
        public Command Command { get; private set; }

        public QuartzJobStarted(string name, string group, Command cmd) : base(name, group)
        {
            Command = cmd;
        }
    }
}