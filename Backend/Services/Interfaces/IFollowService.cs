using Backend.ViewModels;

namespace Backend.Services.Interfaces
{
    public interface IFollowService
    {
        Task<ApiResponse<List<GetFollowResponse>>> GetAsync(int userId);
        Task<ApiResponse> RequestFollowAsync(RequestFollowRequest request);
        Task<ApiResponse> AcceptAsync(AcceptRequest request);
        Task<ApiResponse<List<UserResponse>>> GetContactAsync(int userId);
    }
}