using System;
using System.IO;
using System.Windows.Forms;
using FEClient.Properties;

namespace FEClient.Forms
{
    public partial class Logs : Form
    {
        private readonly DirectoryInfo _sentLogPath = new DirectoryInfo(Application.UserAppDataPath + @"\logs\sent\");
        private readonly DirectoryInfo _receivedLogPath = new DirectoryInfo(Application.UserAppDataPath + @"\logs\received\");
        public Logs()
        {
            InitializeComponent();
            iconImageList.Images.Add(Resources.Icojam_Blue_Bits_Document_arrow_down);
            receivedListView.LargeImageList = iconImageList;
            sentListView.LargeImageList = iconImageList;
        }

        private void Logs_Load(object sender, EventArgs e)
        {
            if (_sentLogPath.Exists)
            {
                foreach (var file in _sentLogPath.GetFiles())
                {
                    var i = new ListViewItem(file.Name) { ImageIndex = 0 };
                    sentListView.Items.Add(i);
                }
            }
            if (_receivedLogPath.Exists)
            {
                foreach (var file in _receivedLogPath.GetFiles())
                {
                    var i = new ListViewItem(file.Name) {ImageIndex = 0};
                    receivedListView.Items.Add(i);
                }
            }
        }

        private void receivedListView_DoubleClick(object sender, EventArgs e)
        {
            var logPath = _receivedLogPath;

            Show_Log(sender, logPath);
        }

        private static void Show_Log(object sender, DirectoryInfo logPath)
        {
            var list = (ListView) sender;
            if (list.SelectedItems.Count <= 0)
                return;
            var item = list.SelectedItems[0];
            var fileName = logPath.FullName + item.Text;
            using (var dialog = new LogViewer(fileName))
            {
                dialog.ShowDialog();
            }
        }

        private void sentListView_DoubleClick(object sender, EventArgs e)
        {
            var logPath = _sentLogPath;

            Show_Log(sender, logPath);
        }
    }
}
