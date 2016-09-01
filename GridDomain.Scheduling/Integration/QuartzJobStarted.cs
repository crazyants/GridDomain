using GridDomain.CQRS;
using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Scheduling.Integration
{
    public class QuartzJobStarted : QuartzJobStatus
    {
        public ICommand Command { get; private set; }

        public QuartzJobStarted(ScheduleKey key, ICommand cmd) : base(key)
        {
            Command = cmd;
        }
    }
}