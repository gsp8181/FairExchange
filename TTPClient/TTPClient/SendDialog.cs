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

namespace TTPClient
{
    public partial class SendDialog : Form
    {
        public SendDialog(string ip, Stream file)
        {
            InitializeComponent();
        }
    }
}
