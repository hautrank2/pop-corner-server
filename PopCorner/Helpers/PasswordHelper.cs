using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography;

namespace PopCorner.Helpers
{
    public class PasswordHelper
    {
        private const int SaltSize = 16;
        private const int HashSize = 20;
        private const int DefaultIterations = 100000;

        public static string Hash(string password, int iterations = DefaultIterations)
        {
            // Create salt
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] key = KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: iterations,
                numBytesRequested: HashSize);

            return $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        }

        public static bool Verify(string password, string hashedPassword)
        {
            if (password == null) return false;
            if (string.IsNullOrWhiteSpace(hashedPassword)) return false;

            var parts = hashedPassword.Split('.');
            int iterations = int.Parse(parts[0]);
            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] storedHash = Convert.FromBase64String(parts[2]);

            byte[] computedHash = KeyDerivation.Pbkdf2(
                 password: password!,
                 salt: salt,
                 prf: KeyDerivationPrf.HMACSHA256,
                 iterationCount: iterations,
                 numBytesRequested: storedHash.Length
             );

            return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
        }
    }
}
