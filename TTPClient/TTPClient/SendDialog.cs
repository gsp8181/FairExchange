using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grapevine.Client;
using Newtonsoft.Json.Linq;
using TTPClient.Security;

namespace TTPClient
{
    public partial class SendDialog : Form
    {
        private string ip;
        private FileInfo file;
        private string email = "test@email.com";
        public SendDialog(string ip, string fileName)
        {
            InitializeComponent();
            MyResource.StartTransmission += MyResource_StartTransmission;
            MyResource.StartTransmissionAndRespSent += MyResource_StartTransmissionAndRespSent;
            this.ip = ip;
            this.file = new FileInfo(fileName);

            progressLabel.Text += ip;

            var client = new RESTClient("http://" + ip + ":6555");
            var req = new RESTRequest("/notify/");
            JObject data = new JObject { { "fileName", file.Name }, { "email", email } };//TODO: two names at once?! send guid?
            req.Method = Grapevine.HttpMethod.POST;
            req.ContentType = Grapevine.ContentType.JSON; //TODO: async and await
            req.Payload = data.ToString();
            var response = client.Execute(req);
            //MessageBox.Show("Status Code: " + response.StatusCode); //TODO: if 200
            timer1.Start();

        }

        private void MyResource_StartTransmissionAndRespSent(object sender, NotifyRequest vars)
        {
            if (vars.fileName != file.Name)
                return;
            this.Invoke((MethodInvoker)delegate
            {
                timer2_Tick();
            }); //TODO: maybe another timeout timer?
        }

        private void MyResource_StartTransmission(object sender, NotifyRequest addrSender, NotifyArgs callbackArgs)
        {
            if (addrSender.fileName != file.Name)
                return;
            callbackArgs.hasSet = true; //TODO: check filename and dispatch another thread which updates progress, maybe have a positive event firing
            this.Invoke((MethodInvoker)delegate
            {
                timer1.Stop();
            });

        }

        private void SendDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            MyResource.StartTransmission -= MyResource_StartTransmission;
            MyResource.StartTransmissionAndRespSent -= MyResource_StartTransmissionAndRespSent;
            this.Dispose();
            //file.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            progressBar1.Style = ProgressBarStyle.Continuous;
            MessageBox.Show("Remote user did not respond in time", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.Close();
        }

        private void timer2_Tick()
        {
            progressLabel.Text = "Sending " + file.Name;
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.Value = 25;

            var stream = file.OpenRead(); //TODO: if not null and using!
            string text;
            using (StreamReader sr = new StreamReader(stream)) //TODO: all using for streams
            {

                text = sr.ReadToEnd();
            }
            var client = new RESTClient("http://" + ip + ":6555");
            var req = new RESTRequest("/file/")
            {
                Method = Grapevine.HttpMethod.POST,
                ContentType = Grapevine.ContentType.JSON
            };
            JObject data = new JObject
                {
                    {"fileName", file.Name},
                    {"email", email},
                    {"data", Base64.Base64Encode(text)}
                };
            req.Payload = data.ToString();
            req.Timeout = 10 * 1000;
            //req.Payload = text;
            var response = client.Execute(req);
            if (response.ReturnedError || !string.IsNullOrEmpty(response.Error)) //TODO: accepted? TODO: better response checking for example timeout
            {
                progressBar1.Value = 0;
                progressBar1.Style = ProgressBarStyle.Continuous; //TODO: update label
                MessageBox.Show("Remote server did not accept the file" + Environment.NewLine + response.Error, "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            var json = JObject.Parse(response.Content);
            MessageBox.Show(json.Value<string>("signature"));


            //TODO: use async and await
            progressBar1.Value = 50;
            progressLabel.Text = "Sent file undefined behaviour now! :)";

        }
    }
}
