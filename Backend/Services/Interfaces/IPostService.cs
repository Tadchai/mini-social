using Backend.ViewModels;

namespace Backend.Services.Interfaces
{
    public interface IPostService
    {
        Task<PagedResponse<PostResponse>> GetByIdAsync(int userId, DateTime? lastCreatedAt, int? lastId, int pageSize);
        Task<PagedResponse<PostResponse>> GetByFollowAsync(int userId, DateTime? lastCreatedAt, int? lastId, int pageSize);
        Task<ApiResponse> CreatePostAsync(CreatePostRequest request, int userId, HttpRequest httpRequest);
        Task<ApiResponse> UpdatePostAsync(UpdatePostRequest request);
        Task<ApiResponse> DeletePostAsync(DeletePostRequest request);
    }
}