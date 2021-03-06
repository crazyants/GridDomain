namespace GridDomain.Tools.Persistence.SqlPersistence
{
    public interface IAkkaSqlPersistenceContext : System.IDisposable
    {
        System.Data.Entity.DbSet<JournalEntry> Journal { get; set; } // JournalEntry
        System.Data.Entity.DbSet<Metadata> Metadatas { get; set; } // Metadata
        System.Data.Entity.DbSet<Snapshot> Snapshots { get; set; } // Snapshots

        int SaveChanges();
        System.Threading.Tasks.Task<int> SaveChangesAsync();
        System.Threading.Tasks.Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken);
    }
}