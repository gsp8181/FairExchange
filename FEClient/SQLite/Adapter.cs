using System.Linq;

namespace FEClient.SQLite
{
    internal class Adapter
    {
        static Adapter()
        {
            Instance = new Adapter();
        }

        public static Adapter Instance { get; private set; }

        public void Insert(PubKey key)
        {
            var db = new FedbContext();

            db.PubKeys.Add(key);
            db.SaveChanges();
        }

        public PubKey GetByEmail(string email)
        {
            var db = new FedbContext();
            var pubKeys = (from p in db.PubKeys
                where p.Email == email
                orderby p.Id descending
                select p);

            return !pubKeys.Any() ? null : pubKeys.First();
        }
    }
}