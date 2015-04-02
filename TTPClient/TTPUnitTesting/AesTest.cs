using System;
using System.Security.Permissions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using TTPClient.Security;

namespace TTPUnitTesting
{
    [TestClass]
    public class AesTest
    {
        private const string TestString = "abcd";

        [TestMethod]
        public void Encryption()
        {
            var encrypt = Aes.Encrypt(TestString);
            Assert.AreNotEqual(TestString, encrypt);

            var decrypt = Aes.Decrypt(encrypt.DataStr, encrypt.Key.Key, encrypt.Key.IV);
            Assert.AreEqual(decrypt, TestString);

        }

        [TestMethod]
        public void KeyTest()
        {
            var encrypt = Aes.Encrypt(TestString);
            Assert.AreEqual(encrypt.Key.keyStr, Convert.ToBase64String(encrypt.Key.Key));
            Assert.AreEqual(encrypt.Key.ivStr, Convert.ToBase64String(encrypt.Key.IV));
            Assert.AreEqual(encrypt.Key.ToString(), encrypt.Key.ToJObject().ToString());
            JObject jo = new JObject();
            jo.Add("key", encrypt.Key.keyStr);
            jo.Add("iv", encrypt.Key.ivStr);
            Assert.AreEqual(jo.ToString(),encrypt.Key.ToJObject().ToString());
        }
    }
}
