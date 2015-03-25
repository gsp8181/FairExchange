using Grapevine.Client;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TTPClient
{
    public partial class Form1 : Form
    {
        WebClient syncClient = new WebClient();

        public Form1()
        {
            InitializeComponent();
            MyResource.Notify += MyResource_Click;
            MyResource.FileRecieved += MyResource_FileRecieved;
        }

        void MyResource_FileRecieved(object sender, string fileName)
        {
            notifyIcon1.ShowBalloonTip(5000, "File Recieved", fileName, ToolTipIcon.Info);
        }

        void MyResource_Click(object sender, string myValue)
        {
            //notifyIcon1.BalloonTipText(myValue);
            notifyIcon1.ShowBalloonTip(5000, "Request Recieved", myValue, ToolTipIcon.Info);
        }

        private void whatMyIp_Click(object sender, EventArgs e)
        {
            var url = textBox1.Text + "/rest/config/ip/";

            var content = syncClient.DownloadString(url);
            MessageBox.Show(content);
        }

        private void regWithTrackerButton_Click(object sender, EventArgs e)
        {
            byte[] emailBytes = System.Text.Encoding.UTF8.GetBytes(emailBox.Text);
            var url = textBox1.Text + "/rest/sessions/";
            var request = (HttpWebRequest)WebRequest.CreateHttp(url);
            request.Method = "POST";
            request.ContentLength = emailBytes.Length;
            request.ContentType = "application/json";
            var dataStream = request.GetRequestStream();
            dataStream.Write(emailBytes, 0, emailBytes.Length);
            dataStream.Close();
            var response = (HttpWebResponse)request.GetResponse();
            MessageBox.Show(response.StatusDescription);
        }

        private void getRemoteStatus_Click(object sender, EventArgs e)
        {
            var url = textBox1.Text + "/rest/sessions/";
            string input = Microsoft.VisualBasic.Interaction.InputBox("Prompt", "Title", "Default", -1, -1);

            url = url + input;
            try
            {
                var content = syncClient.DownloadString(url);
                MessageBox.Show(content);
            }
            catch (WebException ex)
            {
                MessageBox.Show("Could not find. " + ex.Message);
            }

        }

        private void PortOpenButton_Click(object sender, EventArgs e)
        {

        }

        private void GenDSAKeysButton_Click(object sender, EventArgs e)
        {
            using (var rsa = new DSACryptoServiceProvider(1024))
            {
                try
                {
                    string publicPrivateKeyXML = rsa.ToXmlString(true);
                    string publicOnlyKeyXML = rsa.ToXmlString(false);
                    AsymmetricCipherKeyPair dsaKey = DotNetUtilities.GetDsaKeyPair(rsa);
                    //MessageBox.Show(publicOnlyKeyXML);
                    MessageBox.Show(publicPrivateKeyXML);
                    StringWriter sw = new StringWriter();
                    PemWriter pw = new PemWriter(sw);
                    pw.WriteObject(dsaKey);
                    //pw.flush();
                    String rsakeypem = sw.ToString();
                    MessageBox.Show(rsakeypem);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            { 

            var stream = openFileDialog1.OpenFile(); //TODO: if not null and using!
                using (StreamReader sr = new StreamReader(stream))
                {
                    String text = sr.ReadToEnd();
                    string ip = Microsoft.VisualBasic.Interaction.InputBox("IP", "IP ADDRESS", "Default", -1, -1);
                    var client = new RESTClient("http://" + ip + ":6555");
                    var req = new RESTRequest("/file/");
                    req.Method = Grapevine.HttpMethod.POST;
                    req.ContentType = Grapevine.ContentType.JSON;
                    req.Payload = text;
                    client.Execute(req);
                    MessageBox.Show(text);
                }
                    
            //MessageBox.Show(stream.);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Program.server.OnStart = new Grapevine.Server.ToggleServerHandler(this.onServerStartNotify);
            Program.server.Start();
        }

        private void onServerStartNotify()
        {//TODO: check if port is open
            notifyIcon1.ShowBalloonTip(5000, "Started", "Server started and is listening on port 6555", ToolTipIcon.Info);
        }
    }
}
