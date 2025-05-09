using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.ViewModels
{
    enum ChatMessageType{
        text,
        image,
        file
    }
    public class MessageResponse
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int Type { get; set; }
        public int SenderId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateGroupRequest
    {
        public string Name { get; set; }
    }
    public class UpdateGroupRequest
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
    }
}