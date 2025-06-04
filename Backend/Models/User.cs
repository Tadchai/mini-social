using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Salt { get; set; } = null!;

    public string? ProfilePic { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<ConversationUser> ConversationUsers { get; set; } = new List<ConversationUser>();

    public virtual ICollection<Follow> FollowFollowers { get; set; } = new List<Follow>();

    public virtual ICollection<Follow> FollowFollowings { get; set; } = new List<Follow>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
