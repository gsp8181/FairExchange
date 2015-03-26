using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TTPClient
{
    public partial class SendDialog : Form
    {
        private string ip;
        private Stream file;
        public SendDialog(string ip, Stream file)
        {
            InitializeComponent();
            MyResource.StartTransmission += MyResource_StartTransmission;
            this.ip = ip;
            this.file = file; //TODO: 60 second timer til something happens
            
        }

        void MyResource_StartTransmission(object sender, NotifyRequest addrSender, NotifyArgs callbackArgs)
        {
            callbackArgs.hasSet = true; //TODO: check filename and dispatch another thread which updates progress
        }

        private void SendDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            MyResource.StartTransmission -= MyResource_StartTransmission;
            file.Close();
        }
    }
}
