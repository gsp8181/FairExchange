using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTPClient
{
    public class NotifyRequest
    {
        public string fileName { get; set; }
        public string email { get; set; }
        public string ip { get; set; }
        public string guid { get; set; }
    }
}
