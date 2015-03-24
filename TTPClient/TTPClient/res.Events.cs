using Grapevine.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTPClient
{
    public sealed partial class MyResource : RESTResource
    {
        public delegate void NotifyHandler(object sender, string myValue);
        public static event NotifyHandler Notify = delegate { };

        public delegate void FileRecievedHandler(object sender, string fileName);
        public static event FileRecievedHandler FileRecieved = delegate { };
    }
}
