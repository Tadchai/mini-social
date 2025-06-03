using Backend.ViewModels;

namespace Backend.Services.Interfaces
{
    public interface IGroupService
    {
        Task<PagedResponse<MessageResponse>> GetMessagesAsync(int groupId, string? cursor, int pageSize);
        Task<ApiResponse> CreateGroupAsync(CreateGroupRequest request);
        Task<IdResponse> CreatePrivateAsync(CreatePrivateGroupRequest request, int currentUserId);
        Task<ApiResponse> UpdateGroupAsync(UpdateGroupRequest request);
    }
}