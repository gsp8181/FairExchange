namespace FEClient.API
{
    public class NotifyRequest
    {
        public string fileName { get; set; }
        public string email { get; set; }
        public string ip { get; set; }
        public string guid { get; set; }

        public int timeout { get; set; }

        public int complexity { get; set; }
    }
}
