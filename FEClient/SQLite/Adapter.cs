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

        private readonly static string AppDir = new DirectoryInfo(Application.UserAppDataPath).Parent.FullName;
        private readonly static FileInfo SqLite = new FileInfo(AppDir + @"\db.sqlite3");

        private static readonly string connString = "Data Source=" + SqLite.FullName +
                                                    ";Version=3;DbLinqProvider=sqlite;";
        //private readonly SQLiteConnection m_dbConnection;
        private readonly DataContext context;


        private Adapter()
        {
            //m_dbConnection = new SQLiteConnection();
            //m_dbConnection.Open();
            //context = new DataContext(connString);

        }

        public void insert(PubKey key)
        {
            using (DataContext db = new DataContext(connString))
            {

                Table<PubKey> statuses = db.GetTable<PubKey>();
                statuses.InsertOnSubmit(key);
                db.SubmitChanges();
            }
        }

        public PubKey GetByEmail(string email)
        {
            using (DataContext db = new DataContext(connString))
            {
                var pubKey = (from p in db.GetTable<PubKey>()
                              where p.Email == email
                              select p).First();
                return pubKey;
            }
        }

    }
}
