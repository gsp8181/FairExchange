using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace FEClient.Security
{
    public static class Sha1
    {
        public static string HashJObject(JObject realData)
        {
            if(realData == null)
                throw new ArgumentNullException("realData");

            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(realData.ToString()));
                var hashStr = Convert.ToBase64String(hash);
                return hashStr;
            }
            
        }

        internal static object HashString(string p)
        {
            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(p));
                var hashStr = Convert.ToBase64String(hash);
                return hashStr;
            }
        }
    }
}
