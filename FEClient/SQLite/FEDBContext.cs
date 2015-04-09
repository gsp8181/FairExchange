using System.Data.Entity;

namespace FEClient.SQLite
{
    class FedbContext : DbContext
    {
        //public FEDBContext(DbConnection conn) : base(conn,true) //TODO: should use autoregister
        //{
        //this.Database.CreateIfNotExists();
        //}
        /*protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions
                .Remove<PluralizingTableNameConvention>();
        }*/

        static FedbContext()
        {
            // Database initialize
#if DROP_DB_ON_START
            Database.SetInitializer(new DropCreateDatabaseAlways<FedbContext>());
#elif DEBUG
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<FEDBContext>());
#else
            Database.SetInitializer(new CreateDatabaseIfNotExists<FEDBContext>());  
#endif
            using (FedbContext db = new FedbContext())
                db.Database.Initialize(false);
        }

        public DbSet<PubKey> PubKeys { get; set; }

    }

}
