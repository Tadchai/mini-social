using Backend.ViewModels;

namespace Backend.Services.Interfaces
{
    public interface ISearchService
    {
        Task<ApiResponse<List<PostResponse>>> SearchPostsAsync(string q, string? cursor, int pageSize);
        Task<ApiResponse<List<UserResponse>>> SearchUsersAsync(string q, string? cursor, int pageSize);
    }
}