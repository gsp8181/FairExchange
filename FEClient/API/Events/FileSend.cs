using System;

namespace FEClient.API.Events
{
    public class FileSendEventArgs : EventArgs //TODO: make immutable
    {
        public FileSendEventArgs(string filename, string email, string data, string iv, string guid, string signature)
        {
            FileName = filename;
            Email = email;
            Data = data;
            Iv = iv;
            Guid = guid;
            Signature = signature;
        }

        public string FileName { get; private set; }
        public string Email { get; private set; }
        public string Data { get; private set; }
        public string Iv { get; private set; }
        public string Guid { get; private set; }
        public string Signature { get; private set; }
        public bool HasSet { get; set; }
    }
}