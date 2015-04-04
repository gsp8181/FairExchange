using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace TTPClient
{
    public class FileSend
    {
        public string fileName { get; set; }
        public string email { get; set; }
        public string data { get; set; }
        public string iv { get; set; }
        public string guid { get; set; }
    }
}
