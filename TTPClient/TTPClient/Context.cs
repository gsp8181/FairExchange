using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using Grapevine.Server;
using TTPClient.API;
using TTPClient.NotMyCode;

namespace TTPClient
{
    
    public partial class Context : ApplicationContext
    {
        public static RESTServer server = new RESTServer("+", "6555", "http", "index.html", null, 5);
        public Context()
        {
            while (!SettingsWrapper.Instance.IsSet)
            {
                using (SettingsDialog dialog = new SettingsDialog())
                {
                    dialog.ShowDialog(); //TODO: if cancel then quit
                }
            }

            //SettingsWrapper.Instance.RegWithTracker(); //TODO: is port open? notify registration balloon?


            InitializeComponent();
            //ClientRestApi.Notify += MyResource_Click;
            ClientRestApi.NotifyRecieved += MyResource_NotifyRecieved;

            Form1_Load(null,null);
        }



        private NotifyRequest currentTipReq;
        private SettingsWrapper settings = SettingsWrapper.Instance;
        //RESTClient restClient = new RESTClient(textBox1.Text);

        private void ShowBalloonTip(int timeout, string tipTitle, string tipText, ToolTipIcon tipIcon,
            NotifyRequest nr = null)
        {
            currentTipReq = nr;
            notifyIcon.ShowBalloonTip(timeout, tipTitle, tipText, tipIcon);
        }

        private void MyResource_NotifyRecieved(object sender, NotifyRequest nr)
        {
            ShowBalloonTip(60000, "Incoming File",
                nr.email + " wants to send you " + nr.fileName + ". Click to accept", ToolTipIcon.Info, nr);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            createFirewallException(6555);
            Context.server.OnStart = onServerStartNotify;
            Context.server.Start();
            if (!Context.server.IsListening) //TODO: maybe sort out with a timer
            {
                NetAclChecker.AddAddress("http://+:6555/");
                Context.server.Start();
                /*if (!Program.server.IsListening)
                {
                    MessageBox.Show("Could not bind port 6555", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error); //todo: does not work
                    Environment.Exit(-1);
                }*/
            }
        }

        private void onServerStartNotify()
        {
//TODO: check if port is open
            ShowBalloonTip(5000, "Started", "Server started and is listening on port 6555", ToolTipIcon.Info);
        }

        private void createFirewallException(int port)
            //Stackoverflow how to diusplay windows firewall has blocked some features of this program;
        {
            var ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
                //TODO: is this needed in the presence of the other listener?
            var ipLocalEndPoint = new IPEndPoint(ipAddress, port);

            var t = new TcpListener(ipLocalEndPoint);
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
            this.ExitThread();
            //Application.Exit();
            //Environment.Exit(0);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SettingsDialog().ShowDialog();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var so = new SendOptions())
            {
                so.ShowDialog();
            }
        }

        private void logsToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

    }
}