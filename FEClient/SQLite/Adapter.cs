using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.SqlTypes;
using System.Data.SQLite.EF6;
using System.Data.SQLite.Linq;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace FEClient.SQLite
{
    class Adapter
    {
        private readonly static Adapter _Instance = new Adapter();
        public static Adapter Instance { get { return _Instance; } }



            private readonly string connString;
        private readonly SQLiteConnection m_dbConnection;
        private readonly DataContext context;


        private Adapter()
        {
            string appDir = new DirectoryInfo(Application.UserAppDataPath).Parent.FullName;
            FileInfo SqLite = new FileInfo(/*AppDir*/Application.UserAppDataPath + @"\db.sqlite3");
            connString = "Data Source=" + SqLite.FullName + ";Version=3;DbLinqProvider=sqlite;";

            if (!SqLite.Exists)
                Create_DB();

            m_dbConnection = new SQLiteConnection(connString);
            m_dbConnection.Open();
            //context = new DataContext(connString);

        }

        private void Create_DB()
        {
            
        }

        public void insert(PubKey key)
        {
            using (var db = new FEDBContext(m_dbConnection))
            {
                
                //Table<PubKey> statuses = db.GetTable<PubKey>();
                //statuses.InsertOnSubmit(key);
                //db.SubmitChanges();
                db.PubKeys.Add(key);
                //db.PubKeys.InsertOnSubmit(key);
                db.SaveChanges();
            }
        }

        public PubKey GetByEmail(string email)
        {
            using (var db = new DataContext(m_dbConnection))
            {
                var pubKey = (from p in db.GetTable<PubKey>()
                              where p.Email == email
                              select p).First();
                return pubKey;
            }
        }

    }
}
