using System;
using System.Windows.Forms;

namespace TTPClient
{
    
    public class Context : ApplicationContext
    {
        public Form1 form1;
        public SettingsWrapper settings = new SettingsWrapper();
        public Context()
        {
            while (!settings.IsSet)
            {
                using (SettingsDialog dialog = new SettingsDialog())
                {
                    dialog.ShowDialog();
                }
            }

            form1 = new Form1();
            this.MainForm = form1;
            form1.Show();
        }

    }
}