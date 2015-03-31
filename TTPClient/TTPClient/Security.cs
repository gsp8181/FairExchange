﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace TTPClient
{
    static class Security
    {
        private static CspParameters csparams = new CspParameters(13) { KeyContainerName = "ttpclient" };
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
            using (var rsa = new DSACryptoServiceProvider(1024, csparams))
            {
                //string publicPrivateKeyXML = rsa.ToXmlString(true);
              // string publicOnlyKeyXML = rsa.ToXmlString(false);
                AsymmetricCipherKeyPair dsaKey = DotNetUtilities.GetDsaKeyPair(rsa);
                StringWriter sw = new StringWriter();
                PemWriter pw = new PemWriter(sw);
                pw.WriteObject(dsaKey.Public);
                String rsakeypem = sw.ToString();
                return rsakeypem;
            }


        }

        public static void Regenerate_DSA()
        {
            using (var rsa = new DSACryptoServiceProvider(1024, csparams))
            {
                rsa.PersistKeyInCsp = false;
                rsa.Clear();
            }
            using (var rsa = new DSACryptoServiceProvider(1024, csparams))
            {
                rsa.PersistKeyInCsp = true;
            }
        }

        public static string SignData(string data)
        {
            using (var rsa = new DSACryptoServiceProvider(1024, csparams))
            {
                return rsa.SignData(Encoding.UTF8.GetBytes(data)).ToString();
            }
        }

        public static string EncryptData(string data)
        {
            using (var rsa = new DSACryptoServiceProvider(1024, csparams))
            {
                throw new NotImplementedException();
            }
        }
    }
}
