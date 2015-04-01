using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TTPClient.Security
{
    public class Aes
    {
        public static string Decrypt(string payload, byte[] aeskey, byte[] aesiv)
        {
            AesCryptoServiceProvider aesCSP = new AesCryptoServiceProvider();

            aesCSP.Key = aeskey;
            aesCSP.IV = aesiv;

            var encrypted = Convert.FromBase64String(payload);


            ICryptoTransform xfrm = aesCSP.CreateDecryptor();
            byte[] outBlock = xfrm.TransformFinalBlock(encrypted, 0, encrypted.Length);

            return Encoding.UTF8.GetString(outBlock);
        }

        public static AesData Encrypt(string data)
        {
            AesCryptoServiceProvider aesCSP = new AesCryptoServiceProvider();
            byte[] inBlock = Encoding.UTF8.GetBytes(data);
            ICryptoTransform xfrm = aesCSP.CreateEncryptor();
            byte[] outBlock = xfrm.TransformFinalBlock(inBlock, 0, inBlock.Length);
            string encrypted = Convert.ToBase64String(outBlock);
            AesKeys ae = new AesKeys();
            ae.Key = aesCSP.Key;
            ae.IV = aesCSP.IV;

            AesData ad = new AesData();
            ad.Key = ae;
            ad.DataStr = encrypted;
            return ad;
        }

        public static string Decrypt(string payload, string aeskey, string aesiv)
        {
            var aesKeyBytes = Convert.FromBase64String(aeskey);
            var aesIvBytes = Convert.FromBase64String(aesiv);
            return Decrypt(payload, aesKeyBytes, aesIvBytes);
        }
    }
}
