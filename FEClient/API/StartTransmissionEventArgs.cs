namespace FEClient.API
{
    public class StartTransmissionEventArgs
    {
        public StartTransmissionEventArgs(string fileName, string email, string ip, string guid, int timeout, int complexity)
        {
            FileName = fileName;
            Email = email;
            Ip = ip;
            Guid = guid;
            Timeout = timeout;
            Complexity = complexity;
        }
        public string FileName { get; private set; }
        public string Email { get; private set; }
        public string Ip { get; private set; }
        public string Guid { get; private set; }
        public int Timeout { get; private set; }
        public int Complexity { get; private set; }

        public StartTransmissionEventArgs (NotifyRequestEventArgs obj)
        {
            FileName = obj.FileName;
            Email = obj.Email;
            Ip = obj.Ip;
            Guid = obj.Guid;
            Timeout = obj.Timeout;
            Complexity = obj.Complexity;
        }

        public bool HasSet { get; set; }
    }
}
