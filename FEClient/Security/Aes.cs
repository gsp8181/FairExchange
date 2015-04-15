using System;
using System.Security.Cryptography;

namespace FEClient.Security
{
    public static class Aes
    {
        public static byte[] Decrypt(string payload, byte[] aeskey, byte[] aesiv, int rounds)
            //TODO: needs some validity method like check against sig?
        {
            using (var aesCsp = new AesCryptoServiceProvider())
            {
                if(rounds > 0)
                { 
                aesCsp.Key = Strengthen(aeskey, aesiv, rounds);
                }
                else
                {
                    aesCsp.Key = aeskey;
                }
                aesCsp.IV = aesiv;

                var encrypted = Convert.FromBase64String(payload);


                var xfrm = aesCsp.CreateDecryptor();
                var outBlock = xfrm.TransformFinalBlock(encrypted, 0, encrypted.Length);

                return outBlock;
            }
        }

        public static AesData Encrypt(byte[] data, int rounds)
        {
            using (var aesCsp = new AesCryptoServiceProvider())
            {
                aesCsp.GenerateIV();
                aesCsp.GenerateKey();
                var ae = new AesKeys(aesCsp.Key, aesCsp.IV) {Rounds = rounds};
                //TODO: embed the parameter sent through rest as THIS instead, probably when RSA takes shape

                if(rounds > 0)
                    aesCsp.Key = Strengthen(aesCsp.Key, aesCsp.IV, rounds);


                //byte[] inBlock = Encoding.UTF8.GetBytes(data);
                var xfrm = aesCsp.CreateEncryptor();
                var outBlock = xfrm.TransformFinalBlock(data, 0, data.Length);
                var encrypted = Convert.ToBase64String(outBlock);
                var ad = new AesData {Key = ae, DataStr = encrypted};
                return ad;
            }
        }

        private static byte[] Strengthen(byte[] p, byte[] s, int rounds)
        {
            using(var rfcKey = new Rfc2898DeriveBytes(p, s, rounds))
            { 
                return rfcKey.GetBytes(p.Length);
            }
        }

        public static byte[] Decrypt(string payload, string aeskey, string aesiv, int rounds)
        {
            var aesKeyBytes = Convert.FromBase64String(aeskey);
            var aesIvBytes = Convert.FromBase64String(aesiv);
            return Decrypt(payload, aesKeyBytes, aesIvBytes, rounds);
        }
    }
}