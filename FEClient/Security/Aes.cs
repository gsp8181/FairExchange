using System;
using System.Security.Cryptography;
using System.Text;

namespace FEClient.Security
{
    public class Aes
    {

        public static string Decrypt(string payload, byte[] aeskey, byte[] aesiv, int rounds) //TODO: needs some validity method like check against sig?
        {
            using (AesCryptoServiceProvider aesCSP = new AesCryptoServiceProvider())
            { 
            aesCSP.Key = Strengthen(aeskey, aesiv, rounds);
            aesCSP.IV = aesiv;

            var encrypted = Convert.FromBase64String(payload);


            ICryptoTransform xfrm = aesCSP.CreateDecryptor();
            byte[] outBlock = xfrm.TransformFinalBlock(encrypted, 0, encrypted.Length);

            return Encoding.UTF8.GetString(outBlock);
            }
        }

        public static AesData Encrypt(string data, int rounds)
        {
            using (AesCryptoServiceProvider aesCSP = new AesCryptoServiceProvider())
            { 
            aesCSP.GenerateIV();
            aesCSP.GenerateKey();
            AesKeys ae = new AesKeys();
            ae.Key = aesCSP.Key;
            ae.IV = aesCSP.IV;
            ae.Rounds = rounds; //TODO: embed the parameter sent through rest as THIS instead, probably when RSA takes shape


            aesCSP.Key = Strengthen(aesCSP.Key, aesCSP.IV, rounds);



            byte[] inBlock = Encoding.UTF8.GetBytes(data);
            ICryptoTransform xfrm = aesCSP.CreateEncryptor();
            byte[] outBlock = xfrm.TransformFinalBlock(inBlock, 0, inBlock.Length);
            string encrypted = Convert.ToBase64String(outBlock);
            AesData ad = new AesData();
            ad.Key = ae;
            ad.DataStr = encrypted;
            return ad;
            }
        }

        private static byte[] Strengthen(byte[] p, byte[] s, int rounds)
        {
            Rfc2898DeriveBytes rfcKey = new Rfc2898DeriveBytes(p, s, rounds);
            
            return rfcKey.GetBytes(p.Length);
        }


        public static string Decrypt(string payload, string aeskey, string aesiv, int rounds)
        {
            var aesKeyBytes = Convert.FromBase64String(aeskey);
            var aesIvBytes = Convert.FromBase64String(aesiv);
            return Decrypt(payload, aesKeyBytes, aesIvBytes, rounds);
        }
    }
}
