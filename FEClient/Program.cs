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
            //AppDomain.SetData("DataDirectory", Application.UserAppDataPath);

#if !MULTIINSTANCE
            if (Mutex.WaitOne(TimeSpan.Zero, true))
            {
#endif
                Thread.GetDomain().SetData("DataDirectory", Application.UserAppDataPath);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                using (var context = new Context())
                { 
                    Application.Run(context);
                }
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