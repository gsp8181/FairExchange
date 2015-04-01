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

        public static EncryptedData EncryptData(string data, RSACryptoServiceProvider rsa) //TODO: this encrypts to self
        {
            var ad = AES.Encrypt(data);

            var keyStr = ad.Key.ToString();

            var encryptedKey = EncryptKey(rsa, keyStr);

            var output = new EncryptedData();
            output.Data = ad.DataStr;
            output.Key = encryptedKey;
            return output;


        }

        public static bool VerifySignature(string data, string signature, string pubKey)
        {
                var RSAKeyInfo = GetPublicKeyParams(pubKey);

                using (var rsa = new RSACryptoServiceProvider())
                {
                    rsa.PersistKeyInCsp = false;
                    rsa.ImportParameters(RSAKeyInfo);
                    return false;
                    //return rsa.VerifyData(data, rsa);
                }
        }

        public static string SignData(string data)
        {
            using (var rsa = new RSACryptoServiceProvider(2048, csparams))
            {
                var bytes = Encoding.UTF8.GetBytes(data);
                var signature = rsa.SignData(bytes, new SHA1CryptoServiceProvider());
                return Convert.ToBase64String(signature);
            }
        }

        private static string EncryptKey(RSACryptoServiceProvider rsa, string keyStr)
        {
            var encryptedKeyBytes = rsa.Encrypt(Encoding.UTF8.GetBytes(keyStr), false);
            var encryptedKey = Convert.ToBase64String(encryptedKeyBytes);
            return encryptedKey;
        }

        private static string EncryptKey(string pemKey, string keyStr)
        {
            var RSAKeyInfo = GetPublicKeyParams(pemKey);

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportParameters(RSAKeyInfo);
                return EncryptKey(rsa,keyStr);
            }
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
            var RSAKeyInfo = GetPublicKeyParams(key);

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportParameters(RSAKeyInfo);
                return EncryptData(data, rsa);
            }
        }

        private static RSAParameters GetPublicKeyParams(string pemKey)
        {
            var keyStream = new MemoryStream(Encoding.UTF8.GetBytes(pemKey ?? ""));
            var keyStreamReader = new StreamReader(keyStream);
            var pemRead = new PemReader(keyStreamReader);
            var keyParameter = (AsymmetricKeyParameter) pemRead.ReadObject();
            RsaKeyParameters rsaKeyParameters = (RsaKeyParameters) keyParameter;

            RSAParameters rsaKeyInfo = DotNetUtilities.ToRSAParameters(rsaKeyParameters);
            return rsaKeyInfo;
        }

        public static string DecryptData(string payload, string key)
        {
                var keyStr = DecryptKey(key);
                var keyObj = JObject.Parse(keyStr);

                var aeskey = keyObj.Value<string>("key");
                var aesiv = keyObj.Value<string>("iv");

                return AES.Decrypt(payload, aeskey, aesiv);
        }

        public static string DecryptKey(string encryptedKey)
        {
            using (var rsa = new RSACryptoServiceProvider(2048, csparams))
            {
                var keyBytes = Convert.FromBase64String(encryptedKey);
                var decryptedKey =  Encoding.UTF8.GetString(rsa.Decrypt(keyBytes, false));
                return decryptedKey;
            }
        }

        
    }
}
