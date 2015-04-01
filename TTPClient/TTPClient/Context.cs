using System;
using System.Windows.Forms;

namespace TTPClient
{
    
    public class Context : ApplicationContext
    {
        public Form1 form1;
        public Context()
        {
            while (!SettingsWrapper.Instance.IsSet)
            {
                using (SettingsDialog dialog = new SettingsDialog())
                {
                    dialog.ShowDialog(); //TODO: if cancel then quit
                }
            }

            SettingsWrapper.Instance.RegWithTracker(); //TODO: is port open? notify registration balloon?

            form1 = new Form1();
            this.MainForm = form1;
            form1.Show();
        }

    }
}