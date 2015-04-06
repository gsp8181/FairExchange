using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grapevine.Server;

namespace FEClient
{
    static class Program
    {
        static Context form;

#if SINGLEINSTANCE
        private static string guid =
            ((GuidAttribute) Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (GuidAttribute), true)[0]).Value;
        static Mutex mutex = new Mutex(true,guid);
#endif


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if SINGLEINSTANCE
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
#endif
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(form = new Context());
#if SINGLEINSTANCE
                mutex.ReleaseMutex();
            }
#endif
        }
    }
}
