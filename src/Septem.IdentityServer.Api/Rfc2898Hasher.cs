using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Septem.IdentityServer.Api
{
    public class Rfc2898Hasher
    {
        public static string ComputeSaltedHash(string password, string salt)
        {
            using var hasher = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt));
            return Convert.ToBase64String(hasher.GetBytes(256 / 8));
        }

        public static bool VerifyHashes(string hashA, string hashB)
        {
            var digestA = Convert.FromBase64String(hashA);
            var digestB = Convert.FromBase64String(hashB);

            if (digestA.Length != digestB.Length) return false;

            var result = 0;
            for (var i = 0; i < digestA.Length; i++)
            {
                result |= digestA[i] ^ digestB[i];
            }
            return result == 0;
        }

        public static string GenerateSalt() => Generate64String(128 / 8);

        private static string Generate64String(int arrayLength)
        {
            var salt = new byte[arrayLength];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }
    }
}
