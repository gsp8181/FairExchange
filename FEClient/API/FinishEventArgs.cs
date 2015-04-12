using System;

namespace FEClient.API
{
    public class FinishEventArgs : EventArgs
    {
        public FinishEventArgs(string guid)
        {
            Guid = guid;
        }
        public string Guid { get; private set; }
        public bool HasSet { get; set; }

    }
}
