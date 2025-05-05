﻿using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class Message
{
    public int Id { get; set; }

    public int ConversationId { get; set; }

    public int SenderId { get; set; }

    public string Content { get; set; } = null!;

    public int Type { get; set; }

    public DateTime CreatedAt { get; set; }
}
