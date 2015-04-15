using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using FEClient.API;
using FEClient.API.Events;
using FEClient.Forms;
using FEClient.NotMyCode;
using Grapevine.Server;

namespace FEClient
{
    public partial class Context : ApplicationContext
    {
        private NotifyRequestEventArgs _currentTipReq;

        public Context()
        {
            while (!SettingsWrapper.IsSet)
            {
                using (var dialog = new SettingsDialog())
                {
                    dialog.ShowDialog();
                    if (dialog.DialogResult == DialogResult.Cancel)
                    {
                        Environment.Exit(-1);
                    }
                }
            }

            InitializeComponent();

            ClientRestApi.NotifyRecieved += ClientRestApi_NotifyRecieved;

            notifyIcon.Text += " :" + Port;

            NetAclChecker.CreateFirewallException(int.Parse(Port)); 
            timeoutTimer.Start();
            Server_Create();
            if (!Server.IsListening)
            {
                NetAclChecker.AddAddress("http://+:" + Port + "/");
                Server_Create();
            }
        }

        private void Server_Create()
        {
            if(Server != null)
            { 
                Server.Stop();
                Server.Dispose();
            }
            Server = new RESTServer("+", Port) {OnStart = OnServerStartNotify};
            Server.Start();
        }

        private void timeoutTimer_Tick(object sender, EventArgs e)
        {
            Server_Create();
            if (!Server.IsListening)
            {
                ShowBalloonTip(10000, "Server has not started",
                    "The server has not started properly\nPlease check your network settings", ToolTipIcon.Error, null);
            }
        }

        private void ShowBalloonTip(int timeout, string tipTitle, string tipText, ToolTipIcon tipIcon,
            NotifyRequestEventArgs nr = null)
        {
            _currentTipReq = nr;
            notifyIcon.ShowBalloonTip(timeout, tipTitle, tipText, tipIcon);
        }

        private void ClientRestApi_NotifyRecieved(object sender, NotifyRequestEventArgs nr)
        {
            ShowBalloonTip(60000, "Incoming File",
                nr.Email + " wants to send you " + nr.FileName + ". Click to accept", ToolTipIcon.Info, nr);
        }

        private void OnServerStartNotify()
        {
            timeoutTimer.Stop();
            ShowBalloonTip(5000, "Started", "Server started and is listening on port " + Port, ToolTipIcon.Info);
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
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
            Environment.Exit(0);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new SettingsDialog())
            {
                dialog.ShowDialog();
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var so = new SendOptions();

            so.Show();
            so.Focus();
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void logsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var logs = new Logs();
            logs.Show();
            logs.Focus();
        }

        public static readonly string Port = ConfigurationManager.AppSettings["Port"];
        private static RESTServer Server; // = new RESTServer("+", Port);
    }
}