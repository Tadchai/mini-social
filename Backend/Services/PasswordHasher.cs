using System.Security.Cryptography;
using System.Text;
using Backend.Services.Interfaces;
using Backend.ViewModels;

namespace Backend.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password, string salt)
        {
            using var sha256 = SHA256.Create();
            var combined = password + salt;
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
            return Convert.ToHexString(hash).ToLower();
        }

        public int VerifyPassword(string password, string salt, string hashedPassword)
        {
            var hashOfInput = HashPassword(password, salt);
            return hashOfInput == hashedPassword
            ? (int)PasswordVerificationResult.Success
            : (int)PasswordVerificationResult.Failed;
        }
    }
}