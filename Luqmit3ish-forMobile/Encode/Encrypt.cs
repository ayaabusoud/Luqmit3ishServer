using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Luqmit3ish_forMobile.Encrypt
{
    public static class Encrypt
    {

        public static string EncryptPassword(string password)
        {
            // Generate a salt value
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            // Create a password hash using bcrypt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Combine the salt and hash into a single string
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string savedPasswordHash = Convert.ToBase64String(hashBytes);

            return savedPasswordHash;
        }

        public static bool VerifyPassword(string password, string savedPasswordHash)
        {
            // Retrieve the salt value from the stored hash
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Generate a hash value using the retrieved salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Compare the generated hash value with the stored hash value
            bool passwordMatches = hash.SequenceEqual(hashBytes.Skip(16).ToArray());
            return passwordMatches;
        }
    }
 
}
