﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace FEClient.Security
{
    public class AesKeys
    {
        public string keyStr
        {
            get { return Convert.ToBase64String(Key); }
            set { Key = Convert.FromBase64String(value); }
        }

        public string ivStr
        {
            get { return Convert.ToBase64String(IV); }
            set { Key = Convert.FromBase64String(value); }
        }

        public int Rounds { get; set; }

        public byte[] Key { get; set; }

        public byte[] IV { get; set; }

        public JObject ToJObject()
        {
            return new JObject
            {
                {"key", keyStr},
                {"iv", ivStr}, //TODO: nono make this a serialisable object
                {"rounds", Rounds}
            };
        }

        public override string ToString()
        {
            return ToJObject().ToString();
        }
    }
}
