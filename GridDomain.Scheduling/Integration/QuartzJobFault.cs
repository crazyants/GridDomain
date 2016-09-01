using System;

namespace GridDomain.Scheduling.Integration
{
    public class QuartzJobFault : QuartzJobStatus
    {
        public Exception Error { get; private set; }

        public QuartzJobFault(string name, string group, Exception ex) : base(name, group)
        {
            Error = ex;
        }
    }
}