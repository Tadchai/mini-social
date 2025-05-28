using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Backend.Helpers;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SearchController : Controller
    {
        private readonly MiniSocialContext _context;

        public SearchController(MiniSocialContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> SearchPosts([FromQuery] string q, [FromQuery] string? cursor)
        {
            const int pageSize = 10;
            var lastId = CursorHelper.DecodeCursor(cursor);

            var query = _context.Posts.Where(p => p.Content.Contains(q));

            if (lastId.HasValue)
            {
                query = query.Where(p => p.Id < lastId.Value); 
            }

            var posts = await query
                .OrderByDescending(p => p.Id)
                .Take(pageSize)
                .ToListAsync();

            var nextCursor = posts.Count == pageSize
                ? CursorHelper.EncodeCursor(posts.Last().Id)
                : null;

            return Ok(new
            {
                results = posts,
                nextCursor = nextCursor
            });
        }


    }
}