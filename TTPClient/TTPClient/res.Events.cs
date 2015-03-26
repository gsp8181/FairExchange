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
        //public delegate void NotifyHandler(object sender, string myValue);
        //public static event NotifyHandler Notify = delegate { };

        public delegate void FileRecievedHandler(object sender, string fileName);
        public static event FileRecievedHandler FileRecieved = delegate { };

        public delegate void NotifyRecievedHandler(object sender, NotifyRequest vars);
        public static event NotifyRecievedHandler NotifyRecieved = delegate { };

        public delegate void StartTransmissionHandler(object sender, NotifyRequest vars, NotifyArgs callbackArgs);
        public static event StartTransmissionHandler StartTransmission = delegate { };
    }
}
