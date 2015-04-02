using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TTPClient
{
    interface ITrackerMethods
    {
        string getIp(string email);
        string getEmail(string ip);
        string getPublicKey(string email);
        void sendPem();

    }
}
