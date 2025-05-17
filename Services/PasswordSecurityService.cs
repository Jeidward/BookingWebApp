using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Interfaces.IServices;


namespace Services
{
    public class PasswordSecurityService : IPasswordSecurityService
    {
        private const int _keySize = 64;
        private const int _iterations = 350000;


        public string HashPassword(string password, out byte[] salt)
        {
            var hashAlgorithm = HashAlgorithmName.SHA512;
            salt = RandomNumberGenerator.GetBytes(_keySize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, _iterations, hashAlgorithm,
                _keySize);

            return Convert.ToBase64String(hash);
        }

        //for storedSalt
        public string HashPassword(string password, byte[] salt)
        {
            var hashAlgorithm = HashAlgorithmName.SHA512;
            var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, _iterations, hashAlgorithm, _keySize);
            return Convert.ToBase64String(hash);
        }

    }
}
