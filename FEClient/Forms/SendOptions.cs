using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Windows.Forms;
using FEClient.NotMyCode;

namespace FEClient.Forms
{
    public partial class SendOptions : Form
    {
        private readonly int _rounds;

        public SendOptions()
        {
            InitializeComponent();
            _rounds = RandomNumber.Value(500, 1500);
                //TODO: does this reprisent a problem because as convergence happens on 1500 it COULD get more rewarding to terminate
            roundsBox.Text = _rounds.ToString();
        }

        private bool AddressBoxIsIp
        {
            get
            {
                var ipText = destinationBox.Text;
                if (ipText.Contains(":"))
                {
                    var index = ipText.LastIndexOf(":", StringComparison.Ordinal);
                    ipText = ipText.Remove(index, ipText.Length - index);
                }
                IPAddress ipObj;
                return IPAddress.TryParse(ipText, out ipObj);
            }
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

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void okButton_Click(object sender, EventArgs e)
        {
            if (!ValidateAll())
            {
                return;
            }

            var ip = ""; //TODO: does not need to be used?!

            if (AddressBoxIsIp)
            {
                ip = destinationBox.Text;
            }

            if (!ip.Contains(":"))
            {
                ip += ":6555";
            }


            var sendDialog = new SendDialog(ip, fileBox.Text, int.Parse(roundsBox.Text), int.Parse(complexityBox.Text),
                int.Parse(timeoutBox.Text));
            sendDialog.Show(); //TODO: validate
            Close();
        }

        private bool ValidateAll()
        {
            if (!AddressBoxIsIp)
            {
                errorProvider.SetError(destinationBox, "Wrong format, should be an email or IP");
                return false;
            }
            if (string.IsNullOrWhiteSpace(fileBox.Text))
                return false;

            var fileInfo = new FileInfo(fileBox.Text);
            return fileInfo.Exists && fileInfo.Length > 0;
        }

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
            errorProvider.SetError(destinationBox,
                !AddressBoxIsIp ? "Wrong format, should be an IP address" : string.Empty);
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
                //roundsBox.Text = "1000";
                roundsBox.Text = _rounds.ToString();
                complexityBox.Text = "1300000";
                timeoutBox.Text = "5000";
                roundsBox.Enabled = false;
                complexityBox.Enabled = false;
                timeoutBox.Enabled = false;
            }
        }

        private void roundsBox_KeyPress(object sender, KeyPressEventArgs e) //TODO: you CAN still paste a number in!
        {
            if (!Char.IsNumber(e.KeyChar) && !Char.IsControl(e.KeyChar))
                e.Handled = true;
        }
    }
}