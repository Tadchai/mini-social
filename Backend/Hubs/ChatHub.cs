using Backend.Models;
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

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendMessage(int groupId, int userId, string text)
        {
            var group = await _context.Conversations.SingleAsync(c => c.Id == groupId);
            var user = await _context.Users.SingleAsync(u => u.Id == userId);
            if (group == null || user == null) return;

            var messageModel = new Message
            {
                ConversationId = groupId,
                SenderId = userId,
                Content = text,
                CreatedAt = DateTime.Now,
            };
            _context.Messages.Add(messageModel);
            await _context.SaveChangesAsync();

            await Clients.Group(group.Name).SendAsync("ReceiveMessage", user.Username, text);
        }
    }
}