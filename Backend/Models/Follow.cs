using System;
using System.Collections.Generic;

namespace Backend.Models;

public partial class Follow
{
    public int Id { get; set; }

    public int FollowerId { get; set; }

    public int FollowingId { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User Follower { get; set; } = null!;

    public virtual User Following { get; set; } = null!;
}
