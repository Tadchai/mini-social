using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class Post
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Content { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual ICollection<PostImage> PostImages { get; set; } = new List<PostImage>();
}
