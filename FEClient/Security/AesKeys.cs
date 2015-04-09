using System;
using Newtonsoft.Json.Linq;

namespace FEClient.Security
{
    public class AesKeys
    {
        public string KeyStr
        {
            get { return Convert.ToBase64String(Key); }
            set { Key = Convert.FromBase64String(value); }
        }

        public string IvStr
        {
            get { return Convert.ToBase64String(Iv); }
            set { Key = Convert.FromBase64String(value); }
        }

        public int Rounds { get; set; }

        public byte[] Key { get; set; }

        public byte[] Iv { get; set; }

        public JObject ToJObject()
        {
            return new JObject
            {
                {"key", KeyStr},
                {"iv", IvStr}, //TODO: nono make this a serialisable object
                {"rounds", Rounds}
            };
        }

        public override string ToString()
        {
            return ToJObject().ToString();
        }
    }
}
