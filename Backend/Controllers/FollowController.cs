using System.Security.Claims;
using Backend.Services.Interfaces;
using Backend.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class FollowController : ControllerBase
    {
        private readonly IFollowService _followService;
        public FollowController(IFollowService followService)
        {
            _followService = followService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _followService.GetAsync(userId);
            return new JsonResult(result);
        }

        [HttpPost]
        public new async Task<IActionResult> Request([FromBody] RequestFollowRequest request)
        {
            var result = await _followService.RequestFollowAsync(request);
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Accept([FromBody] AcceptRequest request)
        {
            var result = await _followService.AcceptAsync(request);
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> Contact()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _followService.GetContactAsync(userId);
            return new JsonResult(result);
        }
    }
}