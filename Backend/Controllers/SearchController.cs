using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Backend.Helpers;
using Backend.Models;
using Backend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Backend.Helpers.CursorHelper;

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
            if (string.IsNullOrWhiteSpace(q))
                return new JsonResult(new ApiResponse { Message = "Query string 'q' is required.", StatusCode = HttpStatusCode.BadRequest });

            const int pageSize = 10;

            var query = _context.Posts.Where(p => EF.Functions.Like(p.Content, $"%{q}%"));

            if (cursor != null)
            {
                var (type, id) = CursorHelper.DecodeCursor(cursor);
                if (type != CursorType.Post || id == null)
                {
                    return new JsonResult(new ApiResponse { Message = "Invalid cursor.", StatusCode = HttpStatusCode.BadRequest });
                }
                query = query.Where(p => p.Id < id);
            }

            var posts = await (from p in query
                               orderby p.Id descending
                               select new
                               {
                                   Id = p.Id,
                                   Content = p.Content,
                                   CreatedAt = p.CreateAt
                               })
                               .Take(pageSize + 1)
                               .ToListAsync();

            var postImages = await (from pi in _context.PostImages
                                    where posts.Select(x => x.Id).Contains(pi.PostId)
                                    select new
                                    {
                                        PostId = pi.PostId,
                                        Id = pi.Id,
                                        ImageUrl = pi.ImageUrl
                                    })
                                    .ToListAsync();

            var postDic = posts.ToDictionary(x => x.Id, x => new List<ImageResponse>());
            foreach (var image in postImages)
            {
                postDic[image.PostId].Add(new ImageResponse
                {
                    Id = image.Id,
                    ImageUrl = image.ImageUrl
                });
            }

            var data = (from p in posts
                        select new PostResponse
                        {
                            Id = p.Id,
                            Content = p.Content,
                            ImageUrl = postDic[p.Id],
                            CreatedAt = p.CreatedAt
                        })
                        .Take(pageSize)
                        .ToList();

            bool hasNextPage = posts.Count() > pageSize;

            return new JsonResult(new ApiWithPagedResponse<PostResponse>
                {
                    Data = data,
                    LastCursor = data.Any() ? new LastCursor { CreatedAt = data.Last().CreatedAt, Id = data.Last().Id } : null,
                    HasNextPage = hasNextPage,
                    Message = "Search Posts successfully.",
                    StatusCode = HttpStatusCode.OK
                });
        }


    }
}