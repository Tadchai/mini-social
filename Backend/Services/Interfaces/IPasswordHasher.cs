namespace Backend.Services.Interfaces
{
    public interface IPasswordHasher
    {
        string HashPassword(string password, string salt);
        int VerifyPassword(string password, string salt, string hashedPassword);
    }
}