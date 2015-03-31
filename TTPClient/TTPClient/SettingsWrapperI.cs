using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TTPClient
{
    interface SettingsWrapperI
    {
        void setEmail(string email);
        void setTTP(string ttp);
        string loadTTP();
        string loadEmail();
        bool isSet();
        bool regWithTracker(string email, string tracker );
        bool regWithTracker();
    }
}
