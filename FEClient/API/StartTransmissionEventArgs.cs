using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEClient.API
{
    public class StartTransmissionEventArgs : NotifyRequestEventArgs
    {
        public StartTransmissionEventArgs(string fileName, string email, string ip, string guid, int timeout, int complexity)
            : base(fileName, email, ip, guid, timeout, complexity)
        {
        }

        public bool HasSet { get; set; }
    }
}
