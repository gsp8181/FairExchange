using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Windows.Forms;

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
                var pubKey = (from p in db.PubKeys
                              where p.Email == email
                              select p).First();
                return pubKey;
            
        }

    }
}
