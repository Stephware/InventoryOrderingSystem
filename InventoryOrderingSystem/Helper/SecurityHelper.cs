using System.Security.Cryptography;

namespace InventoryOrderingSystem.Helper
{
    public class SecurityHelper
    {
        private const int saltSize = 16; // 128 bits
        private const int hashSize = 32; // 256 bits
        private const int iteration = 10000;

        // 🔐 HASH PASSWORD
        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(saltSize);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iteration,
                HashAlgorithmName.SHA256,
                hashSize
            );

            byte[] hashBytes = new byte[saltSize + hashSize];

            Array.Copy(salt, 0, hashBytes, 0, saltSize);
            Array.Copy(hash, 0, hashBytes, saltSize, hashSize);

            return Convert.ToBase64String(hashBytes);
        }

        // 🔍 VERIFY PASSWORD
        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            // ✔ decode the STORED hash (NOT the entered password)
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // ✔ extract salt
            byte[] salt = new byte[saltSize];
            Array.Copy(hashBytes, 0, salt, 0, saltSize);

            // ✔ hash the entered password using same salt
            byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(
                enteredPassword,
                salt,
                iteration,
                HashAlgorithmName.SHA256,
                hashSize
            );

            // ✔ compare securely
            return CryptographicOperations.FixedTimeEquals(
                computedHash,
                hashBytes.AsSpan(saltSize, hashSize)
            );
        }
    }
}