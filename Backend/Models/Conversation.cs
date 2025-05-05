using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class Conversation
{
    public int Id { get; set; }

    public bool IsGroup { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
