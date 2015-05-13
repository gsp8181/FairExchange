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

            var ip = destinationBox.Text;

            if (!ip.Contains(":"))
            {
                ip += ":6555";
            }


            var sendDialog = new SendDialog(ip, fileBox.Text, int.Parse(roundsBox.Text), int.Parse(complexityBox.Text),
                int.Parse(timeoutBox.Text));
            sendDialog.Show(); 
            Close();
        }

        private bool ValidateAll()
        {
            if (!AddressBoxIsIp)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(fileBox.Text))
                return false;

            if (!ValidateBoxes(roundsBox) || !ValidateBoxes(complexityBox) || !ValidateBoxes(timeoutBox))
                return false;

            var fileInfo = new FileInfo(fileBox.Text);
            return fileInfo.Exists && fileInfo.Length > 0; //TODO: verbose! like error box
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

        private void advancedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (advancedCheckBox.Checked)
            {
                if (MessageBox.Show("Do you wish to enable advanced options? " +
                                    "Setting incorrect values can help an adversary intercept your communications. " +
                                    "If you do not fully understand what you are doing, press Cancel.", 
                                    "Enable Advanced Options", 
                                    MessageBoxButtons.OKCancel, 
                                    MessageBoxIcon.Warning,
                                    MessageBoxDefaultButton.Button2) != DialogResult.OK)
                {
                    advancedCheckBox.Checked = false;
                    return;
                }
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

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar) && !Char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        private void roundsBox_Validated(object sender, EventArgs e)
        {
            var sendBox = (TextBox) sender;
            ValidateBoxes(sendBox);
        }

        private bool ValidateBoxes(TextBox sendBox)
        {
            if (sendBox.Text == "")
            {
                errorProvider.SetError(sendBox, "You must put in a number");
                return false;
            }
            if (int.Parse(sendBox.Text) <= 0)
            {
                errorProvider.SetError(sendBox, "Must be > 0");
                return false;
            }
            errorProvider.SetError(sendBox, string.Empty);
            return true;
        }
    }
}