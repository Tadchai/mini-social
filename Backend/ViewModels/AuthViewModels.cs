namespace Backend.ViewModels
{
    public enum PasswordVerificationResult
    {
        Success,
        Failed,
    }
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class RegisterRequest : LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}