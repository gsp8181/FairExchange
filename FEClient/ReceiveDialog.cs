using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FEClient.API;
using Grapevine.Client;
using Newtonsoft.Json.Linq;

namespace FEClient
{
    public partial class ReceiveDialog : Form
    {
        private string fileName;
        private string ip;
        private string guid;
        private FileInfo localFile = new FileInfo(Path.GetTempFileName()); //TODO: why not just hold in memory?
        private string iv;
        private Stack<string> dict = new Stack<string>(); //TODO: holds I
        private bool stopped = false;
        private int complexity;
        public ReceiveDialog(NotifyRequest startObj)
        {
            InitializeComponent();
            progressLabel.Text += fileName;
            this.ip = startObj.ip;
            this.fileName = startObj.fileName;
            this.guid = startObj.guid;
            timer2.Interval = startObj.timeout;
            this.complexity = startObj.complexity;
            ClientRestApi.FileRecieved += MyResource_FileRecieved;
            ClientRestApi.FileRecievedAndRespSent += MyResource_FileRecievedAndRespSent;
            ClientRestApi.KeyRecieved += ClientRestApi_KeyRecieved;
            saveFileDialog1.FileName = fileName;
            saveFileDialog1.DefaultExt = new FileInfo(fileName).Extension;
            backgroundWorker1.RunWorkerAsync();
        }

        void ClientRestApi_KeyRecieved(object sender, KeyArgs key, NotifyArgs callbackArgs)
        {
            if (guid != key.guid || !stopped)
            {
                return;
            }
            dict.Push(key.key);
            callbackArgs.hasSet = true;

            this.Invoke((MethodInvoker) delegate
            {
                timer2.Stop();
                timer2.Start();
            });
        }

        void MyResource_FileRecievedAndRespSent(object sender, FileSend file)
        {
            if (guid != file.guid) //TODO: and filenames
                return;

            using (StreamWriter sw = new StreamWriter(localFile.OpenWrite())) //TODO: on another thread
            {
                sw.Write(file.data); //TODO: WHY?
            }
            iv = file.iv;
            this.Invoke((MethodInvoker)delegate //TODO; does this happen AFTER keys start coming?
            {
                progressBar1.Style = ProgressBarStyle.Continuous;
                progressBar1.Value = 50;
                progressLabel.Text = "File recieved, obtaining decryption keys";
                timer2.Start();
            });
        }

        private void MyResource_FileRecieved(object sender, FileSend file, NotifyArgs callbackArgs)
        {
            if (this.fileName != file.fileName) //TODO: email!! or guid!
            {
                return;
            }
            callbackArgs.hasSet = true;
            stopped = true;

            //ShowBalloonTip(5000, "File Recieved", fileName, ToolTipIcon.Info);
        }

        private void ReceiveDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClientRestApi.FileRecieved -= MyResource_FileRecieved;
            ClientRestApi.FileRecievedAndRespSent -= MyResource_FileRecievedAndRespSent;
            ClientRestApi.KeyRecieved -= ClientRestApi_KeyRecieved;
            this.Dispose();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) //TODO: does this have to be separate
        {
            var client = new RESTClient("http://" + ip);
            var req = new RESTRequest("/start/");
            JObject data = new JObject { { "fileName", fileName }, { "email", SettingsWrapper.Instance.Email }, {"ttp", SettingsWrapper.Instance.TTP}, {"guid", guid} };
            req.Method = Grapevine.HttpMethod.POST;
            req.ContentType = Grapevine.ContentType.JSON;
            req.Payload = data.ToString();
            var response = client.Execute(req);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Stop();
            stopped = false;
            //try and decrypt or close and display error
            var key = dict.Peek();
            var str = File.ReadAllText(localFile.FullName);
            this.progressBar1.Value = 90;
            this.progressLabel.Text = "Decrypting";

            var decrypted = Security.Aes.Decrypt(str, key, this.iv,complexity);

            File.WriteAllText(localFile.FullName,decrypted);

            this.progressBar1.Value = 100; //TODO: another thread

            saveFileDialog1.ShowDialog();

            //MessageBox.Show(decrypted);

            //MessageBox.Show(localFile.FullName);
            Close();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            var senderDialog = (SaveFileDialog) sender;
            File.Copy(localFile.FullName,senderDialog.FileName,true);
        }
    }
}
