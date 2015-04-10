using System.Data.Entity;

namespace FEClient.SQLite
{
    internal class FedbContext : DbContext
    {
        static FedbContext()
        {
            // Database initialize
#if DROP_DB_ON_START
            Database.SetInitializer(new DropCreateDatabaseAlways<FedbContext>());
#elif DEBUG
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<FedbContext>());
#else
            Database.SetInitializer(new CreateDatabaseIfNotExists<FedbContext>());  
#endif
            using (var db = new FedbContext())
                db.Database.Initialize(false);
        }

        public DbSet<PubKey> PubKeys { get; set; }
    }
}