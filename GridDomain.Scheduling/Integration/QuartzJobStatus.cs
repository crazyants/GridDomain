namespace GridDomain.Scheduling.Integration
{
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
}