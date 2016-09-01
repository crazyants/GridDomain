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

    public class QuartzJobCompleted
    {
        public string Name { get; private set; }
        public string Group { get; private set; }
        public object Result { get; private set; }

        public QuartzJobCompleted(string name, string group, object result)
        {
            Name = name;
            Group = group;
            Result = result;
        }
    }
}