using System.Linq;

namespace FEClient.SQLite
{
    class Adapter
    {
        private readonly static Adapter _Instance = new Adapter();
        public static Adapter Instance { get { return _Instance; } }


        public void insert(PubKey key)
        {
            var db = new FEDBContext();
            
                
                //Table<PubKey> statuses = db.GetTable<PubKey>();
                //statuses.InsertOnSubmit(key);
                //db.SubmitChanges();
                db.PubKeys.Add(key); 
                //db.PubKeys.InsertOnSubmit(key);
                db.SaveChanges();
            
        }

        public PubKey GetByEmail(string email)
        {
            var db = new FEDBContext();
            var pubKeys = (from p in db.PubKeys
                where p.Email == email
                orderby p.Id descending
                select p);

            return !pubKeys.Any() ? null : pubKeys.First();
        }

    }
}
