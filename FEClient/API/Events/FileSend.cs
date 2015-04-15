using System;

namespace FEClient.API.Events
{
    public class FileSendEventArgs : EventArgs
    {
        public FileSendEventArgs(string fileName, string email, string data, string iv, string guid, string signature)
        {
            FileName = fileName;
            Email = email;
            Data = data;
            IV = iv;
            Guid = guid;
            Signature = signature;
        }

        public string FileName { get; private set; }
        public string Email { get; private set; }
        public string Data { get; private set; }
        public string IV { get; private set; }
        public string Guid { get; private set; }
        public string Signature { get; private set; }
        public bool HasSet { get; set; }
    }
}