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
        public SettingsDialog() //TODO: cancelbutton, regen enabled as a boolean [default true]
        {
            InitializeComponent();
            LoadSettings();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //errorProvider1.Clear();
            if (string.IsNullOrWhiteSpace(ttpBox.Text) || string.IsNullOrWhiteSpace(emailBox.Text))
            {
                return;
            }
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

        private void ttpBox_Validated(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ttpBox.Text))
            {
                errorProvider1.SetError(ttpBox,"TTP cannot be empty");
            }
            else
            {
                errorProvider1.SetError(ttpBox, String.Empty);
            }
        }

        private void emailBox_Validated(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(emailBox.Text))
            {
                errorProvider1.SetError(emailBox, "Email cannot be empty");
            }
            else
            {
                errorProvider1.SetError(emailBox,String.Empty);
            }
        }


    }
}
