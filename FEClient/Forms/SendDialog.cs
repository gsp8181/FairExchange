using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Windows.Forms;
using FEClient.API;
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
        }

        private void MyResource_StartTransmissionAndRespSent(object sender, NotifyRequest vars)
        {
            if (vars.Guid != _guid)
                return;
            Invoke((MethodInvoker) delegate { timer2_Tick(); }); //TODO: maybe another timeout timer?
        }

        private void MyResource_StartTransmission(object sender, NotifyRequest addrSender, NotifyArgs callbackArgs)
        {
            if (addrSender.FileName != _file.Name)
                return;
            callbackArgs.HasSet = true;
            Invoke((MethodInvoker) delegate { timeoutTimer.Stop(); });
        }

        private void SendDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClientRestApi.StartTransmission -= MyResource_StartTransmission; //TODO: delink events before form.close()? maybe involve
            ClientRestApi.StartTransmissionAndRespSent -= MyResource_StartTransmissionAndRespSent;
            Dispose();
            //file.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar.Style = ProgressBarStyle.Continuous;
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
            var req = new RESTRequest("/file/")
            {
                Method = HttpMethod.POST,
                ContentType = ContentType.JSON
            };

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

            var sig = Rsa.SignStringData(data.ToString());

            var toSend = new JObject
            {
                {"data", data.ToString()},
                {"signature", sig}
            };

            req.Payload = toSend.ToString();
            //Sends the request
            var response = client.Execute(req);

            //If there was an error then fail and quit
            if (response.ReturnedError || !string.IsNullOrEmpty(response.Error))
                //TODO: accepted? TODO: better response checking for example timeout
            {
                progressBar.Style = ProgressBarStyle.Continuous; //TODO: update label
                MessageBox.Show("Remote server did not accept the file\n" + response.Error, "Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }
            var json = JObject.Parse(response.Content);
            var remoteSig = json.Value<string>("signature");

            if (!Rsa.VerifySignature(sig, remoteSig, _remoteKey))
                //TODO:INSTEAD This should send an abort request of some kind, make sure it doesn't lock recievedialog
            {
                MessageBox.Show("Signature verification failed, transfer terminated", "Failed", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Close();
                return;
            }


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
            var stopwatch = new Stopwatch();
            var client = new RESTClient("http://" + _ip);
            stopwatch.Start();
            for (var i = 0; i < _amount; i++)
            {
                //TODO:Check cancellation
                var fkey = _fakeKeys.Dequeue();

                var data = new JObject {{"key", fkey}, {"guid", _guid}, {"i", i}};

                var req = new RESTRequest("/key/");

                req.Timeout = _timeout; //TODO actually take from input AND set a timer
                req.Method = HttpMethod.POST;
                req.ContentType = ContentType.JSON; //TODO: async and await
                req.Payload = data.ToString();
                var response = client.Execute(req);
                if (stopwatch.ElapsedMilliseconds > _timeout)
                {
                    //TODO: STOP
                    MessageBox.Show("Timed out!");
                }
                stopwatch.Restart();

                var sig = JObject.Parse(response.Content).Value<string>("signature");
                if (!Rsa.VerifySignature(data.ToString(), sig, _remoteKey))
                    //TODO: is this a performance hit converting from string every time?
                {
                    MessageBox.Show("Error, signature verification failed"); //TODO: complete
                }

                if (response.StatusCode != HttpStatusCode.OK) //TODO: OR IF TIMEOUT
                {
                    Invoke((MethodInvoker) delegate { MessageBox.Show("Error"); //TODO: STOP!
                    });
                    return;
                }


                sendKeysBackgroundWorker.ReportProgress((int)(((double)i/_amount)*100));
            }

            var realData = new JObject {{"key", _key.KeyStr}, {"guid", _guid}, {"i", _amount}}; //TODO: encrypt keys??


            var realReq = new RESTRequest("/key/"); //TODO: split into method with above bit
            realReq.Timeout = _timeout;
            realReq.Method = HttpMethod.POST;
            realReq.ContentType = ContentType.JSON; //TODO: async and await
            realReq.Payload = realData.ToString();
            var realResponse = client.Execute(realReq);
            if (realResponse.StatusCode != HttpStatusCode.OK) //TODO: OR IF TIMEOUT
            {
                Invoke((MethodInvoker) delegate
                {
                    MessageBox.Show("Error"); //TODO: bigger!
                    this.Close();
                });
                return;
            }

            var realSig = JObject.Parse(realResponse.Content).Value<string>("signature");
            //TODO: save this!!! along with data!

            if (!Rsa.VerifySignature(realData.ToString(), realSig, _remoteKey))
                //TODO: is this a performance hit converting from string every time?
            {
                MessageBox.Show("Error, signature verification failed"); //TODO: complete
            }


            sendKeysBackgroundWorker.ReportProgress(100);

            Invoke((MethodInvoker) delegate
            {
                this.progressLabel.Text = "Sending finish token";
            });

            var finData = new JObject {{"guid", _guid}};

            var finReq = new RESTRequest("/finish/", HttpMethod.POST, ContentType.JSON, _timeout);
            //TODO: migrate all requests to this
            finReq.Payload = finData.ToString();
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

            //Encrypts the data
            _aesData = Aes.Encrypt(text, _complexity);
            //Stores the encryption key as a global variable
            _key = _aesData.Key;

            int bytes;
            using (var aesCsp = new AesCryptoServiceProvider())
            {
                bytes = aesCsp.KeySize/8;
            }
            var rng = new RNGCryptoServiceProvider();
            for (var i = 0; i < _amount; i++)
            {
                var randBytes = new byte[bytes];
                rng.GetBytes(randBytes);
                _fakeKeys.Enqueue(Convert.ToBase64String(randBytes));
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressLabel.Text = "Attempting to contact " + _ip;


            var client = new RESTClient("http://" + _ip);
            if (Common.GetSSHKey(_ip, out _remoteKey)) //TODO: async
            {
                Close();
                return;
            }

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
            req.Payload = data.ToString();
            var response = client.Execute(req);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show("error");
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