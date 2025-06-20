using Backend.Models;
using Backend.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.Interfaces
{
    public class FollowService : IFollowService
    {
        private readonly MiniSocialContext _context;
        public FollowService(MiniSocialContext context, IConfiguration configuration)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<GetFollowResponse>>> GetAsync(int userId)
        {
            var queryFollow = from f in _context.Follows
                              join u in _context.Users on f.FollowingId equals u.Id
                              where f.FollowerId == userId && f.Status == (int)FollowStatus.Accepted
                              select new GetFollowResponse
                              {
                                  UserId = u.Id,
                                  Username = u.Username
                              };
            var resultFollow = await queryFollow.ToListAsync();

            return new ApiResponse<List<GetFollowResponse>> { Data = resultFollow, Message = "Get Follow successfully.", StatusCode = HttpStatusCode.OK };
        }

        public async Task<ApiResponse> RequestFollowAsync(RequestFollowRequest request)
        {
            if (request.FollowerId == request.FollowingId)
                return new ApiResponse { Message = "Unable to follow myself", StatusCode = HttpStatusCode.BadRequest };

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var followModel = new Follow
                {
                    FollowerId = request.FollowerId,
                    FollowingId = request.FollowingId,
                    Status = (int)FollowStatus.Pending,
                    CreatedAt = DateTime.Now
                };
                await _context.Follows.AddAsync(followModel);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ApiResponse { Message = "Follow successfully.", StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponse { Message = $"An error occurred: {ex.Message}", StatusCode = HttpStatusCode.InternalServerError };
            }

        }

        public async Task<ApiResponse> AcceptAsync(AcceptRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var followModel = await _context.Follows.SingleAsync(x => x.Id == request.FollowId);
                if (request.Status == (int)FollowStatus.Accepted)
                {
                    followModel.Status = (int)FollowStatus.Accepted;
                    followModel.UpdatedAt = DateTime.Now;
                }
                else if (request.Status == (int)FollowStatus.Rejected)
                {
                    followModel.Status = (int)FollowStatus.Rejected;
                    followModel.UpdatedAt = DateTime.Now;
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ApiResponse { Message = "Follow successfully.", StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponse { Message = $"An error occurred: {ex.Message}", StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        public async Task<ApiResponse<List<UserResponse>>> GetContactAsync(int userId)
        {
            var query = (from c in _context.Conversations
                         join cuSelf in _context.ConversationUsers on c.Id equals cuSelf.ConversationId
                         where cuSelf.UserId == userId && !c.IsGroup

                         join cuOther in _context.ConversationUsers on c.Id equals cuOther.ConversationId
                         where cuOther.UserId != userId

                         join u in _context.Users on cuOther.UserId equals u.Id

                         join m in _context.Messages on c.Id equals m.ConversationId into messageGroup
                         let lastMessage = messageGroup
                             .OrderByDescending(m => m.CreatedAt)
                             .FirstOrDefault()

                         where lastMessage != null

                         orderby lastMessage.CreatedAt descending

                         select new UserResponse
                         {
                             Id = u.Id,
                             Username = u.Username,
                         });
            var result = await query.Take(10).ToListAsync();

            return new ApiResponse<List<UserResponse>> { Data = result, Message = "Get Contact successfully.", StatusCode = HttpStatusCode.OK };
        }
    }
}