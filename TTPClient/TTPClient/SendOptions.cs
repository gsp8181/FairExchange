using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace TTPClient
{
    public partial class SendOptions : Form
    {
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
            var sendDialog = new SendDialog(textBox1.Text, textBox2.Text); //TODO:RESOLVE EMAIL OR IP??
            sendDialog.Show(); //TODO: validate
            this.Close();
        }
    }
}
