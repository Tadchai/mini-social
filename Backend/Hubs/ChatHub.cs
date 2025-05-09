using Backend.Models;
using Backend.ViewModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Backend.Hubs
{
    public class ChatHub : Hub
    {
        private readonly MiniSocialContext _context;
        public ChatHub(MiniSocialContext context)
        {
            _context = context;
        }

        public async Task JoinConversation(int conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        }

        public async Task SendMessage(int conversationId, int senderId, string content, int type = (int)ChatMessageType.text)
        {
            var message = new Message
            {
                ConversationId = conversationId,
                SenderId = senderId,
                Content = content,
                Type = type,
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