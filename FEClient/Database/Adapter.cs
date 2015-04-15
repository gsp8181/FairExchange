using System.Linq;

namespace FEClient.Database
{
    internal static class Adapter
    {

        public static void Insert(PubKey key)
        {
            using(var db = new FedbContext())
            { 
            db.PubKeys.Add(key);
            db.SaveChanges();
            }
        }

        public static PubKey GetByEmail(string email)
        {
            using (var db = new FedbContext())
            {
                var pubKeys = (from p in db.PubKeys
                    where p.Email == email
                    orderby p.Id descending
                    select p);

                return !pubKeys.Any() ? null : pubKeys.First();
            }
        }
    }
}