using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using FEClient.API;
using FEClient.Security;
using Grapevine;
using Grapevine.Client;
using Newtonsoft.Json.Linq;

namespace FEClient
{
    public partial class ReceiveDialog : Form
    {
        private string _fileName;
        private string _ip;
        private string _guid;
        private FileInfo _localFile = new FileInfo(Path.GetTempFileName()); //TODO: why not just hold in memory?
        private string _iv;
        private Stack<string> _dict = new Stack<string>(); //TODO: holds I
        private bool _stopped;
        private int _complexity;
        public ReceiveDialog(NotifyRequest startObj)
        {
            InitializeComponent();
            progressLabel.Text += _fileName;
            _ip = startObj.Ip;
            _fileName = startObj.FileName;
            _guid = startObj.Guid;
            timer2.Interval = startObj.Timeout;
            _complexity = startObj.Complexity;
            ClientRestApi.FileRecieved += MyResource_FileRecieved;
            ClientRestApi.FileRecievedAndRespSent += MyResource_FileRecievedAndRespSent;
            ClientRestApi.KeyRecieved += ClientRestApi_KeyRecieved;
            saveFileDialog1.FileName = _fileName;
            saveFileDialog1.DefaultExt = new FileInfo(_fileName).Extension;
            backgroundWorker1.RunWorkerAsync();
        }

        void ClientRestApi_KeyRecieved(object sender, KeyArgs key, NotifyArgs callbackArgs)
        {
            if (_guid != key.Guid || !_stopped)
            {
                return;
            }
            _dict.Push(key.Key);
            callbackArgs.HasSet = true;

            Invoke((MethodInvoker) delegate
            {
                timer2.Stop();
                timer2.Start();
            });
        }

        void MyResource_FileRecievedAndRespSent(object sender, FileSend file)
        {
            if (_guid != file.Guid) //TODO: and filenames
                return;

            using (StreamWriter sw = new StreamWriter(_localFile.OpenWrite())) //TODO: on another thread
            {
                sw.Write(file.Data); //TODO: WHY?
            }
            _iv = file.Iv;
            Invoke((MethodInvoker)delegate //TODO; does this happen AFTER keys start coming?
            {
                progressBar1.Style = ProgressBarStyle.Continuous;
                progressBar1.Value = 33;
                progressLabel.Text = "File recieved, obtaining decryption keys";
                timer2.Start();
            });
        }

        private void MyResource_FileRecieved(object sender, FileSend file, NotifyArgs callbackArgs)
        {
            if (_fileName != file.FileName) //TODO: email!! or guid!
            {
                return;
            }
            callbackArgs.HasSet = true;
            _stopped = true;

            //ShowBalloonTip(5000, "File Recieved", fileName, ToolTipIcon.Info);
        }

        private void ReceiveDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClientRestApi.FileRecieved -= MyResource_FileRecieved;
            ClientRestApi.FileRecievedAndRespSent -= MyResource_FileRecievedAndRespSent;
            ClientRestApi.KeyRecieved -= ClientRestApi_KeyRecieved;
            Dispose();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) //TODO: does this have to be separate
        {
            var client = new RESTClient("http://" + _ip);
            var req = new RESTRequest("/start/");
            JObject data = new JObject { { "fileName", _fileName }, { "email", SettingsWrapper.Email }, {"guid", _guid} };
            req.Method = HttpMethod.POST;
            req.ContentType = ContentType.JSON;
            req.Payload = data.ToString();
            var response = client.Execute(req);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Stop();
            _stopped = false;
            //try and decrypt or close and display error

            backgroundWorker2.RunWorkerAsync();
            progressBar1.Value = 67;
            progressLabel.Text = "Decrypting";




        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            var senderDialog = (SaveFileDialog) sender;
            File.Copy(_localFile.FullName,senderDialog.FileName,true);
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            var key = _dict.Peek();
            var str = File.ReadAllText(_localFile.FullName);

            var decrypted = Aes.Decrypt(str, key, _iv, _complexity); //TODO: try catch

            File.WriteAllText(_localFile.FullName, decrypted);
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Value = 100;

            saveFileDialog1.ShowDialog(); //TODO: save again??

            //MessageBox.Show(decrypted);

            //MessageBox.Show(localFile.FullName);
            Close();
        }
    }
}
