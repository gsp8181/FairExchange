using System;
using System.Security.Cryptography;

namespace FEClient.Security
{
    public class Aes
    {
        public static byte[] Decrypt(string payload, byte[] aeskey, byte[] aesiv, int rounds)
            //TODO: needs some validity method like check against sig?
        {
            using (var aesCsp = new AesCryptoServiceProvider())
            {
                aesCsp.Key = Strengthen(aeskey, aesiv, rounds);
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
                var ae = new AesKeys();
                ae.Key = aesCsp.Key;
                ae.Iv = aesCsp.IV;
                ae.Rounds = rounds;
                    //TODO: embed the parameter sent through rest as THIS instead, probably when RSA takes shape


                aesCsp.Key = Strengthen(aesCsp.Key, aesCsp.IV, rounds);


                //byte[] inBlock = Encoding.UTF8.GetBytes(data);
                var xfrm = aesCsp.CreateEncryptor();
                var outBlock = xfrm.TransformFinalBlock(data, 0, data.Length);
                var encrypted = Convert.ToBase64String(outBlock);
                var ad = new AesData();
                ad.Key = ae;
                ad.DataStr = encrypted;
                return ad;
            }
        }

        private static byte[] Strengthen(byte[] p, byte[] s, int rounds)
        {
            var rfcKey = new Rfc2898DeriveBytes(p, s, rounds);

            return rfcKey.GetBytes(p.Length);
        }

        public static byte[] Decrypt(string payload, string aeskey, string aesiv, int rounds)
        {
            var aesKeyBytes = Convert.FromBase64String(aeskey);
            var aesIvBytes = Convert.FromBase64String(aesiv);
            return Decrypt(payload, aesKeyBytes, aesIvBytes, rounds);
        }
    }
}