using System;
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
        }
    }
}
