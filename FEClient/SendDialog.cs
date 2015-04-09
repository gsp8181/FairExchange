using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Windows.Forms;
using FEClient.API;
using FEClient.Security;
using FEClient.SQLite;
using Grapevine;
using Grapevine.Client;
using Newtonsoft.Json.Linq;
using Aes = FEClient.Security.Aes;

namespace FEClient
{
    public partial class SendDialog : Form
    {
        private readonly string _ip;
        private FileInfo _file;
        private AesKeys _key;
        private AesData _aesData;
        private readonly string _guid;
        public Queue<string> FakeKeys = new Queue<string>();
        private readonly int _amount;
        private readonly int _complexity;
        private readonly int _timeout;
        private string _remoteKey;

        public SendDialog(string ip, string fileName, int rounds, int complexity, int timeout)
        {
            InitializeComponent();
            ClientRestApi.StartTransmission += MyResource_StartTransmission;
            ClientRestApi.StartTransmissionAndRespSent += MyResource_StartTransmissionAndRespSent;
            this._ip = ip;
            _file = new FileInfo(fileName);

            _guid = Guid.NewGuid().ToString();
            _amount = rounds;
            this._complexity = complexity;
            this._timeout = timeout;

        }

        private void MyResource_StartTransmissionAndRespSent(object sender, NotifyRequest vars)
        {
            if (vars.Guid != _guid)
                return;
            Invoke((MethodInvoker)delegate
            {
                timer2_Tick();
            }); //TODO: maybe another timeout timer?
        }

        private void MyResource_StartTransmission(object sender, NotifyRequest addrSender, NotifyArgs callbackArgs)
        {
            if (addrSender.FileName != _file.Name)
                return;
            callbackArgs.HasSet = true;
            Invoke((MethodInvoker)delegate
            {
                timer1.Stop();
            });

        }

        private void SendDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClientRestApi.StartTransmission -= MyResource_StartTransmission;
            ClientRestApi.StartTransmissionAndRespSent -= MyResource_StartTransmissionAndRespSent;
            Dispose();
            //file.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Style = ProgressBarStyle.Continuous;
            MessageBox.Show("Remote user did not respond in time", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
        }

        private void timer2_Tick()
        {
            // Updates progress label to show file is sending
            progressLabel.Text = "Sending " + _file.Name;
            progressBar1.Style = ProgressBarStyle.Continuous;

            // Creates a new POST request to the remote client
            var client = new RESTClient("http://" + _ip);
            var req = new RESTRequest("/file/")
            {
                Method = HttpMethod.POST,
                ContentType = ContentType.JSON
            };

            //Embeds the data (fig 1)
            JObject data = new JObject
                {
                    {"fileName", _file.Name},
                    {"email", SettingsWrapper.Email},
                    {"guid", _guid},
                    {"iv",_key.IvStr},
                    //{"data", Base64.Base64Encode(text)}
                    //{"complexity",this.complexity},
                    //{"timeout",this.timeout},
                    {"data", _aesData.DataStr}, //TODO: RSA SIGN!!
                    {"signature", "NYI"}
                    // NRO (sSa(F nro, B, L, C)
                };
            req.Payload = data.ToString();
            //Sends the request
            var response = client.Execute(req);

            //If there was an error then fail and quit
            if (response.ReturnedError || !string.IsNullOrEmpty(response.Error)) //TODO: accepted? TODO: better response checking for example timeout
            {
                progressBar1.Style = ProgressBarStyle.Continuous; //TODO: update label
                MessageBox.Show("Remote server did not accept the file" + Environment.NewLine + response.Error, "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }
            var json = JObject.Parse(response.Content);
            //MessageBox.Show(json.Value<string>("signature"));


            // Update the progress box
            //TODO: use async and await
            progressLabel.Text = "Sending Keys";
            backgroundWorker1.RunWorkerAsync();

        }

        private void SendDialog_Load(object sender, EventArgs e)
        {
            backgroundWorker2.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var stopwatch = new Stopwatch();
            var client = new RESTClient("http://" + _ip);
            stopwatch.Start();
            for (int i = 0; i < _amount; i++)
            { //Check cancellation
                var fkey = FakeKeys.Dequeue();

                JObject data = new JObject {{"key", fkey},{"guid",_guid},{"i",i}};

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
                    
                    if(response.StatusCode != HttpStatusCode.OK) //TODO: OR IF TIMEOUT
                {
                    Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show("Error");
                    });
                    return;
                }

                //TODO:Check Sig

                backgroundWorker1.ReportProgress((i/_amount)*100); //TODO: fix
            }

            JObject realData = new JObject {{"key", _key.KeyStr}, {"guid", _guid}, {"i", _amount}}; //TODO: encrypt keys??


            var realReq = new RESTRequest("/key/");//TODO: split into method with above bit
            realReq.Timeout = _timeout;
            realReq.Method = HttpMethod.POST;
            realReq.ContentType = ContentType.JSON; //TODO: async and await
            realReq.Payload = realData.ToString();
            var realResponse = client.Execute(realReq);
            if (realResponse.StatusCode != HttpStatusCode.OK) //TODO: OR IF TIMEOUT
            {
                Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show("Error");
                    this.Close();
                });
                return;
            }


            backgroundWorker1.ReportProgress(100);
            //Check Sig


        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage; //TODO: does not work
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            // Opens and reads the file to the end
            var stream = _file.OpenRead(); //TODO: if not null and using!
            string text;
            using (StreamReader sr = new StreamReader(stream)) //TODO: all using for streams
            {

                text = sr.ReadToEnd();
            }

            //Encrypts the data
            _aesData = Aes.Encrypt(text, _complexity);
            //Stores the encryption key as a global variable
            _key = _aesData.Key;

            int bytes;
            using (AesCryptoServiceProvider aesCsp = new AesCryptoServiceProvider())
            {
                bytes = aesCsp.KeySize / 8;
            }
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            for (int i = 0; i < _amount; i++)
            {
                byte[] randBytes = new byte[bytes];
                rng.GetBytes(randBytes);
                FakeKeys.Enqueue(Convert.ToBase64String(randBytes)); 
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {



            progressLabel.Text = "Attempting to contact " + _ip;


            var client = new RESTClient("http://" + _ip);

            var keyReq = new RESTRequest("/ident/");

            var keyResponse = client.Execute(keyReq);

            if (keyResponse.StatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show("error"); //TODO: make better
            }

            var keyRespObj = JObject.Parse(keyResponse.Content);

            _remoteKey = keyRespObj.Value<string>("pubKey"); //TODO: flag on error
            var email = keyRespObj.Value<string>("email");

            var keyObj = Adapter.Instance.GetByEmail(email);

            if (keyObj == null)
            {
                var dialogResult =
                    MessageBox.Show(
                        "The key for " + email + " has not been registered, do you wish to accept?\n" + _remoteKey,
                        "New key", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.OK)
                {
                    var dbObj = new PubKey();
                    dbObj.Email = email;
                    dbObj.Pem = _remoteKey;
                    Adapter.Instance.Insert(dbObj);
                }
                else
                {
                    Close();
                    return;
                }
            } else if (keyObj.Pem != _remoteKey)
            {
                var dialogResult =
    MessageBox.Show(
        "The key for " + email + " has BEEN CHANGED, this could indicate interception\n Do you wish to accept?\n" + _remoteKey,
        "CHANGED KEY", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.OK)
                {
                    var dbObj = new PubKey();
                    dbObj.Email = email;
                    dbObj.Pem = _remoteKey;
                    Adapter.Instance.Insert(dbObj);
                }
                else
                {
                    Close();
                    return;
                }
            }


            var req = new RESTRequest("/notify/");
            var data = new JObject { { "fileName", _file.Name }, { "email", SettingsWrapper.Email }, { "guid", _guid }, {"timeout", _timeout}, {"complexity", _complexity}, {"port",Context.Port} };
            req.Method = HttpMethod.POST;
            req.ContentType = ContentType.JSON; //TODO: async and await
            req.Payload = data.ToString();
            var response = client.Execute(req);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show("error"); //TODO; this needs to be better, maybe a handle error method which tries to get the error string
            }
            timer1.Start();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressLabel.Text = "Finished";
        }
    }
}
