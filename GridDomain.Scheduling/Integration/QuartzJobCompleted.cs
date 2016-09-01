namespace GridDomain.Scheduling.Integration
{
    public class QuartzJobCompleted : QuartzJobStatus
    {
        public object Result { get; private set; }

        public QuartzJobCompleted(string name, string group, object result):base(name,group)
        {
            Result = result;
        }
    }
}