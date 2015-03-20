﻿using System;
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
                } catch (WebException ex)
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
                    //MessageBox.Show(publicOnlyKeyXML);
                    MessageBox.Show(publicPrivateKeyXML);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }
    }
}
