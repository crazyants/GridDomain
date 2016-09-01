using System;
using GridDomain.CQRS;

namespace GridDomain.Scheduling.Integration
{
    public class CompleteJob : Command
    {
        public string Name { get; private set; }
        public string Group { get; private set; }

        public CompleteJob(string name, string group)
        {
            Name = name;
            Group = group;
        }
    }


    public class QuartzJobStatus
    {
        public string Name { get; private set; }
        public string Group { get; private set; }

        public QuartzJobStatus(string name, string @group)
        {
            Name = name;
            Group = @group;
        }
    }
    public class QuartzJobCompleted : QuartzJobStatus
    {
        public object Result { get; private set; }

        public QuartzJobCompleted(string name, string group, object result):base(name,group)
        {
            Result = result;
        }
    }

    public class QuartzJobStarted : QuartzJobStatus
    {
        public Command Command { get; private set; }

        public QuartzJobStarted(string name, string group, Command cmd) : base(name, group)
        {
            Command = cmd;
        }
    }


    public class QuartzJobFault : QuartzJobStatus
    {
        public Exception Error { get; private set; }

        public QuartzJobFault(string name, string group, Exception ex) : base(name, group)
        {
            Error = ex;
        }
    }
}