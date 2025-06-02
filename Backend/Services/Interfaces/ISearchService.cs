using Backend.ViewModels;

namespace Backend.Services.Interfaces
{
    public interface ISearchService
    {
        Task<ApiResponse<List<PostResponse>>> SearchPostsAsync(string q, string? cursor);
    }
}