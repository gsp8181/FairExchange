using System.IO;
using System.Windows.Forms;

namespace FEClient.Forms
{
    public partial class LogViewer : Form
    {
        public LogViewer(string fileName)
        {
            InitializeComponent();
            var file = new FileInfo(fileName);
            using (var stream = file.OpenText())
            {
                logBox.Text = stream.ReadToEnd();
            }
            logBox.SelectionStart = 0;
            logBox.SelectionLength = 0;
        }
    }
}
