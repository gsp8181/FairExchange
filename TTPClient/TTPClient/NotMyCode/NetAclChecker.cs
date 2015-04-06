using System;
using System.Diagnostics;

namespace FEClient.NotMyCode
{
    public static class NetAclChecker //http://stackoverflow.com/questions/2583347/c-sharp-httplistener-without-using-netsh-to-register-a-uri
    {
        public static void AddAddress(string address)
        {
            AddAddress(address, Environment.UserDomainName, Environment.UserName);
        }

        public static void AddAddress(string address, string domain, string user)
        {
            string args = string.Format(@"http add urlacl url={0} user={1}\{2}", address, domain, user); //TODO: check urlacl first

            ProcessStartInfo psi = new ProcessStartInfo("netsh", args)
            {
                Verb = "runas",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true
            };

            Process.Start(psi).WaitForExit();
        }
    }
}
