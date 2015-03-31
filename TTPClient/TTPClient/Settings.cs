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
        private SettingsWrapper settings = new SettingsWrapper();
        public SettingsDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveSettings();
            settings.RegWithTracker();
            this.Close();
        }

        public void SaveSettings()
        {

            settings.Email = this.emailBox.Text;
            settings.TTP = this.ttpBox.Text;
        }

        public void LoadSettings()
        {
            emailBox.Text = settings.Email;
            ttpBox.Text = settings.TTP;

        }

        private void GenDSAKeysButton_Click(object sender, EventArgs e)
        {
            Security.Regenerate_DSA();
            MessageBox.Show("Regenerated Keys", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


    }
}
