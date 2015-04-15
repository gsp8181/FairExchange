using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
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

        private readonly FileInfo _localFile;

        private readonly FileStream _log; 
        private readonly StreamWriter _logWriter;

        private volatile Stack<string> _dict = new Stack<string>(); //TODO: holds I
        private volatile string _iv;
        private bool _recievingCodes;
        private string _remoteKey;

        public ReceiveDialog(NotifyRequestEventArgs startObj)
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

            var logPath = (Application.UserAppDataPath + @"\logs\received\");
            new DirectoryInfo(logPath).Create();

            _localFile = new FileInfo(Path.GetTempFileName());

                var logFile = new FileInfo(logPath + DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss") + ".log");


            _log = logFile.OpenWrite();
            _logWriter = new StreamWriter(_log);

            _logWriter.WriteLine("Log started at " + DateTime.Today.ToString("yyyy:MM:dd:HH:mm:sszzz"));
            _logWriter.WriteLine("{0} at {1}", startObj.Email, _ip);
            _logWriter.WriteLine("Sending: {0} ({1})",_fileName,_guid);
            _logWriter.WriteLine("Timeout: " + startObj.Timeout);
            _logWriter.WriteLine("Complexity: " + _complexity);
            _logWriter.WriteLine("Local Public Key");
            _logWriter.WriteLine(Rsa.PublicKey);
            

            saveFileDialog.FileName = _fileName;
            saveFileDialog.DefaultExt = new FileInfo(_fileName).Extension;
            sendStartRequestWorker.RunWorkerAsync();
        }

        private void ClientRestApi_Finish(object sender, FinishEventArgs e)
        {
            if (_guid != e.Guid || !_recievingCodes) return;
            _logWriter.WriteLine("Recieved FINISH packet");
            e.HasSet = true;
            Invoke((MethodInvoker) Decrypt);
        }

        private void ClientRestApi_KeyRecieved(object sender, KeyReceivedEventArgs e)
        {
            if (_guid != e.Guid || !_recievingCodes)  //TODO: and IP == ip for sig recieved
            {
                return;
            }
            _logWriter.WriteLineAsync("Received Key: " + e.Key);
            _dict.Push(e.Key);
            e.HasSet = true;

            Invoke((MethodInvoker) delegate
            {
                decryptTimer.Stop();
                decryptTimer.Start();
            });
        }

        private void MyResource_FileRecievedAndRespSent(object sender, FileSendEventArgs file)
        {
            if (_guid != file.Guid) //TODO: and filenames
                return;

            using (var sw = new StreamWriter(_localFile.OpenWrite())) //TODO: on another thread
            {
                sw.Write(file.Data); //TODO: WHY?
            }
            _iv = file.Iv;
            _logWriter.WriteLine("Received encrypted file and saved at " + _localFile);
            _logWriter.WriteLine("Received IV: " + _iv);
            _logWriter.WriteLine("Starting Key Receive");
            
            Invoke((MethodInvoker) delegate //TODO; does this happen AFTER keys start coming?
            {
                progressBar1.Style = ProgressBarStyle.Continuous;
                progressBar1.Value = 33;
                progressLabel.Text = "File recieved, obtaining decryption keys";
                decryptTimer.Start();
            });
        }

        private void MyResource_FileRecieved(object sender, FileSendEventArgs file)
        {
            if (_guid != file.Guid)
            {
                return;
            }
            var hashStr = Sha1.HashString(file.Data);
            _logWriter.WriteLine("Received Data, SHA1 Hash: " + hashStr); //TODO: make new and truncate data?
            _logWriter.WriteLine("Recieved Signature: " + ""); //TODO: NYI
            if (!Rsa.VerifySignature(file.Data, file.Signature, _remoteKey))
            {
                _logWriter.WriteLine("Signature verification failed, transfer terminated");
                _logWriter.WriteLine("Offending signature: " + file.Signature);
                MessageBox.Show("Signature verification failed, transfer terminated", "Failed", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Close();
                return;
            }

            file.HasSet = true;
            _recievingCodes = true;

            //ShowBalloonTip(5000, "File Recieved", fileName, ToolTipIcon.Info);
        }

        private void ReceiveDialog_FormClosed(object sender, FormClosedEventArgs e) //TODO: delink before close, should dispose be done instead?
        {
            ClientRestApi.FileRecieved -= MyResource_FileRecieved;
            ClientRestApi.FileRecievedAndRespSent -= MyResource_FileRecievedAndRespSent;
            ClientRestApi.KeyRecieved -= ClientRestApi_KeyRecieved;
            ClientRestApi.Finish -= ClientRestApi_Finish;

            //_logWriter.Close();
            //_log.Close();

            Dispose();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) //TODO: does this have to be separate
        {
            if (!Common.GetSshKey(_ip, out _remoteKey))
            {
                _logWriter.WriteLine("Remote public key not trusted, terminated");
                _logWriter.WriteLine("Remote Public Key");
                _logWriter.WriteLine(_remoteKey);
                Close();
                return;
            }

            _logWriter.WriteLine("Remote Public Key");
            _logWriter.WriteLine(_remoteKey);


            var client = new RESTClient("http://" + _ip);
            var data = new JObject { { "fileName", _fileName }, { "email", SettingsWrapper.Email }, { "guid", _guid } };

            var encData = Rsa.EncryptData(data.ToString(), _remoteKey, 0);

            var req = new RESTRequest("/start/", HttpMethod.POST, ContentType.JSON) {Payload = encData.ToString()};
            var response = client.Execute(req); //TODO: e.response?
            if (response.StatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show("Attempt to respond to notification request returned error", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            _logWriter.WriteLine("Timeout expired, using last received key");
            Decrypt();
        }

        private void Decrypt()
        {
            decryptTimer.Stop();
            _recievingCodes = false;
            //try and decrypt or close and display error

            _logWriter.Write("Received {0} total keys",_dict.Count);

            decryptBackgroundWorker.RunWorkerAsync();
            progressBar1.Value = 67;
            progressLabel.Text = "Decrypting";
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            var senderDialog = (SaveFileDialog) sender;
            File.Copy(_localFile.FullName, senderDialog.FileName, true);
            _logWriter.WriteLine("Saved as " + senderDialog.FileName);
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            var key = _dict.Peek(); //TODO: 'System.InvalidOperationException' STACK EMPTY
            var str = File.ReadAllText(_localFile.FullName);

            _logWriter.WriteLine("Attempting to decrypt using latest key");
            _logWriter.WriteLine(key);

            var decrypted = Aes.Decrypt(str, key, _iv, _complexity); //TODO: try catch

            File.WriteAllBytes(_localFile.FullName, decrypted);
            _logWriter.WriteLine("Decryption Successful");
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