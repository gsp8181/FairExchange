using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FEClient.Security
{
    public class Aes
    {
        private const int ROUNDS = 16; //TODO:to user specified?? but this can't be static for reasons of thread safety

        public static string Decrypt(string payload, byte[] aeskey, byte[] aesiv, int rounds) //TODO: needs some validity method like check against sig?
        {
            AesCryptoServiceProvider aesCSP = new AesCryptoServiceProvider();

            aesCSP.Key = Strengthen(aeskey, rounds);
            aesCSP.IV = aesiv;

            var encrypted = Convert.FromBase64String(payload);


            ICryptoTransform xfrm = aesCSP.CreateDecryptor();
            byte[] outBlock = xfrm.TransformFinalBlock(encrypted, 0, encrypted.Length);

            return Encoding.UTF8.GetString(outBlock);
        }

        public static AesData Encrypt(string data, int rounds = ROUNDS)
        {
            AesCryptoServiceProvider aesCSP = new AesCryptoServiceProvider();
            aesCSP.GenerateIV();
            aesCSP.GenerateKey();
            AesKeys ae = new AesKeys();
            ae.Key = aesCSP.Key;
            ae.IV = aesCSP.IV;
            ae.Rounds = ROUNDS;

            aesCSP.Key = Strengthen(aesCSP.Key, rounds);

            byte[] inBlock = Encoding.UTF8.GetBytes(data);
            ICryptoTransform xfrm = aesCSP.CreateEncryptor();
            byte[] outBlock = xfrm.TransformFinalBlock(inBlock, 0, inBlock.Length);
            string encrypted = Convert.ToBase64String(outBlock);
            AesData ad = new AesData();
            ad.Key = ae;
            ad.DataStr = encrypted;
            return ad;
        }

        private static byte[] Strengthen(byte[] p, int rounds)
        {
            return p; //TODO: implement
        }


        public static string Decrypt(string payload, string aeskey, string aesiv, int rounds)
        {
            var aesKeyBytes = Convert.FromBase64String(aeskey);
            var aesIvBytes = Convert.FromBase64String(aesiv);
            return Decrypt(payload, aesKeyBytes, aesIvBytes, rounds);
        }
    }
}
