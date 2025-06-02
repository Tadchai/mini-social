namespace Backend.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(string userId);
    }
}