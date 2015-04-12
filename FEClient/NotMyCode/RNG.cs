using System;
using System.Security.Cryptography;

namespace FEClient.NotMyCode
{
    public static class Rng
    {
        public static int NextInt(int min, int max) //http://www.vcskicks.com/code-snippet/rng-int.php
        {
            var rng = new RNGCryptoServiceProvider();
            var buffer = new byte[4];

            rng.GetBytes(buffer);
            var result = BitConverter.ToInt32(buffer, 0);

            return new Random(result).Next(min, max);
        }
    }
}