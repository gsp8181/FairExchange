using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEClient.SQLite
{
    class FEDBContext : DbContext
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

        static FEDBContext()
        {
            // Database initialize
#if DROP_DB_ON_START
            Database.SetInitializer(new DropCreateDatabaseAlways<FEDBContext>());
#elif DEBUG
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<FEDBContext>());
#else
            Database.SetInitializer(new CreateDatabaseIfNotExists<FEDBContext>());  
#endif
            using (FEDBContext db = new FEDBContext())
                db.Database.Initialize(false);
        }

        public DbSet<PubKey> PubKeys { get; set; }

    }

}
