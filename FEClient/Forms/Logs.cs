﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FEClient.Forms
{
    public partial class Logs : Form
    {
        DirectoryInfo sentLogpath = new DirectoryInfo(Application.UserAppDataPath + @"\logs\sent\");
        DirectoryInfo receivedLogPath = new DirectoryInfo(Application.UserAppDataPath + @"\logs\received\");
//        ImageList icon;
        public Logs()
        {
            InitializeComponent();
        }

        private void Logs_Load(object sender, EventArgs e)
        {
            var icon = new ImageList();
            icon.Images.Add(global::FEClient.Properties.Resources.Icojam_Blue_Bits_Document_arrow_down);
            receivedListView.LargeImageList = icon;
            sentListView.LargeImageList = icon;
            /*if (sentLogpath.Exists)
            {
                foreach(var file in sentLogpath.GetFiles())
                {
                    ListViewItem i = new ListViewItem(file.Name);
                    sentListView.Items.Add(i);
                }
            }*/
            if (receivedLogPath.Exists)
            {
                foreach (var file in receivedLogPath.GetFiles())
                {
                    ListViewItem i = new ListViewItem(file.Name);
                    i.ImageIndex = 0;
                    receivedListView.Items.Add(i);
                }
            }
        }

        private void receivedListView_DoubleClick(object sender, EventArgs e)
        {
            var list = (ListView) sender;
            if (list.SelectedItems.Count <= 0)
                return;
            var item = list.SelectedItems[0];
            MessageBox.Show(item.Text);
        }
    }
}
