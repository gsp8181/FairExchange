using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TTPClient
{
    public partial class ReceiveDialog : Form
    {
        private string fileName;
        private string ip;
        public ReceiveDialog(string ip, string fileName)
        {
            InitializeComponent();
            progressLabel.Text += fileName;
            this.ip = ip;
            this.fileName = fileName;
            MyResource.FileRecieved += MyResource_FileRecieved;
        }

        private void MyResource_FileRecieved(object sender, FileSend file, NotifyArgs callbackArgs)
        {
            if (this.fileName == file.fileName)
            {
                callbackArgs.hasSet = true;
            }
            this.Invoke((MethodInvoker) delegate
            {
                progressBar1.Style = ProgressBarStyle.Continuous;
                progressBar1.Value = 33;
                progressLabel.Text = "File recieved, processing";
            });
            //ShowBalloonTip(5000, "File Recieved", fileName, ToolTipIcon.Info);
        }

        private void ReceiveDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            MyResource.FileRecieved -= MyResource_FileRecieved;
            this.Dispose();
        }
    }
}
