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
        void setEmail();
        void setTTP();
        void loadTTP();
        void loadEmail();
        bool isSet();
        bool regWithTracker(string email, string tracker );
        bool regWithTracker();
    }
}
