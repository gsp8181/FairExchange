using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using FEClient.API;
using FEClient.API.Events;
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
        private volatile Stack<string> _dict = new Stack<string>();
        private volatile string _iv;
        private bool _recievingCodes;
        private string _remoteKey;
        private bool _terminated;
        private bool receiveTerminated;
        private string newName;
#if CHEAT_RANDOM
        private int count;
        private readonly int cheatat;
#endif
#if CHEAT_HOLDANDDECRYPT || CHEAT_HOLDANDDECRYPT_SMART
        private readonly int cheatTimeout;
        private Thread thread; //TODO: really here?
        private volatile bool completed;
        private string tempStore;
#endif


        public ReceiveDialog(NotifyRequestEventArgs startObj)
        {
            if (startObj == null)
                throw new ArgumentNullException("startObj");
            InitializeComponent();
            progressLabel.Text += _fileName;
            _ip = startObj.IP;
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
            _logWriter.WriteLine("Sending: {0} ({1})", _fileName, _guid);
            _logWriter.WriteLine("Timeout: " + startObj.Timeout);
            _logWriter.WriteLine("Complexity: " + _complexity);
            _logWriter.WriteLine("Local Public Key");
            _logWriter.WriteLine(Rsa.PublicKey);

#if CHEAT_RANDOM
            _logWriter.WriteLine("Also attempting to cheat using RANDOM");
            cheatat = NotMyCode.RandomNumber.Value(500, 1500);
            _logWriter.WriteLine("Cheating at " + cheatat);
#endif

#if CHEAT_HOLDANDDECRYPT_SMART
            _logWriter.WriteLine("Also attempting to cheat using HOLDANDDECRYPT");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Aes.Encrypt(Encoding.UTF8.GetBytes(Rsa.PublicKey), 1000);
            stopwatch.Stop();
            var time = (int)stopwatch.ElapsedMilliseconds;
            time = time*(startObj.Complexity/1000)*3;
            _logWriter.WriteLine("Estimated decrypt time: " + time + "ms");
            if (time < startObj.Timeout)
            {
                _logWriter.WriteLine("Hold and Decrypt cheat is using smart timeout");
                cheatTimeout = time;
            }
            else
            {
                cheatTimeout = -1;
                _logWriter.WriteLine("Smart timeout detection would fail"); //TODO: dialog box
            }
#elif CHEAT_HOLDANDDECRYPT
            _logWriter.WriteLine("Also attempting to cheat using HOLDANDDECRYPT");
            _logWriter.WriteLine("Attempting decrypt up to timeout of " + cheatTimeout);
            cheatTimeout = (startObj.Timeout/10*9);
#endif

            saveFileDialog.FileName = _fileName;
            saveFileDialog.DefaultExt = new FileInfo(_fileName).Extension;
            sendStartRequestWorker.RunWorkerAsync();
        }

        private void ClientRestApi_Finish(object sender, FinishEventArgs e)
        {
            if (_guid != e.Guid || !_recievingCodes/* || e.IP != _ip*/) //TODO: FIX CHECK SIG, strip or check ports
                return;
            _logWriter.WriteLine("Recieved FINISH packet");
            e.HasSet = true;
            Invoke((MethodInvoker) Decrypt);
        }

        private void ClientRestApi_KeyRecieved(object sender, KeyReceivedEventArgs e)
        {
            if (_guid != e.Guid || !_recievingCodes/* || e.IP != _ip*/) //TODO: fix check sig, strip or checo ports
            {
                return;
            }
            _logWriter.WriteLineAsync("Received Key: " + e.Key);
            _dict.Push(e.Key);

#if CHEAT_RANDOM
            if(count++ == cheatat)
            {
                _logWriter.WriteLineAsync("Attempting to cheat using last given code");
                Invoke ((MethodInvoker) delegate
                {
                    progressLabel.Text = "Attempting to cheat!";
                    Decrypt();
                });
                return;
            }
#endif

#if CHEAT_HOLDANDDECRYPT || CHEAT_HOLDANDDECRYPT_SMART
            thread = new Thread(new ThreadStart(CheatDecrypt)); //TODO: params?
            thread.Start();
            Thread.Sleep(cheatTimeout);
            if (!completed)
            {
                thread.Abort();
            }
            else
            {
                _logWriter.WriteLine("Key has been successfully fraudulently obtained");
                _logWriter.WriteLine("Decryption is in progress on cheat thread using last sent key");
                Terminate();
                thread.Join();
                _logWriter.WriteLine("Decryption Successful");
                Invoke((MethodInvoker) delegate{ saveFileDialog.ShowDialog(); });
                return;
            }
#endif

            e.HasSet = true;

            Invoke((MethodInvoker) delegate
            {
                decryptTimer.Stop();
                decryptTimer.Start();
            });
        }

#if CHEAT_HOLDANDDECRYPT || CHEAT_HOLDANDDECRYPT_SMART
        private void CheatDecrypt()
        {
            var key = _dict.Peek();
            try
            {
                var decrypted = Aes.Decrypt(tempStore, key, _iv, _complexity);
                completed = true;
                //tempStore = decrypted;


                newName = Path.GetTempFileName();
                _logWriter.WriteLine("Saving cheat decrypted file to " + newName);
                File.WriteAllBytes(newName, decrypted);
            }
            catch (Exception) { }
        }
#endif

        private void Terminate()
        {
            TermFileReceiveEvents();
            if (_terminated) 
                return;

            Invoke((MethodInvoker) delegate { decryptTimer.Stop(); });
            ClientRestApi.KeyRecieved -= ClientRestApi_KeyRecieved;
            ClientRestApi.Finish -= ClientRestApi_Finish;
            decryptBackgroundWorker.CancelAsync();
            sendStartRequestWorker.CancelAsync();


            _terminated = true;
        }

        private void TermFileReceiveEvents()
        {
            if (receiveTerminated) 
                return;

            ClientRestApi.FileRecieved -= MyResource_FileRecieved;
            ClientRestApi.FileRecievedAndRespSent -= MyResource_FileRecievedAndRespSent;
            receiveTerminated = true;
        }

        private void MyResource_FileRecievedAndRespSent(object sender, FileSendEventArgs file)
        {
            if (_guid != file.Guid || file.FileName != _fileName)
                return;

            using (var sw = new StreamWriter(_localFile.OpenWrite()))
            {
                sw.Write(file.Data);
            }
            _iv = file.IV;
            _logWriter.WriteLine("Received encrypted file and saved at " + _localFile);
            _logWriter.WriteLine("Received IV: " + _iv);
            _logWriter.WriteLine("Starting Key Receive");

#if CHEAT_HOLDANDDECRYPT || CHEAT_HOLDANDDECRYPT_SMART
            tempStore = file.Data;
#endif

            Invoke((MethodInvoker) delegate 
            {
                progressBar1.Style = ProgressBarStyle.Continuous;
                progressBar1.Value = 33;
                progressLabel.Text = "File recieved, obtaining decryption keys";
                decryptTimer.Start();
            });
            TermFileReceiveEvents();
        }

        private void MyResource_FileRecieved(object sender, FileSendEventArgs file)
        {
            if (_guid != file.Guid)
            {
                return;
            }
            var hashStr = Sha1.HashString(file.Data);
            _logWriter.WriteLine("Received Data, SHA1 Hash: " + hashStr);
            _logWriter.WriteLine("Recieved Signature: " + file.Signature); 
            if (!Rsa.VerifySignature(file.Data, file.Signature, _remoteKey))
            {
                _logWriter.WriteLine("Signature verification failed, transfer terminated");
                _logWriter.WriteLine("Offending signature: " + file.Signature);
                Terminate();
                MessageBox.Show("Signature verification failed, transfer terminated", "Failed", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Invoke((MethodInvoker) Close);
                return;
            }

            file.HasSet = true;
            _recievingCodes = true;

            //ShowBalloonTip(5000, "File Recieved", fileName, ToolTipIcon.Info);
        }

        private void ReceiveDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            Terminate();
            Dispose();
        }

        private void startSendBackgroundWorker_DoWork(object sender, DoWorkEventArgs e) 
        {
            var key = Common.GetSshKey(_ip);
            if (key.IsSet == false)
            {
                if (string.IsNullOrEmpty(key.Email))
                {
                    _logWriter.WriteLine("Could not contact remote server, terminated");
                    Terminate();
                    Invoke((MethodInvoker)Close);
                    return;
                }
                _logWriter.WriteLine("Remote public key not trusted, terminated");
                _logWriter.WriteLine("Email: " + key.Email);
                _logWriter.WriteLine("Remote Public Key");
                _logWriter.WriteLine(key.RemoteKey);
                Terminate();
                Invoke((MethodInvoker)Close);
                return;
            }
            _remoteKey = key.RemoteKey;

            _logWriter.WriteLine("Email: " + key.Email);
            _logWriter.WriteLine("Remote Public Key");
            _logWriter.WriteLine(_remoteKey);


            var client = new RESTClient("http://" + _ip);
            var data = new JObject {{"fileName", _fileName}, {"email", SettingsWrapper.Email}, {"guid", _guid}};

            var encData = Rsa.EncryptData(data.ToString(), _remoteKey, 0);

            var req = new RESTRequest("/start/", HttpMethod.POST, ContentType.JSON) {Payload = encData.ToString()};
            var response = client.Execute(req); 
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Terminate();
                MessageBox.Show("Attempt to respond to notification request returned error", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Invoke((MethodInvoker)Close);
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

            _logWriter.Write("Received {0} total keys", _dict.Count);

            decryptBackgroundWorker.RunWorkerAsync();
            progressBar1.Value = 67;
            progressLabel.Text = "Decrypting";
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            var senderDialog = (SaveFileDialog) sender;
            File.Copy(newName, senderDialog.FileName, true);
            _logWriter.WriteLine("Saved as " + senderDialog.FileName);
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = false;
            if (_dict.Count < 1)
            {
                _logWriter.WriteLine("No keys available, failed");
                Terminate();
                MessageBox.Show("No keys were received, failed", "No keys", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Invoke((MethodInvoker)Close);
                return;
            }
            var key = _dict.Peek();
            var str = File.ReadAllText(_localFile.FullName);

            _logWriter.WriteLine("Attempting to decrypt using latest key");
            _logWriter.WriteLine(key);
            byte[] decrypted;

            try
            {
                decrypted = Aes.Decrypt(str, key, _iv, _complexity);
            }
            catch (Exception ex)
            {
                _logWriter.WriteLine("Decryption failed");
                _logWriter.WriteLine(ex.Message);
                _logWriter.WriteLine("Last key attempted: " + key);
                Terminate();
                MessageBox.Show("Decryption failed, key is invalid\nCheck the logs for more details",
                    "Decryption Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Invoke((MethodInvoker)Close);
                return;
            }

            newName = Path.GetTempFileName();
            _logWriter.WriteLine("Saving decrypted file to " + newName);
            File.WriteAllBytes(newName, decrypted);
            e.Result = true;
            _logWriter.WriteLine("Decryption Successful");
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Value = 100;
            Terminate();
            if ((bool)e.Result == false)
                return;

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