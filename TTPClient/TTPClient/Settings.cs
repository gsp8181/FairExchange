using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TTPClient.Properties;

namespace TTPClient
{
    public partial class SettingsDialog : Form
    {
        public SettingsDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Form1.RegBySettings();
            this.Close();
        }

        public void SaveSettings()
        {

            Settings.Default["Email"] = this.emailBox.Text;
            Settings.Default["TTP"] = this.textBox1.Text;

            Settings.Default.Save();
        }

        public void LoadSettings()
        {
            emailBox.Text = (string) Settings.Default["Email"];
            textBox1.Text = (string) Settings.Default["TTP"];

        }
    }
}
