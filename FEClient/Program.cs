using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace FEClient
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
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

#if !MULTIINSTANCE
        private static readonly string _guid =
            ((GuidAttribute) Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (GuidAttribute), true)[0]).Value;

        private static readonly Mutex _mutex = new Mutex(true, _guid); //TODO: port specific
#endif
    }
}