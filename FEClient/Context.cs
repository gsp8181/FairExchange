﻿using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using FEClient.API;
using FEClient.Forms;
using FEClient.NotMyCode;
using Grapevine.Server;

namespace FEClient
{
    public partial class Context : ApplicationContext
    {
        private NotifyRequestEventArgs _currentTipReq;
        public static readonly string Port = ConfigurationManager.AppSettings["Port"];
        private static readonly RESTServer Server = new RESTServer("+", Port);

        public Context()
        {
            while (!SettingsWrapper.IsSet)
            {
                using (var dialog = new SettingsDialog())
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

            notifyIcon.Text += " :" + Port;

            CreateFirewallException(int.Parse(Port)); //TODO: wait for firewall
            Server.OnStart = OnServerStartNotify;
            Server.Start();
            if (!Server.IsListening) //TODO: maybe sort out with a timer
            {
                NetAclChecker.AddAddress("http://+:" + Port + "/");
                Server.Start();
                /*if (!Program.server.IsListening)
                {
                    MessageBox.Show("Could not bind port 6555", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error); //todo: does not work
                    Environment.Exit(-1);
                }*/
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

        private void OnServerStartNotify() //TODO: if this doesn't happen in 20 ish seconds, abort/retry/fail?
        {
//TODO: check if port is open
            ShowBalloonTip(5000, "Started", "Server started and is listening on port " + Port, ToolTipIcon.Info);
        }

        private static void CreateFirewallException(int port)
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
            Environment.Exit(0);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SettingsDialog().ShowDialog();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var so = new SendOptions();
            
                so.Show();
                so.Focus();
            
        }

        private void logsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var logs = new Logs();
            logs.Show();
            logs.Focus();
        }
    }
}