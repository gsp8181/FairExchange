using System;
using System.Windows.Forms;
using TTPClient.Security;

namespace TTPClient
{
    public partial class SettingsDialog : Form
    {
        private SettingsWrapper settings = SettingsWrapper.Instance;
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
            Rsa.Regenerate_RSA();
            MessageBox.Show("Regenerated Keys", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ttpBox_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(ttpBox,
                string.IsNullOrWhiteSpace(ttpBox.Text) ? "TTP cannot be empty" : String.Empty);
        }

        private void emailBox_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(emailBox,
                string.IsNullOrWhiteSpace(emailBox.Text) ? "Email cannot be empty" : String.Empty);
        }
    }
}
