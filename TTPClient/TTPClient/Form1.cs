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
using TTPClient.NotMyCode;
using TTPClient.Properties;
using TTPClient.Security;

namespace TTPClient
{
    public partial class Form1 : Form
    {
        WebClient syncClient = new WebClient();
        NotifyRequest currentTipReq;
        SettingsWrapper settings = SettingsWrapper.Instance;
        //RESTClient restClient = new RESTClient(textBox1.Text);

        public Form1()
        {
            InitializeComponent();
            //ClientRestApi.Notify += MyResource_Click;
            ClientRestApi.NotifyRecieved += MyResource_NotifyRecieved;   
        }

        private void ShowBalloonTip(int timeout, string tipTitle, string tipText, ToolTipIcon tipIcon, NotifyRequest nr = null)
        {
            currentTipReq = nr;
            notifyIcon.ShowBalloonTip(timeout, tipTitle, tipText, tipIcon);
        }

        private void MyResource_NotifyRecieved(object sender, NotifyRequest nr)
        {
            ShowBalloonTip(60000, "Incoming File",
                nr.email + " wants to send you " + nr.fileName + ". Click to accept", ToolTipIcon.Info, nr);
        }

        private void whatMyIp_Click(object sender, EventArgs e)
        {
            var url = settings.TTP + "/rest/config/ip/";

            var content = syncClient.DownloadString(url);
            MessageBox.Show(content);
        }

        private void regWithTrackerButton_Click(object sender, EventArgs e)
        {
            var response = settings.RegWithTracker();
            MessageBox.Show(response.ToString());
        }

        private void PortOpenButton_Click(object sender, EventArgs e)
        {

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
            IPEndPoint ipLocalEndPoint = new IPEndPoint(ipAddress, port);

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
            var receiveDialog = new ReceiveDialog(currentTipReq.ip, currentTipReq.fileName, currentTipReq.guid);
            receiveDialog.Show();

            /*var client = new RESTClient("http://" + currentTipReq.ip + ":6555");
            var req = new RESTRequest("/start/");
            JObject data = new JObject {{"fileName", currentTipReq.fileName}, {"email", settings.Email}};
            req.Method = Grapevine.HttpMethod.POST;
            req.ContentType = Grapevine.ContentType.JSON;
            req.Payload = data.ToString();
            var response = client.Execute(req);*/
            //MessageBox.Show("Status Code: " + response.StatusCode);
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            //Application.Exit();
            //Environment.Exit(0);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SettingsDialog().ShowDialog();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SendOptions so = new SendOptions())
            {
                so.ShowDialog();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            SettingsWrapper.Instance.RegWithTracker();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Rsa.getPublicKey())
            ;
        }

        private void logsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
