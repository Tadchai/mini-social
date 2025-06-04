using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SearchController : Controller
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        public async Task<IActionResult> Posts([FromQuery] string q, [FromQuery] string? cursor, [FromQuery] int pageSize = 5)
        {
            var result = await _searchService.SearchPostsAsync(q, cursor, pageSize);
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> Users([FromQuery] string q, [FromQuery] string? cursor, [FromQuery] int pageSize = 5)
        {
            var result = await _searchService.SearchUsersAsync(q, cursor, pageSize);
            return new JsonResult(result);
        }
    }
}