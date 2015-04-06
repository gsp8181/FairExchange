using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using FEClient.API;
using FEClient.NotMyCode;
using Grapevine.Server;

namespace FEClient
{
    
    public partial class Context : ApplicationContext
    {
        private static RESTServer server = new RESTServer("+", "6555", "http", "index.html", null, 5);
        public Context()
        {
            do
            {
                using (SettingsDialog dialog = new SettingsDialog())
                {
                    dialog.ShowDialog(); //TODO: if cancel then quit
                    if (dialog.DialogResult == DialogResult.Cancel)
                    {
                        Environment.Exit(-1);
                    }
                }
            } while (!SettingsWrapper.Instance.IsSet);

            //TODO: is port open?


            InitializeComponent();

            ClientRestApi.NotifyRecieved += MyResource_NotifyRecieved;

            Form1_Load(null,null);
        }



        private NotifyRequest currentTipReq;

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
            server.OnStart = onServerStartNotify;
            server.Start();
            if (!server.IsListening) //TODO: maybe sort out with a timer
            {
                NetAclChecker.AddAddress("http://+:6555/");
                server.Start();
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