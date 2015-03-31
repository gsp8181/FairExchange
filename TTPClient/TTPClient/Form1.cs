using Grapevine.Client;
using Newtonsoft.Json.Linq;
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
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TTPClient.Properties;

namespace TTPClient
{
    public partial class Form1 : Form
    {
        WebClient syncClient = new WebClient();
        NotifyRequest currentTipReq = null;
        SettingsWrapper settings = new SettingsWrapper();
        //RESTClient restClient = new RESTClient(textBox1.Text);

        public Form1()
        {
            InitializeComponent();
            //MyResource.Notify += MyResource_Click;
            MyResource.NotifyRecieved += MyResource_NotifyRecieved;   
        }

        private void ShowBalloonTip(int timeout, string tipTitle, string tipText, ToolTipIcon tipIcon, NotifyRequest nr = null)
        {
            if (nr == null)
            {
                currentTipReq = null;
            }
            else
            {
                currentTipReq = nr;
            }
            notifyIcon1.ShowBalloonTip(timeout, tipTitle, tipText, tipIcon);
        }

        private void MyResource_NotifyRecieved(object sender, NotifyRequest nr)
        {
            ShowBalloonTip(60000, "Incoming File",
                nr.email + " wants to send you " + nr.fileName + ". Click to accept", ToolTipIcon.Info, nr);
        }

        private void whatMyIp_Click(object sender, EventArgs e)
        {
            var url = (string)Settings.Default["TTP"] + "/rest/config/ip/";

            var content = syncClient.DownloadString(url);
            MessageBox.Show(content);
        }

        private void regWithTrackerButton_Click(object sender, EventArgs e)
        {
            var response = settings.RegWithTracker();
            MessageBox.Show(response.ToString());
        }

        private void getRemoteStatus_Click(object sender, EventArgs e)
        {
            var url = (string)Settings.Default["TTP"] + "/rest/sessions/";
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var filename = openFileDialog1.FileName;
                string ip = Microsoft.VisualBasic.Interaction.InputBox("IP", "IP ADDRESS", "127.0.0.1", -1, -1); //TODO: Transition to email/ip combo?
                IPAddress ipObj;
                /*if (!IPAddress.TryParse(ip,out ipObj))
                {
                    MessageBox.Show("Failed to parse IP");
                    return;
                }*/
                
                var sendDialog = new SendDialog(ip, filename);
                sendDialog.Show();


            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            createFirewallException(6555);
            Program.server.OnStart = onServerStartNotify;
            Program.server.Start();
            settings.RegWithTracker();
            if (!Program.server.IsListening) //TODO: maybe sort out with a timer
            {
                NetAclChecker.AddAddress("http://+:6555/");
                Program.server.Start();
                /*if (!Program.server.IsListening)
                {
                    MessageBox.Show("Could not bind port 6555", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error); //todo: does not work
                    Environment.Exit(-1);
                }*/
            }
        }

        private void onServerStartNotify()
        {//TODO: check if port is open
            ShowBalloonTip(5000, "Started", "Server started and is listening on port 6555", ToolTipIcon.Info);
        }

        private void createFirewallException(int port) //Stackoverflow how to diusplay windows firewall has blocked some features of this program;
        {
            IPAddress ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0]; //TODO: is this needed in the presence of the other listener?
            IPEndPoint ipLocalEndPoint = new IPEndPoint(ipAddress, 6555);

            TcpListener t = new TcpListener(ipLocalEndPoint);
            t.Start();
            t.Stop();
        }

        private void notifyIcon1_BalloonTipClicked_1(object sender, EventArgs e)
        {
            if (currentTipReq == null)
            {
                return;
            }
            var receiveDialog = new ReceiveDialog(currentTipReq.ip, currentTipReq.fileName);
            receiveDialog.Show();

            var client = new RESTClient("http://" + currentTipReq.ip + ":6555");
            var req = new RESTRequest("/start/");
            JObject data = new JObject();
            data.Add("fileName",currentTipReq.fileName);
            data.Add("email", (string)Settings.Default["Email"]);
            req.Method = Grapevine.HttpMethod.POST;
            req.ContentType = Grapevine.ContentType.JSON;
            req.Payload = data.ToString();
            var response = client.Execute(req);
            //MessageBox.Show("Status Code: " + response.StatusCode);
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            //Application.Exit();
            Environment.Exit(0);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SettingsDialog().ShowDialog();
        }
    }
}
