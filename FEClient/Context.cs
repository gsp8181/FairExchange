using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using FEClient.API;
using FEClient.NotMyCode;
using FEClient.SQLite;
using Grapevine.Server;

namespace FEClient
{
    
    public partial class Context : ApplicationContext
    {
        public const string port = "6555";
        private static RESTServer server = new RESTServer("+", port, "http", "index.html", null, 5);
        private NotifyRequest currentTipReq;
        private Adapter adapter = Adapter.Instance;

        public Context()
        {
            while (!SettingsWrapper.Instance.IsSet)
            {
                using (SettingsDialog dialog = new SettingsDialog())
                {
                    dialog.ShowDialog(); //TODO: if cancel then quit
                    if (dialog.DialogResult == DialogResult.Cancel)
                    {
                        Environment.Exit(-1);
                    }
                }
            }

            //TODO: is port open?

            InitializeComponent();

            ClientRestApi.NotifyRecieved += ClientRestApi_NotifyRecieved;

            createFirewallException(int.Parse(port));
            server.OnStart = onServerStartNotify;
            server.Start();
            if (!server.IsListening) //TODO: maybe sort out with a timer
            {
                NetAclChecker.AddAddress("http://+:"+port+"/");
                server.Start();
                /*if (!Program.server.IsListening)
                {
                    MessageBox.Show("Could not bind port 6555", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error); //todo: does not work
                    Environment.Exit(-1);
                }*/
            }
        }

        private void ShowBalloonTip(int timeout, string tipTitle, string tipText, ToolTipIcon tipIcon,
            NotifyRequest nr = null)
        {
            currentTipReq = nr;
            notifyIcon.ShowBalloonTip(timeout, tipTitle, tipText, tipIcon);
        }

        private void ClientRestApi_NotifyRecieved(object sender, NotifyRequest nr)
        {
            ShowBalloonTip(60000, "Incoming File",
                nr.email + " wants to send you " + nr.fileName + ". Click to accept", ToolTipIcon.Info, nr);
        }

        private void onServerStartNotify()
        {
//TODO: check if port is open
            ShowBalloonTip(5000, "Started", "Server started and is listening on port " + port, ToolTipIcon.Info);
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

        private void notifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            if (currentTipReq == null)
            {
                return;
            }
            var receiveDialog = new ReceiveDialog(currentTipReq);
            receiveDialog.Show();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExitThread();
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