using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.ViewModels
{
    public enum FollowStatus
    {
        Pending,
        Accepted,
        Rejected
    }
    public class FollowRequest
    {
        public int FollowerId { get; set; }
        public int FollowingId { get; set; }
    }
    public class AcceptRequest
    {
        public int FollowId { get; set; }
        public int Status { get; set; }
    }
    public class GetFollowResponse
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}