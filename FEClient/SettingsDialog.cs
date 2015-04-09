using System;
using System.Windows.Forms;
using FEClient.Security;

namespace FEClient
{
    public partial class SettingsDialog : Form
    {
        private SettingsWrapper _settings = SettingsWrapper.Instance;
        public SettingsDialog() //TODO: cancelbutton, regen enabled as a boolean [default true]
        {
            InitializeComponent();
            LoadSettings();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //errorProvider1.Clear();
            if (string.IsNullOrWhiteSpace(emailBox.Text))
            {
                return;
            }
            SaveSettings();
            DialogResult = DialogResult.OK;
            Close();
        }

        public void SaveSettings()
        {

            _settings.Email = emailBox.Text;
        }

        public void LoadSettings()
        {
            emailBox.Text = _settings.Email;

        }

        private void GenDSAKeysButton_Click(object sender, EventArgs e)
        {
            Rsa.Regenerate_RSA();
            MessageBox.Show("Regenerated Keys", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void emailBox_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(emailBox,
                string.IsNullOrWhiteSpace(emailBox.Text) ? "Email cannot be empty" : String.Empty);
        }

        private void publicKeyButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Rsa.GetPublicKey());
        }
    }
}
