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

namespace FEClient.Forms
{
    public partial class ReceiveDialog : Form
    {
        private readonly int _complexity;
        private readonly string _fileName;
        private readonly string _guid;
        private readonly string _ip;

        private readonly FileInfo _localFile = new FileInfo(Path.GetTempFileName());

        private volatile Stack<string> _dict = new Stack<string>(); //TODO: holds I
        private volatile string _iv;
        private bool _recievingCodes;
        private string _remoteKey;

        public ReceiveDialog(NotifyRequest startObj)
        {
            InitializeComponent();
            progressLabel.Text += _fileName;
            _ip = startObj.Ip;
            _fileName = startObj.FileName;
            _guid = startObj.Guid;
            decryptTimer.Interval = startObj.Timeout;
            _complexity = startObj.Complexity;
            ClientRestApi.FileRecieved += MyResource_FileRecieved;
            ClientRestApi.FileRecievedAndRespSent += MyResource_FileRecievedAndRespSent;
            ClientRestApi.KeyRecieved += ClientRestApi_KeyRecieved;
            ClientRestApi.Finish += ClientRestApi_Finish;
            saveFileDialog.FileName = _fileName;
            saveFileDialog.DefaultExt = new FileInfo(_fileName).Extension;
            sendStartRequestWorker.RunWorkerAsync();
        }

        private void ClientRestApi_Finish(object sender, string guid, NotifyArgs callbackArgs)
        {
            if (_guid == guid && _recievingCodes)
            {
                callbackArgs.HasSet = true;
                Invoke((MethodInvoker) delegate {timer2_Tick(this, null); });
            }
        }

        private void ClientRestApi_KeyRecieved(object sender, KeyArgs key, NotifyArgs callbackArgs)
        {
            if (_guid != key.Guid || !_recievingCodes)
            {
                return;
            }
            _dict.Push(key.Key);
            callbackArgs.HasSet = true;

            Invoke((MethodInvoker) delegate
            {
                decryptTimer.Stop();
                decryptTimer.Start();
            });
        }

        private void MyResource_FileRecievedAndRespSent(object sender, FileSend file)
        {
            if (_guid != file.Guid) //TODO: and filenames
                return;

            using (var sw = new StreamWriter(_localFile.OpenWrite())) //TODO: on another thread
            {
                sw.Write(file.Data); //TODO: WHY?
            }
            _iv = file.Iv;
            Invoke((MethodInvoker) delegate //TODO; does this happen AFTER keys start coming?
            {
                progressBar1.Style = ProgressBarStyle.Continuous;
                progressBar1.Value = 33;
                progressLabel.Text = "File recieved, obtaining decryption keys";
                decryptTimer.Start();
            });
        }

        private void MyResource_FileRecieved(object sender, FileSend file, NotifyArgs callbackArgs)
        {
            if (_guid != file.Guid)
            {
                return;
            }
            if (!Rsa.VerifySignature(file.Data, file.Signature, _remoteKey))
            {
                MessageBox.Show("Signature verification failed, transfer terminated", "Failed", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Close();
                return;
            }

            callbackArgs.HasSet = true;
            _recievingCodes = true;

            //ShowBalloonTip(5000, "File Recieved", fileName, ToolTipIcon.Info);
        }

        private void ReceiveDialog_FormClosed(object sender, FormClosedEventArgs e) //TODO: delink before close
        {
            ClientRestApi.FileRecieved -= MyResource_FileRecieved;
            ClientRestApi.FileRecievedAndRespSent -= MyResource_FileRecievedAndRespSent;
            ClientRestApi.KeyRecieved -= ClientRestApi_KeyRecieved;
            ClientRestApi.Finish -= ClientRestApi_Finish;
            Dispose();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) //TODO: does this have to be separate
        {
            if (Common.GetSSHKey(_ip, out _remoteKey))
            {
                Close();
                return;
            }


            var client = new RESTClient("http://" + _ip);
            var req = new RESTRequest("/start/");
            var data = new JObject {{"fileName", _fileName}, {"email", SettingsWrapper.Email}, {"guid", _guid}};
            req.Method = HttpMethod.POST;
            req.ContentType = ContentType.JSON;
            req.Payload = data.ToString();
            var response = client.Execute(req); //TODO: e.response?
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            decryptTimer.Stop();
            _recievingCodes = false;
            //try and decrypt or close and display error

            decryptBackgroundWorker.RunWorkerAsync();
            progressBar1.Value = 67;
            progressLabel.Text = "Decrypting";
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            var senderDialog = (SaveFileDialog) sender;
            File.Copy(_localFile.FullName, senderDialog.FileName, true);
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            var key = _dict.Peek(); //TODO: 'System.InvalidOperationException' STACK EMPTY
            var str = File.ReadAllText(_localFile.FullName);

            var decrypted = Aes.Decrypt(str, key, _iv, _complexity); //TODO: try catch

            File.WriteAllBytes(_localFile.FullName, decrypted);
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Value = 100;

            DialogResult result;
            do
            {
                result = saveFileDialog.ShowDialog();
                if (result != DialogResult.OK)
                {
                    var msgResult = MessageBox.Show("You are not saving the file, would you like to retry?", "Not saved",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (msgResult == DialogResult.Cancel)
                        break;
                }
            } while (result != DialogResult.OK);

            //MessageBox.Show(decrypted);

            //MessageBox.Show(localFile.FullName);
            Close();
        }
    }
}