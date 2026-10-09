using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecurityApplicationLogic.Services
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            int saltSize = 16; 
            int keySize = 32;
            int iterations = 100_000;

            byte[] salt = RandomNumberGenerator.GetBytes(saltSize);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password,salt,iterations,HashAlgorithmName.SHA256,keySize);

            return $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(storedHash))
                return false;

            if (storedHash.Split('.').Length != 3)
                return false;
            
            var parts = storedHash.Split('.');
            int iterations = int.Parse(parts[0]);
            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] storedKey = Convert.FromBase64String(parts[2]);

            byte[] computedKey = Rfc2898DeriveBytes.Pbkdf2(password,salt,iterations,HashAlgorithmName.SHA256,storedKey.Length);

            return CryptographicOperations.FixedTimeEquals(computedKey, storedKey);
        }
    }
}
