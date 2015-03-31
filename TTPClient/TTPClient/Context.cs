using System;
using System.Windows.Forms;

namespace TTPClient
{
    
    public class Context : ApplicationContext
    {
        public Form1 form1;
        public Context()
        {      
         form1 = new Form1();
            form1.Show();
        }

    }
}