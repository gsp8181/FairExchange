using System;

namespace FEClient.API.Events
{
    public class KeyReceivedEventArgs : EventArgs
    {
        public KeyReceivedEventArgs(string key, string guid, int i)
        {
            Key = key;
            Guid = guid;
            I = i;
        }

        public string Key { get; private set; }
        public string Guid { get; private set; }
        public int I { get; private set; }
        public bool HasSet { get; set; }
    }
}