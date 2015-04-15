using System;
using System.Security.Cryptography;

namespace FEClient.Security
{
    public static class Aes
    {
        public static byte[] Decrypt(string payload, byte[] key, byte[] iv, int rounds)
        {
            using (var aesCsp = new AesCryptoServiceProvider())
            {
                aesCsp.Key = rounds > 0 ? Strengthen(key, iv, rounds) : key;

                aesCsp.IV = iv;

                var encrypted = Convert.FromBase64String(payload);


                var xfrm = aesCsp.CreateDecryptor();
                var outBlock = xfrm.TransformFinalBlock(encrypted, 0, encrypted.Length);

                return outBlock;
            }
        }

        public static AesData Encrypt(byte[] data, int rounds)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            using (var aesCsp = new AesCryptoServiceProvider())
            {
                aesCsp.GenerateIV();
                aesCsp.GenerateKey();
                var ae = new AesKeys(aesCsp.Key, aesCsp.IV) {Rounds = rounds};
                
                if (rounds > 0)
                    aesCsp.Key = Strengthen(aesCsp.Key, aesCsp.IV, rounds);

                var xfrm = aesCsp.CreateEncryptor();
                var outBlock = xfrm.TransformFinalBlock(data, 0, data.Length);
                var encrypted = Convert.ToBase64String(outBlock);
                var ad = new AesData {Key = ae, Data = encrypted};
                return ad;
            }
        }

        private static byte[] Strengthen(byte[] key, byte[] salt, int rounds)
        {
            using (var rfcKey = new Rfc2898DeriveBytes(key, salt, rounds))
            {
                return rfcKey.GetBytes(key.Length);
            }
        }

        public static byte[] Decrypt(string payload, string key, string iv, int rounds)
        {
            var aesKeyBytes = Convert.FromBase64String(key);
            var aesIvBytes = Convert.FromBase64String(iv);
            return Decrypt(payload, aesKeyBytes, aesIvBytes, rounds);
        }
    }
}