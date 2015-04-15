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
        private readonly int _timeout;
        private AesData _aesData;
        private AesKeys _key;
        private string _remoteKey;
        private readonly FileStream _log;
        private readonly StreamWriter _logWriter;
        private bool _terminated;

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
            _logWriter.WriteLine("{0} at {1}", /*startObj.Email*/null, _ip); //TODO: fix email
            _logWriter.WriteLine("Sending: {0} ({1})", fileName, _guid);
            _logWriter.WriteLine("Timeout: " + _timeout);
            _logWriter.WriteLine("Complexity: " + _complexity);
            _logWriter.WriteLine("Rounds " + _amount);
            _logWriter.WriteLine("Local Public Key");
            _logWriter.WriteLine(Rsa.PublicKey);
        }

        private void MyResource_StartTransmissionAndRespSent(object sender, StartTransmissionEventArgs vars)
        {
            if (vars.Guid != _guid)
                return;
            Invoke((MethodInvoker)timer2_Tick); //TODO: maybe another timeout timer?
        }

        private void MyResource_StartTransmission(object sender, StartTransmissionEventArgs args)
        {
            if (args.FileName != _file.Name)
                return;
            args.HasSet = true;
            Invoke((MethodInvoker)delegate { timeoutTimer.Stop(); });
        }

        private void SendDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            Terminate();
            Dispose();
            //file.Close();
        }

        private void Terminate()
        {
            if (!_terminated)
            {
                Invoke((MethodInvoker) delegate
                {
                    timeoutTimer.Stop();
                });
                    ClientRestApi.StartTransmission -= MyResource_StartTransmission;
                    ClientRestApi.StartTransmissionAndRespSent -= MyResource_StartTransmissionAndRespSent; //TODO: cancel background workers
                
                
            _terminated = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _logWriter.WriteLineAsync("Timeout expired, terminating");
            progressBar.Style = ProgressBarStyle.Continuous;
            Terminate();
            MessageBox.Show("Remote user did not respond in time", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
        }

        private void timer2_Tick()
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
                {"data", _aesData.DataStr}
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
                progressBar.Style = ProgressBarStyle.Continuous; //TODO: update label
                Terminate();
                MessageBox.Show("Remote server did not accept the file\n" + response.Error, "Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }
            var json = JObject.Parse(response.Content);
            var remoteSig = json.Value<string>("signature");

            _logWriter.WriteLine("Returned signature " + remoteSig);

            if (!Rsa.VerifySignature(sig, remoteSig, _remoteKey))
            //TODO:INSTEAD This should send an abort request of some kind, make sure it doesn't lock recievedialog
            {
                _logWriter.WriteLine("Signature verification failed, terminated");
                Terminate();
                MessageBox.Show("Signature verification failed, transfer terminated", "Failed", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Close();
                return;
            }

            _logWriter.WriteLine("Signature verification successful");


            // Update the progress box
            //TODO: use async and await
            progressLabel.Text = "Sending Keys";
            sendKeysBackgroundWorker.RunWorkerAsync();
        }

        private void SendDialog_Load(object sender, EventArgs e)
        {
            generateKeysBackgroundWorker.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            _logWriter.WriteLine("Now sending fake keys");
            var stopwatch = new Stopwatch();
            var client = new RESTClient("http://" + _ip);
            stopwatch.Start();
            for (var i = 0; i < _amount; i++)
            {
                //TODO:Check cancellation
                var fkey = _fakeKeys.Dequeue();

                var data = new JObject { { "key", fkey }, { "guid", _guid }, { "i", i } }; //TODO: rsa sign?

                var encData = Rsa.EncryptData(data.ToString(), _remoteKey, 0);

                var req = new RESTRequest("/key/", HttpMethod.POST, ContentType.JSON, _timeout)
                {
                    Payload = encData.ToString()
                }; //TODO: async and await

                var response = client.Execute(req);
                if (stopwatch.ElapsedMilliseconds > _timeout)
                {
                    _logWriter.WriteLineAsync(string.Format("Failed on key {0} through timeout, {1}ms elapsed", i, stopwatch.ElapsedMilliseconds));
                    _logWriter.WriteLineAsync("Failed fake key: " + fkey);

                    MessageBox.Show("Timed out, transmission ended", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Terminate();
                    Close();
                    return;
                }
                stopwatch.Restart();

                var sig = JObject.Parse(response.Content).Value<string>("signature");
                if (!Rsa.VerifySignature(data.ToString(), sig, _remoteKey))
                //TODO: is this a performance hit converting from string every time?
                {
                    var hashStr = Sha1.HashJObject(data);
                    _logWriter.WriteLineAsync(string.Format("Failed on fake key {0} as signature verification failed", i));
                    _logWriter.WriteLineAsync("Actual data: " + data);
                    _logWriter.WriteLineAsync("Data hash: " + hashStr);
                    _logWriter.WriteLineAsync("Provided signature " + sig);
                    Terminate();
                    MessageBox.Show("Error, signature verification failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return;
                    //this.Close();
                }

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    _logWriter.WriteLine("Error through malformed HTTP Code on fake key {0}; {1} with error {2}", i, fkey, response.Error);
                    Terminate();
                    MessageBox.Show("Error, remote server returned error\n" + response.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return;
                }


                sendKeysBackgroundWorker.ReportProgress((int)(((double)i / _amount) * 100));
            }

            var realData = new JObject { { "key", _key.KeyStr }, { "guid", _guid }, { "i", _amount } }; //TODO: encrypt keys??, rsa sign?
            var encRealData = Rsa.EncryptData(realData.ToString(), _remoteKey, 0);

            var realReq = new RESTRequest("/key/", HttpMethod.POST, ContentType.JSON, _timeout)
            {
                Payload = encRealData.ToString()
            }; //TODO: split into method with above bit //TODO: async and await

            var realResponse = client.Execute(realReq);

            _logWriter.WriteLine("Sent real key {0}", _key.KeyStr);
            _logWriter.WriteLine(realData);

            if (realResponse.StatusCode != HttpStatusCode.OK) //TODO: OR IF TIMEOUT
            {
                _logWriter.WriteLine("ERROR: Sent REAL key and error was returned: {0}", realResponse.Error);
                //Invoke((MethodInvoker) delegate
                //{
                Terminate();
                MessageBox.Show("Error, sent real key and error was returned\n" + realResponse.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); //TODO: split into sendakey(key);?
                Close();
                //});
                return;
            }

            var realSig = JObject.Parse(realResponse.Content).Value<string>("signature");
            var hashStr2 = Sha1.HashJObject(realData);
            _logWriter.WriteLine("Hash of data: " + hashStr2);
            _logWriter.WriteLine("Given signature " + realSig);

            if (!Rsa.VerifySignature(realData.ToString(), realSig, _remoteKey))
            //TODO: is this a performance hit converting from string every time?
            {
                _logWriter.WriteLine("ERROR, sent REAL key and signature verification failed, terminating");
                Terminate();
                MessageBox.Show("Error, signature verification failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            _logWriter.WriteLine("Signature verified successfully");
            _logWriter.WriteLine("Finished, sending finish token!");

            sendKeysBackgroundWorker.ReportProgress(100);

            Invoke((MethodInvoker)delegate
            {
                this.progressLabel.Text = "Sending finish token";
            });

            var finData = new JObject { { "guid", _guid } };
            var encryptedFinData = Rsa.EncryptData(finData.ToString(), _remoteKey, 0);

            var finReq = new RESTRequest("/finish/", HttpMethod.POST, ContentType.JSON, _timeout)
            {
                Payload = encryptedFinData.ToString()
            };
            client.Execute(finReq);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage; //TODO: does not work
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            // Opens and reads the file to the end
            var text = File.ReadAllBytes(_file.FullName); //TODO: if not null and using!

            _logWriter.WriteLine("Encrypting file");

            //Encrypts the data
            _aesData = Aes.Encrypt(text, _complexity);
            //Stores the encryption key as a global variable
            _key = _aesData.Key;

            _logWriter.WriteLine("Using key: " + _aesData.Key.KeyStr);
            _logWriter.WriteLine("IV: " + _aesData.Key.IvStr);

            int bytes;
            using (var aesCsp = new AesCryptoServiceProvider())
            {
                bytes = aesCsp.KeySize / 8;
            }
            _logWriter.WriteLine("Key is {0} bytes long so generating {1} fake keys of {2} byte length", bytes, _amount, bytes);
            using (var rng = new RNGCryptoServiceProvider())
            {
                for (var i = 0; i < _amount; i++)
                {
                    var randBytes = new byte[bytes];
                    rng.GetBytes(randBytes);
                    _fakeKeys.Enqueue(Convert.ToBase64String(randBytes));
                }
            }

            _logWriter.WriteLine("Contacting " + _ip);

            Invoke((MethodInvoker)delegate { progressLabel.Text = "Attempting to contact " + _ip; });
            if (!Common.GetSshKey(_ip, out _remoteKey)) 
            {
                _logWriter.WriteLine("Remote public key not trusted, terminated");
                _logWriter.WriteLine("Remote Public Key");
                _logWriter.WriteLine(_remoteKey);
                e.Result = false;
                Terminate();
                Close();
                return;
            }
            _logWriter.WriteLine("Remote Public Key");
            _logWriter.WriteLine(_remoteKey);
            e.Result = true;
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((bool)e.Result == false)
                return;

            var client = new RESTClient("http://" + _ip);


            progressLabel.Text = "Waiting for the user to respond";

            var req = new RESTRequest("/notify/", HttpMethod.POST, ContentType.JSON, _timeout);//TODO: async and await
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
                MessageBox.Show("error");
                Close();
                return;
                //TODO; this needs to be better, maybe a handle error method which tries to get the error string
            }
            timeoutTimer.Start();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressLabel.Text = "Finished";
        }
    }
}