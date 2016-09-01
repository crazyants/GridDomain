using GridDomain.Scheduling.Akka.Messages;

namespace GridDomain.Scheduling.Integration
{
    public class QuartzJobStatus
    {
        public string Name { get; private set; }
        public string Group { get; private set; }
        private QuartzJobStatus(string name, string @group)
        {
            Name = name;
            Group = @group;
        }

        public QuartzJobStatus(ScheduleKey key) : this(key.Name, key.Group)
        {
        }
    }
}