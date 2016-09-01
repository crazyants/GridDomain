using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Scheduling.Integration
{
    public class QuartzJobCompleted : QuartzJobStatus
    {
        public object Result { get; private set; }

        public QuartzJobCompleted(ScheduleKey key, object result):base(key)
        {
            Result = result;
        }
    }
}