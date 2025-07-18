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
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<IActionResult> My([FromQuery] string? cursor, [FromQuery] int pageSize = 5)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _postService.GetByIdAsync(userId, cursor, pageSize);
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> Follow([FromQuery] string? cursor, [FromQuery] int pageSize = 5)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _postService.GetByFollowAsync(userId, cursor, pageSize);
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreatePostRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _postService.CreatePostAsync(request, userId, Request);
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> Update([FromBody] UpdatePostRequest request)
        {
            var result = await _postService.UpdatePostAsync(request);
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<ActionResult> Delete([FromBody] DeletePostRequest request)
        {
            var result = await _postService.DeletePostAsync(request);
            return new JsonResult(result);
        }
    }
}