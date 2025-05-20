using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Backend.ViewModels;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly MiniSocialContext _context;
        private readonly IConfiguration _configuration;
        public AuthController(MiniSocialContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        static string HashPassword(string password, string salt)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                string passwordWithSalt = password + salt;
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(passwordWithSalt));

                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        static int VerifyPassword(string enteredPassword, string storedSalt, string storedHashedPassword)
        {
            string hashedEnteredPassword = HashPassword(enteredPassword, storedSalt);

            return hashedEnteredPassword == storedHashedPassword ? (int)PasswordVerificationResult.Success : (int)PasswordVerificationResult.Failed;
        }

        private string GenerateJwtToken(string userId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var userModel = await _context.Users.AnyAsync(u => u.Username == request.Username);
            if (userModel)
                return new JsonResult(new ApiResponse { Message = "Name is already in use.", StatusCode = HttpStatusCode.Conflict });

            if (request.Password != request.confirmPassword)
                return new JsonResult(new ApiResponse { Message = "Password do not match.", StatusCode = HttpStatusCode.Conflict });

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var salt = Guid.NewGuid().ToString();
                    var hashPassword = HashPassword(request.Password, salt);
                    var user = new User
                    {
                        Username = request.Username,
                        Email = request.Email,
                        Password = hashPassword,
                        Salt = salt,
                        CreatedAt = DateTime.Now,
                    };
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new JsonResult(new ApiResponse { Message = "User Created successfully.", StatusCode = HttpStatusCode.Created });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new JsonResult(new ApiResponse { Message = $"An error occurred: {ex.Message}", StatusCode = HttpStatusCode.InternalServerError });
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var userModel = await _context.Users.SingleOrDefaultAsync(u => u.Username == request.Username);
                if (userModel == null)
                    return new JsonResult(new { Message = "Invalid username or password." });

                var verifyResult = VerifyPassword(request.Password, userModel.Salt, userModel.Password);
                if (verifyResult == (int)PasswordVerificationResult.Failed)
                    return new JsonResult(new { Message = "Invalid username or password." });

                var token = GenerateJwtToken(userModel.Id.ToString());

                return new JsonResult(new ApiWithTokenResponse { Token = token, Message = "Login successfully.", StatusCode = HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ApiResponse { Message = $"An error occurred: {ex.Message}", StatusCode = HttpStatusCode.InternalServerError });
            }
        }

    }
}