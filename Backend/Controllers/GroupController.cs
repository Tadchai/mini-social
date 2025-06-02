using System.Security.Claims;
using Backend.Services.Interfaces;
using Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class GroupController : Controller
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet]
        public async Task<IActionResult> Messages([FromQuery] int groupId, DateTime? lastCreatedAt = null, int? lastId = null, int pageSize = 5)
        {
            var result = await _groupService.GetMessagesAsync(groupId, lastCreatedAt, lastId);
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGroupRequest request)
        {
            var result = await _groupService.CreateGroupAsync(request);
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePrivate([FromBody] CreatePrivateGroupRequest request)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _groupService.CreatePrivateAsync(request, currentUserId);
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] UpdateGroupRequest request)
        {
            var result = await _groupService.UpdateGroupAsync(request);
            return new JsonResult(result);
        }
    }
}