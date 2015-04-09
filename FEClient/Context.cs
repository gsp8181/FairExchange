using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using FEClient.API;
using FEClient.Forms;
using FEClient.NotMyCode;
using FEClient.SQLite;
using Grapevine.Server;

namespace FEClient
{
    
    public partial class Context : ApplicationContext
    {
        public const string Port = "6555";
        private static RESTServer _server = new RESTServer("+", Port);
        private NotifyRequest _currentTipReq;

        public Context()
        {
            while (!SettingsWrapper.IsSet)
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

            CreateFirewallException(int.Parse(Port));
            _server.OnStart = OnServerStartNotify;
            _server.Start();
            if (!_server.IsListening) //TODO: maybe sort out with a timer
            {
                NetAclChecker.AddAddress("http://+:"+Port+"/");
                _server.Start();
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
            _currentTipReq = nr;
            notifyIcon.ShowBalloonTip(timeout, tipTitle, tipText, tipIcon);
        }

        private void ClientRestApi_NotifyRecieved(object sender, NotifyRequest nr)
        {
            ShowBalloonTip(60000, "Incoming File",
                nr.Email + " wants to send you " + nr.FileName + ". Click to accept", ToolTipIcon.Info, nr);
        }

        private void OnServerStartNotify()
        {
//TODO: check if port is open
            ShowBalloonTip(5000, "Started", "Server started and is listening on port " + Port, ToolTipIcon.Info);
        }

        private void CreateFirewallException(int port)
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
            if (_currentTipReq == null)
            {
                return;
            }
            var receiveDialog = new ReceiveDialog(_currentTipReq);
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