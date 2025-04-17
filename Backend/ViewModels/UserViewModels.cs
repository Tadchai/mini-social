using System.ComponentModel.DataAnnotations;

namespace api.ViewModels
{
    public class RegisterRequest
    {
        public string Username { get; set;}
        public string Email { get; set;}
        public string Password { get; set;}
        public string confirmPassword { get; set;}
    }

    public class LoginRequest
    {
        public string Username { get; set;}
        public string Password { get; set;}
    }
}