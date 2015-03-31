using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grapevine.Server;

namespace TTPClient
{
    static class Program
    {
        public static RESTServer server = new RESTServer("+","6555","http","index.html",null,5);
        public static Context form;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(form = new Context());
        }
    }
}
