using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace TTPClient
{
    static class Security
    {
        private static CspParameters csparams = new CspParameters(1) { KeyContainerName = "ttpclient" };
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

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

        public static JObject EncryptData(string data, RSACryptoServiceProvider rsa) //TODO: this encrypts to self
        {
                AesCryptoServiceProvider aesCSP = new AesCryptoServiceProvider();
                //aesCSP.GenerateKey();
                //aesCSP.GenerateIV();
                byte[] inBlock = Encoding.UTF8.GetBytes(data);
                ICryptoTransform xfrm = aesCSP.CreateEncryptor();
                byte[] outBlock = xfrm.TransformFinalBlock(inBlock, 0, inBlock.Length);

                string encrypted =  Convert.ToBase64String(outBlock);
                var keyStr = new JObject
                {
                    {"key", Convert.ToBase64String(aesCSP.Key)},
                    {"iv", Convert.ToBase64String(aesCSP.IV)} //TODO: nono
                }.ToString();
                var encryptedKeyBytes = rsa.Encrypt(Encoding.UTF8.GetBytes(keyStr), false);
                var encryptedKey = Convert.ToBase64String(encryptedKeyBytes);

                var output = new JObject();
                output.Add("data",encrypted);
                output.Add("key", encryptedKey);
                return output;


            }


        public static JObject EncryptData(string data) //encrypts with OWN KEY
        {
            using (var rsa = new RSACryptoServiceProvider(2048, csparams))
            {
                return EncryptData(data, rsa);
            }
        }

        public static JObject EncryptData(string data, string key)
        {
            var keyStream =  new MemoryStream(Encoding.UTF8.GetBytes(key ?? ""));
            var keyStreamReader = new StreamReader(keyStream);
            var pemRead = new PemReader(keyStreamReader);
            var keyParameter = (AsymmetricKeyParameter)pemRead.ReadObject();
            RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)keyParameter;

            RSAParameters RSAKeyInfo = DotNetUtilities.ToRSAParameters(rsaKeyParameters);

            //var val = (RsaKeyParameters)PublicKey.GetKey();
            //var ModulusVal = Encoding.UTF8.GetBytes(val.Modulus.ToString());
            //var ExponentVal = Encoding.UTF8.GetBytes(val.Exponent.ToString());

            //var RSAKeyInfo = new RSAParameters();
            //RSAKeyInfo.Modulus = ModulusVal;
            //RSAKeyInfo.Exponent = ExponentVal;

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

                AesCryptoServiceProvider aesCSP = new AesCryptoServiceProvider();

                var aeskey = keyObj.Value<string>("key");
                var aesiv = keyObj.Value<string>("iv");

                aesCSP.Key = Convert.FromBase64String(aeskey);
                aesCSP.IV = Convert.FromBase64String(aesiv);

                var encrypted = Convert.FromBase64String(payload);

                
                ICryptoTransform xfrm = aesCSP.CreateDecryptor();
                byte[] outBlock = xfrm.TransformFinalBlock(encrypted, 0, encrypted.Length);

                return Encoding.UTF8.GetString(outBlock);
            }
        }
    }
}
