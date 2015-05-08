using System;

namespace FEClient.API.Events
{
    public class KeyReceivedEventArgs : EventArgs
    {
        public KeyReceivedEventArgs(string key, string guid, int number, string ip, string strArgs)
        {
            Key = key;
            Guid = guid;
            Number = number;
            IP = ip;
            StrArgs = strArgs;
        }

        public string Key { get; private set; }
        public string Guid { get; private set; }
        public int Number { get; private set; }
        public bool HasSet { get; set; }
        public string StrArgs { get; private set; }
        public string IP { get; set; }
    }
}