using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace FEClient
{
    static class Program
    {

#if !MULTIINSTANCE
        private static string _guid =
            ((GuidAttribute) Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (GuidAttribute), true)[0]).Value;
        static Mutex _mutex = new Mutex(true,_guid); //TODO: port specific
#endif


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if !MULTIINSTANCE
            if (_mutex.WaitOne(TimeSpan.Zero, true))
            {
#endif
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Context());
#if !MULTIINSTANCE
                _mutex.ReleaseMutex();
            }
#endif
        }
    }
}
