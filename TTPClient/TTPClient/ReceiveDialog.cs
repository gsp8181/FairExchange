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
using Grapevine.Client;
using Newtonsoft.Json.Linq;

namespace TTPClient
{
    public partial class ReceiveDialog : Form
    {
        private string fileName;
        private string ip;
        private FileInfo localFile = new FileInfo(Path.GetTempFileName());
        public ReceiveDialog(string ip, string fileName)
        {
            InitializeComponent();
            progressLabel.Text += fileName;
            this.ip = ip;
            this.fileName = fileName;
            MyResource.FileRecieved += MyResource_FileRecieved;
            MyResource.FileRecievedAndRespSent += MyResource_FileRecievedAndRespSent;
            backgroundWorker1.RunWorkerAsync();
        }

        void MyResource_FileRecievedAndRespSent(object sender, FileSend file)
        {
            if (fileName != file.fileName)
                return;
            using (StreamWriter sw = new StreamWriter(localFile.OpenWrite())) //TODO: on another thread
            {
                sw.Write(file.data);
            }
            this.Invoke((MethodInvoker)delegate
            {
                progressBar1.Style = ProgressBarStyle.Continuous;
                progressBar1.Value = 33;
                progressLabel.Text = "File recieved, processing";
            });
        }

        private void MyResource_FileRecieved(object sender, FileSend file, NotifyArgs callbackArgs)
        {
            if (this.fileName != file.fileName) //TODO: email!! or guid!
            {
                return;
            }
            callbackArgs.hasSet = true;

            //ShowBalloonTip(5000, "File Recieved", fileName, ToolTipIcon.Info);
        }

        private void ReceiveDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            MyResource.FileRecieved -= MyResource_FileRecieved;
            MyResource.FileRecievedAndRespSent -= MyResource_FileRecievedAndRespSent;
            this.Dispose();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) //TODO: does this have to be separate
        {
            var client = new RESTClient("http://" + ip + ":6555");
            var req = new RESTRequest("/start/");
            JObject data = new JObject { { "fileName", fileName }, { "email", SettingsWrapper.Instance.Email } };
            req.Method = Grapevine.HttpMethod.POST;
            req.ContentType = Grapevine.ContentType.JSON;
            req.Payload = data.ToString();
            var response = client.Execute(req);
        }
    }
}
