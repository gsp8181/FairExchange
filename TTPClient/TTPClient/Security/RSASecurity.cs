using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace TTPClient.Security
{
    static class Rsa
    {
        private static CspParameters csparams = new CspParameters(1) { KeyContainerName = "ttpclient" };

        public static string getPublicKey()
        {
            using (var rsa = new RSACryptoServiceProvider(2048, csparams))
            {
                var dsaKey = DotNetUtilities.GetRsaKeyPair(rsa);
                var sw = new StringWriter();
                var pw = new PemWriter(sw);
                pw.WriteObject(dsaKey.Public);
                String rsakeypem = sw.ToString();
                return rsakeypem;
            }

        }

        public static void Regenerate_RSA()
        {
            using (var rsa = new RSACryptoServiceProvider(2048, csparams))
            {
                rsa.PersistKeyInCsp = false;
                rsa.Clear();
            }
            using (var rsa = new RSACryptoServiceProvider(2048, csparams))
            {
                rsa.PersistKeyInCsp = true;
            }
        }

        public static string SignData(string data)
        {
            using (var rsa = new RSACryptoServiceProvider(2048, csparams))
            {
                throw new NotImplementedException();
                //return rsa.SignData(Encoding.UTF8.GetBytes(data)).ToString();
            }
        }

        public static EncryptedData EncryptData(string data, RSACryptoServiceProvider rsa) //TODO: this encrypts to self
        {
            var ad = AES.Encrypt(data);

            var keyStr = ad.Key.ToString();

            var encryptedKeyBytes = rsa.Encrypt(Encoding.UTF8.GetBytes(keyStr), false);
            var encryptedKey = Convert.ToBase64String(encryptedKeyBytes);

            var output = new EncryptedData();
            output.Data = ad.DataStr;
            output.Key = encryptedKey;
            return output;


        }


        public static EncryptedData EncryptData(string data) //encrypts with OWN KEY
        {
            using (var rsa = new RSACryptoServiceProvider(2048, csparams))
            {
                return EncryptData(data, rsa);
            }
        }

        public static EncryptedData EncryptData(string data, string key)
        {
            var keyStream = new MemoryStream(Encoding.UTF8.GetBytes(key ?? ""));
            var keyStreamReader = new StreamReader(keyStream);
            var pemRead = new PemReader(keyStreamReader);
            var keyParameter = (AsymmetricKeyParameter)pemRead.ReadObject();
            RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)keyParameter;

            RSAParameters RSAKeyInfo = DotNetUtilities.ToRSAParameters(rsaKeyParameters);

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportParameters(RSAKeyInfo);
                return EncryptData(data, rsa);
            }
        }

        public static string DecryptData(string payload, string key)
        {
            using (var rsa = new RSACryptoServiceProvider(2048, csparams))
            {
                var keyBytes = Convert.FromBase64String(key);
                var keyStr = Encoding.UTF8.GetString(rsa.Decrypt(keyBytes, false));
                var keyObj = JObject.Parse(keyStr);

                var aeskey = keyObj.Value<string>("key");
                var aesiv = keyObj.Value<string>("iv");

                return AES.Decrypt(payload, aeskey, aesiv);
            }
        }

        
    }
}
