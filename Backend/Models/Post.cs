﻿using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class Post
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<PostImage> PostImages { get; set; } = new List<PostImage>();

    public virtual User User { get; set; } = null!;
}
