using System;

namespace FEClient.API
{
    [Obsolete]
    public class FileSend
    {
        public string FileName { get; set; }
        public string Email { get; set; }
        public string Data { get; set; }
        public string Iv { get; set; }
        public string Guid { get; set; }
        public string Signature { get; set; }
    }
}