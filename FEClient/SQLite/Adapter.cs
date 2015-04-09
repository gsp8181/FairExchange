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



            private readonly string connString;
        private readonly SQLiteConnection m_dbConnection;
        private readonly DataContext context;


        private Adapter()
        {
            string appDir = new DirectoryInfo(Application.UserAppDataPath).Parent.FullName;
            FileInfo SqLite = new FileInfo(/*AppDir*/Application.UserAppDataPath + @"\db.sqlite3");
            connString = "Data Source=" + SqLite.FullName + ";Version=3;DbLinqProvider=sqlite;";

            bool create = false;

            if (!SqLite.Exists)
                create = true;

            m_dbConnection = new SQLiteConnection(connString);
            m_dbConnection.Open();

            if (create)
                Create_DB();
            //context = new DataContext(connString);

        }

        private void Create_DB()
        {
            string createPubKey = @"CREATE TABLE [PubKey] (
  [Id] INTEGER NOT NULL
, [Email] nvarchar(100) NOT NULL
, [Pem] nvarchar(200) NOT NULL
, CONSTRAINT [PK_PubKey] PRIMARY KEY ([Id])
);";
            SQLiteCommand command = new SQLiteCommand(createPubKey, m_dbConnection);
            command.ExecuteNonQuery();
        }

        public void insert(PubKey key)
        {
            var db = new FEDBContext(m_dbConnection);
            
                
                //Table<PubKey> statuses = db.GetTable<PubKey>();
                //statuses.InsertOnSubmit(key);
                //db.SubmitChanges();
                db.PubKeys.Add(key);
                //db.PubKeys.InsertOnSubmit(key);
                db.SaveChanges();
            
        }

        public PubKey GetByEmail(string email)
        {
            var db = new DataContext(m_dbConnection);
            
                var pubKey = (from p in db.GetTable<PubKey>()
                              where p.Email == email
                              select p).First();
                return pubKey;
            
        }

    }
}
