using System.ComponentModel.DataAnnotations;

namespace Backend.ViewModels
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}