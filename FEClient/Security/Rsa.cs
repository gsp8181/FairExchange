using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace FEClient.Security
{
    public static class Rsa
    {
        private static readonly CspParameters CsParams = new CspParameters(1) {KeyContainerName = "ttpclient"};
        //, KeyNumber = 1};

        public static string PublicKey
        {
            get
            {
                using (var rsa = new RSACryptoServiceProvider(2048, CsParams))
                {
                    var dsaKey = DotNetUtilities.GetRsaKeyPair(rsa);
                    using (var sw = new StringWriter())
                    {
                        var pw = new PemWriter(sw);
                        pw.WriteObject(dsaKey.Public);
                        var rsakeypem = sw.ToString();
                        return rsakeypem;
                    }
                }
            }
        }

#if DEBUG
        public static string PrivateKey
        {
            get
            {
                using (var rsa = new RSACryptoServiceProvider(2048, CsParams))
                {
                    var dsaKey = DotNetUtilities.GetRsaKeyPair(rsa);
                    using (var sw = new StringWriter())
                    {
                        var pw = new PemWriter(sw);
                        pw.WriteObject(dsaKey.Private);
                        var rsakeypem = sw.ToString();
                        return rsakeypem;
                    }
                }
            }
        }
#endif

        public static void RegenerateRsa()
        {
            using (var rsa = new RSACryptoServiceProvider(2048, CsParams))
            {
                rsa.PersistKeyInCsp = false;
                rsa.Clear();
            }
            using (var rsa = new RSACryptoServiceProvider(2048, CsParams))
            {
                rsa.PersistKeyInCsp = true;
            }
        }

        public static EncryptedData EncryptData(string data, RSACryptoServiceProvider rsa, int rounds)
        {
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var ad = Aes.Encrypt(dataBytes, rounds);

            var keyStr = ad.Key.ToString();

            var encryptedKey = EncryptKey(rsa, keyStr);

            var output = new EncryptedData {Data = ad.Data, Key = encryptedKey};
            return output;
        }

        public static bool VerifySignature(string data, string signatureB64, string pubKey)
        {
            var rsaKeyInfo = GetPublicKeyParams(pubKey);

            return VerifyStringSignature(data, signatureB64, rsaKeyInfo);
        }

        private static bool VerifyStringSignature(string data, string signatureB64, RSAParameters rsaKeyInfo)
        {
            if(string.IsNullOrWhiteSpace(data) || string.IsNullOrWhiteSpace(signatureB64))
            {
                return false;
            }
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var signatureBytes = Convert.FromBase64String(signatureB64);

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportParameters(rsaKeyInfo);
                //return false;
                using (var sha1 = new SHA1CryptoServiceProvider())
                {
                    return rsa.VerifyData(dataBytes, sha1, signatureBytes);
                }
            }
        }

        public static string SignStringData(string data)
        {
            using (var rsa = new RSACryptoServiceProvider(2048, CsParams))
            {
                var bytes = Encoding.UTF8.GetBytes(data);
                using (var sha1 = new SHA1CryptoServiceProvider())
                {
                    var signature = rsa.SignData(bytes, sha1);
                    return Convert.ToBase64String(signature);
                }
            }
        }

        private static string EncryptKey(RSACryptoServiceProvider rsa, string keyStr)
        {
            var encryptedKeyBytes = rsa.Encrypt(Encoding.UTF8.GetBytes(keyStr), false);
            var encryptedKey = Convert.ToBase64String(encryptedKeyBytes);
            return encryptedKey;
        }

        /*private static string EncryptKey(string pemKey, string keyStr)
        {
            var rsaKeyInfo = GetPublicKeyParams(pemKey);

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportParameters(rsaKeyInfo);
                return EncryptKey(rsa, keyStr);
            }
        }*/

        public static EncryptedData EncryptData(string data, string key, int rounds)
        {
            var rsaKeyInfo = GetPublicKeyParams(key);

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.PersistKeyInCsp = false;
                rsa.ImportParameters(rsaKeyInfo);
                return EncryptData(data, rsa, rounds);
            }
        }

        private static RSAParameters GetPublicKeyParams(string pemKey)
        {
            using (var keyStream = new MemoryStream(Encoding.UTF8.GetBytes(pemKey ?? "")))
            {
                var keyStreamReader = new StreamReader(keyStream);
                var pemRead = new PemReader(keyStreamReader);
                var keyParameter = (AsymmetricKeyParameter) pemRead.ReadObject();
                var rsaKeyParameters = (RsaKeyParameters) keyParameter;

                var rsaKeyInfo = DotNetUtilities.ToRSAParameters(rsaKeyParameters);
                return rsaKeyInfo;
            }
        }

        public static byte[] DecryptData(string payload, string key)
        {
            var keyStr = DecryptKey(key);
            var keyObj = JObject.Parse(keyStr);

            var aeskey = keyObj.Value<string>("key");
            var aesiv = keyObj.Value<string>("iv");
            var rounds = keyObj.Value<int>("rounds");

            //var payloadBytes = Convert.FromBase64String(payload);

            return Aes.Decrypt(payload, aeskey, aesiv, rounds);
        }

        public static string DecryptKey(string encryptedKey)
        {
            using (var rsa = new RSACryptoServiceProvider(2048, CsParams))
            {
                var keyBytes = Convert.FromBase64String(encryptedKey);
                var decryptedKey = Encoding.UTF8.GetString(rsa.Decrypt(keyBytes, false));
                return decryptedKey;
            }
        }
    }
}