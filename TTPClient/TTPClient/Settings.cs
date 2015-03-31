using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
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

        private void GenDSAKeysButton_Click(object sender, EventArgs e)
        {
            Security.Regenerate_DSA();
            MessageBox.Show("Regenerated Keys", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


    }
}
