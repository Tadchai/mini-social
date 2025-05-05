using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class ConversationUser
{
    public int Id { get; set; }

    public int ConversationId { get; set; }

    public int UserId { get; set; }

    public DateTime JoinedAt { get; set; }
}
