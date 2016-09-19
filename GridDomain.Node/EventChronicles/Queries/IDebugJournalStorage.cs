namespace GridDomain.Node.EventChronicles.Queries
{
    public interface IDebugJournalStorage
    {
        void AddOrUpdate(DebugJournalEntry[] entries);
    }
}