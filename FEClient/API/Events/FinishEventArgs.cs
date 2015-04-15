using System;

namespace FEClient.API.Events
{
    public class FinishEventArgs : EventArgs
    {
        public FinishEventArgs(string guid, string ip)
        {
            Guid = guid;
            IP = ip;
        }

        public string Guid { get; private set; }
        public bool HasSet { get; set; }

        public string IP { get; private set; }
    }
}