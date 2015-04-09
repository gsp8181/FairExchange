using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace FEClient
{
    public partial class SendOptions : Form
    {
        private SettingsWrapper _settings = SettingsWrapper.Instance;
        public SendOptions()
        {
            InitializeComponent();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            fileBox.Text = openFileDialog.FileName;
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void okButton_Click(object sender, EventArgs e)
        {
            if (!validateAll())
            {
                return;
            }

            string ip = ""; //TODO: does not need to be used?!
            /*if (addressBoxIsEmail)
            {
                var url = settings.TTP + "/rest/sessions/";

                url = url + destinationBox.Text;
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
            } else*/ if (addressBoxIsIp)
            {
                ip = destinationBox.Text;
            }

            if (!ip.Contains(":"))
            {
                ip += ":6555";
            }


            var sendDialog = new SendDialog(ip, fileBox.Text, int.Parse(roundsBox.Text), int.Parse(complexityBox.Text), int.Parse(timeoutBox.Text));
            sendDialog.Show(); //TODO: validate
            Close();
        }

        private bool validateAll()
        {
            if (!addressBoxIsIp)
            {
                errorProvider.SetError(destinationBox,"Wrong format, should be an email or IP");
                return false;
            }
            if (string.IsNullOrWhiteSpace(fileBox.Text))
                return false;

            var fileInfo = new FileInfo(fileBox.Text);
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
                var ipText = destinationBox.Text;
                if (ipText.Contains(':'))
                {
                    var index = ipText.LastIndexOf(":");
                    ipText = ipText.Remove(index, ipText.Count() - index);
                }
                IPAddress ipObj;
                return IPAddress.TryParse(ipText, out ipObj);
            }
        }

        //private bool addressBoxIsEmail
        //{
        //    get { return new RegexUtilities().IsValidEmail(destinationBox.Text); }
        //}

        private void textBox2_Validated(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(fileBox.Text))
                return;

            var fileInfo = new FileInfo(fileBox.Text);
            if (!fileInfo.Exists || fileInfo.Length <= 0)
            {
                errorProvider.SetError(fileBox, "File does not exist or is blank");
            }
            else
            {
                errorProvider.SetError(fileBox, string.Empty);
            }
        }

        private void textBox1_Validated(object sender, EventArgs e)
        {
            if (!addressBoxIsIp)
            {
                errorProvider.SetError(destinationBox, "Wrong format, should be an IP address");
            }
            else
            {
                errorProvider.SetError(destinationBox, string.Empty);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (advancedCheckBox.Checked)
            {
                roundsBox.Enabled = true;
                complexityBox.Enabled = true;
                timeoutBox.Enabled = true;
            }
            else
            {
                roundsBox.Text = "1000";
                complexityBox.Text = "1300000";
                timeoutBox.Text = "5000";
                roundsBox.Enabled = false;
                complexityBox.Enabled = false;
                timeoutBox.Enabled = false;
            }
        }

        private void roundsBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            //var textSender = (TextBox)sender;
            if(!Char.IsNumber(e.KeyChar) && !Char.IsControl(e.KeyChar))
            e.Handled = true;
        }
    }
}
