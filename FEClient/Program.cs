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
            if (Mutex.WaitOne(TimeSpan.Zero, true))
            {
#endif
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Context());
#if !MULTIINSTANCE
                Mutex.ReleaseMutex();
            }
#endif
        }

#if !MULTIINSTANCE
        private static readonly string Guid =
            ((GuidAttribute) Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (GuidAttribute), true)[0]).Value;

        private static readonly Mutex Mutex = new Mutex(true, Guid); //TODO: port specific
#endif
    }
}