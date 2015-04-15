namespace FEClient.Forms
{
    class Ident
    {
        public Ident(bool isSet, string remoteKey, string email)
        {
            IsSet = isSet;
            RemoteKey = remoteKey;
            Email = email;
        }
        public bool IsSet { get; private set; }
        public string RemoteKey { get; private set; }
        public string Email { get; private set; }
    }
}
