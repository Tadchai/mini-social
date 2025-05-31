using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Backend.Models;
using Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ChatController : Controller
    {
        private readonly MiniSocialContext _context;

        public ChatController(MiniSocialContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages([FromQuery] int groupId, DateTime? lastCreatedAt = null, int? lastId = null, int pageSize = 5)
        {
            try
            {
                var queryMessage = from m in _context.Messages
                                   where m.ConversationId == groupId
                                   select m;

                if (lastCreatedAt.HasValue && lastId.HasValue)
                {
                    queryMessage = queryMessage.Where(m => m.CreatedAt < lastCreatedAt.Value || (m.CreatedAt == lastCreatedAt.Value && m.Id < lastId.Value));
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
                return new JsonResult(new ApiWithPagedResponse<MessageResponse>
                {
                    Data = data,
                    LastCursor = 
                    {
                        CreatedAt = lastMessage.CreatedAt,
                        Id = lastMessage.Id
                    },
                    HasNextPage = hasNextPage,
                    Message = "Message retrieved successfully.",
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new ApiResponse { Message = $"An error occurred: {ex.Message}", StatusCode = HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
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
                    return new JsonResult(new ApiResponse { Message = "Group Created successfully.", StatusCode = HttpStatusCode.Created });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new JsonResult(new ApiResponse { Message = $"An error occurred: {ex.Message}", StatusCode = HttpStatusCode.InternalServerError });
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePrivate([FromBody] CreatePrivateGroupRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

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
                    return new JsonResult(new ApiWithIdResponse { Id = conversationId, Message = "Chat already exists.", StatusCode = HttpStatusCode.OK });
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

                return new JsonResult(new ApiWithIdResponse { Id = conversationModel.Id, Message = "PrivateGroup Created successfully.", StatusCode = HttpStatusCode.Created });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new JsonResult(new ApiResponse { Message = $"An error occurred: {ex.Message}", StatusCode = HttpStatusCode.InternalServerError });
            }
        }


        [HttpPost]
        public async Task<IActionResult> UpdateGroup([FromBody] UpdateGroupRequest request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var GroupModel = await _context.Conversations.SingleAsync(c => c.Id == request.GroupId);
                    GroupModel.Name = request.Name;
                    GroupModel.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new JsonResult(new ApiResponse { Message = "Group Upddated successfully.", StatusCode = HttpStatusCode.Created });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new JsonResult(new ApiResponse { Message = $"An error occurred: {ex.Message}", StatusCode = HttpStatusCode.InternalServerError });
                }
            }
        }

    }
}