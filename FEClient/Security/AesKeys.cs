using System;
using Newtonsoft.Json.Linq;

namespace FEClient.Security
{
    public class AesKeys
    {
        private byte[] _iv;
        /*public byte[] Key { get; set; }
        public byte[] Iv { get; set; }*/
        private byte[] _key;

        public AesKeys(byte[] key, byte[] iv)
        {
            _key = key;
            _iv = iv;
        }

        public string KeyStr
        {
            get { return Convert.ToBase64String(_key); }
            set { _key = Convert.FromBase64String(value); }
        }

        public string IvStr
        {
            get { return Convert.ToBase64String(_iv); }
            set { _iv = Convert.FromBase64String(value); }
        }

        public int Rounds { get; set; }

        public byte[] Key()
        {
            return _key;
        }

        public byte[] Iv()
        {
            return _iv;
        }

        public void Key(byte[] key)
        {
            _key = key;
        }

        public void Iv(byte[] iv)
        {
            _iv = iv;
        }

        public JObject ToJObject()
        {
            return new JObject
            {
                {"key", KeyStr},
                {"iv", IvStr},
                {"rounds", Rounds}
            };
        }

        public override string ToString()
        {
            return ToJObject().ToString();
        }
    }
}