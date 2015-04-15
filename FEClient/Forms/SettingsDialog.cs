using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;
using System.Windows.Forms;
using FEClient.Security;

namespace FEClient.Forms
{
    public partial class SettingsDialog : Form
    {
        public SettingsDialog()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!ValidateEmail())
            {
                return;
            }
            SaveSettings();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void SaveSettings()
        {
            SettingsWrapper.Email = emailBox.Text;
        }

        private void LoadSettings()
        {
            emailBox.Text = SettingsWrapper.Email;
        }

        private void GenDSAKeysButton_Click(object sender, EventArgs e)
        {
            Rsa.Regenerate_RSA();
            MessageBox.Show("Regenerated Keys", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void emailBox_Validated(object sender, EventArgs e)
        {
            ValidateEmail();
        }

        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Net.Mail.MailAddress"
            )]
        private bool ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(emailBox.Text))
            {
                errorProvider.SetError(emailBox, "Email cannot be empty");
                return false;
            }
            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new MailAddress(emailBox.Text);
            }
            catch (Exception)
            {
                errorProvider.SetError(emailBox, "Email could not be verified");
                return false;
            }
            errorProvider.SetError(emailBox, String.Empty);
            return true;
        }

        private void publicKeyButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Rsa.PublicKey);
        }
    }
}