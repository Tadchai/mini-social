using Backend.Models;
using Backend.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Backend.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly MiniSocialContext _context;
        public ChatHub(MiniSocialContext context)
        {
            _context = context;
        }

        public async Task JoinConversation(int conversationId)
        {
            Console.WriteLine($"Joining conversation {conversationId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
        }

        public async Task SendMessage(int conversationId, string content)
        {

            var senderId = int.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var message = new Message
            {
                ConversationId = conversationId,
                SenderId = senderId,
                Content = content,
                Type = (int)ChatMessageType.text,
                CreatedAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            await Clients.Group($"conversation_{conversationId}").SendAsync("ReceiveMessage", new
            {
                message.Id,
                message.SenderId,
                message.Content,
                message.Type,
                message.CreatedAt,
                message.ConversationId
            });
        }
    }
}