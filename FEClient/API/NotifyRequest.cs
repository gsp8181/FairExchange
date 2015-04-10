namespace FEClient.API
{
    public class NotifyRequest
    {
        public string FileName { get; set; }
        public string Email { get; set; }
        public string Ip { get; set; }
        public string Guid { get; set; }
        public int Timeout { get; set; }
        public int Complexity { get; set; }
    }
}