using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Windows.Forms;
using FEClient.API;
using FEClient.API.Events;
using FEClient.Security;
using Grapevine;
using Grapevine.Client;
using Newtonsoft.Json.Linq;
using Aes = FEClient.Security.Aes;

namespace FEClient.Forms
{
    public partial class SendDialog : Form
    {
        private readonly int _amount;
        private readonly int _complexity;
        private readonly Queue<string> _fakeKeys = new Queue<string>();
        private readonly FileInfo _file;
        private readonly string _guid;
        private readonly string _ip;
        private readonly FileStream _log;
        private readonly StreamWriter _logWriter;
        private readonly int _timeout;
        private AesData _aesData;
        private AesKeys _key;
        private string _remoteKey;
        private bool _terminated;
        private bool _startRevoked;
        private bool respRevoked;

        public SendDialog(string ip, string fileName, int rounds, int complexity, int timeout)
        {
            InitializeComponent();
            ClientRestApi.StartTransmission += MyResource_StartTransmission;
            ClientRestApi.StartTransmissionAndRespSent += MyResource_StartTransmissionAndRespSent;
            _ip = ip;
            _file = new FileInfo(fileName);

            _guid = Guid.NewGuid().ToString();
            _amount = rounds;
            _complexity = complexity;
            _timeout = timeout;

            var logPath = (Application.UserAppDataPath + @"\logs\sent\");
            new DirectoryInfo(logPath).Create();

            var logFile = new FileInfo(logPath + DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss") + ".log");


            _log = logFile.OpenWrite();
            _logWriter = new StreamWriter(_log);

            _logWriter.WriteLine("Log started at " + DateTime.Today.ToString("yyyy:MM:dd:HH:mm:sszzz"));
            _logWriter.WriteLine("Contacting {0}", _ip);
            _logWriter.WriteLine("Sending: {0} ({1})", fileName, _guid);
            _logWriter.WriteLine("Timeout: " + _timeout);
            _logWriter.WriteLine("Complexity: " + _complexity);
            _logWriter.WriteLine("Rounds " + _amount);
            _logWriter.WriteLine("Local Public Key");
            _logWriter.WriteLine(Rsa.PublicKey);
        }

        private void MyResource_StartTransmissionAndRespSent(object sender, StartTransmissionEventArgs vars)
        {
            if (vars.Guid != _guid || vars.FileName != _file.Name/* || vars.IP != _ip*/) //TODO:STRIP ++ CHECK PORTS
                return;
            Invoke((MethodInvoker) SendFile); 
            RevokeResp();
        }

        private void MyResource_StartTransmission(object sender, StartTransmissionEventArgs args)
        {
            if (args.FileName != _file.Name || args.Guid != _guid/* || args.IP != _ip*/) // TODO: strip or check ports
                return;
            args.HasSet = true;
            Invoke((MethodInvoker) delegate { timeoutTimer.Stop(); });
            RevokeStart();
        }

        private void SendDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            Terminate();
            Dispose();
            //file.Close();
        }

        private void Terminate()
        {
            RevokeStart();
            RevokeResp();
            if (!_terminated)
            {
                Invoke((MethodInvoker) delegate { timeoutTimer.Stop(); });

                 sendKeysBackgroundWorker.CancelAsync();
                _terminated = true;
            }
        }

        private void RevokeStart()
        {
            if(!_startRevoked)
            { 
                ClientRestApi.StartTransmission -= MyResource_StartTransmission;
                _startRevoked = true;
            }
            
        }

        private void RevokeResp()
        {
            if(!respRevoked)
            { 
                ClientRestApi.StartTransmissionAndRespSent -= MyResource_StartTransmissionAndRespSent;
                respRevoked = true;
            }
        }

        private void timeoutTimer_Tick(object sender, EventArgs e)
        {
            _logWriter.WriteLineAsync("Timeout expired, terminating");
            progressBar.Style = ProgressBarStyle.Continuous;
            Terminate();
            MessageBox.Show("Remote user did not respond in time", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Invoke((MethodInvoker)Close);
        }

        private void SendFile()
        {
            // Updates progress label to show file is sending
            progressLabel.Text = "Sending " + _file.Name;
            progressBar.Style = ProgressBarStyle.Continuous;

            // Creates a new POST request to the remote client
            var client = new RESTClient("http://" + _ip);
            var req = new RESTRequest("/file/", HttpMethod.POST, ContentType.JSON);

            //Embeds the data (fig 1)
            var data = new JObject
            {
                {"fileName", _file.Name},
                {"email", SettingsWrapper.Email},
                {"guid", _guid},
                {"iv", _key.IvStr},
                {"complexity", _complexity},
                {"data", _aesData.Data}
            };
            _logWriter.WriteLine("Sending data");
            var hashStr = Sha1.HashJObject(data);
            _logWriter.WriteLine("Data Hash: " + hashStr);
            var sig = Rsa.SignStringData(data.ToString());
            _logWriter.WriteLine("Signature: " + sig);

            _logWriter.WriteLineAsync("Data: " + data);

            var toSend = new JObject
            {
                {"data", data.ToString()},
                {"signature", sig}
            };

            var encData = Rsa.EncryptData(toSend.ToString(), _remoteKey, 0);

            req.Payload = encData.ToString();
            //Sends the request
            var response = client.Execute(req);

            //If there was an error then fail and quit
            if (response.ReturnedError || !string.IsNullOrEmpty(response.Error))
            {
                _logWriter.WriteLine("Sending data resulted in an error: " + response.Error);
                progressBar.Style = ProgressBarStyle.Continuous;
                Terminate();
                MessageBox.Show("Remote server did not accept the file\n" + response.Error, "Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Invoke((MethodInvoker)Close);
                return;
            }
            var json = JObject.Parse(response.Content);
            var remoteSig = json.Value<string>("signature");

            _logWriter.WriteLine("Returned signature " + remoteSig);

            if (!Rsa.VerifySignature(sig, remoteSig, _remoteKey))
            {
                _logWriter.WriteLine("Signature verification failed, terminated");
                Terminate();
                MessageBox.Show("Signature verification failed, transfer terminated", "Failed", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Invoke((MethodInvoker)Close);
                return;
            }

            _logWriter.WriteLine("Signature verification successful");


            // Update the progress box
            progressLabel.Text = "Sending Keys";
            sendKeysBackgroundWorker.RunWorkerAsync();
        }

        private void SendDialog_Load(object sender, EventArgs e)
        {
            generateKeysBackgroundWorker.RunWorkerAsync();
        }

        private void sendKeysBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _logWriter.WriteLine("Now sending fake keys");
            var stopwatch = new Stopwatch();
            var client = new RESTClient("http://" + _ip);
            stopwatch.Start();
            for (var i = 0; i < _amount; i++)
            {
                if (e.Cancel)
                {
                    return;
                }

                var fkey = _fakeKeys.Dequeue();

                var data = new JObject {{"key", fkey}, {"guid", _guid}, {"i", i}}; 

                var encData = Rsa.EncryptData(data.ToString(), _remoteKey, 0);

                var req = new RESTRequest("/key/", HttpMethod.POST, ContentType.JSON, _timeout)
                {
                    Payload = encData.ToString()
                }; 

                var response = client.Execute(req);
                if (stopwatch.ElapsedMilliseconds > _timeout)
                {
                    _logWriter.WriteLineAsync(string.Format("Failed on key {0} through timeout, {1}ms elapsed", i,
                        stopwatch.ElapsedMilliseconds));
                    _logWriter.WriteLineAsync("Failed fake key: " + fkey);

                    MessageBox.Show("Timed out, transmission ended", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Terminate();
                    Invoke((MethodInvoker)Close);
                    return;
                }
                stopwatch.Restart();

                var sig = JObject.Parse(response.Content).Value<string>("signature");
                if (!Rsa.VerifySignature(data.ToString(), sig, _remoteKey))
                {
                    var hashStr = Sha1.HashJObject(data);
                    _logWriter.WriteLine("Failed on fake key {0} as signature verification failed", i);
                    _logWriter.WriteLine("Actual data: " + data);
                    _logWriter.WriteLine("Data hash: " + hashStr);
                    _logWriter.WriteLine("Provided signature " + sig);
                    Terminate();
                    MessageBox.Show("Error, signature verification failed", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    Invoke((MethodInvoker)Close);
                    return;
                    //this.Close();
                }

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    _logWriter.WriteLine("Error through malformed HTTP Code on fake key {0}; {1} with error {2}", i,
                        fkey, response.Error);
                    Terminate();
                    MessageBox.Show("Error, remote server returned error\n" + response.Error, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Invoke((MethodInvoker)Close);
                    return;
                }


                sendKeysBackgroundWorker.ReportProgress((int) (((double) i/_amount)*100));
            }

            var realData = new JObject {{"key", _key.KeyStr}, {"guid", _guid}, {"i", _amount}};
            var encRealData = Rsa.EncryptData(realData.ToString(), _remoteKey, 0);

            var realReq = new RESTRequest("/key/", HttpMethod.POST, ContentType.JSON, _timeout)
            {
                Payload = encRealData.ToString()
            };

            if (e.Cancel)
            {
                e.Result = false;
                return;
            }

            var realResponse = client.Execute(realReq);

            _logWriter.WriteLine("Sent real key {0}", _key.KeyStr);
            _logWriter.WriteLine(realData);

            if (realResponse.StatusCode != HttpStatusCode.OK)
            {
                _logWriter.WriteLine("ERROR: Sent REAL key and error was returned: {0}", realResponse.Error);
                Terminate();
                MessageBox.Show("Error, sent real key and error was returned\n" + realResponse.Error, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Invoke((MethodInvoker)Close);
                return;
            }

            var realSig = JObject.Parse(realResponse.Content).Value<string>("signature");
            var hashStr2 = Sha1.HashJObject(realData);
            _logWriter.WriteLine("Hash of data: " + hashStr2);
            _logWriter.WriteLine("Given signature " + realSig);

            if (!Rsa.VerifySignature(realData.ToString(), realSig, _remoteKey))
            {
                _logWriter.WriteLine("ERROR, sent REAL key and signature verification failed, terminating");
                Terminate();
                MessageBox.Show("Error, signature verification failed", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Invoke((MethodInvoker)Close);
                return;
            }

            _logWriter.WriteLine("Signature verified successfully");
            _logWriter.WriteLine("Finished, sending finish token!");

            sendKeysBackgroundWorker.ReportProgress(100);

            Invoke((MethodInvoker) delegate { this.progressLabel.Text = "Sending finish token"; });

            var finData = new JObject {{"guid", _guid}};
            var encryptedFinData = Rsa.EncryptData(finData.ToString(), _remoteKey, 0);

            var finReq = new RESTRequest("/finish/", HttpMethod.POST, ContentType.JSON, _timeout)
            {
                Payload = encryptedFinData.ToString()
            };
            client.Execute(finReq);
        }

        private void sendKeysBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void generateKeysBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Opens and reads the file to the end
            bool retry;
            byte[] text = {};
            do
            {
                retry = true;
                try
                {
                    text = File.ReadAllBytes(_file.FullName);
                    retry = false;
                }
                catch (Exception exception)
                {
                    if (
                        MessageBox.Show(exception.Message + "\nWould you like to retry?",
                            "Failed to read " + _file.FullName, MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    {
                        Invoke((MethodInvoker)Close);
                        return;
                    }
                }
            } while (retry);

            _logWriter.WriteLine("Encrypting file");

            //Encrypts the data
            _aesData = Aes.Encrypt(text, _complexity);
            //Stores the encryption key as a global variable
            _key = _aesData.Key;

            _logWriter.WriteLine("Using key: " + _aesData.Key.KeyStr);
            _logWriter.WriteLine("IV: " + _aesData.Key.IvStr);

            if (e.Cancel)
            {
                e.Result = false;
                return;
            }

            int bytes;
            using (var aesCsp = new AesCryptoServiceProvider())
            {
                bytes = aesCsp.KeySize/8;
            }
            _logWriter.WriteLine("Key is {0} bytes long so generating {1} fake keys of {2} byte length", bytes, _amount,
                bytes);
            using (var rng = new RNGCryptoServiceProvider())
            {
                for (var i = 0; i < _amount; i++)
                {
                    var randBytes = new byte[bytes];
                    rng.GetBytes(randBytes);
                    _fakeKeys.Enqueue(Convert.ToBase64String(randBytes));
                }
            }

            if (e.Cancel)
            {
                e.Result = false;
                return;
            }

            _logWriter.WriteLine("Contacting " + _ip);

            Invoke((MethodInvoker) delegate { progressLabel.Text = "Attempting to contact " + _ip; });
            var key = Common.GetSshKey(_ip);
            _logWriter.WriteLine("Remote Email: " + key.Email);

            if (e.Cancel)
            {
                e.Result = false;
                return;
            }

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
                _logWriter.WriteLine("Remote Public Key");
                _logWriter.WriteLine(key.RemoteKey);
                e.Result = false;
                Terminate();
                Invoke((MethodInvoker)Close);
                return;
            }
            _remoteKey = key.RemoteKey;
            _logWriter.WriteLine("Remote Public Key");
            _logWriter.WriteLine(_remoteKey);
            if (e.Cancel)
            {
                e.Result = false;
                return;
            }
            e.Result = true;
        }

        private void generateKeysBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((bool) e.Result == false)
                return;

            var client = new RESTClient("http://" + _ip);


            progressLabel.Text = "Waiting for the user to respond";

            var req = new RESTRequest("/notify/", HttpMethod.POST, ContentType.JSON, _timeout); 
            var data = new JObject
            {
                {"fileName", _file.Name},
                {"email", SettingsWrapper.Email},
                {"guid", _guid},
                {"timeout", _timeout},
                {"complexity", _complexity},
                {"port", Context.Port}
            };

            var encData = Rsa.EncryptData(data.ToString(), _remoteKey, 0);

            req.Payload = encData.ToString();
            var response = client.Execute(req);
            _logWriter.WriteLine("Sending start request");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logWriter.WriteLine("Start request failed with error, terminating");
                Terminate();
                MessageBox.Show("Start request has failed, terminating", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }
            timeoutTimer.Start();
        }

        private void sendKeysBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Terminate();
            progressLabel.Text = "Finished";
        }

        private void abortButton_Click(object sender, EventArgs e)
        {
            _logWriter.WriteLineAsync("Termination requested");
            Terminate();
        }
    }
}