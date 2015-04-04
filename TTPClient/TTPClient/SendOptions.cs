using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows.Forms;
using TTPClient.NotMyCode;

namespace TTPClient
{
    public partial class SendOptions : Form
    {
        private SettingsWrapper settings = SettingsWrapper.Instance;
        public SendOptions()
        {
            InitializeComponent();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            textBox2.Text = openFileDialog1.FileName;
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void okButton_Click(object sender, EventArgs e)
        {
            if (!validateAll())
            {
                return;
            }
            string ip = ""; //TODO: does not need to be used?!
            if (addressBoxIsEmail)
            {
                var url = settings.TTP + "/rest/sessions/";

                url = url + textBox1.Text;
                try
                {
                    var content = new WebClient().DownloadString(url);
                    ip = content;
                }
                catch (WebException ex)
                {
                    MessageBox.Show("Could not find. " + ex.Message);
                    return;
                }
            } else if (addressBoxIsIp)
            {
                ip = textBox1.Text;
            }


            var sendDialog = new SendDialog(ip, textBox2.Text, 100); //TODO:RESOLVE EMAIL OR IP??
            sendDialog.Show(); //TODO: validate
            this.Close();
        }

        private bool validateAll()
        {
            if (!addressBoxIsIp && !addressBoxIsEmail)
            {
                errorProvider.SetError(textBox1,"Wrong format, should be an email or IP");
                return false;
            }
            if (string.IsNullOrWhiteSpace(textBox2.Text))
                return false;

            var fileInfo = new FileInfo(textBox2.Text);
            if (fileInfo.Exists && fileInfo.Length > 0)
            {
                return true;
            }
            return false;
        }

        private bool addressBoxIsIp
        {
            get
            {
                IPAddress ipObj;
                return IPAddress.TryParse(textBox1.Text, out ipObj);
            }
        }

        private bool addressBoxIsEmail
        {
            get { return new RegexUtilities().IsValidEmail(textBox1.Text); }
        }

        private void textBox2_Validated(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
                return;

            var fileInfo = new FileInfo(textBox2.Text);
            if (!fileInfo.Exists || fileInfo.Length <= 0)
            {
                errorProvider.SetError(textBox2, "File does not exist or is blank");
            }
            else
            {
                errorProvider.SetError(textBox2, string.Empty);
            }
        }

        private void textBox1_Validated(object sender, EventArgs e)
        {
            if (!addressBoxIsIp && !addressBoxIsEmail)
            {
                errorProvider.SetError(textBox1, "Wrong format, should be an email or IP");
            }
            else
            {
                errorProvider.SetError(textBox1, string.Empty);
            }
        }
    }
}
