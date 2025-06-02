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
        public async Task<IActionResult> Posts([FromQuery] string q, [FromQuery] string? cursor)
        {
            var result = await _searchService.SearchPostsAsync(q, cursor);
            return new JsonResult(result);
        }
    }
}