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
        public FEDBContext(DbConnection conn) : base(conn,true) //TODO: should use autoregister
        {
            this.Database.CreateIfNotExists();
        }
        /*protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions
                .Remove<PluralizingTableNameConvention>();
        }*/

        public DbSet<PubKey> PubKeys { get; set; }
        
    }
}
