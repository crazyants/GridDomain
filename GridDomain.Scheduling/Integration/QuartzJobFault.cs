using System;
using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Scheduling.Integration
{
    public class QuartzJobFault : QuartzJobStatus
    {
        public Exception Error { get; private set; }

        public QuartzJobFault(ScheduleKey key, Exception ex) : base(key)
        {
            Error = ex;
        }
    }
}