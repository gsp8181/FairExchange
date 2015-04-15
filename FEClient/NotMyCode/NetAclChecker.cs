using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace FEClient.NotMyCode
{
    public static class NetAclChecker
        //http://stackoverflow.com/questions/2583347/c-sharp-httplistener-without-using-netsh-to-register-a-uri
    {
        public static void AddAddress(string address)
        {
            AddAddress(address, Environment.UserDomainName, Environment.UserName);
        }

        public static void AddAddress(string address, string domain, string user)
        {
            var args = string.Format(@"http add urlacl url={0} user={1}\{2}", address, domain, user);
            //TODO: check urlacl first

            var psi = new ProcessStartInfo("netsh", args)
            {
                Verb = "runas",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true
            };

            Process.Start(psi).WaitForExit();
        }

        public static void CreateFirewallException(int port) //Stackoverflow how to diusplay windows firewall has blocked some features of this program; 
        {
            var ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
            var ipLocalEndPoint = new IPEndPoint(ipAddress, port);

            var t = new TcpListener(ipLocalEndPoint);
            t.Start();
            t.Stop();
        }
    }
}