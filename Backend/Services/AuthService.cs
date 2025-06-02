using Backend.Models;
using Backend.Services.Interfaces;
using Backend.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly MiniSocialContext _context;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(MiniSocialContext context, ITokenService tokenService, IPasswordHasher passwordHasher)
        {
            _context = context;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        public async Task<ApiResponse> RegisterAsync(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return new ApiResponse { Message = "Name is already in use.", StatusCode = HttpStatusCode.Conflict };

            if (request.Password != request.ConfirmPassword)
                return new ApiResponse { Message = "Passwords do not match.", StatusCode = HttpStatusCode.Conflict };

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var salt = Guid.NewGuid().ToString();
                var hashedPassword = _passwordHasher.HashPassword(request.Password, salt);
                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    Password = hashedPassword,
                    Salt = salt,
                    CreatedAt = DateTime.Now,
                };
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ApiResponse { Message = "User Created successfully.", StatusCode = HttpStatusCode.Created };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponse { Message = $"An error occurred: {ex.Message}", StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        public async Task<TokenResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == request.Username);
                if (user == null || _passwordHasher.VerifyPassword(request.Password, user.Salt, user.Password) == (int)PasswordVerificationResult.Failed)
                    return new TokenResponse { Message = "Invalid username or password.", StatusCode = HttpStatusCode.Unauthorized };

                var token = _tokenService.GenerateJwtToken(user.Id.ToString());
                return new TokenResponse { Data = token, Message = "Login successfully.", StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                return new TokenResponse { Message = $"An error occurred: {ex.Message}", StatusCode = HttpStatusCode.InternalServerError };
            }
        }
    }
}