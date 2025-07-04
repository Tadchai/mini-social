using Backend.Models;
using Backend.Services.Interfaces;
using Backend.ViewModels;
using Microsoft.EntityFrameworkCore;
using static Backend.Services.CursorService;

namespace Backend.Services
{
    public class GroupService : IGroupService
    {
        private readonly MiniSocialContext _context;
        private readonly ICursorService _cursorService;
        public GroupService(MiniSocialContext context, ICursorService cursorService)
        {
            _context = context;
            _cursorService = cursorService;
        }

        public async Task<ApiResponse> CreateGroupAsync(CreateGroupRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var GroupModel = new Conversation
                {
                    IsGroup = true,
                    Name = request.Name,
                    CreatedAt = DateTime.Now,
                };
                await _context.Conversations.AddAsync(GroupModel);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ApiResponse { Message = "Group Created successfully.", StatusCode = HttpStatusCode.Created };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponse { Message = $"An error occurred: {ex.Message}", StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        public async Task<IdResponse> CreatePrivateAsync(CreatePrivateGroupRequest request, int currentUserId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var queryConversation = _context.ConversationUsers
                                        .Where(cu => cu.UserId == currentUserId || cu.UserId == request.TargetUserId)
                                        .GroupBy(cu => cu.ConversationId)
                                        .Where(g =>
                                            g.Count() == 2 &&
                                            g.Any(cu => cu.UserId == currentUserId) &&
                                            g.Any(cu => cu.UserId == request.TargetUserId)
                                        )
                                        .Select(g => g.Key);
                var conversationId = await queryConversation.SingleOrDefaultAsync();
                if (conversationId != 0)
                {
                    return new IdResponse { Data = conversationId, Message = "Chat already exists.", StatusCode = HttpStatusCode.OK };
                }

                var targetUsername = await _context.Users
                                    .Where(u => u.Id == request.TargetUserId)
                                    .Select(u => u.Username)
                                    .SingleAsync();

                var conversationModel = new Conversation
                {
                    IsGroup = false,
                    Name = targetUsername,
                    CreatedAt = DateTime.Now,
                };

                await _context.Conversations.AddAsync(conversationModel);
                await _context.SaveChangesAsync();

                var conversationUsers = new List<ConversationUser>
                {
                    new ConversationUser { ConversationId = conversationModel.Id, UserId = currentUserId, JoinedAt = DateTime.Now },
                    new ConversationUser { ConversationId = conversationModel.Id, UserId = request.TargetUserId, JoinedAt = DateTime.Now }
                };

                await _context.ConversationUsers.AddRangeAsync(conversationUsers);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new IdResponse { Data = conversationModel.Id, Message = "PrivateGroup Created successfully.", StatusCode = HttpStatusCode.Created };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new IdResponse { Data = 0, Message = $"An error occurred: {ex.Message}", StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        public async Task<ApiResponse> UpdateGroupAsync(UpdateGroupRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var GroupModel = await _context.Conversations.SingleAsync(c => c.Id == request.GroupId);
                GroupModel.Name = request.Name;
                GroupModel.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ApiResponse { Message = "Group Upddated successfully.", StatusCode = HttpStatusCode.Created };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponse { Message = $"An error occurred: {ex.Message}", StatusCode = HttpStatusCode.InternalServerError };
            }

        }

        public async Task<PagedResponse<MessageResponse>> GetMessagesAsync(int groupId, string? cursor, int pageSize)
        {
            var queryMessage = from m in _context.Messages
                               where m.ConversationId == groupId
                               select m;

            if (cursor != null)
            {
                var payload = _cursorService.DecodeCursor(cursor);
                if (payload == null || payload.Type != CursorType.Group)
                {
                    return new PagedResponse<MessageResponse> { Message = "Invalid cursor.", StatusCode = HttpStatusCode.BadRequest };
                }
                queryMessage = queryMessage.Where(m => m.CreatedAt < payload.CreatedAt || (m.CreatedAt == payload.CreatedAt && m.Id < payload.Id));
            }

            var messages = await (from m in queryMessage
                                  orderby m.CreatedAt descending, m.Id descending
                                  select new
                                  {
                                      m.Id,
                                      m.Content,
                                      m.CreatedAt,
                                      m.SenderId,
                                      m.Type,
                                  })
                                  .Take(pageSize + 1)
                                  .ToListAsync();

            var data = (from m in messages
                        select new MessageResponse
                        {
                            Id = m.Id,
                            SenderId = m.SenderId,
                            Content = m.Content,
                            Type = m.Type,
                            CreatedAt = m.CreatedAt
                        })
                        .Take(pageSize + 1)
                        .ToList();

            var lastMessage = data.LastOrDefault();
            bool hasNextPage = messages.Count() > pageSize;
            return new PagedResponse<MessageResponse>
            {
                Data = data,
                LastCursor = data.Any() ? _cursorService.EncodeCursor(new CursorPayload { Type = CursorType.Group, Id = data.Last().Id, CreatedAt = data.Last().CreatedAt }) : null,
                HasNextPage = hasNextPage,
                Message = "Message retrieved successfully.",
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}