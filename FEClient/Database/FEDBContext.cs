using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;

namespace FEClient.Database
{
    internal class FedbContext : DbContext
    {
        static FedbContext()
        {
            // Database initialize
#if DROP_DB_ON_START
            System.Data.Entity.Database.SetInitializer(new DropCreateDatabaseAlways<FedbContext>());
#elif DEBUG
            System.Data.Entity.Database.SetInitializer(new DropCreateDatabaseIfModelChanges<FedbContext>());
#else
            System.Data.Entity.Database.SetInitializer(new CreateDatabaseIfNotExists<FedbContext>());  
#endif
            using (var db = new FedbContext())
                db.Database.Initialize(false);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public DbSet<PubKey> PubKeys { get; set; }
    }
}