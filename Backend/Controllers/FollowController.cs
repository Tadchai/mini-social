using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;
using Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class FollowController : ControllerBase
    {
        private readonly MiniSocialContext _context;
        public FollowController(MiniSocialContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Follow([FromBody] FollowRequest request)
        {
            if (request.FollowerId == request.FollowingId)
                return new JsonResult(new ApiResponse { Message = "Unable to follow myself", StatusCode = HttpStatusCode.BadRequest });

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var followModel = new Follow
                    {
                        FollowerId = request.FollowerId,
                        FollowingId = request.FollowingId,
                        Status = (int)FollowStatus.Pending,
                        CreatedAt = DateTime.Now
                    };
                    await _context.Follows.AddAsync(followModel);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new JsonResult(new ApiResponse { Message = "Follow successfully.", StatusCode = HttpStatusCode.OK });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new JsonResult(new ApiResponse { Message = $"An error occurred: {ex.Message}", StatusCode = HttpStatusCode.InternalServerError });
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Accept([FromBody] AcceptRequest request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var followModel = await _context.Follows.SingleAsync(x => x.Id == request.FollowId);
                    if(request.Status == (int)FollowStatus.Accepted)
                    {
                        followModel.Status = (int)FollowStatus.Accepted;
                        followModel.UpdatedAt = DateTime.Now;
                    }
                    else if(request.Status == (int)FollowStatus.Rejected)
                    {
                        followModel.Status = (int)FollowStatus.Rejected;
                        followModel.UpdatedAt = DateTime.Now;
                    }
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new JsonResult(new ApiResponse { Message = "Follow successfully.", StatusCode = HttpStatusCode.OK });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new JsonResult(new ApiResponse { Message = $"An error occurred: {ex.Message}", StatusCode = HttpStatusCode.InternalServerError });
                }
            }
        }

    }
}